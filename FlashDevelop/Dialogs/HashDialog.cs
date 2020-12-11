// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Text;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PluginCore;
using PluginCore.Localization;
using PluginCore.Utilities;
using PluginCore.Controls;

namespace FlashDevelop.Dialogs
{
    public class HashDialog : SmartForm
    {
        Button okButton;
        Button cancelButton;
        ComboBox hashComboBox;
        ComboBox encodingComboBox;
        TextBox outputTextBox;
        TextBox inputTextBox;
        Label inputTextLabel;
        Label outputTextLabel;
        Label encodingLabel;
        Label hashLabel;
    
        public HashDialog()
        {
            Owner = Globals.MainForm;
            Font = PluginBase.Settings.DefaultFont;
            FormGuid = "f521ee73-e082-4218-a41f-bd1a501ebe27";
            HashResultText = string.Empty;
            InitializeComponent();
            ApplyLocalizedTexts();
        }

        #region Windows Form Designer Generated Code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        void InitializeComponent()
        {
            inputTextBox = new TextBoxEx();
            okButton = new ButtonEx();
            hashLabel = new Label();
            hashComboBox = new FlatCombo();
            inputTextLabel = new Label();
            outputTextLabel = new Label();
            outputTextBox = new TextBoxEx();
            encodingLabel = new Label();
            encodingComboBox = new FlatCombo();
            cancelButton = new ButtonEx();
            SuspendLayout();
            // 
            // inputTextBox
            // 
            inputTextBox.Location = new Point(12, 68);
            inputTextBox.Multiline = true;
            inputTextBox.Name = "inputTextBox";
            inputTextBox.Size = new Size(386, 75);
            inputTextBox.TabIndex = 7;
            inputTextBox.TextChanged += InputTextBoxChanged;
            inputTextBox.Font = PluginBase.Settings.DefaultFont; // Do not remove!
            // 
            // okButton
            // 
            okButton.FlatStyle = FlatStyle.System;
            okButton.Location = new Point(314, 248);
            okButton.Name = "okButton";
            okButton.Size = new Size(85, 23);
            okButton.TabIndex = 1;
            okButton.Text = "&OK";
            okButton.UseVisualStyleBackColor = true;
            okButton.DialogResult = DialogResult.OK;
            // 
            // hashLabel
            // 
            hashLabel.AutoSize = true;
            hashLabel.FlatStyle = FlatStyle.System;
            hashLabel.Location = new Point(14, 8);
            hashLabel.Name = "hashLabel";
            hashLabel.Size = new Size(35, 13);
            hashLabel.TabIndex = 2;
            hashLabel.Text = "Hash:";
            // 
            // hashComboBox
            // 
            hashComboBox.FlatStyle = FlatStyle.System;
            hashComboBox.FormattingEnabled = true;
            hashComboBox.Items.AddRange(new object[] {"None", "MD5", "SHA-1", "SHA-256", "RIPEMD-160"});
            hashComboBox.Location = new Point(12, 25);
            hashComboBox.Name = "hashComboBox";
            hashComboBox.Size = new Size(120, 21);
            hashComboBox.TabIndex = 3;
            hashComboBox.SelectedIndex = 1;
            hashComboBox.SelectedIndexChanged += CheckBoxSelectedIndexChanged;
            // 
            // inputTextLabel
            // 
            inputTextLabel.AutoSize = true;
            inputTextLabel.FlatStyle = FlatStyle.System;
            inputTextLabel.Location = new Point(14, 51);
            inputTextLabel.Name = "inputTextLabel";
            inputTextLabel.Size = new Size(60, 13);
            inputTextLabel.TabIndex = 6;
            inputTextLabel.Text = "Input text:";
            // 
            // outputTextLabel
            // 
            outputTextLabel.AutoSize = true;
            outputTextLabel.FlatStyle = FlatStyle.System;
            outputTextLabel.Location = new Point(14, 149);
            outputTextLabel.Name = "outputTextLabel";
            outputTextLabel.Size = new Size(68, 13);
            outputTextLabel.TabIndex = 8;
            outputTextLabel.Text = "Output text:";
            // 
            // outputTextBox
            // 
            outputTextBox.BackColor = SystemColors.Window;
            outputTextBox.Location = new Point(12, 166);
            outputTextBox.Multiline = true;
            outputTextBox.Name = "outputTextBox";
            outputTextBox.ReadOnly = true;
            outputTextBox.Size = new Size(386, 75);
            outputTextBox.TabIndex = 9;
            outputTextBox.ForeColor = SystemColors.GrayText;
            outputTextBox.Font = PluginBase.Settings.DefaultFont; // Do not remove!
            // 
            // encodingLabel
            // 
            encodingLabel.AutoSize = true;
            encodingLabel.FlatStyle = FlatStyle.System;
            encodingLabel.Location = new Point(156, 8);
            encodingLabel.Name = "encodingLabel";
            encodingLabel.Size = new Size(54, 13);
            encodingLabel.TabIndex = 4;
            encodingLabel.Text = "Encoding:";
            // 
            // encodingComboBox
            // 
            encodingComboBox.FlatStyle = FlatStyle.System;
            encodingComboBox.FormattingEnabled = true;
            encodingComboBox.Items.AddRange(new object[] {"Hex", "Base64", "Array"});
            encodingComboBox.Location = new Point(154, 25);
            encodingComboBox.Name = "encodingComboBox";
            encodingComboBox.Size = new Size(120, 21);
            encodingComboBox.TabIndex = 5;
            encodingComboBox.SelectedIndex = 0;
            encodingComboBox.SelectedIndexChanged += CheckBoxSelectedIndexChanged;
            // 
            // cancelButton
            // 
            cancelButton.DialogResult = DialogResult.Cancel;
            cancelButton.FlatStyle = FlatStyle.System;
            cancelButton.Location = new Point(221, 248);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new Size(85, 23);
            cancelButton.TabIndex = 10;
            cancelButton.Text = "&Cancel";
            cancelButton.UseVisualStyleBackColor = true;
            cancelButton.DialogResult = DialogResult.Cancel;
            // 
            // HashDialog
            // 
            AcceptButton = okButton;
            AutoScaleDimensions = new SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = cancelButton;
            ClientSize = new Size(411, 284);
            Controls.Add(cancelButton);
            Controls.Add(encodingComboBox);
            Controls.Add(encodingLabel);
            Controls.Add(outputTextBox);
            Controls.Add(outputTextLabel);
            Controls.Add(inputTextLabel);
            Controls.Add(hashComboBox);
            Controls.Add(hashLabel);
            Controls.Add(okButton);
            Controls.Add(inputTextBox);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "HashDialog";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = " Insert Hash";
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        #region Methods And Event Handlers

        /// <summary>
        /// Result hash text of the dialog
        /// </summary>
        public string HashResultText { get; private set; }

        /// <summary>
        /// Applies the localized texts
        /// </summary>
        void ApplyLocalizedTexts()
        {
            okButton.Text = TextHelper.GetString("Label.Ok");
            hashLabel.Text = TextHelper.GetString("Label.HashInput");
            encodingLabel.Text = TextHelper.GetString("Label.EncodingInput");
            inputTextLabel.Text = TextHelper.GetString("Label.InputText");
            outputTextLabel.Text = TextHelper.GetString("Label.OutputText");
            cancelButton.Text = TextHelper.GetString("Label.Cancel");
            Text = " " + TextHelper.GetString("Title.HashDialog");
        }

        /// <summary>
        /// Updates the output when the index is changed
        /// </summary>
        void CheckBoxSelectedIndexChanged(object sender, EventArgs e) => InputTextBoxChanged(null, null);

        /// <summary>
        /// When text changes, update the hash to the output box
        /// </summary>
        void InputTextBoxChanged(object sender, EventArgs e)
        {
            var bytes = Encoding.UTF8.GetBytes(inputTextBox.Text);
            try
            {
                if (hashComboBox.SelectedIndex == 1) OutputHash(MD5.Compute(bytes));
                else if (hashComboBox.SelectedIndex == 2) OutputHash(SHA1.Compute(bytes));
                else if (hashComboBox.SelectedIndex == 3) OutputHash(SHA256.Compute(bytes));
                else if (hashComboBox.SelectedIndex == 4) OutputHash(RMD160.Compute(bytes));
                else OutputHash(bytes);
            }
            catch (InvalidOperationException)
            {
                outputTextBox.Text = "[ERROR: FIPS 140-2 compliance]";
            }
        }

        /// <summary>
        /// Outputs the hash to the output box in specified encoding
        /// </summary>
        void OutputHash(byte[] hashBytes)
        {
            if (encodingComboBox.SelectedIndex == 0)
            {
                outputTextBox.Text = HashResultText = Base16.Encode(hashBytes);
            } 
            else if (encodingComboBox.SelectedIndex == 1)
            {
                outputTextBox.Text = HashResultText = Base64.Encode(hashBytes);
            }
            else if (encodingComboBox.SelectedIndex == 2)
            {
                var output = string.Join(", ", hashBytes.Select(entry => entry.ToString()).ToArray());
                outputTextBox.Text = output;
                HashResultText = output;
            }
        }

        #endregion
    }
}