// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using PluginCore;
using PluginCore.Localization;
using ProjectManager.Controls.TreeView;

namespace SourceControl.Sources.Subversion
{
    internal class MenuItems : IVCMenuItems
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
            Update = new ToolStripMenuItem(TextHelper.GetString("Label.Update"), PluginBase.MainForm.FindImage("159|1|-3|3"), Update_Click);
            Commit = new ToolStripMenuItem(TextHelper.GetString("Label.Commit"), PluginBase.MainForm.FindImage("62"), Commit_Click);
            Push = null;
            ShowLog = new ToolStripMenuItem(TextHelper.GetString("Label.ShowLog"), PluginBase.MainForm.FindImage("95"), ShowLog_Click);
            MidSeparator = new ToolStripSeparator();
            Annotate = new ToolStripMenuItem("Annotate", PluginBase.MainForm.FindImage("45")) { Enabled = false };
            Diff = new ToolStripMenuItem(TextHelper.GetString("Label.Diff"), PluginBase.MainForm.FindImage("251"), Diff_Click);
            DiffChange = new ToolStripMenuItem(TextHelper.GetString("Label.DiffWithPrevious"), PluginBase.MainForm.FindImage("251"), DiffChange_Click);
            Add = new ToolStripMenuItem(TextHelper.GetString("Label.Add"), PluginBase.MainForm.FindImage("33"), Add_Click);
            Ignore = new ToolStripMenuItem(TextHelper.GetString("Label.AddToIgnoreList"), PluginBase.MainForm.FindImage("166"), Ignore_Click);
            UndoAdd = new ToolStripMenuItem(TextHelper.GetString("Label.UndoAdd"), PluginBase.MainForm.FindImage("73"), UndoAdd_Click);
            Revert = new ToolStripMenuItem(TextHelper.GetString("Label.Revert"), PluginBase.MainForm.FindImage("73"), Revert_Click);
            EditConflict = new ToolStripMenuItem(TextHelper.GetString("Label.EditConflict"), PluginBase.MainForm.FindImage("196"), EditConflict_Click);
        }

        string GetPaths()
        {
            var paths = new List<string>();
            if (currentNodes != null)
                foreach (var node in currentNodes)
                {
                    if (node is GenericNode genericNode)
                        paths.Add(genericNode.BackingPath);
                }
            return string.Join("*", paths);
        }

        void EditConflict_Click(object sender, EventArgs e) => TortoiseProc.Execute("conflicteditor", GetPaths());

        void Revert_Click(object sender, EventArgs e) => TortoiseProc.Execute("revert", GetPaths());

        void UndoAdd_Click(object sender, EventArgs e) => TortoiseProc.Execute("revert", GetPaths());

        void Add_Click(object sender, EventArgs e) => TortoiseProc.Execute("add", GetPaths());

        void Ignore_Click(object sender, EventArgs e) => TortoiseProc.Execute("ignore", GetPaths());

        void DiffChange_Click(object sender, EventArgs e) => TortoiseProc.Execute("diff", GetPaths());

        void Diff_Click(object sender, EventArgs e)
        {
            if (currentNodes is null || currentNodes.Length != 2) return;
            var path1 = ((GenericNode) currentNodes[0]).BackingPath;
            var path2 = ((GenericNode) currentNodes[1]).BackingPath;
            TortoiseProc.Execute("diff", path1, path2);
        }

        void ShowLog_Click(object sender, EventArgs e) => TortoiseProc.Execute("log", GetPaths());

        void Commit_Click(object sender, EventArgs e) => TortoiseProc.Execute("commit", GetPaths());

        void Update_Click(object sender, EventArgs e) => TortoiseProc.Execute("update", GetPaths());
    }
}
