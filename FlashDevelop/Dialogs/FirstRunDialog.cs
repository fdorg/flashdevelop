using System;
using System.IO;
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
        Commands commands;
        ProgressBar progressBar;
        BackgroundWorker worker;
        PictureBox pictureBox;
        Label infoLabel;

        public FirstRunDialog()
        {
            Font = PluginBase.Settings.DefaultFont;
            InitializeComponent();
            InitializeExternals();
        }

        #region Windows Form Designer Generated Code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        void InitializeComponent()
        {
            infoLabel = new Label();
            progressBar = new ProgressBarEx();
            pictureBox = new PictureBox();
            ((ISupportInitialize)(pictureBox)).BeginInit();
            SuspendLayout();
            // 
            // infoLabel
            //
            infoLabel.Anchor = (AnchorStyles.Top | AnchorStyles.Left) | AnchorStyles.Right;
            infoLabel.BackColor = System.Drawing.SystemColors.Control;
            infoLabel.FlatStyle = FlatStyle.System;
            infoLabel.Location = new System.Drawing.Point(13, 36);
            infoLabel.Name = "infoLabel";
            infoLabel.Size = new System.Drawing.Size(361, 16);
            infoLabel.TabIndex = 0;
            infoLabel.Text = DistroConfig.DISTRIBUTION_NAME + " is initializing. Please wait...";
            infoLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // progressBar
            // 
            progressBar.Location = new System.Drawing.Point(13, 84);
            progressBar.Name = "progressBar";
            progressBar.Size = new System.Drawing.Size(361, 14);
            progressBar.TabIndex = 0;
            // 
            // pictureBox
            // 
            pictureBox.BorderStyle = BorderStyle.FixedSingle;
            pictureBox.Dock = DockStyle.Fill;
            pictureBox.Location = new System.Drawing.Point(0, 0);
            pictureBox.Name = "pictureBox";
            pictureBox.Size = new System.Drawing.Size(386, 110);
            pictureBox.TabIndex = 2;
            pictureBox.TabStop = false;
            // 
            // FirstRunDialog
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(386, 110);
            Controls.Add(progressBar);
            Controls.Add(infoLabel);
            Controls.Add(pictureBox);
            FormBorderStyle = FormBorderStyle.None;
            MaximumSize = new System.Drawing.Size(386, 110);
            MinimumSize = new System.Drawing.Size(386, 110);
            Name = "FirstRunDialog";
            ShowIcon = false;
            ShowInTaskbar = false;
            SizeGripStyle = SizeGripStyle.Hide;
            StartPosition = FormStartPosition.CenterScreen;
            Text = " Initializing...";
            Load += FirstRunDialogLoad;
            ((ISupportInitialize)(pictureBox)).EndInit();
            ResumeLayout(false);

        }

        #endregion

        #region Methods And Event Handlers

        /// <summary>
        /// Initializes the external images and texts
        /// </summary>
        void InitializeExternals() => infoLabel.Text = TextHelper.GetString("Info.Initializing");

        /// <summary>
        /// Handles the load event
        /// </summary>
        void FirstRunDialogLoad(object sender, EventArgs e)
        {
            LoadCommandsFile();
            worker = new BackgroundWorker {WorkerReportsProgress = true, WorkerSupportsCancellation = true};
            worker.DoWork += ProcessCommands;
            worker.ProgressChanged += ProgressChanged;
            worker.RunWorkerCompleted += WorkerCompleted;
            worker.RunWorkerAsync();
        }

        /// <summary>
        /// Forces the application to close
        /// </summary>
        void FirstRunDialogClick(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Abort;
            Close();
        }

        /// <summary>
        /// Updates the progress
        /// </summary>
        void ProgressChanged(object sender, ProgressChangedEventArgs e) => progressBar.Value = e.ProgressPercentage;

        /// <summary>
        /// Handles the finish of the work
        /// </summary>
        void WorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if ((bool)e.Result)
            {
                if (!File.Exists(FileNameHelper.SettingData))
                {
                    Globals.MainForm.AppSettings = SettingObject.GetDefaultSettings();
                }
                ((SettingObject)PluginBase.Settings).LatestCommand = commands.LatestCommand;
                Close();
            }
            else
            {
                infoLabel.Text = TextHelper.GetString("Info.InitFailed");
                pictureBox.Click += FirstRunDialogClick;
                progressBar.Click += FirstRunDialogClick;
                infoLabel.Click += FirstRunDialogClick;
            }
        }

        /// <summary>
        /// Processes the specified commands
        /// </summary>
        void ProcessCommands(object sender, DoWorkEventArgs e)
        {
            try
            {
                int count = 0;
                int total = commands.Entries.Count;
                foreach (Command command in commands.Entries)
                {
                    if (command.Number > ((SettingObject)PluginBase.Settings).LatestCommand)
                    {
                        string data = ProcessArguments(command.Data);
                        if (command.Action.ToLower() == "copy")
                        {
                            var args = data.Split(';');
                            if (Directory.Exists(args[0])) FolderHelper.CopyFolder(args[0], args[1]);
                            else
                            {
                                if (File.Exists(args[0]) && args.Length == 3 && args[2] == "keep-old") File.Copy(args[0], args[1], false);
                                else if (File.Exists(args[0])) File.Copy(args[0], args[1], true);
                            }
                        }
                        else if (command.Action.ToLower() == "move")
                        {
                            string[] args = data.Split(';');
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
                            var locale = PluginBase.Settings.LocaleVersion.ToString();
                            var appman = Path.Combine(PathHelper.ToolDir, "appman/AppMan.exe");
                            ProcessHelper.StartAsync(appman, "-locale=" + locale);
                        }
                    }
                    count++;
                    int percent = (100 * count) / total;
                    worker.ReportProgress(percent);
                }
                e.Result = true;
            }
            catch (Exception ex)
            {
                e.Result = false;
                ErrorManager.AddToLog("Init failed.", ex);
                worker.CancelAsync();
            }
        }

        /// <summary>
        /// Processes the default path arguments
        /// </summary>
        void LoadCommandsFile()
        {
            commands = new Commands();
            var filename = Path.Combine(PathHelper.AppDir, "FirstRun.fdb");
            commands = ObjectSerializer.Deserialize(filename, commands);
        }

        /// <summary>
        /// Processes the default path arguments
        /// </summary>
        static string ProcessArguments(string text)
        {
            var result = text;
            if (result is null) return string.Empty;
            result = result.Replace("$(AppDir)", PathHelper.AppDir);
            result = result.Replace("$(UserAppDir)", PathHelper.UserAppDir);
            result = result.Replace("$(BaseDir)", PathHelper.BaseDir);
            return result;
        }

        /// <summary>
        /// Checks if we should process the commands
        /// </summary>
        public static bool ShouldProcessCommands()
        {
            var filename = Path.Combine(PathHelper.AppDir, "FirstRun.fdb");
            if (!File.Exists(filename)) return false;
            var commands = new Commands();
            commands = ObjectSerializer.Deserialize(filename, commands);
            return commands.LatestCommand > ((SettingObject)PluginBase.Settings).LatestCommand;
        }

        /// <summary>
        /// Shows the first run dialog
        /// </summary>
        public new static DialogResult Show()
        {
            using var dialog = new FirstRunDialog();
            return dialog.ShowDialog();
        }

        #endregion

    }

    #region Command Classes

    [Serializable]
    public class Commands
    {
        public int LatestCommand = 0;
        public List<Command> Entries = new List<Command>();
    }

    [Serializable]
    public class Command
    {
        public int Number;
        public string Action;
        public string Data;

        public Command()
        {
        }

        public Command(int number, string action, string data)
        {
            Number = number;
            Action = action;
            Data = data;
        }

    }
    #endregion
}