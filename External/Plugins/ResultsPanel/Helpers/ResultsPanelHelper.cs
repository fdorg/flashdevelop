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
        private static Dictionary<string, List<PluginUI>> multipleUIs; // { groupId, List<PluginUI> }
        private static Dictionary<string, PluginUI> pluginUIs; // { groupData, PluginUI }

        internal static PluginUI ActiveUI { get; set; }

        internal static void Initialize(PluginMain pluginMain, PluginUI pluginUI)
        {
            main = pluginMain;
            mainUI = pluginUI;
            multipleUIs = new Dictionary<string, List<PluginUI>>();
            pluginUIs = new Dictionary<string, PluginUI>();
            ActiveUI = mainUI;

            mainUI.ParentPanel.Tag = mainUI;
            mainUI.ParentPanel.IsActivatedChanged += ParentPanel_IsActivatedChanged;
        }

        internal static void ClearResults(string groupData)
        {
            if (groupData == null)
            {
                mainUI.ClearOutput();
            }
            else
            {
                PluginUI ui;
                if (pluginUIs.TryGetValue(groupData, out ui)) ui.ClearOutput();
            }
        }

        internal static void ShowResults(string groupData)
        {
            PluginUI ui;
            if (groupData == null)
            {
                ui = mainUI;
            }
            if (!pluginUIs.TryGetValue(groupData, out ui))
            {
                ui = AddResultsPanel(groupData);
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
                if (trace.GroupData != null && !pluginUIs.ContainsKey(trace.GroupData))
                {
                    AddResultsPanel(trace.GroupData);
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
            ActiveUI = mainUI;
        }
        
        /// <summary>
        /// Creates a new results panel.
        /// </summary>
        /// <param name="groupData">
        /// The trace group of the results panel.
        /// <para/>
        /// Format: <c>GroupID:arg1,arg2,...</c>
        /// </param>
        private static PluginUI AddResultsPanel(string groupData)
        {
            string groupId;
            string[] args;
            TraceManager.ParseGroupData(groupData, out groupId, out args);

            var traceGroup = TraceManager.GetTraceGroup(groupId); // Group must exist
            var ui = new PluginUI(main, groupData, groupId);
            ui.Text = string.Format(traceGroup.Title ?? TextHelper.GetString("Title.PluginPanel"), args);
            ui.ParentPanel = PluginBase.MainForm.CreateDynamicPersistDockablePanel(ui, main.Guid, groupId, traceGroup.Icon ?? main.pluginImage, DockState.DockBottomAutoHide);
            ui.ParentPanel.Tag = ui;
            ui.ParentPanel.DockStateChanged += ParentPanel_DockStateChanged;
            ui.ParentPanel.IsActivatedChanged += ParentPanel_IsActivatedChanged;
            pluginUIs.Add(groupData, ui);

            if (args.Length > 0) // Multiple instances for one group id
            {
                if (!multipleUIs.ContainsKey(groupId))
                {
                    multipleUIs.Add(groupId, new List<PluginUI>() { ui });
                }
                else
                {
                    var list = multipleUIs[groupId];
                    if (list.Count == 1 && list[0].ParentPanel.IsHidden)
                    {
                        list[0].ParentPanel.Close();
                        list.Clear();
                    }
                    list.Add(ui);
                }
            }
            return ui;
        }

        private static void ParentPanel_DockStateChanged(object sender, EventArgs e)
        {
            var ui = (PluginUI) ((DockContent) sender).Tag;
            if (ui.ParentPanel.IsHidden)
            {
                ui.OnPanelHidden();
                ui.ParentPanel.DockStateChanged -= ParentPanel_DockStateChanged;
                ui.ParentPanel.IsActivatedChanged -= ParentPanel_IsActivatedChanged;

                List<PluginUI> list;
                if (multipleUIs.TryGetValue(ui.GroupId, out list) && list.Count > 1)
                {
                    ui.ParentPanel.Close();
                    list.Remove(ui);
                }
            }
        }

        private static void ParentPanel_IsActivatedChanged(object sender, EventArgs e)
        {
            var ui = (PluginUI) ((DockContent) sender).Tag;
            if (ui.ParentPanel.IsActivated)
            {
                ui.OnPanelActivated();
            }
        }
    }
}
