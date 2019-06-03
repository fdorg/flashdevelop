using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Web;
using System.Windows.Forms;
using PluginCore;
using PluginCore.Helpers;
using PluginCore.Localization;
using PluginCore.Managers;
using PluginCore.Utilities;
using ProjectManager;
using StartPage.Controls;
using WeifenLuo.WinFormsUI.Docking;

namespace StartPage
{
    public class PluginMain : IPlugin
    {
        private bool justOpened = true;
        private string defaultRssUrl = "";
        private string defaultStartPageUrl = "";
        private StartPageWebBrowser startPageWebBrowser;
        private string settingFilename;
        private Settings settingObject;
        private DockContent startPage;
        private Image pluginImage;

        #region Required Properties

        /// <summary>
        /// Api level of the plugin
        /// </summary>
        public int Api => 1;

        /// <summary>
        /// Name of the plugin
        /// </summary> 
        public string Name { get; } = "StartPage";

        /// <summary>
        /// GUID of the plugin
        /// </summary>
        public string Guid { get; } = "e4246322-bc55-4f4a-99c8-aaeeed0a7b9a";

        /// <summary>
        /// Author of the plugin
        /// </summary> 
        public string Author { get; } = "FlashDevelop Team";

        /// <summary>
        /// Description of the plugin
        /// </summary> 
        public string Description { get; set; } = "Adds a start page to FlashDevelop.";

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
                case EventType.Command :
                    DataEvent de = (DataEvent)e;
                    switch (de.Action)
                    {
                        case ProjectManagerEvents.Project :
                            // Close pluginPanel if the user has the setting checked and a project is opened
                            if (de.Data != null && this.startPage != null)
                            {
                                if (this.settingObject.CloseOnProjectOpen) this.startPage.Close();
                                // The project manager does not update recent projects until after
                                // it broadcasts this event so we'll wait a little bit before refreshing
                                Timer timer = new Timer();
                                timer.Interval = 100;
                                timer.Tick += delegate { timer.Stop(); if (!this.startPageWebBrowser.IsDisposed) this.startPageWebBrowser.SendProjectInfo(); };
                                timer.Start();
                            }
                            break;
                    }
                    break;

                case EventType.FileEmpty :
                    if ((this.justOpened & (this.settingObject.ShowStartPageOnStartup == ShowStartPageOnStartupEnum.Always || this.settingObject.ShowStartPageOnStartup == ShowStartPageOnStartupEnum.NotRestoringSession)) || this.settingObject.ShowStartPageInsteadOfUntitled)
                    {
                        this.ShowStartPage();
                        this.justOpened = false;
                        e.Handled = true;
                    }
                    break;

                case EventType.RestoreSession:
                    var session = (ISession) ((DataEvent)e).Data;
                    if (session.Type != SessionType.Startup) return; // handle only startup sessions
                    if (this.settingObject.ShowStartPageOnStartup == ShowStartPageOnStartupEnum.Always) e.Handled = true;
                    else if (session.Files.Count > 0) this.justOpened = false;
                    break;
            }

        }
        
        #endregion

        #region Custom Methods

        private string CurrentPageUrl => this.settingObject.UseCustomStartPage ? this.settingObject.CustomStartPage : this.defaultStartPageUrl;
        private string CurrentRssUrl => this.settingObject.UseCustomRssFeed ? this.settingObject.CustomRssFeed : this.defaultRssUrl;

        /// <summary>
        /// Initializes important variables
        /// </summary>
        public void InitBasics()
        {
            int lenght = DistroConfig.DISTRIBUTION_NAME.Length + 1;
            string dataDir = Path.Combine(PathHelper.DataDir, "StartPage");
            string localeName = PluginBase.MainForm.Settings.LocaleVersion.ToString();
            string version = Application.ProductName.Substring(lenght, Application.ProductName.IndexOfOrdinal(" for") - lenght);
            string fileWithArgs = "index.html?l=" + localeName + "&v=" + HttpUtility.HtmlEncode(version);
            this.defaultStartPageUrl = Path.Combine(PathHelper.AppDir, "StartPage", fileWithArgs);
            this.defaultRssUrl = DistroConfig.DISTRIBUTION_RSS; // Default feed...
            if (!Directory.Exists(dataDir)) Directory.CreateDirectory(dataDir);
            this.settingFilename = Path.Combine(dataDir, "Settings.fdb");
            this.Description = TextHelper.GetString("Info.Description");
            this.pluginImage = PluginBase.MainForm.FindImage("224");
        }

        /// <summary>
        /// Adds the required event handlers
        /// </summary> 
        public void AddEventHandlers()
        {
            EventManager.AddEventHandler(this, EventType.Command);
            EventManager.AddEventHandler(this, EventType.FileEmpty);
            EventManager.AddEventHandler(this, EventType.RestoreSession);
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
        public void SaveSettings() => ObjectSerializer.Serialize(this.settingFilename, this.settingObject);

        /// <summary>
        /// Creates a menu item for the plugin
        /// </summary>
        public void CreateMenuItem()
        {
            string title = TextHelper.GetString("Label.ViewMenuItem");
            ToolStripMenuItem viewMenu = (ToolStripMenuItem)PluginBase.MainForm.FindMenuItem("ViewMenu");
            ToolStripMenuItem viewItem = new ToolStripMenuItem(title, this.pluginImage, this.ViewMenuClick);
            PluginBase.MainForm.RegisterShortcutItem("ViewMenu.ShowStartPage", viewItem);
            viewMenu.DropDownItems.Add(viewItem);
        }

        /// <summary>
        /// Creates the Start Page Tab
        /// </summary>
        public void CreateStartPage()
        {
            this.startPageWebBrowser = new StartPageWebBrowser(this.CurrentPageUrl, this.CurrentRssUrl);
            this.startPage = PluginBase.MainForm.CreateCustomDocument(this.startPageWebBrowser);
            this.startPage.Icon = ImageKonverter.ImageToIcon(this.pluginImage);
            this.startPage.Disposed += this.PluginPanelDisposed;
            this.startPage.Closing += this.PluginPanelClosing; 
            this.startPage.Text = TextHelper.GetString("Title.StartPage");
        }

        /// <summary>
        /// Shows the Start Page Tab and creates if necessary.
        /// </summary>
        public void ShowStartPage()
        {
            if (this.startPage == null) this.CreateStartPage();
            else this.startPage.Show(PluginBase.MainForm.DockPanel);
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Shows the start page.
        /// </summary>
        private void ViewMenuClick(object sender, EventArgs e)
        {
            this.ShowStartPage();
        }

        /// <summary>
        /// Some internal event handling for closing.
        /// </summary>
        private void PluginPanelClosing(object sender, CancelEventArgs e)
        {
            if (this.settingObject.ShowStartPageInsteadOfUntitled && PluginBase.MainForm.Documents.Length == 1)
            {
                e.Cancel = true;
            }
        }

        /// <summary>
        /// Reset the start page reference.
        /// </summary>
        private void PluginPanelDisposed(object sender, EventArgs e)
        {
            this.startPage = null;
        }

        #endregion

    }
}