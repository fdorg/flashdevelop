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
        Label infoLabel;
        Button deleteButton;
        Button notifyButton;
        Button openButton;

        public RecoveryDialog()
        {
            Owner = Globals.MainForm;
            Font = PluginBase.Settings.DefaultFont;
            FormGuid = "54452519-b993-47f6-9d27-22d31bced4ff";
            InitializeComponent();
            ApplyLocalizedTexts();
        }

        #region Windows Form Designer Generated Code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        void InitializeComponent()
        {
            deleteButton = new ButtonEx();
            openButton = new ButtonEx();
            notifyButton = new ButtonEx();
            infoLabel = new Label();
            SuspendLayout();
            // 
            // deleteButton
            // 
            deleteButton.FlatStyle = FlatStyle.System;
            deleteButton.Location = new System.Drawing.Point(133, 88);
            deleteButton.Name = "deleteButton";
            deleteButton.Size = new System.Drawing.Size(85, 23);
            deleteButton.TabIndex = 2;
            deleteButton.Text = "&Delete Files";
            deleteButton.UseVisualStyleBackColor = true;
            deleteButton.Click += DeleteButtonClick;
            // 
            // openButton
            // 
            openButton.FlatStyle = FlatStyle.System;
            openButton.Location = new System.Drawing.Point(42, 88);
            openButton.Name = "openButton";
            openButton.Size = new System.Drawing.Size(85, 23);
            openButton.TabIndex = 3;
            openButton.Text = "&Open Files";
            openButton.UseVisualStyleBackColor = true;
            openButton.Click += OpenButtonClick;
            // 
            // notifyButton
            // 
            notifyButton.FlatStyle = FlatStyle.System;
            notifyButton.Location = new System.Drawing.Point(224, 88);
            notifyButton.Name = "notifyButton";
            notifyButton.Size = new System.Drawing.Size(85, 23);
            notifyButton.TabIndex = 1;
            notifyButton.Text = "&Notify Later";
            notifyButton.UseVisualStyleBackColor = true;
            notifyButton.Click += NotifyButtonClick;
            // 
            // infoLabel
            //
            infoLabel.Anchor = (AnchorStyles.Top | AnchorStyles.Left) | AnchorStyles.Right;
            infoLabel.FlatStyle = FlatStyle.System;
            infoLabel.Location = new System.Drawing.Point(13, 13);
            infoLabel.Name = "infoLabel";
            infoLabel.Size = new System.Drawing.Size(331, 66);
            infoLabel.TabIndex = 4;
            infoLabel.Text = "The recovery folder contains temporary backup files and this possibly means that your previous coding session was interrupted. \r\n\r\nWhat do you want to do to these files?";
            infoLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // RecoveryDialog
            // 
            AcceptButton = openButton;
            CancelButton = notifyButton;
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(356, 124);
            Controls.Add(notifyButton);
            Controls.Add(openButton);
            Controls.Add(deleteButton);
            Controls.Add(infoLabel);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "RecoveryDialog";
            StartPosition = FormStartPosition.CenterParent;
            Text = " Recovery File Notification";
            ResumeLayout(false);

        }

        #endregion

        #region Methods And Event Handlers

        /// <summary>
        /// Applies the localized texts to the form
        /// </summary>
        void ApplyLocalizedTexts()
        {
            openButton.Text = TextHelper.GetString("Label.OpenFiles");
            notifyButton.Text = TextHelper.GetString("Label.NotifyLater");
            deleteButton.Text = TextHelper.GetString("Label.DeleteFiles");
            Text = " " + TextHelper.GetString("Title.RecoveryDialog");
            string info = TextHelper.GetString("Info.RecoveryInfo");
            infoLabel.Text = string.Format(info, "\n\n");
        }

        /// <summary>
        /// Notifies the user next time with the same info
        /// </summary>
        void NotifyButtonClick(object sender, EventArgs e) => Close();

        /// <summary>
        /// Opens the files from the recovery files folder
        /// </summary>
        void OpenButtonClick(object sender, EventArgs e)
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
                DeleteButtonClick(null, null);
            }
            else Close();
        }

        /// <summary>
        /// Deletes the files from the recovery files folder
        /// </summary>
        void DeleteButtonClick(object sender, EventArgs e)
        {
            var files = GetRecoveryFiles();
            foreach (string file in files) FileHelper.Recycle(file);
            Close();
        }

        /// <summary>
        /// Gets the files from the recovery folder
        /// </summary>
        static string[] GetRecoveryFiles()
        {
            var path = Path.Combine(PathHelper.SettingDir, "Recovery");
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            return Directory.GetFiles(path);
        }

        /// <summary>
        /// Checks whether we should show the dialog
        /// </summary>
        public static bool ShouldShowDialog() => !SingleInstanceApp.AlreadyExists && GetRecoveryFiles().Length > 0;

        /// <summary>
        /// Shows the recovery dialog
        /// </summary>
        public new static void Show()
        {
            using var dialog = new RecoveryDialog();
            dialog.ShowDialog();
        }

        #endregion
    }
}