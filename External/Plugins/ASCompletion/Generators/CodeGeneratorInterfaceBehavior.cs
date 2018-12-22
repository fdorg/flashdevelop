using System.Collections.Generic;
using ASCompletion.Completion;
using ASCompletion.Context;
using ASCompletion.Model;
using PluginCore;
using PluginCore.Localization;
using ScintillaNet;

namespace ASCompletion.Generators
{
    public class CodeGeneratorInterfaceBehavior : ICodeGeneratorBehavior
    {
        public void ContextualGenerator(ScintillaControl sci, int position, ASResult expr, List<ICompletionListItem> options)
        {
            var line = sci.LineFromPosition(position);
            var found = ((ASGenerator) ASContext.Context.CodeGenerator).GetDeclarationAtLine(line);
            if (CanShowNewInterfaceList(sci, position, expr, found)) ShowNewInterfaceList(sci, expr, found, options);
            if (CanShowNewMemberList(sci, position, expr, found))
            {
                ShowNewVariableList(sci, expr, found, options);
                ShowNewPropertyList(sci, expr, found, options);
                ShowNewMethodList(sci, expr, found, options);
            }
        }

        protected virtual bool CanShowNewInterfaceList(ScintillaControl sci, int position, ASResult expr, FoundDeclaration found)
        {
            return ASGenerator.contextToken != null
                   && ASComplete.IsTextStyle(sci.BaseStyleAt(position - 1))
                   && ASContext.Context.CodeComplete.PositionIsBeforeBody(sci, position, found.InClass)
                   && (expr.Context.WordBefore == "extends" || expr.Context.Separator == ":");
        }

        static void ShowNewInterfaceList(ScintillaControl sci, ASResult expr, FoundDeclaration found, ICollection<ICompletionListItem> options)
        {
            var label = TextHelper.GetString("ASCompletion.Label.GenerateInterface");
            options.Add(new GeneratorItem(label, GeneratorJobType.Interface, found.Member, found.InClass, expr));
        }

        static bool CanShowNewMemberList(ScintillaControl sci, int position, ASResult expr, FoundDeclaration found)
        {
            return ASGenerator.contextToken != null
                   && ASComplete.IsTextStyle(sci.BaseStyleAt(position - 1))
                   && !ASContext.Context.CodeComplete.PositionIsBeforeBody(sci, position, found.InClass)
                   && expr.IsNull() && expr.Context.ContextFunction == null && expr.Context.ContextMember == null;
        }

        protected virtual void ShowNewVariableList(ScintillaControl sci, ASResult expr, FoundDeclaration found, ICollection<ICompletionListItem> options)
        {
        }

        static void ShowNewPropertyList(ScintillaControl sci, ASResult expr, FoundDeclaration found, ICollection<ICompletionListItem> options)
        {
            var member = new MemberModel {Name = expr.Context.Value};
            var label = TextHelper.GetString("ASCompletion.Label.GenerateGetSet");
            options.Add(new GeneratorItem(label, GeneratorJobType.GetterSetter, member, found.InClass));
            label = TextHelper.GetString("ASCompletion.Label.GenerateGet");
            options.Add(new GeneratorItem(label, GeneratorJobType.Getter, member, found.InClass));
            label = TextHelper.GetString("ASCompletion.Label.GenerateSet");
            options.Add(new GeneratorItem(label, GeneratorJobType.Setter, member, found.InClass));
        }

        static void ShowNewMethodList(ScintillaControl sci, ASResult expr, FoundDeclaration found, ICollection<ICompletionListItem> options)
        {
            var label = TextHelper.GetString("ASCompletion.Label.GenerateFunctionInterface");
            options.Add(new GeneratorItem(label, GeneratorJobType.FunctionPublic, found.Member, found.InClass));
        }

        public void GenerateProperty(GeneratorJobType job, ScintillaControl sci, MemberModel member, ClassModel inClass)
        {
            sci.BeginUndoAction();
            try
            {
                if (job == GeneratorJobType.GetterSetter) GenerateGetterSetter(sci, member, TemplateUtils.GetTemplate("IGetterSetter"));
                else if (job == GeneratorJobType.Setter) GenerateProperty(sci, member, TemplateUtils.GetTemplate("ISetter"));
                else if (job == GeneratorJobType.Getter) GenerateProperty(sci, member, TemplateUtils.GetTemplate("IGetter"));
            }
            finally
            {
                sci.EndUndoAction();
            }
        }

        static void GenerateGetterSetter(ScintillaControl sci, MemberModel member, string template)
        {
            if (!string.IsNullOrEmpty(template))
            {
                GenerateProperty(sci, member, template);
                return;
            }
            GenerateProperty(sci, member, TemplateUtils.GetTemplate("IGetter"));
            var pos = sci.LineEndPosition(sci.CurrentLine);
            sci.SetSel(pos, pos);
            sci.NewLine();
            GenerateProperty(sci, member, TemplateUtils.GetTemplate("ISetter"));
        }

        protected static void GenerateProperty(ScintillaControl sci, MemberModel member, string template)
        {
            var features = ASContext.Context.Features;
            template = TemplateUtils.ReplaceTemplateVariable(template, "EntryPoint", string.Empty);
            template = TemplateUtils.ReplaceTemplateVariable(template, "Type", $"$(EntryPoint){features.dynamicKey}$(ExitPoint)");
            template = TemplateUtils.ToDeclarationString(member, template);
            template = TemplateUtils.ReplaceTemplateVariable(template, "BlankLine", string.Empty);
            template = TemplateUtils.ReplaceTemplateVariable(template, "Void", features.voidKey);
            sci.SelectWord();
            ASGenerator.InsertCode(sci.SelectionStart, template, sci);
        }
    }
}