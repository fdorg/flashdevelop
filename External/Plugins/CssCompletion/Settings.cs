using System;
using System.ComponentModel;
using PluginCore.Localization;

namespace CssCompletion
{
    [Serializable]
    public class Settings
    {
        private bool disableAutoCompletion = false;
        private bool disableInsertColon = false;
        private bool disableCompileOnSave = false;
        private bool disableMinifyOnSave = false;
        private bool enableVerboseCompilation = false;
        private bool disableAutoCloseBraces = false;

        [DisplayName("Disable Auto-Close Blocks"), DefaultValue(false)]
        [LocalizedDescription("CssCompletion.Description.DisableAutoCloseBraces")]
        public bool DisableAutoCloseBraces
        {
            get => disableAutoCloseBraces;
            set => disableAutoCloseBraces = value;
        }

        [DisplayName("Disable Auto Completion"), DefaultValue(false)]
        [LocalizedDescription("CssCompletion.Description.DisableAutoCompletion")]
        public bool DisableAutoCompletion
        {
            get => disableAutoCompletion;
            set => disableAutoCompletion = value;
        }

        [DisplayName("Disable Insert Colon"), DefaultValue(false)]
        [LocalizedDescription("CssCompletion.Description.DisableInsertColon")]
        public bool DisableInsertColon
        {
            get => disableInsertColon;
            set => disableInsertColon = value;
        }

        [DisplayName("Disable Compile To CSS On Save"), DefaultValue(false)]
        [LocalizedDescription("CssCompletion.Description.DisableCompileOnSave")]
        public bool DisableCompileOnSave
        {
            get => disableCompileOnSave;
            set => disableCompileOnSave = value;
        }

        [DisplayName("Disable Minify CSS On Save"), DefaultValue(false)]
        [LocalizedDescription("CssCompletion.Description.DisableMinifyOnSave")]
        public bool DisableMinifyOnSave
        {
            get => disableMinifyOnSave;
            set => disableMinifyOnSave = value;
        }

        [DisplayName("Enable Verbose Compilation"), DefaultValue(false)]
        [LocalizedDescription("CssCompletion.Description.EnableVerboseCompilation")]
        public bool EnableVerboseCompilation
        {
            get => enableVerboseCompilation;
            set => enableVerboseCompilation = value;
        }
    }

}

