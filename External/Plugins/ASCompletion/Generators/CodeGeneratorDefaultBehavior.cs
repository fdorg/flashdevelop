using System.Collections.Generic;
using ASCompletion.Completion;
using ASCompletion.Context;
using ASCompletion.Model;
using PluginCore;
using PluginCore.Localization;
using ScintillaNet;

namespace ASCompletion.Generators
{
    public class CodeGeneratorDefaultBehavior : ICodeGeneratorBehavior
    {
        public virtual bool ContextualGenerator(ScintillaControl sci, int position, ASResult expr, ICollection<ICompletionListItem> options)
        {
            var result = false;
            var ctx = ASContext.Context;
            var line = sci.LineFromPosition(position);
            var found = ((ASGenerator) ctx.CodeGenerator).GetDeclarationAtLine(line);
            if (CanShowGenerateExtends(sci, position, expr, found))
            {
                ShowGenerateExtends(sci, expr, found, options);
                result = true;
            }
            // TODO: CanShowGenerateImplements
            // TODO: CanShowGenerateType
            if (CanShowGenerateMember(sci, position, expr, found))
            {
                ShowGenerateField(sci, expr, found, options);
                ShowGenerateProperty(sci, expr, found, options);
                ShowGenerateMethod(sci, expr, found, options);
                result = true;
            }
            return result;
        }

        protected virtual bool CanShowGenerateExtends(ScintillaControl sci, int position, ASResult expr, FoundDeclaration found) => false;

        protected virtual void ShowGenerateExtends(ScintillaControl sci, ASResult expr, FoundDeclaration found, ICollection<ICompletionListItem> options)
        {
        }

        static bool CanShowGenerateMember(ScintillaControl sci, int position, ASResult expr, FoundDeclaration found)
        {
            return ASGenerator.contextToken != null
                   && ASComplete.IsTextStyle(sci.BaseStyleAt(position - 1))
                   && !ASContext.Context.CodeComplete.PositionIsBeforeBody(sci, position, found.InClass)
                   && expr.IsNull() && expr.Context.ContextFunction == null && expr.Context.ContextMember == null;
        }

        protected virtual void ShowGenerateField(ScintillaControl sci, ASResult expr, FoundDeclaration found, ICollection<ICompletionListItem> options)
        {
        }

        static void ShowGenerateProperty(ScintillaControl sci, ASResult expr, FoundDeclaration found, ICollection<ICompletionListItem> options)
        {
            var member = new MemberModel {Name = expr.Context.Value};
            var label = TextHelper.GetString("ASCompletion.Label.GenerateGetSet");
            options.Add(new GeneratorItem(label, GeneratorJobType.GetterSetter, member, found.InClass));
            label = TextHelper.GetString("ASCompletion.Label.GenerateGet");
            options.Add(new GeneratorItem(label, GeneratorJobType.Getter, member, found.InClass));
            label = TextHelper.GetString("ASCompletion.Label.GenerateSet");
            options.Add(new GeneratorItem(label, GeneratorJobType.Setter, member, found.InClass));
        }

        protected virtual void ShowGenerateMethod(ScintillaControl sci, ASResult expr, FoundDeclaration found, ICollection<ICompletionListItem> options)
        {
        }

        public virtual bool GenerateProperty(GeneratorJobType job, ScintillaControl sci, MemberModel member, ClassModel inClass) => false;
    }
}
