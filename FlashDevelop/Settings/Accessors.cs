using System;
using System.Text;
using System.Drawing;
using System.Reflection;
using System.Collections;
using System.ComponentModel;
using System.Drawing.Design;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Windows.Forms.Design;
using PluginCore.Localization;
using System.Windows.Forms;
using PluginCore.Managers;
using ScintillaNet;
using PluginCore;

namespace FlashDevelop.Settings
{
    public partial class SettingObject : ISettings
    {
        #region Folding

        [DefaultValue(false)]
        [DisplayName("Fold At Else")]
        [LocalizedCategory("FlashDevelop.Category.Folding")]
        [LocalizedDescription("FlashDevelop.Description.FoldAtElse")]
        public Boolean FoldAtElse
        {
            get { return this.foldAtElse; }
            set { this.foldAtElse = value; }
        }

        [DefaultValue(true)]
        [DisplayName("Fold At Comment")]
        [LocalizedCategory("FlashDevelop.Category.Folding")]
        [LocalizedDescription("FlashDevelop.Description.FoldComment")]
        public Boolean FoldComment
        {
            get { return this.foldComment; }
            set { this.foldComment = value; }
        }

        [DefaultValue(false)]
        [DisplayName("Use Compact Folding")]
        [LocalizedCategory("FlashDevelop.Category.Folding")]
        [LocalizedDescription("FlashDevelop.Description.FoldCompact")]
        public Boolean FoldCompact
        {
            get { return this.foldCompact; }
            set { this.foldCompact = value; }
        }

        [DefaultValue(true)]
        [DisplayName("Fold At Html")]
        [LocalizedCategory("FlashDevelop.Category.Folding")]
        [LocalizedDescription("FlashDevelop.Description.FoldHtml")]
        public Boolean FoldHtml
        {
            get { return this.foldHtml; }
            set { this.foldHtml = value; }
        }

        [DefaultValue(true)]
        [DisplayName("Fold At Preprocessor")]
        [LocalizedCategory("FlashDevelop.Category.Folding")]
        [LocalizedDescription("FlashDevelop.Description.FoldPreprocessor")]
        public Boolean FoldPreprocessor
        {
            get { return this.foldPreprocessor; }
            set { this.foldPreprocessor = value; }
        }

        [DefaultValue(true)]
        [DisplayName("Enable Folding")]
        [LocalizedCategory("FlashDevelop.Category.Folding")]
        [LocalizedDescription("FlashDevelop.Description.UseFolding")]
        public Boolean UseFolding
        {
            get { return this.useFolding; }
            set { this.useFolding = value; }
        }

        [DisplayName("Fold Flags")]
        [LocalizedCategory("FlashDevelop.Category.Folding")]
        [DefaultValue(ScintillaNet.Enums.FoldFlag.LineAfterContracted)]
        [LocalizedDescription("FlashDevelop.Description.FoldFlags")]
        public ScintillaNet.Enums.FoldFlag FoldFlags
        {
            get { return this.foldFlags; }
            set { this.foldFlags = value; }
        }

        #endregion

        #region Display

        [DefaultValue(false)]
        [DisplayName("View EOL Characters")]
        [LocalizedCategory("FlashDevelop.Category.Display")]
        [LocalizedDescription("FlashDevelop.Description.ViewEOL")]
        public Boolean ViewEOL
        {
            get { return this.viewEOL; }
            set { this.viewEOL = value; }
        }

        [DefaultValue(true)]
        [DisplayName("View Shortcuts")]
        [LocalizedCategory("FlashDevelop.Category.Display")]
        [LocalizedDescription("FlashDevelop.Description.ViewShortcuts")]
        public Boolean ViewShortcuts
        {
            get { return this.viewShortcuts; }
            set { this.viewShortcuts = value; }
        }

        [DefaultValue(true)]
        [DisplayName("View Bookmarks")]
        [LocalizedCategory("FlashDevelop.Category.Display")]
        [LocalizedDescription("FlashDevelop.Description.ViewBookmarks")]
        public Boolean ViewBookmarks
        {
            get { return this.viewBookmarks; }
            set { this.viewBookmarks = value; }
        }

        [DefaultValue(true)]
        [DisplayName("View Line Numbers")]
        [LocalizedCategory("FlashDevelop.Category.Display")]
        [LocalizedDescription("FlashDevelop.Description.ViewLineNumbers")]
        public Boolean ViewLineNumbers
        {
            get { return this.viewLineNumbers; }
            set { this.viewLineNumbers = value; }
        }

        [DefaultValue(true)]
        [DisplayName("View Indentation Guides")]
        [LocalizedCategory("FlashDevelop.Category.Display")]
        [LocalizedDescription("FlashDevelop.Description.ViewIndentationGuides")]
        public Boolean ViewIndentationGuides
        {
            get { return this.viewIndentationGuides; }
            set { this.viewIndentationGuides = value; }
        }

        [DefaultValue(false)]
        [DisplayName("View Whitespace Characters")]
        [LocalizedCategory("FlashDevelop.Category.Display")]
        [LocalizedDescription("FlashDevelop.Description.ViewWhitespace")]
        public Boolean ViewWhitespace
        {
            get { return this.viewWhitespace; }
            set { this.viewWhitespace = value; }
        }

        [DefaultValue(true)]
        [DisplayName("View ToolBar")]
        [LocalizedCategory("FlashDevelop.Category.Display")]
        [LocalizedDescription("FlashDevelop.Description.ViewToolBar")]
        public Boolean ViewToolBar
        {
            get { return this.viewToolBar; }
            set { this.viewToolBar = value; }
        }

        [DefaultValue(true)]
        [DisplayName("View StatusBar")]
        [LocalizedCategory("FlashDevelop.Category.Display")]
        [LocalizedDescription("FlashDevelop.Description.ViewStatusBar")]
        public Boolean ViewStatusBar
        {
            get { return this.viewStatusBar; }
            set { this.viewStatusBar = value; }
        }

        [DefaultValue(false)]
        [DisplayName("View Modified Lines")]
        [LocalizedCategory("FlashDevelop.Category.Display")]
        [LocalizedDescription("FlashDevelop.Description.ViewModifiedLines")]
        public Boolean ViewModifiedLines
        {
            get { return this.viewModifiedLines; }
            set { this.viewModifiedLines = value; }
        }

        [DisplayName("ComboBox Flat Style")]
        [LocalizedCategory("FlashDevelop.Category.Display")]
        [LocalizedDescription("FlashDevelop.Description.ComboBoxFlatStyle")]
        [DefaultValue(FlatStyle.Popup)]
        public FlatStyle ComboBoxFlatStyle
        {
            get { return this.comboBoxFlatStyle; }
            set { this.comboBoxFlatStyle = value; }
        }

        [DefaultValue(true)]
        [DisplayName("Use List View Grouping")]
        [LocalizedCategory("FlashDevelop.Category.Display")]
        [LocalizedDescription("FlashDevelop.Description.UseListViewGrouping")]
        public Boolean UseListViewGrouping
        {
            get { return this.useListViewGrouping; }
            set { this.useListViewGrouping = value; }
        }

        [DefaultValue(false)]
        [DisplayName("Use System UI Colors")]
        [LocalizedCategory("FlashDevelop.Category.Display")]
        [LocalizedDescription("FlashDevelop.Description.UseSystemColors")]
        public Boolean UseSystemColors
        {
            get { return this.useSystemColors; }
            set { this.useSystemColors = value; }
        }

        [DisplayName("UI Render Mode")]
        [LocalizedCategory("FlashDevelop.Category.Display")]
        [LocalizedDescription("FlashDevelop.Description.RenderMode")]
        [DefaultValue(UiRenderMode.Professional)]
        public UiRenderMode RenderMode
        {
            get { return this.uiRenderMode; }
            set { this.uiRenderMode = value; }
        }

        [XmlIgnore]
        [DisplayName("UI Console Font")]
        [LocalizedCategory("FlashDevelop.Category.Display")]
        [LocalizedDescription("FlashDevelop.Description.ConsoleFont")]
        [DefaultValue(typeof(Font), "Courier New, 8.75pt")]
        public Font ConsoleFont
        {
            get { return this.consoleFont; }
            set { this.consoleFont = value; }
        }

        [XmlIgnore]
        [DisplayName("UI Default Font")]
        [LocalizedCategory("FlashDevelop.Category.Display")]
        [LocalizedDescription("FlashDevelop.Description.DefaultFont")]
        [DefaultValue(typeof(Font), "Tahoma, 8.25pt")]
        public Font DefaultFont
        {
            get { return this.defaultFont; }
            set { this.defaultFont = value; }
        }

        #endregion

        #region Editor

        [DefaultValue(false)]
        [DisplayName("Highlight Caret Line")]
        [LocalizedCategory("FlashDevelop.Category.Editor")]
        [LocalizedDescription("FlashDevelop.Description.CaretLineVisible")]
        public Boolean CaretLineVisible
        {
            get { return this.caretLineVisible; }
            set { this.caretLineVisible = value; }
        }

        [DefaultValue(true)]
        [DisplayName("Disable Highlight Guide")]
        [LocalizedCategory("FlashDevelop.Category.Editor")]
        [LocalizedDescription("FlashDevelop.Description.HighlightGuide")]
        public Boolean HighlightGuide
        {
            get { return this.highlightGuide; }
            set { this.highlightGuide = value; }
        }

        [DefaultValue(0)]
        [DisplayName("Print Margin Column")]
        [LocalizedCategory("FlashDevelop.Category.Editor")]
        [LocalizedDescription("FlashDevelop.Description.PrintMarginColumn")]
        public Int32 PrintMarginColumn
        {
            get { return this.printMarginColumn; }
            set { this.printMarginColumn = value; }
        }

        [DisplayName("Virtual Space Mode")]
        [LocalizedCategory("FlashDevelop.Category.Editor")]
        [LocalizedDescription("FlashDevelop.Description.VirtualSpaceMode")]
        [DefaultValue(ScintillaNet.Enums.VirtualSpaceMode.RectangularSelection)]
        public ScintillaNet.Enums.VirtualSpaceMode VirtualSpaceMode
        {
            get { return this.virtualSpaceMode; }
            set { this.virtualSpaceMode = value; }
        }

        [DisplayName("End Of Line Mode")]
        [DefaultValue(ScintillaNet.Enums.EndOfLine.CRLF)]
        [LocalizedCategory("FlashDevelop.Category.Editor")]
        [LocalizedDescription("FlashDevelop.Description.EOLMode")]
        public ScintillaNet.Enums.EndOfLine EOLMode
        {
            get { return this.eolMode; }
            set { this.eolMode = value; }
        }

        [DefaultValue(500)]
        [DisplayName("Caret Period")]
        [LocalizedCategory("FlashDevelop.Category.Editor")]
        [LocalizedDescription("FlashDevelop.Description.CaretPeriod")]
        public Int32 CaretPeriod
        {
            get { return this.caretPeriod; }
            set { this.caretPeriod = value; }
        }

        [DefaultValue(2)]
        [DisplayName("Caret Width")]
        [LocalizedCategory("FlashDevelop.Category.Editor")]
        [LocalizedDescription("FlashDevelop.Description.CaretWidth")]
        public Int32 CaretWidth
        {
            get { return this.caretWidth; }
            set { this.caretWidth = value; }
        }

        [DefaultValue(3000)]
        [DisplayName("Scroll Area Width")]
        [LocalizedCategory("FlashDevelop.Category.Editor")]
        [LocalizedDescription("FlashDevelop.Description.ScrollWidth")]
        public Int32 ScrollWidth
        {
            get { return this.scrollWidth; }
            set { this.scrollWidth = value; }
        }

        [DefaultValue("as")]
        [DisplayName("Default File Extension")]
        [LocalizedCategory("FlashDevelop.Category.Editor")]
        [LocalizedDescription("FlashDevelop.Description.DefaultFileExtension")]
        public String DefaultFileExtension
        {
            get { return this.defaultFileExtension; }
            set { this.defaultFileExtension = value; }
        }

        [DefaultValue(15000)]
        [DisplayName("Backup Interval")]
        [LocalizedCategory("FlashDevelop.Category.Editor")]
        [LocalizedDescription("FlashDevelop.Description.BackupInterval")]
        public Int32 BackupInterval
        {
            get { return this.backupInterval; }
            set { this.backupInterval = value; }
        }

        [DefaultValue(3000)]
        [DisplayName("File Poll Interval")]
        [LocalizedCategory("FlashDevelop.Category.Editor")]
        [LocalizedDescription("FlashDevelop.Description.FilePollInterval")]
        public Int32 FilePollInterval
        {
            get { return this.filePollInterval; }
            set { this.filePollInterval = value; }
        }

        [DefaultValue(ScintillaNet.Enums.HighlightMatchingWordsMode.SelectionOrPosition)]
        [DisplayName("Highlight Matching Words Mode")]
        [LocalizedCategory("FlashDevelop.Category.Editor")]
        [LocalizedDescription("FlashDevelop.Description.HighlightMatchingWordsMode")]
        public ScintillaNet.Enums.HighlightMatchingWordsMode HighlightMatchingWordsMode
        {
            get { return this.highlightMatchingWordsMode; }
            set { this.highlightMatchingWordsMode = value; }
        }

        #endregion

        #region Locale

        [DisplayName("Selected Locale")]
        [DefaultValue(LocaleVersion.en_US)]
        [LocalizedCategory("FlashDevelop.Category.Locale")]
        [LocalizedDescription("FlashDevelop.Description.LocaleVersion")]
        public LocaleVersion LocaleVersion
        {
            get { return this.localeVersion; }
            set { this.localeVersion = value; }
        }

        [DefaultValue(CodePage.UTF8)]
        [DisplayName("Default CodePage")]
        [LocalizedCategory("FlashDevelop.Category.Locale")]
        [LocalizedDescription("FlashDevelop.Description.DefaultCodePage")]
        public CodePage DefaultCodePage
        {
            get { return this.defaultCodePage; }
            set { this.defaultCodePage = value; }
        }

        [DefaultValue(false)]
        [DisplayName("Create Unicode With BOM")]
        [LocalizedCategory("FlashDevelop.Category.Locale")]
        [LocalizedDescription("FlashDevelop.Description.SaveUnicodeWithBOM")]
        public Boolean SaveUnicodeWithBOM
        {
            get { return this.saveUnicodeWithBOM; }
            set { this.saveUnicodeWithBOM = value; }
        }

        #endregion

        #region Indenting

        [DefaultValue(false)]
        [DisplayName("Use Backspace Unindents")]
        [LocalizedCategory("FlashDevelop.Category.Indenting")]
        [LocalizedDescription("FlashDevelop.Description.BackSpaceUnIndents")]
        public Boolean BackSpaceUnIndents
        {
            get { return this.backSpaceUnIndents; }
            set { this.backSpaceUnIndents = value; }
        }

        [DefaultValue(true)]
        [DisplayName("Use Tab Indents")]
        [LocalizedCategory("FlashDevelop.Category.Indenting")]
        [LocalizedDescription("FlashDevelop.Description.TabIndents")]
        public Boolean TabIndents
        {
            get { return this.tabIndents; }
            set { this.tabIndents = value; }
        }

        [DefaultValue(ScintillaNet.Enums.IndentView.Real)]
        [DisplayName("Indent Guide Type")]
        [LocalizedCategory("FlashDevelop.Category.Indenting")]
        [LocalizedDescription("FlashDevelop.Description.IndentView")]
        public ScintillaNet.Enums.IndentView IndentView
        {
            get 
            {
                if ((Int32)this.indentView == 0) this.indentView = ScintillaNet.Enums.IndentView.Real;
                return this.indentView; 
            }
            set
            {
                this.indentView = value;
            }
        }

        [DisplayName("Smart Indent Type")]
        [DefaultValue(ScintillaNet.Enums.SmartIndent.CPP)]
        [LocalizedCategory("FlashDevelop.Category.Indenting")]
        [LocalizedDescription("FlashDevelop.Description.SmartIndentType")]
        public ScintillaNet.Enums.SmartIndent SmartIndentType
        {
            get { return this.smartIndentType; }
            set { this.smartIndentType = value; }
        }

        [DisplayName("Coding Style Type")]
        [DefaultValue(CodingStyle.BracesAfterLine)]
        [LocalizedCategory("FlashDevelop.Category.Indenting")]
        [LocalizedDescription("FlashDevelop.Description.CodingStyle")]
        public CodingStyle CodingStyle
        {
            get { return this.codingStyle; }
            set { this.codingStyle = value; }
        }

        [DisplayName("Comment Block Indenting")]
        [DefaultValue(CommentBlockStyle.Indented)]
        [LocalizedCategory("FlashDevelop.Category.Indenting")]
        [LocalizedDescription("FlashDevelop.Description.CommentBlockStyle")]
        public CommentBlockStyle CommentBlockStyle
        {
            get { return this.commentBlockStyle; }
            set { this.commentBlockStyle = value; }
        }

        [DefaultValue(4)]
        [DisplayName("Indenting Size")]
        [LocalizedCategory("FlashDevelop.Category.Indenting")]
        [LocalizedDescription("FlashDevelop.Description.IndentSize")]
        public Int32 IndentSize
        {
            get { return this.indentSize; }
            set { this.indentSize = value; }
        }

        [DefaultValue(4)]
        [DisplayName("Width Of Tab")]
        [LocalizedCategory("FlashDevelop.Category.Indenting")]
        [LocalizedDescription("FlashDevelop.Description.TabWidth")]
        public Int32 TabWidth
        {
            get { return this.tabWidth; }
            set { this.tabWidth = value; }
        }

        [DefaultValue(true)]
        [DisplayName("Use Tab Characters")]
        [LocalizedCategory("FlashDevelop.Category.Indenting")]
        [LocalizedDescription("FlashDevelop.Description.UseTabs")]
        public Boolean UseTabs
        {
            get { return this.useTabs; }
            set { this.useTabs = value; }
        }

        #endregion

        #region Features

        [DefaultValue(true)]
        [DisplayName("Apply File Extension")]
        [LocalizedCategory("FlashDevelop.Category.Features")]
        [LocalizedDescription("FlashDevelop.Description.ApplyFileExtension")]
        public Boolean ApplyFileExtension
        {
            get { return this.applyFileExtension; }
            set { this.applyFileExtension = value; }
        }

        [DefaultValue(false)]
        [DisplayName("Automaticly Reload Modified Files")]
        [LocalizedCategory("FlashDevelop.Category.Features")]
        [LocalizedDescription("FlashDevelop.Description.AutoReloadModifiedFiles")]
        public Boolean AutoReloadModifiedFiles
        {
            get { return this.autoReloadModifiedFiles; }
            set { this.autoReloadModifiedFiles = value; }
        }

        [DefaultValue(false)]
        [DisplayName("Use Sequential Tabbing")]
        [LocalizedCategory("FlashDevelop.Category.Features")]
        [LocalizedDescription("FlashDevelop.Description.SequentialTabbing")]
        public Boolean SequentialTabbing
        {
            get { return this.sequentialTabbing; }
            set { this.sequentialTabbing = value; }
        }

        [DefaultValue(false)]
        [DisplayName("Wrap Editor Text")]
        [LocalizedCategory("FlashDevelop.Category.Features")]
        [LocalizedDescription("FlashDevelop.Description.WrapText")]
        public Boolean WrapText
        {
            get { return this.wrapText; }
            set { this.wrapText = value; }
        }

        [DefaultValue(true)]
        [DisplayName("Use Brace Matching")]
        [LocalizedCategory("FlashDevelop.Category.Features")]
        [LocalizedDescription("FlashDevelop.Description.BraceMatchingEnabled")]
        public Boolean BraceMatchingEnabled
        {
            get { return this.braceMatchingEnabled; }
            set { this.braceMatchingEnabled = value; }
        }

        [DefaultValue(true)]
        [DisplayName("Line Comments After Indent")]
        [LocalizedCategory("FlashDevelop.Category.Features")]
        [LocalizedDescription("FlashDevelop.Description.LineCommentsAfterIndent")]
        public Boolean LineCommentsAfterIndent
        {
            get { return this.lineCommentsAfterIndent; }
            set { this.lineCommentsAfterIndent = value; }
        }

        [DefaultValue(false)]
        [DisplayName("Move Cursor After Comment")]
        [LocalizedCategory("FlashDevelop.Category.Features")]
        [LocalizedDescription("FlashDevelop.Description.MoveCursorAfterComment")]
        public Boolean MoveCursorAfterComment
        {
            get { return this.moveCursorAfterComment; }
            set { this.moveCursorAfterComment = value; }
        }

        [DefaultValue(true)]
        [DisplayName("Restore File States")]
        [LocalizedCategory("FlashDevelop.Category.Features")]
        [LocalizedDescription("FlashDevelop.Description.RestoreFileStates")]
        public Boolean RestoreFileStates
        {
            get { return this.restoreFileStates; }
            set { this.restoreFileStates = value; }
        }

        [DefaultValue(true)]
        [DisplayName("Restore File Session")]
        [LocalizedCategory("FlashDevelop.Category.Features")]
        [LocalizedDescription("FlashDevelop.Description.RestoreFileSession")]
        public Boolean RestoreFileSession
        {
            get { return this.restoreFileSession; }
            set { this.restoreFileSession = value; }
        }

        [DefaultValue(false)]
        [DisplayName("Confirm On Exit")]
        [LocalizedCategory("FlashDevelop.Category.Features")]
        [LocalizedDescription("FlashDevelop.Description.ConfirmOnExit")]
        public Boolean ConfirmOnExit
        {
            get { return this.confirmOnExit; }
            set { this.confirmOnExit = value; }
        }

        [DefaultValue(false)]
        [DisplayName("Disable Replace In Files Confirm")]
        [LocalizedCategory("FlashDevelop.Category.Features")]
        [LocalizedDescription("FlashDevelop.Description.DisableReplaceFilesConfirm")]
        public Boolean DisableReplaceFilesConfirm
        {
            get { return this.disableReplaceFilesConfirm; }
            set { this.disableReplaceFilesConfirm = value; }
        }

        [DefaultValue(true)]
        [DisplayName("Redirect Find In Files Results")]
        [LocalizedCategory("FlashDevelop.Category.Features")]
        [LocalizedDescription("FlashDevelop.Description.RedirectFilesResults")]
        public Boolean RedirectFilesResults
        {
            get { return this.redirectFilesResults; }
            set { this.redirectFilesResults = value; }
        }

        [DefaultValue(false)]
        [DisplayName("Disable Find Option Sync")]
        [LocalizedCategory("FlashDevelop.Category.Features")]
        [LocalizedDescription("FlashDevelop.Description.DisableFindOptionSync")]
        public Boolean DisableFindOptionSync
        {
            get { return this.disableFindOptionSync; }
            set { this.disableFindOptionSync = value; }
        }

        [DefaultValue(false)]
        [DisplayName("Disable Find Text Updating")]
        [LocalizedCategory("FlashDevelop.Category.Features")]
        [LocalizedDescription("FlashDevelop.Description.DisableFindTextUpdating")]
        public Boolean DisableFindTextUpdating
        {
            get { return this.disableFindTextUpdating; }
            set { this.disableFindTextUpdating = value; }
        }

        [DefaultValue(false)]
        [DisplayName("Disable Simple Quick Find")]
        [LocalizedCategory("FlashDevelop.Category.Features")]
        [LocalizedDescription("FlashDevelop.Description.DisableSimpleQuickFind")]
        public Boolean DisableSimpleQuickFind
        {
            get { return this.disableSimpleQuickFind; }
            set { this.disableSimpleQuickFind = value; }
        }

        [DefaultValue(false)]
        [DisplayName("Disable Tab Differentiation")]
        [LocalizedCategory("FlashDevelop.Category.Features")]
        [LocalizedDescription("FlashDevelop.Description.DisableTabDifferentiation")]
        public Boolean DisableTabDifferentiation
        {
            get { return this.disableTabDifferentiation; }
            set { this.disableTabDifferentiation = value; }
        }

        #endregion

        #region Formatting

        [DefaultValue(false)]
        [DisplayName("Keep Indent Tabs")]
        [LocalizedCategory("FlashDevelop.Category.Formatting")]
        [LocalizedDescription("FlashDevelop.Description.KeepIndentTabs")]
        public Boolean KeepIndentTabs
        {
            get { return this.keepIndentTabs; }
            set { this.keepIndentTabs = value; }
        }

        [DefaultValue(false)]
        [DisplayName("Strip Trailing Spaces")]
        [LocalizedCategory("FlashDevelop.Category.Formatting")]
        [LocalizedDescription("FlashDevelop.Description.StripTrailingSpaces")]
        public Boolean StripTrailingSpaces
        {
            get { return this.stripTrailingSpaces; }
            set { this.stripTrailingSpaces = value; }
        }

        [DefaultValue(true)]
        [DisplayName("Ensure Consistent Line Ends")]
        [LocalizedCategory("FlashDevelop.Category.Formatting")]
        [LocalizedDescription("FlashDevelop.Description.EnsureConsistentLineEnds")]
        public Boolean EnsureConsistentLineEnds
        {
            get { return this.ensureConsistentLineEnds; }
            set { this.ensureConsistentLineEnds = value; }
        }

        [DefaultValue(false)]
        [DisplayName("Ensure Last Line End")]
        [LocalizedCategory("FlashDevelop.Category.Formatting")]
        [LocalizedDescription("FlashDevelop.Description.EnsureLastLineEnd")]
        public Boolean EnsureLastLineEnd
        {
            get { return this.ensureLastLineEnd; }
            set { this.ensureLastLineEnd = value; }
        }

        #endregion

        #region State

        [DisplayName("Latest Startup Command")]
        [LocalizedCategory("FlashDevelop.Category.State")]
        [LocalizedDescription("FlashDevelop.Description.LatestCommand")]
        public Int32 LatestCommand
        {
            get { return this.latestCommand; }
            set { this.latestCommand = value; }
        }

        [DisplayName("Last Active Path")]
        [LocalizedCategory("FlashDevelop.Category.State")]
        [LocalizedDescription("FlashDevelop.Description.LatestDialogPath")]
        public String LatestDialogPath
        {
            get { return this.latestDialogPath; }
            set { this.latestDialogPath = value; }
        }

        [DisplayName("Window Size")]
        [LocalizedCategory("FlashDevelop.Category.State")]
        [LocalizedDescription("FlashDevelop.Description.WindowSize")]
        public Size WindowSize
        {
            get { return this.windowSize; }
            set { this.windowSize = value; }
        }

        [DisplayName("Window State")]
        [LocalizedCategory("FlashDevelop.Category.State")]
        [LocalizedDescription("FlashDevelop.Description.WindowState")]
        public FormWindowState WindowState
        {
            get { return this.windowState; }
            set { this.windowState = value; }
        }

        [DisplayName("Window Position")]
        [LocalizedCategory("FlashDevelop.Category.State")]
        [LocalizedDescription("FlashDevelop.Description.WindowPosition")]
        public Point WindowPosition
        {
            get { return this.windowPosition; }
            set { this.windowPosition = value; }
        }

        [DisplayName("Previous Documents")]
        [LocalizedCategory("FlashDevelop.Category.State")]
        [LocalizedDescription("FlashDevelop.Description.PreviousDocuments")]
        [Editor("System.Windows.Forms.Design.StringCollectionEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor,System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
        public List<String> PreviousDocuments
        {
            get { return this.previousDocuments; }
            set { this.previousDocuments = value; }
        }

        [DisplayName("Disabled Plugins")]
        [LocalizedCategory("FlashDevelop.Category.State")]
        [LocalizedDescription("FlashDevelop.Description.DisabledPlugins")]
        [Editor("System.Windows.Forms.Design.StringCollectionEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor,System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
        public List<String> DisabledPlugins
        {
            get { return this.disabledPlugins; }
            set { this.disabledPlugins = value; }
        }

        #endregion

        #region Controls

        [DefaultValue(500)]
        [DisplayName("Hover Delay")]
        [LocalizedCategory("FlashDevelop.Category.Controls")]
        [LocalizedDescription("FlashDevelop.Description.HoverDelay")]
        public Int32 HoverDelay
        {
            get { return this.uiHoverDelay; }
            set
            {
                this.uiHoverDelay = value;
                foreach (ITabbedDocument doc in PluginBase.MainForm.Documents)
                {
                    if (doc.IsEditable) doc.SciControl.MouseDwellTime = uiHoverDelay;
                }
            }
        }

        [DefaultValue(100)]
        [DisplayName("Display Delay")]
        [LocalizedCategory("FlashDevelop.Category.Controls")]
        [LocalizedDescription("FlashDevelop.Description.DisplayDelay")]
        public Int32 DisplayDelay
        {
            get { return this.uiDisplayDelay; }
            set { this.uiDisplayDelay = value; }
        }

        [DefaultValue(false)]
        [DisplayName("Show Details")]
        [LocalizedCategory("FlashDevelop.Category.Controls")]
        [LocalizedDescription("FlashDevelop.Description.ShowDetails")]
        public Boolean ShowDetails
        {
            get { return this.uiShowDetails; }
            set { this.uiShowDetails = value; }
        }

        [DefaultValue(true)]
        [DisplayName("Auto Filter List")]
        [LocalizedCategory("FlashDevelop.Category.Controls")]
        [LocalizedDescription("FlashDevelop.Description.AutoFilterList")]
        public Boolean AutoFilterList
        {
            get { return this.uiAutoFilterList; }
            set { this.uiAutoFilterList = value; }
        }

        [DefaultValue(true)]
        [DisplayName("Enable AutoHide")]
        [LocalizedCategory("FlashDevelop.Category.Controls")]
        [LocalizedDescription("FlashDevelop.Description.EnableAutoHide")]
        public Boolean EnableAutoHide
        {
            get { return this.uiEnableAutoHide; }
            set { this.uiEnableAutoHide = value; }
        }

        [DefaultValue(false)]
        [DisplayName("Wrap List")]
        [LocalizedCategory("FlashDevelop.Category.Controls")]
        [LocalizedDescription("FlashDevelop.Description.WrapList")]
        public Boolean WrapList
        {
            get { return this.uiWrapList; }
            set { this.uiWrapList = value; }
        }

        [DefaultValue(false)]
        [DisplayName("Disable Smart Matching")]
        [LocalizedCategory("FlashDevelop.Category.Controls")]
        [LocalizedDescription("FlashDevelop.Description.DisableSmartMatch")]
        public Boolean DisableSmartMatch
        {
            get { return this.uiDisableSmartMatch; }
            set { this.uiDisableSmartMatch = value; }
        }

        [DefaultValue("")]
        [DisplayName("Completion List Insertion Triggers")]
        [LocalizedCategory("FlashDevelop.Category.Controls")]
        [LocalizedDescription("FlashDevelop.Description.InsertionTriggers")]
        public String InsertionTriggers
        {
            get { return this.uiInsertionTriggers; }
            set { this.uiInsertionTriggers = value; }
        }

        #endregion

        #region Paths

        [DefaultValue("")]
        [DisplayName("Custom Snippet Directory")]
        [LocalizedCategory("FlashDevelop.Category.Paths")]
        [LocalizedDescription("FlashDevelop.Description.CustomSnippetDir")]
        [Editor(typeof(FolderNameEditor), typeof(UITypeEditor))]
        public String CustomSnippetDir
        {
            get { return this.customSnippetDir; }
            set { this.customSnippetDir = value; }
        }

        [DefaultValue("")]
        [DisplayName("Custom Template Directory")]
        [LocalizedCategory("FlashDevelop.Category.Paths")]
        [LocalizedDescription("FlashDevelop.Description.CustomTemplateDir")]
        [Editor(typeof(FolderNameEditor), typeof(UITypeEditor))]
        public String CustomTemplateDir
        {
            get { return this.customTemplateDir; }
            set { this.customTemplateDir = value; }
        }

        [DefaultValue("")]
        [DisplayName("Custom Projects Directory")]
        [LocalizedCategory("FlashDevelop.Category.Paths")]
        [LocalizedDescription("FlashDevelop.Description.CustomProjectsDir")]
        [Editor(typeof(FolderNameEditor), typeof(UITypeEditor))]
        public String CustomProjectsDir
        {
            get { return this.customProjectsDir; }
            set { this.customProjectsDir = value; }
        }

        #endregion

    }

}
