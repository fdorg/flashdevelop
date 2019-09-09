// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
namespace ProjectManager.Controls.AS2
{
    partial class AS2PropertiesDialog
    {
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.injectionTab = new System.Windows.Forms.TabPage();
            this.inputBrowseButton = new System.Windows.Forms.ButtonEx();
            this.inputSwfBox = new System.Windows.Forms.TextBoxEx();
            this.inputFileLabel = new System.Windows.Forms.Label();
            this.injectionCheckBox = new System.Windows.Forms.CheckBoxEx();
            this.infoLabel = new System.Windows.Forms.Label();
            this.tabControl.SuspendLayout();
            this.injectionTab.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.injectionTab);
            this.tabControl.Controls.SetChildIndex(this.injectionTab, 0);
            // 
            // injectionTab
            // 
            this.injectionTab.Controls.Add(this.inputBrowseButton);
            this.injectionTab.Controls.Add(this.inputSwfBox);
            this.injectionTab.Controls.Add(this.inputFileLabel);
            this.injectionTab.Controls.Add(this.injectionCheckBox);
            this.injectionTab.Controls.Add(this.infoLabel);
            this.injectionTab.Location = new System.Drawing.Point(4, 22);
            this.injectionTab.Name = "injectionTab";
            this.injectionTab.Size = new System.Drawing.Size(334, 272);
            this.injectionTab.TabIndex = 5;
            this.injectionTab.Text = "Injection";
            this.injectionTab.UseVisualStyleBackColor = true;
            // 
            // inputBrowseButton
            // 
            this.inputBrowseButton.Enabled = false;
            this.inputBrowseButton.Location = new System.Drawing.Point(230, 166);
            this.inputBrowseButton.Name = "inputBrowseButton";
            this.inputBrowseButton.Size = new System.Drawing.Size(75, 21);
            this.inputBrowseButton.TabIndex = 14;
            this.inputBrowseButton.Text = "&Browse...";
            this.inputBrowseButton.Click += new System.EventHandler(this.inputBrowseButton_Click);
            // 
            // inputSwfBox
            // 
            this.inputSwfBox.Enabled = false;
            this.inputSwfBox.Location = new System.Drawing.Point(94, 168);
            this.inputSwfBox.Name = "inputSwfBox";
            this.inputSwfBox.Size = new System.Drawing.Size(128, 21);
            this.inputSwfBox.TabIndex = 13;
            this.inputSwfBox.TextChanged += new System.EventHandler(this.inputSwfBox_TextChanged);
            // 
            // inputFileLabel
            // 
            this.inputFileLabel.Location = new System.Drawing.Point(3, 167);
            this.inputFileLabel.Name = "inputFileLabel";
            this.inputFileLabel.Size = new System.Drawing.Size(88, 18);
            this.inputFileLabel.TabIndex = 12;
            this.inputFileLabel.Text = "&Input SWF file:";
            this.inputFileLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // injectionCheckBox
            // 
            this.injectionCheckBox.Location = new System.Drawing.Point(15, 139);
            this.injectionCheckBox.Name = "injectionCheckBox";
            this.injectionCheckBox.Size = new System.Drawing.Size(168, 21);
            this.injectionCheckBox.TabIndex = 11;
            this.injectionCheckBox.Text = " Enable Code Injection";
            this.injectionCheckBox.CheckedChanged += new System.EventHandler(this.injectionCheckBox_CheckedChanged);
            // 
            // infoLabel
            // 
            this.infoLabel.Location = new System.Drawing.Point(12, 11);
            this.infoLabel.Name = "infoLabel";
            this.infoLabel.Size = new System.Drawing.Size(312, 125);
            this.infoLabel.TabIndex = 8;
            // 
            // AS2PropertiesDialog
            //
            this.Name = "AS2PropertiesDialog";
            this.tabControl.ResumeLayout(false);
            this.injectionTab.ResumeLayout(false);
            this.injectionTab.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabPage injectionTab;
        private System.Windows.Forms.Button inputBrowseButton;
        private System.Windows.Forms.TextBox inputSwfBox;
        private System.Windows.Forms.Label inputFileLabel;
        private System.Windows.Forms.CheckBox injectionCheckBox;
        private System.Windows.Forms.Label infoLabel;

    }
}
