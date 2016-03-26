/*
 * Custom Treeview
 * - custom tooltip (that don't appear behind the window!)
 * - flicker free
 * - extends StateSavingTreeView
 */
using PluginCore;
using System.Drawing;

namespace System.Windows.Forms
{
    public class FixedTreeView : StateSavingTreeView
    {
        public delegate void NodeClickedHandler(object sender, TreeNode node);
        public event NodeClickedHandler NodeClicked;

        /*private const int TVS_NOTOOLTIPS = 0x80;
        private ToolTip tip;
        private int px;
        private int py;*/
        private TreeNode clickedNode;

        // disable the automatic tooltips
        /*protected override System.Windows.Forms.CreateParams CreateParams
        {
            get {
                CreateParams p = base.CreateParams;
                p.Style = p.Style | TVS_NOTOOLTIPS;
                return p;
            }
        }*/

        // find clicked node on mousedown
        protected override void OnMouseDown(MouseEventArgs e)
        {
            clickedNode = this.GetNodeAt(e.X, e.Y);
            if (Win32.ShouldUseWin32() && clickedNode != null)
            {
                TreeNode currentNode = clickedNode;
                int offset = 25 - Win32.GetScrollPos(this.Handle, Win32.SB_HORZ);
                while (currentNode.Parent != null)
                {
                    offset += 20;
                    currentNode = currentNode.Parent;
                }
                if (e.X > offset - 5)
                {
                    base.SelectedNode = clickedNode;
                }
                else clickedNode = null;
            }
            base.OnMouseDown(e);
        }

        // on mouseup init delayed NodeClicked event
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (clickedNode != null && NodeClicked != null)
                NodeClicked(this, clickedNode);
        }

        // custom tooltip
        /*protected override void OnMouseMove(MouseEventArgs e)
        {
            // create tooltip
            if (tip == null) {
                tip = new ToolTip();
                tip.ShowAlways = true;
                tip.AutoPopDelay = 10000;
            }
            // get node under mouse
            TreeNode currentNode = this.GetNodeAt(this.PointToClient(Cursor.Position));
            string prev = tip.GetToolTip(this);
            if (currentNode != null)
            {
                string text = currentNode.Text;
                if ((prev == text) && (px == e.X) && (py == e.Y))
                    return;
                
                // text dimensions
                int offset = 25 - Win32.Scrolling.GetScrollPos(this.Handle, Win32.Scrolling.SB_HORZ);
                while (currentNode.Parent != null)
                {
                    offset += 20;
                    currentNode = currentNode.Parent;
                }
                if (e.X < offset-5) {
                    if (prev != "") tip.SetToolTip(this, "");
                    return;
                }
                Graphics g = ((Control)this).CreateGraphics();
                SizeF textSize = g.MeasureString(text, this.Font);
                int w = (int)textSize.Width;
                if (w+offset+10 > this.Width)
                {
                    px = e.X;
                    py = e.Y;
                    tip.SetToolTip(this, text);
                }
                else if (prev != "") tip.SetToolTip(this, "");
            }
            else if (prev != "") tip.SetToolTip(this, "");
        }*/

        public FixedTreeView()
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT && Environment.OSVersion.Version.Major < 6)
            {
                SetStyle(ControlStyles.UserPaint, true);
            }
            else SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            if (Win32.ShouldUseWin32() && DoubleBuffered) Win32.SendMessage(Handle, Win32.TVM_SETEXTENDEDSTYLE, (IntPtr)Win32.TVS_EX_DOUBLEBUFFER, (IntPtr)Win32.TVS_EX_DOUBLEBUFFER);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (Win32.ShouldUseWin32() && GetStyle(ControlStyles.UserPaint))
            {
                Message m = new Message();
                m.HWnd = Handle;
                m.Msg = Win32.WM_PRINTCLIENT;
                m.WParam = e.Graphics.GetHdc();
                m.LParam = (IntPtr)Win32.PRF_CLIENT;
                DefWndProc(ref m);
                e.Graphics.ReleaseHdc(m.WParam);
            }
            base.OnPaint(e);
        }

        protected override void WndProc(ref Message message)
        {
            switch (message.Msg)
            {
                case 0xf: // WM_PAINT
                    OnPaint(new PaintEventArgs(Graphics.FromHwnd(this.Handle), this.Bounds));
                    break;
            }
            base.WndProc(ref message);
        }

    }

}
