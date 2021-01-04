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
        readonly string rssUrl;
        readonly string pageUrl;
        bool showingStartPage;
        readonly RecentProjectList recentProjectList;
        readonly StartPageActions startPageActions;
        WebBrowserEx webBrowser;
        DragDropPanel dndPanel;

        public StartPageWebBrowser(string pageUrl, string rssUrl)
        {
            this.rssUrl = rssUrl;
            this.pageUrl = pageUrl;
            InitializeDragDrop();
            InitializeComponent();
            startPageActions = new StartPageActions();
            recentProjectList = new RecentProjectList();
            webBrowser.ObjectForScripting = startPageActions;
            startPageActions.DocumentCompleted += WebBrowserDocumentCompleted;
            ShowStartPage();
        }
        
        #region Component Designer Generated Code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        void InitializeComponent()
        {
            webBrowser = new WebBrowserEx();
            SuspendLayout();
            // 
            // webBrowser
            // 
            webBrowser.AllowWebBrowserDrop = false;
            webBrowser.Dock = DockStyle.Fill;
            webBrowser.IsWebBrowserContextMenuEnabled = false;
            webBrowser.Location = new System.Drawing.Point(0, 0);
            webBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            webBrowser.Name = "webBrowser";
            webBrowser.Size = new System.Drawing.Size(696, 602);
            webBrowser.TabIndex = 0;
            webBrowser.WebBrowserShortcutsEnabled = false;
            webBrowser.Navigating += WebBrowserNavigating;
            webBrowser.NewWindow += WebBrowserNewWindow;
            // 
            // StartPageWebBrowser
            //
            Dock = DockStyle.Fill;
            AutoScaleMode = AutoScaleMode.None;
            Controls.Add(webBrowser);
            Name = "StartPageWebBrowser";
            Size = new System.Drawing.Size(696, 602);
            ResumeLayout(false);
        }

        #endregion

        #region Methods And Event Handlers

        /// <summary>
        /// Initializes the drag and drop operation.
        /// </summary>
        void InitializeDragDrop()
        {
            dndPanel = new DragDropPanel();
            Controls.Add(dndPanel);
        }

        /// <summary>
        /// Shows the start page
        /// </summary>
        public void ShowStartPage()
        {
            showingStartPage = true;
            webBrowser.Navigate(pageUrl);
        }

        /// <summary>
        /// Updates the recent projects list
        /// </summary>
        public void SendProjectInfo()
        {
            recentProjectList.Update(ProjectManager.PluginMain.Settings.RecentProjects);
            webBrowser.Document?.InvokeScript("handleXmlData", new[] { recentProjectList.ToXml(), null});
        }
        
        /// <summary>
        /// Updates the recent projects list
        /// </summary>
        void WebBrowserDocumentCompleted(object sender, EventArgs e)
        {
            recentProjectList.Update(ProjectManager.PluginMain.Settings.RecentProjects);
            webBrowser.Document?.InvokeScript("handleXmlData", new[] { recentProjectList.ToXml(), rssUrl});
        }

        /// <summary>
        /// If the page tries to open a new window use a fd tab instead
        /// </summary>
        void WebBrowserNewWindow(object sender, CancelEventArgs e)
        {
            startPageActions.ShowURL(webBrowser.StatusText);
            e.Cancel = true;
        }

        /// <summary>
        /// If we're not about to show the start page and it isn't a javascript call then open a new fd tab
        /// </summary>
        void WebBrowserNavigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            if (IsDownloadable(e.Url.ToString())) return;
            if (!showingStartPage && !e.Url.ToString().StartsWithOrdinal("javascript"))
            {
                startPageActions.ShowURL(e.Url.ToString());
                e.Cancel = true;
            }
            showingStartPage = false;
        }

        /// <summary>
        /// Checks if the url is downloadable file
        /// </summary>
        static bool IsDownloadable(string url) => url.EndsWithOrdinal(".exe") || url.EndsWithOrdinal(".zip");

        #endregion

        #region Start Page Actions

        [ComVisible(true)]
        public class StartPageActions
        {
            public event EventHandler DocumentCompleted;

            static string RELEASE_NOTES_URL = Path.Combine(PathHelper.DocDir, "index.html");

            public void PageReady() => DocumentCompleted?.Invoke(this, null);

            /// <summary>
            /// Called from webpage to browse any url in separate browser control
            /// </summary>
            public void ShowURL(string url) => PluginBase.MainForm.CallCommand("Browse", url);

            /// <summary>
            /// Called from webpage to browse FlashDevelop Homepage
            /// </summary>
            public void ShowHome() => ShowURL(DistroConfig.DISTRIBUTION_HOME);

            /// <summary>
            /// Called from webpage to browse release notes
            /// </summary>
            public void ShowReleaseNotes() => ShowURL(RELEASE_NOTES_URL);

            /// <summary>
            /// Called from webpage to browse documentation
            /// </summary>
            public void ShowDocumentation() => ShowURL(DistroConfig.DISTRIBUTION_HELP);

            /// <summary>
            /// Called from webpage to show FlashDevelop About dialog
            /// </summary>
            public void ShowAbout() => PluginBase.MainForm.CallCommand("About", null);

            /// <summary>
            /// Opens appman to install new software
            /// </summary>
            public void InstallSoftware() => PluginBase.MainForm.CallCommand("RunProcess", "$(Quote)$(AppDir)\\Tools\\appman\\AppMan.exe$(Quote);-locale=$(Locale)");

            /// <summary>
            /// Called from webpage to show New Project Dialog
            /// </summary>
            public void NewProject()
            {
                var de = new DataEvent(EventType.Command, ProjectManagerCommands.NewProject, null);
                EventManager.DispatchEvent(this, de);
            }

            /// <summary>
            /// Called from webpage to open a project
            /// </summary>
            public void OpenProject(string path) => PluginBase.MainForm.OpenEditableDocument(path);

            /// <summary>
            /// Called from webpage to open project dialog
            /// </summary>
            public void OpenProjectDialog()
            {
                var de = new DataEvent(EventType.Command, ProjectManagerCommands.OpenProject, null);
                EventManager.DispatchEvent(this, de);
            }

        }

        #endregion
    }

    #region WebBrowserEx

    internal class WebBrowserEx : WebBrowser
    {
        /// <summary>
        /// Redirect events to MainForm.
        /// </summary>
        public override bool PreProcessMessage(ref Message msg)
            => ((Form)PluginBase.MainForm).PreProcessMessage(ref msg);
    }

    #endregion
}