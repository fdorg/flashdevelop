using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using ASCompletion.Context;
using PluginCore.Localization;
using PluginCore.Utilities;
using flash.tools.debugger;
using PluginCore;
using PluginCore.Helpers;

namespace FlashDebugger
{
    class StackframeUI : DockPanelControl
    {
        private ListView lv;
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
        private ToolStrip toolStripFilters;

        private ToolStripMenuItem copyContextMenuItem;
        private ToolStripMenuItem copyAllContextMenuItem;
        private ToolStripMenuItem setFrameContextMenuItem;
        private ToolStripMenuItem gotoSourceContextMenuItem;
        private ToolStripMenuItem justMyCodeContextMenuItem;

        private List<ListViewItem> wholeFrameStack;

        public StackframeUI(PluginMain pluginMain, ImageList imageList)
        {
            this.pluginMain = pluginMain;
            wholeFrameStack = new List<ListViewItem>();

            InitializeComponents(imageList);
            InitializeContextMenu();
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
            this.toolStripItemMatchCase.Text = "Match case";
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
            this.toolStripItemNegate.Text = "Match opposite";
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
            this.lv = new ListView();
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
                column.Width = ScaleHelper.Scale(column.Width);

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
            this.copyAllContextMenuItem.ShortcutKeyDisplayString = DataConverter.KeysToString(Keys.Control | Keys.Shift | Keys.C);
            this.setFrameContextMenuItem = new ToolStripMenuItem(TextHelper.GetString("Label.SetCurrentFrame"), null, new EventHandler(this.SetCurrentFrameClick));
            this.setFrameContextMenuItem.ShortcutKeyDisplayString = DataConverter.KeysToString(Keys.Enter);
            this.gotoSourceContextMenuItem = new ToolStripMenuItem(TextHelper.GetString("Label.GotoSource"), null, new EventHandler(this.GotoSourceClick));
            this.gotoSourceContextMenuItem.ShortcutKeyDisplayString = DataConverter.KeysToString(Keys.Control | Keys.Enter);
            this.justMyCodeContextMenuItem = new ToolStripMenuItem(TextHelper.GetString("Label.JustMyCode"), null, new EventHandler(this.JustMyCodeClick));
            menu.Items.AddRange(new ToolStripItem[] {this.copyContextMenuItem, this.copyAllContextMenuItem, new ToolStripSeparator(), 
                this.setFrameContextMenuItem, this.gotoSourceContextMenuItem, this.justMyCodeContextMenuItem});
            this.lv.ContextMenuStrip = menu;
            menu.Font = PluginBase.Settings.DefaultFont;
            menu.Renderer = new DockPanelStripRenderer(false);
            this.toolStripFilters.Renderer = new DockPanelStripRenderer();
            //menu.Opening += ContextMenuOpening;
        }

        public void ClearItem()
        {
            wholeFrameStack.Clear();
            lv.Items.Clear();
        }

        public void ActiveItem()
        {
            foreach (ListViewItem item in lv.Items)
            {
                if (item.ImageIndex == currentImageIndex)
                {
                    item.ImageIndex = -1;
                    break;
                }
            }
            lv.SelectedItems[0].ImageIndex = currentImageIndex;
        }

        public void AddFrames(Frame[] frames)
        {
            lv.BeginUpdate();
            lv.Items.Clear();
            wholeFrameStack.Clear();
            if (frames.GetLength(0) > 0)
            {
                IASContext context = ASContext.GetLanguageContext(PluginBase.CurrentProject.Language);
                foreach (var pathModel in context.Classpath)
                {
                    if (!pathModel.IsVirtual)
                    {
                        
                    }
                }
                foreach (Frame item in frames)
                {
                    String title = item.getCallSignature();
                    if (item.getLocation().getFile() != null) title += " at " + item.getLocation().getFile() + ":" + item.getLocation().getLine();
                    wholeFrameStack.Add(lv.Items.Add(new ListViewItem(new string[] {"", title}, -1) {Tag = item}));
                }
                lv.Items[0].ImageIndex = currentImageIndex;
            }
            lv.EndUpdate();
            toolStripTextBoxFilter.Enabled = lv.Items.Count > 0;
        }

        private void Lv_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                if (lv.SelectedIndices.Count > 0)
                {
                    PluginMain.debugManager.CurrentFrame = lv.SelectedIndices[0];
                    ActiveItem();
                }
            }
        }

        void Lv_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (lv.SelectedIndices.Count > 0)
            {
                if (PluginMain.debugManager.CurrentFrame == lv.SelectedIndices[0])
                {
                    Location tmp = PluginMain.debugManager.CurrentLocation;
                    PluginMain.debugManager.CurrentLocation = null;
                    PluginMain.debugManager.CurrentLocation = tmp;
                }
                else
                {
                    PluginMain.debugManager.CurrentFrame = lv.SelectedIndices[0];
                    ActiveItem();
                }
            }
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

        /// <summary>
        /// Filters the results...
        /// </summary>
        private void FilterResults()
        {
            lv.BeginUpdate();
            string filterText = toolStripTextBoxFilter.Text.Trim();
            lv.Items.Clear();

            Regex regex = null;

            if (toolStripItemRegEx.Checked)
            {
                try
                {
                    regex = new Regex(filterText, toolStripItemMatchCase.Checked ? RegexOptions.None
                                    : RegexOptions.IgnoreCase);
                }
                catch (Exception)
                {
                    return;
                }
            }

            foreach (var item in wholeFrameStack)
            {
                bool match = true;
                if (filterText != string.Empty)
                {
                    if (regex != null)
                    {
                        match = regex.IsMatch(item.SubItems[1].Text);
                    }
                    else
                    {
                        match = item.SubItems[1].Text.IndexOf(filterText,
                            toolStripItemMatchCase.Checked ? StringComparison.Ordinal
                                : StringComparison.OrdinalIgnoreCase) > -1;
                    }

                    if (toolStripItemNegate.Checked) match = !match;
                }

                if (match) lv.Items.Add(item);
            }
            
            lv.EndUpdate();
        }

        private void CopyTextClick(Object sender, EventArgs e)
        {
            
        }

        private void CopyAllTextClick(Object sender, EventArgs e)
        {
            
        }

        private void SetCurrentFrameClick(Object sender, EventArgs e)
        {
            
        }

        private void GotoSourceClick(Object sender, EventArgs e)
        {
            
        }

        private void JustMyCodeClick(Object sender, EventArgs e)
        {
            
        }

    }

}
