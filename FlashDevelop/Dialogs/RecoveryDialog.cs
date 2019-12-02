// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.IO;
using System.Windows.Forms;
using PluginCore.Localization;
using PluginCore.Utilities;
using PluginCore.Helpers;
using PluginCore.Controls;
using PluginCore;

namespace FlashDevelop.Dialogs
{
    public class RecoveryDialog : SmartForm
    {
        private System.Windows.Forms.Label infoLabel;
        private System.Windows.Forms.Button deleteButton;
        private System.Windows.Forms.Button notifyButton;
        private System.Windows.Forms.Button openButton;

        public RecoveryDialog()
        {
            this.Owner = Globals.MainForm;
            this.Font = PluginBase.MainForm.Settings.DefaultFont;
            this.FormGuid = "54452519-b993-47f6-9d27-22d31bced4ff";
            this.InitializeComponent();
            this.ApplyLocalizedTexts();
        }

        #region Windows Form Designer Generated Code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.deleteButton = new System.Windows.Forms.ButtonEx();
            this.openButton = new System.Windows.Forms.ButtonEx();
            this.notifyButton = new System.Windows.Forms.ButtonEx();
            this.infoLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // deleteButton
            // 
            this.deleteButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.deleteButton.Location = new System.Drawing.Point(133, 88);
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Size = new System.Drawing.Size(85, 23);
            this.deleteButton.TabIndex = 2;
            this.deleteButton.Text = "&Delete Files";
            this.deleteButton.UseVisualStyleBackColor = true;
            this.deleteButton.Click += this.DeleteButtonClick;
            // 
            // openButton
            // 
            this.openButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.openButton.Location = new System.Drawing.Point(42, 88);
            this.openButton.Name = "openButton";
            this.openButton.Size = new System.Drawing.Size(85, 23);
            this.openButton.TabIndex = 3;
            this.openButton.Text = "&Open Files";
            this.openButton.UseVisualStyleBackColor = true;
            this.openButton.Click += this.OpenButtonClick;
            // 
            // notifyButton
            // 
            this.notifyButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.notifyButton.Location = new System.Drawing.Point(224, 88);
            this.notifyButton.Name = "notifyButton";
            this.notifyButton.Size = new System.Drawing.Size(85, 23);
            this.notifyButton.TabIndex = 1;
            this.notifyButton.Text = "&Notify Later";
            this.notifyButton.UseVisualStyleBackColor = true;
            this.notifyButton.Click += this.NotifyButtonClick;
            // 
            // infoLabel
            //
            this.infoLabel.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right;
            this.infoLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.infoLabel.Location = new System.Drawing.Point(13, 13);
            this.infoLabel.Name = "infoLabel";
            this.infoLabel.Size = new System.Drawing.Size(331, 66);
            this.infoLabel.TabIndex = 4;
            this.infoLabel.Text = "The recovery folder contains temporary backup files and this possibly means that your previous coding session was interrupted. \r\n\r\nWhat do you want to do to these files?";
            this.infoLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // RecoveryDialog
            // 
            this.AcceptButton = this.openButton;
            this.CancelButton = this.notifyButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(356, 124);
            this.Controls.Add(this.notifyButton);
            this.Controls.Add(this.openButton);
            this.Controls.Add(this.deleteButton);
            this.Controls.Add(this.infoLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RecoveryDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = " Recovery File Notification";
            this.ResumeLayout(false);

        }

        #endregion

        #region Methods And Event Handlers

        /// <summary>
        /// Applies the localized texts to the form
        /// </summary>
        private void ApplyLocalizedTexts()
        {
            this.openButton.Text = TextHelper.GetString("Label.OpenFiles");
            this.notifyButton.Text = TextHelper.GetString("Label.NotifyLater");
            this.deleteButton.Text = TextHelper.GetString("Label.DeleteFiles");
            this.Text = " " + TextHelper.GetString("Title.RecoveryDialog");
            string info = TextHelper.GetString("Info.RecoveryInfo");
            this.infoLabel.Text = string.Format(info, "\n\n");
        }

        /// <summary>
        /// Notifies the user next time with the same info
        /// </summary>
        private void NotifyButtonClick(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Opens the files from the recovery files folder
        /// </summary>
        private void OpenButtonClick(object sender, EventArgs e)
        {
            string[] files = GetRecoveryFiles();
            foreach (string file in files)
            {
                string arguments = "bak;" + file;
                PluginBase.MainForm.CallCommand("NewFromTemplate", arguments);
            }
            string message = TextHelper.GetString("Info.DeleteFilesAlso");
            string caption = " " + TextHelper.GetString("Title.ConfirmDialog");
            if (MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                this.DeleteButtonClick(null, null);
            }
            else this.Close();
        }

        /// <summary>
        /// Deletes the files from the recovery files folder
        /// </summary>
        private void DeleteButtonClick(object sender, EventArgs e)
        {
            string[] files = GetRecoveryFiles();
            foreach (string file in files) FileHelper.Recycle(file);
            this.Close();
        }

        /// <summary>
        /// Gets the files from the recovery folder
        /// </summary>
        private static string[] GetRecoveryFiles()
        {
            string folder = Path.Combine(PathHelper.SettingDir, "Recovery");
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
            return Directory.GetFiles(folder);
        }

        /// <summary>
        /// Checks whether we should show the dialog
        /// </summary>
        public static bool ShouldShowDialog()
        {
            if (!SingleInstanceApp.AlreadyExists && GetRecoveryFiles().Length > 0) return true;
            return false;
        }

        /// <summary>
        /// Shows the recovery dialog
        /// </summary>
        public new static void Show()
        {
            using RecoveryDialog recoveryDialog = new RecoveryDialog();
            recoveryDialog.ShowDialog();
        }

        #endregion

    }

}