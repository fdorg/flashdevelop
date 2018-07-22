using System;
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;
using PluginCore.Helpers;
using PluginCore.Localization;
using PluginCore.Utilities;
using PluginCore;
using CodeRefactor.Commands;
using PluginCore.Controls;

namespace CodeRefactor.Controls
{
    public class SurroundMenu : ToolStripMenuItem
    {
        private List<ICompletionListItem> items;

        public SurroundMenu()
        {
            Text = TextHelper.GetString("Label.SurroundWith");
            items = new List<ICompletionListItem>();
        }

        public void Clear()
        {
            items.Clear();
        }

        /// <summary>
        /// Generates the menu for the selected sci control
        /// </summary>
        public void GenerateSnippets(ScintillaNet.ScintillaControl sci)
        {
            var files = new List<string>();

            string specific = Path.Combine(PathHelper.SnippetDir, sci.ConfigurationLanguage, SurroundWithCommand.SurroundFolder);
            if (Directory.Exists(specific))
            {
                var walker = new PathWalker(specific, "*" + SurroundWithCommand.SurroundExt, false);
                files.AddRange(walker.GetFiles());
            }

            string global = Path.Combine(PathHelper.SnippetDir, SurroundWithCommand.SurroundFolder);
            if (Directory.Exists(global))
            {
                var walker = new PathWalker(global, "*" + SurroundWithCommand.SurroundExt, false);
                files.AddRange(walker.GetFiles());
            }

            items.Clear();
            if (files.Count > 0)
            {
                files.Sort();
                foreach (string file in files)
                {
                    string content = File.ReadAllText(file);
                    if (content.Contains("{0}"))
                    {
                        items.Add(new SurroundWithItem(Path.GetFileNameWithoutExtension(file)));
                    }
                }
            }
        }

        protected override void OnClick(EventArgs e)
        {
            CompletionList.Show(items, false);
        }
    }
}
