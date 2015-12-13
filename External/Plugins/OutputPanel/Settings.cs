using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
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
                if (highlightMarkers == null)
                {
                    this.highlightMarkers = new List<HighlightMarker>
                    {
                        new HighlightMarker("Info:", LogLevel.Info),
                        new HighlightMarker("Debug:", LogLevel.Debug),
                        new HighlightMarker("Warning:", LogLevel.Warning),
                        new HighlightMarker("Error:", LogLevel.Error),
                        new HighlightMarker("Fatal:", LogLevel.Fatal)
                    };
                }
                return highlightMarkers;
            }
            set { this.highlightMarkers = value; }
        }

    }

    [Serializable]
    public class HighlightMarker
    {
        private String marker;
        private LogLevel level;
        private Color highlightColor;

        public HighlightMarker() : this(String.Empty, LogLevel.Custom)
        {
        }

        public HighlightMarker(String marker, LogLevel level)
        {
            this.marker = marker;
            this.level = level;
            this.highlightColor = Color.Empty;
        }

        public HighlightMarker(String marker, Color highlightColor)
        {
            this.marker = marker;
            this.level = LogLevel.Custom;
            this.highlightColor = highlightColor;
        }

        [Category("Properties")]
        [LocalizedDescription("OutputPanel.Description.Marker")]
        [DefaultValue("")]
        public String Marker
        {
            get { return this.marker; }
            set { this.marker = value; }
        }

        [Category("Properties")]
        [LocalizedDescription("OutputPanel.Description.Level")]
        [DefaultValue(LogLevel.Debug)]
        public LogLevel Level
        {
            get { return this.level; }
            set
            {
                this.level = value;
                if (value != LogLevel.Custom) this.highlightColor = Color.Empty;
            }
        }

        [Category("Properties")]
        [Description("User defined color for custom highlight markers.")]
        [DefaultValue(typeof(Color), "Empty")]
        public Color HighlightColor
        {
            get { return this.highlightColor; }
            set
            {
                this.level = LogLevel.Custom;
                this.highlightColor = value;
            }
        }

        [Browsable(false)]
        public Boolean IsValid
        {
            get
            {
                return this.level != LogLevel.Custom || !this.highlightColor.IsEmpty;
            }
        }

        /// <summary>
        /// Use shorten name for the marker item.
        /// </summary>
        public override string ToString()
        {
            return IsValid ? marker + "(" + level + ")" : "New HighlightMarker";
        }

    }

    [Serializable]
    public enum LogLevel
    {
        Info,
        Debug,
        Warning,
        Error,
        Fatal,
        Custom,
        [Browsable(false)]
        ProcessStart = -1,
        [Browsable(false)]
        ProcessEnd = -2,
        [Browsable(false)]
        ProcessError = -3
    }

}
