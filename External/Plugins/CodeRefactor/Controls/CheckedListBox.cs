using System.Windows.Forms;
using System.Drawing;
using PluginCore;

namespace CodeRefactor.Controls
{
    class CheckedListBox : CheckedListBoxEx
    {
        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            string label;
            if (e.Index > -1 && e.Index < Items.Count && (label = Items[e.Index].ToString()).StartsWithOrdinal("---"))
            {
                e.Graphics.FillRectangle(new SolidBrush(BackColor), e.Bounds);
                e.Graphics.DrawString(label.Substring(4), Font, new SolidBrush(e.ForeColor), e.Bounds);
            }
            else base.OnDrawItem(e);
        }

    }

}
