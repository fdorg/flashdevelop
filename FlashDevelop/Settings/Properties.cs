// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
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
        private string customProjectsDir = string.Empty;
        private string customTemplateDir = string.Empty;
        private string customSnippetDir = string.Empty;
        private string customCommandPrompt = string.Empty;
        private Font consoleFont = new Font("Courier New", 8.75F);
        private Font defaultFont = new Font("Tahoma", 8.25F);
        private int tabWidth = 4;
        private int indentSize = 4;
        private int caretPeriod = 500;
        private int caretWidth = 2;
        private int scrollWidth = 3000;
        private int latestCommand = 0;
        private int printMarginColumn = 0;
        private int backupInterval = 15000;
        private int filePollInterval = 3000;
        private int maxRecentFiles = 15;
        private int highlightMatchingWordsDelay = 1200;
        private HighlightMatchingWordsMode highlightMatchingWordsMode = HighlightMatchingWordsMode.SelectionOrPosition;
        private LocaleVersion localeVersion = LocaleVersion.en_US;
        private List<string> previousDocuments = new List<string>();
        private List<string> disabledPlugins = new List<string>();
        private string latestDialogPath = Application.StartupPath;
        private string defaultFileExtension = DistroConfig.DISTRIBUTION_EXT;
        private bool confirmOnExit = false;
        private bool keepIndentTabs = false;
        private bool keepCaretCentered = false;
        private bool endAtLastLine = false;
        private bool disableFindOptionSync = false;
        private bool disableSimpleQuickFind = false;
        private bool disableTabDifferentiation = false;
        private bool disableReplaceFilesConfirm = false;
        private bool autoReloadModifiedFiles = false;
        private bool saveUnicodeWithBOM = false;
        private bool disableFindTextUpdating = false;
        private bool redirectFilesResults = true;
        private bool applyFileExtension = true;
        private bool restoreFileStates = true;
        private bool restoreFileSession = true;
        private bool backSpaceUnIndents = false;
        private bool braceMatchingEnabled = true;
        private bool caretLineVisible = false;
        private bool ensureConsistentLineEnds = true;
        private bool ensureLastLineEnd = false;
        private bool foldAtElse = false;
        private bool foldComment = true;
        private bool foldCompact = false;
        private bool foldHtml = true;
        private bool foldPreprocessor = true;
        private bool highlightGuide = true;
        private bool lineCommentsAfterIndent = true;
        private bool moveCursorAfterComment = false;
        private bool stripTrailingSpaces = false;
        private bool sequentialTabbing = false;
        private bool tabIndents = true;
        private bool useFolding = true;
        private bool useTabs = true;
        private bool viewEOL = false;
        private bool viewBookmarks = true;
        private bool viewLineNumbers = true;
        private bool viewIndentationGuides = true;
        private bool viewShortcuts = true;
        private bool viewToolBar = true;
        private bool viewStatusBar = true;
        private bool viewWhitespace = false;
        private bool viewModifiedLines = false;
        private bool wrapText = false;
        private FormWindowState windowState = FormWindowState.Maximized;
        private Point windowPosition = new Point(Screen.PrimaryScreen.WorkingArea.Left + 100, Screen.PrimaryScreen.WorkingArea.Top + 70);
        private Size windowSize = new Size(Screen.PrimaryScreen.WorkingArea.Right - 200, Screen.PrimaryScreen.WorkingArea.Bottom - 140);
        private UpdateInterval checkForUpdates = UpdateInterval.Monthly;
        private int uiHoverDelay = 500;
        private int uiDisplayDelay = 100;
        private int uiMaxTraceLines = 1000;
        private long lastUpdateCheck = 0;
        private bool uiShowDetails = false;
        private bool uiAutoFilterList = true;
        private bool uiEnableAutoHide = true;
        private bool uiWrapList = false;
        private bool uiDisableSmartMatch = false;
        private string uiInsertionTriggers = "";
        private int clipboardHistorySize = 50;
    }

}
