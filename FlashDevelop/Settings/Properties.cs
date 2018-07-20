// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using System;
using System.Drawing;
using System.Collections.Generic;
using PluginCore.Localization;
using System.Windows.Forms;
using ScintillaNet.Enums;
using PluginCore;

namespace FlashDevelop.Settings
{
    [Serializable]
    public partial class SettingObject : ISettings
    {
        private CodePage defaultCodePage = CodePage.UTF8;
        private EndOfLine eolMode = EndOfLine.CRLF;
        private IndentView indentView = IndentView.Real;
        private FoldFlag foldFlags = FoldFlag.LineAfterContracted;
        private SmartIndent smartIndentType = SmartIndent.CPP;
        private VirtualSpaceMode virtualSpaceMode = VirtualSpaceMode.RectangularSelection;
        private CommentBlockStyle commentBlockStyle = CommentBlockStyle.Indented;
        private CodingStyle codingStyle = CodingStyle.BracesAfterLine;
        private String customProjectsDir = String.Empty;
        private String customTemplateDir = String.Empty;
        private String customSnippetDir = String.Empty;
        private String customCommandPrompt = String.Empty;
        private Font consoleFont = new Font("Courier New", 8.75F);
        private Font defaultFont = new Font("Tahoma", 8.25F);
        private Int32 tabWidth = 4;
        private Int32 indentSize = 4;
        private Int32 caretPeriod = 500;
        private Int32 caretWidth = 2;
        private Int32 scrollWidth = 3000;
        private Int32 latestCommand = 0;
        private Int32 printMarginColumn = 0;
        private Int32 backupInterval = 15000;
        private Int32 filePollInterval = 3000;
        private Int32 maxRecentFiles = 15;
        private Int32 highlightMatchingWordsDelay = 1200;
        private HighlightMatchingWordsMode highlightMatchingWordsMode = HighlightMatchingWordsMode.SelectionOrPosition;
        private LocaleVersion localeVersion = LocaleVersion.en_US;
        private List<String> previousDocuments = new List<String>();
        private List<String> disabledPlugins = new List<String>();
        private String latestDialogPath = Application.StartupPath;
        private String defaultFileExtension = DistroConfig.DISTRIBUTION_EXT;
        private Boolean confirmOnExit = false;
        private Boolean keepIndentTabs = false;
        private Boolean keepCaretCentered = false;
        private Boolean endAtLastLine = false;
        private Boolean disableFindOptionSync = false;
        private Boolean disableSimpleQuickFind = false;
        private Boolean disableTabDifferentiation = false;
        private Boolean disableReplaceFilesConfirm = false;
        private Boolean autoReloadModifiedFiles = false;
        private Boolean saveUnicodeWithBOM = false;
        private Boolean disableFindTextUpdating = false;
        private Boolean redirectFilesResults = true;
        private Boolean applyFileExtension = true;
        private Boolean restoreFileStates = true;
        private Boolean restoreFileSession = true;
        private Boolean backSpaceUnIndents = false;
        private Boolean braceMatchingEnabled = true;
        private Boolean caretLineVisible = false;
        private Boolean ensureConsistentLineEnds = true;
        private Boolean ensureLastLineEnd = false;
        private Boolean foldAtElse = false;
        private Boolean foldComment = true;
        private Boolean foldCompact = false;
        private Boolean foldHtml = true;
        private Boolean foldPreprocessor = true;
        private Boolean highlightGuide = true;
        private Boolean lineCommentsAfterIndent = true;
        private Boolean moveCursorAfterComment = false;
        private Boolean stripTrailingSpaces = false;
        private Boolean sequentialTabbing = false;
        private Boolean tabIndents = true;
        private Boolean useFolding = true;
        private Boolean useTabs = true;
        private Boolean viewEOL = false;
        private Boolean viewBookmarks = true;
        private Boolean viewLineNumbers = true;
        private Boolean viewIndentationGuides = true;
        private Boolean viewShortcuts = true;
        private Boolean viewToolBar = true;
        private Boolean viewStatusBar = true;
        private Boolean viewWhitespace = false;
        private Boolean viewModifiedLines = false;
        private Boolean wrapText = false;
        private FormWindowState windowState = FormWindowState.Maximized;
        private Point windowPosition = new Point(Screen.PrimaryScreen.WorkingArea.Left + 100, Screen.PrimaryScreen.WorkingArea.Top + 70);
        private Size windowSize = new Size(Screen.PrimaryScreen.WorkingArea.Right - 200, Screen.PrimaryScreen.WorkingArea.Bottom - 140);
        private UpdateInterval checkForUpdates = UpdateInterval.Monthly;
        private Int32 uiHoverDelay = 500;
        private Int32 uiDisplayDelay = 100;
        private Int64 lastUpdateCheck = 0;
        private Boolean uiShowDetails = false;
        private Boolean uiAutoFilterList = true;
        private Boolean uiEnableAutoHide = true;
        private Boolean uiWrapList = false;
        private Boolean uiDisableSmartMatch = false;
        private String uiInsertionTriggers = "";
        private Int32 clipboardHistorySize = 50;
    }

}
