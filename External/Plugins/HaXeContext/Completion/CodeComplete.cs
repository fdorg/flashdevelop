using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ASCompletion.Completion;
using ASCompletion.Context;
using ASCompletion.Model;
using HaXeContext.Generators;
using HaXeContext.Model;
using PluginCore;
using PluginCore.Controls;
using PluginCore.Helpers;
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

        /// <inheritdoc />
        protected override bool IsAvailableForToolTip(ScintillaControl sci, int position)
        {
            return base.IsAvailableForToolTip(sci, position)
                   || (sci.GetWordFromPosition(position) is { } word && (word == "cast" || word == "new"));
        }

        /// <summary>
        /// Whether the character at the position is inside of the
        /// brackets of haxe metadata (@:allow(path) etc)
        /// </summary>
        protected override bool IsMetadataArgument(ScintillaControl sci, int position)
        {
            if (ASContext.Context.CurrentMember != null) return false;
            var next = (char)sci.CharAt(position);
            var openingBracket = false;
            for (var i = position; i > 0; i--)
            {
                var c = next;
                next = (char)sci.CharAt(i);
                switch (c)
                {
                    case ')':
                    case '}':
                    case ';':
                        return false;
                    case '(':
                        openingBracket = true;
                        break;
                }
                if (openingBracket && c == ':' && next == '@')
                    return true;
            }
            return false;
        }

        public override bool IsRegexStyle(ScintillaControl sci, int position)
        {
            return base.IsRegexStyle(sci, position)
                   || (sci.BaseStyleAt(position) == 10 && sci.CharAt(position) == '~' && sci.CharAt(position + 1) == '/');
        }

        /// <summary>
        /// Returns whether or not position is inside of an expression block in String interpolation ('${expr}')
        /// </summary>
        public override bool IsStringInterpolationStyle(ScintillaControl sci, int position)
        {
            if (!ASContext.Context.Features.hasStringInterpolation) return false;
            var stringChar = sci.GetStringType(position - 1);
            if (!ASContext.Context.Features.stringInterpolationQuotes.Contains(stringChar)) return false;
            var hadDot = false;
            var current = (char)sci.CharAt(position);
            for (var i = position - 1; i >= 0; i--)
            {
                var next = current;
                current = (char)sci.CharAt(i);
                if (current == stringChar)
                {
                    if (!IsEscapedCharacter(sci, i)) break;
                }
                else if (current == '.' || current == '[' || current == '(') hadDot = true;
                else if (current == '$')
                {
                    if ((!hadDot || next == '{') && !IsEscapedCharacter(sci, i, '$')) return true;
                    hadDot = true;
                }
                else if (current == '}')
                {
                    i = sci.BraceMatch(i);
                    current = (char)sci.CharAt(i);
                    if (i > 0 && current == '{' && sci.CharAt(i - 1) == '$') break;
                }
            }
            return false;
        }

        /// <inheritdoc />
        protected override bool OnChar(ScintillaControl sci, int value, char prevValue, bool autoHide)
        {
            var currentPos = sci.CurrentPos;
            switch (value)
            {
                case ':':
                    if (prevValue == '@') return HandleMetadataCompletion(autoHide);
                    break;
                case '>':
                    // for example: SomeType-><complete>
                    if (prevValue == '-' && IsType(currentPos - 2)) return HandleNewCompletion(sci, string.Empty, autoHide, string.Empty);
                    break;
                case '(':
                    // for example: SomeType->(<complete>
                    if (prevValue == '>' && (currentPos - 3) is { } p && p > 0 && (char)sci.CharAt(p) == '-' && IsType(p))
                        return HandleNewCompletion(sci, string.Empty, autoHide, string.Empty);
                    // for example: someFunction(<complete>
                    if (HandleFunctionCompletion(sci, currentPos, autoHide)) return false;
                    // for example: @:forward(<complete> or @:forwardStatics(<complete>
                    return HandleMetadataForwardCompletion(sci, autoHide);
                default:
                    // for example: https://haxe.org/manual/macro-reification-expression.html
                    if (ASContext.Context.CurrentMember is { } member
                        && member.Flags.HasFlag(FlagType.Function) && member.Flags.HasFlag((FlagType) HaxeFlagType.Macro))
                    {
                        // for example: $a<complete>
                        if (value != '$' && !string.IsNullOrEmpty(GetWordLeft(sci, ref currentPos))) value = (char) sci.CharAt(currentPos);
                        if (value == '$')
                        {
                            // for example: var $<complete>
                            if (GetWordLeft(sci, ref currentPos) == "var") return false;
                            // TODO slavara: handle object.$<complete>
                            return HandleExpressionReificationCompletion(sci, autoHide);
                        }
                    }
                    break;
            }
            return false;
            // Utils
            bool IsType(int position) => GetExpressionType(sci, position, false, true).Type is { } t && !t.IsVoid();
        }

        protected override bool HandleNewCompletion(ScintillaControl sci, string tail, bool autoHide, string keyword, List<ICompletionListItem> list)
        {
            if (keyword == "new")
            {
                list = list
                    .Where(it =>
                    {
                        if (it is MemberItem item && item.Member is { } member)
                        {
                            var flags = member.Flags;
                            if ((flags & FlagType.Interface) != 0 || (flags & FlagType.Enum) != 0) return false;
                            var @class = member as ClassModel ?? ResolveType(member.Type, ASContext.Context.CurrentModel);
                            if (@class is null) return false;
                            var recursive = (@class.Flags & FlagType.Abstract) == 0;
                            return @class.ContainsMember(FlagType.Access | FlagType.Function | FlagType.Constructor, recursive)
                                   || @class.ContainsMember(FlagType.Function | FlagType.Constructor, recursive);
                        }
                        return true;
                    })
                    .ToList();
            }
            return base.HandleNewCompletion(sci, tail, autoHide, keyword, list);
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
                        while (GetExpressionType(sci, pos + 1, false, true) is { } expr && expr.Type != null && !expr.Type.IsVoid())
                        {
                            if (expr.Context.WordBefore == "case") return HandleSwitchCaseCompletion(sci, pos, autoHide);
                            if (expr.Context.Separator is { } separator && (separator == "|" || separator == ","))
                                pos = expr.Context.SeparatorPosition - 1;
                            else break;
                        }
                    }
                    // for example: someFunction(<complete>
                    if (HandleFunctionCompletion(sci, position, autoHide)) return true;
                    // for example: @:forward(methodName, <complete> or @:forwardStatics(methodName, <complete>
                    return HandleMetadataForwardCompletion(sci, autoHide);
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

        bool HandleMetadataForwardCompletion(ScintillaControl sci, bool autoHide)
        {
            var ctx = ASContext.Context;
            if (!ctx.CurrentClass.IsVoid()) return false;
            var currentLine = sci.CurrentLine;
            var line = sci.GetLine(currentLine);
            // for example: @:forward() <complete>
            if (line.LastIndexOf(')') is { } p && (p != -1 && (sci.PositionFromLine(currentLine) + p) < sci.CurrentPos)) return false;
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
            if (ctx.CurrentModel.Classes.Find(it => it.LineFrom > currentLine) is { } @class && @class.Flags.HasFlag(FlagType.Abstract))
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
                    if (names.Length != 0) list.Items.RemoveAll(it => names.Contains(it.Name));
                }
                if (list.Count > 0) CompletionList.Show(list.Select(it => new MemberItem(it)).ToList<ICompletionListItem>(), autoHide);
                return true;
            }
            return false;
        }

        bool HandleFunctionCompletion(ScintillaControl sci, int position, bool autoHide)
        {
            var pos = position - 1;
            var paramIndex = FindParameterIndex(sci, ref pos);
            if (pos != -1 && ResolveFunction(sci, pos, autoHide)
                && calltipMember?.Parameters is { } parameters && paramIndex < parameters.Count
                && parameters[paramIndex] is { } parameter && parameter.Type is { } parameterType)
            {
                ClassModel type;
                if (FileParser.IsFunctionType(parameterType))
                {
                    type = FileParser.FunctionTypeToMemberModel<ClassModel>(parameterType, ASContext.Context.Features);
                    type.Flags |= FlagType.Function;
                }   
                else type = ResolveType(parameterType, calltipMember.InFile);
                if (!type.IsVoid())
                {
                    HandleAssignCompletion(sci, autoHide, ' ', type, new ASResult {Context = new ASExpr(), Member = type, Type = type});
                    return true;
                }
            }
            return false;
        }

        /// <param name="sci">Scintilla control</param>
        /// <param name="position">Current cursor position</param>
        /// <param name="autoHide">Don't keep the list open if the word does not match</param>
        /// <returns>Auto-completion has been handled</returns>
        bool HandleAssignCompletion(ScintillaControl sci, int position, bool autoHide)
        {
            var expr = GetExpressionType(sci, position, false, true);
            return expr.Type is { } type && HandleAssignCompletion(sci, autoHide, (char) sci.CharAt(position), type, expr);
        }

        bool HandleAssignCompletion(ScintillaControl sci, bool autoHide, char c, ClassModel type, ASResult expr)
        {
            var ctx = ASContext.Context;
            // for example: function(v:Bool = <complete>, var v:Bool = <complete>, v != <complete>, v == <complete>
            if ((c == ' ' || c == '!' || c == '=') && type.Name == ctx.Features.booleanKey)
            {
                var word = sci.GetWordFromPosition(sci.CurrentPos);
                if (string.IsNullOrEmpty(word) || "false".StartsWithOrdinal(word))
                    completionHistory[ctx.CurrentClass.QualifiedName] = "false";
                return HandleDotCompletion(sci, autoHide, null, (a, b) =>
                {
                    var aLabel = (a as TemplateItem)?.Label;
                    var bLabel = (b as TemplateItem)?.Label;
                    if (IsBool(aLabel) && IsBool(bLabel))
                    {
                        if (aLabel == "false") return -1;
                        return 1;
                    }
                    if (IsBool(aLabel)) return -1;
                    if (IsBool(bLabel)) return 1;
                    return 0;
                    // Utils
                    static bool IsBool(string s) => s == "true" || s == "false";
                });
            }
            // for example: function(v:Type = <complete>
            if (expr.Context.ContextFunction != null && expr.Context.BeforeBody && !IsEnum(type))
            {
                if (expr.Context.Separator != "->" && ctx.GetDefaultValue(type.Name) is { } v && v != "null") return false;
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
                        if (expr.Type is null) return false;
                        functionType = expr.Type.Name + "->" + functionType;
                    }
                    member = FileParser.FunctionTypeToMemberModel<MemberModel>(functionType, ctx.Features);
                }
                if (member is null) return false;
                var functionName = "function() {}";
                var list = new List<ICompletionListItem> {new AnonymousFunctionGeneratorItem(functionName, () => CodeGenerator.GenerateAnonymousFunction(sci, member, TemplateUtils.GetTemplate("AnonymousFunction")))};
                if (ctx is Context context && context.GetCurrentSDKVersion() >= "4.0.0")
                {
                    functionName = "() -> {}";
                    list.Insert(0, new AnonymousFunctionGeneratorItem(functionName, () => CodeGenerator.GenerateAnonymousFunction(sci, member, TemplateUtils.GetTemplate("AnonymousFunction.Haxe4"))));
                }
                var word = sci.GetWordFromPosition(sci.CurrentPos);
                if (string.IsNullOrEmpty(word) || functionName.StartsWithOrdinal(word)) completionHistory[ctx.CurrentClass.QualifiedName] = functionName;
                return HandleDotCompletion(sci, autoHide, list, null);
            }
            // for example: v = <complete>, v != <complete>, v == <complete>
            if (c == ' ' || c == '!' || c == '=')
            {
                if (IsEnum(type))
                    return HandleDotCompletion(sci, autoHide, null, (a, b) =>
                    {
                        var aMember = (a as MemberItem)?.Member;
                        var bMember = (b as MemberItem)?.Member;
                        var aType = aMember?.Type;
                        var bType = bMember?.Type;
                        if (aType != null && aType == type.Name && IsEnumValue(aMember.Flags)
                            && bType == type.Name && IsEnumValue(bMember.Flags))
                        {
                            return aMember.Name.CompareTo(bMember.Name);
                        }

                        if (aType == type.Name && IsEnumValue(aMember.Flags)) return -1;
                        if (bType == type.Name && IsEnumValue(bMember.Flags)) return 1;
                        return 0;

                        // Utils
                        static bool IsEnumValue(FlagType flags) => (flags & FlagType.Static) != 0 && (flags & FlagType.Variable) != 0;
                    });
                var orders = new Dictionary<string, int>
                {
                    {"null", 0}
                };
                List<ICompletionListItem> list = null;
                // for example: var v:Typedef = <complete>
                if (IsTypedef(type, ref type))
                {
                    orders.Add("{}", 1);
                    list = new List<ICompletionListItem>
                    {
                        new ObjectInitializerGeneratorItem("{}", $"Creates a new {type.Name} and initializes it with the specified name and value property pairs.", () => GenerateObjectInitializer(sci, type))
                    };
                }
                // for example: var v:Array<TItem> = <complete>
                else if (type.Name.StartsWithOrdinal("Array<"))
                {
                    orders.Add("[]", 1);
                    list = new List<ICompletionListItem>
                    {
                        new ObjectInitializerGeneratorItem("[]", "Initializes a new array with the specified elements (a0, and so on)", () => GenerateObjectInitializer(sci, "[$(EntryPoint)]"))
                    };
                }
                // for example: var v:Map<TKey, TValue> = <complete>
                else if (type.Name.StartsWithOrdinal("Map<") || type.Name.StartsWith("IMap<"))
                {
                    orders.Add("[=>]", 1);
                    list = new List<ICompletionListItem>
                    {
                        new ObjectInitializerGeneratorItem("[=>]", "Creates a new map and initializes it with the specified name and value property pairs.", () => GenerateObjectInitializer(sci, "[$(EntryPoint)]"))
                    };
                }
                // for example: var v:Dynamic = <complete>
                else if (type.Name.StartsWithOrdinal("Dynamic"))
                {
                    orders.Add("{}", 1);
                    list = new List<ICompletionListItem>
                    {
                        new ObjectInitializerGeneratorItem("{}", "Creates a new dynamic object and initializes it with the specified name and value property pairs.", () => GenerateObjectInitializer(sci, "{$(EntryPoint)}"))
                    };
                }
                // for example: var v:Constructible = <complete>
                if (type.Flags.HasFlag(FlagType.Class))
                {
                    var extends = type;
                    if (extends.IsVoid()) type.ResolveExtends();
                    var access = ctx.TypesAffinity(type, ctx.CurrentClass);
                    MemberModel member = null;
                    while (!extends.IsVoid())
                    {
                        member = extends.Members.Search(extends.Constructor, FlagType.Constructor, access);
                        if (member != null) break;
                        extends = extends.Extends;
                    }
                    if (member != null)
                    {
                        var label = string.IsNullOrEmpty(type.IndexType) 
                            ? $"new {type.Constructor}()"
                            : $"new {type.Constructor}<{type.IndexType}>()";
                        orders.Add(label, 2);
                        if (list is null) list = new List<ICompletionListItem>();
                        list.Add(new ObjectInitializerGeneratorItem(label, $"Creates a new {type.Constructor}", () => GenerateObjectInitializer(sci , $"{label}$(EntryPoint)")));
                    }
                }
                if (ctx.GetDefaultValue(type.Name) == "null")
                {
                    var word = sci.GetWordFromPosition(sci.CurrentPos);
                    if (string.IsNullOrEmpty(word) || "null".StartsWithOrdinal(word)) completionHistory[ctx.CurrentClass.QualifiedName] = "null";
                    return HandleDotCompletion(sci, autoHide, list, (a, b) =>
                    {
                        var aLabel = (a as TemplateItem)?.Label ?? (a as ObjectInitializerGeneratorItem)?.Label;
                        var bLabel = (b as TemplateItem)?.Label ?? (b as ObjectInitializerGeneratorItem)?.Label;
                        if (aLabel != null && bLabel != null && orders.ContainsKey(aLabel) && orders.ContainsKey(bLabel))
                            return orders[aLabel].CompareTo(orders[bLabel]);
                        if (aLabel != null && orders.ContainsKey(aLabel)) return -1;
                        if (bLabel != null && orders.ContainsKey(bLabel)) return 1;
                        return 0;
                    });
                }
                return HandleDotCompletion(sci, autoHide, null, null);
            }
            return false;
            // Utils
            static bool IsEnum(ClassModel t) => t.Flags.HasFlag(FlagType.Enum)
                                         || (t.Flags.HasFlag(FlagType.Abstract) && !t.Members.IsNullOrEmpty()
                                             && t.MetaDatas != null && t.MetaDatas.Any(it => it.Name == ":enum"));

            static bool IsFunction(MemberModel m) => m != null && m.Flags.HasFlag(FlagType.Function);

            static bool IsTypedef(ClassModel t, ref ClassModel realType)
            {
                if (t.Flags.HasFlag(FlagType.TypeDef))
                {
                    // for example: typedef Typedef = {}
                    if (string.IsNullOrEmpty(t.ExtendsType)) return true;
                    /**
                     * for example:
                     * typedef Typedef = {
                     *     > Type1,
                     *     > Type2,
                     * }
                     */
                    if (t.ExtendsTypes != null) return true;
                    t.ResolveExtends();
                    var extends = t.Extends;
                    while (!extends.IsVoid())
                    {
                        if (!extends.Flags.HasFlag(FlagType.TypeDef))
                        {
                            realType = extends;
                            if (!string.IsNullOrEmpty(t.Extends.IndexType))
                            {
                                realType = (ClassModel) realType.Clone();
                                realType.IndexType = t.Extends.IndexType;
                            }
                            return false;
                        }
                        extends = extends.Extends;
                    }
                    return true;
                }
                realType = t;
                return false;
            }
        }

        bool HandleExpressionReificationCompletion(ScintillaControl sci, bool autoHide)
        {
            var list = new List<ICompletionListItem>
            {
                new ExpressionReificationGeneratorItem("", "Expr", "Expr", 
                    "${} : Expr -> Expr This can be used to compose expressions. The expression within the delimiting { } is executed, with its value being used in place.",
                    () => GenerateExpressionReification(sci, "")
                ),
                new ExpressionReificationGeneratorItem("e", "Expr", "Expr",
                    "$e{} : Expr -> Expr This can be used to compose expressions. The expression within the delimiting { } is executed, with its value being used in place.",
                    () => GenerateExpressionReification(sci, "e")
                ),
                new ExpressionReificationGeneratorItem("a", "Array<Expr>", "Array<Expr>",
                    "$a{} : Array<Expr> -> Array<Expr> or Array<Expr> -> Expr If used in a place where an Array<Expr> is expected (e.g. call arguments, block elements), $a{} treats its value as that array. Otherwise it generates an array declaration.",
                    () => GenerateExpressionReification(sci, "a")
                ),
                new ExpressionReificationGeneratorItem("b", "Array<Expr>", "Expr",
                    "$b{} : Array<Expr> -> Expr Generates a block expression from the given expression array.",
                    () => GenerateExpressionReification(sci, "b")
                ),
                new ExpressionReificationGeneratorItem("i", "String", "Expr",
                    "$i{} : String -> Expr Generates an identifier from the given string.",
                    () => GenerateExpressionReification(sci, "i")
                ),
                new ExpressionReificationGeneratorItem("p", "Array<String>", "Expr",
                    "$p{} : Array<String> -> Expr Generates a field expression from the given string array.",
                    () => GenerateExpressionReification(sci, "p")
                ),
                new ExpressionReificationGeneratorItem("v", "Dynamic", "Expr",
                    "$v{} : Dynamic -> Expr Generates an expression depending on the type of its argument. This is only guaranteed to work for basic types and enum instances.",
                    () => GenerateExpressionReification(sci, "v")
                ),
            };
            return HandleDotCompletion(sci, autoHide, list, null);
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
                if (string.IsNullOrEmpty(item.Type) && !string.IsNullOrEmpty(item.Value))
                {
                    var expr = EvalExpression(item.Value, new ASExpr(), model, ASContext.Context.CurrentClass, true, false, false);
                    if (expr.Type != null && !expr.Type.IsVoid()) item.Type = expr.Type.Name;
                }
                var type = item.Type;
                if (string.IsNullOrEmpty(type)) type = "Dynamic";
                var name = item.Name;
                if (name.StartsWith('?'))
                {
                    name = name.Substring(1);
                    if (!type.StartsWithOrdinal("Null<")) type = $"Null<{type}>";
                }
                item = (MemberModel) item.Clone();
                item.Name = name;
                item.Type = type;
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
            if ((member != null && expr.Path != "super") || type is null)
            {
                // for example: cast(<complete>
                if (expr.Context.Value == "cast")
                {
                    FunctionContextResolved(sci, expr.Context, Context.StubSafeCastFunction, ClassModel.VoidClass, false);
                    return true;
                }
                return base.ResolveFunction(sci, position, expr, autoHide);
            }
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

        /// <inheritdoc />
        protected override void InferType(ScintillaControl sci, ASExpr local, MemberModel member)
        {
            /**
             * for example:
             * var v = it;
             * for(it in v) {
             *     it<complete>
             * }
             */
            if (!string.IsNullOrEmpty(local.Value) && sci.PositionFromLine(member.LineFrom) > local.Position) return;
            if (!TryInferGenericType(member).IsVoid()) return;
            if (member.Flags.HasFlag(FlagType.ParameterVar))
            {
                if (FileParser.IsFunctionType(member.Type) || !string.IsNullOrEmpty(member.Type)) return;
                InferParameterType(sci, member);
                return;
            }
            var ctx = ASContext.Context;
            if (member.Flags.HasFlag(FlagType.Function))
            {
                if (member.Flags.HasFlag(FlagType.Constructor)) return;
                if (member.Name.StartsWith("get_") || member.Name.StartsWith("set_"))
                {
                    var property = ctx.CurrentClass.Members.Search(member.Name.Substring(4), 0, 0);
                    if (property != null)
                    {
                        if (string.IsNullOrEmpty(property.Type)) InferType(sci, property);
                        member.Type = property.Type;
                        member.Flags |= FlagType.Inferred;
                        return;
                    }
                }
                InferFunctionType(sci, member);
                if (string.IsNullOrEmpty(member.Type))
                {
                    member.Type = ctx.Features.voidKey;
                    member.Flags |= FlagType.Inferred;
                }
                return;
            }
            var line = sci.GetLine(member.LineFrom);
            var m = Regex.Match(line, "\\s*for\\s*\\(\\s*" + member.Name + "\\s*in\\s*");
            if (!m.Success)
            {
                base.InferType(sci, local, member);
                if (string.IsNullOrEmpty(member.Type) && (member.Flags & (FlagType.Variable | FlagType.Getter | FlagType.Setter)) != 0)
                    member.Type = ResolveType(ctx.Features.dynamicKey, null).Name;
                return;
            }
            var currentModel = ctx.CurrentModel;
            var rvalueStart = sci.PositionFromLine(member.LineFrom) + m.Index + m.Length;
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
                    var type = ResolveType("Int", null);
                    member.Type = type.QualifiedName;
                    member.Flags |= FlagType.Inferred;
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
                     *     trace(a<cursor>);
                     * }
                     */
                    var wordLeft = sci.GetWordLeft(i - 1, false);
                    if (wordLeft == member.Name)
                    {
                        var lineBefore = sci.LineFromPosition(i) - 1;
                        var vars = local.LocalVars;
                        vars.Items.Sort((l, r) => l.LineFrom > r.LineFrom ? -1 : l.LineFrom < r.LineFrom ? 1 : 0);
                        var model = vars.Items.Find(it => it.LineFrom <= lineBefore);
                        if (model != null) expr = new ASResult {Type = ResolveType(model.Type, ctx.CurrentModel), InClass = ctx.CurrentClass};
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
                    if (exprType is null) return;
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
                        var iterator = members.Search("iterator", 0, 0);
                        if (iterator is null)
                        {
                            if (members.Contains("hasNext", 0, 0))
                            {
                                iterator = members.Search("next", 0, 0);
                                if (iterator != null) iteratorIndexType = iterator.Type;
                            }
                            var exprTypeIndexType = exprType.IndexType;
                            if (exprType.Name.StartsWithOrdinal("Iterator<")
                                && !string.IsNullOrEmpty(exprTypeIndexType) && ResolveType(exprTypeIndexType, currentModel).IsVoid())
                            {
                                exprType = expr.InClass;
                                break;
                            }
                            if (iteratorIndexType != null) break;
                        }
                        else
                        {
                            var type = ResolveType(iterator.Type, currentModel);
                            iteratorIndexType = type.IndexType;
                            break;
                        }
                        exprType = exprType.Extends;
                    }
                    if (iteratorIndexType != null)
                    {
                        member.Type = iteratorIndexType;
                        var exprTypeIndexType = exprType.IndexType;
                        if (!string.IsNullOrEmpty(exprTypeIndexType) && exprTypeIndexType.Contains(','))
                        {
                            var t = exprType;
                            var originTypes = t.IndexType.Split(',');
                            if (!originTypes.Contains(member.Type))
                            {
                                member.Type = null;
                                t.ResolveExtends();
                                t = t.Extends;
                                while (!t.IsVoid())
                                {
                                    var types = t.IndexType.Split(',');
                                    for (var j = 0; j < types.Length; j++)
                                    {
                                        if (types[j] != iteratorIndexType) continue;
                                        member.Type = originTypes[j].Trim();
                                        break;
                                    }
                                    if (member.Type != null) break;
                                    t = t.Extends;
                                }
                            }
                        }
                    }
                    if (member.Type is null)
                    {
                        var type = ResolveType(ctx.Features.dynamicKey, null);
                        member.Type = type.QualifiedName;
                    }
                    member.Flags |= FlagType.Inferred;
                    return;
                }
            }
        }

        protected override void InferVariableType(ScintillaControl sci, string declarationLine, int rvalueStart, ASExpr local, MemberModel var)
        {
            if (local.PositionExpression <= rvalueStart && rvalueStart <= local.Position) return;
            var features = ASContext.Context.Features;
            if (!string.IsNullOrEmpty(local.Separator))
            {
                string type = null;
                // for example: var v = value == v;
                if (features.BooleanOperators.Contains(local.Separator)) type = ResolveType(features.booleanKey, null).Name;
                // for example: var v = 1 | v;
                else if (features.BitwiseOperators.Contains(local.Separator)) type = ResolveType(features.IntegerKey, null).Name;
                // for example: var v = ++v;
                else if (features.IncrementDecrementOperators.Contains(local.Separator)) type = ResolveType(features.numberKey, null).Name;
                // for example: var v = 1 + v;
                else if (local.Separator.Length == 1 && features.ArithmeticOperators.Contains(local.Separator[0]))
                {
                    var expr = GetExpressionType(sci, local.SeparatorPosition, false, true);
                    type = expr.Member?.Type ?? expr.Type?.Name;
                }
                if (!string.IsNullOrEmpty(type))
                {
                    var.Type = type;
                    var.Flags |= FlagType.Inferred;
                    return;
                }
            }
            var word = sci.GetWordRight(rvalueStart, true);
            if (word == "new")
            {
                rvalueStart = sci.WordEndPosition(rvalueStart, false) + 1;
                word = sci.GetWordRight(rvalueStart, true);
            }
            // for example: `var v = v;` or `var V = new V();`
            if (word == local.Value) return;
            // for example: var v<complete> = untyped __js__('value')
            if (word != "true" && word != "false" && word != "null" && features.codeKeywords.Contains(word))
            {
                var type = ResolveType(features.dynamicKey, null);
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

        static bool InferVariableType(ScintillaControl sci, int rvalueStart, MemberModel var)
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
                if (c == '[')
                {
                    if (genCount == 0 && parCount == 0)
                    {
                        arrCount++;
                        isInExpr = true;
                    }
                }
                else if (c == ']')
                {
                    if (genCount == 0 && parCount == 0)
                    {
                        arrCount--;
                        rvalueEnd = i + 1;
                        if (arrCount < 0) break;
                    }
                }
                else if (c == '(')
                {
                    if (genCount == 0 && arrCount == 0)
                    {
                        parCount++;
                        isInExpr = true;
                    }
                }
                else if (c == ')')
                {
                    if (genCount == 0 && arrCount == 0)
                    {
                        parCount--;
                        rvalueEnd = i + 1;
                        if (parCount < 0) break;
                    }
                }
                else if (c == '<')
                {
                    if (parCount == 0 && arrCount == 0)
                    {
                        genCount++;
                        isInExpr = true;
                    }
                }
                else if (c == '>')
                {
                    if (parCount == 0 && arrCount == 0)
                    {
                        genCount--;
                        rvalueEnd = i + 1;
                        if (genCount < 0) break;
                    }
                }
                if (parCount > 0 || genCount > 0 || arrCount > 0) continue;
                if (c <= ' ')
                {
                    hadDot = false;
                    isInExpr = true;
                    continue;
                }
                if (c == ';' || (!hadDot && characterClass.Contains(c))) break;
                if (c == '.'
                    // for example: <StartPosition>expr1 + expr2<EndPosition>
                    || ctx.Features.ArithmeticOperators.Contains(c)
                    // for example: <StartPosition>expr1 ? expr2<EndPosition>
                    || c == '?')
                {
                    i += 1;
                    hadDot = true;
                    rvalueEnd = ExpressionEndPosition(sci, i, endPosition, true);
                }
                else
                {
                    var offset = 0;
                    if (// for example: <StartPosition>expr1 || expr2<EndPosition>
                        TryGetOperatorMaxLength(ctx.Features.BooleanOperators, c, ref offset)
                        // for example: <StartPosition>expr1 >> expr2<EndPosition>
                        || TryGetOperatorMaxLength(ctx.Features.BitwiseOperators, c, ref offset))
                    {
                        i += offset;
                        hadDot = true;
                        rvalueEnd = ExpressionEndPosition(sci, i, endPosition, true);
                    }
                    // Utils
                    static bool TryGetOperatorMaxLength(string[] operators, char firstChar, ref int result)
                    {
                        foreach (var it in operators)
                        {
                            if (it[0] == firstChar) result = Math.Max(result, it.Length);
                        }
                        return result > 0;
                    }
                }
                isInExpr = true;
            }
            var expr = GetExpressionType(sci, rvalueEnd, false, true);
            // for example: var v = ClassType;
            if (expr.Type != null && expr.Type != Context.StubFunctionClass)
            {
                if (expr.Type.Flags == FlagType.Class && expr.IsStatic)
                    var.Type = $"Class<{expr.Type.QualifiedName}>";
                else var.Type = expr.Type.QualifiedName;
                var.Flags |= FlagType.Inferred;
                return true;
            }
            if (expr.Member != null)
            {
                // for example: var v = someFunction;
                if (expr.Type == Context.StubFunctionClass)
                {
                    var.Flags |= FlagType.Function;
                    var.Comments = expr.Member.Comments;
                    var.Parameters = expr.Member.Parameters;
                    var.Type = expr.Member.Type;
                }
                // for example: var v = Class.new;
                else if (FileParser.IsFunctionType(expr.Member.Type))
                {
                    FileParser.FunctionTypeToMemberModel(expr.Member.Type, ctx.Features, var);
                    var.Comments = expr.Member.Comments;
                    var.Flags |= FlagType.Function;
                }
                else var.Type = expr.Member.Type;
                var.Flags |= FlagType.Inferred;
                return true;
            }
            return false;
        }

        static void InferParameterType(ScintillaControl sci, MemberModel var)
        {
            var ctx = ASContext.Context;
            var value = var.Value;
            var type = ctx.ResolveToken(value, ctx.CurrentModel);
            if (type.IsVoid())
            {
                if (!string.IsNullOrEmpty(value) && value != "null" && var.ValueEndPosition != -1
                    && char.IsLetter(value[0]) && (var.Name != value && (var.Name[0] != '?' || var.Name != '?' + value)))
                    type = GetExpressionType(sci, var.ValueEndPosition, false, true).Type ?? ClassModel.VoidClass;
                if (type.IsVoid()) type = ResolveType(ctx.Features.dynamicKey, null);
            }
            var.Type = type.Name;
        }

        static ClassModel TryInferGenericType(MemberModel var)
        {
            var ctx = ASContext.Context;
            var template = ctx.CurrentMember?.Template ?? ctx.CurrentClass?.Template;
            if (!string.IsNullOrEmpty(template) && !string.IsNullOrEmpty(var.Type)
                && ResolveType(var.Type, ctx.CurrentModel).IsVoid())
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
            return ResolveType(rvalue, ASContext.Context.CurrentModel);
        }

        static void InferFunctionType(ScintillaControl sci, MemberModel member)
        {
            /**
             *  for example:
             *
             *  function foo() {
             *      return "";
             *  }
             *
             *  function bar() {
             *      foo().<complete>
             *  }
             */
            var endPosition = sci.PositionFromLine(member.LineFrom);
            for (var i = sci.LineEndPosition(member.LineTo); i > endPosition; i--)
            {
                if (sci.PositionIsOnComment(i) || sci.CharAt(i) != ';') continue;
                var expr = GetExpression(sci, i, true);
                if (expr.Value is { } name && !string.IsNullOrEmpty(name) && name != ASContext.Context.Features.voidKey)
                {
                    var wordBefore = expr.WordBefore;
                    // for example: untyped "";<position>
                    if (string.IsNullOrEmpty(wordBefore))
                    {
                        var p = expr.PositionExpression - 1;
                        wordBefore = GetWordLeft(sci, ref p);
                        if (!string.IsNullOrEmpty(wordBefore)) expr.WordBeforePosition = p;
                    }
                    var isUntyped = wordBefore == "untyped";
                    if (isUntyped || wordBefore == "new") wordBefore = GetWordLeft(sci, expr.WordBeforePosition - 1);
                    else if (wordBefore != "return") wordBefore = GetWordLeft(sci, expr.PositionExpression);
                    if (wordBefore == "return")
                    {
                        if (isUntyped) member.Type = ASContext.Context.Features.dynamicKey;
                        /**
                         * for example:
                         * function foo() {
                         *     ...
                         *     return foo();
                         * }
                         */
                        else if (GetWordRight(sci, expr.PositionExpression) == member.Name)
                            member.Type = ASContext.Context.Features.dynamicKey;
                        else
                        {
                            var expressionType = GetExpressionType(sci, i, false, true);
                            var type = expressionType.Member?.Type ?? expressionType.Type?.Name;
                            member.Type = type;
                        }
                        member.Flags |= FlagType.Inferred;
                    }
                }
                break;
            }
        }

        /// <inheritdoc />
        protected override bool HandleImplementsCompletion(ScintillaControl sci, bool autoHide)
        {
            var extends = new HashSet<string>();
            var list = new List<ICompletionListItem>();
            foreach (var it in ASContext.Context.GetAllProjectClasses().Distinct())
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

        /// <inheritdoc />
        protected override ASExpr GetExpressionEx(ScintillaControl sci, int position, bool ignoreWhiteSpace)
        {
            var result = base.GetExpressionEx(sci, position, ignoreWhiteSpace);
            if (result.ContextFunction is { } member
                && (member.Flags & (FlagType) HaxeFlagType.Macro) != 0
                && sci.CharAt(result.PositionExpression - 1) == '$')
            {
                result.Value = $"${result.Value}";
                result.PositionExpression--;
                GetOperatorLeft(sci, result.PositionExpression, result);
            }
            return result;
        }

        /// <inheritdoc />
        protected override ASResult EvalExpression(string expression, ASExpr context, FileModel inFile, ClassModel inClass, bool complete, bool asFunction, bool filterVisibility)
        {
            if (!string.IsNullOrEmpty(expression))
            {
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
                var ctx = ASContext.Context;
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
                            if (count == 0)
                            {
                                // transform #0~ to []
                                context.Value = context.SubExpressions[0];
                            }
                            context.SubExpressions.RemoveAt(i);
                            return base.EvalExpression(expression, context, inFile, inClass, complete, asFunction, filterVisibility);
                        }
                        expression = expression.Replace(".#" + i + "~", "." + subExpression);
                    }
                }
                if (expression.Length > 1 && expression[0] is char c && (c == '\'' || c == '"'))
                {
                    var type = ResolveType(ctx.Features.stringKey, inFile);
                    // for example: ""|, ''|
                    if (context.SubExpressions is null) expression = type.Name + ".#.";
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
                else if (!context.SubExpressions.IsNullOrEmpty())
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
                if (string.IsNullOrEmpty(context.WordBefore) && expression[0] == '$') expression = expression.Substring(1);
            }
            var result = base.EvalExpression(expression, context, inFile, inClass, complete, asFunction, filterVisibility);
            // for example: trace<complete>, trace<cursor>()
            if (result.Member is null && result.Type is null && (expression == "trace.#0~" || expression == "trace"))
            {
                var type = ResolveType("haxe.Log", inFile);
                if (!type.IsVoid())
                {
                    result.Member = type.Members.Search("trace", 0, 0);
                    result.InClass = type;
                    result.InFile = type.InFile;
                    result.RelClass = inClass;
                    result.Type = Context.StubFunctionClass;
                }
            }
            return result;
        }

        protected override string GetToolTipTextEx(ASResult expr)
        {
            if (expr.Context is { } context)
            {
                if (expr.Type is { } leftExprType
                    && expr.Context.RightOperator is { } @operator
                    && PluginBase.MainForm.CurrentDocument?.SciControl is { } sci
                    && expr.Context.Position is { } position
                    && GetCharLeft(sci, true, ref position) is { } c
                    && @operator.Contains(c))
                {
                    if (leftExprType.Flags.HasFlag(FlagType.Abstract))
                    {
                        var endPosition = ExpressionEndPosition(sci, position + 1, true);
                        var rightExpr = GetExpressionType(sci, endPosition, false, true);
                        foreach (var member in leftExprType.Members)
                        {
                            if ((rightExpr.Type is null || (member.Parameters?.Count >= 2 && member.Parameters[1].Type == rightExpr.Type.Name))
                                && member.MetaDatas?.FirstOrDefault(it => it.Name == ":op") is { } meta
                                && meta.Params.TryGetValue("Default", out var value)
                                && Regex.IsMatch(value, $"\\w((\\s)|(?!\\s))+{Regex.Escape(@operator)}((\\s)|(?!\\s))+\\w"))
                            {
                                expr.Member = member;
                                break;
                            }
                        }
                    }
                }
                else if (expr.Member is null)
                {
                    // for example: cast<cursor>(expr, Type);
                    if (context.SubExpressions != null && context.WordBefore == "cast") expr.Member = Context.StubSafeCastFunction;
                    else if (context.Value is { } s)
                    {
                        // for example: cast<cursor> expr;
                        if (s == "cast") expr.Member = Context.StubUnsafeCastFunction;
                        // for example: 'c'.code<complete> or "\n".code<complete>
                        else if ((s.Length == 12 || (s.Length == 13 && s[1] == '\\')) && (s[0] == '\'' || s[0] == '"') && s.EndsWithOrdinal(".#0~.code"))
                            expr.Member = Context.StubStringCodeProperty;
                    }
                }
            }
            return base.GetToolTipTextEx(expr);
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

        protected override string GetMemberModifiersTooltipText(MemberModel member)
        {
            var result = base.GetMemberModifiersTooltipText(member);
            var flags = member.Flags;
            if (!flags.HasFlag(FlagType.Class))
            {
                if (flags.HasFlag((FlagType) HaxeFlagType.Macro)) result = $"macro {result}";
                if (flags.HasFlag((FlagType) HaxeFlagType.Inline)) result += "inline ";
            }
            return result;
        }

        protected override string GetMemberSignatureTooltipText(MemberModel member, string modifiers, string foundIn)
        {
            var flags = member.Flags;
            if (flags.HasFlag((FlagType) HaxeFlagType.Inline) && flags.HasFlag(FlagType.Variable) && member.Value is { } value)
                return $"{modifiers}var {member} = {value}{foundIn}";
            return base.GetMemberSignatureTooltipText(member, modifiers, foundIn);
        }

        protected override string GetCalltipDef(MemberModel member)
        {
            if ((member.Flags & FlagType.ParameterVar) != 0 && FileParser.IsFunctionType(member.Type))
            {
                var tmp = FileParser.FunctionTypeToMemberModel<MemberModel>(member.Type, ASContext.Context.Features);
                tmp.Name = member.Name;
                tmp.Flags |= FlagType.Function;
                member = tmp;
            }
            return base.GetCalltipDef(member);
        }

        protected override void GetInstanceMembers(bool autoHide, ASResult expr, ClassModel exprType, FlagType mask, int dotIndex, MemberList result)
        {
            if (exprType.Flags.HasFlag(FlagType.Abstract))
            {
                if (expr.Member?.Name == "this")
                {
                    var extends = exprType.Extends;
                    if (extends.IsVoid())
                    {
                        extends = ResolveType(exprType.ExtendsType, expr.InFile ?? ASContext.Context.CurrentModel);
                        if (extends.IsVoid()) return;
                    }
                    base.GetInstanceMembers(autoHide, expr, extends, mask, dotIndex, result);
                    return;
                }
                if (!string.IsNullOrEmpty(exprType.ExtendsType)
                    // for example: @:enum abstract
                    && exprType.MetaDatas is var metaDatas && (metaDatas is null || metaDatas.All(it => it.Name != ":enum"))
                    // for example: abstract Null<T> from T to T
                    && (string.IsNullOrEmpty(exprType.Template) || exprType.ExtendsType != exprType.IndexType))
                {
                    var access = ASContext.Context.TypesAffinity(ASContext.Context.CurrentClass, exprType);
                    result.Merge(exprType.GetSortedMembersList(), mask, access);
                    if (metaDatas is null) return;
                    var extends = exprType.Extends;
                    if (extends.IsVoid())
                    {
                        extends = ResolveType(exprType.ExtendsType, expr.InFile ?? ASContext.Context.CurrentModel);
                        if (extends.IsVoid()) return;
                    }
                    var @params = mask.HasFlag(FlagType.Static)
                        ? metaDatas.Find(it => it.Name == ":forwardStatics")?.Params
                        : metaDatas.Find(it => it.Name == ":forward")?.Params;
                    if (@params is null)
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
            Context.TryResolveStaticExtensions(exprType, ASContext.Context.CurrentModel, out exprType);
            base.GetInstanceMembers(autoHide, expr, exprType, mask, dotIndex, result);
        }

        /// <inheritdoc />
        protected override void FindMemberEx(string token, FileModel inFile, ASResult result, FlagType mask, Visibility access)
        {
            base.FindMemberEx(token, inFile, result, mask, access);
            if (result.Type != null && !result.Type.IsVoid()) return;
            var list = ASContext.Context.GetTopLevelElements();
            if (list.IsNullOrEmpty()) return;
            foreach (var it in list)
            {
                if (it.Name != token || !it.Flags.HasFlag(FlagType.Enum)) continue;
                var type = ResolveType(it.Type, inFile);
                result.Type = type;
                result.InClass = type;
                result.IsStatic = false;
                return;
            }
        }

        /// <inheritdoc />
        protected override void FindMemberEx(string token, ClassModel inClass, ASResult result, FlagType mask, Visibility access)
        {
            if (string.IsNullOrEmpty(token)) return;
            if (result.IsNull())
            {
                base.FindMemberEx(token, inClass, result, mask, access);
                if (result.Member != null && string.IsNullOrEmpty(result.Member.Type) && result.Member.Flags.HasFlag(FlagType.Function))
                    InferType(PluginBase.MainForm.CurrentDocument?.SciControl, result.Member);
            }
            var context = result.Context;
            var member = result.Member;
            // for example: Class.new<complete>
            if (token == "new" && member != null && context != null && context.SubExpressions is null)
            {
                result.IsStatic = false;
                result.Member = new MemberModel("new", null, member.Flags & ~FlagType.Constructor & ~FlagType.Function, member.Access);
                if (string.IsNullOrEmpty(result.Member.Type)) result.Member.Type = member.Name;
                result.Member.Parameters = member.Parameters;
                result.Member.Type = ToFunctionDeclarationString(result.Member);
                result.Member.Comments = member.Comments;
                result.Type = null;
                return;
            }
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
                else if (result.Type.IndexType is { } indexType && FileParser.IsFunctionType(indexType))
                {
                    result.Member = (MemberModel) result.Member.Clone();
                    FileParser.FunctionTypeToMemberModel(indexType, ASContext.Context.Features, result.Member);
                    result.Member.Name = "item";
                    result.Member.Flags |= FlagType.Function;
                    result.Type = (ClassModel) Context.StubFunctionClass.Clone();
                    result.Type.Parameters = result.Member.Parameters;
                    result.Type.Type = result.Member.Type;
                    return;
                }
            }
            else if (context != null
                     && member != null && (member.Flags.HasFlag(FlagType.Function)
                     // TODO slavara: temporary solution, because at the moment the function parameters are not converted to the function.
                     || member.Flags.HasFlag(FlagType.ParameterVar) && FileParser.IsFunctionType(member.Type)))
            {
                var returnType = member.Type;
                var subExpressions = context.SubExpressions;
                if (!string.IsNullOrEmpty(member.Template) && subExpressions?.LastOrDefault() is { } subExpression && subExpression.Length > 2)
                {
                    var subExpressionPosition = context.SubExpressionPositions.Last();
                    subExpression = subExpression.Substring(1, subExpression.Length - 2);
                    var expressions = new List<ASResult>();
                    var arrCount = 0;
                    var groupCount = 0;
                    for (int i = 0, length = subExpression.Length - 1; i <= length; i++)
                    {
                        var c = subExpression[i];
                        if (c == '[')
                        {
                            if (groupCount == 0) arrCount++;
                        }
                        else if (c == ']')
                        {
                            if (groupCount == 0) arrCount--;
                        }
                        else if (c == '(' || c == '{' || c == '<')
                        {
                            if (arrCount == 0) groupCount++;
                        }
                        else if (c == ')' || c == '}' || c == '>')
                        {
                            if (arrCount == 0) groupCount--;
                        }
                        if ((arrCount == 0 && groupCount == 0 && c == ',') || i == length)
                        {
                            if (i == length) i++;
                            var expr = GetExpressionType(PluginBase.MainForm.CurrentDocument?.SciControl, subExpressionPosition + i, false, true);
                            if (expr.Type is null) expr.Type = ClassModel.VoidClass;
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
                        if (template.Contains(':', out var p)) template = template.Substring(0, p);
                        var reTemplateType = new Regex($"\\b{template}\\b");
                        if (member.Parameters is { } parameters)
                        {
                            for (var j = 0; j < parameters.Count && j < expressions.Count; j++)
                            {
                                var parameter = parameters[j];
                                var parameterType = parameter.Type;
                                if (parameterType != template)
                                {
                                    // for example: typedef Null<T> = T, abstract Null<T> from T to T
                                    if (reTemplateType.IsMatch(parameterType)
                                        && ResolveType(parameterType, result.InFile) is { } expr && !expr.IsVoid()
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
                    && subExpressions != null && subExpressions.Count > 1)
                {
                    var type = (ClassModel) Context.StubFunctionClass.Clone();
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
            // for example: Null<SomeType>
            if (inClass.ExtendsType is { } extendsType && !string.IsNullOrEmpty(extendsType) && extendsType != ASContext.Context.Features.objectKey
                && inClass.Extends.IsVoid() && !string.IsNullOrEmpty(inClass.Template) && !string.IsNullOrEmpty(inClass.IndexType))
            {
                var type = ResolveType(extendsType, ASContext.Context.CurrentModel);
                if (!type.IsVoid()) inClass = type;
            }
            base.FindMemberEx(token, inClass, result, mask, access);
            /**
             * for example:
             * using StringTools;
             * "".startsWith(<complete>
             */
            if (result.IsNull() && !string.IsNullOrEmpty(result.Path) && result.RelClass != null && !result.RelClass.IsVoid())
            {
                if (Context.TryResolveStaticExtensions(inClass, ASContext.Context.CurrentModel, out inClass))
                {
                    base.FindMemberEx(token, inClass, result, mask, access);
                    if (result.Member != null && result.Member.Flags.HasFlag(FlagType.Using))
                    {
                        var relClass = FindMember(result.Member.LineFrom, result.Member.InFile.Classes) ?? ClassModel.VoidClass;
                        result.InClass = relClass;
                        result.RelClass = relClass;
                        result.InFile = result.Member.InFile;
                        result.Type = null;
                    }
                }
            }
            member = result.Member;
            if (member?.Type != null && (result.Type is null || result.Type.IsVoid()))
            {
                /**
                 * for example:
                 * class Some<T:String> {
                 *     var v:T;
                 *     function test() {
                 *         v.<complete>
                 *     }
                 * }
                 */
                var clone = (MemberModel) member.Clone();
                var type = TryInferGenericType(clone);
                if (!type.IsVoid())
                {
                    result.Member = clone;
                    result.Type = type;
                    return;
                }
                /**
                 * for example:
                 * var v = (variable:IInterface).someMethod<T:{}>((param0:Class<T>): String):T;
                 * v.<complete>
                 */
                type = ResolveType(member.Type, result.InClass?.InFile ?? ASContext.Context.CurrentModel);
                if (!type.IsVoid()) result.Type = type;
            }
        }

        protected override Visibility TypesAffinity(ASExpr context, ClassModel inClass, ClassModel withClass)
        {
            var result = base.TypesAffinity(context, inClass, withClass);
            if (context != null
                && PluginBase.MainForm.CurrentDocument?.SciControl is { } sci
                && context.WordBefore == "privateAccess" && context.WordBeforePosition is { } p
                && sci.CharAt(p - 2) == '@' && sci.CharAt(p - 1) == ':') result |= Visibility.Private;
            return result;
        }

        public override string ToFunctionDeclarationString(MemberModel member)
        {
            var voidKey = ASContext.Context.Features.voidKey;
            var dynamicTypeName = ResolveType(ASContext.Context.Features.dynamicKey, null).Name;
            var parameters = member.Parameters?.Select(it => it.Type).ToList() ?? new List<string> {voidKey};
            parameters.Add(member.Type ?? voidKey);
            var sb = new StringBuilder();
            for (var i = 0; i < parameters.Count; i++)
            {
                if (i > 0) sb.Append("->");
                var t = parameters[i];
                if (t is null) sb.Append(dynamicTypeName);
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

        public override MemberModel FunctionTypeToMemberModel(string type, FileModel inFile)
        {
            var result = FileParser.FunctionTypeToMemberModel<MemberModel>(type, ASContext.Context.Features);
            return result.Type != type ? result : null;
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
                if (expr.Member is { } m && m.Type is { } typeName)
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
                    var type = ResolveType(typeName, ctx.CurrentModel);
                    if (type.Members.Count == 0) return null;
                    if ((type.Flags.HasFlag(FlagType.Abstract) && type.MetaDatas != null && type.MetaDatas.Any(tag => tag.Name == ":enum")))
                    {
                        return type.Members.Select(it => new MemberItem(it)).ToList<ICompletionListItem>();
                    }
                    if (type.Flags.HasFlag(FlagType.Enum))
                    {
                        return type.Members.Select(it =>
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
                static string CleanNullableType(string s)
                {
                    var startIndex = s.IndexOfOrdinal("Null<");
                    if (startIndex == -1) return s;
                    startIndex += 5;
                    while (s.IndexOfOrdinal("Null<", startIndex) is { } p && p != -1)
                    {
                        startIndex = p + 5;
                    }
                    return s.Substring(startIndex, s.Length - (startIndex + startIndex / 5));
                }
            }
        }

        static void GenerateExpressionReification(ScintillaControl sci, string name)
        {
            sci.BeginUndoAction();
            try
            {
                sci.SelectWord();
                var position = sci.SelectionStart + name.Length + 1;
                sci.ReplaceSel($"{name}{{}}");
                sci.SetSel(position, position);
            }
            finally
            {
                sci.EndUndoAction();
            }
        }

        static void GenerateObjectInitializer(ScintillaControl sci, ClassModel type)
        {
            sci.BeginUndoAction();
            try
            {
                var members = new List<MemberModel>();
                GetMembers(type, members);
                var sb = new StringBuilder();
                sb.Append('{');
                for (var i = 0; i < members.Count; i++)
                {
                    var member = members[i];
                    sb.Append(SnippetHelper.BOUNDARY);
                    sb.Append("\n\t");
                    if (member.MetaDatas != null && member.MetaDatas.Any(it => it.Name == ":optional"))
                    {
                        sb.Append("//");
                        sb.Append(member.Name);
                        sb.Append(':');
                        sb.Append(',');
                        sb.Append("// optional");
                    }
                    else
                    {
                        sb.Append(member.Name);
                        sb.Append(':');
                        if (i == 0) sb.Append(SnippetHelper.ENTRYPOINT);
                        sb.Append(',');
                    }
                }
                sb.Append("\n}");
                var pos = sci.CurrentPos;
                if (GetCharLeft(sci, ref pos) == '{') sci.SetSel(pos, sci.CurrentPos);
                ASGenerator.InsertCode(sci.CurrentPos, sb.ToString());
            }
            finally
            {
                sci.EndUndoAction();
            }
            // Utils
            static void GetMembers(ClassModel t, List<MemberModel> result)
            {
                t.ResolveExtends();
                while (!t.IsVoid())
                {
                    result.AddRange(t.Members);
                    if (t.ExtendsTypes != null)
                    {
                        for (int i = 0, count = t.ExtendsTypes.Count; i < count; i++)
                        {
                            var model = ResolveType(t.ExtendsTypes[i], t.InFile ?? ASContext.Context.CurrentModel);
                            if (!model.IsVoid()) GetMembers(model, result);
                        }
                    }
                    t = t.Extends;
                }
            }
        }

        static void GenerateObjectInitializer(ScintillaControl sci, string template)
        {
            sci.BeginUndoAction();
            try
            {
                var pos = sci.CurrentPos;
                if (GetCharLeft(sci, ref pos) == template[0]) sci.SetSel(pos, sci.CurrentPos);
                ASGenerator.InsertCode(sci.CurrentPos, template);
            }
            finally
            {
                sci.EndUndoAction();
            }
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

    class ExpressionReificationGeneratorItem : ObjectInitializerGeneratorItem
    {
        public ExpressionReificationGeneratorItem(string name, string exprType, string returnType, string comments, Action action)
            : base($"${name}{{}}", $"${name}{{expr:{exprType}}} : {returnType}\r\n\r\n{comments}", action)
        {
        }
    }

    class ObjectInitializerGeneratorItem : ICompletionListItem
    {
        readonly Action action;

        public ObjectInitializerGeneratorItem(string name, string description, Action action)
        {
            Label = name;
            Description = description;
            this.action = action;
        }

        public string Label { get; }
        public string Description { get; }
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