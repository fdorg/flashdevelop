using System.Collections.Generic;
using ASCompletion.Completion;
using PluginCore;
using ScintillaNet;

namespace HaXeContext.Generators
{
    class CodeGeneratorInterfaceStrategy : ASCompletion.Generators.CodeGeneratorInterfaceStrategy
    {
        protected override void ShowCustomGenerators(ScintillaControl sci, ASResult expr, FoundDeclaration found, ICollection<ICompletionListItem> options)
        {
            base.ShowCustomGenerators(sci, expr, found, options);
        }
    }
}
