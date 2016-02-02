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
        private Color color = Color.FromArgb(255, 0, 0, 0);

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            String label = Items[e.Index].ToString();
            if (label.StartsWithOrdinal("---"))
            {
                e.Graphics.FillRectangle(new SolidBrush(BackColor), e.Bounds);
                e.Graphics.DrawString(label.Substring(4), Font, Brushes.Gray, e.Bounds);
            }
            else base.OnDrawItem(e);
        }

    }

}
