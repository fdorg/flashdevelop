using System.Collections.Generic;
using ASCompletion.Completion;
using PluginCore;
using PluginCore.Localization;
using ScintillaNet;

namespace HaXeContext.Generators
{
    class CodeGeneratorAbstractBehavior : ASCompletion.Generators.CodeGeneratorInterfaceBehavior
    {
        protected override bool CanShowGenerateExtends(ScintillaControl sci, int position, ASResult expr, FoundDeclaration found)
        {
            return false;
        }

        protected override void ShowGenerateField(ScintillaControl sci, ASResult expr, FoundDeclaration found, List<ICompletionListItem> options)
        {
            var label = TextHelper.GetString("ASCompletion.Label.GeneratePrivateVar");
            options.Add(new GeneratorItem(label, GeneratorJobType.Variable, () => {}));
            label = TextHelper.GetString("ASCompletion.Label.GeneratePublicVar");
            options.Add(new GeneratorItem(label, GeneratorJobType.VariablePublic, () => {}));
        }

        protected override void ShowGenerateMethod(ScintillaControl sci, ASResult expr, FoundDeclaration found, ICollection<ICompletionListItem> options)
        {
            var label = TextHelper.GetString("ASCompletion.Label.GeneratePrivateFunction");
            options.Add(new GeneratorItem(label, GeneratorJobType.Function, () => {}));
            label = TextHelper.GetString("ASCompletion.Label.GenerateFunctionPublic");
            options.Add(new GeneratorItem(label, GeneratorJobType.FunctionPublic, () => {}));
        }
    }
}
