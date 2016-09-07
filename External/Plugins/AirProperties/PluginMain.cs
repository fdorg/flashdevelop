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
        private String pluginName = "AirProperties";
        private String pluginGuid = "275b4759-0bc8-43bf-8b33-a69a16a9a978";
        private String pluginDesc = "Adds an AIR application properties management form for AIR projects.";
        private String pluginHelp = "http://www.flashdevelop.org/community/";
        private String pluginAuth = "FlashDevelop Team";
        private ToolStripMenuItem pluginMenuItem;
        private ToolStripButton pmMenuButton;
        private String settingFilename;
        private Settings settingObject;
        private AirWizard wizard;

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
        Object IPlugin.Settings
        {
            get { return this.settingObject; }
        }

        /// <summary>
        /// Internal access to settings
        /// </summary>
        [Browsable(false)]
        public Settings Settings
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
            this.CreateMenuItems();
            this.AddEventHandlers();
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
                case EventType.Command:
                    String cmd = (e as DataEvent).Action;
                    if (cmd == "ProjectManager.Project" || cmd == "ASCompletion.ClassPath")
                    {
                        this.UpdateMenuItems();
                    }
                    else if (cmd == "ProjectManager.Menu")
                    {
                        Object menu = (e as DataEvent).Data;
                        this.AddMenuItems(menu as ToolStripMenuItem);
                    }
                    else if (cmd == "ProjectManager.ToolBar")
                    {
                        Object toolStrip = (e as DataEvent).Data;
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
            String dataPath = Path.Combine(PathHelper.DataDir, "AirProperties");
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
            Boolean pluginActive = false;
            if (this.pluginMenuItem == null || this.pmMenuButton == null) return;
            if (PluginBase.CurrentProject != null)
            {
                Project project = (Project)PluginBase.CurrentProject;
                pluginActive = project.MovieOptions.Platform.StartsWithOrdinal("AIR");
            }
            this.pluginMenuItem.Enabled = this.pmMenuButton.Enabled = pluginActive;
        }
               
        /// <summary>
        /// Opens the plugin panel if closed
        /// </summary>
        public void OpenWizard(Object sender, EventArgs e)
        {
            this.wizard = new AirWizard(this);
            if (this.wizard.IsPropertiesLoaded)
            {
                this.wizard.ShowDialog(PluginBase.MainForm);
            }
        }

        /// <summary>
        /// Gets embedded image from resources
        /// </summary>
        public static Image GetImage(String imageName)
        {
            imageName = "AirProperties.Resources." + imageName;
            Assembly assembly = Assembly.GetExecutingAssembly();
            return new Bitmap(assembly.GetManifestResourceStream(imageName));
        }

        /// <summary>
        /// Loads the plugin settings
        /// </summary>
        public void LoadSettings()
        {
            this.settingObject = new Settings();
            if (!File.Exists(this.settingFilename)) this.SaveSettings();
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

    }
    
}
