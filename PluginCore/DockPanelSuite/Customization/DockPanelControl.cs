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
        Pen borderPen;
        DockBorders borders;
        public Boolean AutoKeyHandling = false;

        public DockPanelControl()
        {
            Borders = DockBorders.Left | DockBorders.Right;
            Color color = PluginCore.PluginBase.MainForm.GetThemeColor("DockPanelControl.BorderColor");
            if (color != Color.Empty) borderPen = new Pen(color);
            else borderPen = SystemPens.ControlDark;
        }

        private DockBorders Borders
        {
            get { return borders; }
            set
            {
                borders = value;
                this.Padding = new Padding((borders & DockBorders.Left) > 0 ? 1 : 0, (borders & DockBorders.Top) > 0 ? 1 : 0, (borders & DockBorders.Right) > 0 ? 1 : 0, (borders & DockBorders.Bottom) > 0 ? 1 : 0);
            }
        }
        
        protected override Boolean ProcessDialogKey(Keys keyData)
        {
            if (this.AutoKeyHandling && this.ContainsFocus)
            {
                if (keyData == Keys.Escape)
                {
                    ITabbedDocument doc = PluginBase.MainForm.CurrentDocument;
                    if (doc != null && doc.IsEditable) 
                    {
                        doc.SciControl.Focus();
                        return true;
                    }
                }
            }
            return base.ProcessDialogKey(keyData);
        }

        /// <summary>
        /// Actual painting is done here
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            this.CheckDockPosition();
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
        private void CheckDockPosition()
        {
            Boolean isOnlyTab;
            DockContent dock = this.Parent as DockContent;
            if (dock == null || dock.Pane == null) return;
            if (dock.IsFloat)
            {
                DockBorders local;
                isOnlyTab = this.CountPanels(false) == 1;
                if (isOnlyTab) local = DockBorders.Left | DockBorders.Top | DockBorders.Right | DockBorders.Bottom;
                else local = DockBorders.Left | DockBorders.Top | DockBorders.Right;
                if (dock.Pane.HasCaption) local -= DockBorders.Top;
                Borders = local;
            }
            else
            {
                isOnlyTab = this.CountPanels(true) == 1;
                if (isOnlyTab) Borders = DockBorders.Left | DockBorders.Bottom | DockBorders.Right;
                else Borders = DockBorders.Left | DockBorders.Right;
            }
        }

        private Boolean IsAutoHidden(DockContent content)
        {
            DockState state = content.DockState;
            return state == DockState.DockLeftAutoHide || state == DockState.DockRightAutoHide 
                || state == DockState.DockTopAutoHide || state == DockState.DockBottomAutoHide;
        }

        /// <summary>
        /// Counts the the contents excluding hidden and floating (option) windows
        /// </summary>
        private Int32 CountPanels(Boolean includeFloats)
        {
            Int32 count = 0;
            DockContent dock = this.Parent as DockContent;
            for (Int32 i = 0; i < dock.Pane.Contents.Count; i++)
            {
                if (dock.Pane.Contents[i] is DockContent)
                {
                    IDockContent current = dock.Pane.Contents[i];
                    if (includeFloats && !current.DockHandler.IsHidden && !current.DockHandler.IsFloat) count++;
                    else if (!includeFloats && !current.DockHandler.IsHidden) count++;
                }
            }
            return count;
        }

    }

}
