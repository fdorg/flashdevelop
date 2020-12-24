using System.Drawing;

namespace System.Windows.Forms
{
    /// <summary>
    /// A TreeView that shows tooltips when the node text is not visible
    /// (the TreeView's automatic implementation of this is buggy)
    /// </summary>
    public class ToolTippedTreeView : DragDropTreeView
    {
        const int TVS_NOTOOLTIPS = 0x80;
        ToolTip tip;
        int px;
        int py;

        // disable the automatic tooltips
        protected override CreateParams CreateParams
        {
            get 
            {
                CreateParams p = base.CreateParams;
                p.Style |= TVS_NOTOOLTIPS;
                return p;
            }
        }

        // custom tooltip
        protected override void OnMouseMove(MouseEventArgs e)
        {
            // create tooltip
            tip ??= new ToolTip {ShowAlways = true, AutoPopDelay = 10000};
            // get node under mouse
            var currentNode = GetNodeAt(PointToClient(Cursor.Position));
            var prev = tip.GetToolTip(this);
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
                Graphics g = CreateGraphics();
                SizeF textSize = g.MeasureString(text, Font);
                int w = (int)textSize.Width;
                if (w+offset > Width)
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
