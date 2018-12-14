using System.Collections.Generic;
using ASCompletion.Completion;
using ASCompletion.Context;
using ASCompletion.Model;
using PluginCore;
using PluginCore.Localization;
using ScintillaNet;

namespace ASCompletion.Generators
{
    public class CodeGeneratorInterfaceBehavior : CodeGeneratorDefaultBehavior
    {

        protected override bool CanShowGenerateExtends(ScintillaControl sci, int position, ASResult expr, FoundDeclaration found)
        {
            return ASGenerator.contextToken != null
                   && ASComplete.IsTextStyle(sci.BaseStyleAt(position - 1))
                   && ASContext.Context.CodeComplete.PositionIsBeforeBody(sci, position, found.InClass)
                   && (expr.Context.WordBefore == "extends" || expr.Context.Separator == ":");
        }

        protected override void ShowGenerateExtends(ScintillaControl sci, ASResult expr, FoundDeclaration found, ICollection<ICompletionListItem> options)
        {
            var label = TextHelper.GetString("ASCompletion.Label.GenerateInterface");
            options.Add(new GeneratorItem(label, GeneratorJobType.Interface, found.Member, found.InClass, expr));
        }

        protected override void ShowGenerateMethod(ScintillaControl sci, ASResult expr, FoundDeclaration found, ICollection<ICompletionListItem> options)
        {
            var label = TextHelper.GetString("ASCompletion.Label.GenerateFunctionInterface");
            options.Add(new GeneratorItem(label, GeneratorJobType.FunctionPublic, found.Member, found.InClass));
        }

        public virtual void GenerateProperty(GeneratorJobType job, ScintillaControl sci, MemberModel member, ClassModel inClass)
        {
            
            if (job == GeneratorJobType.GetterSetter) GenerateGetterSetter(sci, member, TemplateUtils.GetTemplate("IGetterSetter"));
            else if (job == GeneratorJobType.Setter) GenerateProperty(sci, member, TemplateUtils.GetTemplate("ISetter"));
            else if (job == GeneratorJobType.Getter) GenerateProperty(sci, member, TemplateUtils.GetTemplate("IGetter"));
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