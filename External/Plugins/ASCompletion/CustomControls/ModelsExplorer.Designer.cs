namespace ASCompletion
{
    partial class ModelsExplorer
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.outlineTreeView = new System.Windows.Forms.FixedTreeView();
            this.outlineContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.exploreToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.convertToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.updateTimer = new System.Windows.Forms.Timer(this.components);
            this.toolStrip = new PluginCore.Controls.ToolStripEx();
            this.filterLabel = new System.Windows.Forms.ToolStripLabel();
            this.filterTextBox = new System.Windows.Forms.ToolStripSpringTextBox();
            this.refreshButton = new System.Windows.Forms.ToolStripButton();
            this.rebuildButton = new System.Windows.Forms.ToolStripButton();
            this.searchButton = new System.Windows.Forms.ToolStripButton();
            this.folderBrowserDialog = new Ookii.Dialogs.VistaFolderBrowserDialog();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.outlineContextMenuStrip.SuspendLayout();
            this.toolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // outlineTreeView
            // 
            this.outlineTreeView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.outlineTreeView.ContextMenuStrip = this.outlineContextMenuStrip;
            this.outlineTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.outlineTreeView.Name = "outlineTreeView";
            this.outlineTreeView.Size = new System.Drawing.Size(383, 334);
            this.outlineTreeView.TabIndex = 1;
            this.outlineTreeView.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.outlineTreeView_BeforeExpand);
            this.outlineTreeView.DoubleClick += new System.EventHandler(this.outlineTreeView_Click);
            // 
            // outlineContextMenuStrip
            // 
            this.outlineContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exploreToolStripMenuItem,
            this.editToolStripMenuItem,
            this.convertToolStripMenuItem});
            this.outlineContextMenuStrip.Name = "outlineContextMenuStrip";
            this.outlineContextMenuStrip.Size = new System.Drawing.Size(181, 70);
            this.outlineContextMenuStrip.Opening += new System.ComponentModel.CancelEventHandler(this.outlineContextMenuStrip_Opening);
            // 
            // exploreToolStripMenuItem
            // 
            this.exploreToolStripMenuItem.Name = "exploreToolStripMenuItem";
            this.exploreToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.exploreToolStripMenuItem.Text = "Explore";
            this.exploreToolStripMenuItem.Click += new System.EventHandler(this.ExploreToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.editToolStripMenuItem.Text = "Edit";
            this.editToolStripMenuItem.Click += new System.EventHandler(this.EditToolStripMenuItem_Click);
            // 
            // convertToolStripMenuItem
            // 
            this.convertToolStripMenuItem.Name = "convertToolStripMenuItem";
            this.convertToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.convertToolStripMenuItem.Text = "Convert To Intrinsic";
            this.convertToolStripMenuItem.Click += new System.EventHandler(this.ConvertToolStripMenuItem_Click);
            // 
            // updateTimer
            // 
            this.updateTimer.Tick += new System.EventHandler(this.updateTimer_Tick);
            // 
            // toolStrip
            // 
            this.toolStrip.CanOverflow = false;
            this.toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.filterLabel,
            this.filterTextBox,
            this.refreshButton,
            this.rebuildButton,
            this.searchButton});
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Padding = new System.Windows.Forms.Padding(2, 1, 2, 2);
            this.toolStrip.Size = new System.Drawing.Size(383, 25);
            this.toolStrip.Dock = System.Windows.Forms.DockStyle.Top;
            this.toolStrip.TabIndex = 2;
            this.toolStrip.Text = "toolStrip";
            // 
            // filterLabel
            // 
            this.filterLabel.Name = "filterLabel";
            this.filterLabel.Size = new System.Drawing.Size(56, 19);
            this.filterLabel.Text = "Find type:";
            // 
            // filterTextBox
            // 
            this.filterTextBox.Name = "filterTextBox";
            this.filterTextBox.Size = new System.Drawing.Size(100, 22);
            this.filterTextBox.Padding = new System.Windows.Forms.Padding(0, 0, 1, 0);
            this.filterTextBox.Leave += new System.EventHandler(this.filterTextBox_Leave);
            this.filterTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ModelsExplorer_KeyDown);
            this.filterTextBox.TextChanged += new System.EventHandler(this.filterTextBox_TextChanged);
            // 
            // refreshButton
            //
            this.refreshButton.Margin = new System.Windows.Forms.Padding(0, 1, 0, 1);
            this.refreshButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.refreshButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.refreshButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.refreshButton.Name = "refreshButton";
            this.refreshButton.Size = new System.Drawing.Size(23, 19);
            this.refreshButton.Text = "toolStripButton1";
            this.refreshButton.ToolTipText = "Refresh";
            this.refreshButton.Click += new System.EventHandler(this.RefreshButton_Click);
            // 
            // rebuildButton
            //
            this.rebuildButton.Margin = new System.Windows.Forms.Padding(0, 1, 0, 1);
            this.rebuildButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.rebuildButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.rebuildButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.rebuildButton.Name = "rebuildButton";
            this.rebuildButton.Size = new System.Drawing.Size(23, 19);
            this.rebuildButton.Text = "toolStripButton1";
            this.rebuildButton.ToolTipText = "Rebuild";
            this.rebuildButton.Click += new System.EventHandler(this.RebuildButton_Click);
            // 
            // searchButton
            //
            this.searchButton.Margin = new System.Windows.Forms.Padding(0, 1, 0, 1);
            this.searchButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.searchButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.searchButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.searchButton.Name = "searchButton";
            this.searchButton.Size = new System.Drawing.Size(23, 19);
            this.searchButton.Text = "toolStripButton1";
            this.searchButton.ToolTipText = "Search";
            this.searchButton.Click += new System.EventHandler(this.SearchButton_Click);
            // 
            // ModelsExplorer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.outlineTreeView);
            this.Controls.Add(this.toolStrip);
            this.Name = "ModelsExplorer";
            this.Padding = new System.Windows.Forms.Padding(1, 0, 1, 0);
            this.Size = new System.Drawing.Size(385, 359);
            this.outlineContextMenuStrip.ResumeLayout(false);
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.FixedTreeView outlineTreeView;
        private System.Windows.Forms.Timer updateTimer;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripLabel filterLabel;
        private System.Windows.Forms.ToolStripSpringTextBox filterTextBox;
        private System.Windows.Forms.ToolStripButton rebuildButton;
        private System.Windows.Forms.ToolStripButton refreshButton;
        private System.Windows.Forms.ToolStripButton searchButton;
        private System.Windows.Forms.ContextMenuStrip outlineContextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem exploreToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem convertToolStripMenuItem;
        private Ookii.Dialogs.VistaFolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
    }
}