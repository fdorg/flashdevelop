using System;
using System.Collections;
using System.Diagnostics;
using PluginCore;

namespace System.Windows.Forms
{
    public class TreeState
    {
        public string highlight;
        public string TopPath;
        public string BottomPath;
        public ArrayList ExpandedPaths;
        public TreeState()
        {
            ExpandedPaths = new ArrayList();
        }
    }

	/// <summary>
	/// Exposes methods to save and restore TreeView state (such as scroll position
	/// and expanded nodes) when rebuilding.
	/// </summary>
	public class StateSavingTreeView : TreeView
	{
        public TreeState State = new TreeState();

        public void BeginStatefulUpdate(TreeState withState)
        {
            if (withState != null) State = withState;
            BeginStatefulUpdate();
        }

		public void BeginStatefulUpdate()
		{
			base.BeginUpdate();
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
			base.EndUpdate();
			RestoreScrollState();
		}

		#region Expanded State Saving

		public void SaveExpandedState()
		{
			State.ExpandedPaths.Clear();
			AddExpandedPaths(base.Nodes);
		}

		public void RestoreExpandedState()
		{
			foreach (string path in State.ExpandedPaths)
			{
				TreeNode node = FindClosestPath(path);
				if (node != null)
					node.Expand();
			}
		}

		void AddExpandedPaths(TreeNodeCollection nodes)
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
				TreeNode bottomNode = null;
				FindBottom(base.Nodes,ref bottomNode);
				return bottomNode;
			}
		}

		private void FindBottom(TreeNodeCollection nodes, ref TreeNode bottomNode)
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
			if (base.Nodes.Count < 1) return;

			// store what nodes were at the top and bottom so we can try and preserve scroll
			// use the tag instead of node reference because you're most likely rebuilding
			// the tree
			TreeNode node = base.TopNode;
            if (node != null) State.TopPath = node.FullPath;
            else State.TopPath = null;
			//
			node = this.BottomNode;
            if (node != null) State.BottomPath = node.FullPath;
            else State.BottomPath = null;
		}

		public void RestoreScrollState()
		{
			if (base.Nodes.Count < 1) return;

            TreeNode bottomNode = FindClosestPath(State.BottomPath);
            TreeNode topNode = FindClosestPath(State.TopPath);

            if (bottomNode != null)
                bottomNode.EnsureVisible();

            if (topNode != null)
                topNode.EnsureVisible();

			// manually scroll all the way to the left
			if (Win32.ShouldUseWin32()) Win32.ScrollToLeft(this);
		}

		public TreeNode FindClosestPath(string path)
		{
			if (path == null || path.Length < 1) return null;
			Queue queue = new Queue(path.Split('\\'));
			return FindClosestPath(base.Nodes,queue);
		}

		private TreeNode FindClosestPath(TreeNodeCollection nodes, Queue queue)
		{
			string nextChunk = queue.Dequeue() as string;

			foreach (TreeNode node in nodes)
			{
				if (node.Text == nextChunk)
				{
					if (queue.Count > 0 && node.Nodes.Count > 0)
						return FindClosestPath(node.Nodes,queue);
					else
						return node; // as close as we'll get
				}
			}
			return null;
		}

		/*private void HScroll(System.IntPtr direction)
		{
			//Set  direction to 0 to scroll left 1 char
			//Set  direction to 1 to scroll right 1 char
			//Set  direction to 2 to scroll 1 page left
			//Set  direction to 3 to scroll 1 page right
			System.Windows.Forms.Message hScrollMessage = new Message();

			hScrollMessage.HWnd   = Handle;
			hScrollMessage.Msg    = 0x0114;  // // #define WM_HSCROLL 0x0114
			hScrollMessage.WParam = direction;
			this.DefWndProc( ref hScrollMessage );
		}*/

		#endregion
	}
}
