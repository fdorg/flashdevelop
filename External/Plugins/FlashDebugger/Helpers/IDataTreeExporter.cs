// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
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
