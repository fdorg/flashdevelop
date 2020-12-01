using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
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
        string dataPath;
        string settingsFile;
        GeneralSettings settingObject;
        PluginUI pluginUI;
        Image pluginIcon;
        const EventType eventMask =
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

        List<ToolStripItem> menuItems;
        ToolStripItem quickBuildItem;
        int currentPos;
        string currentDoc;
        bool started;
        FlashErrorsWatcher flashErrorsWatcher;
        bool checking;
        Timer timerPosition;
        int lastHoverPosition;

        readonly Regex reVirtualFile = new Regex("\\.(swf|swc)::", RegexOptions.Compiled);
        readonly Regex reArgs = new Regex("\\$\\((Typ|Mbr|Itm)", RegexOptions.Compiled);
        readonly Regex reCostlyArgs = new Regex("\\$\\((TypClosest|ItmUnique)", RegexOptions.Compiled);

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

        public int Api => 1;

        public string Name { get; } = nameof(ASCompletion);

        public string Guid { get; } = "078c7c1a-c667-4f54-9e47-d45c0e835c4e";

        public string Author { get; } = "FlashDevelop Team";

        public string Description { get; set; } = "Code completion engine for FlashDevelop.";

        public string Help { get; } = "www.flashdevelop.org/community/";

        [Browsable(false)]
        public virtual object Settings => settingObject;

        #endregion

        #region Plugin Properties

        [Browsable(false)]
        public virtual PluginUI Panel => pluginUI;

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
                    pluginUI.UpdateAfterTheme();
                }

                // current active document
                var doc = PluginBase.MainForm.CurrentDocument;
                // editor ready?
                if (doc is null) return;
                var sci = doc.SciControl;

                //
                //  Events always handled
                //
                DataEvent de;
                switch (e.Type)
                {
                    // caret position in editor
                    case EventType.UIRefresh:
                        if (sci is null) return;
                        ASContext.Context.OnBraceMatch(sci);
                        timerPosition.Enabled = false;
                        timerPosition.Enabled = true;
                        return;

                    // key combinations
                    case EventType.Keys:
                        var keys = ((KeyEvent) e).Value;
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
                        if (sci is null) return;
                        e.Handled = ASComplete.OnShortcut(keys, sci);
                        return;

                    // user-customized shortcuts
                    case EventType.Shortcut:
                        de = (DataEvent) e;
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
                        if (PluginBase.MainForm.CurrentDocument is {} tabbedDocument)
                        {
                            ApplyMarkers(tabbedDocument.SplitSci1);
                            ApplyMarkers(tabbedDocument.SplitSci2);
                        }
                        break;

                    case EventType.FileSave:
                        if (sci is null) return;
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
                        // detect ActionScript language version
                        if (sci is null) return;
                        if (sci.FileName.ToLower().EndsWithOrdinal(".as"))
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
                        if (sci is null)
                        {
                            ASContext.SetCurrentFile(null, true);
                            ContextChanged();
                            return;
                        }
                        currentDoc = sci.FileName;
                        currentPos = sci.CurrentPos;
                        ASContext.SetCurrentFile(doc, false);
                        // UI
                        ContextChanged();
                        return;

                    case EventType.Completion:
                        if (ASContext.Context.IsFileValid) e.Handled = true;
                        return;

                    // some commands work all the time
                    case EventType.Command:
                        de = (DataEvent) e;
                        var command = de.Action ?? "";

                        if (command.StartsWithOrdinal("ASCompletion."))
                        {
                            var cmdData = de.Data as string;

                            // add a custom classpath
                            if (command == "ASCompletion.ClassPath")
                            {
                                if (de.Data is Hashtable info)
                                {
                                    var setup = new ContextSetupInfos
                                    {
                                        Platform = (string) info["platform"],
                                        Lang = (string) info["lang"],
                                        Version = (string) info["version"],
                                        TargetBuild = (string) info["targetBuild"],
                                        Classpath = (string[]) info["classpath"],
                                        HiddenPaths = (string[]) info["hidden"]
                                    };
                                    ASContext.SetLanguageClassPath(setup);
                                    if (setup.AdditionalPaths != null) // report custom classpath
                                        info["additional"] = setup.AdditionalPaths.ToArray();
                                }
                                e.Handled = true;
                            }
                            // send a UserClasspath
                            else if (command == "ASCompletion.GetUserClasspath")
                            {
                                if (de.Data is Hashtable info && info.ContainsKey("language"))
                                {
                                    var context = ASContext.GetLanguageContext(info["language"] as string);
                                    if (context?.Settings?.UserClasspath != null)
                                        info["cp"] = new List<string>(context.Settings.UserClasspath);
                                }
                                e.Handled = true;
                            }
                            // update a UserClasspath
                            else if (command == "ASCompletion.SetUserClasspath")
                            {
                                if (de.Data is Hashtable info && info.ContainsKey("language") && info.ContainsKey("cp"))
                                {
                                    var context = ASContext.GetLanguageContext(info["language"] as string);
                                    if (info["cp"] is List<string> cp && context?.Settings != null)
                                    {
                                        context.Settings.UserClasspath = cp.ToArray();
                                    }
                                }
                                e.Handled = true;
                            }
                            // send the language's default compiler path
                            else if (command == "ASCompletion.GetCompilerPath")
                            {
                                if (de.Data is Hashtable info && info.ContainsKey("language"))
                                {
                                    var context = ASContext.GetLanguageContext(info["language"] as string);
                                    if (context != null)
                                        info["compiler"] = context.GetCompilerPath();
                                }
                                e.Handled = true;
                            }
                            // show a language's compiler settings
                            else if (command == "ASCompletion.ShowSettings")
                            {
                                e.Handled = true;
                                var context = ASContext.GetLanguageContext(cmdData);
                                if (context is null) return;
                                const string filter = "SDK";
                                var name = cmdData.ToUpper() switch
                                {
                                    "AS2" => "AS2Context",
                                    "AS3" => "AS3Context",
                                    _ => cmdData.Substring(0, 1).ToUpper() + cmdData.Substring(1) + "Context",
                                };
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
                                if (flashErrorsWatcher is null) flashErrorsWatcher = new FlashErrorsWatcher();
                                e.Handled = CallFlashIDE.Run(settingObject.PathToFlashIDE, cmdData);
                            }
                            // create Flash 8+ trust file
                            else if (command == "ASCompletion.CreateTrustFile")
                            {
                                if (cmdData != null)
                                {
                                    var args = cmdData.Split(';');
                                    if (args.Length == 2)
                                        e.Handled = CreateTrustFile.Run(args[0], args[1]);
                                }
                            }
                            else if (command == "ASCompletion.GetClassPath")
                            {
                                if (cmdData != null)
                                {
                                    var args = cmdData.Split(';');
                                    if (args.Length == 1)
                                    {
                                        var model = ASContext.Context.GetFileModel(args[0]);
                                        var aClass = model.GetPublicClass();
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
                                foreach (var cp in ASContext.Context.Classpath)
                                    cp.DisableWatcher();
                            }
                            else if (command == "ProjectManager.FileActions.EnableWatchers")
                            {
                                // classpaths could be invalid now - remove those, BuildClassPath() is too expensive
                                ASContext.Context.Classpath.RemoveAll(cp => !Directory.Exists(cp.Path));

                                foreach (var cp in ASContext.Context.Classpath)
                                    cp.EnableWatcher();
                            }
                            // Return requested language SDK list
                            else if (command == "ASCompletion.InstalledSDKs")
                            {
                                if (de.Data is Hashtable info && info.ContainsKey("language"))
                                {
                                    var context = ASContext.GetLanguageContext(info["language"] as string);
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
                            var cmdData = de.Data as string;
                            if (reVirtualFile.IsMatch(cmdData))
                            {
                                var path = Regex.Split(cmdData, "::");
                                var fileName = path[0] + Path.DirectorySeparatorChar
                                    + path[1].Replace('.', Path.DirectorySeparatorChar).Replace("::", Path.DirectorySeparatorChar.ToString());
                                var found = ModelsExplorer.Instance.OpenFile(fileName);
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
                            var te = (TextEvent) e;
                            if (reArgs.IsMatch(te.Value))
                            {
                                // resolve current element
                                var details = ASComplete.ResolveElement(sci, null);
                                te.Value = ArgumentsProcessor.Process(te.Value, details);

                                if (te.Value.Contains('$') && reCostlyArgs.IsMatch(te.Value))
                                {
                                    var result = ASComplete.CurrentResolvedContext.Result ?? new ASResult();
                                    details = new Hashtable();
                                    // Get closest list (Array or Vector)
                                    string closestListName = "", closestListItemType = "";
                                    ASComplete.FindClosestList(ASContext.Context, result.Context, sci.CurrentLine, ref closestListName, ref closestListItemType);
                                    details.Add("TypClosestListName", closestListName);
                                    details.Add("TypClosestListItemType", closestListItemType);
                                    // get free iterator index
                                    var iterator = ASComplete.FindFreeIterator(ASContext.Context, ASContext.Context.CurrentClass, result.Context);
                                    details.Add("ItmUniqueVar", iterator);
                                    te.Value = ArgumentsProcessor.Process(te.Value, details);
                                }
                            }
                            break;

                        // menu commands
                        case EventType.Command:
                            de = (DataEvent) e;
                            var command = de.Action ?? string.Empty;
                            if (command.StartsWithOrdinal("ASCompletion."))
                            {
                                var cmdData = de.Data as string;
                                switch (command)
                                {
                                    // run MTASC
                                    case "ASCompletion.CustomBuild":
                                        ASContext.Context.RunCMD(cmdData ?? string.Empty);
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
                                        ASComplete.OnShortcut(Keys.Control | Keys.Space, sci);
                                        e.Handled = true;
                                        break;

                                    case "ASCompletion.CtrlShiftSpace":
                                        ASComplete.OnShortcut(Keys.Control | Keys.Shift | Keys.Space, sci);
                                        e.Handled = true;
                                        break;

                                    case "ASCompletion.CtrlAltSpace":
                                        ASComplete.OnShortcut(Keys.Control | Keys.Alt | Keys.Space, sci);
                                        e.Handled = true;
                                        break;

                                    case "ASCompletion.ContextualGenerator":
                                        if (ASContext.HasContext)
                                        {
                                            var options = new List<ICompletionListItem>();
                                            ASGenerator.ContextualGenerator(sci, options);
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
                            ASContext.Context.OnProcessEnd(((TextEvent) e).Value);
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
        public DockContent PluginPanel { get; set; }

        /// <summary>
        /// Gets the PluginSettings
        /// </summary>
        [Browsable(false)]
        public virtual GeneralSettings PluginSettings => settingObject;

        [Browsable(false)]
        public virtual List<ToolStripItem> MenuItems => menuItems;

        #endregion

        #region Initialization

        void InitSettings()
        {
            Description = TextHelper.GetString("Info.Description");
            dataPath = Path.Combine(PathHelper.DataDir, nameof(ASCompletion));
            if (!Directory.Exists(dataPath)) Directory.CreateDirectory(dataPath);
            else if (PluginBase.MainForm.RefreshConfig) PathExplorer.ClearPersistentCache();
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
                settingObject = ObjectSerializer.Deserialize(settingsFile, settingObject);
            }
        }

        void SaveSettings() => ObjectSerializer.Serialize(settingsFile, settingObject);

        void LoadBitmaps()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var stream = assembly.GetManifestResourceStream("ASCompletion.Icons.UpDownArrow.png");

            upDownArrow = new Bitmap(PluginBase.MainForm.ImageSetAdjust(Image.FromStream(stream)));
            downArrow = new Bitmap(PluginBase.MainForm.FindImage16("22"));
            upArrow = new Bitmap(PluginBase.MainForm.FindImage16("8"));
        }

        void CreatePanel()
        {
            pluginIcon = PluginBase.MainForm.FindImage("99");
            pluginUI = new PluginUI(this) {Text = TextHelper.GetString("Title.PluginPanel")};
            PluginPanel = PluginBase.MainForm.CreateDockablePanel(pluginUI, Guid, pluginIcon, DockState.DockRight);
        }

        void CreateMenuItems()
        {
            var mainForm = PluginBase.MainForm;
            menuItems = new List<ToolStripItem>();
            ToolStripMenuItem item;
            var menu = (ToolStripMenuItem)mainForm.FindMenuItem("ViewMenu");
            if (menu != null)
            {
                item = new ToolStripMenuItem(TextHelper.GetString("Label.ViewMenuItem"), pluginIcon, OpenPanel);
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
                item = new ToolStripMenuItem(TextHelper.GetString("Label.CheckSyntax"), image, CheckSyntax, Keys.F7);
                PluginBase.MainForm.RegisterShortcutItem("FlashToolsMenu.CheckSyntax", item);
                menu.DropDownItems.Add(item);
                menuItems.Add(item);

                // quick build
                image = pluginUI.GetIcon(PluginUI.ICON_QUICK_BUILD);
                item = new ToolStripMenuItem(TextHelper.GetString("Label.QuickBuild"), image, QuickBuild, Keys.Control | Keys.F8);
                PluginBase.MainForm.RegisterShortcutItem("FlashToolsMenu.QuickBuild", item);
                menu.DropDownItems.Add(item);
                //menuItems.Add(item);
                quickBuildItem = item;

                menu.DropDownItems.Add(new ToolStripSeparator());

                // model cleanup
                image = mainForm.FindImage("153");
                item = new ToolStripMenuItem(TextHelper.GetString("Label.RebuildClasspathCache"), image, RebuildClasspath);
                PluginBase.MainForm.RegisterShortcutItem("FlashToolsMenu.RebuildClasspathCache", item);
                menu.DropDownItems.Add(item);

                // convert to intrinsic
                item = new ToolStripMenuItem(TextHelper.GetString("Label.ConvertToIntrinsic"), null, MakeIntrinsic);
                PluginBase.MainForm.RegisterShortcutItem("FlashToolsMenu.ConvertToIntrinsic", item);
                menu.DropDownItems.Add(item);
                menuItems.Add(item);
            }

            // toolbar items
            var toolStrip = mainForm.ToolStrip;
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
                var emenu = mainForm.EditorMenu;
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

        void AddEventHandlers()
        {
            // scintilla controls listeners
            UITools.Manager.OnCharAdded += OnChar;
            UITools.Manager.OnMouseHover += OnMouseHover;
            UITools.Manager.OnTextChanged += OnTextChanged;
            UITools.CallTip.OnUpdateCallTip += OnUpdateCallTip;
            UITools.Tip.OnUpdateSimpleTip += OnUpdateSimpleTip;
            CompletionList.OnInsert += ASComplete.HandleCompletionInsert;
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
            timerPosition.Elapsed += TimerPosition_Elapsed;

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
            if (PluginBase.CurrentProject is null) return;
            astCache.UpdateCompleteCache();
        }

        void UpdateOpenDocumentMarkers()
        {
            foreach (var document in PluginBase.MainForm.Documents)
            {
                if (document.SciControl is null) continue;
                UpdateMarkersFromCache(document.SplitSci1);
                UpdateMarkersFromCache(document.SplitSci2);
            }
        }

        void ApplyMarkers(ScintillaControl sci)
        {
            if (settingObject.DisableInheritanceNavigation || sci is null) return;
            //Register marker
            sci.MarkerDefineRGBAImage(MarkerDown, downArrow);
            sci.MarkerDefineRGBAImage(MarkerUp, upArrow);
            sci.MarkerDefineRGBAImage(MarkerUpDown, upDownArrow);
            //Setup margin
            const int mask = (1 << MarkerDown) | (1 << MarkerUp) | (1 << MarkerUpDown);
            sci.SetMarginMaskN(Margin, mask);
            sci.MarginSensitiveN(Margin, true);
            sci.MarginClick -= Sci_MarginClick;
            sci.MarginClick += Sci_MarginClick;
            UpdateMarkersFromCache(sci);
        }

        void UpdateMarkersFromCache(ScintillaControl sci)
        {
            const int marginWidth = 16;
            sci.SetMarginWidthN(Margin, 0); //margin is only made visible if something is found

            sci.MarkerDeleteAll(MarkerUp);
            sci.MarkerDeleteAll(MarkerDown);
            sci.MarkerDeleteAll(MarkerUpDown);

            if (settingObject.DisableInheritanceNavigation) return;

            if (PluginBase.CurrentProject is null) return;
            var context = ASContext.GetLanguageContext(PluginBase.CurrentProject.Language) as ASContext;
            if (context is null) return;

            var fileModel = context.GetCachedFileModel(sci.FileName);

            for (var index = 0; index < fileModel.Classes.Count; index++)
            {
                var @class = fileModel.Classes[index];
                var model = astCache.GetCachedModel(@class);
                if (model is null) return;

                if (model.ChildClassModels.Count > 0 || model.ImplementorClassModels.Count > 0)
                {
                    sci.SetMarginWidthN(Margin, marginWidth);
                    sci.MarkerAdd(@class.LineFrom, MarkerDown);
                }

                foreach (var implementing in model.Implementing)
                {
                    sci.SetMarginWidthN(Margin, marginWidth);
                    sci.MarkerAdd(implementing.Key.LineFrom, MarkerUp);
                }

                foreach (var implementor in model.Implementors)
                {
                    sci.SetMarginWidthN(Margin, marginWidth);
                    sci.MarkerAdd(implementor.Key.LineFrom, MarkerDown);
                }

                foreach (var overriders in model.Overriders)
                {
                    sci.SetMarginWidthN(Margin, marginWidth);
                    sci.MarkerAdd(overriders.Key.LineFrom, MarkerDown);
                }

                foreach (var overrides in model.Overriding)
                {
                    sci.SetMarginWidthN(Margin, marginWidth);
                    sci.MarkerAdd(overrides.Key.LineFrom, MarkerUp);
                }

                var lineCount = sci.LineCount;
                for (var i = 0; i < lineCount; ++i)
                {
                    var mask = sci.MarkerGet(i);
                    const int searchMask = (1 << MarkerDown) | (1 << MarkerUp);
                    if ((mask & searchMask) == searchMask)
                    {
                        sci.MarkerDelete(i, MarkerUp);
                        sci.MarkerDelete(i, MarkerDown);
                        sci.MarkerDelete(i, MarkerUp); //this needs to be done twice,
                        sci.MarkerDelete(i, MarkerDown); //because a member could for example implement and override at the same time
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
        string DetectActionscriptVersion(ITabbedDocument doc)
        {
            var parser = new ASFileParser();
            var sci = doc.SciControl;
            var model = new FileModel(sci.FileName);
            parser.ParseSrc(model, sci.Text);
            if (model.Version == 1 && PluginBase.CurrentProject != null)
            {
                var lang = PluginBase.CurrentProject.Language;
                return lang == "*"
                    ? "as2"
                    : lang;
            }
            if (model.Version > 2) return "as3";
            if (model.Version > 1) return "as2";
            if (settingObject.LastASVersion != null && settingObject.LastASVersion.StartsWithOrdinal("as"))
            {
                return settingObject.LastASVersion;
            }
            return "as2";
        }

        /// <summary>
        /// Clear and rebuild classpath models cache
        /// </summary>
        static void RebuildClasspath(object sender, EventArgs e) => ASContext.RebuildClasspath();

        /// <summary>
        /// Open de types explorer dialog
        /// </summary>
        static void TypesExplorer(object sender, EventArgs e)
        {
            ModelsExplorer.Instance.UpdateTree();
            ModelsExplorer.Open();
        }

        /// <summary>
        /// Opens the plugin panel again if closed
        /// </summary>
        public void OpenPanel(object sender, EventArgs e) => PluginPanel.Show();

        /// <summary>
        /// Menu item command: Check ActionScript
        /// </summary>
        public void CheckSyntax(object sender, EventArgs e)
        {
            if (checking || PluginBase.MainForm.SavingMultiple) return;
            checking = true;
            ASContext.Context.CheckSyntax();
            checking = false;
        }

        /// <summary>
        /// Menu item command: Quick Build
        /// </summary>
        public void QuickBuild(object sender, EventArgs e) => ASContext.Context.BuildCMD(false);

        /// <summary>
        /// Menu item command: Convert To Intrinsic
        /// </summary>
        public void MakeIntrinsic(object sender, EventArgs e)
        {
            if (PluginBase.MainForm.CurrentDocument is {} doc && doc.IsEditable)
                ASContext.Context.MakeIntrinsic(null);
        }

        /// <summary>
        /// Menu item command: Peek Definition
        /// </summary>
        static void PeekDefinition(object sender, EventArgs e)
        {
            if (ASComplete.CurrentResolvedContext?.Result is { } result && !result.IsNull())
            {
                var code = ASComplete.GetCodeTipCode(result);
                if (code is null) return;
                UITools.CodeTip.Show(PluginBase.MainForm.CurrentDocument?.SciControl, result.Context.PositionExpression, code);
            }
        }

        /// <summary>
        /// Menu item command: Goto Declaration
        /// </summary>
        public void GotoDeclaration(object sender, EventArgs e) => ASComplete.DeclarationLookup(PluginBase.MainForm.CurrentDocument?.SciControl);

        /// <summary>
        /// Menu item command: Goto Type Declaration
        /// </summary>
        static void GotoTypeDeclaration(object sender, EventArgs e) => ASComplete.TypeDeclarationLookup(PluginBase.MainForm.CurrentDocument?.SciControl);

        /// <summary>
        /// Menu item command: Back From Declaration or Type Declaration
        /// </summary>
        public void BackDeclaration(object sender, EventArgs e) => pluginUI.RestoreLastLookupPosition();

        /// <summary>
        /// Sets the IsEnabled value of all the CommandBarItems
        /// </summary>
        /// <param name="enabled">Is the item enabled?</param>
        void SetItemsEnabled(bool enabled, bool canBuild)
        {
            foreach (var item in menuItems) item.Enabled = enabled;
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
                if (sci1 != null)
                {
                    sci1.MarkerDeleteAll(MarkerUp);
                    sci1.MarkerDeleteAll(MarkerDown);
                    sci1.MarkerDeleteAll(MarkerUpDown);
                }
                var sci2 = DocumentManager.FindDocument(obj.FileName)?.SplitSci2;
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
                if (PluginBase.CurrentProject is null) return;

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
                if (cached is null) return;
                if (declaration.InClass.LineFrom == line)
                {
                    ReferenceList.Show(
                        ReferenceList.ConvertClassCache(cached.ImplementorClassModels),
                        new List<Reference>(0), 
                        ReferenceList.ConvertClassCache(cached.ChildClassModels),
                        new List<Reference>(0)
                    );
                    return;
                }

                if (declaration.Member is null) return;

                cached.Implementing.TryGetValue(declaration.Member, out var implementing);
                cached.Implementors.TryGetValue(declaration.Member, out var implementors);
                cached.Overriders.TryGetValue(declaration.Member, out var overriders);
                cached.Overriding.TryGetValue(declaration.Member, out var overridden);

                ReferenceList.Show(
                    ReferenceList.ConvertCache(declaration.Member, implementors ?? new HashSet<ClassModel>()),
                    ReferenceList.ConvertCache(declaration.Member, implementing ?? new HashSet<ClassModel>()),
                    ReferenceList.ConvertCache(declaration.Member, overriders ?? new HashSet<ClassModel>()),
                    ReferenceList.ConvertCache(declaration.Member, overridden ?? new HashSet<ClassModel>())
                );
            }
        }

        /// <summary>
        /// Display completion list or calltip info
        /// </summary>
        static void OnChar(ScintillaControl sci, int value)
        {
            if (sci.Lexer == 3 || sci.Lexer == 4)
                ASComplete.OnChar(sci, value, true);
        }

        void OnMouseHover(ScintillaControl sci, int position)
        {
            var ctx = ASContext.Context;
            if (!ctx.IsFileValid) return;
            lastHoverPosition = position;
            if (!ctx.CodeComplete.IsAvailableForToolTip(sci, position)) return;
            position = ASComplete.ExpressionEndPosition(sci, position);
            var expr = ASComplete.GetExpressionType(sci, position, false, true);
            if (Control.ModifierKeys == Keys.Control)
            {
                if (ASComplete.GetCodeTipCode(expr) is { } text) UITools.CodeTip.Show(sci, position - expr.Path.Length, text);
            }
            else
            {
                if (ASComplete.GetToolTipText(expr) is { } text) UITools.Tip.ShowAtMouseLocation(text);
            }
        }

        void OnTextChanged(ScintillaControl sender, int position, int length, int linesAdded)
        {
            ASContext.OnTextChanged(sender, position, length, linesAdded);

            if (settingObject.DisableInheritanceNavigation) return;

            var start = sender.LineFromPosition(position);
            var end = sender.LineFromPosition(position + Math.Abs(length));

            for (var i = start; i <= end; ++i)
            {
                var mask = sender.MarkerGet(i);
                const int searchMask = (1 << MarkerDown) | (1 << MarkerUp) | (1 << MarkerUpDown);
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

        static void OnUpdateCallTip(ScintillaControl sci, int position)
        {
            if (!ASComplete.HasCalltip()) return;
            var pos = sci.CurrentPos - 1;
            var c = (char)sci.CharAt(pos);
            if ((c == ',' || c == '(') && sci.BaseStyleAt(pos) == 0)
                sci.Colourise(0, -1);
            ASComplete.HandleFunctionCompletion(sci, false);
        }

        void OnUpdateSimpleTip(ScintillaControl sci, Point mousePosition)
        {
            if (UITools.Tip.Visible)
                OnMouseHover(sci, lastHoverPosition);
        }

        void TimerPosition_Elapsed(object sender, ElapsedEventArgs e)
        {
            var sci = PluginBase.MainForm.CurrentDocument?.SciControl;
            if (sci is null) return;
            var position = sci.CurrentPos;
            if (position == currentPos) return;
            currentPos = position;
            ContextChanged();
        }

        void ContextChanged()
        {
            var doc = PluginBase.MainForm.CurrentDocument;
            var isValid = false;
            if (doc?.SciControl is { } sci)
            {
                if (currentDoc == sci.FileName)
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