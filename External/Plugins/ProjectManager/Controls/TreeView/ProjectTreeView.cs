using System;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ProjectManager.Projects;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using PluginCore;
using PluginCore.Collections;
using PluginCore.Managers;

namespace ProjectManager.Controls.TreeView
{
    public delegate void DragPathEventHandler(string fromPath, string toPath);

    /// <summary>
    /// The Project Explorer TreeView
    /// </summary>
    public class ProjectTreeView : DragDropTreeView, IEventHandler
    {
        readonly Dictionary<string, GenericNode> nodeMap;
        List<Project> projects = new List<Project>();
        Project activeProject;
        public static ProjectTreeView Instance;
        public event DragPathEventHandler MovePath;
        public event DragPathEventHandler CopyPath;
        public new event EventHandler DoubleClick;

        public ProjectTreeView()
        {
            Instance = this;
            MultiSelect = true;
            nodeMap = new Dictionary<string, GenericNode>(StringComparer.OrdinalIgnoreCase);
            ShowNodeToolTips = true;

            EventManager.AddEventHandler(this, EventType.ApplyTheme);
        }

        public void HandleEvent(object sender, NotifyEvent e, HandlingPriority priority)
        {
            if (e.Type == EventType.ApplyTheme)
            {
                RefreshColors();
            }
        }

        void RefreshColors()
        {
            BeginUpdate();
            RefreshNodeColors(Nodes, true);
            EndUpdate();
        }

        void RefreshNodeColors(IEnumerable nodes, bool recursive)
        {
            foreach (GenericNode node in nodes)
            {
                if (recursive) RefreshNodeColors(node.Nodes, recursive);

                node.BackColor = BackColor;
                node.ForeColor = ForeColor;
                node.ForeColorRequest = ForeColor;
            }
        }

        public Project ProjectOf(string path)
        {
            foreach (GenericNode node in Nodes)
            {
                if (node is ProjectNode projectNode && path.StartsWith(projectNode.BackingPath, StringComparison.OrdinalIgnoreCase))
                    return projectNode.ProjectRef;
            }
            return null;
        }

        public Project ProjectOf(GenericNode node)
        {
            var p = node;
            while (p != null && !(p is ProjectNode))
                p = p.Parent as GenericNode;
            return (p as ProjectNode)?.ProjectRef;
        }

        public void Select(string path)
        {
            if (nodeMap.ContainsKey(path)) SelectedNode = nodeMap[path];
            else
            {
                var index = 0;
                var separator = Path.DirectorySeparatorChar.ToString();
                while (true)
                {
                    index = path.IndexOfOrdinal(separator, index);
                    if (index == -1) break; // Stop, not found
                    var subPath = path.Substring(0, index);
                    if (nodeMap.ContainsKey(subPath)) nodeMap[subPath].Expand();
                    index++;
                }
                if (nodeMap.ContainsKey(path)) SelectedNode = nodeMap[path];
            }
        }

        // this is called by GenericNode when a selected node is refreshed, so that
        // the context menu can rebuild itself accordingly.
        public void NotifySelectionChanged() => OnAfterSelect(new TreeViewEventArgs(SelectedNode));

        public static bool IsFileTypeHidden(string path)
        {
            if (Path.GetFileName(path).StartsWithOrdinal("~$")) return true;
            var ext = Path.GetExtension(path).ToLower();
            return PluginMain.Settings.ExcludedFileTypes.Any(ext.Equals);
        }

        public void RefreshNode(GenericNode node)
        {
            // refresh the first parent that *can* be refreshed
            while (node != null && !node.IsRefreshable)
            {
                node = node.Parent as GenericNode;
            }
            if (node is null) return;
            // if you refresh a SwfFileNode this way (by asking for it), you get
            // special feedback
            if (node is SwfFileNode swfNode) swfNode.RefreshWithFeedback(true);
            else node.Refresh(true);
        }

        #region Custom Double-Click Behavior

        protected override void DefWndProc(ref Message m)
        {
            if (m.Msg == 515 && DoubleClick is { } doubleClick) // WM_LBUTTONDBLCLK - &H203
            {
                // ok, we only want the base treeview to handle double-clicking to expand things
                // if there's one node selected and it's a folder.
                if (SelectedNodes.Count == 1 && SelectedNode is DirectoryNode)
                    base.DefWndProc(ref m);
                else
                    doubleClick(this, EventArgs.Empty); // let someone else handle it!
            }
            else base.DefWndProc(ref m);
        }

        #endregion

        #region Properties

        public IDictionary<string, GenericNode> NodeMap => nodeMap;

        public Project Project
        {
            get => activeProject;
            set
            {
                activeProject = value;
                string path = activeProject?.Directory;
                SelectedNode = null;
                foreach (GenericNode node in Nodes)
                {
                    if (!(node is ProjectNode)) continue;
                    if (node.BackingPath == path)
                    {
                        ((ProjectNode) node).IsActive = true;
                        SelectedNode = node;
                    }
                    else ((ProjectNode) node).IsActive = false;
                }
            }
        }

        public List<Project> Projects
        {
            get => projects;
            set
            {
                projects = value != null ? new List<Project>(value) : new List<Project>();

                try
                {
                    BeginUpdate();
                    BuildTree();
                }
                finally
                {
                    EndUpdate();
                }
                Refresh();

                try
                {
                    if (projects.Count > 0)
                    {
                        ExpandedPaths = PluginMain.Settings.GetPrefs(projects[0]).ExpandedPaths;
                        if (Win32.ShouldUseWin32()) Win32.SetScrollPos(this, new Point());
                    }
                    else Project = null;

                    if (Nodes.Count > 0) SelectedNode = Nodes[0] as GenericNode;
                }
                finally
                {
                    EndUpdate();
                }
            }
        }

        public string PathToSelect { get; set; }

        public new GenericNode SelectedNode
        {
            get => base.SelectedNode as GenericNode;
            set => base.SelectedNode = value;
        }

        public string SelectedPath => SelectedNode?.BackingPath;

        public string[] SelectedPaths
        {
            get
            {
                var selectedNodes = SelectedNodes;
                if (selectedNodes.IsNullOrEmpty()) return Array.Empty<string>();
                var result = new List<string>();
                foreach (GenericNode node in selectedNodes)
                {
                    result.Add(node.BackingPath);

                    // if this is a "mapped" file, that is a file that "hides" other related files,
                    // make sure we select the related files also.
                    // DISABLED - causes inconsistent behavior
                    /*if (node is FileNode)
                        foreach (GenericNode mappedNode in node.Nodes)
                            if (mappedNode is FileNode)
                                paths.Add(mappedNode.BackingPath);*/
                }
                return result.ToArray();
            }
            set
            {
                var nodes = new List<TreeNode>();
                foreach (var path in value)
                    if (nodeMap.TryGetValue(path, out var node))
                        nodes.Add(node);
                SelectedNodes = nodes;
            }
        }

        public LibraryAsset SelectedAsset => activeProject.GetAsset(SelectedPath);

        public List<string> ExpandedPaths
        {
            get
            {
                var expanded = new List<string>();
                AddExpanded(Nodes, expanded); // add in the correct order - top-down
                return expanded;
            }
            set
            {
                foreach (var path in value)
                    if (nodeMap.ContainsKey(path))
                    {
                        var node = nodeMap[path];
                        if (!(node is SwfFileNode) && !(node is ProjectNode))
                        {
                            node.Expand();
                            //if (!(node is ReferencesNode))
                            //    node.Refresh(false);
                        }
                    }
            }
        }

        void AddExpanded(IEnumerable nodes, ICollection<string> list)
        {
            foreach (GenericNode node in nodes)
                if (node.IsExpanded)
                {
                    list.Add(node.BackingPath);
                    AddExpanded(node.Nodes, list);
                }
        }

        #endregion

        #region TreeView Population
        /// <summary>
        /// Rebuilds the tree from scratch.
        /// </summary>
        public void RebuildTree()
        {
            var scrollPos = new Point();
            // store old tree state
            var previouslyExpanded = ExpandedPaths;
            if (Win32.ShouldUseWin32()) scrollPos = Win32.GetScrollPos(this);
            var currentPath = SelectedNode?.BackingPath;

            try
            {
                BeginUpdate();
                BuildTree();

                // BUG: avoid nodes expansion to generate redraws
                EndUpdate();
                BeginUpdate();
                
                // restore tree state
                ExpandedPaths = previouslyExpanded;
                if (currentPath != null && NodeMap.ContainsKey(currentPath))
                    SelectedNode = NodeMap[currentPath];
                else
                    SelectedNode = Nodes[0] as GenericNode;// projectNode;
            }
            finally
            {
                EndUpdate();
                if (Win32.ShouldUseWin32()) Win32.SetScrollPos(this, scrollPos);
            }
        }

        /// <summary>
        /// Rebuilds the tree from scratch.
        /// </summary>
        public void BuildTree()
        {
            foreach (GenericNode node in Nodes)
                node.Dispose();

            SelectedNodes = null;
            Nodes.Clear();
            nodeMap.Clear();
            ShowRootLines = true;

            if (projects.Count == 0)
                return;

            foreach (Project project in projects)
                RebuildProjectNode(project);
        }

        void RebuildProjectNode(Project project)
        {
            activeProject = project;

            // create the top-level project node
            var projectNode = new ProjectNode(project);
            Nodes.Add(projectNode);
            projectNode.Refresh(true);
            projectNode.Expand();
            projectNode.References = new ReferencesNode(project, "References");
        }

        /// <summary>
        /// Refreshes all visible nodes in-place.
        /// </summary>
        public void RefreshTree() => RefreshTree(null);

        /// <summary>
        /// Refreshes only the nodes representing the given paths, or all nodes if
        /// paths is null.
        /// </summary>
        public void RefreshTree(string[] paths)
        {
            Point scrollPos = new Point();
            if (Win32.ShouldUseWin32()) scrollPos = Win32.GetScrollPos(this);
            try
            {
                BeginUpdate();
                if (paths is null)
                {
                    // full recursive refresh
                    foreach (GenericNode node in Nodes)
                        node.Refresh(true);
                }
                else
                {
                    // selective refresh
                    foreach (string path in paths)
                        if (nodeMap.ContainsKey(path))
                            nodeMap[path].Refresh(false);
                }
            }
            catch { }
            finally
            {
                EndUpdate();
                if (Win32.ShouldUseWin32()) Win32.SetScrollPos(this, scrollPos);
            }
        }

        #endregion

        #region TreeView Overrides

        protected override void OnBeforeExpand(TreeViewCancelEventArgs e)
        {
            // signal the node about to expand that the expansion is coming
            if (e.Node is GenericNode node)
            {
                BeginUpdate();
                node.BeforeExpand();
                EndUpdate();
            }

            base.OnBeforeExpand(e);
        }

        protected override void OnBeforeLabelEdit(NodeLabelEditEventArgs e)
        {
            base.OnBeforeLabelEdit(e);
            var node = (GenericNode) e.Node;

            if (PathToSelect == node.BackingPath)
            {
                e.CancelEdit = false;
                PathToSelect = null;
            }

            if (!node.IsRenamable)
                e.CancelEdit = true;
        }

        protected override void OnAfterSelect(TreeViewEventArgs e)
        {
            base.OnAfterSelect(e);
            HideSelection = (SelectedNode is ProjectNode);
        }

        protected override void OnItemDrag(ItemDragEventArgs e)
        {
            if (e.Item is GenericNode node && node.IsDraggable)
                base.OnItemDrag(e);
        }

        protected override DataObject BeginDragNodes(List<TreeNode> nodes)
        {
            var data = base.BeginDragNodes(nodes);

            // we also want to drag files, not just nodes, so that we can drop
            // them on explorer, etc.
            var paths = new StringCollection();

            foreach (GenericNode node in nodes)
                paths.Add(node.BackingPath);

            data.SetFileDropList(paths);
            return data;
        }

        protected override void OnMoveNode(TreeNode node, TreeNode targetNode)
        {
            if (node is GenericNode genericNode && MovePath is { } movePath)
            {
                var fromPath = genericNode.BackingPath;
                var toPath = ((GenericNode) targetNode).BackingPath;
                movePath(fromPath, toPath);
            }
        }

        protected override void OnCopyNode(TreeNode node, TreeNode targetNode)
        {
            if (node is GenericNode genericNode && CopyPath is { } copePath)
            {
                var fromPath = genericNode.BackingPath;
                var toPath = ((GenericNode) targetNode).BackingPath;
                copePath(fromPath, toPath);
            }
        }

        protected override void OnFileDrop(string[] paths, TreeNode targetNode)
        {
            if (targetNode is GenericNode node && CopyPath is { } copePath)
            {
                var toPath = node.BackingPath;
                foreach (var fromPath in paths)
                    copePath(fromPath, toPath);
            }
        }

        protected override TreeNode ChangeDropTarget(TreeNode targetNode)
        {
            // you can only drop things into folders
            var node = targetNode as GenericNode;
            while (node != null && (!node.IsDropTarget || node.IsInvalid))
                node = node.Parent as GenericNode;
            return node;
        }


        #endregion

    }
}