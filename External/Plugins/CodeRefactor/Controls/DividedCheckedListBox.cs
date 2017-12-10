using System;
using System.Text;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using PluginCore;

namespace CodeRefactor.Controls
{
    class DividedCheckedListBox : CheckedListBox
    {
        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            string label;
            if (e.Index > -1 && e.Index < Items.Count && (label = Items[e.Index].ToString()).StartsWithOrdinal("---"))
            {
                e.Graphics.FillRectangle(new SolidBrush(BackColor), e.Bounds);
                e.Graphics.DrawString(label.Substring(4), Font, Brushes.Gray, e.Bounds);
            }
            else base.OnDrawItem(e);
        }

    }

}
