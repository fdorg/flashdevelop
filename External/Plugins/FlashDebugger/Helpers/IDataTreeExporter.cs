// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
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