// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace WeifenLuo.WinFormsUI.Docking
{
    public class DockPaneCollection : ReadOnlyCollection<DockPane>
    {
        internal DockPaneCollection()
            : base(new List<DockPane>())
        {
        }

        internal int Add(DockPane pane)
        {
            if (Items.Contains(pane))
                return Items.IndexOf(pane);

            Items.Add(pane);
            return Count - 1;
        }

        internal void AddAt(DockPane pane, int index)
        {
            if (index < 0 || index > Items.Count - 1)
                return;
            
            if (Contains(pane))
                return;

            Items.Insert(index, pane);
        }

        internal void Dispose()
        {
            for (int i=Count - 1; i>=0; i--)
                this[i].Close();
        }

        internal void Remove(DockPane pane)
        {
            Items.Remove(pane);
        }
    }
}
