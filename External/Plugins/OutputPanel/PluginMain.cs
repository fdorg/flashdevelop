// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using PluginCore;
using PluginCore.Helpers;
using PluginCore.Localization;
using PluginCore.Managers;
using PluginCore.Utilities;
using WeifenLuo.WinFormsUI.Docking;

namespace OutputPanel
{
    public class PluginMain : IPlugin
    {
        string settingFilename;
        PluginUI pluginUI;
        Image pluginImage;

        #region Required Properties

        /// <summary>
        /// Api level of the plugin
        /// </summary>
        public int Api => 1;

        /// <summary>
        /// Name of the plugin
        /// </summary> 
        public string Name { get; } = nameof(OutputPanel);

        /// <summary>
        /// GUID of the plugin
        /// </summary>
        public string Guid { get; } = "54749f71-694b-47e0-9b05-e9417f39f20d";

        /// <summary>
        /// Author of the plugin
        /// </summary> 
        public string Author { get; } = "FlashDevelop Team";

        /// <summary>
        /// Description of the plugin
        /// </summary> 
        public string Description { get; private set; } = "Adds a output panel for debug messages to FlashDevelop.";

        /// <summary>
        /// Web address for help
        /// </summary> 
        public string Help { get; } = "www.flashdevelop.org/community/";

        /// <summary>
        /// Object that contains the settings
        /// </summary>
        [Browsable(false)]
        public object Settings => PluginSettings;

        #endregion
        
        #region Required Methods
        
        /// <summary>
        /// Initializes the plugin
        /// </summary>
        public void Initialize()
        {
            InitBasics();
            LoadSettings();
            AddEventHandlers();
            CreatePluginPanel();
            CreateMenuItem();
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
                case EventType.ProcessStart:
                    if (PluginSettings.ClearMode == ClearModeAction.OnEveryProcess)
                    {
                        pluginUI.ClearOutput(null, null);
                    }
                    break;

                case EventType.ProcessEnd:
                    if (PluginSettings.ShowOnProcessEnd && !PluginSettings.ShowOnOutput)
                    {
                        pluginUI.DisplayOutput();
                    }
                    break;

                case EventType.Trace:
                    pluginUI.AddTraces();
                    if (PluginSettings.ShowOnOutput)
                    {
                        pluginUI.DisplayOutput();
                    }
                    break;

                case EventType.Keys:
                    e.Handled = pluginUI.OnShortcut(((KeyEvent) e).Value);
                    break;

                case EventType.ApplySettings:
                    pluginUI.ApplyWrapText();
                    break;

                case EventType.UIStarted:
                    pluginUI.UpdateAfterTheme();
                    break;

                case EventType.Command:
                    var de = (DataEvent) e;
                    if (de.Action == "ProjectManager.BuildingProject" && PluginSettings.ClearMode == ClearModeAction.OnBuildStart)
                    {
                        pluginUI.ClearOutput(null, null);
                    }
                    break;
            }
        }

        #endregion

        #region Custom Properties

        /// <summary>
        /// Gets the PluginPanel
        /// </summary>
        [Browsable(false)]
        public DockContent PluginPanel { get; private set; }

        /// <summary>
        /// Gets the PluginSettings
        /// </summary>
        [Browsable(false)]
        public Settings PluginSettings { get; private set; }

        #endregion

        #region Custom Methods

        /// <summary>
        /// Initializes important variables
        /// </summary>
        public void InitBasics()
        {
            var path = Path.Combine(PathHelper.DataDir, nameof(OutputPanel));
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            settingFilename = Path.Combine(path, "Settings.fdb");
            Description = TextHelper.GetString("Info.Description");
            pluginImage = PluginBase.MainForm.FindImage("50");
        }

        /// <summary>
        /// Adds the required event handlers
        /// </summary>
        public void AddEventHandlers()
        {
            const EventType eventMask = EventType.ProcessStart | EventType.ProcessEnd | EventType.Trace | EventType.ApplySettings | EventType.Keys | EventType.UIStarted | EventType.Command;
            EventManager.AddEventHandler(this, eventMask);
        }

        /// <summary>
        /// Creates a menu item for the plugin
        /// </summary>
        public void CreateMenuItem()
        {
            var label = TextHelper.GetString("Label.ViewMenuItem");
            var viewMenu = (ToolStripMenuItem)PluginBase.MainForm.FindMenuItem("ViewMenu");
            var viewItem = new ToolStripMenuItem(label, pluginImage, OpenPanel);
            PluginBase.MainForm.RegisterShortcutItem("ViewMenu.ShowOutput", viewItem);
            viewMenu.DropDownItems.Add(viewItem);
        }

        /// <summary>
        /// Creates a plugin panel for the plugin
        /// </summary>
        public void CreatePluginPanel()
        {
            pluginUI = new PluginUI(this) {Text = TextHelper.GetString("Title.PluginPanel")};
            PluginPanel = PluginBase.MainForm.CreateDockablePanel(pluginUI, Guid, pluginImage, DockState.DockBottom);
        }

        /// <summary>
        /// Loads the plugin settings
        /// </summary>
        public void LoadSettings()
        {
            PluginSettings = new Settings();
            if (!File.Exists(settingFilename)) SaveSettings();
            else PluginSettings = (Settings) ObjectSerializer.Deserialize(settingFilename, PluginSettings);
        }

        /// <summary>
        /// Saves the plugin settings
        /// </summary>
        public void SaveSettings() => ObjectSerializer.Serialize(settingFilename, PluginSettings);

        /// <summary>
        /// Opens the plugin panel if closed
        /// </summary>
        public void OpenPanel(object sender, EventArgs e) => PluginPanel.Show();

        #endregion
    }
}