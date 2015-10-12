using System;
using System.ComponentModel;
using PluginCore.Localization;

namespace ResultsPanel
{
    [Serializable]
    public class Settings
    {
        private Boolean scrollToBottom = false;

        [DisplayName("Scroll To Bottom")]
        [LocalizedDescription("ResultsPanel.Description.ScrollToBottom"), DefaultValue(false)]
        public Boolean ScrollToBottom
        {
            get { return scrollToBottom; }
            set { scrollToBottom = value; }
        }

    }

}
