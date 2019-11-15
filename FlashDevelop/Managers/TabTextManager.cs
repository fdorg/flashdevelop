// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
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
                        string name = Path.GetFileName(doc.FileName);
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
        public static void SetTabText(ITabbedDocument doc, string text)
        {
            if (doc.Text != text) doc.Text = text;
        }

        /// <summary>
        /// Updates the tab texts by differenting them
        /// </summary>
        private static void DifferentiateTabTexts()
        {
            var byName = new Dictionary<string, List<string>>();
            foreach (var doc in Globals.MainForm.Documents)
            {
                if (doc.IsEditable)
                {
                    string fileName = doc.FileName;
                    string name = Path.GetFileName(fileName);
                    if (!byName.ContainsKey(name)) byName[name] = new List<string>();
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
        private static List<ExplodePath> Discriminate(List<string> tabs)
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
                string match = paths[0][i];
                for (var j = 1; j < paths.Count; j++)
                {
                    var path = paths[j];
                    string part = path[i];
                    if (part is null) continue;
                    if (part != match) notMatch = true;
                    else hasMatch = true;
                }
                if (notMatch)
                {
                    foreach (var path in paths)
                    {
                        if (path[i] is null) continue;
                        if (path.Diff.Length > 0)
                        {
                            path.Diff = Path.DirectorySeparatorChar + path.Diff;
                            if (hadMatch) path.Diff = Path.DirectorySeparatorChar + "..." + path.Diff;
                        }
                        path.Diff = path[i] + path.Diff;
                    }
                    if (!hasMatch) break;
                    hadMatch = false;
                    hadDiff = true;
                }
                else if (hadDiff) break;
                else hadMatch = true;
            }
            return paths;
        }
    }

    class ExplodePath
    {
        public string Tab;
        public string Diff;
        public string[] Parts;
        public static int Longer;
        public int Length => Parts.Length;

        public ExplodePath(string tab)
        {
            Tab = tab;
            string path = tab;
            string[] parts = Regex.Split(Path.GetDirectoryName(path), "[\\\\/]+");
            Array.Reverse(parts);
            if (parts.Length > Longer) Longer = parts.Length;
            Parts = parts;
            Diff = "";
        }

        public static int LongerFirst(ExplodePath a, ExplodePath b) => a.Length - b.Length;

        public string this[int i] => i < Parts.Length ? Parts[i] : null;
    }

}
