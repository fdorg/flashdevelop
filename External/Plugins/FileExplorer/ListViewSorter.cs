using System;
using System.Collections;
using System.IO;
using System.Windows.Forms;
using PluginCore.Localization;

namespace FileExplorer 
{
    public class ListViewSorter : IComparer
    {
        private Int32 columnToSort;
        private GenericComparer comparer;
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
        public Int32 Compare(Object x, Object y)
        {
            Int32 compareResult = 0;
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
        public Int32 SortColumn
        {
            set { this.columnToSort = value; }
            get { return this.columnToSort; }
        }
        
        /// <summary>
        /// Gets or sets the order of sorting to apply.
        /// </summary>      
        public SortOrder Order
        {
            set { this.orderOfSort = value; }
            get { return this.orderOfSort; }
        }
        
    }
    
    public class GenericComparer
    {
        /// <summary>
        /// Checks if the item is a browser (button).
        /// </summary>
        public Boolean ItemIsBrowser(ListViewItem item)
        {
            return (item.SubItems[0].Text == "[..]");
        }
        
        /// <summary>
        /// Checks if the item is a folder. 
        /// </summary>
        public Boolean ItemIsFolder(ListViewItem item)
        {
            String path = item.Tag.ToString();
            return Directory.Exists(path);
        }
        
        /// <summary>
        /// Compares two supplied ListViewItems. 
        /// </summary>
        public Int32 CompareFiles(ListViewItem x, ListViewItem y)
        {
            String xVal = x.SubItems[0].Text;
            String yVal = y.SubItems[0].Text;
            if (this.ItemIsBrowser(x) || this.ItemIsBrowser(y))
            {
                return 0;
            }
            if (this.ItemIsFolder(x) && this.ItemIsFolder(y)) 
            {
                return String.Compare(xVal, yVal);
            }
            if (this.ItemIsFolder(x) && !this.ItemIsFolder(y)) 
            {
                return -1;
            }
            if (!this.ItemIsFolder(x) && this.ItemIsFolder(y))
            {
                return 1;
            }
            return String.Compare(xVal, yVal);
        }
        
        /// <summary>
        /// Compares two supplied ListViewItems. 
        /// </summary>
        public Int32 CompareSize(ListViewItem x, ListViewItem y)
        {
            String info = TextHelper.GetString("Info.Kilobytes");
            String xVal = x.SubItems[1].Text.Replace(info, "").Trim();
            String yVal = y.SubItems[1].Text.Replace(info, "").Trim();
            if (this.ItemIsBrowser(x) || this.ItemIsBrowser(y))
            {
                return 0;
            }
            if (this.ItemIsFolder(x) && this.ItemIsFolder(y))
            {
                return String.Compare(x.SubItems[0].Text, y.SubItems[0].Text);
            }
            if (this.ItemIsFolder(x) && !this.ItemIsFolder(y)) 
            {
                return -1;
            }
            if (!this.ItemIsFolder(x) && this.ItemIsFolder(y))
            {
                return 1;
            }
            Int32 numX = Int32.Parse(xVal);
            Int32 numY = Int32.Parse(yVal);
            if (numX > numY) return -1;
            else if(numX < numY) return 1;
            else return 0;
        }
        
        /// <summary>
        /// Compares two supplied ListViewItems. 
        /// </summary>
        public Int32 CompareModified(ListViewItem x, ListViewItem y)
        {
            String xVal = x.SubItems[3].Text;
            String yVal = y.SubItems[3].Text;
            if (this.ItemIsBrowser(x) || this.ItemIsBrowser(y))
            {
                return 0;
            }
            if (this.ItemIsFolder(x) && this.ItemIsFolder(y))
            {
                return String.Compare(x.SubItems[0].Text, y.SubItems[0].Text);
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
        public Int32 CompareType(ListViewItem x, ListViewItem y)
        {
            String xVal = x.SubItems[2].Text;
            String yVal = y.SubItems[2].Text;
            if (this.ItemIsBrowser(x) || this.ItemIsBrowser(y))
            {
                return 0;
            }
            if (this.ItemIsFolder(x) && this.ItemIsFolder(y))
            {
                return String.Compare(x.SubItems[0].Text, y.SubItems[0].Text);
            }
            if (this.ItemIsFolder(x) && !this.ItemIsFolder(y)) 
            {
                return -1;
            }
            if (!this.ItemIsFolder(x) && this.ItemIsFolder(y))
            {
                return 1;
            }
            return String.Compare(xVal, yVal);
        }

    }
    
}
