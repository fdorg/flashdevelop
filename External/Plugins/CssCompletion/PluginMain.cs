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
        string settingFilename;
        Settings settingObject;
        Dictionary<string, CssFeatures> enabledLanguages;
        SimpleIni config;
        Completion completion;
        CssFeatures features;
        Timer updater;
        string updateFile;
        CssFeatures updateFeatures;

        #region Required Properties

        /// <summary>
        /// Api level of the plugin
        /// </summary>
        public int Api => 1;

        /// <summary>
        /// Name of the plugin
        /// </summary> 
        public string Name { get; } = nameof(CssCompletion);

        /// <summary>
        /// GUID of the plugin
        /// </summary>
        public string Guid { get; } = "c156cdec-5c88-4bcb-b186-a5678516698c";

        /// <summary>
        /// Author of the plugin
        /// </summary> 
        public string Author { get; } = "FlashDevelop Team";

        /// <summary>
        /// Description of the plugin
        /// </summary> 
        public string Description { get; set; } = "Adds CSS completion to FlashDevelop.";

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
            var sci = PluginBase.MainForm.CurrentDocument?.SciControl;
            if (sci is null) return;
            switch (e.Type)
            {
                case EventType.Keys:
                {
                    var keys = ((KeyEvent) e).Value;
                    if (IsSupported(sci) && keys == (Keys.Control | Keys.Space) && completion != null)
                    {
                        completion.OnComplete(sci, sci.CurrentPos);
                        e.Handled = true;
                    }
                    break;
                }
                case EventType.FileSwitch:
                case EventType.SyntaxChange:
                case EventType.ApplySettings:
                {
                    if (IsSupported(sci))
                    {
                        var ext = Path.GetExtension(sci.FileName).ToLower();
                        features = enabledLanguages.ContainsKey(ext) ? enabledLanguages[ext] : null;
                        if (completion is null) completion = new Completion(config, settingObject);
                        completion.OnFileChanged(features);
                        if (features?.Syntax != null)
                        {
                            sci.SetProperty(features.Syntax, "1");
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
                    if (IsSupported(sci))
                    {
                        updateFile = sci.FileName;
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
            var path = Path.Combine(PathHelper.DataDir, nameof(CssCompletion));
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            settingFilename = Path.Combine(path, "Settings.fdb");
            Description = TextHelper.GetString("Info.Description");

            updater = new Timer();
            updater.Interval = 100;
            updater.AutoReset = false;
            updater.SynchronizingObject = PluginBase.MainForm as Form;
            updater.Elapsed += Updater_Elapsed;

            CompletionItem.TagIcon = (Bitmap)PluginBase.MainForm.FindImage("417");
            CompletionItem.PropertyIcon = (Bitmap)PluginBase.MainForm.FindImage("532");
            CompletionItem.VariableIcon = (Bitmap)PluginBase.MainForm.FindImage("5");
            CompletionItem.ValueIcon = (Bitmap)PluginBase.MainForm.FindImage("237");
            CompletionItem.PseudoIcon = (Bitmap)PluginBase.MainForm.FindImage("509");
            CompletionItem.PrefixesIcon = (Bitmap)PluginBase.MainForm.FindImage("480");
        }

        void Updater_Elapsed(object sender, ElapsedEventArgs e) => Optimizer.ProcessFile(updateFile, updateFeatures, settingObject);

        /// <summary>
        /// Adds the required event handlers
        /// </summary> 
        public void AddEventHandlers()
        {
            UITools.Manager.OnCharAdded += SciControlCharAdded;
            UITools.Manager.OnTextChanged += SciControlTextChanged;
            CompletionList.OnInsert += CompletionList_OnInsert;
            EventManager.AddEventHandler(this, EventType.Keys | EventType.FileSave | EventType.ApplySettings | EventType.SyntaxChange | EventType.FileSwitch | EventType.Completion);
        }

        /// <summary>
        /// Loads the plugin settings
        /// </summary>
        public void LoadSettings()
        {
            settingObject = new Settings();
            if (!File.Exists(settingFilename)) SaveSettings();
            else settingObject = (Settings) ObjectSerializer.Deserialize(settingFilename, settingObject);

            enabledLanguages = new Dictionary<string, CssFeatures>();
            config = ConfigHelper.Parse(PathHelper.ResolvePath("tools/css/completion.ini"), false);
            foreach (var def in config)
            {
                var section = def.Value;
                if (!section.ContainsKey("ext")) continue;
                var exts = section["ext"].Trim().Split(',');
                var features = new CssFeatures(def.Key, section);
                foreach (var ext in exts)
                    enabledLanguages.Add("." + ext, features);
            }
        }

        /// <summary>
        /// Saves the plugin settings
        /// </summary>
        public void SaveSettings() => ObjectSerializer.Serialize(settingFilename, settingObject);

        /// <summary>
        /// Checks if the language should use basic completion 
        /// </summary>
        public bool IsSupported(ITabbedDocument document) => IsSupported(document.SciControl);

        public bool IsSupported(ScintillaControl sci) => sci.ConfigurationLanguage == "css";

        #endregion

        #region UI events

        void CompletionList_OnInsert(ScintillaControl sender, int position, string text, char trigger, ICompletionListItem item)
        {
            if (completion != null && sender.ConfigurationLanguage == "css")
                completion.OnInsert(sender, position, text, trigger, item);
        }

        void SciControlTextChanged(ScintillaControl sender, int position, int length, int linesAdded)
        {
            if (completion != null && sender.ConfigurationLanguage == "css")
                completion.OnTextChanged(sender, position, length, linesAdded); 
        }

        /// <summary>
        /// Shows the completion list atomaticly after typing three chars
        /// </summary>
        void SciControlCharAdded(ScintillaControl sci, int value)
        {
            if (completion != null && sci.ConfigurationLanguage == "css")
                completion.OnCharAdded(sci, sci.CurrentPos, value);
        }

        #endregion
    }
}