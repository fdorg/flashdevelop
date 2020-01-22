// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using PluginCore;
using PluginCore.Localization;

namespace AS3Context.Controls
{
    delegate void ViewObjectEvent(TypeItem item);

    class ProfilerLiveObjectsView
    {
        public event ViewObjectEvent OnViewObject;

        Dictionary<string, TypeItem> items;
        readonly TypeItemComparer comparer;
        readonly ToolStripMenuItem viewObjectsItem;

        public ListView ListView { get; }

        public ProfilerLiveObjectsView(ListView view)
        {
            // config
            ListView = view;

            comparer = new TypeItemComparer();
            comparer.SortColumn = TypeItem.COL_COUNT;
            comparer.Sorting = SortOrder.Descending;

            ListView.ListViewItemSorter = comparer;
            ListView.ColumnClick += listView_ColumnClick;

            // action
            viewObjectsItem = new ToolStripMenuItem(TextHelper.GetString("Label.ViewObjectsItem"));
            viewObjectsItem.Click += onViewObjects;

            ListView.ContextMenuStrip = new ContextMenuStrip();
            ListView.ContextMenuStrip.Font = PluginBase.Settings.DefaultFont;
            ListView.ContextMenuStrip.Renderer = new DockPanelStripRenderer(false);
            ListView.ContextMenuStrip.Items.Add(viewObjectsItem);

            ListView.DoubleClick += onViewObjects;
        }

        void listView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (comparer.SortColumn == e.Column)
            {
                comparer.Sorting = comparer.Sorting == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending;
            }
            else
            {
                comparer.SortColumn = e.Column;
                comparer.Sorting = e.Column >= 2 ? SortOrder.Descending : SortOrder.Ascending;
            }
            ListView.Sort();
        }

        void onViewObjects(object sender, EventArgs e)
        {
            if (ListView.SelectedItems.Count == 1)
            {
                OnViewObject?.Invoke(ListView.SelectedItems[0].Tag as TypeItem);
            }
        }

        public void Clear()
        {
            items = new Dictionary<string, TypeItem>();
            ListView.Items.Clear();
        }

        /// <summary>
        /// Live objects stats
        /// </summary>
        /// <param name="lines"></param>
        public void UpdateTypeGrid(string[] lines)
        {
            ListView.BeginUpdate();
            foreach (TypeItem item in items.Values)
                item.Zero();

            try
            {
                foreach (string line in lines)
                {
                    string[] parts = line.Split('/');
                    TypeItem item;
                    if (parts.Length == 4)
                    {
                        item = new TypeItem(parts[3]);
                        items[parts[0]] = item;
                        ListView.Items.Add(item.ListItem);
                    }
                    else if (!items.ContainsKey(parts[0])) continue;
                    else item = items[parts[0]];
                    item.Update(parts[1], parts[2]);
                }
                ListView.Sort();
            }
            finally
            {
                ListView.EndUpdate();
            }
        }
    }

    #region Model

    class TypeItemComparer : IComparer
    {
        public int SortColumn;
        public SortOrder Sorting;

        int IComparer.Compare(object x, object y)
        {
            var a = (TypeItem)((ListViewItem)x).Tag;
            var b = (TypeItem)((ListViewItem)y).Tag;
            var comp = SortColumn switch
            {
                TypeItem.COL_PKG => a.Package.CompareTo(b.Package),
                TypeItem.COL_MAX => a.Maximum.CompareTo(b.Maximum),
                TypeItem.COL_COUNT => a.Count.CompareTo(b.Count),
                TypeItem.COL_MEM => a.Memory.CompareTo(b.Memory),
                _ => a.Name.CompareTo(b.Name),
            };
            return Sorting == SortOrder.Ascending ? comp : -comp;
        }
    }


    class TypeItem
    {
        public const int COL_PKG = 1;
        public const int COL_MAX = 2;
        public const int COL_COUNT = 3;
        public const int COL_MEM = 4;

        public string QName;
        public string Name;
        public string Package = "";
        public int Count;
        public int Maximum;
        public int Memory;
        public ListViewItem ListItem;
        public bool zero;

        public TypeItem(string fullName)
        {
            QName = fullName;
            int p = fullName.IndexOf(':');
            if (p >= 0)
            {
                Name = fullName.Substring(p + 2);
                Package = fullName.Substring(0, p);
            }
            else Name = fullName;
            ListItem = new ListViewItem(Name) {Tag = this};
            ListItem.SubItems.Add(new ListViewItem.ListViewSubItem(ListItem, Package));
            ListItem.SubItems.Add(new ListViewItem.ListViewSubItem(ListItem, "0"));
            ListItem.SubItems.Add(new ListViewItem.ListViewSubItem(ListItem, "0"));
            ListItem.SubItems.Add(new ListViewItem.ListViewSubItem(ListItem, "0"));
        }

        public void Update(string cpt, string mem)
        {
            zero = false;

            int.TryParse(cpt, out Count);
            ListItem.SubItems[COL_COUNT].Text = Count.ToString("N0");

            if (Maximum < Count)
            {
                Maximum = Count;
                ListItem.SubItems[COL_MAX].Text = Maximum.ToString("N0");
            }

            int.TryParse(mem, out Memory);
            ListItem.SubItems[COL_MEM].Text = Memory.ToString("N0");
        }

        public void Zero()
        {
            if (zero) return;
            zero = true;
            ListItem.SubItems[COL_COUNT].Text = "0";
            ListItem.SubItems[COL_MEM].Text = "0";
        }
    }

    #endregion
}
