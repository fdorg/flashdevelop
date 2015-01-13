using System;
using System.Collections.Generic;
using ProjectManager.Controls.TreeView;
using SourceControl.Managers;
using System.Windows.Forms;
using SourceControl.Sources;
using PluginCore.Localization;
using PluginCore;

namespace SourceControl.Actions
{
    static class TreeContextMenuUpdate
    {
        private static ToolStripMenuItem scItem;

        static internal void SetMenu(ProjectTreeView tree, ProjectSelectionState state)
        {
            if (tree == null || state.Manager == null) return;
            
            IVCMenuItems menuItems = state.Manager.MenuItems;
            menuItems.CurrentNodes = (TreeNode[])tree.SelectedNodes.ToArray(typeof(TreeNode));
            menuItems.CurrentManager = state.Manager;

            AddSCMainItem(tree);
            scItem.DropDownItems.Clear();
            
            // let a VC provide a completely custom items list
            foreach (KeyValuePair<ToolStripItem, VCMenuItemProperties> item in menuItems.Items)
            {
                if (item.Value.Show.Invoke(state))
                {
                    scItem.DropDownItems.Add(item.Key);
                    if (item.Value.Enable != null)
                        item.Key.Enabled = item.Value.Enable.Invoke(state);
                }
            }

            // classical VC menu items
            if (menuItems != null)
            {
                List<ToolStripItem> items = new List<ToolStripItem>();

                // generic
                items.Add(menuItems.Update);
                items.Add(menuItems.Commit);
                items.Add(menuItems.Push);
                items.Add(menuItems.ShowLog);
                int minLen = items.Count;

                // specific
                if (state.Files == 2 && state.Total == 2) items.Add(menuItems.Diff);
                if (state.Conflict == 1 && state.Total == 1) items.Add(menuItems.EditConflict);

                if (state.Unknown + state.Ignored > 0 || state.Dirs > 0) items.Add(menuItems.Add);
                if (state.Unknown + state.Ignored == state.Total) items.Add(menuItems.Ignore);

                if (state.Unknown + state.Ignored < state.Total)
                {
                    if (state.Added > 0) items.Add(menuItems.UndoAdd);
                    else if (state.Revert > 0)
                    {
                        if (state.Diff > 0) items.Add(menuItems.DiffChange);
                        items.Add(menuItems.Revert);
                    }
                    else if (state.Total == 1) items.Add(menuItems.DiffChange);
                }
                if (items.Count > minLen) items.Insert(minLen, menuItems.MidSeparator);
                items.RemoveAll(item => item == null);
                scItem.DropDownItems.AddRange(items.ToArray());
            }
        }

        private static void AddSCMainItem(ProjectTreeView tree)
        {
            if (scItem == null)
            {
                scItem = new ToolStripMenuItem();
                scItem.Text = TextHelper.GetString("Label.SourceControl");
                scItem.Image = PluginBase.MainForm.FindImage("480");
            }
            // add in same group as Open/Execute/Shell menu...
            Boolean isProjectNode = tree.SelectedNodes.Count > 0 && tree.SelectedNodes[0].GetType().ToString().EndsWith("ProjectNode");
            Int32 index = GetNthSeparatorIndex(tree.ContextMenuStrip, isProjectNode ? 2 : 1);
            if (index >= 0) tree.ContextMenuStrip.Items.Insert(index, scItem);
            else tree.ContextMenuStrip.Items.Add(scItem);
        }

        private static Int32 GetNthSeparatorIndex(ContextMenuStrip menu, Int32 n)
        {
            Int32 index = -1;
            foreach (ToolStripItem item in menu.Items)
            {
                index++;
                if (item is ToolStripSeparator)
                    if (--n <= 0) return index;
            }
            return -1;
        }

    }

}
