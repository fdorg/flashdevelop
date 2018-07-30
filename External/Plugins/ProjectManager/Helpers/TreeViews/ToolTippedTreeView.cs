using System.Drawing;

namespace System.Windows.Forms
{
    /// <summary>
    /// A TreeView that shows tooltips when the node text is not visible
    /// (the TreeView's automatic implementation of this is buggy)
    /// </summary>
    public class ToolTippedTreeView : DragDropTreeView
    {
        private const int TVS_NOTOOLTIPS = 0x80;
        private ToolTip tip;
        private int px;
        private int py;

        // disable the automatic tooltips
        protected override System.Windows.Forms.CreateParams CreateParams
        {
            get 
            {
                CreateParams p = base.CreateParams;
                p.Style = p.Style | TVS_NOTOOLTIPS;
                return p;
            }
        }

        // custom tooltip
        protected override void OnMouseMove(MouseEventArgs e)
        {
            // create tooltip
            if (tip == null) 
            {
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
                int offset = 25;
                while (currentNode.Parent != null)
                {
                    offset += 20;
                    currentNode = currentNode.Parent;
                }
                if (e.X < offset-5) 
                {
                    if (prev != "") tip.SetToolTip(this, "");
                    return;
                }
                Graphics g = this.CreateGraphics();
                SizeF textSize = g.MeasureString(text, this.Font);
                int w = (int)textSize.Width;
                if (w+offset > this.Width)
                {
                    px = e.X;
                    py = e.Y;
                    tip.SetToolTip(this, text);
                }
                else if (prev != "") tip.SetToolTip(this, "");
            }
            else if (prev != "") tip.SetToolTip(this, "");
        }
    }
}
