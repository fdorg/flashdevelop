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
        private string settingFilename;
        private Settings settingObject;

        #region Required Properties
        
        /// <summary>
        /// Api level of the plugin
        /// </summary>
        public int Api => 1;

        /// <summary>
        /// Name of the plugin
        /// </summary>
        public string Name { get; } = "BridgeSettings";

        /// <summary>
        /// GUID of the plugin
        /// </summary>
        public string Guid { get; } = "30727dde-3e23-4fb8-a8fb-9feb958402fc";

        /// <summary>
        /// Author of the plugin
        /// </summary>
        public string Author { get; } = "FlashDevelop Team";

        /// <summary>
        /// Description of the plugin
        /// </summary>
        public string Description { get; set; } = "Adds virtualization bridge configuration to FlashDevelop.";

        /// <summary>
        /// Web address for help
        /// </summary>
        public string Help { get; } = "www.flashdevelop.org/community/";

        /// <summary>
        /// Object that contains the settings
        /// </summary>
        [Browsable(false)]
        public object Settings => this.settingObject;

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
        public void HandleEvent(object sender, NotifyEvent e, HandlingPriority priority)
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
            string dataPath = Path.Combine(PathHelper.DataDir, "BridgeSettings");
            if (!Directory.Exists(dataPath)) Directory.CreateDirectory(dataPath);
            this.settingFilename = Path.Combine(dataPath, "Settings.fdb");
            this.Description = TextHelper.GetString("Info.Description");
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
                object obj = ObjectSerializer.Deserialize(this.settingFilename, this.settingObject);
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
