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
        private string settingFilename;
        private PluginUI pluginUI;
        private Image pluginImage;

        #region Required Properties

        /// <summary>
        /// Api level of the plugin
        /// </summary>
        public int Api => 1;

        /// <summary>
        /// Name of the plugin
        /// </summary> 
        public string Name { get; } = "OutputPanel";

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
        public object Settings => this.PluginSettings;

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
            this.CreatePluginPanel();
            this.CreateMenuItem();
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
                case EventType.ProcessStart:
                    if (this.PluginSettings.ClearMode == ClearModeAction.OnEveryProcess)
                    {
                        this.pluginUI.ClearOutput(null, null);
                    }
                    break;

                case EventType.ProcessEnd:
                    if (this.PluginSettings.ShowOnProcessEnd && !this.PluginSettings.ShowOnOutput)
                    {
                        this.pluginUI.DisplayOutput();
                    }
                    break;

                case EventType.Trace:
                    this.pluginUI.AddTraces();
                    if (this.PluginSettings.ShowOnOutput)
                    {
                        this.pluginUI.DisplayOutput();
                    }
                    break;

                case EventType.Keys:
                    Keys keys = ((KeyEvent) e).Value;
                    e.Handled = this.pluginUI.OnShortcut(keys);
                    break;

                case EventType.ApplySettings:
                    this.pluginUI.ApplyWrapText();
                    break;

                case EventType.UIStarted:
                    this.pluginUI.UpdateAfterTheme();
                    break;

                case EventType.Command:
                    var dataEvent = e as DataEvent;
                    if (dataEvent.Action == "ProjectManager.BuildingProject" && this.PluginSettings.ClearMode == ClearModeAction.OnBuildStart)
                    {
                        this.pluginUI.ClearOutput(null, null);
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
            string dataPath = Path.Combine(PathHelper.DataDir, "OutputPanel");
            if (!Directory.Exists(dataPath)) Directory.CreateDirectory(dataPath);
            this.settingFilename = Path.Combine(dataPath, "Settings.fdb");
            this.Description = TextHelper.GetString("Info.Description");
            this.pluginImage = PluginBase.MainForm.FindImage("50");
        }

        /// <summary>
        /// Adds the required event handlers
        /// </summary>
        public void AddEventHandlers()
        {
            var eventMask = EventType.ProcessStart | EventType.ProcessEnd | EventType.Trace | EventType.ApplySettings | EventType.Keys | EventType.UIStarted | EventType.Command;
            EventManager.AddEventHandler(this, eventMask);
        }

        /// <summary>
        /// Creates a menu item for the plugin
        /// </summary>
        public void CreateMenuItem()
        {
            string label = TextHelper.GetString("Label.ViewMenuItem");
            ToolStripMenuItem viewMenu = (ToolStripMenuItem)PluginBase.MainForm.FindMenuItem("ViewMenu");
            ToolStripMenuItem viewItem = new ToolStripMenuItem(label, this.pluginImage, this.OpenPanel);
            PluginBase.MainForm.RegisterShortcutItem("ViewMenu.ShowOutput", viewItem);
            viewMenu.DropDownItems.Add(viewItem);
        }

        /// <summary>
        /// Creates a plugin panel for the plugin
        /// </summary>
        public void CreatePluginPanel()
        {
            this.pluginUI = new PluginUI(this);
            this.pluginUI.Text = TextHelper.GetString("Title.PluginPanel");
            this.PluginPanel = PluginBase.MainForm.CreateDockablePanel(this.pluginUI, this.Guid, this.pluginImage, DockState.DockBottom);
        }

        /// <summary>
        /// Loads the plugin settings
        /// </summary>
        public void LoadSettings()
        {
            this.PluginSettings = new Settings();
            if (!File.Exists(this.settingFilename)) this.SaveSettings();
            else
            {
                object obj = ObjectSerializer.Deserialize(this.settingFilename, this.PluginSettings);
                this.PluginSettings = (Settings)obj;
            }
        }

        /// <summary>
        /// Saves the plugin settings
        /// </summary>
        public void SaveSettings() => ObjectSerializer.Serialize(this.settingFilename, this.PluginSettings);

        /// <summary>
        /// Opens the plugin panel if closed
        /// </summary>
        public void OpenPanel(object sender, EventArgs e) => this.PluginPanel.Show();

        #endregion
        
    }
    
}
