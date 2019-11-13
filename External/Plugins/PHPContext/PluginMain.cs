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
        private readonly string associatedSyntax = "HTML"; // ie. coloring syntax file name
        private ContextSettings settingObject;
        private string settingFilename;
        private Context contextInstance;

        #region Required Properties

        /// <summary>
        /// Api level of the plugin
        /// </summary>
        public int Api => 1;

        /// <summary>
        /// Name of the plugin
        /// </summary>
        public string Name { get; } = nameof(PHPContext);

        /// <summary>
        /// GUID of the plugin
        /// </summary>
        public string Guid { get; } = "2eecf4ad-08f5-45d7-8060-86b637e94773";

        /// <summary>
        /// Author of the plugin
        /// </summary>
        public string Author { get; } = "FlashDevelop Team";

        /// <summary>
        /// Description of the plugin
        /// </summary>
        public string Description { get; set; } = "PHP context for the ASCompletion engine.";

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
            var path = Path.Combine(PathHelper.DataDir, nameof(PHPContext));
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
            settingObject = new ContextSettings();
            if (!File.Exists(settingFilename)) SaveSettings();
            else settingObject = (ContextSettings) ObjectSerializer.Deserialize(settingFilename, settingObject);
            if (settingObject.LanguageDefinitions is null) settingObject.LanguageDefinitions = @"Library\PHP\intrinsic";
            settingObject.OnClasspathChanged += SettingObjectOnClasspathChanged;
        }

        /// <summary>
        /// Update the classpath if an important setting has changed
        /// </summary>
        private void SettingObjectOnClasspathChanged() => contextInstance?.BuildClassPath();

        /// <summary>
        /// Saves the plugin settings
        /// </summary>
        public void SaveSettings()
        {
            settingObject.OnClasspathChanged -= SettingObjectOnClasspathChanged;
            ObjectSerializer.Serialize(settingFilename, settingObject);
        }

        #endregion

    }

}
