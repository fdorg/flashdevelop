using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using ASCompletion.Context;
using PluginCore;
using PluginCore.Helpers;
using PluginCore.Localization;
using PluginCore.Managers;
using PluginCore.Utilities;

namespace AS2Context
{
    public class PluginMain : IPlugin, InstalledSDKOwner
    {
        private String pluginName = "AS2Context";
        private String pluginGuid = "1f387fab-421b-42ac-a985-72a03534f731";
        private String pluginHelp = "www.flashdevelop.org/community/";
        private String pluginDesc = "ActionScript 2 context for the ASCompletion engine.";
        private String pluginAuth = "FlashDevelop Team";
        private AS2Settings settingObject;
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
        }

        /// <summary>
        /// Handles the incoming events
        /// </summary>
        public void HandleEvent(Object sender, NotifyEvent e, HandlingPriority priority)
        {
            switch (e.Type)
            {
                case EventType.UIStarted:
                    contextInstance = new Context(settingObject);
                    ValidateSettings();
                    // Associate this context with AS2 language
                    ASContext.RegisterLanguage(contextInstance, "as2");
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
            String dataPath = Path.Combine(PathHelper.DataDir, "AS2Context");
            if (!Directory.Exists(dataPath)) Directory.CreateDirectory(dataPath);
            this.settingFilename = Path.Combine(dataPath, "Settings.fdb");
            this.pluginDesc = TextHelper.GetString("Info.Description");
        }

        /// <summary>
        /// Adds the required event handlers
        /// </summary>
        public void AddEventHandlers()
        {
            EventManager.AddEventHandler(this, EventType.UIStarted);
        }

        /// <summary>
        /// Loads the plugin settings
        /// </summary>
        public void LoadSettings()
        {
            this.settingObject = new AS2Settings();
            if (!File.Exists(this.settingFilename)) this.SaveSettings();
            else
            {
                Object obj = ObjectSerializer.Deserialize(this.settingFilename, this.settingObject);
                this.settingObject = (AS2Settings)obj;
                if (settingObject.InstalledSDKs != null)
                    foreach (InstalledSDK sdk in settingObject.InstalledSDKs)
                        sdk.Owner = this;
            }
            if (this.settingObject.MMClassPath == null) this.settingObject.MMClassPath = FindMMClassPath();
            if (this.settingObject.UserClasspath == null)
            {
                if (this.settingObject.MMClassPath != null) this.settingObject.UserClasspath = new String[] { this.settingObject.MMClassPath };
                else this.settingObject.UserClasspath = new String[] {};
            }
        }

        /// <summary>
        /// Fix some settings values when the context has been created
        /// </summary>
        private void ValidateSettings()
        {
            if (settingObject.InstalledSDKs == null || settingObject.InstalledSDKs.Length == 0)
            {
                List<InstalledSDK> allSdks = new List<InstalledSDK>();
                string includedSDK = "Tools\\mtasc";
                if (Directory.Exists(PathHelper.ResolvePath(includedSDK)))
                {
                    InstalledSDK sdk = new InstalledSDK(this);
                    sdk.Path = includedSDK;
                    allSdks.Add(sdk);
                }
                string appManDir = Path.Combine(PathHelper.BaseDir, @"Data\AppMan\Archive\mtasc");
                if (Directory.Exists(appManDir))
                {
                    string[] versionDirs = Directory.GetDirectories(appManDir);
                    foreach (string versionDir in versionDirs)
                    {
                        if (Directory.Exists(versionDir))
                        {
                            InstalledSDK sdk = new InstalledSDK(this);
                            sdk.Path = versionDir;
                            allSdks.Add(sdk);
                        }
                    }
                }
                settingObject.InstalledSDKs = allSdks.ToArray();
            }
            else
            {
                foreach (InstalledSDK sdk in settingObject.InstalledSDKs)
                {
                    sdk.Validate();
                }
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
            if (project != null)
                path = PathHelper.ResolvePath(path, Path.GetDirectoryName(project.ProjectPath));
            else
                path = PathHelper.ResolvePath(path);
            
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
            string descriptor = Path.Combine(path, "changes.txt");
            if (File.Exists(descriptor))
            {
                string raw = File.ReadAllText(descriptor);
                Match mVer = Regex.Match(raw, "[0-9\\-]+\\s*:\\s*([0-9.]+)");
                if (mVer.Success)
                {
                    sdk.Version = mVer.Groups[1].Value;
                    sdk.Name = "MTASC " + sdk.Version;
                    return true;
                }
                else ErrorManager.ShowInfo("Invalid changes.txt file:\n" + descriptor);
            }
            else ErrorManager.ShowInfo("No changes.txt found:\n" + descriptor);
            return false;
        }

        #endregion

        #region Macromedia/Adobe Flash IDE

        // locations in Application Data
        static readonly private string[] MACROMEDIA_VERSIONS = {
            "\\Adobe\\Flash CS5\\",
            "\\Adobe\\Flash CS4\\",
            "\\Adobe\\Flash CS3\\",
            "\\Macromedia\\Flash 8\\", 
            "\\Macromedia\\Flash MX 2004\\"
        };

        /// <summary>
        /// Explore the possible locations for the Macromedia Flash IDE classpath
        /// </summary>
        static private string FindMMClassPath()
        {
            bool found = false;
            string deflang = CultureInfo.CurrentUICulture.Name;
            deflang = deflang.Substring(0, 2);
            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string cp = "";
            foreach (string path in MACROMEDIA_VERSIONS)
            {
                cp = localAppData + path;
                // default language
                if (Directory.Exists(cp + deflang + "\\Configuration\\Classes\\"))
                {
                    cp += deflang + "\\Configuration\\Classes\\";
                    found = true;
                }
                // look for other languages
                else if (Directory.Exists(cp))
                {
                    string[] dirs = Directory.GetDirectories(cp);
                    foreach (string dir in dirs)
                    {
                        if (Directory.Exists(dir + "\\Configuration\\Classes\\"))
                        {
                            cp = dir + "\\Configuration\\Classes\\";
                            found = true;
                            break;
                        }
                    }
                }
                if (found) break;
            }
            if (found) return cp;
            else return null;
        }
        #endregion

    }

}

