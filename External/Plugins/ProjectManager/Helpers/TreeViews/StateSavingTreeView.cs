// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System.Collections;
using System.Collections.Generic;
using PluginCore;

namespace System.Windows.Forms
{
    /// <summary>
    /// Exposes methods to save and restore TreeView state (such as scroll position
    /// and expanded nodes) when rebuilding.
    /// </summary>
    /// <remarks>
    /// I am no longer using this class, since the new ProjectTreeView updates all
    /// nodes in-place for improved scaleability.
    /// </remarks>
    public class StateSavingTreeView : TreeViewEx
    {
        string topPath;
        string bottomPath;
        readonly List<string> expandedPaths = new List<string>();

        public void BeginStatefulUpdate()
        {
            BeginUpdate();
            SaveExpandedState();
            SaveScrollState();
        }

        public void EndStatefulUpdate()
        {
            RestoreExpandedState();
            EndUpdate();
            RestoreScrollState();
        }

        #region Expanded State Saving

        public void SaveExpandedState()
        {
            expandedPaths.Clear();
            AddExpandedPaths(Nodes);
        }

        public void RestoreExpandedState()
        {
            foreach (string path in expandedPaths)
            {
                TreeNode node = FindClosestPath(path);
                node?.Expand();
            }
        }

        void AddExpandedPaths(TreeNodeCollection nodes)
        {
            foreach (TreeNode node in nodes)
            {
                if (node.IsExpanded) expandedPaths.Add(node.FullPath);
                if (node.Nodes.Count > 0) AddExpandedPaths(node.Nodes);
            }
        }

        #endregion

        #region Scroll Position Saving

        public TreeNode BottomNode
        {
            get
            {
                TreeNode bottomNode = null;
                FindBottom(Nodes,ref bottomNode);
                return bottomNode;
            }
        }

        void FindBottom(TreeNodeCollection nodes, ref TreeNode bottomNode)
        {
            foreach (TreeNode node in nodes)
            {
                if (node.IsVisible)
                    bottomNode = node;

                else if (bottomNode != null)
                    return; // this node is the first invisible node after finding visible ones

                if (node.IsExpanded && node.Nodes.Count > 0)
                    FindBottom(node.Nodes,ref bottomNode);
            }
        }

        public void SaveScrollState()
        {
            if (Nodes.Count == 0) return;

            // store what nodes were at the top and bottom so we can try and preserve scroll
            // use the tag instead of node reference because you're most likely rebuilding
            // the tree
            TreeNode node = TopNode;
            topPath = node?.FullPath;
            //
            node = BottomNode;
            bottomPath = node?.FullPath;
        }

        public void RestoreScrollState()
        {
            if (Nodes.Count == 0) return;

            TreeNode bottomNode = FindClosestPath(bottomPath);
            TreeNode topNode = FindClosestPath(topPath);

            bottomNode?.EnsureVisible();

            topNode?.EnsureVisible();

            // manually scroll all the way to the left
            if (Win32.ShouldUseWin32()) Win32.ScrollToLeft(this);
        }

        TreeNode FindClosestPath(string path)
        {
            if (string.IsNullOrEmpty(path)) return null;
            var queue = new Queue<string>(path.Split('\\'));
            return FindClosestPath(Nodes, queue);
        }

        TreeNode FindClosestPath(IEnumerable nodes, Queue<string> queue)
        {
            string nextChunk = queue.Dequeue();
            foreach (TreeNode node in nodes)
            {
                if (node.Text == nextChunk)
                {
                    if (queue.Count > 0 && node.Nodes.Count > 0)
                        return FindClosestPath(node.Nodes,queue);
                    return node; // as close as we'll get
                }
            }
            return null;
        }

        #endregion
    }
}
