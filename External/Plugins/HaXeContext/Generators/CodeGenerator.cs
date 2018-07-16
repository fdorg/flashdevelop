using System.Collections.Generic;
using System.Linq;
using ASCompletion.Completion;
using ASCompletion.Context;
using ASCompletion.Model;
using PluginCore;
using PluginCore.Controls;
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
            var ctx = ASContext.Context;
            var curClass = ctx.CurrentClass;
            if (curClass.IsVoid()) return false;
            var flags = curClass.Flags;
            if (flags.HasFlag(FlagType.Abstract) || flags.HasFlag(FlagType.Interface) || flags.HasFlag(FlagType.TypeDef)) return false;

            var members = new List<MemberModel>();
            curClass.ResolveExtends();

            // explore getters or setters
            const FlagType mask = FlagType.Function | FlagType.Getter | FlagType.Setter;
            var tmpClass = curClass.Extends;
            var acc = ctx.TypesAffinity(curClass, tmpClass);
            while (tmpClass != null && !tmpClass.IsVoid())
            {
                foreach (MemberModel member in tmpClass.Members)
                {
                    if (curClass.Members.Search(member.Name, FlagType.Override, 0) != null) continue;
                    var parameters = member.Parameters;
                    if ((member.Flags & FlagType.Dynamic) > 0
                        && (member.Access & acc) > 0
                        && ((member.Flags & FlagType.Function) > 0
                            || ((member.Flags & mask) > 0 && parameters != null && (parameters[0].Name == "get" || parameters[1].Name == "set"))))
                    {
                        members.Add(member);
                    }
                }

                tmpClass = tmpClass.Extends;
                // members visibility
                acc = ctx.TypesAffinity(curClass, tmpClass);
            }
            members.Sort();

            var list = new List<ICompletionListItem>();
            MemberModel last = null;
            foreach (var member in members)
            {
                if (last == null || last.Name != member.Name)
                    list.Add(new MemberItem(member));
                last = member;
            }
            if (list.Count > 0) CompletionList.Show(list, autoHide);
            return true;
        }

        protected override bool AssignStatementToVar(ScintillaControl sci, ClassModel inClass, ASExpr expr)
        {
            var ctx = inClass.InFile.Context;
            ClassModel type = null;
            // for example: cast value|, cast(value, Type)|
            if (expr.WordBefore == "cast")
            {
                // for example: cast(value, Type)|
                if (expr.SubExpressions != null && expr.SubExpressions.Count > 0 && expr.Value[0] == '#')
                    type = ctx.ResolveToken("cast" + expr.SubExpressions.Last(), inClass.InFile);
                else type = ctx.ResolveType(ctx.Features.dynamicKey, inClass.InFile);
            }
            // for example: untyped value|
            else if (expr.WordBefore == "untyped") type = ctx.ResolveType(ctx.Features.dynamicKey, inClass.InFile);
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

        protected override void GenerateEventHandler(ScintillaControl sci, int position, string template, string currentTarget, string eventName, string handlerName)
        {
            if (currentTarget != null)
            {
                var delta = 0;
                if (TryImportType("flash.events.IEventDispatcher", ref delta, sci.LineFromPosition(position)))
                {
                    position += delta;
                    sci.SetSel(position, position);
                    lookupPosition += delta;
                    currentTarget = "cast(e.currentTarget, IEventDispatcher)";
                }
                var ctx = ASContext.Context;
                if (currentTarget.Length == 0 && ASContext.CommonSettings.GenerateScope && ctx.Features.ThisKey != null)
                    currentTarget = ctx.Features.ThisKey;
                if (currentTarget.Length > 0) currentTarget += ".";
                var remove = $"{currentTarget}removeEventListener({eventName}, {handlerName});\n\t$(EntryPoint)";
                template = template.Replace("$(EntryPoint)", remove);
            }
            InsertCode(position, template, sci);
        }

        protected override string GetFunctionType(ASResult expr)
        {
            var voidKey = ASContext.Context.Features.voidKey;
            var parameters = expr.Member.Parameters?.Select(it => it.Type).ToList() ?? new List<string> {voidKey};
            parameters.Add(expr.Member.Type ?? voidKey);
            var qualifiedName = string.Empty;
            for (var i = 0; i < parameters.Count; i++)
            {
                if (i > 0) qualifiedName += "->";
                var t = parameters[i];
                if (t.Contains("->") && !t.StartsWith('(')) t = $"({t})";
                qualifiedName += t;
            }
            return qualifiedName;
        }
    }
}
