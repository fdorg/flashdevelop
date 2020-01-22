// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Collections;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace WeifenLuo.WinFormsUI.Docking
{
    public abstract partial class AutoHideStripBase : Control
    {
        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        protected class Tab : IDisposable
        {
            readonly IDockContent m_content;

            protected internal Tab(IDockContent content)
            {
                m_content = content;
            }

            ~Tab()
            {
                Dispose(false);
            }

            public IDockContent Content => m_content;

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(bool disposing)
            {
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        protected sealed class TabCollection : IEnumerable<Tab>
        {
            #region IEnumerable Members
            IEnumerator<Tab> IEnumerable<Tab>.GetEnumerator()
            {
                for (int i = 0; i < Count; i++)
                    yield return this[i];
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                for (int i = 0; i < Count; i++)
                    yield return this[i];
            }
            #endregion

            internal TabCollection(DockPane pane)
            {
                m_dockPane = pane;
            }

            readonly DockPane m_dockPane = null;
            public DockPane DockPane => m_dockPane;

            public DockPanel DockPanel => DockPane.DockPanel;

            public int Count => DockPane.DisplayingContents.Count;

            public Tab this[int index]
            {
                get
                {
                    IDockContent content = DockPane.DisplayingContents[index];
                    if (content is null)
                        throw (new ArgumentOutOfRangeException(nameof(index)));
                    if (content.DockHandler.AutoHideTab is null)
                        content.DockHandler.AutoHideTab = (DockPanel.AutoHideStripControl.CreateTab(content));
                    return content.DockHandler.AutoHideTab as Tab;
                }
            }

            public bool Contains(Tab tab)
            {
                return (IndexOf(tab) != -1);
            }

            public bool Contains(IDockContent content)
            {
                return (IndexOf(content) != -1);
            }

            public int IndexOf(Tab tab)
            {
                if (tab is null)
                    return -1;

                return IndexOf(tab.Content);
            }

            public int IndexOf(IDockContent content)
            {
                return DockPane.DisplayingContents.IndexOf(content);
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        protected class Pane : IDisposable
        {
            readonly DockPane m_dockPane;

            protected internal Pane(DockPane dockPane)
            {
                m_dockPane = dockPane;
            }

            ~Pane()
            {
                Dispose(false);
            }

            public DockPane DockPane => m_dockPane;

            public TabCollection AutoHideTabs
            {
                get
                {
                    if (DockPane.AutoHideTabs is null)
                        DockPane.AutoHideTabs = new TabCollection(DockPane);
                    return DockPane.AutoHideTabs as TabCollection;
                }
            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            protected virtual void Dispose(bool disposing)
            {
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        protected sealed class PaneCollection : IEnumerable<Pane>
        {
            class AutoHideState
            {
                public readonly DockState m_dockState;
                public bool m_selected = false;

                public AutoHideState(DockState dockState)
                {
                    m_dockState = dockState;
                }

                public DockState DockState => m_dockState;

                public bool Selected
                {
                    get => m_selected;
                    set => m_selected = value;
                }
            }

            class AutoHideStateCollection
            {
                readonly AutoHideState[] m_states;

                public AutoHideStateCollection()
                {
                    m_states = new[]  {   
                                                new AutoHideState(DockState.DockTopAutoHide),
                                                new AutoHideState(DockState.DockBottomAutoHide),
                                                new AutoHideState(DockState.DockLeftAutoHide),
                                                new AutoHideState(DockState.DockRightAutoHide)
                                            };
                }

                public AutoHideState this[DockState dockState]
                {
                    get
                    {
                        for (int i = 0; i < m_states.Length; i++)
                        {
                            if (m_states[i].DockState == dockState)
                                return m_states[i];
                        }
                        throw new ArgumentOutOfRangeException(nameof(dockState));
                    }
                }

                public bool ContainsPane(DockPane pane)
                {
                    if (pane.IsHidden)
                        return false;

                    for (int i = 0; i < m_states.Length; i++)
                    {
                        if (m_states[i].DockState == pane.DockState && m_states[i].Selected)
                            return true;
                    }
                    return false;
                }
            }

            internal PaneCollection(DockPanel panel, DockState dockState)
            {
                m_dockPanel = panel;
                m_states = new AutoHideStateCollection();
                States[DockState.DockTopAutoHide].Selected = (dockState == DockState.DockTopAutoHide);
                States[DockState.DockBottomAutoHide].Selected = (dockState == DockState.DockBottomAutoHide);
                States[DockState.DockLeftAutoHide].Selected = (dockState == DockState.DockLeftAutoHide);
                States[DockState.DockRightAutoHide].Selected = (dockState == DockState.DockRightAutoHide);
            }

            readonly DockPanel m_dockPanel;
            public DockPanel DockPanel => m_dockPanel;

            readonly AutoHideStateCollection m_states;
            AutoHideStateCollection States => m_states;

            public int Count
            {
                get
                {
                    int count = 0;
                    foreach (DockPane pane in DockPanel.Panes)
                    {
                        if (States.ContainsPane(pane))
                            count++;
                    }

                    return count;
                }
            }

            public Pane this[int index]
            {
                get
                {
                    int count = 0;
                    foreach (DockPane pane in DockPanel.Panes)
                    {
                        if (!States.ContainsPane(pane))
                            continue;

                        if (count == index)
                        {
                            if (pane.AutoHidePane is null)
                                pane.AutoHidePane = DockPanel.AutoHideStripControl.CreatePane(pane);
                            return pane.AutoHidePane as Pane;
                        }

                        count++;
                    }
                    throw new ArgumentOutOfRangeException(nameof(index));
                }
            }

            public bool Contains(Pane pane)
            {
                return (IndexOf(pane) != -1);
            }

            public int IndexOf(Pane pane)
            {
                if (pane is null)
                    return -1;

                int index = 0;
                foreach (DockPane dockPane in DockPanel.Panes)
                {
                    if (!States.ContainsPane(pane.DockPane))
                        continue;

                    if (pane == dockPane.AutoHidePane)
                        return index;

                    index++;
                }
                return -1;
            }

            #region IEnumerable Members

            IEnumerator<Pane> IEnumerable<Pane>.GetEnumerator()
            {
                for (int i = 0; i < Count; i++)
                    yield return this[i];
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                for (int i = 0; i < Count; i++)
                    yield return this[i];
            }

            #endregion
        }

        protected AutoHideStripBase(DockPanel panel)
        {
            m_dockPanel = panel;
            m_panesTop = new PaneCollection(panel, DockState.DockTopAutoHide);
            m_panesBottom = new PaneCollection(panel, DockState.DockBottomAutoHide);
            m_panesLeft = new PaneCollection(panel, DockState.DockLeftAutoHide);
            m_panesRight = new PaneCollection(panel, DockState.DockRightAutoHide);

            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.Selectable, false);
        }

        readonly DockPanel m_dockPanel;
        protected DockPanel DockPanel => m_dockPanel;

        readonly PaneCollection m_panesTop;
        protected PaneCollection PanesTop => m_panesTop;

        readonly PaneCollection m_panesBottom;
        protected PaneCollection PanesBottom => m_panesBottom;

        readonly PaneCollection m_panesLeft;
        protected PaneCollection PanesLeft => m_panesLeft;

        readonly PaneCollection m_panesRight;
        protected PaneCollection PanesRight => m_panesRight;

        protected PaneCollection GetPanes(DockState dockState)
        {
            if (dockState == DockState.DockTopAutoHide)
                return PanesTop;
            if (dockState == DockState.DockBottomAutoHide)
                return PanesBottom;
            if (dockState == DockState.DockLeftAutoHide)
                return PanesLeft;
            if (dockState == DockState.DockRightAutoHide)
                return PanesRight;
            throw new ArgumentOutOfRangeException(nameof(dockState));
        }

        internal int GetNumberOfPanes(DockState dockState)
        {
            return GetPanes(dockState).Count;
        }

        protected Rectangle RectangleTopLeft
        {
            get
            {   
                int height = MeasureHeight();
                return PanesTop.Count > 0 && PanesLeft.Count > 0 ? new Rectangle(0, 0, height, height) : Rectangle.Empty;
            }
        }

        protected Rectangle RectangleTopRight
        {
            get
            {
                int height = MeasureHeight();
                return PanesTop.Count > 0 && PanesRight.Count > 0 ? new Rectangle(Width - height, 0, height, height) : Rectangle.Empty;
            }
        }

        protected Rectangle RectangleBottomLeft
        {
            get
            {
                int height = MeasureHeight();
                return PanesBottom.Count > 0 && PanesLeft.Count > 0 ? new Rectangle(0, Height - height, height, height) : Rectangle.Empty;
            }
        }

        protected Rectangle RectangleBottomRight
        {
            get
            {
                int height = MeasureHeight();
                return PanesBottom.Count > 0 && PanesRight.Count > 0 ? new Rectangle(Width - height, Height - height, height, height) : Rectangle.Empty;
            }
        }

        protected internal Rectangle GetTabStripRectangle(DockState dockState)
        {
            int height = MeasureHeight();
            if (dockState == DockState.DockTopAutoHide && PanesTop.Count > 0)
                return new Rectangle(RectangleTopLeft.Width, 0, Width - RectangleTopLeft.Width - RectangleTopRight.Width, height);
            if (dockState == DockState.DockBottomAutoHide && PanesBottom.Count > 0)
                return new Rectangle(RectangleBottomLeft.Width, Height - height, Width - RectangleBottomLeft.Width - RectangleBottomRight.Width, height);
            if (dockState == DockState.DockLeftAutoHide && PanesLeft.Count > 0)
                return new Rectangle(0, RectangleTopLeft.Width, height, Height - RectangleTopLeft.Height - RectangleBottomLeft.Height);
            if (dockState == DockState.DockRightAutoHide && PanesRight.Count > 0)
                return new Rectangle(Width - height, RectangleTopRight.Width, height, Height - RectangleTopRight.Height - RectangleBottomRight.Height);
            return Rectangle.Empty;
        }

        GraphicsPath m_displayingArea = null;

        GraphicsPath DisplayingArea
        {
            get
            {
                if (m_displayingArea is null)
                    m_displayingArea = new GraphicsPath();

                return m_displayingArea;
            }
        }

        void SetRegion()
        {
            DisplayingArea.Reset();
            DisplayingArea.AddRectangle(RectangleTopLeft);
            DisplayingArea.AddRectangle(RectangleTopRight);
            DisplayingArea.AddRectangle(RectangleBottomLeft);
            DisplayingArea.AddRectangle(RectangleBottomRight);
            DisplayingArea.AddRectangle(GetTabStripRectangle(DockState.DockTopAutoHide));
            DisplayingArea.AddRectangle(GetTabStripRectangle(DockState.DockBottomAutoHide));
            DisplayingArea.AddRectangle(GetTabStripRectangle(DockState.DockLeftAutoHide));
            DisplayingArea.AddRectangle(GetTabStripRectangle(DockState.DockRightAutoHide));
            Region = new Region(DisplayingArea);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.Button != MouseButtons.Left)
                return;

            IDockContent content = HitTest();
            if (content is null)
                return;

            SetActiveAutoHideContent(content);

            content.DockHandler.Activate();
        }

        protected override void OnMouseHover(EventArgs e)
        {
            base.OnMouseHover(e);

            if (!DockPanel.ShowAutoHideContentOnHover) return;

            IDockContent content = HitTest();
            SetActiveAutoHideContent(content);

            // requires further tracking of mouse hover behavior,
            ResetMouseEventArgs();
        }

        void SetActiveAutoHideContent(IDockContent content)
        {
            if (content != null && DockPanel.ActiveAutoHideContent != content) DockPanel.ActiveAutoHideContent = content;
        }

        protected override void OnLayout(LayoutEventArgs levent)
        {
            RefreshChanges();
            base.OnLayout (levent);
        }

        internal void RefreshChanges()
        {
            if (IsDisposed) return;
            SetRegion();
            OnRefreshChanges();
        }

        protected virtual void OnRefreshChanges()
        {
        }

        protected internal abstract int MeasureHeight();

        IDockContent HitTest()
        {
            Point ptMouse = PointToClient(Control.MousePosition);
            return HitTest(ptMouse);
        }

        protected virtual Tab CreateTab(IDockContent content)
        {
            return new Tab(content);
        }

        protected virtual Pane CreatePane(DockPane dockPane)
        {
            return new Pane(dockPane);
        }

        protected abstract IDockContent HitTest(Point point);
    }
}
