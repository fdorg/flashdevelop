using System;
using System.ComponentModel;
using System.Windows.Forms;
using PluginCore.Localization;
using PluginCore.Utilities;
using PluginCore;
using ResultsPanel.Helpers;

namespace ResultsPanel
{
    class PanelContextMenu : ContextMenuStrip
    {
        internal const Keys CopyEntryKeys = Keys.Control | Keys.C;
        internal const Keys IgnoreEntryKeys = Keys.Delete;

        public PanelContextMenu()
        {
            ClearEntries = new ToolStripMenuItem(TextHelper.GetString("Label.ClearEntries"), null, ClearEntries_Click);
            CopyEntry = new ToolStripMenuItem(TextHelper.GetString("Label.CopyEntry"), null, CopyEntry_Click);
            IgnoreEntry = new ToolStripMenuItem(TextHelper.GetString("Label.IgnoreEntry"), null, IgnoreEntry_Click);
            ClearIgnoredEntries = new ToolStripMenuItem(TextHelper.GetString("Label.ClearIgnoredEntries"), null, ClearIgnoredEntries_Click);
            NextEntry = new ToolStripMenuItem(TextHelper.GetString("Label.NextEntry"), null, NextEntry_Click);
            PreviousEntry = new ToolStripMenuItem(TextHelper.GetString("Label.PreviousEntry"), null, PreviousEntry_Click);

            CopyEntry.ShortcutKeyDisplayString = DataConverter.KeysToString(CopyEntryKeys);
            IgnoreEntry.ShortcutKeyDisplayString = DataConverter.KeysToString(IgnoreEntryKeys);
            
            Items.AddRange(new ToolStripItem[]
            {
                ClearEntries,
                CopyEntry,
                IgnoreEntry,
                ClearIgnoredEntries,
                NextEntry,
                PreviousEntry,
            });

            Font = PluginBase.Settings.DefaultFont;
            Renderer = new DockPanelStripRenderer(false);
        }

        public ToolStripMenuItem ClearEntries { get; }

        public ToolStripMenuItem CopyEntry { get; }

        public ToolStripMenuItem IgnoreEntry { get; }

        public ToolStripMenuItem ClearIgnoredEntries { get; }

        public ToolStripMenuItem NextEntry { get; }

        public ToolStripMenuItem PreviousEntry { get; }
        
        protected override void OnOpening(CancelEventArgs e)
        {
            ResultsPanelHelper.ActiveUI = (PluginUI) SourceControl.Parent;
            NextEntry.Enabled = PreviousEntry.Enabled = ClearEntries.Enabled = ResultsPanelHelper.ActiveUI.EntriesView.Items.Count > 0;
            IgnoreEntry.Enabled = CopyEntry.Enabled = ResultsPanelHelper.ActiveUI.EntriesView.SelectedItems.Count > 0;
            ClearIgnoredEntries.Enabled = ResultsPanelHelper.ActiveUI.IgnoredEntries.Count > 0;
            base.OnOpening(e);
        }
        
        private void ClearEntries_Click(object sender, EventArgs e) => ResultsPanelHelper.ActiveUI.ClearOutput();

        private void CopyEntry_Click(object sender, EventArgs e) => ResultsPanelHelper.ActiveUI.CopyText();

        private void IgnoreEntry_Click(object sender, EventArgs e) => ResultsPanelHelper.ActiveUI.IgnoreEntry();

        private void ClearIgnoredEntries_Click(object sender, EventArgs e) => ResultsPanelHelper.ActiveUI.ClearIgnoredEntries();

        private void NextEntry_Click(object sender, EventArgs e) => ResultsPanelHelper.ActiveUI.NextEntry();

        private void PreviousEntry_Click(object sender, EventArgs e) => ResultsPanelHelper.ActiveUI.PreviousEntry();
    }
}
