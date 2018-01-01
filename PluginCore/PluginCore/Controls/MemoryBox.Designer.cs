using System.Windows.Forms;

namespace PluginCore.PluginCore.Controls
{
    partial class MemoryBox
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
            this.SuspendLayout();
            // 
            // yesButton
            // 
            this.yesButton.DisabledBackColor = System.Drawing.SystemColors.Control;
            this.yesButton.DisabledTextColor = System.Drawing.SystemColors.ControlDark;
            this.yesButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.yesButton.Location = new System.Drawing.Point(16, 60);
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
            this.yesToAllButton.DisabledBackColor = System.Drawing.SystemColors.Control;
            this.yesToAllButton.DisabledTextColor = System.Drawing.SystemColors.ControlDark;
            this.yesToAllButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.yesToAllButton.Location = new System.Drawing.Point(97, 60);
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
            this.noButton.Location = new System.Drawing.Point(178, 60);
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
            this.noToAllButton.Location = new System.Drawing.Point(259, 60);
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
            this.cancelButton.Location = new System.Drawing.Point(340, 60);
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
            this.label.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.label.Location = new System.Drawing.Point(13, 24);
            this.label.Name = "label";
            this.label.Size = new System.Drawing.Size(35, 13);
            this.label.TabIndex = 7;
            this.label.Text = "label1";
            this.label.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // MemoryBox
            // 
            this.AcceptButton = this.yesButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(432, 98);
            this.Controls.Add(this.label);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.noToAllButton);
            this.Controls.Add(this.noButton);
            this.Controls.Add(this.yesToAllButton);
            this.Controls.Add(this.yesButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MemoryBox";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "MemoryBox";
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ButtonEx yesButton;
        private System.Windows.Forms.ButtonEx yesToAllButton;
        private System.Windows.Forms.ButtonEx noButton;
        private System.Windows.Forms.ButtonEx noToAllButton;
        private System.Windows.Forms.ButtonEx cancelButton;
        private Label label;
    }
}