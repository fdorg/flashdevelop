using PluginCore.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace LintingHelper
{
    [Serializable]
    public class Settings
    {
        private bool lintOnSave = true;
        private bool lintOnOpen = true;

        [DisplayName("Lint after saving a file")]
        [LocalizedDescription("LintingHelper.Description.LintOnSave"), DefaultValue(true)]
        public bool LintOnSave
        {
            get { return this.lintOnSave; }
            set { this.lintOnSave = value; }
        }

        [DisplayName("Lint after opening a file")]
        [LocalizedDescription("LintingHelper.Description.LintOnOpen"), DefaultValue(true)]
        public bool LintOnOpen
        {
            get { return this.lintOnOpen; }
            set { this.lintOnOpen = value; }
        }
    }
}
