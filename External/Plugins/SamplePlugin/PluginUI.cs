using System;
using System.Collections;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI;
using PluginCore;

namespace SamplePlugin
{
    public class PluginUI : UserControl
    {
        private RichTextBox richTextBox;
        private PluginMain pluginMain;
        
        public PluginUI(PluginMain pluginMain)
        {
            this.InitializeComponent();
            this.pluginMain = pluginMain;
        }

        /// <summary>
        /// Accessor to the RichTextBox
        /// </summary>
        public RichTextBox Output
        {
            get { return this.richTextBox; }
        }
        
        #region Windows Forms Designer Generated Code

        /// <summary>
        /// This method is required for Windows Forms designer support.
        /// Do not change the method contents inside the source code editor. The Forms designer might
        /// not be able to load this method if it was changed manually.
        /// </summary>
        private void InitializeComponent() 
        {
            this.richTextBox = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // richTextBox
            // 
            this.richTextBox.DetectUrls = false;
            this.richTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBox.Location = new System.Drawing.Point(0, 0);
            this.richTextBox.Name = "richTextBox";
            this.richTextBox.Size = new System.Drawing.Size(280, 352);
            this.richTextBox.TabIndex = 0;
            this.richTextBox.Text = "";
            // 
            // PluginUI
            // 
            this.Controls.Add(this.richTextBox);
            this.Name = "PluginUI";
            this.Size = new System.Drawing.Size(280, 352);
            this.ResumeLayout(false);

        }

        #endregion
                
    }

}
