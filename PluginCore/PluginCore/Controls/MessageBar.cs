using System;
using System.IO;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI;
using PluginCore.Helpers;

namespace PluginCore.Controls
{
    public class MessageBar : UserControl
    {
        static public bool Locked;

        private ToolTip tip;
        private Label label;
        private InertButton buttonClose;
        private String currentMessage;

        public MessageBar()
        {
            this.InitializeComponent();
            this.InitializeGraphics();
        }
        
        static public void ShowWarning(String message)
        {
            if (Locked) return;
            CreateBar(PluginBase.MainForm.CurrentDocument, message);
        }

        static public void HideWarning()
        {
            HideBar(PluginBase.MainForm.CurrentDocument);
        }

        static private MessageBar CreateBar(ITabbedDocument target, String message)
        {
            MessageBar bar;
            foreach (Control ctrl in target.Controls)
            {
                if (ctrl is MessageBar)
                {
                    bar = (ctrl as MessageBar);
                    bar.Update(message);
                    bar.Visible = true;
                    return bar;
                }
            }
            bar = new MessageBar();
            bar.Visible = false;
            target.Controls.Add(bar);
            bar.Dock = DockStyle.Top;
            bar.Update(message);
            bar.SendToBack();
            bar.Visible = true;
            return bar;
        }

        static public void HideBar(ITabbedDocument target)
        {
            foreach (Control ctrl in target.Controls)
            {
                if (ctrl is MessageBar)
                {
                    (ctrl as MessageBar).MessageBarClick(null, null);
                    return;
                }
            }
        }
        
        #region InitializeComponent
        
        public void InitializeComponent() 
        {
            this.buttonClose = new WeifenLuo.WinFormsUI.InertButton();
            this.label = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // buttonClose
            // 
            this.buttonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClose.Location = new System.Drawing.Point(477, 5);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(16, 14);
            this.buttonClose.TabIndex = 2;
            this.buttonClose.Click += new System.EventHandler(this.MessageBarClick);
            // 
            // label
            // 
            this.label.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
            this.label.ForeColor = System.Drawing.SystemColors.InfoText;
            this.label.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label.Location = new System.Drawing.Point(2, 0);
            this.label.Name = "label";
            this.label.Size = new System.Drawing.Size(494, 22);
            this.label.TabIndex = 0;
            this.label.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.label.Click += new System.EventHandler(this.MessageBarClick);
            this.label.MouseEnter += new System.EventHandler(this.LabelMouseEnter);
            this.label.MouseLeave += new System.EventHandler(this.LabelMouseLeave);
            // 
            // MessageBar
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.SystemColors.Info;
            this.ForeColor = System.Drawing.SystemColors.InfoText;
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.label);
            this.Name = "MessageBar";
            this.Size = new System.Drawing.Size(496, 24);
            this.Click += new System.EventHandler(this.MessageBarClick);
            this.ResumeLayout(false);
            this.PerformLayout();
        }
        
        #endregion
        
        #region Methods And Event Handlers
        
        private void InitializeGraphics()
        {
            label.Image = PluginBase.MainForm.FindImage16("196"); // warning
            buttonClose.ImageEnabled = ResourceHelper.LoadBitmap("MessageBarClose.bmp");
        }

        public void Update(String message)
        {
            currentMessage = message;
            Int32 p = message.IndexOf('\r');
            if (p < 0) p = message.IndexOf('\n');
            if (p >= 0) message = message.Substring(0, p) + " ...";
            label.Text = "      " + message;
            Color fore = PluginBase.MainForm.GetThemeColor("MessageBar.ForeColor");
            Color back = PluginBase.MainForm.GetThemeColor("MessageBar.BackColor");
            label.ForeColor = fore == Color.Empty ? System.Drawing.SystemColors.InfoText : fore;
            this.ForeColor = fore == Color.Empty ? System.Drawing.SystemColors.InfoText : fore;
            this.BackColor = back == Color.Empty ? System.Drawing.SystemColors.Info : back;
        }
        
        public void ButtonCloseClick(Object sender, System.EventArgs e)
        {
            MessageBarClick(null, null);
        }
        
        public void MessageBarClick(Object sender, System.EventArgs e)
        {
            currentMessage = "";
            Hide();
        }
        
        public void LabelMouseEnter(Object sender, System.EventArgs e)
        {
            if (tip == null)
            {
                tip = new ToolTip();
                tip.ShowAlways = true;
                tip.AutoPopDelay = 10000;
            }
            tip.SetToolTip(label, currentMessage);
        }
        
        public void LabelMouseLeave(Object sender, System.EventArgs e)
        {
            if (tip != null) tip.SetToolTip(label, "");
        }
        
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = CreateGraphics();
            g.DrawLine(new Pen(SystemColors.ControlDark, 1), 0, Height - 1, Width, Height - 1);
        }
        
        #endregion
        
    }
    
    
}
