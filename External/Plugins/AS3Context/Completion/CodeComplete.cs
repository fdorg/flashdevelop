using System.Text.RegularExpressions;
using ASCompletion.Completion;
using ASCompletion.Context;
using ASCompletion.Model;

namespace AS3Context.Completion
{
    class CodeComplete : ASComplete
    {
        static readonly Regex re_asExpr = new Regex(@"\((?<lv>.+)\s(?<op>as)\s+(?<rv>\w+)\)");

        protected override ASResult EvalExpression(string expression, ASExpr context, FileModel inFile, ClassModel inClass, bool complete, bool asFunction, bool filterVisibility)
        {
            if (expression != null && context.SubExpressions != null && context.SubExpressions.Count > 0)
            {
                var lastIndex = context.SubExpressions.Count - 1;
                var subExpr = context.SubExpressions[lastIndex];
                if (subExpr.Length >= 8/*"(v as T)".Length*/ && subExpr[0] == '(')
                {
                    var m = re_asExpr.Match(subExpr);
                    if (m.Success)
                    {
                        var type = ASContext.Context.ResolveType(m.Groups["rv"].Value.Trim(), inFile);
                        expression = type.Name + ".#" + expression.Substring(("#" + lastIndex + "~").Length);
                    }
                }
            }
            return base.EvalExpression(expression, context, inFile, inClass, complete, asFunction, filterVisibility);
        }
    }
}
