namespace SharedObjectReader.Controls
{
    partial class panelAMF3Int
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.prop_name = new System.Windows.Forms.TextBox();
            this.prop_value = new System.Windows.Forms.TextBox();
            this.SuspendLayout();

            this.Dock = System.Windows.Forms.DockStyle.Fill;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "&Property Name:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(22, 67);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "&Value:";
            // 
            // prop_name
            // 
            this.prop_name.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.prop_name.Location = new System.Drawing.Point(130, 21);
            this.prop_name.Name = "prop_name";
            this.prop_name.Size = new System.Drawing.Size(1057, 20);
            this.prop_name.TabIndex = 2;
            // 
            // prop_value
            // 
            this.prop_value.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.prop_value.Location = new System.Drawing.Point(130, 64);
            this.prop_value.Name = "prop_value";
            this.prop_value.Size = new System.Drawing.Size(1057, 20);
            this.prop_value.TabIndex = 3;
            // 
            // panelAMF0Number
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.prop_value);
            this.Controls.Add(this.prop_name);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "panelAMF0Number";
            this.Size = new System.Drawing.Size(1201, 659);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox prop_name;
        private System.Windows.Forms.TextBox prop_value;
    }
}
