using System.Collections.Generic;
using System.Linq;
using ASCompletion.Completion;
using ASCompletion.Context;
using ASCompletion.Model;
using PluginCore;
using ScintillaNet;

namespace AS3Context.Completion
{
    class CodeComplete : ASComplete
    {
        /// <inheritdoc />
        protected override ASResult EvalExpression(string expression, ASExpr context, FileModel inFile, ClassModel inClass, bool complete, bool asFunction, bool filterVisibility)
        {
            if (!string.IsNullOrEmpty(expression))
            {
                var ctx = ASContext.Context;
                var features = ctx.Features;
                // for example: 1.0.<complete>, 5e-324.<complete>
                if (char.IsDigit(expression, 0)
                    // for example: -1.<complete>
                    || (expression.Length > 1 && expression[0] == '-' && char.IsDigit(expression, 1))
                    // for example: --1.<complete>
                    || (expression.Length > 2 && expression[0] == '-' && expression[1] == '-' &&
                        char.IsDigit(expression, 2)))
                {
                    int p;
                    var pe2 = -1;
                    if (expression.Contains("e-", out var pe1) || expression.Contains("e+", out pe2))
                    {
                        p = expression.IndexOf('.');
                        if (p == -1) p = expression.Length - 1;
                        else if (p < pe1 || p < pe2)
                        {
                            var p2 = expression.IndexOf('.', p + 1);
                            p = p2 != -1 ? p2 : expression.Length - 1;
                        }
                    }
                    else
                    {
                        p = expression.IndexOf('.');
                        if (p == expression.Length - 1) p = -1;
                        else if (p != -1)
                        {
                            // for example: 1.0.<complete>
                            if (char.IsDigit(expression[p + 1]))
                            {
                                var p2 = expression.IndexOf('.', p + 1);
                                p = p2 != -1 ? p2 : expression.Length - 1;
                            }
                            // for example: -1.valueOf().<complete>
                            else p = -1;
                        }
                    }

                    if (p != -1)
                    {
                        expression = "Number.#." + expression.Substring(p + 1);
                        return base.EvalExpression(expression, context, inFile, inClass, complete, asFunction,
                            filterVisibility);
                    }
                }

                if (context.SubExpressions != null)
                {
                    var count = context.SubExpressions.Count - 1;
                    // transform #2~.#1~.#0~ to #2~.[].[]
                    for (var i = 0; i <= count; i++)
                    {
                        var subExpression = context.SubExpressions[i];
                        if (subExpression.Length < 2 || subExpression[0] != '[') continue;
                        // for example: [].<complete>
                        if (expression[0] == '#' && i == count)
                        {
                            var type = ResolveType(features.arrayKey, inFile);
                            if (type.IsVoid()) break;
                            expression = type.Name + ".#" + expression.Substring(("#" + i + "~").Length);
                            context.SubExpressions.RemoveAt(i);
                            return base.EvalExpression(expression, context, inFile, inClass, complete, asFunction,
                                filterVisibility);
                        }

                        expression = expression.Replace(">.#" + i + "~", ">" + subExpression);
                        expression = expression.Replace(".#" + i + "~", "." + subExpression);
                    }
                }

                if (expression.Length > 1 && expression[0] is { } c && (c == '"' || c == '\''))
                {
                    var type = ResolveType(features.stringKey, inFile);
                    // for example: ""|, ''|
                    if (context.SubExpressions is null) expression = type.Name + ".#.";
                    // for example: "".<complete>, ''.<complete>
                    else
                    {
                        var pattern = c + ".#" + (context.SubExpressions.Count - 1) + "~";
                        var startIndex = expression.IndexOfOrdinal(pattern) + pattern.Length;
                        expression = type.Name + ".#" + expression.Substring(startIndex);
                    }
                }
                // for example: new <T>[].<complete>
                else if (expression.Contains(">.[")) expression = expression.Replace(">.[", ">[");
                // transform Vector.<T> to Vector<T>
                else if (expression.Contains(".<")) expression = expression.Replace(".<", "<");
                // for example: /pattern/.<complete>
                else if (expression.StartsWithOrdinal("#RegExp")) expression = expression.Substring(1);
                else if (!context.SubExpressions.IsNullOrEmpty())
                {
                    var expr = context.SubExpressions.Last();
                    // for example: (v as T).<complete>, (v is Complete).<complete>, ...
                    if (expr.Length >= 8 /*"(v as T)".Length*/ && expr[0] == '(')
                    {
                        var type = ctx.ResolveToken(expr, inFile);
                        if (!type.IsVoid())
                        {
                            expression = type.Name + ".#" +
                                         expression.Substring(("#" + (context.SubExpressions.Count - 1) + "~").Length);
                            context.SubExpressions.RemoveAt(context.SubExpressions.Count - 1);
                            if (context.SubExpressions.Count == 0) context.SubExpressions = null;
                        }
                    }
                }
            }

            return base.EvalExpression(expression, context, inFile, inClass, complete, asFunction, filterVisibility);
        }

        /// <inheritdoc />
        protected override bool HandleNewCompletion(ScintillaControl sci, string tail, bool autoHide, string keyword, List<ICompletionListItem> list)
        {
            if (keyword == "new")
            {
                list = list
                    .Where(it => !(it is MemberItem item)
                                 || item.Member is null
                                 || (item.Member.Flags & FlagType.Interface) == 0)
                    .ToList();
            }
            return base.HandleNewCompletion(sci, tail, autoHide, keyword, list);
        }

        /// <inheritdoc />
        protected override bool IsAvailableForToolTip(ScintillaControl sci, int position)
        {
            return base.IsAvailableForToolTip(sci, position)
                   || (sci.GetWordFromPosition(position) is { } word
                       && (word == "as" || word == "is" || word == "instanceof" || word == "typeof" || word == "delete"));
        }

        /// <inheritdoc />
        protected override string GetToolTipTextEx(ASResult expr)
        {
            if (expr.Member is null && expr.Context?.Value is {} s)
            {
                switch (s)
                {
                    // for example: variable as$(EntryPoint) Type
                    case "as":
                        expr.Member = Context.StubAsExpression;
                        break;
                    // for example: variable is$(EntryPoint) Type
                    case "is":
                        expr.Member = Context.StubIsExpression;
                        break;
                    // for example: variable instanceof$(EntryPoint) function
                    case "instanceof":
                        expr.Member = Context.StubInstanceOfExpression;
                        break;
                    // for example: typeof(EntryPoint) expression
                    case "typeof":
                        expr.Member = Context.StubTypeOfExpression;
                        break;
                    // for example: delete$(EntryPoint) reference
                    case "delete":
                        expr.Member = Context.StubDeleteExpression;
                        break;
                }
            }
            return base.GetToolTipTextEx(expr);
        }
    }
}