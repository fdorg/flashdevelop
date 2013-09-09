using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Design;
using System.ComponentModel;
using System.Windows.Forms.Design;
using System.Collections.Generic;
using PluginCore.Localization;

namespace LayoutManager
{
    [Serializable]
    public class Settings
    {
        private static Settings instance;
        private String customLayoutPath = String.Empty;
        
        public Settings()
        {
            instance = this;
        }

        /// <summary> 
        /// Get the instance of the class
        /// </summary>
        public static Settings Instance
        {
            get { return instance; }
            set { instance = value; }
        }

        /// <summary> 
        /// Get and sets the customLayoutPath
        /// </summary>
        [DisplayName("Custom Layout File Directory")]
        [LocalizedDescription("LayoutManager.Description.CustomLayoutPath"), DefaultValue("")]
        [Editor(typeof(FolderNameEditor), typeof(UITypeEditor))]
        public String CustomLayoutPath
        {
            get { return this.customLayoutPath; }
            set { this.customLayoutPath = value; }
        }

    }

}
