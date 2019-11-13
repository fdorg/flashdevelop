using System;
using System.Collections.Generic;
using System.IO;
using FlashDevelop.Docking;
using PluginCore;
using PluginCore.Helpers;
using PluginCore.Managers;
using WeifenLuo.WinFormsUI.Docking;

namespace FlashDevelop.Managers
{
    class LayoutManager
    {
        public static List<DockContent> PluginPanels;
        private static readonly DeserializeDockContent contentDeserializer;
        private static readonly HashSet<string> savedPersistStrings;
        private static readonly List<DockContent> dynamicContentTemplates;

        static LayoutManager()
        {
            PluginPanels = new List<DockContent>();
            contentDeserializer = GetContentFromPersistString;
            savedPersistStrings = new HashSet<string>();
            dynamicContentTemplates = new List<DockContent>();
        }

        /// <summary>
        /// Builds the look of the layout systems
        /// </summary>
        public static void BuildLayoutSystems(string file)
        {
            try
            {
                FileHelper.EnsureUpdatedFile(file);
                if (File.Exists(file))
                {
                    var dockPanel = Globals.MainForm.DockPanel;
                    var documents = dockPanel.GetDocuments();
                    savedPersistStrings.Clear();
                    CloseDocumentContents(documents);
                    ClosePluginPanelContents();
                    CloseDynamicContentTemplates();
                    dockPanel.LoadFromXml(file, contentDeserializer);
                    RestoreDynamicContentTemplates();
                    RestoreUnrestoredPlugins(dockPanel);
                    ShowDocumentContents(documents, dockPanel);
                }
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Retrieves the content by persist string
        /// </summary>
        private static DockContent GetContentFromPersistString(string persistString)
        {
            foreach (var pluginPanel in PluginPanels)
            {
                if (pluginPanel.GetPersistString() != persistString) continue;
                if (pluginPanel.DockPanel is null) // Duplicate persistString
                {
                    savedPersistStrings.Add(persistString);
                    return pluginPanel;
                }
            }
            if (persistString == typeof(TabbedDocument).ToString())
            {
                return null;
            }
            foreach (var template in dynamicContentTemplates)
            {
                if (template.GetPersistString() != persistString) continue;
                // Choose the first template content layout
                // During layout reload, template may already exist, in which case DockPanel is null from CloseDynamicContentTemplates()
                if (template.DockPanel is null)
                {
                    savedPersistStrings.Add(persistString);
                    return template;
                }
                return null;
            }
            var newTemplate = new DockablePanel.Template(persistString);
            dynamicContentTemplates.Add(newTemplate);
            savedPersistStrings.Add(persistString);
            return newTemplate;
        }

        internal static void SetContentLayout(DockablePanel dockablePanel, string persistString)
        {
            for (int i = 0; i < PluginPanels.Count; i++)
            {
                var pluginPanel = PluginPanels[i];
                if (pluginPanel.DockPanel is null)
                {
                    PluginPanels.RemoveAt(i--);
                }
                else if (pluginPanel.GetPersistString() == persistString)
                {
                    dockablePanel.DockPanel = pluginPanel.DockPanel;
                    dockablePanel.AutoHidePortion = pluginPanel.AutoHidePortion;
                    dockablePanel.IsFloat = pluginPanel.IsFloat;
                    dockablePanel.Pane = pluginPanel.Pane;
                    return;
                }
            }
            for (int i = 0; i < dynamicContentTemplates.Count; i++)
            {
                var template = dynamicContentTemplates[i];
                if (template.GetPersistString() == persistString)
                {
                    dockablePanel.DockPanel = template.DockPanel;
                    dockablePanel.AutoHidePortion = template.AutoHidePortion;
                    dockablePanel.IsFloat = template.IsFloat;
                    dockablePanel.Pane = template.Pane;

                    // No need for a template if a new window exists.
                    dynamicContentTemplates.RemoveAt(i);
                    template.Close();
                    return;
                }
            }
            dockablePanel.Show();
        }

        /// <summary>
        /// Closes the document contents for xml restoring
        /// </summary>
        private static void CloseDocumentContents(IDockContent[] documents)
        {
            foreach (DockContent document in documents)
            {
                document.DockPanel = null;
            }
        }

        /// <summary>
        /// Shows the document contents for xml restoring
        /// </summary>
        private static void ShowDocumentContents(IDockContent[] documents, DockPanel dockPanel)
        {
            foreach (DockContent document in documents)
            {
                document.Show(dockPanel, DockState.Document);
            }
        }

        /// <summary>
        /// Closes the plugin panel contents for xml restoring
        /// </summary>
        private static void ClosePluginPanelContents()
        {
            for (int i = PluginPanels.Count - 1; i >= 0; i--)
            {
                var pluginPanel = PluginPanels[i];
                if (pluginPanel.DockPanel is null)
                {
                    PluginPanels.RemoveAt(i);
                }
                else if (pluginPanel.DockState != DockState.Document)
                {
                    pluginPanel.DockPanel = null;
                }
            }
        }

        /// <summary>
        /// Restore the plugins that have not been restored yet.
        /// These plugins may have been added later to the plugins dir.
        /// </summary>
        private static void RestoreUnrestoredPlugins(DockPanel dockPanel)
        {
            foreach (var pluginPanel in PluginPanels)
            {
                if (!savedPersistStrings.Contains(pluginPanel.GetPersistString()))
                {
                    pluginPanel.Show(dockPanel, DockState.Float);
                }
            }
        }

        private static void CloseDynamicContentTemplates()
        {
            foreach (var template in dynamicContentTemplates)
            {
                template.DockPanel = null;
            }
        }

        private static void RestoreDynamicContentTemplates()
        {
            for (int i = dynamicContentTemplates.Count - 1; i >= 0; i--)
            {
                var template = dynamicContentTemplates[i];
                if (template.DockPanel is null)
                {
                    dynamicContentTemplates.RemoveAt(i);
                }
                else
                {
                    template.Hide();
                }
            }
        }

        /// <summary>
        /// Restores the specified panel layout
        /// </summary>
        public static void RestoreLayout(string file)
        {
            try
            {
                Globals.MainForm.RestoringContents = true;
                TextEvent te = new TextEvent(EventType.RestoreLayout, file);
                EventManager.DispatchEvent(Globals.MainForm, te);
                if (!te.Handled)
                {
                    BuildLayoutSystems(file);
                }
                Globals.MainForm.RestoringContents = false;
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

    }

}
