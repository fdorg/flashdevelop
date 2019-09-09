using System.Collections;
using System.Windows.Forms;

namespace ProjectManager.Controls
{
    /// <summary>
    /// Holds a collection of MergableItems and provides support for merging two
    /// MergableMenu collections in an intelligent way.
    /// </summary>
    public class MergableMenu : CollectionBase
    {
        public void Add(ToolStripMenuItem item, int group, bool isChecked)
        {
            MergableItem mergeItem = new MergableItem();
            mergeItem.Item = item;
            mergeItem.Group = group;
            mergeItem.Checked = isChecked;
            List.Add(mergeItem);
        }

        public void Add(MergableItem item)
        {
            List.Add(item);
        }

        // overloads
        public void Add(ToolStripMenuItem item, int group)
        { Add(item,group,false); }

        /// <summary>
        /// Combines the contents of another menu with our menu in a bitwise-AND sort of way
        /// </summary>
        public MergableMenu Combine(MergableMenu menu)
        {
            MergableMenu newMenu = new MergableMenu();

            foreach (MergableItem item in menu)
                if (Matches(item)) newMenu.Add(item);

            return newMenu;
        }

        private bool Matches(MergableItem item)
        {
            foreach (MergableItem existingItem in List)
            {
                if (existingItem.Item == item.Item &&
                    existingItem.Checked == item.Checked)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Add the contents of this MergableMenu to a CommandBarMenu
        /// </summary>
        /// <param name="menu"></param>
        public void Apply(ToolStripItemCollection items)
        {
            int lastGroup = -1;

            foreach (MergableItem item in List)
            {
                if (item.Group != lastGroup && lastGroup > -1)
                    items.Add(new ToolStripSeparator());

                items.Add(item.Apply());
                lastGroup = item.Group;
            }
        }
    }

    public class MergableItem
    {
        public ToolStripMenuItem Item;
        public int Group;
        public bool Checked = false;

        public ToolStripMenuItem Apply()
        {
            Item.Enabled = true;
            Item.Checked = Checked;
            return Item;
        }
    }
}
