// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
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
            int i0 = Convert.ToInt32(x.Tag);
            int i1 = Convert.ToInt32(y.Tag);

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
            String f0 = x.Header;
            String f1 = y.Header;

            return f0.CompareTo(f1);
        }
    }

    class DescriptionComparer : IComparer<ListViewGroup>
    {
        public int Compare(ListViewGroup x, ListViewGroup y)
        {
            String d0 = x.Tag.ToString();
            String d1 = y.Tag.ToString();

            return d0.CompareTo(d1);
        }
    }

    class PathComparer : IComparer<ListViewGroup>
    {
        public int Compare(ListViewGroup x, ListViewGroup y)
        {
            String d0 = x.Tag.ToString();
            String d1 = y.Tag.ToString();

            return d0.CompareTo(d1);
        }
    }
}
