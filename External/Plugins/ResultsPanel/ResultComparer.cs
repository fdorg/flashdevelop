using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ResultsPanel
{
    class TypeComparer : IComparer<ListViewGroup>
    {
        public int Compare(ListViewGroup x, ListViewGroup y)
        {
            int i0 = Convert.ToInt32(x.Name);
            int i1 = Convert.ToInt32(y.Name);

            if (i0 == i1)
            {
                return 0;
            }
            else if (i0 < i1)
            {
                return -1;
            }

            return 1;
        }
    }

    class FileComparer : IComparer<ListViewGroup>
    {
        public int Compare(ListViewGroup x, ListViewGroup y)
        {
            return x.Header.CompareTo(y.Header);
        }
    }

    class DescriptionComparer : IComparer<ListViewGroup>
    {
        public int Compare(ListViewGroup x, ListViewGroup y)
        {
            return x.Name.CompareTo(y.Name);
        }
    }

    class PathComparer : IComparer<ListViewGroup>
    {
        public int Compare(ListViewGroup x, ListViewGroup y)
        {
            return x.Name.CompareTo(y.Name);
        }
    }
}
