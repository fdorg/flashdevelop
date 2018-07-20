// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using WeifenLuo.WinFormsUI.Docking;
using SamplePlugin.Resources;
using PluginCore.Localization;
using PluginCore.Utilities;
using PluginCore.Managers;
using PluginCore.Helpers;
using PluginCore;
using ScintillaNet;
using System.Collections;

namespace Trace
{
    public class PluginMain : IPlugin
    {
        private String pluginName = "Trace";
        private String pluginGuid = "c98399e0-216b-11dc-b8e7-ec7a55d89593";
        private String pluginHelp = "www.flashdevelop.org/community/";
        private String pluginDesc = "Beta 0.0.4 - trace() generator";
        private String pluginAuth = "Jérôme Decoster";

        private String settingFilename;
        private Settings settingObject;
        private String sArgsString;

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
            //this.AddEventHandlers();
            this.InitLocalization();
            //this.CreatePluginPanel();
            this.CreateMenuItem();

            sArgsString = "$(CurWord)¤$(TypPkg)¤$(TypName)¤$(TypKind)¤$(MbrName)¤$(MbrKind)";
        }

        /// <summary>
        /// Initializes important variables
        /// </summary>
        public void InitBasics()
        {
            String dataPath = Path.Combine(PathHelper.DataDir, "Trace");
            if (!Directory.Exists(dataPath)) Directory.CreateDirectory(dataPath);
            this.settingFilename = Path.Combine(dataPath, "Settings.fdb");
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
        }

        /// <summary>
        /// Saves the plugin settings
        /// </summary>
        public void SaveSettings()
        {
            ObjectSerializer.Serialize(this.settingFilename, this.settingObject);
        }

        /// <summary>
        /// Creates a menu item for the plugin and adds a ignored key
        /// </summary>
        public void CreateMenuItem()
        {
            ToolStripMenuItem menu = (ToolStripMenuItem)PluginBase.MainForm.FindMenuItem("InsertMenu");
            if (menu != null)
            {
                menu.DropDownItems.Add(new ToolStripSeparator());

                TraceMenu traceMenu = new TraceMenu();

                traceMenu.TraceSimple.Click += new EventHandler(TraceSimple);
                traceMenu.TraceForIn.Click += new EventHandler(TraceForIn);
                traceMenu.TraceAlternateSimple.Click += new EventHandler(TraceAlternateSimple);
                traceMenu.TraceAlternateForIn.Click += new EventHandler(TraceAlternateForIn);

                Keys kTS = this.settingObject.TraceSimple;
                Keys kTFI = this.settingObject.TraceForIn;
                Keys kTAS = this.settingObject.TraceAlternateSimple;
                Keys kTAFI = this.settingObject.TraceAlternateForIn;

                traceMenu.TraceSimple.ShortcutKeys = kTS;
                traceMenu.TraceForIn.ShortcutKeys = kTFI;
                traceMenu.TraceAlternateSimple.ShortcutKeys = kTAS;
                traceMenu.TraceAlternateForIn.ShortcutKeys = kTAFI;

                PluginBase.MainForm.IgnoredKeys.Add(kTS);
                PluginBase.MainForm.IgnoredKeys.Add(kTFI);
                PluginBase.MainForm.IgnoredKeys.Add(kTAS);
                PluginBase.MainForm.IgnoredKeys.Add(kTAFI);

                menu.DropDownItems.Add(traceMenu);
            }
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

        }

        /// <summary>
        /// Initializes the localization of the plugin
        /// </summary>
        public void InitLocalization()
        {
            LocaleVersion locale = PluginBase.MainForm.Settings.LocaleVersion;
            switch (locale)
            {
                default:
                    // Plugins should default to English...
                    LocaleHelper.Initialize(LocaleVersion.en_US);
                    break;
            }
            //this.pluginDesc = LocaleHelper.GetString("Info.Description");
        }

        #endregion

        #region Custom Methods

        private void BaseSimple(bool aternate)
        {
            ScintillaControl sci = PluginBase.MainForm.CurrentDocument.SciControl;

            sci.BeginUndoAction();

            // Settings
            String setAlt = this.settingObject.AlternateFunction.Trim();
            if (setAlt == "")
                setAlt = "MyLogger.log";
            Boolean setIns = this.settingObject.InsertNewLine;
            Boolean setCmp = this.settingObject.CompactMode;
            Boolean setPkg = this.settingObject.ShowPackageName;
            Boolean setCls = this.settingObject.ShowClassName;

            PositionInfos t = new PositionInfos(sci, sci.CurrentPos, PluginBase.MainForm.ProcessArgString(sArgsString));
            String trace = "";
            Boolean exec = false;

            if (t.HasSelection)
            {
                if (!t.SelectionIsMultiline)
                {
                    String sel1 = t.SelectedText.Trim();
                    if (sel1.EndsWith(";"))
                        sel1 = sel1.Substring(0, sel1.Length - 1);

                    String sel0 = sel1;
                    if (sel0.IndexOf('"') > -1)
                        sel0 = sel0.Replace("\"", "\\\"");

                    String tr0 = "trace( \"";
                    if(aternate)
                        tr0 = setAlt + "( \"";
                    String tr1 = " : \" + ";
                    String tr2 = " );";
                    if (setCmp)
                    {
                        tr0 = "trace(\"";
                        if (aternate)
                            tr0 = setAlt + "(\"";
                        tr1 = ": \"+";
                        tr2 = ");";
                    }

                    trace = tr0 + sel0 + tr1 + sel1 + tr2;
                    exec = true;
                }
            }
            else
            {
                if (t.PreviousWordIsFunction)
                {
                    String pckg = "";
                    String clas = "";

                    if (!t.HasArguments)
                    {
                        String tr0 = "trace( \"";
                        if (aternate)
                            tr0 = setAlt + "( \"";
                        String tr1 = "\" );";
                        if (setCmp)
                        {
                            tr0 = "trace(\"";
                            if (aternate)
                                tr0 = setAlt + "(\"";
                            tr1 = "\");";
                        }

                        if (setPkg)
                            pckg = t.ArgPackageName + ".";

                        if (setCls)
                            clas = t.ArgClassName + ".";

                        trace = tr0 + pckg + clas + t.WordFromPosition + tr1;
                        exec = true;
                    }
                    else
                    {
                        String tr0 = "trace( \"";
                        if (aternate)
                            tr0 = setAlt + "( \"";
                        String tr1 = " > ";
                        String tr2 = " );";
                        if (setCmp)
                        {
                            tr0 = "trace(\"";
                            if (aternate)
                                tr0 = setAlt + "(\"";
                            tr1 = " > ";
                            tr2 = ");";
                        }

                        if (setPkg)
                            pckg = t.ArgPackageName + ".";

                        if (setCls)
                            clas = t.ArgClassName + ".";

                        String args = "";
                        String a = "";
                        for (Int32 i = 0; i < t.Arguments.Count; i++)
                        {
                            a = (String)t.Arguments[i];
                            if (i == 0)
                            {
                                if (!setCmp)
                                    args += a + " : \" + " + a + " + ";
                                else
                                    args += a + ": \"+" + a + "+";
                            }
                            else
                            {
                                if (!setCmp)
                                    args += "\", " + a + " : \" + " + a + " + ";
                                else
                                    args += "\", " + a + ": \"+" + a + "+";
                            }
                        }

                        if (!setCmp)
                            args = args.Substring(0, args.Length - 3);
                        else
                            args = args.Substring(0, args.Length - 1);

                        trace = tr0 + pckg + clas + t.WordFromPosition + tr1 + args + tr2;
                        exec = true;
                    }
                }
                else
                {
                    if (t.WordFromPosition != "")
                    {
                        String tr0 = "trace( \"";
                        if (aternate)
                            tr0 = setAlt + "( \"";
                        String tr1 = " : \" + ";
                        String tr2 = " );";
                        if (setCmp)
                        {
                            tr0 = "trace(\"";
                            if (aternate)
                                tr0 = setAlt + "(\"";
                            tr1 = ": \"+";
                            tr2 = ");";
                        }

                        trace = tr0 + t.WordFromPosition + tr1 + t.WordFromPosition + tr2;
                        exec = true;
                    }
                }
            }

            if (exec)
            {
                HelpTools.SetCaretReadyToTrace(sci, true);

                sci.InsertText(sci.CurrentPos, trace);

                HelpTools.GoToLineEnd(sci, setIns);
            }

            sci.EndUndoAction();
        }

        private void BaseForIn(bool aternate)
        {
            ScintillaControl sci = PluginBase.MainForm.CurrentDocument.SciControl;

            sci.BeginUndoAction();

            // Settings
            String setAlt = this.settingObject.AlternateFunction.Trim();
            if (setAlt == "")
                setAlt = "MyLogger.log";
            Boolean setIns = this.settingObject.InsertNewLine;
            Boolean setCmp = this.settingObject.CompactMode;

            String tr0 = "for( var i:String in ";
            String tr1 = " ) trace( \"key : \" + i + \", value : \" + ";
            if (aternate)
                tr1 = " ) " + setAlt + "( \"key : \" + i + \", value : \" + ";
            String tr2 = "[ i ] );";

            if (setCmp)
            {
                tr0 = "for(var i:String in ";
                tr1 = ") trace(\"key: \"+i+\", value: \"+";
                if (aternate)
                    tr1 = ") " + setAlt + "(\"key: \"+i+\", value: \"+";
                tr2 = "[i]);";
            }

            PositionInfos t = new PositionInfos(sci, sci.CurrentPos, PluginBase.MainForm.ProcessArgString(sArgsString));
            Boolean exec = false;
            String sel = "";

            if (t.HasSelection)
            {
                if (!t.SelectionIsMultiline)
                {
                    sel = t.SelectedText.Trim();
                    exec = true;
                }
            }
            else
            {
                if (t.WordFromPosition != "")
                {
                    sel = t.WordFromPosition;
                    exec = true;
                }
            }

            if (exec)
            {
                HelpTools.SetCaretReadyToTrace(sci, true);

                String trace = tr0 + sel + tr1 + sel + tr2;

                sci.InsertText(sci.CurrentPos, trace);

                HelpTools.GoToLineEnd(sci, setIns);
            }

            sci.EndUndoAction();
        }

        private void TraceSimple(object sender, System.EventArgs e)
        {
            BaseSimple(false);
            //TraceTest();
        }

        private void TraceForIn(object sender, System.EventArgs e)
        {
            BaseForIn(false);
        }

        private void TraceAlternateSimple(object sender, System.EventArgs e)
        {
            BaseSimple(true);
        }

        private void TraceAlternateForIn(object sender, System.EventArgs e)
        {
            BaseForIn(true);
        }
        
        private void Alert(string s)
        {
            PluginCore.Managers.ErrorManager.ShowInfo(s + "     ");
        }

        private void TraceTest()
        {
            ScintillaControl sci = PluginBase.MainForm.CurrentDocument.SciControl;

            PositionInfos t = new PositionInfos(sci, sci.CurrentPos, PluginBase.MainForm.ProcessArgString(sArgsString));

            String nlm = t.NewLineMarker;

            String s = nlm + nlm + "sci.CurrentPos: " + sci.CurrentPos + nlm + nlm;

            s += "ArgCurWord: " + t.ArgCurWord + nlm;
            s += "ArgPackageName: " + t.ArgPackageName + nlm;
            s += "ArgClassName: " + t.ArgClassName + nlm;
            s += "ArgClassType: " + t.ArgClassType + nlm;
            s += "ArgMemberName: " + t.ArgMemberName + nlm;
            s += "ArgMemberType: " + t.ArgMemberType + nlm + nlm;

            s += "SelectionStart: " + t.SelectionStart + nlm;
            s += "SelectionEnd: " + t.SelectionEnd + nlm;
            s += "HasSelection: " + t.HasSelection + nlm;
            s += "SelectionIsMultiline: " + t.SelectionIsMultiline + nlm;
            s += "SelectedText: " + t.SelectedText + nlm;

            s += "CurrentPosition: " + t.CurrentPosition + nlm;
            s += "CurrentCharCode: " + t.CurrentCharCode + nlm;
            s += "CurrentIsWhiteChar : " + t.CurrentIsWhiteChar + nlm;
            s += "CurrentIsDotChar : " + t.CurrentIsDotChar + nlm;
            s += "CurrentIsActionScriptChar : " + t.CurrentIsActionScriptChar + nlm;
            s += "CurrentIsWordChar : " + t.CurrentIsWordChar + nlm;
            s += "CurrentIsInsideComment: " + t.CurrentIsInsideComment + nlm + nlm;

            s += "NextPosition: " + t.NextPosition + nlm;
            s += "CaretIsAtEndOfDocument: " + t.CaretIsAtEndOfDocument + nlm + nlm;

            s += "WordStartPosition: " + t.WordStartPosition + nlm;
            s += "WordEndPosition: " + t.WordEndPosition + nlm;
            s += "WordFromPosition: " + t.WordFromPosition + nlm;
            s += "CaretIsAfterLastLetter: " + t.CaretIsAfterLastLetter + nlm + nlm;

            s += "PreviousPosition: " + t.PreviousPosition + nlm;
            s += "PreviousCharCode: " + t.PreviousCharCode + nlm;
            s += "PreviousIsWhiteChar: " + t.PreviousIsWhiteChar + nlm;
            s += "PreviousIsDotChar: " + t.PreviousIsDotChar + nlm;
            s += "PreviousIsActionScriptChar: " + t.PreviousIsActionScriptChar + nlm + nlm;

            s += "CurrentLineIdx: " + t.CurrentLineIdx + nlm;
            s += "PreviousLineIdx: " + t.PreviousLineIdx + nlm;
            s += "LineIdxMax: " + t.LineIdxMax + nlm;
            s += "LineStartPosition: " + t.LineStartPosition + nlm;
            s += "LineEndPosition: " + t.LineEndPosition + nlm + nlm;

            s += "PreviousNonWhiteCharPosition: " + t.PreviousNonWhiteCharPosition + nlm;
            s += "PreviousWordIsFunction: " + t.PreviousWordIsFunction + nlm;  
            s += "NextNonWhiteCharPosition: " + t.NextNonWhiteCharPosition + nlm + nlm;

            s += "NextOpenBracketPosition: " + t.NextOpenBracketPosition + nlm;
            s += "NextCloseBracketPosition: " + t.NextCloseBracketPosition + nlm;
            s += "NextLeftBracePosition: " + t.NextLeftBracePosition + nlm + nlm;

            s += "HasArguments: " + t.HasArguments + nlm;
            s += "Arguments.Count: " + t.Arguments.Count + nlm + nlm;

            sci.InsertText(sci.TextLength, s);
        }

        #endregion

    }

    #region Custom Class

    public class PositionInfos
    {
        // Variables
        public String ArgCurWord = "";
        public String ArgPackageName = "";
        public String ArgClassName = "";
        public String ArgClassType = ""; // (interface|class)
        public String ArgMemberName = "";
        public String ArgMemberType = "";
        // Selection
        public Int32 SelectionStart = -1;
        public Int32 SelectionEnd = -1;
        public Boolean HasSelection = false;
        public Boolean SelectionIsMultiline = false;
        public String SelectedText = "";
        // Current
        public Int32 CurrentPosition;
        public Int32 CurrentCharCode;
        public Boolean CurrentIsWhiteChar;
        public Boolean CurrentIsDotChar;
        public Boolean CurrentIsActionScriptChar;
        public Boolean CurrentIsWordChar;
        public Boolean CurrentIsInsideComment;
        // Next
        public Int32 NextPosition = -1;
        public Boolean CaretIsAtEndOfDocument = false;
        // Word
        public Int32 CodePage = -1;
        public Int32 WordStartPosition = -1;
        public Int32 WordEndPosition = -1;
        public String WordFromPosition = "";
        public Boolean CaretIsAfterLastLetter = false;
        // Previous
        public Int32 PreviousPosition = -1;
        public Int32 PreviousCharCode = -1;
        public Boolean PreviousIsWhiteChar = false;
        public Boolean PreviousIsDotChar = false;
        public Boolean PreviousIsActionScriptChar = false;
        // Line
        public Int32 CurrentLineIdx;
        public Int32 PreviousLineIdx = -1;
        public Int32 LineIdxMax;
        public Int32 LineStartPosition;
        public Int32 LineEndPosition;
        public String NewLineMarker;
        // Previous / Next
        public Int32 PreviousNonWhiteCharPosition = -1;
        public Boolean PreviousWordIsFunction = false;
        public Int32 NextNonWhiteCharPosition = -1;
        // Function
        public Int32 NextOpenBracketPosition = -1;
        public Int32 NextCloseBracketPosition = -1;
        public Int32 NextLeftBracePosition = -1;
        // Arguments
        public Boolean HasArguments = false;
        public ArrayList Arguments = new ArrayList();

        public PositionInfos(ScintillaControl sci, Int32 position, String argString)
        {
            // Variables
            String[] vars = argString.Split('¤');
            this.ArgCurWord = vars[0];
            this.ArgPackageName = vars[1];
            this.ArgClassName = vars[2];
            this.ArgClassType = vars[3];
            this.ArgMemberName = vars[4];
            this.ArgMemberType = vars[5];

            // Selection
            Int32 ss = sci.SelectionStart;
            Int32 se = sci.SelectionEnd;
            if (se != ss)
            {
                this.SelectionStart = ss;
                this.SelectionEnd = se;
                this.HasSelection = true;
                if (sci.LineFromPosition(ss) != sci.LineFromPosition(se))
                    this.SelectionIsMultiline = true;
                else SelectedText = sci.SelText;
            }

            // Current
            this.CurrentPosition = position;
            this.CurrentCharCode = sci.CharAt(position);
            this.CurrentIsWhiteChar = (HelpTools.IsWhiteChar(this.CurrentCharCode));
            this.CurrentIsDotChar = (this.CurrentCharCode == 46);
            this.CurrentIsActionScriptChar = HelpTools.IsActionScriptChar(this.CurrentCharCode);
            this.CurrentIsWordChar = HelpTools.IsWordChar((byte)this.CurrentCharCode);
            Int32 s = sci.StyleAt(position);
            this.CurrentIsInsideComment = (s == 1 || s == 2 || s == 3 || s == 17);

            // Next
            Int32 np = sci.PositionAfter(position);
            if (np != position)
                this.NextPosition = np;
            else
                this.CaretIsAtEndOfDocument = true;

            // Word
            this.CodePage = sci.CodePage; // (UTF-8|Big Endian|Little Endian : 65001) (8 Bits|UTF-7 : 0)
            
            if (this.CurrentIsInsideComment == false && this.SelectionIsMultiline == false)
            {
                Int32 wsp = sci.WordStartPosition(position, true);
                // Attention (WordEndPosition n'est pas estimé comme par defaut)
                Int32 wep = sci.PositionBefore(sci.WordEndPosition(position, true));

                if (this.CodePage != 65001)
                {
                    wsp = HelpTools.GetWordStartPositionByWordChar(sci, position);
                    // Attention (WordEndPosition n'est pas estimé comme par defaut)
                    wep = sci.PositionBefore(HelpTools.GetWordEndPositionByWordChar(sci, position));
                }

                this.WordStartPosition = wsp;
                this.WordEndPosition = wep;

                if (this.CodePage == 65001)
                    this.WordFromPosition = this.ArgCurWord;
                else
                    this.WordFromPosition = HelpTools.GetText(sci, wsp, sci.PositionAfter(wep));

                if (position > wep)
                    this.CaretIsAfterLastLetter = true;
            }
            
            // Previous
            if (this.CurrentPosition > 0)
            {
                this.PreviousPosition = sci.PositionBefore(position);
                this.PreviousCharCode = sci.CharAt(this.PreviousPosition);
                this.PreviousIsWhiteChar = HelpTools.IsWhiteChar(this.PreviousCharCode);
                this.PreviousIsDotChar = (this.PreviousCharCode == 46);
                this.PreviousIsActionScriptChar = HelpTools.IsActionScriptChar(this.PreviousCharCode);
            }

            // Line
            this.CurrentLineIdx = sci.LineFromPosition(position);
            if (this.CurrentPosition > 0)
                this.PreviousLineIdx = sci.LineFromPosition(this.PreviousPosition);

            this.LineIdxMax = sci.LineCount - 1;
            this.LineStartPosition = HelpTools.LineStartPosition(sci, this.CurrentLineIdx);
            this.LineEndPosition = sci.LineEndPosition(this.CurrentLineIdx);
            this.NewLineMarker = LineEndDetector.GetNewLineMarker(sci.EOLMode);

            // Previous / Next
            if (this.WordStartPosition != -1)
            {
                this.PreviousNonWhiteCharPosition = HelpTools.PreviousNonWhiteCharPosition(sci, this.WordStartPosition);
                this.PreviousWordIsFunction = (sci.GetWordFromPosition(this.PreviousNonWhiteCharPosition) == "function");
                this.NextNonWhiteCharPosition = HelpTools.NextNonWhiteCharPosition(sci, this.WordEndPosition);
            }

            // Function
            if (this.PreviousWordIsFunction)
            {
                Int32 nobp = HelpTools.NextCharPosition(sci, position, "(");
                Int32 ncbp = HelpTools.NextCharPosition(sci, position, ")");
                Int32 nlbp = HelpTools.NextCharPosition(sci, position, "{");
                if ((nobp < ncbp) && (ncbp < nlbp))
                {
                    this.NextOpenBracketPosition = nobp;
                    this.NextCloseBracketPosition = ncbp;
                    this.NextLeftBracePosition = nlbp;
                }

                // Arguments
                String args = HelpTools.GetText(sci, sci.PositionAfter(this.NextOpenBracketPosition), this.NextCloseBracketPosition).Trim();
                if (args.Length > 0)
                {
                    this.HasArguments = true;
                    this.Arguments = HelpTools.ExtractArguments(sci, args);
                }
            }
        }
    }
    
    public class HelpTools
    {
        public static bool IsWhiteChar(Int32 charCode)
        {
            // 32 : space, 10 : LF , 13 : CR , 9 : tabulation
            Int32 c = charCode;
            if (c == 32 || c == 10 || c == 13 || c == 9) return true;
            else return false;
        }

        public static bool IsActionScriptChar(Int32 charCode)
        {
            // A : 65 , Z : 90 , a : 97 , z : 122 , 0 : 48 , 9 : 57 , _ : 95
            Int32 c = charCode;
            if ((c >= 97 && c <= 122) || (c >= 65 && c <= 90) || (c >= 48 && c <= 57) || c == 95) return true;
            else return false;
        }

        public static bool IsWordChar(byte charByte)
        {
            // s'obtient par (byte)sci.CharAt(sci.CurrentPos)
            // A : 65 , Z : 90 , a : 97 , z : 122 , 0 : 48 , 9 : 57 , _ : 95
            // À : 192 , Ö : 214 , Ø : 216 , Ü : 220 , à : 224 , ö : 246 , ø : 248 , ü : 252
            Int32 b = charByte;
            if ((b >= 97 && b <= 122) || (b >= 65 && b <= 90) || (b >= 48 && b <= 57) || b == 95) return true;
            else if ((b >= 192 && b <= 214) || (b >= 216 && b <= 220) || (b >= 224 && b <= 246) || (b >= 248 && b <= 252)) return true;
            else return false;
        }

        public static Int32 GetWordStartPositionByWordChar(ScintillaControl sci, Int32 position)
        {
            Int32 pos = position;
            Int32 bef = sci.PositionBefore(pos);
            while (IsWordChar((byte)sci.CharAt(bef)) == true)
            {
                pos = bef;
                bef = sci.PositionBefore(pos);
                if (bef == pos)
                    return bef;
            }
            
            return pos;
        }

        public static Int32 GetWordEndPositionByWordChar(ScintillaControl sci, Int32 position)
        {
            if (IsWordChar((byte)sci.CharAt(position)) == false)
                return position;

            Int32 pos = position;
            Int32 aft = sci.PositionAfter(pos);
            while (IsWordChar((byte)sci.CharAt(aft)) == true)
            {
                pos = aft;
                aft = sci.PositionAfter(pos);
                if (aft == pos)
                    return aft;
            }

            return aft;
        }

        public static Int32 PreviousNonWhiteCharPosition(ScintillaControl sci, Int32 position)
        {
            Int32 pos = position;
            Int32 bef = sci.PositionBefore(pos);
            if (bef == pos)
                return -1;

            while (IsWhiteChar(sci.CharAt(bef)) == true)
            {
                pos = bef;
                bef = sci.PositionBefore(pos);
                if (bef == pos)
                    return -1;
            }

            return bef;
        }

        public static Int32 NextNonWhiteCharPosition(ScintillaControl sci, Int32 position)
        {
            Int32 pos = position;
            Int32 aft = sci.PositionAfter(pos);
            if (aft == pos)
                return -1;

            while (IsWhiteChar(sci.CharAt(aft)) == true)
            {
                pos = aft;
                aft = sci.PositionAfter(pos);
                if (aft == pos)
                    return -1;
            }

            return aft;
        }

        public static Int32 NextCharPosition(ScintillaControl sci, Int32 position, String c)
        {
            Int32 curPos = sci.CurrentPos;
            sci.GotoPos(position);
            char currentChar = (char)sci.CharAt(sci.CurrentPos);
            if (currentChar.ToString().Equals(c)) sci.CharRight();
            sci.SearchAnchor();
            Int32 next = sci.SearchNext(0, c);
            sci.GotoPos(curPos);
            return next;
        }

        public static Int32 LineStartPosition(ScintillaControl sci, Int32 lineIdx)
        {
            if (lineIdx == 0) return 0;

            Int32 pos = sci.LineEndPosition(lineIdx - 1) + 1;
            if (sci.EOLMode == 0) pos += 1;

            return pos;
        }

        public static String GetText(ScintillaControl sci, Int32 startPosition, Int32 endPosition)
        {
            Int32 curPos = sci.CurrentPos;
            Int32 selStart = sci.SelectionStart;
            Int32 selEnd = sci.SelectionEnd;
            Int32 firstLine = sci.FirstVisibleLine;
            sci.SetSel(startPosition, endPosition);
            String text = sci.SelText;
            sci.GotoPos(curPos);
            sci.SetSel(selStart, selEnd);
            Int32 actualFirstLine = sci.FirstVisibleLine;
            if (actualFirstLine != firstLine)
            {
                sci.LineScroll(0, firstLine - actualFirstLine);
            }
            return text;
        }

        public static ArrayList ExtractArguments(ScintillaControl sci, String text)
        {
            String[] arr = text.Split(',');
            ArrayList args = new ArrayList();

            for (Int32 i = 0; i < arr.Length; i++)
            {
                args.Add(arr[i].Trim().Split(':')[0].Trim());
            }
            return args;
        }

        public static Boolean NextNonWhiteCharIsOpenBrace(ScintillaControl sci, Int32 position)
        {
            Int32 pos = NextNonWhiteCharPosition(sci, position);
            if (pos > -1) return (sci.CharAt(pos) == 123);
            else return false;
        }

        public static void GoToLineEnd(ScintillaControl sci, Boolean insertNewLine)
        {
            sci.GotoPos(sci.LineEndPosition(sci.LineFromPosition(sci.CurrentPos)));

            if (insertNewLine) sci.NewLine();
        }

        public static void SetCaretReadyToTrace(ScintillaControl sci, Boolean insertNewLine)
        {
            GoToLineEnd(sci, false);
            if (NextNonWhiteCharIsOpenBrace(sci, sci.CurrentPos))
                sci.GotoPos(sci.PositionAfter(NextCharPosition(sci, sci.CurrentPos, "{")));

            if (insertNewLine) sci.NewLine();
        }
    }

    #endregion

}
