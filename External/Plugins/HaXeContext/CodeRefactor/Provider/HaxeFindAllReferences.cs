// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using ASCompletion.Completion;
using ASCompletion.Context;
using CodeRefactor.Commands;
using CodeRefactor.Provider;
using PluginCore.FRService;
using ScintillaNet;

namespace HaXeContext.CodeRefactor.Commands
{
    public class HaxeFindAllReferences : FindAllReferences
    {
        bool includeStrings = false;

        public HaxeFindAllReferences(ASResult target, bool output, bool ignoreDeclarations) : base(target, output, ignoreDeclarations)
        {
        }

        protected override void ExecutionImplementation()
        {
            if (IncludeStrings) includeStrings = true;
            IncludeStrings = true;
            base.ExecutionImplementation();
        }

        protected override bool IsInsideCommentOrString(SearchMatch match, ScintillaControl sci, bool includeComments, bool includeStrings)
        {
            if (this.includeStrings) return base.IsInsideCommentOrString(match, sci, includeComments, includeStrings);
            return ASContext.Context.CodeComplete.IsStringInterpolationStyle(sci, match.Index)
                   && RefactoringHelper.DoesMatchPointToTarget(sci, match, CurrentTarget, AssociatedDocumentHelper);
        }
    }
}