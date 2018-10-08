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
        private System.Windows.Forms.Label infoLabel;
        private System.Windows.Forms.Label findLabel;
        private System.Windows.Forms.Label folderLabel;
        private System.Windows.Forms.Label replaceLabel;
        private System.Windows.Forms.Label extensionLabel;
        private System.Windows.Forms.ColumnHeader lineHeader;
        private System.Windows.Forms.ColumnHeader descHeader;
        private System.Windows.Forms.ColumnHeader fileHeader;
        private System.Windows.Forms.ColumnHeader pathHeader;
        private System.Windows.Forms.ColumnHeader replacedHeader;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.GroupBox optionsGroupBox;
        private System.Windows.Forms.ComboBox folderComboBox;
        private System.Windows.Forms.ComboBox extensionComboBox;
        private System.Windows.Forms.ComboBox replaceComboBox;
        private System.Windows.Forms.ComboBox findComboBox;
        private System.Windows.Forms.CheckBox redirectCheckBox;
        private System.Windows.Forms.CheckBox regexCheckBox;
        private System.Windows.Forms.CheckBox escapedCheckBox;
        private System.Windows.Forms.CheckBox wholeWordCheckBox;
        private System.Windows.Forms.CheckBox matchCaseCheckBox;
        private System.Windows.Forms.CheckBox stringsCheckBox;
        private System.Windows.Forms.CheckBox commentsCheckBox;
        private System.Windows.Forms.CheckBox subDirectoriesCheckBox;
        private System.Windows.Forms.ListView resultsView;
        private System.Windows.Forms.Button replaceButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button browseButton;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.Button findButton;
        private PluginCore.FRService.FRRunner runner;
        private PluginCore.IProject lastProject;

        private const string TraceGroup = "FindInFiles";
        
        public FRInFilesDialog()
        {
            this.Owner = Globals.MainForm;
            this.Font = Globals.Settings.DefaultFont;
            this.FormGuid = "d2dbaf53-35ea-4632-b038-5428c9784a32";
            this.InitializeComponent();
            this.ApplyLocalizedTexts();
            this.InitializeGraphics();
            this.UpdateSettings();

            TraceManager.RegisterTraceGroup(TraceGroup, TextHelper.GetString("FlashDevelop.Label.FindAndReplaceResults"), false, true, Globals.MainForm.FindImage("209"));
        }

        #region Windows Form Designer Generated Code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.replaceButton = new System.Windows.Forms.ButtonEx();
            this.redirectCheckBox = new System.Windows.Forms.CheckBoxEx();
            this.regexCheckBox = new System.Windows.Forms.CheckBoxEx();
            this.optionsGroupBox = new System.Windows.Forms.GroupBoxEx();
            this.stringsCheckBox = new System.Windows.Forms.CheckBoxEx();
            this.commentsCheckBox = new System.Windows.Forms.CheckBoxEx();
            this.escapedCheckBox = new System.Windows.Forms.CheckBoxEx();
            this.wholeWordCheckBox = new System.Windows.Forms.CheckBoxEx();
            this.matchCaseCheckBox = new System.Windows.Forms.CheckBoxEx();
            this.subDirectoriesCheckBox = new System.Windows.Forms.CheckBoxEx();
            this.replaceComboBox = new System.Windows.Forms.FlatCombo();
            this.replaceLabel = new System.Windows.Forms.Label();
            this.findComboBox = new System.Windows.Forms.FlatCombo();
            this.findLabel = new System.Windows.Forms.Label();
            this.findButton = new System.Windows.Forms.ButtonEx();
            this.folderComboBox = new System.Windows.Forms.FlatCombo();
            this.folderLabel = new System.Windows.Forms.Label();
            this.extensionComboBox = new System.Windows.Forms.FlatCombo();
            this.extensionLabel = new System.Windows.Forms.Label();
            this.browseButton = new System.Windows.Forms.ButtonEx();
            this.replacedHeader = new System.Windows.Forms.ColumnHeader();
            this.cancelButton = new System.Windows.Forms.ButtonEx();
            this.infoLabel = new System.Windows.Forms.Label();
            this.lineHeader = new System.Windows.Forms.ColumnHeader();
            this.descHeader = new System.Windows.Forms.ColumnHeader();
            this.pathHeader = new System.Windows.Forms.ColumnHeader();
            this.fileHeader = new System.Windows.Forms.ColumnHeader();
            this.resultsView = new System.Windows.Forms.ListViewEx();
            this.progressBar = new System.Windows.Forms.ProgressBarEx();
            this.closeButton = new System.Windows.Forms.ButtonEx();
            this.optionsGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // replaceButton
            // 
            this.replaceButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.replaceButton.Location = new System.Drawing.Point(113, 171);
            this.replaceButton.Name = "replaceButton";
            this.replaceButton.Size = new System.Drawing.Size(90, 23);
            this.replaceButton.TabIndex = 8;
            this.replaceButton.Text = "&Replace";
            this.replaceButton.Click += new System.EventHandler(this.ReplaceButtonClick);
            // 
            // regexCheckBox
            // 
            this.regexCheckBox.AutoSize = true;
            this.regexCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.regexCheckBox.Location = new System.Drawing.Point(12, 85);
            this.regexCheckBox.Name = "regexCheckBox";
            this.regexCheckBox.Size = new System.Drawing.Size(132, 18);
            this.regexCheckBox.TabIndex = 4;
            this.regexCheckBox.Text = " &Regular expressions";
            // 
            // optionsGroupBox
            // 
            this.optionsGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.optionsGroupBox.Controls.Add(this.stringsCheckBox);
            this.optionsGroupBox.Controls.Add(this.commentsCheckBox);
            this.optionsGroupBox.Controls.Add(this.regexCheckBox);
            this.optionsGroupBox.Controls.Add(this.escapedCheckBox);
            this.optionsGroupBox.Controls.Add(this.wholeWordCheckBox);
            this.optionsGroupBox.Controls.Add(this.matchCaseCheckBox);
            this.optionsGroupBox.Controls.Add(this.subDirectoriesCheckBox);
            this.optionsGroupBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.optionsGroupBox.Location = new System.Drawing.Point(347, 15);
            this.optionsGroupBox.Name = "optionsGroupBox";
            this.optionsGroupBox.Size = new System.Drawing.Size(156, 178);
            this.optionsGroupBox.TabIndex = 6;
            this.optionsGroupBox.TabStop = false;
            this.optionsGroupBox.Text = " Options";
            // 
            // stringsCheckBox
            // 
            this.stringsCheckBox.AutoSize = true;
            this.stringsCheckBox.Checked = true;
            this.stringsCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.stringsCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.stringsCheckBox.Location = new System.Drawing.Point(12, 151);
            this.stringsCheckBox.Name = "stringsCheckBox";
            this.stringsCheckBox.Size = new System.Drawing.Size(103, 18);
            this.stringsCheckBox.TabIndex = 7;
            this.stringsCheckBox.Text = " Look in &strings";
            // 
            // commentsCheckBox
            // 
            this.commentsCheckBox.AutoSize = true;
            this.commentsCheckBox.Checked = true;
            this.commentsCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.commentsCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.commentsCheckBox.Location = new System.Drawing.Point(12, 129);
            this.commentsCheckBox.Name = "commentsCheckBox";
            this.commentsCheckBox.Size = new System.Drawing.Size(119, 18);
            this.commentsCheckBox.TabIndex = 6;
            this.commentsCheckBox.Text = " Look in &comments";
            // 
            // escapedCheckBox
            // 
            this.escapedCheckBox.AutoSize = true;
            this.escapedCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.escapedCheckBox.Location = new System.Drawing.Point(12, 63);
            this.escapedCheckBox.Name = "escapedCheckBox";
            this.escapedCheckBox.Size = new System.Drawing.Size(129, 18);
            this.escapedCheckBox.TabIndex = 3;
            this.escapedCheckBox.Text = " &Escaped characters";
            // 
            // wholeWordCheckBox
            // 
            this.wholeWordCheckBox.AutoSize = true;
            this.wholeWordCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.wholeWordCheckBox.Location = new System.Drawing.Point(12, 19);
            this.wholeWordCheckBox.Name = "wholeWordCheckBox";
            this.wholeWordCheckBox.Size = new System.Drawing.Size(92, 18);
            this.wholeWordCheckBox.TabIndex = 1;
            this.wholeWordCheckBox.Text = " &Whole word";
            // 
            // matchCaseCheckBox
            // 
            this.matchCaseCheckBox.AutoSize = true;
            this.matchCaseCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.matchCaseCheckBox.Location = new System.Drawing.Point(12, 41);
            this.matchCaseCheckBox.Name = "matchCaseCheckBox";
            this.matchCaseCheckBox.Size = new System.Drawing.Size(89, 18);
            this.matchCaseCheckBox.TabIndex = 2;
            this.matchCaseCheckBox.Text = " Match &case";
            // 
            // subDirectoriesCheckBox
            // 
            this.subDirectoriesCheckBox.AutoSize = true;
            this.subDirectoriesCheckBox.Checked = true;
            this.subDirectoriesCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.subDirectoriesCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.subDirectoriesCheckBox.Location = new System.Drawing.Point(12, 107);
            this.subDirectoriesCheckBox.Name = "subDirectoriesCheckBox";
            this.subDirectoriesCheckBox.Size = new System.Drawing.Size(138, 18);
            this.subDirectoriesCheckBox.TabIndex = 5;
            this.subDirectoriesCheckBox.Text = " Look in sub&directories";
            // 
            // replaceComboBox
            // 
            this.replaceComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.replaceComboBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.replaceComboBox.Location = new System.Drawing.Point(12, 61);
            this.replaceComboBox.Name = "replaceComboBox";
            this.replaceComboBox.Size = new System.Drawing.Size(324, 21);
            this.replaceComboBox.TabIndex = 2;
            // 
            // replaceLabel
            // 
            this.replaceLabel.AutoSize = true;
            this.replaceLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.replaceLabel.Location = new System.Drawing.Point(13, 46);
            this.replaceLabel.Name = "replaceLabel";
            this.replaceLabel.Size = new System.Drawing.Size(72, 13);
            this.replaceLabel.TabIndex = 24;
            this.replaceLabel.Text = "Replace with:";
            // 
            // findComboBox
            // 
            this.findComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.findComboBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.findComboBox.Location = new System.Drawing.Point(12, 22);
            this.findComboBox.Name = "findComboBox";
            this.findComboBox.Size = new System.Drawing.Size(324, 21);
            this.findComboBox.TabIndex = 1;
            // 
            // findLabel
            // 
            this.findLabel.AutoSize = true;
            this.findLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.findLabel.Location = new System.Drawing.Point(13, 7);
            this.findLabel.Name = "findLabel";
            this.findLabel.Size = new System.Drawing.Size(58, 13);
            this.findLabel.TabIndex = 17;
            this.findLabel.Text = "Find what:";
            // 
            // findButton
            // 
            this.findButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.findButton.Location = new System.Drawing.Point(11, 171);
            this.findButton.Name = "findButton";
            this.findButton.Size = new System.Drawing.Size(90, 23);
            this.findButton.TabIndex = 7;
            this.findButton.Text = "&Find";
            this.findButton.Click += new System.EventHandler(this.FindButtonClick);
            // 
            // folderComboBox
            // 
            this.folderComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.folderComboBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.folderComboBox.Location = new System.Drawing.Point(12, 139);
            this.folderComboBox.Name = "folderComboBox";
            this.folderComboBox.Size = new System.Drawing.Size(293, 21);
            this.folderComboBox.TabIndex = 4;
            this.folderComboBox.Text = "<Project>";
            this.folderComboBox.TextChanged += (sender, args) =>
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
            this.folderLabel.AutoSize = true;
            this.folderLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.folderLabel.Location = new System.Drawing.Point(13, 124);
            this.folderLabel.Name = "folderLabel";
            this.folderLabel.Size = new System.Drawing.Size(60, 13);
            this.folderLabel.TabIndex = 28;
            this.folderLabel.Text = "Top folder:";
            // 
            // extensionComboBox
            // 
            this.extensionComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.extensionComboBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.extensionComboBox.Location = new System.Drawing.Point(12, 100);
            this.extensionComboBox.Name = "extensionComboBox";
            this.extensionComboBox.Size = new System.Drawing.Size(324, 21);
            this.extensionComboBox.TabIndex = 3;
            this.extensionComboBox.Text = ".ext";
            this.extensionComboBox.TextChanged += (sender, args) =>
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
            this.extensionLabel.AutoSize = true;
            this.extensionLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.extensionLabel.Location = new System.Drawing.Point(13, 85);
            this.extensionLabel.Name = "extensionLabel";
            this.extensionLabel.Size = new System.Drawing.Size(83, 13);
            this.extensionLabel.TabIndex = 25;
            this.extensionLabel.Text = "Extension filter:";
            // 
            // browseButton
            // 
            this.browseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.browseButton.Location = new System.Drawing.Point(311, 136);
            this.browseButton.Name = "browseButton";
            this.browseButton.Size = new System.Drawing.Size(26, 24);
            this.browseButton.TabIndex = 5;
            this.browseButton.Click += new System.EventHandler(this.BrowseButtonClick);
            // 
            // replacedHeader
            // 
            this.replacedHeader.Text = "Replacements";
            this.replacedHeader.Width = 120;
            // 
            // cancelButton
            //
            this.cancelButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cancelButton.Location = new System.Drawing.Point(215, 171);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(90, 23);
            this.cancelButton.TabIndex = 9;
            this.cancelButton.Text = "Ca&ncel";
            this.cancelButton.Click += new System.EventHandler(this.CancelButtonClick);
            // 
            // infoLabel
            // 
            this.infoLabel.AutoSize = true;
            this.infoLabel.BackColor = System.Drawing.SystemColors.Control;
            this.infoLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.infoLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.infoLabel.Location = new System.Drawing.Point(14, 203);
            this.infoLabel.Name = "infoLabel";
            this.infoLabel.Size = new System.Drawing.Size(130, 13);
            this.infoLabel.TabIndex = 0;
            this.infoLabel.Text = "No suitable results found.";
            this.infoLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lineHeader
            // 
            this.lineHeader.Text = "Line";
            this.lineHeader.Width = 50;
            // 
            // descHeader
            // 
            this.descHeader.Text = "Description";
            this.descHeader.Width = 120;
            // 
            // fileHeader
            // 
            this.fileHeader.Text = "File";
            this.fileHeader.Width = 120;
            // 
            // pathHeader
            // 
            this.pathHeader.Text = "Path";
            this.pathHeader.Width = 220;
            // 
            // resultsView
            // 
            this.resultsView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.resultsView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.lineHeader,
            this.descHeader,
            this.fileHeader,
            this.pathHeader});
            this.resultsView.ShowGroups = true;
            this.resultsView.LabelWrap = false;
            this.resultsView.FullRowSelect = true;
            this.resultsView.GridLines = true;
            this.resultsView.Location = new System.Drawing.Point(12, 219);
            this.resultsView.Name = "resultsView";
            this.resultsView.ShowItemToolTips = true;
            this.resultsView.Size = new System.Drawing.Size(492, 167);
            this.resultsView.TabIndex = 11;
            this.resultsView.UseCompatibleStateImageBehavior = false;
            this.resultsView.View = System.Windows.Forms.View.Details;
            this.resultsView.DoubleClick += new System.EventHandler(this.ResultsViewDoubleClick);
            // 
            // progressBar
            // 
            this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar.Location = new System.Drawing.Point(12, 392);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(491, 14);
            this.progressBar.TabIndex = 0;
            // 
            // closeButton
            //
            this.closeButton.TabStop = false;
            this.closeButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.closeButton.Location = new System.Drawing.Point(0, 0);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(0, 0);
            this.closeButton.TabIndex = 29;
            this.closeButton.Click += new System.EventHandler(this.CloseButtonClick);
            // 
            // sendCheckBox
            // 
            this.redirectCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Right)));
            this.redirectCheckBox.AutoSize = true;
            this.redirectCheckBox.Checked = true;
            this.redirectCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.redirectCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.redirectCheckBox.Location = new System.Drawing.Point(273, 201);
            this.redirectCheckBox.Name = "redirectCheckBox";
            this.redirectCheckBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.redirectCheckBox.Size = new System.Drawing.Size(230, 25);
            this.redirectCheckBox.TabIndex = 8;
            this.redirectCheckBox.Text = " Send results to Results Panel";
            this.redirectCheckBox.CheckedChanged += new EventHandler(this.RedirectCheckBoxCheckChanged);
            // 
            // FRInFilesDialog
            // 
            this.AcceptButton = this.findButton;
            this.CancelButton = this.closeButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(515, 418);
            this.Controls.Add(this.infoLabel);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.resultsView);
            this.Controls.Add(this.browseButton);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.folderComboBox);
            this.Controls.Add(this.folderLabel);
            this.Controls.Add(this.extensionComboBox);
            this.Controls.Add(this.extensionLabel);
            this.Controls.Add(this.replaceButton);
            this.Controls.Add(this.optionsGroupBox);
            this.Controls.Add(this.replaceComboBox);
            this.Controls.Add(this.replaceLabel);
            this.Controls.Add(this.findComboBox);
            this.Controls.Add(this.findLabel);
            this.Controls.Add(this.findButton);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.redirectCheckBox);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FRInFilesDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.MinimumSize = new System.Drawing.Size(500, 340);
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = " Find And Replace In Files";
            this.Load += new System.EventHandler(this.DialogLoaded);
            this.VisibleChanged += new System.EventHandler(this.VisibleChange);
            this.Closing += new System.ComponentModel.CancelEventHandler(this.DialogClosing);
            this.optionsGroupBox.ResumeLayout(false);
            this.optionsGroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        #region Methods And Event Handlers

        /// <summary>
        /// Applies the settings to the UI
        /// </summary>
        public void UpdateSettings()
        {
            FRDialogGenerics.UpdateComboBoxItems(this.folderComboBox);
            Boolean useGroups = Globals.MainForm.Settings.UseListViewGrouping;
            this.resultsView.ShowGroups = useGroups;
            this.resultsView.GridLines = !useGroups;
        }

        /// <summary>
        /// Initializes the external graphics
        /// </summary>
        private void InitializeGraphics()
        {
            ImageList imageList = new ImageList();
            imageList.ColorDepth = ColorDepth.Depth32Bit;
            imageList.ImageSize = ScaleHelper.Scale(new Size(16, 16));
            imageList.Images.Add(Globals.MainForm.FindImage("203", false));
            this.browseButton.ImageList = imageList;
            this.browseButton.ImageIndex = 0;
        }

        /// <summary>
        /// Applies the localized texts to the form
        /// </summary>
        private void ApplyLocalizedTexts()
        {
            this.findLabel.Text = TextHelper.GetString("Info.FindWhat");
            this.extensionLabel.Text = TextHelper.GetString("Info.ExtensionFilter");
            this.folderLabel.Text = TextHelper.GetString("Info.TopFolder");
            this.replaceLabel.Text = TextHelper.GetString("Info.ReplaceWith");
            this.findButton.Text = TextHelper.GetString("Label.Find");
            this.cancelButton.Text = TextHelper.GetString("Label.Cancel");
            this.replaceButton.Text = TextHelper.GetStringWithoutEllipsis("Label.Replace");
            this.lineHeader.Text = TextHelper.GetString("Info.LineHeader");
            this.descHeader.Text = TextHelper.GetString("Info.DescHeader");
            this.pathHeader.Text = TextHelper.GetString("Info.PathHeader");
            this.fileHeader.Text = TextHelper.GetString("Info.FileHeader");
            this.replacedHeader.Text = TextHelper.GetString("Info.ReplacementsHeader");
            this.optionsGroupBox.Text = " " + TextHelper.GetString("Label.Options");
            this.matchCaseCheckBox.Text = " " + TextHelper.GetString("Label.MatchCase");
            this.wholeWordCheckBox.Text = " " + TextHelper.GetString("Label.WholeWord");
            this.escapedCheckBox.Text = " " + TextHelper.GetString("Label.EscapedCharacters");
            this.regexCheckBox.Text = " " + TextHelper.GetString("Label.RegularExpressions");
            this.subDirectoriesCheckBox.Text = " " + TextHelper.GetString("Label.LookInSubdirectories");
            this.redirectCheckBox.Text = " " + TextHelper.GetString("Info.RedirectFilesResults");
            this.commentsCheckBox.Text = " " + TextHelper.GetString("Label.LookInComments");
            this.stringsCheckBox.Text = " " + TextHelper.GetString("Label.LookInStrings");
            this.Text = " " + TextHelper.GetString("Title.FindAndReplaceInFilesDialog");
            this.pathHeader.Width = -2; // Extend last column
        }

        /// <summary>
        /// Changes the redirection setting and updates the check state.
        /// </summary>
        private void RedirectCheckBoxCheckChanged(Object sender, EventArgs e)
        {
            Globals.Settings.RedirectFilesResults = !Globals.Settings.RedirectFilesResults;
            this.redirectCheckBox.Checked = Globals.Settings.RedirectFilesResults;
        }

        /// <summary>
        /// Runs the find based on the user specified arguments
        /// </summary>
        private void FindButtonClick(Object sender, EventArgs e)
        {
            String mask = this.extensionComboBox.Text;
            if (IsValidPattern() && this.IsValidFileMask(mask))
            {
                bool recursive = this.subDirectoriesCheckBox.Checked;
                var paths = this.folderComboBox.Text.Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries);
                foreach (String path in paths)
                {
                    FRConfiguration config = this.GetFRConfig(path, mask, recursive);
                    if (config == null) return;
                    config.CacheDocuments = true;
                    this.UpdateUIState(true);
                    this.runner = new FRRunner();
                    this.runner.ProgressReport += this.RunnerProgress;
                    this.runner.Finished += this.FindFinished;
                    this.runner.SearchAsync(config);
                    FRDialogGenerics.UpdateComboBoxItems(this.folderComboBox);
                    FRDialogGenerics.UpdateComboBoxItems(this.extensionComboBox);
                    FRDialogGenerics.UpdateComboBoxItems(this.findComboBox);
                }
            }
        }

        /// <summary>
        /// Runs the replace based on the user specified arguments
        /// </summary>
        private void ReplaceButtonClick(Object sender, EventArgs e)
        {
            String mask = this.extensionComboBox.Text;
            if (IsValidPattern() && this.IsValidFileMask(mask))
            {
                if (!Globals.Settings.DisableReplaceFilesConfirm)
                {
                    String caption = TextHelper.GetString("Title.ConfirmDialog");
                    String message = TextHelper.GetString("Info.AreYouSureToReplaceInFiles");
                    DialogResult result = MessageBox.Show(message, caption, MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                    if (result == DialogResult.Cancel) return;
                }
                var recursive = this.subDirectoriesCheckBox.Checked;
                var paths = this.folderComboBox.Text.Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries);
                foreach (String path in paths)
                {
                    FRConfiguration config = this.GetFRConfig(path, mask, recursive);
                    if (config == null) return;
                    config.CacheDocuments = true;
                    config.UpdateSourceFileOnly = false;
                    config.Replacement = this.replaceComboBox.Text;
                    this.UpdateUIState(true);
                    this.runner = new FRRunner();
                    this.runner.ProgressReport += this.RunnerProgress;
                    this.runner.Finished += this.ReplaceFinished;
                    this.runner.ReplaceAsync(config);
                    FRDialogGenerics.UpdateComboBoxItems(this.folderComboBox);
                    FRDialogGenerics.UpdateComboBoxItems(this.extensionComboBox);
                    FRDialogGenerics.UpdateComboBoxItems(this.replaceComboBox);
                    FRDialogGenerics.UpdateComboBoxItems(this.findComboBox);
                }
            }
        }

        /// <summary>
        /// Closes the dialog window
        /// </summary>
        private void CloseButtonClick(Object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Selects the path for the find and replace
        /// </summary>
        private void BrowseButtonClick(Object sender, EventArgs e)
        {
            using (var fbd = new VistaFolderBrowserDialog())
            {
                fbd.Multiselect = true;
                String curDir = this.folderComboBox.Text;
                if (curDir == "<Project>")
                {
                    if (PluginBase.CurrentProject != null)
                    {
                        String projectPath = PluginBase.CurrentProject.ProjectPath;
                        curDir = Path.GetDirectoryName(projectPath);
                    }
                    else curDir = Globals.MainForm.WorkingDirectory;
                }
                if (Directory.Exists(curDir)) fbd.SelectedPath = curDir;
                if (fbd.ShowDialog() == DialogResult.OK && Directory.Exists(fbd.SelectedPath))
                {
                    this.folderComboBox.Text = String.Join(";", fbd.SelectedPaths);
                    this.folderComboBox.SelectionStart = this.folderComboBox.Text.Length;
                }
            }
        }

        /// <summary>
        /// Handles the double click event from results view
        /// </summary>
        private void ResultsViewDoubleClick(Object sender, System.EventArgs e)
        {
            if (this.resultsView.SelectedItems.Count < 1) return;
            ListViewItem item = this.resultsView.SelectedItems[0];
            var data = (KeyValuePair<String, SearchMatch>)item.Tag;
            if (File.Exists(data.Key))
            {
                Globals.MainForm.Activate();
                var doc = Globals.MainForm.OpenEditableDocument(data.Key, false) as ITabbedDocument;
                if (doc != null && doc.IsEditable && this.resultsView.Columns.Count == 4)
                {
                    FRDialogGenerics.SelectMatch(doc.SciControl, data.Value);
                }
            }
        }

        /// <summary>
        /// Cancels the find or replace lookup
        /// </summary>
        private void CancelButtonClick(Object sender, System.EventArgs e)
        {
            runner?.CancelAsync();
        }

        /// <summary>
        /// Runner reports how much of the lookup is done
        /// </summary>
        private void RunnerProgress(Int32 percentDone)
        {
            this.progressBar.Value = percentDone;
        }

        /// <summary>
        /// Handles the results when find is ready
        /// </summary>
        private void FindFinished(FRResults results)
        {
            this.SetupResultsView(true);
            this.resultsView.Items.Clear();
            if (results == null)
            {
                String message = TextHelper.GetString("Info.FindLookupCanceled");
                this.infoLabel.Text = message;
            }
            else if (results.Count == 0)
            {
                String message = TextHelper.GetString("Info.NoMatchesFound");
                this.infoLabel.Text = message;
            }
            else
            {
                if (!this.redirectCheckBox.Checked)
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
                            this.resultsView.Items.Add(item);
                            this.AddToGroup(item, entry.Key);
                        }
                    }
                    var message = TextHelper.GetString("Info.FoundInFiles");
                    var formatted = string.Format(message, matchCount, fileCount);
                    this.infoLabel.Text = formatted;
                    resultsView.EndUpdate();
                } 
                else 
                {
                    var groupData = TraceManager.CreateGroupDataUnique(TraceGroup);
                    Globals.MainForm.CallCommand("PluginCommand", "ResultsPanel.ClearResults;" + groupData);
                    foreach (var entry in results)
                    {
                        foreach (var match in entry.Value)
                        {
                            var message = $"{entry.Key}:{match.Line}: chars {match.Column}-{match.Column + match.Length} : {match.LineText.Trim()}";
                            TraceManager.Add(message, (int)TraceType.Info, groupData);
                        }
                    }
                    Globals.MainForm.CallCommand("PluginCommand", "ResultsPanel.ShowResults;" + groupData);
                    this.Hide();
                }
            }
            this.UpdateUIState(false);
        }

        /// <summary>
        /// Handles the results when replace is ready
        /// </summary>
        private void ReplaceFinished(FRResults results)
        {
            this.SetupResultsView(false);
            this.resultsView.Items.Clear();
            if (results == null)
            {
                String message = TextHelper.GetString("Info.ReplaceLookupCanceled");
                this.infoLabel.Text = message;
            }
            else if (results.Count == 0)
            {
                String message = TextHelper.GetString("Info.NoMatchesFound");
                this.infoLabel.Text = message;
            }
            else
            {
                if (!this.redirectCheckBox.Checked)
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
                        this.resultsView.Items.Add(item);
                        this.AddToGroup(item, entry.Key);
                    }
                    var message = TextHelper.GetString("Info.ReplacedInFiles");
                    var formatted = string.Format(message, matchCount, fileCount);
                    this.infoLabel.Text = formatted;
                    resultsView.EndUpdate();
                } 
                else
                {
                    var groupData = TraceManager.CreateGroupDataUnique(TraceGroup);
                    Globals.MainForm.CallCommand("PluginCommand", "ResultsPanel.ClearResults;" + groupData);
                    foreach (var entry in results)
                    {
                        foreach (var match in entry.Value)
                        {
                            var message = $"{entry.Key}:{match.Line}: chars {match.Column}-{match.Column + match.Length} : {match.Value}";
                            TraceManager.Add(message, (int)TraceType.Info, groupData);
                        }
                    }
                    Globals.MainForm.CallCommand("PluginCommand", "ResultsPanel.ShowResults;" + groupData);
                    this.Hide();
                }
            }
            this.UpdateUIState(false);
        }

        /// <summary>
        /// Adds item to the specified group
        /// </summary>
        private void AddToGroup(ListViewItem item, string path)
        {
            foreach (ListViewGroup lvg in this.resultsView.Groups)
            {
                if (lvg.Tag.ToString() == path)
                {
                    lvg.Items.Add(item);
                    return;
                }
            }
            string gpname;
            if (File.Exists(path)) gpname = Path.GetFileName(path);
            else gpname = TextHelper.GetString("Group.Other");
            var gp = new ListViewGroup();
            gp.Tag = path;
            gp.Header = gpname;
            this.resultsView.Groups.Add(gp);
            gp.Items.Add(item);
        }

        /// <summary>
        /// Setups the resultsView.Columns for find and replace
        /// </summary>
        private void SetupResultsView(Boolean finding)
        {
            if (finding && this.resultsView.Columns.Count != 4)
            {
                this.resultsView.Columns.Clear();
                this.resultsView.Columns.AddRange(new[] { this.lineHeader, this.descHeader, this.fileHeader, this.pathHeader });
            }
            else if (!finding && this.resultsView.Columns.Count != 3)
            {
                this.resultsView.Columns.Clear();
                this.resultsView.Columns.AddRange(new[] { this.replacedHeader, this.fileHeader, this.pathHeader });
            }
        }

        /// <summary>
        /// Just hides the dialog window when closing
        /// </summary>
        private void DialogClosing(Object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            runner?.CancelAsync();
            Globals.CurrentDocument.Activate();
            this.Hide();
        }
        
        /// <summary>
        /// Setups the dialog on load
        /// </summary>
        private void DialogLoaded(Object sender, System.EventArgs e)
        {
            this.cancelButton.Enabled = false;
            String message = TextHelper.GetString("Info.NoMatches");
            this.infoLabel.Text = message;
            this.CenterToParent();
        }

        /// <summary>
        /// Process shortcuts
        /// </summary>
        protected override Boolean ProcessDialogKey(Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.F))
            {
                this.findComboBox.Focus();
                return true;
            }
            return base.ProcessDialogKey(keyData);
        }

        /// <summary>
        /// Some event handling when showing the form
        /// </summary>
        private void VisibleChange(Object sender, EventArgs e)
        {
            if (this.Visible)
            {
                this.findComboBox.Select();
                this.findComboBox.SelectAll();
            }
        }

        /// <summary>
        /// Checks if the file mask is somewhat valid
        /// </summary>
        private bool IsValidFileMask(string mask)
        {
            return !string.IsNullOrEmpty(mask) && mask.Trim().StartsWithOrdinal("*.") && !mask.Contains("..")
                   && !mask.Contains('/') && !mask.Contains('\\') && !mask.Contains(':') && Path.GetInvalidPathChars().All(it => !mask.Contains(it));
        }

        /// <summary>
        /// Updates the file extension and the initial directory
        /// </summary>
        private void UpdateDialogArguments()
        {
            IProject project = PluginBase.CurrentProject;
            Boolean doRefresh = lastProject != null && lastProject != project;
            if (project != null)
            {
                String path = Path.GetDirectoryName(project.ProjectPath);
                if (String.IsNullOrEmpty(this.folderComboBox.Text) || doRefresh)
                {
                    this.folderComboBox.Text = path;
                }
            }
            else if (String.IsNullOrEmpty(this.folderComboBox.Text) || doRefresh)
            {
                this.folderComboBox.Text = Globals.MainForm.WorkingDirectory;
            }
            this.folderComboBox.SelectionStart = this.folderComboBox.Text.Length;
            this.redirectCheckBox.CheckedChanged -= this.RedirectCheckBoxCheckChanged;
            this.redirectCheckBox.Checked = Globals.Settings.RedirectFilesResults;
            this.redirectCheckBox.CheckedChanged += this.RedirectCheckBoxCheckChanged;
            if (!this.IsValidFileMask(this.extensionComboBox.Text) || doRefresh)
            {
                if (project != null)
                {
                    String filter = project.DefaultSearchFilter;
                    this.extensionComboBox.Text = filter;
                }
                else
                {
                    String def = Globals.Settings.DefaultFileExtension;
                    this.extensionComboBox.Text = "*." + def;
                }
            }
            UpdateFindText();
            if (project != null) lastProject = project;
        }

        /// <summary>
        /// Updates the ui based on the running state
        /// </summary> 
        private void UpdateUIState(Boolean running)
        {
            if (running)
            {
                this.cancelButton.Enabled = true;
                this.replaceButton.Enabled = false;
                this.findButton.Enabled = false;
            } 
            else 
            {
                this.cancelButton.Enabled = false;
                this.replaceButton.Enabled = true;
                this.findButton.Enabled = true;
            }
            this.progressBar.Value = 0;
        }

        /// <summary>
        /// Gets search config for find and replace
        /// </summary>
        private FRConfiguration GetFRConfig(String path, String mask, Boolean recursive)
        {
            if (path.Trim() != "<Project>") return new FRConfiguration(path, mask, recursive, this.GetFRSearch());
            if (PluginBase.CurrentProject == null) return null;
            var allFiles = new List<String>();
            var project = PluginBase.CurrentProject;
            var projPath = Path.GetDirectoryName(project.ProjectPath);
            var walker = new PathWalker(projPath, mask, recursive);
            var projFiles = walker.GetFiles();
            foreach (String file in projFiles)
            {
                if (!IsFileHidden(file, project)) allFiles.Add(file);
            }
            foreach (var sp in project.SourcePaths)
            {
                String sourcePath = project.GetAbsolutePath(sp);
                if (Directory.Exists(sourcePath) && !sourcePath.StartsWithOrdinal(projPath))
                {
                    walker = new PathWalker(sourcePath, mask, recursive);
                    allFiles.AddRange(walker.GetFiles());
                }
            }
            return new FRConfiguration(allFiles, this.GetFRSearch());
        }

        /// <summary>
        /// Check if file is hidden in project
        /// </summary>
        private bool IsFileHidden(String file, IProject project)
        {
            String[] hiddenPaths = project.GetHiddenPaths();
            foreach (String hiddenPath in hiddenPaths)
            {
                String absHiddenPath = project.GetAbsolutePath(hiddenPath);
                if (Directory.Exists(absHiddenPath) && file.StartsWithOrdinal(absHiddenPath)) return true;
            }
            return false;
        }

        /// <summary>
        /// Control user pattern
        /// </summary>
        private bool IsValidPattern()
        {
            String pattern = this.findComboBox.Text;
            if (pattern.Length == 0) return false;
            if (this.regexCheckBox.Checked)
            {
                try
                {
                    new Regex(pattern);
                }
                catch (Exception ex)
                {
                    ErrorManager.ShowInfo(ex.Message);
                    this.Select();
                    this.findComboBox.SelectAll();
                    return false;
                }
            }
            return true;
        }

        private bool IsValidExtensionFilter()
        {
            var text = extensionComboBox.Text.Trim();
            if (text.Length == 0) return false;
            var paths = text.Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries);
            return paths.Length > 0 && paths.All(IsValidFileMask);
        }

        private bool IsValidTopLevel()
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
        private FRSearch GetFRSearch()
        {
            String pattern = this.findComboBox.Text;
            FRSearch search = new FRSearch(pattern);
            search.IsRegex = this.regexCheckBox.Checked;
            search.IsEscaped = this.escapedCheckBox.Checked;
            search.WholeWord = this.wholeWordCheckBox.Checked;
            search.NoCase = !this.matchCaseCheckBox.Checked;
            search.Filter = SearchFilter.None;
            if (!this.commentsCheckBox.Checked)
            {
                search.Filter |= SearchFilter.OutsideCodeComments;
            }
            if (!this.stringsCheckBox.Checked)
            {
                search.Filter |= SearchFilter.OutsideStringLiterals;
            }
            return search;
        }

        /// <summary>
        /// Sets the path to find
        /// </summary>
        public void SetFindPath(String path)
        {
            this.folderComboBox.Text = path;
        }

        /// <summary>
        /// Update the dialog args when show is called
        /// </summary>
        public new void Show()
        {
            this.UpdateDialogArguments();
            base.Show();
        }

        /// <summary>
        /// Update the find combo box with the currently selected text.
        /// </summary>
        public void UpdateFindText()
        {
            ITabbedDocument document = Globals.CurrentDocument;
            if (document.IsEditable && document.SciControl.SelText.Length > 0)
            {
                this.findComboBox.Text = document.SciControl.SelText;
            }
        }

        #endregion

    }

}