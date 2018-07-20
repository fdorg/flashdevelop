using System;
using System.Collections;
using System.Windows.Forms;

namespace TaskListPanel
{
    public class ListViewSorter : IComparer
    {
        private Int32 ColumnToSort;
        private SortOrder OrderOfSort;
        private CaseInsensitiveComparer Comparer;

        public ListViewSorter()
        {
            this.ColumnToSort = 0;
            this.Comparer = new CaseInsensitiveComparer();
            this.OrderOfSort = SortOrder.None;
        }

        /// <summary>
        /// This method is inherited from the IComparer interface.  It compares the two objects passed using a case insensitive comparison.
        /// </summary>
        public Int32 Compare(Object x, Object y)
        {
            Int32 compareResult;
            ListViewItem listviewX = (ListViewItem)x;
            ListViewItem listviewY = (ListViewItem)y;
            if (this.ColumnToSort == 1)
            {
                Int32 xVal = Int32.Parse(listviewX.SubItems[1].Text);
                Int32 yVal = Int32.Parse(listviewY.SubItems[1].Text);
                compareResult = xVal.CompareTo(yVal);
                if (this.OrderOfSort == SortOrder.Ascending) return compareResult;
                else if (this.OrderOfSort == SortOrder.Descending) return (-compareResult);
                else return 0;
            }
            else
            {
                compareResult = Comparer.Compare(listviewX.SubItems[this.ColumnToSort].Text, listviewY.SubItems[this.ColumnToSort].Text);
                if (this.OrderOfSort == SortOrder.Ascending) return compareResult;
                else if (this.OrderOfSort == SortOrder.Descending) return (-compareResult);
                else return 0;
            }
        }

        /// <summary>
        /// Gets or sets the number of the column to which to apply the sorting operation (Defaults to '0').
        /// </summary>
        public Int32 SortColumn
        {
            set { this.ColumnToSort = value; }
            get { return this.ColumnToSort; }
        }

        /// <summary>
        /// Gets or sets the order of sorting to apply (for example, 'Ascending' or 'Descending').
        /// </summary>
        public SortOrder Order
        {
            set { this.OrderOfSort = value; }
            get { return this.OrderOfSort; }
        }

    }

}
