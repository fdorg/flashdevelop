using System;
using System.Drawing;
using System.Collections;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace System.Windows.Forms
{
	/// <summary>
	/// Adds simple file-based drag+drop support to the MultiSelectTreeView.
	/// </summary>
	public class DragDropTreeView : MultiSelectTreeView
	{
		Color originalColor;
		Color originalText;
		TreeNode highlightedNode;		

		public DragDropTreeView()
		{
			base.AllowDrop = true;
		}

		protected override void OnItemDrag(ItemDragEventArgs e)
		{
			// can't drag the root node
            if (!base.SelectedNodes.Contains(base.Nodes[0]))
                DoDragDrop(BeginDragNodes(base.SelectedNodes), DragDropEffects.All);
            else
                base.OnItemDrag(e);
		}

        /// <summary>
        /// Constructs draggable DataObjects from a list of dragged nodes.
        /// </summary>
        protected virtual DataObject BeginDragNodes(ArrayList nodes)
        {
            DataObject data = new DataObject();
            data.SetData(nodes);
            return data;
        }

		protected override void OnDragEnter(DragEventArgs e)
		{
			if (IsOurDrag(e.Data))
				e.Effect = (e.KeyState == 9) ? DragDropEffects.Copy : DragDropEffects.Move;
			else if (IsFileDrop(e.Data))
				e.Effect = DragDropEffects.Copy;
			else
				base.OnDragEnter(e);
		}

		protected override void OnDragOver(DragEventArgs e)
		{
			base.OnDragOver(e);

			if (IsOurDrag(e.Data))
				e.Effect = (e.KeyState == 9) ? DragDropEffects.Copy : DragDropEffects.Move;
			else if (IsFileDrop(e.Data))
				e.Effect = DragDropEffects.Copy;
			else
				return;

			TreeNode node = base.GetNodeAt(PointToClient(new Point(e.X,e.Y)));
			node = ChangeDropTarget(node);

			if (node != null)
			{
				if (!base.SelectedNodes.Contains(node))
					HighlightTarget(node);
			}
			else
			{
				e.Effect = DragDropEffects.None;
			}

			DragScroll(e);
		}

		protected override void OnDragDrop(DragEventArgs e)
		{
			if (IsOurDrag(e.Data))
			{
                ArrayList draggedNodes = (ArrayList)e.Data.GetData(typeof(ArrayList));
				TreeNode targetNode = base.GetNodeAt(PointToClient(new Point(e.X,e.Y)));
				targetNode = ChangeDropTarget(targetNode);

				if (draggedNodes != null && targetNode != null)
					DragNodes(draggedNodes,targetNode,e.Effect);
			}
			else if (IsFileDrop(e.Data))
			{
				TreeNode targetNode = base.GetNodeAt(PointToClient(new Point(e.X,e.Y)));
				targetNode = ChangeDropTarget(targetNode);

				if (targetNode == null) return;
				
				// the data is in the form of an array of paths
				Array aFiledrop = (Array)e.Data.GetData(DataFormats.FileDrop);

				// make a string array
				string[] paths = new string[aFiledrop.Length];
				for (int i=0; i<paths.Length; i++)
					paths[i] = aFiledrop.GetValue(i) as string;

				// queue the copy/move operation so we don't hang this thread and block the calling app
				BeginInvoke(new OnFileDropHandler(OnFileDrop),new object[]{paths,targetNode});

				// somehow querycontinuedrag doesn't work in this case
				UnhighlightTarget();
			}
			else base.OnDragDrop(e);
		}

		private delegate void OnFileDropHandler(string[] paths,TreeNode targetNode);

		protected override void OnQueryContinueDrag(QueryContinueDragEventArgs e)
		{
			base.OnQueryContinueDrag(e);
			if (e.Action == DragAction.Cancel || e.Action == DragAction.Drop)
				UnhighlightTarget();
		}

		private void DragNodes(ArrayList nodes, TreeNode targetNode, DragDropEffects effect)
		{
			if (nodes.Contains(targetNode))
				return; // fail silently

			// first cull child nodes of dragged nodes
			nodes = Simplify(nodes);

			// ignore stupid things
			foreach (TreeNode node in nodes)
				if (IsAncestor(node,targetNode) || node.Parent == targetNode)
					return;

			// we didn't make it :(
			if (nodes.Count < 1) return;

			// ok it's time to move it move it
			foreach (TreeNode node in nodes)
				if (effect == DragDropEffects.Move)
					OnMoveNode(node,targetNode);
				else if (effect == DragDropEffects.Copy)
					OnCopyNode(node,targetNode);
		}

		// override these to do useful things
		protected virtual void OnMoveNode(TreeNode node, TreeNode targetNode){}
		protected virtual void OnCopyNode(TreeNode node, TreeNode targetNode) {}
		protected virtual void OnFileDrop(string[] paths, TreeNode targetNode) {}
		
		/// <summary>
		/// You can override this to change the actual drop target, perhaps to a parent
		/// node if you know for a fact that it doesn't make sense to drop things on
		/// the target node.
		/// </summary>
		protected virtual TreeNode ChangeDropTarget(TreeNode targetNode)
		{
			return targetNode;
		}

		private ArrayList Simplify(ArrayList nodes)
		{
			ArrayList simpleNodes = new ArrayList(nodes);
			foreach (TreeNode node1 in nodes)
				foreach (TreeNode node2 in nodes)
					if (IsAncestor(node1,node2))
						simpleNodes.Remove(node2);
			return simpleNodes;
		}

		private bool IsAncestor(TreeNode node1, TreeNode node2)
		{
			for (TreeNode parent = node2.Parent; parent != null; parent = parent.Parent)
				if (parent == node1) return true;
			return false;
		}

        private bool IsOurDrag(IDataObject o)
        {
            return (o.GetDataPresent(typeof(ArrayList)));
        }

		private bool IsFileDrop(IDataObject o)
		{
			return (o.GetDataPresent(DataFormats.FileDrop));
		}

		private void HighlightTarget(TreeNode node)
		{
			if (node != highlightedNode)
			{
				UnhighlightTarget();
				originalColor = node.BackColor;
				originalText = node.ForeColor;
				highlightedNode = node;
				highlightedNode.BackColor = SystemColors.Highlight;
				highlightedNode.ForeColor = SystemColors.HighlightText;
			}
		}

		private void UnhighlightTarget()
		{
			if (highlightedNode != null)
			{
				highlightedNode.BackColor = originalColor;
				highlightedNode.ForeColor = originalText;
			}
		}

		#region Auto-Scroll

		// Implement an "autoscroll" routine for drag
		//  and drop. If the drag cursor moves to the bottom
		//  or top of the treeview, call the Windows API
		//  SendMessage function to scroll up or down automatically.
		private void DragScroll(DragEventArgs e)
		{
            if (!PluginCore.Win32.ShouldUseWin32()) return;

            // Set a constant to define the autoscroll region
			const Single scrollRegion = 20;

			// See where the cursor is
			Point pt = PointToClient(Cursor.Position);

			// See if we need to scroll up or down
			if ((pt.Y + scrollRegion) > Height)
			{
				// Call the API to scroll down
                PluginCore.Win32.SendMessage(Handle, (int)277, (int)1, 0);
			}
			else if (pt.Y < (Top + scrollRegion))
			{
				// Call thje API to scroll up
				PluginCore.Win32.SendMessage(Handle, (int)277, (int)0, 0);
			}
		}

		#endregion
	}
}
