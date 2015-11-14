using System.ComponentModel;
using System.Windows.Forms;
using PluginCore.Localization;
using PluginCore;

namespace CodeRefactor.Controls
{
    public partial class InlineRenameDialog : UserControl
    {
        IContainer components = null;
        Label titleLabel;
        Label border;
        public CheckBox IncludeComments;
        public CheckBox IncludeStrings;
        public CheckBox PreviewChanges;
        public Button ApplyButton;
        public Button CancelButton;

        public InlineRenameDialog(string targetName, bool? includeComments, bool? includeStrings, bool? previewChanges)
        {
            InitializeComponent();
            InititalizeLocalization(targetName);
            ActivateCheckBoxes(includeComments, includeStrings, previewChanges);

            CancelButton.Image = PluginBase.MainForm.FindImage("153");
        }

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
        void InitializeComponent()
        {
            this.titleLabel = new System.Windows.Forms.Label();
            this.IncludeComments = new System.Windows.Forms.CheckBox();
            this.IncludeStrings = new System.Windows.Forms.CheckBox();
            this.PreviewChanges = new System.Windows.Forms.CheckBox();
            this.ApplyButton = new System.Windows.Forms.Button();
            this.CancelButton = new System.Windows.Forms.Button();
            this.border = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // titleLabel
            // 
            this.titleLabel.AutoEllipsis = true;
            this.titleLabel.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
            this.titleLabel.Location = new System.Drawing.Point(0, 0);
            this.titleLabel.Margin = new System.Windows.Forms.Padding(0);
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Padding = new System.Windows.Forms.Padding(3);
            this.titleLabel.Size = new System.Drawing.Size(175, 25);
            this.titleLabel.TabIndex = 5;
            this.titleLabel.Text = "Enter text";
            // 
            // IncludeComments
            // 
            this.IncludeComments.Location = new System.Drawing.Point(0, 25);
            this.IncludeComments.Margin = new System.Windows.Forms.Padding(0);
            this.IncludeComments.Name = "IncludeComments";
            this.IncludeComments.Padding = new System.Windows.Forms.Padding(3);
            this.IncludeComments.Size = new System.Drawing.Size(200, 25);
            this.IncludeComments.TabIndex = 1;
            this.IncludeComments.Text = "Enter text";
            this.IncludeComments.UseVisualStyleBackColor = true;
            // 
            // IncludeStrings
            // 
            this.IncludeStrings.Location = new System.Drawing.Point(0, 50);
            this.IncludeStrings.Margin = new System.Windows.Forms.Padding(0);
            this.IncludeStrings.Name = "IncludeStrings";
            this.IncludeStrings.Padding = new System.Windows.Forms.Padding(3);
            this.IncludeStrings.Size = new System.Drawing.Size(200, 25);
            this.IncludeStrings.TabIndex = 2;
            this.IncludeStrings.Text = "Enter text";
            this.IncludeStrings.UseVisualStyleBackColor = true;
            // 
            // PreviewChanges
            // 
            this.PreviewChanges.Location = new System.Drawing.Point(0, 75);
            this.PreviewChanges.Margin = new System.Windows.Forms.Padding(0);
            this.PreviewChanges.Name = "PreviewChanges";
            this.PreviewChanges.Padding = new System.Windows.Forms.Padding(3);
            this.PreviewChanges.Size = new System.Drawing.Size(200, 25);
            this.PreviewChanges.TabIndex = 3;
            this.PreviewChanges.Text = "Enter text";
            this.PreviewChanges.UseVisualStyleBackColor = true;
            // 
            // ApplyButton
            // 
            this.ApplyButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.ApplyButton.Location = new System.Drawing.Point(120, 100);
            this.ApplyButton.Margin = new System.Windows.Forms.Padding(0);
            this.ApplyButton.Name = "ApplyButton";
            this.ApplyButton.Size = new System.Drawing.Size(75, 25);
            this.ApplyButton.TabIndex = 0;
            this.ApplyButton.Text = "Apply";
            this.ApplyButton.UseVisualStyleBackColor = true;
            // 
            // CancelButton
            // 
            this.CancelButton.FlatAppearance.BorderSize = 0;
            this.CancelButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.CancelButton.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
            this.CancelButton.Location = new System.Drawing.Point(175, 0);
            this.CancelButton.Margin = new System.Windows.Forms.Padding(0);
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new System.Drawing.Size(25, 25);
            this.CancelButton.TabIndex = 4;
            this.CancelButton.UseVisualStyleBackColor = true;
            // 
            // border
            // 
            this.border.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.border.Location = new System.Drawing.Point(0, 130);
            this.border.Margin = new System.Windows.Forms.Padding(0);
            this.border.Name = "border";
            this.border.Size = new System.Drawing.Size(200, 5);
            this.border.TabIndex = 6;
            // 
            // InlineRenameDialog
            // 
            this.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.Controls.Add(this.CancelButton);
            this.Controls.Add(this.border);
            this.Controls.Add(this.ApplyButton);
            this.Controls.Add(this.PreviewChanges);
            this.Controls.Add(this.IncludeStrings);
            this.Controls.Add(this.IncludeComments);
            this.Controls.Add(this.titleLabel);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
            this.Name = "InlineRenameDialog";
            this.Size = new System.Drawing.Size(200, 135);
            this.ResumeLayout(false);

        }

        #endregion

        void InititalizeLocalization(string targetName)
        {
            titleLabel.Text = string.Format(TextHelper.GetString("Title.RenameDialog"), targetName);
            IncludeComments.Text = TextHelper.GetString("Label.IncludeComments");
            IncludeStrings.Text = TextHelper.GetString("Label.IncludeStrings");
            PreviewChanges.Text = "Preview changes";
            ApplyButton.Text = TextHelper.GetString("ProjectManager.Label.Apply");
        }

        void ActivateCheckBoxes(bool? includeComments, bool? includeStrings, bool? previewChanges)
        {
            if (includeComments.HasValue)
                IncludeComments.Checked = includeComments.Value;
            else
                IncludeComments.Enabled = false;

            if (includeStrings.HasValue)
                IncludeStrings.Checked = includeStrings.Value;
            else
                IncludeStrings.Enabled = false;

            if (previewChanges.HasValue)
                PreviewChanges.Checked = previewChanges.Value;
            else
                PreviewChanges.Enabled = false;
        }
    }
}
