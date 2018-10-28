
namespace ASClassWizard.Wizards
{
    partial class AS3InterfaceWizard
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
            this.baseBox = new System.Windows.Forms.TextBoxEx();
            this.classBox = new System.Windows.Forms.TextBoxEx();
            this.packageBox = new System.Windows.Forms.TextBoxEx();
            this.classLabel = new System.Windows.Forms.Label();
            this.baseLabel = new System.Windows.Forms.Label();
            this.packageLabel = new System.Windows.Forms.Label();
            this.packageBrowse = new System.Windows.Forms.ButtonEx();
            this.baseBrowse = new System.Windows.Forms.ButtonEx();
            this.errorLabel = new System.Windows.Forms.Label();
            this.errorIcon = new System.Windows.Forms.PictureBox();
            this.flowLayoutPanel6 = new System.Windows.Forms.FlowLayoutPanel();
            this.flowLayoutPanel9 = new System.Windows.Forms.FlowLayoutPanel();
            this.titleLabel = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBoxEx();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.cancelButton = new System.Windows.Forms.ButtonEx();
            this.okButton = new System.Windows.Forms.ButtonEx();
            ((System.ComponentModel.ISupportInitialize)(this.errorIcon)).BeginInit();
            this.flowLayoutPanel6.SuspendLayout();
            this.flowLayoutPanel9.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // baseBox
            // 
            this.baseBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.baseBox.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.baseBox.Enabled = false;
            this.baseBox.Location = new System.Drawing.Point(106, 63);
            this.baseBox.Name = "baseBox";
            this.baseBox.Size = new System.Drawing.Size(267, 20);
            this.baseBox.TabIndex = 9;
            this.baseBox.UseTheme = true;
            this.baseBox.TextChanged += new System.EventHandler(this.baseBox_TextChanged);
            // 
            // classBox
            // 
            this.classBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.classBox.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.classBox.Location = new System.Drawing.Point(106, 34);
            this.classBox.Name = "classBox";
            this.classBox.Size = new System.Drawing.Size(267, 20);
            this.classBox.TabIndex = 7;
            this.classBox.Text = "INewInterface";
            this.classBox.UseTheme = true;
            this.classBox.TextChanged += new System.EventHandler(this.classBox_TextChanged);
            // 
            // packageBox
            // 
            this.packageBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.packageBox.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.packageBox.Location = new System.Drawing.Point(106, 5);
            this.packageBox.Name = "packageBox";
            this.packageBox.Size = new System.Drawing.Size(267, 20);
            this.packageBox.TabIndex = 1;
            this.packageBox.UseTheme = true;
            this.packageBox.TextChanged += new System.EventHandler(this.packageBox_TextChanged);
            // 
            // classLabel
            // 
            this.classLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.classLabel.AutoSize = true;
            this.classLabel.Location = new System.Drawing.Point(3, 37);
            this.classLabel.Name = "classLabel";
            this.classLabel.Size = new System.Drawing.Size(49, 13);
            this.classLabel.TabIndex = 6;
            this.classLabel.Text = "Interface";
            // 
            // baseLabel
            // 
            this.baseLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.baseLabel.AutoSize = true;
            this.baseLabel.Location = new System.Drawing.Point(3, 66);
            this.baseLabel.Name = "baseLabel";
            this.baseLabel.Size = new System.Drawing.Size(75, 13);
            this.baseLabel.TabIndex = 8;
            this.baseLabel.Text = "Base interface";
            // 
            // packageLabel
            // 
            this.packageLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.packageLabel.AutoSize = true;
            this.packageLabel.Location = new System.Drawing.Point(3, 8);
            this.packageLabel.Name = "packageLabel";
            this.packageLabel.Size = new System.Drawing.Size(50, 13);
            this.packageLabel.TabIndex = 0;
            this.packageLabel.Text = "Package";
            // 
            // packageBrowse
            // 
            this.packageBrowse.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.packageBrowse.DisabledBackColor = System.Drawing.SystemColors.Control;
            this.packageBrowse.DisabledTextColor = System.Drawing.SystemColors.ControlDark;
            this.packageBrowse.Location = new System.Drawing.Point(381, 3);
            this.packageBrowse.Name = "packageBrowse";
            this.packageBrowse.Size = new System.Drawing.Size(74, 23);
            this.packageBrowse.TabIndex = 2;
            this.packageBrowse.Text = "Browse...";
            this.packageBrowse.UseTheme = true;
            this.packageBrowse.UseVisualStyleBackColor = true;
            this.packageBrowse.Click += new System.EventHandler(this.packageBrowse_Click);
            // 
            // baseBrowse
            // 
            this.baseBrowse.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.baseBrowse.DisabledBackColor = System.Drawing.SystemColors.Control;
            this.baseBrowse.DisabledTextColor = System.Drawing.SystemColors.ControlDark;
            this.baseBrowse.Location = new System.Drawing.Point(381, 61);
            this.baseBrowse.Name = "baseBrowse";
            this.baseBrowse.Size = new System.Drawing.Size(74, 23);
            this.baseBrowse.TabIndex = 10;
            this.baseBrowse.Text = "Browse...";
            this.baseBrowse.UseTheme = true;
            this.baseBrowse.UseVisualStyleBackColor = true;
            this.baseBrowse.Click += new System.EventHandler(this.baseBrowse_Click);
            // 
            // errorLabel
            // 
            this.errorLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.errorLabel.AutoSize = true;
            this.errorLabel.ForeColor = System.Drawing.Color.Black;
            this.errorLabel.Location = new System.Drawing.Point(25, 4);
            this.errorLabel.Name = "errorLabel";
            this.errorLabel.Size = new System.Drawing.Size(29, 13);
            this.errorLabel.TabIndex = 0;
            this.errorLabel.Text = "Error";
            // 
            // errorIcon
            // 
            this.errorIcon.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.errorIcon.Location = new System.Drawing.Point(3, 3);
            this.errorIcon.Name = "errorIcon";
            this.errorIcon.Size = new System.Drawing.Size(16, 16);
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
            this.flowLayoutPanel6.Location = new System.Drawing.Point(5, 3);
            this.flowLayoutPanel6.Name = "flowLayoutPanel6";
            this.flowLayoutPanel6.Size = new System.Drawing.Size(296, 23);
            this.flowLayoutPanel6.TabIndex = 0;
            // 
            // flowLayoutPanel9
            // 
            this.flowLayoutPanel9.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel9.BackColor = System.Drawing.SystemColors.Window;
            this.flowLayoutPanel9.Controls.Add(this.titleLabel);
            this.flowLayoutPanel9.Location = new System.Drawing.Point(12, 12);
            this.flowLayoutPanel9.Name = "flowLayoutPanel9";
            this.flowLayoutPanel9.Padding = new System.Windows.Forms.Padding(5);
            this.flowLayoutPanel9.Size = new System.Drawing.Size(466, 35);
            this.flowLayoutPanel9.TabIndex = 0;
            // 
            // titleLabel
            // 
            this.titleLabel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.titleLabel.AutoSize = true;
            this.titleLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.titleLabel.Location = new System.Drawing.Point(8, 5);
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new System.Drawing.Size(169, 13);
            this.titleLabel.TabIndex = 0;
            this.titleLabel.Text = "New Actionscript 2 Interface";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.groupBox2.Controls.Add(this.tableLayoutPanel2);
            this.groupBox2.Location = new System.Drawing.Point(10, 53);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(468, 104);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.UseTheme = true;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel2.ColumnCount = 3;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 27.42382F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 72.57618F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 83F));
            this.tableLayoutPanel2.Controls.Add(this.baseBrowse, 2, 2);
            this.tableLayoutPanel2.Controls.Add(this.packageBrowse, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.packageLabel, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.packageBox, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.baseBox, 1, 2);
            this.tableLayoutPanel2.Controls.Add(this.baseLabel, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.classLabel, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.classBox, 1, 1);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 12);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 3;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(460, 88);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.Controls.Add(this.cancelButton);
            this.flowLayoutPanel1.Controls.Add(this.okButton);
            this.flowLayoutPanel1.Controls.Add(this.flowLayoutPanel6);
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(12, 163);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(466, 29);
            this.flowLayoutPanel1.TabIndex = 2;
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.DisabledBackColor = System.Drawing.SystemColors.Control;
            this.cancelButton.DisabledTextColor = System.Drawing.SystemColors.ControlDark;
            this.cancelButton.Location = new System.Drawing.Point(388, 3);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
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
            this.okButton.Location = new System.Drawing.Point(307, 3);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 1;
            this.okButton.Text = "Ok";
            this.okButton.UseTheme = true;
            this.okButton.UseVisualStyleBackColor = true;
            // 
            // AS3InterfaceWizard
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(490, 204);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.flowLayoutPanel9);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AS3InterfaceWizard";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "New ActionScript Interface";
            this.Load += new System.EventHandler(this.AS3ClassWizard_Load);
            ((System.ComponentModel.ISupportInitialize)(this.errorIcon)).EndInit();
            this.flowLayoutPanel6.ResumeLayout(false);
            this.flowLayoutPanel6.PerformLayout();
            this.flowLayoutPanel9.ResumeLayout(false);
            this.flowLayoutPanel9.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label packageLabel;
        private System.Windows.Forms.Label classLabel;
        private System.Windows.Forms.Label baseLabel;
        private System.Windows.Forms.Label errorLabel;
        private System.Windows.Forms.PictureBox errorIcon;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel6;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel9;
        private System.Windows.Forms.Label titleLabel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.TextBoxEx packageBox;
        private System.Windows.Forms.ButtonEx packageBrowse;
        private System.Windows.Forms.TextBoxEx classBox;
        private System.Windows.Forms.TextBoxEx baseBox;
        private System.Windows.Forms.ButtonEx baseBrowse;
        private System.Windows.Forms.GroupBoxEx groupBox2;
        private System.Windows.Forms.ButtonEx cancelButton;
        private System.Windows.Forms.ButtonEx okButton;
    }
}

