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
        private String pluginName = "StartPage";
        private String pluginGuid = "e4246322-bc55-4f4a-99c8-aaeeed0a7b9a";
        private String pluginHelp = "www.flashdevelop.org/community/";
        private String pluginDesc = "Adds a start page to FlashDevelop.";
        private String pluginAuth = "FlashDevelop Team";
        private Boolean justOpened = true;
        private String defaultRssUrl = "";
        private String defaultStartPageUrl = "";
        private StartPageWebBrowser startPageWebBrowser;
        private String settingFilename;
        private Settings settingObject;
        private DockContent startPage;
        private Image pluginImage;

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
                case EventType.Command :
                    DataEvent de = (DataEvent)e;
                    switch (de.Action)
                    {
                        case ProjectManagerEvents.Project :
                            // Close pluginPanel if the user has the setting checked and a project is opened
                            if (de.Data != null & this.startPage != null)
                            {
                                if (this.settingObject.CloseOnProjectOpen) this.startPage.Close();
                                if (this.startPage != null)
                                {
                                    // The project manager does not update recent projects until after
                                    // it broadcasts this event so we'll wait a little bit before refreshing
                                    Timer timer = new Timer();
                                    timer.Interval = 100;
                                    timer.Tick += delegate { timer.Stop(); if (!this.startPageWebBrowser.IsDisposed) this.startPageWebBrowser.SendProjectInfo(); };
                                    timer.Start();
                                }
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
                    ISession session = ((DataEvent)e).Data as ISession;
                    if (session.Type != SessionType.Startup) return; // handle only startup sessions
                    if (this.settingObject.ShowStartPageOnStartup == ShowStartPageOnStartupEnum.Always) e.Handled = true;
                    else if (session.Files.Count > 0) this.justOpened = false;
                    break;
            }

        }
        
        #endregion

        #region Custom Methods

        private String CurrentPageUrl { get { return this.settingObject.UseCustomStartPage ? this.settingObject.CustomStartPage : this.defaultStartPageUrl; } }
        private String CurrentRssUrl { get { return this.settingObject.UseCustomRssFeed ? this.settingObject.CustomRssFeed : this.defaultRssUrl; } }

        /// <summary>
        /// Initializes important variables
        /// </summary>
        public void InitBasics()
        {
            String dataDir = Path.Combine(PathHelper.DataDir, "StartPage");
            String startPageDir = Path.Combine(PathHelper.AppDir, "StartPage");
            String localeName = PluginBase.MainForm.Settings.LocaleVersion.ToString();
            String version = Application.ProductName.Substring(13, Application.ProductName.IndexOf(" for") - 13);
            String fileWithArgs = "index.html?l=" + localeName + "&v=" + HttpUtility.HtmlEncode(version);
            this.defaultStartPageUrl = Path.Combine(startPageDir, fileWithArgs);
            this.defaultRssUrl = "http://www.flashdevelop.org/community/rss.php?f=15";
            if (!Directory.Exists(dataDir)) Directory.CreateDirectory(dataDir);
            this.settingFilename = Path.Combine(dataDir, "Settings.fdb");
            this.pluginDesc = TextHelper.GetString("Info.Description");
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

        /// <summary>
        /// Creates a menu item for the plugin
        /// </summary>
        public void CreateMenuItem()
        {
            String title = TextHelper.GetString("Label.ViewMenuItem");
            ToolStripMenuItem viewMenu = (ToolStripMenuItem)PluginBase.MainForm.FindMenuItem("ViewMenu");
            ToolStripMenuItem viewItem = new ToolStripMenuItem(title, this.pluginImage, new EventHandler(this.ViewMenuClick));
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
            this.startPage.Disposed += new EventHandler(this.PluginPanelDisposed);
            this.startPage.Closing += new CancelEventHandler(this.PluginPanelClosing); 
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
        private void ViewMenuClick(Object sender, EventArgs e)
        {
            this.ShowStartPage();
        }

        /// <summary>
        /// Some internal event handling for closing.
        /// </summary>
        private void PluginPanelClosing(Object sender, CancelEventArgs e)
        {
            if (this.settingObject.ShowStartPageInsteadOfUntitled && PluginBase.MainForm.Documents.Length == 1)
            {
                e.Cancel = true;
            }
        }

        /// <summary>
        /// Reset the start page reference.
        /// </summary>
        private void PluginPanelDisposed(Object sender, EventArgs e)
        {
            this.startPage = null;
        }

        #endregion

    }
    
}
