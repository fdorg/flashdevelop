// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.IO;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using ProjectManager.Projects;
using ProjectManager.Helpers;
using PluginCore;
using PluginCore.Managers;
using PluginCore.Localization;
using PluginCore.Controls;
using ProjectManager.Actions;
using System.Collections.Generic;
using System.Linq;
using Ookii.Dialogs;

namespace ProjectManager.Controls
{
    public class PropertiesDialog : SmartForm
    {
        #region Form Designer

        Button btnOK;
        Button btnCancel;
        Button btnApply;
        TabPage movieTab;
        TextBox outputSwfBox;
        Label exportinLabel;
        Label pxLabel;
        Label fpsLabel;
        Label bgcolorLabel;
        Label framerateLabel;
        Label dimensionsLabel;
        Label xLabel;
        ColorDialog colorDialog;
        Button outputBrowseButton;
        GroupBox generalGroupBox;
        GroupBox playGroupBox;
        TabPage classpathsTab;
        Label label2;
        Label label3;
        Button btnGlobalClasspaths;
        GroupBox groupBox3;
        TabPage buildTab;
        GroupBox groupBox4;
        TextBox preBuildBox;
        Button preBuilderButton;
        GroupBox groupBox5;
        Button postBuilderButton;
        TextBox postBuildBox;
        ToolTip agressiveTip;
        CheckBox alwaysExecuteCheckBox;
        ComboBox testMovieCombo;
        TabPage compilerTab;
        PropertyGrid propertyGrid;
        Label outputTypeLabel;
        ComboBox outputCombo;
        GroupBox platformGroupBox;
        ComboBox platformCombo;
        Button editCommandButton;
        ComboBox versionCombo;
        TabPage sdkTabPage;
        Button manageButton;
        ComboBox sdkComboBox;
        GroupBox sdkGroupBox;
        GroupBox customGroupBox;
        Label labelUseGlobal;
        PictureBox warningImage;
        Label labelWarning;
        Button browseButton;
        TextBox customTextBox;
        Label labelUseCustom;
        protected TabControl tabControl;
        protected TextBox colorTextBox;
        protected Label colorLabel;
        protected TextBox fpsTextBox;
        protected TextBox heightTextBox;
        protected TextBox widthTextBox;
        ClasspathControl classpathControl;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing) components?.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer Generated Code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        void InitializeComponent()
        {
            components = new Container();
            btnOK = new ButtonEx();
            btnCancel = new ButtonEx();
            btnApply = new ButtonEx();
            tabControl = new TabControlEx();
            movieTab = new TabPage();
            platformGroupBox = new GroupBoxEx();
            versionCombo = new FlatCombo();
            platformCombo = new FlatCombo();
            outputTypeLabel = new Label();
            outputCombo = new FlatCombo();
            generalGroupBox = new GroupBoxEx();
            widthTextBox = new TextBoxEx();
            outputBrowseButton = new ButtonEx();
            heightTextBox = new TextBoxEx();
            xLabel = new Label();
            dimensionsLabel = new Label();
            colorTextBox = new TextBoxEx();
            framerateLabel = new Label();
            outputSwfBox = new TextBoxEx();
            fpsTextBox = new TextBoxEx();
            exportinLabel = new Label();
            colorLabel = new Label();
            pxLabel = new Label();
            bgcolorLabel = new Label();
            fpsLabel = new Label();
            playGroupBox = new GroupBoxEx();
            testMovieCombo = new FlatCombo();
            editCommandButton = new ButtonEx();
            classpathsTab = new TabPage();
            groupBox3 = new GroupBoxEx();
            label2 = new Label();
            label3 = new Label();
            btnGlobalClasspaths = new ButtonEx();
            buildTab = new TabPage();
            groupBox5 = new GroupBoxEx();
            alwaysExecuteCheckBox = new CheckBoxEx();
            postBuilderButton = new ButtonEx();
            postBuildBox = new TextBoxEx();
            groupBox4 = new GroupBoxEx();
            preBuilderButton = new ButtonEx();
            preBuildBox = new TextBoxEx();
            compilerTab = new TabPage();
            propertyGrid = new PropertyGrid();
            colorDialog = new ColorDialog();
            sdkTabPage = new TabPage();
            manageButton = new ButtonEx();
            sdkComboBox = new FlatCombo();
            sdkGroupBox = new GroupBoxEx();
            labelUseGlobal = new Label();
            customGroupBox = new GroupBoxEx();
            browseButton = new ButtonEx();
            customTextBox = new TextBoxEx();
            labelUseCustom = new Label();
            warningImage = new PictureBox();
            labelWarning = new Label();
            agressiveTip = new ToolTip(components);
            sdkTabPage.SuspendLayout();
            sdkGroupBox.SuspendLayout();
            customGroupBox.SuspendLayout();
            tabControl.SuspendLayout();
            movieTab.SuspendLayout();
            platformGroupBox.SuspendLayout();
            generalGroupBox.SuspendLayout();
            playGroupBox.SuspendLayout();
            classpathsTab.SuspendLayout();
            groupBox3.SuspendLayout();
            buildTab.SuspendLayout();
            groupBox5.SuspendLayout();
            groupBox4.SuspendLayout();
            compilerTab.SuspendLayout();
            SuspendLayout();
            // 
            // btnOK
            // 
            btnOK.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnOK.Location = new Point(116, 316);
            btnOK.Name = "btnOK";
            btnOK.Size = new Size(75, 21);
            btnOK.TabIndex = 1;
            btnOK.Text = "&OK";
            btnOK.Click += btnOK_Click;
            // 
            // btnCancel
            // 
            btnCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.Location = new Point(197, 316);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(75, 21);
            btnCancel.TabIndex = 2;
            btnCancel.Text = "&Cancel";
            btnCancel.Click += btnCancel_Click;
            // 
            // btnApply
            // 
            btnApply.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnApply.Enabled = false;
            btnApply.Location = new Point(278, 316);
            btnApply.Name = "btnApply";
            btnApply.Size = new Size(75, 21);
            btnApply.TabIndex = 3;
            btnApply.Text = "&Apply";
            btnApply.Click += btnApply_Click;
            // 
            // tabControl
            // 
            tabControl.Anchor = ((AnchorStyles.Top | AnchorStyles.Bottom) | AnchorStyles.Left) | AnchorStyles.Right;
            tabControl.Controls.Add(movieTab);
            tabControl.Controls.Add(sdkTabPage);
            tabControl.Controls.Add(classpathsTab);
            tabControl.Controls.Add(buildTab);
            tabControl.Controls.Add(compilerTab);
            tabControl.Location = new Point(12, 12);
            tabControl.Name = "tabControl";
            tabControl.SelectedIndex = 0;
            tabControl.Size = new Size(342, 298);
            tabControl.TabIndex = 0;
            // 
            // movieTab
            // 
            movieTab.Controls.Add(platformGroupBox);
            movieTab.Controls.Add(generalGroupBox);
            movieTab.Controls.Add(playGroupBox);
            movieTab.Location = new Point(4, 22);
            movieTab.Name = "movieTab";
            movieTab.Size = new Size(334, 272);
            movieTab.TabIndex = 0;
            movieTab.Text = "Output";
            movieTab.UseVisualStyleBackColor = true;
            // 
            // platformGroupBox
            //
            platformGroupBox.Anchor = ((AnchorStyles.Top) | AnchorStyles.Left) | AnchorStyles.Right;
            platformGroupBox.Controls.Add(versionCombo);
            platformGroupBox.Controls.Add(platformCombo);
            platformGroupBox.Controls.Add(outputTypeLabel);
            platformGroupBox.Controls.Add(outputCombo);
            platformGroupBox.Location = new Point(8, 3);
            platformGroupBox.Name = "platformGroupBox";
            platformGroupBox.Size = new Size(319, 74);
            platformGroupBox.TabIndex = 4;
            platformGroupBox.TabStop = false;
            platformGroupBox.Text = "Platform";
            // 
            // versionCombo
            // 
            versionCombo.DropDownStyle = ComboBoxStyle.DropDownList;
            versionCombo.Location = new Point(234, 19);
            versionCombo.Name = "versionCombo";
            versionCombo.Size = new Size(75, 21);
            versionCombo.TabIndex = 13;
            // 
            // platformCombo
            // 
            platformCombo.DropDownStyle = ComboBoxStyle.DropDownList;
            platformCombo.Location = new Point(11, 19);
            platformCombo.Name = "platformCombo";
            platformCombo.Size = new Size(219, 21);
            platformCombo.TabIndex = 12;
            //
            // outputTypeLabel
            //
            outputTypeLabel.Location = new Point(9, 44);
            outputTypeLabel.Name = "outputTypeLabel";
            outputTypeLabel.Size = new Size(96, 18);
            outputTypeLabel.TabIndex = 13;
            outputTypeLabel.Text = "Compilation &Target:";
            outputTypeLabel.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // outputCombo
            // 
            outputCombo.DropDownStyle = ComboBoxStyle.DropDownList;
            outputCombo.Location = new Point(107, 45);
            outputCombo.Name = "outputCombo";
            outputCombo.Size = new Size(202, 21);
            outputCombo.TabIndex = 14;
            // 
            // generalGroupBox
            // 
            generalGroupBox.Anchor = ((AnchorStyles.Top | AnchorStyles.Bottom) | AnchorStyles.Left) | AnchorStyles.Right;
            generalGroupBox.Controls.Add(widthTextBox);
            generalGroupBox.Controls.Add(outputBrowseButton);
            generalGroupBox.Controls.Add(heightTextBox);
            generalGroupBox.Controls.Add(xLabel);
            generalGroupBox.Controls.Add(dimensionsLabel);
            generalGroupBox.Controls.Add(colorTextBox);
            generalGroupBox.Controls.Add(framerateLabel);
            generalGroupBox.Controls.Add(outputSwfBox);
            generalGroupBox.Controls.Add(fpsTextBox);
            generalGroupBox.Controls.Add(exportinLabel);
            generalGroupBox.Controls.Add(colorLabel);
            generalGroupBox.Controls.Add(pxLabel);
            generalGroupBox.Controls.Add(bgcolorLabel);
            generalGroupBox.Controls.Add(fpsLabel);
            generalGroupBox.Location = new Point(8, 82);
            generalGroupBox.Name = "generalGroupBox";
            generalGroupBox.Size = new Size(319, 129);
            generalGroupBox.TabIndex = 6;
            generalGroupBox.TabStop = false;
            generalGroupBox.Text = "General";
            // 
            // widthTextBox
            // 
            widthTextBox.Location = new Point(108, 44);
            widthTextBox.MaxLength = 4;
            widthTextBox.Name = "widthTextBox";
            widthTextBox.Size = new Size(32, 20);
            widthTextBox.TabIndex = 4;
            widthTextBox.Text = "500";
            widthTextBox.TextChanged += widthTextBox_TextChanged;
            // 
            // outputBrowseButton
            // 
            outputBrowseButton.Location = new Point(233, 15);
            outputBrowseButton.Name = "outputBrowseButton";
            outputBrowseButton.Size = new Size(76, 21);
            outputBrowseButton.TabIndex = 2;
            outputBrowseButton.Text = "&Browse...";
            outputBrowseButton.Click += outputBrowseButton_Click;
            // 
            // heightTextBox
            // 
            heightTextBox.Location = new Point(161, 44);
            heightTextBox.MaxLength = 4;
            heightTextBox.Name = "heightTextBox";
            heightTextBox.Size = new Size(32, 20);
            heightTextBox.TabIndex = 5;
            heightTextBox.Text = "300";
            heightTextBox.TextChanged += heightTextBox_TextChanged;
            // 
            // xLabel
            // 
            xLabel.Location = new Point(145, 44);
            xLabel.Name = "xLabel";
            xLabel.Size = new Size(13, 17);
            xLabel.TabIndex = 21;
            xLabel.Text = "x";
            xLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // dimensionsLabel
            // 
            dimensionsLabel.Location = new Point(8, 46);
            dimensionsLabel.Name = "dimensionsLabel";
            dimensionsLabel.Size = new Size(96, 13);
            dimensionsLabel.TabIndex = 3;
            dimensionsLabel.Text = "&Dimensions:";
            dimensionsLabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // colorTextBox
            // 
            colorTextBox.Location = new Point(139, 71);
            colorTextBox.MaxLength = 7;
            colorTextBox.Name = "colorTextBox";
            colorTextBox.Size = new Size(55, 20);
            colorTextBox.TabIndex = 9;
            colorTextBox.Text = "#FFFFFF";
            colorTextBox.TextChanged += colorTextBox_TextChanged;
            // 
            // framerateLabel
            // 
            framerateLabel.Location = new Point(16, 97);
            framerateLabel.Name = "framerateLabel";
            framerateLabel.Size = new Size(88, 17);
            framerateLabel.TabIndex = 8;
            framerateLabel.Text = "&Framerate:";
            framerateLabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // outputSwfBox
            // 
            outputSwfBox.Location = new Point(108, 17);
            outputSwfBox.Name = "outputSwfBox";
            outputSwfBox.Size = new Size(121, 20);
            outputSwfBox.TabIndex = 1;
            outputSwfBox.TextChanged += outputSwfBox_TextChanged;
            // 
            // fpsTextBox
            // 
            fpsTextBox.Location = new Point(109, 98);
            fpsTextBox.MaxLength = 3;
            fpsTextBox.Name = "fpsTextBox";
            fpsTextBox.Size = new Size(27, 20);
            fpsTextBox.TabIndex = 37;
            fpsTextBox.Text = "30";
            fpsTextBox.TextChanged += fpsTextBox_TextChanged;
            // 
            // exportinLabel
            // 
            exportinLabel.Location = new Point(8, 16);
            exportinLabel.Name = "exportinLabel";
            exportinLabel.Size = new Size(96, 18);
            exportinLabel.TabIndex = 0;
            exportinLabel.Text = "&Output File:";
            exportinLabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // colorLabel
            // 
            colorLabel.BackColor = SystemColors.Window;
            colorLabel.BorderStyle = BorderStyle.FixedSingle;
            colorLabel.Cursor = Cursors.Hand;
            colorLabel.FlatStyle = FlatStyle.System;
            colorLabel.ForeColor = SystemColors.ControlText;
            colorLabel.Location = new Point(109, 72);
            colorLabel.Name = "colorLabel";
            colorLabel.Size = new Size(17, 16);
            colorLabel.TabIndex = 7;
            colorLabel.Click += colorLabel_Click;
            // 
            // pxLabel
            // 
            pxLabel.Location = new Point(199, 47);
            pxLabel.Name = "pxLabel";
            pxLabel.Size = new Size(19, 14);
            pxLabel.TabIndex = 30;
            pxLabel.Text = "px";
            // 
            // bgcolorLabel
            // 
            bgcolorLabel.Location = new Point(5, 70);
            bgcolorLabel.Name = "bgcolorLabel";
            bgcolorLabel.Size = new Size(99, 18);
            bgcolorLabel.TabIndex = 6;
            bgcolorLabel.Text = "Background &color:";
            bgcolorLabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // fpsLabel
            // 
            fpsLabel.Location = new Point(141, 101);
            fpsLabel.Name = "fpsLabel";
            fpsLabel.Size = new Size(32, 17);
            fpsLabel.TabIndex = 28;
            fpsLabel.Text = "fps";
            // 
            // playGroupBox
            // 
            playGroupBox.Anchor = (AnchorStyles.Bottom | AnchorStyles.Left) | AnchorStyles.Right;
            playGroupBox.Controls.Add(testMovieCombo);
            playGroupBox.Controls.Add(editCommandButton);
            playGroupBox.Location = new Point(8, 216);
            playGroupBox.Name = "playGroupBox";
            playGroupBox.Size = new Size(319, 47);
            playGroupBox.TabIndex = 7;
            playGroupBox.TabStop = false;
            playGroupBox.Text = "Test &Movie";
            // 
            // testMovieCombo
            //
            testMovieCombo.DropDownStyle = ComboBoxStyle.DropDownList;
            testMovieCombo.Location = new Point(11, 17);
            testMovieCombo.Name = "testMovieCombo";
            testMovieCombo.Size = new Size(219, 21);
            testMovieCombo.TabIndex = 12;
            testMovieCombo.SelectedIndexChanged += testMovieCombo_SelectedIndexChanged;
            // 
            // editCommandButton
            // 
            editCommandButton.Location = new Point(234, 15);
            editCommandButton.Name = "editCommandButton";
            editCommandButton.Size = new Size(75, 21);
            editCommandButton.TabIndex = 2;
            editCommandButton.Text = "&Edit...";
            editCommandButton.Visible = false;
            editCommandButton.Click += editCommandButton_Click;
            // 
            // classpathsTab
            // 
            classpathsTab.Controls.Add(groupBox3);
            classpathsTab.Controls.Add(label3);
            classpathsTab.Controls.Add(btnGlobalClasspaths);
            classpathsTab.Location = new Point(4, 22);
            classpathsTab.Name = "classpathsTab";
            classpathsTab.Size = new Size(334, 272);
            classpathsTab.TabIndex = 3;
            classpathsTab.Text = "Classpaths";
            classpathsTab.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            groupBox3.Anchor = ((AnchorStyles.Top | AnchorStyles.Bottom) | AnchorStyles.Left) | AnchorStyles.Right;
            groupBox3.Controls.Add(label2);
            groupBox3.Location = new Point(8, 3);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(319, 192);
            groupBox3.TabIndex = 0;
            groupBox3.TabStop = false;
            groupBox3.Text = "&Project Classpaths";
            // 
            // label2
            // 
            label2.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label2.ForeColor = Color.FromArgb(64, 64, 64);
            label2.Location = new Point(16, 166);
            label2.Name = "label2";
            label2.Size = new Size(288, 13);
            label2.TabIndex = 2;
            label2.Text = "Project classpaths are relative to the project location";
            // 
            // label3
            // 
            label3.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            label3.ForeColor = Color.FromArgb(64, 64, 64);
            label3.Location = new Point(14, 204);
            label3.Name = "label3";
            label3.Size = new Size(313, 31);
            label3.TabIndex = 1;
            label3.Text = "Global classpaths are specific to your machine\r\nand are not stored in the project file.";
            // 
            // btnGlobalClasspaths
            // 
            btnGlobalClasspaths.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnGlobalClasspaths.Location = new Point(15, 237);
            btnGlobalClasspaths.Name = "btnGlobalClasspaths";
            btnGlobalClasspaths.Size = new Size(150, 21);
            btnGlobalClasspaths.TabIndex = 2;
            btnGlobalClasspaths.Text = "&Edit Global Classpaths...";
            btnGlobalClasspaths.Click += btnGlobalClasspaths_Click;
            // 
            // buildTab
            // 
            buildTab.Controls.Add(groupBox5);
            buildTab.Controls.Add(groupBox4);
            buildTab.Location = new Point(4, 22);
            buildTab.Name = "buildTab";
            buildTab.Size = new Size(334, 272);
            buildTab.TabIndex = 4;
            buildTab.Text = "Build";
            buildTab.UseVisualStyleBackColor = true;
            // 
            // groupBox5
            //
            groupBox5.Anchor = ((AnchorStyles.Top | AnchorStyles.Bottom) | AnchorStyles.Left) | AnchorStyles.Right;
            groupBox5.Controls.Add(alwaysExecuteCheckBox);
            groupBox5.Controls.Add(postBuilderButton);
            groupBox5.Controls.Add(postBuildBox);
            groupBox5.Location = new Point(8, 145);
            groupBox5.Name = "groupBox5";
            groupBox5.Size = new Size(319, 118);
            groupBox5.TabIndex = 1;
            groupBox5.TabStop = false;
            groupBox5.Text = "Post-Build Command Line";
            // 
            // alwaysExecuteCheckBox
            //
            alwaysExecuteCheckBox.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            alwaysExecuteCheckBox.Location = new Point(13, 84);
            alwaysExecuteCheckBox.Name = "alwaysExecuteCheckBox";
            alwaysExecuteCheckBox.Size = new Size(144, 17);
            alwaysExecuteCheckBox.TabIndex = 2;
            alwaysExecuteCheckBox.Text = "Always execute";
            agressiveTip.SetToolTip(alwaysExecuteCheckBox, "Execute the Post-Build Command Line even after a failed build");
            alwaysExecuteCheckBox.CheckedChanged += alwaysExecuteCheckBox_CheckedChanged;
            // 
            // postBuilderButton
            // 
            postBuilderButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            postBuilderButton.Location = new Point(232, 81);
            postBuilderButton.Name = "postBuilderButton";
            postBuilderButton.Size = new Size(75, 21);
            postBuilderButton.TabIndex = 1;
            postBuilderButton.Text = "Builder...";
            postBuilderButton.Click += postBuilderButton_Click;
            // 
            // postBuildBox
            // 
            postBuildBox.Anchor = ((AnchorStyles.Top | AnchorStyles.Bottom) | AnchorStyles.Left) | AnchorStyles.Right;
            postBuildBox.Location = new Point(13, 21);
            postBuildBox.Multiline = true;
            postBuildBox.Name = "postBuildBox";
            postBuildBox.Size = new Size(293, 55);
            postBuildBox.TabIndex = 0;
            postBuildBox.TextChanged += postBuildBox_TextChanged;
            // 
            // groupBox4
            // 
            groupBox4.Anchor = (AnchorStyles.Top | AnchorStyles.Left) | AnchorStyles.Right;
            groupBox4.Controls.Add(preBuilderButton);
            groupBox4.Controls.Add(preBuildBox);
            groupBox4.Location = new Point(8, 3);
            groupBox4.Name = "groupBox4";
            groupBox4.Size = new Size(319, 140);
            groupBox4.TabIndex = 0;
            groupBox4.TabStop = false;
            groupBox4.Text = "Pre-Build Command Line";
            // 
            // preBuilderButton
            // 
            preBuilderButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            preBuilderButton.Location = new Point(232, 103);
            preBuilderButton.Name = "preBuilderButton";
            preBuilderButton.Size = new Size(75, 21);
            preBuilderButton.TabIndex = 1;
            preBuilderButton.Text = "Builder...";
            preBuilderButton.Click += preBuilderButton_Click;
            // 
            // preBuildBox
            // 
            preBuildBox.Anchor = ((AnchorStyles.Top | AnchorStyles.Bottom) | AnchorStyles.Left) | AnchorStyles.Right;
            preBuildBox.Location = new Point(13, 21);
            preBuildBox.Multiline = true;
            preBuildBox.Name = "preBuildBox";
            preBuildBox.Size = new Size(293, 77);
            preBuildBox.TabIndex = 0;
            preBuildBox.TextChanged += preBuildBox_TextChanged;
            // 
            // compilerTab
            // 
            compilerTab.Controls.Add(propertyGrid);
            compilerTab.Location = new Point(4, 22);
            compilerTab.Name = "compilerTab";
            compilerTab.Padding = new Padding(3);
            compilerTab.Size = new Size(334, 272);
            compilerTab.TabIndex = 1;
            compilerTab.Text = "Compiler Options";
            compilerTab.UseVisualStyleBackColor = true;
            // 
            // propertyGrid
            // 
            propertyGrid.Dock = DockStyle.Fill;
            propertyGrid.Location = new Point(3, 3);
            propertyGrid.LineColor = SystemColors.ScrollBar;
            propertyGrid.Name = "propertyGrid";
            propertyGrid.Size = new Size(328, 266);
            propertyGrid.TabIndex = 0;
            propertyGrid.ToolbarVisible = false;
            propertyGrid.PropertyValueChanged += propertyGrid_PropertyValueChanged;
            propertyGrid.Font = Font;
            // 
            // sdkTabPage
            // 
            sdkTabPage.Controls.Add(sdkGroupBox);
            sdkTabPage.Controls.Add(customGroupBox);
            sdkTabPage.Location = new Point(4, 22);
            sdkTabPage.Name = "sdkTabPage";
            sdkTabPage.Padding = new Padding(3);
            sdkTabPage.Size = new Size(334, 272);
            sdkTabPage.TabIndex = 5;
            sdkTabPage.Text = "SDK";
            sdkTabPage.UseVisualStyleBackColor = true;
            // 
            // manageButton
            // 
            manageButton.Location = new Point(224, 37);
            manageButton.Name = "manageButton";
            manageButton.Size = new Size(85, 21);
            manageButton.TabIndex = 2;
            manageButton.Text = "Manage...";
            manageButton.UseVisualStyleBackColor = true;
            manageButton.Click += manageButton_Click;
            // 
            // sdkComboBox
            // 
            sdkComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            sdkComboBox.FormattingEnabled = true;
            sdkComboBox.Location = new Point(11, 39);
            sdkComboBox.Name = "sdkComboBox";
            sdkComboBox.Size = new Size(204, 21);
            sdkComboBox.TabIndex = 1;
            sdkComboBox.SelectedIndexChanged += sdkCombo_SelectedIndexChanged;
            // 
            // sdkGroupBox
            // 
            sdkGroupBox.Anchor = (AnchorStyles.Top | AnchorStyles.Left) | AnchorStyles.Right;
            sdkGroupBox.Controls.Add(labelUseGlobal);
            sdkGroupBox.Controls.Add(labelWarning);
            sdkGroupBox.Controls.Add(warningImage);
            sdkGroupBox.Controls.Add(sdkComboBox);
            sdkGroupBox.Controls.Add(manageButton);
            sdkGroupBox.Location = new Point(8, 3);
            sdkGroupBox.Name = "sdkGroupBox";
            sdkGroupBox.Size = new Size(319, 92);
            sdkGroupBox.TabIndex = 1;
            sdkGroupBox.TabStop = false;
            sdkGroupBox.Text = "Installed SDKs";
            // 
            // labelUseGlobal
            // 
            labelUseGlobal.Location = new Point(8, 18);
            labelUseGlobal.Name = "labelUseGlobal";
            labelUseGlobal.Size = new Size(154, 13);
            labelUseGlobal.TabIndex = 3;
            labelUseGlobal.Text = "Use a SDK configured globally.";
            // 
            // warningImage
            // 
            warningImage.Location = new Point(12, 67);
            warningImage.Name = "warningImage";
            warningImage.Size = new Size(16, 16);
            warningImage.TabIndex = 4;
            warningImage.TabStop = false;
            warningImage.Visible = false;
            // 
            // labelWarning
            // 
            labelWarning.AutoSize = true;
            labelWarning.Location = new Point(30, 68);
            labelWarning.Name = "labelWarning";
            labelWarning.Size = new Size(242, 13);
            labelWarning.TabIndex = 5;
            labelWarning.Text = "";
            // 
            // customGroupBox
            // 
            customGroupBox.Anchor = (AnchorStyles.Top | AnchorStyles.Left) | AnchorStyles.Right;
            customGroupBox.Controls.Add(browseButton);
            customGroupBox.Controls.Add(customTextBox);
            customGroupBox.Controls.Add(labelUseCustom);
            customGroupBox.Location = new Point(8, 102);
            customGroupBox.Name = "customGroupBox";
            customGroupBox.Size = new Size(319, 74);
            customGroupBox.TabIndex = 2;
            customGroupBox.TabStop = false;
            customGroupBox.Text = "Custom SDK";
            // 
            // browseButton
            // 
            browseButton.Location = new Point(224, 37);
            browseButton.Name = "browseButton";
            browseButton.Size = new Size(85, 21);
            browseButton.TabIndex = 6;
            browseButton.Text = "Browse...";
            browseButton.UseVisualStyleBackColor = true;
            browseButton.Click += browseButton_Click;
            // 
            // customTextBox
            // 
            customTextBox.Location = new Point(11, 38);
            customTextBox.Name = "customTextBox";
            customTextBox.Size = new Size(204, 21);
            customTextBox.TabIndex = 5;
            customTextBox.TextChanged += customTextBox_TextChanged;
            // 
            // labelUseCustom
            //
            labelUseCustom.Location = new Point(8, 18);
            labelUseCustom.Name = "labelUseCustom";
            labelUseCustom.Size = new Size(213, 13);
            labelUseCustom.TabIndex = 4;
            labelUseCustom.Text = "Relative or absolute path to a specific SDK.";
            // 
            // agressiveTip
            // 
            agressiveTip.AutomaticDelay = 0;
            // 
            // PropertiesDialog
            // 
            AcceptButton = btnOK;
            CancelButton = btnCancel;
            AutoScaleBaseSize = new Size(5, 13);
            ClientSize = new Size(366, 348);
            MinimumSize = new Size(377, 377);
            Controls.Add(tabControl);
            Controls.Add(btnApply);
            Controls.Add(btnCancel);
            Controls.Add(btnOK);
            FormBorderStyle = FormBorderStyle.Sizable;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "PropertiesDialog";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Project Properties";
            tabControl.ResumeLayout(false);
            movieTab.ResumeLayout(false);
            movieTab.PerformLayout();
            platformGroupBox.ResumeLayout(false);
            generalGroupBox.ResumeLayout(false);
            generalGroupBox.PerformLayout();
            playGroupBox.ResumeLayout(false);
            classpathsTab.ResumeLayout(false);
            groupBox3.ResumeLayout(false);
            buildTab.ResumeLayout(false);
            groupBox5.ResumeLayout(false);
            groupBox5.PerformLayout();
            groupBox4.ResumeLayout(false);
            groupBox4.PerformLayout();
            compilerTab.ResumeLayout(false);
            sdkGroupBox.ResumeLayout(false);
            sdkGroupBox.PerformLayout();
            customGroupBox.ResumeLayout(false);
            customGroupBox.PerformLayout();
            sdkTabPage.ResumeLayout(false);
            sdkTabPage.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        #endregion

        CompilerOptions optionsCopy;
        bool sdkChanged;
        bool isPropertyGridReadOnly;
        LanguagePlatform langPlatform;

        public event EventHandler OpenGlobalClasspaths;

        public PropertiesDialog() 
        {
            Font = PluginBase.Settings.DefaultFont;
            FormGuid = "4216fccc-781b-4b06-89f6-d9f7e77c2e5f";
            InitializeComponent();
            CreateClassPathControl();
            InitializeLocalization();
        }

        void CreateClassPathControl()
        {
            classpathControl = new ClasspathControl();
            classpathControl.Anchor = ((AnchorStyles.Top | AnchorStyles.Bottom) | AnchorStyles.Left) | AnchorStyles.Right;
            classpathControl.Classpaths = Array.Empty<string>();
            classpathControl.Language = null;
            classpathControl.Location = new Point(17, 22);
            classpathControl.Name = "classpathControl";
            classpathControl.Project = null;
            classpathControl.Size = new Size(287, 134);
            classpathControl.TabIndex = 0;
            groupBox3.Controls.Add(classpathControl);
        }

        protected Project BaseProject { get; private set; }

        public void SetProject(Project project)
        {
            BaseProject = project;
            BuildDisplay();
        }

        #region Change Tracking

        public bool PropertiesChanged { get; protected set; }

        public bool ClasspathsChanged { get; protected set; }

        public bool AssetsChanged { get; protected set; }

        public bool PlatformChanged { get; protected set; }

        #endregion

        #region Localization

        void InitializeLocalization()
        {
            btnOK.Text = TextHelper.GetString("Label.OK");
            buildTab.Text = TextHelper.GetString("Info.Build");
            movieTab.Text = TextHelper.GetString("Info.Output");
            btnApply.Text = TextHelper.GetString("Label.Apply");
            btnCancel.Text = TextHelper.GetString("Label.Cancel");
            preBuilderButton.Text = TextHelper.GetString("Info.Builder");
            postBuilderButton.Text = TextHelper.GetString("Info.Builder");
            label2.Text = TextHelper.GetString("Info.ProjectClasspaths");
            outputTypeLabel.Text = TextHelper.GetString("Info.OutputType");
            outputBrowseButton.Text = TextHelper.GetString("Label.Browse");
            groupBox3.Text = TextHelper.GetStringWithoutEllipsis("Label.ProjectClasspaths");
            groupBox5.Text = TextHelper.GetString("Info.PostBuildCmdLine");
            dimensionsLabel.Text = TextHelper.GetString("Label.Dimensions");
            label3.Text = string.Format(TextHelper.GetString("Info.GlobalClasspaths"), "\n");
            agressiveTip.SetToolTip(alwaysExecuteCheckBox, TextHelper.GetString("ToolTip.AlwaysExecute"));
            btnGlobalClasspaths.Text = TextHelper.GetString("Label.EditGlobalClasspaths");
            alwaysExecuteCheckBox.Text = TextHelper.GetString("Info.AlwaysExecute");
            bgcolorLabel.Text = TextHelper.GetString("Label.BackgroundColor");
            framerateLabel.Text = TextHelper.GetString("Label.FrameRate");
            compilerTab.Text = TextHelper.GetString("Info.CompilerOptions");
            platformGroupBox.Text = TextHelper.GetString("Info.Platform");
            groupBox4.Text = TextHelper.GetString("Info.PreBuildCmdLine");
            exportinLabel.Text = TextHelper.GetString("Label.OutputFile");
            classpathsTab.Text = TextHelper.GetString("Info.Classpaths");
            Text = " " + TextHelper.GetString("Title.ProjectProperties");
            generalGroupBox.Text = TextHelper.GetString("Info.General");
            playGroupBox.Text = TextHelper.GetString("Label.TestMovie");
            editCommandButton.Text = TextHelper.GetString("Info.EditCommand");
            manageButton.Text = TextHelper.GetString("Label.Manage");
            sdkGroupBox.Text = TextHelper.GetString("Label.SDKGroup");
            sdkTabPage.Text = TextHelper.GetString("Label.SDKTab");
            customGroupBox.Text = TextHelper.GetString("Label.SDKTabCustom");
            browseButton.Text = TextHelper.GetString("Label.SDKBrowse");
            labelUseCustom.Text = TextHelper.GetString("Label.SDKUseCustom");
            labelUseGlobal.Text = TextHelper.GetString("Label.SDKUseGlobal");
        }

        #endregion

        protected virtual void BuildDisplay()
        {
            Text = " " + BaseProject.Name + " (" + BaseProject.LanguageDisplayName + ") " + TextHelper.GetString("Info.Properties");

            langPlatform = GetLanguagePlatform(BaseProject.MovieOptions.Platform);

            InitOutputTab();
            InitBuildTab();
            InitOptionsTab();
            if (BaseProject.IsCompilable)
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

        void InitOptionsTab() => UpdateCompilerOptions();

        void UpdateCompilerOptions()
        {
            var readOnly = IsExternalConfiguration();
            if (readOnly == isPropertyGridReadOnly && propertyGrid.SelectedObject != null) 
                return;
            isPropertyGridReadOnly = readOnly;

            // clone the compiler options object because the PropertyGrid modifies its
            // object directly
            optionsCopy = BaseProject.CompilerOptions.Clone();

            if (isPropertyGridReadOnly)
                TypeDescriptor.AddAttributes(optionsCopy, new Attribute[] { new ReadOnlyAttribute(true) });

            propertyGrid.SelectedObject = optionsCopy;
            PropertiesChanged = false;
        }

        void InitBuildTab()
        {
            preBuildBox.Text = BaseProject.PreBuildEvent;
            postBuildBox.Text = BaseProject.PostBuildEvent;
            alwaysExecuteCheckBox.Checked = BaseProject.AlwaysRunPostBuild;
        }

        void InitClasspathTab()
        {
            classpathControl.Changed += classpathControl_Changed;
            classpathControl.Project = BaseProject;
            classpathControl.Classpaths = BaseProject.Classpaths.ToArray();
            classpathControl.Language = BaseProject.Language;
            classpathControl.LanguageBox.Visible = false;
            UpdateClasspaths();
            ClasspathsChanged = false;
        }

        void UpdateClasspaths()
        {
            if (IsExternalConfiguration())
            {
                classpathControl.Enabled = false;
                label2.Text = string.Format(TextHelper.GetString("Info.ProjectClasspathsDisabled"), langPlatform.Name);
                return;
            }

            classpathControl.Enabled = true;
            label2.Text = TextHelper.GetString("Info.ProjectClasspaths");
        }

        void InitSDKTab()
        {
            sdkComboBox.Items.Clear();
            int select = 0;
            BuildActions.LatestSDKMatchQuality = -1;
            warningImage.Visible = false;
            labelWarning.Text = "";

            // retrieve SDK list
            InstalledSDK[] sdks = BuildActions.GetInstalledSDKs(BaseProject);
            if (!sdks.IsNullOrEmpty())
            {
                sdkComboBox.Items.Add(TextHelper.GetString("Label.SDKComboDefault") + " (" + sdks[0].Name + ")");
                sdkComboBox.Items.AddRange(sdks);
            }
            else sdkComboBox.Items.Add(TextHelper.GetString("Label.SDKComboNoSDK"));

            InstalledSDK sdk = BuildActions.MatchSDK(sdks, BaseProject);
            if (sdk != InstalledSDK.INVALID_SDK)
            {
                if (BuildActions.LatestSDKMatchQuality >= 0)
                    select = 1 + Array.IndexOf(sdks, sdk);

                if (BuildActions.LatestSDKMatchQuality > 0)
                {
                    string icon = BuildActions.LatestSDKMatchQuality < 10 ? "196" : "197";
                    warningImage.Image = PluginBase.MainForm.FindImage(icon, false);
                    warningImage.Visible = true;
                    string[] p = (BaseProject.PreferredSDK + ";;").Split(';');
                    labelWarning.Text = TextHelper.GetString("Label.SDKExpected") 
                        + " " + p[0] + " (" + p[1] + ")";
                }
            }

            sdkComboBox.SelectedIndex = select;
            if (sdk != InstalledSDK.INVALID_SDK && (sdks is null || !sdks.Contains(sdk)))
                customTextBox.Text = sdk.Path;
            sdkChanged = false;
        }

        void InitOutputTab()
        {
            MovieOptions options = BaseProject.MovieOptions;

            var types = Array.ConvertAll(BaseProject.MovieOptions.OutputTypes, it => it.ToString());
            InitCombo(outputCombo, types, BaseProject.OutputType, "Label.OutputType");
            outputCombo.SelectedIndexChanged += outputCombo_SelectedIndexChanged;

            outputSwfBox.Text = BaseProject.OutputPath;
            widthTextBox.Text = options.Width.ToString();
            heightTextBox.Text = options.Height.ToString();
            colorTextBox.Text = options.Background;
            fpsTextBox.Text = options.Fps.ToString();

            InitCombo(platformCombo, BaseProject.MovieOptions.TargetPlatforms, BaseProject.MovieOptions.Platform);
            platformCombo.SelectedIndexChanged += platformCombo_SelectedIndexChanged;

            InitCombo(versionCombo, BaseProject.MovieOptions.TargetVersions(platformCombo.Text), BaseProject.MovieOptions.Version);
            versionCombo.SelectedIndexChanged += versionCombo_SelectedIndexChanged;
            UpdateVersionCombo();

            InitTestMovieOptions();
            UpdateGeneralPanel();
            UpdateEditCommandButton();
        }

        void UpdateEditCommandButton()
        {
            var state = GetTestMovie();
            editCommandButton.Visible = state == TestMovieBehavior.Custom 
                || state == TestMovieBehavior.OpenDocument
                || state == TestMovieBehavior.Webserver;
        }

        void UpdateVersionCombo()
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

        void InitTestMovieOptions()
        {
            var output = GetOutput();
            var platform = GetPlatform();
            var options = new List<TestMovieBehavior>();
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
            var items = options.ConvertAll(item => item.ToString());
            InitCombo(testMovieCombo, items.ToArray(), BaseProject.TestMovieBehavior.ToString(), "Label.TestMovie");
        }

        static void InitCombo(ComboBox combo, object[] items, object select) => InitCombo(combo, items, @select, null);

        static void InitCombo(ComboBox combo, object[] values, object select, string localizePrefix)
        {
            combo.Items.Clear();
            var items = Array.ConvertAll(values, value => new ComboItem(value, localizePrefix));
            combo.Items.AddRange(items);

            if (select != null)
            {
                var value = select.ToString();
                var index = Array.FindIndex(items, item => item.Value.ToString() == value);
                if (index >= 0) combo.SelectedIndex = index;
            }
        }

        string GetPlatform()
        {
            if (platformCombo.SelectedIndex < 0) return null;
            return ((ComboItem) platformCombo.SelectedItem).Value.ToString();
        }

        OutputType GetOutput()
        {
            if (outputCombo.SelectedIndex < 0) return OutputType.Unknown;
            return (OutputType)Enum.Parse(typeof(OutputType), ((ComboItem) outputCombo.SelectedItem).Value.ToString());
        }

        TestMovieBehavior GetTestMovie()
        {
            if (testMovieCombo.SelectedIndex < 0) return TestMovieBehavior.Unknown;
            return (TestMovieBehavior)Enum.Parse(typeof(TestMovieBehavior), ((ComboItem) testMovieCombo.SelectedItem).Value.ToString());
        }

        protected void Modified() => btnApply.Enabled = true;

        protected virtual bool Apply()
        {
            var options = BaseProject.MovieOptions;
            try
            {
                BaseProject.OutputType = GetOutput();
                if (OuputValid(outputSwfBox.Text)) BaseProject.OutputPath = outputSwfBox.Text;
                BaseProject.Classpaths.Clear();
                BaseProject.Classpaths.AddRange(classpathControl.Classpaths);
                options.Width = int.Parse(widthTextBox.Text);
                options.Height = int.Parse(heightTextBox.Text);
                options.BackgroundColor = Color.FromArgb(255, colorLabel.BackColor);
                options.Fps = int.Parse(fpsTextBox.Text);
                options.Platform = GetPlatform();
                options.Version = versionCombo.Text;
                BaseProject.PreBuildEvent = preBuildBox.Text;
                BaseProject.PostBuildEvent = postBuildBox.Text;
                BaseProject.AlwaysRunPostBuild = alwaysExecuteCheckBox.Checked;
                BaseProject.TestMovieBehavior = GetTestMovie();

                if (sdkChanged)
                {
                    if (customTextBox.Text.Length > 0) BaseProject.PreferredSDK = customTextBox.Text;
                    else
                    {
                        var sdk = sdkComboBox.SelectedItem as InstalledSDK;
                        BaseProject.PreferredSDK = sdk?.ToPreferredSDK();
                    }
                }
            }
            catch (Exception exception)
            {
                ErrorManager.ShowError(exception);
                return false;
            }
            // copy compiler option values
            BaseProject.CompilerOptions = optionsCopy;
            btnApply.Enabled = false;
            PropertiesChanged = true;
            return true;
        }

        static bool OuputValid(string path)
        {
            try
            {
                if (path != "") new FileInfo(path);
                return true;
            }
            catch(Exception ex)
            {
                ErrorManager.ShowInfo(ex.Message);
                return false;
            }
        }

        void btnOK_Click(object sender, EventArgs e)
        {
            if (btnApply.Enabled) if (!Apply()) return;
            Close();
        }

        void btnCancel_Click(object sender, EventArgs e) => Close();

        void btnApply_Click(object sender, EventArgs e) => Apply();

        void outputSwfBox_TextChanged(object sender, EventArgs e)
        {
            ClasspathsChanged = true;
            Modified();
        }

        void widthTextBox_TextChanged(object sender, EventArgs e) { Modified(); }

        void heightTextBox_TextChanged(object sender, EventArgs e) { Modified(); }

        void colorTextBox_TextChanged(object sender, EventArgs e) 
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
            catch { colorLabel.BackColor = SystemColors.WindowText; }
            Modified(); 
        }

        void fpsTextBox_TextChanged(object sender, EventArgs e) => Modified();

        void preBuildBox_TextChanged(object sender, EventArgs e) => Modified();

        void postBuildBox_TextChanged(object sender, EventArgs e) => Modified();

        void alwaysExecuteCheckBox_CheckedChanged(object sender, EventArgs e) => Modified();

        void testMovieCombo_SelectedIndexChanged(object sender, EventArgs e) 
        { 
            Modified();
            editCommandButton.Visible = testMovieCombo.Text.IndexOfOrdinal("..") > 0;
        }

        void propertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e) => Modified();

        void platformCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            langPlatform = GetLanguagePlatform(platformCombo.Text);

            versionCombo.Items.Clear();
            versionCombo.Items.AddRange(BaseProject.MovieOptions.TargetVersions(platformCombo.Text));
            versionCombo.SelectedIndex = Math.Max(0, versionCombo.Items.IndexOf(BaseProject.MovieOptions.DefaultVersion(platformCombo.Text)));
            UpdateVersionCombo();
            InitTestMovieOptions();
            UpdateGeneralPanel();
            UpdateEditCommandButton();
            DetectExternalToolchain();
            UpdateClasspaths();
            UpdateCompilerOptions();
            PlatformChanged = true;
            Modified();
        }

        static void SelectItem(ComboBox combo, object value)
        {
            foreach (var item in combo.Items)
                if (item.ToString() == value.ToString())
                {
                    combo.SelectedItem = item;
                    break;
                }
        }

        void versionCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            PlatformChanged = true;
            Modified();
        }

        void classpathControl_Changed(object sender, EventArgs e)
        {
            ClasspathsChanged = true; // keep special track of this, it's a big deal
            Modified();
        }

        void colorLabel_Click(object sender, EventArgs e)
        {
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                colorLabel.BackColor = colorDialog.Color;
                colorTextBox.Text = ToHtml(colorLabel.BackColor);
                Modified();
            }
        }

        static string ToHtml(Color c) => $"#{(c.R << 16) + (c.G << 8) + c.B:X6}";

        void btnGlobalClasspaths_Click(object sender, EventArgs e) => OpenGlobalClasspaths?.Invoke(this,new EventArgs());

        void outputBrowseButton_Click(object sender, EventArgs e)
        {
            using var dialog = new SaveFileDialog();
            dialog.Filter = "*.*|*.*";
            dialog.OverwritePrompt = false;
            dialog.InitialDirectory = BaseProject.Directory;
            // try pre-setting the current output path
            try
            {
                var path = BaseProject.GetAbsolutePath(outputSwfBox.Text);
                if (File.Exists(path)) dialog.FileName = path;
            }
            catch { }
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                outputSwfBox.Text = BaseProject.GetRelativePath(dialog.FileName);
            }
        }

        void editCommandButton_Click(object sender, EventArgs e)
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

            using var dialog = new LineEntryDialog(caption, label, BaseProject.TestMovieCommand ?? "");
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                BaseProject.TestMovieCommand = dialog.Line;
                Modified();
                btnOK.Focus();
            }
        }

        void preBuilderButton_Click(object sender, EventArgs e)
        {
            using var dialog = new BuildEventDialog(BaseProject) {CommandLine = preBuildBox.Text};
            if (dialog.ShowDialog(this) == DialogResult.OK) preBuildBox.Text = dialog.CommandLine;
        }

        void postBuilderButton_Click(object sender, EventArgs e)
        {
            using var dialog = new BuildEventDialog(BaseProject) {CommandLine = postBuildBox.Text};
            if (dialog.ShowDialog(this) == DialogResult.OK) postBuildBox.Text = dialog.CommandLine;
        }

        void outputCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateGeneralPanel();
            InitTestMovieOptions();
            Modified();
        }

        void UpdateGeneralPanel()
        {
            OutputType output = GetOutput();
            generalGroupBox.Enabled = BaseProject.MovieOptions.HasOutput(output);
            testMovieCombo.Enabled = (output != OutputType.Library);
            
            bool isGraphical = BaseProject.MovieOptions.IsGraphical(GetPlatform());
            widthTextBox.Enabled = heightTextBox.Enabled = fpsTextBox.Enabled
                = colorTextBox.Enabled = colorLabel.Enabled = isGraphical;

            if (IsExternalToolchain())
                exportinLabel.Text = TextHelper.GetString("Label.ConfigurationFile");
            else
                exportinLabel.Text = TextHelper.GetString("Label.OutputFile");
        }

        void manageButton_Click(object sender, EventArgs e)
        {
            var de = new DataEvent(EventType.Command, "ASCompletion.ShowSettings", BaseProject.Language);
            EventManager.DispatchEvent(this, de);
            if (de.Handled) InitSDKTab();
        }

        void customTextBox_TextChanged(object sender, EventArgs e)
        {
            sdkChanged = true;
            Modified();
        }

        void browseButton_Click(object sender, EventArgs e)
        {
            using var folder = new VistaFolderBrowserDialog();
            if (Directory.Exists(customTextBox.Text)) folder.SelectedPath = customTextBox.Text;
            if (folder.ShowDialog() == DialogResult.OK) customTextBox.Text = folder.SelectedPath;
        }

        void sdkCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            customTextBox.Text = "";
            sdkChanged = true;
            Modified();
        }

        /* PLATFORM CONFIGURATION */

        void DetectExternalToolchain()
        {
            if (!IsExternalToolchain()) return;

            SelectItem(outputCombo, OutputType.Application);
            SelectItem(testMovieCombo, TestMovieBehavior.Custom);
            BaseProject.TestMovieCommand = "";

            if (langPlatform.DefaultProjectFile is null) return;

            foreach (string fileName in langPlatform.DefaultProjectFile)
                if (File.Exists(BaseProject.GetAbsolutePath(fileName)))
                {
                    outputSwfBox.Text = fileName;
                    break;
                }
        }

        bool IsExternalConfiguration()
        {
            var selectedVersion = versionCombo.Text.Length == 0 ? "1.0" : versionCombo.Text;
            var version = langPlatform?.GetVersion(selectedVersion);
            return version?.Commands != null && version.Commands.ContainsKey("display");
        }

        bool IsExternalToolchain() => langPlatform?.ExternalToolchain != null;

        LanguagePlatform GetLanguagePlatform(string platformName)
        {
            if (PlatformData.SupportedLanguages.ContainsKey(BaseProject.Language))
            {
                var lang = PlatformData.SupportedLanguages[BaseProject.Language];
                if (lang.Platforms.ContainsKey(platformName)) return lang.Platforms[platformName];
            }
            return null;
        }

    }

    internal class ComboItem
    {
        public string Label;
        public object Value;

        public ComboItem(object value, string localizePrefix)
        {
            Value = value;
            Label = localizePrefix != null ? TextHelper.GetString(localizePrefix + value) : value.ToString();
            if (string.IsNullOrEmpty(Label)) Label = value.ToString();
        }

        public override string ToString() => Label;
    }
}
