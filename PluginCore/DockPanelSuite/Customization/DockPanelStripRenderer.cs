using System;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using PluginCore.Managers;
using PluginCore;
using PluginCore.Helpers;

namespace System.Windows.Forms
{
    #region DockDrawHelper

    public class DockDrawHelper
    {
        public static readonly Color ColorSubmenuBG = Color.FromArgb(255, 240, 240, 240);
        public static readonly Color ColorSelectedBG_Blue = Color.FromArgb(255, 186, 228, 246);
        public static readonly Color ColorSelectedBG_Header_Blue = Color.FromArgb(255, 146, 202, 230);
        public static readonly Color ColorSelectedBG_White = Color.FromArgb(255, 241, 248, 251);
        public static readonly Color ColorSelectedBG_Border = Color.FromArgb(255, 125, 162, 206);
        public static readonly Color ColorCheckBG = Color.FromArgb(255, 206, 237, 250);

        /// <summary>
        /// 
        /// </summary>
        public static void DrawRoundedRectangle(Graphics graphics, int xAxis, int yAxis, int width, int height, int diameter, Color color)
        {
            Pen pen = new Pen(color);
            var BaseRect = new RectangleF(xAxis, yAxis, width, height);
            var ArcRect = new RectangleF(BaseRect.Location, new SizeF(diameter, diameter));
            graphics.DrawArc(pen, ArcRect, 180, 90);
            graphics.DrawLine(pen, xAxis + (int)(diameter / 2), yAxis, xAxis + width - (int)(diameter / 2), yAxis);
            ArcRect.X = BaseRect.Right - diameter;
            graphics.DrawArc(pen, ArcRect, 270, 90);
            graphics.DrawLine(pen, xAxis + width, yAxis + (int)(diameter / 2), xAxis + width, yAxis + height - (int)(diameter / 2));
            ArcRect.Y = BaseRect.Bottom - diameter;
            graphics.DrawArc(pen, ArcRect, 0, 90);
            graphics.DrawLine(pen, xAxis + (int)(diameter / 2), yAxis + height, xAxis + width - (int)(diameter / 2), yAxis + height);
            ArcRect.X = BaseRect.Left;
            graphics.DrawArc(pen, ArcRect, 90, 90);
            graphics.DrawLine(pen, xAxis, yAxis + (int)(diameter / 2), xAxis, yAxis + height - (int)(diameter / 2));
        }

    } 

    #endregion

    public class DockPanelStripRenderer : ToolStripRenderer
    {
        private Boolean drawBottomBorder;
        private ProfessionalColorTable colorTable;
        static ToolStripRenderer renderer;

        public DockPanelStripRenderer() : this(true) { }

        public DockPanelStripRenderer(Boolean drawBottomBorder)
        {
            this.drawBottomBorder = drawBottomBorder;
            this.colorTable = new ProfessionalColorTable();
            UiRenderMode renderMode = PluginBase.MainForm.Settings.RenderMode;
            if (renderMode == UiRenderMode.System) renderer = new ToolStripSystemRenderer();
            else renderer = new ToolStripProfessionalRenderer();
        }

        protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
        {
            if (e.ToolStrip is StatusStrip) return;
            ToolStripRenderEventArgs ea = new ToolStripRenderEventArgs(e.Graphics, e.ToolStrip, new Rectangle(-10, -3, e.AffectedBounds.Width + 20, e.AffectedBounds.Height + 6), e.BackColor);
            renderer.DrawToolStripBackground(ea);
        }

        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
        {
            if (e.ToolStrip is StatusStrip)
            {
                Color back = PluginBase.MainForm.GetThemeColor("ToolStrip.3dDarkColor");
                e.Graphics.DrawLine(back == Color.Empty ? SystemPens.ControlDark : new Pen(back), 0, 0, e.ToolStrip.Width, 0);
                Color fore = PluginBase.MainForm.GetThemeColor("ToolStrip.3dLightColor");
                e.Graphics.DrawLine(fore == Color.Empty ? SystemPens.ButtonHighlight : new Pen(fore), 1, 1, e.ToolStrip.Width, 1);
            }
            else if (e.ToolStrip is ToolStripDropDownMenu)
            {
                renderer.DrawToolStripBorder(e);
                if (renderer is ToolStripProfessionalRenderer && e.ConnectedArea.Width > 0)
                {
                    e.Graphics.DrawLine(SystemPens.ControlLight, e.ConnectedArea.Left, e.ConnectedArea.Top, e.ConnectedArea.Right - 1, e.ConnectedArea.Top);
                }
            }
            else if (this.drawBottomBorder)
            {
                Rectangle r = e.AffectedBounds;
                e.Graphics.DrawLine(SystemPens.ControlDark, r.Left, r.Bottom - 1, r.Right, r.Bottom - 1);
            }
        }

        protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e)
        {
            if (renderer is ToolStripSystemRenderer)
            {
                if (e.ToolStrip is ToolStripDropDownMenu)
                {
                    renderer.DrawSeparator(e);
                    Pen pen = new Pen(SystemColors.ControlDark);
                    e.Graphics.DrawLine(pen, e.Item.ContentRectangle.Left, e.Item.ContentRectangle.Top, e.Item.ContentRectangle.Right, e.Item.ContentRectangle.Top);
                    pen.Dispose();
                }
                else
                {
                    Pen pen = new Pen(SystemColors.ControlDark);
                    Int32 middle = e.Item.ContentRectangle.Left + e.Item.ContentRectangle.Width / 2;
                    e.Graphics.DrawLine(pen, middle, e.Item.ContentRectangle.Top + 1, middle, e.Item.ContentRectangle.Bottom - 2);
                    pen.Dispose();
                }
            }
            else if (e.Item is ToolStripSeparator && e.Vertical)
            {
                Color light = PluginBase.MainForm.GetThemeColor("ToolStrip.3dLightColor");
                Color dark = PluginBase.MainForm.GetThemeColor("ToolStrip.3dDarkColor");
                if (dark != Color.Empty && light != Color.Empty)
                {
                    Pen pen = new Pen(dark);
                    Int32 middle = e.Item.ContentRectangle.Left + e.Item.ContentRectangle.Width / 2;
                    e.Graphics.DrawLine(pen, middle - 1, e.Item.ContentRectangle.Top + 2, middle - 1, e.Item.ContentRectangle.Bottom - 4);
                    pen.Dispose();
                    Pen pen2 = new Pen(light);
                    e.Graphics.DrawLine(pen2, middle, e.Item.ContentRectangle.Top + 2, middle, e.Item.ContentRectangle.Bottom - 4);
                    pen2.Dispose();
                } 
                else renderer.DrawSeparator(e);
            }
            else renderer.DrawSeparator(e);
        }

        protected override void OnRenderGrip(ToolStripGripRenderEventArgs e)
        {
            if (renderer is ToolStripProfessionalRenderer)
            {
                if (e.GripStyle == ToolStripGripStyle.Hidden) return;
                Color fore = PluginBase.MainForm.GetThemeColor("ToolStrip.3dLightColor");
                using (Brush lightBrush = new SolidBrush(fore == Color.Empty ? this.colorTable.GripLight : fore))
                {
                    Rectangle r = new Rectangle(e.GripBounds.Left, e.GripBounds.Top + 6, 2, 2);
                    for (Int32 i = 0; i < e.GripBounds.Height - 11; i += 4)
                    {
                        e.Graphics.FillRectangle(lightBrush, r);
                        r.Offset(0, 4);
                    }
                }
                Color back = PluginBase.MainForm.GetThemeColor("ToolStrip.3dDarkColor");
                using (Brush darkBrush = new SolidBrush(back == Color.Empty ? this.colorTable.GripDark: back))
                {
                    Rectangle r = new Rectangle(e.GripBounds.Left - 1, e.GripBounds.Top + 5, 2, 2);
                    for (Int32 i = 0; i < e.GripBounds.Height - 11; i += 4)
                    {
                        e.Graphics.FillRectangle(darkBrush, r);
                        r.Offset(0, 4);
                    }
                }
            }
            else renderer.DrawGrip(e);
        }

        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            if (renderer is ToolStripProfessionalRenderer)
            {
                Color back = PluginBase.MainForm.GetThemeColor("ToolStripItem.BackColor");
                Color border = PluginBase.MainForm.GetThemeColor("ToolStripItem.BorderColor");
                if (e.Item.Enabled)
                {
                    if (!e.Item.IsOnDropDown && e.Item.Selected)
                    {
                        Rectangle rect = new Rectangle(0, 0, e.Item.Width, e.Item.Height);
                        Rectangle rect2 = new Rectangle(1, 1, e.Item.Width - 2, e.Item.Height - 2);
                        LinearGradientBrush b = new LinearGradientBrush(rect, back == Color.Empty ? DockDrawHelper.ColorSelectedBG_White : back, back == Color.Empty ? DockDrawHelper.ColorSelectedBG_Blue : back, LinearGradientMode.Vertical);
                        e.Graphics.FillRectangle(b, rect);
                        Rectangle rect3 = new Rectangle(rect2.Left - 1, rect2.Top - 1, rect2.Width + 1, rect2.Height + 1);
                        Rectangle rect4 = new Rectangle(rect3.Left + 1, rect3.Top + 1, rect3.Width - 2, rect3.Height - 2);
                        e.Graphics.DrawRectangle(new Pen(border == Color.Empty ? DockDrawHelper.ColorSelectedBG_Border : border), rect3);
                        e.Graphics.DrawRectangle(new Pen(back == Color.Empty ? DockDrawHelper.ColorSelectedBG_White : back), rect4);
                    }
                    else if (e.Item.IsOnDropDown && e.Item.Selected)
                    {
                        Rectangle rect = new Rectangle(3, 1, e.Item.Width - 4, e.Item.Height - 2);
                        Rectangle rect2 = new Rectangle(4, 2, e.Item.Width - 6, e.Item.Height - 4);
                        LinearGradientBrush b = new LinearGradientBrush(rect, back == Color.Empty ? DockDrawHelper.ColorSelectedBG_White : back, back == Color.Empty ? DockDrawHelper.ColorSelectedBG_Blue : back, LinearGradientMode.Vertical);
                        SolidBrush b2 = new SolidBrush(border == Color.Empty ? DockDrawHelper.ColorSelectedBG_Border : border);
                        e.Graphics.FillRectangle(b, rect);
                        DockDrawHelper.DrawRoundedRectangle(e.Graphics, rect.Left - 1, rect.Top - 1, rect.Width, rect.Height + 1, 3, border == Color.Empty ? DockDrawHelper.ColorSelectedBG_Border : border);
                        DockDrawHelper.DrawRoundedRectangle(e.Graphics, rect2.Left - 1, rect2.Top - 1, rect2.Width, rect2.Height + 1, 3, back == Color.Empty ? DockDrawHelper.ColorSelectedBG_White : back);
                        e.Item.ForeColor = Color.Black;
                    }
                    if (((ToolStripMenuItem)e.Item).DropDown.Visible && !e.Item.IsOnDropDown)
                    {
                        renderer.DrawMenuItemBackground(e);
                    }
                }
            }
            else renderer.DrawMenuItemBackground(e);
        }

        protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e)
        {
            if (renderer is ToolStripProfessionalRenderer)
            {
                Boolean isOver = false;
                Color back = PluginBase.MainForm.GetThemeColor("ToolStripItem.BackColor");
                Color border = PluginBase.MainForm.GetThemeColor("ToolStripItem.BorderColor");
                if (e.Item is ToolStripButton)
                {
                    ToolStripButton button = e.Item as ToolStripButton;
                    Rectangle bBounds = button.Owner.RectangleToScreen(button.Bounds);
                    isOver = bBounds.Contains(Control.MousePosition);
                }
                if (e.Item.Selected || ((ToolStripButton)e.Item).Checked || (isOver && e.Item.Enabled))
                {
                    Rectangle rect = new Rectangle(0, 0, e.Item.Width, e.Item.Height);
                    Rectangle rect2 = new Rectangle(1, 1, e.Item.Width - 2, e.Item.Height - 2);
                    LinearGradientBrush b = new LinearGradientBrush(rect, back == Color.Empty ? DockDrawHelper.ColorSelectedBG_White : back, back == Color.Empty ? DockDrawHelper.ColorSelectedBG_Blue : back, LinearGradientMode.Vertical);
                    e.Graphics.FillRectangle(b, rect);
                    Rectangle rect3 = new Rectangle(rect2.Left - 1, rect2.Top - 1, rect2.Width + 1, rect2.Height + 1);
                    Rectangle rect4 = new Rectangle(rect3.Left + 1, rect3.Top + 1, rect3.Width - 2, rect3.Height - 2);
                    e.Graphics.DrawRectangle(new Pen(border == Color.Empty ? DockDrawHelper.ColorSelectedBG_Border : border), rect3);
                    e.Graphics.DrawRectangle(new Pen(back == Color.Empty ? DockDrawHelper.ColorSelectedBG_White : back), rect4);
                }
                if (e.Item.Pressed)
                {
                    Rectangle rect = new Rectangle(1, 1, e.Item.Width - 2, e.Item.Height - 2);
                    LinearGradientBrush b = new LinearGradientBrush(rect, back == Color.Empty ? DockDrawHelper.ColorSelectedBG_White : back, back == Color.Empty ? DockDrawHelper.ColorSelectedBG_Blue : back, LinearGradientMode.Vertical);
                    e.Graphics.FillRectangle(b, rect);
                    Rectangle rect2 = new Rectangle(rect.Left - 1, rect.Top - 1, rect.Width + 1, rect.Height + 1);
                    e.Graphics.DrawRectangle(new Pen(border == Color.Empty ? DockDrawHelper.ColorSelectedBG_Border : border), rect2);
                }
            }
            else renderer.DrawButtonBackground(e);
        }

        protected override void OnRenderDropDownButtonBackground(ToolStripItemRenderEventArgs e)
        {
            if (renderer is ToolStripProfessionalRenderer)
            {
                Color back = PluginBase.MainForm.GetThemeColor("ToolStripItem.BackColor");
                Color border = PluginBase.MainForm.GetThemeColor("ToolStripItem.BorderColor");
                if (e.Item.Selected)
                {
                    Rectangle rectBorder = new Rectangle(0, 0, e.Item.Width, e.Item.Height);
                    Rectangle rectBack = new Rectangle(1, 1, e.Item.Width - 2, e.Item.Height - 2);
                    LinearGradientBrush backBrush = new LinearGradientBrush(rectBack, back == Color.Empty ? DockDrawHelper.ColorSelectedBG_White : back, back == Color.Empty ? DockDrawHelper.ColorSelectedBG_Blue : back, LinearGradientMode.Vertical);
                    e.Graphics.FillRectangle(backBrush, rectBack);

                    Rectangle rect2 = new Rectangle(rectBack.Left - 1, rectBack.Top - 1, rectBack.Width + 1, rectBack.Height + 1);
                    Rectangle rect3 = new Rectangle(rect2.Left + 1, rect2.Top + 1, rect2.Width - 2, rect2.Height - 2);
                    e.Graphics.DrawRectangle(new Pen(border == Color.Empty ? DockDrawHelper.ColorSelectedBG_Border : border), rect2);
                    e.Graphics.DrawRectangle(new Pen(back == Color.Empty ? DockDrawHelper.ColorSelectedBG_White : back), rect3);
                }
                if (e.Item.Pressed) renderer.DrawDropDownButtonBackground(e);
            }
            else renderer.DrawDropDownButtonBackground(e);
        }

        protected override void OnRenderImageMargin(ToolStripRenderEventArgs e)
        {
            if (renderer is ToolStripProfessionalRenderer)
            {
                SolidBrush line = new SolidBrush(SystemColors.ControlLight);
                Rectangle rect = new Rectangle(e.AffectedBounds.Width, 0, 1, e.AffectedBounds.Height);
                Rectangle rect2 = new Rectangle(0, 0, e.AffectedBounds.Width, e.AffectedBounds.Height);
                LinearGradientBrush back = new LinearGradientBrush(rect2, this.colorTable.ImageMarginGradientBegin, this.colorTable.ImageMarginGradientEnd, 0.2f);
                e.Graphics.FillRectangle(back, rect2);
                e.Graphics.FillRectangle(line, rect);
            }
            else renderer.DrawImageMargin(e);
        }

        protected override void OnRenderItemCheck(ToolStripItemImageRenderEventArgs e)
        {
            if (renderer is ToolStripProfessionalRenderer)
            {
                Color back = PluginBase.MainForm.GetThemeColor("ToolStripItem.BackColor");
                Color border = PluginBase.MainForm.GetThemeColor("ToolStripItem.BorderColor");
                Rectangle borderRect = new Rectangle(4, 2, ScaleHelper.Scale(18), ScaleHelper.Scale(18));
                Rectangle backRect = new Rectangle(5, 3, borderRect.Width - 2, borderRect.Height - 2);
                SolidBrush borderBrush = new SolidBrush(border == Color.Empty ? DockDrawHelper.ColorSelectedBG_Border : border);
                LinearGradientBrush backBrush = new LinearGradientBrush(backRect, back == Color.Empty ? DockDrawHelper.ColorSelectedBG_White : back, back == Color.Empty ? DockDrawHelper.ColorSelectedBG_Blue : back, LinearGradientMode.Vertical);
                e.Graphics.FillRectangle(borderBrush, borderRect);
                e.Graphics.FillRectangle(backBrush, backRect);
                e.Graphics.DrawImage(e.Image, 5 + ((backRect.Width - e.ImageRectangle.Width) / 2), 3 + ((backRect.Height - e.ImageRectangle.Height) / 2), e.ImageRectangle.Width, e.ImageRectangle.Height);
            }
            else renderer.DrawItemCheck(e);
        }

        protected override void OnRenderStatusStripSizingGrip(ToolStripRenderEventArgs e)
        {
            Color dark = PluginBase.MainForm.GetThemeColor("ToolStrip.3dDarkColor");
            Color light = PluginBase.MainForm.GetThemeColor("ToolStrip.3dLightColor");
            if (dark != Color.Empty && light != Color.Empty)
            {
                using (SolidBrush darkBrush = new SolidBrush(dark), lightBrush = new SolidBrush(light))
                {
                    // Do we need to invert the drawing edge?
                    bool rtl = (e.ToolStrip.RightToLeft == RightToLeft.Yes);

                    // Find vertical position of the lowest grip line
                    int y = e.AffectedBounds.Bottom - 3 * 2 + 1;

                    // Draw three lines of grips
                    for (int i = 3; i >= 1; i--)
                    {
                        // Find the rightmost grip position on the line
                        int x = (rtl ? e.AffectedBounds.Left + 1 : e.AffectedBounds.Right - 3 * 2 + 1);

                        // Draw grips from right to left on line
                        for (int j = 0; j < i; j++)
                        {
                            // Just the single grip glyph
                            DrawGripGlyph(e.Graphics, x, y, darkBrush, lightBrush);

                            // Move left to next grip position
                            x -= (rtl ? -4 : 4);
                        }
                        // Move upwards to next grip line
                        y -= 4;
                    }
                }
            }
            else renderer.DrawStatusStripSizingGrip(e);
        }

        private void DrawGripGlyph(Graphics g, int x, int y, Brush darkBrush, Brush lightBrush)
        {
            g.FillRectangle(lightBrush, x + 1, y + 1, 2, 2);
            g.FillRectangle(darkBrush, x, y, 2, 2);
        }

        #region Reuse Some Renderer Stuff

        protected override void OnRenderItemBackground(ToolStripItemRenderEventArgs e)
        {
            renderer.DrawItemBackground(e);
        }

        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            renderer.DrawItemText(e);
        }

        protected override void OnRenderItemImage(ToolStripItemImageRenderEventArgs e)
        {
            renderer.DrawItemImage(e);
        }

        protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e)
        {
            renderer.DrawArrow(e);
        }

        protected override void OnRenderLabelBackground(ToolStripItemRenderEventArgs e)
        {
            renderer.DrawLabelBackground(e);
        }

        protected override void OnRenderOverflowButtonBackground(ToolStripItemRenderEventArgs e)
        {
            renderer.DrawOverflowButtonBackground(e);
        }

        protected override void OnRenderSplitButtonBackground(ToolStripItemRenderEventArgs e)
        {
            renderer.DrawSplitButton(e);
        }

        protected override void OnRenderToolStripContentPanelBackground(ToolStripContentPanelRenderEventArgs e)
        {
            renderer.DrawToolStripContentPanelBackground(e);
        }

        protected override void OnRenderToolStripPanelBackground(ToolStripPanelRenderEventArgs e)
        {
            renderer.DrawToolStripPanelBackground(e);
        }

        protected override void OnRenderToolStripStatusLabelBackground(ToolStripItemRenderEventArgs e)
        {
            renderer.DrawToolStripStatusLabelBackground(e);
        }

        #endregion

    }

}
