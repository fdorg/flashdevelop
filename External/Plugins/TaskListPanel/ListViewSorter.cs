using System.Collections;
using System.Windows.Forms;

namespace TaskListPanel
{
    public class ListViewSorter : IComparer
    {
        readonly CaseInsensitiveComparer Comparer;

        public ListViewSorter()
        {
            SortColumn = 0;
            Comparer = new CaseInsensitiveComparer();
            Order = SortOrder.None;
        }

        /// <summary>
        /// This method is inherited from the IComparer interface.  It compares the two objects passed using a case insensitive comparison.
        /// </summary>
        public int Compare(object x, object y)
        {
            int compareResult;
            ListViewItem listviewX = (ListViewItem)x;
            ListViewItem listviewY = (ListViewItem)y;
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