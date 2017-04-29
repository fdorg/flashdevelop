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
        private DockContent pluginPanel;
        private Settings settingObject;
        private String settingFilename;
        private PluginUI pluginUI;
        internal Image pluginImage;

        private ResultsPanelHelper panelHelper;

        // Shortcut management
        public const Keys CopyEntryKeys = Keys.Control | Keys.C;
        public const Keys IgnoreEntryKeys = Keys.Delete;

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
        public void HandleEvent(Object sender, NotifyEvent e, HandlingPriority priority)
        {
            switch (e.Type)
            {
                case EventType.Command:
                    DataEvent evnt = (DataEvent)e;
                    if (evnt.Action == "ResultsPanel.ClearResults")
                    {
                        e.Handled = true;

                        if (evnt.Data == null)
                            this.pluginUI.ClearOutput();
                        else
                            this.panelHelper.Clear(evnt.Data as string);
                    }
                    else if (evnt.Action == "ResultsPanel.ShowResults")
                    {
                        e.Handled = true;

                        if (evnt.Data == null)
                        {
                            this.pluginUI.AddLogEntries();
                            this.pluginUI.DisplayOutput();
                        }
                        else
                        {
                            this.panelHelper.ShowResults(evnt.Data as string);
                        }
                        
                    }
                    break;

                case EventType.ApplySettings:
                    this.pluginUI.ApplySettings();
                    break;

                case EventType.ProcessStart:
                    this.pluginUI.ClearOutput();
                    break;

                case EventType.ProcessEnd:
                    this.pluginUI.DisplayOutput();
                    break;

                case EventType.Trace:
                    this.panelHelper.OnTrace();
                    this.pluginUI.AddLogEntries();
                    break;

                case EventType.FileOpen:
                    TextEvent fileOpen = (TextEvent)e;
                    this.pluginUI.AddSquiggles(fileOpen.Value);
                    break;
                case EventType.UIClosing:
                    this.panelHelper.RemoveResultsPanels();
                    break;
                case EventType.Keys:
                    KeyEvent ke = (KeyEvent)e;
                    switch (PluginBase.MainForm.GetShortcutItemId(ke.Value))
                    {
                        case null:
                            break;
                        case "ResultsPanel.ShowNextResult":
                            ke.Handled = pluginUI.NextEntry();
                            break;
                        case "ResultsPanel.ShowPrevResult":
                            ke.Handled = pluginUI.PreviousEntry();
                            break;
                        case "ResultsPanel.ClearResults":
                            ke.Handled = pluginUI.ClearOutput();
                            break;
                        case "ResultsPanel.ClearIgnoredEntries":
                            ke.Handled = pluginUI.ClearIgnoredEntries();
                            break;
                        default:
                            if (ke.Value == CopyEntryKeys) ke.Handled = pluginUI.CopyTextShortcut();
                            else if (ke.Value == IgnoreEntryKeys) ke.Handled = pluginUI.IgnoreEntryShortcut();
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
                | EventType.Trace | EventType.Keys | EventType.Shortcut | EventType.ApplySettings | EventType.UIClosing;
            EventManager.AddEventHandler(this, eventMask);
        }

        /// <summary>
        /// Creates a menu item for the plugin
        /// </summary>
        public void CreateMenuItem()
        {
            String title = TextHelper.GetString("Label.ViewMenuItem");
            ToolStripMenuItem viewMenu = (ToolStripMenuItem)PluginBase.MainForm.FindMenuItem("ViewMenu");
            ToolStripMenuItem viewItem = new ToolStripMenuItem(title, this.pluginImage, new EventHandler(this.OpenPanel));
            PluginBase.MainForm.RegisterShortcutItem("ResultsPanel.ShowNextResult", this.pluginUI.nextEntryContextMenuItem);
            PluginBase.MainForm.RegisterShortcutItem("ResultsPanel.ShowPrevResult", this.pluginUI.previousEntryContextMenuItem);
            PluginBase.MainForm.RegisterShortcutItem("ResultsPanel.ClearResults", this.pluginUI.clearEntriesContextMenuItem);
            PluginBase.MainForm.RegisterShortcutItem("ResultsPanel.ClearIgnoredEntries", this.pluginUI.clearIgnoredEntriesContextMenuItem);
            PluginBase.MainForm.RegisterShortcutItem("ViewMenu.ShowResults", viewItem);
            viewMenu.DropDownItems.Add(viewItem);
        }

        /// <summary>
        /// Creates a plugin panel for the plugin
        /// </summary>
        public void CreatePluginPanel()
        {
            this.pluginUI = new PluginUI(this);
            this.pluginUI.Text = TextHelper.GetString("Title.PluginPanel");
            this.pluginPanel = PluginBase.MainForm.CreateDockablePanel(this.pluginUI, this.pluginGuid, this.pluginImage, DockState.DockBottomAutoHide);

            this.panelHelper = new ResultsPanelHelper(this);
        }
        
        /// <summary>
        /// Opens the plugin panel if closed
        /// </summary>
        public void OpenPanel(Object sender, EventArgs e)
        {
            this.pluginPanel.Show();
        }

        #endregion

    }
    
}
