using System;
using System.ComponentModel;
using System.Drawing.Design;
using Ookii.Dialogs;
using PluginCore.Localization;

namespace LayoutManager
{
    [Serializable]
    public class Settings
    {
        string customLayoutPath = string.Empty;
        
        public Settings()
        {
            Instance = this;
        }

        /// <summary> 
        /// Get the instance of the class
        /// </summary>
        public static Settings Instance { get; set; }

        /// <summary> 
        /// Get and sets the customLayoutPath
        /// </summary>
        [DisplayName("Custom Layout File Directory")]
        [LocalizedDescription("LayoutManager.Description.CustomLayoutPath"), DefaultValue("")]
        [Editor(typeof(VistaFolderNameEditor), typeof(UITypeEditor))]
        public string CustomLayoutPath
        {
            get => customLayoutPath;
            set => customLayoutPath = value;
        }
    }
}