using System.Collections.Generic;
using System.Windows.Forms;
using SourceControl.Managers;

namespace SourceControl.Sources
{
    public delegate void VCManagerStatusChange(IVCManager sender);

    /// <summary>
    /// Some version control solution manager
    /// </summary>
    public interface IVCManager
    {
        /// <summary>
        /// Notify that some versionned files' state changed (to update treeview)
        /// </summary>
        event VCManagerStatusChange OnChange;

        IVCMenuItems MenuItems { get; }
        IVCFileActions FileActions { get; }

        /// <summary>
        /// Return if the location is under VC 
        /// - if true, all the subtree will be considered under VC too.
        /// </summary>
        bool IsPathUnderVC(string path);

        /// <summary>
        /// Return a file/dir status
        /// </summary>
        VCItemStatus GetOverlay(string path, string rootPath);
        List<VCStatusReport> GetAllOverlays(string path, string rootPath);

        /// <summary>
        /// SC request for refreshing status of items
        /// - expected that OnChange is fired to notify when status has been updated
        /// </summary>
        void GetStatus(string rootPath);

        /// <summary>
        /// SC notification that IO changes happened in a location under VC 
        /// </summary>
        bool SetPathDirty(string path, string rootPath);
    }

    /// <summary>
    /// Expose context menu items
    /// 
    /// An item should handle the Click action and use the provided Nodes & Manager references 
    /// to process the item action.
    /// 
    /// Items can be provided by setting some/all the classic menu items (display rules hardcoded)
    /// OR provide a completely custom list of VCMenuItemProperties to provide (item + display rule).
    /// </summary>
    public interface IVCMenuItems
    {
        /// <summary>
        /// Set by SC plugin to provide the selected files/dirs
        /// </summary>
        TreeNode[] CurrentNodes { set; }

        /// <summary>
        /// Set by SC plugin to provide the manager instance
        /// </summary>
        IVCManager CurrentManager { set; }

        /* classic VC menu items - return null to disable an item */
        ToolStripItem Update { get; }
        ToolStripItem Commit { get; }
        ToolStripItem Push { get; }
        ToolStripItem ShowLog { get; }
        ToolStripItem MidSeparator { get; }
        ToolStripItem Diff { get; }
        ToolStripItem DiffChange { get; }
        ToolStripItem Add { get; }
        ToolStripItem Ignore { get; }
        ToolStripItem UndoAdd { get; }
        ToolStripItem Revert { get; }
        ToolStripItem EditConflict { get; }

        /* OR completely custom items list */
        Dictionary<ToolStripItem, VCMenuItemProperties> Items { get; }
    }

    public delegate bool ShowVCMenuItemDelegate(ProjectSelectionState state);
    public struct VCMenuItemProperties
    {
        public ShowVCMenuItemDelegate Show;
        public ShowVCMenuItemDelegate Enable;
    }

    /// <summary>
    /// Let a manager handle or prevent file/dir operations.
    /// Return false to handle the action.
    /// </summary>
    public interface IVCFileActions
    {
        bool FileNew(string path);
        bool FileOpen(string path);
        bool FileReload(string path);
        bool FileModifyRO(string path);

        bool FileBeforeRename(string path);
        bool FileRename(string path, string newName);
        bool FileDelete(string[] paths, bool confirm);
        bool FileMove(string fromPath, string toPath);

        bool BuildProject();
        bool TestProject();
        bool SaveProject();
    }

    /// <summary>
    /// Struct to provide the status of a file/dir element
    /// </summary>
    public class VCStatusReport
    {
        public string Path;
        public VCItemStatus Status;

        public VCStatusReport(string path, VCItemStatus status)
        {
            Path = path;
            Status = status;
        }
    }

    /// <summary>
    /// File/dir element possible status states
    /// </summary>
    public enum VCItemStatus : int
    {
        Undefined = -1,
        Unknown = 0,
        External = 1,
        Ignored = 2,
        UpToDate = 3,
        Added = 4,
        Deleted = 5,
        Replaced = 6,
        Missing = 7,
        Modified = 8,
        Conflicted = 9
    }
}
