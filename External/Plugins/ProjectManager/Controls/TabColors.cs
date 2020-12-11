using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
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
                foreach (var doc in PluginBase.MainForm.Documents)
                {
                    var tab = (DockContent) doc;
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
                foreach (var doc in PluginBase.MainForm.Documents)
                {
                    if (doc.IsEditable) UpdateTabColor(doc, paths, settings);
                }
            }
        }

        /// <summary>
        /// Updates color of a document tab
        /// </summary>
        static void UpdateTabColor(ITabbedDocument doc, IEnumerable<string> paths, ProjectManagerSettings settings)
        {
            var fileName = doc.FileName;
            var isMatch = paths.Any(path => fileName.StartsWith(path, StringComparison.OrdinalIgnoreCase));
            var tab = (DockContent) doc;
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