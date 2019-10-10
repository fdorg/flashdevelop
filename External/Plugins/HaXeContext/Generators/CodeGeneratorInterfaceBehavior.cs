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
        protected override bool CanShowNewInterfaceList(ScintillaControl sci, int position, ASResult expr, FoundDeclaration found)
        {
            return sci.GetWordFromPosition(position) is { } token
                   && token.Length > 0
                   && char.IsUpper(token[0])
                   && base.CanShowNewInterfaceList(sci, position, expr, found);
        }

        protected override void ShowNewVariableList(ScintillaControl sci, ASResult expr, FoundDeclaration found, ICollection<ICompletionListItem> options)
        {
            var label = TextHelper.GetString("ASCompletion.Label.GeneratePublicVar");
            options.Add(new GeneratorItem(label, (GeneratorJobType) GeneratorJob.IVariable, () =>
            {
                sci.BeginUndoAction();
                try
                {
                    GenerateProperty(sci, new MemberModel {Name = expr.Context.Value}, TemplateUtils.GetTemplate("IVariable"));
                }
                finally
                {
                    sci.EndUndoAction();
                }
            }));
        }
    }
}
