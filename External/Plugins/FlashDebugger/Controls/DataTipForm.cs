using System;
using System.Drawing;
using System.Windows.Forms;
using Aga.Controls.Tree;
using FlashDebugger.Controls.DataTree;
using flash.tools.debugger;
using PluginCore;

namespace FlashDebugger.Controls
{
    public partial class DataTipForm : Form
    {
        public TreeViewAdv Tree => dataTreeControl.Tree;

        public DataTreeControl DataTree => dataTreeControl;

        public AutoRowHeightLayout RowLayout => rowLayout;

        readonly AutoRowHeightLayout rowLayout;

        public DataTipForm()
        {
            InitializeComponent();
            rowLayout = new AutoRowHeightLayout(Tree, Tree.RowHeight);
            Tree.Cursor = Cursors.Default;
            Tree.ShowLines = false;
            Tree.Expanded += Tree_SizeChanged;
            Tree.Collapsed += Tree_SizeChanged;
            DataTree.ValueChanged += Tree_SizeChanged;
        }

        public void SetVariable(Variable variable, string path)
        {
            SetVariable(variable);
            DataTree.Nodes[0].Tag = path;
        }

        public void SetVariable(Variable variable)
        {
            DataTree.Nodes.Clear();
            DataTree.AddNode(new VariableNode(variable)
                                 {
                                     HideClassId = PluginMain.settingObject.HideClassIds,
                                     HideFullClasspath = PluginMain.settingObject.HideFullClasspaths
                                 });
            DoResize();
        }

        private void Tree_SizeChanged(object sender, EventArgs e)
        {
            DoResize();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.None)
            {
                switch (DoHitTest(e.X, e.Y))
                {
                    case Win32.HitTest.HTTOP:
                    case Win32.HitTest.HTBOTTOM:
                        base.Cursor = Cursors.SizeNS;
                        break;

                    case Win32.HitTest.HTLEFT:
                    case Win32.HitTest.HTRIGHT:
                        base.Cursor = Cursors.SizeWE;
                        break;

                    case Win32.HitTest.HTTOPLEFT:
                    case Win32.HitTest.HTBOTTOMRIGHT:
                        base.Cursor = Cursors.SizeNWSE;
                        break;

                    case Win32.HitTest.HTTOPRIGHT:
                    case Win32.HitTest.HTBOTTOMLEFT:
                        base.Cursor = Cursors.SizeNESW;
                        break;

                    default:
                    case Win32.HitTest.HTCLIENT:
                        base.Cursor = base.DefaultCursor;
                        base.OnMouseMove(e);
                        break;
                }
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            Point screenPoint = PointToScreen(new Point(e.X, e.Y));
            Win32.HitTest ht = DoHitTest(e.X, e.Y);
            if (Win32.ShouldUseWin32())
            {
                Win32.ReleaseCapture();
                Win32.SendMessage(Handle, Win32.WM_NCLBUTTONDOWN, (int)ht, screenPoint.Y << 16 | screenPoint.X);
            }
        }

        private Win32.HitTest DoHitTest(int X, int Y)
        {
            int cornerWidth = Math.Min(Width / 2, SystemInformation.HorizontalScrollBarArrowWidth);
            int cornerHeight = Math.Min(Height / 2, SystemInformation.VerticalScrollBarArrowHeight);
            if (X < cornerWidth)
            {
                if (Y < cornerHeight)
                {
                    // Cursor Top Left
                    return Win32.HitTest.HTTOPLEFT;
                }

                if (Y > (Height - cornerHeight))
                {
                    // Cursor Bottom Left
                    return Win32.HitTest.HTBOTTOMLEFT;
                }
// Cursor Left
                return Win32.HitTest.HTLEFT;
            }

            if (X > (Width - cornerWidth))
            {
                if (Y < cornerHeight)
                {
                    // Cursor Top Right
                    return Win32.HitTest.HTTOPRIGHT;
                }

                if (Y > (Height - cornerHeight))
                {
                    // Cursor Bottom Right
                    return Win32.HitTest.HTBOTTOMRIGHT;
                }
// Cursor Right
                return Win32.HitTest.HTRIGHT;
            }
            if (Y < cornerHeight)
            {
                // Cursor Top
                return Win32.HitTest.HTTOP;
            }
            if (Y > (Height - cornerHeight))
            {
                // Cursor Bottom
                return Win32.HitTest.HTBOTTOM;
            }
            return Win32.HitTest.HTCLIENT;
        }

        private void DoResize()
        {
            using (Graphics g = Tree.CreateGraphics())
            {
                int nameMaxW = TextWidth(g, Tree.Columns[0].Header) + DataTree.Margin.Horizontal;
                int valueMaxW = TextWidth(g, Tree.Columns[1].Header) + DataTree.Margin.Horizontal;
                int height = 0;
                DataTree.Tree.Columns[0].Width = Screen.GetWorkingArea(this).Width;
                foreach (TreeNodeAdv node in DataTree.Tree.Root.Children)
                {
                    CalcHeightWidth(g, node, ref height, ref nameMaxW, ref valueMaxW);
                }
                DataTree.Tree.Columns[0].Width = nameMaxW;
                DataTree.Tree.Columns[1].Width = valueMaxW;
                int width = nameMaxW + valueMaxW + DataTree.Tree.Margin.Horizontal + Padding.Horizontal + SystemInformation.VerticalScrollBarWidth;
                Form parentForm = PluginBase.MainForm as Form;
                Point locationMainForm = parentForm.PointToClient(Location);
                int maxWidth = parentForm.Width - locationMainForm.X - Padding.Horizontal - 2 * SystemInformation.VerticalScrollBarWidth;
                if (width > maxWidth)
                {
                    width = maxWidth;
                }
                Width = width;
                int h = DataTree.Tree.ColumnHeaderHeight + height + Padding.Vertical + SystemInformation.HorizontalScrollBarHeight;
                int maxHeight = parentForm.Height - locationMainForm.Y - Padding.Vertical - 2 * SystemInformation.HorizontalScrollBarHeight;
                if (h > maxHeight)
                {
                    h = maxHeight;
                }
                Height = h;
            }
        }

        private bool CalcHeightWidth(Graphics g, TreeNodeAdv node, ref int height, ref int widthName, ref int widthValue)
        {
            if (node.IsExpanded)
            {
                foreach (TreeNodeAdv child in node.Children)
                {
                    CalcHeightWidth(g, child, ref height, ref widthName, ref widthValue);
                }
            }
            height += RowLayout.GetRowBounds(node.Row).Height;
            int nodeWidth = 0;
            foreach (NodeControlInfo nodeInfo in DataTree.Tree.GetNodeControls(node))
            {
                if (!(nodeInfo.Control is Aga.Controls.Tree.NodeControls.NodeTextBox))
                {
                    nodeWidth += nodeInfo.Bounds.X + nodeInfo.Bounds.Width + DataTree.Tree.Margin.Right;
                    break;
                }
            }
            int nameWidth = nodeWidth + TextWidth(g, node.ToString()) + DataTree.Margin.Horizontal;
            int valueWidth = TextWidth(g, (node.Tag as DataNode).Value) + DataTree.Margin.Horizontal;
            if (widthName < nameWidth)
            {
                widthName = nameWidth;
            }
            if (widthValue < valueWidth)
            {
                widthValue = valueWidth;
            }
            return true;
        }

        private int TextWidth(Graphics g, string str)
        {
            SizeF textSize = g.MeasureString(str, Tree.Font);
            return (int)textSize.Width;
        }

    }

}
