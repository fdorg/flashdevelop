using System.ComponentModel;
using System.Drawing;
using System.IO;
using FlashDebugger.Debugger;
using PluginCore;
using PluginCore.Helpers;
using PluginCore.Localization;
using PluginCore.Managers;
using PluginCore.Utilities;
using ProjectManager;

namespace FlashDebugger
{
    public class PluginMain : IPlugin
    {
        PanelsHelper panelsHelpers;
        MenusHelper menusHelper;
        string settingFilename;
        Image pluginImage;
        bool firstRun;

        internal static Settings settingObject;
        internal static LiveDataTip liveDataTip;
        internal static DebuggerManager debugManager;
        internal static BreakPointManager breakPointManager;
        internal static WatchManager watchManager;
        internal static bool disableDebugger;

        #region Required Properties

        /// <summary>
        /// Api level of the plugin
        /// </summary>
        public int Api => 1;

        /// <summary>
        /// Name of the plugin
        /// </summary> 
        public string Name => nameof(FlashDebugger);

        /// <summary>
        /// GUID of the plugin
        /// </summary>
        public string Guid => PanelsHelper.localsGuid;

        /// <summary>
        /// Author of the plugin
        /// </summary> 
        public string Author => "FlashDevelop Team";

        /// <summary>
        /// Description of the plugin
        /// </summary> 
        public string Description { get; set; } = "Hosts the ActionScript 3 debugger in FlashDevelop.";

        /// <summary>
        /// Web address for help
        /// </summary> 
        public string Help => "http://www.flashdevelop.org/community/";

        /// <summary>
        /// Object that contains the settings
        /// </summary>
        [Browsable(false)]
        public object Settings => settingObject;

        #endregion
        
        #region Required Methods
        
        /// <summary>
        /// Initializes the plugin
        /// </summary>
        public void Initialize()
        {
            InitBasics();
            LoadSettings();
            AddEventHandlers();
            InitLocalization();
            CreateMenuItems();
            CreatePluginPanel();
        }
        
        /// <summary>
        /// Disposes the plugin
        /// </summary>
        public void Dispose()
        {
            SaveSettings();
            breakPointManager.Save();
            watchManager.Save();
            debugManager.Cleanup();
        }

        /// <summary>
        /// Handles the incoming events
        /// </summary>
        public void HandleEvent(object sender, NotifyEvent e, HandlingPriority priority)
        {
            if (debugManager is null) return;
            switch (e.Type)
            {
                case EventType.FileOpen:
                    var te = (TextEvent)e;
                    ScintillaHelper.AddSciEvent(te.Value);
                    breakPointManager.SetBreakPointsToEditor(te.Value);
                    break;

                case EventType.UIStarted:
                    menusHelper.AddToolStripItems();
                    menusHelper.UpdateMenuState(this, DebuggerState.Initializing);
                    debugManager.RestoreOldLayout();
                    break;
                
                case EventType.UIClosing:
                    if (debugManager.FlashInterface.isDebuggerStarted)
                    {
                        debugManager.FlashInterface.Detach();
                    }
                    break;

                case EventType.ApplySettings:
                    menusHelper.UpdateMenuState(this);
                    break;

                case EventType.FileSwitch:
                    menusHelper.UpdateMenuState(this);
                    break;

                case EventType.Command:
                    var de = (DataEvent)e;
                    if (de.Action == "AS3Context.StartDebugger")
                    {
                        if (settingObject.StartDebuggerOnTestMovie)
                        {
                            if (debugManager.Start(de.Data != null)) de.Handled = true;
                        }
                        return;
                    }
                    if (!de.Action.StartsWithOrdinal("ProjectManager"))  return;
                    if (de.Action == ProjectManagerEvents.Project)
                    {
                        var project = PluginBase.CurrentProject;
                        if (project != null && project.EnableInteractiveDebugger)
                        {
                            disableDebugger = false;
                            if (breakPointManager.Project != null && breakPointManager.Project != project)
                            {
                                breakPointManager.Save();
                                watchManager.Save();
                            }
                            PanelsHelper.breakPointUI.Clear();
                            PanelsHelper.watchUI.Clear();
                            breakPointManager.Project = project;
                            breakPointManager.Load();
                            breakPointManager.SetBreakPointsToEditor(PluginBase.MainForm.Documents);

                            watchManager.Project = project;
                            watchManager.Load();
                        }
                        else
                        {
                            disableDebugger = true;
                            if (breakPointManager.Project != null)
                            {
                                breakPointManager.Save();
                                watchManager.Save();
                            }
                            PanelsHelper.breakPointUI.Clear();
                            PanelsHelper.watchUI.Clear();
                        }
                    }
                    else if (disableDebugger) return;
                    if (de.Action == ProjectManagerCommands.HotBuild || de.Action == ProjectManagerCommands.BuildProject)
                    {
                        if (debugManager.FlashInterface.isDebuggerStarted)
                        {
                            if (debugManager.FlashInterface.isDebuggerSuspended)
                            {
                                debugManager.Continue_Click(null, null);
                            }
                            debugManager.Stop_Click(null, null);
                        }
                    }
                    if (de.Action == ProjectManagerEvents.TestProject)
                    {
                        if (debugManager.FlashInterface.isDebuggerStarted)
                        {
                            if (debugManager.FlashInterface.isDebuggerSuspended)
                            {
                                debugManager.Continue_Click(null, null);
                                e.Handled = true;
                                return;
                            }
                        }
                    }
                    if (de.Action == ProjectManagerEvents.TestProject)
                    {
                        menusHelper.UpdateMenuState(this, DebuggerState.Initializing);
                    }
                    break;
            }
        }
        
        #endregion

        #region Custom Methods

        /// <summary>
        /// Initializes important variables
        /// </summary>
        void InitBasics()
        {
            var path = Path.Combine(PathHelper.DataDir, nameof(FlashDebugger));
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            settingFilename = Path.Combine(path, "Settings.fdb");
            pluginImage = PluginBase.MainForm.FindImage("54|23|5|4");
            breakPointManager = new BreakPointManager();
            watchManager = new WatchManager();
            debugManager = new DebuggerManager();
            liveDataTip = new LiveDataTip();
        }

        /// <summary>
        /// Creates the required menu items
        /// </summary>
        void CreateMenuItems() => menusHelper = new MenusHelper(pluginImage, debugManager);

        /// <summary>
        /// Creates a plugin panel for the plugin
        /// </summary>
        void CreatePluginPanel()
        {
            panelsHelpers = new PanelsHelper(this, pluginImage);
            if (firstRun) panelsHelpers.DockTogether();
        }

        /// <summary>
        /// Initializes the localization of the plugin
        /// </summary>
        void InitLocalization() => Description = TextHelper.GetString("Info.Description");

        /// <summary>
        /// Adds the required event handlers
        /// </summary> 
        void AddEventHandlers()
        {
            EventManager.AddEventHandler(this, EventType.FileOpen | EventType.UIClosing | EventType.FileSwitch | EventType.ApplySettings);
            EventManager.AddEventHandler(this, EventType.UIStarted, HandlingPriority.Low);
            EventManager.AddEventHandler(this, EventType.Command, HandlingPriority.High);
        }

        #endregion

        #region Settings Management

        /// <summary>
        /// Loads the plugin settings
        /// </summary>
        public void LoadSettings()
        {
            settingObject = new Settings();
            if (!File.Exists(settingFilename))
            {
                SaveSettings();
                firstRun = true;
            }
            else
            {
                settingObject = (Settings)ObjectSerializer.Deserialize(settingFilename, settingObject);
            }
        }

        /// <summary>
        /// Saves the plugin settings
        /// </summary>
        public void SaveSettings() => ObjectSerializer.Serialize(settingFilename, settingObject);

        #endregion
    }
}