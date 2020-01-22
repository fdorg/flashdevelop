// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System.Drawing;
using WeifenLuo.WinFormsUI.Docking;
using PluginCore;

namespace System.Windows.Forms
{
    [Flags]
    public enum DockBorders
    {
        None = 0,
        Left = 1 << 1,
        Top = 1 << 2,
        Right = 1 << 3,
        Bottom = 1 << 4
    }

    public class DockPanelControl : UserControl
    {
        readonly Pen borderPen;
        DockBorders borders;
        public bool AutoKeyHandling = false;

        public DockPanelControl()
        {
            Borders = DockBorders.Left | DockBorders.Right;
            var color = PluginBase.MainForm.GetThemeColor("DockPanelControl.BorderColor");
            borderPen = color != Color.Empty ? new Pen(color) : SystemPens.ControlDark;
        }

        DockBorders Borders
        {
            get => borders;
            set
            {
                borders = value;
                Padding = new Padding((borders & DockBorders.Left) > 0 ? 1 : 0, (borders & DockBorders.Top) > 0 ? 1 : 0, (borders & DockBorders.Right) > 0 ? 1 : 0, (borders & DockBorders.Bottom) > 0 ? 1 : 0);
            }
        }
        
        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (AutoKeyHandling
                && ContainsFocus
                && keyData == Keys.Escape
                && PluginBase.MainForm.CurrentDocument?.SciControl is { } sci)
            {
                sci.Focus();
                return true;
            }
            return base.ProcessDialogKey(keyData);
        }

        /// <summary>
        /// Actual painting is done here
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            CheckDockPosition();
            if ((borders & DockBorders.Left) > 0)
            {
                e.Graphics.DrawLine(borderPen, e.ClipRectangle.Left, e.ClipRectangle.Top, e.ClipRectangle.Left, e.ClipRectangle.Bottom + 1);
            }
            if ((borders & DockBorders.Top) > 0)
            {
                e.Graphics.DrawLine(borderPen, e.ClipRectangle.Left, e.ClipRectangle.Top, e.ClipRectangle.Right, e.ClipRectangle.Top);
            }
            if ((borders & DockBorders.Right) > 0)
            {
                e.Graphics.DrawLine(borderPen, e.ClipRectangle.Right - 1, e.ClipRectangle.Top, e.ClipRectangle.Right - 1, e.ClipRectangle.Bottom + 1);
            }
            if ((borders & DockBorders.Bottom) > 0)
            {
                e.Graphics.DrawLine(borderPen, e.ClipRectangle.Left, e.ClipRectangle.Bottom - 1, e.ClipRectangle.Right, e.ClipRectangle.Bottom - 1);
            }
        }

        /// <summary>
        /// Special logic that draws the borders for content
        /// </summary>
        void CheckDockPosition()
        {
            bool isOnlyTab;
            DockContent dock = Parent as DockContent;
            if (dock?.Pane is null) return;
            if (dock.IsFloat)
            {
                DockBorders local;
                isOnlyTab = CountPanels(false) == 1;
                if (isOnlyTab) local = DockBorders.Left | DockBorders.Top | DockBorders.Right | DockBorders.Bottom;
                else local = DockBorders.Left | DockBorders.Top | DockBorders.Right;
                if (dock.Pane.HasCaption) local -= DockBorders.Top;
                Borders = local;
            }
            else
            {
                isOnlyTab = CountPanels(true) == 1;
                if (isOnlyTab) Borders = DockBorders.Left | DockBorders.Bottom | DockBorders.Right;
                else Borders = DockBorders.Left | DockBorders.Right;
            }
        }

        bool IsAutoHidden(DockContent content)
        {
            var state = content.DockState;
            return state == DockState.DockLeftAutoHide
                   || state == DockState.DockRightAutoHide
                   || state == DockState.DockTopAutoHide
                   || state == DockState.DockBottomAutoHide;
        }

        /// <summary>
        /// Counts the the contents excluding hidden and floating (option) windows
        /// </summary>
        int CountPanels(bool includeFloats)
        {
            var count = 0;
            var dock = (DockContent) Parent;
            foreach (var it in dock.Pane.Contents)
            {
                if (it is DockContent)
                {
                    if (includeFloats && !it.DockHandler.IsHidden && !it.DockHandler.IsFloat) count++;
                    else if (!includeFloats && !it.DockHandler.IsHidden) count++;
                }
            }
            return count;
        }
    }
}