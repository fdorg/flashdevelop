// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
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
    public partial class SettingObject
    {
        #region Folding

        [DefaultValue(false)]
        [DisplayName("Fold At Else")]
        [LocalizedCategory("FlashDevelop.Category.Folding")]
        [LocalizedDescription("FlashDevelop.Description.FoldAtElse")]
        public bool FoldAtElse
        {
            get => foldAtElse;
            set => foldAtElse = value;
        }

        [DefaultValue(true)]
        [DisplayName("Fold At Comment")]
        [LocalizedCategory("FlashDevelop.Category.Folding")]
        [LocalizedDescription("FlashDevelop.Description.FoldComment")]
        public bool FoldComment
        {
            get => foldComment;
            set => foldComment = value;
        }

        [DefaultValue(false)]
        [DisplayName("Use Compact Folding")]
        [LocalizedCategory("FlashDevelop.Category.Folding")]
        [LocalizedDescription("FlashDevelop.Description.FoldCompact")]
        public bool FoldCompact
        {
            get => foldCompact;
            set => foldCompact = value;
        }

        [DefaultValue(true)]
        [DisplayName("Fold At Html")]
        [LocalizedCategory("FlashDevelop.Category.Folding")]
        [LocalizedDescription("FlashDevelop.Description.FoldHtml")]
        public bool FoldHtml
        {
            get => foldHtml;
            set => foldHtml = value;
        }

        [DefaultValue(true)]
        [DisplayName("Fold At Preprocessor")]
        [LocalizedCategory("FlashDevelop.Category.Folding")]
        [LocalizedDescription("FlashDevelop.Description.FoldPreprocessor")]
        public bool FoldPreprocessor
        {
            get => foldPreprocessor;
            set => foldPreprocessor = value;
        }

        [DefaultValue(true)]
        [DisplayName("Enable Folding")]
        [LocalizedCategory("FlashDevelop.Category.Folding")]
        [LocalizedDescription("FlashDevelop.Description.UseFolding")]
        public bool UseFolding
        {
            get => useFolding;
            set => useFolding = value;
        }

        [DisplayName("Fold Flags")]
        [LocalizedCategory("FlashDevelop.Category.Folding")]
        [DefaultValue(FoldFlag.LineAfterContracted)]
        [LocalizedDescription("FlashDevelop.Description.FoldFlags")]
        public FoldFlag FoldFlags
        {
            get => foldFlags;
            set => foldFlags = value;
        }

        #endregion

        #region Display

        [DefaultValue(false)]
        [DisplayName("View EOL Characters")]
        [LocalizedCategory("FlashDevelop.Category.Display")]
        [LocalizedDescription("FlashDevelop.Description.ViewEOL")]
        public bool ViewEOL
        {
            get => viewEOL;
            set => viewEOL = value;
        }

        [DefaultValue(true)]
        [DisplayName("View Shortcuts")]
        [LocalizedCategory("FlashDevelop.Category.Display")]
        [LocalizedDescription("FlashDevelop.Description.ViewShortcuts")]
        public bool ViewShortcuts
        {
            get => viewShortcuts;
            set => viewShortcuts = value;
        }

        [DefaultValue(true)]
        [DisplayName("View Bookmarks")]
        [LocalizedCategory("FlashDevelop.Category.Display")]
        [LocalizedDescription("FlashDevelop.Description.ViewBookmarks")]
        public bool ViewBookmarks
        {
            get => viewBookmarks;
            set => viewBookmarks = value;
        }

        [DefaultValue(true)]
        [DisplayName("View Line Numbers")]
        [LocalizedCategory("FlashDevelop.Category.Display")]
        [LocalizedDescription("FlashDevelop.Description.ViewLineNumbers")]
        public bool ViewLineNumbers
        {
            get => viewLineNumbers;
            set => viewLineNumbers = value;
        }

        [DefaultValue(true)]
        [DisplayName("View Indentation Guides")]
        [LocalizedCategory("FlashDevelop.Category.Display")]
        [LocalizedDescription("FlashDevelop.Description.ViewIndentationGuides")]
        public bool ViewIndentationGuides
        {
            get => viewIndentationGuides;
            set => viewIndentationGuides = value;
        }

        [DefaultValue(false)]
        [DisplayName("View Whitespace Characters")]
        [LocalizedCategory("FlashDevelop.Category.Display")]
        [LocalizedDescription("FlashDevelop.Description.ViewWhitespace")]
        public bool ViewWhitespace
        {
            get => viewWhitespace;
            set => viewWhitespace = value;
        }

        [DefaultValue(true)]
        [DisplayName("View ToolBar")]
        [LocalizedCategory("FlashDevelop.Category.Display")]
        [LocalizedDescription("FlashDevelop.Description.ViewToolBar")]
        public bool ViewToolBar
        {
            get => viewToolBar;
            set => viewToolBar = value;
        }

        [DefaultValue(true)]
        [DisplayName("View StatusBar")]
        [LocalizedCategory("FlashDevelop.Category.Display")]
        [LocalizedDescription("FlashDevelop.Description.ViewStatusBar")]
        public bool ViewStatusBar
        {
            get => viewStatusBar;
            set => viewStatusBar = value;
        }

        [DefaultValue(false)]
        [DisplayName("View Modified Lines")]
        [LocalizedCategory("FlashDevelop.Category.Display")]
        [LocalizedDescription("FlashDevelop.Description.ViewModifiedLines")]
        public bool ViewModifiedLines
        {
            get => viewModifiedLines;
            set => viewModifiedLines = value;
        }

        [XmlIgnore]
        [DisplayName("UI Console Font")]
        [LocalizedCategory("FlashDevelop.Category.Display")]
        [LocalizedDescription("FlashDevelop.Description.ConsoleFont")]
        [DefaultValue(typeof(Font), "Courier New, 8.75pt")]
        public Font ConsoleFont
        {
            get => consoleFont;
            set => consoleFont = value;
        }

        [XmlIgnore]
        [DisplayName("UI Default Font")]
        [LocalizedCategory("FlashDevelop.Category.Display")]
        [LocalizedDescription("FlashDevelop.Description.DefaultFont")]
        [DefaultValue(typeof(Font), "Tahoma, 8.25pt")]
        [RequiresRestart]
        public Font DefaultFont
        {
            get => defaultFont;
            set => defaultFont = value;
        }

        #endregion

        #region Editor

        [DefaultValue(false)]
        [DisplayName("Highlight Caret Line")]
        [LocalizedCategory("FlashDevelop.Category.Editor")]
        [LocalizedDescription("FlashDevelop.Description.CaretLineVisible")]
        public bool CaretLineVisible
        {
            get => caretLineVisible;
            set => caretLineVisible = value;
        }

        [DefaultValue(false)]
        [DisplayName("Keep Caret Centered")]
        [LocalizedCategory("FlashDevelop.Category.Editor")]
        [LocalizedDescription("FlashDevelop.Description.KeepCaretCentered")]
        public bool KeepCaretCentered
        {
            get => keepCaretCentered;
            set => keepCaretCentered = value;
        }

        [DefaultValue(false)]
        [DisplayName("End At Last Line")]
        [LocalizedCategory("FlashDevelop.Category.Editor")]
        [LocalizedDescription("FlashDevelop.Description.EndAtLastLine")]
        public bool EndAtLastLine
        {
            get => endAtLastLine;
            set => endAtLastLine = value;
        }

        [DefaultValue(true)]
        [DisplayName("Disable Highlight Guide")]
        [LocalizedCategory("FlashDevelop.Category.Editor")]
        [LocalizedDescription("FlashDevelop.Description.HighlightGuide")]
        public bool HighlightGuide
        {
            get => highlightGuide;
            set => highlightGuide = value;
        }

        [DefaultValue(0)]
        [DisplayName("Print Margin Column")]
        [LocalizedCategory("FlashDevelop.Category.Editor")]
        [LocalizedDescription("FlashDevelop.Description.PrintMarginColumn")]
        public int PrintMarginColumn
        {
            get => printMarginColumn;
            set => printMarginColumn = value;
        }

        [DisplayName("Virtual Space Mode")]
        [LocalizedCategory("FlashDevelop.Category.Editor")]
        [LocalizedDescription("FlashDevelop.Description.VirtualSpaceMode")]
        [DefaultValue(VirtualSpaceMode.RectangularSelection)]
        public VirtualSpaceMode VirtualSpaceMode
        {
            get => virtualSpaceMode;
            set => virtualSpaceMode = value;
        }

        [DisplayName("End Of Line Mode")]
        [DefaultValue(EndOfLine.CRLF)]
        [LocalizedCategory("FlashDevelop.Category.Editor")]
        [LocalizedDescription("FlashDevelop.Description.EOLMode")]
        public EndOfLine EOLMode
        {
            get => eolMode;
            set => eolMode = value;
        }

        [DefaultValue(500)]
        [DisplayName("Caret Period")]
        [LocalizedCategory("FlashDevelop.Category.Editor")]
        [LocalizedDescription("FlashDevelop.Description.CaretPeriod")]
        public int CaretPeriod
        {
            get => caretPeriod;
            set => caretPeriod = value;
        }

        [DefaultValue(2)]
        [DisplayName("Caret Width")]
        [LocalizedCategory("FlashDevelop.Category.Editor")]
        [LocalizedDescription("FlashDevelop.Description.CaretWidth")]
        public int CaretWidth
        {
            get => caretWidth;
            set => caretWidth = value;
        }

        [DefaultValue(3000)]
        [DisplayName("Scroll Area Width")]
        [LocalizedCategory("FlashDevelop.Category.Editor")]
        [LocalizedDescription("FlashDevelop.Description.ScrollWidth")]
        public int ScrollWidth
        {
            get => scrollWidth;
            set
            {
                if (value < 0) throw new ArgumentOutOfRangeException();
                scrollWidth = value;
            }
        }

        [DefaultValue(DistroConfig.DISTRIBUTION_EXT)]
        [DisplayName("Default File Extension")]
        [LocalizedCategory("FlashDevelop.Category.Editor")]
        [LocalizedDescription("FlashDevelop.Description.DefaultFileExtension")]
        public string DefaultFileExtension
        {
            get => defaultFileExtension;
            set => defaultFileExtension = value;
        }

        [DefaultValue(15000)]
        [DisplayName("Backup Interval")]
        [LocalizedCategory("FlashDevelop.Category.Editor")]
        [LocalizedDescription("FlashDevelop.Description.BackupInterval")]
        public int BackupInterval
        {
            get => backupInterval;
            set => backupInterval = value;
        }

        [DefaultValue(3000)]
        [DisplayName("File Poll Interval")]
        [LocalizedCategory("FlashDevelop.Category.Editor")]
        [LocalizedDescription("FlashDevelop.Description.FilePollInterval")]
        public int FilePollInterval
        {
            get => filePollInterval;
            set => filePollInterval = value;
        }

        [DefaultValue(HighlightMatchingWordsMode.SelectionOrPosition)]
        [DisplayName("Highlight Matching Words Mode")]
        [LocalizedCategory("FlashDevelop.Category.Editor")]
        [LocalizedDescription("FlashDevelop.Description.HighlightMatchingWordsMode")]
        public HighlightMatchingWordsMode HighlightMatchingWordsMode
        {
            get => highlightMatchingWordsMode;
            set => highlightMatchingWordsMode = value;
        }

        [DefaultValue(false)]
        [DisplayName("Highlight Matching Words Case-Sensitive")]
        [LocalizedCategory("FlashDevelop.Category.Editor")]
        [LocalizedDescription("FlashDevelop.Description.HighlightMatchingWordsCaseSensitive")]
        public bool HighlightMatchingWordsCaseSensitive { get; set; } = false;

        [DefaultValue(1200)]
        [DisplayName("Highlight Matching Words Delay")]
        [LocalizedCategory("FlashDevelop.Category.Editor")]
        [LocalizedDescription("FlashDevelop.Description.HighlightMatchingWordsDelay")]
        public int HighlightMatchingWordsDelay
        {
            get
            {
                if (highlightMatchingWordsDelay <= 0) highlightMatchingWordsDelay = 1200;
                return highlightMatchingWordsDelay;
            }
            set => highlightMatchingWordsDelay = value;
        }

        [DefaultValue(50)]
        [DisplayName("Clipboard History Size")]
        [LocalizedCategory("FlashDevelop.Category.Editor")]
        [LocalizedDescription("FlashDevelop.Description.ClipboardHistorySize")]
        public int ClipboardHistorySize
        {
            get
            {
                if (clipboardHistorySize <= 0) clipboardHistorySize = 50; // value was lost in the settings file, and was set via serialization.
                return clipboardHistorySize;
            }
            set
            {
                if (value <= 0) throw new ArgumentOutOfRangeException(nameof(value));
                clipboardHistorySize = value;
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
            get => localeVersion;
            set => localeVersion = value;
        }

        [DefaultValue(CodePage.UTF8)]
        [DisplayName("Default CodePage")]
        [LocalizedCategory("FlashDevelop.Category.Locale")]
        [LocalizedDescription("FlashDevelop.Description.DefaultCodePage")]
        public CodePage DefaultCodePage
        {
            get => defaultCodePage;
            set => defaultCodePage = value;
        }

        [DefaultValue(false)]
        [DisplayName("Create Unicode With BOM")]
        [LocalizedCategory("FlashDevelop.Category.Locale")]
        [LocalizedDescription("FlashDevelop.Description.SaveUnicodeWithBOM")]
        public bool SaveUnicodeWithBOM
        {
            get => saveUnicodeWithBOM;
            set => saveUnicodeWithBOM = value;
        }

        #endregion

        #region Indenting

        [DefaultValue(false)]
        [DisplayName("Use Backspace Unindents")]
        [LocalizedCategory("FlashDevelop.Category.Indenting")]
        [LocalizedDescription("FlashDevelop.Description.BackSpaceUnIndents")]
        public bool BackSpaceUnIndents
        {
            get => backSpaceUnIndents;
            set => backSpaceUnIndents = value;
        }

        [DefaultValue(true)]
        [DisplayName("Use Tab Indents")]
        [LocalizedCategory("FlashDevelop.Category.Indenting")]
        [LocalizedDescription("FlashDevelop.Description.TabIndents")]
        public bool TabIndents
        {
            get => tabIndents;
            set => tabIndents = value;
        }

        [DefaultValue(IndentView.Real)]
        [DisplayName("Indent Guide Type")]
        [LocalizedCategory("FlashDevelop.Category.Indenting")]
        [LocalizedDescription("FlashDevelop.Description.IndentView")]
        public IndentView IndentView
        {
            get
            {
                if ((int)indentView == 0) indentView = IndentView.Real;
                return indentView;
            }
            set => indentView = value;
        }

        [DisplayName("Smart Indent Type")]
        [DefaultValue(SmartIndent.CPP)]
        [LocalizedCategory("FlashDevelop.Category.Indenting")]
        [LocalizedDescription("FlashDevelop.Description.SmartIndentType")]
        public SmartIndent SmartIndentType
        {
            get => smartIndentType;
            set => smartIndentType = value;
        }

        [DisplayName("Coding Style Type")]
        [DefaultValue(CodingStyle.BracesAfterLine)]
        [LocalizedCategory("FlashDevelop.Category.Indenting")]
        [LocalizedDescription("FlashDevelop.Description.CodingStyle")]
        public CodingStyle CodingStyle
        {
            get => codingStyle;
            set => codingStyle = value;
        }

        [DisplayName("Comment Block Indenting")]
        [DefaultValue(CommentBlockStyle.Indented)]
        [LocalizedCategory("FlashDevelop.Category.Indenting")]
        [LocalizedDescription("FlashDevelop.Description.CommentBlockStyle")]
        public CommentBlockStyle CommentBlockStyle
        {
            get => commentBlockStyle;
            set => commentBlockStyle = value;
        }

        [DefaultValue(4)]
        [DisplayName("Indenting Size")]
        [LocalizedCategory("FlashDevelop.Category.Indenting")]
        [LocalizedDescription("FlashDevelop.Description.IndentSize")]
        public int IndentSize
        {
            get => indentSize;
            set => indentSize = value;
        }

        [DefaultValue(4)]
        [DisplayName("Width Of Tab")]
        [LocalizedCategory("FlashDevelop.Category.Indenting")]
        [LocalizedDescription("FlashDevelop.Description.TabWidth")]
        public int TabWidth
        {
            get => tabWidth;
            set => tabWidth = value;
        }

        [DefaultValue(true)]
        [DisplayName("Use Tab Characters")]
        [LocalizedCategory("FlashDevelop.Category.Indenting")]
        [LocalizedDescription("FlashDevelop.Description.UseTabs")]
        public bool UseTabs
        {
            get => useTabs;
            set => useTabs = value;
        }

        #endregion

        #region Features

        [DefaultValue(true)]
        [DisplayName("Apply File Extension")]
        [LocalizedCategory("FlashDevelop.Category.Features")]
        [LocalizedDescription("FlashDevelop.Description.ApplyFileExtension")]
        public bool ApplyFileExtension
        {
            get => applyFileExtension;
            set => applyFileExtension = value;
        }

        [DefaultValue(false)]
        [DisplayName("Automatically Reload Modified Files")]
        [LocalizedCategory("FlashDevelop.Category.Features")]
        [LocalizedDescription("FlashDevelop.Description.AutoReloadModifiedFiles")]
        public bool AutoReloadModifiedFiles
        {
            get => autoReloadModifiedFiles;
            set => autoReloadModifiedFiles = value;
        }

        [DefaultValue(false)]
        [DisplayName("Use Sequential Tabbing")]
        [LocalizedCategory("FlashDevelop.Category.Features")]
        [LocalizedDescription("FlashDevelop.Description.SequentialTabbing")]
        public bool SequentialTabbing
        {
            get => sequentialTabbing;
            set => sequentialTabbing = value;
        }

        [DefaultValue(false)]
        [DisplayName("Wrap Editor Text")]
        [LocalizedCategory("FlashDevelop.Category.Features")]
        [LocalizedDescription("FlashDevelop.Description.WrapText")]
        public bool WrapText
        {
            get => wrapText;
            set => wrapText = value;
        }

        [DefaultValue(true)]
        [DisplayName("Use Brace Matching")]
        [LocalizedCategory("FlashDevelop.Category.Features")]
        [LocalizedDescription("FlashDevelop.Description.BraceMatchingEnabled")]
        public bool BraceMatchingEnabled
        {
            get => braceMatchingEnabled;
            set => braceMatchingEnabled = value;
        }

        [DefaultValue(true)]
        [DisplayName("Line Comments After Indent")]
        [LocalizedCategory("FlashDevelop.Category.Features")]
        [LocalizedDescription("FlashDevelop.Description.LineCommentsAfterIndent")]
        public bool LineCommentsAfterIndent
        {
            get => lineCommentsAfterIndent;
            set => lineCommentsAfterIndent = value;
        }

        [DefaultValue(false)]
        [DisplayName("Move Cursor After Comment")]
        [LocalizedCategory("FlashDevelop.Category.Features")]
        [LocalizedDescription("FlashDevelop.Description.MoveCursorAfterComment")]
        public bool MoveCursorAfterComment
        {
            get => moveCursorAfterComment;
            set => moveCursorAfterComment = value;
        }

        [DefaultValue(15)]
        [DisplayName("Max Recent Files")]
        [LocalizedCategory("FlashDevelop.Category.Features")]
        [LocalizedDescription("FlashDevelop.Description.MaxRecentFiles")]
        public int MaxRecentFiles
        {
            get
            {
                if (maxRecentFiles <= 0) maxRecentFiles = 15;
                return maxRecentFiles;
            }
            set => maxRecentFiles = value;
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
            get => restoreFileStates;
            set => restoreFileStates = value;
        }

        [DefaultValue(true)]
        [DisplayName("Restore File Session")]
        [LocalizedCategory("FlashDevelop.Category.Features")]
        [LocalizedDescription("FlashDevelop.Description.RestoreFileSession")]
        public bool RestoreFileSession
        {
            get => restoreFileSession;
            set => restoreFileSession = value;
        }

        [DefaultValue(false)]
        [DisplayName("Confirm On Exit")]
        [LocalizedCategory("FlashDevelop.Category.Features")]
        [LocalizedDescription("FlashDevelop.Description.ConfirmOnExit")]
        public bool ConfirmOnExit
        {
            get => confirmOnExit;
            set => confirmOnExit = value;
        }

        [DefaultValue(false)]
        [DisplayName("Disable Replace In Files Confirm")]
        [LocalizedCategory("FlashDevelop.Category.Features")]
        [LocalizedDescription("FlashDevelop.Description.DisableReplaceFilesConfirm")]
        public bool DisableReplaceFilesConfirm
        {
            get => disableReplaceFilesConfirm;
            set => disableReplaceFilesConfirm = value;
        }

        [DefaultValue(true)]
        [DisplayName("Redirect Find In Files Results")]
        [LocalizedCategory("FlashDevelop.Category.Features")]
        [LocalizedDescription("FlashDevelop.Description.RedirectFilesResults")]
        public bool RedirectFilesResults
        {
            get => redirectFilesResults;
            set => redirectFilesResults = value;
        }

        [DefaultValue(false)]
        [DisplayName("Disable Find Option Sync")]
        [LocalizedCategory("FlashDevelop.Category.Features")]
        [LocalizedDescription("FlashDevelop.Description.DisableFindOptionSync")]
        public bool DisableFindOptionSync
        {
            get => disableFindOptionSync;
            set => disableFindOptionSync = value;
        }

        [DefaultValue(false)]
        [DisplayName("Disable Find Text Updating")]
        [LocalizedCategory("FlashDevelop.Category.Features")]
        [LocalizedDescription("FlashDevelop.Description.DisableFindTextUpdating")]
        public bool DisableFindTextUpdating
        {
            get => disableFindTextUpdating;
            set => disableFindTextUpdating = value;
        }

        [DefaultValue(false)]
        [DisplayName("Disable Simple Quick Find")]
        [LocalizedCategory("FlashDevelop.Category.Features")]
        [LocalizedDescription("FlashDevelop.Description.DisableSimpleQuickFind")]
        public bool DisableSimpleQuickFind
        {
            get => disableSimpleQuickFind;
            set => disableSimpleQuickFind = value;
        }

        [DefaultValue(false)]
        [DisplayName("Disable Tab Differentiation")]
        [LocalizedCategory("FlashDevelop.Category.Features")]
        [LocalizedDescription("FlashDevelop.Description.DisableTabDifferentiation")]
        public bool DisableTabDifferentiation
        {
            get => disableTabDifferentiation;
            set => disableTabDifferentiation = value;
        }

        #endregion

        #region Formatting

        [DefaultValue(false)]
        [DisplayName("Keep Indent Tabs")]
        [LocalizedCategory("FlashDevelop.Category.Formatting")]
        [LocalizedDescription("FlashDevelop.Description.KeepIndentTabs")]
        public bool KeepIndentTabs
        {
            get => keepIndentTabs;
            set => keepIndentTabs = value;
        }

        [DefaultValue(false)]
        [DisplayName("Trim Trailing Whitespace")]
        [LocalizedCategory("FlashDevelop.Category.Formatting")]
        [LocalizedDescription("FlashDevelop.Description.StripTrailingSpaces")]
        public bool StripTrailingSpaces
        {
            get => stripTrailingSpaces;
            set => stripTrailingSpaces = value;
        }

        [DefaultValue(true)]
        [DisplayName("Ensure Consistent Line Ends")]
        [LocalizedCategory("FlashDevelop.Category.Formatting")]
        [LocalizedDescription("FlashDevelop.Description.EnsureConsistentLineEnds")]
        public bool EnsureConsistentLineEnds
        {
            get => ensureConsistentLineEnds;
            set => ensureConsistentLineEnds = value;
        }

        [DefaultValue(false)]
        [DisplayName("Ensure Last Line End")]
        [LocalizedCategory("FlashDevelop.Category.Formatting")]
        [LocalizedDescription("FlashDevelop.Description.EnsureLastLineEnd")]
        public bool EnsureLastLineEnd
        {
            get => ensureLastLineEnd;
            set => ensureLastLineEnd = value;
        }

        #endregion

        #region State

        [DisplayName("Check For Updates")]
        [LocalizedCategory("FlashDevelop.Category.State")]
        [LocalizedDescription("FlashDevelop.Description.CheckForUpdates")]
        public UpdateInterval CheckForUpdates
        {
            get => checkForUpdates;
            set => checkForUpdates = value;
        }

        [DisplayName("Automatically Check Updates For")]
        [LocalizedCategory("FlashDevelop.Category.State")]
        [DefaultValue(UpdateType.StableRelease)]
        public UpdateType AutomaticallyCheckUpdatesFor
        {
            get;
            set;
        } = UpdateType.StableRelease;

        [DisplayName("Latest Startup Command")]
        [LocalizedCategory("FlashDevelop.Category.State")]
        [LocalizedDescription("FlashDevelop.Description.LatestCommand")]
        public int LatestCommand
        {
            get => latestCommand;
            set => latestCommand = value;
        }

        [DisplayName("Last Active Path")]
        [LocalizedCategory("FlashDevelop.Category.State")]
        [LocalizedDescription("FlashDevelop.Description.LatestDialogPath")]
        [Editor(typeof(VistaFolderNameEditor), typeof(UITypeEditor))]
        public string LatestDialogPath
        {
            get => latestDialogPath;
            set => latestDialogPath = value;
        }

        [DisplayName("Window Size")]
        [LocalizedCategory("FlashDevelop.Category.State")]
        [LocalizedDescription("FlashDevelop.Description.WindowSize")]
        public Size WindowSize
        {
            get => windowSize;
            set => windowSize = value;
        }

        [DisplayName("Window State")]
        [LocalizedCategory("FlashDevelop.Category.State")]
        [LocalizedDescription("FlashDevelop.Description.WindowState")]
        public FormWindowState WindowState
        {
            get => windowState;
            set => windowState = value;
        }

        [DisplayName("Window Position")]
        [LocalizedCategory("FlashDevelop.Category.State")]
        [LocalizedDescription("FlashDevelop.Description.WindowPosition")]
        public Point WindowPosition
        {
            get => windowPosition;
            set => windowPosition = value;
        }

        [DisplayName("Previous Documents")]
        [LocalizedCategory("FlashDevelop.Category.State")]
        [LocalizedDescription("FlashDevelop.Description.PreviousDocuments")]
        [Editor("System.Windows.Forms.Design.StringCollectionEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor,System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
        public List<string> PreviousDocuments
        {
            get => previousDocuments;
            set => previousDocuments = value;
        }

        [DisplayName("Disabled Plugins")]
        [LocalizedCategory("FlashDevelop.Category.State")]
        [LocalizedDescription("FlashDevelop.Description.DisabledPlugins")]
        [Editor("System.Windows.Forms.Design.StringCollectionEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor,System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
        public List<string> DisabledPlugins
        {
            get => disabledPlugins;
            set => disabledPlugins = value;
        }

        #endregion

        #region Controls

        [DefaultValue(500)]
        [DisplayName("Hover Delay")]
        [LocalizedCategory("FlashDevelop.Category.Controls")]
        [LocalizedDescription("FlashDevelop.Description.HoverDelay")]
        public int HoverDelay
        {
            get => uiHoverDelay;
            set
            {
                uiHoverDelay = value;
                foreach (var doc in PluginBase.MainForm.Documents)
                {
                    if (doc.SciControl is { } sci) sci.MouseDwellTime = uiHoverDelay;
                }
            }
        }

        [DefaultValue(100)]
        [DisplayName("Display Delay")]
        [LocalizedCategory("FlashDevelop.Category.Controls")]
        [LocalizedDescription("FlashDevelop.Description.DisplayDelay")]
        public int DisplayDelay
        {
            get => uiDisplayDelay;
            set => uiDisplayDelay = value;
        }

        [DefaultValue(false)]
        [DisplayName("Show Details")]
        [LocalizedCategory("FlashDevelop.Category.Controls")]
        [LocalizedDescription("FlashDevelop.Description.ShowDetails")]
        public bool ShowDetails
        {
            get => uiShowDetails;
            set => uiShowDetails = value;
        }

        [DefaultValue(true)]
        [DisplayName("Auto Filter List")]
        [LocalizedCategory("FlashDevelop.Category.Controls")]
        [LocalizedDescription("FlashDevelop.Description.AutoFilterList")]
        public bool AutoFilterList
        {
            get => uiAutoFilterList;
            set => uiAutoFilterList = value;
        }

        [DefaultValue(true)]
        [DisplayName("Enable AutoHide")]
        [LocalizedCategory("FlashDevelop.Category.Controls")]
        [LocalizedDescription("FlashDevelop.Description.EnableAutoHide")]
        public bool EnableAutoHide
        {
            get => uiEnableAutoHide;
            set => uiEnableAutoHide = value;
        }

        [DefaultValue(false)]
        [DisplayName("Wrap List")]
        [LocalizedCategory("FlashDevelop.Category.Controls")]
        [LocalizedDescription("FlashDevelop.Description.WrapList")]
        public bool WrapList
        {
            get => uiWrapList;
            set => uiWrapList = value;
        }

        [DefaultValue(false)]
        [DisplayName("Disable Smart Matching")]
        [LocalizedCategory("FlashDevelop.Category.Controls")]
        [LocalizedDescription("FlashDevelop.Description.DisableSmartMatch")]
        public bool DisableSmartMatch
        {
            get => uiDisableSmartMatch;
            set => uiDisableSmartMatch = value;
        }

        [DefaultValue("")]
        [DisplayName("Completion List Insertion Triggers")]
        [LocalizedCategory("FlashDevelop.Category.Controls")]
        [LocalizedDescription("FlashDevelop.Description.InsertionTriggers")]
        public string InsertionTriggers
        {
            get => uiInsertionTriggers;
            set => uiInsertionTriggers = value;
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
            get => customSnippetDir;
            set => customSnippetDir = value;
        }

        [DefaultValue("")]
        [DisplayName("Custom Template Directory")]
        [LocalizedCategory("FlashDevelop.Category.Paths")]
        [LocalizedDescription("FlashDevelop.Description.CustomTemplateDir")]
        [Editor(typeof(VistaFolderNameEditor), typeof(UITypeEditor))]
        public string CustomTemplateDir
        {
            get => customTemplateDir;
            set => customTemplateDir = value;
        }

        [DefaultValue("")]
        [DisplayName("Custom Projects Directory")]
        [LocalizedCategory("FlashDevelop.Category.Paths")]
        [LocalizedDescription("FlashDevelop.Description.CustomProjectsDir")]
        [Editor(typeof(VistaFolderNameEditor), typeof(UITypeEditor))]
        public string CustomProjectsDir
        {
            get => customProjectsDir;
            set => customProjectsDir = value;
        }

        [DefaultValue("")]
        [DisplayName("Custom Command Prompt")]
        [LocalizedCategory("FlashDevelop.Category.Paths")]
        [LocalizedDescription("FlashDevelop.Description.CustomCommandPrompt")]
        [Editor(typeof(FileNameEditor), typeof(UITypeEditor))]
        public string CustomCommandPrompt
        {
            get => customCommandPrompt;
            set => customCommandPrompt = value;
        }

        #endregion

        #region Hidden

        [Browsable(false)]
        public long LastUpdateCheck
        {
            get => lastUpdateCheck;
            set => lastUpdateCheck = value;
        }

        #endregion

        #region Legacy

        [Browsable(false)]
        public bool UseListViewGrouping
        {
            get => PluginBase.MainForm.GetThemeFlag("ListView.UseGrouping", true);
            set {}
        }

        [Browsable(false)]
        public UiRenderMode RenderMode
        {
            get => PluginBase.MainForm.GetThemeValue("Global.UiRenderMode", "Professional") switch
            {
                "System" => UiRenderMode.System,
                _ => UiRenderMode.Professional
            };
            set {}
        }

        [Browsable(false)]
        public FlatStyle ComboBoxFlatStyle
        {
            get => PluginBase.MainForm.GetThemeValue("ComboBox.FlatStyle", "Standard") switch
            {
                "Flat" => FlatStyle.Flat,
                "Standard" => FlatStyle.Standard,
                "System" => FlatStyle.System,
                _ => FlatStyle.Popup,
            };
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