using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;
using ASCompletion.Completion;
using ASCompletion.Context;
using ASCompletion.Model;
using LoomContext.Projects;
using PluginCore;
using PluginCore.Helpers;
using PluginCore.Localization;
using PluginCore.Managers;
using PluginCore.Utilities;
using ProjectManager.Helpers;
using ProjectManager.Projects;

namespace LoomContext
{
    public class PluginMain : IPlugin, InstalledSDKOwner
    {
        private String pluginName = "LoomContext";
        private String pluginGuid = "fc62c534-db6e-4c58-b901-cd0b837e61c0";
        private String pluginHelp = "www.flashdevelop.org/community/";
        private String pluginDesc = "Loom context for the ASCompletion engine.";
        private String pluginAuth = "FlashDevelop Team";
        static private LoomSettings settingObject;
        private Context contextInstance;
        private String settingFilename;

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
        Object IPlugin.Settings
        {
            get { return settingObject; }
        }

        static public LoomSettings Settings
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
            this.SaveSettings();
        }

        /// <summary>
        /// Handles the incoming events
        /// </summary>
        public void HandleEvent(Object sender, NotifyEvent e, HandlingPriority priority)
        {
            switch (e.Type)
            {
                case EventType.ProcessArgs:
                    /*TextEvent te = e as TextEvent;
                    if (te.Value.IndexOf("$(FlexSDK)") >= 0)
                    {
                        te.Value = te.Value.Replace("$(FlexSDK)", contextInstance.GetCompilerPath());
                    }*/
                    break;

                case EventType.Command:
                    DataEvent de = e as DataEvent;
                    string action = de.Action;

                    if (PluginBase.CurrentProject == null || !(PluginBase.CurrentProject is LoomProject))
                    {
                        if (action == "ProjectManager.Project") LoomHelper.Monitor(null);
                        return;
                    }

                    if (action == "ProjectManager.OpenVirtualFile")
                    {
                        if (PluginBase.CurrentProject != null && PluginBase.CurrentProject.Language == "Loom")
                            e.Handled = OpenVirtualFileModel(de.Data as String);
                    }
                    else
                    if (action == "ProjectManager.BuildingProject"
                        && (PluginBase.CurrentProject as LoomProject).OutputType == OutputType.Application)
                    {
                        de.Handled = true;
                        PluginBase.MainForm.CallCommand("SaveAllModified", null);
                        LoomHelper.Build(PluginBase.CurrentProject as LoomProject);
                    }
                    else if (action == "ProjectManager.TestingProject" 
                             && (PluginBase.CurrentProject as LoomProject).OutputType == OutputType.Application)
                    {
                        de.Handled = true;
                        PluginBase.MainForm.CallCommand("SaveAllModified", null);
                        LoomHelper.Run(PluginBase.CurrentProject as LoomProject);
                    }
                    else if (action == "ProjectManager.Project")
                    {
                        LoomHelper.Monitor(de.Data as LoomProject);
                    }
                    break;

                case EventType.UIStarted:
                    contextInstance = new Context(settingObject);
                    ValidateSettings();
                    // Associate this context with Loom language
                    ASContext.RegisterLanguage(contextInstance, "loom");
                    break;

                case EventType.FileSave:
                case EventType.FileSwitch:
                    //if (contextInstance != null) contextInstance.OnFileOperation(e); // TODO syntax check
                    break;
            }
        }

        private bool OpenVirtualFileModel(string virtualPath)
        {
            int p = virtualPath.IndexOfOrdinal("::");
            if (p < 0) return false;

            string container = virtualPath.Substring(0, p);
            string ext = Path.GetExtension(container).ToLower();
            if (ext != ".loomlib") return false;
            if (!File.Exists(container)) return false;

            string fileName = Path.Combine(container, virtualPath.Substring(p + 2).Replace('.', Path.DirectorySeparatorChar));
            PathModel path = new PathModel(container, contextInstance);

            if (path.HasFile(fileName))
            {
                FileModel model = path.GetFile(fileName);
                ASComplete.OpenVirtualFile(model);
                return true;
            }
            return false;
        }

        #endregion

        #region Custom Methods

        /// <summary>
        /// Initializes important variables
        /// </summary>
        public void InitBasics()
        {
            String dataPath = Path.Combine(PathHelper.DataDir, "LoomContext");
            if (!Directory.Exists(dataPath)) Directory.CreateDirectory(dataPath);
            this.settingFilename = Path.Combine(dataPath, "Settings.fdb");
            this.pluginDesc = TextHelper.GetString("Info.Description");
        }

        /// <summary>
        /// Adds the required event handlers
        /// </summary>
        public void AddEventHandlers()
        {
            EventManager.AddEventHandler(this, EventType.UIStarted | EventType.Command /*| EventType.FileSwitch | EventType.FileSave*/);
        }

        /// <summary>
        /// Loads the plugin settings
        /// </summary>
        public void LoadSettings()
        {
            settingObject = new LoomSettings();
            if (!File.Exists(this.settingFilename)) this.SaveSettings();
            else
            {
                Object obj = ObjectSerializer.Deserialize(this.settingFilename, settingObject);
                settingObject = (LoomSettings)obj;
            }

            ProjectCreator.AppendProjectType("project.lsproj", typeof(LoomProject));
        }

        /// <summary>
        /// Fix some settings values when the context has been created
        /// </summary>
        private void ValidateSettings()
        {
            InstalledSDK sdk;
            List<InstalledSDK> sdks = new List<InstalledSDK>();
            string usersdks = Environment.ExpandEnvironmentVariables("%USERPROFILE%\\.loom\\sdks");
            if (Directory.Exists(usersdks))
                foreach(string dir in Directory.GetDirectories(usersdks))
                {
                    InstalledSDKContext.Current = this;
                    sdk = new InstalledSDK(this);
                    sdk.Path = dir;
                    sdks.Add(sdk);
                }

            if (settingObject.InstalledSDKs != null)
            {
                char[] slashes = new char[] { '/', '\\' };
                foreach (InstalledSDK oldSdk in settingObject.InstalledSDKs)
                {
                    string oldPath = oldSdk.Path.TrimEnd(slashes);
                    foreach (InstalledSDK newSdk in sdks)
                    {
                        string newPath = newSdk.Path.TrimEnd(slashes);
                        if (newPath.Equals(oldPath, StringComparison.OrdinalIgnoreCase))
                        {
                            sdks.Remove(newSdk);
                            break;
                        }
                    }
                }
                sdks.InsertRange(0, settingObject.InstalledSDKs);
            }
            settingObject.InstalledSDKs = sdks.ToArray();
            
            settingObject.OnClasspathChanged += SettingObjectOnClasspathChanged;
            settingObject.OnInstalledSDKsChanged += settingObjectOnInstalledSDKsChanged;
        }

        /// <summary>
        /// Notify of SDK collection changes
        /// </summary>
        void settingObjectOnInstalledSDKsChanged()
        {
            if (contextInstance != null)
            {
                DataEvent de = new DataEvent(EventType.Command, "ProjectManager.InstalledSDKsChanged", "loom");
                EventManager.DispatchEvent(contextInstance, de);
                if (!de.Handled) contextInstance.BuildClassPath();
            }
        }

        /// <summary>
        /// Update the classpath if an important setting has changed
        /// </summary>
        private void SettingObjectOnClasspathChanged()
        {
            if (contextInstance != null) contextInstance.BuildClassPath();
        }

        /// <summary>
        /// Saves the plugin settings
        /// </summary>
        public void SaveSettings()
        {
            ObjectSerializer.Serialize(this.settingFilename, settingObject);
        }

        #endregion

        #region InstalledSDKOwner Membres

        public bool ValidateSDK(InstalledSDK sdk)
        {
            sdk.Owner = this;
            string path = sdk.Path;
            Match mBin = Regex.Match(path, "[/\\\\]bin$", RegexOptions.IgnoreCase);
            if (mBin.Success)
                sdk.Path = path = path.Substring(0, mBin.Index);

            IProject project = PluginBase.CurrentProject;
            if (project != null)
                path = PathHelper.ResolvePath(path, Path.GetDirectoryName(project.ProjectPath));
            else
                path = PathHelper.ResolvePath(path);

            try
            {
                if (path == null || !Directory.Exists(path))
                {
                    ErrorManager.ShowInfo("Path not found:\n" + sdk.Path);
                    return false;
                }
            }
            catch (Exception ex)
            {
                ErrorManager.ShowInfo("Invalid path (" + ex.Message + "):\n" + sdk.Path);
                return false;
            }

            string descriptor = Path.Combine(path, "MIN_CLI_VERSION");

            if (File.Exists(descriptor))
            {
                string raw = File.ReadAllText(descriptor);
                Match mVer = Regex.Match(raw, "[0-9.]+");
                if (mVer.Success)
                {
                    sdk.Name = Path.GetFileName(path);
                    sdk.Version = mVer.Value;
                    return true;
                }
                else ErrorManager.ShowInfo("Invalid SDK descriptor:\n" + descriptor);
            }
            else ErrorManager.ShowInfo("No SDK descriptor found:\n" + descriptor);
            return false;
        }

        #endregion

    }

}
