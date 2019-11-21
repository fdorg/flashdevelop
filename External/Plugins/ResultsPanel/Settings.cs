using System;
using System.ComponentModel;
using PluginCore.Localization;

namespace ResultsPanel
{
    [Serializable]
    public class Settings
    {
        private bool scrollToBottom = false;
        private GroupingMethod defaultGroup = GroupingMethod.File;
        private bool highlightOnlyActivePanelEntries = false;
        private bool keepResultsByDefault = false;

        [DisplayName("Scroll To Bottom")]
        [LocalizedDescription("ResultsPanel.Description.ScrollToBottom"), DefaultValue(false)]
        public bool ScrollToBottom
        {
            get => scrollToBottom;
            set => scrollToBottom = value;
        }

        [DisplayName("Default Grouping")]
        [LocalizedDescription("ResultsPanel.Description.DefaultGrouping"), DefaultValue(GroupingMethod.File)]
        public GroupingMethod DefaultGrouping
        {
            get => defaultGroup;
            set => defaultGroup = value;
        }

        [DisplayName("Highlight Only Active Panel Entries")]
        [LocalizedDescription("ResultsPanel.Description.HighlightOnlyActivePanelEntries"), DefaultValue(false)]
        public bool HighlightOnlyActivePanelEntries
        {
            get => highlightOnlyActivePanelEntries;
            set => highlightOnlyActivePanelEntries = value;
        }

        [DisplayName("Keep Results By Default")]
        [LocalizedDescription("ResultsPanel.Description.KeepResultsByDefault"), DefaultValue(false)]
        public bool KeepResultsByDefault
        {
            get => keepResultsByDefault;
            set => keepResultsByDefault = value;
        }
    }

}
