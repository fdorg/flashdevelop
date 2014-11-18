using System;
using System.Drawing;
using System.Windows.Forms;
using Aga.Controls.Tree;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Aga.Controls.Tree.NodeControls;
using flash.tools.debugger;
using PluginCore.Localization;
using PluginCore;
using flash.tools.debugger.expression;

namespace FlashDebugger.Controls
{
    public partial class DataTreeControl : UserControl, IToolTipProvider
    {
        private DataTreeModel _model;
        private static ViewerForm viewerForm = null;
        private ContextMenuStrip _contextMenuStrip;
        private ToolStripMenuItem copyMenuItem, viewerMenuItem, watchMenuItem;
		private List<String> expandedList = new List<String>();
		private bool watchMode;

        public Collection<Node> Nodes
        {
            get
			{
				return _model.Root.Nodes;
			}
        }

        public TreeViewAdv Tree
        {
            get
			{
				return _tree;
			}
        }

        public ViewerForm Viewer
        {
            get
			{
				return viewerForm;
			}
        }

		public DataTreeControl():this(false)
		{
		}

		public DataTreeControl(bool watchMode)
        {
            InitializeComponent();
            this.ValueNodeTextBox.ToolTipProvider = this;
			this.watchMode = watchMode;
            _model = new DataTreeModel();
            _tree.Model = _model;
            this.Controls.Add(_tree);
			_tree.Expanding += new EventHandler<TreeViewAdvEventArgs>(TreeExpanding);
			_tree.SelectionChanged += new EventHandler(TreeSelectionChanged);
			_tree.LoadOnDemand = true;
			_tree.AutoRowHeight = true;
			NameNodeTextBox.IsEditEnabledValueNeeded += new EventHandler<NodeControlValueEventArgs>(NameNodeTextBox_IsEditEnabledValueNeeded);
			ValueNodeTextBox.DrawText += new EventHandler<DrawEventArgs>(ValueNodeTextBox_DrawText);
			ValueNodeTextBox.IsEditEnabledValueNeeded += new EventHandler<NodeControlValueEventArgs>(ValueNodeTextBox_IsEditEnabledValueNeeded);
			ValueNodeTextBox.EditorShowing += new System.ComponentModel.CancelEventHandler(ValueNodeTextBox_EditorShowing);
			ValueNodeTextBox.EditorHided += new EventHandler(ValueNodeTextBox_EditorHided);
			_tree.NodeMouseDoubleClick += new EventHandler<TreeNodeAdvMouseEventArgs>(_tree_NodeMouseDoubleClick);
			_contextMenuStrip = new ContextMenuStrip();
			if (PluginBase.MainForm != null && PluginBase.Settings != null)
			{
				_contextMenuStrip.Font = PluginBase.Settings.DefaultFont;
                _contextMenuStrip.Renderer = new DockPanelStripRenderer(false);
			}
			_tree.ContextMenuStrip = _contextMenuStrip;
            this.NameTreeColumn.Header = TextHelper.GetString("Label.Name");
            this.ValueTreeColumn.Header = TextHelper.GetString("Label.Value");
            copyMenuItem = new ToolStripMenuItem(TextHelper.GetString("Label.Copy"), null, new EventHandler(this.CopyItemClick));
            viewerMenuItem = new ToolStripMenuItem(TextHelper.GetString("Label.Viewer"), null, new EventHandler(this.ViewerItemClick));
            _contextMenuStrip.Items.AddRange(new ToolStripMenuItem[] { copyMenuItem, viewerMenuItem});
			if (watchMode)
			{
				watchMenuItem = new ToolStripMenuItem(TextHelper.GetString("Label.Unwatch"), null, new EventHandler(this.WatchItemClick));
			}
			else
			{
				watchMenuItem = new ToolStripMenuItem(TextHelper.GetString("Label.Watch"), null, new EventHandler(this.WatchItemClick));
			}
			_contextMenuStrip.Items.Add(watchMenuItem);
			TreeSelectionChanged(null, null);
            viewerForm = new ViewerForm();
            viewerForm.StartPosition = FormStartPosition.Manual;
        }

		void NameNodeTextBox_IsEditEnabledValueNeeded(object sender, NodeControlValueEventArgs e)
		{
			e.Value = false;
		}

		void ValueNodeTextBox_EditorHided(object sender, EventArgs e)
		{
			NodeTextBox box = sender as NodeTextBox;
			DataNode node = box.Parent.CurrentNode.Tag as DataNode;
			node.IsEditing = false;
		}

		void ValueNodeTextBox_EditorShowing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			NodeTextBox box = sender as NodeTextBox;
			DataNode node = box.Parent.CurrentNode.Tag as DataNode;
			node.IsEditing = true;
		}

		void ValueNodeTextBox_IsEditEnabledValueNeeded(object sender, NodeControlValueEventArgs e)
		{
            DataNode node = e.Node.Tag as DataNode;
			int type = node.Variable.getValue().getType();
			if (type != VariableType_.BOOLEAN && type != VariableType_.NUMBER && type != VariableType_.STRING)
			{
				e.Value = false;
			}
		}

		void ValueNodeTextBox_DrawText(object sender, DrawEventArgs e)
		{
            try
            {
                DataNode node = e.Node.Tag as DataNode;
                FlashInterface flashInterface = PluginMain.debugManager.FlashInterface;
                if (node.Variable != null && node.Variable.hasValueChanged(flashInterface.Session))
                {
                    e.TextColor = Color.Red;
                }
            }
            catch (NullReferenceException) { }
		}

        public DataNode AddNode(DataNode node)
        {
			_model.Root.Nodes.Add(node);
			RestoreExpanded();
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

		public void Clear()
		{
			if (Nodes.Count > 0) SaveExpanded();
			Nodes.Clear();
		}


        private void CopyItemClick(Object sender, System.EventArgs e)
        {
            if (Tree.SelectedNode != null)
            {
                DataNode node = Tree.SelectedNode.Tag as DataNode;
                Clipboard.SetText(string.Format("{0} = {1}",node.Text, node.Value));
            }  
        }
        private void ViewerItemClick(Object sender, System.EventArgs e)
        {
            if (Tree.SelectedNode != null)
            {
                if (viewerForm == null)
                {
                    viewerForm = new ViewerForm();
                    viewerForm.StartPosition = FormStartPosition.Manual;
                }
                DataNode node = Tree.SelectedNode.Tag as DataNode;
                viewerForm.Exp = node.Text;
                // use IsEditing to get unfiltered value
                bool ed = node.IsEditing;
                node.IsEditing = true;
				viewerForm.Value = node.Value;
                node.IsEditing = ed;
                Form mainform = (PluginBase.MainForm as Form);
                viewerForm.Left = mainform.Left + mainform.Width / 2 - viewerForm.Width / 2;
                viewerForm.Top = mainform.Top + mainform.Height / 2 - viewerForm.Height / 2;
                viewerForm.ShowDialog();
            }
        }
		private void WatchItemClick(Object sender, EventArgs e)
		{
			if (Tree.SelectedNode == null) return;
			DataNode node = Tree.SelectedNode.Tag as DataNode;
			if (watchMode)
			{
				PanelsHelper.watchUI.RemoveElement(Tree.SelectedNode.Row);
			}
			else
			{
				PanelsHelper.watchUI.AddElement(GetVariablePath(node));
			}
		}

		private String GetVariablePath(Node node)
		{
			String ret = "";
			if (node.Tag != null && node.Tag is String)
                return (String)node.Tag; // fix for: live tip value has no parent
            if (node.Parent != null) ret = GetVariablePath(node.Parent);
			if (node is DataNode)
			{
				DataNode datanode = node as DataNode;
				if (datanode.Variable != null)
				{
					if (ret == "") return datanode.Variable.getName();
					if ((datanode.Variable.getAttributes() & 0x00020000) == 0x00020000) //VariableAttribute_.IS_DYNAMIC
					{
						ret += "[\"" + datanode.Variable.getName() + "\"]";
					}
					else
					{
						ret += "." + datanode.Variable.getName();
					}
				}
			}
			return ret;
		}

		void TreeSelectionChanged(Object sender, EventArgs e)
		{
			foreach (ToolStripMenuItem item in _contextMenuStrip.Items)
			{
				item.Enabled = (Tree.SelectedNode != null);
			}
			if (watchMode) watchMenuItem.Enabled = (Tree.SelectedNode != null && Tree.SelectedNode.Level == 1);
		}

		void TreeExpanding(Object sender, TreeViewAdvEventArgs e)
        {
            if (e.Node.Index >= 0)
            {
                DataNode node = e.Node.Tag as DataNode;
				if (node.Nodes.Count == 0 && node.Variable != null)
                {
					FlashInterface flashInterface = PluginMain.debugManager.FlashInterface;
					List<DataNode> nodes = new List<DataNode>();
					List<DataNode> inherited = new List<DataNode>();
					List<DataNode> statics = new List<DataNode>();
					int tmpLimit = node.ChildrenShowLimit;
					foreach (Variable member in node.Variable.getValue().getMembers(flashInterface.Session))
					{
						DataNode memberNode = new DataNode(member);
                        
						if (member.isAttributeSet(VariableAttribute_.IS_STATIC))
						{
							statics.Add(memberNode);
						}
						else if (member.getLevel() > 0)
						{
							inherited.Add(memberNode);
						}
						else
						{
							nodes.Add(memberNode);
						}
					}
					if (inherited.Count > 0)
					{
						DataNode inheritedNode = new DataNode("[inherited]");
						inherited.Sort();
						foreach (DataNode item in inherited)
						{
							inheritedNode.Nodes.Add(item);
						}
						node.Nodes.Add(inheritedNode);
					}
					if (statics.Count > 0)
					{
						DataNode staticNode = new DataNode("[static]");
						statics.Sort();
						foreach (DataNode item in statics)
						{
							staticNode.Nodes.Add(item);
						}
						node.Nodes.Add(staticNode);
					}
					//test children
					foreach (String ch in node.Variable.getValue().getClassHierarchy(false))
					{
						if (ch.Equals("flash.display::DisplayObjectContainer"))
						{
							double numChildren = ((java.lang.Double)node.Variable.getValue().getMemberNamed(flashInterface.Session, "numChildren").getValue().getValueAsObject()).doubleValue();
							DataNode childrenNode = new DataNode("[children]");
							for (int i = 0; i < numChildren; i++)
							{
								try
								{
                                    IASTBuilder b = new ASTBuilder(false);
                                    string cmd = GetVariablePath(node) + ".getChildAt(" + i + ")";
                                    ValueExp exp = b.parse(new java.io.StringReader(cmd));
                                    var ctx = new ExpressionContext(flashInterface.Session, flashInterface.GetFrames()[PluginMain.debugManager.CurrentFrame]);
                                    var obj = exp.evaluate(ctx);
                                    if (obj is flash.tools.debugger.concrete.DValue) obj = new flash.tools.debugger.concrete.DVariable("getChildAt(" + i + ")", (flash.tools.debugger.concrete.DValue)obj, ((flash.tools.debugger.concrete.DValue)obj).getIsolateId());
                                    DataNode childNode = new DataNode((Variable)obj);
                                    childNode.Text = "child_" + i;
									childrenNode.Nodes.Add(childNode);
								}
                                catch (Exception) { }
							}
							node.Nodes.Add(childrenNode);
						}
                        else if (ch.Equals("flash.events::EventDispatcher"))
                        {
                            Variable list = node.Variable.getValue().getMemberNamed(flashInterface.Session, "listeners");
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
                             * */
                        }
					}
					//test children
					nodes.Sort();
					_tree.BeginUpdate();
					foreach (DataNode item in nodes)
					{
						if (0 == tmpLimit--) break;
						node.Nodes.Add(item);
					}
					if (tmpLimit == -1)
					{
						DataNode moreNode = new DataNode("...");
						node.Nodes.Add(moreNode);
					}
					_tree.EndUpdate();
                }
            }
        }

		void _tree_NodeMouseDoubleClick(object sender, TreeNodeAdvMouseEventArgs e)
		{
			DataNode node = e.Node.Tag as DataNode;
			if (node.Text == "..." && node.Variable == null)
			{
				e.Handled = true;
				_tree.BeginUpdate();
				(node.Parent as DataNode).ChildrenShowLimit += 500;
				TreeNodeAdv parent = e.Node.Parent;
				int ind = e.Node.Index;
				parent.Collapse(true);
				node.Parent.Nodes.Clear();
				parent.Expand(true);
				_tree.EndUpdate();
				if (parent.Children.Count>ind) _tree.ScrollTo(parent.Children[ind]);
			}
		}

		public void SaveExpanded()
		{
			expandedList.Clear();
			SaveExpanded(Nodes);
		}

		private void SaveExpanded(Collection<Node> nodes)
		{
			if (nodes == null) return;
			foreach (DataNode node in nodes)
			{
				if (Tree.FindNode(_model.GetPath(node)).IsExpanded)
				{
					expandedList.Add(_model.GetFullPath(node));
					SaveExpanded(node.Nodes);
				}
			}
		}

		public void RestoreExpanded()
		{
			RestoreExpanded(Nodes);
		}

		private void RestoreExpanded(Collection<Node> nodes)
		{
			if (nodes == null) return;
			foreach (DataNode node in nodes)
			{
				if (expandedList.Contains(_model.GetFullPath(node)))
				{
					Tree.FindNode(_model.GetPath(node)).Expand();
					RestoreExpanded(node.Nodes);
				}
			}
		}


        #region IToolTipProvider Members

        public string GetToolTip(TreeNodeAdv node, NodeControl nodeControl)
        {
            DataNode dataNode = node.Tag as DataNode;
            // use IsEditing to get unfiltered value
            bool e = dataNode.IsEditing;
            dataNode.IsEditing = true;
            string r = dataNode.Value;
            dataNode.IsEditing = e;
            if (r.Length > 300) r = r.Substring(0, 300) + "[...]";
            return r;
        }

        #endregion
    }

}
