// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
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

namespace BasicCompletion
{
    public class PluginMain : IPlugin
    {
        readonly Hashtable updateTable = new Hashtable();
        readonly Hashtable baseTable = new Hashtable();
        readonly Hashtable fileTable = new Hashtable();
        Timer updateTimer;
        bool isActive;
        bool isSupported;
        string settingFilename;
        Settings settingObject;
        string[] projKeywords;

        #region Required Properties

        /// <summary>
        /// Api level of the plugin
        /// </summary>
        public int Api => 1;

        /// <summary>
        /// Name of the plugin
        /// </summary> 
        public string Name { get; } = nameof(BasicCompletion);

        /// <summary>
        /// GUID of the plugin
        /// </summary>
        public string Guid { get; } = "c5564dec-5288-4bbb-b286-a5678536698b";

        /// <summary>
        /// Author of the plugin
        /// </summary> 
        public string Author { get; } = "FlashDevelop Team";

        /// <summary>
        /// Description of the plugin
        /// </summary> 
        public string Description { get; set; } = "Adds global basic code completion support to FlashDevelop.";

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
            InitTimer();
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
                    if (isSupported && keys == (Keys.Control | Keys.Space))
                    {
                        var lang = sci.ConfigurationLanguage;
                        var items = GetCompletionListItems(lang, sci.FileName);
                        if (!items.IsNullOrEmpty())
                        {
                            items.Sort();
                            var curPos = sci.CurrentPos - 1;
                            var word = sci.GetWordLeft(curPos, false);
                            CompletionList.Show(items, false, word);
                            e.Handled = true;
                        }
                    }
                    else if (isSupported && keys == (Keys.Control | Keys.Alt | Keys.Space))
                    {
                        PluginBase.MainForm.CallCommand("InsertSnippet", "null");
                        e.Handled = true;
                    }
                    break;
                }
                case EventType.UIStarted:
                {
                    isSupported = false;
                    isActive = true;
                    break;
                }
                case EventType.UIClosing:
                {
                    isActive = false;
                    break;
                }
                case EventType.FileSwitch:
                {
                    isSupported = false;
                    break;
                }
                case EventType.Completion:
                {
                    if (!e.Handled && isActive)
                    {
                        isSupported = true;
                        e.Handled = true;
                    }
                    HandleFile(sci);
                    break;
                }
                case EventType.SyntaxChange:
                case EventType.ApplySettings:
                {
                    HandleFile(sci);
                    break;
                }
                case EventType.FileSave:
                {
                    var te = (TextEvent) e;
                    if (te.Value == sci.FileName && isSupported) AddDocumentKeywords(sci);
                    else
                    {
                        var saveDoc = DocumentManager.FindDocument(te.Value);
                        if (saveDoc != null) updateTable[te.Value] = true;
                    }
                    break;
                }
                case EventType.Command:
                {
                    var de = (DataEvent) e;
                    if (de.Action == "ProjectManager.Project")
                    {
                        if (de.Data is IProject project) LoadProjectKeywords(project);
                    }
                    break;
                }
            }
        }

        /// <summary>
        /// Handles the completion and config for a file
        /// </summary>
        void HandleFile(ScintillaControl sci)
        {
            if (isSupported)
            {
                var language = sci.ConfigurationLanguage;
                if (!baseTable.ContainsKey(language)) AddBaseKeywords(language);
                if (!fileTable.ContainsKey(sci.FileName)) AddDocumentKeywords(sci);
                if (updateTable.ContainsKey(sci.FileName)) // Need to update after save?
                {
                    updateTable.Remove(sci.FileName);
                    AddDocumentKeywords(sci);
                }
                updateTimer.Stop();
            }
            else if (updateTable.ContainsKey(sci.FileName)) // Not supported saved, remove
            {
                updateTable.Remove(sci.FileName);
            }
        }

        #endregion

        #region Custom Methods

        /// <summary>
        /// Initializes the update timer
        /// </summary>
        public void InitTimer()
        {
            updateTimer = new Timer();
            updateTimer.SynchronizingObject = PluginBase.MainForm as Form;
            updateTimer.Elapsed += UpdateTimerElapsed;
            updateTimer.Interval = 500;
        }

        /// <summary>
        /// After the timer elapses, update doc keywords
        /// </summary>
        void UpdateTimerElapsed(object sender, ElapsedEventArgs e)
        {
            var doc = PluginBase.MainForm.CurrentDocument;
            if (doc?.SciControl is {} sci && isSupported) AddDocumentKeywords(sci);
        }

        /// <summary>
        /// Initializes important variables
        /// </summary>
        public void InitBasics()
        {
            var path = Path.Combine(PathHelper.DataDir, nameof(BasicCompletion));
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            settingFilename = Path.Combine(path, "Settings.fdb");
            Description = TextHelper.GetString("Info.Description");
        }

        /// <summary>
        /// Adds the required event handlers
        /// </summary> 
        public void AddEventHandlers()
        {
            UITools.Manager.OnCharAdded += SciControlCharAdded;
            UITools.Manager.OnTextChanged += SciControlTextChanged;
            const EventType eventTypes = EventType.Keys | EventType.FileSave | EventType.ApplySettings | EventType.SyntaxChange | EventType.FileSwitch | EventType.Command | EventType.UIStarted | EventType.UIClosing;
            EventManager.AddEventHandler(this, EventType.Completion, HandlingPriority.Low);
            EventManager.AddEventHandler(this, eventTypes);
        }

        /// <summary>
        /// Loads the plugin settings
        /// </summary>
        public void LoadSettings()
        {
            settingObject = new Settings();
            if (!File.Exists(settingFilename)) SaveSettings();
            else settingObject = (Settings) ObjectSerializer.Deserialize(settingFilename, settingObject);
        }

        /// <summary>
        /// Saves the plugin settings
        /// </summary>
        public void SaveSettings() => ObjectSerializer.Serialize(settingFilename, settingObject);

        /// <summary>
        /// Adds base keywords from config file to hashtable
        /// </summary>
        public void AddBaseKeywords(string language)
        {
            var keywords = new List<string>();
            var lang = ScintillaControl.Configuration.GetLanguage(language);
            foreach (var usekeyword in lang.usekeywords)
            {
                var kc = ScintillaControl.Configuration.GetKeywordClass(usekeyword.cls);
                if (kc?.val is null) continue;
                var entry = Regex.Replace(kc.val, @"\t|\n|\r", " ");
                var words = entry.Split(new[]{' '}, StringSplitOptions.RemoveEmptyEntries);
                foreach (var word in words)
                {
                    if (word.Length > 3 && !keywords.Contains(word) && !word.StartsWithOrdinal("\x5E"))
                    {
                        keywords.Add(word);
                    }
                }
            }
            baseTable[language] = keywords;
        }

        /// <summary>
        /// Load the current project's keywords from completion file
        /// </summary>
        public void LoadProjectKeywords(IProject project)
        {
            var projDir = Path.GetDirectoryName(project.ProjectPath);
            var complFile = Path.Combine(projDir, "COMPLETION");
            if (!File.Exists(complFile)) return;
            try
            {
                var text = File.ReadAllText(complFile);
                var matches = Regex.Matches(text, "[A-Za-z0-9_$]{2,}");
                var words = new Dictionary<int, string>();
                for (int i = 0; i < matches.Count; i++)
                {
                    var word = matches[i].Value;
                    var hash = word.GetHashCode();
                    if (words.ContainsKey(hash)) continue;
                    words.Add(hash, word);
                }
                var keywords = new string[words.Values.Count];
                words.Values.CopyTo(keywords, 0);
                projKeywords = keywords;
            }
            catch { /* No errors please... */ }
        }

        /// <summary>
        /// Adds document keywords from config file to hashtable
        /// </summary>
        public void AddDocumentKeywords(ITabbedDocument document) => AddDocumentKeywords(document.SciControl);

        /// <summary>
        /// Adds document keywords from config file to hashtable
        /// </summary>
        public void AddDocumentKeywords(ScintillaControl sci)
        {
            var textLang = sci.ConfigurationLanguage;
            var language = ScintillaControl.Configuration.GetLanguage(textLang);
            if (language.characterclass is null) return;
            var wordCharsRegex = "[" + language.characterclass.Characters + "]{2,}";
            var matches = Regex.Matches(sci.Text, wordCharsRegex);
            var words = new Dictionary<int, string>();
            for (var i = 0; i < matches.Count; i++)
            {
                var word = matches[i].Value;
                var hash = word.GetHashCode();
                if (words.ContainsKey(hash)) continue;
                words.Add(hash, word);
            }
            var keywords = new string[words.Values.Count];
            words.Values.CopyTo(keywords, 0);
            fileTable[sci.FileName] = keywords;
        }

        /// <summary>
        /// Gets the completion list items combining base and doc keywords
        /// </summary>
        public List<ICompletionListItem> GetCompletionListItems(string lang, string file)
        {
            var allWords = new List<string>();
            if (baseTable.ContainsKey(lang))
            {
                var baseWords = baseTable[lang] as List<string>;
                allWords.AddRange(baseWords);
            }
            if (fileTable.ContainsKey(file))
            {
                var fileWords = (string[]) fileTable[file];
                foreach (var it in fileWords)
                {
                    if (!allWords.Contains(it)) allWords.Add(it);
                }
            }
            if (PluginBase.CurrentProject != null && projKeywords != null)
            {
                foreach (var it in projKeywords)
                {
                    if (!allWords.Contains(it)) allWords.Add(it);
                }
            }
            return allWords.Select(it => new CompletionItem(it)).ToList<ICompletionListItem>();
        }

        /// <summary>
        /// Shows the completion list automatically after typing three chars
        /// </summary>
        void SciControlCharAdded(ScintillaControl sci, int value)
        {
            if (!isSupported || settingObject.DisableAutoCompletion) return;
            var lang = sci.ConfigurationLanguage;
            var config = ScintillaControl.Configuration.GetLanguage(lang);
            var characters = config.characterclass.Characters;
            // Do not autocomplete in word
            if (characters.Contains(sci.CurrentChar)) return;
            // Autocomplete after typing word chars only
            if (!characters.Contains((char)value)) return;
            var curWord = sci.GetWordLeft(sci.CurrentPos - 1, false);
            if (curWord is null || curWord.Length < 3) return;
            var items = GetCompletionListItems(lang, sci.FileName);
            if (items.IsNullOrEmpty()) return;
            items.Sort();
            CompletionList.Show(items, true, curWord);
            var insert = settingObject.AutoInsertType;
            if (insert == AutoInsert.Never || (insert == AutoInsert.CPP && (sci.Lexer != 3/*CPP*/ || sci.PositionIsOnComment(sci.CurrentPos)) || lang == "text"))
            {
                CompletionList.DisableAutoInsertion();
            }
        }

        /// <summary>
        /// Starts the timer for the document keywords updating
        /// </summary>
        void SciControlTextChanged(ScintillaControl sci, int position, int length, int linesAdded)
        {
            if (!isSupported) return;
            updateTimer.Stop();
            updateTimer.Interval = Math.Max(500, sci.Length / 10);
            updateTimer.Start();
        }

        #endregion

    }

    #region Extra Classes

    /// <summary>
    /// Simple completion list item
    /// </summary>
    public class CompletionItem : ICompletionListItem, IComparable, IComparable<ICompletionListItem>
    {
        public CompletionItem(string label)
        {
            Label = label;
        }

        public string Label { get; }

        public string Description => TextHelper.GetString("Info.CompletionItemDesc");

        public Bitmap Icon => (Bitmap)PluginBase.MainForm.FindImage("315");

        public string Value => Label;

        int IComparable.CompareTo(object obj)
        {
            return string.Compare(Label, (obj as ICompletionListItem).Label, true);
        }
        int IComparable<ICompletionListItem>.CompareTo(ICompletionListItem other)
        {
            return string.Compare(Label, other.Label, true);
        }
    }

    #endregion
}