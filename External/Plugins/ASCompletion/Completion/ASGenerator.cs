using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ASCompletion.Context;
using ASCompletion.Generators;
using ASCompletion.Helpers;
using ASCompletion.Model;
using ASCompletion.Settings;
using PluginCore;
using PluginCore.Controls;
using PluginCore.Helpers;
using PluginCore.Localization;
using PluginCore.Managers;
using PluginCore.Utilities;
using ScintillaNet;

namespace ASCompletion.Completion
{
    public class ASGenerator : IContextualGenerator
    {
        #region context detection (ie. entry points)

        internal const string patternEvent = "Listener\\s*\\((\\s*([a-z_0-9.\\\"']+)\\s*,)?\\s*(?<event>[a-z_0-9.\\\"']+)\\s*,\\s*(this\\.)?{0}";
        const string patternAS2Delegate = @"\.\s*create\s*\(\s*[a-z_0-9.]+,\s*{0}";
        const string patternVarDecl = @"\s*{0}\s*:\s*{1}";
        const string patternMethod = @"{0}\s*\(";
        const string patternMethodDecl = @"function\s+{0}\s*\(";
        const string patternClass = @"new\s*{0}";
        const string BlankLine = "$(Boundary)\n\n";
        protected internal const string NewLine = "$(Boundary)\n";
        private static readonly Regex reModifiers = new Regex("^\\s*(\\$\\(Boundary\\))?([a-z ]+)(function|var|const)", RegexOptions.Compiled);
        private static readonly Regex reSuperCall = new Regex("^super\\s*\\(", RegexOptions.Compiled);

        protected internal static string contextToken;
        internal static string contextParam;
        internal static Match contextMatch;
        internal static ASResult contextResolved;
        internal static MemberModel contextMember;
        private static bool firstVar;

        readonly CodeGeneratorInterfaceBehavior codeGeneratorInterfaceBehavior = new CodeGeneratorInterfaceBehavior();

        protected virtual ICodeGeneratorBehavior GetCodeGeneratorBehavior()
        {
            if ((ASContext.Context.CurrentClass.Flags & FlagType.Interface) != 0)
                return codeGeneratorInterfaceBehavior;
            return null;
        }

        private static bool IsHaxe => ASContext.Context.CurrentModel.haXe;

        public static bool HandleGeneratorCompletion(ScintillaControl sci, bool autoHide, string word)
        {
            var generator = ASContext.Context.CodeGenerator as ASGenerator;
            if (generator == null) return false;
            if (!string.IsNullOrEmpty(word) && word == ASContext.Context.Features.overrideKey)
                return generator.HandleOverrideCompletion(autoHide);
            return false;
        }

        public static void ContextualGenerator(ScintillaControl sci, List<ICompletionListItem> options)
        {
            ASContext.Context.CodeGenerator.ContextualGenerator(sci, sci.CurrentPos, options);
        }

        public bool ContextualGenerator(ScintillaControl sci, int position, List<ICompletionListItem> options)
        {
            if (ASContext.Context is ASContext ctx) ctx.UpdateCurrentFile(false);

            lookupPosition = -1;
            if (sci.PositionIsOnComment(position)) return false;
            var style = sci.BaseStyleAt(position);
            if (style == 19 || style == 24) // on keyword
                return false;
            contextMatch = null;
            contextToken = sci.GetWordFromPosition(position);
            var expr = ASComplete.GetExpressionType(sci, sci.WordEndPosition(position, true));
            contextResolved = expr;
            ContextualGenerator(sci, position, expr, options);
            return true;
        }

        protected virtual void ContextualGenerator(ScintillaControl sci, int position, ASResult resolve, List<ICompletionListItem> options)
        {
            var suggestItemDeclaration = false;
            var ctx = ASContext.Context;
            var line = sci.LineFromPosition(position);
            var found = GetDeclarationAtLine(line);
            if (contextToken != null && resolve.Member == null && sci.BaseStyleAt(position) != 5)
            {
                // import declaration
                if ((resolve.Type == null || resolve.Type.IsVoid() || !ctx.IsImported(resolve.Type, line)) && CheckAutoImport(resolve, options)) return;
                if (resolve.Type == null)
                {
                    if (CanShowGenerateClass(sci, position, resolve, found))
                    {
                        ShowGenerateClassList(found, resolve.Context, options);
                        return;
                    }
                    suggestItemDeclaration = ASComplete.IsTextStyle(sci.BaseStyleAt(position - 1));
                }
            }

            var behavior = GetCodeGeneratorBehavior();
            if (behavior != null)
            {
                behavior.ContextualGenerator(sci, position, resolve, options);
                return;
            }
            
            if (CanShowConvertToConst(sci, position, resolve, found))
            {
                ShowConvertToConst(found, options);
                return;
            }

            // ignore automatic vars (MovieClip members)
            if (resolve.Member != null && ((resolve.Member.Flags & FlagType.AutomaticVar) > 0 || resolve.InClass?.QualifiedName == "Object"))
            {
                resolve.Member = null;
                resolve.Type = null;
            }

            if (!found.InClass.IsVoid() && contextToken != null)
            {
                // implement interface
                if (CanShowImplementInterfaceList(sci, position, resolve, found))
                {
                    if (ctx.Features.hasGenerics && resolve.RelClass?.Implements != null)
                    {
                        var name = resolve.Type.Name;
                        foreach (var it in resolve.RelClass.Implements)
                        {
                            string interfaceName;
                            if (it.IndexOf('<') is int p && p != -1) interfaceName = it.Substring(0, p);
                            else interfaceName = it;
                            if (interfaceName != name) continue;
                            contextParam = it;
                            break;
                        }
                    }
                    else contextParam = resolve.Type.Type;
                    ShowImplementInterface(found, options);
                    return;
                }
                // promote to class var
                if (!ctx.CurrentClass.IsVoid() && resolve.Member != null && (resolve.Member.Flags & FlagType.LocalVar) > 0)
                {
                    contextMember = resolve.Member;
                    ShowPromoteLocalAndAddParameter(found, options);
                    return;
                }
            }
           
            if (found.Member != null)
            {
                // private var -> property
                if ((found.Member.Flags & FlagType.Variable) > 0 && (found.Member.Flags & FlagType.LocalVar) == 0)
                {
                    var text = sci.GetLine(line);
                    // maybe we just want to import the member's non-imported type
                    var m = Regex.Match(text, string.Format(patternVarDecl, found.Member.Name, contextToken));
                    if (m.Success)
                    {
                        contextMatch = m;
                        var type = ctx.ResolveType(contextToken, ctx.CurrentModel);
                        if (type.IsVoid() && CheckAutoImport(resolve, options)) return;
                    }
                    if (CanShowGetSetList(sci, position, resolve, found)) ShowGetSetList(found, options);
                    return;
                }
                // inside a function
                if ((found.Member.Flags & (FlagType.Function | FlagType.Getter | FlagType.Setter)) > 0
                    && resolve.Member == null && resolve.Type == null)
                {
                    if (CanShowGenerateGetter(sci, position, resolve, found))
                    {
                        ShowGetterList(found, options);
                        return;
                    }
                    if (CanShowGenerateSetter(sci, position, resolve, found))
                    {
                        ShowSetterList(found, options);
                        return;
                    }
                    var text = sci.GetLine(line);
                    if (contextToken != null)
                    {
                        // "generate event handlers" suggestion
                        var re = string.Format(patternEvent, contextToken);
                        var m = Regex.Match(text, re, RegexOptions.IgnoreCase);
                        if (m.Success)
                        {
                            contextMatch = m;
                            var pos = ASComplete.ExpressionEndPosition(sci, sci.PositionFromLine(line) + m.Index);
                            var expr = ASComplete.GetExpressionType(sci, pos, false, true);
                            contextParam = ((ASGenerator) ctx.CodeGenerator).CheckEventType(expr.Member, m.Groups["event"].Value);
                            ShowEventList(found, options);
                            return;
                        }
                        m = Regex.Match(text, string.Format(patternAS2Delegate, contextToken), RegexOptions.IgnoreCase);
                        if (m.Success)
                        {
                            contextMatch = m;
                            ShowDelegateList(found, options);
                            return;
                        }
                        // suggest delegate
                        if (ctx.Features.hasDelegates)
                        {
                            m = Regex.Match(text, @"([a-z0-9_.]+)\s*\+=\s*" + contextToken, RegexOptions.IgnoreCase);
                            if (m.Success)
                            {
                                int offset = sci.PositionFromLine(sci.LineFromPosition(position))
                                    + m.Groups[1].Index + m.Groups[1].Length;
                                resolve = ASComplete.GetExpressionType(sci, offset);
                                if (resolve.Member != null)
                                    contextMember = ResolveDelegate(resolve.Member.Type, resolve.InFile);
                                contextMatch = m;
                                ShowDelegateList(found, options);
                                return;
                            }
                        }
                    }
                    else
                    {
                        // insert a default handler name, then "generate event handlers" suggestion
                        Match m = Regex.Match(text, String.Format(patternEvent, ""), RegexOptions.IgnoreCase);
                        if (m.Success)
                        {
                            int regexIndex = m.Index + sci.PositionFromLine(sci.CurrentLine);
                            GenerateDefaultHandlerName(sci, position, regexIndex, m.Groups["event"].Value, true);
                            resolve = ASComplete.GetExpressionType(sci, sci.CurrentPos);
                            if (resolve.Member == null || (resolve.Member.Flags & FlagType.AutomaticVar) > 0)
                            {
                                contextMatch = m;
                                contextParam = CheckEventType(m.Groups["event"].Value);
                                ShowEventList(found, options);
                            }
                            return;
                        }

                        // insert default delegate name, then "generate delegate" suggestion
                        if (ctx.Features.hasDelegates)
                        {
                            m = Regex.Match(text, @"([a-z0-9_.]+)\s*\+=\s*", RegexOptions.IgnoreCase);
                            if (m.Success)
                            {
                                int offset = sci.PositionFromLine(sci.LineFromPosition(position))
                                        + m.Groups[1].Index + m.Groups[1].Length;
                                resolve = ASComplete.GetExpressionType(sci, offset);
                                if (resolve.Member != null)
                                {
                                    contextMember = ResolveDelegate(resolve.Member.Type, resolve.InFile);
                                    string delegateName = resolve.Member.Name;
                                    if (delegateName.StartsWithOrdinal("on")) delegateName = delegateName.Substring(2);
                                    GenerateDefaultHandlerName(sci, position, offset, delegateName, false);
                                    resolve = ASComplete.GetExpressionType(sci, sci.CurrentPos);
                                    if (resolve.Member == null || (resolve.Member.Flags & FlagType.AutomaticVar) > 0)
                                    {
                                        contextMatch = m;
                                        ShowDelegateList(found, options);
                                    }
                                    return;
                                }
                            }
                        }
                    }
                }

                // "Generate fields from parameters" suggestion
                if ((found.Member.Flags & FlagType.Function) > 0
                    && found.Member.Parameters != null && (found.Member.Parameters.Count > 0)
                    && resolve.Member != null && (resolve.Member.Flags & FlagType.ParameterVar) > 0)
                {
                    contextMember = resolve.Member;
                    ShowFieldFromParameter(found, options);
                    return;
                }

                // "add to interface" suggestion
                if (CanShowAddToInterfaceList(sci, position, resolve, found))
                {
                    string funcName = found.Member.Name;
                    FlagType flags = found.Member.Flags & ~FlagType.Access;

                    List<string> interfaces = new List<string>();
                    foreach (string interf in found.InClass.Implements)
                    {
                        bool skip = false;
                        ClassModel cm = ctx.ResolveType(interf, ctx.CurrentModel);
                        foreach (MemberModel m in cm.Members)
                        {
                            if (m.Name.Equals(funcName) && m.Flags.Equals(flags))
                            {
                                skip = true;
                                break;
                            }
                        }
                        if (!skip)
                        {
                            interfaces.Add(interf);
                        }
                    }
                    if (interfaces.Count > 0)
                    {
                        ShowAddInterfaceDefList(found, interfaces, options);
                        return;
                    }
                }

                // "assign var to statement" suggestion
                var curLine = sci.GetLine(sci.CurrentLine);
                var ln = curLine.TrimEnd();
                if (ln.Length > 0
                    && sci.PositionFromLine(sci.CurrentLine) is var positionFromLine
                    && ln.Length <= sci.CurrentPos - positionFromLine) // cursor at end of line
                {
                    var returnType = GetStatementReturnType(sci, found.InClass, curLine, positionFromLine);
                    if (!CanShowAssignStatementToVariable(sci, returnType.Resolve)) return;
                    if (returnType.Resolve.Type == null && returnType.Resolve.Context?.WordBefore == "new") ShowGenerateClassList(found, returnType.Resolve.Context, options);
                    else if (returnType.Resolve.Type == null && returnType.Resolve.Member == null) return;
                    else ShowAssignStatementToVarList(found, returnType, options);
                    return;
                }
            }

            // suggest generate constructor / toString
            if (CanShowGenerateConstructorAndToString(sci, position, resolve, found))
            {
                bool hasConstructor = false;
                bool hasToString = false;
                foreach (MemberModel m in ctx.CurrentClass.Members)
                {
                    if (!hasConstructor && (m.Flags & FlagType.Constructor) > 0)
                        hasConstructor = true;

                    if (!hasToString && (m.Flags & FlagType.Function) > 0 && m.Name.Equals("toString"))
                        hasToString = true;
                }

                if (!hasConstructor || !hasToString)
                {
                    ShowConstructorAndToStringList(found, hasConstructor, hasToString, options);
                    return;
                }
            }

            if (resolve.Member != null
                && resolve.Type != null
                && resolve.Type.QualifiedName == ctx.Features.stringKey
                && !found.InClass.IsVoid())
            {
                int lineStartPos = sci.PositionFromLine(sci.CurrentLine);
                var text = sci.GetLine(line);
                var lineStart = text.Substring(0, sci.CurrentPos - lineStartPos);
                var m = Regex.Match(lineStart, @"new\s+(?<event>\w+)\s*\(\s*\w+");
                if (m.Success)
                {
                    var g = m.Groups["event"];
                    var eventResolve = ASComplete.GetExpressionType(sci, lineStartPos + g.Index + g.Length);
                    if (eventResolve?.Type != null)
                    {
                        var aType = eventResolve.Type;
                        aType.ResolveExtends();
                        while (!aType.IsVoid() && aType.QualifiedName != "Object")
                        {
                            if (aType.QualifiedName == "flash.events.Event")
                            {
                                contextParam = eventResolve.Type.QualifiedName;
                                ShowEventMetatagList(found, options);
                                return;
                            }
                            aType = aType.Extends;
                        }
                    }
                }
            }

            // suggest declaration
            if (contextToken != null)
            {
                if (suggestItemDeclaration)
                {
                    var text = sci.GetLine(line);
                    var m = Regex.Match(text, string.Format(patternClass, contextToken));
                    if (m.Success)
                    {
                        contextMatch = m;
                        ShowGenerateClassList(found, options);
                    }
                    else if (!found.InClass.IsVoid())
                    {
                        m = Regex.Match(text, string.Format(patternMethod, contextToken));
                        if (m.Success)
                        {
                            if (CanShowNewMethodList(sci, position, resolve, found))
                            {
                                contextMatch = m;
                                ((ASGenerator) ctx.CodeGenerator).ShowNewMethodList(sci, resolve, found, options);
                            }
                        }
                        else
                        {
                            if (CanShowNewVarList(sci, position, resolve, found)) ((ASGenerator) ctx.CodeGenerator).ShowNewVarList(sci, resolve, found, options);
                            if (CanShowGenerateInterface(sci, position, resolve, found)) ShowGenerateInterfaceList(resolve, found, options);
                        }
                    }
                }
                else
                {
                    if (resolve.InClass?.InFile != null
                        && resolve.Member != null
                        && (resolve.Member.Flags & FlagType.Function) > 0
                        && File.Exists(resolve.InClass.InFile.FileName)
                        && !resolve.InClass.InFile.FileName.StartsWithOrdinal(PathHelper.AppDir))
                    {
                        var text = sci.GetLine(line);
                        var m1 = Regex.Match(text, string.Format(patternMethodDecl, contextToken));
                        var m2 = Regex.Match(text, string.Format(patternMethod, contextToken));
                        if (!m1.Success && m2.Success)
                        {
                            contextMatch = m1;
                            ShowChangeMethodDeclList(found, options);
                        }
                    }
                    else if (resolve.Type?.InFile != null
                        && resolve.RelClass != null
                        && File.Exists(resolve.Type.InFile.FileName)
                        && !resolve.Type.InFile.FileName.StartsWithOrdinal(PathHelper.AppDir))
                    {
                        var text = sci.GetLine(line);
                        var m = Regex.Match(text, string.Format(patternClass, contextToken));
                        if (m.Success)
                        {
                            contextMatch = m;
                            MemberModel constructor = null;
                            var type = resolve.Type;
                            type.ResolveExtends();
                            while (!type.IsVoid())
                            {
                                constructor = type.Members.Search(type.Name, FlagType.Constructor, 0);
                                if (constructor != null) break;
                                type = type.Extends;
                            }
                            if (constructor == null) ShowConstructorAndToStringList(new FoundDeclaration { InClass = resolve.Type }, false, true, options);
                            else
                            {
                                var constructorParametersCount = constructor.Parameters?.Count ?? 0;
                                var wordEndPosition = sci.WordEndPosition(sci.CurrentPos, true);
                                var parameters = ParseFunctionParameters(sci, wordEndPosition);
                                if (parameters.Count != constructorParametersCount) ShowChangeConstructorDeclarationList(found, parameters, options);
                                else
                                {
                                    for (var i = 0; i < parameters.Count; i++)
                                    {
                                        if (parameters[i].paramType == constructor.Parameters[i].Type) continue;
                                        ShowChangeConstructorDeclarationList(found, parameters, options);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            // TODO: Empty line, show generators list? yep
        }

        /// <summary>
        /// Check if "Assign statement to variable" are available at the current cursor position.
        /// </summary>
        /// <param name="sci">The Scintilla control containing the document</param>
        /// <param name="expr">Expression at cursor position</param>
        /// <returns>true, if can show "Assign statement to variable" list</returns>
        protected virtual bool CanShowAssignStatementToVariable(ScintillaControl sci, ASResult expr)
        {
            // for example: return expr<generator>
            if (expr.Context.WordBefore == "return") return false;
            return (expr.Member?.Type is string t && t != ASContext.Context.Features.voidKey) || expr.Type != ClassModel.VoidClass;
        }

        /// <summary>
        /// Check if "Convert to constant" are available at the current cursor position.
        /// </summary>
        /// <param name="sci">The Scintilla control containing the document</param>
        /// <param name="position">Cursor position</param>
        /// <param name="expr">Expression at cursor position</param>
        /// <param name="found">The declaration target at current line(can not be null)</param>
        /// <returns>true, if can show "Convert to constant" list</returns>
        protected virtual bool CanShowConvertToConst(ScintillaControl sci, int position, ASResult expr, FoundDeclaration found)
        {
            return !ASContext.Context.CurrentClass.Flags.HasFlag(FlagType.Interface)
                && ASComplete.IsLiteralStyle(sci.BaseStyleAt(position));
        }

        /// <summary>
        /// Check if "Getter and Setter" are available at the current cursor position.
        /// </summary>
        /// <param name="sci">The Scintilla control containing the document</param>
        /// <param name="position">Cursor position</param>
        /// <param name="expr">Expression at cursor position</param>
        /// <param name="found">The declaration target at current line(can not be null)</param>
        /// <returns>true, if can show "Getter and Setter" list</returns>
        protected virtual bool CanShowGetSetList(ScintillaControl sci, int position, ASResult expr, FoundDeclaration found) => true;

        /// <summary>
        /// Check if "Getter" are available at the current cursor position.
        /// </summary>
        /// <param name="sci">The Scintilla control containing the document</param>
        /// <param name="position">Cursor position</param>
        /// <param name="expr">Expression at cursor position</param>
        /// <param name="found">The declaration target at current line(can not be null)</param>
        /// <returns>true, if can show "Getter" list</returns>
        protected virtual bool CanShowGenerateGetter(ScintillaControl sci, int position, ASResult expr, FoundDeclaration found) => false;

        /// <summary>
        /// Check if "Setter" are available at the current cursor position.
        /// </summary>
        /// <param name="sci">The Scintilla control containing the document</param>
        /// <param name="position">Cursor position</param>
        /// <param name="expr">Expression at cursor position</param>
        /// <param name="found">The declaration target at current line(can not be null)</param>
        /// <returns>true, if can show "Setter" list</returns>
        protected virtual bool CanShowGenerateSetter(ScintillaControl sci, int position, ASResult expr, FoundDeclaration found) => false;

        /// <summary>
        /// Check if "Generate constructor" and "Generate toString()" are available at the current cursor position.
        /// </summary>
        /// <param name="sci">The Scintilla control containing the document</param>
        /// <param name="position">Cursor position</param>
        /// <param name="expr">Expression at cursor position</param>
        /// <param name="found">The declaration target at current line(can not be null)</param>
        /// <returns>true, if can show "Generate constructor" and(or) "Generate toString()" list</returns>
        protected virtual bool CanShowGenerateConstructorAndToString(ScintillaControl sci, int position, ASResult expr, FoundDeclaration found)
        {
            return contextToken == null
                && found.Member == null
                && !found.InClass.IsVoid()
                && !found.InClass.Flags.HasFlag(FlagType.Interface)
                && position < sci.LineEndPosition(found.InClass.LineTo)
                && !ASContext.Context.CodeComplete.PositionIsBeforeBody(sci, position, found.InClass);
        }

        /// <summary>
        /// Check if "Implement Interface" are available at the current cursor position.
        /// </summary>
        /// <param name="sci">The Scintilla control containing the document</param>
        /// <param name="position">Cursor position</param>
        /// <param name="expr">Expression at cursor position</param>
        /// <param name="found">Declaration target at current line(can not be null)</param>
        /// <returns>true, if can show "Implement Interface" list</returns>
        protected virtual bool CanShowImplementInterfaceList(ScintillaControl sci, int position, ASResult expr, FoundDeclaration found)
        {
            if (expr.Context.ContextFunction != null || expr.Context.ContextMember != null
                || expr.Member != null
                || expr.Type == null || (expr.Type.Flags & FlagType.Interface) == 0) return false;
            var type = expr.Type;
            type.ResolveExtends();
            while (!type.IsVoid())
            {
                if (type.Members.Count > 0) return true;
                type = type.Extends;
            }
            return false;
        }

        /// <summary>
        /// Check if "Generate public function and Generate public callback" are available at the current cursor position.
        /// </summary>
        /// <param name="sci">The Scintilla control containing the document</param>
        /// <param name="position">Cursor position</param>
        /// <param name="expr">Expression at cursor position</param>
        /// <param name="found">Declaration target at current line(can not be null)</param>
        /// <returns>true, if can show "Generate public function and Generate public callback" list</returns>
        protected virtual bool CanShowNewMethodList(ScintillaControl sci, int position, ASResult expr, FoundDeclaration found)
        {
            var inClass = expr.RelClass ?? found.InClass;
            return (inClass.Flags & FlagType.Interface) == 0 || !expr.IsStatic;
        }

        /// <summary>
        /// Check if "Generate public variable" are available at the current cursor position.
        /// </summary>
        /// <param name="sci">The Scintilla control containing the document</param>
        /// <param name="position">Cursor position</param>
        /// <param name="expr">Expression at cursor position</param>
        /// <param name="found">Declaration target at current line(can not be null)</param>
        /// <returns>true, if can show "Generate public function and Generate public callback" list</returns>
        protected virtual bool CanShowNewVarList(ScintillaControl sci, int position, ASResult expr, FoundDeclaration found)
        {
            var inClass = expr.RelClass ?? found.InClass;
            return (inClass.Flags & FlagType.Interface) == 0 || !expr.IsStatic;
        }

        /// <summary>
        /// Check if "Add to interface" are available at the current cursor position.
        /// </summary>
        /// <param name="sci">The Scintilla control containing the document</param>
        /// <param name="position">Cursor position</param>
        /// <param name="expr">Expression at cursor position</param>
        /// <param name="found">Declaration target at current line(can not be null)</param>
        /// <returns>true, if can show "Add to interface" list</returns>
        protected virtual bool CanShowAddToInterfaceList(ScintillaControl sci, int position, ASResult expr, FoundDeclaration found)
        {
            return expr.Member != null
                   && !expr.Member.Flags.HasFlag(FlagType.Static)
                   && !expr.Member.Flags.HasFlag(FlagType.Constructor)
                   && expr.Member.Name == found.Member.Name
                   && sci.LineFromPosition(position) == found.Member.LineFrom
                   && ((found.Member.Flags & FlagType.Function) > 0
                       || (found.Member.Flags & FlagType.Getter) > 0
                       || (found.Member.Flags & FlagType.Setter) > 0)
                   && !found.InClass.IsVoid()
                   && found.InClass.Implements != null
                   && found.InClass.Implements.Count > 0;
        }

        /// <summary>
        /// Check if "Create new class" are available at the current cursor position.
        /// </summary>
        /// <param name="sci">The Scintilla control containing the document</param>
        /// <param name="position">Cursor position</param>
        /// <param name="expr">Expression at cursor position</param>
        /// <param name="found">Declaration target at current line(can not be null)</param>
        /// <returns>true, if can show "Create new class" list</returns>
        protected virtual bool CanShowGenerateClass(ScintillaControl sci, int position, ASResult expr, FoundDeclaration found)
        {
            // for example: public var foo : Fo|o
            return expr.Context.Separator == ":";
        }

        /// <summary>
        /// Check if "Create new interface" are available at the current cursor position.
        /// </summary>
        /// <param name="sci">The Scintilla control containing the document</param>
        /// <param name="position">Cursor position</param>
        /// <param name="expr">Expression at cursor position</param>
        /// <param name="found">Declaration target at current line(can not be null)</param>
        /// <returns>true, if can show "Create new interface" list</returns>
        protected virtual bool CanShowGenerateInterface(ScintillaControl sci, int position, ASResult expr, FoundDeclaration found)
        {
            return contextToken != null
                   && ASComplete.IsTextStyle(sci.BaseStyleAt(position - 1))
                   // fox example: implements IFoo<generator>
                   && ((expr.Context.WordBefore == "implements" && ASContext.Context.CodeComplete.PositionIsBeforeBody(sci, position, found.InClass))
                       // for example: public var foo : Fo|o
                       || (expr.Context.Separator == ":"));
        }

        private static MemberModel ResolveDelegate(string type, FileModel inFile)
        {
            foreach (MemberModel def in inFile.Members)
                if (def.Name == type && (def.Flags & FlagType.Delegate) > 0)
                    return def;

            if (!type.Contains('.'))
            {
                var dotType = '.' + type;
                var imports = ASContext.Context.ResolveImports(inFile);
                foreach (MemberModel import in imports)
                    if (import.Type.EndsWithOrdinal(dotType))
                    {
                        type = import.Type;
                        break;
                    }
            }

            var known = ASContext.Context.GetAllProjectClasses();
            foreach (MemberModel def in known)
                if (def.Type == type && (def.Flags & FlagType.Delegate) > 0)
                    return def;
            return null;
        }

        private static void GenerateDefaultHandlerName(ScintillaControl sci, int position, int targetPos, string eventName, bool closeBrace)
        {
            string target = null;
            int contextOwnerPos = GetContextOwnerEndPos(sci, sci.WordStartPosition(targetPos, true));
            if (contextOwnerPos != -1)
            {
                ASResult contextOwnerResult = ASComplete.GetExpressionType(sci, contextOwnerPos);
                if (contextOwnerResult != null && !contextOwnerResult.IsNull()
                    && contextOwnerResult.Member != null)
                {
                    if (contextOwnerResult.Member.Name == "contentLoaderInfo" && sci.CharAt(contextOwnerPos) == '.')
                    {
                        // we want to name the event from the loader var and not from the contentLoaderInfo parameter
                        contextOwnerPos = GetContextOwnerEndPos(sci, sci.WordStartPosition(contextOwnerPos - 1, true));
                        if (contextOwnerPos != -1)
                        {
                            contextOwnerResult = ASComplete.GetExpressionType(sci, contextOwnerPos);
                            if (contextOwnerResult != null && !contextOwnerResult.IsNull()
                                && contextOwnerResult.Member != null)
                            {
                                target = contextOwnerResult.Member.Name;
                            }
                        }
                    }
                    else
                    {
                        target = contextOwnerResult.Member.Name;
                    }
                }
            }
            
            eventName = Camelize(eventName.Substring(eventName.LastIndexOf('.') + 1));
            if (target != null) target = target.TrimStart('_');

            switch (ASContext.CommonSettings.HandlerNamingConvention)
            {
                case HandlerNamingConventions.handleTargetEventName:
                    if (target == null) contextToken = "handle" + Capitalize(eventName);
                    else contextToken = "handle" + Capitalize(target) + Capitalize(eventName);
                    break;
                case HandlerNamingConventions.onTargetEventName:
                    if (target == null) contextToken = "on" + Capitalize(eventName);
                    else contextToken = "on" + Capitalize(target) + Capitalize(eventName);
                    break;
                case HandlerNamingConventions.target_eventNameHandler:
                    if (target == null) contextToken = eventName + "Handler";
                    else contextToken = target + "_" + eventName + "Handler";
                    break;
                default: //HandlerNamingConventions.target_eventName
                    if (target == null) contextToken = eventName;
                    else contextToken = target + "_" + eventName;
                    break;
            }

            char c = (char)sci.CharAt(position - 1);
            if (c == ',') InsertCode(position, "$(Boundary) " + contextToken + "$(Boundary)", sci);
            else InsertCode(position, contextToken, sci);

            position = sci.WordEndPosition(position + 1, true);
            sci.SetSel(position, position);
            c = (char)sci.CharAt(position);
            if (c <= 32) sci.ReplaceSel(closeBrace ? ");" : ";");

            sci.SetSel(position, position);
        }

        public virtual FoundDeclaration GetDeclarationAtLine(int line)
        {
            var result = new FoundDeclaration();
            var model = ASContext.Context.CurrentModel;
            result.Member = GetMemberAtLine(model.Members.Items);
            if (result.Member == null)
            {
                result.InClass = (ClassModel) GetMemberAtLine(model.Classes);
                if (result.InClass != null) result.Member = GetMemberAtLine(result.InClass.Members.Items);
            }
            if (result.InClass == null) result.InClass = ClassModel.VoidClass;
            return result;

            // Utils
            MemberModel GetMemberAtLine(IEnumerable<MemberModel> list) => list.FirstOrDefault(it => it.LineFrom <= line && it.LineTo >= line);
        }

        protected bool CheckAutoImport(ASResult expr, List<ICompletionListItem> options)
        {
            if (ASContext.Context.CurrentClass.Equals(expr.RelClass)) return false;
            var allClasses = ASContext.Context.GetAllProjectClasses();
            if (allClasses != null)
            {
                var names = new HashSet<string>();
                var matches = new List<MemberModel>();
                var dotToken = "." + contextToken;
                foreach (MemberModel member in allClasses)
                    if (!names.Contains(member.Name) && member.Name.EndsWithOrdinal(dotToken))
                    {
                        matches.Add(member);
                        names.Add(member.Name);
                    }
                if (matches.Count > 0)
                {
                    ShowImportClass(matches, options);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// For the Event handlers generator:
        /// check that the event name's const is declared in an Event type
        /// </summary>
        internal static string CheckEventType(string name)
        {
            if (name.Contains('"')) return "Event";
            if (name.IndexOf('.') is int index && index > 0) name = name.Substring(0, index);
            var model = ASContext.Context.ResolveType(name, ASContext.Context.CurrentModel);
            if (model.IsVoid() || model.Name == "Event") return "Event";
            model.ResolveExtends();
            while (!model.IsVoid() && model.Name != "Event")
                model = model.Extends;
            if (model.Name == "Event") return name;
            return "Event";
        }

        protected virtual string CheckEventType(MemberModel handler, string eventName)
        {
            if (handler?.Parameters is List<MemberModel> parameters && parameters.Count > 1)
            {
                var parameter = parameters[1];
                if (parameter != null)
                {
                    if (parameter.Parameters != null && parameter.Parameters.Count > 0) return parameter.Parameters[0].Type;
                    if (parameter.Type is string type && type != "Function") return type;
                }
            }
            return CheckEventType(eventName);
        }
        #endregion

        #region generators lists

        private static void ShowImportClass(List<MemberModel> matches, ICollection<ICompletionListItem> options)
        {
            if (matches.Count == 1)
            {
                GenerateJob(GeneratorJobType.AddImport, matches[0], null, null, null);
                return;
            }
            
            foreach (MemberModel member in matches)
            {
                if ((member.Flags & FlagType.Class) > 0)
                    options.Add(new GeneratorItem("import " + member.Type, GeneratorJobType.AddImport, member, null));
                else if (member.IsPackageLevel)
                    options.Add(new GeneratorItem("import " + member.Name, GeneratorJobType.AddImport, member, null));
            }
        }

        private static void ShowPromoteLocalAndAddParameter(FoundDeclaration found, ICollection<ICompletionListItem> options)
        {
            string label = TextHelper.GetString("ASCompletion.Label.PromoteLocal");
            string labelMove = TextHelper.GetString("ASCompletion.Label.MoveDeclarationOnTop");
            string labelParam = TextHelper.GetString("ASCompletion.Label.AddAsParameter");
            options.Add(new GeneratorItem(label, GeneratorJobType.PromoteLocal, found.Member, found.InClass));
            options.Add(new GeneratorItem(labelMove, GeneratorJobType.MoveLocalUp, found.Member, found.InClass));
            options.Add(new GeneratorItem(labelParam, GeneratorJobType.AddAsParameter, found.Member, found.InClass));
        }

        private static void ShowConvertToConst(FoundDeclaration found, ICollection<ICompletionListItem> options)
        {
            string label = TextHelper.GetString("ASCompletion.Label.ConvertToConst");
            options.Add(new GeneratorItem(label, GeneratorJobType.ConvertToConst, found.Member, found.InClass));
        }

        private static void ShowImplementInterface(FoundDeclaration found, ICollection<ICompletionListItem> options)
        {
            string label = TextHelper.GetString("ASCompletion.Label.ImplementInterface");
            options.Add(new GeneratorItem(label, GeneratorJobType.ImplementInterface, null, found.InClass));
        }

        protected virtual void ShowNewVarList(ScintillaControl sci, ASResult expr, FoundDeclaration found, ICollection<ICompletionListItem> options)
        {
            if (expr.InClass == null || found.InClass.QualifiedName.Equals(expr.RelClass.QualifiedName))
                expr = null;
            ASResult exprLeft = null;
            var currentPos = sci.CurrentPos;
            var curWordStartPos = sci.WordStartPosition(currentPos, true);
            if ((char)sci.CharAt(curWordStartPos - 1) == '.') exprLeft = ASComplete.GetExpressionType(sci, curWordStartPos - 1);
            if (exprLeft != null && exprLeft.Type == null) exprLeft = null;
            var generateClass = true;
            if (exprLeft != null)
            {
                if (exprLeft.Type.InFile != null && !File.Exists(exprLeft.Type.InFile.FileName)) return;
                generateClass = false;
                var curClass = ASContext.Context.CurrentClass;
                if (!IsHaxe)
                {
                    if (exprLeft.Type.Equals(curClass)) exprLeft = null;
                }
                else 
                {
                    curClass.ResolveExtends();
                    while (!curClass.IsVoid())
                    {
                        if (curClass.Equals(exprLeft.Type))
                        {
                            exprLeft = null;
                            break;
                        }
                        curClass = curClass.Extends;
                    }
                }
            }

            var textAtCursor = sci.GetWordFromPosition(currentPos);
            string label;
            if (textAtCursor != null && textAtCursor.ToUpper().Equals(textAtCursor))
            {
                label = TextHelper.GetString("ASCompletion.Label.GenerateConstant");
                options.Add(new GeneratorItem(label, GeneratorJobType.Constant, found.Member, found.InClass));
            }

            bool genProtectedDecl = GetDefaultVisibility(found.InClass) == Visibility.Protected;
            if (expr == null && exprLeft == null)
            {
                if (genProtectedDecl) label = TextHelper.GetString("ASCompletion.Label.GenerateProtectedVar");
                else label = TextHelper.GetString("ASCompletion.Label.GeneratePrivateVar");
                options.Add(new GeneratorItem(label, GeneratorJobType.Variable, found.Member, found.InClass));
            }

            label = TextHelper.GetString("ASCompletion.Label.GeneratePublicVar");
            options.Add(new GeneratorItem(label, GeneratorJobType.VariablePublic, found.Member, found.InClass));

            if (expr == null && exprLeft == null)
            {
                if (genProtectedDecl) label = TextHelper.GetString("ASCompletion.Label.GenerateProtectedFunction");
                else label = TextHelper.GetString("ASCompletion.Label.GeneratePrivateFunction");
                options.Add(new GeneratorItem(label, GeneratorJobType.Function, found.Member, found.InClass));
            }

            label = TextHelper.GetString("ASCompletion.Label.GenerateFunctionPublic");
            options.Add(new GeneratorItem(label, GeneratorJobType.FunctionPublic, found.Member, found.InClass));

            if (generateClass)
            {
                label = TextHelper.GetString("ASCompletion.Label.GenerateClass");
                options.Add(new GeneratorItem(label, GeneratorJobType.Class, found.Member, found.InClass));
            }
        }

        private static void ShowChangeMethodDeclList(FoundDeclaration found, ICollection<ICompletionListItem> options)
        {
            string label = TextHelper.GetString("ASCompletion.Label.ChangeMethodDecl");
            options.Add(new GeneratorItem(label, GeneratorJobType.ChangeMethodDecl, found.Member, found.InClass));
        }

        private static void ShowChangeConstructorDeclarationList(FoundDeclaration found, IList<FunctionParameter> parameters, ICollection<ICompletionListItem> options)
        {
            var label = TextHelper.GetString("ASCompletion.Label.ChangeConstructorDecl");
            options.Add(new GeneratorItem(label, GeneratorJobType.ChangeConstructorDecl, found.Member, found.InClass, parameters));
        }

        protected virtual void ShowNewMethodList(ScintillaControl sci, ASResult expr, FoundDeclaration found, ICollection<ICompletionListItem> options)
        {
            if (expr.RelClass == null || found.InClass.QualifiedName.Equals(expr.RelClass.QualifiedName))
                expr = null;
            string label;
            var inClass = expr != null ? expr.RelClass : found.InClass;
            var isInterface = (inClass.Flags & FlagType.Interface) > 0;
            if (!isInterface && expr == null)
            {
                if (GetDefaultVisibility(found.InClass) == Visibility.Protected)
                    label = TextHelper.GetString("ASCompletion.Label.GenerateProtectedFunction");
                else label = TextHelper.GetString("ASCompletion.Label.GeneratePrivateFunction");
                options.Add(new GeneratorItem(label, GeneratorJobType.Function, found.Member, found.InClass));
            }
            if (isInterface) label = TextHelper.GetString("ASCompletion.Label.GenerateFunctionInterface");
            else label = TextHelper.GetString("ASCompletion.Label.GenerateFunctionPublic");
            options.Add(new GeneratorItem(label, GeneratorJobType.FunctionPublic, found.Member, found.InClass));
            label = TextHelper.GetString("ASCompletion.Label.GeneratePublicCallback");
            options.Add(new GeneratorItem(label, GeneratorJobType.VariablePublic, found.Member, found.InClass));
        }

        private static void ShowAssignStatementToVarList(FoundDeclaration found, StatementReturnType data, ICollection<ICompletionListItem> options)
        {
            var label = TextHelper.GetString("ASCompletion.Label.AssignStatementToVar");
            options.Add(new GeneratorItem(label, GeneratorJobType.AssignStatementToVar, found.Member, found.InClass, data));
        }

        private static void ShowGenerateClassList(FoundDeclaration found, ICollection<ICompletionListItem> options) => ShowGenerateClassList(found, null, options);

        private static void ShowGenerateClassList(FoundDeclaration found, ASExpr expr, ICollection<ICompletionListItem> options)
        {
            var label = TextHelper.GetString("ASCompletion.Label.GenerateClass");
            options.Add(new GeneratorItem(label, GeneratorJobType.Class, found.Member, found.InClass, expr));
        }

        private static void ShowGenerateInterfaceList(ASResult expr, FoundDeclaration found, ICollection<ICompletionListItem> options)
        {
            var label = TextHelper.GetString("ASCompletion.Label.GenerateInterface");
            options.Add(new GeneratorItem(label, GeneratorJobType.Interface, found.Member, found.InClass, expr));
        }

        private static void ShowConstructorAndToStringList(FoundDeclaration found, bool hasConstructor, bool hasToString, ICollection<ICompletionListItem> options)
        {
            if (!hasConstructor)
            {
                var label = TextHelper.GetString("ASCompletion.Label.GenerateConstructor");
                options.Add(new GeneratorItem(label, GeneratorJobType.Constructor, found.Member, found.InClass));
            }

            if (!hasToString)
            {
                var label = TextHelper.GetString("ASCompletion.Label.GenerateToString");
                options.Add(new GeneratorItem(label, GeneratorJobType.ToString, found.Member, found.InClass));
            }
        }

        private static void ShowEventMetatagList(FoundDeclaration found, ICollection<ICompletionListItem> options)
        {
            string label = TextHelper.GetString("ASCompletion.Label.GenerateEventMetatag");
            options.Add(new GeneratorItem(label, GeneratorJobType.EventMetatag, found.Member, found.InClass));
        }

        private static void ShowFieldFromParameter(FoundDeclaration found, ICollection<ICompletionListItem> options)
        {
            Hashtable parameters = new Hashtable();
            parameters["scope"] = GetDefaultVisibility(found.InClass);
            string label;
            if (GetDefaultVisibility(found.InClass) == Visibility.Protected)
                label = TextHelper.GetString("ASCompletion.Label.GenerateProtectedFieldFromParameter");
            else label = TextHelper.GetString("ASCompletion.Label.GeneratePrivateFieldFromParameter");
            options.Add(new GeneratorItem(label, GeneratorJobType.FieldFromParameter, found.Member, found.InClass, parameters));
            parameters = new Hashtable();
            parameters["scope"] = Visibility.Public;
            label = TextHelper.GetString("ASCompletion.Label.GeneratePublicFieldFromParameter");
            options.Add(new GeneratorItem(label, GeneratorJobType.FieldFromParameter, found.Member, found.InClass, parameters));
        }

        private static void ShowAddInterfaceDefList(FoundDeclaration found, IEnumerable<string> interfaces, ICollection<ICompletionListItem> options)
        {
            var label = TextHelper.GetString("ASCompletion.Label.AddInterfaceDef");
            foreach (var interf in interfaces)
            {
                options.Add(new GeneratorItem(String.Format(label, interf), GeneratorJobType.AddInterfaceDef, found.Member, found.InClass, interf));
            }
        }

        private static void ShowDelegateList(FoundDeclaration found, ICollection<ICompletionListItem> options)
        {
            string label = String.Format(TextHelper.GetString("ASCompletion.Label.GenerateHandler"), "Delegate");
            options.Add(new GeneratorItem(label, GeneratorJobType.Delegate, found.Member, found.InClass));
        }

        internal static void ShowEventList(FoundDeclaration found, List<ICompletionListItem> options)
        {
            var tmp = TextHelper.GetString("ASCompletion.Label.GenerateHandler");
            var labelEvent = string.Format(tmp, "Event");
            var labelContext = string.Format(tmp, contextParam);
            string[] choices = null;
            if (contextParam != "Event")
            {
                var type = ASContext.Context.ResolveType(contextParam, ASContext.Context.CurrentModel);
                type.ResolveExtends();
                while (!type.IsVoid())
                {
                    if (type.Name == "Event")
                    {
                        choices = new[] { labelContext, labelEvent };
                        break;
                    }
                    type = type.Extends;
                }
                if (choices == null) choices = new[] { labelContext };
            }
            else if (HasDataEvent())
            {
                var labelDataEvent = string.Format(tmp, "DataEvent");
                choices = new[] { labelEvent, labelDataEvent };
            }
            else choices = new[] { labelEvent };

            foreach (var choice in choices)
            {
                options.Add(new GeneratorItem(choice,
                    choice == labelContext ? GeneratorJobType.ComplexEvent : GeneratorJobType.BasicEvent,
                    found.Member, found.InClass));
            }
        }

        private static bool HasDataEvent()
        {
            return !ASContext.Context.ResolveType("flash.events.DataEvent", ASContext.Context.CurrentModel).IsVoid();
        }

        private static void ShowGetSetList(FoundDeclaration found, ICollection<ICompletionListItem> options)
        {
            string name = GetPropertyNameFor(found.Member);
            ASResult result = new ASResult();
            ClassModel curClass = ASContext.Context.CurrentClass;
            ASComplete.FindMember(name, curClass, result, FlagType.Getter, 0);
            bool hasGetter = !result.IsNull();
            ASComplete.FindMember(name, curClass, result, FlagType.Setter, 0);
            bool hasSetter = !result.IsNull();
            if (hasGetter && hasSetter) return;
            if (!hasGetter && !hasSetter)
            {
                string label = TextHelper.GetString("ASCompletion.Label.GenerateGetSet");
                options.Add(new GeneratorItem(label, GeneratorJobType.GetterSetter, found.Member, found.InClass));
            }
            ShowGetterList(found, options);
            ShowSetterList(found, options);
        }

        private static void ShowGetterList(FoundDeclaration found, ICollection<ICompletionListItem> options)
        {
            var name = GetPropertyNameFor(found.Member);
            var result = new ASResult();
            ASComplete.FindMember(name, ASContext.Context.CurrentClass, result, FlagType.Getter, 0);
            if (!result.IsNull()) return;
            var label = TextHelper.GetString("ASCompletion.Label.GenerateGet");
            options.Add(new GeneratorItem(label, GeneratorJobType.Getter, found.Member, found.InClass));
        }

        private static void ShowSetterList(FoundDeclaration found, ICollection<ICompletionListItem> options)
        {
            var name = GetPropertyNameFor(found.Member);
            var result = new ASResult();
            ASComplete.FindMember(name, ASContext.Context.CurrentClass, result, FlagType.Setter, 0);
            if (!result.IsNull()) return;
            var label = TextHelper.GetString("ASCompletion.Label.GenerateSet");
            options.Add(new GeneratorItem(label, GeneratorJobType.Setter, found.Member, found.InClass));
        }

        #endregion

        #region code generation

        public static void SetJobContext(String contextToken, String contextParam, MemberModel contextMember, Match contextMatch)
        {
            ASGenerator.contextToken = contextToken;
            ASGenerator.contextParam = contextParam;
            ASGenerator.contextMember = contextMember;
            ASGenerator.contextMatch = contextMatch;
        }

        public static void GenerateJob(GeneratorJobType job, MemberModel member, ClassModel inClass, string itemLabel, object data)
        {
            var sci = ASContext.CurSciControl;
            lookupPosition = sci.CurrentPos;

            int position;
            MemberModel latest;
            bool detach = true;
            switch (job)
            {
                case GeneratorJobType.Getter:
                case GeneratorJobType.Setter:
                case GeneratorJobType.GetterSetter:
                    var generator = ((ASGenerator) ASContext.Context.CodeGenerator);
                    var strategy = generator.GetCodeGeneratorBehavior();
                    if (strategy != null)
                    {
                        ((CodeGeneratorInterfaceBehavior) strategy).GenerateProperty(job, sci, member, inClass);
                        return;
                    }
                    // default behavior
                    generator.GenerateProperty(job, member, inClass, sci);
                    break;

                case GeneratorJobType.BasicEvent:
                case GeneratorJobType.ComplexEvent:
                    latest = TemplateUtils.GetTemplateBlockMember(sci, TemplateUtils.GetBoundary("EventHandlers"));
                    if (latest == null)
                    {
                        if (ASContext.CommonSettings.MethodsGenerationLocations == MethodsGenerationLocations.AfterSimilarAccessorMethod)
                            latest = GetLatestMemberForFunction(inClass, GetDefaultVisibility(inClass), member);
                        if (latest == null)
                            latest = member;
                    }

                    position = sci.PositionFromLine(latest.LineTo + 1) - (sci.EOLMode == 0 ? 2 : 1);
                    sci.SetSel(position, position);
                    string type = contextParam;
                    if (job == GeneratorJobType.BasicEvent)
                        type = itemLabel.Contains("DataEvent") ? "DataEvent" : "Event";
                    GenerateEventHandler(contextToken, type, member, position, inClass);
                    break;

                case GeneratorJobType.Delegate:
                    position = sci.PositionFromLine(member.LineTo + 1) - ((sci.EOLMode == 0) ? 2 : 1);
                    sci.SetSel(position, position);
                    GenerateDelegateMethod(contextToken, member, position, inClass);
                    break;

                case GeneratorJobType.Constant:
                case GeneratorJobType.Variable:
                case GeneratorJobType.VariablePublic:
                    sci.BeginUndoAction();
                    try
                    {
                        GenerateVariableJob(job, sci, member, detach, inClass);
                    }
                    finally
                    {
                        sci.EndUndoAction();
                    }
                    break;

                case GeneratorJobType.Constructor:
                    sci.BeginUndoAction();
                    try
                    {
                        GenerateConstructorJob(sci, inClass);
                    }
                    finally
                    {
                        sci.EndUndoAction();
                    }
                    break;

                case GeneratorJobType.ChangeConstructorDecl:
                    sci.BeginUndoAction();
                    try
                    {
                        var parameters = data as IList<FunctionParameter>;
                        if (parameters == null) ChangeConstructorDecl(sci, inClass);
                        else ChangeConstructorDecl(sci, inClass, parameters);
                    }
                    finally
                    {
                        sci.EndUndoAction();
                    }
                    break;

                case GeneratorJobType.Function:
                case GeneratorJobType.FunctionPublic:
                    sci.BeginUndoAction();
                    try
                    {
                        GenerateFunctionJob(job, sci, member, true, inClass);
                    }
                    finally
                    {
                        sci.EndUndoAction();
                    }
                    break;

                case GeneratorJobType.ImplementInterface:
                    var iType = ASContext.Context.ResolveType(contextParam, inClass.InFile ?? ASContext.Context.CurrentModel);
                    if (iType.IsVoid()) return;
                    latest = GetLatestMemberForFunction(inClass, Visibility.Public, null) ?? FindLatest(0, 0, inClass, false, false);
                    if (latest == null)
                    {
                        position = GetBodyStart(inClass.LineFrom, inClass.LineTo, sci);
                        detach = false;
                    }
                    else position = sci.PositionFromLine(latest.LineTo + 1) - ((sci.EOLMode == 0) ? 2 : 1);
                    sci.SetSel(position, position);
                    GenerateImplementation(iType, inClass, sci, detach);
                    break;

                case GeneratorJobType.MoveLocalUp:
                    sci.BeginUndoAction();
                    try
                    {
                        if (!RemoveLocalDeclaration(sci, contextMember)) return;

                        position = GetBodyStart(member.LineFrom, member.LineTo, sci);
                        sci.SetSel(position, position);

                        string varType = contextMember.Type;
                        if (varType == "") varType = null;

                        string template = TemplateUtils.GetTemplate("Variable");
                        template = TemplateUtils.ReplaceTemplateVariable(template, "Name", contextMember.Name);
                        template = TemplateUtils.ReplaceTemplateVariable(template, "Type", varType);
                        template = TemplateUtils.ReplaceTemplateVariable(template, "Modifiers", null);
                        template = TemplateUtils.ReplaceTemplateVariable(template, "Value", null);
                        template += "\n$(Boundary)";

                        lookupPosition += SnippetHelper.InsertSnippetText(sci, position, template);

                        sci.SetSel(lookupPosition, lookupPosition);
                    }
                    finally
                    {
                        sci.EndUndoAction();
                    }
                    break;

                case GeneratorJobType.PromoteLocal:
                    sci.BeginUndoAction();
                    try
                    {
                        if (!RemoveLocalDeclaration(sci, contextMember)) return;
                        latest = GetLatestMemberForVariable(GeneratorJobType.Variable, inClass, GetDefaultVisibility(inClass), member);
                        if (latest == null) return;
                        position = FindNewVarPosition(sci, inClass, latest);
                        if (position <= 0) return;
                        sci.SetSel(position, position);
                        var newMember = new MemberModel
                        {
                            Name = contextMember.Name,
                            Type = contextMember.Type,
                            Access = GetDefaultVisibility(inClass)
                        };
                        if ((member.Flags & FlagType.Static) > 0) newMember.Flags |= FlagType.Access;
                        // for example: var f<generator>:Function/*(v1:Type):void*/
                        if ((contextMember.Flags & FlagType.Function) != 0)
                        {
                            newMember.Type = ((ASGenerator) ASContext.Context.CodeGenerator).FunctionToString(new MemberModel {Parameters = contextMember.Parameters, Type = newMember.Type});
                        }
                        GenerateVariable(newMember, position, detach);
                        sci.SetSel(lookupPosition, lookupPosition);
                        if (ASContext.Context.Settings.GenerateImports)
                        {
                            var imports = new List<string> {newMember.Type};
                            var types = GetQualifiedTypes(imports, inClass.InFile);
                            lookupPosition += AddImportsByName(types, sci.LineFromPosition(lookupPosition));
                            sci.SetSel(lookupPosition, lookupPosition);
                        }
                    }
                    finally
                    {
                        sci.EndUndoAction();
                    }
                    break;

                case GeneratorJobType.AddAsParameter:
                    sci.BeginUndoAction();
                    try
                    {
                        AddAsParameter(sci, member);
                    }
                    finally
                    {
                        sci.EndUndoAction();
                    }
                    break;

                case GeneratorJobType.AddImport:
                    position = sci.CurrentPos;
                    if ((member.Flags & (FlagType.Class | FlagType.Enum | FlagType.Struct | FlagType.TypeDef)) == 0)
                    {
                        if (member.InFile == null) break;
                        member.Type = member.Name;
                    }
                    sci.BeginUndoAction();
                    try
                    {
                        position += InsertImport(member, true);
                        sci.SetSel(position, position);
                    }
                    finally
                    {
                        sci.EndUndoAction();
                    }
                    break;

                case GeneratorJobType.Class:
                    if (data is ASExpr) GenerateClass(sci, inClass, (ASExpr) data);
                    else GenerateClass(sci, inClass, sci.GetWordFromPosition(sci.CurrentPos));
                    break;

                case GeneratorJobType.Interface:
                    GenerateInterface(inClass, contextToken);
                    break;

                case GeneratorJobType.ToString:
                    sci.BeginUndoAction();
                    try
                    {
                        GenerateToString(sci, inClass);
                    }
                    finally
                    {
                        sci.EndUndoAction();
                    }
                    break;

                case GeneratorJobType.FieldFromParameter:
                    sci.BeginUndoAction();
                    try
                    {
                        GenerateFieldFromParameter(sci, member, inClass, (Visibility) (((Hashtable) data)["scope"]));
                    }
                    finally
                    {
                        sci.EndUndoAction();
                    }
                    break;

                case GeneratorJobType.AddInterfaceDef:
                    sci.BeginUndoAction();
                    try
                    {
                        AddInterfaceDefJob(sci, member, inClass, (string)data);
                    }
                    finally
                    {
                        sci.EndUndoAction();
                    }
                    break;

                case GeneratorJobType.ConvertToConst:
                    sci.BeginUndoAction();
                    try
                    {
                        ConvertToConst(sci, member, inClass, detach);
                    }
                    finally
                    {
                        sci.EndUndoAction();
                    }
                    break;

                case GeneratorJobType.ChangeMethodDecl:
                    sci.BeginUndoAction();
                    try
                    {
                        ChangeMethodDecl(sci, inClass);
                    }
                    finally
                    {
                        sci.EndUndoAction();
                    }
                    break;

                case GeneratorJobType.EventMetatag:
                    sci.BeginUndoAction();
                    try
                    {
                        EventMetatag(sci, inClass);
                    }
                    finally
                    {
                        sci.EndUndoAction();
                    }
                    break;

                case GeneratorJobType.AssignStatementToVar:
                    sci.BeginUndoAction();
                    try
                    {
                        if (data is StatementReturnType returnType) AssignStatementToVar(sci, inClass, returnType);
                        else AssignStatementToVar(sci, inClass);
                    }
                    finally
                    {
                        sci.EndUndoAction();
                    }
                    break;
            }
        }

        protected virtual void GenerateProperty(GeneratorJobType job, MemberModel member, ClassModel inClass, ScintillaControl sci)
        {
            var ctx = ASContext.Context;
            var name = GetPropertyNameFor(member);
            var location = ASContext.CommonSettings.PropertiesGenerationLocation;
            var latest = TemplateUtils.GetTemplateBlockMember(sci, TemplateUtils.GetBoundary("AccessorsMethods"));
            if (latest != null) location = PropertiesGenerationLocations.AfterLastPropertyDeclaration;
            else
            {
                if (location == PropertiesGenerationLocations.AfterLastPropertyDeclaration)
                {
                    if (job == GeneratorJobType.Getter || job == GeneratorJobType.Setter)
                        latest = FindMember(name ?? member.Name, inClass);
                    if (latest == null) latest = FindLatest(FlagType.Getter | FlagType.Setter, 0, inClass, false, false);
                }
                else latest = member;
            }
            if (latest == null) return;
            sci.BeginUndoAction();
            try
            {
                if ((member.Access & Visibility.Public) > 0) MakePrivate(sci, member, inClass);
                if (name == null) // rename var with starting underscore
                {
                    name = member.Name;
                    var newName = GetNewPropertyNameFor(member);
                    if (RenameMember(sci, member, newName)) member.Name = newName;
                }
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
                // for example: private var foo<generator>:Function/*(v1:*):void*/
                if ((member.Flags & FlagType.Function) != 0)
                {
                    member = (MemberModel) member.Clone();
                    member.Type = ((ASGenerator) ctx.CodeGenerator).FunctionToString(member);
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

        static void AssignStatementToVar(ScintillaControl sci, ClassModel inClass)
        {
            var currentLine = sci.CurrentLine;
            var returnType = GetStatementReturnType(sci, inClass, sci.GetLine(currentLine), sci.PositionFromLine(currentLine));
            AssignStatementToVar(sci, inClass, returnType);
        }
        
        static void AssignStatementToVar(ScintillaControl sci, ClassModel inClass, StatementReturnType returnType)
        {
            var ctx = inClass.InFile.Context;
            var resolve = returnType.Resolve;
            string type = null;
            List<ASResult> expressions = null;
            var context = resolve.Context;
            if (context != null)
            {
                // for example: typeof v, delete o[k], ...
                if (((ASGenerator) ctx.CodeGenerator).AssignStatementToVar(sci, inClass, context)) return;
                // for example: 1 + 1, 1 << 1, ...
                var operators = ctx.Features.ArithmeticOperators
                    .Select(it => it.ToString())
                    .Concat(ctx.Features.IncrementDecrementOperators)
                    .Concat(ctx.Features.BitwiseOperators)
                    .Concat(ctx.Features.BooleanOperators)
                    .ToHashSet();
                var sep = new[] {' '};
                var isValid = new Func<ASExpr, bool>((c) => c.Separator.Contains(' ') 
                    && c.Separator.Split(sep, StringSplitOptions.RemoveEmptyEntries).Any(it => operators.Contains(it.Trim())));
                if (operators.Contains(context.Separator) || operators.Contains(context.RightOperator) || isValid(context))
                {
                    if (context.Separator == "/" || context.Separator == "%") type = ctx.Features.numberKey;
                    var current = resolve;
                    context = current.Context;
                    expressions = new List<ASResult> {current};
                    var rop = false;
                    while (operators.Contains(context.Separator) || (rop = operators.Contains(context.RightOperator)) || isValid(context))
                    {
                        if (type == null && (context.Separator == "/" || context.Separator == "%")) type = ctx.Features.numberKey;
                        var position = rop ? context.PositionExpression : context.SeparatorPosition;
                        current = ASComplete.GetExpressionType(sci, position, false, true);
                        if (current == null || current.IsNull()) break;
                        expressions.Add(current);
                        context = current.Context;
                        rop = false;
                    }
                }
            }
            int pos;
            if (expressions == null) pos = GetStartOfStatement(resolve);
            else
            {
                var last = expressions.Last();
                pos = last.Context.Separator != ";" ? last.Context.SeparatorPosition : last.Context.PositionExpression;
                var first = expressions.First();
                if (ctx.Features.BooleanOperators.Contains(first.Context.Separator)) type = ctx.Features.booleanKey;
            }
            if (type == null
                && resolve.Member == null && resolve.Type.Flags.HasFlag(FlagType.Class)
                && resolve.Type.Name != ctx.Features.booleanKey
                && resolve.Type.Name != "Function"
                && !string.IsNullOrEmpty(resolve.Path) && !char.IsDigit(resolve.Path[0]))
            {
                var expr = ASComplete.GetExpression(sci, returnType.Position);
                if (string.IsNullOrEmpty(expr.WordBefore))
                {
                    var characters = ScintillaControl.Configuration.GetLanguage(ctx.Settings.LanguageId.ToLower()).characterclass.Characters;
                    if (resolve.Path.All(it => characters.Contains(it)))
                    {
                        if (inClass.InFile.haXe) type = "Class<Dynamic>";
                        else type = ctx.ResolveType("Class", resolve.InFile).QualifiedName;
                    }
                }
            }

            var word = returnType.Word;
            if (!string.IsNullOrEmpty(word) && char.IsDigit(word[0])) word = null;
            string varname = null;
            if (string.IsNullOrEmpty(type) && !resolve.IsNull())
            {
                if (resolve.Member?.Type != null) type = resolve.Member.Type;
                else if (resolve.Type?.Name != null)
                {
                    type = resolve.Type.QualifiedName;
                    if (resolve.Type.IndexType == "*") type += ".<*>";
                    else if (resolve.Type.FullName.Contains(".<Vector>")) type = resolve.Type.FullName.Replace(".<Vector>", ".<Vector.<*>>");
                }

                if (resolve.Member?.Name != null) varname = GuessVarName(resolve.Member.Name, type);
            }
            if (!string.IsNullOrEmpty(word) && (string.IsNullOrEmpty(type) || Regex.IsMatch(type, "(<[^]]+>)"))) word = null;
            if (type == ctx.Features.voidKey) type = null;
            if (varname == null) varname = GuessVarName(word, type);
            if (varname != null && varname == word) varname = varname.Length == 1 ? varname + "1" : varname[0] + "";
            ((ASGenerator) ctx.CodeGenerator).AssignStatementToVar(sci, pos, varname, type);
        }

        protected virtual void AssignStatementToVar(ScintillaControl sci, int position, string name, string type)
        {
            var template = TemplateUtils.GetTemplate("AssignVariable");
            template = TemplateUtils.ReplaceTemplateVariable(template, "Name", AvoidKeyword(name));
            template = TemplateUtils.ReplaceTemplateVariable(template, "Type", GetShortType(type));
            sci.SetSel(position, position);
            InsertCode(position, template, sci);
            if (ASContext.Context.Settings.GenerateImports && type != null)
            {
                var types = GetQualifiedTypes(new [] {type}, ASContext.Context.CurrentModel);
                position += AddImportsByName(types, sci.LineFromPosition(position));
                sci.SetSel(position, position);
            }
        }

        protected virtual bool AssignStatementToVar(ScintillaControl sci, ClassModel inClass, ASExpr expr)
        {
            var ctx = inClass.InFile.Context;
            ClassModel type;
            switch (expr.WordBefore)
            {
                case "typeof":
                    type = ctx.ResolveType(ctx.Features.stringKey, inClass.InFile);
                    break;
                case "delete":
                    type = ctx.ResolveType(ctx.Features.booleanKey, inClass.InFile);
                    break;
                default:
                    return false;
            }
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

        internal static string AvoidKeyword(string word)
        {
            var features = ASContext.Context.Features;
            return features.accessKeywords.Contains(word)
                   || features.codeKeywords.Contains(word)
                   || features.declKeywords.Contains(word)
                   || features.typesKeywords.Contains(word)
                   || features.typesPreKeys.Contains(word)
                   || features.Literals.Contains(word)
                ? $"{word}Value"
                : word;
        }

        private static void EventMetatag(ScintillaControl sci, ClassModel inClass)
        {
            ASResult resolve = ASComplete.GetExpressionType(sci, sci.WordEndPosition(sci.CurrentPos, true));
            string line = sci.GetLine(inClass.LineFrom);
            int position = sci.PositionFromLine(inClass.LineFrom) + (line.Length - line.TrimStart().Length);

            var value = resolve.Member.Value;
            if (value != null)
            {
                if (value.StartsWith('\"')) value = value.Trim('"');
                else if (value.StartsWith('\'')) value = value.Trim('\'');
            }
            else value = resolve.Member.Type;
            if (string.IsNullOrEmpty(value)) return;

            Regex re1 = new Regex("'(?:[^'\\\\]|(?:\\\\\\\\)|(?:\\\\\\\\)*\\\\.{1})*'");
            Regex re2 = new Regex("\"(?:[^\"\\\\]|(?:\\\\\\\\)|(?:\\\\\\\\)*\\\\.{1})*\"");
            Match m1 = re1.Match(value);
            Match m2 = re2.Match(value);

            if (m1.Success || m2.Success)
            {
                Match m = null;
                if (m1.Success && m2.Success) m = m1.Index > m2.Index ? m2 : m1;
                else if (m1.Success) m = m1;
                else m = m2;
                value = value.Substring(m.Index + 1, m.Length - 2);
            }

            string template = TemplateUtils.GetTemplate("EventMetatag");
            template = TemplateUtils.ReplaceTemplateVariable(template, "Name", value);
            template = TemplateUtils.ReplaceTemplateVariable(template, "Type", contextParam);
            template += "\n$(Boundary)";

            AddLookupPosition();

            sci.SetSel(position, position);
            InsertCode(position, template, sci);
        }

        private static void ConvertToConst(ScintillaControl sci, MemberModel member, ClassModel inClass, bool detach)
        {
            var suggestion = "NEW_CONST";
            var label = TextHelper.GetString("ASCompletion.Label.ConstName");
            var title = TextHelper.GetString("ASCompletion.Title.ConvertToConst");

            Hashtable info = new Hashtable();
            info["suggestion"] = suggestion;
            info["label"] = label;
            info["title"] = title;
            var de = new DataEvent(EventType.Command, "ProjectManager.LineEntryDialog", info);
            EventManager.DispatchEvent(null, de);
            if (!de.Handled) return;

            int position = sci.CurrentPos;
            int style = sci.BaseStyleAt(position);

            int wordPosEnd = position + 1;
            int wordPosStart = position;

            while (sci.BaseStyleAt(wordPosEnd) == style) wordPosEnd++;
            while (sci.BaseStyleAt(wordPosStart - 1) == style) wordPosStart--;
            
            sci.SetSel(wordPosStart, wordPosEnd);
            var word = sci.SelText;
            suggestion = (string)info["suggestion"];
            sci.ReplaceSel(suggestion);
            
            if (member == null)
            {
                detach = false;
                lookupPosition = -1;
                position = sci.WordStartPosition(sci.CurrentPos, true);
                sci.SetSel(position, sci.WordEndPosition(position, true));
            }
            else
            {
                var latest = GetLatestMemberForVariable(GeneratorJobType.Constant, inClass, Visibility.Private, new MemberModel("", "", FlagType.Static, 0));
                if (latest != null)
                {
                    if (!member.Flags.HasFlag(FlagType.Function) && sci.LineFromPosition(wordPosStart) is int line && latest.LineFrom >= line)
                    {
                        position = sci.LineIndentPosition(line);
                        sci.SetSel(position, position);
                        sci.NewLine();
                        detach = false;
                    }
                    else position = FindNewVarPosition(sci, inClass, latest);
                }
                else
                {
                    position = GetBodyStart(inClass.LineFrom, inClass.LineTo, sci);
                    detach = false;
                }
                if (position <= 0) return;
                sci.SetSel(position, position);
            }

            var m = NewMember(suggestion, member, FlagType.Variable | FlagType.Constant | FlagType.Static, GetDefaultVisibility(inClass));
            switch (style)
            {
                case 4:
                    m.Type = ASContext.Context.Features.numberKey;
                    break;
                case 6:
                case 7:
                    m.Type = ASContext.Context.Features.stringKey;
                    break;
            }

            m.Value = word;
            GenerateVariable(m, position, detach);
        }

        private static void ChangeMethodDecl(ScintillaControl sci, ClassModel inClass)
        {
            int wordPos = sci.WordEndPosition(sci.CurrentPos, true);
            var parameters = ParseFunctionParameters(sci, wordPos);

            ASResult funcResult = ASComplete.GetExpressionType(sci, sci.WordEndPosition(sci.CurrentPos, true));
            if (funcResult?.Member == null) return;
            if (funcResult.InClass != null && !funcResult.InClass.Equals(inClass))
            {
                AddLookupPosition();
                lookupPosition = -1;

                ASContext.MainForm.OpenEditableDocument(funcResult.InClass.InFile.FileName, true);
                sci = ASContext.CurSciControl;
                var fileModel = ASContext.Context.GetCodeModel(sci.Text);
                foreach (ClassModel cm in fileModel.Classes)
                {
                    if (cm.QualifiedName.Equals(funcResult.InClass.QualifiedName))
                    {
                        funcResult.InClass = cm;
                        break;
                    }
                }
                inClass = funcResult.InClass;

                ASContext.Context.UpdateContext(inClass.LineFrom);
            }

            MemberList members = inClass.Members;
            foreach (MemberModel m in members)
            {
                if (m.Equals(funcResult.Member))
                {
                    funcResult.Member = m;
                    break;
                }
            }

            ChangeDecl(sci, inClass, funcResult.Member, parameters);
        }

        private static void ChangeConstructorDecl(ScintillaControl sci, ClassModel inClass)
        {
            var position = sci.WordEndPosition(sci.CurrentPos, true);
            var parameters = ParseFunctionParameters(sci, position);
            ChangeConstructorDecl(sci, inClass, parameters);
        }

        private static void ChangeConstructorDecl(ScintillaControl sci, ClassModel inClass, IList<FunctionParameter> parameters)
        {
            var funcResult = ASComplete.GetExpressionType(sci, sci.WordEndPosition(sci.CurrentPos, true));
            if (funcResult?.Type == null) return;
            if (!funcResult.Type.Equals(inClass))
            {
                AddLookupPosition();
                lookupPosition = -1;

                ASContext.MainForm.OpenEditableDocument(funcResult.Type.InFile.FileName, true);
                sci = ASContext.CurSciControl;
                var fileModel = ASContext.Context.GetFileModel(funcResult.Type.InFile.FileName);
                foreach (ClassModel cm in fileModel.Classes)
                {
                    if (cm.QualifiedName.Equals(funcResult.Type.QualifiedName))
                    {
                        funcResult.Type = cm;
                        break;
                    }
                }

                inClass = funcResult.Type;
                ASContext.Context.UpdateContext(inClass.LineFrom);
            }

            foreach (MemberModel m in inClass.Members)
            {
                if ((m.Flags & FlagType.Constructor) > 0)
                {
                    funcResult.Member = m;
                    break;
                }
            }

            if (funcResult.Member == null) return;
            if (!string.IsNullOrEmpty(ASContext.Context.Features.ConstructorKey)) funcResult.Member.Name = ASContext.Context.Features.ConstructorKey;

            ChangeDecl(sci, inClass, funcResult.Member, parameters);
        }

        private static void ChangeDecl(ScintillaControl sci, ClassModel inClass, MemberModel memberModel, IList<FunctionParameter> functionParameters)
        {
            bool paramsDiffer = false;
            if (memberModel.Parameters != null)
            {
                // check that parameters have one and the same type
                if (memberModel.Parameters.Count == functionParameters.Count)
                {
                    if (functionParameters.Count > 0)
                    {
                        List<MemberModel> parameters = memberModel.Parameters;
                        for (int i = 0; i < parameters.Count; i++)
                        {
                            MemberModel p = parameters[i];
                            if (p.Type != functionParameters[i].paramType)
                            {
                                paramsDiffer = true;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    paramsDiffer = true;
                }
            }
            // check that parameters count differs
            else if (functionParameters.Count != 0)
            {
                paramsDiffer = true;
            }

            if (!paramsDiffer) return;
            int app = 0;
            var newParameters = new List<MemberModel>();
            var existingParameters = memberModel.Parameters;
            for (int i = 0; i < functionParameters.Count; i++)
            {
                FunctionParameter p = functionParameters[i];
                if (existingParameters != null
                    && existingParameters.Count > (i - app)
                    && existingParameters[i - app].Type == p.paramType)
                {
                    newParameters.Add(existingParameters[i - app]);
                }
                else
                {
                    if (existingParameters != null && existingParameters.Count < functionParameters.Count) app++;
                    newParameters.Add(new MemberModel(AvoidKeyword(p.paramName), GetShortType(p.paramQualType), FlagType.ParameterVar, 0));
                }
            }

            memberModel.Parameters = newParameters;

            var posStart = sci.PositionFromLine(memberModel.LineFrom);
            var posEnd = sci.LineEndPosition(memberModel.LineTo);
            sci.SetSel(posStart, posEnd);
            var selectedText = sci.SelText;
            var rStart = new Regex(@"\s{1}" + memberModel.Name + @"\s*\(([^\)]*)\)(\s*:\s*([^({{|\n|\r|\s|;)]+))?");
            var mStart = rStart.Match(selectedText);
            if (!mStart.Success) return;

            var start = mStart.Index + posStart;
            var end = start + mStart.Length;

            sci.SetSel(start, end);

            var decl = TemplateUtils.ToDeclarationString(memberModel, TemplateUtils.GetTemplate("MethodDeclaration"));
            InsertCode(sci.CurrentPos, "$(Boundary) " + decl, sci);

            // add imports to function argument types
            if (ASContext.Context.Settings.GenerateImports && functionParameters.Count > 0)
            {
                var l = new string[functionParameters.Count];
                for (var i = 0; i < functionParameters.Count; i++)
                {
                    l[i] = functionParameters[i].paramQualType;
                }
                var types = GetQualifiedTypes(l, inClass.InFile);
                start += AddImportsByName(types, sci.LineFromPosition(end));
            }
            sci.SetSel(start, start);
        }

        private static void AddAsParameter(ScintillaControl sci, MemberModel member)
        {
            if (!RemoveLocalDeclaration(sci, contextMember)) return;
            var posStart = sci.PositionFromLine(member.LineFrom);
            var posEnd = sci.LineEndPosition(member.LineTo);
            sci.SetSel(posStart, posEnd);
            var rStart = new Regex(@"\s{1}" + member.Name + @"\s*\(([^\)]*)\)(\s*:\s*([^({{|\n|\r|\s|;)]+))?");
            var mStart = rStart.Match(sci.SelText);
            if (!mStart.Success) return;
            var start = mStart.Index + posStart + 1;
            var end = mStart.Index + posStart + mStart.Length;
            sci.SetSel(start, end);
            var memberCopy = (MemberModel) member.Clone();
            if (memberCopy.Parameters == null) memberCopy.Parameters = new List<MemberModel>();
            if ((contextMember.Flags & FlagType.Function) != 0 && contextMember.Parameters != null)
            {
                var parameter = (MemberModel) contextMember.Clone();
                parameter.Type = ((ASGenerator) ASContext.Context.CodeGenerator).FunctionToString(parameter);
                memberCopy.Parameters.Add(parameter);
            }
            else memberCopy.Parameters.Add(contextMember);
            var template = TemplateUtils.ToDeclarationString(memberCopy, TemplateUtils.GetTemplate("MethodDeclaration"));
            InsertCode(start, template, sci);
            var position = sci.LineEndPosition(sci.CurrentLine);
            sci.SetSel(position, position);
            if (ASContext.Context.Settings.GenerateImports)
            {
                var imports = new List<string> {contextMember.Type};
                var types = GetQualifiedTypes(imports, ASContext.Context.CurrentModel);
                position += AddImportsByName(types, sci.LineFromPosition(position));
                sci.SetSel(position, position);
            }
        }

        private static void AddInterfaceDefJob(ScintillaControl sci, MemberModel member, ClassModel inClass, string interf)
        {
            var ctx = ASContext.Context;
            var aType = ctx.ResolveType(interf, ctx.CurrentModel);
            if (aType.IsVoid()) return;
            var fileModel = ctx.GetFileModel(aType.InFile.FileName);
            foreach (var cm in fileModel.Classes)
            {
                if (!cm.QualifiedName.Equals(aType.QualifiedName)) continue;
                aType = cm;
                break;
            }
            var codeGenerator = (ASGenerator) ctx.CodeGenerator;
            var template = codeGenerator.GetAddInterfaceDefTemplate(member);
            ASContext.MainForm.OpenEditableDocument(aType.InFile.FileName, true);
            sci = ASContext.CurSciControl;
            var latest = GetLatestMemberForFunction(aType, Visibility.Default, new MemberModel());
            int position;
            if (latest == null) position = GetBodyStart(aType.LineFrom, aType.LineTo, sci);
            else
            {
                position = sci.PositionFromLine(latest.LineTo + 1) - ((sci.EOLMode == 0) ? 2 : 1);
                template = NewLine + template;
            }
            string type = null;
            if ((member.Flags & FlagType.Setter) != 0)
            {
                if (member.Parameters != null && member.Parameters.Count > 0)
                {
                    var parameter = member.Parameters[0];
                    if ((parameter.Flags & FlagType.Function) != 0) type = codeGenerator.FunctionToString(parameter);
                    else type = parameter.Type;
                }
                if (type == null) type = member.Type;
                if (type == ctx.Features.voidKey) type = ctx.Features.dynamicKey;
            }
            else type = member.Type ?? ctx.Features.voidKey;
            sci.SetSel(position, position);
            template = TemplateUtils.ReplaceTemplateVariable(template, "Type", type);
            template = TemplateUtils.ToDeclarationString(member, template);
            template = TemplateUtils.ReplaceTemplateVariable(template, "BlankLine", NewLine);
            template = TemplateUtils.ReplaceTemplateVariable(template, "Void", ctx.Features.voidKey);
            if (ctx.Settings.GenerateImports)
            {
                var imports = new List<string>();
                var parameters = member.Parameters;
                if (parameters != null && parameters.Count > 0) imports.AddRange(from t in parameters where t.Type != null select t.Type);
                if (member.Type != null) imports.Add(member.Type);
                if (imports.Count > 0)
                {
                    var types = GetQualifiedTypes(imports, inClass.InFile);
                    position += AddImportsByName(types, sci.LineFromPosition(position));
                }
            }
            sci.SetSel(position, position);
            InsertCode(position, template, sci);
        }

        protected virtual string GetAddInterfaceDefTemplate(MemberModel member)
        {
            if ((member.Flags & FlagType.Getter) > 0) return TemplateUtils.GetTemplate("IGetter");
            if ((member.Flags & FlagType.Setter) > 0) return TemplateUtils.GetTemplate("ISetter");
            return TemplateUtils.GetTemplate("IFunction");
        }

        private static void GenerateFieldFromParameter(ScintillaControl sci, MemberModel member, ClassModel inClass, Visibility scope)
        {
            var bodyStart = GetBodyStart(member.LineFrom, member.LineTo, sci, false);
            var fbsLine = sci.LineFromPosition(bodyStart);
            var endPos = sci.LineEndPosition(member.LineTo);

            sci.SetSel(bodyStart, endPos);
            var body = sci.SelText;
            var trimmed = body.TrimStart();

            var m = reSuperCall.Match(trimmed);
            if (m.Success && m.Index == 0) bodyStart = GetEndOfStatement(bodyStart + (body.Length - trimmed.Length), endPos, sci);

            bodyStart = GetOrSetPointOfInsertion(bodyStart, endPos, fbsLine, sci);

            sci.SetSel(bodyStart, bodyStart);

            var paramName = contextMember.Name;
            var paramType = contextMember.Type;
            paramType = ((ASGenerator) ASContext.Context.CodeGenerator).GetFieldTypeFromParameter(paramType, ref paramName);

            var varName = paramName;
            var scopedVarName = varName;

            if ((scope & Visibility.Public) > 0)
            {
                if ((member.Flags & FlagType.Static) > 0) scopedVarName = inClass.Name + "." + varName;
                else scopedVarName = "this." + varName;
            }
            else
            {
                var prefixFields = ASContext.CommonSettings.PrefixFields;
                if (prefixFields.Length > 0 && !varName.StartsWithOrdinal(prefixFields))
                {
                    scopedVarName = varName = prefixFields + varName;
                }

                if (ASContext.CommonSettings.GenerateScope || prefixFields == "")
                {
                    if ((member.Flags & FlagType.Static) > 0) scopedVarName = inClass.Name + "." + varName;
                    else scopedVarName = "this." + varName;
                }
            }

            var template = TemplateUtils.GetTemplate("FieldFromParameter");
            template = TemplateUtils.ReplaceTemplateVariable(template, "Name", scopedVarName);
            template = TemplateUtils.ReplaceTemplateVariable(template, "Value", paramName);
            template += "\n$(Boundary)";

            SnippetHelper.InsertSnippetText(sci, bodyStart, template);

            //TODO: We also need to check parent classes!!!
            foreach (MemberModel classMember in inClass.Members)
                if (classMember.Name.Equals(varName))
                {
                    ASContext.Panel.RestoreLastLookupPosition();
                    return;
                }

            var latest = GetLatestMemberForVariable(GeneratorJobType.Variable, inClass, GetDefaultVisibility(inClass), new MemberModel());
            if (latest == null) return;

            var position = FindNewVarPosition(sci, inClass, latest);
            if (position <= 0) return;
            sci.SetSel(position, position);

            var newMember = NewMember(varName, member, FlagType.Variable, scope);
            newMember.Type = paramType;

            GenerateVariable(newMember, position, true);
            ASContext.Panel.RestoreLastLookupPosition();
            if (ASContext.Context.Settings.GenerateImports)
            {
                var imports = new List<string> {paramType};
                var types = GetQualifiedTypes(imports, inClass.InFile);
                position += AddImportsByName(types, sci.LineFromPosition(position));
                sci.SetSel(position, position);
            }
        }

        protected virtual string GetFieldTypeFromParameter(string paramType, ref string paramName)
        {
            //foo(v1<generator>:Function/*(v1:Type):void*/)
            if ((contextMember.Flags & FlagType.Function) != 0) return FunctionToString(new MemberModel {Parameters = contextMember.Parameters, Type = paramType});
            if (paramName.StartsWithOrdinal("..."))
            {
                paramName = paramName.TrimStart('.');
                return "Array";
            }
            return paramType;
        }

        /// <summary>
        /// Tries to get the best position inside a code block, delimited by { and }, to add new code, inserting new lines if needed.
        /// </summary>
        /// <param name="lineFrom">The line inside the Scintilla document where the owner member of the body starts</param>
        /// <param name="lineTo">The line inside the Scintilla document where the owner member of the body ends</param>
        /// <param name="sci">The Scintilla control containing the document</param>
        /// <returns>The position inside the scintilla document, or -1 if not suitable position was found</returns>
        public static int GetBodyStart(int lineFrom, int lineTo, ScintillaControl sci) => GetBodyStart(lineFrom, lineTo, sci, true);

        /// <summary>
        /// Tries to get the start position of a code block, delimited by { and }
        /// </summary>
        /// <param name="lineFrom">The line inside the Scintilla document where the owner member of the body starts</param>
        /// <param name="lineTo">The line inside the Scintilla document where the owner member of the body ends</param>
        /// <param name="sci">The Scintilla control containing the document</param>
        /// <param name="needsPointOfInsertion">If true looks for the position to add new code, inserting new lines if needed</param>
        /// <returns>The position inside the Scintilla document, or -1 if not suitable position was found</returns>
        public static int GetBodyStart(int lineFrom, int lineTo, ScintillaControl sci, bool needsPointOfInsertion)
        {
            int posStart = sci.PositionFromLine(lineFrom);
            int posEnd = sci.LineEndPosition(lineTo);

            int funcBodyStart = -1;

            int genCount = 0, parCount = 0;
            for (int i = posStart; i <= posEnd; i++)
            {
                char c = (char)sci.CharAt(i);

                if (c == '{')
                {
                    int style = sci.BaseStyleAt(i);
                    if (ASComplete.IsCommentStyle(style) || ASComplete.IsLiteralStyle(style) || genCount > 0 || parCount > 0)
                        continue;
                    funcBodyStart = i;
                    break;
                }
                else if (c == '<')
                {
                    int style = sci.BaseStyleAt(i);
                    if (style == 10)
                        genCount++;
                }
                else if (c == '>')
                {
                    int style = sci.BaseStyleAt(i);
                    if (style == 10 && genCount > 0)
                        genCount--;
                }
                else if (c == '(')
                {
                    int style = sci.BaseStyleAt(i);
                    if (style == 10)
                        parCount++;
                }
                else if (c == ')')
                {
                    int style = sci.BaseStyleAt(i);
                    if (style == 10)
                        parCount--;
                }
            }

            if (funcBodyStart == -1)
                return -1;

            if (needsPointOfInsertion)
            {
                int ln = sci.LineFromPosition(funcBodyStart);

                funcBodyStart++;
                return GetOrSetPointOfInsertion(funcBodyStart, posEnd, ln, sci);
            }

            return funcBodyStart + 1;
        }

        [Obsolete(message: "Please use ASGenerator.GetStartOfStatement(expr) instead of ASGenerator.GetStartOfStatement(sci, statementEnd, expr)")]
        public static int GetStartOfStatement(ScintillaControl sci, int statementEnd, ASResult expr) => GetStartOfStatement(expr);

        public static int GetStartOfStatement(ASResult expr)
        {
            if (expr.Type != null)
            {
                var wordBefore = expr.Context.WordBefore;
                if (!string.IsNullOrEmpty(wordBefore) && ASContext.Context.Features.codeKeywords.Contains(wordBefore)) return expr.Context.WordBeforePosition;
            }
            return expr.Context.PositionExpression;
        }

        /// <summary>
        /// Tries to get the best position after a statement, to add new code, inserting new lines if needed.
        /// </summary>
        /// <param name="startPos">The position inside the Scintilla document where the statement starts</param>
        /// <param name="endPos">The position inside the Scintilla document where the owner member of the statement ends</param>
        /// <param name="sci">The Scintilla control containing the document</param>
        /// <returns>The position inside the Scintilla document</returns>
        /// <remarks>For now internal because for the current use we don't need to detect a lot of cases! use with caution!</remarks>
        public static int GetEndOfStatement(int startPos, int endPos, ScintillaControl sci)
        {
            var groupCount = 0;
            var brCount = 0;
            var statementEnd = startPos;
            while (statementEnd < endPos)
            {
                if (sci.PositionIsOnComment(statementEnd) || sci.PositionIsInString(statementEnd))
                {
                    statementEnd++;
                    continue;
                }
                var endOfStatement = false;
                var c = (char)sci.CharAt(statementEnd++);
                switch (c)
                {
                    case '\r':
                    case '\n':
                    case ',':
                        endOfStatement = groupCount == 0 && brCount == 0;
                        break;
                    case ';':
                        endOfStatement = brCount == 0; // valid or invalid end of statement
                        break;
                    case '(':
                    case '[':
                        groupCount++;
                        break;
                    case '{':
                        brCount++;
                        break;
                    case ')':
                    case ']':
                        groupCount--;
                        endOfStatement = groupCount < 0;
                        break;
                    case '}':
                        brCount--;
                        endOfStatement = brCount < 0;
                        break;
                }
                if (endOfStatement) break;
            }
            return statementEnd;
        }

        /// <summary>
        /// Looks for the best next position to insert new code, inserting new lines if needed
        /// </summary>
        /// <param name="startPos">The position inside the Scintilla document to start looking for the insertion position</param>
        /// <param name="endPos">The end position inside the Scintilla document</param>
        /// <param name="baseLine">The line inside the document to use as the base for the indentation level and detect if the desired point
        /// matches the end line</param>
        /// <param name="sci">The ScintillaControl where our document resides</param>
        /// <returns>The insertion point position</returns>
        private static int GetOrSetPointOfInsertion(int startPos, int endPos, int baseLine, ScintillaControl sci)
        {
            char[] characterClass = { ' ', '\r', '\n', '\t' };
            int nCount = 0;
            int extraLine = 1;

            int initialLn = sci.LineFromPosition(startPos);
            int baseIndent = sci.GetLineIndentation(baseLine);

            bool found = false;
            while (startPos <= endPos)
            {
                char c = (char)sci.CharAt(startPos);
                if (!characterClass.Contains(c))
                {
                    int endLn = sci.LineFromPosition(startPos);
                    if (endLn == baseLine || endLn == initialLn)
                    {
                        sci.InsertText(startPos, sci.NewLineMarker);
                        // Do we want to set the line indentation no matter what? {\r\t\t\t\r} -> {\r\t\r}
                        // Better results in most cases, but maybe highly unwanted in others?
                        sci.SetLineIndentation(++endLn, baseIndent + sci.Indent);
                        startPos = sci.LineIndentPosition(endLn);
                    }
                    if (c == '}')
                    {
                        sci.InsertText(startPos, sci.NewLineMarker);
                        sci.SetLineIndentation(endLn + 1, baseIndent);
                        // In relation with previous comment... we'll reinden this one: {\r} -> {\r\t\r}
                        if (sci.GetLineIndentation(endLn) <= baseIndent)
                        {
                            sci.SetLineIndentation(endLn, baseIndent + sci.Indent);
                            startPos = sci.LineIndentPosition(endLn);
                        }
                    }
                    found = true;
                    break;
                }
                else if (sci.EOLMode == 1 && c == '\r' && (++nCount) > extraLine)
                {
                    found = true;
                    break;
                }
                else if (c == '\n' && (++nCount) > extraLine)
                {
                    if (sci.EOLMode != 2)
                    {
                        startPos--;
                    }
                    found = true;
                    break;
                }
                startPos++;
            }

            if (!found) startPos--;

            return startPos;
        }

        private static void GenerateToString(ScintillaControl sci, ClassModel inClass)
        {
            var resultMember = new MemberModel("toString", ASContext.Context.Features.stringKey, FlagType.Function, Visibility.Public);
            var isOverride = false;
            inClass.ResolveExtends();
            var aType = inClass.Extends;
            while (!aType.IsVoid() && aType.QualifiedName != "Object")
            {
                foreach (MemberModel method in aType.Members)
                {
                    if (method.Name == "toString")
                    {
                        isOverride = true;
                        break;
                    }
                }
                if (isOverride)
                {
                    resultMember.Flags |= FlagType.Override;
                    break;
                }
                // interface inheritance
                aType = aType.Extends;
            }
            var membersString = new StringBuilder();
            var len = 0;
            foreach (MemberModel m in inClass.Members)
            {
                if (((m.Flags & FlagType.Variable) > 0 || (m.Flags & FlagType.Getter) > 0)
                    && (m.Access & Visibility.Public) > 0
                    && (m.Flags & FlagType.Constant) == 0)
                {
                    var oneMembersString = new StringBuilder();
                    oneMembersString.Append(" ").Append(m.Name).Append("=\" + ").Append(m.Name).Append(" + ");
                    membersString.Append(oneMembersString);
                    len += oneMembersString.Length;
                    if (len > 80)
                    {
                        len = 0;
                        membersString.Append("\n\t\t\t\t");
                    }
                    membersString.Append("\"");
                }
            }
            var template = TemplateUtils.GetTemplate("ToString");
            var result = TemplateUtils.ToDeclarationWithModifiersString(resultMember, template);
            result = TemplateUtils.ReplaceTemplateVariable(result, "Body", "\"[" + inClass.Name + membersString + "]\"");
            InsertCode(sci.CurrentPos, result, sci);
        }

        private static void GenerateVariableJob(GeneratorJobType job, ScintillaControl sci, MemberModel member, bool detach, ClassModel inClass)
        {
            var wordStartPos = sci.WordStartPosition(sci.CurrentPos, true);
            var visibility = job.Equals(GeneratorJobType.Variable) ? GetDefaultVisibility(inClass) : Visibility.Public;
            // evaluate, if the variable (or constant) should be generated in other class
            var varResult = ASComplete.GetExpressionType(sci, sci.WordEndPosition(sci.CurrentPos, true));
            if (member != null && ASContext.CommonSettings.GenerateScope && !varResult.Context.Value.Contains(ASContext.Context.Features.dot)) AddExplicitScopeReference(sci, inClass, member);
            var contextOwnerPos = GetContextOwnerEndPos(sci, sci.WordStartPosition(sci.CurrentPos, true));
            var isStatic = new MemberModel();
            if (contextOwnerPos != -1)
            {
                var contextOwnerResult = ASComplete.GetExpressionType(sci, contextOwnerPos);
                if (contextOwnerResult != null
                    && (contextOwnerResult.Member == null || (contextOwnerResult.Member.Flags & FlagType.Constructor) > 0)
                    && contextOwnerResult.Type != null)
                {
                    isStatic.Flags |= FlagType.Static;
                }
            }
            else if (member != null && (member.Flags & FlagType.Static) > 0)
            {
                isStatic.Flags |= FlagType.Static;
            }

            ASResult returnType = null;
            var lineNum = sci.CurrentLine;
            var line = sci.GetLine(lineNum);
            
            if (Regex.IsMatch(line, "\\b" + Regex.Escape(contextToken) + "\\("))
            {
                returnType = new ASResult {Type = ASContext.Context.ResolveType("Function", null)};
            }
            else
            {
                for (int i = sci.WordEndPosition(sci.CurrentPos, true), length = sci.Length; i < length; i++)
                {
                    if (sci.PositionIsOnComment(i)) continue;
                    var c = sci.CharAt(i);
                    if (c <= ' ') continue;
                    if (c == '=')
                    {
                        i = GetEndOfStatement(i, sci.Length, sci) - 1;
                        returnType = ASComplete.GetExpressionType(sci, i, false, true);
                    }
                    break;
                }
            }
            var isOtherClass = false;
            if (varResult.RelClass != null && !varResult.RelClass.IsVoid() && !varResult.RelClass.Equals(inClass))
            {
                AddLookupPosition();
                lookupPosition = -1;

                ASContext.MainForm.OpenEditableDocument(varResult.RelClass.InFile.FileName, false);
                sci = ASContext.CurSciControl;
                isOtherClass = true;
                var fileModel = ASContext.Context.GetCodeModel(sci.Text);
                foreach (var cm in fileModel.Classes)
                {
                    if (!cm.QualifiedName.Equals(varResult.RelClass.QualifiedName)) continue;
                    varResult.RelClass = cm;
                    break;
                }
                inClass = varResult.RelClass;

                ASContext.Context.UpdateContext(inClass.LineFrom);
            }

            var latest = GetLatestMemberForVariable(job, inClass, visibility, isStatic);
            var position = 0;
            // if we generate variable in current class..
            if (!isOtherClass && member == null)
            {
                detach = false;
                lookupPosition = -1;
                position = sci.WordStartPosition(sci.CurrentPos, true);
                sci.SetSel(position, sci.WordEndPosition(position, true));
            }
            else // if we generate variable in another class
            {
                if (latest != null)
                {
                    position = FindNewVarPosition(sci, inClass, latest);
                }
                else
                {
                    position = GetBodyStart(inClass.LineFrom, inClass.LineTo, sci);
                    detach = false;
                }
                if (position <= 0) return;
                sci.SetSel(position, position);
            }

            // if this is a constant, we assign a value to constant
            string returnTypeStr = null;
            if (job == GeneratorJobType.Constant && returnType == null) isStatic.Flags |= FlagType.Static;
            else if (returnType != null)
            {
                if (returnType.Member != null)
                {
                    if ((returnType.Member.Flags & FlagType.Function) != 0)
                        returnTypeStr = ((ASGenerator) ASContext.Context.CodeGenerator).FunctionToString(returnType.Member);
                    else if (returnType.Member.Type != ASContext.Context.Features.voidKey)
                        returnTypeStr = returnType.Member.Type;
                }
                else if (returnType.Type != null) returnTypeStr = GetShortType(returnType.Type.Type);
                if (ASContext.Context.Settings.GenerateImports)
                {
                    ClassModel inClassForImport;
                    if (returnType.InClass != null) inClassForImport = returnType.InClass;
                    else if (returnType.RelClass != null) inClassForImport = returnType.RelClass;
                    else inClassForImport = inClass;
                    List<string> imports = null;
                    if (returnType.Member != null)
                    {
                        if (returnTypeStr != ASContext.Context.Features.voidKey) imports = new List<string> {returnTypeStr};
                    }
                    else if (returnType.Type != null) imports = new List<string> {returnType.Type.QualifiedName};
                    if (imports != null)
                    {
                        var types = GetQualifiedTypes(imports, inClassForImport.InFile);
                        position += AddImportsByName(types, sci.LineFromPosition(position));
                        sci.SetSel(position, position);
                    }
                }
            }

            var kind = job.Equals(GeneratorJobType.Constant) ? FlagType.Constant : FlagType.Variable;
            var newMember = NewMember(contextToken, isStatic, kind, visibility);
            if (returnTypeStr != null) newMember.Type = returnTypeStr;
            else
            {
                var pos = wordStartPos;
                var index = ASComplete.FindParameterIndex(sci, ref pos);
                if (pos != -1)
                {
                    var expr = ASComplete.GetExpressionType(sci, pos);
                    if (expr?.Member?.Parameters.Count > 0) newMember.Type = expr.Member.Parameters[index].Type;
                }
            }
            if (job == GeneratorJobType.Constant && returnType == null)
            {
                if (string.IsNullOrEmpty(newMember.Type)) newMember.Type = "String = \"" + Camelize(contextToken) + "\"";
                else
                {
                    var value = ASContext.Context.GetDefaultValue(newMember.Type);
                    if (!string.IsNullOrEmpty(value)) newMember.Type += " = " + value;
                }
            }
            GenerateVariable(newMember, position, detach);
        }

        private static int GetContextOwnerEndPos(ScintillaControl sci, int wordStartPos)
        {
            var pos = wordStartPos - 1;
            var dotFound = false;
            while (pos > 0)
            {
                var c = (char) sci.CharAt(pos);
                if (c == '.' && !dotFound) dotFound = true;
                else if (c == '\t' || c == '\n' || c == '\r' || c == ' ') { /* skip */ }
                else return dotFound ? pos + 1 : -1;
                pos--;
            }
            return pos;
        }

        public static string Capitalize(string name)
        {
            return !string.IsNullOrEmpty(name) ? char.ToUpper(name[0]) + name.Substring(1) : name;
        }

        public static string Camelize(string name)
        {
            name = name.Trim('\'', '"');
            var parts = name.ToLower().Split('_');
            var result = "";
            foreach (var part in parts)
            {
                if (result.Length > 0) result += Capitalize(part);
                else result = part;
            }
            return result;
        }

        public static List<FunctionParameter> ParseFunctionParameters(ScintillaControl sci, int p)
        {
            List<FunctionParameter> prms = new List<FunctionParameter>();
            StringBuilder sb = new StringBuilder();
            List<ASResult> types = new List<ASResult>();
            bool isFuncStarted = false;
            bool doBreak = false;
            bool writeParam = false;
            int subClosuresCount = 0;
            var arrCount = 0;
            IASContext ctx = ASContext.Context;
            char[] charsToTrim = {' ', '\t', '\r', '\n'};
            int counter = sci.TextLength; // max number of chars in parameters line (to avoid infinitive loop)
            string characterClass = ScintillaControl.Configuration.GetLanguage(sci.ConfigurationLanguage).characterclass.Characters;

            while (p < counter && !doBreak)
            {
                if (sci.PositionIsOnComment(p))
                {
                    p++;
                    continue;
                }
                if (sci.PositionIsInString(p))
                {
                    sb.Append((char)sci.CharAt(p++));
                    continue;
                }
                var c = (char) sci.CharAt(p++);
                ASResult result;
                if (c == '(' && !isFuncStarted)
                {
                    if (sb.ToString().Trim(charsToTrim).Length == 0) isFuncStarted = true;
                    else break;
                }
                else if (c == ';' && !isFuncStarted) break;
                else if (c == ')' && isFuncStarted && subClosuresCount == 0)
                {
                    isFuncStarted = false;
                    writeParam = true;
                    doBreak = true;
                }
                else if (c == '(' || c == '[' || c == '{')
                {
                    if (c == '[') arrCount++;
                    subClosuresCount++;
                    sb.Append(c);
                }
                else if (c == ')' || c == ']' || c == '}')
                {
                    if (c == ']') arrCount--;
                    subClosuresCount--;
                    sb.Append(c);
                    if (subClosuresCount == 0)
                    {
                        if (c == ']' && arrCount == 0)
                        {
                            var cNext = sci.CharAt(p);
                            if (cNext != '[' && cNext != '.' && cNext != '(')
                            {
                                if (!sb.ToString().Contains("<"))
                                {
                                    result = ASComplete.GetExpressionType(sci, p);
                                    if (result.Type != null) result.Member = null;
                                    else result.Type = ctx.ResolveType(ctx.Features.arrayKey, null);
                                    types.Insert(0, result);
                                }
                            }
                        }
                    }
                }
                else if (c == ',' && subClosuresCount == 0) writeParam = true;
                else if (isFuncStarted) sb.Append(c);
                else if (characterClass.Contains(c)) doBreak = true;

                if (writeParam)
                {
                    writeParam = false;
                    string trimmed = sb.ToString().Trim(charsToTrim);
                    var trimmedLength = trimmed.Length;
                    if (trimmedLength > 0)
                    {
                        var last = trimmed[trimmedLength - 1];
                        var type = last == '}' && trimmed.StartsWith(ctx.Features.functionKey)
                                   ? ctx.ResolveType("Function", null)
                                   : ctx.ResolveToken(trimmed, ctx.CurrentModel);
                        if (!type.IsVoid()) result = new ASResult {Type = type};
                        else result = ASComplete.GetExpressionType(sci, p - 1, false, true);
                        if (result != null && !result.IsNull())
                        {
                            if (characterClass.Contains(last)) types.Insert(0, result);
                            else types.Add(result);
                        }
                        if (types.Count == 0)
                        {
                            result = new ASResult();
                            result.Type = ctx.ResolveType(ctx.Features.objectKey, null);
                            types.Add(result);
                        }

                        result = types[0];
                        string paramName = null;
                        string paramType = null;
                        string paramQualType = null;

                        if (result.Member == null)
                        {
                            paramType = result.Type.Name;
                            paramQualType = result.Type.QualifiedName;
                        }
                        else
                        {
                            if (result.Member.Name != null)
                            {
                                paramName = result.Member.Name.Trim('@');
                            }
                            if (result.Member.Type == null)
                            {
                                paramType = ctx.Features.dynamicKey;
                                paramQualType = ctx.Features.dynamicKey;
                            }
                            else
                            {
                                var flags = result.Member.Flags;
                                if ((flags & FlagType.Function) != 0 && (flags & FlagType.Getter) == 0 && (flags & FlagType.Setter) == 0
                                    && !result.Path.EndsWith('~'))
                                {
                                    paramType = ((ASGenerator) ctx.CodeGenerator).FunctionToString(result.Member);
                                }
                                else paramType = MemberModel.FormatType(GetShortType(result.Member.Type));
                                if (result.InClass == null) paramQualType = result.Type.QualifiedName;
                                else paramQualType = GetQualifiedType(paramType, result.InClass);
                            }
                        }
                        prms.Add(new FunctionParameter(paramName, paramType, paramQualType, result));
                    }
                    types = new List<ASResult>();
                    sb = new StringBuilder();
                }
            }
            for (int i = 0; i < prms.Count; i++)
            {
                if (prms[i].paramType == "void")
                {
                    prms[i].paramName = "object";
                    prms[i].paramType = null;
                }
                else prms[i].paramName = GuessVarName(prms[i].paramName, MemberModel.FormatType(GetShortType(prms[i].paramType)));
            }
            for (int i = 0; i < prms.Count; i++)
            {
                int iterator = -1;
                bool nameUnique = false;
                string name = prms[i].paramName;
                string suggestedName = name;
                while (!nameUnique) 
                {
                    iterator++;
                    suggestedName = name + (iterator == 0 ? "" : iterator + "");
                    bool gotMatch = false;
                    for (int j = 0; j < i; j++)
                    {
                        if (prms[j] != prms[i] && prms[j].paramName == suggestedName)
                        {
                            gotMatch = true;
                            break;
                        }
                    }
                    nameUnique = !gotMatch;
                }
                prms[i].paramName = suggestedName;
            }
            return prms;
        }

        static void GenerateConstructorJob(ScintillaControl sci, ClassModel inClass)
        {
            var position = sci.WordEndPosition(sci.CurrentPos, true);
            var parameters = ParseFunctionParameters(sci, position);
            var member = new MemberModel(inClass.Name, inClass.QualifiedName, FlagType.Constructor | FlagType.Function, Visibility.Public)
            {
                Parameters = parameters.Select(it => new MemberModel(it.paramName, it.paramQualType, FlagType.ParameterVar, 0)).ToList()
            };
            var currentClass = ASContext.Context.CurrentClass;
            if (currentClass != inClass)
            {
                AddLookupPosition();
                lookupPosition = -1;
                if (currentClass.InFile != inClass.InFile) sci = ((ITabbedDocument)PluginBase.MainForm.OpenEditableDocument(inClass.InFile.FileName, false)).SciControl;
                ASContext.Context.UpdateContext(inClass.LineFrom);
            }
            position = GetBodyStart(inClass.LineFrom, inClass.LineTo, sci);
            sci.SetSel(position, position);
            ((ASGenerator) ASContext.Context.CodeGenerator).GenerateFunction(sci, member, position, inClass, false);
        }

        private static void GenerateFunctionJob(GeneratorJobType job, ScintillaControl sci, MemberModel member, bool detach, ClassModel inClass)
        {
            var visibility = job.Equals(GeneratorJobType.FunctionPublic) ? Visibility.Public : GetDefaultVisibility(inClass);
            var wordStartPos = sci.WordStartPosition(sci.CurrentPos, true);
            var wordPos = sci.WordEndPosition(sci.CurrentPos, true);
            var parameters = ParseFunctionParameters(sci, wordPos);
            // evaluate, if the function should be generated in other class
            var funcResult = ASComplete.GetExpressionType(sci, wordPos);
            if (member != null && ASContext.CommonSettings.GenerateScope && !funcResult.Context.Value.Contains(ASContext.Context.Features.dot)) AddExplicitScopeReference(sci, inClass, member);
            var contextOwnerPos = GetContextOwnerEndPos(sci, wordStartPos);
            var isStatic = new MemberModel();
            if (contextOwnerPos != -1)
            {
                var contextOwnerResult = ASComplete.GetExpressionType(sci, contextOwnerPos);
                if (contextOwnerResult != null
                    && (contextOwnerResult.Member == null || (contextOwnerResult.Member.Flags & FlagType.Constructor) > 0)
                    && contextOwnerResult.Type != null)
                {
                    isStatic.Flags |= FlagType.Static;
                }
            }
            else if (member != null && (member.Flags & FlagType.Static) > 0)
            {
                isStatic.Flags |= FlagType.Static;
            }
            bool isOtherClass = false;
            if (funcResult.RelClass != null && !funcResult.RelClass.IsVoid() && !funcResult.RelClass.Equals(inClass))
            {
                AddLookupPosition();
                lookupPosition = -1;

                ASContext.MainForm.OpenEditableDocument(funcResult.RelClass.InFile.FileName, true);
                sci = ASContext.CurSciControl;
                isOtherClass = true;
                var fileModel = ASContext.Context.GetCodeModel(sci.Text);
                foreach (var cm in fileModel.Classes)
                {
                    if (cm.QualifiedName.Equals(funcResult.RelClass.QualifiedName))
                    {
                        funcResult.RelClass = cm;
                        break;
                    }
                }
                inClass = funcResult.RelClass;
                ASContext.Context.UpdateContext(inClass.LineFrom);
            }

            string blockTmpl;
            if ((isStatic.Flags & FlagType.Static) > 0) blockTmpl = TemplateUtils.GetBoundary("StaticMethods");
            else if ((visibility & Visibility.Public) > 0) blockTmpl = TemplateUtils.GetBoundary("PublicMethods");
            else blockTmpl = TemplateUtils.GetBoundary("PrivateMethods");

            var position = 0;
            var latest = TemplateUtils.GetTemplateBlockMember(sci, blockTmpl);
            if (latest == null || (!isOtherClass && member == null))
            {
                latest = GetLatestMemberForFunction(inClass, visibility, isStatic);
                // if we generate function in current class..
                if (!isOtherClass)
                {
                    var location = ASContext.CommonSettings.MethodsGenerationLocations;
                    if (member == null)
                    {
                        detach = false;
                        lookupPosition = -1;
                        position = sci.WordStartPosition(sci.CurrentPos, true);
                        sci.SetSel(position, sci.WordEndPosition(position, true));
                    }
                    else if (latest != null && location == MethodsGenerationLocations.AfterSimilarAccessorMethod)
                    {
                        position = sci.PositionFromLine(latest.LineTo + 1) - (sci.EOLMode == 0 ? 2 : 1);
                        sci.SetSel(position, position);
                    }
                    else
                    {
                        position = sci.PositionFromLine(member.LineTo + 1) - (sci.EOLMode == 0 ? 2 : 1);
                        sci.SetSel(position, position);
                    }
                }
                else // if we generate function in another class..
                {
                    if (latest != null)
                    {
                        position = sci.PositionFromLine(latest.LineTo + 1) - (sci.EOLMode == 0 ? 2 : 1);
                    }
                    else
                    {
                        position = GetBodyStart(inClass.LineFrom, inClass.LineTo, sci);
                        detach = false;
                    }
                    sci.SetSel(position, position);
                }
            }
            else
            {
                position = sci.PositionFromLine(latest.LineTo + 1) - (sci.EOLMode == 0 ? 2 : 1);
                sci.SetSel(position, position);
            }
            string newMemberType = null;
            ASResult callerExpr = null;
            MemberModel caller = null;
            var pos = wordStartPos;
            var parameterIndex = ASComplete.FindParameterIndex(sci, ref pos);
            if (pos != -1)
            {
                callerExpr = ASComplete.GetExpressionType(sci, pos);
                if (callerExpr != null) caller = callerExpr.Member;
            }
            if (caller?.Parameters != null && caller.Parameters.Count > 0)
            {
                string CleanType(string s) => s.StartsWith("(") && s.EndsWith(')') ? CleanType(s.Trim('(', ')')) : s;
                var param = caller.Parameters[parameterIndex];
                var parameterType = param.Type;
                if ((char) sci.CharAt(wordPos) == '(') newMemberType = parameterType;
                else
                {
                    var model = ASContext.Context.CodeComplete.FunctionTypeToMemberModel(parameterType, callerExpr.InFile);
                    if (model != null)
                    {
                        newMemberType = model.Type;
                        if (model.Parameters != null)
                        {
                            foreach (var it in model.Parameters)
                            {
                                var type = CleanType(it.Type);
                                parameters.Add(new FunctionParameter(it.Name, type, type, null));
                            }
                        }
                    }
                    else newMemberType = ASContext.Context.Features.voidKey;
                }
                newMemberType = CleanType(newMemberType);
                // for example: 
                //      foo(v1<generator>)
                //      function foo(v1:Function/*(v1:Type):void*/)
                if ((param.Flags & FlagType.Function) != 0 && parameters.Count != param.Parameters.Count)
                {
                    parameters.Clear();
                    foreach (var it in param.Parameters)
                    {
                        parameters.Add(new FunctionParameter(it.Name, it.Type, it.Type, null));
                    }
                }
            }
            // add imports to function argument types
            if (ASContext.Context.Settings.GenerateImports && parameters.Count > 0)
            {
                var types = GetQualifiedTypes(parameters.Select(it => it.paramQualType), inClass.InFile);
                position += AddImportsByName(types, sci.LineFromPosition(position));
                if (latest == null) sci.SetSel(position, sci.WordEndPosition(position, true));
                else sci.SetSel(position, position);
            }
            var newMember = NewMember(contextToken, isStatic, FlagType.Function, visibility);
            newMember.Parameters = new List<MemberModel>();
            foreach (var it in parameters)
            {
                var type = it.paramType.Length > it.paramQualType.Length ? it.paramType : it.paramQualType;
                newMember.Parameters.Add(new MemberModel(AvoidKeyword(it.paramName), GetShortType(type), FlagType.ParameterVar, 0));
            }
            if (newMemberType != null) newMember.Type = newMemberType;
            ((ASGenerator) ASContext.Context.CodeGenerator).GenerateFunction(sci, newMember, position, inClass, detach);
        }

        protected virtual void GenerateFunction(ScintillaControl sci, MemberModel member, int position, ClassModel inClass, bool detach)
        {
            string template;
            if ((inClass.Flags & FlagType.Interface) > 0)
            {
                template = TemplateUtils.GetTemplate("IFunction");
                template = TemplateUtils.ToDeclarationString(member, template);
            }
            else if ((member.Flags & FlagType.Constructor) > 0)
            {
                template = TemplateUtils.GetTemplate("Constructor");
                template = TemplateUtils.ToDeclarationWithModifiersString(member, template);
                var line = sci.LineFromPosition(position);
                if (GetDeclarationAtLine(line).Member != null) template += $"{NewLine}{NewLine}{NewLine}";
                else if (GetDeclarationAtLine(line + 1).Member != null) template += $"{NewLine}{NewLine}";
            }
            else
            {
                var body = GetFunctionBody(member, inClass);
                template = TemplateUtils.GetTemplate("Function");
                template = TemplateUtils.ToDeclarationWithModifiersString(member, template);
                template = TemplateUtils.ReplaceTemplateVariable(template, "Body", body);
            }
            GenerateFunction(position, template, detach);
        }

        protected virtual string GetFunctionBody(MemberModel member, ClassModel inClass)
        {
            switch (ASContext.CommonSettings.GeneratedMemberDefaultBodyStyle)
            {
                case GeneratedMemberBodyStyle.ReturnDefaultValue:
                    var defaultValue = ASContext.Context.GetDefaultValue(member.Type);
                    if (!string.IsNullOrEmpty(defaultValue)) return $"return {defaultValue};";
                    break;
            }
            return null;
        }

        protected void GenerateFunction(int position, string declaration, bool detach)
        {
            if (detach) declaration = NewLine + TemplateUtils.ReplaceTemplateVariable(declaration, "BlankLine", NewLine);
            else declaration = TemplateUtils.ReplaceTemplateVariable(declaration, "BlankLine", null);
            InsertCode(position, declaration);
        }

        static void GenerateClass(ScintillaControl sci, ClassModel inClass, ASExpr data)
        {
            var parameters = ParseFunctionParameters(sci, sci.WordEndPosition(data.PositionExpression, false));
            GenerateClass(inClass, data.Value, parameters);
        }

        static void GenerateClass(ScintillaControl sci, ClassModel inClass, string className)
        {
            var parameters = ParseFunctionParameters(sci, sci.WordEndPosition(sci.CurrentPos, true));
            GenerateClass(inClass, className, parameters);
        }

        private static void GenerateClass(ClassModel inClass, string className, IEnumerable<FunctionParameter> parameters)
        {
            AddLookupPosition(); // remember last cursor position for Shift+F4

            var constructorArgs = new List<MemberModel>();
            var constructorArgTypes = new List<string>();
            var paramMember = new MemberModel();
            foreach (var p in parameters)
            {
                constructorArgs.Add(new MemberModel(AvoidKeyword(p.paramName), p.paramType, FlagType.ParameterVar, 0));
                constructorArgTypes.Add(CleanType(GetQualifiedType(p.paramQualType, inClass)));
            }
            
            paramMember.Parameters = constructorArgs;

            var paramsString = TemplateUtils.ParametersString(paramMember, true);
            var info = new Hashtable();
            info["className"] = string.IsNullOrEmpty(className) ? "Class" : className;
            info["templatePath"] = Path.Combine(PathHelper.TemplateDir, "ProjectFiles", PluginBase.CurrentProject.GetType().Name, $"Class{ASContext.Context.Settings.DefaultExtension}.fdt");
            info["inDirectory"] = Path.GetDirectoryName(inClass.InFile.FileName);
            info["constructorArgs"] = paramsString.Length > 0 ? paramsString : null;
            info["constructorArgTypes"] = constructorArgTypes;
            var de = new DataEvent(EventType.Command, "ProjectManager.CreateNewFile", info);
            EventManager.DispatchEvent(null, de);
        }

        static void GenerateInterface(ClassModel inClass, string name)
        {
            AddLookupPosition(); // remember last cursor position for Shift+F4
            var info = new Hashtable();
            info["interfaceName"] = string.IsNullOrEmpty(name) ? "IInterface" : name;
            info["templatePath"] = Path.Combine(PathHelper.TemplateDir, "ProjectFiles", PluginBase.CurrentProject.GetType().Name, $"Interface{ASContext.Context.Settings.DefaultExtension}.fdt");
            info["inDirectory"] = Path.GetDirectoryName(inClass.InFile.FileName);
            var de = new DataEvent(EventType.Command, "ProjectManager.CreateNewFile", info);
            EventManager.DispatchEvent(null, de);
        }

        public static void GenerateExtractVariable(ScintillaControl sci, string newName)
        {
            string expression = sci.SelText.Trim('=', ' ', '\t', '\n', '\r', ';', '.');
            expression = expression.TrimEnd('(', '[', '{', '<');
            expression = expression.TrimStart(')', ']', '}', '>');

            var cFile = ASContext.Context.GetCodeModel(ASContext.Context.CurrentModel, sci.Text);
            MemberModel current = cFile.Context.CurrentMember;

            string characterClass = ScintillaControl.Configuration.GetLanguage(sci.ConfigurationLanguage).characterclass.Characters;

            int funcBodyStart = GetBodyStart(current.LineFrom, current.LineTo, sci);
            sci.SetSel(funcBodyStart, sci.LineEndPosition(current.LineTo));
            string currentMethodBody = sci.SelText;
            var insertPosition = funcBodyStart + currentMethodBody.IndexOfOrdinal(expression);
            var line = sci.LineFromPosition(insertPosition);
            insertPosition = sci.LineIndentPosition(line);
            
            int lastPos = -1;
            sci.Colourise(0, -1);
            while (true)
            {
                lastPos = currentMethodBody.IndexOfOrdinal(expression, lastPos + 1);
                if (lastPos > -1)
                {
                    char prevOrNextChar;
                    if (lastPos > 0)
                    {
                        prevOrNextChar = currentMethodBody[lastPos - 1];
                        if (characterClass.Contains(prevOrNextChar))
                        {
                            continue;
                        }
                    }
                    if (lastPos + expression.Length < currentMethodBody.Length)
                    {
                        prevOrNextChar = currentMethodBody[lastPos + expression.Length];
                        if (characterClass.Contains(prevOrNextChar))
                        {
                            continue;
                        }
                    }

                    var pos = funcBodyStart + lastPos;
                    int style = sci.BaseStyleAt(pos);
                    if (ASComplete.IsCommentStyle(style)) continue;
                    sci.SetSel(pos, pos + expression.Length);
                    sci.ReplaceSel(newName);
                    currentMethodBody = currentMethodBody.Substring(0, lastPos) + newName + currentMethodBody.Substring(lastPos + expression.Length);
                    lastPos += newName.Length;
                }
                else
                {
                    break;
                }
            }
            
            sci.SetSel(insertPosition, insertPosition);
            var m = new MemberModel(newName, "", FlagType.LocalVar, 0) {Value = expression};

            string snippet = TemplateUtils.GetTemplate("Variable");
            snippet = TemplateUtils.ReplaceTemplateVariable(snippet, "Modifiers", null);
            snippet = TemplateUtils.ToDeclarationString(m, snippet);
            snippet += NewLine + "$(Boundary)";
            SnippetHelper.InsertSnippetText(sci, sci.CurrentPos, snippet);
        }

        public static void GenerateExtractMethod(ScintillaControl sci, string newName)
        {
            string selection = sci.SelText;
            if (string.IsNullOrEmpty(selection)) return;

            var trimmedLength = selection.TrimStart().Length;
            if (trimmedLength == 0) return;

            sci.SetSel(sci.SelectionStart + selection.Length - trimmedLength, sci.SelectionEnd);
            sci.CurrentPos = sci.SelectionEnd;

            int lineStart = sci.LineFromPosition(sci.SelectionStart);
            int lineEnd = sci.LineFromPosition(sci.SelectionEnd);
            int firstLineIndent = sci.GetLineIndentation(lineStart);
            int entryPointIndent = sci.Indent;

            for (int i = lineStart; i <= lineEnd; i++)
            {
                int indent = sci.GetLineIndentation(i);
                if (i > lineStart)
                {
                    sci.SetLineIndentation(i, indent - firstLineIndent + entryPointIndent);
                }
            }

            string selText = sci.SelText;
            string template = TemplateUtils.GetTemplate("CallFunction");
            template = TemplateUtils.ReplaceTemplateVariable(template, "Name", newName);
            template = TemplateUtils.ReplaceTemplateVariable(template, "Arguments", "");
            sci.Colourise(0, -1);
            var pos = sci.SelectionEnd - 1;
            var endPos = sci.TextLength;
            while (pos++ < endPos)
            {
                var style = sci.StyleAt(pos);
                if (ASComplete.IsCommentStyle(style)) continue;
                var c = (char) sci.CharAt(pos);
                if (c == '\n' || c == '\r')
                {
                    template += ";";
                    break;
                }
                if ((c == ';' || c == ',' || c == '.' || c == ')' || c == '}' || c == '{') && !ASComplete.IsStringStyle(style)) break;
            }
            InsertCode(sci.CurrentPos, template, sci);

            var ctx = ASContext.Context;
            ctx.GetCodeModel(ctx.CurrentModel, sci.Text);
            var found = ((ASGenerator) ctx.CodeGenerator).GetDeclarationAtLine(lineStart);
            if (found.Member == null) return;

            lookupPosition = sci.CurrentPos;
            AddLookupPosition();

            MemberModel latest = TemplateUtils.GetTemplateBlockMember(sci, TemplateUtils.GetBoundary("PrivateMethods"));

            if (latest == null)
                latest = GetLatestMemberForFunction(found.InClass, GetDefaultVisibility(found.InClass), found.Member);

            if (latest == null)
                latest = found.Member;

            int position = sci.PositionFromLine(latest.LineTo + 1) - ((sci.EOLMode == 0) ? 2 : 1);
            sci.SetSel(position, position);

            FlagType flags = FlagType.Function;
            if ((found.Member.Flags & FlagType.Static) > 0)
            {
                flags |= FlagType.Static;
            }

            MemberModel m = new MemberModel(newName, ctx.Features.voidKey, flags, GetDefaultVisibility(found.InClass));

            template = NewLine + TemplateUtils.GetTemplate("Function");
            template = TemplateUtils.ToDeclarationWithModifiersString(m, template);
            template = TemplateUtils.ReplaceTemplateVariable(template, "Body", selText);
            template = TemplateUtils.ReplaceTemplateVariable(template, "BlankLine", NewLine);
            InsertCode(position, template, sci);
        }

        private static int FindNewVarPosition(ScintillaControl sci, ClassModel inClass, MemberModel latest)
        {
            firstVar = false;
            // found a var?
            if ((latest.Flags & FlagType.Variable) > 0)
                return sci.PositionFromLine(latest.LineTo + 1) - ((sci.EOLMode == 0) ? 2 : 1);

            // add as first member
            int line = 0;
            int maxLine = sci.LineCount;
            if (inClass != null)
            {
                line = inClass.LineFrom;
                maxLine = inClass.LineTo;
            }
            else if (ASContext.Context.InPrivateSection) line = ASContext.Context.CurrentModel.PrivateSectionIndex;
            else maxLine = ASContext.Context.CurrentModel.PrivateSectionIndex;
            while (line < maxLine)
            {
                string text = sci.GetLine(line++);
                if (text.Contains('{'))
                {
                    firstVar = true;
                    return sci.PositionFromLine(line) - ((sci.EOLMode == 0) ? 2 : 1);
                }
            }
            return -1;
        }

        private static bool RemoveLocalDeclaration(ScintillaControl sci, MemberModel contextMember)
        {
            int removed = 0;
            if (contextResolved != null)
            {
                contextResolved.Context.LocalVars.Items.Sort(new ByDeclarationPositionMemberComparer());
                contextResolved.Context.LocalVars.Items.Reverse();
                foreach (MemberModel member in contextResolved.Context.LocalVars)
                {
                    if (member.Name == contextMember.Name)
                    {
                        RemoveOneLocalDeclaration(sci, member);
                        removed++;
                    }
                }
            }
            if (removed == 0) return RemoveOneLocalDeclaration(sci, contextMember);
            else return true;
        }

        private static bool RemoveOneLocalDeclaration(ScintillaControl sci, MemberModel contextMember)
        {
            string type = "";
            if (contextMember.Type != null && (contextMember.Flags & FlagType.Inferred) == 0)
            {
                // for example: var f:Function/*(v1:Type):void*/
                if ((contextMember.Flags & FlagType.Function) != 0) type = ":\\s*Function\\/\\*.*\\*\\/";
                else
                {
                    type = MemberModel.FormatType(contextMember.Type);
                    if (type.IndexOf('*') > 0)
                        type = type.Replace("/*", @"/\*\s*").Replace("*/", @"\s*\*/");
                    type = @":\s*" + type;
                }
            }
            var name = contextMember.Name;
            var reDecl = new Regex($@"[\s\(]((var|const)\s+{name}\s*{type})\s*");
            for (var i = contextMember.LineFrom; i <= contextMember.LineTo + 10; i++)
            {
                var text = sci.GetLine(i);
                var m = reDecl.Match(text);
                if (!m.Success) continue;
                var index = sci.MBSafeTextLength(text.Substring(0, m.Groups[1].Index));
                var position = sci.PositionFromLine(i) + index;
                var len = sci.MBSafeTextLength(m.Groups[1].Value);
                sci.SetSel(position, position + len);
                if (ASContext.CommonSettings.GenerateScope) name = "this." + name;
                if (contextMember.Type == null || (contextMember.Flags & FlagType.Inferred) != 0) name += " ";
                sci.ReplaceSel(name);
                UpdateLookupPosition(position, name.Length - len);
                return true;
            }
            return false;
        }

        internal static StatementReturnType GetStatementReturnType(ScintillaControl sci, ClassModel inClass, string line, int startPos)
        {
            var target = new Regex(@"[;\s\n\r]*", RegexOptions.RightToLeft);
            var m = target.Match(line);
            if (!m.Success) return null;
            line = line.Substring(0, m.Index);
            if (line.Length == 0) return null;
            var pos = startPos + m.Index;
            var expr = ASComplete.GetExpressionType(sci, pos, false, true);
            if (expr.Type != null || expr.Member != null) pos = expr.Context.Position;
            var ctx = inClass.InFile.Context;
            var features = ctx.Features;
            var resolve = expr;
            if (resolve.Type != null && !resolve.IsPackage)
            {
                if (resolve.Type.Name == "Function")
                {
                    var type = ((ASGenerator) ctx.CodeGenerator).FunctionToString(expr.Member);
                    resolve = new ASResult {Type = new ClassModel {Name = type, InFile = FileModel.Ignore}, Context =  expr.Context};
                }
                else if (!string.IsNullOrEmpty(resolve.Path) && Regex.IsMatch(resolve.Path, @"(\.\[.{0,}?\])$", RegexOptions.RightToLeft))
                    resolve.Member = null;
            }
            var word = sci.GetWordFromPosition(pos);
            if (string.IsNullOrEmpty(word) && resolve.Type != null)
            {
                var tokens = Regex.Split(resolve.Context.Value, Regex.Escape(features.dot));
                word = tokens.LastOrDefault(it => it.Length > 0 && !(it.Length >= 2 && it[0] == '#' && it[it.Length - 1] == '~') && char.IsLetter(it[0]));
            }
            return new StatementReturnType(resolve, pos, word);
        }

        protected virtual string FunctionToString(MemberModel member) => member != null ? $"Function/*({member.ParametersString()}):{member.Type}*/" : "Function";

        protected static string GuessVarName(string name, string type)
        {
            if (name == "_") name = null;
            if (string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(type))
            {
                var m = Regex.Match(type, "^([a-z0-9_$]+)", RegexOptions.IgnoreCase);
                if (m.Success) name = m.Groups[1].Value;
                else name = type;
            }

            if (string.IsNullOrEmpty(name)) return name;

            // if constant then convert to camelCase
            if (name.ToUpper() == name) name = Camelize(name);

            // if getter, then remove 'get' prefix
            name = name.TrimStart('_');
            if (name.Length > 3 && name.StartsWithOrdinal("get"))
            {
                var c = name[3];
                if (!char.IsDigit(c) && c.ToString() == char.ToUpper(c).ToString())
                {
                    name = char.ToLower(c) + name.Substring(4);
                }
            }

            if (name.Length > 1) name = char.ToLower(name[0]) + name.Substring(1);
            else name = char.ToLower(name[0]) + "";
            var features = ASContext.Context.Features;
            if (name == features.ThisKey || name == features.BaseKey || type == name)
            {
                if (!string.IsNullOrEmpty(type)) name = char.ToLower(type[0]) + type.Substring(1);
                else if(name == features.BaseKey) name = "p_super";
                else name = "p_this";
            }
            return name;
        }

        private static void GenerateImplementation(ClassModel iType, ClassModel inClass, ScintillaControl sci, bool detached)
        {
            var typesUsed = new HashSet<string>();
            var header = TemplateUtils.ReplaceTemplateVariable(TemplateUtils.GetTemplate("ImplementHeader"), "Class", iType.Type);
            header = TemplateUtils.ReplaceTemplateVariable(header, "BlankLine", detached ? BlankLine : null);
            var sb = new StringBuilder();
            sb.Append(header);
            sb.Append(NewLine);
            var entry = true;
            var result = new ASResult();
            var ctx = ASContext.Context;
            var features = ctx.Features;
            var codeGenerator = ((ASGenerator) ctx.CodeGenerator);
            var canGenerate = false;
            var isHaxe = IsHaxe;
            var flags = (FlagType.Function | FlagType.Getter | FlagType.Setter);
            if (isHaxe) flags |= FlagType.Variable;

            iType.ResolveExtends(); // resolve inheritance chain
            while (!iType.IsVoid() && iType.QualifiedName != "Object")
            {
                for (var i = 0; i < iType.Members.Count; i++)
                {
                    var method = iType.Members[i];
                    if ((method.Flags & flags) == 0 || method.Name == iType.Name)
                        continue;

                    // check if method exists
                    ASComplete.FindMember(method.Name, inClass, result, method.Flags, 0);
                    if (!result.IsNull()) continue;

                    string decl;
                    if ((method.Flags & FlagType.Getter) > 0)
                    {
                        // for example: function get foo():Function/*(v:*):int*/
                        if ((method.Flags & FlagType.Function) != 0 && method.Parameters != null)
                            method.Type = codeGenerator.FunctionToString(method);
                        decl = codeGenerator.GetGetterImplementationTemplate(method);
                    }
                    else if ((method.Flags & FlagType.Setter) > 0)
                    {
                        // for example: function get set(v:Function/*(v:*):int*/):void
                        if (method.Parameters != null && method.Parameters.Count > 0)
                        {
                            var parameter = method.Parameters[0];
                            if ((parameter.Flags & FlagType.Function) != 0 && parameter.Parameters != null)
                                parameter.Type = codeGenerator.FunctionToString(parameter);
                        }
                        decl = TemplateUtils.ToDeclarationWithModifiersString(method, TemplateUtils.GetTemplate("Setter"));
                    }
                    else if ((method.Flags & FlagType.Function) > 0)
                    {
                        // for example: function get set(v:Function/*(v:*):int*/):void
                        if (method.Parameters != null)
                        {
                            foreach (var parameter in method.Parameters)
                            {
                                if ((parameter.Flags & FlagType.Function) != 0 && parameter.Parameters != null)
                                    parameter.Type = codeGenerator.FunctionToString(parameter);
                            }
                        }
                        decl = TemplateUtils.ToDeclarationWithModifiersString(method, TemplateUtils.GetTemplate("Function"));
                    }
                    else decl = NewLine + TemplateUtils.ToDeclarationWithModifiersString(method, TemplateUtils.GetTemplate("Variable"));

                    decl = TemplateUtils.ReplaceTemplateVariable(decl, "Member", "_" + method.Name);
                    decl = TemplateUtils.ReplaceTemplateVariable(decl, "Void", features.voidKey);
                    decl = TemplateUtils.ReplaceTemplateVariable(decl, "Body", codeGenerator.GetFunctionBody(method, inClass));
                    decl = TemplateUtils.ReplaceTemplateVariable(decl, "BlankLine", NewLine);

                    if (!entry) decl = TemplateUtils.ReplaceTemplateVariable(decl, "EntryPoint", null);
                    decl += NewLine;
                    entry = false;
                    sb.Append(decl);
                    canGenerate = true;
                    typesUsed.Add(method.Type);
                    if (method.Parameters != null && method.Parameters.Count > 0)
                        foreach (var param in method.Parameters)
                            typesUsed.Add(param.Type);
                }

                if (ctx.Settings.GenerateImports) typesUsed = (HashSet<string>) GetQualifiedTypes(typesUsed, iType.InFile);
                // interface inheritance
                iType = iType.Extends;
            }
            if (!canGenerate) return;
            sci.BeginUndoAction();
            try
            {
                var position = sci.CurrentPos;
                if (ctx.Settings.GenerateImports && typesUsed.Count > 0)
                {
                    position += AddImportsByName(typesUsed, sci.LineFromPosition(position));
                    sci.SetSel(position, position);
                }
                InsertCode(position, sb.ToString(), sci);
            }
            finally { sci.EndUndoAction(); }
        }

        protected virtual string GetGetterImplementationTemplate(MemberModel method)
        {
            return TemplateUtils.ToDeclarationWithModifiersString(method, TemplateUtils.GetTemplate("Getter"));
        }

        private static void AddTypeOnce(ICollection<string> typesUsed, string qualifiedName)
        {
            if (!typesUsed.Contains(qualifiedName)) typesUsed.Add(qualifiedName);
        }

        protected static IEnumerable<string> GetQualifiedTypes(IEnumerable<string> types, FileModel inFile)
        {
            var result = new HashSet<string>();
            types = ASContext.Context.DecomposeTypes(types);
            foreach (var type in types)
            {
                if (type == "*" || type.Contains(".")) result.Add(type);
                else
                {
                    var model = ASContext.Context.ResolveType(type, inFile);
                    if (!model.IsVoid() && model.InFile.Package.Length > 0) result.Add(model.QualifiedName);
                }
            }
            return result;
        }

        private static string GetQualifiedType(string type, ClassModel aType)
        {
            var dynamicKey = ASContext.Context.Features.dynamicKey ?? "*";
            if (string.IsNullOrEmpty(type)) return dynamicKey;
            if (ASContext.Context.DecomposeTypes(new [] {type}).Count() > 1) return type;
            if (type.IndexOf('<') > 0) // Vector.<Point>
            {
                Match mGeneric = Regex.Match(type, "<([^>]+)>");
                if (mGeneric.Success)
                {
                    return GetQualifiedType(mGeneric.Groups[1].Value, aType);
                }
            }

            if (type.IndexOf('.') > 0) return type;

            ClassModel aClass = ASContext.Context.ResolveType(type, aType.InFile);
            if (!aClass.IsVoid())
            {
                return aClass.QualifiedName;
            }
            return dynamicKey;
        }

        private static MemberModel NewMember(string name, MemberModel calledFrom, FlagType kind, Visibility access)
        {
            var type = kind == FlagType.Function && !ASContext.Context.Features.hasInference
                ? ASContext.Context.Features.voidKey
                : null;
            if (calledFrom != null && (calledFrom.Flags & FlagType.Static) > 0)
                kind |= FlagType.Static;
            return new MemberModel(name, type, kind, access);
        }

        /// <summary>
        /// Get Visibility.Private or Visibility.Protected, depending on user setting forcing the use of protected.
        /// </summary>
        private static Visibility GetDefaultVisibility(ClassModel model)
        {
            if (ASContext.Context.Features.protectedKey != null
                && ASContext.CommonSettings.GenerateProtectedDeclarations
                && (model.Flags & FlagType.Final) == 0)
                return Visibility.Protected;
            return Visibility.Private;
        }

        private static void GenerateVariable(MemberModel member, int position, bool detach)
        {
            string result;
            if ((member.Flags & FlagType.Constant) > 0)
            {
                string template = TemplateUtils.GetTemplate("Constant");
                result = TemplateUtils.ToDeclarationWithModifiersString(member, template);
                result = TemplateUtils.ReplaceTemplateVariable(result, "Value", member.Value);
            }
            else
            {
                string template = TemplateUtils.GetTemplate("Variable");
                result = TemplateUtils.ToDeclarationWithModifiersString(member, template);
            }

            if (firstVar) 
            { 
                result = '\t' + result; 
                firstVar = false; 
            }
            if (detach) result = NewLine + result;
            InsertCode(position, result);
        }

        public static bool MakePrivate(ScintillaControl Sci, MemberModel member, ClassModel inClass)
        {
            var features = ASContext.Context.Features;
            var visibility = GetPrivateKeyword(inClass);
            if (features.publicKey == null || visibility == null) return false;
            var rePublic = new Regex($@"\s*({features.publicKey})\s+");
            for (int i = member.LineFrom; i <= member.LineTo; i++)
            {
                var line = Sci.GetLine(i);
                var m = rePublic.Match(line);
                if (m.Success)
                {
                    var index = Sci.MBSafeTextLength(line.Substring(0, m.Groups[1].Index));
                    var position = Sci.PositionFromLine(i) + index;
                    Sci.SetSel(position, position + features.publicKey.Length);
                    Sci.ReplaceSel(visibility);
                    UpdateLookupPosition(position, features.publicKey.Length - visibility.Length);
                    return true;
                }
            }
            return false;
        }

        public static bool RenameMember(ScintillaControl Sci, MemberModel member, string newName)
        {
            ContextFeatures features = ASContext.Context.Features;
            string kind = features.varKey;

            if ((member.Flags & FlagType.Getter) > 0)
                kind = features.getKey;
            else if ((member.Flags & FlagType.Setter) > 0)
                kind = features.setKey;
            else if (member.Flags == FlagType.Function)
                kind = features.functionKey;

            Regex reMember = new Regex(String.Format(@"{0}\s+({1})[\s:]", kind, member.Name));

            for (int i = member.LineFrom; i <= member.LineTo; i++)
            {
                var line = Sci.GetLine(i);
                var m = reMember.Match(line);
                if (m.Success)
                {
                    var index = Sci.MBSafeTextLength(line.Substring(0, m.Groups[1].Index));
                    var position = Sci.PositionFromLine(i) + index;
                    Sci.SetSel(position, position + member.Name.Length);
                    Sci.ReplaceSel(newName);
                    UpdateLookupPosition(position, 1);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Return an obvious property name matching a private var, or null
        /// </summary>
        static string GetPropertyNameFor(MemberModel member)
        {
            if (IsHaxe) return null;
            var name = member.Name;
            if (name.Length == 0 || (member.Access & Visibility.Public) != 0) return null;
            var parts = Regex.Match(name, "([^_$]*)[_$]+(.*)");
            if (!parts.Success) return null;
            var pre = parts.Groups[1].Value;
            var post = parts.Groups[2].Value;
            return pre.Length > post.Length ? pre : post;
        }

        /// <summary>
        /// Return a smart new property name
        /// </summary>
        private static string GetNewPropertyNameFor(MemberModel member)
        {
            if (member.Name.Length == 0) return "prop";
            if (Regex.IsMatch(member.Name, "^[A-Z].*[a-z]"))
                return char.ToLower(member.Name[0]) + member.Name.Substring(1);
            return "_" + member.Name;
        }

        private static void GenerateDelegateMethod(string name, MemberModel afterMethod, int position, ClassModel inClass)
        {
            ContextFeatures features = ASContext.Context.Features;

            string acc = GetPrivateAccessor(afterMethod, inClass);
            string template = TemplateUtils.GetTemplate("Delegate");
            string args = null;
            string type = features.voidKey;

            if (features.hasDelegates && contextMember != null) // delegate functions types
            {
                args = contextMember.ParametersString();
                type = contextMember.Type;
            }

            string decl = BlankLine + TemplateUtils.ReplaceTemplateVariable(template, "Modifiers", acc);
            decl = TemplateUtils.ReplaceTemplateVariable(decl, "Name", name);
            decl = TemplateUtils.ReplaceTemplateVariable(decl, "Arguments", args);
            decl = TemplateUtils.ReplaceTemplateVariable(decl, "Type", type);
            InsertCode(position, decl);
        }

        private static void GenerateEventHandler(string name, string type, MemberModel afterMethod, int position, ClassModel inClass)
        {
            var ctx = ASContext.Context;
            var sci = ASContext.CurSciControl;
            sci.BeginUndoAction();
            try
            {
                var delta = 0;
                var eventClass = ctx.ResolveType(type, ctx.CurrentModel);
                if (eventClass.IsVoid())
                {
                    if (TryImportType("flash.events." + type, ref delta, sci.LineFromPosition(position)))
                    {
                        position += delta;
                        sci.SetSel(position, position);
                    }
                    else type = null;
                }
                lookupPosition += delta;
                var newMember = new MemberModel
                {
                    Name = name,
                    Type = type,
                    Access = GetDefaultVisibility(inClass)
                };
                if ((afterMethod.Flags & FlagType.Static) > 0) newMember.Flags = FlagType.Static;
                var template = TemplateUtils.GetTemplate("EventHandler");
                var declaration = NewLine + TemplateUtils.ToDeclarationWithModifiersString(newMember, template);
                declaration = TemplateUtils.ReplaceTemplateVariable(declaration, "Void", ctx.Features.voidKey);
                var eventName = contextMatch.Groups["event"].Value;
                var autoRemove = AddRemoveEvent(eventName);
                ((ASGenerator) ctx.CodeGenerator).GenerateEventHandler(sci, position, declaration, autoRemove, eventName, name);
            }
            finally
            {
                sci.EndUndoAction();
            }
        }

        protected virtual void GenerateEventHandler(ScintillaControl sci, int position, string template, string currentTarget, string eventName, string handlerName)
        {
            var ctx = ASContext.Context;
            if (currentTarget != null)
            {
                var delta = 0;
                if (TryImportType("flash.events.IEventDispatcher", ref delta, sci.LineFromPosition(position)))
                {
                    position += delta;
                    sci.SetSel(position, position);
                    lookupPosition += delta;
                    currentTarget = "IEventDispatcher(e.currentTarget)";
                }
                if (currentTarget.Length == 0 && ASContext.CommonSettings.GenerateScope && ctx.Features.ThisKey != null)
                    currentTarget = ctx.Features.ThisKey;
                if (currentTarget.Length > 0) currentTarget += ".";
                var remove = $"{currentTarget}removeEventListener({eventName}, {handlerName});\n\t$(EntryPoint)";
                template = template.Replace("$(EntryPoint)", remove);
            }
            InsertCode(position, template, sci);
        }

        protected static bool TryImportType(string type, ref int delta, int atLine)
        {
            var eventClass = ASContext.Context.ResolveType(type, ASContext.Context.CurrentModel);
            if (eventClass.IsVoid()) return false;
            var typesUsed = new List<string> {type};
            delta += AddImportsByName(typesUsed, atLine);
            return true;
        }

        private static string AddRemoveEvent(string eventName)
        {
            foreach (var autoRemove in ASContext.CommonSettings.EventListenersAutoRemove)
            {
                var test = autoRemove.Trim();
                if (test.Length == 0 || test.StartsWithOrdinal("//")) continue;
                var colonPos = test.IndexOf(':');
                if (colonPos >= 0) test = test.Substring(colonPos + 1);
                if (test != eventName) continue;
                return colonPos < 0 ? "" : autoRemove.Trim().Substring(0, colonPos);
            }
            return null;
        }

        private static void GenerateGetter(string name, MemberModel member, int position) => GenerateGetter(name, member, position, true, false);

        protected static void GenerateGetter(string name, MemberModel member, int position, bool startsWithNewLine, bool endsWithNewLine)
        {
            var newMember = new MemberModel
            {
                Name = name,
                Type = MemberModel.FormatType(GetShortType(member.Type)),
                Access = IsHaxe ? Visibility.Private : Visibility.Public
            };
            if ((member.Flags & FlagType.Static) > 0) newMember.Flags = FlagType.Static;
            string template = TemplateUtils.GetTemplate("Getter");
            string decl;
            if (startsWithNewLine) decl = NewLine + TemplateUtils.ToDeclarationWithModifiersString(newMember, template);
            else decl = TemplateUtils.ToDeclarationWithModifiersString(newMember, template);
            decl = TemplateUtils.ReplaceTemplateVariable(decl, "Member", member.Name);
            decl = TemplateUtils.ReplaceTemplateVariable(decl, "BlankLine", NewLine);
            if (endsWithNewLine) decl += NewLine + NewLine;
            InsertCode(position, decl);
        }

        protected static void GenerateSetter(string name, MemberModel member, int position)
        {
            var newMember = new MemberModel
            {
                Name = name,
                Type = GetShortType(GetShortType(member.Type)),
                Access = IsHaxe ? Visibility.Private : Visibility.Public
            };
            if ((member.Flags & FlagType.Static) > 0) newMember.Flags = FlagType.Static;
            string template = TemplateUtils.GetTemplate("Setter");
            string decl = NewLine + TemplateUtils.ToDeclarationWithModifiersString(newMember, template);
            decl = TemplateUtils.ReplaceTemplateVariable(decl, "Member", member.Name);
            decl = TemplateUtils.ReplaceTemplateVariable(decl, "Void", ASContext.Context.Features.voidKey ?? "void");
            decl = TemplateUtils.ReplaceTemplateVariable(decl, "BlankLine", NewLine);
            InsertCode(position, decl);
        }

        protected static void GenerateGetterSetter(string name, MemberModel member, int position)
        {
            string template = TemplateUtils.GetTemplate("GetterSetter");
            if (template == "")
            {
                GenerateSetter(name, member, position);
                ASContext.CurSciControl.SetSel(position, position);
                GenerateGetter(name, member, position);
                return;
            }
            var newMember = new MemberModel
            {
                Name = name,
                Type = MemberModel.FormatType(GetShortType(member.Type)),
                Access = IsHaxe ? Visibility.Private : Visibility.Public
            };
            if ((member.Flags & FlagType.Static) > 0) newMember.Flags = FlagType.Static;
            string decl = NewLine + TemplateUtils.ToDeclarationWithModifiersString(newMember, template);
            decl = TemplateUtils.ReplaceTemplateVariable(decl, "Member", member.Name);
            decl = TemplateUtils.ReplaceTemplateVariable(decl, "Void", ASContext.Context.Features.voidKey ?? "void");
            decl = TemplateUtils.ReplaceTemplateVariable(decl, "BlankLine", NewLine);
            InsertCode(position, decl);
        }

        private static string GetStaticKeyword(MemberModel member)
        {
            if ((member.Flags & FlagType.Static) > 0) return ASContext.Context.Features.staticKey ?? "static";
            return null;
        }

        private static string GetPrivateAccessor(MemberModel member, ClassModel inClass)
        {
            string acc = GetStaticKeyword(member);
            if (!string.IsNullOrEmpty(acc)) acc += " ";
            return acc + GetPrivateKeyword(inClass);
        }

        private static string GetPrivateKeyword(ClassModel inClass)
        {
            if (GetDefaultVisibility(inClass) == Visibility.Protected) return ASContext.Context.Features.protectedKey ?? "protected";
            return ASContext.Context.Features.privateKey ?? "private";
        }

        private static MemberModel GetLatestMemberForFunction(ClassModel inClass, Visibility access, MemberModel isStatic)
        {
            MemberModel latest = null;
            if (isStatic != null && (isStatic.Flags & FlagType.Static) > 0)
            {
                latest = FindLatest(FlagType.Function | FlagType.Static, access, inClass);
                if (latest == null)
                {
                    latest = FindLatest(FlagType.Function | FlagType.Static, 0, inClass, true, false);
                }
            }
            else
            {
                latest = FindLatest(FlagType.Function, access, inClass);
            }
            if (latest == null)
            {
                latest = FindLatest(FlagType.Function, 0, inClass, true, false);
            }
            if (latest == null)
            {
                latest = FindLatest(FlagType.Function, 0, inClass, false, false);
            }
            return latest;
        }

        private static MemberModel GetLatestMemberForVariable(GeneratorJobType job, ClassModel inClass, Visibility access, MemberModel isStatic)
        {
            MemberModel latest = null;
            if (job.Equals(GeneratorJobType.Constant))
            {
                if ((isStatic.Flags & FlagType.Static) > 0)
                {
                    latest = FindLatest(FlagType.Constant | FlagType.Static, access, inClass);
                }
                else
                {
                    latest = FindLatest(FlagType.Constant, access, inClass);
                }
                if (latest == null)
                {
                    latest = FindLatest(FlagType.Constant, 0, inClass, true, false);
                }
            }
            else
            {
                if ((isStatic.Flags & FlagType.Static) > 0)
                {
                    latest = FindLatest(FlagType.Variable | FlagType.Static, access, inClass);
                    if (latest == null)
                    {
                        latest = FindLatest(FlagType.Variable | FlagType.Static, 0, inClass, true, false);
                    }
                }
                else
                {
                    latest = FindLatest(FlagType.Variable, access, inClass);
                }
            }
            if (latest == null)
            {
                latest = FindLatest(FlagType.Variable, access, inClass, false, false);
            }
            return latest;
        }

        protected static MemberModel FindMember(string name, ClassModel inClass)
        {
            MemberList list;
            if (inClass == ClassModel.VoidClass)
                list = ASContext.Context.CurrentModel.Members;
            else list = inClass.Members;

            MemberModel found = null;
            foreach (MemberModel member in list)
            {
                if (member.Name == name)
                {
                    found = member;
                    break;
                }
            }
            return found;
        }

        private static MemberModel FindLatest(FlagType match, ClassModel inClass) => FindLatest(match, 0, inClass);

        private static MemberModel FindLatest(FlagType match, Visibility visi, ClassModel inClass) => FindLatest(match, visi, inClass, true, true);

        protected static MemberModel FindLatest(FlagType match, Visibility visi, ClassModel inClass, bool isFlagMatchStrict, bool isVisibilityMatchStrict)
        {
            MemberList list;
            if (inClass == ClassModel.VoidClass)
                list = ASContext.Context.CurrentModel.Members;
            else
                list = inClass.Members;

            MemberModel latest = null;
            MemberModel fallback = null;
            foreach (MemberModel member in list)
            {
                fallback = member;
                if (isFlagMatchStrict && isVisibilityMatchStrict)
                {
                    if ((member.Flags & match) == match && (visi == 0 || (member.Access & visi) == visi))
                    {
                        latest = member;
                    }
                }
                else if (isFlagMatchStrict)
                {
                    if ((member.Flags & match) == match && (visi == 0 || (member.Access & visi) > 0))
                    {
                        latest = member;
                    }
                }
                else if (isVisibilityMatchStrict)
                {
                    if ((member.Flags & match) > 0 && (visi == 0 || (member.Access & visi) == visi))
                    {
                        latest = member;
                    }
                }
                else
                {
                    if ((member.Flags & match) > 0 && (visi == 0 || (member.Access & visi) > 0))
                    {
                        latest = member;
                    }
                }

            }
            if (isFlagMatchStrict || isVisibilityMatchStrict)
                fallback = null;
            return latest ?? fallback;
        }

        static void AddExplicitScopeReference(ScintillaControl sci, ClassModel inClass, MemberModel inMember)
        {
            var position = sci.CurrentPos;
            var start = sci.WordStartPosition(position, true);
            var length = sci.MBSafeTextLength(contextToken);
            sci.SetSel(start, start + length);
            var scope = (inMember.Flags & FlagType.Static) != 0 ? inClass.QualifiedName : "this";
            var text = $"{scope}{ASContext.Context.Features.dot}{contextToken}";
            sci.ReplaceSel(text);
            UpdateLookupPosition(position, text.Length - length);
        }
        #endregion

        #region override generator

        /// <summary>
        /// List methods to override
        /// </summary>
        /// <param name="autoHide">Don't keep the list open if the word does not match</param>
        /// <returns>Completion was handled</returns>
        protected virtual bool HandleOverrideCompletion(bool autoHide)
        {
            var ctx = ASContext.Context;
            var codeGenerator = (ASGenerator)ctx.CodeGenerator;
            var curClass = ctx.CurrentClass;
            if (curClass.IsVoid()) return false;

            var members = new List<MemberModel>();
            curClass.ResolveExtends(); // Resolve inheritance chain

            // explore getters or setters
            const FlagType mask = FlagType.Function | FlagType.Getter | FlagType.Setter;
            var tmpClass = curClass.Extends;
            var access = ctx.TypesAffinity(curClass, tmpClass);
            while (!tmpClass.IsVoid())
            {
                if (tmpClass.QualifiedName.StartsWithOrdinal("flash.utils.Proxy"))
                {
                    foreach (MemberModel member in tmpClass.Members)
                    {
                        member.Namespace = "flash_proxy";
                        members.Add(member);
                    }
                    break;
                }
                foreach (MemberModel member in tmpClass.Members)
                {
                    if (curClass.Members.Contains(member.Name, FlagType.Override, 0)) continue;
                    if ((member.Flags & FlagType.Dynamic) == 0
                        || (member.Access & access) == 0
                        || ((member.Flags & FlagType.Function) == 0 && (member.Flags & mask) == 0)) continue;
                    if (member.Parameters != null && member.Parameters.Count > 0)
                    {
                        foreach (var it in member.Parameters)
                        {
                            if ((it.Flags & FlagType.Function) == 0 || it.Parameters == null) continue;
                            it.Type = codeGenerator.FunctionToString(it);
                            it.Parameters = null;
                        }
                    }
                    if ((member.Flags & FlagType.Getter) != 0 && member.Parameters != null)
                    {
                        member.Type = codeGenerator.FunctionToString(member);
                        member.Parameters = null;
                    }
                    members.Add(member);
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

        public static void GenerateOverride(ScintillaControl Sci, ClassModel ofClass, MemberModel member, int position)
        {
            var ctx = ASContext.Context;
            var features = ctx.Features;
            var typesUsed = new List<string>();
            var isProxy = (member.Namespace == "flash_proxy");
            if (isProxy) typesUsed.Add("flash.utils.flash_proxy");
            
            var line = Sci.LineFromPosition(position);
            var currentText = Sci.GetLine(line);
            var startPos = currentText.Length;
            GetStartPos(currentText, ref startPos, features.privateKey);
            GetStartPos(currentText, ref startPos, features.protectedKey);
            GetStartPos(currentText, ref startPos, features.internalKey);
            GetStartPos(currentText, ref startPos, features.publicKey);
            GetStartPos(currentText, ref startPos, features.staticKey);
            GetStartPos(currentText, ref startPos, features.overrideKey);
            startPos += Sci.PositionFromLine(line);

            var newMember = new MemberModel
            {
                Name = member.Name,
                Type = member.Type
            };
            if (features.hasNamespaces && !string.IsNullOrEmpty(member.Namespace) && member.Namespace != "internal")
                newMember.Namespace = member.Namespace;
            else newMember.Access = member.Access;

            var isAS2Event = ctx.Settings.LanguageId == "AS2" && member.Name.StartsWithOrdinal("on");
            if (!isAS2Event && ofClass.QualifiedName != "Object") newMember.Flags |= FlagType.Override;
            var declaration = string.Empty;
            var flags = member.Flags;
            if ((flags & FlagType.Static) > 0) newMember.Flags |= FlagType.Static;
            var parameters = member.Parameters;
            if ((flags & (FlagType.Getter | FlagType.Setter)) > 0)
            {
                if (IsHaxe) newMember.Access = Visibility.Private;
                var type = newMember.Type;
                if (parameters != null && parameters.Count == 1 && parameters[0].Type != null) type = parameters[0].Type;
                type = MemberModel.FormatType(type);
                if (type == null && !features.hasInference) type = features.objectKey;
                newMember.Type = type;
                declaration = ((ASGenerator) ctx.CodeGenerator).TryGetOverrideGetterTemplate(ofClass, parameters, newMember);
                var set = ((ASGenerator) ctx.CodeGenerator).TryGetOverrideSetterTemplate(ofClass, parameters, newMember);
                if (set.Length > 0 && declaration.Length > 0) set = "\n\n" + set.Replace("$(EntryPoint)", "");
                declaration += set;
                declaration = TemplateUtils.ReplaceTemplateVariable(declaration, "BlankLine", "");
                typesUsed.Add(type);
            }
            else
            {
                var type = MemberModel.FormatType(newMember.Type);
                var noRet = type == null || type.Equals("void", StringComparison.OrdinalIgnoreCase);
                type = (noRet && type != null) ? features.voidKey : type;
                if (!noRet) typesUsed.Add(type);
                newMember.Template = member.Template;
                newMember.Type = type;
                // fix parameters if needed
                if (parameters != null)
                    foreach (var para in parameters)
                        if (para.Type == "any") para.Type = "*";

                newMember.Parameters = parameters;
                var action = (isProxy || isAS2Event) ? "" : GetSuperCall(member, typesUsed);
                var template = TemplateUtils.GetTemplate("MethodOverride");
                template = TemplateUtils.ToDeclarationWithModifiersString(newMember, template);
                template = TemplateUtils.ReplaceTemplateVariable(template, "Method", action);
                declaration = template;
            }
            Sci.BeginUndoAction();
            try
            {
                if (ctx.Settings.GenerateImports && typesUsed.Count > 0)
                {
                    var types = GetQualifiedTypes(typesUsed, ofClass.InFile);
                    var offset = AddImportsByName(types, line);
                    position += offset;
                    startPos += offset;
                }
                Sci.SetSel(startPos, position + member.Name.Length);
                InsertCode(startPos, declaration, Sci);
            }
            finally { Sci.EndUndoAction(); }
        }

        protected virtual string TryGetOverrideGetterTemplate(ClassModel ofClass, List<MemberModel> parameters, MemberModel newMember)
        {
            var name = newMember.Name;
            if (!ofClass.Members.Contains(name, FlagType.Getter, 0)) return string.Empty;
            var result = TemplateUtils.GetTemplate("OverrideGetter", "Getter");
            result = TemplateUtils.ToDeclarationWithModifiersString(newMember, result);
            result = TemplateUtils.ReplaceTemplateVariable(result, "Member", $"super.{name}");
            return result;
        }

        protected virtual string TryGetOverrideSetterTemplate(ClassModel ofClass, List<MemberModel> parameters, MemberModel newMember)
        {
            var name = newMember.Name;
            if (!ofClass.Members.Contains(name, FlagType.Setter, 0)) return string.Empty;
            var template = TemplateUtils.GetTemplate("OverrideSetter", "Setter");
            template = TemplateUtils.ToDeclarationWithModifiersString(newMember, template);
            template = TemplateUtils.ReplaceTemplateVariable(template, "Member", $"super.{name}");
            template = TemplateUtils.ReplaceTemplateVariable(template, "Void", ASContext.Context.Features.voidKey ?? "void");
            return template;
        }

        public static void GenerateDelegateMethods(ScintillaControl sci, MemberModel member,
            Dictionary<MemberModel, ClassModel> selectedMembers, ClassModel classModel, ClassModel inClass)
        {
            var ctx = ASContext.Context;
            var generateImports = ctx.Settings.GenerateImports;
            sci.BeginUndoAction();
            try
            {
                var result = TemplateUtils.GetTemplate("DelegateMethodsHeader");
                result = TemplateUtils.ReplaceTemplateVariable(result,  "Class", classModel.Type);
                var position = -1;
                var importsList = new List<string>();
                var isStaticMember = (member.Flags & FlagType.Static) > 0;
                inClass.ResolveExtends();
                foreach (var m in selectedMembers.Keys)
                {
                    var mCopy = (MemberModel) m.Clone();
                    var methodTemplate = NewLine;
                    var overrideFound = false;
                    var baseClassType = inClass;
                    while (!baseClassType.IsVoid())
                    {
                        var inClassMembers = baseClassType.Members;
                        foreach (MemberModel inClassMember in inClassMembers)
                        {
                            if ((inClassMember.Flags & FlagType.Function) > 0 && m.Name.Equals(inClassMember.Name))
                            {
                                mCopy.Flags |= FlagType.Override;
                                overrideFound = true;
                                break;
                            }
                        }
                        if (overrideFound) break;
                        baseClassType = baseClassType.Extends;
                    }
                    var flags = m.Flags;
                    if (isStaticMember && (flags & FlagType.Static) == 0) mCopy.Flags |= FlagType.Static;
                    var variableTemplate = string.Empty;
                    ((ASGenerator) ctx.CodeGenerator).TryGetGetterSetterDelegateTemplate(member, m, ref flags, ref variableTemplate, ref methodTemplate);
                    if (!string.IsNullOrEmpty(variableTemplate)) result += variableTemplate + ":" + m.Type + ";";
                    if ((flags & FlagType.Function) > 0)
                    {
                        methodTemplate += TemplateUtils.GetTemplate("Function");
                        methodTemplate = TemplateUtils.ReplaceTemplateVariable(methodTemplate, "Body", "<<$(Return) >>$(Body)");
                        methodTemplate = TemplateUtils.ReplaceTemplateVariable(methodTemplate, "EntryPoint", null);
                        methodTemplate = TemplateUtils.ToDeclarationWithModifiersString(mCopy, methodTemplate);
                        if (m.Type != null && m.Type.ToLower() != "void")
                            methodTemplate = TemplateUtils.ReplaceTemplateVariable(methodTemplate, "Return", "return");
                        else
                            methodTemplate = TemplateUtils.ReplaceTemplateVariable(methodTemplate, "Return", null);

                        // check for varargs
                        var isVararg = false;
                        if (m.Parameters != null && m.Parameters.Count > 0)
                        {
                            var mm = m.Parameters[m.Parameters.Count - 1];
                            if (mm.Name.StartsWithOrdinal("..."))
                                isVararg = true;
                        }

                        string callMethodTemplate = TemplateUtils.GetTemplate("CallFunction");
                        if (!isVararg)
                        {
                            callMethodTemplate = TemplateUtils.ReplaceTemplateVariable(callMethodTemplate, "Name", member.Name + "." + m.Name);
                            callMethodTemplate = TemplateUtils.ReplaceTemplateVariable(callMethodTemplate, "Arguments", 
                                TemplateUtils.CallParametersString(m));
                            callMethodTemplate += ";";
                        }
                        else 
                        {
                            List<MemberModel> pseudoParamsList = new List<MemberModel>();
                            pseudoParamsList.Add(new MemberModel("null", null, FlagType.ParameterVar, 0));
                            pseudoParamsList.Add(new MemberModel("[$(Subarguments)].concat($(Lastsubargument))", null, FlagType.ParameterVar, 0));
                            MemberModel pseudoParamsOwner = new MemberModel();
                            pseudoParamsOwner.Parameters = pseudoParamsList;

                            callMethodTemplate = TemplateUtils.ReplaceTemplateVariable(callMethodTemplate, "Name",
                                member.Name + "." + m.Name + ".apply");
                            callMethodTemplate = TemplateUtils.ReplaceTemplateVariable(callMethodTemplate, "Arguments",
                                TemplateUtils.CallParametersString(pseudoParamsOwner));
                            callMethodTemplate += ";";

                            List<MemberModel> arrayParamsList = new List<MemberModel>();
                            for (int i = 0; i < m.Parameters.Count - 1; i++)
                            {
                                MemberModel param = m.Parameters[i];
                                arrayParamsList.Add(param);
                            }

                            pseudoParamsOwner.Parameters = arrayParamsList;

                            callMethodTemplate = TemplateUtils.ReplaceTemplateVariable(callMethodTemplate, "Subarguments",
                                TemplateUtils.CallParametersString(pseudoParamsOwner));

                            callMethodTemplate = TemplateUtils.ReplaceTemplateVariable(callMethodTemplate, "Lastsubargument", 
                                m.Parameters[m.Parameters.Count - 1].Name.TrimStart('.', ' '));
                        }

                        methodTemplate = TemplateUtils.ReplaceTemplateVariable(methodTemplate, "Body", callMethodTemplate);
                    }
                    methodTemplate = TemplateUtils.ReplaceTemplateVariable(methodTemplate, "BlankLine", NewLine);
                    result += methodTemplate;
                    if (generateImports && m.Parameters != null)
                    {
                        importsList.AddRange(from param in m.Parameters where param.Type != null select param.Type);
                    }
                    if (position < 0)
                    {
                        MemberModel latest = GetLatestMemberForFunction(inClass, mCopy.Access, mCopy);
                        if (latest == null)
                        {
                            position = sci.WordStartPosition(sci.CurrentPos, true);
                            sci.SetSel(position, sci.WordEndPosition(position, true));
                        }
                        else
                        {
                            position = sci.PositionFromLine(latest.LineTo + 1) - ((sci.EOLMode == 0) ? 2 : 1);
                            sci.SetSel(position, position);
                        }
                    }
                    else position = sci.CurrentPos;
                    if (generateImports && m.Type != null) importsList.Add(m.Type);
                }
                if (generateImports && importsList.Count > 0 && position > -1)
                {
                    var types = GetQualifiedTypes(importsList, inClass.InFile);
                    position += AddImportsByName(types, sci.LineFromPosition(position));
                    sci.SetSel(position, position);
                }
                InsertCode(position, result, sci);
            }
            finally { sci.EndUndoAction(); }
        }

        protected virtual void TryGetGetterSetterDelegateTemplate(MemberModel member, MemberModel receiver, ref FlagType flags, ref string variableTemplate, ref string methodTemplate)
        {
            if ((flags & FlagType.Getter) != 0)
            {
                var modifiers = (TemplateUtils.GetStaticExternOverride(receiver) + TemplateUtils.GetModifiers(receiver)).Trim();
                methodTemplate += TemplateUtils.GetTemplate("Getter");
                methodTemplate = TemplateUtils.ReplaceTemplateVariable(methodTemplate, "Modifiers", modifiers);
                methodTemplate = TemplateUtils.ReplaceTemplateVariable(methodTemplate, "Name", receiver.Name);
                methodTemplate = TemplateUtils.ReplaceTemplateVariable(methodTemplate, "EntryPoint", "");
                methodTemplate = TemplateUtils.ReplaceTemplateVariable(methodTemplate, "Type", MemberModel.FormatType(receiver.Type));
                methodTemplate = TemplateUtils.ReplaceTemplateVariable(methodTemplate, "Member", member.Name + "." + receiver.Name);
                flags &= ~FlagType.Function;
            }
            if ((flags & FlagType.Setter) != 0)
            {
                var modifiers = (TemplateUtils.GetStaticExternOverride(receiver) + TemplateUtils.GetModifiers(receiver)).Trim();
                methodTemplate += TemplateUtils.GetTemplate("Setter");
                methodTemplate = TemplateUtils.ReplaceTemplateVariable(methodTemplate, "Modifiers", modifiers);
                methodTemplate = TemplateUtils.ReplaceTemplateVariable(methodTemplate, "Name", receiver.Name);
                methodTemplate = TemplateUtils.ReplaceTemplateVariable(methodTemplate, "EntryPoint", "");
                methodTemplate = TemplateUtils.ReplaceTemplateVariable(methodTemplate, "Type", receiver.Parameters[0].Type);
                methodTemplate = TemplateUtils.ReplaceTemplateVariable(methodTemplate, "Member", member.Name + "." + receiver.Name);
                methodTemplate = TemplateUtils.ReplaceTemplateVariable(methodTemplate, "Void", ASContext.Context.Features.voidKey ?? "void");
                flags &= ~FlagType.Function;
            }
        }

        private static void GetStartPos(string currentText, ref int startPos, string keyword)
        {
            if (keyword == null) return;
            int p = currentText.IndexOfOrdinal(keyword);
            if (p > 0 && p < startPos) startPos = p;
        }

        static Regex reShortType = new Regex(@"(?=\w+\.<)|(?:\w+\.)");

        static string GetShortType(string type)
        {
            if (string.IsNullOrEmpty(type)) return type;
            if (!type.Contains('@') && type.LastIndexOf('.') is int startIndex && startIndex != -1)
            {
                var importName = type.Substring(startIndex + 1);
                var imports = ASContext.Context.ResolveImports(ASContext.Context.CurrentModel);
                if (imports.Count == 0) imports = ASContext.Context.CurrentModel.Imports;
                if (!imports.Items.Any(it => it.Name == importName && it.Type != type))
                    type = reShortType.Replace(type, string.Empty);
            }
            else type = reShortType.Replace(type, string.Empty);
            return MemberModel.FormatType(type);
        }

        private static string CleanType(string type)
        {
            if (string.IsNullOrEmpty(type)) return type;
            int p = type.IndexOf('$');
            if (p > 0) type = type.Substring(0, p);
            p = type.IndexOf('<');
            if (p > 1 && type[p - 1] == '.') p--;
            if (p > 0) type = type.Substring(0, p);
            p = type.IndexOf('@');
            if (p > 0) type = type.Substring(0, p);
            return type;
        }

        private static string GetSuperCall(MemberModel member, List<string> typesUsed)
        {
            string args = "";
            if (member.Parameters != null)
                foreach (var param in member.Parameters)
                {
                    if (param.Name.StartsWith('.')) break;
                    args += ", " + TemplateUtils.GetParamName(param);
                    AddTypeOnce(typesUsed, param.Type);
                }

            bool noRet = string.IsNullOrEmpty(member.Type) || member.Type.Equals("void", StringComparison.OrdinalIgnoreCase);
            if (!noRet) AddTypeOnce(typesUsed, member.Type);

            string action = "";
            if ((member.Flags & FlagType.Function) > 0)
            {
                action =
                    (noRet ? "" : "return ")
                    + "super." + member.Name
                    + ((args.Length > 2) ? "(" + args.Substring(2) + ")" : "()") + ";";
            }
            else if ((member.Flags & FlagType.Setter) > 0 && args.Length > 0)
            {
                action = "super." + member.Name + " = " + member.Parameters[0].Name + ";";
            }
            else if ((member.Flags & FlagType.Getter) > 0)
            {
                action = "return super." + member.Name + ";";
            }
            return action;
        }

        #endregion

        #region imports generator

        /// <summary>
        /// Generates all the missing imports in the given types list
        /// </summary>
        /// <param name="typesUsed">Types to import if needed</param>
        /// <param name="atLine">Current line in editor</param>
        /// <returns>Inserted characters count</returns>
        protected static int AddImportsByName(IEnumerable<string> typesUsed, int atLine)
        {
            var length = 0;
            var context = ASContext.Context;
            var addedTypes = new HashSet<string>();
            typesUsed = context.DecomposeTypes(typesUsed);
            foreach (var type in typesUsed)
            {
                var cleanType = CleanType(type);
                if (string.IsNullOrEmpty(cleanType) || addedTypes.Contains(cleanType) || cleanType.IndexOf('.') <= 0)
                    continue;
                addedTypes.Add(cleanType);
                var import = new MemberModel(cleanType.Substring(cleanType.LastIndexOf('.') + 1), cleanType, FlagType.Import, Visibility.Public);
                if (!context.IsImported(import, atLine))
                    length += InsertImport(import, false);
            }
            return length;
        }

        /// <summary>
        /// Add an 'import' statement in the current file
        /// </summary>
        /// <param name="member">Generates 'import {member.Type};'</param>
        /// <param name="fixScrolling">Keep the editor view as if we didn't add any code in the file</param>
        /// <returns>Inserted characters count</returns>
        public static int InsertImport(MemberModel member, bool fixScrolling)
        {
            ScintillaControl sci = ASContext.CurSciControl;
            FileModel cFile = ASContext.Context.CurrentModel;
            int position = sci.CurrentPos;
            int curLine = sci.LineFromPosition(position);

            string fullPath = member.Type;
            if ((member.Flags & (FlagType.Class | FlagType.Enum | FlagType.TypeDef | FlagType.Struct)) > 0)
            {
                FileModel inFile = member.InFile;
                if (inFile != null && inFile.Module == member.Name && inFile.Package != "")
                    fullPath = inFile.Package + "." + inFile.Module;
                fullPath = CleanType(fullPath);
            }
            string nl = LineEndDetector.GetNewLineMarker(sci.EOLMode);
            string statement = "import " + fullPath + ";" + nl;

            // locate insertion point
            int line = (ASContext.Context.InPrivateSection) ? cFile.PrivateSectionIndex : 0;
            if (cFile.InlinedRanges != null)
            {
                foreach (InlineRange range in cFile.InlinedRanges)
                {
                    if (position > range.Start && position < range.End)
                    {
                        line = sci.LineFromPosition(range.Start) + 1;
                        break;
                    }
                }
            }
            int firstLine = line;
            bool found = false;
            int packageLine = -1;
            int indent = 0;
            int skipIfDef = 0;
            var importComparer = new CaseSensitiveImportComparer();
            while (line < curLine)
            {
                var txt = sci.GetLine(line++).TrimStart();
                if (packageLine != -2 && txt.StartsWith("package"))
                {
                    packageLine = line;
                    firstLine = line;
                }
                // skip Haxe #if blocks
                else if (txt.StartsWithOrdinal("#if ") && !txt.Contains("#end")) skipIfDef++;
                else if (skipIfDef > 0)
                {
                    if (txt.StartsWithOrdinal("#end")) skipIfDef--;
                    else continue;
                }
                // insert imports after a package declaration
                else if (txt.Length > 6 && txt.StartsWithOrdinal("import") && txt[6] <= 32)
                {
                    packageLine = -2;
                    found = true;
                    indent = sci.GetLineIndentation(line - 1);
                    // insert in alphabetical order
                    var mImport = ASFileParserRegexes.Import.Match(txt);
                    if (mImport.Success && importComparer.Compare(mImport.Groups["package"].Value, fullPath) > 0)
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

                if (packageLine >= 0)
                {
                    if (IsHaxe) packageLine = -2;
                    else if (txt.Contains('{'))
                    {
                        packageLine = -2;
                        indent = sci.GetLineIndentation(line - 1) + PluginBase.MainForm.Settings.IndentSize;
                        firstLine = line;
                    }
                }
            }

            // insert
            if (line == curLine) line = firstLine;
            position = sci.PositionFromLine(line);
            firstLine = sci.FirstVisibleLine;
            sci.SetSel(position, position);
            sci.ReplaceSel(statement);
            sci.SetLineIndentation(line, indent);
            sci.LineScroll(0, firstLine - sci.FirstVisibleLine + 1);

            ASContext.Context.RefreshContextCache(fullPath);
            return sci.GetLine(line).Length;
        }
        #endregion

        #region common safe code insertion

        protected static int lookupPosition;

        public static void InsertCode(int position, string src) => InsertCode(position, src, ASContext.CurSciControl);

        public static void InsertCode(int position, string src, ScintillaControl sci)
        {
            sci.BeginUndoAction();
            try
            {
                if (ASContext.CommonSettings.DeclarationModifierOrder.Length > 1)
                    src = FixModifiersLocation(src, ASContext.CommonSettings.DeclarationModifierOrder);

                int len = SnippetHelper.InsertSnippetText(sci, position + sci.MBSafeTextLength(sci.SelText), src);
                UpdateLookupPosition(position, len);
                AddLookupPosition(sci);
            }
            finally { sci.EndUndoAction(); }
        }

        /// <summary>
        /// Order declaration modifiers
        /// </summary>
        private static string FixModifiersLocation(string src, string[] modifierOrder)
        {
            bool needUpdate = false;
            string[] lines = src.Split('\n');
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];

                Match m = reModifiers.Match(line);
                if (!m.Success) continue;

                Group decl = m.Groups[2];
                string modifiers = decl.Value;
                string before = "", after = "";
                bool insertAfter = false;

                for (int j = 0; j < modifierOrder.Length; j++)
                {
                    string modifier = modifierOrder[j];
                    if (modifier == GeneralSettings.DECLARATION_MODIFIER_REST) insertAfter = true;
                    else
                    {
                        modifier = RemoveAndExtractModifier(modifier, ref modifiers);
                        if (insertAfter) after += modifier;
                        else before += modifier;
                    }
                }

                modifiers = before + modifiers + after;

                if (decl.Value != modifiers)
                {
                    lines[i] = line.Remove(decl.Index, decl.Length).Insert(decl.Index, modifiers);
                    needUpdate = true;
                }
            }
            return needUpdate ? string.Join("\n", lines) : src;
        }

        private static string RemoveAndExtractModifier(string modifier, ref string modifiers)
        {
            modifier += " ";
            int index = modifiers.IndexOf(modifier, StringComparison.Ordinal);

            if (index == -1) return null;
            modifiers = modifiers.Remove(index, modifier.Length);
            return modifier;
        }

        protected static void UpdateLookupPosition(int position, int delta)
        {
            if (lookupPosition > position)
            {
                if (lookupPosition < position + delta) lookupPosition = position;// replaced text at cursor position
                else lookupPosition += delta;
            }
        }

        private static void AddLookupPosition() => AddLookupPosition(ASContext.CurSciControl);

        protected static void AddLookupPosition(ScintillaControl sci)
        {
            if (lookupPosition < 0 || sci == null) return;
            var lookupLine = sci.LineFromPosition(lookupPosition);
            var lookupCol = lookupPosition - sci.PositionFromLine(lookupLine);
            // TODO: Refactor, doesn't make a lot of sense to have this feature inside the Panel
            ASContext.Panel.SetLastLookupPosition(sci.FileName, lookupLine, lookupCol);
        }

        #endregion
    }

    #region related structures
    /// <summary>
    /// Available generators
    /// </summary>
    public enum GeneratorJobType
    {
        GetterSetter,
        Getter,
        Setter,
        ComplexEvent,
        BasicEvent,
        Delegate,
        Variable,
        Function,
        ImplementInterface,
        PromoteLocal,
        MoveLocalUp,
        AddImport,
        Class,
        FunctionPublic,
        VariablePublic,
        Constant,
        Constructor,
        ToString,
        FieldFromParameter,
        AddInterfaceDef,
        ConvertToConst,
        AddAsParameter,
        ChangeMethodDecl,
        EventMetatag,
        AssignStatementToVar,
        ChangeConstructorDecl,
        Interface,
    }

    /// <summary>
    /// Generation completion list item
    /// </summary>
    internal class GeneratorItem : ICompletionListItem
    {
        internal GeneratorJobType Job { get; }
        private readonly MemberModel member;
        private readonly ClassModel inClass;

        public GeneratorItem(string label, GeneratorJobType job, MemberModel member, ClassModel inClass)
        {
            Label = label;
            this.Job = job;
            this.member = member;
            this.inClass = inClass;
        }

        public GeneratorItem(string label, GeneratorJobType job, MemberModel member, ClassModel inClass, object data) : this(label, job, member, inClass)
        {
            Data = data;
        }

        public string Label { get; }

        public string Description => TextHelper.GetString("Info.GeneratorTemplate");

        public Bitmap Icon => (Bitmap)ASContext.Panel.GetIcon(PluginUI.ICON_DECLARATION);

        public string Value
        {
            get
            {
                ASGenerator.GenerateJob(Job, member, inClass, Label, Data);
                return null;
            }
        }

        public object Data { get; }
    }

    public class FoundDeclaration
    {
        public MemberModel Member;
        public ClassModel InClass;

        public FoundDeclaration()
        {
            Member = null;
            InClass = ClassModel.VoidClass;
        }
    }

    public class FunctionParameter
    {
        public string paramType;
        public string paramQualType;
        public string paramName;
        public ASResult result;

        public FunctionParameter(string parameter, string paramType, string paramQualType, ASResult result)
        {
            this.paramName = parameter;
            this.paramType = paramType;
            this.paramQualType = paramQualType;
            this.result = result;
        }
    }

    class StatementReturnType
    {
        public readonly ASResult Resolve;
        public readonly int Position;
        public readonly string Word;

        public StatementReturnType(ASResult resolve, int position, string word)
        {
            Resolve = resolve;
            Position = position;
            Word = word;
        }
    }
    #endregion
}

