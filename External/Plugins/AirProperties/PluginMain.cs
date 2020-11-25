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
        ToolStripMenuItem pluginMenuItem;
        ToolStripButton pmMenuButton;
        string settingFilename;

        #region Required Properties

        /// <summary>
        /// Api level of the plugin
        /// </summary>
        public int Api => 1;

        /// <summary>
        /// Name of the plugin
        /// </summary> 
        public string Name { get; } = nameof(AirProperties);

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
            InitBasics();
            LoadSettings();
            CreateMenuItems();
            AddEventHandlers();
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
                    var cmd = ((DataEvent) e).Action;
                    if (cmd == "ProjectManager.Project" || cmd == "ASCompletion.ClassPath")
                    {
                        UpdateMenuItems();
                    }
                    else if (cmd == "ProjectManager.Menu")
                    {
                        AddMenuItems(((DataEvent) e).Data as ToolStripMenuItem);
                    }
                    else if (cmd == "ProjectManager.ToolBar")
                    {
                        AddToolBarItems(((DataEvent) e).Data as ToolStrip);
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
            var path = Path.Combine(PathHelper.DataDir, nameof(AirProperties));
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            settingFilename = Path.Combine(path, "Settings.fdb");
        }

        /// <summary>
        /// Adds the required event handlers
        /// </summary> 
        public void AddEventHandlers() => EventManager.AddEventHandler(this, EventType.Command);

        /// <summary>
        /// Create and registers the menu item
        /// </summary>
        void CreateMenuItems()
        {
            var image = PluginBase.MainForm.GetAutoAdjustedImage(GetImage("blockdevice_small.png"));
            pluginMenuItem = new ToolStripMenuItem(TextHelper.GetString("Label.ProjectMenuItem"), image, OpenWizard, null);
            PluginBase.MainForm.RegisterShortcutItem("ProjectMenu.AirApplicationProperties", pluginMenuItem);
            pluginMenuItem.Enabled = false;
        }

        /// <summary>
        /// Adds the necessary menu item to the project menu
        /// </summary>
        void AddMenuItems(ToolStripDropDownItem menu) => menu.DropDownItems.Insert(menu.DropDownItems.Count - 1, pluginMenuItem);

        /// <summary>
        /// Adds the necessary project manager toolstrip button
        /// </summary>
        void AddToolBarItems(ToolStrip toolStrip)
        {
            pmMenuButton = new ToolStripButton();
            pmMenuButton.Image = pluginMenuItem.Image;
            pmMenuButton.Text = TextHelper.GetStringWithoutMnemonicsOrEllipsis("Label.ProjectMenuItem");
            pmMenuButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            pmMenuButton.Click += OpenWizard;
            PluginBase.MainForm.RegisterSecondaryItem("ProjectMenu.AirApplicationProperties", pmMenuButton);
            toolStrip.Items.Insert(6, pmMenuButton);
        }

        /// <summary>
        /// Enables or disables the menu item if the project is an AIR project
        /// </summary>
        public void UpdateMenuItems()
        {
            if (pluginMenuItem is null || pmMenuButton is null) return;
            var pluginActive = false;
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
            using var wizard = new AirWizard(this);
            if (wizard.IsPropertiesLoaded)
            {
                wizard.ShowDialog(PluginBase.MainForm);
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
            Settings = new Settings();
            if (!File.Exists(settingFilename)) SaveSettings();
            else Settings = ObjectSerializer.Deserialize(settingFilename, Settings);
        }

        /// <summary>
        /// Saves the plugin settings
        /// </summary>
        public void SaveSettings() => ObjectSerializer.Serialize(settingFilename, Settings);

        #endregion
    }
}