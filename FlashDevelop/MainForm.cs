
#region Imports

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using CSScriptLibrary;
using FlashDevelop.Controls;
using FlashDevelop.Dialogs;
using FlashDevelop.Docking;
using FlashDevelop.Helpers;
using FlashDevelop.Managers;
using FlashDevelop.Settings;
using FlashDevelop.Utilities;
using ICSharpCode.SharpZipLib.Zip;
using PluginCore;
using PluginCore.Controls;
using PluginCore.Helpers;
using PluginCore.Localization;
using PluginCore.Managers;
using PluginCore.Utilities;
using ScintillaNet;
using ScintillaNet.Configuration;
using WeifenLuo.WinFormsUI.Docking;

#endregion

namespace FlashDevelop
{
    public class MainForm : Form, IMainForm, IMessageFilter
    {
        #region Constructor

        public MainForm()
        {
            Globals.MainForm = this;
            PluginBase.Initialize(this);
            this.DoubleBuffered = true;
            this.InitializeErrorLog();
            this.InitializeSettings();
            this.InitializeLocalization();
            if (this.InitializeFirstRun() != DialogResult.Abort)
            {
                // Suspend layout!
                this.SuspendLayout();
                this.InitializeConfig();
                this.InitializeRendering();
                this.InitializeComponents();
                this.InitializeProcessRunner();
                this.InitializeSmartDialogs();
                this.InitializeMainForm();
                this.InitializeGraphics();
                Application.AddMessageFilter(this);
            }
            else this.Load += new EventHandler(this.MainFormLoaded);
        }

        /// <summary>
        /// Initializes some extra error logging
        /// </summary>
        private void InitializeErrorLog()
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(this.OnUnhandledException);
        }

        /// <summary>
        /// Handles the catched unhandled exception and logs it
        /// </summary>
        private void OnUnhandledException(Object sender, UnhandledExceptionEventArgs e)
        {
            Exception exception = new Exception(e.ExceptionObject.ToString());
            ErrorManager.AddToLog("Unhandled exception: ", exception);
        }

        /// <summary>
        /// Exit nicely after the form has been loaded
        /// </summary>
        private void MainFormLoaded(Object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

        #region Private Properties

        /* AppMan */
        private FileSystemWatcher amWatcher;

        /* Components */
        private QuickFind quickFind;
        private DockPanel dockPanel;
        private ToolStrip toolStrip;
        private MenuStrip menuStrip;
        private StatusStrip statusStrip;
        private ToolStripPanel toolStripPanel;
        private ToolStripProgressBarEx toolStripProgressBar;
        private ToolStripStatusLabel toolStripProgressLabel;
        private ToolStripStatusLabel toolStripStatusLabel;
        private ToolStripButton restartButton;
        private ProcessRunner processRunner;

        /* Dialogs */
        private PrintDialog printDialog;
        private ColorDialog colorDialog;
        private OpenFileDialog openFileDialog;
        private SaveFileDialog saveFileDialog;
        private PrintPreviewDialog printPreviewDialog;
        private FRInFilesDialog frInFilesDialog;
        private FRInDocDialog frInDocDialog;
        private GoToDialog gotoDialog;
        
        /* Settings */
        private SettingObject appSettings;

        /* Context Menus */
        private ContextMenuStrip tabMenu;
        private ContextMenuStrip editorMenu;

        /* Working Dir */
        private String workingDirectory = String.Empty;

        /* Form State */
        private FormState formState;
        private Hashtable fullScreenDocks;
        private Boolean isFullScreen = false;
        private Boolean panelIsActive = false;
        private Boolean savingMultiple = false;
        private Boolean notifyOpenFile = false;
        private Boolean reloadingDocument = false;
        private Boolean processingContents = false;
        private Boolean restoringContents = false;
        private Boolean closingForOpenFile = false;
        private Boolean closingEntirely = false;
        private Boolean closeAllCanceled = false;
        private Boolean restartRequested = false;
        private Boolean refreshConfig = false;
        private Boolean closingAll = false;
        
        /* Singleton */
        public static Boolean Silent;
        public static Boolean IsFirst;
        public static String[] Arguments;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the DockPanel
        /// </summary> 
        public DockPanel DockPanel
        {
            get { return this.dockPanel; }
        }

        /// <summary>
        /// Gets the Scintilla configuration
        /// </summary>
        public Scintilla SciConfig
        {
            get { return ScintillaManager.SciConfig; }
        }

        /// <summary>
        /// Gets the menu strip
        /// </summary>
        public MenuStrip MenuStrip
        {
            get { return this.menuStrip; }
        }

        /// <summary>
        /// Gets the tool strip
        /// </summary>
        public ToolStrip ToolStrip
        {
            get { return this.toolStrip; }
        }

        /// <summary>
        /// Gets the tool strip panel
        /// </summary>
        public ToolStripPanel ToolStripPanel
        {
            get { return this.toolStripPanel; }
        }

        /// <summary>
        /// Gets the toolStripStatusLabel
        /// </summary>
        public ToolStripStatusLabel StatusLabel
        {
            get { return this.toolStripStatusLabel; }
        }

        /// <summary>
        /// Gets the toolStripProgressLabel
        /// </summary>
        public ToolStripStatusLabel ProgressLabel
        {
            get { return this.toolStripProgressLabel; }
        }

        /// <summary>
        /// Gets the ToolStripProgressBarEx
        /// </summary>
        public ToolStripProgressBar ProgressBar
        {
            get { return this.toolStripProgressBar; }
        }

        /// <summary>
        /// Gets the TabMenu
        /// </summary>
        public ContextMenuStrip TabMenu
        {
            get { return this.tabMenu; }
        }

        /// <summary>
        /// Gets the EditorMenu
        /// </summary>
        public ContextMenuStrip EditorMenu
        {
            get { return this.editorMenu; }
        }

        /// <summary>
        /// Gets the StatusStrip
        /// </summary>
        public StatusStrip StatusStrip
        {
            get { return this.statusStrip; }
        }

        /// <summary>
        /// Gets the IgnoredKeys
        /// </summary>
        public List<Keys> IgnoredKeys
        {
            get { return ShortcutManager.AllShortcuts; }
        }

        /// <summary>
        /// Gets the Settings interface
        /// </summary>
        public ISettings Settings
        {
            get { return this.appSettings; }
        }

        /// <summary>
        /// Gets or sets the actual Settings
        /// </summary>
        public SettingObject AppSettings
        {
            get { return this.appSettings; }
            set { this.appSettings = value; }
        }

        /// <summary>
        /// Gets the CurrentDocument
        /// </summary>
        public ITabbedDocument CurrentDocument
        {
            get { return this.dockPanel.ActiveDocument as ITabbedDocument; }
        }

        /// <summary>
        /// Is FlashDevelop closing?
        /// </summary>
        public Boolean ClosingEntirely
        {
            get { return this.closingEntirely; }
        }

        /// <summary>
        /// Is this first MainForm instance?
        /// </summary>
        public Boolean IsFirstInstance
        {
            get { return IsFirst; }
        }

        /// <summary>
        /// Is FlashDevelop in multi instance mode?
        /// </summary>
        public Boolean MultiInstanceMode
        {
            get { return Program.MultiInstanceMode; }
        }

        /// <summary>
        /// Is FlashDevelop in standalone mode?
        /// </summary>
        public Boolean StandaloneMode
        {
            get 
            {
                String file = Path.Combine(PathHelper.AppDir, ".local");
                return File.Exists(file); 
            }
        }

        /// <summary>
        /// Gets the all available documents
        /// </summary> 
        public ITabbedDocument[] Documents
        {
            get
            {
                List<ITabbedDocument> documents = new List<ITabbedDocument>();
                foreach (DockPane pane in DockPanel.Panes)
                {
                    if (pane.DockState == DockState.Document)
                    {
                        foreach (IDockContent content in pane.Contents)
                        {
                            if (content is TabbedDocument)
                            {
                                documents.Add(content as TabbedDocument);
                            }
                        }
                    }
                }
                return documents.ToArray();
            }
        }

        /// <summary>
        /// Does FlashDevelop hold modified documents?
        /// </summary> 
        public Boolean HasModifiedDocuments
        {
            get
            {
                foreach (ITabbedDocument document in this.Documents)
                {
                    if (document.IsModified) return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Gets or sets the WorkingDirectory
        /// </summary>
        public String WorkingDirectory
        {
            get
            {
                if (!Directory.Exists(this.workingDirectory))
                {
                    this.workingDirectory = GetWorkingDirectory();
                }
                return this.workingDirectory;
            }
            set { this.workingDirectory = value; }
        }

        /// <summary>
        /// Gets or sets the ProcessIsRunning
        /// </summary>
        public Boolean ProcessIsRunning
        {
            get { return this.processRunner.IsRunning; }
        }

        /// <summary>
        /// Gets the panelIsActive
        /// </summary>
        public Boolean PanelIsActive
        {
            get { return this.panelIsActive; }
        }

        /// <summary>
        /// Gets the isFullScreen
        /// </summary>
        public Boolean IsFullScreen
        {
            get { return this.isFullScreen; }
        }

        /// <summary>
        /// Gets or sets the ReloadingDocument
        /// </summary>
        public Boolean ReloadingDocument
        {
            get { return this.reloadingDocument; }
            set { this.reloadingDocument = value; }
        }

        /// <summary>
        /// Gets or sets the CloseAllCanceled
        /// </summary>
        public Boolean CloseAllCanceled
        {
            get { return this.closeAllCanceled; }
            set { this.closeAllCanceled = value; }
        }

        /// <summary>
        /// Gets or sets the ProcessingContents
        /// </summary>
        public Boolean ProcessingContents
        {
            get { return this.processingContents; }
            set { this.processingContents = value; }
        }

        /// <summary>
        /// Gets or sets the RestoringContents
        /// </summary>
        public Boolean RestoringContents
        {
            get { return this.restoringContents; }
            set { this.restoringContents = value; }
        }

        /// <summary>
        /// Gets or sets the SavingMultiple
        /// </summary>
        public Boolean SavingMultiple
        {
            get { return this.savingMultiple; }
            set { this.savingMultiple = value; }
        }

        /// <summary>
        /// Gets or sets the RestartRequested
        /// </summary>
        public Boolean RestartRequested
        {
            get { return this.restartRequested; }
            set { this.restartRequested = value; }
        }

        /// <summary>
        /// Gets whether the application requires a restart to apply changes.
        /// </summary>
        public Boolean RequiresRestart
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the RefreshConfig
        /// </summary>
        public Boolean RefreshConfig
        {
            get { return this.refreshConfig; }
            set { this.refreshConfig = value; }
        }

        /// <summary>
        /// Gets the application start args
        /// </summary>
        public String[] StartArguments
        {
            get { return Arguments; }
        }

        /// <summary>
        /// Gets the application custom args
        /// </summary>
        public List<Argument> CustomArguments
        {
            get { return ArgumentDialog.CustomArguments; }
        }

        /// <summary>
        /// Gets the application's version
        /// </summary>
        public new String ProductVersion
        {
            get { return Application.ProductVersion; }
        }

        /// <summary>
        /// Gets the full human readable version string
        /// </summary>
        public new String ProductName
        {
            get { return Application.ProductName; }
        }

        /// <summary>
        /// Gets the command prompt executable (custom or cmd.exe by default).
        /// </summary>
        public string CommandPromptExecutable
        {
            get
            {
                if (!String.IsNullOrEmpty(Settings.CustomCommandPrompt) && File.Exists(Settings.CustomCommandPrompt))
                    return Settings.CustomCommandPrompt;
                return "cmd.exe";
            }
        }

        /// <summary>
        /// Gets the version of the operating system
        /// </summary>
        public Version OSVersion
        {
            get { return Environment.OSVersion.Version; }
        }

        #endregion

        #region Component Creation
       
        /// <summary>
        /// Creates a new custom document
        /// </summary>
        public DockContent CreateCustomDocument(Control ctrl)
        {
            try
            {
                TabbedDocument tabbedDocument = new TabbedDocument();
                tabbedDocument.Closing += new System.ComponentModel.CancelEventHandler(this.OnDocumentClosing);
                tabbedDocument.Closed += new System.EventHandler(this.OnDocumentClosed);
                tabbedDocument.Text = TextHelper.GetString("Title.CustomDocument");
                tabbedDocument.TabPageContextMenuStrip = this.tabMenu;
                tabbedDocument.Controls.Add(ctrl);
                tabbedDocument.Show();
                return tabbedDocument;
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
                return null;
            }
        }

        /// <summary>
        /// Creates a new empty document
        /// </summary>
        public DockContent CreateEditableDocument(String file, String text, Int32 codepage)
        {
            try
            {
                this.notifyOpenFile = true;
                TabbedDocument tabbedDocument = new TabbedDocument();
                tabbedDocument.Closing += new System.ComponentModel.CancelEventHandler(this.OnDocumentClosing);
                tabbedDocument.Closed += new System.EventHandler(this.OnDocumentClosed);
                tabbedDocument.TabPageContextMenuStrip = this.tabMenu;
                tabbedDocument.ContextMenuStrip = this.editorMenu;
                tabbedDocument.Text = Path.GetFileName(file);
                tabbedDocument.AddEditorControls(file, text, codepage);
                tabbedDocument.Show();
                return tabbedDocument;
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
                return null;
            }
        }

        /// <summary>
        /// Creates a floating panel for the plugin
        /// </summary>
        public DockContent CreateDockablePanel(Control ctrl, String guid, Image image, DockState defaultDockState)
        {
            try
            {
                DockablePanel dockablePanel = new DockablePanel(ctrl, guid);
                dockablePanel.Show();
                dockablePanel.Image = image;
                dockablePanel.DockState = defaultDockState;
                LayoutManager.PluginPanels.Add(dockablePanel);
                return dockablePanel;
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
                return null;
            }
        }

        /// <summary>
        /// Creates a dynamic persist panel for plugins.
        /// </summary>
        public DockContent CreateDynamicPersistDockablePanel(Control ctrl, String guid, String id, Image image, DockState defaultDockState)
        {
            try
            {
                var dockablePanel = new DockablePanel(ctrl, guid + ":" + id);
                dockablePanel.Image = image;
                dockablePanel.DockState = defaultDockState;
                LayoutManager.SetContentLayout(dockablePanel, dockablePanel.GetPersistString());
                LayoutManager.PluginPanels.Add(dockablePanel);
                return dockablePanel;
            }
            catch (Exception e)
            {
                ErrorManager.ShowError(e);
                return null;
            }
        }

        /// <summary>
        /// Opens the specified file and creates an editable document
        /// </summary>
        public DockContent OpenEditableDocument(String org, Encoding encoding, Boolean restorePosition)
        {
            DockContent createdDoc;
            EncodingFileInfo info;
            String file = PathHelper.GetPhysicalPathName(org);
            TextEvent te = new TextEvent(EventType.FileOpening, file);
            EventManager.DispatchEvent(this, te);
            if (te.Handled)
            {
                if (this.Documents.Length == 0)
                {
                    this.SmartNew(null, null);
                    return null;
                }
                else return null;
            }
            else if (file.EndsWithOrdinal(".delete.fdz"))
            {
                this.CallCommand("RemoveZip", file);
                return null;
            }
            else if (file.EndsWithOrdinal(".fdz"))
            {
                this.CallCommand("ExtractZip", file);
                if (file.IndexOf("theme", StringComparison.OrdinalIgnoreCase) != -1)
                {
                    String currentTheme = Path.Combine(PathHelper.ThemesDir, "CURRENT");
                    if (File.Exists(currentTheme))
                    {
                        ThemeManager.LoadTheme(currentTheme);
                        ThemeManager.WalkControls(this);
                        this.RefreshSciConfig();
                    }
                }
                return null;
            }
            try
            {
                foreach (ITabbedDocument doc in this.Documents)
                {
                    if (doc.IsEditable && doc.FileName.ToUpper() == file.ToUpper())
                    {
                        doc.Activate();
                        return doc as DockContent;
                    }
                }
            }
            catch {}
            if (encoding == null)
            {
                info = FileHelper.GetEncodingFileInfo(file);
                if (info.CodePage == -1) return null; // If the file is locked, stop.
            }
            else
            {
                info = FileHelper.GetEncodingFileInfo(file);
                if (info.CodePage == -1) return null; // If the file is locked, stop.
                info.Contents = FileHelper.ReadFile(file, encoding);
                info.CodePage = encoding.CodePage;
            }
            DataEvent de = new DataEvent(EventType.FileDecode, file, null);
            EventManager.DispatchEvent(this, de); // Lets ask if a plugin wants to decode the data..
            if (de.Handled)
            {
                info.Contents = de.Data as String;
                info.CodePage = Encoding.UTF8.CodePage; // assume plugin always return UTF8
            }
            try
            {
                if (this.CurrentDocument != null && this.CurrentDocument.IsUntitled && !this.CurrentDocument.IsModified && this.Documents.Length == 1)
                {
                    this.closingForOpenFile = true;
                    this.CurrentDocument.Close();
                    this.closingForOpenFile = false;
                    createdDoc = this.CreateEditableDocument(file, info.Contents, info.CodePage);
                }
                else createdDoc = this.CreateEditableDocument(file, info.Contents, info.CodePage);
                ButtonManager.AddNewReopenMenuItem(file);
            }
            catch
            {
                createdDoc = this.CreateEditableDocument(file, info.Contents, info.CodePage);
                ButtonManager.AddNewReopenMenuItem(file);
            }
            TabbedDocument document = (TabbedDocument)createdDoc;
            document.SciControl.SaveBOM = info.ContainsBOM;
            document.SciControl.BeginInvoke((MethodInvoker)delegate
            {
                if (this.appSettings.RestoreFileStates)
                {
                    FileStateManager.ApplyFileState(document, restorePosition);
                }
            });
            ButtonManager.UpdateFlaggedButtons();
            return createdDoc;
        }
        public DockContent OpenEditableDocument(String file, Boolean restorePosition)
        {
            return this.OpenEditableDocument(file, null, restorePosition);
        }
        public DockContent OpenEditableDocument(String file)
        {
            return this.OpenEditableDocument(file, null, true);
        }

        #endregion

        #region Construct Components
       
        /// <summary>
        /// Initializes the graphics
        /// </summary>
        private void InitializeGraphics()
        {
            Icon icon = new Icon(ResourceHelper.GetStream("FlashDevelopIcon.ico"));
            this.Icon = this.printPreviewDialog.Icon = icon;
        }

        /// <summary>
        /// Initializes the theme and config detection
        /// </summary>
        private void InitializeConfig()
        {
            try
            {
                // Check for FD update
                String update = Path.Combine(PathHelper.BaseDir, ".update");
                if (File.Exists(update))
                {
                    File.Delete(update);
                    this.refreshConfig = true;
                }
                // Check for appman update
                String appman = Path.Combine(PathHelper.BaseDir, ".appman");
                if (File.Exists(appman))
                {
                    File.Delete(appman);
                    this.refreshConfig = true;
                }
                // Load platform data from user files
                PlatformData.Load(Path.Combine(PathHelper.SettingDir, "Platforms"));
                // Load current theme for applying later
                String currentTheme = Path.Combine(PathHelper.ThemesDir, "CURRENT");
                if (File.Exists(currentTheme)) ThemeManager.LoadTheme(currentTheme);
                // Apply FD dir and appman dir to PATH
                String amPath = Path.Combine(PathHelper.ToolDir, "AppMan");
                String oldPath = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Process);
                String newPath = oldPath + ";" + amPath + ";" + PathHelper.AppDir;
                Environment.SetEnvironmentVariable("PATH", newPath, EnvironmentVariableTarget.Process);
                // Watch for appman update notifications
                this.amWatcher = new FileSystemWatcher(PathHelper.BaseDir, ".appman");
                this.amWatcher.Changed += new FileSystemEventHandler(this.AppManUpdate);
                this.amWatcher.Created += new FileSystemEventHandler(this.AppManUpdate);
                this.amWatcher.IncludeSubdirectories = false;
                this.amWatcher.EnableRaisingEvents = true;
            }
            catch {} // No errors...
        }

        /// <summary>
        /// When AppMan installs something it notifies of changes. Forward notifications.
        /// </summary>
        private void AppManUpdate(Object sender, FileSystemEventArgs e)
        {
            try
            {

                NotifyEvent ne = new NotifyEvent(EventType.AppChanges);
                EventManager.DispatchEvent(this, ne); // Notify plugins...
                String appMan = Path.Combine(PathHelper.BaseDir, ".appman");
                String contents = File.ReadAllText(appMan);
                if (contents == "restart")
                {
                    this.RestartRequired();
                }
            }
            catch {} // No errors...
        }

        /// <summary>
        /// Initializes the restart button
        /// </summary>
        private void InitializeRestartButton()
        {
            this.restartButton = new ToolStripButton();
            this.restartButton.Image = this.FindImage("73|6|3|3");
            this.restartButton.Alignment = ToolStripItemAlignment.Right;
            this.restartButton.Text = TextHelper.GetString("Label.Restart");
            this.restartButton.ToolTipText = TextHelper.GetString("Info.RequiresRestart");
            this.restartButton.Click += delegate { this.Restart(null, null); };
            this.restartButton.Visible = false;
            this.toolStrip.Items.Add(this.restartButton);
        }

        /// <summary>
        /// Initializes the smart dialogs
        /// </summary>
        public void InitializeSmartDialogs()
        {
            this.formState = new FormState();
            this.gotoDialog = new GoToDialog();
            this.frInFilesDialog = new FRInFilesDialog();
            this.frInDocDialog = new FRInDocDialog();
        }

        /// <summary>
        /// Initializes the First Run dialog
        /// </summary>
        private DialogResult InitializeFirstRun()
        {
            if (!this.StandaloneMode && IsFirst && FirstRunDialog.ShouldProcessCommands())
            {
                return FirstRunDialog.Show();
            }
            return DialogResult.None;
        }

        /// <summary>
        /// Initializes the UI rendering
        /// </summary>
        private void InitializeRendering()
        {
            if (Globals.Settings.RenderMode == UiRenderMode.System)
            {
                ToolStripManager.VisualStylesEnabled = true;
                ToolStripManager.RenderMode = ToolStripManagerRenderMode.System;
            }
            else if (Globals.Settings.RenderMode == UiRenderMode.Professional)
            {
                ToolStripManager.VisualStylesEnabled = false;
                ToolStripManager.RenderMode = ToolStripManagerRenderMode.Professional;
            }
        }

        /// <summary>
        /// Initializes the application settings
        /// </summary>
        private void InitializeSettings()
        {
            this.appSettings = SettingObject.GetDefaultSettings();
            if (File.Exists(FileNameHelper.SettingData))
            {
                Object obj = ObjectSerializer.Deserialize(FileNameHelper.SettingData, this.appSettings, false);
                this.appSettings = (SettingObject)obj;
            }
            SettingObject.EnsureValidity(this.appSettings);
            FileStateManager.RemoveOldStateFiles();
        }

        /// <summary>
        /// Initializes the localization from .locale file
        /// </summary>
        private void InitializeLocalization()
        {
            try
            {
                String filePath = Path.Combine(PathHelper.BaseDir, ".locale");
                if (File.Exists(filePath))
                {
                    String enumData = File.ReadAllText(filePath).Trim();
                    LocaleVersion localeVersion = (LocaleVersion)Enum.Parse(typeof(LocaleVersion), enumData);
                    this.appSettings.LocaleVersion = localeVersion;
                    File.Delete(filePath);
                }
            }
            catch {} // No errors...
        }

        /// <summary>
        /// Initializes the process runner
        /// </summary>
        public void InitializeProcessRunner()
        {
            this.processRunner = new ProcessRunner();
            this.processRunner.RedirectInput = true;
            this.processRunner.ProcessEnded += ProcessEnded;
            this.processRunner.Output += ProcessOutput;
            this.processRunner.Error += ProcessError;
        }

        /// <summary>
        /// Checks for updates in specified schedule
        /// </summary>
        public void CheckForUpdates()
        {
            try
            {
                DateTime last = new DateTime(this.appSettings.LastUpdateCheck);
                TimeSpan elapsed = DateTime.UtcNow.Subtract(last);
                switch (this.appSettings.CheckForUpdates)
                {
                    case UpdateInterval.Weekly:
                    {
                        if (elapsed.TotalDays >= 7)
                        {
                            this.appSettings.LastUpdateCheck = DateTime.UtcNow.Ticks;
                            UpdateDialog.Show(true);
                        }
                        break;
                    }
                    case UpdateInterval.Monthly:
                    {
                        if (elapsed.TotalDays >= 30)
                        {
                            this.appSettings.LastUpdateCheck = DateTime.UtcNow.Ticks;
                            UpdateDialog.Show(true);
                        }
                        break;
                    }
                    default: break;
                }
            }
            catch { /* NO ERRORS PLEASE */ }
        }

        /// <summary>
        /// Initializes the window position and size
        /// </summary>
        public void InitializeWindow()
        {
            this.WindowState = this.appSettings.WindowState;
            Rectangle bounds = new Rectangle(this.appSettings.WindowPosition, this.appSettings.WindowSize);
            bounds.Inflate(-4, -25);
            Boolean validPosition = false;
            foreach (Screen screen in Screen.AllScreens)
            {
                if (screen.Bounds.IntersectsWith(bounds))
                {
                    this.Location = this.appSettings.WindowPosition;
                    validPosition = true;
                    break;
                }
            }
            if (!validPosition) this.Location = new Point(0, 0);
            // Continue/perform layout!
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        /// <summary>
        /// Initialises the plugins, restores the layout and sets an fixed position
        /// </summary>
        public void InitializeMainForm()
        {
            try
            {
                String pluginDir = PathHelper.PluginDir; // Plugins of all users
                if (Directory.Exists(pluginDir)) PluginServices.FindPlugins(pluginDir);
                if (!this.StandaloneMode) // No user plugins on standalone...
                {
                    String userPluginDir = PathHelper.UserPluginDir;
                    if (Directory.Exists(userPluginDir)) PluginServices.FindPlugins(userPluginDir);
                    else Directory.CreateDirectory(userPluginDir);
                }
                LayoutManager.BuildLayoutSystems(FileNameHelper.LayoutData);
                ShortcutManager.LoadCustomShortcuts();
                ArgumentDialog.LoadCustomArguments();
                ClipboardManager.Initialize(this);
                PluginCore.Controls.UITools.Init();
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Initializes the form components
        /// </summary>
        private void InitializeComponents()
        {
            this.quickFind = new QuickFind();
            this.dockPanel = new DockPanel();
            this.statusStrip = new StatusStrip();
            this.toolStripPanel = new ToolStripPanel();
            this.menuStrip = StripBarManager.GetMenuStrip(FileNameHelper.MainMenu);
            this.toolStrip = StripBarManager.GetToolStrip(FileNameHelper.ToolBar);
            this.editorMenu = StripBarManager.GetContextMenu(FileNameHelper.ScintillaMenu);
            this.tabMenu = StripBarManager.GetContextMenu(FileNameHelper.TabMenu);
            this.toolStripStatusLabel = new ToolStripStatusLabel();
            this.toolStripProgressLabel = new ToolStripStatusLabel();
            this.toolStripProgressBar = new ToolStripProgressBarEx();
            this.printPreviewDialog = new PrintPreviewDialog();
            this.saveFileDialog = new SaveFileDialog();
            this.openFileDialog = new OpenFileDialog();
            this.colorDialog = new ColorDialog();
            this.printDialog = new PrintDialog();
            //
            // toolStripPanel
            //
            this.toolStripPanel.Dock = DockStyle.Top;
            if (PlatformHelper.IsRunningOnMono())
            {
                this.toolStripPanel.Controls.Add(this.menuStrip);
                this.toolStripPanel.Controls.Add(this.toolStrip);
            }
            else 
            {
                this.toolStripPanel.Controls.Add(this.toolStrip);
                this.toolStripPanel.Controls.Add(this.menuStrip);
            }
            this.tabMenu.Font = Globals.Settings.DefaultFont;
            this.toolStrip.Font = Globals.Settings.DefaultFont;
            this.menuStrip.Font = Globals.Settings.DefaultFont;
            this.editorMenu.Font = Globals.Settings.DefaultFont;
            this.tabMenu.Renderer = new DockPanelStripRenderer(false);
            this.editorMenu.Renderer = new DockPanelStripRenderer(false);
            this.menuStrip.Renderer = new DockPanelStripRenderer(false);
            this.toolStrip.Renderer = new DockPanelStripRenderer(false);
            this.toolStrip.Padding = new Padding(0, 1, 0, 0);
            this.toolStrip.Size = new Size(500, 26);
            this.toolStrip.Stretch = true;
            // 
            // openFileDialog
            //
            this.openFileDialog.Title = " " + TextHelper.GetString("Title.OpenFileDialog");
            this.openFileDialog.Filter = TextHelper.GetString("Info.FileDialogFilter") + "|*.*";
            this.openFileDialog.RestoreDirectory = true;
            //
            // colorDialog
            //
            this.colorDialog.FullOpen = true;
            this.colorDialog.ShowHelp = false;
            // 
            // printPreviewDialog
            //
            this.printPreviewDialog.Enabled = true;
            this.printPreviewDialog.Name = "printPreviewDialog";
            this.printPreviewDialog.StartPosition = FormStartPosition.CenterParent;
            this.printPreviewDialog.TransparencyKey = Color.Empty;
            this.printPreviewDialog.Visible = false;
            // 
            // saveFileDialog
            //
            this.saveFileDialog.Title = " " + TextHelper.GetString("Title.SaveFileDialog");
            this.saveFileDialog.Filter = TextHelper.GetString("Info.FileDialogFilter") + "|*.*";
            this.saveFileDialog.RestoreDirectory = true;
            // 
            // dockPanel
            //
            this.dockPanel.TabIndex = 2;
            this.dockPanel.DocumentStyle = DocumentStyle.DockingWindow;
            this.dockPanel.DockWindows[DockState.Document].Controls.Add(this.quickFind);
            this.dockPanel.Dock = DockStyle.Fill;
            this.dockPanel.Name = "dockPanel";
            //
            // toolStripStatusLabel
            //
            this.toolStripStatusLabel.Name = "toolStripStatusLabel";
            this.toolStripStatusLabel.TextAlign = ContentAlignment.MiddleLeft;
            this.toolStripStatusLabel.Spring = true;
            //
            // toolStripProgressLabel
            //
            this.toolStripProgressLabel.AutoSize = true;
            this.toolStripProgressLabel.Name = "toolStripProgressLabel";
            this.toolStripProgressLabel.TextAlign = ContentAlignment.MiddleRight;
            this.toolStripProgressLabel.Visible = false;
            //
            // toolStripProgressBar
            //
            this.toolStripProgressBar.Name = "toolStripProgressBar";
            this.toolStripProgressBar.ControlAlign = ContentAlignment.MiddleRight;
            this.toolStripProgressBar.ProgressBar.Width = 120;
            this.toolStripProgressBar.Visible = false;
            // 
            // statusStrip
            //
            this.statusStrip.TabIndex = 3;
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Items.Add(this.toolStripStatusLabel);
            this.statusStrip.Items.Add(this.toolStripProgressLabel);
            this.statusStrip.Items.Add(this.toolStripProgressBar);
            this.statusStrip.Font = Globals.Settings.DefaultFont;
            this.statusStrip.Renderer = new DockPanelStripRenderer(false);
            this.statusStrip.Stretch = true;
            // 
            // MainForm
            //
            this.AllowDrop = true;
            this.Name = "MainForm";
            this.Text = DistroConfig.DISTRIBUTION_NAME;
            this.Controls.Add(this.dockPanel);
            this.Controls.Add(this.toolStripPanel);
            this.Controls.Add(this.statusStrip);
            this.MainMenuStrip = this.menuStrip;
            this.Size = this.appSettings.WindowSize;
            this.Font = this.appSettings.DefaultFont;
            this.StartPosition = FormStartPosition.Manual;
            this.Closing += new CancelEventHandler(this.OnMainFormClosing);
            this.FormClosed += new FormClosedEventHandler(this.OnMainFormClosed);
            this.Activated += new EventHandler(this.OnMainFormActivate);
            this.Shown += new EventHandler(this.OnMainFormShow);
            this.Load += new EventHandler(this.OnMainFormLoad);
            this.LocationChanged += new EventHandler(this.OnMainFormLocationChange);
            this.GotFocus += new EventHandler(this.OnMainFormGotFocus);
            this.Resize += new EventHandler(this.OnMainFormResize);
            ScintillaManager.ConfigurationLoaded += this.ApplyAllSettings;
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Checks the file changes and activates
        /// </summary>
        private void OnMainFormActivate(Object sender, System.EventArgs e)
        {
            if (this.CurrentDocument == null) return;
            this.CurrentDocument.Activate(); // Activate the current document
            ButtonManager.UpdateFlaggedButtons();
        }

        /// <summary>
        /// Checks the file changes when recieving focus
        /// </summary>
        private void OnMainFormGotFocus(Object sender, System.EventArgs e)
        {
            if (this.CurrentDocument == null) return;
            ButtonManager.UpdateFlaggedButtons();
        }

        /// <summary>
        /// Initalizes the windows state after show is called and
        /// check if we need to notify user for recovery files
        /// </summary>
        private void OnMainFormShow(Object sender, System.EventArgs e)
        {
            if (RecoveryDialog.ShouldShowDialog()) RecoveryDialog.Show();
        }

        /// <summary>
        /// Saves the window size as it's being resized
        /// </summary>
        private void OnMainFormResize(Object sender, System.EventArgs e)
        {
            if (this.WindowState != FormWindowState.Maximized && this.WindowState != FormWindowState.Minimized)
            {
                this.appSettings.WindowSize = this.Size;
            }
        }

        /// <summary>
        /// Saves the window location as it's being moved
        /// </summary>
        private void OnMainFormLocationChange(Object sender, System.EventArgs e)
        {
            if (this.WindowState != FormWindowState.Maximized && this.WindowState != FormWindowState.Minimized)
            {
                this.appSettings.WindowSize = this.Size;
                this.appSettings.WindowPosition = this.Location;
            }
        }

        /// <summary>
        /// Setups misc stuff when MainForm is loaded
        /// </summary>
        private void OnMainFormLoad(Object sender, System.EventArgs e)
        {
            /**
            * DockPanel events
            */
            this.dockPanel.ActivePaneChanged += new EventHandler(this.OnActivePaneChanged);
            this.dockPanel.ActiveContentChanged += new EventHandler(this.OnActiveContentChanged);
            this.dockPanel.ActiveDocumentChanged += new EventHandler(this.OnActiveDocumentChanged);
            this.dockPanel.ContentRemoved += new EventHandler<DockContentEventArgs>(this.OnContentRemoved);
            /**
            * Populate menus and check buttons 
            */
            ButtonManager.PopulateReopenMenu();
            ButtonManager.UpdateFlaggedButtons();
            /**
            * Set the initial directory for file dialogs
            */
            String path = this.appSettings.LatestDialogPath;
            this.openFileDialog.InitialDirectory = path;
            this.saveFileDialog.InitialDirectory = path;
            this.workingDirectory = path;
            /**
            * Open document[s] in startup 
            */
            if (Arguments != null && Arguments.Length != 0)
            {
                this.ProcessParameters(Arguments);
                Arguments = null;
            }
            else if (this.appSettings.RestoreFileSession)
            {
                String file = FileNameHelper.SessionData;
                SessionManager.RestoreSession(file, SessionType.Startup);
            }
            if (this.Documents.Length == 0)
            {
                NotifyEvent ne = new NotifyEvent(EventType.FileEmpty);
                EventManager.DispatchEvent(this, ne);
                if (!ne.Handled) this.SmartNew(null, null);
            }
            /**
            * Apply the default loaded theme
            */
            ThemeManager.WalkControls(this);
            /**
            * Notify plugins that the application is ready
            */
            EventManager.DispatchEvent(this, new NotifyEvent(EventType.UIStarted));
            EventManager.DispatchEvent(this, new NotifyEvent(EventType.Completion));
            /**
            * Start polling for file changes outside of the editor
            */
            FilePollManager.InitializePolling();
            /**
            * Apply all settings to all documents
            */
            this.ApplyAllSettings();
            /**
            * Initialize window and continue layout
            */
            this.InitializeWindow();
            /**
            * Initializes the restart button
            */
            this.InitializeRestartButton();
            /**
            * Check for updates when needed
            */
            this.CheckForUpdates();
        }

        /// <summary>
        /// Checks that if there are modified documents when the MainForm is closing
        /// </summary>
        public void OnMainFormClosing(Object sender, System.ComponentModel.CancelEventArgs e)
        {
            this.closingEntirely = true;
            Session session = SessionManager.GetCurrentSession();
            NotifyEvent ne = new NotifyEvent(EventType.UIClosing);
            EventManager.DispatchEvent(this, ne);
            if (ne.Handled)
            {
                this.closingEntirely = false;
                e.Cancel = true;
            }
            if (!e.Cancel && Globals.Settings.ConfirmOnExit)
            {
                String title = TextHelper.GetString("Title.ConfirmDialog");
                String message = TextHelper.GetString("Info.AreYouSureToExit");
                DialogResult result = MessageBox.Show(this, message, " " + title, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.No) e.Cancel = true;
            }
            if (!e.Cancel) this.CloseAllDocuments(false);
            if (this.closeAllCanceled)
            {
                this.closeAllCanceled = false;
                this.closingEntirely = false;
                e.Cancel = true;
            }
            if (!e.Cancel && this.isFullScreen)
            {
                this.ToggleFullScreen(null, null);
            }
            if (!e.Cancel && this.Documents.Length == 0)
            {
                NotifyEvent fe = new NotifyEvent(EventType.FileEmpty);
                EventManager.DispatchEvent(this, fe);
                if (!fe.Handled) this.SmartNew(null, null);
            }
            if (!e.Cancel)
            {
                String file = FileNameHelper.SessionData;
                SessionManager.SaveSession(file, session);
                ShortcutManager.SaveCustomShortcuts();
                ArgumentDialog.SaveCustomArguments();
                ClipboardManager.Dispose();
                PluginServices.DisposePlugins();
                this.KillProcess();
                this.SaveAllSettings();
            }
            else this.restartRequested = false;
        }

        /// <summary>
        /// When form is closed restart if requested.
        /// </summary>
        public void OnMainFormClosed(Object sender, FormClosedEventArgs e)
        {
            if (this.restartRequested)
            {
                this.restartRequested = false;
                Process.Start(Application.ExecutablePath);
                Process.GetCurrentProcess().Kill();
            }
        }

        /// <summary>
        /// When dock changes, applies the padding to documents
        /// </summary>
        private void OnActivePaneChanged(Object sender, EventArgs e)
        {
            this.quickFind.ApplyFixedDocumentPadding();
        }

        /// <summary>
        /// When document is removed update tab texts
        /// </summary>
        public void OnContentRemoved(Object sender, DockContentEventArgs e)
        {
            TabTextManager.UpdateTabTexts();
        }

        /// <summary>
        /// Dispatch UIRefresh event and focus scintilla control
        /// </summary>
        private void OnActiveContentChanged(Object sender, System.EventArgs e)
        {
            if (this.dockPanel.ActiveContent != null)
            {
                if (this.dockPanel.ActiveContent.GetType() == typeof(TabbedDocument))
                {
                    this.panelIsActive = false;
                    TabbedDocument document = (TabbedDocument)this.dockPanel.ActiveContent;
                    document.Activate();
                }
                else this.panelIsActive = true;
                NotifyEvent ne = new NotifyEvent(EventType.UIRefresh);
                EventManager.DispatchEvent(this, ne);
            }
        }

        /// <summary>
        /// Updates the UI, tabbing, working directory and the button states. 
        /// Also notifies the plugins for the FileOpen and FileSwitch events.
        /// </summary>
        public void OnActiveDocumentChanged(Object sender, System.EventArgs e)
        {
            try
            {
                if (this.CurrentDocument == null) return;
                this.OnScintillaControlUpdateControl(this.CurrentDocument.SciControl);
                this.quickFind.CanSearch = this.CurrentDocument.IsEditable;
                /**
                * Bring this newly active document to the top of the tab history
                * unless you're currently cycling through tabs with the keyboard
                */
                TabbingManager.UpdateSequentialIndex(this.CurrentDocument);
                if (!TabbingManager.TabTimer.Enabled)
                {
                    TabbingManager.TabHistory.Remove(this.CurrentDocument);
                    TabbingManager.TabHistory.Insert(0, this.CurrentDocument);
                }
                if (this.CurrentDocument.IsEditable)
                {
                    /**
                    * Apply correct extension when saving
                    */
                    if (this.appSettings.ApplyFileExtension)
                    {
                        String extension = Path.GetExtension(this.CurrentDocument.FileName);
                        if (extension != "") this.saveFileDialog.DefaultExt = extension;
                    }
                    /**
                    * Set current working directory
                    */
                    String path = Path.GetDirectoryName(this.CurrentDocument.FileName);
                    if (!this.CurrentDocument.IsUntitled && Directory.Exists(path))
                    {
                        this.workingDirectory = path;
                    }
                    /**
                    * Checks the file changes
                    */
                    TabbedDocument document = (TabbedDocument)this.CurrentDocument;
                    document.Activate();
                    /**
                    * Processes the opened file
                    */
                    if (this.notifyOpenFile)
                    {
                        ScintillaManager.UpdateControlSyntax(this.CurrentDocument.SciControl);
                        if (File.Exists(this.CurrentDocument.FileName))
                        {
                            TextEvent te = new TextEvent(EventType.FileOpen, this.CurrentDocument.FileName);
                            EventManager.DispatchEvent(this, te);
                        }
                        this.notifyOpenFile = false;
                    }
                }
                TabTextManager.UpdateTabTexts();
                NotifyEvent ne = new NotifyEvent(EventType.FileSwitch);
                EventManager.DispatchEvent(this, ne);
                NotifyEvent ce = new NotifyEvent(EventType.Completion);
                EventManager.DispatchEvent(this, ce);
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Checks that if the are any modified documents when closing.
        /// </summary>
        public void OnDocumentClosing(Object sender, System.ComponentModel.CancelEventArgs e)
        {
            ITabbedDocument document = (ITabbedDocument)sender;
            if (this.closeAllCanceled && this.closingAll) e.Cancel = true;
            else if (document.IsModified)
            {
                String saveChanges = TextHelper.GetString("Info.SaveChanges");
                String saveChangesTitle = TextHelper.GetString("Title.SaveChanges");
                DialogResult result = MessageBox.Show(this, saveChanges, saveChangesTitle + " " + document.Text, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    if (document.IsUntitled)
                    {
                        this.saveFileDialog.FileName = document.FileName;
                        if (this.saveFileDialog.ShowDialog(this) == DialogResult.OK && this.saveFileDialog.FileName.Length != 0)
                        {
                            ButtonManager.AddNewReopenMenuItem(this.saveFileDialog.FileName);
                            document.Save(this.saveFileDialog.FileName);
                        }
                        else
                        {
                            if (this.closingAll) this.closeAllCanceled = true;
                            e.Cancel = true;
                        }
                    }
                    else document.Save();
                }
                else if (result == DialogResult.Cancel)
                {
                    if (this.closingAll) this.closeAllCanceled = true;
                    e.Cancel = true;
                }
                else if (result == DialogResult.No)
                {
                    RecoveryManager.RemoveTemporaryFile(document.FileName);
                }
            }
            if (this.Documents.Length == 1 && document.IsUntitled && !document.IsModified && document.SciControl.Length == 0 && !e.Cancel && !this.closingForOpenFile && !this.restoringContents)
            {
                e.Cancel = true;
            }
            if (this.Documents.Length == 1 && !e.Cancel && !this.closingForOpenFile && !this.closingEntirely && !this.restoringContents)
            {
                NotifyEvent ne = new NotifyEvent(EventType.FileEmpty);
                EventManager.DispatchEvent(this, ne);
                if (!ne.Handled) this.SmartNew(null, null);
            }
        }

        /// <summary>
        /// Activates the previous document when document is closed
        /// </summary>
        public void OnDocumentClosed(Object sender, System.EventArgs e)
        {
            ITabbedDocument document = sender as ITabbedDocument;
            TabbingManager.TabHistory.Remove(document);
            TextEvent ne = new TextEvent(EventType.FileClose, document.FileName);
            EventManager.DispatchEvent(this, ne);
            if (this.appSettings.SequentialTabbing)
            {
                if (TabbingManager.SequentialIndex == 0) this.Documents[0].Activate();
                else TabbingManager.NavigateTabsSequentially(-1);
            }
            else TabbingManager.NavigateTabHistory(0);
            if (document.IsEditable && !document.IsUntitled)
            {
                if (this.appSettings.RestoreFileStates) FileStateManager.SaveFileState(document);
                RecoveryManager.RemoveTemporaryFile(document.FileName);
                OldTabsManager.SaveOldTabDocument(document.FileName);
            }
            ButtonManager.UpdateFlaggedButtons();
        }

        /// <summary>
        /// Refreshes the statusbar display and updates the important edit buttons
        /// </summary>
        public void OnScintillaControlUpdateControl(ScintillaControl sci)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate { this.OnScintillaControlUpdateControl(sci); });
                return;
            }
            ITabbedDocument document = DocumentManager.FindDocument(sci);
            if (sci != null && document != null && document.IsEditable)
            {
                String statusText = " " + TextHelper.GetString("Info.StatusText");
                String line = sci.CurrentLine + 1 + " / " + sci.LineCount;
                String column = sci.Column(sci.CurrentPos) + 1 + " / " + (sci.Column(sci.LineEndPosition(sci.CurrentLine)) + 1);
                var oldOS = this.OSVersion.Major < 6; // Vista is 6.0 and ok...
                String file = oldOS ? PathHelper.GetCompactPath(sci.FileName) : sci.FileName;
                String eol = (sci.EOLMode == 0) ? "CR+LF" : ((sci.EOLMode == 1) ? "CR" : "LF");
                String encoding = ButtonManager.GetActiveEncodingName();
                this.toolStripStatusLabel.Text = String.Format(statusText, line, column, eol, encoding, file);
            }
            else this.toolStripStatusLabel.Text = " ";
            this.OnUpdateMainFormDialogTitle();
            ButtonManager.UpdateFlaggedButtons();
            NotifyEvent ne = new NotifyEvent(EventType.UIRefresh);
            EventManager.DispatchEvent(this, ne);
        }

        /// <summary>
        /// Opens the selected files on drop
        /// </summary>
        public void OnScintillaControlDropFiles(ScintillaControl sci, String data)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate { this.OnScintillaControlDropFiles(null, data); });
                return;
            }
            String[] files = Regex.Split(data.Substring(1, data.Length - 2), "\" \"");
            foreach (String file in files)
            {
                if (File.Exists(file))
                {
                    DockContent doc = this.OpenEditableDocument(file);
                    if (doc == null || Control.ModifierKeys == Keys.Control) return;
                    DockContent drop = DocumentManager.FindDocument(sci) as DockContent;
                    if (drop != null && drop.Pane != null)
                    {
                        doc.DockTo(drop.Pane, DockStyle.Fill, -1);
                        doc.Activate();
                    }
                }
            }
        }

        /// <summary>
        /// Notifies when the user is trying to modify a read only file
        /// </summary>
        public void OnScintillaControlModifyRO(ScintillaControl sci)
        {
            if (!sci.Enabled || !File.Exists(sci.FileName)) return;
            TextEvent te = new TextEvent(EventType.FileModifyRO, sci.FileName);
            EventManager.DispatchEvent(this, te);
            if (te.Handled) return; // Let plugin handle this...
            String dlgTitle = TextHelper.GetString("Title.ConfirmDialog");
            String message = TextHelper.GetString("Info.MakeReadOnlyWritable");
            if (MessageBox.Show(this, message, dlgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                ScintillaManager.MakeFileWritable(sci);
            }
        }

        /// <summary>
        /// Handles the modified event
        /// </summary>
        public void OnScintillaControlModified(ScintillaControl sender, Int32 pos, Int32 modType, String text, Int32 length, Int32 lAdded, Int32 line, Int32 fLevelNow, Int32 fLevelPrev)
        {
            ITabbedDocument document = DocumentManager.FindDocument(sender);
            if (document != null && document.IsEditable)
            {
                this.OnDocumentModify(document);
                if (this.appSettings.ViewModifiedLines)
                {
                    Int32 flags = sender.ModEventMask;
                    sender.ModEventMask = flags & ~(Int32)ScintillaNet.Enums.ModificationFlags.ChangeMarker;
                    Int32 modLine = sender.LineFromPosition(pos);
                    sender.MarkerAdd(modLine, 2);
                    for (Int32 i = 0; i <= lAdded; i++)
                    {
                        sender.MarkerAdd(modLine + i, 2);
                    }
                    sender.ModEventMask = flags;
                }
            }
        }

        /// <summary>
        /// Provides a basic folding service and notifies the plugins for the MarginClick event
        /// </summary>
        public void OnScintillaControlMarginClick(ScintillaControl sci, Int32 modifiers, Int32 position, Int32 margin)
        {
            if (margin == ScintillaManager.FoldingMargin)
            {
                Int32 line = sci.LineFromPosition(position);
                if (Control.ModifierKeys == Keys.Control) MarkerManager.ToggleMarker(sci, 0, line);
                else sci.ToggleFold(line);
            }
        }

        /// <summary>
        /// Handles the mouse wheel on hover
        /// </summary>
        public Boolean PreFilterMessage(ref Message m)
        {
            if (Win32.ShouldUseWin32() && m.Msg == 0x20a) // WM_MOUSEWHEEL
            {
                Int32 x = unchecked((short)(long)m.LParam);
                Int32 y = unchecked((short)((long)m.LParam >> 16));
                IntPtr hWnd = Win32.WindowFromPoint(new Point(x, y));
                if (hWnd != IntPtr.Zero)
                {
                    ITabbedDocument doc = Globals.CurrentDocument;
                    if (Control.FromHandle(hWnd) != null)
                    {
                        Win32.SendMessage(hWnd, m.Msg, m.WParam, m.LParam);
                        return true;
                    }
                    else if (doc != null && doc.IsEditable && (hWnd == doc.SplitSci1.HandleSci || hWnd == doc.SplitSci2.HandleSci))
                    {
                        Win32.SendMessage(hWnd, m.Msg, m.WParam, m.LParam);
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Handles the application shortcuts
        /// </summary>
        protected override Boolean ProcessCmdKey(ref Message msg, Keys keyData)
        {
            /**
            * Notify plugins. Don't notify ControlKey or ShiftKey as it polls a lot
            */
            KeyEvent ke = new KeyEvent(EventType.Keys, keyData);
            Keys keyCode = keyData & Keys.KeyCode;
            if ((keyCode != Keys.ControlKey) && (keyCode != Keys.ShiftKey))
            {
                EventManager.DispatchEvent(this, ke);
            }
            if (!ke.Handled)
            {
                /**
                * Ignore basic control keys if sci doesn't have focus.
                */ 
                if (Globals.SciControl == null || !Globals.SciControl.IsFocus)
                {
                    if (keyData == (Keys.Control | Keys.C)) return false;
                    else if (keyData == (Keys.Control | Keys.V)) return false;
                    else if (keyData == (Keys.Control | Keys.X)) return false;
                    else if (keyData == (Keys.Control | Keys.A)) return false;
                    else if (keyData == (Keys.Control | Keys.Z)) return false;
                    else if (keyData == (Keys.Control | Keys.Y)) return false;
                }
                /**
                * Process special key combinations and allow "chaining" of 
                * Ctrl-Tab commands if you keep holding control down.
                */
                if ((keyData & Keys.Control) != 0)
                {
                    Boolean sequentialTabbing = this.appSettings.SequentialTabbing;
                    if ((keyData == (Keys.Control | Keys.Next)) || (keyData == (Keys.Control | Keys.Tab)))
                    {
                        TabbingManager.TabTimer.Enabled = true;
                        if (keyData == (Keys.Control | Keys.Next) || sequentialTabbing)
                        {
                            TabbingManager.NavigateTabsSequentially(1);
                        }
                        else TabbingManager.NavigateTabHistory(1);
                        return true;
                    }
                    if ((keyData == (Keys.Control | Keys.Prior)) || (keyData == (Keys.Control | Keys.Shift | Keys.Tab)))
                    {
                        TabbingManager.TabTimer.Enabled = true;
                        if (keyData == (Keys.Control | Keys.Prior) || sequentialTabbing)
                        {
                            TabbingManager.NavigateTabsSequentially(-1);
                        }
                        else TabbingManager.NavigateTabHistory(-1);
                        return true;
                    }
                }
                return base.ProcessCmdKey(ref msg, keyData);
            }
            return true;
        }

        /// <summary>
        /// Notifies the plugins for the SyntaxChange event
        /// </summary>
        public void OnSyntaxChange(String language)
        {
            TextEvent te = new TextEvent(EventType.SyntaxChange, language);
            EventManager.DispatchEvent(this, te);
        }

        /// <summary>
        /// Updates the MainForm's title automatically
        /// </summary>
        public void OnUpdateMainFormDialogTitle()
        {
            IProject project = PluginBase.CurrentProject;
            ITabbedDocument document = this.CurrentDocument;
            if (project != null) this.Text = project.Name + " - " + DistroConfig.DISTRIBUTION_NAME;
            else if (document != null && document.IsEditable)
            {
                String file = Path.GetFileName(document.FileName);
                this.Text = file + " - " + DistroConfig.DISTRIBUTION_NAME;
            }
            else this.Text = DistroConfig.DISTRIBUTION_NAME;
        }

        /// <summary>
        /// Sets the current document unmodified and updates it
        /// </summary>
        public void OnDocumentReload(ITabbedDocument document)
        {
            document.IsModified = false;
            this.reloadingDocument = false;
            this.OnUpdateMainFormDialogTitle();
            if (document.IsEditable) document.SciControl.MarkerDeleteAll(2);
            ButtonManager.UpdateFlaggedButtons();
        }

        /// <summary>
        /// Sets the current document modified
        /// </summary>
        public void OnDocumentModify(ITabbedDocument document)
        {
            if (document.IsEditable && !document.IsModified && !this.reloadingDocument && !this.processingContents)
            {
                document.IsModified = true;
                TextEvent te = new TextEvent(EventType.FileModify, document.FileName);
                EventManager.DispatchEvent(this, te);
            }
        }

        /// <summary>
        /// Notifies the plugins for the FileSave event and includes the given reason for the save.
        /// </summary>
        public void OnFileSave(ITabbedDocument document, string oldFile, string reason)
        {
            if (oldFile != null)
            {
                String args = document.FileName + ";" + oldFile;
                TextEvent rename = new TextEvent(EventType.FileRename, args);
                EventManager.DispatchEvent(this, rename);
                TextEvent open = new TextEvent(EventType.FileOpen, document.FileName);
                EventManager.DispatchEvent(this, open);
            }
            this.OnUpdateMainFormDialogTitle();
            if (document.IsEditable) document.SciControl.MarkerDeleteAll(2);
            TextDataEvent save = new TextDataEvent(EventType.FileSave, document.FileName, reason);
            EventManager.DispatchEvent(this, save);
            ButtonManager.UpdateFlaggedButtons();
            TabTextManager.UpdateTabTexts();
        }

        /// <summary>
        /// Notifies the plugins for the FileSave event
        /// </summary>
        public void OnFileSave(ITabbedDocument document, String oldFile)
        {
            OnFileSave(document, oldFile, null);
        }

        /// <summary>
        /// Handles clipboard updates.
        /// </summary>
        protected override void WndProc(ref Message m)
        {
            if (ClipboardManager.HandleWndProc(ref m))
            {
                ClipboardHistoryDialog.UpdateHistory();
            }
            base.WndProc(ref m);
        }

        #endregion

        #region General Methods

        /// <summary>
        /// Finds the specified plugin
        /// </summary>
        public IPlugin FindPlugin(String guid)
        {
            AvailablePlugin plugin = PluginServices.Find(guid);
            return plugin.Instance;
        }

        /// <summary>
        /// Finds the specified composed/ready image that is automatically adjusted according to the theme.
        /// <para/>
        /// If you make a copy of the image returned by this method, the copy will not be automatically adjusted.
        /// </summary>
        public Image FindImage(String data)
        {
            return FindImage(data, true);
        }

        /// <summary>
        /// Finds the specified composed/ready image.
        /// <para/>
        /// If you make a copy of the image returned by this method, the copy will not be automatically adjusted, even if <code>autoAdjusted</code> is <code>true</code>.
        /// </summary>
        public Image FindImage(String data, Boolean autoAdjusted)
        {
            try
            {
                lock (this) return ImageManager.GetComposedBitmap(data, autoAdjusted);
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
                return null;
            }
        }

        /// <summary>
        /// Finds the specified composed/ready image that is automatically adjusted according to the theme.
        /// The image size is always 16x16.
        /// <para/>
        /// If you make a copy of the image returned by this method, the copy will not be automatically adjusted.
        /// </summary>
        public Image FindImage16(String data)
        {
            return FindImage16(data, true);
        }

        /// <summary>
        /// Finds the specified composed/ready image. The image size is always 16x16.
        /// <para/>
        /// If you make a copy of the image returned by this method, the copy will not be automatically adjusted, even if <code>autoAdjusted</code> is <code>true</code>.
        /// </summary>
        public Image FindImage16(String data, Boolean autoAdjusted)
        {
            try
            {
                lock (this) return ImageManager.GetComposedBitmapSize16(data, autoAdjusted);
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
                return null;
            }
        }

        /// <summary>
        /// Finds the specified composed/ready image and returns a copy of the image that has its color adjusted.
        /// This method is typically used for populating a <see cref="ImageList"/> object.
        /// <para/>
        /// Equivalent to calling <code>ImageSetAdjust(FindImage(data, false))</code>.
        /// </summary>
        public Image FindImageAndSetAdjust(String data)
        {
            return ImageSetAdjust(FindImage(data, false));
        }

        /// <summary>
        /// Returns a copy of the specified image that has its color adjusted.
        /// </summary>
        public Image ImageSetAdjust(Image image)
        {
            return ImageManager.SetImageAdjustment(image);
        }

        /// <summary>
        /// Gets a copy of the image that gets automatically adjusted according to the theme.
        /// </summary>
        public Image GetAutoAdjustedImage(Image image)
        {
            return ImageManager.GetAutoAdjustedImage(image);
        }

        /// <summary>
        /// Adjusts all images for different themes.
        /// </summary>
        public void AdjustAllImages()
        {
            ImageManager.AdjustAllImages();
            ImageListManager.RefreshAll();

            for (int i = 0, length = LayoutManager.PluginPanels.Count; i < length; i++)
            {
                DockablePanel panel = LayoutManager.PluginPanels[i] as DockablePanel;
                if (panel != null) panel.RefreshIcon();
            }
        }
        
        /// <summary>
        /// Themes the controls from the parent
        /// </summary>
        public void ThemeControls(Object parent)
        {
            ThemeManager.WalkControls(parent);
        }

        /// <summary>
        /// Gets a theme property color
        /// </summary>
        public Color GetThemeColor(String id)
        {
            return ThemeManager.GetThemeColor(id);
        }

        /// <summary>
        /// Gets a theme property color with a fallback
        /// </summary>
        public Color GetThemeColor(String id, Color fallback)
        {
            Color color = ThemeManager.GetThemeColor(id);
            if (color != Color.Empty) return color;
            else return fallback;
        }

        /// <summary>
        /// Gets a theme property value
        /// </summary>
        public String GetThemeValue(String id)
        {
            return ThemeManager.GetThemeValue(id);
        }

        /// <summary>
        /// Gets a theme property value with a fallback
        /// </summary>
        public String GetThemeValue(String id, String fallback)
        {
            String value = ThemeManager.GetThemeValue(id);
            if (!String.IsNullOrEmpty(value)) return value;
            else return fallback;
        }

        /// <summary>
        /// Gets a theme flag value.
        /// </summary>
        public Boolean GetThemeFlag(String id)
        {
            return GetThemeFlag(id, false);
        }

        /// <summary>
        /// Gets a theme flag value with a fallback.
        /// </summary>
        public Boolean GetThemeFlag(String id, Boolean fallback)
        {
            String value = ThemeManager.GetThemeValue(id);
            if (String.IsNullOrEmpty(value)) return fallback;
            switch (value.ToLower())
            {
                case "true": return true;
                case "false": return false;
                default: return fallback;
            }
        }

        /// <summary>
        /// Sets if child controls should use theme.
        /// </summary>
        public void SetUseTheme(Object obj, Boolean use)
        {
            ThemeManager.SetUseTheme(obj, use);
        }

        /// <summary>
        /// Finds the specified menu item by name
        /// </summary>
        public ToolStripItem FindMenuItem(String name)
        {
            return StripBarManager.FindMenuItem(name);
        }

        /// <summary>
        /// Finds the menu items that have the specified name
        /// </summary>
        public List<ToolStripItem> FindMenuItems(String name)
        {
            return StripBarManager.FindMenuItems(name);
        }

        /// <summary>
        /// Lets you update menu items using the flag functionality
        /// </summary>
        public void AutoUpdateMenuItem(ToolStripItem item, String action)
        {
            Boolean value = ButtonManager.ValidateFlagAction(item, action);
            ButtonManager.ExecuteFlagAction(item, action, value);
        }

        /// <summary>
        /// Gets the specified item's shortcut keys.
        /// </summary>
        public Keys GetShortcutItemKeys(String id)
        {
            ShortcutItem item = ShortcutManager.GetRegisteredItem(id);
            return item == null ? Keys.None : item.Custom;
        }

        /// <summary>
        /// Gets the specified item's id.
        /// </summary>
        public String GetShortcutItemId(Keys keys)
        {
            ShortcutItem item = ShortcutManager.GetRegisteredItem(keys);
            return item == null ? string.Empty : item.Id;
        }

        /// <summary>
        /// Registers a new menu item with the shortcut manager
        /// </summary>
        public void RegisterShortcutItem(String id, Keys keys)
        {
            ShortcutManager.RegisterItem(id, keys);
        }

        /// <summary>
        /// Registers a new menu item with the shortcut manager
        /// </summary>
        public void RegisterShortcutItem(String id, ToolStripMenuItem item)
        {
            ShortcutManager.RegisterItem(id, item);
        }

        /// <summary>
        /// Registers a new secondary menu item with the shortcut manager
        /// </summary>
        public void RegisterSecondaryItem(String id, ToolStripItem item)
        {
            ShortcutManager.RegisterSecondaryItem(id, item);
        }

        /// <summary>
        /// Updates a registered secondary menu item in the shortcut manager
        /// - should be called when the tooltip changes.
        /// </summary>
        public void ApplySecondaryShortcut(ToolStripItem item)
        {
            ShortcutManager.ApplySecondaryShortcut(item);
        }

        /// <summary>
        /// Shows the settings dialog
        /// </summary>
        public void ShowSettingsDialog(String itemName)
        {
            SettingDialog.Show(itemName, "");
        }
        public void ShowSettingsDialog(String itemName, String filter)
        {
            SettingDialog.Show(itemName, filter);
        }

        /// <summary>
        /// Shows the error dialog if the sender is ErrorManager
        /// </summary>
        public void ShowErrorDialog(Object sender, Exception ex)
        {
            if (sender.GetType().ToString() != "PluginCore.Managers.ErrorManager")
            {
                String message = TextHelper.GetString("Info.OnlyErrorManager");
                ErrorDialog.Show(new Exception(message));
            }
            else ErrorDialog.Show(ex);
        }

        /// <summary>
        /// Show a message to the user to restart FD
        /// </summary>
        public void RestartRequired()
        {
            if (this.restartButton != null) this.restartButton.Visible = true;
            this.RequiresRestart = true;
            String message = TextHelper.GetString("Info.RequiresRestart");
            TraceManager.Add(message);
        }

        /// <summary>
        /// Refreshes the main form
        /// </summary>
        public void RefreshUI()
        {
            if (this.CurrentDocument == null) return;
            ScintillaControl sci = this.CurrentDocument.SciControl;
            this.OnScintillaControlUpdateControl(sci);
        }

        /// <summary>
        /// Clears the temporary files from disk
        /// </summary>
        public void ClearTemporaryFiles(String file)
        {
            RecoveryManager.RemoveTemporaryFile(file);
            FileStateManager.RemoveStateFile(file);
        }

        /// <summary>
        /// Refreshes the scintilla configuration
        /// </summary>
        public void RefreshSciConfig()
        {
            ScintillaManager.LoadConfiguration();
        }

        /// <summary>
        /// Processes the argument string variables
        /// </summary>
        public String ProcessArgString(String args, bool dispatch)
        {
            return ArgsProcessor.ProcessString(args, dispatch);
        }
        public String ProcessArgString(String args)
        {
            return ArgsProcessor.ProcessString(args, true);
        }

        /// <summary>
        /// Processes the incoming arguments 
        /// </summary> 
        public void ProcessParameters(String[] args)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker)delegate { this.ProcessParameters(args); });
                return;
            }
            this.Activate(); this.Focus();
            if (args != null && args.Length != 0)
            {
                Silent = Array.IndexOf(args, "-silent") != -1;
                for (Int32 i = 0; i < args.Length; i++)
                {
                    OpenDocumentFromParameters(args[i]);
                }
            }
            if (Win32.ShouldUseWin32()) Win32.RestoreWindow(this.Handle);
            /**
            * Notify plugins about start arguments
            */
            NotifyEvent ne = new NotifyEvent(EventType.StartArgs);
            EventManager.DispatchEvent(this, ne);
        }

        /// <summary>
        /// 
        /// </summary>
        private void OpenDocumentFromParameters(String file)
        {
            Match openParams = Regex.Match(file, "@([0-9]+)($|:([0-9]+)$)"); // path@line:col
            if (openParams.Success)
            {
                file = file.Substring(0, openParams.Index);
                file = PathHelper.GetLongPathName(file);
                if (File.Exists(file))
                {
                    TabbedDocument doc = this.OpenEditableDocument(file, false) as TabbedDocument;
                    if (doc != null) ApplyOpenParams(openParams, doc.SciControl);
                    else if (CurrentDocument.FileName == file) ApplyOpenParams(openParams, CurrentDocument.SciControl);
                }
            }
            else if (File.Exists(file))
            {
                file = PathHelper.GetLongPathName(file);
                this.OpenEditableDocument(file);
            }
        }

        /// <summary>
        /// 
        /// </summary>        
        private void ApplyOpenParams(Match openParams, ScintillaControl sci)
        {
            if (sci == null) return;
            Int32 col = 0;
            Int32 line = Math.Min(sci.LineCount - 1, Math.Max(0, Int32.Parse(openParams.Groups[1].Value) - 1));
            if (openParams.Groups.Count > 3 && openParams.Groups[3].Value.Length > 0)
            {
                col = Int32.Parse(openParams.Groups[3].Value);
            }
            Int32 position = sci.PositionFromLine(line) + col;
            sci.SetSel(position, position);
        }

        /// <summary>
        /// Closes all open documents with an option: exceptCurrent
        /// </summary>
        public void CloseAllDocuments(Boolean exceptCurrent)
        {
            CloseAllDocuments(exceptCurrent, false);
        }
        public void CloseAllDocuments(Boolean exceptCurrent, Boolean exceptOtherPanes)
        {
            ITabbedDocument current = this.CurrentDocument;
            DockPane currentPane = (current == null) ? null : current.DockHandler.PanelPane;
            this.closeAllCanceled = false; this.closingAll = true;
            var documents = new List<ITabbedDocument>(Documents);
            foreach (var document in documents)
            {
                Boolean close = true;
                if (exceptCurrent && document == current) close = false;
                if (exceptOtherPanes && document.DockHandler.PanelPane != currentPane) close = false;
                if (close) document.Close();
            }
            this.closingAll = false;
        }

        /// <summary>
        /// Updates all needed settings after modification
        /// </summary>
        public void ApplyAllSettings()
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke((MethodInvoker) this.ApplyAllSettings);
                return;
            }
            ShortcutManager.ApplyAllShortcuts();
            EventManager.DispatchEvent(this, new NotifyEvent(EventType.ApplySettings));
            for (Int32 i = 0; i < this.Documents.Length; i++)
            {
                ITabbedDocument document = this.Documents[i];
                if (document.IsEditable)
                {
                    ScintillaManager.ApplySciSettings(document.SplitSci1, true);
                    ScintillaManager.ApplySciSettings(document.SplitSci2, true);
                }
            }
            this.frInFilesDialog.UpdateSettings();
            this.statusStrip.Visible = this.appSettings.ViewStatusBar;
            this.toolStrip.Visible = this.isFullScreen ? false : this.appSettings.ViewToolBar;
            ButtonManager.UpdateFlaggedButtons();
            TabTextManager.UpdateTabTexts();
            ClipboardManager.ApplySettings();
        }

        /// <summary>
        /// Saves all settings of the FlashDevelop
        /// </summary>
        public void SaveAllSettings()
        {
            SaveSettings();
            SaveLayout();
        }

        /// <summary>
        /// Saves settings to a file.
        /// </summary>
        public void SaveSettings()
        {
            try
            {
                this.appSettings.WindowState = this.WindowState;
                this.appSettings.LatestDialogPath = this.workingDirectory;
                if (this.WindowState != FormWindowState.Maximized && this.WindowState != FormWindowState.Minimized)
                {
                    this.appSettings.WindowSize = this.Size;
                    this.appSettings.WindowPosition = this.Location;
                }
                if (!File.Exists(FileNameHelper.SettingData))
                {
                    String folder = Path.GetDirectoryName(FileNameHelper.SettingData);
                    if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
                }
                ObjectSerializer.Serialize(FileNameHelper.SettingData, this.appSettings);
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Saves layout to a file.
        /// </summary>
        public void SaveLayout()
        {
            try
            {
                this.dockPanel.SaveAsXml(FileNameHelper.LayoutData);
            }
            catch (Exception ex)
            {
                // Ignore errors on multi instance full close...
                if (this.MultiInstanceMode && this.ClosingEntirely) return;
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Resolves the working directory
        /// </summary>
        public String GetWorkingDirectory()
        {
            IProject project = PluginBase.CurrentProject;
            ITabbedDocument document = this.CurrentDocument;
            if (document != null && document.IsEditable && File.Exists(document.FileName))
            {
                return Path.GetDirectoryName(document.FileName);
            }
            else if (project != null && File.Exists(project.ProjectPath))
            {
                return Path.GetDirectoryName(project.ProjectPath);
            }
            else return PathHelper.AppDir;
        }

        /// <summary>
        /// Gets the amount instances running
        /// </summary>
        public Int32 GetInstanceCount()
        {
            Process current = Process.GetCurrentProcess();
            return Process.GetProcessesByName(current.ProcessName).Length;
        }

        /// <summary>
        /// Sets the text to find globally
        /// </summary>
        public void SetFindText(Object sender, String text)
        {
            if (sender != this.quickFind) this.quickFind.SetFindText(text);
            if (sender != this.frInDocDialog) this.frInDocDialog.SetFindText(text);
        }

        /// <summary>
        /// Sets the case setting to find globally
        /// </summary>
        public void SetMatchCase(Object sender, Boolean matchCase)
        {
            if (sender != this.quickFind) this.quickFind.SetMatchCase(matchCase);
            if (sender != this.frInDocDialog) this.frInDocDialog.SetMatchCase(matchCase);
        }

        /// <summary>
        /// Sets the whole word setting to find globally
        /// </summary>
        public void SetWholeWord(Object sender, Boolean wholeWord)
        {
            if (sender != this.quickFind) this.quickFind.SetWholeWord(wholeWord);
            if (sender != this.frInDocDialog) this.frInDocDialog.SetWholeWord(wholeWord);
        }

        #endregion

        #region Click Handlers

        /// <summary>
        /// Creates a new blank document
        /// </summary>
        public void New(Object sender, EventArgs e)
        {
            String fileName = DocumentManager.GetNewDocumentName(null);
            TextEvent te = new TextEvent(EventType.FileNew, fileName);
            EventManager.DispatchEvent(this, te);
            if (!te.Handled)
            {
                this.CreateEditableDocument(fileName, "", (Int32)this.appSettings.DefaultCodePage);
            }
        }

        /// <summary>
        /// Creates a new blank document tracking current project
        /// </summary>
        public void SmartNew(Object sender, EventArgs e)
        {
            String ext = "";
            if (PluginBase.CurrentProject != null)
            {
                try
                {
                    String filter = PluginBase.CurrentProject.DefaultSearchFilter;
                    String tempExt = filter.Split(';')[0].Replace("*.", "");
                    if (Regex.Match(tempExt, "^[A-Za-z0-9]+$").Success) ext = tempExt;
                }
                catch { /* NO ERRORS */ }
            }
            String fileName = DocumentManager.GetNewDocumentName(ext);
            TextEvent te = new TextEvent(EventType.FileNew, fileName);
            EventManager.DispatchEvent(this, te);
            if (!te.Handled)
            {
                this.CreateEditableDocument(fileName, "", (Int32)this.appSettings.DefaultCodePage);
            }
        }

        /// <summary>
        /// Create a new document from a template
        /// </summary>
        public void NewFromTemplate(Object sender, EventArgs e)
        {
            try
            {
                ToolStripItem button = (ToolStripItem)sender;
                String[] args = this.ProcessArgString(((ItemData)button.Tag).Tag).Split(';');
                Encoding encoding = Encoding.GetEncoding((Int32)this.appSettings.DefaultCodePage);
                String fileName = DocumentManager.GetNewDocumentName(args[0]);
                String contents = FileHelper.ReadFile(args[1], encoding);
                String lineEndChar = LineEndDetector.GetNewLineMarker((int)Settings.EOLMode);
                contents = Regex.Replace(contents, @"\r\n?|\n", lineEndChar);
                String processed = this.ProcessArgString(contents);
                ActionPoint actionPoint = SnippetHelper.ProcessActionPoint(processed);
                if (this.Documents.Length == 1 && this.Documents[0].IsUntitled)
                {
                    this.closingForOpenFile = true;
                    this.Documents[0].Close();
                    this.closingForOpenFile = false;
                }
                TextEvent te = new TextEvent(EventType.FileTemplate, fileName);
                EventManager.DispatchEvent(this, te);
                if (!te.Handled)
                {
                    ITabbedDocument document = (ITabbedDocument)this.CreateEditableDocument(fileName, actionPoint.Text, encoding.CodePage);
                    SnippetHelper.ExecuteActionPoint(actionPoint, document.SciControl);
                }
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Create the specified new document from the given template
        /// </summary>
        public void FileFromTemplate(String templatePath, String newFilePath)
        {
            try
            {
                Encoding encoding = Encoding.GetEncoding((Int32)this.appSettings.DefaultCodePage);
                String contents = FileHelper.ReadFile(templatePath);
                String processed = this.ProcessArgString(contents);
                String lineEndChar = LineEndDetector.GetNewLineMarker((int)Settings.EOLMode);
                processed = Regex.Replace(processed, @"\r\n?|\n", lineEndChar);
                ActionPoint actionPoint = SnippetHelper.ProcessActionPoint(processed);
                FileHelper.WriteFile(newFilePath, actionPoint.Text, encoding, Globals.Settings.SaveUnicodeWithBOM);
                if (actionPoint.EntryPosition != -1)
                {
                    if (this.Documents.Length == 1 && this.Documents[0].IsUntitled)
                    {
                        this.closingForOpenFile = true;
                        this.Documents[0].Close();
                        this.closingForOpenFile = false;
                    }
                    TextEvent te = new TextEvent(EventType.FileTemplate, newFilePath);
                    EventManager.DispatchEvent(this, te);
                    if (!te.Handled)
                    {
                        ITabbedDocument document = (ITabbedDocument)this.CreateEditableDocument(newFilePath, actionPoint.Text, encoding.CodePage);
                        SnippetHelper.ExecuteActionPoint(actionPoint, document.SciControl);
                    }
                }
                else
                {
                    TextEvent te = new TextEvent(EventType.FileTemplate, newFilePath);
                    EventManager.DispatchEvent(this, te);
                }
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Opens the open file dialog and opens the selected files
        /// </summary>
        public void Open(Object sender, System.EventArgs e)
        {
            this.openFileDialog.Multiselect = true;
            this.openFileDialog.InitialDirectory = this.WorkingDirectory;
            if (this.openFileDialog.ShowDialog(this) == DialogResult.OK && this.openFileDialog.FileName.Length != 0)
            {
                Int32 count = this.openFileDialog.FileNames.Length;
                for (Int32 i = 0; i < count; i++)
                {
                    this.OpenEditableDocument(openFileDialog.FileNames[i]);
                }
            }
            this.openFileDialog.Multiselect = false;
        }

        /// <summary>
        /// Opens the open file dialog and opens the selected files in specific encoding
        /// </summary>
        public void OpenIn(Object sender, System.EventArgs e)
        {
            ToolStripItem button = (ToolStripItem)sender;
            Int32 encMode = Convert.ToInt32(((ItemData)button.Tag).Tag);
            Encoding encoding = Encoding.GetEncoding(encMode);
            this.openFileDialog.Multiselect = true; // Allow multiple...
            this.openFileDialog.InitialDirectory = this.WorkingDirectory;
            if (this.openFileDialog.ShowDialog(this) == DialogResult.OK && this.openFileDialog.FileName.Length != 0)
            {
                Int32 count = this.openFileDialog.FileNames.Length;
                for (Int32 i = 0; i < count; i++)
                {
                    if (encMode == 0) // Detect 8bit encoding...
                    {
                        Int32 codepage = FileHelper.GetFileCodepage(openFileDialog.FileNames[i]);
                        encoding = Encoding.GetEncoding(codepage);
                    }
                    this.OpenEditableDocument(openFileDialog.FileNames[i], encoding, false);
                }
            }
            this.openFileDialog.Multiselect = false;
        }

        /// <summary>
        /// Opens a the specified file in the UI
        /// </summary>
        public void Edit(Object sender, System.EventArgs e)
        {
            ToolStripItem button = (ToolStripItem)sender;
            String file = this.ProcessArgString(((ItemData)button.Tag).Tag);
            if (File.Exists(file)) this.OpenEditableDocument(file);
        }

        /// <summary>
        /// Reopens a file from the old documents list
        /// </summary>
        public void Reopen(Object sender, System.EventArgs e)
        {
            ToolStripItem button = (ToolStripItem)sender;
            String file = button.Tag.ToString();
            if (File.Exists(file))
            {
                this.OpenEditableDocument(file);
                ButtonManager.AddNewReopenMenuItem(file);
            }
            else
            {
                String message = TextHelper.GetString("Info.InvalidFileOnReopen");
                Settings.PreviousDocuments.Remove(file);
                ButtonManager.PopulateReopenMenu();
                ErrorManager.ShowInfo(message);
            }
        }

        /// <summary>
        /// Opens the last closed tabs if they are not open
        /// </summary>
        public void ReopenClosed(Object sender, System.EventArgs e)
        {
            OldTabsManager.OpenOldTabDocument();
        }

        /// <summary>
        /// Clears invalid entries from the old documents list
        /// </summary>
        public void CleanReopenList(Object sender, System.EventArgs e)
        {
            FileHelper.FilterByExisting(this.appSettings.PreviousDocuments, true);
            ButtonManager.PopulateReopenMenu();
        }

        /// <summary>
        /// Clears all entries from the old documents list
        /// </summary>
        public void ClearReopenList(Object sender, System.EventArgs e)
        {
            this.appSettings.PreviousDocuments.Clear();
            ButtonManager.PopulateReopenMenu();
        }

        /// <summary>
        /// Views the current clipboard history
        /// </summary>
        public void ClipboardHistory(object sender, EventArgs e)
        {
            ClipboardTextData data;
            if (ClipboardHistoryDialog.Show(out data))
            {
                Globals.SciControl.ReplaceSel(data.Text);
            }
        }

        /// <summary>
        /// Pastes lines at the correct indent level
        /// </summary>
        public void SmartPaste(Object sender, System.EventArgs e)
        {
            ScintillaControl sci = Globals.SciControl;
            if (sci.CanPaste)
            {
                // if clip is not line-based, then just do simple paste
                if ((sci.SelTextSize > 0 && !sci.SelText.EndsWith('\n')) || !Clipboard.GetText().EndsWith('\n') || Clipboard.ContainsData("MSDEVColumnSelect")) sci.Paste();
                else
                {
                    sci.BeginUndoAction();
                    try
                    {
                        if (sci.SelTextSize > 0) sci.Clear();
                        sci.Home();
                        int pos = sci.CurrentPos;
                        int lines = sci.LineCount;
                        sci.Paste();
                        lines = sci.LineCount - lines; // = # of lines in the pasted text
                        sci.ReindentLines(sci.LineFromPosition(pos), lines);
                    }
                    finally
                    {
                        sci.EndUndoAction();
                    }
                }
            }
        }

        /// <summary>
        /// Saves the current file session
        /// </summary>
        public void SaveSession(Object sender, System.EventArgs e)
        {
            try
            {
                ToolStripItem button = (ToolStripItem)sender;
                String file = ((ItemData)button.Tag).Tag;
                SessionManager.SaveSession(file);
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Restores the specified file session
        /// </summary>
        public void RestoreSession(Object sender, System.EventArgs e)
        {
            try
            {
                ToolStripItem button = (ToolStripItem)sender;
                String file = ((ItemData)button.Tag).Tag;
                SessionManager.RestoreSession(file, SessionType.External);
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Restores the specified panel layout
        /// </summary>
        public void RestoreLayout(Object sender, System.EventArgs e)
        {
            try
            {
                ToolStripItem button = (ToolStripItem)sender;
                String file = ((ItemData)button.Tag).Tag;
                LayoutManager.RestoreLayout(file);
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Saves the current file or opens a save file dialog
        /// </summary>
        public void Save(Object sender, System.EventArgs e)
        {
            try
            {
                if (this.CurrentDocument.IsUntitled)
                {
                    this.saveFileDialog.FileName = this.CurrentDocument.FileName;
                    this.saveFileDialog.InitialDirectory = this.WorkingDirectory;
                    if (this.saveFileDialog.ShowDialog(this) == DialogResult.OK && this.saveFileDialog.FileName.Length != 0)
                    {
                        ButtonManager.AddNewReopenMenuItem(this.saveFileDialog.FileName);
                        this.CurrentDocument.Save(this.saveFileDialog.FileName);
                        NotifyEvent ne = new NotifyEvent(EventType.FileSwitch);
                        EventManager.DispatchEvent(this, ne);
                        NotifyEvent ce = new NotifyEvent(EventType.Completion);
                        EventManager.DispatchEvent(this, ce);
                    }
                }
                else if (this.CurrentDocument.IsModified)
                {
                    var button = (ToolStripItem)sender;
                    var reason = ((ItemData)button.Tag).Tag;
                    this.CurrentDocument.Save(this.CurrentDocument.FileName, reason);
                }
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Saves the current document with the specified file name
        /// </summary>
        public void SaveAs(Object sender, System.EventArgs e)
        {
            this.saveFileDialog.FileName = this.CurrentDocument.FileName;
            this.saveFileDialog.InitialDirectory = this.WorkingDirectory;
            if (this.saveFileDialog.ShowDialog(this) == DialogResult.OK && this.saveFileDialog.FileName.Length != 0)
            {
                ButtonManager.AddNewReopenMenuItem(this.saveFileDialog.FileName);
                this.CurrentDocument.Save(this.saveFileDialog.FileName);
                NotifyEvent ne = new NotifyEvent(EventType.FileSwitch);
                EventManager.DispatchEvent(this, ne);
                NotifyEvent ce = new NotifyEvent(EventType.Completion);
                EventManager.DispatchEvent(this, ce);
            }
        }

        /// <summary>
        /// Saves the selected text as a snippet
        /// </summary>
        public void SaveAsSnippet(Object sender, System.EventArgs e)
        {
            try
            {
                this.saveFileDialog.FileName = "";
                this.saveFileDialog.DefaultExt = ".fds";
                String prevFilter = this.saveFileDialog.Filter;
                String snippetFilter = TextHelper.GetString("Info.SnippetFilter");
                this.saveFileDialog.Filter = snippetFilter + "|*.fds";
                String prevRootPath = this.saveFileDialog.InitialDirectory;
                this.saveFileDialog.InitialDirectory = PathHelper.SnippetDir;
                if (this.saveFileDialog.ShowDialog(this) == DialogResult.OK && this.saveFileDialog.FileName.Length != 0)
                {
                    String contents = Globals.SciControl.SelText;
                    String file = this.saveFileDialog.FileName;
                    FileHelper.WriteFile(file, contents, Encoding.UTF8);
                }
                this.saveFileDialog.InitialDirectory = prevRootPath;
                this.saveFileDialog.Filter = prevFilter;
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Saves the selected text as a template
        /// </summary>
        public void SaveAsTemplate(Object sender, System.EventArgs e)
        {
            try
            {
                this.saveFileDialog.FileName = "";
                this.saveFileDialog.DefaultExt = ".fdt";
                String prevFilter = this.saveFileDialog.Filter;
                String templateFilter = TextHelper.GetString("Info.TemplateFilter");
                this.saveFileDialog.Filter = templateFilter + "|*.fdt";
                String prevRootPath = this.saveFileDialog.InitialDirectory;
                this.saveFileDialog.InitialDirectory = PathHelper.TemplateDir;
                if (this.saveFileDialog.ShowDialog(this) == DialogResult.OK && this.saveFileDialog.FileName.Length != 0)
                {
                    String contents = Globals.SciControl.SelText;
                    String file = this.saveFileDialog.FileName;
                    FileHelper.WriteFile(file, contents, Encoding.UTF8);
                }
                this.saveFileDialog.InitialDirectory = prevRootPath;
                this.saveFileDialog.Filter = prevFilter;
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Saves all modified documents or opens a save file dialog
        /// </summary>
        public void SaveAll(Object sender, System.EventArgs e)
        {
            try 
            {
                this.savingMultiple = true;
                ITabbedDocument[] documents = this.Documents;
                ITabbedDocument active = this.CurrentDocument;
                for (Int32 i = 0; i < documents.Length; i++)
                {
                    ITabbedDocument current = documents[i];
                    if (current.IsEditable && current.IsModified)
                    {
                        if (current.IsUntitled)
                        {
                            this.saveFileDialog.FileName = current.FileName;
                            this.saveFileDialog.InitialDirectory = this.WorkingDirectory;
                            if (this.saveFileDialog.ShowDialog(this) == System.Windows.Forms.DialogResult.OK && this.saveFileDialog.FileName.Length != 0)
                            {
                                ButtonManager.AddNewReopenMenuItem(this.saveFileDialog.FileName);
                                current.Save(this.saveFileDialog.FileName);
                            }
                        }
                        else current.Save();
                    }
                }
                this.savingMultiple = false;
                active.Activate();
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Saves only modified documents with extension filter
        /// </summary>
        public void SaveAllModified(Object sender, System.EventArgs e)
        {
            try
            {
                String filter = "*";
                this.savingMultiple = true;
                ToolStripItem button = (ToolStripItem)sender;
                filter = ((ItemData)button.Tag).Tag + filter;
                ITabbedDocument[] documents = this.Documents;
                ITabbedDocument active = this.CurrentDocument;
                for (Int32 i = 0; i < documents.Length; i++)
                {
                    ITabbedDocument current = documents[i];
                    if (current.IsEditable && current.IsModified && !current.IsUntitled && current.Text.EndsWithOrdinal(filter))
                    {
                        current.Save();
                        current.IsModified = false;
                    }
                }
                this.savingMultiple = false;
                active.Activate();
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Closes the current document
        /// </summary>
        public void Close(Object sender, System.EventArgs e)
        {
            this.CurrentDocument.Close();
        }

        /// <summary>
        /// Closes all open documents in the current pane
        /// </summary>
        public void CloseAll(Object sender, System.EventArgs e)
        {
            this.CloseAllDocuments(false, true);
        }

        /// <summary>
        /// Closes all open documents except the current in the current pane
        /// </summary>
        public void CloseOthers(Object sender, System.EventArgs e)
        {
            this.CloseAllDocuments(true, true);
        }

        /// <summary>
        /// Exits the application
        /// </summary>
        public void Exit(Object sender, System.EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Duplicates the current document
        /// </summary>
        public void Duplicate(Object sender, System.EventArgs e)
        {
            ScintillaControl sci = Globals.SciControl;
            String extension = Path.GetExtension(sci.FileName);
            String filename = DocumentManager.GetNewDocumentName(extension);
            DockContent document = this.CreateEditableDocument(filename, sci.Text, sci.Encoding.CodePage);
            ((TabbedDocument)document).IsModified = true;
        }

        /// <summary>
        /// Reverts the document to the default state
        /// </summary>
        public void Revert(Object sender, System.EventArgs e)
        {
            this.CurrentDocument.Revert(true);
        }

        /// <summary>
        /// Reloads the current document
        /// </summary>
        public void Reload(Object sender, System.EventArgs e)
        {
            this.CurrentDocument.Reload(true);
        }

        /// <summary>
        /// Prints the current document
        /// </summary>
        public void Print(Object sender, System.EventArgs e)
        {
            if (Globals.SciControl.TextLength == 0)
            {
                String message = TextHelper.GetString("Info.NothingToPrint");
                ErrorManager.ShowInfo(message);
                return;
            }
            try
            {
                this.printDialog.PrinterSettings = PrintingManager.GetPrinterSettings();
                this.printDialog.Document = PrintingManager.CreatePrintDocument();
                if (this.printDialog.ShowDialog(this) == DialogResult.OK)
                {
                    this.printDialog.Document.Print();
                }
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Views the current print document
        /// </summary>
        public void PrintPreview(Object sender, System.EventArgs e)
        {
            if (Globals.SciControl.TextLength == 0)
            {
                String message = TextHelper.GetString("Info.NothingToPrint");
                ErrorManager.ShowInfo(message);
                return;
            }
            try
            {
                this.printDialog.PrinterSettings = PrintingManager.GetPrinterSettings();
                this.printPreviewDialog.Document = PrintingManager.CreatePrintDocument();
                this.printPreviewDialog.ShowDialog(this);
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Opens a goto dialog
        /// </summary>
        public void GoTo(Object sender, System.EventArgs e)
        {
            if (!this.gotoDialog.Visible) this.gotoDialog.Show();
            else this.gotoDialog.Activate();
        }

        /// <summary>
        /// Displays the next result
        /// </summary>
        public void FindNext(Object sender, System.EventArgs e)
        {
            Boolean update = !Globals.Settings.DisableFindTextUpdating;
            Boolean simple = !Globals.Settings.DisableSimpleQuickFind && !this.quickFind.Visible;
            this.frInDocDialog.FindNext(true, update, simple);
        }

        /// <summary>
        /// Displays the previous result
        /// </summary>
        public void FindPrevious(Object sender, System.EventArgs e)
        {
            Boolean update = !Globals.Settings.DisableFindTextUpdating;
            Boolean simple = !Globals.Settings.DisableSimpleQuickFind && !this.quickFind.Visible;
            this.frInDocDialog.FindNext(false, update, simple);
        }

        /// <summary>
        /// Opens a find and replace dialog
        /// </summary>
        public void FindAndReplace(Object sender, System.EventArgs e)
        {
            if (!this.frInDocDialog.Visible) this.frInDocDialog.Show();
            else
            {
                this.frInDocDialog.InitializeFindText();
                this.frInDocDialog.Activate();
            }
        }

        /// <summary>
        /// Opens a find and replace dialog with a location
        /// </summary>
        public void FindAndReplaceFrom(Object sender, System.EventArgs e)
        {
            ToolStripItem button = (ToolStripItem)sender;
            String file = ((ItemData)button.Tag).Tag;
            this.BeginInvoke((MethodInvoker)delegate
            {
                OpenEditableDocument(file);
            });
            if (!this.frInDocDialog.Visible) this.frInDocDialog.Show();
            else this.frInDocDialog.Activate();
        }

        /// <summary>
        /// Opens a find and replace in files dialog
        /// </summary>
        public void FindAndReplaceInFiles(Object sender, System.EventArgs e)
        {
            if (!this.frInFilesDialog.Visible) this.frInFilesDialog.Show();
            else
            {
                this.frInFilesDialog.UpdateFindText();
                this.frInFilesDialog.Activate();
            }
        }

        /// <summary>
        /// Opens a find and replace in files dialog with a location
        /// </summary>
        public void FindAndReplaceInFilesFrom(Object sender, System.EventArgs e)
        {
            ToolStripItem button = (ToolStripItem)sender;
            String path = ((ItemData)button.Tag).Tag;
            if (!this.frInFilesDialog.Visible) this.frInFilesDialog.Show(); // Show first..
            else this.frInFilesDialog.Activate();
            this.frInFilesDialog.SetFindPath(path);
        }

        /// <summary>
        /// Shows the quick find control
        /// </summary>
        public void QuickFind(Object sender, System.EventArgs e)
        {
            this.quickFind.ShowControl();
        }

        /// <summary>
        /// Opens the edit shortcut dialog
        /// </summary>
        public void EditShortcuts(Object sender, System.EventArgs e)
        {
            ShortcutDialog.Show();
        }

        /// <summary>
        /// Opens the edit snippet dialog
        /// </summary>
        public void EditSnippets(Object sender, System.EventArgs e)
        {
            SnippetDialog.Show();
        }

        /// <summary>
        /// Opens the edit syntax dialog
        /// </summary>
        public void EditSyntax(Object sender, System.EventArgs e)
        {
            EditorDialog.Show();
        }

        /// <summary>
        /// Opens the about dialog
        /// </summary>
        public void About(Object sender, System.EventArgs e)
        {
            AboutDialog.Show();
        }

        /// <summary>
        /// Opens the settings dialog
        /// </summary>
        public void ShowSettings(Object sender, System.EventArgs e)
        {
            SettingDialog.Show(DistroConfig.DISTRIBUTION_NAME, "");
        }

        /// <summary>
        /// Shows the application in fullscreen or normal mode
        /// </summary>
        public void ToggleFullScreen(Object sender, System.EventArgs e)
        {
            if (this.isFullScreen)
            {
                this.formState.Restore(this);
                if (this.appSettings.ViewToolBar) this.toolStrip.Visible = true;
                foreach (DockPane pane in this.dockPanel.Panes)
                {
                    if (this.fullScreenDocks[pane] != null)
                    {
                        pane.DockState = (DockState)this.fullScreenDocks[pane];
                    }
                }
                this.isFullScreen = false;
            } 
            else 
            {
                this.formState.Maximize(this);
                this.toolStrip.Visible = false;
                this.fullScreenDocks = new Hashtable();
                foreach (DockPane pane in this.dockPanel.Panes)
                {
                    this.fullScreenDocks[pane] = pane.DockState;
                    switch (pane.DockState)
                    {
                        case DockState.DockLeft:
                            pane.DockState = DockState.DockLeftAutoHide;
                            break;
                        case DockState.DockRight:
                            pane.DockState = DockState.DockRightAutoHide;
                            break;
                        case DockState.DockBottom:
                            pane.DockState = DockState.DockBottomAutoHide;
                            break;
                        case DockState.DockTop:
                            pane.DockState = DockState.DockTopAutoHide;
                            break;
                    }
                }
                this.isFullScreen = true;
            }
            ButtonManager.UpdateFlaggedButtons();
        }

        /// <summary>
        /// Extracts a zip file by extending paths with fd args
        /// </summary>
        public void ExtractZip(Object sender, System.EventArgs e)
        {
            try 
            {
                String zipLog = String.Empty;
                String zipFile = String.Empty;
                Boolean requiresRestart = false;
                Boolean silentInstall = Silent;
                ToolStripItem button = (ToolStripItem)sender;
                String[] chunks = (((ItemData)button.Tag).Tag).Split(';');
                if (chunks.Length > 1)
                {
                    zipFile = chunks[0];
                    silentInstall = chunks[1] == "true";
                }
                else zipFile = chunks[0];
                if (!File.Exists(zipFile)) return; // Skip missing file...
                String caption = TextHelper.GetString("Title.ConfirmDialog");
                String message = TextHelper.GetString("Info.ZipConfirmExtract") + "\n" + zipFile;
                if (silentInstall || MessageBox.Show(message, caption, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                {
                    ZipEntry entry = null;
                    zipLog += "FDZ: " + zipFile + "\r\n";
                    ZipInputStream zis = new ZipInputStream(new FileStream(zipFile, FileMode.Open, FileAccess.Read));
                    while ((entry = zis.GetNextEntry()) != null)
                    {
                        Int32 size = 2048;
                        Byte[] data = new Byte[2048];
                        String fdpath = this.ProcessArgString(entry.Name, false).Replace("/", "\\");
                        if (entry.IsFile)
                        {
                            String ext = Path.GetExtension(fdpath);
                            if (File.Exists(fdpath) && (ext == ".dll" || ext == ".fdb" || ext == ".fdl"))
                            {
                                fdpath += ".new";
                                requiresRestart = true;
                            }
                            else if (ext == ".dll" || ext == ".fdb" || ext == ".fdl")
                            {
                                requiresRestart = true;
                            }
                            zipLog += "Extract: " + fdpath + "\r\n";
                            String dirPath = Path.GetDirectoryName(fdpath);
                            if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);
                            FileStream extracted = new FileStream(fdpath, FileMode.Create);
                            while (true)
                            {
                                size = zis.Read(data, 0, data.Length);
                                if (size > 0) extracted.Write(data, 0, size);
                                else break;
                            }
                            extracted.Close();
                            extracted.Dispose();
                        }
                        else if (!Directory.Exists(fdpath))
                        {
                            zipLog += "Create: " + fdpath + "\r\n";
                            Directory.CreateDirectory(fdpath);
                        }
                    }
                    String finish = TextHelper.GetString("Info.ZipExtractDone");
                    String restart = TextHelper.GetString("Info.RequiresRestart");
                    if (requiresRestart)
                    {
                        zipLog += "Restart required.\r\n";
                        if (!silentInstall) finish += "\n" + restart;
                        this.RestartRequired();
                    }
                    String logFile = Path.Combine(PathHelper.BaseDir, "Extensions.log");
                    File.AppendAllText(logFile, zipLog + "Done.\r\n\r\n", Encoding.UTF8);
                    if (!silentInstall) ErrorManager.ShowInfo(finish);
                }
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Removes a zip file by extending paths with fd args
        /// </summary>
        public void RemoveZip(Object sender, System.EventArgs e)
        {
            try
            {
                String zipLog = String.Empty;
                String zipFile = String.Empty;
                Boolean requiresRestart = false;
                Boolean silentRemove = Silent;
                List<String> removeDirs = new List<String>();
                ToolStripItem button = (ToolStripItem)sender;
                String[] chunks = (((ItemData)button.Tag).Tag).Split(';');
                if (chunks.Length > 1)
                {
                    zipFile = chunks[0];
                    silentRemove = chunks[1] == "true";
                }
                else zipFile = chunks[0];
                if (!File.Exists(zipFile)) return; // Skip missing file...
                String caption = TextHelper.GetString("Title.ConfirmDialog");
                String message = TextHelper.GetString("Info.ZipConfirmRemove") + "\n" + zipFile;
                if (silentRemove || MessageBox.Show(message, caption, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                {
                    ZipEntry entry = null;
                    zipLog += "FDZ: " + zipFile + "\r\n";
                    ZipInputStream zis = new ZipInputStream(new FileStream(zipFile, FileMode.Open, FileAccess.Read));
                    while ((entry = zis.GetNextEntry()) != null)
                    {
                        String fdpath = this.ProcessArgString(entry.Name, false).Replace("/", "\\");
                        if (entry.IsFile)
                        {
                            String ext = Path.GetExtension(fdpath);
                            if (File.Exists(fdpath))
                            {
                                zipLog += "Delete: " + fdpath + "\r\n";
                                if (ext == ".dll" || ext == ".fdb" || ext == ".fdl")
                                {
                                    requiresRestart = true;
                                    File.Copy(fdpath, fdpath + ".del", true);
                                }
                                else
                                {
                                    try { File.Delete(fdpath); }
                                    catch
                                    {
                                        requiresRestart = true;
                                        File.Copy(fdpath, fdpath + ".del", true);
                                    }
                                }
                            }
                        }
                        else removeDirs.Add(fdpath);
                    }
                    // Remove empty dirs
                    removeDirs.Reverse();
                    foreach (String dir in removeDirs)
                    {
                        if (FolderHelper.IsDirectoryEmpty(dir) && !this.DirIsImportant(dir))
                        {
                            zipLog += "Remove: " + dir + "\r\n";
                            try { Directory.Delete(dir); }
                            catch { /* NO ERRORS */ }
                        }
                    }
                    String finish = TextHelper.GetString("Info.ZipRemoveDone");
                    String restart = TextHelper.GetString("Info.RequiresRestart");
                    if (requiresRestart)
                    {
                        zipLog += "Restart required.\r\n";                        
                        if (!silentRemove) finish += "\n" + restart;
                        this.RestartRequired();
                    }
                    String logFile = Path.Combine(PathHelper.BaseDir, "Extensions.log");
                    File.AppendAllText(logFile, zipLog + "Done.\r\n\r\n", Encoding.UTF8);
                    if (!silentRemove) ErrorManager.ShowInfo(finish);
                }
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }
        private Boolean DirIsImportant(String dir)
        {
            String full = Path.GetDirectoryName(dir);
            String[] importants = new String[3] { PathHelper.UserPluginDir, PathHelper.UserLibraryDir, PathHelper.UserProjectsDir };
            return Array.IndexOf(importants, full) > -1;
        }

        /// <summary>
        /// Opens the browser with the specified file
        /// </summary>
        public void Browse(Object sender, System.EventArgs e)
        {
            Browser browser = new Browser();
            browser.Dock = DockStyle.Fill;
            if (sender != null)
            {
                ToolStripItem button = (ToolStripItem)sender;
                String url = this.ProcessArgString(((ItemData)button.Tag).Tag);
                this.CreateCustomDocument(browser);
                if (url.Trim() != "") browser.WebBrowser.Navigate(url);
                else browser.WebBrowser.GoHome();
            }
            else browser.WebBrowser.GoHome();
        }

        /// <summary>
        /// Opens the home page in browser
        /// </summary>
        public void ShowHome(Object sender, System.EventArgs e)
        {
            this.CallCommand("Browse", DistroConfig.DISTRIBUTION_HOME);
        }

        /// <summary>
        /// Opens the help page in browser
        /// </summary>
        public void ShowHelp(Object sender, System.EventArgs e)
        {
            this.CallCommand("Browse", DistroConfig.DISTRIBUTION_HELP);
        }

        /// <summary>
        /// Opens the arguments dialog
        /// </summary>
        public void ShowArguments(Object sender, System.EventArgs e)
        {
            ArgumentDialog.Show();
        }

        /// <summary>
        /// Checks for updates from flashdevelop.org
        /// </summary>
        public void CheckUpdates(Object sender, System.EventArgs e)
        {
            UpdateDialog.Show(false);
        }

        /// <summary>
        /// Toggles the currect document to and from split view
        /// </summary>
        public void ToggleSplitView(Object sender, System.EventArgs e)
        {
            if (this.CurrentDocument.IsEditable)
            {
                this.CurrentDocument.IsSplitted = !this.CurrentDocument.IsSplitted;
                ButtonManager.UpdateFlaggedButtons();
            }
        }

        /// <summary>
        /// Moves the user to the matching brace
        /// </summary>
        public void GoToMatchingBrace(Object sender, System.EventArgs e)
        {
            ScintillaControl sci = Globals.SciControl;
            Int32 curPos = sci.CurrentPos;
            Char brace = (Char)sci.CharAt(curPos);
            if (brace != '{' && brace != '[' && brace != '(' && brace != '}' && brace != ']' && brace != ')')
            {
                curPos = sci.CurrentPos - 1;
            }
            Int32 bracePosEnd = sci.BraceMatch(curPos);
            if (bracePosEnd != -1) sci.GotoPos(bracePosEnd + 1);
        }

        /// <summary>
        /// Adds or removes a bookmark
        /// </summary>
        public void ToggleBookmark(Object sender, System.EventArgs e)
        {
            ScintillaControl sci = Globals.SciControl;
            MarkerManager.ToggleMarker(sci, 0, sci.CurrentLine);
        }

        /// <summary>
        /// Moves the cursor to the next bookmark
        /// </summary>
        public void NextBookmark(Object sender, System.EventArgs e)
        {
            ScintillaControl sci = Globals.SciControl;
            MarkerManager.NextMarker(sci, 0, sci.CurrentLine);
        }

        /// <summary>
        /// Moves the cursor to the previous bookmark
        /// </summary>
        public void PrevBookmark(Object sender, System.EventArgs e)
        {
            ScintillaControl sci = Globals.SciControl;
            MarkerManager.PreviousMarker(sci, 0, sci.CurrentLine);
        }

        /// <summary>
        /// Removes all bookmarks
        /// </summary>
        public void ClearBookmarks(Object sender, System.EventArgs e)
        {
            Globals.SciControl.MarkerDeleteAll(0);
            UITools.Manager.MarkerChanged(Globals.SciControl, -1);
            ButtonManager.UpdateFlaggedButtons();
        }

        /// <summary>
        /// Converts all end of line characters
        /// </summary>
        public void ConvertEOL(Object sender, System.EventArgs e)
        {
            try
            {
                ToolStripItem button = (ToolStripItem)sender;
                ScintillaControl sci = Globals.SciControl;
                Int32 eolMode = Convert.ToInt32(((ItemData)button.Tag).Tag);
                sci.ConvertEOLs(eolMode); sci.EOLMode = eolMode;
                this.OnScintillaControlUpdateControl(sci);
                this.OnDocumentModify(this.CurrentDocument);
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Toggles the folding of the editor
        /// </summary>
        public void ToggleFold(Object sender, System.EventArgs e)
        {
            Int32 pos = Globals.SciControl.CurrentPos;
            Int32 line = Globals.SciControl.LineFromPosition(pos);
            Globals.SciControl.ToggleFold(line);
        }

        /// <summary>
        /// Toggles a boolean setting
        /// </summary>
        public void ToggleBooleanSetting(Object sender, System.EventArgs e)
        {
            try
            {
                ToolStripItem button = (ToolStripItem)sender;
                String settingKey = ((ItemData)button.Tag).Tag;
                Boolean value = (Boolean)this.appSettings.GetValue(settingKey);
                if (value) this.appSettings.SetValue(settingKey, false);
                else this.appSettings.SetValue(settingKey, true);
                this.ApplyAllSettings();
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Changes the encoding of the current document
        /// </summary>
        public void ChangeEncoding(Object sender, System.EventArgs e)
        {
            try
            {
                ToolStripItem button = (ToolStripItem)sender;
                ScintillaControl sci = Globals.SciControl;
                Int32 encMode = Convert.ToInt32(((ItemData)button.Tag).Tag);
                sci.Encoding = Encoding.GetEncoding(encMode);
                this.OnScintillaControlUpdateControl(sci);
                this.OnDocumentModify(this.CurrentDocument);
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        public void ToggleSaveBOM(Object sender, System.EventArgs e)
        {
            try
            {
                ScintillaControl sci = Globals.SciControl;
                sci.SaveBOM = !sci.SaveBOM;
                this.OnScintillaControlUpdateControl(sci);
                this.OnDocumentModify(this.CurrentDocument);
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Converts the encoding of the current document
        /// </summary>
        public void ConvertEncoding(Object sender, System.EventArgs e)
        {
            try
            {
                ToolStripItem button = (ToolStripItem)sender;
                ScintillaControl sci = Globals.SciControl;
                Int32 encMode = Convert.ToInt32(((ItemData)button.Tag).Tag);
                Int32 curMode = sci.Encoding.CodePage; // From current..
                String converted = DataConverter.ChangeEncoding(sci.Text, curMode, encMode);
                sci.Encoding = Encoding.GetEncoding(encMode);
                sci.Text = converted; // Set after codepage change
                this.OnScintillaControlUpdateControl(sci);
                this.OnDocumentModify(this.CurrentDocument);
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Inserts a snippet to the current position
        /// </summary>
        public void InsertSnippet(Object sender, System.EventArgs e)
        {
            try
            {
                ToolStripItem button = (ToolStripItem)sender;
                String word = (((ItemData)button.Tag).Tag);
                if (word != "null") SnippetManager.InsertTextByWord(word, false);
                else SnippetManager.InsertTextByWord(null, false);
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Inserts a new GUID to the editor
        /// </summary>
        public void InsertGUID(Object sender, System.EventArgs e)
        {
            String guid = Guid.NewGuid().ToString();
            Globals.SciControl.ReplaceSel(guid);
        }

        /// <summary>
        /// Inserts a custom hash to the editor
        /// </summary>
        public void InsertHash(Object sender, System.EventArgs e)
        {
            using (HashDialog cd = new HashDialog())
            {
                if (cd.ShowDialog() == DialogResult.OK)
                {
                    Globals.SciControl.ReplaceSel(cd.HashResultText);
                }
            }
        }

        /// <summary>
        /// Inserts a color to the editor
        /// </summary>
        public void InsertColor(Object sender, System.EventArgs e)
        {
            try
            {
                Boolean hasPrefix = true;
                Boolean isAsterisk = false;
                ScintillaControl sci = Globals.SciControl;
                if (sci.SelText.Length > 0)
                {
                    isAsterisk = sci.SelText.StartsWith('#');
                    if (sci.SelText.StartsWithOrdinal("0x") && sci.SelText.Length == 8)
                    {
                        Int32 convertedColor = DataConverter.StringToColor(sci.SelText);
                        this.colorDialog.Color = ColorTranslator.FromWin32(convertedColor);
                    }
                    else if (sci.SelText.StartsWith('#') && sci.SelText.Length == 7)
                    {
                        String foundColor = sci.SelText.Replace("#", "0x");
                        Int32 convertedColor = DataConverter.StringToColor(foundColor);
                        this.colorDialog.Color = ColorTranslator.FromWin32(convertedColor);
                    }
                    else if (sci.SelText.Length == 6)
                    {
                        hasPrefix = false;
                        Int32 convertedColor = DataConverter.StringToColor("0x" + sci.SelText);
                        this.colorDialog.Color = ColorTranslator.FromWin32(convertedColor);
                    }
                }
                if (this.colorDialog.ShowDialog(this) == DialogResult.OK)
                {
                    String colorText = DataConverter.ColorToHex(this.colorDialog.Color);
                    if (!hasPrefix) colorText = colorText.Replace("0x", "");
                    else if (isAsterisk || sci.ConfigurationLanguage == "css")
                    {
                        colorText = colorText.Replace("0x", "#");
                    }
                    sci.ReplaceSel(colorText);
                }
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Inserts the file info to the current position
        /// </summary>
        public void InsertFileDetails(Object sender, System.EventArgs e)
        {
            try
            {
                FileInfo fileInfo = new FileInfo(this.CurrentDocument.FileName);
                String message = TextHelper.GetString("Info.FileDetails");
                String newline = LineEndDetector.GetNewLineMarker(Globals.SciControl.EOLMode);
                String path = fileInfo.FullName.ToString();
                String created = fileInfo.CreationTime.ToString();
                String modified = fileInfo.LastWriteTime.ToString();
                String size = fileInfo.Length.ToString();
                String info = String.Format(message, newline, path, created, modified, size);
                Globals.SciControl.ReplaceSel(info);
            }
            catch
            {
                String message = TextHelper.GetString("Info.NoFileInfoAvailable");
                ErrorManager.ShowInfo(message);
            }
        }

        /// <summary>
        /// Inserts a global timestamp to the current position
        /// </summary>
        public void InsertTimestamp(Object sender, System.EventArgs e)
        {
            try
            {
                DateTime dateTime = DateTime.Now;
                ToolStripItem button = (ToolStripItem)sender;
                String date = (((ItemData)button.Tag).Tag);
                String currentDate = dateTime.ToString(date);
                Globals.SciControl.ReplaceSel(currentDate);
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Inserts text from the specified file to the current position
        /// </summary>
        public void InsertFile(Object sender, System.EventArgs e)
        {
            try
            {
                ToolStripItem button = (ToolStripItem)sender;
                String file = this.ProcessArgString(((ItemData)button.Tag).Tag);
                if (File.Exists(file))
                {
                    Encoding to = Globals.SciControl.Encoding;
                    EncodingFileInfo info = FileHelper.GetEncodingFileInfo(file);
                    if (info.CodePage == -1) return; // If the file is locked, stop.
                    String contents = DataConverter.ChangeEncoding(info.Contents, info.CodePage, to.CodePage);
                    Globals.SciControl.ReplaceSel(contents);
                }
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Changes the language of the current document
        /// </summary>
        public void ChangeSyntax(Object sender, System.EventArgs e)
        {
            try
            {
                ScintillaControl sci = Globals.SciControl;
                ToolStripItem button = (ToolStripItem)sender;
                string language = ((ItemData) button.Tag).Tag;
                if (sci.ConfigurationLanguage.Equals(language)) return; // already using this syntax
                ScintillaManager.ChangeSyntax(language, sci);
                string extension = sci.GetFileExtension();
                if (!string.IsNullOrEmpty(extension))
                {
                    string title = TextHelper.GetString("Title.RememberExtensionDialog"); 
                    string message = TextHelper.GetString("Info.RememberExtensionDialog"); 
                    if (MessageBox.Show(message, title, MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        sci.SaveExtensionToSyntaxConfig(extension);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Sorts the currently selected lines
        /// </summary>
        public void SortLines(Object sender, System.EventArgs e)
        {
            ScintillaControl sci = Globals.SciControl;
            Int32 curLine = sci.LineFromPosition(sci.SelectionStart);
            Int32 endLine = sci.LineFromPosition(sci.SelectionEnd);
            List<String> lines = new List<String>();
            for (Int32 line = curLine; line < endLine + 1; ++line)
            {
                lines.Add(sci.GetLine(line));
            }
            lines.Sort(CompareLines);
            StringBuilder result = new StringBuilder();
            foreach (String s in lines)
            {
                result.Append(s);
            }
            Int32 selStart = sci.PositionFromLine(curLine);
            Int32 selEnd = sci.PositionFromLine(endLine) + sci.MBSafeTextLength(sci.GetLine(endLine));
            sci.SetSel(selStart, selEnd);
            sci.ReplaceSel(result.ToString());
        }

        /// <summary>
        /// Sorts the currently selected lines in groups
        /// </summary>
        public void SortLineGroups(Object sender, System.EventArgs e)
        {
            ScintillaControl sci = Globals.SciControl;
            Int32 curLine = sci.LineFromPosition(sci.SelectionStart);
            Int32 endLine = sci.LineFromPosition(sci.SelectionEnd);
            List<List<String>> lineLists = new List<List<String>>();
            List<String> curList = new List<String>();
            lineLists.Add(curList);
            for (Int32 line = curLine; line < endLine + 1; ++line)
            {
                String lineText = sci.GetLine(line);
                if (lineText.Trim() == "")
                {
                    curList.Sort(CompareLines);
                    curList.Add(lineText);
                    curList = new List<String>();
                    lineLists.Add(curList);
                    continue;
                }
                curList.Add(lineText);
            }
            curList.Sort(CompareLines);
            StringBuilder result = new StringBuilder();
            foreach (List<String> l in lineLists)
            {
                foreach (String s in l)
                {
                    result.Append(s);
                }
            }
            Int32 selStart = sci.PositionFromLine(curLine);
            Int32 selEnd = sci.PositionFromLine(endLine) + sci.MBSafeTextLength(sci.GetLine(endLine));
            sci.SetSel(selStart, selEnd);
            sci.ReplaceSel(result.ToString());
        }
        private static Int32 CompareLines(String a, String b)
        {
            Char[] whitespace = {'\t', ' '};
            a = a.TrimStart(whitespace);
            b = b.TrimStart(whitespace);
            return a.CompareTo(b);
        }

        /// <summary>
        /// Line-comment the selected text
        /// </summary>
        public void CommentLine(Object sender, System.EventArgs e)
        {
            ScintillaControl sci = Globals.SciControl;
            String lineComment = ScintillaManager.GetLineComment(sci.ConfigurationLanguage);
            if (lineComment.Length == 0) return;
            Int32 position = sci.CurrentPos;
            Int32 curLine = sci.LineFromPosition(position);
            Int32 startPosInLine = position - sci.PositionFromLine(curLine);
            Int32 finalPos = position;
            Int32 startLine = sci.LineFromPosition(sci.SelectionStart);
            Int32 line = startLine;
            Int32 endLine = sci.LineFromPosition(sci.SelectionEnd);
            if (endLine > line && curLine == endLine && startPosInLine == 0)
            {
                curLine--;
                endLine--;
                finalPos = sci.PositionFromLine(curLine);
            }
            Boolean afterIndent = this.appSettings.LineCommentsAfterIndent;
            sci.BeginUndoAction();
            try
            {
                while (line <= endLine)
                {
                    position = (afterIndent) ? sci.LineIndentPosition(line) : sci.PositionFromLine(line);
                    sci.InsertText(position, lineComment);
                    if (line == curLine)
                    {
                        finalPos = sci.PositionFromLine(line) + startPosInLine;
                        if (finalPos >= position) finalPos += lineComment.Length;
                    }
                    line++;
                }
                sci.SetSel(finalPos, finalPos);
                if (startLine == endLine && (endLine < sci.LineCount) && this.appSettings.MoveCursorAfterComment)
                {
                    sci.LineDown();
                }
            }
            finally
            {
                sci.EndUndoAction();
            }
            sci.Colourise(0, -1);
        }

        /// <summary>
        /// Line-uncomment the selected text
        /// </summary>
        public void UncommentLine(Object sender, System.EventArgs e)
        {
            ScintillaControl sci = Globals.SciControl;
            String lineComment = ScintillaManager.GetLineComment(sci.ConfigurationLanguage);
            if (lineComment.Length == 0) return;
            Int32 position = sci.CurrentPos;
            Int32 curLine = sci.LineFromPosition(position);
            Int32 startPosInLine = position - sci.PositionFromLine(curLine);
            Int32 finalPos = position;
            Int32 startLine = sci.LineFromPosition(sci.SelectionStart);
            Int32 line = startLine;
            Int32 endLine = sci.LineFromPosition(sci.SelectionEnd);
            if (endLine > line && curLine == endLine && startPosInLine == 0)
            {
                curLine--;
                endLine--;
                finalPos = sci.PositionFromLine(curLine);
            }
            String text;
            sci.BeginUndoAction();
            try
            {
                while (line <= endLine)
                {
                    if (sci.LineLength(line) == 0) text = "";
                    else text = sci.GetLine(line).TrimStart();
                    if (text.StartsWithOrdinal(lineComment))
                    {
                        position = sci.LineIndentPosition(line);
                        sci.SetSel(position, position + lineComment.Length);
                        sci.ReplaceSel("");
                        if (line == curLine)
                        {
                            finalPos = sci.PositionFromLine(line) + Math.Min(startPosInLine, sci.LineLength(line));
                            if (finalPos >= position + lineComment.Length) finalPos -= lineComment.Length;
                        }
                    }
                    line++;
                }
                sci.SetSel(finalPos, finalPos); 
                if (startLine == endLine && (endLine < sci.LineCount) && this.appSettings.MoveCursorAfterComment)
                {
                    sci.LineDown();
                }
            }
            finally
            {
                sci.EndUndoAction();
            }
        }

        /// <summary>
        /// Box-comments or uncomments the selected text
        /// </summary>
        public void CommentSelection(Object sender, System.EventArgs e)
        {
            CommentSelection();
        }
        private bool? CommentSelection()
        {
            ScintillaControl sci = Globals.SciControl;
            Int32 selEnd = sci.SelectionEnd;
            Int32 selStart = sci.SelectionStart;
            String commentEnd = ScintillaManager.GetCommentEnd(sci.ConfigurationLanguage);
            String commentStart = ScintillaManager.GetCommentStart(sci.ConfigurationLanguage);
            if (sci.SelText.StartsWithOrdinal(commentStart) && sci.SelText.EndsWithOrdinal(commentEnd))
            {
                sci.BeginUndoAction();
                try
                {
                    Int32 indexLength = sci.SelText.Length - commentStart.Length - commentEnd.Length;
                    String withoutComment = sci.SelText.Substring(commentStart.Length, indexLength);
                    sci.ReplaceSel(withoutComment);
                }
                finally
                {
                    sci.EndUndoAction();
                }
                return false;
            }
            else if (sci.SelText.Length > 0)
            {
                sci.BeginUndoAction();
                try
                {
                    sci.InsertText(selStart, commentStart);
                    sci.InsertText(selEnd + commentStart.Length, commentEnd);
                }
                finally
                {
                    sci.EndUndoAction();
                }
                return true;
            }
            return null;
        }

        /// <summary>
        /// Uncomments a comment block from current position
        /// </summary>
        public void UncommentBlock(Object sender, System.EventArgs e)
        {
            ScintillaControl sci = Globals.SciControl;
            sci.Colourise(0, -1); // update coloring
            Int32 selEnd = sci.SelectionEnd;
            Int32 selStart = sci.SelectionStart;
            String commentEnd = ScintillaManager.GetCommentEnd(sci.ConfigurationLanguage);
            String commentStart = ScintillaManager.GetCommentStart(sci.ConfigurationLanguage);
            if ((sci.PositionIsOnComment(selStart) && (sci.PositionIsOnComment(selEnd)) || sci.PositionIsOnComment(selEnd - 1)) || (selEnd == selStart && sci.PositionIsOnComment(selStart - 1)))
            {
                sci.BeginUndoAction();
                try
                {
                    Int32 lexer = sci.Lexer;
                    // find selection bounds
                    if (!sci.PositionIsOnComment(selStart, lexer)) selStart--;
                    Int32 scrollTop = sci.FirstVisibleLine;
                    Int32 initPos = sci.CurrentPos;
                    Int32 start = selStart;
                    while (start > 0 && sci.PositionIsOnComment(start, lexer)) start--;
                    Int32 end = selEnd;
                    Int32 length = sci.TextLength;
                    while (end < length && sci.PositionIsOnComment(end, lexer)) end++;
                    sci.SetSel(start + 1, end);
                    // remove comment
                    String selText = sci.SelText;
                    if (selText.StartsWithOrdinal(commentStart) && selText.EndsWithOrdinal(commentEnd))
                    {
                        sci.SetSel(end - commentEnd.Length, end);
                        sci.ReplaceSel("");
                        sci.SetSel(start + 1, start + commentStart.Length + 1);
                        sci.ReplaceSel("");
                        // fix caret pos
                        if (initPos > end - commentEnd.Length) initPos = end - commentEnd.Length;
                        if (initPos <= start + commentStart.Length) initPos = start + 1;
                        else initPos -= commentStart.Length;
                    }
                    sci.SetSel(initPos, initPos);
                    if (scrollTop != sci.FirstVisibleLine) // fix page scrolling
                    {
                        sci.LineScroll(0, scrollTop - sci.FirstVisibleLine);
                    }
                }
                finally
                {
                    sci.EndUndoAction();
                }
            }
        }

        /// <summary>
        /// Toggles the line comment in a smart way
        /// </summary>
        public void ToggleLineComment(Object sender, System.EventArgs e)
        {
            ScintillaControl sci = Globals.SciControl;
            String lineComment = ScintillaManager.GetLineComment(sci.ConfigurationLanguage);
            Int32 position = sci.CurrentPos;
            // try doing a block comment on the current line instead (xml, html...)
            if (lineComment == "")
            {
                ToggleBlockOnCurrentLine(sci);
                return;
            }
            Int32 curLine = sci.LineFromPosition(position);
            Int32 startPosInLine = position - sci.PositionFromLine(curLine);
            Int32 startLine = sci.LineFromPosition(sci.SelectionStart);
            Int32 line = startLine;
            Int32 endLine = sci.LineFromPosition(sci.SelectionEnd);
            if (endLine > line && curLine == endLine && startPosInLine == 0)
            {
                curLine--;
                endLine--;
            }
            String text;
            Boolean containsCodeLine = false;
            while (line <= endLine)
            {
                if (sci.LineLength(line) == 0) text = "";
                else text = sci.GetLine(line).TrimStart();
                if (!text.StartsWithOrdinal(lineComment))
                {
                    containsCodeLine = true;
                    break;
                }
                line++;
            }
            if (containsCodeLine) this.CommentLine(null, null);
            else this.UncommentLine(null, null);
        }
        private void ToggleBlockOnCurrentLine(ScintillaControl sci)
        {
            Int32 selStart = sci.SelectionStart;
            Int32 indentPos = sci.LineIndentPosition(sci.CurrentLine);
            Int32 lineEndPos = sci.LineEndPosition(sci.CurrentLine);
            bool afterBlockStart = sci.CurrentPos > indentPos;
            bool afterBlockEnd = sci.CurrentPos >= lineEndPos;
            sci.SelectionStart = indentPos;
            sci.SelectionEnd = lineEndPos;
            bool ? added = CommentSelection();
            if (added == null) return;
            int factor = (bool)added ? 1 : -1;
            String commentEnd = ScintillaManager.GetCommentEnd(sci.ConfigurationLanguage);
            String commentStart = ScintillaManager.GetCommentStart(sci.ConfigurationLanguage);
            // preserve cursor pos
            if (afterBlockStart) selStart += commentStart.Length * factor;
            if (afterBlockEnd) selStart += commentEnd.Length * factor;
            sci.SetSel(selStart, selStart);
        }

        /// <summary>
        /// Toggles the block comment in a smart way
        /// </summary>
        public void ToggleBlockComment(Object sender, System.EventArgs e)
        {
            ScintillaControl sci = Globals.SciControl;
            if (sci.SelText.Length > 0) this.CommentSelection(null, null);
            else this.UncommentBlock(null, null);
        }

        /// <summary>
        /// Lets user browse for an theme file
        /// </summary>
        public void SelectTheme(Object sender, System.EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.InitialDirectory = PathHelper.ThemesDir;
                ofd.Title = " " + TextHelper.GetString("Title.OpenFileDialog");
                ofd.Filter = TextHelper.GetString("Info.ThemesFilter");
                if (ofd.ShowDialog(this) == DialogResult.OK)
                {
                    String ext = Path.GetExtension(ofd.FileName).ToLower();
                    if (ext == ".fdi")
                    {
                        ThemeManager.LoadTheme(ofd.FileName);
                        ThemeManager.WalkControls(this);
                    }
                    else
                    {
                        this.CallCommand("ExtractZip", ofd.FileName + ";true");
                        String currentTheme = Path.Combine(PathHelper.ThemesDir, "CURRENT");
                        if (File.Exists(currentTheme)) ThemeManager.LoadTheme(currentTheme);
                        ThemeManager.WalkControls(this);
                        this.RefreshSciConfig();
                    }
                }
            }
        }

        /// <summary>
        /// Loads an theme file and applies it
        /// </summary>
        public void LoadThemeFile(Object sender, System.EventArgs e)
        {
            ToolStripItem button = (ToolStripItem)sender;
            String file = this.ProcessArgString(((ItemData)button.Tag).Tag);
            ThemeManager.LoadTheme(file);
        }

        /// <summary>
        /// Invokes the specified registered menu item
        /// </summary>
        public void InvokeMenuItem(Object sender, System.EventArgs e)
        {
            try
            {
                ToolStripItem button = (ToolStripItem)sender;
                String registeredItem = ((ItemData)button.Tag).Tag;
                ShortcutItem item = ShortcutManager.GetRegisteredItem(registeredItem);
                if (item.Item != null) item.Item.PerformClick();
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Calls a ScintillaControl command
        /// </summary>
        public void ScintillaCommand(Object sender, System.EventArgs e)
        {
            try
            {
                ToolStripItem button = (ToolStripItem)sender;
                String command = ((ItemData)button.Tag).Tag;
                Type mfType = Globals.SciControl.GetType();
                MethodInfo method = mfType.GetMethod(command, new Type[0]);
                method.Invoke(Globals.SciControl, null);
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Calls a custom plugin command
        /// </summary>
        public void PluginCommand(object sender, EventArgs e)
        {
            try
            {
                var item = (ToolStripItem) sender;
                string[] args = ((ItemData) item.Tag).Tag.Split(new[] { ';' }, 2);
                string action = args[0]; // Action of the command
                string data = args.Length > 1 ? args[1] : null;
                EventManager.DispatchEvent(this, new DataEvent(EventType.Command, action, data));
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Calls a normal MainForm method
        /// </summary>
        public Boolean CallCommand(String command, String args)
        {
            try
            {
                var method = this.GetType().GetMethod(command);
                if (method == null) throw new MethodAccessException();
                var item = new ToolStripMenuItem();
                item.Tag = new ItemData(null, args, null); // Tag is used for args
                method.Invoke(this, new[] { item, null });
                return true;
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
                return false;
            }
        }

        /// <summary>
        /// Runs a simple process
        /// </summary>
        public void RunProcess(Object sender, System.EventArgs e)
        {
            try
            {
                ToolStripItem button = (ToolStripItem)sender;
                String args = this.ProcessArgString(((ItemData)button.Tag).Tag);
                Int32 position = args.IndexOf(';'); // Position of the arguments
                NotifyEvent ne = new NotifyEvent(EventType.ProcessStart);
                EventManager.DispatchEvent(this, ne);
                if (position > -1)
                {
                    String message = TextHelper.GetString("Info.RunningProcess");
                    TraceManager.Add(message + " " + args.Substring(0, position) + " " + args.Substring(position + 1), (Int32)TraceType.ProcessStart);
                    ProcessStartInfo psi = new ProcessStartInfo();
                    psi.WorkingDirectory = this.WorkingDirectory;
                    psi.Arguments = args.Substring(position + 1);
                    psi.FileName = args.Substring(0, position);
                    ProcessHelper.StartAsync(psi);
                }
                else
                {
                    String message = TextHelper.GetString("Info.RunningProcess");
                    TraceManager.Add(message + " " + args, (Int32)TraceType.ProcessStart);
                    if (args.ToLower().EndsWithOrdinal(".bat"))
                    {
                        Process bp = new Process();
                        bp.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        bp.StartInfo.FileName = @args;
                        bp.Start();
                    }
                    else
                    {
                        ProcessStartInfo psi = new ProcessStartInfo(args);
                        psi.WorkingDirectory = this.WorkingDirectory;
                        ProcessHelper.StartAsync(psi);
                    }
                }
                ButtonManager.UpdateFlaggedButtons();
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Runs a process and tracks it's progress
        /// </summary>
        public void RunProcessCaptured(Object sender, System.EventArgs e)
        {
            try
            {
                ToolStripItem button = (ToolStripItem)sender;
                String args = this.ProcessArgString(((ItemData)button.Tag).Tag);
                Int32 position = args.IndexOf(';'); // Position of the arguments
                NotifyEvent ne = new NotifyEvent(EventType.ProcessStart);
                EventManager.DispatchEvent(this, ne);
                if (position < 0)
                {
                    String message = TextHelper.GetString("Info.NotEnoughArguments");
                    TraceManager.Add(message + " " + args, (Int32)TraceType.Error);
                    return;
                }
                String message2 = TextHelper.GetString("Info.RunningProcess");
                TraceManager.Add(message2 + " " + args.Substring(0, position) + " " + args.Substring(position + 1), (Int32)TraceType.ProcessStart);
                this.processRunner.Run(args.Substring(0, position), args.Substring(position + 1));
                ButtonManager.UpdateFlaggedButtons();
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Handles the incoming info output
        /// </summary>
        private void ProcessOutput(Object sender, String line)
        {
            TraceManager.AddAsync(line, (Int32)TraceType.Info);
        }

        /// <summary>
        /// Handles the incoming error output
        /// </summary> 
        private void ProcessError(Object sender, String line)
        {
            TraceManager.AddAsync(line, (Int32)TraceType.ProcessError);
        }

        /// <summary>
        /// Handles the ending of a process
        /// </summary>
        private void ProcessEnded(Object sender, Int32 exitCode)
        {
            if (this.InvokeRequired) this.BeginInvoke((MethodInvoker)delegate { this.ProcessEnded(sender, exitCode); });
            else
            {
                String result = String.Format("Done({0})", exitCode);
                TraceManager.Add(result, (Int32)TraceType.ProcessEnd);
                TextEvent te = new TextEvent(EventType.ProcessEnd, result);
                EventManager.DispatchEvent(this, te);
                ButtonManager.UpdateFlaggedButtons();
            }
        }

        /// <summary>
        /// Stop the currently running process
        /// </summary>
        public void KillProcess(Object sender, System.EventArgs e)
        {
            this.KillProcess();
        }

        /// <summary>
        /// Stop the currently running process
        /// </summary>
        public void KillProcess()
        {
            if (this.processRunner.IsRunning)
            {
                this.processRunner.KillProcess();
            }
        }

        /// <summary>
        /// Backups users setting files to a FDZ file
        /// </summary>
        public void BackupSettings(Object sender, EventArgs e)
        {
            try
            {
                using (SaveFileDialog sfd = new SaveFileDialog())
                {
                    sfd.AddExtension = true; sfd.DefaultExt = "fdz";
                    sfd.Filter = TextHelper.GetString("FlashDevelop.Info.ZipFilter");
                    String dirMarker = "\\" + DistroConfig.DISTRIBUTION_NAME + "\\";
                    if (sfd.ShowDialog(this) == DialogResult.OK)
                    {
                        List<String> settingFiles = new List<String>();
                        ZipFile zipFile = ZipFile.Create(sfd.FileName);
                        settingFiles.AddRange(Directory.GetFiles(PathHelper.DataDir, "*.*", SearchOption.AllDirectories));
                        settingFiles.AddRange(Directory.GetFiles(PathHelper.SnippetDir, "*.*", SearchOption.AllDirectories));
                        settingFiles.AddRange(Directory.GetFiles(PathHelper.SettingDir, "*.*", SearchOption.AllDirectories));
                        settingFiles.AddRange(Directory.GetFiles(PathHelper.TemplateDir, "*.*", SearchOption.AllDirectories));
                        settingFiles.AddRange(Directory.GetFiles(PathHelper.UserLibraryDir, "*.*", SearchOption.AllDirectories));
                        settingFiles.AddRange(Directory.GetFiles(PathHelper.UserProjectsDir, "*.*", SearchOption.AllDirectories));
                        zipFile.BeginUpdate();
                        foreach (String settingFile in settingFiles)
                        {
                            Int32 index = settingFile.IndexOfOrdinal(dirMarker) + dirMarker.Length;
                            zipFile.Add(settingFile, "$(BaseDir)\\" + settingFile.Substring(index));
                        }
                        zipFile.CommitUpdate();
                        zipFile.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Executes the specified C# Script file
        /// </summary>
        public void ExecuteScript(Object sender, EventArgs e)
        {
            ToolStripItem button = (ToolStripItem)sender;
            String file = this.ProcessArgString(((ItemData)button.Tag).Tag);
            try
            {
                Host host = new Host();
                String[] args = file.Split(new Char[1]{';'});
                if (args.Length == 1 || String.IsNullOrEmpty(args[1])) return; // no file selected / the open file dialog was cancelled
                if (args[0] == "Internal") host.ExecuteScriptInternal(args[1], false);
                else if (args[0] == "Development") host.ExecuteScriptInternal(args[1], true);
                else host.ExecuteScriptExternal(file);
            }
            catch (Exception ex)
            {
                String message = TextHelper.GetString("Info.CouldNotExecuteScript");
                ErrorManager.ShowWarning(message + "\r\n" + ex.Message, null);
            }
        }

        /// <summary>
        /// Test the controls in a dedicated form
        /// </summary>
        public void TestControls(Object sender, EventArgs e)
        {
            ControlDialog cd = new ControlDialog();
            cd.Show(this);
        }

        /// <summary>
        /// Outputs the supplied argument string
        /// </summary>
        public void Debug(Object sender, EventArgs e)
        {
            ToolStripItem button = (ToolStripItem)sender;
            String args = this.ProcessArgString(((ItemData)button.Tag).Tag);
            if (args == String.Empty) ErrorManager.ShowError(new Exception("Debug"));
            else ErrorManager.ShowInfo(args);
        }

        /// <summary>
        /// Restarts FlashDevelop
        /// </summary>
        public void Restart(Object sender, EventArgs e)
        {
            if (this.GetInstanceCount() == 1)
            {
                this.restartRequested = true;
                this.Close();
            }
        }

        #endregion

    }

    #region Script Host

    public class Host : MarshalByRefObject
    {
        /// <summary>
        /// Executes the script in a seperate appdomain and then unloads it
        /// NOTE: This is more suitable for one pass processes
        /// </summary>
        public void ExecuteScriptExternal(String script)
        {
            if (!File.Exists(script)) throw new FileNotFoundException();
            using (AsmHelper helper = new AsmHelper(CSScript.CompileFile(script, null, true), null, true))
            {
                helper.Invoke("*.Execute");
            }
        }

        /// <summary>
        /// Executes the script and adds it to the current app domain
        /// NOTE: This locks the assembly script file
        /// </summary>
        public void ExecuteScriptInternal(String script, Boolean random)
        {
            if (!File.Exists(script)) throw new FileNotFoundException();
            String file = random ? Path.GetTempFileName() : null;
            AsmHelper helper = new AsmHelper(CSScript.Load(script, file, false, null));
            helper.Invoke("*.Execute");
        }

    }

    #endregion

}
