using PluginCore;
using PluginCore.Helpers;
using PluginCore.Managers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
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
            Image upArrow = PluginBase.MainForm.GetAutoAdjustedImage(PluginBase.MainForm.FindImage16("495"));
            Image downArrow = PluginBase.MainForm.GetAutoAdjustedImage(PluginBase.MainForm.FindImage16("493"));

            this.SmallImageList.Images.Add(upArrow);
            this.SmallImageList.Images.Add(downArrow);

            upArrowIndex = this.SmallImageList.Images.Count - 2;
            downArrowIndex = upArrowIndex + 1;
        }

        public void SortGroups(ColumnHeader column, SortOrder order, IComparer<ListViewGroup> comparer)
        {
            sortedColumn = column;
            sortOrder = order;

            SetArrow(column, order);

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
                    Image arrow = null;
                    switch (sortOrder)
                    {
                        case SortOrder.Ascending:
                            arrow = this.SmallImageList.Images[upArrowIndex];
                            break;
                        case SortOrder.Descending:
                            arrow = this.SmallImageList.Images[downArrowIndex];
                            break;
                    }

                    int x = e.Bounds.Location.X + (e.Bounds.Width - 16) / 2;
                    e.Graphics.DrawImage(arrow, x, e.Bounds.Y - 5);
                }
            }
            else
            {
                e.DrawDefault = true;
                //use imageindex if win32 is not available
                if (!Win32.ShouldUseWin32())
                {
                    if (isSorted)
                    {
                        e.Header.ImageIndex = sortOrder == SortOrder.Ascending ? upArrowIndex : downArrowIndex;
                        e.Header.TextAlign = HorizontalAlignment.Left; //set alignment to remove the previously drawn icon
                    }
                    else if (e.Header.ImageIndex != -1)
                    {
                        e.Header.ImageIndex = -1;
                        e.Header.TextAlign = HorizontalAlignment.Left; //set alignment to remove the previously drawn icon
                    }
                    //this.Refresh();
                }

            }
        }

        private void SetArrow(ColumnHeader column, SortOrder order)
        {
            if (Win32.ShouldUseWin32())
            {
                ArrowHelper.SetSortIcon(this, column.Index, order);
                return;
            }

            switch (order)
            {
                case SortOrder.Ascending:
                    column.ImageIndex = upArrowIndex;
                    break;
                case SortOrder.Descending:
                    column.ImageIndex = downArrowIndex;
                    break;
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
            int textHeight = TextRenderer.MeasureText("HeightTest", font).Height + 1;
            Rectangle textRect = new Rectangle(bounds.X + 3, bounds.Y + (bounds.Height / 2) - (textHeight / 2), bounds.Width, bounds.Height);
            TextRenderer.DrawText(device, text, font, textRect.Location, foreColor);
        }
    }
}
