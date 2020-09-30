using System;
using System.Drawing;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI;
using PluginCore.Helpers;

namespace PluginCore.Controls
{
    public class MessageBar : UserControl
    {
        public static bool Locked;

        const string WarningIcon = "196";
        const string QuestionIcon = "222";

        ToolTip tip;
        Label label;
        InertButton buttonClose;
        InertButton[] optionButtons;
        string currentMessage;

        public MessageBar() : this(WarningIcon) //warning
        {
        }

        public MessageBar(string icon)
        {
            InitializeComponent();
            InitializeGraphics(icon);
        }

        public static void ShowQuestion(string message, string[] options, Action<string> onChoose)
        {
            var bar = CreateBar(PluginBase.MainForm.CurrentDocument, message, QuestionIcon);
            bar.Visible = false;
            //remove old option buttons
            if (bar.optionButtons != null)
            {
                foreach (var btn in bar.optionButtons)
                    bar.Controls.Remove(btn);
            }
            //add new option buttons
            bar.optionButtons = new InertButton[options.Length];
            var x = bar.buttonClose.Location.X;
            for (var i = options.Length-1; i >= 0; --i)
            {
                var option = options[i];

                var btn = CreateOptionButton(option);

                var width = TextRenderer.MeasureText(option, btn.Font).Width + 4;
                btn.Size = new Size(width, 16);
                x -= width + 5;
                btn.Location = new Point(x, 4);
                btn.Click += (sender, args) =>
                {
                    bar.MessageBarClick(null, null);
                    onChoose(((InertButton) sender).Text);
                };

                bar.optionButtons[i] = btn;
                bar.Controls.Add(btn);
                btn.BringToFront();
            }
            bar.Visible = true;
        }
        
        public static void ShowWarning(string message)
        {
            if (Locked) return;
            CreateBar(PluginBase.MainForm.CurrentDocument, message, WarningIcon);
        }

        public static void HideWarning() => HideBar(PluginBase.MainForm.CurrentDocument);

        static MessageBar CreateBar(ITabbedDocument target, string message, string icon)
        {
            foreach (Control ctrl in target.Controls)
            {
                if (ctrl is MessageBar messageBar)
                {
                    messageBar.Update(message, icon);
                    messageBar.Visible = true;
                    return messageBar;
                }
            }
            var bar = new MessageBar(icon) {Visible = false};
            target.Controls.Add(bar);
            bar.Dock = DockStyle.Top;
            bar.Update(message);
            bar.SendToBack();
            bar.Visible = true;
            return bar;
        }

        static InertButton CreateOptionButton(string text)
        {
            return new InertButton
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                BorderWidth = 1,
                ImageDisabled = null,
                ImageIndexDisabled = -1,
                ImageIndexEnabled = -1,
                IsPopup = false,
                Monochrome = true,
                RepeatClick = false,
                RepeatClickDelay = 500,
                RepeatClickInterval = 100,
                Text = text,
                ToolTipText = "",
                UseVisualStyleBackColor = true
            };
        }

        public static void HideBar(ITabbedDocument target)
        {
            foreach (Control ctrl in target.Controls)
            {
                if (ctrl is MessageBar bar)
                {
                    bar.MessageBarClick(null, null);
                    return;
                }
            }
        }
        
        #region InitializeComponent
        
        public void InitializeComponent() 
        {
            buttonClose = new InertButton();
            label = new Label();
            SuspendLayout();
            // 
            // buttonClose
            // 
            buttonClose.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            buttonClose.BorderWidth = 1;
            buttonClose.ImageDisabled = null;
            buttonClose.ImageIndexDisabled = -1;
            buttonClose.ImageIndexEnabled = -1;
            buttonClose.IsPopup = false;
            buttonClose.Location = new Point(477, 5);
            buttonClose.Monochrome = true;
            buttonClose.Name = "buttonClose";
            buttonClose.RepeatClick = true;
            buttonClose.RepeatClickDelay = 500;
            buttonClose.RepeatClickInterval = 100;
            buttonClose.Size = new Size(16, 14);
            buttonClose.TabIndex = 2;
            buttonClose.ToolTipText = "";
            buttonClose.Click += MessageBarClick;
            // 
            // label
            // 
            label.Anchor = (AnchorStyles.Top | AnchorStyles.Left) 
                                | AnchorStyles.Right;
            label.ForeColor = SystemColors.InfoText;
            label.ImageAlign = ContentAlignment.MiddleLeft;
            label.Location = new Point(2, 0);
            label.Name = "label";
            label.Size = new Size(494, 22);
            label.TabIndex = 0;
            label.TextAlign = ContentAlignment.MiddleLeft;
            label.Click += MessageBarClick;
            label.MouseEnter += LabelMouseEnter;
            label.MouseLeave += LabelMouseLeave;
            // 
            // MessageBar
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = SystemColors.Info;
            Controls.Add(buttonClose);
            Controls.Add(label);
            ForeColor = SystemColors.InfoText;
            Name = "MessageBar";
            Size = new Size(496, 24);
            Click += MessageBarClick;
            ResumeLayout(false);

        }
        
        #endregion
        
        #region Methods And Event Handlers

        void InitializeGraphics(string icon)
        {
            label.Image = PluginBase.MainForm.FindImage16(icon);
            buttonClose.ImageEnabled = ResourceHelper.LoadBitmap("MessageBarClose.bmp");
        }

        public void Update(string message, string icon)
        {
            Update(message);
            label.Image = PluginBase.MainForm.FindImage16(icon);
        }

        public void Update(string message)
        {
            currentMessage = message;
            var p = message.IndexOf('\r');
            if (p < 0) p = message.IndexOf('\n');
            if (p >= 0) message = message.Substring(0, p) + " ...";
            label.Text = "      " + message;
            var fore = PluginBase.MainForm.GetThemeColor("MessageBar.ForeColor");
            var back = PluginBase.MainForm.GetThemeColor("MessageBar.BackColor");
            label.ForeColor = fore == Color.Empty ? SystemColors.InfoText : fore;
            ForeColor = fore == Color.Empty ? SystemColors.InfoText : fore;
            BackColor = back == Color.Empty ? SystemColors.Info : back;
        }
        
        public void ButtonCloseClick(object sender, EventArgs e) => MessageBarClick(null, null);

        public void MessageBarClick(object sender, EventArgs e)
        {
            currentMessage = "";
            Hide();
        }
        
        public void LabelMouseEnter(object sender, EventArgs e)
        {
            tip ??= new ToolTip {ShowAlways = true, AutoPopDelay = 10000};
            tip.SetToolTip(label, currentMessage);
        }
        
        public void LabelMouseLeave(object sender, EventArgs e) => tip?.SetToolTip(label, "");

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = CreateGraphics();
            g.DrawLine(new Pen(SystemColors.ControlDark, 1), 0, Height - 1, Width, Height - 1);
        }
        
        #endregion
    }
}