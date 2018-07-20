using System;
using System.IO;
using System.Windows.Forms;
using System.ComponentModel;
using System.Runtime.InteropServices;
using StartPage.ProjectInfo;
using PluginCore.Managers;
using PluginCore.Helpers;
using ProjectManager;
using PluginCore;

namespace StartPage.Controls
{
    public class StartPageWebBrowser : UserControl
    {
        private String rssUrl = String.Empty;
        private String pageUrl = String.Empty;
        private Boolean showingStartPage = false;
        private RecentProjectList recentProjectList;
        private StartPageActions startPageActions;
        private WebBrowserEx webBrowser;
        private DragDropPanel dndPanel;

        public StartPageWebBrowser(String pageUrl, String rssUrl)
        {
            this.rssUrl = rssUrl;
            this.pageUrl = pageUrl;
            this.InitializeDragDrop();
            this.InitializeComponent();
            this.startPageActions = new StartPageActions();
            this.recentProjectList = new RecentProjectList();
            this.webBrowser.ObjectForScripting = this.startPageActions;
            this.startPageActions.DocumentCompleted += new EventHandler(WebBrowserDocumentCompleted);
            this.ShowStartPage();
        }
        
        #region Component Designer Generated Code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.webBrowser = new WebBrowserEx();
            this.SuspendLayout();
            // 
            // webBrowser
            // 
            this.webBrowser.AllowWebBrowserDrop = false;
            this.webBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowser.IsWebBrowserContextMenuEnabled = false;
            this.webBrowser.Location = new System.Drawing.Point(0, 0);
            this.webBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowser.Name = "webBrowser";
            this.webBrowser.Size = new System.Drawing.Size(696, 602);
            this.webBrowser.TabIndex = 0;
            this.webBrowser.WebBrowserShortcutsEnabled = false;
            this.webBrowser.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(this.WebBrowserNavigating);
            this.webBrowser.NewWindow += new System.ComponentModel.CancelEventHandler(this.WebBrowserNewWindow);
            // 
            // StartPageWebBrowser
            //
            this.Dock = System.Windows.Forms.DockStyle.Fill;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.Controls.Add(this.webBrowser);
            this.Name = "StartPageWebBrowser";
            this.Size = new System.Drawing.Size(696, 602);
            this.ResumeLayout(false);

        }

        #endregion

        #region Methods And Event Handlers

        /// <summary>
        /// Initializes the drag and drop operation.
        /// </summary>
        private void InitializeDragDrop()
        {
            this.dndPanel = new DragDropPanel();
            this.Controls.Add(this.dndPanel);
        }

        /// <summary>
        /// Shows the start page
        /// </summary>
        public void ShowStartPage()
        {
            this.showingStartPage = true;
            this.webBrowser.Navigate(this.pageUrl);
        }

        /// <summary>
        /// Updates the recent projects list
        /// </summary>
        public void SendProjectInfo()
        {
            this.recentProjectList.Update(ProjectManager.PluginMain.Settings.RecentProjects);
            this.webBrowser.Document.InvokeScript("handleXmlData", new String[] { this.recentProjectList.ToXml(), null});
        }
        
        /// <summary>
        /// Updates the recent projects list
        /// </summary>
        private void WebBrowserDocumentCompleted(Object sender, EventArgs e)
        {
            this.recentProjectList.Update(ProjectManager.PluginMain.Settings.RecentProjects);
            this.webBrowser.Document.InvokeScript("handleXmlData", new String[] { this.recentProjectList.ToXml(), this.rssUrl});
        }

        /// <summary>
        /// If the page tries to open a new window use a fd tab instead
        /// </summary>
        private void WebBrowserNewWindow(Object sender, CancelEventArgs e)
        {
            this.startPageActions.ShowURL(this.webBrowser.StatusText);
            e.Cancel = true;
        }

        /// <summary>
        /// If we're not about to show the start page and it isn't a javascript call then open a new fd tab
        /// </summary>
        private void WebBrowserNavigating(Object sender, WebBrowserNavigatingEventArgs e)
        {
            if (this.IsDownloadable(e.Url.ToString())) return;
            if (!this.showingStartPage && !e.Url.ToString().StartsWithOrdinal("javascript"))
            {
                this.startPageActions.ShowURL(e.Url.ToString());
                e.Cancel = true;
            }
            this.showingStartPage = false;
        }

        /// <summary>
        /// Checks if the url is downloadable file
        /// </summary>
        private Boolean IsDownloadable(String url)
        {
            return url.EndsWithOrdinal(".exe") || url.EndsWithOrdinal(".zip");
        }

        #endregion

        #region Start Page Actions

        [ComVisible(true)]
        public class StartPageActions
        {
            public event EventHandler DocumentCompleted;

            private static String RELEASE_NOTES_URL = Path.Combine(PathHelper.DocDir, "index.html");

            public void PageReady()
            {
                if (DocumentCompleted != null) DocumentCompleted(this, null);
            }

            /// <summary>
            /// Called from webpage to browse any url in seperate browser control
            /// </summary>
            public void ShowURL(String url)
            {
                PluginBase.MainForm.CallCommand("Browse", url);
            }

            /// <summary>
            /// Called from webpage to browse FlashDevelop Homepage
            /// </summary>
            public void ShowHome()
            {
                this.ShowURL(DistroConfig.DISTRIBUTION_HOME);
            }

            /// <summary>
            /// Called from webpage to browse release notes
            /// </summary>
            public void ShowReleaseNotes()
            {
                this.ShowURL(RELEASE_NOTES_URL);
            }

            /// <summary>
            /// Called from webpage to browse documentation
            /// </summary>
            public void ShowDocumentation()
            {
                this.ShowURL(DistroConfig.DISTRIBUTION_HELP);
            }

            /// <summary>
            /// Called from webpage to show FlashDevelop About dialog
            /// </summary>
            public void ShowAbout()
            {
                PluginBase.MainForm.CallCommand("About", null);
            }

            /// <summary>
            /// Opens appman to install new software
            /// </summary>
            public void InstallSoftware()
            {
                PluginBase.MainForm.CallCommand("RunProcess", "$(Quote)$(AppDir)\\Tools\\appman\\AppMan.exe$(Quote);-locale=$(Locale)");
            }

            /// <summary>
            /// Called from webpage to show New Project Dialog
            /// </summary>
            public void NewProject()
            {
                DataEvent de = new DataEvent(EventType.Command, ProjectManagerCommands.NewProject, null);
                EventManager.DispatchEvent(this, de);
            }

            /// <summary>
            /// Called from webpage to open a project
            /// </summary>
            public void OpenProject(String path)
            {
                PluginBase.MainForm.OpenEditableDocument(path);
            }

            /// <summary>
            /// Called from webpage to open project dialog
            /// </summary>
            public void OpenProjectDialog()
            {
                DataEvent de = new DataEvent(EventType.Command, ProjectManagerCommands.OpenProject, null);
                EventManager.DispatchEvent(this, de);
            }

        }

        #endregion

    }

    #region WebBrowserEx

    class WebBrowserEx : WebBrowser
    {
        /// <summary>
        /// Redirect events to MainForm.
        /// </summary>
        public override Boolean PreProcessMessage(ref Message msg)
        {
            return ((Form)PluginBase.MainForm).PreProcessMessage(ref msg);
        }

    }

    #endregion

}
