// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System.Windows.Forms;
using System.Drawing;
using PluginCore;

namespace CodeRefactor.Controls
{
    internal class CheckedListBox : CheckedListBoxEx
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