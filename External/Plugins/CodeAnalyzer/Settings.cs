// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
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
        private String pmdRuleset = String.Empty;
        
        /// <summary>
        /// Get and sets the ruleset file
        /// </summary>
        [DisplayName("Default Ruleset File")]
        [LocalizedDescription("CodeAnalyzer.Description.PMDRuleset"), DefaultValue("")]
        [Editor(typeof(FileNameEditor), typeof(UITypeEditor))]
        public String PMDRuleset
        {
            get { return pmdRuleset; }
            set { pmdRuleset = value; }
        }

    }

}
