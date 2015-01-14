using System;
using System.IO;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ProjectManager.Projects;
using ProjectManager.Helpers;
using PluginCore;
using PluginCore.Managers;
using PluginCore.Localization;
using ProjectManager.Controls.AS2;
using ProjectManager.Controls.AS3;
using PluginCore.Controls;
using System.Collections;
using ProjectManager.Actions;
using System.Collections.Generic;
using Ookii.Dialogs;

namespace ProjectManager.Controls
{
	public class PropertiesDialog : SmartForm
	{
		#region Form Designer

		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.TabPage movieTab;
		private System.Windows.Forms.TextBox outputSwfBox;
		private System.Windows.Forms.Label exportinLabel;
		private System.Windows.Forms.Label pxLabel;
		private System.Windows.Forms.Label fpsLabel;
        private System.Windows.Forms.Label bgcolorLabel;
		private System.Windows.Forms.Label framerateLabel;
		private System.Windows.Forms.Label dimensionsLabel;
        private System.Windows.Forms.Label xLabel;
		private System.Windows.Forms.ColorDialog colorDialog;
        private System.Windows.Forms.Button outputBrowseButton;
		private System.Windows.Forms.GroupBox generalGroupBox;
		private System.Windows.Forms.GroupBox playGroupBox;
        private System.Windows.Forms.TabPage classpathsTab;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnGlobalClasspaths;
        private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.TabPage buildTab;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.TextBox preBuildBox;
		private System.Windows.Forms.Button preBuilderButton;
		private System.Windows.Forms.GroupBox groupBox5;
		private System.Windows.Forms.Button postBuilderButton;
		private System.Windows.Forms.TextBox postBuildBox;
		private System.Windows.Forms.ToolTip agressiveTip;
        private System.Windows.Forms.CheckBox alwaysExecuteCheckBox;
        private System.Windows.Forms.ComboBox testMovieCombo;
        private System.Windows.Forms.TabPage compilerTab;
        private System.Windows.Forms.PropertyGrid propertyGrid;
        private System.Windows.Forms.Label outputTypeLabel;
        private System.Windows.Forms.ComboBox outputCombo;
        private System.Windows.Forms.GroupBox platformGroupBox;
        private System.Windows.Forms.ComboBox platformCombo;
        private System.Windows.Forms.Button editCommandButton;
        private System.Windows.Forms.ComboBox versionCombo;
        private System.Windows.Forms.TabPage sdkTabPage;
        private System.Windows.Forms.Button manageButton;
        private System.Windows.Forms.ComboBox sdkComboBox;
        private System.Windows.Forms.GroupBox sdkGroupBox;
        private System.Windows.Forms.GroupBox customGroupBox;
        private System.Windows.Forms.Label labelUseGlobal;
        private System.Windows.Forms.PictureBox warningImage;
        private System.Windows.Forms.Label labelWarning;
        private System.Windows.Forms.Button browseButton;
        private System.Windows.Forms.TextBox customTextBox;
        private System.Windows.Forms.Label labelUseCustom;
        protected System.Windows.Forms.TabControl tabControl;
        protected System.Windows.Forms.TextBox colorTextBox;
        protected System.Windows.Forms.Label colorLabel;
        protected System.Windows.Forms.TextBox fpsTextBox;
        protected System.Windows.Forms.TextBox heightTextBox;
        protected System.Windows.Forms.TextBox widthTextBox;
        private ClasspathControl classpathControl;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer Generated Code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnApply = new System.Windows.Forms.Button();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.movieTab = new System.Windows.Forms.TabPage();
            this.platformGroupBox = new System.Windows.Forms.GroupBox();
            this.versionCombo = new System.Windows.Forms.ComboBox();
            this.platformCombo = new System.Windows.Forms.ComboBox();
            this.outputTypeLabel = new System.Windows.Forms.Label();
            this.outputCombo = new System.Windows.Forms.ComboBox();
            this.generalGroupBox = new System.Windows.Forms.GroupBox();
            this.widthTextBox = new System.Windows.Forms.TextBox();
            this.outputBrowseButton = new System.Windows.Forms.Button();
            this.heightTextBox = new System.Windows.Forms.TextBox();
            this.xLabel = new System.Windows.Forms.Label();
            this.dimensionsLabel = new System.Windows.Forms.Label();
            this.colorTextBox = new System.Windows.Forms.TextBox();
            this.framerateLabel = new System.Windows.Forms.Label();
            this.outputSwfBox = new System.Windows.Forms.TextBox();
            this.fpsTextBox = new System.Windows.Forms.TextBox();
            this.exportinLabel = new System.Windows.Forms.Label();
            this.colorLabel = new System.Windows.Forms.Label();
            this.pxLabel = new System.Windows.Forms.Label();
            this.bgcolorLabel = new System.Windows.Forms.Label();
            this.fpsLabel = new System.Windows.Forms.Label();
            this.playGroupBox = new System.Windows.Forms.GroupBox();
            this.testMovieCombo = new System.Windows.Forms.ComboBox();
            this.editCommandButton = new System.Windows.Forms.Button();
            this.classpathsTab = new System.Windows.Forms.TabPage();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnGlobalClasspaths = new System.Windows.Forms.Button();
            this.buildTab = new System.Windows.Forms.TabPage();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.alwaysExecuteCheckBox = new System.Windows.Forms.CheckBox();
            this.postBuilderButton = new System.Windows.Forms.Button();
            this.postBuildBox = new System.Windows.Forms.TextBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.preBuilderButton = new System.Windows.Forms.Button();
            this.preBuildBox = new System.Windows.Forms.TextBox();
            this.compilerTab = new System.Windows.Forms.TabPage();
            this.propertyGrid = new System.Windows.Forms.PropertyGrid();
            this.colorDialog = new System.Windows.Forms.ColorDialog();
            this.sdkTabPage = new System.Windows.Forms.TabPage();
            this.manageButton = new System.Windows.Forms.Button();
            this.sdkComboBox = new System.Windows.Forms.ComboBox();
            this.sdkGroupBox = new System.Windows.Forms.GroupBox();
            this.labelUseGlobal = new System.Windows.Forms.Label();
            this.customGroupBox = new System.Windows.Forms.GroupBox();
            this.browseButton = new System.Windows.Forms.Button();
            this.customTextBox = new System.Windows.Forms.TextBox();
            this.labelUseCustom = new System.Windows.Forms.Label();
            this.warningImage = new System.Windows.Forms.PictureBox();
            this.labelWarning = new System.Windows.Forms.Label();
            this.agressiveTip = new System.Windows.Forms.ToolTip(this.components);
            this.sdkTabPage.SuspendLayout();
            this.sdkGroupBox.SuspendLayout();
            this.customGroupBox.SuspendLayout();
            this.tabControl.SuspendLayout();
            this.movieTab.SuspendLayout();
            this.platformGroupBox.SuspendLayout();
            this.generalGroupBox.SuspendLayout();
            this.playGroupBox.SuspendLayout();
            this.classpathsTab.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.buildTab.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.compilerTab.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(116, 316);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 21);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "&OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(197, 316);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 21);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnApply
            // 
            this.btnApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnApply.Enabled = false;
            this.btnApply.Location = new System.Drawing.Point(278, 316);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(75, 21);
            this.btnApply.TabIndex = 3;
            this.btnApply.Text = "&Apply";
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // tabControl
            // 
            this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl.Controls.Add(this.movieTab);
            this.tabControl.Controls.Add(this.sdkTabPage);
            this.tabControl.Controls.Add(this.classpathsTab);
            this.tabControl.Controls.Add(this.buildTab);
            this.tabControl.Controls.Add(this.compilerTab);
            this.tabControl.Location = new System.Drawing.Point(12, 12);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(342, 298);
            this.tabControl.TabIndex = 0;
            // 
            // movieTab
            // 
            this.movieTab.Controls.Add(this.platformGroupBox);
            this.movieTab.Controls.Add(this.generalGroupBox);
            this.movieTab.Controls.Add(this.playGroupBox);
            this.movieTab.Location = new System.Drawing.Point(4, 22);
            this.movieTab.Name = "movieTab";
            this.movieTab.Size = new System.Drawing.Size(334, 272);
            this.movieTab.TabIndex = 0;
            this.movieTab.Text = "Output";
            this.movieTab.UseVisualStyleBackColor = true;
            // 
            // platformGroupBox
            //
            this.platformGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.platformGroupBox.Controls.Add(this.versionCombo);
            this.platformGroupBox.Controls.Add(this.platformCombo);
            this.platformGroupBox.Controls.Add(this.outputTypeLabel);
            this.platformGroupBox.Controls.Add(this.outputCombo);
            this.platformGroupBox.Location = new System.Drawing.Point(8, 3);
            this.platformGroupBox.Name = "platformGroupBox";
            this.platformGroupBox.Size = new System.Drawing.Size(319, 74);
            this.platformGroupBox.TabIndex = 4;
            this.platformGroupBox.TabStop = false;
            this.platformGroupBox.Text = "Platform";
            // 
            // versionCombo
            // 
            this.versionCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.versionCombo.Location = new System.Drawing.Point(234, 19);
            this.versionCombo.Name = "versionCombo";
            this.versionCombo.Size = new System.Drawing.Size(75, 21);
            this.versionCombo.TabIndex = 13;
            // 
            // platformCombo
            // 
            this.platformCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.platformCombo.Location = new System.Drawing.Point(11, 19);
            this.platformCombo.Name = "platformCombo";
            this.platformCombo.Size = new System.Drawing.Size(219, 21);
            this.platformCombo.TabIndex = 12;
            //
            // outputTypeLabel
            //
            this.outputTypeLabel.Location = new System.Drawing.Point(9, 44);
            this.outputTypeLabel.Name = "outputTypeLabel";
            this.outputTypeLabel.Size = new System.Drawing.Size(96, 18);
            this.outputTypeLabel.TabIndex = 13;
            this.outputTypeLabel.Text = "Compilation &Target:";
            this.outputTypeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // outputCombo
            // 
            this.outputCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.outputCombo.Location = new System.Drawing.Point(107, 45);
            this.outputCombo.Name = "outputCombo";
            this.outputCombo.Size = new System.Drawing.Size(202, 21);
            this.outputCombo.TabIndex = 14;
            // 
            // generalGroupBox
            // 
            this.generalGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.generalGroupBox.Controls.Add(this.widthTextBox);
            this.generalGroupBox.Controls.Add(this.outputBrowseButton);
            this.generalGroupBox.Controls.Add(this.heightTextBox);
            this.generalGroupBox.Controls.Add(this.xLabel);
            this.generalGroupBox.Controls.Add(this.dimensionsLabel);
            this.generalGroupBox.Controls.Add(this.colorTextBox);
            this.generalGroupBox.Controls.Add(this.framerateLabel);
            this.generalGroupBox.Controls.Add(this.outputSwfBox);
            this.generalGroupBox.Controls.Add(this.fpsTextBox);
            this.generalGroupBox.Controls.Add(this.exportinLabel);
            this.generalGroupBox.Controls.Add(this.colorLabel);
            this.generalGroupBox.Controls.Add(this.pxLabel);
            this.generalGroupBox.Controls.Add(this.bgcolorLabel);
            this.generalGroupBox.Controls.Add(this.fpsLabel);
            this.generalGroupBox.Location = new System.Drawing.Point(8, 82);
            this.generalGroupBox.Name = "generalGroupBox";
            this.generalGroupBox.Size = new System.Drawing.Size(319, 129);
            this.generalGroupBox.TabIndex = 6;
            this.generalGroupBox.TabStop = false;
            this.generalGroupBox.Text = "General";
            // 
            // widthTextBox
            // 
            this.widthTextBox.Location = new System.Drawing.Point(108, 44);
            this.widthTextBox.MaxLength = 4;
            this.widthTextBox.Name = "widthTextBox";
            this.widthTextBox.Size = new System.Drawing.Size(32, 20);
            this.widthTextBox.TabIndex = 4;
            this.widthTextBox.Text = "500";
            this.widthTextBox.TextChanged += new System.EventHandler(this.widthTextBox_TextChanged);
            // 
            // outputBrowseButton
            // 
            this.outputBrowseButton.Location = new System.Drawing.Point(233, 15);
            this.outputBrowseButton.Name = "outputBrowseButton";
            this.outputBrowseButton.Size = new System.Drawing.Size(76, 21);
            this.outputBrowseButton.TabIndex = 2;
            this.outputBrowseButton.Text = "&Browse...";
            this.outputBrowseButton.Click += new System.EventHandler(this.outputBrowseButton_Click);
            // 
            // heightTextBox
            // 
            this.heightTextBox.Location = new System.Drawing.Point(161, 44);
            this.heightTextBox.MaxLength = 4;
            this.heightTextBox.Name = "heightTextBox";
            this.heightTextBox.Size = new System.Drawing.Size(32, 20);
            this.heightTextBox.TabIndex = 5;
            this.heightTextBox.Text = "300";
            this.heightTextBox.TextChanged += new System.EventHandler(this.heightTextBox_TextChanged);
            // 
            // xLabel
            // 
            this.xLabel.Location = new System.Drawing.Point(145, 44);
            this.xLabel.Name = "xLabel";
            this.xLabel.Size = new System.Drawing.Size(13, 17);
            this.xLabel.TabIndex = 21;
            this.xLabel.Text = "x";
            this.xLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // dimensionsLabel
            // 
            this.dimensionsLabel.Location = new System.Drawing.Point(8, 46);
            this.dimensionsLabel.Name = "dimensionsLabel";
            this.dimensionsLabel.Size = new System.Drawing.Size(96, 13);
            this.dimensionsLabel.TabIndex = 3;
            this.dimensionsLabel.Text = "&Dimensions:";
            this.dimensionsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // colorTextBox
            // 
            this.colorTextBox.Location = new System.Drawing.Point(139, 71);
            this.colorTextBox.MaxLength = 7;
            this.colorTextBox.Name = "colorTextBox";
            this.colorTextBox.Size = new System.Drawing.Size(55, 20);
            this.colorTextBox.TabIndex = 9;
            this.colorTextBox.Text = "#FFFFFF";
            this.colorTextBox.TextChanged += new System.EventHandler(this.colorTextBox_TextChanged);
            // 
            // framerateLabel
            // 
            this.framerateLabel.Location = new System.Drawing.Point(16, 97);
            this.framerateLabel.Name = "framerateLabel";
            this.framerateLabel.Size = new System.Drawing.Size(88, 17);
            this.framerateLabel.TabIndex = 8;
            this.framerateLabel.Text = "&Framerate:";
            this.framerateLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // outputSwfBox
            // 
            this.outputSwfBox.Location = new System.Drawing.Point(108, 17);
            this.outputSwfBox.Name = "outputSwfBox";
            this.outputSwfBox.Size = new System.Drawing.Size(121, 20);
            this.outputSwfBox.TabIndex = 1;
            this.outputSwfBox.TextChanged += new System.EventHandler(this.outputSwfBox_TextChanged);
            // 
            // fpsTextBox
            // 
            this.fpsTextBox.Location = new System.Drawing.Point(109, 98);
            this.fpsTextBox.MaxLength = 3;
            this.fpsTextBox.Name = "fpsTextBox";
            this.fpsTextBox.Size = new System.Drawing.Size(27, 20);
            this.fpsTextBox.TabIndex = 37;
            this.fpsTextBox.Text = "30";
            this.fpsTextBox.TextChanged += new System.EventHandler(this.fpsTextBox_TextChanged);
            // 
            // exportinLabel
            // 
            this.exportinLabel.Location = new System.Drawing.Point(8, 16);
            this.exportinLabel.Name = "exportinLabel";
            this.exportinLabel.Size = new System.Drawing.Size(96, 18);
            this.exportinLabel.TabIndex = 0;
            this.exportinLabel.Text = "&Output File:";
            this.exportinLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // colorLabel
            // 
            this.colorLabel.BackColor = System.Drawing.Color.White;
            this.colorLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.colorLabel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.colorLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.colorLabel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.colorLabel.Location = new System.Drawing.Point(109, 72);
            this.colorLabel.Name = "colorLabel";
            this.colorLabel.Size = new System.Drawing.Size(17, 16);
            this.colorLabel.TabIndex = 7;
            this.colorLabel.Click += new System.EventHandler(this.colorLabel_Click);
            // 
            // pxLabel
            // 
            this.pxLabel.Location = new System.Drawing.Point(199, 47);
            this.pxLabel.Name = "pxLabel";
            this.pxLabel.Size = new System.Drawing.Size(19, 14);
            this.pxLabel.TabIndex = 30;
            this.pxLabel.Text = "px";
            // 
            // bgcolorLabel
            // 
            this.bgcolorLabel.Location = new System.Drawing.Point(5, 70);
            this.bgcolorLabel.Name = "bgcolorLabel";
            this.bgcolorLabel.Size = new System.Drawing.Size(99, 18);
            this.bgcolorLabel.TabIndex = 6;
            this.bgcolorLabel.Text = "Background &color:";
            this.bgcolorLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // fpsLabel
            // 
            this.fpsLabel.Location = new System.Drawing.Point(141, 101);
            this.fpsLabel.Name = "fpsLabel";
            this.fpsLabel.Size = new System.Drawing.Size(32, 17);
            this.fpsLabel.TabIndex = 28;
            this.fpsLabel.Text = "fps";
            // 
            // playGroupBox
            // 
            this.playGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.playGroupBox.Controls.Add(this.testMovieCombo);
            this.playGroupBox.Controls.Add(this.editCommandButton);
            this.playGroupBox.Location = new System.Drawing.Point(8, 216);
            this.playGroupBox.Name = "playGroupBox";
            this.playGroupBox.Size = new System.Drawing.Size(319, 47);
            this.playGroupBox.TabIndex = 7;
            this.playGroupBox.TabStop = false;
            this.playGroupBox.Text = "Test &Movie";
            // 
            // testMovieCombo
            //
            this.testMovieCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.testMovieCombo.Location = new System.Drawing.Point(11, 17);
            this.testMovieCombo.Name = "testMovieCombo";
            this.testMovieCombo.Size = new System.Drawing.Size(219, 21);
            this.testMovieCombo.TabIndex = 12;
            this.testMovieCombo.SelectedIndexChanged += new System.EventHandler(this.testMovieCombo_SelectedIndexChanged);
            // 
            // editCommandButton
            // 
            this.editCommandButton.Location = new System.Drawing.Point(234, 15);
            this.editCommandButton.Name = "editCommandButton";
            this.editCommandButton.Size = new System.Drawing.Size(75, 21);
            this.editCommandButton.TabIndex = 2;
            this.editCommandButton.Text = "&Edit...";
            this.editCommandButton.Visible = false;
            this.editCommandButton.Click += new System.EventHandler(this.editCommandButton_Click);
            // 
            // classpathsTab
            // 
            this.classpathsTab.Controls.Add(this.groupBox3);
            this.classpathsTab.Controls.Add(this.label3);
            this.classpathsTab.Controls.Add(this.btnGlobalClasspaths);
            this.classpathsTab.Location = new System.Drawing.Point(4, 22);
            this.classpathsTab.Name = "classpathsTab";
            this.classpathsTab.Size = new System.Drawing.Size(334, 272);
            this.classpathsTab.TabIndex = 3;
            this.classpathsTab.Text = "Classpaths";
            this.classpathsTab.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Location = new System.Drawing.Point(8, 3);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(319, 192);
            this.groupBox3.TabIndex = 0;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "&Project Classpaths";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label2.Location = new System.Drawing.Point(16, 166);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(288, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Project classpaths are relative to the project location";
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label3.Location = new System.Drawing.Point(14, 204);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(313, 31);
            this.label3.TabIndex = 1;
            this.label3.Text = "Global classpaths are specific to your machine\r\nand are not stored in the project file.";
            // 
            // btnGlobalClasspaths
            // 
            this.btnGlobalClasspaths.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnGlobalClasspaths.Location = new System.Drawing.Point(15, 237);
            this.btnGlobalClasspaths.Name = "btnGlobalClasspaths";
            this.btnGlobalClasspaths.Size = new System.Drawing.Size(150, 21);
            this.btnGlobalClasspaths.TabIndex = 2;
            this.btnGlobalClasspaths.Text = "&Edit Global Classpaths...";
            this.btnGlobalClasspaths.Click += new System.EventHandler(this.btnGlobalClasspaths_Click);
            // 
            // buildTab
            // 
            this.buildTab.Controls.Add(this.groupBox5);
            this.buildTab.Controls.Add(this.groupBox4);
            this.buildTab.Location = new System.Drawing.Point(4, 22);
            this.buildTab.Name = "buildTab";
            this.buildTab.Size = new System.Drawing.Size(334, 272);
            this.buildTab.TabIndex = 4;
            this.buildTab.Text = "Build";
            this.buildTab.UseVisualStyleBackColor = true;
            // 
            // groupBox5
            //
            this.groupBox5.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox5.Controls.Add(this.alwaysExecuteCheckBox);
            this.groupBox5.Controls.Add(this.postBuilderButton);
            this.groupBox5.Controls.Add(this.postBuildBox);
            this.groupBox5.Location = new System.Drawing.Point(8, 145);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(319, 118);
            this.groupBox5.TabIndex = 1;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Post-Build Command Line";
            // 
            // alwaysExecuteCheckBox
            //
            this.alwaysExecuteCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.alwaysExecuteCheckBox.Location = new System.Drawing.Point(13, 84);
            this.alwaysExecuteCheckBox.Name = "alwaysExecuteCheckBox";
            this.alwaysExecuteCheckBox.Size = new System.Drawing.Size(144, 17);
            this.alwaysExecuteCheckBox.TabIndex = 2;
            this.alwaysExecuteCheckBox.Text = "Always execute";
            this.agressiveTip.SetToolTip(this.alwaysExecuteCheckBox, "Execute the Post-Build Command Line even after a failed build");
            this.alwaysExecuteCheckBox.CheckedChanged += new System.EventHandler(this.alwaysExecuteCheckBox_CheckedChanged);
            // 
            // postBuilderButton
            // 
            this.postBuilderButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.postBuilderButton.Location = new System.Drawing.Point(232, 81);
            this.postBuilderButton.Name = "postBuilderButton";
            this.postBuilderButton.Size = new System.Drawing.Size(75, 21);
            this.postBuilderButton.TabIndex = 1;
            this.postBuilderButton.Text = "Builder...";
            this.postBuilderButton.Click += new System.EventHandler(this.postBuilderButton_Click);
            // 
            // postBuildBox
            // 
            this.postBuildBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.postBuildBox.Location = new System.Drawing.Point(13, 21);
            this.postBuildBox.Multiline = true;
            this.postBuildBox.Name = "postBuildBox";
            this.postBuildBox.Size = new System.Drawing.Size(293, 55);
            this.postBuildBox.TabIndex = 0;
            this.postBuildBox.TextChanged += new System.EventHandler(this.postBuildBox_TextChanged);
            // 
            // groupBox4
            // 
            this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox4.Controls.Add(this.preBuilderButton);
            this.groupBox4.Controls.Add(this.preBuildBox);
            this.groupBox4.Location = new System.Drawing.Point(8, 3);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(319, 140);
            this.groupBox4.TabIndex = 0;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Pre-Build Command Line";
            // 
            // preBuilderButton
            // 
            this.preBuilderButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.preBuilderButton.Location = new System.Drawing.Point(232, 103);
            this.preBuilderButton.Name = "preBuilderButton";
            this.preBuilderButton.Size = new System.Drawing.Size(75, 21);
            this.preBuilderButton.TabIndex = 1;
            this.preBuilderButton.Text = "Builder...";
            this.preBuilderButton.Click += new System.EventHandler(this.preBuilderButton_Click);
            // 
            // preBuildBox
            // 
            this.preBuildBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.preBuildBox.Location = new System.Drawing.Point(13, 21);
            this.preBuildBox.Multiline = true;
            this.preBuildBox.Name = "preBuildBox";
            this.preBuildBox.Size = new System.Drawing.Size(293, 77);
            this.preBuildBox.TabIndex = 0;
            this.preBuildBox.TextChanged += new System.EventHandler(this.preBuildBox_TextChanged);
            // 
            // compilerTab
            // 
            this.compilerTab.Controls.Add(this.propertyGrid);
            this.compilerTab.Location = new System.Drawing.Point(4, 22);
            this.compilerTab.Name = "compilerTab";
            this.compilerTab.Padding = new System.Windows.Forms.Padding(3);
            this.compilerTab.Size = new System.Drawing.Size(334, 272);
            this.compilerTab.TabIndex = 1;
            this.compilerTab.Text = "Compiler Options";
            this.compilerTab.UseVisualStyleBackColor = true;
            // 
            // propertyGrid
            // 
            this.propertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid.LineColor = System.Drawing.SystemColors.ScrollBar;
            this.propertyGrid.Location = new System.Drawing.Point(3, 3);
            this.propertyGrid.Name = "propertyGrid";
            this.propertyGrid.Size = new System.Drawing.Size(328, 266);
            this.propertyGrid.TabIndex = 0;
            this.propertyGrid.ToolbarVisible = false;
            this.propertyGrid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGrid_PropertyValueChanged);
            // 
            // sdkTabPage
            // 
            this.sdkTabPage.Controls.Add(this.sdkGroupBox);
            this.sdkTabPage.Controls.Add(this.customGroupBox);
            this.sdkTabPage.Location = new System.Drawing.Point(4, 22);
            this.sdkTabPage.Name = "sdkTabPage";
            this.sdkTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.sdkTabPage.Size = new System.Drawing.Size(334, 272);
            this.sdkTabPage.TabIndex = 5;
            this.sdkTabPage.Text = "SDK";
            this.sdkTabPage.UseVisualStyleBackColor = true;
            // 
            // manageButton
            // 
            this.manageButton.Location = new System.Drawing.Point(224, 37);
            this.manageButton.Name = "manageButton";
            this.manageButton.Size = new System.Drawing.Size(85, 21);
            this.manageButton.TabIndex = 2;
            this.manageButton.Text = "Manage...";
            this.manageButton.UseVisualStyleBackColor = true;
            this.manageButton.Click += new EventHandler(this.manageButton_Click);
            // 
            // sdkComboBox
            // 
            this.sdkComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.sdkComboBox.FormattingEnabled = true;
            this.sdkComboBox.Location = new System.Drawing.Point(11, 39);
            this.sdkComboBox.Name = "sdkComboBox";
            this.sdkComboBox.Size = new System.Drawing.Size(204, 21);
            this.sdkComboBox.TabIndex = 1;
            this.sdkComboBox.SelectedIndexChanged += new EventHandler(this.sdkCombo_SelectedIndexChanged);
            // 
            // sdkGroupBox
            // 
            this.sdkGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.sdkGroupBox.Controls.Add(this.labelUseGlobal);
            this.sdkGroupBox.Controls.Add(this.labelWarning);
            this.sdkGroupBox.Controls.Add(this.warningImage);
            this.sdkGroupBox.Controls.Add(this.sdkComboBox);
            this.sdkGroupBox.Controls.Add(this.manageButton);
            this.sdkGroupBox.Location = new System.Drawing.Point(8, 3);
            this.sdkGroupBox.Name = "sdkGroupBox";
            this.sdkGroupBox.Size = new System.Drawing.Size(319, 92);
            this.sdkGroupBox.TabIndex = 1;
            this.sdkGroupBox.TabStop = false;
            this.sdkGroupBox.Text = "Installed SDKs";
            // 
            // labelUseGlobal
            // 
            this.labelUseGlobal.AutoSize = true;
            this.labelUseGlobal.Location = new System.Drawing.Point(8, 18);
            this.labelUseGlobal.Name = "labelUseGlobal";
            this.labelUseGlobal.Size = new System.Drawing.Size(154, 13);
            this.labelUseGlobal.TabIndex = 3;
            this.labelUseGlobal.Text = "Use a SDK configured globally.";
            // 
            // warningImage
            // 
            this.warningImage.Location = new System.Drawing.Point(12, 67);
            this.warningImage.Name = "warningImage";
            this.warningImage.Size = new System.Drawing.Size(16, 16);
            this.warningImage.TabIndex = 4;
            this.warningImage.TabStop = false;
            this.warningImage.Visible = false;
            // 
            // labelWarning
            // 
            this.labelWarning.AutoSize = true;
            this.labelWarning.Location = new System.Drawing.Point(30, 68);
            this.labelWarning.Name = "labelWarning";
            this.labelWarning.Size = new System.Drawing.Size(242, 13);
            this.labelWarning.TabIndex = 5;
            this.labelWarning.Text = "";
            // 
            // customGroupBox
            // 
            this.customGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.customGroupBox.Controls.Add(this.browseButton);
            this.customGroupBox.Controls.Add(this.customTextBox);
            this.customGroupBox.Controls.Add(this.labelUseCustom);
            this.customGroupBox.Location = new System.Drawing.Point(8, 102);
            this.customGroupBox.Name = "customGroupBox";
            this.customGroupBox.Size = new System.Drawing.Size(319, 74);
            this.customGroupBox.TabIndex = 2;
            this.customGroupBox.TabStop = false;
            this.customGroupBox.Text = "Custom SDK";
            // 
            // browseButton
            // 
            this.browseButton.Location = new System.Drawing.Point(224, 37);
            this.browseButton.Name = "browseButton";
            this.browseButton.Size = new System.Drawing.Size(85, 21);
            this.browseButton.TabIndex = 6;
            this.browseButton.Text = "Browse...";
            this.browseButton.UseVisualStyleBackColor = true;
            this.browseButton.Click += new EventHandler(this.browseButton_Click);
            // 
            // customTextBox
            // 
            this.customTextBox.Location = new System.Drawing.Point(11, 38);
            this.customTextBox.Name = "customTextBox";
            this.customTextBox.Size = new System.Drawing.Size(204, 21);
            this.customTextBox.TabIndex = 5;
            this.customTextBox.TextChanged += new EventHandler(this.customTextBox_TextChanged);
            // 
            // labelUseCustom
            // 
            this.labelUseCustom.AutoSize = true;
            this.labelUseCustom.Location = new System.Drawing.Point(8, 18);
            this.labelUseCustom.Name = "labelUseCustom";
            this.labelUseCustom.Size = new System.Drawing.Size(213, 13);
            this.labelUseCustom.TabIndex = 4;
            this.labelUseCustom.Text = "Relative or absolute path to a specific SDK.";
            // 
            // agressiveTip
            // 
            this.agressiveTip.AutomaticDelay = 0;
            // 
            // PropertiesDialog
            // 
            this.AcceptButton = this.btnOK;
            this.CancelButton = this.btnCancel;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(366, 348);
            this.MinimumSize = new System.Drawing.Size(377, 377);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PropertiesDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Project Properties";
            this.tabControl.ResumeLayout(false);
            this.movieTab.ResumeLayout(false);
            this.movieTab.PerformLayout();
            this.platformGroupBox.ResumeLayout(false);
            this.generalGroupBox.ResumeLayout(false);
            this.generalGroupBox.PerformLayout();
            this.playGroupBox.ResumeLayout(false);
            this.classpathsTab.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.buildTab.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.compilerTab.ResumeLayout(false);
            this.sdkGroupBox.ResumeLayout(false);
            this.sdkGroupBox.PerformLayout();
            this.customGroupBox.ResumeLayout(false);
            this.customGroupBox.PerformLayout();
            this.sdkTabPage.ResumeLayout(false);
            this.sdkTabPage.PerformLayout();
            this.ResumeLayout(false);
		}

		#endregion

		#endregion

		private Project project;
        private CompilerOptions optionsCopy;
        private Boolean propertiesChanged;
        private Boolean platformChanged;
        private Boolean classpathsChanged;
        private Boolean assetsChanged;
        private Boolean sdkChanged;
        private Boolean isPropertyGridReadOnly;
        private LanguagePlatform langPlatform;

		public event EventHandler OpenGlobalClasspaths;

        public PropertiesDialog() 
        {
            this.Font = PluginBase.Settings.DefaultFont;
            this.FormGuid = "4216fccc-781b-4b06-89f6-d9f7e77c2e5f";
            this.InitializeComponent();
            this.CreateClassPathControl();
            this.InitializeLocalization();
        }

        private void CreateClassPathControl()
        {
            this.classpathControl = new ProjectManager.Controls.ClasspathControl();
            this.classpathControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.classpathControl.Classpaths = new string[0];
            this.classpathControl.Language = null;
            this.classpathControl.Location = new System.Drawing.Point(17, 22);
            this.classpathControl.Name = "classpathControl";
            this.classpathControl.Project = null;
            this.classpathControl.Size = new System.Drawing.Size(287, 134);
            this.classpathControl.TabIndex = 0;
            this.groupBox3.Controls.Add(this.classpathControl);
        }

        protected Project BaseProject { get { return project; } }

        public void SetProject(Project project)
        {
            this.project = project;
            BuildDisplay();
        }

        #region Change Tracking

        public bool PropertiesChanged
        {
            get { return propertiesChanged; }
            protected set { propertiesChanged = value; }
        }
        
        public bool ClasspathsChanged
        {
            get { return classpathsChanged; }
            protected set { classpathsChanged = value; }
        }

		public bool AssetsChanged
        {
            get { return assetsChanged; }
            protected set { assetsChanged = value; }
        }

        public bool PlatformChanged
        {
            get { return platformChanged; }
            protected set { platformChanged = value; }
        }

        #endregion

        #region Localization

        private void InitializeLocalization()
        {
            this.btnOK.Text = TextHelper.GetString("Label.OK");
            this.buildTab.Text = TextHelper.GetString("Info.Build");
            this.movieTab.Text = TextHelper.GetString("Info.Output");
            this.btnApply.Text = TextHelper.GetString("Label.Apply");
            this.btnCancel.Text = TextHelper.GetString("Label.Cancel");
            this.preBuilderButton.Text = TextHelper.GetString("Info.Builder");
            this.postBuilderButton.Text = TextHelper.GetString("Info.Builder");
            this.label2.Text = TextHelper.GetString("Info.ProjectClasspaths");
            this.outputTypeLabel.Text = TextHelper.GetString("Info.OutputType");
            this.outputBrowseButton.Text = TextHelper.GetString("Label.Browse");
            this.groupBox3.Text = TextHelper.GetString("Label.ProjectClasspaths").Replace("...", "");
            this.groupBox5.Text = TextHelper.GetString("Info.PostBuildCmdLine");
            this.dimensionsLabel.Text = TextHelper.GetString("Label.Dimensions");
            this.label3.Text = String.Format(TextHelper.GetString("Info.GlobalClasspaths"), "\n");
            this.agressiveTip.SetToolTip(this.alwaysExecuteCheckBox, TextHelper.GetString("ToolTip.AlwaysExecute"));
            this.btnGlobalClasspaths.Text = TextHelper.GetString("Label.EditGlobalClasspaths");
            this.alwaysExecuteCheckBox.Text = TextHelper.GetString("Info.AlwaysExecute");
            this.bgcolorLabel.Text = TextHelper.GetString("Label.BackgroundColor");
            this.framerateLabel.Text = TextHelper.GetString("Label.FrameRate");
            this.compilerTab.Text = TextHelper.GetString("Info.CompilerOptions");
            this.platformGroupBox.Text = TextHelper.GetString("Info.Platform");
            this.groupBox4.Text = TextHelper.GetString("Info.PreBuildCmdLine");
            this.exportinLabel.Text = TextHelper.GetString("Label.OutputFile");
            this.classpathsTab.Text = TextHelper.GetString("Info.Classpaths");
            this.Text = " " + TextHelper.GetString("Title.ProjectProperties");
            this.generalGroupBox.Text = TextHelper.GetString("Info.General");
            this.playGroupBox.Text = TextHelper.GetString("Label.TestMovie");
            this.editCommandButton.Text = TextHelper.GetString("Info.EditCommand");
            this.manageButton.Text = TextHelper.GetString("Label.Manage");
            this.sdkGroupBox.Text = TextHelper.GetString("Label.SDKGroup");
            this.sdkTabPage.Text = TextHelper.GetString("Label.SDKTab");
            this.customGroupBox.Text = TextHelper.GetString("Label.SDKTabCustom");
            this.browseButton.Text = TextHelper.GetString("Label.SDKBrowse");
            this.labelUseCustom.Text = TextHelper.GetString("Label.SDKUseCustom");
            this.labelUseGlobal.Text = TextHelper.GetString("Label.SDKUseGlobal");
        }

        #endregion

        protected virtual void BuildDisplay()
		{
            this.Text = " " + project.Name + " (" + project.Language.ToUpper() + ") " + TextHelper.GetString("Info.Properties");

            langPlatform = GetLanguagePlatform(project.MovieOptions.Platform);

            InitOutputTab();
            InitBuildTab();
            InitOptionsTab();
            if (project.IsCompilable)
            {
                InitSDKTab();
                InitClasspathTab();
            }
            else
            {
                tabControl.TabPages.Remove(sdkTabPage);
                tabControl.TabPages.Remove(classpathsTab);
            }

			btnApply.Enabled = false;
		}

        private void InitOptionsTab()
        {
            UpdateCompilerOptions();
        }

        private void UpdateCompilerOptions()
        {
            var readOnly = IsExternalConfiguration();
            if (readOnly == isPropertyGridReadOnly && propertyGrid.SelectedObject != null) 
                return;
            isPropertyGridReadOnly = readOnly;

            // clone the compiler options object because the PropertyGrid modifies its
            // object directly
            optionsCopy = project.CompilerOptions.Clone();

            if (isPropertyGridReadOnly)
                TypeDescriptor.AddAttributes(optionsCopy, new Attribute[] { new ReadOnlyAttribute(true) });

            propertyGrid.SelectedObject = optionsCopy;
            propertiesChanged = false;
        }

        private void InitBuildTab()
        {
            preBuildBox.Text = project.PreBuildEvent;
            postBuildBox.Text = project.PostBuildEvent;
            alwaysExecuteCheckBox.Checked = project.AlwaysRunPostBuild;
        }

        private void InitClasspathTab()
        {
            classpathControl.Changed += new EventHandler(classpathControl_Changed);
            classpathControl.Project = project;
            classpathControl.Classpaths = project.Classpaths.ToArray();
            classpathControl.Language = project.Language;
            classpathControl.LanguageBox.Visible = false;
            UpdateClasspaths();
            classpathsChanged = false;
        }

        private void UpdateClasspaths()
        {
            if (IsExternalConfiguration())
            {
                classpathControl.Enabled = false;
                label2.Text = String.Format(TextHelper.GetString("Info.ProjectClasspathsDisabled"), langPlatform.Name);
                return;
            }

            classpathControl.Enabled = true;
            label2.Text = TextHelper.GetString("Info.ProjectClasspaths");
        }

        private void InitSDKTab()
        {
            sdkComboBox.Items.Clear();
            int select = 0;
            BuildActions.LatestSDKMatchQuality = -1;
            warningImage.Visible = false;
            labelWarning.Text = "";

            // retrieve SDK list
            InstalledSDK[] sdks = BuildActions.GetInstalledSDKs(project);
            if (sdks != null && sdks.Length > 0)
            {
                sdkComboBox.Items.Add(TextHelper.GetString("Label.SDKComboDefault") + " (" + sdks[0].Name + ")");
                sdkComboBox.Items.AddRange(sdks);
            }
            else sdkComboBox.Items.Add(TextHelper.GetString("Label.SDKComboNoSDK"));

            InstalledSDK sdk = BuildActions.MatchSDK(sdks, project);
            if (sdk != InstalledSDK.INVALID_SDK)
            {
                if (BuildActions.LatestSDKMatchQuality >= 0)
                    select = 1 + Array.IndexOf(sdks, sdk);

                if (BuildActions.LatestSDKMatchQuality > 0)
                {
                    string icon = BuildActions.LatestSDKMatchQuality < 10 ? "196" : "197";
                    warningImage.Image = PluginBase.MainForm.FindImage(icon);
                    warningImage.Visible = true;
                    string[] p = (project.PreferredSDK + ";;").Split(';');
                    labelWarning.Text = TextHelper.GetString("Label.SDKExpected") 
                        + " " + p[0] + " (" + p[1] + ")";
                }
            }

            sdkComboBox.SelectedIndex = select;
            if (sdk != InstalledSDK.INVALID_SDK && (sdks == null || Array.IndexOf(sdks, sdk) < 0)) 
                customTextBox.Text = sdk.Path;
            sdkChanged = false;
        }

        private void InitOutputTab()
        {
            MovieOptions options = project.MovieOptions;

            string[] types = Array.ConvertAll<OutputType, string>(
                    project.MovieOptions.OutputTypes, 
                    (ot) => ot.ToString()
                );
            InitCombo(outputCombo, types, project.OutputType, "Label.OutputType");
            outputCombo.SelectedIndexChanged += new EventHandler(outputCombo_SelectedIndexChanged);

            outputSwfBox.Text = project.OutputPath;
            widthTextBox.Text = options.Width.ToString();
            heightTextBox.Text = options.Height.ToString();
            colorTextBox.Text = options.Background;
            fpsTextBox.Text = options.Fps.ToString();

            InitCombo(platformCombo, project.MovieOptions.TargetPlatforms, project.MovieOptions.Platform);
            platformCombo.SelectedIndexChanged += new EventHandler(platformCombo_SelectedIndexChanged);

            InitCombo(versionCombo, project.MovieOptions.TargetVersions(this.platformCombo.Text), project.MovieOptions.Version);
            versionCombo.SelectedIndexChanged += new EventHandler(versionCombo_SelectedIndexChanged);
            UpdateVersionCombo();

            InitTestMovieOptions();
            UpdateGeneralPanel();
            UpdateEditCommandButton();
        }

        private void UpdateEditCommandButton()
        {
            TestMovieBehavior state = GetTestMovie();
            editCommandButton.Visible = state == TestMovieBehavior.Custom 
                || state == TestMovieBehavior.OpenDocument
                || state == TestMovieBehavior.Webserver;
        }

        private void UpdateVersionCombo()
        {
            if (versionCombo.Items.Count > 1)
            {
                versionCombo.Enabled = true;
            }
            else
            {
                versionCombo.Enabled = false;
                versionCombo.SelectedIndex = -1;
            }
        }

        private void InitTestMovieOptions()
        {
            OutputType output = GetOutput();
            string platform = GetPlatform();

            List<TestMovieBehavior> options = new List<TestMovieBehavior>();
            if (platform == PlatformData.FLASHPLAYER_PLATFORM)
            {
                options.Add(TestMovieBehavior.Default);
                options.Add(TestMovieBehavior.NewTab);
                options.Add(TestMovieBehavior.NewWindow);
                options.Add(TestMovieBehavior.ExternalPlayer);
            }
            options.Add(TestMovieBehavior.OpenDocument);
            options.Add(TestMovieBehavior.Webserver);
            options.Add(TestMovieBehavior.Custom);
            List<string> items = options.ConvertAll<string>((item) => item.ToString());

            InitCombo(testMovieCombo, items.ToArray(), project.TestMovieBehavior.ToString(), "Label.TestMovie");
        }

        private void InitCombo(ComboBox combo, object[] items, object select)
        {
            InitCombo(combo, items, select, null);
        }

        private void InitCombo(ComboBox combo, object[] values, object select, string localizePrefix)
        {
            combo.Items.Clear();
            ComboItem[] items = Array.ConvertAll<object, ComboItem>(values, (value) => new ComboItem(value, localizePrefix));
            combo.Items.AddRange(items);

            if (select != null)
            {
                string value = select.ToString();
                int index = Array.FindIndex<ComboItem>(items, (item) => item.Value.ToString() == value);
                if (index >= 0) combo.SelectedIndex = index;
            }
        }

        private string GetPlatform()
        {
            if (platformCombo.SelectedIndex < 0) return null;
            else return (platformCombo.SelectedItem as ComboItem).Value.ToString();
        }

        private OutputType GetOutput()
        {
            if (outputCombo.SelectedIndex < 0) return OutputType.Unknown;
            else return (OutputType)Enum.Parse(typeof(OutputType), (outputCombo.SelectedItem as ComboItem).Value.ToString());
        }

        private TestMovieBehavior GetTestMovie()
        {
            if (testMovieCombo.SelectedIndex < 0) return TestMovieBehavior.Unknown;
            else return (TestMovieBehavior)Enum.Parse(typeof(TestMovieBehavior), (testMovieCombo.SelectedItem as ComboItem).Value.ToString());
        }

		protected void Modified()
		{
			btnApply.Enabled = true;
		}

		protected virtual bool Apply()
		{
			MovieOptions options = project.MovieOptions;

			try
			{
                project.OutputType = GetOutput();
                if (OuputValid(outputSwfBox.Text)) project.OutputPath = outputSwfBox.Text;
				project.Classpaths.Clear();
				project.Classpaths.AddRange(classpathControl.Classpaths);
				options.Width = int.Parse(widthTextBox.Text);
				options.Height = int.Parse(heightTextBox.Text);
				options.BackgroundColor = Color.FromArgb(255, colorLabel.BackColor);
				options.Fps = int.Parse(fpsTextBox.Text);
                options.Platform = GetPlatform();
                options.Version = versionCombo.Text;
				project.PreBuildEvent = preBuildBox.Text;
				project.PostBuildEvent = postBuildBox.Text;
				project.AlwaysRunPostBuild = alwaysExecuteCheckBox.Checked;
                project.TestMovieBehavior = GetTestMovie();

                if (sdkChanged)
                {
                    if (customTextBox.Text.Length > 0) project.PreferredSDK = customTextBox.Text;
                    else
                    {
                        InstalledSDK sdk = sdkComboBox.SelectedItem as InstalledSDK;
                        project.PreferredSDK = sdk != null ? sdk.ToPreferredSDK() : null;
                    }
                }
			}
			catch (Exception exception)
			{
                ErrorManager.ShowError(exception);
				return false;
			}
            // copy compiler option values
            project.CompilerOptions = optionsCopy;
			btnApply.Enabled = false;
			propertiesChanged = true;
			return true;
		}

        private bool OuputValid(string path)
        {
            try
            {
                if (path != "") new FileInfo(path);
                return true;
            }
            catch(Exception ex) {
                ErrorManager.ShowInfo(ex.Message);
                return false;
            }
        }

		private void btnOK_Click(object sender, EventArgs e)
		{
			if (btnApply.Enabled) if (!Apply()) return;
			this.Close();
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			this.Close();
		}

		private void btnApply_Click(object sender, EventArgs e)
		{
			Apply();
		}

		private void outputSwfBox_TextChanged(object sender, EventArgs e)
		{
			classpathsChanged = true;
			Modified();
		}

		private void widthTextBox_TextChanged(object sender, EventArgs e) { Modified(); }

		private void heightTextBox_TextChanged(object sender, EventArgs e) { Modified(); }

		private void colorTextBox_TextChanged(object sender, EventArgs e) 
        {
            string rgb = colorTextBox.Text;
            if (rgb.Length == 0) rgb = "#000000";
            if (rgb[0] != '#') rgb = '#' + rgb;
            if (rgb.Length > 7) rgb = rgb.Substring(0, 7);
            else while (rgb.Length < 7) rgb = "#0" + rgb.Substring(1);
            try
            {
                colorLabel.BackColor = ColorTranslator.FromHtml(rgb);
            }
            catch { colorLabel.BackColor = Color.Black; }
            Modified(); 
        }
		private void fpsTextBox_TextChanged(object sender, EventArgs e) { Modified(); }

		private void preBuildBox_TextChanged(object sender, System.EventArgs e) { Modified(); }

		private void postBuildBox_TextChanged(object sender, System.EventArgs e) { Modified(); }

		private void alwaysExecuteCheckBox_CheckedChanged(object sender, System.EventArgs e) { Modified(); }

		private void testMovieCombo_SelectedIndexChanged(object sender, System.EventArgs e) 
        { 
            Modified();
            editCommandButton.Visible = testMovieCombo.Text.IndexOf("..") > 0;
        }

		private void propertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
		{ 
            Modified();
        }

        void platformCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            langPlatform = GetLanguagePlatform(platformCombo.Text);

            this.versionCombo.Items.Clear();
            this.versionCombo.Items.AddRange(project.MovieOptions.TargetVersions(this.platformCombo.Text));
            this.versionCombo.SelectedIndex = Math.Max(0, this.versionCombo.Items.IndexOf(
                        project.MovieOptions.DefaultVersion(this.platformCombo.Text)));

            UpdateVersionCombo();
            InitTestMovieOptions();
            UpdateGeneralPanel();
            UpdateEditCommandButton();
            DetectExternalToolchain();
            UpdateClasspaths();
            UpdateCompilerOptions();

            platformChanged = true;
            Modified();
        }

        private void SelectItem(ComboBox combo, object value)
        {
            foreach (var item in combo.Items)
                if (item.ToString() == value.ToString())
                {
                    combo.SelectedItem = item;
                    break;
                }
        }

		private void versionCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            platformChanged = true;
            Modified();
        }

        void classpathControl_Changed(object sender, EventArgs e)
        {
            classpathsChanged = true; // keep special track of this, it's a big deal
            Modified();
        }

		private void colorLabel_Click(object sender, EventArgs e)
		{
			if (this.colorDialog.ShowDialog() == DialogResult.OK)
			{
				this.colorLabel.BackColor = this.colorDialog.Color;
				this.colorTextBox.Text = this.ToHtml(this.colorLabel.BackColor);
				Modified();
			}
		}

		private string ToHtml(Color c)
		{
			return string.Format("#{0:X6}", (c.R << 16) + (c.G << 8) + c.B);
		}

		private void btnGlobalClasspaths_Click(object sender, EventArgs e)
		{
			if (OpenGlobalClasspaths != null) OpenGlobalClasspaths(this,new EventArgs());
		}

		private void outputBrowseButton_Click(object sender, EventArgs e)
		{
			SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "*.*|*.*"; // TextHelper.GetString("Info.FlashMovieFilter");
			dialog.OverwritePrompt = false;
			dialog.InitialDirectory = project.Directory;
			// try pre-setting the current output path
			try
			{
				string path = project.GetAbsolutePath(outputSwfBox.Text);
				if (File.Exists(path)) dialog.FileName = path;
			}
			catch { }
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                outputSwfBox.Text = project.GetRelativePath(dialog.FileName);
            }
		}

        private void editCommandButton_Click(object sender, System.EventArgs e)
        {
            string caption;
            string label;
            if (testMovieCombo.SelectedIndex == 4)
            {
                caption = TextHelper.GetString("Title.CustomTestMovieDocument");
                label = TextHelper.GetString("Label.CustomTestMovieDocument");
            }
            else
            {
                caption = TextHelper.GetString("Title.CustomTestMovieCommand");
                label = TextHelper.GetString("Label.CustomTestMovieCommand");
            }
            LineEntryDialog dialog = new LineEntryDialog(caption, label, project.TestMovieCommand ?? "");
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                project.TestMovieCommand = dialog.Line;
                Modified();
                btnOK.Focus();
            }
        }

		private void preBuilderButton_Click(object sender, System.EventArgs e)
		{
			using (BuildEventDialog dialog = new BuildEventDialog(project))
			{
				dialog.CommandLine = preBuildBox.Text;
				if (dialog.ShowDialog(this) == DialogResult.OK) preBuildBox.Text = dialog.CommandLine;
			}
		}

		private void postBuilderButton_Click(object sender, System.EventArgs e)
		{
			using (BuildEventDialog dialog = new BuildEventDialog(project))
			{
				dialog.CommandLine = postBuildBox.Text;
				if (dialog.ShowDialog(this) == DialogResult.OK) postBuildBox.Text = dialog.CommandLine;
			}
		}

        private void outputCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateGeneralPanel();
            InitTestMovieOptions();
            Modified();
        }

        private void UpdateGeneralPanel()
        {
            OutputType output = GetOutput();
            generalGroupBox.Enabled = project.MovieOptions.HasOutput(output);
            testMovieCombo.Enabled = (output != OutputType.Library);
            
            bool isGraphical = project.MovieOptions.IsGraphical(GetPlatform());
            widthTextBox.Enabled = heightTextBox.Enabled = fpsTextBox.Enabled
                = colorTextBox.Enabled = colorLabel.Enabled = isGraphical;

            if (IsExternalToolchain())
                exportinLabel.Text = TextHelper.GetString("Label.ConfigurationFile");
            else
                exportinLabel.Text = TextHelper.GetString("Label.OutputFile");
        }

        private void manageButton_Click(object sender, EventArgs e)
        {
            DataEvent de = new DataEvent(EventType.Command, "ASCompletion.ShowSettings", project.Language);
            EventManager.DispatchEvent(this, de);
            if (de.Handled) InitSDKTab();
        }

        void customTextBox_TextChanged(object sender, EventArgs e)
        {
            sdkChanged = true;
            Modified();
        }

        private void browseButton_Click(object sender, EventArgs e)
        {
            VistaFolderBrowserDialog folder = new VistaFolderBrowserDialog();
            if (customTextBox.Text.Length > 0 && Directory.Exists(customTextBox.Text))
                folder.SelectedPath = customTextBox.Text;
            if (folder.ShowDialog() == DialogResult.OK)
            {
                customTextBox.Text = folder.SelectedPath;
            }
        }

        private void sdkCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            customTextBox.Text = "";
            sdkChanged = true;
            Modified();
        }

        /* PLATFORM CONFIGURATION */

        private void DetectExternalToolchain()
        {
            if (!IsExternalToolchain()) return;

            SelectItem(outputCombo, OutputType.Application);
            SelectItem(testMovieCombo, TestMovieBehavior.Custom);
            project.TestMovieCommand = "";

            if (langPlatform.DefaultProjectFile == null) return;

            foreach (string fileName in langPlatform.DefaultProjectFile)
                if (File.Exists(project.GetAbsolutePath(fileName)))
                {
                    outputSwfBox.Text = fileName;
                    break;
                }
        }

        private bool IsExternalConfiguration()
        {
            string selectedVersion = versionCombo.Text == "" ? "1.0" : versionCombo.Text;
            PlatformVersion version = langPlatform.GetVersion(selectedVersion);
            return version != null && version.Commands != null && version.Commands.ContainsKey("display");
        }

        private bool IsExternalToolchain()
        {
            return langPlatform != null && langPlatform.ExternalToolchain != null;
        }

        private LanguagePlatform GetLanguagePlatform(string platformName)
        {
            if (PlatformData.SupportedLanguages.ContainsKey(project.Language))
            {
                SupportedLanguage lang = PlatformData.SupportedLanguages[project.Language];
                if (lang.Platforms.ContainsKey(platformName))
                    return lang.Platforms[platformName];
            }
            return null;
        }

	}

    class ComboItem
    {
        public string Label;
        public object Value;

        public ComboItem(object value, string localizePrefix)
        {
            Value = value;
            Label = localizePrefix != null ? TextHelper.GetString(localizePrefix + value) : value.ToString();
            if (String.IsNullOrEmpty(Label)) Label = value.ToString();
        }

        public override string ToString()
        {
 	         return Label;
        }
    }
}
