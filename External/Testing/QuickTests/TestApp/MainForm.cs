// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using System;
using System.Windows.Forms;

namespace TestApp
{
    public class MainForm : Form
    {
        public MainForm()
        {
            this.SuspendLayout();
            this.CreateContent();
            this.ResumeLayout(false);
        }

        public void CreateContent()
        {
            this.Text = "TestApp for QuickTests";
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 600);
            //

        }

    }

}
