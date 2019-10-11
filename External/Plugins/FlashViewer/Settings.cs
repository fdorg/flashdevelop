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
        private string playerPath = string.Empty;
        private ViewStyle displayStyle = ViewStyle.External;
        private bool disableAutoConfig = false;

        /// <summary> 
        /// Get and sets the playerPath
        /// </summary>
        [DisplayName("External Player Path")]
        [LocalizedDescription("FlashViewer.Description.PlayerPath"), DefaultValue("")]
        [Editor(typeof(FileNameEditor), typeof(UITypeEditor))]
        public string PlayerPath
        {
            get => this.playerPath;
            set => this.playerPath = value;
        }

        /// <summary> 
        /// Get and sets the displayStyle
        /// </summary>
        [DisplayName("Movie Display Style")]
        [LocalizedDescription("FlashViewer.Description.DisplayStyle"), DefaultValue(ViewStyle.External)]
        public ViewStyle DisplayStyle 
        {
            get => this.displayStyle;
            set => this.displayStyle = value;
        }

        /// <summary> 
        /// Get and sets the disableAutoConfig
        /// </summary>
        [DisplayName("Disable Auto-Configure")]
        [LocalizedDescription("FlashViewer.Description.DisableAutoConfig"), DefaultValue(false)]
        public bool DisableAutoConfig
        {
            get => this.disableAutoConfig;
            set => this.disableAutoConfig = value;
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
