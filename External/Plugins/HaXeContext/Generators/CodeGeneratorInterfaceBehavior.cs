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
        protected override bool CanShowGenerateExtends(ScintillaControl sci, int position, ASResult expr, FoundDeclaration found)
        {
            var token = sci.GetWordFromPosition(position);
            return !string.IsNullOrEmpty(token) && char.IsUpper(token[0])
                   && base.CanShowGenerateExtends(sci, position, expr, found);
        }

        protected override void ShowMemberGenerator(ScintillaControl sci, ASResult expr, FoundDeclaration found, ICollection<ICompletionListItem> options)
        {
            var member = new MemberModel {Name = expr.Context.Value};
            var label = TextHelper.GetString("ASCompletion.Label.GeneratePublicVar");
            options.Add(new GeneratorItem(label, (GeneratorJobType) GeneratorJob.IVariable, () => GenerateProperty(sci, member, TemplateUtils.GetTemplate("IVariable"))));
        }
    }
}
