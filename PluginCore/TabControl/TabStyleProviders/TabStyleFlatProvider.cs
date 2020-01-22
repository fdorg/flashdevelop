// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System.Drawing;
using PluginCore.Managers;
using PluginCore;
using System.Drawing.Drawing2D;

// HACK: Added
namespace System.Windows.Forms
{
	[System.ComponentModel.ToolboxItem(false)]
	public class TabStyleFlatProvider : TabStyleProvider, IEventHandler
	{
        Color TabBackColor { get; set; } = SystemColors.Control;
        Color TabBackColorHot { get; set; } = SystemColors.Highlight;
        Color TabBackColorActive { get; set; } = SystemColors.Highlight;
        Color TabBackColorDisabled { get; set; } = SystemColors.Control;
        Color TabActiveSeparator { get; set; } = SystemColors.Window;

        public TabStyleFlatProvider(CustomTabControl tabControl) : base(tabControl)
        {
            this._Radius = 1;
            this._Overlap = 1;
            this.HotTrack = true;
            this.FocusTrack = true;
            this.Padding = new Point(10, 3); //	Must set after the _Radius
            EventManager.AddEventHandler(this, EventType.ApplyTheme);
            this.RefreshColors();
        }

        Color GetThemeColor(string id)
        {
            return PluginBase.MainForm.GetThemeColor(id);
        }

        public void HandleEvent(object sender, NotifyEvent e, HandlingPriority priority)
        {
            if (e.Type == EventType.ApplyTheme) RefreshColors();
        }

        public void RefreshColors()
        {
            this._BorderColor = GetThemeColor("TabControl.BorderColor");
            this._BorderColorHot = GetThemeColor("TabControl.BorderColorHot");
            this._BorderColorSelected = GetThemeColor("TabControl.BorderColorSelected");
            this._CloserColor = GetThemeColor("TabControl.CloserColor");
            this._CloserColorActive = GetThemeColor("TabControl.CloserColorActive");
            this._FocusColor = GetThemeColor("TabControl.FocusColor");
            this._TextColor = GetThemeColor("TabControl.TextColor");
            this._TextColorDisabled = GetThemeColor("TabControl.TextColorDisabled");
            this._TextColorSelected = GetThemeColor("TabControl.TextColorSelected");
            this.TabActiveSeparator = GetThemeColor("TabControl.TabActiveSeparator");
            this.TabBackColorActive = GetThemeColor("TabControl.TabBackColorActive");
            this.TabBackColorDisabled = GetThemeColor("TabControl.TabBackColorDisabled");
            this.TabBackColorHot = GetThemeColor("TabControl.TabBackColorHot");
            this.TabBackColor = GetThemeColor("TabControl.TabBackColor");
        }

        public override Brush GetPageBackgroundBrush(int index)
        {
            return new SolidBrush(this.TabActiveSeparator);
        }

        protected override Brush GetTabBackgroundBrush(int index)
        {
            SolidBrush brush = new SolidBrush(this.TabBackColor);
            if (this._TabControl.SelectedIndex == index)
            {
                brush = new SolidBrush(this.TabBackColorActive);
            }
            else if (!this._TabControl.TabPages[index].Enabled)
            {
                brush = new SolidBrush(this.TabBackColorDisabled);
            }
            else if (this._HotTrack && index == this._TabControl.ActiveIndex)
            {
                brush = new SolidBrush(this.TabBackColorHot);
            }
            return brush;
        }

        protected override void DrawTabFocusIndicator(GraphicsPath tabpath, int index, Graphics graphics)
        {
            if (this._FocusTrack && this._TabControl.Focused && index == this._TabControl.SelectedIndex)
            {
                int width = 3;
                Rectangle focusRect = Rectangle.Empty;
                RectangleF pathRect = tabpath.GetBounds();
                Brush focusBrush = new SolidBrush(this._FocusColor);
                switch (this._TabControl.Alignment)
                {
                    case TabAlignment.Top:
                        focusRect = new Rectangle((int)pathRect.X, (int)pathRect.Y, (int)pathRect.Width, width);
                        break;
                    case TabAlignment.Bottom:
                        focusRect = new Rectangle((int)pathRect.X, (int)pathRect.Bottom - width, (int)pathRect.Width, width);
                        break;
                    case TabAlignment.Left:
                        focusRect = new Rectangle((int)pathRect.X, (int)pathRect.Y, width, (int)pathRect.Height);
                        break;
                    case TabAlignment.Right:
                        focusRect = new Rectangle((int)pathRect.Right - width, (int)pathRect.Y, width, (int)pathRect.Height);
                        break;
                }
                //	Ensure the focus stip does not go outside the tab
                Region focusRegion = new Region(focusRect);
                focusRegion.Intersect(tabpath);
                graphics.FillRegion(focusBrush, focusRegion);
                focusRegion.Dispose();
                focusBrush.Dispose();
            }
        }

        public override void AddTabBorder(System.Drawing.Drawing2D.GraphicsPath path, System.Drawing.Rectangle tabBounds)
        {
            switch (this._TabControl.Alignment)
            {
                case TabAlignment.Top:
                    path.AddLine(tabBounds.X, tabBounds.Bottom, tabBounds.X, tabBounds.Y);
                    path.AddLine(tabBounds.X, tabBounds.Y, tabBounds.Right, tabBounds.Y);
                    path.AddLine(tabBounds.Right, tabBounds.Y, tabBounds.Right, tabBounds.Bottom);
                    break;
                case TabAlignment.Bottom:
                    path.AddLine(tabBounds.Right, tabBounds.Y, tabBounds.Right, tabBounds.Bottom);
                    path.AddLine(tabBounds.Right, tabBounds.Bottom, tabBounds.X, tabBounds.Bottom);
                    path.AddLine(tabBounds.X, tabBounds.Bottom, tabBounds.X, tabBounds.Y);
                    break;
                case TabAlignment.Left:
                    path.AddLine(tabBounds.Right, tabBounds.Bottom, tabBounds.X, tabBounds.Bottom);
                    path.AddLine(tabBounds.X, tabBounds.Bottom, tabBounds.X, tabBounds.Y);
                    path.AddLine(tabBounds.X, tabBounds.Y, tabBounds.Right, tabBounds.Y);
                    break;
                case TabAlignment.Right:
                    path.AddLine(tabBounds.X, tabBounds.Y, tabBounds.Right, tabBounds.Y);
                    path.AddLine(tabBounds.Right, tabBounds.Y, tabBounds.Right, tabBounds.Bottom);
                    path.AddLine(tabBounds.Right, tabBounds.Bottom, tabBounds.X, tabBounds.Bottom);
                    break;
            }
        }
    }

}
