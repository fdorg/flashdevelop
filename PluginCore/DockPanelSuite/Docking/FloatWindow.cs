using System;
using System.Drawing;
using System.Windows.Forms;
using System.Security.Permissions;
using System.Diagnostics.CodeAnalysis;
using PluginCore.DockPanelSuite;

namespace WeifenLuo.WinFormsUI.Docking
{
    public class FloatWindow : Form, INestedPanesContainer, IDockDragSource
    {
        private NestedPaneCollection m_nestedPanes;
        internal const int WM_CHECKDISPOSE = (int)(Win32.Msgs.WM_USER + 1);

        protected internal FloatWindow(DockPanel dockPanel, DockPane pane)
        {
            InternalConstruct(dockPanel, pane, false, Rectangle.Empty);
        }

        protected internal FloatWindow(DockPanel dockPanel, DockPane pane, Rectangle bounds)
        {
            InternalConstruct(dockPanel, pane, true, bounds);
        }

        private void InternalConstruct(DockPanel dockPanel, DockPane pane, bool boundsSpecified, Rectangle bounds)
        {
            if (dockPanel is null)
                throw(new ArgumentNullException(Strings.FloatWindow_Constructor_NullDockPanel));

            m_nestedPanes = new NestedPaneCollection(this);

            FormBorderStyle = FormBorderStyle.SizableToolWindow;
            ShowInTaskbar = false;
            if (dockPanel.RightToLeft != RightToLeft)
                RightToLeft = dockPanel.RightToLeft;
            if (RightToLeftLayout != dockPanel.RightToLeftLayout)
                RightToLeftLayout = dockPanel.RightToLeftLayout;
            
            SuspendLayout();
            if (boundsSpecified)
            {
                Bounds = bounds;
                StartPosition = FormStartPosition.Manual;
            }
            else
            {
                StartPosition = FormStartPosition.CenterParent;
                Size = dockPanel.DefaultFloatWindowSize;
            }

            m_dockPanel = dockPanel;
            Owner = DockPanel.FindForm();
            DockPanel.AddFloatWindow(this);
            if (pane != null)
                pane.FloatWindow = this;

            ResumeLayout();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                DockPanel?.RemoveFloatWindow(this);
                m_dockPanel = null;
            }
            base.Dispose(disposing);
        }

        private bool m_allowEndUserDocking = true;
        public bool AllowEndUserDocking
        {
            get => m_allowEndUserDocking;
            set => m_allowEndUserDocking = value;
        }

        private bool m_doubleClickTitleBarToDock = true;
        public bool DoubleClickTitleBarToDock
        {
            get => m_doubleClickTitleBarToDock;
            set => m_doubleClickTitleBarToDock = value;
        }

        public NestedPaneCollection NestedPanes => m_nestedPanes;

        public VisibleNestedPaneCollection VisibleNestedPanes => NestedPanes.VisibleNestedPanes;

        private DockPanel m_dockPanel;
        public DockPanel DockPanel => m_dockPanel;

        public DockState DockState => DockState.Float;

        public bool IsFloat => DockState == DockState.Float;

        internal bool IsDockStateValid(DockState dockState)
        {
            foreach (DockPane pane in NestedPanes)
                foreach (IDockContent content in pane.Contents)
                    if (!DockHelper.IsDockStateValid(dockState, content.DockHandler.DockAreas))
                        return false;

            return true;
        }

        protected override void OnActivated(EventArgs e)
        {
            DockPanel.FloatWindows.BringWindowToFront(this);
            base.OnActivated (e);
        }

        protected override void OnLayout(LayoutEventArgs levent)
        {
            VisibleNestedPanes.Refresh();
            RefreshChanges();
            Visible = (VisibleNestedPanes.Count > 0);
            SetText();

            base.OnLayout(levent);
        }


        [SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "System.Windows.Forms.Control.set_Text(System.String)")]
        internal void SetText()
        {
            DockPane theOnlyPane = (VisibleNestedPanes.Count == 1) ? VisibleNestedPanes[0] : null;

            if (theOnlyPane is null)
                Text = " "; // use " " instead of string.Empty because the whole title bar will disappear when ControlBox is set to false.
            else if (theOnlyPane.ActiveContent is null)
                Text = " ";
            else
                Text = theOnlyPane.ActiveContent.DockHandler.TabText;
        }

        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            Rectangle rectWorkArea = SystemInformation.VirtualScreen;

            if (y + height > rectWorkArea.Bottom)
                y -= (y + height) - rectWorkArea.Bottom;

            if (y < rectWorkArea.Top)
                y += rectWorkArea.Top - y;

            base.SetBoundsCore (x, y, width, height, specified);
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == (int)Win32.Msgs.WM_NCLBUTTONDOWN)
            {
                if (IsDisposed)
                    return;

                uint result = !NativeMethods.ShouldUseWin32() ? 0 : NativeMethods.SendMessage(this.Handle, (int)Win32.Msgs.WM_NCHITTEST, 0, (uint)m.LParam);
                if (result == 2 && DockPanel.AllowEndUserDocking && this.AllowEndUserDocking)   // HITTEST_CAPTION
                {
                    Activate();
                    m_dockPanel.BeginDrag(this);
                }
                else
                    base.WndProc(ref m);

                return;
            }

            if (m.Msg == (int)Win32.Msgs.WM_NCRBUTTONDOWN)
            {
                uint result = !NativeMethods.ShouldUseWin32() ? 0 : NativeMethods.SendMessage(this.Handle, (int)Win32.Msgs.WM_NCHITTEST, 0, (uint)m.LParam);
                if (result == 2)    // HITTEST_CAPTION
                {
                    DockPane theOnlyPane = (VisibleNestedPanes.Count == 1) ? VisibleNestedPanes[0] : null;
                    if (theOnlyPane?.ActiveContent != null)
                    {
                        theOnlyPane.ShowTabPageContextMenu(this, PointToClient(Control.MousePosition));
                        return;
                    }
                }

                base.WndProc(ref m);
                return;
            }
            if (m.Msg == (int)Win32.Msgs.WM_CLOSE)
            {
                if (NestedPanes.Count == 0)
                {
                    base.WndProc(ref m);
                    return;
                }

                for (int i = NestedPanes.Count - 1; i >= 0; i--)
                {
                    DockContentCollection contents = NestedPanes[i].Contents;
                    for (int j = contents.Count - 1; j >= 0; j--)
                    {
                        IDockContent content = contents[j];
                        if (content.DockHandler.DockState != DockState.Float)
                            continue;

                        if (!content.DockHandler.CloseButton)
                            continue;

                        if (content.DockHandler.HideOnClose)
                            content.DockHandler.Hide();
                        else
                            content.DockHandler.Close();
                    }
                }

                return;
            }
            if (m.Msg == (int)Win32.Msgs.WM_NCLBUTTONDBLCLK)
            {
                uint result = !DoubleClickTitleBarToDock || !NativeMethods.ShouldUseWin32() ? 0 : NativeMethods.SendMessage(this.Handle, (int)Win32.Msgs.WM_NCHITTEST, 0, (uint)m.LParam);
                if (result != 2)    // HITTEST_CAPTION
                {
                    base.WndProc(ref m);
                    return;
                }
                if (DockPanel.AllowEndUserFloatChange)
                {
                    DockPanel.SuspendLayout(true);

                    // Restore to panel
                    foreach (DockPane pane in NestedPanes)
                    {
                        if (pane.DockState != DockState.Float)
                            continue;
                        pane.RestoreToPanel();
                    }

                    DockPanel.ResumeLayout(true, true);
                    return;
                }
            }
            else if (m.Msg == WM_CHECKDISPOSE)
            {
                if (NestedPanes.Count == 0)
                    Dispose();

                return;
            }

            base.WndProc(ref m);
        }

        internal void RefreshChanges()
        {
            if (IsDisposed) return;
            if (VisibleNestedPanes.Count == 0)
            {
                ControlBox = true;
                return;
            }

            for (int i=VisibleNestedPanes.Count - 1; i>=0; i--)
            {
                DockContentCollection contents = VisibleNestedPanes[i].Contents;
                for (int j=contents.Count - 1; j>=0; j--)
                {
                    IDockContent content = contents[j];
                    if (content.DockHandler.DockState != DockState.Float)
                        continue;

                    if (content.DockHandler.CloseButton)
                    {
                        ControlBox = true;
                        return;
                    }
                }
            }
            ControlBox = false;
        }

        public virtual Rectangle DisplayingRectangle => ClientRectangle;

        internal void TestDrop(IDockDragSource dragSource, DockOutlineBase dockOutline)
        {
            if (VisibleNestedPanes.Count == 1)
            {
                DockPane pane = VisibleNestedPanes[0];
                if (!dragSource.CanDockTo(pane))
                    return;

                Point ptMouse = Control.MousePosition;
                uint lParam = Win32Helper.MakeLong(ptMouse.X, ptMouse.Y);
                if (NativeMethods.ShouldUseWin32())
                {
                    if (NativeMethods.SendMessage(Handle, (int)Win32.Msgs.WM_NCHITTEST, 0, lParam) == (uint)Win32.HitTest.HTCAPTION)
                    {
                        dockOutline.Show(VisibleNestedPanes[0], -1);
                    }
                }
            }
        }

        #region IDockDragSource Members

        #region IDragSource Members

        Control IDragSource.DragControl => this;

        #endregion

        bool IDockDragSource.IsDockStateValid(DockState dockState)
        {
            return IsDockStateValid(dockState);
        }

        bool IDockDragSource.CanDockTo(DockPane pane)
        {
            if (!IsDockStateValid(pane.DockState))
                return false;

            if (pane.FloatWindow == this)
                return false;

            return true;
        }

        Rectangle IDockDragSource.BeginDrag(Point ptMouse)
        {
            return Bounds;
        }

        public  void FloatAt(Rectangle floatWindowBounds)
        {
            Bounds = floatWindowBounds;
        }

        public void DockTo(DockPane pane, DockStyle dockStyle, int contentIndex)
        {
            if (dockStyle == DockStyle.Fill)
            {
                for (int i = NestedPanes.Count - 1; i >= 0; i--)
                {
                    DockPane paneFrom = NestedPanes[i];
                    for (int j = paneFrom.Contents.Count - 1; j >= 0; j--)
                    {
                        IDockContent c = paneFrom.Contents[j];
                        c.DockHandler.Pane = pane;
                        if (contentIndex != -1)
                            pane.SetContentIndex(c, contentIndex);
                    }
                }
            }
            else
            {
                DockAlignment alignment = DockAlignment.Left;
                if (dockStyle == DockStyle.Left)
                    alignment = DockAlignment.Left;
                else if (dockStyle == DockStyle.Right)
                    alignment = DockAlignment.Right;
                else if (dockStyle == DockStyle.Top)
                    alignment = DockAlignment.Top;
                else if (dockStyle == DockStyle.Bottom)
                    alignment = DockAlignment.Bottom;

                MergeNestedPanes(VisibleNestedPanes, pane.NestedPanesContainer.NestedPanes, pane, alignment, 0.5);
            }
        }

        public void DockTo(DockPanel panel, DockStyle dockStyle)
        {
            if (panel != DockPanel)
                throw new ArgumentException(Strings.IDockDragSource_DockTo_InvalidPanel, nameof(panel));

            var nestedPanesTo = dockStyle switch
            {
                DockStyle.Top => DockPanel.DockWindows[DockState.DockTop].NestedPanes,
                DockStyle.Bottom => DockPanel.DockWindows[DockState.DockBottom].NestedPanes,
                DockStyle.Left => DockPanel.DockWindows[DockState.DockLeft].NestedPanes,
                DockStyle.Right => DockPanel.DockWindows[DockState.DockRight].NestedPanes,
                DockStyle.Fill => DockPanel.DockWindows[DockState.Document].NestedPanes,
                _ => null
            };

            DockPane prevPane = null;
            for (int i = nestedPanesTo.Count - 1; i >= 0; i--)
                if (nestedPanesTo[i] != VisibleNestedPanes[0])
                    prevPane = nestedPanesTo[i];
            MergeNestedPanes(VisibleNestedPanes, nestedPanesTo, prevPane, DockAlignment.Left, 0.5);
        }

        private static void MergeNestedPanes(VisibleNestedPaneCollection nestedPanesFrom, NestedPaneCollection nestedPanesTo, DockPane prevPane, DockAlignment alignment, double proportion)
        {
            if (nestedPanesFrom.Count == 0)
                return;

            int count = nestedPanesFrom.Count;
            DockPane[] panes = new DockPane[count];
            DockPane[] prevPanes = new DockPane[count];
            DockAlignment[] alignments = new DockAlignment[count];
            double[] proportions = new double[count];

            for (int i = 0; i < count; i++)
            {
                panes[i] = nestedPanesFrom[i];
                prevPanes[i] = nestedPanesFrom[i].NestedDockingStatus.PreviousPane;
                alignments[i] = nestedPanesFrom[i].NestedDockingStatus.Alignment;
                proportions[i] = nestedPanesFrom[i].NestedDockingStatus.Proportion;
            }

            DockPane pane = panes[0].DockTo(nestedPanesTo.Container, prevPane, alignment, proportion);
            panes[0].DockState = nestedPanesTo.DockState;

            for (int i = 1; i < count; i++)
            {
                for (int j = i; j < count; j++)
                {
                    if (prevPanes[j] == panes[i - 1])
                        prevPanes[j] = pane;
                }
                pane = panes[i].DockTo(nestedPanesTo.Container, prevPanes[i], alignments[i], proportions[i]);
                panes[i].DockState = nestedPanesTo.DockState;
            }
        }

        #endregion
    }
}
