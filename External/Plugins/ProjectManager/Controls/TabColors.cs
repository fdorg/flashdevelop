// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using PluginCore;
using WeifenLuo.WinFormsUI.Docking;

namespace ProjectManager.Controls
{
    public enum HighlightType
    {
        ExternalFiles,
        ProjectFiles,
        Disabled
    }

    public class TabColors
    {
        /// <summary>
        /// Default tab highlight color
        /// </summary>
        public static Color TabHighlightColor = Color.FromArgb(255, 190, 60);

        /// <summary>
        /// Updates colors of a all document tabs
        /// </summary>
        public static void UpdateTabColors(ProjectManagerSettings settings)
        {
            if (PluginBase.CurrentProject is null)
            {
                foreach (ITabbedDocument doc in PluginBase.MainForm.Documents)
                {
                    DockContent tab = doc as DockContent;
                    if (doc.IsEditable && tab.TabColor != Color.Transparent)
                    {
                        tab.TabColor = Color.Transparent;
                    }
                }
            }
            else if (!PluginBase.MainForm.ClosingEntirely)
            {
                List<string> paths = new List<string>();
                paths.Add(Path.GetDirectoryName(PluginBase.CurrentProject.ProjectPath));
                foreach (string path in PluginBase.CurrentProject.SourcePaths)
                {
                    paths.Add(PluginBase.CurrentProject.GetAbsolutePath(path));
                }
                foreach (ITabbedDocument doc in PluginBase.MainForm.Documents)
                {
                    if (doc.IsEditable) UpdateTabColor(doc, paths, settings);
                }
            }
        }

        /// <summary>
        /// Updates color of a document tab
        /// </summary>
        private static void UpdateTabColor(ITabbedDocument doc, List<string> paths, ProjectManagerSettings settings)
        {
            bool isMatch = false;
            foreach (string path in paths)
            {
                if (doc.FileName.StartsWith(path, StringComparison.OrdinalIgnoreCase))
                {
                    isMatch = true;
                    break;
                }
            }
            DockContent tab = doc as DockContent;
            if (!isMatch && settings.TabHighlightType == HighlightType.ExternalFiles)
            {
                if (tab.TabColor != TabHighlightColor) tab.TabColor = TabHighlightColor;
            }
            else if (isMatch && settings.TabHighlightType == HighlightType.ProjectFiles)
            {
                if (tab.TabColor != TabHighlightColor) tab.TabColor = TabHighlightColor;
            }
            else if (tab.TabColor != Color.Transparent) tab.TabColor = Color.Transparent;
        }

    }

}
