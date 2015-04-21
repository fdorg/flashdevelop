using System;
using System.IO;
using System.Data;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using ASDocGen.Utilities;
using ASDocGen.Objects;
using System.Text.RegularExpressions;
using System.Xml;

namespace ASDocGen
{
    public class MainForm : Form
    {
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.Button openProjectButton;
        private System.Windows.Forms.Button saveProjectButton;
        private System.Windows.Forms.Button newProjectButton;
        private System.Windows.Forms.Button generateDocsButton;
        private System.Windows.Forms.Button saveSettingsButton;
        private System.Windows.Forms.Button asdocBrowseButton;
        private System.Windows.Forms.Button as2apiBrowseButton;
        private System.Windows.Forms.Button browseOutputDirButton;
        private System.Windows.Forms.Button removeClasspathButton;
        private System.Windows.Forms.Button browseClasspathButton;
        private System.Windows.Forms.Button newFromProjectButton;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.ComboBox compilerComboBox;
        private System.Windows.Forms.ListBox classpathListBox;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage outputTabPage;
        private System.Windows.Forms.TabPage projectTabPage;
        private System.Windows.Forms.TabPage settingsTabPage;
        private System.Windows.Forms.TextBox packagesTextBox;
        private System.Windows.Forms.TextBox outputDirTextBox;
        private System.Windows.Forms.TextBox pageTitleTextBox;
        private System.Windows.Forms.RichTextBox outputTextBox;
        private System.Windows.Forms.TextBox extraOptionsTextBox;
        private System.Windows.Forms.TextBox asdocLocationTextBox;
        private System.Windows.Forms.TextBox as2apiLocationTextBox;
        private System.Windows.Forms.CheckBox filesCheckBox;
        private System.Windows.Forms.Label packagesLabel;
        private System.Windows.Forms.Label outputDirLabel;
        private System.Windows.Forms.Label pageTitleLabel;
        private System.Windows.Forms.Label classpathLabel;
        private System.Windows.Forms.Label extraOptionsLabel;
        private System.Windows.Forms.Label compilerLabel;
        private System.Windows.Forms.Label as2apiLocationLabel;
        private System.Windows.Forms.Label asdocLocationLabel;
        private System.Windows.Forms.LinkLabel documentationLabel;
        private ASDocGen.Utilities.ProcessRunner processRunner;
        private ASDocGen.Objects.Project activeProject;
        private ASDocGen.Objects.Settings appSettings;
        private System.Boolean settingsAreModified;
        private System.Boolean projectIsModified;
        private System.String[] arguments;

        public MainForm(String[] arguments)
        {
            this.LoadSettings();
            this.arguments = arguments;
            this.projectIsModified = false;
            this.settingsAreModified = false;
            this.Font = SystemFonts.MenuFont;
            this.InitializeProcessRunner();
            this.InitializeComponent();
            this.InitializeDialogs();
        }

        #region Windows Form Designer Generated Code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.openProjectButton = new System.Windows.Forms.Button();
            this.saveProjectButton = new System.Windows.Forms.Button();
            this.newProjectButton = new System.Windows.Forms.Button();
            this.generateDocsButton = new System.Windows.Forms.Button();
            this.newFromProjectButton = new System.Windows.Forms.Button();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.outputTabPage = new System.Windows.Forms.TabPage();
            this.outputTextBox = new System.Windows.Forms.RichTextBox();
            this.projectTabPage = new System.Windows.Forms.TabPage();
            this.compilerLabel = new System.Windows.Forms.Label();
            this.compilerComboBox = new System.Windows.Forms.ComboBox();
            this.extraOptionsTextBox = new System.Windows.Forms.TextBox();
            this.extraOptionsLabel = new System.Windows.Forms.Label();
            this.packagesTextBox = new System.Windows.Forms.TextBox();
            this.outputDirTextBox = new System.Windows.Forms.TextBox();
            this.pageTitleTextBox = new System.Windows.Forms.TextBox();
            this.packagesLabel = new System.Windows.Forms.Label();
            this.browseOutputDirButton = new System.Windows.Forms.Button();
            this.removeClasspathButton = new System.Windows.Forms.Button();
            this.browseClasspathButton = new System.Windows.Forms.Button();
            this.outputDirLabel = new System.Windows.Forms.Label();
            this.pageTitleLabel = new System.Windows.Forms.Label();
            this.classpathLabel = new System.Windows.Forms.Label();
            this.classpathListBox = new System.Windows.Forms.ListBox();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.settingsTabPage = new System.Windows.Forms.TabPage();
            this.filesCheckBox = new System.Windows.Forms.CheckBox();
            this.saveSettingsButton = new System.Windows.Forms.Button();
            this.asdocLocationTextBox = new System.Windows.Forms.TextBox();
            this.asdocBrowseButton = new System.Windows.Forms.Button();
            this.asdocLocationLabel = new System.Windows.Forms.Label();
            this.as2apiLocationTextBox = new System.Windows.Forms.TextBox();
            this.as2apiBrowseButton = new System.Windows.Forms.Button();
            this.as2apiLocationLabel = new System.Windows.Forms.Label();
            this.documentationLabel = new System.Windows.Forms.LinkLabel();
            this.closeButton = new System.Windows.Forms.Button();
            this.outputTabPage.SuspendLayout();
            this.projectTabPage.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.settingsTabPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // openProjectButton
            // 
            this.openProjectButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.openProjectButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.openProjectButton.Location = new System.Drawing.Point(10, 326);
            this.openProjectButton.Name = "openProjectButton";
            this.openProjectButton.Size = new System.Drawing.Size(100, 23);
            this.openProjectButton.TabIndex = 1;
            this.openProjectButton.Text = "Open Project...";
            this.openProjectButton.UseVisualStyleBackColor = true;
            this.openProjectButton.Click += new System.EventHandler(this.OpenProjectButtonClick);
            // 
            // saveProjectButton
            // 
            this.saveProjectButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.saveProjectButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.saveProjectButton.Location = new System.Drawing.Point(115, 326);
            this.saveProjectButton.Name = "saveProjectButton";
            this.saveProjectButton.Size = new System.Drawing.Size(100, 23);
            this.saveProjectButton.TabIndex = 2;
            this.saveProjectButton.Text = "Save Project...";
            this.saveProjectButton.UseVisualStyleBackColor = true;
            this.saveProjectButton.Click += new System.EventHandler(this.SaveProjectButtonClick);
            // 
            // newProjectButton
            // 
            this.newProjectButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.newProjectButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.newProjectButton.Location = new System.Drawing.Point(220, 326);
            this.newProjectButton.Name = "newProjectButton";
            this.newProjectButton.Size = new System.Drawing.Size(100, 23);
            this.newProjectButton.TabIndex = 3;
            this.newProjectButton.Text = "New Project...";
            this.newProjectButton.UseVisualStyleBackColor = true;
            this.newProjectButton.Click += new System.EventHandler(this.NewProjectButtonClick);
            // 
            // newFromProjectButton
            // 
            this.newFromProjectButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.newFromProjectButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.newFromProjectButton.Location = new System.Drawing.Point(325, 326);
            this.newFromProjectButton.Name = "newProjectButton";
            this.newFromProjectButton.Size = new System.Drawing.Size(100, 23);
            this.newFromProjectButton.TabIndex = 4;
            this.newFromProjectButton.Text = "Import Project...";
            this.newFromProjectButton.UseVisualStyleBackColor = true;
            this.newFromProjectButton.Click += new System.EventHandler(this.NewFromProjectButtonClick);
            // 
            // generateDocsButton
            // 
            this.generateDocsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.generateDocsButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.generateDocsButton.Location = new System.Drawing.Point(441, 326);
            this.generateDocsButton.Name = "generateDocsButton";
            this.generateDocsButton.Size = new System.Drawing.Size(100, 23);
            this.generateDocsButton.TabIndex = 5;
            this.generateDocsButton.Text = "Generate!";
            this.generateDocsButton.UseVisualStyleBackColor = true;
            this.generateDocsButton.Click += new System.EventHandler(this.GenerateDocsButtonClick);
            // 
            // outputTabPage
            // 
            this.outputTabPage.BackColor = System.Drawing.Color.White;
            this.outputTabPage.Controls.Add(this.outputTextBox);
            this.outputTabPage.Location = new System.Drawing.Point(4, 22);
            this.outputTabPage.Name = "outputTabPage";
            this.outputTabPage.Size = new System.Drawing.Size(518, 284);
            this.outputTabPage.Padding = new System.Windows.Forms.Padding(2, 3, 3, 2);
            this.outputTabPage.TabIndex = 2;
            this.outputTabPage.Text = "Output";
            // 
            // outputTextBox
            // 
            this.outputTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.outputTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.outputTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.outputTextBox.Font = new System.Drawing.Font("Courier New", 8.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.outputTextBox.Location = new System.Drawing.Point(3, 3);
            this.outputTextBox.Name = "outputTextBox";
            this.outputTextBox.ReadOnly = true;
            this.outputTextBox.Size = new System.Drawing.Size(511, 278);
            this.outputTextBox.TabIndex = 3;
            this.outputTextBox.Text = "";
            this.outputTextBox.WordWrap = false;
            // 
            // projectTabPage
            // 
            this.projectTabPage.BackColor = System.Drawing.Color.White;
            this.projectTabPage.Controls.Add(this.compilerLabel);
            this.projectTabPage.Controls.Add(this.compilerComboBox);
            this.projectTabPage.Controls.Add(this.extraOptionsTextBox);
            this.projectTabPage.Controls.Add(this.extraOptionsLabel);
            this.projectTabPage.Controls.Add(this.packagesTextBox);
            this.projectTabPage.Controls.Add(this.outputDirTextBox);
            this.projectTabPage.Controls.Add(this.pageTitleTextBox);
            this.projectTabPage.Controls.Add(this.packagesLabel);
            this.projectTabPage.Controls.Add(this.browseOutputDirButton);
            this.projectTabPage.Controls.Add(this.removeClasspathButton);
            this.projectTabPage.Controls.Add(this.browseClasspathButton);
            this.projectTabPage.Controls.Add(this.outputDirLabel);
            this.projectTabPage.Controls.Add(this.pageTitleLabel);
            this.projectTabPage.Controls.Add(this.classpathLabel);
            this.projectTabPage.Controls.Add(this.classpathListBox);
            this.projectTabPage.Location = new System.Drawing.Point(4, 22);
            this.projectTabPage.Name = "projectTabPage";
            this.projectTabPage.Size = new System.Drawing.Size(518, 284);
            this.projectTabPage.TabIndex = 1;
            this.projectTabPage.Text = "Project";
            // 
            // compilerLabel
            // 
            this.compilerLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.compilerLabel.AutoSize = true;
            this.compilerLabel.BackColor = System.Drawing.SystemColors.Window;
            this.compilerLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.compilerLabel.Location = new System.Drawing.Point(319, 206);
            this.compilerLabel.Name = "compilerLabel";
            this.compilerLabel.Size = new System.Drawing.Size(52, 13);
            this.compilerLabel.Text = "Compiler:";
            // 
            // compilerComboBox
            // 
            this.compilerComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.compilerComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.compilerComboBox.FormattingEnabled = true;
            this.compilerComboBox.Items.AddRange(new object[]{"AS2API (AS2)","ASDOC (AS3)"});
            this.compilerComboBox.Location = new System.Drawing.Point(317, 222);
            this.compilerComboBox.Name = "compilerComboBox";
            this.compilerComboBox.Size = new System.Drawing.Size(186, 21);
            this.compilerComboBox.TabIndex = 9;
            this.compilerComboBox.SelectedIndexChanged += new System.EventHandler(this.UpdateProjectIsModified);
            // 
            // extraOptionsTextBox
            // 
            this.extraOptionsTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.extraOptionsTextBox.Location = new System.Drawing.Point(14, 222);
            this.extraOptionsTextBox.Multiline = true;
            this.extraOptionsTextBox.Name = "extraOptionsTextBox";
            this.extraOptionsTextBox.Size = new System.Drawing.Size(290, 50);
            this.extraOptionsTextBox.TabIndex = 8;
            this.extraOptionsTextBox.Font = SystemFonts.MenuFont;
            this.extraOptionsTextBox.TextChanged += new System.EventHandler(this.UpdateProjectIsModified);
            // 
            // extraOptionsLabel
            // 
            this.extraOptionsLabel.AutoSize = true;
            this.extraOptionsLabel.BackColor = System.Drawing.SystemColors.Window;
            this.extraOptionsLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.extraOptionsLabel.Location = new System.Drawing.Point(15, 206);
            this.extraOptionsLabel.Name = "extraOptionsLabel";
            this.extraOptionsLabel.Size = new System.Drawing.Size(75, 13);
            this.extraOptionsLabel.Text = "Extra options:";
            // 
            // packagesTextBox
            // 
            this.packagesTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.packagesTextBox.Location = new System.Drawing.Point(14, 64);
            this.packagesTextBox.Name = "packagesTextBox";
            this.packagesTextBox.Size = new System.Drawing.Size(491, 21);
            this.packagesTextBox.TabIndex = 2;
            this.packagesTextBox.TextChanged += new System.EventHandler(this.UpdateProjectIsModified);
            // 
            // outputDirTextBox
            // 
            this.outputDirTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.outputDirTextBox.Location = new System.Drawing.Point(14, 105);
            this.outputDirTextBox.Name = "outputDirTextBox";
            this.outputDirTextBox.Size = new System.Drawing.Size(383, 21);
            this.outputDirTextBox.TabIndex = 3;
            this.outputDirTextBox.TextChanged += new System.EventHandler(this.UpdateProjectIsModified);
            // 
            // pageTitleTextBox
            // 
            this.pageTitleTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.pageTitleTextBox.Location = new System.Drawing.Point(14, 23);
            this.pageTitleTextBox.Name = "pageTitleTextBox";
            this.pageTitleTextBox.Size = new System.Drawing.Size(491, 21);
            this.pageTitleTextBox.TabIndex = 1;
            this.pageTitleTextBox.TextChanged += new System.EventHandler(this.UpdateProjectIsModified);
            // 
            // packagesLabel
            // 
            this.packagesLabel.AutoSize = true;
            this.packagesLabel.BackColor = System.Drawing.SystemColors.Window;
            this.packagesLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.packagesLabel.Location = new System.Drawing.Point(15, 48);
            this.packagesLabel.Name = "packagesLabel";
            this.packagesLabel.Size = new System.Drawing.Size(56, 13);
            this.packagesLabel.Text = "Packages:";
            // 
            // browseOutputDirButton
            // 
            this.browseOutputDirButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.browseOutputDirButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.browseOutputDirButton.Location = new System.Drawing.Point(411, 104);
            this.browseOutputDirButton.Name = "browseOutputDirButton";
            this.browseOutputDirButton.Size = new System.Drawing.Size(94, 23);
            this.browseOutputDirButton.TabIndex = 4;
            this.browseOutputDirButton.Text = "Browse";
            this.browseOutputDirButton.UseVisualStyleBackColor = true;
            this.browseOutputDirButton.Click += new System.EventHandler(this.BrowseOutputDirButtonClick);
            // 
            // removeClasspathButton
            // 
            this.removeClasspathButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.removeClasspathButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.removeClasspathButton.Location = new System.Drawing.Point(411, 173);
            this.removeClasspathButton.Name = "removeClasspathButton";
            this.removeClasspathButton.Size = new System.Drawing.Size(94, 23);
            this.removeClasspathButton.TabIndex = 7;
            this.removeClasspathButton.Text = "Remove";
            this.removeClasspathButton.UseVisualStyleBackColor = true;
            this.removeClasspathButton.Click += new System.EventHandler(this.RemoveClasspathButtonClick);
            // 
            // browseClasspathButton
            // 
            this.browseClasspathButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.browseClasspathButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.browseClasspathButton.Location = new System.Drawing.Point(411, 145);
            this.browseClasspathButton.Name = "browseClasspathButton";
            this.browseClasspathButton.Size = new System.Drawing.Size(94, 23);
            this.browseClasspathButton.TabIndex = 6;
            this.browseClasspathButton.Text = "Browse";
            this.browseClasspathButton.UseVisualStyleBackColor = true;
            this.browseClasspathButton.Click += new System.EventHandler(this.BrowseClasspathButtonClick);
            // 
            // outputDirLabel
            // 
            this.outputDirLabel.AutoSize = true;
            this.outputDirLabel.BackColor = System.Drawing.SystemColors.Window;
            this.outputDirLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.outputDirLabel.Location = new System.Drawing.Point(15, 89);
            this.outputDirLabel.Name = "outputDirLabel";
            this.outputDirLabel.Size = new System.Drawing.Size(91, 13);
            this.outputDirLabel.Text = "Output directory:";
            // 
            // pageTitleLabel
            // 
            this.pageTitleLabel.AutoSize = true;
            this.pageTitleLabel.BackColor = System.Drawing.SystemColors.Window;
            this.pageTitleLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.pageTitleLabel.Location = new System.Drawing.Point(15, 7);
            this.pageTitleLabel.Name = "pageTitleLabel";
            this.pageTitleLabel.Size = new System.Drawing.Size(56, 13);
            this.pageTitleLabel.Text = "Page title:";
            // 
            // classpathLabel
            // 
            this.classpathLabel.AutoSize = true;
            this.classpathLabel.BackColor = System.Drawing.SystemColors.Window;
            this.classpathLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.classpathLabel.Location = new System.Drawing.Point(15, 130);
            this.classpathLabel.Name = "classpathLabel";
            this.classpathLabel.Size = new System.Drawing.Size(63, 13);
            this.classpathLabel.Text = "Classpaths:";
            // 
            // classpathListBox
            // 
            this.classpathListBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.classpathListBox.FormattingEnabled = true;
            this.classpathListBox.Location = new System.Drawing.Point(14, 146);
            this.classpathListBox.Name = "classpathListBox";
            this.classpathListBox.Size = new System.Drawing.Size(383, 56);
            this.classpathListBox.Font = SystemFonts.MenuFont;
            this.classpathListBox.TabIndex = 5;
            // 
            // tabControl
            // 
            this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl.Controls.Add(this.projectTabPage);
            this.tabControl.Controls.Add(this.settingsTabPage);
            this.tabControl.Controls.Add(this.outputTabPage);
            this.tabControl.Location = new System.Drawing.Point(11, 11);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(531, 310);
            this.tabControl.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.tabControl.TabIndex = 0;
            // 
            // settingsTabPage
            // 
            this.settingsTabPage.BackColor = System.Drawing.Color.White;
            this.settingsTabPage.Controls.Add(this.filesCheckBox);
            this.settingsTabPage.Controls.Add(this.saveSettingsButton);
            this.settingsTabPage.Controls.Add(this.asdocLocationTextBox);
            this.settingsTabPage.Controls.Add(this.asdocBrowseButton);
            this.settingsTabPage.Controls.Add(this.asdocLocationLabel);
            this.settingsTabPage.Controls.Add(this.as2apiLocationTextBox);
            this.settingsTabPage.Controls.Add(this.as2apiBrowseButton);
            this.settingsTabPage.Controls.Add(this.as2apiLocationLabel);
            this.settingsTabPage.Location = new System.Drawing.Point(4, 22);
            this.settingsTabPage.Name = "settingsTabPage";
            this.settingsTabPage.Size = new System.Drawing.Size(518, 284);
            this.settingsTabPage.TabIndex = 2;
            this.settingsTabPage.Text = "Settings";
            // 
            // filesCheckBox
            // 
            this.filesCheckBox.AutoSize = true;
            this.filesCheckBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.filesCheckBox.Location = new System.Drawing.Point(14, 92);
            this.filesCheckBox.Name = "filesCheckBox";
            this.filesCheckBox.Size = new System.Drawing.Size(256, 18);
            this.filesCheckBox.TabIndex = 15;
            this.filesCheckBox.Text = "Copy compiler related template files on compile";
            this.filesCheckBox.UseVisualStyleBackColor = true;
            this.filesCheckBox.CheckedChanged += new System.EventHandler(this.UpdateSettingsAreModified);
            // 
            // saveSettingsButton
            // 
            this.saveSettingsButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.saveSettingsButton.Location = new System.Drawing.Point(13, 120);
            this.saveSettingsButton.Name = "saveSettingsButton";
            this.saveSettingsButton.Size = new System.Drawing.Size(104, 23);
            this.saveSettingsButton.TabIndex = 16;
            this.saveSettingsButton.Text = "Save Settings";
            this.saveSettingsButton.UseVisualStyleBackColor = true;
            this.saveSettingsButton.Click += new System.EventHandler(this.SaveSettingsClick);
            // 
            // asdocLocationTextBox
            // 
            this.asdocLocationTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.asdocLocationTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.asdocLocationTextBox.Location = new System.Drawing.Point(14, 64);
            this.asdocLocationTextBox.Name = "asdocLocationTextBox";
            this.asdocLocationTextBox.Size = new System.Drawing.Size(387, 21);
            this.asdocLocationTextBox.TabIndex = 13;
            this.asdocLocationTextBox.TextChanged += new System.EventHandler(this.UpdateSettingsAreModified);
            // 
            // asdocBrowseButton
            // 
            this.asdocBrowseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.asdocBrowseButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.asdocBrowseButton.Location = new System.Drawing.Point(412, 62);
            this.asdocBrowseButton.Name = "asdocBrowseButton";
            this.asdocBrowseButton.Size = new System.Drawing.Size(94, 23);
            this.asdocBrowseButton.TabIndex = 14;
            this.asdocBrowseButton.Text = "Browse";
            this.asdocBrowseButton.UseVisualStyleBackColor = true;
            this.asdocBrowseButton.Click += new System.EventHandler(this.AsdocBrowseButtonClick);
            // 
            // asdocLocationLabel
            // 
            this.asdocLocationLabel.AutoSize = true;
            this.asdocLocationLabel.BackColor = System.Drawing.SystemColors.Window;
            this.asdocLocationLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.asdocLocationLabel.Location = new System.Drawing.Point(15, 48);
            this.asdocLocationLabel.Name = "asdocLocationLabel";
            this.asdocLocationLabel.Size = new System.Drawing.Size(82, 13);
            this.asdocLocationLabel.Text = "ASDoc location:";
            // 
            // as2apiLocationTextBox
            // 
            this.as2apiLocationTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.as2apiLocationTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.as2apiLocationTextBox.Location = new System.Drawing.Point(14, 23);
            this.as2apiLocationTextBox.Name = "as2apiLocationTextBox";
            this.as2apiLocationTextBox.Size = new System.Drawing.Size(387, 21);
            this.as2apiLocationTextBox.TabIndex = 11;
            this.as2apiLocationTextBox.TextChanged += new System.EventHandler(this.UpdateSettingsAreModified);
            // 
            // as2apiBrowseButton
            // 
            this.as2apiBrowseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.as2apiBrowseButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.as2apiBrowseButton.Location = new System.Drawing.Point(412, 22);
            this.as2apiBrowseButton.Name = "as2apiBrowseButton";
            this.as2apiBrowseButton.Size = new System.Drawing.Size(94, 23);
            this.as2apiBrowseButton.TabIndex = 12;
            this.as2apiBrowseButton.Text = "Browse";
            this.as2apiBrowseButton.UseVisualStyleBackColor = true;
            this.as2apiBrowseButton.Click += new System.EventHandler(this.As2apiBrowseButtonClick);
            // 
            // as2apiLocationLabel
            // 
            this.as2apiLocationLabel.AutoSize = true;
            this.as2apiLocationLabel.BackColor = System.Drawing.SystemColors.Window;
            this.as2apiLocationLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.as2apiLocationLabel.Location = new System.Drawing.Point(15, 8);
            this.as2apiLocationLabel.Name = "as2apiLocationLabel";
            this.as2apiLocationLabel.Size = new System.Drawing.Size(87, 13);
            this.as2apiLocationLabel.Text = "AS2API location:";
            // 
            // closeButton
            // 
            this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.closeButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.closeButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.closeButton.Location = new System.Drawing.Point(372, 330);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(11, 13);
            this.closeButton.Text = "Close";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Visible = false;
            this.closeButton.Click += new System.EventHandler(this.CloseButtonClick);
            // 
            // documentationLabel
            //
            this.documentationLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.documentationLabel.Text = "Help";
            this.documentationLabel.AutoSize = true;
            this.documentationLabel.Size = new System.Drawing.Size(11, 13);
            this.documentationLabel.Location = new System.Drawing.Point(510, 11);
            this.documentationLabel.Name = "documentationLabel";
            this.documentationLabel.Click += new EventHandler(this.DocumentationLabelClick);
            this.documentationLabel.Font = SystemFonts.MenuFont;
            // 
            // MainForm
            // 
            this.AcceptButton = this.generateDocsButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.closeButton;
            this.ClientSize = new System.Drawing.Size(551, 360);
            this.MinimumSize = new System.Drawing.Size(558, 385);
            this.Controls.Add(this.documentationLabel);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.generateDocsButton);
            this.Controls.Add(this.newProjectButton);
            this.Controls.Add(this.saveProjectButton);
            this.Controls.Add(this.openProjectButton);
            this.Controls.Add(this.newFromProjectButton);
            this.Name = "MainForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = " ActionScript Documentation Generator";
            this.Load += new System.EventHandler(this.MainFormLoaded);
            this.FormClosing += new FormClosingEventHandler(this.MainFormClosing);
            this.outputTabPage.ResumeLayout(false);
            this.projectTabPage.ResumeLayout(false);
            this.projectTabPage.PerformLayout();
            this.tabControl.ResumeLayout(false);
            this.settingsTabPage.ResumeLayout(false);
            this.settingsTabPage.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        #region Methods And Event Handlers

        /// <summary>
        /// Initializes the file dialogs.
        /// </summary>
        private void InitializeDialogs()
        {
            this.saveFileDialog.AddExtension = true;
            this.saveFileDialog.FileName = "Untitled.docproj";
            this.saveFileDialog.Filter = "Documentation Project|*.docproj";
            this.openFileDialog.Filter = "Documentation Project|*.docproj";
        }

        /// <summary>
        /// Initializes the process runner.
        /// </summary>
        private void InitializeProcessRunner()
        {
            this.processRunner = new ProcessRunner();
            this.processRunner.DataReceived += new DataReceivedEventHandler(this.ProcessDataReceived);
            this.processRunner.Exited += new EventHandler(this.ProcessExited);
        }

        /// <summary>
        /// Gets the application directory of the executable.
        /// </summary>
        private String AppDir
        {
            get { return Path.GetDirectoryName(Application.ExecutablePath); }
        }

        /// <summary>
        /// Gets the path to the default classes directory of MTASC.
        /// </summary>
        private String MtascStdDir
        {
            get 
            {
                String parentDir = Directory.GetParent(this.AppDir).FullName;
                return Path.Combine(parentDir, @"mtasc\std");
            }
        }

        /// <summary>
        /// Gets the path to the default classes directory of MTASC for FP8.
        /// </summary>
        private String MtascStd8Dir
        {
            get 
            {
                String parentDir = Directory.GetParent(this.AppDir).FullName;
                return Path.Combine(parentDir, @"mtasc\std8");
            }
        }

        /// <summary>
        /// Gets the path to the setting directory.
        /// </summary>
        private String SettingDir
        {
            get 
            {
                String local = Path.Combine(this.AppDir, @"..\..\.local");
                if (!File.Exists(local))
                {
                    String userAppDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                    String asDocGenDataDir = Path.Combine(userAppDir, @"FlashDevelop\Data\ASDocGen\");
                    if (!Directory.Exists(asDocGenDataDir)) Directory.CreateDirectory(asDocGenDataDir);
                    return asDocGenDataDir;
                }
                else
                {
                    String fdPath = Path.Combine(this.AppDir, @"..\..\");
                    fdPath = Path.GetFullPath(fdPath); /* Fix weird path */
                    String asDocGenDataDir = Path.Combine(fdPath, @"Data\ASDocGen\");
                    if (!Directory.Exists(asDocGenDataDir)) Directory.CreateDirectory(asDocGenDataDir);
                    return asDocGenDataDir;
                }
            }
        }

        /// <summary>
        /// Gets the path to the project or app directory.
        /// </summary>
        private String WorkingDir
        {
            get 
            {
                String projDir = Path.GetDirectoryName(this.saveFileDialog.FileName);
                if (Directory.Exists(projDir)) return projDir;
                else return this.AppDir;
            }
        }

        /// <summary>
        /// Gets or sets if the project is modified.
        /// </summary>
        private Boolean ProjectIsModified
        {
            get { return this.projectIsModified; }
            set
            {
                this.projectIsModified = value;
                if (this.projectIsModified) this.projectTabPage.Text = "Project*";
                else this.projectTabPage.Text = "Project";
            }
        }

        /// <summary>
        /// Gets or sets if the settings are modified.
        /// </summary>
        private Boolean SettingsAreModified
        {
            get { return this.settingsAreModified; }
            set
            {
                this.settingsAreModified = value;
                if (this.settingsAreModified)
                {
                    this.settingsTabPage.Text = "Settings*";
                    this.saveSettingsButton.Enabled = true;
                }
                else
                {
                    this.settingsTabPage.Text = "Settings";
                    this.saveSettingsButton.Enabled = false;
                }
            }
        }

        /// <summary>
        /// Updates the project file to the controls.
        /// </summary>
        private void UpdateProjectToSettings()
        {
            String classPaths = "";
            this.activeProject.pageTitle = this.pageTitleTextBox.Text;
            this.activeProject.sourcesList = this.packagesTextBox.Text;
            this.activeProject.outputDirectory = this.outputDirTextBox.Text;
            this.activeProject.activeCompiler = this.compilerComboBox.SelectedIndex;
            this.activeProject.extraOptions = this.extraOptionsTextBox.Text;
            for (Int32 i = 0; i < this.classpathListBox.Items.Count; i++)
            {
                String classPath = this.classpathListBox.Items[i].ToString();
                if (i == this.classpathListBox.Items.Count - 1) classPaths += classPath;
                else classPaths += classPath + ";";
            }
            this.activeProject.classPaths = classPaths;
        }

        /// <summary>
        /// Updates the control's values to the project file.
        /// </summary>
        private void UpdateProjectToDialog()
        {
            this.classpathListBox.Items.Clear();
            this.pageTitleTextBox.Text = this.activeProject.pageTitle;
            this.packagesTextBox.Text = this.activeProject.sourcesList;
            this.outputDirTextBox.Text = this.activeProject.outputDirectory;
            this.compilerComboBox.SelectedIndex = this.activeProject.activeCompiler;
            this.extraOptionsTextBox.Text = this.activeProject.extraOptions;
            String[] classPaths = this.activeProject.classPaths.Split(';');
            for (Int32 i = 0; i < classPaths.Length; i++)
            {
                this.classpathListBox.Items.Add(classPaths[i]);
            }
        }

        /// <summary>
        /// Updates the dialog based on the compiler/language.
        /// </summary>
        private void UpdateDialogForLanguage()
        {
            if (this.compilerComboBox.SelectedIndex == 0) // as2api
            {
                this.packagesLabel.Text = "Packages:";
            }
            else if (this.compilerComboBox.SelectedIndex == 1) // asdoc
            {
                this.packagesLabel.Text = "Exclude classes:";
            }
        }

        /// <summary>
        /// Loads the application settings from the setting file.
        /// </summary>
        private void LoadSettings()
        {
            this.appSettings = new Settings();
            this.activeProject = new Project();
            String settingFile = Path.Combine(this.SettingDir, "Settings.xml");
            if (File.Exists(settingFile))
            {
                Object settings = ObjectSerializer.Deserialize(settingFile, this.appSettings);
                this.appSettings = (Settings)settings;
            }
            // Try to find asdoc path from: AppMan's Apps or FD/Tools/flexsdk/
            if (String.IsNullOrEmpty(this.appSettings.asdocLocation))
            {
                try
                {
                    this.appSettings.asdocLocation = DetectSDKLocation();
                }
                catch {}
            }
            if (!File.Exists(settingFile))
            {
                ObjectSerializer.Serialize(settingFile, this.appSettings);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private string DetectSDKLocation()
        {
            String asdocPath = String.Empty;
            String asdocPath2 = String.Empty;
            String parentDir = Directory.GetParent(this.AppDir).FullName;
            String userAppDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            String appManDir = Path.Combine(userAppDir, @"FlashDevelop\Apps\flexsdk");
            if (Directory.Exists(appManDir))
            {
                String[] versionDirs = Directory.GetDirectories(appManDir);
                foreach (string versionDir in versionDirs)
                {
                    if (Directory.Exists(versionDir))
                    {
                        asdocPath = Path.Combine(versionDir, @"bin\asdoc.exe");
                        asdocPath2 = Path.Combine(versionDir, @"bin\asdoc.bat");
                        if (File.Exists(asdocPath)) return Path.GetDirectoryName(asdocPath);
                        if (File.Exists(asdocPath2)) return Path.GetDirectoryName(asdocPath2);
                    }
                }
            }
            appManDir = Path.Combine(userAppDir, @"FlashDevelop\Apps\flexairsdk");
            if (Directory.Exists(appManDir))
            {
                String[] versionDirs = Directory.GetDirectories(appManDir);
                foreach (string versionDir in versionDirs)
                {
                    if (Directory.Exists(versionDir))
                    {
                        asdocPath = Path.Combine(versionDir, @"bin\asdoc.exe");
                        asdocPath2 = Path.Combine(versionDir, @"bin\asdoc.bat");
                        if (File.Exists(asdocPath)) return Path.GetDirectoryName(asdocPath);
                        if (File.Exists(asdocPath2)) return Path.GetDirectoryName(asdocPath2);
                    }
                }
            }
            appManDir = Path.Combine(userAppDir, @"FlashDevelop\Apps\ascsdk");
            if (Directory.Exists(appManDir))
            {
                String[] versionDirs = Directory.GetDirectories(appManDir);
                foreach (string versionDir in versionDirs)
                {
                    if (Directory.Exists(versionDir))
                    {
                        asdocPath = Path.Combine(versionDir, @"bin\asdoc.exe");
                        asdocPath2 = Path.Combine(versionDir, @"bin\asdoc.bat");
                        if (File.Exists(asdocPath)) return Path.GetDirectoryName(asdocPath);
                        if (File.Exists(asdocPath2)) return Path.GetDirectoryName(asdocPath2);
                    }
                }
            }
            asdocPath = Path.Combine(parentDir, @"flexsdk\bin\asdoc.exe");
            asdocPath2 = Path.Combine(parentDir, @"flexsdk\bin\asdoc.bat");
            if (File.Exists(asdocPath)) return Path.GetDirectoryName(asdocPath);
            if (File.Exists(asdocPath2)) return Path.GetDirectoryName(asdocPath2);
            return "";
        }

        /// <summary>
        /// Saves the application settings to a setting file.
        /// </summary>
        private void SaveSettings()
        {
            this.appSettings.copyCustomFiles = this.filesCheckBox.Checked;
            this.appSettings.as2apiLocation = this.as2apiLocationTextBox.Text;
            this.appSettings.asdocLocation = this.asdocLocationTextBox.Text;
            String settingFile = Path.Combine(this.SettingDir, "Settings.xml");
            ObjectSerializer.Serialize(settingFile, this.appSettings);
            this.SettingsAreModified = false;
        }

        /// <summary>
        /// Applies the setting values to the controls.
        /// </summary>
        private void MainFormLoaded(Object sender, EventArgs e)
        {
            this.filesCheckBox.Checked = this.appSettings.copyCustomFiles;
            this.as2apiLocationTextBox.Text = this.appSettings.as2apiLocation;
            this.asdocLocationTextBox.Text = this.appSettings.asdocLocation;
            this.compilerComboBox.SelectedIndex = 0;
            this.SettingsAreModified = false;
            this.ProjectIsModified = false;
            this.ProcessStartArguments();
        }

        /// <summary>
        /// If there are unsaved changes, ask user to confirm close.
        /// </summary>
        private void MainFormClosing(Object sender, FormClosingEventArgs e)
        {
            if (this.ProjectIsModified || this.SettingsAreModified)
            {
                String message = "You have unsaved changes. Are you sure you want to quit?";
                if (MessageBox.Show(message, " Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    e.Cancel = true;
                }
            }
        }

        /// <summary>
        /// Processes the incoming arguments
        /// </summary>
        private void ProcessStartArguments()
        {
            foreach (String argument in this.arguments)
            {
                if (File.Exists(argument) && argument.EndsWith(".docproj"))
                {
                    String file = Path.GetFullPath(argument);
                    Object project = ObjectSerializer.Deserialize(file, this.activeProject);
                    this.activeProject = (Project)project;
                    this.UpdateProjectToDialog();
                    this.ProjectIsModified = false;
                    this.saveFileDialog.FileName = this.openFileDialog.FileName = file;
                    this.tabControl.SelectedTab = this.projectTabPage;
                    return;
                }
            }
        }

        /// <summary>
        /// Marks the settings as modified.
        /// </summary>
        private void UpdateSettingsAreModified(Object sender, EventArgs e)
        {
            this.SettingsAreModified = true;
        }

        /// <summary>
        /// Marks the project as modified and updates the dialog.
        /// </summary>
        private void UpdateProjectIsModified(Object sender, EventArgs e)
        {
            this.ProjectIsModified = true;
            this.UpdateDialogForLanguage();
        }

        /// <summary>
        /// Closes the application if user clicks ESC.
        /// </summary>
        private void CloseButtonClick(Object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Saves the application setting if user clicks.
        /// </summary>
        private void SaveSettingsClick(Object sender, EventArgs e)
        {
            this.SaveSettings();
        }

        /// <summary>
        /// Opens the documentation in a browser
        /// </summary>
        private void DocumentationLabelClick(Object sender, EventArgs e)
        {
            String helpDir = Path.Combine(this.AppDir, "Help");
            Process.Start(Path.Combine(helpDir, "index.html"));
        }

        /// <summary>
        /// Browses for the output directory of the documentation.
        /// </summary>
        private void BrowseOutputDirButtonClick(Object sender, EventArgs e)
        {
            if (this.folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                this.outputDirTextBox.Text = this.folderBrowserDialog.SelectedPath;
            }
        }

        /// <summary>
        /// Browses for a new classpath for the documentation.
        /// </summary>
        private void BrowseClasspathButtonClick(Object sender, EventArgs e)
        {
            if (this.folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                this.classpathListBox.Items.Add(this.folderBrowserDialog.SelectedPath);
                this.ProjectIsModified = true;
            }
        }

        /// <summary>
        /// Removes a classpath from the list of classpaths.
        /// </summary>
        private void RemoveClasspathButtonClick(Object sender, EventArgs e)
        {
            if (this.classpathListBox.SelectedIndices.Count > 0)
            {
                Int32 index = this.classpathListBox.SelectedIndex;
                this.classpathListBox.Items.RemoveAt(index);
                this.ProjectIsModified = true;
            }

        }

        /// <summary>
        /// Browses for the location of the as2api executable.
        /// </summary>
        private void As2apiBrowseButtonClick(Object sender, EventArgs e)
        {
            if (this.folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                this.as2apiLocationTextBox.Text = this.folderBrowserDialog.SelectedPath;
            }
        }

        /// <summary>
        /// Browses for the location of the asdoc executable.
        /// </summary>
        private void AsdocBrowseButtonClick(Object sender, EventArgs e)
        {
            if (this.folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                this.asdocLocationTextBox.Text = this.folderBrowserDialog.SelectedPath;
            }
        }

        /// <summary>
        /// Browses for a project to be opened.
        /// </summary>
        private void OpenProjectButtonClick(Object sender, EventArgs e)
        {
            if (this.openFileDialog.ShowDialog() == DialogResult.OK)
            {
                String file = this.openFileDialog.FileName;
                Object project = ObjectSerializer.Deserialize(file, this.activeProject);
                this.activeProject = (Project)project;
                this.UpdateProjectToDialog();
                this.ProjectIsModified = false;
                this.saveFileDialog.FileName = this.openFileDialog.FileName;
                this.tabControl.SelectedTab = this.projectTabPage;
            }
        }

        /// <summary>
        /// Saves the current project or browses for location of it.
        /// </summary>
        private void SaveProjectButtonClick(Object sender, EventArgs e)
        {
            if (this.saveFileDialog.FileName == "Untitled.docproj")
            {
                if (this.saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    this.UpdateProjectToSettings();
                    String file = this.saveFileDialog.FileName;
                    ObjectSerializer.Serialize(file, this.activeProject);
                    this.ProjectIsModified = false;
                }
            }
            else 
            {   
                this.UpdateProjectToSettings();
                String file = this.saveFileDialog.FileName;
                ObjectSerializer.Serialize(file, this.activeProject);
                this.ProjectIsModified = false;
            }
        }

        /// <summary>
        /// Creates a new project and clears the controls.
        /// </summary>
        private void NewProjectButtonClick(Object sender, EventArgs e)
        {
            this.saveFileDialog.FileName = "Untitled.docproj";
            this.activeProject = new Project();
            this.pageTitleTextBox.Text = "";
            this.packagesTextBox.Text = "";
            this.outputDirTextBox.Text = "";
            this.classpathListBox.Items.Clear();
            this.compilerComboBox.SelectedIndex = 0;
            this.extraOptionsTextBox.Text = "";
            this.ProjectIsModified = false;
            this.tabControl.SelectedTab = this.projectTabPage;
        }

        /// <summary>
        /// Creates a new project and clears the controls.
        /// </summary>
        private void NewFromProjectButtonClick(Object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Project Files (*.as2proj,*.as3proj)|*.as2proj;*.as3proj";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                String path = ofd.FileName;
                Regex nameRegex = new Regex(@"[\w\d\(\) -]+\.");
                FileStream project = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
                MatchCollection matches = nameRegex.Matches(path);
                XmlDocument projXml = new XmlDocument(); projXml.Load(project);
                XmlNodeList node = projXml.GetElementsByTagName("class");
                String[] paths = new String[node.Count];
                for (Int32 i = 0; i < node.Count; i++)
                {
                    XmlAttributeCollection attributes = node[i].Attributes;
                    if (attributes[0].Name == "path") paths[i] = attributes[0].Value;
                }
                Int32 version = Int32.Parse(path.Substring(path.Length - 5, 1)) - 2;
                String name = matches[matches.Count - 1].Value;
                name = name.Substring(0, name.Length - 1);
                this.pageTitleTextBox.Text = name;
                this.classpathListBox.Items.Clear();
                this.saveFileDialog.FileName = "Untitled.docproj";
                this.compilerComboBox.SelectedIndex = version;
                this.classpathListBox.Items.AddRange(paths);
            }
        }

        /// <summary>
        /// Generates the documentation based on the current state.
        /// </summary>
        private void GenerateDocsButtonClick(Object sender, EventArgs e)
        {
            this.UpdateProjectToSettings();
            this.BuildDocumentation();
        }

        /// <summary>
        /// Enables or disables the controls (while compiling).
        /// </summary>
        private void SetControlsEnabled(Boolean value)
        {
            this.generateDocsButton.Enabled = value;
        }

        /// <summary>
        /// Builds the bat file contents, saves and runs it. Also disables the requireq controls.
        /// </summary>
        private void BuildDocumentation()
        {
            try
            {
                String contents = "";
                if (this.activeProject.activeCompiler == 0) // as2api
                {
                    contents += "\"" + Path.Combine(this.appSettings.as2apiLocation, "as2api.exe") + "\"";
                    if (this.activeProject.pageTitle.Trim() != String.Empty)
                    {
                        contents += " --title " + "\"" + this.activeProject.pageTitle + "\"";
                    }
                    if (this.activeProject.outputDirectory.Trim() != String.Empty)
                    {
                        contents += " --output-dir " + "\"" + this.activeProject.outputDirectory + "\"";
                    }
                    if (this.activeProject.classPaths.Trim() != String.Empty)
                    {
                        String classPaths = this.activeProject.classPaths;
                        classPaths = classPaths + ";" + this.MtascStdDir + ";" + this.MtascStd8Dir;
                        contents += " --classpath " + "\"" + classPaths + "\"";
                    }
                    if (this.activeProject.extraOptions.Trim() != String.Empty)
                    {
                        contents += " " + this.activeProject.extraOptions;
                    }
                    if (this.activeProject.sourcesList.Trim() != String.Empty)
                    {
                        contents += " " + this.activeProject.sourcesList;
                    }
                }
                else if (this.activeProject.activeCompiler == 1) // asdoc
                {
                    String asdocPath = Path.Combine(this.appSettings.asdocLocation, "asdoc.exe");
                    String asdocPath2 = Path.Combine(this.appSettings.asdocLocation, "asdoc.bat");
                    if (File.Exists(asdocPath)) contents += "\"" + asdocPath + "\"";
                    if (File.Exists(asdocPath2)) contents += "\"" + asdocPath2 + "\"";
                    if (this.activeProject.classPaths.Trim() != String.Empty)
                    {
                        String[] classPaths = this.activeProject.classPaths.Split(';');
                        for (Int32 i = 0; i < classPaths.Length; i++)
                        {
                            contents += " -doc-sources " + "\"" + classPaths[i] + "\"";
                        }
                    }
                    if (this.activeProject.pageTitle.Trim() != String.Empty)
                    {
                        contents += " -main-title " + "\"" + this.activeProject.pageTitle + "\"";
                    }
                    if (this.activeProject.outputDirectory.Trim() != String.Empty)
                    {
                        contents += " -output " + "\"" + this.activeProject.outputDirectory + "\"";
                    }
                    if (this.activeProject.sourcesList.Trim() != String.Empty)
                    {
                        String[] classes = this.activeProject.sourcesList.TrimEnd().Split(' ');
                        foreach (String cls in classes) contents += " -exclude-classes " + "\"" + cls + "\"";
                    }
                    if (this.activeProject.extraOptions.Trim() != String.Empty)
                    {
                        contents += " " + this.activeProject.extraOptions;
                    }
                    if (contents.EndsWith("asdoc.exe\"") || contents.EndsWith("asdoc.bat\""))
                    {
                        contents += " -help advanced";
                    }
                }
                String batFile = Path.Combine(this.SettingDir, "ASDocGen.bat");
                String converted = this.ChangeEncoding(contents, Encoding.UTF8, Encoding.Default);
                File.WriteAllText(batFile, ArgsProcessor.Process(converted), Encoding.Default);
                this.SetControlsEnabled(false);
                this.outputTextBox.Text = "";
                this.RunProcess(batFile);
            }
            catch (Exception ex)
            {
                DialogHelper.ShowError(ex.Message);
            }
        }

        /// <summary>
        /// Converts text to another encoding
        /// </summary>
        private String ChangeEncoding(String text, Encoding from, Encoding to)
        {
            try
            {
                Byte[] fromBytes = from.GetBytes(text);
                Byte[] toBytes = Encoding.Convert(from, to, fromBytes);
                return to.GetString(toBytes);
            }
            catch (Exception ex)
            {
                DialogHelper.ShowError(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Copies the compiler specific files to the output directory.
        /// </summary>
        private void CopyCustomStyleFiles()
        {
            try
            {
                String filesDir = Path.Combine(this.AppDir, "Files");
                if (Directory.Exists(filesDir))
                {
                    String[] files = Directory.GetFiles(filesDir);
                    String compilerFlag = this.activeProject.activeCompiler == 0 ? "as2api-" : "asdoc-";
                    foreach (String file in files)
                    {
                        String name = Path.GetFileName(file);
                        String path = this.activeProject.outputDirectory;
                        if (path.Trim() != String.Empty && name.StartsWith(compilerFlag))
                        {
                            String outputFile = Path.Combine(path, name).Replace(compilerFlag, "");
                            File.Copy(file, outputFile, true);
                        }
                    }
                }
            } 
            catch (Exception ex)
            {
                DialogHelper.ShowError(ex.Message);
            }
        }

        /// <summary>
        /// Runs the process for the specified file.
        /// </summary>
        private void RunProcess(String file)
        {
            this.processRunner.Run(file, this.WorkingDir);
        }

        /// <summary>
        /// When process data is recieved display it in the output tab.
        /// </summary>
        private void ProcessDataReceived(Object sender, DataReceivedEventArgs e)
        {
            if (this.InvokeRequired) this.BeginInvoke((MethodInvoker)delegate { this.ProcessDataReceived(sender, e); });
            else if (!String.IsNullOrEmpty(e.Data))
            {
                this.outputTextBox.AppendText(e.Data + Environment.NewLine);
            }
        }

        /// <summary>
        /// When process finishes, copies the custom files and enables the controls.
        /// </summary>
        private void ProcessExited(Object sender, EventArgs e)
        {
            if (this.InvokeRequired) this.BeginInvoke((MethodInvoker)delegate { this.ProcessExited(sender, e); });
            else
            {
                this.CopyCustomStyleFiles();
                this.tabControl.SelectedTab = this.outputTabPage;
                this.SetControlsEnabled(true);
            }
        }

        #endregion

    }

}
