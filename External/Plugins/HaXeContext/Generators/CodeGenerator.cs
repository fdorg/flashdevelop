using System.Collections.Generic;
using ASCompletion.Completion;
using ASCompletion.Context;
using ASCompletion.Model;
using PluginCore;
using ScintillaNet;

namespace HaXeContext.Generators
{
    internal class CodeGenerator : ASGenerator
    {
        protected override void ContextualGenerator(ScintillaControl sci, int position, ASResult expr, List<ICompletionListItem> options)
        {
            var context = ASContext.Context;
            if (context.CurrentClass.Flags.HasFlag(FlagType.Enum | FlagType.TypeDef) || context.CurrentClass.Flags.HasFlag(FlagType.Interface))
            {
                if (contextToken != null && expr.Member == null && !context.IsImported(expr.Type ?? ClassModel.VoidClass, sci.CurrentLine)) CheckAutoImport(expr, options);
                return;
            }
            base.ContextualGenerator(sci, position, expr, options);
        }

        protected override bool CanShowConvertToConst(ScintillaControl sci, int position, ASResult expr, FoundDeclaration found)
        {
            return !ASContext.Context.CodeComplete.IsStringInterpolationStyle(sci, position) 
                && base.CanShowConvertToConst(sci, position, expr, found);
        }

        protected override bool CanShowImplementInterfaceList(ScintillaControl sci, int position, ASResult expr, FoundDeclaration found)
        {
            return expr.Context.Separator != "=" && base.CanShowImplementInterfaceList(sci, position, expr, found);
        }

        protected override bool CanShowGenerateConstructorAndToString(ScintillaControl sci, int position, ASResult expr, FoundDeclaration found)
        {
            var flags = found.InClass.Flags;
            return !flags.HasFlag(FlagType.Enum)
                && !flags.HasFlag(FlagType.TypeDef)
                && base.CanShowGenerateConstructorAndToString(sci, position, expr, found);
        }

        protected override bool HandleOverrideCompletion(bool autoHide)
        {
            var flags = ASContext.Context.CurrentClass.Flags;
            return !flags.HasFlag(FlagType.Abstract)
                && !flags.HasFlag(FlagType.Interface)
                && !flags.HasFlag(FlagType.TypeDef)
                && base.HandleOverrideCompletion(autoHide);
        }

        protected override bool AssignStatementToVar(ScintillaControl sci, ClassModel inClass, ASExpr expr)
        {
            var ctx = inClass.InFile.Context;
            ClassModel type = null;
            if (expr.WordBefore == "cast" || expr.WordBefore == "untyped") type = ctx.ResolveType(ctx.Features.dynamicKey, inClass.InFile);
            if (type == null) return false;
            var varName = GuessVarName(type.Name, type.Type);
            varName = AvoidKeyword(varName);
            var template = TemplateUtils.GetTemplate("AssignVariable");
            template = TemplateUtils.ReplaceTemplateVariable(template, "Name", varName);
            template = TemplateUtils.ReplaceTemplateVariable(template, "Type", type.Name);
            var pos = expr.WordBeforePosition;
            sci.SetSel(pos, pos);
            InsertCode(pos, template, sci);
            return true;
        }
    }
}
