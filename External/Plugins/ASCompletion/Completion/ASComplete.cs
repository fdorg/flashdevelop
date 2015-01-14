/*
 * Code completion
 */

using System;
using System.Windows.Forms;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Diagnostics;
using PluginCore;
using PluginCore.Managers;
using PluginCore.Controls;
using ASCompletion.Model;
using ASCompletion.Context;
using System.IO;
using PluginCore.Helpers;
using PluginCore.Localization;
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

        static private Braces[] AddClosingBracesData = new Braces[] { 
            new Braces('(', ')'), new Braces('[', ']'), new Braces('{', '}'), new Braces('"', '"'), new Braces('\'', '\'') };
        #endregion

        #region application_event_handlers
        /// <summary>
		/// Character written in editor
		/// </summary>
		/// <param name="Value">Character inserted</param>
		static public bool OnChar(ScintillaControl Sci, int Value, bool autoHide)
		{
            IASContext ctx = ASContext.Context;
            ContextFeatures features = ctx.Features;
			if (ctx.Settings == null || !ctx.Settings.CompletionEnabled) 
                return false;
			try
			{
                if (Sci.IsSelectionRectangle) 
                    return false;
				// code auto
                int eolMode = Sci.EOLMode;
                if (((Value == 10) && (eolMode != 1)) || ((Value == 13) && (eolMode == 1)))
				{
                    if (ASContext.HasContext && ASContext.Context.IsFileValid) HandleStructureCompletion(Sci);
					return false;
				}

                int position = Sci.CurrentPos;
                if (position < 2) return false;

                char prevValue = (char)Sci.CharAt(position - 2);
                bool skipQuoteCheck = false;

                // string interpolation
                if (features.hasStringInterpolation &&
                    features.stringInterpolationQuotes.IndexOf(Sci.GetStringType(position)) >= 0)
                {
                    if (Value == '$')
                        return HandleInterpolationCompletion(Sci, autoHide, false);
                    else if (prevValue == '$' && Value == '{')
                    {
                        return HandleInterpolationCompletion(Sci, autoHide, true);
                    }
                    else if (IsInterpolationExpr(Sci, position))
                        skipQuoteCheck = true; // continue on with regular completion
                }
                
                if (!skipQuoteCheck)
                {
                    // ignore text in comments & quoted text
                    Sci.Colourise(0, -1);
                    int stylemask = (1 << Sci.StyleBits) - 1;
                    int style = Sci.StyleAt(position - 1) & stylemask;
                    if (!IsTextStyle(style) && !IsTextStyle(Sci.StyleAt(position) & stylemask))
                    {
                        // documentation completion
                        if (ASContext.CommonSettings.SmartTipsEnabled && IsCommentStyle(style))
                            return ASDocumentation.OnChar(Sci, Value, position, style);
                        else if (autoHide)
                        {
                            // close quotes
                            HandleAddClosingBraces(Sci, (char)Value, true);
                            return false;
                        }
                    }
                }

                // close brace/parens
                if (autoHide) HandleAddClosingBraces(Sci, (char)Value, true);

				// stop here if the class is not valid
				if (!ASContext.HasContext || !ASContext.Context.IsFileValid) return false;

				// handle
				switch (Value)
				{
					case '.':
                        if (features.dot == "." || !autoHide)
                            return HandleDotCompletion(Sci, autoHide);
                        break;

                    case '>':
                        if (features.dot == "->" && prevValue == '-')
                            return HandleDotCompletion(Sci, autoHide);
                        break;

                    case ' ':
						position--;
						string word = GetWordLeft(Sci, ref position);
                        if (word.Length <= 0)
                        {
                            char c = (char)Sci.CharAt(position);
                            if (c == ':' && features.hasEcmaTyping)
                                return HandleColonCompletion(Sci, "", autoHide);
                            else if (autoHide && (c == '(' || c == ',') && !ASContext.CommonSettings.DisableCallTip)
                                return HandleFunctionCompletion(Sci, autoHide);
                            break;
                        }
                        if (word == "package" || Array.IndexOf(features.typesKeywords, word) >= 0) 
                            return false;
                        // new/extends/instanceof/...
                        if (features.HasTypePreKey(word))
							return HandleNewCompletion(Sci, "", autoHide, word);
                        // import
                        if (features.hasImports && (word == features.importKey || word == features.importKeyAlt))
							return HandleImportCompletion(Sci, "", autoHide);
                        // public/internal/private/protected/static
                        if (Array.IndexOf(features.accessKeywords, word) >= 0)
                            return HandleDeclarationCompletion(Sci, "", autoHide);
                        // override
                        if (word == features.overrideKey)
                            return ASGenerator.HandleGeneratorCompletion(Sci, autoHide, word);
						break;

					case ':':
                        if (ASContext.Context.CurrentModel.haXe && 
                            ASContext.Context.CurrentMember == null && prevValue == '@')
                        {
                            return HandleMetadataCompletion(Sci, autoHide);
                        }
                        if (features.hasEcmaTyping)
                        {
                            return HandleColonCompletion(Sci, "", autoHide);
                        }
                        else break;

                    case '<':
                        if (features.hasGenerics && position > 2)
                        {
                            char c0 = (char)Sci.CharAt(position - 2);
                            bool result = false;
                            if (c0 == '.' /*|| Char.IsLetterOrDigit(c0)*/)
                                return HandleColonCompletion(Sci, "", autoHide);
                            return result;
                        }
                        else break;

					case '(':
                    case ',':
                        if (!ASContext.CommonSettings.DisableCallTip)
                            return HandleFunctionCompletion(Sci, autoHide);
                        else return false;

					case ')':
                        if (UITools.CallTip.CallTipActive) UITools.CallTip.Hide();
						return false;

					case '*':
                        if (features.hasImportsWildcard) return CodeAutoOnChar(Sci, Value);
                        break;

                    case ';':
                        if (!ASContext.CommonSettings.DisableCodeReformat) 
                            ReformatLine(Sci, position);
                        break;

                    default:
                        AutoStartCompletion(Sci, position);
                        break;
				}
			}
			catch (Exception ex) {
				ErrorManager.ShowError(/*"Completion error",*/ ex);
			}

			// CodeAuto context
			if (!PluginCore.Controls.CompletionList.Active) LastExpression = null;
			return false;
        }

        internal static void OnTextChanged(ScintillaControl sci, int position, int length, int linesAdded)
        {
            // TODO track text changes -> LastChar
        }

		/// <summary>
		/// Handle shortcuts
		/// </summary>
		/// <param name="keys">Test keys</param>
		/// <returns></returns>
		static public bool OnShortcut(Keys keys, ScintillaControl Sci)
		{
            if (Sci.IsSelectionRectangle) 
                return false;

            // dot complete
			if (keys == (Keys.Control | Keys.Space))
			{
                if (ASContext.HasContext && ASContext.Context.IsFileValid)
				{
                    // try to get completion as if we had just typed the previous char
                    if (OnChar(Sci, Sci.CharAt(Sci.PositionBefore(Sci.CurrentPos)), false))
                        return true;
					else
                    {
                        // force dot completion
                        OnChar(Sci, '.', false);
                        return true;
                    }
				}
				else return false;
			}
            else if (keys == Keys.Back)
            {
                HandleAddClosingBraces(Sci, Sci.CurrentChar, false);
                return false;
            }
			// show calltip
			else if (keys == (Keys.Control | Keys.Shift | Keys.Space))
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
            else if (keys == (Keys.Control | Keys.Alt | Keys.Space))
            {
                if (ASContext.HasContext && ASContext.Context.IsFileValid && !ASContext.Context.Settings.LazyClasspathExploration)
                {
                    int position = Sci.CurrentPos-1;
                    string tail = GetWordLeft(Sci, ref position);
                    ContextFeatures features = ASContext.Context.Features;
                    if (tail.IndexOf(features.dot) < 0 && features.HasTypePreKey(tail)) tail = "";
                    // display the full project classes list
                    HandleAllClassesCompletion(Sci, tail, false, true);
                    return true;
                }
                else return false;
            }
			// hot build
			else if (keys == (Keys.Control | Keys.Enter))
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
                                    Commands.CallFlashIDE.Run(idePath, cmd);
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
        private static void AutoStartCompletion(ScintillaControl Sci, int position)
        {
            if (!CompletionList.Active && ASContext.Context.Features.hasEcmaTyping 
                && ASContext.CommonSettings.AlwaysCompleteWordLength > 0)
            {
                // fire completion if starting to write a word
                bool valid = true;
                int n = ASContext.CommonSettings.AlwaysCompleteWordLength;
                int wordStart = Sci.WordStartPosition(position, true);
                if (position - wordStart != n)
                    return;
                char c = (char)Sci.CharAt(wordStart);
                string characterClass = ScintillaControl.Configuration.GetLanguage(Sci.ConfigurationLanguage).characterclass.Characters;
                if (Char.IsDigit(c) || characterClass.IndexOf(c) < 0)
                    return;
                // give a guess to the context (do not complete where it should not)
                if (valid)
                {
                    int pos = wordStart - 1;
                    c = ' ';
                    char c2 = ' ';
                    bool hadWS = false;
                    bool canComplete = false;
                    while (pos > 0)
                    {
                        c = (char)Sci.CharAt(pos--);
                        if (hadWS && characterClass.IndexOf(c) >= 0) break;
                        else if (c == '<' && ((char)Sci.CharAt(pos + 2) == '/' || !hadWS)) break;
                        else if (":;,+-*%!&|<>/{}()[=?".IndexOf(c) >= 0)
                        {
                            canComplete = true;
                            // TODO  Add HTML lookup here
                            if (pos > 0)
                            {
                                char c0 = (char)Sci.CharAt(pos);
                                if (c == '/' && c0 == '<') canComplete = false;
                            }
                            break;
                        }
                        else if (c <= 32)
                        {
                            if (c == '\r' || c == '\n')
                            {
                                canComplete = true;
                                break;
                            }
                            else if (pos > 1)
                            {
                                int style = Sci.BaseStyleAt(pos - 1);
                                if (style == 19)
                                {
                                    canComplete = true;
                                    break;
                                }
                            }
                            hadWS = true;
                        }
                        else if (c != '.' && characterClass.IndexOf(c) < 0)
                        {
                            // TODO support custom DOT
                            canComplete = false;
                            break;
                        }
                        c2 = c;
                    }
                    if (canComplete) HandleDotCompletion(Sci, true);
                }
            }
        }
		#endregion

        #region add_closing_braces
        public static void HandleAddClosingBraces(ScintillaControl sci, char c, bool addedChar)
        {
            if (!ASContext.CommonSettings.AddClosingBraces)
                return;

            int stylemask = (1 << sci.StyleBits) - 1;
            int style = sci.StyleAt(sci.CurrentPos - 1) & stylemask;

            if (IsTextStyle(sci.StyleAt(sci.CurrentPos - 2) & stylemask) || IsInterpolationExpr(sci, sci.CurrentPos - 2))
            {
                foreach (Braces braces in AddClosingBracesData)
                {
                    if (addedChar)
                        HandleAddBrace(sci, c, braces);
                    else
                        HandleRemoveBrace(sci, c, braces);
                }
            }
        }

        private static void HandleAddBrace(ScintillaControl sci, char c, Braces braces)
        {
            if (ASContext.CommonSettings.AddClosingBraces)
            {
                if (c == braces.opening)
                {
                    // already an opening brace?
                    if ((char)sci.CharAt(sci.CurrentPos - 2) != braces.opening)
                    {
                        sci.InsertText(sci.CurrentPos, braces.closing.ToString());
                    }
                }
                else if (c == braces.closing)
                {
                    // already a closing brace?
                    if (sci.CurrentChar == braces.closing)
                    {
                        sci.DeleteForward();
                    }
                }
            }
        }

        private static void HandleRemoveBrace(ScintillaControl sci, char c, Braces braces)
        {
            if (ASContext.CommonSettings.AddClosingBraces && c == braces.closing)
            {
                if ((char)sci.CharAt(sci.CurrentPos - 1) == braces.opening)
                {
                    sci.DeleteForward();
                }
            }
        }
        #endregion

        #region plugin commands
        /// <summary>
		/// Using the text under at cursor position, search and open the object/class/member declaration
		/// </summary>
		/// <param name="Sci">Control</param>
		/// <returns>Declaration was found</returns>
		static public bool DeclarationLookup(ScintillaControl Sci)
		{
			if (!ASContext.Context.IsFileValid || (Sci == null)) return false;

			// get type at cursor position
			int position = Sci.WordEndPosition(Sci.CurrentPos, true);
			ASResult result = GetExpressionType(Sci, position, false);

			// browse to package folder
            if (result.IsPackage && result.InFile != null)
			{
				return ASContext.Context.BrowseTo(result.InFile.Package);
			}

			// open source and show declaration
            if (!result.IsNull())
			{
                if (result.Member != null && (result.Member.Flags & FlagType.AutomaticVar) > 0)
                    return false;

                // open the file
                return OpenDocumentToDeclaration(Sci, result);
			}
            // show overriden method
            else if (ASContext.Context.CurrentMember != null 
                && ASContext.Context.Features.overrideKey != null
                && Sci.GetWordFromPosition(position) == ASContext.Context.Features.overrideKey)
            {
                MemberModel member = ASContext.Context.CurrentMember;
                if ((member.Flags & FlagType.Override) > 0)
                {
                    ClassModel tmpClass = ASContext.Context.CurrentClass;
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
                                OpenDocumentToDeclaration(Sci, result);
                                break;
                            }
                            tmpClass = tmpClass.Extends;
                        }
                    }
                }
            }
			return false;
		}

        /// <summary>
        /// Show resolved element declaration
        /// </summary>
        static public bool OpenDocumentToDeclaration(ScintillaControl Sci, ASResult result)
        {
            FileModel model = result.InFile
                ?? ((result.Member != null && result.Member.InFile != null) ? result.Member.InFile : null)
                ?? ((result.Type != null) ? result.Type.InFile : null);
            if (model == null || model.FileName == "") return false;
            ClassModel inClass = result.InClass ?? result.Type;

            // for Back command
            if (Sci != null)
            {
                int lookupLine = Sci.CurrentLine;
                int lookupCol = Sci.CurrentPos - Sci.PositionFromLine(lookupLine);
                ASContext.Panel.SetLastLookupPosition(ASContext.Context.CurrentFile, lookupLine, lookupCol);
            }

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
            if ((inClass == null || inClass.IsVoid()) && result.Member == null)
                return false;

            Sci = ASContext.CurSciControl;
            if (Sci == null)
                return false;

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
            else if (inClass.LineFrom > 0)
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
            if (ext == "") ext = model.Context.GetExplorerMask()[0];
            string dummyFile = Path.Combine(
                Path.GetDirectoryName(model.FileName),
                "[model] " + Path.GetFileNameWithoutExtension(model.FileName) + ext);
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
            ASContext.MainForm.CreateEditableDocument(dummyFile, src, Encoding.UTF8.CodePage);
        }

        static public void LocateMember(string keyword, string name, int line)
        {
            try
            {
                ScintillaControl sci = PluginBase.MainForm.CurrentDocument.SciControl;
                if (sci == null || line <= 0) return;

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
                            sci.EnsureVisible(sci.LineFromPosition(position));
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
                if (ASContext.Context.IsFileValid) result = GetExpressionType(Sci, position);
                else result = new ASResult();
                CurrentResolvedContext.Result = result;
                ContextFeatures features = context.Features;

                Hashtable args = CurrentResolvedContext.Arguments;
                string package = context.CurrentModel.Package;
                args.Add("TypPkg", package);

                ClassModel cClass = context.CurrentClass;
                if (cClass == null) cClass = ClassModel.VoidClass;
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
                        = ASContext.Context.ResolveType(context.CurrentMember.Type, context.CurrentModel);
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
                    ClassModel oClass = result.InClass != null ? result.InClass : result.Type;

                    if (oClass.IsVoid() && (result.Member.Flags & FlagType.Function) == 0 && (result.Member.Flags & FlagType.Namespace) == 0)
                        return;

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
                ASComplete.OnResolvedContextChanged(CurrentResolvedContext);
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
                    if (cmd == null || cmd.Length == 0) return null;
                    // top-level vars should be searched only if the command includes member information
                    if (CurrentResolvedContext.Result.InClass == ClassModel.VoidClass && cmd.IndexOf("$(Itm") < 0) 
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
            MemberModel closestList = null;
            if (expr == null || expr.LocalVars == null) 
                return;

            foreach (MemberModel m in expr.LocalVars)
            {
                if (m.LineFrom > lineNum)
                    continue;
                if (closestList != null && (lineNum - m.LineFrom) >= (lineNum - closestList.LineFrom))
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
            bool restartCycle = false;
            MemberList members = cClass.Members;
            List<MemberModel> parameters = context.CurrentMember.Parameters;
            while (true)
            {
                restartCycle = false;
                if (expr != null && expr.LocalVars != null)
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
				string txt = Sci.GetLine(line-1).TrimEnd();
				int style = Sci.BaseStyleAt(position);

                if (Sci.CurrentChar == '}')
                {
                    Sci.DeleteForward();
                    AutoCloseBrace(Sci, line);
                }
                // in comments
                else if (PluginBase.Settings.CommentBlockStyle == CommentBlockStyle.Indented && txt.EndsWith("*/"))
                    FixIndentationAfterComments(Sci, line);
                else if (IsCommentStyle(style) && (Sci.BaseStyleAt(position + 1) == style))
                    FormatComments(Sci, txt, line);
                // in code
                else
                {
                    // braces
                    if (!ASContext.CommonSettings.DisableAutoCloseBraces)
                    {
                        if (txt.IndexOf("//") > 0) // remove comment at end of line
                        {
                            int slashes = Sci.MBSafeTextLength(txt.Substring(0, txt.IndexOf("//") + 1));
                            if (Sci.PositionIsOnComment(Sci.PositionFromLine(line-1) + slashes))
                                txt = txt.Substring(0, txt.IndexOf("//")).Trim();
                        }
                        if (txt.EndsWith("{") && (line > 1)) AutoCloseBrace(Sci, line);
                    }
                    // code reformatting
                    if (!ASContext.CommonSettings.DisableCodeReformat && !txt.EndsWith("*/"))
                        ReformatLine(Sci, Sci.PositionFromLine(line) - 1);
                }
			}
			catch (Exception ex)
			{
				ErrorManager.ShowError(ex);
			}
		}

        private static void ReformatLine(ScintillaControl Sci, int position)
        {
            int line = Sci.LineFromPosition(position);
            string txt = Sci.GetLine(line);
            int curPos = Sci.CurrentPos;
            int startPos = Sci.PositionFromLine(line);
            int offset = Sci.MBSafeLengthFromBytes(txt, position - startPos);
            
            ReformatOptions options = new ReformatOptions();
            options.Newline = LineEndDetector.GetNewLineMarker(Sci.EOLMode);
            options.CondenseWhitespace = ASContext.CommonSettings.CondenseWhitespace;
            options.BraceAfterLine = ASContext.CommonSettings.ReformatBraces 
                && PluginBase.MainForm.Settings.CodingStyle == CodingStyle.BracesAfterLine;
            options.CompactChars = ASContext.CommonSettings.CompactChars;
            options.SpacedChars = ASContext.CommonSettings.SpacedChars;
            options.SpaceBeforeFunctionCall = ASContext.CommonSettings.SpaceBeforeFunctionCall;
            options.AddSpaceAfter = ASContext.CommonSettings.AddSpaceAfter.Split(' ');
            options.IsPhp = ASContext.Context.Settings.LanguageId == "PHP";
            options.IsHaXe = ASContext.Context.Settings.LanguageId == "HAXE";

            int newOffset = offset;
            string replace = Reformater.ReformatLine(txt, options, ref newOffset);

            if (replace != txt)
            {
                position = curPos + newOffset - offset;
                Sci.SetSel(startPos, startPos + Sci.MBSafeTextLength(txt));
                Sci.ReplaceSel(replace);
                Sci.SetSel(position, position);
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
            int newIndent = indent + Sci.TabWidth;
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
                if (txt.StartsWith("/*")) break;
                else if (!txt.StartsWith("*")) break;
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
            if (txt.StartsWith("/*"))
            {
                Sci.ReplaceSel("* ");
                if (PluginBase.Settings.CommentBlockStyle == CommentBlockStyle.Indented)
                    Sci.SetLineIndentation(line, Sci.GetLineIndentation(line) + 1);
                int position = Sci.LineIndentPosition(line) + 2;
                Sci.SetSel(position, position);
            }
            else if (txt.StartsWith("*"))
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
            
            // current model
            FileModel cFile = ASContext.Context.CurrentModel;
            ClassModel cClass = ASContext.Context.CurrentClass;

            // does it need indentation?
            int tab = 0;
            int tempLine = line-1;
            int tempIndent;
            string tempText;
            while (tempLine > 0)
            {
                tempText = Sci.GetLine(tempLine).Trim();
                if (insideClass && IsTypeDecl(tempText, features.typesKeywords))
                {
                    tempIndent = Sci.GetLineIndentation(tempLine);
                    tab = tempIndent + Sci.TabWidth;
                    break;
                }
                if (tempText.Length > 0 && (tempText.EndsWith("}") || IsDeclaration(tempText, features)))
                {
                    tempIndent = Sci.GetLineIndentation(tempLine);
                    tab = tempIndent;
                    if (tempText.EndsWith("{")) tab += Sci.TabWidth;
                    break;
                }
                tempLine--;
            }
            if (tab > 0)
            {
                tempIndent = Sci.GetLineIndentation(line);
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

        private static bool IsTypeDecl(string line, string[] typesKeywords)
        {
            foreach (string keyword in typesKeywords)
                if (line.IndexOf(keyword) >= 0) return true;
            return false;
        }

        private static bool IsDeclaration(string line, ContextFeatures features)
        {
            foreach (string keyword in features.accessKeywords)
                if (line.StartsWith(keyword)) return true;
            foreach (string keyword in features.declKeywords)
                if (line.StartsWith(keyword)) return true;
            return false;
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
		static private string paramInfo = "";

		static public bool HasCalltip()
		{
			return UITools.CallTip.CallTipActive && (calltipDef != null);
		}

		/// <summary>
		/// Show highlighted calltip
		/// </summary>
		/// <param name="Sci">Scintilla control</param>
		/// <param name="paramNumber">Highlight param number</param>
		static private void ShowCalltip(ScintillaControl Sci, int paramNumber)
		{
			ShowCalltip(Sci, paramNumber, false);
		}

		static private void ShowCalltip(ScintillaControl Sci, int paramIndex, bool forceRedraw)
		{
            // measure highlighting
			int start = calltipDef.IndexOf('(');
            while ((start >= 0) && (paramIndex-- > 0))
				start = FindNearSymbolInFunctDef(calltipDef, ",", start + 1);

			int end = FindNearSymbolInFunctDef(calltipDef, ",", start + 1);
			if (end < 0)
				end = FindNearSymbolInFunctDef(calltipDef, ")", start + 1);

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

				if (paramName.Length > 0)
				{
					Match mParam = Regex.Match(calltipMember.Comments, "@param\\s+" + Regex.Escape(paramName) + "[ \t:]+(?<desc>[^\r\n]*)");
					if (mParam.Success)
						paramInfo = "\n" + "[U]" + paramName + ":" + "[/U]" + mParam.Groups["desc"].Value.Trim();
					else
						paramInfo = "";
				}
				else paramInfo = "";
			}

			// show calltip
            if (!UITools.CallTip.CallTipActive || UITools.Manager.ShowDetails != calltipDetails || paramName != prevParam)
			{
				prevParam = paramName;
                calltipDetails = UITools.Manager.ShowDetails;
				string text = calltipDef + ASDocumentation.GetTipDetails(calltipMember, paramName);
                UITools.CallTip.CallTipShow(Sci, calltipPos - calltipOffset, text, forceRedraw);
			}

			// highlight
			if ((start < 0) || (end < 0)) UITools.CallTip.CallTipSetHlt(0, 0, true);
			else UITools.CallTip.CallTipSetHlt(start + 1, end, true);
		}

        static string[] featStart = new string[] { "/*", "{", "<", "[", "(" };
        static string[] featEnd = new string[] { "*/", "}", ">", "]", ")" };

		static private int FindNearSymbolInFunctDef(string defBody, string symbol, int startAt)
		{
			int end = -1;
			int featBeg;
			while (true)
			{
				end = defBody.IndexOf(symbol, startAt);
                if (end < 0) break;
                bool cont = false;
                for (int i = 0; i < featStart.Length; i++)
                {
                    featBeg = defBody.IndexOf(featStart[i], startAt);
                    if (featBeg >= 0 && featBeg < end)
                    {
                        startAt = Math.Max(featBeg + 1, defBody.IndexOf(featEnd[i], featBeg));
                        cont = true;
                        break;
                    }
                }
                if (!cont) break;
			}
			return end;
		}

		/// <summary>
		/// Display method signature
		/// </summary>
		/// <param name="Sci">Scintilla control</param>
		/// <returns>Auto-completion has been handled</returns>
		static public bool HandleFunctionCompletion(ScintillaControl Sci, bool autoHide)
		{
			return HandleFunctionCompletion(Sci, autoHide, false);
		}

		static public bool HandleFunctionCompletion(ScintillaControl Sci, bool autoHide, bool forceRedraw)
		{
            // only auto-complete where it makes sense
            if (DeclarationSectionOnly()) 
                return false;

            int position = Sci.CurrentPos - 1;
            int paramIndex = FindParameterIndex(Sci, ref position);
            if (position < 0) return false;
            
            // continuing calltip ?
			if (HasCalltip())
			{
				if (calltipPos == position)
				{
                    ShowCalltip(Sci, paramIndex, forceRedraw);
					return true;
				}
                else UITools.CallTip.Hide();
			}

            if (!ResolveFunction(Sci, position, autoHide))
                return true;

			// EventDispatchers
            if (paramIndex == 0 && calltipRelClass != null && calltipMember.Name.EndsWith("EventListener"))
            {
                ShowListeners(Sci, position, calltipRelClass);
                return true;
            }

            // show calltip
            ShowCalltip(Sci, paramIndex, forceRedraw);
			return true;
        }

        /// <summary>
        /// Find declaration of function called in code
        /// </summary>
        /// <param name="position">Position obtained by FindParameterIndex()</param>
        /// <returns>Function successfully resolved</returns>
        private static bool ResolveFunction(ScintillaControl Sci, int position, bool autoHide)
        {
            calltipPos = 0;
            calltipMember = null;
            calltipRelClass = null;

            // get expression at cursor position
			ASExpr expr = GetExpression(Sci, position, true);
			if (expr.Value == null || expr.Value.Length == 0
			    || (expr.WordBefore == "function" && expr.Separator == ' '))
				return false;

			// Context
            IASContext ctx = ASContext.Context;
            FileModel aFile = ctx.CurrentModel;
            ClassModel aClass = ctx.CurrentClass;
            ASResult result;

            // Expression before cursor
            expr.LocalVars = ParseLocalVars(expr);
            result = EvalExpression(expr.Value, expr, aFile, aClass, true, true);
            if (!result.IsNull() && result.Member == null && result.Type != null)
            {
                foreach(MemberModel member in result.Type.Members)
                    if (member.Name == result.Type.Constructor)
                    {
                        result.Member = member;
                        break;
                    }
            }
            if (result.IsNull() || (result.Member != null && (result.Member.Flags & FlagType.Function) == 0))
            {
                // custom completion
                MemberModel customMethod = ctx.ResolveFunctionContext(Sci, expr, autoHide);
                if (customMethod != null)
                {
                    result = new ASResult();
                    result.Member = customMethod;
                }
            }
			if (result.IsNull()) 
                return false;

			MemberModel method = result.Member;
            if (method == null)
			{
                if (result.Type == null)
                    return false;
				string constructor = ASContext.GetLastStringToken(result.Type.Name, ".");
				result.Member = method = result.Type.Members.Search(constructor, FlagType.Constructor, 0);
				if (method == null)
					return false;
			}
            else if ((method.Flags & FlagType.Function) == 0)
            {
                if (method.Name == "super" && result.Type != null)
                {
                    result.Member = method = result.Type.Members.Search(result.Type.Constructor, FlagType.Constructor, 0);
                    if (method == null)
                        return false;
                }
                else return false;
            }

            // inherit doc
            while ((method.Flags & FlagType.Override) > 0 && result.InClass != null
                && (method.Comments == null || method.Comments.Trim() == "" || method.Comments.Contains("@inheritDoc")))
            {
                FindMember(method.Name, result.InClass.Extends, result, 0, 0);
                method = result.Member;
                if (method == null) 
                    return false;
            }
            if ((method.Comments == null || method.Comments.Trim() == "")
                && result.InClass != null && result.InClass.Implements != null)
            {
                ASResult iResult = new ASResult();
                foreach (string type in result.InClass.Implements)
                {
                    ClassModel model = ASContext.Context.ResolveType(type, result.InFile);
                    FindMember(method.Name, model, iResult, 0, 0);
                    if (iResult.Member != null)
                    {
                        iResult.RelClass = result.RelClass;
                        result = iResult;
                        method = iResult.Member;
                        break;
                    }
                }
            }

            expr.Position = position;
            FunctionContextResolved(Sci, expr, method, result.RelClass, false);
            return true;
        }

        public static void FunctionContextResolved(ScintillaControl Sci, ASExpr expr, MemberModel method, ClassModel inClass, bool showTip)
        {
            if (method == null || string.IsNullOrEmpty(method.Name)) 
                return;
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
                position++;
                int paramIndex = FindParameterIndex(Sci, ref position);
                if (position < 0) return;
                ShowCalltip(Sci, paramIndex, true);
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
            if (calltipMember != null && calltipMember.Parameters != null
                && paramIndex < calltipMember.Parameters.Count)
            {
                MemberModel param = calltipMember.Parameters[paramIndex];
                string type = param.Type;
                if (indexTypeOnly && (String.IsNullOrEmpty(type) || type.IndexOf('@') < 0))
                    return ClassModel.VoidClass;
                if (ASContext.Context.Features.objectKey == "Dynamic" && type.StartsWith("Dynamic@"))
                    type = type.Replace("Dynamic@", "Dynamic<") + ">";
                return ASContext.Context.ResolveType(type, ASContext.Context.CurrentModel);
            }
            else return ClassModel.VoidClass;
        }

        /// <summary>
        /// Locate beginning of function call parameters and return index of current parameter
        /// </summary>
        private static int FindParameterIndex(ScintillaControl Sci, ref int position)
        {
            int parCount = 0;
            int braCount = 0;
            int comaCount = 0;
            int arrCount = 0;
            int style = 0;
            int stylemask = (1 << Sci.StyleBits) - 1;
            char c;
            while (position >= 0)
            {
                style = Sci.StyleAt(position) & stylemask;
                if (style == 19)
                {
                    string keyword = GetWordLeft(Sci, ref position);
                    if (!ASContext.Context.Features.HasTypePreKey(keyword))
                    {
                        position = -1;
                        break;
                    }
                }
                if ((!IsLiteralStyle(style) && IsTextStyleEx(style)) || IsInterpolationExpr(Sci, position))
                {
                    c = (char)Sci.CharAt(position);
                    if (c == ';')
                    {
                        position = -1;
                        break;
                    }
                    // skip {} () [] blocks
                    if (((braCount > 0) && (c != '{' && c != '}'))
                        || ((parCount > 0) && (c != '(' && c != ')'))
                        || ((arrCount > 0) && (c != '[' && c != ']')))
                    {
                        position--;
                        continue;
                    }
                    // new block
                    if (c == '}') braCount++;
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

                    // new parameter reached
                    else if ((c == ',') && (parCount == 0))
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
                FileModel inFile = ofClass.InFile;
                if (inFile.MetaDatas != null)
                {
                    foreach (ASMetaData meta in inFile.MetaDatas)
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
                FlagType flags = FlagType.Variable | FlagType.Constant;
                Visibility acc = Visibility.Public;
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
                            flags = member.Flags;
                            acc = member.Access;
                            if (meta.Comments == null && member.Comments != null) 
                                comments = member.Comments;
                            break;
                        }
                    }

                    if (!typeFound)
                    {
                        if (evClass.InFile.Package.StartsWith("flash.")) 
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

        int CompareEvents(Object a, Object b)
        {
            return 0;
        }
		#endregion

		#region dot_completion
		/// <summary>
		/// Complete object member
		/// </summary>
		/// <param name="Sci">Scintilla control</param>
		/// <param name="autoHide">Don't keep the list open if the word does not match</param>
		/// <returns>Auto-completion has been handled</returns>
		static private bool HandleDotCompletion(ScintillaControl Sci, bool autoHide)
		{
            //this method can exit at multiple points, so reset the current class now rather than later
            currentClassHash = null;

            // only auto-complete where it makes sense
            if (autoHide && DeclarationSectionOnly())
                return false;

			// get expression at cursor position
			int position = Sci.CurrentPos;
			ASExpr expr = GetExpression(Sci, position);
			if (expr.Value == null)
                return true;
            IASContext ctx = ASContext.Context;
            ContextFeatures features = ctx.Features;
            int dotIndex = expr.Value.LastIndexOf(features.dot);
            if (dotIndex == 0 && expr.Separator != '"')
                return true;

			// complete keyword
            string word = expr.WordBefore;
            if (word != null && Array.IndexOf(features.declKeywords, word) >= 0)
                return false;
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
                else if (features.hasEcmaTyping && expr.Separator == ':'
                    && HandleColonCompletion(Sci, expr.Value, autoHide))
                    return true;

                // no completion
                if ((expr.BeforeBody && expr.Separator != '=')
                    || expr.coma == ComaExpression.AnonymousObject
                    || expr.coma == ComaExpression.FunctionDeclaration)
                    return false;

                if (expr.coma == ComaExpression.AnonymousObjectParam)
                {
                    int cpos = Sci.CurrentPos - 1;
                    int paramIndex = FindParameterIndex(Sci, ref cpos);
                    if (calltipPos != cpos) ResolveFunction(Sci, cpos, autoHide);
                    if (calltipMember == null) return false;
                    argumentType = ResolveParameterType(paramIndex, true);
                    if (argumentType.IsVoid()) return false;
                }

                // complete declaration
                MemberModel cMember = ASContext.Context.CurrentMember;
                int line = Sci.LineFromPosition(position);
                if (cMember == null && !ASContext.Context.CurrentClass.IsVoid())
                {
                    if (expr.Value != null && expr.Value.Length > 0)
                        return HandleDeclarationCompletion(Sci, expr.Value, autoHide);
                    else if (ASContext.Context.CurrentModel.Version >= 2)
                        return ASGenerator.HandleGeneratorCompletion(Sci, autoHide, features.overrideKey);
                }
                else if (cMember != null && line == cMember.LineFrom)
                {
                    string text = Sci.GetLine(line);
                    int p = text.IndexOf(cMember.Name);
                    if (p < 0 || position < Sci.PositionFromLine(line) + p)
                        return HandleDeclarationCompletion(Sci, expr.Value, autoHide);
                }
            }
            else
            {
                if (expr.Value.EndsWith("..") || Regex.IsMatch(expr.Value, "^[0-9]+\\.")) 
                    return false;
            }

            string tail = (dotIndex >= 0) ? expr.Value.Substring(dotIndex + features.dot.Length) : expr.Value;
            
            // custom completion
            MemberList items = ASContext.Context.ResolveDotContext(Sci, expr, autoHide);
            if (items != null)
            {
                DotContextResolved(Sci, expr, items, autoHide);
                return true;
            }

            // Context
            ASResult result;
            ClassModel tmpClass;
            bool outOfDate = (expr.Separator == ':') ? ctx.UnsetOutOfDate() : false;
            FileModel cFile = ctx.CurrentModel;
            ClassModel cClass = ctx.CurrentClass;

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
                if (expr.Separator == '"')
                {
                    tmpClass = ctx.ResolveType("String", null);
                    result.Type = tmpClass;
                    dotIndex = 1;
                }
                else tmpClass = cClass;
			}

            //stores a reference to our current class.  tmpClass gets overwritten later, so we need to store the current class separately
            ClassModel classScope = tmpClass;

			MemberList mix = new MemberList();
			// local vars are the first thing to try
            if ((result.IsNull() || (dotIndex < 0)) && expr.ContextFunction != null)
                mix.Merge(expr.LocalVars);

			// get all members
			FlagType mask = 0;
            // members visibility
            ClassModel curClass = cClass;
            curClass.ResolveExtends();
            Visibility acc = ctx.TypesAffinity(curClass, tmpClass);

            // list package elements
            if (result.IsPackage)
            {
                mix.Merge(result.InFile.Imports);
                mix.Merge(result.InFile.Members);
            }
            // list instance members
			else if (expr.ContextFunction != null || expr.Separator != ':' || (dotIndex > 0 && !result.IsNull()))
			{
				// user setting may ask to hide some members
                bool limitMembers = autoHide; // ASContext.Context.HideIntrinsicMembers || (autoHide && !ASContext.Context.AlwaysShowIntrinsicMembers);

                // static or instance members?
                if (!result.IsNull()) mask = result.IsStatic ? FlagType.Static : FlagType.Dynamic;
                else if (expr.ContextFunction == null || IsStatic(expr.ContextFunction)) mask = FlagType.Static;
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
            List<ICompletionListItem> list = new List<ICompletionListItem>();
            foreach (MemberModel member in mix)
            {
                if ((member.Flags & FlagType.Template) > 0)
                    list.Add(new TemplateItem(member));
                else
                    list.Add(new MemberItem(member));
            }
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
            ClassModel cClass = ctx.CurrentClass;
            bool inClass = !cClass.IsVoid();

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
            if (!inClass.IsVoid() && (inClass.Flags & (FlagType.Enum | FlagType.TypeDef | FlagType.Struct)) > 0)
                return true;
            return false;
        }

        private static void AutoselectDotToken(ClassModel classScope, string tail)
        {
            // remember the latest class resolved for completion to store later the inserted member
            currentClassHash = classScope != null ? classScope.QualifiedName : null;

            // if the completion history has a matching entry, it means the user has previously completed from this class.
            if (currentClassHash != null && completionHistory.ContainsKey(currentClassHash))
            {
                // If the last-completed member for the class starts with the currently typed text (tail), select it!
                // Note that if the tail is currently empty (i.e., the user has just typed the first dot), this still passes.
                // This allows it to highlight the last-completed member instantly just by hitting the dot.
                // Also does a check if the tail matches exactly the currently selected item; don't change it!
                if (CompletionList.SelectedLabel != tail && completionHistory[currentClassHash].ToLower().StartsWith(tail.ToLower()))
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
            if (!local.Value.StartsWith(expr.Value) 
                || expr.Value.LastIndexOf(features.dot) != local.Value.LastIndexOf(features.dot))
                return;
            string word = Sci.GetWordLeft(position-1, false);

            // current list
            string reSelect = null;
            if (CompletionList.Active) reSelect = CompletionList.SelectedLabel;

            // show completion
            List<ICompletionListItem> customList = new List<ICompletionListItem>();
            bool testActive = !CompletionList.Active && expr.Position != position;
            foreach (MemberModel member in items)
            {
                if (testActive && member.Name == word) 
                    return;
                customList.Add(new MemberItem(member));
            }
            CompletionList.Show(customList, autoHide, word);

            if (reSelect != null) CompletionList.SelectItem(reSelect);
        }

		#endregion

		#region types_completion

        static private void SelectTypedNewMember(ScintillaControl sci)
        {
            try
            {
                ASExpr expr = GetExpression(sci, sci.CurrentPos);
                if (expr.Value == null) return;
                IASContext ctx = ASContext.Context;
                // try local var
                expr.LocalVars = ParseLocalVars(expr);
                foreach (MemberModel localVar in expr.LocalVars)
                {
                    if (localVar.LineTo == ctx.CurrentLine)
                    {
                        if (localVar.Type != null) // Might be non typed var
                        {
                            string typeName = localVar.Type;
                            ClassModel aClass = ctx.ResolveType(typeName, ctx.CurrentModel);
                            if (!aClass.IsVoid()) typeName = aClass.Type;
                            CompletionList.SelectItem(typeName);
                        }
                        return;
                    }
                }
                // try member
                string currentLine = sci.GetLine(sci.CurrentLine);
                Match mVarNew = Regex.Match(currentLine, "\\s*(?<name>[a-z_$][a-z._$0-9]*)(?<decl>[: ]*)(?<type>[a-z.0-9<>]*)\\s*=\\s*new\\s", RegexOptions.IgnoreCase);
                if (mVarNew.Success)
                {
                    string name = mVarNew.Groups["name"].Value;
                    ASResult result = EvalExpression(name, expr, ctx.CurrentModel, ctx.CurrentClass, true, false);
                    if (result != null && result.Member != null && result.Member.Type != null) // Might be missing or wrongly typed member
                    {
                        string typeName = result.Member.Type;
                        ClassModel aClass = ctx.ResolveType(typeName, ctx.CurrentModel);
                        if (!aClass.IsVoid()) typeName = aClass.Type;
                        CompletionList.SelectItem(typeName);
                    }
                }
            }
            catch {} // Do not throw exception with incorrect types
        }

		static private bool HandleNewCompletion(ScintillaControl Sci, string tail, bool autoHide, string keyword)
		{
            if (!ASContext.Context.Settings.LazyClasspathExploration
                && ASContext.Context.Settings.CompletionListAllTypes)
            {
                // show all project classes
                HandleAllClassesCompletion(Sci, tail, true, true);
                SelectTypedNewMember(Sci);
                return true;
            }

			// Consolidate known classes
			MemberList known = new MemberList();
            known.Merge(ASContext.Context.GetVisibleExternalElements());
            // show
            List<ICompletionListItem> list = new List<ICompletionListItem>();
			foreach(MemberModel member in known)
				list.Add(new MemberItem(new MemberModel(member.Type, member.Type, member.Flags, member.Access)));
			CompletionList.Show(list, autoHide, tail);
            SelectTypedNewMember(Sci);
			return true;
		}

		static private bool HandleImportCompletion(ScintillaControl Sci, string tail, bool autoHide)
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
                MemberList known = new MemberList();
                known.Merge(ASContext.Context.GetVisibleExternalElements());

                // show
                List<ICompletionListItem> list = new List<ICompletionListItem>();
                foreach (MemberModel member in known)
                    list.Add(new MemberItem(member));
                CompletionList.Show(list, autoHide, tail);
            }
			return true;
		}

		static private bool HandleColonCompletion(ScintillaControl Sci, string tail, bool autoHide)
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
                MemberList known = new MemberList();
                ClassModel cClass = ASContext.Context.CurrentClass;
                known.Merge(ASContext.Context.GetVisibleExternalElements());

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
            if (keyword == ASContext.Context.Features.varKey || keyword == ASContext.Context.Features.constKey)
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
                ContextFeatures features = ASContext.Context.Features;
                if (keyword == features.functionKey)
                    coma = ComaExpression.FunctionDeclaration;
                else
                {
                    keyword = GetWordLeft(Sci, ref position);
                    if (keyword == features.functionKey || keyword == features.getKey || keyword == features.setKey)
                        coma = ComaExpression.FunctionDeclaration;
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
        static public void HandleAllClassesCompletion(ScintillaControl Sci, string tail, bool classesOnly, bool showClassVars)
        {
            MemberList known = ASContext.Context.GetAllProjectClasses();
            if (known.Count == 0) return;

            // get local Class vars
            if (showClassVars)
            {
                MemberList found = new MemberList();

                ASExpr expr = GetExpression(Sci, Sci.CurrentPos);
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

            if (ASContext.Context.Features.hasDelegates && !ASContext.Context.CurrentClass.IsVoid())
            {
                foreach (MemberModel field in ASContext.Context.CurrentClass.Members)
                    if ((field.Flags & FlagType.Delegate) > 0)
                        known.Add(field);
            }

            List<ICompletionListItem> list = new List<ICompletionListItem>();
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

            CompletionList.Show(list, false, tail);
        }

        static private bool HandleInterpolationCompletion(ScintillaControl sci, bool autoHide, bool expressions)
        {
            IASContext ctx = ASContext.Context;
            MemberList members = new MemberList();
            ASExpr expr = GetExpression(sci, sci.CurrentPos);

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

        static private bool HandleMetadataCompletion(ScintillaControl sci, bool autoHide)
        {
            List<ICompletionListItem> list = new List<ICompletionListItem>();
            foreach (KeyValuePair<string, string> meta in ASContext.Context.Features.metadata)
            {
                MemberModel member = new MemberModel();
                member.Name = meta.Key;
                member.Comments = meta.Value;
                member.Type = "Compiler Metadata";
                list.Add(new MemberItem(member));
                CompletionList.Show(list, autoHide);
            }
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
        static private ASResult EvalExpression(string expression, ASExpr context, FileModel inFile, ClassModel inClass, bool complete, bool asFunction)
        {
            return EvalExpression(expression, context, inFile, inClass, complete, asFunction, true);
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
        static private ASResult EvalExpression(string expression, ASExpr context, FileModel inFile, ClassModel inClass, bool complete, bool asFunction, bool filterVisibility)
		{
			ASResult notFound = new ASResult();
            notFound.Context = context;
            if (string.IsNullOrEmpty(expression)) return notFound;

            ContextFeatures features = ASContext.Context.Features;
            if (expression.StartsWith(features.dot))
            {
                if (expression.StartsWith(features.dot + "#")) expression = expression.Substring(1);
                else if (context.Separator == '"') expression = '"' + expression;
                else return notFound;
            }

			string[] tokens = Regex.Split(expression, Regex.Escape(features.dot));

			// eval first token
			string token = tokens[0];
            if (token.Length == 0) return notFound;
            if (asFunction && tokens.Length == 1) token += "(";

			ASResult head;
            if (token.StartsWith("#"))
            {
                Match mSub = re_sub.Match(token);
                if (mSub.Success)
                {
                    bool haxeCast = false;
                    if (ASContext.Context.CurrentModel.haXe && context.SubExpressions.Contains("cast"))
                    {
                        haxeCast = true;
                        context.SubExpressions.Remove("cast");
                    }
                    string subExpr = context.SubExpressions[Convert.ToInt16(mSub.Groups["index"].Value)];
                    if (haxeCast) subExpr = subExpr.Replace(" ", "").Replace(",", " as ");
                    // parse sub expression
                    subExpr = subExpr.Substring(1, subExpr.Length - 2).Trim();
                    ASExpr subContext = new ASExpr(context);
                    subContext.SubExpressions = ExtractedSubex = new List<string>();
                    subExpr = re_balancedParenthesis.Replace(subExpr, new MatchEvaluator(ExtractSubex));
                    Match m = re_refineExpression.Match(subExpr);
                    if (!m.Success) return notFound;
                    Regex re_dot = new Regex("[\\s]*" + Regex.Escape(features.dot) + "[\\s]*");
                    subExpr = re_dot.Replace(re_whiteSpace.Replace(m.Value, " "), features.dot).Trim();
                    int space = subExpr.LastIndexOf(' ');
                    if (space > 0)
                    {
                        string trash = subExpr.Substring(0, space).TrimEnd();
                        subExpr = subExpr.Substring(space + 1);
                        if (trash.EndsWith("as")) subExpr += features.dot + "#";
                    }
                    // eval sub expression
                    head = EvalExpression(subExpr, subContext, inFile, inClass, true, false);
                    if (head.Member != null)
                        head.Type = ASContext.Context.ResolveType(head.Member.Type, head.Type.InFile);
                }
                else
                {
                    token = token.Substring(token.IndexOf('~') + 1);
                    head = EvalVariable(token, context, inFile, inClass);
                }
            }
            else if (token == "\"") // literal string
            {
                head = new ASResult();
                head.Type = ASContext.Context.ResolveType("String", null);
            }
            else head = EvalVariable(token, context, inFile, inClass); // regular eval

			// no head, exit
			if (head.IsNull()) return notFound;

            // accessing instance member in static function, exit
            if (IsStatic(context.ContextFunction) && context.WordBefore != features.overrideKey
                && head.RelClass == inClass
                && head.Member != null && !IsStatic(head.Member)
                && (head.Member.Flags & FlagType.Constructor) == 0)
                return notFound;

            // resolve
            ASResult result = EvalTail(context, inFile, head, tokens, complete, filterVisibility);

            // if failed, try as qualified class name
            if ((result == null || result.IsNull()) && tokens.Length > 1) 
            {
                ClassModel qualif = ASContext.Context.ResolveType(expression, null);
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

        static ASResult EvalTail(ASExpr context, FileModel inFile, ASResult head, string[] tokens, bool complete, bool filterVisibility)
        {
			// eval tail
			int n = tokens.Length;
			if (!complete) n--;
			// context
            ContextFeatures features = ASContext.Context.Features;
			ASResult step = head;
			ClassModel resultClass = head.Type;
			// look for static or dynamic members?
			FlagType mask = head.IsStatic ? FlagType.Static : FlagType.Dynamic;
            // members visibility
            IASContext ctx = ASContext.Context;
            ClassModel curClass = ctx.CurrentClass;
            curClass.ResolveExtends();
            Visibility acc = ctx.TypesAffinity(curClass, step.Type);

			// explore
            bool inE4X = false;
            string token = tokens[0];
            string path = token;
            step.Path = token;
            step.Context = context;

			for (int i=1; i<n; i++)
			{
                token = tokens[i];
                path += features.dot + token;
                step.Path = path;

                if (token.Length == 0)
                {
                    // this means 2 dots in the expression: consider as E4X expression
                    if (ctx.Features.hasE4X && IsXmlType(step.Type) && i < n - 1)
                    {
                        inE4X = true;
                        step = new ASResult();
                        step.Member = new MemberModel(token, "XMLList", FlagType.Variable | FlagType.Dynamic | FlagType.AutomaticVar, Visibility.Public);
                        step.Type = ctx.ResolveType(ctx.Features.objectKey, null);
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
                    resultClass = step.Type;
                    // handle typed indexes automatic typing
                    if (token[0] == '#' && step.InClass != null && step.Member != null
                        && step.InClass.IndexType == step.Member.Type)
                    {
                        step.InFile = step.InClass.InFile;
                    }


                    // if the current class ends back to the starting point (classA -> classB -> classA), 
                    // restore the private, protected, and internal member references
                    if (curClass != null && curClass == step.Type)
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

                    if (!step.IsStatic)
                    {
                        if ((mask & FlagType.Static) > 0)
                        {
                            mask -= FlagType.Static;
                            mask |= FlagType.Dynamic;
                        }
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

        private static bool IsStatic(MemberModel member)
        {
            return member != null && (member.Flags & FlagType.Static) > 0;
        }

		/// <summary>
		/// Find variable type in function context
		/// </summary>
		/// <param name="token">Variable name</param>
        /// <param name="context">Completion context</param>
        /// <param name="inFile">File context</param>
		/// <param name="inClass">Class context</param>
		/// <returns>Class/member struct</returns>
		static private ASResult EvalVariable(string token, ASExpr local, FileModel inFile, ClassModel inClass)
		{
			ASResult result = new ASResult();
            IASContext context = ASContext.Context;
            if (!inClass.IsVoid()) inFile = inClass.InFile;

            int p = token.IndexOf('(');
            if (p > 0) token = token.Substring(0, p);

            // top-level elements resolution
            context.ResolveTopLevelElement(token, result);
            if (!result.IsNull())
            {
                if (result.Member != null && (result.Member.Flags & FlagType.Function) > 0 && p < 0)
                    result.Type = ResolveType("Function", null);
                return result;
            }
            if (context.CurrentModel.haXe && !inClass.IsVoid() && token == "new" && local.BeforeBody)
                return EvalVariable(inClass.Name, local, inFile, inClass);
            // local vars
            if (local.LocalVars != null)
            {
                // Haxe 3 get/set keyword in properties declaration
                if ((token == "set" || token == "get") && local.ContextFunction == null 
                    && local.ContextMember != null && local.ContextMember.Parameters != null && local.ContextMember.Parameters.Count == 2)
                {
                    if (token == "get" && local.ContextMember.Parameters[0].Name == "get") return EvalVariable("get_" + local.ContextMember.Name, local, inFile, inClass);
                    if (token == "set" && local.ContextMember.Parameters[1].Name == "set") return EvalVariable("set_" + local.ContextMember.Name, local, inFile, inClass);
                }

                foreach (MemberModel var in local.LocalVars)
                {
                    if (var.Name == token)
                    {
                        result.Member = var;
                        result.InFile = inFile;
                        result.InClass = inClass;
                        if (var.Type == null && (var.Flags & FlagType.LocalVar) > 0
                            && context.Features.hasInference /*&& !context.Features.externalCompletion*/)
                            InferVariableType(local, var);

                        if ((var.Flags & FlagType.Function) > 0)
                            result.Type = ResolveType("Function", null);
                        else
                            result.Type = ResolveType(var.Type, inFile);

                        return result;
                    }
                }
            }
			// method parameters
            if (local.ContextFunction != null && local.ContextFunction.Parameters != null)
			{
                foreach(MemberModel para in local.ContextFunction.Parameters)
                if (para.Name == token || (para.Name[0] == '?' && para.Name.Substring(1) == token))
                {
                    result.Member = para;
                    result.Type = ResolveType(para.Type, inFile);
                    return result;
                }
			}
            // class members
            if (!inClass.IsVoid())
            {
                FindMember(token, inClass, result, 0, 0);
                if (!result.IsNull())
                    return result;
            }
            // file member
            if (inFile.Version != 2 || inClass.IsVoid())
            {
                FindMember(token, inFile, result, 0, 0);
                if (!result.IsNull())
                    return result;
            }
			// current file types
            foreach(ClassModel aClass in inFile.Classes)
            {
                if (aClass.Name == token)
                {
                    if (!context.InPrivateSection || aClass.Access == Visibility.Private)
                    {
                        result.Type = aClass;
                        result.IsStatic = (p < 0);
                        return result;
                    }
                }
            }
            // visible types & declarations
            var visible = context.GetVisibleExternalElements();
            foreach (MemberModel aDecl in visible)
            {
                if (aDecl.Name == token)
                {
                    if ((aDecl.Flags & FlagType.Package) > 0)
                    {
                        FileModel package = context.ResolvePackage(token, false);
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
                            foreach(ClassModel aClass in aDecl.InFile.Classes)
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
                    else if ((aDecl.Flags & FlagType.Variable) > 0)
                    {
                        result.Member = aDecl;
                        result.RelClass = ClassModel.VoidClass;
                        result.InClass = FindClassOf(aDecl);
                        result.Type = context.ResolveType(aDecl.Type, aDecl.InFile);
                        result.InFile = aDecl.InFile;
                        return result;
                    }
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
            IASContext context = ASContext.Context;
            if (qname == null) return ClassModel.VoidClass;
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
                            return context.GetModel(aDecl.InFile.Package, qname, inFile != null ? inFile.Package : null);
                        }
                        else return context.ResolveType(aDecl.Type, inFile);
                    }
                }

            return context.ResolveType(qname, inFile);
        }

        /// <summary>
        /// Infer very simple cases: var foo = {expression}
        /// </summary>
        private static void InferVariableType(ASExpr local, MemberModel var)
        {
            ScintillaControl sci = ASContext.CurSciControl;
            if (sci == null || var.LineFrom >= sci.LineCount) 
                return;
            // is it a simple affectation inference?
            string text = sci.GetLine(var.LineFrom);
            Regex reVar = new Regex("\\s*var\\s+" + var.Name + "\\s*=([^;]+)");
            Match m = reVar.Match(text);
            if (m.Success && m.Groups[1].Length > 1)
            {
                int p = text.IndexOf(';');
                text = text.TrimEnd();
                if (p < 0) p = text.Length;
                if (text.EndsWith("(")) p--;
                // resolve expression
                ASExpr expr = GetExpression(sci, sci.PositionFromLine(var.LineFrom) + p, true);
                if (!string.IsNullOrEmpty(expr.Value))
                {
                    ASResult result = EvalExpression(expr.Value, expr, ASContext.Context.CurrentModel, ASContext.Context.CurrentClass, true, false);
                    if (!result.IsNull())
                    {
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
                }
            }
        }

        private static void FindInPackage(string token, FileModel inFile, string pkg, ASResult result)
        {
            IASContext context = ASContext.Context;
            int p = token.IndexOf('(');

            FileModel inPackage = context.ResolvePackage(pkg, false);
            if (inPackage != null)
            {
                int pLen = pkg != null ? pkg.Length : 0;
                foreach (MemberModel friend in inPackage.Imports)
                {
                    if (friend.Name == token && (pLen == 0 || friend.Type.LastIndexOf(context.Features.dot) == pLen))
                    {
                        ClassModel friendClass = context.GetModel(inFile.Package, token, inFile.Package);
                        if (!friendClass.IsVoid())
                        {
                            result.Type = friendClass;
                            result.IsStatic = (p < 0);
                            return;
                        }
                        break;
                    }
                }
                foreach (MemberModel friend in inPackage.Members)
                {
                    if (friend.Name == token)
                    {
                        result.Member = friend;
                        result.Type = (p < 0 && (friend.Flags & FlagType.Function) > 0)
                            ? context.ResolveType("Function", null)
                            : context.ResolveType(friend.Type, friend.InFile);
                        return;
                    }
                }
            }
            return;
        }

        /// <summary>
        /// Find package-level member
        /// </summary>
        /// <param name="token">To match</param>
        /// <param name="inFile">In given file</param>
        /// <param name="result">Class/Member struct</param>
        /// <param name="mask">Flags mask</param>
        /// <param name="acc">Visibility mask</param>
        static public void FindMember(string token, FileModel inFile, ASResult result, FlagType mask, Visibility acc)
        {
            if (string.IsNullOrEmpty(token))
                return;

            // package
            if (result.IsPackage)
            {
                string fullName = (result.InFile.Package.Length > 0) ? result.InFile.Package + "." + token : token;
                int p;
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
                    else if ((p = mPack.Name.IndexOf('<')) > 0)
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

            MemberModel found;
            // variable
            found = inFile.Members.Search(token, mask, acc);
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
                return;
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
        static public void FindMember(string token, ClassModel inClass, ASResult result, FlagType mask, Visibility acc)
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
            if (token == "[]")
            {
                result.IsStatic = false;
                if (result.Type == null || result.Type.IndexType == null)
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
                            if (token.EndsWith("_mc") || token.StartsWith("mc")) autoType = "MovieClip";
                            else if (token.EndsWith("_txt") || token.StartsWith("txt")) autoType = "TextField";
                            else if (token.EndsWith("_btn") || token.StartsWith("bt")) autoType = "Button";
                        }
                        else if (tmpClass.InFile.Version == 3) 
                        {
                            if (token.EndsWith("_mc") || token.StartsWith("mc")) autoType = "flash.display.MovieClip";
                            else if (token.EndsWith("_txt") || token.StartsWith("txt")) autoType = "flash.text.TextField";
                            else if (token.EndsWith("_btn") || token.StartsWith("bt")) autoType = "flash.display.SimpleButton";
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
		static private List<string> ExtractedSubex;

        /// <summary>
        /// Find Actionscript expression at cursor position
        /// </summary>
        /// <param name="sci">Scintilla Control</param>
        /// <param name="position">Cursor position</param>
        /// <returns></returns>
        static private ASExpr GetExpression(ScintillaControl Sci, int position)
        {
            return GetExpression(Sci, position, false);
        }

		/// <summary>
		/// Find Actionscript expression at cursor position
		/// </summary>
		/// <param name="sci">Scintilla Control</param>
		/// <param name="position">Cursor position</param>
        /// <param name="ignoreWhiteSpace">Skip whitespace at position</param>
		/// <returns></returns>
        static private ASExpr GetExpression(ScintillaControl Sci, int position, bool ignoreWhiteSpace)
		{
            bool haXe = ASContext.Context.CurrentModel.haXe;
			ASExpr expression = new ASExpr();
			expression.Position = position;
			expression.Separator = ' ';

            // file's member declared at this position
            expression.ContextMember = ASContext.Context.CurrentMember;
            int minPos = 0;
            string body = null;
            if (expression.ContextMember != null)
            {
                minPos = Sci.PositionFromLine(expression.ContextMember.LineFrom);
                StringBuilder sbBody = new StringBuilder();
                for (int i = expression.ContextMember.LineFrom; i <= expression.ContextMember.LineTo; i++)
                    sbBody.Append(Sci.GetLine(i)).Append('\n');
                body = sbBody.ToString();
                //int tokPos = body.IndexOf(expression.ContextMember.Name);
                //if (tokPos >= 0) minPos += tokPos + expression.ContextMember.Name.Length;

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
                        expression.BeforeBody = (position < Sci.PositionFromLine(expression.ContextMember.LineFrom) + pos);
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
                    expression.BeforeBody = (eqPos < 0 || position < Sci.PositionFromLine(expression.ContextMember.LineFrom) + eqPos);
                }
            }

            // get the word characters from the syntax definition
            string characterClass = ScintillaControl.Configuration.GetLanguage(Sci.ConfigurationLanguage).characterclass.Characters;

            // get expression before cursor
            ContextFeatures features = ASContext.Context.Features;
            int stylemask = (1 << Sci.StyleBits) -1;
			int style = (position >= minPos) ? Sci.StyleAt(position) & stylemask : 0;
            StringBuilder sb = new StringBuilder();
            StringBuilder sbSub = new StringBuilder();
            int subCount = 0;
			char c = ' ';
            char c2;
            int startPos = position;
            int braceCount = 0;
            int sqCount = 0;
            int genCount = 0;
            bool hasGenerics = features.hasGenerics;
            bool hadWS = false;
            bool hadDot = ignoreWhiteSpace;
            int dotCount = 0;
            bool inRegex = false;
            char dot = features.dot[features.dot.Length-1];
            while (position > minPos)
            {
                position--;
                style = Sci.StyleAt(position) & stylemask;
                if (style == 14) // regex literal
                {
                    if (hadDot) inRegex = true;
                    else break;
                }
                else if (!IsCommentStyle(style))
                {
                    c2 = c;
                    c = (char)Sci.CharAt(position);
                    // end of regex literal
                    if (inRegex)
                    {
                        inRegex = false;
                        if (expression.SubExpressions == null) expression.SubExpressions = new List<string>();
                        expression.SubExpressions.Add("");
                        sb.Insert(0, "RegExp.#" + (subCount++) + "~");
                    }
                    // array access
                    if (c == '[')
                    {
                        sqCount--;
                        if (sqCount == 0)
                        {
                            if (sbSub.Length > 0) sbSub.Insert(0, '[');
                            if (braceCount == 0) sb.Insert(0, ".[]");
                            continue;
                        }
                        if (sqCount < 0)
                        {
                            expression.Separator = ';';
                            break;
                        }
                    }
                    else if (c == ']')
                    {
                        sqCount++;
                    }
                    //
                    else if (c == '<' && hasGenerics)
                    {
                        genCount--;
                        if (genCount < 0)
                        {
                            expression.Separator = ';';
                            break;
                        }
                    }
                    // ignore sub-expressions (method calls' parameters)
                    else if (c == '(')
                    {
                        braceCount--;
                        if (braceCount == 0 && sqCount == 0)
                        {
                            int testPos = position - 1;
                            string testWord = GetWordLeft(Sci, ref testPos);

                            sbSub.Insert(0, c);
                            if (haXe && testWord == "cast") expression.SubExpressions.Add("cast");
                            expression.SubExpressions.Add(sbSub.ToString());
                            sb.Insert(0, ".#" + (subCount++) + "~"); // method call or sub expression

                            if (testWord == "return" || testWord == "case" || testWord == "defaut" || (haXe && testWord == "cast"))
                            {
                                // AS3, AS2, Loom ex: return (a as B).<complete>
                                // Haxe ex: return cast(a, B).<complete>
                                expression.Separator = ';';
                                expression.WordBefore = testWord;
                                break;
                            }
                            else continue;
                        }
                        else if (braceCount < 0)
                        {
                            expression.Separator = ';';
                            int testPos = position - 1;
                            string testWord = GetWordLeft(Sci, ref testPos); // anonymous function
                            string testWord2 = GetWordLeft(Sci, ref testPos) ?? "null"; // regular function
                            if (testWord == features.functionKey || testWord == "catch"
                                || testWord2 == features.functionKey
                                || testWord2 == features.getKey || testWord2 == features.setKey)
                            {
                                expression.Separator = ',';
                                expression.coma = ComaExpression.FunctionDeclaration;
                            }
                            else expression.WordBefore = testWord;
                            break;
                        }
                    }
                    else if (c == ')')
                    {
                        if (!hadDot)
                        {
                            expression.Separator = ';';
                            break;
                        }
                        if (braceCount == 0) // start sub-expression
                        {
                            if (expression.SubExpressions == null) expression.SubExpressions = new List<string>();
                            sbSub = new StringBuilder();
                        }
                        braceCount++;
                    }
                    else if (c == '>' && hasGenerics)
                    {
                        if (c2 == '.' || c2 == '(')
                            genCount++;
                        else break;
                    }
                    if (braceCount > 0 || sqCount > 0 || genCount > 0) 
                    {
                        if (c == ';') // not expected: something's wrong
                        {
                            expression.Separator = ';';
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
                    }
                    else if (c == dot)
                    {
                        if (features.dot.Length == 2)
                            hadDot = position > 0 && Sci.CharAt(position - 1) == features.dot[0];
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
                    else if (characterClass.IndexOf(c) >= 0)
                    {
                        if (hadWS && !hadDot)
                        {
                            expression.Separator = ' ';
                            break;
                        }
                        hadWS = false;
                        hadDot = false;
                        dotCount = 0;
                        sb.Insert(0, c);
                        startPos = position;
                    }
                    else if (c == ';')
                    {
                        expression.Separator = ';';
                        break;
                    }
                    else if (hasGenerics && (genCount > 0 || c == '<'))
                    {
                        sb.Insert(0, c);
                    }
                    else if (c == '{')
                    {
                        expression.coma = DisambiguateComa(Sci, position, minPos);
                        expression.Separator = (expression.coma == ComaExpression.None) ? ';' : ',';
                        break;
                    }
                    else if (c == ',')
                    {
                        expression.coma = DisambiguateComa(Sci, position, minPos);
                        expression.Separator = (expression.coma == ComaExpression.None) ? ';' : ',';
                        break;
                    }
                    else if (c == ':')
                    {
                        expression.Separator = ':';
                        break;
                    }
                    else if (c == '=')
                    {
                        expression.Separator = '=';
                        break;
                    }
                    else //if (hadWS && !hadDot)
                    {
                        if (c == '\'' || c == '"') expression.Separator = '"';
                        else expression.Separator = ';';
                        break;
                    }
                }
                // string literals only allowed in sub-expressions
                else
                {
                    if (braceCount == 0) // not expected: something's wrong
                    {
                        expression.Separator = ';';
                        break;
                    }
                }
            }

            // check if there is a particular keyword
            if (expression.Separator == ' ') 
            {
                expression.WordBefore = GetWordLeft(Sci, ref position);
            }

			// result
            expression.Value = sb.ToString();
            expression.PositionExpression = startPos;
            LastExpression = expression;
			return expression;
		}

        /// <summary>
        /// Find out in what context is a coma-separated expression
        /// </summary>
        /// <returns></returns>
        private static ComaExpression DisambiguateComa(ScintillaControl Sci, int position, int minPos)
        {
            ContextFeatures features = ASContext.Context.Features;
            // find block start '(' or '{'
            int parCount = 0;
            int braceCount = 0;
            int sqCount = 0;
            char c = (char)Sci.CharAt(position);
            bool wasPar = false;
            //if (c == '{') { wasPar = true; position--; }
            while (position > minPos)
            {
                c = (char)Sci.CharAt(position);
                if (c == ';')
                {
                    return ComaExpression.None;
                }
                else if ((c == ',' || c == '=') && wasPar)
                {
                    return ComaExpression.AnonymousObject;
                }
                // var declaration
                else if (c == ':')
                {
                    position--;
                    string word = GetWordLeft(Sci, ref position);
                    word = GetWordLeft(Sci, ref position);
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
                else if (c == ']')
                {
                    if (wasPar) return ComaExpression.None;
                    sqCount++;
                }
                // function declaration or parameter
                else if (c == '(')
                {
                    parCount--;
                    if (parCount < 0)
                    {
                        position--;
                        string word1 = GetWordLeft(Sci, ref position);
                        if (word1 == features.functionKey) return ComaExpression.FunctionDeclaration; // anonymous function
                        string word2 = GetWordLeft(Sci, ref position);
                        if (word2 == features.functionKey || word2 == features.setKey || word2 == features.getKey)
                            return ComaExpression.FunctionDeclaration; // function declaration
                        if (features.hasDelegates && word2 == "delegate")
                            return ComaExpression.FunctionDeclaration; // delegate declaration
                        return ComaExpression.FunctionParameter; // function call
                    }
                }
                else if (c == ')')
                {
                    if (wasPar) return ComaExpression.None;
                    parCount++;
                }
                // code block or anonymous object
                else if (c == '{')
                {
                    braceCount--;
                    if (braceCount < 0)
                    {
                        position--;
                        string word1 = GetWordLeft(Sci, ref position);
                        c = (word1.Length > 0) ? word1[word1.Length - 1] : (char)Sci.CharAt(position);
                        if (":,(=".IndexOf(c) >= 0)
                        {
                            string line = Sci.GetLine(Sci.LineFromPosition(position));
                            if (Regex.IsMatch(line, @"\b(case|default)\b.*:")) break; // case: code block
                            return ComaExpression.AnonymousObjectParam;
                        }
                        else if (c != ')' && c != '}' && !Char.IsLetterOrDigit(c)) return ComaExpression.AnonymousObject;
                        break;
                    }
                }
                else if (c == '}')
                {
                    if (wasPar) return ComaExpression.None;
                    braceCount++;
                }
                else if (c == '?') return ComaExpression.AnonymousObject;
                position--;
            }
            return ComaExpression.None;
        }

		/// <summary>
		/// Parse function body for local var definitions
		/// TODO  ASComplete: parse coma separated local vars definitions
		/// </summary>
		/// <param name="expression">Expression source</param>
		/// <returns>Local vars dictionnary (name, type)</returns>
		static public MemberList ParseLocalVars(ASExpr expression)
		{
			FileModel model;
            if (expression.FunctionBody != null && expression.FunctionBody.Length > 0)
            {
                MemberModel cm = expression.ContextMember;
                string functionBody = Regex.Replace(expression.FunctionBody, "function\\s*\\(", "function __anonfunc__("); // name anonymous functions
                model = ASContext.Context.GetCodeModel(functionBody);
                int memberCount = model.Members.Count;
                for (int memberIndex = 0; memberIndex < memberCount; memberIndex++)
                {
                    MemberModel member = model.Members[memberIndex];

                    if (cm.Equals(member)) continue;

                    member.Flags |= FlagType.LocalVar;
                    member.LineFrom += expression.FunctionOffset;
                    member.LineTo += expression.FunctionOffset;

                    if ((member.Flags & FlagType.Function) == FlagType.Function)
                    {
                        if (member.Name == "__anonfunc__")
                        {
                            model.Members.Remove(member);
                            memberCount--;
                            memberIndex--;
                        }

                        if (member.Parameters == null) continue;

                        foreach (MemberModel parameter in member.Parameters)
                        {
                            parameter.LineFrom += expression.FunctionOffset;
                            parameter.LineTo += expression.FunctionOffset;
                            model.Members.Add(parameter);
                        }
                    }
                }
            }
            else model = new FileModel();
            if (expression.ContextFunction != null && expression.ContextFunction.Parameters != null)
            {
                ContextFeatures features = ASContext.Context.Features;
                foreach (MemberModel item in expression.ContextFunction.Parameters)
                {
                    if (item.Name.StartsWith(features.dot)) 
                        model.Members.Merge(new MemberModel(item.Name.Substring(item.Name.LastIndexOf(features.dot) + 1), "Array", item.Flags, item.Access));
                    else if (item.Name[0] == '?') model.Members.Merge(new MemberModel(item.Name.Substring(1), item.Type, item.Flags, item.Access));
                    else model.Members.Merge(item);
                }
                if (features.functionArguments != null)
                    model.Members.Add(ASContext.Context.Features.functionArguments);
            }
            model.Members.Sort();
            return model.Members;
		}

		/// <summary>
		/// Extract sub-expressions
		/// </summary>
		static private string ExtractSubex(Match m)
		{
            ExtractedSubex.Add(m.Value);
            return ".#" + (ExtractedSubex.Count - 1) + "~";
		}
		#endregion

		#region tools_functions


        static public bool IsLiteralStyle(int style)
        {
            return (style == 4) || (style == 6) || (style == 7);
        }

        /// <summary>
        /// Text is word 
        /// </summary>
        static public bool IsTextStyle(int style)
		{
			return style == 0 || style == 10 /*punctuation*/ || style == 11 /*identifier*/ 
                || style == 16 /*word2 (secondary keywords: class name)*/
                || style == 24 /*word4 (add keywords4)*/ || style == 25 /*word5 (add keywords5)*/
                || style == 127 /*PHP*/;
		}

        /// <summary>
        /// Text is word or keyword
        /// </summary>
        static public bool IsTextStyleEx(int style)
		{
            return style == 0 || style == 5 /*word (secondary keywords)*/
                || style == 10 /*punctuation*/ || style == 11 /*identifier*/ 
                || style == 16 /*word2 (secondary keywords: class name)*/ 
                || style == 19 /*globalclass (primary keywords)*/ || style == 23 /*word3 (add keywords3)*/
                || style == 24 /*word4 (add keywords4)*/ || style == 25 /*word5 (add keywords5)*/
                || style == 127 /*PHP*/;
		}

		static public bool IsCommentStyle(int style)
		{
			return style == 1 || style == 2 || style == 3 /*comments*/
                || style == 17 || style == 18 /*javadoc tags*/;
		}

		static public string GetWordLeft(ScintillaControl Sci, ref int position)
		{
            // get the word characters from the syntax definition
            string characterClass = ScintillaControl.Configuration.GetLanguage(Sci.ConfigurationLanguage).characterclass.Characters;

			string word = "";
			//string exclude = "(){};,+*/\\=:.%\"<>";
			bool skipWS = true;
			int style;
			int stylemask = (1 << Sci.StyleBits) -1;
			char c;
			while (position >= 0)
			{
				style = Sci.StyleAt(position) & stylemask;
				if (IsTextStyleEx(style))
				{
					c = (char)Sci.CharAt(position);
					if (c <= ' ')
					{
						if (!skipWS)
							break;
					}
                    else if (characterClass.IndexOf(c) < 0) break;
					else if (style != 6)
					{
						word = c+word;
						skipWS = false;
					}
				}
				position--;
			}
			return word;
		}

        static public ASResult GetExpressionType(ScintillaControl sci, int position)
        {
            return GetExpressionType(sci, position, true);
        }

		static public ASResult GetExpressionType(ScintillaControl sci, int position, bool filterVisibility)
		{
            // context
            int line = sci.LineFromPosition(position);
            if (line != ASContext.Context.CurrentLine) 
                ASContext.Context.UpdateContext(line);
            try
            {
                ASExpr expr = GetExpression(sci, position);
                expr.LocalVars = ParseLocalVars(expr);
                if (string.IsNullOrEmpty(expr.Value))
                {
                    ASResult res = new ASResult();
                    res.Context = expr;
                    return res;
                }
                FileModel aFile = ASContext.Context.CurrentModel;
                ClassModel aClass = ASContext.Context.CurrentClass;
                // Expression before cursor
                return EvalExpression(expr.Value, expr, aFile, aClass, true, false, filterVisibility);
            }
            finally
            {
                // restore context
                if (line != ASContext.Context.CurrentLine) 
                    ASContext.Context.UpdateContext(ASContext.Context.CurrentLine);
            }
		}

        /// <summary>
        /// Returns whether or not position is insidse of an expression
        /// block in Haxe String interpolation ('${expr}')
        /// </summary>
        static private bool IsInterpolationExpr(ScintillaControl sci, int position)
        {
            ContextFeatures features = ASContext.Context.Features;
            if (!features.hasStringInterpolation ||
                features.stringInterpolationQuotes.IndexOf(sci.GetStringType(position)) < 0)
                return false;

            char prev = ' ';
            char c = ' ';
            char next = (char)sci.CharAt(position);
            for (int i = position; i > 0; i--)
            {
                prev = c;
                c = next;
                next = (char)sci.CharAt(i);

                if (c == '\'')
                    if (next != '\\')
                        return false;
                if (c == '$' && prev == '{')
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Whether the character at the position is inside of the
        /// brackets of haxe metadata (@:allow(path) etc)
        /// </summary>
        static private bool IsMetadataArgument(ScintillaControl sci, int position)
        {
            if (!ASContext.Context.CurrentModel.haXe || ASContext.Context.CurrentMember != null)
                return false;

            char c = ' ';
            char next = (char)sci.CharAt(position);
            bool openingBracket = false;

            for (int i = position; i > 0; i--)
            {
                c = next;
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
            return model != null
                && (model.QualifiedName == "XML" || model.QualifiedName == "XMLList");
        }

		#endregion

		#region tooltips formatting
		static public string GetToolTipText(ASResult result)
		{
			if (result.Member != null && result.InClass != null)
			{
                return MemberTooltipText(result.Member, result.InClass) + GetToolTipDoc(result.Member);
			}
			else if (result.Member != null && (result.Member.Flags & FlagType.Constructor) != FlagType.Constructor)
			{
                return MemberTooltipText(result.Member, ClassModel.VoidClass) + GetToolTipDoc(result.Member);
			}
			else if (result.InClass != null)
			{
                return ClassModel.ClassDeclaration(result.InClass) + GetToolTipDoc(result.InClass);
			}
			else if (result.Type != null)
			{
                return ClassModel.ClassDeclaration(result.Type) + GetToolTipDoc(result.Type);
			}
			else return null;
		}

        private static string GetToolTipDoc(MemberModel model)
        {
            string details = (UITools.Manager.ShowDetails) ? ASDocumentation.GetTipFullDetails(model, null) : ASDocumentation.GetTipShortDetails(model, null);
            return details.TrimStart(new char[] { ' ', '\u2026' });
        }

		static private string MemberTooltipText(MemberModel member, ClassModel inClass)
		{
			// modifiers
            FlagType ft = member.Flags;
            Visibility acc = member.Access;
			string modifiers = "";
			if ((ft & FlagType.Class) == 0)
            {
                if ((ft & FlagType.LocalVar) > 0)
                    modifiers += "(local) ";
                else if ((ft & FlagType.ParameterVar) > 0)
                    modifiers += "(parameter) ";
                else if ((ft & FlagType.AutomaticVar) > 0)
                    modifiers += "(auto) ";
                else
                {
                    if ((ft & FlagType.Extern) > 0)
                        modifiers += "extern ";
                    if ((ft & FlagType.Native) > 0)
                        modifiers += "native ";
                    if ((ft & FlagType.Static) > 0)
                        modifiers += "static ";
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
            string foundIn = "";
            if (inClass != ClassModel.VoidClass)
            {
                string package = inClass.InFile.Package;
                foundIn = "\n[COLOR=#666666:MULTIPLY]in " + MemberModel.FormatType(inClass.QualifiedName) + "[/COLOR]";
            }
            if ((ft & (FlagType.Getter | FlagType.Setter)) > 0)
                return String.Format("{0}property {1}{2}", modifiers, member.ToString(), foundIn);
            else if (ft == FlagType.Function)
                return String.Format("{0}function {1}{2}", modifiers, member.ToString(), foundIn);
            else if ((ft & FlagType.Namespace) > 0)
                return String.Format("{0}namespace {1}{2}", modifiers, member.Name, foundIn);
            else if ((ft & FlagType.Constant) > 0)
            {
                if (member.Value == null)
                    return String.Format("{0}const {1}{2}", modifiers, member.ToString(), foundIn);
                else
                    return String.Format("{0}const {1} = {2}{3}", modifiers, member.ToString(), member.Value, foundIn);
            }
            else if ((ft & FlagType.Variable) > 0)
                return String.Format("{0}var {1}{2}", modifiers, member.ToString(), foundIn);
            else if ((ft & FlagType.Delegate) > 0)
                return String.Format("{0}delegate {1}{2}", modifiers, member.ToString(), foundIn);
            else
                return String.Format("{0}{1}{2}", modifiers, member.ToString(), foundIn);
		}
		#endregion

		#region automatic code generation
		static private ASExpr LastExpression;

        /// <summary>
        /// When typing a fully qualified class name:
        /// - automatically insert import statement 
        /// - replace with short name
        /// </summary>
        static internal void HandleCompletionInsert(ScintillaControl sci, int position, string text, char trigger, ICompletionListItem item)
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
                // was a fully qualified type inserted?
                ASExpr expr = GetExpression(sci, position + text.Length);
                if (expr.Value == null) return;

                // look for a snippet
                ContextFeatures features = ASContext.Context.Features;
                if (trigger == '\t' && expr.Value.IndexOf(features.dot) < 0)
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
            try
            {
                ClassModel import = (item as EventItem).EventType;
                if (!ASContext.Context.IsImported(import, sci.LineFromPosition(position)))
                {
                    if (ASContext.Context.Settings.GenerateImports)
                    {
                        int offset = ASGenerator.InsertImport(import, true);
                        if (offset > 0)
                        {
                            position += offset;
                            sci.SetSel(position, position);
                        }
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
                import = context.Member.Clone() as MemberModel;
                import.Type = inFile.Package + "." + import.Name;
            }
            // if not completed a type
            else if (context.IsNull() || !context.IsStatic || context.Type == null
                || (context.Type.Type != null && context.Type.Type.IndexOf(features.dot) < 0)
                || context.Type.IsVoid())
            {
                if (context.Member != null && expr.Separator == ' '
                    && expr.WordBefore == features.overrideKey)
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
                else if (!context.IsNull())
                {
                    if (expr.WordBefore == features.importKey)
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

            if (expr.Separator == ' ' && expr.WordBefore != null && expr.WordBefore != "")
            {
                if (expr.WordBefore == features.importKey || expr.WordBefore == features.importKeyAlt
                    /*|| (!features.HasTypePreKey(expr.WordBefore) && expr.WordBefore != "case" && expr.WordBefore != "return")*/)
                {
                    if (expr.WordBefore == features.importKey || expr.WordBefore == features.importKeyAlt)
                        ASContext.Context.RefreshContextCache(expr.Value);
                    return true;
                }
            }

            int offset = 0;
            int startPos = expr.PositionExpression;
            int endPos = sci.CurrentPos;

            if (ASContext.Context.Settings.GenerateImports && shouldShortenType(sci, position, import, cFile, ref offset))
            {
                // insert short name
                startPos += offset;
                endPos += offset;
                sci.SetSel(startPos, endPos);
                sci.ReplaceSel(checkShortName(import.Name));
                sci.SetSel(sci.CurrentPos, sci.CurrentPos);
            }            
            return true;
        }

        private static bool shouldShortenType(ScintillaControl sci, int position, MemberModel import, FileModel cFile, ref int offset)
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

        private static string checkShortName(string name)
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
		static private bool CodeAutoOnChar(ScintillaControl sci, int value)
		{
			if (ASContext.Context.Settings == null || !ASContext.Context.Settings.GenerateImports)
				return false;

			int position = sci.CurrentPos;

			if (value == '*' && position > 1 && sci.CharAt(position-2) == '.' && LastExpression != null)
			{
				// context
				if (LastExpression.Separator == ' ' && LastExpression.WordBefore != null 
                    && !ASContext.Context.Features.HasTypePreKey(LastExpression.WordBefore))
					return false;

                FileModel cFile = ASContext.Context.CurrentModel;
                ClassModel cClass = ASContext.Context.CurrentClass;
				ASResult context = EvalExpression(LastExpression.Value, LastExpression, cFile, cClass, true, false);
                if (context.IsNull() || !context.IsPackage || context.InFile == null)
					return false;

				string package = LastExpression.Value;
				int startPos = LastExpression.Position;
				string check = "";
				char c;
				while (startPos > LastExpression.PositionExpression && check.Length <= package.Length && check != package)
				{
					c = (char)sci.CharAt(--startPos);
					if (c > 32) check = c+check;
				}
				if (check != package)
					return false;

				// insert import
                string statement = "import " + package + "*;" + LineEndDetector.GetNewLineMarker(sci.EOLMode);
				int endPos = sci.CurrentPos;
				int line = 0;
				int curLine = sci.LineFromPosition(position);
				bool found = false;
				while (line < curLine)
				{
					if (sci.GetLine(line++).IndexOf("import") >= 0) found = true;
					else if (found) {
						line--;
						break;
					}
				}
				if (line == curLine) line = 0;
				position = sci.PositionFromLine(line);
				line = sci.FirstVisibleLine;
				sci.SetSel(position, position);
				sci.ReplaceSel(statement);

				// prepare insertion of the term as usual
				startPos += statement.Length;
				endPos += statement.Length;
				sci.SetSel(startPos, endPos);
				sci.ReplaceSel("");
				sci.LineScroll(0, line-sci.FirstVisibleLine+1);

				// create classes list
                List<ICompletionListItem> list = new List<ICompletionListItem>();
				foreach(MemberModel import in cClass.InFile.Imports)
				if (import.Type.StartsWith(package))
					list.Add(new MemberItem(import));
				CompletionList.Show(list, false);
				return true;
			}
			return false;
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

        public System.Drawing.Bitmap Icon
        {
            get { return (System.Drawing.Bitmap)ASContext.Panel.GetIcon(icon); }
        }

        public string Value
        {
            get 
            {
                if (member.Name.IndexOf('<') > 0)
                {
                    if (member.Name.IndexOf(".<") > 0) 
                        return member.Name.Substring(0, member.Name.IndexOf(".<"));
                    else return member.Name.Substring(0, member.Name.IndexOf('<'));
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

        public System.Drawing.Bitmap Icon
        {
            get { return (System.Drawing.Bitmap)ASContext.Panel.GetIcon(PluginUI.ICON_DECLARATION); }
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

        public System.Drawing.Bitmap Icon
        {
            get { return (System.Drawing.Bitmap)ASContext.Panel.GetIcon(PluginUI.ICON_CONST); }
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
	sealed public class ASExpr
	{
		public int Position;
        public MemberModel ContextMember;
        public MemberList LocalVars;
        public MemberModel ContextFunction;
        public string FunctionBody;
        public int FunctionOffset;
        public bool BeforeBody;

        public int PositionExpression;
		public string Value;
        public List<string> SubExpressions;
		public char Separator;
		public string WordBefore;
        public ComaExpression coma;

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
	sealed public class ASResult
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

    sealed class Braces
    {
        public char opening;
        public char closing;

        public Braces(char opening, char closing)
        {
            this.opening = opening;
            this.closing = closing;
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


