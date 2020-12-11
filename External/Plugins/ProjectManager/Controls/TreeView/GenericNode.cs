// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using ProjectManager.Projects;

namespace ProjectManager.Controls.TreeView
{
    public abstract class GenericNode : MultiSelectTreeNode, IDisposable
    {
        protected Project project;
        protected bool isInvalid;
        protected bool isRefreshable;
        protected bool isDropTarget;
        protected bool isDraggable;
        protected bool isRenamable;
        public Dictionary<string, object> Meta;

        protected GenericNode(string backingPath)
        {
            BackingPath = backingPath;
            Text = Path.GetFileName(backingPath);
            Tree.NodeMap[backingPath] = this; // create backreference
        }

        /// <summary> Gets whether this node represents invalid data. </summary>
        public bool IsInvalid => isInvalid;

        /// <summary> Gets whether this node can be "refreshed" logically. </summary>
        public bool IsRefreshable => isRefreshable;

        /// <summary> Gets whether this node can have other nodes dropped onto it. </summary>
        public bool IsDropTarget => isDropTarget;

        /// <summary> Gets whether this node can be dragged and dropped elsewhere in the tree. </summary>
        public bool IsDraggable => isDraggable;

        /// <summary> Gets whether this node can be renamed. </summary>
        public bool IsRenamable => isRenamable;

        public virtual void Dispose()
        {
            project = null;
            Tree.NodeMap.Remove(BackingPath);
        }

        public virtual void Refresh(bool recursive)
        {
            if (Parent != null) project = ((GenericNode)Parent).project;
            if (Tree.SelectedNodes.Contains(this))
                Tree.NotifySelectionChanged();
        }

        /// <summary>
        /// Signal this node that it is about to expand.
        /// </summary>
        public virtual void BeforeExpand()
        {
        }

        public string BackingPath { get; }

        protected static ProjectTreeView Tree => ProjectTreeView.Instance;

        public override bool Equals(object obj)
        {
            if (obj is GenericNode node) return node.BackingPath == BackingPath;
            return base.Equals(obj);
        }
    }

    public class GenericNodeList : List<GenericNode>
    {
        public void AddRange(TreeNodeCollection nodes)
        {
            foreach (TreeNode node in nodes)
                if (node is GenericNode genericNode) Add(genericNode);
                else throw new Exception("Unexpected node was not a GenericNode: " + node.Name);
        }
    }
}