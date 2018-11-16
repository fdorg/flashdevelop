using System.Linq;
using ASCompletion.Completion;
using ASCompletion.Context;
using ASCompletion.Model;
using PluginCore;

namespace AS3Context.Completion
{
    class CodeComplete : ASComplete
    {
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
                    || (expression.Length > 2 && expression[0] == '-' && expression[1] == '-' && char.IsDigit(expression, 2)))
                {
                    int p;
                    int pe1;
                    var pe2 = -1;
                    if ((pe1 = expression.IndexOfOrdinal("e-")) != -1 || (pe2 = expression.IndexOfOrdinal("e+")) != -1)
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
                        return base.EvalExpression(expression, context, inFile, inClass, complete, asFunction, filterVisibility);
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
                            return base.EvalExpression(expression, context, inFile, inClass, complete, asFunction, filterVisibility);
                        }
                        expression = expression.Replace(">.#" + i + "~", ">" + subExpression);
                        expression = expression.Replace(".#" + i + "~", "." + subExpression);
                    }
                }
                if (expression.Length > 1 && expression[0] is char c && (c == '"' || c == '\''))
                {
                    var type = ResolveType(features.stringKey, inFile);
                    // for example: ""|, ''|
                    if (context.SubExpressions == null) expression = type.Name + ".#.";
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
                else if (context.SubExpressions != null && context.SubExpressions.Count > 0)
                {
                    var expr = context.SubExpressions.Last();
                    // for example: (v as T).<complete>, (v is Complete).<complete>, ...
                    if (expr.Length >= 8 /*"(v as T)".Length*/ && expr[0] == '(')
                    {
                        var type = ctx.ResolveToken(expr, inFile);
                        if (!type.IsVoid())
                        {
                            expression = type.Name + ".#" + expression.Substring(("#" + (context.SubExpressions.Count - 1) + "~").Length);
                            context.SubExpressions.RemoveAt(context.SubExpressions.Count - 1);
                            if (context.SubExpressions.Count == 0) context.SubExpressions = null;
                        }
                    }
                }
            }
            return base.EvalExpression(expression, context, inFile, inClass, complete, asFunction, filterVisibility);
        }
    }
}
