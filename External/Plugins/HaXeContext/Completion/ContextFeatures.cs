using System.Collections.Generic;

namespace HaXeContext.Completion
{
    public class ContextFeatures : ASCompletion.Completion.ContextFeatures
    {
        public readonly string AbstractKey = "abstract";
        public readonly string MacroKey = "macro";
        public readonly string ExternKey = "extern";
        public readonly string FromKey = "from";
        public readonly string ToKey = "to";

        protected override void GetDeclarationKeywords(string foundMember, List<string> result)
        {
            if (foundMember == AbstractKey)
            {
                result.Add(ToKey);
                result.Add(FromKey);
            }
            base.GetDeclarationKeywords(foundMember, result);
        }
    }
}