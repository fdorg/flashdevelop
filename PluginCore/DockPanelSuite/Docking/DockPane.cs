using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Security.Permissions;
using System.Diagnostics.CodeAnalysis;
using PluginCore.DockPanelSuite;

namespace WeifenLuo.WinFormsUI.Docking
{
    [ToolboxItem(false)]
    public partial class DockPane : UserControl, IDockDragSource
    {
        public enum AppearanceStyle
        {
            ToolWindow,
            Document
        }

        enum HitTestArea
        {
            Caption,
            TabStrip,
            Content,
            None
        }

        struct HitTestResult
        {
            public readonly HitTestArea HitArea;
            public readonly int Index;

            public HitTestResult(HitTestArea hitTestArea, int index)
            {
                HitArea = hitTestArea;
                Index = index;
            }
        }

        DockPaneCaptionBase CaptionControl { get; set; }

        internal DockPaneStripBase TabStripControl { get; set; }

        protected internal DockPane(IDockContent content, DockState visibleState, bool show)
        {
            InternalConstruct(content, visibleState, false, Rectangle.Empty, null, DockAlignment.Right, 0.5, show);
        }

        [SuppressMessage("Microsoft.Naming", "CA1720:AvoidTypeNamesInParameters", MessageId = "1#")]
        protected internal DockPane(IDockContent content, FloatWindow floatWindow, bool show)
        {
            if (floatWindow is null)
                throw new ArgumentNullException(nameof(floatWindow));

            InternalConstruct(content, DockState.Float, false, Rectangle.Empty, floatWindow.NestedPanes.GetDefaultPreviousPane(this), DockAlignment.Right, 0.5, show);
        }

        protected internal DockPane(IDockContent content, DockPane previousPane, DockAlignment alignment, double proportion, bool show)
        {
            if (previousPane is null)
                throw(new ArgumentNullException(nameof(previousPane)));
            InternalConstruct(content, previousPane.DockState, false, Rectangle.Empty, previousPane, alignment, proportion, show);
        }

        [SuppressMessage("Microsoft.Naming", "CA1720:AvoidTypeNamesInParameters", MessageId = "1#")]
        protected internal DockPane(IDockContent content, Rectangle floatWindowBounds, bool show)
        {
            InternalConstruct(content, DockState.Float, true, floatWindowBounds, null, DockAlignment.Right, 0.5, show);
        }

        void InternalConstruct(IDockContent content, DockState dockState, bool flagBounds, Rectangle floatWindowBounds, DockPane prevPane, DockAlignment alignment, double proportion, bool show)
        {
            if (dockState == DockState.Hidden || dockState == DockState.Unknown)
                throw new ArgumentException(Strings.DockPane_SetDockState_InvalidState);

            if (content is null)
                throw new ArgumentNullException(Strings.DockPane_Constructor_NullContent);

            if (content.DockHandler.DockPanel is null)
                throw new ArgumentException(Strings.DockPane_Constructor_NullDockPanel);


            SuspendLayout();
            SetStyle(ControlStyles.Selectable, false);

            IsFloat = (dockState == DockState.Float);

            Contents = new DockContentCollection();
            DisplayingContents = new DockContentCollection(this);
            DockPanel = content.DockHandler.DockPanel;
            DockPanel.AddPane(this);

            m_splitter = new SplitterControl(this);

            NestedDockingStatus = new NestedDockingStatus(this);

            CaptionControl = DockPanel.DockPaneCaptionFactory.CreateDockPaneCaption(this);
            TabStripControl = DockPanel.DockPaneStripFactory.CreateDockPaneStrip(this);
            Controls.AddRange(new Control[] {   CaptionControl, TabStripControl });
            
            DockPanel.SuspendLayout(true);
            if (flagBounds)
                FloatWindow = DockPanel.FloatWindowFactory.CreateFloatWindow(DockPanel, this, floatWindowBounds);
            else if (prevPane != null)
                DockTo(prevPane.NestedPanesContainer, prevPane, alignment, proportion);

            SetDockState(dockState);
            if (show)
                content.DockHandler.Pane = this;
            else if (IsFloat)
                content.DockHandler.FloatPane = this;
            else
                content.DockHandler.PanelPane = this;

            ResumeLayout();
            DockPanel.ResumeLayout(true, true);
        }

        bool m_inDisposing;
        protected override void Dispose(bool disposing)
        {
            // IMPORTANT: avoid nested call into this method on Mono. 
            // https://github.com/dockpanelsuite/dockpanelsuite/issues/16
            if (!NativeMethods.ShouldUseWin32() && m_inDisposing)
            {
                return;
            }

            if (!NativeMethods.ShouldUseWin32())
            {
                m_inDisposing = true;
            }

            if (disposing)
            {
                m_dockState = DockState.Unknown;

                NestedPanesContainer?.NestedPanes.Remove(this);

                if (DockPanel != null)
                {
                    DockPanel.RemovePane(this);
                    DockPanel = null;
                }

                Splitter.Dispose();
                AutoHidePane?.Dispose();
            }
            base.Dispose(disposing);
        }

        IDockContent m_activeContent;
        public virtual IDockContent ActiveContent
        {
            get => m_activeContent;
            set
            {
                if (ActiveContent == value)
                    return;

                if (value != null)
                {
                    if (!DisplayingContents.Contains(value)) return;
                        //throw(new InvalidOperationException(Strings.DockPane_ActiveContent_InvalidValue));
                }
                else
                {
                    if (DisplayingContents.Count != 0) return;
                        //throw(new InvalidOperationException(Strings.DockPane_ActiveContent_InvalidValue));
                }

                IDockContent oldValue = m_activeContent;

                if (DockPanel.ActiveAutoHideContent == oldValue)
                    DockPanel.ActiveAutoHideContent = null;

                m_activeContent = value;

                if (DockPanel.DocumentStyle == DocumentStyle.DockingMdi && DockState == DockState.Document)
                {
                    m_activeContent?.DockHandler.Form.BringToFront();
                }
                else
                {
                    // NICK: Prevents flicker
                    BackColor = Color.Transparent;

                    m_activeContent?.DockHandler.SetVisible();
                    if (oldValue != null && DisplayingContents.Contains(oldValue))
                        oldValue.DockHandler.SetVisible();
                }

                FloatWindow?.SetText();

                if (DockPanel.DocumentStyle == DocumentStyle.DockingMdi &&
                    DockState == DockState.Document)
                    RefreshChanges(false);  // delayed layout to reduce screen flicker
                else
                    RefreshChanges();

                if (m_activeContent != null)
                    TabStripControl.EnsureTabVisible(m_activeContent);
            }
        }

        public virtual bool AllowDockDragAndDrop { get; set; } = true;

        internal IDisposable AutoHidePane { get; set; } = null;

        internal object AutoHideTabs { get; set; } = null;

        ContextMenuStrip TabPageContextMenu
        {
            get
            {
                var content = ActiveContent;
                if (content is null) return null;
                return content.DockHandler.TabPageContextMenuStrip
                       ?? content.DockHandler.TabPageContextMenu;
            }
        }

        internal bool HasTabPageContextMenu => TabPageContextMenu != null;

        internal void ShowTabPageContextMenu(Control control, Point position)
        {
            if (TabPageContextMenu is { } contextMenuStrip) contextMenuStrip.Show(control, position);
        }

        Rectangle CaptionRectangle
        {
            get
            {
                if (!HasCaption) return Rectangle.Empty;
                var rectWindow = DisplayingRectangle;
                var x = rectWindow.X;
                var y = rectWindow.Y;
                var width = rectWindow.Width;
                var height = CaptionControl.MeasureHeight();
                return new Rectangle(x, y, width, height);
            }
        }

        internal Rectangle ContentRectangle
        {
            get
            {
                Rectangle rectWindow = DisplayingRectangle;
                Rectangle rectCaption = CaptionRectangle;
                Rectangle rectTabStrip = TabStripRectangle;

                int x = rectWindow.X;
                int y = rectWindow.Y + (rectCaption.IsEmpty ? 0 : rectCaption.Height) +
                    (DockState == DockState.Document ? rectTabStrip.Height : 0);
                int width = rectWindow.Width;
                int height = rectWindow.Height - rectCaption.Height - rectTabStrip.Height;

                return new Rectangle(x, y, width, height);
            }
        }

        internal Rectangle TabStripRectangle => Appearance == AppearanceStyle.ToolWindow
            ? TabStripRectangle_ToolWindow
            : TabStripRectangle_Document;

        Rectangle TabStripRectangle_ToolWindow
        {
            get
            {
                if (DisplayingContents.Count <= 1 || IsAutoHide)
                    return Rectangle.Empty;

                Rectangle rectWindow = DisplayingRectangle;

                int width = rectWindow.Width;
                int height = TabStripControl.MeasureHeight();
                int x = rectWindow.X;
                int y = rectWindow.Bottom - height;
                Rectangle rectCaption = CaptionRectangle;
                if (rectCaption.Contains(x, y))
                    y = rectCaption.Y + rectCaption.Height;

                return new Rectangle(x, y, width, height);
            }
        }

        Rectangle TabStripRectangle_Document
        {
            get
            {
                if (DisplayingContents.Count == 0)
                    return Rectangle.Empty;

                if (DisplayingContents.Count == 1 && DockPanel.DocumentStyle == DocumentStyle.DockingSdi)
                    return Rectangle.Empty;

                Rectangle rectWindow = DisplayingRectangle;
                int x = rectWindow.X;
                int y = rectWindow.Y;
                int width = rectWindow.Width;
                int height = TabStripControl.MeasureHeight();

                return new Rectangle(x, y, width, height);
            }
        }

        public virtual string CaptionText => ActiveContent is null ? string.Empty : ActiveContent.DockHandler.TabText;

        public DockContentCollection Contents { get; set; }

        public DockContentCollection DisplayingContents { get; set; }

        public DockPanel DockPanel { get; set; }

        public bool HasCaption => DockState != DockState.Document
                                  && DockState != DockState.Hidden
                                  && DockState != DockState.Unknown
                                  && (DockState != DockState.Float || FloatWindow.VisibleNestedPanes.Count > 1);

        public bool IsActivated { get; set; }

        internal void SetIsActivated(bool value)
        {
            if (IsActivated == value)
                return;

            IsActivated = value;
            if (DockState != DockState.Document)
                RefreshChanges(false);
            OnIsActivatedChanged(EventArgs.Empty);
        }

        public bool IsActiveDocumentPane { get; set; }

        internal void SetIsActiveDocumentPane(bool value)
        {
            if (IsActiveDocumentPane == value)
                return;

            IsActiveDocumentPane = value;
            if (DockState == DockState.Document)
                RefreshChanges();
            OnIsActiveDocumentPaneChanged(EventArgs.Empty);
        }

        public bool IsDockStateValid(DockState dockState)
        {
            foreach (IDockContent content in Contents)
                if (!content.DockHandler.IsDockStateValid(dockState))
                    return false;

            return true;
        }

        public bool IsAutoHide => DockHelper.IsDockStateAutoHide(DockState);

        public AppearanceStyle Appearance => (DockState == DockState.Document) ? AppearanceStyle.Document : AppearanceStyle.ToolWindow;

        internal Rectangle DisplayingRectangle => ClientRectangle;

        public void Activate()
        {
            if (DockHelper.IsDockStateAutoHide(DockState) && DockPanel.ActiveAutoHideContent != ActiveContent)
                DockPanel.ActiveAutoHideContent = ActiveContent;
            else if (!IsActivated)
                ActiveContent?.DockHandler.Activate();
        }

        internal void AddContent(IDockContent content)
        {
            if (Contents.Contains(content))
                return;

            Contents.Add(content);
        }

        internal void Close() => Dispose();

        public void CloseActiveContent() => CloseContent(ActiveContent);

        internal void CloseContent(IDockContent content)
        {
            DockPanel dockPanel = DockPanel;
            dockPanel.SuspendLayout(true);

            if (content is null)
                return;

            if (!content.DockHandler.CloseButton)
                return;

            if (content.DockHandler.HideOnClose)
                content.DockHandler.Hide();
            else
                content.DockHandler.Close();

            dockPanel.ResumeLayout(true, true);
        }

        HitTestResult GetHitTest(Point ptMouse)
        {
            Point ptMouseClient = PointToClient(ptMouse);

            Rectangle rectCaption = CaptionRectangle;
            if (rectCaption.Contains(ptMouseClient))
                return new HitTestResult(HitTestArea.Caption, -1);

            Rectangle rectContent = ContentRectangle;
            if (rectContent.Contains(ptMouseClient))
                return new HitTestResult(HitTestArea.Content, -1);

            Rectangle rectTabStrip = TabStripRectangle;
            if (rectTabStrip.Contains(ptMouseClient))
                return new HitTestResult(HitTestArea.TabStrip, TabStripControl.HitTest(TabStripControl.PointToClient(ptMouse)));

            return new HitTestResult(HitTestArea.None, -1);
        }

        public bool IsHidden { get; set; } = true;

        void SetIsHidden(bool value)
        {
            if (IsHidden == value)
                return;

            IsHidden = value;
            if (DockHelper.IsDockStateAutoHide(DockState))
            {
                DockPanel.RefreshAutoHideStrip();
                DockPanel.PerformLayout();
            }
            else
            {
                ((Control) NestedPanesContainer)?.PerformLayout();
            }
        }

        protected override void OnLayout(LayoutEventArgs levent)
        {
            SetIsHidden(DisplayingContents.Count == 0);
            if (!IsHidden)
            {
                CaptionControl.Bounds = CaptionRectangle;
                TabStripControl.Bounds = TabStripRectangle;

                SetContentBounds();

                foreach (IDockContent content in Contents)
                {
                    if (DisplayingContents.Contains(content))
                        if (content.DockHandler.FlagClipWindow && content.DockHandler.Form.Visible)
                            content.DockHandler.FlagClipWindow = false;
                }
            }

            base.OnLayout(levent);
        }

        internal void SetContentBounds()
        {
            Rectangle rectContent = ContentRectangle;
            if (DockState == DockState.Document && DockPanel.DocumentStyle == DocumentStyle.DockingMdi)
                rectContent = DockPanel.RectangleToMdiClient(RectangleToScreen(rectContent));

            Rectangle rectInactive = new Rectangle(-rectContent.Width, rectContent.Y, rectContent.Width, rectContent.Height);
            foreach (IDockContent content in Contents)
                if (content.DockHandler.Pane == this)
                {
                    if (content == ActiveContent)
                        content.DockHandler.Form.Bounds = rectContent;
                    else
                        content.DockHandler.Form.Bounds = rectInactive;
                }
        }

        internal void RefreshChanges() => RefreshChanges(true);

        void RefreshChanges(bool performLayout)
        {
            if (IsDisposed) return;
            CaptionControl.RefreshChanges();
            TabStripControl.RefreshChanges();
            if (DockState == DockState.Float) FloatWindow.RefreshChanges();
            if (DockHelper.IsDockStateAutoHide(DockState) && DockPanel != null)
            {
                DockPanel.RefreshAutoHideStrip();
                DockPanel.PerformLayout();
            }
            if (performLayout) PerformLayout();
        }

        internal void RemoveContent(IDockContent content)
        {
            if (!Contents.Contains(content)) return;
            Contents.Remove(content);
        }

        public void SetContentIndex(IDockContent content, int index)
        {
            int oldIndex = Contents.IndexOf(content);
            if (oldIndex == -1)
                throw(new ArgumentException(Strings.DockPane_SetContentIndex_InvalidContent));

            if (index < 0 || index > Contents.Count - 1)
                if (index != -1)
                    throw(new ArgumentOutOfRangeException(Strings.DockPane_SetContentIndex_InvalidIndex));
                
            if (oldIndex == index)
                return;
            if (oldIndex == Contents.Count - 1 && index == -1)
                return;

            Contents.Remove(content);
            if (index == -1)
                Contents.Add(content);
            else if (oldIndex < index)
                Contents.AddAt(content, index - 1);
            else
                Contents.AddAt(content, index);

            RefreshChanges();
        }

        void SetParent()
        {
            if (DockState == DockState.Unknown || DockState == DockState.Hidden)
            {
                SetParent(null);
                Splitter.Parent = null;
            }
            else if (DockState == DockState.Float)
            {
                SetParent(FloatWindow);
                Splitter.Parent = FloatWindow;
            }
            else if (DockHelper.IsDockStateAutoHide(DockState))
            {
                SetParent(DockPanel.AutoHideControl);
                Splitter.Parent = null;
            }
            else
            {
                SetParent(DockPanel.DockWindows[DockState]);
                Splitter.Parent = Parent;
            }
        }

        void SetParent(Control value)
        {
            if (Parent == value)
                return;

            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            // Workaround of .Net Framework bug:
            // Change the parent of a control with focus may result in the first
            // MDI child form get activated. 
            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            IDockContent contentFocused = GetFocusedContent();
            if (contentFocused != null)
                DockPanel.SaveFocus();

            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

            Parent = value;

            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            // Workaround of .Net Framework bug:
            // Change the parent of a control with focus may result in the first
            // MDI child form get activated. 
            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            contentFocused?.DockHandler.Activate();
            //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        }

        public new void Show()
        {
            Activate();
        }

        internal void TestDrop(IDockDragSource dragSource, DockOutlineBase dockOutline)
        {
            if (!dragSource.CanDockTo(this))
                return;

            Point ptMouse = MousePosition;

            HitTestResult hitTestResult = GetHitTest(ptMouse);
            if (hitTestResult.HitArea == HitTestArea.Caption)
                dockOutline.Show(this, -1);
            else if (hitTestResult.HitArea == HitTestArea.TabStrip && hitTestResult.Index != -1)
                dockOutline.Show(this, hitTestResult.Index);
        }

        internal void ValidateActiveContent()
        {
            if (ActiveContent is null)
            {
                if (DisplayingContents.Count != 0)
                    ActiveContent = DisplayingContents[0];
                return;
            }

            if (DisplayingContents.IndexOf(ActiveContent) >= 0)
                return;

            IDockContent prevVisible = null;
            for (int i=Contents.IndexOf(ActiveContent)-1; i>=0; i--)
                if (Contents[i].DockHandler.DockState == DockState)
                {
                    prevVisible = Contents[i];
                    break;
                }

            IDockContent nextVisible = null;
            for (int i=Contents.IndexOf(ActiveContent)+1; i<Contents.Count; i++)
                if (Contents[i].DockHandler.DockState == DockState)
                {
                    nextVisible = Contents[i];
                    break;
                }

            if (prevVisible != null)
                ActiveContent = prevVisible;
            else if (nextVisible != null)
                ActiveContent = nextVisible;
            else
                ActiveContent = null;
        }

        static readonly object DockStateChangedEvent = new object();
        public event EventHandler DockStateChanged
        {
            add => Events.AddHandler(DockStateChangedEvent, value);
            remove => Events.RemoveHandler(DockStateChangedEvent, value);
        }
        protected virtual void OnDockStateChanged(EventArgs e)
        {
            EventHandler handler = (EventHandler)Events[DockStateChangedEvent];
            handler?.Invoke(this, e);
        }

        static readonly object IsActivatedChangedEvent = new object();
        public event EventHandler IsActivatedChanged
        {
            add => Events.AddHandler(IsActivatedChangedEvent, value);
            remove => Events.RemoveHandler(IsActivatedChangedEvent, value);
        }
        protected virtual void OnIsActivatedChanged(EventArgs e)
        {
            EventHandler handler = (EventHandler)Events[IsActivatedChangedEvent];
            handler?.Invoke(this, e);
        }

        static readonly object IsActiveDocumentPaneChangedEvent = new object();
        public event EventHandler IsActiveDocumentPaneChanged
        {
            add => Events.AddHandler(IsActiveDocumentPaneChangedEvent, value);
            remove => Events.RemoveHandler(IsActiveDocumentPaneChangedEvent, value);
        }
        protected virtual void OnIsActiveDocumentPaneChanged(EventArgs e)
        {
            EventHandler handler = (EventHandler)Events[IsActiveDocumentPaneChangedEvent];
            handler?.Invoke(this, e);
        }

        public DockWindow DockWindow
        {
            get => NestedDockingStatus.NestedPanes?.Container as DockWindow;
            set
            {
                var oldValue = DockWindow;
                if (oldValue == value) return;
                DockTo(value);
            }
        }

        public FloatWindow FloatWindow
        {
            get => NestedDockingStatus.NestedPanes?.Container as FloatWindow;
            set
            {
                var oldValue = FloatWindow;
                if (oldValue == value) return;
                DockTo(value);
            }
        }

        public NestedDockingStatus NestedDockingStatus { get; set; }

        public bool IsFloat { get; set; }

        public INestedPanesContainer NestedPanesContainer => NestedDockingStatus.NestedPanes?.Container;

        DockState m_dockState = DockState.Unknown;
        public DockState DockState
        {
            get => m_dockState;
            set => SetDockState(value);
        }

        public DockPane SetDockState(DockState value)
        {
            if (value == DockState.Unknown || value == DockState.Hidden)
                throw new InvalidOperationException(Strings.DockPane_SetDockState_InvalidState);

            if ((value == DockState.Float) == IsFloat)
            {
                InternalSetDockState(value);
                return this;
            }
            if (DisplayingContents.Count == 0) return null;
            IDockContent firstContent = null;
            for (int i=0; i<DisplayingContents.Count; i++)
            {
                IDockContent content = DisplayingContents[i];
                if (content.DockHandler.IsDockStateValid(value))
                {
                    firstContent = content;
                    break;
                }
            }
            if (firstContent is null) return null;
            firstContent.DockHandler.DockState = value;
            DockPane pane = firstContent.DockHandler.Pane;
            DockPanel.SuspendLayout(true);
            for (int i=0; i<DisplayingContents.Count; i++)
            {
                IDockContent content = DisplayingContents[i];
                if (content.DockHandler.IsDockStateValid(value))
                    content.DockHandler.Pane = pane;
            }
            DockPanel.ResumeLayout(true, true);
            return pane;
        }

        void InternalSetDockState(DockState value)
        {
            if (m_dockState == value)
                return;

            DockState oldDockState = m_dockState;
            INestedPanesContainer oldContainer = NestedPanesContainer;

            m_dockState = value;

            SuspendRefreshStateChange();

            IDockContent contentFocused = GetFocusedContent();
            if (contentFocused != null)
                DockPanel.SaveFocus();

            if (!IsFloat)
                DockWindow = DockPanel.DockWindows[DockState];
            else if (FloatWindow is null)
                FloatWindow = DockPanel.FloatWindowFactory.CreateFloatWindow(DockPanel, this);

            if (contentFocused != null)
            {
                if (NativeMethods.ShouldUseWin32()) contentFocused.DockHandler.Activate();
            }

            ResumeRefreshStateChange(oldContainer, oldDockState);
        }

        int m_countRefreshStateChange;

        void SuspendRefreshStateChange()
        {
            m_countRefreshStateChange ++;
            DockPanel.SuspendLayout(true);
        }

        void ResumeRefreshStateChange()
        {
            m_countRefreshStateChange --;
            #if DEBUG
            if (m_countRefreshStateChange < 0) throw new InvalidOperationException();
            #endif
            if (m_countRefreshStateChange < 0) m_countRefreshStateChange = 0;
            DockPanel.ResumeLayout(true, true);
        }

        bool IsRefreshStateChangeSuspended => m_countRefreshStateChange != 0;

        void ResumeRefreshStateChange(INestedPanesContainer oldContainer, DockState oldDockState)
        {
            ResumeRefreshStateChange();
            RefreshStateChange(oldContainer, oldDockState);
        }

        void RefreshStateChange(INestedPanesContainer oldContainer, DockState oldDockState)
        {
            lock (this)
            {
                if (IsRefreshStateChangeSuspended)
                    return;

                SuspendRefreshStateChange();
            }

            DockPanel.SuspendLayout(true);

            var contentFocused = GetFocusedContent();
            if (contentFocused != null) DockPanel.SaveFocus();
            SetParent();

            ActiveContent?.DockHandler.SetDockState(ActiveContent.DockHandler.IsHidden, DockState, ActiveContent.DockHandler.Pane);
            foreach (IDockContent content in Contents)
            {
                if (content.DockHandler.Pane == this)
                    content.DockHandler.SetDockState(content.DockHandler.IsHidden, DockState, content.DockHandler.Pane);
            }

            if (oldContainer != null)
            {
                Control oldContainerControl = (Control)oldContainer;
                if (oldContainer.DockState == oldDockState && !oldContainerControl.IsDisposed)
                    oldContainerControl.PerformLayout();
            }
            if (DockHelper.IsDockStateAutoHide(oldDockState))
                DockPanel.RefreshActiveAutoHideContent();

            if (NestedPanesContainer.DockState == DockState)
                ((Control)NestedPanesContainer).PerformLayout();
            if (DockHelper.IsDockStateAutoHide(DockState))
                DockPanel.RefreshActiveAutoHideContent();

            if (DockHelper.IsDockStateAutoHide(oldDockState) ||
                DockHelper.IsDockStateAutoHide(DockState))
            {
                DockPanel.RefreshAutoHideStrip();
                DockPanel.PerformLayout();
            }

            ResumeRefreshStateChange();

            contentFocused?.DockHandler.Activate();

            DockPanel.ResumeLayout(true, true);

            if (oldDockState != DockState)
                OnDockStateChanged(EventArgs.Empty);
        }

        IDockContent GetFocusedContent()
        {
            IDockContent contentFocused = null;
            foreach (IDockContent content in Contents)
            {
                if (content.DockHandler.Form.ContainsFocus)
                {
                    contentFocused = content;
                    break;
                }
            }

            return contentFocused;
        }

        public DockPane DockTo(INestedPanesContainer container)
        {
            if (container is null)
                throw new InvalidOperationException(Strings.DockPane_DockTo_NullContainer);

            DockAlignment alignment;
            if (container.DockState == DockState.DockLeft || container.DockState == DockState.DockRight)
                alignment = DockAlignment.Bottom;
            else
                alignment = DockAlignment.Right;

            return DockTo(container, container.NestedPanes.GetDefaultPreviousPane(this), alignment, 0.5);
        }

        public DockPane DockTo(INestedPanesContainer container, DockPane previousPane, DockAlignment alignment, double proportion)
        {
            if (container is null) throw new InvalidOperationException(Strings.DockPane_DockTo_NullContainer);
            if (container.IsFloat == IsFloat)
            {
                InternalAddToDockList(container, previousPane, alignment, proportion);
                return this;
            }
            var firstContent = GetFirstContent(container.DockState);
            if (firstContent is null) return null;
            DockPanel.DummyContent.DockPanel = DockPanel;
            var pane = container.IsFloat
                ? DockPanel.DockPaneFactory.CreateDockPane(DockPanel.DummyContent, (FloatWindow)container, true)
                : DockPanel.DockPaneFactory.CreateDockPane(DockPanel.DummyContent, container.DockState, true);
            pane.DockTo(container, previousPane, alignment, proportion);
            SetVisibleContentsToPane(pane);
            DockPanel.DummyContent.DockPanel = null;
            return pane;
        }

        void SetVisibleContentsToPane(DockPane pane) => SetVisibleContentsToPane(pane, ActiveContent);

        void SetVisibleContentsToPane(DockPane pane, IDockContent activeContent)
        {
            for (int i=0; i<DisplayingContents.Count; i++)
            {
                IDockContent content = DisplayingContents[i];
                if (content.DockHandler.IsDockStateValid(pane.DockState))
                {
                    content.DockHandler.Pane = pane;
                    i--;
                }
            }

            if (activeContent.DockHandler.Pane == pane)
                pane.ActiveContent = activeContent;
        }

        void InternalAddToDockList(INestedPanesContainer container, DockPane prevPane, DockAlignment alignment, double proportion)
        {
            if ((container.DockState == DockState.Float) != IsFloat)
                throw new InvalidOperationException(Strings.DockPane_DockTo_InvalidContainer);

            int count = container.NestedPanes.Count;
            if (container.NestedPanes.Contains(this))
                count --;
            if (prevPane is null && count > 0)
                throw new InvalidOperationException(Strings.DockPane_DockTo_NullPrevPane);

            if (prevPane != null && !container.NestedPanes.Contains(prevPane))
                throw new InvalidOperationException(Strings.DockPane_DockTo_NoPrevPane);

            if (prevPane == this)
                throw new InvalidOperationException(Strings.DockPane_DockTo_SelfPrevPane);

            var oldContainer = NestedPanesContainer;
            var oldDockState = DockState;
            container.NestedPanes.Add(this);
            NestedDockingStatus.SetStatus(container.NestedPanes, prevPane, alignment, proportion);
            if (DockHelper.IsDockWindowState(DockState)) m_dockState = container.DockState;
            RefreshStateChange(oldContainer, oldDockState);
        }

        public void SetNestedDockingProportion(double proportion)
        {
            NestedDockingStatus.SetStatus(NestedDockingStatus.NestedPanes, NestedDockingStatus.PreviousPane, NestedDockingStatus.Alignment, proportion);
            ((Control) NestedPanesContainer)?.PerformLayout();
        }

        public DockPane Float()
        {
            DockPanel.SuspendLayout(true);
            var activeContent = ActiveContent;
            var floatPane = GetFloatPaneFromContents();
            if (floatPane is null)
            {
                var firstContent = GetFirstContent(DockState.Float);
                if (firstContent is null)
                {
                    DockPanel.ResumeLayout(true, true);
                    return null;
                }
                floatPane = DockPanel.DockPaneFactory.CreateDockPane(firstContent,DockState.Float, true);
            }
            SetVisibleContentsToPane(floatPane, activeContent);

            DockPanel.ResumeLayout(true, true);
            return floatPane;
        }

        DockPane GetFloatPaneFromContents()
        {
            DockPane floatPane = null;
            for (int i=0; i<DisplayingContents.Count; i++)
            {
                IDockContent content = DisplayingContents[i];
                if (!content.DockHandler.IsDockStateValid(DockState.Float))
                    continue;

                if (floatPane != null && content.DockHandler.FloatPane != floatPane)
                    return null;
                floatPane = content.DockHandler.FloatPane;
            }

            return floatPane;
        }

        IDockContent GetFirstContent(DockState dockState)
        {
            for (int i=0; i<DisplayingContents.Count; i++)
            {
                IDockContent content = DisplayingContents[i];
                if (content.DockHandler.IsDockStateValid(dockState))
                    return content;
            }
            return null;
        }

        public void RestoreToPanel()
        {
            DockPanel.SuspendLayout(true);

            for (int i=DisplayingContents.Count-1; i>=0; i--)
            {
                IDockContent content = DisplayingContents[i];
                if (content.DockHandler.CheckDockState(false) != DockState.Unknown)
                    content.DockHandler.IsFloat = false;
            }

            DockPanel.ResumeLayout(true, true);
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == (int)Win32.Msgs.WM_MOUSEACTIVATE) Activate();
            base.WndProc(ref m);
        }

        #region IDockDragSource Members

        #region IDragSource Members

        Control IDragSource.DragControl => this;

        #endregion

        bool IDockDragSource.IsDockStateValid(DockState dockState) => IsDockStateValid(dockState);

        bool IDockDragSource.CanDockTo(DockPane pane) => IsDockStateValid(pane.DockState) && pane != this;

        Rectangle IDockDragSource.BeginDrag(Point ptMouse)
        {
            var location = PointToScreen(new Point(0, 0));
            Size size;
            var floatPane = ActiveContent.DockHandler.FloatPane;
            if (DockState == DockState.Float || floatPane is null || floatPane.FloatWindow.NestedPanes.Count != 1)
                size = DockPanel.DefaultFloatWindowSize;
            else
                size = floatPane.FloatWindow.Size;

            if (ptMouse.X > location.X + size.Width)
                location.X += ptMouse.X - (location.X + size.Width) + Measures.SplitterSize;

            return new Rectangle(location, size);
        }

        public void FloatAt(Rectangle floatWindowBounds)
        {
            if (FloatWindow is null || FloatWindow.NestedPanes.Count != 1)
                FloatWindow = DockPanel.FloatWindowFactory.CreateFloatWindow(DockPanel, this, floatWindowBounds);
            else
                FloatWindow.Bounds = floatWindowBounds;

            DockState = DockState.Float;
        }

        public void DockTo(DockPane pane, DockStyle dockStyle, int contentIndex)
        {
            if (dockStyle == DockStyle.Fill)
            {
                IDockContent activeContent = ActiveContent;
                for (int i = Contents.Count - 1; i >= 0; i--)
                {
                    IDockContent c = Contents[i];
                    c.DockHandler.Pane = pane;
                    if (contentIndex != -1)
                        pane.SetContentIndex(c, contentIndex);
                }
                pane.ActiveContent = activeContent;
            }
            else
            {
                if (dockStyle == DockStyle.Left)
                    DockTo(pane.NestedPanesContainer, pane, DockAlignment.Left, 0.5);
                else if (dockStyle == DockStyle.Right)
                    DockTo(pane.NestedPanesContainer, pane, DockAlignment.Right, 0.5);
                else if (dockStyle == DockStyle.Top)
                    DockTo(pane.NestedPanesContainer, pane, DockAlignment.Top, 0.5);
                else if (dockStyle == DockStyle.Bottom)
                    DockTo(pane.NestedPanesContainer, pane, DockAlignment.Bottom, 0.5);

                DockState = pane.DockState;
            }
        }

        public void DockTo(DockPanel panel, DockStyle dockStyle)
        {
            if (panel != DockPanel) throw new ArgumentException(Strings.IDockDragSource_DockTo_InvalidPanel, nameof(panel));
            DockState = dockStyle switch
            {
                DockStyle.Top => DockState.DockTop,
                DockStyle.Bottom => DockState.DockBottom,
                DockStyle.Left => DockState.DockLeft,
                DockStyle.Right => DockState.DockRight,
                DockStyle.Fill => DockState.Document,
                _ => DockState
            };
        }

        #endregion
    }
}
