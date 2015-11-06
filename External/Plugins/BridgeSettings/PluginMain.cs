using System;
using System.ComponentModel;
using System.IO;
using PluginCore;
using PluginCore.Bridge;
using PluginCore.Helpers;
using PluginCore.Localization;
using PluginCore.Utilities;

namespace BridgeSettings
{
    public class PluginMain: IPlugin
    {
        private String pluginName = "BridgeSettings";
        private String pluginGuid = "30727dde-3e23-4fb8-a8fb-9feb958402fc";
        private String pluginHelp = "www.flashdevelop.org/community/";
        private String pluginDesc = "Adds virtualization bridge configuration to FlashDevelop.";
        private String pluginAuth = "FlashDevelop Team";
        private String settingFilename;
        private Settings settingObject;

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
            BridgeManager.Settings = settingObject;
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
            // Nothing to do here..
        }

        #endregion

        #region Custom methods

        /// <summary>
        /// Sets up the basic stuff
        /// </summary> 
        private void InitBasics()
        {
            String dataPath = Path.Combine(PathHelper.DataDir, "BridgeSettings");
            if (!Directory.Exists(dataPath)) Directory.CreateDirectory(dataPath);
            this.settingFilename = Path.Combine(dataPath, "Settings.fdb");
            this.pluginDesc = TextHelper.GetString("Info.Description");
        }

        /// <summary>
        /// Loads the plugin settings
        /// </summary>
        public void LoadSettings()
        {
            this.settingObject = new Settings();
            if (!File.Exists(this.settingFilename)) this.SaveSettings();
            else
            {
                Object obj = ObjectSerializer.Deserialize(this.settingFilename, this.settingObject);
                this.settingObject = (Settings)obj;
            }
        }

        /// <summary>
        /// Saves the plugin settings
        /// </summary>
        public void SaveSettings()
        {
            ObjectSerializer.Serialize(this.settingFilename, this.settingObject);
        }

        #endregion

    }

}
