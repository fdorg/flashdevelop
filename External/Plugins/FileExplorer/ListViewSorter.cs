using System;
using System.Collections;
using System.IO;
using System.Windows.Forms;
using PluginCore.Localization;

namespace FileExplorer 
{
    public class ListViewSorter : IComparer
    {
        readonly GenericComparer comparer;

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
            var itemX = (ListViewItem)x;
            var itemY = (ListViewItem)y;
            var compareResult = SortColumn switch
            {
                0 => comparer.CompareFiles(itemX, itemY),
                1 => comparer.CompareSize(itemX, itemY),
                2 => comparer.CompareType(itemX, itemY),
                3 => comparer.CompareModified(itemX, itemY),
                _ => 0
            };
            return Order switch
            {
                SortOrder.Ascending => compareResult,
                SortOrder.Descending => -compareResult,
                _ => 0
            };
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
            if (ItemIsBrowser(x) || ItemIsBrowser(y))
            {
                return 0;
            }
            var xVal = x.SubItems[0].Text;
            var yVal = y.SubItems[0].Text;
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
            var info = TextHelper.GetString("Info.Kilobytes");
            var xVal = x.SubItems[1].Text.Replace(info, "").Trim();
            var yVal = y.SubItems[1].Text.Replace(info, "").Trim();
            var numX = int.Parse(xVal);
            var numY = int.Parse(yVal);
            return numY.CompareTo(numX);
        }
        
        /// <summary>
        /// Compares two supplied ListViewItems. 
        /// </summary>
        public int CompareModified(ListViewItem x, ListViewItem y)
        {
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
            return DateTime.Compare(DateTime.Parse(x.SubItems[3].Text), DateTime.Parse(y.SubItems[3].Text));
        }
        
        /// <summary>
        /// Compares two supplied ListViewItems. 
        /// </summary>
        public int CompareType(ListViewItem x, ListViewItem y)
        {
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
            return string.Compare(x.SubItems[2].Text, y.SubItems[2].Text);
        }
    }
}