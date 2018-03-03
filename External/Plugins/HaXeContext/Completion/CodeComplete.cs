using System;
using System.Linq;
using System.Text.RegularExpressions;
using ASCompletion.Completion;
using ASCompletion.Context;
using ASCompletion.Model;
using ScintillaNet;

namespace HaXeContext.Completion
{
    class CodeComplete : ASComplete
    {
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
                char current = (char)sci.CharAt(position);

                for (int i = position - 1; i >= 0; i--)
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
        protected override bool HandleWhiteSpaceCompletion(ScintillaControl sci, int position, string wordLeft, bool autoHide)
        {
            var currentClass = ASContext.Context.CurrentClass;
            if (currentClass.Flags.HasFlag(FlagType.Abstract))
            {
                switch (wordLeft)
                {
                    case "from":
                    case "to":
                        return PositionIsBeforeBody(sci, position, currentClass) && HandleNewCompletion(sci, string.Empty, autoHide, wordLeft);
                }
            }
            return base.HandleWhiteSpaceCompletion(sci, position, wordLeft, autoHide);
        }

        /// <inheritdoc />
        protected override bool ResolveFunction(ScintillaControl sci, int position, ASResult expr, bool autoHide)
        {
            if (expr.Member == null)
            {
                var type = expr.Type;
                if (type != null)
                {
                    var originConstructor = ASContext.GetLastStringToken(type.Name, ".");
                    while (!type.IsVoid())
                    {
                        var constructor = ASContext.GetLastStringToken(type.Name, ".");
                        var member = type.Members.Search(constructor, FlagType.Constructor, 0);
                        if (member != null)
                        {
                            if (originConstructor != member.Name)
                            {
                                member = (MemberModel) member.Clone();
                                member.Name = originConstructor;
                            }
                            expr.Member = member;
                            expr.Context.Position = position;
                            FunctionContextResolved(sci, expr.Context, expr.Member, expr.RelClass, false);
                            return true;
                        }
                        if (type.Flags.HasFlag(FlagType.Abstract)) return false;
                        type.ResolveExtends();
                        type = type.Extends;
                    }
                    return false;
                }
            }
            return base.ResolveFunction(sci, position, expr, autoHide);
        }

        protected override void InferVariableType(ScintillaControl sci, ASExpr local, MemberModel var)
        {
            var line = sci.GetLine(var.LineFrom);
            var m = Regex.Match(line, "\\s*for\\s*\\(\\s*" + var.Name + "\\s*in\\s*");
            if (m.Success)
            {
                var rvalueStart = sci.PositionFromLine(var.LineFrom) + m.Index + m.Length;
                var methodEndPosition = sci.LineEndPosition(ASContext.Context.CurrentMember.LineTo);
                var parCount = 0;
                for (var i = rvalueStart; i < methodEndPosition; i++)
                {
                    if (sci.PositionIsOnComment(i)) continue;
                    var c = (char)sci.CharAt(i);
                    if (c <= ' ') continue;
                    if (c == '(') parCount++;
                    else if (c == ')')
                    {
                        parCount--;
                        if (parCount >= 0) continue;
                        var expr = GetExpressionType(sci, i, false, true);
                        if (expr.Type != null)
                        {
                            var members = expr.Type.Members;
                            var member = members.Search("iterator", 0, 0);
                            if (member == null && members.Search("hasNext", 0, 0) != null)
                                member = members.Search("next", 0, 0);
                            if (member?.Type != null)
                            {
                                var type = ASContext.Context.ResolveType(member.Type, ASContext.Context.CurrentModel);
                                var iteratorIndexType = type.IndexType;
                                var.Type = iteratorIndexType;
                                if (expr.Type.IndexType.Contains(','))
                                {
                                    var t = expr.Type;
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
                                            t.ResolveExtends();
                                            t = t.Extends;
                                        }
                                    }
                                }
                            }
                            if (var.Type == null)
                            {
                                var type = ASContext.Context.ResolveType(ASContext.Context.Features.dynamicKey, null);
                                var.Type = type.QualifiedName;
                            }
                            var.Flags |= FlagType.Inferred;
                        }
                        return;
                    }
                    else if (c == '.' && sci.CharAt(i + 1) == '.' && sci.CharAt(i + 2) == '.')
                    {
                        var type = ASContext.Context.ResolveType("Int", null);
                        var.Type = type.QualifiedName;
                        var.Flags |= FlagType.Inferred;
                        return;
                    }
                }
            }
            else base.InferVariableType(sci, local, var);
        }

        protected override void InferVariableType(ScintillaControl sci, string declarationLine, int rvalueStart, ASExpr local, MemberModel var)
        {
            var word = sci.GetWordRight(rvalueStart, true);
            if (word == "untyped")
            {
                var type = ASContext.Context.ResolveType(ASContext.Context.Features.dynamicKey, null);
                var.Type = type.QualifiedName;
                var.Flags |= FlagType.Inferred;
                return;
            }
            var methodEndPosition = sci.LineEndPosition(ASContext.Context.CurrentMember.LineTo);
            var rvalueEnd = ExpressionEndPosition(sci, rvalueStart, sci.LineEndPosition(var.LineTo));
            for (var i = rvalueEnd; i < methodEndPosition; i++)
            {
                if(sci.PositionIsOnComment(i)) continue;
                var c = (char) sci.CharAt(i);
                if (c <= ' ') continue;
                if (c == ';') break;
                if (c == '.') rvalueEnd = ExpressionEndPosition(sci, i + 1, methodEndPosition);
            }
            var expr = GetExpressionType(sci, rvalueEnd, false, true);
            if (expr.Type != null)
            {
                var.Type = expr.Type.QualifiedName;
                var.Flags |= FlagType.Inferred;
                return;
            }
            if (expr.Member != null)
            {
                var.Type = expr.Member.Type;
                var.Flags |= FlagType.Inferred;
                return;
            }
            base.InferVariableType(sci, declarationLine, rvalueStart, local, var);
        }
    }
}
