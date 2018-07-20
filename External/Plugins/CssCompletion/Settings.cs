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
            get { return disableAutoCloseBraces; }
            set { disableAutoCloseBraces = value; }
        }

        [DisplayName("Disable Auto Completion"), DefaultValue(false)]
        [LocalizedDescription("CssCompletion.Description.DisableAutoCompletion")]
        public Boolean DisableAutoCompletion
        {
            get { return this.disableAutoCompletion; }
            set { this.disableAutoCompletion = value; }
        }

        [DisplayName("Disable Insert Colon"), DefaultValue(false)]
        [LocalizedDescription("CssCompletion.Description.DisableInsertColon")]
        public Boolean DisableInsertColon
        {
            get { return this.disableInsertColon; }
            set { this.disableInsertColon = value; }
        }

        [DisplayName("Disable Compile To CSS On Save"), DefaultValue(false)]
        [LocalizedDescription("CssCompletion.Description.DisableCompileOnSave")]
        public Boolean DisableCompileOnSave
        {
            get { return this.disableCompileOnSave; }
            set { this.disableCompileOnSave = value; }
        }

        [DisplayName("Disable Minify CSS On Save"), DefaultValue(false)]
        [LocalizedDescription("CssCompletion.Description.DisableMinifyOnSave")]
        public Boolean DisableMinifyOnSave
        {
            get { return this.disableMinifyOnSave; }
            set { this.disableMinifyOnSave = value; }
        }

        [DisplayName("Enable Verbose Compilation"), DefaultValue(false)]
        [LocalizedDescription("CssCompletion.Description.EnableVerboseCompilation")]
        public Boolean EnableVerboseCompilation
        {
            get { return this.enableVerboseCompilation; }
            set { this.enableVerboseCompilation = value; }
        }
    }

}

