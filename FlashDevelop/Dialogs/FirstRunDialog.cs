using System;
using System.IO;
using System.Text;
using System.Drawing;
using System.Collections.Generic;
using PluginCore.Localization;
using System.ComponentModel;
using System.Windows.Forms;
using FlashDevelop.Managers;
using FlashDevelop.Helpers;
using FlashDevelop.Settings;
using PluginCore.Utilities;
using PluginCore.Managers;
using PluginCore.Helpers;
using PluginCore;

namespace FlashDevelop.Dialogs
{
    public class FirstRunDialog : Form
    {
        private FlashDevelop.Dialogs.Commands commands;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.ComponentModel.BackgroundWorker worker;
        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.Label infoLabel;

        public FirstRunDialog()
        {
            this.Font = Globals.Settings.DefaultFont;
            this.InitializeComponent();
            this.InitializeExternals();
        }

        #region Windows Form Designer Generated Code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.infoLabel = new System.Windows.Forms.Label();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // infoLabel
            //
            this.infoLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.infoLabel.BackColor = System.Drawing.SystemColors.Control;
            this.infoLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.infoLabel.Location = new System.Drawing.Point(13, 36);
            this.infoLabel.Name = "infoLabel";
            this.infoLabel.Size = new System.Drawing.Size(361, 16);
            this.infoLabel.TabIndex = 0;
            this.infoLabel.Text = DistroConfig.DISTRIBUTION_NAME + " is initializing. Please wait...";
            this.infoLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(13, 84);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(361, 14);
            this.progressBar.TabIndex = 0;
            // 
            // pictureBox
            // 
            this.pictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox.Location = new System.Drawing.Point(0, 0);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(386, 110);
            this.pictureBox.TabIndex = 2;
            this.pictureBox.TabStop = false;
            // 
            // FirstRunDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(386, 110);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.infoLabel);
            this.Controls.Add(this.pictureBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximumSize = new System.Drawing.Size(386, 110);
            this.MinimumSize = new System.Drawing.Size(386, 110);
            this.Name = "FirstRunDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = " Initializing...";
            this.Load += new System.EventHandler(this.FirstRunDialogLoad);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        #region Methods And Event Handlers

        /// <summary>
        /// Initializes the external images and texts
        /// </summary>
        private void InitializeExternals()
        {
            this.infoLabel.Text = TextHelper.GetString("Info.Initializing"); 
        }

        /// <summary>
        /// Handles the load event
        /// </summary>
        private void FirstRunDialogLoad(Object sender, EventArgs e)
        {
            this.LoadCommandsFile();
            this.worker = new BackgroundWorker();
            this.worker.WorkerReportsProgress = true;
            this.worker.WorkerSupportsCancellation = true;
            this.worker.DoWork += new DoWorkEventHandler(this.ProcessCommands);
            this.worker.ProgressChanged += new ProgressChangedEventHandler(this.ProgressChanged);
            this.worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.WorkerCompleted);
            this.worker.RunWorkerAsync();
        }

        /// <summary>
        /// Forces the application to close
        /// </summary>
        private void FirstRunDialogClick(Object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Abort;
            this.Close();
        }

        /// <summary>
        /// Updates the progress
        /// </summary>
        private void ProgressChanged(Object sender, ProgressChangedEventArgs e)
        {
            this.progressBar.Value = e.ProgressPercentage;
        }

        /// <summary>
        /// Handles the finish of the work
        /// </summary>
        private void WorkerCompleted(Object sender, RunWorkerCompletedEventArgs e)
        {
            if (((Boolean)e.Result))
            {
                if (!File.Exists(FileNameHelper.SettingData))
                {
                    SettingObject settings = SettingObject.GetDefaultSettings();
                    Globals.MainForm.AppSettings = settings;
                }
                Globals.Settings.LatestCommand = this.commands.LatestCommand;
                this.Close();
            }
            else
            {
                this.infoLabel.Text = TextHelper.GetString("Info.InitFailed");
                this.pictureBox.Click += new EventHandler(this.FirstRunDialogClick);
                this.progressBar.Click += new EventHandler(this.FirstRunDialogClick);
                this.infoLabel.Click += new EventHandler(this.FirstRunDialogClick);
            }
        }

        /// <summary>
        /// Processes the specified commands
        /// </summary>
        private void ProcessCommands(Object sender, DoWorkEventArgs e)
        {
            try
            {
                Int32 count = 0;
                Int32 total = this.commands.Entries.Count;
                foreach (Command command in this.commands.Entries)
                {
                    if (command.Number > Globals.Settings.LatestCommand)
                    {
                        String data = this.ProcessArguments(command.Data);
                        if (command.Action.ToLower() == "copy")
                        {
                            String[] args = data.Split(';');
                            if (Directory.Exists(args[0])) FolderHelper.CopyFolder(args[0], args[1]);
                            else
                            {
                                if (File.Exists(args[0]) && args.Length == 3 && args[2] == "keep-old") File.Copy(args[0], args[1], false);
                                else if (File.Exists(args[0])) File.Copy(args[0], args[1], true);
                            }
                        }
                        else if (command.Action.ToLower() == "move")
                        {
                            String[] args = data.Split(';');
                            if (Directory.Exists(args[0]))
                            {
                                FolderHelper.CopyFolder(args[0], args[1]);
                                Directory.Delete(args[0], true);
                            }
                            else if (File.Exists(args[0]))
                            {
                                File.Copy(args[0], args[1], true);
                                File.Delete(args[0]);
                            }
                        }
                        else if (command.Action.ToLower() == "delete")
                        {
                            if (Directory.Exists(data)) Directory.Delete(data, true);
                            else if (File.Exists(data)) File.Delete(data);
                        }
                        else if (command.Action.ToLower() == "syntax")
                        {
                            CleanupManager.RevertConfiguration(false);
                        }
                        else if (command.Action.ToLower() == "create")
                        {
                            Directory.CreateDirectory(data);
                        }
                        else if (command.Action.ToLower() == "appman")
                        {
                            String locale = Globals.Settings.LocaleVersion.ToString();
                            String appman = Path.Combine(PathHelper.ToolDir, "appman/AppMan.exe");
                            ProcessHelper.StartAsync(appman, "-locale=" + locale);
                        }
                    }
                    count++;
                    Int32 percent = (100 * count) / total;
                    this.worker.ReportProgress(percent);
                }
                e.Result = true;
            }
            catch (Exception ex)
            {
                e.Result = false;
                ErrorManager.AddToLog("Init failed.", ex);
                this.worker.CancelAsync();
            }
        }

        /// <summary>
        /// Processes the default path arguments
        /// </summary>
        private void LoadCommandsFile()
        {
            this.commands = new Commands();
            String filename = Path.Combine(PathHelper.AppDir, "FirstRun.fdb");
            Object obj = ObjectSerializer.Deserialize(filename, this.commands);
            this.commands = (Commands)obj;
        }

        /// <summary>
        /// Processes the default path arguments
        /// </summary>
        private String ProcessArguments(String text)
        {
            String result = text;
            if (result == null) return String.Empty;
            result = result.Replace("$(AppDir)", PathHelper.AppDir);
            result = result.Replace("$(UserAppDir)", PathHelper.UserAppDir);
            result = result.Replace("$(BaseDir)", PathHelper.BaseDir);
            return result;
        }

        /// <summary>
        /// Checks if we should process the commands
        /// </summary>
        public static Boolean ShouldProcessCommands()
        {
            Commands commands = new Commands();
            String filename = Path.Combine(PathHelper.AppDir, "FirstRun.fdb");
            if (File.Exists(filename))
            {
                commands = (Commands)ObjectSerializer.Deserialize(filename, commands);
                if (commands.LatestCommand > Globals.Settings.LatestCommand) return true;
                else return false;
            }
            else return false;
        }

        /// <summary>
        /// Shows the first run dialog
        /// </summary>
        public static new DialogResult Show()
        {
            FirstRunDialog firstRunDialog = new FirstRunDialog();
            return firstRunDialog.ShowDialog();
        }

        #endregion

    }

    #region Command Classes

    [Serializable]
    public class Commands
    {
        public Int32 LatestCommand = 0;
        public List<Command> Entries = new List<Command>();
    }

    [Serializable]
    public class Command
    {
        public Int32 Number;
        public String Action;
        public String Data;

        public Command(){}
        public Command(Int32 number, String action, String data)
        {
            this.Number = number;
            this.Action = action;
            this.Data = data;
        }

    }

    #endregion

}