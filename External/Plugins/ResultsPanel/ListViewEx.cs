using PluginCore;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ResultsPanel
{
    internal class ListViewEx : System.Windows.Forms.ListViewEx
    {
        private int upArrowIndex;
        private int downArrowIndex;

        internal ColumnHeader SortedColumn
        {
            get;
            private set;
        }

        internal SortOrder SortOrder
        {
            get;
            private set;
        }

        internal ListViewEx()
        {
            SortedColumn = null;
            SortOrder = SortOrder.None;
        }

        internal void AddArrowImages()
        {
            upArrowIndex = SmallImageList.Images.Count - 2;
            downArrowIndex = upArrowIndex + 1;
        }

        internal void SortGroups(ColumnHeader columnHeader, SortOrder sortOrder, Comparison<ListViewGroup> comparison)
        {
            SetArrow(columnHeader, sortOrder);

            SortedColumn = columnHeader;
            SortOrder = sortOrder;

            if (sortOrder != SortOrder.None)
            {
                var groups = new ListViewGroup[Groups.Count];
                Groups.CopyTo(groups, 0);

                switch (sortOrder)
                {
                    case SortOrder.Ascending:
                        Array.Sort(groups, comparison);
                        break;
                    case SortOrder.Descending:
                        Array.Sort(groups, comparison);
                        Array.Reverse(groups);
                        break;
                }

                Groups.Clear();
                Groups.AddRange(groups);
            }
        }

        internal void SortItems(ColumnHeader columnHeader, SortOrder sortOrder, Comparison<ListViewItem> comparison)
        {
            SetArrow(columnHeader, sortOrder);

            SortedColumn = columnHeader;
            SortOrder = sortOrder;

            if (sortOrder != SortOrder.None)
            {
                var items = new ListViewItem[Items.Count];
                Items.CopyTo(items, 0);

                switch (sortOrder)
                {
                    case SortOrder.Ascending:
                        Array.Sort(items, comparison);
                        break;
                    case SortOrder.Descending:
                        Array.Sort(items, comparison);
                        Array.Reverse(items);
                        break;
                }

                Items.Clear();
                Items.AddRange(items);
            }
        }

        protected override void OnDrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            bool isSorted = SortedColumn != null && e.ColumnIndex == SortedColumn.Index && SortOrder != SortOrder.None;

            Color back = PluginBase.MainForm.GetThemeColor("ColumnHeader.BackColor");
            Color text = PluginBase.MainForm.GetThemeColor("ColumnHeader.TextColor");
            Color border = PluginBase.MainForm.GetThemeColor("ColumnHeader.BorderColor");

            if (UseTheme && back != Color.Empty && border != Color.Empty && text != Color.Empty)
            {
                base.OnDrawColumnHeader(sender, e);
                if (isSorted)
                {
                    Image arrow = null;
                    switch (SortOrder)
                    {
                        case SortOrder.Ascending:
                            arrow = SmallImageList.Images[upArrowIndex];
                            break;
                        case SortOrder.Descending:
                            arrow = SmallImageList.Images[downArrowIndex];
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
                        e.Header.ImageIndex = SortOrder == SortOrder.Ascending ? upArrowIndex : downArrowIndex;
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
                this.SetSortIcon(column.Index, order);
                return;
            }

            if (SortedColumn != null)
            {
                SortedColumn.ImageIndex = -1;
            }

            switch (order)
            {
                case SortOrder.None:
                    column.ImageIndex = -1;
                    break;
                case SortOrder.Ascending:
                    column.ImageIndex = upArrowIndex;
                    break;
                case SortOrder.Descending:
                    column.ImageIndex = downArrowIndex;
                    break;
            }
        }
    }
}
