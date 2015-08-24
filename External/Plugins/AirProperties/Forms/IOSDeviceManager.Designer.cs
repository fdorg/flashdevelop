using System.ComponentModel;
using System.Windows.Forms;
using AirProperties.Controls;

namespace AirProperties.Forms
{
    partial class IOSDeviceManager
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

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
            this.components = new System.ComponentModel.Container();
            this.OKButton = new System.Windows.Forms.Button();
            this.CancelButton1 = new System.Windows.Forms.Button();
            this.IOSDevicesTree = new AirProperties.Controls.TriStateTreeView();
            this.SuspendLayout();
            // 
            // OKButton
            // 
            this.OKButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.OKButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OKButton.Location = new System.Drawing.Point(100, 265);
            this.OKButton.Name = "OKButton";
            this.OKButton.Size = new System.Drawing.Size(75, 23);
            this.OKButton.TabIndex = 1;
            this.OKButton.Text = "OK";
            this.OKButton.UseVisualStyleBackColor = true;
            this.OKButton.Click += new System.EventHandler(this.OKButton_Click);
            // 
            // CancelButton1
            // 
            this.CancelButton1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.CancelButton1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.CancelButton1.Location = new System.Drawing.Point(181, 265);
            this.CancelButton1.Name = "CancelButton1";
            this.CancelButton1.Size = new System.Drawing.Size(75, 23);
            this.CancelButton1.TabIndex = 2;
            this.CancelButton1.Text = "Cancel";
            this.CancelButton1.UseVisualStyleBackColor = true;
            // 
            // IOSDevicesTree
            // 
            this.IOSDevicesTree.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.IOSDevicesTree.Location = new System.Drawing.Point(12, 12);
            this.IOSDevicesTree.Name = "IOSDevicesTree";
            this.IOSDevicesTree.Size = new System.Drawing.Size(244, 247);
            this.IOSDevicesTree.TabIndex = 0;
            this.IOSDevicesTree.TriStateStyleProperty = AirProperties.Controls.TriStateTreeView.TriStateStyles.Installer;
            // 
            // IOSDeviceManager
            // 
            this.AcceptButton = this.OKButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.CancelButton1;
            this.ClientSize = new System.Drawing.Size(268, 300);
            this.Controls.Add(this.CancelButton1);
            this.Controls.Add(this.OKButton);
            this.Controls.Add(this.IOSDevicesTree);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "IOSDeviceManager";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Select target iOS devices";
            this.ResumeLayout(false);

        }

        #endregion

        private TriStateTreeView IOSDevicesTree;
        private Button OKButton;
        private Button CancelButton1;
    }
}