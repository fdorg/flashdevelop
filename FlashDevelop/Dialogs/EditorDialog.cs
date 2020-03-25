using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;
using ICSharpCode.SharpZipLib.Zip;
using FlashDevelop.Utilities;
using FlashDevelop.Managers;
using PluginCore.Localization;
using PluginCore.Managers;
using PluginCore.Helpers;
using PluginCore.Controls;
using PluginCore;

namespace FlashDevelop.Dialogs
{
    public class EditorDialog : SmartForm, IThemeHandler
    {
        string languageFile;
        XmlDocument languageDoc;
        XmlElement editorStyleNode;
        XmlElement defaultStyleNode;
        XmlElement currentStyleNode;
        bool isItemSaved = true;
        bool isEditorSaved = true;
        bool isLoadingEditor = false;
        bool isLanguageSaved = true;
        bool isLoadingItem = false;
        Label sizeLabel;
        Label fontLabel;
        Button okButton;
        Button revertButton;
        Button defaultButton;
        Button applyButton;
        Button exportButton;
        Button cancelButton;
        ListView itemListView;
        ColorDialog colorDialog;
        GroupBox itemGroupBox;
        GroupBox languageGroupBox;
        Label markerForeLabel;
        Label markerBackLabel;
        Label marginForeLabel;
        Label marginBackLabel;
        Button markerForeButton;
        Button markerBackButton;
        Button marginForeButton;
        Button marginBackButton;
        TextBox markerForeTextBox;
        TextBox markerBackTextBox;
        TextBox marginForeTextBox;
        TextBox marginBackTextBox;
        Label caretForeLabel;
        Label caretlineBackLabel;
        Label selectionForeLabel;
        Label selectionBackLabel;
        Button caretForeButton;
        Button caretlineBackButton;
        Button selectionForeButton;
        Button selectionBackButton;
        TextBox caretForeTextBox;
        TextBox caretlineBackTextBox;
        TextBox selectionForeTextBox;
        TextBox selectionBackTextBox;
        SaveFileDialog saveFileDialog;
        Label sampleTextLabel;
        ComboBox fontSizeComboBox;
        ComboBox fontNameComboBox;
        TextBox foregroundTextBox;
        CheckBox boldCheckBox;
        CheckBox colorizeCheckBox;
        CheckBox italicsCheckBox;
        TextBox backgroundTextBox;
        Button backgroundButton;
        Button foregroundButton;
        ComboBox languageDropDown;
        ColumnHeader columnHeader;
        Label backgroundLabel;
        Label foregroundLabel;
        Label printMarginLabel;
        Label bookmarkLineLabel;
        Label modifiedLineLabel;
        Label highlightBackLabel;
        Label highlightWordBackLabel;
        Button printMarginButton;
        Button bookmarkLineButton;
        Button modifiedLineButton;
        Button highlightBackButton;
        Button highlightWordBackButton;
        TextBox printMarginTextBox;
        TextBox bookmarkLineTextBox;
        TextBox modifiedLineTextBox;
        TextBox highlightBackTextBox;
        TextBox highlightWordBackTextBox;
        Button debugLineButton;
        Button errorLineButton;
        Button disabledLineButton;
        TextBox disabledLineTextBox;
        TextBox debugLineTextBox;
        TextBox errorLineTextBox;
        Label disabledLineLabel;
        Label debugLineLabel;
        Label errorLineLabel;

        public EditorDialog()
        {
            Owner = Globals.MainForm;
            Font = PluginBase.Settings.DefaultFont;
            FormGuid = "c52528e8-084c-4cb7-9129-cfb64b4184c6";
            InitializeComponent();
            ApplyLocalizedTexts();
            InitializeGraphics();
            PopulateControls();
        }

        #region Windows Form Designer Generated Code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        void InitializeComponent()
        {
            okButton = new ButtonEx();
            applyButton = new ButtonEx();
            defaultButton = new ButtonEx();
            revertButton = new ButtonEx();
            cancelButton = new ButtonEx();
            itemListView = new ListViewEx();
            columnHeader = new ColumnHeader();
            colorDialog = new ColorDialog();
            itemGroupBox = new GroupBoxEx();
            sampleTextLabel = new Label();
            italicsCheckBox = new CheckBoxEx();
            colorizeCheckBox = new CheckBoxEx();
            backgroundButton = new ButtonEx();
            foregroundButton = new ButtonEx();
            boldCheckBox = new CheckBoxEx();
            backgroundTextBox = new TextBoxEx();
            foregroundTextBox = new TextBoxEx();
            fontSizeComboBox = new FlatCombo();
            fontNameComboBox = new FlatCombo();
            sizeLabel = new Label();
            backgroundLabel = new Label();
            foregroundLabel = new Label();
            fontLabel = new Label();
            caretForeButton = new ButtonEx();
            caretlineBackButton = new ButtonEx();
            selectionBackButton = new ButtonEx();
            selectionForeButton = new ButtonEx();
            caretForeTextBox = new TextBoxEx();
            caretlineBackTextBox = new TextBoxEx();
            selectionForeTextBox = new TextBoxEx();
            selectionBackTextBox = new TextBoxEx();
            marginForeButton = new ButtonEx();
            marginBackButton = new ButtonEx();
            markerBackButton = new ButtonEx();
            markerForeButton = new ButtonEx();
            marginForeTextBox = new TextBoxEx();
            marginBackTextBox = new TextBoxEx();
            markerForeTextBox = new TextBoxEx();
            markerBackTextBox = new TextBoxEx();
            saveFileDialog = new SaveFileDialog();
            languageGroupBox = new GroupBoxEx();
            caretForeLabel = new Label();
            caretlineBackLabel = new Label();
            selectionBackLabel = new Label();
            selectionForeLabel = new Label();
            marginForeLabel = new Label();
            marginBackLabel = new Label();
            markerBackLabel = new Label();
            markerForeLabel = new Label();
            printMarginButton = new ButtonEx();
            printMarginLabel = new Label();
            printMarginTextBox = new TextBoxEx();
            highlightBackButton = new ButtonEx();
            highlightBackLabel = new Label();
            highlightBackTextBox = new TextBoxEx();
            highlightWordBackButton = new ButtonEx();
            highlightWordBackLabel = new Label();
            highlightWordBackTextBox = new TextBoxEx();
            modifiedLineButton = new ButtonEx();
            modifiedLineLabel = new Label();
            modifiedLineTextBox = new TextBoxEx();
            bookmarkLineButton = new ButtonEx();
            bookmarkLineLabel = new Label();
            bookmarkLineTextBox = new TextBoxEx();
            errorLineButton = new ButtonEx();
            errorLineLabel = new Label();
            errorLineTextBox = new TextBoxEx();
            debugLineButton = new ButtonEx();
            debugLineLabel = new Label();
            debugLineTextBox = new TextBoxEx();
            disabledLineButton = new ButtonEx();
            disabledLineLabel = new Label();
            disabledLineTextBox = new TextBoxEx();
            languageDropDown = new FlatCombo();
            exportButton = new ButtonEx();
            itemGroupBox.SuspendLayout();
            languageGroupBox.SuspendLayout();
            SuspendLayout();
            //
            // colorDialog
            //
            colorDialog.FullOpen = true;
            // 
            // okButton
            // 
            okButton.FlatStyle = FlatStyle.System;
            okButton.Location = new Point(431, 587);
            okButton.Name = "okButton";
            okButton.Size = new Size(93, 29);
            okButton.TabIndex = 1;
            okButton.Text = "&OK";
            okButton.UseVisualStyleBackColor = true;
            okButton.Click += OkButtonClick;
            // 
            // applyButton
            // 
            applyButton.Enabled = false;
            applyButton.FlatStyle = FlatStyle.System;
            applyButton.Location = new Point(640, 587);
            applyButton.Name = "applyButton";
            applyButton.Size = new Size(93, 29);
            applyButton.TabIndex = 3;
            applyButton.Text = "&Apply";
            applyButton.UseVisualStyleBackColor = true;
            applyButton.Click += SaveButtonClick;
            // 
            // exportButton
            // 
            exportButton.Location = new Point(238, 587);
            exportButton.Name = "exportButton";
            exportButton.Size = new Size(35, 29);
            exportButton.TabIndex = 8;
            exportButton.Click += ExportLanguagesClick;
            // 
            // revertButton
            // 
            revertButton.Location = new Point(285, 587);
            revertButton.Name = "revertButton";
            revertButton.Size = new Size(35, 29);
            revertButton.TabIndex = 9;
            revertButton.Click += RevertLanguagesClick;
            // 
            // defaultButton
            // 
            defaultButton.Location = new Point(332, 587);
            defaultButton.Name = "defaultButton";
            defaultButton.Size = new Size(35, 29);
            defaultButton.TabIndex = 10;
            defaultButton.Click += MakeAsDefaultStyleClick;
            // 
            // cancelButton
            // 
            cancelButton.DialogResult = DialogResult.Cancel;
            cancelButton.FlatStyle = FlatStyle.System;
            cancelButton.Location = new Point(536, 587);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new Size(93, 29);
            cancelButton.TabIndex = 2;
            cancelButton.Text = "&Cancel";
            cancelButton.UseVisualStyleBackColor = true;
            cancelButton.Click += CancelButtonClick;
            // 
            // itemListView
            // 
            itemListView.Alignment = ListViewAlignment.Left;
            itemListView.Anchor = (AnchorStyles.Top | AnchorStyles.Bottom) | AnchorStyles.Left;
            itemListView.Columns.AddRange(new[] {columnHeader});
            itemListView.HeaderStyle = ColumnHeaderStyle.None;
            itemListView.HideSelection = false;
            itemListView.Location = new Point(14, 49);
            itemListView.MultiSelect = false;
            itemListView.Name = "itemListView";
            itemListView.Size = new Size(212, 566);
            itemListView.TabIndex = 5;
            itemListView.UseCompatibleStateImageBehavior = false;
            itemListView.View = View.Details;
            itemListView.SelectedIndexChanged += ItemsSelectedIndexChanged;
            // 
            // itemGroupBox
            // 
            itemGroupBox.Controls.Add(sampleTextLabel);
            itemGroupBox.Controls.Add(italicsCheckBox);
            itemGroupBox.Controls.Add(backgroundButton);
            itemGroupBox.Controls.Add(foregroundButton);
            itemGroupBox.Controls.Add(boldCheckBox);
            itemGroupBox.Controls.Add(backgroundTextBox);
            itemGroupBox.Controls.Add(foregroundTextBox);
            itemGroupBox.Controls.Add(fontSizeComboBox);
            itemGroupBox.Controls.Add(fontNameComboBox);
            itemGroupBox.Controls.Add(sizeLabel);
            itemGroupBox.Controls.Add(backgroundLabel);
            itemGroupBox.Controls.Add(foregroundLabel);
            itemGroupBox.Controls.Add(fontLabel);
            itemGroupBox.FlatStyle = FlatStyle.System;
            itemGroupBox.Location = new Point(238, 372);
            itemGroupBox.Name = "itemGroupBox";
            itemGroupBox.Size = new Size(494, 204);
            itemGroupBox.TabIndex = 7;
            itemGroupBox.TabStop = false;
            itemGroupBox.Text = "Item Style";
            // 
            // sampleTextLabel
            // 
            sampleTextLabel.Anchor = ((AnchorStyles.Top | AnchorStyles.Bottom) | AnchorStyles.Left)| AnchorStyles.Right;
            sampleTextLabel.BackColor = Color.White;
            sampleTextLabel.Location = new Point(15, 130);
            sampleTextLabel.Name = "sampleTextLabel";
            sampleTextLabel.Size = new Size(465, 60);
            sampleTextLabel.TabIndex = 13;
            sampleTextLabel.Text = "Sample Text";
            sampleTextLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // italicsCheckBox
            // 
            italicsCheckBox.AutoSize = true;
            italicsCheckBox.Checked = true;
            italicsCheckBox.CheckState = CheckState.Indeterminate;
            italicsCheckBox.Location = new Point(327, 101);
            italicsCheckBox.Name = "italicsCheckBox";
            italicsCheckBox.Size = new Size(58, 20);
            italicsCheckBox.TabIndex = 12;
            italicsCheckBox.Text = "Italics";
            italicsCheckBox.ThreeState = true;
            italicsCheckBox.UseVisualStyleBackColor = true;
            italicsCheckBox.CheckStateChanged += LanguageItemChanged;
            // 
            // backgroundButton
            // 
            backgroundButton.Location = new Point(267, 90);
            backgroundButton.Name = "backgroundButton";
            backgroundButton.Size = new Size(33, 30);
            backgroundButton.TabIndex = 10;
            backgroundButton.UseVisualStyleBackColor = true;
            backgroundButton.Click += ItemBackgroundButtonClick;
            // 
            // foregroundButton
            // 
            foregroundButton.Location = new Point(117, 90);
            foregroundButton.Name = "foregroundButton";
            foregroundButton.Size = new Size(33, 30);
            foregroundButton.TabIndex = 7;
            foregroundButton.UseVisualStyleBackColor = true;
            foregroundButton.Click += ItemForegroundButtonClick;
            // 
            // boldCheckBox
            // 
            boldCheckBox.AutoSize = true;
            boldCheckBox.Checked = true;
            boldCheckBox.CheckState = CheckState.Indeterminate;
            boldCheckBox.Location = new Point(327, 79);
            boldCheckBox.Name = "boldCheckBox";
            boldCheckBox.Size = new Size(50, 20);
            boldCheckBox.TabIndex = 11;
            boldCheckBox.Text = "Bold";
            boldCheckBox.ThreeState = true;
            boldCheckBox.UseVisualStyleBackColor = true;
            boldCheckBox.CheckStateChanged += LanguageItemChanged;
            // 
            // backgroundTextBox
            // 
            backgroundTextBox.Location = new Point(164, 94);
            backgroundTextBox.Name = "backgroundTextBox";
            backgroundTextBox.Size = new Size(94, 23);
            backgroundTextBox.TabIndex = 9;
            backgroundTextBox.TextChanged += LanguageItemChanged;
            // 
            // foregroundTextBox
            // 
            foregroundTextBox.Location = new Point(14, 94);
            foregroundTextBox.Name = "foregroundTextBox";
            foregroundTextBox.Size = new Size(94, 23);
            foregroundTextBox.TabIndex = 6;
            foregroundTextBox.TextChanged += LanguageItemChanged;
            // 
            // fontSizeComboBox
            // 
            fontSizeComboBox.FormattingEnabled = true;
            fontSizeComboBox.Items.AddRange(new object[] {"","6","7","8","9","10","11","12","13","14","15","16","17","18","19","20","21","22","23","24"});
            fontSizeComboBox.Location = new Point(324, 42);
            fontSizeComboBox.Name = "fontSizeComboBox";
            fontSizeComboBox.Size = new Size(157, 24);
            fontSizeComboBox.TabIndex = 4;
            fontSizeComboBox.TextChanged += LanguageItemChanged;
            // 
            // fontNameComboBox
            // 
            fontNameComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            fontNameComboBox.FormattingEnabled = true;
            fontNameComboBox.Location = new Point(14, 42);
            fontNameComboBox.Name = "fontNameComboBox";
            fontNameComboBox.Size = new Size(285, 24);
            fontNameComboBox.TabIndex = 2;
            fontNameComboBox.SelectedIndexChanged += LanguageItemChanged;
            // 
            // sizeLabel
            // 
            sizeLabel.AutoSize = true;
            sizeLabel.FlatStyle = FlatStyle.System;
            sizeLabel.Location = new Point(324, 21);
            sizeLabel.Name = "sizeLabel";
            sizeLabel.Size = new Size(32, 16);
            sizeLabel.TabIndex = 3;
            sizeLabel.Text = "Size:";
            // 
            // backgroundLabel
            // 
            backgroundLabel.AutoSize = true;
            backgroundLabel.FlatStyle = FlatStyle.System;
            backgroundLabel.Location = new Point(164, 74);
            backgroundLabel.Name = "backgroundLabel";
            backgroundLabel.Size = new Size(75, 16);
            backgroundLabel.TabIndex = 8;
            backgroundLabel.Text = "Background:";
            // 
            // foregroundLabel
            // 
            foregroundLabel.AutoSize = true;
            foregroundLabel.FlatStyle = FlatStyle.System;
            foregroundLabel.Location = new Point(14, 74);
            foregroundLabel.Name = "foregroundLabel";
            foregroundLabel.Size = new Size(75, 16);
            foregroundLabel.TabIndex = 5;
            foregroundLabel.Text = "Foreground:";
            // 
            // fontLabel
            // 
            fontLabel.AutoSize = true;
            fontLabel.FlatStyle = FlatStyle.System;
            fontLabel.Location = new Point(14, 21);
            fontLabel.Name = "fontLabel";
            fontLabel.Size = new Size(35, 16);
            fontLabel.TabIndex = 1;
            fontLabel.Text = "Font:";
            // 
            // caretForeButton
            // 
            caretForeButton.Location = new Point(126, 37);
            caretForeButton.Name = "caretForeButton";
            caretForeButton.Size = new Size(33, 30);
            caretForeButton.TabIndex = 2;
            caretForeButton.Click += CaretForeButtonClick;
            // 
            // caretlineBackButton
            // 
            caretlineBackButton.Location = new Point(126, 86);
            caretlineBackButton.Name = "caretlineBackButton";
            caretlineBackButton.Size = new Size(33, 30);
            caretlineBackButton.TabIndex = 4;
            caretlineBackButton.Click += CaretlineBackButtonClick;
            // 
            // selectionBackButton
            // 
            selectionBackButton.Location = new Point(286, 86);
            selectionBackButton.Name = "selectionBackButton";
            selectionBackButton.Size = new Size(33, 30);
            selectionBackButton.TabIndex = 14;
            selectionBackButton.Click += SelectionBackButtonClick;
            // 
            // selectionForeButton
            // 
            selectionForeButton.Location = new Point(286, 37);
            selectionForeButton.Name = "selectionForeButton";
            selectionForeButton.Size = new Size(33, 30);
            selectionForeButton.TabIndex = 12;
            selectionForeButton.Click += SelectionForeButtonClick;
            // 
            // caretForeTextBox
            // 
            caretForeTextBox.Location = new Point(14, 40);
            caretForeTextBox.Name = "caretForeTextBox";
            caretForeTextBox.Size = new Size(103, 23);
            caretForeTextBox.TabIndex = 1;
            caretForeTextBox.TextChanged += EditorItemChanged;
            // 
            // caretlineBackTextBox
            // 
            caretlineBackTextBox.Location = new Point(14, 90);
            caretlineBackTextBox.Name = "caretlineBackTextBox";
            caretlineBackTextBox.Size = new Size(103, 23);
            caretlineBackTextBox.TabIndex = 3;
            caretlineBackTextBox.TextChanged += EditorItemChanged;
            // 
            // selectionForeTextBox
            // 
            selectionForeTextBox.Location = new Point(174, 40);
            selectionForeTextBox.Name = "selectionForeTextBox";
            selectionForeTextBox.Size = new Size(103, 23);
            selectionForeTextBox.TabIndex = 11;
            selectionForeTextBox.TextChanged += EditorItemChanged;
            // 
            // selectionBackTextBox
            // 
            selectionBackTextBox.Location = new Point(174, 90);
            selectionBackTextBox.Name = "selectionBackTextBox";
            selectionBackTextBox.Size = new Size(103, 23);
            selectionBackTextBox.TabIndex = 13;
            selectionBackTextBox.TextChanged += EditorItemChanged;
            // 
            // marginForeButton
            // 
            marginForeButton.Location = new Point(126, 136);
            marginForeButton.Name = "marginForeButton";
            marginForeButton.Size = new Size(33, 30);
            marginForeButton.TabIndex = 6;
            marginForeButton.Click += MarginForeButtonClick;
            // 
            // marginBackButton
            // 
            marginBackButton.Location = new Point(126, 187);
            marginBackButton.Name = "marginBackButton";
            marginBackButton.Size = new Size(33, 30);
            marginBackButton.TabIndex = 8;
            marginBackButton.Click += MarginBackButtonClick;
            // 
            // markerBackButton
            // 
            markerBackButton.Location = new Point(286, 187);
            markerBackButton.Name = "markerBackButton";
            markerBackButton.Size = new Size(33, 30);
            markerBackButton.TabIndex = 18;
            markerBackButton.Click += MarkerBackButtonClick;
            // 
            // markerForeButton
            // 
            markerForeButton.Location = new Point(286, 136);
            markerForeButton.Name = "markerForeButton";
            markerForeButton.Size = new Size(33, 30);
            markerForeButton.TabIndex = 16;
            markerForeButton.Click += MarkerForeButtonClick;
            // 
            // marginForeTextBox
            // 
            marginForeTextBox.Location = new Point(14, 140);
            marginForeTextBox.Name = "marginForeTextBox";
            marginForeTextBox.Size = new Size(103, 23);
            marginForeTextBox.TabIndex = 5;
            marginForeTextBox.TextChanged += EditorItemChanged;
            // 
            // marginBackTextBox
            // 
            marginBackTextBox.Location = new Point(14, 190);
            marginBackTextBox.Name = "marginBackTextBox";
            marginBackTextBox.Size = new Size(103, 23);
            marginBackTextBox.TabIndex = 7;
            marginBackTextBox.TextChanged += EditorItemChanged;
            // 
            // markerForeTextBox
            // 
            markerForeTextBox.Location = new Point(174, 140);
            markerForeTextBox.Name = "markerForeTextBox";
            markerForeTextBox.Size = new Size(103, 23);
            markerForeTextBox.TabIndex = 15;
            markerForeTextBox.TextChanged += EditorItemChanged;
            // 
            // markerBackTextBox
            // 
            markerBackTextBox.Location = new Point(174, 190);
            markerBackTextBox.Name = "markerBackTextBox";
            markerBackTextBox.Size = new Size(103, 23);
            markerBackTextBox.TabIndex = 17;
            markerBackTextBox.TextChanged += EditorItemChanged;
            // 
            // saveFileDialog
            // 
            saveFileDialog.DefaultExt = "fdz";
            saveFileDialog.Filter = "FlashDevelop Zip Files|*.fdz";
            // 
            // languageGroupBox
            // 
            languageGroupBox.Controls.Add(caretForeButton);
            languageGroupBox.Controls.Add(caretForeLabel);
            languageGroupBox.Controls.Add(caretForeTextBox);
            languageGroupBox.Controls.Add(caretlineBackButton);
            languageGroupBox.Controls.Add(caretlineBackLabel);
            languageGroupBox.Controls.Add(caretlineBackTextBox);
            languageGroupBox.Controls.Add(selectionBackButton);
            languageGroupBox.Controls.Add(selectionBackLabel);
            languageGroupBox.Controls.Add(selectionBackTextBox);
            languageGroupBox.Controls.Add(selectionForeButton);
            languageGroupBox.Controls.Add(selectionForeLabel);
            languageGroupBox.Controls.Add(selectionForeTextBox);
            languageGroupBox.Controls.Add(marginForeButton);
            languageGroupBox.Controls.Add(marginForeLabel);
            languageGroupBox.Controls.Add(marginForeTextBox);
            languageGroupBox.Controls.Add(marginBackButton);
            languageGroupBox.Controls.Add(marginBackLabel);
            languageGroupBox.Controls.Add(marginBackTextBox);
            languageGroupBox.Controls.Add(markerBackButton);
            languageGroupBox.Controls.Add(markerBackLabel);
            languageGroupBox.Controls.Add(markerBackTextBox);
            languageGroupBox.Controls.Add(markerForeButton);
            languageGroupBox.Controls.Add(markerForeLabel);
            languageGroupBox.Controls.Add(markerForeTextBox);
            languageGroupBox.Controls.Add(printMarginButton);
            languageGroupBox.Controls.Add(printMarginLabel);
            languageGroupBox.Controls.Add(printMarginTextBox);
            languageGroupBox.Controls.Add(highlightBackButton);
            languageGroupBox.Controls.Add(highlightBackLabel);
            languageGroupBox.Controls.Add(highlightBackTextBox);
            languageGroupBox.Controls.Add(highlightWordBackButton);
            languageGroupBox.Controls.Add(highlightWordBackLabel);
            languageGroupBox.Controls.Add(highlightWordBackTextBox);
            languageGroupBox.Controls.Add(modifiedLineButton);
            languageGroupBox.Controls.Add(modifiedLineLabel);
            languageGroupBox.Controls.Add(modifiedLineTextBox);
            languageGroupBox.Controls.Add(bookmarkLineButton);
            languageGroupBox.Controls.Add(bookmarkLineLabel);
            languageGroupBox.Controls.Add(bookmarkLineTextBox);
            languageGroupBox.Controls.Add(errorLineButton);
            languageGroupBox.Controls.Add(errorLineLabel);
            languageGroupBox.Controls.Add(errorLineTextBox);
            languageGroupBox.Controls.Add(debugLineButton);
            languageGroupBox.Controls.Add(debugLineLabel);
            languageGroupBox.Controls.Add(debugLineTextBox);
            languageGroupBox.Controls.Add(disabledLineButton);
            languageGroupBox.Controls.Add(disabledLineLabel);
            languageGroupBox.Controls.Add(disabledLineTextBox);
            languageGroupBox.Controls.Add(colorizeCheckBox);
            languageGroupBox.FlatStyle = FlatStyle.System;
            languageGroupBox.Location = new Point(238, 10);
            languageGroupBox.Name = "languageGroupBox";
            languageGroupBox.Size = new Size(494, 357);
            languageGroupBox.TabIndex = 6;
            languageGroupBox.TabStop = false;
            languageGroupBox.Text = "Editor Style";
            // 
            // caretForeLabel
            // 
            caretForeLabel.AutoSize = true;
            caretForeLabel.FlatStyle = FlatStyle.System;
            caretForeLabel.Location = new Point(14, 20);
            caretForeLabel.Name = "caretForeLabel";
            caretForeLabel.Size = new Size(65, 16);
            caretForeLabel.TabIndex = 0;
            caretForeLabel.Text = "Caret fore:";
            // 
            // caretlineBackLabel
            // 
            caretlineBackLabel.AutoSize = true;
            caretlineBackLabel.FlatStyle = FlatStyle.System;
            caretlineBackLabel.Location = new Point(14, 70);
            caretlineBackLabel.Name = "caretlineBackLabel";
            caretlineBackLabel.Size = new Size(90, 16);
            caretlineBackLabel.TabIndex = 0;
            caretlineBackLabel.Text = "Caret line back:";
            // 
            // selectionBackLabel
            // 
            selectionBackLabel.AutoSize = true;
            selectionBackLabel.FlatStyle = FlatStyle.System;
            selectionBackLabel.Location = new Point(174, 70);
            selectionBackLabel.Name = "selectionBackLabel";
            selectionBackLabel.Size = new Size(87, 16);
            selectionBackLabel.TabIndex = 0;
            selectionBackLabel.Text = "Selection back:";
            // 
            // selectionForeLabel
            // 
            selectionForeLabel.AutoSize = true;
            selectionForeLabel.FlatStyle = FlatStyle.System;
            selectionForeLabel.Location = new Point(174, 20);
            selectionForeLabel.Name = "selectionForeLabel";
            selectionForeLabel.Size = new Size(84, 16);
            selectionForeLabel.TabIndex = 0;
            selectionForeLabel.Text = "Selection fore:";
            // 
            // marginForeLabel
            // 
            marginForeLabel.AutoSize = true;
            marginForeLabel.FlatStyle = FlatStyle.System;
            marginForeLabel.Location = new Point(14, 120);
            marginForeLabel.Name = "marginForeLabel";
            marginForeLabel.Size = new Size(73, 16);
            marginForeLabel.TabIndex = 0;
            marginForeLabel.Text = "Margin fore:";
            // 
            // marginBackLabel
            // 
            marginBackLabel.AutoSize = true;
            marginBackLabel.FlatStyle = FlatStyle.System;
            marginBackLabel.Location = new Point(14, 170);
            marginBackLabel.Name = "marginBackLabel";
            marginBackLabel.Size = new Size(76, 16);
            marginBackLabel.TabIndex = 0;
            marginBackLabel.Text = "Margin back:";
            // 
            // markerBackLabel
            // 
            markerBackLabel.AutoSize = true;
            markerBackLabel.FlatStyle = FlatStyle.System;
            markerBackLabel.Location = new Point(174, 170);
            markerBackLabel.Name = "markerBackLabel";
            markerBackLabel.Size = new Size(76, 16);
            markerBackLabel.TabIndex = 0;
            markerBackLabel.Text = "Marker back:";
            // 
            // markerForeLabel
            // 
            markerForeLabel.AutoSize = true;
            markerForeLabel.FlatStyle = FlatStyle.System;
            markerForeLabel.Location = new Point(174, 120);
            markerForeLabel.Name = "markerForeLabel";
            markerForeLabel.Size = new Size(73, 16);
            markerForeLabel.TabIndex = 0;
            markerForeLabel.Text = "Marker fore:";
            // 
            // printMarginButton
            // 
            printMarginButton.Location = new Point(446, 37);
            printMarginButton.Name = "printMarginButton";
            printMarginButton.Size = new Size(33, 30);
            printMarginButton.TabIndex = 22;
            printMarginButton.Click += PrintMarginButtonClick;
            // 
            // printMarginLabel
            // 
            printMarginLabel.AutoSize = true;
            printMarginLabel.FlatStyle = FlatStyle.System;
            printMarginLabel.Location = new Point(334, 20);
            printMarginLabel.Name = "printMarginLabel";
            printMarginLabel.Size = new Size(102, 16);
            printMarginLabel.TabIndex = 0;
            printMarginLabel.Text = "Print margin fore:";
            // 
            // printMarginTextBox
            // 
            printMarginTextBox.Location = new Point(334, 40);
            printMarginTextBox.Name = "printMarginTextBox";
            printMarginTextBox.Size = new Size(103, 23);
            printMarginTextBox.TabIndex = 21;
            printMarginTextBox.TextChanged += EditorItemChanged;
            // 
            // highlightBackButton
            // 
            highlightBackButton.Location = new Point(446, 86);
            highlightBackButton.Name = "highlightBackButton";
            highlightBackButton.Size = new Size(33, 30);
            highlightBackButton.TabIndex = 24;
            highlightBackButton.Click += HighlightBackButtonClick;
            // 
            // highlightBackLabel
            // 
            highlightBackLabel.AutoSize = true;
            highlightBackLabel.FlatStyle = FlatStyle.System;
            highlightBackLabel.Location = new Point(334, 70);
            highlightBackLabel.Name = "highlightBackLabel";
            highlightBackLabel.Size = new Size(88, 16);
            highlightBackLabel.TabIndex = 0;
            highlightBackLabel.Text = "Highlight back:";
            // 
            // highlightBackTextBox
            // 
            highlightBackTextBox.Location = new Point(334, 90);
            highlightBackTextBox.Name = "highlightBackTextBox";
            highlightBackTextBox.Size = new Size(103, 23);
            highlightBackTextBox.TabIndex = 23;
            highlightBackTextBox.TextChanged += EditorItemChanged;
            // 
            // modifiedLineButton
            // 
            modifiedLineButton.Location = new Point(446, 187);
            modifiedLineButton.Name = "modifiedLineButton";
            modifiedLineButton.Size = new Size(33, 30);
            modifiedLineButton.TabIndex = 28;
            modifiedLineButton.Click += ModifiedLineButtonClick;
            // 
            // modifiedLineLabel
            // 
            modifiedLineLabel.AutoSize = true;
            modifiedLineLabel.FlatStyle = FlatStyle.System;
            modifiedLineLabel.Location = new Point(334, 170);
            modifiedLineLabel.Name = "modifiedLineLabel";
            modifiedLineLabel.Size = new Size(107, 16);
            modifiedLineLabel.TabIndex = 0;
            modifiedLineLabel.Text = "Modified line back:";
            // 
            // modifiedLineTextBox
            // 
            modifiedLineTextBox.Location = new Point(334, 190);
            modifiedLineTextBox.Name = "modifiedLineTextBox";
            modifiedLineTextBox.Size = new Size(103, 23);
            modifiedLineTextBox.TabIndex = 27;
            modifiedLineTextBox.TextChanged += EditorItemChanged;
            // 
            // bookmarkLineButton
            // 
            bookmarkLineButton.Location = new Point(446, 136);
            bookmarkLineButton.Name = "bookmarkLineButton";
            bookmarkLineButton.Size = new Size(33, 30);
            bookmarkLineButton.TabIndex = 26;
            bookmarkLineButton.Click += BookmarkLineButtonClick;
            // 
            // bookmarkLineLabel
            // 
            bookmarkLineLabel.AutoSize = true;
            bookmarkLineLabel.FlatStyle = FlatStyle.System;
            bookmarkLineLabel.Location = new Point(334, 120);
            bookmarkLineLabel.Name = "bookmarkLineLabel";
            bookmarkLineLabel.Size = new Size(114, 16);
            bookmarkLineLabel.TabIndex = 0;
            bookmarkLineLabel.Text = "Bookmark line back:";
            // 
            // bookmarkLineTextBox
            // 
            bookmarkLineTextBox.Location = new Point(334, 140);
            bookmarkLineTextBox.Name = "bookmarkLineTextBox";
            bookmarkLineTextBox.Size = new Size(103, 23);
            bookmarkLineTextBox.TabIndex = 25;
            bookmarkLineTextBox.TextChanged += EditorItemChanged;
            // 
            // errorLineButton
            // 
            errorLineButton.Location = new Point(126, 236);
            errorLineButton.Name = "errorLineButton";
            errorLineButton.Size = new Size(33, 30);
            errorLineButton.TabIndex = 10;
            errorLineButton.Click += ErrorLineButtonClick;
            // 
            // errorLineLabel
            // 
            errorLineLabel.AutoSize = true;
            errorLineLabel.FlatStyle = FlatStyle.System;
            errorLineLabel.Location = new Point(14, 220);
            errorLineLabel.Name = "errorLineLabel";
            errorLineLabel.Size = new Size(89, 16);
            errorLineLabel.TabIndex = 0;
            errorLineLabel.Text = "Error line back:";
            // 
            // errorLineTextBox
            // 
            errorLineTextBox.Location = new Point(14, 240);
            errorLineTextBox.Name = "errorLineTextBox";
            errorLineTextBox.Size = new Size(103, 23);
            errorLineTextBox.TabIndex = 9;
            errorLineTextBox.TextChanged += EditorItemChanged;
            // 
            // debugLineButton
            // 
            debugLineButton.Location = new Point(286, 236);
            debugLineButton.Name = "debugLineButton";
            debugLineButton.Size = new Size(33, 30);
            debugLineButton.TabIndex = 20;
            debugLineButton.Click += DebugLineButtonClick;
            // 
            // debugLineLabel
            // 
            debugLineLabel.AutoSize = true;
            debugLineLabel.FlatStyle = FlatStyle.System;
            debugLineLabel.Location = new Point(174, 220);
            debugLineLabel.Name = "debugLineLabel";
            debugLineLabel.Size = new Size(96, 16);
            debugLineLabel.TabIndex = 0;
            debugLineLabel.Text = "Debug line back:";
            // 
            // debugLineTextBox
            // 
            debugLineTextBox.Location = new Point(174, 240);
            debugLineTextBox.Name = "debugLineTextBox";
            debugLineTextBox.Size = new Size(103, 23);
            debugLineTextBox.TabIndex = 19;
            debugLineTextBox.TextChanged += EditorItemChanged;
            // 
            // disabledLineButton
            // 
            disabledLineButton.Location = new Point(446, 236);
            disabledLineButton.Name = "disabledLineButton";
            disabledLineButton.Size = new Size(33, 30);
            disabledLineButton.TabIndex = 30;
            disabledLineButton.Click += DisabledLineButtonClick;
            // 
            // disabledLineLabel
            // 
            disabledLineLabel.AutoSize = true;
            disabledLineLabel.FlatStyle = FlatStyle.System;
            disabledLineLabel.Location = new Point(334, 220);
            disabledLineLabel.Name = "disabledLineLabel";
            disabledLineLabel.Size = new Size(107, 16);
            disabledLineLabel.TabIndex = 0;
            disabledLineLabel.Text = "Disabled line back:";
            // 
            // disabledLineTextBox
            // 
            disabledLineTextBox.Location = new Point(334, 240);
            disabledLineTextBox.Name = "disabledLineTextBox";
            disabledLineTextBox.Size = new Size(103, 23);
            disabledLineTextBox.TabIndex = 29;
            disabledLineTextBox.TextChanged += EditorItemChanged;
            // 
            // highlightWordBackButton
            // 
            highlightWordBackButton.Location = new Point(126, 286);
            highlightWordBackButton.Name = "highlightWordBackButton";
            highlightWordBackButton.Size = new Size(33, 30);
            highlightWordBackButton.TabIndex = 24;
            highlightWordBackButton.Click += HighlightWordBackButtonClick;
            // 
            // highlightWordBackLabel
            // 
            highlightWordBackLabel.AutoSize = true;
            highlightWordBackLabel.FlatStyle = FlatStyle.System;
            highlightWordBackLabel.Location = new Point(14, 270);
            highlightWordBackLabel.Name = "highlightWordBackLabel";
            highlightWordBackLabel.Size = new Size(88, 16);
            highlightWordBackLabel.TabIndex = 0;
            highlightWordBackLabel.Text = "Highlight word back:";
            // 
            // highlightWordBackTextBox
            // 
            highlightWordBackTextBox.Location = new Point(14, 290);
            highlightWordBackTextBox.Name = "highlightWordBackTextBox";
            highlightWordBackTextBox.Size = new Size(103, 23);
            highlightWordBackTextBox.TabIndex = 23;
            highlightWordBackTextBox.TextChanged += EditorItemChanged;
            // 
            // colorizeCheckBox
            // 
            colorizeCheckBox.AutoSize = true;
            colorizeCheckBox.Checked = true;
            colorizeCheckBox.CheckState = CheckState.Indeterminate;
            colorizeCheckBox.Location = new Point(14, 325);
            colorizeCheckBox.Name = "italicsCheckBox";
            colorizeCheckBox.Size = new Size(58, 20);
            colorizeCheckBox.TabIndex = 12;
            colorizeCheckBox.Text = "Colorize first margin with 'gdefault' foreground color";
            colorizeCheckBox.ThreeState = true;
            colorizeCheckBox.UseVisualStyleBackColor = true;
            colorizeCheckBox.CheckedChanged += EditorItemChanged;
            // 
            // languageDropDown
            // 
            languageDropDown.DropDownStyle = ComboBoxStyle.DropDownList;
            languageDropDown.Location = new Point(14, 15);
            languageDropDown.MaxLength = 200;
            languageDropDown.Name = "languageDropDown";
            languageDropDown.Size = new Size(212, 24);
            languageDropDown.TabIndex = 4;
            languageDropDown.SelectedIndexChanged += LanguagesSelectedIndexChanged;
            // 
            // EditorDialog
            // 
            AcceptButton = okButton;
            AutoScaleDimensions = new SizeF(7F, 16F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = cancelButton;
            ClientSize = new Size(746, 630);
            Controls.Add(languageGroupBox);
            Controls.Add(itemGroupBox);
            Controls.Add(languageDropDown);
            Controls.Add(itemListView);
            Controls.Add(revertButton);
            Controls.Add(exportButton);
            Controls.Add(defaultButton);
            Controls.Add(cancelButton);
            Controls.Add(applyButton);
            Controls.Add(okButton);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "EditorDialog";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = " Editor Coloring";
            itemGroupBox.ResumeLayout(false);
            itemGroupBox.PerformLayout();
            languageGroupBox.ResumeLayout(false);
            languageGroupBox.PerformLayout();
            ResumeLayout(false);

        }

        #endregion

        #region Methods And Event Handlers

        /// <summary>
        /// Gets the path to the language directory
        /// </summary>
        string LangDir => Path.Combine(PathHelper.SettingDir, "Languages");

        /// <summary>
        /// Constant xml file style paths
        /// </summary>
        const string stylePath = "Scintilla/languages/language/use-styles/style";

        const string editorStylePath = "Scintilla/languages/language/editor-style";
        const string defaultStylePath = "Scintilla/languages/language/use-styles/style[@name='default']";

        /// <summary>
        /// Applies the localized texts to the form
        /// </summary>
        void ApplyLocalizedTexts()
        {
            ToolTip tooltip = new ToolTip();
            languageDropDown.Font = PluginBase.Settings.DefaultFont;
            fontSizeComboBox.FlatStyle = PluginBase.Settings.ComboBoxFlatStyle;
            fontNameComboBox.FlatStyle = PluginBase.Settings.ComboBoxFlatStyle;
            languageDropDown.FlatStyle = PluginBase.Settings.ComboBoxFlatStyle;
            tooltip.SetToolTip(exportButton, TextHelper.GetString("Label.ExportFiles"));
            tooltip.SetToolTip(revertButton, TextHelper.GetString("Label.RevertFiles"));
            tooltip.SetToolTip(defaultButton, TextHelper.GetString("Label.MakeAsDefault"));
            saveFileDialog.Filter = TextHelper.GetString("Info.ZipFilter");
            Text = " " + TextHelper.GetString("Title.SyntaxEditDialog");
            boldCheckBox.Text = TextHelper.GetString("Info.Bold");
            italicsCheckBox.Text = TextHelper.GetString("Info.Italic");
            itemGroupBox.Text = TextHelper.GetString("Info.ItemStyle");
            languageGroupBox.Text = TextHelper.GetString("Info.EditorStyle");
            foregroundLabel.Text = TextHelper.GetString("Info.Foreground");
            backgroundLabel.Text = TextHelper.GetString("Info.Background");
            sampleTextLabel.Text = TextHelper.GetString("Info.SampleText");
            caretForeLabel.Text = TextHelper.GetString("Info.CaretFore");
            caretlineBackLabel.Text = TextHelper.GetString("Info.CaretLineBack");
            selectionBackLabel.Text = TextHelper.GetString("Info.SelectionBack");
            selectionForeLabel.Text = TextHelper.GetString("Info.SelectionFore");
            marginForeLabel.Text = TextHelper.GetString("Info.MarginFore");
            marginBackLabel.Text = TextHelper.GetString("Info.MarginBack");
            markerBackLabel.Text = TextHelper.GetString("Info.MarkerBack");
            markerForeLabel.Text = TextHelper.GetString("Info.MarkerFore");
            printMarginLabel.Text = TextHelper.GetString("Info.PrintMargin");
            highlightBackLabel.Text = TextHelper.GetString("Info.HighlightBack");
            highlightWordBackLabel.Text = TextHelper.GetString("Info.HighlightWordBack");
            modifiedLineLabel.Text = TextHelper.GetString("Info.ModifiedLine");
            bookmarkLineLabel.Text = TextHelper.GetString("Info.BookmarkLine");
            errorLineLabel.Text = TextHelper.GetString("Info.ErrorLineBack");
            debugLineLabel.Text = TextHelper.GetString("Info.DebugLineBack");
            disabledLineLabel.Text = TextHelper.GetString("Info.DisabledLineBack");
            colorizeCheckBox.Text = TextHelper.GetString("Info.ColorizeMarkerMargin");
            cancelButton.Text = TextHelper.GetString("Label.Cancel");
            applyButton.Text = TextHelper.GetString("Label.Apply");
            fontLabel.Text = TextHelper.GetString("Info.Font");
            sizeLabel.Text = TextHelper.GetString("Info.Size");
            okButton.Text = TextHelper.GetString("Label.Ok");
            if (PluginBase.MainForm.StandaloneMode)
            {
                revertButton.Enabled = false;
            }
        }

        /// <summary>
        /// Initializes the graphics
        /// </summary>
        void InitializeGraphics()
        {
            ImageList imageList = new ImageList();
            imageList.ColorDepth = ColorDepth.Depth32Bit;
            imageList.Images.Add(PluginBase.MainForm.FindImage("129", false)); // snippet;
            imageList.Images.Add(PluginBase.MainForm.FindImage("328", false)); // palette;
            imageList.Images.Add(PluginBase.MainForm.FindImage("55|24|3|3", false)); // revert
            imageList.Images.Add(PluginBase.MainForm.FindImage("55|9|3|3", false)); // export
            imageList.Images.Add(PluginBase.MainForm.FindImage("55|25|3|3", false)); // default
            itemListView.SmallImageList = imageList;
            itemListView.SmallImageList.ImageSize = ScaleHelper.Scale(new Size(16, 16));
            revertButton.ImageList = exportButton.ImageList = imageList;
            disabledLineButton.ImageList = defaultButton.ImageList = imageList;
            foregroundButton.ImageList = backgroundButton.ImageList = imageList;
            caretForeButton.ImageList = caretlineBackButton.ImageList = imageList;
            selectionForeButton.ImageList = selectionBackButton.ImageList = imageList;
            marginBackButton.ImageList = marginForeButton.ImageList = imageList;
            markerBackButton.ImageList = markerForeButton.ImageList = imageList;
            printMarginButton.ImageList = highlightBackButton.ImageList = imageList;
            modifiedLineButton.ImageList = bookmarkLineButton.ImageList = imageList;
            errorLineButton.ImageList = debugLineButton.ImageList = imageList;
            highlightWordBackButton.ImageList = imageList;
        }

        /// <summary>
        /// Apply styling after theme manager
        /// </summary>
        public void AfterTheming()
        {
            UpdateSampleText();
        }

        /// <summary>
        /// Initializes all ui components
        /// </summary>
        void PopulateControls()
        {
            revertButton.ImageIndex = 2;
            exportButton.ImageIndex = 3;
            defaultButton.ImageIndex = 4;
            foregroundButton.ImageIndex = backgroundButton.ImageIndex = 1;
            caretForeButton.ImageIndex = caretlineBackButton.ImageIndex = 1;
            selectionForeButton.ImageIndex = selectionBackButton.ImageIndex = 1;
            marginBackButton.ImageIndex = marginForeButton.ImageIndex = 1;
            markerBackButton.ImageIndex = markerForeButton.ImageIndex = 1;
            printMarginButton.ImageIndex = highlightBackButton.ImageIndex = 1;
            modifiedLineButton.ImageIndex = bookmarkLineButton.ImageIndex = 1;
            disabledLineButton.ImageIndex = highlightWordBackButton.ImageIndex = 1;
            errorLineButton.ImageIndex = debugLineButton.ImageIndex = 1; 
            string[] languageFiles = Directory.GetFiles(LangDir, "*.xml");
            foreach (string language in languageFiles)
            {
                string languageName = Path.GetFileNameWithoutExtension(language);
                languageDropDown.Items.Add(languageName);
            }
            InstalledFontCollection fonts = new InstalledFontCollection();
            fontNameComboBox.Items.Add("");
            foreach (FontFamily font in fonts.Families)
            {
                fontNameComboBox.Items.Add(font.GetName(1033));
            }
            bool foundSyntax = false;
            string curSyntax = ArgsProcessor.GetCurSyntax();
            foreach (object item in languageDropDown.Items)
            {
                if (item.ToString().ToLower() == curSyntax)
                {
                    languageDropDown.SelectedItem = item;
                    foundSyntax = true;
                    break;
                }
            }
            if (!foundSyntax) languageDropDown.SelectedIndex = 0;
            columnHeader.Width = -2;
        }

        /// <summary>
        /// Loads language to be edited
        /// </summary>
        void LoadLanguage(string newLanguage, bool promptToSave)
        {
            if (!isLanguageSaved && promptToSave)
            {
                PromptToSaveLanguage();
            }
            languageDoc = new XmlDocument();
            languageFile = Path.Combine(LangDir, newLanguage + ".xml");
            languageDoc.Load(languageFile);
            LoadEditorStyles();
            defaultStyleNode = languageDoc.SelectSingleNode(defaultStylePath) as XmlElement;
            XmlNodeList styles = languageDoc.SelectNodes(stylePath);
            itemListView.Items.Clear();
            foreach (XmlNode style in styles)
            {
                itemListView.Items.Add(style.Attributes["name"].Value, 0);
            }
            if (itemListView.Items.Count > 0)
            {
                itemListView.Items[0].Selected = true;
            }
            applyButton.Enabled = false;
            isLanguageSaved = true;
        }

        /// <summary>
        /// Loads the language item
        /// </summary>
        void LoadLanguageItem(string item)
        {
            if (!isItemSaved) SaveCurrentItem();
            isLoadingItem = true;
            currentStyleNode = languageDoc.SelectSingleNode(stylePath + "[@name=\"" + item + "\"]") as XmlElement;
            fontNameComboBox.SelectedIndex = 0;
            fontSizeComboBox.Text = "";
            foregroundTextBox.Text = "";
            backgroundTextBox.Text = "";
            boldCheckBox.CheckState = CheckState.Indeterminate;
            italicsCheckBox.CheckState = CheckState.Indeterminate;
            if (currentStyleNode.Attributes["font"] != null)
            {
                string[] fonts = currentStyleNode.Attributes["font"].Value.Split(',');
                foreach (string font in fonts)
                {
                    if (IsFontInstalled(font))
                    {
                        fontNameComboBox.Text = font;
                        break;
                    }
                }
            }
            if (currentStyleNode.Attributes["size"] != null)
            {
                fontSizeComboBox.Text = currentStyleNode.Attributes["size"].Value;
            }
            if (currentStyleNode.Attributes["fore"] != null)
            {
                foregroundTextBox.Text = currentStyleNode.Attributes["fore"].Value;
            }
            if (currentStyleNode.Attributes["back"] != null)
            {
                backgroundTextBox.Text = currentStyleNode.Attributes["back"].Value;
            }
            if (currentStyleNode.Attributes["bold"] != null)
            {
                boldCheckBox.CheckState = CheckState.Unchecked;
                boldCheckBox.Checked = bool.Parse(currentStyleNode.Attributes["bold"].Value);
            }
            if (currentStyleNode.Attributes["italics"] != null)
            {
                italicsCheckBox.CheckState = CheckState.Unchecked;
                italicsCheckBox.Checked = bool.Parse(currentStyleNode.Attributes["italics"].Value);
            }
            UpdateSampleText();
            isLoadingItem = false;
            isItemSaved = true;
        }

        /// <summary>
        /// Checks if font is installed
        /// </summary>
        static bool IsFontInstalled(string fontName)
        {
            using var testFont = new Font(fontName, 9);
            return fontName.Equals(testFont.Name, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Saves the current item being edited
        /// </summary>
        void SaveCurrentItem()
        {
            if (fontNameComboBox.Text.Length != 0) currentStyleNode.SetAttribute("font", fontNameComboBox.Text);
            else currentStyleNode.RemoveAttribute("font");
            if (fontSizeComboBox.Text.Length != 0) currentStyleNode.SetAttribute("size", fontSizeComboBox.Text);
            else currentStyleNode.RemoveAttribute("size");
            if (foregroundTextBox.Text.Length != 0) currentStyleNode.SetAttribute("fore", foregroundTextBox.Text);
            else currentStyleNode.RemoveAttribute("fore");
            if (backgroundTextBox.Text.Length != 0) currentStyleNode.SetAttribute("back", backgroundTextBox.Text);
            else currentStyleNode.RemoveAttribute("back");
            if (boldCheckBox.CheckState == CheckState.Checked) currentStyleNode.SetAttribute("bold", "true");
            else if (boldCheckBox.CheckState == CheckState.Unchecked) currentStyleNode.SetAttribute("bold", "false");
            else currentStyleNode.RemoveAttribute("bold");
            if (italicsCheckBox.CheckState == CheckState.Checked) currentStyleNode.SetAttribute("italics", "true");
            else if (italicsCheckBox.CheckState == CheckState.Unchecked) currentStyleNode.SetAttribute("italics", "false");
            else currentStyleNode.RemoveAttribute("italics");
            isItemSaved = true;
        }
        
        /// <summary>
        /// Load the editor style items
        /// </summary>
        void LoadEditorStyles()
        {
            isLoadingEditor = true;
            caretForeTextBox.Text = "";
            caretlineBackTextBox.Text = "";
            selectionBackTextBox.Text = "";
            selectionForeTextBox.Text = "";
            markerForeTextBox.Text = "";
            markerBackTextBox.Text = "";
            marginForeTextBox.Text = "";
            marginBackTextBox.Text = "";
            printMarginTextBox.Text = "";
            highlightBackTextBox.Text = "";
            highlightWordBackTextBox.Text = "";
            modifiedLineTextBox.Text = "";
            bookmarkLineTextBox.Text = "";
            errorLineTextBox.Text = "";
            debugLineTextBox.Text = "";
            disabledLineTextBox.Text = "";
            colorizeCheckBox.CheckState = CheckState.Indeterminate;
            editorStyleNode = languageDoc.SelectSingleNode(editorStylePath) as XmlElement;
            if (editorStyleNode.Attributes["caret-fore"] != null)
            {
                caretForeTextBox.Text = editorStyleNode.Attributes["caret-fore"].Value;
            }
            if (editorStyleNode.Attributes["caretline-back"] != null)
            {
                caretlineBackTextBox.Text = editorStyleNode.Attributes["caretline-back"].Value;
            }
            if (editorStyleNode.Attributes["selection-back"] != null)
            {
                selectionBackTextBox.Text = editorStyleNode.Attributes["selection-back"].Value;
            }
            if (editorStyleNode.Attributes["selection-fore"] != null)
            {
                selectionForeTextBox.Text = editorStyleNode.Attributes["selection-fore"].Value;
            }
            if (editorStyleNode.Attributes["margin-fore"] != null)
            {
                marginForeTextBox.Text = editorStyleNode.Attributes["margin-fore"].Value;
            }
            if (editorStyleNode.Attributes["margin-back"] != null)
            {
                marginBackTextBox.Text = editorStyleNode.Attributes["margin-back"].Value;
            }
            if (editorStyleNode.Attributes["marker-fore"] != null)
            {
                markerForeTextBox.Text = editorStyleNode.Attributes["marker-fore"].Value;
            }
            if (editorStyleNode.Attributes["marker-back"] != null)
            {
                markerBackTextBox.Text = editorStyleNode.Attributes["marker-back"].Value;
            }
            if (editorStyleNode.Attributes["print-margin"] != null)
            {
                printMarginTextBox.Text = editorStyleNode.Attributes["print-margin"].Value;
            }
            if (editorStyleNode.Attributes["highlight-back"] != null)
            {
                highlightBackTextBox.Text = editorStyleNode.Attributes["highlight-back"].Value;
            }
            if (editorStyleNode.Attributes["highlightword-back"] != null)
            {
                highlightWordBackTextBox.Text = editorStyleNode.Attributes["highlightword-back"].Value;
            }
            if (editorStyleNode.Attributes["modifiedline-back"] != null)
            {
                modifiedLineTextBox.Text = editorStyleNode.Attributes["modifiedline-back"].Value;
            }
            if (editorStyleNode.Attributes["bookmarkline-back"] != null)
            {
                bookmarkLineTextBox.Text = editorStyleNode.Attributes["bookmarkline-back"].Value;
            }
            if (editorStyleNode.Attributes["errorline-back"] != null)
            {
                errorLineTextBox.Text = editorStyleNode.Attributes["errorline-back"].Value;
            }
            if (editorStyleNode.Attributes["debugline-back"] != null)
            {
                debugLineTextBox.Text = editorStyleNode.Attributes["debugline-back"].Value;
            }
            if (editorStyleNode.Attributes["disabledline-back"] != null)
            {
                disabledLineTextBox.Text = editorStyleNode.Attributes["disabledline-back"].Value;
            }
            if (editorStyleNode.Attributes["colorize-marker-back"] != null)
            {
                colorizeCheckBox.CheckState = CheckState.Unchecked;
                colorizeCheckBox.Checked = bool.Parse(editorStyleNode.Attributes["colorize-marker-back"].Value);
            }
            isLoadingEditor = false;
            isEditorSaved = true;
        }

        /// <summary>
        /// Saves the editor style items
        /// </summary>
        void SaveEditorStyles()
        {
            if (caretForeTextBox.Text != "") editorStyleNode.SetAttribute("caret-fore", caretForeTextBox.Text);
            else editorStyleNode.RemoveAttribute("caret-fore");
            if (caretlineBackTextBox.Text != "") editorStyleNode.SetAttribute("caretline-back", caretlineBackTextBox.Text);
            else editorStyleNode.RemoveAttribute("caretline-back");
            if (selectionForeTextBox.Text != "") editorStyleNode.SetAttribute("selection-fore", selectionForeTextBox.Text);
            else editorStyleNode.RemoveAttribute("selection-fore");
            if (selectionBackTextBox.Text != "") editorStyleNode.SetAttribute("selection-back", selectionBackTextBox.Text);
            else editorStyleNode.RemoveAttribute("selection-back");
            if (marginForeTextBox.Text != "") editorStyleNode.SetAttribute("margin-fore", marginForeTextBox.Text);
            else editorStyleNode.RemoveAttribute("margin-fore");
            if (marginBackTextBox.Text != "") editorStyleNode.SetAttribute("margin-back", marginBackTextBox.Text);
            else editorStyleNode.RemoveAttribute("margin-back");
            if (markerForeTextBox.Text != "") editorStyleNode.SetAttribute("marker-fore", markerForeTextBox.Text);
            else editorStyleNode.RemoveAttribute("marker-fore");
            if (markerBackTextBox.Text != "") editorStyleNode.SetAttribute("marker-back", markerBackTextBox.Text);
            else editorStyleNode.RemoveAttribute("marker-back");
            if (printMarginTextBox.Text != "") editorStyleNode.SetAttribute("print-margin", printMarginTextBox.Text);
            else editorStyleNode.RemoveAttribute("print-margin");
            if (highlightBackTextBox.Text != "") editorStyleNode.SetAttribute("highlight-back", highlightBackTextBox.Text);
            else editorStyleNode.RemoveAttribute("highlight-back");
            if (highlightWordBackTextBox.Text != "") editorStyleNode.SetAttribute("highlightword-back", highlightWordBackTextBox.Text);
            else editorStyleNode.RemoveAttribute("highlightword-back");
            if (modifiedLineTextBox.Text != "") editorStyleNode.SetAttribute("modifiedline-back", modifiedLineTextBox.Text);
            else editorStyleNode.RemoveAttribute("modifiedline-back");
            if (bookmarkLineTextBox.Text != "") editorStyleNode.SetAttribute("bookmarkline-back", bookmarkLineTextBox.Text);
            else editorStyleNode.RemoveAttribute("bookmarkline-back");
            if (errorLineTextBox.Text != "") editorStyleNode.SetAttribute("errorline-back", errorLineTextBox.Text);
            else editorStyleNode.RemoveAttribute("errorline-back");
            if (debugLineTextBox.Text != "") editorStyleNode.SetAttribute("debugline-back", debugLineTextBox.Text);
            else editorStyleNode.RemoveAttribute("debugline-back");
            if (disabledLineTextBox.Text != "") editorStyleNode.SetAttribute("disabledline-back", disabledLineTextBox.Text);
            else editorStyleNode.RemoveAttribute("disabledline-back");
            if (colorizeCheckBox.CheckState == CheckState.Checked) editorStyleNode.SetAttribute("colorize-marker-back", "true");
            else if (colorizeCheckBox.CheckState == CheckState.Unchecked) editorStyleNode.SetAttribute("colorize-marker-back", "false");
            else editorStyleNode.RemoveAttribute("colorize-marker-back");
            isEditorSaved = true;
        }

        /// <summary>
        /// Updates the Sample Item from settings in dialog
        /// </summary>
        void UpdateSampleText()
        {
            try
            {
                FontStyle fs = FontStyle.Regular;
                string fontName = fontNameComboBox.Text;
                if (fontName.Length == 0) fontName = defaultStyleNode.Attributes["font"].Value;
                string fontSize = fontSizeComboBox.Text;
                if (fontSize.Length == 0) fontSize = defaultStyleNode.Attributes["size"].Value;
                string foreColor = foregroundTextBox.Text;
                if (foreColor.Length == 0) foreColor = defaultStyleNode.Attributes["fore"].Value;
                string backColor = backgroundTextBox.Text;
                if (backColor.Length == 0) backColor = defaultStyleNode.Attributes["back"].Value;
                if (boldCheckBox.CheckState == CheckState.Checked) fs |= FontStyle.Bold;
                else if (boldCheckBox.CheckState == CheckState.Indeterminate)
                {
                    if (defaultStyleNode.Attributes["bold"] != null)
                    {
                        if (defaultStyleNode.Attributes["bold"].Value == "true") fs |= FontStyle.Bold;
                    }
                }
                if (italicsCheckBox.CheckState == CheckState.Checked) fs |= FontStyle.Italic;
                else if (italicsCheckBox.CheckState == CheckState.Indeterminate)
                {
                    if (defaultStyleNode.Attributes["italics"] != null)
                    {
                        if (defaultStyleNode.Attributes["italics"].Value == "true") fs |= FontStyle.Italic;
                    }
                }
                sampleTextLabel.Text = TextHelper.GetString("Info.SampleText");
                sampleTextLabel.Font = new Font(fontName, float.Parse(fontSize), fs);
                sampleTextLabel.ForeColor = ColorTranslator.FromHtml(foreColor);
                sampleTextLabel.BackColor = ColorTranslator.FromHtml(backColor);
            }
            catch (Exception)
            {
                sampleTextLabel.Font = PluginBase.Settings.ConsoleFont;
                sampleTextLabel.Text = "Preview not available...";
            }
        }

        /// <summary>
        /// Asks the user to save the changes
        /// </summary>
        void PromptToSaveLanguage()
        {
            string message = TextHelper.GetString("Info.SaveCurrentLanguage");
            string caption = TextHelper.GetString("FlashDevelop.Title.ConfirmDialog");
            if (MessageBox.Show(message, caption, MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                SaveCurrentLanguage();
            }
        }

        /// <summary>
        /// After item has been changed, update controls
        /// </summary>
        void ItemsSelectedIndexChanged(object sender, EventArgs e)
        {
            if (itemListView.SelectedIndices.Count > 0)
            {
                string style = itemListView.SelectedItems[0].Text;
                LoadLanguageItem(style);
            }
        }

        /// <summary>
        /// When color has been selected, update controls
        /// </summary>
        void ItemForegroundButtonClick(object sender, EventArgs e)
        {
            colorDialog.Color = ColorTranslator.FromHtml(foregroundTextBox.Text);
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                foregroundTextBox.Text = "0x" + colorDialog.Color.ToArgb().ToString("X8").Substring(2, 6);
            }
        }

        /// <summary>
        /// When color has been selected, update controls
        /// </summary>
        void ItemBackgroundButtonClick(object sender, EventArgs e)
        {
            colorDialog.Color = ColorTranslator.FromHtml(backgroundTextBox.Text);
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                backgroundTextBox.Text = "0x" + colorDialog.Color.ToArgb().ToString("X8").Substring(2, 6);
            }
        }

        /// <summary>
        /// When color has been selected, update controls
        /// </summary>
        void SelectionForeButtonClick(object sender, EventArgs e)
        {
            colorDialog.Color = ColorTranslator.FromHtml(selectionForeTextBox.Text);
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                selectionForeTextBox.Text = "0x" + colorDialog.Color.ToArgb().ToString("X8").Substring(2, 6);
            }
        }

        /// <summary>
        /// When color has been selected, update controls
        /// </summary>
        void SelectionBackButtonClick(object sender, EventArgs e)
        {
            colorDialog.Color = ColorTranslator.FromHtml(selectionBackTextBox.Text);
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                selectionBackTextBox.Text = "0x" + colorDialog.Color.ToArgb().ToString("X8").Substring(2, 6);
            }
        }

        /// <summary>
        /// When color has been selected, update controls
        /// </summary>
        void CaretlineBackButtonClick(object sender, EventArgs e)
        {
            colorDialog.Color = ColorTranslator.FromHtml(caretlineBackTextBox.Text);
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                caretlineBackTextBox.Text = "0x" + colorDialog.Color.ToArgb().ToString("X8").Substring(2, 6);
            }
        }

        /// <summary>
        /// When color has been selected, update controls
        /// </summary>
        void CaretForeButtonClick(object sender, EventArgs e)
        {
            colorDialog.Color = ColorTranslator.FromHtml(caretForeTextBox.Text);
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                caretForeTextBox.Text = "0x" + colorDialog.Color.ToArgb().ToString("X8").Substring(2, 6);
            }
        }

        /// <summary>
        /// When color has been selected, update controls
        /// </summary>
        void MarkerBackButtonClick(object sender, EventArgs e)
        {
            colorDialog.Color = ColorTranslator.FromHtml(markerBackTextBox.Text);
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                markerBackTextBox.Text = "0x" + colorDialog.Color.ToArgb().ToString("X8").Substring(2, 6);
            }
        }

        /// <summary>
        /// When color has been selected, update controls
        /// </summary>
        void MarkerForeButtonClick(object sender, EventArgs e)
        {
            colorDialog.Color = ColorTranslator.FromHtml(markerForeTextBox.Text);
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                markerForeTextBox.Text = "0x" + colorDialog.Color.ToArgb().ToString("X8").Substring(2, 6);
            }
        }
        
        /// <summary>
        /// When color has been selected, update controls
        /// </summary>
        void MarginBackButtonClick(object sender, EventArgs e)
        {
            colorDialog.Color = ColorTranslator.FromHtml(marginForeTextBox.Text);
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                marginBackTextBox.Text = "0x" + colorDialog.Color.ToArgb().ToString("X8").Substring(2, 6);
            }
        }
        
        /// <summary>
        /// When color has been selected, update controls
        /// </summary>
        void MarginForeButtonClick(object sender, EventArgs e)
        {
            colorDialog.Color = ColorTranslator.FromHtml(marginForeTextBox.Text);
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                marginForeTextBox.Text = "0x" + colorDialog.Color.ToArgb().ToString("X8").Substring(2, 6);
            }
        }

        /// <summary>
        /// When color has been selected, update controls
        /// </summary>
        void PrintMarginButtonClick(object sender, EventArgs e)
        {
            colorDialog.Color = ColorTranslator.FromHtml(printMarginTextBox.Text);
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                printMarginTextBox.Text = "0x" + colorDialog.Color.ToArgb().ToString("X8").Substring(2, 6);
            }
        }

        /// <summary>
        /// When color has been selected, update controls
        /// </summary>
        void HighlightBackButtonClick(object sender, EventArgs e)
        {
            colorDialog.Color = ColorTranslator.FromHtml(highlightBackTextBox.Text);
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                highlightBackTextBox.Text = "0x" + colorDialog.Color.ToArgb().ToString("X8").Substring(2, 6);
            }
        }

        /// <summary>
        /// When color has been selected, update controls
        /// </summary>
        void HighlightWordBackButtonClick(object sender, EventArgs e)
        {
            colorDialog.Color = ColorTranslator.FromHtml(highlightWordBackTextBox.Text);
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                highlightWordBackTextBox.Text = "0x" + colorDialog.Color.ToArgb().ToString("X8").Substring(2, 6);
            }
        }

        /// <summary>
        /// When color has been selected, update controls
        /// </summary>
        void ModifiedLineButtonClick(object sender, EventArgs e)
        {
            colorDialog.Color = ColorTranslator.FromHtml(modifiedLineTextBox.Text);
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                modifiedLineTextBox.Text = "0x" + colorDialog.Color.ToArgb().ToString("X8").Substring(2, 6);
            }
        }

        /// <summary>
        /// When color has been selected, update controls
        /// </summary>
        void BookmarkLineButtonClick(object sender, EventArgs e)
        {
            colorDialog.Color = ColorTranslator.FromHtml(bookmarkLineTextBox.Text);
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                bookmarkLineTextBox.Text = "0x" + colorDialog.Color.ToArgb().ToString("X8").Substring(2, 6);
            }
        }

        /// <summary>
        /// When color has been selected, update controls
        /// </summary>
        void ErrorLineButtonClick(object sender, EventArgs e)
        {
            colorDialog.Color = ColorTranslator.FromHtml(errorLineTextBox.Text);
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                errorLineTextBox.Text = "0x" + colorDialog.Color.ToArgb().ToString("X8").Substring(2, 6);
            }
        }

        /// <summary>
        /// When color has been selected, update controls
        /// </summary>
        void DebugLineButtonClick(object sender, EventArgs e)
        {
            colorDialog.Color = ColorTranslator.FromHtml(debugLineTextBox.Text);
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                debugLineTextBox.Text = "0x" + colorDialog.Color.ToArgb().ToString("X8").Substring(2, 6);
            }
        }

        /// <summary>
        /// When color has been selected, update controls
        /// </summary>
        void DisabledLineButtonClick(object sender, EventArgs e)
        {
            colorDialog.Color = ColorTranslator.FromHtml(disabledLineTextBox.Text);
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                disabledLineTextBox.Text = "0x" + colorDialog.Color.ToArgb().ToString("X8").Substring(2, 6);
            }
        }

        /// <summary>
        /// When style item has been changed, update controls
        /// </summary>
        void LanguageItemChanged(object sender, EventArgs e)
        {
            if (!isLoadingItem)
            {
                isItemSaved = false;
                isLanguageSaved = false;
                applyButton.Enabled = true;
                UpdateSampleText();
            }
        }

        /// <summary>
        /// When editor item has been changed, update controls
        /// </summary>
        void EditorItemChanged(object sender, EventArgs e)
        {
            if (!isLoadingEditor)
            {
                isLanguageSaved = false;
                applyButton.Enabled = true;
                isEditorSaved = false;
            }
        }

        /// <summary>
        /// Saves the current modified language
        /// </summary>
        void SaveCurrentLanguage()
        {
            if (!isItemSaved) SaveCurrentItem();
            if (!isEditorSaved) SaveEditorStyles();
            XmlTextWriter xmlWriter = new XmlTextWriter(languageFile, Encoding.UTF8);
            xmlWriter.Formatting = Formatting.Indented;
            xmlWriter.IndentChar = '\t';
            xmlWriter.Indentation = 1;
            languageDoc.Save(xmlWriter);
            applyButton.Enabled = false;
            isLanguageSaved = true;
            isEditorSaved = true;
            xmlWriter.Close();
        }

        /// <summary>
        /// After index has been changed, load the selected language
        /// </summary>
        void LanguagesSelectedIndexChanged(object sender, EventArgs e)
        {
            LoadLanguage(languageDropDown.Text, true);
        }

        /// <summary>
        /// Opens the revert settings dialog
        /// </summary>
        void RevertLanguagesClick(object sender, EventArgs e)
        {
            string caption = TextHelper.GetString("Title.ConfirmDialog");
            string message = TextHelper.GetString("Info.RevertSettingsFiles");
            DialogResult result = MessageBox.Show(message, caption, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);
            if (result == DialogResult.Yes)
            {
                Enabled = false;
                CleanupManager.RevertConfiguration(true);
                RefreshConfiguration();
                Enabled = true;
            }
            else if (result == DialogResult.No)
            {
                Enabled = false;
                CleanupManager.RevertConfiguration(false);
                RefreshConfiguration();
                Enabled = true;
            }
        }

        /// <summary>
        /// Refreshes the langugage configuration
        /// </summary>
        void RefreshConfiguration()
        {
            LoadLanguage(languageDropDown.Text, true);
            if (itemListView.SelectedIndices.Count > 0)
            {
                string language = itemListView.SelectedItems[0].Text;
                LoadLanguageItem(language);
            }
            PluginBase.MainForm.RefreshSciConfig();
        }

        /// <summary>
        /// Makes the current style as the default
        /// </summary>
        void MakeAsDefaultStyleClick(object sender, EventArgs e)
        {
            Enabled = false;
            isLanguageSaved = true;
            string[] confFiles = Directory.GetFiles(LangDir, "*.xml");
            foreach (string confFile in confFiles)
            {
                try
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(confFile);
                    XmlElement currentNode = doc.SelectSingleNode(defaultStylePath) as XmlElement;
                    XmlElement defaultNode = languageDoc.SelectSingleNode(defaultStylePath) as XmlElement;
                    // Save default style
                    if (defaultNode.Attributes["font"] != null) currentNode.SetAttribute("font", defaultNode.Attributes["font"].Value);
                    else currentNode.RemoveAttribute("font");
                    if (defaultNode.Attributes["size"] != null) currentNode.SetAttribute("size", defaultNode.Attributes["size"].Value);
                    else currentNode.RemoveAttribute("size");
                    if (defaultNode.Attributes["fore"] != null) currentNode.SetAttribute("fore", defaultNode.Attributes["fore"].Value);
                    else currentNode.RemoveAttribute("fore");
                    if (defaultNode.Attributes["back"] != null) currentNode.SetAttribute("back", defaultNode.Attributes["back"].Value);
                    else currentNode.RemoveAttribute("back");
                    if (defaultNode.Attributes["bold"] != null) currentNode.SetAttribute("bold", defaultNode.Attributes["bold"].Value);
                    else currentNode.RemoveAttribute("bold");
                    if (defaultNode.Attributes["italics"] != null) currentNode.SetAttribute("italics", defaultNode.Attributes["italics"].Value);
                    else currentNode.RemoveAttribute("italics");
                    // Save editor styles
                    currentNode = doc.SelectSingleNode(editorStylePath) as XmlElement;
                    if (caretForeTextBox.Text != "") currentNode.SetAttribute("caret-fore", caretForeTextBox.Text);
                    else currentNode.RemoveAttribute("caret-fore");
                    if (caretlineBackTextBox.Text != "") currentNode.SetAttribute("caretline-back", caretlineBackTextBox.Text);
                    else currentNode.RemoveAttribute("caretline-back");
                    if (selectionForeTextBox.Text != "") currentNode.SetAttribute("selection-fore", selectionForeTextBox.Text);
                    else currentNode.RemoveAttribute("selection-fore");
                    if (selectionBackTextBox.Text != "") currentNode.SetAttribute("selection-back", selectionBackTextBox.Text);
                    else currentNode.RemoveAttribute("selection-back");
                    if (marginForeTextBox.Text != "") currentNode.SetAttribute("margin-fore", marginForeTextBox.Text);
                    else currentNode.RemoveAttribute("margin-fore");
                    if (marginBackTextBox.Text != "") currentNode.SetAttribute("margin-back", marginBackTextBox.Text);
                    else currentNode.RemoveAttribute("margin-back");
                    if (markerForeTextBox.Text != "") currentNode.SetAttribute("marker-fore", markerForeTextBox.Text);
                    else currentNode.RemoveAttribute("marker-fore");
                    if (markerBackTextBox.Text != "") currentNode.SetAttribute("marker-back", markerBackTextBox.Text);
                    else currentNode.RemoveAttribute("marker-back");
                    if (printMarginTextBox.Text != "") currentNode.SetAttribute("print-margin", printMarginTextBox.Text);
                    else currentNode.RemoveAttribute("print-margin");
                    if (highlightBackTextBox.Text != "") currentNode.SetAttribute("highlight-back", highlightBackTextBox.Text);
                    else currentNode.RemoveAttribute("highlight-back");
                    if (highlightWordBackTextBox.Text != "") currentNode.SetAttribute("highlightword-back", highlightWordBackTextBox.Text);
                    else currentNode.RemoveAttribute("highlightword-back");
                    if (modifiedLineTextBox.Text != "") currentNode.SetAttribute("modifiedline-back", modifiedLineTextBox.Text);
                    else currentNode.RemoveAttribute("modifiedline-back");
                    if (bookmarkLineTextBox.Text != "") currentNode.SetAttribute("bookmarkline-back", bookmarkLineTextBox.Text);
                    else currentNode.RemoveAttribute("bookmarkline-back");
                    if (errorLineTextBox.Text != "") currentNode.SetAttribute("errorline-back", errorLineTextBox.Text);
                    else currentNode.RemoveAttribute("errorline-back");
                    if (debugLineTextBox.Text != "") currentNode.SetAttribute("debugline-back", debugLineTextBox.Text);
                    else currentNode.RemoveAttribute("debugline-back");
                    if (disabledLineTextBox.Text != "") currentNode.SetAttribute("disabledline-back", disabledLineTextBox.Text);
                    else currentNode.RemoveAttribute("disabledline-back");
                    if (colorizeCheckBox.CheckState == CheckState.Checked) currentNode.SetAttribute("colorize-marker-back", "true");
                    else if (colorizeCheckBox.CheckState == CheckState.Unchecked) currentNode.SetAttribute("colorize-marker-back", "false");
                    else currentNode.RemoveAttribute("colorize-marker-back");
                    // Save the file
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
            RefreshConfiguration();
            Enabled = true;
        }

        /// <summary>
        /// Opens the export settings dialog
        /// </summary>
        void ExportLanguagesClick(object sender, EventArgs e)
        {
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string[] langFiles = Directory.GetFiles(LangDir);
                ZipFile zipFile = ZipFile.Create(saveFileDialog.FileName);
                zipFile.BeginUpdate();
                foreach (string langFile in langFiles)
                {
                    var xmlFile = Path.GetFileName(langFile);
                    zipFile.Add(langFile, "$(BaseDir)\\Settings\\Languages\\" + xmlFile);
                }
                zipFile.CommitUpdate();
                zipFile.Close();
            }
        }

        /// <summary>
        /// Saves the current language
        /// </summary>
        void SaveButtonClick(object sender, EventArgs e)
        {
            SaveCurrentLanguage();
            PluginBase.MainForm.RefreshSciConfig();
        }

        /// <summary>
        /// Closes the dialog without saving
        /// </summary>
        void CancelButtonClick(object sender, EventArgs e) => Close();

        /// <summary>
        /// Closes the dialog and saves changes
        /// </summary>
        void OkButtonClick(object sender, EventArgs e)
        {
            if (!isLanguageSaved) SaveCurrentLanguage();
            PluginBase.MainForm.RefreshSciConfig();
            Close();
        }

        /// <summary>
        /// Shows the syntax edit dialog
        /// </summary>
        public new static void Show()
        {
            using var dialog = new EditorDialog();
            dialog.ShowDialog();
        }

        #endregion
    }
}