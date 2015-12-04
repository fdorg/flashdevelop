using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using PluginCore;
using PluginCore.Helpers;
using PluginCore.Localization;
using SwfOp;
using SwfOp.Utils;

namespace ProjectManager.Controls.TreeView
{
    public class WorkingNode : GenericNode
    {
        public WorkingNode(string swfPath) : base(Path.Combine(swfPath, "invalid"))
        {
            Text = TextHelper.GetString("Info.Exploring");
            ImageIndex = SelectedImageIndex = Icons.Gear.Index;
            ForeColor = PluginBase.MainForm.GetThemeColor("ProjectTreeView.SubItemColor", Color.Gray);
            NodeFont = new Font(PluginBase.Settings.DefaultFont, FontStyle.Regular);
        }

        public void SetErrorText(string msg)
        {
            Text = msg;
            ForeColor = PluginBase.MainForm.GetThemeColor("ProjectTreeView.ErrorColor", Color.Red);
        }
    }

    public class FakeNode : GenericNode
    {
        public FakeNode(string filePath) : base(filePath) {}
    }

    public class ExportNode : FakeNode
    {
        static public Regex reSafeChars = new Regex("[*\\:" + Regex.Escape(new String(Path.GetInvalidPathChars())) + "]");

        public string Export;
        public string ContainingSwfPath;

        public ExportNode(string filePath, string export)
            : base(filePath + "::" + (export = reSafeChars.Replace(export, "_")))
        {
            ContainingSwfPath = filePath;
            Export = export;
            Text = export;
            ForeColorRequest = PluginBase.MainForm.GetThemeColor("ProjectTreeView.SubItemColor", Color.Gray);
            ImageIndex = SelectedImageIndex = Icons.ImageResource.Index;
        }

        private static string SafeFileName(string export)
        {
            return reSafeChars.Replace(export, "?");
        }
    }

    public class MemberExportNode : ClassExportNode
    {
        public MemberExportNode(string filePath, string export, int image)
            : base(filePath, export)
        {
            ImageIndex = SelectedImageIndex = image;
        }
    }

    public class ClassExportNode : ExportNode
    {
        public ClassExportNode(string filePath, string export)
            : base(filePath, export)
        {
            ImageIndex = SelectedImageIndex = Icons.Class.Index;
        }
    }

    public class FontExportNode : ExportNode
    {
        public FontExportNode(string filePath, string export)
            : base(filePath, export)
        {
            ImageIndex = SelectedImageIndex = Icons.Font.Index;
        }
    }

    public class HeaderInfoNode : FakeNode
    {
        public HeaderInfoNode(string label)
            : base("")
        {
            Text = label;
            ForeColorRequest = PluginBase.MainForm.GetThemeColor("ProjectTreeView.SubItemColor", Color.Gray);
            ImageIndex = SelectedImageIndex = Icons.Info.Index;
        }
        public HeaderInfoNode(string label, object value)
            : base("")
        {
            Text = label + " : " + value;
            ForeColorRequest = PluginBase.MainForm.GetThemeColor("ProjectTreeView.SubItemColor", Color.Gray);
            ImageIndex = SelectedImageIndex = Icons.Info.Index;
        }
    }

    public class SwfHeaderNode : FakeNode
    {
        public SwfHeaderNode(string filePath)
            : base(filePath + ";__header__")
        {
            Text = "Properties";
            ForeColorRequest = PluginBase.MainForm.GetThemeColor("ProjectTreeView.SubItemColor", Color.Gray);
            ImageIndex = SelectedImageIndex = Icons.HiddenFolder.Index;
        }
    }

    public class ClassesNode : FakeNode
    {
        public ClassesNode(string filePath)
            : base(filePath+";__classes__")
        {
            Text = "Classes";
            ForeColorRequest = PluginBase.MainForm.GetThemeColor("ProjectTreeView.SubItemColor", Color.Gray);
            ImageIndex = SelectedImageIndex = Icons.HiddenFolder.Index;
        }
    }

    public class SymbolsNode : FakeNode
    {
        public SymbolsNode(string filePath)
            : base(filePath + ";__symbols__")
        {
            Text = "Symbols";
            ForeColorRequest = PluginBase.MainForm.GetThemeColor("ProjectTreeView.SubItemColor", Color.Gray);
            ImageIndex = SelectedImageIndex = Icons.HiddenFolder.Index;
        }
    }

    public class FontsNode : FakeNode
    {
        public FontsNode(string filePath)
            : base(filePath + ";__fonts__")
        {
            Text = "Fonts";
            ForeColorRequest = PluginBase.MainForm.GetThemeColor("ProjectTreeView.SubItemColor", Color.Gray);
            ImageIndex = SelectedImageIndex = Icons.HiddenFolder.Index;
        }
    }

    public class SwfFileNode : FileNode
    {
        bool explored;
        bool explorable;
        BackgroundWorker runner;
        ContentParser parser;

        public SwfFileNode(string filePath) : base(filePath)
        {
            string ext = Path.GetExtension(filePath).ToLower();
            explorable = FileInspector.IsSwf(filePath, ext) || ext == ".swc" || ext == ".ane";
            if (explorable)
            {
                isRefreshable = true;
                Nodes.Add(new WorkingNode(filePath));
            }
        }

        public bool FileExists { get { return File.Exists(BackingPath); } }

        public override void Refresh(bool recursive)
        {
            base.Refresh (recursive);

            if (explored && !IsExpanded)
            {
                explored = false;
                Nodes.Clear();
            }

            if (!FileExists || !explorable)
            {
                Nodes.Clear(); // non-existent file can't be explored
                return;
            }
            else if (Nodes.Count == 0)
            {
                Nodes.Add(new WorkingNode(BackingPath));
            }

            if (explored) 
                Explore();
        }

        public void RefreshWithFeedback(bool recursive)
        {
            if (explored)
            {
                Nodes.Clear();
                Nodes.Add(new WorkingNode(BackingPath));
                
                Refresh(recursive);
            }
        }

        public override void BeforeExpand()
        {
            if (!explored)
                Explore();
        }

        private void Explore()
        {
            explored = true;

            if (parser != null) 
                return;
            parser = new ContentParser(BackingPath);

            runner = new BackgroundWorker();
            runner.RunWorkerCompleted += new RunWorkerCompletedEventHandler(runner_ProcessEnded);
            runner.DoWork += new DoWorkEventHandler(runner_DoWork);
            runner.RunWorkerAsync(parser);
        }

        private void runner_DoWork(object sender, DoWorkEventArgs e)
        {
            (e.Argument as ContentParser).Run();
        }

        private void runner_ProcessEnded(object sender, RunWorkerCompletedEventArgs e)
        {
            // marshal to GUI thread
            TreeView.Invoke(new MethodInvoker(AddExports));
        }

        private void AddExports()
        {
            // remove WorkingNode
            TreeView.BeginUpdate();
            try
            {
                if (parser == null)
                    return;
                if (parser.Errors.Count > 0)
                {
                    WorkingNode wnode = Nodes[0] as WorkingNode;
                    if (wnode == null)
                    {
                        Nodes.Clear();
                        wnode = new WorkingNode(BackingPath);
                        Nodes.Add(wnode);
                    }
                    wnode.SetErrorText(parser.Errors[0]);
                    return;
                }

                Nodes.Clear();
                ExportComparer classesComp = new ExportComparer(parser.Frames);
                if (parser.Classes.Count == 1) classesComp.Compare(parser.Classes[0], parser.Classes[0]);
                parser.Classes.Sort(classesComp);
                ExportComparer symbolsComp = new ExportComparer(parser.Frames);
                if (parser.Symbols.Count == 1) symbolsComp.Compare(parser.Symbols[0], parser.Symbols[0]);
                parser.Symbols.Sort(symbolsComp);
                ExportComparer fontsComp = new ExportComparer(parser.Frames);
                if (parser.Fonts.Count == 1) fontsComp.Compare(parser.Fonts[0], parser.Fonts[0]);
                parser.Fonts.Sort(fontsComp);

                SwfHeaderNode hnode = new SwfHeaderNode(BackingPath);
                string ext = Path.GetExtension(BackingPath).ToLower();
                hnode.Nodes.Add(new HeaderInfoNode("File Size", FormatBytes(new FileInfo(BackingPath).Length)));
                hnode.Nodes.Add(new HeaderInfoNode("SWF Version", parser.Header.Version));
                hnode.Nodes.Add(new HeaderInfoNode("AVM", parser.FileAttributes.Actionscript3 ? 2 : 1));
                if (parser.FileAttributes.UseNetwork) hnode.Nodes.Add(new HeaderInfoNode("Use Network"));
                if (parser.FileAttributes.UseDirectBlit) hnode.Nodes.Add(new HeaderInfoNode("Use DirectBlit"));
                if (parser.FileAttributes.UseGPU) hnode.Nodes.Add(new HeaderInfoNode("Use GPU"));
                if (ext == ".swf")
                {
                    hnode.Nodes.Add(new HeaderInfoNode("Dimensions", FormatDimensions(GetSwfRect(parser.Header.Rect))));
                    hnode.Nodes.Add(new HeaderInfoNode("Background", parser.FileAttributes.Background));
                }
                hnode.Nodes.Add(new HeaderInfoNode("Framerate", parser.Header.Fps / 256));
                hnode.Nodes.Add(new HeaderInfoNode("Frames", parser.Header.Frames));
                Nodes.Add(hnode);

                if (parser.Classes.Count > 0)
                {
                    ClassesNode node = new ClassesNode(BackingPath);
                    node.Text += " (" + FormatBytes(parser.AbcSize) + ")";
                    int[] groups = new int[classesComp.groups.Keys.Count];
                    classesComp.groups.Keys.CopyTo(groups, 0);
                    Array.Sort(groups);
                    foreach (int index in groups)
                    {
                        DeclEntry group = parser.Frames[index];
                        string groupName = group.Name;
                        SwfFrameNode frame = new SwfFrameNode(BackingPath, groupName);
                        frame.Text = groupName + " (" + FormatBytes(group.AbcSize) + ")";
                        if (parser.Frames.Count > 1) node.Nodes.Add(frame);

                        List<String> names = classesComp.groups[index];
                        names.Sort(); // TODO Add setting?
                        foreach (string cls in names)
                        {
                            string name = cls.Replace(':', '.');
                            if (cls.EndsWith("()"))
                                node.Nodes.Add(new MemberExportNode(BackingPath, name.Replace("()", ""), Icons.Method.Index));
                            else if (cls.EndsWith("$"))
                                node.Nodes.Add(new MemberExportNode(BackingPath, name.Replace("$", ""), Icons.Variable.Index));
                            else if (cls.EndsWith("#"))
                                node.Nodes.Add(new MemberExportNode(BackingPath, name.Replace("#", ""), Icons.Const.Index));
                            else
                                node.Nodes.Add(new ClassExportNode(BackingPath, name));
                        }
                    }
                    Nodes.Add(node);
                }

                if (parser.Symbols.Count > 0)
                {
                    SymbolsNode node2 = new SymbolsNode(BackingPath);
                    node2.Text += " (" + FormatBytes(parser.TotalSize - parser.AbcSize - parser.FontsSize) + ")";

                    int[] groups = new int[symbolsComp.groups.Keys.Count];
                    symbolsComp.groups.Keys.CopyTo(groups, 0);
                    Array.Sort(groups);
                    foreach(int index in groups)
                    {
                        DeclEntry group = parser.Frames[index];
                        string groupName = group.Name;
                        SwfFrameNode frame = new SwfFrameNode(BackingPath, groupName);
                        frame.Text = groupName + " (" + FormatBytes(group.DataSize) + ")";
                        if (parser.Frames.Count > 1) node2.Nodes.Add(frame);

                        List<String> names = symbolsComp.groups[index];
                        names.Sort(); // TODO Add setting?
                        foreach (string symbol in names)
                            node2.Nodes.Add(new ExportNode(BackingPath, symbol));
                    }
                    Nodes.Add(node2);
                }

                if (parser.Fonts.Count > 0)
                {
                    FontsNode node2 = new FontsNode(BackingPath);
                    node2.Text += " (" + FormatBytes(parser.FontsSize) + ")";
                    int[] groups = new int[fontsComp.groups.Keys.Count];
                    fontsComp.groups.Keys.CopyTo(groups, 0);
                    Array.Sort(groups);
                    foreach (int index in groups)
                    {
                        DeclEntry group = parser.Frames[index];
                        string groupName = group.Name;
                        SwfFrameNode frame = new SwfFrameNode(BackingPath, groupName);
                        frame.Text = groupName + " (" + FormatBytes(group.FontSize) + ")";
                        if (parser.Frames.Count > 1) node2.Nodes.Add(frame);

                        List<String> names = fontsComp.groups[index];
                        names.Sort(); // TODO Add setting?
                        foreach (string font in names)
                            node2.Nodes.Add(new FontExportNode(BackingPath, font));
                    }
                    Nodes.Add(node2);
                }
            }
            finally
            {
                // free parsed model
                parser = null;
                TreeView.EndUpdate();
            }
        }

        private string FormatDimensions(Rectangle rect)
        {
            return rect.Width + "x" + rect.Height;
        }

        private Rectangle GetSwfRect(byte[] bytes)
        {
            BitArray ba = BitParser.GetBitValues(bytes);
            int Nbits = (int)BitParser.ReadUInt32(ba, 5);
            int index = 5;
            int xmin = (int)BitParser.ReadUInt32(ba, index, Nbits) / 20;
            index += Nbits;
            int xmax = (int)BitParser.ReadUInt32(ba, index, Nbits) / 20;
            index += Nbits;
            int ymin = (int)BitParser.ReadUInt32(ba, index, Nbits) / 20;
            index += Nbits;
            int ymax = (int)BitParser.ReadUInt32(ba, index, Nbits) / 20;
            return new Rectangle(xmin, ymin, xmax, ymax);
        }

        public string FormatBytes(long bytes)
        {
            const int scale = 1024;
            string[] orders = new string[] { "Gb", "Mb", "Kb", "b" };
            long max = (long)Math.Pow(scale, orders.Length - 1);

            foreach (string order in orders)
            {
                if (bytes > max)
                    return string.Format("{0:##.##} {1}", decimal.Divide(bytes, max), order);

                max /= scale;
            }
            return "0 b";
        }

        class ExportComparer : IComparer<DeclEntry>
        {
            public Dictionary<int, List<string>> groups = new Dictionary<int, List<string>>();
            private List<DeclEntry> frames;

            public ExportComparer(List<DeclEntry> swfFrames)
            {
                frames = swfFrames;
            }

            public int Compare(DeclEntry a, DeclEntry b)
            {
                string na = a.Name;
                string nb = b.Name;
                if (!groups.ContainsKey(a.Frame)) groups[a.Frame] = new List<string>();
                if (!groups[a.Frame].Contains(na)) groups[a.Frame].Add(na);
                if (!groups.ContainsKey(b.Frame)) groups[b.Frame] = new List<string>();
                if (!groups[b.Frame].Contains(nb)) groups[b.Frame].Add(nb);
                if (a.Frame != b.Frame) return a.Frame - b.Frame;
                return string.Compare(na, nb);
            }

        }
    }

    public class InputSwfNode : SwfFileNode
    {
        public InputSwfNode(string filePath) : base(filePath) {}

        public override void Refresh(bool recursive)
        {
            base.Refresh (recursive);
            ForeColorRequest = PluginBase.MainForm.GetThemeColor("ProjectTreeView.InputColor", SystemColors.Highlight);
        }
    }

    public class SwfFrameNode : GenericNode
    {
        public SwfFrameNode(string filePath, string name)
            : base(filePath + ";" + name)
        {
            ImageIndex = SelectedImageIndex = Icons.DownArrow.Index;
        }
    }
}
