using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using FlashDevelop.Helpers;
using PluginCore;
using PluginCore.Controls;
using PluginCore.Helpers;
using PluginCore.Localization;
using PluginCore.Managers;
using PluginCore.Utilities;
using ScintillaNet;
using ScintillaNet.Configuration;
using ScintillaNet.Enums;
using Keys = System.Windows.Forms.Keys;

namespace FlashDevelop.Managers
{
    class ScintillaManager
    {
        public static Bitmap Bookmark;
        private static bool initialized;
        private static Scintilla sciConfig;
        private static ConfigurationUtility sciConfigUtil;
        private static readonly object initializationLock = new object();
        public static event Action ConfigurationLoaded;

        internal const int LineMargin = 2;
        internal const int BookmarksMargin = 0;
        internal const int FoldingMargin = 3;

        static ScintillaManager()
        {
            Bookmark = ScaleHelper.Scale(new Bitmap(ResourceHelper.GetStream("BookmarkIcon.png")));
        }

        /// <summary>
        /// Initializes the config loading
        /// </summary>
        private static void Initialize()
        {
            if (!initialized)
            {
                lock (initializationLock)
                {
                    if (!initialized)
                    {
                        LoadConfiguration();
                        initialized = true;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the SciConfig and initializes
        /// </summary>
        public static Scintilla SciConfig
        {
            get
            {
                Initialize();
                return sciConfig;
            }
        }

        /// <summary>
        /// Gets the SciConfigUtil and initializes
        /// </summary>
        public static ConfigurationUtility SciConfigUtil
        {
            get
            {
                Initialize();
                return sciConfigUtil;
            }
        }

        /// <summary>
        /// Loads the syntax and refreshes scintilla settings.
        /// </summary>
        public static void LoadConfiguration()
        {
            sciConfigUtil = new ConfigurationUtility(Assembly.GetExecutingAssembly());
            string[] configFiles = Directory.GetFiles(Path.Combine(PathHelper.SettingDir, "Languages"), "*.xml");
            sciConfig = (Scintilla)sciConfigUtil.LoadConfiguration(configFiles);
            ScintillaControl.Configuration = sciConfig;
            ConfigurationLoaded?.Invoke();
        }

        /// <summary>
        /// Update the manually syncable properties if needed.
        /// </summary>
        public static void UpdateSyncProps(ScintillaControl e1, ScintillaControl e2)
        {
            if (e2.SaveBOM != e1.SaveBOM) e2.SaveBOM = e1.SaveBOM;
            if (e2.Encoding != e1.Encoding) e2.Encoding = e1.Encoding;
            if (e2.FileName != e1.FileName) e2.FileName = e1.FileName;
            if (e2.SmartIndentType != e1.SmartIndentType) e2.SmartIndentType = e1.SmartIndentType;
            if (e2.UseHighlightGuides != e1.UseHighlightGuides) e2.UseHighlightGuides = e1.UseHighlightGuides;
            if (e2.ConfigurationLanguage != e1.ConfigurationLanguage) e2.ConfigurationLanguage = e1.ConfigurationLanguage;
            if (e2.IsHiliteSelected != e1.IsHiliteSelected) e2.IsHiliteSelected = e1.IsHiliteSelected;
            if (e2.IsBraceMatching != e1.IsBraceMatching) e2.IsBraceMatching = e1.IsBraceMatching;
        }

        /// <summary>
        /// Detect syntax, ask from plugins if its correct and update
        /// </summary>
        public static void UpdateControlSyntax(ScintillaControl sci)
        {
            string language = SciConfig.GetLanguageFromFile(sci.FileName);
            TextEvent te = new TextEvent(EventType.SyntaxDetect, language);
            EventManager.DispatchEvent(SciConfig, te);
            if (te.Handled && te.Value != null) language = te.Value;
            if (sci.ConfigurationLanguage != language)
            {
                sci.ConfigurationLanguage = language;
            }
            ApplySciSettings(sci);
        }

        /// <summary>
        /// Does the cleanup tasks to the code
        /// </summary>
        public static void CleanUpCode(ScintillaControl sci)
        {
            try
            {
                if (Globals.Settings.EnsureLastLineEnd)
                {
                    sci.AddLastLineEnd();
                }
                if (Globals.Settings.EnsureConsistentLineEnds)
                {
                    sci.ConvertEOLs(sci.EOLMode);
                }
                if (Globals.Settings.StripTrailingSpaces)
                {
                    sci.StripTrailingSpaces(Globals.Settings.KeepIndentTabs);
                }
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Makes the read only file writable
        /// </summary>
        public static void MakeFileWritable(ScintillaControl sci)
        {
            try
            {
                string file = sci.FileName;
                File.SetAttributes(file, FileAttributes.Normal);
                sci.IsReadOnly = false;
                sci.Focus();
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Detects from codepage if the encoding is unicode
        /// </summary>
        public static bool IsUnicode(int codepage)
        {
            return (codepage == Encoding.UTF7.CodePage
                || codepage == Encoding.UTF8.CodePage
                || codepage == Encoding.UTF32.CodePage
                || codepage == Encoding.BigEndianUnicode.CodePage
                || codepage == Encoding.Unicode.CodePage);
        }

        /// <summary>
        /// Gets the line comment string
        /// </summary>
        public static string GetLineComment(string lang)
        {
            Language obj = SciConfig.GetLanguage(lang);
            if (obj.linecomment != null)
            {
                return obj.linecomment;
            }
            return string.Empty;
        }

        /// <summary>
        /// Gets the comment start string
        /// </summary>
        public static string GetCommentStart(string lang)
        {
            Language obj = SciConfig.GetLanguage(lang);
            if (obj.commentstart != null)
            {
                return obj.commentstart;
            }
            return string.Empty;
        }

        /// <summary>
        /// Gets the comment end string
        /// </summary>
        public static string GetCommentEnd(string lang)
        {
            Language obj = SciConfig.GetLanguage(lang);
            if (obj.commentend != null)
            {
                return obj.commentend;
            }
            return string.Empty;
        }

        /// <summary>
        /// Changes the current document's language
        /// </summary>
        public static void ChangeSyntax(string lang, ScintillaControl sci)
        {
            sci.StyleClearAll();
            sci.StyleResetDefault();
            sci.ClearDocumentStyle();
            sci.ConfigurationLanguage = lang;
            sci.Colourise(0, -1);
            sci.Refresh();
            ButtonManager.UpdateFlaggedButtons();
            Globals.MainForm.OnSyntaxChange(lang);
        }

        /// <summary>
        /// Updates editor Globals.Settings to the specified ScintillaControl
        /// </summary>
        public static void ApplySciSettings(ScintillaControl sci)
        {
            ApplySciSettings(sci, false);
        }
        public static void ApplySciSettings(ScintillaControl sci, bool hardUpdate)
        {
            try
            {
                ISettings settings = PluginBase.Settings;
                sci.CaretPeriod = settings.CaretPeriod;
                sci.CaretWidth = settings.CaretWidth;
                sci.EOLMode = LineEndDetector.DetectNewLineMarker(sci.Text, (int)settings.EOLMode);
                sci.IsBraceMatching = settings.BraceMatchingEnabled;
                sci.UseHighlightGuides = !settings.HighlightGuide;
                sci.Indent = settings.IndentSize;
                sci.SmartIndentType = settings.SmartIndentType;
                sci.IsBackSpaceUnIndents = settings.BackSpaceUnIndents;
                sci.IsCaretLineVisible = settings.CaretLineVisible;
                sci.IsIndentationGuides = settings.ViewIndentationGuides;
                sci.IndentView = settings.IndentView;
                sci.IsTabIndents = settings.TabIndents;
                sci.IsUseTabs = settings.UseTabs;
                sci.IsViewEOL = settings.ViewEOL;
                sci.ScrollWidth = Math.Max(settings.ScrollWidth, 1);
                sci.ScrollWidthTracking = settings.ScrollWidth == 0 || settings.ScrollWidth == 3000;
                sci.TabWidth = settings.TabWidth;
                sci.ViewWS = Convert.ToInt32(settings.ViewWhitespace);
                sci.WrapMode = Convert.ToInt32(settings.WrapText);
                sci.SetProperty("fold", Convert.ToInt32(settings.UseFolding).ToString());
                sci.SetProperty("fold.comment", Convert.ToInt32(settings.FoldComment).ToString());
                sci.SetProperty("fold.compact", Convert.ToInt32(settings.FoldCompact).ToString());
                sci.SetProperty("fold.preprocessor", Convert.ToInt32(settings.FoldPreprocessor).ToString());
                sci.SetProperty("fold.at.else", Convert.ToInt32(settings.FoldAtElse).ToString());
                sci.SetProperty("fold.html", Convert.ToInt32(settings.FoldHtml).ToString());
                sci.SetProperty("lexer.cpp.track.preprocessor", "0");
                sci.SetVirtualSpaceOptions((int)settings.VirtualSpaceMode);
                sci.SetFoldFlags((int)settings.FoldFlags);
                /**
                * Set if themes should colorize the first margin
                */
                Language language = SciConfig.GetLanguage(sci.ConfigurationLanguage);
                if (language?.editorstyle != null)
                {
                    bool colorizeMarkerBack = language.editorstyle.ColorizeMarkerBack;
                    if (colorizeMarkerBack) sci.SetMarginTypeN(BookmarksMargin, (int)MarginType.Fore);
                    else sci.SetMarginTypeN(BookmarksMargin, (int)MarginType.Symbol);
                }
                /**
                * Set correct line number margin width
                */
                bool viewLineNumbers = settings.ViewLineNumbers;
                if (viewLineNumbers) sci.SetMarginWidthN(LineMargin, ScaleArea(sci, 36));
                else sci.SetMarginWidthN(LineMargin, 0);
                /**
                * Set correct bookmark margin width
                */
                bool viewBookmarks = settings.ViewBookmarks;
                if (viewBookmarks) sci.SetMarginWidthN(BookmarksMargin, ScaleArea(sci, 14));
                else sci.SetMarginWidthN(BookmarksMargin, 0);
                /**
                * Set correct folding margin width
                */
                bool useFolding = settings.UseFolding;
                if (!useFolding && !viewBookmarks && !viewLineNumbers) sci.SetMarginWidthN(FoldingMargin, 0);
                else if (useFolding) sci.SetMarginWidthN(FoldingMargin, ScaleArea(sci, 15));
                else sci.SetMarginWidthN(FoldingMargin, ScaleArea(sci, 2));

                sci.SetMarginWidthN(1, 0); //Inheritance Margin (see ASCompletion.PluginMain.Margin)
                /**
                * Adjust caret policy based on settings
                */
                if (settings.KeepCaretCentered)
                {
                    sci.SetXCaretPolicy((int)(CaretPolicy.Jumps | CaretPolicy.Even), 30);
                    sci.SetYCaretPolicy((int)(CaretPolicy.Jumps | CaretPolicy.Even), 2);
                }
                else // Match edge...
                {
                    sci.SetXCaretPolicy((int)CaretPolicy.Even, 0);
                    sci.SetYCaretPolicy((int)CaretPolicy.Even, 0);
                }
                sci.SetVisiblePolicy((int)(CaretPolicy.Strict | CaretPolicy.Even), 0);
                /**
                * Set scroll range (if false, over-scroll mode is enabled)
                */
                sci.EndAtLastLine = settings.EndAtLastLine ? 1 : 0;
                /**
                * Adjust the print margin
                */
                sci.EdgeColumn = settings.PrintMarginColumn;
                sci.EdgeMode = sci.EdgeColumn > 0 ? 1 : 0;
                /**
                * Add missing ignored keys
                */
                foreach (Keys keys in ShortcutManager.AllShortcuts)
                {
                    if (keys != Keys.None && !sci.ContainsIgnoredKeys(keys))
                    {
                        sci.AddIgnoredKeys(keys);
                    }
                }
                if (hardUpdate)
                {
                    string lang = sci.ConfigurationLanguage;
                    sci.ConfigurationLanguage = lang;
                }
                sci.Colourise(0, -1);
                sci.Refresh();
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Scale the control area based on font size and DPI
        /// </summary>
        private static int ScaleArea(ScintillaControl sci, int size)
        {
            int value = ScaleHelper.Scale(size);
            Language lang = SciConfig.GetLanguage(sci.ConfigurationLanguage);
            if (lang != null && !lang.usestyles.IsNullOrEmpty())
            {
                // Only larger fonts need scaling...
                if (lang.usestyles[0].FontSize < 11) return value;
                double multi = lang.usestyles[0].FontSize / 9f;
                double adjusted = Convert.ToDouble(value) * (multi < 1 ? 1 : multi);
                value = Convert.ToInt32(Math.Floor(adjusted));
            }
            return value;
        }

        /// <summary>
        /// Creates a new editor control for the document
        /// </summary>
        public static ScintillaControl CreateControl(string file, string text, int codepage)
        {
            Initialize();
            ScintillaControl sci = new ScintillaControl();
            sci.AutoCSeparator = 32;
            sci.AutoCTypeSeparator = 63;
            sci.IsAutoCGetAutoHide = true;
            sci.IsAutoCGetCancelAtStart = false;
            sci.IsAutoCGetChooseSingle = false;
            sci.IsAutoCGetDropRestOfWord = false;
            sci.IsAutoCGetIgnoreCase = false;
            sci.ControlCharSymbol = 0;
            sci.CurrentPos = 0;
            sci.CursorType = -1;
            sci.Dock = DockStyle.Fill;
            sci.EdgeColumn = 0;
            sci.EdgeMode = 0;
            sci.IsMouseDownCaptures = true;
            sci.IsBufferedDraw = true;
            sci.IsOvertype = false;
            sci.IsReadOnly = false;
            sci.IsUndoCollection = true;
            sci.IsUsePalette = true;
            sci.IsTwoPhaseDraw = true;
            sci.LayoutCache = 1;
            sci.Lexer = 3;
            sci.Location = new Point(0, 0);
            sci.MarginLeft = 5;
            sci.MarginRight = 5;
            sci.ModEventMask = (int)ModificationFlags.InsertText | (int)ModificationFlags.DeleteText | (int)ModificationFlags.RedoPerformed | (int)ModificationFlags.UndoPerformed;
            sci.MouseDwellTime = ScintillaControl.MAXDWELLTIME;
            sci.Name = "sci";
            sci.PasteConvertEndings = false;
            sci.PrintColourMode = (int)PrintOption.Normal;
            sci.PrintWrapMode = (int)Wrap.Word;
            sci.PrintMagnification = 0;
            sci.SearchFlags = 0;
            sci.SelectionEnd = 0;
            sci.SelectionMode = 0;
            sci.SelectionStart = 0;
            sci.SmartIndentType = SmartIndent.CPP;
            sci.Status = 0;
            sci.StyleBits = 7;
            sci.TabIndex = 0;
            sci.TargetEnd = 0;
            sci.TargetStart = 0;
            sci.WrapStartIndent = PluginBase.Settings.IndentSize;
            sci.WrapVisualFlagsLocation = (int)WrapVisualLocation.EndByText;
            sci.WrapVisualFlags = (int)WrapVisualFlag.End;
            sci.XOffset = 0;
            sci.ZoomLevel = 0;
            sci.UsePopUp(false);
            sci.SetMarginTypeN(BookmarksMargin, (int)MarginType.Symbol);
            sci.SetMarginMaskN(BookmarksMargin, MarkerManager.MARKERS);
            sci.SetMarginWidthN(BookmarksMargin, ScaleHelper.Scale(14));
            sci.SetMarginTypeN(LineMargin, (int)MarginType.Number);
            sci.SetMarginMaskN(LineMargin, (int)MarginType.Symbol);
            sci.SetMarginTypeN(FoldingMargin, (int)MarginType.Symbol);
            sci.SetMarginMaskN(FoldingMargin, -33554432 | 1 << 2);
            sci.MarginSensitiveN(FoldingMargin, true);
            sci.SetMultiSelectionTyping(true);
            sci.MarkerDefineRGBAImage(0, Bookmark);
            sci.MarkerDefine(2, MarkerSymbol.Fullrect);
            sci.MarkerDefine((int)MarkerOutline.Folder, MarkerSymbol.BoxPlus);
            sci.MarkerDefine((int)MarkerOutline.FolderOpen, MarkerSymbol.BoxMinus);
            sci.MarkerDefine((int)MarkerOutline.FolderSub, MarkerSymbol.VLine);
            sci.MarkerDefine((int)MarkerOutline.FolderTail, MarkerSymbol.LCorner);
            sci.MarkerDefine((int)MarkerOutline.FolderEnd, MarkerSymbol.BoxPlusConnected);
            sci.MarkerDefine((int)MarkerOutline.FolderOpenMid, MarkerSymbol.BoxMinusConnected);
            sci.MarkerDefine((int)MarkerOutline.FolderMidTail, MarkerSymbol.TCorner);
            sci.CodePage = 65001; // Editor handles text as UTF-8
            sci.Encoding = Encoding.GetEncoding(codepage);
            sci.SaveBOM = IsUnicode(codepage) && PluginBase.Settings.SaveUnicodeWithBOM;
            sci.Text = text; sci.FileName = file; // Set text and save file name
            sci.Modified += Globals.MainForm.OnScintillaControlModified;
            sci.MarginClick += Globals.MainForm.OnScintillaControlMarginClick;
            sci.UpdateUI += Globals.MainForm.OnScintillaControlUpdateControl;
            sci.URIDropped += Globals.MainForm.OnScintillaControlDropFiles;
            sci.ModifyAttemptRO += Globals.MainForm.OnScintillaControlModifyRO;
            string untitledFileStart = TextHelper.GetString("Info.UntitledFileStart");
            if (!file.StartsWithOrdinal(untitledFileStart)) sci.IsReadOnly = FileHelper.FileIsReadOnly(file);
            sci.SetFoldFlags((int)PluginBase.Settings.FoldFlags);
            sci.EmptyUndoBuffer(); ApplySciSettings(sci);
            UITools.Manager.ListenTo(sci);
            return sci;
        }

    }

}
