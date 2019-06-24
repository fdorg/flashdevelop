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
