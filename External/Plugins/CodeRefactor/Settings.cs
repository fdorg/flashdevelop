using System;
using System.Text;
using System.ComponentModel;
using System.Collections.Generic;
using PluginCore.Localization;
using System.Windows.Forms;

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
