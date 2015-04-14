using System;
using System.IO;
using System.ComponentModel;
using WeifenLuo.WinFormsUI;
using PluginCore.Localization;
using PluginCore.Managers;
using PluginCore.Utilities;
using PluginCore.Helpers;
using PluginCore;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace UnityContext
{
    public class PluginMain : IPlugin, InstalledSDKOwner
    {
        private String pluginName = "UnityContext";
        private String pluginGuid = "1f387fab-421b-42ac-a985-72a03534f732";
        private String pluginHelp = "www.flashdevelop.org/community/";
        private String pluginDesc = "UnityScript context for the ASCompletion engine.";
        private String pluginAuth = "GameJam / FlashDevelop Team";
        private UnitySettings settingObject;
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
        public Object Settings
        {
            get { return this.settingObject; }
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
            // Register Unity3D project type
            ProjectManager.Helpers.ProjectCreator.AppendProjectType("project.u3dproj", typeof(UnityProject));
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
        public void HandleEvent(Object sender, NotifyEvent e, HandlingPriority prority)
        {
            switch (e.Type)
            {
                case EventType.SyntaxDetect:
                    // detect Actionscript language version
                    ITabbedDocument doc = PluginBase.MainForm.CurrentDocument;
                    if (doc == null || !doc.IsEditable) return;
                    if (doc.FileName.ToLower().EndsWith(".js"))
                    {
                        if (IsUnityProject(doc.FileName))
                        {
                            (e as TextEvent).Value = "unityscript";
                            e.Handled = true;
                        }
                    }
                    break;

                case EventType.Completion:
                    ITabbedDocument doc2 = PluginBase.MainForm.CurrentDocument;
                    if (doc2 == null || !doc2.IsEditable) return;
                    if (doc2.FileName.ToLower().EndsWith(".js"))
                    {
                        if (IsUnityProject(doc2.FileName))
                        {
                            e.Handled = true;
                        }
                    }
                    break;

                case EventType.UIStarted:
                    contextInstance = new Context(settingObject);
                    ValidateSettings();
                    // Associate this context with UnityScript language
                    ASCompletion.Context.ASContext.RegisterLanguage(contextInstance, "UnityScript");
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private bool IsUnityProject(string fileName)
        {
            if (contextInstance != null && contextInstance.Classpath != null)
            {
                foreach (ASCompletion.Model.PathModel path in contextInstance.Classpath)
                {
                    if (fileName.StartsWith(path.Path, StringComparison.OrdinalIgnoreCase)) return true;
                }
            }
            string dir = Path.GetDirectoryName(fileName);
            while (Directory.GetFiles(dir, "*.u3dproj").Length == 0)
            {
                dir = Path.GetDirectoryName(dir);
                if (dir.Length <= 3) return false;
            }
            return true;
        }

        #endregion

        #region Custom Methods

        /// <summary>
        /// Initializes important variables
        /// </summary>
        public void InitBasics()
        {
            String dataPath = Path.Combine(PathHelper.DataDir, "UnityContext");
            if (!Directory.Exists(dataPath)) Directory.CreateDirectory(dataPath);
            this.settingFilename = Path.Combine(dataPath, "Settings.fdb");
            this.pluginDesc = TextHelper.GetString("Info.Description");
        }

        /// <summary>
        /// Adds the required event handlers
        /// </summary>
        public void AddEventHandlers()
        {
            EventManager.AddEventHandler(this, EventType.UIStarted | EventType.SyntaxDetect | EventType.Completion);
        }

        /// <summary>
        /// Loads the plugin settings
        /// </summary>
        public void LoadSettings()
        {
            this.settingObject = new UnitySettings();
            if (!File.Exists(this.settingFilename)) this.SaveSettings();
            else
            {
                Object obj = ObjectSerializer.Deserialize(this.settingFilename, this.settingObject);
                this.settingObject = (UnitySettings)obj;
                if (settingObject.InstalledSDKs != null)
                {
                    foreach (InstalledSDK sdk in settingObject.InstalledSDKs)
                    {
                        sdk.Owner = this;
                    }
                }
            }
            if (this.settingObject.UserClasspath == null)
            {
                this.settingObject.UserClasspath = new String[] {};
            }
        }

        /// <summary>
        /// Fix some settings values when the context has been created
        /// </summary>
        private void ValidateSettings()
        {
            if (settingObject.InstalledSDKs == null || settingObject.InstalledSDKs.Length == 0)
            {
                /*string includedSDK = "Tools\\mtasc";
                if (Directory.Exists(PathHelper.ResolvePath(includedSDK)))
                {
                    InstalledSDK sdk = new InstalledSDK(this);
                    sdk.Path = includedSDK;
                    settingObject.InstalledSDKs = new InstalledSDK[] { sdk };
                }*/
            }
            else foreach (InstalledSDK sdk in settingObject.InstalledSDKs) ValidateSDK(sdk);
            
            settingObject.OnClasspathChanged += SettingObjectOnClasspathChanged;
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
            ObjectSerializer.Serialize(this.settingFilename, this.settingObject);
        }

        #endregion

        #region InstalledSDKOwner Membres

        public bool ValidateSDK(InstalledSDK sdk)
        {
            sdk.Owner = this;
            IProject project = PluginBase.CurrentProject;
            string path = sdk.Path;
            if (project != null) path = PathHelper.ResolvePath(path, Path.GetDirectoryName(project.ProjectPath));
            else path = PathHelper.ResolvePath(path);
            try
            {
                if (path == null || (!Directory.Exists(path) && !File.Exists(path)))
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
            if (!Directory.Exists(path)) path = Path.GetDirectoryName(path);
            string descriptor = Path.Combine(path, "Unity.exe");
            if (File.Exists(descriptor))
            {
                FileVersionInfo info = FileVersionInfo.GetVersionInfo(descriptor);
                sdk.Version = info.ProductVersion;
                sdk.Name = info.ProductName;
            }
            else ErrorManager.ShowInfo("Unity.exe found:\n" + descriptor);
            return false;
        }

        #endregion

    }

}

