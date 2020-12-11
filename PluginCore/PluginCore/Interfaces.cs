// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using PluginCore.Localization;
using ScintillaNet;
using ScintillaNet.Configuration;
using ScintillaNet.Enums;
using WeifenLuo.WinFormsUI.Docking;

namespace PluginCore
{
    public interface IPlugin : IEventHandler
    {
        #region IPlugin Methods

        void Dispose();
        void Initialize();

        #endregion

        #region IPlugin Properties

        int Api { get; }
        string Name { get; }
        string Guid { get; }
        string Help { get; }
        string Author { get; }
        string Description { get; }
        object Settings { get; }

        // List of valid API levels:
        // FlashDevelop 4.0 = 1

        #endregion
    }

    public interface IEventHandler
    {
        #region IEventHandler Methods

        void HandleEvent(object sender, NotifyEvent e, HandlingPriority priority);

        #endregion
    }

    public interface IThemeHandler
    {
        #region IThemeHandler Methods

        void AfterTheming();

        #endregion
    }

    public interface ITabbedDocument : IDockContent
    {
        #region ITabbedDocument Properties

        Icon Icon { get; set; }
        string FileName { get; }
        string Text { get; set; }
        bool UseCustomIcon { get; set; }
        Control.ControlCollection Controls { get; }
        SplitContainer SplitContainer { get; }
        ScintillaControl? SciControl { get; }
        ScintillaControl SplitSci1 { get; }
        ScintillaControl SplitSci2 { get; }
        bool IsModified { get; set; }
        bool IsSplitted { get; set; }
        bool IsBrowsable { get; }
        bool IsUntitled { get; }
        bool IsEditable { get; }
        bool HasBookmarks { get; }
        bool IsAloneInPane { get; }

        #endregion

        #region ITabbedDocument Methods

        void Close();
        void Activate();
        void RefreshTexts();
        void Reload(bool showQuestion);
        void Revert(bool showQuestion);
        void Save(string file);
        void Save(string file, string reason);
        void Save();

        #endregion
    }

    public interface ICompletionListItem
    {
        #region ICompletionListItem Properties

        string Label { get; }
        string? Value { get; }
        string Description { get; }
        Bitmap Icon { get; }

        #endregion
    }

    public interface ICompletionListSpecialItem : ICompletionListItem
    {
        #region ICompletionListSpecialItem Properties

        #endregion
    }

    public interface IMainForm : IContainerControl, IWin32Window
    {
        #region IMainForm Methods

        /// <summary>
        /// Refreshes the main form.
        /// </summary>
        void RefreshUI();
        /// <summary>
        /// Stop the currently running process.
        /// </summary>
        void KillProcess();
        /// <summary>
        /// Refreshes the scintilla configuration.
        /// </summary>
        void RefreshSciConfig();
        /// <summary>
        /// Shows a message to restart FD.
        /// </summary>
        void RestartRequired();
        /// <summary>
        /// Themes the controls from the parent.
        /// </summary>
        void ThemeControls(object control);
        /// <summary>
        /// Clears the temporary file from disk.
        /// </summary>
        void ClearTemporaryFiles(string file);
        /// <summary>
        /// Shows the settings dialog.
        /// </summary>
        void ShowSettingsDialog(string itemName);
        /// <summary>
        /// Shows the error dialog if the sender is <see cref="Managers.ErrorManager"/>.
        /// </summary>
        void ShowErrorDialog(object sender, Exception ex);
        /// <summary>
        /// Shows the settings dialog with a filter.
        /// </summary>
        void ShowSettingsDialog(string itemName, string filter);
        /// <summary>
        /// Lets you update menu items using the flag functionality.
        /// </summary>
        void AutoUpdateMenuItem(ToolStripItem item, string action);
        /// <summary>
        /// Registers a new menu item with the shortcut manager.
        /// </summary>
        void RegisterShortcutItem(string id, System.Windows.Forms.Keys keys);
        /// <summary>
        /// Registers a new menu item with the shortcut manager.
        /// </summary>
        void RegisterShortcutItem(string id, ToolStripMenuItem item);
        /// <summary>
        /// Registers a new secondary menu item with the shortcut manager.
        /// </summary>
        void RegisterSecondaryItem(string id, ToolStripItem item);
        /// <summary>
        /// Updates a registered secondary menu item in the shortcut manager
        /// - should be called when the tooltip changes.
        /// </summary>
        void ApplySecondaryShortcut(ToolStripItem item);
        /// <summary>
        /// Create the specified new document from the given template.
        /// </summary>
        void FileFromTemplate(string templatePath, string newFilePath);
        /// <summary>
        /// Opens an editable document.
        /// </summary>
        DockContent? OpenEditableDocument(string file, bool restoreFileState);
        /// <summary>
        /// Opens an editable document.
        /// </summary>
        DockContent? OpenEditableDocument(string file);
        /// <summary>
        /// Creates a new custom document.
        /// </summary>
        DockContent CreateCustomDocument(Control ctrl);
        /// <summary>
        /// Creates a new empty document.
        /// </summary>
        DockContent CreateEditableDocument(string file, string text, int codepage);
        /// <summary>
        /// Creates a floating panel for the plugin.
        /// </summary>
        DockContent CreateDockablePanel(Control form, string guid, Image image, DockState defaultDockState);
        /// <summary>
        /// Creates a dynamic persist panel for plugins.
        /// </summary>
        DockContent CreateDynamicPersistDockablePanel(Control ctrl, string guid, string id, Image image, DockState defaultDockState);
        /// <summary>
        /// Calls a normal <see cref="IMainForm"/> method.
        /// </summary>
        bool CallCommand(string command, string arguments);
        /// <summary>
        /// Finds the menu items that have the specified name.
        /// </summary>
        List<ToolStripItem> FindMenuItems(string name);
        /// <summary>
        /// Finds the specified menu item by name.
        /// </summary>
        ToolStripItem FindMenuItem(string name);
        /// <summary>
        /// Processes the argument string variables.
        /// </summary>
        string ProcessArgString(string args);
        /// <summary>
        /// Gets the specified item's shortcut keys.
        /// </summary>
        System.Windows.Forms.Keys GetShortcutItemKeys(string id);
        /// <summary>
        /// Gets the specified item's id.
        /// </summary>
        string GetShortcutItemId(System.Windows.Forms.Keys keys);
        /// <summary>
        /// Gets a theme property value.
        /// </summary>
        string GetThemeValue(string id);
        /// <summary>
        /// Gets a theme property color.
        /// </summary>
        Color GetThemeColor(string id);
        /// <summary>
        /// Gets a theme flag value.
        /// </summary>
        bool GetThemeFlag(string id);
        /// <summary>
        /// Gets a theme flag value with a fallback.
        /// </summary>
        bool GetThemeFlag(string id, bool fallback);
        /// <summary>
        /// Gets a theme property value with a fallback.
        /// </summary>
        string GetThemeValue(string id, string fallback);
        /// <summary>
        /// Gets a theme property color with a fallback.
        /// </summary>
        Color GetThemeColor(string id, Color fallback);
        /// <summary>
        /// Sets if child controls should use theme.
        /// </summary>
        void SetUseTheme(object parent, bool use);
        /// <summary>
        /// Finds the specified plugin.
        /// </summary>
        IPlugin FindPlugin(string guid);
        /// <summary>
        /// Adjusts the image for different themes.
        /// </summary>
        Image ImageSetAdjust(Image image);
        /// <summary>
        /// Gets a copy of the image that gets automatically adjusted according to the theme.
        /// </summary>
        Image GetAutoAdjustedImage(Image image);
        /// <summary>
        /// Finds the specified composed/ready image that is automatically adjusted according to the theme.
        /// <para/>
        /// If you make a copy of the image returned by this method, the copy will not be automatically adjusted.
        /// </summary>
        Image FindImage(string data);
        /// <summary>
        /// Finds the specified composed/ready image.
        /// <para/>
        /// If you make a copy of the image returned by this method, the copy will not be automatically adjusted, even if <code>autoAdjusted</code> is <code>true</code>.
        /// </summary>
        Image FindImage(string data, bool autoAdjust);
        /// <summary>
        /// Finds the specified composed/ready image that is automatically adjusted according to the theme.
        /// The image size is always 16x16.
        /// <para/>
        /// If you make a copy of the image returned by this method, the copy will not be automatically adjusted.
        /// </summary>
        Image FindImage16(string data);
        /// <summary>
        /// Finds the specified composed/ready image. The image size is always 16x16.
        /// <para/>
        /// If you make a copy of the image returned by this method, the copy will not be automatically adjusted, even if <code>autoAdjusted</code> is <code>true</code>.
        /// </summary>
        Image FindImage16(string data, bool autoAdjusted);
        /// <summary>
        /// Finds the specified composed/ready image and returns a copy of the image that has its color adjusted.
        /// This method is typically used for populating a <see cref="ImageList"/> object.
        /// <para/>
        /// Equivalent to calling <code>ImageSetAdjust(FindImage(data, false))</code>.
        /// </summary>
        Image FindImageAndSetAdjust(string data);
        /// <summary>
        /// Gets the amount of FD instances running
        /// </summary>
        int GetInstanceCount();

        #endregion

        #region IMainFrom Properties

        /// <summary>
        /// Gets the <see cref="ISettings"/> interface.
        /// </summary>
        ISettings Settings { get; }
        /// <summary>
        /// Gets the tool strip.
        /// </summary>
        ToolStrip ToolStrip { get; }
        /// <summary>
        /// Gets the menu strip.
        /// </summary>
        MenuStrip MenuStrip { get; }
        /// <summary>
        /// Gets the <see cref="Scintilla"/> configuration.
        /// </summary>
        Scintilla SciConfig { get; }
        /// <summary>
        /// Gets the dock panel.
        /// </summary>
        DockPanel DockPanel { get; }
        /// <summary>
        /// Gets the application start arguments.
        /// </summary>
        string[] StartArguments { get; }
        /// <summary>
        /// Gets the application custom arguments.
        /// </summary>
        List<Argument> CustomArguments { get; }
        /// <summary>
        /// Gets the status strip.
        /// </summary>
        StatusStrip StatusStrip { get; }
        /// <summary>
        /// Gets or sets the working directory.
        /// </summary>
        string WorkingDirectory { get; set; }
        /// <summary>
        /// Gets the tool strip panel.
        /// </summary>
        ToolStripPanel ToolStripPanel { get; }
        /// <summary>
        /// Gets the tool strip status label.
        /// </summary>
        ToolStripStatusLabel StatusLabel { get; }
        /// <summary>
        /// Gets the tool strip progress label.
        /// </summary>
        ToolStripStatusLabel ProgressLabel { get; }
        /// <summary>
        /// Gets the tool strip progress bar.
        /// </summary>
        ToolStripProgressBar ProgressBar { get; }
        /// <summary>
        /// Gets the collection of controls contained within this control.
        /// </summary>
        Control.ControlCollection Controls { get; }
        /// <summary>
        /// Gets the tab menu.
        /// </summary>
        ContextMenuStrip TabMenu { get; }
        /// <summary>
        /// Gets the editor menu.
        /// </summary>
        ContextMenuStrip EditorMenu { get; }
        /// <summary>
        /// Gets the current <see cref="ITabbedDocument"/> object.
        /// </summary>
        ITabbedDocument? CurrentDocument { get; }
        /// <summary>
        /// Gets all available documents.
        /// </summary>
        ITabbedDocument[] Documents { get; }
        /// <summary>
        /// Gets whether FlashDevelop holds modified documents.
        /// </summary>
        bool HasModifiedDocuments { get; }
        /// <summary>
        /// Gets whether FlashDevelop is closing.
        /// </summary>
        bool ClosingEntirely { get; }
        /// <summary>
        /// Gets whether a process is running.
        /// </summary>
        bool ProcessIsRunning { get; }
        /// <summary>
        /// Gets whether a document is reloading.
        /// </summary>
        bool ReloadingDocument { get; }
        /// <summary>
        /// Gets whether contents are being processed.
        /// </summary>
        bool ProcessingContents { get; }
        /// <summary>
        /// Gets whether contents are being restored.
        /// </summary>
        bool RestoringContents { get; }
        /// <summary>
        /// Gets saving multiple.
        /// </summary>
        bool SavingMultiple { get; }
        /// <summary>
        /// Gets whether the panel is active.
        /// </summary>
        bool PanelIsActive { get; }
        /// <summary>
        /// Gets whether FlashDevelop is in full screen.
        /// </summary>
        bool IsFullScreen { get; }
        /// <summary>
        /// Gets whether FlashDevelop is in standalone mode.
        /// </summary>
        bool StandaloneMode { get; }
        /// <summary>
        /// Gets whether FlashDevelop is in multi-instance mode.
        /// </summary>
        bool MultiInstanceMode { get; }
        /// <summary>
        /// Gets whether this <see cref="IMainForm"/> is the first instance.
        /// </summary>
        bool IsFirstInstance { get; }
        /// <summary>
        /// Gets whether a restart is required.
        /// </summary>
        bool RestartRequested { get; }
        /// <summary>
        /// Gets whether the application requires a restart to apply changes.
        /// </summary>
        bool RequiresRestart { get; }
        /// <summary>
        /// Gets whether the config should be refreshed.
        /// </summary>
        bool RefreshConfig { get; }
        /// <summary>
        /// Gets the ignored keys.
        /// </summary>
        List<System.Windows.Forms.Keys> IgnoredKeys { get; }
        /// <summary>
        /// Gets the version of the application.
        /// </summary>
        string ProductVersion { get; }
        /// <summary>
        /// Gets the full human readable version string.
        /// </summary>
        string ProductName { get; }
        /// <summary>
        /// Gets the command prompt executable (custom or cmd.exe by default).
        /// </summary>
        string CommandPromptExecutable { get; }

        #endregion
    }

    public interface IProject
    {
        #region IProject Methods

        string[] GetHiddenPaths();
        string GetRelativePath(string path);
        string GetAbsolutePath(string path);

        /// <summary>
        /// When in Release configuration, remove 'debug' from the given path.
        /// Pattern: ([a-zA-Z0-9])[-_.]debug([\\/.])
        /// </summary>
        string FixDebugReleasePath(string path);

        #endregion

        #region IProject Properties

        string Name { get; }
        string Language { get; }
        string OutputPathAbsolute { get; }
        string[] SourcePaths { get; }
        bool TraceEnabled { get; }
        bool EnableInteractiveDebugger { get; }
        string ProjectPath { get; }
        string PreferredSDK { get; }
        string CurrentSDK { get; }
        string DefaultSearchFilter { get; }

        #endregion
    }

    public interface ISettings
    {
        #region ISettings Properties

        Font DefaultFont { get; set; }
        Font ConsoleFont { get; set; }
        List<string> DisabledPlugins { get; set; }
        List<string> PreviousDocuments { get; set; }
        LocaleVersion LocaleVersion { get; set; }
        UiRenderMode RenderMode { get; set; }
        CodingStyle CodingStyle { get; set; }
        CommentBlockStyle CommentBlockStyle { get; set; }
        FlatStyle ComboBoxFlatStyle { get; set; }
        string DefaultFileExtension { get; set; }
        string LatestDialogPath { get; set; }
        bool ConfirmOnExit { get; set; }
        string CustomSnippetDir { get; set; }
        string CustomTemplateDir { get; set; }
        string CustomProjectsDir { get; set; }
        string CustomCommandPrompt { get; set; }
        bool DisableFindOptionSync { get; set; }
        bool DisableSimpleQuickFind { get; set; }
        bool DisableReplaceFilesConfirm { get; set; }
        bool AutoReloadModifiedFiles { get; set; }
        bool UseListViewGrouping { get; set; }
        bool RedirectFilesResults { get; set; }
        bool DisableFindTextUpdating { get; set; }
        bool ApplyFileExtension { get; set; }
        bool RestoreFileStates { get; set; }
        bool RestoreFileSession { get; set; }
        bool BackSpaceUnIndents { get; set; }
        bool BraceMatchingEnabled { get; set; }
        bool CaretLineVisible { get; set; }
        bool EnsureConsistentLineEnds { get; set; }
        bool EnsureLastLineEnd { get; set; }
        bool UseSystemColors { get; set; }
        bool FoldAtElse { get; set; }
        bool FoldComment { get; set; }
        bool FoldCompact { get; set; }
        bool FoldHtml { get; set; }
        bool FoldPreprocessor { get; set; }
        bool HighlightGuide { get; set; }
        bool LineCommentsAfterIndent { get; set; }
        bool MoveCursorAfterComment { get; set; }
        bool StripTrailingSpaces { get; set; }
        bool SequentialTabbing { get; set; }
        bool TabIndents { get; set; }
        bool UseFolding { get; set; }
        bool UseTabs { get; set; }
        bool ViewEOL { get; set; }
        bool ViewBookmarks { get; set; }
        bool ViewLineNumbers { get; set; }
        bool ViewIndentationGuides { get; set; }
        bool ViewModifiedLines { get; set; }
        bool ViewToolBar { get; set; }
        bool ViewStatusBar { get; set; }
        bool ViewWhitespace { get; set; }
        bool ViewShortcuts { get; set; }
        bool WrapText { get; set; }
        EndOfLine EOLMode { get; set; }
        FoldFlag FoldFlags { get; set; }
        SmartIndent SmartIndentType { get; set; }
        VirtualSpaceMode VirtualSpaceMode { get; set; }
        IndentView IndentView { get; set; }
        int HighlightMatchingWordsDelay { get; set; }
        HighlightMatchingWordsMode HighlightMatchingWordsMode { get; set; }
        bool HighlightMatchingWordsCaseSensitive { get; set; }
        CodePage DefaultCodePage { get; set; }
        int TabWidth { get; set; }
        int IndentSize { get; set; }
        int CaretPeriod { get; set; }
        int CaretWidth { get; set; }
        int ScrollWidth { get; set; }
        int PrintMarginColumn { get; set; }
        Size WindowSize { get; set; }
        FormWindowState WindowState { get; set; }
        Point WindowPosition { get; set; }
        int HoverDelay { get; set; }
        int DisplayDelay { get; set; }
        bool ShowDetails { get; set; }
        bool AutoFilterList { get; set; }
        bool EnableAutoHide { get; set; }
        bool WrapList { get; set; }
        bool DisableSmartMatch { get; set; }
        bool SaveUnicodeWithBOM { get; set; }
        bool KeepCaretCentered { get; set; }
        bool EndAtLastLine { get; set; }
        string InsertionTriggers { get; set; }
        int ClipboardHistorySize { get; set; }

        #endregion
    }

    public interface ISession
    {
        #region ISession Properties

        int Index { get; set; }
        List<string> Files { get; set; }
        SessionType Type { get; set; }

        #endregion
    }

}