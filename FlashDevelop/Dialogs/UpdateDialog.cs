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
        const string URL = DistroConfig.DISTRIBUTION_VERSION;
        UpdateInfo updateInfo;
        Label infoLabel;
        Button closeButton;
        Button downloadButton;
        BackgroundWorker worker;
        static bool silentCheck;

        public UpdateDialog()
        {
            Owner = Globals.MainForm;
            Font = PluginBase.Settings.DefaultFont;
            FormGuid = "4d5fdc1c-2698-46e9-b22d-fa9a42ba8d26";
            InitializeComponent();
            ApplyLocalizedTexts();
            InitializeUpdating();
        }

        #region Windows Form Designer Generated Code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        void InitializeComponent()
        {
            downloadButton = new ButtonEx();
            infoLabel = new Label();
            closeButton = new ButtonEx();
            SuspendLayout();
            // 
            // downloadButton
            //
            downloadButton.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left) | AnchorStyles.Right;
            downloadButton.Enabled = false;
            downloadButton.FlatStyle = FlatStyle.System;
            downloadButton.Location = new System.Drawing.Point(72, 69);
            downloadButton.Name = "downloadButton";
            downloadButton.Size = new System.Drawing.Size(116, 23);
            downloadButton.TabIndex = 2;
            downloadButton.Text = "&Download Update";
            downloadButton.UseVisualStyleBackColor = true;
            downloadButton.Click += DownloadButtonClick;
            // 
            // infoLabel
            //
            infoLabel.Anchor = (AnchorStyles.Top | AnchorStyles.Left) | AnchorStyles.Right;
            infoLabel.FlatStyle = FlatStyle.System;
            infoLabel.Location = new System.Drawing.Point(13, 13);
            infoLabel.Name = "infoLabel";
            infoLabel.Size = new System.Drawing.Size(327, 44);
            infoLabel.TabIndex = 0;
            infoLabel.Text = "There is a newer version of FlashDevelop available.\r\n\r\nYour version: *.*.*  /  Server version: *.*.*";
            infoLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // closeButton
            //
            closeButton.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left) | AnchorStyles.Right;
            closeButton.FlatStyle = FlatStyle.System;
            closeButton.Location = new System.Drawing.Point(194, 69);
            closeButton.Name = "closeButton";
            closeButton.Size = new System.Drawing.Size(84, 23);
            closeButton.TabIndex = 1;
            closeButton.Text = "&Close";
            closeButton.UseVisualStyleBackColor = true;
            closeButton.Click += CloseButtonClick;
            // 
            // UpdateDialog
            //
            CancelButton = closeButton;
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(353, 105);
            Controls.Add(closeButton);
            Controls.Add(downloadButton);
            Controls.Add(infoLabel);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "UpdateDialog";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = " Update Check";
            FormClosed += DialogClosed;
            ResumeLayout(false);

        }

        #endregion

        #region Methods And Event Handlers

        /// <summary>
        /// Applies the localized texts to the form
        /// </summary> 
        void ApplyLocalizedTexts()
        {
            Text = " " + TextHelper.GetString("Title.UpdateDialog");
            infoLabel.Text = TextHelper.GetString("Info.CheckingUpdates");
            downloadButton.Text = TextHelper.GetString("Label.DownloadUpdate");
            closeButton.Text = TextHelper.GetString("Label.Close");
        }

        /// <summary>
        /// Downloads the new flashdevelop release
        /// </summary>
        void DownloadButtonClick(object sender, EventArgs e)
        {
            try
            {
                ProcessHelper.StartAsync(updateInfo.DownloadUrl);
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
            Close();
        }

        /// <summary>
        /// When the form is closed cancel the update check
        /// </summary>
        void DialogClosed(object sender, FormClosedEventArgs e)
        {
            if (worker.IsBusy) worker.CancelAsync();
        }

        /// <summary>
        /// Closes the dialog when user clicks buttons
        /// </summary>
        void CloseButtonClick(object sender, EventArgs e) => Close();

        /// <summary>
        /// Startups the update check
        /// </summary>
        void InitializeUpdating()
        {
            worker = new BackgroundWorker();
            worker.DoWork += WorkerDoWork;
            worker.RunWorkerCompleted += WorkerCompleted;
            worker.WorkerSupportsCancellation = true;
            worker.RunWorkerAsync();
        }

        /// <summary>
        /// Does the actual work on background
        /// </summary>
        static void WorkerDoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                var request = WebRequest.Create(URL);
                var response = request.GetResponse();
                var stream = response.GetResponseStream();
                using var reader = new StreamReader(stream);
                var version = reader.ReadLine(); // Read version
                var download = reader.ReadLine(); // Read download
                var product = Application.ProductName; // Internal version
                var length = DistroConfig.DISTRIBUTION_NAME.Length + 1;
                var current = product.Substring(length, product.IndexOfOrdinal(" for") - length);
                stream.Close();
                response.Close(); // Close all resources
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
        void WorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            updateInfo = (UpdateInfo)e.Result;
            if (updateInfo is null)
            {
                string info = TextHelper.GetString("Info.UpdateCheckFailed");
                infoLabel.Text = string.Format(info, "\n\n");
                if (silentCheck) Close();
            }
            else if (updateInfo.NeedsUpdate)
            {
                downloadButton.Enabled = true;
                string info = TextHelper.GetString("Info.UpdateAvailable");
                infoLabel.Text = string.Format(info, "\n\n", updateInfo.UserVersion, updateInfo.ServerVersion);
                if (silentCheck) ShowDialog();
            }
            else
            {
                infoLabel.Text = TextHelper.GetString("Info.NoUpdateAvailable");
                if (silentCheck) Close();
            }
        }

        /// <summary>
        /// Shows the update dialog
        /// </summary>
        public static void Show(bool silent)
        {
            silentCheck = silent;
            using var dialog = new UpdateDialog();
            if (!silentCheck) dialog.ShowDialog();
        }

        #endregion

    }

    #region UpdateInfo

    public class UpdateInfo
    {
        public string UserVersion;
        public string ServerVersion;
        public string DownloadUrl = "http://www.flashdevelop.org/community/viewforum.php?f=11";
        public bool NeedsUpdate;

        public UpdateInfo(string userVersion, string serverVersion, string downloadUrl)
        {
            UserVersion = userVersion;
            ServerVersion = serverVersion;
            DownloadUrl = downloadUrl;
            ParseNeedsUpdate();
        }

        /// <summary>
        /// Parses the needs update value from the version strings
        /// </summary>
        void ParseNeedsUpdate()
        {
            try
            {
                NeedsUpdate = string.Compare(UserVersion, ServerVersion, true) < 0;
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

    }

    #endregion
}