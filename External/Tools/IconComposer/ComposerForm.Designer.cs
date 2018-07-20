// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
namespace IconComposer
{
    partial class ComposerForm
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.buttonPreview = new System.Windows.Forms.Button();
            this.buttonClear = new System.Windows.Forms.Button();
            this.labelCode = new System.Windows.Forms.TextBox();
            this.labelY = new System.Windows.Forms.Label();
            this.labelSer = new System.Windows.Forms.Label();
            this.labelBullet = new System.Windows.Forms.Label();
            this.labelPreview = new System.Windows.Forms.Label();
            this.labelX = new System.Windows.Forms.Label();
            this.numY = new System.Windows.Forms.NumericUpDown();
            this.numX = new System.Windows.Forms.NumericUpDown();
            this.previewBox = new System.Windows.Forms.PictureBox();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.previewBox)).BeginInit();
            this.SuspendLayout();
            //
            // panel1
            //
            this.panel1.AutoScroll = true;
            this.panel1.Controls.Add(this.pictureBox);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(307, 504);
            this.panel1.TabIndex = 1;
            //
            // pictureBox
            //
            this.pictureBox.Location = new System.Drawing.Point(3, 2);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(101, 86);
            this.pictureBox.TabIndex = 2;
            this.pictureBox.TabStop = false;
            this.pictureBox.Click += new System.EventHandler(this.pictureBox_Click);
            this.pictureBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseMove);
            //
            // panel2
            //
            this.panel2.BackColor = System.Drawing.SystemColors.Control;
            this.panel2.Controls.Add(this.buttonPreview);
            this.panel2.Controls.Add(this.buttonClear);
            this.panel2.Controls.Add(this.labelCode);
            this.panel2.Controls.Add(this.labelY);
            this.panel2.Controls.Add(this.labelSer);
            this.panel2.Controls.Add(this.labelBullet);
            this.panel2.Controls.Add(this.labelPreview);
            this.panel2.Controls.Add(this.labelX);
            this.panel2.Controls.Add(this.numY);
            this.panel2.Controls.Add(this.numX);
            this.panel2.Controls.Add(this.previewBox);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel2.Location = new System.Drawing.Point(307, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(102, 504);
            this.panel2.TabIndex = 4;
            //
            // buttonPreview
            //
            this.buttonPreview.Location = new System.Drawing.Point(6, 195);
            this.buttonPreview.Name = "buttonPreview";
            this.buttonPreview.Size = new System.Drawing.Size(91, 24);
            this.buttonPreview.TabIndex = 4;
            this.buttonPreview.Text = "Deserialize";
            this.buttonPreview.UseVisualStyleBackColor = true;
            this.buttonPreview.Click += new System.EventHandler(this.buttonPreview_Click);
            //
            // buttonClear
            //
            this.buttonClear.Location = new System.Drawing.Point(6, 111);
            this.buttonClear.Name = "buttonClear";
            this.buttonClear.Size = new System.Drawing.Size(91, 24);
            this.buttonClear.TabIndex = 3;
            this.buttonClear.Text = "Clear";
            this.buttonClear.UseVisualStyleBackColor = true;
            this.buttonClear.Click += new System.EventHandler(this.buttonClear_Click);
            //
            // labelCode
            //
            this.labelCode.Location = new System.Drawing.Point(6, 168);
            this.labelCode.Name = "labelCode";
            this.labelCode.Size = new System.Drawing.Size(91, 21);
            this.labelCode.TabIndex = 0;
            //
            // labelY
            //
            this.labelY.AutoSize = true;
            this.labelY.Location = new System.Drawing.Point(8, 87);
            this.labelY.Name = "labelY";
            this.labelY.Size = new System.Drawing.Size(51, 13);
            this.labelY.TabIndex = 3;
            this.labelY.Text = "Y Offset:";
            //
            // labelSer
            //
            this.labelSer.AutoSize = true;
            this.labelSer.Location = new System.Drawing.Point(5, 150);
            this.labelSer.Name = "labelSer";
            this.labelSer.Size = new System.Drawing.Size(68, 13);
            this.labelSer.TabIndex = 3;
            this.labelSer.Text = "Serialization:";
            //
            // labelBullet
            //
            this.labelBullet.AutoSize = true;
            this.labelBullet.Location = new System.Drawing.Point(8, 44);
            this.labelBullet.Name = "labelBullet";
            this.labelBullet.Size = new System.Drawing.Size(37, 13);
            this.labelBullet.TabIndex = 3;
            this.labelBullet.Text = "Bullet:";
            //
            // labelPreview
            //
            this.labelPreview.AutoSize = true;
            this.labelPreview.Location = new System.Drawing.Point(8, 16);
            this.labelPreview.Name = "labelPreview";
            this.labelPreview.Size = new System.Drawing.Size(49, 13);
            this.labelPreview.TabIndex = 3;
            this.labelPreview.Text = "Preview:";
            //
            // labelX
            //
            this.labelX.AutoSize = true;
            this.labelX.Location = new System.Drawing.Point(8, 63);
            this.labelX.Name = "labelX";
            this.labelX.Size = new System.Drawing.Size(51, 13);
            this.labelX.TabIndex = 3;
            this.labelX.Text = "X Offset:";
            //
            // numY
            //
            this.numY.Location = new System.Drawing.Point(60, 84);
            this.numY.Maximum = new decimal(new int[] {
            16,
            0,
            0,
            0});
            this.numY.Minimum = new decimal(new int[] {
            16,
            0,
            0,
            -2147483648});
            this.numY.Name = "numY";
            this.numY.Size = new System.Drawing.Size(39, 21);
            this.numY.TabIndex = 2;
            this.numY.ValueChanged += new System.EventHandler(this.numY_ValueChanged);
            //
            // numX
            //
            this.numX.Location = new System.Drawing.Point(60, 60);
            this.numX.Maximum = new decimal(new int[] {
            16,
            0,
            0,
            0});
            this.numX.Minimum = new decimal(new int[] {
            16,
            0,
            0,
            -2147483648});
            this.numX.Name = "numX";
            this.numX.Size = new System.Drawing.Size(39, 21);
            this.numX.TabIndex = 1;
            this.numX.ValueChanged += new System.EventHandler(this.numX_ValueChanged);
            //
            // previewBox
            //
            this.previewBox.Location = new System.Drawing.Point(66, 15);
            this.previewBox.Name = "previewBox";
            this.previewBox.Size = new System.Drawing.Size(16, 16);
            this.previewBox.TabIndex = 1;
            this.previewBox.TabStop = false;
            //
            // ComposerForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.ClientSize = new System.Drawing.Size(409, 504);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel2);
            this.MaximizeBox = false;
            this.Name = "ComposerForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = " Icon Composer";
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.previewBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.PictureBox previewBox;
        private System.Windows.Forms.Label labelY;
        private System.Windows.Forms.Label labelX;
        private System.Windows.Forms.NumericUpDown numY;
        private System.Windows.Forms.NumericUpDown numX;
        private System.Windows.Forms.TextBox labelCode;
        private System.Windows.Forms.Button buttonClear;
        private System.Windows.Forms.Button buttonPreview;
        private System.Windows.Forms.Label labelSer;
        private System.Windows.Forms.Label labelPreview;
        private System.Windows.Forms.Label labelBullet;

    }
}

