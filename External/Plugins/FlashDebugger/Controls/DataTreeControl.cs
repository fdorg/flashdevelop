using System;
using System.Drawing;
using System.Windows.Forms;
using Aga.Controls.Tree;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Aga.Controls.Tree.NodeControls;
using PluginCore.Managers;
using flash.tools.debugger;
using FlashDebugger.Controls.DataTree;
using PluginCore.Localization;
using PluginCore;
using flash.tools.debugger.expression;

namespace FlashDebugger.Controls
{
    public partial class DataTreeControl : UserControl, IToolTipProvider
    {
        public event EventHandler ValueChanged;

        private DataTreeModel _model;
        private static ViewerForm viewerForm;
        private ContextMenuStrip _contextMenuStrip;
        private ToolStripMenuItem copyMenuItem, viewerMenuItem, watchMenuItem, copyValueMenuItem, copyIdMenuItem;
        private DataTreeState state;
        private bool watchMode;
        private bool addingNewExpression;

        public Collection<Node> Nodes
        {
            get { return _model.Root.Nodes; }
        }

        public TreeViewAdv Tree
        {
            get { return _tree; }
        }

        public ViewerForm Viewer
        {
            get { return viewerForm; }
        }

        public DataTreeControl() : this(false){}

        public DataTreeControl(bool watchMode)
        {
            InitializeComponent();
            ValueNodeTextBox.EditEnabled = true;
            ValueNodeTextBox.EditOnClick = true;
            ValueNodeTextBox.ToolTipProvider = this;
            if (watchMode)
            {
                this.watchMode = true;
                NameNodeTextBox.EditEnabled = true;
                NameNodeTextBox.EditorShowing += NameNodeTextBox_EditorShowing;
                NameNodeTextBox.EditorHided += NameNodeTextBox_EditorHided;
                NameNodeTextBox.IsEditEnabledValueNeeded += NameNodeTextBox_IsEditEnabledValueNeeded;
                NameNodeTextBox.LabelChanged += NameNodeTextBox_LabelChanged;
                _tree.KeyDown += Tree_KeyDown;
                _tree.NodeMouseClick += Tree_NameNodeMouseClick;
            }
            _model = new DataTreeModel();
            _tree.Model = _model;
            _tree.FullRowSelect = true;
            Controls.Add(_tree);
            _tree.Expanding += TreeExpanding;
            _tree.NodeMouseDoubleClick += Tree_NodeMouseDoubleClick;
            _tree.LoadOnDemand = true;
            _tree.AutoRowHeight = true;
            _tree.HideSelection = true;

            _tree.CustomDrawHeaders = PluginBase.MainForm.GetThemeColor("ColumnHeader.BorderColor") != Color.Empty;
            _tree.ColumnHeaderBackColor = PluginBase.MainForm.GetThemeColor("ColumnHeader.BackColor", SystemColors.Control);
            _tree.ColumnHeaderTextColor = PluginBase.MainForm.GetThemeColor("ColumnHeader.TextColor", SystemColors.ControlText);
            _tree.ColumnHeaderBorderColor = PluginBase.MainForm.GetThemeColor("ColumnHeader.BorderColor", SystemColors.ActiveBorder);
            _tree.LineColor = PluginBase.MainForm.GetThemeColor("DataTreeControl.LineColor", SystemColors.ActiveBorder);
            _tree.LineColor2 = PluginBase.MainForm.GetThemeColor("DataTreeControl.LineColor", SystemColors.ActiveBorder);
            _tree.DragDropMarkColor = PluginBase.MainForm.GetThemeColor("DataTreeControl.ForeColor", SystemColors.WindowText);

            NameNodeTextBox.DrawText += NameNodeTextBox_DrawText;
            ValueNodeTextBox.DrawText += ValueNodeTextBox_DrawText;
            ValueNodeTextBox.IsEditEnabledValueNeeded += ValueNodeTextBox_IsEditEnabledValueNeeded;
            ValueNodeTextBox.EditorShowing += ValueNodeTextBox_EditorShowing;
            ValueNodeTextBox.EditorHided += ValueNodeTextBox_EditorHided;
            ValueNodeTextBox.LabelChanged += ValueNodeTextBox_LabelChanged;
            _contextMenuStrip = new ContextMenuStrip();
            if (PluginBase.MainForm != null && PluginBase.Settings != null)
            {
                _contextMenuStrip.Font = PluginBase.Settings.DefaultFont;
                _contextMenuStrip.Renderer = new DockPanelStripRenderer(false);
            }
            _tree.ContextMenuStrip = _contextMenuStrip;
            NameTreeColumn.Header = TextHelper.GetString("Label.Name");
            ValueTreeColumn.Header = TextHelper.GetString("Label.Value");
            copyMenuItem = new ToolStripMenuItem(TextHelper.GetString("Label.Copy"), null, CopyItemClick);
            copyValueMenuItem = new ToolStripMenuItem(TextHelper.GetString("Label.CopyValue"), null, CopyItemValueClick);
            copyIdMenuItem = new ToolStripMenuItem(TextHelper.GetString("Label.CopyID"), null, CopyItemIdClick);
            viewerMenuItem = new ToolStripMenuItem(TextHelper.GetString("Label.Viewer"), null, ViewerItemClick);
            _contextMenuStrip.Items.AddRange(new ToolStripItem[]
            {
                copyMenuItem, copyIdMenuItem, copyValueMenuItem, viewerMenuItem
            });
            if (watchMode) watchMenuItem = new ToolStripMenuItem(TextHelper.GetString("Label.Unwatch"), null, WatchItemClick);
            else watchMenuItem = new ToolStripMenuItem(TextHelper.GetString("Label.Watch"), null, WatchItemClick);
            _contextMenuStrip.Items.Add(watchMenuItem);
            _contextMenuStrip.Opening += ContextMenuStrip_Opening;
            viewerForm = new ViewerForm();
            viewerForm.StartPosition = FormStartPosition.Manual;
            // LiveDataTip is created before the settings object. We don't need for it anyway
            if (PluginMain.settingObject != null) PluginMain.settingObject.DataTreeDisplayChanged += DataTreeDisplayChanged;
        }

        void ContextMenuStrip_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            bool enabled = Tree.SelectedNode != null;
            foreach (ToolStripMenuItem item in _contextMenuStrip.Items)
            {
                item.Enabled = enabled;
            }
            if (watchMode) watchMenuItem.Enabled = (enabled && Tree.SelectedNode.Level == 1 && Tree.SelectedNode.NextNode != null);
            bool isValueNode = enabled && (Tree.SelectedNode.Tag as ValueNode) != null;
            copyValueMenuItem.Visible = copyIdMenuItem.Visible = isValueNode;
            while (_contextMenuStrip.Items[_contextMenuStrip.Items.Count - 2] != copyValueMenuItem)
            {
                _contextMenuStrip.Items.RemoveAt(_contextMenuStrip.Items.Count - 2);
            }
            if (isValueNode)
            {
                foreach (var entry in Helpers.DataTreeExporterFactory.Exporters)
                {
                    var exporterItem = new ToolStripMenuItem(TextHelper.GetString("Label.CopyTree"), null, CopyItemTreeClick);
                    if (entry.Key != "") exporterItem.Text += " - " + entry.Key;
                    exporterItem.Tag = entry.Key;
                    _contextMenuStrip.Items.Insert(_contextMenuStrip.Items.Count - 1, exporterItem);
                }
            }
        }

        void DataTreeDisplayChanged(object sender, EventArgs e)
        {
            SaveState();
            foreach (var node in _model.Root.Nodes)
            {
                var valueNode = node as ValueNode;
                if (valueNode != null)
                {
                    if (node.Nodes != null && node.Nodes.Count > 0)
                    {
                        // Needed because of static and inherited members.
                        // If we add a different event or check against a previous value we could avoid removing and reevaluating members.
                        // At any rate, performance shouldn't be a concern here.
                        node.Nodes.Clear();
                        ListChildItems(valueNode);
                    }
                    valueNode.HideClassId = PluginMain.settingObject.HideClassIds;
                    valueNode.HideFullClasspath = PluginMain.settingObject.HideFullClasspaths;
                }

            }
            _tree.FullUpdate();
            RestoreState();
        }

        void NameNodeTextBox_DrawText(object sender, DrawEventArgs e)
        {
            Color grayText = PluginBase.MainForm.GetThemeColor("DataTreeControl.GrayText", SystemColors.GrayText);
            Color hiliteText = PluginBase.MainForm.GetThemeColor("DataTreeControl.HighlightText", SystemColors.HighlightText);
            e.TextColor = PluginBase.MainForm.GetThemeColor("DataTreeControl.ForeColor", SystemColors.WindowText);
            if (e.Node.IsSelected && this.ContainsFocus) e.TextColor = hiliteText;
            try
            {
                if (e.Node.Tag is ErrorNode) e.TextColor = e.Node.IsSelected ? hiliteText : grayText;
            }
            catch (Exception) { }
        }

        void NameNodeTextBox_EditorHided(object sender, EventArgs e)
        {
            if (addingNewExpression)
            {
                NodeTextBox box = sender as NodeTextBox;
                var node = box.Parent.CurrentNode.Tag as Node;
                if (node.Text.Trim() == "") node.Text = TextHelper.GetString("Label.AddExpression");
                addingNewExpression = false;
            }
            // We need to update the tree to avoid some draw problems
            Tree.FullUpdate();
        }

        void NameNodeTextBox_EditorShowing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            NodeTextBox box = sender as NodeTextBox;
            var node = box.Parent.CurrentNode.Tag as Node;
            if (box.Parent.CurrentNode.NextNode == null)
            {
                addingNewExpression = true;
                node.Text = "";
            }
            else addingNewExpression = false;
        }

        void NameNodeTextBox_IsEditEnabledValueNeeded(object sender, NodeControlValueEventArgs e)
        {
            e.Value = e.Node.Level == 1;
        }

        void NameNodeTextBox_LabelChanged(object sender, LabelEventArgs e)
        {
            NodeTextBox box = sender as NodeTextBox;
            if (box.Parent.CurrentNode == null) return;
            DataNode node = box.Parent.CurrentNode.Tag as DataNode;
            if (e.NewLabel.Trim() == "" || e.NewLabel.Trim() == TextHelper.GetString("Label.AddExpression"))
            {
                node.Text = e.OldLabel != "" ? e.OldLabel : TextHelper.GetString("Label.AddExpression");
                return;
            }
            bool newExp;
            if (node.NextNode == null) newExp = PanelsHelper.watchUI.AddElement(e.NewLabel);
            else newExp = PanelsHelper.watchUI.ReplaceElement(e.OldLabel, e.NewLabel);
            if (!newExp) node.Text = e.OldLabel;
        }

        void Tree_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                var node = _tree.SelectedNode;
                if (node != null && node.Level == 1 && node.NextNode != null)
                {
                    PanelsHelper.watchUI.RemoveElement(Tree.SelectedNode.Index);
                }
            }
        }

        void Tree_NameNodeMouseClick(object sender, TreeNodeAdvMouseEventArgs e)
        {
            if (e.Node.Level == 1 && e.Control == NameNodeTextBox && !e.Node.CanExpand && (Tree.SelectedNode == null || Tree.SelectedNode == e.Node))
            {
                NameNodeTextBox.BeginEdit();
                e.Handled = true;
            }
        }

        void ValueNodeTextBox_LabelChanged(object sender, LabelEventArgs e)
        {
            NodeTextBox box = sender as NodeTextBox;
            if (box.Parent.CurrentNode == null) return;
            VariableNode node = box.Parent.CurrentNode.Tag as VariableNode;
            node.IsEditing = false;
            try
            {
                var debugManager = PluginMain.debugManager;
                var flashInterface = debugManager.FlashInterface;
                IASTBuilder b = new ASTBuilder(false);
                ValueExp exp = b.parse(new java.io.StringReader(node.GetVariablePath()));
                var ctx = new ExpressionContext(flashInterface.Session, flashInterface.GetFrames()[debugManager.CurrentFrame]);
                var obj = exp.evaluate(ctx);
                node.Variable = (Variable)obj;
                if (!watchMode) PanelsHelper.watchUI.UpdateElements();
                if (ValueChanged != null) ValueChanged(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(TextHelper.GetString("Error.Reevaluate"), ex);
            }
        }

        void ValueNodeTextBox_EditorShowing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            NodeTextBox box = sender as NodeTextBox;
            VariableNode node = box.Parent.CurrentNode.Tag as VariableNode;
            node.IsEditing = true;
        }

        void ValueNodeTextBox_EditorHided(object sender, EventArgs e)
        {
            // We need to update the tree to avoid some draw problems
            Tree.FullUpdate();
        }

        void ValueNodeTextBox_IsEditEnabledValueNeeded(object sender, NodeControlValueEventArgs e)
        {
            VariableNode node = e.Node.Tag as VariableNode;
            if (node == null || node.Variable == null)
            {
                e.Value = false;
                return;
            }
            int type = node.Variable.getValue().getType();
            // FDB only allows setting the value of scalar variables.
            // Strings can be null, however there is no way with FDB of discerning this case.
            if (node.Variable.isAttributeSet(VariableAttribute_.READ_ONLY) || type != VariableType_.BOOLEAN && type != VariableType_.NUMBER && type != VariableType_.STRING)
            {
                e.Value = false;
            }
        }

        void ValueNodeTextBox_DrawText(object sender, DrawEventArgs e)
        {
            Color grayText = PluginBase.MainForm.GetThemeColor("DataTreeControl.GrayText", SystemColors.GrayText);
            Color errorText = PluginBase.MainForm.GetThemeColor("DataTreeControl.ErrorText", Color.Red);
            Color hiliteText = PluginBase.MainForm.GetThemeColor("DataTreeControl.HighlightText", SystemColors.HighlightText);
            e.TextColor = PluginBase.MainForm.GetThemeColor("DataTreeControl.ForeColor", SystemColors.WindowText);
            if (e.Node.IsSelected && this.ContainsFocus) e.TextColor = hiliteText;
            try
            {
                VariableNode variableNode = e.Node.Tag as VariableNode;
                if (variableNode != null)
                {
                    FlashInterface flashInterface = PluginMain.debugManager.FlashInterface;
                    if (variableNode.Variable != null && variableNode.Variable.hasValueChanged(flashInterface.Session)) 
                    {
                        e.TextColor = errorText;
                    }
                }
                else if (e.Node.Tag is ErrorNode)
                {
                    e.TextColor = e.Node.IsSelected ? hiliteText : grayText;
                }
            }
            catch (NullReferenceException) {}
        }

        public DataNode AddNode(DataNode node)
        {
            _model.Root.Nodes.Add(node);
            return node;
        }

        public DataNode GetNode(string fullpath)
        {
            DataNode node = _model.FindNode(fullpath) as DataNode;
            return node;
        }

        public string GetFullPath(DataNode node)
        {
            return _model.GetFullPath(node);
        }

        private void CopyItemClick(Object sender, System.EventArgs e)
        {
            DataNode node = Tree.SelectedNode.Tag as DataNode;
            Clipboard.SetText(string.Format("{0} = {1}",node.Text, node.Value));
        }
        private void ViewerItemClick(Object sender, System.EventArgs e)
        {
            if (viewerForm == null)
            {
                viewerForm = new ViewerForm();
                viewerForm.StartPosition = FormStartPosition.Manual;
            }
            DataNode node = Tree.SelectedNode.Tag as DataNode;
            viewerForm.Exp = node.Text;
            if (node is ValueNode)
            {
                var vNode = (ValueNode)node;
                // use IsEditing to get unfiltered value
                bool ed = vNode.IsEditing;
                vNode.IsEditing = true;
                viewerForm.Value = node.Value;
                vNode.IsEditing = ed;
            }
            else
            {
                viewerForm.Value = node.Value;
            }
            Form mainform = (PluginBase.MainForm as Form);
            viewerForm.Left = mainform.Left + mainform.Width / 2 - viewerForm.Width / 2;
            viewerForm.Top = mainform.Top + mainform.Height / 2 - viewerForm.Height / 2;
            viewerForm.ShowDialog();
        }

        private void WatchItemClick(Object sender, EventArgs e)
        {
            DataNode node = Tree.SelectedNode.Tag as DataNode;
            if (watchMode) PanelsHelper.watchUI.RemoveElement(Tree.SelectedNode.Index);
            else PanelsHelper.watchUI.AddElement(node.GetVariablePath());
        }

        void TreeExpanding(Object sender, TreeViewAdvEventArgs e)
        {
            if (e.Node.Index >= 0)
            {
                ListChildItems(e.Node.Tag as ValueNode);
            }
        }

        public void ListChildItems(ValueNode node)
        {
            if (node != null && node.Nodes.Count == 0 && node.PlayerValue != null)
            {
                FlashInterface flashInterface = PluginMain.debugManager.FlashInterface;
                List<VariableNode> nodes = new List<VariableNode>();
                List<VariableNode> inherited = new List<VariableNode>();
                List<VariableNode> statics = new List<VariableNode>();
                int tmpLimit = node.ChildrenShowLimit;
                foreach (Variable member in node.PlayerValue.getMembers(flashInterface.Session))
                {
                    VariableNode memberNode = new VariableNode(member)
                    {
                        HideClassId = PluginMain.settingObject.HideClassIds,
                        HideFullClasspath = PluginMain.settingObject.HideFullClasspaths
                    };
                    if (member.isAttributeSet(VariableAttribute_.IS_STATIC))
                    {
                        statics.Add(memberNode);
                    }
                    else if (member.getLevel() > 0)
                    {
                        inherited.Add(memberNode);
                    }
                    else nodes.Add(memberNode);
                }
                // inherited vars
                if (inherited.Count > 0)
                {
                    if (PluginMain.settingObject.CombineInherited)
                    {
                        // list inherited alongside main class members
                        foreach (DataNode item in inherited)
                        {
                            node.Nodes.Add(item);
                        }

                    }
                    else
                    {
                        // list inherited in a [inherited] group
                        ValueNode inheritedNode = new ValueNode("[inherited]");
                        inherited.Sort();
                        foreach (DataNode item in inherited)
                        {
                            inheritedNode.Nodes.Add(item);
                        }
                        node.Nodes.Add(inheritedNode);

                    }
                }
                // static vars
                if (!PluginMain.settingObject.HideStaticMembers && statics.Count > 0)
                {
                    DataNode staticNode = new ValueNode("[static]");
                    statics.Sort();
                    foreach (DataNode item in statics)
                    {
                        staticNode.Nodes.Add(item);
                    }
                    node.Nodes.Add(staticNode);
                }
                // test children
                foreach (String ch in node.PlayerValue.getClassHierarchy(false))
                {
                    if (ch.Equals("flash.display::DisplayObjectContainer"))
                    {
                        double numChildren = ((java.lang.Double)node.PlayerValue.getMemberNamed(flashInterface.Session, "numChildren").getValue().getValueAsObject()).doubleValue();
                        DataNode childrenNode = new ValueNode("[children]");
                        for (int i = 0; i < numChildren; i++)
                        {
                            try
                            {
                                IASTBuilder b = new ASTBuilder(false);
                                string cmd = node.GetVariablePath() + ".getChildAt(" + i + ")";
                                ValueExp exp = b.parse(new java.io.StringReader(cmd));
                                var ctx = new ExpressionContext(flashInterface.Session, flashInterface.GetFrames()[PluginMain.debugManager.CurrentFrame]);
                                var obj = exp.evaluate(ctx);
                                if (obj is flash.tools.debugger.concrete.DValue) obj = new flash.tools.debugger.concrete.DVariable("getChildAt(" + i + ")", (flash.tools.debugger.concrete.DValue)obj, ((flash.tools.debugger.concrete.DValue)obj).getIsolateId());
                                DataNode childNode = new VariableNode((Variable) obj)
                                {
                                    HideClassId = PluginMain.settingObject.HideClassIds,
                                    HideFullClasspath = PluginMain.settingObject.HideFullClasspaths
                                };
                                childNode.Text = "child_" + i;
                                childrenNode.Nodes.Add(childNode);
                            }
                            catch (Exception) { }
                        }
                        node.Nodes.Add(childrenNode);
                    }
                    else if (ch.Equals("flash.events::EventDispatcher"))
                    {
                        Variable list = node.PlayerValue.getMemberNamed(flashInterface.Session, "listeners");
                        var omg = list.getName();
                        /*
                        double numChildren = ((java.lang.Double)node.Variable.getValue().getMemberNamed(flashInterface.Session, "numChildren").getValue().getValueAsObject()).doubleValue();
                        DataNode childrenNode = new DataNode("[children]");
                        for (int i = 0; i < numChildren; i++)
                        {
                            try
                            {

                                IASTBuilder b = new ASTBuilder(false);
                                string cmd = GetVariablePath(node) + ".getChildAt(" + i + ")";
                                ValueExp exp = b.parse(new java.io.StringReader(cmd));
                                var ctx = new ExpressionContext(flashInterface.Session, flashInterface.Session.getFrames()[PluginMain.debugManager.CurrentFrame]);
                                var obj = exp.evaluate(ctx);
                                if (obj is flash.tools.debugger.concrete.DValue) obj = new flash.tools.debugger.concrete.DVariable("child_" + i, (flash.tools.debugger.concrete.DValue)obj);
                                DataNode childNode = new DataNode((Variable)obj);
                                childrenNode.Nodes.Add(childNode);
                            }
                            catch (Exception) { }
                        }
                        node.Nodes.Add(childrenNode);
                        */
                    }
                }
                //test children
                nodes.Sort();
                // add child items
                _tree.BeginUpdate();
                foreach (DataNode item in nodes)
                {
                    if (0 == tmpLimit--) break;
                    node.Nodes.Add(item);
                }
                if (tmpLimit == -1)
                {
                    DataNode moreNode = new ContinuedDataNode();
                    node.Nodes.Add(moreNode);
                }
                _tree.EndUpdate();
            }
        }

        void Tree_NodeMouseDoubleClick(object sender, TreeNodeAdvMouseEventArgs e)
        {
            DataNode node = e.Node.Tag as ContinuedDataNode;
            if (node != null)
            {
                e.Handled = true;
                _tree.BeginUpdate();
                (node.Parent as ValueNode).ChildrenShowLimit += 500;
                TreeNodeAdv parent = e.Node.Parent;
                int ind = e.Node.Index;
                parent.Collapse(true);
                node.Parent.Nodes.Clear();
                parent.Expand(true);
                _tree.EndUpdate();
                if (parent.Children.Count>ind) _tree.ScrollTo(parent.Children[ind]);
            }
        }

        public void SaveState()
        {
            if (state == null) state = new DataTreeState();
            state.Selected = _tree.SelectedNode == null ? null : _model.GetFullPath(_tree.SelectedNode.Tag as Node);
            state.Expanded.Clear();
            if (Nodes != null && Nodes.Count > 0) SaveExpanded(Nodes);
            SaveScrollState();
        }

        private void SaveExpanded(Collection<Node> nodes)
        {
            foreach (Node node in nodes)
            {
                if (!node.IsLeaf && Tree.FindNode(_model.GetPath(node)).IsExpanded)
                {
                    state.Expanded.Add(_model.GetFullPath(node));
                    if (node.Nodes.Count > 0) SaveExpanded(node.Nodes);
                }
            }
        }

        private void SaveScrollState()
        {
            if (Nodes.Count < 1)
            {
                state.TopPath = state.BottomPath = null;
                return;
            }
            var topNode = _tree.FirstVisibleNode;
            state.TopPath = topNode != null ? _model.GetFullPath(_tree.FirstVisibleNode.Tag as Node) : null;
            var bottomNode = _tree.LastVisibleNode;
            state.BottomPath = bottomNode != null ? _model.GetFullPath(bottomNode.Tag as Node) : null;
        }

        public void RestoreState()
        {
            if (state == null) return;
            if (state.Expanded != null && state.Expanded.Count > 0) RestoreExpanded(Nodes);
            if (state.Selected != null) _tree.SelectedNode = _tree.FindNodeByTag(_model.FindNode(state.Selected));
            RestoreScrollState();
        }

        private void RestoreExpanded(Collection<Node> nodes)
        {
            foreach (Node node in nodes)
            {
                if (!node.IsLeaf && state.Expanded.Contains(_model.GetFullPath(node)))
                {
                    Tree.FindNode(_model.GetPath(node)).Expand();
                    if (node.Nodes.Count > 0) RestoreExpanded(node.Nodes);
                }
            }
        }

        private void RestoreScrollState()
        {
            if (Nodes.Count < 1) return;

            if (state.BottomPath != null)
            {
                var bottomNode = Tree.FindNodeByTag(_model.FindNode(state.BottomPath));
                if (bottomNode != null) Tree.EnsureVisible(bottomNode);
            }

            if (state.TopPath != null)
            {
                var topNode = Tree.FindNodeByTag(_model.FindNode(state.TopPath));
                if (topNode != null) Tree.EnsureVisible(topNode);
            }
        }

        #region IToolTipProvider Members

        public string GetToolTip(TreeNodeAdv node, NodeControl nodeControl)
        {
            DataNode dataNode = node.Tag as DataNode;
            string r;
            if (dataNode is ValueNode)
            {
                var vNode = (ValueNode)dataNode;
                // use IsEditing to get unfiltered value
                bool e = vNode.IsEditing;
                vNode.IsEditing = true;
                r = dataNode.Value;
                vNode.IsEditing = e;
            }
            else
                r = dataNode.Value;
            if (r.Length > 300) r = r.Substring(0, 300) + "[...]";
            return r;
        }

        #endregion

        #region State Class

        private class DataTreeState
        {

            public HashSet<string> Expanded = new HashSet<String>();
            public string Selected;
            public string TopPath;
            public string BottomPath;
        }

        #endregion
        
        #region Copy Value, ID, Tree

        private void CopyItemValueClick(Object sender, System.EventArgs e)
        {
            ValueNode node = Tree.SelectedNode.Tag as ValueNode;
            string value = node.Value;
            if (!string.IsNullOrEmpty(value))
                Clipboard.SetText(value);
            else
                Clipboard.Clear();
        }

        private void CopyItemIdClick(Object sender, System.EventArgs e)
        {
            ValueNode node = Tree.SelectedNode.Tag as ValueNode;
            string id = node.Id;
            if (!string.IsNullOrEmpty(id))
                Clipboard.SetText(node.Id);
            else
                Clipboard.Clear();
        }

        private void CopyItemTreeClick(Object sender, System.EventArgs e)
        {
            string exporterKey = (string) ((ToolStripItem) sender).Tag;
            CopyTreeInternal(exporterKey, 0);
        }

        private void CopyTreeInternal(string exporterKey, int levelLimit)
        {
            var node = Tree.SelectedNode.Tag as ValueNode;
            var exporter = Helpers.DataTreeExporterFactory.Exporters[exporterKey];
            exporter.CopyTreeMaxChars = PluginMain.settingObject.CopyTreeMaxChars;
            exporter.CopyTreeMaxRecursion = PluginMain.settingObject.CopyTreeMaxRecursion;
            Clipboard.SetText(exporter.GetTreeAsText(node, "\t", this, levelLimit));
        }
        
        #endregion
        
    }

}
