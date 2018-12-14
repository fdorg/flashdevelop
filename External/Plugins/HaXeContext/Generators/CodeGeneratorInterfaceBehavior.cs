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
            return sci.GetWordFromPosition(position) is string token && token.Length > 0 && char.IsUpper(token[0])
                   && base.CanShowGenerateExtends(sci, position, expr, found);
        }

        protected override void ShowGenerateField(ScintillaControl sci, ASResult expr, FoundDeclaration found, List<ICompletionListItem> options)
        {
            var label = TextHelper.GetString("ASCompletion.Label.GeneratePublicVar");
            options.Add(new GeneratorItem(label, (GeneratorJobType) GeneratorJob.IVariable, () =>
            {
                sci.BeginUndoAction();
                try
                {
                    GenerateProperty(sci, new MemberModel {Name = expr.Context.Value}, TemplateUtils.GetTemplate("IVariable"));
                }
                finally { sci.EndUndoAction(); }
            }));
        }
    }
}
