using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
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
using WeifenLuo.WinFormsUI.Docking;

namespace ASCompletion.Completion
{
    public class ASGenerator
    {
        #region context detection (ie. entry points)
        const string patternEvent = "Listener\\s*\\((\\s*([a-z_0-9.\\\"']+)\\s*,)?\\s*(?<event>[a-z_0-9.\\\"']+)\\s*,\\s*(this\\.)?{0}";
        const string patternAS2Delegate = @"\.\s*create\s*\(\s*[a-z_0-9.]+,\s*{0}";
        const string patternVarDecl = @"\s*{0}\s*:\s*{1}";
        const string patternMethod = @"{0}\s*\(";
        const string patternMethodDecl = @"function\s+{0}\s*\(";
        const string patternClass = @"new\s*{0}";
        const string BlankLine = "$(Boundary)\n\n";
        const string NewLine = "$(Boundary)\n";
        static private Regex reModifiers = new Regex("^\\s*(\\$\\(Boundary\\))?([a-z ]+)(function|var|const)", RegexOptions.Compiled);
        static private Regex reModifier = new Regex("(public |private |protected )", RegexOptions.Compiled);

        static private string contextToken;
        static private string contextParam;
        static private Match contextMatch;
        static private ASResult contextResolved;
        static private MemberModel contextMember;
        static private bool firstVar;

        static List<ICompletionListItem> known;

        static private bool isHaxe
        {
            get { return ASContext.Context.CurrentModel.haXe; }
        }

        static public bool HandleGeneratorCompletion(ScintillaControl Sci, bool autoHide, string word)
        {
            ContextFeatures features = ASContext.Context.Features;
            if (features.overrideKey != null && word == features.overrideKey)
                return HandleOverrideCompletion(Sci, autoHide);
            return false;
        }

        public static List<ICompletionListItem> ContextualGenerator(ScintillaControl Sci)
        {
            known = new List<ICompletionListItem>();

            if (ASContext.Context is ASContext) (ASContext.Context as ASContext).UpdateCurrentFile(false); // update model
            if ((ASContext.Context.CurrentClass.Flags & (FlagType.Enum | FlagType.TypeDef)) > 0) return known;

            lookupPosition = -1;
            int position = Sci.CurrentPos;
            int style = Sci.BaseStyleAt(position);
            if (style == 19) // on keyword
                return known;

            bool isNotInterface = (ASContext.Context.CurrentClass.Flags & FlagType.Interface) == 0;
            int line = Sci.LineFromPosition(position);
            contextToken = Sci.GetWordFromPosition(position);
            contextMatch = null;

            FoundDeclaration found = GetDeclarationAtLine(Sci, line);
            string text = Sci.GetLine(line);
            bool suggestItemDeclaration = false;

            if (isNotInterface)
            {
                if (style == 4 || style == 6 || style == 7)
                {
                    ShowConvertToConst(found);
                    return known;
                }
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
                    ShowImplementInterface(found);
                    return known;
                }

                if (resolve.Member != null && !ASContext.Context.CurrentClass.IsVoid()
                    && (resolve.Member.Flags & FlagType.LocalVar) > 0) // promote to class var
                {
                    contextMember = resolve.Member;
                    ShowPromoteLocalAndAddParameter(found);
                    return known;
                }
            }
            
            if (contextToken != null && resolve.Member == null) // import declaration
            {
                if ((resolve.Type == null || resolve.Type.IsVoid() || !ASContext.Context.IsImported(resolve.Type, line)) && CheckAutoImport(found)) return known;
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
                        if (type.IsVoid() && CheckAutoImport(found))
                            return known;
                    }
                    ShowGetSetList(found);
                    return known;
                }
                // inside a function
                else if ((found.member.Flags & (FlagType.Function | FlagType.Getter | FlagType.Setter)) > 0
                    && resolve.Member == null && resolve.Type == null)
                {
                    if (contextToken != null)
                    {
                        // "generate event handlers" suggestion
                        string re = String.Format(patternEvent, contextToken);
                        Match m = Regex.Match(text, re, RegexOptions.IgnoreCase);
                        if (m.Success)
                        {
                            contextMatch = m;
                            contextParam = CheckEventType(m.Groups["event"].Value);
                            ShowEventList(found);
                            return known;
                        }
                        m = Regex.Match(text, String.Format(patternAS2Delegate, contextToken), RegexOptions.IgnoreCase);
                        if (m.Success)
                        {
                            contextMatch = m;
                            ShowDelegateList(found);
                            return known;
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
                                ShowDelegateList(found);
                                return known;
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
                                ShowEventList(found);
                            }
                            return known;
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
                                    if (delegateName.StartsWith("on")) delegateName = delegateName.Substring(2);
                                    GenerateDefaultHandlerName(Sci, position, offset, delegateName, false);
                                    resolve = ASComplete.GetExpressionType(Sci, Sci.CurrentPos);
                                    if (resolve.Member == null || (resolve.Member.Flags & FlagType.AutomaticVar) > 0)
                                    {
                                        contextMatch = m;
                                        ShowDelegateList(found);
                                    }
                                    return known;
                                }
                            }
                        }
                    }
                }

                // "Generate fields from parameters" suggestion
                if (found.member != null
                    && (found.member.Flags & FlagType.Function) > 0
                    && found.member.Parameters != null && (found.member.Parameters.Count > 0)
                    && resolve.Member != null && (resolve.Member.Flags & FlagType.ParameterVar) > 0)
                {
                    contextMember = resolve.Member;
                    ShowFieldFromParameter(found);
                    return known;
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
                        ShowAddInterfaceDefList(found, interfaces);
                        return known;
                    }
                }

                // "assign var to statement" suggestion
                int curLine = Sci.CurrentLine;
                string ln = Sci.GetLine(curLine).TrimEnd();
                if (ln.Length > 0 && ln.IndexOf("=") == -1 
                    && ln.Length <= Sci.CurrentPos - Sci.PositionFromLine(curLine)) // cursor at end of line
                {
                    ShowAssignStatementToVarList(found);
                    return known;
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
                    ShowConstructorAndToStringList(found, hasConstructor, hasToString);
                    return known;
                }
            }

            if (isNotInterface 
                && resolve.Member != null
                && resolve.Type != null
                && resolve.Type.QualifiedName == "String"
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
                                ShowEventMetatagList(found);
                                return known;
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
                        ShowNewClassList(found);
                    }
                    else if (!found.inClass.IsVoid())
                    {
                        m = Regex.Match(text, String.Format(patternMethod, contextToken));
                        if (m.Success)
                        {
                            contextMatch = m;
                            ShowNewMethodList(found);
                        }
                        else ShowNewVarList(found);
                    }
                }
                else
                {
                    if (resolve != null
                        && resolve.InClass != null
                        && resolve.InClass.InFile != null
                        && resolve.Member != null
                        && (resolve.Member.Flags & FlagType.Function) > 0
                        && File.Exists(resolve.InClass.InFile.FileName)
                        && !resolve.InClass.InFile.FileName.StartsWith(PathHelper.AppDir))
                    {
                        Match m = Regex.Match(text, String.Format(patternMethodDecl, contextToken));
                        Match m2 = Regex.Match(text, String.Format(patternMethod, contextToken));
                        if (!m.Success && m2.Success)
                        {
                            contextMatch = m;
                            ShowChangeMethodDeclList(found);
                        }
                    }
                    else if (resolve != null
                        && resolve.Type != null
                        && resolve.Type.InFile != null
                        && resolve.RelClass != null
                        && File.Exists(resolve.Type.InFile.FileName)
                        && !resolve.Type.InFile.FileName.StartsWith(PathHelper.AppDir))
                    {
                        Match m = Regex.Match(text, String.Format(patternClass, contextToken));
                        if (m.Success)
                        {
                            contextMatch = m;
                            ShowChangeConstructorDeclList(found);
                        }
                    }
                }
            }
            // TODO: Empty line, show generators list? yep
            return known;
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
                    if (import.Type.EndsWith(dotType))
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

        private static void GenerateDefaultHandlerName(ScintillaControl Sci, int position, int targetPos, string eventName, bool closeBrace)
        {
            string target = null;
            int contextOwnerPos = GetContextOwnerEndPos(Sci, Sci.WordStartPosition(targetPos, true));
            if (contextOwnerPos != -1)
            {
                ASResult contextOwnerResult = ASComplete.GetExpressionType(Sci, contextOwnerPos);
                if (contextOwnerResult != null && !contextOwnerResult.IsNull()
                    && contextOwnerResult.Member != null)
                {
                    if (contextOwnerResult.Member.Name == "contentLoaderInfo" && Sci.CharAt(contextOwnerPos) == '.')
                    {
                        // we want to name the event from the loader var and not from the contentLoaderInfo parameter
                        contextOwnerPos = GetContextOwnerEndPos(Sci, Sci.WordStartPosition(contextOwnerPos - 1, true));
                        if (contextOwnerPos != -1)
                        {
                            contextOwnerResult = ASComplete.GetExpressionType(Sci, contextOwnerPos);
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

            char c = (char)Sci.CharAt(position - 1);
            if (c == ',') InsertCode(position, "$(Boundary) " + contextToken + "$(Boundary)");
            else InsertCode(position, contextToken);

            position = Sci.WordEndPosition(position + 1, true);
            Sci.SetSel(position, position);
            c = (char)Sci.CharAt(position);
            if (c <= 32) if (closeBrace) Sci.ReplaceSel(");"); else Sci.ReplaceSel(";");

            Sci.SetSel(position, position);
        }

        private static FoundDeclaration GetDeclarationAtLine(ScintillaControl Sci, int line)
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

        private static bool CheckAutoImport(FoundDeclaration found)
        {
            MemberList allClasses = ASContext.Context.GetAllProjectClasses();
            if (allClasses != null)
            {
                List<string> names = new List<string>();
                List<MemberModel> matches = new List<MemberModel>();
                string dotToken = "." + contextToken;
                foreach (MemberModel member in allClasses)
                    if (member.Name.EndsWith(dotToken) && !names.Contains(member.Name))
                    {
                        matches.Add(member);
                        names.Add(member.Name);
                    }
                if (matches.Count > 0)
                {
                    ShowImportClass(matches);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// For the Event handlers generator:
        /// check that the event name's const is declared in an Event type
        /// </summary>
        private static string CheckEventType(string name)
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

        private static void ShowImportClass(List<MemberModel> matches)
        {
            if (matches.Count == 1)
            {
                GenerateJob(GeneratorJobType.AddImport, matches[0], null, null, null);
                return;
            }
            
            foreach (MemberModel member in matches)
            {
                if ((member.Flags & FlagType.Class) > 0)
                    known.Add(new GeneratorItem("import " + member.Type, GeneratorJobType.AddImport, member, null));
                else if (member.IsPackageLevel)
                    known.Add(new GeneratorItem("import " + member.Name, GeneratorJobType.AddImport, member, null));
            }
        }

        private static void ShowPromoteLocalAndAddParameter(FoundDeclaration found)
        {
            string label = TextHelper.GetString("ASCompletion.Label.PromoteLocal");
            string labelMove = TextHelper.GetString("ASCompletion.Label.MoveDeclarationOnTop");
            string labelParam = TextHelper.GetString("ASCompletion.Label.AddAsParameter");
            known.Add(new GeneratorItem(label, GeneratorJobType.PromoteLocal, found.member, found.inClass));
            known.Add(new GeneratorItem(labelMove, GeneratorJobType.MoveLocalUp, found.member, found.inClass));
            known.Add(new GeneratorItem(labelParam, GeneratorJobType.AddAsParameter, found.member, found.inClass));
        }

        private static void ShowConvertToConst(FoundDeclaration found)
        {
            string label = TextHelper.GetString("ASCompletion.Label.ConvertToConst");
            known.Add(new GeneratorItem(label, GeneratorJobType.ConvertToConst, found.member, found.inClass));
        }

        private static void ShowImplementInterface(FoundDeclaration found)
        {
            string label = TextHelper.GetString("ASCompletion.Label.ImplementInterface");
            known.Add(new GeneratorItem(label, GeneratorJobType.ImplementInterface, null, found.inClass));
        }

        private static void ShowNewVarList(FoundDeclaration found)
        {
            bool generateClass = GetLangIsValid();
            ScintillaControl Sci = ASContext.CurSciControl;
            int currentPos = Sci.CurrentPos;
            ASResult exprAtCursor = ASComplete.GetExpressionType(Sci, Sci.WordEndPosition(currentPos, true));
            if (exprAtCursor == null || exprAtCursor.InClass == null || found.inClass.QualifiedName.Equals(exprAtCursor.RelClass.QualifiedName))
                exprAtCursor = null;
            ASResult exprLeft = null;
            int curWordStartPos = Sci.WordStartPosition(currentPos, true);
            if ((char)Sci.CharAt(curWordStartPos - 1) == '.') exprLeft = ASComplete.GetExpressionType(Sci, curWordStartPos - 1);
            if (exprLeft != null && exprLeft.Type == null) exprLeft = null;
            if (exprLeft != null)
            {
                if (exprLeft.Type.InFile != null && !File.Exists(exprLeft.Type.InFile.FileName)) return;
                generateClass = false;
                ClassModel curClass = ASContext.Context.CurrentClass;
                if (!isHaxe)
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
                known.Add(new GeneratorItem(label, GeneratorJobType.FunctionPublic, found.member, found.inClass));
            }
            else
            {
                string textAtCursor = Sci.GetWordFromPosition(currentPos);
                bool isConst = textAtCursor != null && textAtCursor.ToUpper().Equals(textAtCursor);
                if (isConst)
                {
                    label = TextHelper.GetString("ASCompletion.Label.GenerateConstant");
                    known.Add(new GeneratorItem(label, GeneratorJobType.Constant, found.member, found.inClass));
                }

                bool genProtectedDecl = ASContext.Context.Features.protectedKey != null && ASContext.CommonSettings.GenerateProtectedDeclarations;
                if (exprAtCursor == null && exprLeft == null)
                {
                    if (genProtectedDecl) label = TextHelper.GetString("ASCompletion.Label.GenerateProtectedVar");
                    else label = TextHelper.GetString("ASCompletion.Label.GeneratePrivateVar");
                    known.Add(new GeneratorItem(label, GeneratorJobType.Variable, found.member, found.inClass));
                }

                label = TextHelper.GetString("ASCompletion.Label.GeneratePublicVar");
                known.Add(new GeneratorItem(label, GeneratorJobType.VariablePublic, found.member, found.inClass));

                if (exprAtCursor == null && exprLeft == null)
                {
                    if (genProtectedDecl) label = TextHelper.GetString("ASCompletion.Label.GenerateProtectedFunction");
                    else label = TextHelper.GetString("ASCompletion.Label.GeneratePrivateFunction");
                    known.Add(new GeneratorItem(label, GeneratorJobType.Function, found.member, found.inClass));
                }

                label = TextHelper.GetString("ASCompletion.Label.GenerateFunctionPublic");
                known.Add(new GeneratorItem(label, GeneratorJobType.FunctionPublic, found.member, found.inClass));

                if (generateClass)
                {
                    label = TextHelper.GetString("ASCompletion.Label.GenerateClass");
                    known.Add(new GeneratorItem(label, GeneratorJobType.Class, found.member, found.inClass));
                }
            }
        }

        private static void ShowChangeMethodDeclList(FoundDeclaration found)
        {
            string label = TextHelper.GetString("ASCompletion.Label.ChangeMethodDecl");
            known.Add(new GeneratorItem(label, GeneratorJobType.ChangeMethodDecl, found.member, found.inClass));
        }

        private static void ShowChangeConstructorDeclList(FoundDeclaration found)
        {
            string label = TextHelper.GetString("ASCompletion.Label.ChangeConstructorDecl");
            known.Add(new GeneratorItem(label, GeneratorJobType.ChangeConstructorDecl, found.member, found.inClass));
        }

        private static void ShowNewMethodList(FoundDeclaration found)
        {
            ScintillaControl Sci = ASContext.CurSciControl;
            ASResult result = ASComplete.GetExpressionType(Sci, Sci.WordEndPosition(Sci.CurrentPos, true));
            if (result == null || result.RelClass == null || found.inClass.QualifiedName.Equals(result.RelClass.QualifiedName))
                result = null;
            string label;
            ClassModel inClass = result != null ? result.RelClass : found.inClass;
            bool isInterface = (inClass.Flags & FlagType.Interface) > 0;
            if (!isInterface && result == null)
            {
                if (ASContext.Context.Features.protectedKey != null && ASContext.CommonSettings.GenerateProtectedDeclarations)
                    label = TextHelper.GetString("ASCompletion.Label.GenerateProtectedFunction");
                else label = TextHelper.GetString("ASCompletion.Label.GeneratePrivateFunction");
                known.Add(new GeneratorItem(label, GeneratorJobType.Function, found.member, found.inClass));
            }
            if (isInterface) label = TextHelper.GetString("ASCompletion.Label.GenerateFunctionInterface");
            else label = TextHelper.GetString("ASCompletion.Label.GenerateFunctionPublic");
            known.Add(new GeneratorItem(label, GeneratorJobType.FunctionPublic, found.member, found.inClass));
            label = TextHelper.GetString("ASCompletion.Label.GeneratePublicCallback");
            known.Add(new GeneratorItem(label, GeneratorJobType.VariablePublic, found.member, found.inClass));
        }

        private static void ShowAssignStatementToVarList(FoundDeclaration found)
        {
            if (GetLangIsValid())
            {
                string labelClass = TextHelper.GetString("ASCompletion.Label.AssignStatementToVar");
                known.Add(new GeneratorItem(labelClass, GeneratorJobType.AssignStatementToVar, found.member, found.inClass));
            }
        }

        private static void ShowNewClassList(FoundDeclaration found)
        {
            if (GetLangIsValid())
            {
                string labelClass = TextHelper.GetString("ASCompletion.Label.GenerateClass");
                known.Add(new GeneratorItem(labelClass, GeneratorJobType.Class, found.member, found.inClass));
            }
        }

        private static void ShowConstructorAndToStringList(FoundDeclaration found, bool hasConstructor, bool hasToString)
        {
            if (GetLangIsValid())
            {
                if (!hasConstructor)
                {
                    string labelClass = TextHelper.GetString("ASCompletion.Label.GenerateConstructor");
                    known.Add(new GeneratorItem(labelClass, GeneratorJobType.Constructor, found.member, found.inClass));
                }

                if (!hasToString)
                {
                    string labelClass = TextHelper.GetString("ASCompletion.Label.GenerateToString");
                    known.Add(new GeneratorItem(labelClass, GeneratorJobType.ToString, found.member, found.inClass));
                }
            }
        }

        private static void ShowEventMetatagList(FoundDeclaration found)
        {
            string label = TextHelper.GetString("ASCompletion.Label.GenerateEventMetatag");
            known.Add(new GeneratorItem(label, GeneratorJobType.EventMetatag, found.member, found.inClass));
        }

        private static void ShowFieldFromParameter(FoundDeclaration found)
        {
            if (GetLangIsValid())
            {
                Hashtable parameters = new Hashtable();
                parameters["scope"] = GetDefaultVisibility();
                string label;
                if (ASContext.Context.Features.protectedKey != null && ASContext.CommonSettings.GenerateProtectedDeclarations)
                    label = TextHelper.GetString("ASCompletion.Label.GenerateProtectedFieldFromParameter");
                else label = TextHelper.GetString("ASCompletion.Label.GeneratePrivateFieldFromParameter");
                known.Add(new GeneratorItem(label, GeneratorJobType.FieldFromPatameter, found.member, found.inClass, parameters));
                parameters = new Hashtable();
                parameters["scope"] = Visibility.Public;
                label = TextHelper.GetString("ASCompletion.Label.GeneratePublicFieldFromParameter");
                known.Add(new GeneratorItem(label, GeneratorJobType.FieldFromPatameter, found.member, found.inClass, parameters));
            }
        }

        private static void ShowAddInterfaceDefList(FoundDeclaration found, List<string> interfaces)
        {
            if (GetLangIsValid())
            {
                string labelClass = TextHelper.GetString("ASCompletion.Label.AddInterfaceDef");
                foreach (String interf in interfaces)
                {
                    known.Add(new GeneratorItem(String.Format(labelClass, interf), GeneratorJobType.AddInterfaceDef, found.member, found.inClass, interf));
                }
            }
        }

        private static void ShowDelegateList(FoundDeclaration found)
        {
            string label = String.Format(TextHelper.GetString("ASCompletion.Label.GenerateHandler"), "Delegate");
            known.Add(new GeneratorItem(label, GeneratorJobType.Delegate, found.member, found.inClass));
        }

        private static void ShowEventList(FoundDeclaration found)
        {
            string tmp = TextHelper.GetString("ASCompletion.Label.GenerateHandler");
            string labelEvent = String.Format(tmp, "Event");
            string labelDataEvent = String.Format(tmp, "DataEvent");
            string labelContext = String.Format(tmp, contextParam);
            string[] choices = (contextParam != "Event") ?
                new string[] { labelContext, labelEvent } :
                new string[] { labelEvent, labelDataEvent };
            for (int i = 0; i < choices.Length; i++)
            {
                known.Add(new GeneratorItem(choices[i],
                    choices[i] == labelContext ? GeneratorJobType.ComplexEvent : GeneratorJobType.BasicEvent,
                    found.member, found.inClass));
            }
        }

        private static void ShowGetSetList(FoundDeclaration found)
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
                known.Add(new GeneratorItem(label, GeneratorJobType.GetterSetter, found.member, found.inClass));
            }
            if (!hasGetter)
            {
                string label = TextHelper.GetString("ASCompletion.Label.GenerateGet");
                known.Add(new GeneratorItem(label, GeneratorJobType.Getter, found.member, found.inClass));
            }
            if (!hasSetter)
            {
                string label = TextHelper.GetString("ASCompletion.Label.GenerateSet");
                known.Add(new GeneratorItem(label, GeneratorJobType.Setter, found.member, found.inClass));
            }
        }

        private static bool GetLangIsValid()
        {
            IProject project = PluginBase.CurrentProject;
            if (project == null)
                return false;

            return project.Language.StartsWith("as")
                || project.Language.StartsWith("haxe")
                || project.Language.StartsWith("loom");
        }

        #endregion

        #region code generation

        static private Regex reInsert = new Regex("\\s*([a-z])", RegexOptions.Compiled);

        static public void SetJobContext(String contextToken, String contextParam, MemberModel contextMember, Match contextMatch)
        {
            ASGenerator.contextToken = contextToken;
            ASGenerator.contextParam = contextParam;
            ASGenerator.contextMember = contextMember;
            ASGenerator.contextMatch = contextMatch;
        }

        static public void GenerateJob(GeneratorJobType job, MemberModel member, ClassModel inClass, string itemLabel, Object data)
        {
            ScintillaControl Sci = ASContext.CurSciControl;
            lookupPosition = Sci.CurrentPos;

            int position;
            MemberModel latest = null;
            bool detach = true;
            switch (job)
            {
                case GeneratorJobType.Getter:
                case GeneratorJobType.Setter:
                case GeneratorJobType.GetterSetter:
                    GenerateProperty(job, member, inClass, Sci);
                    break;

                case GeneratorJobType.BasicEvent:
                case GeneratorJobType.ComplexEvent:

                    latest = TemplateUtils.GetTemplateBlockMember(Sci,
                        TemplateUtils.GetBoundary("EventHandlers"));
                    if (latest == null)
                    {
                        if (ASContext.CommonSettings.MethodsGenerationLocations == MethodsGenerationLocations.AfterSimilarAccessorMethod)
                            latest = GetLatestMemberForFunction(inClass, GetDefaultVisibility(), member);
                        if (latest == null)
                            latest = member;
                    }

                    position = Sci.PositionFromLine(latest.LineTo + 1) - ((Sci.EOLMode == 0) ? 2 : 1);
                    Sci.SetSel(position, position);
                    string type = contextParam;
                    if (job == GeneratorJobType.BasicEvent)
                        if (itemLabel.IndexOf("DataEvent") >= 0) type = "DataEvent"; else type = "Event";
                    GenerateEventHandler(contextToken, type, member, position);
                    break;

                case GeneratorJobType.Delegate:
                    position = Sci.PositionFromLine(member.LineTo + 1) - ((Sci.EOLMode == 0) ? 2 : 1);
                    Sci.SetSel(position, position);
                    GenerateDelegateMethod(contextToken, member, position);
                    break;

                case GeneratorJobType.Constant:
                case GeneratorJobType.Variable:
                case GeneratorJobType.VariablePublic:
                    Sci.BeginUndoAction();
                    try
                    {
                        GenerateVariableJob(job, Sci, member, detach, inClass);
                    }
                    finally
                    {
                        Sci.EndUndoAction();
                    }
                    break;

                case GeneratorJobType.Function:
                case GeneratorJobType.FunctionPublic:
                    Sci.BeginUndoAction();
                    try
                    {
                        GenerateFunctionJob(job, Sci, member, detach, inClass);
                    }
                    finally
                    {
                        Sci.EndUndoAction();
                    }
                    break;

                case GeneratorJobType.ImplementInterface:
                    ClassModel aType = ASContext.Context.ResolveType(contextParam, ASContext.Context.CurrentModel);
                    if (aType.IsVoid()) return;

                    latest = GetLatestMemberForFunction(inClass, Visibility.Public, null);
                    if (latest == null)
                        latest = FindLatest(0, 0, inClass, false, false);

                    if (latest == null) return;

                    position = Sci.PositionFromLine(latest.LineTo + 1) - ((Sci.EOLMode == 0) ? 2 : 1);
                    Sci.SetSel(position, position);
                    GenerateImplementation(aType, position);
                    break;

                case GeneratorJobType.MoveLocalUp:
                    Sci.BeginUndoAction();
                    try
                    {
                        if (!RemoveLocalDeclaration(Sci, contextMember)) return;

                        position = GetBodyStart(member.LineFrom, member.LineTo, Sci);
                        Sci.SetSel(position, position);

                        string varType = contextMember.Type;
                        if (varType == "") varType = null;

                        string template = TemplateUtils.GetTemplate("Variable");
                        template = TemplateUtils.ReplaceTemplateVariable(template, "Name", contextMember.Name);
                        template = TemplateUtils.ReplaceTemplateVariable(template, "Type", varType);
                        template = TemplateUtils.ReplaceTemplateVariable(template, "Modifiers", null);
                        template = TemplateUtils.ReplaceTemplateVariable(template, "Value", null);
                        template += "\n$(Boundary)";

                        lookupPosition += SnippetHelper.InsertSnippetText(Sci, position, template);

                        Sci.SetSel(lookupPosition, lookupPosition);
                    }
                    finally
                    {
                        Sci.EndUndoAction();
                    }
                    break;

                case GeneratorJobType.PromoteLocal:
                    Sci.BeginUndoAction();
                    try
                    {
                        if (!RemoveLocalDeclaration(Sci, contextMember)) return;

                        latest = GetLatestMemberForVariable(GeneratorJobType.Variable, inClass, GetDefaultVisibility(), member);
                        if (latest == null) return;

                        position = FindNewVarPosition(Sci, inClass, latest);
                        if (position <= 0) return;
                        Sci.SetSel(position, position);

                        contextMember.Flags -= FlagType.LocalVar;
                        if ((member.Flags & FlagType.Static) > 0)
                            contextMember.Flags |= FlagType.Static;
                        contextMember.Access = GetDefaultVisibility();
                        GenerateVariable(contextMember, position, detach);

                        Sci.SetSel(lookupPosition, lookupPosition);
                    }
                    finally
                    {
                        Sci.EndUndoAction();
                    }
                    break;

                case GeneratorJobType.AddAsParameter:
                    Sci.BeginUndoAction();
                    try
                    {
                        AddAsParameter(inClass, Sci, member, detach);
                    }
                    finally
                    {
                        Sci.EndUndoAction();
                    }
                    
                    break;

                case GeneratorJobType.AddImport:
                    position = Sci.CurrentPos;
                    if ((member.Flags & (FlagType.Class | FlagType.Enum | FlagType.Struct | FlagType.TypeDef)) == 0)
                    {
                        if (member.InFile == null) break;
                        member.Type = member.Name;
                    }
                    Sci.BeginUndoAction();
                    try
                    {
                        int offset = InsertImport(member, true);
                        position += offset;
                        Sci.SetSel(position, position);
                    }
                    finally
                    {
                        Sci.EndUndoAction();
                    }
                    break;

                case GeneratorJobType.Class:
                    String clasName = Sci.GetWordFromPosition(Sci.CurrentPos);
                    GenerateClass(Sci, clasName, inClass);
                    break;

                case GeneratorJobType.Constructor:
                    member = new MemberModel(inClass.Name, inClass.QualifiedName, FlagType.Constructor | FlagType.Function, Visibility.Public);
                    GenerateFunction(
                        member,
                        Sci.CurrentPos, false, inClass);
                    break;

                case GeneratorJobType.ToString:
                    Sci.BeginUndoAction();
                    try
                    {
                        GenerateToString(inClass, Sci, member);
                    }
                    finally
                    {
                        Sci.EndUndoAction();
                    }
                    break;

                case GeneratorJobType.FieldFromPatameter:
                    Sci.BeginUndoAction();
                    try
                    {
                        GenerateFieldFromParameter(Sci, member, inClass, (Visibility)(((Hashtable)data)["scope"]));
                    }
                    finally
                    {
                        Sci.EndUndoAction();
                    }
                    break;

                case GeneratorJobType.AddInterfaceDef:
                    Sci.BeginUndoAction();
                    try
                    {
                        AddInterfaceDefJob(inClass, Sci, member, (String)data);
                    }
                    finally
                    {
                        Sci.EndUndoAction();
                    }
                    break;

                case GeneratorJobType.ConvertToConst:
                    Sci.BeginUndoAction();
                    try
                    {
                        ConvertToConst(inClass, Sci, member, detach);
                    }
                    finally
                    {
                        Sci.EndUndoAction();
                    }
                    break;

                case GeneratorJobType.ChangeMethodDecl:
                    Sci.BeginUndoAction();
                    try
                    {
                        ChangeMethodDecl(Sci, member, inClass);
                    }
                    finally
                    {
                        Sci.EndUndoAction();
                    }
                    break;

                case GeneratorJobType.ChangeConstructorDecl:
                    Sci.BeginUndoAction();
                    try
                    {
                        ChangeConstructorDecl(Sci, member, inClass);
                    }
                    finally
                    {
                        Sci.EndUndoAction();
                    }
                    break;

                case GeneratorJobType.EventMetatag:
                    Sci.BeginUndoAction();
                    try
                    {
                        EventMetatag(inClass, Sci, member);
                    }
                    finally
                    {
                        Sci.EndUndoAction();
                    }
                    break;

                case GeneratorJobType.AssignStatementToVar:
                    Sci.BeginUndoAction();
                    try
                    {
                        AssignStatementToVar(inClass, Sci, member);
                    }
                    finally
                    {
                        Sci.EndUndoAction();
                    }
                    break;
            }
        }

        private static void GenerateProperty(GeneratorJobType job, MemberModel member, ClassModel inClass, ScintillaControl Sci)
        {
            MemberModel latest;
            string name = GetPropertyNameFor(member);
            PropertiesGenerationLocations location = ASContext.CommonSettings.PropertiesGenerationLocation;

            latest = TemplateUtils.GetTemplateBlockMember(Sci, TemplateUtils.GetBoundary("AccessorsMethods"));
            if (latest != null)
            {
                location = PropertiesGenerationLocations.AfterLastPropertyDeclaration;
            }
            else
            {
                if (location == PropertiesGenerationLocations.AfterLastPropertyDeclaration)
                {
                    if (isHaxe) latest = FindLatest(FlagType.Function, 0, inClass, false, false);
                    else latest = FindLatest(FlagType.Getter | FlagType.Setter, 0, inClass, false, false);
                }
                else latest = member;
            }
            if (latest == null) return;

            Sci.BeginUndoAction();
            try
            {
                if (isHaxe)
                {
                    if (name == null) name = member.Name;
                    string args = "(default, default)";
                    if (job == GeneratorJobType.GetterSetter) args = "(get, set)";
                    else if (job == GeneratorJobType.Getter) args = "(get, null)";
                    else if (job == GeneratorJobType.Setter) args = "(default, set)";
                    MakeHaxeProperty(Sci, member, args);
                }
                else
                {
                    if ((member.Access & Visibility.Public) > 0) // hide member
                    {
                        MakePrivate(Sci, member);
                    }
                    if (name == null) // rename var with starting underscore
                    {
                        name = member.Name;
                        string newName = GetNewPropertyNameFor(member);
                        if (RenameMember(Sci, member, newName)) member.Name = newName;
                    }
                }

                int atLine = latest.LineTo + 1;
                if (location == PropertiesGenerationLocations.BeforeVariableDeclaration)
                    atLine = latest.LineTo;
                int position = Sci.PositionFromLine(atLine) - ((Sci.EOLMode == 0) ? 2 : 1);

                if (job == GeneratorJobType.GetterSetter)
                {
                    Sci.SetSel(position, position);
                    GenerateGetterSetter(name, member, position);
                }
                else
                {
                    if (job != GeneratorJobType.Getter)
                    {
                        Sci.SetSel(position, position);
                        GenerateSetter(name, member, position);
                    }
                    if (job != GeneratorJobType.Setter)
                    {
                        Sci.SetSel(position, position);
                        GenerateGetter(name, member, position);
                    }
                }
            }
            finally
            {
                Sci.EndUndoAction();
            }
        }

        private static void AssignStatementToVar(ClassModel inClass, ScintillaControl Sci, MemberModel member)
        {
            int lineNum = Sci.CurrentLine;
            string line = Sci.GetLine(lineNum);
            StatementReturnType returnType = GetStatementReturnType(Sci, inClass, line, Sci.PositionFromLine(lineNum));

            if (returnType == null) return;
            
            string type = null;
            string varname = null;
            ASResult resolve = returnType.resolve;
            string word = returnType.word;

            if (resolve != null && !resolve.IsNull())
            {
                if (resolve.Member != null && resolve.Member.Type != null)
                {
                    type = resolve.Member.Type;
                }
                else if (resolve.Type != null && resolve.Type.Name != null)
                {
                    type = resolve.Type.QualifiedName;
                }

                if (resolve.Member != null && resolve.Member.Name != null)
                {
                    varname = GuessVarName(resolve.Member.Name, type);
                }
            }

            if (!string.IsNullOrEmpty(word) && Char.IsDigit(word[0])) word = null;

            if (!string.IsNullOrEmpty(word) && (string.IsNullOrEmpty(type) || Regex.IsMatch(type, "(<[^]]+>)")))
                word = null;

            if (!string.IsNullOrEmpty(type) && type.Equals("void", StringComparison.OrdinalIgnoreCase))
                type = null;

            if (varname == null) varname = GuessVarName(word, type);

            if (varname != null && varname == word)
                varname = varname.Length == 1 ? varname + "1" : varname[0] + "";

            string cleanType = null;
            if (type != null) cleanType = FormatType(GetShortType(type));
            
            string template = TemplateUtils.GetTemplate("AssignVariable");
            template = TemplateUtils.ReplaceTemplateVariable(template, "Name", varname);
            template = TemplateUtils.ReplaceTemplateVariable(template, "Type", cleanType);

            int indent = Sci.GetLineIndentation(lineNum);
            int pos = Sci.PositionFromLine(lineNum) + indent / Sci.Indent;

            Sci.CurrentPos = pos;
            Sci.SetSel(pos, pos);
            InsertCode(pos, template);

            if (type != null)
            {
                ClassModel inClassForImport = null;
                if (resolve.InClass != null)
                {
                    inClassForImport = resolve.InClass;
                }
                else if (resolve.RelClass != null)
                {
                    inClassForImport = resolve.RelClass;
                }
                else 
                {
                    inClassForImport = inClass;
                }
                List<string> l = new List<string>();
                l.Add(getQualifiedType(type, inClassForImport));
                pos += AddImportsByName(l, Sci.LineFromPosition(pos));
            }
        }

        private static void EventMetatag(ClassModel inClass, ScintillaControl Sci, MemberModel member)
        {
            ASResult resolve = ASComplete.GetExpressionType(Sci, Sci.WordEndPosition(Sci.CurrentPos, true));
            string line = Sci.GetLine(inClass.LineFrom);
            int position = Sci.PositionFromLine(inClass.LineFrom) + (line.Length - line.TrimStart().Length);

            string value = resolve.Member.Value;
            if (value != null)
            {
                if (value.StartsWith("\""))
                {
                    value = value.Trim(new char[] { '"' });
                }
                else if (value.StartsWith("'"))
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

            Sci.CurrentPos = position;
            Sci.SetSel(position, position);
            InsertCode(position, template);
        }

        private static void ConvertToConst(ClassModel inClass, ScintillaControl Sci, MemberModel member, bool detach)
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

            int position = Sci.CurrentPos;
            int style = Sci.BaseStyleAt(position);
            MemberModel latest = null;

            int wordPosEnd = position + 1;
            int wordPosStart = position;

            while (Sci.BaseStyleAt(wordPosEnd) == style) wordPosEnd++;
            while (Sci.BaseStyleAt(wordPosStart - 1) == style) wordPosStart--;
            
            Sci.SetSel(wordPosStart, wordPosEnd);
            string word = Sci.SelText;
            Sci.ReplaceSel(suggestion);
            
            if (member == null)
            {
                detach = false;
                lookupPosition = -1;
                position = Sci.WordStartPosition(Sci.CurrentPos, true);
                Sci.SetSel(position, Sci.WordEndPosition(position, true));
            }
            else
            {
                latest = GetLatestMemberForVariable(GeneratorJobType.Constant, inClass, 
                    Visibility.Private, new MemberModel("", "", FlagType.Static, 0));
                if (latest != null)
                {
                    position = FindNewVarPosition(Sci, inClass, latest);
                }
                else
                {
                    position = GetBodyStart(inClass.LineFrom, inClass.LineTo, Sci);
                    detach = false;
                }
                if (position <= 0) return;
                Sci.SetSel(position, position);
            }

            MemberModel m = NewMember(suggestion, member, FlagType.Variable | FlagType.Constant | FlagType.Static);

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

        private static void ChangeMethodDecl(ScintillaControl Sci, MemberModel member, ClassModel inClass)
        {
            int wordPos = Sci.WordEndPosition(Sci.CurrentPos, true);
            List<FunctionParameter> functionParameters = ParseFunctionParameters(Sci, wordPos);

            ASResult funcResult = ASComplete.GetExpressionType(Sci, Sci.WordEndPosition(Sci.CurrentPos, true));
            if (funcResult == null || funcResult.Member == null) return;
            if (funcResult.InClass != null && !funcResult.InClass.Equals(inClass))
            {
                AddLookupPosition();
                lookupPosition = -1;

                ASContext.MainForm.OpenEditableDocument(funcResult.InClass.InFile.FileName, true);
                Sci = ASContext.CurSciControl;

                FileModel fileModel = new FileModel();
                fileModel.Context = ASContext.Context;
                ASFileParser parser = new ASFileParser();
                parser.ParseSrc(fileModel, Sci.Text);

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

            ChangeDecl(Sci, funcResult.Member, functionParameters);
        }

        private static void ChangeConstructorDecl(ScintillaControl Sci, MemberModel member, ClassModel inClass)
        {
            int wordPos = Sci.WordEndPosition(Sci.CurrentPos, true);
            List<FunctionParameter> functionParameters = ParseFunctionParameters(Sci, wordPos);
            ASResult funcResult = ASComplete.GetExpressionType(Sci, Sci.WordEndPosition(Sci.CurrentPos, true));

            if (funcResult == null || funcResult.Type == null) return;
            if (funcResult.Type != null && !funcResult.Type.Equals(inClass))
            {
                AddLookupPosition();
                lookupPosition = -1;

                ASContext.MainForm.OpenEditableDocument(funcResult.Type.InFile.FileName, true);
                Sci = ASContext.CurSciControl;

                FileModel fileModel = new FileModel(funcResult.Type.InFile.FileName);
                fileModel.Context = ASContext.Context;
                ASFileParser parser = new ASFileParser();
                parser.ParseSrc(fileModel, Sci.Text);

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
            if (isHaxe) funcResult.Member.Name = "new";

            ChangeDecl(Sci, funcResult.Member, functionParameters);
        }

        private static void ChangeDecl(ScintillaControl Sci, MemberModel memberModel, List<FunctionParameter> functionParameters)
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
                        newParameters.Add(new MemberModel(p.paramName, p.paramType, FlagType.ParameterVar, 0));
                    }
                }
                memberModel.Parameters = newParameters;

                int posStart = Sci.PositionFromLine(memberModel.LineFrom);
                int posEnd = Sci.LineEndPosition(memberModel.LineTo);
                Sci.SetSel(posStart, posEnd);
                string selectedText = Sci.SelText;
                Regex rStart = new Regex(@"\s{1}" + memberModel.Name + @"\s*\(([^\)]*)\)(\s*:\s*([^({{|\n|\r|\s|;)]+))?");
                Match mStart = rStart.Match(selectedText);
                if (!mStart.Success)
                {
                    return;
                }

                int start = mStart.Index + posStart;
                int end = start + mStart.Length;

                Sci.SetSel(start, end);

                string decl = TemplateUtils.ToDeclarationString(memberModel, TemplateUtils.GetTemplate("MethodDeclaration"));
                InsertCode(Sci.CurrentPos, "$(Boundary) " + decl);

                // add imports to function argument types
                if (functionParameters.Count > 0)
                {
                    List<string> l = new List<string>();
                    foreach (FunctionParameter fp in functionParameters)
                    {
                        try
                        {
                            l.Add(fp.paramQualType);
                        }
                        catch (Exception)
                        {
                        }
                    }
                    start += AddImportsByName(l, Sci.LineFromPosition(end));
                }

                Sci.SetSel(start, start);
            }
        }

        private static void AddAsParameter(ClassModel inClass, ScintillaControl Sci, MemberModel member, bool detach)
        {
            if (!RemoveLocalDeclaration(Sci, contextMember)) return;

            int posStart = Sci.PositionFromLine(member.LineFrom);
            int posEnd = Sci.LineEndPosition(member.LineTo);
            Sci.SetSel(posStart, posEnd);
            string selectedText = Sci.SelText;
            Regex rStart = new Regex(@"\s{1}" + member.Name + @"\s*\(([^\)]*)\)(\s*:\s*([^({{|\n|\r|\s|;)]+))?");
            Match mStart = rStart.Match(selectedText);
            if (!mStart.Success)
                return;

            int start = mStart.Index + posStart + 1;
            int end = mStart.Index + posStart + mStart.Length;

            Sci.SetSel(start, end);

            MemberModel memberCopy = (MemberModel) member.Clone();

            if (memberCopy.Parameters == null)
                memberCopy.Parameters = new List<MemberModel>();

            memberCopy.Parameters.Add(contextMember);

            string template = TemplateUtils.ToDeclarationString(memberCopy, TemplateUtils.GetTemplate("MethodDeclaration"));
            InsertCode(start, template);

            int currPos = Sci.LineEndPosition(Sci.CurrentLine);

            Sci.SetSel(currPos, currPos);
            Sci.CurrentPos = currPos;
        }

        private static void AddInterfaceDefJob(ClassModel inClass, ScintillaControl Sci, MemberModel member, string interf)
        {
            ClassModel aType = ASContext.Context.ResolveType(interf, ASContext.Context.CurrentModel);
            if (aType.IsVoid()) return;

            FileModel fileModel = ASFileParser.ParseFile(ASContext.Context.CreateFileModel(aType.InFile.FileName));
            foreach (ClassModel cm in fileModel.Classes)
            {
                if (cm.QualifiedName.Equals(aType.QualifiedName))
                {
                    aType = cm;
                    break;
                }
            }

            string template = TemplateUtils.GetTemplate("IFunction");
            if ((member.Flags & FlagType.Getter) > 0)
            {
                template = TemplateUtils.GetTemplate("IGetter");
            }
            else if ((member.Flags & FlagType.Setter) > 0)
            {
                template = TemplateUtils.GetTemplate("ISetter");
            }

            ASContext.MainForm.OpenEditableDocument(aType.InFile.FileName, true);
            Sci = ASContext.CurSciControl;

            MemberModel latest = GetLatestMemberForFunction(aType, Visibility.Default, new MemberModel());
            int position;
            if (latest == null)
            {
                position = GetBodyStart(aType.LineFrom, aType.LineTo, Sci);
            }
            else
            {
                position = Sci.PositionFromLine(latest.LineTo + 1) - ((Sci.EOLMode == 0) ? 2 : 1);
                template = NewLine + template;
            }
            Sci.SetSel(position, position);
            Sci.CurrentPos = position;

            IASContext context = ASContext.Context;
            ContextFeatures features = context.Features;

            template = TemplateUtils.ToDeclarationString(member, template);
            template = TemplateUtils.ReplaceTemplateVariable(template, "BlankLine", NewLine);
            template = TemplateUtils.ReplaceTemplateVariable(template, "Void", features.voidKey);

            List<string> importsList = new List<string>();
            string t;
            List<MemberModel> parms = member.Parameters;
            if (parms != null && parms.Count > 0)
            {
                for (int i = 0; i < parms.Count; i++)
                {
                    if (parms[i].Type != null)
                    {
                        t = getQualifiedType(parms[i].Type, inClass); 
                        importsList.Add(t);
                    }
                }
            }

            if (member.Type != null)
            {
                t = getQualifiedType(member.Type, inClass);
                importsList.Add(t);
            }

            if (importsList.Count > 0)
            {
                int o = AddImportsByName(importsList, Sci.LineFromPosition(position));
                position += o;
                
            }

            Sci.SetSel(position, position);
            Sci.CurrentPos = position;

            InsertCode(position, template);
        }

        private static void GenerateFieldFromParameter(ScintillaControl Sci, MemberModel member, ClassModel inClass,
                    Visibility scope)
        {
            int funcBodyStart = GetBodyStart(member.LineFrom, member.LineTo, Sci);

            Sci.SetSel(funcBodyStart, Sci.LineEndPosition(member.LineTo));
            string body = Sci.SelText;
            string trimmed = body.TrimStart();

            Regex re = new Regex("^super\\s*[\\(|\\.]");
            Match m = re.Match(trimmed);
            if (m.Success)
            {
                if (m.Index == 0)
                {
                    int p = funcBodyStart + (body.Length - trimmed.Length);
                    int l = Sci.LineFromPosition(p);
                    p = Sci.PositionFromLine(l + 1);
                    funcBodyStart = GetBodyStart(member.LineFrom, member.LineTo, Sci, p);
                }
            }

            Sci.SetSel(funcBodyStart, funcBodyStart);
            Sci.CurrentPos = funcBodyStart;

            bool isVararg = false;
            string paramName = contextMember.Name;
            if (paramName.StartsWith("..."))
            {
                paramName = paramName.TrimStart(new char[] { ' ', '.' });
                isVararg = true;
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
                if (ASContext.CommonSettings.PrefixFields.Length > 0 && !paramName.StartsWith(ASContext.CommonSettings.PrefixFields))
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

            SnippetHelper.InsertSnippetText(Sci, funcBodyStart, template);

            MemberList classMembers = inClass.Members;
            foreach (MemberModel classMember in classMembers)
                if (classMember.Name.Equals(varName))
                    return;

            MemberModel latest = GetLatestMemberForVariable(GeneratorJobType.Variable, inClass, GetDefaultVisibility(), new MemberModel());
            if (latest == null) return;

            int position = FindNewVarPosition(Sci, inClass, latest);
            if (position <= 0) return;
            Sci.SetSel(position, position);
            Sci.CurrentPos = position;

            MemberModel mem = NewMember(varName, member, FlagType.Variable, scope);
            if (isVararg) mem.Type = "Array";
            else mem.Type = contextMember.Type;

            GenerateVariable(mem, position, true);
            ASContext.Panel.RestoreLastLookupPosition();
        }

        public static int GetBodyStart(int lineFrom, int lineTo, ScintillaControl Sci)
        {
            return GetBodyStart(lineFrom, lineTo, Sci, -1);
        }

        public static int GetBodyStart(int lineFrom, int lineTo, ScintillaControl Sci, int pos)
        {
            int posStart = Sci.PositionFromLine(lineFrom);
            int posEnd = Sci.LineEndPosition(lineTo);

            Sci.SetSel(posStart, posEnd);

            List<char> characterClass = new List<char>(new char[] { ' ', '\r', '\n', '\t' });
            string currentMethodBody = Sci.SelText;
            int nCount = 0;
            int funcBodyStart = pos;
            int extraLine = 0;
            if (pos == -1)
            {
                funcBodyStart = posStart + currentMethodBody.IndexOf('{');
                extraLine = 1;
            }
            while (funcBodyStart <= posEnd)
            {
                char c = (char)Sci.CharAt(++funcBodyStart);
                if (c == '}')
                {
                    int ln = Sci.LineFromPosition(funcBodyStart);
                    int indent = Sci.GetLineIndentation(ln);
                    if (lineFrom == lineTo || lineFrom == ln)
                    {
                        Sci.InsertText(funcBodyStart, Sci.NewLineMarker);
                        Sci.SetLineIndentation(ln + 1, indent);
                        ln++;
                    }
                    Sci.SetLineIndentation(ln, indent + Sci.Indent);
                    Sci.InsertText(funcBodyStart, Sci.NewLineMarker);
                    Sci.SetLineIndentation(ln + 1, indent);
                    Sci.SetLineIndentation(ln, indent + Sci.Indent);
                    funcBodyStart = Sci.LineEndPosition(ln);
                    break;
                }
                else if (!characterClass.Contains(c))
                {
                    break;
                }
                else if (Sci.EOLMode == 1 && c == '\r' && (++nCount) > extraLine)
                {
                    break;
                }
                else if (c == '\n' && (++nCount) > extraLine)
                {
                    if (Sci.EOLMode != 2)
                    {
                        funcBodyStart--;
                    }
                    break;
                }
            }
            return funcBodyStart;
        }

        private static void GenerateToString(ClassModel inClass, ScintillaControl Sci, MemberModel member)
        {
            MemberModel resultMember = new MemberModel("toString", "String", FlagType.Function, Visibility.Public);

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
            result = TemplateUtils.ReplaceTemplateVariable(result, "Body", "\"[" + inClass.Name + membersString.ToString() + "]\"");

            InsertCode(Sci.CurrentPos, result);
        }

        private static void GenerateVariableJob(GeneratorJobType job, ScintillaControl Sci, MemberModel member,
            bool detach, ClassModel inClass)
        {
            int position = 0;
            MemberModel latest = null;
            bool isOtherClass = false;

            Visibility varVisi = job.Equals(GeneratorJobType.Variable) ? GetDefaultVisibility() : Visibility.Public;
            FlagType ft = job.Equals(GeneratorJobType.Constant) ? FlagType.Constant : FlagType.Variable;

            // evaluate, if the variable (or constant) should be generated in other class
            ASResult varResult = ASComplete.GetExpressionType(Sci, Sci.WordEndPosition(Sci.CurrentPos, true));

            int contextOwnerPos = GetContextOwnerEndPos(Sci, Sci.WordStartPosition(Sci.CurrentPos, true));
            MemberModel isStatic = new MemberModel();
            if (contextOwnerPos != -1)
            {
                ASResult contextOwnerResult = ASComplete.GetExpressionType(Sci, contextOwnerPos);
                if (contextOwnerResult != null)
                {
                    if (contextOwnerResult.Member == null && contextOwnerResult.Type != null)
                    {
                        isStatic.Flags |= FlagType.Static;
                    }
                }
            }
            else if (member != null && (member.Flags & FlagType.Static) > 0)
            {
                isStatic.Flags |= FlagType.Static;
            }

            ASResult returnType = null;
            int lineNum = Sci.CurrentLine;
            string line = Sci.GetLine(lineNum);
            
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
                    int posLineStart = Sci.PositionFromLine(lineNum);
                    if (posLineStart + m.Index >= Sci.CurrentPos)
                    {
                        line = line.Substring(m.Index);
                        StatementReturnType rType = GetStatementReturnType(Sci, inClass, line, posLineStart + m.Index);
                        if (rType != null)
                        {
                            returnType = rType.resolve;
                        }
                    }
                }
            }

            if (varResult.RelClass != null && !varResult.RelClass.IsVoid() && !varResult.RelClass.Equals(inClass))
            {
                AddLookupPosition();
                lookupPosition = -1;

                ASContext.MainForm.OpenEditableDocument(varResult.RelClass.InFile.FileName, false);
                Sci = ASContext.CurSciControl;
                isOtherClass = true;

                FileModel fileModel = new FileModel();
                fileModel.Context = ASContext.Context;
                ASFileParser parser = new ASFileParser();
                parser.ParseSrc(fileModel, Sci.Text);

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

            latest = GetLatestMemberForVariable(job, inClass, varVisi, isStatic);
            
            // if we generate variable in current class..
            if (!isOtherClass && member == null)
            {
                detach = false;
                lookupPosition = -1;
                position = Sci.WordStartPosition(Sci.CurrentPos, true);
                Sci.SetSel(position, Sci.WordEndPosition(position, true));
            }
            else // if we generate variable in another class
            {
                if (latest != null)
                {
                    position = FindNewVarPosition(Sci, inClass, latest);
                }
                else
                {
                    position = GetBodyStart(inClass.LineFrom, inClass.LineTo, Sci);
                    detach = false;
                }
                if (position <= 0) return;
                Sci.SetSel(position, position);
            }

            // if this is a constant, we assign a value to constant
            string returnTypeStr = null;
            string eventValue = null;
            if (job == GeneratorJobType.Constant && returnType == null)
            {
                isStatic.Flags |= FlagType.Static;
                eventValue = "String = \"" + Camelize(contextToken) + "\"";
            }
            else if (returnType != null)
            {
                ClassModel inClassForImport = null;
                if (returnType.InClass != null)
                {
                    inClassForImport = returnType.InClass;
                }
                else if (returnType.RelClass != null)
                {
                    inClassForImport = returnType.RelClass;
                }
                else
                {
                    inClassForImport = inClass;
                }
                List<String> imports = new List<string>();
                if (returnType.Member != null)
                {
                    if (returnType.Member.Type != ASContext.Context.Features.voidKey)
                    {
                        returnTypeStr = FormatType(GetShortType(returnType.Member.Type));
                        imports.Add(getQualifiedType(returnType.Member.Type, inClassForImport));
                    }
                }
                else if (returnType != null && returnType.Type != null)
                {
                    returnTypeStr = FormatType(GetShortType(returnType.Type.QualifiedName));
                    imports.Add(getQualifiedType(returnType.Type.QualifiedName, inClassForImport));
                }
                if (imports.Count > 0)
                {
                    position += AddImportsByName(imports, Sci.LineFromPosition(position));
                    Sci.SetSel(position, position);
                }
            }
            MemberModel newMember = NewMember(contextToken, isStatic, ft, varVisi);
            if (returnTypeStr != null)
            {
                newMember.Type = returnTypeStr;
            }
            else if (eventValue != null)
            {
                newMember.Type = eventValue;
            }
            GenerateVariable(newMember, position, detach);
        }

        private static int GetContextOwnerEndPos(ScintillaControl Sci, int worsStartPos)
        {
            int pos = worsStartPos - 1;
            bool dotFound = false;
            while (pos > 0)
            {
                char c = (char) Sci.CharAt(pos);
                if (c == '.' && !dotFound) dotFound = true;
                else if (c == '\t' || c == '\n' || c == '\r' || c == ' ') { /* skip */ }
                else return dotFound ? pos + 1 : -1;
                pos--;
            }
            return pos;
        }

        static public string Capitalize(string name)
        {
            return !string.IsNullOrEmpty(name) ? Char.ToUpper(name[0]) + name.Substring(1) : name;
        }

        static public string Camelize(string name)
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

        private static List<FunctionParameter> ParseFunctionParameters(ScintillaControl Sci, int p)
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
            ASResult result = null;
            IASContext ctx = ASContext.Context;
            char[] charsToTrim = new char[] { ' ', '\t', '\r', '\n' };
            int counter = Sci.TextLength; // max number of chars in parameters line (to avoid infinitive loop)
            string characterClass = ScintillaControl.Configuration.GetLanguage(Sci.ConfigurationLanguage).characterclass.Characters;
            int lastMemberPos = p;

            // add [] and <>
            while (p < counter && !doBreak)
            {
                char c = (char)Sci.CharAt(p++);
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
                else if (c == ';' && !isFuncStarted)
                {
                    break;
                }
                else if (c == ')' && isFuncStarted && !wasEscapeChar && !isDoubleQuote && !isSingleQuote && subClosuresCount == 0)
                {
                    isFuncStarted = false;
                    writeParam = true;
                    doBreak = true;
                }
                else if ((c == '(' || c == '[' || c == '<' || c == '{') && !wasEscapeChar && !isDoubleQuote && !isSingleQuote)
                {
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
                                result = new ASResult();
                                result.Type = ctx.ResolveType(ctx.Features.objectKey, null);
                                types.Insert(0, result);
                            }
                        }
                        else if (c == '(')
                        {
                            result = ASComplete.GetExpressionType(Sci, lastMemberPos + 1);
                            if (!result.IsNull())
                            {
                                types.Insert(0, result);
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
                    if (c == ']')
                    {
                        result = ASComplete.GetExpressionType(Sci, p);
                        if (result.Type != null) result.Member = null;
                        else result.Type = ctx.ResolveType(ctx.Features.arrayKey, null);
                        types.Insert(0, result);
                        writeParam = true;
                    }
                    subClosuresCount--;
                    sb.Append(c);
                    wasEscapeChar = false;
                }
                else if (c == '\\')
                {
                    wasEscapeChar = !wasEscapeChar;
                    sb.Append(c);
                }
                else if (c == '"' && !wasEscapeChar && !isSingleQuote)
                {
                    isDoubleQuote = !isDoubleQuote;
                    if (subClosuresCount == 0)
                    {
                        if (isDoubleQuote)
                        {
                            result = new ASResult();
                            result.Type = ctx.ResolveType("String", null);
                            types.Add(result);
                        }
                    }
                    sb.Append(c);
                    wasEscapeChar = false;
                }
                else if (c == '\'' && !wasEscapeChar && !isDoubleQuote)
                {
                    isSingleQuote = !isSingleQuote;
                    if (subClosuresCount == 0)
                    {
                        if (isSingleQuote)
                        {
                            result = new ASResult();
                            result.Type = ctx.ResolveType("String", null);
                            types.Add(result);
                        }
                    }
                    sb.Append(c);
                    wasEscapeChar = false;
                }
                else if (c == ',' && subClosuresCount == 0)
                {
                    if (!isSingleQuote && !isDoubleQuote && subClosuresCount == 0)
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
                    doBreak = true;
                }

                if (writeParam)
                {
                    writeParam = false;
                    string trimmed = sb.ToString().Trim(charsToTrim);
                    if (trimmed.Length > 0)
                    {
                        result = ASComplete.GetExpressionType(Sci, lastMemberPos + 1);
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
                            if (result.Member.Type == null || result.Member.Type.Equals("void", StringComparison.OrdinalIgnoreCase))
                            {
                                paramType = result.Type.Name;
                                paramQualType = result.Type.QualifiedName;
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
                                    paramQualType = getQualifiedType(result.Member.Type, result.InClass);
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
                        if (prms[j] != prms[i]
                            && prms[j].paramName == suggestedName)
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

        private static void GenerateFunctionJob(GeneratorJobType job, ScintillaControl Sci, MemberModel member,
            bool detach, ClassModel inClass)
        {
            int position = 0;
            MemberModel latest = null;
            bool isOtherClass = false;

            Visibility funcVisi = job.Equals(GeneratorJobType.FunctionPublic) ? Visibility.Public : GetDefaultVisibility();
            int wordPos = Sci.WordEndPosition(Sci.CurrentPos, true);
            List<FunctionParameter> functionParameters = ParseFunctionParameters(Sci, wordPos);

            // evaluate, if the function should be generated in other class
            ASResult funcResult = ASComplete.GetExpressionType(Sci, Sci.WordEndPosition(Sci.CurrentPos, true));

            int contextOwnerPos = GetContextOwnerEndPos(Sci, Sci.WordStartPosition(Sci.CurrentPos, true));
            MemberModel isStatic = new MemberModel();
            if (contextOwnerPos != -1)
            {
                ASResult contextOwnerResult = ASComplete.GetExpressionType(Sci, contextOwnerPos);
                if (contextOwnerResult != null)
                {
                    if (contextOwnerResult.Member == null && contextOwnerResult.Type != null)
                    {
                        isStatic.Flags |= FlagType.Static;
                    }
                }
            }
            else if (member != null && (member.Flags & FlagType.Static) > 0)
            {
                isStatic.Flags |= FlagType.Static;
            }


            if (funcResult.RelClass != null && !funcResult.RelClass.IsVoid() && !funcResult.RelClass.Equals(inClass))
            {
                AddLookupPosition();
                lookupPosition = -1;

                ASContext.MainForm.OpenEditableDocument(funcResult.RelClass.InFile.FileName, true);
                Sci = ASContext.CurSciControl;
                isOtherClass = true;

                FileModel fileModel = new FileModel();
                fileModel.Context = ASContext.Context;
                ASFileParser parser = new ASFileParser();
                parser.ParseSrc(fileModel, Sci.Text);

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

            string blockTmpl = null;
            if ((isStatic.Flags & FlagType.Static) > 0)
            {
                blockTmpl = TemplateUtils.GetBoundary("StaticMethods");
            }
            else if ((funcVisi & Visibility.Public) > 0)
            {
                blockTmpl = TemplateUtils.GetBoundary("PublicMethods");
            }
            else
            {
                blockTmpl = TemplateUtils.GetBoundary("PrivateMethods");
            }
            latest = TemplateUtils.GetTemplateBlockMember(Sci, blockTmpl);
            if (latest == null || (!isOtherClass && member == null))
            {
                latest = GetLatestMemberForFunction(inClass, funcVisi, isStatic);

                // if we generate function in current class..
                if (!isOtherClass)
                {
                    MethodsGenerationLocations location = ASContext.CommonSettings.MethodsGenerationLocations;
                    if (member == null)
                    {
                        detach = false;
                        lookupPosition = -1;
                        position = Sci.WordStartPosition(Sci.CurrentPos, true);
                        Sci.SetSel(position, Sci.WordEndPosition(position, true));
                    }
                    else if (latest != null && location == MethodsGenerationLocations.AfterSimilarAccessorMethod)
                    {
                        position = Sci.PositionFromLine(latest.LineTo + 1) - ((Sci.EOLMode == 0) ? 2 : 1);
                        Sci.SetSel(position, position);
                    }
                    else
                    {
                        position = Sci.PositionFromLine(member.LineTo + 1) - ((Sci.EOLMode == 0) ? 2 : 1);
                        Sci.SetSel(position, position);
                    }
                }
                else // if we generate function in another class..
                {
                    if (latest != null)
                    {
                        position = Sci.PositionFromLine(latest.LineTo + 1) - ((Sci.EOLMode == 0) ? 2 : 1);
                    }
                    else
                    {
                        position = GetBodyStart(inClass.LineFrom, inClass.LineTo, Sci);
                        detach = false;
                    }
                    Sci.SetSel(position, position);
                }
            }
            else
            {
                position = Sci.PositionFromLine(latest.LineTo + 1) - ((Sci.EOLMode == 0) ? 2 : 1);
                Sci.SetSel(position, position);
            }

            // add imports to function argument types
            if (functionParameters.Count > 0)
            {
                List<string> l = new List<string>();
                foreach (FunctionParameter fp in functionParameters)
                {
                    try
                    {
                        l.Add(fp.paramQualType);
                    }
                    catch (Exception) { }
                }
                int o = AddImportsByName(l, Sci.LineFromPosition(position));
                position += o;
                if (latest == null)
                    Sci.SetSel(position, Sci.WordEndPosition(position, true));
                else
                    Sci.SetSel(position, position);
            }
            
            List<MemberModel> parameters = new List<MemberModel>();
            for (int i = 0; i < functionParameters.Count; i++)
            {
                string name = functionParameters[i].paramName;
                string type = functionParameters[i].paramType;
                parameters.Add(new MemberModel(name, type, FlagType.ParameterVar, 0));
            }
            MemberModel newMember = NewMember(contextToken, isStatic, FlagType.Function, funcVisi);
            newMember.Parameters = parameters;
            GenerateFunction(newMember, position, detach, inClass);
        }

        private static void GenerateClass(ScintillaControl Sci, String className, ClassModel inClass)
        {
            AddLookupPosition(); // remember last cursor position for Shift+F4

            List<FunctionParameter> parameters = ParseFunctionParameters(Sci, Sci.WordEndPosition(Sci.CurrentPos, true));
            List<MemberModel> constructorArgs = new List<MemberModel>();
            List<String> constructorArgTypes = new List<String>();
            MemberModel paramMember = new MemberModel();
            for (int i = 0; i < parameters.Count; i++)
            {
                FunctionParameter p = parameters[i];
                constructorArgs.Add(new MemberModel(p.paramName, p.paramType, FlagType.ParameterVar, 0));
                constructorArgTypes.Add(CleanType(getQualifiedType(p.paramQualType, inClass)));
            }
            
            paramMember.Parameters = constructorArgs;

            IProject project = PluginBase.CurrentProject;
            if (String.IsNullOrEmpty(className)) className = "Class";
            string projFilesDir = Path.Combine(PathHelper.TemplateDir, "ProjectFiles");
            string projTemplateDir = Path.Combine(projFilesDir, project.GetType().Name);
            string paramsString = TemplateUtils.ParametersString(paramMember, true);
            Hashtable info = new Hashtable();
            info["className"] = className;
            if (project.Language.StartsWith("as")) info["templatePath"] = Path.Combine(projTemplateDir, "Class.as.fdt");
            else if (project.Language.StartsWith("haxe")) info["templatePath"] = Path.Combine(projTemplateDir, "Class.hx.fdt");
            else if (project.Language.StartsWith("loom")) info["templatePath"] = Path.Combine(projTemplateDir, "Class.ls.fdt");
            info["inDirectory"] = Path.GetDirectoryName(inClass.InFile.FileName);
            info["constructorArgs"] = paramsString.Length > 0 ? paramsString : null;
            info["constructorArgTypes"] = constructorArgTypes;
            DataEvent de = new DataEvent(EventType.Command, "ProjectManager.CreateNewFile", info);
            EventManager.DispatchEvent(null, de);
            if (de.Handled) return;
        }

        public static void GenerateExtractVariable(ScintillaControl Sci, string NewName)
        {
            FileModel cFile;

            string expression = Sci.SelText.Trim(new char[] { '=', ' ', '\t', '\n', '\r', ';', '.' });
            expression = expression.TrimEnd(new char[] { '(', '[', '{', '<' });
            expression = expression.TrimStart(new char[] { ')', ']', '}', '>' });

            cFile = ASContext.Context.CurrentModel;
            ASFileParser parser = new ASFileParser();
            parser.ParseSrc(cFile, Sci.Text);

            MemberModel current = cFile.Context.CurrentMember;

            string characterClass = ScintillaControl.Configuration.GetLanguage(Sci.ConfigurationLanguage).characterclass.Characters;

            int funcBodyStart = GetBodyStart(current.LineFrom, current.LineTo, Sci);
            Sci.SetSel(funcBodyStart, Sci.LineEndPosition(current.LineTo));
            string currentMethodBody = Sci.SelText;

            bool isExprInSingleQuotes = (expression.StartsWith("'") && expression.EndsWith("'"));
            bool isExprInDoubleQuotes = (expression.StartsWith("\"") && expression.EndsWith("\""));
            int stylemask = (1 << Sci.StyleBits) - 1;
            int lastPos = -1;
            char prevOrNextChar;
            Sci.Colourise(0, -1);
            while (true)
            {
                lastPos = currentMethodBody.IndexOf(expression, lastPos + 1);
                if (lastPos > -1)
                {
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

                    int style = Sci.StyleAt(funcBodyStart + lastPos) & stylemask;
                    if (ASComplete.IsCommentStyle(style))
                    {
                        continue;
                    }
                    else if ((isExprInDoubleQuotes && currentMethodBody[lastPos] == '"' && currentMethodBody[lastPos + expression.Length - 1] == '"')
                        || (isExprInSingleQuotes && currentMethodBody[lastPos] == '\'' && currentMethodBody[lastPos + expression.Length - 1] == '\''))
                    {

                    }
                    else if (!ASComplete.IsTextStyle(style))
                    {
                        continue;
                    }

                    Sci.SetSel(funcBodyStart + lastPos, funcBodyStart + lastPos + expression.Length);
                    Sci.ReplaceSel(NewName);
                    currentMethodBody = currentMethodBody.Substring(0, lastPos) + NewName + currentMethodBody.Substring(lastPos + expression.Length);
                    lastPos += NewName.Length;
                }
                else
                {
                    break;
                }
            }

            Sci.CurrentPos = funcBodyStart;
            Sci.SetSel(Sci.CurrentPos, Sci.CurrentPos);

            MemberModel m = new MemberModel(NewName, "", FlagType.LocalVar, 0);
            m.Value = expression;

            string snippet = TemplateUtils.GetTemplate("Variable");
            snippet = TemplateUtils.ReplaceTemplateVariable(snippet, "Modifiers", null);
            snippet = TemplateUtils.ToDeclarationString(m, snippet);
            snippet += NewLine + "$(Boundary)";
            SnippetHelper.InsertSnippetText(Sci, Sci.CurrentPos, snippet);
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

            InsertCode(Sci.CurrentPos, template + ";");

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
                latest = GetLatestMemberForFunction(found.inClass, GetDefaultVisibility(), found.member);

            if (latest == null)
                latest = found.member;

            int position = Sci.PositionFromLine(latest.LineTo + 1) - ((Sci.EOLMode == 0) ? 2 : 1);
            Sci.SetSel(position, position);

            FlagType flags = FlagType.Function;
            if ((found.member.Flags & FlagType.Static) > 0)
            {
                flags |= FlagType.Static;
            }

            MemberModel m = new MemberModel(NewName, context.Features.voidKey, flags, GetDefaultVisibility());

            template = NewLine + TemplateUtils.GetTemplate("Function");
            template = TemplateUtils.ToDeclarationWithModifiersString(m, template);
            template = TemplateUtils.ReplaceTemplateVariable(template, "Body", selText);
            template = TemplateUtils.ReplaceTemplateVariable(template, "BlankLine", NewLine);
            InsertCode(position, template);
        }

        private static int FindNewVarPosition(ScintillaControl Sci, ClassModel inClass, MemberModel latest)
        {
            firstVar = false;
            // found a var?
            if ((latest.Flags & FlagType.Variable) > 0)
                return Sci.PositionFromLine(latest.LineTo + 1) - ((Sci.EOLMode == 0) ? 2 : 1);

            // add as first member
            int line = 0;
            int maxLine = Sci.LineCount;
            if (inClass != null)
            {
                line = inClass.LineFrom;
                maxLine = inClass.LineTo;
            }
            else if (ASContext.Context.InPrivateSection) line = ASContext.Context.CurrentModel.PrivateSectionIndex;
            else maxLine = ASContext.Context.CurrentModel.PrivateSectionIndex;
            while (line < maxLine)
            {
                string text = Sci.GetLine(line++);
                if (text.IndexOf('{') >= 0)
                {
                    firstVar = true;
                    return Sci.PositionFromLine(line) - ((Sci.EOLMode == 0) ? 2 : 1);
                }
            }
            return -1;
        }

        private static bool RemoveLocalDeclaration(ScintillaControl Sci, MemberModel contextMember)
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
                        RemoveOneLocalDeclaration(Sci, member);
                        removed++;
                    }
                }
            }
            if (removed == 0) return RemoveOneLocalDeclaration(Sci, contextMember);
            else return true;
        }

        private static bool RemoveOneLocalDeclaration(ScintillaControl Sci, MemberModel contextMember)
        {
            string type = "";
            if (contextMember.Type != null && (contextMember.Flags & FlagType.Inferred) == 0)
            {
                type = FormatType(contextMember.Type);
                if (type.IndexOf('*') > 0)
                    type = type.Replace("/*", @"/\*\s*").Replace("*/", @"\s*\*/");
                type = @":\s*" + type;
            }
            Regex reDecl = new Regex(String.Format(@"[\s\(]((var|const)\s+{0}\s*{1})\s*", contextMember.Name, type));
            for (int i = contextMember.LineFrom; i <= contextMember.LineTo + 10; i++)
            {
                string text = Sci.GetLine(i);
                Match m = reDecl.Match(text);
                if (m.Success)
                {
                    int index = Sci.MBSafeTextLength(text.Substring(0, m.Groups[1].Index));
                    int position = Sci.PositionFromLine(i) + index;
                    int len = Sci.MBSafeTextLength(m.Groups[1].Value);
                    Sci.SetSel(position, position + len);
                    if (contextMember.Type == null || (contextMember.Flags & FlagType.Inferred) != 0) Sci.ReplaceSel(contextMember.Name + " ");
                    else Sci.ReplaceSel(contextMember.Name);
                    UpdateLookupPosition(position, contextMember.Name.Length - len);
                    return true;
                }
            }
            return false;
        }

        private static StatementReturnType GetStatementReturnType(ScintillaControl Sci, ClassModel inClass, string line, int startPos)
        {
            Regex target = new Regex(@"[;\s\n\r]*", RegexOptions.RightToLeft);
            Match m = target.Match(line);
            if (!m.Success)
            {
                return null;
            }
            line = line.Substring(0, m.Index);

            if (line.Length == 0)
            {
                return null;
            }

            line = ReplaceAllStringContents(line);

            ASResult resolve = null;
            int pos = -1; 
            string word = null;
            ClassModel type = null;

            if (line[line.Length - 1] == ')')
            {
                pos = -1;
                int lastIndex = 0;
                int bracesBalance = 0;
                while (true)
                {
                    int pos1 = line.IndexOf("(", lastIndex);
                    int pos2 = line.IndexOf(")", lastIndex);
                    if (pos1 != -1 && pos2 != -1)
                    {
                        lastIndex = Math.Min(pos1, pos2);
                    }
                    else if (pos1 != -1 || pos2 != -1)
                    {
                        lastIndex = Math.Max(pos1, pos2);
                    }
                    else
                    {
                        break;
                    }
                    if (lastIndex == pos1)
                    {
                        bracesBalance++;
                        if (bracesBalance == 1)
                        {
                            pos = lastIndex;
                        }
                    }
                    else if (lastIndex == pos2)
                    {
                        bracesBalance--;
                    }
                    lastIndex++;
                }
            }
            else
            {
                pos = line.Length;
            }
            if (pos != -1)
            {
                line = line.Substring(0, pos);
                pos += startPos;
                pos -= line.Length - line.TrimEnd().Length + 1;
                pos = Sci.WordEndPosition(pos, true);
                resolve = ASComplete.GetExpressionType(Sci, pos);
                if (resolve.IsNull()) resolve = null;
                word = Sci.GetWordFromPosition(pos);
            }

            IASContext ctx = inClass.InFile.Context;
            m = Regex.Match(line, "new\\s+([\\w\\d.<>,_$-]+)+(<[^]]+>)|(<[^]]+>)", RegexOptions.IgnoreCase);

            if (m.Success)
            {
                string m1 = m.Groups[1].Value;
                string m2 = m.Groups[2].Value;

                string cname;
                if (string.IsNullOrEmpty(m1) && string.IsNullOrEmpty(m2))
                    cname = m.Groups[0].Value;
                else
                    cname = String.Concat(m1, m2);

                if (cname.StartsWith("<"))
                    cname = "Vector." + cname; // literal vector

                type = ctx.ResolveType(cname, inClass.InFile);
                if (!type.IsVoid()) resolve = null;
            }
            else
            {
                char c = (char)Sci.CharAt(pos);
                if (c == '"' || c == '\'')
                {
                    type = ctx.ResolveType("String", inClass.InFile);
                }
                else if (c == '}')
                {
                    type = ctx.ResolveType(ctx.Features.objectKey, inClass.InFile);
                }
                else if (c == '>')
                {
                    type = ctx.ResolveType("XML", inClass.InFile);
                }
                else if (c == ']')
                {
                    resolve = ASComplete.GetExpressionType(Sci, pos + 1);
                    if (resolve.Type != null) type = resolve.Type;
                    else type = ctx.ResolveType(ctx.Features.arrayKey, inClass.InFile);
                    resolve = null;
                }
                else if (word != null && Char.IsDigit(word[0]))
                {
                    type = ctx.ResolveType(ctx.Features.numberKey, inClass.InFile);
                }
                else if (word == "true" || word == "false")
                {
                    type = ctx.ResolveType(ctx.Features.booleanKey, inClass.InFile);
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
            if (name.Length > 3 && name.StartsWith("get") && (name[3].ToString() == char.ToUpper(name[3]).ToString()))
            {
                name = char.ToLower(name[3]).ToString() + name.Substring(4);
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

        private static void GenerateImplementation(ClassModel aType, int position)
        {
            List<string> typesUsed = new List<string>();

            StringBuilder sb = new StringBuilder(TemplateUtils.ReplaceTemplateVariable(TemplateUtils.GetTemplate("ImplementHeader"), "Class", aType.Type));
            sb.Append(NewLine);
            bool entry = true;
            ASResult result = new ASResult();
            IASContext context = ASContext.Context;
            ClassModel cClass = context.CurrentClass;
            ContextFeatures features = context.Features;
            bool canGenerate = false;

            aType.ResolveExtends(); // resolve inheritance chain
            while (!aType.IsVoid() && aType.QualifiedName != "Object")
            {
                foreach (MemberModel method in aType.Members)
                {
                    if ((method.Flags & (FlagType.Function | FlagType.Getter | FlagType.Setter)) == 0
                        || method.Name == aType.Name)
                        continue;

                    // check if method exists
                    ASComplete.FindMember(method.Name, cClass, result, method.Flags, 0);
                    if (!result.IsNull()) continue;

                    string decl = entry ? NewLine : "";
                    if ((method.Flags & FlagType.Getter) > 0)
                        decl = TemplateUtils.ToDeclarationWithModifiersString(method, TemplateUtils.GetTemplate("Getter"));
                    else if ((method.Flags & FlagType.Setter) > 0)
                        decl = TemplateUtils.ToDeclarationWithModifiersString(method, TemplateUtils.GetTemplate("Setter"));
                    else
                        decl = TemplateUtils.ToDeclarationWithModifiersString(method, TemplateUtils.GetTemplate("Function"));
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

                    addTypeOnce(typesUsed, getQualifiedType(method.Type, aType));

                    if (method.Parameters != null && method.Parameters.Count > 0)
                        foreach (MemberModel param in method.Parameters)
                            addTypeOnce(typesUsed, getQualifiedType(param.Type, aType));
                }
                // interface inheritance
                aType = aType.Extends;
            }
            if (!canGenerate)
                return;

            ScintillaControl Sci = ASContext.CurSciControl;
            Sci.BeginUndoAction();
            try
            {
                position = Sci.CurrentPos;
                if (ASContext.Context.Settings.GenerateImports && typesUsed.Count > 0)
                {
                    int offset = AddImportsByName(typesUsed, Sci.LineFromPosition(position));
                    position += offset;
                    Sci.SetSel(position, position);
                }
                InsertCode(position, sb.ToString());
            }
            finally { Sci.EndUndoAction(); }
        }

        private static void addTypeOnce(List<string> typesUsed, string qualifiedName)
        {
            if (!typesUsed.Contains(qualifiedName)) typesUsed.Add(qualifiedName);
        }

        private static string getQualifiedType(string type, ClassModel aType)
        {
            if (string.IsNullOrEmpty(type)) return "*";
            if (type.IndexOf('<') > 0) // Vector.<Point>
            {
                Match mGeneric = Regex.Match(type, "<([^>]+)>");
                if (mGeneric.Success)
                {
                    return getQualifiedType(mGeneric.Groups[1].Value, aType);
                }
            }

            if (type.IndexOf('.') > 0) return type;

            ClassModel aClass = ASContext.Context.ResolveType(type, aType.InFile);
            if (!aClass.IsVoid())
            {
                if (aClass.InFile.Package.Length != 0)
                    return aClass.QualifiedName;
            }
            return "*";
        }

        private static MemberModel NewMember(string contextToken, MemberModel calledFrom, FlagType kind, Visibility visi)
        {
            string type = (kind == FlagType.Function && !ASContext.Context.Features.hasInference) 
                ? ASContext.Context.Features.voidKey : null;
            if (calledFrom != null && (calledFrom.Flags & FlagType.Static) > 0)
                kind |= FlagType.Static;
            return new MemberModel(contextToken, type, kind, visi);
        }

        private static MemberModel NewMember(string contextToken, MemberModel calledFrom, FlagType kind)
        {
            return NewMember(contextToken, calledFrom, kind, GetDefaultVisibility());
        }

        /// <summary>
        /// Get Visibility.Private or Visibility.Protected, depending on user setting forcing the use of protected.
        /// </summary>
        public static Visibility GetDefaultVisibility()
        {
            if (ASContext.Context.Features.protectedKey != null && ASContext.CommonSettings.GenerateProtectedDeclarations)
                return Visibility.Protected;
            else return Visibility.Private;
        }

        private static void GenerateFunction(MemberModel member, int position, bool detach, ClassModel inClass)
        {
            string template = "";
            string decl = "";
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
                template = TemplateUtils.GetTemplate("Function");
                decl = TemplateUtils.ToDeclarationWithModifiersString(member, template);
                decl = TemplateUtils.ReplaceTemplateVariable(decl, "Body", null);
            }
            if (detach) decl = NewLine + TemplateUtils.ReplaceTemplateVariable(decl, "BlankLine", NewLine);
            else decl = TemplateUtils.ReplaceTemplateVariable(decl, "BlankLine", null);
            InsertCode(position, decl);
        }
        
        private static void GenerateVariable(MemberModel member, int position, bool detach)
        {
            string result = "";
            if ((member.Flags & FlagType.Constant) > 0)
            {
                string template = TemplateUtils.GetTemplate("Constant");
                result = TemplateUtils.ToDeclarationWithModifiersString(member, template);
                if (member.Value == null) 
                    result = TemplateUtils.ReplaceTemplateVariable(result, "Value", null);
                else
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

        private static string ReplaceAll(string template, string oldValue, string newValue)
        {
            if (template == null)
                return null;

            string result = "";
            string[] a = template.Split(new string[] { oldValue }, StringSplitOptions.None);
            for (int i = 0; i < a.Length; i++)
            {
                if (i > 0)
                    result += newValue;
                result += a[i];
            }
            return result;
        }

        public static bool MakePrivate(ScintillaControl Sci, MemberModel member)
        {
            ContextFeatures features = ASContext.Context.Features;
            string visibility = GetPrivateKeyword();
            if (features.publicKey == null || visibility == null) return false;
            Regex rePublic = new Regex(String.Format(@"\s*({0})\s+", features.publicKey));

            string line;
            Match m;
            int index, position;
            for (int i = member.LineFrom; i <= member.LineTo; i++)
            {
                line = Sci.GetLine(i);
                m = rePublic.Match(line);
                if (m.Success)
                {
                    index = Sci.MBSafeTextLength(line.Substring(0, m.Groups[1].Index));
                    position = Sci.PositionFromLine(i) + index;
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

            string line;
            Match m;
            int index, position;
            for (int i = member.LineFrom; i <= member.LineTo; i++)
            {
                line = Sci.GetLine(i);
                m = reMember.Match(line);
                if (m.Success)
                {
                    index = Sci.MBSafeTextLength(line.Substring(0, m.Groups[1].Index));
                    position = Sci.PositionFromLine(i) + index;
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

            string line;
            Match m;
            int index, position;
            for (int i = member.LineFrom; i <= member.LineTo; i++)
            {
                line = Sci.GetLine(i);
                m = reMember.Match(line);
                if (m.Success)
                {
                    index = Sci.MBSafeTextLength(line.Substring(0, m.Groups[1].Index));
                    position = Sci.PositionFromLine(i) + index;
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
            if (name.Length == 0)
                return null;
            Match parts = Regex.Match(name, "([^_$]*)[_$]+(.*)");
            if (parts.Success)
            {
                string pre = parts.Groups[1].Value;
                string post = parts.Groups[2].Value;
                return (pre.Length > post.Length) ? pre : post;
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

        private static void GenerateDelegateMethod(string name, MemberModel afterMethod, int position)
        {
            ContextFeatures features = ASContext.Context.Features;

            string acc = GetPrivateAccessor(afterMethod);
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

        private static void GenerateEventHandler(string name, string type, MemberModel afterMethod, int position)
        {
            ScintillaControl Sci = ASContext.CurSciControl;
            Sci.BeginUndoAction();
            try
            {
                int delta = 0;
                ClassModel eventClass = ASContext.Context.ResolveType(type, ASContext.Context.CurrentModel);
                if (eventClass.IsVoid())
                    if (type == "Event")
                    {
                        List<string> typesUsed = new List<string>();
                        typesUsed.Add("flash.events.Event");
                        delta = AddImportsByName(typesUsed, Sci.LineFromPosition(position));
                        position += delta;
                        Sci.SetSel(position, position);
                    }
                    else if (type == "DataEvent")
                    {
                        List<string> typesUsed = new List<string>();
                        typesUsed.Add("flash.events.DataEvent");
                        delta = AddImportsByName(typesUsed, Sci.LineFromPosition(position));
                        position += delta;
                        Sci.SetSel(position, position);
                    }
                lookupPosition += delta;
                string acc = GetPrivateAccessor(afterMethod);
                string template = TemplateUtils.GetTemplate("EventHandler");
                string decl = NewLine + TemplateUtils.ReplaceTemplateVariable(template, "Modifiers", acc);
                decl = TemplateUtils.ReplaceTemplateVariable(decl, "Name", name);
                decl = TemplateUtils.ReplaceTemplateVariable(decl, "Type", type);
                decl = TemplateUtils.ReplaceTemplateVariable(decl, "Void", ASContext.Context.Features.voidKey);

                string eventName = contextMatch.Groups["event"].Value;
                string autoRemove = AddRemoveEvent(eventName);
                if (autoRemove != null)
                {
                    if (autoRemove.Length > 0) autoRemove += ".";
                    string remove = String.Format("{0}removeEventListener({1}, {2});\n\t$(EntryPoint)", autoRemove, eventName, name);
                    decl = decl.Replace("$(EntryPoint)", remove);
                }
                InsertCode(position, decl);
            }
            finally
            {
                Sci.EndUndoAction();
            }
        }

        static private string AddRemoveEvent(string eventName)
        {
            foreach (string autoRemove in ASContext.CommonSettings.EventListenersAutoRemove)
            {
                string test = autoRemove.Trim();
                if (test.Length == 0 || test.StartsWith("//")) continue;
                int colonPos = test.IndexOf(':');
                if (colonPos >= 0) test = test.Substring(colonPos + 1);
                if (test == eventName)
                {
                    if (colonPos < 0) return "";
                    else return autoRemove.Trim().Substring(0, colonPos);
                }
            }
            return null;
        }

        private static void GenerateGetter(string name, MemberModel member, int position)
        {
            string acc;
            if (isHaxe)
            {
                acc = GetStaticKeyword(member);
                if (!string.IsNullOrEmpty(acc)) acc += " ";
            }
            else acc = GetPublicAccessor(member);
            string template = TemplateUtils.GetTemplate("Getter");
            string decl = NewLine + TemplateUtils.ReplaceTemplateVariable(template, "Modifiers", acc);
            decl = TemplateUtils.ReplaceTemplateVariable(decl, "Name", name);
            decl = TemplateUtils.ReplaceTemplateVariable(decl, "Type", FormatType(member.Type));
            decl = TemplateUtils.ReplaceTemplateVariable(decl, "Member", member.Name);
            decl = TemplateUtils.ReplaceTemplateVariable(decl, "BlankLine", NewLine);
            InsertCode(position, decl);
        }

        private static void GenerateSetter(string name, MemberModel member, int position)
        {
            string acc;
            if (isHaxe)
            {
                acc = GetStaticKeyword(member);
                if (!string.IsNullOrEmpty(acc)) acc += " ";
            } else acc = GetPublicAccessor(member);
            string template = TemplateUtils.GetTemplate("Setter");
            string decl = NewLine + TemplateUtils.ReplaceTemplateVariable(template, "Modifiers", acc);
            decl = TemplateUtils.ReplaceTemplateVariable(decl, "Name", name);
            decl = TemplateUtils.ReplaceTemplateVariable(decl, "Type", FormatType(member.Type));
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
            string acc;
            if (isHaxe)
            {
                acc = GetStaticKeyword(member);
                if (!string.IsNullOrEmpty(acc)) acc += " ";
            }
            else acc = GetPublicAccessor(member);
            string decl = NewLine + TemplateUtils.ReplaceTemplateVariable(template, "Modifiers", acc);
            decl = TemplateUtils.ReplaceTemplateVariable(decl, "Name", name);
            decl = TemplateUtils.ReplaceTemplateVariable(decl, "Type", FormatType(member.Type));
            decl = TemplateUtils.ReplaceTemplateVariable(decl, "Member", member.Name);
            decl = TemplateUtils.ReplaceTemplateVariable(decl, "Void", ASContext.Context.Features.voidKey ?? "void");
            decl = TemplateUtils.ReplaceTemplateVariable(decl, "BlankLine", NewLine);
            InsertCode(position, decl);
        }

        private static string GetStaticKeyword(MemberModel member)
        {
            if ((member.Flags & FlagType.Static) > 0) return ASContext.Context.Features.staticKey ?? "static";
            return string.Empty;
        }

        private static string GetPrivateAccessor(MemberModel member)
        {
            string acc = GetStaticKeyword(member);
            if (!string.IsNullOrEmpty(acc)) acc += " ";
            return acc + GetPrivateKeyword();
        }

        private static string GetPrivateKeyword()
        {
            if (GetDefaultVisibility() == Visibility.Protected) return ASContext.Context.Features.protectedKey ?? "protected";
            return ASContext.Context.Features.privateKey ?? "private";
        }

        private static string GetPublicAccessor(MemberModel member)
        {
            string acc = GetStaticKeyword(member);
            if (!string.IsNullOrEmpty(acc)) acc += " ";
            return acc + ASContext.Context.Features.publicKey ?? "public";
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

        static private MemberModel FindMember(string name, ClassModel inClass)
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

        static private MemberModel FindLatest(FlagType match, ClassModel inClass)
        {
            return FindLatest(match, 0, inClass);
        }

        static private MemberModel FindLatest(FlagType match, Visibility visi, ClassModel inClass)
        {
            return FindLatest(match, visi, inClass, true, true);
        }

        static private MemberModel FindLatest(FlagType match, Visibility visi, ClassModel inClass,
                bool isFlagMatchStrict, bool isVisibilityMatchStrict)
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
                else if (isFlagMatchStrict && !isVisibilityMatchStrict)
                {
                    if ((member.Flags & match) == match && (visi == 0 || (member.Access & visi) > 0))
                    {
                        latest = member;
                    }
                }
                else if (!isFlagMatchStrict && isVisibilityMatchStrict)
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
        
        static private string GetDeclaration(MemberModel member)
        {
            return GetDeclaration(member, true);
        }

        static private string GetDeclaration(MemberModel member, bool addModifiers)
        {
            // modifiers
            string modifiers = TemplateUtils.GetStaticExternOverride(member);
            if (addModifiers) modifiers += TemplateUtils.GetModifiers(member);
            
            // signature
            FlagType ft = member.Flags;
            if ((ft & FlagType.Getter) > 0)
                return String.Format("{0}function get {1}", modifiers, member.ToDeclarationString());
            else if ((ft & FlagType.Setter) > 0)
                return String.Format("{0}function set {1}", modifiers, member.ToDeclarationString());
            else if (ft == FlagType.Function)
                return String.Format("{0}function {1}", modifiers, member.ToDeclarationString());
            else if (((ft & FlagType.Constant) > 0) && ASContext.Context.Settings.LanguageId != "AS2")
                return String.Format("{0}const {1}", modifiers, member.ToDeclarationString());
            else
                return String.Format("{0}var {1}", modifiers, member.ToDeclarationString());
        }
        #endregion

        #region override generator
        /// <summary>
        /// List methods to override
        /// </summary>
        /// <param name="Sci">Scintilla control</param>
        /// <param name="autoHide">Don't keep the list open if the word does not match</param>
        /// <returns>Completion was handled</returns>
        static private bool HandleOverrideCompletion(ScintillaControl Sci, bool autoHide)
        {
            // explore members
            IASContext ctx = ASContext.Context;
            ClassModel curClass = ctx.CurrentClass;
            if (curClass.IsVoid()) return false;

            List<MemberModel> members = new List<MemberModel>();
            curClass.ResolveExtends(); // Resolve inheritance chain

            // explore function or getters or setters
            FlagType mask = FlagType.Function | FlagType.Getter | FlagType.Setter;
            ClassModel tmpClass = curClass.Extends;
            Visibility acc = ctx.TypesAffinity(curClass, tmpClass);
            while (tmpClass != null && !tmpClass.IsVoid())
            {
                if (tmpClass.QualifiedName.StartsWith("flash.utils.Proxy"))
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
                        if ((member.Flags & FlagType.Dynamic) > 0
                            && (member.Flags & mask) > 0
                            && (member.Access & acc) > 0) members.Add(member);

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

        static public void GenerateOverride(ScintillaControl Sci, ClassModel ofClass, MemberModel member, int position)
        {
            ContextFeatures features = ASContext.Context.Features;
            List<string> typesUsed = new List<string>();
            bool isProxy = (member.Namespace == "flash_proxy");
            if (isProxy) typesUsed.Add("flash.utils.flash_proxy");
            bool isAS2Event = ASContext.Context.Settings.LanguageId == "AS2" && member.Name.StartsWith("on");
            bool isObjectMethod = ofClass.QualifiedName == "Object";

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

            FlagType flags = member.Flags;
            string acc = "";
            string decl = "";
            if (features.hasNamespaces && !string.IsNullOrEmpty(member.Namespace) && member.Namespace != "internal")
                acc = member.Namespace;
            else if ((member.Access & Visibility.Public) > 0) acc = features.publicKey;
            else if ((member.Access & Visibility.Internal) > 0) acc = features.internalKey;
            else if ((member.Access & Visibility.Protected) > 0) acc = features.protectedKey;
            else if ((member.Access & Visibility.Private) > 0 && features.methodModifierDefault != Visibility.Private) 
                acc = features.privateKey;

            bool isStatic = (flags & FlagType.Static) > 0;
            if (isStatic) acc = features.staticKey + " " + acc;

            if (!isAS2Event && !isObjectMethod)
                acc = features.overrideKey + " " + acc;

            acc = Regex.Replace(acc, "[ ]+", " ").Trim();

            if ((flags & (FlagType.Getter | FlagType.Setter)) > 0)
            {
                string type = member.Type;
                string name = member.Name;
                if (member.Parameters != null && member.Parameters.Count == 1)
                    type = member.Parameters[0].Type;
                type = FormatType(type);
                if (type == null && !features.hasInference) type = features.objectKey;

                bool genGetter = ofClass.Members.Search(name, FlagType.Getter, 0) != null;
                bool genSetter = ofClass.Members.Search(name, FlagType.Setter, 0) != null;

                if (isHaxe)
                {
                    // property is public but not the methods
                    acc = features.overrideKey;
                }

                if (genGetter)
                {
                    string tpl = TemplateUtils.GetTemplate("OverrideGetter", "Getter");
                    tpl = TemplateUtils.ReplaceTemplateVariable(tpl, "Modifiers", acc);
                    tpl = TemplateUtils.ReplaceTemplateVariable(tpl, "Name", name);
                    tpl = TemplateUtils.ReplaceTemplateVariable(tpl, "Type", type);
                    tpl = TemplateUtils.ReplaceTemplateVariable(tpl, "Member", "super." + name);
                    decl += tpl;
                }
                if (genSetter)
                {
                    string tpl = TemplateUtils.GetTemplate("OverrideSetter", "Setter");
                    tpl = TemplateUtils.ReplaceTemplateVariable(tpl, "Modifiers", acc);
                    tpl = TemplateUtils.ReplaceTemplateVariable(tpl, "Name", name);
                    tpl = TemplateUtils.ReplaceTemplateVariable(tpl, "Type", type);
                    tpl = TemplateUtils.ReplaceTemplateVariable(tpl, "Member", "super." + name);
                    tpl = TemplateUtils.ReplaceTemplateVariable(tpl, "Void", ASContext.Context.Features.voidKey ?? "void");
                    if (decl.Length > 0)
                    {
                        tpl = "\n\n" + tpl.Replace("$(EntryPoint)", "");
                    }
                    decl += tpl;
                }
                decl = TemplateUtils.ReplaceTemplateVariable(decl, "BlankLine", "");
            }
            else
            {
                string type = FormatType(member.Type);
                //if (type == null) type = features.objectKey;
                
                decl = acc + features.functionKey + " ";
                bool noRet = type == null || type.Equals("void", StringComparison.OrdinalIgnoreCase);
                type = (noRet && type != null) ? ASContext.Context.Features.voidKey : type;
                if (!noRet)
                {
                    string qType = getQualifiedType(type, ofClass);
                    typesUsed.Add(qType);
                    if (qType == type)
                    {
                        ClassModel rType = ASContext.Context.ResolveType(type, ofClass.InFile);
                        if (!rType.IsVoid()) type = rType.Name;
                    }
                }

                string action = (isProxy || isAS2Event) ? "" : GetSuperCall(member, typesUsed, ofClass);
                string template = TemplateUtils.GetTemplate("MethodOverride");
                
                // fix parameters if needed
                if (member.Parameters != null)
                    foreach (MemberModel para in member.Parameters)
                       if (para.Type == "any") para.Type = "*";

                template = TemplateUtils.ReplaceTemplateVariable(template, "Modifiers", acc);
                template = TemplateUtils.ReplaceTemplateVariable(template, "Name", member.Name);
                template = TemplateUtils.ReplaceTemplateVariable(template, "Arguments", TemplateUtils.ParametersString(member, true));
                template = TemplateUtils.ReplaceTemplateVariable(template, "Type", type);
                template = TemplateUtils.ReplaceTemplateVariable(template, "Method", action);
                decl = template;
            }

            Sci.BeginUndoAction();
            try
            {
                if (ASContext.Context.Settings.GenerateImports && typesUsed.Count > 0)
                {
                    int offset = AddImportsByName(typesUsed, line);
                    position += offset;
                    startPos += offset;
                }

                Sci.SetSel(startPos, position + member.Name.Length);
                InsertCode(startPos, decl);
            }
            finally { Sci.EndUndoAction(); }
        }

        public static void GenerateDelegateMethods(ScintillaControl Sci, MemberModel member,
            Dictionary<MemberModel, ClassModel> selectedMembers, ClassModel classModel, ClassModel inClass)
        {
            Sci.BeginUndoAction();
            try
            {
                string result = TemplateUtils.ReplaceTemplateVariable(
                    TemplateUtils.GetTemplate("DelegateMethodsHeader"), 
                    "Class", 
                    classModel.Type);

                int position = -1;
                ClassModel type;
                List<string> importsList = new List<string>();
                bool isStaticMember = false;

                if ((member.Flags & FlagType.Static) > 0)
                    isStaticMember = true;

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

                    if (isStaticMember && (m.Flags & FlagType.Static) == 0)
                        mCopy.Flags |= FlagType.Static;

                    if ((m.Flags & FlagType.Setter) > 0)
                    {
                        methodTemplate += TemplateUtils.GetTemplate("Setter");
                        methodTemplate = TemplateUtils.ReplaceTemplateVariable(methodTemplate, "Modifiers", 
                            (TemplateUtils.GetStaticExternOverride(m) + TemplateUtils.GetModifiers(m)).Trim());
                        methodTemplate = TemplateUtils.ReplaceTemplateVariable(methodTemplate, "Name", m.Name);
                        methodTemplate = TemplateUtils.ReplaceTemplateVariable(methodTemplate, "EntryPoint", "");
                        methodTemplate = TemplateUtils.ReplaceTemplateVariable(methodTemplate, "Type", m.Parameters[0].Type);
                        methodTemplate = TemplateUtils.ReplaceTemplateVariable(methodTemplate, "Member", member.Name + "." + m.Name);
                        methodTemplate = TemplateUtils.ReplaceTemplateVariable(methodTemplate, "Void", ASContext.Context.Features.voidKey ?? "void");
                    }
                    else if ((m.Flags & FlagType.Getter) > 0)
                    {
                        methodTemplate += TemplateUtils.GetTemplate("Getter");
                        methodTemplate = TemplateUtils.ReplaceTemplateVariable(methodTemplate, "Modifiers",
                            (TemplateUtils.GetStaticExternOverride(m) + TemplateUtils.GetModifiers(m)).Trim());
                        methodTemplate = TemplateUtils.ReplaceTemplateVariable(methodTemplate, "Name", m.Name);
                        methodTemplate = TemplateUtils.ReplaceTemplateVariable(methodTemplate, "EntryPoint", "");
                        methodTemplate = TemplateUtils.ReplaceTemplateVariable(methodTemplate, "Type", FormatType(m.Type));
                        methodTemplate = TemplateUtils.ReplaceTemplateVariable(methodTemplate, "Member", member.Name + "." + m.Name);
                    }
                    else
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
                            if (mm.Name.StartsWith("..."))
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
                                m.Parameters[m.Parameters.Count - 1].Name.TrimStart(new char[] { '.', ' '}));
                        }

                        methodTemplate = TemplateUtils.ReplaceTemplateVariable(methodTemplate, "Body", callMethodTemplate);
                    }
                    methodTemplate = TemplateUtils.ReplaceTemplateVariable(methodTemplate, "BlankLine", NewLine);
                    result += methodTemplate;

                    if (m.Parameters != null)
                    {
                        for (int i = 0; i < m.Parameters.Count; i++)
                        {
                            MemberModel param = m.Parameters[i];
                            if (param.Type != null)
                            {
                                type = ASContext.Context.ResolveType(param.Type, selectedMembers[m].InFile);
                                importsList.Add(type.QualifiedName);
                            }
                        }
                    }

                    if (position < 0)
                    {
                        MemberModel latest = GetLatestMemberForFunction(inClass, mCopy.Access, mCopy);
                        if (latest == null)
                        {
                            position = Sci.WordStartPosition(Sci.CurrentPos, true);
                            Sci.SetSel(position, Sci.WordEndPosition(position, true));
                        }
                        else
                        {
                            position = Sci.PositionFromLine(latest.LineTo + 1) - ((Sci.EOLMode == 0) ? 2 : 1);
                            Sci.SetSel(position, position);
                        }
                    }
                    else
                    {
                        position = Sci.CurrentPos;
                    }

                    if (m.Type != null)
                    {
                        type = ASContext.Context.ResolveType(m.Type, selectedMembers[m].InFile);
                        importsList.Add(type.QualifiedName);
                    }
                }

                if (importsList.Count > 0 && position > -1)
                {
                    int o = AddImportsByName(importsList, Sci.LineFromPosition(position));
                    position += o;
                    Sci.SetSel(position, position);
                }

                InsertCode(position, result);
            }
            finally { Sci.EndUndoAction(); }
        }

        private static void GetStartPos(string currentText, ref int startPos, string keyword)
        {
            if (keyword == null) return;
            int p = currentText.IndexOf(keyword);
            if (p > 0 && p < startPos) startPos = p;
        }

        private static string GetShortType(string type)
        {
            if (string.IsNullOrEmpty(type))
            {
                return type;
            }
            Regex r = new Regex(@"[^\.]+(\.<.+>)?(@.+|$)");
            Match m = r.Match(type);
            if (m.Success)
            {
                type = m.Value;
            }
            return type;
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
            p = type.IndexOf("@");
            if (p > 0)
            {
                type = type.Substring(0, p);
            }
            return type;
        }

        private static string GetSuperCall(MemberModel member, List<string> typesUsed, ClassModel aType)
        {
            string args = "";
            if (member.Parameters != null)
                foreach (MemberModel param in member.Parameters)
                {
                    if (param.Name.StartsWith(".")) break;
                    args += ", " + TemplateUtils.GetParamName(param);
                    addTypeOnce(typesUsed, getQualifiedType(param.Type, aType));
                }

            bool noRet = string.IsNullOrEmpty(member.Type) || member.Type.Equals("void", StringComparison.OrdinalIgnoreCase);
            if (!noRet) addTypeOnce(typesUsed, getQualifiedType(member.Type, aType));

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
        private static int AddImportsByName(List<string> typesUsed, int atLine)
        {
            int length = 0;
            IASContext context = ASContext.Context;
            List<string> addedTypes = new List<string>();
            string cleanType = null;
            foreach (string type in typesUsed)
            {
                cleanType = CleanType(type);
                if (string.IsNullOrEmpty(cleanType) || cleanType.IndexOf('.') <= 0 || addedTypes.Contains(cleanType))
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
            string txt;
            int indent = 0;
            int skipIfDef = 0;
            Match mImport;
            while (line < curLine)
            {
                txt = sci.GetLine(line++).TrimStart();
                if (txt.StartsWith("package"))
                {
                    packageLine = line;
                    firstLine = line;
                }
                // skip Haxe #if blocks
                else if (txt.StartsWith("#if ") && txt.IndexOf("#end") < 0) skipIfDef++;
                else if (skipIfDef > 0)
                {
                    if (txt.StartsWith("#end")) skipIfDef--;
                    else continue;
                }
                // insert imports after a package declaration
                else if (txt.StartsWith("import"))
                {
                    packageLine = -1;
                    found = true;
                    indent = sci.GetLineIndentation(line - 1);
                    // insert in alphabetical order
                    mImport = ASFileParserRegexes.Import.Match(txt);
                    if (mImport.Success &&
                        String.Compare(mImport.Groups["package"].Value, fullPath) > 0)
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

                if (packageLine >= 0 && !isHaxe && txt.IndexOf('{') >= 0)
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
            ScintillaControl Sci = ASContext.CurSciControl;
            Sci.BeginUndoAction();
            try
            {
                if (ASContext.CommonSettings.StartWithModifiers)
                    src = FixModifiersLocation(src);

                int len = SnippetHelper.InsertSnippetText(Sci, position + Sci.MBSafeTextLength(Sci.SelText), src);
                UpdateLookupPosition(position, len);
                AddLookupPosition();
            }
            finally { Sci.EndUndoAction(); }
        }

        /// <summary>
        /// Move "visibility" modifier at the beginning of the line
        /// </summary>
        private static string FixModifiersLocation(string src)
        {
            bool needUpdate = false;
            string[] lines = src.Split('\n');
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];

                Match m = reModifiers.Match(line);
                if (m.Success)
                {
                    Group decl = m.Groups[2];
                    Match m2 = reModifier.Match(decl.Value);
                    if (m2.Success)
                    {
                        string repl = m2.Value + decl.Value.Remove(m2.Index, m2.Length);
                        lines[i] = line.Remove(decl.Index, decl.Length).Insert(decl.Index, repl);
                        needUpdate = true;
                    }
                }
            }
            if (needUpdate) return String.Join("\n", lines);
            else return src;
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
            if (lookupPosition >= 0)
            {
                ScintillaControl Sci = ASContext.CurSciControl;
                if (Sci == null) return;
                int lookupLine = Sci.LineFromPosition(lookupPosition);
                int lookupCol = lookupPosition - Sci.PositionFromLine(lookupLine);
                ASContext.Panel.SetLastLookupPosition(ASContext.Context.CurrentFile, lookupLine, lookupCol);
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
        FieldFromPatameter,
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

    class FoundDeclaration
    {
        public MemberModel member;
        public ClassModel inClass;

        public FoundDeclaration()
        {
            member = null;
            inClass = ClassModel.VoidClass;
        }
    }

    class FunctionParameter
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
