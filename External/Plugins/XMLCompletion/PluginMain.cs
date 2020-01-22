using System.Collections;
using System.IO;
using PluginCore;
using PluginCore.Helpers;
using PluginCore.Localization;
using PluginCore.Managers;
using PluginCore.Utilities;

namespace XMLCompletion
{
    public class PluginMain : IPlugin
    {
        string settingFilename;
        Settings settingObject;

        #region Required Properties

        /// <summary>
        /// Api level of the plugin
        /// </summary>
        public int Api => 1;

        /// <summary>
        /// Name of the plugin
        /// </summary> 
        public string Name { get; } = nameof(XMLCompletion);

        /// <summary>
        /// GUID of the plugin
        /// </summary>
        public string Guid { get; } = "cfdd5c07-1516-4e2b-8791-a3a40eecc277";

        /// <summary>
        /// Author of the plugin
        /// </summary> 
        public string Author { get; } = "FlashDevelop Team";

        /// <summary>
        /// Description of the plugin
        /// </summary> 
        public string Description { get; set; } = "Provides simple HTML and XML completion.";

        /// <summary>
        /// Web address for help
        /// </summary> 
        public string Help { get; } = "www.flashdevelop.org/community/";

        /// <summary>
        /// Object that contains the settings
        /// </summary>
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
            XMLComplete.Init();
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
                case EventType.FileSwitch:
                case EventType.SyntaxChange:
                    XMLComplete.CurrentFile = PluginBase.MainForm.CurrentDocument.FileName;
                    break;

                case EventType.Completion:
                    if (XMLComplete.Active) e.Handled = true;
                    return;

                case EventType.Keys:
                    e.Handled = XMLComplete.OnShortCut(((KeyEvent)e).Value);
                    break;

                case EventType.Command:
                    var de = (DataEvent)e;
                    if (XMLComplete.Active && !settingObject.DisableZenCoding
                        && de.Action == "SnippetManager.Expand")
                    {
                        var data = (Hashtable)de.Data;
                        if (ZenCoding.expandSnippet(data))
                            de.Handled = true;
                    }
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
            var path = Path.Combine(PathHelper.DataDir, nameof(XMLCompletion));
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            settingFilename = Path.Combine(path, "Settings.fdb");
            Description = TextHelper.GetString("Info.Description");
        }

        /// <summary>
        /// Adds the required event handlers
        /// </summary> 
        public void AddEventHandlers()
        {
            EventType eventType = EventType.FileSwitch | EventType.SyntaxChange | EventType.Keys | EventType.Command | EventType.Completion;
            EventManager.AddEventHandler(this, eventType);
        }

        /// <summary>
        /// Loads the plugin settings
        /// </summary>
        public void LoadSettings()
        {
            settingObject = new Settings();
            if (!File.Exists(settingFilename)) SaveSettings();
            else settingObject = (Settings) ObjectSerializer.Deserialize(settingFilename, settingObject);
            XMLCompletion.Settings.Instance = settingObject;
        }

        /// <summary>
        /// Saves the plugin settings
        /// </summary>
        public void SaveSettings() => ObjectSerializer.Serialize(settingFilename, settingObject);

        #endregion

    }
}