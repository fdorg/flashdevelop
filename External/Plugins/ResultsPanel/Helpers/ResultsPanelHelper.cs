using System.Collections.Generic;
using PluginCore;
using PluginCore.Localization;
using PluginCore.Managers;
using WeifenLuo.WinFormsUI.Docking;

namespace ResultsPanel.Helpers
{
    class ResultsPanelHelper
    {
        readonly Dictionary<string, PluginUI> pluginUis = new Dictionary<string, PluginUI>();
        readonly List<DockContent> pluginPanels = new List<DockContent>();

        readonly PluginMain main;

        public ResultsPanelHelper(PluginMain m)
        {
            main = m;
        }

        public void Clear(string group)
        {
            if (group == null) return;

            PluginUI ui;
            if (pluginUis.TryGetValue(group, out ui))
                ui.ClearOutput();
        }

        /// <summary>
        /// Removes all results panels that are managed by this helper, so they are not saved in the layout file.
        /// This should be called when FlashDevelop is closing.
        /// </summary>
        public void RemoveResultsPanels()
        {
            foreach (var panel in pluginPanels)
                panel.Close();
        }

        public void ShowResults(string group)
        {
            PluginUI ui;
            if (!pluginUis.TryGetValue(group, out ui))
                ui = AddResultsPanel(group);

            ui.DisplayOutput();
        }

        public void OnTrace()
        {
            var traces = TraceManager.TraceLog;
            foreach (var trace in traces)
            {
                if (trace.Group == null) continue; //null is handled by default panel

                if (!pluginUis.ContainsKey(trace.Group))
                    AddResultsPanel(trace.Group);
            }

            foreach (var ui in pluginUis.Values)
            {
                ui.AddLogEntries();
            }
        }

        /// <summary>
        /// Creates a new results panel.
        /// </summary>
        /// <param name="group">The group of the results panel</param>
        PluginUI AddResultsPanel(string group)
        {
            var traceGroup = TraceManager.GetTraceGroup(group);

            var ui = new PluginUI(main, group);
            ui.Text = traceGroup?.Title ?? TextHelper.GetString("Title.PluginPanel");

            pluginPanels.Add(PluginBase.MainForm.CreateDockablePanel(ui, "", traceGroup?.Icon ?? main.pluginImage, DockState.DockBottomAutoHide));
            pluginUis.Add(group, ui);

            return ui;
        }
    }
}
