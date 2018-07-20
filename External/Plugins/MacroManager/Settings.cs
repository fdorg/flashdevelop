using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using PluginCore.Localization;

namespace MacroManager
{
    [Serializable]
    public class Settings
    {
        private List<Macro> userMacros = new List<Macro>();

        /// <summary> 
        /// Get and sets the userMacros
        /// </summary>
        [DisplayName("User Macros")]
        [LocalizedDescription("MacroManager.Description.UserMacros")]
        [Editor(typeof(DescriptiveCollectionEditor), typeof(UITypeEditor))]
        public List<Macro> UserMacros
        {
            get { return this.userMacros; }
            set { this.userMacros = value; }
        }

    }

}
