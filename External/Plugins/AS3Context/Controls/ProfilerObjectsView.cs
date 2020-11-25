using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using PluginCore;
using PluginCore.Localization;

namespace AS3Context.Controls
{
    class ProfilerObjectsView
    {
        readonly Regex reStep = new Regex("([^\\[]+)\\[(.*):([0-9]+)\\]");
        readonly ObjectRefsModel model = new ObjectRefsModel();
        string fileToOpen;
        int lineToOpen;
        readonly Timer delayOpen;

        public ObjectRefsGrid ObjectsGrid { get; }

        public ProfilerObjectsView(ObjectRefsGrid grid)
        {
            ObjectsGrid = grid;

            delayOpen = new Timer();
            delayOpen.Interval = 100;
            delayOpen.Tick += delayOpen_Tick;

            // action
            var openItem = new ToolStripMenuItem(TextHelper.GetString("Label.OpenMethodFile"));
            openItem.Click += objectsGrid_Open;

            ObjectsGrid.ContextMenuStrip = new ContextMenuStrip();
            ObjectsGrid.ContextMenuStrip.Font = PluginBase.Settings.DefaultFont;
            ObjectsGrid.ContextMenuStrip.Renderer = new DockPanelStripRenderer(false);
            ObjectsGrid.ContextMenuStrip.Items.Add(openItem);
            ObjectsGrid.DoubleClick += objectsGrid_Open;
        }

        void delayOpen_Tick(object sender, EventArgs e)
        {
            delayOpen.Stop();
            if (fileToOpen is null) return;
            if (File.Exists(fileToOpen))
            {
                PluginBase.MainForm.OpenEditableDocument(fileToOpen, false);
                if (PluginBase.MainForm.CurrentDocument is {} doc
                    && doc.IsEditable
                    && doc.FileName.Equals(fileToOpen, StringComparison.OrdinalIgnoreCase)
                    && doc.SciControl is {} sci)
                {
                    var pos = sci.PositionFromLine(lineToOpen);
                    sci.SetSel(pos, pos);
                    sci.EnsureVisibleEnforcePolicy(lineToOpen);
                }
            }
            fileToOpen = null;
        }

        void objectsGrid_Open(object sender, EventArgs e)
        {
            if (ObjectsGrid.SelectedNode?.Tag is ObjectRefsNode node && node.Line.Length > 0)
            {
                fileToOpen = node.Path.Replace(';', Path.DirectorySeparatorChar);
                lineToOpen = int.Parse(node.Line) - 1;
                delayOpen.Start();
            }
        }

        public void Clear() => model.Root.Nodes.Clear();

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

            ObjectsGrid.Model = model;
        }
    }
}
