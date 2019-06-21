using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using PluginCore.Localization;

namespace CodeAnalyzer
{
    [Serializable]
    public class Settings
    {
        private string pmdRuleset = string.Empty;
        
        /// <summary>
        /// Get and sets the ruleset file
        /// </summary>
        [DisplayName("Default Ruleset File")]
        [LocalizedDescription("CodeAnalyzer.Description.PMDRuleset"), DefaultValue("")]
        [Editor(typeof(FileNameEditor), typeof(UITypeEditor))]
        public string PMDRuleset
        {
            get => pmdRuleset;
            set => pmdRuleset = value;
        }

    }

}
