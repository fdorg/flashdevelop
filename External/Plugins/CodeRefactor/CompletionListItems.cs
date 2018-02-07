using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using CodeRefactor.Provider;
using PluginCore;
using PluginCore.Localization;

namespace CodeRefactor
{
    internal sealed class RefactorItem : ICompletionListSpecialItem
    {
        private ToolStripItem item;
        private string label;
        private string description;
        private Bitmap icon;

        public static void AddItemToList(ToolStripItem item, List<ICompletionListItem> list)
        {
            if (item.Enabled)
                list.Add(new RefactorItem(item));
        }

        public RefactorItem(ToolStripItem item)
        {
            this.item = item;
            label = TextHelper.RemoveMnemonicsAndEllipsis(item.Text);
            description = TextHelper.GetStringWithoutMnemonics("Label.Refactor");
            icon = (Bitmap) (item.Image ?? PluginBase.MainForm.FindImage("452")); //452 or 473
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

    internal sealed class SurroundWithItem : ICompletionListSpecialItem
    {
        private string label;
        private string description;
        private Bitmap icon;

        public SurroundWithItem(string label)
        {
            this.label = label;
            description = TextHelper.GetStringWithoutMnemonics("Label.SurroundWith");
            icon = (Bitmap) PluginBase.MainForm.FindImage("341");
        }

        public string Label
        {
            get { return label; }
        }

        public string Value
        {
            get
            {
                var command = CommandFactoryProvider.GetFactoryForCurrentDocument().CreateSurroundWithCommand(label);
                command.Execute();
                return null;
            }
        }

        public string Description
        {
            get { return description; }
        }

        public Bitmap Icon
        {
            get { return icon; }
        }
    }
}
