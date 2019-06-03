using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using PluginCore;
using PluginCore.Helpers;
using PluginCore.Localization;
using PluginCore.Managers;
using PluginCore.Utilities;
using ProjectManager.Projects;

namespace AirProperties
{
    public class PluginMain : IPlugin
    {
        private ToolStripMenuItem pluginMenuItem;
        private ToolStripButton pmMenuButton;
        private string settingFilename;
        private AirWizard wizard;

        #region Required Properties

        /// <summary>
        /// Api level of the plugin
        /// </summary>
        public int Api => 1;

        /// <summary>
        /// Name of the plugin
        /// </summary> 
        public string Name { get; } = "AirProperties";

        /// <summary>
        /// GUID of the plugin
        /// </summary>
        public string Guid { get; } = "275b4759-0bc8-43bf-8b33-a69a16a9a978";

        /// <summary>
        /// Author of the plugin
        /// </summary> 
        public string Author { get; } = "FlashDevelop Team";

        /// <summary>
        /// Description of the plugin
        /// </summary> 
        public string Description { get; } = "Adds an AIR application properties management form for AIR projects.";

        /// <summary>
        /// Web address for help
        /// </summary> 
        public string Help { get; } = "http://www.flashdevelop.org/community/";

        /// <summary>
        /// Object that contains the settings
        /// </summary>
        [Browsable(false)]
        object IPlugin.Settings => Settings;

        /// <summary>
        /// Internal access to settings
        /// </summary>
        [Browsable(false)]
        public Settings Settings { get; set; }

        #endregion
        
        #region Required Methods
        
        /// <summary>
        /// Initializes the plugin
        /// </summary>
        public void Initialize()
        {
            this.InitBasics();
            this.LoadSettings();
            this.CreateMenuItems();
            this.AddEventHandlers();
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
                case EventType.Command:
                    string cmd = ((DataEvent) e).Action;
                    if (cmd == "ProjectManager.Project" || cmd == "ASCompletion.ClassPath")
                    {
                        this.UpdateMenuItems();
                    }
                    else if (cmd == "ProjectManager.Menu")
                    {
                        object menu = ((DataEvent) e).Data;
                        this.AddMenuItems(menu as ToolStripMenuItem);
                    }
                    else if (cmd == "ProjectManager.ToolBar")
                    {
                        object toolStrip = ((DataEvent) e).Data;
                        this.AddToolBarItems(toolStrip as ToolStrip);
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
            string dataPath = Path.Combine(PathHelper.DataDir, "AirProperties");
            if (!Directory.Exists(dataPath)) Directory.CreateDirectory(dataPath);
            this.settingFilename = Path.Combine(dataPath, "Settings.fdb");
        }

        /// <summary>
        /// Adds the required event handlers
        /// </summary> 
        public void AddEventHandlers()
        {
            EventManager.AddEventHandler(this, EventType.Command);
        }

        /// <summary>
        /// Create and registers the menu item
        /// </summary>
        private void CreateMenuItems()
        {
            Image image = PluginBase.MainForm.GetAutoAdjustedImage(GetImage("blockdevice_small.png"));
            this.pluginMenuItem = new ToolStripMenuItem(TextHelper.GetString("Label.ProjectMenuItem"), image, this.OpenWizard, null);
            PluginBase.MainForm.RegisterShortcutItem("ProjectMenu.AirApplicationProperties", this.pluginMenuItem);
            this.pluginMenuItem.Enabled = false;
        }

        /// <summary>
        /// Adds the necessary menu item to the project menu
        /// </summary>
        private void AddMenuItems(ToolStripMenuItem projectMenu)
        {
            projectMenu.DropDownItems.Insert(projectMenu.DropDownItems.Count - 1, this.pluginMenuItem);
        }

        /// <summary>
        /// Adds the necessary project manager toolstrip button
        /// </summary>
        private void AddToolBarItems(ToolStrip toolStrip)
        {
            this.pmMenuButton = new ToolStripButton();
            this.pmMenuButton.Image = this.pluginMenuItem.Image;
            this.pmMenuButton.Text = TextHelper.GetStringWithoutMnemonicsOrEllipsis("Label.ProjectMenuItem");
            this.pmMenuButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            this.pmMenuButton.Click += this.OpenWizard;
            PluginBase.MainForm.RegisterSecondaryItem("ProjectMenu.AirApplicationProperties", this.pmMenuButton);
            toolStrip.Items.Insert(6, this.pmMenuButton);
        }

        /// <summary>
        /// Enables or disables the menu item if the project is an AIR project
        /// </summary>
        public void UpdateMenuItems()
        {
            var pluginActive = false;
            if (pluginMenuItem == null || pmMenuButton == null) return;
            if (PluginBase.CurrentProject != null)
            {
                var project = (Project)PluginBase.CurrentProject;
                pluginActive = project.MovieOptions.Platform.StartsWithOrdinal("AIR");
            }
            pluginMenuItem.Enabled = pmMenuButton.Enabled = pluginActive;
        }
               
        /// <summary>
        /// Opens the plugin panel if closed
        /// </summary>
        public void OpenWizard(object sender, EventArgs e)
        {
            using (wizard = new AirWizard(this))
            {
                if (wizard.IsPropertiesLoaded)
                {
                    wizard.ShowDialog(PluginBase.MainForm);
                }
            }
        }

        /// <summary>
        /// Gets embedded image from resources
        /// </summary>
        public static Image GetImage(string imageName)
        {
            imageName = "AirProperties.Resources." + imageName;
            var assembly = Assembly.GetExecutingAssembly();
            return new Bitmap(assembly.GetManifestResourceStream(imageName));
        }

        /// <summary>
        /// Loads the plugin settings
        /// </summary>
        public void LoadSettings()
        {
            this.Settings = new Settings();
            if (!File.Exists(this.settingFilename)) this.SaveSettings();
            else Settings = (Settings) ObjectSerializer.Deserialize(settingFilename, Settings);
        }

        /// <summary>
        /// Saves the plugin settings
        /// </summary>
        public void SaveSettings() => ObjectSerializer.Serialize(settingFilename, Settings);

        #endregion

    }
    
}
