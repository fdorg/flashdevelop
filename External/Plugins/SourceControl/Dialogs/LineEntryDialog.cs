// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System.ComponentModel;
using System.Windows.Forms;
using PluginCore;
using PluginCore.Localization;

namespace SourceControl.Dialogs
{
    /// <summary>
    /// A simple form where a user can enter a text string.
    /// </summary>
    public class LineEntryDialog : Form
    {
        #region Form Designer Components

        private TextBox lineBox;
        private Button btnYes;
        private Button btnNo;
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private readonly Container components = null;
        private Button btnNever;
        private Label titleLabel;

        #endregion

        /// <summary>
        /// Gets the line entered by the user.
        /// </summary>
        public string Line { get; private set; }

        public LineEntryDialog(string captionText, string labelText, string defaultLine)
        {
            InitializeComponent();
            InititalizeLocalization();
            Font = PluginBase.Settings.DefaultFont;
            Text = " " + captionText;
            titleLabel.Text = labelText;
            lineBox.KeyDown += OnLineBoxOnKeyDown;
            lineBox.Text = defaultLine ?? string.Empty;
            lineBox.SelectAll();
            lineBox.Focus();
        }

        #region Dispose

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components?.Dispose();
            }
            base.Dispose( disposing );
        }

        #endregion

        #region Windows Form Designer Generated Code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            titleLabel = new Label();
            lineBox = new TextBox();
            btnYes = new Button();
            btnNo = new Button();
            btnNever = new Button();
            SuspendLayout();
            // 
            // titleLabel
            // 
            titleLabel.Location = new System.Drawing.Point(8, 8);
            titleLabel.Name = "titleLabel";
            titleLabel.Size = new System.Drawing.Size(266, 16);
            titleLabel.TabIndex = 3;
            titleLabel.Text = "Enter text:";
            // 
            // lineBox
            // 
            lineBox.Location = new System.Drawing.Point(10, 24);
            lineBox.Name = "lineBox";
            lineBox.Size = new System.Drawing.Size(260, 20);
            lineBox.TabIndex = 0;
            // 
            // btnYes
            // 
            btnYes.DialogResult = DialogResult.Yes;
            btnYes.FlatStyle = FlatStyle.System;
            btnYes.Location = new System.Drawing.Point(25, 50);
            btnYes.Name = "btnYes";
            btnYes.Size = new System.Drawing.Size(72, 21);
            btnYes.TabIndex = 1;
            btnYes.Text = "Yes";
            btnYes.Click += btnYes_Click;
            // 
            // btnNo
            // 
            btnNo.DialogResult = DialogResult.No;
            btnNo.FlatStyle = FlatStyle.System;
            btnNo.Location = new System.Drawing.Point(103, 50);
            btnNo.Name = "btnNo";
            btnNo.Size = new System.Drawing.Size(72, 21);
            btnNo.TabIndex = 2;
            btnNo.Text = "No";
            btnNo.Click += btnNo_Click;
            // 
            // btnNever
            // 
            btnNever.DialogResult = DialogResult.Cancel;
            btnNever.FlatStyle = FlatStyle.System;
            btnNever.Location = new System.Drawing.Point(181, 50);
            btnNever.Name = "btnNever";
            btnNever.Size = new System.Drawing.Size(72, 21);
            btnNever.TabIndex = 4;
            btnNever.Text = "Never";
            btnNever.Click += btnNever_Click;
            // 
            // LineEntryDialog
            // 
            AcceptButton = btnYes;
            AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            CancelButton = btnNo;
            ClientSize = new System.Drawing.Size(282, 81);
            Controls.Add(btnNever);
            Controls.Add(btnNo);
            Controls.Add(btnYes);
            Controls.Add(lineBox);
            Controls.Add(titleLabel);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "LineEntryDialog";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Enter Text";
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private void InititalizeLocalization()
        {
            btnYes.Text = TextHelper.GetString("Label.Yes");
            btnNo.Text = TextHelper.GetString("Label.No");
            btnNever.Text = TextHelper.GetString("Label.Never");
            titleLabel.Text = TextHelper.GetString("Info.EnterText");
            Text = " " + TextHelper.GetString("Title.EnterText");
        }

        private void btnYes_Click(object sender, System.EventArgs e)
        {
            Line = lineBox.Text;
            CancelEventArgs cancelArgs = new CancelEventArgs(false);
            OnValidating(cancelArgs);
            if (!cancelArgs.Cancel)
            {
                DialogResult = DialogResult.Yes;
                Close();
            }
        }

        private void btnNo_Click(object sender, System.EventArgs e)
        {
            DialogResult = DialogResult.No;
            Close();
        }

        private void btnNever_Click(object sender, System.EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        void OnLineBoxOnKeyDown(object sender, KeyEventArgs args)
        {
            string shortcutId = PluginBase.MainForm.GetShortcutItemId(args.KeyData);
            if (string.IsNullOrEmpty(shortcutId)) return;

            switch (shortcutId)
            {
                case "EditMenu.ToLowercase":
                case "EditMenu.ToUppercase":
                    string selectedText = lineBox.SelectedText;
                    if (string.IsNullOrEmpty(selectedText)) break;
                    selectedText = shortcutId == "EditMenu.ToLowercase" ? selectedText.ToLower() : selectedText.ToUpper();
                    int selectionStart = lineBox.SelectionStart;
                    int selectionLength = lineBox.SelectionLength;
                    lineBox.Paste(selectedText);
                    SelectRange(selectionStart, selectionLength);
                    break;
            }
        }

        public void SelectRange(int start, int length)
        {
            lineBox.Select(start, length);
        }
    }

}
