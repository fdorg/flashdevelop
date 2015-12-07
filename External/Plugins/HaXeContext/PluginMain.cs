using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using PluginCore.Localization;
using PluginCore.Helpers;
using PluginCore.Managers;
using PluginCore.Utilities;
using PluginCore;
using System.Text.RegularExpressions;

namespace HaXeContext
{
    public class PluginMain : IPlugin, InstalledSDKOwner
    {
        private String pluginName = "HaxeContext";
        private String pluginGuid = "ccf2c534-db6b-4c58-b90e-cd0b837e61c5";
        private String pluginHelp = "www.flashdevelop.org/community/";
        private String pluginDesc = "Haxe context for the ASCompletion engine.";
        private String pluginAuth = "FlashDevelop Team";
        private HaXeSettings settingObject;
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
        }

        /// <summary>
        /// Disposes the plugin
        /// </summary>
        public void Dispose()
        {
            this.SaveSettings();
            if (Context.TemporaryOutputFile != null && File.Exists(Context.TemporaryOutputFile))
            {
                File.Delete(Context.TemporaryOutputFile);
            }
        }

        /// <summary>
        /// Handles the incoming events
        /// </summary>
        public void HandleEvent(Object sender, NotifyEvent e, HandlingPriority priority)
        {
            switch (e.Type)
            {
                case EventType.Command:
                    DataEvent de = e as DataEvent;
                    if (de == null) return;
                    if (de.Action == "ProjectManager.RunCustomCommand")
                    {
                        if (ExternalToolchain.HandleProject(PluginBase.CurrentProject))
                            e.Handled = ExternalToolchain.Run(de.Data as string);
                    }
                    else if (de.Action == "ProjectManager.BuildingProject" || de.Action == "ProjectManager.TestingProject")
                    {
                        var completionHandler = contextInstance.completionModeHandler as CompletionServerCompletionHandler;
                        if (completionHandler != null && !completionHandler.IsRunning())
                            completionHandler.StartServer();
                    }
                    else if (de.Action == "ProjectManager.CleanProject")
                    {
                        var project = de.Data as IProject;
                        if (ExternalToolchain.HandleProject(project))
                            e.Handled = ExternalToolchain.Clean(project);
                    }
                    else if (de.Action == "ProjectManager.Project")
                    {
                        var project = de.Data as IProject;
                        ExternalToolchain.Monitor(project);
                    }
                    else if (de.Action == "Context.SetHaxeEnvironment")
                    {
                        contextInstance.SetHaxeEnvironment(de.Data as string);
                    }
                    break;

                case EventType.UIStarted:
                    ValidateSettings();
                    contextInstance = new Context(settingObject);
                    // Associate this context with haxe language
                    ASCompletion.Context.ASContext.RegisterLanguage(contextInstance, "haxe");
                    break;
            }
        }

        #endregion

        #region Custom Methods

        /// <summary>
        /// Initializes important variables
        /// </summary>
        public void InitBasics()
        {
            String dataPath = Path.Combine(PathHelper.DataDir, "HaXeContext");
            if (!Directory.Exists(dataPath)) Directory.CreateDirectory(dataPath);
            this.settingFilename = Path.Combine(dataPath, "Settings.fdb");
            this.pluginDesc = TextHelper.GetString("Info.Description");
        }

        /// <summary>
        /// Adds the required event handlers
        /// </summary>
        public void AddEventHandlers()
        {
            EventManager.AddEventHandler(this, EventType.UIStarted | EventType.Command);
        }

        /// <summary>
        /// Loads the plugin settings
        /// </summary>
        public void LoadSettings()
        {
            this.settingObject = new HaXeSettings();
            if (!File.Exists(this.settingFilename)) this.SaveSettings();
            else
            {
                Object obj = ObjectSerializer.Deserialize(this.settingFilename, this.settingObject);
                this.settingObject = (HaXeSettings)obj;
            }
        }

        /// <summary>
        /// Fix some settings values when the context has been created
        /// </summary>
        private void ValidateSettings()
        {
            if (settingObject.InstalledSDKs == null || settingObject.InstalledSDKs.Length == 0 || PluginBase.MainForm.RefreshConfig)
            {
                string externalSDK;
                InstalledSDK sdk;
                List<InstalledSDK> sdks = new List<InstalledSDK>();
                externalSDK = Environment.ExpandEnvironmentVariables("%HAXEPATH%");
                if (!String.IsNullOrEmpty(externalSDK) && Directory.Exists(PathHelper.ResolvePath(externalSDK)))
                {
                    InstalledSDKContext.Current = this;
                    sdk = new InstalledSDK(this);
                    sdk.Path = externalSDK;
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
            }
            else
            {
                foreach (InstalledSDK sdk in settingObject.InstalledSDKs)
                {
                    ValidateSDK(sdk);
                }
            }
            if (settingObject.CompletionServerPort == 0)
            {
                settingObject.CompletionServerPort = 6000;
                settingObject.CompletionMode = HaxeCompletionModeEnum.CompletionServer;
            }
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
        private void SaveSettings()
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
            if (project != null)
                path = PathHelper.ResolvePath(path, Path.GetDirectoryName(project.ProjectPath));
            else
                path = PathHelper.ResolvePath(path);
            
            try
            {
                if (path == null || !Directory.Exists(path))
                {
                    //ErrorManager.ShowInfo("Path not found:\n" + sdk.Path);
                    return false;
                }
            }
            catch (Exception ex)
            {
                ErrorManager.ShowInfo("Invalid path (" + ex.Message + "):\n" + sdk.Path);
                return false;
            }

            string[] lookup = new string[] {
                Path.Combine(path, "CHANGES.txt"),
                Path.Combine(path, Path.Combine("extra", "CHANGES.txt")),
                Path.Combine(path, Path.Combine("doc", "CHANGES.txt"))
            };
            string descriptor = null;
            foreach(string p in lookup) 
                if (File.Exists(p)) {
                    descriptor = p;
                    break;
                }
            if (descriptor != null)
            {
                string raw = File.ReadAllText(descriptor);
                Match mVer = Regex.Match(raw, "[0-9\\-?]+\\s*:\\s*([0-9.]+)");
                if (mVer.Success)
                {
                    sdk.Version = mVer.Groups[1].Value;
                    sdk.Name = "Haxe " + sdk.Version;
                    return true;
                }
                else ErrorManager.ShowInfo("Invalid changes.txt file:\n" + descriptor);
            }
            else if (File.Exists(Path.Combine(path, "haxe.exe"))) 
            {
                sdk.Version = "0.0";
                sdk.Name = "Haxe ?";
                return true;
            }
            else ErrorManager.ShowInfo("No change.txt found:\n" + descriptor);
            return false;
        }

        #endregion
    
    }

}
