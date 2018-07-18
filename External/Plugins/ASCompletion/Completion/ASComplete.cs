using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using ASCompletion.Commands;
using ASCompletion.Context;
using ASCompletion.Model;
using PluginCore;
using PluginCore.Controls;
using PluginCore.Helpers;
using PluginCore.Localization;
using PluginCore.Managers;
using PluginCore.Utilities;
using ScintillaNet;

namespace ASCompletion.Completion
{
    public delegate void ResolvedContextChangeHandler(ResolvedContext resolved);

    /// <summary>
    /// Description of ASComplete.
    /// </summary>
    public class ASComplete
    {

        #region regular_expressions_definitions
        static private readonly RegexOptions ro_csr = ASFileParserRegexOptions.SinglelineComment | RegexOptions.RightToLeft;
        // refine last expression
        static private readonly Regex re_refineExpression = new Regex("[^\\[\\]{}(:,=+*/%!<>-]*$", ro_csr);
        // code cleaning
        static private readonly Regex re_whiteSpace = new Regex("[\\s]+", ASFileParserRegexOptions.SinglelineComment);
        // balanced matching, see: http://blogs.msdn.com/bclteam/archive/2005/03/15/396452.aspx
        static private readonly Regex re_balancedParenthesis = new Regex("\\([^()]*(((?<Open>\\()[^()]*)+((?<Close-Open>\\))[^()]*)+)*(?(Open)(?!))\\)",
                                                                         ASFileParserRegexOptions.SinglelineComment);
        // expressions
        static private readonly Regex re_sub = new Regex("^#(?<index>[0-9]+)~$", ASFileParserRegexOptions.SinglelineComment);
        #endregion

        #region fields
        static public Keys HelpKeys = Keys.F1;

        //stores the currently used class namespace and name
        static private String currentClassHash = null;
        //stores the last completed member for each class
        static private IDictionary<String, String> completionHistory = new Dictionary<String, String>();

        static public ResolvedContext CurrentResolvedContext;
        static public event ResolvedContextChangeHandler OnResolvedContextChanged;

        #endregion

        #region application_event_handlers
        /// <summary>
        /// Character written in editor
        /// </summary>
        /// <param name="Sci">Scintilla Control</param>
        /// <param name="Value">Character inserted</param>
        /// <param name="autoHide">Auto-started completion (is false when pressing Ctrl+Space or Ctrl+Alt+Space)</param>
        /// <returns>Auto-completion has been handled</returns>
        public static bool OnChar(ScintillaControl Sci, int Value, bool autoHide)
        {
            var ctx = ASContext.Context;
            if (!ctx.CodeComplete.IsAvailable(ctx, autoHide)) return false;
            var features = ctx.Features;
            try
            {
                if (Sci.IsSelectionRectangle) return false;
                // code auto
                int eolMode = Sci.EOLMode;
                if (((Value == '\n') && (eolMode != 1)) || ((Value == '\r') && (eolMode == 1)))
                {
                    if (ASContext.HasContext && ctx.IsFileValid) HandleStructureCompletion(Sci);
                    return false;
                }

                int position = Sci.CurrentPos;
                if (position < 2) return false;

                char prevValue = (char) Sci.CharAt(position - 2);
                bool skipQuoteCheck = false;

                Sci.Colourise(0, -1);
                int style = Sci.BaseStyleAt(position - 1);

                if (features.hasStringInterpolation && (IsStringStyle(style) || IsCharStyle(style)))
                {
                    var stringTypeChar = Sci.GetStringType(position - 2); // start from -2 in case the inserted char is ' or "
                    // string interpolation
                    if (features.stringInterpolationQuotes.Contains(stringTypeChar)
                        && IsMatchingQuote(stringTypeChar, Sci.BaseStyleAt(position - 2)))
                    {
                        if (Value == '$' && !ctx.CodeComplete.IsEscapedCharacter(Sci, position - 1, '$'))
                        {
                            return HandleInterpolationCompletion(Sci, autoHide, false);
                        }
                        if (Value == '{' && prevValue == '$' && !ctx.CodeComplete.IsEscapedCharacter(Sci, position - 2, '$'))
                        {
                            if (autoHide) HandleAddClosingBraces(Sci, (char) Value, true);
                            return HandleInterpolationCompletion(Sci, autoHide, true);
                        }
                        if (ctx.CodeComplete.IsStringInterpolationStyle(Sci, position - 2))
                        {
                            skipQuoteCheck = true; // continue on with regular completion
                        }
                    }
                }

                if (!skipQuoteCheck)
                {
                    // ignore text in comments & quoted text
                    if (!IsTextStyle(style) && !IsTextStyle(Sci.BaseStyleAt(position)))
                    {
                        // documentation completion
                        if (ASContext.CommonSettings.SmartTipsEnabled && IsCommentStyle(style))
                        {
                            return ASDocumentation.OnChar(Sci, Value, position, style);
                        }
                        if (autoHide)
                        {
                            // close quotes
                            HandleAddClosingBraces(Sci, (char) Value, true);
                        }
                        return false;
                    }
                }

                // close brace/parens
                if (autoHide) HandleAddClosingBraces(Sci, (char) Value, true);

                // stop here if the class is not valid
                if (!ASContext.HasContext || !ctx.IsFileValid) return false;

                // handle
                switch (Value)
                {
                    case '.':
                        if (features.dot == "." || !autoHide) return HandleDotCompletion(Sci, autoHide);
                        break;

                    case '>':
                        if (features.dot == "->" && prevValue == '-') return HandleDotCompletion(Sci, autoHide);
                        break;

                    case ' ':
                        if (ctx.CodeComplete.HandleWhiteSpaceCompletion(Sci, position, autoHide)) return true;
                        break;

                    case ':':
                        if (ctx.CurrentModel.haXe && prevValue == '@') return HandleMetadataCompletion(autoHide);
                        if (features.hasEcmaTyping) return HandleColonCompletion(Sci, "", autoHide);
                        break;

                    case '<':
                        if (features.hasGenerics && position > 2)
                        {
                            char c0 = (char)Sci.CharAt(position - 2);
                            //TODO: We should check if we are actually on a generic type
                            if ((ctx.CurrentModel.Version == 3 && c0 == '.') || char.IsLetterOrDigit(c0))
                                return HandleColonCompletion(Sci, "", autoHide);
                            return false;
                        }
                        break;

                    case '(':
                    case ',':
                        if (!ASContext.CommonSettings.DisableCallTip) return HandleFunctionCompletion(Sci, autoHide);
                        return false;

                    case ')':
                        if (UITools.CallTip.CallTipActive) UITools.CallTip.Hide();
                        return false;

                    case '*':
                        if (features.hasImportsWildcard) return CodeAutoOnChar(Sci, Value);
                        break;

                    case ';':
                        if (!ASContext.CommonSettings.DisableCodeReformat) ReformatLine(Sci, position);
                        break;

                    default:
                        AutoStartCompletion(Sci, position);
                        break;
                }
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(/*"Completion error",*/ ex);
            }

            // CodeAuto context
            if (!CompletionList.Active) LastExpression = null;
            return false;
        }

        /// <summary>
        /// Returns true if completion is available
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="autoHide">Auto-started completion (is false when pressing Ctrl+Space or Ctrl+Alt+Space)</param>
        /// <returns>true if completion is available</returns>
        protected virtual bool IsAvailable(IASContext ctx, bool autoHide) => ctx.Settings != null && ctx.Settings.CompletionEnabled;

        /// <summary>
        /// Handle shortcuts
        /// </summary>
        /// <param name="keys">Test keys</param>
        /// <returns></returns>
        public static bool OnShortcut(Keys keys, ScintillaControl Sci)
        {
            if (Sci.IsSelectionRectangle) return false;
            // dot complete
            if (keys == (Keys.Control | Keys.Space))
            {
                if (ASContext.HasContext && ASContext.Context.IsFileValid)
                {
                    // try to get completion as if we had just typed the previous char
                    if (OnChar(Sci, Sci.CharAt(Sci.PositionBefore(Sci.CurrentPos)), false)) return true;
                    // force dot completion
                    OnChar(Sci, '.', false);
                    return true;
                }
                return false;
            }
            if (keys == Keys.Back)
            {
                HandleAddClosingBraces(Sci, Sci.CurrentChar, false);
                return false;
            }
            // show calltip
            if (keys == (Keys.Control | Keys.Shift | Keys.Space))
            {
                if (ASContext.HasContext && ASContext.Context.IsFileValid)
                {
                    //HandleFunctionCompletion(Sci);
                    // force function completion
                    OnChar(Sci, '(', false);
                    return true;
                }
                else return false;
            }
            // project types completion
            if (keys == (Keys.Control | Keys.Alt | Keys.Space))
            {
                if (ASContext.HasContext && ASContext.Context.IsFileValid && !ASContext.Context.Settings.LazyClasspathExploration)
                {
                    int position = Sci.CurrentPos-1;
                    string tail = GetWordLeft(Sci, ref position);
                    ContextFeatures features = ASContext.Context.Features;
                    if (tail.IndexOfOrdinal(features.dot) < 0 && features.HasTypePreKey(tail)) tail = "";
                    // display the full project classes list
                    HandleAllClassesCompletion(Sci, tail, false, true);
                    return true;
                }
                else return false;
            }
            // hot build
            if (keys == (Keys.Control | Keys.Enter))
            {
                // project build
                DataEvent de = new DataEvent(EventType.Command, "ProjectManager.HotBuild", null);
                EventManager.DispatchEvent(ASContext.Context, de);
                //
                if (!de.Handled)
                {
                    // quick build
                    if (!ASContext.Context.BuildCMD(true))
                    {
                        // Flash IDE
                        if (PluginBase.CurrentProject == null)
                        {
                            string idePath = ASContext.CommonSettings.PathToFlashIDE;
                            if (idePath != null && File.Exists(Path.Combine(idePath, "Flash.exe")))
                            {
                                string cmd = Path.Combine("Tools", Path.Combine("flashide", "testmovie.jsfl"));
                                cmd = PathHelper.ResolvePath(cmd);
                                if (cmd != null && File.Exists(cmd))
                                    CallFlashIDE.Run(idePath, cmd);
                            }
                        }
                    }
                }
                return true;
            }
            // help
            else if (keys == HelpKeys && ASContext.HasContext && ASContext.Context.IsFileValid)
            {
                ResolveElement(Sci, "ShowDocumentation");
                return true;
            }
            return false;
        }

        /// <summary>
        /// Fire the completion automatically
        /// </summary>
        private static void AutoStartCompletion(ScintillaControl sci, int position)
        {
            if (!CompletionList.Active && ASContext.Context.Features.hasEcmaTyping 
                && ASContext.CommonSettings.AlwaysCompleteWordLength > 0)
            {
                // fire completion if starting to write a word
                bool valid = true;
                int n = ASContext.CommonSettings.AlwaysCompleteWordLength;
                int wordStart = sci.WordStartPosition(position, true);
                if (position - wordStart != n) return;
                char c = (char)sci.CharAt(wordStart);
                string characterClass = ScintillaControl.Configuration.GetLanguage(sci.ConfigurationLanguage).characterclass.Characters;
                if (char.IsDigit(c) || !characterClass.Contains(c)) return;
                // give a guess to the context (do not complete where it should not)
                if (valid)
                {
                    int pos = wordStart - 1;
                    bool hadWS = false;
                    bool canComplete = false;
                    while (pos > 0)
                    {
                        c = (char)sci.CharAt(pos--);
                        if (hadWS && characterClass.IndexOf(c) >= 0) break;
                        if (c == '<' && ((char)sci.CharAt(pos + 2) == '/' || !hadWS)) break;
                        if (":;,+-*%!&|<>/{}()[=?".Contains(c))
                        {
                            canComplete = true;
                            // TODO  Add HTML lookup here
                            if (pos > 0)
                            {
                                if (c == '/' && (char)sci.CharAt(pos) == '<') canComplete = false;
                            }
                            break;
                        }
                        if (c <= 32)
                        {
                            if (c == '\r' || c == '\n')
                            {
                                canComplete = true;
                                break;
                            }
                            if (pos > 1)
                            {
                                int style = sci.BaseStyleAt(pos - 1);
                                if (style == 19)
                                {
                                    canComplete = true;
                                    break;
                                }
                            }
                            hadWS = true;
                        }
                        else if (c != '.' && !characterClass.Contains(c))
                        {
                            // TODO support custom DOT
                            canComplete = false;
                            break;
                        }
                    }
                    if (canComplete) HandleDotCompletion(sci, true);
                }
            }
        }
        #endregion

        #region add_closing_braces

        public static void HandleAddClosingBraces(ScintillaControl sci, char c, bool addedChar)
        {
            if (!ASContext.CommonSettings.AddClosingBraces) return;
            var context = ASContext.Context;
            if (addedChar)
            {
                if (IsMatchingQuote(c, sci.BaseStyleAt(sci.CurrentPos - 2)) && context.CodeComplete.IsEscapedCharacter(sci, sci.CurrentPos - 1))
                {
                    return;
                }

                bool undo = false;
                byte styleBefore;
                byte styleAfter;

                sci.BeginUndoAction();

                if (c == '"' || c == '\'')
                {
                    // Get the before & after style values unaffected by the entered char
                    sci.DeleteBack();
                    sci.Colourise(0, -1);
                    styleBefore = (byte) sci.BaseStyleAt(sci.CurrentPos - 1);
                    styleAfter = (byte) sci.BaseStyleAt(sci.CurrentPos);
                    sci.AddText(1, c.ToString());
                    undo = true;
                }
                else
                {
                    styleBefore = (byte) sci.BaseStyleAt(sci.CurrentPos - 2);
                    styleAfter = (byte) sci.BaseStyleAt(sci.CurrentPos);
                }

                // not inside a string literal
                int position = sci.CurrentPos - 1;
                if (!(IsStringStyle(styleBefore) && IsStringStyle(styleAfter)) && !(IsCharStyle(styleBefore) && IsCharStyle(styleAfter))
                    || context.CodeComplete.IsStringInterpolationStyle(sci, position))
                {
                    char nextChar = sci.CurrentChar;
                    int nextPos = sci.CurrentPos;

                    while (nextChar == ' ' || nextChar == '\t') // Don't skip new line characters
                    {
                        nextPos++;
                        nextChar = (char) sci.CharAt(nextPos);
                    }

                    foreach (var brace in ASContext.CommonSettings.AddClosingBracesRules)
                    {
                        // Handle opening first for braces that have equal opening & closing chars
                        if (HandleBraceOpen(sci, brace, c, styleAfter, styleBefore)
                            || HandleBraceClose(sci, brace, c, nextChar, nextPos))
                        {
                            undo = false;
                            break;
                        }
                    }
                }
                else if (IsMatchingQuote(c, styleAfter))
                {
                    char nextChar = sci.CurrentChar;
                    int nextPos = sci.CurrentPos;

                    while (nextChar == ' ' || nextChar == '\t') // Don't skip new line characters
                    {
                        nextPos++;
                        nextChar = (char) sci.CharAt(nextPos);
                    }

                    foreach (var brace in ASContext.CommonSettings.AddClosingBracesRules)
                    {
                        if (HandleBraceClose(sci, brace, c, nextChar, nextPos))
                        {
                            undo = false;
                            break;
                        }
                    }
                }

                sci.EndUndoAction();
                if (undo) sci.Undo();
            }
            else
            {
                char open = (char) sci.CharAt(sci.CurrentPos - 1);

                if (IsMatchingQuote(open, sci.BaseStyleAt(sci.CurrentPos - 2)) && context.CodeComplete.IsEscapedCharacter(sci, sci.CurrentPos - 1))
                {
                    return;
                }

                int styleBefore = sci.BaseStyleAt(sci.CurrentPos - 2);
                int styleAfter = sci.BaseStyleAt(sci.CurrentPos);

                // not inside a string literal
                int position = sci.CurrentPos - 1;
                if (!(IsStringStyle(styleBefore) && IsStringStyle(styleAfter)) && !(IsCharStyle(styleBefore) && IsCharStyle(styleAfter))
                    || context.CodeComplete.IsStringInterpolationStyle(sci, position)
                    || IsMatchingQuote(open, styleAfter))
                {
                    int closePos = sci.CurrentPos;

                    while (char.IsWhiteSpace(c))
                    {
                        closePos++;
                        c = (char) sci.CharAt(closePos);
                    }

                    foreach (var brace in ASContext.CommonSettings.AddClosingBracesRules)
                    {
                        if (HandleBraceRemove(sci, brace, open, c, closePos))
                        {
                            break;
                        }
                    }
                }
            }
        }

        private static bool HandleBraceOpen(ScintillaControl sci, Brace brace, char open, byte styleAfter, byte styleBefore)
        {
            if (open == brace.Open)
            {
                char charAfter = (char) sci.CharAt(sci.CurrentPos);
                char charBefore = (char) sci.CharAt(sci.CurrentPos - 2);

                if (brace.ShouldOpen(charBefore, styleBefore, charAfter, styleAfter))
                {
                    sci.InsertText(-1, brace.Close.ToString());
                    if (brace.AddSpace)
                    {
                        sci.AddText(1, " ");
                    }
                    return true;
                }
            }

            return false;
        }

        private static bool HandleBraceClose(ScintillaControl sci, Brace brace, char close, char next, int nextPosition)
        {
            if (close == brace.Close && next == brace.Close)
            {
                if (brace.ShouldClose(sci.CurrentPos, nextPosition))
                {
                    sci.DeleteBack();
                    sci.AnchorPosition = nextPosition;
                    sci.CurrentPos = nextPosition;
                    return true;
                }
            }

            return false;
        }
        
        private static bool HandleBraceRemove(ScintillaControl sci, Brace brace, char open, char close, int closePosition)
        {
            if (open == brace.Open && close == brace.Close)
            {
                if (brace.ShouldRemove(sci.CurrentPos, closePosition))
                {
                    sci.SelectionEnd = closePosition + 1;
                    sci.DeleteBack();
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region plugin commands
        /// <summary>
        /// Using the text under at cursor position, search and open the object/class/member declaration
        /// </summary>
        /// <param name="sci">Control</param>
        /// <returns>Declaration was found</returns>
        public static bool DeclarationLookup(ScintillaControl sci)
        {
            if (!ASContext.Context.IsFileValid || (sci == null)) return false;

            // let the context handle goto declaration if we couldn't find anything
            if (!InternalDeclarationLookup(sci))
            {
                ASExpr expression = GetExpression(sci, sci.CurrentPos);
                if (expression != null)
                {
                    return ASContext.Context.HandleGotoDeclaration(sci, expression);
                }
            }
            return true;
        }

        public static bool TypeDeclarationLookup(ScintillaControl sci)
        {
            if (sci == null || !ASContext.Context.IsFileValid) return false;
            var position = ExpressionEndPosition(sci, sci.CurrentPos);
            var result = GetExpressionType(sci, position, false, true);
            if (result.IsPackage) return false;
            var member = result.Member;
            var type = result.Type;
            if (member == null || member.Flags.HasFlag(FlagType.AutomaticVar) || type == null) return false;
            if (member.Flags.HasFlag(FlagType.Function))
            {
                type = ASContext.Context.ResolveType(result.Member.Type, result.InFile);
                if (type.IsVoid()) return false;
            }
            result.Member = null;
            result.InClass = null;
            result.InFile = null;
            var path = type.Name;
            result.Path = path.Contains(".") ? path.Substring(0, path.IndexOfOrdinal(".")) : path;
            return OpenDocumentToDeclaration(sci, result);
        }

        private static bool InternalDeclarationLookup(ScintillaControl sci)
        {
            // get type at cursor position
            var position = ExpressionEndPosition(sci, sci.CurrentPos);
            var result = GetExpressionType(sci, position, false, true);

            var ctx = ASContext.Context;
            // browse to package folder
            if (result.IsPackage && result.InFile != null) return ctx.BrowseTo(result.InFile.Package);

            // open source and show declaration
            if (!result.IsNull())
            {
                if (result.Member != null && (result.Member.Flags & FlagType.AutomaticVar) > 0)
                    return false;

                // open the file
                return OpenDocumentToDeclaration(sci, result);
            }
            // show overridden method
            if (ctx.CurrentMember != null
                && ctx.Features.overrideKey != null
                && sci.GetWordFromPosition(position) == ctx.Features.overrideKey)
            {
                MemberModel member = ctx.CurrentMember;
                if ((member.Flags & FlagType.Override) > 0)
                {
                    ClassModel tmpClass = ctx.CurrentClass;
                    if (tmpClass != null)
                    {
                        tmpClass.ResolveExtends();
                        tmpClass = tmpClass.Extends;
                        while (tmpClass != null && !tmpClass.IsVoid())
                        {
                            MemberModel found = tmpClass.Members.Search(member.Name, 0, 0);
                            if (found != null)
                            {
                                result = new ASResult();
                                result.Member = found;
                                result.InFile = tmpClass.InFile;
                                result.InClass = tmpClass;
                                OpenDocumentToDeclaration(sci, result);
                                break;
                            }
                            tmpClass = tmpClass.Extends;
                        }
                    }
                }
            }
            return false;
        }

        public static void SaveLastLookupPosition(ScintillaControl Sci)
        {
            if (Sci != null)
            {
                int lookupLine = Sci.CurrentLine;
                int lookupCol = Sci.CurrentPos - Sci.PositionFromLine(lookupLine);
                ASContext.Panel.SetLastLookupPosition(ASContext.Context.CurrentFile, lookupLine, lookupCol);
            }
        }

        /// <summary>
        /// Show resolved element declaration
        /// </summary>
        /// <param name="sci">Scintilla control</param>
        /// <param name="result">Element declaration</param>
        public static bool OpenDocumentToDeclaration(ScintillaControl sci, ASResult result)
        {
            var model = result.InFile ?? result.Member?.InFile ?? result.Type?.InFile;
            if (model == null || model.FileName == "") return false;
            var inClass = result.InClass ?? result.Type;

            SaveLastLookupPosition(sci);

            if (model != ASContext.Context.CurrentModel)
            {
                if (model.FileName.Length > 0 && File.Exists(model.FileName))
                    ASContext.MainForm.OpenEditableDocument(model.FileName, false);
                else
                {
                    OpenVirtualFile(model);
                    result.InFile = ASContext.Context.CurrentModel;
                    if (result.InFile == null) return false;
                    if (inClass != null)
                    {
                        inClass = result.InFile.GetClassByName(inClass.Name);
                        if (result.Member != null)
                            result.Member = inClass.Members.Search(result.Member.Name, 0, 0);
                    }
                    else if (result.Member != null)
                        result.Member = result.InFile.Members.Search(result.Member.Name, 0, 0);
                }
            }
            if ((inClass == null || inClass.IsVoid()) && result.Member == null) return false;
            if (ASContext.CurSciControl == null) return false;

            int line = 0;
            string name = null;
            bool isClass = false;
            // member
            if (result.Member != null && result.Member.LineFrom > 0)
            {
                line = result.Member.LineFrom;
                name = result.Member.Name;
            }
            // class declaration
            else if (inClass != null && inClass.LineFrom > 0)
            {
                line = inClass.LineFrom;
                name = inClass.Name;
                isClass = true;
                // constructor
                foreach (MemberModel member in inClass.Members)
                    if ((member.Flags & FlagType.Constructor) > 0)
                    {
                        line = member.LineFrom;
                        name = member.Name;
                        isClass = false;
                        break;
                    }
            }
            // select
            if (line > 0)
            {
                if (isClass)
                    LocateMember("(class|interface|abstract)", name, line);
                else
                    LocateMember("(function|var|const|get|set|property|namespace|[,(])", name, line);
            }
            return true;
        }

        static public void OpenVirtualFile(FileModel model)
        {
            string ext = Path.GetExtension(model.FileName);
            if (ext == "") ext = model.Context.GetExplorerMask()[0].Replace("*", "");
            string dummyFile = Path.Combine(Path.GetDirectoryName(model.FileName), "[model] " + Path.GetFileNameWithoutExtension(model.FileName) + ext);
            foreach (ITabbedDocument doc in ASContext.MainForm.Documents)
            {
                if (doc.FileName == dummyFile)
                {
                    doc.Activate();
                    return;
                }
            }
            // nice output
            model.Members.Sort();
            foreach (ClassModel aClass in model.Classes) aClass.Members.Sort();
            string src = "//\n// " + model.FileName + "\n//\n" + model.GenerateIntrinsic(false);
            ITabbedDocument temp = ASContext.MainForm.CreateEditableDocument(dummyFile, src, Encoding.UTF8.CodePage) as ITabbedDocument;
            if (temp != null && temp.IsEditable) 
            {
                // The model document will be read only
                temp.SciControl.IsReadOnly = true;
            }
        }

        public static void LocateMember(string keyword, string name, int line)
        {
            LocateMember(PluginBase.MainForm.CurrentDocument.SciControl, keyword, name, line);
        }

        public static void LocateMember(ScintillaControl sci, string keyword, string name, int line)
        {
            if (sci == null || line <= 0) return;
            try
            {
                bool found = false;
                string pattern = String.Format("{0}\\s*(?<name>{1})[^A-z0-9]", (keyword ?? ""), name.Replace(".", "\\s*.\\s*"));
                Regex re = new Regex(pattern);
                for (int i = line; i < line + 2; i++)
                    if (i < sci.LineCount)
                    {
                        string text = sci.GetLine(i);
                        Match m = re.Match(text);
                        if (m.Success)
                        {
                            int position = sci.PositionFromLine(i) + sci.MBSafeTextLength(text.Substring(0, m.Groups["name"].Index));
                            sci.EnsureVisibleEnforcePolicy(sci.LineFromPosition(position));
                            sci.SetSel(position, position + m.Groups["name"].Length);
                            found = true;
                            break;
                        }
                    }

                if (!found)
                {
                    sci.EnsureVisible(line);
                    int linePos = sci.PositionFromLine(line);
                    sci.SetSel(linePos, linePos);
                }
                sci.Focus();
            }
            catch { }
        }

        /// <summary>
        /// Resolve word at cursor position and pre-fill arguments for args processor
        /// </summary>
        internal static void ResolveContext(ScintillaControl Sci)
        {
            try
            {
                // check if a document
                if (Sci == null)
                {
                    ClearResolvedContext();
                    return;
                }

                // check if resolution is needed
                int position = Sci.WordEndPosition(Sci.CurrentPos, true);
                if (CurrentResolvedContext != null && CurrentResolvedContext.Position == position
                    && CurrentResolvedContext.Result != null && !CurrentResolvedContext.Result.IsNull())
                    return;

                // check context
                IASContext context = ASContext.Context;
                if (context == null || context.CurrentModel == null)
                {
                    ClearResolvedContext();
                    return;
                }
                CurrentResolvedContext = new ResolvedContext();
                CurrentResolvedContext.Position = position;

                // get type at cursor position
                ASResult result;
                if (context.IsFileValid) result = GetExpressionType(Sci, position);
                else result = new ASResult();
                CurrentResolvedContext.Result = result;
                ContextFeatures features = context.Features;

                Hashtable args = CurrentResolvedContext.Arguments;
                string package = context.CurrentModel.Package;
                args.Add("TypPkg", package);

                ClassModel cClass = context.CurrentClass ?? ClassModel.VoidClass;
                args.Add("TypName", MemberModel.FormatType(cClass.Name));
                string fullname = MemberModel.FormatType(cClass.QualifiedName);
                args.Add("TypPkgName", fullname);
                FlagType flags = cClass.Flags;
                string kind = GetKind(flags, features);
                args.Add("TypKind", kind);

                if (context.CurrentMember != null)
                {
                    args.Add("MbrName", context.CurrentMember.Name);
                    flags = context.CurrentMember.Flags;
                    kind = GetKind(flags, features);
                    args.Add("MbrKind", kind);

                    ClassModel aType = CurrentResolvedContext.TokenType
                        = context.ResolveType(context.CurrentMember.Type, context.CurrentModel);
                    package = aType.IsVoid() ? "" : aType.InFile.Package;
                    args.Add("MbrTypPkg", package);
                    args.Add("MbrTypName", MemberModel.FormatType(aType.Name));
                    fullname = MemberModel.FormatType(aType.QualifiedName);
                    args.Add("MbrTypePkgName", fullname);
                    flags = aType.Flags;
                    kind = GetKind(flags, features);
                    args.Add("MbrTypKind", kind);
                }
                else
                {
                    args.Add("MbrName", "");
                    args.Add("MbrKind", "");
                    args.Add("MbrTypPkg", "");
                    args.Add("MbrTypName", "");
                    args.Add("MbrTypePkgName", "");
                    args.Add("MbrTypKind", "");
                    args.Add("TypClosestListName", "");
                    args.Add("TypClosestListItemType", "");
                    args.Add("ItmUniqueVar", "");
                }

                // if element can be resolved
                if (result.IsPackage)
                {
                    args.Add("ItmFile", result.InFile.FileName);
                    args.Add("ItmTypPkg", result.Path);
                    args.Add("ItmTypPkgName", result.Path);
                }
                else if (result.Type != null || result.Member != null)
                {
                    ClassModel oClass = result.InClass ?? result.Type;

                    if (oClass.IsVoid() && (result.Member == null || (result.Member.Flags & FlagType.Function) == 0 && (result.Member.Flags & FlagType.Namespace) == 0))
                    {
                        NotifyContextChanged();
                        return;
                    }

                    // type details
                    FileModel file;
                    MemberModel member = result.Member;

                    if (member != null && member.IsPackageLevel)
                    {
                        args.Add("ItmTypName", member.Name);
                        file = member.InFile;
                        fullname = "package";
                        flags = member.Flags;
                    }
                    else
                    {
                        args.Add("ItmTypName", MemberModel.FormatType(oClass.Name));
                        file = oClass.InFile;
                        fullname = MemberModel.FormatType(oClass.Name);
                        flags = oClass.Flags;
                    }
                    package = file.Package;
                    fullname = (package.Length > 0 ? package + "." : "") + fullname;
                    kind = GetKind(flags, features);

                    args.Add("ItmFile", file.FileName);
                    args.Add("ItmTypPkg", package);
                    args.Add("ItmTypPkgName", fullname);
                    args.Add("ItmTypKind", kind);
                    // type as path
                    args.Add("ItmTypPkgNamePath", fullname.Replace('.', '\\'));
                    args.Add("ItmTypPkgNameURL", fullname.Replace('.', '/'));

                    if (result.Type != null)
                    {
                        package = result.Type.InFile.Package;
                        args.Add("ItmClassName", MemberModel.FormatType(result.Type.Name));
                        args.Add("ItmClassPkg", package);
                        args.Add("ItmClassPkgName", MemberModel.FormatType(result.Type.QualifiedName));
                    }
                    else
                    {
                        args.Add("ItmClassName", "");
                        args.Add("ItmClassPkg", "");
                        args.Add("ItmClassPkgName", "");
                    }

                    // element details
                    if (result.Type != null && member != null)
                    {
                        args.Add("ItmName", member.Name);
                        flags = member.Flags & ~(FlagType.LocalVar | FlagType.Dynamic | FlagType.Static);
                        kind = GetKind(flags, features);
                        args.Add("ItmKind", kind);
                        args.Add("ItmNameDocs", member.Name + (kind == features.functionKey ? "()" : ""));
                    }
                    else
                    {
                        args.Add("ItmName", MemberModel.FormatType(oClass.Name));
                        flags = oClass.Flags;
                        kind = GetKind(flags, features);
                        args.Add("ItmKind", kind);
                        args.Add("ItmNameDocs", "");
                    }
                }
                else
                {
                    args.Add("ItmFile", "");
                    args.Add("ItmName", "");
                    args.Add("ItmKind", "");
                    args.Add("ItmTypName", "");
                    args.Add("ItmTypPkg", "");
                    args.Add("ItmTypPkgName", "");
                    args.Add("ItmTypKind", "");
                    args.Add("ItmTypPkgNamePath", "");
                    args.Add("ItmTypPkgNameURL", "");
                    args.Add("ItmClassName", "");
                    args.Add("ItmClassPkg", "");
                    args.Add("ItmClassPkgName", "");
                }
                NotifyContextChanged();
            }
            catch 
            {
            }
        }

        private static void ClearResolvedContext()
        {
            if (CurrentResolvedContext != null && CurrentResolvedContext.Position == -1)
                return;
            CurrentResolvedContext = new ResolvedContext();
            NotifyContextChanged();
        }

        private static void NotifyContextChanged()
        {
            if (OnResolvedContextChanged != null)
                OnResolvedContextChanged.Invoke(CurrentResolvedContext);
        }

        /// <summary>
        /// Using the text under at cursor position, resolve the member/type and call the specified command.
        /// </summary>
        /// <param name="Sci">Control</param>
        /// <returns>Resolved element details</returns>
        static public Hashtable ResolveElement(ScintillaControl Sci, string eventAction)
        {
            if (CurrentResolvedContext == null) ResolveContext(Sci);

            if (eventAction != null && !CurrentResolvedContext.Result.IsNull())
            {
                // other plugins may handle the request
                DataEvent de = new DataEvent(EventType.Command, eventAction, CurrentResolvedContext.Arguments);
                EventManager.DispatchEvent(ASContext.Context, de);
                if (de.Handled) return CurrentResolvedContext.Arguments;

                // help
                if (eventAction == "ShowDocumentation")
                {
                    string cmd = ASContext.Context.Settings.DocumentationCommandLine;
                    if (string.IsNullOrEmpty(cmd)) return null;
                    // top-level vars should be searched only if the command includes member information
                    if (CurrentResolvedContext.Result.InClass == ClassModel.VoidClass && cmd.IndexOfOrdinal("$(Itm") < 0) 
                        return null;
                    // complete command
                    cmd = ArgumentsProcessor.Process(cmd, CurrentResolvedContext.Arguments);
                    // call the command
                    try
                    {
                        ASContext.MainForm.CallCommand("RunProcess", cmd);
                    }
                    catch (Exception ex)
                    {
                        ErrorManager.ShowError(ex);
                    }
                }
            }
            return CurrentResolvedContext.Arguments;
        }

        public static void FindClosestList(IASContext context, ASExpr expr, int lineNum, ref string closestListName, ref string closestListItemType)
        {
            if (expr?.LocalVars == null) return;
            MemberModel closestList = null;
            foreach (MemberModel m in expr.LocalVars)
            {
                if (m.LineFrom > lineNum)
                    continue;
                if (closestList != null && m.LineFrom <= closestList.LineFrom)
                    continue;

                ClassModel aType2 = ASContext.Context.ResolveType(m.Type, context.CurrentModel);
                string objType = ASContext.Context.Features.objectKey;
                while (!aType2.IsVoid() && aType2.QualifiedName != objType)
                {
                    if (aType2.IndexType != null)
                    {
                        closestList = m;
                        closestListItemType = aType2.IndexType;
                        break;
                    }
                    aType2 = aType2.Extends;
                }
            }
            if (closestList != null) closestListName = closestList.Name;
        }

        public static string FindFreeIterator(IASContext context, ClassModel cClass, ASExpr expr)
        {
            int iteratorCount = 105;
            string iterator = ((char)iteratorCount).ToString();
            MemberList members = cClass.Members;
            List<MemberModel> parameters = context.CurrentMember.Parameters;
            while (true)
            {
                var restartCycle = false;
                if (expr?.LocalVars != null)
                    foreach (MemberModel m in expr.LocalVars)
                    {
                        if (m.Name == iterator)
                        {
                            iteratorCount++;
                            iterator = ((char)iteratorCount).ToString();
                            restartCycle = true;
                            break;
                        }
                    }
                if (members != null && !restartCycle)
                {
                    foreach (MemberModel m in members)
                    {
                        if (m.Name == iterator)
                        {
                            iteratorCount++;
                            iterator = ((char)iteratorCount).ToString();
                            restartCycle = true;
                            break;
                        }
                    }
                }
                if (parameters != null && !restartCycle)
                {
                    foreach (MemberModel m in parameters)
                    {
                        if (m.Name == iterator)
                        {
                            iteratorCount++;
                            iterator = ((char)iteratorCount).ToString();
                            restartCycle = true;
                            break;
                        }
                    }
                }
                if (!restartCycle)
                    break;
            }
            return iterator;
        }

        private static string GetKind(FlagType flags, ContextFeatures features)
        {
            if (flags == FlagType.Function) return features.functionKey;
            if ((flags & FlagType.Constant) > 0) return features.constKey;
            if ((flags & (FlagType.Getter | FlagType.Setter)) > 0) return features.varKey;
            if ((flags & FlagType.Interface) > 0) return "interface";
            if ((flags & FlagType.Class) > 0) return "class";
            return "";
        }
        #endregion

        #region structure_completion
        static private void HandleStructureCompletion(ScintillaControl Sci)
        {
            try
            {
                int position = Sci.CurrentPos;
                int line = Sci.LineFromPosition(position);
                if (line == 0)
                    return;
                string txt = Sci.GetLine(line - 1).TrimEnd();
                int style = Sci.BaseStyleAt(position);

                // move closing brace to its own line and fix indentation
                if (Sci.CurrentChar == '}')
                {
                    var openingBrace = Sci.SafeBraceMatch(position);
                    var openLine = openingBrace >= 0 ? Sci.LineFromPosition(openingBrace) : line - 1;
                    Sci.InsertText(Sci.CurrentPos, LineEndDetector.GetNewLineMarker(Sci.EOLMode));
                    Sci.SetLineIndentation(line + 1, Sci.GetLineIndentation(openLine));
                }
                // in comments
                else if (PluginBase.Settings.CommentBlockStyle == CommentBlockStyle.Indented && txt.EndsWithOrdinal("*/"))
                    FixIndentationAfterComments(Sci, line);
                else if (IsCommentStyle(style) && (Sci.BaseStyleAt(position + 1) == style))
                    FormatComments(Sci, txt, line);
                // in code
                else
                {
                    // braces
                    if (!ASContext.CommonSettings.DisableAutoCloseBraces)
                    {
                        if (txt.IndexOfOrdinal("//") > 0) // remove comment at end of line
                        {
                            int slashes = Sci.MBSafeTextLength(txt.Substring(0, txt.IndexOfOrdinal("//") + 1));
                            if (Sci.PositionIsOnComment(Sci.PositionFromLine(line-1) + slashes))
                                txt = txt.Substring(0, txt.IndexOfOrdinal("//")).Trim();
                        }
                        if (txt.EndsWith('{') && (line > 1)) AutoCloseBrace(Sci, line);
                    }
                    // code reformatting
                    if (!ASContext.CommonSettings.DisableCodeReformat && !txt.EndsWithOrdinal("*/"))
                        ReformatLine(Sci, Sci.PositionFromLine(line) - 1);
                }
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        private static void ReformatLine(ScintillaControl sci, int position)
        {
            int line = sci.LineFromPosition(position);
            string txt = sci.GetLine(line).TrimEnd('\r', '\n');
            int curPos = sci.CurrentPos;
            int startPos = sci.PositionFromLine(line);
            int offset = sci.MBSafeLengthFromBytes(txt, position - startPos);
            
            ReformatOptions options = new ReformatOptions();
            options.Newline = LineEndDetector.GetNewLineMarker(sci.EOLMode);
            options.CondenseWhitespace = ASContext.CommonSettings.CondenseWhitespace;
            options.BraceAfterLine = ASContext.CommonSettings.ReformatBraces 
                && PluginBase.MainForm.Settings.CodingStyle == CodingStyle.BracesAfterLine;
            options.CompactChars = ASContext.CommonSettings.CompactChars;
            options.SpacedChars = ASContext.CommonSettings.SpacedChars;
            options.SpaceBeforeFunctionCall = ASContext.CommonSettings.SpaceBeforeFunctionCall;
            options.AddSpaceAfter = ASContext.CommonSettings.AddSpaceAfter.Split(' ');
            options.IsPhp = ASContext.Context.Settings.LanguageId == "PHP";
            options.IsHaXe = ASContext.Context.Settings.LanguageId == "HAXE";

            if (options.IsHaXe)
            {
                var initialStyle = sci.BaseStyleAt(startPos);
                if (initialStyle == 6) options.InString = 1;
                else if (initialStyle == 7) options.InString = 2;
            }

            int newOffset = offset;
            string replace = Reformater.ReformatLine(txt, options, ref newOffset);

            if (replace != txt)
            {
                position = curPos + newOffset - offset;
                sci.SetSel(startPos, startPos + sci.MBSafeTextLength(txt));
                sci.ReplaceSel(replace);
                sci.SetSel(position, position);
            }
        }

        /// <summary>
        /// Add closing brace to a code block.
        /// If enabled, move the starting brace to a new line.
        /// </summary>
        /// <param name="Sci"></param>
        /// <param name="txt"></param>
        /// <param name="line"></param>
        private static void AutoCloseBrace(ScintillaControl Sci, int line)
        {
            // find matching brace
            int bracePos = Sci.LineEndPosition(line - 1) - 1;
            while ((bracePos > 0) && (Sci.CharAt(bracePos) != '{')) bracePos--;
            if (bracePos == 0 || Sci.BaseStyleAt(bracePos) != 10) return;
            int match = Sci.SafeBraceMatch(bracePos);
            int start = line;
            int indent = Sci.GetLineIndentation(start - 1);
            if (match > 0)
            {
                int endIndent = Sci.GetLineIndentation(Sci.LineFromPosition(match));
                if (endIndent + Sci.TabWidth > indent)
                    return;
            }

            // find where to include the closing brace
            int startIndent = indent;
            int count = Sci.LineCount;
            int lastLine = line;
            int position;
            string txt = Sci.GetLine(line).Trim();
            line++;
            int eolMode = Sci.EOLMode;
            string NL = LineEndDetector.GetNewLineMarker(eolMode);

            if (txt.Length > 0 && ")]};,".IndexOf(txt[0]) >= 0)
            {
                Sci.BeginUndoAction();
                try
                {
                    position = Sci.CurrentPos;
                    Sci.InsertText(position, NL + "}");
                    Sci.SetLineIndentation(line, startIndent);
                }
                finally
                {
                    Sci.EndUndoAction();
                }
                return;
            }
            else 
            {
                while (line < count - 1)
                {
                    txt = Sci.GetLine(line).TrimEnd();
                    if (txt.Length != 0)
                    {
                        indent = Sci.GetLineIndentation(line);
                        if (indent <= startIndent) break;
                        lastLine = line;
                    }
                    else break;
                    line++;
                }
            }
            if (line >= count - 1) lastLine = start;

            // insert closing brace
            Sci.BeginUndoAction();
            try
            {
                position = Sci.LineEndPosition(lastLine);
                Sci.InsertText(position, NL + "}");
                Sci.SetLineIndentation(lastLine + 1, startIndent);
            }
            finally
            {
                Sci.EndUndoAction();
            }
        }

        /// <summary>
        /// When javadoc comment blocks have and additional space, 
        /// fix indentation of new line following this block
        /// </summary>
        /// <param name="Sci"></param>
        /// <param name="line"></param>
        private static void FixIndentationAfterComments(ScintillaControl Sci, int line)
        {
            int startLine = line - 1;
            while (startLine > 0)
            {
                string txt = Sci.GetLine(startLine).TrimStart();
                if (txt.StartsWithOrdinal("/*")) break;
                else if (!txt.StartsWith('*')) break;
                startLine--;
            }
            Sci.SetLineIndentation(line, Sci.GetLineIndentation(startLine));
            int position = Sci.LineIndentPosition(line);
            Sci.SetSel(position, position);
        }

        /// <summary>
        /// Add a '*' at the beginning of new lines inside a comment block
        /// </summary>
        /// <param name="Sci"></param>
        /// <param name="txt"></param>
        /// <param name="line"></param>
        private static void FormatComments(ScintillaControl Sci, string txt, int line)
        {
            txt = txt.TrimStart();
            if (txt.StartsWithOrdinal("/*"))
            {
                Sci.ReplaceSel("* ");
                if (PluginBase.Settings.CommentBlockStyle == CommentBlockStyle.Indented)
                    Sci.SetLineIndentation(line, Sci.GetLineIndentation(line) + 1);
                int position = Sci.LineIndentPosition(line) + 2;
                Sci.SetSel(position, position);
            }
            else if (txt.StartsWith('*'))
            {
                Sci.ReplaceSel("* ");
                int position = Sci.LineIndentPosition(line) + 2;
                Sci.SetSel(position, position);
            }
        }
        #endregion

        #region template_completion
        static private bool HandleDeclarationCompletion(ScintillaControl Sci, string tail, bool autoHide)
        {
            int position = Sci.CurrentPos;
            int line = Sci.LineFromPosition(position);
            if (Sci.CharAt(position - 1) <= 32) tail = "";

            // completion support
            IASContext ctx = ASContext.Context;
            ContextFeatures features = ctx.Features;
            bool insideClass = !ctx.CurrentClass.IsVoid() && ctx.CurrentClass.LineFrom < line;
            List<string> support = features.GetDeclarationKeywords(Sci.GetLine(line), insideClass);
            if (support.Count == 0) return true;
            
            // does it need indentation?
            int tab = 0;
            int tempLine = line-1;
            while (tempLine > 0)
            {
                var tempText = Sci.GetLine(tempLine).Trim();
                if (insideClass && CodeUtils.IsTypeDecl(tempText, features.typesKeywords))
                {
                    tab = Sci.GetLineIndentation(tempLine) + Sci.TabWidth;
                    break;
                }
                if (tempText.Length > 0 && (tempText.EndsWith("}") || CodeUtils.IsDeclaration(tempText, features)))
                {
                    tab = Sci.GetLineIndentation(tempLine);
                    if (tempText.EndsWith('{')) tab += Sci.TabWidth;
                    break;
                }
                tempLine--;
            }
            if (tab > 0)
            {
                Sci.SetLineIndentation(line, tab);
            }

            // build list
            List<ICompletionListItem> known = new List<ICompletionListItem>();
            foreach(string token in support)
                known.Add(new DeclarationItem(token));

            // show
            CompletionList.Show(known, autoHide, tail);
            return true;
        }

        #endregion

        #region function_completion
        static private string calltipDef;
        static private MemberModel calltipMember;
        static private bool calltipDetails;
        static private int calltipPos = -1;
        static private int calltipOffset;
        static private ClassModel calltipRelClass;
        static private string prevParam = "";

        public static bool HasCalltip()
        {
            return UITools.CallTip.CallTipActive && (calltipDef != null);
        }

        /// <summary>
        /// Show highlighted calltip
        /// </summary>
        /// <param name="sci">Scintilla control</param>
        /// <param name="paramIndex">Highlight param number</param>
        private static void ShowCalltip(ScintillaControl sci, int paramIndex)
        {
            // measure highlighting
            int start = calltipDef.IndexOf('(');
            while ((start >= 0) && (paramIndex-- > 0))
                start = FindNearSymbolInFunctDef(calltipDef, ',', start + 1);

            int end = FindNearSymbolInFunctDef(calltipDef, ',', start + 1);
            if (end < 0)
                end = FindNearSymbolInFunctDef(calltipDef, ')', start + 1);

            // get parameter name
            string paramName = "";
            if (calltipMember.Comments != null && start >= 0 && end > 0)
            {
                paramName = calltipDef.Substring(start + 1, end - start - 1);

                int p = paramName.IndexOf(':');
                if (p > 0) paramName = paramName.Substring(0, p);
                else
                {
                    p = paramName.IndexOf('=');
                    if (p > 0) paramName = paramName.Substring(0, p);
                }
                char[] toClean = new char[] { ' ', '\t', '\n', '\r', '*', '?' };
                paramName = paramName.Trim(toClean);
            }
            
            // show calltip
            if (!UITools.CallTip.CallTipActive || UITools.Manager.ShowDetails != calltipDetails || paramName != prevParam)
            {
                prevParam = paramName;
                calltipDetails = UITools.Manager.ShowDetails;
                string text = calltipDef + ASDocumentation.GetTipDetails(calltipMember, paramName);
                UITools.CallTip.CallTipShow(sci, calltipPos - calltipOffset, text, false);
            }

            // highlight
            if ((start < 0) || (end < 0)) UITools.CallTip.CallTipSetHlt(0, 0, true);
            else UITools.CallTip.CallTipSetHlt(start + 1, end, true);
        }

        private static int FindNearSymbolInFunctDef(string defBody, char symbol, int startAt)
        {
            string featEnd = null;

            for (int i = startAt, count = defBody.Length; i < count; i++)
            {
                char c = defBody[i];

                if (featEnd == null)
                {
                    switch (c)
                    {
                        case '/':
                            if (i < count - 1 && defBody[i + 1] == '*')
                            {
                                i++;
                                featEnd = "*/";
                            }
                            break;
                        case '{':
                            featEnd = "}";
                            break;
                        case '<':
                            featEnd = ">";
                            break;
                        case '[':
                            featEnd = "]";
                            break;
                        case '(':
                            featEnd = ")";
                            break;
                        case '\'':
                            featEnd = "'";
                            break;
                        case '"':
                            featEnd = "\"";
                            break;
                        default:
                            if (c == symbol)
                                return i;
                            break;
                    }
                }
                else if (c == featEnd[0])
                {
                    if (featEnd == "\"" || featEnd == "'")
                    {
                        // Are we on an escaped ' or ""?
                        int escNo = 0;
                        int l = i - 1;
                        while (l > -1 && defBody[l--] == '\\')
                            escNo++;

                        if (escNo % 2 != 0)
                            continue;
                    }
                    else
                    {
                        int ci = i + 1;
                        int j;
                        int fl = featEnd.Length;
                        for (j = 1; j < fl && ci < count; j++)
                        {
                            if (defBody[ci++] != featEnd[j])
                                break;
                        }

                        if (j != fl)
                            continue;

                        i = ci - 1;
                    }

                    featEnd = null;
                }
            }

            return -1;
        }

        /// <summary>
        /// Display method signature
        /// </summary>
        /// <param name="sci">Scintilla control</param>
        /// <param name="autoHide">Auto-started completion (is false when pressing Ctrl+Space)</param>
        /// <returns>Auto-completion has been handled</returns>
        public static bool HandleFunctionCompletion(ScintillaControl sci, bool autoHide)
        {
            // only auto-complete where it makes sense
            if (DeclarationSectionOnly()) 
                return false;

            int position = sci.CurrentPos - 1;
            int paramIndex = FindParameterIndex(sci, ref position);
            if (position < 0) return false;
            
            // continuing calltip ?
            if (HasCalltip())
            {
                if (calltipPos == position)
                {
                    ShowCalltip(sci, paramIndex);
                    return true;
                }
                UITools.CallTip.Hide();
            }

            if (!ASContext.Context.CodeComplete.ResolveFunction(sci, position, autoHide))
                return true;

            // EventDispatchers
            if (paramIndex == 0 && calltipRelClass != null && calltipMember.Name.EndsWithOrdinal("EventListener"))
            {
                ShowListeners(sci, position, calltipRelClass);
                return true;
            }

            // show calltip
            ShowCalltip(sci, paramIndex);
            return true;
        }

        /// <summary>
        /// Find declaration of function called in code
        /// </summary>
        /// <param name="sci">Scintilla control</param>
        /// <param name="position">Position obtained by FindParameterIndex()</param>
        /// <param name="autoHide">Auto-started completion (is false when pressing Ctrl+Space)</param>
        /// <returns>Function successfully resolved</returns>
        internal bool ResolveFunction(ScintillaControl sci, int position, bool autoHide)
        {
            calltipPos = 0;
            calltipMember = null;
            calltipRelClass = null;
            var ctx = ASContext.Context;
            // get expression at cursor position
            var expr = GetExpression(sci, position, true);
            if (string.IsNullOrEmpty(expr.Value)
                || (expr.WordBefore == ctx.Features.functionKey && expr.Separator == " "))
                return false;
            // Expression before cursor
            expr.LocalVars = ParseLocalVars(expr);
            var result = EvalExpression(expr.Value, expr, ctx.CurrentModel, ctx.CurrentClass, true, true);
            return ResolveFunction(sci, position, result, autoHide);
        }

        /// <summary>
        /// Find declaration of function called in code
        /// </summary>
        /// <param name="sci">Scintilla control</param>
        /// <param name="position">Position obtained by FindParameterIndex()</param>
        /// <param name="expr">Expression before cursor</param>
        /// <param name="autoHide">Auto-started completion (is false when pressing Ctrl+Space)</param>
        /// <returns>Function successfully resolved</returns>
        protected virtual bool ResolveFunction(ScintillaControl sci, int position, ASResult expr, bool autoHide)
        {
           
            if (!expr.IsNull() && expr.Member == null && expr.Type != null)
            {
                foreach (MemberModel member in expr.Type.Members)
                    if (member.Name == expr.Type.Constructor)
                    {
                        expr.Member = member;
                        break;
                    }
            }
            var ctx = ASContext.Context;
            if (expr.IsNull() || (expr.Member != null && (expr.Member.Flags & FlagType.Function) == 0))
            {
                // custom completion
                MemberModel customMethod = ctx.ResolveFunctionContext(sci, expr.Context, autoHide);
                if (customMethod != null) expr = new ASResult {Member = customMethod, Context = new ASExpr()};
            }
            if (expr.IsNull())
                return false;

            MemberModel method = expr.Member;
            if (method == null)
            {
                if (expr.Type == null)
                    return false;
                string constructor = ASContext.GetLastStringToken(expr.Type.Name, ".");
                expr.Member = method = expr.Type.Members.Search(constructor, FlagType.Constructor, 0);
                if (method == null)
                    return false;
            }
            else if ((method.Flags & FlagType.Function) == 0)
            {
                if (method.Name == "super" && expr.Type != null)
                {
                    expr.Member = method = expr.Type.Members.Search(expr.Type.Constructor, FlagType.Constructor, 0);
                    if (method == null)
                        return false;
                }
                else return false;
            }

            // inherit doc
            while ((method.Flags & FlagType.Override) > 0 && expr.InClass != null
                && (method.Comments == null || method.Comments.Trim() == "" || method.Comments.Contains("@inheritDoc")))
            {
                FindMember(method.Name, expr.InClass.Extends, expr, 0, 0);
                method = expr.Member;
                if (method == null)
                    return false;
            }
            if ((method.Comments == null || method.Comments.Trim() == "") && expr.InClass?.Implements != null)
            {
                ASResult iResult = new ASResult();
                foreach (string type in expr.InClass.Implements)
                {
                    ClassModel model = ctx.ResolveType(type, expr.InFile);
                    FindMember(method.Name, model, iResult, 0, 0);
                    if (iResult.Member != null)
                    {
                        iResult.RelClass = expr.RelClass;
                        iResult.Context = expr.Context;
                        expr = iResult;
                        method = iResult.Member;
                        break;
                    }
                }
            }
            expr.Context.Position = position;
            FunctionContextResolved(sci, expr.Context, method, expr.RelClass, false);
            return true;
        }

        public static void FunctionContextResolved(ScintillaControl sci, ASExpr expr, MemberModel method, ClassModel inClass, bool showTip)
        {
            if (string.IsNullOrEmpty(method?.Name)) return;
            if (calltipMember != null && calltipMember.Name == method.Name)
            {
                // use FD-extracted comments
                if (method.Comments == null && !string.IsNullOrEmpty(calltipMember.Comments))
                    method.Comments = calltipMember.Comments;
            }

            int position = expr.Position;
            calltipPos = position;
            calltipOffset = Math.Min(expr.Value.Length, method.Name.Length);
            calltipDef = method.ToString();
            calltipMember = method;
            calltipDetails = UITools.Manager.ShowDetails;
            calltipRelClass = inClass;
            prevParam = "";

            if (showTip)
            {
                position = sci.CurrentPos - 1;
                sci.Colourise(0, -1);
                int paramIndex = FindParameterIndex(sci, ref position);
                if (position < 0) return;
                ShowCalltip(sci, paramIndex);
            }
        }

        /// <summary>
        /// Find type of current parameter in current function call
        /// </summary>
        /// <param name="paramIndex"></param>
        /// <param name="indexTypeOnly">Resolve only if parameter is an Object with an index type</param>
        /// <returns></returns>
        private static ClassModel ResolveParameterType(int paramIndex, bool indexTypeOnly)
        {
            if (calltipMember?.Parameters != null && paramIndex < calltipMember.Parameters.Count)
            {
                MemberModel param = calltipMember.Parameters[paramIndex];
                string type = param.Type;
                if (indexTypeOnly && (String.IsNullOrEmpty(type) || type.IndexOf('@') < 0))
                    return ClassModel.VoidClass;
                if (ASContext.Context.Features.objectKey == "Dynamic" && type.StartsWithOrdinal("Dynamic@"))
                    type = type.Replace("Dynamic@", "Dynamic<") + ">";
                return ASContext.Context.ResolveType(type, ASContext.Context.CurrentModel);
            }
            else return ClassModel.VoidClass;
        }

        /// <summary>
        /// Locate beginning of function call parameters and return index of current parameter
        /// </summary>
        internal static int FindParameterIndex(ScintillaControl sci, ref int position)
        {
            var context = ASContext.Context;
            int parCount = 0;
            int braCount = 0;
            int comaCount = 0;
            int arrCount = 0;
            var genCount = 0;
            while (position >= 0)
            {
                var style = sci.BaseStyleAt(position);
                if ((!IsLiteralStyle(style) && IsTextStyleEx(style)) || context.CodeComplete.IsStringInterpolationStyle(sci, position))
                {
                    var c = (char)sci.CharAt(position);
                    if (c <= ' ')
                    {
                        position--;
                        continue;
                    }
                    // skip {} () [] blocks
                    if ((braCount > 0 && c != '{' && c != '}')
                        || (parCount > 0 && c != '(' && c != ')')
                        || (arrCount > 0 && c != '[' && c != ']'))
                    {
                        position--;
                        continue;
                    }
                    if (c == ';' && braCount == 0)
                    {
                        position = -1;
                        break;
                    }
                    // new block
                    else if (c == '}') braCount++;
                    else if (c == ']') arrCount++;
                    else if (c == ')') parCount++;
                    // block closed
                    else if (c == '{')
                    {
                        if (braCount == 0) comaCount = 0;
                        else braCount--;
                    }
                    else if (c == '[')
                    {
                        if (arrCount == 0) comaCount = 0;
                        else arrCount--;
                    }
                    else if (c == '(')
                    {
                        if (--parCount < 0)
                            // function start found
                            break;
                    }
                    else if (c == '>') genCount++;
                    else if (c == '<') genCount--;
                    // new parameter reached
                    else if (c == ',' && parCount == 0 && genCount == 0)
                        comaCount++;
                }
                position--;
            }
            return comaCount;
        }

        private static void ShowListeners(ScintillaControl Sci, int position, ClassModel ofClass)
        {
            // find event metadatas
            List<ASMetaData> events = new List<ASMetaData>();
            while (ofClass != null && !ofClass.IsVoid())
            {
                if (ofClass.MetaDatas != null)
                {
                    foreach (ASMetaData meta in ofClass.MetaDatas)
                        if (meta.Kind == ASMetaKind.Event) events.Add(meta);
                }
                ofClass = ofClass.Extends;
            }
            if (events.Count == 0) return;

            // format
            events.Sort();
            Dictionary<String, ClassModel> eventTypes = new Dictionary<string, ClassModel>();
            List<ICompletionListItem> list = new List<ICompletionListItem>();
            foreach (ASMetaData meta in events)
            {
                string name = meta.Params["name"];
                Regex reName = new Regex("[\"']" + name + "[\"']");
                string type = meta.Params["type"];
                string comments = meta.Comments;
                if (name.Length > 0 && type.Length > 0)
                {
                    ClassModel evClass;
                    if (!eventTypes.ContainsKey(type))
                    {
                        evClass = ASContext.Context.ResolveType(type.Replace(':', '.'), null);
                        eventTypes.Add(type, evClass);
                    }
                    else evClass = eventTypes[type];
                    if (evClass.IsVoid()) 
                        continue;

                    bool typeFound = false;
                    foreach (MemberModel member in evClass.Members)
                    {
                        if (member.Value != null && reName.IsMatch(member.Value))
                        {
                            typeFound = true;
                            name = evClass.Name + '.' + member.Name;
                            if (meta.Comments == null && member.Comments != null) 
                                comments = member.Comments;
                            break;
                        }
                    }

                    if (!typeFound)
                    {
                        if (evClass.InFile.Package.StartsWithOrdinal("flash.")) 
                            continue; // hide built-in events not available in current player target
                        name = '"' + name + '"';
                    }
                    list.Add(new EventItem(name, evClass, comments));
                }
            }

            // filter
            list.Sort(new CompletionItemComparer());
            List<ICompletionListItem> items = new List<ICompletionListItem>();
            string prev = null;
            foreach (ICompletionListItem item in list)
            {
                if (item.Label != prev) items.Add(item);
                prev = item.Label;
            }

            // display
            Sci.SetSel(position + 1, Sci.CurrentPos);
            string tail = Sci.SelText;
            Sci.SetSel(Sci.SelectionEnd, Sci.SelectionEnd);
            CompletionList.Show(items, true, tail);
        }

        #endregion

        #region dot_completion
        /// <summary>
        /// Complete object member
        /// </summary>
        /// <param name="Sci">Scintilla control</param>
        /// <param name="autoHide">Don't keep the list open if the word does not match</param>
        /// <returns>Auto-completion has been handled</returns>
        private static bool HandleDotCompletion(ScintillaControl Sci, bool autoHide)
        {
            //this method can exit at multiple points, so reset the current class now rather than later
            currentClassHash = null;

            // only auto-complete where it makes sense
            if (autoHide && DeclarationSectionOnly()) return false;

            // get expression at cursor position
            int position = Sci.CurrentPos;
            var expr = GetExpression(Sci, position);
            if (expr.Value == null) return true;
            var ctx = ASContext.Context;
            var features = ctx.Features;
            int dotIndex = expr.Value.LastIndexOfOrdinal(features.dot);
            if (dotIndex == 0 && expr.Separator != "\"") return true;

            // complete keyword
            string word = expr.WordBefore;
            if (word != null && features.declKeywords.Contains(word)) return false;
            ClassModel argumentType = null;
            if (dotIndex < 0)
            {
                if (word != null)
                {
                    if (word == "class" || word == "package" || word == "interface" || word == "catch")
                        return false;
                    if (features.hasInference && word == "for") // haxe doesn't have 'var' in for()
                        return false;
                    // new/extends/implements
                    if (features.HasTypePreKey(word))
                        return HandleNewCompletion(Sci, expr.Value, autoHide, word);
                    // import
                    if (features.hasImports && (word == features.importKey || word == features.importKeyAlt))
                        return HandleImportCompletion(Sci, expr.Value, autoHide);
                }
                // type
                else if (features.hasEcmaTyping && expr.Separator == ":"
                    && HandleColonCompletion(Sci, expr.Value, autoHide))
                    return true;

                // no completion
                if ((expr.BeforeBody && expr.Separator != "=")
                    || expr.coma == ComaExpression.AnonymousObject
                    || expr.coma == ComaExpression.FunctionDeclaration)
                    return false;

                if (expr.coma == ComaExpression.AnonymousObjectParam)
                {
                    int cpos = Sci.CurrentPos - 1;
                    int paramIndex = FindParameterIndex(Sci, ref cpos);
                    if (calltipPos != cpos) ctx.CodeComplete.ResolveFunction(Sci, cpos, autoHide);
                    if (calltipMember == null) return false;
                    argumentType = ResolveParameterType(paramIndex, true);
                    if (argumentType.IsVoid()) return false;
                }

                // complete declaration
                MemberModel cMember = ctx.CurrentMember;
                int line = Sci.LineFromPosition(position);
                if (cMember == null && !ctx.CurrentClass.IsVoid())
                {
                    if (!string.IsNullOrEmpty(expr.Value))
                        return HandleDeclarationCompletion(Sci, expr.Value, autoHide);
                    if (ctx.CurrentModel.Version >= 2)
                        return ASGenerator.HandleGeneratorCompletion(Sci, autoHide, features.overrideKey);
                }
                else if (cMember != null && line == cMember.LineFrom)
                {
                    string text = Sci.GetLine(line);
                    int p;
                    if ((cMember.Flags & FlagType.Constructor) != 0 && !string.IsNullOrEmpty(features.ConstructorKey))
                        p = text.IndexOfOrdinal(features.ConstructorKey);
                    else p = text.IndexOfOrdinal(cMember.Name);
                    if (p < 0 || position < Sci.PositionFromLine(line) + p)
                        return HandleDeclarationCompletion(Sci, expr.Value, autoHide);
                }
            }
            else
            {
                if (expr.Value.EndsWithOrdinal("..") || Regex.IsMatch(expr.Value, "^[0-9]+\\.")) 
                    return false;
            }

            string tail = (dotIndex >= 0) ? expr.Value.Substring(dotIndex + features.dot.Length) : expr.Value;
            
            // custom completion
            var items = ctx.ResolveDotContext(Sci, expr, autoHide);
            if (items != null)
            {
                DotContextResolved(Sci, expr, items, autoHide);
                return true;
            }
            var mix = new MemberList();
            ctx.ResolveDotContext(Sci, expr, mix);

            // Context
            ASResult result;
            ClassModel tmpClass;
            bool outOfDate = (expr.Separator == ":") && ctx.UnsetOutOfDate();
            var cFile = ctx.CurrentModel;
            var cClass = ctx.CurrentClass;

            expr.LocalVars = ParseLocalVars(expr);
            if (argumentType != null)
            {
                result = new ASResult();
                tmpClass = argumentType;
                expr.LocalVars.Clear();
            }
            else if (dotIndex > 0)
            {
                // Expression before cursor
                result = EvalExpression(expr.Value, expr, cFile, cClass, false, false);
                if (result.IsNull())
                {
                    if (outOfDate) ctx.SetOutOfDate();
                    return true;
                }
                if (autoHide && features.hasE4X && IsXmlType(result.Type))
                    return true;
                tmpClass = result.Type;
            }
            else
            {
                result = new ASResult();
                if (expr.Separator == "\"")
                {
                    tmpClass = ctx.ResolveType(ctx.Features.stringKey, null);
                    result.Type = tmpClass;
                    dotIndex = 1;
                }
                else tmpClass = cClass;
            }

            //stores a reference to our current class.  tmpClass gets overwritten later, so we need to store the current class separately
            var classScope = tmpClass;
            // local vars are the first thing to try
            if ((result.IsNull() || (dotIndex < 0)) && expr.ContextFunction != null)
                mix.Merge(expr.LocalVars);

            // members visibility
            var curClass = cClass;
            curClass.ResolveExtends();
            var acc = ctx.TypesAffinity(curClass, tmpClass);

            // list package elements
            if (result.IsPackage)
            {
                mix.Merge(result.InFile.Imports);
                mix.Merge(result.InFile.Members);
            }
            // list instance members
            else if (expr.ContextFunction != null || expr.Separator != ":" || (dotIndex > 0 && !result.IsNull()))
            {
                // user setting may ask to hide some members
                bool limitMembers = autoHide; // ASContext.Context.HideIntrinsicMembers || (autoHide && !ASContext.Context.AlwaysShowIntrinsicMembers);

                // static or instance members?
                FlagType mask = 0;
                if (!result.IsNull()) mask = result.IsStatic ? FlagType.Static : FlagType.Dynamic;
                else if (IsStatic(expr.ContextFunction)) mask = FlagType.Static;
                else mask = 0;
                if (argumentType != null) mask |= FlagType.Variable;

                // explore members
                tmpClass.ResolveExtends();
                if (!limitMembers || result.IsStatic || tmpClass.Name != features.objectKey)
                while (tmpClass != null && !tmpClass.IsVoid())
                {
                    mix.Merge(tmpClass.GetSortedMembersList(), mask, acc);

                    // static inheritance
                    if ((mask & FlagType.Static) > 0)
                    {
                        if ((!features.hasStaticInheritance || dotIndex > 0) && (tmpClass.Flags & FlagType.TypeDef) == 0)
                            break;
                    }
                    else if (!features.hasStaticInheritance) mask |= FlagType.Dynamic;

                    tmpClass = tmpClass.Extends;
                    // hide Object class members
                    if (limitMembers && tmpClass != null && tmpClass.InFile.Package == "" && tmpClass.Name == features.objectKey) 
                        break;
                    // members visibility
                    acc = ctx.TypesAffinity(curClass, tmpClass);
                }
            }
            // known classes / toplevel vars/methods
            if (argumentType == null && (result.IsNull() || (dotIndex < 0)))
            {
                mix.Merge(cFile.GetSortedMembersList());
                mix.Merge(ctx.GetTopLevelElements());
                mix.Merge(ctx.GetVisibleExternalElements());
                mix.Merge(GetKeywords());
            }

            // show
            var list = new List<ICompletionListItem>();
            foreach (MemberModel member in mix)
            {
                if ((member.Flags & FlagType.Template) > 0)
                    list.Add(new TemplateItem(member));
                else
                    list.Add(new MemberItem(member));
            }
            EventManager.DispatchEvent(null, new DataEvent(EventType.Command, "ASCompletion.DotCompletion", list));
            CompletionList.Show(list, autoHide, tail);

            // smart focus token
            //if (!features.externalCompletion)
            AutoselectDotToken(classScope, tail);

            if (outOfDate) ctx.SetOutOfDate();
            return true;
        }

        private static MemberList GetKeywords()
        {
            IASContext ctx = ASContext.Context;
            ContextFeatures features = ctx.Features;
            bool inClass = !ctx.CurrentClass.IsVoid();

            MemberList decl = new MemberList();
            if (inClass || !ctx.CurrentModel.haXe)
            {
                foreach (string key in features.codeKeywords)
                    decl.Add(new MemberModel(key, key, FlagType.Template, 0));
            }
            if (!inClass)
            {
                foreach (string key in features.accessKeywords)
                    decl.Add(new MemberModel(key, key, FlagType.Template, 0));
                foreach (string key in features.typesKeywords)
                    decl.Add(new MemberModel(key, key, FlagType.Template, 0));
            }
            decl.Sort();
            return decl;
        }

        private static bool DeclarationSectionOnly()
        {
            ClassModel inClass = ASContext.Context.CurrentClass;
            return !inClass.IsVoid() && (inClass.Flags & (FlagType.Enum | FlagType.TypeDef | FlagType.Struct)) > 0;
        }

        private static void AutoselectDotToken(ClassModel classScope, string tail)
        {
            // remember the latest class resolved for completion to store later the inserted member
            currentClassHash = classScope?.QualifiedName;

            // if the completion history has a matching entry, it means the user has previously completed from this class.
            if (currentClassHash != null && completionHistory.ContainsKey(currentClassHash))
            {
                // If the last-completed member for the class starts with the currently typed text (tail), select it!
                // Note that if the tail is currently empty (i.e., the user has just typed the first dot), this still passes.
                // This allows it to highlight the last-completed member instantly just by hitting the dot.
                // Also does a check if the tail matches exactly the currently selected item; don't change it!
                if (CompletionList.SelectedLabel != tail && completionHistory[currentClassHash].ToLower().StartsWithOrdinal(tail.ToLower()))
                {
                    CompletionList.SelectItem(completionHistory[currentClassHash]);
                }
            }
        }

        static public void DotContextResolved(ScintillaControl Sci, ASExpr expr, MemberList items, bool autoHide)
        {
            // still valid context and position?
            if (Sci != ASContext.CurSciControl) return;
            int position = Sci.CurrentPos;
            ContextFeatures features = ASContext.Context.Features;
            ASExpr local = GetExpression(Sci, position);
            if (!local.Value.StartsWithOrdinal(expr.Value) 
                || expr.Value.LastIndexOfOrdinal(features.dot) != local.Value.LastIndexOfOrdinal(features.dot))
                return;
            string word = Sci.GetWordLeft(position-1, false);

            // current list
            string reSelect = null;
            if (CompletionList.Active) reSelect = CompletionList.SelectedLabel;

            // show completion
            List<ICompletionListItem> list = new List<ICompletionListItem>();
            bool testActive = !CompletionList.Active && expr.Position != position;
            foreach (MemberModel member in items)
            {
                if (testActive && member.Name == word)
                    return;
                list.Add(new MemberItem(member));
            }
            EventManager.DispatchEvent(null, new DataEvent(EventType.Command, "ASCompletion.DotCompletion", list));
            CompletionList.Show(list, autoHide, word);

            if (reSelect != null) CompletionList.SelectItem(reSelect);
        }

        #endregion

        #region types_completion

        private static string SelectTypedNewMember(ScintillaControl sci)
        {
            try
            {
                ASExpr expr = GetExpression(sci, sci.CurrentPos);
                if (expr.Value == null) return null;
                IASContext ctx = ASContext.Context;
                // try local var
                expr.LocalVars = ParseLocalVars(expr);
                foreach (MemberModel localVar in expr.LocalVars)
                {
                    if (localVar.LineTo == ctx.CurrentLine)
                    {
                        return localVar.Type;
                    }
                }
                // try member
                string currentLine = sci.GetLine(sci.CurrentLine);
                Match mVarNew = Regex.Match(currentLine, "\\s*(?<name>[a-z_$][a-z._$0-9]*)(?<decl>[: ]*)(?<type>[a-z.0-9<>]*)\\s*=\\s*new\\s", RegexOptions.IgnoreCase);
                if (mVarNew.Success)
                {
                    string name = mVarNew.Groups["name"].Value;
                    ASResult result = EvalExpression(name, expr, ctx.CurrentModel, ctx.CurrentClass, true, false);
                    if (result?.Member?.Type != null) // Might be missing or wrongly typed member
                    {
                        return result.Member.Type;
                    }
                }
            }
            catch {} // Do not throw exception with incorrect types

            return null;
        }

        protected static bool HandleNewCompletion(ScintillaControl sci, string tail, bool autoHide, string keyword)
        {
            List<ICompletionListItem> list;

            if (!ASContext.Context.Settings.LazyClasspathExploration
                && ASContext.Context.Settings.CompletionListAllTypes)
            {
                // show all project classes
                list = GetAllClasses(sci, true, true);

                if (list == null) return true;
            }
            else
            {
                // Consolidate known classes
                MemberList known = GetVisibleElements();
                list = new List<ICompletionListItem>();
                foreach (MemberModel member in known)
                    list.Add(new MemberItem(new MemberModel(member.Type, member.Type, member.Flags, member.Access)));
            }

            // If we are instantiating a class:
            //    1. Type exists:
            //       a. Generic type -> Show it with our index type.
            //       b. Not generic type -> Show existing one
            //    2. Type doesn't exist -> Show it with a warning symbol.
            string newItemType;
            if (keyword == "new" && (newItemType = SelectTypedNewMember(sci)) != null)
            {
                IASContext ctx = ASContext.Context;
                ClassModel aClass = ctx.ResolveType(newItemType, ctx.CurrentModel);

                ICompletionListItem newItem = null;
                if (!aClass.IsVoid())
                {
                    // AS2 special srictly typed Arrays supports
                    int p = newItemType.IndexOf('@');
                    if (p > -1) newItemType = newItemType.Substring(0, p);
                    else if (!string.IsNullOrEmpty(aClass.IndexType))
                    {
                        newItemType = aClass.QualifiedName;
                        newItem = new MemberItem(new MemberModel(newItemType, aClass.Type, aClass.Flags, aClass.Access));
                    }
                }
                else
                {
                    newItem = new NonexistentMemberItem(newItemType);
                }

                if (newItem != null)
                {
                    int itemIndex = list.FindIndex(item => string.Compare(item.Label, newItem.Label, StringComparison.OrdinalIgnoreCase) >= 0);

                    int genericStart = newItemType.IndexOfOrdinal("<");
                    if (genericStart > -1 && ASContext.Context.Features.HasGenericsShortNotation)
                    {
                        newItemType = newItemType.Substring(0, genericStart);
                        itemIndex = itemIndex > 0 ? itemIndex : 0;
                    }
                    else itemIndex = itemIndex > 0 ? itemIndex - 1 : 0;

                    list.Insert(itemIndex, newItem);
                }

                CompletionList.Show(list, autoHide, tail);
                CompletionList.SelectItem(newItemType);
            }
            else CompletionList.Show(list, autoHide, tail);
            return true;
        }

        private static bool HandleImportCompletion(ScintillaControl Sci, string tail, bool autoHide)
        {
            if (!ASContext.Context.Features.hasImports) return false;

            if (!ASContext.Context.Settings.LazyClasspathExploration
                && ASContext.Context.Settings.CompletionListAllTypes)
            {
                // show all project classes
                HandleAllClassesCompletion(Sci, "", false, false);
            }
            else
            {
                // list visible classes
                MemberList known = GetVisibleElements();

                // show
                List<ICompletionListItem> list = new List<ICompletionListItem>();
                foreach (MemberModel member in known)
                    list.Add(new MemberItem(member));
                CompletionList.Show(list, autoHide, tail);
            }
            return true;
        }

        private static bool HandleColonCompletion(ScintillaControl Sci, string tail, bool autoHide)
        {
            ComaExpression coma;
            if (DeclarationSectionOnly()) coma = ComaExpression.FunctionDeclaration;
            else coma = GetFunctionContext(Sci, autoHide);

            if (coma != ComaExpression.FunctionDeclaration && coma != ComaExpression.VarDeclaration)
                return false;

            if (!ASContext.Context.Settings.LazyClasspathExploration
                && ASContext.Context.Settings.CompletionListAllTypes)
            {
                // show all project classes
                HandleAllClassesCompletion(Sci, tail, true, false);
            }
            else
            {
                bool outOfDate = ASContext.Context.UnsetOutOfDate();

                // list visible classes
                MemberList known = GetVisibleElements();

                // show
                List<ICompletionListItem> list = new List<ICompletionListItem>();
                foreach (MemberModel member in known)
                    list.Add(new MemberItem(member));
                CompletionList.Show(list, autoHide, tail);
                if (outOfDate) ASContext.Context.SetOutOfDate();
            }
            return true;
        }

        private static ComaExpression GetFunctionContext(ScintillaControl Sci, bool autoHide)
        {
            ContextFeatures features = ASContext.Context.Features;
            ComaExpression coma = ComaExpression.None;
            int position = Sci.CurrentPos - 1;
            char c = ' ';
            //bool inGenericType = false;
            while (position > 0)
            {
                c = (char)Sci.CharAt(position);
                //if (c == '<') inGenericType = true;
                if (c == ':' || c == ';' || c == '=' || c == ',') break;
                position--;
            }
            position--;

            // var declaration
            GetWordLeft(Sci, ref position);
            string keyword = (c == ':') ? GetWordLeft(Sci, ref position) : null;
            if (keyword == features.varKey || (features.constKey != null && keyword == features.constKey))
                coma = ComaExpression.VarDeclaration;
            // function return type
            else if ((char)Sci.CharAt(position) == ')')
            {
                int parCount = 0;
                while (position > 0)
                {
                    position--;
                    c = (char)Sci.CharAt(position);
                    if (c == ')') parCount++;
                    else if (c == '(')
                    {
                        parCount--;
                        if (parCount < 0)
                        {
                            position--;
                            break;
                        }
                    }
                }
                keyword = GetWordLeft(Sci, ref position);
                if (keyword == "" && Sci.CharAt(position) == '>' && features.hasGenerics)
                {
                    int groupCount = 1;
                    position--;
                    while (position >= 0 && groupCount > 0)
                    {
                        c = (char)Sci.CharAt(position);
                        if ("({[<".IndexOf(c) > -1)
                            groupCount--;
                        else if (")}]>".IndexOf(c) > -1)
                            groupCount++;
                        position--;
                    }
                    keyword = GetWordLeft(Sci, ref position);
                }
                if (keyword == features.functionKey)
                    coma = ComaExpression.FunctionDeclaration;
                else
                {
                    keyword = GetWordLeft(Sci, ref position);
                    if (keyword == features.functionKey || keyword == features.getKey || keyword == features.setKey)
                        coma = ComaExpression.FunctionDeclaration;
                    else if (ASContext.Context.CurrentModel.haXe && keyword == features.varKey && 
                        (ASContext.Context.CurrentMember == null || (ASContext.Context.CurrentMember.Flags & FlagType.Function) == 0))
                        coma = ComaExpression.VarDeclaration;  // Haxe Properties
                }
            }
            // needs more guessing
            else
            {
                // config constant, or namespace access
                if (keyword == "" && position > 0 && (char)Sci.CharAt(position) == ':')
                {
                    int pos = position - 1;
                    keyword = GetWordLeft(Sci, ref pos);
                    if (keyword != "" && autoHide) return ComaExpression.None;
                }
                coma = DisambiguateComa(Sci, position, 0);
            }
            return coma;
        }

        /// <summary>
        /// Display the full project classes list
        /// </summary>
        /// <param name="Sci"></param>
        public static void HandleAllClassesCompletion(ScintillaControl Sci, string tail, bool classesOnly, bool showClassVars)
        {
            var list = GetAllClasses(Sci, classesOnly, showClassVars);
            list.Sort(new CompletionItemCaseSensitiveImportComparer());
            CompletionList.Show(list, false, tail);
        }

        private static bool HandleInterpolationCompletion(ScintillaControl sci, bool autoHide, bool expressions)
        {
            IASContext ctx = ASContext.Context;
            MemberList members = new MemberList();
            ASExpr expr = GetExpression(sci, sci.CurrentPos);
            if (expr.ContextMember == null) return false;

            members.Merge(ctx.CurrentClass.GetSortedMembersList());

            if ((expr.ContextMember.Flags & FlagType.Static) > 0)
                members.RemoveAllWithoutFlag(FlagType.Static);
            else
                members.Merge(ctx.CurrentClass.GetSortedInheritedMembersList());

            members.Merge(ParseLocalVars(expr));
            
            if (!expressions)
            {
                members.RemoveAllWithFlag(FlagType.Function);
            }
            
            List<ICompletionListItem> list = new List<ICompletionListItem>();
            foreach (MemberModel member in members)
                list.Add(new MemberItem(member));
            CompletionList.Show(list, autoHide);

            return true;
        }
        #endregion

        private static bool HandleMetadataCompletion(bool autoHide)
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

        /// <summary>
        /// Handle completion after inserting a space character
        /// </summary>
        /// <param name="sci">Scintilla control</param>
        /// <param name="position">Current cursor position</param>
        /// <param name="autoHide">Don't keep the list open if the word does not match</param>
        /// <returns>Auto-completion has been handled</returns>
        private bool HandleWhiteSpaceCompletion(ScintillaControl sci, int position, bool autoHide)
        {
            var ctx = ASContext.Context;
            var features = ctx.Features;
            var pos = position - 1;
            var word = GetWordLeft(sci, ref pos);
            if (word.Length == 0)
            {
                var c = (char)sci.CharAt(pos);
                if (c == ':' && features.hasEcmaTyping) return HandleColonCompletion(sci, "", autoHide);
                if (c == ',')
                {
                    var currentClass = ctx.CurrentClass;
                    if (currentClass.Flags.HasFlag(FlagType.Class) && PositionIsBeforeBody(sci, pos, currentClass))
                    {
                        var endPosition = sci.PositionFromLine(currentClass.LineFrom);
                        for (var i = pos; i > endPosition; i--)
                        {
                            if (sci.PositionIsOnComment(i) || c <= ' ') continue;
                            var e = GetExpressionType(sci, i, false, true);
                            if (e.Type == currentClass) break;
                            var value = e.Context.Value;
                            if (value == features.ExtendsKey) break;
                            if (value == features.ImplementsKey) return HandleNewCompletion(sci, string.Empty, autoHide, string.Empty);
                            i -= value.Length;
                        }
                    }
                }
                if (autoHide && (c == '(' || c == ',') && !ASContext.CommonSettings.DisableCallTip)
                    return HandleFunctionCompletion(sci, autoHide);
                return false;
            }
            // import
            if (features.hasImports && (word == features.importKey || word == features.importKeyAlt))
                return HandleImportCompletion(sci, "", autoHide);
            if (word == "package" || features.typesKeywords.Contains(word)) return false;
            if (word == features.ImplementsKey) return HandleImplementsCompletion(sci, autoHide);
            // new/extends/instanceof/...
            if (features.HasTypePreKey(word)) return HandleNewCompletion(sci, "", autoHide, word);
            var beforeBody = true;
            var expr = CurrentResolvedContext?.Result?.Context;
            if (expr != null) beforeBody = expr.ContextFunction == null || expr.BeforeBody;
            if (!beforeBody && features.codeKeywords.Contains(word)) return false;
            // override
            if (word == features.overrideKey) return ASGenerator.HandleGeneratorCompletion(sci, autoHide, word);
            // public/internal/private/protected/static
            if (features.accessKeywords.Contains(word)) return HandleDeclarationCompletion(sci, "", autoHide);
            return HandleWhiteSpaceCompletion(sci, position, word, autoHide);
        }

        /// <summary>
        /// Handle completion after inserting a space character
        /// </summary>
        /// <param name="sci">Scintilla control</param>
        /// <param name="position">Current cursor position</param>
        /// <param name="wordLeft">Word before cursor</param>
        /// <param name="autoHide">Don't keep the list open if the word does not match</param>
        /// <returns>Auto-completion has been handled</returns>
        protected virtual bool HandleWhiteSpaceCompletion(ScintillaControl sci, int position, string wordLeft, bool autoHide) => false;

        /// <summary>
        /// Display the full project interfaces list
        /// </summary>
        /// <param name="sci">Scintilla control</param>
        /// <param name="autoHide">Don't keep the list open if the word does not match</param>
        /// <returns>Auto-completion has been handled</returns>
        protected virtual bool HandleImplementsCompletion(ScintillaControl sci, bool autoHide)
        {
            var list = new List<ICompletionListItem>();
            foreach (MemberModel it in ASContext.Context.GetAllProjectClasses())
            {
                if (!it.Flags.HasFlag(FlagType.Interface)) continue;
                list.Add(new MemberItem(it));
            }
            CompletionList.Show(list, autoHide);
            return true;
        }

        #region expression_evaluator

        /// <summary>
        /// Find expression type in function context
        /// </summary>
        /// <param name="expression">To evaluate</param>
        /// <param name="context">Completion context</param>
        /// <param name="inFile">File context</param>
        /// <param name="inClass">Class context</param>
        /// <param name="complete">Complete (sub-expression) or partial (dot-completion) evaluation</param>
        /// <param name="asFunction"></param>
        /// <returns>Class/member struct</returns>
        private static ASResult EvalExpression(string expression, ASExpr context, FileModel inFile, ClassModel inClass, bool complete, bool asFunction)
        {
            return ASContext.Context.CodeComplete.EvalExpression(expression, context, inFile, inClass, complete, asFunction, true);
        }

        /// <summary>
        /// Find expression type in function context
        /// </summary>
        /// <param name="expression">To evaluate</param>
        /// <param name="context">Completion context</param>
        /// <param name="inFile">File context</param>
        /// <param name="inClass">Class context</param>
        /// <param name="complete">Complete (sub-expression) or partial (dot-completion) evaluation</param>
        /// <param name="asFunction"></param>
        /// <param name="filterVisibility"></param>
        /// <returns>Class/member struct</returns>
        protected virtual ASResult EvalExpression(string expression, ASExpr context, FileModel inFile, ClassModel inClass, bool complete, bool asFunction, bool filterVisibility)
        {
            var notFound = new ASResult {Context = context};
            if (string.IsNullOrEmpty(expression)) return notFound;
            if (expression[0] == '.')
            {
                if (expression.StartsWithOrdinal(".#")) expression = expression.Substring(1);
                else if (context.Separator == "\"") expression = "\"" + expression;
                else return notFound;
            }

            var ctx = ASContext.Context;
            var features = ctx.Features;
            var tokens = Regex.Split(expression, Regex.Escape(features.dot));

            // eval first token
            var token = tokens[0];
            if (token.Length == 0) return notFound;
            if (asFunction && tokens.Length == 1) token += "(";
            ASResult head = null;
            if (token[0] == '#')
            {
                Match mSub = re_sub.Match(token);
                if (mSub.Success)
                {
                    string subExpr = context.SubExpressions[Convert.ToInt16(mSub.Groups["index"].Value)];
                    // parse sub expression
                    subExpr = subExpr.Substring(1, subExpr.Length - 2).Trim();
                    ASExpr subContext = new ASExpr(context);
                    subContext.SubExpressions = ExtractedSubex = new List<string>();
                    subExpr = re_balancedParenthesis.Replace(subExpr, ExtractSubex);
                    Match m = re_refineExpression.Match(subExpr);
                    if (!m.Success) return notFound;
                    Regex re_dot = new Regex("[\\s]*" + Regex.Escape(features.dot) + "[\\s]*");
                    subExpr = re_dot.Replace(re_whiteSpace.Replace(m.Value, " "), features.dot).Trim();
                    int space = subExpr.LastIndexOf(' ');
                    if (space > 0) subExpr = subExpr.Substring(space + 1);
                    // eval sub expression
                    head = EvalExpression(subExpr, subContext, inFile, inClass, true, false);
                    if (head.Member != null)
                        head.Type = ctx.ResolveType(head.Member.Type, head.Type.InFile);
                }
                else
                {
                    token = token.Substring(token.IndexOf('~') + 1);
                    head = EvalVariable(token, context, inFile, inClass);
                }
            }
            else
            {
                var type = ctx.ResolveToken(token, inClass.InFile);
                if (!type.IsVoid()) return EvalTail(context, inFile, new ASResult {Type = type}, tokens, complete, filterVisibility) ?? notFound;
                if (token.Contains("<")) head = new ASResult {Type = ctx.ResolveType(token, inFile)};
                else head = EvalVariable(token, context, inFile, inClass); // regular eval
            }

            // no head, exit
            if (head.IsNull()) return notFound;

            // accessing instance member in static function, exit
            if (IsStatic(context.ContextFunction) && context.WordBefore != features.overrideKey
                && head.RelClass == inClass
                && head.Member != null && !IsStatic(head.Member)
                && (head.Member.Flags & FlagType.Constructor) == 0)
                return notFound;

            // resolve
            var result = EvalTail(context, inFile, head, tokens, complete, filterVisibility);

            // if failed, try as qualified class name
            if ((result == null || result.IsNull()) && tokens.Length > 1) 
            {
                var qualif = ctx.ResolveType(expression, null);
                if (!qualif.IsVoid())
                {
                    result = new ASResult();
                    result.Context = context;
                    result.IsStatic = true;
                    result.InFile = qualif.InFile;
                    result.Type = qualif;
                }
            }
            return result ?? notFound;
        }

        static ASResult EvalTail(ASExpr context, FileModel inFile, ASResult head, IList<string> tokens, bool complete, bool filterVisibility)
        {
            // eval tail
            int n = tokens.Count;
            if (!complete) n--;
            // context
            IASContext ctx = ASContext.Context;
            ContextFeatures features = ctx.Features;
            ASResult step = head;
            // look for static or dynamic members?
            FlagType mask = head.IsStatic ? FlagType.Static : FlagType.Dynamic;
            // members visibility
            ClassModel curClass = ctx.CurrentClass;
            curClass.ResolveExtends();
            Visibility acc = ctx.TypesAffinity(curClass, step.Type);

            // explore
            var inE4X = false;
            var path = tokens[0];
            step.Path = path;
            step.Context = context;

            for (int i = 1; i < n; i++)
            {
                var token = tokens[i];
                path += features.dot + token;
                step.Path = path;

                if (token.Length == 0)
                {
                    // this means expression ends with one dot
                    if (i == n - 1)
                        return step;
                    // this means 2 dots in the expression: consider as E4X expression
                    if (features.hasE4X && IsXmlType(step.Type) && i < n - 1)
                    {
                        inE4X = true;
                        step = new ASResult();
                        step.Member = new MemberModel(token, "XMLList", FlagType.Variable | FlagType.Dynamic | FlagType.AutomaticVar, Visibility.Public);
                        step.Type = ctx.ResolveType(features.objectKey, null);
                        acc = Visibility.Public;
                    }
                    else return null;
                }
                else if (step.IsPackage)
                {
                    FindMember(token, inFile, step, mask, filterVisibility ? acc : 0);
                    if (step.IsNull())
                        return step;
                }
                else if (step.Type != null && !step.Type.IsVoid())
                {
                    var resultClass = step.Type;
                    // handle typed indexes automatic typing
                    if (token[0] == '#' && step.InClass != null && step.Member != null
                        && step.InClass.IndexType == step.Member.Type)
                    {
                        step.InFile = step.InClass.InFile;
                    }


                    // if the current class ends back to the starting point (classA -> classB -> classA), 
                    // restore the private, protected, and internal member references
                    if (curClass == step.Type)
                    {
                        // full visibility for this evaluation only
                        Visibility selfVisibility = acc | Visibility.Private | Visibility.Protected | Visibility.Internal;
                        FindMember(token, resultClass, step, mask, selfVisibility);
                    }
                    else
                    {
                        FindMember(token, resultClass, step, mask, filterVisibility ? acc : 0);
                    }

                    // Haxe modules
                    if (step.Type == null && features.hasModules)
                    {
                        foreach(ClassModel oClass in resultClass.InFile.Classes)
                            if (oClass.Name == token)
                            {
                                step.Type = oClass;
                                step.InFile = resultClass.InFile;
                                break;
                            }
                    }

                    // handle E4X expressions
                    if (step.Type == null)
                    {
                        if (inE4X || (ctx.Features.hasE4X && IsXmlType(resultClass)))
                        {
                            inE4X = false;
                            step = new ASResult();
                            step.Member = new MemberModel(token, "XMLList", FlagType.Variable | FlagType.Dynamic | FlagType.AutomaticVar, Visibility.Public);
                            step.Type = ctx.ResolveType("XMLList", null);
                            step.Context = context;
                            step.Path = path;
                        }
                        else return step;
                        // members visibility
                        acc = ctx.TypesAffinity(curClass, step.Type);
                    }
                    else inE4X = false;

                    if (!step.IsStatic && (mask & FlagType.Static) > 0)
                    {
                        mask -= FlagType.Static;
                        mask |= FlagType.Dynamic;
                    }
                }
                else
                {
                    step.Member = null;
                    step.InClass = null;
                    step.InFile = null;
                    step.Type = null;
                    return step;
                }
            }
            return step;
        }

        private static bool IsStatic(MemberModel member) => member != null && (member.Flags & FlagType.Static) > 0;

        /// <summary>
        /// Find variable type in function context
        /// </summary>
        /// <param name="token">Variable name</param>
        /// <param name="local">Completion context</param>
        /// <param name="inFile">File context</param>
        /// <param name="inClass">Class context</param>
        /// <returns>Class/member struct</returns>
        private static ASResult EvalVariable(string token, ASExpr local, FileModel inFile, ClassModel inClass)
        {
            var result = new ASResult();
            if (local.coma == ComaExpression.AnonymousObjectParam) return result;
            var context = ASContext.Context;
            var features = context.Features;
            if (!inClass.IsVoid()) inFile = inClass.InFile;

            var p = token.IndexOf('(');
            if (p > 0) token = token.Substring(0, p);

            // top-level elements resolution
            context.ResolveTopLevelElement(token, result);
            if (!result.IsNull())
            {
                if (result.Member != null && (result.Member.Flags & FlagType.Function) > 0 && p < 0)
                    result.Type = ResolveType("Function", null);
                return result;
            }
            if (!inClass.IsVoid() && !string.IsNullOrEmpty(features.ConstructorKey) && token == features.ConstructorKey && local.BeforeBody)
                return EvalVariable(inClass.Name, local, inFile, inClass);
            var contextMember = local.ContextMember;
            if (contextMember == null || local.coma != ComaExpression.None || !local.BeforeBody || (contextMember.Flags & (FlagType.Getter | FlagType.Setter)) > 0
                || (local.BeforeBody && local.WordBefore != features.functionKey))
            {
                // local vars
                if (local.LocalVars != null)
                {
                    // Haxe 3 get/set keyword in properties declaration
                    if ((token == "set" || token == "get") && local.ContextFunction == null && contextMember?.Parameters != null && contextMember.Parameters.Count == 2)
                    {
                        if (token == "get" && contextMember.Parameters[0].Name == "get") return EvalVariable("get_" + contextMember.Name, local, inFile, inClass);
                        if (token == "set" && contextMember.Parameters[1].Name == "set") return EvalVariable("set_" + contextMember.Name, local, inFile, inClass);
                    }
                    if (local.LocalVars.Count > 0)
                    {
                        var vars = local.LocalVars.Items.Where(it => it.Name == token).ToList();
                        if (vars.Count > 0)
                        {
                            var checkFunction = local.SubExpressions != null && local.SubExpressions.Count == 1
                                && !string.IsNullOrEmpty(local.Value) && local.Value.IndexOf('.') == local.Value.IndexOfOrdinal(".#0~")
                                // array access check
                                && local.SubExpressions[0][0] != '[';
                            MemberModel var = null;
                            if (vars.Count > 1)
                            {
                                vars.Sort((l, r) => l.LineFrom > r.LineFrom ? -1 : l.LineFrom < r.LineFrom ? 1 : 0);
                                var = vars.FirstOrDefault(it => it.LineTo < local.LineTo && (!checkFunction || it.Flags.HasFlag(FlagType.Function)));
                            }
                            if (var == null) var = vars.FirstOrDefault(it => !checkFunction || it.Flags.HasFlag(FlagType.Function));
                            if (var != null)
                            {
                                result.Member = var;
                                result.InFile = inFile;
                                result.InClass = inClass;
                                if (features.hasInference && var.Type == null)
                                {
                                    if (var.Flags.HasFlag(FlagType.LocalVar)) context.CodeComplete.InferVariableType(local, var);
                                    else if (var.Flags.HasFlag(FlagType.ParameterVar)) context.CodeComplete.InferParameterVarType(var);
                                }
                                if (string.IsNullOrEmpty(var.Type)) result.Type = context.ResolveType(features.objectKey, null);
                                else if (var.Flags.HasFlag(FlagType.Function)) result.Type = context.ResolveType("Function", null);
                                else result.Type = ResolveType(var.Type, inFile);
                                return result;
                            }
                        }
                    }
                }
                // method parameters
                if (local.ContextFunction?.Parameters != null)
                {
                    foreach (var para in local.ContextFunction.Parameters)
                        if (para.Name == token || (para.Name[0] == '?' && para.Name.Substring(1) == token))
                        {
                            result.Member = para;
                            result.Type = ResolveType(para.Type, inFile);
                            return result;
                        }
                }
            }
            // class members
            if (!inClass.IsVoid())
            {
                FindMember(token, inClass, result, 0, 0);
                if (!result.IsNull())
                {
                    if (features.hasInference)
                    {
                        var member = result.Member;
                        if (member != null && member.Flags.HasFlag(FlagType.Variable) && member.Type == null)
                        {
                            context.CodeComplete.InferVariableType(local, member);
                            if (string.IsNullOrEmpty(member.Type)) member.Type = context.ResolveType(features.dynamicKey, null).Name;
                        }
                    }
                    return result;
                }
            }
            // file member
            if (inFile.Version != 2 || inClass.IsVoid())
            {
                FindMember(token, inFile, result, 0, 0);
                if (!result.IsNull()) return result;
            }
            // current file types
            foreach(var aClass in inFile.Classes)
            {
                if (aClass.Name != token || (context.InPrivateSection && aClass.Access != Visibility.Private)) continue;
                result.Type = aClass;
                result.IsStatic = p < 0;
                return result;
            }
            // visible types & declarations
            var visible = context.GetVisibleExternalElements();
            foreach (MemberModel aDecl in visible)
            {
                if (aDecl.Name != token) continue;
                if ((aDecl.Flags & FlagType.Package) > 0)
                {
                    var package = context.ResolvePackage(token, false);
                    if (package != null)
                    {
                        result.InFile = package;
                        result.IsPackage = true;
                        result.IsStatic = true;
                        return result;
                    }
                }
                else if ((aDecl.Flags & (FlagType.Class | FlagType.Enum)) > 0)
                {
                    ClassModel friendClass = null;
                    if (aDecl.InFile != null)
                    {
                        foreach(var aClass in aDecl.InFile.Classes)
                            if (aClass.Name == token)
                            {
                                friendClass = aClass;
                                break;
                            }
                    }
                    if (friendClass == null) friendClass = context.ResolveType(aDecl.Type, inFile);
                    if (!friendClass.IsVoid())
                    {
                        result.Type = friendClass;
                        result.IsStatic = (p < 0);
                        return result;
                    }
                }
                else if ((aDecl.Flags & FlagType.Function) > 0)
                {
                    result.Member = aDecl;
                    result.RelClass = ClassModel.VoidClass;
                    result.InClass = FindClassOf(aDecl);
                    result.Type = (p < 0)
                        ? context.ResolveType("Function", null)
                        : context.ResolveType(aDecl.Type, aDecl.InFile);
                    result.InFile = aDecl.InFile;
                    return result;
                }
                else if ((aDecl.Flags & (FlagType.Variable | FlagType.Getter)) > 0)
                {
                    result.Member = aDecl;
                    result.RelClass = ClassModel.VoidClass;
                    result.InClass = FindClassOf(aDecl);
                    result.Type = context.ResolveType(aDecl.Type, aDecl.InFile);
                    result.InFile = aDecl.InFile;
                    return result;
                }
            }
            return result;
        }

        private static ClassModel FindClassOf(MemberModel aDecl)
        {
            if (aDecl.InFile != null) 
                foreach (ClassModel aClass in aDecl.InFile.Classes)
                {
                    foreach (MemberModel member in aClass.Members)
                        if (member == aDecl) return aClass;
                }
            return ClassModel.VoidClass;
        }

        private static ClassModel ResolveType(string qname, FileModel inFile)
        {
            if (qname == null) return ClassModel.VoidClass;
            IASContext context = ASContext.Context;
            bool isQualified = qname.IndexOf('.') > 0;

            if (inFile == null || inFile == context.CurrentModel)
                foreach (MemberModel aDecl in context.GetVisibleExternalElements())
                {
                    if (aDecl.Name == qname || (isQualified && aDecl.Type == qname))
                    {
                        if (aDecl.InFile != null)
                        {
                            foreach (ClassModel aClass in aDecl.InFile.Classes)
                                if (aClass.Name == aDecl.Name) return aClass;
                            return context.GetModel(aDecl.InFile.Package, qname, inFile?.Package);
                        }
                        else return context.ResolveType(aDecl.Type, inFile);
                    }
                }

            return context.ResolveType(qname, inFile);
        }

        /// <summary>
        /// Infer very simple cases: var foo = {expression}
        /// </summary>
        private void InferVariableType(ASExpr local, MemberModel var)
        {
            var sci = ASContext.CurSciControl;
            if (sci == null || var.LineFrom >= sci.LineCount) return;
            InferVariableType(sci, local, var);
        }

        protected virtual void InferVariableType(ScintillaControl sci, ASExpr local, MemberModel var)
        {
            // is it a simple affectation inference?
            var text = sci.GetLine(var.LineFrom);
            var m = Regex.Match(text, "=([^;]+)");
            if (!m.Success) return;
            var rvalue = m.Groups[1];
            if (rvalue.Length == 0) return;
            var offset = rvalue.Length - rvalue.Value.TrimStart().Length;
            var rvalueStart = sci.PositionFromLine(var.LineFrom) + rvalue.Index + offset;
            InferVariableType(sci, text, rvalueStart, local, var);
        }

        protected virtual void InferVariableType(ScintillaControl sci, string declarationLine, int rvalueStart, ASExpr local, MemberModel var)
        {
            var p = declarationLine.IndexOf(';');
            var text = declarationLine.TrimEnd();
            if (p < 0) p = text.Length;
            if (text.EndsWith('(')) p--;
            // resolve expression
            var expr = GetExpression(sci, sci.PositionFromLine(var.LineFrom) + p, true);
            if (string.IsNullOrEmpty(expr.Value)) return;
            var result = EvalExpression(expr.Value, expr, ASContext.Context.CurrentModel, ASContext.Context.CurrentClass, true, false);
            if (result.IsNull()) return;
            if (result.Type != null && !result.Type.IsVoid())
            {
                var.Type = result.Type.QualifiedName;
                var.Flags |= FlagType.Inferred;
            }
            else if (result.Member != null)
            {
                var.Type = result.Member.Type;
                var.Flags |= FlagType.Inferred;
            }
        }

        /// <summary>
        /// Infer very simple cases: function foo(value = {expression})
        /// </summary>
        private void InferParameterVarType(MemberModel var)
        {
            var ctx = ASContext.Context;
            var type = ctx.ResolveToken(var.Value, ctx.CurrentModel);
            if (type.IsVoid()) type = ctx.ResolveType(ctx.Features.dynamicKey, null);
            var.Type = type.Name;
        }

        /// <summary>
        /// Find package-level member
        /// </summary>
        /// <param name="token">To match</param>
        /// <param name="inFile">In given file</param>
        /// <param name="result">Class/Member struct</param>
        /// <param name="mask">Flags mask</param>
        /// <param name="acc">Visibility mask</param>
        public static void FindMember(string token, FileModel inFile, ASResult result, FlagType mask, Visibility acc)
        {
            if (string.IsNullOrEmpty(token))
                return;

            // package
            if (result.IsPackage)
            {
                string fullName = (result.InFile.Package.Length > 0) ? result.InFile.Package + "." + token : token;
                foreach (MemberModel mPack in result.InFile.Imports)
                {
                    if (mPack.Name == token)
                    {
                        // sub-package
                        if (mPack.Flags == FlagType.Package)
                        {
                            FileModel package = ASContext.Context.ResolvePackage(fullName, false);
                            if (package != null) result.InFile = package;
                            else
                            {
                                result.IsPackage = false;
                                result.InFile = null;
                            }
                        }
                        // class
                        else
                        {
                            result.IsPackage = false;
                            result.Type = ResolveType(fullName, ASContext.Context.CurrentModel);
                            result.InFile = result.Type.InFile;
                        }
                        return;
                    }
                    else
                    {
                        int p;
                        if ((p = mPack.Name.IndexOf('<')) > 0)
                        {
                            if (p > 1 && mPack.Name[p - 1] == '.') p--;
                            if (mPack.Name.Substring(0, p) == token)
                            {
                                result.IsPackage = false;
                                result.Type = ResolveType(fullName + mPack.Name.Substring(p), ASContext.Context.CurrentModel);
                                result.InFile = result.Type.InFile;
                                return;
                            }
                        }
                    }
                }
                foreach (MemberModel member in result.InFile.Members)
                {
                    if (member.Name == token)
                    {
                        result.InClass = ClassModel.VoidClass;
                        result.InFile = member.InFile ?? inFile;
                        result.Member = member;
                        result.Type = ResolveType(member.Type, inFile);
                        result.IsStatic = false;
                        result.IsPackage = false;
                        return;
                    }
                }
                // dead end
                result.IsPackage = false;
                result.Type = null;
                result.Member = null;
                return;
            }

            // variable
            var found = inFile.Members.Search(token, mask, acc);
            // ignore setters
            if (found != null && (found.Flags & FlagType.Setter) > 0)
            {
                found = null;
                MemberList matches = inFile.Members.MultipleSearch(token, mask, acc);
                foreach (MemberModel member in matches)
                {
                    found = member;
                    if ((member.Flags & FlagType.Setter) == 0) break;
                }
            }
            if (found != null)
            {
                result.InClass = ClassModel.VoidClass;
                result.InFile = inFile;
                result.Member = found;
                result.Type = ResolveType(found.Type, inFile);
                result.IsStatic = false;
            }
        }

        /// <summary>
        /// Match token to a class' member
        /// </summary>
        /// <param name="token">To match</param>
        /// <param name="inClass">In given class</param>
        /// <param name="result">Class/Member struct</param>
        /// <param name="mask">Flags mask</param>
        /// <param name="acc">Visibility mask</param>
        public static void FindMember(string token, ClassModel inClass, ASResult result, FlagType mask, Visibility acc)
        {
            if (string.IsNullOrEmpty(token))
                return;

            IASContext context = ASContext.Context;
            ContextFeatures features = context.Features;
            MemberModel found = null;
            ClassModel tmpClass = inClass;

            if (inClass == null)
            {
                if (result.InFile != null) FindMember(token, result.InFile, result, mask, acc);
                return;
            }
            else result.RelClass = inClass;
            // previous member accessed as an array
            if (token.Length >= 2 && token.First() == '[' && token.Last() == ']')
            {
                result.IsStatic = false;
                if (result.Type?.IndexType == null)
                {
                    result.Member = null;
                    result.InFile = null;
                    result.Type = ResolveType(context.Features.objectKey, null);
                }
                else
                {
                    result.Type = ResolveType(result.Type.IndexType, result.InFile);
                }
                return;
            }
            // previous member called as a method
            else if (token[0] == '#')
            {
                result.IsStatic = false;
                if (result.Member != null)
                {
                    if ((result.Member.Flags & FlagType.Constructor) > 0)
                        result.Type = inClass;
                    else result.Type = ResolveType(result.Member.Type, result.InFile);
                }
                return;
            }
            // variable
            else if (tmpClass != null)
            {
                // member
                tmpClass.ResolveExtends();
                while (!tmpClass.IsVoid())
                {
                    found = tmpClass.Members.Search(token, mask, acc);
                    // ignore setters
                    if (found != null && (found.Flags & FlagType.Setter) > 0)
                    {
                        found = null;
                        MemberList matches = tmpClass.Members.MultipleSearch(token, mask, acc);
                        foreach (MemberModel member in matches)
                        {
                            found = member;
                            if ((member.Flags & FlagType.Getter) > 0) break;
                        }
                    }
                    if (found != null)
                    {
                        result.Member = found;
                        // variable / getter
                        if ((found.Flags & FlagType.Function) == 0)
                        {
                            result.Type = ResolveType(found.Type, tmpClass.InFile);
                            result.IsStatic = false;
                        }
                        // constructor
                        else if ((found.Flags & FlagType.Constructor) > 0)
                        {
                            // is the constructor - ie. a Type
                            if (tmpClass != inClass) // constructor of inherited type
                            {
                                found = null;
                                result.Type = null;
                                result.Member = null;
                                break; 
                            }
                            result.Type = tmpClass;
                            result.IsStatic = true;
                        }
                        // in enum
                        else if ((found.Flags & FlagType.Enum) > 0)
                        {
                            result.Type = ResolveType(found.Type, tmpClass.InFile);
                        }
                        // method
                        else
                        {
                            result.Type = ResolveType("Function", null);
                            result.IsStatic = false;
                        }
                        break;
                    }
                    // Flash IDE-like typing
                    else if (tmpClass.Name == "MovieClip")
                    {
                        string autoType = null;
                        if (tmpClass.InFile.Version < 3)
                        {
                            if (token.EndsWithOrdinal("_mc") || token.StartsWithOrdinal("mc")) autoType = "MovieClip";
                            else if (token.EndsWithOrdinal("_txt") || token.StartsWithOrdinal("txt")) autoType = "TextField";
                            else if (token.EndsWithOrdinal("_btn") || token.StartsWithOrdinal("bt")) autoType = "Button";
                        }
                        else if (tmpClass.InFile.Version == 3) 
                        {
                            if (token.EndsWithOrdinal("_mc") || token.StartsWithOrdinal("mc")) autoType = "flash.display.MovieClip";
                            else if (token.EndsWithOrdinal("_txt") || token.StartsWithOrdinal("txt")) autoType = "flash.text.TextField";
                            else if (token.EndsWithOrdinal("_btn") || token.StartsWithOrdinal("bt")) autoType = "flash.display.SimpleButton";
                        }
                        if (autoType != null)
                        {
                            result.Type = ASContext.Context.ResolveType(autoType, null);
                            result.Member = new MemberModel(token, autoType, FlagType.Variable | FlagType.Dynamic | FlagType.AutomaticVar, Visibility.Public);
                            result.IsStatic = false;
                            return;
                        }
                    }

                    // static inheritance: only AS2 and Haxe typedefs inherit static members
                    if ((mask & FlagType.Static) > 0)
                    {
                        if (!features.hasStaticInheritance && (tmpClass.Flags & FlagType.TypeDef) == 0)
                            break; 
                    }

                    tmpClass = tmpClass.Extends;

                    if (acc == 0 && !tmpClass.IsVoid() && tmpClass.InFile.Version == 3)
                    {
                        acc = Visibility.Public | Visibility.Protected;
                        if (inClass.InFile.Package == tmpClass.InFile.Package) acc |= Visibility.Internal;
                    }
                }
            }

            // result found!
            if (found != null)
            {
                result.InClass = tmpClass;
                result.InFile = tmpClass.InFile;
                if (result.Type == null) 
                    result.Type = ASContext.Context.ResolveType(found.Type, tmpClass.InFile);
                return;
            }
            // try subpackages
            else if (inClass.InFile.TryAsPackage)
            {
                result.Type = ASContext.Context.ResolveType(inClass.Name + "." + token, null);
                if (!result.Type.IsVoid())
                    return;
            }

            // not found
            result.Type = null;
            result.Member = null;
        }

        #endregion

        #region main_code_parser
        private static List<string> ExtractedSubex;

        /// <summary>
        /// Find expression at cursor position
        /// </summary>
        /// <param name="sci">Scintilla Control</param>
        /// <param name="position">Cursor position</param>
        /// <returns></returns>
        internal static ASExpr GetExpression(ScintillaControl sci, int position) => GetExpression(sci, position, false);

        /// <summary>
        /// Find expression at cursor position
        /// </summary>
        /// <param name="sci">Scintilla Control</param>
        /// <param name="position">Cursor position</param>
        /// <param name="ignoreWhiteSpace">Skip whitespace at position</param>
        /// <returns></returns>
        private static ASExpr GetExpression(ScintillaControl sci, int position, bool ignoreWhiteSpace)
        {
            var context = ASContext.Context;
            var haXe = context.CurrentModel.haXe;
            var expression = new ASExpr();
            expression.Position = position;
            expression.Separator = " ";

            // file's member declared at this position
            expression.ContextMember = context.CurrentMember;
            int minPos = 0;
            if (expression.ContextMember != null)
            {
                minPos = sci.PositionFromLine(expression.ContextMember.LineFrom);
                StringBuilder sbBody = new StringBuilder();
                for (int i = expression.ContextMember.LineFrom; i <= expression.ContextMember.LineTo; i++)
                    sbBody.Append(sci.GetLine(i));
                var body = sbBody.ToString();

                var hasBody = FlagType.Function | FlagType.Constructor;
                if (!haXe) hasBody |= FlagType.Getter | FlagType.Setter;

                if ((expression.ContextMember.Flags & hasBody) > 0)
                {
                    expression.ContextFunction = expression.ContextMember;
                    expression.FunctionOffset = expression.ContextMember.LineFrom;

                    Match mStart = Regex.Match(body, "(\\)|[a-z0-9*.,-<>])\\s*{", RegexOptions.IgnoreCase);
                    if (mStart.Success)
                    {
                        // cleanup function body & offset
                        int pos = mStart.Index + mStart.Length - 1;
                        expression.BeforeBody = (position < sci.PositionFromLine(expression.ContextMember.LineFrom) + pos);
                        string pre = body.Substring(0, pos);
                        for (int i = 0; i < pre.Length - 1; i++)
                            if (pre[i] == '\r') { expression.FunctionOffset++; if (pre[i + 1] == '\n') i++; }
                            else if (pre[i] == '\n') expression.FunctionOffset++;
                        body = body.Substring(pos);
                    }
                    expression.FunctionBody = body;
                }
                else
                {
                    int eqPos = body.IndexOf('=');
                    expression.BeforeBody = (eqPos < 0 || position < sci.PositionFromLine(expression.ContextMember.LineFrom) + eqPos);
                }
            }

            // get the word characters from the syntax definition
            var characterClass = ScintillaControl.Configuration.GetLanguage(sci.ConfigurationLanguage).characterclass.Characters;

            // get expression before cursor
            var features = context.Features;
            var sb = new StringBuilder();
            var sbSub = new StringBuilder();
            int subCount = 0;
            char c = ' ';
            var startPosition = position;
            int positionExpression = position;
            int arrCount = 0;
            int parCount = 0;
            int genCount = 0;
            var braCount = 0;
            var dQuotes = 0;
            var sQuotes = 0;
            bool hasGenerics = features.hasGenerics;
            bool hadWS = false;
            bool hadDot = ignoreWhiteSpace;
            int dotCount = 0;
            bool inRegex = false;
            char dot = features.dot[features.dot.Length - 1];
            while (position > minPos)
            {
                position--;
                if (arrCount == 0 && braCount == 0 && parCount == 0 && context.CodeComplete.IsRegexStyle(sci, position))
                {
                    inRegex = true;
                    positionExpression = position;
                    continue;
                }
                var style = sci.BaseStyleAt(position);
                if (!IsCommentStyle(style))
                {
                    // end of regex literal
                    if (inRegex)
                    {
                        if (expression.SubExpressions == null) expression.SubExpressions = new List<string>();
                        expression.SubExpressions.Add("");
                        sb.Insert(0, "#RegExp.#" + (subCount++) + "~");
                        expression.Separator = ";";
                        break;
                    }
                    var c2 = c;
                    c = (char)sci.CharAt(position);
                    if ((dQuotes > 0 && c != '\"') || (sQuotes > 0 && c != '\''))
                    {
                        sbSub.Insert(0, c);
                        continue;
                    }
                    // array access
                    if (c == ']' && parCount == 0)
                    {
                        /**
                         * for example:
                         * var v = []
                         * v.<complete>
                         */
                        if (!hadDot && sb.Length > 0 && characterClass.Contains(sb[0]))
                        {
                            expression.Separator = ";";
                            break;
                        }
                        ignoreWhiteSpace = false;
                        if (arrCount == 0) // start sub-expression
                        {
                            if (expression.SubExpressions == null) expression.SubExpressions = new List<string>();
                            sbSub.Clear();
                        }
                        arrCount++;
                    }
                    else if (c == '[' && parCount == 0)
                    {
                        arrCount--;
                        if (arrCount == 0 && braCount == 0)
                        {
                            positionExpression = position;
                            if (parCount == 0)
                            {
                                sbSub.Insert(0, c);
                                expression.SubExpressions.Add(sbSub.ToString());
                                sbSub.Clear();
                                sb.Insert(0, ".#" + (subCount++) + "~");
                                var pos = position - 1;
                                var word = GetWordLeft(sci, ref pos);
                                // for example: return [].<complete>
                                if (context.Features.codeKeywords.Contains(word))
                                {
                                    expression.Separator = ";";
                                    expression.WordBefore = word;
                                    expression.WordBeforePosition = pos + 1;
                                    break;
                                }
                            }
                            continue;
                        }
                        if (arrCount < 0)
                        {
                            expression.Separator = ";";
                            break;
                        }
                    }
                    else if (c == '<' && hasGenerics && arrCount == 0 && parCount == 0)
                    {
                        genCount--;
                        if (genCount < 0)
                        {
                            expression.Separator = ";";
                            break;
                        }
                    }
                    else if (c == '>' && arrCount == 0 && parCount == 0)
                    {
                        if (haXe && position - 1 > minPos && (char)sci.CharAt(position - 1) == '-')
                        {
                        }
                        else if (hasGenerics)
                        {
                            if (c2 == '.' || c2 == ',' || c2 == '(' || c2 == '[' || c2 == '>' || c2 == '}' || c2 == ')' || position + 1 == startPosition)
                            {
                                genCount++;
                                var length = sb.Length;
                                if (length >= 3)
                                {
                                    var fc = sb[0];
                                    var sc = sb[1];
                                    var lc = sb[length - 1];
                                    if (fc == '.' && sc == '[' && (lc == ']' || (length >= 4 && sb[length - 2] == ']' && lc == '.')))
                                    {
                                        sbSub.Insert(0, sb.ToString(1, length - 1));
                                        sb.Clear();
                                    }
                                }
                            }
                            else break;
                        }
                    }
                    // ignore sub-expressions (method calls' parameters)
                    else if (c == '(')
                    {
                        parCount--;
                        if (parCount == 0 && arrCount == 0)
                        {
                            positionExpression = position;
                            sbSub.Insert(0, c);
                            expression.SubExpressions.Add(sbSub.ToString());
                            sbSub.Clear();
                            sb.Insert(0, ".#" + (subCount++) + "~"); // method call or sub expression
                            var pos = position - 1;
                            var word = GetWordLeft(sci, ref pos);
                            // AS3, AS2, Loom ex: return (a as B).<complete>
                            if (context.Features.codeKeywords.Contains(word))
                            {
                                expression.Separator = ";";
                                expression.WordBefore = word;
                                expression.WordBeforePosition = pos + 1;
                                break;
                            }
                            continue;
                        }
                        if (parCount < 0)
                        {
                            expression.Separator = ";";
                            int testPos = position - 1;
                            string testWord = GetWordLeft(sci, ref testPos); // anonymous function
                            var pos = testPos;
                            string testWord2 = GetWordLeft(sci, ref pos) ?? "null"; // regular function
                            if (testWord == features.functionKey || testWord == "catch"
                                || testWord2 == features.functionKey
                                || testWord2 == features.getKey || testWord2 == features.setKey)
                            {
                                expression.Separator = ",";
                                expression.coma = ComaExpression.FunctionDeclaration;
                            }
                            else
                            {
                                expression.WordBefore = testWord;
                                expression.WordBeforePosition = testPos + 1;
                            }
                            break;
                        }
                    }
                    else if (c == ')')
                    {
                        ignoreWhiteSpace = false;
                        if (!hadDot)
                        {
                            expression.Separator = ";";
                            break;
                        }
                        if (parCount == 0) // start sub-expression
                        {
                            if (expression.SubExpressions == null) expression.SubExpressions = new List<string>();
                            sbSub.Clear();
                        }
                        parCount++;
                    }
                    else if (genCount == 0 && arrCount == 0 && parCount == 0)
                    {
                        if (c == '}')
                        {
                            /**
                             * for example:
                             * var v = {}
                             * v.<complete>
                             */
                            if (!hadDot && sb.Length > 0 && characterClass.Contains(sb[0]))
                            {
                                expression.Separator = ";";
                                break;
                            }
                            if (!ignoreWhiteSpace && hadWS)
                            {
                                expression.Separator = ";";
                                break;
                            }
                            braCount++;
                        }
                        else if (c == '{')
                        {
                            braCount--;
                            if (braCount == 0)
                            {
                                positionExpression = position;
                                sb.Insert(0, "{}");
                                expression.Separator = ";";
                                continue;
                            }
                            if (braCount < 0)
                            {
                                expression.Separator = ";";
                                break;
                            }
                        }
                        else if (braCount == 0)
                        {
                            if (c == '\"' && sQuotes == 0)
                            {
                                if (position == 0 || (char)sci.CharAt(position - 1) == '\\') continue;
                                if (dQuotes == 0) dQuotes++;
                                else
                                {
                                    dQuotes--;
                                    if (dQuotes == 0)
                                    {
                                        positionExpression = position;
                                        expression.Separator = ";";
                                        if (expression.SubExpressions != null)
                                        {
                                            sbSub.Insert(0, c);
                                            sb.Insert(0, sbSub.ToString());
                                            break;
                                        }
                                        sb.Insert(0, string.Concat(c, sbSub, c));
                                        continue;
                                    }
                                }
                                if (hadDot)
                                {
                                    sbSub.Clear();
                                    sbSub.Insert(0, c);
                                    if (expression.SubExpressions == null) expression.SubExpressions = new List<string>();
                                    expression.SubExpressions.Add(string.Empty);
                                    sb.Insert(0, ".#" + (subCount++) + "~");
                                }
                                ignoreWhiteSpace = false;
                                continue;
                            }
                            if (c == '\'' && dQuotes == 0)
                            {
                                if (position == 0 || (char)sci.CharAt(position - 1) == '\\') continue;
                                if (sQuotes == 0) sQuotes++;
                                else
                                {
                                    sQuotes--;
                                    if (sQuotes == 0)
                                    {
                                        positionExpression = position;
                                        expression.Separator = ";";
                                        if (expression.SubExpressions != null)
                                        {
                                            sbSub.Insert(0, c);
                                            sb.Insert(0, sbSub.ToString());
                                            break;
                                        }
                                        sb.Insert(0, string.Concat(c, sbSub, c));
                                        continue;
                                    }
                                }
                                if (hadDot)
                                {
                                    sbSub.Clear();
                                    sbSub.Insert(0, c);
                                    if (expression.SubExpressions == null) expression.SubExpressions = new List<string>();
                                    expression.SubExpressions.Add(string.Empty);
                                    sb.Insert(0, ".#" + (subCount++) + "~");
                                }
                                ignoreWhiteSpace = false;
                                continue;
                            }
                        }
                    }
                    if (parCount > 0 || arrCount > 0 || genCount > 0 || braCount > 0 || dQuotes > 0 || sQuotes > 0) 
                    {
                        if (c == ';') // not expected: something's wrong
                        {
                            expression.Separator = ";";
                            break;
                        }
                        // build sub expression
                        sbSub.Insert(0, c);
                        continue;
                    }
                    // build expression
                    if (c <= 32)
                    {
                        if (genCount == 0) hadWS = true;
                        else
                        {
                            sb.Insert(sb.Length, sbSub.ToString().ToCharArray());
                            positionExpression = position + 1;
                            expression.Separator = " ";
                            break;
                        }
                    }
                    else if (c == dot)
                    {
                        if (features.dot.Length == 2) hadDot = position > 0 && sci.CharAt(position - 1) == features.dot[0];
                        else
                        {
                            hadDot = true;
                            if (++dotCount == 3) // haxe's triple dot in for()
                            {
                                sb.Remove(0, 2);
                                break;
                            }
                        }
                        sb.Insert(0, c);
                    }
                    else if (c == ';')
                    {
                        if(expression.Separator == " ") expression.Separator = ";";
                        break;
                    }
                    else if (hasGenerics && c == '<')
                    {
                        sbSub.Insert(0, c);
                        if (genCount < 0
                            && sci.ConfigurationLanguage == "as3"
                            && position > minPos && sci.CharAt(position - 1) != '.')
                        {
                            positionExpression = position;
                            position--;
                            expression.Separator = " ";
                            break;
                        }
                        genCount--;
                        if (subCount > 0 || genCount < 0)
                        {
                            sb.Insert(0, sbSub);
                            sbSub.Clear();
                        }
                    }
                    else if (c == '{')
                    {
                        expression.coma = DisambiguateComa(sci, position, minPos);
                        if (expression.Separator == " ") expression.Separator = (expression.coma == ComaExpression.None) ? ";" : ",";
                        if (expression.coma == ComaExpression.AnonymousObjectParam)
                        {
                            positionExpression = position;
                            sb.Append(c);
                            braCount++;
                            position++;
                            int endPos;
                            if (expression.ContextFunction != null) endPos = sci.LineEndPosition(expression.ContextFunction.LineTo);
                            else endPos = sci.LineEndPosition(expression.ContextMember.LineTo);
                            while (position < endPos)
                            {
                                style = sci.BaseStyleAt(position);
                                if (!IsCommentStyle(style))
                                {
                                    c = (char) sci.CharAt(position);
                                    sb.Append(c);
                                    if (c == '}')
                                    {
                                        if (--braCount == 0) break;
                                    }
                                    else if (c == '{') braCount++;
                                }
                                position++;
                            }
                        }
                        break;
                    }
                    else if (c == ',')
                    {
                        expression.coma = DisambiguateComa(sci, position, minPos);
                        expression.Separator = (expression.coma == ComaExpression.None) ? ";" : ",";
                        break;
                    }
                    else if (c == ':')
                    {
                        expression.Separator = ":";
                        break;
                    }
                    else if (features.ArithmeticOperators.Contains(c))
                    {
                        expression.SeparatorPosition = position;
                        var p = position - 1;
                        var curOp = c.ToString();
                        foreach (var op in features.IncrementDecrementOperators)
                        {
                            while (op.StartsWithOrdinal(curOp) && p > minPos)
                            {
                                var cc = (char)sci.CharAt(p);
                                if (cc == c) curOp += cc;
                                else break;
                                p--;
                            }
                            if (curOp.Length > 1)
                            {
                                var buf = string.Empty;
                                while (p > minPos)
                                {
                                    if (sci.PositionIsOnComment(p))
                                    {
                                        p--;
                                        continue;
                                    }
                                    var cc = (char)sci.CharAt(p--);
                                    if (cc <= ' ') buf += cc;
                                    else if (features.ArithmeticOperators.Contains(cc))
                                    {
                                        buf = cc + buf;
                                        break;
                                    }
                                    else
                                    {
                                        buf = string.Empty;
                                        break;
                                    }
                                }
                                if (buf.Length > 0) curOp = buf + curOp;
                                break;
                            }
                        }
                        if (sb.Length != 0)
                        {
                            expression.Separator = curOp;
                            expression.SeparatorPosition -= curOp.Length - 1;
                            break;
                        }
                        position -= curOp.Length - 1;
                        expression.RightOperator = curOp;
                        hadDot = true;
                    }
                    else if (characterClass.Contains(c))
                    {
                        if (hadWS && !hadDot)
                        {
                            expression.Separator = " ";
                            break;
                        }
                        hadWS = false;
                        hadDot = false;
                        dotCount = 0;
                        sb.Insert(0, c);
                        positionExpression = position;
                    }
                    else //if (hadWS && !hadDot)
                    {
                        if (hadDot && features.SpecialPostfixOperators.Contains(c))
                        {
                            sb.Insert(0, $".{c}");
                            continue;
                        }
                        if (c == '\'' || c == '"') expression.Separator = "\"";
                        break;
                    }
                }
                // string literals only allowed in sub-expressions
                else
                {
                    if (parCount == 0 && !hadDot) // not expected: something's wrong
                    {
                        expression.Separator = ";";
                        break;
                    }
                }
            }
            
            var value = sb.ToString().TrimStart('.');
            if (features.hasE4X && value.Length >= 2 && value[0] == '<' && value[value.Length - 1]  == '>')
            {
                expression.Separator = ";";
                value = "</>";
            }

            // check if there is a particular keyword
            if (expression.Separator == " " && position > 0)
            {
                var pos = position;
                expression.WordBefore = GetWordLeft(sci, ref pos);
                if (expression.WordBefore.Length > 0)
                {
                    position = pos;
                    expression.WordBeforePosition = position + 1;
                }
            }
            if (expression.Separator == " " || (expression.Separator == ";" && sci.CharAt(position) != ';'))
            {
                var @operator = GetOperatorLeft(sci, ref position);
                if (@operator.Length > 0)
                {
                    expression.Separator = @operator;
                    expression.SeparatorPosition = position + 1;
                }
            }

            expression.Value = value;
            expression.PositionExpression = positionExpression;
            expression.LineFrom = sci.LineFromPosition(positionExpression);
            expression.LineTo = sci.LineFromPosition(expression.Position);
            LastExpression = expression;
            return expression;
        }

        /// <summary>
        /// Find out in what context is a coma-separated expression
        /// </summary>
        /// <returns></returns>
        internal static ComaExpression DisambiguateComa(ScintillaControl sci, int position, int minPos)
        {
            ContextFeatures features = ASContext.Context.Features;
            // find block start '(' or '{'
            int parCount = 0;
            int braceCount = 0;
            int sqCount = 0;
            while (position > minPos)
            {
                var c = (char)sci.CharAt(position);
                if (c == ';')
                {
                    return ComaExpression.None;
                }
                // var declaration
                else if (c == ':')
                {
                    position--;
                    string word = GetWordLeft(sci, ref position);
                    word = GetWordLeft(sci, ref position);
                    if (word == features.varKey) return ComaExpression.VarDeclaration;
                    else continue;
                }
                // Array values
                else if (c == '[')
                {
                    sqCount--;
                    if (sqCount < 0)
                    {
                        return ComaExpression.ArrayValue;
                    }
                }
                else if (c == ']') sqCount++;
                else if (c == '(')
                {
                    parCount--;
                    if (parCount < 0)
                    {
                        position--;
                        string word1 = GetWordLeft(sci, ref position);
                        if (word1 == "" && sci.CharAt(position) == '>' && features.hasGenerics)
                        {
                            // Generic function: function generic<K>(arg:K)
                            int groupCount = 1;
                            position--;
                            while (position >= 0 && groupCount > 0)
                            {
                                c = (char) sci.CharAt(position);
                                if ("({[<".Contains(c)) groupCount--;
                                else if (")}]>".Contains(c)) groupCount++;
                                position--;
                            }
                            word1 = GetWordLeft(sci, ref position);
                        }
                        if (word1 == features.functionKey) return ComaExpression.FunctionDeclaration; // anonymous function
                        var word2 = GetWordLeft(sci, ref position);
                        if (word2 == features.functionKey || word2 == features.setKey || word2 == features.getKey)
                            return ComaExpression.FunctionDeclaration; // function declaration
                        if (features.hasDelegates && word2 == "delegate")
                            return ComaExpression.FunctionDeclaration; // delegate declaration
                        return ComaExpression.FunctionParameter; // function call
                    }
                }
                else if (c == ')') parCount++;
                else if (c == '{')
                {
                    braceCount--;
                    if (braceCount < 0)
                    {
                        position--;
                        string word1 = GetWordLeft(sci, ref position);
                        c = (word1.Length > 0) ? word1[word1.Length - 1] : (char) sci.CharAt(position);
                        if (":,(=".Contains(c))
                        {
                            string line = sci.GetLine(sci.LineFromPosition(position));
                            //TODO: Very limited check, the case|default could be in a previous line, or it could be something else in the same line
                            if (Regex.IsMatch(line, @"\b(case|default)\b.*:")) break; // case: code block
                            if (c == ':' && sci.ConfigurationLanguage == "haxe")
                            {
                                // Anonymous structures
                                ComaExpression coma = DisambiguateComa(sci, position, minPos);
                                if (coma == ComaExpression.FunctionDeclaration || coma == ComaExpression.VarDeclaration)
                                {
                                    return ComaExpression.VarDeclaration;
                                }
                            }
                            return ComaExpression.AnonymousObjectParam;
                        }
                        if (c != ')' && c != '}' && !char.IsLetterOrDigit(c)) return ComaExpression.AnonymousObject;
                        break;
                    }
                }
                else if (c == '}')
                {
                    braceCount++;
                }
                else if (c == '?')
                {
                    //TODO: Change to ASContext.Context.CurrentModel
                    if (sci.ConfigurationLanguage == "haxe") // Haxe optional fields
                    {
                        ComaExpression coma = DisambiguateComa(sci, position - 1, minPos);
                        if (coma == ComaExpression.FunctionDeclaration)
                        {
                            // Function optional argument
                            return coma;
                        }
                        else if (coma == ComaExpression.VarDeclaration)
                        {
                            // Possible anonymous structure optional field. Check we are not in a ternary operator
                            position--;
                            string word1 = GetWordLeft(sci, ref position);
                            c = (word1.Length > 0) ? word1[word1.Length - 1] : (char) sci.CharAt(position);
                            if (c == ',' || c == '{') return coma;
                        }
                    }
                    return ComaExpression.AnonymousObject;
                }
                position--;
            }
            return ComaExpression.None;
        }

        /// <summary>
        /// Parse function body for local var definitions
        /// TODO  ASComplete: parse coma separated local vars definitions
        /// </summary>
        /// <param name="expression">Expression source</param>
        /// <returns>Local vars dictionary (name, type)</returns>
        public static MemberList ParseLocalVars(ASExpr expression)
        {
            FileModel model;
            if (!string.IsNullOrEmpty(expression.FunctionBody))
            {
                var cm = expression.ContextMember;
                var functionBody = Regex.Replace(expression.FunctionBody, "function\\s*\\(", "function __anonfunc__("); // name anonymous functions
                model = ASContext.Context.GetCodeModel(functionBody, true);
                var memberCount = model.Members.Count;
                for (var memberIndex = 0; memberIndex < memberCount; memberIndex++)
                {
                    var member = model.Members[memberIndex];

                    if (cm.Equals(member)) continue;

                    member.Flags |= FlagType.LocalVar;
                    member.LineFrom += expression.FunctionOffset;
                    member.LineTo += expression.FunctionOffset;

                    if ((member.Flags & FlagType.Function) != FlagType.Function) continue;
                    if (member.Name == "__anonfunc__")
                    {
                        model.Members.Remove(member);
                        memberCount--;
                        memberIndex--;
                    }
                    if (member.Parameters == null) continue;
                    foreach (var parameter in member.Parameters)
                    {
                        parameter.LineFrom += expression.FunctionOffset;
                        parameter.LineTo += expression.FunctionOffset;
                        model.Members.Add(parameter);
                    }
                }
            }
            else model = new FileModel();
            model.Members.Sort();
            if (expression.ContextFunction?.Parameters != null)
            {
                var features = ASContext.Context.Features;
                var dot = features.dot;
                foreach (var item in expression.ContextFunction.Parameters)
                {
                    var name = item.Name;
                    if (name.StartsWithOrdinal(dot)) model.Members.MergeByLine(new MemberModel(name.Substring(name.LastIndexOfOrdinal(dot) + 1), "Array", item.Flags, item.Access));
                    else if (name[0] == '?') model.Members.MergeByLine(new MemberModel(name.Substring(1), item.Type, item.Flags, item.Access));
                    else model.Members.MergeByLine(item);
                }
                if (features.functionArguments != null) model.Members.MergeByLine(features.functionArguments);
            }
            return model.Members;
        }

        /// <summary>
        /// Extract sub-expressions
        /// </summary>
        private static string ExtractSubex(Match m)
        {
            ExtractedSubex.Add(m.Value);
            return ".#" + (ExtractedSubex.Count - 1) + "~";
        }
        #endregion

        #region tools_functions

        /// <summary>
        /// Text style is a literal.
        /// </summary>
        public static bool IsLiteralStyle(int style)
        {
            return IsNumericStyle(style) || IsStringStyle(style) || IsCharStyle(style);
        }

        /// <summary>
        /// Text style is a numeric literal.
        /// </summary>
        public static bool IsNumericStyle(int style)
        {
            return style == 4;
        }

        /// <summary>
        /// Text style is a string literal.
        /// </summary>
        public static bool IsStringStyle(int style)
        {
            return style == 6;
        }

        /// <summary>
        /// Text style is character literal.
        /// </summary>
        public static bool IsCharStyle(int style)
        {
            return style == 7;
        }

        /// <summary>
        /// Text is word 
        /// </summary>
        public static bool IsTextStyle(int style)
        {
            return style == 0 || style == 10 /*punctuation*/ || style == 11 /*identifier*/
                || style == 16 /*word2 (secondary keywords: class name)*/
                || style == 24 /*word4 (add keywords4)*/ || style == 25 /*word5 (add keywords5)*/
                || style == 127 /*PHP*/;
        }

        /// <summary>
        /// Text is word or keyword
        /// </summary>
        public static bool IsTextStyleEx(int style)
        {
            return style == 0 || style == 5 /*word (secondary keywords)*/
                || style == 10 /*punctuation*/ || style == 11 /*identifier*/
                || style == 16 /*word2 (secondary keywords: class name)*/
                || style == 19 /*globalclass (primary keywords)*/ || style == 23 /*word3 (add keywords3)*/
                || style == 24 /*word4 (add keywords4)*/ || style == 25 /*word5 (add keywords5)*/
                || style == 127 /*PHP*/;
        }

        public static bool IsCommentStyle(int style)
        {
            return style == 1 || style == 2 || style == 3 /*comments*/
                || style == 17 || style == 18 /*javadoc tags*/;
        }

        public virtual bool IsRegexStyle(ScintillaControl sci, int position) => sci.BaseStyleAt(position) == 14;

        public static string GetWordLeft(ScintillaControl sci, ref int position)
        {
            // get the word characters from the syntax definition
            string characterClass = ScintillaControl.Configuration.GetLanguage(sci.ConfigurationLanguage).characterclass.Characters;
            string word = "";
            //string exclude = "(){};,+*/\\=:.%\"<>";
            bool skipWS = true;
            while (position >= 0)
            {
                var style = sci.BaseStyleAt(position);
                if (IsTextStyleEx(style))
                {
                    var c = (char)sci.CharAt(position);
                    if (c <= ' ')
                    {
                        if (!skipWS)
                            break;
                    }
                    else if (!characterClass.Contains(c)) break;
                    else if (style != 6)
                    {
                        word = c + word;
                        skipWS = false;
                    }
                }
                position--;
            }
            return word;
        }

        static string GetOperatorLeft(ScintillaControl sci, ref int position)
        {
            var result = string.Empty;
            var skipWS = true;
            while (position >= 0)
            {
                var c = (char)sci.CharAt(position);
                if (char.IsDigit(c)) break;
                var style = sci.BaseStyleAt(position);
                if (IsTextStyleEx(style))
                {
                    if (c <= ' ')
                    {
                        if (!skipWS) break;
                    }
                    else if (char.IsLetterOrDigit(c)
                        || c == '.' || c == ',' || c == ':' || c == ';' || c == '_' || c == '$'
                        || c == '"' || c == '\''
                        || c == ')' || c == '('
                        || c == ']' || c == '['
                        || c == '}' || c == '{') break;
                    else
                    {
                        skipWS = false;
                        result = c + result;
                    }
                }
                --position;
            }
            return result;
        }

        public static ASResult GetExpressionType(ScintillaControl sci, int position) => GetExpressionType(sci, position, true);

        public static ASResult GetExpressionType(ScintillaControl sci, int position, bool filterVisibility) => GetExpressionType(sci, position, filterVisibility, false);

        public static ASResult GetExpressionType(ScintillaControl sci, int position, bool filterVisibility, bool ignoreWhiteSpace)
        {
            // context
            int line = sci.LineFromPosition(position);
            if (line != ASContext.Context.CurrentLine) 
                ASContext.Context.UpdateContext(line);
            try
            {
                var expr = GetExpression(sci, position, ignoreWhiteSpace);
                expr.LocalVars = ParseLocalVars(expr);
                if (string.IsNullOrEmpty(expr.Value)) return new ASResult {Context = expr};
                var aFile = ASContext.Context.CurrentModel;
                var aClass = ASContext.Context.CurrentClass;
                // Expression before cursor
                return ASContext.Context.CodeComplete.EvalExpression(expr.Value, expr, aFile, aClass, true, false, filterVisibility);
            }
            finally
            {
                // restore context
                if (line != ASContext.Context.CurrentLine) 
                    ASContext.Context.UpdateContext(ASContext.Context.CurrentLine);
            }
        }

        private static MemberList GetTypeParameters(MemberModel model)
        {
            MemberList retVal = null;
            string template = model.Template;
            if (template != null && template.StartsWith("<"))
            {
                var sb = new StringBuilder();
                int groupCount = 0;
                bool inConstraint = false;
                MemberModel genType = null;
                for (int i = 1, count = template.Length - 1; i < count; i++)
                {
                    char c = template[i];
                    if (!inConstraint)
                    {
                        if (c == ':' || c == ',')
                        {
                            genType = new MemberModel();
                            genType.Name = sb.ToString();
                            genType.Type = sb.ToString();
                            genType.Flags = FlagType.TypeDef;
                            inConstraint = c == ':';
                            if (retVal == null) retVal = new MemberList();
                            retVal.Add(genType);
                            sb.Length = 0;

                            continue;
                        }
                        else if (char.IsWhiteSpace(c)) continue;
                        sb.Append(c);
                    }
                    else
                    {
                        if (c == ',')
                        {
                            if (groupCount == 0)
                            {
                                genType.Type += ":" + sb;
                                genType = null;
                                inConstraint = false;
                                sb.Length = 0;
                                continue;
                            }
                        }
                        else if ("({[<".Contains(c)) groupCount++;
                        else if (")}]>".Contains(c)) groupCount--;
                        sb.Append(c);
                    }
                }
                if (sb.Length > 0)
                {
                    if (retVal == null) retVal = new MemberList();
                    if (!inConstraint)
                    {
                        var name = sb.ToString();
                        retVal.Add(new MemberModel {Name = name, Type = name, Flags = FlagType.TypeDef});
                    }
                    else genType.Type += ":" + sb;
                }
            }

            return retVal;
        }

        private static List<ICompletionListItem> GetAllClasses(ScintillaControl sci, bool classesOnly, bool showClassVars)
        {
            MemberList known = ASContext.Context.GetAllProjectClasses();
            if (known.Count == 0) return null;

            // get local Class vars
            if (showClassVars)
            {
                MemberList found = new MemberList();

                ASExpr expr = GetExpression(sci, sci.CurrentPos);
                if (expr.Value != null)
                {
                    MemberList locals = ParseLocalVars(expr);
                    foreach (MemberModel local in locals)
                        if (local.Type == "Class")
                            found.Add(local);
                }

                if (found.Count > 0)
                {
                    found.Sort();
                    found.Merge(known);
                    known = found;
                }
            }

            if (!ASContext.Context.CurrentClass.IsVoid())
            {
                if (ASContext.Context.Features.hasDelegates)
                {
                    MemberList delegates = new MemberList();

                    foreach (MemberModel field in ASContext.Context.CurrentClass.Members)
                        if ((field.Flags & FlagType.Delegate) > 0)
                            delegates.Add(field);

                    if (delegates.Count > 0)
                    {
                        delegates.Sort();
                        delegates.Merge(known);
                        known = delegates;
                    }
                }

                if (ASContext.Context.Features.hasGenerics)
                {
                    var typeParams = GetVisibleTypeParameters();

                    if (typeParams != null && typeParams.Items.Count > 0)
                    {
                        typeParams.Sort();
                        typeParams.Merge(known);
                        known = typeParams;
                    }
                }
            }

            var list = new List<ICompletionListItem>();
            string prev = null;
            FlagType mask = (classesOnly) ?
                FlagType.Class | FlagType.Interface | FlagType.Enum | FlagType.Delegate | FlagType.Struct | FlagType.TypeDef
                : (FlagType)uint.MaxValue;
            foreach (MemberModel member in known)
            {
                if ((member.Flags & mask) == 0 || prev == member.Name)
                    if (!showClassVars || member.Type != "Class") continue;
                prev = member.Name;
                list.Add(new MemberItem(member));
            }

            return list;
        }

        private static MemberList GetVisibleElements()
        {
            MemberList known = ASContext.Context.GetVisibleExternalElements();

            if (ASContext.Context.Features.hasGenerics && !ASContext.Context.CurrentClass.IsVoid())
            {
                var typeParams = GetVisibleTypeParameters();

                if (typeParams != null && typeParams.Count > 0)
                {
                    typeParams.Sort();
                    typeParams.Merge(known);

                    known = typeParams;
                }
            }

            return known;
        }

        private static MemberList GetVisibleTypeParameters()
        {
            var typeParams = GetTypeParameters(ASContext.Context.CurrentClass);

            var curMember = ASContext.Context.CurrentMember;
            if (curMember != null && (curMember.Flags & FlagType.Function) > 0)
            {
                var memberTypeParams = GetTypeParameters(curMember);
                if (typeParams != null && memberTypeParams != null)
                    typeParams.Add(memberTypeParams);
                else if (typeParams == null)
                    typeParams = memberTypeParams;
            }

            return typeParams;
        }

        /// <summary>
        /// Returns whether or not position is inside of an expression block in String interpolation
        /// <param name="sci">Scintilla Control</param>
        /// <param name="position">Cursor position</param>
        /// </summary>
        public virtual bool IsStringInterpolationStyle(ScintillaControl sci, int position) => false;

        protected bool IsEscapedCharacter(ScintillaControl sci, int position, char escapeChar = '\\')
        {
            bool result = false;
            for (int i = position - 1; i >= 0; i--)
            {
                if (sci.CharAt(i) != escapeChar) break;
                result = !result;
            }
            return result;
        }

        private static bool IsMatchingQuote(char quote, int style)
        {
            return quote == '"' && IsStringStyle(style) || quote == '\'' && IsCharStyle(style);
        }

        /// <summary>
        /// Whether the character at the position is inside of the
        /// brackets of haxe metadata (@:allow(path) etc)
        /// </summary>
        private static bool IsMetadataArgument(ScintillaControl sci, int position)
        {
            if (!ASContext.Context.CurrentModel.haXe || ASContext.Context.CurrentMember != null)
                return false;

            char next = (char)sci.CharAt(position);
            bool openingBracket = false;

            for (int i = position; i > 0; i--)
            {
                var c = next;
                next = (char)sci.CharAt(i);

                if (c == ')' || c == '}' || c == ';')
                    return false;
                if (c == '(')
                    openingBracket = true;
                if (openingBracket && c == ':' && next == '@')
                    return true;
            }
            return false;
        }

        private static bool IsXmlType(ClassModel model)
        {
            return model != null && (model.QualifiedName == "XML" || model.QualifiedName == "XMLList");
        }

        public static int ExpressionEndPosition(ScintillaControl sci, int position) => ExpressionEndPosition(sci, position, false);

        public static int ExpressionEndPosition(ScintillaControl sci, int position, bool skipWhiteSpace)
        {
            var member = ASContext.Context.CurrentMember;
            var endPosition = member != null ? sci.LineEndPosition(member.LineTo) : sci.TextLength;
            return ExpressionEndPosition(sci, position, endPosition, skipWhiteSpace);
        }

        public static int ExpressionEndPosition(ScintillaControl sci, int startPos, int endPos) => ExpressionEndPosition(sci, startPos, endPos, false);

        public static int ExpressionEndPosition(ScintillaControl sci, int startPos, int endPos, bool skipWhiteSpace)
        {
            var ctx = ASContext.Context;
            var result = startPos;
            var statementEnd = startPos;
            var characterClass = ScintillaControl.Configuration.GetLanguage(sci.ConfigurationLanguage).characterclass.Characters;
            var parCount = 0;
            var brCount = 0;
            var arrCount = 0;
            var hadWS = false;
            var stop = false;
            sci.Colourise(0, -1);
            while (statementEnd < endPos)
            {
                if (sci.PositionIsOnComment(statementEnd))
                {
                    statementEnd++;
                    continue;
                }
                if (sci.PositionIsInString(statementEnd) && !ctx.CodeComplete.IsStringInterpolationStyle(sci, statementEnd))
                {
                    statementEnd++;
                    result = statementEnd;
                    continue;
                }
                var c = (char) sci.CharAt(statementEnd++);
                if (c == '(' && arrCount == 0) parCount++;
                else if (c == ')' && arrCount == 0)
                {
                    parCount--;
                    if (parCount == 0) result = statementEnd;
                    if (parCount < 0) break;
                }
                else if (c == '{' && parCount == 0 && arrCount == 0)
                {
                    if (stop) break;
                    brCount++;
                }
                else if (c == '}' && parCount == 0 && arrCount == 0)
                {
                    brCount--;
                    if (brCount == 0) result = statementEnd;
                    if (brCount < 0) break;
                }
                else if (c == '[' && parCount == 0) arrCount++;
                else if (c == ']' && parCount == 0)
                {
                    arrCount--;
                    if (arrCount == 0) result = statementEnd;
                    if (arrCount < 0) break;
                }
                else if (parCount == 0 && arrCount == 0 && brCount == 0)
                {
                    if (characterClass.Contains(c))
                    {
                        if (skipWhiteSpace)
                        {
                            skipWhiteSpace = false;
                            hadWS = false;
                        }
                        else if (hadWS) break;
                        stop = true;
                        result = statementEnd;
                    }
                    else if (c <= ' ') hadWS = true;
                    else break;
                }
            }
            return result;
        }

        /// <summary>
        /// Returns true if position is before body of class or member
        /// </summary>
        public bool PositionIsBeforeBody(ScintillaControl sci, int position, MemberModel member)
        {
            var groupCount = 0;
            var positionFrom = sci.PositionFromLine(member.LineFrom);
            for (var i = positionFrom; i < position; i++)
            {
                if (sci.PositionIsOnComment(position)) continue;
                var c = (char)sci.CharAt(i);
                if (c == '(' || c == '<') groupCount++;
                else if (c == ')' || (c == '>' && sci.CharAt(i - 1) != '-')) groupCount--;
                else if (c == '{' && groupCount == 0) return false;
            }
            return true;
        }

        #endregion

        #region tooltips formatting
        static public string GetCodeTipCode(ASResult result)
        {
            if (result.Member == null)
            {
                return result.Type?.ToString();
            }

            var file = GetFileContents(result.InFile);
            if (string.IsNullOrEmpty(file))
            {
                return MemberTooltipText(result.Member, ClassModel.VoidClass);
            }

            int eolMode = LineEndDetector.DetectNewLineMarker(file, (Int32)PluginBase.MainForm.Settings.EOLMode);
            var eolMarker = LineEndDetector.GetNewLineMarker(eolMode);
            var lines = file.Split(new[] { eolMarker }, StringSplitOptions.None);
            var code = new StringBuilder();
            for (var index = result.Member.LineFrom; index < result.Member.LineTo; index++)
            {
                code.AppendLine(lines[index]);
            }
            code.Append(lines[result.Member.LineTo]);
            return code.ToString();
        }

        static private string GetFileContents(FileModel model)
        {
            if (model != null && model.FileName.Length > 0 && File.Exists(model.FileName))
            {
                foreach (ITabbedDocument doc in PluginBase.MainForm.Documents)
                {
                    if (doc.IsEditable && doc.FileName.ToUpper() == model.FileName.ToUpper())
                    {
                        return doc.SciControl.Text;
                    }
                }
                var info = FileHelper.GetEncodingFileInfo(model.FileName);
                return info?.Contents;
            }
            return null;
        }

        public static string GetToolTipText(ASResult result)
        {
            if (result.Member != null && result.InClass != null)
            {
                return MemberTooltipText(result.Member, result.InClass) + GetToolTipDoc(result.Member);
            }
            if (result.Member != null && (result.Member.Flags & FlagType.Constructor) != FlagType.Constructor)
            {
                return MemberTooltipText(result.Member, ClassModel.VoidClass) + GetToolTipDoc(result.Member);
            }
            if (result.InClass != null)
            {
                return ClassModel.ClassDeclaration(result.InClass) + GetToolTipDoc(result.InClass);
            }
            if (result.Type != null && result.Context.WordBefore == "new") return ASContext.Context.CodeComplete.GetConstructorTooltipText(result.Type);
            return null;
        }

        protected virtual string GetConstructorTooltipText(ClassModel type)
        {
            var name = type.Name;
            var member = type.Members.Search(name, FlagType.Constructor, 0);
            if (member == null) member = new MemberModel(name, name, FlagType.Access | FlagType.Function | FlagType.Constructor, Visibility.Public);
            return MemberTooltipText(member, type) + GetToolTipDoc(member);
        }

        protected static string GetToolTipDoc(MemberModel model)
        {
            string details = (UITools.Manager.ShowDetails) ? ASDocumentation.GetTipFullDetails(model, null) : ASDocumentation.GetTipShortDetails(model, null);
            return details.TrimStart(' ', '\u2026');
        }

        protected static string MemberTooltipText(MemberModel member, ClassModel inClass)
        {
            // modifiers
            var ft = member.Flags;
            var modifiers = "";
            if ((ft & FlagType.Class) == 0)
            {
                if ((ft & FlagType.LocalVar) > 0) modifiers += "(local) ";
                else if ((ft & FlagType.ParameterVar) > 0) modifiers += "(parameter) ";
                else if ((ft & FlagType.AutomaticVar) > 0) modifiers += "(auto) ";
                else
                {
                    if ((ft & FlagType.Extern) > 0)
                        modifiers += "extern ";
                    if ((ft & FlagType.Native) > 0)
                        modifiers += "native ";
                    if ((ft & FlagType.Static) > 0)
                        modifiers += "static ";
                    var acc = member.Access;
                    if ((acc & Visibility.Private) > 0)
                        modifiers += "private ";
                    else if ((acc & Visibility.Public) > 0)
                        modifiers += "public ";
                    else if ((acc & Visibility.Protected) > 0)
                        modifiers += "protected ";
                    else if ((acc & Visibility.Internal) > 0)
                        modifiers += "internal ";
                }
            }
            // signature
            var foundIn = "";
            if (inClass != ClassModel.VoidClass)
            {
                var themeForeColor = PluginBase.MainForm.GetThemeColor("MethodCallTip.InfoColor");
                var foreColorString = themeForeColor != Color.Empty ? ColorTranslator.ToHtml(themeForeColor) : "#666666:MULTIPLY";
                foundIn = "\n[COLOR=" + foreColorString + "]in " + MemberModel.FormatType(inClass.QualifiedName) + "[/COLOR]";
            }
            if ((ft & (FlagType.Getter | FlagType.Setter)) > 0) return $"{modifiers}property {member}{foundIn}";
            if (ft == FlagType.Function) return $"{modifiers}function {member}{foundIn}";
            if ((ft & FlagType.Namespace) > 0) return $"{modifiers}namespace {member.Name}{foundIn}";
            if ((ft & FlagType.Constant) > 0)
            {
                if (member.Value == null) return $"{modifiers}const {member}{foundIn}";
                return $"{modifiers}const {member} = {member.Value}{foundIn}";
            }
            if ((ft & FlagType.Variable) > 0) return $"{modifiers}var {member}{foundIn}";
            if ((ft & FlagType.Delegate) > 0) return $"{modifiers}delegate {member}{foundIn}";
            return $"{modifiers}{member}{foundIn}";
        }
        #endregion

        #region automatic code generation

        private static ASExpr LastExpression;

        /// <summary>
        /// When typing a fully qualified class name:
        /// - automatically insert import statement 
        /// - replace with short name
        /// </summary>
        internal static void HandleCompletionInsert(ScintillaControl sci, int position, string text, char trigger, ICompletionListItem item)
        {
            // if the current class hash was set, we want to store whatever the user selected as the last-completed member for this class.
            if (currentClassHash != null)
            {
                completionHistory[currentClassHash] = text;
            }

            if (!ASContext.Context.IsFileValid)
                return;
            // let the context handle the insertion
            if (ASContext.Context.OnCompletionInsert(sci, position, text, trigger))
                return;
            // event inserted
            if (item is EventItem)
            {
                SmartEventInsertion(sci, position + text.Length, item);
                return;
            }

            // default handling
            if (ASContext.Context.Settings != null)
            {
                int textEndPosition = position + text.Length;
                // was a fully qualified type inserted?
                ASExpr expr = GetExpression(sci, textEndPosition);
                if (expr.Value == null) return;
                ASResult type = GetExpressionType(sci, textEndPosition);
                if (type.IsPackage) return;
                ContextFeatures features = ASContext.Context.Features;

                // add ; for imports
                if (" \n\t".IndexOf(trigger) >= 0 && expr.WordBefore != null 
                    && (expr.WordBefore == features.importKey || expr.WordBefore == features.importKeyAlt))
                {
                    if (!sci.GetLine(sci.CurrentLine).Contains(";")) sci.InsertText(sci.CurrentPos, ";");
                    return;
                }

                // look for a snippet
                if (trigger == '\t' && expr.Value.IndexOfOrdinal(features.dot) < 0)
                {
                    foreach(string key in features.codeKeywords)
                        if (key == expr.Value)
                        {
                            InsertSnippet(key);
                            return;
                        }
                }

                // resolve context & do smart insertion
                expr.LocalVars = ParseLocalVars(expr);
                ASResult context = EvalExpression(expr.Value, expr, ASContext.Context.CurrentModel, ASContext.Context.CurrentClass, true, false);
                if (SmartInsertion(sci, position, expr, context))
                    DispatchInsertedElement(context, trigger);
            }
        }

        private static void SmartEventInsertion(ScintillaControl sci, int position, ICompletionListItem item)
        {
            if (!ASContext.Context.Settings.GenerateImports) return;
            try
            {
                ClassModel import = ((EventItem) item).EventType;
                if (!ASContext.Context.IsImported(import, sci.LineFromPosition(position)))
                {
                    int offset = ASGenerator.InsertImport(import, true);
                    if (offset > 0)
                    {
                        position += offset;
                        sci.SetSel(position, position);
                    }
                }
            }
            catch (Exception) // event type name already present in imports
            {
            }
        }

        private static bool SmartInsertion(ScintillaControl sci, int position, ASExpr expr, ASResult context)
        {
            ContextFeatures features = ASContext.Context.Features;
            FileModel cFile = ASContext.Context.CurrentModel;
            FileModel inFile = null;
            MemberModel import = null;

            // if completed a package-level member
            if (context.Member != null && context.Member.IsPackageLevel && context.Member.InFile.Package != "")
            {
                inFile = context.Member.InFile;
                import = (MemberModel) context.Member.Clone();
                import.Type = inFile.Package + "." + import.Name;
            }
            // if not completed a type
            else if (context.IsNull() || !context.IsStatic || context.Type == null
                || (context.Type.Type != null && context.Type.Type.IndexOfOrdinal(features.dot) < 0)
                || context.Type.IsVoid())
            {
                if (context.Member != null && expr.Separator == " " && expr.WordBefore == features.overrideKey)
                {
                    ASGenerator.GenerateOverride(sci, context.InClass, context.Member, position);
                    return false;
                }
                /*else if (context.Member != null && cMember == null && !context.inClass.IsVoid())
                {
                    string ins = features.overrideKey + " ";
                    string w = sci.GetWordFromPosition(position);
                    sci.SetSel(position, position);
                    sci.ReplaceSel(ins);
                    position = position + ins.Length;
                    sci.CurrentPos = position;
                    sci.SetSel(position, position);
                    ASGenerator.GenerateOverride(sci, context.inClass, context.Member, position);
                    return false;
                }*/
                if (!context.IsNull() && expr.WordBefore == features.importKey)
                {
                    ASContext.Context.RefreshContextCache(expr.Value);
                }
                return true;
            }
            // test inserted type
            else
            {
                inFile = context.InFile;
                import = context.Type;
            }
            if (inFile == null || import == null)
                return false;

            if (expr.Separator == " " && !string.IsNullOrEmpty(expr.WordBefore))
            {
                if (expr.WordBefore == features.importKey || expr.WordBefore == features.importKeyAlt
                    /*|| (!features.HasTypePreKey(expr.WordBefore) && expr.WordBefore != "case" && expr.WordBefore != "return")*/)
                {
                    ASContext.Context.RefreshContextCache(expr.Value);
                    return true;
                }
            }

            int offset = 0;
            int startPos = expr.PositionExpression;
            int endPos = sci.CurrentPos;

            if (ASContext.Context.Settings.GenerateImports && ShouldShortenType(sci, position, import, cFile, ref offset))
            {
                // insert short name
                startPos += offset;
                endPos += offset;
                sci.SetSel(startPos, endPos);
                sci.ReplaceSel(CheckShortName(import.Name));
                sci.SetSel(sci.CurrentPos, sci.CurrentPos);
            }            
            return true;
        }

        private static bool ShouldShortenType(ScintillaControl sci, int position, MemberModel import, FileModel cFile, ref int offset)
        {
            // check if in the same file or package
            /*if (cFile == inFile || features.hasPackages && cFile.Package == inFile.Package)
                return true*/

            if (IsMetadataArgument(sci, position))
                return false;

            // type name already present in imports
            try
            {
                int curLine = sci.LineFromPosition(position);
                if (ASContext.Context.IsImported(import, curLine))
                    return true;
            }
            catch (Exception) 
            {
                return false;
            }

            // class with same name exists in current package?
            if (ASContext.Context.Features.hasPackages && import is ClassModel)
            {
                string cname = import.Name;
                if (cFile.Package.Length > 0) cname = cFile.Package + "." + cname;
                ClassModel inPackage = ASContext.Context.ResolveType(cname, cFile);
                if (!inPackage.IsVoid())
                    return true;
            }

            // insert import
            if (ASContext.Context.Settings.GenerateImports)
            {
                sci.BeginUndoAction();
                try
                {
                    offset = ASGenerator.InsertImport(import, true);
                }
                finally
                {
                    sci.EndUndoAction();
                }
                return true;
            }
            return false;
        }

        private static string CheckShortName(string name)
        {
            int p = name.IndexOf('<');
            if (p > 1 && name[p - 1] == '.') p--;
            return (p > 0) ? name.Substring(0, p) : name;
        }

        private static void DispatchInsertedElement(ASResult context, char trigger)
        {
            Hashtable info = new Hashtable();
            info["context"] = context;
            info["trigger"] = trigger;
            DataEvent de = new DataEvent(EventType.Command, "ASCompletion.InsertedElement", info);
            EventManager.DispatchEvent(ASContext.Context, de);
        }

        private static void InsertSnippet(string word)
        {
            if (HasSnippet(word))
                PluginBase.MainForm.CallCommand("InsertSnippet", word);
        }

        public static bool HasSnippet(string word)
        {
            String global = Path.Combine(PathHelper.SnippetDir, word + ".fds");
            String specificDir = Path.Combine(PathHelper.SnippetDir, ASContext.Context.Settings.LanguageId);
            String specific = Path.Combine(specificDir, word + ".fds");
            return File.Exists(specific) || File.Exists(global);
        }

        /// <summary>
        /// Some characters can fire code generation
        /// </summary>
        /// <param name="sci"></param>
        /// <param name="value">Character inserted</param>
        /// <returns>Code was generated</returns>
        static bool CodeAutoOnChar(ScintillaControl sci, int value)
        {
            if (ASContext.Context.Settings == null || !ASContext.Context.Settings.GenerateImports)
                return false;

            int position = sci.CurrentPos;

            if (value == '*' && position > 1 && sci.CharAt(position - 2) == '.' && LastExpression != null)
                return HandleWildcardList(sci, position, LastExpression);

            return false;
        }

        /// <summary>
        /// User entered a qualified package with a wildcard, eg. flash.geom.*, at an unexpected position, eg. not after 'import'.
        /// Remove expression, generate coresponding wildcard import, and show list of types of this package
        /// </summary>
        static bool HandleWildcardList(ScintillaControl sci, int position, ASExpr expr)
        {
            // validate context
            var context = ASContext.Context;
            if (expr.Separator == " " && expr.WordBefore != null
                && context.Features.HasTypePreKey(expr.WordBefore))
                return false;

            var cFile = context.CurrentModel;
            var cClass = context.CurrentClass;
            var resolved = EvalExpression(expr.Value, expr, cFile, cClass, true, false);
            if (resolved.IsNull() || !resolved.IsPackage || resolved.InFile == null)
                return false;

            string package = resolved.InFile.Package;
            string check = Regex.Replace(expr.Value, "\\s", "").TrimEnd('.');
            if (check != package)
                return false;

            sci.BeginUndoAction();
            try
            {
                // remove temp wildcard
                int startPos = expr.PositionExpression;
                sci.SetSel(startPos, position);
                sci.ReplaceSel("");

                // generate import
                if (context.Settings.GenerateImports)
                {
                    var wildcard = new MemberModel { Name = "*", Type = package + ".*" };
                    if (!context.IsImported(wildcard, sci.LineFromPosition(position)))
                    {
                        startPos += ASGenerator.InsertImport(wildcard, true);
                        sci.SetSel(startPos, startPos);
                    }
                }
            }
            finally
            {
                sci.EndUndoAction();
            }

            // show types
            var list = new List<ICompletionListItem>();
            var imports = context.ResolvePackage(package, false).Imports;
            foreach (MemberModel import in imports)
                list.Add(new MemberItem(import));
            CompletionList.Show(list, false);
            return true;
        }

        #endregion
        
    }

    #region completion list
    /// <summary>
    /// Class member completion list item
    /// </summary>
    public class MemberItem : ICompletionListItem
    {
        protected MemberModel member;
        private int icon;

        public MemberItem(MemberModel oMember)
        {
            member = oMember;
            icon = PluginUI.GetIcon(member.Flags, member.Access); 
        }

        public string Label
        {
            get { return member.FullName; }
        }

        public virtual string Description
        {
            get
            {
                return ClassModel.MemberDeclaration(member) + ASDocumentation.GetTipDetails(member, null);
            }
        }

        public Bitmap Icon
        {
            get { return (Bitmap)ASContext.Panel.GetIcon(icon); }
        }

        public string Value
        {
            get 
            {
                if (member.Name.IndexOf('<') > 0 && member.Template != null)
                {
                    if (member.Name.IndexOfOrdinal(".<") > 0)
                        return member.Name.Substring(0, member.Name.IndexOfOrdinal(".<"));

                    return member.Name.Substring(0, member.Name.IndexOf('<'));
                }
                return member.Name;
            }
        }

        public override string ToString()
        {
            return Label;
        }
    }

    /// <summary>
    /// Nonexistent member completion list item
    /// </summary>
    public class NonexistentMemberItem : ICompletionListItem
    {
        private static Bitmap icon; 

        private string memberName;

        public NonexistentMemberItem(string memberName)
        {
            this.memberName = memberName;
        }

        public string Label
        {
            get { return memberName; }
        }

        public virtual string Description
        {
            get
            {
                return memberName;
            }
        }

        public Bitmap Icon
        {
            get
            {
                if (icon == null) icon = (Bitmap)PluginBase.MainForm.FindImage("197");
                return icon;
            }
        }

        public string Value
        {
            get
            {
                if (memberName.IndexOf('<') > 0)
                {
                    if (memberName.IndexOfOrdinal(".<") > 0)
                        return memberName.Substring(0, memberName.IndexOfOrdinal(".<"));
                    else return memberName.Substring(0, memberName.IndexOf('<'));
                }
                return memberName;
            }
        }

        public override string ToString()
        {
            return Label;
        }
    }

    /// <summary>
    /// Template completion list item
    /// </summary>
    public class TemplateItem : MemberItem
    {
        public TemplateItem(MemberModel oMember) : base(oMember) { }

        override public string Description
        {
            get
            {
                if (ASComplete.HasSnippet(member.Name))
                    member.Comments = "[i](" + TextHelper.GetString("Info.InsertKeywordSnippet") + ")[/i]";
                return base.Description;
            }
        }
    }

    /// <summary>
    /// Declaration completion list item
    /// </summary>
    public class DeclarationItem : ICompletionListItem
    {
        private string label;

        public DeclarationItem(string label)
        {
            this.label = label;
        }

        public string Label
        {
            get { return label; }
        }
        public string Description
        {
            get { return TextHelper.GetString("Info.DeclarationTemplate"); }
        }

        public Bitmap Icon
        {
            get { return (Bitmap)ASContext.Panel.GetIcon(PluginUI.ICON_DECLARATION); }
        }

        public string Value
        {
            get { return label; }
        }
    }

    /// <summary>
    /// Declaration completion list item
    /// </summary>
    public class EventItem : ICompletionListItem
    {
        private string name;
        private string comments;
        private CommentBlock cb;
        public ClassModel EventType;

        public EventItem(string name, ClassModel type, string comments)
        {
            this.name = name;
            EventType = type;
            this.comments = comments;
        }

        public string Label
        {
            get { return name; }
        }
        public string Description
        {
            get 
            {
                if (!ASContext.CommonSettings.SmartTipsEnabled) return TextHelper.GetString("Info.EventConstant");
                if (cb == null) cb = ASDocumentation.ParseComment(comments ?? name);
                string tip = (UITools.Manager.ShowDetails) ? ASDocumentation.GetTipFullDetails(cb, null) : ASDocumentation.GetTipShortDetails(cb, null);
                // remove paragraphs from comments
                return ASDocumentation.RemoveHTMLTags(tip).Trim();
            }
        }

        public Bitmap Icon
        {
            get { return (Bitmap)ASContext.Panel.GetIcon(PluginUI.ICON_CONST); }
        }

        public string Value
        {
            get { return name; }
        }
    }

    public class CompletionItemComparer : IComparer<ICompletionListItem>
    {
        public int Compare(ICompletionListItem a, ICompletionListItem b)
        {
            return a.Label.CompareTo(b.Label);
        }
    }

    public class CompletionItemCaseSensitiveImportComparer : IComparer<ICompletionListItem>
    {
        public int Compare(ICompletionListItem x, ICompletionListItem y)
        {
            return CaseSensitiveImportComparer.CompareImports(x.Label, y.Label);
        }
    }
    #endregion

    #region expressions_structures
    public enum ComaExpression
    {
        None,
        AnonymousObject,
        AnonymousObjectParam,
        VarDeclaration,
        FunctionDeclaration,
        FunctionParameter,
        ArrayValue,
        GenericIndexType
    }

    /// <summary>
    /// Parsed expression with it's function context
    /// </summary>
    public sealed class ASExpr
    {
        /// <summary>
        /// End position of expression
        /// </summary>
        public int Position;
        public MemberModel ContextMember;
        public MemberList LocalVars;
        public MemberModel ContextFunction;
        public string FunctionBody;
        public int FunctionOffset;
        public bool BeforeBody;

        /// <summary>
        /// Start position of expression
        /// </summary>
        public int PositionExpression;
        public string Value;
        public List<string> SubExpressions;
        public string Separator;
        public int SeparatorPosition;
        public string WordBefore;
        public int WordBeforePosition;
        public ComaExpression coma;
        public string RightOperator = string.Empty;

        internal int LineFrom;
        internal int LineTo;

        public ASExpr() { }

        public ASExpr(ASExpr inContext) 
        {
            ContextMember = inContext.ContextMember;
            ContextFunction = inContext.ContextFunction;
            LocalVars = inContext.LocalVars;
            FunctionBody = inContext.FunctionBody;
        }
    }

    /// <summary>
    /// Expressions/tokens evaluation result
    /// </summary>
    public sealed class ASResult
    {
        public ClassModel Type;
        public ClassModel InClass;
        public ClassModel RelClass;
        public FileModel InFile;
        public MemberModel Member;
        public bool IsStatic;
        public bool IsPackage;
        public ASExpr Context;
        public String Path;

        public bool IsNull()
        {
            return (Type == null && Member == null && !IsPackage);
        }
    }

    public sealed class ResolvedContext
    {
        public int Position = -1;
        public Hashtable Arguments = new Hashtable();
        public ASResult Result;
        public ClassModel TokenType;
    }
    #endregion
}