using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using PluginCore;
using PluginCore.Managers;

namespace FlashDevelop.Managers
{
    class TabTextManager
    {
        /// <summary>
        /// Updates the documents tab texts
        /// </summary>
        public static void UpdateTabTexts()
        {
            if (Globals.Settings.DisableTabDifferentiation)
            {
                foreach (var doc in Globals.MainForm.Documents)
                {
                    if (doc.IsEditable)
                    {
                        String name = Path.GetFileName(doc.FileName);
                        if (doc.IsModified) doc.Text = name + "*";
                        else doc.Text = name;
                    }
                }
            }
            else DifferentiateTabTexts();
        }

        /// <summary>
        /// Sets the tab text if needed
        /// </summary>
        public static void SetTabText(ITabbedDocument doc, String text)
        {
            if (doc.Text != text) doc.Text = text;
        }

        /// <summary>
        /// Updates the tab texts by differenting them
        /// </summary>
        private static void DifferentiateTabTexts()
        {
            var byName = new Dictionary<String, List<String>>();
            foreach (var doc in Globals.MainForm.Documents)
            {
                if (doc.IsEditable)
                {
                    String fileName = doc.FileName;
                    String name = Path.GetFileName(fileName);
                    if (!byName.ContainsKey(name)) byName[name] = new List<String>();
                    byName[name].Add(doc.FileName);
                }
            }
            foreach (var entry in byName)
            {
                if (entry.Value.Count == 1)
                {
                    var doc = DocumentManager.FindDocument(entry.Value[0]);
                    if (doc.IsModified) SetTabText(doc, entry.Key + "*");
                    else SetTabText(doc, entry.Key);
                }
                else
                {
                    var paths = Discriminate(entry.Value);
                    foreach (var path in paths)
                    {
                        var doc = DocumentManager.FindDocument(path.Tab);
                        if (doc.IsModified) SetTabText(doc, entry.Key + " (" + path.Diff + ")" + "*");
                        else SetTabText(doc, entry.Key + " (" + path.Diff + ")");
                    }
                }
            }
        }

        /// <summary>
        /// Collects a list of path differences
        /// </summary>
        private static List<ExplodePath> Discriminate(List<String> tabs)
        {
            var paths = new List<ExplodePath>();
            ExplodePath.Longer = 0;
            foreach (var tab in tabs)
            {
                paths.Add(new ExplodePath(tab));
            }
            paths.Sort(ExplodePath.LongerFirst);
            bool hadDiff = false;
            bool hadMatch = false;
            for (var i = 0; i < ExplodePath.Longer; i++)
            {
                bool notMatch = false;
                bool hasMatch = false;
                String match = paths[0][i];
                for (var j = 1; j < paths.Count; j++)
                {
                    var path = paths[j];
                    String part = path[i];
                    if (part == null) continue;
                    if (part != match) notMatch = true;
                    else hasMatch = true;
                }
                if (notMatch)
                {
                    foreach (var path in paths)
                    {
                        if (path[i] == null) continue;
                        if (path.Diff.Length > 0)
                        {
                            path.Diff = Path.DirectorySeparatorChar + path.Diff;
                            if (hadMatch) path.Diff = Path.DirectorySeparatorChar + "..." + path.Diff;
                        }
                        path.Diff = path[i] + path.Diff;
                    }
                    if (!hasMatch) break;
                    hadMatch = false;
                    if (!hasMatch) hadDiff = true;
                }
                else if (hadDiff) break;
                else hadMatch = true;
            }
            return paths;
        }
    }

    class ExplodePath
    {
        public String Tab;
        public String Diff;
        public String[] Parts;
        public static Int32 Longer = 0;
        public Int32 Length { get { return Parts.Length; } }

        public ExplodePath(String tab)
        {
            Tab = tab;
            String path = tab;
            String[] parts = Regex.Split(Path.GetDirectoryName(path), "[\\\\/]+");
            Array.Reverse(parts);
            if (parts.Length > Longer) Longer = parts.Length;
            Parts = parts;
            Diff = "";
        }
        public static Int32 LongerFirst(ExplodePath a, ExplodePath b)
        {
            return a.Length - b.Length;
        }
        public String this[Int32 i]
        {
            get { return i < Parts.Length ? Parts[i] : null; }
        }

    }

}
