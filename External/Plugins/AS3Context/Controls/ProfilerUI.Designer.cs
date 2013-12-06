namespace AS3Context.Controls
{
    partial class ProfilerUI
    {
        /// <summary> 
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur de composants

        /// <summary> 
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas 
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.container = new System.Windows.Forms.Panel();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.memoryPage = new System.Windows.Forms.TabPage();
            this.memStatsPanel = new System.Windows.Forms.Panel();
            this.memStatsLabel = new System.Windows.Forms.Label();
            this.memScaleLabel = new System.Windows.Forms.Label();
            this.memScaleCombo = new System.Windows.Forms.ComboBox();
            this.liveObjectsPage = new System.Windows.Forms.TabPage();
            this.listView = new AS3Context.Controls.ListViewXP();
            this.typeColumn = new System.Windows.Forms.ColumnHeader();
            this.pkgColumn = new System.Windows.Forms.ColumnHeader();
            this.maxColumn = new System.Windows.Forms.ColumnHeader();
            this.countColumn = new System.Windows.Forms.ColumnHeader();
            this.memColumn = new System.Windows.Forms.ColumnHeader();
            this.objectsPage = new System.Windows.Forms.TabPage();
            this.toolStrip = new PluginCore.Controls.ToolStripEx();
            this.memLabel = new System.Windows.Forms.ToolStripLabel();
            this.autoButton = new System.Windows.Forms.ToolStripButton();
            this.runButton = new System.Windows.Forms.ToolStripButton();
            this.gcButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.labelTarget = new System.Windows.Forms.ToolStripLabel();
            this.sep = new System.Windows.Forms.ToolStripSeparator();
            this.profilerChooser = new System.Windows.Forms.ToolStripDropDownButton();
            this.defaultToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.container.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.memoryPage.SuspendLayout();
            this.memStatsPanel.SuspendLayout();
            this.liveObjectsPage.SuspendLayout();
            this.toolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // container
            // 
            this.container.Controls.Add(this.tabControl);
            this.container.Dock = System.Windows.Forms.DockStyle.Fill;
            this.container.Location = new System.Drawing.Point(1, 26);
            this.container.Name = "container";
            this.container.Padding = new System.Windows.Forms.Padding(3, 3, 2, 2);
            this.container.Size = new System.Drawing.Size(488, 338);
            this.container.TabIndex = 0;
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.memoryPage);
            this.tabControl.Controls.Add(this.liveObjectsPage);
            this.tabControl.Controls.Add(this.objectsPage);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(3, 3);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(483, 333);
            this.tabControl.TabIndex = 2;
            // 
            // memoryPage
            // 
            this.memoryPage.Controls.Add(this.memStatsPanel);
            this.memoryPage.Location = new System.Drawing.Point(4, 22);
            this.memoryPage.Name = "memoryPage";
            this.memoryPage.Padding = new System.Windows.Forms.Padding(3);
            this.memoryPage.Size = new System.Drawing.Size(475, 307);
            this.memoryPage.TabIndex = 2;
            this.memoryPage.Text = "Memory";
            this.memoryPage.UseVisualStyleBackColor = true;
            // 
            // memStatsPanel
            // 
            this.memStatsPanel.Controls.Add(this.memStatsLabel);
            this.memStatsPanel.Controls.Add(this.memScaleLabel);
            this.memStatsPanel.Controls.Add(this.memScaleCombo);
            this.memStatsPanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.memStatsPanel.Location = new System.Drawing.Point(3, 3);
            this.memStatsPanel.Name = "memStatsPanel";
            this.memStatsPanel.Size = new System.Drawing.Size(161, 301);
            this.memStatsPanel.TabIndex = 0;
            // 
            // memStatsLabel
            // 
            this.memStatsLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.memStatsLabel.Location = new System.Drawing.Point(3, 0);
            this.memStatsLabel.Name = "memStatsLabel";
            this.memStatsLabel.Size = new System.Drawing.Size(158, 251);
            this.memStatsLabel.TabIndex = 3;
            this.memStatsLabel.Text = "stats";
            // 
            // memScaleLabel
            // 
            this.memScaleLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.memScaleLabel.AutoSize = true;
            this.memScaleLabel.Location = new System.Drawing.Point(3, 261);
            this.memScaleLabel.Name = "memScaleLabel";
            this.memScaleLabel.Size = new System.Drawing.Size(67, 13);
            this.memScaleLabel.TabIndex = 4;
            this.memScaleLabel.Text = "Graph scale:";
            // 
            // memScaleCombo
            // 
            this.memScaleCombo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.memScaleCombo.FormattingEnabled = true;
            this.memScaleCombo.Items.AddRange(new object[] {
            "1:1",
            "2:1",
            "3:1",
            "4:1"});
            this.memScaleCombo.Location = new System.Drawing.Point(6, 277);
            this.memScaleCombo.Name = "memScaleCombo";
            this.memScaleCombo.Size = new System.Drawing.Size(68, 21);
            this.memScaleCombo.TabIndex = 5;
            // 
            // liveObjectsPage
            // 
            this.liveObjectsPage.Controls.Add(this.listView);
            this.liveObjectsPage.Location = new System.Drawing.Point(4, 22);
            this.liveObjectsPage.Name = "liveObjectsPage";
            this.liveObjectsPage.Padding = new System.Windows.Forms.Padding(2, 3, 3, 2);
            this.liveObjectsPage.Size = new System.Drawing.Size(475, 307);
            this.liveObjectsPage.TabIndex = 0;
            this.liveObjectsPage.Text = "Live Objects Count";
            this.liveObjectsPage.UseVisualStyleBackColor = true;
            // 
            // listView
            // 
            this.listView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.typeColumn,
            this.pkgColumn,
            this.maxColumn,
            this.countColumn,
            this.memColumn});
            this.listView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView.FullRowSelect = true;
            this.listView.GridLines = true;
            this.listView.HideSelection = false;
            this.listView.Location = new System.Drawing.Point(2, 3);
            this.listView.MultiSelect = false;
            this.listView.Name = "listView";
            this.listView.Size = new System.Drawing.Size(470, 302);
            this.listView.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.listView.TabIndex = 2;
            this.listView.UseCompatibleStateImageBehavior = false;
            this.listView.View = System.Windows.Forms.View.Details;
            // 
            // typeColumn
            // 
            this.typeColumn.Text = "Type";
            this.typeColumn.Width = 200;
            // 
            // pkgColumn
            // 
            this.pkgColumn.Text = "Package";
            this.pkgColumn.Width = 250;
            // 
            // maxColumn
            // 
            this.maxColumn.Text = "Maximum";
            this.maxColumn.Width = 80;
            // 
            // countColumn
            // 
            this.countColumn.Text = "Count";
            this.countColumn.Width = 80;
            // 
            // memColumn
            // 
            this.memColumn.Text = "Memory";
            this.memColumn.Width = 80;
            // 
            // objectsPage
            // 
            this.objectsPage.Location = new System.Drawing.Point(4, 22);
            this.objectsPage.Name = "objectsPage";
            this.objectsPage.Padding = new System.Windows.Forms.Padding(2, 3, 3, 2);
            this.objectsPage.Size = new System.Drawing.Size(475, 307);
            this.objectsPage.TabIndex = 1;
            this.objectsPage.Text = "Objects";
            this.objectsPage.UseVisualStyleBackColor = true;
            // 
            // toolStrip
            // 
            this.toolStrip.CanOverflow = false;
            this.toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.memLabel,
            this.profilerChooser,
            this.sep,
            this.autoButton,
            this.runButton,
            this.gcButton,
            this.toolStripSeparator1,
            this.labelTarget});
            this.toolStrip.Location = new System.Drawing.Point(1, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Padding = new System.Windows.Forms.Padding(2, 1, 2, 2);
            this.toolStrip.Size = new System.Drawing.Size(488, 26);
            this.toolStrip.TabIndex = 0;
            // 
            // memLabel
            // 
            this.memLabel.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.memLabel.Name = "memLabel";
            this.memLabel.Size = new System.Drawing.Size(64, 20);
            this.memLabel.Text = "Memory: 0";
            // 
            // autoButton
            // 
            this.autoButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.autoButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.autoButton.Margin = new System.Windows.Forms.Padding(0, 1, 0, 1);
            this.autoButton.Name = "autoButton";
            this.autoButton.Size = new System.Drawing.Size(23, 21);
            this.autoButton.Text = "Auto-start Profiler";
            this.autoButton.Click += new System.EventHandler(this.autoButton_Click);
            // 
            // runButton
            // 
            this.runButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.runButton.Margin = new System.Windows.Forms.Padding(0, 1, 0, 1);
            this.runButton.Name = "runButton";
            this.runButton.Size = new System.Drawing.Size(23, 21);
            this.runButton.Text = "Start Profiler";
            this.runButton.Click += new System.EventHandler(this.runButton_Click);
            // 
            // gcButton
            // 
            this.gcButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.gcButton.Margin = new System.Windows.Forms.Padding(0, 1, 0, 1);
            this.gcButton.Name = "gcButton";
            this.gcButton.Size = new System.Drawing.Size(23, 21);
            this.gcButton.Text = "Run Garbage Collector";
            this.gcButton.Click += new System.EventHandler(this.gcButton_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 23);
            // 
            // labelTarget
            // 
            this.labelTarget.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.labelTarget.Name = "labelTarget";
            this.labelTarget.Size = new System.Drawing.Size(31, 20);
            this.labelTarget.Text = "(file)";
            // 
            // sep
            // 
            this.sep.Name = "sep";
            this.sep.Size = new System.Drawing.Size(6, 23);
            // 
            // profilerChooser
            // 
            this.profilerChooser.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.profilerChooser.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.defaultToolStripMenuItem});
            this.profilerChooser.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.profilerChooser.Name = "profilerChooser";
            this.profilerChooser.Size = new System.Drawing.Size(13, 20);
            this.profilerChooser.Text = "toolStripDropDownButton1";
            // 
            // defaultToolStripMenuItem
            // 
            this.defaultToolStripMenuItem.Name = "defaultToolStripMenuItem";
            this.defaultToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.defaultToolStripMenuItem.Text = "Default";
            // 
            // ProfilerUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.container);
            this.Controls.Add(this.toolStrip);
            this.Name = "ProfilerUI";
            this.Size = new System.Drawing.Size(490, 364);
            this.container.ResumeLayout(false);
            this.tabControl.ResumeLayout(false);
            this.memoryPage.ResumeLayout(false);
            this.memStatsPanel.ResumeLayout(false);
            this.memStatsPanel.PerformLayout();
            this.liveObjectsPage.ResumeLayout(false);
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ListViewXP listView;
        private System.Windows.Forms.Panel container;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripLabel memLabel;
        private System.Windows.Forms.ToolStripButton runButton;
        private System.Windows.Forms.ToolStripButton gcButton;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage liveObjectsPage;
        private System.Windows.Forms.TabPage objectsPage;
        private System.Windows.Forms.ColumnHeader typeColumn;
        private System.Windows.Forms.ColumnHeader pkgColumn;
        private System.Windows.Forms.ColumnHeader maxColumn;
        private System.Windows.Forms.ColumnHeader countColumn;
        private System.Windows.Forms.ColumnHeader memColumn;
        private System.Windows.Forms.ToolStripButton autoButton;
        private System.Windows.Forms.TabPage memoryPage;
        private System.Windows.Forms.Panel memStatsPanel;
        private System.Windows.Forms.Label memStatsLabel;
        private System.Windows.Forms.Label memScaleLabel;
        private System.Windows.Forms.ComboBox memScaleCombo;
        private System.Windows.Forms.ToolStripLabel labelTarget;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator sep;
        private System.Windows.Forms.ToolStripDropDownButton profilerChooser;
        private System.Windows.Forms.ToolStripMenuItem defaultToolStripMenuItem;

    }

}
