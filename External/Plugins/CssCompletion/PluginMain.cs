using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Timers;
using System.Windows.Forms;
using PluginCore;
using PluginCore.Controls;
using PluginCore.Helpers;
using PluginCore.Localization;
using PluginCore.Managers;
using PluginCore.Utilities;
using ScintillaNet;
using Timer = System.Timers.Timer;

namespace CssCompletion
{
    public class PluginMain : IPlugin
    {
        private String pluginName = "CssCompletion";
        private String pluginGuid = "c156cdec-5c88-4bcb-b186-a5678516698c";
        private String pluginHelp = "www.flashdevelop.org/community/";
        private String pluginDesc = "Adds CSS completion to FlashDevelop.";
        private String pluginAuth = "FlashDevelop Team";
        private String settingFilename;
        private Settings settingObject;
        private Dictionary<String, CssFeatures> enabledLanguages;
        private SimpleIni config;
        private Completion completion;
        private CssFeatures features;
        private Timer updater;
        private string updateFile;
        private CssFeatures updateFeatures;

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
            ITabbedDocument document = PluginBase.MainForm.CurrentDocument;
            if (document == null || !document.IsEditable) return;
            switch (e.Type)
            {
                case EventType.Keys:
                {
                    Keys keys = (e as KeyEvent).Value;
                    if (this.IsSupported(document) && keys == (Keys.Control | Keys.Space))
                    {
                        if (completion != null)
                        {
                            completion.OnComplete(document.SciControl, document.SciControl.CurrentPos);
                            e.Handled = true;
                        }
                    }
                    break;
                }
                case EventType.FileSwitch:
                case EventType.SyntaxChange:
                case EventType.ApplySettings:
                {
                    if (document.IsEditable && this.IsSupported(document))
                    {
                        string ext = Path.GetExtension(document.FileName).ToLower();
                        features = enabledLanguages.ContainsKey(ext) ? enabledLanguages[ext] : null;
                        if (completion == null) completion = new Completion(config, settingObject);
                        completion.OnFileChanged(features);
                        if (features != null && features.Syntax != null)
                        {
                            ScintillaControl sci = document.SciControl;
                            sci.SetProperty(features.Syntax, features != null ? "1" : "0");
                            sci.Colourise(0, -1);
                        }
                    }
                    break;
                }
                case EventType.Completion:
                {
                    if (features != null) e.Handled = true;
                    return;
                }
                case EventType.FileSave:
                {
                    if (document != null && document.IsEditable && this.IsSupported(document))
                    {
                        updateFile = document.FileName;
                        updateFeatures = features;
                        updater.Start();
                    }
                    break;
                }
            }
        }

        #endregion

        #region Custom Methods

        /// <summary>
        /// Initializes important variables
        /// </summary>
        public void InitBasics()
        {
            String dataPath = Path.Combine(PathHelper.DataDir, "CssCompletion");
            if (!Directory.Exists(dataPath)) Directory.CreateDirectory(dataPath);
            this.settingFilename = Path.Combine(dataPath, "Settings.fdb");
            this.pluginDesc = TextHelper.GetString("Info.Description");

            updater = new Timer();
            updater.Interval = 100;
            updater.AutoReset = false;
            updater.SynchronizingObject = PluginBase.MainForm as Form;
            updater.Elapsed += updater_Elapsed;

            CompletionItem.TagIcon = (Bitmap)PluginBase.MainForm.FindImage("417");
            CompletionItem.PropertyIcon = (Bitmap)PluginBase.MainForm.FindImage("532");
            CompletionItem.VariableIcon = (Bitmap)PluginBase.MainForm.FindImage("5");
            CompletionItem.ValueIcon = (Bitmap)PluginBase.MainForm.FindImage("237");
            CompletionItem.PseudoIcon = (Bitmap)PluginBase.MainForm.FindImage("509");
            CompletionItem.PrefixesIcon = (Bitmap)PluginBase.MainForm.FindImage("480");
        }

        void updater_Elapsed(object sender, ElapsedEventArgs e)
        {
            Optimizer.ProcessFile(updateFile, updateFeatures, settingObject);
        }

        /// <summary>
        /// Adds the required event handlers
        /// </summary> 
        public void AddEventHandlers()
        {
            UITools.Manager.OnCharAdded += SciControlCharAdded;
            UITools.Manager.OnTextChanged += SciControlTextChanged;
            CompletionList.OnInsert += CompletionList_OnInsert;
            EventType eventTypes = EventType.Keys | EventType.FileSave | EventType.ApplySettings | EventType.SyntaxChange | EventType.FileSwitch | EventType.Completion;
            EventManager.AddEventHandler(this, eventTypes);
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

            enabledLanguages = new Dictionary<string, CssFeatures>();
            config = ConfigHelper.Parse(PathHelper.ResolvePath("tools/css/completion.ini"), false);
            foreach (var def in config)
            {
                var section = def.Value;
                if (section.ContainsKey("ext"))
                {
                    string[] exts = section["ext"].Trim().Split(',');
                    var features = new CssFeatures(def.Key, section);
                    foreach (string ext in exts)
                        enabledLanguages.Add("." + ext, features);
                }
            }
        }

        /// <summary>
        /// Saves the plugin settings
        /// </summary>
        public void SaveSettings()
        {
            ObjectSerializer.Serialize(this.settingFilename, this.settingObject);
        }

        /// <summary>
        /// Checks if the language should use basic completion 
        /// </summary>
        public Boolean IsSupported(ITabbedDocument document)
        {
            string lang = document.SciControl.ConfigurationLanguage;
            return lang == "css";
        }

        #endregion

        #region UI events

        private void CompletionList_OnInsert(ScintillaControl sender, int position, string text, char trigger, ICompletionListItem item)
        {
            if (completion != null && sender.ConfigurationLanguage == "css")
                completion.OnInsert(sender, position, text, trigger, item);
        }

        private void SciControlTextChanged(ScintillaControl sender, int position, int length, int linesAdded)
        {
            if (completion != null && sender.ConfigurationLanguage == "css")
                completion.OnTextChanged(sender, position, length, linesAdded); 
        }

        /// <summary>
        /// Shows the completion list atomaticly after typing three chars
        /// </summary>
        private void SciControlCharAdded(ScintillaControl sci, Int32 value)
        {
            if (completion != null && sci.ConfigurationLanguage == "css")
                completion.OnCharAdded(sci, sci.CurrentPos, value);
        }

        #endregion

    }

}

