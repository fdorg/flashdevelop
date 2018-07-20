﻿using System;
using System.ComponentModel;
using PluginCore.Localization;

namespace ResultsPanel
{
    [Serializable]
    public class Settings
    {
        private Boolean scrollToBottom = false;
        private GroupingMethod defaultGroup = GroupingMethod.File;
        private Boolean highlightOnlyActivePanelEntries = false;
        private Boolean keepResultsByDefault = false;

        [DisplayName("Scroll To Bottom")]
        [LocalizedDescription("ResultsPanel.Description.ScrollToBottom"), DefaultValue(false)]
        public Boolean ScrollToBottom
        {
            get { return scrollToBottom; }
            set { scrollToBottom = value; }
        }

        [DisplayName("Default Grouping")]
        [LocalizedDescription("ResultsPanel.Description.DefaultGrouping"), DefaultValue(GroupingMethod.File)]
        public GroupingMethod DefaultGrouping
        {
            get { return defaultGroup; }
            set { defaultGroup = value; }
        }

        [DisplayName("Highlight Only Active Panel Entries")]
        [LocalizedDescription("ResultsPanel.Description.HighlightOnlyActivePanelEntries"), DefaultValue(false)]
        public bool HighlightOnlyActivePanelEntries
        {
            get { return highlightOnlyActivePanelEntries; }
            set { highlightOnlyActivePanelEntries = value; }
        }

        [DisplayName("Keep Results By Default")]
        [LocalizedDescription("ResultsPanel.Description.KeepResultsByDefault"), DefaultValue(false)]
        public bool KeepResultsByDefault
        {
            get { return keepResultsByDefault; }
            set { keepResultsByDefault = value; }
        }
    }

}
