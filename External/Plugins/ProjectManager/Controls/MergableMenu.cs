using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Forms;

namespace ProjectManager.Controls
{
    /// <summary>
    /// Holds a collection of MergableItems and provides support for merging two
    /// MergableMenu collections in an intelligent way.
    /// </summary>
    public class MergableMenu : Collection<MergableItem>
    {
        public void Add(ToolStripMenuItem item, int group, bool isChecked)
        {
            var mergeItem = new MergableItem();
            mergeItem.Item = item;
            mergeItem.Group = group;
            mergeItem.Checked = isChecked;
            Add(mergeItem);
        }

        // overloads
        public void Add(ToolStripMenuItem item, int group) => Add(item, group,false);

        /// <summary>
        /// Combines the contents of another menu with our menu in a bitwise-AND sort of way
        /// </summary>
        public MergableMenu Combine(MergableMenu menu)
        {
            var result = new MergableMenu();
            foreach (var item in menu)
                if (Matches(item)) result.Add(item);
            return result;
        }

        bool Matches(MergableItem item) => this.Any(it => it.Item == item.Item && it.Checked == item.Checked);

        /// <summary>
        /// Add the contents of this MergableMenu to a CommandBarMenu
        /// </summary>
        /// <param name="items"></param>
        public void Apply(ToolStripItemCollection items)
        {
            int lastGroup = -1;
            foreach (var item in this)
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
        public bool Checked;

        public ToolStripMenuItem Apply()
        {
            Item.Enabled = true;
            Item.Checked = Checked;
            return Item;
        }
    }
}