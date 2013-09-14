using System;
using System.ComponentModel;
using PluginCore.Localization;

namespace CodeRefactor
{
    [Serializable]
    public class Settings
    {
        private Boolean _separatePackages;

        [DisplayName(@"Separate Packages")]
        [LocalizedDescription("CodeRefactor.Description.SeparatePackages"), DefaultValue(false)]
        public Boolean SeparatePackages
        {
            get { return _separatePackages; }
            set { _separatePackages = value; }
        }

    }

}