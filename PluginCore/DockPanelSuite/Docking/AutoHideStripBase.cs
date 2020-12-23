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
            protected internal Tab(IDockContent content) => Content = content;

            ~Tab() => Dispose(false);

            public IDockContent Content { get; }

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
                DockPane = pane;
            }

            public DockPane DockPane { get; }

            public DockPanel DockPanel => DockPane.DockPanel;

            public int Count => DockPane.DisplayingContents.Count;

            public Tab this[int index]
            {
                get
                {
                    var content = DockPane.DisplayingContents[index];
                    if (content is null) throw new ArgumentOutOfRangeException(nameof(index));
                    content.DockHandler.AutoHideTab ??= (DockPanel.AutoHideStripControl.CreateTab(content));
                    return content.DockHandler.AutoHideTab as Tab;
                }
            }

            public bool Contains(Tab tab) => IndexOf(tab) != -1;

            public bool Contains(IDockContent content) => IndexOf(content) != -1;

            public int IndexOf(Tab tab)
            {
                if (tab is null) return -1;
                return IndexOf(tab.Content);
            }

            public int IndexOf(IDockContent content) => DockPane.DisplayingContents.IndexOf(content);
        }

        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible")]
        protected class Pane : IDisposable
        {
            protected internal Pane(DockPane dockPane)
            {
                DockPane = dockPane;
            }

            ~Pane()
            {
                Dispose(false);
            }

            public DockPane DockPane { get; }

            public TabCollection AutoHideTabs
            {
                get
                {
                    DockPane.AutoHideTabs ??= new TabCollection(DockPane);
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
                public bool m_selected;

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
                DockPanel = panel;
                States = new AutoHideStateCollection();
                States[DockState.DockTopAutoHide].Selected = (dockState == DockState.DockTopAutoHide);
                States[DockState.DockBottomAutoHide].Selected = (dockState == DockState.DockBottomAutoHide);
                States[DockState.DockLeftAutoHide].Selected = (dockState == DockState.DockLeftAutoHide);
                States[DockState.DockRightAutoHide].Selected = (dockState == DockState.DockRightAutoHide);
            }

            public DockPanel DockPanel { get; }

            AutoHideStateCollection States { get; }

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
            DockPanel = panel;
            PanesTop = new PaneCollection(panel, DockState.DockTopAutoHide);
            PanesBottom = new PaneCollection(panel, DockState.DockBottomAutoHide);
            PanesLeft = new PaneCollection(panel, DockState.DockLeftAutoHide);
            PanesRight = new PaneCollection(panel, DockState.DockRightAutoHide);

            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.Selectable, false);
        }

        protected DockPanel DockPanel { get; }

        protected PaneCollection PanesTop { get; }

        protected PaneCollection PanesBottom { get; }

        protected PaneCollection PanesLeft { get; }

        protected PaneCollection PanesRight { get; }

        protected PaneCollection GetPanes(DockState dockState)
        {
            return dockState switch
            {
                DockState.DockTopAutoHide => PanesTop,
                DockState.DockBottomAutoHide => PanesBottom,
                DockState.DockLeftAutoHide => PanesLeft,
                DockState.DockRightAutoHide => PanesRight,
                _ => throw new ArgumentOutOfRangeException(nameof(dockState))
            };
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
            return dockState switch
            {
                DockState.DockTopAutoHide when PanesTop.Count > 0 => new Rectangle(RectangleTopLeft.Width, 0,
                    Width - RectangleTopLeft.Width - RectangleTopRight.Width, height),
                DockState.DockBottomAutoHide when PanesBottom.Count > 0 => new Rectangle(RectangleBottomLeft.Width,
                    Height - height, Width - RectangleBottomLeft.Width - RectangleBottomRight.Width, height),
                DockState.DockLeftAutoHide when PanesLeft.Count > 0 => new Rectangle(0, RectangleTopLeft.Width, height,
                    Height - RectangleTopLeft.Height - RectangleBottomLeft.Height),
                DockState.DockRightAutoHide when PanesRight.Count > 0 => new Rectangle(Width - height,
                    RectangleTopRight.Width, height, Height - RectangleTopRight.Height - RectangleBottomRight.Height),
                _ => Rectangle.Empty
            };
        }

        GraphicsPath m_displayingArea;

        GraphicsPath DisplayingArea => m_displayingArea ??= new GraphicsPath();

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
