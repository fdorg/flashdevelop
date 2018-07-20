using System;
using System.ComponentModel;
using PluginCore.Localization;

namespace XMLCompletion
{
    [Serializable]
    public class Settings
    {
        private Boolean disableZenCoding = false;
        private Boolean closeTags = true;
        private Boolean insertQuotes = true;
        private Boolean smartIndenter = true;
        private Boolean upperCaseHtmlTags = false;
        private Boolean enableXMLCompletion = true;
        private static Settings instance = null;

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
        /// Option to disable Zen Coding feature
        /// </summary>
        [DisplayName("Disable Zen Coding")]
        [LocalizedDescription("XMLCompletion.Description.DisableZenCoding"), DefaultValue(false)]
        public Boolean DisableZenCoding
        {
            get { return this.disableZenCoding; }
            set { this.disableZenCoding = value; }
        }

        /// <summary> 
        /// Get and sets the closeTags
        /// </summary>
        [DisplayName("Close Tags")]
        [LocalizedDescription("XMLCompletion.Description.CloseTags"), DefaultValue(true)]
        public Boolean CloseTags 
        {
            get { return this.closeTags; }
            set { this.closeTags = value; }
        }

        /// <summary> 
        /// Get and sets the insertQuotes
        /// </summary>
        [DisplayName("Insert Quotes")]
        [LocalizedDescription("XMLCompletion.Description.InsertQuotes"), DefaultValue(true)]
        public Boolean InsertQuotes 
        {
            get { return this.insertQuotes; }
            set { this.insertQuotes = value; }
        }

        /// <summary> 
        /// Get and sets the smartIndenter
        /// </summary>
        [DisplayName("Enable Smart Indenter")]
        [LocalizedDescription("XMLCompletion.Description.SmartIndenter"), DefaultValue(true)]
        public Boolean SmartIndenter
        {
            get { return this.smartIndenter; }
            set { this.smartIndenter = value; }
        }

        /// <summary> 
        /// Get and sets the lowerCaseHtmlTags
        /// </summary>
        [DisplayName("Upper Case Html Tags")]
        [LocalizedDescription("XMLCompletion.Description.UpperCaseHtmlTags"), DefaultValue(false)]
        public Boolean UpperCaseHtmlTags
        {
            get { return this.upperCaseHtmlTags; }
            set { this.upperCaseHtmlTags = value; }
        }

        /// <summary> 
        /// Get and sets the enableDeclarationCompletion
        /// </summary>
        [DisplayName("Enable XML Completion")]
        [LocalizedDescription("XMLCompletion.Description.EnableXMLCompletion"), DefaultValue(true)]
        public Boolean EnableXMLCompletion
        {
            get { return this.enableXMLCompletion; }
            set { this.enableXMLCompletion = value; }
        }

    }

}
