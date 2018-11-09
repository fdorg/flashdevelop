using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using ASCompletion.Completion;
using ASCompletion.Context;
using ASCompletion.Model;
using ASCompletion.Settings;
using HaXeContext.Model;
using PluginCore;
using PluginCore.Controls;
using PluginCore.Localization;
using ScintillaNet;

namespace HaXeContext.Completion
{
    class CodeComplete : ASComplete
    {
        protected override bool IsAvailable(IASContext ctx, bool autoHide)
        {
            return base.IsAvailable(ctx, autoHide) && (!autoHide || ((HaXeSettings)ctx.Settings).DisableCompletionOnDemand);
        }

        public override bool IsRegexStyle(ScintillaControl sci, int position)
        {
            var result = base.IsRegexStyle(sci, position);
            if (result) return true;
            return sci.BaseStyleAt(position) == 10 && sci.CharAt(position) == '~' && sci.CharAt(position + 1) == '/';
        }

        /// <summary>
        /// Returns whether or not position is inside of an expression block in String interpolation ('${expr}')
        /// </summary>
        public override bool IsStringInterpolationStyle(ScintillaControl sci, int position)
        {
            if (!ASContext.Context.Features.hasStringInterpolation) return false;
            var stringChar = sci.GetStringType(position - 1);
            if (ASContext.Context.Features.stringInterpolationQuotes.Contains(stringChar))
            {
                var current = (char)sci.CharAt(position);
                for (var i = position - 1; i >= 0; i--)
                {
                    var next = current;
                    current = (char)sci.CharAt(i);
                    if (current == stringChar)
                    {
                        if (!IsEscapedCharacter(sci, i)) break;
                    }
                    else if (current == '$')
                    {
                        if (next == '{' && !IsEscapedCharacter(sci, i, '$')) return true;
                    }
                    else if (current == '}')
                    {
                        i = sci.BraceMatch(i);
                        current = (char)sci.CharAt(i);
                        if (i > 0 && current == '{' && sci.CharAt(i - 1) == '$') break;
                    }
                }
            }
            return false;
        }

        /// <inheritdoc />
        protected override bool OnChar(ScintillaControl sci, int value, char prevValue, bool autoHide)
        {
            switch (value)
            {
                case ':':
                    if (prevValue == '@') return HandleMetadataCompletion(autoHide);
                    break;
                case '>':
                    // for example: SomeType-><complete>
                    if (prevValue == '-' && IsType(sci.CurrentPos - 2)) return HandleNewCompletion(sci, string.Empty, autoHide, string.Empty);
                    break;
                case '(':
                    // for example: SomeType->(<complete>
                    if (prevValue == '>' && (sci.CurrentPos - 3) is int p && p > 0 && (char)sci.CharAt(p) == '-' && IsType(p))
                        return HandleNewCompletion(sci, string.Empty, autoHide, string.Empty);
                    // for example: @:forward(<complete> or @:forwardStatics(<complete>
                    return HandleForwardCompletion(sci, autoHide);
            }
            return false;
            // Utils
            bool IsType(int position) => GetExpressionType(sci, position, false, true).Type is ClassModel t && !t.IsVoid();
        }

        static bool HandleMetadataCompletion(bool autoHide)
        {
            var list = new List<ICompletionListItem>();
            foreach (var meta in ASContext.Context.Features.metadata)
            {
                var member = new MemberModel();
                member.Name = meta.Key;
                member.Comments = meta.Value;
                member.Type = "Compiler Metadata";
                list.Add(new MemberItem(member));
                CompletionList.Show(list, autoHide);
            }
            return true;
        }

        /// <inheritdoc />
        protected override bool HandleWhiteSpaceCompletion(ScintillaControl sci, int position, string wordLeft, bool autoHide)
        {
            if (string.IsNullOrEmpty(wordLeft))
            {
                var pos = position - 1;
                wordLeft = GetWordLeft(sci, ref pos);
                if (string.IsNullOrEmpty(wordLeft))
                {
                    var c = (char) sci.CharAt(pos--);
                    if (c == '=') return HandleAssignCompletion(sci, pos, autoHide);
                    // for example: case EnumValue | <complete> or case EnumValue, <complete>
                    if (c == '|' || c == ',')
                    {
                        while (GetExpressionType(sci, pos + 1, false, true) is ASResult expr && expr.Type != null && !expr.Type.IsVoid())
                        {
                            if (expr.Context.WordBefore == "case") return HandleSwitchCaseCompletion(sci, pos, autoHide);
                            if (expr.Context.Separator is string separator && (separator == "|" || separator == ","))
                                pos = expr.Context.SeparatorPosition - 1;
                            else break;
                        }
                    }
                    // for example: @:forward(methodName, <complete> or @:forwardStatics(methodName, <complete>
                    return HandleForwardCompletion(sci, autoHide);
                }
                return false;
            }
            var currentClass = ASContext.Context.CurrentClass;
            if (currentClass.Flags.HasFlag(FlagType.Abstract) && (wordLeft == "from" || wordLeft == "to"))
            {
                return PositionIsBeforeBody(sci, position, currentClass) && HandleNewCompletion(sci, string.Empty, autoHide, wordLeft);
            }
            return wordLeft == "case" && HandleSwitchCaseCompletion(sci, position, autoHide);
        }

        bool HandleAssignCompletion(ScintillaControl sci, int position, bool autoHide)
        {
            var c = (char) sci.CharAt(position);
            var expr = GetExpressionType(sci, position, false, true);
            if (!(expr.Type is ClassModel type)) return false;
            var ctx = ASContext.Context;
            // for example: function(v:Type = <complete>
            if (expr.Context.ContextFunction != null && expr.Context.BeforeBody && !IsEnum(type))
            {
                // for example: function(v:Bool = <complete>
                if (type.Name == ctx.Features.booleanKey)
                {
                    var word = sci.GetWordFromPosition(sci.CurrentPos);
                    if (string.IsNullOrEmpty(word) || "true".StartsWithOrdinal(word))
                        completionHistory[ctx.CurrentClass.QualifiedName] = "true";
                    return HandleDotCompletion(sci, autoHide, null, (a, b) =>
                    {
                        var aLabel = (a as TemplateItem)?.Label;
                        var bLabel = (b as TemplateItem)?.Label;
                        if (IsBool(aLabel) && IsBool(bLabel))
                        {
                            if (aLabel == "true") return -1;
                            return 1;
                        }
                        if (IsBool(aLabel)) return -1;
                        if (IsBool(bLabel)) return 1;
                        return 0;
                        // Utils
                        bool IsBool(string s) => s == "true" || s == "false";
                    });
                }
                if (expr.Context.Separator != "->" && ctx.GetDefaultValue(type.Name) is string v && v != "null") return false;
                CompletionList.Show(new List<ICompletionListItem> {new TemplateItem(new MemberModel("null", "null", FlagType.Template, 0))}, autoHide);
                return true;
            }
            // for example: var v:Void->Void = <complete>, (v:Void->Void) = <complete>
            if (c == ' ' && (expr.Context.Separator == "->" || IsFunction(expr.Member)))
            {
                MemberModel member;
                // for example: (v:Void->Void) = <complete>
                if (IsFunction(expr.Member)) member = expr.Member;
                // for example: var v:Void->Void = <complete>
                else
                {
                    var functionType = type.Name;
                    while (expr.Context.Separator == "->")
                    {
                        expr = GetExpressionType(sci, expr.Context.SeparatorPosition, false, true);
                        if (expr.Type == null) return false;
                        functionType = expr.Type.Name + "->" + functionType;
                    }
                    member =  FileParser.FunctionTypeToMemberModel(functionType, ctx.Features);
                }
                if (member == null) return false;
                var functionName = "function() {}";
                var list = new List<ICompletionListItem> {new AnonymousFunctionGeneratorItem(functionName, () => GenerateAnonymousFunction(sci, member, TemplateUtils.GetTemplate("AnonymousFunction")))};
                if (ctx is Context context && context.GetCurrentSDKVersion() >= "4.0.0")
                {
                    functionName = "() -> {}";
                    list.Insert(0, new AnonymousFunctionGeneratorItem(functionName, () => GenerateAnonymousFunction(sci, member, TemplateUtils.GetTemplate("AnonymousFunction.Haxe4"))));
                }
                var word = sci.GetWordFromPosition(sci.CurrentPos);
                if (string.IsNullOrEmpty(word) || functionName.StartsWithOrdinal(word))
                    completionHistory[ctx.CurrentClass.QualifiedName] = functionName;
                return HandleDotCompletion(sci, autoHide, list, null);
            }
            // for example: v = <complete>, v != <complete>, v == <complete>
            if ((c == ' ' || c == '!' || c == '=') && IsEnum(type))
            {
                return HandleDotCompletion(sci, autoHide, null, (a, b) =>
                {
                    var aMember = (a as MemberItem)?.Member;
                    var bMember = (b as MemberItem)?.Member;
                    var aType = aMember?.Type;
                    var bType = bMember?.Type;
                    if (aType == type.Name && IsEnumValue(aMember.Flags)
                        && bType == type.Name && IsEnumValue(bMember.Flags))
                    {
                        return aMember.Name.CompareTo(bMember.Name);
                    }
                    if (aType == type.Name && IsEnumValue(aMember.Flags)) return -1;
                    if (bType == type.Name && IsEnumValue(bMember.Flags)) return 1;
                    return 0;
                    // Utils
                    bool IsEnumValue(FlagType flags) => (flags & FlagType.Static) != 0 && (flags & FlagType.Variable) != 0;
                });
            }
            return false;
            // Utils
            bool IsEnum(ClassModel t) => t.Flags.HasFlag(FlagType.Enum)
                                         || (t.Flags.HasFlag(FlagType.Abstract) && t.Members != null && t.Members.Count > 0
                                             && t.MetaDatas != null && t.MetaDatas.Any(it => it.Name == ":enum"));

            bool IsFunction(MemberModel m) => m != null && m.Flags.HasFlag(FlagType.Function);
        }

        bool HandleForwardCompletion(ScintillaControl sci, bool autoHide)
        {
            var ctx = ASContext.Context;
            if (!ctx.CurrentClass.IsVoid()) return false;
            var currentLine = sci.CurrentLine;
            var line = sci.GetLine(currentLine);
            // for example: @:forward() <complete>
            if (line.LastIndexOf(')') is int p && (p != -1 && (sci.PositionFromLine(currentLine) + p) < sci.CurrentPos)) return false;
            string metaName;
            FlagType mask;
            if (line.StartsWithOrdinal("@:forward("))
            {
                metaName = ":forward";
                mask = FlagType.Dynamic;
            }
            else if (line.StartsWithOrdinal("@:forwardStatics("))
            {
                metaName = ":forwardStatics";
                mask = FlagType.Static;
            }
            else return false;
            if (ctx.CurrentModel.Classes.Find(it => it.LineFrom > currentLine) is ClassModel @class && @class.Flags.HasFlag(FlagType.Abstract))
            {
                var extends = @class.Extends;
                if (!extends.IsVoid()) return false;
                extends = ResolveType(@class.ExtendsType, ctx.CurrentModel);
                if (extends.IsVoid()) return false;
                var list = new MemberList();
                base.GetInstanceMembers(autoHide, new ASResult(), extends, mask, -1, list);
                var @params = @class.MetaDatas?.Find(it => it.Name == metaName)?.Params;
                if (@params != null)
                {
                    var names = @params["Default"]
                        .Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
                        .Select(it => it.Trim()).ToArray();
                    if (names.Length != 0) list.Items.RemoveAll(it => Array.IndexOf(names, it.Name) != -1);
                }
                if (list.Count > 0) CompletionList.Show(list.Items.Select(it => new MemberItem(it)).ToList<ICompletionListItem>(), autoHide);
                return true;
            }
            return false;
        }

        protected override void LocateMember(ScintillaControl sci, int line, string keyword, string name)
        {
            LocateMember(sci, line, $"{keyword ?? ""}\\s*(\\?)?(?<name>{name.Replace(".", "\\s*.\\s*")})[^A-z0-9]");
        }

        protected override void ParseLocalVars(ASExpr expression, FileModel model)
        {
            for (int i = 0, count = expression.ContextFunction.Parameters.Count; i < count; i++)
            {
                var item = expression.ContextFunction.Parameters[i];
                var name = item.Name;
                if (name[0] == '?')
                {
                    if (string.IsNullOrEmpty(item.Type) && (expression.Separator != "=" || item.Value != expression.Value))
                        InferParameterType(item);
                    var type = item.Type;
                    if (string.IsNullOrEmpty(type)) type = "Null<Dynamic>";
                    else if (!type.StartsWithOrdinal("Null<")) type = $"Null<{type}>";
                    item = (MemberModel) item.Clone();
                    item.Name = name.Substring(1);
                    item.Type = type;
                }
                model.Members.MergeByLine(item);
            }
        }

        /// <inheritdoc />
        protected override bool ResolveFunction(ScintillaControl sci, int position, ASResult expr, bool autoHide)
        {
            var member = expr.Member;
            if (member != null && (member.Flags & FlagType.Variable) != 0 && FileParser.IsFunctionType(member.Type))
            {
                FunctionContextResolved(sci, expr.Context, member, expr.RelClass, false);
                return true;
            }
            var type = expr.Type;
            if ((expr.Member != null && expr.Path != "super") || type == null)
                return base.ResolveFunction(sci, position, expr, autoHide);
            var originConstructor = ASContext.GetLastStringToken(type.Name, ".");
            type.ResolveExtends();
            while (!type.IsVoid())
            {
                var constructor = type.Members.Search(ASContext.GetLastStringToken(type.Name, "."), FlagType.Constructor, 0);
                if (constructor != null)
                {
                    if (originConstructor != constructor.Name)
                    {
                        constructor = (MemberModel) constructor.Clone();
                        constructor.Name = originConstructor;
                    }
                    expr.Member = constructor;
                    expr.Context.Position = position;
                    FunctionContextResolved(sci, expr.Context, expr.Member, expr.RelClass, false);
                    return true;
                }
                if (type.Flags.HasFlag(FlagType.Abstract)) return false;
                type = type.Extends;
            }
            return false;
        }

        public void InferVariableType(ScintillaControl sci, MemberModel member) => InferVariableType(sci, new ASExpr(), member);

        /// <inheritdoc />
        protected override void InferVariableType(ScintillaControl sci, ASExpr local, MemberModel var)
        {
            if (!TryInferGenericType(var).IsVoid()) return;
            if (var.Flags.HasFlag(FlagType.ParameterVar))
            {
                if (FileParser.IsFunctionType(var.Type)) return;
                InferParameterType(var);
                return;
            }
            var ctx = ASContext.Context;
            var line = sci.GetLine(var.LineFrom);
            var m = Regex.Match(line, "\\s*for\\s*\\(\\s*" + var.Name + "\\s*in\\s*");
            if (!m.Success)
            {
                base.InferVariableType(sci, local, var);
                if (string.IsNullOrEmpty(var.Type) && (var.Flags & (FlagType.Variable | FlagType.Getter | FlagType.Setter)) != 0)
                    var.Type = ctx.ResolveType(ctx.Features.dynamicKey, null).Name;
                return;
            }
            var currentModel = ctx.CurrentModel;
            var rvalueStart = sci.PositionFromLine(var.LineFrom) + m.Index + m.Length;
            var methodEndPosition = sci.LineEndPosition(ctx.CurrentMember.LineTo);
            var parCount = 0;
            var braCount = 0;
            for (var i = rvalueStart; i < methodEndPosition; i++)
            {
                if (sci.PositionIsOnComment(i) || sci.PositionIsInString(i)) continue;
                var c = (char) sci.CharAt(i);
                if (c <= ' ') continue;
                if (c == '{') braCount++;
                else if (c == '}') braCount--;
                // for(i in 0...1)
                else if (c == '.' && sci.CharAt(i + 1) == '.' && sci.CharAt(i + 2) == '.')
                {
                    var type = ctx.ResolveType("Int", null);
                    var.Type = type.QualifiedName;
                    var.Flags |= FlagType.Inferred;
                    return;
                }
                if (c == '(') parCount++;
                // for(it in expr)
                else if (c == ')' || (c == ';' && braCount == 0))
                {
                    parCount--;
                    if (parCount >= 0) continue;
                    ASResult expr;
                    /**
                     * check:
                     * var a = [1,2,3,4];
                     * for(a in a)
                     * {
                     *     trace(a|); // | <-- cursor
                     * }
                     */
                    var wordLeft = sci.GetWordLeft(i - 1, false);
                    if (wordLeft == var.Name)
                    {
                        var lineBefore = sci.LineFromPosition(i) - 1;
                        var vars = local.LocalVars;
                        vars.Items.Sort((l, r) => l.LineFrom > r.LineFrom ? -1 : l.LineFrom < r.LineFrom ? 1 : 0);
                        var model = vars.Items.Find(it => it.LineFrom <= lineBefore);
                        if (model != null) expr = new ASResult {Type = ctx.ResolveType(model.Type, ctx.CurrentModel), InClass = ctx.CurrentClass};
                        // class members
                        else
                        {
                            expr = new ASResult();
                            FindMember(local.Value, ctx.CurrentClass, expr, 0, 0);
                            if (expr.IsNull()) return;
                        }
                    }
                    else expr = GetExpressionType(sci, i, false, true);
                    var exprType = expr.Type;
                    if (exprType == null) return;
                    string iteratorIndexType = null;
                    exprType.ResolveExtends();
                    while (!exprType.IsVoid())
                    {
                        // typedef Ints = Array<Int>
                        if (exprType.Flags.HasFlag(FlagType.TypeDef) && exprType.Members.Count == 0)
                        {
                            exprType = InferTypedefType(sci, exprType);
                            continue;
                        }
                        var members = exprType.Members;
                        var member = members.Search("iterator", 0, 0);
                        if (member == null)
                        {
                            if (members.Contains("hasNext", 0, 0))
                            {
                                member = members.Search("next", 0, 0);
                                if (member != null) iteratorIndexType = member.Type;
                            }
                            var exprTypeIndexType = exprType.IndexType;
                            if (exprType.Name.StartsWith("Iterator<") && !string.IsNullOrEmpty(exprTypeIndexType) && ctx.ResolveType(exprTypeIndexType, currentModel).IsVoid())
                            {
                                exprType = expr.InClass;
                                break;
                            }
                            if (iteratorIndexType != null) break;
                        }
                        else
                        {
                            var type = ctx.ResolveType(member.Type, currentModel);
                            iteratorIndexType = type.IndexType;
                            break;
                        }
                        exprType = exprType.Extends;
                    }
                    if (iteratorIndexType != null)
                    {
                        var.Type = iteratorIndexType;
                        var exprTypeIndexType = exprType.IndexType;
                        if (!string.IsNullOrEmpty(exprTypeIndexType) && exprTypeIndexType.Contains(','))
                        {
                            var t = exprType;
                            var originTypes = t.IndexType.Split(',');
                            if (!originTypes.Contains(var.Type))
                            {
                                var.Type = null;
                                t.ResolveExtends();
                                t = t.Extends;
                                while (!t.IsVoid())
                                {
                                    var types = t.IndexType.Split(',');
                                    for (var j = 0; j < types.Length; j++)
                                    {
                                        if (types[j] != iteratorIndexType) continue;
                                        var.Type = originTypes[j].Trim();
                                        break;
                                    }
                                    if (var.Type != null) break;
                                    t = t.Extends;
                                }
                            }
                        }
                    }
                    if (var.Type == null)
                    {
                        var type = ctx.ResolveType(ctx.Features.dynamicKey, null);
                        var.Type = type.QualifiedName;
                    }
                    var.Flags |= FlagType.Inferred;
                    return;
                }
            }
        }

        protected override void InferVariableType(ScintillaControl sci, string declarationLine, int rvalueStart, ASExpr local, MemberModel var)
        {
            if (local.PositionExpression <= rvalueStart && rvalueStart <= local.Position) return;
            var word = sci.GetWordRight(rvalueStart, true);
            // for example: var v = v;
            if (word == local.Value) return;
            if (word == "new")
            {
                rvalueStart = sci.WordEndPosition(rvalueStart, false) + 1;
                word = sci.GetWordRight(rvalueStart, true);
            }
            var ctx = ASContext.Context;
            /**
             * for example:
             * class Foo {
             *   function new() {
             *     untyped __js__('value').<complete>
             *   }
             * }
             */
            if (word == "untyped")
            {
                var type = ctx.ResolveType(ctx.Features.dynamicKey, null);
                var.Type = type.QualifiedName;
                var.Flags |= FlagType.Inferred;
                return;
            }
            if (var.Flags.HasFlag(FlagType.LocalVar))
            {
                if (!InferVariableType(sci, rvalueStart, var)) base.InferVariableType(sci, declarationLine, rvalueStart, local, var);
                return;
            }
            if (var.Flags.HasFlag(FlagType.Variable) || var.Flags.HasFlag(FlagType.Getter) || var.Flags.HasFlag(FlagType.Setter))
            {
                InferVariableType(sci, rvalueStart, var);
            }
        }

        bool InferVariableType(ScintillaControl sci, int rvalueStart, MemberModel var)
        {
            var ctx = ASContext.Context;
            var rvalueEnd = ExpressionEndPosition(sci, rvalueStart, sci.LineEndPosition(var.LineTo), true);
            var characterClass = ScintillaControl.Configuration.GetLanguage(sci.ConfigurationLanguage).characterclass.Characters;
            var arrCount = 0;
            var parCount = 0;
            var genCount = 0;
            var hadDot = false;
            var isInExpr = false;
            var lineTo = var.Flags.HasFlag(FlagType.LocalVar) || var.Flags.HasFlag(FlagType.ParameterVar)
                ? ctx.CurrentMember.LineTo
                : ctx.CurrentClass.LineTo;
            var endPosition = sci.LineEndPosition(lineTo);
            for (var i = rvalueEnd; i < endPosition; i++)
            {
                if (arrCount == 0 && parCount == 0 && genCount == 0)
                {
                    if (sci.PositionIsOnComment(i)) continue;
                    if (sci.PositionIsInString(i))
                    {
                        if (isInExpr) break;
                        continue;
                    }
                }
                var c = (char) sci.CharAt(i);
                if (c == '[' && genCount == 0 && parCount == 0)
                {
                    arrCount++;
                    isInExpr = true;
                }
                else if (c == ']' && genCount == 0 && parCount == 0)
                {
                    arrCount--;
                    rvalueEnd = i + 1;
                    if (arrCount < 0) break;
                }
                else if (c == '(' && genCount == 0 && arrCount == 0)
                {
                    parCount++;
                    isInExpr = true;
                }
                else if (c == ')' && genCount == 0 && arrCount == 0)
                {
                    parCount--;
                    rvalueEnd = i + 1;
                    if (parCount < 0) break;
                }
                else if (c == '<' && parCount == 0 && arrCount == 0)
                {
                    genCount++;
                    isInExpr = true;
                }
                else if (c == '>' && parCount == 0 && arrCount == 0)
                {
                    genCount--;
                    rvalueEnd = i + 1;
                    if (genCount < 0) break;
                }
                if (parCount > 0 || genCount > 0 || arrCount > 0) continue;
                if (c <= ' ')
                {
                    hadDot = false;
                    isInExpr = true;
                    continue;
                }
                if (c == ';' || (!hadDot && characterClass.Contains(c))) break;
                if (c == '.')
                {
                    hadDot = true;
                    rvalueEnd = ExpressionEndPosition(sci, i + 1, endPosition);
                }
                isInExpr = true;
            }
            var expr = GetExpressionType(sci, rvalueEnd, false, true);
            if (expr.Type != null)
            {
                // for example: var v = ClassType;
                if (expr.Type.Flags == FlagType.Class && expr.IsStatic)
                    var.Type = $"Class<{expr.Type.QualifiedName}>";
                else var.Type = expr.Type.QualifiedName;
                var.Flags |= FlagType.Inferred;
                return true;
            }
            if (expr.Member != null)
            {
                var.Type = expr.Member.Type;
                var.Flags |= FlagType.Inferred;
                return true;
            }
            return false;
        }

        void InferParameterType(MemberModel var)
        {
            var ctx = ASContext.Context;
            var value = var.Value;
            var type = ctx.ResolveToken(value, ctx.CurrentModel);
            if (type.IsVoid())
            {
                if (!string.IsNullOrEmpty(value) && value != "null" && var.ValueEndPosition != -1
                    && char.IsLetter(value[0]) && (var.Name != value && (var.Name[0] != '?' || var.Name != '?' + value)))
                    type = GetExpressionType(ASContext.CurSciControl, var.ValueEndPosition + 1, true).Type ?? ClassModel.VoidClass;
                if (type.IsVoid()) type = ctx.ResolveType(ctx.Features.dynamicKey, null);
            }
            var.Type = type.Name;
        }

        static ClassModel TryInferGenericType(MemberModel var)
        {
            var ctx = ASContext.Context;
            var template = ctx.CurrentMember?.Template ?? ctx.CurrentClass?.Template;
            if (!string.IsNullOrEmpty(template) && !string.IsNullOrEmpty(var.Type) &&
                ResolveType(var.Type, ctx.CurrentModel).IsVoid())
            {
                var templates = template.Substring(1, template.Length - 2).Split(',');
                foreach (var it in templates)
                {
                    var parts = it.Split(':');
                    if (parts.Length == 1 || parts[0] != var.Type) continue;
                    var type = ResolveType(parts[1], ctx.CurrentModel);
                    var.Type = type.Name;
                    var.Flags |= FlagType.Inferred;
                    return type;
                }
            }
            return ClassModel.VoidClass;
        }

        static ClassModel InferTypedefType(ScintillaControl sci, MemberModel expr)
        {
            var text = sci.GetLine(expr.LineFrom);
            var m = Regex.Match(text, "\\s*typedef\\s+" + expr.Name + "\\s*=([^;]+)");
            if (!m.Success) return ClassModel.VoidClass;
            var rvalue = m.Groups[1].Value.TrimStart();
            return ASContext.Context.ResolveType(rvalue, ASContext.Context.CurrentModel);
        }

        /// <inheritdoc />
        protected override bool HandleImplementsCompletion(ScintillaControl sci, bool autoHide)
        {
            var extends = new HashSet<string>();
            var list = new List<ICompletionListItem>();
            foreach (var it in ASContext.Context.GetAllProjectClasses().Items.Distinct())
            {
                extends.Clear();
                var type = it as ClassModel ?? ClassModel.VoidClass;
                type.ResolveExtends();
                while (!type.IsVoid() && type.Flags.HasFlag(FlagType.TypeDef) && type.Members.Count == 0)
                {
                    if (extends.Contains(type.Type)) break;
                    extends.Add(type.Type);
                    if (!string.IsNullOrEmpty(type.ExtendsType))
                    {
                        type = type.Extends;
                        if (extends.Contains(type.ExtendsType)) break;
                    }
                    else type = InferTypedefType(sci, type);
                }
                if (!type.Flags.HasFlag(FlagType.Interface)) continue;
                list.Add(new MemberItem(it));
            }
            CompletionList.Show(list, autoHide);
            return true;
        }

        protected override ASResult EvalExpression(string expression, ASExpr context, FileModel inFile, ClassModel inClass, bool complete, bool asFunction, bool filterVisibility)
        {
            if (!string.IsNullOrEmpty(expression))
            {
                var ctx = ASContext.Context;
                var features = ctx.Features;
                // for example: 1.0.<complete>, 5e-324.<complete>
                if (char.IsDigit(expression, 0)
                    // for example: -1.<complete>
                    || (expression.Length > 1 && expression[0] == '-' && char.IsDigit(expression, 1))
                    // for example: --1.<complete>
                    || (expression.Length > 2 && expression[0] == '-' && expression[1] == '-' && char.IsDigit(expression, 2)))
                {
                    int p;
                    int pe1;
                    var pe2 = -1;
                    if ((pe1 = expression.IndexOfOrdinal("e-")) != -1 || (pe2 = expression.IndexOfOrdinal("e+")) != -1)
                    {
                        p = expression.IndexOf('.');
                        if (p == -1) p = expression.Length - 1;
                        else if (p < pe1 || p < pe2)
                        {
                            var p2 = expression.IndexOf('.', p + 1);
                            p = p2 != -1 ? p2 : expression.Length - 1;
                        }
                    }
                    else
                    {
                        p = expression.IndexOf('.');
                        if (p == expression.Length - 1) p = -1;
                        else if (p != -1)
                        {
                            // for example: 1.0.<complete>
                            if (char.IsDigit(expression[p + 1]))
                            {
                                var p2 = expression.IndexOf('.', p + 1);
                                p = p2 != -1 ? p2 : expression.Length - 1;
                            }
                            // for example: -1.extensionMethod().<complete>
                            else p = -1;
                        }
                    }
                    if (p != -1)
                    {
                        expression = "Float.#." + expression.Substring(p + 1);
                        return base.EvalExpression(expression, context, inFile, inClass, complete, asFunction, filterVisibility);
                    }
                }
                if (context.SubExpressions != null)
                {
                    var count = context.SubExpressions.Count - 1;
                    // transform #2~.#1~.#0~ to #2~.[].[]
                    for (var i = 0; i <= count; i++)
                    {
                        var subExpression = context.SubExpressions[i];
                        if (subExpression.Length < 2 || subExpression[0] != '[') continue;
                        // for example: [].<complete>, [1 => 2].<complete>
                        if (expression[0] == '#' && i == count)
                        {
                            var type = ctx.ResolveToken(subExpression, inFile);
                            if (type.IsVoid()) break;
                            expression = type.Name + ".#" + expression.Substring(("#" + i + "~").Length);
                            context.SubExpressions.RemoveAt(i);
                            return base.EvalExpression(expression, context, inFile, inClass, complete, asFunction, filterVisibility);
                        }
                        expression = expression.Replace(".#" + i + "~", "." + subExpression);
                    }
                }
                if (expression.Length > 1 && expression[0] is char c && (c == '\'' || c == '"'))
                {
                    var type = ctx.ResolveType(features.stringKey, inFile);
                    // for example: ""|, ''|
                    if (context.SubExpressions == null) expression = type.Name + ".#.";
                    // for example: "".<complete>, ''.<complete>
                    else
                    {
                        var pattern = c + ".#" + (context.SubExpressions.Count - 1) + "~";
                        var startIndex = expression.IndexOfOrdinal(pattern) + pattern.Length;
                        expression = type.Name + ".#" + expression.Substring(startIndex);
                    }
                }
                // for example: ~/pattern/.<complete>
                else if (expression.StartsWithOrdinal("#RegExp")) expression = expression.Replace("#RegExp", "EReg");
                else if (context.SubExpressions != null && context.SubExpressions.Count > 0)
                {
                    var lastIndex = context.SubExpressions.Count - 1;
                    var pattern = "#" + lastIndex + "~";
                    // for example: cast(v, T).<complete>, (v is T).<complete>, (v:T).<complete>, ...
                    if (expression.StartsWithOrdinal(pattern))
                    {
                        var expr = context.SubExpressions[lastIndex];
                        if (context.WordBefore == "cast") expr = "cast" + expr;
                        var type = ctx.ResolveToken(expr, inFile);
                        if (!type.IsVoid()) expression = type.Name + ".#" + expression.Substring(pattern.Length);
                    }
                }
                /**
                 * for example:
                 * macro function foo(v:Expr):Expr {
                 *     return macro {
                 *         $v.<complete>
                 *     }
                 * }
                 */
                if (string.IsNullOrEmpty(context.WordBefore) && context.PositionExpression > 0 &&
                    ASContext.CurSciControl != null && ASContext.CurSciControl.CharAt(context.PositionExpression - 1) == '$')
                {
                    context.PositionExpression -= 1;
                    context.Value = $"${context.Value}";
                }
            }
            return base.EvalExpression(expression, context, inFile, inClass, complete, asFunction, filterVisibility);
        }

        protected override string GetConstructorTooltipText(ClassModel type)
        {
            var inClass = type;
            type.ResolveExtends();
            while (!type.IsVoid())
            {
                var member = type.Members.Search(type.Name, FlagType.Constructor, 0);
                if (member != null)
                {
                    if (member.Name != inClass.Name)
                    {
                        member = (MemberModel) member.Clone();
                        member.Name = inClass.Name;
                        inClass = type;
                    }
                    return MemberTooltipText(member, inClass) + GetToolTipDoc(member);
                }
                type = type.Extends;
            }
            return null;
        }

        protected override string GetCalltipDef(MemberModel member)
        {
            if ((member.Flags & FlagType.ParameterVar) != 0 && FileParser.IsFunctionType(member.Type))
            {
                var tmp = FileParser.FunctionTypeToMemberModel(member.Type, ASContext.Context.Features);
                tmp.Name = member.Name;
                tmp.Flags |= FlagType.Function;
                member = tmp;
            }
            return base.GetCalltipDef(member);
        }

        protected override void GetInstanceMembers(bool autoHide, ASResult expr, ClassModel tmpClass, FlagType mask, int dotIndex, MemberList result)
        {
            if (tmpClass.Flags.HasFlag(FlagType.Abstract))
            {
                if (expr.Member?.Name == "this")
                {
                    var extends = tmpClass.Extends;
                    if (extends.IsVoid())
                    {
                        extends = ASContext.Context.ResolveType(tmpClass.ExtendsType, expr.InFile ?? ASContext.Context.CurrentModel);
                        if (extends.IsVoid()) return;
                    }
                    base.GetInstanceMembers(autoHide, expr, extends, mask, dotIndex, result);
                    return;
                }
                if (!string.IsNullOrEmpty(tmpClass.ExtendsType)
                    // for example: @:enum abstract
                    && tmpClass.MetaDatas is var metaDatas && (metaDatas == null || metaDatas.All(it => it.Name != ":enum"))
                    // for example: abstract Null<T> from T to T
                    && (string.IsNullOrEmpty(tmpClass.Template) || tmpClass.ExtendsType != tmpClass.IndexType))
                {
                    var access = ASContext.Context.TypesAffinity(ASContext.Context.CurrentClass, tmpClass);
                    result.Merge(tmpClass.GetSortedMembersList(), mask, access);
                    if (metaDatas == null) return;
                    var extends = tmpClass.Extends;
                    if (extends.IsVoid())
                    {
                        extends = ASContext.Context.ResolveType(tmpClass.ExtendsType, expr.InFile ?? ASContext.Context.CurrentModel);
                        if (extends.IsVoid()) return;
                    }
                    var @params = mask.HasFlag(FlagType.Static)
                        ? metaDatas.Find(it => it.Name == ":forwardStatics")?.Params
                        : metaDatas.Find(it => it.Name == ":forward")?.Params;
                    if (@params == null)
                    {
                        base.GetInstanceMembers(autoHide, expr, extends, mask, dotIndex, result);
                        return;
                    }
                    var tmp = new MemberList();
                    base.GetInstanceMembers(autoHide, expr, extends, mask, dotIndex, tmp);
                    var names = @params["Default"].Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var param in names)
                    {
                        var member = tmp.Search(param.Trim(), 0, 0);
                        if (member != null) result.Merge(member);
                    }
                    return;
                }
            }
            base.GetInstanceMembers(autoHide, expr, tmpClass, mask, dotIndex, result);
        }

        protected override void FindMemberEx(string token, FileModel inFile, ASResult result, FlagType mask, Visibility access)
        {
            base.FindMemberEx(token, inFile, result, mask, access);
            if (result.Type != null && !result.Type.IsVoid()) return;
            var list = ASContext.Context.GetTopLevelElements();
            if (list == null || list.Count == 0) return;
            foreach (MemberModel it in list)
            {
                if (it.Name != token || !it.Flags.HasFlag(FlagType.Enum)) continue;
                var type = ResolveType(it.Type, inFile);
                result.Type = type;
                result.InClass = type;
                result.IsStatic = false;
                return;
            }
        }

        protected override void FindMemberEx(string token, ClassModel inClass, ASResult result, FlagType mask, Visibility access)
        {
            if (string.IsNullOrEmpty(token)) return;
            // previous member accessed as an array
            if (token.Length > 1 && token[0] == '[' && token[token.Length - 1] == ']' && inClass != null && result.Type != null)
            {
                if ((result.Type.Flags & FlagType.TypeDef) != 0 && result.Type.Extends.IsVoid() && !string.IsNullOrEmpty(result.Type.ExtendsType))
                {
                    /**
                     * for example:
                     * typedef Ints = Array<Int>;
                     * var ints:Ints;
                     * ints[0].<complete>
                     */
                    var type = result.Type;
                    while (!type.IsVoid() && string.IsNullOrEmpty(type.IndexType))
                    {
                        type = ResolveType(type.ExtendsType, ASContext.Context.CurrentModel);
                    }
                    result.Type = type;
                }
                else if (result.Type.IndexType is string indexType && FileParser.IsFunctionType(indexType))
                {
                    result.Member = (MemberModel) result.Member.Clone();
                    FileParser.FunctionTypeToMemberModel(indexType, ASContext.Context.Features, result.Member);
                    result.Member.Name = "item";
                    result.Member.Flags |= FlagType.Function;
                    result.Type = (ClassModel) Context.stubFunctionClass.Clone();
                    result.Type.Parameters = result.Member.Parameters;
                    result.Type.Type = result.Member.Type;
                    return;
                }
            }
            else if (result.Member is MemberModel member && (member.Flags.HasFlag(FlagType.Function)
                     // TODO slavara: temporary solution, because at the moment the function parameters are not converted to the function.
                     || member.Flags.HasFlag(FlagType.ParameterVar) && FileParser.IsFunctionType(member.Type)))
            {
                var returnType = member.Type;
                if (!string.IsNullOrEmpty(member.Template) && result.Context.SubExpressions.Last() is string subExpression && subExpression.Length > 2)
                {
                    var subExpressionPosition = result.Context.SubExpressionPositions.Last();
                    subExpression = subExpression.Substring(1, subExpression.Length - 2);
                    var expressions = new List<ASResult>();
                    var groupCount = 0;
                    for (int i = 0, length = subExpression.Length - 1; i <= length; i++)
                    {
                        var c = subExpression[i];
                        if (c == '[' || c == '(' || c == '{' || c == '<') groupCount++;
                        else if (c == ']' || c == ')' || c == '}' || c == '>') groupCount--;
                        else if (groupCount == 0 && c == ',' || i == length)
                        {
                            if (i == length) i++;
                            var expr = GetExpressionType(ASContext.CurSciControl, subExpressionPosition + i, false, true);
                            if (expr.Type == null) expr.Type = ClassModel.VoidClass;
                            expressions.Add(expr);
                        }
                    }
                    member = (MemberModel) member.Clone();
                    var templates = member.Template.Substring(1, member.Template.Length - 2).Split(',');
                    for (var i = 0; i < templates.Length; i++)
                    {
                        string newType = null;
                        var template = templates[i];
                        // try transform T:{} to T
                        if (template.IndexOf(':') is int p && p != -1) template = template.Substring(0, p);
                        var reTemplateType = new Regex($"\\b{template}\\b");
                        if (member.Parameters is List<MemberModel> parameters)
                        {
                            for (var j = 0; j < parameters.Count && j < expressions.Count; j++)
                            {
                                var parameter = parameters[j];
                                var parameterType = parameter.Type;
                                if (parameterType != template)
                                {
                                    // for example: typedef Null<T> = T, abstract Null<T> from T to T
                                    if (reTemplateType.IsMatch(parameterType)
                                        && ResolveType(parameterType, result.InFile) is ClassModel expr && !expr.IsVoid()
                                        && (expr.Flags & (FlagType.Abstract | FlagType.TypeDef)) != 0)
                                    {
                                    }
                                    else continue;
                                }
                                if (string.IsNullOrEmpty(newType))
                                {
                                    var expr = expressions[j];
                                    if (expr.Type.IsVoid()) break;
                                    newType = expr.Type.Name;
                                }
                                if (string.IsNullOrEmpty(newType)) continue;
                                parameters[j] = (MemberModel) parameter.Clone();
                                parameters[j].Type = reTemplateType.Replace(parameterType, newType);
                            }
                        }
                        if (string.IsNullOrEmpty(newType)) continue;
                        if (!string.IsNullOrEmpty(returnType) && reTemplateType.IsMatch(returnType))
                        {
                            returnType = reTemplateType.Replace(returnType, newType);
                            member.Type = returnType;
                        }
                        templates[i] = newType;
                    }
                    member.Template = $"<{string.Join(", ", templates)}>";
                    result.Member = member;
                    result.Type = ResolveType(returnType, ASContext.Context.CurrentModel);
                    result.InClass = result.Type;
                }
                // previous member called as a method
                else if (token[0] == '#' && FileParser.IsFunctionType(returnType)
                    // for example: (foo():Void->(Void->String))()
                    && result.Context.SubExpressions is List<string> l && l.Count > 1)
                {
                    var type = (ClassModel) Context.stubFunctionClass.Clone();
                    FileParser.FunctionTypeToMemberModel(returnType, ASContext.Context.Features, type);
                    result.Member = new MemberModel
                    {
                        Name = "callback",
                        Flags = FlagType.Variable | FlagType.Function,
                        Parameters = type.Parameters,
                        Type = type.Type,
                    };
                    result.Type = type;
                    return;
                }
            }
            base.FindMemberEx(token, inClass, result, mask, access);
            /**
             * for example:
             * class Some<T:String> {
             *     var v:T;
             *     function test() {
             *         v.<complete>
             *     }
             * }
             */
            if (result.Member?.Type != null && (result.Type == null || result.Type.IsVoid()))
            {
                var clone = (MemberModel)result.Member.Clone();
                if (TryInferGenericType(clone) is ClassModel type && !type.IsVoid())
                {
                    result.Member = clone;
                    result.Type = type;
                    return;
                }
            }
        }

        protected override Visibility TypesAffinity(ASExpr context, ClassModel inClass, ClassModel withClass)
        {
            var result = base.TypesAffinity(context, inClass, withClass);
            if (context != null
                && ASContext.CurSciControl is ScintillaControl sci
                && context.WordBefore == "privateAccess" && context.WordBeforePosition is int p
                && sci.CharAt(p - 2) == '@' && sci.CharAt(p - 1) == ':') result |= Visibility.Private;
            return result;
        }

        public override MemberModel FunctionTypeToMemberModel(string type, FileModel inFile)
        {
            var voidKey = ASContext.Context.Features.voidKey;
            if (type == "Function")
            {
                var paramType = ASContext.Context.ResolveType(type, inFile);
                if (paramType.InFile.Package == "haxe" && paramType.InFile.Module == "Constraints")
                    return new MemberModel {Type = voidKey};
            }
            return FileParser.FunctionTypeToMemberModel(type, ASContext.Context.Features);
        }

        /// <param name="sci">Scintilla control</param>
        /// <param name="position">Current cursor position</param>
        /// <param name="autoHide">Don't keep the list open if the word does not match</param>
        /// <returns>Auto-completion has been handled</returns>
        bool HandleSwitchCaseCompletion(ScintillaControl sci, int position, bool autoHide)
        {
            var ctx = ASContext.Context;
            var member = ctx.CurrentMember ?? ctx.CurrentClass;
            var endPosition = member != null ? sci.PositionFromLine(member.LineFrom) : 0;
            var braCount = 0;
            while (endPosition < position)
            {
                if (sci.PositionIsOnComment(position) || sci.PositionIsInString(position))
                {
                    position--;
                    continue;
                }
                var c = (char)sci.CharAt(position);
                if (c == '}') braCount++;
                else if (c == '{')
                {
                    braCount--;
                    if (braCount < 0)
                    {
                        var expr = GetExpressionType(sci, position, false, true);
                        var list = GetCompletionList(expr);
                        if (list != null)
                        {
                            if (expr.Member.Type.StartsWithOrdinal("Null<")) list.Insert(0, new DeclarationItem("null"));
                            list.Add(new DeclarationItem("_"));
                            CompletionList.Show(list, autoHide);
                            return true;
                        }
                        break;
                    }
                }
                position--;
            }
            return false;
            // Utils
            List<ICompletionListItem> GetCompletionList(ASResult expr)
            {
                if (expr.Member is MemberModel m && m.Type is string typeName)
                {
                    typeName = CleanNullableType(typeName);
                    if (typeName == ctx.Features.booleanKey)
                    {
                        return new List<ICompletionListItem>
                        {
                            new DeclarationItem("true"),
                            new DeclarationItem("false"),
                        };
                    }
                    var type = ctx.ResolveType(typeName, ctx.CurrentModel);
                    if (type.Members.Count == 0) return null;
                    if ((type.Flags.HasFlag(FlagType.Abstract) && type.MetaDatas != null && type.MetaDatas.Any(tag => tag.Name == ":enum")))
                    {
                        return type.Members.Items.Select(it => new MemberItem(it)).ToList<ICompletionListItem>();
                    }
                    if (type.Flags.HasFlag(FlagType.Enum))
                    {
                        return type.Members.Items.Select(it =>
                        {
                            var pattern = it.Name;
                            if (it.Parameters != null)
                            {
                                pattern += "(";
                                for (var j = 0; j < it.Parameters.Count; j++)
                                {
                                    if (j > 0) pattern += ", ";
                                    pattern += it.Parameters[j].Name.TrimStart('?');
                                }
                                pattern += ")";
                            }
                            return new DeclarationItem(pattern);
                        }).ToList<ICompletionListItem>();
                    }
                }
                return null;
                // Utils
                string CleanNullableType(string s)
                {
                    var startIndex = s.IndexOfOrdinal("Null<");
                    if (startIndex == -1) return s;
                    startIndex += 5;
                    while (s.IndexOfOrdinal("Null<", startIndex) is int p && p != -1)
                    {
                        startIndex = p + 5;
                    }
                    return s.Substring(startIndex, s.Length - (startIndex + startIndex / 5));
                }
            }
        }

        static void GenerateAnonymousFunction(ScintillaControl sci, MemberModel member, string template)
        {
            string body = null;
            switch (ASContext.CommonSettings.GeneratedMemberDefaultBodyStyle)
            {
                case GeneratedMemberBodyStyle.ReturnDefaultValue:
                    var ctx = ASContext.Context;
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
            template = TemplateUtils.ToDeclarationWithModifiersString(member, template);
            template = TemplateUtils.ReplaceTemplateVariable(template, "Body", body);
            sci.SelectWord();
            ASGenerator.InsertCode(sci.CurrentPos, template);
        }
    }

    class AnonymousFunctionGeneratorItem : ICompletionListItem
    {
        readonly Action action;

        public AnonymousFunctionGeneratorItem(string label, Action action)
        {
            Label = label;
            this.action = action;
        }

        public string Label { get; }
        public string Description => TextHelper.GetString("ASCompletion.Info.GeneratorTemplate");
        public Bitmap Icon => (Bitmap)ASContext.Panel.GetIcon(34);

        public string Value
        {
            get
            {
                action();
                return null;
            }
        }
    }
}