using System;
using System.Collections.Generic;
using PluginCore;
using PluginCore.Localization;
using PluginCore.Managers;
using WeifenLuo.WinFormsUI.Docking;

namespace ResultsPanel.Helpers
{
    internal static class ResultsPanelHelper
    {
        private static PluginMain main;
        private static PluginUI mainUI;
        private static Dictionary<string, PluginUI> pluginUIs;

        internal static PluginUI ActiveUI { get; set; }

        internal static void Initialize(PluginMain pluginMain, PluginUI pluginUI)
        {
            main = pluginMain;
            mainUI = pluginUI;
            pluginUIs = new Dictionary<string, PluginUI>();
            ActiveUI = mainUI;
        }

        internal static void ClearResults(string groupId)
        {
            if (groupId == null)
            {
                mainUI.ClearOutput();
            }
            else
            {
                PluginUI ui;
                if (pluginUIs.TryGetValue(groupId, out ui)) ui.ClearOutput();
            }
        }

        internal static void ShowResults(string groupId)
        {
            PluginUI ui;
            if (groupId == null)
            {
                ui = mainUI;
            }
            if (!pluginUIs.TryGetValue(groupId, out ui))
            {
                ui = AddResultsPanel(groupId);
            }

            ui.AddLogEntries();
            ui.DisplayOutput();
        }

        internal static void ApplySettings()
        {
            mainUI.ApplySettings();
            foreach (var pluginUI in pluginUIs.Values)
            {
                pluginUI.ApplySettings();
            }
        }

        public static void OnTrace()
        {
            foreach (var trace in TraceManager.TraceLog)
            {
                if (trace.Group != null && !pluginUIs.ContainsKey(trace.Group))
                {
                    AddResultsPanel(trace.Group);
                }
            }

            mainUI.AddLogEntries();
            foreach (var pluginUI in pluginUIs.Values)
            {
                pluginUI.AddLogEntries();
            }
        }

        internal static void OnFileOpen(TextEvent e)
        {
            mainUI.AddSquiggles(e.Value);
            foreach (var pluginUI in pluginUIs.Values)
            {
                pluginUI.AddSquiggles(e.Value);
            }
        }

        /// <summary>
        /// Removes all results panels that are managed by this helper, so they are not saved in the layout file.
        /// This should be called when FlashDevelop is closing.
        /// </summary>
        internal static void RemoveResultsPanels()
        {
            foreach (var pluginUI in pluginUIs.Values)
            {
                pluginUI.ParentPanel.Close();
            }
        }
        
        /// <summary>
        /// Creates a new results panel.
        /// </summary>
        /// <param name="groupId">The trace group of the results panel.</param>
        private static PluginUI AddResultsPanel(string groupId)
        {
            var traceGroup = TraceManager.GetTraceGroup(groupId); // Group must exist
            var ui = new PluginUI(main, groupId);
            ui.Text = traceGroup.Title ?? TextHelper.GetString("Title.PluginPanel");
            ui.ParentPanel = PluginBase.MainForm.CreateDockablePanel(ui, "", traceGroup.Icon ?? main.pluginImage, DockState.DockBottomAutoHide);
            pluginUIs.Add(groupId, ui);
            return ui;
        }
    }
}
