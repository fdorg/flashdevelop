using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using CodeRefactor.Provider;
using PluginCore;
using PluginCore.Localization;

namespace CodeRefactor.Controls
{
    /// <summary>
    /// A pop up dialog for renaming.
    /// </summary>
    /// <seealso cref="ProjectManager.Helpers.LineEntryDialog"/>
    public class PopupRenameDialog : Form, IRenameHelper
    {
        Label titleLabel;
        TextBox inputTxt;
        Button btnOk;
        Button btnCancel;
        CheckBox cbxComments;
        CheckBox cbxStrings;
        string value;
        bool includeComments;
        bool includeStrings;

        /// <summary>
        /// Creates a new instance of <seealso cref="PopupRenameDialog"/>.
        /// </summary>
        /// <param name="targetName">The name of the target to initialize the title.</param>
        public PopupRenameDialog(string targetName)
            : this(targetName, false, false, false)
        {
        }

        /// <summary>
        /// Creates a new instance of <seealso cref="PopupRenameDialog"/>.
        /// </summary>
        /// <param name="targetName">The name of the target to initialize the title.</param>
        /// <param name="includeComments">Whether to initially include comments in search.</param>
        /// <param name="includeStrings">Whether to initially include strings in search.</param>
        public PopupRenameDialog(string targetName, bool includeComments, bool includeStrings)
            : this(targetName, includeComments, includeStrings, false)
        {
        }

        /// <summary>
        /// Creates a new instance of <seealso cref="PopupRenameDialog"/>.
        /// </summary>
        /// <param name="targetName">The name of the target to initialize the title.</param>
        /// <param name="includeComments">Whether to initially include comments in search.</param>
        /// <param name="includeStrings">Whether to initially include strings in search.</param>
        /// <param name="disableOptions">Whether to disable search options.</param>
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
            inputTxt.Focus();
            inputTxt.SelectAll();
        }

        /// <summary>
        /// Gets the current text.
        /// </summary>
        public string Value
        {
            get { return value; }
        }

        /// <summary>
        /// Gets the checked state of include comments check box.
        /// </summary>
        public bool IncludeComments
        {
            get { return includeComments; }
        }

        /// <summary>
        /// Gets the checked state of include strings check box.
        /// </summary>
        public bool IncludeStrings
        {
            get { return includeStrings; }
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.titleLabel = new System.Windows.Forms.Label();
            this.inputTxt = new System.Windows.Forms.TextBox();
            this.btnOk = new System.Windows.Forms.Button();
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
            this.btnOk.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnOk.Location = new System.Drawing.Point(73, 111);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(72, 21);
            this.btnOk.TabIndex = 3;
            this.btnOk.Text = "OK";
            this.btnOk.Click += new System.EventHandler(this.btnOK_Click);
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
            this.AcceptButton = this.btnOk;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(284, 141);
            this.Controls.Add(this.cbxStrings);
            this.Controls.Add(this.cbxComments);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
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

        /// <summary>
        /// Localizes texts in this control.
        /// </summary>
        /// <param name="targetName">The target name to initialize the title with.</param>
        void InititalizeLocalization(string targetName)
        {
            Text = " " + string.Format(TextHelper.GetString("Title.RenameDialog"), targetName);
            titleLabel.Text = TextHelper.GetString("Label.NewName");
            cbxComments.Text = TextHelper.GetString("Label.IncludeComments");
            cbxStrings.Text = TextHelper.GetString("Label.IncludeStrings");
            btnOk.Text = TextHelper.GetString("ProjectManager.Label.OK");
            btnCancel.Text = TextHelper.GetString("ProjectManager.Label.Cancel");
        }

        /// <summary>
        /// Occurs when the key is pressed while <see cref="inputTxt"/> has focus.
        /// </summary>
        /// <param name="sender">The event sender object.</param>
        /// <param name="e">The event arguments.</param>
        void InputTxt_KeyDown(object sender, KeyEventArgs e)
        {
            string shortcutId = PluginBase.MainForm.GetShortcutItemId(e.KeyData);
            if (string.IsNullOrEmpty(shortcutId)) return;
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

        /// <summary>
        /// Occurs when <see cref="btnOk"/> is pressed.
        /// </summary>
        /// <param name="sender">The event sender object.</param>
        /// <param name="e">The event arguments.</param>
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

        /// <summary>
        /// Occurs when <see cref="btnCancel"/> is pressed.
        /// </summary>
        /// <param name="sender">The event sender object.</param>
        /// <param name="e">The event arguments.</param>
        void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
