using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using PluginCore;
using PluginCore.Localization;

namespace CodeRefactor
{
    class RefactorItem : ICompletionListItem
    {
        Bitmap icon;
        string description;
        string label;
        ToolStripItem item;

        public static void AddItemToList(ToolStripItem item, List<ICompletionListItem> list)
        {
            if (item.Enabled)
                list.Add(new RefactorItem(item));
        }

        public RefactorItem(ToolStripItem item)
        {
            this.item = item;
            label = Regex.Replace(item.Text, "[&.]", string.Empty);
            description = TextHelper.GetString("Label.Refactor").Replace("&", string.Empty);
            icon = new Bitmap(item.Image ?? PluginBase.MainForm.FindImage("452")); //452 or 473
        }

        public string Description
        {
            get { return description; }
        }

        public Bitmap Icon
        {
            get { return icon; }
        }

        public string Label
        {
            get { return label; }
        }

        public string Value
        {
            get
            {
                item.PerformClick();
                return null;
            }
        }
    }
}
