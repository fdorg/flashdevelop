using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using PluginCore;
using PluginCore.Helpers;
using PluginCore.Localization;
using PluginCore.Managers;
using PluginCore.Utilities;

namespace MacroManager
{
    public class PluginMain : IPlugin
    {
        private ToolStripSeparator toolbarSeparator;
        private List<ToolStripItem> toolbarItems;
        private ToolStripMenuItem macroMenuItem;
        private ToolStripMenuItem editMenuItem;
        private string settingFilename;

        #region Required Properties

        /// <summary>
        /// Api level of the plugin
        /// </summary>
        public int Api => 1;

        /// <summary>
        /// Name of the plugin
        /// </summary> 
        public string Name { get; } = "MacroManager";

        /// <summary>
        /// GUID of the plugin
        /// </summary>
        public string Guid { get; } = "071817e0-0ee6-11de-8c30-0800200c9a66";

        /// <summary>
        /// Author of the plugin
        /// </summary> 
        public string Author { get; } = "FlashDevelop Team";

        /// <summary>
        /// Description of the plugin
        /// </summary> 
        public string Description { get; set; } = "Adds simple macro capacilities to FlashDevelop.";

        /// <summary>
        /// Web address for help
        /// </summary> 
        public string Help { get; } = "www.flashdevelop.org/community/";

        /// <summary>
        /// Object that contains the settings
        /// </summary>
        [Browsable(false)]
        public object Settings => AppSettings;

        #endregion
        
        #region Required Methods
        
        /// <summary>
        /// Initializes the plugin
        /// </summary>
        public void Initialize()
        {
            this.InitBasics();
            this.LoadSettings();
            this.CreateMainMenuItems();
            this.RefreshMacroToolBarItems();
            this.RefreshMacroMenuItems();
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
            if (e.Type == EventType.UIStarted)
            {
                string initScript = Path.Combine(PathHelper.BaseDir, "InitScript.cs");
                string autoImport = Path.Combine(PathHelper.BaseDir, "InitMacros.fdm");
                if (File.Exists(initScript))
                {
                    string command = "Internal;" + initScript;
                    PluginBase.MainForm.CallCommand("ExecuteScript", command);
                }
                if (File.Exists(autoImport))
                {
                    List<Macro> macros = new List<Macro>();
                    object macrosObject = ObjectSerializer.Deserialize(autoImport, macros, false);
                    macros = (List<Macro>)macrosObject;
                    this.AppSettings.UserMacros.AddRange(macros);
                    try { File.Delete(autoImport); }
                    catch (Exception ex)
                    {
                        ErrorManager.ShowError("Could not delete import file: " + autoImport, ex);
                    }
                    this.RefreshMacroMenuItems();
                }
                this.RunAutoRunMacros();
            }
        }

        #endregion

        #region Custom Methods

        /// <summary>
        /// Accessor for the settings
        /// </summary>
        public Settings AppSettings { get; private set; }

        /// <summary>
        /// Initializes important variables
        /// </summary>
        private void InitBasics()
        {
            this.toolbarItems = new List<ToolStripItem>();
            this.toolbarSeparator = new ToolStripSeparator();
            this.Description = TextHelper.GetString("Info.Description");
            string dataPath = Path.Combine(PathHelper.DataDir, "MacroManager");
            if (!Directory.Exists(dataPath)) Directory.CreateDirectory(dataPath);
            this.settingFilename = Path.Combine(dataPath, "Settings.fdb");
            EventManager.AddEventHandler(this, EventType.UIStarted);
        }

        /// <summary>
        /// Creates the nesessary main menu item
        /// </summary>
        private void CreateMainMenuItems()
        {
            MenuStrip mainMenu = PluginBase.MainForm.MenuStrip;
            this.macroMenuItem = new ToolStripMenuItem(TextHelper.GetString("Label.Macros"));
            this.editMenuItem = new ToolStripMenuItem(TextHelper.GetString("Label.EditMacros"), null, this.EditMenuItemClick, Keys.Control | Keys.F11);
            PluginBase.MainForm.RegisterShortcutItem("MacrosMenu.EditMacros", this.editMenuItem);
            mainMenu.Items.Insert(mainMenu.Items.Count - 2, this.macroMenuItem);
        }

        /// <summary>
        /// Refreshes the macro related menu items
        /// </summary>
        public void RefreshMacroMenuItems()
        {
            this.macroMenuItem.DropDownItems.Clear();
            foreach (Macro macro in this.AppSettings.UserMacros)
            {
                if (!macro.AutoRun)
                {
                    ToolStripMenuItem macroItem = new ToolStripMenuItem();
                    macroItem.Click += this.MacroMenuItemClick;
                    macroItem.ShortcutKeys = macro.Shortcut;
                    macroItem.Text = macro.Label;
                    macroItem.Tag = macro;
                    if (!string.IsNullOrEmpty(macro.Image))
                    {
                        macroItem.Image = PluginBase.MainForm.FindImage(macro.Image);
                    }
                    this.macroMenuItem.DropDownItems.Add(macroItem);
                    if (!PluginBase.MainForm.IgnoredKeys.Contains(macro.Shortcut))
                    {
                        PluginBase.MainForm.IgnoredKeys.Add(macro.Shortcut);
                    }
                }
            }
            this.macroMenuItem.DropDownItems.Add(new ToolStripSeparator());
            this.macroMenuItem.DropDownItems.Add(this.editMenuItem);
        }

        /// <summary>
        /// Refreshes the macro related toolbar items
        /// </summary>
        public void RefreshMacroToolBarItems()
        {
            ToolStrip toolStrip = PluginBase.MainForm.ToolStrip;
            if (!toolStrip.Items.Contains(this.toolbarSeparator))
            {
                toolStrip.Items.Add(this.toolbarSeparator);
            }
            foreach (ToolStripItem item in this.toolbarItems)
            {
                toolStrip.Items.Remove(item);
            }
            this.toolbarItems.Clear();
            foreach (Macro macro in this.AppSettings.UserMacros)
            {
                if (!macro.AutoRun && macro.ShowInToolbar)
                {
                    ToolStripButton macroButton = new ToolStripButton();
                    macroButton.Click += this.MacroMenuItemClick;
                    macroButton.ToolTipText = TextHelper.RemoveMnemonics(macro.Label);
                    macroButton.Tag = macro;
                    if (!string.IsNullOrEmpty(macro.Image))
                    {
                        macroButton.Image = PluginBase.MainForm.FindImage(macro.Image);
                    }
                    else macroButton.Image = PluginBase.MainForm.FindImage("528|13|0|0");
                    if (!PluginBase.MainForm.IgnoredKeys.Contains(macro.Shortcut))
                    {
                        PluginBase.MainForm.IgnoredKeys.Add(macro.Shortcut);
                    }
                    this.toolbarItems.Add(macroButton);
                }
            }
            int index = toolStrip.Items.IndexOf(this.toolbarSeparator);
            this.toolbarSeparator.Visible = this.toolbarItems.Count > 0;
            foreach (ToolStripItem item in this.toolbarItems)
            {
                toolStrip.Items.Insert(index + 1, item);
            }
        }

        /// <summary>
        /// Loads the plugin settings
        /// </summary>
        private void LoadSettings()
        {
            this.AppSettings = new Settings();
            if (!File.Exists(this.settingFilename)) this.SaveSettings();
            else
            {
                object obj = ObjectSerializer.Deserialize(this.settingFilename, this.AppSettings);
                this.AppSettings = (Settings)obj;
            }
            if (this.AppSettings.UserMacros.Count == 0)
            {
                Macro execScript = new Macro("&Execute Script", new string[1] { "ExecuteScript|Development;$(OpenFile)" }, string.Empty, Keys.None);
                Macro execCommand = new Macro("E&xecute Command", new string[1] { "#$$(Command=RunProcess)|$$(Arguments=cmd.exe)" }, string.Empty, Keys.None);
                Macro execfCommand = new Macro("Execu&te Current File", new string[1] { "RunProcess|$(CurFile)" }, string.Empty, Keys.None);
                Macro runSelected = new Macro("Execute &Selected Text", new string[1] { "RunProcess|$(SelText)" }, string.Empty, Keys.None);
                Macro browseSelected = new Macro("&Browse Current File", new string[1] { "Browse|$(CurFile)" }, string.Empty, Keys.None);
                Macro copyTextAsRtf = new Macro("&Copy Text As RTF", new string[1] { "ScintillaCommand|CopyRTF" }, string.Empty, Keys.None);
                this.AppSettings.UserMacros.Add(execScript);
                this.AppSettings.UserMacros.Add(execCommand);
                this.AppSettings.UserMacros.Add(execfCommand);
                this.AppSettings.UserMacros.Add(runSelected);
                this.AppSettings.UserMacros.Add(browseSelected);
                this.AppSettings.UserMacros.Add(copyTextAsRtf);
            }
        }

        /// <summary>
        /// Runs the macros that have autorun enabled
        /// </summary>
        private void RunAutoRunMacros()
        {
            try
            {
                foreach (Macro macro in this.AppSettings.UserMacros)
                {
                    if (macro.AutoRun)
                    {
                        foreach (string entry in macro.Entries)
                        {
                            string[] parts = entry.Split('|');
                            PluginBase.MainForm.CallCommand(parts[0], parts[1]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorManager.AddToLog("Macro AutoRun Failed.", ex);
            }
        }

        /// <summary>
        /// Executes the clicked macro
        /// </summary>
        private void MacroMenuItemClick(object sender, EventArgs e)
        {
            try
            {
                foreach (string entry in ((Macro)((ToolStripItem) sender).Tag).Entries)
                {
                    string data = entry;
                    if (data.StartsWith('#')) // Hardcore mode :)
                    {
                        data = PluginBase.MainForm.ProcessArgString(entry.Substring(1));
                        if (data == "|") return; // Invalid, don't execute..
                    }
                    if (data.Contains('|'))
                    {
                        string[] parts = data.Split('|');
                        PluginBase.MainForm.CallCommand(parts[0], parts[1]);
                    }
                    else PluginBase.MainForm.CallCommand(data, "");
                }
            }
            catch (Exception)
            {
                string message = TextHelper.GetString("Info.CouldNotRunMacro");
                ErrorManager.ShowWarning(message, null);
            }
        }

        /// <summary>
        /// Opens the macro manager dialog
        /// </summary>
        private void EditMenuItemClick(object sender, EventArgs e) => ManagerDialog.Show(this);

        /// <summary>
        /// Saves the plugin settings
        /// </summary>
        private void SaveSettings() => ObjectSerializer.Serialize(settingFilename, AppSettings);

        #endregion

    }
        
    #region Custom Types

    [Serializable]
    public class Macro
    {
        public Macro(){}

        public Macro(string label, string[] entries, string image, Keys shortcut) : this(label, entries, image, shortcut, false, false)
        {
        }

        public Macro(string label, string[] entries, string image, Keys shortcut, bool autoRun, bool showInToolbar) 
        {
            Label = label;
            Image = image;
            Entries = entries;
            Shortcut = shortcut;
            ShowInToolbar = showInToolbar;
            AutoRun = autoRun;
        }

        /// <summary>
        /// Gets and sets the label
        /// </summary>
        [LocalizedDescription("MacroManager.Description.Label")]
        public string Label { get; set; } = string.Empty;

        /// <summary>
        /// Gets and sets the image
        /// </summary>
        [LocalizedDescription("MacroManager.Description.Image")]
        public string Image { get; set; } = string.Empty;

        /// <summary>
        /// Gets and sets the entries
        /// </summary>
        [LocalizedDescription("MacroManager.Description.Entries")]
        public string[] Entries { get; set; } = new string[0];

        /// <summary>
        /// Gets and sets the showInToolbar
        /// </summary>
        [DisplayName("Show In Toolbar")]
        [LocalizedDescription("MacroManager.Description.ShowInToolbar")]
        public bool ShowInToolbar { get; set; }

        /// <summary>
        /// Gets and sets the autoRun
        /// </summary>
        [LocalizedDescription("MacroManager.Description.AutoRun")]
        public bool AutoRun { get; set; }

        /// <summary>
        /// Gets and sets the shortcut
        /// </summary>
        [LocalizedDescription("MacroManager.Description.Shortcut")]
        public Keys Shortcut { get; set; } = Keys.None;

        /// <summary>
        /// Use shorten name for the macro item
        /// </summary>
        public override string ToString() => "Macro";
    }

    #endregion
    
}