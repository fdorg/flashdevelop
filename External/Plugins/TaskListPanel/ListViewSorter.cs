// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System.Collections;
using System.Windows.Forms;

namespace TaskListPanel
{
    public class ListViewSorter : IComparer
    {
        private readonly CaseInsensitiveComparer Comparer;

        public ListViewSorter()
        {
            this.SortColumn = 0;
            this.Comparer = new CaseInsensitiveComparer();
            this.Order = SortOrder.None;
        }

        /// <summary>
        /// This method is inherited from the IComparer interface.  It compares the two objects passed using a case insensitive comparison.
        /// </summary>
        public int Compare(object x, object y)
        {
            int compareResult;
            ListViewItem listviewX = (ListViewItem)x;
            ListViewItem listviewY = (ListViewItem)y;
            if (this.SortColumn == 1)
            {
                int xVal = int.Parse(listviewX.SubItems[1].Text);
                int yVal = int.Parse(listviewY.SubItems[1].Text);
                compareResult = xVal.CompareTo(yVal);
                if (this.Order == SortOrder.Ascending) return compareResult;
                if (this.Order == SortOrder.Descending) return (-compareResult);
                return 0;
            }

            compareResult = Comparer.Compare(listviewX.SubItems[this.SortColumn].Text, listviewY.SubItems[this.SortColumn].Text);
            if (this.Order == SortOrder.Ascending) return compareResult;
            if (this.Order == SortOrder.Descending) return (-compareResult);
            return 0;
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