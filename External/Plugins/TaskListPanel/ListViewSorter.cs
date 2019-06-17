using System;
using System.Collections;
using System.Windows.Forms;

namespace TaskListPanel
{
    public class ListViewSorter : IComparer
    {
        private int ColumnToSort;
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
        public int Compare(object x, object y)
        {
            int compareResult;
            ListViewItem listviewX = (ListViewItem)x;
            ListViewItem listviewY = (ListViewItem)y;
            if (this.ColumnToSort == 1)
            {
                int xVal = int.Parse(listviewX.SubItems[1].Text);
                int yVal = int.Parse(listviewY.SubItems[1].Text);
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
        public int SortColumn
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
