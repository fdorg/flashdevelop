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

        ListViewXP listView;
        private Dictionary<string, TypeItem> items;
        private Dictionary<string, bool> finished = new Dictionary<string, bool>();
        private TypeItemComparer comparer;
        private ToolStripMenuItem viewObjectsItem;

        public ProfilerLiveObjectsView(ListViewXP view)
        {
            // config
            listView = view;

            comparer = new TypeItemComparer();
            comparer.SortColumn = TypeItem.COL_COUNT;
            comparer.Sorting = SortOrder.Descending;

            listView.ListViewItemSorter = comparer;
            listView.ColumnClick += new ColumnClickEventHandler(listView_ColumnClick);

            // action
            viewObjectsItem = new ToolStripMenuItem(TextHelper.GetString("Label.ViewObjectsItem"));
            viewObjectsItem.Click += new EventHandler(onViewObjects);

            listView.ContextMenuStrip = new ContextMenuStrip();
            listView.ContextMenuStrip.Font = PluginBase.Settings.DefaultFont;
            listView.ContextMenuStrip.Renderer = new DockPanelStripRenderer(false);
            listView.ContextMenuStrip.Items.Add(viewObjectsItem);

            listView.DoubleClick += new EventHandler(onViewObjects);
        }

        void listView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (comparer.SortColumn == e.Column)
            {
                if (comparer.Sorting == SortOrder.Ascending)
                    comparer.Sorting = SortOrder.Descending;
                else comparer.Sorting = SortOrder.Ascending;
            }
            else
            {
                comparer.SortColumn = e.Column;
                if (e.Column >= 2)
                    comparer.Sorting = SortOrder.Descending;
                else comparer.Sorting = SortOrder.Ascending;
            }
            listView.Sort();
        }

        private void onViewObjects(object sender, EventArgs e)
        {
            if (listView.SelectedItems.Count == 1)
            {
                if (OnViewObject != null)
                    OnViewObject(listView.SelectedItems[0].Tag as TypeItem);
            }
        }

        public void Clear()
        {
            items = new Dictionary<string, TypeItem>();
            listView.Items.Clear();
        }

        /// <summary>
        /// Live objects stats
        /// </summary>
        /// <param name="lines"></param>
        public void UpdateTypeGrid(string[] lines)
        {
            listView.SetExStyles();
            listView.BeginUpdate();
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
                        listView.Items.Add(item.ListItem);
                    }
                    else if (!items.ContainsKey(parts[0])) continue;
                    else item = items[parts[0]];
                    item.Update(parts[1], parts[2]);
                }
                listView.Sort();
            }
            finally
            {
                listView.EndUpdate();
            }
        }
    }


    #region Model

    class TypeItemComparer : IComparer
    {
        public int SortColumn = 0;
        public SortOrder Sorting;

        int IComparer.Compare(object x, object y)
        {
            TypeItem a = (TypeItem)((ListViewItem)x).Tag;
            TypeItem b = (TypeItem)((ListViewItem)y).Tag;

            int comp;
            switch (SortColumn)
            {
                case TypeItem.COL_PKG: comp = a.Package.CompareTo(b.Package); break;
                case TypeItem.COL_MAX: comp = a.Maximum.CompareTo(b.Maximum); break;
                case TypeItem.COL_COUNT: comp = a.Count.CompareTo(b.Count); break;
                case TypeItem.COL_MEM: comp = a.Memory.CompareTo(b.Memory); break;
                default: comp = a.Name.CompareTo(b.Name); break;
            }

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
            ListItem = new ListViewItem(Name);
            ListItem.Tag = this;
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
