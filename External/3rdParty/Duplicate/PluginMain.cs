using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using WeifenLuo.WinFormsUI.Docking;
using PluginCore.Localization;
using PluginCore.Utilities;
using PluginCore.Managers;
using PluginCore.Helpers;
using PluginCore;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace Duplicate
{
	public class PluginMain : IPlugin
	{
        static readonly Regex RegIncNum = new Regex(@"[0-9]{1,}", RegexOptions.Compiled);

        private String pluginName = "Duplicate";
        private String pluginGuid = "b35c17ba-6d92-4f9b-84e8-5c288ca5e41d";
        private String pluginHelp = "www.flashdevelop.org/community/";
        private String pluginDesc = "Duplicate Plugin For FlashDevelop 4 - version 0.4";
        private String pluginAuth = "Jerome Decoster";
        private String settingFilename;
        private Settings settingObject;

        private string SEPAR = ">";
        private Regex RegexEvents;
        private Regex RegexProperties;
        private List<String> eventsKeys;
        private List<String> eventsValues;
        private List<String> propertiesKeys;
        private List<String> propertiesValues;
        private List<String> wordsKeys;
        private List<String> wordsValues;

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
            this.InitPlugin();
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
		public void HandleEvent(Object sender, NotifyEvent e, HandlingPriority prority)
		{
            switch (e.Type)
            {
                case EventType.Keys:
                    Keys key = (e as KeyEvent).Value;
                    if (key == this.settingObject.IncreaseShortcut) DuplicateIncrease();
                    else if (key == this.settingObject.SwitchShortcut) DuplicateSwitch();
                    else if (key == this.settingObject.WordShortcut) WordSwitch();
                    break;
            }
		}
		
		#endregion

        #region Custom Methods

        private void InitializeEvents()
        {
            if (RegexEvents != null) return;

            List<String> args = settingObject.EventsList;
            eventsKeys = new List<String>(args.Count);
            eventsValues = new List<String>(args.Count);

            string s = "";
            if (args.Count > 0)
            {
                char c = Convert.ToChar(SEPAR);
                string pref = "((?<![_a-zA-Z0-9])";
                string suff = "(?![_a-zA-Z0-9]))";
                string[] tmp = args[0].Split(c);
                s += pref + tmp[0] + suff;
                eventsKeys.Add(tmp[0]);
                eventsValues.Add(tmp[1]);
                
                for (int i = 1; i < args.Count; i++)
                {
                    tmp = args[i].Split(c);
                    s += "|" + pref + tmp[0] + suff;
                    eventsKeys.Add(tmp[0]);
                    eventsValues.Add(tmp[1]);
                }
            }
            //TraceManager.Add(s);
            RegexEvents = new Regex(s, RegexOptions.Compiled);
        }

        private void InitializeProperties()
        {
            if (RegexProperties != null) return;

            List<String> args = settingObject.PropertiesList;
            propertiesKeys = new List<String>();
            propertiesValues = new List<String>();
            
            string s = "";
            if (args.Count > 0)
            {
                char c = Convert.ToChar(SEPAR);
                string[] tmp;

                for (int i = 0; i < args.Count; i++)
                {
                    tmp = args[i].Split(c);
                    propertiesKeys.Add(tmp[0]);
                    propertiesValues.Add(tmp[1]);
                    propertiesKeys.Add(UpperCaseFirst(tmp[0]));
                    propertiesValues.Add(UpperCaseFirst(tmp[1]));
                }

                s += PropertyPrefixe(propertiesKeys[0]) + propertiesKeys[0] + PropertySuffixe(propertiesKeys[0]);
                for (int i = 1; i < propertiesKeys.Count; i++)
                    s += "|" + PropertyPrefixe(propertiesKeys[i]) + propertiesKeys[i] + PropertySuffixe(propertiesKeys[i]);

            }
            //TraceManager.Add(s);
            RegexProperties = new Regex(s, RegexOptions.Compiled);
        }

        private void InitializeWords()
        {
            if (wordsKeys != null) return;

            List<String> args = settingObject.WordsList;
            wordsKeys = new List<String>(args.Count);
            wordsValues = new List<String>(args.Count);
            
            char c = Convert.ToChar(SEPAR);

            for (int i = 0; i < args.Count; i++)
            {
                string[] tmp = args[i].Split(c);
                wordsKeys.Add(tmp[0]);
                wordsValues.Add(tmp[1]);
            }
        }

        public static string UpperCaseFirst(string text)
        {
            return text.Substring(0, 1).ToUpper() + text.Substring(1);
        }

        private string PropertyPrefixe(string property)
        {
            if (Regex.IsMatch(property.Substring(0,1), @"[A-Z]"))
                return "((?<![A-Z])";

            return "((?<![a-z])";
        }

        private string PropertySuffixe(string property)
        {
            if (Regex.IsMatch(property.Substring(property.Length - 1), @"[A-Z]"))
                return "(?![A-Z]))";

            return "(?![a-z]))";
        }

        public void WordSwitch()
        {
            if (!PluginBase.MainForm.CurrentDocument.IsEditable) return;

            ScintillaNet.ScintillaControl sci = PluginBase.MainForm.CurrentDocument.SciControl;

            sci.BeginUndoAction();

            InitializeWords();

            int pos = sci.CurrentPos;
            string word = sci.GetWordFromPosition(pos);
            string switchWord = SwitchValue(wordsKeys, wordsValues, word);

            if (word != switchWord)
            {
                int ws = sci.WordStartPosition(pos, true);
                int we = sci.WordEndPosition(pos, true);
                int ss = sci.SelectionStart;
                int se = sci.SelectionEnd;

                sci.SetSel(ws, we);
                sci.ReplaceSel(switchWord);

                float ratio = ((float)pos - (float)ws) / (float)word.Length;
                int pos2 = ws + Convert.ToInt16(Math.Round((float)switchWord.Length * ratio));

                sci.SetSel(pos2, pos2);
            }

            sci.EndUndoAction();
        }

        public void DuplicateSwitch()
        {
            if (!PluginBase.MainForm.CurrentDocument.IsEditable) return;

            ScintillaNet.ScintillaControl sci = PluginBase.MainForm.CurrentDocument.SciControl;

            sci.BeginUndoAction();
            
            int pos = sci.CurrentPos;
            int line = sci.LineFromPosition(pos);
            int start = sci.PositionFromLine(line);

            if (IsMultilineSelection(sci)) sci.SetSel(start, sci.SelectionEnd);

            int ss = sci.SelectionStart - start;
            int se = sci.SelectionEnd - start;
            
            // sci.GetLine est bugge sur la derniere ligne dans la beta 7
            string text = "";
            if (line < sci.LineCount - 1)
            {
                text = sci.GetLine(line);
                if (sci.EOLMode == 0)
                    text = text.Substring(0, text.Length - 1);
            }
            else
            {
                text = sci.GetCurLine(sci.LineLength(line));
            }

            // 0.3
            string s = "";
            if (text.IndexOf("addEventListener") > -1 || text.IndexOf("removeEventListener") > -1)
            {
                // SWITCH EVENTS

                InitializeEvents();

                ParseEvents test = new ParseEvents(eventsKeys, eventsValues, text);
                //TraceManager.Add("test.name:" + test.name);
                //TraceManager.Add("test.callback:" + test.callback);
                //TraceManager.Add("test.switchName:" + test.switchName);
                //TraceManager.Add("test.switchCallback:" + test.switchCallback);

                String[] arr = RegexEvents.Split(text);
                for (int i = 0; i < arr.Length; i++)
                {
                    if (arr[i] == test.name) s += test.switchName;
                    else s += arr[i];
                }
                s = Regex.Replace(s, test.callback, test.switchCallback);
            }
            else
            {
                // SWITCH PROPERTIES

                InitializeProperties();

                List<String> props = settingObject.PropertiesList;
                
                String[] arr = RegexProperties.Split(text);
                for (int i = 0; i < arr.Length; i++)
                    s += SwitchValue(propertiesKeys, propertiesValues, arr[i]);
            }

            //TraceManager.Add("s:" + s);
            sci.InsertText(sci.LineEndPosition(line), LineEndDetector.GetNewLineMarker(sci.EOLMode) + s);

            int nxt = line + 1;
            int ss2 = sci.PositionFromLine(nxt) + ss;
            if (sci.LineFromPosition(ss2) != nxt) ss2 = sci.LineEndPosition(nxt);
            int se2 = sci.PositionFromLine(nxt) + se;
            if (sci.LineFromPosition(se2) != nxt) se2 = sci.LineEndPosition(nxt);
            sci.SetSel(ss2, se2);

            sci.EndUndoAction();
        }

        public static string SwitchValue(List<String> keys, List<String> values, string key)
        {
            int i = keys.IndexOf(key);
            if (i == -1) return key;
            return values[i];
        }

        public void DuplicateIncrease()
        {
            if (!PluginBase.MainForm.CurrentDocument.IsEditable) return;

            ScintillaNet.ScintillaControl sci = PluginBase.MainForm.CurrentDocument.SciControl;

            sci.BeginUndoAction();

            int pos = sci.CurrentPos;
            int line = sci.LineFromPosition(pos);
            int start = sci.PositionFromLine(line);

            if (IsMultilineSelection(sci)) sci.SetSel(start, sci.SelectionEnd);

            // sci.GetLine est bugge sur la derniere ligne dans la beta 7
            string text = "";
            if (line < sci.LineCount - 1)
            {
                text = sci.GetLine(line);
                if (sci.EOLMode == 0)
                    text = text.Substring(0, text.Length - 1);
            }
            else
            {
                text = sci.GetCurLine(sci.LineLength(line));
            }
            //TraceManager.Add(":" + text + ":");

            int ss = sci.SelectionStart - start;
            int se = sci.SelectionEnd - start;
            int dec = 0;
            //TraceManager.Add("ss:" + ss + ", se:"+se);
            
            MatchCollection matches = RegIncNum.Matches(text);
            String[] arr = RegIncNum.Split(text);

            string s = "";
            int mi;
            int ml;
            string mv;
            bool test1;
            bool test2;
            string prv;
            string nxt;
            string prvmv;
            string nxtmv;
            //
            for (int i = 0; i < arr.Length; i++)
            {
                s += arr[i];
                
                //
                if (i < arr.Length - 1)
                {
                    mi = matches[i].Index;
                    ml = matches[i].Length;
                    mv = matches[i].Value;

                    //TraceManager.Add(i + ":" + arr[i]);
                    // la selection inclue mi ?
                    test1 = (mi >= ss && mi < se);
                    // le debut de la selection se trouve-t-il entre mi et mi+ml ?
                    test2 = (ss >= mi && ss < (mi + ml));
                    // TraceManager.Add("ss:" + ss + ", index:" + mi + ", end:" + (mi + ml) + ", test1:" + test1 + ", test2:"+test2);
                    if (test1 || test2)
                    {
                        s += mv;
                    }
                    else
                    {
                        prv = arr[i];
                        nxt = arr[i + 1];

                        if (i == 0 ) prvmv = "";
                        else prvmv = matches[i - 1].Value;

                        if (i == matches.Count - 1) nxtmv = "";
                        else nxtmv = matches[i + 1].Value;
                        
                        if (prv.EndsWith(".")) s += mv;
                        else if (mv == "9" && prv.EndsWith("scale") && nxt.StartsWith("Grid")) s += mv;     // scale9Grid
                        else if (mv == "2" && prv.EndsWith("LN")) s += mv;                                  // LN2
                        else if (mv == "10" && prv.EndsWith("LN")) s += mv;                                 // LN10
                        else if (mv == "2" && prv.EndsWith("LOG") && nxt.StartsWith("E")) s += mv;          // LOG2E
                        else if (mv == "10" && prv.EndsWith("LOG") && nxt.StartsWith("E")) s += mv;         // LOG10E
                        else if (mv == "2" && prv.EndsWith("SQRT")) s += mv;                                // SQRT2
                        else if (mv == "1" && prv.EndsWith("SQRT") && nxt == "_" && nxtmv == "2") s += mv;  // SQRT1_2
                        else if (mv == "2" && prv == "_" && prvmv == "1") s += mv;                          // SQRT1_2
                        else if (mv == "3" && prv.EndsWith("id")) s += mv;                                  // id3
                        else if (mv == "3" && prv.EndsWith("ID") && nxt.StartsWith("Info")) s += mv;        // ID3Info
                        else if (mv == "1" && prv.EndsWith("AVM") && nxt.StartsWith("Movie")) s += mv;      // AVM1Movie
                        else if (mv == "3" && prv.EndsWith("AS")) s += mv;                                  // AVM1Movie
                        else if (prv.EndsWith("FLASH")) s += mv;                                            // FLASH1 ... FLASH9
                        else if (mv == "2" && prv.EndsWith("ACTIONSCRIPT")) s += mv;                        // ACTIONSCRIPT2
                        else if (mv == "3" && prv.EndsWith("ACTIONSCRIPT")) s += mv;                        // ACTIONSCRIPT3
                        else if (mv == "32" && prv.EndsWith("getPixel")) s += mv;                           // getPixel32
                        else if (mv == "32" && prv.EndsWith("setPixel")) s += mv;                           // setPixel32
                        else
                        {
                            string inc = (Convert.ToInt32(mv) + 1).ToString();

                            if (inc.Length > mv.Length && mi < ss) dec++;

                            s += inc;
                        }
                    }
                }
            }
            
            sci.InsertText(sci.LineEndPosition(line), LineEndDetector.GetNewLineMarker(sci.EOLMode) + s);

            int sel = sci.PositionFromLine(line + 1) + dec;
            sci.SetSel(sel + ss, sel + se);

            sci.EndUndoAction();
        }

        public bool IsMultilineSelection(ScintillaNet.ScintillaControl sci)
        {
            return sci.LineFromPosition(sci.SelectionStart) != sci.LineFromPosition(sci.SelectionEnd);
        }

        /// <summary>
        /// Initializes important variables
        /// </summary>
        public void InitBasics()
        {
            String dataPath = Path.Combine(PathHelper.DataDir, "Duplicate");
            if (!Directory.Exists(dataPath)) Directory.CreateDirectory(dataPath);
            this.settingFilename = Path.Combine(dataPath, "Settings.fdb");
        }

        /// <summary>
        /// Adds the required event handlers
        /// </summary> 
        public void AddEventHandlers()
        {
            EventManager.AddEventHandler(this, EventType.Keys);
        }

        /// <summary>
        /// Creates a menu item for the plugin and adds a ignored key
        /// </summary>
        public void InitPlugin()
        {
            PluginBase.MainForm.IgnoredKeys.Add(settingObject.IncreaseShortcut);
            PluginBase.MainForm.IgnoredKeys.Add(settingObject.SwitchShortcut);
            PluginBase.MainForm.IgnoredKeys.Add(settingObject.WordShortcut);
        }

        /// <summary>
        /// Loads the plugin settings
        /// </summary>
        public void LoadSettings()
        {
            this.settingObject = new Settings();
            if (!File.Exists(this.settingFilename))
            {
                List<String> events = new List<String>();
                events.Add("ADDED_TO_STAGE" + SEPAR + "REMOVED_FROM_STAGE");
                events.Add("REMOVED_FROM_STAGE" + SEPAR + "ADDED_TO_STAGE");
                events.Add("ADDED" + SEPAR + "REMOVED");
                events.Add("REMOVED" + SEPAR + "ADDED");
                events.Add("ACTIVATE" + SEPAR + "DEACTIVATE");
                events.Add("DEACTIVATE" + SEPAR + "ACTIVATE");
                events.Add("INIT" + SEPAR + "COMPLETE");
                events.Add("COMPLETE" + SEPAR + "INIT");
                events.Add("OPEN" + SEPAR + "CLOSE");
                events.Add("CLOSE" + SEPAR + "OPEN");
                events.Add("MOUSE_OVER" + SEPAR + "MOUSE_OUT");
                events.Add("MOUSE_OUT" + SEPAR + "CLICK");
                events.Add("CLICK" + SEPAR + "DOUBLE_CLICK");
                events.Add("DOUBLE_CLICK" + SEPAR + "CLICK");
                events.Add("ROLL_OVER" + SEPAR + "ROLL_OUT");
                events.Add("ROLL_OUT" + SEPAR + "CLICK");
                events.Add("MOUSE_OVER" + SEPAR + "MOUSE_OUT");
                events.Add("MOUSE_OUT" + SEPAR + "CLICK");
                events.Add("MOUSE_DOWN" + SEPAR + "MOUSE_UP");
                events.Add("MOUSE_UP" + SEPAR + "CLICK");
                events.Add("TIMER" + SEPAR + "TIMER_COMPLETE");
                events.Add("TIMER_COMPLETE" + SEPAR + "TIMER");
                events.Add("KEY_DOWN" + SEPAR + "KEY_UP");
                events.Add("KEY_UP" + SEPAR + "KEY_DOWN");
                events.Add("FOCUS_IN" + SEPAR + "FOCUS_OUT");
                events.Add("FOCUS_OUT" + SEPAR + "FOCUS_IN");
                events.Add("KEY_FOCUS_CHANGE" + SEPAR + "MOUSE_FOCUS_CHANGE");
                events.Add("MOUSE_FOCUS_CHANGE" + SEPAR + "KEY_FOCUS_CHANGE");
                events.Add("AT_TARGET" + SEPAR + "BUBBLING_PHASE");
                events.Add("BUBBLING_PHASE" + SEPAR + "CAPTURING_PHASE");
                events.Add("CAPTURING_PHASE" + SEPAR + "AT_TARGET");
                events.Add("MENU_SELECT" + SEPAR + "MENU_ITEM_SELECT");
                events.Add("MENU_ITEM_SELECT" + SEPAR + "MENU_SELECT");
                this.settingObject.EventsList = events;

                List<String> properties = new List<String>();
                properties.Add("width" + SEPAR + "height");
                properties.Add("height" + SEPAR + "width");
                properties.Add("wid" + SEPAR + "hei");
                properties.Add("hei" + SEPAR + "wid");
                properties.Add("w" + SEPAR + "h");
                properties.Add("h" + SEPAR + "w");
                properties.Add("x" + SEPAR + "y");
                properties.Add("y" + SEPAR + "x");
                properties.Add("front" + SEPAR + "back");
                properties.Add("back" + SEPAR + "front");
                properties.Add("left" + SEPAR + "right");
                properties.Add("right" + SEPAR + "left");
                properties.Add("upState" + SEPAR + "overState");
                properties.Add("overState" + SEPAR + "downState");
                properties.Add("downState" + SEPAR + "hitTestState");
                properties.Add("hitTestState" + SEPAR + "upState");
                this.settingObject.PropertiesList = properties;

                List<String> words = new List<String>();
                words.Add("addEventListener" + SEPAR + "removeEventListener");
                words.Add("removeEventListener" + SEPAR + "addEventListener");
                words.Add("public" + SEPAR + "private");
                words.Add("private" + SEPAR + "protected");
                words.Add("protected" + SEPAR + "public");
                words.Add("true" + SEPAR + "false");
                words.Add("false" + SEPAR + "true");
                words.Add("width" + SEPAR + "height");
                words.Add("height" + SEPAR + "width");
                words.Add("scaleX" + SEPAR + "scaleY");
                words.Add("scaleY" + SEPAR + "scaleX");
                words.Add("upState" + SEPAR + "overState");
                words.Add("overState" + SEPAR + "downState");
                words.Add("downState" + SEPAR + "hitTestState");
                words.Add("hitTestState" + SEPAR + "upState");
                this.settingObject.WordsList = words;

                this.SaveSettings();
            } else
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

    class ParseEvents
    {
        public String name = "";
        public String callback = "";
        public String switchName = "";
        public String switchCallback = "";

        public ParseEvents(List<String> keys, List<String> values, string line)
        {
            MatchCollection m;
            string s;

            m = Regex.Matches(line, @"(addEventListener|removeEventListener)[ \(\w]*");
            if (m.Count == 0) return;
            s = line.Substring(line.IndexOf(m[0].Value) + m[0].Value.Length);
            if (s.Substring(0,1) != ".") return;
            s = s.Substring(1);
            
            m = Regex.Matches(s, @"[\w]*");
            if (m.Count == 0) return;
            name = m[0].Value;
            //TraceManager.Add("name:" + name);

            s = s.Split(',')[1];
            m = Regex.Matches(s, @"[ \w]*");
            if (m.Count == 0) return;
            callback = m[0].Value.Trim();
            //TraceManager.Add("callback:" + callback);

            switchName = PluginMain.SwitchValue(keys, values, name);
            //TraceManager.Add("switchName:" + switchName);

            //int idx = -1;
            Variations v1 = new Variations(name);
            //TraceManager.Add("v1.allLowerCase: " + v1.allLowerCase);
            //TraceManager.Add("v1.firstOnlyLowerCase: " + v1.firstOnlyLowerCase);
            //TraceManager.Add("v1.allUpperCase: " + v1.allUpperCase);
            //TraceManager.Add("v1.extractLowerCase: " + v1.extractLowerCase);
            //TraceManager.Add("v1.extractUpperCase: " + v1.extractUpperCase);

            Variations v2 = new Variations(switchName);
            //TraceManager.Add("v2.allLowerCase: " + v2.allLowerCase);
            //TraceManager.Add("v2.firstOnlyLowerCase: " + v2.firstOnlyLowerCase);
            //TraceManager.Add("v2.allUpperCase: " + v2.allUpperCase);
            //TraceManager.Add("v2.extractLowerCase: " + v2.extractLowerCase);
            //TraceManager.Add("v2.extractUpperCase: " + v2.extractUpperCase);

            switchCallback = callback;
            if (callback.IndexOf(v1.allLowerCase) > -1) SwitchCallback(v1.allLowerCase, v2.allLowerCase);
            else if (callback.IndexOf(v1.firstOnlyLowerCase) > -1) SwitchCallback(v1.firstOnlyLowerCase, v2.firstOnlyLowerCase);
            else if (callback.IndexOf(v1.allUpperCase) > -1) SwitchCallback(v1.allUpperCase, v2.allUpperCase);
            else if (callback.IndexOf(v1.extractLowerCase) > -1) SwitchCallback(v1.extractLowerCase, v2.extractLowerCase);
            else if (callback.IndexOf(v1.extractUpperCase) > -1) SwitchCallback(v1.extractUpperCase, v2.extractUpperCase);       
        }

        private void SwitchCallback(string before, string after)
        {
            switchCallback = Regex.Replace(switchCallback, before, after);
        }
    }

    class Variations
    {
        public String allLowerCase = "";
        public String allUpperCase = "";
        public String firstOnlyLowerCase = "";
        public String extractLowerCase = "";
        public String extractUpperCase = "";

        public Variations(string name)
        {
            String[] arr = name.Split('_');
            for (int i = 0; i < arr.Length; i++)
                arr[i] = arr[i].ToLower();

            // mouseout
            for (int i = 0; i < arr.Length; i++)
                allLowerCase += arr[i];

            // mouseOut
            firstOnlyLowerCase = arr[0];
            for (int i = 1; i < arr.Length; i++)
                firstOnlyLowerCase += PluginMain.UpperCaseFirst(arr[i]);

            // MouseOut
            allUpperCase = PluginMain.UpperCaseFirst(firstOnlyLowerCase);

            // out & Out
            if (name.IndexOf("OVER") > -1)
            {
                extractLowerCase = "over";
                extractUpperCase = "Over";
            }
            else if (name.IndexOf("OUT") > -1)
            {
                extractLowerCase = "out";
                extractUpperCase = "Out";
            }
            else if (name.IndexOf("DOWN") > -1)
            {
                extractLowerCase = "down";
                extractUpperCase = "Down";
            }
            else if (name.IndexOf("UP") > -1)
            {
                extractLowerCase = "up";
                extractUpperCase = "Up";
            }

            if (extractLowerCase == "") extractLowerCase = firstOnlyLowerCase;
            if (extractUpperCase == "") extractUpperCase = allUpperCase;
        }
    }
}

