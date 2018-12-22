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
        {
            return sci.GetWordFromPosition(position) is string token && token.Length > 0 && char.IsUpper(token[0])
                   && ASComplete.IsTextStyle(sci.BaseStyleAt(position - 1))
                   && ASContext.Context.CodeComplete.PositionIsBeforeBody(sci, position, found.InClass);
                   //&& (expr.Context.WordBefore is string word && (word == "from" || word == "to"));
        }

        protected override void ShowGenerateExtends(ScintillaControl sci, ASResult expr, FoundDeclaration found, ICollection<ICompletionListItem> options)
        {
            var label = TextHelper.GetString("ASCompletion.Label.GenerateClass");
            options.Add(new GeneratorItem(label, GeneratorJobType.Class, found.Member, found.InClass, expr));
            label = TextHelper.GetString("ASCompletion.Label.GenerateInterface");
            options.Add(new GeneratorItem(label, GeneratorJobType.Interface, found.Member, found.InClass, expr));
        }

        protected override void ShowGenerateField(ScintillaControl sci, ASResult expr, FoundDeclaration found, List<ICompletionListItem> options)
        {
            MemberModel inMember;
            if (found.Member != null)
            {
                inMember = (MemberModel) found.Member.Clone();
                inMember.Flags |= FlagType.Static;
            }
            else inMember = new MemberModel {Flags = FlagType.Static};
            var label = TextHelper.GetString("ASCompletion.Label.GeneratePrivateVar");
            options.Add(new GeneratorItem(label, GeneratorJobType.Variable, inMember, found.InClass));
            label = TextHelper.GetString("ASCompletion.Label.GeneratePublicVar");
            options.Add(new GeneratorItem(label, GeneratorJobType.VariablePublic, inMember, found.InClass));
        }

        protected override void ShowGenerateMethod(ScintillaControl sci, ASResult expr, FoundDeclaration found, ICollection<ICompletionListItem> options)
        {
            var label = TextHelper.GetString("ASCompletion.Label.GeneratePrivateFunction");
            options.Add(new GeneratorItem(label, GeneratorJobType.Function, found.Member, found.InClass));
            label = TextHelper.GetString("ASCompletion.Label.GenerateFunctionPublic");
            options.Add(new GeneratorItem(label, GeneratorJobType.FunctionPublic, found.Member, found.InClass));
        }

        public override void GenerateProperty(GeneratorJobType job, ScintillaControl sci, MemberModel member, ClassModel inClass)
        {

        }
    }
}
