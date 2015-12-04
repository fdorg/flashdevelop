using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using PluginCore.Localization;

namespace OutputPanel
{
    [Serializable]
    public class Settings
    {
        private Boolean showOnOutput = false;
        private Boolean showOnProcessEnd = false;
        private Boolean useLegacyColoring = false;
        private List<HighlightMarker> highlightMarkers;
        private Boolean wrapOutput = false;

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

        /// <summary> 
        /// Get and sets the wrapOutput
        /// </summary>
        [DisplayName("Use Legacy Coloring")]
        [LocalizedDescription("OutputPanel.Description.UseLegacyColoring"), DefaultValue(false)]
        public Boolean UseLegacyColoring
        {
            get { return this.useLegacyColoring; }
            set { this.useLegacyColoring = value; }
        }

        [DisplayName("Highlight Markers")]
        [LocalizedDescription("OutputPanel.Description.HighlightMarkers")]
        [Editor(typeof(DescriptiveCollectionEditor), typeof(UITypeEditor))]
        public List<HighlightMarker> HighlightMarkers
        {
            get
            {
                if (highlightMarkers == null || highlightMarkers.Count == 0)
                {
                    this.highlightMarkers = new List<HighlightMarker>();
                    this.highlightMarkers.Add(new HighlightMarker("Info:", LogLevel.Info));
                    this.highlightMarkers.Add(new HighlightMarker("Debug:", LogLevel.Debug));
                    this.highlightMarkers.Add(new HighlightMarker("Warning:", LogLevel.Warning));
                    this.highlightMarkers.Add(new HighlightMarker("Error:", LogLevel.Error));
                    this.highlightMarkers.Add(new HighlightMarker("Fatal:", LogLevel.Fatal));
                }
                return highlightMarkers;
            }
            set { this.highlightMarkers = value; }
        }

    }

    [Serializable]
    public class HighlightMarker
    {
        public String marker = "Info:";
        public LogLevel level = LogLevel.Debug;
        
        public HighlightMarker(){}
        public HighlightMarker(String marker, LogLevel level)
        {
            this.marker = marker;
            this.level = level;
        }

        [LocalizedDescription("OutputPanel.Description.Marker")]
        public String Marker
        {
            get { return this.marker; }
            set { this.marker = value; }
        }

        [LocalizedDescription("OutputPanel.Description.Level")]
        public LogLevel Level
        {
            get { return this.level; }
            set { this.level = value; }
        }

        /// <summary>
        /// Use shorten name for the marker item.
        /// </summary>
        public override string ToString()
        {
            return "HighlightMarker";
        }

    }

    [Serializable]
    public enum LogLevel
    {
        Info,
        Debug,
        Warning,
        Error,
        Fatal
    }

}
