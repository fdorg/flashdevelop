using System;
using System.ComponentModel;
using PluginCore.Localization;

namespace CodeRefactor
{
    [Serializable]
    public class Settings
    {
        private Boolean separatePackages = false;

        [DisplayName("Separate Packages")]
        [LocalizedDescription("CodeRefactor.Description.SeparatePackages"), DefaultValue(false)]
        public Boolean SeparatePackages
        {
            get { return this.separatePackages; }
            set { this.separatePackages = value; }
        }

    }

}
