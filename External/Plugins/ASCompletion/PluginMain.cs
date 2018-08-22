using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Timers;
using System.Windows.Forms;
using ASCompletion.Commands;
using ASCompletion.Completion;
using ASCompletion.Context;
using ASCompletion.Controls;
using ASCompletion.Helpers;
using ASCompletion.Model;
using ASCompletion.Settings;
using PluginCore;
using PluginCore.Controls;
using PluginCore.Helpers;
using PluginCore.Localization;
using PluginCore.Managers;
using PluginCore.Utilities;
using ScintillaNet;
using WeifenLuo.WinFormsUI.Docking;
using Timer = System.Timers.Timer;

namespace ASCompletion
{
    public class PluginMain: IPlugin
    {
        private string pluginName = "ASCompletion";
        private string pluginGuid = "078c7c1a-c667-4f54-9e47-d45c0e835c4e";
        private string pluginAuth = "FlashDevelop Team";
        private string pluginHelp = "www.flashdevelop.org/community/";
        private string pluginDesc = "Code completion engine for FlashDevelop.";

        private string dataPath;
        private string settingsFile;
        private GeneralSettings settingObject;
        private DockContent pluginPanel;
        private PluginUI pluginUI;
        private Image pluginIcon;
        private EventType eventMask =
            EventType.FileOpen |
            EventType.FileSave |
            EventType.FileSwitch |
            EventType.SyntaxChange |
            EventType.Completion |
            EventType.SyntaxDetect |
            EventType.UIRefresh |
            EventType.Keys |
            EventType.Shortcut |
            EventType.Command |
            EventType.ProcessEnd |
            EventType.ApplySettings |
            EventType.ProcessArgs;
        private List<ToolStripItem> menuItems;
        private ToolStripItem quickBuildItem;
        private int currentPos;
        private string currentDoc;
        private bool started;
        private FlashErrorsWatcher flashErrorsWatcher;
        private bool checking = false;
        private Timer timerPosition;
        private int lastHoverPosition;

        private Regex reVirtualFile = new Regex("\\.(swf|swc)::", RegexOptions.Compiled);
        private Regex reArgs = new Regex("\\$\\((Typ|Mbr|Itm)", RegexOptions.Compiled);
        private Regex reCostlyArgs = new Regex("\\$\\((TypClosest|ItmUnique)", RegexOptions.Compiled);

        const int Margin = 1;
        const int MarkerDown = 16;
        const int MarkerUp = 17;
        const int MarkerUpDown = 18;

        Bitmap downArrow;
        Bitmap upArrow;
        Bitmap upDownArrow;

        readonly ASTCache astCache = new ASTCache();
        bool initializedCache = true;
        IProject lastProject;
        Timer astCacheTimer;

        #region Required Properties

        public Int32 Api
        {
            get { return 1; }
        }

        public string Name
        {
            get { return pluginName; }
        }

        public string Guid
        {
            get { return pluginGuid; }
        }

        public string Author
        {
            get { return pluginAuth; }
        }

        public string Description
        {
            get { return pluginDesc; }
        }

        public string Help
        {
            get { return pluginHelp; }
        }

        [Browsable(false)]
        public virtual Object Settings
        {
            get { return settingObject; }
        }
        #endregion

        #region Plugin Properties

        [Browsable(false)]
        public virtual PluginUI Panel
        {
            get { return pluginUI; }
        }

        [Browsable(false)]
        public virtual string DataPath
        {
            get { return dataPath; }
        }

        #endregion

        #region Required Methods

        /// <summary>
        /// Initializes the plugin
        /// </summary>
        public virtual void Initialize()
        {
            try
            {
                InitSettings();
                LoadBitmaps();
                CreatePanel();
                CreateMenuItems();
                AddEventHandlers();
                ASContext.GlobalInit(this);
                ModelsExplorer.CreatePanel();
            }
            catch(Exception ex)
            {
                ErrorManager.ShowError(/*"Failed to initialize the completion engine.\n"+ex.Message,*/ ex);
            }
        }

        /// <summary>
        /// Disposes the plugin
        /// </summary>
        public void Dispose()
        {
            timerPosition.Enabled = false;
            astCacheTimer.Enabled = false;
            PathExplorer.StopBackgroundExploration();
            SaveSettings();
        }

        /// <summary>
        /// Handles the incoming events
        /// </summary>
        public void HandleEvent(object sender, NotifyEvent e, HandlingPriority priority)
        {
            try
            {
                // ignore all events when leaving
                if (PluginBase.MainForm.ClosingEntirely) return;

                // application start
                if (!started && e.Type == EventType.UIStarted)
                {
                    started = true;
                    PathExplorer.OnUIStarted();
                    // associate context to initial document
                    e = new NotifyEvent(EventType.SyntaxChange);
                    this.pluginUI.UpdateAfterTheme();
                }

                // current active document
                ITabbedDocument doc = PluginBase.MainForm.CurrentDocument;
                // editor ready?
                if (doc == null) return;
                ScintillaControl sci = doc.IsEditable ? doc.SciControl : null;

                //
                //  Events always handled
                //
                DataEvent de;
                switch (e.Type)
                {
                    // caret position in editor
                    case EventType.UIRefresh:
                        if (!doc.IsEditable) return;
                        timerPosition.Enabled = false;
                        timerPosition.Enabled = true;
                        return;

                    // key combinations
                    case EventType.Keys:
                        Keys keys = ((KeyEvent) e).Value;
                        if (ModelsExplorer.HasFocus)
                        {
                            e.Handled = ModelsExplorer.Instance.OnShortcut(keys);
                            return;
                        }
                        if (pluginUI.OnShortcut(keys))
                        {
                            e.Handled = true;
                            return;
                        }
                        if (!doc.IsEditable) return;
                        e.Handled = ASComplete.OnShortcut(keys, sci);
                        return;

                    // user-customized shortcuts
                    case EventType.Shortcut:
                        de = e as DataEvent;
                        if (de.Action == "Completion.ShowHelp")
                        {
                            ASComplete.HelpKeys = (Keys)de.Data;
                            de.Handled = true;
                        }
                        return;

                    //
                    // File management
                    //
                    case EventType.FileOpen:
                        ApplyMarkers(PluginBase.MainForm.CurrentDocument.SplitSci1);
                        ApplyMarkers(PluginBase.MainForm.CurrentDocument.SplitSci2);
                        break;

                    case EventType.FileSave:
                        if (!doc.IsEditable) return;
                        ASContext.Context.CheckModel(false);
                        // toolbar
                        var isValid = ASContext.Context.IsFileValid;
                        if (isValid && !PluginBase.MainForm.SavingMultiple)
                        {
                            if (ASContext.Context.Settings.CheckSyntaxOnSave) CheckSyntax(null, null);
                            ASContext.Context.RemoveClassCompilerCache();
                        }
                        return;

                    case EventType.SyntaxDetect:
                        // detect Actionscript language version
                        if (!doc.IsEditable) return;
                        if (doc.FileName.ToLower().EndsWithOrdinal(".as"))
                        {
                            settingObject.LastASVersion = DetectActionscriptVersion(doc);
                            ((TextEvent) e).Value = settingObject.LastASVersion;
                            e.Handled = true;
                        }
                        break;

                    case EventType.ApplySettings:
                        if (settingObject.ASTCacheUpdateInterval <= 0)
                            settingObject.ASTCacheUpdateInterval = 120; //2 minutes

                        if (settingObject.DisableInheritanceNavigation)
                        {
                            astCacheTimer.Stop();
                            astCache.Clear();
                            foreach (var document in PluginBase.MainForm.Documents)
                            {
                                //remove the markers
                                UpdateMarkersFromCache(document.SplitSci1);
                                UpdateMarkersFromCache(document.SplitSci2);
                            }
                        }
                        goto case EventType.SyntaxChange;
                    case EventType.SyntaxChange:
                    case EventType.FileSwitch:
                        if (!doc.IsEditable)
                        {
                            ASContext.SetCurrentFile(null, true);
                            ContextChanged();
                            return;
                        }
                        currentDoc = doc.FileName;
                        currentPos = sci.CurrentPos;
                        // check file
                        bool ignoreFile = !doc.IsEditable;
                        ASContext.SetCurrentFile(doc, ignoreFile);
                        // UI
                        ContextChanged();
                        return;

                    case EventType.Completion:
                        if (ASContext.Context.IsFileValid) e.Handled = true;
                        return;

                    // some commands work all the time
                    case EventType.Command:
                        de = e as DataEvent;
                        string command = de.Action ?? "";

                        if (command.StartsWithOrdinal("ASCompletion."))
                        {
                            string cmdData = de.Data as string;

                            // add a custom classpath
                            if (command == "ASCompletion.ClassPath")
                            {
                                Hashtable info = de.Data as Hashtable;
                                if (info != null)
                                {
                                    ContextSetupInfos setup = new ContextSetupInfos();
                                    setup.Platform = (string)info["platform"];
                                    setup.Lang = (string)info["lang"];
                                    setup.Version = (string)info["version"];
                                    setup.TargetBuild = (string)info["targetBuild"];
                                    setup.Classpath = (string[])info["classpath"];
                                    setup.HiddenPaths = (string[])info["hidden"];
                                    ASContext.SetLanguageClassPath(setup);
                                    if (setup.AdditionalPaths != null) // report custom classpath
                                        info["additional"] = setup.AdditionalPaths.ToArray();
                                }
                                e.Handled = true;
                            }
                            // send a UserClasspath
                            else if (command == "ASCompletion.GetUserClasspath")
                            {
                                Hashtable info = de.Data as Hashtable;
                                if (info != null && info.ContainsKey("language"))
                                {
                                    IASContext context = ASContext.GetLanguageContext(info["language"] as string);
                                    if (context?.Settings?.UserClasspath != null)
                                        info["cp"] = new List<string>(context.Settings.UserClasspath);
                                }
                                e.Handled = true;
                            }
                            // update a UserClasspath
                            else if (command == "ASCompletion.SetUserClasspath")
                            {
                                Hashtable info = de.Data as Hashtable;
                                if (info != null && info.ContainsKey("language") && info.ContainsKey("cp"))
                                {
                                    IASContext context = ASContext.GetLanguageContext(info["language"] as string);
                                    List<string> cp = info["cp"] as List<string>;
                                    if (cp != null && context?.Settings != null)
                                    {
                                        string[] pathes = new string[cp.Count];
                                        cp.CopyTo(pathes);
                                        context.Settings.UserClasspath = pathes;
                                    }
                                }
                                e.Handled = true;
                            }
                            // send the language's default compiler path
                            else if (command == "ASCompletion.GetCompilerPath")
                            {
                                Hashtable info = de.Data as Hashtable;
                                if (info != null && info.ContainsKey("language"))
                                {
                                    IASContext context = ASContext.GetLanguageContext(info["language"] as string);
                                    if (context != null)
                                        info["compiler"] = context.GetCompilerPath();
                                }
                                e.Handled = true;
                            }
                            // show a language's compiler settings
                            else if (command == "ASCompletion.ShowSettings")
                            {
                                e.Handled = true;
                                IASContext context = ASContext.GetLanguageContext(cmdData);
                                if (context == null) return;
                                const string filter = "SDK";
                                string name = "";
                                switch (cmdData.ToUpper())
                                {
                                    case "AS2": name = "AS2Context"; break;
                                    case "AS3": name = "AS3Context"; break;
                                    default: 
                                        name = cmdData.Substring(0, 1).ToUpper() + cmdData.Substring(1) + "Context";
                                        break;
                                }
                                PluginBase.MainForm.ShowSettingsDialog(name, filter);
                            }
                            // Open types explorer dialog
                            else if (command == "ASCompletion.TypesExplorer")
                            {
                                TypesExplorer(null, null);
                            }
                            // call the Flash IDE
                            else if (command == "ASCompletion.CallFlashIDE")
                            {
                                if (flashErrorsWatcher == null) flashErrorsWatcher = new FlashErrorsWatcher();
                                e.Handled = CallFlashIDE.Run(settingObject.PathToFlashIDE, cmdData);
                            }
                            // create Flash 8+ trust file
                            else if (command == "ASCompletion.CreateTrustFile")
                            {
                                if (cmdData != null)
                                {
                                    string[] args = cmdData.Split(';');
                                    if (args.Length == 2)
                                        e.Handled = CreateTrustFile.Run(args[0], args[1]);
                                }
                            }
                            else if (command == "ASCompletion.GetClassPath")
                            {
                                if (cmdData != null)
                                {
                                    string[] args = cmdData.Split(';');
                                    if (args.Length == 1)
                                    {
                                        FileModel model = ASContext.Context.GetFileModel(args[0]);
                                        ClassModel aClass = model.GetPublicClass();
                                        if (!aClass.IsVoid())
                                        {
                                            Clipboard.SetText(aClass.QualifiedName);
                                            e.Handled = true;
                                        }
                                    }
                                }
                            }
                            else if (command == "ProjectManager.FileActions.DisableWatchers")
                            {
                                foreach (PathModel cp in ASContext.Context.Classpath)
                                    cp.DisableWatcher();
                            }
                            else if (command == "ProjectManager.FileActions.EnableWatchers")
                            {
                                // classpaths could be invalid now - remove those, BuildClassPath() is too expensive
                                ASContext.Context.Classpath.RemoveAll(cp => !Directory.Exists(cp.Path));

                                foreach (PathModel cp in ASContext.Context.Classpath)
                                    cp.EnableWatcher();
                            }
                            // Return requested language SDK list
                            else if (command == "ASCompletion.InstalledSDKs")
                            {
                                Hashtable info = de.Data as Hashtable;
                                if (info != null && info.ContainsKey("language"))
                                {
                                    IASContext context = ASContext.GetLanguageContext(info["language"] as string);
                                    if (context != null)
                                        info["sdks"] = context.Settings.InstalledSDKs;
                                }
                                e.Handled = true;
                            }
                            //PathExplorer finished looking for files, update cache
                            else if (command == "ASCompletion.PathExplorerFinished" && !initializedCache)
                            {
                                UpdateCompleteCache();
                                initializedCache = true;
                            }
                        }
                        // Create a fake document from a FileModel
                        else if (command == "ProjectManager.OpenVirtualFile")
                        {
                            string cmdData = de.Data as string;
                            if (reVirtualFile.IsMatch(cmdData))
                            {
                                string[] path = Regex.Split(cmdData, "::");
                                string fileName = path[0] + Path.DirectorySeparatorChar
                                    + path[1].Replace('.', Path.DirectorySeparatorChar).Replace("::", Path.DirectorySeparatorChar.ToString());
                                FileModel found = ModelsExplorer.Instance.OpenFile(fileName);
                                if (found != null) e.Handled = true;
                            }
                        }
                        else if (command == "ProjectManager.UserRefreshTree")
                        {
                            ASContext.UserRefreshRequestAll();
                        }
                        else if (command == "ProjectManager.Project" && !settingObject.DisableInheritanceNavigation)
                        {
                            if (lastProject != PluginBase.CurrentProject)
                            {
                                lastProject = PluginBase.CurrentProject;
                                initializedCache = false;
                            }
                        }
                        else if (command == "ProjectManager.Menu")
                        {
                            var image = PluginBase.MainForm.FindImage("202");
                            var item = new ToolStripMenuItem(TextHelper.GetString("Label.TypesExplorer"), image, TypesExplorer, Keys.Control | Keys.J);
                            PluginBase.MainForm.RegisterShortcutItem("FlashToolsMenu.TypeExplorer", item);
                            ((ToolStripMenuItem)de.Data).DropDownItems.Insert(6, item);
                        }
                        break;
                }

                //
                // Actionscript context specific
                //
                if (ASContext.Context.IsFileValid)
                {
                    switch (e.Type)
                    {
                        case EventType.ProcessArgs:
                            TextEvent te = (TextEvent) e;
                            if (reArgs.IsMatch(te.Value))
                            {
                                // resolve current element
                                Hashtable details = ASComplete.ResolveElement(sci, null);
                                te.Value = ArgumentsProcessor.Process(te.Value, details);

                                if (te.Value.Contains('$') && reCostlyArgs.IsMatch(te.Value))
                                {
                                    ASResult result = ASComplete.CurrentResolvedContext.Result ?? new ASResult();
                                    details = new Hashtable();
                                    // Get closest list (Array or Vector)
                                    string closestListName = "", closestListItemType = "";
                                    ASComplete.FindClosestList(ASContext.Context, result.Context, sci.CurrentLine, ref closestListName, ref closestListItemType);
                                    details.Add("TypClosestListName", closestListName);
                                    details.Add("TypClosestListItemType", closestListItemType);
                                    // get free iterator index
                                    string iterator = ASComplete.FindFreeIterator(ASContext.Context, ASContext.Context.CurrentClass, result.Context);
                                    details.Add("ItmUniqueVar", iterator);
                                    te.Value = ArgumentsProcessor.Process(te.Value, details);
                                }
                            }
                            break;

                        // menu commands
                        case EventType.Command:
                            de = e as DataEvent;
                            string command = de.Action ?? "";
                            if (command.StartsWith("ASCompletion.", StringComparison.Ordinal))
                            {
                                string cmdData = de.Data as string;
                                switch (command)
                                {
                                    // run MTASC
                                    case "ASCompletion.CustomBuild":
                                        if (cmdData != null) ASContext.Context.RunCMD(cmdData);
                                        else ASContext.Context.RunCMD("");
                                        e.Handled = true;
                                        break;

                                    // build the SWF using MTASC
                                    case "ASCompletion.QuickBuild":
                                        ASContext.Context.BuildCMD(false);
                                        e.Handled = true;
                                        break;

                                    // resolve element under cursor and open declaration
                                    case "ASCompletion.GotoDeclaration":
                                        ASComplete.DeclarationLookup(sci);
                                        e.Handled = true;
                                        break;

                                    // resolve element under cursor and send a CustomData event
                                    case "ASCompletion.ResolveElement":
                                        ASComplete.ResolveElement(sci, cmdData);
                                        e.Handled = true;
                                        break;

                                    case "ASCompletion.MakeIntrinsic":
                                        ASContext.Context.MakeIntrinsic(cmdData);
                                        e.Handled = true;
                                        break;

                                    // alternative to default shortcuts
                                    case "ASCompletion.CtrlSpace":
                                        ASComplete.OnShortcut(Keys.Control | Keys.Space, ASContext.CurSciControl);
                                        e.Handled = true;
                                        break;

                                    case "ASCompletion.CtrlShiftSpace":
                                        ASComplete.OnShortcut(Keys.Control | Keys.Shift | Keys.Space, ASContext.CurSciControl);
                                        e.Handled = true;
                                        break;

                                    case "ASCompletion.CtrlAltSpace":
                                        ASComplete.OnShortcut(Keys.Control | Keys.Alt | Keys.Space, ASContext.CurSciControl);
                                        e.Handled = true;
                                        break;

                                    case "ASCompletion.ContextualGenerator":
                                        if (ASContext.HasContext)
                                        {
                                            var options = new List<ICompletionListItem>();
                                            ASGenerator.ContextualGenerator(ASContext.CurSciControl, options);
                                            EventManager.DispatchEvent(this, new DataEvent(EventType.Command, "ASCompletion.ContextualGenerator.AddOptions", options));
                                            if (options.Count == 0)
                                            {
                                                PluginBase.MainForm.StatusLabel.Text = TextHelper.GetString("Info.NoContextGeneratorCode");
                                            }
                                            CompletionList.Show(options, false);
                                        }
                                        break;
                                }
                            }
                            return;

                        case EventType.ProcessEnd:
                            string procResult = (e as TextEvent).Value;
                            ASContext.Context.OnProcessEnd(procResult);
                            break;
                    }
                }
            }
            catch(Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        #endregion

        #region Custom Properties

        /// <summary>
        /// Gets the PluginPanel
        /// </summary>
        [Browsable(false)]
        public DockContent PluginPanel => pluginPanel;

        /// <summary>
        /// Gets the PluginSettings
        /// </summary>
        [Browsable(false)]
        public virtual GeneralSettings PluginSettings => settingObject;

        [Browsable(false)]
        public virtual List<ToolStripItem> MenuItems => menuItems;

        #endregion

        #region Initialization

        private void InitSettings()
        {
            pluginDesc = TextHelper.GetString("Info.Description");
            dataPath = Path.Combine(PathHelper.DataDir, "ASCompletion");
            if (!Directory.Exists(dataPath)) Directory.CreateDirectory(dataPath);
            else if (PluginBase.MainForm.RefreshConfig) CleanData();
            settingsFile = Path.Combine(dataPath, "Settings.fdb");
            settingObject = new GeneralSettings();
            if (!File.Exists(settingsFile))
            {
                // default settings
                settingObject.JavadocTags = GeneralSettings.DEFAULT_TAGS;
                settingObject.PathToFlashIDE = CallFlashIDE.FindFlashIDE();
                SaveSettings();
            }
            else
            {
                Object obj = ObjectSerializer.Deserialize(settingsFile, settingObject);
                settingObject = (GeneralSettings)obj;
            }
        }

        /// <summary>
        /// FD has been updated, clean some app data
        /// </summary>
        private void CleanData() => PathExplorer.ClearPersistentCache();

        private void SaveSettings()
        {
            ObjectSerializer.Serialize(settingsFile, this.settingObject);
        }

        void LoadBitmaps()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            var stream = assembly.GetManifestResourceStream("ASCompletion.Icons.UpDownArrow.png");

            upDownArrow = new Bitmap(PluginBase.MainForm.ImageSetAdjust(Image.FromStream(stream)));
            downArrow = new Bitmap(PluginBase.MainForm.FindImage16("22"));
            upArrow = new Bitmap(PluginBase.MainForm.FindImage16("8"));
        }

        private void CreatePanel()
        {
            pluginIcon = PluginBase.MainForm.FindImage("99");
            pluginUI = new PluginUI(this);
            pluginUI.Text = TextHelper.GetString("Title.PluginPanel");
            pluginPanel = PluginBase.MainForm.CreateDockablePanel(pluginUI, pluginGuid, pluginIcon, DockState.DockRight);
        }

        private void CreateMenuItems()
        {
            IMainForm mainForm = PluginBase.MainForm;
            menuItems = new List<ToolStripItem>();
            ToolStripMenuItem item;
            ToolStripMenuItem menu = (ToolStripMenuItem)mainForm.FindMenuItem("ViewMenu");
            if (menu != null)
            {
                item = new ToolStripMenuItem(TextHelper.GetString("Label.ViewMenuItem"), pluginIcon, new EventHandler(OpenPanel));
                PluginBase.MainForm.RegisterShortcutItem("ViewMenu.ShowOutline", item);
                menu.DropDownItems.Add(item);
            }

            Image image;
            // tools items
            menu = (ToolStripMenuItem)mainForm.FindMenuItem("FlashToolsMenu");
            if (menu != null)
            {
                menu.DropDownItems.Add(new ToolStripSeparator());

                // check actionscript
                image = pluginUI.GetIcon(PluginUI.ICON_CHECK_SYNTAX);
                item = new ToolStripMenuItem(TextHelper.GetString("Label.CheckSyntax"), image, new EventHandler(CheckSyntax), Keys.F7);
                PluginBase.MainForm.RegisterShortcutItem("FlashToolsMenu.CheckSyntax", item);
                menu.DropDownItems.Add(item);
                menuItems.Add(item);

                // quick build
                image = pluginUI.GetIcon(PluginUI.ICON_QUICK_BUILD);
                item = new ToolStripMenuItem(TextHelper.GetString("Label.QuickBuild"), image, new EventHandler(QuickBuild), Keys.Control | Keys.F8);
                PluginBase.MainForm.RegisterShortcutItem("FlashToolsMenu.QuickBuild", item);
                menu.DropDownItems.Add(item);
                //menuItems.Add(item);
                quickBuildItem = item;

                menu.DropDownItems.Add(new ToolStripSeparator());

                // model cleanup
                image = mainForm.FindImage("153");
                item = new ToolStripMenuItem(TextHelper.GetString("Label.RebuildClasspathCache"), image, new EventHandler(RebuildClasspath));
                PluginBase.MainForm.RegisterShortcutItem("FlashToolsMenu.RebuildClasspathCache", item);
                menu.DropDownItems.Add(item);

                // convert to intrinsic
                item = new ToolStripMenuItem(TextHelper.GetString("Label.ConvertToIntrinsic"), null, new EventHandler(MakeIntrinsic));
                PluginBase.MainForm.RegisterShortcutItem("FlashToolsMenu.ConvertToIntrinsic", item);
                menu.DropDownItems.Add(item);
                menuItems.Add(item);
            }

            // toolbar items
            ToolStrip toolStrip = mainForm.ToolStrip;
            if (toolStrip != null)
            {
                toolStrip.Items.Add(new ToolStripSeparator());
                // check
                image = pluginUI.GetIcon(PluginUI.ICON_CHECK_SYNTAX);
                var button = new ToolStripButton(image);
                button.Name = "CheckSyntax";
                button.ToolTipText = TextHelper.GetStringWithoutMnemonics("Label.CheckSyntax");
                button.Click += CheckSyntax;
                PluginBase.MainForm.RegisterSecondaryItem("FlashToolsMenu.CheckSyntax", button);
                toolStrip.Items.Add(button);
                menuItems.Add(button);
            }

            // search items
            menu = (ToolStripMenuItem)mainForm.FindMenuItem("SearchMenu");
            if (menu != null)
            {
                menu.DropDownItems.Add(new ToolStripSeparator());

                // peek definition
                image = mainForm.FindImage("99|9|3|-3");
                item = new ToolStripMenuItem(TextHelper.GetString("Label.PeekDefinition"), image, PeekDefinition);
                PluginBase.MainForm.RegisterShortcutItem("SearchMenu.PeekDefinition", item);
                menu.DropDownItems.Add(item);
                menuItems.Add(item);

                // goto declaration
                image = mainForm.FindImage("99|9|3|-3");
                item = new ToolStripMenuItem(TextHelper.GetString("Label.GotoDeclaration"), image, GotoDeclaration, Keys.F4);
                PluginBase.MainForm.RegisterShortcutItem("SearchMenu.GotoDeclaration", item);
                menu.DropDownItems.Add(item);
                menuItems.Add(item);

                // goto type declaration
                image = mainForm.FindImage("99|9|3|-3");
                item = new ToolStripMenuItem(TextHelper.GetString("Label.GotoTypeDeclaration"), image, GotoTypeDeclaration);
                PluginBase.MainForm.RegisterShortcutItem("SearchMenu.GotoTypeDeclaration", item);
                menu.DropDownItems.Add(item);
                menuItems.Add(item);

                // goto back from declaration
                image = mainForm.FindImage("99|1|-3|-3");
                item = new ToolStripMenuItem(TextHelper.GetString("Label.BackFromDeclaration"), image, BackDeclaration, Keys.Shift | Keys.F4);
                PluginBase.MainForm.RegisterShortcutItem("SearchMenu.BackFromDeclaration", item);
                menu.DropDownItems.Add(item);
                pluginUI.LookupMenuItem = item;
                item.Enabled = false;

                // editor items
                ContextMenuStrip emenu = mainForm.EditorMenu;
                if (emenu != null)
                {
                    // peek definition
                    image = mainForm.FindImage("99|9|3|-3");
                    item = new ToolStripMenuItem(TextHelper.GetString("Label.PeekDefinition"), image, PeekDefinition);
                    PluginBase.MainForm.RegisterSecondaryItem("SearchMenu.PeekDefinition", item);
                    item.Enabled = false;
                    emenu.Items.Insert(4, item);
                    menuItems.Add(item);

                    // goto declaration
                    image = mainForm.FindImage("99|9|3|-3");
                    item = new ToolStripMenuItem(TextHelper.GetString("Label.GotoDeclaration"), image, GotoDeclaration);
                    PluginBase.MainForm.RegisterSecondaryItem("SearchMenu.GotoDeclaration", item);
                    emenu.Items.Insert(5, item);
                    menuItems.Add(item);

                    // goto type declaration
                    image = mainForm.FindImage("99|9|3|-3");
                    item = new ToolStripMenuItem(TextHelper.GetString("Label.GotoTypeDeclaration"), image, GotoTypeDeclaration);
                    PluginBase.MainForm.RegisterSecondaryItem("SearchMenu.GotoTypeDeclaration", item);
                    emenu.Items.Insert(6, item);
                    emenu.Items.Insert(7, new ToolStripSeparator());
                    menuItems.Add(item);
                }
            }
        }

        private void AddEventHandlers()
        {
            // scintilla controls listeners
            UITools.Manager.OnCharAdded += new UITools.CharAddedHandler(OnChar);
            UITools.Manager.OnMouseHover += new UITools.MouseHoverHandler(OnMouseHover);
            UITools.Manager.OnTextChanged += new UITools.TextChangedHandler(OnTextChanged);
            UITools.CallTip.OnUpdateCallTip += new MethodCallTip.UpdateCallTipHandler(OnUpdateCallTip);
            UITools.Tip.OnUpdateSimpleTip += new RichToolTip.UpdateTipHandler(OnUpdateSimpleTip);
            CompletionList.OnInsert += new InsertedTextHandler(ASComplete.HandleCompletionInsert);
            FileModel.OnFileUpdate += OnFileUpdate;
            PathModel.OnFileRemove += OnFileRemove;
            PathModel.OnFileAdded += OnFileUpdate;

            // shortcuts
            PluginBase.MainForm.IgnoredKeys.Add(Keys.Back);
            PluginBase.MainForm.IgnoredKeys.Add(Keys.Control | Keys.Enter);
            PluginBase.MainForm.IgnoredKeys.Add(Keys.Space | Keys.Control | Keys.Alt); // complete project types
            PluginBase.MainForm.RegisterShortcutItem("Completion.ShowHelp", Keys.F1);

            // application events
            EventManager.AddEventHandler(this, eventMask);
            EventManager.AddEventHandler(this, EventType.UIStarted, HandlingPriority.Low);
            
            // cursor position changes tracking
            timerPosition = new Timer();
            timerPosition.SynchronizingObject = PluginBase.MainForm as Form;
            timerPosition.Interval = 200;
            timerPosition.Elapsed += timerPosition_Elapsed;

            //Cache update
            astCache.FinishedUpdate += UpdateOpenDocumentMarkers;
            astCacheTimer = new Timer
            {
                SynchronizingObject = PluginBase.MainForm as Form,
                AutoReset = false,
                Enabled = false
            };
            astCacheTimer.Elapsed += AstCacheTimer_Elapsed;
        }

        #endregion

        #region Plugin actions

        void UpdateCompleteCache()
        {
            if (PluginBase.CurrentProject == null) return;

            astCache.UpdateCompleteCache();
        }

        void UpdateOpenDocumentMarkers()
        {
            foreach (var document in PluginBase.MainForm.Documents)
            {
                if (!document.IsEditable) continue;

                UpdateMarkersFromCache(document.SplitSci1);
                UpdateMarkersFromCache(document.SplitSci2);
            }
        }

        void ApplyMarkers(ScintillaControl sci)
        {
            if (settingObject.DisableInheritanceNavigation || sci == null) return;
            
            //Register marker
            sci.MarkerDefineRGBAImage(MarkerDown, downArrow);
            sci.MarkerDefineRGBAImage(MarkerUp, upArrow);
            sci.MarkerDefineRGBAImage(MarkerUpDown, upDownArrow);
            //Setup margin
            var mask = (1 << MarkerDown) | (1 << MarkerUp) | (1 << MarkerUpDown);
            sci.SetMarginMaskN(Margin, mask);
            sci.MarginSensitiveN(Margin, true);

            sci.MarginClick -= Sci_MarginClick;
            sci.MarginClick += Sci_MarginClick;

            UpdateMarkersFromCache(sci);
        }

        void UpdateMarkersFromCache(ScintillaControl sci)
        {
            var marginWidth = 16;
            sci.SetMarginWidthN(Margin, 0); //margin is only made visible if something is found

            sci.MarkerDeleteAll(MarkerUp);
            sci.MarkerDeleteAll(MarkerDown);
            sci.MarkerDeleteAll(MarkerUpDown);

            if (settingObject.DisableInheritanceNavigation) return;

            if (PluginBase.CurrentProject == null) return;
            var context = ASContext.GetLanguageContext(PluginBase.CurrentProject.Language) as ASContext;
            if (context == null) return;

            var fileModel = context.GetCachedFileModel(sci.FileName);

            foreach (var clas in fileModel.Classes)
            {
                var cls = astCache.GetCachedModel(clas);
                if (cls == null) return;

                if (cls.ChildClassModels.Count > 0 || cls.ImplementorClassModels.Count > 0)
                {
                    sci.SetMarginWidthN(Margin, marginWidth);
                    sci.MarkerAdd(clas.LineFrom, MarkerDown);
                }

                foreach (var implementing in cls.Implementing)
                {
                    sci.SetMarginWidthN(Margin, marginWidth);
                    sci.MarkerAdd(implementing.Key.LineFrom, MarkerUp);
                }
                foreach (var implementor in cls.Implementors)
                {
                    sci.SetMarginWidthN(Margin, marginWidth);
                    sci.MarkerAdd(implementor.Key.LineFrom, MarkerDown);
                }
                foreach (var overriders in cls.Overriders)
                {
                    sci.SetMarginWidthN(Margin, marginWidth);
                    sci.MarkerAdd(overriders.Key.LineFrom, MarkerDown);
                }
                foreach (var overrides in cls.Overriding)
                {
                    sci.SetMarginWidthN(Margin, marginWidth);
                    sci.MarkerAdd(overrides.Key.LineFrom, MarkerUp);
                }

                for (var i = 0; i < sci.LineCount; ++i)
                {
                    var mask = sci.MarkerGet(i);
                    var searchMask = (1 << MarkerDown) | (1 << MarkerUp);
                    if ((mask & searchMask) == searchMask)
                    {
                        sci.MarkerDelete(i, MarkerUp);
                        sci.MarkerDelete(i, MarkerDown);
                        sci.MarkerDelete(i, MarkerUp);      //this needs to be done twice,
                        sci.MarkerDelete(i, MarkerDown);    //because a member could for example implement and override at the same time

                        sci.MarkerAdd(i, MarkerUpDown);
                    }
                }
            }
        }

        /// <summary>
        /// AS2/AS3 detection
        /// </summary>
        /// <param name="doc">Document to check</param>
        /// <returns>Detected language</returns>
        private string DetectActionscriptVersion(ITabbedDocument doc)
        {
            ASFileParser parser = new ASFileParser();
            FileModel model = new FileModel(doc.FileName);
            parser.ParseSrc(model, doc.SciControl.Text);
            if (model.Version == 1 && PluginBase.CurrentProject != null)
            {
                String lang = PluginBase.CurrentProject.Language;
                if (lang == "*") return "as2";
                else return lang;
            }
            else if (model.Version > 2) return "as3";
            else if (model.Version > 1) return "as2";
            else if (settingObject.LastASVersion != null && settingObject.LastASVersion.StartsWithOrdinal("as"))
            {
                return settingObject.LastASVersion;
            }
            else return "as2";
        }

        /// <summary>
        /// Clear and rebuild classpath models cache
        /// </summary>
        private void RebuildClasspath(object sender, EventArgs e)
        {
            ASContext.RebuildClasspath();
        }

        /// <summary>
        /// Open de types explorer dialog
        /// </summary>
        private void TypesExplorer(object sender, EventArgs e)
        {
            ModelsExplorer.Instance.UpdateTree();
            ModelsExplorer.Open();
        }

        /// <summary>
        /// Opens the plugin panel again if closed
        /// </summary>
        public void OpenPanel(object sender, EventArgs e) => pluginPanel.Show();

        /// <summary>
        /// Menu item command: Check ActionScript
        /// </summary>
        public void CheckSyntax(object sender, EventArgs e)
        {
            if (!checking && !PluginBase.MainForm.SavingMultiple)
            {
                checking = true;
                ASContext.Context.CheckSyntax();
                checking = false;
            }
        }

        /// <summary>
        /// Menu item command: Quick Build
        /// </summary>
        public void QuickBuild(object sender, EventArgs e)
        {
            ASContext.Context.BuildCMD(false);
        }

        /// <summary>
        /// Menu item command: Convert To Intrinsic
        /// </summary>
        public void MakeIntrinsic(object sender, EventArgs e)
        {
            if (PluginBase.MainForm.CurrentDocument.IsEditable)
                ASContext.Context.MakeIntrinsic(null);
        }

        /// <summary>
        /// Menu item command: Peek Definition
        /// </summary>
        void PeekDefinition(object sender, EventArgs e)
        {
            if (ASComplete.CurrentResolvedContext?.Result is ASResult result && !result.IsNull())
            {
                var code = ASComplete.GetCodeTipCode(result);
                if (code == null) return;
                UITools.CodeTip.Show(ASContext.CurSciControl, result.Context.PositionExpression, code);
            }
        }

        /// <summary>
        /// Menu item command: Goto Declaration
        /// </summary>
        public void GotoDeclaration(object sender, EventArgs e) => ASComplete.DeclarationLookup(ASContext.CurSciControl);

        /// <summary>
        /// Menu item command: Goto Type Declaration
        /// </summary>
        void GotoTypeDeclaration(object sender, EventArgs e) => ASComplete.TypeDeclarationLookup(ASContext.CurSciControl);

        /// <summary>
        /// Menu item command: Back From Declaration or Type Declaration
        /// </summary>
        public void BackDeclaration(object sender, EventArgs e) => pluginUI.RestoreLastLookupPosition();

        /// <summary>
        /// Sets the IsEnabled value of all the CommandBarItems
        /// </summary>
        /// <param name="enabled">Is the item enabled?</param>
        private void SetItemsEnabled(bool enabled, bool canBuild)
        {
            foreach (ToolStripItem item in menuItems) item.Enabled = enabled;
            quickBuildItem.Enabled = canBuild;
        }

        #endregion

        #region Event handlers

        void OnFileRemove(FileModel obj)
        {
            PluginBase.RunAsync(() =>
            {

                foreach (var cls in obj.Classes)
                {
                    var cached = astCache.GetCachedModel(cls);
                    astCache.Remove(cls);
                    if (cached != null)
                        foreach (var c in cached.ConnectedClassModels) //need to update all connected stuff
                            astCache.MarkAsOutdated(c);
                }

                try
                {
                    astCacheTimer.Stop();
                    astCacheTimer.Start();
                }
                catch
                {
                }

                var sci1 = DocumentManager.FindDocument(obj.FileName)?.SplitSci1;
                var sci2 = DocumentManager.FindDocument(obj.FileName)?.SplitSci2;

                if (sci1 != null)
                {
                    sci1.MarkerDeleteAll(MarkerUp);
                    sci1.MarkerDeleteAll(MarkerDown);
                    sci1.MarkerDeleteAll(MarkerUpDown);
                }
                if (sci2 != null)
                {
                    sci2.MarkerDeleteAll(MarkerUp);
                    sci2.MarkerDeleteAll(MarkerDown);
                    sci2.MarkerDeleteAll(MarkerUpDown);
                }

                EventManager.DispatchEvent(this, new DataEvent(EventType.Command, "ASCompletion.FileModelUpdated", obj));
            });
        }

        /// <summary>
        /// Called when a file is parsed again (could be called multiple times)
        /// </summary>
        /// <param name="obj"></param>
        void OnFileUpdate(FileModel obj)
        {
            PluginBase.RunAsync(() =>
            {
                if (PluginBase.CurrentProject == null) return;

                foreach (var cls in obj.Classes)
                {
                    astCache.MarkAsOutdated(cls);
                }

                astCacheTimer.Stop();
                astCacheTimer.Start();

                EventManager.DispatchEvent(this, new DataEvent(EventType.Command, "ASCompletion.FileModelUpdated", obj));
            });
        }

        void AstCacheTimer_Elapsed(object sender, ElapsedEventArgs e) => astCache.UpdateOutdatedModels();

        void Sci_MarginClick(ScintillaControl sender, int modifiers, int position, int margin)
        {
            if (margin != Margin) return;

            var line = sender.LineFromPosition(position);
            var lineMask = sender.MarkerGet(line);
            if ((lineMask & (1 << MarkerDown)) > 0 || (lineMask & (1 << MarkerUp)) > 0 || (lineMask & (1 << MarkerUpDown)) > 0) //marker is clicked
            {
                var declaration = ASContext.Context.GetDeclarationAtLine(line); //this could be problematic if there are multiple declarations in one line
                var cached = astCache.GetCachedModel(declaration.InClass);

                if (cached == null) return;

                
                if (declaration.InClass.LineFrom == line)
                {
                    ReferenceList.Show(
                        ReferenceList.ConvertClassCache(cached.ImplementorClassModels).ToList(),
                        new List<Reference>(0), 
                        ReferenceList.ConvertClassCache(cached.ChildClassModels).ToList(),
                        new List<Reference>(0)
                    );
                    return;
                }

                if (declaration.Member == null) return;

                HashSet<ClassModel> implementing;
                cached.Implementing.TryGetValue(declaration.Member, out implementing);

                HashSet<ClassModel> implementors;
                cached.Implementors.TryGetValue(declaration.Member, out implementors);

                HashSet<ClassModel> overriders;
                cached.Overriders.TryGetValue(declaration.Member, out overriders);

                HashSet<ClassModel> overridden;
                cached.Overriding.TryGetValue(declaration.Member, out overridden);

                ReferenceList.Show(
                    ReferenceList.ConvertCache(declaration.Member, implementors ?? new HashSet<ClassModel>()).ToList(),
                    ReferenceList.ConvertCache(declaration.Member, implementing ?? new HashSet<ClassModel>()).ToList(),
                    ReferenceList.ConvertCache(declaration.Member, overriders ?? new HashSet<ClassModel>()).ToList(),
                    ReferenceList.ConvertCache(declaration.Member, overridden ?? new HashSet<ClassModel>()).ToList()
                );
            }
        }

        /// <summary>
        /// Display completion list or calltip info
        /// </summary>
        private void OnChar(ScintillaControl sci, int value)
        {
            if (sci.Lexer == 3 || sci.Lexer == 4)
                ASComplete.OnChar(sci, value, true);
        }

        private void OnMouseHover(ScintillaControl sci, int position)
        {
            var ctx = ASContext.Context;
            if (!ctx.IsFileValid) return;
            lastHoverPosition = position;

            // get word at mouse position
            var style = sci.BaseStyleAt(position);
            if (!ASComplete.IsTextStyle(style) && (!ctx.Features.hasInterfaces || !ctx.CodeComplete.IsStringInterpolationStyle(sci, position))) return;
            position = ASComplete.ExpressionEndPosition(sci, position);
            var result = ASComplete.GetExpressionType(sci, position, false, true);

            // set tooltip
            if (!result.IsNull())
            {
                if (Control.ModifierKeys == Keys.Control)
                {
                    var code = ASComplete.GetCodeTipCode(result);
                    if (code == null) return;
                    UITools.CodeTip.Show(sci, position - result.Path.Length, code);
                }
                else
                {
                    var text = ASComplete.GetToolTipText(result);
                    if (text == null) return;
                    // show tooltip
                    UITools.Tip.ShowAtMouseLocation(text);
                }
            }
        }

        private void OnTextChanged(ScintillaControl sender, int position, int length, int linesAdded)
        {
            ASContext.OnTextChanged(sender, position, length, linesAdded);

            if (settingObject.DisableInheritanceNavigation) return;

            var start = sender.LineFromPosition(position);
            var end = sender.LineFromPosition(position + Math.Abs(length));

            for (var i = start; i <= end; ++i)
            {
                var mask = sender.MarkerGet(i);
                var searchMask = (1 << MarkerDown) | (1 << MarkerUp) | (1 << MarkerUpDown);
                if ((mask & searchMask) > 0)
                {
                    sender.MarkerDelete(i, MarkerUp);
                    sender.MarkerDelete(i, MarkerDown);
                    sender.MarkerDelete(i, MarkerUpDown);
                    sender.MarkerDelete(i, MarkerUp);
                    sender.MarkerDelete(i, MarkerDown);
                    sender.MarkerDelete(i, MarkerUpDown);
                }
            }
            
        }

        private void OnUpdateCallTip(ScintillaControl sci, int position)
        {
            if (ASComplete.HasCalltip())
            {
                int pos = sci.CurrentPos - 1;
                char c = (char)sci.CharAt(pos);
                if ((c == ',' || c == '(') && sci.BaseStyleAt(pos) == 0)
                    sci.Colourise(0, -1);
                ASComplete.HandleFunctionCompletion(sci, false);
            }
        }

        private void OnUpdateSimpleTip(ScintillaControl sci, Point mousePosition)
        {
            if (UITools.Tip.Visible)
                OnMouseHover(sci, lastHoverPosition);
        }

        void timerPosition_Elapsed(object sender, ElapsedEventArgs e)
        {
            ScintillaControl sci = ASContext.CurSciControl;
            if (sci == null) return;
            int position = sci.CurrentPos;
            if (position != currentPos)
            {
                currentPos = position;
                ContextChanged();
            }
        }

        private void ContextChanged()
        {
            var doc = PluginBase.MainForm.CurrentDocument;
            var isValid = false;

            if (doc.IsEditable)
            {
                var sci = ASContext.CurSciControl;
                if (currentDoc == doc.FileName && sci != null)
                {
                    var line = sci.LineFromPosition(currentPos);
                    ASContext.SetCurrentLine(line);
                }
                else ASComplete.CurrentResolvedContext = null; // force update

                isValid = ASContext.Context.IsFileValid;
                if (isValid) ASComplete.ResolveContext(sci);
            }
            else ASComplete.ResolveContext(null);
            
            var enableItems = isValid && !doc.IsUntitled;
            pluginUI.OutlineTree.Enabled = ASContext.Context.CurrentModel != null;
            SetItemsEnabled(enableItems, ASContext.Context.CanBuild);
        }
        #endregion
    }

}
