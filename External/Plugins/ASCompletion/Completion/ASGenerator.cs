using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ASCompletion.Context;
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
    public class ASGenerator
    {
        #region context detection (ie. entry points)

        internal const string patternEvent = "Listener\\s*\\((\\s*([a-z_0-9.\\\"']+)\\s*,)?\\s*(?<event>[a-z_0-9.\\\"']+)\\s*,\\s*(this\\.)?{0}";
        const string patternAS2Delegate = @"\.\s*create\s*\(\s*[a-z_0-9.]+,\s*{0}";
        const string patternVarDecl = @"\s*{0}\s*:\s*{1}";
        const string patternMethod = @"{0}\s*\(";
        const string patternMethodDecl = @"function\s+{0}\s*\(";
        const string patternClass = @"new\s*{0}";
        const string BlankLine = "$(Boundary)\n\n";
        const string NewLine = "$(Boundary)\n";
        static private Regex reModifiers = new Regex("^\\s*(\\$\\(Boundary\\))?([a-z ]+)(function|var|const)", RegexOptions.Compiled);
        static private Regex reSuperCall = new Regex("^super\\s*\\(", RegexOptions.Compiled);

        static internal string contextToken;
        static internal string contextParam;
        static internal Match contextMatch;
        static internal ASResult contextResolved;
        static internal MemberModel contextMember;
        static private bool firstVar;

        static private bool IsHaxe
        {
            get { return ASContext.Context.CurrentModel.haXe; }
        }

        static public bool HandleGeneratorCompletion(ScintillaControl Sci, bool autoHide, string word)
        {
            ContextFeatures features = ASContext.Context.Features;
            if (features.overrideKey != null && word == features.overrideKey)
                return HandleOverrideCompletion(autoHide);
            return false;
        }

        public static void ContextualGenerator(ScintillaControl Sci, List<ICompletionListItem> options)
        {
            if (ASContext.Context is ASContext) (ASContext.Context as ASContext).UpdateCurrentFile(false); // update model
            if ((ASContext.Context.CurrentClass.Flags & (FlagType.Enum | FlagType.TypeDef)) > 0) return;

            lookupPosition = -1;
            int position = Sci.CurrentPos;
            int style = Sci.BaseStyleAt(position);
            if (style == 19) // on keyword
                return;

            bool isNotInterface = (ASContext.Context.CurrentClass.Flags & FlagType.Interface) == 0;
            int line = Sci.LineFromPosition(position);
            contextToken = Sci.GetWordFromPosition(position);
            contextMatch = null;

            FoundDeclaration found = GetDeclarationAtLine(Sci, line);
            string text = Sci.GetLine(line);
            bool suggestItemDeclaration = false;

            if (isNotInterface && ASComplete.IsLiteralStyle(style))
            {
                ShowConvertToConst(found, options);
                return;
            }

            ASResult resolve = ASComplete.GetExpressionType(Sci, Sci.WordEndPosition(position, true));
            contextResolved = resolve;
            
            // ignore automatic vars (MovieClip members)
            if (isNotInterface
                && resolve.Member != null
                && (((resolve.Member.Flags & FlagType.AutomaticVar) > 0) || (resolve.InClass != null && resolve.InClass.QualifiedName == "Object")))
            {
                resolve.Member = null;
                resolve.Type = null;
            }

            if (isNotInterface && found.inClass != ClassModel.VoidClass && contextToken != null)
            {
                if (resolve.Member == null && resolve.Type != null
                    && (resolve.Type.Flags & FlagType.Interface) > 0) // implement interface
                {
                    contextParam = resolve.Type.Type;
                    ShowImplementInterface(found, options);
                    return;
                }

                if (resolve.Member != null && !ASContext.Context.CurrentClass.IsVoid()
                    && (resolve.Member.Flags & FlagType.LocalVar) > 0) // promote to class var
                {
                    contextMember = resolve.Member;
                    ShowPromoteLocalAndAddParameter(found, options);
                    return;
                }
            }
            
            if (contextToken != null && resolve.Member == null) // import declaration
            {
                if ((resolve.Type == null || resolve.Type.IsVoid() || !ASContext.Context.IsImported(resolve.Type, line)) && CheckAutoImport(resolve, options)) return;
                if (resolve.Type == null)
                {
                    suggestItemDeclaration = ASComplete.IsTextStyle(Sci.BaseStyleAt(position - 1));
                }
            }

            if (isNotInterface && found.member != null)
            {
                // private var -> property
                if ((found.member.Flags & FlagType.Variable) > 0 && (found.member.Flags & FlagType.LocalVar) == 0)
                {
                    // maybe we just want to import the member's non-imported type
                    Match m = Regex.Match(text, String.Format(patternVarDecl, found.member.Name, contextToken));
                    if (m.Success)
                    {
                        contextMatch = m;
                        ClassModel type = ASContext.Context.ResolveType(contextToken, ASContext.Context.CurrentModel);
                        if (type.IsVoid() && CheckAutoImport(resolve, options))
                            return;
                    }
                    ShowGetSetList(found, options);
                    return;
                }
                // inside a function
                if ((found.member.Flags & (FlagType.Function | FlagType.Getter | FlagType.Setter)) > 0
                    && resolve.Member == null && resolve.Type == null)
                {
                    if (IsHaxe)
                    {
                        if (contextToken == "get")
                        {
                            ShowGetterList(found, options);
                            return;
                        }
                        if (contextToken == "set")
                        {
                            ShowSetterList(found, options);
                            return;
                        }
                    }
                    if (contextToken != null)
                    {
                        // "generate event handlers" suggestion
                        string re = String.Format(patternEvent, contextToken);
                        Match m = Regex.Match(text, re, RegexOptions.IgnoreCase);
                        if (m.Success)
                        {
                            contextMatch = m;
                            contextParam = CheckEventType(m.Groups["event"].Value);
                            ShowEventList(found, options);
                            return;
                        }
                        m = Regex.Match(text, String.Format(patternAS2Delegate, contextToken), RegexOptions.IgnoreCase);
                        if (m.Success)
                        {
                            contextMatch = m;
                            ShowDelegateList(found, options);
                            return;
                        }
                        // suggest delegate
                        if (ASContext.Context.Features.hasDelegates)
                        {
                            m = Regex.Match(text, @"([a-z0-9_.]+)\s*\+=\s*" + contextToken, RegexOptions.IgnoreCase);
                            if (m.Success)
                            {
                                int offset = Sci.PositionFromLine(Sci.LineFromPosition(position))
                                    + m.Groups[1].Index + m.Groups[1].Length;
                                resolve = ASComplete.GetExpressionType(Sci, offset);
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
                            int regexIndex = m.Index + Sci.PositionFromLine(Sci.CurrentLine);
                            GenerateDefaultHandlerName(Sci, position, regexIndex, m.Groups["event"].Value, true);
                            resolve = ASComplete.GetExpressionType(Sci, Sci.CurrentPos);
                            if (resolve.Member == null || (resolve.Member.Flags & FlagType.AutomaticVar) > 0)
                            {
                                contextMatch = m;
                                contextParam = CheckEventType(m.Groups["event"].Value);
                                ShowEventList(found, options);
                            }
                            return;
                        }

                        // insert default delegate name, then "generate delegate" suggestion
                        if (ASContext.Context.Features.hasDelegates)
                        {
                            m = Regex.Match(text, @"([a-z0-9_.]+)\s*\+=\s*", RegexOptions.IgnoreCase);
                            if (m.Success)
                            {
                                int offset = Sci.PositionFromLine(Sci.LineFromPosition(position))
                                        + m.Groups[1].Index + m.Groups[1].Length;
                                resolve = ASComplete.GetExpressionType(Sci, offset);
                                if (resolve.Member != null)
                                {
                                    contextMember = ResolveDelegate(resolve.Member.Type, resolve.InFile);
                                    string delegateName = resolve.Member.Name;
                                    if (delegateName.StartsWithOrdinal("on")) delegateName = delegateName.Substring(2);
                                    GenerateDefaultHandlerName(Sci, position, offset, delegateName, false);
                                    resolve = ASComplete.GetExpressionType(Sci, Sci.CurrentPos);
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
                if ((found.member.Flags & FlagType.Function) > 0
                    && found.member.Parameters != null && (found.member.Parameters.Count > 0)
                    && resolve.Member != null && (resolve.Member.Flags & FlagType.ParameterVar) > 0)
                {
                    contextMember = resolve.Member;
                    ShowFieldFromParameter(found, options);
                    return;
                }

                // "add to interface" suggestion
                if (resolve.Member != null
                    && resolve.Member.Name == found.member.Name
                    && line == found.member.LineFrom
                    && ((found.member.Flags & FlagType.Function) > 0
                        || (found.member.Flags & FlagType.Getter) > 0
                        || (found.member.Flags & FlagType.Setter) > 0)
                    && found.inClass != ClassModel.VoidClass
                    && found.inClass.Implements != null
                    && found.inClass.Implements.Count > 0)
                {
                    string funcName = found.member.Name;
                    FlagType flags = found.member.Flags & ~FlagType.Access;
                    
                    List<string> interfaces = new List<string>();
                    foreach (string interf in found.inClass.Implements)
                    {
                        bool skip = false;
                        ClassModel cm = ASContext.Context.ResolveType(interf, ASContext.Context.CurrentModel);
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
                int curLine = Sci.CurrentLine;
                string ln = Sci.GetLine(curLine).TrimEnd();
                if (ln.Length > 0 && !ln.Contains('=')
                    && ln.Length <= Sci.CurrentPos - Sci.PositionFromLine(curLine)) // cursor at end of line
                {
                    var returnType = GetStatementReturnType(Sci, found.inClass, Sci.GetLine(curLine), Sci.PositionFromLine(curLine));
                    if (returnType.resolve.Member?.Type == ASContext.Context.Features.voidKey) return;
                    if (returnType.resolve.Type == null && returnType.resolve.Context?.WordBefore == "new") ShowNewClassList(found, options, returnType.resolve.Context);
                    else ShowAssignStatementToVarList(found, options, returnType);
                    return;
                }
            }
            
            // suggest generate constructor / toString
            if (isNotInterface && found.member == null && found.inClass != ClassModel.VoidClass && contextToken == null)
            {
                bool hasConstructor = false;
                bool hasToString = false;
                foreach (MemberModel m in ASContext.Context.CurrentClass.Members)
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

            if (isNotInterface 
                && resolve.Member != null
                && resolve.Type != null
                && resolve.Type.QualifiedName == ASContext.Context.Features.stringKey
                && found.inClass != ClassModel.VoidClass)
            {
                int lineStartPos = Sci.PositionFromLine(Sci.CurrentLine);
                string lineStart = text.Substring(0, Sci.CurrentPos - lineStartPos);
                Match m = Regex.Match(lineStart, String.Format(@"new\s+(?<event>\w+)\s*\(\s*\w+", lineStart));
                if (m.Success)
                {
                    Group g = m.Groups["event"];
                    ASResult eventResolve = ASComplete.GetExpressionType(Sci, lineStartPos + g.Index + g.Length);
                    if (eventResolve != null && eventResolve.Type != null)
                    {
                        ClassModel aType = eventResolve.Type;
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
                    Match m = Regex.Match(text, String.Format(patternClass, contextToken));
                    if (m.Success)
                    {
                        contextMatch = m;
                        ShowNewClassList(found, options);
                    }
                    else if (!found.inClass.IsVoid())
                    {
                        m = Regex.Match(text, String.Format(patternMethod, contextToken));
                        if (m.Success)
                        {
                            contextMatch = m;
                            ShowNewMethodList(found, options);
                        }
                        else ShowNewVarList(found, options);
                    }
                }
                else
                {
                    if (resolve.InClass != null
                        && resolve.InClass.InFile != null
                        && resolve.Member != null
                        && (resolve.Member.Flags & FlagType.Function) > 0
                        && File.Exists(resolve.InClass.InFile.FileName)
                        && !resolve.InClass.InFile.FileName.StartsWithOrdinal(PathHelper.AppDir))
                    {
                        Match m = Regex.Match(text, String.Format(patternMethodDecl, contextToken));
                        Match m2 = Regex.Match(text, String.Format(patternMethod, contextToken));
                        if (!m.Success && m2.Success)
                        {
                            contextMatch = m;
                            ShowChangeMethodDeclList(found, options);
                        }
                    }
                    else if (resolve.Type != null
                        && resolve.Type.InFile != null
                        && resolve.RelClass != null
                        && File.Exists(resolve.Type.InFile.FileName)
                        && !resolve.Type.InFile.FileName.StartsWithOrdinal(PathHelper.AppDir))
                    {
                        Match m = Regex.Match(text, String.Format(patternClass, contextToken));
                        if (m.Success)
                        {
                            contextMatch = m;
                            ShowChangeConstructorDeclList(found, options);
                        }
                    }
                }
            }
            // TODO: Empty line, show generators list? yep
        }

        private static MemberModel ResolveDelegate(string type, FileModel inFile)
        {
            foreach (MemberModel def in inFile.Members)
                if (def.Name == type && (def.Flags & FlagType.Delegate) > 0)
                    return def;

            if (type.IndexOf('.') < 0)
            {
                string dotType = '.' + type;
                MemberList imports = ASContext.Context.ResolveImports(inFile);
                foreach (MemberModel import in imports)
                    if (import.Type.EndsWithOrdinal(dotType))
                    {
                        type = import.Type;
                        break;
                    }
            }

            MemberList known = ASContext.Context.GetAllProjectClasses();
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
            if (target != null) target = target.TrimStart(new char[] { '_' });

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
            if (c <= 32) if (closeBrace) sci.ReplaceSel(");"); else sci.ReplaceSel(";");

            sci.SetSel(position, position);
        }

        private static FoundDeclaration GetDeclarationAtLine(ScintillaControl sci, int line)
        {
            FoundDeclaration result = new FoundDeclaration();
            FileModel model = ASContext.Context.CurrentModel;

            foreach (MemberModel member in model.Members)
            {
                if (member.LineFrom <= line && member.LineTo >= line)
                {
                    result.member = member;
                    return result;
                }
            }

            foreach (ClassModel aClass in model.Classes)
            {
                if (aClass.LineFrom <= line && aClass.LineTo >= line)
                {
                    result.inClass = aClass;
                    foreach (MemberModel member in aClass.Members)
                    {
                        if (member.LineFrom <= line && member.LineTo >= line)
                        {
                            result.member = member;
                            return result;
                        }
                    }
                    return result;
                }
            }
            return result;
        }

        static bool CheckAutoImport(ASResult expr, List<ICompletionListItem> options)
        {
            if (ASContext.Context.CurrentClass.Equals(expr.RelClass)) return false;
            MemberList allClasses = ASContext.Context.GetAllProjectClasses();
            if (allClasses != null)
            {
                var names = new HashSet<string>();
                List<MemberModel> matches = new List<MemberModel>();
                string dotToken = "." + contextToken;
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
            if (name.IndexOf('"') >= 0) return "Event";
            if (name.IndexOf('.') > 0) name = name.Substring(0, name.IndexOf('.'));
            ClassModel model = ASContext.Context.ResolveType(name, ASContext.Context.CurrentModel);
            if (model.IsVoid() || model.Name == "Event") return "Event";
            model.ResolveExtends();
            while (!model.IsVoid() && model.Name != "Event")
                model = model.Extends;
            if (model.Name == "Event") return name;
            else return "Event";
        }
        #endregion

        #region generators lists

        private static void ShowImportClass(List<MemberModel> matches, List<ICompletionListItem> options)
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

        private static void ShowPromoteLocalAndAddParameter(FoundDeclaration found, List<ICompletionListItem> options)
        {
            string label = TextHelper.GetString("ASCompletion.Label.PromoteLocal");
            string labelMove = TextHelper.GetString("ASCompletion.Label.MoveDeclarationOnTop");
            string labelParam = TextHelper.GetString("ASCompletion.Label.AddAsParameter");
            options.Add(new GeneratorItem(label, GeneratorJobType.PromoteLocal, found.member, found.inClass));
            options.Add(new GeneratorItem(labelMove, GeneratorJobType.MoveLocalUp, found.member, found.inClass));
            options.Add(new GeneratorItem(labelParam, GeneratorJobType.AddAsParameter, found.member, found.inClass));
        }

        private static void ShowConvertToConst(FoundDeclaration found, List<ICompletionListItem> options)
        {
            string label = TextHelper.GetString("ASCompletion.Label.ConvertToConst");
            options.Add(new GeneratorItem(label, GeneratorJobType.ConvertToConst, found.member, found.inClass));
        }

        private static void ShowImplementInterface(FoundDeclaration found, List<ICompletionListItem> options)
        {
            string label = TextHelper.GetString("ASCompletion.Label.ImplementInterface");
            options.Add(new GeneratorItem(label, GeneratorJobType.ImplementInterface, null, found.inClass));
        }

        private static void ShowNewVarList(FoundDeclaration found, List<ICompletionListItem> options)
        {
            bool generateClass = GetLangIsValid();
            ScintillaControl sci = ASContext.CurSciControl;
            int currentPos = sci.CurrentPos;
            ASResult exprAtCursor = ASComplete.GetExpressionType(sci, sci.WordEndPosition(currentPos, true));
            if (exprAtCursor == null || exprAtCursor.InClass == null || found.inClass.QualifiedName.Equals(exprAtCursor.RelClass.QualifiedName))
                exprAtCursor = null;
            ASResult exprLeft = null;
            int curWordStartPos = sci.WordStartPosition(currentPos, true);
            if ((char)sci.CharAt(curWordStartPos - 1) == '.') exprLeft = ASComplete.GetExpressionType(sci, curWordStartPos - 1);
            if (exprLeft != null && exprLeft.Type == null) exprLeft = null;
            if (exprLeft != null)
            {
                if (exprLeft.Type.InFile != null && !File.Exists(exprLeft.Type.InFile.FileName)) return;
                generateClass = false;
                ClassModel curClass = ASContext.Context.CurrentClass;
                if (!IsHaxe)
                {
                    if (exprLeft.Type.Equals(curClass)) exprLeft = null;
                }
                else 
                {
                    while (!curClass.IsVoid())
                    {
                        if (curClass.Equals(exprLeft.Type))
                        {
                            exprLeft = null;
                            break;
                        }
                        curClass.ResolveExtends();
                        curClass = curClass.Extends;
                    }
                }
            }
            string label;
            if ((exprAtCursor != null && exprAtCursor.RelClass != null && (exprAtCursor.RelClass.Flags & FlagType.Interface) > 0)
                || (found.inClass != null && (found.inClass.Flags & FlagType.Interface) > 0))
            {
                label = TextHelper.GetString("ASCompletion.Label.GenerateFunctionInterface");
                options.Add(new GeneratorItem(label, GeneratorJobType.FunctionPublic, found.member, found.inClass));
            }
            else
            {
                string textAtCursor = sci.GetWordFromPosition(currentPos);
                bool isConst = textAtCursor != null && textAtCursor.ToUpper().Equals(textAtCursor);
                if (isConst)
                {
                    label = TextHelper.GetString("ASCompletion.Label.GenerateConstant");
                    options.Add(new GeneratorItem(label, GeneratorJobType.Constant, found.member, found.inClass));
                }

                bool genProtectedDecl = GetDefaultVisibility(found.inClass) == Visibility.Protected;
                if (exprAtCursor == null && exprLeft == null)
                {
                    if (genProtectedDecl) label = TextHelper.GetString("ASCompletion.Label.GenerateProtectedVar");
                    else label = TextHelper.GetString("ASCompletion.Label.GeneratePrivateVar");
                    options.Add(new GeneratorItem(label, GeneratorJobType.Variable, found.member, found.inClass));
                }

                label = TextHelper.GetString("ASCompletion.Label.GeneratePublicVar");
                options.Add(new GeneratorItem(label, GeneratorJobType.VariablePublic, found.member, found.inClass));

                if (exprAtCursor == null && exprLeft == null)
                {
                    if (genProtectedDecl) label = TextHelper.GetString("ASCompletion.Label.GenerateProtectedFunction");
                    else label = TextHelper.GetString("ASCompletion.Label.GeneratePrivateFunction");
                    options.Add(new GeneratorItem(label, GeneratorJobType.Function, found.member, found.inClass));
                }

                label = TextHelper.GetString("ASCompletion.Label.GenerateFunctionPublic");
                options.Add(new GeneratorItem(label, GeneratorJobType.FunctionPublic, found.member, found.inClass));

                if (generateClass)
                {
                    label = TextHelper.GetString("ASCompletion.Label.GenerateClass");
                    options.Add(new GeneratorItem(label, GeneratorJobType.Class, found.member, found.inClass));
                }
            }
        }

        private static void ShowChangeMethodDeclList(FoundDeclaration found, List<ICompletionListItem> options)
        {
            string label = TextHelper.GetString("ASCompletion.Label.ChangeMethodDecl");
            options.Add(new GeneratorItem(label, GeneratorJobType.ChangeMethodDecl, found.member, found.inClass));
        }

        private static void ShowChangeConstructorDeclList(FoundDeclaration found, List<ICompletionListItem> options)
        {
            string label = TextHelper.GetString("ASCompletion.Label.ChangeConstructorDecl");
            options.Add(new GeneratorItem(label, GeneratorJobType.ChangeConstructorDecl, found.member, found.inClass));
        }

        private static void ShowNewMethodList(FoundDeclaration found, List<ICompletionListItem> options)
        {
            ScintillaControl sci = ASContext.CurSciControl;
            ASResult result = ASComplete.GetExpressionType(sci, sci.WordEndPosition(sci.CurrentPos, true));
            if (result == null || result.RelClass == null || found.inClass.QualifiedName.Equals(result.RelClass.QualifiedName))
                result = null;
            string label;
            ClassModel inClass = result != null ? result.RelClass : found.inClass;
            bool isInterface = (inClass.Flags & FlagType.Interface) > 0;
            if (!isInterface && result == null)
            {
                if (GetDefaultVisibility(found.inClass) == Visibility.Protected)
                    label = TextHelper.GetString("ASCompletion.Label.GenerateProtectedFunction");
                else label = TextHelper.GetString("ASCompletion.Label.GeneratePrivateFunction");
                options.Add(new GeneratorItem(label, GeneratorJobType.Function, found.member, found.inClass));
            }
            if (isInterface) label = TextHelper.GetString("ASCompletion.Label.GenerateFunctionInterface");
            else label = TextHelper.GetString("ASCompletion.Label.GenerateFunctionPublic");
            options.Add(new GeneratorItem(label, GeneratorJobType.FunctionPublic, found.member, found.inClass));
            label = TextHelper.GetString("ASCompletion.Label.GeneratePublicCallback");
            options.Add(new GeneratorItem(label, GeneratorJobType.VariablePublic, found.member, found.inClass));
        }

        static void ShowAssignStatementToVarList(FoundDeclaration found, ICollection<ICompletionListItem> options, StatementReturnType data)
        {
            var label = TextHelper.GetString("ASCompletion.Label.AssignStatementToVar");
            options.Add(new GeneratorItem(label, GeneratorJobType.AssignStatementToVar, found.member, found.inClass, data));
        }

        private static void ShowNewClassList(FoundDeclaration found, ICollection<ICompletionListItem> options) => ShowNewClassList(found, options, null);

        static void ShowNewClassList(FoundDeclaration found, ICollection<ICompletionListItem> options, ASExpr expr)
        {
            var label = TextHelper.GetString("ASCompletion.Label.GenerateClass");
            options.Add(new GeneratorItem(label, GeneratorJobType.Class, found.member, found.inClass, expr));
        }

        private static void ShowConstructorAndToStringList(FoundDeclaration found, bool hasConstructor, bool hasToString, List<ICompletionListItem> options)
        {
            if (GetLangIsValid())
            {
                if (!hasConstructor)
                {
                    string labelClass = TextHelper.GetString("ASCompletion.Label.GenerateConstructor");
                    options.Add(new GeneratorItem(labelClass, GeneratorJobType.Constructor, found.member, found.inClass));
                }

                if (!hasToString)
                {
                    string labelClass = TextHelper.GetString("ASCompletion.Label.GenerateToString");
                    options.Add(new GeneratorItem(labelClass, GeneratorJobType.ToString, found.member, found.inClass));
                }
            }
        }

        private static void ShowEventMetatagList(FoundDeclaration found, List<ICompletionListItem> options)
        {
            string label = TextHelper.GetString("ASCompletion.Label.GenerateEventMetatag");
            options.Add(new GeneratorItem(label, GeneratorJobType.EventMetatag, found.member, found.inClass));
        }

        private static void ShowFieldFromParameter(FoundDeclaration found, List<ICompletionListItem> options)
        {
            if (GetLangIsValid())
            {
                Hashtable parameters = new Hashtable();
                parameters["scope"] = GetDefaultVisibility(found.inClass);
                string label;
                if (GetDefaultVisibility(found.inClass) == Visibility.Protected)
                    label = TextHelper.GetString("ASCompletion.Label.GenerateProtectedFieldFromParameter");
                else label = TextHelper.GetString("ASCompletion.Label.GeneratePrivateFieldFromParameter");
                options.Add(new GeneratorItem(label, GeneratorJobType.FieldFromParameter, found.member, found.inClass, parameters));
                parameters = new Hashtable();
                parameters["scope"] = Visibility.Public;
                label = TextHelper.GetString("ASCompletion.Label.GeneratePublicFieldFromParameter");
                options.Add(new GeneratorItem(label, GeneratorJobType.FieldFromParameter, found.member, found.inClass, parameters));
            }
        }

        static void ShowAddInterfaceDefList(FoundDeclaration found, IEnumerable<string> interfaces, ICollection<ICompletionListItem> options)
        {
            if (!GetLangIsValid()) return;
            var label = TextHelper.GetString("ASCompletion.Label.AddInterfaceDef");
            foreach (var interf in interfaces)
            {
                options.Add(new GeneratorItem(String.Format(label, interf), GeneratorJobType.AddInterfaceDef, found.member, found.inClass, interf));
            }
        }

        private static void ShowDelegateList(FoundDeclaration found, List<ICompletionListItem> options)
        {
            string label = String.Format(TextHelper.GetString("ASCompletion.Label.GenerateHandler"), "Delegate");
            options.Add(new GeneratorItem(label, GeneratorJobType.Delegate, found.member, found.inClass));
        }

        internal static void ShowEventList(FoundDeclaration found, List<ICompletionListItem> options)
        {
            string tmp = TextHelper.GetString("ASCompletion.Label.GenerateHandler");
            string labelEvent = String.Format(tmp, "Event");
            string labelDataEvent = String.Format(tmp, "DataEvent");
            string labelContext = String.Format(tmp, contextParam);
            string[] choices;
            if (contextParam != "Event") choices = new string[] { labelContext, labelEvent };
            else if (HasDataEvent()) choices = new string[] { labelEvent, labelDataEvent };
            else choices = new string[] { labelEvent };

            for (int i = 0; i < choices.Length; i++)
            {
                options.Add(new GeneratorItem(choices[i],
                    choices[i] == labelContext ? GeneratorJobType.ComplexEvent : GeneratorJobType.BasicEvent,
                    found.member, found.inClass));
            }
        }

        private static bool HasDataEvent()
        {
            return !ASContext.Context.ResolveType("flash.events.DataEvent", ASContext.Context.CurrentModel).IsVoid();
        }

        private static void ShowGetSetList(FoundDeclaration found, List<ICompletionListItem> options)
        {
            string name = GetPropertyNameFor(found.member);
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
                options.Add(new GeneratorItem(label, GeneratorJobType.GetterSetter, found.member, found.inClass));
            }
            ShowGetterList(found, options);
            ShowSetterList(found, options);
        }

        static void ShowGetterList(FoundDeclaration found, ICollection<ICompletionListItem> options)
        {
            var name = GetPropertyNameFor(found.member);
            var result = new ASResult();
            ASComplete.FindMember(name, ASContext.Context.CurrentClass, result, FlagType.Getter, 0);
            if (!result.IsNull()) return;
            var label = TextHelper.GetString("ASCompletion.Label.GenerateGet");
            options.Add(new GeneratorItem(label, GeneratorJobType.Getter, found.member, found.inClass));
        }

        static void ShowSetterList(FoundDeclaration found, ICollection<ICompletionListItem> options)
        {
            var name = GetPropertyNameFor(found.member);
            var result = new ASResult();
            ASComplete.FindMember(name, ASContext.Context.CurrentClass, result, FlagType.Setter, 0);
            if (!result.IsNull()) return;
            var label = TextHelper.GetString("ASCompletion.Label.GenerateSet");
            options.Add(new GeneratorItem(label, GeneratorJobType.Setter, found.member, found.inClass));
        }

        private static bool GetLangIsValid()
        {
            IProject project = PluginBase.CurrentProject;
            if (project == null)
                return false;

            return project.Language.StartsWithOrdinal("as")
                || project.Language.StartsWithOrdinal("haxe")
                || project.Language.StartsWithOrdinal("loom");
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

        public static void GenerateJob(GeneratorJobType job, MemberModel member, ClassModel inClass, string itemLabel, Object data)
        {
            ScintillaControl sci = ASContext.CurSciControl;
            lookupPosition = sci.CurrentPos;

            int position;
            MemberModel latest;
            bool detach = true;
            switch (job)
            {
                case GeneratorJobType.Getter:
                case GeneratorJobType.Setter:
                case GeneratorJobType.GetterSetter:
                    GenerateProperty(job, member, inClass, sci);
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

                case GeneratorJobType.Function:
                case GeneratorJobType.FunctionPublic:
                    sci.BeginUndoAction();
                    try
                    {
                        GenerateFunctionJob(job, sci, member, detach, inClass);
                    }
                    finally
                    {
                        sci.EndUndoAction();
                    }
                    break;

                case GeneratorJobType.ImplementInterface:
                    ClassModel iType = ASContext.Context.ResolveType(contextParam, inClass.InFile ?? ASContext.Context.CurrentModel );
                    if (iType.IsVoid()) return;

                    latest = GetLatestMemberForFunction(inClass, Visibility.Public, null);
                    if (latest == null)
                        latest = FindLatest(0, 0, inClass, false, false);

                    if (latest == null)
                    {
                        position = GetBodyStart(inClass.LineFrom, inClass.LineTo, sci);
                        detach = false;
                    }
                    else
                        position = sci.PositionFromLine(latest.LineTo + 1) - ((sci.EOLMode == 0) ? 2 : 1);

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

                        GenerateVariable(newMember, position, detach);
                        sci.SetSel(lookupPosition, lookupPosition);
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
                        int offset = InsertImport(member, true);
                        position += offset;
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

                case GeneratorJobType.Constructor:
                    member = new MemberModel(inClass.Name, inClass.QualifiedName, FlagType.Constructor | FlagType.Function, Visibility.Public);
                    GenerateFunction(member, sci.CurrentPos, false, inClass);
                    break;

                case GeneratorJobType.ToString:
                    sci.BeginUndoAction();
                    try
                    {
                        GenerateToString(sci, member, inClass);
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
                        GenerateFieldFromParameter(sci, member, inClass, (Visibility)(((Hashtable)data)["scope"]));
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
                        AddInterfaceDefJob(sci, member, inClass, (String)data);
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

                case GeneratorJobType.ChangeConstructorDecl:
                    sci.BeginUndoAction();
                    try
                    {
                        ChangeConstructorDecl(sci, inClass);
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
                        if (data is StatementReturnType) AssignStatementToVar(sci, inClass, (StatementReturnType)data);
                        else AssignStatementToVar(sci, inClass);
                    }
                    finally
                    {
                        sci.EndUndoAction();
                    }
                    break;
            }
        }

        private static void GenerateProperty(GeneratorJobType job, MemberModel member, ClassModel inClass, ScintillaControl sci)
        {
            string name = GetPropertyNameFor(member);
            PropertiesGenerationLocations location = ASContext.CommonSettings.PropertiesGenerationLocation;

            var latest = TemplateUtils.GetTemplateBlockMember(sci, TemplateUtils.GetBoundary("AccessorsMethods"));
            if (latest != null)
            {
                location = PropertiesGenerationLocations.AfterLastPropertyDeclaration;
            }
            else
            {
                if (location == PropertiesGenerationLocations.AfterLastPropertyDeclaration)
                {
                    if (IsHaxe) latest = FindLatest(FlagType.Function, 0, inClass, false, false);
                    else latest = FindLatest(FlagType.Getter | FlagType.Setter, 0, inClass, false, false);
                }
                else latest = member;
            }
            if (latest == null) return;

            sci.BeginUndoAction();
            try
            {
                if (IsHaxe)
                {
                    if (name == null) name = member.Name;
                    string args = "(default, default)";
                    if (job == GeneratorJobType.GetterSetter) args = "(get, set)";
                    else if (job == GeneratorJobType.Getter) args = "(get, null)";
                    else if (job == GeneratorJobType.Setter) args = "(default, set)";
                    MakeHaxeProperty(sci, member, args);
                }
                else
                {
                    if ((member.Access & Visibility.Public) > 0) // hide member
                    {
                        MakePrivate(sci, member, inClass);
                    }
                    if (name == null) // rename var with starting underscore
                    {
                        name = member.Name;
                        string newName = GetNewPropertyNameFor(member);
                        if (RenameMember(sci, member, newName)) member.Name = newName;
                    }
                }

                int atLine = latest.LineTo + 1;
                if (location == PropertiesGenerationLocations.BeforeVariableDeclaration)
                    atLine = latest.LineTo;
                int position = sci.PositionFromLine(atLine) - ((sci.EOLMode == 0) ? 2 : 1);

                if (job == GeneratorJobType.GetterSetter)
                {
                    sci.SetSel(position, position);
                    GenerateGetterSetter(name, member, position);
                }
                else
                {
                    if (job != GeneratorJobType.Getter)
                    {
                        sci.SetSel(position, position);
                        GenerateSetter(name, member, position);
                    }
                    if (job != GeneratorJobType.Setter)
                    {
                        sci.SetSel(position, position);
                        GenerateGetter(name, member, position);
                    }
                }
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
            var resolve = returnType.resolve;
            string type = null;
            if (resolve.Member == null && (resolve.Type.Flags & FlagType.Class) != 0
                && resolve.Type.Name != ctx.Features.booleanKey
                && resolve.Type.Name != "Function"
                && !string.IsNullOrEmpty(resolve.Path) && !char.IsDigit(resolve.Path[0]))
            {
                var expr = ASComplete.GetExpression(sci, returnType.position);
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

            var word = returnType.word;
            if (!string.IsNullOrEmpty(word) && char.IsDigit(word[0])) word = null;
            string varname = null;
            if (string.IsNullOrEmpty(type) && !resolve.IsNull())
            {
                if (resolve.Member?.Type != null) type = resolve.Member.Type;
                else if (resolve.Type?.Name != null) type = resolve.Type.QualifiedName;
                if (resolve.Member?.Name != null) varname = GuessVarName(resolve.Member.Name, type);
            }
            if (!string.IsNullOrEmpty(word) && (string.IsNullOrEmpty(type) || Regex.IsMatch(type, "(<[^]]+>)"))) word = null;
            if (type == ctx.Features.voidKey) type = null;
            if (varname == null) varname = GuessVarName(word, type);
            if (varname != null && varname == word) varname = varname.Length == 1 ? varname + "1" : varname[0] + "";
            varname = AvoidKeyword(varname);
            string cleanType = null;
            if (type != null) cleanType = FormatType(GetShortType(type));
            
            string template = TemplateUtils.GetTemplate("AssignVariable");
            template = TemplateUtils.ReplaceTemplateVariable(template, "Name", varname);
            template = TemplateUtils.ReplaceTemplateVariable(template, "Type", cleanType);

            var pos = GetStartOfStatement(sci, sci.CurrentPos, resolve);
            sci.SetSel(pos, pos);
            InsertCode(pos, template, sci);

            if (ASContext.Context.Settings.GenerateImports && type != null)
            {
                var inClassForImport = resolve.InClass ?? resolve.RelClass ?? inClass;
                var types = GetQualifiedTypes(new [] {type}, inClassForImport.InFile);
                AddImportsByName(types, sci.LineFromPosition(pos));
            }
        }

        public static string AvoidKeyword(string word)
        {
            var features = ASContext.Context.Features;
            return features.accessKeywords.Contains(word)
                   || features.codeKeywords.Contains(word)
                   || features.declKeywords.Contains(word)
                   || features.typesKeywords.Contains(word)
                   || features.typesPreKeys.Contains(word)
                ? $"{word}Value"
                : word;
        }

        private static void EventMetatag(ScintillaControl sci, ClassModel inClass)
        {
            ASResult resolve = ASComplete.GetExpressionType(sci, sci.WordEndPosition(sci.CurrentPos, true));
            string line = sci.GetLine(inClass.LineFrom);
            int position = sci.PositionFromLine(inClass.LineFrom) + (line.Length - line.TrimStart().Length);

            string value = resolve.Member.Value;
            if (value != null)
            {
                if (value.StartsWith('\"'))
                {
                    value = value.Trim(new char[] { '"' });
                }
                else if (value.StartsWith('\''))
                {
                    value = value.Trim(new char[] { '\'' });
                }
            }
            else value = resolve.Member.Type;

            if (string.IsNullOrEmpty(value))
                return;

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

            sci.CurrentPos = position;
            sci.SetSel(position, position);
            InsertCode(position, template, sci);
        }

        private static void ConvertToConst(ScintillaControl sci, MemberModel member, ClassModel inClass, bool detach)
        {
            String suggestion = "NEW_CONST";
            String label = TextHelper.GetString("ASCompletion.Label.ConstName");
            String title = TextHelper.GetString("ASCompletion.Title.ConvertToConst");

            Hashtable info = new Hashtable();
            info["suggestion"] = suggestion;
            info["label"] = label;
            info["title"] = title;
            DataEvent de = new DataEvent(EventType.Command, "ProjectManager.LineEntryDialog", info);
            EventManager.DispatchEvent(null, de);
            if (!de.Handled)
                return;
            
            suggestion = (string)info["suggestion"];

            int position = sci.CurrentPos;
            int style = sci.BaseStyleAt(position);
            MemberModel latest = null;

            int wordPosEnd = position + 1;
            int wordPosStart = position;

            while (sci.BaseStyleAt(wordPosEnd) == style) wordPosEnd++;
            while (sci.BaseStyleAt(wordPosStart - 1) == style) wordPosStart--;
            
            sci.SetSel(wordPosStart, wordPosEnd);
            string word = sci.SelText;
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
                latest = GetLatestMemberForVariable(GeneratorJobType.Constant, inClass, 
                    Visibility.Private, new MemberModel("", "", FlagType.Static, 0));
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

            MemberModel m = NewMember(suggestion, member, FlagType.Variable | FlagType.Constant | FlagType.Static, GetDefaultVisibility(inClass));

            var features = ASContext.Context.Features;

            switch (style)
            {
                case 4:
                    m.Type = features.numberKey;
                    break;
                case 6:
                case 7:
                    m.Type = features.stringKey;
                    break;
            }

            m.Value = word;
            GenerateVariable(m, position, detach);
        }

        private static void ChangeMethodDecl(ScintillaControl sci, ClassModel inClass)
        {
            int wordPos = sci.WordEndPosition(sci.CurrentPos, true);
            List<FunctionParameter> functionParameters = ParseFunctionParameters(sci, wordPos);

            ASResult funcResult = ASComplete.GetExpressionType(sci, sci.WordEndPosition(sci.CurrentPos, true));
            if (funcResult == null || funcResult.Member == null) return;
            if (funcResult.InClass != null && !funcResult.InClass.Equals(inClass))
            {
                AddLookupPosition();
                lookupPosition = -1;

                ASContext.MainForm.OpenEditableDocument(funcResult.InClass.InFile.FileName, true);
                sci = ASContext.CurSciControl;

                FileModel fileModel = new FileModel();
                fileModel.Context = ASContext.Context;
                ASFileParser parser = new ASFileParser();
                parser.ParseSrc(fileModel, sci.Text);

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

            ChangeDecl(sci, inClass, funcResult.Member, functionParameters);
        }

        private static void ChangeConstructorDecl(ScintillaControl sci, ClassModel inClass)
        {
            int wordPos = sci.WordEndPosition(sci.CurrentPos, true);
            List<FunctionParameter> functionParameters = ParseFunctionParameters(sci, wordPos);
            ASResult funcResult = ASComplete.GetExpressionType(sci, sci.WordEndPosition(sci.CurrentPos, true));

            if (funcResult == null || funcResult.Type == null) return;
            if (!funcResult.Type.Equals(inClass))
            {
                AddLookupPosition();
                lookupPosition = -1;

                ASContext.MainForm.OpenEditableDocument(funcResult.Type.InFile.FileName, true);
                sci = ASContext.CurSciControl;

                FileModel fileModel = new FileModel(funcResult.Type.InFile.FileName);
                fileModel.Context = ASContext.Context;
                ASFileParser parser = new ASFileParser();
                parser.ParseSrc(fileModel, sci.Text);

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

            ChangeDecl(sci, inClass, funcResult.Member, functionParameters);
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

            if (paramsDiffer)
            {
                int app = 0;
                List<MemberModel> newParameters = new List<MemberModel>();
                List<MemberModel> existingParameters = memberModel.Parameters;
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
                        if (existingParameters != null && existingParameters.Count < functionParameters.Count)
                        {
                            app++;
                        }
                        newParameters.Add(new MemberModel(AvoidKeyword(p.paramName), p.paramType, FlagType.ParameterVar, 0));
                    }
                }
                memberModel.Parameters = newParameters;

                int posStart = sci.PositionFromLine(memberModel.LineFrom);
                int posEnd = sci.LineEndPosition(memberModel.LineTo);
                sci.SetSel(posStart, posEnd);
                string selectedText = sci.SelText;
                Regex rStart = new Regex(@"\s{1}" + memberModel.Name + @"\s*\(([^\)]*)\)(\s*:\s*([^({{|\n|\r|\s|;)]+))?");
                Match mStart = rStart.Match(selectedText);
                if (!mStart.Success)
                {
                    return;
                }

                int start = mStart.Index + posStart;
                int end = start + mStart.Length;

                sci.SetSel(start, end);

                string decl = TemplateUtils.ToDeclarationString(memberModel, TemplateUtils.GetTemplate("MethodDeclaration"));
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
        }

        private static void AddAsParameter(ScintillaControl sci, MemberModel member)
        {
            if (!RemoveLocalDeclaration(sci, contextMember)) return;

            int posStart = sci.PositionFromLine(member.LineFrom);
            int posEnd = sci.LineEndPosition(member.LineTo);
            sci.SetSel(posStart, posEnd);
            string selectedText = sci.SelText;
            Regex rStart = new Regex(@"\s{1}" + member.Name + @"\s*\(([^\)]*)\)(\s*:\s*([^({{|\n|\r|\s|;)]+))?");
            Match mStart = rStart.Match(selectedText);
            if (!mStart.Success)
                return;

            int start = mStart.Index + posStart + 1;
            int end = mStart.Index + posStart + mStart.Length;

            sci.SetSel(start, end);

            MemberModel memberCopy = (MemberModel) member.Clone();

            if (memberCopy.Parameters == null)
                memberCopy.Parameters = new List<MemberModel>();

            memberCopy.Parameters.Add(contextMember);

            string template = TemplateUtils.ToDeclarationString(memberCopy, TemplateUtils.GetTemplate("MethodDeclaration"));
            InsertCode(start, template, sci);

            int currPos = sci.LineEndPosition(sci.CurrentLine);

            sci.SetSel(currPos, currPos);
            sci.CurrentPos = currPos;
        }

        private static void AddInterfaceDefJob(ScintillaControl sci, MemberModel member, ClassModel inClass, string interf)
        {
            var context = ASContext.Context;
            ClassModel aType = context.ResolveType(interf, context.CurrentModel);
            if (aType.IsVoid()) return;

            FileModel fileModel = ASFileParser.ParseFile(context.CreateFileModel(aType.InFile.FileName));
            foreach (ClassModel cm in fileModel.Classes)
            {
                if (cm.QualifiedName.Equals(aType.QualifiedName))
                {
                    aType = cm;
                    break;
                }
            }

            string template;
            if ((member.Flags & FlagType.Getter) > 0)
            {
                template = TemplateUtils.GetTemplate("IGetter");
            }
            else if ((member.Flags & FlagType.Setter) > 0)
            {
                template = TemplateUtils.GetTemplate("ISetter");
            }
            else template = TemplateUtils.GetTemplate("IFunction");

            ASContext.MainForm.OpenEditableDocument(aType.InFile.FileName, true);
            sci = ASContext.CurSciControl;

            MemberModel latest = GetLatestMemberForFunction(aType, Visibility.Default, new MemberModel());
            int position;
            if (latest == null)
            {
                position = GetBodyStart(aType.LineFrom, aType.LineTo, sci);
            }
            else
            {
                position = sci.PositionFromLine(latest.LineTo + 1) - ((sci.EOLMode == 0) ? 2 : 1);
                template = NewLine + template;
            }
            sci.SetSel(position, position);
            sci.CurrentPos = position;
            template = TemplateUtils.ReplaceTemplateVariable(template, "Type", member.Type ?? context.Features.voidKey);
            template = TemplateUtils.ToDeclarationString(member, template);
            template = TemplateUtils.ReplaceTemplateVariable(template, "BlankLine", NewLine);
            template = TemplateUtils.ReplaceTemplateVariable(template, "Void", context.Features.voidKey);

            if (context.Settings.GenerateImports)
            {
                List<string> importsList = new List<string>();
                List<MemberModel> parms = member.Parameters;
                if (parms != null && parms.Count > 0)
                {
                    importsList.AddRange(from t in parms where t.Type != null select t.Type);
                }
                if (member.Type != null) importsList.Add(member.Type);
                if (importsList.Count > 0)
                {
                    var types = GetQualifiedTypes(importsList, inClass.InFile);
                    position += AddImportsByName(types, sci.LineFromPosition(position));
                }
            }

            sci.SetSel(position, position);
            sci.CurrentPos = position;

            InsertCode(position, template, sci);
        }

        private static void GenerateFieldFromParameter(ScintillaControl sci, MemberModel member, ClassModel inClass, Visibility scope)
        {
            int funcBodyStart = GetBodyStart(member.LineFrom, member.LineTo, sci, false);
            int fbsLine = sci.LineFromPosition(funcBodyStart);
            int endPos = sci.LineEndPosition(member.LineTo);

            sci.SetSel(funcBodyStart, endPos);
            string body = sci.SelText;
            string trimmed = body.TrimStart();

            Match m = reSuperCall.Match(trimmed);
            if (m.Success && m.Index == 0)
            {
                funcBodyStart = GetEndOfStatement(funcBodyStart + (body.Length - trimmed.Length), endPos, sci);
            }

            funcBodyStart = GetOrSetPointOfInsertion(funcBodyStart, endPos, fbsLine, sci);

            sci.SetSel(funcBodyStart, funcBodyStart);
            sci.CurrentPos = funcBodyStart;

            bool isVararg = false;
            string paramName = contextMember.Name;
            var paramType = contextMember.Type;
            if (paramName.StartsWithOrdinal("..."))
            {
                paramName = paramName.TrimStart(' ', '.');
                isVararg = true;
            }
            else if (inClass.InFile.haXe && paramName.StartsWithOrdinal("?"))
            {
                paramName = paramName.Remove(0, 1);
                if (!string.IsNullOrEmpty(paramType) && !paramType.StartsWith("Null<")) paramType = $"Null<{paramType}>";
            }
            string varName = paramName;
            string scopedVarName = varName;

            if ((scope & Visibility.Public) > 0)
            {
                if ((member.Flags & FlagType.Static) > 0)
                    scopedVarName = inClass.Name + "." + varName;
                else
                    scopedVarName = "this." + varName;
            }
            else
            {
                if (ASContext.CommonSettings.PrefixFields.Length > 0 && !varName.StartsWithOrdinal(ASContext.CommonSettings.PrefixFields))
                {
                    scopedVarName = varName = ASContext.CommonSettings.PrefixFields + varName;
                }

                if (ASContext.CommonSettings.GenerateScope || ASContext.CommonSettings.PrefixFields == "")
                {
                    if ((member.Flags & FlagType.Static) > 0)
                        scopedVarName = inClass.Name + "." + varName;
                    else
                        scopedVarName = "this." + varName;
                }
            }

            string template = TemplateUtils.GetTemplate("FieldFromParameter");
            template = TemplateUtils.ReplaceTemplateVariable(template, "Name", scopedVarName);
            template = TemplateUtils.ReplaceTemplateVariable(template, "Value", paramName);
            template += "\n$(Boundary)";

            SnippetHelper.InsertSnippetText(sci, funcBodyStart, template);

            //TODO: We also need to check parent classes!!!
            MemberList classMembers = inClass.Members;
            foreach (MemberModel classMember in classMembers)
                if (classMember.Name.Equals(varName))
                {
                    ASContext.Panel.RestoreLastLookupPosition();
                    return;
                }

            MemberModel latest = GetLatestMemberForVariable(GeneratorJobType.Variable, inClass, GetDefaultVisibility(inClass), new MemberModel());
            if (latest == null) return;

            int position = FindNewVarPosition(sci, inClass, latest);
            if (position <= 0) return;
            sci.SetSel(position, position);
            sci.CurrentPos = position;

            MemberModel mem = NewMember(varName, member, FlagType.Variable, scope);
            if (isVararg) mem.Type = "Array";
            else mem.Type = paramType;

            GenerateVariable(mem, position, true);
            ASContext.Panel.RestoreLastLookupPosition();
        }

        /// <summary>
        /// Tries to get the best position inside a code block, delimited by { and }, to add new code, inserting new lines if needed.
        /// </summary>
        /// <param name="lineFrom">The line inside the Scintilla document where the owner member of the body starts</param>
        /// <param name="lineTo">The line inside the Scintilla document where the owner member of the body ends</param>
        /// <param name="sci">The Scintilla control containing the document</param>
        /// <returns>The position inside the scintilla document, or -1 if not suitable position was found</returns>
        public static int GetBodyStart(int lineFrom, int lineTo, ScintillaControl sci)
        {
            return GetBodyStart(lineFrom, lineTo, sci, true);
        }

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

        public static int GetStartOfStatement(ScintillaControl sci, int statementEnd, ASResult expr)
        {
            var line = sci.LineFromPosition(statementEnd);
            var text = sci.GetLine(line);
            var match = Regex.Match(text, @"[;\s\n\r]*", RegexOptions.RightToLeft);
            if (match.Success) statementEnd = sci.PositionFromLine(line) + match.Index;
            var result = 0;
            var characters = ScintillaControl.Configuration.GetLanguage(sci.ConfigurationLanguage).characterclass.Characters;
            var arrCount = 0;
            var parCount = 0;
            var genCount = 0;
            var braCount = 0;
            var dQuotes = 0;
            var sQuotes = 0;
            var hasDot = false;
            var c = ' ';
            for (var i = statementEnd; i > 0; i--)
            {
                if (sci.PositionIsOnComment(i - 1)) continue;
                var pc = c;
                c = (char)sci.CharAt(i - 1);
                if (c == ']') arrCount++;
                else if (c == '[' && arrCount > 0) arrCount--;
                else if (c == ')') parCount++;
                else if (c == '(' && parCount > 0) parCount--;
                else if (c == '>')
                {
                    if (i > 1 && (char)sci.CharAt(i - 2) != '-') genCount++;
                }
                else if (c == '<' && genCount > 0) genCount--;
                else if (c == '}') braCount++;
                else if (c == '{' && braCount > 0) braCount--;
                else if (c == '\"' && sQuotes == 0)
                {
                    if (i <= 1 || (char) sci.CharAt(i - 2) == '\\') continue;
                    if (dQuotes == 0) dQuotes++;
                    else dQuotes--;
                    if (arrCount == 0 && parCount == 0) hasDot = false;
                }
                else if (c == '\'' && dQuotes == 0)
                {
                    if (i <= 1 || (char) sci.CharAt(i - 2) == '\\') continue;
                    if (sQuotes == 0) sQuotes++;
                    else sQuotes--;
                    if (arrCount == 0 && parCount == 0) hasDot = false;
                }
                else if (arrCount == 0 && parCount == 0 && genCount == 0 && braCount == 0 && dQuotes == 0 && sQuotes == 0 && !characters.Contains(c) && c != '.')
                {
                    if (hasDot && c <= ' ')
                    {
                        while (i > 0)
                        {
                            var nextPos = i - 1;
                            c = (char) sci.CharAt(nextPos);
                            if (c > ' ' && !sci.PositionIsOnComment(nextPos)) break;
                            i = nextPos;
                        }
                        i++;
                    }
                    else
                    {
                        result = i;
                        break;
                    }
                }
                else if (!hasDot && c == '.') hasDot = pc != '<' && parCount == 0;
                else if (hasDot && characters.Contains(c))
                {
                    var exprType = ASComplete.GetExpressionType(sci, i);
                    if (!exprType.IsNull()) expr = exprType;
                    hasDot = false;
                }
            }
            if (expr.Type != null && (expr.Type.Flags & FlagType.Class) > 0 && expr.Context?.WordBefore == "new")
                result = sci.WordStartPosition(result - 1, false);
            else if (IsHaxe && expr.Context?.WordBefore == "cast" && (char)sci.CharAt(result) == '(')
                result = sci.WordStartPosition(result - 1, true);
            return result;
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
            int groupCount = 0;
            int brCount = 0;
            int statementEnd = startPos;
            while (statementEnd < endPos)
            {
                char c = (char)sci.CharAt(statementEnd++);

                bool endOfStatement = false;
                switch (c)
                {
                    case '\r':
                    case '\n':
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
                        break;
                    case '}':
                        brCount--;
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
                if (Array.IndexOf(characterClass, c) == -1)
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

        private static void GenerateToString(ScintillaControl sci, MemberModel member, ClassModel inClass)
        {
            MemberModel resultMember = new MemberModel("toString", ASContext.Context.Features.stringKey, FlagType.Function, Visibility.Public);

            bool isOverride = false;
            inClass.ResolveExtends();
            if (inClass.Extends != null)
            {
                ClassModel aType = inClass.Extends;
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
            }
            MemberList members = inClass.Members;
            StringBuilder membersString = new StringBuilder();
            StringBuilder oneMembersString;
            int len = 0;
            foreach (MemberModel m in members)
            {
                if (((m.Flags & FlagType.Variable) > 0 || (m.Flags & FlagType.Getter) > 0)
                    && (m.Access & Visibility.Public) > 0
                    && (m.Flags & FlagType.Constant) == 0)
                {
                    oneMembersString = new StringBuilder();
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


            string template = TemplateUtils.GetTemplate("ToString");
            string result = TemplateUtils.ToDeclarationWithModifiersString(resultMember, template);
            result = TemplateUtils.ReplaceTemplateVariable(result, "Body", "\"[" + inClass.Name + membersString + "]\"");

            InsertCode(sci.CurrentPos, result, sci);
        }

        private static void GenerateVariableJob(GeneratorJobType job, ScintillaControl sci, MemberModel member, bool detach, ClassModel inClass)
        {
            var wordStartPos = sci.WordStartPosition(sci.CurrentPos, true);
            var position = 0;
            Visibility visibility = job.Equals(GeneratorJobType.Variable) ? GetDefaultVisibility(inClass) : Visibility.Public;
            // evaluate, if the variable (or constant) should be generated in other class
            ASResult varResult = ASComplete.GetExpressionType(sci, sci.WordEndPosition(sci.CurrentPos, true));
            if (member != null && ASContext.CommonSettings.GenerateScope && !varResult.Context.Value.Contains(ASContext.Context.Features.dot)) AddExplicitScopeReference(sci, inClass, member);
            int contextOwnerPos = GetContextOwnerEndPos(sci, sci.WordStartPosition(sci.CurrentPos, true));
            MemberModel isStatic = new MemberModel();
            if (contextOwnerPos != -1)
            {
                ASResult contextOwnerResult = ASComplete.GetExpressionType(sci, contextOwnerPos);
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
            int lineNum = sci.CurrentLine;
            string line = sci.GetLine(lineNum);
            
            Match m = Regex.Match(line, "\\b" + Regex.Escape(contextToken) + "\\(");
            if (m.Success)
            {
                returnType = new ASResult();
                returnType.Type = ASContext.Context.ResolveType("Function", null);
            }
            else
            {
                m = Regex.Match(line, @"=\s*[^;\n\r}}]+");
                if (m.Success)
                {
                    int posLineStart = sci.PositionFromLine(lineNum);
                    if (posLineStart + m.Index >= sci.CurrentPos)
                    {
                        line = line.Substring(m.Index);
                        StatementReturnType rType = GetStatementReturnType(sci, inClass, line, posLineStart + m.Index);
                        if (rType != null)
                        {
                            returnType = rType.resolve;
                        }
                    }
                }
            }
            bool isOtherClass = false;
            if (varResult.RelClass != null && !varResult.RelClass.IsVoid() && !varResult.RelClass.Equals(inClass))
            {
                AddLookupPosition();
                lookupPosition = -1;

                ASContext.MainForm.OpenEditableDocument(varResult.RelClass.InFile.FileName, false);
                sci = ASContext.CurSciControl;
                isOtherClass = true;

                FileModel fileModel = new FileModel();
                fileModel.Context = ASContext.Context;
                ASFileParser parser = new ASFileParser();
                parser.ParseSrc(fileModel, sci.Text);

                foreach (ClassModel cm in fileModel.Classes)
                {
                    if (cm.QualifiedName.Equals(varResult.RelClass.QualifiedName))
                    {
                        varResult.RelClass = cm;
                        break;
                    }
                }
                inClass = varResult.RelClass;

                ASContext.Context.UpdateContext(inClass.LineFrom);
            }

            var latest = GetLatestMemberForVariable(job, inClass, visibility, isStatic);
            
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
            if (job == GeneratorJobType.Constant && returnType == null)
            {
                isStatic.Flags |= FlagType.Static;
            }
            else if (returnType != null)
            {
                ClassModel inClassForImport;
                if (returnType.InClass != null) inClassForImport = returnType.InClass;
                else if (returnType.RelClass != null) inClassForImport = returnType.RelClass;
                else inClassForImport = inClass;
                List<string> imports = new List<string>(1);
                if (returnType.Member != null)
                {
                    if (returnType.Member.Type != ASContext.Context.Features.voidKey)
                    {
                        returnTypeStr = returnType.Member.Type;
                        imports.Add(returnType.Member.Type);
                    }
                }
                else if (returnType.Type != null)
                {
                    returnTypeStr = returnType.Type.QualifiedName;
                    imports.Add(returnType.Type.QualifiedName);
                }
                if (ASContext.Context.Settings.GenerateImports && imports.Count > 0)
                {
                    var types = GetQualifiedTypes(imports, inClassForImport.InFile);
                    position += AddImportsByName(types, sci.LineFromPosition(position));
                    sci.SetSel(position, position);
                }
            }
            FlagType kind = job.Equals(GeneratorJobType.Constant) ? FlagType.Constant : FlagType.Variable;
            MemberModel newMember = NewMember(contextToken, isStatic, kind, visibility);
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
            int pos = wordStartPos - 1;
            bool dotFound = false;
            while (pos > 0)
            {
                char c = (char) sci.CharAt(pos);
                if (c == '.' && !dotFound) dotFound = true;
                else if (c == '\t' || c == '\n' || c == '\r' || c == ' ') { /* skip */ }
                else return dotFound ? pos + 1 : -1;
                pos--;
            }
            return pos;
        }

        public static string Capitalize(string name)
        {
            return !string.IsNullOrEmpty(name) ? Char.ToUpper(name[0]) + name.Substring(1) : name;
        }

        public static string Camelize(string name)
        {
            name = name.Trim(new char[] { '\'', '"' });
            string[] parts = name.ToLower().Split('_');
            string result = "";
            foreach (string part in parts)
            {
                if (result.Length > 0)
                    result += Capitalize(part);
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
            bool isDoubleQuote = false;
            bool isSingleQuote = false;
            bool wasEscapeChar = false;
            bool doBreak = false;
            bool writeParam = false;
            int subClosuresCount = 0;
            var arrCount = 0;
            IASContext ctx = ASContext.Context;
            char[] charsToTrim = new char[] { ' ', '\t', '\r', '\n' };
            int counter = sci.TextLength; // max number of chars in parameters line (to avoid infinitive loop)
            string characterClass = ScintillaControl.Configuration.GetLanguage(sci.ConfigurationLanguage).characterclass.Characters;
            int lastMemberPos = p;

            // add [] and <>
            while (p < counter && !doBreak)
            {
                var c = (char)sci.CharAt(p++);
                ASResult result = null;
                if (c == '(' && !isFuncStarted)
                {
                    if (sb.ToString().Trim(charsToTrim).Length == 0)
                    {
                        isFuncStarted = true;
                    }
                    else
                    {
                        break;
                    }
                }
                else if (c == ';' && !isFuncStarted) break;
                else if (c == ')' && isFuncStarted && !wasEscapeChar && !isDoubleQuote && !isSingleQuote && subClosuresCount == 0)
                {
                    isFuncStarted = false;
                    writeParam = true;
                    doBreak = true;
                }
                else if ((c == '(' || c == '[' || c == '<' || c == '{') && !wasEscapeChar && !isDoubleQuote && !isSingleQuote)
                {
                    if (c == '[') arrCount++;
                    if (subClosuresCount == 0)
                    {
                        if (c == '{')
                        {
                            if (sb.ToString().TrimStart().Length > 0)
                            {
                                result = new ASResult();
                                result.Type = ctx.ResolveType("Function", null);
                                types.Insert(0, result);
                            }
                            else
                            {
                                result = ASComplete.GetExpressionType(sci, p);
                                types.Insert(0, result);
                            }
                        }
                        else if (c == '(')
                        {
                            if (!sb.ToString().Contains("<") && !isFuncStarted)
                            {
                                result = ASComplete.GetExpressionType(sci, lastMemberPos + 1);
                                if (!result.IsNull())
                                {
                                    types.Insert(0, result);
                                }
                            }
                        }
                        else if (c == '<')
                        {
                            if (sb.ToString().TrimStart().Length == 0)
                            {
                                result = new ASResult();
                                result.Type = ctx.ResolveType("XML", null);
                                types.Insert(0, result);
                            }
                        }
                    }
                    subClosuresCount++;
                    sb.Append(c);
                    wasEscapeChar = false;
                }
                else if ((c == ')' || c == ']' || c == '>' || c == '}') && !wasEscapeChar && !isDoubleQuote && !isSingleQuote)
                {
                    subClosuresCount--;
                    sb.Append(c);
                    wasEscapeChar = false;
                    if (c == ']')
                    {
                        if (arrCount > 0) arrCount--;
                        if (arrCount == 0)
                        {
                            var cNext = sci.CharAt(p);
                            if (cNext != '[' && cNext != '.')
                            {
                                if (!sb.ToString().Contains("<"))
                                {
                                    result = ASComplete.GetExpressionType(sci, p);
                                    if (result.Type != null) result.Member = null;
                                    else result.Type = ctx.ResolveType(ctx.Features.arrayKey, null);
                                    types.Insert(0, result);
                                }
                                writeParam = true;
                            }
                        }
                    }
                    else if (c == ')' && subClosuresCount == 0 && sb.ToString().StartsWithOrdinal("new"))
                    {
                        lastMemberPos = p - 1;
                        writeParam = true;
                    }
                }
                else if (c == '\\')
                {
                    wasEscapeChar = !wasEscapeChar;
                    sb.Append(c);
                }
                else if (c == '"' && !wasEscapeChar && !isSingleQuote)
                {
                    isDoubleQuote = !isDoubleQuote;
                    if (subClosuresCount == 0 && !isDoubleQuote && (char) sci.CharAt(p) != '.')
                    {
                        result = ASComplete.GetExpressionType(sci, p);
                        types.Add(result);
                    }
                    sb.Append(c);
                    wasEscapeChar = false;
                }
                else if (c == '\'' && !wasEscapeChar && !isDoubleQuote)
                {
                    isSingleQuote = !isSingleQuote;
                    if (subClosuresCount == 0 && !isSingleQuote)
                    {
                        result = ASComplete.GetExpressionType(sci, p);
                        types.Add(result);
                    }
                    sb.Append(c);
                    wasEscapeChar = false;
                }
                else if (c == ',' && subClosuresCount == 0 && !isDoubleQuote && !isSingleQuote)
                {
                    if (isFuncStarted)
                    {
                        result = ASComplete.GetExpressionType(sci, p - 1, true, true);
                        result.Context.coma = ComaExpression.FunctionParameter;
                        types.Add(result);
                        writeParam = true;
                    }
                    else if (!isSingleQuote && !isDoubleQuote)
                    {
                        writeParam = true;
                    }
                    else
                    {
                        sb.Append(c);
                    }
                    wasEscapeChar = false;
                }
                else if (isFuncStarted)
                {
                    sb.Append(c);
                    if (!isSingleQuote && !isDoubleQuote && subClosuresCount == 0 && characterClass.IndexOf(c) > -1)
                    {
                        lastMemberPos = p - 1;
                    }
                    wasEscapeChar = false;
                }
                else if (characterClass.IndexOf(c) > -1)
                {
                    if (!isDoubleQuote && !isSingleQuote) doBreak = true;
                    else sb.Append(c);
                }

                if (writeParam)
                {
                    writeParam = false;
                    string trimmed = sb.ToString().Trim(charsToTrim);
                    if (trimmed.Length > 0)
                    {
                        if (trimmed.Contains("<"))
                        {
                            var expr = trimmed.StartsWithOrdinal("new")
                                ? ASComplete.GetExpressionType(sci, lastMemberPos + 1, true, true).Context
                                : null;
                            trimmed = Regex.Replace(trimmed, @"^new\s", string.Empty);
                            trimmed = Regex.Replace(trimmed, @">\(.*", ">");
                            var type = ctx.ResolveType(trimmed, ctx.CurrentModel);
                            result = new ASResult {Type = type, Context = expr};
                        }
                        else if (trimmed.StartsWithOrdinal("new")) result = ASComplete.GetExpressionType(sci, lastMemberPos + 1, true, true);
                        else result = ASComplete.GetExpressionType(sci, lastMemberPos + 1);
                        if (result != null && !result.IsNull())
                        {
                            if (characterClass.IndexOf(trimmed[trimmed.Length - 1]) > -1)
                            {
                                types.Insert(0, result);
                            }
                            else
                            {
                                types.Add(result);
                            }
                        }
                        else
                        {
                            double d = double.MaxValue;
                            try
                            {
                                d = double.Parse(trimmed, CultureInfo.InvariantCulture);
                            }
                            catch (Exception)
                            {
                            }
                            if (d != double.MaxValue && d.ToString().Length == trimmed.Length)
                            {
                                result = new ASResult();
                                result.Type = ctx.ResolveType(ctx.Features.numberKey, null);
                                types.Insert(0, result);
                            }
                            else if (trimmed.Equals("true") || trimmed.Equals("false"))
                            {
                                result = new ASResult();
                                result.Type = ctx.ResolveType(ctx.Features.booleanKey, null);
                                types.Insert(0, result);
                            }
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
                                paramType = FormatType(GetShortType(result.Member.Type));
                                if (result.InClass == null)
                                {
                                    paramQualType = result.Type.QualifiedName;
                                }
                                else
                                {
                                    paramQualType = GetQualifiedType(result.Member.Type, result.InClass);
                                }
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
                else prms[i].paramName = GuessVarName(prms[i].paramName, FormatType(GetShortType(prms[i].paramType)));
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

        private static void GenerateFunctionJob(GeneratorJobType job, ScintillaControl sci, MemberModel member, bool detach, ClassModel inClass)
        {
            Visibility visibility = job.Equals(GeneratorJobType.FunctionPublic) ? Visibility.Public : GetDefaultVisibility(inClass);
            var wordStartPos = sci.WordStartPosition(sci.CurrentPos, true);
            int wordPos = sci.WordEndPosition(sci.CurrentPos, true);
            List<FunctionParameter> functionParameters = ParseFunctionParameters(sci, wordPos);
            // evaluate, if the function should be generated in other class
            ASResult funcResult = ASComplete.GetExpressionType(sci, sci.WordEndPosition(sci.CurrentPos, true));
            if (member != null && ASContext.CommonSettings.GenerateScope && !funcResult.Context.Value.Contains(ASContext.Context.Features.dot)) AddExplicitScopeReference(sci, inClass, member);
            int contextOwnerPos = GetContextOwnerEndPos(sci, sci.WordStartPosition(sci.CurrentPos, true));
            MemberModel isStatic = new MemberModel();
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

                var fileModel = new FileModel {Context = ASContext.Context};
                var parser = new ASFileParser();
                parser.ParseSrc(fileModel, sci.Text);

                foreach (ClassModel cm in fileModel.Classes)
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
            if ((isStatic.Flags & FlagType.Static) > 0)
            {
                blockTmpl = TemplateUtils.GetBoundary("StaticMethods");
            }
            else if ((visibility & Visibility.Public) > 0)
            {
                blockTmpl = TemplateUtils.GetBoundary("PublicMethods");
            }
            else
            {
                blockTmpl = TemplateUtils.GetBoundary("PrivateMethods");
            }
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
                Func<string, string> cleanType = null;
                cleanType = s => s.StartsWith("(") && s.EndsWith(')') ? cleanType(s.Trim('(', ')')) : s;
                var parameterType = caller.Parameters[parameterIndex].Type;
                if ((char) sci.CharAt(wordPos) == '(') newMemberType = parameterType;
                else
                {
                    var isNativeFunctionType = false;
                    if (parameterType == "Function")
                    {
                        if (IsHaxe)
                        {
                            var paramType = ASContext.Context.ResolveType(parameterType, callerExpr.InFile);
                            if (paramType.InFile.Package == "haxe" && paramType.InFile.Module == "Constraints")
                                isNativeFunctionType = true;
                        }
                        else isNativeFunctionType = true;
                    }
                    var voidKey = ASContext.Context.Features.voidKey;
                    if (isNativeFunctionType) newMemberType = voidKey;
                    else
                    {
                        var parCount = 0;
                        var braCount = 0;
                        var genCount = 0;
                        var startPosition = 0;
                        var typeLength = parameterType.Length;
                        for (var i = 0; i < typeLength; i++)
                        {
                            string type = null;
                            var c = parameterType[i];
                            if (c == '(') parCount++;
                            else if (c == ')')
                            {
                                parCount--;
                                if (parCount == 0 && braCount == 0 && genCount == 0)
                                {
                                    type = parameterType.Substring(startPosition, (i + 1) - startPosition);
                                    startPosition = i + 1;
                                }
                            }
                            else if (c == '{') braCount++;
                            else if (c == '}')
                            {
                                braCount--;
                                if (parCount == 0 && braCount == 0 && genCount == 0)
                                {
                                    type = parameterType.Substring(startPosition, (i + 1) - startPosition);
                                    startPosition = i + 1;
                                }
                            }
                            else if (c == '<') genCount++;
                            else if (c == '>' && parameterType[i - 1] != '-')
                            {
                                genCount--;
                                if (parCount == 0 && braCount == 0 && genCount == 0)
                                {
                                    type = parameterType.Substring(startPosition, (i + 1) - startPosition);
                                    startPosition = i + 1;
                                }
                            }
                            else if (parCount == 0 && braCount == 0 && genCount == 0 && c == '-' &&
                                     parameterType[i + 1] == '>')
                            {
                                if (i > startPosition) type = parameterType.Substring(startPosition, i - startPosition);
                                startPosition = i + 2;
                                i++;
                            }
                            if (type == null)
                            {
                                if (i == typeLength - 1 && i > startPosition)
                                    newMemberType = parameterType.Substring(startPosition);
                                continue;
                            }
                            type = cleanType(type);
                            var parameter = $"parameter{functionParameters.Count}";
                            if (type.StartsWith('?'))
                            {
                                parameter = $"?{parameter}";
                                type = type.TrimStart('?');
                            }
                            if (i == typeLength - 1) newMemberType = type;
                            else functionParameters.Add(new FunctionParameter(parameter, type, type, callerExpr));
                        }
                        if (functionParameters.Count == 1 && functionParameters[0].paramType == voidKey)
                            functionParameters.Clear();
                    }
                }
                newMemberType = cleanType(newMemberType);
            }
            // add imports to function argument types
            if (ASContext.Context.Settings.GenerateImports && functionParameters.Count > 0)
            {
                var types = GetQualifiedTypes(functionParameters.Select(it => it.paramQualType), inClass.InFile);
                position += AddImportsByName(types, sci.LineFromPosition(position));
                if (latest == null) sci.SetSel(position, sci.WordEndPosition(position, true));
                else sci.SetSel(position, position);
            }
            var newMember = NewMember(contextToken, isStatic, FlagType.Function, visibility);
            newMember.Parameters = functionParameters.Select(parameter => new MemberModel(parameter.paramName, parameter.paramQualType, FlagType.ParameterVar, 0)).ToList();
            if (newMemberType != null) newMember.Type = newMemberType;
            GenerateFunction(newMember, position, detach, inClass);
        }

        static void GenerateFunction(MemberModel member, int position, bool detach, ClassModel inClass)
        {
            string template;
            string decl;
            if ((inClass.Flags & FlagType.Interface) > 0)
            {
                template = TemplateUtils.GetTemplate("IFunction");
                decl = TemplateUtils.ToDeclarationString(member, template);
            }
            else if ((member.Flags & FlagType.Constructor) > 0)
            {
                template = TemplateUtils.GetTemplate("Constructor");
                decl = TemplateUtils.ToDeclarationWithModifiersString(member, template);
            }
            else
            {
                string body = null;
                switch (ASContext.CommonSettings.GeneratedMemberDefaultBodyStyle)
                {
                    case GeneratedMemberBodyStyle.ReturnDefaultValue:
                        var type = member.Type;
                        if (inClass.InFile.haXe)
                        {
                            var expr = inClass.InFile.Context.ResolveType(type, inClass.InFile);
                            if ((expr.Flags & FlagType.Abstract) != 0 && !string.IsNullOrEmpty(expr.ExtendsType))
                                type = expr.ExtendsType;
                        }
                        var defaultValue = inClass.InFile.Context.GetDefaultValue(type);
                        if (!string.IsNullOrEmpty(defaultValue)) body = $"return {defaultValue};";
                        break;
                }
                template = TemplateUtils.GetTemplate("Function");
                decl = TemplateUtils.ToDeclarationWithModifiersString(member, template);
                decl = TemplateUtils.ReplaceTemplateVariable(decl, "Body", body);
            }
            if (detach) decl = NewLine + TemplateUtils.ReplaceTemplateVariable(decl, "BlankLine", NewLine);
            else decl = TemplateUtils.ReplaceTemplateVariable(decl, "BlankLine", null);
            InsertCode(position, decl);
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

        private static void GenerateClass(ClassModel inClass, string className, IList<FunctionParameter> parameters)
        {
            AddLookupPosition(); // remember last cursor position for Shift+F4

            List<MemberModel> constructorArgs = new List<MemberModel>();
            List<String> constructorArgTypes = new List<String>();
            MemberModel paramMember = new MemberModel();
            for (int i = 0; i < parameters.Count; i++)
            {
                FunctionParameter p = parameters[i];
                constructorArgs.Add(new MemberModel(AvoidKeyword(p.paramName), p.paramType, FlagType.ParameterVar, 0));
                constructorArgTypes.Add(CleanType(GetQualifiedType(p.paramQualType, inClass)));
            }
            
            paramMember.Parameters = constructorArgs;

            IProject project = PluginBase.CurrentProject;
            if (String.IsNullOrEmpty(className)) className = "Class";
            string projFilesDir = Path.Combine(PathHelper.TemplateDir, "ProjectFiles");
            string projTemplateDir = Path.Combine(projFilesDir, project.GetType().Name);
            string paramsString = TemplateUtils.ParametersString(paramMember, true);
            Hashtable info = new Hashtable();
            info["className"] = className;
            if (project.Language.StartsWithOrdinal("as")) info["templatePath"] = Path.Combine(projTemplateDir, "Class.as.fdt");
            else if (project.Language.StartsWithOrdinal("haxe")) info["templatePath"] = Path.Combine(projTemplateDir, "Class.hx.fdt");
            else if (project.Language.StartsWithOrdinal("loom")) info["templatePath"] = Path.Combine(projTemplateDir, "Class.ls.fdt");
            info["inDirectory"] = Path.GetDirectoryName(inClass.InFile.FileName);
            info["constructorArgs"] = paramsString.Length > 0 ? paramsString : null;
            info["constructorArgTypes"] = constructorArgTypes;
            DataEvent de = new DataEvent(EventType.Command, "ProjectManager.CreateNewFile", info);
            EventManager.DispatchEvent(null, de);
        }

        public static void GenerateExtractVariable(ScintillaControl sci, string newName)
        {
            string expression = sci.SelText.Trim(new char[] { '=', ' ', '\t', '\n', '\r', ';', '.' });
            expression = expression.TrimEnd(new char[] { '(', '[', '{', '<' });
            expression = expression.TrimStart(new char[] { ')', ']', '}', '>' });

            var cFile = ASContext.Context.CurrentModel;
            ASFileParser parser = new ASFileParser();
            parser.ParseSrc(cFile, sci.Text);

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
                        if (characterClass.IndexOf(prevOrNextChar) > -1)
                        {
                            continue;
                        }
                    }
                    if (lastPos + expression.Length < currentMethodBody.Length)
                    {
                        prevOrNextChar = currentMethodBody[lastPos + expression.Length];
                        if (characterClass.IndexOf(prevOrNextChar) > -1)
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
            
            sci.CurrentPos = insertPosition;
            sci.SetSel(sci.CurrentPos, sci.CurrentPos);
            MemberModel m = new MemberModel(newName, "", FlagType.LocalVar, 0);
            m.Value = expression;

            string snippet = TemplateUtils.GetTemplate("Variable");
            snippet = TemplateUtils.ReplaceTemplateVariable(snippet, "Modifiers", null);
            snippet = TemplateUtils.ToDeclarationString(m, snippet);
            snippet += NewLine + "$(Boundary)";
            SnippetHelper.InsertSnippetText(sci, sci.CurrentPos, snippet);
        }

        public static void GenerateExtractMethod(ScintillaControl Sci, string NewName)
        {
            FileModel cFile;
            IASContext context = ASContext.Context;

            string selection = Sci.SelText;
            if (string.IsNullOrEmpty(selection))
            {
                return;
            }

            if (selection.TrimStart().Length == 0)
            {
                return;
            }

            Sci.SetSel(Sci.SelectionStart + selection.Length - selection.TrimStart().Length,
                Sci.SelectionEnd);
            Sci.CurrentPos = Sci.SelectionEnd;

            int lineStart = Sci.LineFromPosition(Sci.SelectionStart);
            int lineEnd = Sci.LineFromPosition(Sci.SelectionEnd);
            int firstLineIndent = Sci.GetLineIndentation(lineStart);
            int entryPointIndent = Sci.Indent;

            for (int i = lineStart; i <= lineEnd; i++)
            {
                int indent = Sci.GetLineIndentation(i);
                if (i > lineStart)
                {
                    Sci.SetLineIndentation(i, indent - firstLineIndent + entryPointIndent);
                }
            }

            string selText = Sci.SelText;
            string template = TemplateUtils.GetTemplate("CallFunction");
            template = TemplateUtils.ReplaceTemplateVariable(template, "Name", NewName);
            template = TemplateUtils.ReplaceTemplateVariable(template, "Arguments", "");

            InsertCode(Sci.CurrentPos, template + ";", Sci);

            cFile = ASContext.Context.CurrentModel;
            ASFileParser parser = new ASFileParser();
            parser.ParseSrc(cFile, Sci.Text);

            FoundDeclaration found = GetDeclarationAtLine(Sci, lineStart);
            if (found == null || found.member == null)
            {
                return;
            }

            lookupPosition = Sci.CurrentPos;
            AddLookupPosition();

            MemberModel latest = TemplateUtils.GetTemplateBlockMember(Sci, TemplateUtils.GetBoundary("PrivateMethods"));

            if (latest == null)
                latest = GetLatestMemberForFunction(found.inClass, GetDefaultVisibility(found.inClass), found.member);

            if (latest == null)
                latest = found.member;

            int position = Sci.PositionFromLine(latest.LineTo + 1) - ((Sci.EOLMode == 0) ? 2 : 1);
            Sci.SetSel(position, position);

            FlagType flags = FlagType.Function;
            if ((found.member.Flags & FlagType.Static) > 0)
            {
                flags |= FlagType.Static;
            }

            MemberModel m = new MemberModel(NewName, context.Features.voidKey, flags, GetDefaultVisibility(found.inClass));

            template = NewLine + TemplateUtils.GetTemplate("Function");
            template = TemplateUtils.ToDeclarationWithModifiersString(m, template);
            template = TemplateUtils.ReplaceTemplateVariable(template, "Body", selText);
            template = TemplateUtils.ReplaceTemplateVariable(template, "BlankLine", NewLine);
            InsertCode(position, template, Sci);
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
                if (text.IndexOf('{') >= 0)
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
                type = FormatType(contextMember.Type);
                if (type.IndexOf('*') > 0)
                    type = type.Replace("/*", @"/\*\s*").Replace("*/", @"\s*\*/");
                type = @":\s*" + type;
            }
            var name = contextMember.Name;
            Regex reDecl = new Regex(String.Format(@"[\s\(]((var|const)\s+{0}\s*{1})\s*", name, type));
            for (int i = contextMember.LineFrom; i <= contextMember.LineTo + 10; i++)
            {
                string text = sci.GetLine(i);
                Match m = reDecl.Match(text);
                if (m.Success)
                {
                    int index = sci.MBSafeTextLength(text.Substring(0, m.Groups[1].Index));
                    int position = sci.PositionFromLine(i) + index;
                    int len = sci.MBSafeTextLength(m.Groups[1].Value);
                    sci.SetSel(position, position + len);
                    if (ASContext.CommonSettings.GenerateScope) name = "this." + name;
                    if (contextMember.Type == null || (contextMember.Flags & FlagType.Inferred) != 0) name += " ";
                    sci.ReplaceSel(name);
                    UpdateLookupPosition(position, name.Length - len);
                    return true;
                }
            }
            return false;
        }

        internal static StatementReturnType GetStatementReturnType(ScintillaControl sci, ClassModel inClass, string line, int startPos)
        {
            Regex target = new Regex(@"[;\s\n\r]*", RegexOptions.RightToLeft);
            Match m = target.Match(line);
            if (!m.Success) return null;
            line = line.Substring(0, m.Index);
            if (line.Length == 0) return null;
            var haxe = sci.ConfigurationLanguage == "haxe";
            line = ReplaceAllStringContents(line);
            var bracesRemoved = false;
            var pos = -1;
            char c;
            if (line.Last() == ')')
            {
                var bracesCount = 1;
                var position = startPos + line.Length - 1;
                while (position-- > 0)
                {
                    if (sci.PositionIsOnComment(position)) continue;
                    c = (char)sci.CharAt(position);
                    if (c == ')') bracesCount++;
                    else if (c == '(')
                    {
                        bracesCount--;
                        if (bracesCount > 0) continue;
                        if (haxe && sci.GetWordLeft(position - 1, true) == "cast")
                        {
                            pos = startPos + line.Length;
                            break;
                        }
                        var lineFromPosition = sci.LineFromPosition(position);
                        startPos = sci.PositionFromLine(lineFromPosition);
                        var tmpLine = sci.GetLine(lineFromPosition);
                        tmpLine = tmpLine.Substring(0, position - startPos);
                        if (string.IsNullOrEmpty(tmpLine.TrimStart()))
                        {
                            pos = startPos + line.Length;
                            break;
                        }
                        line = tmpLine;
                        pos = position;
                        bracesRemoved = true;
                        break;
                    }
                }
            }
            else pos = startPos + line.Length - 1;
            var ctx = inClass.InFile.Context;
            var features = ctx.Features;
            ASResult resolve = null;
            string word = null;
            ClassModel type = null;
            if (pos != -1)
            {
                pos = sci.WordEndPosition(pos, true);
                c = line.TrimEnd().Last();
                resolve = ASComplete.GetExpressionType(sci, "]}\"'".Contains(c) || (c == '>' && !bracesRemoved) ? pos + 1 : pos, true, true);
                if (resolve.Type != null && !resolve.IsPackage)
                {
                    if (resolve.Type.Name == "Function" && !bracesRemoved)
                    {
                        if (haxe)
                        {
                            var voidKey = features.voidKey;
                            var parameters = resolve.Member.Parameters?.Select(it => it.Type).ToList() ?? new List<string> {voidKey};
                            parameters.Add(resolve.Member.Type ?? voidKey);
                            var qualifiedName = string.Empty;
                            for (var i = 0; i < parameters.Count; i++)
                            {
                                if (i > 0) qualifiedName += "->";
                                var t = parameters[i];
                                if (t.Contains("->") && !t.StartsWith('(')) t = $"({t})";
                                qualifiedName += t;
                            }
                            resolve = null;
                            type = new ClassModel {Name = qualifiedName, InFile = FileModel.Ignore};
                        }
                        else resolve.Member = null;
                    }
                    else if (!string.IsNullOrEmpty(resolve.Path) && Regex.IsMatch(resolve.Path, @"(\.\[.{0,}?\])$", RegexOptions.RightToLeft))
                        resolve.Member = null;
                }
                word = sci.GetWordFromPosition(pos);
            }
            if (resolve?.Type == null || resolve.Type.IsVoid())
            {
                c = (char)sci.CharAt(pos);
                if (c == ']')
                {
                    resolve = ASComplete.GetExpressionType(sci, pos + 1);
                    type = resolve.Type ?? ctx.ResolveType(features.arrayKey, inClass.InFile);
                    resolve = null;
                }
                if (type != null && type.IsVoid()) type = null;
            }
            if (resolve == null) resolve = new ASResult();
            if (resolve.Type == null) resolve.Type = type;
            return new StatementReturnType(resolve, pos, word);
        }

        private static string ReplaceAllStringContents(string line)
        {
            string retLine = line;
            Regex re1 = new Regex("'(?:[^'\\\\]|(?:\\\\\\\\)|(?:\\\\\\\\)*\\\\.{1})*'");
            Regex re2 = new Regex("\"(?:[^\"\\\\]|(?:\\\\\\\\)|(?:\\\\\\\\)*\\\\.{1})*\"");
            Match m1 = re1.Match(line);
            Match m2 = re2.Match(line);
            while (m1.Success || m2.Success)
            {
                Match m = null;
                if (m1.Success && m2.Success) m = m1.Index > m2.Index ? m2 : m1;
                else if (m1.Success) m = m1;
                else m = m2;
                string sub = "";
                string val = m.Value;
                for (int j = 0; j < val.Length - 2; j++) 
                    sub += "A";
                
                line = line.Substring(0, m.Index) + sub + "AA" + line.Substring(m.Index + m.Value.Length);
                retLine = retLine.Substring(0, m.Index + 1) + sub + retLine.Substring(m.Index + m.Value.Length - 1);

                m1 = re1.Match(line);
                m2 = re2.Match(line);
            }
            return retLine;
        }

        private static string GuessVarName(string name, string type)
        {
            if (string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(type))
            {
                Match m = Regex.Match(type, "^([a-z0-9_$]+)", RegexOptions.IgnoreCase);
                if (m.Success)
                    name = m.Groups[1].Value;
                else
                    name = type;
            }

            if (string.IsNullOrEmpty(name)) 
                return name;

            // if constant then convert to camelCase
            if (name.ToUpper() == name)
                name = Camelize(name);

            // if getter, then remove 'get' prefix
            name = name.TrimStart(new char[] { '_' });
            if (name.Length > 3 && name.StartsWithOrdinal("get") && (name[3].ToString() == char.ToUpper(name[3]).ToString()))
            {
                name = char.ToLower(name[3]) + name.Substring(4);
            }

            if (name.Length > 1)
                name = Char.ToLower(name[0]) + name.Substring(1);
            else
                name = Char.ToLower(name[0]) + "";

            if (name == "this" || type == name)
            {
                if (!string.IsNullOrEmpty(type))
                    name = Char.ToLower(type[0]) + type.Substring(1);
                else
                    name = "p_this";
            }
            return name;
        }

        private static void GenerateImplementation(ClassModel iType, ClassModel inClass, ScintillaControl sci, bool detached)
        {
            var typesUsed = new HashSet<string>();

            StringBuilder sb = new StringBuilder();

            string header = TemplateUtils.ReplaceTemplateVariable(TemplateUtils.GetTemplate("ImplementHeader"), "Class", iType.Type);

            header = TemplateUtils.ReplaceTemplateVariable(header, "BlankLine", detached ? BlankLine : null);

            sb.Append(header);
            sb.Append(NewLine);
            bool entry = true;
            ASResult result = new ASResult();
            IASContext context = ASContext.Context;
            ContextFeatures features = context.Features;
            bool canGenerate = false;
            bool isHaxe = IsHaxe;
            FlagType flags = (FlagType.Function | FlagType.Getter | FlagType.Setter);
            if (isHaxe) flags |= FlagType.Variable;

            iType.ResolveExtends(); // resolve inheritance chain
            while (!iType.IsVoid() && iType.QualifiedName != "Object")
            {
                foreach (MemberModel method in iType.Members)
                {
                    if ((method.Flags & flags) == 0
                        || method.Name == iType.Name)
                        continue;

                    // check if method exists
                    ASComplete.FindMember(method.Name, inClass, result, method.Flags, 0);
                    if (!result.IsNull()) continue;

                    string decl;
                    if ((method.Flags & FlagType.Getter) > 0)
                    {
                        if (isHaxe)
                        {
                            decl = TemplateUtils.ToDeclarationWithModifiersString(method, TemplateUtils.GetTemplate("Property"));

                            string templateName = null;
                            string metadata = null;
                            if (method.Parameters[0].Name == "get")
                            {
                                if (method.Parameters[1].Name == "set")
                                {
                                    templateName = "GetterSetter";
                                    metadata = "@:isVar";
                                }
                                else
                                    templateName = "Getter";
                            }
                            else if (method.Parameters[1].Name == "set")
                            {
                                templateName = "Setter";
                            }

                            decl = TemplateUtils.ReplaceTemplateVariable(decl, "MetaData", metadata);

                            if (templateName != null)
                            {
                                var accessor = NewLine + TemplateUtils.ToDeclarationString(method, TemplateUtils.GetTemplate(templateName));
                                accessor = TemplateUtils.ReplaceTemplateVariable(accessor, "Modifiers", null);
                                accessor = TemplateUtils.ReplaceTemplateVariable(accessor, "Member", method.Name);
                                decl += accessor;
                            }
                        }
                        else
                            decl = TemplateUtils.ToDeclarationWithModifiersString(method, TemplateUtils.GetTemplate("Getter"));
                    }
                    else if ((method.Flags & FlagType.Setter) > 0)
                        decl = TemplateUtils.ToDeclarationWithModifiersString(method, TemplateUtils.GetTemplate("Setter"));
                    else if ((method.Flags & FlagType.Function) > 0)
                        decl = TemplateUtils.ToDeclarationWithModifiersString(method, TemplateUtils.GetTemplate("Function"));
                    else
                        decl = NewLine + TemplateUtils.ToDeclarationWithModifiersString(method, TemplateUtils.GetTemplate("Variable"));
                    decl = TemplateUtils.ReplaceTemplateVariable(decl, "Member", "_" + method.Name);
                    decl = TemplateUtils.ReplaceTemplateVariable(decl, "Void", features.voidKey);
                    decl = TemplateUtils.ReplaceTemplateVariable(decl, "Body", null);
                    decl = TemplateUtils.ReplaceTemplateVariable(decl, "BlankLine", NewLine);

                    if (!entry)
                    {
                        decl = TemplateUtils.ReplaceTemplateVariable(decl, "EntryPoint", null);
                    }

                    decl += NewLine;

                    entry = false;

                    sb.Append(decl);
                    canGenerate = true;

                    typesUsed.Add(method.Type);

                    if (method.Parameters != null && method.Parameters.Count > 0)
                        foreach (MemberModel param in method.Parameters)
                            typesUsed.Add(param.Type);
                }
                if (ASContext.Context.Settings.GenerateImports) typesUsed = (HashSet<string>) GetQualifiedTypes(typesUsed, iType.InFile);
                // interface inheritance
                iType = iType.Extends;
            }
            if (!canGenerate)
                return;

            sci.BeginUndoAction();
            try
            {
                int position = sci.CurrentPos;
                if (ASContext.Context.Settings.GenerateImports && typesUsed.Count > 0)
                {
                    int offset = AddImportsByName(typesUsed, sci.LineFromPosition(position));
                    position += offset;
                    sci.SetSel(position, position);
                }
                InsertCode(position, sb.ToString(), sci);
            }
            finally { sci.EndUndoAction(); }
        }

        private static void AddTypeOnce(List<string> typesUsed, string qualifiedName)
        {
            if (!typesUsed.Contains(qualifiedName)) typesUsed.Add(qualifiedName);
        }

        static IEnumerable<string> GetQualifiedTypes(IEnumerable<string> types, FileModel inFile)
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

        private static MemberModel NewMember(string contextToken, MemberModel calledFrom, FlagType kind, Visibility visi)
        {
            string type = (kind == FlagType.Function && !ASContext.Context.Features.hasInference) 
                ? ASContext.Context.Features.voidKey : null;
            if (calledFrom != null && (calledFrom.Flags & FlagType.Static) > 0)
                kind |= FlagType.Static;
            return new MemberModel(contextToken, type, kind, visi);
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
            ContextFeatures features = ASContext.Context.Features;
            string visibility = GetPrivateKeyword(inClass);
            if (features.publicKey == null || visibility == null) return false;
            Regex rePublic = new Regex(String.Format(@"\s*({0})\s+", features.publicKey));

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

        public static bool MakeHaxeProperty(ScintillaControl Sci, MemberModel member, string args)
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
                    Sci.ReplaceSel(member.Name + args);
                    UpdateLookupPosition(position, 1);
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
        private static string GetPropertyNameFor(MemberModel member)
        {
            string name = member.Name;
            if (name.Length == 0 || (member.Access & Visibility.Public) > 0 || IsHaxe) return null;
            Match parts = Regex.Match(name, "([^_$]*)[_$]+(.*)");
            if (parts.Success)
            {
                string pre = parts.Groups[1].Value;
                string post = parts.Groups[2].Value;
                return pre.Length > post.Length ? pre : post;
            }
            return null;
        }

        /// <summary>
        /// Return a smart new property name
        /// </summary>
        private static string GetNewPropertyNameFor(MemberModel member)
        {
            if (member.Name.Length == 0)
                return "prop";
            if (Regex.IsMatch(member.Name, "^[A-Z].*[a-z]"))
                return Char.ToLower(member.Name[0]) + member.Name.Substring(1);
            else
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
            ScintillaControl sci = ASContext.CurSciControl;
            sci.BeginUndoAction();
            try
            {
                int delta = 0;
                ClassModel eventClass = ASContext.Context.ResolveType(type, ASContext.Context.CurrentModel);
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
                var decl = NewLine + TemplateUtils.ToDeclarationWithModifiersString(newMember, template);
                decl = TemplateUtils.ReplaceTemplateVariable(decl, "Void", ASContext.Context.Features.voidKey);

                string eventName = contextMatch.Groups["event"].Value;
                string autoRemove = AddRemoveEvent(eventName);
                if (autoRemove != null)
                {
                    if (autoRemove.Length == 0 && ASContext.CommonSettings.GenerateScope) autoRemove = "this";
                    if (autoRemove.Length > 0) autoRemove += ".";
                    string remove = string.Format("{0}removeEventListener({1}, {2});\n\t$(EntryPoint)", autoRemove, eventName, name);
                    decl = decl.Replace("$(EntryPoint)", remove);
                }
                InsertCode(position, decl, sci);
            }
            finally
            {
                sci.EndUndoAction();
            }
        }

        private static bool TryImportType(string type, ref int delta, int atLine)
        {
            ClassModel eventClass = ASContext.Context.ResolveType(type, ASContext.Context.CurrentModel);
            if (eventClass.IsVoid())
                return false;
            
            List<string> typesUsed = new List<string>();
            typesUsed.Add(type);
            delta += AddImportsByName(typesUsed, atLine);
            return true;
        }

        private static string AddRemoveEvent(string eventName)
        {
            foreach (string autoRemove in ASContext.CommonSettings.EventListenersAutoRemove)
            {
                string test = autoRemove.Trim();
                if (test.Length == 0 || test.StartsWithOrdinal("//")) continue;
                int colonPos = test.IndexOf(':');
                if (colonPos >= 0) test = test.Substring(colonPos + 1);
                if (test != eventName) continue;
                return colonPos < 0 ? "" : autoRemove.Trim().Substring(0, colonPos);
            }
            return null;
        }

        private static void GenerateGetter(string name, MemberModel member, int position)
        {
            var newMember = new MemberModel
            {
                Name = name,
                Type = FormatType(member.Type),
                Access = IsHaxe ? Visibility.Private : Visibility.Public
            };
            if ((member.Flags & FlagType.Static) > 0) newMember.Flags = FlagType.Static;
            string template = TemplateUtils.GetTemplate("Getter");
            string decl = NewLine + TemplateUtils.ToDeclarationWithModifiersString(newMember, template);
            decl = TemplateUtils.ReplaceTemplateVariable(decl, "Member", member.Name);
            decl = TemplateUtils.ReplaceTemplateVariable(decl, "BlankLine", NewLine);
            InsertCode(position, decl);
        }

        private static void GenerateSetter(string name, MemberModel member, int position)
        {
            var newMember = new MemberModel
            {
                Name = name,
                Type = FormatType(member.Type),
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

        private static void GenerateGetterSetter(string name, MemberModel member, int position)
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
                Type = FormatType(member.Type),
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

        private static MemberModel GetLatestMemberForFunction(ClassModel inClass, Visibility funcVisi, MemberModel isStatic)
        {
            MemberModel latest = null;
            if (isStatic != null && (isStatic.Flags & FlagType.Static) > 0)
            {
                latest = FindLatest(FlagType.Function | FlagType.Static, funcVisi, inClass);
                if (latest == null)
                {
                    latest = FindLatest(FlagType.Function | FlagType.Static, 0, inClass, true, false);
                }
            }
            else
            {
                latest = FindLatest(FlagType.Function, funcVisi, inClass);
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

        private static MemberModel GetLatestMemberForVariable(GeneratorJobType job, ClassModel inClass, Visibility varVisi, MemberModel isStatic)
        {
            MemberModel latest = null;
            if (job.Equals(GeneratorJobType.Constant))
            {
                if ((isStatic.Flags & FlagType.Static) > 0)
                {
                    latest = FindLatest(FlagType.Constant | FlagType.Static, varVisi, inClass);
                }
                else
                {
                    latest = FindLatest(FlagType.Constant, varVisi, inClass);
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
                    latest = FindLatest(FlagType.Variable | FlagType.Static, varVisi, inClass);
                    if (latest == null)
                    {
                        latest = FindLatest(FlagType.Variable | FlagType.Static, 0, inClass, true, false);
                    }
                }
                else
                {
                    latest = FindLatest(FlagType.Variable, varVisi, inClass);
                }
            }
            if (latest == null)
            {
                latest = FindLatest(FlagType.Variable, varVisi, inClass, false, false);
            }
            return latest;
        }

        private static MemberModel FindMember(string name, ClassModel inClass)
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

        private static MemberModel FindLatest(FlagType match, ClassModel inClass)
        {
            return FindLatest(match, 0, inClass);
        }

        private static MemberModel FindLatest(FlagType match, Visibility visi, ClassModel inClass)
        {
            return FindLatest(match, visi, inClass, true, true);
        }

        private static MemberModel FindLatest(FlagType match, Visibility visi, ClassModel inClass, bool isFlagMatchStrict, bool isVisibilityMatchStrict)
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
        static bool HandleOverrideCompletion(bool autoHide)
        {
            // explore members
            IASContext ctx = ASContext.Context;
            ClassModel curClass = ctx.CurrentClass;
            if (curClass.IsVoid()) return false;

            List<MemberModel> members = new List<MemberModel>();
            curClass.ResolveExtends(); // Resolve inheritance chain

            // explore getters or setters
            FlagType mask = FlagType.Function | FlagType.Getter | FlagType.Setter;
            ClassModel tmpClass = curClass.Extends;
            Visibility acc = ctx.TypesAffinity(curClass, tmpClass);
            while (tmpClass != null && !tmpClass.IsVoid())
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
                else
                {
                    foreach (MemberModel member in tmpClass.Members)
                    {
                        if (curClass.Members.Search(member.Name, FlagType.Override, 0) != null) continue;
                        var parameters = member.Parameters;
                        if ((member.Flags & FlagType.Dynamic) > 0
                            && (member.Access & acc) > 0
                            && ((member.Flags & FlagType.Function) > 0 
                                || ((member.Flags & mask) > 0 && (!IsHaxe || parameters[0].Name == "get" || parameters[1].Name == "set"))))
                        {
                            members.Add(member);
                        }
                    }

                    tmpClass = tmpClass.Extends;
                    // members visibility
                    acc = ctx.TypesAffinity(curClass, tmpClass);
                }
            }
            members.Sort();

            // build list
            List<ICompletionListItem> known = new List<ICompletionListItem>();

            MemberModel last = null;
            foreach (MemberModel member in members)
            {
                if (last == null || last.Name != member.Name)
                    known.Add(new MemberItem(member));
                last = member;
            }
            if (known.Count > 0) CompletionList.Show(known, autoHide);
            return true;
        }

        public static void GenerateOverride(ScintillaControl Sci, ClassModel ofClass, MemberModel member, int position)
        {
            var context = ASContext.Context;
            var features = context.Features;
            List<string> typesUsed = new List<string>();
            bool isProxy = (member.Namespace == "flash_proxy");
            if (isProxy) typesUsed.Add("flash.utils.flash_proxy");
            
            int line = Sci.LineFromPosition(position);
            string currentText = Sci.GetLine(line);
            int startPos = currentText.Length;
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

            bool isAS2Event = context.Settings.LanguageId == "AS2" && member.Name.StartsWithOrdinal("on");
            if (!isAS2Event && ofClass.QualifiedName != "Object") newMember.Flags |= FlagType.Override;

            string decl = "";

            FlagType flags = member.Flags;
            if ((flags & FlagType.Static) > 0) newMember.Flags |= FlagType.Static;
            var parameters = member.Parameters;
            if ((flags & (FlagType.Getter | FlagType.Setter)) > 0)
            {
                if (IsHaxe) newMember.Access = Visibility.Private;
                var type = newMember.Type;
                var name = newMember.Name;
                if (parameters != null && parameters.Count == 1) type = parameters[0].Type;
                type = FormatType(type);
                if (type == null && !features.hasInference) type = features.objectKey;
                newMember.Type = type;
                var currentClass = context.CurrentClass;
                if (ofClass.Members.Search(name, FlagType.Getter, 0) != null
                    && (!IsHaxe || (parameters?[0].Name == "get" && currentClass.Members.Search($"get_{name}", FlagType.Function, 0) == null)))
                {
                    var template = TemplateUtils.GetTemplate("OverrideGetter", "Getter");
                    template = TemplateUtils.ToDeclarationWithModifiersString(newMember, template);
                    template = TemplateUtils.ReplaceTemplateVariable(template, "Member", $"super.{name}");
                    decl += template;
                }
                if (ofClass.Members.Search(name, FlagType.Setter, 0) != null
                    && (!IsHaxe || (parameters?[1].Name == "set" && currentClass.Members.Search($"set_{name}", FlagType.Function, 0) == null)))
                {
                    var template = TemplateUtils.GetTemplate("OverrideSetter", "Setter");
                    template = TemplateUtils.ToDeclarationWithModifiersString(newMember, template);
                    template = TemplateUtils.ReplaceTemplateVariable(template, "Member", $"super.{name}");
                    template = TemplateUtils.ReplaceTemplateVariable(template, "Void", features.voidKey ?? "void");
                    if (decl.Length > 0) template = "\n\n" + template.Replace("$(EntryPoint)", "");
                    decl += template;
                }
                decl = TemplateUtils.ReplaceTemplateVariable(decl, "BlankLine", "");
                typesUsed.Add(type);
            }
            else
            {
                var type = FormatType(newMember.Type);
                var noRet = type == null || type.Equals("void", StringComparison.OrdinalIgnoreCase);
                type = (noRet && type != null) ? features.voidKey : type;
                if (!noRet) typesUsed.Add(type);
                newMember.Template = member.Template;
                newMember.Type = type;
                // fix parameters if needed
                if (parameters != null)
                    foreach (MemberModel para in parameters)
                        if (para.Type == "any") para.Type = "*";

                newMember.Parameters = parameters;
                var action = (isProxy || isAS2Event) ? "" : GetSuperCall(member, typesUsed);
                var template = TemplateUtils.GetTemplate("MethodOverride");
                template = TemplateUtils.ToDeclarationWithModifiersString(newMember, template);
                template = TemplateUtils.ReplaceTemplateVariable(template, "Method", action);
                decl = template;
            }

            Sci.BeginUndoAction();
            try
            {
                if (context.Settings.GenerateImports && typesUsed.Count > 0)
                {
                    var types = GetQualifiedTypes(typesUsed, ofClass.InFile);
                    int offset = AddImportsByName(types, line);
                    position += offset;
                    startPos += offset;
                }

                Sci.SetSel(startPos, position + member.Name.Length);
                InsertCode(startPos, decl, Sci);
            }
            finally { Sci.EndUndoAction(); }
        }

        public static void GenerateDelegateMethods(ScintillaControl sci, MemberModel member,
            Dictionary<MemberModel, ClassModel> selectedMembers, ClassModel classModel, ClassModel inClass)
        {
            sci.BeginUndoAction();
            try
            {
                string result = TemplateUtils.ReplaceTemplateVariable(
                    TemplateUtils.GetTemplate("DelegateMethodsHeader"), 
                    "Class", 
                    classModel.Type);

                int position = -1;
                List<string> importsList = new List<string>();
                bool isStaticMember = (member.Flags & FlagType.Static) > 0;

                inClass.ResolveExtends();
                
                Dictionary<MemberModel, ClassModel>.KeyCollection selectedMemberKeys = selectedMembers.Keys;
                foreach (MemberModel m in selectedMemberKeys)
                {
                    MemberModel mCopy = (MemberModel) m.Clone();

                    string methodTemplate = NewLine;

                    bool overrideFound = false;
                    ClassModel baseClassType = inClass;
                    while (baseClassType != null && !baseClassType.IsVoid())
                    {
                        MemberList inClassMembers = baseClassType.Members;
                        foreach (MemberModel inClassMember in inClassMembers)
                        {
                            if ((inClassMember.Flags & FlagType.Function) > 0
                               && m.Name.Equals(inClassMember.Name))
                            {
                                mCopy.Flags |= FlagType.Override;
                                overrideFound = true;
                                break;
                            }
                        }

                        if (overrideFound)
                            break;

                        baseClassType = baseClassType.Extends;
                    }

                    var flags = m.Flags;
                    if (isStaticMember && (flags & FlagType.Static) == 0) mCopy.Flags |= FlagType.Static;
                    var variableTemplate = string.Empty;
                    if (IsHaxe & (flags & (FlagType.Getter | FlagType.Setter)) != 0)
                    {
                        variableTemplate = NewLine + NewLine + (TemplateUtils.GetStaticExternOverride(m) + TemplateUtils.GetModifiers(m)).Trim() + " var " + m.Name;
                    }
                    if ((flags & FlagType.Getter) > 0)
                    {
                        if (!IsHaxe || (m.Parameters[0].Name != "null" && m.Parameters[0].Name != "never"))
                        {
                            string modifiers;
                            if (IsHaxe)
                            {
                                variableTemplate += "(get, ";
                                modifiers = (TemplateUtils.GetStaticExternOverride(m) + TemplateUtils.GetModifiers(Visibility.Private)).Trim();
                            }
                            else modifiers = (TemplateUtils.GetStaticExternOverride(m) + TemplateUtils.GetModifiers(m)).Trim();
                            methodTemplate += TemplateUtils.GetTemplate("Getter");
                            methodTemplate = TemplateUtils.ReplaceTemplateVariable(methodTemplate, "Modifiers", modifiers);
                            methodTemplate = TemplateUtils.ReplaceTemplateVariable(methodTemplate, "Name", m.Name);
                            methodTemplate = TemplateUtils.ReplaceTemplateVariable(methodTemplate, "EntryPoint", "");
                            methodTemplate = TemplateUtils.ReplaceTemplateVariable(methodTemplate, "Type", FormatType(m.Type));
                            methodTemplate = TemplateUtils.ReplaceTemplateVariable(methodTemplate, "Member", member.Name + "." + m.Name);
                            flags &= ~FlagType.Function;
                        }
                        else variableTemplate += "(" + m.Parameters[0].Name + ", ";
                    }
                    if ((flags & FlagType.Setter) > 0)
                    {
                        if (!IsHaxe || (m.Parameters[1].Name != "null" && m.Parameters[1].Name != "never"))
                        {
                            string modifiers;
                            string type;
                            if (IsHaxe)
                            {
                                variableTemplate += "set)";
                                if (methodTemplate != NewLine) methodTemplate += NewLine;
                                modifiers = (TemplateUtils.GetStaticExternOverride(m) + TemplateUtils.GetModifiers(Visibility.Private)).Trim();
                                type = FormatType(m.Type);
                            }
                            else
                            {
                                modifiers = (TemplateUtils.GetStaticExternOverride(m) + TemplateUtils.GetModifiers(m)).Trim();
                                type = m.Parameters[0].Type;
                            }
                            methodTemplate += TemplateUtils.GetTemplate("Setter");
                            methodTemplate = TemplateUtils.ReplaceTemplateVariable(methodTemplate, "Modifiers", modifiers);
                            methodTemplate = TemplateUtils.ReplaceTemplateVariable(methodTemplate, "Name", m.Name);
                            methodTemplate = TemplateUtils.ReplaceTemplateVariable(methodTemplate, "EntryPoint", "");
                            methodTemplate = TemplateUtils.ReplaceTemplateVariable(methodTemplate, "Type", type);
                            methodTemplate = TemplateUtils.ReplaceTemplateVariable(methodTemplate, "Member", member.Name + "." + m.Name);
                            methodTemplate = TemplateUtils.ReplaceTemplateVariable(methodTemplate, "Void", ASContext.Context.Features.voidKey ?? "void");
                            flags &= ~FlagType.Function;
                        }
                        else variableTemplate += m.Parameters[1].Name + ")";
                    }
                    if (!string.IsNullOrEmpty(variableTemplate))
                    {
                        variableTemplate += ":" + m.Type + ";";
                        result += variableTemplate;
                    }
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
                        bool isVararg = false;
                        if (m.Parameters != null && m.Parameters.Count > 0)
                        {
                            MemberModel mm = m.Parameters[m.Parameters.Count - 1];
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

                    if (ASContext.Context.Settings.GenerateImports && m.Parameters != null)
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

                    if (ASContext.Context.Settings.GenerateImports && m.Type != null) importsList.Add(m.Type);
                }

                if (ASContext.Context.Settings.GenerateImports && importsList.Count > 0 && position > -1)
                {
                    var types = GetQualifiedTypes(importsList, inClass.InFile);
                    position += AddImportsByName(types, sci.LineFromPosition(position));
                    sci.SetSel(position, position);
                }

                InsertCode(position, result, sci);
            }
            finally { sci.EndUndoAction(); }
        }

        private static void GetStartPos(string currentText, ref int startPos, string keyword)
        {
            if (keyword == null) return;
            int p = currentText.IndexOfOrdinal(keyword);
            if (p > 0 && p < startPos) startPos = p;
        }

        private static string GetShortType(string type)
        {
            return string.IsNullOrEmpty(type) ? type : Regex.Replace(type, @"(?=\w+\.<)|(?:\w+\.)", string.Empty);
        }

        private static string FormatType(string type)
        {
            return MemberModel.FormatType(type);
        }

        private static string CleanType(string type)
        {
            if (string.IsNullOrEmpty(type))
            {
                return type;
            }
            int p = type.IndexOf('$');
            if (p > 0) type = type.Substring(0, p);
            p = type.IndexOf('<');
            if (p > 1 && type[p - 1] == '.') p--;
            if (p > 0) type = type.Substring(0, p);
            p = type.IndexOf('@');
            if (p > 0)
            {
                type = type.Substring(0, p);
            }
            return type;
        }

        private static string GetSuperCall(MemberModel member, List<string> typesUsed)
        {
            string args = "";
            if (member.Parameters != null)
                foreach (MemberModel param in member.Parameters)
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
        private static int AddImportsByName(IEnumerable<string> typesUsed, int atLine)
        {
            int length = 0;
            IASContext context = ASContext.Context;
            var addedTypes = new HashSet<string>();
            typesUsed = context.DecomposeTypes(typesUsed);
            foreach (string type in typesUsed)
            {
                var cleanType = CleanType(type);
                if (string.IsNullOrEmpty(cleanType) || addedTypes.Contains(cleanType) || cleanType.IndexOf('.') <= 0)
                    continue;
                addedTypes.Add(cleanType);
                MemberModel import = new MemberModel(cleanType.Substring(cleanType.LastIndexOf('.') + 1), cleanType, FlagType.Import, Visibility.Public);
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
                if (txt.StartsWith("package"))
                {
                    packageLine = line;
                    firstLine = line;
                }
                // skip Haxe #if blocks
                else if (txt.StartsWithOrdinal("#if ") && txt.IndexOfOrdinal("#end") < 0) skipIfDef++;
                else if (skipIfDef > 0)
                {
                    if (txt.StartsWithOrdinal("#end")) skipIfDef--;
                    else continue;
                }
                // insert imports after a package declaration
                else if (txt.Length > 6 && txt.StartsWithOrdinal("import") && txt[6] <= 32)
                {
                    packageLine = -1;
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

                if (packageLine >= 0 && !IsHaxe && txt.IndexOf('{') >= 0)
                {
                    packageLine = -1;
                    indent = sci.GetLineIndentation(line - 1) + PluginBase.MainForm.Settings.IndentSize;
                    firstLine = line;
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
        static private int lookupPosition;

        public static void InsertCode(int position, string src)
        {
            InsertCode(position, src, ASContext.CurSciControl);
        }

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

        private static void UpdateLookupPosition(int position, int delta)
        {
            if (lookupPosition > position)
            {
                if (lookupPosition < position + delta) lookupPosition = position;// replaced text at cursor position
                else lookupPosition += delta;
            }
        }

        private static void AddLookupPosition()
        {
            AddLookupPosition(ASContext.CurSciControl);
        }

        private static void AddLookupPosition(ScintillaControl sci)
        {
            if (lookupPosition >= 0 && sci != null)
            {
                int lookupLine = sci.LineFromPosition(lookupPosition);
                int lookupCol = lookupPosition - sci.PositionFromLine(lookupLine);
                // TODO: Refactor, doesn't make a lot of sense to have this feature inside the Panel
                ASContext.Panel.SetLastLookupPosition(sci.FileName, lookupLine, lookupCol);
            }
        }
        #endregion     
    }

    #region related structures
    /// <summary>
    /// Available generators
    /// </summary>
    public enum GeneratorJobType:int
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
    }

    /// <summary>
    /// Generation completion list item
    /// </summary>
    class GeneratorItem : ICompletionListItem
    {
        private string label;
        private GeneratorJobType job;
        private MemberModel member;
        private ClassModel inClass;
        private Object data;

        public GeneratorItem(string label, GeneratorJobType job, MemberModel member, ClassModel inClass)
        {
            this.label = label;
            this.job = job;
            this.member = member;
            this.inClass = inClass;
        }

        public GeneratorItem(string label, GeneratorJobType job, MemberModel member, ClassModel inClass, Object data) : this(label, job, member, inClass)
        {
            this.data = data;
        }

        public string Label
        {
            get { return label; }
        }
        public string Description
        {
            get { return TextHelper.GetString("Info.GeneratorTemplate"); }
        }

        public Bitmap Icon
        {
            get { return (Bitmap)ASContext.Panel.GetIcon(PluginUI.ICON_DECLARATION); }
        }

        public string Value
        {
            get
            {
                ASGenerator.GenerateJob(job, member, inClass, label, data);
                return null;
            }
        }

        public Object Data
        {
            get
            {
                return data;
            }
        }
    }

    internal class FoundDeclaration
    {
        public MemberModel member;
        public ClassModel inClass;

        public FoundDeclaration()
        {
            member = null;
            inClass = ClassModel.VoidClass;
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
        public ASResult resolve;
        public Int32 position;
        public String word;

        public StatementReturnType(ASResult resolve, Int32 position, String word)
        {
            this.resolve = resolve;
            this.position = position;
            this.word = word;
        }
    }
    #endregion
}

