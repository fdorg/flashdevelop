using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using PluginCore;
using PluginCore.Helpers;
using PluginCore.Localization;
using PluginCore.Managers;
using PluginCore.Utilities;
using ResultsPanel.Helpers;
using WeifenLuo.WinFormsUI.Docking;

namespace ResultsPanel
{
    public class PluginMain : IPlugin
    {
        private String pluginName = "ResultsPanel";
        private String pluginGuid = "24df7cd8-e5f0-4171-86eb-7b2a577703ba";
        private String pluginHelp = "www.flashdevelop.org/community/";
        private String pluginDesc = "Adds a results panel for console info to FlashDevelop";
        private String pluginAuth = "FlashDevelop Team";
        //private DockContent pluginPanel;
        private Settings settingObject;
        private String settingFilename;
        internal PluginUI pluginUI;
        internal Image pluginImage;
        internal PanelContextMenu contextMenuStrip;

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
        public Object Settings
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
        }
        
        /// <summary>
        /// Handles the incoming events
        /// </summary>
        public void HandleEvent(Object sender, NotifyEvent e, HandlingPriority priority)
        {
            switch (e.Type)
            {
                case EventType.Command:
                    var de = (DataEvent) e;
                    switch (de.Action)
                    {
                        case "ResultsPanel.ClearResults":
                            de.Handled = true;
                            ResultsPanelHelper.ClearResults((string) de.Data);
                            break;

                        case "ResultsPanel.ShowResults":
                            e.Handled = true;
                            ResultsPanelHelper.ShowResults((string) de.Data);
                            break;
                    }
                    break;

                case EventType.ApplySettings:
                    ResultsPanelHelper.ApplySettings();
                    break;

                case EventType.ProcessStart:
                    this.pluginUI.ClearOutput();
                    break;

                case EventType.ProcessEnd:
                    this.pluginUI.DisplayOutput();
                    break;

                case EventType.Trace:
                    ResultsPanelHelper.OnTrace();
                    break;

                case EventType.FileOpen:
                    ResultsPanelHelper.OnFileOpen((TextEvent) e);
                    break;
                    
                case EventType.Keys:
                    KeyEvent ke = (KeyEvent) e;
                    switch (PluginBase.MainForm.GetShortcutItemId(ke.Value))
                    {
                        case null:
                            break;
                        case "ResultsPanel.ShowNextResult":
                            ke.Handled = ResultsPanelHelper.ActiveUI.NextEntry();
                            break;
                        case "ResultsPanel.ShowPrevResult":
                            ke.Handled = ResultsPanelHelper.ActiveUI.PreviousEntry();
                            break;
                        case "ResultsPanel.ClearResults":
                            ke.Handled = ResultsPanelHelper.ActiveUI.ClearOutput();
                            break;
                        case "ResultsPanel.ClearIgnoredEntries":
                            ke.Handled = ResultsPanelHelper.ActiveUI.ClearIgnoredEntries();
                            break;
                        default:
                            if (ke.Value == PanelContextMenu.CopyEntryKeys) ke.Handled = ResultsPanelHelper.ActiveUI.CopyTextShortcut();
                            else if (ke.Value == PanelContextMenu.IgnoreEntryKeys) ke.Handled = ResultsPanelHelper.ActiveUI.IgnoreEntryShortcut();
                            break;
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
            String dataDir = Path.Combine(PathHelper.DataDir, "ResultsPanel");
            if (!Directory.Exists(dataDir)) Directory.CreateDirectory(dataDir);
            this.settingFilename = Path.Combine(dataDir, "Settings.fdb");
            this.pluginDesc = TextHelper.GetString("Info.Description");
            this.pluginImage = PluginBase.MainForm.FindImage("127");
        }

        /// <summary>
        /// Saves the plugin settings
        /// </summary>
        public void SaveSettings()
        {
            ObjectSerializer.Serialize(this.settingFilename, this.settingObject);
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
        /// Adds the required event handlers
        /// </summary> 
        public void AddEventHandlers()
        {
            EventType eventMask = EventType.ProcessEnd | EventType.ProcessStart | EventType.FileOpen | EventType.Command
                | EventType.Trace | EventType.Keys | EventType.Shortcut | EventType.ApplySettings;
            EventManager.AddEventHandler(this, eventMask);
        }

        /// <summary>
        /// Creates a menu item for the plugin
        /// </summary>
        public void CreateMenuItem()
        {
            ToolStripMenuItem viewMenu = (ToolStripMenuItem)PluginBase.MainForm.FindMenuItem("ViewMenu");
            ToolStripMenuItem viewItem = new ToolStripMenuItem(TextHelper.GetString("Label.ViewMenuItem"), this.pluginImage, new EventHandler(this.OpenPanel));
            viewMenu.DropDownItems.Add(viewItem);

            this.contextMenuStrip = new PanelContextMenu();
            PluginBase.MainForm.RegisterShortcutItem("ResultsPanel.ShowNextResult", this.contextMenuStrip.NextEntry);
            PluginBase.MainForm.RegisterShortcutItem("ResultsPanel.ShowPrevResult", this.contextMenuStrip.PreviousEntry);
            PluginBase.MainForm.RegisterShortcutItem("ResultsPanel.ClearResults", this.contextMenuStrip.ClearEntries);
            PluginBase.MainForm.RegisterShortcutItem("ResultsPanel.ClearIgnoredEntries", this.contextMenuStrip.ClearIgnoredEntries);
            PluginBase.MainForm.RegisterShortcutItem("ViewMenu.ShowResults", viewItem);
        }

        /// <summary>
        /// Creates a plugin panel for the plugin
        /// </summary>
        public void CreatePluginPanel()
        {
            this.pluginUI = new PluginUI(this);
            this.pluginUI.Text = TextHelper.GetString("Title.PluginPanel");
            this.pluginUI.ParentPanel = PluginBase.MainForm.CreateDockablePanel(this.pluginUI, this.pluginGuid, this.pluginImage, DockState.DockBottomAutoHide);
            ResultsPanelHelper.Initialize(this, this.pluginUI);
        }
        
        /// <summary>
        /// Opens the plugin panel if closed
        /// </summary>
        public void OpenPanel(Object sender, EventArgs e)
        {
            ResultsPanelHelper.ActiveUI.ParentPanel.Show();
        }

        #endregion

    }
    
}
