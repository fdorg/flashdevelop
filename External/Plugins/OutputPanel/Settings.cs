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
        private bool showOnOutput = false;
        private bool showOnProcessEnd = false;
        private bool useLegacyColoring = false;
        private List<HighlightMarker> highlightMarkers;
        private bool wrapOutput = false;
        private ClearModeAction clearMode = ClearModeAction.OnEveryProcess;

        /// <summary> 
        /// Get and sets the alwaysShow
        /// </summary>
        [DisplayName("Show On Output")] 
        [LocalizedDescription("OutputPanel.Description.ShowOnOutput"), DefaultValue(false)]
        public bool ShowOnOutput 
        {
            get => this.showOnOutput;
            set => this.showOnOutput = value;
        }

        /// <summary> 
        /// Get and sets the showOnProcessEnd
        /// </summary>
        [DisplayName("Show On Process End")] 
        [LocalizedDescription("OutputPanel.Description.ShowOnProcessEnd"), DefaultValue(false)]
        public bool ShowOnProcessEnd 
        {
            get => this.showOnProcessEnd;
            set => this.showOnProcessEnd = value;
        }

        /// <summary> 
        /// Get and sets the wrapOutput
        /// </summary>
        [DisplayName("Wrap Output")]
        [LocalizedDescription("OutputPanel.Description.WrapOutput"), DefaultValue(false)]
        public bool WrapOutput
        {
            get => this.wrapOutput;
            set => this.wrapOutput = value;
        }

        /// <summary> 
        /// Get and sets the wrapOutput
        /// </summary>
        [DisplayName("Use Legacy Coloring")]
        [LocalizedDescription("OutputPanel.Description.UseLegacyColoring"), DefaultValue(false)]
        public bool UseLegacyColoring
        {
            get => this.useLegacyColoring;
            set => this.useLegacyColoring = value;
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
            set => this.highlightMarkers = value;
        }

        /// <summary> 
        /// Get and sets the way in which the output text will be cleared
        /// </summary>
        [DisplayName("Clear Mode")]
        [LocalizedDescription("OutputPanel.Description.ClearMode"), DefaultValue(ClearModeAction.OnEveryProcess)]
        public ClearModeAction ClearMode
        {
            get => this.clearMode;
            set => this.clearMode = value;
        }

    }

    public enum ClearModeAction
    {
        OnEveryProcess,
        OnBuildStart,
        Manual
    }

    [Serializable]
    public class HighlightMarker
    {
        public string marker = "Info:";
        public LogLevel level = LogLevel.Debug;
        
        public HighlightMarker(){}
        public HighlightMarker(string marker, LogLevel level)
        {
            this.marker = marker;
            this.level = level;
        }

        [LocalizedDescription("OutputPanel.Description.Marker")]
        public string Marker
        {
            get => this.marker;
            set => this.marker = value;
        }

        [LocalizedDescription("OutputPanel.Description.Level")]
        public LogLevel Level
        {
            get => this.level;
            set => this.level = value;
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
