using PluginCore;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using PluginCore.Localization;

namespace CodeRefactor.Controls
{
    public partial class RenameFileDialog : Form
    {
        static readonly Regex re_validFirstChar = new Regex(@"^[A-Z_$]", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private string oldFullPath;
        private string oldDirectoryName;
        private string oldName;
        private string ext;

        #region Windows Form Designer generated code

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button ExitButton;
        public System.Windows.Forms.Button OKButton;
        public System.Windows.Forms.TextBox NewName;
        public System.Windows.Forms.CheckBox UpdateReferences;
        private System.Windows.Forms.Label WarningLabel;

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
            this.WarningLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            //
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
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
            this.ExitButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ExitButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.ExitButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.ExitButton.Location = new System.Drawing.Point(335, 69);
            this.ExitButton.Name = "ExitButton";
            this.ExitButton.Size = new System.Drawing.Size(75, 23);
            this.ExitButton.TabIndex = 3;
            this.ExitButton.Text = "Cancel";
            this.ExitButton.UseVisualStyleBackColor = true;
            // 
            // OKButton
            //
            this.OKButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.OKButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.OKButton.Location = new System.Drawing.Point(254, 69);
            this.OKButton.Name = "OKButton";
            this.OKButton.Size = new System.Drawing.Size(75, 23);
            this.OKButton.TabIndex = 2;
            this.OKButton.Text = "OK";
            this.OKButton.UseVisualStyleBackColor = true;
            // 
            // NewName
            //
            this.NewName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.NewName.Location = new System.Drawing.Point(77, 16);
            this.NewName.Name = "NewName";
            this.NewName.Size = new System.Drawing.Size(333, 20);
            this.NewName.TabIndex = 0;
            this.NewName.WordWrap = false;
            // 
            // UpdateReferences
            //
            this.UpdateReferences.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.UpdateReferences.AutoSize = true;
            this.UpdateReferences.Checked = true;
            this.UpdateReferences.CheckState = System.Windows.Forms.CheckState.Checked;
            this.UpdateReferences.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.UpdateReferences.Location = new System.Drawing.Point(13, 49);
            this.UpdateReferences.Name = "UpdateReferences";
            this.UpdateReferences.Size = new System.Drawing.Size(114, 17);
            this.UpdateReferences.TabIndex = 1;
            this.UpdateReferences.Text = "Update references";
            this.UpdateReferences.UseVisualStyleBackColor = true;
            // 
            // WarningLabel
            // 
            this.WarningLabel.AutoSize = true;
            this.WarningLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.WarningLabel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.WarningLabel.Location = new System.Drawing.Point(10, 76);
            this.WarningLabel.Margin = new System.Windows.Forms.Padding(0);
            this.WarningLabel.Name = "WarningLabel";
            this.WarningLabel.Size = new System.Drawing.Size(54, 13);
            this.WarningLabel.TabIndex = 12;
            this.WarningLabel.Text = "Warning";
            this.WarningLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // RenameFileDialog
            //
            this.AcceptButton = this.OKButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.ExitButton;
            this.ClientSize = new System.Drawing.Size(424, 105);
            this.Controls.Add(this.WarningLabel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ExitButton);
            this.Controls.Add(this.OKButton);
            this.Controls.Add(this.NewName);
            this.Controls.Add(this.UpdateReferences);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RenameFileDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Rename Class";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public RenameFileDialog()
        {
            this.Owner = (Form)PluginBase.MainForm;
            this.Font = PluginBase.Settings.DefaultFont;
            this.InitializeComponent();
            this.OKButton.DialogResult = DialogResult.OK;
            this.WarningLabel.Text = TextHelper.GetString("Label.EnterName");
            this.Text = TextHelper.GetString("Title.RenameDialog").Replace(" '{0}'", "...");
            this.ExitButton.Text = TextHelper.GetString("FlashDevelop.Label.Cancel");
            this.OKButton.Text = TextHelper.GetString("FlashDevelop.Label.Ok");
        }

        /// <summary>
        /// 
        /// </summary>
        internal void ShowDialogFor(string path)
        {
            if (string.IsNullOrEmpty(path)) Close();
            else
            {
                UpdateReferences.Checked = true;
                SetOldFullPath(path);
                ShowDialog();
                OnNewNameChanged();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void SetOldFullPath(string path)
        {
            oldFullPath = path;
            oldDirectoryName = Path.GetDirectoryName(path);
            oldName = Path.GetFileNameWithoutExtension(path);
            ext = Path.GetExtension(path);
            NewName.TextChanged += OnNewNameChanged;
            NewName.Text = oldName;
            NewName.SelectAll();
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnNewNameChanged(object sender = null, EventArgs e = null)
        {
            bool canRename = false;
            bool withImage = false;    
            string newName = NewName.Text;
            string newFileName = string.Concat(newName, ext);
            string newFullPath = Path.Combine(oldDirectoryName, newFileName);
            this.WarningLabel.AutoSize = true;
            this.Text = " " + string.Format(TextHelper.GetString("Title.RenameDialog"), oldName);
            if (string.IsNullOrEmpty(newName) || newName == oldName)
            {
                WarningLabel.Text = TextHelper.GetString("Label.EnterName");
            }
            else if (!re_validFirstChar.IsMatch(newName))
            {
                WarningLabel.Text = " " + TextHelper.GetString("Label.NotAValidId");
                withImage = true;
            }
            else if (File.Exists(newFullPath))
            {
                WarningLabel.Text = " " + string.Format(TextHelper.GetString("Label.NameTaken"), newFileName);
                withImage = true;
            }
            else
            {
                WarningLabel.ResetText();
                canRename = true;
            }
            if (withImage)
            {
                int width = WarningLabel.Width;
                WarningLabel.Image = PluginBase.MainForm.FindImage("197");
                WarningLabel.AutoSize = false;
                WarningLabel.Width = width + WarningLabel.Image.Width;
                WarningLabel.Height = WarningLabel.Image.Height;
            }
            else WarningLabel.Image = null;
            OKButton.Enabled = canRename;
        }

    }

}
