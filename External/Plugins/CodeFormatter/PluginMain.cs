﻿using System;
using System.ComponentModel;
using System.IO;
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
        private String pluginName = "CodeFormatter";
        private String pluginGuid = "f7f1e15b-282a-4e55-ba58-5f2c02765247";
        private String pluginDesc = "Adds multiple code formatters to FlashDevelop.";
        private String pluginHelp = "www.flashdevelop.org/community/";
        private String pluginAuth = "FlashDevelop Team";
        private ToolStripMenuItem contextMenuItem;
        private ToolStripMenuItem mainMenuItem;
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
        public void HandleEvent(Object sender, NotifyEvent e, HandlingPriority priority)
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
                        ToolStripMenuItem mainMenu = (ToolStripMenuItem)de.Data;
                        this.AttachMainMenuItem(mainMenu);
                        this.UpdateMenuItems();
                    }
                    else if (de.Action == "CodeRefactor.ContextMenu")
                    {
                        ToolStripMenuItem contextMenu = (ToolStripMenuItem)de.Data;
                        this.AttachContextMenuItem(contextMenu);
                        this.UpdateMenuItems();
                    }
                    else if (de.Action == "CodeFormatter.FormatDocument")
                    {
                        ITabbedDocument document = (ITabbedDocument)de.Data;
                        this.DoFormat(document);
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
            String dataPath = Path.Combine(PathHelper.DataDir, "CodeFormatter");
            if (!Directory.Exists(dataPath)) Directory.CreateDirectory(dataPath);
            this.settingFilename = Path.Combine(dataPath, "Settings.fdb");
            this.pluginDesc = TextHelper.GetString("Info.Description");
        }

        /// <summary>
        /// Update the menu items if they are available
        /// </summary>
        private void UpdateMenuItems()
        {
            if (this.mainMenuItem == null || this.contextMenuItem == null) return;
            ITabbedDocument doc = PluginBase.MainForm.CurrentDocument;
            Boolean isValid = doc != null && doc.IsEditable && this.DocumentType != TYPE_UNKNOWN;
            this.mainMenuItem.Enabled = this.contextMenuItem.Enabled = isValid;
        }

        /// <summary>
        /// Creates a menu item for the plugin
        /// </summary>
        public void CreateMainMenuItem()
        {
            String label = TextHelper.GetString("Label.CodeFormatter");
            this.mainMenuItem = new ToolStripMenuItem(label, null, new EventHandler(this.Format), Keys.Control | Keys.Shift | Keys.D2);
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
            String label = TextHelper.GetString("Label.CodeFormatter");
            this.contextMenuItem = new ToolStripMenuItem(label, null, new EventHandler(this.Format), Keys.None);
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

        #region Code Formatting

        private const int TYPE_AS3 = 0;
        private const int TYPE_MXML = 1;
        private const int TYPE_XML = 2;
        private const int TYPE_CPP = 3;
        private const int TYPE_UNKNOWN = 4;

        /// <summary>
        /// Formats the current document
        /// </summary>
        public void Format(Object sender, EventArgs e)
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
                Int32 oldPos = CurrentPos;
                String source = doc.SciControl.Text;
                try
                {
                    switch (DocumentType)
                    {
                        case TYPE_AS3:
                            ASPrettyPrinter asPrinter = new ASPrettyPrinter(true, source);
                            FormatUtility.configureASPrinter(asPrinter, this.settingObject);
                            String asResultData = asPrinter.print(0);
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
                            String mxmlResultData = mxmlPrinter.print(0);
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
                            String optionData;
                            if (doc.SciControl.ConfigurationLanguage == "haxe")
                            {
                                optionData = HaxeAStyleHelper.GetAStyleArguments(this.settingObject);
                            }
                            else
                            {
                                optionData = this.GetOptionData(doc.SciControl.ConfigurationLanguage.ToLower());
                            }
                            String resultData = asi.FormatSource(source, optionData);
                            if (String.IsNullOrEmpty(resultData))
                            {
                                TraceManager.Add(TextHelper.GetString("Info.CouldNotFormat"), -3);
                                PluginBase.MainForm.CallCommand("PluginCommand", "ResultsPanel.ShowResults");
                            }
                            else
                            {
                                // Remove all empty lines if not specified for astyle
                                // Why? Commented out for now, as it conflicts with HaxeAStyleDialog
                                //if (!optionData.Contains("--delete-empty-lines"))
                                //{
                                //    resultData = Regex.Replace(resultData, @"^\s+$[\r\n]*", Environment.NewLine, RegexOptions.Multiline);
                                //}
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
        private String GetOptionData(String language)
        {
            String optionData;
            if (language == "cpp") optionData = this.settingObject.Pref_AStyle_CPP;
            else optionData = this.settingObject.Pref_AStyle_Others;
            if (String.IsNullOrEmpty(optionData))
            {
                Int32 tabSize = PluginBase.Settings.TabWidth;
                Boolean useTabs = PluginBase.Settings.UseTabs;
                Int32 spaceSize = PluginBase.Settings.IndentSize;
                CodingStyle codingStyle = PluginBase.Settings.CodingStyle;
                optionData = AStyleInterface.DefaultOptions + " --mode=c";
                if (language != "cpp") optionData += "s"; // --mode=cs
                if (useTabs) optionData += " --indent=force-tab=" + tabSize.ToString();
                else optionData += " --indent=spaces=" + spaceSize.ToString();
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
                String compressedText = CompressText(sci.Text.Substring(0, sci.MBSafeCharPosition(sci.CurrentPos)));
                return compressedText.Length;
            }
            set 
            {
                Boolean found = false;
                ScintillaControl sci = PluginBase.MainForm.CurrentDocument.SciControl;
                String documentText = sci.Text;
                Int32 low = 0; Int32 midpoint = 0;
                Int32 high = documentText.Length - 1;
                while (low < high)
                {
                    midpoint = (low + high) / 2;
                    String compressedText = CompressText(documentText.Substring(0, midpoint));
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
        public Int32 DocumentType
        {
            get 
            {
                ITabbedDocument document = PluginBase.MainForm.CurrentDocument;
                if (!document.IsEditable) return TYPE_UNKNOWN;
                String ext = Path.GetExtension(document.FileName).ToLower();
                String lang = document.SciControl.ConfigurationLanguage.ToLower();
                if (ASContext.Context.CurrentModel.Context != null && ASContext.Context.CurrentModel.Context.GetType().ToString().Equals("AS3Context.Context")) 
                {
                    if (ext == ".as") return TYPE_AS3;
                    else if (ext == ".mxml") return TYPE_MXML;
                }
                else if (lang == "xml") return TYPE_XML;
                else if (document.SciControl.Lexer == 3 && Win32.ShouldUseWin32()) return TYPE_CPP;
                return TYPE_UNKNOWN;
            }
        }

        /// <summary>
        /// Compress text for finding correct restore position
        /// </summary>
        public String CompressText(String originalText)
        {
            String compressedText = originalText.Replace(" ", "");
            compressedText = compressedText.Replace("\t", "");
            compressedText = compressedText.Replace("\n", "");
            compressedText = compressedText.Replace("\r", "");
            return compressedText;
        }

        #endregion

    }
    
}