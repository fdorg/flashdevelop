using System;
using System.Text;
using System.Drawing;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using PluginCore.Localization;
using PluginCore.Utilities;
using PluginCore.Helpers;

namespace FlashDevelop.Dialogs
{
    public class HashDialog : Form
    {
        private System.String resultHashText;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.ComboBox hashComboBox;
        private System.Windows.Forms.ComboBox encodingComboBox;
        private System.Windows.Forms.TextBox outputTextBox;
        private System.Windows.Forms.TextBox inputTextBox;
        private System.Windows.Forms.Label inputTextLabel;
        private System.Windows.Forms.Label outputTextLabel;
        private System.Windows.Forms.Label encodingLabel;
        private System.Windows.Forms.Label hashLabel;
    
        public HashDialog()
        {
            this.Owner = Globals.MainForm;
            this.Font = Globals.Settings.DefaultFont;
            this.resultHashText = String.Empty;
            this.InitializeComponent();
            this.ApplyLocalizedTexts();
            ScaleHelper.AdjustForHighDPI(this);
        }

        #region Windows Form Designer Generated Code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.inputTextBox = new System.Windows.Forms.TextBox();
            this.okButton = new System.Windows.Forms.Button();
            this.hashLabel = new System.Windows.Forms.Label();
            this.hashComboBox = new System.Windows.Forms.ComboBox();
            this.inputTextLabel = new System.Windows.Forms.Label();
            this.outputTextLabel = new System.Windows.Forms.Label();
            this.outputTextBox = new System.Windows.Forms.TextBox();
            this.encodingLabel = new System.Windows.Forms.Label();
            this.encodingComboBox = new System.Windows.Forms.ComboBox();
            this.cancelButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // inputTextBox
            // 
            this.inputTextBox.Location = new System.Drawing.Point(12, 68);
            this.inputTextBox.Multiline = true;
            this.inputTextBox.Name = "inputTextBox";
            this.inputTextBox.Size = new System.Drawing.Size(386, 75);
            this.inputTextBox.TabIndex = 7;
            this.inputTextBox.TextChanged += new System.EventHandler(this.InputTextBoxChanged);
            this.inputTextBox.Font = Globals.Settings.DefaultFont; // Do not remove!
            // 
            // okButton
            // 
            this.okButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.okButton.Location = new System.Drawing.Point(314, 248);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(85, 23);
            this.okButton.TabIndex = 1;
            this.okButton.Text = "&OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.DialogResult = DialogResult.OK;
            // 
            // hashLabel
            // 
            this.hashLabel.AutoSize = true;
            this.hashLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.hashLabel.Location = new System.Drawing.Point(14, 8);
            this.hashLabel.Name = "hashLabel";
            this.hashLabel.Size = new System.Drawing.Size(35, 13);
            this.hashLabel.TabIndex = 2;
            this.hashLabel.Text = "Hash:";
            // 
            // hashComboBox
            // 
            this.hashComboBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.hashComboBox.FormattingEnabled = true;
            this.hashComboBox.Items.AddRange(new object[] {"None", "MD5", "SHA-1", "SHA-256", "RIPEMD-160"});
            this.hashComboBox.Location = new System.Drawing.Point(12, 25);
            this.hashComboBox.Name = "hashComboBox";
            this.hashComboBox.Size = new System.Drawing.Size(120, 21);
            this.hashComboBox.TabIndex = 3;
            this.hashComboBox.SelectedIndex = 1;
            this.hashComboBox.SelectedIndexChanged += new System.EventHandler(this.CheckBoxSelectedIndexChanged);
            // 
            // inputTextLabel
            // 
            this.inputTextLabel.AutoSize = true;
            this.inputTextLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.inputTextLabel.Location = new System.Drawing.Point(14, 51);
            this.inputTextLabel.Name = "inputTextLabel";
            this.inputTextLabel.Size = new System.Drawing.Size(60, 13);
            this.inputTextLabel.TabIndex = 6;
            this.inputTextLabel.Text = "Input text:";
            // 
            // outputTextLabel
            // 
            this.outputTextLabel.AutoSize = true;
            this.outputTextLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.outputTextLabel.Location = new System.Drawing.Point(14, 149);
            this.outputTextLabel.Name = "outputTextLabel";
            this.outputTextLabel.Size = new System.Drawing.Size(68, 13);
            this.outputTextLabel.TabIndex = 8;
            this.outputTextLabel.Text = "Output text:";
            // 
            // outputTextBox
            // 
            this.outputTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.outputTextBox.Location = new System.Drawing.Point(12, 166);
            this.outputTextBox.Multiline = true;
            this.outputTextBox.Name = "outputTextBox";
            this.outputTextBox.ReadOnly = true;
            this.outputTextBox.Size = new System.Drawing.Size(386, 75);
            this.outputTextBox.TabIndex = 9;
            this.outputTextBox.ForeColor = SystemColors.GrayText;
            this.outputTextBox.Font = Globals.Settings.DefaultFont; // Do not remove!
            // 
            // encodingLabel
            // 
            this.encodingLabel.AutoSize = true;
            this.encodingLabel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.encodingLabel.Location = new System.Drawing.Point(156, 8);
            this.encodingLabel.Name = "encodingLabel";
            this.encodingLabel.Size = new System.Drawing.Size(54, 13);
            this.encodingLabel.TabIndex = 4;
            this.encodingLabel.Text = "Encoding:";
            // 
            // encodingComboBox
            // 
            this.encodingComboBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.encodingComboBox.FormattingEnabled = true;
            this.encodingComboBox.Items.AddRange(new object[] {"Hex", "Base64", "Array"});
            this.encodingComboBox.Location = new System.Drawing.Point(154, 25);
            this.encodingComboBox.Name = "encodingComboBox";
            this.encodingComboBox.Size = new System.Drawing.Size(120, 21);
            this.encodingComboBox.TabIndex = 5;
            this.encodingComboBox.SelectedIndex = 0;
            this.encodingComboBox.SelectedIndexChanged += new System.EventHandler(this.CheckBoxSelectedIndexChanged);
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cancelButton.Location = new System.Drawing.Point(221, 248);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(85, 23);
            this.cancelButton.TabIndex = 10;
            this.cancelButton.Text = "&Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.DialogResult = DialogResult.Cancel;
            // 
            // HashDialog
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(411, 284);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.encodingComboBox);
            this.Controls.Add(this.encodingLabel);
            this.Controls.Add(this.outputTextBox);
            this.Controls.Add(this.outputTextLabel);
            this.Controls.Add(this.inputTextLabel);
            this.Controls.Add(this.hashComboBox);
            this.Controls.Add(this.hashLabel);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.inputTextBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "HashDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = " Insert Hash";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        #region Methods And Event Handlers

        /// <summary>
        /// Result hash text of the dialog
        /// </summary>
        public String HashResultText
        {
            get { return this.resultHashText; }
        }

        /// <summary>
        /// Applies the localized texts
        /// </summary>
        private void ApplyLocalizedTexts()
        {
            this.okButton.Text = TextHelper.GetString("Label.Ok");
            this.hashLabel.Text = TextHelper.GetString("Label.HashInput");
            this.encodingLabel.Text = TextHelper.GetString("Label.EncodingInput");
            this.inputTextLabel.Text = TextHelper.GetString("Label.InputText");
            this.outputTextLabel.Text = TextHelper.GetString("Label.OutputText");
            this.cancelButton.Text = TextHelper.GetString("Label.Cancel");
            this.Text = " " + TextHelper.GetString("Title.HashDialog");
        }

        /// <summary>
        /// Updates the output when the index is changed
        /// </summary>
        private void CheckBoxSelectedIndexChanged(Object sender, EventArgs e)
        {
            this.InputTextBoxChanged(null, null);
        }

        /// <summary>
        /// When text changes, update the hash to the output box
        /// </summary>
        private void InputTextBoxChanged(Object sender, EventArgs e)
        {
            Byte[] plainBytes = Encoding.UTF8.GetBytes(this.inputTextBox.Text);
            try
            {
                if (this.hashComboBox.SelectedIndex == 1)
                {
                    this.OutputHash(MD5.Compute(plainBytes));
                }
                else if (this.hashComboBox.SelectedIndex == 2)
                {
                    this.OutputHash(SHA1.Compute(plainBytes));
                }
                else if (this.hashComboBox.SelectedIndex == 3)
                {
                    this.OutputHash(SHA256.Compute(plainBytes));
                }
                else if (this.hashComboBox.SelectedIndex == 4)
                {
                    this.OutputHash(RMD160.Compute(plainBytes));
                }
                else this.OutputHash(plainBytes);
            }
            catch (InvalidOperationException)
            {
                this.outputTextBox.Text = "[ERROR: FIPS 140-2 compliance]";
            }
        }

        /// <summary>
        /// Outputs the hash to the output box in specified encoding
        /// </summary>
        private void OutputHash(Byte[] hashBytes)
        {
            if (this.encodingComboBox.SelectedIndex == 0)
            {
                String b16Hash = Base16.Encode(hashBytes);
                this.outputTextBox.Text = this.resultHashText = b16Hash;
            } 
            else if (this.encodingComboBox.SelectedIndex == 1)
            {
                String b64Hash = Base64.Encode(hashBytes);
                this.outputTextBox.Text = this.resultHashText = b64Hash;
            }
            else if (this.encodingComboBox.SelectedIndex == 2)
            {
                List<String> array = new List<String>();
                foreach (Byte entry in hashBytes) array.Add(entry.ToString());
                String output = String.Join(", ", array.ToArray());
                this.outputTextBox.Text = output;
                this.resultHashText = output;
            }
        }

        #endregion

    }

}
