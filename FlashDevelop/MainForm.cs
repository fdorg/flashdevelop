using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
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
using PluginCore.Collections;
using PluginCore.Controls;
using PluginCore.Helpers;
using PluginCore.Localization;
using PluginCore.Managers;
using PluginCore.Utilities;
using ScintillaNet;
using ScintillaNet.Configuration;
using WeifenLuo.WinFormsUI.Docking;

namespace FlashDevelop
{
    public class MainForm : Form, IMainForm, IMessageFilter
    {
        #region Constructor

        public MainForm()
        {
            Globals.MainForm = this;
            PluginBase.Initialize(this);
            DoubleBuffered = true;
            InitializeErrorLog();
            InitializeSettings();
            InitializeLocalization();
            if (InitializeFirstRun() != DialogResult.Abort)
            {
                // Suspend layout!
                SuspendLayout();
                InitializeConfig();
                InitializeRendering();
                InitializeComponents();
                InitializeProcessRunner();
                InitializeSmartDialogs();
                InitializeMainForm();
                InitializeGraphics();
                Application.AddMessageFilter(this);
            }
            else Load += MainFormLoaded;
        }

        /// <summary>
        /// Initializes some extra error logging
        /// </summary>
        void InitializeErrorLog() => AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

        /// <summary>
        /// Handles the catched unhandled exception and logs it
        /// </summary>
        void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception exception = new Exception(e.ExceptionObject.ToString());
            ErrorManager.AddToLog("Unhandled exception: ", exception);
        }

        /// <summary>
        /// Exit nicely after the form has been loaded
        /// </summary>
        void MainFormLoaded(object sender, EventArgs e) => Close();

        #endregion

        #region Private Properties

        /* AppMan */
        FileSystemWatcher amWatcher;

        /* Components */
        QuickFind quickFind;
        ToolStripProgressBarEx toolStripProgressBar;
        ToolStripButton restartButton;
        ProcessRunner processRunner;

        /* Dialogs */
        PrintDialog printDialog;
        ColorDialog colorDialog;
        OpenFileDialog openFileDialog;
        SaveFileDialog saveFileDialog;
        PrintPreviewDialog printPreviewDialog;
        FRInFilesDialog frInFilesDialog;
        FRInDocDialog frInDocDialog;
        GoToDialog gotoDialog;

        /* Working Dir */
        string workingDirectory = string.Empty;

        /* Form State */
        FormState formState;
        Hashtable fullScreenDocks;
        bool notifyOpenFile;
        bool closingForOpenFile;
        bool closingAll;
        
        /* Singleton */
        public static bool Silent;
        public static bool IsFirst;
        public static string[] Arguments;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the DockPanel
        /// </summary> 
        public DockPanel DockPanel { get; private set; }

        /// <summary>
        /// Gets the Scintilla configuration
        /// </summary>
        public Scintilla SciConfig => ScintillaManager.SciConfig;

        /// <summary>
        /// Gets the menu strip
        /// </summary>
        public MenuStrip MenuStrip { get; private set; }

        /// <summary>
        /// Gets the tool strip
        /// </summary>
        public ToolStrip ToolStrip { get; private set; }

        /// <summary>
        /// Gets the tool strip panel
        /// </summary>
        public ToolStripPanel ToolStripPanel { get; private set; }

        /// <summary>
        /// Gets the toolStripStatusLabel
        /// </summary>
        public ToolStripStatusLabel StatusLabel { get; private set; }

        /// <summary>
        /// Gets the toolStripProgressLabel
        /// </summary>
        public ToolStripStatusLabel ProgressLabel { get; private set; }

        /// <summary>
        /// Gets the ToolStripProgressBarEx
        /// </summary>
        public ToolStripProgressBar ProgressBar => toolStripProgressBar;

        /// <summary>
        /// Gets the TabMenu
        /// </summary>
        public ContextMenuStrip TabMenu { get; private set; }

        /// <summary>
        /// Gets the EditorMenu
        /// </summary>
        public ContextMenuStrip EditorMenu { get; private set; }

        /// <summary>
        /// Gets the StatusStrip
        /// </summary>
        public StatusStrip StatusStrip { get; private set; }

        /// <summary>
        /// Gets the IgnoredKeys
        /// </summary>
        public List<Keys> IgnoredKeys => ShortcutManager.AllShortcuts;

        /// <summary>
        /// Gets the Settings interface
        /// </summary>
        public ISettings Settings => AppSettings;

        /// <summary>
        /// Gets or sets the actual Settings
        /// </summary>
        public SettingObject AppSettings { get; set; }

        /// <summary>
        /// Gets the CurrentDocument
        /// </summary>
        public ITabbedDocument CurrentDocument => DockPanel.ActiveDocument as ITabbedDocument;

        /// <summary>
        /// Is FlashDevelop closing?
        /// </summary>
        public bool ClosingEntirely { get; private set; }

        /// <summary>
        /// Is this first MainForm instance?
        /// </summary>
        public bool IsFirstInstance => IsFirst;

        /// <summary>
        /// Is FlashDevelop in multi instance mode?
        /// </summary>
        public bool MultiInstanceMode => Program.MultiInstanceMode;

        /// <summary>
        /// Is FlashDevelop in standalone mode?
        /// </summary>
        public bool StandaloneMode => File.Exists(Path.Combine(PathHelper.AppDir, ".local"));

        /// <summary>
        /// Gets the all available documents
        /// </summary> 
        public ITabbedDocument[] Documents
        {
            get
            {
                var documents = new List<ITabbedDocument>();
                foreach (DockPane pane in DockPanel.Panes)
                {
                    if (pane.DockState == DockState.Document)
                    {
                        foreach (IDockContent content in pane.Contents)
                        {
                            if (content is TabbedDocument document)
                            {
                                documents.Add(document);
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
        public bool HasModifiedDocuments => Documents.Any(document => document.IsModified);

        /// <summary>
        /// Gets or sets the WorkingDirectory
        /// </summary>
        public string WorkingDirectory
        {
            get
            {
                if (!Directory.Exists(workingDirectory))
                {
                    workingDirectory = GetWorkingDirectory();
                }
                return workingDirectory;
            }
            set => workingDirectory = value;
        }

        /// <summary>
        /// Gets or sets the ProcessIsRunning
        /// </summary>
        public bool ProcessIsRunning => processRunner.IsRunning;

        /// <summary>
        /// Gets the panelIsActive
        /// </summary>
        public bool PanelIsActive { get; private set; }

        /// <summary>
        /// Gets the isFullScreen
        /// </summary>
        public bool IsFullScreen { get; private set; }

        /// <summary>
        /// Gets or sets the ReloadingDocument
        /// </summary>
        public bool ReloadingDocument { get; set; }

        /// <summary>
        /// Gets or sets the CloseAllCanceled
        /// </summary>
        public bool CloseAllCanceled { get; set; }

        /// <summary>
        /// Gets or sets the ProcessingContents
        /// </summary>
        public bool ProcessingContents { get; set; } = false;

        /// <summary>
        /// Gets or sets the RestoringContents
        /// </summary>
        public bool RestoringContents { get; set; } = false;

        /// <summary>
        /// Gets or sets the SavingMultiple
        /// </summary>
        public bool SavingMultiple { get; set; }

        /// <summary>
        /// Gets or sets the RestartRequested
        /// </summary>
        public bool RestartRequested { get; set; }

        /// <summary>
        /// Gets whether the application requires a restart to apply changes.
        /// </summary>
        public bool RequiresRestart { get; private set; }

        /// <summary>
        /// Gets or sets the RefreshConfig
        /// </summary>
        public bool RefreshConfig { get; set; }

        /// <summary>
        /// Gets the application start args
        /// </summary>
        public string[] StartArguments => Arguments;

        /// <summary>
        /// Gets the application custom args
        /// </summary>
        public List<Argument> CustomArguments => ArgumentDialog.CustomArguments;

        /// <summary>
        /// Gets the application's version
        /// </summary>
        public new string ProductVersion => Application.ProductVersion;

        /// <summary>
        /// Gets the full human readable version string
        /// </summary>
        public new string ProductName => Application.ProductName;

        /// <summary>
        /// Gets the command prompt executable (custom or cmd.exe by default).
        /// </summary>
        public string CommandPromptExecutable
        {
            get
            {
                if (!string.IsNullOrEmpty(Settings.CustomCommandPrompt) && File.Exists(Settings.CustomCommandPrompt))
                    return Settings.CustomCommandPrompt;
                return "cmd.exe";
            }
        }

        /// <summary>
        /// Gets the version of the operating system
        /// </summary>
        public Version OSVersion => Environment.OSVersion.Version;

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
                tabbedDocument.Closing += OnDocumentClosing;
                tabbedDocument.Closed += OnDocumentClosed;
                tabbedDocument.Text = TextHelper.GetString("Title.CustomDocument");
                tabbedDocument.TabPageContextMenuStrip = TabMenu;
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
        public DockContent CreateEditableDocument(string file, string text, int codepage)
        {
            try
            {
                notifyOpenFile = true;
                TabbedDocument tabbedDocument = new TabbedDocument();
                tabbedDocument.Closing += OnDocumentClosing;
                tabbedDocument.Closed += OnDocumentClosed;
                tabbedDocument.TabPageContextMenuStrip = TabMenu;
                tabbedDocument.ContextMenuStrip = EditorMenu;
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
        public DockContent CreateDockablePanel(Control ctrl, string guid, Image image, DockState defaultDockState)
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
        public DockContent CreateDynamicPersistDockablePanel(Control ctrl, string guid, string id, Image image, DockState defaultDockState)
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
        public DockContent OpenEditableDocument(string org, Encoding encoding, bool restorePosition)
        {
            string file = PathHelper.GetPhysicalPathName(org);
            TextEvent te = new TextEvent(EventType.FileOpening, file);
            EventManager.DispatchEvent(this, te);
            if (te.Handled)
            {
                if (Documents.Length == 0) SmartNew(null, null);
                return null;
            }
            if (file.EndsWithOrdinal(".delete.fdz"))
            {
                CallCommand("RemoveZip", file);
                return null;
            }
            if (file.EndsWithOrdinal(".fdz"))
            {
                CallCommand("ExtractZip", file);
                if (file.IndexOf("theme", StringComparison.OrdinalIgnoreCase) != -1)
                {
                    string currentTheme = Path.Combine(PathHelper.ThemesDir, "CURRENT");
                    if (File.Exists(currentTheme))
                    {
                        ThemeManager.LoadTheme(currentTheme);
                        ThemeManager.WalkControls(this);
                        RefreshSciConfig();
                    }
                }
                return null;
            }
            try
            {
                foreach (ITabbedDocument doc in Documents)
                {
                    if (doc.IsEditable && doc.FileName.ToUpper() == file.ToUpper())
                    {
                        doc.Activate();
                        return doc as DockContent;
                    }
                }
            }
            catch { }
            DockContent createdDoc;
            EncodingFileInfo info;
            if (encoding is null)
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
                info.Contents = de.Data as string;
                info.CodePage = Encoding.UTF8.CodePage; // assume plugin always return UTF8
            }
            try
            {
                if (CurrentDocument != null && CurrentDocument.IsUntitled && !CurrentDocument.IsModified && Documents.Length == 1)
                {
                    closingForOpenFile = true;
                    CurrentDocument.Close();
                    closingForOpenFile = false;
                    createdDoc = CreateEditableDocument(file, info.Contents, info.CodePage);
                }
                else createdDoc = CreateEditableDocument(file, info.Contents, info.CodePage);
                ButtonManager.AddNewReopenMenuItem(file);
            }
            catch
            {
                createdDoc = CreateEditableDocument(file, info.Contents, info.CodePage);
                ButtonManager.AddNewReopenMenuItem(file);
            }
            var document = (TabbedDocument)createdDoc;
            document.SciControl.SaveBOM = info.ContainsBOM;
            document.SciControl.BeginInvoke((MethodInvoker)(() =>
            {
                if (AppSettings.RestoreFileStates)
                {
                    FileStateManager.ApplyFileState(document, restorePosition);
                }
            }));
            ButtonManager.UpdateFlaggedButtons();
            return createdDoc;
        }

        public DockContent OpenEditableDocument(string file, bool restorePosition) => OpenEditableDocument(file, null, restorePosition);

        public DockContent OpenEditableDocument(string file) => OpenEditableDocument(file, null, true);

        #endregion

        #region Construct Components
       
        /// <summary>
        /// Initializes the graphics
        /// </summary>
        void InitializeGraphics()
        {
            Icon icon = new Icon(ResourceHelper.GetStream("FlashDevelopIcon.ico"));
            Icon = printPreviewDialog.Icon = icon;
        }

        /// <summary>
        /// Initializes the theme and config detection
        /// </summary>
        void InitializeConfig()
        {
            try
            {
                // Check for FD update
                string update = Path.Combine(PathHelper.BaseDir, ".update");
                if (File.Exists(update))
                {
                    File.Delete(update);
                    RefreshConfig = true;
                }
                // Check for appman update
                string appman = Path.Combine(PathHelper.BaseDir, ".appman");
                if (File.Exists(appman))
                {
                    File.Delete(appman);
                    RefreshConfig = true;
                }
                // Load platform data from user files
                PlatformData.Load(Path.Combine(PathHelper.SettingDir, "Platforms"));
                // Load current theme for applying later
                string currentTheme = Path.Combine(PathHelper.ThemesDir, "CURRENT");
                if (File.Exists(currentTheme)) ThemeManager.LoadTheme(currentTheme);
                // Apply FD dir and appman dir to PATH
                string amPath = Path.Combine(PathHelper.ToolDir, "AppMan");
                string oldPath = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Process);
                string newPath = oldPath + ";" + amPath + ";" + PathHelper.AppDir;
                Environment.SetEnvironmentVariable("PATH", newPath, EnvironmentVariableTarget.Process);
                // Watch for appman update notifications
                amWatcher = new FileSystemWatcher(PathHelper.BaseDir, ".appman");
                amWatcher.Changed += AppManUpdate;
                amWatcher.Created += AppManUpdate;
                amWatcher.IncludeSubdirectories = false;
                amWatcher.EnableRaisingEvents = true;
            }
            catch {} // No errors...
        }

        /// <summary>
        /// When AppMan installs something it notifies of changes. Forward notifications.
        /// </summary>
        void AppManUpdate(object sender, FileSystemEventArgs e)
        {
            try
            {
                NotifyEvent ne = new NotifyEvent(EventType.AppChanges);
                EventManager.DispatchEvent(this, ne); // Notify plugins...
                string appMan = Path.Combine(PathHelper.BaseDir, ".appman");
                string contents = File.ReadAllText(appMan);
                if (contents == "restart")
                {
                    RestartRequired();
                }
            }
            catch {} // No errors...
        }

        /// <summary>
        /// Initializes the restart button
        /// </summary>
        void InitializeRestartButton()
        {
            restartButton = new ToolStripButton();
            restartButton.Image = FindImage("73|6|3|3");
            restartButton.Alignment = ToolStripItemAlignment.Right;
            restartButton.Text = TextHelper.GetString("Label.Restart");
            restartButton.ToolTipText = TextHelper.GetString("Info.RequiresRestart");
            restartButton.Click += delegate { Restart(null, null); };
            restartButton.Visible = false;
            ToolStrip.Items.Add(restartButton);
        }

        /// <summary>
        /// Initializes the smart dialogs
        /// </summary>
        public void InitializeSmartDialogs()
        {
            formState = new FormState();
            gotoDialog = new GoToDialog();
            frInFilesDialog = new FRInFilesDialog();
            frInDocDialog = new FRInDocDialog();
        }

        /// <summary>
        /// Initializes the First Run dialog
        /// </summary>
        DialogResult InitializeFirstRun()
        {
            if (!StandaloneMode && IsFirst && FirstRunDialog.ShouldProcessCommands())
            {
                return FirstRunDialog.Show();
            }
            return DialogResult.None;
        }

        /// <summary>
        /// Initializes the UI rendering
        /// </summary>
        void InitializeRendering()
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
        void InitializeSettings()
        {
            AppSettings = SettingObject.GetDefaultSettings();
            if (File.Exists(FileNameHelper.SettingData))
            {
                object obj = ObjectSerializer.Deserialize(FileNameHelper.SettingData, AppSettings, false);
                AppSettings = (SettingObject)obj;
            }
            SettingObject.EnsureValidity(AppSettings);
            FileStateManager.RemoveOldStateFiles();
        }

        /// <summary>
        /// Initializes the localization from .locale file
        /// </summary>
        void InitializeLocalization()
        {
            try
            {
                string filePath = Path.Combine(PathHelper.BaseDir, ".locale");
                if (File.Exists(filePath))
                {
                    string enumData = File.ReadAllText(filePath).Trim();
                    LocaleVersion localeVersion = (LocaleVersion)Enum.Parse(typeof(LocaleVersion), enumData);
                    AppSettings.LocaleVersion = localeVersion;
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
            processRunner = new ProcessRunner();
            processRunner.RedirectInput = true;
            processRunner.ProcessEnded += ProcessEnded;
            processRunner.Output += ProcessOutput;
            processRunner.Error += ProcessError;
        }

        /// <summary>
        /// Checks for updates in specified schedule
        /// </summary>
        public void CheckForUpdates()
        {
            try
            {
                DateTime last = new DateTime(AppSettings.LastUpdateCheck);
                TimeSpan elapsed = DateTime.UtcNow.Subtract(last);
                switch (AppSettings.CheckForUpdates)
                {
                    case UpdateInterval.Weekly:
                    {
                        if (elapsed.TotalDays >= 7)
                        {
                            AppSettings.LastUpdateCheck = DateTime.UtcNow.Ticks;
                            UpdateDialog.Show(true);
                        }
                        break;
                    }
                    case UpdateInterval.Monthly:
                    {
                        if (elapsed.TotalDays >= 30)
                        {
                            AppSettings.LastUpdateCheck = DateTime.UtcNow.Ticks;
                            UpdateDialog.Show(true);
                        }
                        break;
                    }
                }
            }
            catch { /* NO ERRORS PLEASE */ }
        }

        /// <summary>
        /// Initializes the window position and size
        /// </summary>
        public void InitializeWindow()
        {
            WindowState = AppSettings.WindowState;
            Rectangle bounds = new Rectangle(AppSettings.WindowPosition, AppSettings.WindowSize);
            bounds.Inflate(-4, -25);
            bool validPosition = false;
            foreach (Screen screen in Screen.AllScreens)
            {
                if (screen.Bounds.IntersectsWith(bounds))
                {
                    Location = AppSettings.WindowPosition;
                    validPosition = true;
                    break;
                }
            }
            if (!validPosition) Location = new Point(0, 0);
            // Continue/perform layout!
            ResumeLayout(false);
            PerformLayout();
        }

        /// <summary>
        /// Initialises the plugins, restores the layout and sets an fixed position
        /// </summary>
        public void InitializeMainForm()
        {
            try
            {
                string pluginDir = PathHelper.PluginDir; // Plugins of all users
                if (Directory.Exists(pluginDir)) PluginServices.FindPlugins(pluginDir);
                if (!StandaloneMode) // No user plugins on standalone...
                {
                    string userPluginDir = PathHelper.UserPluginDir;
                    if (Directory.Exists(userPluginDir)) PluginServices.FindPlugins(userPluginDir);
                    else Directory.CreateDirectory(userPluginDir);
                }
                LayoutManager.BuildLayoutSystems(FileNameHelper.LayoutData);
                ShortcutManager.LoadCustomShortcuts();
                ArgumentDialog.LoadCustomArguments();
                ClipboardManager.Initialize(this);
                UITools.Init();
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Initializes the form components
        /// </summary>
        void InitializeComponents()
        {
            quickFind = new QuickFind();
            DockPanel = new DockPanel();
            StatusStrip = new StatusStrip();
            ToolStripPanel = new ToolStripPanel();
            MenuStrip = StripBarManager.GetMenuStrip(FileNameHelper.MainMenu);
            ToolStrip = StripBarManager.GetToolStrip(FileNameHelper.ToolBar);
            EditorMenu = StripBarManager.GetContextMenu(FileNameHelper.ScintillaMenu);
            TabMenu = StripBarManager.GetContextMenu(FileNameHelper.TabMenu);
            StatusLabel = new ToolStripStatusLabel();
            ProgressLabel = new ToolStripStatusLabel();
            toolStripProgressBar = new ToolStripProgressBarEx();
            printPreviewDialog = new PrintPreviewDialog();
            saveFileDialog = new SaveFileDialog();
            openFileDialog = new OpenFileDialog();
            colorDialog = new ColorDialog();
            printDialog = new PrintDialog();
            //
            // toolStripPanel
            //
            ToolStripPanel.Dock = DockStyle.Top;
            if (PlatformHelper.IsRunningOnMono())
            {
                ToolStripPanel.Controls.Add(MenuStrip);
                ToolStripPanel.Controls.Add(ToolStrip);
            }
            else 
            {
                ToolStripPanel.Controls.Add(ToolStrip);
                ToolStripPanel.Controls.Add(MenuStrip);
            }
            TabMenu.Font = Globals.Settings.DefaultFont;
            ToolStrip.Font = Globals.Settings.DefaultFont;
            MenuStrip.Font = Globals.Settings.DefaultFont;
            EditorMenu.Font = Globals.Settings.DefaultFont;
            TabMenu.Renderer = new DockPanelStripRenderer(false);
            EditorMenu.Renderer = new DockPanelStripRenderer(false);
            MenuStrip.Renderer = new DockPanelStripRenderer(false);
            ToolStrip.Renderer = new DockPanelStripRenderer(false);
            ToolStrip.Padding = new Padding(0, 1, 0, 0);
            ToolStrip.Size = new Size(500, 26);
            ToolStrip.Stretch = true;
            // 
            // openFileDialog
            //
            openFileDialog.Title = " " + TextHelper.GetString("Title.OpenFileDialog");
            openFileDialog.Filter = TextHelper.GetString("Info.FileDialogFilter") + "|*.*";
            openFileDialog.RestoreDirectory = true;
            //
            // colorDialog
            //
            colorDialog.FullOpen = true;
            colorDialog.ShowHelp = false;
            // 
            // printPreviewDialog
            //
            printPreviewDialog.Enabled = true;
            printPreviewDialog.Name = "printPreviewDialog";
            printPreviewDialog.StartPosition = FormStartPosition.CenterParent;
            printPreviewDialog.TransparencyKey = Color.Empty;
            printPreviewDialog.Visible = false;
            // 
            // saveFileDialog
            //
            saveFileDialog.Title = " " + TextHelper.GetString("Title.SaveFileDialog");
            saveFileDialog.Filter = TextHelper.GetString("Info.FileDialogFilter") + "|*.*";
            saveFileDialog.RestoreDirectory = true;
            // 
            // dockPanel
            //
            DockPanel.TabIndex = 2;
            DockPanel.DocumentStyle = DocumentStyle.DockingWindow;
            DockPanel.DockWindows[DockState.Document].Controls.Add(quickFind);
            DockPanel.Dock = DockStyle.Fill;
            DockPanel.Name = "dockPanel";
            //
            // toolStripStatusLabel
            //
            StatusLabel.Name = "toolStripStatusLabel";
            StatusLabel.TextAlign = ContentAlignment.MiddleLeft;
            StatusLabel.Spring = true;
            //
            // toolStripProgressLabel
            //
            ProgressLabel.AutoSize = true;
            ProgressLabel.Name = "toolStripProgressLabel";
            ProgressLabel.TextAlign = ContentAlignment.MiddleRight;
            ProgressLabel.Visible = false;
            //
            // toolStripProgressBar
            //
            toolStripProgressBar.Name = "toolStripProgressBar";
            toolStripProgressBar.ControlAlign = ContentAlignment.MiddleRight;
            toolStripProgressBar.ProgressBar.Width = 120;
            toolStripProgressBar.Visible = false;
            // 
            // statusStrip
            //
            StatusStrip.TabIndex = 3;
            StatusStrip.Name = "statusStrip";
            StatusStrip.Items.Add(StatusLabel);
            StatusStrip.Items.Add(ProgressLabel);
            StatusStrip.Items.Add(toolStripProgressBar);
            StatusStrip.Font = Globals.Settings.DefaultFont;
            StatusStrip.Renderer = new DockPanelStripRenderer(false);
            StatusStrip.Stretch = true;
            // 
            // MainForm
            //
            AllowDrop = true;
            Name = "MainForm";
            Text = DistroConfig.DISTRIBUTION_NAME;
            Controls.Add(DockPanel);
            Controls.Add(ToolStripPanel);
            Controls.Add(StatusStrip);
            MainMenuStrip = MenuStrip;
            Size = AppSettings.WindowSize;
            Font = AppSettings.DefaultFont;
            StartPosition = FormStartPosition.Manual;
            Closing += OnMainFormClosing;
            FormClosed += OnMainFormClosed;
            Activated += OnMainFormActivate;
            Shown += OnMainFormShow;
            Load += OnMainFormLoad;
            LocationChanged += OnMainFormLocationChange;
            GotFocus += OnMainFormGotFocus;
            Resize += OnMainFormResize;
            ScintillaManager.ConfigurationLoaded += ApplyAllSettings;
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Checks the file changes and activates
        /// </summary>
        void OnMainFormActivate(object sender, EventArgs e)
        {
            if (CurrentDocument is null) return;
            CurrentDocument.Activate(); // Activate the current document
            ButtonManager.UpdateFlaggedButtons();
        }

        /// <summary>
        /// Checks the file changes when recieving focus
        /// </summary>
        void OnMainFormGotFocus(object sender, EventArgs e)
        {
            if (CurrentDocument is null) return;
            ButtonManager.UpdateFlaggedButtons();
        }

        /// <summary>
        /// Initalizes the windows state after show is called and
        /// check if we need to notify user for recovery files
        /// </summary>
        void OnMainFormShow(object sender, EventArgs e)
        {
            if (RecoveryDialog.ShouldShowDialog()) RecoveryDialog.Show();
        }

        /// <summary>
        /// Saves the window size as it's being resized
        /// </summary>
        void OnMainFormResize(object sender, EventArgs e)
        {
            if (WindowState != FormWindowState.Maximized && WindowState != FormWindowState.Minimized)
            {
                AppSettings.WindowSize = Size;
            }
        }

        /// <summary>
        /// Saves the window location as it's being moved
        /// </summary>
        void OnMainFormLocationChange(object sender, EventArgs e)
        {
            if (WindowState != FormWindowState.Maximized && WindowState != FormWindowState.Minimized)
            {
                AppSettings.WindowSize = Size;
                AppSettings.WindowPosition = Location;
            }
        }

        /// <summary>
        /// Setups misc stuff when MainForm is loaded
        /// </summary>
        void OnMainFormLoad(object sender, EventArgs e)
        {
            /**
            * DockPanel events
            */
            DockPanel.ActivePaneChanged += OnActivePaneChanged;
            DockPanel.ActiveContentChanged += OnActiveContentChanged;
            DockPanel.ActiveDocumentChanged += OnActiveDocumentChanged;
            DockPanel.ContentRemoved += OnContentRemoved;
            /**
            * Populate menus and check buttons 
            */
            ButtonManager.PopulateReopenMenu();
            ButtonManager.UpdateFlaggedButtons();
            /**
            * Set the initial directory for file dialogs
            */
            string path = AppSettings.LatestDialogPath;
            openFileDialog.InitialDirectory = path;
            saveFileDialog.InitialDirectory = path;
            workingDirectory = path;
            /**
            * Open document[s] in startup 
            */
            if (!Arguments.IsNullOrEmpty())
            {
                ProcessParameters(Arguments);
                Arguments = null;
            }
            else if (AppSettings.RestoreFileSession)
            {
                string file = FileNameHelper.SessionData;
                SessionManager.RestoreSession(file, SessionType.Startup);
            }
            if (Documents.Length == 0)
            {
                NotifyEvent ne = new NotifyEvent(EventType.FileEmpty);
                EventManager.DispatchEvent(this, ne);
                if (!ne.Handled) SmartNew(null, null);
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
            ApplyAllSettings();
            /**
            * Initialize window and continue layout
            */
            InitializeWindow();
            /**
            * Initializes the restart button
            */
            InitializeRestartButton();
            /**
            * Check for updates when needed
            */
            CheckForUpdates();
        }

        /// <summary>
        /// Checks that if there are modified documents when the MainForm is closing
        /// </summary>
        public void OnMainFormClosing(object sender, CancelEventArgs e)
        {
            ClosingEntirely = true;
            Session session = SessionManager.GetCurrentSession();
            NotifyEvent ne = new NotifyEvent(EventType.UIClosing);
            EventManager.DispatchEvent(this, ne);
            if (ne.Handled)
            {
                ClosingEntirely = false;
                e.Cancel = true;
            }
            if (!e.Cancel && Globals.Settings.ConfirmOnExit)
            {
                string title = TextHelper.GetString("Title.ConfirmDialog");
                string message = TextHelper.GetString("Info.AreYouSureToExit");
                DialogResult result = MessageBox.Show(this, message, " " + title, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.No) e.Cancel = true;
            }
            if (!e.Cancel) CloseAllDocuments(false);
            if (CloseAllCanceled)
            {
                CloseAllCanceled = false;
                ClosingEntirely = false;
                e.Cancel = true;
            }
            if (!e.Cancel && IsFullScreen)
            {
                ToggleFullScreen(null, null);
            }
            if (!e.Cancel && Documents.Length == 0)
            {
                NotifyEvent fe = new NotifyEvent(EventType.FileEmpty);
                EventManager.DispatchEvent(this, fe);
                if (!fe.Handled) SmartNew(null, null);
            }
            if (!e.Cancel)
            {
                string file = FileNameHelper.SessionData;
                SessionManager.SaveSession(file, session);
                ShortcutManager.SaveCustomShortcuts();
                ArgumentDialog.SaveCustomArguments();
                ClipboardManager.Dispose();
                PluginServices.DisposePlugins();
                KillProcess();
                SaveAllSettings();
            }
            else RestartRequested = false;
        }

        /// <summary>
        /// When form is closed restart if requested.
        /// </summary>
        public void OnMainFormClosed(object sender, FormClosedEventArgs e)
        {
            if (!RestartRequested) return;
            RestartRequested = false;
            Process.Start(Application.ExecutablePath);
            Process.GetCurrentProcess().Kill();
        }

        /// <summary>
        /// When dock changes, applies the padding to documents
        /// </summary>
        void OnActivePaneChanged(object sender, EventArgs e) => quickFind.ApplyFixedDocumentPadding();

        /// <summary>
        /// When document is removed update tab texts
        /// </summary>
        public void OnContentRemoved(object sender, DockContentEventArgs e) => TabTextManager.UpdateTabTexts();

        /// <summary>
        /// Dispatch UIRefresh event and focus scintilla control
        /// </summary>
        void OnActiveContentChanged(object sender, EventArgs e)
        {
            if (DockPanel.ActiveContent is null) return;
            if (DockPanel.ActiveContent.GetType() == typeof(TabbedDocument))
            {
                PanelIsActive = false;
                TabbedDocument document = (TabbedDocument)DockPanel.ActiveContent;
                document.Activate();
            }
            else PanelIsActive = true;
            NotifyEvent ne = new NotifyEvent(EventType.UIRefresh);
            EventManager.DispatchEvent(this, ne);
        }

        /// <summary>
        /// Updates the UI, tabbing, working directory and the button states. 
        /// Also notifies the plugins for the FileOpen and FileSwitch events.
        /// </summary>
        public void OnActiveDocumentChanged(object sender, EventArgs e)
        {
            try
            {
                if (CurrentDocument is null) return;
                OnScintillaControlUpdateControl(CurrentDocument.SciControl);
                quickFind.CanSearch = CurrentDocument.IsEditable;
                /**
                * Bring this newly active document to the top of the tab history
                * unless you're currently cycling through tabs with the keyboard
                */
                TabbingManager.UpdateSequentialIndex(CurrentDocument);
                if (!TabbingManager.TabTimer.Enabled)
                {
                    TabbingManager.TabHistory.Remove(CurrentDocument);
                    TabbingManager.TabHistory.Insert(0, CurrentDocument);
                }
                if (CurrentDocument.IsEditable)
                {
                    /**
                    * Apply correct extension when saving
                    */
                    if (AppSettings.ApplyFileExtension)
                    {
                        string extension = Path.GetExtension(CurrentDocument.FileName);
                        if (extension != "") saveFileDialog.DefaultExt = extension;
                    }
                    /**
                    * Set current working directory
                    */
                    string path = Path.GetDirectoryName(CurrentDocument.FileName);
                    if (!CurrentDocument.IsUntitled && Directory.Exists(path))
                    {
                        workingDirectory = path;
                    }
                    /**
                    * Checks the file changes
                    */
                    TabbedDocument document = (TabbedDocument)CurrentDocument;
                    document.Activate();
                    /**
                    * Processes the opened file
                    */
                    if (notifyOpenFile)
                    {
                        ScintillaManager.UpdateControlSyntax(CurrentDocument.SciControl);
                        if (File.Exists(CurrentDocument.FileName))
                        {
                            TextEvent te = new TextEvent(EventType.FileOpen, CurrentDocument.FileName);
                            EventManager.DispatchEvent(this, te);
                        }
                        notifyOpenFile = false;
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
        public void OnDocumentClosing(object sender, CancelEventArgs e)
        {
            ITabbedDocument document = (ITabbedDocument)sender;
            if (CloseAllCanceled && closingAll) e.Cancel = true;
            else if (document.IsModified)
            {
                string saveChanges = TextHelper.GetString("Info.SaveChanges");
                string saveChangesTitle = TextHelper.GetString("Title.SaveChanges");
                DialogResult result = MessageBox.Show(this, saveChanges, saveChangesTitle + " " + document.Text, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    if (document.IsUntitled)
                    {
                        saveFileDialog.FileName = document.FileName;
                        if (saveFileDialog.ShowDialog(this) == DialogResult.OK && saveFileDialog.FileName.Length != 0)
                        {
                            ButtonManager.AddNewReopenMenuItem(saveFileDialog.FileName);
                            document.Save(saveFileDialog.FileName);
                        }
                        else
                        {
                            if (closingAll) CloseAllCanceled = true;
                            e.Cancel = true;
                        }
                    }
                    else document.Save();
                }
                else if (result == DialogResult.Cancel)
                {
                    if (closingAll) CloseAllCanceled = true;
                    e.Cancel = true;
                }
                else if (result == DialogResult.No)
                {
                    RecoveryManager.RemoveTemporaryFile(document.FileName);
                }
            }
            if (Documents.Length == 1 && document.IsUntitled && !document.IsModified && document.SciControl.Length == 0 && !e.Cancel && !closingForOpenFile && !RestoringContents)
            {
                e.Cancel = true;
            }
            if (Documents.Length == 1 && !e.Cancel && !closingForOpenFile && !ClosingEntirely && !RestoringContents)
            {
                NotifyEvent ne = new NotifyEvent(EventType.FileEmpty);
                EventManager.DispatchEvent(this, ne);
                if (!ne.Handled) SmartNew(null, null);
            }
        }

        /// <summary>
        /// Activates the previous document when document is closed
        /// </summary>
        public void OnDocumentClosed(object sender, EventArgs e)
        {
            ITabbedDocument document = (ITabbedDocument) sender;
            TabbingManager.TabHistory.Remove(document);
            TextEvent ne = new TextEvent(EventType.FileClose, document.FileName);
            EventManager.DispatchEvent(this, ne);
            if (AppSettings.SequentialTabbing)
            {
                if (TabbingManager.SequentialIndex == 0) Documents[0].Activate();
                else TabbingManager.NavigateTabsSequentially(-1);
            }
            else TabbingManager.NavigateTabHistory(0);
            if (document.IsEditable && !document.IsUntitled)
            {
                if (AppSettings.RestoreFileStates) FileStateManager.SaveFileState(document);
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
            if (InvokeRequired)
            {
                BeginInvoke((MethodInvoker)(() => OnScintillaControlUpdateControl(sci)));
                return;
            }
            var document = DocumentManager.FindDocument(sci);
            if (sci != null && document != null && document.IsEditable)
            {
                string statusText = " " + TextHelper.GetString("Info.StatusText");
                string line = sci.CurrentLine + 1 + " / " + sci.LineCount;
                string column = sci.Column(sci.CurrentPos) + 1 + " / " + (sci.Column(sci.LineEndPosition(sci.CurrentLine)) + 1);
                var oldOS = OSVersion.Major < 6; // Vista is 6.0 and ok...
                string file = oldOS ? PathHelper.GetCompactPath(sci.FileName) : sci.FileName;
                string eol = (sci.EOLMode == 0) ? "CR+LF" : ((sci.EOLMode == 1) ? "CR" : "LF");
                string encoding = ButtonManager.GetActiveEncodingName();
                StatusLabel.Text = string.Format(statusText, line, column, eol, encoding, file);
            }
            else StatusLabel.Text = " ";
            OnUpdateMainFormDialogTitle();
            ButtonManager.UpdateFlaggedButtons();
            var ne = new NotifyEvent(EventType.UIRefresh);
            EventManager.DispatchEvent(this, ne);
        }

        /// <summary>
        /// Opens the selected files on drop
        /// </summary>
        public void OnScintillaControlDropFiles(ScintillaControl sci, string data)
        {
            if (InvokeRequired)
            {
                BeginInvoke((MethodInvoker)(() => OnScintillaControlDropFiles(null, data)));
                return;
            }
            var files = Regex.Split(data.Substring(1, data.Length - 2), "\" \"");
            foreach (var file in files)
            {
                if (File.Exists(file))
                {
                    var doc = OpenEditableDocument(file);
                    if (doc is null || ModifierKeys == Keys.Control) return;
                    var drop = DocumentManager.FindDocument(sci) as DockContent;
                    if (drop?.Pane != null)
                    {
                        doc.DockTo(drop.Pane, DockStyle.Fill, -1);
                        doc.Activate();
                    }
                }
                else if (Directory.Exists(file))
                {
                    var de = new TextEvent(EventType.FolderOpen, file);
                    EventManager.DispatchEvent(this, de);
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
            string dlgTitle = TextHelper.GetString("Title.ConfirmDialog");
            string message = TextHelper.GetString("Info.MakeReadOnlyWritable");
            if (MessageBox.Show(this, message, dlgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                ScintillaManager.MakeFileWritable(sci);
            }
        }

        /// <summary>
        /// Handles the modified event
        /// </summary>
        public void OnScintillaControlModified(ScintillaControl sender, int pos, int modType, string text, int length, int lAdded, int line, int fLevelNow, int fLevelPrev)
        {
            var document = DocumentManager.FindDocument(sender);
            if (document is null || !document.IsEditable) return;
            OnDocumentModify(document);
            if (!AppSettings.ViewModifiedLines) return;
            int flags = sender.ModEventMask;
            sender.ModEventMask = flags & ~(int)ScintillaNet.Enums.ModificationFlags.ChangeMarker;
            int modLine = sender.LineFromPosition(pos);
            sender.MarkerAdd(modLine, 2);
            for (int i = 0; i <= lAdded; i++)
            {
                sender.MarkerAdd(modLine + i, 2);
            }
            sender.ModEventMask = flags;
        }

        /// <summary>
        /// Provides a basic folding service and notifies the plugins for the MarginClick event
        /// </summary>
        public void OnScintillaControlMarginClick(ScintillaControl sci, int modifiers, int position, int margin)
        {
            if (margin != ScintillaManager.FoldingMargin) return;
            int line = sci.LineFromPosition(position);
            if (ModifierKeys == Keys.Control) MarkerManager.ToggleMarker(sci, 0, line);
            else sci.ToggleFold(line);
        }

        /// <summary>
        /// Handles the mouse wheel on hover
        /// </summary>
        public bool PreFilterMessage(ref Message m)
        {
            if (Win32.ShouldUseWin32() && m.Msg == 0x20a) 
            {
                int x = unchecked((short)(long)m.LParam);
                int y = unchecked((short)((long)m.LParam >> 16));
                var hWnd = Win32.WindowFromPoint(new Point(x, y));
                if (hWnd == IntPtr.Zero) return false;
                var doc = Globals.CurrentDocument;
                if (FromHandle(hWnd) != null)
                {
                    Win32.SendMessage(hWnd, m.Msg, m.WParam, m.LParam);
                    return true;
                }

                if (doc != null && doc.IsEditable && (hWnd == doc.SplitSci1.HandleSci || hWnd == doc.SplitSci2.HandleSci))
                {
                    Win32.SendMessage(hWnd, m.Msg, m.WParam, m.LParam);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Handles the application shortcuts
        /// </summary>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
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
                if (CurrentDocument.SciControl is null || !CurrentDocument.SciControl.IsFocus)
                {
                    if (keyData == (Keys.Control | Keys.C)) return false;
                    if (keyData == (Keys.Control | Keys.V)) return false;
                    if (keyData == (Keys.Control | Keys.X)) return false;
                    if (keyData == (Keys.Control | Keys.A)) return false;
                    if (keyData == (Keys.Control | Keys.Z)) return false;
                    if (keyData == (Keys.Control | Keys.Y)) return false;
                }
                /**
                * Process special key combinations and allow "chaining" of 
                * Ctrl-Tab commands if you keep holding control down.
                */
                if ((keyData & Keys.Control) != 0)
                {
                    bool sequentialTabbing = AppSettings.SequentialTabbing;
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
        public void OnSyntaxChange(string language)
        {
            TextEvent te = new TextEvent(EventType.SyntaxChange, language);
            EventManager.DispatchEvent(this, te);
        }

        /// <summary>
        /// Updates the MainForm's title automatically
        /// </summary>
        public void OnUpdateMainFormDialogTitle()
        {
            if (PluginBase.CurrentProject is {} project) Text = project.Name + " - " + DistroConfig.DISTRIBUTION_NAME;
            else if (CurrentDocument is { } document && document.IsEditable)
            {
                string file = Path.GetFileName(document.FileName);
                Text = file + " - " + DistroConfig.DISTRIBUTION_NAME;
            }
            else Text = DistroConfig.DISTRIBUTION_NAME;
        }

        /// <summary>
        /// Sets the current document unmodified and updates it
        /// </summary>
        public void OnDocumentReload(ITabbedDocument document)
        {
            document.IsModified = false;
            ReloadingDocument = false;
            OnUpdateMainFormDialogTitle();
            if (document.IsEditable) document.SciControl.MarkerDeleteAll(2);
            ButtonManager.UpdateFlaggedButtons();
        }

        /// <summary>
        /// Sets the current document modified
        /// </summary>
        public void OnDocumentModify(ITabbedDocument document)
        {
            if (!document.IsEditable || document.IsModified || ReloadingDocument || ProcessingContents) return;
            document.IsModified = true;
            TextEvent te = new TextEvent(EventType.FileModify, document.FileName);
            EventManager.DispatchEvent(this, te);
        }

        /// <summary>
        /// Notifies the plugins for the FileSave event and includes the given reason for the save.
        /// </summary>
        public void OnFileSave(ITabbedDocument document, string oldFile, string reason)
        {
            if (oldFile != null)
            {
                string args = document.FileName + ";" + oldFile;
                TextEvent rename = new TextEvent(EventType.FileRename, args);
                EventManager.DispatchEvent(this, rename);
                TextEvent open = new TextEvent(EventType.FileOpen, document.FileName);
                EventManager.DispatchEvent(this, open);
            }
            OnUpdateMainFormDialogTitle();
            if (document.IsEditable) document.SciControl.MarkerDeleteAll(2);
            TextDataEvent save = new TextDataEvent(EventType.FileSave, document.FileName, reason);
            EventManager.DispatchEvent(this, save);
            ButtonManager.UpdateFlaggedButtons();
            TabTextManager.UpdateTabTexts();
        }

        /// <summary>
        /// Notifies the plugins for the FileSave event
        /// </summary>
        public void OnFileSave(ITabbedDocument document, string oldFile) => OnFileSave(document, oldFile, null);

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
        public IPlugin FindPlugin(string guid) => PluginServices.Find(guid).Instance;

        /// <summary>
        /// Finds the specified composed/ready image that is automatically adjusted according to the theme.
        /// <para/>
        /// If you make a copy of the image returned by this method, the copy will not be automatically adjusted.
        /// </summary>
        public Image FindImage(string data) => FindImage(data, true);

        /// <summary>
        /// Finds the specified composed/ready image.
        /// <para/>
        /// If you make a copy of the image returned by this method, the copy will not be automatically adjusted, even if <code>autoAdjusted</code> is <code>true</code>.
        /// </summary>
        public Image FindImage(string data, bool autoAdjusted)
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
        public Image FindImage16(string data) => FindImage16(data, true);

        /// <summary>
        /// Finds the specified composed/ready image. The image size is always 16x16.
        /// <para/>
        /// If you make a copy of the image returned by this method, the copy will not be automatically adjusted, even if <code>autoAdjusted</code> is <code>true</code>.
        /// </summary>
        public Image FindImage16(string data, bool autoAdjusted)
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
        public Image FindImageAndSetAdjust(string data) => ImageSetAdjust(FindImage(data, false));

        /// <summary>
        /// Returns a copy of the specified image that has its color adjusted.
        /// </summary>
        public Image ImageSetAdjust(Image image) => ImageManager.SetImageAdjustment(image);

        /// <summary>
        /// Gets a copy of the image that gets automatically adjusted according to the theme.
        /// </summary>
        public Image GetAutoAdjustedImage(Image image) => ImageManager.GetAutoAdjustedImage(image);

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
                panel?.RefreshIcon();
            }
        }
        
        /// <summary>
        /// Themes the controls from the parent
        /// </summary>
        public void ThemeControls(object parent) => ThemeManager.WalkControls(parent);

        /// <summary>
        /// Gets a theme property color
        /// </summary>
        public Color GetThemeColor(string id) => ThemeManager.GetThemeColor(id);

        /// <summary>
        /// Gets a theme property color with a fallback
        /// </summary>
        public Color GetThemeColor(string id, Color fallback)
        {
            return ThemeManager.GetThemeColor(id) is {} color && color != Color.Empty ? color : fallback;
        }

        /// <summary>
        /// Gets a theme property value
        /// </summary>
        public string GetThemeValue(string id) => ThemeManager.GetThemeValue(id);

        /// <summary>
        /// Gets a theme property value with a fallback
        /// </summary>
        public string GetThemeValue(string id, string fallback)
        {
            var value = ThemeManager.GetThemeValue(id);
            return !string.IsNullOrEmpty(value) ? value : fallback;
        }

        /// <summary>
        /// Gets a theme flag value.
        /// </summary>
        public bool GetThemeFlag(string id) => GetThemeFlag(id, false);

        /// <summary>
        /// Gets a theme flag value with a fallback.
        /// </summary>
        public bool GetThemeFlag(string id, bool fallback)
        {
            var value = ThemeManager.GetThemeValue(id);
            if (string.IsNullOrEmpty(value)) return fallback;
            return value.ToLower() switch
            {
                "true" => true,
                "false" => false,
                _ => fallback,
            };
        }

        /// <summary>
        /// Sets if child controls should use theme.
        /// </summary>
        public void SetUseTheme(object obj, bool use) => ThemeManager.SetUseTheme(obj, use);

        /// <summary>
        /// Finds the specified menu item by name
        /// </summary>
        public ToolStripItem FindMenuItem(string name) => StripBarManager.FindMenuItem(name);

        /// <summary>
        /// Finds the menu items that have the specified name
        /// </summary>
        public List<ToolStripItem> FindMenuItems(string name) => StripBarManager.FindMenuItems(name);

        /// <summary>
        /// Lets you update menu items using the flag functionality
        /// </summary>
        public void AutoUpdateMenuItem(ToolStripItem item, string action)
        {
            bool value = ButtonManager.ValidateFlagAction(item, action);
            ButtonManager.ExecuteFlagAction(item, action, value);
        }

        /// <summary>
        /// Gets the specified item's shortcut keys.
        /// </summary>
        public Keys GetShortcutItemKeys(string id) => ShortcutManager.GetRegisteredItem(id)?.Custom ?? Keys.None;

        /// <summary>
        /// Gets the specified item's id.
        /// </summary>
        public string GetShortcutItemId(Keys keys) => ShortcutManager.GetRegisteredItem(keys)?.Id ?? string.Empty;

        /// <summary>
        /// Registers a new menu item with the shortcut manager
        /// </summary>
        public void RegisterShortcutItem(string id, Keys keys) => ShortcutManager.RegisterItem(id, keys);

        /// <summary>
        /// Registers a new menu item with the shortcut manager
        /// </summary>
        public void RegisterShortcutItem(string id, ToolStripMenuItem item) => ShortcutManager.RegisterItem(id, item);

        /// <summary>
        /// Registers a new secondary menu item with the shortcut manager
        /// </summary>
        public void RegisterSecondaryItem(string id, ToolStripItem item) => ShortcutManager.RegisterSecondaryItem(id, item);

        /// <summary>
        /// Updates a registered secondary menu item in the shortcut manager
        /// - should be called when the tooltip changes.
        /// </summary>
        public void ApplySecondaryShortcut(ToolStripItem item) => ShortcutManager.ApplySecondaryShortcut(item);

        /// <summary>
        /// Shows the settings dialog
        /// </summary>
        public void ShowSettingsDialog(string itemName) => SettingDialog.Show(itemName, "");

        public void ShowSettingsDialog(string itemName, string filter) => SettingDialog.Show(itemName, filter);

        /// <summary>
        /// Shows the error dialog if the sender is ErrorManager
        /// </summary>
        public void ShowErrorDialog(object sender, Exception ex)
        {
            if (sender.GetType().ToString() != "PluginCore.Managers.ErrorManager")
            {
                string message = TextHelper.GetString("Info.OnlyErrorManager");
                ErrorDialog.Show(new Exception(message));
            }
            else ErrorDialog.Show(ex);
        }

        /// <summary>
        /// Show a message to the user to restart FD
        /// </summary>
        public void RestartRequired()
        {
            if (restartButton != null) restartButton.Visible = true;
            RequiresRestart = true;
            string message = TextHelper.GetString("Info.RequiresRestart");
            TraceManager.Add(message);
        }

        /// <summary>
        /// Refreshes the main form
        /// </summary>
        public void RefreshUI()
        {
            if (CurrentDocument is null) return;
            OnScintillaControlUpdateControl(CurrentDocument.SciControl);
        }

        /// <summary>
        /// Clears the temporary files from disk
        /// </summary>
        public void ClearTemporaryFiles(string file)
        {
            RecoveryManager.RemoveTemporaryFile(file);
            FileStateManager.RemoveStateFile(file);
        }

        /// <summary>
        /// Refreshes the scintilla configuration
        /// </summary>
        public void RefreshSciConfig() => ScintillaManager.LoadConfiguration();

        /// <summary>
        /// Processes the argument string variables
        /// </summary>
        public string ProcessArgString(string args) => ArgsProcessor.ProcessString(args, true);

        public string ProcessArgString(string args, bool dispatch) => ArgsProcessor.ProcessString(args, dispatch);

        /// <summary>
        /// Processes the incoming arguments 
        /// </summary> 
        public void ProcessParameters(string[] args)
        {
            if (InvokeRequired)
            {
                BeginInvoke((MethodInvoker)(() => ProcessParameters(args)));
                return;
            }
            Activate();
            Focus();
            if (!args.IsNullOrEmpty())
            {
                Silent = args.Contains("-silent");
                foreach (var arg in args)
                {
                    OpenDocumentFromParameters(arg);
                }
            }
            if (Win32.ShouldUseWin32()) Win32.RestoreWindow(Handle);
            /**
            * Notify plugins about start arguments
            */
            NotifyEvent ne = new NotifyEvent(EventType.StartArgs);
            EventManager.DispatchEvent(this, ne);
        }

        /// <summary>
        /// 
        /// </summary>
        void OpenDocumentFromParameters(string file)
        {
            Match openParams = Regex.Match(file, "@([0-9]+)($|:([0-9]+)$)"); // path@line:col
            if (openParams.Success)
            {
                file = file.Substring(0, openParams.Index);
                file = PathHelper.GetLongPathName(file);
                if (File.Exists(file))
                {
                    TabbedDocument doc = OpenEditableDocument(file, false) as TabbedDocument;
                    if (doc != null) ApplyOpenParams(openParams, doc.SciControl);
                    else if (CurrentDocument.FileName == file) ApplyOpenParams(openParams, CurrentDocument.SciControl);
                }
            }
            else if (File.Exists(file))
            {
                file = PathHelper.GetLongPathName(file);
                OpenEditableDocument(file);
            }
        }

        /// <summary>
        /// 
        /// </summary>        
        void ApplyOpenParams(Match openParams, ScintillaControl sci)
        {
            if (sci is null) return;
            int col = 0;
            int line = Math.Min(sci.LineCount - 1, Math.Max(0, int.Parse(openParams.Groups[1].Value) - 1));
            if (openParams.Groups.Count > 3 && openParams.Groups[3].Value.Length > 0)
            {
                col = int.Parse(openParams.Groups[3].Value);
            }
            int position = sci.PositionFromLine(line) + col;
            sci.SetSel(position, position);
        }

        /// <summary>
        /// Closes all open documents with an option: exceptCurrent
        /// </summary>
        public void CloseAllDocuments(bool exceptCurrent) => CloseAllDocuments(exceptCurrent, false);

        public void CloseAllDocuments(bool exceptCurrent, bool exceptOtherPanes)
        {
            ITabbedDocument current = CurrentDocument;
            DockPane currentPane = current?.DockHandler.PanelPane;
            CloseAllCanceled = false; closingAll = true;
            var documents = new List<ITabbedDocument>(Documents);
            foreach (var document in documents)
            {
                var close = !(exceptCurrent && document == current);
                if (exceptOtherPanes && document.DockHandler.PanelPane != currentPane) close = false;
                if (close) document.Close();
            }
            closingAll = false;
        }

        /// <summary>
        /// Updates all needed settings after modification
        /// </summary>
        public void ApplyAllSettings()
        {
            if (InvokeRequired)
            {
                BeginInvoke((MethodInvoker) ApplyAllSettings);
                return;
            }
            ShortcutManager.ApplyAllShortcuts();
            EventManager.DispatchEvent(this, new NotifyEvent(EventType.ApplySettings));
            foreach (var document in Documents)
            {
                if (!document.IsEditable) continue;
                ScintillaManager.ApplySciSettings(document.SplitSci1, true);
                ScintillaManager.ApplySciSettings(document.SplitSci2, true);
            }
            frInFilesDialog.UpdateSettings();
            StatusStrip.Visible = AppSettings.ViewStatusBar;
            ToolStrip.Visible = !IsFullScreen && AppSettings.ViewToolBar;
            ButtonManager.UpdateFlaggedButtons();
            TabTextManager.UpdateTabTexts();
            ClipboardManager.ApplySettings();
            TraceManager.ApplySettings(AppSettings.MaxTraceLines);
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
                AppSettings.WindowState = WindowState;
                AppSettings.LatestDialogPath = workingDirectory;
                if (WindowState != FormWindowState.Maximized && WindowState != FormWindowState.Minimized)
                {
                    AppSettings.WindowSize = Size;
                    AppSettings.WindowPosition = Location;
                }
                if (!File.Exists(FileNameHelper.SettingData))
                {
                    string folder = Path.GetDirectoryName(FileNameHelper.SettingData);
                    if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
                }
                ObjectSerializer.Serialize(FileNameHelper.SettingData, AppSettings);
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
                DockPanel.SaveAsXml(FileNameHelper.LayoutData);
            }
            catch (Exception ex)
            {
                // Ignore errors on multi instance full close...
                if (MultiInstanceMode && ClosingEntirely) return;
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Resolves the working directory
        /// </summary>
        public string GetWorkingDirectory()
        {
            var project = PluginBase.CurrentProject;
            var document = CurrentDocument;
            if (document != null && document.IsEditable && File.Exists(document.FileName))
            {
                return Path.GetDirectoryName(document.FileName);
            }

            if (project != null && File.Exists(project.ProjectPath))
            {
                return Path.GetDirectoryName(project.ProjectPath);
            }
            return PathHelper.AppDir;
        }

        /// <summary>
        /// Gets the amount instances running
        /// </summary>
        public int GetInstanceCount()
        {
            Process current = Process.GetCurrentProcess();
            return Process.GetProcessesByName(current.ProcessName).Length;
        }

        /// <summary>
        /// Sets the text to find globally
        /// </summary>
        public void SetFindText(object sender, string text)
        {
            if (sender != quickFind) quickFind.SetFindText(text);
            if (sender != frInDocDialog) frInDocDialog.SetFindText(text);
        }

        /// <summary>
        /// Sets the case setting to find globally
        /// </summary>
        public void SetMatchCase(object sender, bool matchCase)
        {
            if (sender != quickFind) quickFind.SetMatchCase(matchCase);
            if (sender != frInDocDialog) frInDocDialog.SetMatchCase(matchCase);
        }

        /// <summary>
        /// Sets the whole word setting to find globally
        /// </summary>
        public void SetWholeWord(object sender, bool wholeWord)
        {
            if (sender != quickFind) quickFind.SetWholeWord(wholeWord);
            if (sender != frInDocDialog) frInDocDialog.SetWholeWord(wholeWord);
        }

        #endregion

        #region Click Handlers

        /// <summary>
        /// Creates a new blank document
        /// </summary>
        public void New(object sender, EventArgs e)
        {
            string fileName = DocumentManager.GetNewDocumentName(null);
            TextEvent te = new TextEvent(EventType.FileNew, fileName);
            EventManager.DispatchEvent(this, te);
            if (!te.Handled)
            {
                CreateEditableDocument(fileName, "", (int)AppSettings.DefaultCodePage);
            }
        }

        /// <summary>
        /// Creates a new blank document tracking current project
        /// </summary>
        public void SmartNew(object sender, EventArgs e)
        {
            string ext = "";
            if (PluginBase.CurrentProject != null)
            {
                try
                {
                    string filter = PluginBase.CurrentProject.DefaultSearchFilter;
                    string tempExt = filter.Split(';')[0].Replace("*.", "");
                    if (Regex.Match(tempExt, "^[A-Za-z0-9]+$").Success) ext = tempExt;
                }
                catch { /* NO ERRORS */ }
            }
            string fileName = DocumentManager.GetNewDocumentName(ext);
            TextEvent te = new TextEvent(EventType.FileNew, fileName);
            EventManager.DispatchEvent(this, te);
            if (!te.Handled)
            {
                CreateEditableDocument(fileName, "", (int)AppSettings.DefaultCodePage);
            }
        }

        /// <summary>
        /// Create a new document from a template
        /// </summary>
        public void NewFromTemplate(object sender, EventArgs e)
        {
            try
            {
                ToolStripItem button = (ToolStripItem)sender;
                string[] args = ProcessArgString(((ItemData)button.Tag).Tag).Split(';');
                Encoding encoding = Encoding.GetEncoding((int)AppSettings.DefaultCodePage);
                string fileName = DocumentManager.GetNewDocumentName(args[0]);
                string contents = FileHelper.ReadFile(args[1], encoding);
                string lineEndChar = LineEndDetector.GetNewLineMarker((int)Settings.EOLMode);
                contents = Regex.Replace(contents, @"\r\n?|\n", lineEndChar);
                string processed = ProcessArgString(contents);
                ActionPoint actionPoint = SnippetHelper.ProcessActionPoint(processed);
                if (Documents.Length == 1 && Documents[0].IsUntitled)
                {
                    closingForOpenFile = true;
                    Documents[0].Close();
                    closingForOpenFile = false;
                }
                TextEvent te = new TextEvent(EventType.FileTemplate, fileName);
                EventManager.DispatchEvent(this, te);
                if (!te.Handled)
                {
                    ITabbedDocument document = (ITabbedDocument)CreateEditableDocument(fileName, actionPoint.Text, encoding.CodePage);
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
        public void FileFromTemplate(string templatePath, string newFilePath)
        {
            try
            {
                var encoding = Encoding.GetEncoding((int)AppSettings.DefaultCodePage);
                var contents = FileHelper.ReadFile(templatePath);
                var processed = ProcessArgString(contents);
                var lineEndChar = LineEndDetector.GetNewLineMarker((int)Settings.EOLMode);
                processed = Regex.Replace(processed, @"\r\n?|\n", lineEndChar);
                var actionPoint = SnippetHelper.ProcessActionPoint(processed);
                FileHelper.WriteFile(newFilePath, actionPoint.Text, encoding, Globals.Settings.SaveUnicodeWithBOM);
                if (actionPoint.EntryPosition != -1)
                {
                    if (Documents.Length == 1 && Documents[0].IsUntitled)
                    {
                        closingForOpenFile = true;
                        Documents[0].Close();
                        closingForOpenFile = false;
                    }
                    var te = new TextEvent(EventType.FileTemplate, newFilePath);
                    EventManager.DispatchEvent(this, te);
                    if (!te.Handled)
                    {
                        var document = (ITabbedDocument)CreateEditableDocument(newFilePath, actionPoint.Text, encoding.CodePage);
                        SnippetHelper.ExecuteActionPoint(actionPoint, document.SciControl);
                    }
                }
                else
                {
                    var te = new TextEvent(EventType.FileTemplate, newFilePath);
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
        public void Open(object sender, EventArgs e)
        {
            openFileDialog.Multiselect = true;
            openFileDialog.InitialDirectory = WorkingDirectory;
            if (openFileDialog.ShowDialog(this) == DialogResult.OK && openFileDialog.FileName.Length != 0)
            {
                int count = openFileDialog.FileNames.Length;
                for (int i = 0; i < count; i++)
                {
                    OpenEditableDocument(openFileDialog.FileNames[i]);
                }
            }
            openFileDialog.Multiselect = false;
        }

        /// <summary>
        /// Opens the open file dialog and opens the selected files in specific encoding
        /// </summary>
        public void OpenIn(object sender, EventArgs e)
        {
            ToolStripItem button = (ToolStripItem)sender;
            int encMode = Convert.ToInt32(((ItemData)button.Tag).Tag);
            Encoding encoding = Encoding.GetEncoding(encMode);
            openFileDialog.Multiselect = true; // Allow multiple...
            openFileDialog.InitialDirectory = WorkingDirectory;
            if (openFileDialog.ShowDialog(this) == DialogResult.OK && openFileDialog.FileName.Length != 0)
            {
                int count = openFileDialog.FileNames.Length;
                for (int i = 0; i < count; i++)
                {
                    if (encMode == 0) // Detect 8bit encoding...
                    {
                        int codepage = FileHelper.GetFileCodepage(openFileDialog.FileNames[i]);
                        encoding = Encoding.GetEncoding(codepage);
                    }
                    OpenEditableDocument(openFileDialog.FileNames[i], encoding, false);
                }
            }
            openFileDialog.Multiselect = false;
        }

        /// <summary>
        /// Opens a the specified file in the UI
        /// </summary>
        public void Edit(object sender, EventArgs e)
        {
            ToolStripItem button = (ToolStripItem)sender;
            string file = ProcessArgString(((ItemData)button.Tag).Tag);
            if (File.Exists(file)) OpenEditableDocument(file);
        }

        /// <summary>
        /// Reopens a file from the old documents list
        /// </summary>
        public void Reopen(object sender, EventArgs e)
        {
            ToolStripItem button = (ToolStripItem)sender;
            string file = button.Tag.ToString();
            if (File.Exists(file))
            {
                OpenEditableDocument(file);
                ButtonManager.AddNewReopenMenuItem(file);
            }
            else
            {
                string message = TextHelper.GetString("Info.InvalidFileOnReopen");
                Settings.PreviousDocuments.Remove(file);
                ButtonManager.PopulateReopenMenu();
                ErrorManager.ShowInfo(message);
            }
        }

        /// <summary>
        /// Opens the last closed tabs if they are not open
        /// </summary>
        public void ReopenClosed(object sender, EventArgs e) => OldTabsManager.OpenOldTabDocument();

        /// <summary>
        /// Clears invalid entries from the old documents list
        /// </summary>
        public void CleanReopenList(object sender, EventArgs e)
        {
            FileHelper.FilterByExisting(AppSettings.PreviousDocuments, true);
            ButtonManager.PopulateReopenMenu();
        }

        /// <summary>
        /// Clears all entries from the old documents list
        /// </summary>
        public void ClearReopenList(object sender, EventArgs e)
        {
            AppSettings.PreviousDocuments.Clear();
            ButtonManager.PopulateReopenMenu();
        }

        /// <summary>
        /// Views the current clipboard history
        /// </summary>
        public void ClipboardHistory(object sender, EventArgs e)
        {
            if (ClipboardHistoryDialog.Show(out var data))
            {
                CurrentDocument.SciControl.ReplaceSel(data.Text);
            }
        }

        /// <summary>
        /// Pastes lines at the correct indent level
        /// </summary>
        public void SmartPaste(object sender, EventArgs e)
        {
            var sci = CurrentDocument.SciControl;
            if (!sci.CanPaste) return;
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

        /// <summary>
        /// Saves the current file session
        /// </summary>
        public void SaveSession(object sender, EventArgs e)
        {
            try
            {
                ToolStripItem button = (ToolStripItem)sender;
                string file = ((ItemData)button.Tag).Tag;
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
        public void RestoreSession(object sender, EventArgs e)
        {
            try
            {
                ToolStripItem button = (ToolStripItem)sender;
                string file = ((ItemData)button.Tag).Tag;
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
        public void RestoreLayout(object sender, EventArgs e)
        {
            try
            {
                ToolStripItem button = (ToolStripItem)sender;
                string file = ((ItemData)button.Tag).Tag;
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
        public void Save(object sender, EventArgs e)
        {
            try
            {
                if (CurrentDocument.IsUntitled)
                {
                    saveFileDialog.FileName = CurrentDocument.FileName;
                    saveFileDialog.InitialDirectory = WorkingDirectory;
                    if (saveFileDialog.ShowDialog(this) == DialogResult.OK && saveFileDialog.FileName.Length != 0)
                    {
                        ButtonManager.AddNewReopenMenuItem(saveFileDialog.FileName);
                        CurrentDocument.Save(saveFileDialog.FileName);
                        NotifyEvent ne = new NotifyEvent(EventType.FileSwitch);
                        EventManager.DispatchEvent(this, ne);
                        NotifyEvent ce = new NotifyEvent(EventType.Completion);
                        EventManager.DispatchEvent(this, ce);
                    }
                }
                else if (CurrentDocument.IsModified)
                {
                    var button = (ToolStripItem)sender;
                    var reason = ((ItemData)button.Tag).Tag;
                    CurrentDocument.Save(CurrentDocument.FileName, reason);
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
        public void SaveAs(object sender, EventArgs e)
        {
            saveFileDialog.FileName = CurrentDocument.FileName;
            saveFileDialog.InitialDirectory = WorkingDirectory;
            if (saveFileDialog.ShowDialog(this) == DialogResult.OK && saveFileDialog.FileName.Length != 0)
            {
                ButtonManager.AddNewReopenMenuItem(saveFileDialog.FileName);
                CurrentDocument.Save(saveFileDialog.FileName);
                NotifyEvent ne = new NotifyEvent(EventType.FileSwitch);
                EventManager.DispatchEvent(this, ne);
                NotifyEvent ce = new NotifyEvent(EventType.Completion);
                EventManager.DispatchEvent(this, ce);
            }
        }

        /// <summary>
        /// Saves the selected text as a snippet
        /// </summary>
        public void SaveAsSnippet(object sender, EventArgs e)
        {
            try
            {
                saveFileDialog.FileName = "";
                saveFileDialog.DefaultExt = ".fds";
                string prevFilter = saveFileDialog.Filter;
                string snippetFilter = TextHelper.GetString("Info.SnippetFilter");
                saveFileDialog.Filter = snippetFilter + "|*.fds";
                string prevRootPath = saveFileDialog.InitialDirectory;
                saveFileDialog.InitialDirectory = PathHelper.SnippetDir;
                if (saveFileDialog.ShowDialog(this) == DialogResult.OK && saveFileDialog.FileName.Length != 0)
                {
                    string contents = CurrentDocument.SciControl.SelText;
                    string file = saveFileDialog.FileName;
                    FileHelper.WriteFile(file, contents, Encoding.UTF8);
                }
                saveFileDialog.InitialDirectory = prevRootPath;
                saveFileDialog.Filter = prevFilter;
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Saves the selected text as a template
        /// </summary>
        public void SaveAsTemplate(object sender, EventArgs e)
        {
            try
            {
                saveFileDialog.FileName = "";
                saveFileDialog.DefaultExt = ".fdt";
                string prevFilter = saveFileDialog.Filter;
                string templateFilter = TextHelper.GetString("Info.TemplateFilter");
                saveFileDialog.Filter = templateFilter + "|*.fdt";
                string prevRootPath = saveFileDialog.InitialDirectory;
                saveFileDialog.InitialDirectory = PathHelper.TemplateDir;
                if (saveFileDialog.ShowDialog(this) == DialogResult.OK && saveFileDialog.FileName.Length != 0)
                {
                    string contents = CurrentDocument.SciControl.SelText;
                    string file = saveFileDialog.FileName;
                    FileHelper.WriteFile(file, contents, Encoding.UTF8);
                }
                saveFileDialog.InitialDirectory = prevRootPath;
                saveFileDialog.Filter = prevFilter;
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Saves all modified documents or opens a save file dialog
        /// </summary>
        public void SaveAll(object sender, EventArgs e)
        {
            try 
            {
                SavingMultiple = true;
                ITabbedDocument[] documents = Documents;
                ITabbedDocument active = CurrentDocument;
                foreach (var current in documents)
                {
                    if (current.IsEditable && current.IsModified)
                    {
                        if (current.IsUntitled)
                        {
                            saveFileDialog.FileName = current.FileName;
                            saveFileDialog.InitialDirectory = WorkingDirectory;
                            if (saveFileDialog.ShowDialog(this) == DialogResult.OK && saveFileDialog.FileName.Length != 0)
                            {
                                ButtonManager.AddNewReopenMenuItem(saveFileDialog.FileName);
                                current.Save(saveFileDialog.FileName);
                            }
                        }
                        else current.Save();
                    }
                }
                SavingMultiple = false;
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
        public void SaveAllModified(object sender, EventArgs e)
        {
            try
            {
                string filter = "*";
                SavingMultiple = true;
                ToolStripItem button = (ToolStripItem)sender;
                filter = ((ItemData)button.Tag).Tag + filter;
                ITabbedDocument[] documents = Documents;
                ITabbedDocument active = CurrentDocument;
                foreach (var current in documents)
                {
                    if (current.IsEditable && current.IsModified && !current.IsUntitled && current.Text.EndsWithOrdinal(filter))
                    {
                        current.Save();
                        current.IsModified = false;
                    }
                }
                SavingMultiple = false;
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
        public void Close(object sender, EventArgs e) => CurrentDocument.Close();

        /// <summary>
        /// Closes all open documents in the current pane
        /// </summary>
        public void CloseAll(object sender, EventArgs e) => CloseAllDocuments(false, true);

        /// <summary>
        /// Closes all open documents except the current in the current pane
        /// </summary>
        public void CloseOthers(object sender, EventArgs e) => CloseAllDocuments(true, true);

        /// <summary>
        /// Exits the application
        /// </summary>
        public void Exit(object sender, EventArgs e) => Close();

        /// <summary>
        /// Duplicates the current document
        /// </summary>
        public void Duplicate(object sender, EventArgs e)
        {
            ScintillaControl sci = CurrentDocument.SciControl;
            string extension = Path.GetExtension(sci.FileName);
            string filename = DocumentManager.GetNewDocumentName(extension);
            DockContent document = CreateEditableDocument(filename, sci.Text, sci.Encoding.CodePage);
            ((TabbedDocument)document).IsModified = true;
        }

        /// <summary>
        /// Reverts the document to the default state
        /// </summary>
        public void Revert(object sender, EventArgs e) => CurrentDocument.Revert(true);

        /// <summary>
        /// Reloads the current document
        /// </summary>
        public void Reload(object sender, EventArgs e) => CurrentDocument.Reload(true);

        /// <summary>
        /// Prints the current document
        /// </summary>
        public void Print(object sender, EventArgs e)
        {
            if (CurrentDocument.SciControl.TextLength == 0)
            {
                string message = TextHelper.GetString("Info.NothingToPrint");
                ErrorManager.ShowInfo(message);
                return;
            }
            try
            {
                printDialog.PrinterSettings = PrintingManager.GetPrinterSettings();
                printDialog.Document = PrintingManager.CreatePrintDocument();
                if (printDialog.ShowDialog(this) == DialogResult.OK)
                {
                    printDialog.Document.Print();
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
        public void PrintPreview(object sender, EventArgs e)
        {
            if (CurrentDocument.SciControl.TextLength == 0)
            {
                string message = TextHelper.GetString("Info.NothingToPrint");
                ErrorManager.ShowInfo(message);
                return;
            }
            try
            {
                printDialog.PrinterSettings = PrintingManager.GetPrinterSettings();
                printPreviewDialog.Document = PrintingManager.CreatePrintDocument();
                printPreviewDialog.ShowDialog(this);
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Opens a goto dialog
        /// </summary>
        public void GoTo(object sender, EventArgs e)
        {
            if (!gotoDialog.Visible) gotoDialog.Show();
            else gotoDialog.Activate();
        }

        /// <summary>
        /// Displays the next result
        /// </summary>
        public void FindNext(object sender, EventArgs e)
        {
            bool update = !Globals.Settings.DisableFindTextUpdating;
            bool simple = !Globals.Settings.DisableSimpleQuickFind && !quickFind.Visible;
            frInDocDialog.FindNext(true, update, simple);
        }

        /// <summary>
        /// Displays the previous result
        /// </summary>
        public void FindPrevious(object sender, EventArgs e)
        {
            bool update = !Globals.Settings.DisableFindTextUpdating;
            bool simple = !Globals.Settings.DisableSimpleQuickFind && !quickFind.Visible;
            frInDocDialog.FindNext(false, update, simple);
        }

        /// <summary>
        /// Opens a find and replace dialog
        /// </summary>
        public void FindAndReplace(object sender, EventArgs e)
        {
            if (!frInDocDialog.Visible) frInDocDialog.Show();
            else
            {
                frInDocDialog.InitializeFindText();
                frInDocDialog.Activate();
            }
        }

        /// <summary>
        /// Opens a find and replace dialog with a location
        /// </summary>
        public void FindAndReplaceFrom(object sender, EventArgs e)
        {
            ToolStripItem button = (ToolStripItem)sender;
            string file = ((ItemData)button.Tag).Tag;
            BeginInvoke((MethodInvoker)(() => OpenEditableDocument(file)));
            if (!frInDocDialog.Visible) frInDocDialog.Show();
            else frInDocDialog.Activate();
        }

        /// <summary>
        /// Opens a find and replace in files dialog
        /// </summary>
        public void FindAndReplaceInFiles(object sender, EventArgs e)
        {
            if (!frInFilesDialog.Visible) frInFilesDialog.Show();
            else
            {
                frInFilesDialog.UpdateFindText();
                frInFilesDialog.Activate();
            }
        }

        /// <summary>
        /// Opens a find and replace in files dialog with a location
        /// </summary>
        public void FindAndReplaceInFilesFrom(object sender, EventArgs e)
        {
            ToolStripItem button = (ToolStripItem)sender;
            string path = ((ItemData)button.Tag).Tag;
            if (!frInFilesDialog.Visible) frInFilesDialog.Show(); // Show first..
            else frInFilesDialog.Activate();
            frInFilesDialog.SetFindPath(path);
        }

        /// <summary>
        /// Shows the quick find control
        /// </summary>
        public void QuickFind(object sender, EventArgs e) => quickFind.ShowControl();

        /// <summary>
        /// Opens the edit shortcut dialog
        /// </summary>
        public void EditShortcuts(object sender, EventArgs e) => ShortcutDialog.Show();

        /// <summary>
        /// Opens the edit snippet dialog
        /// </summary>
        public void EditSnippets(object sender, EventArgs e) => SnippetDialog.Show();

        /// <summary>
        /// Opens the edit syntax dialog
        /// </summary>
        public void EditSyntax(object sender, EventArgs e) => EditorDialog.Show();

        /// <summary>
        /// Opens the about dialog
        /// </summary>
        public void About(object sender, EventArgs e) => AboutDialog.Show();

        /// <summary>
        /// Opens the settings dialog
        /// </summary>
        public void ShowSettings(object sender, EventArgs e)
        {
            SettingDialog.Show(DistroConfig.DISTRIBUTION_NAME, "");
        }

        /// <summary>
        /// Shows the application in fullscreen or normal mode
        /// </summary>
        public void ToggleFullScreen(object sender, EventArgs e)
        {
            if (IsFullScreen)
            {
                formState.Restore(this);
                if (AppSettings.ViewToolBar) ToolStrip.Visible = true;
                foreach (DockPane pane in DockPanel.Panes)
                {
                    if (fullScreenDocks[pane] != null)
                    {
                        pane.DockState = (DockState)fullScreenDocks[pane];
                    }
                }
                IsFullScreen = false;
            } 
            else 
            {
                formState.Maximize(this);
                ToolStrip.Visible = false;
                fullScreenDocks = new Hashtable();
                foreach (DockPane pane in DockPanel.Panes)
                {
                    fullScreenDocks[pane] = pane.DockState;
                    pane.DockState = pane.DockState switch
                    {
                        DockState.DockLeft => DockState.DockLeftAutoHide,
                        DockState.DockRight => DockState.DockRightAutoHide,
                        DockState.DockBottom => DockState.DockBottomAutoHide,
                        DockState.DockTop => DockState.DockTopAutoHide,
                        _ => pane.DockState,
                    };
                }
                IsFullScreen = true;
            }
            ButtonManager.UpdateFlaggedButtons();
        }

        /// <summary>
        /// Extracts a zip file by extending paths with fd args
        /// </summary>
        public void ExtractZip(object sender, EventArgs e)
        {
            try 
            {
                string zipLog = string.Empty;
                string zipFile = string.Empty;
                bool requiresRestart = false;
                bool silentInstall = Silent;
                ToolStripItem button = (ToolStripItem)sender;
                string[] chunks = (((ItemData)button.Tag).Tag).Split(';');
                if (chunks.Length > 1)
                {
                    zipFile = chunks[0];
                    silentInstall = chunks[1] == "true";
                }
                else zipFile = chunks[0];
                if (!File.Exists(zipFile)) return; // Skip missing file...
                string caption = TextHelper.GetString("Title.ConfirmDialog");
                string message = TextHelper.GetString("Info.ZipConfirmExtract") + "\n" + zipFile;
                if (silentInstall || MessageBox.Show(message, caption, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                {
                    ZipEntry entry;
                    zipLog += "FDZ: " + zipFile + "\r\n";
                    ZipInputStream zis = new ZipInputStream(new FileStream(zipFile, FileMode.Open, FileAccess.Read));
                    while ((entry = zis.GetNextEntry()) != null)
                    {
                        byte[] data = new byte[2048];
                        string fdpath = ProcessArgString(entry.Name, false).Replace("/", "\\");
                        if (entry.IsFile)
                        {
                            string ext = Path.GetExtension(fdpath);
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
                            string dirPath = Path.GetDirectoryName(fdpath);
                            if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);
                            FileStream extracted = new FileStream(fdpath, FileMode.Create);
                            while (true)
                            {
                                var size = zis.Read(data, 0, data.Length);
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
                    string finish = TextHelper.GetString("Info.ZipExtractDone");
                    string restart = TextHelper.GetString("Info.RequiresRestart");
                    if (requiresRestart)
                    {
                        zipLog += "Restart required.\r\n";
                        if (!silentInstall) finish += "\n" + restart;
                        RestartRequired();
                    }
                    string logFile = Path.Combine(PathHelper.BaseDir, "Extensions.log");
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
        public void RemoveZip(object sender, EventArgs e)
        {
            try
            {
                string zipLog = string.Empty;
                string zipFile = string.Empty;
                bool requiresRestart = false;
                bool silentRemove = Silent;
                List<string> removeDirs = new List<string>();
                ToolStripItem button = (ToolStripItem)sender;
                string[] chunks = (((ItemData)button.Tag).Tag).Split(';');
                if (chunks.Length > 1)
                {
                    zipFile = chunks[0];
                    silentRemove = chunks[1] == "true";
                }
                else zipFile = chunks[0];
                if (!File.Exists(zipFile)) return; // Skip missing file...
                string caption = TextHelper.GetString("Title.ConfirmDialog");
                string message = TextHelper.GetString("Info.ZipConfirmRemove") + "\n" + zipFile;
                if (silentRemove || MessageBox.Show(message, caption, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
                {
                    ZipEntry entry;
                    zipLog += "FDZ: " + zipFile + "\r\n";
                    ZipInputStream zis = new ZipInputStream(new FileStream(zipFile, FileMode.Open, FileAccess.Read));
                    while ((entry = zis.GetNextEntry()) != null)
                    {
                        string fdpath = ProcessArgString(entry.Name, false).Replace("/", "\\");
                        if (entry.IsFile)
                        {
                            string ext = Path.GetExtension(fdpath);
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
                    foreach (string dir in removeDirs)
                    {
                        if (FolderHelper.IsDirectoryEmpty(dir) && !DirIsImportant(dir))
                        {
                            zipLog += "Remove: " + dir + "\r\n";
                            try { Directory.Delete(dir); }
                            catch { /* NO ERRORS */ }
                        }
                    }
                    string finish = TextHelper.GetString("Info.ZipRemoveDone");
                    string restart = TextHelper.GetString("Info.RequiresRestart");
                    if (requiresRestart)
                    {
                        zipLog += "Restart required.\r\n";                        
                        if (!silentRemove) finish += "\n" + restart;
                        RestartRequired();
                    }
                    string logFile = Path.Combine(PathHelper.BaseDir, "Extensions.log");
                    File.AppendAllText(logFile, zipLog + "Done.\r\n\r\n", Encoding.UTF8);
                    if (!silentRemove) ErrorManager.ShowInfo(finish);
                }
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        bool DirIsImportant(string dir)
        {
            var full = Path.GetDirectoryName(dir);
            return full == PathHelper.UserPluginDir
                || full == PathHelper.UserLibraryDir
                || full == PathHelper.UserProjectsDir;
        }

        /// <summary>
        /// Opens the browser with the specified file
        /// </summary>
        public void Browse(object sender, EventArgs e)
        {
            var browser = new Browser {Dock = DockStyle.Fill};
            if (sender != null)
            {
                var button = (ToolStripItem)sender;
                var url = ProcessArgString(((ItemData)button.Tag).Tag);
                CreateCustomDocument(browser);
                if (url.Trim().Length > 0) browser.WebBrowser.Navigate(url);
                else browser.WebBrowser.GoHome();
            }
            else browser.WebBrowser.GoHome();
        }

        /// <summary>
        /// Opens the home page in browser
        /// </summary>
        public void ShowHome(object sender, EventArgs e) => CallCommand("Browse", DistroConfig.DISTRIBUTION_HOME);

        /// <summary>
        /// Opens the help page in browser
        /// </summary>
        public void ShowHelp(object sender, EventArgs e) => CallCommand("Browse", DistroConfig.DISTRIBUTION_HELP);

        /// <summary>
        /// Opens the arguments dialog
        /// </summary>
        public void ShowArguments(object sender, EventArgs e) => ArgumentDialog.Show();

        /// <summary>
        /// Checks for updates from flashdevelop.org
        /// </summary>
        public void CheckUpdates(object sender, EventArgs e) => UpdateDialog.Show(false);

        /// <summary>
        /// Toggles the currect document to and from split view
        /// </summary>
        public void ToggleSplitView(object sender, EventArgs e)
        {
            if (!CurrentDocument.IsEditable) return;
            CurrentDocument.IsSplitted = !CurrentDocument.IsSplitted;
            ButtonManager.UpdateFlaggedButtons();
        }

        /// <summary>
        /// Moves the user to the matching brace
        /// </summary>
        public void GoToMatchingBrace(object sender, EventArgs e)
        {
            var sci = CurrentDocument.SciControl;
            var curPos = sci.CurrentPos;
            char c;
            if (curPos > 0)
            {
                c = (char) sci.CharAt(curPos - 1);
                if (c  == '{' || c == '[' || c == '(' || c == ')' || c == ']' || c == '}') curPos--;
            }
            c = (char)sci.CharAt(curPos);
            if (c != '{' && c != '[' && c != '(' && c != '}' && c != ']' && c != ')') curPos--;
            var bracePosEnd = sci.BraceMatch(curPos);
            if (bracePosEnd != -1) sci.GotoPos(bracePosEnd + 1);
        }

        /// <summary>
        /// Adds or removes a bookmark
        /// </summary>
        public void ToggleBookmark(object sender, EventArgs e)
        {
            MarkerManager.ToggleMarker(CurrentDocument.SciControl, 0, CurrentDocument.SciControl.CurrentLine);
        }

        /// <summary>
        /// Moves the cursor to the next bookmark
        /// </summary>
        public void NextBookmark(object sender, EventArgs e)
        {
            MarkerManager.NextMarker(CurrentDocument.SciControl, 0, CurrentDocument.SciControl.CurrentLine);
        }

        /// <summary>
        /// Moves the cursor to the previous bookmark
        /// </summary>
        public void PrevBookmark(object sender, EventArgs e)
        {
            MarkerManager.PreviousMarker(CurrentDocument.SciControl, 0, CurrentDocument.SciControl.CurrentLine);
        }

        /// <summary>
        /// Removes all bookmarks
        /// </summary>
        public void ClearBookmarks(object sender, EventArgs e)
        {
            var sci = CurrentDocument.SciControl;
            sci.MarkerDeleteAll(0);
            UITools.Manager.MarkerChanged(sci, -1);
            ButtonManager.UpdateFlaggedButtons();
        }

        /// <summary>
        /// Converts all end of line characters
        /// </summary>
        public void ConvertEOL(object sender, EventArgs e)
        {
            try
            {
                var button = (ToolStripItem)sender;
                var sci = CurrentDocument.SciControl;
                var eolMode = Convert.ToInt32(((ItemData)button.Tag).Tag);
                sci.ConvertEOLs(eolMode);
                sci.EOLMode = eolMode;
                OnScintillaControlUpdateControl(sci);
                OnDocumentModify(CurrentDocument);
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Toggles the folding of the editor
        /// </summary>
        public void ToggleFold(object sender, EventArgs e)
        {
            var sci = CurrentDocument.SciControl;
            var pos = sci.CurrentPos;
            var line = sci.LineFromPosition(pos);
            sci.ToggleFold(line);
        }

        /// <summary>
        /// Toggles a boolean setting
        /// </summary>
        public void ToggleBooleanSetting(object sender, EventArgs e)
        {
            try
            {
                ToolStripItem button = (ToolStripItem)sender;
                string settingKey = ((ItemData)button.Tag).Tag;
                bool value = (bool)AppSettings.GetValue(settingKey);
                AppSettings.SetValue(settingKey, !value);
                ApplyAllSettings();
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Changes the encoding of the current document
        /// </summary>
        public void ChangeEncoding(object sender, EventArgs e)
        {
            try
            {
                ToolStripItem button = (ToolStripItem)sender;
                ScintillaControl sci = CurrentDocument.SciControl;
                int encMode = Convert.ToInt32(((ItemData)button.Tag).Tag);
                sci.Encoding = Encoding.GetEncoding(encMode);
                OnScintillaControlUpdateControl(sci);
                OnDocumentModify(CurrentDocument);
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        public void ToggleSaveBOM(object sender, EventArgs e)
        {
            try
            {
                ScintillaControl sci = CurrentDocument.SciControl;
                sci.SaveBOM = !sci.SaveBOM;
                OnScintillaControlUpdateControl(sci);
                OnDocumentModify(CurrentDocument);
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Converts the encoding of the current document
        /// </summary>
        public void ConvertEncoding(object sender, EventArgs e)
        {
            try
            {
                var button = (ToolStripItem)sender;
                var encMode = Convert.ToInt32(((ItemData)button.Tag).Tag);
                var sci = CurrentDocument.SciControl;
                var curMode = sci.Encoding.CodePage; // From current..
                var converted = DataConverter.ChangeEncoding(sci.Text, curMode, encMode);
                sci.Encoding = Encoding.GetEncoding(encMode);
                sci.Text = converted; // Set after codepage change
                OnScintillaControlUpdateControl(sci);
                OnDocumentModify(CurrentDocument);
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Inserts a snippet to the current position
        /// </summary>
        public void InsertSnippet(object sender, EventArgs e)
        {
            try
            {
                ToolStripItem button = (ToolStripItem)sender;
                string word = (((ItemData)button.Tag).Tag);
                SnippetManager.InsertTextByWord(word != "null" ? word : null, false);
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Inserts a new GUID to the editor
        /// </summary>
        public void InsertGUID(object sender, EventArgs e)
        {
            string guid = Guid.NewGuid().ToString();
            CurrentDocument.SciControl.ReplaceSel(guid);
        }

        /// <summary>
        /// Inserts a custom hash to the editor
        /// </summary>
        public void InsertHash(object sender, EventArgs e)
        {
            using HashDialog cd = new HashDialog();
            if (cd.ShowDialog() == DialogResult.OK)
            {
                CurrentDocument.SciControl.ReplaceSel(cd.HashResultText);
            }
        }

        /// <summary>
        /// Inserts a color to the editor
        /// </summary>
        public void InsertColor(object sender, EventArgs e)
        {
            try
            {
                bool hasPrefix = true;
                bool isAsterisk = false;
                ScintillaControl sci = CurrentDocument.SciControl;
                if (sci.SelText.Length > 0)
                {
                    isAsterisk = sci.SelText.StartsWith('#');
                    if (sci.SelText.StartsWithOrdinal("0x") && sci.SelText.Length == 8)
                    {
                        int convertedColor = DataConverter.StringToColor(sci.SelText);
                        colorDialog.Color = ColorTranslator.FromWin32(convertedColor);
                    }
                    else if (sci.SelText.StartsWith('#') && sci.SelText.Length == 7)
                    {
                        string foundColor = sci.SelText.Replace("#", "0x");
                        int convertedColor = DataConverter.StringToColor(foundColor);
                        colorDialog.Color = ColorTranslator.FromWin32(convertedColor);
                    }
                    else if (sci.SelText.Length == 6)
                    {
                        hasPrefix = false;
                        int convertedColor = DataConverter.StringToColor("0x" + sci.SelText);
                        colorDialog.Color = ColorTranslator.FromWin32(convertedColor);
                    }
                }
                if (colorDialog.ShowDialog(this) == DialogResult.OK)
                {
                    string colorText = DataConverter.ColorToHex(colorDialog.Color);
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
        public void InsertFileDetails(object sender, EventArgs e)
        {
            try
            {
                FileInfo fileInfo = new FileInfo(CurrentDocument.FileName);
                string message = TextHelper.GetString("Info.FileDetails");
                string newline = LineEndDetector.GetNewLineMarker(CurrentDocument.SciControl.EOLMode);
                string path = fileInfo.FullName;
                string created = fileInfo.CreationTime.ToString();
                string modified = fileInfo.LastWriteTime.ToString();
                string size = fileInfo.Length.ToString();
                string info = string.Format(message, newline, path, created, modified, size);
                CurrentDocument.SciControl.ReplaceSel(info);
            }
            catch
            {
                string message = TextHelper.GetString("Info.NoFileInfoAvailable");
                ErrorManager.ShowInfo(message);
            }
        }

        /// <summary>
        /// Inserts a global timestamp to the current position
        /// </summary>
        public void InsertTimestamp(object sender, EventArgs e)
        {
            try
            {
                DateTime dateTime = DateTime.Now;
                ToolStripItem button = (ToolStripItem)sender;
                string date = (((ItemData)button.Tag).Tag);
                string currentDate = dateTime.ToString(date);
                CurrentDocument.SciControl.ReplaceSel(currentDate);
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Inserts text from the specified file to the current position
        /// </summary>
        public void InsertFile(object sender, EventArgs e)
        {
            try
            {
                ToolStripItem button = (ToolStripItem)sender;
                string file = ProcessArgString(((ItemData)button.Tag).Tag);
                if (File.Exists(file))
                {
                    Encoding to = CurrentDocument.SciControl.Encoding;
                    EncodingFileInfo info = FileHelper.GetEncodingFileInfo(file);
                    if (info.CodePage == -1) return; // If the file is locked, stop.
                    string contents = DataConverter.ChangeEncoding(info.Contents, info.CodePage, to.CodePage);
                    CurrentDocument.SciControl.ReplaceSel(contents);
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
        public void ChangeSyntax(object sender, EventArgs e)
        {
            try
            {
                ScintillaControl sci = CurrentDocument.SciControl;
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
        public void SortLines(object sender, EventArgs e)
        {
            ScintillaControl sci = CurrentDocument.SciControl;
            int curLine = sci.LineFromPosition(sci.SelectionStart);
            int endLine = sci.LineFromPosition(sci.SelectionEnd);
            List<string> lines = new List<string>();
            for (int line = curLine; line < endLine + 1; ++line)
            {
                lines.Add(sci.GetLine(line));
            }
            lines.Sort(CompareLines);
            StringBuilder result = new StringBuilder();
            foreach (string s in lines)
            {
                result.Append(s);
            }
            int selStart = sci.PositionFromLine(curLine);
            int selEnd = sci.PositionFromLine(endLine) + sci.MBSafeTextLength(sci.GetLine(endLine));
            sci.SetSel(selStart, selEnd);
            sci.ReplaceSel(result.ToString());
        }

        /// <summary>
        /// Sorts the currently selected lines in groups
        /// </summary>
        public void SortLineGroups(object sender, EventArgs e)
        {
            ScintillaControl sci = CurrentDocument.SciControl;
            int curLine = sci.LineFromPosition(sci.SelectionStart);
            int endLine = sci.LineFromPosition(sci.SelectionEnd);
            List<List<string>> lineLists = new List<List<string>>();
            List<string> curList = new List<string>();
            lineLists.Add(curList);
            for (int line = curLine; line < endLine + 1; ++line)
            {
                var lineText = sci.GetLine(line);
                if (lineText.Trim().Length == 0)
                {
                    curList.Sort(CompareLines);
                    curList.Add(lineText);
                    curList = new List<string>();
                    lineLists.Add(curList);
                    continue;
                }
                curList.Add(lineText);
            }
            curList.Sort(CompareLines);
            StringBuilder result = new StringBuilder();
            foreach (var l in lineLists)
            {
                foreach (var s in l)
                {
                    result.Append(s);
                }
            }
            int selStart = sci.PositionFromLine(curLine);
            int selEnd = sci.PositionFromLine(endLine) + sci.MBSafeTextLength(sci.GetLine(endLine));
            sci.SetSel(selStart, selEnd);
            sci.ReplaceSel(result.ToString());
        }

        static int CompareLines(string a, string b)
        {
            char[] whitespace = {'\t', ' '};
            a = a.TrimStart(whitespace);
            b = b.TrimStart(whitespace);
            return a.CompareTo(b);
        }

        /// <summary>
        /// Line-comment the selected text
        /// </summary>
        public void CommentLine(object sender, EventArgs e)
        {
            ScintillaControl sci = CurrentDocument.SciControl;
            string lineComment = ScintillaManager.GetLineComment(sci.ConfigurationLanguage);
            if (lineComment.Length == 0) return;
            int position = sci.CurrentPos;
            int curLine = sci.LineFromPosition(position);
            int startPosInLine = position - sci.PositionFromLine(curLine);
            int finalPos = position;
            int startLine = sci.LineFromPosition(sci.SelectionStart);
            int line = startLine;
            int endLine = sci.LineFromPosition(sci.SelectionEnd);
            if (endLine > line && curLine == endLine && startPosInLine == 0)
            {
                curLine--;
                endLine--;
                finalPos = sci.PositionFromLine(curLine);
            }
            bool afterIndent = AppSettings.LineCommentsAfterIndent;
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
                if (startLine == endLine && (endLine < sci.LineCount) && AppSettings.MoveCursorAfterComment)
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
        public void UncommentLine(object sender, EventArgs e)
        {
            ScintillaControl sci = CurrentDocument.SciControl;
            string lineComment = ScintillaManager.GetLineComment(sci.ConfigurationLanguage);
            if (lineComment.Length == 0) return;
            int position = sci.CurrentPos;
            int curLine = sci.LineFromPosition(position);
            int startPosInLine = position - sci.PositionFromLine(curLine);
            int finalPos = position;
            int startLine = sci.LineFromPosition(sci.SelectionStart);
            int line = startLine;
            int endLine = sci.LineFromPosition(sci.SelectionEnd);
            if (endLine > line && curLine == endLine && startPosInLine == 0)
            {
                curLine--;
                endLine--;
                finalPos = sci.PositionFromLine(curLine);
            }

            sci.BeginUndoAction();
            try
            {
                while (line <= endLine)
                {
                    var text = sci.LineLength(line) == 0 ? "" : sci.GetLine(line).TrimStart();
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
                if (startLine == endLine && (endLine < sci.LineCount) && AppSettings.MoveCursorAfterComment)
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
        public void CommentSelection(object sender, EventArgs e) => CommentSelection();

        bool? CommentSelection()
        {
            ScintillaControl sci = CurrentDocument.SciControl;
            int selEnd = sci.SelectionEnd;
            int selStart = sci.SelectionStart;
            string commentEnd = ScintillaManager.GetCommentEnd(sci.ConfigurationLanguage);
            string commentStart = ScintillaManager.GetCommentStart(sci.ConfigurationLanguage);
            if (sci.SelText.StartsWithOrdinal(commentStart) && sci.SelText.EndsWithOrdinal(commentEnd))
            {
                sci.BeginUndoAction();
                try
                {
                    int indexLength = sci.SelText.Length - commentStart.Length - commentEnd.Length;
                    string withoutComment = sci.SelText.Substring(commentStart.Length, indexLength);
                    sci.ReplaceSel(withoutComment);
                }
                finally
                {
                    sci.EndUndoAction();
                }
                return false;
            }
            if (sci.SelText.Length > 0)
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
        public void UncommentBlock(object sender, EventArgs e)
        {
            ScintillaControl sci = CurrentDocument.SciControl;
            sci.Colourise(0, -1); // update coloring
            int selEnd = sci.SelectionEnd;
            int selStart = sci.SelectionStart;
            string commentEnd = ScintillaManager.GetCommentEnd(sci.ConfigurationLanguage);
            string commentStart = ScintillaManager.GetCommentStart(sci.ConfigurationLanguage);
            if ((sci.PositionIsOnComment(selStart) && (sci.PositionIsOnComment(selEnd)) || sci.PositionIsOnComment(selEnd - 1)) || (selEnd == selStart && sci.PositionIsOnComment(selStart - 1)))
            {
                sci.BeginUndoAction();
                try
                {
                    int lexer = sci.Lexer;
                    // find selection bounds
                    if (!sci.PositionIsOnComment(selStart, lexer)) selStart--;
                    int scrollTop = sci.FirstVisibleLine;
                    int initPos = sci.CurrentPos;
                    int start = selStart;
                    while (start > 0 && sci.PositionIsOnComment(start, lexer)) start--;
                    int end = selEnd;
                    int length = sci.TextLength;
                    while (end < length && sci.PositionIsOnComment(end, lexer)) end++;
                    sci.SetSel(start + 1, end);
                    // remove comment
                    string selText = sci.SelText;
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
        public void ToggleLineComment(object sender, EventArgs e)
        {
            ScintillaControl sci = CurrentDocument.SciControl;
            string lineComment = ScintillaManager.GetLineComment(sci.ConfigurationLanguage);
            int position = sci.CurrentPos;
            // try doing a block comment on the current line instead (xml, html...)
            if (lineComment == "")
            {
                ToggleBlockOnCurrentLine(sci);
                return;
            }
            int curLine = sci.LineFromPosition(position);
            int startPosInLine = position - sci.PositionFromLine(curLine);
            int startLine = sci.LineFromPosition(sci.SelectionStart);
            int line = startLine;
            int endLine = sci.LineFromPosition(sci.SelectionEnd);
            if (endLine > line && curLine == endLine && startPosInLine == 0) endLine--;
            bool containsCodeLine = false;
            while (line <= endLine)
            {
                var text = sci.LineLength(line) == 0 ? "" : sci.GetLine(line).TrimStart();
                if (!text.StartsWithOrdinal(lineComment))
                {
                    containsCodeLine = true;
                    break;
                }
                line++;
            }
            if (containsCodeLine) CommentLine(null, null);
            else UncommentLine(null, null);
        }

        void ToggleBlockOnCurrentLine(ScintillaControl sci)
        {
            int selStart = sci.SelectionStart;
            int indentPos = sci.LineIndentPosition(sci.CurrentLine);
            int lineEndPos = sci.LineEndPosition(sci.CurrentLine);
            bool afterBlockStart = sci.CurrentPos > indentPos;
            bool afterBlockEnd = sci.CurrentPos >= lineEndPos;
            sci.SelectionStart = indentPos;
            sci.SelectionEnd = lineEndPos;
            bool? added = CommentSelection();
            if (added is null) return;
            int factor = (bool)added ? 1 : -1;
            string commentEnd = ScintillaManager.GetCommentEnd(sci.ConfigurationLanguage);
            string commentStart = ScintillaManager.GetCommentStart(sci.ConfigurationLanguage);
            // preserve cursor pos
            if (afterBlockStart) selStart += commentStart.Length * factor;
            if (afterBlockEnd) selStart += commentEnd.Length * factor;
            sci.SetSel(selStart, selStart);
        }

        /// <summary>
        /// Toggles the block comment in a smart way
        /// </summary>
        public void ToggleBlockComment(object sender, EventArgs e)
        {
            if (CurrentDocument.SciControl.SelText.Length > 0) CommentSelection(null, null);
            else UncommentBlock(null, null);
        }

        /// <summary>
        /// Lets user browse for an theme file
        /// </summary>
        public void SelectTheme(object sender, EventArgs e)
        {
            using OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = PathHelper.ThemesDir;
            ofd.Title = " " + TextHelper.GetString("Title.OpenFileDialog");
            ofd.Filter = TextHelper.GetString("Info.ThemesFilter");
            if (ofd.ShowDialog(this) == DialogResult.OK)
            {
                string ext = Path.GetExtension(ofd.FileName).ToLower();
                if (ext == ".fdi")
                {
                    ThemeManager.LoadTheme(ofd.FileName);
                    ThemeManager.WalkControls(this);
                }
                else
                {
                    CallCommand("ExtractZip", ofd.FileName + ";true");
                    string currentTheme = Path.Combine(PathHelper.ThemesDir, "CURRENT");
                    if (File.Exists(currentTheme)) ThemeManager.LoadTheme(currentTheme);
                    ThemeManager.WalkControls(this);
                    RefreshSciConfig();
                }
            }
        }

        /// <summary>
        /// Loads an theme file and applies it
        /// </summary>
        public void LoadThemeFile(object sender, EventArgs e)
        {
            ToolStripItem button = (ToolStripItem)sender;
            string file = ProcessArgString(((ItemData)button.Tag).Tag);
            ThemeManager.LoadTheme(file);
        }

        /// <summary>
        /// Invokes the specified registered menu item
        /// </summary>
        public void InvokeMenuItem(object sender, EventArgs e)
        {
            try
            {
                ToolStripItem button = (ToolStripItem)sender;
                string registeredItem = ((ItemData)button.Tag).Tag;
                ShortcutItem item = ShortcutManager.GetRegisteredItem(registeredItem);
                item.Item?.PerformClick();
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Calls a ScintillaControl command
        /// </summary>
        public void ScintillaCommand(object sender, EventArgs e)
        {
            try
            {
                var button = (ToolStripItem)sender;
                var command = ((ItemData)button.Tag).Tag;
                var mfType = CurrentDocument.SciControl.GetType();
                var method = mfType.GetMethod(command, EmptyArray<Type>.Instance);
                method.Invoke(CurrentDocument.SciControl, null);
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
                var args = ((ItemData) item.Tag).Tag.Split(new[] { ';' }, 2);
                var action = args[0]; // Action of the command
                var data = args.Length > 1 ? args[1] : null;
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
        public bool CallCommand(string command, string args)
        {
            if (IsDisposed) return false;

            try
            {
                var method = GetType().GetMethod(command);
                if (method is null) throw new MethodAccessException();
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
        public void RunProcess(object sender, EventArgs e)
        {
            try
            {
                ToolStripItem button = (ToolStripItem)sender;
                string args = ProcessArgString(((ItemData)button.Tag).Tag);
                int position = args.IndexOf(';'); // Position of the arguments
                NotifyEvent ne = new NotifyEvent(EventType.ProcessStart);
                EventManager.DispatchEvent(this, ne);
                if (position > -1)
                {
                    string message = TextHelper.GetString("Info.RunningProcess");
                    TraceManager.Add(message + " " + args.Substring(0, position) + " " + args.Substring(position + 1), (int)TraceType.ProcessStart);
                    ProcessStartInfo psi = new ProcessStartInfo();
                    psi.WorkingDirectory = WorkingDirectory;
                    psi.Arguments = args.Substring(position + 1);
                    psi.FileName = args.Substring(0, position);
                    ProcessHelper.StartAsync(psi);
                }
                else
                {
                    string message = TextHelper.GetString("Info.RunningProcess");
                    TraceManager.Add(message + " " + args, (int)TraceType.ProcessStart);
                    if (args.ToLower().EndsWithOrdinal(".bat"))
                    {
                        Process bp = new Process();
                        bp.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        bp.StartInfo.FileName = @args;
                        bp.Start();
                    }
                    else
                    {
                        var psi = new ProcessStartInfo(args) {WorkingDirectory = WorkingDirectory};
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
        public void RunProcessCaptured(object sender, EventArgs e)
        {
            try
            {
                ToolStripItem button = (ToolStripItem)sender;
                string args = ProcessArgString(((ItemData)button.Tag).Tag);
                int position = args.IndexOf(';'); // Position of the arguments
                NotifyEvent ne = new NotifyEvent(EventType.ProcessStart);
                EventManager.DispatchEvent(this, ne);
                if (position < 0)
                {
                    string message = TextHelper.GetString("Info.NotEnoughArguments");
                    TraceManager.Add(message + " " + args, (int)TraceType.Error);
                    return;
                }
                string message2 = TextHelper.GetString("Info.RunningProcess");
                TraceManager.Add(message2 + " " + args.Substring(0, position) + " " + args.Substring(position + 1), (int)TraceType.ProcessStart);
                processRunner.Run(args.Substring(0, position), args.Substring(position + 1));
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
        void ProcessOutput(object sender, string line) => TraceManager.AddAsync(line, (int)TraceType.Info);

        /// <summary>
        /// Handles the incoming error output
        /// </summary> 
        void ProcessError(object sender, string line) => TraceManager.AddAsync(line, (int)TraceType.ProcessError);

        /// <summary>
        /// Handles the ending of a process
        /// </summary>
        void ProcessEnded(object sender, int exitCode)
        {
            if (InvokeRequired) BeginInvoke((MethodInvoker)(() => ProcessEnded(sender, exitCode)));
            else
            {
                var result = $"Done({exitCode})";
                TraceManager.Add(result, (int)TraceType.ProcessEnd);
                var te = new TextEvent(EventType.ProcessEnd, result);
                EventManager.DispatchEvent(this, te);
                ButtonManager.UpdateFlaggedButtons();
            }
        }

        /// <summary>
        /// Stop the currently running process
        /// </summary>
        public void KillProcess(object sender, EventArgs e) => KillProcess();

        /// <summary>
        /// Stop the currently running process
        /// </summary>
        public void KillProcess()
        {
            if (processRunner.IsRunning)
            {
                processRunner.KillProcess();
            }
        }

        /// <summary>
        /// Backups users setting files to a FDZ file
        /// </summary>
        public void BackupSettings(object sender, EventArgs e)
        {
            try
            {
                using var sfd = new SaveFileDialog();
                sfd.AddExtension = true;
                sfd.DefaultExt = "fdz";
                sfd.Filter = TextHelper.GetString("FlashDevelop.Info.ZipFilter");
                var dirMarker = "\\" + DistroConfig.DISTRIBUTION_NAME + "\\";
                if (sfd.ShowDialog(this) != DialogResult.OK) return;
                var settingFiles = new List<string>();
                settingFiles.AddRange(Directory.GetFiles(PathHelper.DataDir, "*.*", SearchOption.AllDirectories));
                settingFiles.AddRange(Directory.GetFiles(PathHelper.SnippetDir, "*.*", SearchOption.AllDirectories));
                settingFiles.AddRange(Directory.GetFiles(PathHelper.SettingDir, "*.*", SearchOption.AllDirectories));
                settingFiles.AddRange(Directory.GetFiles(PathHelper.TemplateDir, "*.*", SearchOption.AllDirectories));
                settingFiles.AddRange(Directory.GetFiles(PathHelper.UserLibraryDir, "*.*", SearchOption.AllDirectories));
                settingFiles.AddRange(Directory.GetFiles(PathHelper.UserProjectsDir, "*.*", SearchOption.AllDirectories));
                var zipFile = ZipFile.Create(sfd.FileName);
                zipFile.BeginUpdate();
                foreach (string settingFile in settingFiles)
                {
                    int index = settingFile.IndexOfOrdinal(dirMarker) + dirMarker.Length;
                    zipFile.Add(settingFile, "$(BaseDir)\\" + settingFile.Substring(index));
                }
                zipFile.CommitUpdate();
                zipFile.Close();
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Executes the specified C# Script file
        /// </summary>
        public void ExecuteScript(object sender, EventArgs e)
        {
            ToolStripItem button = (ToolStripItem)sender;
            string file = ProcessArgString(((ItemData)button.Tag).Tag);
            try
            {
                Host host = new Host();
                string[] args = file.Split(';');
                if (args.Length == 1 || string.IsNullOrEmpty(args[1])) return; // no file selected / the open file dialog was cancelled
                if (args[0] == "Internal") host.ExecuteScriptInternal(args[1], false);
                else if (args[0] == "Development") host.ExecuteScriptInternal(args[1], true);
                else host.ExecuteScriptExternal(file);
            }
            catch (Exception ex)
            {
                string message = TextHelper.GetString("Info.CouldNotExecuteScript");
                ErrorManager.ShowWarning(message + "\r\n" + ex.Message, null);
            }
        }

        /// <summary>
        /// Test the controls in a dedicated form
        /// </summary>
        public void TestControls(object sender, EventArgs e)
        {
            using ControlDialog cd = new ControlDialog();
            cd.Show(this);
        }

        /// <summary>
        /// Outputs the supplied argument string
        /// </summary>
        public void Debug(object sender, EventArgs e)
        {
            var button = (ToolStripItem)sender;
            var args = ProcessArgString(((ItemData)button.Tag).Tag);
            if (args.Length == 0) ErrorManager.ShowError(new Exception("Debug"));
            else ErrorManager.ShowInfo(args);
        }

        /// <summary>
        /// Restarts FlashDevelop
        /// </summary>
        public void Restart(object sender, EventArgs e)
        {
            if (GetInstanceCount() == 1)
            {
                RestartRequested = true;
                Close();
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
        public void ExecuteScriptExternal(string script)
        {
            if (!File.Exists(script)) throw new FileNotFoundException();
            using AsmHelper helper = new AsmHelper(CSScript.CompileFile(script, null, true), null, true);
            helper.Invoke("*.Execute");
        }

        /// <summary>
        /// Executes the script and adds it to the current app domain
        /// NOTE: This locks the assembly script file
        /// </summary>
        public void ExecuteScriptInternal(string script, bool random)
        {
            if (!File.Exists(script)) throw new FileNotFoundException();
            string file = random ? Path.GetTempFileName() : null;
            AsmHelper helper = new AsmHelper(CSScript.Load(script, file, false, null));
            helper.Invoke("*.Execute");
        }

    }

    #endregion

}
