// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace WeifenLuo.WinFormsUI.Docking
{
    public class FloatWindowCollection : ReadOnlyCollection<FloatWindow>
    {
        internal FloatWindowCollection()
            : base(new List<FloatWindow>())
        {
        }

        internal int Add(FloatWindow fw)
        {
            if (Items.Contains(fw))
                return Items.IndexOf(fw);

            Items.Add(fw);
            return Count - 1;
        }

        internal void Dispose()
        {
            for (int i=Count - 1; i>=0; i--)
                this[i].Close();
        }

        internal void Remove(FloatWindow fw)
        {
            Items.Remove(fw);
        }

        internal void BringWindowToFront(FloatWindow fw)
        {
            Items.Remove(fw);
            Items.Add(fw);
        }
    }
}
