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
    internal class StackframeUI : DockPanelControl
    {
        ListViewEx lv;
        ColumnHeader imageColumnHeader;
        ColumnHeader frameColumnHeader;
        PluginMain pluginMain;
        int currentImageIndex;
        ToolStripLabel toolStripLabelFilter;
        ToolStripSpringTextBox toolStripTextBoxFilter;
        ToolStripButton clearFilterButton;
        ToolStripDropDownButton toolStripDropDownOptions;
        ToolStripMenuItem toolStripItemMatchCase;
        ToolStripMenuItem toolStripItemRegEx;
        ToolStripMenuItem toolStripItemNegate;
        ToolStripEx toolStripFilters;
        ToolStripMenuItem copyContextMenuItem;
        ToolStripMenuItem copyAllContextMenuItem;
        ToolStripMenuItem setFrameContextMenuItem;
        ToolStripMenuItem gotoSourceContextMenuItem;
        ToolStripMenuItem justMyCodeContextMenuItem;
        readonly List<ListViewItem> wholeFrameStack;
        int lastSelected;
        bool justMyCode;

        public StackframeUI(PluginMain pluginMain, ImageList imageList)
        {
            AutoKeyHandling = true;
            this.pluginMain = pluginMain;
            wholeFrameStack = new List<ListViewItem>();
            InitializeComponents(imageList);
            InitializeContextMenu();
            InitializeLocalization();
            ScrollBarEx.Attach(lv);
        }

        void InitializeComponents(ImageList imageList)
        {
            toolStripLabelFilter = new ToolStripLabel();
            toolStripTextBoxFilter = new ToolStripSpringTextBox();
            clearFilterButton = new ToolStripButton();
            toolStripDropDownOptions = new ToolStripDropDownButton();
            toolStripItemMatchCase = new ToolStripMenuItem();
            toolStripItemRegEx = new ToolStripMenuItem();
            toolStripItemNegate = new ToolStripMenuItem();
            toolStripFilters = new ToolStripEx();
            toolStripFilters.SuspendLayout();
            // 
            // toolStripTextBoxFilter
            //
            toolStripTextBoxFilter.Name = "toolStripTextBoxFilter";
            toolStripTextBoxFilter.Size = new Size(100, 25);
            toolStripTextBoxFilter.Padding = new Padding(0, 0, 1, 0);
            toolStripTextBoxFilter.Enabled = false;
            toolStripTextBoxFilter.TextChanged += ToolStripTextFieldFilter_Changed;
            // 
            // toolStripLabelFilter
            //
            toolStripLabelFilter.Margin = new Padding(2, 1, 0, 1);
            toolStripLabelFilter.Name = "toolStripLabelFilter";
            toolStripLabelFilter.Size = new Size(36, 22);
            toolStripLabelFilter.Text = "Filter:";
            // 
            // clearFilterButton
            //
            clearFilterButton.Enabled = false;
            clearFilterButton.DisplayStyle = ToolStripItemDisplayStyle.Image;
            clearFilterButton.ImageTransparentColor = Color.Magenta;
            clearFilterButton.Margin = new Padding(0, 1, 0, 1);
            clearFilterButton.Name = "clearFilterButton";
            clearFilterButton.Size = new Size(23, 26);
            clearFilterButton.Alignment = ToolStripItemAlignment.Right;
            clearFilterButton.Image = PluginBase.MainForm.FindImage("153");
            clearFilterButton.Click += ClearFilterButton_Click;
            // 
            // toolStripItemMatchCase
            // 
            toolStripItemMatchCase.Name = "toolStripItemMatchCase";
            toolStripItemMatchCase.CheckOnClick = true;
            toolStripItemMatchCase.Text = "Match Case";
            toolStripItemNegate.Click += FilterOption_Click;
            // 
            // toolStripItemRegEx
            // 
            toolStripItemRegEx.Name = "toolStripItemRegEx";
            toolStripItemRegEx.CheckOnClick = true;
            toolStripItemRegEx.Text = "Regular Expression";
            toolStripItemNegate.Click += FilterOption_Click;
            // 
            // toolStripItemNegate
            // 
            toolStripItemNegate.Name = "toolStripItemNegate";
            toolStripItemNegate.CheckOnClick = true;
            toolStripItemNegate.Text = "Match Opposite";
            toolStripItemNegate.Click += FilterOption_Click;
            // 
            // toolStripDropDownOptions
            // 
            toolStripDropDownOptions.DisplayStyle = ToolStripItemDisplayStyle.Text;
            toolStripDropDownOptions.DropDownItems.AddRange(new ToolStripItem[] {
            toolStripItemMatchCase,
            toolStripItemRegEx,
            toolStripItemNegate});
            toolStripDropDownOptions.Name = "toolStripDropDownOptions";
            toolStripDropDownOptions.Text = "Options";
            // 
            // toolStripFilters
            // 
            toolStripFilters.ImageScalingSize = ScaleHelper.Scale(new Size(16, 16));
            toolStripFilters.CanOverflow = false;
            toolStripFilters.LayoutStyle = ToolStripLayoutStyle.HorizontalStackWithOverflow;
            toolStripFilters.Padding = new Padding(1, 1, 2, 2);
            toolStripFilters.GripStyle = ToolStripGripStyle.Hidden;
            toolStripFilters.Items.AddRange(new ToolStripItem[] {
            toolStripLabelFilter,
            toolStripTextBoxFilter, 
            clearFilterButton,
            toolStripDropDownOptions});
            toolStripFilters.Name = "toolStripFilters";
            toolStripFilters.Location = new Point(1, 0);
            toolStripFilters.Size = new Size(710, 25);
            toolStripFilters.TabIndex = 0;
            toolStripFilters.Text = "toolStripFilters";
            // lv
            lv = new ListViewEx();
            lv.ShowItemToolTips = true;
            imageColumnHeader = new ColumnHeader();
            imageColumnHeader.Text = string.Empty;
            imageColumnHeader.Width = 20;
            frameColumnHeader = new ColumnHeader();
            frameColumnHeader.Text = string.Empty;
            lv.Columns.AddRange(new[] {
            imageColumnHeader,
            frameColumnHeader});
            lv.FullRowSelect = true;
            lv.BorderStyle = BorderStyle.None;
            lv.Dock = DockStyle.Fill;
            foreach (ColumnHeader column in lv.Columns)
            {
                column.Width = ScaleHelper.Scale(column.Width);
            }
            lv.SmallImageList = imageList;
            currentImageIndex = imageList.Images.IndexOfKey("StartContinue");
            lv.View = View.Details;
            lv.MouseDoubleClick += Lv_MouseDoubleClick;
            lv.KeyDown += Lv_KeyDown;
            lv.SizeChanged += Lv_SizeChanged;
            Controls.Add(lv);
            Controls.Add(toolStripFilters);
            toolStripFilters.ResumeLayout(false);
            toolStripFilters.PerformLayout();
        }

        public void InitializeContextMenu()
        {
            ContextMenuStrip menu = new ContextMenuStrip();
            copyContextMenuItem = new ToolStripMenuItem(TextHelper.GetString("Label.Copy"), null, CopyTextClick);
            copyContextMenuItem.ShortcutKeyDisplayString = DataConverter.KeysToString(Keys.Control | Keys.C);
            copyAllContextMenuItem = new ToolStripMenuItem(TextHelper.GetString("Label.CopyAll"), null, CopyAllTextClick);
            setFrameContextMenuItem = new ToolStripMenuItem(TextHelper.GetString("Label.SetCurrentFrame"), null, SetCurrentFrameClick);
            setFrameContextMenuItem.ShortcutKeyDisplayString = DataConverter.KeysToString(Keys.Enter);
            gotoSourceContextMenuItem = new ToolStripMenuItem(TextHelper.GetString("Label.GotoSource"), null, GotoSourceClick);
            gotoSourceContextMenuItem.ShortcutKeyDisplayString = DataConverter.KeysToString(Keys.Shift | Keys.Enter);
            justMyCodeContextMenuItem = new ToolStripMenuItem(TextHelper.GetString("Label.JustMyCode"), null, JustMyCodeClick);
            justMyCodeContextMenuItem.CheckOnClick = true;
            menu.Items.AddRange(new ToolStripItem[] {copyContextMenuItem, copyAllContextMenuItem, new ToolStripSeparator(), setFrameContextMenuItem, gotoSourceContextMenuItem, justMyCodeContextMenuItem});
            lv.ContextMenuStrip = menu;
            menu.Font = PluginBase.Settings.DefaultFont;
            menu.Renderer = new DockPanelStripRenderer(false);
            toolStripFilters.Renderer = new DockPanelStripRenderer();
            menu.Opening += ContextMenuOpening;
        }

        void InitializeLocalization()
        {
            toolStripLabelFilter.Text = TextHelper.GetString("Label.Filter");
            toolStripItemMatchCase.Text = TextHelper.GetString("Label.MatchCase");
            toolStripItemRegEx.Text = TextHelper.GetString("Label.RegularExpression");
            toolStripItemNegate.Text = TextHelper.GetString("Label.MatchOpposite");
            toolStripDropDownOptions.Text = TextHelper.GetString("Label.Options");
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
                    string title = item.getCallSignature();
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
                        if (color == Color.Empty) color = SystemColors.GrayText;
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

        void Lv_KeyDown(object sender, KeyEventArgs e)
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

        void Lv_SizeChanged(object sender, EventArgs e)
        {
            frameColumnHeader.Width = lv.Width - imageColumnHeader.Width;
        }

        /// <summary>
        /// Clears the filter control text
        /// </summary>
        void ClearFilterButton_Click(object sender, EventArgs e)
        {
            clearFilterButton.Enabled = false;
            toolStripTextBoxFilter.Clear();
        }

        /// <summary>
        /// Filter the result on check change
        /// </summary>
        void ToolStripTextFieldFilter_Changed(object sender, EventArgs e)
        {
            clearFilterButton.Enabled = toolStripTextBoxFilter.Text.Trim().Length > 0;
            FilterResults();
        }

        void FilterOption_Click(object sender, EventArgs e)
        {
            if (clearFilterButton.Enabled) FilterResults();
        }

        void ContextMenuOpening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            copyContextMenuItem.Enabled = setFrameContextMenuItem.Enabled = gotoSourceContextMenuItem.Enabled = lv.SelectedItems.Count > 0;
            copyAllContextMenuItem.Enabled = lv.Items.Count > 0;
        }

        void CopyTextClick(object sender, EventArgs e)
        {
            Clipboard.SetText(lv.SelectedItems[0].SubItems[1].Text);
        }

        void CopyAllTextClick(object sender, EventArgs e)
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

        void SetCurrentFrameClick(object sender, EventArgs e)
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

        void GotoSourceClick(object sender, EventArgs e)
        {
            var frame = ((ListItemData)lv.SelectedItems[0].Tag).Frame;
            if (frame is null) return;
            string file = PluginMain.debugManager.GetLocalPath(frame.getLocation().getFile());
            if (file is null) return;
            ScintillaHelper.ActivateDocument(file, frame.getLocation().getLine() - 1, false);
        }

        void JustMyCodeClick(object sender, EventArgs e)
        {
            justMyCode = justMyCodeContextMenuItem.Checked;
            FilterResults();
        }

        /// <summary>
        /// Filters the results...
        /// </summary>
        void FilterResults()
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

        class ListItemData
        {
            public Frame Frame;
            public int Index;
        }

    }

}
