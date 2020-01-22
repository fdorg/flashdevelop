// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
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
        readonly ToolStripItem item;

        public static void AddItemToList(ToolStripItem item, List<ICompletionListItem> list)
        {
            if (item.Enabled)
                list.Add(new RefactorItem(item));
        }

        public RefactorItem(ToolStripItem item)
        {
            this.item = item;
            Label = TextHelper.RemoveMnemonicsAndEllipsis(item.Text);
            Description = TextHelper.GetStringWithoutMnemonics("Label.Refactor");
            Icon = (Bitmap) (item.Image ?? PluginBase.MainForm.FindImage("452")); //452 or 473
        }

        public string Description { get; }

        public Bitmap Icon { get; }

        public string Label { get; }

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
        public SurroundWithItem(string label)
        {
            Label = label;
            Description = TextHelper.GetStringWithoutMnemonics("Label.SurroundWith");
            Icon = (Bitmap) PluginBase.MainForm.FindImage("341");
        }

        public string Label { get; }

        public string Value
        {
            get
            {
                var command = CommandFactoryProvider.GetFactoryForCurrentDocument().CreateSurroundWithCommand(Label);
                command.Execute();
                return null;
            }
        }

        public string Description { get; }

        public Bitmap Icon { get; }
    }
}