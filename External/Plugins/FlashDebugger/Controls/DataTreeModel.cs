using Aga.Controls.Tree;

namespace FlashDebugger.Controls
{
    public class DataTreeModel : TreeModel
    {
        static private char[] chTrims = { '.' };

        public string GetFullPath(Node node)
        {
            if (node == Root) return string.Empty;
            else
            {
                string path = string.Empty;
                while (node != Root && node != null)
                {
                    path = string.Format("{0}.{1}", node.Text, path);
                    node = node.Parent;
                }
                return path.TrimEnd(chTrims);
            }
        }

        public Node FindNode(string path)
        {
            if (path == string.Empty) return Root;
            else return FindNode(Root, path, 0);
        }

        private Node FindNode(Node root, string path, int level)
        {
            foreach (Node node in root.Nodes)
            {
                if (path == GetFullPath(node)) return node;
                else
                {
                    if (node.Nodes.Count > 0)
                    {
                        Node tmp = FindNode(node, path, level + 1);
                        if (tmp != null) return tmp;
                    }
                }
            }
            return null;
        }

    }

}
