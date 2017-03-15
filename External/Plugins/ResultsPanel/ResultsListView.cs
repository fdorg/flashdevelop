using PluginCore;
using PluginCore.Helpers;
using PluginCore.Managers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace ResultsPanel
{
    class ResultsListView : ListViewEx
    {
        private ColumnHeader sortedColumn;
        private SortOrder sortOrder = SortOrder.None;

        private int upArrowIndex;
        private int downArrowIndex;

        public ColumnHeader SortedColumn
        {
            get
            {
                return sortedColumn;
            }
        }

        public SortOrder SortOrder
        {
            get
            {
                return sortOrder;
            }
        }

        public ResultsListView() : base()
        {
        }

        public void AddArrowImages()
        {
            Bitmap upArrow = new Bitmap(16, 16);
            Graphics g = Graphics.FromImage(upArrow);
            DrawArrow(g, new Rectangle(0, 0, 16, 16), SortOrder.Ascending);
            g.Dispose();
            this.SmallImageList.Images.Add(upArrow);

            Bitmap downArrow = new Bitmap(16, 16);
            g = Graphics.FromImage(downArrow);
            DrawArrow(g, new Rectangle(0, 0, 16, 16), SortOrder.Descending);
            g.Dispose();
            this.SmallImageList.Images.Add(downArrow);

            upArrowIndex = this.SmallImageList.Images.Count - 2;
            downArrowIndex = upArrowIndex + 1;
        }

        public void SortGroups(ColumnHeader column, SortOrder order, IComparer<ListViewGroup> comparer)
        {
            sortedColumn = column;
            sortOrder = order;

            ListViewGroup[] groups = new ListViewGroup[this.Groups.Count];
            this.Groups.CopyTo(groups, 0);

            switch (order)
            {
                case SortOrder.None:
                    break;
                case SortOrder.Ascending:
                    Array.Sort(groups, comparer);
                    break;
                case SortOrder.Descending:
                    Array.Sort(groups, comparer);
                    Array.Reverse(groups);
                    break;
            }

            foreach (ListViewGroup gp in groups)
            {
                this.Groups.Remove(gp);
            }

            foreach (ListViewGroup gp in groups)
            {
                this.Groups.Add(gp);
            }
        }

        override protected void OnDrawColumnHeader(Object sender, DrawListViewColumnHeaderEventArgs e)
        {
            bool isSorted = sortedColumn != null && e.ColumnIndex == sortedColumn.Index && sortOrder != SortOrder.None;

            Color back = PluginBase.MainForm.GetThemeColor("ColumnHeader.BackColor");
            Color text = PluginBase.MainForm.GetThemeColor("ColumnHeader.TextColor");
            Color border = PluginBase.MainForm.GetThemeColor("ColumnHeader.BorderColor");
            if (UseTheme && back != Color.Empty && border != Color.Empty && text != Color.Empty)
            {
                base.OnDrawColumnHeader(sender, e);
                if (isSorted)
                {
                    DrawArrow(e.Graphics, new Rectangle(e.Bounds.Location, new Size(e.Bounds.Width, 8)));
                }
            }
            else
            {
                e.DrawDefault = true;
                if (isSorted)
                {
                    e.Header.ImageIndex = sortOrder == SortOrder.Ascending ? upArrowIndex : downArrowIndex;
                }
                else if (e.Header.ImageIndex != -1)
                {
                    e.Header.ImageIndex = -1;
                    e.Header.TextAlign = HorizontalAlignment.Left; //set alignment to remove the previously drawn icon
                }
            }
        }

        private void DrawArrow(IDeviceContext device, Rectangle bounds, SortOrder order = SortOrder.None)
        {
            if (order == SortOrder.None)
            {
                order = sortOrder;
            }
            
            VisualStyleElement arrow = null;
            switch (order)
            {
                case SortOrder.Ascending:
                    arrow = VisualStyleElement.Header.SortArrow.SortedUp;
                    break;
                case SortOrder.Descending:
                    arrow = VisualStyleElement.Header.SortArrow.SortedDown;
                    break;
            }

            var arrowRenderer = new VisualStyleRenderer(arrow);
            arrowRenderer.DrawBackground(device, new Rectangle(bounds.Location, bounds.Size));
        }

        private void DrawColumnText(IDeviceContext device, Rectangle bounds, String text, Font font, Color foreColor)
        {
            Int32 textHeight = TextRenderer.MeasureText("HeightTest", font).Height + 1;
            Rectangle textRect = new Rectangle(bounds.X + 3, bounds.Y + (bounds.Height / 2) - (textHeight / 2), bounds.Width, bounds.Height);
            TextRenderer.DrawText(device, text, font, textRect.Location, foreColor);
        }
    }
}
