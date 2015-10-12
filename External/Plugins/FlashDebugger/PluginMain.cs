using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
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
        private String pluginName = "FlashDebugger";
        private String pluginHelp = "http://www.flashdevelop.org/community/";
        private String pluginDesc = "Hosts the ActionScript 3 debugger in FlashDevelop.";
        private String pluginAuth = "FlashDevelop Team";
        private PanelsHelper panelsHelpers;
        private MenusHelper menusHelper;
        private String settingFilename;
        private Image pluginImage;

        static internal Settings settingObject;
        static internal LiveDataTip liveDataTip;
        static internal DebuggerManager debugManager;
        static internal BreakPointManager breakPointManager;
        static internal Boolean disableDebugger = false;

        #region Required Properties

        /// <summary>
        /// Api level of the plugin
        /// </summary>
        public Int32 Api
        {
            get { return 1; }
        }

        /// <summary>
        /// Name of the plugin
        /// </summary> 
        public String Name
        {
            get { return this.pluginName; }
        }

        /// <summary>
        /// GUID of the plugin
        /// </summary>
        public String Guid
        {
            get { return PanelsHelper.localsGuid; }
        }

        /// <summary>
        /// Author of the plugin
        /// </summary> 
        public String Author
        {
            get { return this.pluginAuth; }
        }

        /// <summary>
        /// Description of the plugin
        /// </summary> 
        public String Description
        {
            get { return this.pluginDesc; }
        }

        /// <summary>
        /// Web address for help
        /// </summary> 
        public String Help
        {
            get { return this.pluginHelp; }
        }

        /// <summary>
        /// Object that contains the settings
        /// </summary>
        [Browsable(false)]
        public Object Settings
        {
            get { return settingObject; }
        }
        
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
            debugManager.Cleanup();
        }

        /// <summary>
        /// Handles the incoming events
        /// </summary>
        public void HandleEvent(Object sender, NotifyEvent e, HandlingPriority priority)
        {
            if (debugManager == null) return;
            switch (e.Type)
            {
                case EventType.FileOpen:
                    TextEvent evnt = (TextEvent)e;
                    ScintillaHelper.AddSciEvent(evnt.Value);
                    breakPointManager.SetBreakPointsToEditor(evnt.Value);
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
                    DataEvent buildevnt = (DataEvent)e;
                    if (buildevnt.Action == "AS3Context.StartDebugger")
                    {
                        if (settingObject.StartDebuggerOnTestMovie)
                        {
                            if (debugManager.Start(buildevnt.Data != null)) buildevnt.Handled = true;
                        }
                        return;
                    }
                    if (!buildevnt.Action.StartsWith("ProjectManager"))  return;
                    if (buildevnt.Action == ProjectManagerEvents.Project)
                    {
                        IProject project = PluginBase.CurrentProject;
                        if (project != null && project.EnableInteractiveDebugger)
                        {
                            disableDebugger = false;
                            PanelsHelper.breakPointUI.Clear();
                            if (breakPointManager.Project != null && breakPointManager.Project != project)
                            {
                                breakPointManager.Save();
                            }
                            breakPointManager.Project = project;
                            breakPointManager.Load();
                            breakPointManager.SetBreakPointsToEditor(PluginBase.MainForm.Documents);
                        }
                        else
                        {
                            disableDebugger = true;
                            if (breakPointManager.Project != null)
                            {
                                breakPointManager.Save();
                            }
                            PanelsHelper.breakPointUI.Clear();
                        }
                    }
                    else if (disableDebugger) return;
                    if (buildevnt.Action == ProjectManagerCommands.HotBuild || buildevnt.Action == ProjectManagerCommands.BuildProject)
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
                    if (buildevnt.Action == ProjectManagerEvents.TestProject)
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
                    if (buildevnt.Action == ProjectManagerEvents.TestProject)
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
        private void InitBasics()
        {
            String dataPath = Path.Combine(PathHelper.DataDir, "FlashDebugger");
            if (!Directory.Exists(dataPath)) Directory.CreateDirectory(dataPath);
            this.settingFilename = Path.Combine(dataPath, "Settings.fdb");
            this.pluginImage = PluginBase.MainForm.FindImage("54|23|5|4");
            breakPointManager = new BreakPointManager();
            debugManager = new DebuggerManager();
            liveDataTip = new LiveDataTip();
        }

        /// <summary>
        /// Creates the required menu items
        /// </summary>
        private void CreateMenuItems()
        {
            menusHelper = new MenusHelper(pluginImage, debugManager, settingObject);
        }

        /// <summary>
        /// Creates a plugin panel for the plugin
        /// </summary>
        private void CreatePluginPanel()
        {
            panelsHelpers = new PanelsHelper(this, pluginImage);
        }

        /// <summary>
        /// Initializes the localization of the plugin
        /// </summary>
        private void InitLocalization()
        {
            pluginDesc = TextHelper.GetString("Info.Description");
        }

        /// <summary>
        /// Adds the required event handlers
        /// </summary> 
        private void AddEventHandlers()
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
            if (!File.Exists(this.settingFilename)) SaveSettings();
            else
            {
                Object obj = ObjectSerializer.Deserialize(this.settingFilename, settingObject);
                settingObject = (Settings)obj;
            }
        }

        /// <summary>
        /// Saves the plugin settings
        /// </summary>
        public void SaveSettings()
        {
            ObjectSerializer.Serialize(this.settingFilename, settingObject);
        }

        #endregion

    }

}
