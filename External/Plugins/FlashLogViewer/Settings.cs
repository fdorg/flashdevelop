using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using PluginCore.Localization;

namespace FlashLogViewer
{
    [Serializable]
    public class Settings
    {
        private string flashLogFile = "";
        private string policyLogFile = "";
        private string regexError = "Error #";
        private string regexWarning = "Warning: ";
        private StartType trackingStartType = StartType.Manually;
        private bool keepPopupTopMost = true;
        private bool colourWarnings = true;
        private int updateInterval = 100;

        /// <summary> 
        /// Get or sets the flashLogFile.
        /// </summary>
        [DisplayName("Flash Log File")]
        [LocalizedCategory("FlashLogViewer.Category.Files")]
        [LocalizedDescription("FlashLogViewer.Description.FlashLogFile")]
        [Editor(typeof(FileNameEditor), typeof(UITypeEditor))]
        public string FlashLogFile 
        {
            get { return this.flashLogFile; }
            set { this.flashLogFile = value; }
        }

        /// <summary> 
        /// Get or sets the policyLogFile.
        /// </summary>
        [DisplayName("Policy Log File")]
        [LocalizedCategory("FlashLogViewer.Category.Files")]
        [LocalizedDescription("FlashLogViewer.Description.PolicyLogFile")]
        [Editor(typeof(FileNameEditor), typeof(UITypeEditor))]
        public string PolicyLogFile
        {
            get { return this.policyLogFile; }
            set { this.policyLogFile = value; }
        }

        /// <summary> 
        /// Get or sets the colourWarnings.
        /// </summary>
        [DefaultValue(true)]
        [DisplayName("Colorize Warnings")]
        [LocalizedDescription("FlashLogViewer.Description.ColourWarnings")]
        public bool ColourWarnings
        {
            get { return this.colourWarnings; }
            set { this.colourWarnings = value; }
        }

        /// <summary> 
        /// Get or sets the keepPopupTopMost.
        /// </summary>
        [DefaultValue(true)]
        [DisplayName("Keep Popup On Top")]
        [LocalizedDescription("FlashLogViewer.Description.KeepPopupTopMost")]
        public bool KeepPopupTopMost
        {
            get { return this.keepPopupTopMost; }
            set { this.keepPopupTopMost = value; }
        }

        /// <summary> 
        /// Get or sets the trackingStartType.
        /// </summary>
        [DisplayName("Start Tracking")]
        [DefaultValue(StartType.Manually)]
        [LocalizedDescription("FlashLogViewer.Description.TrackingStartType")]
        public StartType TrackingStartType
        {
            get { return this.trackingStartType; }
            set { this.trackingStartType = value; }
        }

        /// <summary> 
        /// Get or sets the updateInterval.
        /// </summary>
        [DefaultValue(100)]
        [DisplayName("Update Interval")]
        [LocalizedDescription("FlashLogViewer.Description.UpdateInterval")]
        public int UpdateInterval
        {
            get { return this.updateInterval; }
            set { this.updateInterval = value; }
        }

        /// <summary> 
        /// Get or sets the regexWarning.
        /// </summary>
        [DefaultValue("Warning: ")]
        [DisplayName("Regex For Warnings")]
        [LocalizedCategory("FlashLogViewer.Category.Regex")]
        [LocalizedDescription("FlashLogViewer.Description.RegexWarning")]
        public string RegexWarning
        {
            get { return this.regexWarning; }
            set { this.regexWarning = value; }
        }

        /// <summary> 
        /// Get or sets the regexError.
        /// </summary>
        [DefaultValue("Error #")]
        [DisplayName("Regex For Errors")]
        [LocalizedCategory("FlashLogViewer.Category.Regex")]
        [LocalizedDescription("FlashLogViewer.Description.RegexError")]
        public string RegexError
        {
            get { return this.regexError; }
            set { this.regexError = value; }
        }

     }

    public enum StartType
    {
        OnProgramStart,
        OnBuildComplete,
        Manually
    }

}
