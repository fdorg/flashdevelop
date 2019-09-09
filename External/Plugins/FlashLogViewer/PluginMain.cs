// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
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

namespace FlashLogViewer
{
    public class PluginMain : IPlugin
    {
        private string settingFilename;
        private Settings settingObject;
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
        public string Name { get; } = nameof(FlashLogViewer);

        /// <summary>
        /// GUID of the plugin
        /// </summary>
        public string Guid { get; } = "2cf76544-5736-11dc-8314-0800200c9a66";

        /// <summary>
        /// Author of the plugin
        /// </summary> 
        public string Author { get; } = "FlashDevelop Team";

        /// <summary>
        /// Description of the plugin
        /// </summary> 
        public string Description { get; set; } = "Adds a Flash Player main and policy log panel to FlashDevelop.";

        /// <summary>
        /// Web address for help
        /// </summary> 
        public string Help { get; } = "http://www.flashdevelop.org/community/";

        /// <summary>
        /// Object that contains the settings
        /// </summary>
        [Browsable(false)]
        public object Settings => settingObject;

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
            var startType = settingObject.TrackingStartType;
            switch (e.Type)
            {
                case EventType.UIStarted:
                {
                    if (startType == StartType.OnProgramStart)
                    {
                        pluginUI.EnableTracking(true);
                    }
                    break;
                }
                case EventType.Command:
                {
                    var de = (DataEvent)e;
                    if (startType == StartType.OnBuildComplete && de.Action == "ProjectManager.BuildComplete") 
                    {
                        pluginUI.EnableTracking(true);
                    }
                    break;
                }
            }
        }
        
        #endregion

        #region Custom Methods
       
        /// <summary>
        /// Accessor for the plugin panel
        /// </summary>
        public DockContent PluginPanel { get; set; }

        /// <summary>
        /// Initializes important variables
        /// </summary>
        public void InitBasics()
        {
            var path = Path.Combine(PathHelper.DataDir, nameof(FlashLogViewer));
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            settingFilename = Path.Combine(path, "Settings.fdb");
            Description = TextHelper.GetString("Info.Description");
            pluginImage = PluginBase.MainForm.FindImage("412");
        }

        /// <summary>
        /// Adds the required event handlers
        /// </summary> 
        public void AddEventHandlers() => EventManager.AddEventHandler(this, EventType.UIStarted | EventType.Command |EventType.ProcessEnd);

        /// <summary>
        /// Creates a menu item for the plugin and adds a ignored key
        /// </summary>
        public void CreateMenuItem()
        {
            var viewMenu = (ToolStripMenuItem)PluginBase.MainForm.FindMenuItem("ViewMenu");
            var viewItem = new ToolStripMenuItem(TextHelper.GetString("Label.ViewMenuItem"), pluginImage, OpenPanel);
            PluginBase.MainForm.RegisterShortcutItem("ViewMenu.ShowLogs", viewItem);
            viewMenu.DropDownItems.Add(viewItem);
        }  

        /// <summary>
        /// Creates a plugin panel for the plugin
        /// </summary>
        public void CreatePluginPanel()
        {
            pluginUI = new PluginUI(this) {Text = TextHelper.GetString("Title.PluginPanel")};
            PluginPanel = PluginBase.MainForm.CreateDockablePanel(pluginUI, Guid, pluginImage, DockState.DockBottomAutoHide);
        }

        /// <summary>
        /// Loads the plugin settings
        /// </summary>
        public void LoadSettings()
        {
            settingObject = new Settings();
            if (!File.Exists(settingFilename)) SaveSettings();
            else settingObject = (Settings) ObjectSerializer.Deserialize(settingFilename, settingObject);
        }

        /// <summary>
        /// Saves the plugin settings
        /// </summary>
        public void SaveSettings() => ObjectSerializer.Serialize(settingFilename, settingObject);

        /// <summary>
        /// Opens the plugin panel if closed
        /// </summary>
        public void OpenPanel(object sender, EventArgs e) => PluginPanel.Show();

        #endregion
    }
}