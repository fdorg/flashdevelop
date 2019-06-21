using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.ComponentModel;
using PluginCore.DockPanelSuite;
using PluginCore.Helpers;

namespace WeifenLuo.WinFormsUI.Docking
{
    internal class VS2005DockPaneCaption : DockPaneCaptionBase
    {
        private sealed class InertButton : InertButtonBase
        {
            private readonly Bitmap m_image;
            private readonly Bitmap m_imageAutoHide;

            public InertButton(VS2005DockPaneCaption dockPaneCaption, Bitmap image, Bitmap imageAutoHide) : base()
            {
                DockPaneCaption = dockPaneCaption;
                m_image = image;
                m_imageAutoHide = imageAutoHide;
                RefreshChanges();
            }

            private VS2005DockPaneCaption DockPaneCaption { get; }

            public bool IsAutoHide => DockPaneCaption.DockPane.IsAutoHide;

            public override Bitmap Image => IsAutoHide ? m_imageAutoHide : m_image;

            protected override void OnRefreshChanges()
            {
                if (DockPaneCaption.ImageColor != ForeColor)
                {
                    ForeColor = DockPaneCaption.ImageColor;
                    Invalidate();
                }
            }
        }

        private readonly ToolTip m_toolTip;

        public VS2005DockPaneCaption(DockPane pane) : base(pane)
        {
            SuspendLayout();

            Font = PluginCore.PluginBase.Settings.DefaultFont;
            Components = new Container();
            m_toolTip = new ToolTip(Components);

            // Adjust size based on scale
            double scale = ScaleHelper.GetScale();
            if (scale >= 2) // 200%
            {
                TextGapTop = 3;
                TextGapBottom = 6;
                ButtonGapBottom = 4;
            }
            else if (scale >= 1.5) // 150%
            {
                TextGapTop = 2;
                TextGapBottom = 4;
                ButtonGapBottom = 4;
            }
            else if (scale >= 1.2) // 120%
            {
                TextGapTop = 2;
                TextGapBottom = 2;
            }
            // Else 100%

            ResumeLayout();
        }

        private static Bitmap _imageButtonClose;
        private static Bitmap ImageButtonClose
        {
            get
            {
                if (_imageButtonClose == null)
                    _imageButtonClose = ScaleHelper.Scale(Resources.DockPane_Close);

                return _imageButtonClose;
            }
        }

        private InertButton m_buttonClose;
        private InertButton ButtonClose
        {
            get
            {
                if (m_buttonClose == null)
                {
                    m_buttonClose = new InertButton(this, ImageButtonClose, ImageButtonClose);
                    m_toolTip.SetToolTip(m_buttonClose, ToolTipClose);
                    m_buttonClose.Click += Close_Click;
                    Controls.Add(m_buttonClose);
                }

                return m_buttonClose;
            }
        }

        private static Bitmap _imageButtonAutoHide;
        private static Bitmap ImageButtonAutoHide
        {
            get
            {
                if (_imageButtonAutoHide == null)
                    _imageButtonAutoHide = ScaleHelper.Scale(Resources.DockPane_AutoHide);

                return _imageButtonAutoHide;
            }
        }

        private static Bitmap _imageButtonDock;
        private static Bitmap ImageButtonDock
        {
            get
            {
                if (_imageButtonDock == null)
                    _imageButtonDock = ScaleHelper.Scale(Resources.DockPane_Dock);

                return _imageButtonDock;
            }
        }

        private InertButton m_buttonAutoHide;
        private InertButton ButtonAutoHide
        {
            get
            {
                if (m_buttonAutoHide == null)
                {
                    m_buttonAutoHide = new InertButton(this, ImageButtonDock, ImageButtonAutoHide);
                    m_toolTip.SetToolTip(m_buttonAutoHide, ToolTipAutoHide);
                    m_buttonAutoHide.Click += AutoHide_Click;
                    Controls.Add(m_buttonAutoHide);
                }

                return m_buttonAutoHide;
            }
        }

        private static Bitmap _imageButtonOptions;
        private static Bitmap ImageButtonOptions
        {
            get
            {
                if (_imageButtonOptions == null)
                    _imageButtonOptions = Resources.DockPane_Option;

                return _imageButtonOptions;
            }
        }

        private InertButton m_buttonOptions;
        private InertButton ButtonOptions
        {
            get
            {
                if (m_buttonOptions == null)
                {
                    m_buttonOptions = new InertButton(this, ImageButtonOptions, ImageButtonOptions);
                    m_toolTip.SetToolTip(m_buttonOptions, ToolTipOptions);
                    m_buttonOptions.Click += Options_Click;
                    Controls.Add(m_buttonOptions);
                }
                return m_buttonOptions;
            }
        }

        private IContainer Components { get; }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                Components.Dispose();
            base.Dispose(disposing);
        }

        static int TextGapTop { get; set; } = 1;

        static int TextGapBottom { get; set; } = 1;

        private static int TextGapLeft { get; } = 3;

        private static int TextGapRight { get; } = 3;

        private static int ButtonGapTop { get; } = 3;

        static int ButtonGapBottom { get; set; } = 3;

        private static int ButtonGapLeft { get; } = 3;

        private static int ButtonGapRight { get; } = 3;

        private static int ButtonGapBetween { get; } = 1;

        private static string _toolTipClose;
        private static string ToolTipClose
        {
            get
            {   
                if (_toolTipClose == null)
                    // HACK: _toolTipClose = Strings.DockPaneCaption_ToolTipClose;
                    _toolTipClose = PluginCore.Localization.TextHelper.GetString("PluginCore.Docking.ToolTipClose");
                return _toolTipClose;
            }
        }

        private static string _toolTipOptions;
        private static string ToolTipOptions
        {
            get
            {
                if (_toolTipOptions == null)
                    // HACK: _toolTipOptions = Strings.DockPaneCaption_ToolTipOptions;
                    _toolTipOptions = PluginCore.Localization.TextHelper.GetString("PluginCore.Docking.ToolTipOptions");
                return _toolTipOptions;
            }
        }

        private static string _toolTipAutoHide;
        private static string ToolTipAutoHide
        {
            get
            {   
                if (_toolTipAutoHide == null)
                    // HACK: _toolTipAutoHide = Strings.DockPaneCaption_ToolTipAutoHide;
                    _toolTipAutoHide = PluginCore.Localization.TextHelper.GetString("PluginCore.Docking.ToolTipAutoHide");
                return _toolTipAutoHide;
            }
        }

        private static Blend _activeBackColorGradientBlend;
        private static Blend ActiveBackColorGradientBlend
        {
            get
            {
                if (_activeBackColorGradientBlend == null)
                {
                    Blend blend = new Blend(2);

                    blend.Factors = new float[]{0.5F, 1.0F};
                    blend.Positions = new float[]{0.0F, 1.0F};
                    _activeBackColorGradientBlend = blend;
                }

                return _activeBackColorGradientBlend;
            }
        }

        private static Color ActiveBackColorGradientBegin
        {
            get 
            {
                Color color = PluginCore.PluginBase.MainForm.GetThemeColor("VS2005DockPaneCaption.TabGradientStart");
                return color == Color.Empty ? SystemColors.GradientActiveCaption : color;
            }
        }

        private static Color ActiveBackColorGradientEnd
        {
            get 
            {
                Color color = PluginCore.PluginBase.MainForm.GetThemeColor("VS2005DockPaneCaption.TabGradientEnd");
                return color == Color.Empty ? SystemColors.ActiveCaption : color;
            }
        }

        private static Color InactiveBackColor
        {
            get
            {
                Color color = PluginCore.PluginBase.MainForm.GetThemeColor("VS2005DockPaneCaption.BackColor");
                if (color != Color.Empty) return color;
                else return Color.FromArgb(204, 199, 186);
            }
        }

        private static Color ActiveTextColor => SystemColors.ActiveCaptionText;

        private static Color InactiveTextColor => SystemColors.ControlText;

        private Color TextColor
        {
            get
            {
                Color color = Color.Empty;
                if (DockPane.IsActivated) color = PluginCore.PluginBase.MainForm.GetThemeColor("VS2005DockPaneCaption.ActiveForeColor");
                if (!DockPane.IsActivated || color == Color.Empty)
                {
                    color = PluginCore.PluginBase.MainForm.GetThemeColor("VS2005DockPaneCaption.ForeColor");
                }
                if (color != Color.Empty) return color;
                else return DockPane.IsActivated ? ActiveTextColor : InactiveTextColor;
            }
        }

        private Color ImageColor
        {
            get
            {
                Color color;
                if (DockPane.IsActivated) color = PluginCore.PluginBase.MainForm.GetThemeColor("VS2005DockPaneCaption.ActiveImageColor");
                else color = PluginCore.PluginBase.MainForm.GetThemeColor("VS2005DockPaneCaption.ImageColor");
                if (color != Color.Empty) return color;
                else return TextColor;
            }
        }

        private static readonly TextFormatFlags _textFormat =
            TextFormatFlags.SingleLine |
            TextFormatFlags.EndEllipsis |
            TextFormatFlags.VerticalCenter;
        private TextFormatFlags TextFormat
        {
            get
            {
                if (RightToLeft == RightToLeft.No)
                    return _textFormat;
                else
                    return _textFormat | TextFormatFlags.RightToLeft | TextFormatFlags.Right;
            }
        }

        protected internal override int MeasureHeight()
        {
            int height = Font.Height + TextGapTop + TextGapBottom;

            if (height < ButtonClose.Image.Height + ButtonGapTop + ButtonGapBottom)
                height = ButtonClose.Image.Height + ButtonGapTop + ButtonGapBottom;

            return height;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint (e);
            DrawCaption(e.Graphics);
        }

        private void DrawCaption(Graphics g)
        {
            if (ClientRectangle.Width == 0 || ClientRectangle.Height == 0)
                return;

            Pen borderPen = SystemPens.ControlDark;

            Color color = PluginCore.PluginBase.MainForm.GetThemeColor("VS2005DockPaneCaption.BorderColor");
            if (color != Color.Empty) borderPen = new Pen(color);

            if (DockPane.IsActivated)
            {
                // HACK: Looks more like VS2005..
                LinearGradientBrush brush = new LinearGradientBrush(ClientRectangle, ActiveBackColorGradientBegin, ActiveBackColorGradientEnd, LinearGradientMode.Vertical);
                brush.Blend = ActiveBackColorGradientBlend;
                g.FillRectangle(brush, ClientRectangle);
                Rectangle fixedRect = ClientRectangle;
                fixedRect.Height -= 1;
                fixedRect.Width -= 1;
                g.DrawRectangle(borderPen, fixedRect);
            }
            else
            {
                // HACK: Looks more like VS2005..
                SolidBrush fillBrush = new SolidBrush(InactiveBackColor);
                g.FillRectangle(fillBrush, ClientRectangle);
                Rectangle fixedRect = ClientRectangle;
                fixedRect.Height -= 1;
                fixedRect.Width -= 1;
                g.DrawRectangle(borderPen, fixedRect);
            }

            Rectangle rectCaption = ClientRectangle;

            Rectangle rectCaptionText = rectCaption;
            rectCaptionText.X += TextGapLeft;
            rectCaptionText.Width -= TextGapLeft + TextGapRight;
            rectCaptionText.Width -= ButtonGapLeft + ButtonClose.Width + ButtonGapRight;
            if (ShouldShowAutoHideButton)
                rectCaptionText.Width -= ButtonAutoHide.Width + ButtonGapBetween;
            if (HasTabPageContextMenu)
                rectCaptionText.Width -= ButtonOptions.Width + ButtonGapBetween;
            rectCaptionText.Y += TextGapTop;
            rectCaptionText.Height -= TextGapTop + TextGapBottom;
            TextRenderer.DrawText(g, DockPane.CaptionText, Font, DrawHelper.RtlTransform(this, rectCaptionText), TextColor, TextFormat);
        }

        protected override void OnLayout(LayoutEventArgs levent)
        {
            SetButtonsPosition();
            base.OnLayout (levent);
        }

        protected override void OnRefreshChanges()
        {
            SetButtons();
            Invalidate();
        }

        private bool CloseButtonEnabled => DockPane.ActiveContent?.DockHandler.CloseButton ?? false;

        private bool ShouldShowAutoHideButton => !DockPane.IsFloat;

        private void SetButtons()
        {
            ButtonClose.Enabled = CloseButtonEnabled;
            ButtonAutoHide.Visible = ShouldShowAutoHideButton;
            ButtonOptions.Visible = HasTabPageContextMenu;
            ButtonClose.RefreshChanges();
            ButtonAutoHide.RefreshChanges();
            ButtonOptions.RefreshChanges();
            
            SetButtonsPosition();
        }

        private void SetButtonsPosition()
        {
            // set the size and location for close and auto-hide buttons
            Rectangle rectCaption = ClientRectangle;
            int buttonWidth = ButtonClose.Image.Width;
            int buttonHeight = ButtonClose.Image.Height;
            int height = rectCaption.Height - ButtonGapTop - ButtonGapBottom;
            if (buttonHeight < height)
            {
                buttonWidth = buttonWidth * (height / buttonHeight);
                buttonHeight = height;
            }
            Size buttonSize = new Size(buttonWidth, buttonHeight);
            int x = rectCaption.X + rectCaption.Width - 1 - ButtonGapRight - m_buttonClose.Width;
            int y = rectCaption.Y + ButtonGapTop;
            Point point = new Point(x, y);
            ButtonClose.Bounds = DrawHelper.RtlTransform(this, new Rectangle(point, buttonSize));
            point.Offset(-(buttonWidth + ButtonGapBetween), 0);
            ButtonAutoHide.Bounds = DrawHelper.RtlTransform(this, new Rectangle(point, buttonSize));
            if (ShouldShowAutoHideButton)
                point.Offset(-(buttonWidth + ButtonGapBetween), 0);
            ButtonOptions.Bounds = DrawHelper.RtlTransform(this, new Rectangle(point, buttonSize));
        }

        private void Close_Click(object sender, EventArgs e)
        {
            DockPane.CloseActiveContent();
        }

        private void AutoHide_Click(object sender, EventArgs e)
        {
            if (!DockPane.IsAutoHide)
                DockPane.ActiveContent.DockHandler.GiveUpFocus();
            DockPane.DockState = DockHelper.ToggleAutoHideState(DockPane.DockState);
        }

        private void Options_Click(object sender, EventArgs e)
        {
            ShowTabPageContextMenu(PointToClient(MousePosition));
        }

        protected override void OnRightToLeftChanged(EventArgs e)
        {
            base.OnRightToLeftChanged(e);
            PerformLayout();
        }
    }
}
