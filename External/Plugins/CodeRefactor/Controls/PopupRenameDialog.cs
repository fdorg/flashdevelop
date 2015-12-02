using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using CodeRefactor.Provider;
using PluginCore;
using PluginCore.Localization;

namespace CodeRefactor.Controls
{
    public class PopupRenameDialog : Form, IRenameHelper
    {
        IContainer components = null;
        Label titleLabel;
        TextBox inputTxt;
        Button btnOK;
        Button btnCancel;
        CheckBox cbxComments;
        CheckBox cbxStrings;
        private string value;
        private bool includeComments;
        private bool includeStrings;
        Dictionary<Keys, string> shortcuts;

        public PopupRenameDialog(string targetName)
            : this(targetName, false, false, false)
        {

        }

        public PopupRenameDialog(string targetName, bool includeComments, bool includeStrings)
            : this(targetName, includeComments, includeStrings, false)
        {
        }

        public PopupRenameDialog(string targetName, bool includeComments, bool includeStrings, bool disableOptions)
        {
            InitializeComponent();
            InititalizeLocalization(targetName);

            if (disableOptions)
            {
                cbxComments.Enabled = false;
                cbxStrings.Enabled = false;
            }
            else
            {
                cbxComments.Checked = includeComments;
                cbxStrings.Checked = includeStrings;
            }

            inputTxt.Text = targetName;
            inputTxt.SelectAll();
            inputTxt.Focus();

            shortcuts = PluginBase.MainForm.GetShortcutItemsByKeys();
        }

        public string Value
        {
            get { return value; }
        }

        public bool IncludeComments
        {
            get { return includeComments; }
        }

        public bool IncludeStrings
        {
            get { return includeStrings; }
        }

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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        void InitializeComponent()
        {
            this.titleLabel = new System.Windows.Forms.Label();
            this.inputTxt = new System.Windows.Forms.TextBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.cbxComments = new System.Windows.Forms.CheckBox();
            this.cbxStrings = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // titleLabel
            // 
            this.titleLabel.AutoEllipsis = true;
            this.titleLabel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
            this.titleLabel.Location = new System.Drawing.Point(9, 9);
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new System.Drawing.Size(266, 18);
            this.titleLabel.TabIndex = 5;
            this.titleLabel.Text = "Enter text:";
            // 
            // inputTxt
            // 
            this.inputTxt.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
            this.inputTxt.Location = new System.Drawing.Point(12, 30);
            this.inputTxt.Name = "inputTxt";
            this.inputTxt.Size = new System.Drawing.Size(260, 23);
            this.inputTxt.TabIndex = 0;
            this.inputTxt.KeyDown += new System.Windows.Forms.KeyEventHandler(this.InputTxt_KeyDown);
            // 
            // btnOK
            // 
            this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnOK.Location = new System.Drawing.Point(73, 111);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(72, 21);
            this.btnOK.TabIndex = 3;
            this.btnOK.Text = "OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnCancel.Location = new System.Drawing.Point(151, 111);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(72, 21);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // cbxComments
            // 
            this.cbxComments.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
            this.cbxComments.Location = new System.Drawing.Point(12, 59);
            this.cbxComments.Name = "cbxComments";
            this.cbxComments.Size = new System.Drawing.Size(260, 20);
            this.cbxComments.TabIndex = 1;
            this.cbxComments.Text = "Enter Text";
            this.cbxComments.UseVisualStyleBackColor = true;
            // 
            // cbxStrings
            // 
            this.cbxStrings.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
            this.cbxStrings.Location = new System.Drawing.Point(12, 85);
            this.cbxStrings.Name = "cbxStrings";
            this.cbxStrings.Size = new System.Drawing.Size(260, 20);
            this.cbxStrings.TabIndex = 2;
            this.cbxStrings.Text = "Enter Text";
            this.cbxStrings.UseVisualStyleBackColor = true;
            // 
            // PopupRenameDialog
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(284, 141);
            this.Controls.Add(this.cbxStrings);
            this.Controls.Add(this.cbxComments);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.inputTxt);
            this.Controls.Add(this.titleLabel);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PopupRenameDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Enter Text";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        void InititalizeLocalization(string targetName)
        {
            Text = " " + string.Format(TextHelper.GetString("Title.RenameDialog"), targetName);
            titleLabel.Text = TextHelper.GetString("Label.NewName");
            cbxComments.Text = TextHelper.GetString("Label.IncludeComments");
            cbxStrings.Text = TextHelper.GetString("Label.IncludeStrings");
            btnOK.Text = TextHelper.GetString("ProjectManager.Label.OK");
            btnCancel.Text = TextHelper.GetString("ProjectManager.Label.Cancel");
        }

        void InputTxt_KeyDown(object sender, KeyEventArgs e)
        {
            string shortcutId;
            if (shortcuts.TryGetValue(e.KeyData, out shortcutId))
            {
                switch (shortcutId)
                {
                    case "EditMenu.ToLowercase":
                    case "EditMenu.ToUppercase":
                        string text = inputTxt.SelectedText;
                        if (string.IsNullOrEmpty(text)) break;
                        text = shortcutId == "EditMenu.ToLowercase" ? text.ToLower() : text.ToUpper();
                        int selectionStart = inputTxt.SelectionStart;
                        inputTxt.Paste(text);
                        inputTxt.Select(selectionStart, text.Length);
                        break;
                }
            }
        }

        void btnOK_Click(object sender, EventArgs e)
        {
            value = inputTxt.Text;
            includeComments = cbxComments.Checked;
            includeStrings = cbxStrings.Checked;
            var cancelArgs = new CancelEventArgs();
            OnValidating(cancelArgs);
            if (cancelArgs.Cancel) return;
            DialogResult = DialogResult.OK;
            Close();
        }

        void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
