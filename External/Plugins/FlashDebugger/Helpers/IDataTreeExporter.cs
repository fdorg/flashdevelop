using FlashDebugger.Controls;
using FlashDebugger.Controls.DataTree;

namespace FlashDebugger.Helpers
{
    public interface IDataTreeExporter
    {

        int CopyTreeMaxRecursion { get; set; }
        int CopyTreeMaxChars { get; set; }

        string GetTreeAsText(ValueNode dataNode, string levelSep, DataTreeControl control, int levelLimit);

    }
}
