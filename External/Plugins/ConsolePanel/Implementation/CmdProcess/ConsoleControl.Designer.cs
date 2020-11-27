namespace ConsolePanel.Implementation.CmdProcess
{
    partial class ConsoleControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pnlClipping = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // pnlClipping
            // 
            this.pnlClipping.Location = new System.Drawing.Point(0, 0);
            this.pnlClipping.Name = "pnlClipping";
            this.pnlClipping.Size = new System.Drawing.Size(128, 103);
            this.pnlClipping.TabIndex = 0;
            this.pnlClipping.Enter += new System.EventHandler(this.CmdPanel_Enter);
            // 
            // ConsoleControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.pnlClipping);
            this.ForeColor = System.Drawing.Color.White;
            this.Name = "ConsoleControl";
            this.Size = new System.Drawing.Size(246, 149);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.ConsoleControl_Paint);
            this.Enter += new System.EventHandler(this.CmdPanel_Enter);
            this.Resize += new System.EventHandler(this.CmdPanel_Resize);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlClipping;
    }
}
