﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ASCompletion.Completion;
using ASCompletion.Context;
using ASCompletion.Model;
using HaXeContext.Model;
using PluginCore;
using PluginCore.Controls;
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
                    break;
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

        static bool HandleAssignCompletion(ScintillaControl sci, int position, bool autoHide)
        {
            // for example: v = <complete>, v != <complete>, v == <complete>
            if ((char) sci.CharAt(position) is char c && (c == ' ' || c == '!' || c == '='))
            {
                var expr = GetExpressionType(sci, position, false, true);
                if (expr.Type != null && expr.Type.Flags.HasFlag(FlagType.Abstract)
                    && expr.Type.Members is MemberList members && members.Count > 0
                    && expr.Type.MetaDatas != null && expr.Type.MetaDatas.Any(it => it.Name == ":enum"))
                {
                    var list = new List<ICompletionListItem>();
                    foreach (MemberModel member in members)
                    {
                        if (member.Flags.HasFlag(FlagType.Variable) && !member.Access.HasFlag(Visibility.Private))
                        {
                            list.Add(new MemberItem(member));
                        }
                    }
                    if (list.Count <= 0) return false;
                    CompletionList.Show(list, autoHide);
                    return true;
                }
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
                        InferParameterVarType(item);
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
            var ctx = ASContext.Context;
            var line = sci.GetLine(var.LineFrom);
            var m = Regex.Match(line, "\\s*for\\s*\\(\\s*" + var.Name + "\\s*in\\s*");
            if (!m.Success)
            {
                base.InferVariableType(sci, local, var);
                if (string.IsNullOrEmpty(var.Type)
                    && (var.Flags.HasFlag(FlagType.Variable)
                        || var.Flags.HasFlag(FlagType.Getter)
                        || var.Flags.HasFlag(FlagType.Setter)))
                {
                    var.Type = ctx.ResolveType(ctx.Features.dynamicKey, null).Name;
                }
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
                var.Type = expr.Type.QualifiedName;
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
                        type = ASContext.Context.ResolveType(type.ExtendsType, result.InFile);
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
                if (member.Template is string template && result.Context.SubExpressions.Last() is string subExpression && subExpression.Length > 2)
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
                    var templates = template.Substring(1, template.Length - 2).Split(',');
                    for (var i = 0; i < templates.Length; i++)
                    {
                        string newType = null;
                        var templateType = templates[i];
                        if (member.Parameters is List<MemberModel> parameters)
                        {
                            for (var j = 0; j < parameters.Count && j < expressions.Count; j++)
                            {
                                var parameter = parameters[j];
                                if (parameter.Type != templateType) continue;
                                if (string.IsNullOrEmpty(newType))
                                {
                                    var expr = expressions[j];
                                    if (expr.Type.IsVoid()) break;
                                    newType = expr.Type.Name;
                                }
                                if (string.IsNullOrEmpty(newType)) continue;
                                parameters[j] = (MemberModel) parameter.Clone();
                                parameters[j].Type = newType;
                            }
                        }
                        if (!string.IsNullOrEmpty(newType))
                        {
                            var r = new Regex($"\\b{templateType}\\b");
                            if (!string.IsNullOrEmpty(returnType) && r.IsMatch(returnType))
                            {
                                returnType = r.Replace(returnType, newType);
                                member.Type = returnType;
                            }
                            templates[i] = newType;
                        }
                    }
                    member.Template = $"<{string.Join(", ", templates)}>";
                    result.Member = member;
                }
                // previous member called as a method
                if (token[0] == '#' && FileParser.IsFunctionType(returnType)
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
    }
} 