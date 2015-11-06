
using ASClassWizard.Resources;

namespace ASClassWizard.Wizards
{
    partial class AS3ClassWizard
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
            this.flowLayoutPanel5 = new System.Windows.Forms.FlowLayoutPanel();
            this.constructorCheck = new System.Windows.Forms.CheckBox();
            this.superCheck = new System.Windows.Forms.CheckBox();
            this.flowLayoutPanel4 = new System.Windows.Forms.FlowLayoutPanel();
            this.implementBrowse = new System.Windows.Forms.Button();
            this.implementRemove = new System.Windows.Forms.Button();
            this.baseBox = new System.Windows.Forms.TextBox();
            this.classBox = new System.Windows.Forms.TextBox();
            this.packageBox = new System.Windows.Forms.TextBox();
            this.classLabel = new System.Windows.Forms.Label();
            this.accessLabel = new System.Windows.Forms.Label();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.publicRadio = new System.Windows.Forms.RadioButton();
            this.internalRadio = new System.Windows.Forms.RadioButton();
            this.flowLayoutPanel3 = new System.Windows.Forms.FlowLayoutPanel();
            this.dynamicCheck = new System.Windows.Forms.CheckBox();
            this.finalCheck = new System.Windows.Forms.CheckBox();
            this.baseLabel = new System.Windows.Forms.Label();
            this.generationLabel = new System.Windows.Forms.Label();
            this.packageLabel = new System.Windows.Forms.Label();
            this.packageBrowse = new System.Windows.Forms.Button();
            this.baseBrowse = new System.Windows.Forms.Button();
            this.errorLabel = new System.Windows.Forms.Label();
            this.errorIcon = new System.Windows.Forms.PictureBox();
            this.flowLayoutPanel6 = new System.Windows.Forms.FlowLayoutPanel();
            this.flowLayoutPanel9 = new System.Windows.Forms.FlowLayoutPanel();
            this.titleLabel = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.implementLabel = new System.Windows.Forms.Label();
            this.implementList = new System.Windows.Forms.ListBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.flowLayoutPanel5.SuspendLayout();
            this.flowLayoutPanel4.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.flowLayoutPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorIcon)).BeginInit();
            this.flowLayoutPanel6.SuspendLayout();
            this.flowLayoutPanel9.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutPanel5
            // 
            this.flowLayoutPanel5.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel5.Controls.Add(this.constructorCheck);
            this.flowLayoutPanel5.Controls.Add(this.superCheck);
            this.flowLayoutPanel5.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel5.Location = new System.Drawing.Point(106, 206);
            this.flowLayoutPanel5.Name = "flowLayoutPanel5";
            this.flowLayoutPanel5.Size = new System.Drawing.Size(268, 54);
            this.flowLayoutPanel5.TabIndex = 15;
            // 
            // constructorCheck
            // 
            this.constructorCheck.AutoSize = true;
            this.constructorCheck.Enabled = false;
            this.constructorCheck.Checked = true;
            this.constructorCheck.Location = new System.Drawing.Point(3, 3);
            this.constructorCheck.Name = "constructorCheck";
            this.constructorCheck.Size = new System.Drawing.Size(127, 17);
            this.constructorCheck.TabIndex = 0;
            this.constructorCheck.Text = "Generate Constructor";
            this.constructorCheck.UseVisualStyleBackColor = true;
            // 
            // superCheck
            // 
            this.superCheck.AutoSize = true;
            this.superCheck.Enabled = false;
            this.superCheck.Checked = true;
            this.superCheck.Location = new System.Drawing.Point(3, 26);
            this.superCheck.Name = "superCheck";
            this.superCheck.Size = new System.Drawing.Size(120, 17);
            this.superCheck.TabIndex = 1;
            this.superCheck.Text = "Generate Super call";
            this.superCheck.UseVisualStyleBackColor = true;
            // 
            // flowLayoutPanel4
            // 
            this.flowLayoutPanel4.Controls.Add(this.implementBrowse);
            this.flowLayoutPanel4.Controls.Add(this.implementRemove);
            this.flowLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill; 
            this.flowLayoutPanel4.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel4.Location = new System.Drawing.Point(377, 140);
            this.flowLayoutPanel4.Margin = new System.Windows.Forms.Padding(3,0,0,0);
            this.flowLayoutPanel4.Name = "flowLayoutPanel4";
            this.flowLayoutPanel4.Size = new System.Drawing.Size(80, 62);
            this.flowLayoutPanel4.TabIndex = 13;
            // 
            // implementBrowse
            // 
            this.implementBrowse.Location = new System.Drawing.Point(3, 3);
            this.implementBrowse.Name = "implementBrowse";
            this.implementBrowse.Size = new System.Drawing.Size(74, 23);
            this.implementBrowse.TabIndex = 0;
            this.implementBrowse.Text = "Browse...";
            this.implementBrowse.UseVisualStyleBackColor = true;
            this.implementBrowse.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.implementBrowse.Click += new System.EventHandler(this.implementBrowse_Click);
            // 
            // implementRemove
            // 
            this.implementRemove.Enabled = false;
            this.implementRemove.Location = new System.Drawing.Point(3, 32);
            this.implementRemove.Name = "implementRemove";
            this.implementRemove.Size = new System.Drawing.Size(74, 23);
            this.implementRemove.TabIndex = 1;
            this.implementRemove.Text = "Remove";
            this.implementRemove.UseVisualStyleBackColor = true;
            this.implementRemove.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.implementRemove.Click += new System.EventHandler(this.interfaceRemove_Click);
            // 
            // baseBox
            // 
            this.baseBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.baseBox.Enabled = false;
            this.baseBox.Location = new System.Drawing.Point(106, 116);
            this.baseBox.Name = "baseBox";
            this.baseBox.Size = new System.Drawing.Size(268, 20);
            this.baseBox.TabIndex = 9;
            this.baseBox.TextChanged += new System.EventHandler(this.baseBox_TextChanged);
            // 
            // classBox
            // 
            this.classBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.classBox.Location = new System.Drawing.Point(106, 88);
            this.classBox.Name = "classBox";
            this.classBox.Size = new System.Drawing.Size(268, 20);
            this.classBox.TabIndex = 7;
            this.classBox.Text = "NewClass";
            this.classBox.TextChanged += new System.EventHandler(this.classBox_TextChanged);
            // 
            // packageBox
            // 
            this.packageBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.packageBox.Location = new System.Drawing.Point(106, 4);
            this.packageBox.Name = "packageBox";
            this.packageBox.Size = new System.Drawing.Size(268, 20);
            this.packageBox.TabIndex = 1;
            this.packageBox.TextChanged += new System.EventHandler(this.packageBox_TextChanged);
            // 
            // classLabel
            // 
            this.classLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.classLabel.AutoSize = true;
            this.classLabel.Location = new System.Drawing.Point(3, 91);
            this.classLabel.Name = "classLabel";
            this.classLabel.Size = new System.Drawing.Size(32, 13);
            this.classLabel.TabIndex = 6;
            this.classLabel.Text = "Class";
            // 
            // accessLabel
            // 
            this.accessLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.accessLabel.AutoSize = true;
            this.accessLabel.Location = new System.Drawing.Point(3, 35);
            this.accessLabel.Name = "accessLabel";
            this.accessLabel.Size = new System.Drawing.Size(42, 13);
            this.accessLabel.TabIndex = 3;
            this.accessLabel.Text = "Access";
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.Controls.Add(this.publicRadio);
            this.flowLayoutPanel2.Controls.Add(this.internalRadio);
            this.flowLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel2.Location = new System.Drawing.Point(106, 31);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(268, 22);
            this.flowLayoutPanel2.TabIndex = 4;
            // 
            // publicRadio
            // 
            this.publicRadio.AutoSize = true;
            this.publicRadio.Checked = true;
            this.publicRadio.Location = new System.Drawing.Point(3, 3);
            this.publicRadio.Name = "publicRadio";
            this.publicRadio.Size = new System.Drawing.Size(53, 17);
            this.publicRadio.TabIndex = 0;
            this.publicRadio.TabStop = true;
            this.publicRadio.Text = "public";
            this.publicRadio.UseVisualStyleBackColor = true;
            // 
            // internalRadio
            // 
            this.internalRadio.AutoSize = true;
            this.internalRadio.Location = new System.Drawing.Point(62, 3);
            this.internalRadio.Name = "internalRadio";
            this.internalRadio.Size = new System.Drawing.Size(59, 17);
            this.internalRadio.TabIndex = 1;
            this.internalRadio.Text = "internal";
            this.internalRadio.UseVisualStyleBackColor = true;
            // 
            // flowLayoutPanel3
            // 
            this.flowLayoutPanel3.Controls.Add(this.dynamicCheck);
            this.flowLayoutPanel3.Controls.Add(this.finalCheck);
            this.flowLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel3.Location = new System.Drawing.Point(106, 59);
            this.flowLayoutPanel3.Name = "flowLayoutPanel3";
            this.flowLayoutPanel3.Size = new System.Drawing.Size(268, 22);
            this.flowLayoutPanel3.TabIndex = 5;
            // 
            // dynamicCheck
            // 
            this.dynamicCheck.AutoSize = true;
            this.dynamicCheck.Location = new System.Drawing.Point(3, 3);
            this.dynamicCheck.Name = "dynamicCheck";
            this.dynamicCheck.Size = new System.Drawing.Size(65, 17);
            this.dynamicCheck.TabIndex = 0;
            this.dynamicCheck.Text = "dynamic";
            this.dynamicCheck.UseVisualStyleBackColor = true;
            // 
            // finalCheck
            // 
            this.finalCheck.AutoSize = true;
            this.finalCheck.Location = new System.Drawing.Point(74, 3);
            this.finalCheck.Name = "finalCheck";
            this.finalCheck.Size = new System.Drawing.Size(45, 17);
            this.finalCheck.TabIndex = 1;
            this.finalCheck.Text = "final";
            this.finalCheck.UseVisualStyleBackColor = true;
            // 
            // baseLabel
            // 
            this.baseLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.baseLabel.AutoSize = true;
            this.baseLabel.Location = new System.Drawing.Point(3, 119);
            this.baseLabel.Name = "baseLabel";
            this.baseLabel.Size = new System.Drawing.Size(58, 13);
            this.baseLabel.TabIndex = 8;
            this.baseLabel.Text = "Base class";
            // 
            // generationLabel
            // 
            this.generationLabel.AutoSize = true;
            this.generationLabel.Location = new System.Drawing.Point(3, 203);
            this.generationLabel.Name = "generationLabel";
            this.generationLabel.Padding = new System.Windows.Forms.Padding(0, 6, 0, 0);
            this.generationLabel.Size = new System.Drawing.Size(85, 19);
            this.generationLabel.TabIndex = 14;
            this.generationLabel.Text = "Code generation";
            // 
            // packageLabel
            // 
            this.packageLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.packageLabel.AutoSize = true;
            this.packageLabel.Location = new System.Drawing.Point(3, 7);
            this.packageLabel.Name = "packageLabel";
            this.packageLabel.Size = new System.Drawing.Size(50, 13);
            this.packageLabel.TabIndex = 0;
            this.packageLabel.Text = "Package";
            // 
            // packageBrowse
            // 
            this.packageBrowse.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.packageBrowse.Location = new System.Drawing.Point(381, 3);
            this.packageBrowse.Name = "packageBrowse";
            this.packageBrowse.Size = new System.Drawing.Size(74, 23);
            this.packageBrowse.TabIndex = 2;
            this.packageBrowse.Text = "Browse...";
            this.packageBrowse.UseVisualStyleBackColor = true;
            this.packageBrowse.Click += new System.EventHandler(this.packageBrowse_Click);
            // 
            // baseBrowse
            // 
            this.baseBrowse.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.baseBrowse.Location = new System.Drawing.Point(381, 115);
            this.baseBrowse.Name = "baseBrowse";
            this.baseBrowse.Size = new System.Drawing.Size(74, 23);
            this.baseBrowse.TabIndex = 10;
            this.baseBrowse.Text = "Browse...";
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
            this.errorLabel.Size = new System.Drawing.Size(34, 13);
            this.errorLabel.TabIndex = 0;
            this.errorLabel.Text = "Error";
            // 
            // errorIcon
            //
            this.errorIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.errorIcon.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.errorIcon.Location = new System.Drawing.Point(3, 3);
            this.errorIcon.Name = "errorIcon";
            this.errorIcon.Size = new System.Drawing.Size(16, 16);
            this.errorIcon.TabIndex = 0;
            this.errorIcon.TabStop = false;
            // 
            // flowLayoutPanel6
            // 
            this.flowLayoutPanel6.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel6.Controls.Add(this.errorIcon);
            this.flowLayoutPanel6.Controls.Add(this.errorLabel);
            this.flowLayoutPanel6.Location = new System.Drawing.Point(3, 3);
            this.flowLayoutPanel6.Name = "flowLayoutPanel6";
            this.flowLayoutPanel6.Size = new System.Drawing.Size(296, 23);
            this.flowLayoutPanel6.TabIndex = 0;
            // 
            // flowLayoutPanel9
            // 
            this.flowLayoutPanel9.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
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
            this.titleLabel.Size = new System.Drawing.Size(148, 13);
            this.titleLabel.TabIndex = 0;
            this.titleLabel.Text = "New Actionscript 2 Class";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.tableLayoutPanel2);
            this.groupBox2.Location = new System.Drawing.Point(10, 53);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(468, 279);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel2.ColumnCount = 3;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 27.42382F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 72.57618F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 82F));
            this.tableLayoutPanel2.Controls.Add(this.flowLayoutPanel5, 1, 6);
            this.tableLayoutPanel2.Controls.Add(this.baseBrowse, 2, 4);
            this.tableLayoutPanel2.Controls.Add(this.generationLabel, 0, 6);
            this.tableLayoutPanel2.Controls.Add(this.flowLayoutPanel4, 2, 5);
            this.tableLayoutPanel2.Controls.Add(this.packageBrowse, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.packageLabel, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.implementLabel, 0, 5);
            this.tableLayoutPanel2.Controls.Add(this.packageBox, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.baseBox, 1, 4);
            this.tableLayoutPanel2.Controls.Add(this.baseLabel, 0, 4);
            this.tableLayoutPanel2.Controls.Add(this.implementList, 1, 5);
            this.tableLayoutPanel2.Controls.Add(this.accessLabel, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.classLabel, 0, 3);
            this.tableLayoutPanel2.Controls.Add(this.flowLayoutPanel2, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.flowLayoutPanel3, 1, 2);
            this.tableLayoutPanel2.Controls.Add(this.classBox, 1, 3);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 12);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 7;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 63F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(460, 263);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // implementLabel
            // 
            this.implementLabel.AutoSize = true;
            this.implementLabel.Location = new System.Drawing.Point(3, 140);
            this.implementLabel.Name = "implementLabel";
            this.implementLabel.Padding = new System.Windows.Forms.Padding(0, 6, 0, 0);
            this.implementLabel.Size = new System.Drawing.Size(55, 19);
            this.implementLabel.TabIndex = 11;
            this.implementLabel.Text = "Implement";
            // 
            // implementList
            // 
            this.implementList.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.implementList.FormattingEnabled = true;
            this.implementList.Location = new System.Drawing.Point(106, 143);
            this.implementList.Name = "implementList";
            this.implementList.Size = new System.Drawing.Size(268, 56);
            this.implementList.TabIndex = 12;
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
            this.flowLayoutPanel1.Location = new System.Drawing.Point(12, 338);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(466, 29);
            this.flowLayoutPanel1.TabIndex = 2;
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(388, 3);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 2;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // okButton
            // 
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Enabled = false;
            this.okButton.Location = new System.Drawing.Point(307, 3);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 1;
            this.okButton.Text = "Ok";
            this.okButton.UseVisualStyleBackColor = true;
            // 
            // AS3ClassWizard
            //
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(490, 379);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.flowLayoutPanel9);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Name = "AS3ClassWizard";
            this.Text = "New ActionScript Class";
            this.Load += new System.EventHandler(this.AS3ClassWizard_Load);
            this.flowLayoutPanel5.ResumeLayout(false);
            this.flowLayoutPanel5.PerformLayout();
            this.flowLayoutPanel4.ResumeLayout(false);
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel2.PerformLayout();
            this.flowLayoutPanel3.ResumeLayout(false);
            this.flowLayoutPanel3.PerformLayout();
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
        private System.Windows.Forms.TextBox packageBox;
        private System.Windows.Forms.Button packageBrowse;
        private System.Windows.Forms.Label classLabel;
        private System.Windows.Forms.TextBox classBox;
        private System.Windows.Forms.Label accessLabel;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.RadioButton publicRadio;
        private System.Windows.Forms.RadioButton internalRadio;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel3;
        private System.Windows.Forms.CheckBox dynamicCheck;
        private System.Windows.Forms.CheckBox finalCheck;
        private System.Windows.Forms.Label baseLabel;
        private System.Windows.Forms.TextBox baseBox;
        private System.Windows.Forms.Button baseBrowse;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel4;
        private System.Windows.Forms.Button implementBrowse;
        private System.Windows.Forms.Button implementRemove;
        private System.Windows.Forms.Label generationLabel;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel5;
        private System.Windows.Forms.CheckBox constructorCheck;
        private System.Windows.Forms.CheckBox superCheck;
        private System.Windows.Forms.Label errorLabel;
        private System.Windows.Forms.PictureBox errorIcon;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel6;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel9;
        private System.Windows.Forms.Label titleLabel;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label implementLabel;
        private System.Windows.Forms.ListBox implementList;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
    }
}

