using System.Drawing;
using System.Collections.Generic;
using System.Linq;

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
            if (!SelectedNodes.Contains(Nodes[0]))
                DoDragDrop(BeginDragNodes(SelectedNodes), DragDropEffects.All);
            else
                base.OnItemDrag(e);
        }

        /// <summary>
        /// Constructs draggable DataObjects from a list of dragged nodes.
        /// </summary>
        protected virtual DataObject BeginDragNodes(IList<TreeNode> nodes)
        {
            var data = new DataObject();
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

            var node = GetNodeAt(PointToClient(new Point(e.X, e.Y)));
            node = ChangeDropTarget(node);

            if (node != null)
            {
                if (!SelectedNodes.Contains(node))
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
                var draggedNodes = (List<TreeNode>)e.Data.GetData(typeof(List<TreeNode>));
                var targetNode = GetNodeAt(PointToClient(new Point(e.X,e.Y)));
                targetNode = ChangeDropTarget(targetNode);

                if (draggedNodes != null && targetNode != null)
                    DragNodes(draggedNodes, targetNode, e.Effect);
            }
            else if (IsFileDrop(e.Data))
            {
                TreeNode targetNode = GetNodeAt(PointToClient(new Point(e.X,e.Y)));
                targetNode = ChangeDropTarget(targetNode);

                if (targetNode is null) return;
                
                // the data is in the form of an array of paths
                Array aFiledrop = (Array)e.Data.GetData(DataFormats.FileDrop);

                // make a string array
                string[] paths = new string[aFiledrop.Length];
                for (int i=0; i<paths.Length; i++)
                    paths[i] = aFiledrop.GetValue(i) as string;

                // queue the copy/move operation so we don't hang this thread and block the calling app
                BeginInvoke(new OnFileDropHandler(OnFileDrop), paths, targetNode);

                // somehow querycontinuedrag doesn't work in this case
                UnhighlightTarget();
            }
            else base.OnDragDrop(e);
        }

        delegate void OnFileDropHandler(string[] paths,TreeNode targetNode);

        protected override void OnQueryContinueDrag(QueryContinueDragEventArgs e)
        {
            base.OnQueryContinueDrag(e);
            if (e.Action == DragAction.Cancel || e.Action == DragAction.Drop)
                UnhighlightTarget();
        }

        void DragNodes(List<TreeNode> nodes, TreeNode targetNode, DragDropEffects effect)
        {
            if (nodes.Contains(targetNode))
                return; // fail silently

            // first cull child nodes of dragged nodes
            nodes = Simplify(nodes);

            // ignore stupid things
            foreach (var node in nodes)
                if (IsAncestor(node,targetNode) || node.Parent == targetNode)
                    return;

            // we didn't make it :(
            if (nodes.Count == 0) return;

            // ok it's time to move it move it
            foreach (var node in nodes)
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
        protected virtual TreeNode ChangeDropTarget(TreeNode targetNode) => targetNode;

        static List<TreeNode> Simplify(List<TreeNode> nodes)
        {
            var result = nodes.ToList();
            foreach (var node1 in nodes)
                foreach (var node2 in nodes)
                    if (IsAncestor(node1,node2))
                        result.Remove(node2);
            return result;
        }

        static bool IsAncestor(TreeNode node1, TreeNode node2)
        {
            for (var parent = node2.Parent; parent != null; parent = parent.Parent)
                if (parent == node1) return true;
            return false;
        }

        static bool IsOurDrag(IDataObject o) => o.GetDataPresent(typeof(List<TreeNode>));

        static bool IsFileDrop(IDataObject o) => o.GetDataPresent(DataFormats.FileDrop);

        void HighlightTarget(TreeNode node)
        {
            if (node != highlightedNode)
            {
                UnhighlightTarget();
                originalColor = node.BackColor;
                originalText = node.ForeColor;
                highlightedNode = node;
                highlightedNode.BackColor = PluginCore.PluginBase.MainForm.GetThemeColor("TreeView.Highlight", SystemColors.Highlight);
                highlightedNode.ForeColor = PluginCore.PluginBase.MainForm.GetThemeColor("TreeView.HighlightText", SystemColors.HighlightText);
            }
        }

        void UnhighlightTarget()
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
        void DragScroll(DragEventArgs e)
        {
            if (!PluginCore.Win32.ShouldUseWin32()) return;

            // Set a constant to define the autoscroll region
            const float scrollRegion = 20;

            // See where the cursor is
            Point pt = PointToClient(Cursor.Position);

            // See if we need to scroll up or down
            if ((pt.Y + scrollRegion) > Height)
            {
                // Call the API to scroll down
                PluginCore.Win32.SendMessage(Handle, 277, 1, 0);
            }
            else if (pt.Y < (Top + scrollRegion))
            {
                // Call thje API to scroll up
                PluginCore.Win32.SendMessage(Handle, 277, 0, 0);
            }
        }

        #endregion
    }
}
