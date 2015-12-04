using System;
using System.Collections.Generic;
using System.IO;
using PluginCore;
using PluginCore.Helpers;
using PluginCore.Managers;
using WeifenLuo.WinFormsUI.Docking;

namespace FlashDevelop.Managers
{
    class LayoutManager
    {
        public static List<DockContent> PluginPanels;
        public static DeserializeDockContent ContentDeserializer;
        public static List<String> RestoredPlugins;
 
        static LayoutManager()
        {
            PluginPanels = new List<DockContent>();
            ContentDeserializer = new DeserializeDockContent(GetContentFromPersistString);
            RestoredPlugins = new List<String>();
        }

        /// <summary>
        /// Builds the look of the layout systems
        /// </summary>
        public static void BuildLayoutSystems(String file)
        {
            try
            {
                FileHelper.EnsureUpdatedFile(file);
                DockPanel dockPanel = Globals.MainForm.DockPanel;
                if (File.Exists(file))
                {
                    CloseDocumentContents();
                    ClosePluginPanelContents();
                    dockPanel.LoadFromXml(file, ContentDeserializer);
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
        public static DockContent GetContentFromPersistString(String persistString)
        {
            for (Int32 i = 0; i < PluginPanels.Count; i++)
            {
                DockContent pluginPanel = PluginPanels[i];
                if (pluginPanel.GetPersistString() == persistString)
                {
                    RestoredPlugins.Add(pluginPanel.GetPersistString());
                    return pluginPanel;
                }
            }
            return null;
        }

        /// <summary>
        /// Shows the document contents for xml restoring
        /// </summary>
        public static void ShowDocumentContents()
        {
            DockPanel dockPanel = Globals.MainForm.DockPanel;
            IDockContent[] documents = dockPanel.GetDocuments();
            foreach (DockContent document in documents)
            {
                document.Show(dockPanel);
            }
        }

        /// <summary>
        /// Closes the document contents for xml restoring
        /// </summary>
        public static void CloseDocumentContents()
        {
            DockPanel dockPanel = Globals.MainForm.DockPanel;
            IDockContent[] documents = dockPanel.GetDocuments();
            foreach (DockContent document in documents)
            {
                document.DockPanel = null;
            }
        }

        /// <summary>
        /// Closes the plugin panel contents for xml restoring
        /// </summary>
        public static void ClosePluginPanelContents()
        {
            foreach (DockContent pluginPanel in PluginPanels)
            {
                if (pluginPanel.DockState != DockState.Document)
                {
                    pluginPanel.DockPanel = null;
                }
            }
        }

        /// <summary>
        /// Restore the plugins that have not been restored yet.
        /// These plugins may have been added later to the plugins dir.
        /// </summary>
        public static void RestoreUnrestoredPlugins()
        {
            DockPanel dockPanel = Globals.MainForm.DockPanel;
            foreach (DockContent pluginPanel in PluginPanels)
            {
                if (!RestoredPlugins.Contains(pluginPanel.GetPersistString()))
                {
                    pluginPanel.Show(dockPanel, DockState.Float);
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
