using System;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using PluginCore.Localization;

namespace BasicCompletion
{
    [Serializable]
    public class Settings
    {
        private Boolean disableAutoCompletion = false;
        private List<String> customLanguages = new List<String>();
        public static List<String> DEFAULT_LANGUAGES = new List<String> { "jscript", "csharp", "python", "text", "cpp", "properties", "batch" };
        
        /// <summary> 
        /// Get and sets the SupportedLanguages
        /// </summary>
        [DisplayName("Custom Languages")]
        [LocalizedDescription("BasicCompletion.Description.CustomLanguages")]
        [Editor("System.Windows.Forms.Design.StringCollectionEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "System.Drawing.Design.UITypeEditor,System.Drawing, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
        public List<String> CustomLanguages
        {
            get { return this.customLanguages; }
            set { this.customLanguages = value; }
        }

        /// <summary> 
        /// Get and sets the DisableAutoCompletion
        /// </summary>
        [DisplayName("Disable Auto Completion"), DefaultValue(false)]
        [LocalizedDescription("BasicCompletion.Description.DisableAutoCompletion")]
        public Boolean DisableAutoCompletion
        {
            get { return this.disableAutoCompletion; }
            set { this.disableAutoCompletion = value; }
        }

    }

}

