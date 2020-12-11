// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Collections;
using System.Windows.Forms;

namespace TaskListPanel
{
    public class ListViewSorter : IComparer
    {
        readonly StringComparer Comparer = StringComparer.OrdinalIgnoreCase;

        public ListViewSorter()
        {
            SortColumn = 0;
            Order = SortOrder.None;
        }

        /// <summary>
        /// This method is inherited from the IComparer interface.  It compares the two objects passed using a case insensitive comparison.
        /// </summary>
        public int Compare(object x, object y)
        {
            int compareResult;
            var listviewX = (ListViewItem)x;
            var listviewY = (ListViewItem)y;
            if (SortColumn == 1)
            {
                int xVal = int.Parse(listviewX.SubItems[1].Text);
                int yVal = int.Parse(listviewY.SubItems[1].Text);
                compareResult = xVal.CompareTo(yVal);
                return Order switch
                {
                    SortOrder.Ascending => compareResult,
                    SortOrder.Descending => -compareResult,
                    _ => 0
                };
            }

            compareResult = Comparer.Compare(listviewX.SubItems[SortColumn].Text, listviewY.SubItems[SortColumn].Text);
            return Order switch
            {
                SortOrder.Ascending => compareResult,
                SortOrder.Descending => -compareResult,
                _ => 0
            };
        }

        /// <summary>
        /// Gets or sets the number of the column to which to apply the sorting operation (Defaults to '0').
        /// </summary>
        public int SortColumn { set; get; }

        /// <summary>
        /// Gets or sets the order of sorting to apply (for example, 'Ascending' or 'Descending').
        /// </summary>
        public SortOrder Order { set; get; }

    }
}