using System.Collections.Generic;

namespace HaXeContext.Completion
{
    public class ContextFeatures : ASCompletion.Completion.ContextFeatures
    {
        public readonly string AbstractKey = "abstract";
        public readonly string MacroKey = "macro";
        public readonly string ExternKey = "extern";

        protected override void GetDeclarationKeywords(string foundMember, List<string> result)
        {
            if (foundMember == AbstractKey)
            {
                result.Add("to");
                result.Add("from");
            }
            base.GetDeclarationKeywords(foundMember, result);
        }
    }
}