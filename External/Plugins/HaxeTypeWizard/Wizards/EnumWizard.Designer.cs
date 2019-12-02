// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com

namespace HaxeTypeWizard.Wizards
{
    public partial class EnumWizard
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
            this.errorLabel = new System.Windows.Forms.Label();
            this.errorIcon = new System.Windows.Forms.PictureBox();
            this.flowLayoutPanel6 = new System.Windows.Forms.FlowLayoutPanel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.cancelButton = new System.Windows.Forms.ButtonEx();
            this.okButton = new System.Windows.Forms.ButtonEx();
            this.titleLabel = new System.Windows.Forms.Label();
            this.flowLayoutPanel9 = new System.Windows.Forms.FlowLayoutPanel();
            this.groupBox2 = new System.Windows.Forms.GroupBoxEx();
            this.classBox = new System.Windows.Forms.TextBoxEx();
            this.typeLabel = new System.Windows.Forms.Label();
            this.packageBox = new System.Windows.Forms.TextBoxEx();
            this.packageLabel = new System.Windows.Forms.Label();
            this.packageBrowse = new System.Windows.Forms.ButtonEx();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.errorIcon)).BeginInit();
            this.flowLayoutPanel6.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel9.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // errorLabel
            // 
            this.errorLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.errorLabel.AutoSize = true;
            this.errorLabel.ForeColor = System.Drawing.Color.Black;
            this.errorLabel.Location = new System.Drawing.Point(33, 5);
            this.errorLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.errorLabel.Name = "errorLabel";
            this.errorLabel.Size = new System.Drawing.Size(40, 17);
            this.errorLabel.TabIndex = 0;
            this.errorLabel.Text = "Error";
            // 
            // errorIcon
            // 
            this.errorIcon.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.errorIcon.Location = new System.Drawing.Point(4, 4);
            this.errorIcon.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.errorIcon.Name = "errorIcon";
            this.errorIcon.Size = new System.Drawing.Size(21, 20);
            this.errorIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.errorIcon.TabIndex = 0;
            this.errorIcon.TabStop = false;
            // 
            // flowLayoutPanel6
            // 
            this.flowLayoutPanel6.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel6.Controls.Add(this.errorIcon);
            this.flowLayoutPanel6.Controls.Add(this.errorLabel);
            this.flowLayoutPanel6.Location = new System.Drawing.Point(6, 4);
            this.flowLayoutPanel6.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.flowLayoutPanel6.Name = "flowLayoutPanel6";
            this.flowLayoutPanel6.Size = new System.Drawing.Size(395, 28);
            this.flowLayoutPanel6.TabIndex = 0;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.Controls.Add(this.cancelButton);
            this.flowLayoutPanel1.Controls.Add(this.okButton);
            this.flowLayoutPanel1.Controls.Add(this.flowLayoutPanel6);
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(16, 201);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(621, 36);
            this.flowLayoutPanel1.TabIndex = 2;
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.DisabledBackColor = System.Drawing.SystemColors.Control;
            this.cancelButton.DisabledTextColor = System.Drawing.SystemColors.ControlDark;
            this.cancelButton.Location = new System.Drawing.Point(517, 4);
            this.cancelButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(100, 28);
            this.cancelButton.TabIndex = 2;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseTheme = true;
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // okButton
            // 
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.DisabledBackColor = System.Drawing.SystemColors.Control;
            this.okButton.DisabledTextColor = System.Drawing.SystemColors.ControlDark;
            this.okButton.Enabled = false;
            this.okButton.Location = new System.Drawing.Point(409, 4);
            this.okButton.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(100, 28);
            this.okButton.TabIndex = 1;
            this.okButton.Text = "Ok";
            this.okButton.UseTheme = true;
            this.okButton.UseVisualStyleBackColor = true;
            // 
            // titleLabel
            // 
            this.titleLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.titleLabel.AutoSize = true;
            this.titleLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.titleLabel.Location = new System.Drawing.Point(11, 6);
            this.titleLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new System.Drawing.Size(124, 17);
            this.titleLabel.TabIndex = 0;
            this.titleLabel.Text = "New Haxe Enum";
            // 
            // flowLayoutPanel9
            // 
            this.flowLayoutPanel9.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel9.BackColor = System.Drawing.SystemColors.Window;
            this.flowLayoutPanel9.Controls.Add(this.titleLabel);
            this.flowLayoutPanel9.Location = new System.Drawing.Point(16, 15);
            this.flowLayoutPanel9.Margin = new System.Windows.Forms.Padding(4);
            this.flowLayoutPanel9.Name = "flowLayoutPanel9";
            this.flowLayoutPanel9.Padding = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.flowLayoutPanel9.Size = new System.Drawing.Size(621, 43);
            this.flowLayoutPanel9.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.groupBox2.Controls.Add(this.tableLayoutPanel2);
            this.groupBox2.Location = new System.Drawing.Point(13, 65);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox2.Size = new System.Drawing.Size(624, 128);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.UseTheme = true;
            // 
            // classBox
            // 
            this.classBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.classBox.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.classBox.Location = new System.Drawing.Point(141, 41);
            this.classBox.Margin = new System.Windows.Forms.Padding(4);
            this.classBox.Name = "classBox";
            this.classBox.Size = new System.Drawing.Size(354, 22);
            this.classBox.TabIndex = 7;
            this.classBox.Text = "NewEnum";
            this.classBox.UseTheme = true;
            this.classBox.TextChanged += new System.EventHandler(this.ClassBox_TextChanged);
            // 
            // typeLabel
            // 
            this.typeLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.typeLabel.AutoSize = true;
            this.typeLabel.Location = new System.Drawing.Point(4, 44);
            this.typeLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.typeLabel.Name = "typeLabel";
            this.typeLabel.Size = new System.Drawing.Size(45, 17);
            this.typeLabel.TabIndex = 6;
            this.typeLabel.Text = "Name";
            // 
            // packageBox
            // 
            this.packageBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.packageBox.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.packageBox.Location = new System.Drawing.Point(141, 6);
            this.packageBox.Margin = new System.Windows.Forms.Padding(4);
            this.packageBox.Name = "packageBox";
            this.packageBox.Size = new System.Drawing.Size(354, 22);
            this.packageBox.TabIndex = 1;
            this.packageBox.UseTheme = true;
            this.packageBox.TextChanged += new System.EventHandler(this.PackageBox_TextChanged);
            // 
            // packageLabel
            // 
            this.packageLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.packageLabel.AutoSize = true;
            this.packageLabel.Location = new System.Drawing.Point(4, 9);
            this.packageLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.packageLabel.Name = "packageLabel";
            this.packageLabel.Size = new System.Drawing.Size(63, 17);
            this.packageLabel.TabIndex = 0;
            this.packageLabel.Text = "Package";
            // 
            // packageBrowse
            // 
            this.packageBrowse.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.packageBrowse.DisabledBackColor = System.Drawing.SystemColors.Control;
            this.packageBrowse.DisabledTextColor = System.Drawing.SystemColors.ControlDark;
            this.packageBrowse.Location = new System.Drawing.Point(506, 4);
            this.packageBrowse.Margin = new System.Windows.Forms.Padding(4);
            this.packageBrowse.Name = "packageBrowse";
            this.packageBrowse.Size = new System.Drawing.Size(99, 27);
            this.packageBrowse.TabIndex = 2;
            this.packageBrowse.Text = "Browse...";
            this.packageBrowse.UseTheme = true;
            this.packageBrowse.UseVisualStyleBackColor = true;
            this.packageBrowse.Click += new System.EventHandler(this.PackageBrowse_Click);
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel2.ColumnCount = 3;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 27.42382F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 72.57618F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 113F));
            this.tableLayoutPanel2.Controls.Add(this.packageBrowse, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.packageLabel, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.packageBox, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.typeLabel, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.classBox, 1, 1);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 12);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(4);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(613, 70);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // EnumWizard
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(653, 251);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.flowLayoutPanel9);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EnumWizard";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "New Haxe Enum";
            this.Load += new System.EventHandler(this.ClassWizard_Load);
            ((System.ComponentModel.ISupportInitialize)(this.errorIcon)).EndInit();
            this.flowLayoutPanel6.ResumeLayout(false);
            this.flowLayoutPanel6.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel9.ResumeLayout(false);
            this.flowLayoutPanel9.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label errorLabel;
        private System.Windows.Forms.PictureBox errorIcon;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel6;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.ButtonEx cancelButton;
        private System.Windows.Forms.ButtonEx okButton;
        private System.Windows.Forms.Label titleLabel;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel9;
        private System.Windows.Forms.GroupBoxEx groupBox2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.ButtonEx packageBrowse;
        private System.Windows.Forms.Label packageLabel;
        private System.Windows.Forms.TextBoxEx packageBox;
        private System.Windows.Forms.Label typeLabel;
        private System.Windows.Forms.TextBoxEx classBox;
    }
}

