using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using PluginCore.Localization;
using FlashDevelop.Utilities;
using PluginCore.Utilities;
using PluginCore.Controls;
using PluginCore.FRService;
using PluginCore.Managers;
using PluginCore.Helpers;
using Ookii.Dialogs;
using PluginCore;

namespace FlashDevelop.Dialogs
{
    public class FRInFilesDialog : SmartForm
    {
        Label infoLabel;
        Label findLabel;
        Label folderLabel;
        Label replaceLabel;
        Label extensionLabel;
        ColumnHeader lineHeader;
        ColumnHeader descHeader;
        ColumnHeader fileHeader;
        ColumnHeader pathHeader;
        ColumnHeader replacedHeader;
        ProgressBarEx progressBar;
        GroupBox optionsGroupBox;
        ComboBox folderComboBox;
        ComboBox extensionComboBox;
        ComboBox replaceComboBox;
        ComboBox findComboBox;
        CheckBox redirectCheckBox;
        CheckBox regexCheckBox;
        CheckBox escapedCheckBox;
        CheckBox wholeWordCheckBox;
        CheckBox matchCaseCheckBox;
        CheckBox stringsCheckBox;
        CheckBox commentsCheckBox;
        CheckBox subDirectoriesCheckBox;
        ListView resultsView;
        Button replaceButton;
        Button cancelButton;
        Button browseButton;
        Button closeButton;
        Button findButton;
        FRRunner runner;
        IProject lastProject;

        const string TraceGroup = "FindInFiles";
        
        public FRInFilesDialog()
        {
            Owner = Globals.MainForm;
            Font = PluginBase.MainForm.Settings.DefaultFont;
            FormGuid = "d2dbaf53-35ea-4632-b038-5428c9784a32";
            InitializeComponent();
            ApplyLocalizedTexts();
            InitializeGraphics();
            UpdateSettings();

            TraceManager.RegisterTraceGroup(TraceGroup, TextHelper.GetString("FlashDevelop.Label.FindAndReplaceResults"), false, true, PluginBase.MainForm.FindImage("209"));
        }

        #region Windows Form Designer Generated Code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        void InitializeComponent()
        {
            replaceButton = new ButtonEx();
            redirectCheckBox = new CheckBoxEx();
            regexCheckBox = new CheckBoxEx();
            optionsGroupBox = new GroupBoxEx();
            stringsCheckBox = new CheckBoxEx();
            commentsCheckBox = new CheckBoxEx();
            escapedCheckBox = new CheckBoxEx();
            wholeWordCheckBox = new CheckBoxEx();
            matchCaseCheckBox = new CheckBoxEx();
            subDirectoriesCheckBox = new CheckBoxEx();
            replaceComboBox = new FlatCombo();
            replaceLabel = new Label();
            findComboBox = new FlatCombo();
            findLabel = new Label();
            findButton = new ButtonEx();
            folderComboBox = new FlatCombo();
            folderLabel = new Label();
            extensionComboBox = new FlatCombo();
            extensionLabel = new Label();
            browseButton = new ButtonEx();
            replacedHeader = new ColumnHeader();
            cancelButton = new ButtonEx();
            infoLabel = new Label();
            lineHeader = new ColumnHeader();
            descHeader = new ColumnHeader();
            pathHeader = new ColumnHeader();
            fileHeader = new ColumnHeader();
            resultsView = new ListViewEx();
            progressBar = new ProgressBarEx();
            closeButton = new ButtonEx();
            optionsGroupBox.SuspendLayout();
            SuspendLayout();
            // 
            // replaceButton
            // 
            replaceButton.FlatStyle = FlatStyle.System;
            replaceButton.Location = new Point(113, 171);
            replaceButton.Name = "replaceButton";
            replaceButton.Size = new Size(90, 23);
            replaceButton.TabIndex = 8;
            replaceButton.Text = "&Replace";
            replaceButton.Click += ReplaceButtonClick;
            // 
            // regexCheckBox
            // 
            regexCheckBox.AutoSize = true;
            regexCheckBox.FlatStyle = FlatStyle.System;
            regexCheckBox.Location = new Point(12, 85);
            regexCheckBox.Name = "regexCheckBox";
            regexCheckBox.Size = new Size(132, 18);
            regexCheckBox.TabIndex = 4;
            regexCheckBox.Text = " &Regular expressions";
            // 
            // optionsGroupBox
            // 
            optionsGroupBox.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            optionsGroupBox.Controls.Add(stringsCheckBox);
            optionsGroupBox.Controls.Add(commentsCheckBox);
            optionsGroupBox.Controls.Add(regexCheckBox);
            optionsGroupBox.Controls.Add(escapedCheckBox);
            optionsGroupBox.Controls.Add(wholeWordCheckBox);
            optionsGroupBox.Controls.Add(matchCaseCheckBox);
            optionsGroupBox.Controls.Add(subDirectoriesCheckBox);
            optionsGroupBox.FlatStyle = FlatStyle.System;
            optionsGroupBox.Location = new Point(347, 15);
            optionsGroupBox.Name = "optionsGroupBox";
            optionsGroupBox.Size = new Size(156, 178);
            optionsGroupBox.TabIndex = 6;
            optionsGroupBox.TabStop = false;
            optionsGroupBox.Text = " Options";
            // 
            // stringsCheckBox
            // 
            stringsCheckBox.AutoSize = true;
            stringsCheckBox.Checked = true;
            stringsCheckBox.CheckState = CheckState.Checked;
            stringsCheckBox.FlatStyle = FlatStyle.System;
            stringsCheckBox.Location = new Point(12, 151);
            stringsCheckBox.Name = "stringsCheckBox";
            stringsCheckBox.Size = new Size(103, 18);
            stringsCheckBox.TabIndex = 7;
            stringsCheckBox.Text = " Look in &strings";
            // 
            // commentsCheckBox
            // 
            commentsCheckBox.AutoSize = true;
            commentsCheckBox.Checked = true;
            commentsCheckBox.CheckState = CheckState.Checked;
            commentsCheckBox.FlatStyle = FlatStyle.System;
            commentsCheckBox.Location = new Point(12, 129);
            commentsCheckBox.Name = "commentsCheckBox";
            commentsCheckBox.Size = new Size(119, 18);
            commentsCheckBox.TabIndex = 6;
            commentsCheckBox.Text = " Look in c&omments";
            // 
            // escapedCheckBox
            // 
            escapedCheckBox.AutoSize = true;
            escapedCheckBox.FlatStyle = FlatStyle.System;
            escapedCheckBox.Location = new Point(12, 63);
            escapedCheckBox.Name = "escapedCheckBox";
            escapedCheckBox.Size = new Size(129, 18);
            escapedCheckBox.TabIndex = 3;
            escapedCheckBox.Text = " &Escaped characters";
            // 
            // wholeWordCheckBox
            // 
            wholeWordCheckBox.AutoSize = true;
            wholeWordCheckBox.FlatStyle = FlatStyle.System;
            wholeWordCheckBox.Location = new Point(12, 19);
            wholeWordCheckBox.Name = "wholeWordCheckBox";
            wholeWordCheckBox.Size = new Size(92, 18);
            wholeWordCheckBox.TabIndex = 1;
            wholeWordCheckBox.Text = " &Whole word";
            // 
            // matchCaseCheckBox
            // 
            matchCaseCheckBox.AutoSize = true;
            matchCaseCheckBox.FlatStyle = FlatStyle.System;
            matchCaseCheckBox.Location = new Point(12, 41);
            matchCaseCheckBox.Name = "matchCaseCheckBox";
            matchCaseCheckBox.Size = new Size(89, 18);
            matchCaseCheckBox.TabIndex = 2;
            matchCaseCheckBox.Text = " Match &case";
            // 
            // subDirectoriesCheckBox
            // 
            subDirectoriesCheckBox.AutoSize = true;
            subDirectoriesCheckBox.Checked = true;
            subDirectoriesCheckBox.CheckState = CheckState.Checked;
            subDirectoriesCheckBox.FlatStyle = FlatStyle.System;
            subDirectoriesCheckBox.Location = new Point(12, 107);
            subDirectoriesCheckBox.Name = "subDirectoriesCheckBox";
            subDirectoriesCheckBox.Size = new Size(138, 18);
            subDirectoriesCheckBox.TabIndex = 5;
            subDirectoriesCheckBox.Text = " Look in sub&directories";
            // 
            // replaceComboBox
            // 
            replaceComboBox.Anchor = (AnchorStyles.Top | AnchorStyles.Left) | AnchorStyles.Right;
            replaceComboBox.FlatStyle = FlatStyle.System;
            replaceComboBox.Location = new Point(12, 61);
            replaceComboBox.Name = "replaceComboBox";
            replaceComboBox.Size = new Size(324, 21);
            replaceComboBox.TabIndex = 2;
            // 
            // replaceLabel
            // 
            replaceLabel.AutoSize = true;
            replaceLabel.FlatStyle = FlatStyle.System;
            replaceLabel.Location = new Point(13, 46);
            replaceLabel.Name = "replaceLabel";
            replaceLabel.Size = new Size(72, 13);
            replaceLabel.TabIndex = 24;
            replaceLabel.Text = "Replace with:";
            // 
            // findComboBox
            // 
            findComboBox.Anchor = (AnchorStyles.Top | AnchorStyles.Left) | AnchorStyles.Right;
            findComboBox.FlatStyle = FlatStyle.System;
            findComboBox.Location = new Point(12, 22);
            findComboBox.Name = "findComboBox";
            findComboBox.Size = new Size(324, 21);
            findComboBox.TabIndex = 1;
            // 
            // findLabel
            // 
            findLabel.AutoSize = true;
            findLabel.FlatStyle = FlatStyle.System;
            findLabel.Location = new Point(13, 7);
            findLabel.Name = "findLabel";
            findLabel.Size = new Size(58, 13);
            findLabel.TabIndex = 17;
            findLabel.Text = "Find what:";
            // 
            // findButton
            // 
            findButton.FlatStyle = FlatStyle.System;
            findButton.Location = new Point(11, 171);
            findButton.Name = "findButton";
            findButton.Size = new Size(90, 23);
            findButton.TabIndex = 7;
            findButton.Text = "&Find";
            findButton.Click += FindButtonClick;
            // 
            // folderComboBox
            // 
            folderComboBox.Anchor = (AnchorStyles.Top | AnchorStyles.Left) | AnchorStyles.Right;
            folderComboBox.FlatStyle = FlatStyle.System;
            folderComboBox.Location = new Point(12, 139);
            folderComboBox.Name = "folderComboBox";
            folderComboBox.Size = new Size(293, 21);
            folderComboBox.TabIndex = 4;
            folderComboBox.Text = "<Project>";
            folderComboBox.TextChanged += (sender, args) =>
            {
                var backColor = findComboBox.BackColor;
                var buttonsEnabled = true;
                var isValidTopLevel = IsValidTopLevel();
                if (!IsValidExtensionFilter() || !isValidTopLevel)
                {
                    if (!isValidTopLevel && folderComboBox.Text.Trim().Length > 0) backColor = Color.Salmon;
                    buttonsEnabled = false;
                }
                folderComboBox.BackColor = backColor;
                findButton.Enabled = buttonsEnabled;
                replaceButton.Enabled = buttonsEnabled;
            };
            // 
            // folderLabel
            // 
            folderLabel.AutoSize = true;
            folderLabel.FlatStyle = FlatStyle.System;
            folderLabel.Location = new Point(13, 124);
            folderLabel.Name = "folderLabel";
            folderLabel.Size = new Size(60, 13);
            folderLabel.TabIndex = 28;
            folderLabel.Text = "Top folder:";
            // 
            // extensionComboBox
            // 
            extensionComboBox.Anchor = (AnchorStyles.Top | AnchorStyles.Left) | AnchorStyles.Right;
            extensionComboBox.FlatStyle = FlatStyle.System;
            extensionComboBox.Location = new Point(12, 100);
            extensionComboBox.Name = "extensionComboBox";
            extensionComboBox.Size = new Size(324, 21);
            extensionComboBox.TabIndex = 3;
            extensionComboBox.Text = ".ext";
            extensionComboBox.TextChanged += (sender, args) =>
            {
                var backColor = findComboBox.BackColor;
                var buttonsEnabled = true;
                var isValidExtensionFilter = IsValidExtensionFilter();
                if (!isValidExtensionFilter || !IsValidTopLevel())
                {
                    if (!isValidExtensionFilter && extensionComboBox.Text.Trim().Length > 0) backColor = Color.Salmon;
                    buttonsEnabled = false;
                }
                extensionComboBox.BackColor = backColor;
                findButton.Enabled = buttonsEnabled;
                replaceButton.Enabled = buttonsEnabled;
            };
            // 
            // extensionLabel
            // 
            extensionLabel.AutoSize = true;
            extensionLabel.FlatStyle = FlatStyle.System;
            extensionLabel.Location = new Point(13, 85);
            extensionLabel.Name = "extensionLabel";
            extensionLabel.Size = new Size(83, 13);
            extensionLabel.TabIndex = 25;
            extensionLabel.Text = "Extension filter:";
            // 
            // browseButton
            // 
            browseButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            browseButton.Location = new Point(311, 136);
            browseButton.Name = "browseButton";
            browseButton.Size = new Size(26, 24);
            browseButton.TabIndex = 5;
            browseButton.Click += BrowseButtonClick;
            // 
            // replacedHeader
            // 
            replacedHeader.Text = "Replacements";
            replacedHeader.Width = 120;
            // 
            // cancelButton
            //
            cancelButton.FlatStyle = FlatStyle.System;
            cancelButton.Location = new Point(215, 171);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new Size(90, 23);
            cancelButton.TabIndex = 9;
            cancelButton.Text = "Ca&ncel";
            cancelButton.Click += CancelButtonClick;
            // 
            // infoLabel
            // 
            infoLabel.AutoSize = true;
            infoLabel.BackColor = SystemColors.Control;
            infoLabel.FlatStyle = FlatStyle.System;
            infoLabel.ForeColor = SystemColors.ControlText;
            infoLabel.Location = new Point(14, 203);
            infoLabel.Name = "infoLabel";
            infoLabel.Size = new Size(130, 13);
            infoLabel.TabIndex = 0;
            infoLabel.Text = "No suitable results found.";
            infoLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lineHeader
            // 
            lineHeader.Text = "Line";
            lineHeader.Width = 50;
            // 
            // descHeader
            // 
            descHeader.Text = "Description";
            descHeader.Width = 120;
            // 
            // fileHeader
            // 
            fileHeader.Text = "File";
            fileHeader.Width = 120;
            // 
            // pathHeader
            // 
            pathHeader.Text = "Path";
            pathHeader.Width = 220;
            // 
            // resultsView
            // 
            resultsView.Anchor = ((AnchorStyles.Top | AnchorStyles.Bottom) | AnchorStyles.Left) | AnchorStyles.Right;
            resultsView.Columns.AddRange(new[] {
            lineHeader,
            descHeader,
            fileHeader,
            pathHeader});
            resultsView.ShowGroups = true;
            resultsView.LabelWrap = false;
            resultsView.FullRowSelect = true;
            resultsView.GridLines = true;
            resultsView.Location = new Point(12, 219);
            resultsView.Name = "resultsView";
            resultsView.ShowItemToolTips = true;
            resultsView.Size = new Size(492, 167);
            resultsView.TabIndex = 11;
            resultsView.UseCompatibleStateImageBehavior = false;
            resultsView.View = View.Details;
            resultsView.DoubleClick += ResultsViewDoubleClick;
            // 
            // progressBar
            // 
            progressBar.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left) | AnchorStyles.Right;
            progressBar.Location = new Point(12, 392);
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(491, 14);
            progressBar.TabIndex = 0;
            // 
            // closeButton
            //
            closeButton.TabStop = false;
            closeButton.FlatStyle = FlatStyle.System;
            closeButton.Location = new Point(0, 0);
            closeButton.Name = "closeButton";
            closeButton.Size = new Size(0, 0);
            closeButton.TabIndex = 29;
            closeButton.Click += CloseButtonClick;
            // 
            // sendCheckBox
            // 
            redirectCheckBox.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom) | AnchorStyles.Right;
            redirectCheckBox.AutoSize = true;
            redirectCheckBox.Checked = true;
            redirectCheckBox.CheckState = CheckState.Checked;
            redirectCheckBox.FlatStyle = FlatStyle.System;
            redirectCheckBox.Location = new Point(273, 201);
            redirectCheckBox.Name = "redirectCheckBox";
            redirectCheckBox.RightToLeft = RightToLeft.Yes;
            redirectCheckBox.Size = new Size(230, 25);
            redirectCheckBox.TabIndex = 8;
            redirectCheckBox.Text = " Send results to Results Panel";
            redirectCheckBox.CheckedChanged += RedirectCheckBoxCheckChanged;
            // 
            // FRInFilesDialog
            // 
            AcceptButton = findButton;
            CancelButton = closeButton;
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(515, 418);
            Controls.Add(infoLabel);
            Controls.Add(cancelButton);
            Controls.Add(resultsView);
            Controls.Add(browseButton);
            Controls.Add(progressBar);
            Controls.Add(folderComboBox);
            Controls.Add(folderLabel);
            Controls.Add(extensionComboBox);
            Controls.Add(extensionLabel);
            Controls.Add(replaceButton);
            Controls.Add(optionsGroupBox);
            Controls.Add(replaceComboBox);
            Controls.Add(replaceLabel);
            Controls.Add(findComboBox);
            Controls.Add(findLabel);
            Controls.Add(findButton);
            Controls.Add(closeButton);
            Controls.Add(redirectCheckBox);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FRInFilesDialog";
            ShowIcon = false;
            ShowInTaskbar = false;
            MinimumSize = new Size(500, 340);
            SizeGripStyle = SizeGripStyle.Show;
            StartPosition = FormStartPosition.CenterParent;
            Text = " Find And Replace In Files";
            Load += DialogLoaded;
            VisibleChanged += VisibleChange;
            Closing += DialogClosing;
            optionsGroupBox.ResumeLayout(false);
            optionsGroupBox.PerformLayout();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        #region Methods And Event Handlers

        /// <summary>
        /// Applies the settings to the UI
        /// </summary>
        public void UpdateSettings()
        {
            FRDialogGenerics.UpdateComboBoxItems(folderComboBox);
            bool useGroups = PluginBase.MainForm.Settings.UseListViewGrouping;
            resultsView.ShowGroups = useGroups;
            resultsView.GridLines = !useGroups;
        }

        /// <summary>
        /// Initializes the external graphics
        /// </summary>
        void InitializeGraphics()
        {
            ImageList imageList = new ImageList();
            imageList.ColorDepth = ColorDepth.Depth32Bit;
            imageList.ImageSize = ScaleHelper.Scale(new Size(16, 16));
            imageList.Images.Add(PluginBase.MainForm.FindImage("203", false));
            browseButton.ImageList = imageList;
            browseButton.ImageIndex = 0;
        }

        /// <summary>
        /// Applies the localized texts to the form
        /// </summary>
        void ApplyLocalizedTexts()
        {
            findLabel.Text = TextHelper.GetString("Info.FindWhat");
            extensionLabel.Text = TextHelper.GetString("Info.ExtensionFilter");
            folderLabel.Text = TextHelper.GetString("Info.TopFolder");
            replaceLabel.Text = TextHelper.GetString("Info.ReplaceWith");
            findButton.Text = TextHelper.GetString("Label.Find");
            cancelButton.Text = TextHelper.GetString("Label.Cancel");
            replaceButton.Text = TextHelper.GetStringWithoutEllipsis("Label.Replace");
            lineHeader.Text = TextHelper.GetString("Info.LineHeader");
            descHeader.Text = TextHelper.GetString("Info.DescHeader");
            pathHeader.Text = TextHelper.GetString("Info.PathHeader");
            fileHeader.Text = TextHelper.GetString("Info.FileHeader");
            replacedHeader.Text = TextHelper.GetString("Info.ReplacementsHeader");
            optionsGroupBox.Text = " " + TextHelper.GetString("Label.Options");
            matchCaseCheckBox.Text = " " + TextHelper.GetString("Label.MatchCase");
            wholeWordCheckBox.Text = " " + TextHelper.GetString("Label.WholeWord");
            escapedCheckBox.Text = " " + TextHelper.GetString("Label.EscapedCharacters");
            regexCheckBox.Text = " " + TextHelper.GetString("Label.RegularExpressions");
            subDirectoriesCheckBox.Text = " " + TextHelper.GetString("Label.LookInSubdirectories");
            redirectCheckBox.Text = " " + TextHelper.GetString("Info.RedirectFilesResults");
            commentsCheckBox.Text = " " + TextHelper.GetString("Label.LookInComments");
            stringsCheckBox.Text = " " + TextHelper.GetString("Label.LookInStrings");
            Text = " " + TextHelper.GetString("Title.FindAndReplaceInFilesDialog");
            pathHeader.Width = -2; // Extend last column
        }

        /// <summary>
        /// Changes the redirection setting and updates the check state.
        /// </summary>
        void RedirectCheckBoxCheckChanged(object sender, EventArgs e)
        {
            PluginBase.MainForm.Settings.RedirectFilesResults = !PluginBase.MainForm.Settings.RedirectFilesResults;
            redirectCheckBox.Checked = PluginBase.MainForm.Settings.RedirectFilesResults;
        }

        /// <summary>
        /// Runs the find based on the user specified arguments
        /// </summary>
        void FindButtonClick(object sender, EventArgs e)
        {
            if (!IsValidPattern()) return;
            var mask = extensionComboBox.Text.Trim();
            if (!IsValidFileMask(mask)) return;
            var recursive = subDirectoriesCheckBox.Checked;
            var paths = folderComboBox.Text.Trim().Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries);
            foreach (var path in paths)
            {
                var config = GetFRConfig(path, mask, recursive);
                if (config is null) return;
                config.CacheDocuments = true;
                UpdateUIState(true);
                runner = new FRRunner();
                runner.ProgressReport += RunnerProgress;
                runner.Finished += FindFinished;
                runner.SearchAsync(config);
                FRDialogGenerics.UpdateComboBoxItems(folderComboBox);
                FRDialogGenerics.UpdateComboBoxItems(extensionComboBox);
                FRDialogGenerics.UpdateComboBoxItems(findComboBox);
            }
        }

        /// <summary>
        /// Runs the replace based on the user specified arguments
        /// </summary>
        void ReplaceButtonClick(object sender, EventArgs e)
        {
            if (!IsValidPattern()) return;
            var mask = extensionComboBox.Text.Trim();
            if (!IsValidFileMask(mask)) return;
            if (!PluginBase.MainForm.Settings.DisableReplaceFilesConfirm)
            {
                var caption = TextHelper.GetString("Title.ConfirmDialog");
                var message = TextHelper.GetString("Info.AreYouSureToReplaceInFiles");
                var result = MessageBox.Show(message, caption, MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (result == DialogResult.Cancel) return;
            }
            var recursive = subDirectoriesCheckBox.Checked;
            var paths = folderComboBox.Text.Trim().Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries);
            foreach (var path in paths)
            {
                var config = GetFRConfig(path, mask, recursive);
                if (config is null) return;
                config.CacheDocuments = true;
                config.UpdateSourceFileOnly = false;
                config.Replacement = replaceComboBox.Text;
                UpdateUIState(true);
                runner = new FRRunner();
                runner.ProgressReport += RunnerProgress;
                runner.Finished += ReplaceFinished;
                runner.ReplaceAsync(config);
                FRDialogGenerics.UpdateComboBoxItems(folderComboBox);
                FRDialogGenerics.UpdateComboBoxItems(extensionComboBox);
                FRDialogGenerics.UpdateComboBoxItems(replaceComboBox);
                FRDialogGenerics.UpdateComboBoxItems(findComboBox);
            }
        }

        /// <summary>
        /// Closes the dialog window
        /// </summary>
        void CloseButtonClick(object sender, EventArgs e) => Close();

        /// <summary>
        /// Selects the path for the find and replace
        /// </summary>
        void BrowseButtonClick(object sender, EventArgs e)
        {
            using var dialog = new VistaFolderBrowserDialog {Multiselect = true};
            var curDir = folderComboBox.Text.Trim();
            if (curDir == "<Project>")
            {
                curDir = PluginBase.CurrentProject is null
                    ? PluginBase.MainForm.WorkingDirectory
                    : Path.GetDirectoryName(PluginBase.CurrentProject.ProjectPath);
            }
            if (Directory.Exists(curDir)) dialog.SelectedPath = curDir;
            if (dialog.ShowDialog() == DialogResult.OK && Directory.Exists(dialog.SelectedPath))
            {
                folderComboBox.Text = string.Join(";", dialog.SelectedPaths);
                folderComboBox.SelectionStart = folderComboBox.Text.Length;
            }
        }

        /// <summary>
        /// Handles the double click event from results view
        /// </summary>
        void ResultsViewDoubleClick(object sender, EventArgs e)
        {
            if (resultsView.SelectedItems.Count == 0) return;
            var item = resultsView.SelectedItems[0];
            var data = (KeyValuePair<string, SearchMatch>)item.Tag;
            if (!File.Exists(data.Key)) return;
            Globals.MainForm.Activate();
            var doc = PluginBase.MainForm.OpenEditableDocument(data.Key, false) as ITabbedDocument;
            if (doc?.SciControl is {} sci && resultsView.Columns.Count == 4)
            {
                FRDialogGenerics.SelectMatch(sci, data.Value);
            }
        }

        /// <summary>
        /// Cancels the find or replace lookup
        /// </summary>
        void CancelButtonClick(object sender, EventArgs e) => runner?.CancelAsync();

        /// <summary>
        /// Runner reports how much of the lookup is done
        /// </summary>
        void RunnerProgress(int percentDone) => progressBar.Value = percentDone;

        /// <summary>
        /// Handles the results when find is ready
        /// </summary>
        void FindFinished(FRResults results)
        {
            SetupResultsView(true);
            resultsView.Items.Clear();
            if (results is null)
            {
                string message = TextHelper.GetString("Info.FindLookupCanceled");
                infoLabel.Text = message;
            }
            else if (results.Count == 0)
            {
                string message = TextHelper.GetString("Info.NoMatchesFound");
                infoLabel.Text = message;
            }
            else
            {
                if (!redirectCheckBox.Checked)
                {
                    resultsView.BeginUpdate();
                    var fileCount = 0;
                    var matchCount = 0;
                    foreach (var entry in results)
                    {
                        fileCount++;
                        foreach (var match in entry.Value)
                        {
                            matchCount++;
                            var item = new ListViewItem();
                            item.Text = match.Line.ToString();
                            item.SubItems.Add(match.LineText.Trim());
                            item.SubItems.Add(Path.GetFileName(entry.Key));
                            item.SubItems.Add(Path.GetDirectoryName(entry.Key));
                            item.Tag = new KeyValuePair<string, SearchMatch>(entry.Key, match);
                            resultsView.Items.Add(item);
                            AddToGroup(item, entry.Key);
                        }
                    }
                    var message = TextHelper.GetString("Info.FoundInFiles");
                    var formatted = string.Format(message, matchCount, fileCount);
                    infoLabel.Text = formatted;
                    resultsView.EndUpdate();
                } 
                else 
                {
                    var groupData = TraceManager.CreateGroupDataUnique(TraceGroup);
                    PluginBase.MainForm.CallCommand("PluginCommand", "ResultsPanel.ClearResults;" + groupData);
                    foreach (var entry in results)
                    {
                        foreach (var match in entry.Value)
                        {
                            var message = $"{entry.Key}:{match.Line}: chars {match.Column}-{match.Column + match.Length} : {match.LineText.Trim()}";
                            TraceManager.Add(message, (int)TraceType.Info, groupData);
                        }
                    }
                    PluginBase.MainForm.CallCommand("PluginCommand", "ResultsPanel.ShowResults;" + groupData);
                    Hide();
                }
            }
            UpdateUIState(false);
        }

        /// <summary>
        /// Handles the results when replace is ready
        /// </summary>
        void ReplaceFinished(FRResults results)
        {
            SetupResultsView(false);
            resultsView.Items.Clear();
            if (results is null)
            {
                string message = TextHelper.GetString("Info.ReplaceLookupCanceled");
                infoLabel.Text = message;
            }
            else if (results.Count == 0)
            {
                string message = TextHelper.GetString("Info.NoMatchesFound");
                infoLabel.Text = message;
            }
            else
            {
                if (!redirectCheckBox.Checked)
                {
                    resultsView.BeginUpdate();
                    var fileCount = 0;
                    var matchCount = 0;
                    foreach (var entry in results)
                    {
                        fileCount++;
                        var replaceCount = entry.Value.Count;
                        if (replaceCount == 0) continue;
                        matchCount += replaceCount;
                        var item = new ListViewItem();
                        item.Tag = new KeyValuePair<string, SearchMatch>(entry.Key, null);
                        item.Text = replaceCount.ToString();
                        item.SubItems.Add(Path.GetFileName(entry.Key));
                        item.SubItems.Add(Path.GetDirectoryName(entry.Key));
                        resultsView.Items.Add(item);
                        AddToGroup(item, entry.Key);
                    }
                    var message = TextHelper.GetString("Info.ReplacedInFiles");
                    var formatted = string.Format(message, matchCount, fileCount);
                    infoLabel.Text = formatted;
                    resultsView.EndUpdate();
                } 
                else
                {
                    var groupData = TraceManager.CreateGroupDataUnique(TraceGroup);
                    PluginBase.MainForm.CallCommand("PluginCommand", "ResultsPanel.ClearResults;" + groupData);
                    foreach (var entry in results)
                    {
                        foreach (var match in entry.Value)
                        {
                            var message = $"{entry.Key}:{match.Line}: chars {match.Column}-{match.Column + match.Length} : {match.Value}";
                            TraceManager.Add(message, (int)TraceType.Info, groupData);
                        }
                    }
                    PluginBase.MainForm.CallCommand("PluginCommand", "ResultsPanel.ShowResults;" + groupData);
                    Hide();
                }
            }
            UpdateUIState(false);
        }

        /// <summary>
        /// Adds item to the specified group
        /// </summary>
        void AddToGroup(ListViewItem item, string path)
        {
            foreach (ListViewGroup lvg in resultsView.Groups)
            {
                if (lvg.Tag.ToString() == path)
                {
                    lvg.Items.Add(item);
                    return;
                }
            }

            var gp = new ListViewGroup();
            gp.Tag = path;
            gp.Header = File.Exists(path)
                ? Path.GetFileName(path)
                : TextHelper.GetString("Group.Other");
            resultsView.Groups.Add(gp);
            gp.Items.Add(item);
        }

        /// <summary>
        /// Setups the resultsView.Columns for find and replace
        /// </summary>
        void SetupResultsView(bool finding)
        {
            if (finding && resultsView.Columns.Count != 4)
            {
                resultsView.Columns.Clear();
                resultsView.Columns.AddRange(new[] { lineHeader, descHeader, fileHeader, pathHeader });
            }
            else if (!finding && resultsView.Columns.Count != 3)
            {
                resultsView.Columns.Clear();
                resultsView.Columns.AddRange(new[] { replacedHeader, fileHeader, pathHeader });
            }
        }

        /// <summary>
        /// Just hides the dialog window when closing
        /// </summary>
        void DialogClosing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            runner?.CancelAsync();
            PluginBase.MainForm.CurrentDocument.Activate();
            Hide();
        }
        
        /// <summary>
        /// Setups the dialog on load
        /// </summary>
        void DialogLoaded(object sender, EventArgs e)
        {
            cancelButton.Enabled = false;
            infoLabel.Text = TextHelper.GetString("Info.NoMatches");
            CenterToParent();
        }

        /// <summary>
        /// Process shortcuts
        /// </summary>
        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.F))
            {
                findComboBox.Focus();
                return true;
            }
            return base.ProcessDialogKey(keyData);
        }

        /// <summary>
        /// Some event handling when showing the form
        /// </summary>
        void VisibleChange(object sender, EventArgs e)
        {
            if (Visible)
            {
                findComboBox.Select();
                findComboBox.SelectAll();
            }
        }

        /// <summary>
        /// Checks if the file mask is somewhat valid
        /// </summary>
        bool IsValidFileMask(string mask)
        {
            return !string.IsNullOrEmpty(mask) && mask.Trim().StartsWithOrdinal("*.") && !mask.Contains("..")
                   && !mask.Contains('/') && !mask.Contains('\\') && !mask.Contains(':') && Path.GetInvalidPathChars().All(it => !mask.Contains(it));
        }

        /// <summary>
        /// Updates the file extension and the initial directory
        /// </summary>
        void UpdateDialogArguments()
        {
            var project = PluginBase.CurrentProject;
            var doRefresh = lastProject != null && lastProject != project;
            if (project != null)
            {
                string path = Path.GetDirectoryName(project.ProjectPath);
                if (string.IsNullOrEmpty(folderComboBox.Text) || doRefresh)
                {
                    folderComboBox.Text = path;
                }
            }
            else if (string.IsNullOrEmpty(folderComboBox.Text) || doRefresh)
            {
                folderComboBox.Text = PluginBase.MainForm.WorkingDirectory;
            }
            folderComboBox.SelectionStart = folderComboBox.Text.Length;
            redirectCheckBox.CheckedChanged -= RedirectCheckBoxCheckChanged;
            redirectCheckBox.Checked = PluginBase.MainForm.Settings.RedirectFilesResults;
            redirectCheckBox.CheckedChanged += RedirectCheckBoxCheckChanged;
            if (!IsValidFileMask(extensionComboBox.Text) || doRefresh)
            {
                if (project != null)
                {
                    string filter = project.DefaultSearchFilter;
                    extensionComboBox.Text = filter;
                }
                else
                {
                    string def = PluginBase.MainForm.Settings.DefaultFileExtension;
                    extensionComboBox.Text = "*." + def;
                }
            }
            UpdateFindText();
            if (project != null) lastProject = project;
        }

        /// <summary>
        /// Updates the ui based on the running state
        /// </summary> 
        void UpdateUIState(bool running)
        {
            if (running)
            {
                cancelButton.Enabled = true;
                replaceButton.Enabled = false;
                findButton.Enabled = false;
            } 
            else 
            {
                cancelButton.Enabled = false;
                replaceButton.Enabled = true;
                findButton.Enabled = true;
            }
            progressBar.Value = 0;
        }

        /// <summary>
        /// Gets search config for find and replace
        /// </summary>
        FRConfiguration GetFRConfig(string path, string mask, bool recursive)
        {
            if (path.Trim() != "<Project>") return new FRConfiguration(path, mask, recursive, GetFRSearch());
            if (PluginBase.CurrentProject is null) return null;
            var allFiles = new List<string>();
            var project = PluginBase.CurrentProject;
            var projPath = Path.GetDirectoryName(project.ProjectPath);
            var walker = new PathWalker(projPath, mask, recursive);
            var projFiles = walker.GetFiles();
            foreach (string file in projFiles)
            {
                if (!IsFileHidden(file, project)) allFiles.Add(file);
            }
            foreach (var sp in project.SourcePaths)
            {
                string sourcePath = project.GetAbsolutePath(sp);
                if (Directory.Exists(sourcePath) && !sourcePath.StartsWithOrdinal(projPath))
                {
                    walker = new PathWalker(sourcePath, mask, recursive);
                    allFiles.AddRange(walker.GetFiles());
                }
            }
            return new FRConfiguration(allFiles, GetFRSearch());
        }

        /// <summary>
        /// Check if file is hidden in project
        /// </summary>
        bool IsFileHidden(string file, IProject project)
        {
            string[] hiddenPaths = project.GetHiddenPaths();
            foreach (string hiddenPath in hiddenPaths)
            {
                string absHiddenPath = project.GetAbsolutePath(hiddenPath);
                if (Directory.Exists(absHiddenPath) && file.StartsWithOrdinal(absHiddenPath)) return true;
            }
            return false;
        }

        /// <summary>
        /// Control user pattern
        /// </summary>
        bool IsValidPattern()
        {
            string pattern = findComboBox.Text;
            if (pattern.Length == 0) return false;
            if (regexCheckBox.Checked)
            {
                try
                {
                    new Regex(pattern);
                }
                catch (Exception ex)
                {
                    ErrorManager.ShowInfo(ex.Message);
                    Select();
                    findComboBox.SelectAll();
                    return false;
                }
            }
            return true;
        }

        bool IsValidExtensionFilter()
        {
            var text = extensionComboBox.Text.Trim();
            if (text.Length == 0) return false;
            var paths = text.Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries);
            return paths.Length > 0 && paths.All(IsValidFileMask);
        }

        bool IsValidTopLevel()
        {
            var text = folderComboBox.Text.Trim();
            if (text.Length == 0) return false;
            if (text == "<Project>") return true;
            var paths = text.Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries);
            return paths.Length > 0 && paths.All(Directory.Exists);
        }

        /// <summary>
        /// Gets search object for find and replace
        /// </summary>
        FRSearch GetFRSearch()
        {
            string pattern = findComboBox.Text;
            FRSearch search = new FRSearch(pattern);
            search.IsRegex = regexCheckBox.Checked;
            search.IsEscaped = escapedCheckBox.Checked;
            search.WholeWord = wholeWordCheckBox.Checked;
            search.NoCase = !matchCaseCheckBox.Checked;
            search.Filter = SearchFilter.None;
            if (!commentsCheckBox.Checked)
            {
                search.Filter |= SearchFilter.OutsideCodeComments;
            }
            if (!stringsCheckBox.Checked)
            {
                search.Filter |= SearchFilter.OutsideStringLiterals;
            }
            return search;
        }

        /// <summary>
        /// Sets the path to find
        /// </summary>
        public void SetFindPath(string path) => folderComboBox.Text = path;

        /// <summary>
        /// Update the dialog args when show is called
        /// </summary>
        public new void Show()
        {
            UpdateDialogArguments();
            base.Show();
        }

        /// <summary>
        /// Update the find combo box with the currently selected text.
        /// </summary>
        public void UpdateFindText()
        {
            var sci = PluginBase.MainForm.CurrentDocument.SciControl;
            if (sci != null && sci.SelText.Length > 0) findComboBox.Text = sci.SelText;
        }

        #endregion
    }
}