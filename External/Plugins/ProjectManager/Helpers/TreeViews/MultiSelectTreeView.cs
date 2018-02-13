using System.Collections;
using System.Drawing;

namespace System.Windows.Forms
{
    /// <summary>
    /// An enhanced TreeView more appropriate for files/folders, designed to behave
    /// exactly like the excellent treeview setup in Visual Studio.
    ///
    /// This was challenging!  -nick
    /// </summary>
    public class MultiSelectTreeView : TreeViewEx
    {
        bool multiSelect;
        bool ignoreNextMultiSelect;

        ArrayList selectedNodes;
        Hashtable originalColor;
        TreeNode beginRange;
        Timer labelEditTimer;

        public MultiSelectTreeView()
        {
            selectedNodes = new ArrayList();
            originalColor = new Hashtable();
            labelEditTimer = new Timer();
            labelEditTimer.Interval = 1500;
            labelEditTimer.Tick += labelEditTimer_Tick;
        }

        public bool MultiSelect
        {
            get { return multiSelect; }
            set
            {
                multiSelect = value;
                
                if (!multiSelect)
                    foreach (TreeNode node in selectedNodes)
                        UnpaintNode(node);
            }
        }

        public void ForceLabelEdit()
        {
            if (SelectedNode != null)
            {
                labelEditTimer.Enabled = false;
                SelectedNode.BeginEdit();
            }
        }

        private void IgnoreNextLabelEdit()
        {
            labelEditTimer.Enabled = false;
            labelEditTimer.Enabled = true;
        }

        private void labelEditTimer_Tick(object sender, EventArgs e)
        {
            labelEditTimer.Enabled = false;
        }

        // prevents some flicker
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case 0x0014: // Stop erase background message
                    m.Msg = (int)0x0000; // Set to null
                    break;
                case 0xf: // WM_PAINT
                    OnPaint(new PaintEventArgs(Graphics.FromHwnd(this.Handle), this.Bounds));
                    break;
            }
            base.WndProc(ref m);
        }

        /// <summary>
        /// Gets or sets an ArrayList containing the current selected TreeNodes.
        /// </summary>
        public ArrayList SelectedNodes
        {
            get
            {
                if (multiSelect)
                    return selectedNodes;
                else
                    return new ArrayList(new object[]{base.SelectedNode});
            }
            set
            {
                if (value == null)
                {
                    SelectedNode = null;
                    UnselectAllExcept(null);
                }
                else if (value.Count > 0)
                {
                    SelectedNode = value[0] as TreeNode;
                    UnselectAllExcept(SelectedNode);
                    foreach (TreeNode node in value)
                        SelectNode(node);
                }
                else SelectedNode = null;
            }
        }

        // change the behavior of the treeview to select on mouse DOWN instead of
        // mouse UP which causes stupid focus rectangle drawing and flickering.
        protected override void OnMouseDown(MouseEventArgs e)
        {
            TreeNode clickedNode = base.GetNodeAt(e.X, e.Y);

            if (clickedNode != null)
            {
                // if you clicked on an already-selected group of nodes, don't unselect
                // the others yet
                if (clickedNode != SelectedNode && SelectedNodes.Contains(clickedNode))
                    ignoreNextMultiSelect = true;

                if (e.Button == MouseButtons.Left &&
                    clickedNode != base.SelectedNode)
                    IgnoreNextLabelEdit(); // workaround for treeview drawing bug

                // unpaint this node now for less flicker
                if (multiSelect && IsCtrlDown && !IsShiftDown && selectedNodes.Contains(clickedNode))
                    UnpaintNode(clickedNode);
                else
                    base.SelectedNode = clickedNode;
            }

            base.OnMouseDown (e);
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            TreeNode clickedNode = base.GetNodeAt(e.X, e.Y);

            if (clickedNode != null)
            {
                if (multiSelect && e.Button == MouseButtons.Left && 
                    !IsCtrlDown && !IsShiftDown && selectedNodes.Count > 1)
                {
                    IgnoreNextLabelEdit();
                    // we can't call SelectSingle() because we need OnAfterSelect to be called
                    // so the proper events are thrown
                    SelectedNode = null;
                    SelectedNode = clickedNode;
                }
            }

            base.OnMouseUp (e);
        }

        protected override void OnBeforeLabelEdit(NodeLabelEditEventArgs e)
        {
            if (labelEditTimer.Enabled)
                e.CancelEdit = true;

            base.OnBeforeLabelEdit (e);
        }

        #region MultiSelect

        private bool IsCtrlDown { get { return (ModifierKeys & Keys.Control) > 0; } }
        private bool IsShiftDown { get { return (ModifierKeys & Keys.Shift) > 0; } }

        protected override void OnBeforeSelect(TreeViewCancelEventArgs e)
        {
            // we can only do ctrl-deselect in beforeselect
            if (multiSelect && IsCtrlDown && !IsShiftDown && selectedNodes.Contains(e.Node))
            {
                UnselectNode(e.Node);
                e.Cancel = true;
            }
            else base.OnBeforeSelect(e);
        }

        protected override void OnAfterSelect(TreeViewEventArgs e)
        {
            // we want to do nothing if you right-clicked a group of nodes
            if (!multiSelect || ignoreNextMultiSelect)
            {
                ignoreNextMultiSelect = false;
            }
            else if (base.Focused) // only multiselect if we're focused
            {
                // if you were holding ctrl or shift, it's multi-select time!
                if (IsShiftDown && beginRange != null)
                {
                    // multiple shift-groups are only possible with ctrl held down
                    if (!IsCtrlDown) UnselectAllExcept(beginRange);

                    // select all nodes between last beginRange node and this one
                    SelectRange(beginRange,e.Node);
                    SelectNode(beginRange);
                    SelectNode(e.Node);
                }
                else if (IsCtrlDown)
                {
                    beginRange = e.Node;
                    SelectNode(e.Node);
                }
                else SelectSingle(e.Node);
            }
            else
            {
                // paint this node
                SelectSingle(e.Node);
            }
            
            base.OnAfterSelect (e);
        }
        
        private TreeNode FindSelectedNode(TreeNode excludeNode)
        {
            foreach (TreeNode node in selectedNodes)
                if (node != excludeNode)
                    return node;
            return null;
        }

        private void SelectSingle(TreeNode node)
        {
            // unselect all nodes
            foreach (TreeNode selectedNode in selectedNodes)
                UnpaintNode(selectedNode);

            selectedNodes.Clear();
            SelectNode(node);
            beginRange = node;
        }

        private void SelectRange(TreeNode node1, TreeNode node2)
        {
            // just walk the whole damn tree looking for these nodes
            if (node1 == node2) return; // nice try
            bool found = false;
            bool finished = false;
            SelectRange(base.Nodes,node1,node2,ref found,ref finished);
        }

        private void SelectRange(TreeNodeCollection nodes, TreeNode node1, 
            TreeNode node2, ref bool found, ref bool finished)
        {
            foreach (TreeNode node in nodes)
            {
                if (found) SelectNode(node);

                // we've reached a boundary
                if (node == node1 || node == node2)
                {
                    if (!found) found = true;
                    else { finished = true; return; } // done!
                }

                if (node.Nodes.Count > 0 && node.IsExpanded)
                    SelectRange(node.Nodes,node1,node2,ref found, ref finished);

                if (finished) return;
            }
        }

        private void UnselectAllExcept(TreeNode node)
        {
            foreach (TreeNode selectedNode in selectedNodes.Clone() as ArrayList)
                if (selectedNode != node)
                    UnselectNode(selectedNode);
        }

        private void SelectNode(TreeNode node)
        {
            if (!selectedNodes.Contains(node))
            {
                selectedNodes.Add(node);
                PaintNode(node);
            }
        }

        private void UnselectNode(TreeNode node)
        {
            if (selectedNodes.Contains(node))
            {
                UnpaintNode(node);
                selectedNodes.Remove(node);
            }
        }

        private void PaintNode(TreeNode node)
        {
            if (!originalColor.Contains(node))
            {
                originalColor[node] = node.ForeColor;
                node.BackColor = PluginCore.PluginBase.MainForm.GetThemeColor("TreeView.Highlight", SystemColors.Highlight);
                node.ForeColor = PluginCore.PluginBase.MainForm.GetThemeColor("TreeView.HighlightText", SystemColors.HighlightText);
                MultiSelectTreeNode mNode = node as MultiSelectTreeNode;
                if (mNode != null) mNode.Painted = true;
            }
        }

        private void UnpaintNode(TreeNode node)
        {
            if (originalColor.Contains(node))
            {
                MultiSelectTreeNode mNode = node as MultiSelectTreeNode;

                node.BackColor = base.BackColor;
                if (mNode != null)
                {
                    mNode.ForeColor = mNode.ForeColorRequest;
                    mNode.Painted = false;
                }
                else
                    node.ForeColor = (Color)originalColor[node];
                originalColor.Remove(node);
            }
        }

        // unpaint/unselect all nodes when we're not focused because it looks silly

        protected override void OnLostFocus(EventArgs e)
        {
            if (multiSelect)
            {
                UnselectAllExcept(base.SelectedNode);
                if (base.SelectedNode != null)
                    UnpaintNode(base.SelectedNode);
            }
            base.OnLostFocus(e);
        }

        protected override void OnGotFocus(EventArgs e)
        {
            if (multiSelect && base.SelectedNode != null)
                PaintNode(base.SelectedNode);

            base.OnGotFocus (e);
        }

        #endregion
    }

    /// <summary>
    /// A special treenode that plays nice with the multiselect tree view and remembers
    /// the foreground color.
    /// </summary>
    public class MultiSelectTreeNode : TreeNode
    {
        Color foreColorRequest = SystemColors.WindowText;
        internal bool Painted;

        public Color ForeColorRequest
        {
            get { return foreColorRequest; }
            set
            {
                foreColorRequest = value;
                if (!Painted)
                    ForeColor = foreColorRequest;
            }
        }
    }
}
