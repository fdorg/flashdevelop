using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using PluginCore;
using PluginCore.Helpers;
using ProjectManager.Controls.TreeView;
using SourceControl.Actions;
using SourceControl.Sources;

namespace SourceControl.Managers
{
    public class OverlayManager
    {
        public const string META_VC = "SourceControl.VC";
        public const string META_ROOT = "SourceControl.ROOT";
        public const string META_STATUS = "SourceControl.STATUS";

        ProjectTreeView currentTree;
        FSWatchers fsWatchers;

        public OverlayManager(FSWatchers fsWatchers)
        {
            this.fsWatchers = fsWatchers;

            FileNode.OnFileNodeRefresh += FileNode_OnFileNodeRefresh;
            DirectoryNode.OnDirectoryNodeRefresh += DirectoryNode_OnDirectoryNodeRefresh;

            OverlayMap.Init();
        }

        internal void Dispose()
        {
            OverlayMap.Reset();
            fsWatchers = null;
        }

        internal void Reset()
        {
            OverlayMap.Reset();
            if (currentTree != null) ResetNodes(currentTree.Nodes);
        }

        internal void Refresh()
        {
            if (currentTree != null)
            {
                RefreshNodes(currentTree.Nodes);
                SelectionChanged();
            }
        }

        private void RefreshNodes(IEnumerable nodes)
        {
            foreach (TreeNode node in nodes)
            {
                if (node is DirectoryNode directoryNode)
                {
                    if (UpdateNodeStatus(directoryNode))
                        RefreshNodes(directoryNode.Nodes);
                }
                else if (node is FileNode fileNode)
                {
                    UpdateNodeStatus(fileNode);
                }
            }
        }

        internal void SelectionChanged()
        {
            ProjectSelectionState state = new ProjectSelectionState(currentTree);
            TreeContextMenuUpdate.SetMenu(currentTree, state);
        }

        void ResetNodes(IEnumerable nodes)
        {
            foreach (TreeNode node in nodes)
            {
                GenericNode gnode = node as GenericNode;
                if (gnode?.Meta != null && gnode.Meta.ContainsKey(META_VC))
                {
                    gnode.Meta.Remove(META_VC);
                    gnode.Meta.Remove(META_ROOT);
                    gnode.Meta.Remove(META_STATUS);
                }
                if (node.Nodes.Count > 0) ResetNodes(node.Nodes);
            }
        }

        void DirectoryNode_OnDirectoryNodeRefresh(DirectoryNode node)
        {
            if (node is ProjectNode)
                currentTree = node.TreeView as ProjectTreeView;

            UpdateNodeStatus(node);
        }

        void FileNode_OnFileNodeRefresh(FileNode node) => UpdateNodeStatus(node);

        bool UpdateNodeStatus(GenericNode node)
        {
            if (node.Meta is null)
                node.Meta = new Dictionary<string, object>();

            if (!node.Meta.ContainsKey(META_VC))
                LocateVC(node);

            if (node.Meta[META_VC] is IVCManager currentVC && currentTree != null)
            {
                string root = (string)node.Meta[META_ROOT];
                VCItemStatus status = currentVC.GetOverlay(node.BackingPath, root);
                node.Meta[META_STATUS] = status;
                OverlayMap.SetOverlay(status, node, currentTree);
                return status != VCItemStatus.Ignored;
            }
            return false;
        }

        void LocateVC(GenericNode node)
        {
            node.Meta[META_VC] = null;
            node.Meta[META_ROOT] = null;
            node.Meta[META_STATUS] = VCItemStatus.Unknown;

            GenericNode parent = node.Parent as GenericNode;
            if (parent?.Meta != null && parent.Meta.ContainsKey(META_VC))
            {
                node.Meta[META_VC] = parent.Meta[META_VC];
                node.Meta[META_ROOT] = parent.Meta[META_ROOT];
                return;
            }

            WatcherVCResult result = fsWatchers.ResolveVC(node.BackingPath);
            if (result != null)
            {
                node.Meta[META_VC] = result.Manager;
                node.Meta[META_ROOT] = result.Watcher.Path;
            }
        }
    }

    #region Overlay builder

    class OverlayMap: Dictionary<int, int>
    {
        static Image iconSkin;
        static readonly Dictionary<VCItemStatus, OverlayMap> maps = new Dictionary<VCItemStatus, OverlayMap>();

        public static void Init()
        {
            iconSkin = GetSkin();

            AddOverlay(VCItemStatus.UpToDate);
            AddOverlay(VCItemStatus.Modified);
            AddOverlay(VCItemStatus.Ignored);
            AddOverlay(VCItemStatus.Added);
            AddOverlay(VCItemStatus.Conflicted);
            AddOverlay(VCItemStatus.Deleted);
            AddOverlay(VCItemStatus.Replaced);
            AddOverlay(VCItemStatus.External);
            AddOverlay(VCItemStatus.Unknown);
        }

        private static Image GetSkin()
        {
            return PluginBase.MainForm.GetAutoAdjustedImage(ProjectWatcher.Skin); //can be changed by external SC-Plugin
        }

        public static void Reset()
        {
            foreach (OverlayMap map in maps.Values)
                map.Clear();
        }

        private static void AddOverlay(VCItemStatus status)
        {
            maps.Add(status, new OverlayMap());
        }

        public static void SetOverlay(VCItemStatus status, TreeNode node, TreeView tree)
        {
            if (!maps.ContainsKey(status)) return;
            OverlayMap map = maps[status];

            if (map.ContainsKey(node.ImageIndex))
            {
                node.SelectedImageIndex = node.ImageIndex = map[node.ImageIndex];
                return;
            }

            if (tree.ImageList.Images.Count <= node.ImageIndex)
                return;

            var original = tree.ImageList.Images[node.ImageIndex];
            var composed = original.Clone() as Bitmap;
            var curSize = ScaleHelper.GetScale() > 1.5 ? 32 : 16;
            using var destination = Graphics.FromImage(composed);
            destination.DrawImage(iconSkin, 
                new Rectangle(0, 0, composed.Width, composed.Height), 
                new Rectangle((int)status * curSize, 0, curSize, curSize), GraphicsUnit.Pixel);
            int index = tree.ImageList.Images.Count;
            tree.ImageList.Images.Add(composed);
            map[node.ImageIndex] = index;
            node.SelectedImageIndex = node.ImageIndex = index;
        }

        public OverlayMap()
        {
        }
    }

    #endregion

    #region Project Selection State

    public class ProjectSelectionState
    {
        public int Files;
        public int Dirs;
        public int Unknown;
        public int Ignored;
        public int Added;
        public int Revert;
        public int Diff;
        public int Conflict;
        public int Modified;
        public int Replaced;
        public int Other;
        public int Total;
        public IVCManager Manager;

        public ProjectSelectionState(MultiSelectTreeView tree)
        {
            if (tree is null || tree.SelectedNodes.Count == 0)
                return;

            foreach (TreeNode node in tree.SelectedNodes)
            {
                if (node is FileNode) Files++;
                else if (node is DirectoryNode) Dirs++;
                else return; // unknown node in selection - no action

                GenericNode gnode = (GenericNode)node;
                if (gnode.Meta is null || !gnode.Meta.ContainsKey(OverlayManager.META_STATUS)
                    || !gnode.Meta.ContainsKey(OverlayManager.META_VC))
                    return; // incomplete status

                if (Manager is null) Manager = gnode.Meta[OverlayManager.META_VC] as IVCManager;
                else if (gnode.Meta[OverlayManager.META_VC] != Manager)
                    return; // several managers...

                VCItemStatus status = (VCItemStatus)(gnode.Meta[OverlayManager.META_STATUS]);
                if (status == VCItemStatus.Unknown) Unknown++;
                else if (status == VCItemStatus.Ignored) Ignored++;
                else if (status == VCItemStatus.Added) Added++;
                else if (status == VCItemStatus.Conflicted) Conflict++;
                else if (status == VCItemStatus.Modified) { Modified++; Revert++; Diff++; }
                else if (status == VCItemStatus.Deleted) { Revert++; Diff++; }
                else if (status == VCItemStatus.Replaced) { Replaced++; Revert++; }
                else Other++;
                Total++;
            }
        }
    }

    #endregion
}
