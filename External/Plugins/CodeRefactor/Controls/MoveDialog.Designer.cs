using System;
using System.Windows.Forms;

namespace CodeRefactor.Controls
{
    partial class MoveDialog
    {
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.input = new System.Windows.Forms.TextBox();
            this.showExternalClasspaths = new System.Windows.Forms.CheckBox();
            this.tree = new System.Windows.Forms.ListBox();
            this.cancelButton = new System.Windows.Forms.Button();
            this.processButton = new System.Windows.Forms.Button();
            this.fixPackages = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Target folder:";
            // 
            // input
            // 
            this.input.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.input.Location = new System.Drawing.Point(12, 27);
            this.input.Name = "input";
            this.input.Size = new System.Drawing.Size(360, 20);
            this.input.TabIndex = 0;
            // 
            // showExternalClasspaths
            // 
            this.showExternalClasspaths.AutoSize = true;
            this.showExternalClasspaths.Location = new System.Drawing.Point(12, 54);
            this.showExternalClasspaths.Name = "showExternalClasspaths";
            this.showExternalClasspaths.Size = new System.Drawing.Size(180, 17);
            this.showExternalClasspaths.TabIndex = 1;
            this.showExternalClasspaths.Text = "Show external classpaths(Ctrl+E)";
            this.showExternalClasspaths.UseVisualStyleBackColor = true;
            this.showExternalClasspaths.CheckStateChanged += new System.EventHandler(this.OnShowExternalClasspathsCheckStateChanged);
            // 
            // tree
            // 
            this.tree.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tree.FormattingEnabled = true;
            this.tree.Location = new System.Drawing.Point(12, 79);
            this.tree.Name = "tree";
            this.tree.Size = new System.Drawing.Size(360, 108);
            this.tree.TabIndex = 2;
            this.tree.MouseDoubleClick += new MouseEventHandler(OnTreeMouseDoubleClick);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(297, 226);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 5;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // processButton
            // 
            this.processButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.processButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.processButton.Location = new System.Drawing.Point(216, 226);
            this.processButton.Name = "processButton";
            this.processButton.Size = new System.Drawing.Size(75, 23);
            this.processButton.TabIndex = 4;
            this.processButton.Text = "Process";
            this.processButton.UseVisualStyleBackColor = true;
            // 
            // fixPackages
            // 
            this.fixPackages.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.fixPackages.AutoSize = true;
            this.fixPackages.Checked = true;
            this.fixPackages.CheckState = System.Windows.Forms.CheckState.Checked;
            this.fixPackages.Location = new System.Drawing.Point(11, 203);
            this.fixPackages.Name = "fixPackages";
            this.fixPackages.Size = new System.Drawing.Size(119, 17);
            this.fixPackages.TabIndex = 3;
            this.fixPackages.Text = "Fix packages(Ctrl+I)";
            this.fixPackages.UseVisualStyleBackColor = true;
            // 
            // MoveDialog
            // 
            this.AcceptButton = this.processButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(384, 261);
            this.Controls.Add(this.fixPackages);
            this.Controls.Add(this.tree);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.input);
            this.Controls.Add(this.showExternalClasspaths);
            this.Controls.Add(this.processButton);
            this.Controls.Add(this.cancelButton);
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(400, 300);
            this.Name = "MoveDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Move To Folder";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

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

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox showExternalClasspaths;
        private System.Windows.Forms.ListBox tree;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button processButton;
        private System.Windows.Forms.CheckBox fixPackages;
        private System.Windows.Forms.TextBox input;
    }
}