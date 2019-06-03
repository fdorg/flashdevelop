using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using PluginCore;
using PluginCore.Helpers;
using PluginCore.Localization;
using PluginCore.Managers;
using PluginCore.Utilities;
using WeifenLuo.WinFormsUI.Docking;

namespace TaskListPanel
{
    public class PluginMain : IPlugin
    {
        private string settingFilename;
        private Settings settingObject;
        private DockContent pluginPanel;
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
        public string Name { get; } = "TaskListPanel";

        /// <summary>
        /// GUID of the plugin
        /// </summary>
        public string Guid { get; } = "40feac2b-a68a-498e-ad78-52a8268efa45";

        /// <summary>
        /// Author of the plugin
        /// </summary> 
        public string Author { get; } = "FlashDevelop Team";

        /// <summary>
        /// Description of the plugin
        /// </summary> 
        public string Description { get; set; } = "Adds a task list panel to FlashDevelop.";

        /// <summary>
        /// Web address for help
        /// </summary> 
        public string Help { get; } = "www.flashdevelop.org/community/";

        /// <summary>
        /// Object that contains the settings
        /// </summary>
        public object Settings => this.settingObject;

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
            this.CreateMenuItem();
            this.CreatePluginPanel();
        }
        
        /// <summary>
        /// Disposes the plugin
        /// </summary>
        public void Dispose()
        {
            this.SaveSettings();
            this.pluginUI.Terminate();
        }
        
        /// <summary>
        /// Handles the incoming events
        /// </summary>
        public void HandleEvent(object sender, NotifyEvent e, HandlingPriority priority)
        {
            switch (e.Type)
            {
                case EventType.UIStarted:
                    EventManager.AddEventHandler(this, EventType.Command);
                    break;

                case EventType.ApplySettings:
                    this.pluginUI.UpdateSettings();
                    break;

                case EventType.Command:
                    DataEvent de = (DataEvent)e;
                    if (de.Action == "ProjectManager.Project")
                    {
                        this.pluginUI.InitProject();
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
            this.Description = TextHelper.GetString("Info.Description");
            string dataPath = Path.Combine(PathHelper.DataDir, "TaskListPanel");
            if (!Directory.Exists(dataPath)) Directory.CreateDirectory(dataPath);
            this.settingFilename = Path.Combine(dataPath, "Settings.fdb");
            this.pluginImage = PluginBase.MainForm.FindImage("75");
        }

        /// <summary>
        /// Adds the required event handlers
        /// </summary> 
        public void AddEventHandlers() => EventManager.AddEventHandler(this, EventType.UIStarted | EventType.ApplySettings);

        /// <summary>
        /// Creates a plugin panel for the plugin
        /// </summary>
        public void CreatePluginPanel()
        {
            this.pluginUI = new PluginUI(this);
            this.pluginUI.Text = TextHelper.GetString("Title.PluginPanel");
            this.pluginPanel = PluginBase.MainForm.CreateDockablePanel(this.pluginUI, this.Guid, this.pluginImage, DockState.DockBottomAutoHide);
        }

        /// <summary>
        /// Creates a menu item for the plugin and adds a ignored key
        /// </summary>
        public void CreateMenuItem()
        {
            ToolStripMenuItem viewMenu = (ToolStripMenuItem)PluginBase.MainForm.FindMenuItem("ViewMenu");
            ToolStripMenuItem viewItem = new ToolStripMenuItem(TextHelper.GetString("Label.ViewMenuItem"), this.pluginImage, new EventHandler(this.OpenPanel), null);
            PluginBase.MainForm.RegisterShortcutItem("ViewMenu.ShowTasks", viewItem);
            viewMenu.DropDownItems.Add(viewItem);
        }

        /// <summary>
        /// Loads the plugin settings
        /// </summary>
        public void LoadSettings()
        {
            this.settingObject = new Settings();
            if (!File.Exists(this.settingFilename)) this.SaveSettings();
            else settingObject = (Settings) ObjectSerializer.Deserialize(settingFilename, settingObject);
        }

        /// <summary>
        /// Saves the plugin settings
        /// </summary>
        public void SaveSettings() => ObjectSerializer.Serialize(settingFilename, settingObject);

        /// <summary>
        /// Opens the plugin panel if closed
        /// </summary>
        public void OpenPanel(object sender, EventArgs e) => pluginPanel.Show();

        #endregion

    }
    
}
