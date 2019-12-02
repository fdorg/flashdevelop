// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using PluginCore.Localization;
using System;
using System.ComponentModel;

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
            get => lintOnSave;
            set => lintOnSave = value;
        }

        [DisplayName("Lint after opening a file")]
        [LocalizedDescription("LintingHelper.Description.LintOnOpen"), DefaultValue(true)]
        public bool LintOnOpen
        {
            get => lintOnOpen;
            set => lintOnOpen = value;
        }
    }
}
