using System;
using System.IO;
using System.Drawing;
using System.ComponentModel;
using PluginCore.Localization;
using PluginCore.Utilities;
using PluginCore.Managers;
using PluginCore.Helpers;
using SourceControl.Actions;
using ProjectManager.Projects;
using ProjectManager.Actions;
using ProjectManager;
using PluginCore;

namespace SourceControl
{
	public class PluginMain : IPlugin
	{
        private String pluginName = "SourceControl";
        private String pluginGuid = "42ac7fab-421b-1f38-a985-5735468ac489";
        private String pluginHelp = "www.flashdevelop.org/community/";
        private String pluginDesc = "Source Control integration for FlashDevelop.";
        private String pluginAuth = "FlashDevelop Team";
        private static Settings settingObject;
        private String settingFilename;
        private Boolean ready;

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
			get { return this.pluginGuid; }
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
            this.InitBasics();
            this.LoadSettings();
            this.AddEventHandlers();
        }
		
		/// <summary>
		/// Disposes the plugin
		/// </summary>
		public void Dispose()
		{
            ProjectWatcher.Dispose();
            this.SaveSettings();
		}
		
		/// <summary>
		/// Handles the incoming events
		/// </summary>
		public void HandleEvent(Object sender, NotifyEvent e, HandlingPriority prority)
		{
            switch (e.Type)
            {
                case EventType.UIStarted:
                    ProjectWatcher.Init();
                    this.ready = true;
                    break;

                // Catches Project change event and display the active project path
                case EventType.Command:
                    if (!this.ready) return;
                    DataEvent de = e as DataEvent;
                    String cmd = de.Action;
                    if (!cmd.StartsWith("ProjectManager.")) return;
                    switch (cmd)
                    {
                        case ProjectManagerEvents.Project:
                            ProjectWatcher.SetProject(de.Data as Project);
                            break;

                        case ProjectManagerEvents.TreeSelectionChanged:
                            ProjectWatcher.SelectionChanged();
                            break;

                        case ProjectManagerEvents.UserRefreshTree:
                            ProjectWatcher.ForceRefresh();
                            break;

                        case ProjectFileActionsEvents.FileBeforeRename:
                            try
                            {
                                de.Handled = ProjectWatcher.HandleFileBeforeRename(de.Data as String);
                            }
                            catch (Exception ex)
                            {
                                ErrorManager.ShowError(ex);
                                de.Handled = true;
                            }
                            break;

                        case ProjectFileActionsEvents.FileRename:
                            try
                            {
                                de.Handled = ProjectWatcher.HandleFileRename(de.Data as String[]);
                            }
                            catch (Exception ex)
                            {
                                ErrorManager.ShowError(ex);
                                de.Handled = true;
                            }
                            break;

                        case ProjectFileActionsEvents.FileDeleteSilent:
                            try
                            {
                                de.Handled = ProjectWatcher.HandleFileDelete(de.Data as String[], false);
                            }
                            catch (Exception ex)
                            {
                                ErrorManager.ShowError(ex);
                                de.Handled = true;
                            }
                            break;

                        case ProjectFileActionsEvents.FileDelete:
                            try
                            {
                                de.Handled = ProjectWatcher.HandleFileDelete(de.Data as String[], true);
                            }
                            catch (Exception ex)
                            {
                                ErrorManager.ShowError(ex);
                                de.Handled = true;
                            }
                            break;

                        case ProjectFileActionsEvents.FileMove:
                            try
                            {
                                de.Handled = ProjectWatcher.HandleFileMove(de.Data as String[]);
                            }
                            catch (Exception ex)
                            {
                                ErrorManager.ShowError(ex);
                                de.Handled = true;
                            }
                            break;

                        case ProjectManagerEvents.BuildProject:
                            try
                            {
                                de.Handled = ProjectWatcher.HandleBuildProject();
                            }
                            catch (Exception ex)
                            {
                                ErrorManager.ShowError(ex);
                                de.Handled = true;
                            }
                            break;

                        case ProjectManagerEvents.TestProject:
                            try
                            {
                                de.Handled = ProjectWatcher.HandleTestProject();
                            }
                            catch (Exception ex)
                            {
                                ErrorManager.ShowError(ex);
                                de.Handled = true;
                            }
                            break;

                        case ProjectManagerEvents.BeforeSave:
                            try
                            {
                                de.Handled = ProjectWatcher.HandleSaveProject((string)de.Data);
                            }
                            catch (Exception ex)
                            {
                                ErrorManager.ShowError(ex);
                                de.Handled = true;
                            }
                            break;
                    }
                    break;
                case EventType.FileOpen:
                    try
                    {
                        e.Handled = ProjectWatcher.HandleFileOpen((e as TextEvent).Value);
                    }
                    catch (Exception ex)
                    {
                        ErrorManager.ShowError(ex);
                        e.Handled = true;
                    }
                    break;
                case EventType.FileReload:
                    try
                    {
                        e.Handled = ProjectWatcher.HandleFileReload((e as TextEvent).Value);
                    }
                    catch (Exception ex)
                    {
                        ErrorManager.ShowError(ex);
                        e.Handled = true;
                    }
                    break;
                case EventType.FileModifyRO:
                    try
                    {
                        e.Handled = ProjectWatcher.HandleFileModifyRO((e as TextEvent).Value);
                    }
                    catch (Exception ex)
                    {
                        ErrorManager.ShowError(ex);
                        e.Handled = true;
                    }
                    break;
                case EventType.FileNew:
                case EventType.FileTemplate:
                    try
                    {
                        e.Handled = ProjectWatcher.HandleFileNew((e as TextEvent).Value);
                    }
                    catch (Exception ex)
                    {
                        ErrorManager.ShowError(ex);
                        e.Handled = true;
                    }
                    break;
            }
		}
		
		#endregion

        #region Custom Methods
        
        /// <summary>
        /// Acessor got the settings object
        /// </summary>
        public static Settings SCSettings 
        { 
            get { return settingObject; }
        }

        /// <summary>
        /// Initializes important variables
        /// </summary>
        public void InitBasics()
        {
            String dataPath = Path.Combine(PathHelper.DataDir, "SourceControl");
            if (!Directory.Exists(dataPath)) Directory.CreateDirectory(dataPath);
            this.settingFilename = Path.Combine(dataPath, "Settings.fdb");
            this.pluginDesc = TextHelper.GetString("Info.Description");
        }

        /// <summary>
        /// Adds the required event handlers
        /// </summary> 
        public void AddEventHandlers()
        {
            EventManager.AddEventHandler(this, EventType.UIStarted | EventType.Command | EventType.FileModifyRO | EventType.FileOpen | EventType.FileReload | EventType.FileNew | EventType.FileTemplate);
        }

        /// <summary>
        /// Loads the plugin settings
        /// </summary>
        public void LoadSettings()
        {
            settingObject = new Settings();
            if (!File.Exists(this.settingFilename)) this.SaveSettings();
            else
            {
                Object obj = ObjectSerializer.Deserialize(this.settingFilename, settingObject);
                settingObject = (Settings)obj;
            }
            // Try to find svn path from: Tools/sliksvn/
            if (settingObject.SVNPath == "svn.exe")
            {
                String svnCmdPath = @"Tools\sliksvn\bin\svn.exe";
                if (PathHelper.ResolvePath(svnCmdPath) != null) settingObject.SVNPath = svnCmdPath;
            }
            // Try to find TortoiseProc path from program files
            if (settingObject.TortoiseSVNProcPath == "TortoiseProc.exe")
            {
                String programFiles = Environment.GetEnvironmentVariable("ProgramFiles");
                String torProcPath = Path.Combine(programFiles, @"TortoiseSVN\bin\TortoiseProc.exe");
                if (File.Exists(torProcPath)) settingObject.TortoiseSVNProcPath = torProcPath;
            }
            CheckPathExists(settingObject.SVNPath, "TortoiseSVN (svn)");
            CheckPathExists(settingObject.TortoiseSVNProcPath, "TortoiseSVN (Proc)");
            CheckPathExists(settingObject.GITPath, "TortoiseGit (git)");
            CheckPathExists(settingObject.TortoiseGITProcPath, "TortoiseGIT (Proc)");
            CheckPathExists(settingObject.HGPath, "TortoiseHG (hg)");
            CheckPathExists(settingObject.TortoiseHGProcPath, "TortoiseHG (Proc)");
        }

        /// <summary>
        /// 
        /// </summary>
        private void CheckPathExists(String path, String name)
        {
            if (String.IsNullOrEmpty(path)) return;
            if (!Path.IsPathRooted(path)) return;
            if (!File.Exists(path))
            {
                String msg = String.Format(TextHelper.GetString("FlashDevelop.Info.InvalidToolPath"), name, "SourceControl") + ":\n" + path;
                TraceManager.AddAsync(msg, -3);
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
