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
        string line;

        #region Form Designer Components

        private System.Windows.Forms.TextBox lineBox;
        private System.Windows.Forms.Button btnYes;
        private System.Windows.Forms.Button btnNo;
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;
        private Button btnNever;
        private System.Windows.Forms.Label titleLabel;

        #endregion

        /// <summary>
        /// Gets the line entered by the user.
        /// </summary>
        public string Line
        {
            get { return line; }
        }

        public LineEntryDialog(string captionText, string labelText, string defaultLine)
        {
            InitializeComponent();
            InititalizeLocalization();
            this.Font = PluginBase.Settings.DefaultFont;
            this.Text = " " + captionText;
            titleLabel.Text = labelText;
            lineBox.KeyDown += OnLineBoxOnKeyDown;
            lineBox.Text = (defaultLine != null) ? defaultLine : string.Empty;
            lineBox.SelectAll();
            lineBox.Focus();
        }

        #region Dispose

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose( bool disposing )
        {
            if( disposing )
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
            this.titleLabel = new System.Windows.Forms.Label();
            this.lineBox = new System.Windows.Forms.TextBox();
            this.btnYes = new System.Windows.Forms.Button();
            this.btnNo = new System.Windows.Forms.Button();
            this.btnNever = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // titleLabel
            // 
            this.titleLabel.Location = new System.Drawing.Point(8, 8);
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new System.Drawing.Size(266, 16);
            this.titleLabel.TabIndex = 3;
            this.titleLabel.Text = "Enter text:";
            // 
            // lineBox
            // 
            this.lineBox.Location = new System.Drawing.Point(10, 24);
            this.lineBox.Name = "lineBox";
            this.lineBox.Size = new System.Drawing.Size(260, 20);
            this.lineBox.TabIndex = 0;
            // 
            // btnYes
            // 
            this.btnYes.DialogResult = System.Windows.Forms.DialogResult.Yes;
            this.btnYes.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnYes.Location = new System.Drawing.Point(25, 50);
            this.btnYes.Name = "btnYes";
            this.btnYes.Size = new System.Drawing.Size(72, 21);
            this.btnYes.TabIndex = 1;
            this.btnYes.Text = "Yes";
            this.btnYes.Click += this.btnYes_Click;
            // 
            // btnNo
            // 
            this.btnNo.DialogResult = System.Windows.Forms.DialogResult.No;
            this.btnNo.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnNo.Location = new System.Drawing.Point(103, 50);
            this.btnNo.Name = "btnNo";
            this.btnNo.Size = new System.Drawing.Size(72, 21);
            this.btnNo.TabIndex = 2;
            this.btnNo.Text = "No";
            this.btnNo.Click += this.btnNo_Click;
            // 
            // btnNever
            // 
            this.btnNever.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnNever.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnNever.Location = new System.Drawing.Point(181, 50);
            this.btnNever.Name = "btnNever";
            this.btnNever.Size = new System.Drawing.Size(72, 21);
            this.btnNever.TabIndex = 4;
            this.btnNever.Text = "Never";
            this.btnNever.Click += this.btnNever_Click;
            // 
            // LineEntryDialog
            // 
            this.AcceptButton = this.btnYes;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this.btnNo;
            this.ClientSize = new System.Drawing.Size(282, 81);
            this.Controls.Add(this.btnNever);
            this.Controls.Add(this.btnNo);
            this.Controls.Add(this.btnYes);
            this.Controls.Add(this.lineBox);
            this.Controls.Add(this.titleLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LineEntryDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Enter Text";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private void InititalizeLocalization()
        {
            this.btnYes.Text = TextHelper.GetString("Label.Yes");
            this.btnNo.Text = TextHelper.GetString("Label.No");
            this.btnNever.Text = TextHelper.GetString("Label.Never");
            this.titleLabel.Text = TextHelper.GetString("Info.EnterText");
            this.Text = " " + TextHelper.GetString("Title.EnterText");
        }

        private void btnYes_Click(object sender, System.EventArgs e)
        {
            this.line = lineBox.Text;
            CancelEventArgs cancelArgs = new CancelEventArgs(false);
            OnValidating(cancelArgs);
            if (!cancelArgs.Cancel)
            {
                this.DialogResult = DialogResult.Yes;
                this.Close();
            }
        }

        private void btnNo_Click(object sender, System.EventArgs e)
        {
            this.DialogResult = DialogResult.No;
            this.Close();
        }

        private void btnNever_Click(object sender, System.EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
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
