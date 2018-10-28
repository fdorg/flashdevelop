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
            var ctx = ASContext.Context;
            var line = sci.LineFromPosition(position);
            var found = ((ASGenerator) ctx.CodeGenerator).GetDeclarationAtLine(line);
            if (CanShowGenerateNewInterface(sci, position, expr, found)) ShowGenerateNewInterface(sci, expr, found, options);
            if (CanShowGenerateNewMethod(sci, position, expr, found))
            {
                ShowCustomGenerators(sci, expr, found, options);
                ShowGenerateGetterSetter(sci, expr, found, options);
                ShowGenerateNewMethod(sci, expr, found, options);
            }
        }

        protected virtual bool CanShowGenerateNewInterface(ScintillaControl sci, int position, ASResult expr, FoundDeclaration found)
        {
            return ASGenerator.contextToken != null
                   && ASComplete.IsTextStyle(sci.BaseStyleAt(position - 1))
                   && ASContext.Context.CodeComplete.PositionIsBeforeBody(sci, position, found.InClass)
                   && (expr.Context.WordBefore == "extends" || expr.Context.Separator == ":");
        }

        static bool CanShowGenerateNewMethod(ScintillaControl sci, int position, ASResult expr, FoundDeclaration found)
        {
            return ASGenerator.contextToken != null
                   && ASComplete.IsTextStyle(sci.BaseStyleAt(position - 1))
                   && !ASContext.Context.CodeComplete.PositionIsBeforeBody(sci, position, found.InClass)
                   && expr.IsNull() && expr.Context.ContextFunction == null && expr.Context.ContextMember == null;
        }

        protected virtual void ShowCustomGenerators(ScintillaControl sci, ASResult expr, FoundDeclaration found, ICollection<ICompletionListItem> options)
        {
        }

        static void ShowGenerateNewInterface(ScintillaControl sci, ASResult expr, FoundDeclaration found, ICollection<ICompletionListItem> options)
        {
            var label = TextHelper.GetString("ASCompletion.Label.GenerateInterface");
            options.Add(new GeneratorItem(label, GeneratorJobType.Interface, found.Member, found.InClass, expr));
        }

        static void ShowGenerateGetterSetter(ScintillaControl sci, ASResult expr, FoundDeclaration found, ICollection<ICompletionListItem> options)
        {
            var member = new MemberModel {Name = expr.Context.Value};
            var label = TextHelper.GetString("ASCompletion.Label.GenerateGetSet");
            options.Add(new GeneratorItem(label, GeneratorJobType.GetterSetter, member, found.InClass));
            label = TextHelper.GetString("ASCompletion.Label.GenerateGet");
            options.Add(new GeneratorItem(label, GeneratorJobType.Getter, member, found.InClass));
            label = TextHelper.GetString("ASCompletion.Label.GenerateSet");
            options.Add(new GeneratorItem(label, GeneratorJobType.Setter, member, found.InClass));
        }

        static void ShowGenerateNewMethod(ScintillaControl sci, ASResult expr, FoundDeclaration found, ICollection<ICompletionListItem> options)
        {
            var label = TextHelper.GetString("ASCompletion.Label.GenerateFunctionInterface");
            options.Add(new GeneratorItem(label, GeneratorJobType.FunctionPublic, found.Member, found.InClass));
        }

        public void GenerateProperty(GeneratorJobType job, ScintillaControl sci, MemberModel member, ClassModel inClass)
        {
            
            if (job == GeneratorJobType.GetterSetter) GenerateGetterSetter(sci, member, TemplateUtils.GetTemplate("IGetterSetter"));
            else if (job == GeneratorJobType.Setter) GenerateAccessor(sci, member, TemplateUtils.GetTemplate("ISetter"));
            else if (job == GeneratorJobType.Getter) GenerateAccessor(sci, member, TemplateUtils.GetTemplate("IGetter"));
        }

        static void GenerateGetterSetter(ScintillaControl sci, MemberModel member, string template)
        {
            if (!string.IsNullOrEmpty(template))
            {
                GenerateAccessor(sci, member, template);
                return;
            }
            GenerateAccessor(sci, member, TemplateUtils.GetTemplate("IGetter"));
            var pos = sci.LineEndPosition(sci.CurrentLine);
            sci.SetSel(pos, pos);
            sci.NewLine();
            GenerateAccessor(sci, member, TemplateUtils.GetTemplate("ISetter"));
        }

        protected static void GenerateAccessor(ScintillaControl sci, MemberModel member, string template)
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