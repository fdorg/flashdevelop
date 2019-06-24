using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Xml.Serialization;
using Ookii.Dialogs;
using PluginCore;
using PluginCore.Localization;
using ScintillaNet.Enums;

namespace FlashDevelop.Settings
{
    public partial class SettingObject : ISettings
    {
        #region Folding

        [DefaultValue(false)]
        [DisplayName("Fold At Else")]
        [LocalizedCategory("FlashDevelop.Category.Folding")]
        [LocalizedDescription("FlashDevelop.Description.FoldAtElse")]
        public bool FoldAtElse
        {
            get => this.foldAtElse;
            set => this.foldAtElse = value;
        }

        [DefaultValue(true)]
        [DisplayName("Fold At Comment")]
        [LocalizedCategory("FlashDevelop.Category.Folding")]
        [LocalizedDescription("FlashDevelop.Description.FoldComment")]
        public bool FoldComment
        {
            get => this.foldComment;
            set => this.foldComment = value;
        }

        [DefaultValue(false)]
        [DisplayName("Use Compact Folding")]
        [LocalizedCategory("FlashDevelop.Category.Folding")]
        [LocalizedDescription("FlashDevelop.Description.FoldCompact")]
        public bool FoldCompact
        {
            get => this.foldCompact;
            set => this.foldCompact = value;
        }

        [DefaultValue(true)]
        [DisplayName("Fold At Html")]
        [LocalizedCategory("FlashDevelop.Category.Folding")]
        [LocalizedDescription("FlashDevelop.Description.FoldHtml")]
        public bool FoldHtml
        {
            get => this.foldHtml;
            set => this.foldHtml = value;
        }

        [DefaultValue(true)]
        [DisplayName("Fold At Preprocessor")]
        [LocalizedCategory("FlashDevelop.Category.Folding")]
        [LocalizedDescription("FlashDevelop.Description.FoldPreprocessor")]
        public bool FoldPreprocessor
        {
            get => this.foldPreprocessor;
            set => this.foldPreprocessor = value;
        }

        [DefaultValue(true)]
        [DisplayName("Enable Folding")]
        [LocalizedCategory("FlashDevelop.Category.Folding")]
        [LocalizedDescription("FlashDevelop.Description.UseFolding")]
        public bool UseFolding
        {
            get => this.useFolding;
            set => this.useFolding = value;
        }

        [DisplayName("Fold Flags")]
        [LocalizedCategory("FlashDevelop.Category.Folding")]
        [DefaultValue(FoldFlag.LineAfterContracted)]
        [LocalizedDescription("FlashDevelop.Description.FoldFlags")]
        public FoldFlag FoldFlags
        {
            get => this.foldFlags;
            set => this.foldFlags = value;
        }

        #endregion

        #region Display

        [DefaultValue(false)]
        [DisplayName("View EOL Characters")]
        [LocalizedCategory("FlashDevelop.Category.Display")]
        [LocalizedDescription("FlashDevelop.Description.ViewEOL")]
        public bool ViewEOL
        {
            get => this.viewEOL;
            set => this.viewEOL = value;
        }

        [DefaultValue(true)]
        [DisplayName("View Shortcuts")]
        [LocalizedCategory("FlashDevelop.Category.Display")]
        [LocalizedDescription("FlashDevelop.Description.ViewShortcuts")]
        public bool ViewShortcuts
        {
            get => this.viewShortcuts;
            set => this.viewShortcuts = value;
        }

        [DefaultValue(true)]
        [DisplayName("View Bookmarks")]
        [LocalizedCategory("FlashDevelop.Category.Display")]
        [LocalizedDescription("FlashDevelop.Description.ViewBookmarks")]
        public bool ViewBookmarks
        {
            get => this.viewBookmarks;
            set => this.viewBookmarks = value;
        }

        [DefaultValue(true)]
        [DisplayName("View Line Numbers")]
        [LocalizedCategory("FlashDevelop.Category.Display")]
        [LocalizedDescription("FlashDevelop.Description.ViewLineNumbers")]
        public bool ViewLineNumbers
        {
            get => this.viewLineNumbers;
            set => this.viewLineNumbers = value;
        }

        [DefaultValue(true)]
        [DisplayName("View Indentation Guides")]
        [LocalizedCategory("FlashDevelop.Category.Display")]
        [LocalizedDescription("FlashDevelop.Description.ViewIndentationGuides")]
        public bool ViewIndentationGuides
        {
            get => this.viewIndentationGuides;
            set => this.viewIndentationGuides = value;
        }

        [DefaultValue(false)]
        [DisplayName("View Whitespace Characters")]
        [LocalizedCategory("FlashDevelop.Category.Display")]
        [LocalizedDescription("FlashDevelop.Description.ViewWhitespace")]
        public bool ViewWhitespace
        {
            get => this.viewWhitespace;
            set => this.viewWhitespace = value;
        }

        [DefaultValue(true)]
        [DisplayName("View ToolBar")]
        [LocalizedCategory("FlashDevelop.Category.Display")]
        [LocalizedDescription("FlashDevelop.Description.ViewToolBar")]
        public bool ViewToolBar
        {
            get => this.viewToolBar;
            set => this.viewToolBar = value;
        }

        [DefaultValue(true)]
        [DisplayName("View StatusBar")]
        [LocalizedCategory("FlashDevelop.Category.Display")]
        [LocalizedDescription("FlashDevelop.Description.ViewStatusBar")]
        public bool ViewStatusBar
        {
            get => this.viewStatusBar;
            set => this.viewStatusBar = value;
        }

        [DefaultValue(false)]
        [DisplayName("View Modified Lines")]
        [LocalizedCategory("FlashDevelop.Category.Display")]
        [LocalizedDescription("FlashDevelop.Description.ViewModifiedLines")]
        public bool ViewModifiedLines
        {
            get => this.viewModifiedLines;
            set => this.viewModifiedLines = value;
        }

        [XmlIgnore]
        [DisplayName("UI Console Font")]
        [LocalizedCategory("FlashDevelop.Category.Display")]
        [LocalizedDescription("FlashDevelop.Description.ConsoleFont")]
        [DefaultValue(typeof(Font), "Courier New, 8.75pt")]
        public Font ConsoleFont
        {
            get => this.consoleFont;
            set => this.consoleFont = value;
        }

        [XmlIgnore]
        [DisplayName("UI Default Font")]
        [LocalizedCategory("FlashDevelop.Category.Display")]
        [LocalizedDescription("FlashDevelop.Description.DefaultFont")]
        [DefaultValue(typeof(Font), "Tahoma, 8.25pt")]
        [RequiresRestart]
        public Font DefaultFont
        {
            get => this.defaultFont;
            set => this.defaultFont = value;
        }

        #endregion

        #region Editor

        [DefaultValue(false)]
        [DisplayName("Highlight Caret Line")]
        [LocalizedCategory("FlashDevelop.Category.Editor")]
        [LocalizedDescription("FlashDevelop.Description.CaretLineVisible")]
        public bool CaretLineVisible
        {
            get => this.caretLineVisible;
            set => this.caretLineVisible = value;
        }

        [DefaultValue(false)]
        [DisplayName("Keep Caret Centered")]
        [LocalizedCategory("FlashDevelop.Category.Editor")]
        [LocalizedDescription("FlashDevelop.Description.KeepCaretCentered")]
        public bool KeepCaretCentered
        {
            get => this.keepCaretCentered;
            set => this.keepCaretCentered = value;
        }

        [DefaultValue(false)]
        [DisplayName("End At Last Line")]
        [LocalizedCategory("FlashDevelop.Category.Editor")]
        [LocalizedDescription("FlashDevelop.Description.EndAtLastLine")]
        public bool EndAtLastLine
        {
            get => this.endAtLastLine;
            set => this.endAtLastLine = value;
        }

        [DefaultValue(true)]
        [DisplayName("Disable Highlight Guide")]
        [LocalizedCategory("FlashDevelop.Category.Editor")]
        [LocalizedDescription("FlashDevelop.Description.HighlightGuide")]
        public bool HighlightGuide
        {
            get => this.highlightGuide;
            set => this.highlightGuide = value;
        }

        [DefaultValue(0)]
        [DisplayName("Print Margin Column")]
        [LocalizedCategory("FlashDevelop.Category.Editor")]
        [LocalizedDescription("FlashDevelop.Description.PrintMarginColumn")]
        public int PrintMarginColumn
        {
            get => this.printMarginColumn;
            set => this.printMarginColumn = value;
        }

        [DisplayName("Virtual Space Mode")]
        [LocalizedCategory("FlashDevelop.Category.Editor")]
        [LocalizedDescription("FlashDevelop.Description.VirtualSpaceMode")]
        [DefaultValue(VirtualSpaceMode.RectangularSelection)]
        public VirtualSpaceMode VirtualSpaceMode
        {
            get => this.virtualSpaceMode;
            set => this.virtualSpaceMode = value;
        }

        [DisplayName("End Of Line Mode")]
        [DefaultValue(EndOfLine.CRLF)]
        [LocalizedCategory("FlashDevelop.Category.Editor")]
        [LocalizedDescription("FlashDevelop.Description.EOLMode")]
        public EndOfLine EOLMode
        {
            get => this.eolMode;
            set => this.eolMode = value;
        }

        [DefaultValue(500)]
        [DisplayName("Caret Period")]
        [LocalizedCategory("FlashDevelop.Category.Editor")]
        [LocalizedDescription("FlashDevelop.Description.CaretPeriod")]
        public int CaretPeriod
        {
            get => this.caretPeriod;
            set => this.caretPeriod = value;
        }

        [DefaultValue(2)]
        [DisplayName("Caret Width")]
        [LocalizedCategory("FlashDevelop.Category.Editor")]
        [LocalizedDescription("FlashDevelop.Description.CaretWidth")]
        public int CaretWidth
        {
            get => this.caretWidth;
            set => this.caretWidth = value;
        }

        [DefaultValue(3000)]
        [DisplayName("Scroll Area Width")]
        [LocalizedCategory("FlashDevelop.Category.Editor")]
        [LocalizedDescription("FlashDevelop.Description.ScrollWidth")]
        public int ScrollWidth
        {
            get => this.scrollWidth;
            set
            {
                if (value < 0) throw new ArgumentOutOfRangeException();
                this.scrollWidth = value;
            }
        }

        [DefaultValue(DistroConfig.DISTRIBUTION_EXT)]
        [DisplayName("Default File Extension")]
        [LocalizedCategory("FlashDevelop.Category.Editor")]
        [LocalizedDescription("FlashDevelop.Description.DefaultFileExtension")]
        public string DefaultFileExtension
        {
            get => this.defaultFileExtension;
            set => this.defaultFileExtension = value;
        }

        [DefaultValue(15000)]
        [DisplayName("Backup Interval")]
        [LocalizedCategory("FlashDevelop.Category.Editor")]
        [LocalizedDescription("FlashDevelop.Description.BackupInterval")]
        public int BackupInterval
        {
            get => this.backupInterval;
            set => this.backupInterval = value;
        }

        [DefaultValue(3000)]
        [DisplayName("File Poll Interval")]
        [LocalizedCategory("FlashDevelop.Category.Editor")]
        [LocalizedDescription("FlashDevelop.Description.FilePollInterval")]
        public int FilePollInterval
        {
            get => this.filePollInterval;
            set => this.filePollInterval = value;
        }

        [DefaultValue(HighlightMatchingWordsMode.SelectionOrPosition)]
        [DisplayName("Highlight Matching Words Mode")]
        [LocalizedCategory("FlashDevelop.Category.Editor")]
        [LocalizedDescription("FlashDevelop.Description.HighlightMatchingWordsMode")]
        public HighlightMatchingWordsMode HighlightMatchingWordsMode
        {
            get => this.highlightMatchingWordsMode;
            set => this.highlightMatchingWordsMode = value;
        }

        [DefaultValue(1200)]
        [DisplayName("Highlight Matching Words Delay")]
        [LocalizedCategory("FlashDevelop.Category.Editor")]
        [LocalizedDescription("FlashDevelop.Description.HighlightMatchingWordsDelay")]
        public int HighlightMatchingWordsDelay
        {
            get
            {
                if (this.highlightMatchingWordsDelay <= 0) this.highlightMatchingWordsDelay = 1200;
                return this.highlightMatchingWordsDelay;
            }
            set => this.highlightMatchingWordsDelay = value;
        }

        [DefaultValue(50)]
        [DisplayName("Clipboard History Size")]
        [LocalizedCategory("FlashDevelop.Category.Editor")]
        [LocalizedDescription("FlashDevelop.Description.ClipboardHistorySize")]
        public int ClipboardHistorySize
        {
            get
            {
                if (this.clipboardHistorySize <= 0) this.clipboardHistorySize = 50; // value was lost in the settings file, and was set via serialization.
                return this.clipboardHistorySize;
            }
            set
            {
                if (value <= 0) throw new ArgumentOutOfRangeException(nameof(value));
                this.clipboardHistorySize = value;
            }
        }

        #endregion

        #region Locale

        [DisplayName("Selected Locale")]
        [DefaultValue(LocaleVersion.en_US)]
        [LocalizedCategory("FlashDevelop.Category.Locale")]
        [LocalizedDescription("FlashDevelop.Description.LocaleVersion")]
        [RequiresRestart]
        public LocaleVersion LocaleVersion
        {
            get => this.localeVersion;
            set => this.localeVersion = value;
        }

        [DefaultValue(CodePage.UTF8)]
        [DisplayName("Default CodePage")]
        [LocalizedCategory("FlashDevelop.Category.Locale")]
        [LocalizedDescription("FlashDevelop.Description.DefaultCodePage")]
        public CodePage DefaultCodePage
        {
            get => this.defaultCodePage;
            set => this.defaultCodePage = value;
        }

        [DefaultValue(false)]
        [DisplayName("Create Unicode With BOM")]
        [LocalizedCategory("FlashDevelop.Category.Locale")]
        [LocalizedDescription("FlashDevelop.Description.SaveUnicodeWithBOM")]
        public bool SaveUnicodeWithBOM
        {
            get => this.saveUnicodeWithBOM;
            set => this.saveUnicodeWithBOM = value;
        }

        #endregion

        #region Indenting

        [DefaultValue(false)]
        [DisplayName("Use Backspace Unindents")]
        [LocalizedCategory("FlashDevelop.Category.Indenting")]
        [LocalizedDescription("FlashDevelop.Description.BackSpaceUnIndents")]
        public bool BackSpaceUnIndents
        {
            get => this.backSpaceUnIndents;
            set => this.backSpaceUnIndents = value;
        }

        [DefaultValue(true)]
        [DisplayName("Use Tab Indents")]
        [LocalizedCategory("FlashDevelop.Category.Indenting")]
        [LocalizedDescription("FlashDevelop.Description.TabIndents")]
        public bool TabIndents
        {
            get => this.tabIndents;
            set => this.tabIndents = value;
        }

        [DefaultValue(IndentView.Real)]
        [DisplayName("Indent Guide Type")]
        [LocalizedCategory("FlashDevelop.Category.Indenting")]
        [LocalizedDescription("FlashDevelop.Description.IndentView")]
        public IndentView IndentView
        {
            get
            {
                if ((int)this.indentView == 0) this.indentView = IndentView.Real;
                return this.indentView;
            }
            set => this.indentView = value;
        }

        [DisplayName("Smart Indent Type")]
        [DefaultValue(SmartIndent.CPP)]
        [LocalizedCategory("FlashDevelop.Category.Indenting")]
        [LocalizedDescription("FlashDevelop.Description.SmartIndentType")]
        public SmartIndent SmartIndentType
        {
            get => this.smartIndentType;
            set => this.smartIndentType = value;
        }

        [DisplayName("Coding Style Type")]
        [DefaultValue(CodingStyle.BracesAfterLine)]
        [LocalizedCategory("FlashDevelop.Category.Indenting")]
        [LocalizedDescription("FlashDevelop.Description.CodingStyle")]
        public CodingStyle CodingStyle
        {
            get => this.codingStyle;
            set => this.codingStyle = value;
        }

        [DisplayName("Comment Block Indenting")]
        [DefaultValue(CommentBlockStyle.Indented)]
        [LocalizedCategory("FlashDevelop.Category.Indenting")]
        [LocalizedDescription("FlashDevelop.Description.CommentBlockStyle")]
        public CommentBlockStyle CommentBlockStyle
        {
            get => this.commentBlockStyle;
            set => this.commentBlockStyle = value;
        }

        [DefaultValue(4)]
        [DisplayName("Indenting Size")]
        [LocalizedCategory("FlashDevelop.Category.Indenting")]
        [LocalizedDescription("FlashDevelop.Description.IndentSize")]
        public int IndentSize
        {
            get => this.indentSize;
            set => this.indentSize = value;
        }

        [DefaultValue(4)]
        [DisplayName("Width Of Tab")]
        [LocalizedCategory("FlashDevelop.Category.Indenting")]
        [LocalizedDescription("FlashDevelop.Description.TabWidth")]
        public int TabWidth
        {
            get => this.tabWidth;
            set => this.tabWidth = value;
        }

        [DefaultValue(true)]
        [DisplayName("Use Tab Characters")]
        [LocalizedCategory("FlashDevelop.Category.Indenting")]
        [LocalizedDescription("FlashDevelop.Description.UseTabs")]
        public bool UseTabs
        {
            get => this.useTabs;
            set => this.useTabs = value;
        }

        #endregion

        #region Features

        [DefaultValue(true)]
        [DisplayName("Apply File Extension")]
        [LocalizedCategory("FlashDevelop.Category.Features")]
        [LocalizedDescription("FlashDevelop.Description.ApplyFileExtension")]
        public bool ApplyFileExtension
        {
            get => this.applyFileExtension;
            set => this.applyFileExtension = value;
        }

        [DefaultValue(false)]
        [DisplayName("Automatically Reload Modified Files")]
        [LocalizedCategory("FlashDevelop.Category.Features")]
        [LocalizedDescription("FlashDevelop.Description.AutoReloadModifiedFiles")]
        public bool AutoReloadModifiedFiles
        {
            get => this.autoReloadModifiedFiles;
            set => this.autoReloadModifiedFiles = value;
        }

        [DefaultValue(false)]
        [DisplayName("Use Sequential Tabbing")]
        [LocalizedCategory("FlashDevelop.Category.Features")]
        [LocalizedDescription("FlashDevelop.Description.SequentialTabbing")]
        public bool SequentialTabbing
        {
            get => this.sequentialTabbing;
            set => this.sequentialTabbing = value;
        }

        [DefaultValue(false)]
        [DisplayName("Wrap Editor Text")]
        [LocalizedCategory("FlashDevelop.Category.Features")]
        [LocalizedDescription("FlashDevelop.Description.WrapText")]
        public bool WrapText
        {
            get => this.wrapText;
            set => this.wrapText = value;
        }

        [DefaultValue(true)]
        [DisplayName("Use Brace Matching")]
        [LocalizedCategory("FlashDevelop.Category.Features")]
        [LocalizedDescription("FlashDevelop.Description.BraceMatchingEnabled")]
        public bool BraceMatchingEnabled
        {
            get => this.braceMatchingEnabled;
            set => this.braceMatchingEnabled = value;
        }

        [DefaultValue(true)]
        [DisplayName("Line Comments After Indent")]
        [LocalizedCategory("FlashDevelop.Category.Features")]
        [LocalizedDescription("FlashDevelop.Description.LineCommentsAfterIndent")]
        public bool LineCommentsAfterIndent
        {
            get => this.lineCommentsAfterIndent;
            set => this.lineCommentsAfterIndent = value;
        }

        [DefaultValue(false)]
        [DisplayName("Move Cursor After Comment")]
        [LocalizedCategory("FlashDevelop.Category.Features")]
        [LocalizedDescription("FlashDevelop.Description.MoveCursorAfterComment")]
        public bool MoveCursorAfterComment
        {
            get => this.moveCursorAfterComment;
            set => this.moveCursorAfterComment = value;
        }

        [DefaultValue(15)]
        [DisplayName("Max Recent Files")]
        [LocalizedCategory("FlashDevelop.Category.Features")]
        [LocalizedDescription("FlashDevelop.Description.MaxRecentFiles")]
        public int MaxRecentFiles
        {
            get
            {
                if (this.maxRecentFiles <= 0) this.maxRecentFiles = 15;
                return this.maxRecentFiles;
            }
            set => this.maxRecentFiles = value;
        }

        [DefaultValue(1000)]
        [DisplayName("Max Trace Lines")]
        [LocalizedCategory("FlashDevelop.Category.Features")]
        [LocalizedDescription("FlashDevelop.Description.MaxTraceLines")]
        public int MaxTraceLines
        {
            get
            {
                if (uiMaxTraceLines <= 0) uiMaxTraceLines = 1000;
                return uiMaxTraceLines;
            }
            set => uiMaxTraceLines = value;
        }

        [DefaultValue(true)]
        [DisplayName("Restore File States")]
        [LocalizedCategory("FlashDevelop.Category.Features")]
        [LocalizedDescription("FlashDevelop.Description.RestoreFileStates")]
        public bool RestoreFileStates
        {
            get => this.restoreFileStates;
            set => this.restoreFileStates = value;
        }

        [DefaultValue(true)]
        [DisplayName("Restore File Session")]
        [LocalizedCategory("FlashDevelop.Category.Features")]
        [LocalizedDescription("FlashDevelop.Description.RestoreFileSession")]
        public bool RestoreFileSession
        {
            get => this.restoreFileSession;
            set => this.restoreFileSession = value;
        }

        [DefaultValue(false)]
        [DisplayName("Confirm On Exit")]
        [LocalizedCategory("FlashDevelop.Category.Features")]
        [LocalizedDescription("FlashDevelop.Description.ConfirmOnExit")]
        public bool ConfirmOnExit
        {
            get => this.confirmOnExit;
            set => this.confirmOnExit = value;
        }

        [DefaultValue(false)]
        [DisplayName("Disable Replace In Files Confirm")]
        [LocalizedCategory("FlashDevelop.Category.Features")]
        [LocalizedDescription("FlashDevelop.Description.DisableReplaceFilesConfirm")]
        public bool DisableReplaceFilesConfirm
        {
            get => this.disableReplaceFilesConfirm;
            set => this.disableReplaceFilesConfirm = value;
        }

        [DefaultValue(true)]
        [DisplayName("Redirect Find In Files Results")]
        [LocalizedCategory("FlashDevelop.Category.Features")]
        [LocalizedDescription("FlashDevelop.Description.RedirectFilesResults")]
        public bool RedirectFilesResults
        {
            get => this.redirectFilesResults;
            set => this.redirectFilesResults = value;
        }

        [DefaultValue(false)]
        [DisplayName("Disable Find Option Sync")]
        [LocalizedCategory("FlashDevelop.Category.Features")]
        [LocalizedDescription("FlashDevelop.Description.DisableFindOptionSync")]
        public bool DisableFindOptionSync
        {
            get => this.disableFindOptionSync;
            set => this.disableFindOptionSync = value;
        }

        [DefaultValue(false)]
        [DisplayName("Disable Find Text Updating")]
        [LocalizedCategory("FlashDevelop.Category.Features")]
        [LocalizedDescription("FlashDevelop.Description.DisableFindTextUpdating")]
        public bool DisableFindTextUpdating
        {
            get => this.disableFindTextUpdating;
            set => this.disableFindTextUpdating = value;
        }

        [DefaultValue(false)]
        [DisplayName("Disable Simple Quick Find")]
        [LocalizedCategory("FlashDevelop.Category.Features")]
        [LocalizedDescription("FlashDevelop.Description.DisableSimpleQuickFind")]
        public bool DisableSimpleQuickFind
        {
            get => this.disableSimpleQuickFind;
            set => this.disableSimpleQuickFind = value;
        }

        [DefaultValue(false)]
        [DisplayName("Disable Tab Differentiation")]
        [LocalizedCategory("FlashDevelop.Category.Features")]
        [LocalizedDescription("FlashDevelop.Description.DisableTabDifferentiation")]
        public bool DisableTabDifferentiation
        {
            get => this.disableTabDifferentiation;
            set => this.disableTabDifferentiation = value;
        }

        #endregion

        #region Formatting

        [DefaultValue(false)]
        [DisplayName("Keep Indent Tabs")]
        [LocalizedCategory("FlashDevelop.Category.Formatting")]
        [LocalizedDescription("FlashDevelop.Description.KeepIndentTabs")]
        public bool KeepIndentTabs
        {
            get => this.keepIndentTabs;
            set => this.keepIndentTabs = value;
        }

        [DefaultValue(false)]
        [DisplayName("Trim Trailing Whitespace")]
        [LocalizedCategory("FlashDevelop.Category.Formatting")]
        [LocalizedDescription("FlashDevelop.Description.StripTrailingSpaces")]
        public bool StripTrailingSpaces
        {
            get => this.stripTrailingSpaces;
            set => this.stripTrailingSpaces = value;
        }

        [DefaultValue(true)]
        [DisplayName("Ensure Consistent Line Ends")]
        [LocalizedCategory("FlashDevelop.Category.Formatting")]
        [LocalizedDescription("FlashDevelop.Description.EnsureConsistentLineEnds")]
        public bool EnsureConsistentLineEnds
        {
            get => this.ensureConsistentLineEnds;
            set => this.ensureConsistentLineEnds = value;
        }

        [DefaultValue(false)]
        [DisplayName("Ensure Last Line End")]
        [LocalizedCategory("FlashDevelop.Category.Formatting")]
        [LocalizedDescription("FlashDevelop.Description.EnsureLastLineEnd")]
        public bool EnsureLastLineEnd
        {
            get => this.ensureLastLineEnd;
            set => this.ensureLastLineEnd = value;
        }

        #endregion

        #region State

        [DisplayName("Check For Updates")]
        [LocalizedCategory("FlashDevelop.Category.State")]
        [LocalizedDescription("FlashDevelop.Description.CheckForUpdates")]
        public UpdateInterval CheckForUpdates
        {
            get => this.checkForUpdates;
            set => this.checkForUpdates = value;
        }

        [DisplayName("Latest Startup Command")]
        [LocalizedCategory("FlashDevelop.Category.State")]
        [LocalizedDescription("FlashDevelop.Description.LatestCommand")]
        public int LatestCommand
        {
            get => this.latestCommand;
            set => this.latestCommand = value;
        }

        [DisplayName("Last Active Path")]
        [LocalizedCategory("FlashDevelop.Category.State")]
        [LocalizedDescription("FlashDevelop.Description.LatestDialogPath")]
        [Editor(typeof(VistaFolderNameEditor), typeof(UITypeEditor))]
        public string LatestDialogPath
        {
            get => this.latestDialogPath;
            set => this.latestDialogPath = value;
        }

        [DisplayName("Window Size")]
        [LocalizedCategory("FlashDevelop.Category.State")]
        [LocalizedDescription("FlashDevelop.Description.WindowSize")]
        public Size WindowSize
        {
            get => this.windowSize;
            set => this.windowSize = value;
        }

        [DisplayName("Window State")]
        [LocalizedCategory("FlashDevelop.Category.State")]
        [LocalizedDescription("FlashDevelop.Description.WindowState")]
        public FormWindowState WindowState
        {
            get => this.windowState;
            set => this.windowState = value;
        }

        [DisplayName("Window Position")]
        [LocalizedCategory("FlashDevelop.Category.State")]
        [LocalizedDescription("FlashDevelop.Description.WindowPosition")]
        public Point WindowPosition
        {
            get => this.windowPosition;
            set => this.windowPosition = value;
        }

        [DisplayName("Previous Documents")]
        [LocalizedCategory("FlashDevelop.Category.State")]
        [LocalizedDescription("FlashDevelop.Description.PreviousDocuments")]
        [Editor("System.Windows.Forms.Design.StringCollectionEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor,System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
        public List<string> PreviousDocuments
        {
            get => this.previousDocuments;
            set => this.previousDocuments = value;
        }

        [DisplayName("Disabled Plugins")]
        [LocalizedCategory("FlashDevelop.Category.State")]
        [LocalizedDescription("FlashDevelop.Description.DisabledPlugins")]
        [Editor("System.Windows.Forms.Design.StringCollectionEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor,System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
        public List<string> DisabledPlugins
        {
            get => this.disabledPlugins;
            set => this.disabledPlugins = value;
        }

        #endregion

        #region Controls

        [DefaultValue(500)]
        [DisplayName("Hover Delay")]
        [LocalizedCategory("FlashDevelop.Category.Controls")]
        [LocalizedDescription("FlashDevelop.Description.HoverDelay")]
        public int HoverDelay
        {
            get => this.uiHoverDelay;
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
        public int DisplayDelay
        {
            get => this.uiDisplayDelay;
            set => this.uiDisplayDelay = value;
        }

        [DefaultValue(false)]
        [DisplayName("Show Details")]
        [LocalizedCategory("FlashDevelop.Category.Controls")]
        [LocalizedDescription("FlashDevelop.Description.ShowDetails")]
        public bool ShowDetails
        {
            get => this.uiShowDetails;
            set => this.uiShowDetails = value;
        }

        [DefaultValue(true)]
        [DisplayName("Auto Filter List")]
        [LocalizedCategory("FlashDevelop.Category.Controls")]
        [LocalizedDescription("FlashDevelop.Description.AutoFilterList")]
        public bool AutoFilterList
        {
            get => this.uiAutoFilterList;
            set => this.uiAutoFilterList = value;
        }

        [DefaultValue(true)]
        [DisplayName("Enable AutoHide")]
        [LocalizedCategory("FlashDevelop.Category.Controls")]
        [LocalizedDescription("FlashDevelop.Description.EnableAutoHide")]
        public bool EnableAutoHide
        {
            get => this.uiEnableAutoHide;
            set => this.uiEnableAutoHide = value;
        }

        [DefaultValue(false)]
        [DisplayName("Wrap List")]
        [LocalizedCategory("FlashDevelop.Category.Controls")]
        [LocalizedDescription("FlashDevelop.Description.WrapList")]
        public bool WrapList
        {
            get => this.uiWrapList;
            set => this.uiWrapList = value;
        }

        [DefaultValue(false)]
        [DisplayName("Disable Smart Matching")]
        [LocalizedCategory("FlashDevelop.Category.Controls")]
        [LocalizedDescription("FlashDevelop.Description.DisableSmartMatch")]
        public bool DisableSmartMatch
        {
            get => this.uiDisableSmartMatch;
            set => this.uiDisableSmartMatch = value;
        }

        [DefaultValue("")]
        [DisplayName("Completion List Insertion Triggers")]
        [LocalizedCategory("FlashDevelop.Category.Controls")]
        [LocalizedDescription("FlashDevelop.Description.InsertionTriggers")]
        public string InsertionTriggers
        {
            get => this.uiInsertionTriggers;
            set => this.uiInsertionTriggers = value;
        }

        #endregion

        #region Paths

        [DefaultValue("")]
        [DisplayName("Custom Snippet Directory")]
        [LocalizedCategory("FlashDevelop.Category.Paths")]
        [LocalizedDescription("FlashDevelop.Description.CustomSnippetDir")]
        [Editor(typeof(VistaFolderNameEditor), typeof(UITypeEditor))]
        public string CustomSnippetDir
        {
            get => this.customSnippetDir;
            set => this.customSnippetDir = value;
        }

        [DefaultValue("")]
        [DisplayName("Custom Template Directory")]
        [LocalizedCategory("FlashDevelop.Category.Paths")]
        [LocalizedDescription("FlashDevelop.Description.CustomTemplateDir")]
        [Editor(typeof(VistaFolderNameEditor), typeof(UITypeEditor))]
        public string CustomTemplateDir
        {
            get => this.customTemplateDir;
            set => this.customTemplateDir = value;
        }

        [DefaultValue("")]
        [DisplayName("Custom Projects Directory")]
        [LocalizedCategory("FlashDevelop.Category.Paths")]
        [LocalizedDescription("FlashDevelop.Description.CustomProjectsDir")]
        [Editor(typeof(VistaFolderNameEditor), typeof(UITypeEditor))]
        public string CustomProjectsDir
        {
            get => this.customProjectsDir;
            set => this.customProjectsDir = value;
        }

        [DefaultValue("")]
        [DisplayName("Custom Command Prompt")]
        [LocalizedCategory("FlashDevelop.Category.Paths")]
        [LocalizedDescription("FlashDevelop.Description.CustomCommandPrompt")]
        [Editor(typeof(FileNameEditor), typeof(UITypeEditor))]
        public string CustomCommandPrompt
        {
            get => this.customCommandPrompt;
            set => this.customCommandPrompt = value;
        }

        #endregion

        #region Hidden

        [Browsable(false)]
        public long LastUpdateCheck
        {
            get => this.lastUpdateCheck;
            set => this.lastUpdateCheck = value;
        }

        #endregion

        #region Legacy

        [Browsable(false)]
        public bool UseListViewGrouping
        {
            get => Globals.MainForm.GetThemeFlag("ListView.UseGrouping", true);
            set {}
        }

        [Browsable(false)]
        public UiRenderMode RenderMode
        {
            get
            {
                string value = Globals.MainForm.GetThemeValue("Global.UiRenderMode", "Professional");
                if (value == "System") return UiRenderMode.System;
                return UiRenderMode.Professional;
            }
            set {}
        }

        [Browsable(false)]
        public FlatStyle ComboBoxFlatStyle
        {
            get
            {
                string value = Globals.MainForm.GetThemeValue("ComboBox.FlatStyle", "Standard");
                switch (value)
                {
                    case "Flat": return FlatStyle.Flat;
                    case "Standard": return FlatStyle.Standard;
                    case "System": return FlatStyle.System;
                    default: return FlatStyle.Popup;
                }
            }
            set {}
        }

        [Browsable(false)]
        public bool UseSystemColors
        {
            get => false;
            set {}
        }

        #endregion

    }

}
