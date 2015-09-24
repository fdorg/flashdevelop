using System;
using System.ComponentModel;
using System.IO;
using ASCompletion.Context;
using PluginCore;
using PluginCore.Helpers;
using PluginCore.Localization;
using PluginCore.Managers;
using PluginCore.Utilities;

namespace PHPContext
{
    public class PluginMain : IPlugin
    {
        private String pluginName = "PHPContext";
        private String pluginGuid = "2eecf4ad-08f5-45d7-8060-86b637e94773";
        private String pluginHelp = "www.flashdevelop.org/community/";
        private String pluginDesc = "PHP context for the ASCompletion engine.";
        private String pluginAuth = "FlashDevelop Team";
        private String languageID = "PHP"; // change also in ContextSetting class
        private String associatedSyntax = "HTML"; // ie. coloring syntax file name
        private ContextSettings settingObject;
        private String settingFilename;
        private Context contextInstance;

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
                case EventType.UIStarted :
                    contextInstance = new Context(settingObject);
                    // Associate this context with a file type
                    ASContext.RegisterLanguage(contextInstance, associatedSyntax);
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
            String dataPath = Path.Combine(PathHelper.DataDir, languageID + "Context");
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
            this.settingObject = new ContextSettings();
            if (!File.Exists(this.settingFilename)) this.SaveSettings();
            else
            {
                Object obj = ObjectSerializer.Deserialize(this.settingFilename, this.settingObject);
                this.settingObject = (ContextSettings)obj;
            }
            if (this.settingObject.LanguageDefinitions == null) this.settingObject.LanguageDefinitions = @"Library\PHP\intrinsic";
            this.settingObject.OnClasspathChanged += SettingObjectOnClasspathChanged;
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
            this.settingObject.OnClasspathChanged -= SettingObjectOnClasspathChanged;
            ObjectSerializer.Serialize(this.settingFilename, this.settingObject);
        }

        #endregion

    }

}
