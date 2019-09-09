// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System.IO;
using System.Windows.Forms;

namespace ASClassWizard.Controls.TreeView
{
    class SimpleDirectoryNode : TreeNode
    {
        public bool dirty;
        public string directoryPath;

        public SimpleDirectoryNode(string directory, string path) 
            : base(Path.GetFileName(directory))
        {
            dirty = true;
            directoryPath = path;

            if (Directory.GetDirectories(path).Length > 0)
                Nodes.Add(new TreeNode(""));
        }
    }
}
