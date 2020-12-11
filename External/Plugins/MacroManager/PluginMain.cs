// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
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
        ToolStripSeparator toolbarSeparator;
        List<ToolStripItem> toolbarItems;
        ToolStripMenuItem macroMenuItem;
        ToolStripMenuItem editMenuItem;
        string settingFilename;

        #region Required Properties

        /// <summary>
        /// Api level of the plugin
        /// </summary>
        public int Api => 1;

        /// <summary>
        /// Name of the plugin
        /// </summary> 
        public string Name { get; } = nameof(MacroManager);

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
            InitBasics();
            LoadSettings();
            CreateMainMenuItems();
            RefreshMacroToolBarItems();
            RefreshMacroMenuItems();
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
            if (e.Type != EventType.UIStarted) return;
            var initScript = Path.Combine(PathHelper.BaseDir, "InitScript.cs");
            if (File.Exists(initScript))
            {
                var command = $"Internal;{initScript}";
                PluginBase.MainForm.CallCommand("ExecuteScript", command);
            }
            var autoImport = Path.Combine(PathHelper.BaseDir, "InitMacros.fdm");
            if (File.Exists(autoImport))
            {
                var macros = new List<Macro>();
                var macrosObject = ObjectSerializer.Deserialize(autoImport, macros, false);
                macros = macrosObject;
                AppSettings.UserMacros.AddRange(macros);
                try { File.Delete(autoImport); }
                catch (Exception ex)
                {
                    ErrorManager.ShowError("Could not delete import file: " + autoImport, ex);
                }
                RefreshMacroMenuItems();
            }
            RunAutoRunMacros();
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
        void InitBasics()
        {
            toolbarItems = new List<ToolStripItem>();
            toolbarSeparator = new ToolStripSeparator();
            Description = TextHelper.GetString("Info.Description");
            var path = Path.Combine(PathHelper.DataDir, nameof(MacroManager));
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            settingFilename = Path.Combine(path, "Settings.fdb");
            EventManager.AddEventHandler(this, EventType.UIStarted);
        }

        /// <summary>
        /// Creates the nesessary main menu item
        /// </summary>
        void CreateMainMenuItems()
        {
            var mainMenu = PluginBase.MainForm.MenuStrip;
            macroMenuItem = new ToolStripMenuItem(TextHelper.GetString("Label.Macros"));
            editMenuItem = new ToolStripMenuItem(TextHelper.GetString("Label.EditMacros"), null, EditMenuItemClick, Keys.Control | Keys.F11);
            PluginBase.MainForm.RegisterShortcutItem("MacrosMenu.EditMacros", editMenuItem);
            mainMenu.Items.Insert(mainMenu.Items.Count - 2, macroMenuItem);
        }

        /// <summary>
        /// Refreshes the macro related menu items
        /// </summary>
        public void RefreshMacroMenuItems()
        {
            macroMenuItem.DropDownItems.Clear();
            foreach (Macro macro in AppSettings.UserMacros)
            {
                if (!macro.AutoRun)
                {
                    ToolStripMenuItem macroItem = new ToolStripMenuItem();
                    macroItem.Click += MacroMenuItemClick;
                    macroItem.ShortcutKeys = macro.Shortcut;
                    macroItem.Text = macro.Label;
                    macroItem.Tag = macro;
                    if (!string.IsNullOrEmpty(macro.Image))
                    {
                        macroItem.Image = PluginBase.MainForm.FindImage(macro.Image);
                    }
                    macroMenuItem.DropDownItems.Add(macroItem);
                    if (!PluginBase.MainForm.IgnoredKeys.Contains(macro.Shortcut))
                    {
                        PluginBase.MainForm.IgnoredKeys.Add(macro.Shortcut);
                    }
                }
            }
            macroMenuItem.DropDownItems.Add(new ToolStripSeparator());
            macroMenuItem.DropDownItems.Add(editMenuItem);
        }

        /// <summary>
        /// Refreshes the macro related toolbar items
        /// </summary>
        public void RefreshMacroToolBarItems()
        {
            var toolStrip = PluginBase.MainForm.ToolStrip;
            if (!toolStrip.Items.Contains(toolbarSeparator)) toolStrip.Items.Add(toolbarSeparator);
            foreach (ToolStripItem item in toolbarItems)
            {
                toolStrip.Items.Remove(item);
            }
            toolbarItems.Clear();
            foreach (Macro macro in AppSettings.UserMacros)
            {
                if (!macro.AutoRun && macro.ShowInToolbar)
                {
                    ToolStripButton macroButton = new ToolStripButton();
                    macroButton.Click += MacroMenuItemClick;
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
                    toolbarItems.Add(macroButton);
                }
            }
            int index = toolStrip.Items.IndexOf(toolbarSeparator);
            toolbarSeparator.Visible = toolbarItems.Count > 0;
            foreach (ToolStripItem item in toolbarItems)
            {
                toolStrip.Items.Insert(index + 1, item);
            }
        }

        /// <summary>
        /// Loads the plugin settings
        /// </summary>
        void LoadSettings()
        {
            AppSettings = new Settings();
            if (!File.Exists(settingFilename)) SaveSettings();
            else AppSettings = ObjectSerializer.Deserialize(settingFilename, AppSettings);
            AppSettings.UserMacros.RemoveAll(it => it.Label.IsNullOrEmpty() && it.Entries.IsNullOrEmpty());
            if (AppSettings.UserMacros.All(it => it.Entries?[0] != "ExecuteScript|Development;$(OpenFile)"))
                AppSettings.UserMacros.Add(new Macro("&Execute Script", new[] {"ExecuteScript|Development;$(OpenFile)"}));
            if (AppSettings.UserMacros.All(it => it.Entries?[0] != "#$$(Command=RunProcess)|$$(Arguments=cmd.exe)"))
                AppSettings.UserMacros.Add(new Macro("E&xecute Command", new[] { "#$$(Command=RunProcess)|$$(Arguments=cmd.exe)" }));
            if (AppSettings.UserMacros.All(it => it.Entries?[0] != "RunProcess|$(CurFile)"))
                AppSettings.UserMacros.Add(new Macro("Execu&te Current File", new[] { "RunProcess|$(CurFile)" }));
            if (AppSettings.UserMacros.All(it => it.Entries?[0] != "RunProcess|$(SelText)"))
                AppSettings.UserMacros.Add(new Macro("Execute &Selected Text", new[] { "RunProcess|$(SelText)" }));
            if (AppSettings.UserMacros.All(it => it.Entries?[0] != "Browse|$(CurFile)"))
                AppSettings.UserMacros.Add(new Macro("&Browse Current File", new[] { "Browse|$(CurFile)" }));
            if (AppSettings.UserMacros.All(it => it.Entries?[0] != "ScintillaCommand|CopyRTF"))
                AppSettings.UserMacros.Add(new Macro("&Copy Text As RTF", new[] { "ScintillaCommand|CopyRTF" }));
        }

        /// <summary>
        /// Runs the macros that have autorun enabled
        /// </summary>
        void RunAutoRunMacros()
        {
            try
            {
                foreach (var macro in AppSettings.UserMacros)
                {
                    if (!macro.AutoRun) continue;
                    foreach (var entry in macro.Entries)
                    {
                        var parts = entry.Split('|');
                        PluginBase.MainForm.CallCommand(parts[0], parts[1]);
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
        static void MacroMenuItemClick(object sender, EventArgs e)
        {
            try
            {
                var entries = ((Macro)((ToolStripItem) sender).Tag).Entries;
                if (!entries.IsNullOrEmpty())
                {
                    foreach (var entry in entries)
                    {
                        var data = entry;
                        if (data.StartsWith('#')) // Hardcore mode :)
                        {
                            data = PluginBase.MainForm.ProcessArgString(entry.Substring(1));
                            if (data == "|") return; // Invalid, don't execute..
                        }
                        if (data.Contains('|'))
                        {
                            var parts = data.Split('|');
                            PluginBase.MainForm.CallCommand(parts[0], parts[1]);
                        }
                        else PluginBase.MainForm.CallCommand(data, "");
                    }
                    return;
                }
            }
            catch
            {
            }
            var message = TextHelper.GetString("Info.CouldNotRunMacro");
            ErrorManager.ShowWarning(message, null);
        }

        /// <summary>
        /// Opens the macro manager dialog
        /// </summary>
        void EditMenuItemClick(object sender, EventArgs e) => ManagerDialog.Show(this);

        /// <summary>
        /// Saves the plugin settings
        /// </summary>
        void SaveSettings() => ObjectSerializer.Serialize(settingFilename, AppSettings);

        #endregion

    }
        
    #region Custom Types

    [Serializable]
    public class Macro
    {
        public Macro(){}

        public Macro(string label, string[] entries) : this(label, entries, string.Empty, Keys.None, false, false)
        {
        }

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
        public string[] Entries { get; set; } = Array.Empty<string>();

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
        public override string ToString() => nameof(Macro);
    }

    #endregion
}