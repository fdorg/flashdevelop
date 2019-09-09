using System;
using System.Collections;
using System.IO;
using System.Windows.Forms;
using PluginCore.Localization;

namespace FileExplorer 
{
    public class ListViewSorter : IComparer
    {
        private readonly GenericComparer comparer;

        public ListViewSorter()
        {
            SortColumn = 0;
            comparer = new GenericComparer();
            Order = SortOrder.None;
        }
    
        /// <summary>
        /// Compares the two objects passed using a case insensitive comparison.
        /// </summary>
        public int Compare(object x, object y)
        {
            int compareResult = 0;
            ListViewItem listviewX = (ListViewItem)x;
            ListViewItem listviewY = (ListViewItem)y;
            if (SortColumn == 0)
            {
                compareResult = comparer.CompareFiles(listviewX, listviewY);
            } 
            else if (SortColumn == 1)
            {
                compareResult = comparer.CompareSize(listviewX, listviewY);
            } 
            else if (SortColumn == 2)
            {
                compareResult = comparer.CompareType(listviewX, listviewY);
            }
            else if (SortColumn == 3)
            {
                compareResult = comparer.CompareModified(listviewX, listviewY);
            }
            if (Order == SortOrder.Ascending)
            {
                return compareResult;
            }
            if (Order == SortOrder.Descending)
            {
                return (-compareResult);
            }
            return 0;
        }
    
        /// <summary>
        /// Gets or sets the number of the column.
        /// </summary>
        public int SortColumn { set; get; }

        /// <summary>
        /// Gets or sets the order of sorting to apply.
        /// </summary>      
        public SortOrder Order { set; get; }
    }
    
    public class GenericComparer
    {
        /// <summary>
        /// Checks if the item is a browser (button).
        /// </summary>
        public bool ItemIsBrowser(ListViewItem item) => (item.SubItems[0].Text == "[..]");

        /// <summary>
        /// Checks if the item is a folder. 
        /// </summary>
        public bool ItemIsFolder(ListViewItem item) => Directory.Exists(item.Tag.ToString());

        /// <summary>
        /// Compares two supplied ListViewItems. 
        /// </summary>
        public int CompareFiles(ListViewItem x, ListViewItem y)
        {
            string xVal = x.SubItems[0].Text;
            string yVal = y.SubItems[0].Text;
            if (ItemIsBrowser(x) || ItemIsBrowser(y))
            {
                return 0;
            }
            if (ItemIsFolder(x) && ItemIsFolder(y)) 
            {
                return string.Compare(xVal, yVal);
            }
            if (ItemIsFolder(x) && !ItemIsFolder(y)) 
            {
                return -1;
            }
            if (!ItemIsFolder(x) && ItemIsFolder(y))
            {
                return 1;
            }
            return string.Compare(xVal, yVal);
        }
        
        /// <summary>
        /// Compares two supplied ListViewItems. 
        /// </summary>
        public int CompareSize(ListViewItem x, ListViewItem y)
        {
            string info = TextHelper.GetString("Info.Kilobytes");
            string xVal = x.SubItems[1].Text.Replace(info, "").Trim();
            string yVal = y.SubItems[1].Text.Replace(info, "").Trim();
            if (ItemIsBrowser(x) || ItemIsBrowser(y))
            {
                return 0;
            }
            if (ItemIsFolder(x) && ItemIsFolder(y))
            {
                return string.Compare(x.SubItems[0].Text, y.SubItems[0].Text);
            }
            if (ItemIsFolder(x) && !ItemIsFolder(y)) 
            {
                return -1;
            }
            if (!ItemIsFolder(x) && ItemIsFolder(y))
            {
                return 1;
            }
            int numX = int.Parse(xVal);
            int numY = int.Parse(yVal);
            if (numX > numY) return -1;
            if(numX < numY) return 1;
            return 0;
        }
        
        /// <summary>
        /// Compares two supplied ListViewItems. 
        /// </summary>
        public int CompareModified(ListViewItem x, ListViewItem y)
        {
            string xVal = x.SubItems[3].Text;
            string yVal = y.SubItems[3].Text;
            if (ItemIsBrowser(x) || ItemIsBrowser(y))
            {
                return 0;
            }
            if (ItemIsFolder(x) && ItemIsFolder(y))
            {
                return string.Compare(x.SubItems[0].Text, y.SubItems[0].Text);
            }
            if (ItemIsFolder(x) && !ItemIsFolder(y)) 
            {
                return -1;
            }
            if (!ItemIsFolder(x) && ItemIsFolder(y))
            {
                return 1;
            }
            return DateTime.Compare(DateTime.Parse(xVal), DateTime.Parse(yVal));
        }
        
        /// <summary>
        /// Compares two supplied ListViewItems. 
        /// </summary>
        public int CompareType(ListViewItem x, ListViewItem y)
        {
            string xVal = x.SubItems[2].Text;
            string yVal = y.SubItems[2].Text;
            if (ItemIsBrowser(x) || ItemIsBrowser(y))
            {
                return 0;
            }
            if (ItemIsFolder(x) && ItemIsFolder(y))
            {
                return string.Compare(x.SubItems[0].Text, y.SubItems[0].Text);
            }
            if (ItemIsFolder(x) && !ItemIsFolder(y)) 
            {
                return -1;
            }
            if (!ItemIsFolder(x) && ItemIsFolder(y))
            {
                return 1;
            }
            return string.Compare(xVal, yVal);
        }

    }
    
}
