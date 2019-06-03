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
        public string Name { get; } = "BookmarkPanel";

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
            this.InitBasics();
            this.AddEventHandlers();
            this.CreatePluginPanel();
            this.CreateMenuItem();
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
                    if (e is TextEvent fo) this.pluginUI.CreateDocument(fo.Value);
                    break;

                case EventType.FileClose:
                    if (e is TextEvent fc) this.pluginUI.CloseDocument(fc.Value);
                    break;

                case EventType.ApplySettings:
                    this.pluginUI.UpdateSettings();
                    break;

                case EventType.FileEmpty:
                    this.pluginUI.CloseAll();
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
            this.pluginImage = PluginBase.MainForm.FindImage("402");
        }

        /// <summary>
        /// Adds the required event handlers
        /// </summary> 
        public void AddEventHandlers()
        {
            EventManager.AddEventHandler(this, EventType.FileOpen | EventType.FileClose | EventType.FileEmpty | EventType.ApplySettings);
        }

        /// <summary>
        /// Creates a menu item for the plugin and adds a ignored key
        /// </summary>
        public void CreateMenuItem()
        {
            ToolStripMenuItem viewMenu = (ToolStripMenuItem)PluginBase.MainForm.FindMenuItem("ViewMenu");
            ToolStripMenuItem viewItem = new ToolStripMenuItem(TextHelper.GetString("Label.ViewMenuItem"), this.pluginImage, this.OpenPanel);
            PluginBase.MainForm.RegisterShortcutItem("ViewMenu.ShowBookmarks", viewItem);
            viewMenu.DropDownItems.Add(viewItem);
        }

        /// <summary>
        /// Creates a plugin panel for the plugin
        /// </summary>
        public void CreatePluginPanel()
        {
            this.pluginUI = new PluginUI(this);
            this.pluginUI.Text = TextHelper.GetString("Title.PluginPanel");
            this.pluginPanel = PluginBase.MainForm.CreateDockablePanel(this.pluginUI, this.Guid, this.pluginImage, DockState.DockRight);
        }

        /// <summary>
        /// Opens the plugin panel if closed
        /// </summary>
        public void OpenPanel(object sender, EventArgs e)
        {
            this.pluginPanel.Show();
        }

        #endregion

    }
    
}
