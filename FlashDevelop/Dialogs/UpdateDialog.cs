using System;
using System.IO;
using System.Net;
using System.Text;
using System.Timers;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using System.ComponentModel;
using PluginCore.Localization;
using PluginCore.Managers;
using PluginCore.Helpers;

namespace FlashDevelop.Dialogs
{
    public class UpdateDialog : Form
    {
        private UpdateInfo updateInfo = null;
        private System.Windows.Forms.Label infoLabel;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.Button downloadButton;
        private System.ComponentModel.BackgroundWorker worker;
        private String URL = "http://www.flashdevelop.org/latest.txt";
        private static Boolean silentCheck = false;

        public UpdateDialog()
        {
            this.Owner = Globals.MainForm;
            this.Font = Globals.Settings.DefaultFont;
            this.InitializeComponent();
            this.ApplyLocalizedTexts();
            this.InitializeUpdating();
            ScaleHelper.AdjustForHighDPI(this);
        }

        #region Windows Form Designer Generated Code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.downloadButton = new System.Windows.Forms.Button();
            this.infoLabel = new System.Windows.Forms.Label();
            this.closeButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // downloadButton
            // 
            this.downloadButton.Enabled = false;
            this.downloadButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.downloadButton.Location = new System.Drawing.Point(72, 69);
            this.downloadButton.Name = "downloadButton";
            this.downloadButton.Size = new System.Drawing.Size(116, 23);
            this.downloadButton.TabIndex = 2;
            this.downloadButton.Text = "&Download Update";
            this.downloadButton.UseVisualStyleBackColor = true;
            this.downloadButton.Click += new System.EventHandler(this.DownloadButtonClick);
            // 
            // infoLabel
            //
            this.infoLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
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
            this.closeButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.closeButton.Location = new System.Drawing.Point(194, 69);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(84, 23);
            this.closeButton.TabIndex = 1;
            this.closeButton.Text = "&Close";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.CloseButtonClick);
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
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.DialogClosed);
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
        private void DownloadButtonClick(Object sender, EventArgs e)
        {
            try
            {
                String address = this.updateInfo.DownloadUrl;
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
        private void DialogClosed(Object sender, FormClosedEventArgs e)
        {
            if (this.worker.IsBusy)
            {
                this.worker.CancelAsync();
            }
        }

        /// <summary>
        /// Closes the dialog when user clicks buttons
        /// </summary>
        private void CloseButtonClick(Object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Startups the update check
        /// </summary>
        private void InitializeUpdating()
        {
            this.worker = new BackgroundWorker();
            this.worker.DoWork += new DoWorkEventHandler(this.WorkerDoWork);
            this.worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.WorkerCompleted);
            this.worker.WorkerSupportsCancellation = true;
            this.worker.RunWorkerAsync();
        }

        /// <summary>
        /// Does the actual work on background
        /// </summary>
        private void WorkerDoWork(Object sender, DoWorkEventArgs e)
        {
            try
            {
                WebRequest request = WebRequest.Create(URL);
                WebResponse response = request.GetResponse();
                Stream stream = response.GetResponseStream();
                StreamReader reader = new StreamReader(stream);
                String version = reader.ReadLine(); // Read version
                String download = reader.ReadLine(); // Read download
                String product = Application.ProductName; // Internal version
                String current = product.Substring(13, product.IndexOf(" for") - 13);
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
        private void WorkerCompleted(Object sender, RunWorkerCompletedEventArgs e)
        {
            this.updateInfo = (UpdateInfo)e.Result;
            if (this.updateInfo == null)
            {
                String info = TextHelper.GetString("Info.UpdateCheckFailed");
                String formatted = String.Format(info, "\n\n");
                this.infoLabel.Text = formatted;
                if (silentCheck) this.Close();
            }
            else if (this.updateInfo.NeedsUpdate)
            {
                this.downloadButton.Enabled = true;
                String info = TextHelper.GetString("Info.UpdateAvailable");
                String formatted = String.Format(info, "\n\n", this.updateInfo.UserVersion, this.updateInfo.ServerVersion);
                this.infoLabel.Text = formatted;
                if (silentCheck) this.ShowDialog();
            }
            else if (!this.updateInfo.NeedsUpdate)
            {
                String info = TextHelper.GetString("Info.NoUpdateAvailable");
                this.infoLabel.Text = info;
                if (silentCheck) this.Close();
            }
        }

        /// <summary>
        /// Shows the update dialog
        /// </summary>
        public static void Show(Boolean silent)
        {
            silentCheck = silent;
            UpdateDialog updateDialog = new UpdateDialog();
            if (!silentCheck) updateDialog.ShowDialog();
        }

        #endregion

    }

    #region UpdateInfo

    public class UpdateInfo
    {
        public String UserVersion = null;
        public String ServerVersion = null;
        public String DownloadUrl = "http://www.flashdevelop.org/community/viewforum.php?f=11";
        public Boolean NeedsUpdate = false;

        public UpdateInfo(String userVersion, String serverVersion, String downloadUrl)
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
                Int32 result = String.Compare(this.UserVersion, this.ServerVersion, true);
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