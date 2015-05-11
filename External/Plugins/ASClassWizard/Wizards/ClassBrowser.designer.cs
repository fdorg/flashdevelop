using ProjectManager.Controls.TreeView;
using ProjectManager.Projects;
using ASClassWizard.Resources;

namespace ASClassWizard.Wizards
{
    partial class ClassBrowser
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
            this.components = new System.ComponentModel.Container();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.itemList = new ASClassWizard.Wizards.GListBox();
            this.classBrowserBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.filterBox = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.classBrowserBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(219, 270);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 3;
            this.cancelButton.Text = "&Select None";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(137, 270);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 2;
            this.okButton.Text = "&Ok";
            this.okButton.UseVisualStyleBackColor = true;
            // 
            // itemList
            // 
            this.itemList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.itemList.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.itemList.FormattingEnabled = true;
            this.itemList.ImageList = null;
            this.itemList.Location = new System.Drawing.Point(12, 40);
            this.itemList.Name = "itemList";
            this.itemList.Size = new System.Drawing.Size(282, 225);
            this.itemList.TabIndex = 1;
            this.itemList.DoubleClick += new System.EventHandler(this.itemList_DoubleClick);
            // 
            // filterBox
            // 
            this.filterBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.filterBox.Location = new System.Drawing.Point(12, 12);
            this.filterBox.Name = "filterBox";
            this.filterBox.Size = new System.Drawing.Size(282, 20);
            this.filterBox.TabIndex = 0;
            this.filterBox.TextChanged += new System.EventHandler(this.filterBox_TextChanged);
            this.filterBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.filterBox_KeyDown);
            // 
            // ClassBrowser
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(306, 303);
            this.Controls.Add(this.filterBox);
            this.Controls.Add(this.itemList);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            this.Name = "ClassBrowser";
            this.Text = "Open Type";
            this.Load += new System.EventHandler(this.ClassBrowser_Load);
            ((System.ComponentModel.ISupportInitialize)(this.classBrowserBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.BindingSource classBrowserBindingSource;
        private GListBox itemList;
        private System.Windows.Forms.TextBox filterBox;
    }
}