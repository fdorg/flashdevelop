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

        internal static void OnTrace()
        {
            var traces = TraceManager.TraceLog;
            foreach (var trace in traces)
            {
                PluginUI ui;
                if (!pluginUis.TryGetValue(trace.Group, out ui))
                    ui = AddResultsPanel(TextHelper.GetString("Title.PluginPanel"), trace.Group);

                ui.AddLogEntries();
            }

            foreach (var ui in pluginUis.Values)
            {
                ui.DisplayOutput();
            }
        }

        /// <summary>
        /// Creates a new results panel.
        /// </summary>
        /// <param name="title">The title of the panel</param>
        /// <param name="group">The group of the results panel</param>
        private static PluginUI AddResultsPanel(string title, string group)
        {
            var ui = new PluginUI(main, group);
            ui.Text = title;
            PluginBase.MainForm.CreateDockablePanel(ui, MANAGER_GUID, main.pluginImage, DockState.DockBottomAutoHide);
            pluginUis.Add(group, ui);

            return ui;
        }
    }
}
