using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using ASCompletion.Completion;
using ASCompletion.Context;
using ASCompletion.Model;
using ASCompletion.Settings;
using PluginCore;
using PluginCore.Controls;
using PluginCore.Helpers;
using PluginCore.Localization;
using ScintillaNet;

namespace HaXeContext.Generators
{
    public enum GeneratorJob
    {
        Switch
    }

    internal class CodeGenerator : ASGenerator
    {
        /// <inheritdoc />
        protected override void ContextualGenerator(ScintillaControl sci, int position, ASResult expr, List<ICompletionListItem> options)
        {
            var ctx = ASContext.Context;
            var currentClass = ctx.CurrentClass;
            if (currentClass.Flags.HasFlag(FlagType.Enum | FlagType.TypeDef) || currentClass.Flags.HasFlag(FlagType.Interface))
            {
                if (contextToken != null && expr.Member == null && !ctx.IsImported(expr.Type ?? ClassModel.VoidClass, sci.CurrentLine)) CheckAutoImport(expr, options);
                return;
            }
            var member = expr.Member;
            if (member != null && !member.Flags.HasFlag(FlagType.Enum)
                && expr.Context.WordBefore is var word && word != ctx.Features.varKey && word != ctx.Features.functionKey)
            {
                var isAvailable = true;
                var contextMember = expr.Context.ContextMember;
                var end = contextMember != null ? sci.PositionFromLine(contextMember.LineTo) : sci.TextLength;
                for (var i = ASComplete.ExpressionEndPosition(sci, sci.CurrentPos); i < end; i++)
                {
                    if (sci.PositionIsOnComment(i)) continue;
                    var c = (char) sci.CharAt(i);
                    if (c <= ' ') continue;
                    if (c == '.')
                    {
                        isAvailable = false;
                        break;
                    }
                    if (c > ' ') break;
                }
                if (isAvailable)
                {
                    var type = ctx.ResolveType(member.Type, expr.InFile);
                    if (type.Flags.HasFlag(FlagType.Enum) && type.Members.Count > 0)
                    {
                        var label = TextHelper.GetString("Info.GenerateSwitch");
                        options.Add(new GeneratorItem(label, GeneratorJob.Switch, () => Generate(GeneratorJob.Switch, sci, expr)));
                    }
                }
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
            while (!tmpClass.IsVoid())
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
            ClassModel type;
            switch (expr.WordBefore)
            {
                // for example: cast(value, Type)|
                case "cast" when expr.SubExpressions != null && expr.SubExpressions.Count > 0 && expr.Value[0] == '#':
                    type = ctx.ResolveToken("cast" + expr.SubExpressions.Last(), inClass.InFile);
                    break;
                case "cast": // for example: cast value|
                case "untyped": // for example: untyped value|
                    type = ctx.ResolveType(ctx.Features.dynamicKey, inClass.InFile);
                    break;
                default:
                    return false;
            }
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

        protected override void GenerateProperty(GeneratorJobType job, MemberModel member, ClassModel inClass, ScintillaControl sci)
        {
            var location = ASContext.CommonSettings.PropertiesGenerationLocation;
            var latest = TemplateUtils.GetTemplateBlockMember(sci, TemplateUtils.GetBoundary("AccessorsMethods"));
            if (latest != null) location = PropertiesGenerationLocations.AfterLastPropertyDeclaration;
            else
            {
                if (location == PropertiesGenerationLocations.AfterLastPropertyDeclaration)
                {
                    if (job == GeneratorJobType.Setter) latest = FindMember("get_" + member.Name, inClass);
                    else if (job == GeneratorJobType.Getter) latest = FindMember("set_" + member.Name, inClass);
                    if (latest == null) latest = FindLatest(FlagType.Function, 0, inClass, false, false);
                }
                else latest = member;
            }
            if (latest == null) return;
            sci.BeginUndoAction();
            try
            {
                var name = member.Name;
                var args = "(default, default)";
                if (job == GeneratorJobType.GetterSetter) args = "(get, set)";
                else if (job == GeneratorJobType.Getter) args = "(get, null)";
                else if (job == GeneratorJobType.Setter) args = "(default, set)";
                MakeProperty(sci, member, args);
                var startsWithNewLine = true;
                var endsWithNewLine = false;
                int atLine;
                if (location == PropertiesGenerationLocations.BeforeVariableDeclaration) atLine = latest.LineTo;
                else if (job == GeneratorJobType.Getter && (latest.Flags & (FlagType.Dynamic | FlagType.Function)) != 0)
                {
                    atLine = latest.LineFrom;
                    var declaration = GetDeclarationAtLine(atLine - 1);
                    startsWithNewLine = declaration.Member != null;
                    endsWithNewLine = true;
                }
                else atLine = latest.LineTo + 1;
                var position = sci.PositionFromLine(atLine) - ((sci.EOLMode == 0) ? 2 : 1);
                sci.SetSel(position, position);
                if (job == GeneratorJobType.GetterSetter) GenerateGetterSetter(name, member, position);
                else if (job == GeneratorJobType.Setter) GenerateSetter(name, member, position);
                else if (job == GeneratorJobType.Getter) GenerateGetter(name, member, position, startsWithNewLine, endsWithNewLine);
            }
            finally
            {
                sci.EndUndoAction();
            }
        }

        static void MakeProperty(ScintillaControl sci, MemberModel member, string args)
        {
            var features = ASContext.Context.Features;
            var kind = features.varKey;
            if ((member.Flags & FlagType.Getter) > 0) kind = features.getKey;
            else if ((member.Flags & FlagType.Setter) > 0) kind = features.setKey;
            else if (member.Flags == FlagType.Function) kind = features.functionKey;
            else kind = $@"(?:(?<access>public |private |static |inline )\s)*?{kind}";
            var reMember = new Regex($@"{kind}\s+({member.Name})[\s:]", RegexOptions.IgnorePatternWhitespace);
            for (var i = member.LineFrom; i <= member.LineTo; i++)
            {
                var line = sci.GetLine(i);
                var m = reMember.Match(line);
                if (!m.Success) continue;
                var offset = 0;
                var positionFromLine = sci.PositionFromLine(i);
                if (args == "(get, set)")
                {
                    sci.SetSel(positionFromLine + m.Index, sci.LineEndPosition(i));
                    sci.ReplaceSel($"@:isVar {sci.SelText}");
                    offset = "$:isVar ".Length;
                    line = sci.GetLine(i);
                }
                var index = sci.MBSafeTextLength(line.Substring(0, m.Groups[1].Index + offset));
                var position = positionFromLine + index;
                sci.SetSel(position, position + member.Name.Length);
                sci.ReplaceSel(member.Name + args);
                UpdateLookupPosition(position, 1);
                return;
            }
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

        protected override string GetFieldTypeFromParameter(string paramType, ref string paramName)
        {
            if (paramName.StartsWith('?'))
            {
                paramName = paramName.Remove(0, 1);
                if (string.IsNullOrEmpty(paramType)) return "Null<Dynamic>";
                if (!paramType.StartsWith("Null<")) return $"Null<{paramType}>";
            }
            return paramType;
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

        static void Generate(GeneratorJob job, ScintillaControl sci, ASResult expr)
        {
            switch (job)
            {
                case GeneratorJob.Switch:
                    sci.BeginUndoAction();
                    try
                    {
                        GenerateSwitch(sci, expr, expr.InFile.Context.ResolveType(expr.Member.Type, expr.InFile));
                    }
                    finally { sci.EndUndoAction(); }
                    break;
            }
        }

        static void GenerateSwitch(ScintillaControl sci, ASResult expr, ClassModel inClass)
        {
            var start = expr.Context.PositionExpression;
            int end;
            if (expr.Type.QualifiedName == ASContext.Context.ResolveType("Function", null).QualifiedName)
                end = GetEndOfStatement(start, sci.Length, sci) - 1;
            else end = expr.Context.Position;
            sci.SetSel(start, end);
            var template = TemplateUtils.GetTemplate("Switch");
            template = TemplateUtils.ReplaceTemplateVariable(template, "Name", sci.SelText);
            template = template.Replace(SnippetHelper.ENTRYPOINT, string.Empty);
            var body = string.Empty;
            for (var i = 0; i < inClass.Members.Count; i++)
            {
                var it = inClass.Members[i];
                body += SnippetHelper.BOUNDARY;
                if (i > 0) body += "\n";
                body += "\tcase " + it.Name;
                if (it.Parameters != null)
                {
                    body += "(";
                    for (var j = 0; j < it.Parameters.Count; j++)
                    {
                        if (j > 0) body += ", ";
                        body += it.Parameters[j].Name.TrimStart('?');
                    }
                    body += ")";
                }
                body += ":";
                if (i == 0) body += ' ' + SnippetHelper.ENTRYPOINT;
            }
            template = TemplateUtils.ReplaceTemplateVariable(template, "Body", body);
            InsertCode(start, template, sci);
        }
    }

    class GeneratorItem : ICompletionListItem
    {
        internal GeneratorJob Job { get; }
        readonly Action action;

        public GeneratorItem(string label, GeneratorJob job, Action action)
        {
            Label = label;
            Job = job;
            this.action = action;
        }

        public string Label { get; }

        public string Value
        {
            get
            {
                action.Invoke();
                return null;
            }
        }

        public string Description => TextHelper.GetString("ASCompletion.Info.GeneratorTemplate");

        public Bitmap Icon => (Bitmap) ASContext.Panel.GetIcon(34);
    }
}
