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
        private ToolStripMenuItem contextMenuItem;
        private ToolStripMenuItem mainMenuItem;
        private string settingFilename;
        private Settings settingObject;

        #region Required Properties

        /// <summary>
        /// Api level of the plugin
        /// </summary>
        public int Api => 1;

        /// <summary>
        /// Name of the plugin
        /// </summary>
        public string Name { get; } = "CodeFormatter";

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
        public object Settings => this.settingObject;

        #endregion
        
        #region Required Methods
        
        /// <summary>
        /// Initializes the plugin
        /// </summary>
        public void Initialize()
        {
            this.InitBasics();
            this.CreateMainMenuItem();
            this.CreateContextMenuItem();
            this.LoadSettings();
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
            switch (e.Type)
            {
                case EventType.FileSwitch:
                    this.UpdateMenuItems();
                    break;

                case EventType.Command:
                    DataEvent de = (DataEvent)e;
                    if (de.Action == "CodeRefactor.Menu")
                    {
                        this.AttachMainMenuItem((ToolStripMenuItem)de.Data);
                        this.UpdateMenuItems();
                    }
                    else if (de.Action == "CodeRefactor.ContextMenu")
                    {
                        this.AttachContextMenuItem((ToolStripMenuItem)de.Data);
                        this.UpdateMenuItems();
                    }
                    else if (de.Action == "CodeFormatter.FormatDocument")
                    {
                        this.DoFormat((ITabbedDocument)de.Data);
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
            EventManager.AddEventHandler(this, EventType.Command);
            EventManager.AddEventHandler(this, EventType.FileSwitch);
            string dataPath = Path.Combine(PathHelper.DataDir, "CodeFormatter");
            if (!Directory.Exists(dataPath)) Directory.CreateDirectory(dataPath);
            this.settingFilename = Path.Combine(dataPath, "Settings.fdb");
            this.Description = TextHelper.GetString("Info.Description");
        }

        /// <summary>
        /// Update the menu items if they are available
        /// </summary>
        private void UpdateMenuItems()
        {
            if (this.mainMenuItem == null || this.contextMenuItem == null) return;
            ITabbedDocument doc = PluginBase.MainForm.CurrentDocument;
            bool isValid = doc != null && doc.IsEditable && this.DocumentType != TYPE_UNKNOWN;
            this.mainMenuItem.Enabled = this.contextMenuItem.Enabled = isValid;
        }

        /// <summary>
        /// Creates a menu item for the plugin
        /// </summary>
        public void CreateMainMenuItem()
        {
            string label = TextHelper.GetString("Label.CodeFormatter");
            this.mainMenuItem = new ToolStripMenuItem(label, null, this.Format, Keys.Control | Keys.Shift | Keys.D2);
            PluginBase.MainForm.RegisterShortcutItem("RefactorMenu.CodeFormatter", this.mainMenuItem);
        }
        private void AttachMainMenuItem(ToolStripMenuItem mainMenu)
        {
            mainMenu.DropDownItems.Insert(7, this.mainMenuItem);
        }

        /// <summary>
        /// Creates a context menu item for the plugin
        /// </summary>
        public void CreateContextMenuItem()
        {
            string label = TextHelper.GetString("Label.CodeFormatter");
            this.contextMenuItem = new ToolStripMenuItem(label, null, this.Format, Keys.None);
            PluginBase.MainForm.RegisterSecondaryItem("RefactorMenu.CodeFormatter", this.contextMenuItem);
        }
        public void AttachContextMenuItem(ToolStripMenuItem contextMenu)
        {
            contextMenu.DropDownItems.Insert(6, this.contextMenuItem);
        }

        /// <summary>
        /// Loads the plugin settings
        /// </summary>
        public void LoadSettings()
        {
            this.settingObject = new Settings();
            if (!File.Exists(this.settingFilename)) 
            {
                this.settingObject.InitializeDefaultPreferences();
                this.SaveSettings();
            }
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

        #endregion

        #region Code Formatting

        private const int TYPE_AS3 = 0;
        private const int TYPE_MXML = 1;
        private const int TYPE_XML = 2;
        private const int TYPE_CPP = 3;
        private const int TYPE_UNKNOWN = 4;
        
        /// <summary>
        /// Formats the current document
        /// </summary>
        public void Format(object sender, EventArgs e)
        {
            this.DoFormat(PluginBase.MainForm.CurrentDocument);
        }

        /// <summary>
        /// Formats the specified document
        /// </summary>
        private void DoFormat(ITabbedDocument doc)
        {
            if (doc.IsEditable)
            {
                doc.SciControl.BeginUndoAction();
                int oldPos = CurrentPos;
                string source = doc.SciControl.Text;
                try
                {
                    switch (DocumentType)
                    {
                        case TYPE_AS3:
                            ASPrettyPrinter asPrinter = new ASPrettyPrinter(true, source);
                            FormatUtility.configureASPrinter(asPrinter, this.settingObject);
                            string asResultData = asPrinter.print(0);
                            if (asResultData == null)
                            {
                                TraceManager.Add(TextHelper.GetString("Info.CouldNotFormat"), -3);
                                PluginBase.MainForm.CallCommand("PluginCommand", "ResultsPanel.ShowResults");
                            }
                            else
                            {
                                doc.SciControl.Text = asResultData;
                                doc.SciControl.ConvertEOLs(doc.SciControl.EOLMode);
                            }
                            break;

                        case TYPE_MXML:
                        case TYPE_XML:
                            MXMLPrettyPrinter mxmlPrinter = new MXMLPrettyPrinter(source);
                            FormatUtility.configureMXMLPrinter(mxmlPrinter, this.settingObject);
                            string mxmlResultData = mxmlPrinter.print(0);
                            if (mxmlResultData == null)
                            {
                                TraceManager.Add(TextHelper.GetString("Info.CouldNotFormat"), -3);
                                PluginBase.MainForm.CallCommand("PluginCommand", "ResultsPanel.ShowResults");
                            }
                            else
                            {
                                doc.SciControl.Text = mxmlResultData;
                                doc.SciControl.ConvertEOLs(doc.SciControl.EOLMode);
                            }
                            break;

                        case TYPE_CPP:
                            AStyleInterface asi = new AStyleInterface();
                            string optionData = this.GetOptionData(doc.SciControl.ConfigurationLanguage.ToLower());
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
                                doc.SciControl.Text = resultData;
                                doc.SciControl.ConvertEOLs(doc.SciControl.EOLMode);
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
                doc.SciControl.EndUndoAction();
            }
        }

        /// <summary>
        /// Get the options for the formatter based on FD settings or manual command
        /// </summary>
        private string GetOptionData(string language)
        {
            string optionData;
            if (language == "cpp") optionData = this.settingObject.Pref_AStyle_CPP;
            else optionData = this.settingObject.Pref_AStyle_Others;
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
                    else if (value < compressedText.Length) high = midpoint - 1;
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
                ITabbedDocument document = PluginBase.MainForm.CurrentDocument;
                if (!document.IsEditable) return TYPE_UNKNOWN;
                string ext = Path.GetExtension(document.FileName).ToLower();
                string lang = document.SciControl.ConfigurationLanguage.ToLower();
                if (ASContext.Context.CurrentModel.Context != null && ASContext.Context.CurrentModel.Context.GetType().ToString().Equals("AS3Context.Context")) 
                {
                    if (ext == ".as") return TYPE_AS3;
                    if (ext == ".mxml") return TYPE_MXML;
                }
                else if (lang == "xml") return TYPE_XML;
                else if (document.SciControl.Lexer == 3 && Win32.ShouldUseWin32()) return TYPE_CPP;
                return TYPE_UNKNOWN;
            }
        }

        /// <summary>
        /// Compress text for finding correct restore position
        /// </summary>
        public string CompressText(string originalText)
        {
            string compressedText = originalText.Replace(" ", "");
            compressedText = compressedText.Replace("\t", "");
            compressedText = compressedText.Replace("\n", "");
            compressedText = compressedText.Replace("\r", "");
            return compressedText;
        }

        #endregion

    }
    
}