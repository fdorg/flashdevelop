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
        private static DeserializeDockContent contentDeserializer;
        private static HashSet<string> savedPersistStrings;
        private static List<DockContent> dynamicContentTemplates;
 
        static LayoutManager()
        {
            PluginPanels = new List<DockContent>();
            contentDeserializer = new DeserializeDockContent(GetContentFromPersistString);
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
                var dockPanel = Globals.MainForm.DockPanel;
                if (File.Exists(file))
                {
                    savedPersistStrings.Clear();
                    CloseDocumentContents();
                    ClosePluginPanelContents();
                    CloseDynamicContentTemplates();
                    dockPanel.LoadFromXml(file, contentDeserializer);
                    RestoreDynamicContentTemplates();
                    RestoreUnrestoredPlugins();
                    ShowDocumentContents();
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
        private static DockContent GetContentFromPersistString(String persistString)
        {
            for (int i = 0; i < PluginPanels.Count; i++)
            {
                var pluginPanel = PluginPanels[i];
                if (pluginPanel.GetPersistString() == persistString)
                {
                    if (pluginPanel.DockPanel == null) // Duplicate persistString
                    {
                        savedPersistStrings.Add(persistString);
                        return pluginPanel;
                    }
                }
            }
            if (persistString == typeof(TabbedDocument).ToString())
            {
                return null;
            }
            for (int i = 0; i < dynamicContentTemplates.Count; i++)
            {
                var template = dynamicContentTemplates[i];
                if (template.GetPersistString() == persistString)
                {
                    // Choose the first template content layout
                    // During layout reload, template may already exist, in which case DockPanel == null from CloseDynamicContentTemplates()
                    if (template.DockPanel == null)
                    {
                        savedPersistStrings.Add(persistString);
                        return template;
                    }
                    return null;
                }
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
                if (pluginPanel.DockPanel == null)
                {
                    PluginPanels.RemoveAt(i--);
                }
                else if (pluginPanel.GetPersistString() == persistString)
                {
                    dockablePanel.DockPanel = pluginPanel.DockPanel;
                    dockablePanel.AutoHidePortion = pluginPanel.AutoHidePortion;
                    dockablePanel.IsFloat = pluginPanel.IsFloat;
                    dockablePanel.Pane = pluginPanel.Pane;
                    break;
                }
            }
            for (int i = 0; i < dynamicContentTemplates.Count; i++)
            {
                var template = dynamicContentTemplates[i];
                if (template.GetPersistString() == dockablePanel.GetPersistString())
                {
                    dockablePanel.DockPanel = template.DockPanel;
                    dockablePanel.AutoHidePortion = template.AutoHidePortion;
                    dockablePanel.IsFloat = template.IsFloat;
                    dockablePanel.Pane = template.Pane;

                    // No need for a template if a new window exists.
                    dynamicContentTemplates.RemoveAt(i);
                    template.Close();
                    break;
                }
            }
        }
        
        /// <summary>
        /// Shows the document contents for xml restoring
        /// </summary>
        private static void ShowDocumentContents()
        {
            var dockPanel = Globals.MainForm.DockPanel;
            var documents = dockPanel.GetDocuments();
            foreach (DockContent document in documents)
            {
                document.Show(dockPanel);
            }
        }

        /// <summary>
        /// Closes the document contents for xml restoring
        /// </summary>
        private static void CloseDocumentContents()
        {
            var dockPanel = Globals.MainForm.DockPanel;
            var documents = dockPanel.GetDocuments();
            foreach (DockContent document in documents)
            {
                document.DockPanel = null;
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
                if (pluginPanel.DockPanel == null)
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
        private static void RestoreUnrestoredPlugins()
        {
            var dockPanel = Globals.MainForm.DockPanel;
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
                if (template.DockPanel == null)
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
        public static void RestoreLayout(String file)
        {
            try
            {
                Globals.MainForm.RestoringContents = true;
                Session session = SessionManager.GetCurrentSession();
                TextEvent te = new TextEvent(EventType.RestoreLayout, file);
                EventManager.DispatchEvent(Globals.MainForm, te);
                if (!te.Handled)
                {
                    Globals.MainForm.CloseAllDocuments(false);
                    if (!Globals.MainForm.CloseAllCanceled)
                    {
                        session.Type = SessionType.Layout;
                        BuildLayoutSystems(file);
                        SessionManager.RestoreSession("", session);
                    }
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
