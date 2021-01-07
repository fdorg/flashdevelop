using System;
using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using ASCompletion.Context;
using CodeFormatter.Handlers;
using CodeFormatter.Utilities;
using PluginCore;
using PluginCore.Helpers;
using PluginCore.Localization;
using PluginCore.Managers;
using PluginCore.Utilities;
using ScintillaNet;

namespace CodeFormatter
{
    public class PluginMain : IPlugin
    {
        ToolStripMenuItem contextMenuItem;
        ToolStripMenuItem mainMenuItem;
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
        public string Name { get; } = nameof(CodeFormatter);

        /// <summary>
        /// GUID of the plugin
        /// </summary>
        public string Guid { get; } = "f7f1e15b-282a-4e55-ba58-5f2c02765247";

        /// <summary>
        /// Author of the plugin
        /// </summary>
        public string Author { get; } = "FlashDevelop Team";

        /// <summary>
        /// Description of the plugin
        /// </summary>
        public string Description { get; set; } = "Adds multiple code formatters to FlashDevelop.";

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
            CreateMainMenuItem();
            CreateContextMenuItem();
            LoadSettings();
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
                    UpdateMenuItems();
                    break;

                case EventType.Command:
                    var de = (DataEvent)e;
                    if (de.Action == "CodeRefactor.Menu")
                    {
                        AttachMainMenuItem((ToolStripMenuItem)de.Data);
                        UpdateMenuItems();
                    }
                    else if (de.Action == "CodeRefactor.ContextMenu")
                    {
                        AttachContextMenuItem((ToolStripMenuItem)de.Data);
                        UpdateMenuItems();
                    }
                    else if (de.Action == "CodeFormatter.FormatDocument")
                    {
                        DoFormat((ITabbedDocument)de.Data);
                    }
                    break;
            }
        }
        
        #endregion

        #region Custom Methods
        
        /// <summary>
        /// Initializes important variables
        /// </summary>
        void InitBasics()
        {
            EventManager.AddEventHandler(this, EventType.Command);
            EventManager.AddEventHandler(this, EventType.FileSwitch);
            var dataPath = Path.Combine(PathHelper.DataDir, nameof(CodeFormatter));
            if (!Directory.Exists(dataPath)) Directory.CreateDirectory(dataPath);
            settingFilename = Path.Combine(dataPath, "Settings.fdb");
            Description = TextHelper.GetString("Info.Description");
        }

        /// <summary>
        /// Update the menu items if they are available
        /// </summary>
        void UpdateMenuItems()
        {
            if (mainMenuItem is null || contextMenuItem is null) return;
            var doc = PluginBase.MainForm.CurrentDocument;
            var isValid = doc != null && doc.IsEditable && DocumentType != TYPE_UNKNOWN;
            mainMenuItem.Enabled = contextMenuItem.Enabled = isValid;
        }

        /// <summary>
        /// Creates a menu item for the plugin
        /// </summary>
        void CreateMainMenuItem()
        {
            string label = TextHelper.GetString("Label.CodeFormatter");
            mainMenuItem = new ToolStripMenuItem(label, null, Format, Keys.Control | Keys.Shift | Keys.D2);
            PluginBase.MainForm.RegisterShortcutItem("RefactorMenu.CodeFormatter", mainMenuItem);
        }

        void AttachMainMenuItem(ToolStripDropDownItem menu) => menu.DropDownItems.Insert(7, mainMenuItem);

        /// <summary>
        /// Creates a context menu item for the plugin
        /// </summary>
        void CreateContextMenuItem()
        {
            string label = TextHelper.GetString("Label.CodeFormatter");
            contextMenuItem = new ToolStripMenuItem(label, null, Format, Keys.None);
            PluginBase.MainForm.RegisterSecondaryItem("RefactorMenu.CodeFormatter", contextMenuItem);
        }

        void AttachContextMenuItem(ToolStripDropDownItem menu) => menu.DropDownItems.Insert(6, contextMenuItem);

        /// <summary>
        /// Loads the plugin settings
        /// </summary>
        void LoadSettings()
        {
            settingObject = new Settings();
            if (!File.Exists(settingFilename)) 
            {
                settingObject.InitializeDefaultPreferences();
                SaveSettings();
            }
            else settingObject = ObjectSerializer.Deserialize(settingFilename, settingObject);
        }

        /// <summary>
        /// Saves the plugin settings
        /// </summary>
        void SaveSettings() => ObjectSerializer.Serialize(settingFilename, settingObject);

        #endregion

        #region Code Formatting

        const int TYPE_AS3 = 0;
        const int TYPE_MXML = 1;
        const int TYPE_XML = 2;
        const int TYPE_CPP = 3;
        const int TYPE_UNKNOWN = 4;
        
        /// <summary>
        /// Formats the current document
        /// </summary>
        public void Format(object sender, EventArgs e) => DoFormat(PluginBase.MainForm.CurrentDocument);

        /// <summary>
        /// Formats the specified document
        /// </summary>
        void DoFormat(ITabbedDocument doc)
        {
            if (!doc.IsEditable) return;
            var sci = doc.SciControl;
            if (sci is null) return;
            sci.BeginUndoAction();
            int oldPos = CurrentPos;
            string source = sci.Text;
            try
            {
                switch (DocumentType)
                {
                    case TYPE_AS3:
                        var asPrinter = new ASPrettyPrinter(true, source);
                        FormatUtility.configureASPrinter(asPrinter, settingObject);
                        var asResultData = asPrinter.print(0);
                        if (asResultData is null)
                        {
                            TraceManager.Add(TextHelper.GetString("Info.CouldNotFormat"), -3);
                            PluginBase.MainForm.CallCommand("PluginCommand", "ResultsPanel.ShowResults");
                        }
                        else
                        {
                            sci.Text = asResultData;
                            sci.ConvertEOLs(sci.EOLMode);
                        }
                        break;

                    case TYPE_MXML:
                    case TYPE_XML:
                        MXMLPrettyPrinter mxmlPrinter = new MXMLPrettyPrinter(source);
                        FormatUtility.configureMXMLPrinter(mxmlPrinter, settingObject);
                        string mxmlResultData = mxmlPrinter.print(0);
                        if (mxmlResultData is null)
                        {
                            TraceManager.Add(TextHelper.GetString("Info.CouldNotFormat"), -3);
                            PluginBase.MainForm.CallCommand("PluginCommand", "ResultsPanel.ShowResults");
                        }
                        else
                        {
                            sci.Text = mxmlResultData;
                            sci.ConvertEOLs(sci.EOLMode);
                        }
                        break;

                    case TYPE_CPP:
                        AStyleInterface asi = new AStyleInterface();
                        string optionData = GetOptionData(sci.ConfigurationLanguage.ToLower());
                        string resultData = asi.FormatSource(source, optionData);
                        if (string.IsNullOrEmpty(resultData))
                        {
                            TraceManager.Add(TextHelper.GetString("Info.CouldNotFormat"), -3);
                            PluginBase.MainForm.CallCommand("PluginCommand", "ResultsPanel.ShowResults");
                        }
                        else
                        {
                            // Remove all empty lines if not specified for astyle
                            if (!optionData.Contains("--delete-empty-lines"))
                            {
                                resultData = Regex.Replace(resultData, @"^\s+$[\r\n]*", Environment.NewLine, RegexOptions.Multiline);
                            }
                            sci.Text = resultData;
                            sci.ConvertEOLs(sci.EOLMode);
                        }
                        break;
                }
            }
            catch (Exception)
            {
                TraceManager.Add(TextHelper.GetString("Info.CouldNotFormat"), -3);
                PluginBase.MainForm.CallCommand("PluginCommand", "ResultsPanel.ShowResults");
            }
            CurrentPos = oldPos;
            sci.EndUndoAction();
        }

        /// <summary>
        /// Get the options for the formatter based on FD settings or manual command
        /// </summary>
        string GetOptionData(string language)
        {
            string optionData;
            if (language == "cpp") optionData = settingObject.Pref_AStyle_CPP;
            else optionData = settingObject.Pref_AStyle_Others;
            if (string.IsNullOrEmpty(optionData))
            {
                int tabSize = PluginBase.Settings.TabWidth;
                bool useTabs = PluginBase.Settings.UseTabs;
                int spaceSize = PluginBase.Settings.IndentSize;
                CodingStyle codingStyle = PluginBase.Settings.CodingStyle;
                optionData = AStyleInterface.DefaultOptions + " --mode=c";
                if (language != "cpp") optionData += "s"; // --mode=cs
                if (useTabs) optionData += " --indent=force-tab=" + tabSize;
                else optionData += " --indent=spaces=" + spaceSize;
                if (codingStyle == CodingStyle.BracesAfterLine) optionData += " --style=allman";
                else optionData += " --style=attach";
            }
            return optionData;
        }
        
        /// <summary>
        /// Gets or sets the current position, ignoring whitespace
        /// </summary>
        public int CurrentPos
        {
            get
            {
                ScintillaControl sci = PluginBase.MainForm.CurrentDocument.SciControl;
                string compressedText = CompressText(sci.Text.Substring(0, sci.MBSafeCharPosition(sci.CurrentPos)));
                return compressedText.Length;
            }
            set 
            {
                bool found = false;
                ScintillaControl sci = PluginBase.MainForm.CurrentDocument.SciControl;
                string documentText = sci.Text;
                int low = 0;
                int high = documentText.Length - 1;
                while (low < high)
                {
                    var midpoint = (low + high) / 2;
                    string compressedText = CompressText(documentText.Substring(0, midpoint));
                    if (value == compressedText.Length)
                    {
                        found = true;
                        sci.SetSel(midpoint, midpoint);
                        break;
                    }
                    if (value < compressedText.Length) high = midpoint - 1;
                    else low = midpoint + 1;
                }
                if (!found) 
                {
                    sci.SetSel(documentText.Length, documentText.Length);
                }
            }
        }

        /// <summary>
        /// Gets the formatting type of the document
        /// </summary>
        public int DocumentType
        {
            get 
            {
                var sci = PluginBase.MainForm.CurrentDocument.SciControl;
                if (sci is null) return TYPE_UNKNOWN;
                var ext = Path.GetExtension(sci.FileName).ToLower();
                var lang = sci.ConfigurationLanguage.ToLower();
                if (ASContext.Context.CurrentModel.Context is { } ctx && ctx.GetType().ToString().Equals("AS3Context.Context"))
                {
                    if (ext == ".as") return TYPE_AS3;
                    if (ext == ".mxml") return TYPE_MXML;
                }
                else if (lang == "xml") return TYPE_XML;
                else if (sci.Lexer == 3 && Win32.ShouldUseWin32()) return TYPE_CPP;
                return TYPE_UNKNOWN;
            }
        }

        /// <summary>
        /// Compress text for finding correct restore position
        /// </summary>
        public string CompressText(string originalText)
        {
            var result = originalText.Replace(" ", "");
            result = result.Replace("\t", "");
            result = result.Replace("\n", "");
            result = result.Replace("\r", "");
            return result;
        }

        #endregion
    }
}