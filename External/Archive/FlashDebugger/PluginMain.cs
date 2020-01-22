// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.IO;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using ASCompletion.Context;
using PluginCore.Localization;
using PluginCore.Helpers;
using PluginCore.Managers;
using PluginCore.Utilities;
using ProjectManager.Projects;
using ProjectManager.Projects.AS3;
using PluginCore;

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
        static internal Boolean debugBuildStart;
        private Boolean buildCmpFlg = false;

	    #region Required Properties

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
			get { return PanelsHelper.pluginGuid; }
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
		public void HandleEvent(Object sender, NotifyEvent e, HandlingPriority prority)
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

                case EventType.ProcessEnd:
                    TextEvent textevnt = (TextEvent)e;
                    if (buildCmpFlg && textevnt.Value != "Done(0)")
                    {
                        buildCmpFlg = false;
						menusHelper.UpdateMenuState(this, DebuggerState.Initializing);
                    }
                    break;

                case EventType.Command:
                    PluginCore.DataEvent buildevnt = (PluginCore.DataEvent)e;
                    if (buildevnt.Action == "AS3Context.StartDebugger")
                    {
                        if (settingObject.StartDebuggerOnTestMovie)
                        {
                            if (debugManager.Start(false)) buildevnt.Handled = true;
                        }
                        return;
                    }
                    if (!buildevnt.Action.StartsWith("ProjectManager")) return;
                    if (buildevnt.Action == ProjectManager.ProjectManagerEvents.Project)
                    {
                        IProject project = PluginBase.CurrentProject;
                        if (project != null && project is AS3Project)
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
                    if (debugBuildStart && buildevnt.Action == ProjectManager.ProjectManagerEvents.BuildFailed)
                    {
                        debugBuildStart = false;
                        buildCmpFlg = false;
                        menusHelper.UpdateMenuState(this, DebuggerState.Initializing);
                    }
                    else if (buildevnt.Action == ProjectManager.ProjectManagerEvents.TestProject)
                    {
                        if (debugManager.FlashInterface.isDebuggerStarted)
                        {
                            if (debugManager.FlashInterface.isDebuggerSuspended)
                            {
                                debugManager.Continue_Click(null, null);
                            }
                            e.Handled = true;
                            return;
                        }
                        debugBuildStart = false;
                        buildCmpFlg = false;
                        menusHelper.UpdateMenuState(this, DebuggerState.Initializing);
                    }
                    else if (debugBuildStart && buildevnt.Action == ProjectManager.ProjectManagerEvents.BuildProject && buildevnt.Data.ToString() == "Debug")
                    {
                        buildCmpFlg = true;
                    }
                    else if (buildevnt.Action == ProjectManager.ProjectManagerEvents.BuildFailed)
                    {
                        menusHelper.OnBuildFailed();
                        debugBuildStart = false;
                        buildCmpFlg = false;
                    }
                    else if (buildCmpFlg && buildevnt.Action == ProjectManager.ProjectManagerEvents.BuildComplete)
                    {
                        if (buildCmpFlg)
                        {
                            debugManager.Start(debugManager.currentProject.OutputPathAbsolute);
                        }
                        else menusHelper.OnBuildComplete();
                        debugBuildStart = false;
                        buildCmpFlg = false;
                        menusHelper.UpdateMenuState(this, DebuggerState.Stopped);
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
            EventManager.AddEventHandler(this, EventType.FileEmpty | EventType.FileOpen | EventType.ProcessStart | EventType.ProcessEnd | EventType.Command | EventType.UIClosing | EventType.ApplySettings);
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
