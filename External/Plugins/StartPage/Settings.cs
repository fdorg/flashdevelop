using System;
using System.ComponentModel;
using PluginCore.Localization;

namespace StartPage
{
    [Serializable]
    public class Settings
    {
        private bool closeOnProjectOpen = false;
        private bool showStartPageInsteadOfUntitled = false;
        private ShowStartPageOnStartupEnum showStartPageOnStartup = ShowStartPageOnStartupEnum.NotRestoringSession;
        private bool useCustomStartPage = false;
        private bool useCustomRssFeed = false;
        private string customStartPage = "";
        private string customRssFeed = "";

        #region Common Settings

        /// <summary>
        /// Gets and sets closeOnProjectOpen
        /// </summary>
        [DisplayName("Close Start Page On Project Open")]
        [LocalizedDescription("StartPage.Description.CloseOnProjectOpen")]
        [LocalizedCategory("StartPage.Category.Common")]
        [DefaultValue(false)]
        public bool CloseOnProjectOpen
        {
            get { return closeOnProjectOpen; }
            set { this.closeOnProjectOpen = value; }
        }

        /// <summary>
        /// Gets and sets showStartPageInsteadOfUntitled
        /// </summary>
        [DisplayName("Show Start Page Instead Of Untitled")]
        [LocalizedDescription("StartPage.Description.ShowStartPageInsteadOfUntitled")]
        [LocalizedCategory("StartPage.Category.Common")]
        [DefaultValue(false)]
        public bool ShowStartPageInsteadOfUntitled
        {
            get { return showStartPageInsteadOfUntitled; }
            set { this.showStartPageInsteadOfUntitled = value; }
        }

        /// <summary>
        /// Gets and sets showStartPageOnStartup
        /// </summary>
        [DisplayName("Show Start Page On Startup")]
        [LocalizedCategory("StartPage.Category.Common")]
        [LocalizedDescription("StartPage.Description.ShowStartPageOnStartup")]
        [DefaultValue(ShowStartPageOnStartupEnum.NotRestoringSession)]
        public ShowStartPageOnStartupEnum ShowStartPageOnStartup
        {
            get { return showStartPageOnStartup; }
            set { this.showStartPageOnStartup = value; }
        }

        #endregion

        #region Custom Settings

        /// <summary>
        /// Gets and sets useCustomRssFeed
        /// </summary>
        [DisplayName("Custom Start Page")]
        [LocalizedDescription("StartPage.Description.CustomStartPage")]
        [LocalizedCategory("StartPage.Category.Custom")]
        [DefaultValue("")]
        public string CustomStartPage
        {
            get { return customStartPage; }
            set { this.customStartPage = value; }
        }

        /// <summary>
        /// Gets and sets useCustomRssFeed
        /// </summary>
        [DisplayName("Use Custom Start Page")]
        [LocalizedDescription("StartPage.Description.UseCustomStartPage")]
        [LocalizedCategory("StartPage.Category.Custom")]
        [DefaultValue(false)]
        public bool UseCustomStartPage
        {
            get { return useCustomStartPage; }
            set { this.useCustomStartPage = value; }
        }

        /// <summary>
        /// Gets and sets useCustomRssFeed
        /// </summary>
        [DisplayName("Custom RSS Feed")]
        [LocalizedDescription("StartPage.Description.CustomRssFeed")]
        [LocalizedCategory("StartPage.Category.Custom")]
        [DefaultValue("")]
        public string CustomRssFeed
        {
            get { return customRssFeed; }
            set { this.customRssFeed = value; }
        }

        /// <summary>
        /// Gets and sets useCustomRssFeed
        /// </summary>
        [DisplayName("Use Custom RSS Feed")]
        [LocalizedDescription("StartPage.Description.UseCustomRssFeed")]
        [LocalizedCategory("StartPage.Category.Custom")]
        [DefaultValue(false)]
        public bool UseCustomRssFeed
        {
            get { return useCustomRssFeed; }
            set { this.useCustomRssFeed = value; }
        }

        #endregion

    }

    #region Enums

    public enum ShowStartPageOnStartupEnum
    {
        Always,
        NotRestoringSession,
        Never
    }

    #endregion

}