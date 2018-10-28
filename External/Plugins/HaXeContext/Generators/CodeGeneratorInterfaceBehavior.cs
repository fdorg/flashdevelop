using System.Collections.Generic;
using ASCompletion.Completion;
using ASCompletion.Model;
using PluginCore;
using PluginCore.Localization;
using ScintillaNet;

namespace HaXeContext.Generators
{
    class CodeGeneratorInterfaceBehavior : ASCompletion.Generators.CodeGeneratorInterfaceBehavior
    {
        protected override bool CanShowGenerateNewInterface(ScintillaControl sci, int position, ASResult expr, FoundDeclaration found)
        {
            var token = sci.GetWordFromPosition(position);
            return !string.IsNullOrEmpty(token) && char.IsUpper(token[0])
                   && base.CanShowGenerateNewInterface(sci, position, expr, found);
        }

        protected override void ShowCustomGenerators(ScintillaControl sci, ASResult expr, FoundDeclaration found, ICollection<ICompletionListItem> options)
        {
            var member = new MemberModel {Name = expr.Context.Value};
            var label = TextHelper.GetString("ASCompletion.Label.GeneratePublicVar");
            options.Add(new GeneratorItem(label, GeneratorJob.IVariable, () => GenerateAccessor(sci, member, TemplateUtils.GetTemplate("IVariable"))));
        }
    }
}
