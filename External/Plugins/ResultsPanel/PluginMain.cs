using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using WeifenLuo.WinFormsUI.Docking;
using PluginCore.Localization;
using PluginCore.Utilities;
using PluginCore.Managers;
using PluginCore.Helpers;
using PluginCore;

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
        private Image pluginImage;

        // Shortcut management
        public Keys NextError = Keys.F12;
        public Keys PrevError = Keys.Shift | Keys.F12;
        public Keys CopyEntry = Keys.Control | Keys.C;
        public Keys IgnoreEntry = Keys.Delete;

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
        public void HandleEvent(Object sender, NotifyEvent e, HandlingPriority prority)
        {
            switch (e.Type)
            {
                case EventType.Command:
                    DataEvent evnt = (DataEvent)e;
                    if (evnt.Action == "ResultsPanel.ClearResults")
                    {
                        e.Handled = true;
                        this.pluginUI.ClearOutput();
                    }
                    else if (evnt.Action == "ResultsPanel.ShowResults")
                    {
                        e.Handled = true;
                        this.pluginUI.AddLogEntries();
                        this.pluginUI.DisplayOutput();
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
                    this.pluginUI.AddLogEntries();
                    break;

                case EventType.FileOpen:
                    TextEvent fileOpen = (TextEvent)e;
                    this.pluginUI.AddSquiggles(fileOpen.Value);
                    break;

                case EventType.Keys:
                    KeyEvent ke = (KeyEvent)e;
                    if (ke.Value == this.NextError)
                    {
                        ke.Handled = true;
                        this.pluginUI.NextEntry(null, null);
                    }
                    else if (ke.Value == this.PrevError)
                    {
                        ke.Handled = true;
                        this.pluginUI.PreviousEntry(null, null);
                    }
                    else if (ke.Value == this.CopyEntry)
                    {
                        ke.Handled = pluginUI.CopyTextShortcut();
                    }
                    else if (ke.Value == this.IgnoreEntry)
                    {
                        ke.Handled = pluginUI.IgnoreEntryShortcut();
                    }
                    break;

                case EventType.Shortcut:
                    DataEvent de = (DataEvent)e;
                    if (de.Action == "ResultsPanel.ShowNextResult")
                    {
                        this.NextError = (Keys)de.Data;
                    }
                    else if (de.Action == "ResultsPanel.ShowPrevResult")
                    {
                        this.PrevError = (Keys)de.Data;
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
            EventType eventMask = EventType.ProcessEnd | EventType.ProcessStart | EventType.FileOpen | EventType.Command | EventType.Trace | EventType.Keys | EventType.Shortcut | EventType.ApplySettings;
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
            PluginBase.MainForm.RegisterShortcutItem("ResultsPanel.ShowNextResult", this.NextError);
            PluginBase.MainForm.RegisterShortcutItem("ResultsPanel.ShowPrevResult", this.PrevError);
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
        }

        /// <summary>
        /// Opens the plugin panel if closed
        /// </summary>
        public void OpenPanel(Object sender, System.EventArgs e)
        {
            this.pluginPanel.Show();
        }

		#endregion

	}
	
}
