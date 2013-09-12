using System;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using PluginCore.Localization;

namespace BasicCompletion
{
    [Serializable]
    public class Settings
    {
        private Boolean disableAutoCompletion = false;

        /// <summary> 
        /// Get and sets the DisableAutoCompletion
        /// </summary>
        [DisplayName("Disable Auto Completion"), DefaultValue(false)]
        [LocalizedDescription("BasicCompletion.Description.DisableAutoCompletion")]
        public Boolean DisableAutoCompletion
        {
            get { return this.disableAutoCompletion; }
            set { this.disableAutoCompletion = value; }
        }

    }

}

