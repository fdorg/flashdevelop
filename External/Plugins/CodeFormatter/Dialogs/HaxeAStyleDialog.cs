using CodeFormatter.Preferences;
using CodeFormatter.Utilities;
using PluginCore;
using ScintillaNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using PluginCore.Localization;
using System.Reflection;
using System.Windows.Forms.Design;

namespace CodeFormatter.Dialogs
{
    public class HaxeAStyleDialog : Form, IWindowsFormsEditorService
    {
        private readonly ScintillaControl txtExample;
        private readonly Dictionary<CheckBox, string> mapping = new Dictionary<CheckBox, string>();

        private readonly string exampleCode;

        #region Windows Form Designer generated code

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabIndents;
        private System.Windows.Forms.Label lblIndentSize;
        private System.Windows.Forms.NumericUpDown numIndentWidth;
        private System.Windows.Forms.CheckBox checkForceTabs;
        private System.Windows.Forms.CheckBox checkTabs;
        private System.Windows.Forms.TabPage tabBrackets;
        private System.Windows.Forms.CheckBox checkIndentCase;
        private System.Windows.Forms.CheckBox checkIndentSwitches;
        private System.Windows.Forms.Panel pnlSci;
        private System.Windows.Forms.Label lblBracketStyle;
        private System.Windows.Forms.ComboBox cbBracketStyle;
        private System.Windows.Forms.CheckBox checkIndentConditional;
        private System.Windows.Forms.CheckBox checkAttachClasses;
        private System.Windows.Forms.TabPage tabPadding;
        private System.Windows.Forms.CheckBox checkPadHeaders;
        private System.Windows.Forms.CheckBox checkPadCommas;
        private System.Windows.Forms.CheckBox checkPadAll;
        private System.Windows.Forms.CheckBox checkPadBlocks;
        private System.Windows.Forms.CheckBox checkFillEmptyLines;
        private System.Windows.Forms.CheckBox checkDeleteEmptyLines;
        private System.Windows.Forms.CheckBox checkAddBrackets;
        private System.Windows.Forms.CheckBox checkOneLineBrackets;
        private System.Windows.Forms.CheckBox checkRemoveBrackets;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.TabPage tabFormatting;
        private System.Windows.Forms.CheckBox checkKeepOneLineStatements;
        private System.Windows.Forms.CheckBox checkKeepOneLineBlocks;
        private System.Windows.Forms.CheckBox checkBreakElseifs;
        private System.Windows.Forms.CheckBox checkBreakClosing;
        private CheckBox checkPadParensIn;
        private CheckBox checkPadParensOut;
        private CheckBox checkCurrentFile;

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

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabIndents = new System.Windows.Forms.TabPage();
            this.checkIndentConditional = new System.Windows.Forms.CheckBox();
            this.checkIndentCase = new System.Windows.Forms.CheckBox();
            this.checkIndentSwitches = new System.Windows.Forms.CheckBox();
            this.lblIndentSize = new System.Windows.Forms.Label();
            this.numIndentWidth = new System.Windows.Forms.NumericUpDown();
            this.checkForceTabs = new System.Windows.Forms.CheckBox();
            this.checkTabs = new System.Windows.Forms.CheckBox();
            this.tabBrackets = new System.Windows.Forms.TabPage();
            this.checkBreakClosing = new System.Windows.Forms.CheckBox();
            this.checkRemoveBrackets = new System.Windows.Forms.CheckBox();
            this.checkOneLineBrackets = new System.Windows.Forms.CheckBox();
            this.checkAddBrackets = new System.Windows.Forms.CheckBox();
            this.checkAttachClasses = new System.Windows.Forms.CheckBox();
            this.lblBracketStyle = new System.Windows.Forms.Label();
            this.cbBracketStyle = new System.Windows.Forms.ComboBox();
            this.tabPadding = new System.Windows.Forms.TabPage();
            this.checkPadParensOut = new System.Windows.Forms.CheckBox();
            this.checkPadParensIn = new System.Windows.Forms.CheckBox();
            this.checkFillEmptyLines = new System.Windows.Forms.CheckBox();
            this.checkDeleteEmptyLines = new System.Windows.Forms.CheckBox();
            this.checkPadHeaders = new System.Windows.Forms.CheckBox();
            this.checkPadCommas = new System.Windows.Forms.CheckBox();
            this.checkPadAll = new System.Windows.Forms.CheckBox();
            this.checkPadBlocks = new System.Windows.Forms.CheckBox();
            this.tabFormatting = new System.Windows.Forms.TabPage();
            this.checkKeepOneLineStatements = new System.Windows.Forms.CheckBox();
            this.checkKeepOneLineBlocks = new System.Windows.Forms.CheckBox();
            this.checkBreakElseifs = new System.Windows.Forms.CheckBox();
            this.pnlSci = new System.Windows.Forms.Panel();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.checkCurrentFile = new System.Windows.Forms.CheckBox();
            this.tabControl.SuspendLayout();
            this.tabIndents.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numIndentWidth)).BeginInit();
            this.tabBrackets.SuspendLayout();
            this.tabPadding.SuspendLayout();
            this.tabFormatting.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabIndents);
            this.tabControl.Controls.Add(this.tabBrackets);
            this.tabControl.Controls.Add(this.tabPadding);
            this.tabControl.Controls.Add(this.tabFormatting);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(764, 506);
            this.tabControl.TabIndex = 0;
            // 
            // tabIndents
            // 
            this.tabIndents.Controls.Add(this.checkIndentConditional);
            this.tabIndents.Controls.Add(this.checkIndentCase);
            this.tabIndents.Controls.Add(this.checkIndentSwitches);
            this.tabIndents.Controls.Add(this.lblIndentSize);
            this.tabIndents.Controls.Add(this.numIndentWidth);
            this.tabIndents.Controls.Add(this.checkForceTabs);
            this.tabIndents.Controls.Add(this.checkTabs);
            this.tabIndents.Location = new System.Drawing.Point(4, 22);
            this.tabIndents.Name = "tabIndents";
            this.tabIndents.Padding = new System.Windows.Forms.Padding(3);
            this.tabIndents.Size = new System.Drawing.Size(756, 480);
            this.tabIndents.TabIndex = 0;
            this.tabIndents.Text = "Tabs / Indents";
            this.tabIndents.UseVisualStyleBackColor = true;
            // 
            // checkIndentConditional
            // 
            this.checkIndentConditional.AutoSize = true;
            this.checkIndentConditional.Location = new System.Drawing.Point(6, 119);
            this.checkIndentConditional.Name = "checkIndentConditional";
            this.checkIndentConditional.Size = new System.Drawing.Size(166, 17);
            this.checkIndentConditional.TabIndex = 11;
            this.checkIndentConditional.Tag = "--indent-preproc-cond";
            this.checkIndentConditional.Text = "Indent conditional compilation";
            this.checkIndentConditional.UseVisualStyleBackColor = true;
            this.checkIndentConditional.Click += new System.EventHandler(this.check_Click);
            // 
            // checkIndentCase
            // 
            this.checkIndentCase.AutoSize = true;
            this.checkIndentCase.Location = new System.Drawing.Point(6, 96);
            this.checkIndentCase.Name = "checkIndentCase";
            this.checkIndentCase.Size = new System.Drawing.Size(116, 17);
            this.checkIndentCase.TabIndex = 6;
            this.checkIndentCase.Tag = "--indent-cases";
            this.checkIndentCase.Text = "Indent case blocks";
            this.checkIndentCase.UseVisualStyleBackColor = true;
            this.checkIndentCase.Click += new System.EventHandler(this.check_Click);
            // 
            // checkIndentSwitches
            // 
            this.checkIndentSwitches.AutoSize = true;
            this.checkIndentSwitches.Location = new System.Drawing.Point(6, 73);
            this.checkIndentSwitches.Name = "checkIndentSwitches";
            this.checkIndentSwitches.Size = new System.Drawing.Size(100, 17);
            this.checkIndentSwitches.TabIndex = 5;
            this.checkIndentSwitches.Tag = "--indent-switches";
            this.checkIndentSwitches.Text = "Indent switches";
            this.checkIndentSwitches.UseVisualStyleBackColor = true;
            this.checkIndentSwitches.Click += new System.EventHandler(this.check_Click);
            // 
            // lblIndentSize
            // 
            this.lblIndentSize.AutoSize = true;
            this.lblIndentSize.Location = new System.Drawing.Point(6, 49);
            this.lblIndentSize.Name = "lblIndentSize";
            this.lblIndentSize.Size = new System.Drawing.Size(38, 13);
            this.lblIndentSize.TabIndex = 3;
            this.lblIndentSize.Text = "Width:";
            // 
            // numIndentWidth
            // 
            this.numIndentWidth.Location = new System.Drawing.Point(50, 47);
            this.numIndentWidth.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.numIndentWidth.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numIndentWidth.Name = "numIndentWidth";
            this.numIndentWidth.Size = new System.Drawing.Size(40, 20);
            this.numIndentWidth.TabIndex = 2;
            this.numIndentWidth.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.numIndentWidth.ValueChanged += new System.EventHandler(this.numIndentWidth_ValueChanged);
            // 
            // checkForceTabs
            // 
            this.checkForceTabs.AutoSize = true;
            this.checkForceTabs.Location = new System.Drawing.Point(25, 29);
            this.checkForceTabs.Name = "checkForceTabs";
            this.checkForceTabs.Size = new System.Drawing.Size(80, 17);
            this.checkForceTabs.TabIndex = 1;
            this.checkForceTabs.Text = "Force Tabs";
            this.checkForceTabs.UseVisualStyleBackColor = true;
            this.checkForceTabs.Click += new System.EventHandler(this.check_Click);
            // 
            // checkTabs
            // 
            this.checkTabs.AutoSize = true;
            this.checkTabs.Location = new System.Drawing.Point(6, 6);
            this.checkTabs.Name = "checkTabs";
            this.checkTabs.Size = new System.Drawing.Size(72, 17);
            this.checkTabs.TabIndex = 0;
            this.checkTabs.Text = "Use Tabs";
            this.checkTabs.UseVisualStyleBackColor = true;
            this.checkTabs.Click += new System.EventHandler(this.check_Click);
            // 
            // tabBrackets
            // 
            this.tabBrackets.Controls.Add(this.checkBreakClosing);
            this.tabBrackets.Controls.Add(this.checkRemoveBrackets);
            this.tabBrackets.Controls.Add(this.checkOneLineBrackets);
            this.tabBrackets.Controls.Add(this.checkAddBrackets);
            this.tabBrackets.Controls.Add(this.checkAttachClasses);
            this.tabBrackets.Controls.Add(this.lblBracketStyle);
            this.tabBrackets.Controls.Add(this.cbBracketStyle);
            this.tabBrackets.Location = new System.Drawing.Point(4, 22);
            this.tabBrackets.Name = "tabBrackets";
            this.tabBrackets.Padding = new System.Windows.Forms.Padding(3);
            this.tabBrackets.Size = new System.Drawing.Size(756, 480);
            this.tabBrackets.TabIndex = 1;
            this.tabBrackets.Text = "Brackets";
            this.tabBrackets.UseVisualStyleBackColor = true;
            // 
            // checkBreakClosing
            // 
            this.checkBreakClosing.AutoSize = true;
            this.checkBreakClosing.Location = new System.Drawing.Point(6, 125);
            this.checkBreakClosing.Name = "checkBreakClosing";
            this.checkBreakClosing.Size = new System.Drawing.Size(134, 17);
            this.checkBreakClosing.TabIndex = 12;
            this.checkBreakClosing.Tag = "--break-closing-brackets";
            this.checkBreakClosing.Text = "Break closing brackets";
            this.checkBreakClosing.UseVisualStyleBackColor = true;
            this.checkBreakClosing.Click += new System.EventHandler(this.check_Click);
            // 
            // checkRemoveBrackets
            // 
            this.checkRemoveBrackets.AutoSize = true;
            this.checkRemoveBrackets.Location = new System.Drawing.Point(6, 102);
            this.checkRemoveBrackets.Name = "checkRemoveBrackets";
            this.checkRemoveBrackets.Size = new System.Drawing.Size(183, 17);
            this.checkRemoveBrackets.TabIndex = 8;
            this.checkRemoveBrackets.Tag = "--remove-brackets";
            this.checkRemoveBrackets.Text = "Remove braces from conditionals";
            this.checkRemoveBrackets.UseVisualStyleBackColor = true;
            this.checkRemoveBrackets.Click += new System.EventHandler(this.check_Click);
            // 
            // checkOneLineBrackets
            // 
            this.checkOneLineBrackets.AutoSize = true;
            this.checkOneLineBrackets.Location = new System.Drawing.Point(25, 79);
            this.checkOneLineBrackets.Name = "checkOneLineBrackets";
            this.checkOneLineBrackets.Size = new System.Drawing.Size(160, 17);
            this.checkOneLineBrackets.TabIndex = 7;
            this.checkOneLineBrackets.Text = "Add braces on the same line";
            this.checkOneLineBrackets.UseVisualStyleBackColor = true;
            this.checkOneLineBrackets.Click += new System.EventHandler(this.check_Click);
            // 
            // checkAddBrackets
            // 
            this.checkAddBrackets.AutoSize = true;
            this.checkAddBrackets.Location = new System.Drawing.Point(6, 56);
            this.checkAddBrackets.Name = "checkAddBrackets";
            this.checkAddBrackets.Size = new System.Drawing.Size(191, 17);
            this.checkAddBrackets.TabIndex = 6;
            this.checkAddBrackets.Text = "Add braces to one line conditionals";
            this.checkAddBrackets.UseVisualStyleBackColor = true;
            this.checkAddBrackets.Click += new System.EventHandler(this.check_Click);
            // 
            // checkAttachClasses
            // 
            this.checkAttachClasses.AutoSize = true;
            this.checkAttachClasses.Location = new System.Drawing.Point(6, 33);
            this.checkAttachClasses.Name = "checkAttachClasses";
            this.checkAttachClasses.Size = new System.Drawing.Size(95, 17);
            this.checkAttachClasses.TabIndex = 2;
            this.checkAttachClasses.Tag = "--attach-classes";
            this.checkAttachClasses.Text = "Attach classes";
            this.checkAttachClasses.UseVisualStyleBackColor = true;
            this.checkAttachClasses.Click += new System.EventHandler(this.check_Click);
            // 
            // lblBracketStyle
            // 
            this.lblBracketStyle.AutoSize = true;
            this.lblBracketStyle.Location = new System.Drawing.Point(8, 9);
            this.lblBracketStyle.Name = "lblBracketStyle";
            this.lblBracketStyle.Size = new System.Drawing.Size(62, 13);
            this.lblBracketStyle.TabIndex = 1;
            this.lblBracketStyle.Text = "Brace style:";
            // 
            // cbBracketStyle
            // 
            this.cbBracketStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbBracketStyle.FormattingEnabled = true;
            this.cbBracketStyle.Location = new System.Drawing.Point(85, 6);
            this.cbBracketStyle.Name = "cbBracketStyle";
            this.cbBracketStyle.Size = new System.Drawing.Size(120, 21);
            this.cbBracketStyle.TabIndex = 0;
            this.cbBracketStyle.SelectionChangeCommitted += new System.EventHandler(this.cbBracketStyle_SelectionChangeCommitted);
            // 
            // tabPadding
            // 
            this.tabPadding.Controls.Add(this.checkPadParensOut);
            this.tabPadding.Controls.Add(this.checkPadParensIn);
            this.tabPadding.Controls.Add(this.checkFillEmptyLines);
            this.tabPadding.Controls.Add(this.checkDeleteEmptyLines);
            this.tabPadding.Controls.Add(this.checkPadHeaders);
            this.tabPadding.Controls.Add(this.checkPadCommas);
            this.tabPadding.Controls.Add(this.checkPadAll);
            this.tabPadding.Controls.Add(this.checkPadBlocks);
            this.tabPadding.Location = new System.Drawing.Point(4, 22);
            this.tabPadding.Name = "tabPadding";
            this.tabPadding.Size = new System.Drawing.Size(756, 480);
            this.tabPadding.TabIndex = 2;
            this.tabPadding.Text = "Padding";
            this.tabPadding.UseVisualStyleBackColor = true;
            // 
            // checkPadParensOut
            // 
            this.checkPadParensOut.AutoSize = true;
            this.checkPadParensOut.Location = new System.Drawing.Point(6, 167);
            this.checkPadParensOut.Name = "checkPadParensOut";
            this.checkPadParensOut.Size = new System.Drawing.Size(152, 17);
            this.checkPadParensOut.TabIndex = 24;
            this.checkPadParensOut.Tag = "--pad-paren-out";
            this.checkPadParensOut.Text = "Pad ouside of parentheses";
            this.checkPadParensOut.UseVisualStyleBackColor = true;
            this.checkPadParensOut.Click += new System.EventHandler(this.check_Click);
            // 
            // checkPadParensIn
            // 
            this.checkPadParensIn.AutoSize = true;
            this.checkPadParensIn.Location = new System.Drawing.Point(6, 144);
            this.checkPadParensIn.Name = "checkPadParensIn";
            this.checkPadParensIn.Size = new System.Drawing.Size(148, 17);
            this.checkPadParensIn.TabIndex = 23;
            this.checkPadParensIn.Tag = "--pad-paren-in";
            this.checkPadParensIn.Text = "Pad inside of parentheses";
            this.checkPadParensIn.UseVisualStyleBackColor = true;
            this.checkPadParensIn.Click += new System.EventHandler(this.check_Click);
            // 
            // checkFillEmptyLines
            // 
            this.checkFillEmptyLines.AutoSize = true;
            this.checkFillEmptyLines.Location = new System.Drawing.Point(6, 121);
            this.checkFillEmptyLines.Name = "checkFillEmptyLines";
            this.checkFillEmptyLines.Size = new System.Drawing.Size(93, 17);
            this.checkFillEmptyLines.TabIndex = 22;
            this.checkFillEmptyLines.Tag = "--fill-empty-lines";
            this.checkFillEmptyLines.Text = "Fill empty lines";
            this.checkFillEmptyLines.UseVisualStyleBackColor = true;
            this.checkFillEmptyLines.Click += new System.EventHandler(this.check_Click);
            // 
            // checkDeleteEmptyLines
            // 
            this.checkDeleteEmptyLines.AutoSize = true;
            this.checkDeleteEmptyLines.Location = new System.Drawing.Point(6, 98);
            this.checkDeleteEmptyLines.Name = "checkDeleteEmptyLines";
            this.checkDeleteEmptyLines.Size = new System.Drawing.Size(112, 17);
            this.checkDeleteEmptyLines.TabIndex = 12;
            this.checkDeleteEmptyLines.Tag = "--delete-empty-lines";
            this.checkDeleteEmptyLines.Text = "Delete empty lines";
            this.checkDeleteEmptyLines.UseVisualStyleBackColor = true;
            this.checkDeleteEmptyLines.Click += new System.EventHandler(this.check_Click);
            // 
            // checkPadHeaders
            // 
            this.checkPadHeaders.AutoSize = true;
            this.checkPadHeaders.Location = new System.Drawing.Point(6, 75);
            this.checkPadHeaders.Name = "checkPadHeaders";
            this.checkPadHeaders.Size = new System.Drawing.Size(86, 17);
            this.checkPadHeaders.TabIndex = 21;
            this.checkPadHeaders.Tag = "--pad-header";
            this.checkPadHeaders.Text = "Pad headers";
            this.checkPadHeaders.UseVisualStyleBackColor = true;
            this.checkPadHeaders.Click += new System.EventHandler(this.check_Click);
            // 
            // checkPadCommas
            // 
            this.checkPadCommas.AutoSize = true;
            this.checkPadCommas.Location = new System.Drawing.Point(6, 52);
            this.checkPadCommas.Name = "checkPadCommas";
            this.checkPadCommas.Size = new System.Drawing.Size(87, 17);
            this.checkPadCommas.TabIndex = 20;
            this.checkPadCommas.Tag = "--pad-comma";
            this.checkPadCommas.Text = "Pad commas";
            this.checkPadCommas.UseVisualStyleBackColor = true;
            this.checkPadCommas.Click += new System.EventHandler(this.check_Click);
            // 
            // checkPadAll
            // 
            this.checkPadAll.AutoSize = true;
            this.checkPadAll.Location = new System.Drawing.Point(25, 29);
            this.checkPadAll.Name = "checkPadAll";
            this.checkPadAll.Size = new System.Drawing.Size(92, 17);
            this.checkPadAll.TabIndex = 19;
            this.checkPadAll.Text = "Pad all blocks";
            this.checkPadAll.UseVisualStyleBackColor = true;
            this.checkPadAll.Click += new System.EventHandler(this.check_Click);
            // 
            // checkPadBlocks
            // 
            this.checkPadBlocks.AutoSize = true;
            this.checkPadBlocks.Location = new System.Drawing.Point(6, 6);
            this.checkPadBlocks.Name = "checkPadBlocks";
            this.checkPadBlocks.Size = new System.Drawing.Size(79, 17);
            this.checkPadBlocks.TabIndex = 16;
            this.checkPadBlocks.Text = "Pad blocks";
            this.checkPadBlocks.UseVisualStyleBackColor = true;
            this.checkPadBlocks.Click += new System.EventHandler(this.check_Click);
            // 
            // tabFormatting
            // 
            this.tabFormatting.Controls.Add(this.checkKeepOneLineStatements);
            this.tabFormatting.Controls.Add(this.checkKeepOneLineBlocks);
            this.tabFormatting.Controls.Add(this.checkBreakElseifs);
            this.tabFormatting.Location = new System.Drawing.Point(4, 22);
            this.tabFormatting.Name = "tabFormatting";
            this.tabFormatting.Padding = new System.Windows.Forms.Padding(3);
            this.tabFormatting.Size = new System.Drawing.Size(756, 480);
            this.tabFormatting.TabIndex = 3;
            this.tabFormatting.Text = "Formatting";
            this.tabFormatting.UseVisualStyleBackColor = true;
            // 
            // checkKeepOneLineStatements
            // 
            this.checkKeepOneLineStatements.AutoSize = true;
            this.checkKeepOneLineStatements.Location = new System.Drawing.Point(6, 52);
            this.checkKeepOneLineStatements.Name = "checkKeepOneLineStatements";
            this.checkKeepOneLineStatements.Size = new System.Drawing.Size(145, 17);
            this.checkKeepOneLineStatements.TabIndex = 14;
            this.checkKeepOneLineStatements.Tag = "--keep-one-line-statements";
            this.checkKeepOneLineStatements.Text = "Keep one line statements";
            this.checkKeepOneLineStatements.UseVisualStyleBackColor = true;
            this.checkKeepOneLineStatements.Click += new System.EventHandler(this.check_Click);
            // 
            // checkKeepOneLineBlocks
            // 
            this.checkKeepOneLineBlocks.AutoSize = true;
            this.checkKeepOneLineBlocks.Location = new System.Drawing.Point(6, 29);
            this.checkKeepOneLineBlocks.Name = "checkKeepOneLineBlocks";
            this.checkKeepOneLineBlocks.Size = new System.Drawing.Size(125, 17);
            this.checkKeepOneLineBlocks.TabIndex = 13;
            this.checkKeepOneLineBlocks.Tag = "--keep-one-line-blocks";
            this.checkKeepOneLineBlocks.Text = "Keep one line blocks";
            this.checkKeepOneLineBlocks.UseVisualStyleBackColor = true;
            this.checkKeepOneLineBlocks.Click += new System.EventHandler(this.check_Click);
            // 
            // checkBreakElseifs
            // 
            this.checkBreakElseifs.AutoSize = true;
            this.checkBreakElseifs.Location = new System.Drawing.Point(6, 6);
            this.checkBreakElseifs.Name = "checkBreakElseifs";
            this.checkBreakElseifs.Size = new System.Drawing.Size(89, 17);
            this.checkBreakElseifs.TabIndex = 12;
            this.checkBreakElseifs.Tag = "--break-elseifs";
            this.checkBreakElseifs.Text = "Break else ifs";
            this.checkBreakElseifs.UseVisualStyleBackColor = true;
            this.checkBreakElseifs.Click += new System.EventHandler(this.check_Click);
            // 
            // pnlSci
            // 
            this.pnlSci.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlSci.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlSci.Location = new System.Drawing.Point(292, 28);
            this.pnlSci.Name = "pnlSci";
            this.pnlSci.Size = new System.Drawing.Size(458, 468);
            this.pnlSci.TabIndex = 11;
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSave.Location = new System.Drawing.Point(6, 475);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 12;
            this.btnSave.Text = "&OK";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCancel.Location = new System.Drawing.Point(87, 475);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 12;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // checkCurrentFile
            // 
            this.checkCurrentFile.AutoSize = true;
            this.checkCurrentFile.BackColor = System.Drawing.Color.White;
            this.checkCurrentFile.Location = new System.Drawing.Point(167, 479);
            this.checkCurrentFile.Name = "checkCurrentFile";
            this.checkCurrentFile.Size = new System.Drawing.Size(105, 17);
            this.checkCurrentFile.TabIndex = 13;
            this.checkCurrentFile.Text = "Show current file";
            this.checkCurrentFile.UseVisualStyleBackColor = false;
            this.checkCurrentFile.CheckedChanged += new System.EventHandler(this.checkExampleFile_CheckedChanged);
            // 
            // HaxeAStyleDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(764, 506);
            this.Controls.Add(this.pnlSci);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.checkCurrentFile);
            this.Controls.Add(this.tabControl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(346, 306);
            this.Name = "HaxeAStyleDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Haxe Formatter Settings";
            this.tabControl.ResumeLayout(false);
            this.tabIndents.ResumeLayout(false);
            this.tabIndents.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numIndentWidth)).EndInit();
            this.tabBrackets.ResumeLayout(false);
            this.tabBrackets.PerformLayout();
            this.tabPadding.ResumeLayout(false);
            this.tabPadding.PerformLayout();
            this.tabFormatting.ResumeLayout(false);
            this.tabFormatting.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public HaxeAStyleDialog(HaxeAStyleOptions options)
        {
            InitializeComponent();
            InitializeLocalization();

            var currentDoc = PluginBase.MainForm.CurrentDocument;
            if (string.IsNullOrEmpty(currentDoc?.SciControl?.Text) || currentDoc.SciControl.ConfigurationLanguage != "haxe")
            {
                checkCurrentFile.Enabled = false;
            }

            //Read example file
            var id = "CodeFormatter.Resources.AStyleExample.hx";
            Assembly assembly = Assembly.GetExecutingAssembly();
            using (var reader = new StreamReader(assembly.GetManifestResourceStream(id)))
            {
                exampleCode = reader.ReadToEnd();
            }

            this.Font = PluginBase.Settings.DefaultFont;

            //Create Scintilla
            txtExample = new ScintillaControl
            {
                Dock = DockStyle.Fill,
                ConfigurationLanguage = "haxe",
                ViewWhitespace = ScintillaNet.Enums.WhiteSpace.VisibleAlways,
                Lexer = 3
            };
            txtExample.SetProperty("lexer.cpp.track.preprocessor", "0");

            this.pnlSci.Controls.Add(txtExample);

            checkForceTabs.DataBindings.Add("Enabled", checkTabs, "Checked");
            checkPadAll.DataBindings.Add("Enabled", checkPadBlocks, "Checked");

            checkOneLineBrackets.DataBindings.Add("Enabled", checkAddBrackets, "Checked");

            foreach (TabPage page in this.tabControl.TabPages)
            {
                MapCheckBoxes(page);
            }

            cbBracketStyle.DataSource = new[]
            {
                "Allman", "Java", "Kernighan & Ritchie", "Stroustrup", "Whitesmith", "VTK", "Banner", "GNU", "Linux",
                "Horstmann", "One True Brace", "Google", /*"Mozilla",*/ "Pico", "Lisp"
            }; //Mozilla not supported by old version of AStyle

            SetOptions(options);

            ValidateControls();
            ReformatExample();
        }

        #region Localization

        private void InitializeLocalization()
        {
            this.Text = TextHelper.GetString("Title.AStyleFormatterSettings");
            this.btnSave.Text = TextHelper.GetString("FlashDevelop.Label.Save");
            this.btnCancel.Text = TextHelper.GetString("FlashDevelop.Label.Cancel");
            this.checkCurrentFile.Text = TextHelper.GetString("Label.ShowCurrentFile");
            this.checkAddBrackets.Text = TextHelper.GetString("Label.AddBrackets");
            this.checkAttachClasses.Text = TextHelper.GetString("Label.AttachClasses");
            this.checkBreakClosing.Text = TextHelper.GetString("Label.BreakClosing");
            this.checkBreakElseifs.Text = TextHelper.GetString("Label.BreakElseifs");
            this.checkDeleteEmptyLines.Text = TextHelper.GetString("Label.DeleteEmptyLines");
            this.checkFillEmptyLines.Text = TextHelper.GetString("Label.FillEmptyLines");
            this.checkForceTabs.Text = TextHelper.GetString("Label.ForceTabs");
            this.checkIndentCase.Text = TextHelper.GetString("Label.IndentCases");
            this.checkIndentConditional.Text = TextHelper.GetString("Label.IndentConditionals");
            this.checkIndentSwitches.Text = TextHelper.GetString("Label.IndentSwitches");
            this.checkKeepOneLineBlocks.Text = TextHelper.GetString("Label.KeepOneLineBlocks");
            this.checkKeepOneLineStatements.Text = TextHelper.GetString("Label.KeepOneLineStatements");
            this.checkPadCommas.Text = TextHelper.GetString("Label.PadCommas");
            this.checkOneLineBrackets.Text = TextHelper.GetString("Label.AddOneLineBrackets");
            this.checkPadAll.Text = TextHelper.GetString("Label.PadAllBlocks");
            this.checkPadBlocks.Text = TextHelper.GetString("Label.PadBlocks");
            this.checkPadHeaders.Text = TextHelper.GetString("Label.PadHeaders");
            this.checkRemoveBrackets.Text = TextHelper.GetString("Label.RemoveBracketsFromContitionals");
            this.checkTabs.Text = TextHelper.GetString("Label.UseTabs");
            this.checkPadParensIn.Text = TextHelper.GetString("Label.PadParensIn");
            this.checkPadParensOut.Text = TextHelper.GetString("Label.PadParensOut");
            this.lblBracketStyle.Text = TextHelper.GetString("Label.BracketStyle");
            this.lblIndentSize.Text = TextHelper.GetString("Label.IndentSize");
            this.tabBrackets.Text = TextHelper.GetString("Info.Brackets");
            this.tabFormatting.Text = TextHelper.GetString("Info.Formatting");
            this.tabIndents.Text = TextHelper.GetString("Info.Indentation");
            this.tabPadding.Text = TextHelper.GetString("Info.Padding");
        }

        #endregion

        /// <summary>
        /// Fills <see cref="mapping"/>.
        /// It checks the given <paramref name="page"/> for checkboxes that have a Tag.
        /// The tab is assumed to be the flag that should be set.
        /// Checkboxes without Tag are ignored by this method and have to be handled manually.
        /// </summary>
        private void MapCheckBoxes(TabPage page)
        {
            foreach (Control c in page.Controls)
            {
                CheckBox check = c as CheckBox;
                if (check?.Tag != null)
                {
                    //Tag is used to assign simple flags to the corresponding control
                    //More complex options are handled in SetOptions and GetOptions
                    mapping.Add(check, (string) check.Tag);
                }
            }
        }

        /// <summary>
        /// Helper method used by <see cref="GetOptions"/> to automatically read values of simple flags from the
        /// respective checkboxes into the given <paramref name="options"/>.
        /// </summary>
        /// <param name="page">The TabPage to set options for</param>
        private void GetBasicTabOptions(List<HaxeAStyleOption> options, TabPage page)
        {
            foreach (Control c in page.Controls)
            {
                CheckBox check = c as CheckBox;
                if (check != null && IsChecked(check) && mapping.ContainsKey(check))
                {
                    options.Add(new HaxeAStyleOption(mapping[check]));
                }
            }
        }

        /// <summary>
        /// Helper method used by <see cref="SetOptions"/> to automatically set simple flag checkboxes.
        /// </summary>
        /// <param name="page">The TabPage to search through</param>
        private void SetBasicTabOptions(HaxeAStyleOptions options, TabPage page)
        {
            foreach (Control c in page.Controls)
            {
                CheckBox check = c as CheckBox;

                bool hasOption = check != null && mapping.ContainsKey(check) && options.Exists(mapping[check]);

                if (hasOption)
                {
                    check.Checked = true;
                }
            }
        }

        /// <summary>
        /// Helper method to fill this dialog from the given <paramref name="options"/>.
        /// </summary>
        private void SetOptions(HaxeAStyleOptions options)
        {
            if (options.Count == 0)
            {
                return;
            }

            //set default switches
            foreach (TabPage page in this.tabControl.TabPages)
            {
                SetBasicTabOptions(options, page);
            }

            //Brackets
            cbBracketStyle.SelectedItem = HaxeAStyleHelper.GetNameFromBraceStyle((string)options.Find("--style").Value);

            checkOneLineBrackets.Checked = options.Exists("--add-one-line-brackets");
            checkAddBrackets.Checked = checkOneLineBrackets.Checked || options.Exists("--add-brackets");

            //Padding
            HaxeAStyleOption breakBlocks = options.Find("--break-blocks");

            if (breakBlocks != null)
            {
                checkPadBlocks.Checked = true;
                checkPadAll.Checked = "all".Equals(breakBlocks.Value);
            }

            //Tabs / Indentation
            HaxeAStyleOption forceTabs = options.Find("--indent=force-tab");
            HaxeAStyleOption useTabs = options.Find("--indent=tab");
            HaxeAStyleOption useSpaces = options.Find("--indent=spaces");

            if (forceTabs != null)
            {
                checkTabs.Checked = true;
                checkForceTabs.Checked = true;
                numIndentWidth.Value = Convert.ToDecimal(forceTabs.Value);
            }
            else if (useTabs != null)
            {
                checkTabs.Checked = true;
                //numIndentWidth.Enabled = false;
                numIndentWidth.Value = Convert.ToDecimal(useTabs.Value);
            }
            else
            {
                checkTabs.Checked = false;
                numIndentWidth.Value = Convert.ToDecimal(useSpaces.Value);
            }
        }

        /// <summary>
        /// Helper method to create <see cref="HaxeAStyleOptions" /> from the currently selected
        /// options.
        /// </summary>
        /// <returns>An object of type <see cref="HaxeAStyleOptions" />, which is a list of <see cref="HaxeAStyleOption"/></returns>
        internal HaxeAStyleOptions GetOptions()
        {
            HaxeAStyleOptions options = new HaxeAStyleOptions();

            //HaxeAStyleHelper.AddDefaultOptions(options);

            //handling default switches
            foreach (TabPage page in this.tabControl.TabPages)
            {
                GetBasicTabOptions(options, page);
            }

            //special options

            //Tabs / Indentation
            if (IsChecked(checkForceTabs))
            {
                options.Add(new HaxeAStyleOption("--indent=force-tab", numIndentWidth.Value));
            }
            else if (IsChecked(checkTabs))
            {
                options.Add(new HaxeAStyleOption("--indent=tab", numIndentWidth.Value));
            }
            else
            {
                options.Add(new HaxeAStyleOption("--indent=spaces", numIndentWidth.Value));
            }

            //Brackets
            options.Add(new HaxeAStyleOption("--style",
                HaxeAStyleHelper.GetBraceStyleFromName((string) cbBracketStyle.SelectedItem)));
            
            if (IsChecked(checkAddBrackets))
            {
                if (IsChecked(checkOneLineBrackets))
                    options.Add(new HaxeAStyleOption("--add-one-line-brackets"));
                else
                    options.Add(new HaxeAStyleOption("--add-brackets"));
            }

            //Padding
            if (IsChecked(checkPadBlocks))
            {
                if (IsChecked(checkPadAll))
                    options.Add(new HaxeAStyleOption("--break-blocks", "all"));
                else
                    options.Add(new HaxeAStyleOption("--break-blocks"));
            }
            //options.Add("--indent-continuation=" + numIndentContinuation.Value); //not supported by old version of AStyle

            return options;
        }

        /// <summary>
        /// Helper method to apply the selected AStyle settings to the text
        /// </summary>
        private void ReformatExample()
        {
            AStyleInterface astyle = new AStyleInterface();
            string[] options = GetOptions().ToStringArray();

            var firstLine = txtExample.FirstVisibleLine;

            txtExample.IsReadOnly = false;
            txtExample.Text = exampleCode;

            var currentDoc = PluginBase.MainForm.CurrentDocument;
            if (checkCurrentFile.Checked && currentDoc?.SciControl != null)
            {
                txtExample.Text = currentDoc.SciControl.Text;
            }
            //txtExample.TabWidth = (int) numIndentWidth.Value;
            txtExample.TabWidth = PluginBase.Settings.TabWidth;
            txtExample.IsFocus = true;
            txtExample.Text = astyle.FormatSource(txtExample.Text, string.Join(" ", options));
            txtExample.IsReadOnly = true;

            txtExample.FirstVisibleLine = firstLine;
        }

        /// <summary>
        /// Helper method to determine if <paramref name="chk"/> is enabled and checked.
        /// </summary>
        private static bool IsChecked(CheckBox chk)
        {
            return chk.Checked && chk.Enabled;
        }

        /// <summary>
        /// Checks for incompatible selections and fixes them.
        /// For example, it makes no sense to delete and fill empty lines at the same time.
        /// </summary>
        /// <param name="sender">An optional argument to determin what control triggers the validation. Needed for some checks</param>
        private void ValidateControls(object sender = null)
        {
            //Bracket style
            string style = (string)cbBracketStyle.SelectedValue;

            switch (style)
            {
                case "Java":
                case "Kernighan & Ritchie":
                case "Stroustrup":
                case "Linux":
                case "One True Brace":
                    checkBreakClosing.Enabled = true;
                    break;
                default: //style enables this by default
                    checkBreakClosing.Enabled = false;
                    break;
            }
            switch (style)
            {
                case "Java":
                case "Stroustrup":
                case "Banner":
                case "Google":
                case "Lisp": //style enables this by default
                    checkAttachClasses.Enabled = false;
                    break;
                default:
                    checkAttachClasses.Enabled = true;
                    break;
            }
            checkRemoveBrackets.Enabled = style != "One True Brace";

            //Checkboxes
            checkKeepOneLineBlocks.Enabled = !checkOneLineBrackets.Checked;
            //These exclude each other:
            if (sender == checkFillEmptyLines && checkFillEmptyLines.Checked)
            {
                checkDeleteEmptyLines.Checked = false;
            }
            else if (checkDeleteEmptyLines.Checked)
            {
                checkFillEmptyLines.Checked = false;
            }

            if (sender == checkAddBrackets && checkAddBrackets.Checked)
            {
                checkRemoveBrackets.Checked = false;
            }
            else if (checkRemoveBrackets.Checked)
            {
                checkAddBrackets.Checked = false;
            }
            //
        }

        private void check_Click(object sender, EventArgs e)
        {
            ValidateControls(sender);

            ReformatExample();
        }

        private void numIndentWidth_ValueChanged(object sender, EventArgs e)
        {
            ReformatExample();
        }

        private void cbBracketStyle_SelectionChangeCommitted(object sender, EventArgs e)
        {
            ValidateControls();

            ReformatExample();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        public void CloseDropDown()
        {
            //throw new NotImplementedException();
        }

        public void DropDownControl(Control control)
        {
            //throw new NotImplementedException();
        }

        public DialogResult ShowDialog(Form dialog)
        {
            return dialog.ShowDialog(this);
        }

        private void checkExampleFile_CheckedChanged(object sender, EventArgs e)
        {
            ReformatExample();
        }
    }
}
