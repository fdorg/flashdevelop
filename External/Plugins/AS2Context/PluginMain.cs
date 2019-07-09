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
        private AS2Settings settingObject;
        private Context contextInstance;
        private string settingFilename;

        #region Required Properties
        
        /// <summary>
        /// Api level of the plugin
        /// </summary>
        public int Api => 1;

        /// <summary>
        /// Name of the plugin
        /// </summary>
        public string Name { get; } = nameof(AS2Context);

        /// <summary>
        /// GUID of the plugin
        /// </summary>
        public string Guid { get; } = "1f387fab-421b-42ac-a985-72a03534f731";

        /// <summary>
        /// Author of the plugin
        /// </summary>
        public string Author { get; } = "FlashDevelop Team";

        /// <summary>
        /// Description of the plugin
        /// </summary>
        public string Description { get; set; } = "ActionScript 2 context for the ASCompletion engine.";

        /// <summary>
        /// Web address for help
        /// </summary>
        public string Help { get; } = "www.flashdevelop.org/community/";

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
        }

        /// <summary>
        /// Disposes the plugin
        /// </summary>
        public void Dispose() => SaveSettings();

        /// <summary>
        /// Handles the incoming events
        /// </summary>
        public void HandleEvent(object sender, NotifyEvent e, HandlingPriority priority)
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
            var path = Path.Combine(PathHelper.DataDir, nameof(AS2Context));
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            settingFilename = Path.Combine(path, "Settings.fdb");
            Description = TextHelper.GetString("Info.Description");
        }

        /// <summary>
        /// Adds the required event handlers
        /// </summary>
        public void AddEventHandlers() => EventManager.AddEventHandler(this, EventType.UIStarted);

        /// <summary>
        /// Loads the plugin settings
        /// </summary>
        public void LoadSettings()
        {
            settingObject = new AS2Settings();
            if (!File.Exists(settingFilename)) SaveSettings();
            else
            {
                settingObject = (AS2Settings)ObjectSerializer.Deserialize(settingFilename, settingObject);
                if (settingObject.InstalledSDKs != null)
                    foreach (var sdk in settingObject.InstalledSDKs)
                        sdk.Owner = this;
            }
            if (settingObject.MMClassPath is null) settingObject.MMClassPath = FindMMClassPath();
            if (settingObject.UserClasspath is null)
            {
                settingObject.UserClasspath = settingObject.MMClassPath != null
                                            ? new[] { settingObject.MMClassPath }
                                            : new string[] {};
            }
        }

        /// <summary>
        /// Fix some settings values when the context has been created
        /// </summary>
        private void ValidateSettings()
        {
            if (settingObject.InstalledSDKs.IsNullOrEmpty())
            {
                var allSdks = new List<InstalledSDK>();
                var includedSDK = "Tools\\mtasc";
                if (Directory.Exists(PathHelper.ResolvePath(includedSDK)))
                {
                    allSdks.Add(new InstalledSDK(this) {Path = includedSDK});
                }
                var appManDir = Path.Combine(PathHelper.BaseDir, @"Data\AppMan\Archive\mtasc");
                if (Directory.Exists(appManDir))
                {
                    var versionDirs = Directory.GetDirectories(appManDir);
                    foreach (var versionDir in versionDirs)
                    {
                        if (!Directory.Exists(versionDir)) continue;
                        allSdks.Add(new InstalledSDK(this) {Path = versionDir});
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
        private void SettingObjectOnClasspathChanged() => contextInstance?.BuildClassPath();

        /// <summary>
        /// Saves the plugin settings
        /// </summary>
        public void SaveSettings() => ObjectSerializer.Serialize(settingFilename, settingObject);

        #endregion

        #region InstalledSDKOwner Membres

        public bool ValidateSDK(InstalledSDK sdk)
        {
            sdk.Owner = this;
            
            var project = PluginBase.CurrentProject;
            var path = sdk.Path;
            path = project != null
                ? PathHelper.ResolvePath(path, Path.GetDirectoryName(project.ProjectPath))
                : PathHelper.ResolvePath(path);
            
            try
            {
                if (path is null || (!Directory.Exists(path) && !File.Exists(path)))
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
            var descriptor = Path.Combine(path, "changes.txt");
            if (File.Exists(descriptor))
            {
                var text = File.ReadAllText(descriptor);
                var mVer = Regex.Match(text, "[0-9\\-]+\\s*:\\s*([0-9.]+)");
                if (mVer.Success)
                {
                    sdk.Version = mVer.Groups[1].Value;
                    sdk.Name = $"MTASC {sdk.Version}";
                    return true;
                }
                ErrorManager.ShowInfo("Invalid changes.txt file:\n" + descriptor);
            }
            else ErrorManager.ShowInfo("No changes.txt found:\n" + descriptor);
            return false;
        }

        #endregion

        #region Macromedia/Adobe Flash IDE

        // locations in Application Data
        private static readonly string[] MACROMEDIA_VERSIONS = {
            "\\Adobe\\Flash CS5\\",
            "\\Adobe\\Flash CS4\\",
            "\\Adobe\\Flash CS3\\",
            "\\Macromedia\\Flash 8\\", 
            "\\Macromedia\\Flash MX 2004\\"
        };

        /// <summary>
        /// Explore the possible locations for the Macromedia Flash IDE classpath
        /// </summary>
        private static string FindMMClassPath()
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
            return found ? cp : null;
        }
        #endregion
    }
}