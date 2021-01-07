using System;
using System.Drawing;
using System.Windows.Forms;
using PluginCore;
using PluginCore.Localization;
using PluginCore.Managers;
using WeifenLuo.WinFormsUI.Docking;

namespace BookmarkPanel
{
    public class PluginMain : IPlugin
    {
        DockContent pluginPanel;
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
        public string Name { get; } = nameof(BookmarkPanel);

        /// <summary>
        /// GUID of the plugin
        /// </summary>
        public string Guid { get; } = "9b79609e-2b05-4e88-9430-21713aafc827";

        /// <summary>
        /// Author of the plugin
        /// </summary> 
        public string Author { get; } = "FlashDevelop Team";

        /// <summary>
        /// Description of the plugin
        /// </summary> 
        public string Description { get; set; } = "Adds a bookmark management panel to FlashDevelop.";

        /// <summary>
        /// Web address for help
        /// </summary> 
        public string Help { get; } = "www.flashdevelop.org/community/";

        /// <summary>
        /// Object that contains the settings
        /// </summary>
        public object Settings => null;

        #endregion
        
        #region Required Methods
        
        /// <summary>
        /// Initializes the plugin
        /// </summary>
        public void Initialize()
        {
            InitBasics();
            AddEventHandlers();
            CreatePluginPanel();
            CreateMenuItem();
        }
        
        /// <summary>
        /// Disposes the plugin
        /// </summary>
        public void Dispose()
        {
            // Nothing here...
        }
        
        /// <summary>
        /// Handles the incoming events
        /// </summary>
        public void HandleEvent(object sender, NotifyEvent e, HandlingPriority priority)
        {
            switch (e.Type)
            {
                case EventType.FileOpen:
                    if (e is TextEvent fo) pluginUI.CreateDocument(fo.Value);
                    break;

                case EventType.FileClose:
                    if (e is TextEvent fc) pluginUI.CloseDocument(fc.Value);
                    break;

                case EventType.ApplySettings:
                    pluginUI.UpdateSettings();
                    break;

                case EventType.FileEmpty:
                    pluginUI.CloseAll();
                    break;
            }

        }
        
        #endregion

        #region Custom Methods

        /// <summary>
        /// Initializes important variables
        /// </summary>
        void InitBasics()
        {
            Description = TextHelper.GetString("Info.Description");
            pluginImage = PluginBase.MainForm.FindImage("402");
        }

        /// <summary>
        /// Adds the required event handlers
        /// </summary> 
        void AddEventHandlers() => EventManager.AddEventHandler(this, EventType.FileOpen | EventType.FileClose | EventType.FileEmpty | EventType.ApplySettings);

        /// <summary>
        /// Creates a menu item for the plugin and adds a ignored key
        /// </summary>
        void CreateMenuItem()
        {
            var viewMenu = (ToolStripMenuItem)PluginBase.MainForm.FindMenuItem("ViewMenu");
            var viewItem = new ToolStripMenuItem(TextHelper.GetString("Label.ViewMenuItem"), pluginImage, OpenPanel);
            PluginBase.MainForm.RegisterShortcutItem("ViewMenu.ShowBookmarks", viewItem);
            viewMenu.DropDownItems.Add(viewItem);
        }

        /// <summary>
        /// Creates a plugin panel for the plugin
        /// </summary>
        void CreatePluginPanel()
        {
            pluginUI = new PluginUI {Text = TextHelper.GetString("Title.PluginPanel")};
            pluginPanel = PluginBase.MainForm.CreateDockablePanel(pluginUI, Guid, pluginImage, DockState.DockRight);
        }

        /// <summary>
        /// Opens the plugin panel if closed
        /// </summary>
        void OpenPanel(object sender, EventArgs e) => pluginPanel.Show();

        #endregion
    }
}