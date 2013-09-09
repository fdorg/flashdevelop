namespace SharedObjectReader.Controls
{
    partial class panelSharedObject
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
            this.label3 = new System.Windows.Forms.Label();
            this.so_encoding = new System.Windows.Forms.Label();
            this.so_size = new System.Windows.Forms.Label();
            this.so_name = new System.Windows.Forms.Label();
            this.rawView = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(106, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "SharedObject Name:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(22, 44);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(49, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "File Size:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(22, 64);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(80, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "AMF Encoding:";
            // 
            // so_encoding
            // 
            this.so_encoding.AutoSize = true;
            this.so_encoding.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.so_encoding.Location = new System.Drawing.Point(164, 64);
            this.so_encoding.Name = "so_encoding";
            this.so_encoding.Size = new System.Drawing.Size(93, 13);
            this.so_encoding.TabIndex = 5;
            this.so_encoding.Text = "AMF Encoding:";
            // 
            // so_size
            // 
            this.so_size.AutoSize = true;
            this.so_size.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.so_size.Location = new System.Drawing.Point(164, 44);
            this.so_size.Name = "so_size";
            this.so_size.Size = new System.Drawing.Size(59, 13);
            this.so_size.TabIndex = 4;
            this.so_size.Text = "File Size:";
            // 
            // so_name
            // 
            this.so_name.AutoSize = true;
            this.so_name.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.so_name.Location = new System.Drawing.Point(164, 24);
            this.so_name.Name = "so_name";
            this.so_name.Size = new System.Drawing.Size(124, 13);
            this.so_name.TabIndex = 3;
            this.so_name.Text = "SharedObject Name:";
            // 
            // rawView
            // 
            this.rawView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.rawView.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rawView.Location = new System.Drawing.Point(28, 128);
            this.rawView.Multiline = true;
            this.rawView.Name = "rawView";
            this.rawView.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.rawView.Size = new System.Drawing.Size(1170, 585);
            this.rawView.TabIndex = 6;
            // 
            // panelSharedObject
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.rawView);
            this.Controls.Add(this.so_encoding);
            this.Controls.Add(this.so_size);
            this.Controls.Add(this.so_name);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "panelSharedObject";
            this.Size = new System.Drawing.Size(1201, 716);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label so_encoding;
        private System.Windows.Forms.Label so_size;
        private System.Windows.Forms.Label so_name;
        private System.Windows.Forms.TextBox rawView;
    }
}
