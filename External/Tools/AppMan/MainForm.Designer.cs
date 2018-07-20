using System;
using System.Windows.Forms;
using System.ComponentModel;

namespace AppMan
{
    partial class MainForm
    {
        private System.Windows.Forms.Label pathLabel;
        private System.Windows.Forms.Label selectLabel;
        private System.Windows.Forms.ListView listView;
        private System.Windows.Forms.TextBox pathTextBox;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button exploreButton;
        private System.Windows.Forms.Button deleteButton;
        private System.Windows.Forms.Button installButton;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel;
        private System.Windows.Forms.ColumnHeader versionHeader;
        private System.Windows.Forms.ColumnHeader statusHeader;
        private System.Windows.Forms.ColumnHeader infoHeader;
        private System.Windows.Forms.ColumnHeader nameHeader;
        private System.Windows.Forms.ColumnHeader descHeader;
        private System.Windows.Forms.ColumnHeader typeHeader;
        private System.Windows.Forms.LinkLabel updateLinkLabel;
        private System.Windows.Forms.LinkLabel allLinkLabel;
        private System.Windows.Forms.LinkLabel newLinkLabel;
        private System.Windows.Forms.LinkLabel instLinkLabel;
        private System.Windows.Forms.LinkLabel noneLinkLabel;

        #region Windows Form Designer Generated Code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.listView = new System.Windows.Forms.ListView();
            this.infoHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.nameHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.versionHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.descHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.statusHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.typeHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.deleteButton = new System.Windows.Forms.Button();
            this.installButton = new System.Windows.Forms.Button();
            this.pathLabel = new System.Windows.Forms.Label();
            this.pathTextBox = new System.Windows.Forms.TextBox();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.allLinkLabel = new System.Windows.Forms.LinkLabel();
            this.newLinkLabel = new System.Windows.Forms.LinkLabel();
            this.instLinkLabel = new System.Windows.Forms.LinkLabel();
            this.selectLabel = new System.Windows.Forms.Label();
            this.noneLinkLabel = new System.Windows.Forms.LinkLabel();
            this.exploreButton = new System.Windows.Forms.Button();
            this.updateLinkLabel = new System.Windows.Forms.LinkLabel();
            this.cancelButton = new System.Windows.Forms.Button();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // listView
            // 
            this.listView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listView.CheckBoxes = true;
            this.listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.nameHeader,
            this.versionHeader,
            this.infoHeader,
            this.descHeader,
            this.statusHeader,
            this.typeHeader});
            this.listView.HideSelection = true;
            this.listView.MultiSelect = false;
            this.listView.FullRowSelect = true;
            this.listView.GridLines = true;
            this.listView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listView.Location = new System.Drawing.Point(13, 48);
            this.listView.Name = "listView";
            this.listView.ShowItemToolTips = true;
            this.listView.Size = new System.Drawing.Size(760, 420);
            this.listView.TabIndex = 4;
            this.listView.UseCompatibleStateImageBehavior = false;
            this.listView.View = System.Windows.Forms.View.Details;
            this.listView.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.ListViewItemCheck);
            this.listView.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.ListViewItemChecked);
            this.listView.DrawSubItem += new System.Windows.Forms.DrawListViewSubItemEventHandler(this.ListViewDrawSubItem);
            this.listView.DrawColumnHeader += new System.Windows.Forms.DrawListViewColumnHeaderEventHandler(this.ListViewDrawColumnHeader);
            this.listView.DrawItem += new System.Windows.Forms.DrawListViewItemEventHandler(this.ListViewDrawItem);
            this.listView.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ListViewMouseMove);
            this.listView.Click += new System.EventHandler(this.ListViewClick);
            this.listView.OwnerDraw = true;
            // 
            // nameHeader
            // 
            this.nameHeader.Text = "Name";
            this.nameHeader.Width = 160;
            // 
            // infoHeader
            // 
            this.infoHeader.Text = " !";
            this.infoHeader.Width = 24;
            // 
            // versionHeader
            // 
            this.versionHeader.Text = "Version";
            this.versionHeader.Width = 90;
            // 
            // descHeader
            // 
            this.descHeader.Text = "Description";
            this.descHeader.Width = 319;
            // 
            // statusHeader
            // 
            this.statusHeader.Text = "Status";
            this.statusHeader.Width = 70;
            // 
            // typeHeader
            // 
            this.typeHeader.Text = "Type";
            this.typeHeader.Width = 75;
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 550);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Padding = new System.Windows.Forms.Padding(1, 0, 16, 0);
            this.statusStrip.Size = new System.Drawing.Size(756, 22);
            this.statusStrip.TabIndex = 4;
            this.statusStrip.Text = "statusStrip";
            // 
            // statusLabel
            // 
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(719, 17);
            this.statusLabel.Spring = true;
            this.statusLabel.Text = "No items selected.";
            this.statusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // deleteButton
            // 
            this.deleteButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.deleteButton.Enabled = false;
            this.deleteButton.Location = new System.Drawing.Point(628, 511);
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Size = new System.Drawing.Size(146, 27);
            this.deleteButton.TabIndex = 11;
            this.deleteButton.Text = "Delete selected...";
            this.deleteButton.UseVisualStyleBackColor = true;
            this.deleteButton.Click += new System.EventHandler(this.DeleteButtonClick);
            // 
            // installButton
            // 
            this.installButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.installButton.Enabled = false;
            this.installButton.Location = new System.Drawing.Point(628, 477);
            this.installButton.Name = "installButton";
            this.installButton.Size = new System.Drawing.Size(146, 27);
            this.installButton.TabIndex = 10;
            this.installButton.Text = "Install selected...";
            this.installButton.UseVisualStyleBackColor = true;
            this.installButton.Click += new System.EventHandler(this.InstallButtonClick);
            // 
            // pathLabel
            // 
            this.pathLabel.AutoSize = true;
            this.pathLabel.Location = new System.Drawing.Point(13, 18);
            this.pathLabel.Name = "pathLabel";
            this.pathLabel.Size = new System.Drawing.Size(68, 15);
            this.pathLabel.TabIndex = 1;
            this.pathLabel.Text = "Install path:";
            // 
            // pathTextBox
            // 
            this.pathTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pathTextBox.Enabled = false;
            this.pathTextBox.Location = new System.Drawing.Point(86, 15);
            this.pathTextBox.Name = "pathTextBox";
            this.pathTextBox.ReadOnly = true;
            this.pathTextBox.Size = new System.Drawing.Size(585, 23);
            this.pathTextBox.TabIndex = 2;
            // 
            // progressBar
            // 
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.Location = new System.Drawing.Point(13, 513);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(608, 23);
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar.TabIndex = 12;
            // 
            // allLinkLabel
            // 
            this.allLinkLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.allLinkLabel.AutoSize = true;
            this.allLinkLabel.Location = new System.Drawing.Point(61, 491);
            this.allLinkLabel.Name = "allLinkLabel";
            this.allLinkLabel.Size = new System.Drawing.Size(21, 15);
            this.allLinkLabel.TabIndex = 6;
            this.allLinkLabel.TabStop = true;
            this.allLinkLabel.Text = "All";
            this.allLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.AllLinkLabelLinkClicked);
            // 
            // newLinkLabel
            // 
            this.newLinkLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.newLinkLabel.AutoSize = true;
            this.newLinkLabel.Location = new System.Drawing.Point(127, 491);
            this.newLinkLabel.Name = "newLinkLabel";
            this.newLinkLabel.Size = new System.Drawing.Size(31, 15);
            this.newLinkLabel.TabIndex = 8;
            this.newLinkLabel.TabStop = true;
            this.newLinkLabel.Text = "New";
            this.newLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.NewLinkLabelLinkClicked);
            // 
            // instLinkLabel
            // 
            this.instLinkLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.instLinkLabel.AutoSize = true;
            this.instLinkLabel.Location = new System.Drawing.Point(164, 491);
            this.instLinkLabel.Name = "instLinkLabel";
            this.instLinkLabel.Size = new System.Drawing.Size(51, 15);
            this.instLinkLabel.TabIndex = 9;
            this.instLinkLabel.TabStop = true;
            this.instLinkLabel.Text = "Installed";
            this.instLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.InstLinkLabelLinkClicked);
            // 
            // selectLabel
            // 
            this.selectLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.selectLabel.AutoSize = true;
            this.selectLabel.Location = new System.Drawing.Point(12, 491);
            this.selectLabel.Name = "selectLabel";
            this.selectLabel.Size = new System.Drawing.Size(41, 15);
            this.selectLabel.TabIndex = 5;
            this.selectLabel.Text = "Select:";
            // 
            // noneLinkLabel
            // 
            this.noneLinkLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.noneLinkLabel.AutoSize = true;
            this.noneLinkLabel.Location = new System.Drawing.Point(85, 491);
            this.noneLinkLabel.Name = "noneLinkLabel";
            this.noneLinkLabel.Size = new System.Drawing.Size(36, 15);
            this.noneLinkLabel.TabIndex = 7;
            this.noneLinkLabel.TabStop = true;
            this.noneLinkLabel.Text = "None";
            this.noneLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.NoneLinkLabelLinkClicked);
            // 
            // exploreButton
            // 
            this.exploreButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.exploreButton.Location = new System.Drawing.Point(679, 13);
            this.exploreButton.Name = "exploreButton";
            this.exploreButton.Size = new System.Drawing.Size(95, 27);
            this.exploreButton.TabIndex = 3;
            this.exploreButton.Text = "Explore...";
            this.exploreButton.UseVisualStyleBackColor = true;
            this.exploreButton.Click += new System.EventHandler(this.ExploreButtonClick);
            // 
            // updateLinkLabel
            // 
            this.updateLinkLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.updateLinkLabel.AutoSize = true;
            this.updateLinkLabel.Location = new System.Drawing.Point(221, 491);
            this.updateLinkLabel.Name = "updateLinkLabel";
            this.updateLinkLabel.Size = new System.Drawing.Size(50, 15);
            this.updateLinkLabel.TabIndex = 13;
            this.updateLinkLabel.TabStop = true;
            this.updateLinkLabel.Text = "Updates";
            this.updateLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.UpdatesLinkLabelLinkClicked);
            // 
            // taskButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.Enabled = false;
            this.cancelButton.Location = new System.Drawing.Point(587, 477);
            this.cancelButton.Name = "taskButton";
            this.cancelButton.Size = new System.Drawing.Size(34, 32);
            this.cancelButton.TabIndex = 15;
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.CancelButtonClick);
            // 
            // MainForm
            //
            this.Text = "AppMan";
            this.Name = "MainForm";
            this.HelpButton = true;
            this.DoubleBuffered = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(787, 572);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.updateLinkLabel);
            this.Controls.Add(this.exploreButton);
            this.Controls.Add(this.noneLinkLabel);
            this.Controls.Add(this.selectLabel);
            this.Controls.Add(this.instLinkLabel);
            this.Controls.Add(this.newLinkLabel);
            this.Controls.Add(this.allLinkLabel);
            this.Controls.Add(this.pathTextBox);
            this.Controls.Add(this.pathLabel);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.installButton);
            this.Controls.Add(this.deleteButton);
            this.Controls.Add(this.listView);
            this.Controls.Add(this.statusStrip);
            this.MinimumSize = new System.Drawing.Size(485, 340);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new EventHandler(this.MainFormLoad);
            this.HelpRequested += new HelpEventHandler(this.MainFormHelpRequested);
            this.HelpButtonClicked += new CancelEventHandler(this.MainFormHelpButtonClicked);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

    }

}
