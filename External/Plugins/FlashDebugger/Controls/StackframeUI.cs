using System;
using System.Windows.Forms;
using flash.tools.debugger;
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
        private ToolStrip toolStripFilters;

        public StackframeUI(PluginMain pluginMain, ImageList imageList)
        {
            this.pluginMain = pluginMain;

            this.toolStripLabelFilter = new System.Windows.Forms.ToolStripLabel();
            this.toolStripTextBoxFilter = new System.Windows.Forms.ToolStripSpringTextBox();
            this.clearFilterButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripFilters = new PluginCore.Controls.ToolStripEx();
            this.toolStripFilters.SuspendLayout();

            // 
            // toolStripTextBoxFilter
            //
            this.toolStripTextBoxFilter.Name = "toolStripTextBoxFilter";
            this.toolStripTextBoxFilter.Size = new System.Drawing.Size(100, 25);
            this.toolStripTextBoxFilter.Padding = new System.Windows.Forms.Padding(0, 0, 1, 0);
            this.toolStripTextBoxFilter.TextChanged += new System.EventHandler(this.ToolStripButtonErrorCheckedChanged);
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
            this.clearFilterButton.Click += new System.EventHandler(this.ClearFilterButtonClick);

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
            this.clearFilterButton});
            this.toolStripFilters.Name = "toolStripFilters";
            this.toolStripFilters.Location = new System.Drawing.Point(1, 0);
            this.toolStripFilters.Size = new System.Drawing.Size(710, 25);
            this.toolStripFilters.TabIndex = 0;
            this.toolStripFilters.Text = "toolStripFilters";

            lv = new ListView();
            lv.ShowItemToolTips = true;
            this.imageColumnHeader = new ColumnHeader();
            this.imageColumnHeader.Text = string.Empty;
            this.imageColumnHeader.Width = 20;

            this.frameColumnHeader = new ColumnHeader();
            this.frameColumnHeader.Text = string.Empty;

            lv.Columns.AddRange(new ColumnHeader[] {
            this.imageColumnHeader,
            this.frameColumnHeader});
            lv.FullRowSelect = true;
            lv.BorderStyle = BorderStyle.None;
            lv.Dock = System.Windows.Forms.DockStyle.Fill;

            foreach (ColumnHeader column in lv.Columns)
                column.Width = ScaleHelper.Scale(column.Width);

            lv.SmallImageList = imageList;
            currentImageIndex = imageList.Images.IndexOfKey("StartContinue");

            lv.View = System.Windows.Forms.View.Details;
            lv.MouseDoubleClick += new MouseEventHandler(lv_MouseDoubleClick);
            lv.KeyDown += new KeyEventHandler(lv_KeyDown);
            lv.SizeChanged += new EventHandler(lv_SizeChanged);

            this.Controls.Add(lv);
            this.Controls.Add(toolStripFilters);
            this.toolStripFilters.ResumeLayout(false);
            this.toolStripFilters.PerformLayout();
        }

        void lv_SizeChanged(object sender, EventArgs e)
        {
            this.frameColumnHeader.Width = lv.Width - this.imageColumnHeader.Width;
        }

        public void ClearItem()
        {
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
            lv.Items.Clear();
            if (frames.GetLength(0) > 0)
            {
                foreach (Frame item in frames)
                {
                    String title = item.getCallSignature();
                    if (item.getLocation().getFile() != null) title += " at " + item.getLocation().getFile() + ":" + item.getLocation().getLine();
                    lv.Items.Add(new ListViewItem(new string[] {"", title}, -1));
                }
                lv.Items[0].ImageIndex = currentImageIndex;
            }
        }

        void lv_KeyDown(object sender, KeyEventArgs e)
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

        void lv_MouseDoubleClick(object sender, MouseEventArgs e)
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

        /// <summary>
        /// Clears the filter control text
        /// </summary>
        private void ClearFilterButtonClick(Object sender, System.EventArgs e)
        {
            this.clearFilterButton.Enabled = false;
            this.toolStripTextBoxFilter.Clear();
        }

        /// <summary>
        /// Filter the result on check change
        /// </summary>
        private void ToolStripButtonErrorCheckedChanged(Object sender, EventArgs e)
        {
            this.clearFilterButton.Enabled = this.toolStripTextBoxFilter.Text.Trim().Length > 0;
            this.FilterResults();
        }

        /// <summary>
        /// Filters the results...
        /// </summary>
        private void FilterResults()
        {
            lv.BeginUpdate();
            string filterText = toolStripTextBoxFilter.Text.ToLower();
            lv.Items.Clear();
            lv.EndUpdate();
        }

    }

}
