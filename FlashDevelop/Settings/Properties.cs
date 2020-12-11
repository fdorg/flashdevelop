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
        CodePage defaultCodePage = CodePage.UTF8;
        EndOfLine eolMode = EndOfLine.CRLF;
        IndentView indentView = IndentView.Real;
        FoldFlag foldFlags = FoldFlag.LineAfterContracted;
        SmartIndent smartIndentType = SmartIndent.CPP;
        VirtualSpaceMode virtualSpaceMode = VirtualSpaceMode.RectangularSelection;
        CommentBlockStyle commentBlockStyle = CommentBlockStyle.Indented;
        CodingStyle codingStyle = CodingStyle.BracesAfterLine;
        string customProjectsDir = string.Empty;
        string customTemplateDir = string.Empty;
        string customSnippetDir = string.Empty;
        string customCommandPrompt = string.Empty;
        Font consoleFont = new Font("Courier New", 8.75F);
        Font defaultFont = new Font("Tahoma", 8.25F);
        int tabWidth = 4;
        int indentSize = 4;
        int caretPeriod = 500;
        int caretWidth = 2;
        int scrollWidth = 3000;
        int latestCommand = 0;
        int printMarginColumn = 0;
        int backupInterval = 15000;
        int filePollInterval = 3000;
        int maxRecentFiles = 15;
        int highlightMatchingWordsDelay = 1200;
        HighlightMatchingWordsMode highlightMatchingWordsMode = HighlightMatchingWordsMode.SelectionOrPosition;
        LocaleVersion localeVersion = LocaleVersion.en_US;
        List<string> previousDocuments = new List<string>();
        List<string> disabledPlugins = new List<string>();
        string latestDialogPath = Application.StartupPath;
        string defaultFileExtension = DistroConfig.DISTRIBUTION_EXT;
        bool confirmOnExit = false;
        bool keepIndentTabs = false;
        bool keepCaretCentered = false;
        bool endAtLastLine = false;
        bool disableFindOptionSync = false;
        bool disableSimpleQuickFind = false;
        bool disableTabDifferentiation = false;
        bool disableReplaceFilesConfirm = false;
        bool autoReloadModifiedFiles = false;
        bool saveUnicodeWithBOM = false;
        bool disableFindTextUpdating = false;
        bool redirectFilesResults = true;
        bool applyFileExtension = true;
        bool restoreFileStates = true;
        bool restoreFileSession = true;
        bool backSpaceUnIndents = false;
        bool braceMatchingEnabled = true;
        bool caretLineVisible = false;
        bool ensureConsistentLineEnds = true;
        bool ensureLastLineEnd = false;
        bool foldAtElse = false;
        bool foldComment = true;
        bool foldCompact = false;
        bool foldHtml = true;
        bool foldPreprocessor = true;
        bool highlightGuide = true;
        bool lineCommentsAfterIndent = true;
        bool moveCursorAfterComment = false;
        bool stripTrailingSpaces = false;
        bool sequentialTabbing = false;
        bool tabIndents = true;
        bool useFolding = true;
        bool useTabs = true;
        bool viewEOL = false;
        bool viewBookmarks = true;
        bool viewLineNumbers = true;
        bool viewIndentationGuides = true;
        bool viewShortcuts = true;
        bool viewToolBar = true;
        bool viewStatusBar = true;
        bool viewWhitespace = false;
        bool viewModifiedLines = false;
        bool wrapText = false;
        FormWindowState windowState = FormWindowState.Maximized;
        Point windowPosition = new Point(Screen.PrimaryScreen.WorkingArea.Left + 100, Screen.PrimaryScreen.WorkingArea.Top + 70);
        Size windowSize = new Size(Screen.PrimaryScreen.WorkingArea.Right - 200, Screen.PrimaryScreen.WorkingArea.Bottom - 140);
        UpdateInterval checkForUpdates = UpdateInterval.Monthly;
        int uiHoverDelay = 500;
        int uiDisplayDelay = 100;
        int uiMaxTraceLines = 1000;
        long lastUpdateCheck = 0;
        bool uiShowDetails = false;
        bool uiAutoFilterList = true;
        bool uiEnableAutoHide = true;
        bool uiWrapList = false;
        bool uiDisableSmartMatch = false;
        string uiInsertionTriggers = "";
        int clipboardHistorySize = 50;
    }
}