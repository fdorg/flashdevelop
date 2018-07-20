// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
namespace AirProperties
{
    partial class LocaleManager
    {
        private System.Windows.Forms.ListBox AvailableListBox;
        private System.Windows.Forms.TextBox CustomLocaleField;
        private System.Windows.Forms.Button AddNewButton;
        private System.Windows.Forms.Button RemoveButton;
        private System.Windows.Forms.ListBox SelectedListBox;
        private System.Windows.Forms.Button AddButton;
        private System.Windows.Forms.Button OKButton;
        private System.Windows.Forms.GroupBox LocalesGroupBox;
        private System.Windows.Forms.Label SelectedLocalesLabel;
        private System.Windows.Forms.Label DefaultLocalesLabel;
        private System.Windows.Forms.Label CustomLocaleLabel;
        private System.Windows.Forms.ErrorProvider ValidationErrorProvider;
        private System.Windows.Forms.Button CancelButton1;

        #region Windows Form Designer Generated Code
        
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.AvailableListBox = new System.Windows.Forms.ListBoxEx();
            this.CustomLocaleField = new System.Windows.Forms.TextBoxEx();
            this.AddNewButton = new System.Windows.Forms.ButtonEx();
            this.RemoveButton = new System.Windows.Forms.ButtonEx();
            this.SelectedListBox = new System.Windows.Forms.ListBoxEx();
            this.AddButton = new System.Windows.Forms.ButtonEx();
            this.OKButton = new System.Windows.Forms.ButtonEx();
            this.LocalesGroupBox = new System.Windows.Forms.GroupBoxEx();
            this.SelectedLocalesLabel = new System.Windows.Forms.Label();
            this.DefaultLocalesLabel = new System.Windows.Forms.Label();
            this.CustomLocaleLabel = new System.Windows.Forms.Label();
            this.ValidationErrorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.CancelButton1 = new System.Windows.Forms.ButtonEx();
            this.LocalesGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ValidationErrorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // AvailableListBox
            // 
            this.AvailableListBox.FormattingEnabled = true;
            this.AvailableListBox.Location = new System.Drawing.Point(10, 35);
            this.AvailableListBox.Name = "AvailableListBox";
            this.AvailableListBox.Size = new System.Drawing.Size(145, 173);
            this.AvailableListBox.Sorted = true;
            this.AvailableListBox.TabIndex = 1;
            this.AvailableListBox.DoubleClick += new System.EventHandler(this.AvailableListBox_DoubleClick);
            // 
            // CustomLocaleField
            // 
            this.ValidationErrorProvider.SetIconPadding(this.CustomLocaleField, 66);
            this.CustomLocaleField.Location = new System.Drawing.Point(112, 223);
            this.CustomLocaleField.Name = "CustomLocaleField";
            this.CustomLocaleField.Size = new System.Drawing.Size(145, 21);
            this.CustomLocaleField.TabIndex = 7;
            this.CustomLocaleField.Validating += new System.ComponentModel.CancelEventHandler(this.CustomLocaleField_Validating);
            // 
            // AddNewButton
            // 
            this.AddNewButton.Location = new System.Drawing.Point(263, 220);
            this.AddNewButton.Name = "AddNewButton";
            this.AddNewButton.Size = new System.Drawing.Size(57, 23);
            this.AddNewButton.TabIndex = 8;
            this.AddNewButton.Text = "Add";
            this.AddNewButton.UseVisualStyleBackColor = true;
            this.AddNewButton.Click += new System.EventHandler(this.AddNewButton_Click);
            // 
            // RemoveButton
            // 
            this.RemoveButton.Location = new System.Drawing.Point(168, 112);
            this.RemoveButton.Name = "RemoveButton";
            this.RemoveButton.Size = new System.Drawing.Size(34, 23);
            this.RemoveButton.TabIndex = 3;
            this.RemoveButton.Text = "<";
            this.RemoveButton.UseVisualStyleBackColor = true;
            this.RemoveButton.Click += new System.EventHandler(this.RemoveButton_Click);
            // 
            // SelectedListBox
            // 
            this.SelectedListBox.FormattingEnabled = true;
            this.SelectedListBox.Location = new System.Drawing.Point(216, 35);
            this.SelectedListBox.Name = "SelectedListBox";
            this.SelectedListBox.Size = new System.Drawing.Size(145, 173);
            this.SelectedListBox.TabIndex = 5;
            this.SelectedListBox.DoubleClick += new System.EventHandler(this.SelectedListBox_DoubleClick);
            // 
            // AddButton
            // 
            this.AddButton.Location = new System.Drawing.Point(168, 83);
            this.AddButton.Name = "AddButton";
            this.AddButton.Size = new System.Drawing.Size(34, 23);
            this.AddButton.TabIndex = 2;
            this.AddButton.Text = ">";
            this.AddButton.UseVisualStyleBackColor = true;
            this.AddButton.Click += new System.EventHandler(this.AddButton_Click);
            // 
            // OKButton
            // 
            this.OKButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.OKButton.Location = new System.Drawing.Point(230, 271);
            this.OKButton.Name = "OKButton";
            this.OKButton.Size = new System.Drawing.Size(75, 23);
            this.OKButton.TabIndex = 1;
            this.OKButton.Text = "OK";
            this.OKButton.UseVisualStyleBackColor = true;
            this.OKButton.Click += new System.EventHandler(this.OKButton_Click);
            // 
            // LocalesGroupBox
            // 
            this.LocalesGroupBox.Controls.Add(this.SelectedLocalesLabel);
            this.LocalesGroupBox.Controls.Add(this.DefaultLocalesLabel);
            this.LocalesGroupBox.Controls.Add(this.CustomLocaleLabel);
            this.LocalesGroupBox.Controls.Add(this.CustomLocaleField);
            this.LocalesGroupBox.Controls.Add(this.AvailableListBox);
            this.LocalesGroupBox.Controls.Add(this.AddButton);
            this.LocalesGroupBox.Controls.Add(this.AddNewButton);
            this.LocalesGroupBox.Controls.Add(this.SelectedListBox);
            this.LocalesGroupBox.Controls.Add(this.RemoveButton);
            this.LocalesGroupBox.Location = new System.Drawing.Point(13, 8);
            this.LocalesGroupBox.Name = "LocalesGroupBox";
            this.LocalesGroupBox.Size = new System.Drawing.Size(370, 257);
            this.LocalesGroupBox.TabIndex = 0;
            this.LocalesGroupBox.TabStop = false;
            this.LocalesGroupBox.Text = "Locales";
            // 
            // SelectedLocalesLabel
            // 
            this.SelectedLocalesLabel.AutoSize = true;
            this.SelectedLocalesLabel.Location = new System.Drawing.Point(214, 18);
            this.SelectedLocalesLabel.Name = "SelectedLocalesLabel";
            this.SelectedLocalesLabel.Size = new System.Drawing.Size(86, 13);
            this.SelectedLocalesLabel.TabIndex = 4;
            this.SelectedLocalesLabel.Text = "Selected Locales";
            // 
            // DefaultLocalesLabel
            // 
            this.DefaultLocalesLabel.AutoSize = true;
            this.DefaultLocalesLabel.Location = new System.Drawing.Point(8, 19);
            this.DefaultLocalesLabel.Name = "DefaultLocalesLabel";
            this.DefaultLocalesLabel.Size = new System.Drawing.Size(80, 13);
            this.DefaultLocalesLabel.TabIndex = 0;
            this.DefaultLocalesLabel.Text = "Default Locales";
            // 
            // CustomLocaleLabel
            // 
            this.CustomLocaleLabel.AutoSize = true;
            this.CustomLocaleLabel.Location = new System.Drawing.Point(8, 225);
            this.CustomLocaleLabel.Name = "CustomLocaleLabel";
            this.CustomLocaleLabel.Size = new System.Drawing.Size(98, 13);
            this.CustomLocaleLabel.TabIndex = 6;
            this.CustomLocaleLabel.Text = "Add Custom Locale";
            // 
            // ValidationErrorProvider
            // 
            this.ValidationErrorProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
            this.ValidationErrorProvider.ContainerControl = this;
            // 
            // CancelButton
            // 
            this.CancelButton1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.CancelButton1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.CancelButton1.Location = new System.Drawing.Point(310, 271);
            this.CancelButton1.Name = "CancelButton";
            this.CancelButton1.Size = new System.Drawing.Size(75, 23);
            this.CancelButton1.TabIndex = 2;
            this.CancelButton1.Text = "Cancel";
            this.CancelButton1.UseVisualStyleBackColor = true;
            this.CancelButton1.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // LocaleManager
            //
            this.MinimizeBox = false;
            this.MaximizeBox = false;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
            this.ClientSize = new System.Drawing.Size(397, 305);
            this.Controls.Add(this.CancelButton1);
            this.Controls.Add(this.LocalesGroupBox);
            this.Controls.Add(this.OKButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "LocaleManager";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = " Configure AIR Application Installer Locales";
            this.LocalesGroupBox.ResumeLayout(false);
            this.LocalesGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ValidationErrorProvider)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

    }

}