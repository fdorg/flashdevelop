using System.Collections.Generic;

namespace HaXeContext.Completion
{
    public class ContextFeatures : ASCompletion.Completion.ContextFeatures
    {
        public string abstractKey = "abstract";
        public string macroKey = "macro";

        protected override void GetDeclarationKeywords(string foundMember, List<string> result)
        {
            if (foundMember == abstractKey)
            {
                result.Add("to");
                result.Add("from");
            }
            base.GetDeclarationKeywords(foundMember, result);
        }
    }
}