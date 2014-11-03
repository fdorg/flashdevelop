using System;
using System.IO;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;
using PluginCore.Helpers;
using PluginCore.Localization;
using PluginCore.Utilities;
using PluginCore;

namespace CodeRefactor.Controls
{
    public class SurroundMenu : ToolStripMenuItem
    {
        private Image image;
        private List<String> items;

        public SurroundMenu()
        {
            this.Text = TextHelper.GetString("Label.SurroundWith");
            this.image = PluginBase.MainForm.FindImage("559"); // empty
        }
		
        /// <summary>
        /// 
        /// </summary>
        override public bool Enabled
        {
            set
            {
                base.Enabled = value;
                // explicitly en- / disable drop down items, the menu can still open
                foreach (ToolStripDropDownItem dropDownItem in DropDownItems)
				{
                    dropDownItem.Enabled = value;
				}
            }
        }

        /// <summary>
        /// Generates the menu for the selected sci control
        /// </summary>
        public void GenerateSnippets(ScintillaNet.ScintillaControl sci)
        {
            String path;
            String content;
            PathWalker walker;
            List<String> files;
            items = new List<String>();
            String surroundFolder = "surround";
            path = Path.Combine(PathHelper.SnippetDir, surroundFolder);
            if (Directory.Exists(path))
            {
                walker = new PathWalker(PathHelper.SnippetDir + surroundFolder, "*.fds", false);
                files = walker.GetFiles();
                foreach (String file in files)
                {
                    items.Add(file);
                }
            }
            path = Path.Combine(PathHelper.SnippetDir, sci.ConfigurationLanguage);
            path = Path.Combine(path, surroundFolder);
            if (Directory.Exists(path))
            {
                walker = new PathWalker(path, "*.fds", false);
                files = walker.GetFiles();
                foreach (String file in files)
                {
                    items.Add(file);
                }
            }
            if (items.Count > 0) items.Sort();
            this.DropDownItems.Clear();
            foreach (String itm in items)
            {
                content = File.ReadAllText(itm);
                if (content.IndexOf("{0}") > -1)
                {
                    this.DropDownItems.Insert(this.DropDownItems.Count, new ToolStripMenuItem(Path.GetFileNameWithoutExtension(itm), image));
                }
            }
        }

    }

}
