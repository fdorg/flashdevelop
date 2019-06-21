using System;
using System.Collections;
using System.IO;
using System.Windows.Forms;
using PluginCore.Localization;

namespace FileExplorer 
{
    public class ListViewSorter : IComparer
    {
        private int columnToSort;
        private readonly GenericComparer comparer;
        private SortOrder orderOfSort;

        public ListViewSorter()
        {
            this.columnToSort = 0;
            this.comparer = new GenericComparer();
            this.orderOfSort = SortOrder.None;
        }
    
        /// <summary>
        /// Compares the two objects passed using a case insensitive comparison.
        /// </summary>
        public int Compare(object x, object y)
        {
            int compareResult = 0;
            ListViewItem listviewX = (ListViewItem)x;
            ListViewItem listviewY = (ListViewItem)y;
            if (this.columnToSort == 0)
            {
                compareResult = comparer.CompareFiles(listviewX, listviewY);
            } 
            else if (this.columnToSort == 1)
            {
                compareResult = comparer.CompareSize(listviewX, listviewY);
            } 
            else if (this.columnToSort == 2)
            {
                compareResult = comparer.CompareType(listviewX, listviewY);
            }
            else if (this.columnToSort == 3)
            {
                compareResult = comparer.CompareModified(listviewX, listviewY);
            }
            if (this.orderOfSort == SortOrder.Ascending)
            {
                return compareResult;
            }
            else if (this.orderOfSort == SortOrder.Descending)
            {
                return (-compareResult);
            }
            else return 0;
        }
    
        /// <summary>
        /// Gets or sets the number of the column.
        /// </summary>
        public int SortColumn
        {
            set => this.columnToSort = value;
            get => this.columnToSort;
        }
        
        /// <summary>
        /// Gets or sets the order of sorting to apply.
        /// </summary>      
        public SortOrder Order
        {
            set => this.orderOfSort = value;
            get => this.orderOfSort;
        }
        
    }
    
    public class GenericComparer
    {
        /// <summary>
        /// Checks if the item is a browser (button).
        /// </summary>
        public bool ItemIsBrowser(ListViewItem item)
        {
            return (item.SubItems[0].Text == "[..]");
        }
        
        /// <summary>
        /// Checks if the item is a folder. 
        /// </summary>
        public bool ItemIsFolder(ListViewItem item)
        {
            string path = item.Tag.ToString();
            return Directory.Exists(path);
        }
        
        /// <summary>
        /// Compares two supplied ListViewItems. 
        /// </summary>
        public int CompareFiles(ListViewItem x, ListViewItem y)
        {
            string xVal = x.SubItems[0].Text;
            string yVal = y.SubItems[0].Text;
            if (this.ItemIsBrowser(x) || this.ItemIsBrowser(y))
            {
                return 0;
            }
            if (this.ItemIsFolder(x) && this.ItemIsFolder(y)) 
            {
                return string.Compare(xVal, yVal);
            }
            if (this.ItemIsFolder(x) && !this.ItemIsFolder(y)) 
            {
                return -1;
            }
            if (!this.ItemIsFolder(x) && this.ItemIsFolder(y))
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
            if (this.ItemIsBrowser(x) || this.ItemIsBrowser(y))
            {
                return 0;
            }
            if (this.ItemIsFolder(x) && this.ItemIsFolder(y))
            {
                return string.Compare(x.SubItems[0].Text, y.SubItems[0].Text);
            }
            if (this.ItemIsFolder(x) && !this.ItemIsFolder(y)) 
            {
                return -1;
            }
            if (!this.ItemIsFolder(x) && this.ItemIsFolder(y))
            {
                return 1;
            }
            int numX = int.Parse(xVal);
            int numY = int.Parse(yVal);
            if (numX > numY) return -1;
            else if(numX < numY) return 1;
            else return 0;
        }
        
        /// <summary>
        /// Compares two supplied ListViewItems. 
        /// </summary>
        public int CompareModified(ListViewItem x, ListViewItem y)
        {
            string xVal = x.SubItems[3].Text;
            string yVal = y.SubItems[3].Text;
            if (this.ItemIsBrowser(x) || this.ItemIsBrowser(y))
            {
                return 0;
            }
            if (this.ItemIsFolder(x) && this.ItemIsFolder(y))
            {
                return string.Compare(x.SubItems[0].Text, y.SubItems[0].Text);
            }
            if (this.ItemIsFolder(x) && !this.ItemIsFolder(y)) 
            {
                return -1;
            }
            if (!this.ItemIsFolder(x) && this.ItemIsFolder(y))
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
            if (this.ItemIsBrowser(x) || this.ItemIsBrowser(y))
            {
                return 0;
            }
            if (this.ItemIsFolder(x) && this.ItemIsFolder(y))
            {
                return string.Compare(x.SubItems[0].Text, y.SubItems[0].Text);
            }
            if (this.ItemIsFolder(x) && !this.ItemIsFolder(y)) 
            {
                return -1;
            }
            if (!this.ItemIsFolder(x) && this.ItemIsFolder(y))
            {
                return 1;
            }
            return string.Compare(xVal, yVal);
        }

    }
    
}
