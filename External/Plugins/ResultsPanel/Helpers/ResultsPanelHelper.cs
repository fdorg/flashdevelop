// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
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
        private static List<PluginUI> pluginUIs;

        internal static PluginUI ActiveUI { get; set; }

        internal static IList<PluginUI> PluginUIs => pluginUIs;

        internal static PluginUI MainUI { get; private set; }

        internal static void Initialize(PluginMain pluginMain, PluginUI pluginUI)
        {
            main = pluginMain;
            MainUI = pluginUI;
            pluginUIs = new List<PluginUI>();
            ActiveUI = MainUI;
            MainUI.ParentPanel.Tag = MainUI;
            MainUI.ParentPanel.IsActivatedChanged += ParentPanel_IsActivatedChanged;
        }

        /// <summary>
        /// Clears an existing panel, or creates a new panel with <paramref name="groupData"/>.
        /// </summary>
        internal static void ClearResults(string groupData)
        {
            if (groupData is null)
            {
                MainUI.ClearOutput();
                return;
            }

            TraceManager.ParseGroupData(groupData, out var groupId, out var args);
            foreach (var pluginUI in pluginUIs)
            {
                if (pluginUI.GroupData == groupData ||
                    pluginUI.GroupId == groupId && !pluginUI.Locked)
                {
                    UpdateResultsPanel(pluginUI, groupData, groupId, args);
                    pluginUI.ClearOutput();
                    return;
                }
            }
            AddResultsPanel(groupData, groupId, args);
        }

        /// <summary>
        /// Displays an existing panel, or creates a new panel with <paramref name="groupData"/> and displays it.
        /// </summary>
        internal static void ShowResults(string groupData)
        {
            if (groupData is null)
            {
                MainUI.AddLogEntries();
                MainUI.DisplayOutput();
                return;
            }

            TraceManager.ParseGroupData(groupData, out var groupId, out var args);

            foreach (var pluginUI in pluginUIs)
            {
                if (pluginUI.GroupData == groupData ||
                    pluginUI.GroupId == groupId && !pluginUI.Locked)
                {
                    pluginUI.AddLogEntries();
                    pluginUI.DisplayOutput();
                    return;
                }
            }

            var newUI = AddResultsPanel(groupData, groupId, args);
            newUI.AddLogEntries();
            newUI.DisplayOutput();
        }

        /// <summary>
        /// Apply settings to all panels.
        /// </summary>
        internal static void ApplySettings()
        {
            MainUI.ApplySettings();
            foreach (var pluginUI in pluginUIs)
            {
                pluginUI.ApplySettings();
                pluginUI.ClearSquiggles();
            }

            MainUI.ClearSquiggles();
            MainUI.AddSquiggles();
            if (MainUI.Settings.HighlightOnlyActivePanelEntries)
            {
                if (ActiveUI.GroupId != null)
                {
                    ActiveUI.AddSquiggles();
                }
            }
            else
            {
                foreach (var pluginUI in pluginUIs)
                {
                    if (!pluginUI.ParentPanel.IsHidden)
                    {
                        pluginUI.AddSquiggles();
                    }
                }
            }
        }

        /// <summary>
        /// Update all panels.
        /// </summary>
        public static void OnTrace()
        {
            MainUI.AddLogEntries();
            foreach (var pluginUI in pluginUIs)
            {
                pluginUI.AddLogEntries();
            }
        }

        /// <summary>
        /// Add highlights to the open file.
        /// </summary>
        internal static void OnFileOpen(TextEvent e)
        {
            MainUI.AddSquiggles(e.Value);

            if (MainUI.Settings.HighlightOnlyActivePanelEntries)
            {
                if (ActiveUI.GroupId != null)
                {
                    ActiveUI.AddSquiggles(e.Value);
                }
            }
            else
            {
                foreach (var pluginUI in pluginUIs)
                {
                    pluginUI.AddSquiggles(e.Value);
                }
            }
        }

        /// <summary>
        /// Creates a new results panel.
        /// </summary>
        /// <param name="groupData">
        /// The trace group of the results panel.
        /// <para/>
        /// Format: <c>GroupID:arg1,arg2,...</c>
        /// </param>
        private static PluginUI AddResultsPanel(string groupData, string groupId, string[] args)
        {
            var traceGroup = TraceManager.GetTraceGroup(groupId); // Group must exist
            var ui = new PluginUI(main, groupData, groupId, traceGroup.ShowFilterButtons, traceGroup.AllowMultiplePanels);
            ui.Text = string.Format(traceGroup.Title ?? TextHelper.GetString("Title.PluginPanel"), args);
            ui.ParentPanel = PluginBase.MainForm.CreateDynamicPersistDockablePanel(ui, main.Guid, groupId, traceGroup.Icon ?? main.pluginImage, DockState.DockBottomAutoHide);
            ui.ParentPanel.Tag = ui;
            ui.ParentPanel.DockStateChanged += ParentPanel_DockStateChanged;
            ui.ParentPanel.IsActivatedChanged += ParentPanel_IsActivatedChanged;

            if (args.Length > 0) // Possible multiple instances for one group id
            {
                foreach (var pluginUI in pluginUIs)
                {
                    if (pluginUI.GroupId == groupId && pluginUI.ParentPanel.IsHidden)
                    {
                        CloseResultsPanel(pluginUI);
                        break; // There should only be one panel hidden without being closed
                    }
                }
            }
            pluginUIs.Add(ui);

            return ui;
        }

        /// <summary>
        /// Update an existing panel to use for a new group data.
        /// </summary>
        private static void UpdateResultsPanel(PluginUI ui, string groupData, string groupId, string[] args)
        {
            if (ui.GroupData != groupData)
            {
                var traceGroup = TraceManager.GetTraceGroup(groupId);
                ui.GroupData = groupData;
                ui.Text = string.Format(traceGroup.Title ?? TextHelper.GetString("Title.PluginPanel"), args);
                ui.ParentPanel.Text = ui.Text;
            }
        }

        /// <summary>
        /// Close the panel and remove it from the list.
        /// </summary>
        private static void CloseResultsPanel(PluginUI ui)
        {
            ui.ParentPanel.DockStateChanged -= ParentPanel_DockStateChanged;
            ui.ParentPanel.IsActivatedChanged -= ParentPanel_IsActivatedChanged;
            ui.ParentPanel.Close();
            pluginUIs.Remove(ui);
        }

        private static void ParentPanel_DockStateChanged(object sender, EventArgs e)
        {
            var ui = (PluginUI) ((DockContent) sender).Tag;
            if (ui.ParentPanel.IsHidden)
            {
                ui.OnPanelHidden();

                foreach (var pluginUI in pluginUIs)
                {
                    if (pluginUI != ui && pluginUI.GroupId == ui.GroupId)
                    {
                        // ui is not the only panel with the group id, so safely close it
                        CloseResultsPanel(ui);
                        break;
                    }
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
