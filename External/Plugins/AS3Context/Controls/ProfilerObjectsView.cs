using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using PluginCore;
using PluginCore.Localization;
using ScintillaNet;

namespace AS3Context.Controls
{
    class ProfilerObjectsView
    {
        ObjectRefsGrid objectsGrid;
        Regex reStep = new Regex("([^\\[]+)\\[(.*):([0-9]+)\\]");
        ObjectRefsModel model = new ObjectRefsModel();
        ToolStripMenuItem openItem;
        string fileToOpen;
        int lineToOpen;
        Timer delayOpen;

        public ProfilerObjectsView(ObjectRefsGrid grid)
        {
            objectsGrid = grid;

            delayOpen = new Timer();
            delayOpen.Interval = 100;
            delayOpen.Tick += new EventHandler(delayOpen_Tick);

            // action
            openItem = new ToolStripMenuItem(TextHelper.GetString("Label.OpenMethodFile"));
            openItem.Click += new EventHandler(objectsGrid_Open);

            objectsGrid.ContextMenuStrip = new ContextMenuStrip();
            objectsGrid.ContextMenuStrip.Font = PluginBase.Settings.DefaultFont;
            objectsGrid.ContextMenuStrip.Renderer = new DockPanelStripRenderer(false);
            objectsGrid.ContextMenuStrip.Items.Add(openItem);
            objectsGrid.DoubleClick += new EventHandler(objectsGrid_Open);
        }

        void delayOpen_Tick(object sender, EventArgs e)
        {
            delayOpen.Stop();
            if (fileToOpen != null)
            {
                if (File.Exists(fileToOpen))
                {
                    PluginBase.MainForm.OpenEditableDocument(fileToOpen, false);
                    if (PluginBase.MainForm.CurrentDocument.IsEditable
                        && PluginBase.MainForm.CurrentDocument.FileName.Equals(fileToOpen, StringComparison.OrdinalIgnoreCase))
                    {
                        ScintillaControl sci = PluginBase.MainForm.CurrentDocument.SciControl;
                        int pos = sci.PositionFromLine(lineToOpen);
                        sci.SetSel(pos, pos);
                        sci.EnsureVisible(lineToOpen);
                    }
                }
                fileToOpen = null;
            }
        }

        void objectsGrid_Open(object sender, EventArgs e)
        {
            if (objectsGrid.SelectedNode != null)
            {
                ObjectRefsNode node = objectsGrid.SelectedNode.Tag as ObjectRefsNode;
                if (node != null && node.Line.Length > 0)
                {
                    fileToOpen = node.Path.Replace(';', Path.DirectorySeparatorChar);
                    lineToOpen = int.Parse(node.Line) - 1;
                    delayOpen.Start();
                }
            }
        }

        public void Clear()
        {
            model.Root.Nodes.Clear();
        }

        public void Display(string qname, string[] info)
        {
            Clear();

            foreach (string line in info)
            {
                ObjectRefsNode node = new ObjectRefsNode(qname, "","");

                string[] steps = line.Split(',');
                foreach (string step in steps)
                {
                    Match m = reStep.Match(step);
                    if (m.Success)
                    {
                        node.Nodes.Add(new ObjectRefsNode(m.Groups[1].Value, m.Groups[2].Value, m.Groups[3].Value));
                    }
                    else
                    {
                        node.Nodes.Add(new ObjectRefsNode(step, "", ""));
                    }
                }

                model.Root.Nodes.Add(node);
            }

            objectsGrid.Model = model;
        }
    }
}
