using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;

namespace ASClassWizard.Controls.TreeView
{
    class SimpleDirectoryNode : TreeNode
    {
        public bool dirty;
        public string directoryPath;

        public SimpleDirectoryNode(string directory, string path) 
            : base(Path.GetFileName(directory))
        {
            this.dirty = true;
            this.directoryPath = path;

            if (Directory.GetDirectories(path).Length > 0)
                this.Nodes.Add(new TreeNode(""));
        }
    }
}
