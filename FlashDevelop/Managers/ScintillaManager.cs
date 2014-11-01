using System;
using System.IO;
using System.Text;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using System.Collections.Generic;
using ScintillaNet.Configuration;
using PluginCore.Localization;
using FlashDevelop.Settings;
using FlashDevelop.Utilities;
using FlashDevelop.Helpers;
using PluginCore.Managers;
using PluginCore.Utilities;
using PluginCore.Controls;
using PluginCore.Helpers;
using ScintillaNet;
using PluginCore;

namespace FlashDevelop.Managers
{
    class ScintillaManager
    {
        public static Scintilla SciConfig;
        public static ConfigurationUtility SciConfigUtil;
        public static System.String XpmBookmark;

        static ScintillaManager()
        {
            Bitmap bookmark = new Bitmap(ResourceHelper.GetStream("BookmarkIcon.bmp"));
            XpmBookmark = ScintillaNet.XPM.ConvertToXPM(bookmark, "#00FF00");
            LoadConfiguration();
        }

        /// <summary>
        /// Loads the syntax and refreshes scintilla settings.
        /// </summary>
        public static void LoadConfiguration()
        {
            SciConfigUtil = new ConfigurationUtility(Assembly.GetExecutingAssembly());
            String[] configFiles = Directory.GetFiles(Path.Combine(PathHelper.SettingDir, "Languages"), "*.xml");
            SciConfig = (Scintilla)SciConfigUtil.LoadConfiguration(configFiles);
            ScintillaControl.Configuration = SciConfig;
            MainForm.Instance.ApplyAllSettings();
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
            String language = SciConfig.GetLanguageFromFile(sci.FileName);
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
                String file = sci.FileName;
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
        /// Selects a correct codepage for the editor
        /// </summary>
        public static Int32 SelectCodePage(Int32 codepage)
        {
            if (codepage == 65001 || codepage == 1201 || codepage == 1200) return 65001;
            else return 0; // Disable multibyte support
        }

        /// <summary>
        /// Gets the line comment string
        /// </summary>
        public static String GetLineComment(String lang)
        {
            Language obj = SciConfig.GetLanguage(lang);
            if (obj.linecomment != null)
            {
                return obj.linecomment;
            }
            return String.Empty;
        }

        /// <summary>
        /// Gets the comment start string
        /// </summary>
        public static String GetCommentStart(String lang)
        {
            Language obj = SciConfig.GetLanguage(lang);
            if (obj.commentstart != null)
            {
                return obj.commentstart;
            }
            return String.Empty;
        }

        /// <summary>
        /// Gets the comment end string
        /// </summary>
        public static String GetCommentEnd(String lang)
        {
            Language obj = SciConfig.GetLanguage(lang);
            if (obj.commentend != null)
            {
                return obj.commentend;
            }
            return String.Empty;
        }

        /// <summary>
        /// Changes the current document's language
        /// </summary>
        public static void ChangeSyntax(String lang, ScintillaControl sci)
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
        public static void ApplySciSettings(ScintillaControl sci, Boolean hardUpdate)
        {
            try
            {
                sci.CaretPeriod = Globals.Settings.CaretPeriod;
                sci.CaretWidth = Globals.Settings.CaretWidth;
                sci.EOLMode = LineEndDetector.DetectNewLineMarker(sci.Text, (Int32)Globals.Settings.EOLMode);
                sci.IsBraceMatching = Globals.Settings.BraceMatchingEnabled;
                sci.UseHighlightGuides = !Globals.Settings.HighlightGuide;
                sci.Indent = Globals.Settings.IndentSize;
                sci.SmartIndentType = Globals.Settings.SmartIndentType;
                sci.IsBackSpaceUnIndents = Globals.Settings.BackSpaceUnIndents;
                sci.IsCaretLineVisible = Globals.Settings.CaretLineVisible;
                sci.IsIndentationGuides = Globals.Settings.ViewIndentationGuides;
                sci.IndentView = Globals.Settings.IndentView;
                sci.IsTabIndents = Globals.Settings.TabIndents;
                sci.IsUseTabs = Globals.Settings.UseTabs;
                sci.IsViewEOL = Globals.Settings.ViewEOL;
                sci.ScrollWidth = Globals.Settings.ScrollWidth;
                sci.TabWidth = Globals.Settings.TabWidth;
                sci.ViewWS = Convert.ToInt32(Globals.Settings.ViewWhitespace);
                sci.WrapMode = Convert.ToInt32(Globals.Settings.WrapText);
                sci.SetProperty("fold", Convert.ToInt32(Globals.Settings.UseFolding).ToString());
                sci.SetProperty("fold.comment", Convert.ToInt32(Globals.Settings.FoldComment).ToString());
                sci.SetProperty("fold.compact", Convert.ToInt32(Globals.Settings.FoldCompact).ToString());
                sci.SetProperty("fold.preprocessor", Convert.ToInt32(Globals.Settings.FoldPreprocessor).ToString());
                sci.SetProperty("fold.at.else", Convert.ToInt32(Globals.Settings.FoldAtElse).ToString());
                sci.SetProperty("fold.html", Convert.ToInt32(Globals.Settings.FoldHtml).ToString());
                sci.SetProperty("lexer.cpp.track.preprocessor", "0");
                sci.SetVirtualSpaceOptions((Int32)Globals.Settings.VirtualSpaceMode);
                sci.SetFoldFlags((Int32)Globals.Settings.FoldFlags);
                /**
                * Set if themes should colorize the first margin
                */
                Language language = SciConfig.GetLanguage(sci.ConfigurationLanguage);
                if (language != null && language.editorstyle != null)
                {
                    Boolean colorizeMarkerBack = language.editorstyle.ColorizeMarkerBack;
                    if (colorizeMarkerBack) sci.SetMarginTypeN(0, (Int32)ScintillaNet.Enums.MarginType.Fore);
                    else sci.SetMarginTypeN(0, (Int32)ScintillaNet.Enums.MarginType.Symbol);
                }
                /** 
                * Set correct line number margin width
                */
                Boolean viewLineNumbers = Globals.Settings.ViewLineNumbers;
                if (viewLineNumbers) sci.SetMarginWidthN(1, ScaleHelper.Scale(36));
                else sci.SetMarginWidthN(1, 0);
                /**
                * Set correct bookmark margin width
                */
                Boolean viewBookmarks = Globals.Settings.ViewBookmarks;
                if (viewBookmarks) sci.SetMarginWidthN(0, ScaleHelper.Scale(14));
                else sci.SetMarginWidthN(0, 0);
                /**
                * Set correct folding margin width
                */
                Boolean useFolding = Globals.Settings.UseFolding;
                if (!useFolding && !viewBookmarks && !viewLineNumbers) sci.SetMarginWidthN(2, 0);
                else if (useFolding) sci.SetMarginWidthN(2, ScaleHelper.Scale(15));
                else sci.SetMarginWidthN(2, ScaleHelper.Scale(2));
                /**
                * Adjust the print margin
                */
                sci.EdgeColumn = Globals.Settings.PrintMarginColumn;
                if (sci.EdgeColumn > 0) sci.EdgeMode = 1;
                else sci.EdgeMode = 0;
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
                    String lang = sci.ConfigurationLanguage;
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
        /// Creates a new editor control for the document 
        /// </summary>
        public static ScintillaControl CreateControl(String file, String text, Int32 codepage)
        {
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
            sci.Dock = System.Windows.Forms.DockStyle.Fill;
            sci.EndAtLastLine = 1;
            sci.EdgeColumn = 0;
            sci.EdgeMode = 0;
            sci.IsHScrollBar = true;
            sci.IsMouseDownCaptures = true;
            sci.IsBufferedDraw = true;
            sci.IsOvertype = false;
            sci.IsReadOnly = false;
            sci.IsUndoCollection = true;
            sci.IsVScrollBar = true;
            sci.IsUsePalette = true;
            sci.IsTwoPhaseDraw = true;
            sci.LayoutCache = 1;
            sci.Lexer = 3;
            sci.Location = new System.Drawing.Point(0, 0);
            sci.MarginLeft = 5;
            sci.MarginRight = 5;
            sci.ModEventMask = (Int32)ScintillaNet.Enums.ModificationFlags.InsertText | (Int32)ScintillaNet.Enums.ModificationFlags.DeleteText | (Int32)ScintillaNet.Enums.ModificationFlags.RedoPerformed | (Int32)ScintillaNet.Enums.ModificationFlags.UndoPerformed;
            sci.MouseDwellTime = ScintillaControl.MAXDWELLTIME;
            sci.Name = "sci";
            sci.PasteConvertEndings = false;
            sci.PrintColourMode = (Int32)ScintillaNet.Enums.PrintOption.Normal;
            sci.PrintWrapMode = (Int32)ScintillaNet.Enums.Wrap.Word;
            sci.PrintMagnification = 0;
            sci.SearchFlags = 0;
            sci.SelectionEnd = 0;
            sci.SelectionMode = 0;
            sci.SelectionStart = 0;
            sci.SmartIndentType = ScintillaNet.Enums.SmartIndent.CPP;
            sci.Status = 0;
            sci.StyleBits = 7;
            sci.TabIndex = 0;
            sci.TargetEnd = 0;
            sci.TargetStart = 0;
            sci.WrapStartIndent = Globals.Settings.IndentSize;
            sci.WrapVisualFlagsLocation = (Int32)ScintillaNet.Enums.WrapVisualLocation.EndByText;
            sci.WrapVisualFlags = (Int32)ScintillaNet.Enums.WrapVisualFlag.End;
            sci.XOffset = 0;
            sci.ZoomLevel = 0;
            sci.UsePopUp(false);
            sci.SetMarginTypeN(0, (Int32)ScintillaNet.Enums.MarginType.Symbol);
            sci.SetMarginMaskN(0, MarkerManager.MARKERS);
            sci.SetMarginWidthN(0, ScaleHelper.Scale(14));
            sci.SetMarginTypeN(1, (Int32)ScintillaNet.Enums.MarginType.Number);
            sci.SetMarginMaskN(1, (Int32)ScintillaNet.Enums.MarginType.Symbol);
            sci.SetMarginTypeN(2, (Int32)ScintillaNet.Enums.MarginType.Symbol);
            sci.SetMarginMaskN(2, -33554432 | 1 << 2);
            sci.MarginSensitiveN(2, true);
            sci.SetMultiSelectionTyping(true);
            sci.MarkerDefinePixmap(0, XpmBookmark);
            sci.MarkerDefine(2, ScintillaNet.Enums.MarkerSymbol.Fullrect);
            sci.MarkerDefine((Int32)ScintillaNet.Enums.MarkerOutline.Folder, ScintillaNet.Enums.MarkerSymbol.BoxPlus);
            sci.MarkerDefine((Int32)ScintillaNet.Enums.MarkerOutline.FolderOpen, ScintillaNet.Enums.MarkerSymbol.BoxMinus);
            sci.MarkerDefine((Int32)ScintillaNet.Enums.MarkerOutline.FolderSub, ScintillaNet.Enums.MarkerSymbol.VLine);
            sci.MarkerDefine((Int32)ScintillaNet.Enums.MarkerOutline.FolderTail, ScintillaNet.Enums.MarkerSymbol.LCorner);
            sci.MarkerDefine((Int32)ScintillaNet.Enums.MarkerOutline.FolderEnd, ScintillaNet.Enums.MarkerSymbol.BoxPlusConnected);
            sci.MarkerDefine((Int32)ScintillaNet.Enums.MarkerOutline.FolderOpenMid, ScintillaNet.Enums.MarkerSymbol.BoxMinusConnected);
            sci.MarkerDefine((Int32)ScintillaNet.Enums.MarkerOutline.FolderMidTail, ScintillaNet.Enums.MarkerSymbol.TCorner);
            sci.SetXCaretPolicy((Int32)(ScintillaNet.Enums.CaretPolicy.Jumps | ScintillaNet.Enums.CaretPolicy.Even), 30);
            sci.SetYCaretPolicy((Int32)(ScintillaNet.Enums.CaretPolicy.Jumps | ScintillaNet.Enums.CaretPolicy.Even), 2);
            sci.ScrollWidthTracking = (Globals.Settings.ScrollWidth == 3000);
            sci.CodePage = SelectCodePage(codepage);
            sci.Encoding = Encoding.GetEncoding(codepage);
            sci.SaveBOM = (sci.CodePage == 65001) && Globals.Settings.SaveUnicodeWithBOM;
            sci.Text = text; sci.FileName = file; // Set text and save file name
            sci.Modified += new ModifiedHandler(Globals.MainForm.OnScintillaControlModified);
            sci.MarginClick += new MarginClickHandler(Globals.MainForm.OnScintillaControlMarginClick);
            sci.UpdateUI += new UpdateUIHandler(Globals.MainForm.OnScintillaControlUpdateControl);
            sci.URIDropped += new URIDroppedHandler(Globals.MainForm.OnScintillaControlDropFiles);
            sci.ModifyAttemptRO += new ModifyAttemptROHandler(Globals.MainForm.OnScintillaControlModifyRO);
            String untitledFileStart = TextHelper.GetString("Info.UntitledFileStart");
            if (!file.StartsWith(untitledFileStart)) sci.IsReadOnly = FileHelper.FileIsReadOnly(file);
            sci.SetFoldFlags((Int32)Globals.Settings.FoldFlags);
            sci.EmptyUndoBuffer(); ApplySciSettings(sci);
            UITools.Manager.ListenTo(sci);
            return sci;
        }

    }

}
