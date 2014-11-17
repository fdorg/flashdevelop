using System;
using System.Collections;
using System.Diagnostics;
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
	public class StateSavingTreeView : TreeView
	{
		string topPath;
		string bottomPath;
		ArrayList expandedPaths = new ArrayList();

		public void BeginStatefulUpdate()
		{
			base.BeginUpdate();
			SaveExpandedState();
			SaveScrollState();
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
			expandedPaths.Clear();
			AddExpandedPaths(base.Nodes);
		}

		public void RestoreExpandedState()
		{
			foreach (string path in expandedPaths)
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
			if (node != null) topPath = node.FullPath;
			else topPath = null;
			//
			node = this.BottomNode;
			if (node != null) bottomPath = node.FullPath;
			else bottomPath = null;
		}

		public void RestoreScrollState()
		{
			if (base.Nodes.Count < 1) return;

			TreeNode bottomNode = FindClosestPath(bottomPath);
			TreeNode topNode = FindClosestPath(topPath);

			if (bottomNode != null)
				bottomNode.EnsureVisible();

			if (topNode != null)
				topNode.EnsureVisible();

			// manually scroll all the way to the left
            if (Win32.ShouldUseWin32()) Win32.ScrollToLeft(this);
		}

		private TreeNode FindClosestPath(string path)
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

		#endregion
	}
}
