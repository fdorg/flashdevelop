using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using PluginCore;
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
            get => showOnOutput;
            set => showOnOutput = value;
        }

        /// <summary> 
        /// Get and sets the showOnProcessEnd
        /// </summary>
        [DisplayName("Show On Process End")] 
        [LocalizedDescription("OutputPanel.Description.ShowOnProcessEnd"), DefaultValue(false)]
        public bool ShowOnProcessEnd 
        {
            get => showOnProcessEnd;
            set => showOnProcessEnd = value;
        }

        /// <summary> 
        /// Get and sets the wrapOutput
        /// </summary>
        [DisplayName("Wrap Output")]
        [LocalizedDescription("OutputPanel.Description.WrapOutput"), DefaultValue(false)]
        public bool WrapOutput
        {
            get => wrapOutput;
            set => wrapOutput = value;
        }

        /// <summary> 
        /// Get and sets the wrapOutput
        /// </summary>
        [DisplayName("Use Legacy Coloring")]
        [LocalizedDescription("OutputPanel.Description.UseLegacyColoring"), DefaultValue(false)]
        public bool UseLegacyColoring
        {
            get => useLegacyColoring;
            set => useLegacyColoring = value;
        }

        [DisplayName("Highlight Markers")]
        [LocalizedDescription("OutputPanel.Description.HighlightMarkers")]
        [Editor(typeof(DescriptiveCollectionEditor), typeof(UITypeEditor))]
        public List<HighlightMarker> HighlightMarkers
        {
            get
            {
                if (highlightMarkers.IsNullOrEmpty())
                {
                    highlightMarkers = new List<HighlightMarker>();
                    highlightMarkers.Add(new HighlightMarker("Info:", LogLevel.Info));
                    highlightMarkers.Add(new HighlightMarker("Debug:", LogLevel.Debug));
                    highlightMarkers.Add(new HighlightMarker("Warning:", LogLevel.Warning));
                    highlightMarkers.Add(new HighlightMarker("Error:", LogLevel.Error));
                    highlightMarkers.Add(new HighlightMarker("Fatal:", LogLevel.Fatal));
                }
                return highlightMarkers;
            }
            set => highlightMarkers = value;
        }

        /// <summary> 
        /// Get and sets the way in which the output text will be cleared
        /// </summary>
        [DisplayName("Clear Mode")]
        [LocalizedDescription("OutputPanel.Description.ClearMode"), DefaultValue(ClearModeAction.OnEveryProcess)]
        public ClearModeAction ClearMode
        {
            get => clearMode;
            set => clearMode = value;
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
            get => marker;
            set => marker = value;
        }

        [LocalizedDescription("OutputPanel.Description.Level")]
        public LogLevel Level
        {
            get => level;
            set => level = value;
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
