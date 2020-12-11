// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
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