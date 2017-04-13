using System.Collections.Generic;
using PluginCore;
using PluginCore.Localization;
using PluginCore.Managers;
using WeifenLuo.WinFormsUI.Docking;

namespace ResultsPanel.Managers
{
    static class ResultsPanelManager
    {
        private static readonly Dictionary<string, PluginUI> pluginUis = new Dictionary<string, PluginUI>();

        private static PluginMain main;
        private const string MANAGER_GUID = "C32104FA-0E7D-4463-A705-8B1C2580B53A";

        internal static void Init(PluginMain m)
        {
            main = m;
        }

        internal static void Clear(string group)
        {
            if (group == null) return;

            PluginUI ui;
            if (pluginUis.TryGetValue(group, out ui))
            {
                ui.ClearOutput();
            }
        }

        internal static void ShowResults(string group)
        {
            PluginUI ui;
            if (!pluginUis.TryGetValue(group, out ui))
                ui = AddResultsPanel(group);

            ui.DisplayOutput();
        }

        internal static void OnTrace()
        {
            var traces = TraceManager.TraceLog;
            foreach (var trace in traces)
            {
                if (trace.Group == null) return; //null is handled by default panel

                PluginUI ui;
                if (!pluginUis.TryGetValue(trace.Group, out ui))
                    ui = AddResultsPanel(trace.Group);

                ui.AddLogEntries();
            }
        }

        /// <summary>
        /// Creates a new results panel.
        /// </summary>
        /// <param name="title">The title of the panel</param>
        /// <param name="group">The group of the results panel</param>
        private static PluginUI AddResultsPanel(string group)
        {
            var ui = new PluginUI(main, group);
            ui.Text = TraceManager.GetTraceGroup(group)?.Title ?? TextHelper.GetString("Title.PluginPanel");
            PluginBase.MainForm.CreateDockablePanel(ui, MANAGER_GUID, main.pluginImage, DockState.DockBottomAutoHide);
            pluginUis.Add(group, ui);

            return ui;
        }
    }
}
