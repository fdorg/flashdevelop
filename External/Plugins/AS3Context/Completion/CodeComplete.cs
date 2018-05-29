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
                // for example: new <T>[].<complete>
                if (expression.Contains(">.[")) expression = expression.Replace(">.[", ">[");
                // for example: Vector.<T> -> Vector<T>
                else if (expression.Contains(".<")) expression = expression.Replace(".<", "<");
                // for example: ~/pattern/.<complete>
                else if (expression.StartsWithOrdinal("#RegExp")) expression = expression.Substring(1);
                else if (context.SubExpressions != null && context.SubExpressions.Count > 0)
                {
                    var lastIndex = context.SubExpressions.Count - 1;
                    var subExpr = context.SubExpressions[lastIndex];
                    // for example: (v as T).<complete>, (v is Complete).<complete>, ...
                    if (subExpr.Length >= 8/*"(v as T)".Length*/ && subExpr[0] == '(')
                    {
                        var ctx = ASContext.Context;
                        ClassModel type = null;
                        var m = re_asExpr.Match(subExpr);
                        if (m.Success) type = ctx.ResolveType(m.Groups["rv"].Value.Trim(), inFile);
                        if (type == null && re_isExpr.IsMatch(subExpr)) type = ctx.ResolveType(ctx.Features.booleanKey, inFile);
                        if (type != null) expression = type.Name + ".#" + expression.Substring(("#" + lastIndex + "~").Length);
                    }
                }
            }
            return base.EvalExpression(expression, context, inFile, inClass, complete, asFunction, filterVisibility);
        }
    }
}
