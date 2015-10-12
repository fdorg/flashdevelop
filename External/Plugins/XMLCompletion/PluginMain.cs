using System;
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
        private String pluginName = "XMLCompletion";
        private String pluginGuid = "cfdd5c07-1516-4e2b-8791-a3a40eecc277";
        private String pluginHelp = "www.flashdevelop.org/community/";
        private String pluginDesc = "Provides simple HTML and XML completion.";
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
            XMLComplete.Init();
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
                    DataEvent de = (DataEvent)e;
                    if (XMLComplete.Active && !settingObject.DisableZenCoding
                        && de.Action == "SnippetManager.Expand")
                    {
                        Hashtable data = (Hashtable)de.Data;
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
            String dataPath = Path.Combine(PathHelper.DataDir, "XMLCompletion");
            if (!Directory.Exists(dataPath)) Directory.CreateDirectory(dataPath);
            this.settingFilename = Path.Combine(dataPath, "Settings.fdb");
            this.pluginDesc = TextHelper.GetString("Info.Description");
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
            this.settingObject = new Settings();
            if (!File.Exists(this.settingFilename)) this.SaveSettings();
            else
            {
                Object obj = ObjectSerializer.Deserialize(this.settingFilename, this.settingObject);
                this.settingObject = (Settings)obj;
            }
            XMLCompletion.Settings.Instance = this.settingObject;
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
