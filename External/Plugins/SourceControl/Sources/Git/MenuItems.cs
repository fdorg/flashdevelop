using System;
using System.Collections.Generic;
using System.Windows.Forms;
using PluginCore;
using PluginCore.Localization;
using ProjectManager.Controls.TreeView;
using ProjectManager.Helpers;

namespace SourceControl.Sources.Git
{
    class MenuItems : IVCMenuItems
    {
        TreeNode[] currentNodes;
        IVCManager currentManager;

        public TreeNode[] CurrentNodes { set => currentNodes = value; }
        public IVCManager CurrentManager { set => currentManager = value; }

        public ToolStripItem Update { get; }

        public ToolStripItem Commit { get; }

        public ToolStripItem Push { get; }

        public ToolStripItem ShowLog { get; }

        public ToolStripItem MidSeparator { get; }

        public ToolStripItem Annotate { get; }

        public ToolStripItem Diff { get; }

        public ToolStripItem DiffChange { get; }

        public ToolStripItem Add { get; }

        public ToolStripItem Ignore { get; }

        public ToolStripItem UndoAdd { get; }

        public ToolStripItem Revert { get; }

        public ToolStripItem EditConflict { get; }

        public Dictionary<ToolStripItem, VCMenuItemProperties> Items { get; } = new Dictionary<ToolStripItem, VCMenuItemProperties>();

        public MenuItems()
        {
            Update = new ToolStripMenuItem(TextHelper.GetString("Label.Pull"), PluginBase.MainForm.FindImage("159|1|-3|3"), Update_Click);
            Commit = new ToolStripMenuItem(TextHelper.GetString("Label.Commit"), PluginBase.MainForm.FindImage("62"), Commit_Click);
            Push = new ToolStripMenuItem(TextHelper.GetString("Label.Push"), PluginBase.MainForm.FindImage("159|9|-3|3"), Push_Click);
            ShowLog = new ToolStripMenuItem(TextHelper.GetString("Label.ShowLog"), PluginBase.MainForm.FindImage("95"), ShowLog_Click);
            MidSeparator = new ToolStripSeparator();
            Annotate = new ToolStripMenuItem("Annotate", PluginBase.MainForm.FindImage("45"), Annotate_Click);
            Diff = new ToolStripMenuItem(TextHelper.GetString("Label.Diff"), PluginBase.MainForm.FindImage("251"), Diff_Click);
            DiffChange = new ToolStripMenuItem(TextHelper.GetString("Label.DiffWithPrevious"), PluginBase.MainForm.FindImage("251"), DiffChange_Click);
            Add = new ToolStripMenuItem(TextHelper.GetString("Label.Add"), PluginBase.MainForm.FindImage("33"), Add_Click);
            Ignore = new ToolStripMenuItem(TextHelper.GetString("Label.AddToIgnoreList"), PluginBase.MainForm.FindImage("166"), Ignore_Click);
            UndoAdd = new ToolStripMenuItem(TextHelper.GetString("Label.UndoAdd"), PluginBase.MainForm.FindImage("73"), UndoAdd_Click);
            Revert = new ToolStripMenuItem(TextHelper.GetString("Label.Revert"), PluginBase.MainForm.FindImage("73"), Revert_Click);
            EditConflict = new ToolStripMenuItem(TextHelper.GetString("Label.EditConflict"), PluginBase.MainForm.FindImage("196"), EditConflict_Click);
        }

        private string GetPaths() => string.Join("*", GetPathsArray());

        private string[] GetPathsArray()
        {
            var paths = new List<string>();
            if (currentNodes is null) return paths.ToArray();
            foreach (var node in currentNodes)
                if (node is GenericNode treeNode) paths.Add(treeNode.BackingPath);
            return paths.ToArray();
        }

        void EditConflict_Click(object sender, EventArgs e) => TortoiseProc.Execute("conflicteditor", GetPaths());

        void Revert_Click(object sender, EventArgs e) => TortoiseProc.Execute("revert", GetPaths());

        void UndoAdd_Click(object sender, EventArgs e) => new ResetCommand(GetPathsArray());

        void Add_Click(object sender, EventArgs e) => TortoiseProc.Execute("add", GetPaths());

        void Ignore_Click(object sender, EventArgs e) => TortoiseProc.Execute("ignore", GetPaths());

        void Annotate_Click(object sender, EventArgs e)
        {
            if (currentNodes != null)
            {
                new BlameCommand(((GenericNode) currentNodes[0]).BackingPath);
            }
        }

        void DiffChange_Click(object sender, EventArgs e) => TortoiseProc.Execute("diff", GetPaths());

        void Diff_Click(object sender, EventArgs e)
        {
            if (currentNodes is null || currentNodes.Length != 2) return;
            string path1 = ((GenericNode) currentNodes[0]).BackingPath;
            string path2 = ((GenericNode) currentNodes[1]).BackingPath;
            TortoiseProc.Execute("diff", path1, path2);
        }

        void ShowLog_Click(object sender, EventArgs e) => TortoiseProc.Execute("log", GetPaths());

        void Push_Click(object sender, EventArgs e) => TortoiseProc.Execute("push", GetPaths());

        void Commit_Click(object sender, EventArgs e)
        {
            var title = TextHelper.GetString("Label.Commit");
            var msg = TextHelper.GetString("Info.EnterMessage");
            using var dialog = new LineEntryDialog(title, msg, "");
            if (dialog.ShowDialog() != DialogResult.OK || dialog.Line == "") return;
            new CommitCommand(GetPathsArray(), dialog.Line);
        }

        void Update_Click(object sender, EventArgs e) => TortoiseProc.Execute("pull", GetPaths());
    }
}
