// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
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
        /// <inheritdoc />
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

        /// <inheritdoc />
        protected override bool CanShowConvertToConst(ScintillaControl sci, int position, ASResult expr, FoundDeclaration found)
        {
            return !ASContext.Context.CodeComplete.IsStringInterpolationStyle(sci, position) 
                && base.CanShowConvertToConst(sci, position, expr, found);
        }

        /// <inheritdoc />
        protected override bool CanShowImplementInterfaceList(ScintillaControl sci, int position, ASResult expr, FoundDeclaration found)
        {
            return expr.Context.Separator != "=" && base.CanShowImplementInterfaceList(sci, position, expr, found);
        }

        /// <inheritdoc />
        protected override bool CanShowGenerateGetter(ScintillaControl sci, int position, ASResult expr, FoundDeclaration found)
        {
            return contextToken == "get" && found.Member != null && found.Member.Flags.HasFlag(FlagType.Getter | FlagType.Setter);
        }

        /// <inheritdoc />
        protected override bool CanShowGenerateSetter(ScintillaControl sci, int position, ASResult expr, FoundDeclaration found)
        {
            return contextToken == "set" && found.Member != null && found.Member.Flags.HasFlag(FlagType.Getter | FlagType.Setter);
        }

        /// <inheritdoc />
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
            var access = ctx.TypesAffinity(curClass, tmpClass);
            while (tmpClass != null && !tmpClass.IsVoid())
            {
                foreach (MemberModel member in tmpClass.Members)
                {
                    if (curClass.Members.Search(member.Name, FlagType.Override, 0) != null) continue;
                    var parameters = member.Parameters;
                    var parametersCount = parameters?.Count ?? 0;
                    if ((member.Flags & FlagType.Dynamic) != 0
                        && (member.Access & access) != 0
                        && ((member.Flags & FlagType.Function) != 0
                            || ((member.Flags & mask) != 0 && ((parametersCount > 0 && parameters[0].Name == "get")
                                                               || (parametersCount > 1 && parameters[1].Name == "set")
                        ))))
                    {
                        members.Add(member);
                    }
                }

                tmpClass = tmpClass.Extends;
                // members visibility
                access = ctx.TypesAffinity(curClass, tmpClass);
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
            AssignStatementToVar(sci, expr.WordBeforePosition, varName, type.Name);
            return true;
        }

        protected override void AssignStatementToVar(ScintillaControl sci, int position, string name, string type)
        {
            if (((HaXeSettings) ASContext.Context.Settings).DisableTypeDeclaration) type = null;
            base.AssignStatementToVar(sci, position, name, type);
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

        protected override string GetAddInterfaceDefTemplate(MemberModel member)
        {
            if ((member.Flags & (FlagType.Getter | FlagType.Setter)) != 0)
            {
                var template = TemplateUtils.GetTemplate("IGetterSetter");
                var parameters = member.Parameters;
                if (parameters != null)
                {
                    if (parameters.Count > 0) template = template.Replace("get_$(Name)", parameters[0].Name);
                    if (parameters.Count > 1) template = template.Replace("set_$(Name)", parameters[1].Name);
                }
                return template;
            }
            return base.GetAddInterfaceDefTemplate(member);
        }

        protected override string GetGetterImplementationTemplate(MemberModel method)
        {
            var result = TemplateUtils.ToDeclarationWithModifiersString(method, TemplateUtils.GetTemplate("Property"));
            string templateName = null;
            string metadata = null;
            var parameters = method.Parameters;
            var parametersCount = parameters?.Count ?? 0;
            if (parametersCount > 0)
            {
                if (parameters[0].Name == "get")
                {
                    if (parametersCount > 1 && parameters[1].Name == "set")
                    {
                        templateName = "GetterSetter";
                        metadata = "@:isVar";
                    }
                    else templateName = "Getter";
                }
                else if (parametersCount > 1 && parameters[1].Name == "set") templateName = "Setter";
            }
            result = TemplateUtils.ReplaceTemplateVariable(result, "MetaData", metadata);
            if (templateName != null)
            {
                var accessor = NewLine + TemplateUtils.ToDeclarationString(method, TemplateUtils.GetTemplate(templateName));
                accessor = TemplateUtils.ReplaceTemplateVariable(accessor, "Modifiers", null);
                accessor = TemplateUtils.ReplaceTemplateVariable(accessor, "Member", method.Name);
                result += accessor;
            }
            return result;
        }

        protected override void TryGetGetterSetterDelegateTemplate(MemberModel member, MemberModel receiver, ref FlagType flags, ref string variableTemplate, ref string methodTemplate)
        {
            if ((flags & (FlagType.Getter | FlagType.Setter)) != 0)
            {
                variableTemplate = NewLine + NewLine + (TemplateUtils.GetStaticExternOverride(receiver) + TemplateUtils.GetModifiers(receiver)).Trim() + " var " + receiver.Name;
            }
            var parameters = receiver.Parameters;
            var parametersCount = parameters?.Count ?? 0;
            if ((flags & FlagType.Getter) != 0)
            {
                if (parametersCount == 0 || (parameters[0].Name is var name && (name != "null" && name != "never")))
                {
                    variableTemplate += "(get, ";
                    var modifiers = (TemplateUtils.GetStaticExternOverride(receiver) + TemplateUtils.GetModifiers(Visibility.Private)).Trim();
                    methodTemplate += TemplateUtils.GetTemplate("Getter");
                    methodTemplate = TemplateUtils.ReplaceTemplateVariable(methodTemplate, "Modifiers", modifiers);
                    methodTemplate = TemplateUtils.ReplaceTemplateVariable(methodTemplate, "Name", receiver.Name);
                    methodTemplate = TemplateUtils.ReplaceTemplateVariable(methodTemplate, "EntryPoint", "");
                    methodTemplate = TemplateUtils.ReplaceTemplateVariable(methodTemplate, "Type", MemberModel.FormatType(receiver.Type));
                    methodTemplate = TemplateUtils.ReplaceTemplateVariable(methodTemplate, "Member", member.Name + "." + receiver.Name);
                    flags &= ~FlagType.Function;
                }
                else variableTemplate += "(" + parameters[0].Name + ", ";
            }
            if ((flags & FlagType.Setter) != 0)
            {
                if (parametersCount == 1 || (parameters[1].Name is var name && (name != "null" && name != "never")))
                {
                    variableTemplate += "set)";
                    if (methodTemplate != NewLine) methodTemplate += NewLine;
                    var modifiers = (TemplateUtils.GetStaticExternOverride(receiver) + TemplateUtils.GetModifiers(Visibility.Private)).Trim();
                    methodTemplate += TemplateUtils.GetTemplate("Setter");
                    methodTemplate = TemplateUtils.ReplaceTemplateVariable(methodTemplate, "Modifiers", modifiers);
                    methodTemplate = TemplateUtils.ReplaceTemplateVariable(methodTemplate, "Name", receiver.Name);
                    methodTemplate = TemplateUtils.ReplaceTemplateVariable(methodTemplate, "EntryPoint", "");
                    methodTemplate = TemplateUtils.ReplaceTemplateVariable(methodTemplate, "Type", MemberModel.FormatType(receiver.Type));
                    methodTemplate = TemplateUtils.ReplaceTemplateVariable(methodTemplate, "Member", member.Name + "." + receiver.Name);
                    flags &= ~FlagType.Function;
                }
                else variableTemplate += receiver.Parameters[1].Name + ")";
            }
        }

        protected override string TryGetOverrideGetterTemplate(ClassModel ofClass, List<MemberModel> parameters, MemberModel newMember)
        {
            if (parameters == null || parameters.Count == 0 || parameters.First().Name != "get"
                || ASContext.Context.CurrentClass.Members.Search($"get_{newMember.Name}", FlagType.Function, 0) != null) return string.Empty;
            return base.TryGetOverrideGetterTemplate(ofClass, parameters, newMember);
        }

        protected override string TryGetOverrideSetterTemplate(ClassModel ofClass, List<MemberModel> parameters, MemberModel newMember)
        {
            if (parameters == null || parameters.Count == 0 || parameters.Count > 2 || parameters.Last().Name  != "set"
                || ASContext.Context.CurrentClass.Members.Search($"set_{newMember.Name}", FlagType.Function, 0) != null) return string.Empty;
            return base.TryGetOverrideSetterTemplate(ofClass, parameters, newMember);
        }
    }
}
