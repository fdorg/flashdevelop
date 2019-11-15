// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.IO;
using System.Net;
using System.Windows.Forms;
using System.ComponentModel;
using PluginCore.Localization;
using PluginCore.Managers;
using PluginCore.Helpers;
using PluginCore.Controls;
using PluginCore;

namespace FlashDevelop.Dialogs
{
    public class UpdateDialog : SmartForm
    {
        private UpdateInfo updateInfo = null;
        private System.Windows.Forms.Label infoLabel;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.Button downloadButton;
        private System.ComponentModel.BackgroundWorker worker;
        private readonly string URL = DistroConfig.DISTRIBUTION_VERSION;
        private static bool silentCheck = false;

        public UpdateDialog()
        {
            this.Owner = Globals.MainForm;
            this.Font = Globals.Settings.DefaultFont;
            this.FormGuid = "4d5fdc1c-2698-46e9-b22d-fa9a42ba8d26";
            this.InitializeComponent();
            this.ApplyLocalizedTexts();
            this.InitializeUpdating();
        }

        #region Windows Form Designer Generated Code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.downloadButton = new System.Windows.Forms.ButtonEx();
            this.infoLabel = new System.Windows.Forms.Label();
            this.closeButton = new System.Windows.Forms.ButtonEx();
            this.SuspendLayout();
            // 
            // downloadButton
            //
            this.downloadButton.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right;
            this.downloadButton.Enabled = false;
            this.downloadButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.downloadButton.Location = new System.Drawing.Point(72, 69);
            this.downloadButton.Name = "downloadButton";
            this.downloadButton.Size = new System.Drawing.Size(116, 23);
            this.downloadButton.TabIndex = 2;
            this.downloadButton.Text = "&Download Update";
            this.downloadButton.UseVisualStyleBackColor = true;
            this.downloadButton.Click += this.DownloadButtonClick;
            // 
            // infoLabel
            //
            this.infoLabel.Anchor = (System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right;
            this.infoLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.infoLabel.Location = new System.Drawing.Point(13, 13);
            this.infoLabel.Name = "infoLabel";
            this.infoLabel.Size = new System.Drawing.Size(327, 44);
            this.infoLabel.TabIndex = 0;
            this.infoLabel.Text = "There is a newer version of FlashDevelop available.\r\n\r\nYour version: *.*.*  /  Server version: *.*.*";
            this.infoLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // closeButton
            //
            this.closeButton.Anchor = (System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right;
            this.closeButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.closeButton.Location = new System.Drawing.Point(194, 69);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(84, 23);
            this.closeButton.TabIndex = 1;
            this.closeButton.Text = "&Close";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += this.CloseButtonClick;
            // 
            // UpdateDialog
            //
            this.CancelButton = this.closeButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(353, 105);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.downloadButton);
            this.Controls.Add(this.infoLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "UpdateDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = " Update Check";
            this.FormClosed += this.DialogClosed;
            this.ResumeLayout(false);

        }

        #endregion

        #region Methods And Event Handlers

        /// <summary>
        /// Applies the localized texts to the form
        /// </summary> 
        private void ApplyLocalizedTexts()
        {
            this.Text = " " + TextHelper.GetString("Title.UpdateDialog");
            this.infoLabel.Text = TextHelper.GetString("Info.CheckingUpdates");
            this.downloadButton.Text = TextHelper.GetString("Label.DownloadUpdate");
            this.closeButton.Text = TextHelper.GetString("Label.Close");
        }

        /// <summary>
        /// Downloads the new flashdevelop release
        /// </summary>
        private void DownloadButtonClick(object sender, EventArgs e)
        {
            try
            {
                string address = this.updateInfo.DownloadUrl;
                ProcessHelper.StartAsync(address);
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
            this.Close();
        }

        /// <summary>
        /// When the form is closed cancel the update check
        /// </summary>
        private void DialogClosed(object sender, FormClosedEventArgs e)
        {
            if (this.worker.IsBusy)
            {
                this.worker.CancelAsync();
            }
        }

        /// <summary>
        /// Closes the dialog when user clicks buttons
        /// </summary>
        private void CloseButtonClick(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Startups the update check
        /// </summary>
        private void InitializeUpdating()
        {
            this.worker = new BackgroundWorker();
            this.worker.DoWork += this.WorkerDoWork;
            this.worker.RunWorkerCompleted += this.WorkerCompleted;
            this.worker.WorkerSupportsCancellation = true;
            this.worker.RunWorkerAsync();
        }

        /// <summary>
        /// Does the actual work on background
        /// </summary>
        private void WorkerDoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                WebRequest request = WebRequest.Create(URL);
                WebResponse response = request.GetResponse();
                Stream stream = response.GetResponseStream();
                StreamReader reader = new StreamReader(stream);
                string version = reader.ReadLine(); // Read version
                string download = reader.ReadLine(); // Read download
                string product = Application.ProductName; // Internal version
                int lenght = DistroConfig.DISTRIBUTION_NAME.Length + 1;
                string current = product.Substring(lenght, product.IndexOfOrdinal(" for") - lenght);
                stream.Close(); response.Close(); // Close all resources
                e.Result = new UpdateInfo(current, version, download);
            }
            catch (Exception ex)
            {
                ErrorManager.AddToLog("Update check failed.", ex);
                e.Result = null;
            }
        }

        /// <summary>
        /// Handles the finish of the update check
        /// </summary>
        private void WorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.updateInfo = (UpdateInfo)e.Result;
            if (this.updateInfo is null)
            {
                string info = TextHelper.GetString("Info.UpdateCheckFailed");
                string formatted = string.Format(info, "\n\n");
                this.infoLabel.Text = formatted;
                if (silentCheck) this.Close();
            }
            else if (this.updateInfo.NeedsUpdate)
            {
                this.downloadButton.Enabled = true;
                string info = TextHelper.GetString("Info.UpdateAvailable");
                string formatted = string.Format(info, "\n\n", this.updateInfo.UserVersion, this.updateInfo.ServerVersion);
                this.infoLabel.Text = formatted;
                if (silentCheck) this.ShowDialog();
            }
            else
            {
                string info = TextHelper.GetString("Info.NoUpdateAvailable");
                this.infoLabel.Text = info;
                if (silentCheck) this.Close();
            }
        }

        /// <summary>
        /// Shows the update dialog
        /// </summary>
        public static void Show(bool silent)
        {
            silentCheck = silent;
            using UpdateDialog updateDialog = new UpdateDialog();
            if (!silentCheck) updateDialog.ShowDialog();
        }

        #endregion

    }

    #region UpdateInfo

    public class UpdateInfo
    {
        public string UserVersion = null;
        public string ServerVersion = null;
        public string DownloadUrl = "http://www.flashdevelop.org/community/viewforum.php?f=11";
        public bool NeedsUpdate = false;

        public UpdateInfo(string userVersion, string serverVersion, string downloadUrl)
        {
            this.UserVersion = userVersion;
            this.ServerVersion = serverVersion;
            this.DownloadUrl = downloadUrl;
            this.ParseNeedsUpdate();
        }

        /// <summary>
        /// Parses the needs update value from the version strings
        /// </summary>
        private void ParseNeedsUpdate()
        {
            try
            {
                int result = string.Compare(this.UserVersion, this.ServerVersion, true);
                this.NeedsUpdate = (result < 0);
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

    }

    #endregion

}