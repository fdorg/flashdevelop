using System;
using System.Drawing;
using System.Resources;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using ScintillaNet.Configuration;
using WeifenLuo.WinFormsUI;
using WeifenLuo.WinFormsUI.Docking;
using PluginCore.Localization;
using PluginCore.Managers;
using ScintillaNet;

namespace PluginCore
{
    public interface IPlugin : IEventHandler
    {
	    #region IPlugin Methods
		
	    void Dispose();
	    void Initialize();
		
	    #endregion
		
	    #region IPlugin Properties

        Int32 Api { get; }
        String Name { get; }
        String Guid { get; }
        String Help { get; }
        String Author { get; }
        String Description { get; }
        Object Settings { get; }

        // List of valid API levels:
        // FlashDevelop 4.0 = 1
		
	    #endregion
    }

    public interface IEventHandler
	{
        #region IEventHandler Methods

        void HandleEvent(Object sender, NotifyEvent e, HandlingPriority priority);

        #endregion
	}

    public interface ITabbedDocument
    {
        #region ITabbedDocument Properties

        Icon Icon { get; set; }
        String FileName { get; }
        String Text { get; set; }
        Boolean UseCustomIcon { get; set; }
        Control.ControlCollection Controls { get; }
        SplitContainer SplitContainer { get; }
        ScintillaControl SciControl { get; }
        ScintillaControl SplitSci1 { get; }
        ScintillaControl SplitSci2 { get; }
        Boolean IsModified { get; set; }
        Boolean IsSplitted { get; set; }
        Boolean IsBrowsable { get; }
        Boolean IsUntitled { get; }
        Boolean IsEditable { get; }
        Boolean HasBookmarks { get; }

        #endregion

        #region ITabbedDocument Methods

        void Close();
        void Activate();
        void RefreshTexts();
        void Reload(Boolean showQuestion);
        void Revert(Boolean showQuestion);
        void Save(String file);
        void Save();

        #endregion
    }

    public interface ICompletionListItem
    {
        #region ICompletionListItem Properties

        String Label { get; }
        String Value { get; }
        String Description { get; }
        Bitmap Icon { get; }

        #endregion
    }

    public interface IMainForm : IWin32Window
    {
        #region IMainForm Methods

        void RefreshUI();
        void KillProcess();
        void RefreshSciConfig();
        void ThemeControls(Object control);
        void ClearTemporaryFiles(String file);
        void ShowSettingsDialog(String itemName);
        void ShowErrorDialog(Object sender, Exception ex);
        void ShowSettingsDialog(String itemName, String filter);
        void AutoUpdateMenuItem(ToolStripItem item, String action);
        void RegisterShortcutItem(String id, Keys keys);
        void RegisterShortcutItem(String id, ToolStripMenuItem item);
        void RegisterSecondaryItem(String id, ToolStripItem item);
        void ApplySecondaryShortcut(ToolStripItem item);
        void FileFromTemplate(String templatePath, String newFilePath);
        DockContent OpenEditableDocument(String file, Boolean restoreFileState);
        DockContent OpenEditableDocument(String file);
        DockContent CreateCustomDocument(Control ctrl);
        DockContent CreateEditableDocument(String file, String text, Int32 codepage);
		DockContent CreateDockablePanel(Control form, String guid, Image image, DockState defaultDockState);
        Boolean CallCommand(String command, String arguments);
        List<ToolStripItem> FindMenuItems(String name);
        ToolStripItem FindMenuItem(String name);
        String ProcessArgString(String args);
        Keys GetShortcutItemKeys(String id);
        String GetThemeValue(String id);
        Color GetThemeColor(String id);
        IPlugin FindPlugin(String guid);
        Image ImageSetAdjust(Image image);
        Image FindImage(String data);

        #endregion

        #region IMainFrom Properties

        ISettings Settings { get; }
        ToolStrip ToolStrip { get; }
        MenuStrip MenuStrip { get; }
        Scintilla SciConfig { get; }
        DockPanel DockPanel { get; }
        String[] StartArguments { get; }
        List<Argument> CustomArguments { get; }
        StatusStrip StatusStrip { get; }
        String WorkingDirectory { get; set; }
        ToolStripPanel ToolStripPanel { get; }
        ToolStripStatusLabel StatusLabel { get; }
        ToolStripStatusLabel ProgressLabel { get; }
        ToolStripProgressBar ProgressBar { get; }
        Control.ControlCollection Controls { get; }
        ContextMenuStrip TabMenu { get; }
        ContextMenuStrip EditorMenu { get; }
        ITabbedDocument CurrentDocument { get; }
        ITabbedDocument[] Documents { get; }
        Boolean HasModifiedDocuments { get; }
        Boolean ClosingEntirely { get; }
        Boolean ProcessIsRunning { get; }
        Boolean ReloadingDocument { get; }
        Boolean ProcessingContents { get; }
        Boolean RestoringContents { get; }
        Boolean SavingMultiple { get; }
        Boolean PanelIsActive { get; }
        Boolean IsFullScreen { get; }
        Boolean StandaloneMode { get; }
        Boolean MultiInstanceMode { get; }
        Boolean IsFirstInstance { get; }
        Boolean RestartRequested { get; }
        Boolean RefreshConfig { get; }
        List<Keys> IgnoredKeys { get; }
        String ProductVersion { get; }
        String ProductName { get; }

        #endregion
    }

    public interface IProject
    {
        #region IProject Methods

        String[] GetHiddenPaths();
        String GetRelativePath(String path);
        String GetAbsolutePath(String path);

        /// <summary>
        /// When in Release configuration, remove 'debug' from the given path.
        /// Pattern: ([a-zA-Z0-9])[-_.]debug([\\/.])
        /// </summary>
        String FixDebugReleasePath(String path);

        #endregion

        #region IProject Properties

        String Name { get; }
        String Language { get; }
        String OutputPathAbsolute { get; }
        String[] SourcePaths { get; }
        Boolean TraceEnabled { get; }
        Boolean EnableInteractiveDebugger { get; }
        String ProjectPath { get; }
        String PreferredSDK { get; }
        String CurrentSDK { get; }
        String DefaultSearchFilter { get; }

        #endregion
    }

    public interface ISettings
    {
        #region ISettings Properties

        Font DefaultFont { get; set; }
        Font ConsoleFont { get; set; }
        List<String> DisabledPlugins { get; set; }
        List<String> PreviousDocuments { get; set; }
        LocaleVersion LocaleVersion { get; set; }
        UiRenderMode RenderMode { get; set; }
        CodingStyle CodingStyle { get; set; }
        CommentBlockStyle CommentBlockStyle { get; set; }
        FlatStyle ComboBoxFlatStyle { get; set; }
        String DefaultFileExtension { get; set; }
        String LatestDialogPath { get; set; }
        Boolean ConfirmOnExit { get; set; }
        String CustomSnippetDir { get; set; }
        String CustomTemplateDir { get; set; }
        String CustomProjectsDir { get; set; }
        Boolean DisableFindOptionSync { get; set; }
        Boolean DisableSimpleQuickFind { get; set; }
        Boolean DisableReplaceFilesConfirm { get; set; }
        Boolean AutoReloadModifiedFiles { get; set; }
        Boolean UseListViewGrouping { get; set; }
        Boolean RedirectFilesResults { get; set; }
        Boolean DisableFindTextUpdating { get; set; }
        Boolean ApplyFileExtension { get; set; }
        Boolean RestoreFileStates { get; set; }
        Boolean BackSpaceUnIndents { get; set; }
        Boolean BraceMatchingEnabled { get; set; }
        Boolean CaretLineVisible { get; set; }
        Boolean EnsureConsistentLineEnds { get; set; }
        Boolean EnsureLastLineEnd { get; set; }
        Boolean UseSystemColors { get; set; }
        Boolean FoldAtElse { get; set; }
        Boolean FoldComment { get; set; }
        Boolean FoldCompact { get; set; }
        Boolean FoldHtml { get; set; }
        Boolean FoldPreprocessor { get; set; }
        Boolean HighlightGuide { get; set; }
        Boolean LineCommentsAfterIndent { get; set; }
        Boolean MoveCursorAfterComment { get; set; }
        Boolean StripTrailingSpaces { get; set; }
        Boolean SequentialTabbing { get; set; }
        Boolean TabIndents { get; set; }
        Boolean UseFolding { get; set; }
        Boolean UseTabs { get; set; }
        Boolean ViewEOL { get; set; }
        Boolean ViewBookmarks { get; set; }
        Boolean ViewLineNumbers { get; set; }
        Boolean ViewIndentationGuides { get; set; }
        Boolean ViewModifiedLines { get; set; }
        Boolean ViewToolBar { get; set; }
        Boolean ViewStatusBar { get; set; }
        Boolean ViewWhitespace { get; set; }
        Boolean ViewShortcuts { get; set; }
        Boolean WrapText { get; set; }
        ScintillaNet.Enums.EndOfLine EOLMode { get; set; }
        ScintillaNet.Enums.FoldFlag FoldFlags { get; set; }
        ScintillaNet.Enums.SmartIndent SmartIndentType { get; set; }
        ScintillaNet.Enums.VirtualSpaceMode VirtualSpaceMode { get; set; }
        ScintillaNet.Enums.IndentView IndentView { get; set; }
        ScintillaNet.Enums.HighlightMatchingWordsMode HighlightMatchingWordsMode { get; set; }
        CodePage DefaultCodePage { get; set; }
        Int32 TabWidth { get; set; }
        Int32 IndentSize { get; set; }
        Int32 CaretPeriod { get; set; }
        Int32 CaretWidth { get; set; }
        Int32 ScrollWidth { get; set; }
        Int32 PrintMarginColumn  { get; set; }
        Size WindowSize { get; set; }
        FormWindowState WindowState { get; set; }
        Point WindowPosition { get; set; }
        Int32 HoverDelay { get; set; }
        Int32 DisplayDelay { get; set; }
        Boolean ShowDetails { get; set; }
        Boolean AutoFilterList { get; set; }
        Boolean EnableAutoHide { get; set; }
        Boolean WrapList { get; set; }
        Boolean DisableSmartMatch { get; set; }
        Boolean SaveUnicodeWithBOM { get; set; }
        String InsertionTriggers { get; set; }
        
        #endregion
    }

    public interface ISession
    {
        #region ISession Properties

        Int32 Index { get; set; }
        List<String> Files { get; set; }
        SessionType Type { get; set; }

        #endregion
    }

}