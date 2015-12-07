using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using PluginCore.Localization;

namespace FlashViewer
{
    [Serializable]
    public class Settings
    {
        private String playerPath = String.Empty;
        private ViewStyle displayStyle = ViewStyle.External;
        private Boolean disableAutoConfig = false;

        /// <summary> 
        /// Get and sets the playerPath
        /// </summary>
        [DisplayName("External Player Path")]
        [LocalizedDescription("FlashViewer.Description.PlayerPath"), DefaultValue("")]
        [Editor(typeof(FileNameEditor), typeof(UITypeEditor))]
        public String PlayerPath
        {
            get { return this.playerPath; }
            set { this.playerPath = value; }
        }

        /// <summary> 
        /// Get and sets the displayStyle
        /// </summary>
        [DisplayName("Movie Display Style")]
        [LocalizedDescription("FlashViewer.Description.DisplayStyle"), DefaultValue(ViewStyle.External)]
        public ViewStyle DisplayStyle 
        {
            get { return this.displayStyle; }
            set { this.displayStyle = value; }
        }

        /// <summary> 
        /// Get and sets the disableAutoConfig
        /// </summary>
        [DisplayName("Disable Auto-Configure")]
        [LocalizedDescription("FlashViewer.Description.DisableAutoConfig"), DefaultValue(false)]
        public Boolean DisableAutoConfig
        {
            get { return this.disableAutoConfig; }
            set { this.disableAutoConfig = value; }
        }
    }

    /// <summary>
    /// Style how the flash movies are viewed
    /// </summary>
    public enum ViewStyle
    {
        Popup,
        External,
        Document
    }

}
