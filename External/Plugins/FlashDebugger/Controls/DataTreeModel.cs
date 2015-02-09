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
            else return FindNode(Root, path);
        }

        private Node FindNode(Node root, string path)
        {
            foreach (Node node in root.Nodes)
            {
                // TODO: We could optimize some more here, we don't need to get the full path
                string nodePath = GetFullPath(node);
                if (path == nodePath) return node;
                if (path.StartsWith(nodePath))
                {
                    if (node.Nodes.Count > 0)
                    {
                        Node tmp = FindNode(node, path);
                        if (tmp != null) return tmp;
                    }
                }
            }
            return null;
        }

    }

}
