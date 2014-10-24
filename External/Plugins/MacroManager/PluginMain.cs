using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using PluginCore.Localization;
using PluginCore.Utilities;
using PluginCore.Managers;
using PluginCore.Helpers;
using PluginCore;

namespace MacroManager
{
	public class PluginMain : IPlugin
	{
        private String pluginName = "MacroManager";
        private String pluginGuid = "071817e0-0ee6-11de-8c30-0800200c9a66";
        private String pluginHelp = "www.flashdevelop.org/community/";
        private String pluginDesc = "Adds simple macro capacilities to FlashDevelop.";
        private String pluginAuth = "FlashDevelop Team";
        private ToolStripSeparator toolbarSeparator;
        private List<ToolStripItem> toolbarItems;
        private ToolStripMenuItem macroMenuItem;
        private ToolStripMenuItem editMenuItem;
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
            this.LoadSettings();
            this.CreateMainMenuItems();
            this.RefreshMacroToolBarItems();
            this.RefreshMacroMenuItems();
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
            if (e.Type == EventType.UIStarted)
            {
                String initScript = Path.Combine(PathHelper.BaseDir, "InitScript.cs");
                if (File.Exists(initScript))
                {
                    String command = "Internal;" + initScript;
                    PluginBase.MainForm.CallCommand("ExecuteScript", command);
                }
                this.RunAutoRunMacros();
            }
		}
		
		#endregion

        #region Custom Methods
        
        /// <summary>
        /// Accessor for the settings
        /// </summary>
        public Settings AppSettings
        {
            get { return this.settingObject; }
        }

        /// <summary>
        /// Initializes important variables
        /// </summary>
        private void InitBasics()
        {
            this.toolbarItems = new List<ToolStripItem>();
            this.toolbarSeparator = new ToolStripSeparator();
            this.pluginDesc = TextHelper.GetString("Info.Description");
            String dataPath = Path.Combine(PathHelper.DataDir, "MacroManager");
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
            this.editMenuItem = new ToolStripMenuItem(TextHelper.GetString("Label.EditMacros"), PluginBase.MainForm.FindImage("559"), this.EditMenuItemClick, Keys.Control | Keys.F10);
            PluginBase.MainForm.RegisterShortcutItem("MacrosMenu.EditMacros", this.editMenuItem);
            mainMenu.Items.Insert(mainMenu.Items.Count - 2, this.macroMenuItem);
        }

        /// <summary>
        /// Refreshes the macro related menu items
        /// </summary>
        public void RefreshMacroMenuItems()
        {
            this.macroMenuItem.DropDownItems.Clear();
            foreach (Macro macro in this.settingObject.UserMacros)
            {
                if (!macro.AutoRun)
                {
                    ToolStripMenuItem macroItem = new ToolStripMenuItem();
                    macroItem.Click += new EventHandler(this.MacroMenuItemClick);
                    macroItem.ShortcutKeys = macro.Shortcut;
                    macroItem.Text = macro.Label;
                    macroItem.Tag = macro;
                    if (!String.IsNullOrEmpty(macro.Image))
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
            foreach (Macro macro in this.settingObject.UserMacros)
            {
                if (!macro.AutoRun && macro.ShowInToolbar)
                {
                    ToolStripButton macroButton = new ToolStripButton();
                    macroButton.Click += new EventHandler(this.MacroMenuItemClick);
                    macroButton.ToolTipText = macro.Label.Replace("&", "");
                    macroButton.Tag = macro;
                    if (!String.IsNullOrEmpty(macro.Image))
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
            Int32 index = toolStrip.Items.IndexOf(this.toolbarSeparator);
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
            this.settingObject = new Settings();
            if (!File.Exists(this.settingFilename)) this.SaveSettings();
            else
            {
                Object obj = ObjectSerializer.Deserialize(this.settingFilename, this.settingObject);
                this.settingObject = (Settings)obj;
            }
            if (this.settingObject.UserMacros.Count == 0)
            {
                Macro execScript = new Macro("&Execute Script", new String[1] { "ExecuteScript|Development;$(OpenFile)" }, String.Empty, Keys.None);
                Macro execCommand = new Macro("E&xecute Command", new String[1] { "#$$(Command=RunProcess)|$$(Arguments=cmd.exe)" }, String.Empty, Keys.None);
                Macro execfCommand = new Macro("Execu&te Current File", new String[1] { "RunProcess|$(CurFile)" }, String.Empty, Keys.None);
                Macro runSelected = new Macro("Execute &Selected Text", new String[1] { "RunProcess|$(SelText)" }, String.Empty, Keys.None);
                Macro browseSelected = new Macro("&Browse Current File", new String[1] { "Browse|$(CurFile)" }, String.Empty, Keys.None);
                Macro copyTextAsRtf = new Macro("&Copy Text As RTF", new String[1] { "ScintillaCommand|CopyRTF" }, String.Empty, Keys.None);
                this.settingObject.UserMacros.Add(execScript);
                this.settingObject.UserMacros.Add(execCommand);
                this.settingObject.UserMacros.Add(execfCommand);
                this.settingObject.UserMacros.Add(runSelected);
                this.settingObject.UserMacros.Add(browseSelected);
                this.settingObject.UserMacros.Add(copyTextAsRtf);
            }
        }

        /// <summary>
        /// Runs the macros that have autorun enabled
        /// </summary>
        private void RunAutoRunMacros()
        {
            try
            {
                foreach (Macro macro in this.settingObject.UserMacros)
                {
                    if (macro.AutoRun)
                    {
                        foreach (String entry in macro.Entries)
                        {
                            String[] parts = entry.Split(new Char[1] { '|' });
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
        private void MacroMenuItemClick(Object sender, EventArgs e)
        {
            try
            {
                ToolStripItem macroItem = sender as ToolStripItem;
                foreach (String entry in ((Macro)macroItem.Tag).Entries)
                {
                    String data = entry;
                    if (data.StartsWith("#")) // Hardcore mode :)
                    {
                        data = PluginBase.MainForm.ProcessArgString(entry.Substring(1));
                        if (data == "|") return; // Invalid, don't execute..
                    }
                    if (data.IndexOf('|') != -1)
                    {
                        String[] parts = data.Split(new Char[1] { '|' });
                        PluginBase.MainForm.CallCommand(parts[0], parts[1]);
                    }
                    else PluginBase.MainForm.CallCommand(data, "");
                }
            }
            catch (Exception)
            {
                String message = TextHelper.GetString("Info.CouldNotRunMacro");
                ErrorManager.ShowWarning(message, null);
            }
        }

        /// <summary>
        /// Opens the macro manager dialog
        /// </summary>
        private void EditMenuItemClick(Object sender, EventArgs e)
        {
            ManagerDialog.Show(this);
        }

        /// <summary>
        /// Saves the plugin settings
        /// </summary>
        private void SaveSettings()
        {
            ObjectSerializer.Serialize(this.settingFilename, this.settingObject);
        }

		#endregion

    }
        
    #region Custom Types

    [Serializable]
    public class Macro
    {
        private Keys shortcut = Keys.None;
        private String image = String.Empty;
        private String label = String.Empty;
        private String[] entries = new String[0];
        private Boolean showInToolbar = false;
        private Boolean autoRun = false;

        public Macro(){}
        public Macro(String label, String[] entries, String image, Keys shortcut)
        {
            this.Label = label;
            this.image = image;
            this.entries = entries;
            this.shortcut = shortcut;
            this.showInToolbar = false;
            this.autoRun = false;
        }
        public Macro(String label, String[] entries, String image, Keys shortcut, Boolean autoRun, Boolean showInToolbar) 
        {
            this.Label = label;
            this.image = image;
            this.entries = entries;
            this.shortcut = shortcut;
            this.showInToolbar = showInToolbar;
            this.autoRun = autoRun;
        }

        /// <summary>
        /// Gets and sets the label
        /// </summary>
        [LocalizedDescription("MacroManager.Description.Label")]
        public String Label
        {
            get { return this.label; }
            set { this.label = value; }
        }

        /// <summary>
        /// Gets and sets the image
        /// </summary>
        [LocalizedDescription("MacroManager.Description.Image")]
        public String Image
        {
            get { return this.image; }
            set { this.image = value; }
        }

        /// <summary>
        /// Gets and sets the entries
        /// </summary>
        [LocalizedDescription("MacroManager.Description.Entries")]
        public String[] Entries
        {
            get { return this.entries; }
            set { this.entries = value; }
        }

        /// <summary>
        /// Gets and sets the showInToolbar
        /// </summary>
        [DisplayName("Show In Toolbar")]
        [LocalizedDescription("MacroManager.Description.ShowInToolbar")]
        public Boolean ShowInToolbar
        {
            get { return this.showInToolbar; }
            set { this.showInToolbar = value; }
        }

        /// <summary>
        /// Gets and sets the autoRun
        /// </summary>
        [LocalizedDescription("MacroManager.Description.AutoRun")]
        public Boolean AutoRun
        {
            get { return this.autoRun; }
            set { this.autoRun = value; }
        }

        /// <summary>
        /// Gets and sets the shortcut
        /// </summary>
        [LocalizedDescription("MacroManager.Description.Shortcut")]
        public Keys Shortcut
        {
            get { return this.shortcut; }
            set { this.shortcut = value; }
        }

    }

    #endregion
	
}