using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using ProjectManager.Projects;

namespace ProjectManager.Controls.TreeView
{
    public abstract class GenericNode : MultiSelectTreeNode, IDisposable
    {
        string backingPath;

        protected Project project;
        protected bool isInvalid;
        protected bool isRefreshable;
        protected bool isDropTarget;
        protected bool isDraggable;
        protected bool isRenamable;
        public Dictionary<string, object> Meta;

        protected GenericNode(string backingPath)
        {
            this.backingPath = backingPath;

            Text = Path.GetFileName(backingPath);
            Tree.NodeMap[backingPath] = this; // create backreference
        }

        /// <summary> Gets whether this node represents invalid data. </summary>
        public bool IsInvalid { get { return isInvalid; } }

        /// <summary> Gets whether this node can be "refreshed" logically. </summary>
        public bool IsRefreshable { get { return isRefreshable; } }

        /// <summary> Gets whether this node can have other nodes dropped onto it. </summary>
        public bool IsDropTarget { get { return isDropTarget; } }

        /// <summary> Gets whether this node can be dragged and dropped elsewhere in the tree. </summary>
        public bool IsDraggable { get { return isDraggable; } }

        /// <summary> Gets whether this node can be renamed. </summary>
        public bool IsRenamable { get { return isRenamable; } }

        public virtual void Dispose()
        {
            project = null;
            Tree.NodeMap.Remove(backingPath);
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

        public string BackingPath
        {
            get { return backingPath; }
            set { backingPath = value; }
        }

        protected static ProjectTreeView Tree
        {
            get { return ProjectTreeView.Instance; }
        }

        public override bool Equals(object obj)
        {
            GenericNode node = obj as GenericNode;
            if (node != null)
                return node.BackingPath == backingPath;
            else
                return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    public class GenericNodeList : List<GenericNode>
    {
        public void AddRange(TreeNodeCollection nodes)
        {
            foreach (TreeNode node in nodes)
                if (node is GenericNode)
                    Add(node as GenericNode);
                else
                    throw new Exception("Unexpected node was not a GenericNode: " + node.Name);
        }
    }
}
