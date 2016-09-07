using System.ComponentModel;
using System.Windows.Forms;
using PluginCore;
using PluginCore.Localization;

namespace ProjectManager.Helpers
{
    /// <summary>
    /// A simple form where a user can enter a text string.
    /// </summary>
    public class LineEntryDialog : Form
    {
        string line;

        #region Form Designer Components

        private System.Windows.Forms.TextBox lineBox;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;
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
                if(components != null)
                {
                    components.Dispose();
                }
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
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
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
            // btnOK
            // 
            this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnOK.Location = new System.Drawing.Point(68, 50);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(72, 21);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnCancel.Location = new System.Drawing.Point(149, 50);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(72, 21);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // LineEntryDialog
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(282, 81);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
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
            this.btnOK.Text = TextHelper.GetString("Label.OK");
            this.btnCancel.Text = TextHelper.GetString("Label.Cancel");
            this.titleLabel.Text = TextHelper.GetString("Info.EnterText");
            this.Text = " " + TextHelper.GetString("Title.EnterText");
        }

        private void btnOK_Click(object sender, System.EventArgs e)
        {
            this.line = lineBox.Text;
            CancelEventArgs cancelArgs = new CancelEventArgs(false);
            OnValidating(cancelArgs);
            if (!cancelArgs.Cancel)
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void btnCancel_Click(object sender, System.EventArgs e)
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
