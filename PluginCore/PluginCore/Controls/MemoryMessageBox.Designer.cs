using System.Windows.Forms;

namespace PluginCore.PluginCore.Controls
{
    partial class MemoryMessageBox
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
            this.yesButton = new System.Windows.Forms.ButtonEx();
            this.yesToAllButton = new System.Windows.Forms.ButtonEx();
            this.noButton = new System.Windows.Forms.ButtonEx();
            this.noToAllButton = new System.Windows.Forms.ButtonEx();
            this.cancelButton = new System.Windows.Forms.ButtonEx();
            this.label = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.dontAsk = new System.Windows.Forms.CheckBoxEx();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // yesButton
            // 
            this.yesButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.yesButton.DisabledBackColor = System.Drawing.SystemColors.Control;
            this.yesButton.DisabledTextColor = System.Drawing.SystemColors.ControlDark;
            this.yesButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.yesButton.Location = new System.Drawing.Point(3, 3);
            this.yesButton.Name = "yesButton";
            this.yesButton.Size = new System.Drawing.Size(75, 23);
            this.yesButton.TabIndex = 1;
            this.yesButton.Text = "Yes";
            this.yesButton.UseTheme = true;
            this.yesButton.UseVisualStyleBackColor = true;
            this.yesButton.Click += new System.EventHandler(this.OnYesButtonClick);
            // 
            // yesToAllButton
            // 
            this.yesToAllButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.yesToAllButton.DisabledBackColor = System.Drawing.SystemColors.Control;
            this.yesToAllButton.DisabledTextColor = System.Drawing.SystemColors.ControlDark;
            this.yesToAllButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.yesToAllButton.Location = new System.Drawing.Point(84, 3);
            this.yesToAllButton.Name = "yesToAllButton";
            this.yesToAllButton.Size = new System.Drawing.Size(75, 23);
            this.yesToAllButton.TabIndex = 2;
            this.yesToAllButton.Text = "Yes to All";
            this.yesToAllButton.UseTheme = true;
            this.yesToAllButton.UseVisualStyleBackColor = true;
            this.yesToAllButton.Click += new System.EventHandler(this.OnYesToAllButtonClick);
            // 
            // noButton
            // 
            this.noButton.DisabledBackColor = System.Drawing.SystemColors.Control;
            this.noButton.DisabledTextColor = System.Drawing.SystemColors.ControlDark;
            this.noButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.noButton.Location = new System.Drawing.Point(165, 3);
            this.noButton.Name = "noButton";
            this.noButton.Size = new System.Drawing.Size(75, 23);
            this.noButton.TabIndex = 3;
            this.noButton.Text = "No";
            this.noButton.UseTheme = true;
            this.noButton.UseVisualStyleBackColor = true;
            this.noButton.Click += new System.EventHandler(this.OnNoButtonClick);
            // 
            // noToAllButton
            // 
            this.noToAllButton.DisabledBackColor = System.Drawing.SystemColors.Control;
            this.noToAllButton.DisabledTextColor = System.Drawing.SystemColors.ControlDark;
            this.noToAllButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.noToAllButton.Location = new System.Drawing.Point(246, 3);
            this.noToAllButton.Name = "noToAllButton";
            this.noToAllButton.Size = new System.Drawing.Size(75, 23);
            this.noToAllButton.TabIndex = 4;
            this.noToAllButton.Text = "Not to All";
            this.noToAllButton.UseTheme = true;
            this.noToAllButton.UseVisualStyleBackColor = true;
            this.noToAllButton.Click += new System.EventHandler(this.OnNoToAllButtonClick);
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.DisabledBackColor = System.Drawing.SystemColors.Control;
            this.cancelButton.DisabledTextColor = System.Drawing.SystemColors.ControlDark;
            this.cancelButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cancelButton.Location = new System.Drawing.Point(327, 3);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 5;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseTheme = true;
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.OnCancelButtonClick);
            // 
            // label
            // 
            this.label.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label.AutoSize = true;
            this.label.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label.Location = new System.Drawing.Point(3, 20);
            this.label.Margin = new System.Windows.Forms.Padding(3, 20, 3, 20);
            this.label.Name = "label";
            this.label.Size = new System.Drawing.Size(405, 13);
            this.label.TabIndex = 7;
            this.label.Text = "label1";
            this.label.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.BackColor = System.Drawing.Color.Transparent;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.label, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 1);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(19, 17);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(411, 88);
            this.tableLayoutPanel1.TabIndex = 8;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.tableLayoutPanel2.AutoSize = true;
            this.tableLayoutPanel2.ColumnCount = 5;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.Controls.Add(this.yesButton, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.yesToAllButton, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.cancelButton, 4, 0);
            this.tableLayoutPanel2.Controls.Add(this.noButton, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.noToAllButton, 3, 0);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 56);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(405, 29);
            this.tableLayoutPanel2.TabIndex = 9;
            // 
            // dontAsk
            // 
            this.dontAsk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.dontAsk.AutoSize = true;
            this.dontAsk.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.dontAsk.DisabledTextColor = System.Drawing.SystemColors.ControlDark;
            this.dontAsk.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.dontAsk.Location = new System.Drawing.Point(10, 117);
            this.dontAsk.Name = "dontAsk";
            this.dontAsk.Size = new System.Drawing.Size(25, 5);
            this.dontAsk.TabIndex = 9;
            this.dontAsk.UseTheme = true;
            this.dontAsk.UseVisualStyleBackColor = true;
            // 
            // MemoryMessageBox
            // 
            this.AcceptButton = this.yesButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(448, 131);
            this.Controls.Add(this.dontAsk);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MemoryMessageBox";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "MemoryBox";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ButtonEx yesButton;
        private System.Windows.Forms.ButtonEx yesToAllButton;
        private System.Windows.Forms.ButtonEx noButton;
        private System.Windows.Forms.ButtonEx noToAllButton;
        private System.Windows.Forms.ButtonEx cancelButton;
        private Label label;
        private TableLayoutPanel tableLayoutPanel1;
        private TableLayoutPanel tableLayoutPanel2;
        private CheckBoxEx dontAsk;
    }
}