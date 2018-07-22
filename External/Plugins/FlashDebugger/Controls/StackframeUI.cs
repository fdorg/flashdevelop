using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using PluginCore.Localization;
using PluginCore.Utilities;
using ProjectManager.Projects;
using flash.tools.debugger;
using PluginCore;
using PluginCore.Helpers;
using PluginCore.Controls;
using System.Drawing;

namespace FlashDebugger
{
    class StackframeUI : DockPanelControl
    {
        private ListViewEx lv;
        private ColumnHeader imageColumnHeader;
        private ColumnHeader frameColumnHeader;
        private PluginMain pluginMain;
        private int currentImageIndex;
        private ToolStripLabel toolStripLabelFilter;
        private ToolStripSpringTextBox toolStripTextBoxFilter;
        private ToolStripButton clearFilterButton;
        private ToolStripDropDownButton toolStripDropDownOptions;
        private ToolStripMenuItem toolStripItemMatchCase;
        private ToolStripMenuItem toolStripItemRegEx;
        private ToolStripMenuItem toolStripItemNegate;
        private ToolStripEx toolStripFilters;
        private ToolStripMenuItem copyContextMenuItem;
        private ToolStripMenuItem copyAllContextMenuItem;
        private ToolStripMenuItem setFrameContextMenuItem;
        private ToolStripMenuItem gotoSourceContextMenuItem;
        private ToolStripMenuItem justMyCodeContextMenuItem;
        private List<ListViewItem> wholeFrameStack;
        private int lastSelected;
        private bool justMyCode;

        public StackframeUI(PluginMain pluginMain, ImageList imageList)
        {
            this.AutoKeyHandling = true;
            this.pluginMain = pluginMain;
            wholeFrameStack = new List<ListViewItem>();
            InitializeComponents(imageList);
            InitializeContextMenu();
            InitializeLocalization();
            ScrollBarEx.Attach(lv);
        }

        private void InitializeComponents(ImageList imageList)
        {
            this.toolStripLabelFilter = new System.Windows.Forms.ToolStripLabel();
            this.toolStripTextBoxFilter = new System.Windows.Forms.ToolStripSpringTextBox();
            this.clearFilterButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripDropDownOptions = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStripItemMatchCase = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripItemRegEx = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripItemNegate = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripFilters = new PluginCore.Controls.ToolStripEx();
            this.toolStripFilters.SuspendLayout();
            // 
            // toolStripTextBoxFilter
            //
            this.toolStripTextBoxFilter.Name = "toolStripTextBoxFilter";
            this.toolStripTextBoxFilter.Size = new System.Drawing.Size(100, 25);
            this.toolStripTextBoxFilter.Padding = new System.Windows.Forms.Padding(0, 0, 1, 0);
            this.toolStripTextBoxFilter.Enabled = false;
            this.toolStripTextBoxFilter.TextChanged += new System.EventHandler(this.ToolStripTextFieldFilter_Changed);
            // 
            // toolStripLabelFilter
            //
            this.toolStripLabelFilter.Margin = new System.Windows.Forms.Padding(2, 1, 0, 1);
            this.toolStripLabelFilter.Name = "toolStripLabelFilter";
            this.toolStripLabelFilter.Size = new System.Drawing.Size(36, 22);
            this.toolStripLabelFilter.Text = "Filter:";
            // 
            // clearFilterButton
            //
            this.clearFilterButton.Enabled = false;
            this.clearFilterButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.clearFilterButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.clearFilterButton.Margin = new System.Windows.Forms.Padding(0, 1, 0, 1);
            this.clearFilterButton.Name = "clearFilterButton";
            this.clearFilterButton.Size = new System.Drawing.Size(23, 26);
            this.clearFilterButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.clearFilterButton.Image = PluginBase.MainForm.FindImage("153");
            this.clearFilterButton.Click += new System.EventHandler(this.ClearFilterButton_Click);
            // 
            // toolStripItemMatchCase
            // 
            this.toolStripItemMatchCase.Name = "toolStripItemMatchCase";
            this.toolStripItemMatchCase.CheckOnClick = true;
            this.toolStripItemMatchCase.Text = "Match Case";
            this.toolStripItemNegate.Click += FilterOption_Click;
            // 
            // toolStripItemRegEx
            // 
            this.toolStripItemRegEx.Name = "toolStripItemRegEx";
            this.toolStripItemRegEx.CheckOnClick = true;
            this.toolStripItemRegEx.Text = "Regular Expression";
            this.toolStripItemNegate.Click += FilterOption_Click;
            // 
            // toolStripItemNegate
            // 
            this.toolStripItemNegate.Name = "toolStripItemNegate";
            this.toolStripItemNegate.CheckOnClick = true;
            this.toolStripItemNegate.Text = "Match Opposite";
            this.toolStripItemNegate.Click += FilterOption_Click;
            // 
            // toolStripDropDownOptions
            // 
            this.toolStripDropDownOptions.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownOptions.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripItemMatchCase,
            this.toolStripItemRegEx,
            this.toolStripItemNegate});
            this.toolStripDropDownOptions.Name = "toolStripDropDownOptions";
            this.toolStripDropDownOptions.Text = "Options";
            // 
            // toolStripFilters
            // 
            this.toolStripFilters.ImageScalingSize = ScaleHelper.Scale(new System.Drawing.Size(16, 16));
            this.toolStripFilters.CanOverflow = false;
            this.toolStripFilters.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.toolStripFilters.Padding = new System.Windows.Forms.Padding(1, 1, 2, 2);
            this.toolStripFilters.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStripFilters.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabelFilter,
            this.toolStripTextBoxFilter, 
            this.clearFilterButton,
            this.toolStripDropDownOptions});
            this.toolStripFilters.Name = "toolStripFilters";
            this.toolStripFilters.Location = new System.Drawing.Point(1, 0);
            this.toolStripFilters.Size = new System.Drawing.Size(710, 25);
            this.toolStripFilters.TabIndex = 0;
            this.toolStripFilters.Text = "toolStripFilters";
            // lv
            this.lv = new ListViewEx();
            this.lv.ShowItemToolTips = true;
            this.imageColumnHeader = new ColumnHeader();
            this.imageColumnHeader.Text = string.Empty;
            this.imageColumnHeader.Width = 20;
            this.frameColumnHeader = new ColumnHeader();
            this.frameColumnHeader.Text = string.Empty;
            this.lv.Columns.AddRange(new ColumnHeader[] {
            this.imageColumnHeader,
            this.frameColumnHeader});
            this.lv.FullRowSelect = true;
            this.lv.BorderStyle = BorderStyle.None;
            this.lv.Dock = System.Windows.Forms.DockStyle.Fill;
            foreach (ColumnHeader column in lv.Columns)
            {
                column.Width = ScaleHelper.Scale(column.Width);
            }
            lv.SmallImageList = imageList;
            currentImageIndex = imageList.Images.IndexOfKey("StartContinue");
            lv.View = System.Windows.Forms.View.Details;
            lv.MouseDoubleClick += new MouseEventHandler(Lv_MouseDoubleClick);
            lv.KeyDown += new KeyEventHandler(Lv_KeyDown);
            lv.SizeChanged += new EventHandler(Lv_SizeChanged);
            this.Controls.Add(lv);
            this.Controls.Add(toolStripFilters);
            this.toolStripFilters.ResumeLayout(false);
            this.toolStripFilters.PerformLayout();
        }

        public void InitializeContextMenu()
        {
            ContextMenuStrip menu = new ContextMenuStrip();
            this.copyContextMenuItem = new ToolStripMenuItem(TextHelper.GetString("Label.Copy"), null, new EventHandler(this.CopyTextClick));
            this.copyContextMenuItem.ShortcutKeyDisplayString = DataConverter.KeysToString(Keys.Control | Keys.C);
            this.copyAllContextMenuItem = new ToolStripMenuItem(TextHelper.GetString("Label.CopyAll"), null, new EventHandler(this.CopyAllTextClick));
            this.setFrameContextMenuItem = new ToolStripMenuItem(TextHelper.GetString("Label.SetCurrentFrame"), null, new EventHandler(this.SetCurrentFrameClick));
            this.setFrameContextMenuItem.ShortcutKeyDisplayString = DataConverter.KeysToString(Keys.Enter);
            this.gotoSourceContextMenuItem = new ToolStripMenuItem(TextHelper.GetString("Label.GotoSource"), null, new EventHandler(this.GotoSourceClick));
            this.gotoSourceContextMenuItem.ShortcutKeyDisplayString = DataConverter.KeysToString(Keys.Shift | Keys.Enter);
            this.justMyCodeContextMenuItem = new ToolStripMenuItem(TextHelper.GetString("Label.JustMyCode"), null, new EventHandler(this.JustMyCodeClick));
            this.justMyCodeContextMenuItem.CheckOnClick = true;
            menu.Items.AddRange(new ToolStripItem[] {this.copyContextMenuItem, this.copyAllContextMenuItem, new ToolStripSeparator(), this.setFrameContextMenuItem, this.gotoSourceContextMenuItem, this.justMyCodeContextMenuItem});
            this.lv.ContextMenuStrip = menu;
            menu.Font = PluginBase.Settings.DefaultFont;
            menu.Renderer = new DockPanelStripRenderer(false);
            this.toolStripFilters.Renderer = new DockPanelStripRenderer();
            menu.Opening += ContextMenuOpening;
        }

        private void InitializeLocalization()
        {
            this.toolStripLabelFilter.Text = TextHelper.GetString("Label.Filter");
            this.toolStripItemMatchCase.Text = TextHelper.GetString("Label.MatchCase");
            this.toolStripItemRegEx.Text = TextHelper.GetString("Label.RegularExpression");
            this.toolStripItemNegate.Text = TextHelper.GetString("Label.MatchOpposite");
            this.toolStripDropDownOptions.Text = TextHelper.GetString("Label.Options");
        }

        public void ClearItem()
        {
            wholeFrameStack.Clear();
            lv.Items.Clear();
            toolStripTextBoxFilter.Enabled = false;
        }

        public void ActiveItem()
        {
            lv.Items[lastSelected].ImageIndex = -1;
            int index = PluginMain.debugManager.CurrentFrame;
            var selectedItem = wholeFrameStack[index];
            if (justMyCode)
            {
                // We'll have to check for every item to make sure
                for (int i = 0, count = lv.Items.Count; i < count; i++)
                {
                    var itemData = (ListItemData)lv.Items[i].Tag;
                    if (itemData.Index < index) continue;
                    if (itemData.Index == index)
                    {
                        lv.Items[i].ImageIndex = currentImageIndex;
                        lastSelected = i;
                        break;
                    }
                    if (itemData.Index > index)
                    {
                        lv.Items[i - 1].ImageIndex = currentImageIndex;
                        lastSelected = i - 1;
                        break;
                    }
                    if (i == count - 1 && itemData.Index == -1)
                    {
                        lv.Items[i].ImageIndex = currentImageIndex;
                        lastSelected = i;
                        break;
                    }
                }
            }
            else
            {
                lastSelected = PluginMain.debugManager.CurrentFrame;
                selectedItem.ImageIndex = currentImageIndex;
            }
        }

        public void AddFrames(Frame[] frames)
        {
            wholeFrameStack.Clear();
            if (frames.GetLength(0) > 0)
            {
                Project project = PluginBase.CurrentProject as Project;
                int i = 0;
                foreach (Frame item in frames)
                {
                    String title = item.getCallSignature();
                    bool ownFile = false;
                    SourceFile sourceFile = item.getLocation().getFile();
                    if (sourceFile != null)
                    {
                        title += " at " + sourceFile + ":" + item.getLocation().getLine();
                        if (project != null)
                        {
                            foreach (string cp in project.AbsoluteClasspaths)
                            {
                                string pathBackSlash = cp.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                                pathBackSlash = pathBackSlash.Contains(Path.AltDirectorySeparatorChar.ToString()) ? pathBackSlash + Path.AltDirectorySeparatorChar : pathBackSlash + Path.DirectorySeparatorChar;
                                if (sourceFile.getFullPath().ToString().StartsWithOrdinal(pathBackSlash))
                                {
                                    ownFile = true;
                                    break;
                                }
                            }
                        }
                    }
                    var listItem = new ListViewItem(new[] { string.Empty, title }, -1)
                    {
                        Tag = new ListItemData { Frame = item, Index = i++ }
                    };
                    listItem.UseItemStyleForSubItems = false;
                    // Apply proper theming colour
                    if (!ownFile)
                    {
                        Color color = PluginBase.MainForm.GetThemeColor("ListView.ForeColor");
                        if (color == Color.Empty) color = System.Drawing.SystemColors.GrayText;
                        listItem.SubItems[1].ForeColor = color;
                    }
                    wholeFrameStack.Add(listItem);
                }
                FilterResults();
                toolStripTextBoxFilter.Enabled = true;
            }
            else
            {
                lv.Items.Clear();
                toolStripTextBoxFilter.Enabled = false;
            }
        }

        private void Lv_KeyDown(object sender, KeyEventArgs e)
        {
            if (lv.SelectedIndices.Count == 0) return;
            if (e.KeyCode == Keys.Return)
            {
                if ((e.Modifiers & Keys.Shift) > 0) GotoSourceClick(sender, e);
                else SetCurrentFrameClick(sender, e);
            }
            else if (e.KeyCode == Keys.C)
            {
                if ((e.Modifiers & Keys.Control) > 0) CopyTextClick(sender, e);
            }
        }

        void Lv_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (lv.SelectedIndices.Count > 0) SetCurrentFrameClick(sender, e);
        }

        private void Lv_SizeChanged(object sender, EventArgs e)
        {
            this.frameColumnHeader.Width = lv.Width - this.imageColumnHeader.Width;
        }

        /// <summary>
        /// Clears the filter control text
        /// </summary>
        private void ClearFilterButton_Click(Object sender, System.EventArgs e)
        {
            this.clearFilterButton.Enabled = false;
            this.toolStripTextBoxFilter.Clear();
        }

        /// <summary>
        /// Filter the result on check change
        /// </summary>
        private void ToolStripTextFieldFilter_Changed(Object sender, EventArgs e)
        {
            this.clearFilterButton.Enabled = this.toolStripTextBoxFilter.Text.Trim() != string.Empty;
            this.FilterResults();
        }

        private void FilterOption_Click(Object sender, EventArgs e)
        {
            if (clearFilterButton.Enabled) FilterResults();
        }

        private void ContextMenuOpening(Object sender, System.ComponentModel.CancelEventArgs e)
        {
            copyContextMenuItem.Enabled = setFrameContextMenuItem.Enabled = gotoSourceContextMenuItem.Enabled = lv.SelectedItems.Count > 0;
            copyAllContextMenuItem.Enabled = lv.Items.Count > 0;
        }

        private void CopyTextClick(Object sender, EventArgs e)
        {
            Clipboard.SetText(lv.SelectedItems[0].SubItems[1].Text);
        }

        private void CopyAllTextClick(Object sender, EventArgs e)
        {
            var sb = new StringBuilder();
            string filter = toolStripTextBoxFilter.Text.Trim();
            if (filter != string.Empty)
            {
                sb.Append(TextHelper.GetString("Label.Filter")).AppendLine(filter).AppendLine();
            }
            foreach (ListViewItem item in lv.Items)
            {
                sb.AppendLine(item.SubItems[1].Text);
            }
            Clipboard.SetText(sb.ToString());
        }

        private void SetCurrentFrameClick(Object sender, EventArgs e)
        {
            int index = ((ListItemData)lv.SelectedItems[0].Tag).Index;
            if (index == -1) return;
            if (PluginMain.debugManager.CurrentFrame == index)
            {
                Location tmp = PluginMain.debugManager.CurrentLocation;
                PluginMain.debugManager.CurrentLocation = null;
                PluginMain.debugManager.CurrentLocation = tmp;
            }
            else PluginMain.debugManager.CurrentFrame = index;
            ActiveItem();
        }

        private void GotoSourceClick(Object sender, EventArgs e)
        {
            var frame = ((ListItemData)lv.SelectedItems[0].Tag).Frame;
            if (frame == null) return;
            string file = PluginMain.debugManager.GetLocalPath(frame.getLocation().getFile());
            if (file == null) return;
            ScintillaHelper.ActivateDocument(file, frame.getLocation().getLine() - 1, false);
        }

        private void JustMyCodeClick(Object sender, EventArgs e)
        {
            justMyCode = justMyCodeContextMenuItem.Checked;
            FilterResults();
        }

        /// <summary>
        /// Filters the results...
        /// </summary>
        private void FilterResults()
        {
            lv.BeginUpdate();
            string filterText = toolStripTextBoxFilter.Text.Trim();
            lv.Items.Clear();
            Regex regex = null;
            Color color = PluginBase.MainForm.GetThemeColor("ToolStripTextBoxControl.GrayText", SystemColors.GrayText);
            if (toolStripItemRegEx.Checked)
            {
                try
                {
                    regex = new Regex(filterText, toolStripItemMatchCase.Checked ? RegexOptions.None : RegexOptions.IgnoreCase);
                }
                catch (Exception)
                {
                    lv.EndUpdate();
                    return;
                }
            }
            bool lastExternal = false;
            int currentFrame = PluginMain.debugManager.CurrentFrame;
            foreach (var item in wholeFrameStack)
            {
                bool match = true;
                item.ImageIndex = -1;
                if (filterText != string.Empty)
                {
                    if (regex != null) match = regex.IsMatch(item.SubItems[1].Text);
                    else
                    {
                        match = item.SubItems[1].Text.IndexOf(filterText, toolStripItemMatchCase.Checked ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase) > -1;
                    }
                    if (toolStripItemNegate.Checked) match = !match;
                }
                if (match)
                {
                    // Check proper theming colour
                    if (justMyCode && item.SubItems[1].ForeColor == color)
                    {
                        if (lastExternal)
                        {
                            if (((ListItemData) item.Tag).Index == currentFrame)
                            {
                                lv.Items[lv.Items.Count - 1].ImageIndex = currentImageIndex;
                                lastSelected = lv.Items.Count - 1;
                            }
                            continue;
                        }
                        var newItem = lv.Items.Add(new ListViewItem(new[] { string.Empty, "[External Code]" }, -1)
                        {
                            Tag = new ListItemData { Index = -1 }
                        });
                        newItem.UseItemStyleForSubItems = false;
                        // Apply proper theming colour
                        newItem.SubItems[1].ForeColor = color;
                        if (((ListItemData) item.Tag).Index == currentFrame)
                        {
                            newItem.ImageIndex = currentImageIndex;
                            lastSelected = lv.Items.Count - 1;
                        }
                        lastExternal = true;
                    }
                    else
                    {
                        lastExternal = false;
                        lv.Items.Add(item);
                        if (((ListItemData)item.Tag).Index == currentFrame)
                        {
                            item.ImageIndex = currentImageIndex;
                            lastSelected = lv.Items.Count - 1;
                        }
                    }
                }
            }
            wholeFrameStack[PluginMain.debugManager.CurrentFrame].ImageIndex = currentImageIndex;
            lv.EndUpdate();
        }

        private class ListItemData
        {
            public Frame Frame;
            public int Index;
        }

    }

}
