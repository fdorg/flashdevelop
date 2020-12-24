using System.ComponentModel;
using System.Windows.Forms;
using PluginCore;
using PluginCore.Localization;
using PluginCore.Controls;

namespace ProjectManager.Helpers
{
    /// <summary>
    /// A simple form where a user can enter a text string.
    /// </summary>
    public class LineEntryDialog : SmartForm
    {
        #region Form Designer Components

        TextBox lineBox;
        Button btnOK;
        Button btnCancel;
        /// <summary>
        /// Required designer variable.
        /// </summary>
        readonly Container components = null;

        Label titleLabel;

        #endregion

        /// <summary>
        /// Gets the line entered by the user.
        /// </summary>
        public string Line { get; private set; }

        public LineEntryDialog(string captionText, string labelText, string defaultLine)
        {
            InitializeComponent();
            InitializeLocalization();
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
        protected override void Dispose( bool disposing )
        {
            if (disposing) components?.Dispose();
            base.Dispose(disposing);
        }

        #endregion

        #region Windows Form Designer Generated Code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        void InitializeComponent()
        {
            titleLabel = new Label();
            lineBox = new TextBoxEx();
            btnOK = new ButtonEx();
            btnCancel = new ButtonEx();
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
            // btnOK
            // 
            btnOK.FlatStyle = FlatStyle.System;
            btnOK.Location = new System.Drawing.Point(68, 50);
            btnOK.Name = "btnOK";
            btnOK.Size = new System.Drawing.Size(72, 21);
            btnOK.TabIndex = 1;
            btnOK.Text = "OK";
            btnOK.Click += btnOK_Click;
            // 
            // btnCancel
            // 
            btnCancel.DialogResult = DialogResult.Cancel;
            btnCancel.FlatStyle = FlatStyle.System;
            btnCancel.Location = new System.Drawing.Point(149, 50);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new System.Drawing.Size(72, 21);
            btnCancel.TabIndex = 2;
            btnCancel.Text = "Cancel";
            btnCancel.Click += btnCancel_Click;
            // 
            // LineEntryDialog
            // 
            AcceptButton = btnOK;
            AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            CancelButton = btnCancel;
            ClientSize = new System.Drawing.Size(282, 81);
            Controls.Add(btnCancel);
            Controls.Add(btnOK);
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

        void InitializeLocalization()
        {
            btnOK.Text = TextHelper.GetString("Label.OK");
            btnCancel.Text = TextHelper.GetString("Label.Cancel");
            titleLabel.Text = TextHelper.GetString("Info.EnterText");
            Text = " " + TextHelper.GetString("Title.EnterText");
        }

        void btnOK_Click(object sender, System.EventArgs e)
        {
            Line = lineBox.Text;
            var cancelArgs = new CancelEventArgs(false);
            OnValidating(cancelArgs);
            if (!cancelArgs.Cancel)
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        void btnCancel_Click(object sender, System.EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        void OnLineBoxOnKeyDown(object sender, KeyEventArgs args)
        {
            var shortcutId = PluginBase.MainForm.GetShortcutItemId(args.KeyData);
            if (string.IsNullOrEmpty(shortcutId)) return;
            switch (shortcutId)
            {
                case "EditMenu.ToLowercase":
                case "EditMenu.ToUppercase":
                    var selectedText = lineBox.SelectedText;
                    if (string.IsNullOrEmpty(selectedText)) break;
                    selectedText = shortcutId == "EditMenu.ToLowercase" ? selectedText.ToLower() : selectedText.ToUpper();
                    var selectionStart = lineBox.SelectionStart;
                    var selectionLength = lineBox.SelectionLength;
                    lineBox.Paste(selectedText);
                    SelectRange(selectionStart, selectionLength);
                    break;
            }
        }

        public void SelectRange(int start, int length) => lineBox.Select(start, length);
    }
}