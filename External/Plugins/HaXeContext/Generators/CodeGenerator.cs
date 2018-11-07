using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ASCompletion.Completion;
using ASCompletion.Context;
using ASCompletion.Generators;
using ASCompletion.Model;
using ASCompletion.Settings;
using HaXeContext.Completion;
using HaXeContext.Model;
using PluginCore;
using PluginCore.Controls;
using PluginCore.Helpers;
using PluginCore.Localization;
using ScintillaNet;

namespace HaXeContext.Generators
{
    public enum GeneratorJob
    {
        EnumConstructor,
        Switch,
        IVariable,
    }

    internal class CodeGenerator : ASGenerator
    {
        readonly CodeGeneratorInterfaceBehavior codeGeneratorInterfaceBehavior = new CodeGeneratorInterfaceBehavior();

        protected override ICodeGeneratorBehavior GetCodeGeneratorBehavior()
        {
            if ((ASContext.Context.CurrentClass.Flags & FlagType.Interface) != 0)
                return codeGeneratorInterfaceBehavior;
            return base.GetCodeGeneratorBehavior();
        }

        /// <inheritdoc />
        protected override void ContextualGenerator(ScintillaControl sci, int position, ASResult expr, List<ICompletionListItem> options)
        {
            // for example: @:meta|Tag
            if (expr.Context.Separator == ":" && expr.Context.SeparatorPosition > 0 && sci.CharAt(expr.Context.SeparatorPosition - 1) == '@') return;
            var ctx = ASContext.Context;
            var currentClass = ctx.CurrentClass;
            if (currentClass.Flags.HasFlag(FlagType.Enum | FlagType.TypeDef))
            {
                if (contextToken != null && expr.Member == null && !ctx.IsImported(expr.Type ?? ClassModel.VoidClass, sci.CurrentLine)) CheckAutoImport(expr, options);
                return;
            }
            if (CanShowGenerateSwitch(sci, position, expr))
            {
                var label = TextHelper.GetString("Info.GenerateSwitch");
                options.Add(new GeneratorItem(label, GeneratorJob.Switch, () => Generate(GeneratorJob.Switch, sci, expr)));
            }
            base.ContextualGenerator(sci, position, expr, options);
        }

        /// <inheritdoc />
        protected override bool CanShowAssignStatementToVariable(ScintillaControl sci, ASResult expr)
        {
            if (!base.CanShowAssignStatementToVariable(sci, expr)) return false;
            // for example: return cast expr<generator>, return untyped expr<generator>
            return ((expr.Context.WordBefore != "cast" || expr.Context.WordBefore != "untyped")
                    && sci.GetWordLeft(expr.Context.WordBeforePosition - 1, true) != "return");
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
        protected override bool CanShowNewMethodList(ScintillaControl sci, int position, ASResult expr, FoundDeclaration found)
        {
            var inClass = expr.RelClass ?? found.InClass;
            if (inClass.Flags.HasFlag(FlagType.Enum) && !expr.IsStatic) return false;
            if (inClass.Flags.HasFlag(FlagType.TypeDef) && expr.IsStatic) return false;
            return base.CanShowNewMethodList(sci, position, expr, found);
        }

        /// <inheritdoc />
        protected override bool CanShowNewVarList(ScintillaControl sci, int position, ASResult expr, FoundDeclaration found)
        {
            var inClass = expr.RelClass ?? found.InClass;
            if (inClass.Flags.HasFlag(FlagType.Enum) && !expr.IsStatic) return false;
            if (inClass.Flags.HasFlag(FlagType.TypeDef) && expr.IsStatic) return false;
            return !found.InClass.IsVoid()
                && !ASContext.Context.CodeComplete.PositionIsBeforeBody(sci, position, found.InClass)
                && base.CanShowNewMethodList(sci, position, expr, found);
        }

        /// <inheritdoc />
        protected override bool CanShowGetSetList(ScintillaControl sci, int position, ASResult expr, FoundDeclaration found)
        {
            var inClass = expr.RelClass ?? found.InClass;
            if (inClass.Flags.HasFlag(FlagType.Abstract)) return false;
            return base.CanShowGetSetList(sci, position, expr, found);
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
                    if (curClass.Members.Contains(member.Name, FlagType.Override, 0)) continue;
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

        protected override void GenerateFunction(ScintillaControl sci, MemberModel member, int position, ClassModel inClass, bool detach)
        {
            if (inClass.Flags.HasFlag(FlagType.TypeDef) || inClass.Flags.HasFlag(FlagType.Interface))
            {
                var template = TemplateUtils.GetTemplate("IFunction");
                if (string.IsNullOrEmpty(member.Type)) template = TemplateUtils.ReplaceTemplateVariable(template, "Type", ASContext.Context.Features.voidKey);
                var declaration = TemplateUtils.ToDeclarationString(member, template);
                GenerateFunction(position, declaration, detach);
            }
            else base.GenerateFunction(sci, member, position, inClass, detach);
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

        public override FoundDeclaration GetDeclarationAtLine(int line)
        {
            var result = base.GetDeclarationAtLine(line);
            if (result.Member is MemberModel member
                && string.IsNullOrEmpty(member.Type)
                && (member.Flags.HasFlag(FlagType.Variable)
                    || member.Flags.HasFlag(FlagType.Getter)
                    || member.Flags.HasFlag(FlagType.Setter)))
            {
                ((CodeComplete)ASContext.Context.CodeComplete).InferVariableType(ASContext.CurSciControl, member);
            }
            return result;
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

        protected override string CheckEventType(MemberModel handler, string eventName)
        {
            if (handler?.Parameters is List<MemberModel> parameters && parameters.Count > 1 && parameters[1]?.Type is string type)
            {
                if (type == "haxe.Constraints.Function") return string.Empty;
                if (FileParser.IsFunctionType(type))
                {
                    var member = FileParser.FunctionTypeToMemberModel(type, ASContext.Context.Features);
                    if (member.Parameters.Count > 0 && member.Parameters[0].Type is string result)
                    {
                        if (result.Equals(ASContext.Context.Features.voidKey)) return string.Empty;
                        return result;
                    }
                }
            }
            return base.CheckEventType(handler, eventName);
        }

        protected override string GetAddInterfaceDefTemplate(MemberModel member)
        {
            if ((member.Flags & (FlagType.Getter | FlagType.Setter)) != 0)
            {
                var template = TemplateUtils.GetTemplate("IGetterSetter");
                var parameters = member.Parameters;
                if (parameters != null)
                {
                    if (parameters.Count > 0) template = template.Replace("get", parameters[0].Name);
                    if (parameters.Count > 1) template = template.Replace("set", parameters[1].Name);
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

        protected override string GetFunctionBody(MemberModel member, ClassModel inClass)
        {
            switch (ASContext.CommonSettings.GeneratedMemberDefaultBodyStyle)
            {
                case GeneratedMemberBodyStyle.ReturnDefaultValue:
                    var type = member.Type;
                    var expr = ASContext.Context.ResolveType(type, inClass.InFile);
                    if ((expr.Flags & FlagType.Abstract) != 0 && !string.IsNullOrEmpty(expr.ExtendsType))
                        type = expr.ExtendsType;
                    var defaultValue = ASContext.Context.GetDefaultValue(type);
                    if (!string.IsNullOrEmpty(defaultValue)) return $"return {defaultValue};";
                    break;
            }
            return null;
        }

        protected override string FunctionToString(MemberModel member)
        {
            var voidKey = ASContext.Context.Features.voidKey;
            var dynamicTypeName = ASContext.Context.ResolveType(ASContext.Context.Features.dynamicKey, null).Name;
            var parameters = member.Parameters?.Select(it => it.Type).ToList() ?? new List<string> {voidKey};
            parameters.Add(member.Type ?? voidKey);
            var sb = new StringBuilder();
            for (var i = 0; i < parameters.Count; i++)
            {
                if (i > 0) sb.Append("->");
                var t = parameters[i];
                if (t == null) sb.Append(dynamicTypeName);
                else if (FileParser.IsFunctionType(t))
                {
                    sb.Append('(');
                    sb.Append(t);
                    sb.Append(')');
                }
                else sb.Append(t);
            }
            return sb.ToString();
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

        protected override void ShowNewMethodList(ScintillaControl sci, ASResult expr, FoundDeclaration found, ICollection<ICompletionListItem> options)
        {
            var inClass = expr.RelClass;
            if (inClass != null && inClass.Flags.HasFlag(FlagType.Enum) && expr.IsStatic)
            {
                var label = TextHelper.GetString("ASCompletion.Label.GenerateConstructor");
                options.Add(new GeneratorItem(label, GeneratorJob.EnumConstructor, () => Generate(GeneratorJob.EnumConstructor, sci, expr)));
            }
            else base.ShowNewMethodList(sci, expr, found, options);
        }

        protected override void ShowNewVarList(ScintillaControl sci, ASResult expr, FoundDeclaration found, ICollection<ICompletionListItem> options)
        {
            var inClass = expr.RelClass;
            if (inClass != null && inClass.Flags.HasFlag(FlagType.Enum) && expr.IsStatic)
            {
                var label = TextHelper.GetString("ASCompletion.Label.GenerateConstructor");
                options.Add(new GeneratorItem(label, GeneratorJob.EnumConstructor, () => Generate(GeneratorJob.EnumConstructor, sci, expr)));
            }
            else base.ShowNewVarList(sci, expr, found, options);
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
                || ASContext.Context.CurrentClass.Members.Contains($"get_{newMember.Name}", FlagType.Function, 0)) return string.Empty;
            return base.TryGetOverrideGetterTemplate(ofClass, parameters, newMember);
        }

        protected override string TryGetOverrideSetterTemplate(ClassModel ofClass, List<MemberModel> parameters, MemberModel newMember)
        {
            if (parameters == null || parameters.Count == 0 || parameters.Count > 2 || parameters.Last().Name  != "set"
                || ASContext.Context.CurrentClass.Members.Contains($"set_{newMember.Name}", FlagType.Function, 0)) return string.Empty;
            return base.TryGetOverrideSetterTemplate(ofClass, parameters, newMember);
        }

        bool CanShowGenerateSwitch(ScintillaControl sci, int position, ASResult expr)
        {
            var member = expr.Member;
            if (member == null
                || member.Flags.HasFlag(FlagType.Enum) 
                || (member.Flags.HasFlag(FlagType.ParameterVar) && expr.Context.BeforeBody)) return false;
            var ctx = ASContext.Context;
            var word = expr.Context.WordBefore;
            if (word == ctx.Features.varKey || word == ctx.Features.functionKey) return false;
            var contextMember = expr.Context.ContextMember;
            var end = contextMember != null ? sci.PositionFromLine(contextMember.LineTo) : sci.TextLength;
            for (var i = ASComplete.ExpressionEndPosition(sci, position); i < end; i++)
            {
                if (sci.PositionIsOnComment(i)) continue;
                var c = (char) sci.CharAt(i);
                if (c <= ' ') continue;
                if (c == '.') return false;
                break;
            }
            var type = ctx.ResolveType(member.Type, expr.InFile);
            return (type.Flags.HasFlag(FlagType.Enum) && type.Members.Count > 0)
                   || (type.Flags.HasFlag(FlagType.Abstract) && type.MetaDatas != null && type.MetaDatas.Any(it => it.Name == ":enum")
                       && type.Members.Items.Any(it => it.Flags.HasFlag(FlagType.Variable)));
        }

        static void Generate(GeneratorJob job, ScintillaControl sci, ASResult expr)
        {
            switch (job)
            {
                case GeneratorJob.EnumConstructor:
                    sci.BeginUndoAction();
                    try
                    {
                        GenerateEnumConstructor(sci, expr, expr.RelClass);
                    }
                    finally { sci.EndUndoAction(); }
                    break;
                case GeneratorJob.Switch:
                    sci.BeginUndoAction();
                    try
                    {
                        GenerateSwitch(sci, expr, ASContext.Context.ResolveType(expr.Member.Type, ASContext.Context.CurrentModel));
                    }
                    finally { sci.EndUndoAction(); }
                    break;
            }
        }

        static void GenerateEnumConstructor(ScintillaControl sci, ASResult expr, ClassModel inClass)
        {
            var end = sci.WordEndPosition(sci.CurrentPos, true);
            var parameters = ParseFunctionParameters(sci, end);
            var currentClass = ASContext.Context.CurrentClass;
            if (currentClass != inClass)
            {
                AddLookupPosition(sci);
                lookupPosition = -1;
                if (currentClass.InFile != inClass.InFile) sci = ((ITabbedDocument) PluginBase.MainForm.OpenEditableDocument(inClass.InFile.FileName, false)).SciControl;
                ASContext.Context.UpdateContext(inClass.LineFrom);
            }
            var position = GetBodyStart(inClass.LineFrom, inClass.LineTo, sci);
            if (ASContext.Context.Settings.GenerateImports && parameters.Count > 0)
            {
                var types = GetQualifiedTypes(parameters.Select(it => it.paramQualType), inClass.InFile);
                position += AddImportsByName(types, sci.LineFromPosition(position));
            }
            sci.SetSel(position, position);
            var member = new MemberModel(contextToken, inClass.Type, FlagType.Constructor, Visibility.Public)
            {
                Parameters = parameters.Select(it => new MemberModel(it.paramName, it.paramQualType, FlagType.ParameterVar, 0)).ToList()
            };
            var declaration = member.Name;
            if (parameters.Count > 0) declaration += $"({member.ParametersString()})";
            declaration += ";";
            InsertCode(position, declaration, sci);
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
            var sb = new StringBuilder();
            for (var i = 0; i < inClass.Members.Count; i++)
            {
                var it = inClass.Members[i];
                sb.Append(SnippetHelper.BOUNDARY);
                if (i > 0) sb.Append('\n');
                sb.Append("\tcase ");
                sb.Append(it.Name);
                if (it.Parameters != null)
                {
                    sb.Append('(');
                    for (var j = 0; j < it.Parameters.Count; j++)
                    {
                        if (j > 0) sb.Append(", ");
                        sb.Append(it.Parameters[j].Name.TrimStart('?'));
                    }
                    sb.Append(')');
                }
                sb.Append(':');
                if (i == 0)
                {
                    sb.Append(' ');
                    sb.Append(SnippetHelper.ENTRYPOINT);
                }
            }
            template = TemplateUtils.ReplaceTemplateVariable(template, "Body", sb.ToString());
            InsertCode(start, template, sci);
        }
    }

    internal class GeneratorItem : ICompletionListItem
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
