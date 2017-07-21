using System.Drawing;
using System.Windows.Forms;
using PluginCore.Localization;
using PluginCore.Managers;

namespace PluginCore.Controls
{
    internal class ShortcutKeysEditorDialog : Form
    {
        private bool allowNone;
        private bool supportExtended;
        private bool shouldReset;
        private ShortcutKeys newKeys;
        private System.Windows.Forms.Button applyBtn;
        private System.Windows.Forms.Button cancelBtn;
        private System.Windows.Forms.Button resetBtn;
        private System.Windows.Forms.Label shortcutTxt;

        internal ShortcutKeysEditorDialog(ShortcutKeys defaultKeys, bool allowNone, bool supportExtended)
        {
            this.newKeys = defaultKeys;
            this.allowNone = allowNone;
            this.supportExtended = supportExtended;
            this.shouldReset = true;
            InitializeComponent();
            InitializeLocalization();
            InitializeFont();
            UpdateDisplay();
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.applyBtn = new System.Windows.Forms.Button();
            this.cancelBtn = new System.Windows.Forms.Button();
            this.resetBtn = new System.Windows.Forms.Button();
            this.shortcutTxt = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // applyBtn
            // 
            this.applyBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.applyBtn.Location = new System.Drawing.Point(141, 25);
            this.applyBtn.Name = "applyBtn";
            this.applyBtn.Size = new System.Drawing.Size(65, 20);
            this.applyBtn.TabIndex = 0;
            this.applyBtn.TabStop = false;
            this.applyBtn.Text = "Apply";
            this.applyBtn.UseVisualStyleBackColor = true;
            // 
            // cancelBtn
            // 
            this.cancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelBtn.Location = new System.Drawing.Point(209, 25);
            this.cancelBtn.Name = "cancelBtn";
            this.cancelBtn.Size = new System.Drawing.Size(65, 20);
            this.cancelBtn.TabIndex = 1;
            this.cancelBtn.TabStop = false;
            this.cancelBtn.Text = "Cancel";
            this.cancelBtn.UseVisualStyleBackColor = true;
            // 
            // resetBtn
            // 
            this.resetBtn.Location = new System.Drawing.Point(6, 25);
            this.resetBtn.Name = "resetBtn";
            this.resetBtn.Size = new System.Drawing.Size(65, 20);
            this.resetBtn.TabIndex = 2;
            this.resetBtn.TabStop = false;
            this.resetBtn.Text = "Reset";
            this.resetBtn.UseVisualStyleBackColor = true;
            this.resetBtn.Click += new System.EventHandler(this.ResetBtn_Click);
            // 
            // shortcutTxt
            // 
            this.shortcutTxt.BackColor = System.Drawing.SystemColors.Window;
            this.shortcutTxt.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.shortcutTxt.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.shortcutTxt.Location = new System.Drawing.Point(6, 6);
            this.shortcutTxt.Margin = new System.Windows.Forms.Padding(0);
            this.shortcutTxt.Name = "shortcutTxt";
            this.shortcutTxt.Size = new System.Drawing.Size(268, 16);
            this.shortcutTxt.TabIndex = 3;
            this.shortcutTxt.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // ShortcutModificationDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(280, 51);
            this.Controls.Add(this.shortcutTxt);
            this.Controls.Add(this.resetBtn);
            this.Controls.Add(this.cancelBtn);
            this.Controls.Add(this.applyBtn);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.KeyPreview = true;
            this.Name = "ShortcutModificationDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Press new shortcut keys:";
            this.ResumeLayout(false);

        }

        #endregion

        internal ShortcutKeys NewKeys
        {
            get { return newKeys; }
        }

        private void InitializeLocalization()
        {
            Text = TextHelper.GetString("FlashDevelop.Info.PressNewShortcut");
            applyBtn.Text = TextHelper.GetStringWithoutMnemonics("FlashDevelop.Label.Apply");
            cancelBtn.Text = TextHelper.GetStringWithoutMnemonics("FlashDevelop.Label.Cancel");
            resetBtn.Text = TextHelper.GetStringWithoutMnemonics("FlashDevelop.Label.Reset");
        }

        private void InitializeFont()
        {
            Font = PluginBase.Settings.DefaultFont;
        }

        private void UpdateDisplay()
        {
            shortcutTxt.Text = newKeys.ToString();
            if (ShortcutKeysManager.IsValidShortcut(newKeys) || allowNone && newKeys.IsNone)
            {
                applyBtn.Enabled = true;
                shortcutTxt.ForeColor = Color.DarkGreen;
            }
            else
            {
                applyBtn.Enabled = false;
                shortcutTxt.ForeColor = Color.DarkRed;
            }
        }

        private void ResetBtn_Click(object sender, System.EventArgs e)
        {
            newKeys = ShortcutKeys.None;
            UpdateDisplay();
            shortcutTxt.Focus();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            var keyCode = keyData & Keys.KeyCode;
            switch (keyCode)
            {
                case Keys.ControlKey:
                case Keys.ShiftKey:
                case Keys.Menu:
                    return base.ProcessCmdKey(ref msg, keyData);
                default:
                    if (supportExtended && !shouldReset)
                    {
                        ShortcutKeysManager.UpdateShortcutKeys(ref newKeys, keyData);
                    }
                    else
                    {
                        newKeys = keyData;
                        shouldReset = false;
                    }
                    UpdateDisplay();
                    return true;
            }
        }
    }
}
