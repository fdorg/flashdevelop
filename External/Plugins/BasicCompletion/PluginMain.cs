using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
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
using ScintillaNet.Configuration;
using Timer = System.Timers.Timer;

namespace BasicCompletion
{
    public class PluginMain : IPlugin
    {
        private readonly Hashtable updateTable = new Hashtable();
        private readonly Hashtable baseTable = new Hashtable();
        private readonly Hashtable fileTable = new Hashtable();
        private Timer updateTimer;
        private bool isActive;
        private bool isSupported;
        private string settingFilename;
        private Settings settingObject;
        private string[] projKeywords;

        #region Required Properties

        /// <summary>
        /// Api level of the plugin
        /// </summary>
        public int Api => 1;

        /// <summary>
        /// Name of the plugin
        /// </summary> 
        public string Name { get; } = "BasicCompletion";

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
            this.InitTimer();
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
        public void HandleEvent(object sender, NotifyEvent e, HandlingPriority priority)
        {
            ITabbedDocument document = PluginBase.MainForm.CurrentDocument;
            if (document == null || !document.IsEditable) return;
            switch (e.Type)
            {
                case EventType.Keys:
                {
                    Keys keys = (e as KeyEvent).Value;
                    if (this.isSupported && keys == (Keys.Control | Keys.Space))
                    {
                        string lang = document.SciControl.ConfigurationLanguage;
                        List<ICompletionListItem> items = this.GetCompletionListItems(lang, document.FileName);
                        if (items != null && items.Count > 0)
                        {
                            items.Sort();
                            int curPos = document.SciControl.CurrentPos - 1;
                            string curWord = document.SciControl.GetWordLeft(curPos, false);
                            if (curWord == null) curWord = string.Empty;
                            CompletionList.Show(items, false, curWord);
                            e.Handled = true;
                        }
                    }
                    else if (this.isSupported && keys == (Keys.Control | Keys.Alt | Keys.Space))
                    {
                        PluginBase.MainForm.CallCommand("InsertSnippet", "null");
                        e.Handled = true;
                    }
                    break;
                }
                case EventType.UIStarted:
                {
                    this.isSupported = false;
                    this.isActive = true;
                    break;
                }
                case EventType.UIClosing:
                {
                    isActive = false;
                    break;
                }
                case EventType.FileSwitch:
                {
                    this.isSupported = false;
                    break;
                }
                case EventType.Completion:
                {
                    if (!e.Handled && isActive)
                    {
                        this.isSupported = true;
                        e.Handled = true;
                    }
                    this.HandleFile(document);
                    break;
                }
                case EventType.SyntaxChange:
                case EventType.ApplySettings:
                {
                    this.HandleFile(document);
                    break;
                }
                case EventType.FileSave:
                {
                    TextEvent te = e as TextEvent;
                    if (te.Value == document.FileName && this.isSupported) this.AddDocumentKeywords(document);
                    else
                    {
                        ITabbedDocument saveDoc = DocumentManager.FindDocument(te.Value);
                        if (saveDoc != null) this.updateTable[te.Value] = true;
                    }
                    break;
                }
                case EventType.Command:
                {
                    DataEvent de = e as DataEvent;
                    if (de.Action == "ProjectManager.Project")
                    {
                        IProject project = de.Data as IProject;
                        if (project != null) this.LoadProjectKeywords(project);
                    }
                    break;
                }
            }
        }

        /// <summary>
        /// Handles the completion and config for a file
        /// </summary>
        private void HandleFile(ITabbedDocument document)
        {
            if (this.isSupported)
            {
                string language = document.SciControl.ConfigurationLanguage;
                if (!this.baseTable.ContainsKey(language)) this.AddBaseKeywords(language);
                if (!this.fileTable.ContainsKey(document.FileName)) this.AddDocumentKeywords(document);
                if (this.updateTable.ContainsKey(document.FileName)) // Need to update after save?
                {
                    this.updateTable.Remove(document.FileName);
                    this.AddDocumentKeywords(document);
                }
                this.updateTimer.Stop();
            }
            else if (this.updateTable.ContainsKey(document.FileName)) // Not supported saved, remove
            {
                this.updateTable.Remove(document.FileName);
            }
        }

        #endregion

        #region Custom Methods

        /// <summary>
        /// Initializes the update timer
        /// </summary>
        public void InitTimer()
        {
            this.updateTimer = new Timer();
            this.updateTimer.SynchronizingObject = PluginBase.MainForm as Form;
            this.updateTimer.Elapsed += this.UpdateTimerElapsed;
            this.updateTimer.Interval = 500;
        }

        /// <summary>
        /// After the timer elapses, update doc keywords
        /// </summary>
        private void UpdateTimerElapsed(object sender, ElapsedEventArgs e)
        {
            ITabbedDocument doc = PluginBase.MainForm.CurrentDocument;
            if (doc != null && doc.IsEditable && this.isSupported)
            {
                this.AddDocumentKeywords(doc);
            }
        }

        /// <summary>
        /// Initializes important variables
        /// </summary>
        public void InitBasics()
        {
            string dataPath = Path.Combine(PathHelper.DataDir, "BasicCompletion");
            if (!Directory.Exists(dataPath)) Directory.CreateDirectory(dataPath);
            this.settingFilename = Path.Combine(dataPath, "Settings.fdb");
            this.Description = TextHelper.GetString("Info.Description");
        }

        /// <summary>
        /// Adds the required event handlers
        /// </summary> 
        public void AddEventHandlers()
        {
            UITools.Manager.OnCharAdded += this.SciControlCharAdded;
            UITools.Manager.OnTextChanged += this.SciControlTextChanged;
            EventType eventTypes = EventType.Keys | EventType.FileSave | EventType.ApplySettings | EventType.SyntaxChange | EventType.FileSwitch | EventType.Command | EventType.UIStarted | EventType.UIClosing;
            EventManager.AddEventHandler(this, EventType.Completion, HandlingPriority.Low);
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

        /// <summary>
        /// Adds base keywords from config file to hashtable
        /// </summary>
        public void AddBaseKeywords(string language)
        {
            List<string> keywords = new List<string>();
            Language lang = ScintillaControl.Configuration.GetLanguage(language);
            for (int i = 0; i < lang.usekeywords.Length; i++)
            {
                UseKeyword usekeyword = lang.usekeywords[i];
                var kc = ScintillaControl.Configuration.GetKeywordClass(usekeyword.cls);
                if (kc?.val != null)
                {
                    string entry = Regex.Replace(kc.val, @"\t|\n|\r", " ");
                    string[] words = entry.Split(new char[]{' '}, StringSplitOptions.RemoveEmptyEntries);
                    for (int j = 0; j < words.Length; j++)
                    {
                        if (words[j].Length > 3 && !keywords.Contains(words[j]) && !words[j].StartsWithOrdinal("\x5E"))
                        {
                            keywords.Add(words[j]);
                        }
                    }
                }
            }
            this.baseTable[language] = keywords;
        }

        /// <summary>
        /// Load the current project's keywords from completion file
        /// </summary>
        public void LoadProjectKeywords(IProject project)
        {
            string projDir = Path.GetDirectoryName(project.ProjectPath);
            string complFile = Path.Combine(projDir, "COMPLETION");
            if (File.Exists(complFile))
            {
                try
                {
                    string text = File.ReadAllText(complFile);
                    string wordCharsRegex = "[A-Za-z0-9_$]{2,}";
                    MatchCollection matches = Regex.Matches(text, wordCharsRegex);
                    Dictionary<int, string> words = new Dictionary<int, string>();
                    for (int i = 0; i < matches.Count; i++)
                    {
                        string word = matches[i].Value;
                        int hash = word.GetHashCode();
                        if (words.ContainsKey(hash)) continue;
                        words.Add(hash, word);
                    }
                    string[] keywords = new string[words.Values.Count];
                    words.Values.CopyTo(keywords, 0);
                    this.projKeywords = keywords;
                }
                catch { /* No errors please... */ }
            }
        }

        /// <summary>
        /// Adds document keywords from config file to hashtable
        /// </summary>
        public void AddDocumentKeywords(ITabbedDocument document)
        {
            string textLang = document.SciControl.ConfigurationLanguage;
            Language language = ScintillaControl.Configuration.GetLanguage(textLang);
            if (language.characterclass != null)
            {
                string wordCharsRegex = "[" + language.characterclass.Characters + "]{2,}";
                MatchCollection matches = Regex.Matches(document.SciControl.Text, wordCharsRegex);
                Dictionary<int, string> words = new Dictionary<int, string>();
                for (int i = 0; i < matches.Count; i++)
                {
                    string word = matches[i].Value;
                    int hash = word.GetHashCode();
                    if (words.ContainsKey(hash)) continue;
                    words.Add(hash, word);
                }
                string[] keywords = new string[words.Values.Count];
                words.Values.CopyTo(keywords, 0);
                this.fileTable[document.FileName] = keywords;
            }
        }

        /// <summary>
        /// Gets the completion list items combining base and doc keywords
        /// </summary>
        public List<ICompletionListItem> GetCompletionListItems(string lang, string file)
        {
            List<string> allWords = new List<string>();
            if (this.baseTable.ContainsKey(lang))
            {
                List<string> baseWords = this.baseTable[lang] as List<string>;
                allWords.AddRange(baseWords);
            }
            if (this.fileTable.ContainsKey(file))
            {
                string[] fileWords = this.fileTable[file] as string[];
                for (int i = 0; i < fileWords.Length; i++)
                {
                    if (!allWords.Contains(fileWords[i])) allWords.Add(fileWords[i]);
                }
            }
            if (PluginBase.CurrentProject != null && this.projKeywords != null)
            {
                for (int i = 0; i < this.projKeywords.Length; i++)
                {
                    if (!allWords.Contains(this.projKeywords[i])) allWords.Add(this.projKeywords[i]);
                }
            }
            List<ICompletionListItem> items = new List<ICompletionListItem>();
            for (int j = 0; j < allWords.Count; j++) items.Add(new CompletionItem(allWords[j]));
            return items;
        }

        /// <summary>
        /// Shows the completion list automatically after typing three chars
        /// </summary>
        private void SciControlCharAdded(ScintillaControl sci, int value)
        {
            if (this.isSupported && !settingObject.DisableAutoCompletion)
            {
                string lang = sci.ConfigurationLanguage;
                AutoInsert insert = settingObject.AutoInsertType;
                Language config = ScintillaControl.Configuration.GetLanguage(lang);
                string characters = config.characterclass.Characters;
                // Do not autocomplete in word
                char c = sci.CurrentChar;
                if (characters.Contains(c)) return;
                // Autocomplete after typing word chars only
                if (!characters.Contains((char)value)) return;
                string curWord = sci.GetWordLeft(sci.CurrentPos - 1, false);
                if (curWord == null || curWord.Length < 3) return;
                List<ICompletionListItem> items = this.GetCompletionListItems(lang, sci.FileName);
                if (items != null && items.Count > 0)
                {
                    items.Sort();
                    CompletionList.Show(items, true, curWord);
                    if (insert == AutoInsert.Never || (insert == AutoInsert.CPP && (sci.Lexer != 3/*CPP*/ || sci.PositionIsOnComment(sci.CurrentPos)) || lang == "text"))
                    {
                        CompletionList.DisableAutoInsertion();
                    }
                }
            }
        }

        /// <summary>
        /// Starts the timer for the document keywords updating
        /// </summary>
        private void SciControlTextChanged(ScintillaControl sci, int position, int length, int linesAdded)
        {
            if (this.isSupported)
            {
                this.updateTimer.Stop();
                this.updateTimer.Interval = Math.Max(500, sci.Length / 10);
                this.updateTimer.Start();
            }
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

