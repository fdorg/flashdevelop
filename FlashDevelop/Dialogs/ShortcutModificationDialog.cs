using System.Drawing;
using System.Windows.Forms;
using PluginCore;
using PluginCore.Localization;
using PluginCore.Managers;

namespace FlashDevelop.Dialogs
{
    internal class ShortcutModificationDialog : Form
    {
        private bool supportExtended;
        private ShortcutKeys newKeys;
        private System.Windows.Forms.Button applyBtn;
        private System.Windows.Forms.Button cancelBtn;
        private System.Windows.Forms.Label shortcutDisplayTxt;

        internal ShortcutModificationDialog(bool supportExtended)
        {
            InitializeComponent();
            InitializeLocalization();
            InitializeFont();
            this.supportExtended = supportExtended;
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
            this.shortcutDisplayTxt = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // applyBtn
            // 
            this.applyBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.applyBtn.Enabled = false;
            this.applyBtn.Location = new System.Drawing.Point(116, 35);
            this.applyBtn.Name = "applyBtn";
            this.applyBtn.Size = new System.Drawing.Size(75, 23);
            this.applyBtn.TabIndex = 0;
            this.applyBtn.TabStop = false;
            this.applyBtn.Text = "Apply";
            this.applyBtn.UseVisualStyleBackColor = true;
            // 
            // cancelBtn
            // 
            this.cancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelBtn.Location = new System.Drawing.Point(197, 35);
            this.cancelBtn.Name = "cancelBtn";
            this.cancelBtn.Size = new System.Drawing.Size(75, 23);
            this.cancelBtn.TabIndex = 1;
            this.cancelBtn.TabStop = false;
            this.cancelBtn.Text = "Cancel";
            this.cancelBtn.UseVisualStyleBackColor = true;
            // 
            // shortcutDisplayTxt
            // 
            this.shortcutDisplayTxt.Location = new System.Drawing.Point(0, 9);
            this.shortcutDisplayTxt.Margin = new System.Windows.Forms.Padding(0);
            this.shortcutDisplayTxt.Name = "shortcutDisplayTxt";
            this.shortcutDisplayTxt.Size = new System.Drawing.Size(284, 23);
            this.shortcutDisplayTxt.TabIndex = 2;
            this.shortcutDisplayTxt.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // ShortcutModificationDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 70);
            this.Controls.Add(this.shortcutDisplayTxt);
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
            Text = TextHelper.GetString("Info.PressNewShortcut");
            applyBtn.Text = TextHelper.GetStringWithoutMnemonics("Label.Apply");
            cancelBtn.Text = TextHelper.GetStringWithoutMnemonics("Label.Cancel");
            shortcutDisplayTxt.Text = string.Empty;
        }

        private void InitializeFont()
        {
            shortcutDisplayTxt.Font = new Font(Globals.Settings.DefaultFont.FontFamily, 12F);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode != Keys.ControlKey && e.KeyCode != Keys.ShiftKey)
            {
                if (supportExtended &&
                    newKeys.IsSimple &&
                    ShortcutKeysManager.IsValidExtendedShortcutFirst(newKeys.First) &&
                    ShortcutKeysManager.IsValidExtendedShortcutSecond(e.KeyData))
                {
                    newKeys = new ShortcutKeys(newKeys.First, e.KeyData);
                }
                else
                {
                    newKeys = e.KeyData;
                }
                shortcutDisplayTxt.Text = newKeys.ToString();
                if (ShortcutKeysManager.IsValidShortcut(newKeys))
                {
                    applyBtn.Enabled = true;
                    shortcutDisplayTxt.ForeColor = Color.DarkGreen;
                }
                else
                {
                    applyBtn.Enabled = false;
                    shortcutDisplayTxt.ForeColor = Color.DarkRed;
                }
                e.Handled = true;
            }
            base.OnKeyDown(e);
        }
    }
}
