using System;
using System.ComponentModel;
using PluginCore.Localization;

namespace CodeRefactor
{
    [Serializable]
    public class Settings
    {
        private Boolean separatePackages = false;
        private Boolean disableMoveRefactoring = false;

        [DisplayName("Separate Packages")]
        [LocalizedDescription("CodeRefactor.Description.SeparatePackages"), DefaultValue(false)]
        public Boolean SeparatePackages
        {
            get { return this.separatePackages; }
            set { this.separatePackages = value; }
        }

        [DisplayName("Disable Move Refactoring")]
        [LocalizedDescription("CodeRefactor.Description.DisableMoveRefactoring"), DefaultValue(false)]
        public Boolean DisableMoveRefactoring
        {
            get { return this.disableMoveRefactoring; }
            set { this.disableMoveRefactoring = value; }
        }

    }

}
