﻿namespace CodeRefactor.Controls
{
    partial class ClassRenameDialog
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
            this.label1 = new System.Windows.Forms.Label();
            this.ExitButton = new System.Windows.Forms.Button();
            this.OKButton = new System.Windows.Forms.Button();
            this.NewName = new System.Windows.Forms.TextBox();
            this.UpdateReferences = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(10, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "New name:";
            // 
            // ExitButton
            // 
            this.ExitButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.ExitButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.ExitButton.Location = new System.Drawing.Point(396, 45);
            this.ExitButton.Name = "ExitButton";
            this.ExitButton.Size = new System.Drawing.Size(75, 23);
            this.ExitButton.TabIndex = 10;
            this.ExitButton.Text = "Cancel";
            this.ExitButton.UseVisualStyleBackColor = true;
            // 
            // OKButton
            // 
            this.OKButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.OKButton.Location = new System.Drawing.Point(315, 45);
            this.OKButton.Name = "OKButton";
            this.OKButton.Size = new System.Drawing.Size(75, 23);
            this.OKButton.TabIndex = 9;
            this.OKButton.Text = "Ok";
            this.OKButton.UseVisualStyleBackColor = true;
            // 
            // NewName
            // 
            this.NewName.Location = new System.Drawing.Point(77, 16);
            this.NewName.Name = "NewName";
            this.NewName.Size = new System.Drawing.Size(393, 20);
            this.NewName.TabIndex = 7;
            this.NewName.WordWrap = false;
            // 
            // UpdateReferences
            // 
            this.UpdateReferences.AutoSize = true;
            this.UpdateReferences.Checked = true;
            this.UpdateReferences.CheckState = System.Windows.Forms.CheckState.Checked;
            this.UpdateReferences.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.UpdateReferences.Location = new System.Drawing.Point(13, 47);
            this.UpdateReferences.Name = "UpdateReferences";
            this.UpdateReferences.Size = new System.Drawing.Size(114, 17);
            this.UpdateReferences.TabIndex = 8;
            this.UpdateReferences.Text = "Update references";
            this.UpdateReferences.UseVisualStyleBackColor = true;
            // 
            // ClassRenameDialog
            // 
            this.AcceptButton = this.OKButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.ExitButton;
            this.ClientSize = new System.Drawing.Size(484, 80);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ExitButton);
            this.Controls.Add(this.OKButton);
            this.Controls.Add(this.NewName);
            this.Controls.Add(this.UpdateReferences);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ClassRenameDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Class Rename";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button ExitButton;
        public System.Windows.Forms.Button OKButton;
        public System.Windows.Forms.TextBox NewName;
        public System.Windows.Forms.CheckBox UpdateReferences;
    }
}