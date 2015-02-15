using System.Text;
using Aga.Controls.Tree;

namespace FlashDebugger.Controls
{
    public class DataTreeModel : TreeModel
    {
        static private char[] chTrims = { '.' };

        public string GetFullPath(Node node)
        {
            if (node == Root) return string.Empty;

            var path = new StringBuilder();
            while (node != Root && node != null)
            {
                path.Insert(0, node.Text).Insert(node.Text.Length, ".");
                node = node.Parent;
            }
            if (path.Length > 0) path.Length--;

            return path.ToString();
        }

        public Node FindNode(string path)
        {
            if (string.IsNullOrEmpty(path)) return Root;
            return FindNode(Root, path, new StringBuilder());
        }

        private Node FindNode(Node root, string path, StringBuilder nodePath)
        {
            int initialLength = nodePath.Length;
            foreach (Node node in root.Nodes)
            {
                nodePath.Append(node.Text);
                if (path == nodePath.ToString()) return node;
                nodePath.Append(".");
                if (path.StartsWith(nodePath.ToString()))
                {
                    if (node.Nodes.Count > 0)
                    {
                        Node tmp = FindNode(node, path, nodePath);
                        if (tmp != null) return tmp;
                    }
                }
                else nodePath.Length = initialLength;
            }
            return null;
        }

    }

}
