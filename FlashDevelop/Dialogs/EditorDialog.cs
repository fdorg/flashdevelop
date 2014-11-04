using System;
using System.IO;
using System.Xml;
using System.Data;
using System.Text;
using System.Drawing;
using System.Xml.XPath;
using System.Drawing.Text;
using System.Windows.Forms;
using System.ComponentModel;
using ICSharpCode.SharpZipLib.Zip;
using System.Collections.Generic;
using FlashDevelop.Utilities;
using FlashDevelop.Managers;
using PluginCore.Localization;
using PluginCore.Managers;
using PluginCore.Helpers;
using PluginCore;

namespace FlashDevelop.Dialogs
{
    public class EditorDialog : Form
    {
        private String languageFile;
        private XmlDocument languageDoc;
        private XmlElement editorStyleNode;
        private XmlElement defaultStyleNode;
        private XmlElement currentStyleNode;
        private Boolean isItemSaved = true;
        private Boolean isEditorSaved = true;
        private Boolean isLoadingEditor = false;
        private Boolean isLanguageSaved = true;
        private Boolean isLoadingItem = false;
        private System.Windows.Forms.Label sizeLabel;
        private System.Windows.Forms.Label fontLabel;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button revertButton;
        private System.Windows.Forms.Button defaultButton;
        private System.Windows.Forms.Button applyButton;
        private System.Windows.Forms.Button exportButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.ListView itemListView;
        private System.Windows.Forms.ColorDialog colorDialog;
        private System.Windows.Forms.GroupBox itemGroupBox;
        private System.Windows.Forms.GroupBox languageGroupBox;
        private System.Windows.Forms.Label markerForeLabel;
        private System.Windows.Forms.Label markerBackLabel;
        private System.Windows.Forms.Label marginForeLabel;
        private System.Windows.Forms.Label marginBackLabel;
        private System.Windows.Forms.Button markerForeButton;
        private System.Windows.Forms.Button markerBackButton;
        private System.Windows.Forms.Button marginForeButton;
        private System.Windows.Forms.Button marginBackButton;
        private System.Windows.Forms.TextBox markerForeTextBox;
        private System.Windows.Forms.TextBox markerBackTextBox;
        private System.Windows.Forms.TextBox marginForeTextBox;
        private System.Windows.Forms.TextBox marginBackTextBox;
        private System.Windows.Forms.Label caretForeLabel;
        private System.Windows.Forms.Label caretlineBackLabel;
        private System.Windows.Forms.Label selectionForeLabel;
        private System.Windows.Forms.Label selectionBackLabel;
        private System.Windows.Forms.Button caretForeButton;
        private System.Windows.Forms.Button caretlineBackButton;
        private System.Windows.Forms.Button selectionForeButton;
        private System.Windows.Forms.Button selectionBackButton;
        private System.Windows.Forms.TextBox caretForeTextBox;
        private System.Windows.Forms.TextBox caretlineBackTextBox;
        private System.Windows.Forms.TextBox selectionForeTextBox;
        private System.Windows.Forms.TextBox selectionBackTextBox;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.Label sampleTextLabel;
        private System.Windows.Forms.ComboBox fontSizeComboBox;
        private System.Windows.Forms.ComboBox fontNameComboBox;
        private System.Windows.Forms.TextBox foregroundTextBox;
        private System.Windows.Forms.CheckBox boldCheckBox;
        private System.Windows.Forms.CheckBox colorizeCheckBox;
        private System.Windows.Forms.CheckBox italicsCheckBox;
        private System.Windows.Forms.TextBox backgroundTextBox;
        private System.Windows.Forms.Button backgroundButton;
        private System.Windows.Forms.Button foregroundButton;
        private System.Windows.Forms.ComboBox languageDropDown;
        private System.Windows.Forms.ColumnHeader columnHeader;
        private System.Windows.Forms.Label backgroundLabel;
        private System.Windows.Forms.Label foregroundLabel;
        private System.Windows.Forms.Label printMarginLabel;
        private System.Windows.Forms.Label bookmarkLineLabel;
        private System.Windows.Forms.Label modifiedLineLabel;
        private System.Windows.Forms.Label highlightBackLabel;
        private System.Windows.Forms.Button printMarginButton;
        private System.Windows.Forms.Button bookmarkLineButton;
        private System.Windows.Forms.Button modifiedLineButton;
        private System.Windows.Forms.Button highlightBackButton;
        private System.Windows.Forms.TextBox printMarginTextBox;
        private System.Windows.Forms.TextBox bookmarkLineTextBox;
        private System.Windows.Forms.TextBox modifiedLineTextBox;
        private System.Windows.Forms.TextBox highlightBackTextBox;
        private System.Windows.Forms.Button debugLineButton;
        private System.Windows.Forms.Button errorLineButton;
        private System.Windows.Forms.Button disabledLineButton;
        private System.Windows.Forms.TextBox disabledLineTextBox;
        private System.Windows.Forms.TextBox debugLineTextBox;
        private System.Windows.Forms.TextBox errorLineTextBox;
        private System.Windows.Forms.Label disabledLineLabel;
        private System.Windows.Forms.Label debugLineLabel;
        private System.Windows.Forms.Label errorLineLabel;

        public EditorDialog()
        {
            this.Owner = Globals.MainForm;
            this.Font = Globals.Settings.DefaultFont;
            this.InitializeComponent();
            this.ApplyLocalizedTexts();
            this.InitializeGraphics();
            this.PopulateControls();
            ScaleHelper.AdjustForHighDPI(this);
        }

        #region Windows Form Designer Generated Code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.okButton = new System.Windows.Forms.Button();
            this.applyButton = new System.Windows.Forms.Button();
            this.defaultButton = new System.Windows.Forms.Button();
            this.revertButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.itemListView = new System.Windows.Forms.ListView();
            this.columnHeader = new System.Windows.Forms.ColumnHeader();
            this.colorDialog = new System.Windows.Forms.ColorDialog();
            this.itemGroupBox = new System.Windows.Forms.GroupBox();
            this.sampleTextLabel = new System.Windows.Forms.Label();
            this.italicsCheckBox = new System.Windows.Forms.CheckBox();
            this.colorizeCheckBox = new System.Windows.Forms.CheckBox();
            this.backgroundButton = new System.Windows.Forms.Button();
            this.foregroundButton = new System.Windows.Forms.Button();
            this.boldCheckBox = new System.Windows.Forms.CheckBox();
            this.backgroundTextBox = new System.Windows.Forms.TextBox();
            this.foregroundTextBox = new System.Windows.Forms.TextBox();
            this.fontSizeComboBox = new System.Windows.Forms.ComboBox();
            this.fontNameComboBox = new System.Windows.Forms.ComboBox();
            this.sizeLabel = new System.Windows.Forms.Label();
            this.backgroundLabel = new System.Windows.Forms.Label();
            this.foregroundLabel = new System.Windows.Forms.Label();
            this.fontLabel = new System.Windows.Forms.Label();
            this.caretForeButton = new System.Windows.Forms.Button();
            this.caretlineBackButton = new System.Windows.Forms.Button();
            this.selectionBackButton = new System.Windows.Forms.Button();
            this.selectionForeButton = new System.Windows.Forms.Button();
            this.caretForeTextBox = new System.Windows.Forms.TextBox();
            this.caretlineBackTextBox = new System.Windows.Forms.TextBox();
            this.selectionForeTextBox = new System.Windows.Forms.TextBox();
            this.selectionBackTextBox = new System.Windows.Forms.TextBox();
            this.marginForeButton = new System.Windows.Forms.Button();
            this.marginBackButton = new System.Windows.Forms.Button();
            this.markerBackButton = new System.Windows.Forms.Button();
            this.markerForeButton = new System.Windows.Forms.Button();
            this.marginForeTextBox = new System.Windows.Forms.TextBox();
            this.marginBackTextBox = new System.Windows.Forms.TextBox();
            this.markerForeTextBox = new System.Windows.Forms.TextBox();
            this.markerBackTextBox = new System.Windows.Forms.TextBox();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.languageGroupBox = new System.Windows.Forms.GroupBox();
            this.caretForeLabel = new System.Windows.Forms.Label();
            this.caretlineBackLabel = new System.Windows.Forms.Label();
            this.selectionBackLabel = new System.Windows.Forms.Label();
            this.selectionForeLabel = new System.Windows.Forms.Label();
            this.marginForeLabel = new System.Windows.Forms.Label();
            this.marginBackLabel = new System.Windows.Forms.Label();
            this.markerBackLabel = new System.Windows.Forms.Label();
            this.markerForeLabel = new System.Windows.Forms.Label();
            this.printMarginButton = new System.Windows.Forms.Button();
            this.printMarginLabel = new System.Windows.Forms.Label();
            this.printMarginTextBox = new System.Windows.Forms.TextBox();
            this.highlightBackButton = new System.Windows.Forms.Button();
            this.highlightBackLabel = new System.Windows.Forms.Label();
            this.highlightBackTextBox = new System.Windows.Forms.TextBox();
            this.modifiedLineButton = new System.Windows.Forms.Button();
            this.modifiedLineLabel = new System.Windows.Forms.Label();
            this.modifiedLineTextBox = new System.Windows.Forms.TextBox();
            this.bookmarkLineButton = new System.Windows.Forms.Button();
            this.bookmarkLineLabel = new System.Windows.Forms.Label();
            this.bookmarkLineTextBox = new System.Windows.Forms.TextBox();
            this.errorLineButton = new System.Windows.Forms.Button();
            this.errorLineLabel = new System.Windows.Forms.Label();
            this.errorLineTextBox = new System.Windows.Forms.TextBox();
            this.debugLineButton = new System.Windows.Forms.Button();
            this.debugLineLabel = new System.Windows.Forms.Label();
            this.debugLineTextBox = new System.Windows.Forms.TextBox();
            this.disabledLineButton = new System.Windows.Forms.Button();
            this.disabledLineLabel = new System.Windows.Forms.Label();
            this.disabledLineTextBox = new System.Windows.Forms.TextBox();
            this.languageDropDown = new System.Windows.Forms.ComboBox();
            this.exportButton = new System.Windows.Forms.Button();
            this.itemGroupBox.SuspendLayout();
            this.languageGroupBox.SuspendLayout();
            this.SuspendLayout();
            //
            // colorDialog
            //
            this.colorDialog.FullOpen = true;
            // 
            // okButton
            // 
            this.okButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.okButton.Location = new System.Drawing.Point(431, 534);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(93, 29);
            this.okButton.TabIndex = 1;
            this.okButton.Text = "&OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.OkButtonClick);
            // 
            // applyButton
            // 
            this.applyButton.Enabled = false;
            this.applyButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.applyButton.Location = new System.Drawing.Point(640, 534);
            this.applyButton.Name = "applyButton";
            this.applyButton.Size = new System.Drawing.Size(93, 29);
            this.applyButton.TabIndex = 3;
            this.applyButton.Text = "&Apply";
            this.applyButton.UseVisualStyleBackColor = true;
            this.applyButton.Click += new System.EventHandler(this.SaveButtonClick);
            // 
            // exportButton
            // 
            this.exportButton.Location = new System.Drawing.Point(238, 534);
            this.exportButton.Name = "exportButton";
            this.exportButton.Size = new System.Drawing.Size(35, 29);
            this.exportButton.TabIndex = 8;
            this.exportButton.Click += new System.EventHandler(this.ExportLanguagesClick);
            // 
            // revertButton
            // 
            this.revertButton.Location = new System.Drawing.Point(285, 534);
            this.revertButton.Name = "revertButton";
            this.revertButton.Size = new System.Drawing.Size(35, 29);
            this.revertButton.TabIndex = 9;
            this.revertButton.Click += new System.EventHandler(this.RevertLanguagesClick);
            // 
            // defaultButton
            // 
            this.defaultButton.Location = new System.Drawing.Point(332, 534);
            this.defaultButton.Name = "defaultButton";
            this.defaultButton.Size = new System.Drawing.Size(35, 29);
            this.defaultButton.TabIndex = 10;
            this.defaultButton.Click += new System.EventHandler(this.MakeAsDefaultStyleClick);
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cancelButton.Location = new System.Drawing.Point(536, 534);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(93, 29);
            this.cancelButton.TabIndex = 2;
            this.cancelButton.Text = "&Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.CancelButtonClick);
            // 
            // itemListView
            // 
            this.itemListView.Alignment = System.Windows.Forms.ListViewAlignment.Left;
            this.itemListView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left)));
            this.itemListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {this.columnHeader});
            this.itemListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.itemListView.HideSelection = false;
            this.itemListView.Location = new System.Drawing.Point(14, 49);
            this.itemListView.MultiSelect = false;
            this.itemListView.Name = "itemListView";
            this.itemListView.Size = new System.Drawing.Size(212, 513);
            this.itemListView.TabIndex = 5;
            this.itemListView.UseCompatibleStateImageBehavior = false;
            this.itemListView.View = System.Windows.Forms.View.Details;
            this.itemListView.SelectedIndexChanged += new System.EventHandler(this.ItemsSelectedIndexChanged);
            // 
            // itemGroupBox
            // 
            this.itemGroupBox.Controls.Add(this.sampleTextLabel);
            this.itemGroupBox.Controls.Add(this.italicsCheckBox);
            this.itemGroupBox.Controls.Add(this.backgroundButton);
            this.itemGroupBox.Controls.Add(this.foregroundButton);
            this.itemGroupBox.Controls.Add(this.boldCheckBox);
            this.itemGroupBox.Controls.Add(this.backgroundTextBox);
            this.itemGroupBox.Controls.Add(this.foregroundTextBox);
            this.itemGroupBox.Controls.Add(this.fontSizeComboBox);
            this.itemGroupBox.Controls.Add(this.fontNameComboBox);
            this.itemGroupBox.Controls.Add(this.sizeLabel);
            this.itemGroupBox.Controls.Add(this.backgroundLabel);
            this.itemGroupBox.Controls.Add(this.foregroundLabel);
            this.itemGroupBox.Controls.Add(this.fontLabel);
            this.itemGroupBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.itemGroupBox.Location = new System.Drawing.Point(238, 319);
            this.itemGroupBox.Name = "itemGroupBox";
            this.itemGroupBox.Size = new System.Drawing.Size(494, 204);
            this.itemGroupBox.TabIndex = 7;
            this.itemGroupBox.TabStop = false;
            this.itemGroupBox.Text = "Item Style";
            // 
            // sampleTextLabel
            // 
            this.sampleTextLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left)| System.Windows.Forms.AnchorStyles.Right)));
            this.sampleTextLabel.BackColor = System.Drawing.Color.White;
            this.sampleTextLabel.Location = new System.Drawing.Point(15, 130);
            this.sampleTextLabel.Name = "sampleTextLabel";
            this.sampleTextLabel.Size = new System.Drawing.Size(465, 60);
            this.sampleTextLabel.TabIndex = 13;
            this.sampleTextLabel.Text = "Sample Text";
            this.sampleTextLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // italicsCheckBox
            // 
            this.italicsCheckBox.AutoSize = true;
            this.italicsCheckBox.Checked = true;
            this.italicsCheckBox.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            this.italicsCheckBox.Location = new System.Drawing.Point(327, 101);
            this.italicsCheckBox.Name = "italicsCheckBox";
            this.italicsCheckBox.Size = new System.Drawing.Size(58, 20);
            this.italicsCheckBox.TabIndex = 12;
            this.italicsCheckBox.Text = "Italics";
            this.italicsCheckBox.ThreeState = true;
            this.italicsCheckBox.UseVisualStyleBackColor = true;
            this.italicsCheckBox.CheckStateChanged += new System.EventHandler(this.LanguageItemChanged);
            // 
            // backgroundButton
            // 
            this.backgroundButton.Location = new System.Drawing.Point(267, 90);
            this.backgroundButton.Name = "backgroundButton";
            this.backgroundButton.Size = new System.Drawing.Size(33, 30);
            this.backgroundButton.TabIndex = 10;
            this.backgroundButton.UseVisualStyleBackColor = true;
            this.backgroundButton.Click += new System.EventHandler(this.ItemBackgroundButtonClick);
            // 
            // foregroundButton
            // 
            this.foregroundButton.Location = new System.Drawing.Point(117, 90);
            this.foregroundButton.Name = "foregroundButton";
            this.foregroundButton.Size = new System.Drawing.Size(33, 30);
            this.foregroundButton.TabIndex = 7;
            this.foregroundButton.UseVisualStyleBackColor = true;
            this.foregroundButton.Click += new System.EventHandler(this.ItemForegroundButtonClick);
            // 
            // boldCheckBox
            // 
            this.boldCheckBox.AutoSize = true;
            this.boldCheckBox.Checked = true;
            this.boldCheckBox.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            this.boldCheckBox.Location = new System.Drawing.Point(327, 79);
            this.boldCheckBox.Name = "boldCheckBox";
            this.boldCheckBox.Size = new System.Drawing.Size(50, 20);
            this.boldCheckBox.TabIndex = 11;
            this.boldCheckBox.Text = "Bold";
            this.boldCheckBox.ThreeState = true;
            this.boldCheckBox.UseVisualStyleBackColor = true;
            this.boldCheckBox.CheckStateChanged += new System.EventHandler(this.LanguageItemChanged);
            // 
            // backgroundTextBox
            // 
            this.backgroundTextBox.Location = new System.Drawing.Point(164, 94);
            this.backgroundTextBox.Name = "backgroundTextBox";
            this.backgroundTextBox.Size = new System.Drawing.Size(94, 23);
            this.backgroundTextBox.TabIndex = 9;
            this.backgroundTextBox.TextChanged += new System.EventHandler(this.LanguageItemChanged);
            // 
            // foregroundTextBox
            // 
            this.foregroundTextBox.Location = new System.Drawing.Point(14, 94);
            this.foregroundTextBox.Name = "foregroundTextBox";
            this.foregroundTextBox.Size = new System.Drawing.Size(94, 23);
            this.foregroundTextBox.TabIndex = 6;
            this.foregroundTextBox.TextChanged += new System.EventHandler(this.LanguageItemChanged);
            // 
            // fontSizeComboBox
            // 
            this.fontSizeComboBox.FormattingEnabled = true;
            this.fontSizeComboBox.Items.AddRange(new object[] {"","6","7","8","9","10","11","12","13","14","15","16","17","18","19","20","21","22","23","24"});
            this.fontSizeComboBox.Location = new System.Drawing.Point(324, 42);
            this.fontSizeComboBox.Name = "fontSizeComboBox";
            this.fontSizeComboBox.Size = new System.Drawing.Size(157, 24);
            this.fontSizeComboBox.TabIndex = 4;
            this.fontSizeComboBox.TextChanged += new System.EventHandler(this.LanguageItemChanged);
            // 
            // fontNameComboBox
            // 
            this.fontNameComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.fontNameComboBox.FormattingEnabled = true;
            this.fontNameComboBox.Location = new System.Drawing.Point(14, 42);
            this.fontNameComboBox.Name = "fontNameComboBox";
            this.fontNameComboBox.Size = new System.Drawing.Size(285, 24);
            this.fontNameComboBox.TabIndex = 2;
            this.fontNameComboBox.SelectedIndexChanged += new System.EventHandler(this.LanguageItemChanged);
            // 
            // sizeLabel
            // 
            this.sizeLabel.AutoSize = true;
            this.sizeLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.sizeLabel.Location = new System.Drawing.Point(324, 21);
            this.sizeLabel.Name = "sizeLabel";
            this.sizeLabel.Size = new System.Drawing.Size(32, 16);
            this.sizeLabel.TabIndex = 3;
            this.sizeLabel.Text = "Size:";
            // 
            // backgroundLabel
            // 
            this.backgroundLabel.AutoSize = true;
            this.backgroundLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.backgroundLabel.Location = new System.Drawing.Point(164, 74);
            this.backgroundLabel.Name = "backgroundLabel";
            this.backgroundLabel.Size = new System.Drawing.Size(75, 16);
            this.backgroundLabel.TabIndex = 8;
            this.backgroundLabel.Text = "Background:";
            // 
            // foregroundLabel
            // 
            this.foregroundLabel.AutoSize = true;
            this.foregroundLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.foregroundLabel.Location = new System.Drawing.Point(14, 74);
            this.foregroundLabel.Name = "foregroundLabel";
            this.foregroundLabel.Size = new System.Drawing.Size(75, 16);
            this.foregroundLabel.TabIndex = 5;
            this.foregroundLabel.Text = "Foreground:";
            // 
            // fontLabel
            // 
            this.fontLabel.AutoSize = true;
            this.fontLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.fontLabel.Location = new System.Drawing.Point(14, 21);
            this.fontLabel.Name = "fontLabel";
            this.fontLabel.Size = new System.Drawing.Size(35, 16);
            this.fontLabel.TabIndex = 1;
            this.fontLabel.Text = "Font:";
            // 
            // caretForeButton
            // 
            this.caretForeButton.Location = new System.Drawing.Point(126, 37);
            this.caretForeButton.Name = "caretForeButton";
            this.caretForeButton.Size = new System.Drawing.Size(33, 30);
            this.caretForeButton.TabIndex = 2;
            this.caretForeButton.Click += new System.EventHandler(this.CaretForeButtonClick);
            // 
            // caretlineBackButton
            // 
            this.caretlineBackButton.Location = new System.Drawing.Point(126, 86);
            this.caretlineBackButton.Name = "caretlineBackButton";
            this.caretlineBackButton.Size = new System.Drawing.Size(33, 30);
            this.caretlineBackButton.TabIndex = 4;
            this.caretlineBackButton.Click += new System.EventHandler(this.CaretlineBackButtonClick);
            // 
            // selectionBackButton
            // 
            this.selectionBackButton.Location = new System.Drawing.Point(286, 86);
            this.selectionBackButton.Name = "selectionBackButton";
            this.selectionBackButton.Size = new System.Drawing.Size(33, 30);
            this.selectionBackButton.TabIndex = 14;
            this.selectionBackButton.Click += new System.EventHandler(this.SelectionBackButtonClick);
            // 
            // selectionForeButton
            // 
            this.selectionForeButton.Location = new System.Drawing.Point(286, 37);
            this.selectionForeButton.Name = "selectionForeButton";
            this.selectionForeButton.Size = new System.Drawing.Size(33, 30);
            this.selectionForeButton.TabIndex = 12;
            this.selectionForeButton.Click += new System.EventHandler(this.SelectionForeButtonClick);
            // 
            // caretForeTextBox
            // 
            this.caretForeTextBox.Location = new System.Drawing.Point(14, 40);
            this.caretForeTextBox.Name = "caretForeTextBox";
            this.caretForeTextBox.Size = new System.Drawing.Size(103, 23);
            this.caretForeTextBox.TabIndex = 1;
            this.caretForeTextBox.TextChanged += new System.EventHandler(this.EditorItemChanged);
            // 
            // caretlineBackTextBox
            // 
            this.caretlineBackTextBox.Location = new System.Drawing.Point(14, 90);
            this.caretlineBackTextBox.Name = "caretlineBackTextBox";
            this.caretlineBackTextBox.Size = new System.Drawing.Size(103, 23);
            this.caretlineBackTextBox.TabIndex = 3;
            this.caretlineBackTextBox.TextChanged += new System.EventHandler(this.EditorItemChanged);
            // 
            // selectionForeTextBox
            // 
            this.selectionForeTextBox.Location = new System.Drawing.Point(174, 40);
            this.selectionForeTextBox.Name = "selectionForeTextBox";
            this.selectionForeTextBox.Size = new System.Drawing.Size(103, 23);
            this.selectionForeTextBox.TabIndex = 11;
            this.selectionForeTextBox.TextChanged += new System.EventHandler(this.EditorItemChanged);
            // 
            // selectionBackTextBox
            // 
            this.selectionBackTextBox.Location = new System.Drawing.Point(174, 90);
            this.selectionBackTextBox.Name = "selectionBackTextBox";
            this.selectionBackTextBox.Size = new System.Drawing.Size(103, 23);
            this.selectionBackTextBox.TabIndex = 13;
            this.selectionBackTextBox.TextChanged += new System.EventHandler(this.EditorItemChanged);
            // 
            // marginForeButton
            // 
            this.marginForeButton.Location = new System.Drawing.Point(126, 136);
            this.marginForeButton.Name = "marginForeButton";
            this.marginForeButton.Size = new System.Drawing.Size(33, 30);
            this.marginForeButton.TabIndex = 6;
            this.marginForeButton.Click += new System.EventHandler(this.MarginForeButtonClick);
            // 
            // marginBackButton
            // 
            this.marginBackButton.Location = new System.Drawing.Point(126, 187);
            this.marginBackButton.Name = "marginBackButton";
            this.marginBackButton.Size = new System.Drawing.Size(33, 30);
            this.marginBackButton.TabIndex = 8;
            this.marginBackButton.Click += new System.EventHandler(this.MarginBackButtonClick);
            // 
            // markerBackButton
            // 
            this.markerBackButton.Location = new System.Drawing.Point(286, 187);
            this.markerBackButton.Name = "markerBackButton";
            this.markerBackButton.Size = new System.Drawing.Size(33, 30);
            this.markerBackButton.TabIndex = 18;
            this.markerBackButton.Click += new System.EventHandler(this.MarkerBackButtonClick);
            // 
            // markerForeButton
            // 
            this.markerForeButton.Location = new System.Drawing.Point(286, 136);
            this.markerForeButton.Name = "markerForeButton";
            this.markerForeButton.Size = new System.Drawing.Size(33, 30);
            this.markerForeButton.TabIndex = 16;
            this.markerForeButton.Click += new System.EventHandler(this.MarkerForeButtonClick);
            // 
            // marginForeTextBox
            // 
            this.marginForeTextBox.Location = new System.Drawing.Point(14, 140);
            this.marginForeTextBox.Name = "marginForeTextBox";
            this.marginForeTextBox.Size = new System.Drawing.Size(103, 23);
            this.marginForeTextBox.TabIndex = 5;
            this.marginForeTextBox.TextChanged += new System.EventHandler(this.EditorItemChanged);
            // 
            // marginBackTextBox
            // 
            this.marginBackTextBox.Location = new System.Drawing.Point(14, 190);
            this.marginBackTextBox.Name = "marginBackTextBox";
            this.marginBackTextBox.Size = new System.Drawing.Size(103, 23);
            this.marginBackTextBox.TabIndex = 7;
            this.marginBackTextBox.TextChanged += new System.EventHandler(this.EditorItemChanged);
            // 
            // markerForeTextBox
            // 
            this.markerForeTextBox.Location = new System.Drawing.Point(174, 140);
            this.markerForeTextBox.Name = "markerForeTextBox";
            this.markerForeTextBox.Size = new System.Drawing.Size(103, 23);
            this.markerForeTextBox.TabIndex = 15;
            this.markerForeTextBox.TextChanged += new System.EventHandler(this.EditorItemChanged);
            // 
            // markerBackTextBox
            // 
            this.markerBackTextBox.Location = new System.Drawing.Point(174, 190);
            this.markerBackTextBox.Name = "markerBackTextBox";
            this.markerBackTextBox.Size = new System.Drawing.Size(103, 23);
            this.markerBackTextBox.TabIndex = 17;
            this.markerBackTextBox.TextChanged += new System.EventHandler(this.EditorItemChanged);
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.DefaultExt = "fdz";
            this.saveFileDialog.Filter = "FlashDevelop Zip Files|*.fdz";
            // 
            // languageGroupBox
            // 
            this.languageGroupBox.Controls.Add(this.caretForeButton);
            this.languageGroupBox.Controls.Add(this.caretForeLabel);
            this.languageGroupBox.Controls.Add(this.caretForeTextBox);
            this.languageGroupBox.Controls.Add(this.caretlineBackButton);
            this.languageGroupBox.Controls.Add(this.caretlineBackLabel);
            this.languageGroupBox.Controls.Add(this.caretlineBackTextBox);
            this.languageGroupBox.Controls.Add(this.selectionBackButton);
            this.languageGroupBox.Controls.Add(this.selectionBackLabel);
            this.languageGroupBox.Controls.Add(this.selectionBackTextBox);
            this.languageGroupBox.Controls.Add(this.selectionForeButton);
            this.languageGroupBox.Controls.Add(this.selectionForeLabel);
            this.languageGroupBox.Controls.Add(this.selectionForeTextBox);
            this.languageGroupBox.Controls.Add(this.marginForeButton);
            this.languageGroupBox.Controls.Add(this.marginForeLabel);
            this.languageGroupBox.Controls.Add(this.marginForeTextBox);
            this.languageGroupBox.Controls.Add(this.marginBackButton);
            this.languageGroupBox.Controls.Add(this.marginBackLabel);
            this.languageGroupBox.Controls.Add(this.marginBackTextBox);
            this.languageGroupBox.Controls.Add(this.markerBackButton);
            this.languageGroupBox.Controls.Add(this.markerBackLabel);
            this.languageGroupBox.Controls.Add(this.markerBackTextBox);
            this.languageGroupBox.Controls.Add(this.markerForeButton);
            this.languageGroupBox.Controls.Add(this.markerForeLabel);
            this.languageGroupBox.Controls.Add(this.markerForeTextBox);
            this.languageGroupBox.Controls.Add(this.printMarginButton);
            this.languageGroupBox.Controls.Add(this.printMarginLabel);
            this.languageGroupBox.Controls.Add(this.printMarginTextBox);
            this.languageGroupBox.Controls.Add(this.highlightBackButton);
            this.languageGroupBox.Controls.Add(this.highlightBackLabel);
            this.languageGroupBox.Controls.Add(this.highlightBackTextBox);
            this.languageGroupBox.Controls.Add(this.modifiedLineButton);
            this.languageGroupBox.Controls.Add(this.modifiedLineLabel);
            this.languageGroupBox.Controls.Add(this.modifiedLineTextBox);
            this.languageGroupBox.Controls.Add(this.bookmarkLineButton);
            this.languageGroupBox.Controls.Add(this.bookmarkLineLabel);
            this.languageGroupBox.Controls.Add(this.bookmarkLineTextBox);
            this.languageGroupBox.Controls.Add(this.errorLineButton);
            this.languageGroupBox.Controls.Add(this.errorLineLabel);
            this.languageGroupBox.Controls.Add(this.errorLineTextBox);
            this.languageGroupBox.Controls.Add(this.debugLineButton);
            this.languageGroupBox.Controls.Add(this.debugLineLabel);
            this.languageGroupBox.Controls.Add(this.debugLineTextBox);
            this.languageGroupBox.Controls.Add(this.disabledLineButton);
            this.languageGroupBox.Controls.Add(this.disabledLineLabel);
            this.languageGroupBox.Controls.Add(this.disabledLineTextBox);
            this.languageGroupBox.Controls.Add(this.colorizeCheckBox);
            this.languageGroupBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.languageGroupBox.Location = new System.Drawing.Point(238, 10);
            this.languageGroupBox.Name = "languageGroupBox";
            this.languageGroupBox.Size = new System.Drawing.Size(494, 304);
            this.languageGroupBox.TabIndex = 6;
            this.languageGroupBox.TabStop = false;
            this.languageGroupBox.Text = "Editor Style";
            // 
            // caretForeLabel
            // 
            this.caretForeLabel.AutoSize = true;
            this.caretForeLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.caretForeLabel.Location = new System.Drawing.Point(14, 20);
            this.caretForeLabel.Name = "caretForeLabel";
            this.caretForeLabel.Size = new System.Drawing.Size(65, 16);
            this.caretForeLabel.TabIndex = 0;
            this.caretForeLabel.Text = "Caret fore:";
            // 
            // caretlineBackLabel
            // 
            this.caretlineBackLabel.AutoSize = true;
            this.caretlineBackLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.caretlineBackLabel.Location = new System.Drawing.Point(14, 70);
            this.caretlineBackLabel.Name = "caretlineBackLabel";
            this.caretlineBackLabel.Size = new System.Drawing.Size(90, 16);
            this.caretlineBackLabel.TabIndex = 0;
            this.caretlineBackLabel.Text = "Caret line back:";
            // 
            // selectionBackLabel
            // 
            this.selectionBackLabel.AutoSize = true;
            this.selectionBackLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.selectionBackLabel.Location = new System.Drawing.Point(174, 70);
            this.selectionBackLabel.Name = "selectionBackLabel";
            this.selectionBackLabel.Size = new System.Drawing.Size(87, 16);
            this.selectionBackLabel.TabIndex = 0;
            this.selectionBackLabel.Text = "Selection back:";
            // 
            // selectionForeLabel
            // 
            this.selectionForeLabel.AutoSize = true;
            this.selectionForeLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.selectionForeLabel.Location = new System.Drawing.Point(174, 20);
            this.selectionForeLabel.Name = "selectionForeLabel";
            this.selectionForeLabel.Size = new System.Drawing.Size(84, 16);
            this.selectionForeLabel.TabIndex = 0;
            this.selectionForeLabel.Text = "Selection fore:";
            // 
            // marginForeLabel
            // 
            this.marginForeLabel.AutoSize = true;
            this.marginForeLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.marginForeLabel.Location = new System.Drawing.Point(14, 120);
            this.marginForeLabel.Name = "marginForeLabel";
            this.marginForeLabel.Size = new System.Drawing.Size(73, 16);
            this.marginForeLabel.TabIndex = 0;
            this.marginForeLabel.Text = "Margin fore:";
            // 
            // marginBackLabel
            // 
            this.marginBackLabel.AutoSize = true;
            this.marginBackLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.marginBackLabel.Location = new System.Drawing.Point(14, 170);
            this.marginBackLabel.Name = "marginBackLabel";
            this.marginBackLabel.Size = new System.Drawing.Size(76, 16);
            this.marginBackLabel.TabIndex = 0;
            this.marginBackLabel.Text = "Margin back:";
            // 
            // markerBackLabel
            // 
            this.markerBackLabel.AutoSize = true;
            this.markerBackLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.markerBackLabel.Location = new System.Drawing.Point(174, 170);
            this.markerBackLabel.Name = "markerBackLabel";
            this.markerBackLabel.Size = new System.Drawing.Size(76, 16);
            this.markerBackLabel.TabIndex = 0;
            this.markerBackLabel.Text = "Marker back:";
            // 
            // markerForeLabel
            // 
            this.markerForeLabel.AutoSize = true;
            this.markerForeLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.markerForeLabel.Location = new System.Drawing.Point(174, 120);
            this.markerForeLabel.Name = "markerForeLabel";
            this.markerForeLabel.Size = new System.Drawing.Size(73, 16);
            this.markerForeLabel.TabIndex = 0;
            this.markerForeLabel.Text = "Marker fore:";
            // 
            // printMarginButton
            // 
            this.printMarginButton.Location = new System.Drawing.Point(446, 37);
            this.printMarginButton.Name = "printMarginButton";
            this.printMarginButton.Size = new System.Drawing.Size(33, 30);
            this.printMarginButton.TabIndex = 22;
            this.printMarginButton.Click += new System.EventHandler(this.PrintMarginButtonClick);
            // 
            // printMarginLabel
            // 
            this.printMarginLabel.AutoSize = true;
            this.printMarginLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.printMarginLabel.Location = new System.Drawing.Point(334, 20);
            this.printMarginLabel.Name = "printMarginLabel";
            this.printMarginLabel.Size = new System.Drawing.Size(102, 16);
            this.printMarginLabel.TabIndex = 0;
            this.printMarginLabel.Text = "Print margin fore:";
            // 
            // printMarginTextBox
            // 
            this.printMarginTextBox.Location = new System.Drawing.Point(334, 40);
            this.printMarginTextBox.Name = "printMarginTextBox";
            this.printMarginTextBox.Size = new System.Drawing.Size(103, 23);
            this.printMarginTextBox.TabIndex = 21;
            this.printMarginTextBox.TextChanged += new System.EventHandler(this.EditorItemChanged);
            // 
            // highlightBackButton
            // 
            this.highlightBackButton.Location = new System.Drawing.Point(446, 86);
            this.highlightBackButton.Name = "highlightBackButton";
            this.highlightBackButton.Size = new System.Drawing.Size(33, 30);
            this.highlightBackButton.TabIndex = 24;
            this.highlightBackButton.Click += new System.EventHandler(this.HighlightBackButtonClick);
            // 
            // highlightBackLabel
            // 
            this.highlightBackLabel.AutoSize = true;
            this.highlightBackLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.highlightBackLabel.Location = new System.Drawing.Point(334, 70);
            this.highlightBackLabel.Name = "highlightBackLabel";
            this.highlightBackLabel.Size = new System.Drawing.Size(88, 16);
            this.highlightBackLabel.TabIndex = 0;
            this.highlightBackLabel.Text = "Highlight back:";
            // 
            // highlightBackTextBox
            // 
            this.highlightBackTextBox.Location = new System.Drawing.Point(334, 90);
            this.highlightBackTextBox.Name = "highlightBackTextBox";
            this.highlightBackTextBox.Size = new System.Drawing.Size(103, 23);
            this.highlightBackTextBox.TabIndex = 23;
            this.highlightBackTextBox.TextChanged += new System.EventHandler(this.EditorItemChanged);
            // 
            // modifiedLineButton
            // 
            this.modifiedLineButton.Location = new System.Drawing.Point(446, 187);
            this.modifiedLineButton.Name = "modifiedLineButton";
            this.modifiedLineButton.Size = new System.Drawing.Size(33, 30);
            this.modifiedLineButton.TabIndex = 28;
            this.modifiedLineButton.Click += new System.EventHandler(this.ModifiedLineButtonClick);
            // 
            // modifiedLineLabel
            // 
            this.modifiedLineLabel.AutoSize = true;
            this.modifiedLineLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.modifiedLineLabel.Location = new System.Drawing.Point(334, 170);
            this.modifiedLineLabel.Name = "modifiedLineLabel";
            this.modifiedLineLabel.Size = new System.Drawing.Size(107, 16);
            this.modifiedLineLabel.TabIndex = 0;
            this.modifiedLineLabel.Text = "Modified line back:";
            // 
            // modifiedLineTextBox
            // 
            this.modifiedLineTextBox.Location = new System.Drawing.Point(334, 190);
            this.modifiedLineTextBox.Name = "modifiedLineTextBox";
            this.modifiedLineTextBox.Size = new System.Drawing.Size(103, 23);
            this.modifiedLineTextBox.TabIndex = 27;
            this.modifiedLineTextBox.TextChanged += new System.EventHandler(this.EditorItemChanged);
            // 
            // bookmarkLineButton
            // 
            this.bookmarkLineButton.Location = new System.Drawing.Point(446, 136);
            this.bookmarkLineButton.Name = "bookmarkLineButton";
            this.bookmarkLineButton.Size = new System.Drawing.Size(33, 30);
            this.bookmarkLineButton.TabIndex = 26;
            this.bookmarkLineButton.Click += new System.EventHandler(this.BookmarkLineButtonClick);
            // 
            // bookmarkLineLabel
            // 
            this.bookmarkLineLabel.AutoSize = true;
            this.bookmarkLineLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.bookmarkLineLabel.Location = new System.Drawing.Point(334, 120);
            this.bookmarkLineLabel.Name = "bookmarkLineLabel";
            this.bookmarkLineLabel.Size = new System.Drawing.Size(114, 16);
            this.bookmarkLineLabel.TabIndex = 0;
            this.bookmarkLineLabel.Text = "Bookmark line back:";
            // 
            // bookmarkLineTextBox
            // 
            this.bookmarkLineTextBox.Location = new System.Drawing.Point(334, 140);
            this.bookmarkLineTextBox.Name = "bookmarkLineTextBox";
            this.bookmarkLineTextBox.Size = new System.Drawing.Size(103, 23);
            this.bookmarkLineTextBox.TabIndex = 25;
            this.bookmarkLineTextBox.TextChanged += new System.EventHandler(this.EditorItemChanged);
            // 
            // errorLineButton
            // 
            this.errorLineButton.Location = new System.Drawing.Point(126, 236);
            this.errorLineButton.Name = "errorLineButton";
            this.errorLineButton.Size = new System.Drawing.Size(33, 30);
            this.errorLineButton.TabIndex = 10;
            this.errorLineButton.Click += new System.EventHandler(this.ErrorLineButtonClick);
            // 
            // errorLineLabel
            // 
            this.errorLineLabel.AutoSize = true;
            this.errorLineLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.errorLineLabel.Location = new System.Drawing.Point(14, 220);
            this.errorLineLabel.Name = "errorLineLabel";
            this.errorLineLabel.Size = new System.Drawing.Size(89, 16);
            this.errorLineLabel.TabIndex = 0;
            this.errorLineLabel.Text = "Error line back:";
            // 
            // errorLineTextBox
            // 
            this.errorLineTextBox.Location = new System.Drawing.Point(14, 240);
            this.errorLineTextBox.Name = "errorLineTextBox";
            this.errorLineTextBox.Size = new System.Drawing.Size(103, 23);
            this.errorLineTextBox.TabIndex = 9;
            this.errorLineTextBox.TextChanged += new System.EventHandler(this.EditorItemChanged);
            // 
            // debugLineButton
            // 
            this.debugLineButton.Location = new System.Drawing.Point(286, 236);
            this.debugLineButton.Name = "debugLineButton";
            this.debugLineButton.Size = new System.Drawing.Size(33, 30);
            this.debugLineButton.TabIndex = 20;
            this.debugLineButton.Click += new System.EventHandler(this.DebugLineButtonClick);
            // 
            // debugLineLabel
            // 
            this.debugLineLabel.AutoSize = true;
            this.debugLineLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.debugLineLabel.Location = new System.Drawing.Point(174, 220);
            this.debugLineLabel.Name = "debugLineLabel";
            this.debugLineLabel.Size = new System.Drawing.Size(96, 16);
            this.debugLineLabel.TabIndex = 0;
            this.debugLineLabel.Text = "Debug line back:";
            // 
            // debugLineTextBox
            // 
            this.debugLineTextBox.Location = new System.Drawing.Point(174, 240);
            this.debugLineTextBox.Name = "debugLineTextBox";
            this.debugLineTextBox.Size = new System.Drawing.Size(103, 23);
            this.debugLineTextBox.TabIndex = 19;
            this.debugLineTextBox.TextChanged += new System.EventHandler(this.EditorItemChanged);
            // 
            // disabledLineButton
            // 
            this.disabledLineButton.Location = new System.Drawing.Point(446, 236);
            this.disabledLineButton.Name = "disabledLineButton";
            this.disabledLineButton.Size = new System.Drawing.Size(33, 30);
            this.disabledLineButton.TabIndex = 30;
            this.disabledLineButton.Click += new System.EventHandler(this.DisabledLineButtonClick);
            // 
            // disabledLineLabel
            // 
            this.disabledLineLabel.AutoSize = true;
            this.disabledLineLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.disabledLineLabel.Location = new System.Drawing.Point(334, 220);
            this.disabledLineLabel.Name = "disabledLineLabel";
            this.disabledLineLabel.Size = new System.Drawing.Size(107, 16);
            this.disabledLineLabel.TabIndex = 0;
            this.disabledLineLabel.Text = "Disabled line back:";
            // 
            // disabledLineTextBox
            // 
            this.disabledLineTextBox.Location = new System.Drawing.Point(334, 240);
            this.disabledLineTextBox.Name = "disabledLineTextBox";
            this.disabledLineTextBox.Size = new System.Drawing.Size(103, 23);
            this.disabledLineTextBox.TabIndex = 29;
            this.disabledLineTextBox.TextChanged += new System.EventHandler(this.EditorItemChanged);
            // 
            // colorizeCheckBox
            // 
            this.colorizeCheckBox.AutoSize = true;
            this.colorizeCheckBox.Checked = true;
            this.colorizeCheckBox.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            this.colorizeCheckBox.Location = new System.Drawing.Point(14, 274);
            this.colorizeCheckBox.Name = "italicsCheckBox";
            this.colorizeCheckBox.Size = new System.Drawing.Size(58, 20);
            this.colorizeCheckBox.TabIndex = 12;
            this.colorizeCheckBox.Text = "Colorize first margin with 'gdefault' foreground color";
            this.colorizeCheckBox.ThreeState = true;
            this.colorizeCheckBox.UseVisualStyleBackColor = true;
            this.colorizeCheckBox.CheckedChanged += new System.EventHandler(this.EditorItemChanged);
            // 
            // languageDropDown
            // 
            this.languageDropDown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.languageDropDown.Location = new System.Drawing.Point(14, 15);
            this.languageDropDown.MaxLength = 200;
            this.languageDropDown.Name = "languageDropDown";
            this.languageDropDown.Size = new System.Drawing.Size(212, 24);
            this.languageDropDown.TabIndex = 4;
            this.languageDropDown.SelectedIndexChanged += new System.EventHandler(this.LanguagesSelectedIndexChanged);
            // 
            // EditorDialog
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(746, 577);
            this.Controls.Add(this.languageGroupBox);
            this.Controls.Add(this.itemGroupBox);
            this.Controls.Add(this.languageDropDown);
            this.Controls.Add(this.itemListView);
            this.Controls.Add(this.revertButton);
            this.Controls.Add(this.exportButton);
            this.Controls.Add(this.defaultButton);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.applyButton);
            this.Controls.Add(this.okButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EditorDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = " Editor Coloring";
            this.itemGroupBox.ResumeLayout(false);
            this.itemGroupBox.PerformLayout();
            this.languageGroupBox.ResumeLayout(false);
            this.languageGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        #region Methods And Event Handlers

        /// <summary>
        /// Gets the path to the language directory
        /// </summary>
        private String LangDir
        {
            get { return Path.Combine(PathHelper.SettingDir, "Languages"); }
        }

        /// <summary>
        /// Constant xml file style paths
        /// </summary>
        private const String stylePath = "Scintilla/languages/language/use-styles/style";
        private const String editorStylePath = "Scintilla/languages/language/editor-style";
        private const String defaultStylePath = "Scintilla/languages/language/use-styles/style[@name='default']";

        /// <summary>
        /// Applies the localized texts to the form
        /// </summary>
        private void ApplyLocalizedTexts()
        {
            ToolTip tooltip = new ToolTip();
            this.languageDropDown.Font = Globals.Settings.DefaultFont;
            this.languageDropDown.FlatStyle = Globals.Settings.ComboBoxFlatStyle;
            tooltip.SetToolTip(this.exportButton, TextHelper.GetString("Label.ExportFiles"));
            tooltip.SetToolTip(this.revertButton, TextHelper.GetString("Label.RevertFiles"));
            tooltip.SetToolTip(this.defaultButton, TextHelper.GetString("Label.MakeAsDefault"));
            this.saveFileDialog.Filter = TextHelper.GetString("Info.ZipFilter");
            this.Text = " " + TextHelper.GetString("Title.SyntaxEditDialog");
            this.boldCheckBox.Text = TextHelper.GetString("Info.Bold");
            this.italicsCheckBox.Text = TextHelper.GetString("Info.Italic");
            this.itemGroupBox.Text = TextHelper.GetString("Info.ItemStyle");
            this.languageGroupBox.Text = TextHelper.GetString("Info.EditorStyle");
            this.foregroundLabel.Text = TextHelper.GetString("Info.Foreground");
            this.backgroundLabel.Text = TextHelper.GetString("Info.Background");
            this.sampleTextLabel.Text = TextHelper.GetString("Info.SampleText");
            this.caretForeLabel.Text = TextHelper.GetString("Info.CaretFore");
            this.caretlineBackLabel.Text = TextHelper.GetString("Info.CaretLineBack");
            this.selectionBackLabel.Text = TextHelper.GetString("Info.SelectionBack");
            this.selectionForeLabel.Text = TextHelper.GetString("Info.SelectionFore");
            this.marginForeLabel.Text = TextHelper.GetString("Info.MarginFore");
            this.marginBackLabel.Text = TextHelper.GetString("Info.MarginBack");
            this.markerBackLabel.Text = TextHelper.GetString("Info.MarkerBack");
            this.markerForeLabel.Text = TextHelper.GetString("Info.MarkerFore");
            this.printMarginLabel.Text = TextHelper.GetString("Info.PrintMargin");
            this.highlightBackLabel.Text = TextHelper.GetString("Info.HighlightBack");
            this.modifiedLineLabel.Text = TextHelper.GetString("Info.ModifiedLine");
            this.bookmarkLineLabel.Text = TextHelper.GetString("Info.BookmarkLine");
            this.errorLineLabel.Text = TextHelper.GetString("Info.ErrorLineBack");
            this.debugLineLabel.Text = TextHelper.GetString("Info.DebugLineBack");
            this.disabledLineLabel.Text = TextHelper.GetString("Info.DisabledLineBack");
            this.colorizeCheckBox.Text = TextHelper.GetString("Info.ColorizeMarkerMargin");
            this.cancelButton.Text = TextHelper.GetString("Label.Cancel");
            this.applyButton.Text = TextHelper.GetString("Label.Apply");
            this.fontLabel.Text = TextHelper.GetString("Info.Font");
            this.sizeLabel.Text = TextHelper.GetString("Info.Size");
            this.okButton.Text = TextHelper.GetString("Label.Ok");
            if (Globals.MainForm.StandaloneMode)
            {
                this.revertButton.Enabled = false;
            }
        }

        /// <summary>
        /// Initializes the graphics
        /// </summary>
        private void InitializeGraphics()
        {
            ImageList imageList = new ImageList();
            imageList.ColorDepth = ColorDepth.Depth32Bit;
            imageList.Images.Add(PluginBase.MainForm.FindImage("129")); // snippet;
            imageList.Images.Add(PluginBase.MainForm.FindImage("328")); // palette;
            imageList.Images.Add(PluginBase.MainForm.FindImage("55|24|3|3")); // revert
            imageList.Images.Add(PluginBase.MainForm.FindImage("55|9|3|3")); // export
            imageList.Images.Add(PluginBase.MainForm.FindImage("55|25|3|3")); // default
            this.itemListView.SmallImageList = imageList;
            this.itemListView.SmallImageList.ImageSize = ScaleHelper.Scale(new Size(16, 16));
            this.revertButton.ImageList = this.exportButton.ImageList = imageList;
            this.disabledLineButton.ImageList = this.defaultButton.ImageList = imageList;
            this.foregroundButton.ImageList = this.backgroundButton.ImageList = imageList;
            this.caretForeButton.ImageList = this.caretlineBackButton.ImageList = imageList;
            this.selectionForeButton.ImageList = this.selectionBackButton.ImageList = imageList;
            this.marginBackButton.ImageList = this.marginForeButton.ImageList = imageList;
            this.markerBackButton.ImageList = this.markerForeButton.ImageList = imageList;
            this.printMarginButton.ImageList = this.highlightBackButton.ImageList = imageList;
            this.modifiedLineButton.ImageList = this.bookmarkLineButton.ImageList = imageList;
            this.errorLineButton.ImageList = this.debugLineButton.ImageList = imageList;
        }

        /// <summary>
        /// Initializes all ui components
        /// </summary>
        private void PopulateControls()
        {
            this.revertButton.ImageIndex = 2;
            this.exportButton.ImageIndex = 3;
            this.defaultButton.ImageIndex = 4;
            this.foregroundButton.ImageIndex = this.backgroundButton.ImageIndex = 1;
            this.caretForeButton.ImageIndex = this.caretlineBackButton.ImageIndex = 1;
            this.selectionForeButton.ImageIndex = this.selectionBackButton.ImageIndex = 1;
            this.marginBackButton.ImageIndex = this.marginForeButton.ImageIndex = 1;
            this.markerBackButton.ImageIndex = this.markerForeButton.ImageIndex = 1;
            this.printMarginButton.ImageIndex = this.highlightBackButton.ImageIndex = 1;
            this.modifiedLineButton.ImageIndex = this.bookmarkLineButton.ImageIndex = 1;
            this.errorLineButton.ImageIndex = this.debugLineButton.ImageIndex = 1; 
            this.disabledLineButton.ImageIndex = 1;
            String[] languageFiles = Directory.GetFiles(this.LangDir, "*.xml");
            foreach (String language in languageFiles)
            {
                String languageName = Path.GetFileNameWithoutExtension(language);
                this.languageDropDown.Items.Add(languageName);
            }
            InstalledFontCollection fonts = new InstalledFontCollection();
            this.fontNameComboBox.Items.Add("");
            foreach (FontFamily font in fonts.Families)
            {
                this.fontNameComboBox.Items.Add(font.GetName(1033));
            }
            Boolean foundSyntax = false;
            String curSyntax = ArgsProcessor.GetCurSyntax();
            foreach (Object item in this.languageDropDown.Items)
            {
                if (item.ToString().ToLower() == curSyntax)
                {
                    this.languageDropDown.SelectedItem = item;
                    foundSyntax = true;
                    break;
                }
            }
            if (!foundSyntax) this.languageDropDown.SelectedIndex = 0;
            this.columnHeader.Width = -2;
        }

        /// <summary>
        /// Loads language to be edited
        /// </summary>
        private void LoadLanguage(String newLanguage, Boolean promptToSave)
        {
            if (!this.isLanguageSaved && promptToSave)
            {
                this.PromptToSaveLanguage();
            }
            this.languageDoc = new XmlDocument();
            this.languageFile = Path.Combine(this.LangDir, newLanguage + ".xml");
            this.languageDoc.Load(languageFile);
            this.LoadEditorStyles();
            this.defaultStyleNode = this.languageDoc.SelectSingleNode(defaultStylePath) as XmlElement;
            XmlNodeList styles = this.languageDoc.SelectNodes(stylePath);
            this.itemListView.Items.Clear();
            foreach (XmlNode style in styles)
            {
                this.itemListView.Items.Add(style.Attributes["name"].Value, 0);
            }
            if (this.itemListView.Items.Count > 0)
            {
                this.itemListView.Items[0].Selected = true;
            }
            this.applyButton.Enabled = false;
            this.isLanguageSaved = true;
        }

        /// <summary>
        /// Loads the language item
        /// </summary>
        private void LoadLanguageItem(String item)
        {
            if (!this.isItemSaved) this.SaveCurrentItem();
            this.isLoadingItem = true;
            this.currentStyleNode = this.languageDoc.SelectSingleNode(stylePath + "[@name=\"" + item + "\"]") as XmlElement;
            this.fontNameComboBox.SelectedIndex = 0;
            this.fontSizeComboBox.Text = "";
            this.foregroundTextBox.Text = "";
            this.backgroundTextBox.Text = "";
            this.boldCheckBox.CheckState = CheckState.Indeterminate;
            this.italicsCheckBox.CheckState = CheckState.Indeterminate;
            if (this.currentStyleNode.Attributes["font"] != null)
            {
                String[] fonts = this.currentStyleNode.Attributes["font"].Value.Split(',');
                foreach (String font in fonts)
                {
                    if (IsFontInstalled(font))
                    {
                        this.fontNameComboBox.Text = font;
                        break;
                    }
                }
            }
            if (this.currentStyleNode.Attributes["size"] != null)
            {
                this.fontSizeComboBox.Text = this.currentStyleNode.Attributes["size"].Value;
            }
            if (this.currentStyleNode.Attributes["fore"] != null)
            {
                this.foregroundTextBox.Text = this.currentStyleNode.Attributes["fore"].Value;
            }
            if (this.currentStyleNode.Attributes["back"] != null)
            {
                this.backgroundTextBox.Text = this.currentStyleNode.Attributes["back"].Value;
            }
            if (this.currentStyleNode.Attributes["bold"] != null)
            {
                this.boldCheckBox.CheckState = CheckState.Unchecked;
                this.boldCheckBox.Checked = Boolean.Parse(this.currentStyleNode.Attributes["bold"].Value);
            }
            if (this.currentStyleNode.Attributes["italics"] != null)
            {
                this.italicsCheckBox.CheckState = CheckState.Unchecked;
                this.italicsCheckBox.Checked = Boolean.Parse(this.currentStyleNode.Attributes["italics"].Value);
            }
            this.UpdateSampleText();
            this.isLoadingItem = false;
            this.isItemSaved = true;
        }

        /// <summary>
        /// Checks if font is installed
        /// </summary>
        private static Boolean IsFontInstalled(String fontName)
        {
            using (var testFont = new Font(fontName, 9))
            {
                return fontName.Equals(testFont.Name, StringComparison.InvariantCultureIgnoreCase);
            }
        }

        /// <summary>
        /// Saves the current item being edited
        /// </summary>
        private void SaveCurrentItem()
        {
            if (this.fontNameComboBox.Text != "") this.currentStyleNode.SetAttribute("font", fontNameComboBox.Text);
            else this.currentStyleNode.RemoveAttribute("font");
            if (this.fontSizeComboBox.Text != "") this.currentStyleNode.SetAttribute("size", fontSizeComboBox.Text);
            else this.currentStyleNode.RemoveAttribute("size");
            if (this.foregroundTextBox.Text != "") this.currentStyleNode.SetAttribute("fore", foregroundTextBox.Text);
            else this.currentStyleNode.RemoveAttribute("fore");
            if (this.backgroundTextBox.Text != "") this.currentStyleNode.SetAttribute("back", backgroundTextBox.Text);
            else this.currentStyleNode.RemoveAttribute("back");
            if (this.boldCheckBox.CheckState == CheckState.Checked) this.currentStyleNode.SetAttribute("bold", "true");
            else if (this.boldCheckBox.CheckState == CheckState.Unchecked) this.currentStyleNode.SetAttribute("bold", "false");
            else this.currentStyleNode.RemoveAttribute("bold");
            if (this.italicsCheckBox.CheckState == CheckState.Checked) this.currentStyleNode.SetAttribute("italics", "true");
            else if (this.italicsCheckBox.CheckState == CheckState.Unchecked) this.currentStyleNode.SetAttribute("italics", "false");
            else this.currentStyleNode.RemoveAttribute("italics");
            this.isItemSaved = true;
        }
        
        /// <summary>
        /// Load the editor style items
        /// </summary>
        private void LoadEditorStyles()
        {
            this.isLoadingEditor = true;
            this.caretForeTextBox.Text = "";
            this.caretlineBackTextBox.Text = "";
            this.selectionBackTextBox.Text = "";
            this.selectionForeTextBox.Text = "";
            this.markerForeTextBox.Text = "";
            this.markerBackTextBox.Text = "";
            this.marginForeTextBox.Text = "";
            this.marginBackTextBox.Text = "";
            this.printMarginTextBox.Text = "";
            this.highlightBackTextBox.Text = "";
            this.modifiedLineTextBox.Text = "";
            this.bookmarkLineTextBox.Text = "";
            this.errorLineTextBox.Text = "";
            this.debugLineTextBox.Text = "";
            this.disabledLineTextBox.Text = "";
            this.colorizeCheckBox.CheckState = CheckState.Indeterminate;
            this.editorStyleNode = this.languageDoc.SelectSingleNode(editorStylePath) as XmlElement;
            if (this.editorStyleNode.Attributes["caret-fore"] != null)
            {
                this.caretForeTextBox.Text = this.editorStyleNode.Attributes["caret-fore"].Value;
            }
            if (this.editorStyleNode.Attributes["caretline-back"] != null)
            {
                this.caretlineBackTextBox.Text = this.editorStyleNode.Attributes["caretline-back"].Value;
            }
            if (this.editorStyleNode.Attributes["selection-back"] != null)
            {
                this.selectionBackTextBox.Text = this.editorStyleNode.Attributes["selection-back"].Value;
            }
            if (this.editorStyleNode.Attributes["selection-fore"] != null)
            {
                this.selectionForeTextBox.Text = this.editorStyleNode.Attributes["selection-fore"].Value;
            }
            if (this.editorStyleNode.Attributes["margin-fore"] != null)
            {
                this.marginForeTextBox.Text = this.editorStyleNode.Attributes["margin-fore"].Value;
            }
            if (this.editorStyleNode.Attributes["margin-back"] != null)
            {
                this.marginBackTextBox.Text = this.editorStyleNode.Attributes["margin-back"].Value;
            }
            if (this.editorStyleNode.Attributes["marker-fore"] != null)
            {
                this.markerForeTextBox.Text = this.editorStyleNode.Attributes["marker-fore"].Value;
            }
            if (this.editorStyleNode.Attributes["marker-back"] != null)
            {
                this.markerBackTextBox.Text = this.editorStyleNode.Attributes["marker-back"].Value;
            }
            if (this.editorStyleNode.Attributes["print-margin"] != null)
            {
                this.printMarginTextBox.Text = this.editorStyleNode.Attributes["print-margin"].Value;
            }
            if (this.editorStyleNode.Attributes["highlight-back"] != null)
            {
                this.highlightBackTextBox.Text = this.editorStyleNode.Attributes["highlight-back"].Value;
            }
            if (this.editorStyleNode.Attributes["modifiedline-back"] != null)
            {
                this.modifiedLineTextBox.Text = this.editorStyleNode.Attributes["modifiedline-back"].Value;
            }
            if (this.editorStyleNode.Attributes["bookmarkline-back"] != null)
            {
                this.bookmarkLineTextBox.Text = this.editorStyleNode.Attributes["bookmarkline-back"].Value;
            }
            if (this.editorStyleNode.Attributes["errorline-back"] != null)
            {
                this.errorLineTextBox.Text = this.editorStyleNode.Attributes["errorline-back"].Value;
            }
            if (this.editorStyleNode.Attributes["debugline-back"] != null)
            {
                this.debugLineTextBox.Text = this.editorStyleNode.Attributes["debugline-back"].Value;
            }
            if (this.editorStyleNode.Attributes["disabledline-back"] != null)
            {
                this.disabledLineTextBox.Text = this.editorStyleNode.Attributes["disabledline-back"].Value;
            }
            if (this.editorStyleNode.Attributes["colorize-marker-back"] != null)
            {
                this.colorizeCheckBox.CheckState = CheckState.Unchecked;
                this.colorizeCheckBox.Checked = Boolean.Parse(this.editorStyleNode.Attributes["colorize-marker-back"].Value);
            }
            this.isLoadingEditor = false;
            this.isEditorSaved = true;
        }

        /// <summary>
        /// Saves the editor style items
        /// </summary>
        private void SaveEditorStyles()
        {
            if (this.caretForeTextBox.Text != "") this.editorStyleNode.SetAttribute("caret-fore", this.caretForeTextBox.Text);
            else this.editorStyleNode.RemoveAttribute("caret-fore");
            if (this.caretlineBackTextBox.Text != "") this.editorStyleNode.SetAttribute("caretline-back", this.caretlineBackTextBox.Text);
            else this.editorStyleNode.RemoveAttribute("caretline-back");
            if (this.selectionForeTextBox.Text != "") this.editorStyleNode.SetAttribute("selection-fore", this.selectionForeTextBox.Text);
            else this.editorStyleNode.RemoveAttribute("selection-fore");
            if (this.selectionBackTextBox.Text != "") this.editorStyleNode.SetAttribute("selection-back", this.selectionBackTextBox.Text);
            else this.editorStyleNode.RemoveAttribute("selection-back");
            if (this.marginForeTextBox.Text != "") this.editorStyleNode.SetAttribute("margin-fore", this.marginForeTextBox.Text);
            else this.editorStyleNode.RemoveAttribute("margin-fore");
            if (this.marginBackTextBox.Text != "") this.editorStyleNode.SetAttribute("margin-back", this.marginBackTextBox.Text);
            else this.editorStyleNode.RemoveAttribute("margin-back");
            if (this.markerForeTextBox.Text != "") this.editorStyleNode.SetAttribute("marker-fore", this.markerForeTextBox.Text);
            else this.editorStyleNode.RemoveAttribute("marker-fore");
            if (this.markerBackTextBox.Text != "") this.editorStyleNode.SetAttribute("marker-back", this.markerBackTextBox.Text);
            else this.editorStyleNode.RemoveAttribute("marker-back");
            if (this.printMarginTextBox.Text != "") this.editorStyleNode.SetAttribute("print-margin", this.printMarginTextBox.Text);
            else this.editorStyleNode.RemoveAttribute("print-margin");
            if (this.highlightBackTextBox.Text != "") this.editorStyleNode.SetAttribute("highlight-back", this.highlightBackTextBox.Text);
            else this.editorStyleNode.RemoveAttribute("highlight-back");
            if (this.modifiedLineTextBox.Text != "") this.editorStyleNode.SetAttribute("modifiedline-back", this.modifiedLineTextBox.Text);
            else this.editorStyleNode.RemoveAttribute("modifiedline-back");
            if (this.bookmarkLineTextBox.Text != "") this.editorStyleNode.SetAttribute("bookmarkline-back", this.bookmarkLineTextBox.Text);
            else this.editorStyleNode.RemoveAttribute("bookmarkline-back");
            if (this.errorLineTextBox.Text != "") this.editorStyleNode.SetAttribute("errorline-back", this.errorLineTextBox.Text);
            else this.editorStyleNode.RemoveAttribute("errorline-back");
            if (this.debugLineTextBox.Text != "") this.editorStyleNode.SetAttribute("debugline-back", this.debugLineTextBox.Text);
            else this.editorStyleNode.RemoveAttribute("debugline-back");
            if (this.disabledLineTextBox.Text != "") this.editorStyleNode.SetAttribute("disabledline-back", this.disabledLineTextBox.Text);
            else this.editorStyleNode.RemoveAttribute("disabledline-back");
            if (this.colorizeCheckBox.CheckState == CheckState.Checked) this.editorStyleNode.SetAttribute("colorize-marker-back", "true");
            else if (this.colorizeCheckBox.CheckState == CheckState.Unchecked) this.editorStyleNode.SetAttribute("colorize-marker-back", "false");
            else this.currentStyleNode.RemoveAttribute("colorize-marker-back");
            this.isEditorSaved = true;
        }

        /// <summary>
        /// Updates the Sample Item from settings in dialog
        /// </summary>
        private void UpdateSampleText()
        {
            try
            {
                FontStyle fs = FontStyle.Regular;
                String fontName = this.fontNameComboBox.Text;
                if (fontName == "") fontName = this.defaultStyleNode.Attributes["font"].Value;
                String fontSize = this.fontSizeComboBox.Text;
                if (fontSize == "") fontSize = this.defaultStyleNode.Attributes["size"].Value;
                String foreColor = this.foregroundTextBox.Text;
                if (foreColor == "") foreColor = this.defaultStyleNode.Attributes["fore"].Value;
                String backColor = this.backgroundTextBox.Text;
                if (backColor == "") backColor = this.defaultStyleNode.Attributes["back"].Value;
                if (this.boldCheckBox.CheckState == CheckState.Checked) fs |= FontStyle.Bold;
                else if (this.boldCheckBox.CheckState == CheckState.Indeterminate)
                {
                    if (this.defaultStyleNode.Attributes["bold"] != null)
                    {
                        if (this.defaultStyleNode.Attributes["bold"].Value == "true") fs |= FontStyle.Bold;
                    }
                }
                if (this.italicsCheckBox.CheckState == CheckState.Checked) fs |= FontStyle.Italic;
                else if (this.italicsCheckBox.CheckState == CheckState.Indeterminate)
                {
                    if (this.defaultStyleNode.Attributes["italics"] != null)
                    {
                        if (this.defaultStyleNode.Attributes["italics"].Value == "true") fs |= FontStyle.Italic;
                    }
                }
                this.sampleTextLabel.Text = TextHelper.GetString("Info.SampleText");
                this.sampleTextLabel.Font = new Font(fontName, float.Parse(fontSize), fs);
                this.sampleTextLabel.ForeColor = ColorTranslator.FromHtml(foreColor);
                this.sampleTextLabel.BackColor = ColorTranslator.FromHtml(backColor);
            }
            catch (Exception)
            {
                this.sampleTextLabel.Font = PluginBase.Settings.ConsoleFont;
                this.sampleTextLabel.Text = "Preview not available...";
            }
        }

        /// <summary>
        /// Asks the user to save the changes
        /// </summary>
        private void PromptToSaveLanguage()
        {
            String message = TextHelper.GetString("Info.SaveCurrentLanguage");
            String caption = TextHelper.GetString("FlashDevelop.Title.ConfirmDialog");
            if (MessageBox.Show(message, caption, MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                this.SaveCurrentLanguage();
            }
        }

        /// <summary>
        /// After item has been changed, update controls
        /// </summary>
        private void ItemsSelectedIndexChanged(Object sender, EventArgs e)
        {
            if (this.itemListView.SelectedIndices.Count > 0)
            {
                String language = this.itemListView.SelectedItems[0].Text;
                this.LoadLanguageItem(language);
            }
        }

        /// <summary>
        /// When color has been selected, update controls
        /// </summary>
        private void ItemForegroundButtonClick(Object sender, EventArgs e)
        {
            this.colorDialog.Color = ColorTranslator.FromHtml(this.foregroundTextBox.Text);
            if (this.colorDialog.ShowDialog() == DialogResult.OK)
            {
                this.foregroundTextBox.Text = "0x" + this.colorDialog.Color.ToArgb().ToString("X8").Substring(2, 6);
            }
        }

        /// <summary>
        /// When color has been selected, update controls
        /// </summary>
        private void ItemBackgroundButtonClick(Object sender, EventArgs e)
        {
            this.colorDialog.Color = ColorTranslator.FromHtml(this.backgroundTextBox.Text);
            if (this.colorDialog.ShowDialog() == DialogResult.OK)
            {
                this.backgroundTextBox.Text = "0x" + this.colorDialog.Color.ToArgb().ToString("X8").Substring(2, 6);
            }
        }

        /// <summary>
        /// When color has been selected, update controls
        /// </summary>
        private void SelectionForeButtonClick(Object sender, EventArgs e)
        {
            this.colorDialog.Color = ColorTranslator.FromHtml(this.selectionForeTextBox.Text);
            if (this.colorDialog.ShowDialog() == DialogResult.OK)
            {
                this.selectionForeTextBox.Text = "0x" + this.colorDialog.Color.ToArgb().ToString("X8").Substring(2, 6);
            }
        }

        /// <summary>
        /// When color has been selected, update controls
        /// </summary>
        private void SelectionBackButtonClick(Object sender, EventArgs e)
        {
            this.colorDialog.Color = ColorTranslator.FromHtml(this.selectionBackTextBox.Text);
            if (this.colorDialog.ShowDialog() == DialogResult.OK)
            {
                this.selectionBackTextBox.Text = "0x" + this.colorDialog.Color.ToArgb().ToString("X8").Substring(2, 6);
            }
        }

        /// <summary>
        /// When color has been selected, update controls
        /// </summary>
        private void CaretlineBackButtonClick(Object sender, EventArgs e)
        {
            this.colorDialog.Color = ColorTranslator.FromHtml(this.caretlineBackTextBox.Text);
            if (this.colorDialog.ShowDialog() == DialogResult.OK)
            {
                this.caretlineBackTextBox.Text = "0x" + this.colorDialog.Color.ToArgb().ToString("X8").Substring(2, 6);
            }
        }

        /// <summary>
        /// When color has been selected, update controls
        /// </summary>
        private void CaretForeButtonClick(Object sender, EventArgs e)
        {
            this.colorDialog.Color = ColorTranslator.FromHtml(this.caretForeTextBox.Text);
            if (this.colorDialog.ShowDialog() == DialogResult.OK)
            {
                this.caretForeTextBox.Text = "0x" + this.colorDialog.Color.ToArgb().ToString("X8").Substring(2, 6);
            }
        }

        /// <summary>
        /// When color has been selected, update controls
        /// </summary>
        private void MarkerBackButtonClick(Object sender, EventArgs e)
        {
            this.colorDialog.Color = ColorTranslator.FromHtml(this.markerBackTextBox.Text);
            if (this.colorDialog.ShowDialog() == DialogResult.OK)
            {
                this.markerBackTextBox.Text = "0x" + this.colorDialog.Color.ToArgb().ToString("X8").Substring(2, 6);
            }
        }

        /// <summary>
        /// When color has been selected, update controls
        /// </summary>
        private void MarkerForeButtonClick(Object sender, EventArgs e)
        {
            this.colorDialog.Color = ColorTranslator.FromHtml(this.markerForeTextBox.Text);
            if (this.colorDialog.ShowDialog() == DialogResult.OK)
            {
                this.markerForeTextBox.Text = "0x" + this.colorDialog.Color.ToArgb().ToString("X8").Substring(2, 6);
            }
        }
        
        /// <summary>
        /// When color has been selected, update controls
        /// </summary>
        private void MarginBackButtonClick(Object sender, EventArgs e)
        {
            this.colorDialog.Color = ColorTranslator.FromHtml(this.marginForeTextBox.Text);
            if (this.colorDialog.ShowDialog() == DialogResult.OK)
            {
                this.marginBackTextBox.Text = "0x" + this.colorDialog.Color.ToArgb().ToString("X8").Substring(2, 6);
            }
        }
        
        /// <summary>
        /// When color has been selected, update controls
        /// </summary>
        private void MarginForeButtonClick(Object sender, EventArgs e)
        {
            this.colorDialog.Color = ColorTranslator.FromHtml(this.marginForeTextBox.Text);
            if (this.colorDialog.ShowDialog() == DialogResult.OK)
            {
                this.marginForeTextBox.Text = "0x" + this.colorDialog.Color.ToArgb().ToString("X8").Substring(2, 6);
            }
        }

        /// <summary>
        /// When color has been selected, update controls
        /// </summary>
        private void PrintMarginButtonClick(Object sender, EventArgs e)
        {
            this.colorDialog.Color = ColorTranslator.FromHtml(this.printMarginTextBox.Text);
            if (this.colorDialog.ShowDialog() == DialogResult.OK)
            {
                this.printMarginTextBox.Text = "0x" + this.colorDialog.Color.ToArgb().ToString("X8").Substring(2, 6);
            }
        }

        /// <summary>
        /// When color has been selected, update controls
        /// </summary>
        private void HighlightBackButtonClick(Object sender, EventArgs e)
        {
            this.colorDialog.Color = ColorTranslator.FromHtml(this.highlightBackTextBox.Text);
            if (this.colorDialog.ShowDialog() == DialogResult.OK)
            {
                this.highlightBackTextBox.Text = "0x" + this.colorDialog.Color.ToArgb().ToString("X8").Substring(2, 6);
            }
        }

        /// <summary>
        /// When color has been selected, update controls
        /// </summary>
        private void ModifiedLineButtonClick(Object sender, EventArgs e)
        {
            this.colorDialog.Color = ColorTranslator.FromHtml(this.modifiedLineTextBox.Text);
            if (this.colorDialog.ShowDialog() == DialogResult.OK)
            {
                this.modifiedLineTextBox.Text = "0x" + this.colorDialog.Color.ToArgb().ToString("X8").Substring(2, 6);
            }
        }

        /// <summary>
        /// When color has been selected, update controls
        /// </summary>
        private void BookmarkLineButtonClick(Object sender, EventArgs e)
        {
            this.colorDialog.Color = ColorTranslator.FromHtml(this.bookmarkLineTextBox.Text);
            if (this.colorDialog.ShowDialog() == DialogResult.OK)
            {
                this.bookmarkLineTextBox.Text = "0x" + this.colorDialog.Color.ToArgb().ToString("X8").Substring(2, 6);
            }
        }

        /// <summary>
        /// When color has been selected, update controls
        /// </summary>
        private void ErrorLineButtonClick(Object sender, EventArgs e)
        {
            this.colorDialog.Color = ColorTranslator.FromHtml(this.errorLineTextBox.Text);
            if (this.colorDialog.ShowDialog() == DialogResult.OK)
            {
                this.errorLineTextBox.Text = "0x" + this.colorDialog.Color.ToArgb().ToString("X8").Substring(2, 6);
            }
        }

        /// <summary>
        /// When color has been selected, update controls
        /// </summary>
        private void DebugLineButtonClick(Object sender, EventArgs e)
        {
            this.colorDialog.Color = ColorTranslator.FromHtml(this.debugLineTextBox.Text);
            if (this.colorDialog.ShowDialog() == DialogResult.OK)
            {
                this.debugLineTextBox.Text = "0x" + this.colorDialog.Color.ToArgb().ToString("X8").Substring(2, 6);
            }
        }

        /// <summary>
        /// When color has been selected, update controls
        /// </summary>
        private void DisabledLineButtonClick(Object sender, EventArgs e)
        {
            this.colorDialog.Color = ColorTranslator.FromHtml(this.disabledLineTextBox.Text);
            if (this.colorDialog.ShowDialog() == DialogResult.OK)
            {
                this.disabledLineTextBox.Text = "0x" + this.colorDialog.Color.ToArgb().ToString("X8").Substring(2, 6);
            }
        }

        /// <summary>
        /// When style item has been changed, update controls
        /// </summary>
        private void LanguageItemChanged(Object sender, EventArgs e)
        {
            if (!this.isLoadingItem)
            {
                this.isItemSaved = false;
                this.isLanguageSaved = false;
                this.applyButton.Enabled = true;
                this.UpdateSampleText();
            }
        }

        /// <summary>
        /// When editor item has been changed, update controls
        /// </summary>
        private void EditorItemChanged(Object sender, EventArgs e)
        {
            if (!this.isLoadingEditor)
            {
                this.isLanguageSaved = false;
                this.applyButton.Enabled = true;
                this.isEditorSaved = false;
            }
        }

        /// <summary>
        /// Saves the current modified language
        /// </summary>
        private void SaveCurrentLanguage()
        {
            if (!this.isItemSaved) this.SaveCurrentItem();
            if (!this.isEditorSaved) this.SaveEditorStyles();
            XmlTextWriter xmlWriter = new XmlTextWriter(this.languageFile, Encoding.UTF8);
            xmlWriter.Formatting = Formatting.Indented;
            xmlWriter.IndentChar = '\t';
            xmlWriter.Indentation = 1;
            this.languageDoc.Save(xmlWriter);
            this.applyButton.Enabled = false;
            this.isLanguageSaved = true;
            this.isEditorSaved = true;
            xmlWriter.Close();
        }

        /// <summary>
        /// After index has been changed, load the selected language
        /// </summary>
        private void LanguagesSelectedIndexChanged(Object sender, EventArgs e)
        {
            this.LoadLanguage(this.languageDropDown.Text, true);
        }

        /// <summary>
        /// Opens the revert settings dialog
        /// </summary>
        private void RevertLanguagesClick(Object sender, EventArgs e)
        {
            String caption = TextHelper.GetString("Title.ConfirmDialog");
            String message = TextHelper.GetString("Info.RevertSettingsFiles");
            DialogResult result = MessageBox.Show(message, caption, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);
            if (result == DialogResult.Yes)
            {
                this.Enabled = false;
                CleanupManager.RevertConfiguration(true);
                this.RefreshConfiguration();
                this.Enabled = true;
            }
            else if (result == DialogResult.No)
            {
                this.Enabled = false;
                CleanupManager.RevertConfiguration(false);
                this.RefreshConfiguration();
                this.Enabled = true;
            }
        }

        /// <summary>
        /// Refreshes the langugage configuration
        /// </summary>
        private void RefreshConfiguration()
        {
            this.LoadLanguage(this.languageDropDown.Text, true);
            if (this.itemListView.SelectedIndices.Count > 0)
            {
                String language = this.itemListView.SelectedItems[0].Text;
                this.LoadLanguageItem(language);
            }
            Globals.MainForm.RefreshSciConfig();
        }

        /// <summary>
        /// Makes the current style as the default
        /// </summary>
        private void MakeAsDefaultStyleClick(Object sender, EventArgs e)
        {
            this.Enabled = false;
            this.isLanguageSaved = true;
            String[] confFiles = Directory.GetFiles(this.LangDir, "*.xml");
            foreach (String confFile in confFiles)
            {
                try
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(confFile);
                    XmlElement node = doc.SelectSingleNode(defaultStylePath) as XmlElement;
                    if (this.fontNameComboBox.Text != "") node.SetAttribute("font", fontNameComboBox.Text);
                    else node.RemoveAttribute("font");
                    if (this.fontSizeComboBox.Text != "") node.SetAttribute("size", fontSizeComboBox.Text);
                    else node.RemoveAttribute("size");
                    if (this.foregroundTextBox.Text != "") node.SetAttribute("fore", foregroundTextBox.Text);
                    else node.RemoveAttribute("fore");
                    if (this.backgroundTextBox.Text != "") node.SetAttribute("back", backgroundTextBox.Text);
                    else node.RemoveAttribute("back");
                    if (this.boldCheckBox.CheckState == CheckState.Checked) node.SetAttribute("bold", "true");
                    else if (this.boldCheckBox.CheckState == CheckState.Unchecked) node.SetAttribute("bold", "false");
                    else node.RemoveAttribute("bold");
                    if (this.italicsCheckBox.CheckState == CheckState.Checked) node.SetAttribute("italics", "true");
                    else if (this.italicsCheckBox.CheckState == CheckState.Unchecked) node.SetAttribute("italics", "false");
                    else node.RemoveAttribute("italics");
                    XmlTextWriter xmlWriter = new XmlTextWriter(confFile, Encoding.UTF8);
                    xmlWriter.Formatting = Formatting.Indented;
                    xmlWriter.IndentChar = '\t';
                    xmlWriter.Indentation = 1;
                    doc.Save(xmlWriter);
                    xmlWriter.Close();
                }
                catch (Exception ex)
                {
                    ErrorManager.ShowError(ex);
                }
            }
            this.RefreshConfiguration();
            this.Enabled = true;
        }

        /// <summary>
        /// Opens the export settings dialog
        /// </summary>
        private void ExportLanguagesClick(Object sender, EventArgs e)
        {
            if (this.saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                String xmlFile = "";
                String[] langFiles = Directory.GetFiles(this.LangDir);
                ZipFile zipFile = ZipFile.Create(this.saveFileDialog.FileName);
                zipFile.BeginUpdate();
                foreach (String langFile in langFiles)
                {
                    xmlFile = Path.GetFileName(langFile);
                    zipFile.Add(langFile, "$(BaseDir)\\Settings\\Languages\\" + xmlFile);
                }
                zipFile.CommitUpdate();
                zipFile.Close();
            }
        }

        /// <summary>
        /// Saves the current language
        /// </summary>
        private void SaveButtonClick(Object sender, EventArgs e)
        {
            this.SaveCurrentLanguage();
            Globals.MainForm.RefreshSciConfig();
        }

        /// <summary>
        /// Closes the dialog without saving
        /// </summary>
        private void CancelButtonClick(Object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Closes the dialog and saves changes
        /// </summary>
        private void OkButtonClick(Object sender, EventArgs e)
        {
            if (!this.isLanguageSaved) this.SaveCurrentLanguage();
            Globals.MainForm.RefreshSciConfig();
            this.Close();
        }

        /// <summary>
        /// Shows the syntax edit dialog
        /// </summary>
        public static new void Show()
        {
            EditorDialog sp = new EditorDialog();
            sp.ShowDialog();
        }

        #endregion

    }

}
