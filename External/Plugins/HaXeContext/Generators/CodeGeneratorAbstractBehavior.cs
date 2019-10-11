using System.Collections.Generic;
using ASCompletion.Completion;
using ASCompletion.Context;
using ASCompletion.Model;
using PluginCore;
using PluginCore.Localization;
using ScintillaNet;

namespace HaXeContext.Generators
{
    class CodeGeneratorAbstractBehavior : ASCompletion.Generators.CodeGeneratorDefaultBehavior
    {
        protected override bool CanShowGenerateExtends(ScintillaControl sci, int position, ASResult expr, FoundDeclaration found)
            => sci.GetWordFromPosition(position) is { } token
               && token.Length > 0
               && char.IsUpper(token[0])
               && ASComplete.IsTextStyle(sci.BaseStyleAt(position - 1))
               && ASContext.Context.CodeComplete.PositionIsBeforeBody(sci, position, found.InClass);

        protected override void ShowGenerateExtends(ScintillaControl sci, ASResult expr, FoundDeclaration found, ICollection<ICompletionListItem> options)
        {
            var label = TextHelper.GetString("ASCompletion.Label.GenerateClass");
            options.Add(new GeneratorItem(label, GeneratorJobType.Class, found.Member, found.InClass, expr));
            label = TextHelper.GetString("ASCompletion.Label.GenerateInterface");
            options.Add(new GeneratorItem(label, GeneratorJobType.Interface, found.Member, found.InClass, expr));
        }

        protected override void ShowGenerateField(ScintillaControl sci, ASResult expr, FoundDeclaration found, ICollection<ICompletionListItem> options)
        {
            var member = (MemberModel) found.Member?.Clone() ?? new MemberModel();
            member.Flags |= FlagType.Static;
            var label = TextHelper.GetString("ASCompletion.Label.GeneratePrivateStaticVar");
            options.Add(new GeneratorItem(label, GeneratorJobType.Variable, member, found.InClass));
            label = TextHelper.GetString("ASCompletion.Label.GeneratePublicStaticVar");
            options.Add(new GeneratorItem(label, GeneratorJobType.VariablePublic, member, found.InClass));
        }

        protected override bool CanShowGenerateProperty(ScintillaControl sci, int position, ASResult expr, FoundDeclaration found) 
            => sci.GetWordFromPosition(position) is { } token
               && token.Length > 0
               && expr.Member != null
               && expr.Member.Flags.HasFlag(FlagType.Variable)
               && !expr.Member.Flags.HasFlag(FlagType.Enum);

        protected override void ShowGenerateMethod(ScintillaControl sci, ASResult expr, FoundDeclaration found, ICollection<ICompletionListItem> options)
        {
            var label = TextHelper.GetString("ASCompletion.Label.GeneratePrivateFunction");
            options.Add(new GeneratorItem(label, GeneratorJobType.Function, found.Member, found.InClass));
            label = TextHelper.GetString("ASCompletion.Label.GenerateFunctionPublic");
            options.Add(new GeneratorItem(label, GeneratorJobType.FunctionPublic, found.Member, found.InClass));
        }
    }
}
