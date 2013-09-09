using System;
using System.Text;
using System.Collections.Generic;
using System.ComponentModel;
using PluginCore.Localization;
using System.Windows.Forms;

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
