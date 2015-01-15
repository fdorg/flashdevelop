using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Drawing.Design;
using System.ComponentModel.Design;
using PluginCore.Localization;

namespace OutputPanel
{
    [Serializable]
    [DefaultProperty("Path")]
    public class HighlightKeyword
    {
        private String m_Keyword;
        private LogLevel m_Level;

        public HighlightKeyword()
        {
            m_Keyword = "";
        }

        public HighlightKeyword(String value)
        {
            m_Keyword = value;
        }
        
        public String Keyword
        {
            get { return m_Keyword; }
            set { m_Keyword = value; }
        }

        public override String ToString()
        {
            return m_Keyword;
        }
        
        [DefaultValue(LogLevel.Debug)]
        [LocalizedDescription("OutputPanel.Description.LogLevel")]
        public LogLevel Level
        {
            get { return m_Level; }
            set { m_Level = value; }
        }
    }

    [Serializable]
    public class Settings
    {
        private Boolean showOnOutput = false;
        private Boolean showOnProcessEnd = false;
        private Boolean wrapOutput = false;
        private HighlightKeyword[] keywords = new HighlightKeyword[] { };

        /// <summary> 
        /// Get and sets the alwaysShow
        /// </summary>
        [DisplayName("Show On Output")] 
        [LocalizedDescription("OutputPanel.Description.ShowOnOutput"), DefaultValue(false)]
        public Boolean ShowOnOutput 
        {
            get { return this.showOnOutput; }
            set { this.showOnOutput = value; }
        }

        /// <summary> 
        /// Get and sets the showOnProcessEnd
        /// </summary>
        [DisplayName("Show On Process End")] 
        [LocalizedDescription("OutputPanel.Description.ShowOnProcessEnd"), DefaultValue(false)]
        public Boolean ShowOnProcessEnd 
        {
            get { return this.showOnProcessEnd; }
            set { this.showOnProcessEnd = value; }
        }

        /// <summary> 
        /// Get and sets the wrapOutput
        /// </summary>
        [DisplayName("Wrap Output")]
        [LocalizedDescription("OutputPanel.Description.WrapOutput"), DefaultValue(false)]
        public Boolean WrapOutput
        {
            get { return this.wrapOutput; }
            set { this.wrapOutput = value; }
        }

        [DisplayName("Highlighted Keywords")]
        [LocalizedCategory("FlashDebugger.Category.Misc")]
        [LocalizedDescription("FlashDebugger.Description.HighlightedKeywords")]
        [Editor(typeof(ArrayEditor), typeof(UITypeEditor))]
        public HighlightKeyword[] Keywords
        {
            get
            {
                if (keywords == null || keywords.Length == 0)
                {
                    keywords = new HighlightKeyword[] { };
                }
                return keywords;
            }
            set { keywords = value; }
        }
       
    }

    public enum LogLevel
    {
        Info,
        Debug,
        Warning,
        Error,
        Fatal
    }

}
