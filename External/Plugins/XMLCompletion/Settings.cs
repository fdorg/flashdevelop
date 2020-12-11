using System;
using System.ComponentModel;
using PluginCore.Localization;

namespace XMLCompletion
{
    [Serializable]
    public class Settings
    {
        bool disableZenCoding = false;
        bool closeTags = true;
        bool insertQuotes = true;
        bool smartIndenter = true;
        bool upperCaseHtmlTags = false;
        bool enableXMLCompletion = true;
        static Settings instance = null;

        public Settings()
        {
            instance = this;
        }

        /// <summary> 
        /// Get the instance of the class
        /// </summary>
        public static Settings Instance
        {
            get => instance;
            set => instance = value;
        }

        /// <summary> 
        /// Option to disable Zen Coding feature
        /// </summary>
        [DisplayName("Disable Zen Coding")]
        [LocalizedDescription("XMLCompletion.Description.DisableZenCoding"), DefaultValue(false)]
        public bool DisableZenCoding
        {
            get => this.disableZenCoding;
            set => this.disableZenCoding = value;
        }

        /// <summary> 
        /// Get and sets the closeTags
        /// </summary>
        [DisplayName("Close Tags")]
        [LocalizedDescription("XMLCompletion.Description.CloseTags"), DefaultValue(true)]
        public bool CloseTags 
        {
            get => this.closeTags;
            set => this.closeTags = value;
        }

        /// <summary> 
        /// Get and sets the insertQuotes
        /// </summary>
        [DisplayName("Insert Quotes")]
        [LocalizedDescription("XMLCompletion.Description.InsertQuotes"), DefaultValue(true)]
        public bool InsertQuotes 
        {
            get => this.insertQuotes;
            set => this.insertQuotes = value;
        }

        /// <summary> 
        /// Get and sets the smartIndenter
        /// </summary>
        [DisplayName("Enable Smart Indenter")]
        [LocalizedDescription("XMLCompletion.Description.SmartIndenter"), DefaultValue(true)]
        public bool SmartIndenter
        {
            get => this.smartIndenter;
            set => this.smartIndenter = value;
        }

        /// <summary> 
        /// Get and sets the lowerCaseHtmlTags
        /// </summary>
        [DisplayName("Upper Case Html Tags")]
        [LocalizedDescription("XMLCompletion.Description.UpperCaseHtmlTags"), DefaultValue(false)]
        public bool UpperCaseHtmlTags
        {
            get => this.upperCaseHtmlTags;
            set => this.upperCaseHtmlTags = value;
        }

        /// <summary> 
        /// Get and sets the enableDeclarationCompletion
        /// </summary>
        [DisplayName("Enable XML Completion")]
        [LocalizedDescription("XMLCompletion.Description.EnableXMLCompletion"), DefaultValue(true)]
        public bool EnableXMLCompletion
        {
            get => this.enableXMLCompletion;
            set => this.enableXMLCompletion = value;
        }

    }

}
