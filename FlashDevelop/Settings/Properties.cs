using System;
using System.Text;
using System.Drawing;
using System.Reflection;
using System.Collections;
using System.ComponentModel;
using System.Collections.Generic;
using PluginCore.Localization;
using System.Windows.Forms;
using PluginCore.Managers;
using PluginCore;

namespace FlashDevelop.Settings
{
    [Serializable]
    [DefaultProperty("AutoFilterList")]
    public partial class SettingObject : ISettings
    {
        private CodePage defaultCodePage = CodePage.UTF8;
        private ScintillaNet.Enums.EndOfLine eolMode = ScintillaNet.Enums.EndOfLine.CRLF;
        private ScintillaNet.Enums.IndentView indentView = ScintillaNet.Enums.IndentView.Real;
        private ScintillaNet.Enums.FoldFlag foldFlags = ScintillaNet.Enums.FoldFlag.LineAfterContracted;
        private ScintillaNet.Enums.SmartIndent smartIndentType = ScintillaNet.Enums.SmartIndent.CPP;
        private ScintillaNet.Enums.VirtualSpaceMode virtualSpaceMode = ScintillaNet.Enums.VirtualSpaceMode.RectangularSelection;
        private UiRenderMode uiRenderMode = UiRenderMode.Professional;
        private CodingStyle codingStyle = CodingStyle.BracesAfterLine;
        private CommentBlockStyle commentBlockStyle = CommentBlockStyle.Indented;
        private FlatStyle comboBoxFlatStyle = FlatStyle.Popup;
        private Font consoleFont = new Font("Courier New", 8.75F);
        private Font defaultFont = new Font("Tahoma", 8.25F);
        private String customProjectsDir = String.Empty;
        private String customTemplateDir = String.Empty;
        private String customSnippetDir = String.Empty;
        private Int32 tabWidth = 4;
        private Int32 indentSize = 4;
        private Int32 caretPeriod = 500;
        private Int32 caretWidth = 2;
        private Int32 scrollWidth = 3000;
        private Int32 latestCommand = 0;
        private Int32 printMarginColumn = 0;
        private Int32 backupInterval = 15000;
        private Int32 filePollInterval = 3000;
        private ScintillaNet.Enums.HighlightMatchingWordsMode highlightMatchingWordsMode = ScintillaNet.Enums.HighlightMatchingWordsMode.SelectionOrPosition;
        private LocaleVersion localeVersion = LocaleVersion.en_US;
        private List<String> previousDocuments = new List<String>();
        private List<String> disabledPlugins = new List<String>();
        private String latestDialogPath = Application.StartupPath;
        private String defaultFileExtension = "as";
        private Boolean confirmOnExit = false;
        private Boolean keepIndentTabs = false;
        private Boolean useSystemColors = false;
        private Boolean disableFindOptionSync = false;
        private Boolean disableSimpleQuickFind = false;
        private Boolean disableTabDifferentiation = false;
        private Boolean disableReplaceFilesConfirm = false;
        private Boolean autoReloadModifiedFiles = false;
        private Boolean saveUnicodeWithBOM = false;
        private Boolean disableFindTextUpdating = false;
        private Boolean redirectFilesResults = true;
        private Boolean useListViewGrouping = true;
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
        private Point windowPosition = new Point(
            Screen.PrimaryScreen.WorkingArea.Left + 100,
            Screen.PrimaryScreen.WorkingArea.Top + 70);
        private Size windowSize = new Size(
            Screen.PrimaryScreen.WorkingArea.Right - 200,
            Screen.PrimaryScreen.WorkingArea.Bottom - 140);
        private Int32 uiHoverDelay = 500;
        private Int32 uiDisplayDelay = 100;
        private Boolean uiShowDetails = false;
        private Boolean uiAutoFilterList = true;
        private Boolean uiEnableAutoHide = true;
        private Boolean uiWrapList = false;
        private Boolean uiDisableSmartMatch = false;
        private String uiInsertionTriggers = "";

    }

}
