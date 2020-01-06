using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ASCompletion.Completion;
using ASCompletion.Context;
using ASCompletion.Generators;
using ASCompletion.Model;
using ASCompletion.Settings;
using HaXeContext.Model;
using PluginCore;
using PluginCore.Controls;
using PluginCore.Helpers;
using PluginCore.Localization;
using PluginCore.Managers;
using PluginCore.Utilities;
using ScintillaNet;

namespace HaXeContext.Generators
{
    public enum GeneratorJob : long
    {
        EnumConstructor = GeneratorJobType.User << 1,
        Switch = GeneratorJobType.User << 2,
        IVariable = GeneratorJobType.User << 3,
        ConvertStaticMethodCallToStaticExtensionCall = GeneratorJobType.User << 4,
        Enum = GeneratorJobType.User << 5,
        ConstructorWithInitializer = GeneratorJobType.User << 6,
        InitializeLocalVariable = GeneratorJobType.User << 7,
    }

    class CodeGenerator : ASGenerator
    {
        readonly CodeGeneratorInterfaceBehavior codeGeneratorInterfaceBehavior = new CodeGeneratorInterfaceBehavior();
        readonly CodeGeneratorAbstractBehavior codeGeneratorAbstractBehavior = new CodeGeneratorAbstractBehavior();

        protected override ICodeGeneratorBehavior GetCodeGeneratorBehavior()
        {
            if ((ASContext.Context.CurrentClass.Flags & FlagType.Interface) != 0)
                return codeGeneratorInterfaceBehavior;
            if ((ASContext.Context.CurrentClass.Flags & FlagType.Abstract) != 0)
                return codeGeneratorAbstractBehavior;
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
                if (contextToken != null && expr.Member is null && !ctx.IsImported(expr.Type ?? ClassModel.VoidClass, sci.CurrentLine)) CheckAutoImport(expr, options);
                return;
            }
            if (CanShowGenerateSwitch(sci, position, expr))
            {
                var label = TextHelper.GetString("Info.GenerateSwitch");
                options.Add(new GeneratorItem(label, (GeneratorJobType) GeneratorJob.Switch, () => Generate(GeneratorJob.Switch, sci, expr)));
            }
            if (CanShowConvertStaticMethodCallToStaticExtensionCall(sci, expr))
            {
                var label = TextHelper.GetString("Label.ConvertStaticMethodCallToStaticExtensionCall");
                options.Add(new GeneratorItem(label, (GeneratorJobType) GeneratorJob.ConvertStaticMethodCallToStaticExtensionCall, () => Generate(GeneratorJob.ConvertStaticMethodCallToStaticExtensionCall, sci, expr)));
            }
            if (CanShowInitializeLocalVariable(expr))
            {
                var label = string.Format(TextHelper.GetString("Label.InitializeLocalVariable"), expr.Member.Name);
                options.Add(new GeneratorItem(label, (GeneratorJobType)GeneratorJob.InitializeLocalVariable, () => Generate(GeneratorJob.InitializeLocalVariable, sci, expr)));
            }
            base.ContextualGenerator(sci, position, expr, options);
        }

        /// <inheritdoc />
        protected override bool CanShowAssignStatementToVariable(ScintillaControl sci, ASResult expr)
        {
            if (!base.CanShowAssignStatementToVariable(sci, expr)) return false;
            // for example: return cast expr<generator>, return untyped expr<generator>
            return (expr.Context.WordBefore != "cast" || expr.Context.WordBefore != "untyped")
                   && sci.GetWordLeft(expr.Context.WordBeforePosition - 1, true) != "return";
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
        protected override bool CanShowGenerateInterface(ScintillaControl sci, int position, ASResult expr, FoundDeclaration found)
        {
            return !string.IsNullOrEmpty(contextToken) && char.IsUpper(contextToken[0]) && base.CanShowGenerateInterface(sci, position, expr, found);
        }

        /// <inheritdoc />
        protected override bool CanShowGenerateClass(ScintillaControl sci, int position, ASResult expr, FoundDeclaration found)
        {
            return !string.IsNullOrEmpty(contextToken)
                   && char.IsUpper(contextToken[0])
                   && base.CanShowGenerateClass(sci, position, expr, found);
        }

        static bool CanShowGenerateEnum(ScintillaControl sci, int position, ASResult expr, FoundDeclaration found)
        {
            return !string.IsNullOrEmpty(contextToken)
                   && char.IsUpper(contextToken[0])
                   // for example: public var foo : Foo<generator>
                   && expr.Context.Separator == ":";
        }

        static void ShowGenerateEnumList(ScintillaControl sci, ASResult expr, FoundDeclaration found, ICollection<ICompletionListItem> options)
        {
            var label = TextHelper.GetString("ASCompletion.Label.GenerateEnum");
            options.Add(new GeneratorItem(label, (GeneratorJobType) GeneratorJob.Enum, () =>
            {
                sci.BeginUndoAction();
                try
                {
                    GenerateEnum(sci, found.InClass, expr.Context);
                }
                finally { sci.EndUndoAction(); }
            }));
        }

        /// <inheritdoc />
        protected override bool CanShowGetSetList(ScintillaControl sci, int position, ASResult expr, FoundDeclaration found)
        {
            var inClass = expr.RelClass ?? found.InClass ?? ClassModel.VoidClass;
            return !inClass.Flags.HasFlag(FlagType.Abstract) && base.CanShowGetSetList(sci, position, expr, found);
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

        /// <inheritdoc />
        protected override bool TryShowGenerateType(ScintillaControl sci, int position, ASResult expr, FoundDeclaration found, List<ICompletionListItem> options)
        {
            var result = base.TryShowGenerateType(sci, position, expr, found, options);
            // TryShowGenerateAbstract
            if (CanShowGenerateEnum(sci, position, expr, found)) ShowGenerateEnumList(sci, expr, found, options);
            // TryShowGenerateTypedef
            return result;
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
                foreach (var member in tmpClass.Members)
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
                if (last is null || last.Name != member.Name)
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
                case "cast" when !expr.SubExpressions.IsNullOrEmpty() && expr.Value[0] == '#':
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

        protected override void GenerateClass(ScintillaControl sci, int position, MemberModel inClass, string name, Hashtable info)
        {
            info["GenericTemplate"] = GetGenericDeclaration(sci, position);
            base.GenerateClass(sci, position, inClass, name, info);
        }

        protected override void GenerateInterface(ScintillaControl sci, MemberModel inClass, string name, Hashtable info)
        {
            info["GenericTemplate"] = GetGenericDeclaration(sci, sci.CurrentPos);
            base.GenerateInterface(sci, inClass, name, info);
        }

        static string GetGenericDeclaration(ScintillaControl sci, int position)
        {
            position = ASComplete.ExpressionEndPosition(sci, position, true);
            if (ASComplete.GetCharRight(sci, ref position) != '<') return null;
            var endTemplatePosition = ((Context)ASContext.GetLanguageContext("haxe")).BraceMatch(sci, position);
            if (endTemplatePosition == -1) return null;
            var parCount = 0;
            var braCount = 0;
            var genCount = 0;
            var numTypes = 1;
            for (var i = position + 1; i < endTemplatePosition; i++)
            {
                if (sci.PositionIsOnComment(i)) continue;
                var c = (char)sci.CharAt(i);
                if (c == ' ') continue;
                if (c == '(')
                {
                    if (braCount == 0 && genCount == 0) parCount++;
                }
                else if (c == ')')
                {
                    if (braCount == 0 && genCount == 0) parCount--;
                }
                else if (c == '{')
                {
                    if (parCount == 0 && genCount == 0) braCount++;
                }
                else if (c == '}')
                {
                    if (parCount == 0 && genCount == 0) braCount--;
                }
                else if (c == '<')
                {
                    if (braCount == 0 && parCount == 0) genCount++;
                }
                else if (c == '>')
                {
                    if (braCount == 0 && parCount == 0) genCount--;
                }
                else if (c == ',' && parCount == 0 && braCount == 0 && genCount == 0) numTypes++;
            }
            if (numTypes == 1) return "<T>";
            var sb = new StringBuilder("<T1");
            for (var i = 1; i < numTypes; i++)
            {
                sb.Append(", T");
                sb.Append((i + 1).ToString());
            }
            sb.Append(">");
            return sb.ToString();
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

        protected override void GenerateFunction(ScintillaControl sci, int position, ClassModel inClass, MemberModel member, bool detach)
        {
            if (inClass.Flags.HasFlag(FlagType.TypeDef) || inClass.Flags.HasFlag(FlagType.Interface))
            {
                var template = TemplateUtils.GetTemplate("IFunction");
                if (string.IsNullOrEmpty(member.Type)) template = TemplateUtils.ReplaceTemplateVariable(template, "Type", ASContext.Context.Features.voidKey);
                var declaration = TemplateUtils.ToDeclarationString(member, template);
                GenerateFunction(position, declaration, detach);
            }
            else base.GenerateFunction(sci, position, inClass, member, detach);
        }

        protected override void GenerateProperty(GeneratorJobType job, ScintillaControl sci, ClassModel inClass, MemberModel member)
        {
            var location = ASContext.CommonSettings.PropertiesGenerationLocation;
            var latest = TemplateUtils.GetTemplateBlockMember(sci, TemplateUtils.GetBoundary("AccessorsMethods"));
            if (latest is null)
            {
                if (location == PropertiesGenerationLocations.AfterLastPropertyDeclaration)
                {
                    latest = job switch
                    {
                        GeneratorJobType.Setter => ASComplete.FindMember("get_" + member.Name, inClass),
                        GeneratorJobType.Getter => ASComplete.FindMember("set_" + member.Name, inClass),
                        _ => null
                    } ?? FindLatest(FlagType.Function, 0, inClass, false, false);
                }
                else latest = member;
            }
            else location = PropertiesGenerationLocations.AfterLastPropertyDeclaration;
            if (latest is null) return;
            sci.BeginUndoAction();
            try
            {
                var name = member.Name;
                var args = job switch
                {
                    GeneratorJobType.GetterSetter => "(get, set)",
                    GeneratorJobType.Getter => "(get, null)",
                    GeneratorJobType.Setter => "(default, set)",
                    _ => "(default, default)"
                };
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
                var position = sci.PositionFromLine(atLine) - (sci.EOLMode == 0 ? 2 : 1);
                sci.SetSel(position, position);
                // for example: var foo<generator>:TParam1->TReturn;
                if ((member.Flags & FlagType.Function) != 0)
                {
                    member = (MemberModel) member.Clone();
                    member.Type = ASContext.Context.CodeComplete.ToFunctionDeclarationString(member);
                }
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
            if (result.Member is { } member
                && string.IsNullOrEmpty(member.Type)
                && (member.Flags.HasFlag(FlagType.Variable)
                    || member.Flags.HasFlag(FlagType.Getter)
                    || member.Flags.HasFlag(FlagType.Setter)))
            {
                ASContext.Context.CodeComplete.InferType(PluginBase.MainForm.CurrentDocument?.SciControl, member);
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
            if (handler?.Parameters is { } parameters && parameters.Count > 1 && parameters[1]?.Type is { } type)
            {
                if (type == "haxe.Constraints.Function") return string.Empty;
                if (FileParser.IsFunctionType(type))
                {
                    var features = ASContext.Context.Features;
                    var member = FileParser.FunctionTypeToMemberModel<MemberModel>(type, features);
                    if (member.Parameters.Count > 0 && member.Parameters[0].Type is { } result)
                    {
                        return result.Equals(features.voidKey)
                            ? string.Empty
                            : result;
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
                if (member.Parameters is { } parameters)
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
                if (!paramType.StartsWithOrdinal("Null<")) return $"Null<{paramType}>";
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
                var template = NewLine + TemplateUtils.ToDeclarationString(method, TemplateUtils.GetTemplate(templateName));
                template = TemplateUtils.ReplaceTemplateVariable(template, "Modifiers", null);
                template = TemplateUtils.ReplaceTemplateVariable(template, "Member", method.Name);
                result += template;
            }
            return result;
        }

        protected override void ShowNewMethodList(ScintillaControl sci, ASResult expr, FoundDeclaration found, ICollection<ICompletionListItem> options)
        {
            var inClass = expr.RelClass;
            if (inClass != null && inClass.Flags.HasFlag(FlagType.Enum) && expr.IsStatic)
            {
                var label = TextHelper.GetString("ASCompletion.Label.GenerateConstructor");
                options.Add(new GeneratorItem(label, (GeneratorJobType) GeneratorJob.EnumConstructor, () => Generate(GeneratorJob.EnumConstructor, sci, expr)));
            }
            else base.ShowNewMethodList(sci, expr, found, options);
        }

        protected override void ShowNewVarList(ScintillaControl sci, ASResult expr, FoundDeclaration found, ICollection<ICompletionListItem> options)
        {
            var inClass = expr.RelClass;
            if (inClass != null && inClass.Flags.HasFlag(FlagType.Enum) && expr.IsStatic)
            {
                var label = TextHelper.GetString("ASCompletion.Label.GenerateConstructor");
                options.Add(new GeneratorItem(label, (GeneratorJobType) GeneratorJob.EnumConstructor, () => Generate(GeneratorJob.EnumConstructor, sci, expr)));
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
            if (parameters.IsNullOrEmpty() || parameters.First().Name != "get"
                || ASContext.Context.CurrentClass.Members.Contains($"get_{newMember.Name}", FlagType.Function, 0)) return string.Empty;
            return base.TryGetOverrideGetterTemplate(ofClass, parameters, newMember);
        }

        protected override string TryGetOverrideSetterTemplate(ClassModel ofClass, List<MemberModel> parameters, MemberModel newMember)
        {
            if (parameters.IsNullOrEmpty() || parameters.Count > 2 || parameters.Last().Name  != "set"
                || ASContext.Context.CurrentClass.Members.Contains($"set_{newMember.Name}", FlagType.Function, 0)) return string.Empty;
            return base.TryGetOverrideSetterTemplate(ofClass, parameters, newMember);
        }

        protected override MemberModel ToParameterVar(FunctionParameter member)
        {
            if (member.result?.Type is { } t && (t.Flags & FlagType.Struct) != 0)
                return new MemberModel(AvoidKeyword(member.paramName), GetShortType(t.Type), FlagType.ParameterVar, 0);
            return base.ToParameterVar(member);
        }

        protected override string ToDeclarationWithModifiersString(MemberModel member, string template)
        {
            if (((HaXeSettings) ASContext.Context.Settings).DisableVoidTypeDeclaration && member.Type == ASContext.Context.Features.voidKey)
                template = TemplateUtils.ReplaceTemplateVariable(template, "Type", null);
            return base.ToDeclarationWithModifiersString(member, template);
        }

        static bool CanShowGenerateSwitch(ScintillaControl sci, int position, ASResult expr)
        {
            var member = expr.Member;
            if (member is null
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
                       && type.Members.Any(it => it.Flags.HasFlag(FlagType.Variable)));
        }

        static bool CanShowConvertStaticMethodCallToStaticExtensionCall(ScintillaControl sci, ASResult expr)
        {
            return expr.Member is { } member
                   && member.Parameters?.Count > 0
                   && (member.LineFrom != sci.CurrentLine || !expr.Context.BeforeBody)
                   && member.Flags.HasFlag(FlagType.Static | FlagType.Function);
        }

        static bool CanShowInitializeLocalVariable(ASResult expr)
        {
            return expr.Member is { } member
                   && member.Flags.HasFlag(FlagType.LocalVar)
                   && !member.Flags.HasFlag(FlagType.Inferred)
                   && member.Value is null;
        }

        static void Generate(GeneratorJob job, ScintillaControl sci, ASResult expr)
        {
            sci.BeginUndoAction();
            try
            {
                switch (job)
                {
                    case GeneratorJob.EnumConstructor:
                        GenerateEnumConstructor(sci, expr.RelClass);
                        break;
                    case GeneratorJob.Switch:
                        GenerateSwitch(sci, expr);
                        break;
                    case GeneratorJob.ConvertStaticMethodCallToStaticExtensionCall:
                        ConvertStaticMethodCallToStaticExtensionCall(sci, expr);
                        break;
                    case GeneratorJob.InitializeLocalVariable:
                        InitializeLocalVariable(sci, expr);
                        break;
                }
            }
            finally { sci.EndUndoAction(); }
        }

        internal static void ConvertStaticMethodCallToStaticExtensionCall(ScintillaControl sci, ASResult expr)
        {
            var member = expr.Member;
            var inClass = expr.InClass;
            var isImported = ASContext.Context.IsImported(inClass, sci.CurrentLine);
            var caretPos = sci.CurrentPos;
            var startPos = expr.Context.PositionExpression;
            var endPos = sci.LineEndPosition(expr.Context.ContextFunction?.LineTo ?? expr.Context.ContextMember.LineFrom);
            endPos = GetEndOfStatement(startPos, endPos, sci);
            var parameters = ParseFunctionParameters(sci, sci.WordEndPosition(caretPos, true));
            var ctx = parameters[0].result.Context;
            var value = ctx.Value;
            if (ctx.SubExpressions != null)
            {
                var startIndex = 0;
                for (var i = ctx.SubExpressions.Count - 1; i >= 0; i--)
                {
                    var pattern = ".#" + i + "~";
                    startIndex = value.IndexOf(pattern, startIndex);
                    if (startIndex == -1) continue;
                    var newValue = ctx.SubExpressions[i];
                    value = value.Replace(pattern, newValue);
                    startIndex += newValue.Length;
                }
            }
            value = value.Replace(".[", "[");
            if (ctx.WordBefore == "new") value = "new " + value;
            var statement = value + "." + member.Name + "(" + string.Join(", ", parameters.Skip(1).Select(it =>
            {
                if (it.result.Member is { } model) return model.ToDeclarationString();
                return it.result.Context.Value;
            })) + ");";
            sci.SetSel(startPos, endPos);
            sci.ReplaceSel(statement);
            if (isImported) return;
            caretPos += value.Length - (expr.Path.Length - member.Name.Length);
            caretPos += InsertUsing(inClass);
            sci.SetSel(caretPos, caretPos);
        }

        /// <summary>
        /// Add an 'using' statement in the current file
        /// </summary>
        /// <param name="member">Generates 'using {member.Type};'</param>
        /// <returns>Inserted characters count</returns>
        static int InsertUsing(MemberModel member)
        {
            var statement = string.Empty;
            if (member.InFile is { } inFile && member.Name != inFile.Module)
            {
                if (!string.IsNullOrEmpty(inFile.Package)) statement = inFile.Package + ".";
                statement += inFile.Module + "." + member.Name;
            }
            if (string.IsNullOrEmpty(statement)) statement = member.Type;
            var sci = PluginBase.MainForm.CurrentDocument.SciControl;
            var newLineMarker = LineEndDetector.GetNewLineMarker(sci.EOLMode);
            statement = "using " + statement + ";" + newLineMarker;
            int position;
            var ctx = ASContext.Context;
            var cFile = ctx.CurrentModel;
            var line = ctx.InPrivateSection ? cFile.PrivateSectionIndex : 0;
            if (cFile.InlinedRanges != null)
            {
                position = sci.CurrentPos;
                foreach (var range in cFile.InlinedRanges)
                {
                    if (position > range.Start && position < range.End)
                    {
                        line = sci.LineFromPosition(range.Start) + 1;
                        break;
                    }
                }
            }
            var firstLine = line;
            var found = false;
            var indent = 0;
            var skipIfDef = 0;
            var importKey = ctx.Features.importKey;
            var importKeyAlt = ctx.Features.importKeyAlt;
            var importKeyAltLength = importKeyAlt.Length;
            var curLine = sci.CurrentLine;
            while (line < curLine)
            {
                var txt = sci.GetLine(line++).TrimStart();
                if (txt.StartsWith("package") || txt.StartsWithOrdinal(importKey)) firstLine = line;
                else if (txt.StartsWithOrdinal("#if") && txt.IndexOfOrdinal("#end") == -1) skipIfDef++;
                else if (skipIfDef > 0)
                {
                    if (txt.StartsWithOrdinal("#end")) skipIfDef--;
                }
                else if (txt.Length > importKeyAltLength && txt.StartsWithOrdinal(importKeyAlt) && txt[importKeyAltLength] <= 32)
                {
                    found = true;
                    indent = sci.GetLineIndentation(line - 1);
                    var m = ASFileParserRegexes.Import.Match(txt);
                    if (m.Success && CaseSensitiveImportComparer.CompareImports(m.Groups["package"].Value, member.Type) > 0)
                    {
                        line--;
                        break;
                    }
                }
                else if (found)
                {
                    line--;
                    break;
                }
            }
            if (line == curLine) line = firstLine;
            position = sci.PositionFromLine(line);
            firstLine = sci.FirstVisibleLine;
            sci.SetSel(position, position);
            sci.ReplaceSel(statement);
            sci.SetLineIndentation(line, indent);
            sci.LineScroll(0, firstLine - sci.FirstVisibleLine + 1);
            ctx.RefreshContextCache(member.Type);
            return sci.GetLine(line).Length;
        }

        static void GenerateEnum(ScintillaControl sci, MemberModel inClass, ASExpr expr)
        {
            //ASGenerator.AddLookupPosition(); // remember last cursor position for Shift+F4
            var info = new Hashtable();
            info["GenericTemplate"] = GetGenericDeclaration(sci, sci.WordEndPosition(expr.PositionExpression, false));
            info["className"] = string.IsNullOrEmpty(expr.Value) ? "Enum" : expr.Value;
            info["templatePath"] = Path.Combine(PathHelper.TemplateDir, "ProjectFiles", PluginBase.CurrentProject.GetType().Name, $"Enum{ASContext.Context.Settings.DefaultExtension}.fdt");
            info["inDirectory"] = Path.GetDirectoryName(inClass.InFile.FileName);
            var de = new DataEvent(EventType.Command, "ProjectManager.CreateNewFile", info);
            EventManager.DispatchEvent(null, de);
        }

        static void GenerateEnumConstructor(ScintillaControl sci, MemberModel inClass)
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

        static void GenerateSwitch(ScintillaControl sci, ASResult expr)
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
            var inClass = ASContext.Context.ResolveType(expr.Member.Type, ASContext.Context.CurrentModel);
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

        internal static void GenerateAnonymousFunction(ScintillaControl sci, MemberModel member, string template)
        {
            var ctx = ASContext.Context;
            string body = null;
            switch (ASContext.CommonSettings.GeneratedMemberDefaultBodyStyle)
            {
                case GeneratedMemberBodyStyle.ReturnDefaultValue:
                    var returnTypeName = member.Type;
                    var returnType = ctx.ResolveType(returnTypeName, ctx.CurrentModel);
                    if ((returnType.Flags & FlagType.Abstract) != 0
                        && !string.IsNullOrEmpty(returnType.ExtendsType)
                        && returnType.ExtendsType != ctx.Features.dynamicKey)
                        returnTypeName = returnType.ExtendsType;
                    var defaultValue = ctx.GetDefaultValue(returnTypeName);
                    if (!string.IsNullOrEmpty(defaultValue)) body = $"return {defaultValue};";
                    break;
            }
            template = ((CodeGenerator) ctx.CodeGenerator).ToDeclarationWithModifiersString(member, template);
            template = TemplateUtils.ReplaceTemplateVariable(template, "Body", body);
            sci.SelectWord();
            InsertCode(sci.CurrentPos, template);
        }

        static void InitializeLocalVariable(ScintillaControl sci, ASResult expr)
        {
            var model = expr.Context.ContextFunction;
            var position = GetBodyStart(model.LineFrom, model.LineTo, sci, false);
            position += expr.Member.StartPosition - 1;
            position += expr.Context.Value.Length;
            var c = ASComplete.GetCharRight(sci, true, ref position);
            if (c == ':') position = ASComplete.ExpressionEndPosition(sci, position + 1, true);
            sci.InsertText(position, " ");
            position++;
            sci.SetSel(position, position);
            var value = ASContext.Context.GetDefaultValue(expr.Member.Type);
            InsertCode(position, $" = $(EntryPoint){value}$(ExitPoint)", sci);
        }
    }
}