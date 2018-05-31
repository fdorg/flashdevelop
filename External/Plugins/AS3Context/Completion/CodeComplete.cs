﻿using System.Linq;
using System.Text.RegularExpressions;
using ASCompletion.Completion;
using ASCompletion.Context;
using ASCompletion.Model;
using PluginCore;

namespace AS3Context.Completion
{
    class CodeComplete : ASComplete
    {
        static readonly Regex re_asExpr = new Regex(@"\((?<lv>.+)\s(?<op>as)\s+(?<rv>\w+)\)");
        static readonly Regex re_isExpr = new Regex(@"\((?<lv>.+)\s(?<op>is)\s+(?<rv>\w+)\)");

        protected override ASResult EvalExpression(string expression, ASExpr context, FileModel inFile, ClassModel inClass, bool complete, bool asFunction, bool filterVisibility)
        {
            if (expression != null)
            {
                var ctx = ASContext.Context;
                var features = ctx.Features;
                if (context.SubExpressions != null && context.SubExpressions.Count > 0)
                {
                    var count = context.SubExpressions.Count;
                    // transform #2~.#1~.#0~ to #2~.[].[]
                    for (var i = 0; i < count; i++)
                    {
                        var subExpression = context.SubExpressions[i];
                        if (subExpression.Length < 2 || subExpression[0] != '[') continue;
                        // for example: [].<complete>
                        if (expression[0] == '#' && i == count - 1)
                        {
                            var type = ctx.ResolveType(features.arrayKey, inFile);
                            expression = type.Name + ".#" + expression.Substring(("#" + i + "~").Length);
                            context.SubExpressions.RemoveAt(i);
                            return base.EvalExpression(expression, context, inFile, inClass, complete, asFunction, filterVisibility);
                        }
                        expression = expression.Replace(">.#" + i + "~", ">" + subExpression);
                        expression = expression.Replace(".#" + i + "~", "." + subExpression);
                    }
                }
                var c = expression[0];
                if (c == '"' || c == '\'')
                {
                    var type = ctx.ResolveType(features.stringKey, inFile);
                    // for example: ""|, ''|
                    if (context.SubExpressions == null) expression = type.Name + ".#.";
                    // for example: "".<complete>, ''.<complete>
                    else
                    {
                        var pattern = c + ".#" + (context.SubExpressions.Count - 1) + "~";
                        var startIndex = expression.IndexOfOrdinal(pattern) + pattern.Length;
                        expression = type.Name + ".#" + expression.Substring(startIndex);
                        if (context.SubExpressions.Count == 1) context.SubExpressions = null;
                    }
                }
                // for example: new <T>[].<complete>
                else if (expression.Contains(">.[")) expression = expression.Replace(">.[", ">[");
                // transform Vector.<T> to Vector<T>
                else if (expression.Contains(".<")) expression = expression.Replace(".<", "<");
                // for example: ~/pattern/.<complete>
                else if (expression.StartsWithOrdinal("#RegExp")) expression = expression.Substring(1);
                else if (context.SubExpressions != null && context.SubExpressions.Count > 0)
                {
                    var expr = context.SubExpressions.Last();
                    // for example: (v as T).<complete>, (v is Complete).<complete>, ...
                    if (expr.Length >= 8 /*"(v as T)".Length*/ && expr[0] == '(')
                    {
                        ClassModel type = null;
                        var m = re_asExpr.Match(expr);
                        if (m.Success) type = ctx.ResolveType(m.Groups["rv"].Value.Trim(), inFile);
                        if (type == null && re_isExpr.IsMatch(expr)) type = ctx.ResolveType(features.booleanKey, inFile);
                        if (type != null)
                        {
                            expression = type.Name + ".#" + expression.Substring(("#" + (context.SubExpressions.Count - 1) + "~").Length);
                            context.SubExpressions.RemoveAt(context.SubExpressions.Count - 1);
                        }
                    }
                }
            }
            return base.EvalExpression(expression, context, inFile, inClass, complete, asFunction, filterVisibility);
        }
    }
}
