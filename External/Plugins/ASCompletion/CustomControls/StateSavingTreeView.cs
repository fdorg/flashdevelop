using System.Collections;
using System.Collections.Generic;
using PluginCore;

namespace System.Windows.Forms
{
    public class TreeState
    {
        public string highlight;
        public string TopPath;
        public string BottomPath;
        public List<string> ExpandedPaths;
        public TreeState()
        {
            ExpandedPaths = new List<string>();
        }
    }

    /// <summary>
    /// Exposes methods to save and restore TreeView state (such as scroll position
    /// and expanded nodes) when rebuilding.
    /// </summary>
    public class StateSavingTreeView : TreeViewEx
    {
        public TreeState State = new TreeState();

        public void BeginStatefulUpdate(TreeState withState)
        {
            if (withState != null) State = withState;
            BeginStatefulUpdate();
        }

        public void BeginStatefulUpdate()
        {
            BeginUpdate();
            SaveExpandedState();
            SaveScrollState();
        }

        public void EndStatefulUpdate(TreeState withState)
        {
            if (withState != null) State = withState;
            EndStatefulUpdate();
        }

        public void EndStatefulUpdate()
        {
            RestoreExpandedState();
            EndUpdate();
            RestoreScrollState();
        }

        public new void BeginUpdate()
        {
            Win32.SendMessage(Handle, Win32.WM_SETREDRAW, IntPtr.Zero, IntPtr.Zero);
            base.BeginUpdate();
        }

        public new void EndUpdate()
        {
            base.EndUpdate();
            Win32.SendMessage(Handle, Win32.WM_SETREDRAW, new IntPtr(1), IntPtr.Zero);
        }

        #region Expanded State Saving

        public void SaveExpandedState()
        {
            State.ExpandedPaths.Clear();
            AddExpandedPaths(Nodes);
        }

        public void RestoreExpandedState()
        {
            foreach (string path in State.ExpandedPaths)
            {
                var node = FindClosestPath(path);
                node?.Expand();
            }
        }

        void AddExpandedPaths(IEnumerable nodes)
        {
            foreach (TreeNode node in nodes)
            {
                if (node.IsExpanded) State.ExpandedPaths.Add(node.FullPath);
                if (node.Nodes.Count > 0) AddExpandedPaths(node.Nodes);
            }
        }

        #endregion

        #region Scroll Position Saving

        public TreeNode BottomNode
        {
            get
            {
                TreeNode result = null;
                FindBottom(Nodes, ref result);
                return result;
            }
        }

        static void FindBottom(IEnumerable nodes, ref TreeNode bottomNode)
        {
            foreach (TreeNode node in nodes)
            {
                if (node.IsVisible) bottomNode = node;
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
            State.TopPath = node?.FullPath;
            //
            node = BottomNode;
            State.BottomPath = node?.FullPath;
        }

        public void RestoreScrollState()
        {
            if (Nodes.Count == 0) return;

            var bottomNode = FindClosestPath(State.BottomPath);
            var topNode = FindClosestPath(State.TopPath);

            bottomNode?.EnsureVisible();
            topNode?.EnsureVisible();

            // manually scroll all the way to the left
            if (Win32.ShouldUseWin32()) Win32.ScrollToLeft(this);
        }

        public TreeNode FindClosestPath(string path)
        {
            if (string.IsNullOrEmpty(path)) return null;
            var queue = new Queue<string>(path.Split('\\'));
            return FindClosestPath(Nodes, queue);
        }

        static TreeNode FindClosestPath(IEnumerable nodes, Queue<string> queue)
        {
            var nextChunk = queue.Dequeue();
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