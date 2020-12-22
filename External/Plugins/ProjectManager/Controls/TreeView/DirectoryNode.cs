using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using PluginCore;
using PluginCore.Helpers;
using ProjectManager.Controls.AS3;
using ProjectManager.Projects;
using ProjectManager.Projects.AS3;

namespace ProjectManager.Controls.TreeView
{
    public delegate void DirectoryNodeRefresh(DirectoryNode node);
    public delegate void DirectoryNodeMapping(DirectoryNode node, FileMappingRequest request);

    public class PlaceholderNode : GenericNode
    {
        public PlaceholderNode(string backingPath) : base(backingPath) { }
    }

    public class DirectoryNode : GenericNode
    {
        public static event DirectoryNodeRefresh OnDirectoryNodeRefresh;
        public static event DirectoryNodeMapping OnDirectoryNodeMapping;

        bool dirty;

        public DirectoryNode InsideClasspath { get; private set; }
        public DirectoryNode InsideLibrarypath { get; private set; }

        public DirectoryNode(string directory) : base(directory)
        {
            isDraggable = true;
            isDropTarget = true;
            isRenamable = true;
        }

        public override void Dispose()
        {
            base.Dispose();
            // dispose children
            foreach (GenericNode node in Nodes)
                node.Dispose();
        }

        public override void Refresh(bool recursive)
        {
            if (IsInvalid) return;

            base.Refresh(recursive);

            // item icon
            if (Parent is DirectoryNode node)
            {
                InsideClasspath = node.InsideClasspath;
                InsideLibrarypath = node.InsideLibrarypath;
            }

            var colorId = "ProjectTreeView.ForeColor";
            if (project != null)
            {
                if (project.IsPathHidden(BackingPath)) ImageIndex = Icons.HiddenFolder.Index;
                else if (InsideClasspath is null && project.IsClassPath(BackingPath))
                {
                    InsideClasspath = this;
                    ImageIndex = Icons.ClasspathFolder.Index;
                }
                else if (InsideClasspath != null && project.IsCompileTarget(BackingPath))
                    ImageIndex = Icons.FolderCompile.Index;
                else if (InsideLibrarypath is null && project.IsLibraryAsset(BackingPath))
                {
                    var asset = project.GetAsset(BackingPath);
                    colorId = asset.SwfMode switch
                    {
                        SwfAssetMode.ExternalLibrary => "ProjectTreeView.ExternalLibraryTextColor",
                        SwfAssetMode.Library => "ProjectTreeView.LibraryTextColor",
                        SwfAssetMode.IncludedLibrary => "ProjectTreeView.IncludedLibraryTextColor",
                        _ => colorId
                    };

                    InsideLibrarypath = this;
                    ImageIndex = Icons.LibrarypathFolder.Index;
                }
                else
                    ImageIndex = Icons.Folder.Index;
            }
            else ImageIndex = Icons.Folder.Index;

            SelectedImageIndex = ImageIndex;

            var textColor = PluginBase.MainForm.GetThemeColor(colorId);
            if (colorId != "ProjectTreeView.ForeColor" && textColor == Color.Empty) textColor = SystemColors.Highlight;
            ForeColorRequest = textColor != Color.Empty ? textColor : SystemColors.ControlText;

            // make the plus/minus sign correct
            // TODO: Check if this works ok!
            bool empty = !Directory.Exists(BackingPath) || FolderHelper.IsDirectoryEmpty(BackingPath);

            if (!empty)
            {
                // we want the plus sign because we have *something* in here
                if (Nodes.Count == 0)
                    Nodes.Add(new PlaceholderNode(Path.Combine(BackingPath, "placeholder")));

                // we're already expanded, so refresh our children
                if (IsExpanded || Path.GetDirectoryName(Tree.PathToSelect) == BackingPath)
                    PopulateChildNodes(recursive);
                else
                    dirty = true; // refresh on demand
            }
            else
            {
                // we just became empty!
                if (Nodes.Count > 0)
                    PopulateChildNodes(recursive);
            }

            NotifyRefresh();
        }

        protected virtual void NotifyRefresh()
        {
            // hook for plugins
            OnDirectoryNodeRefresh?.Invoke(this);
        }

        /// <summary>
        /// Signal this node that it is about to expand.
        /// </summary>
        public override void BeforeExpand()
        {
            if (dirty) PopulateChildNodes(false);
        }

        void PopulateChildNodes(bool recursive)
        {
            dirty = false;

            // nuke the placeholder
            if (Nodes.Count == 1 && Nodes[0] is PlaceholderNode)
                Nodes.RemoveAt(0);

            // do a nice stateful update against the filesystem
            var nodesToDie = new GenericNodeList();
            
            // don't remove project output node if it exists - it's annoying when it
            // disappears during a build
            foreach (GenericNode node in Nodes)
            {
                if (node is ProjectOutputNode output)
                {
                    if (project != null && !project.IsPathHidden(output.BackingPath)) output.Refresh(recursive);
                    else nodesToDie.Add(output);
                }
                else if (node is ReferencesNode) node.Refresh(recursive);
                else nodesToDie.Add(node);

                // add any mapped nodes
                if (node is FileNode && !(node is SwfFileNode))
                    nodesToDie.AddRange(node.Nodes);
            }

            if (Directory.Exists(BackingPath))
            {
                PopulateDirectories(nodesToDie, recursive);
                PopulateFiles(nodesToDie, recursive);
            }

            foreach (var node in nodesToDie)
            {
                node.Dispose();
                Nodes.Remove(node);
            }
        }

        void PopulateDirectories(GenericNodeList nodesToDie, bool recursive)
        {
            foreach (string directory in Directory.GetDirectories(BackingPath))
            {
                if (IsDirectoryExcluded(directory))
                    continue;

                DirectoryNode node;
                if (Tree.NodeMap.ContainsKey(directory))
                {
                    node = Tree.NodeMap[directory] as DirectoryNode;
                    if (node != null) // ASClassWizard injects SimpleDirectoryNode != DirectoryNode
                    {
                        if (recursive) node.Refresh(recursive);
                        nodesToDie.Remove(node);
                        continue;
                    }

                    TreeNode fake = Tree.NodeMap[directory];
                    fake.Parent?.Nodes.Remove(fake);
                }

                node = new DirectoryNode(directory);
                InsertNode(Nodes, node);
                node.Refresh(recursive);
                nodesToDie.Remove(node);
            }
        }

        void PopulateFiles(GenericNodeList nodesToDie, bool recursive)
        {
            var files = Directory.GetFiles(BackingPath);
            foreach (string file in files)
            {
                if (IsFileExcluded(file))
                    continue;

                if (Tree.NodeMap.ContainsKey(file))
                {
                    GenericNode node = Tree.NodeMap[file];
                    node.Refresh(recursive);
                    nodesToDie.Remove(node);
                }
                else
                {
                    FileNode node = FileNode.Create(file, project);
                    InsertNode(Nodes, node);
                    node.Refresh(recursive);
                    nodesToDie.Remove(node);
                }
            }

            FileMapping mapping = GetFileMapping(files);
            if (mapping is null) return;

            foreach (string file in files)
            {
                if (IsFileExcluded(file))
                    continue;

                GenericNode node = Tree.NodeMap[file];

                // ensure this file is in the right spot
                if (mapping.ContainsKey(file) && Tree.NodeMap.ContainsKey(mapping[file]))
                    EnsureParentedBy(node, Tree.NodeMap[mapping[file]]);
                else
                    EnsureParentedBy(node, this);
            }
        }

        static void EnsureParentedBy(GenericNode child, TreeNode parent)
        {
            if (child.Parent != parent)
            {
                child.Parent.Nodes.Remove(child);
                InsertNode(parent.Nodes, child);
            }
        }

        // Let another plugin extend the tree by specifying mapping
        FileMapping GetFileMapping(string[] files)
        {
            FileMappingRequest request = new FileMappingRequest(files);

            // Give plugins a chance to respond first
            OnDirectoryNodeMapping?.Invoke(this, request);

            // No one cares?  ok, well we do know one thing: Mxml
            if (request.Mapping.Count == 0 && Tree.Project is AS3Project 
                && PluginMain.Settings.EnableMxmlMapping)
                MxmlFileMapping.AddMxmlMapping(request);

            return request.Mapping.Count > 0 ? request.Mapping : null;
        }

        /// <summary>
        /// Inserts a node in the correct location (sorting alphabetically by
        /// directories first, then files).
        /// </summary>
        /// <param name="node"></param>
        static void InsertNode(TreeNodeCollection nodes, GenericNode node)
        {
            bool inserted = false;

            for (int i=0; i<nodes.Count; i++)
            {
                GenericNode existingNode = nodes[i] as GenericNode;

                if (node is FileNode && existingNode is DirectoryNode)
                    continue;

                if ((node is DirectoryNode && existingNode is FileNode)
                    || string.Compare(existingNode.Text, node.Text, true) > 0)
                {
                    nodes.Insert(i,node);
                    inserted = true;
                    break;
                }
            }

            if (!inserted)
                nodes.Add(node); // append to the end of the list

            // is the tree looking to have this node selected?
            if (Tree.PathToSelect == node.BackingPath)
            {
                // use SelectedNode so multiselect treeview can handle painting
                Tree.SelectedNodes = new List<TreeNode> {node};
            }
        }

        bool IsDirectoryExcluded(string path)
        {
            if (project is null) return false;

            string dirName = Path.GetFileName(path);
            foreach (string excludedDir in PluginMain.Settings.ExcludedDirectories)
                if (dirName.Equals(excludedDir, StringComparison.OrdinalIgnoreCase))
                    return true;

            return !project.ShowHiddenPaths && project.IsPathHidden(path);
        }

        bool IsFileExcluded(string path)
        {
            if (project is null) return false;

            if (path == project.ProjectPath) return true;

            return !project.ShowHiddenPaths && (project.IsPathHidden(path) || path.Contains("\\.") || ProjectTreeView.IsFileTypeHidden(path));
        }
    }
}
