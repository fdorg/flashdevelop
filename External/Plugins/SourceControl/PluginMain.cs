using System;
using System.Collections;
using System.ComponentModel;
using System.IO;
using PluginCore;
using PluginCore.Helpers;
using PluginCore.Localization;
using PluginCore.Managers;
using PluginCore.Utilities;
using ProjectManager;
using ProjectManager.Actions;
using ProjectManager.Projects;
using SourceControl.Actions;
using SourceControl.Helpers;

namespace SourceControl
{
    public class PluginMain : IPlugin
    {
        private string settingFilename;
        private bool ready;

        #region Required Properties

        /// <summary>
        /// Api level of the plugin
        /// </summary>
        public int Api => 1;

        /// <summary>
        /// Name of the plugin
        /// </summary> 
        public string Name { get; } = nameof(SourceControl);

        /// <summary>
        /// GUID of the plugin
        /// </summary>
        public string Guid { get; } = "42ac7fab-421b-1f38-a985-5735468ac489";

        /// <summary>
        /// Author of the plugin
        /// </summary> 
        public string Author { get; } = "FlashDevelop Team";

        /// <summary>
        /// Description of the plugin
        /// </summary> 
        public string Description { get; set; } = "Source Control integration for FlashDevelop.";

        /// <summary>
        /// Web address for help
        /// </summary> 
        public string Help { get; } = "www.flashdevelop.org/community/";

        /// <summary>
        /// Object that contains the settings
        /// </summary>
        [Browsable(false)]
        public object Settings => SCSettings;

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
        }
        
        /// <summary>
        /// Disposes the plugin
        /// </summary>
        public void Dispose()
        {
            ProjectWatcher.Dispose();
            SaveSettings();
        }
        
        /// <summary>
        /// Handles the incoming events
        /// </summary>
        public void HandleEvent(object sender, NotifyEvent e, HandlingPriority priority)
        {
            switch (e.Type)
            {
                case EventType.UIStarted:
                    ProjectWatcher.Init();
                    ready = true;
                    break;

                // Catches Project change event and display the active project path
                case EventType.Command:
                    if (!ready) return;
                    DataEvent de = (DataEvent) e;
                    string cmd = de.Action;
                    if (!cmd.StartsWithOrdinal("ProjectManager.")) return;
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
                                de.Handled = ProjectWatcher.HandleFileBeforeRename(de.Data as string);
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
                                de.Handled = ProjectWatcher.HandleFileRename(de.Data as string[]);
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
                                de.Handled = ProjectWatcher.HandleFileDelete(de.Data as string[], false);
                            }
                            catch (Exception ex)
                            {
                                ErrorManager.ShowError(ex);
                                de.Handled = true;
                            }
                            break;
                        case ProjectManagerEvents.FilePasted: //ProjectFileActionsEvents.FilePaste
                            //cannot distinguish between copy and cut, so assume it was copied
                            var files = de.Data as Hashtable;
                            ProjectWatcher.HandleFileCopied((string)files["fromPath"], (string)files["toPath"]);
                            break;


                        case ProjectFileActionsEvents.FileDelete:
                            try
                            {
                                de.Handled = ProjectWatcher.HandleFileDelete(de.Data as string[], true);
                            }
                            catch (Exception ex)
                            {
                                ErrorManager.ShowError(ex);
                                de.Handled = true;
                            }
                            break;
                            
                        case ProjectFileActionsEvents.FileMove: //this is never called, because CodeRefactor catches it before us
                            try
                            {
                                de.Handled = ProjectWatcher.HandleFileMove(de.Data as string[]);
                            }
                            catch (Exception ex)
                            {
                                ErrorManager.ShowError(ex);
                                de.Handled = true;
                            }
                            break;

                        case ProjectManagerEvents.FileMoved:
                            try
                            {
                                var file = de.Data as Hashtable;
                                ProjectWatcher.HandleFileMoved((string)file["fromPath"], (string)file["toPath"]);
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
                        e.Handled = ProjectWatcher.HandleFileOpen(((TextEvent) e).Value);
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
                        e.Handled = ProjectWatcher.HandleFileReload(((TextEvent) e).Value);
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
                        e.Handled = ProjectWatcher.HandleFileModifyRO(((TextEvent) e).Value);
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
                        var file = ((TextEvent) e).Value;
                        if (File.Exists(file)) e.Handled = ProjectWatcher.HandleFileNew(file);   
                    }
                    catch (Exception ex)
                    {
                        ErrorManager.ShowError(ex);
                        e.Handled = true;
                    }
                    break;
                case EventType.ApplyTheme:
                    AnnotatedDocument.ApplyTheme();
                    break;
            }
        }
        
        #endregion

        #region Custom Methods
        
        /// <summary>
        /// Acessor got the settings object
        /// </summary>
        public static Settings SCSettings { get; set; }

        /// <summary>
        /// Initializes important variables
        /// </summary>
        public void InitBasics()
        {
            var path = Path.Combine(PathHelper.DataDir, nameof(SourceControl));
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            settingFilename = Path.Combine(path, "Settings.fdb");
            Description = TextHelper.GetString("Info.Description");
        }

        /// <summary>
        /// Adds the required event handlers
        /// </summary> 
        public void AddEventHandlers()
        {
            EventManager.AddEventHandler(this, EventType.UIStarted | EventType.Command | EventType.FileModifyRO | EventType.FileOpen | EventType.FileReload | EventType.FileNew | EventType.FileTemplate | EventType.ApplyTheme);
        }

        /// <summary>
        /// Loads the plugin settings
        /// </summary>
        public void LoadSettings()
        {
            SCSettings = new Settings();
            if (!File.Exists(settingFilename)) SaveSettings();
            else SCSettings = (Settings) ObjectSerializer.Deserialize(settingFilename, SCSettings);

            #region Detect Git

            // Try to find git path from program files
            if (SCSettings.GITPath == "git.exe")
            {
                var gitPath = PathHelper.FindFromProgramFiles(@"Git\bin\git.exe");
                if (File.Exists(gitPath)) SCSettings.GITPath = gitPath;

            }
            // Try to find TortoiseProc path from program files
            if (SCSettings.TortoiseGITProcPath == "TortoiseGitProc.exe")
            {
                string torProcPath = PathHelper.FindFromProgramFiles(@"TortoiseGit\bin\TortoiseGitProc.exe");
                if (File.Exists(torProcPath)) SCSettings.TortoiseGITProcPath = torProcPath;
            }

            #endregion

            #region Detect SVN

            // Try to find svn path from: Tools/sliksvn/
            if (SCSettings.SVNPath == "svn.exe")
            {
                string svnCmdPath = @"Tools\sliksvn\bin\svn.exe";
                if (PathHelper.ResolvePath(svnCmdPath) != null) SCSettings.SVNPath = svnCmdPath;
            }
            // Try to find sliksvn path from program files
            if (SCSettings.SVNPath == "svn.exe")
            {
                string slSvnPath = PathHelper.FindFromProgramFiles(@"SlikSvn\bin\svn.exe");
                if (File.Exists(slSvnPath)) SCSettings.SVNPath = slSvnPath;
            }
            // Try to find svn from TortoiseSVN
            if (SCSettings.SVNPath == "svn.exe")
            {
                string torSvnPath = PathHelper.FindFromProgramFiles(@"TortoiseSVN\bin\svn.exe");
                if (File.Exists(torSvnPath)) SCSettings.SVNPath = torSvnPath;
            }
            // Try to find TortoiseProc path from program files
            if (SCSettings.TortoiseSVNProcPath == "TortoiseProc.exe")
            {
                string torProcPath = PathHelper.FindFromProgramFiles(@"TortoiseSVN\bin\TortoiseProc.exe");
                if (File.Exists(torProcPath)) SCSettings.TortoiseSVNProcPath = torProcPath;
            }

            #endregion

            CheckPathExists(SCSettings.SVNPath, "TortoiseSVN (svn)");
            CheckPathExists(SCSettings.TortoiseSVNProcPath, "TortoiseSVN (Proc)");
            CheckPathExists(SCSettings.GITPath, "TortoiseGit (git)");
            CheckPathExists(SCSettings.TortoiseGITProcPath, "TortoiseGIT (Proc)");
            CheckPathExists(SCSettings.HGPath, "TortoiseHG (hg)");
            CheckPathExists(SCSettings.TortoiseHGProcPath, "TortoiseHG (Proc)");
        }

        /// <summary>
        /// 
        /// </summary>
        private void CheckPathExists(string path, string name)
        {
            if (string.IsNullOrEmpty(path)) return;
            if (!Path.IsPathRooted(path)) return;
            if (File.Exists(path)) return;
            var msg = string.Format(TextHelper.GetString("FlashDevelop.Info.InvalidToolPath"), name, "SourceControl") + ":\n" + path;
            TraceManager.AddAsync(msg, -3);
        }

        /// <summary>
        /// Saves the plugin settings
        /// </summary>
        public void SaveSettings() => ObjectSerializer.Serialize(settingFilename, SCSettings);

        #endregion
    }
}