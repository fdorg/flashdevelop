using System;
using System.ComponentModel;
using PluginCore.Localization;

namespace StartPage
{
    [Serializable]
    public class Settings
    {
        private Boolean closeOnProjectOpen = false;
        private Boolean showStartPageInsteadOfUntitled = false;
        private ShowStartPageOnStartupEnum showStartPageOnStartup = ShowStartPageOnStartupEnum.NotRestoringSession;
        private Boolean useCustomStartPage = false;
        private Boolean useCustomRssFeed = false;
        private String customStartPage = "";
        private String customRssFeed = "";

        #region Common Settings

        /// <summary>
        /// Gets and sets closeOnProjectOpen
        /// </summary>
        [DisplayName("Close Start Page On Project Open")]
        [LocalizedDescription("StartPage.Description.CloseOnProjectOpen")]
        [LocalizedCategory("StartPage.Category.Common")]
        [DefaultValue(false)]
        public Boolean CloseOnProjectOpen
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
        public Boolean ShowStartPageInsteadOfUntitled
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
        public String CustomStartPage
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
        public Boolean UseCustomStartPage
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
        public String CustomRssFeed
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
        public Boolean UseCustomRssFeed
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
