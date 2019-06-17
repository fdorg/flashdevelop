using PluginCore.Helpers;
using PluginCore.Managers;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace PluginCore.Controls
{
    /// <summary>
    /// A custom scrollbar control, modified for FD:
    /// http://www.codeproject.com/Articles/41869/Custom-Drawn-Scrollbar
    /// </summary>
    [Designer(typeof(ScrollBarDesigner))]
    [DefaultEvent("Scroll")]
    [DefaultProperty("Value")]
    public class ScrollBarEx : Control
    {
        #region Static

        /// <summary>
        /// Attaches the custom scrollbars to the specified controls, requires restart.
        /// </summary>
        public static IDisposable Attach(object obj)
        {
            return ScrollBarEx.Attach(obj, false);
        }
        public static IDisposable Attach(object obj, bool childrenToo)
        {
            if (!Win32.ShouldUseWin32() && !PluginBase.MainForm.GetThemeFlag("ScrollBar.UseGlobally", false)) return null;
            else if (obj is Control && childrenToo)
            {
                Control parent = obj as Control;
                foreach (Control ctrl in parent.Controls) ScrollBarEx.Attach(ctrl);
                return null;
            }
            else if (obj is ListBox) return new ListBoxScroller(obj as ListBox);
            else if (obj is ListView) return new ListViewScroller(obj as ListView);
            else if (obj is TreeView) return new TreeViewScroller(obj as TreeView);
            else if (obj is RichTextBox) return new RichTextBoxScroller(obj as RichTextBox);
            else if (obj is DataGridView) return new DataGridViewScroller(obj as DataGridView);
            else if (obj is PropertyGrid) return new PropertyGridScroller(obj as PropertyGrid);
            else if (obj is TextBox && (obj as TextBox).Multiline && (obj as TextBox).WordWrap)
            {
                return new TextBoxScroller(obj as TextBoxEx);
            }
            else return null;
        }

        /// <summary>
        /// Resizes based on display scale. If the result is an even number, rounds to the nearest odd number further away from zero than value.
        /// </summary>
        public static int ScaleOddUp(int value)
        {
            int result = ScaleHelper.Scale(value);
            return (result % 2 == 1) ? result : (result + Math.Sign(value));
        }

        #endregion

        #region Tunables

        /// <summary>
        /// Auto-repeat delay.
        /// </summary>
        private const int PROGRESS_TIMER_DELAY = 300;

        /// <summary>
        /// Auto-repeat interval.
        /// </summary>
        private const int PROGRESS_TIMER_TICK = 33;
        #endregion

        #region Drawing

        private Color curPosColor = Color.DarkBlue;
        private Color foreColor = SystemColors.ControlDarkDark;
        private Color foreColorHot = SystemColors.Highlight;
        private Color foreColorPressed = SystemColors.HotTrack;
        private Color arrowColor = SystemColors.ControlDarkDark;
        private Color arrowColorHot = SystemColors.Highlight;
        private Color arrowColorPressed = SystemColors.HotTrack;
        private Color backColor = SystemColors.ActiveBorder;
        private Color backColorDisabled = SystemColors.ControlLight;
        private Color borderColor = SystemColors.ActiveBorder;
        private Color borderColorDisabled = SystemColors.Control;

        /// <summary>
        /// Draws the background.
        /// </summary>
        /// <param name="g">The <see cref="Graphics"/> used to paint.</param>
        /// <param name="rect">The rectangle in which to paint.</param>
        /// <param name="orientation">The <see cref="ScrollBarOrientation"/>.</param>
        private void DrawBackground(Graphics g, Rectangle rect, ScrollBarOrientation orientation)
        {
            if (g == null) throw new ArgumentNullException("g");

            if (rect.IsEmpty || g.IsVisibleClipEmpty || !g.VisibleClipBounds.IntersectsWith(rect))
                return;

            switch (orientation)
            {
                case ScrollBarOrientation.Vertical:
                    DrawBackgroundVertical(g, rect);
                    break;
                default:
                    DrawBackgroundHorizontal(g, rect);
                    break;
            }
        }

        /// <summary>
        /// Draws the thumb.
        /// </summary>
        /// <param name="g">The <see cref="Graphics"/> used to paint.</param>
        /// <param name="rect">The rectangle in which to paint.</param>
        /// <param name="state">The <see cref="ScrollBarState"/> of the thumb.</param>
        /// <param name="orientation">The <see cref="ScrollBarOrientation"/>.</param>
        private void DrawThumb(Graphics g, Rectangle rect, ScrollBarState state, ScrollBarOrientation orientation)
        {
            if (g == null) throw new ArgumentNullException("g");

            if (rect.IsEmpty || g.IsVisibleClipEmpty || !g.VisibleClipBounds.IntersectsWith(rect) || state == ScrollBarState.Disabled)
                return;

            Color color;
            switch (state)
            {
                case ScrollBarState.Hot:
                    color = foreColorHot;
                    break;
                case ScrollBarState.Pressed:
                    color = foreColorPressed;
                    break;
                default:
                    color = foreColor;
                    break;
            }

            switch (orientation)
            {
                case ScrollBarOrientation.Vertical:
                    DrawThumbVertical(g, rect, color);
                    break;
                default:
                    DrawThumbHorizontal(g, rect, color);
                    break;
            }
        }

        /// <summary>
        /// Draws an arrow button.
        /// </summary>
        /// <param name="g">The <see cref="Graphics"/> used to paint.</param>
        /// <param name="rect">The rectangle in which to paint.</param>
        /// <param name="state">The <see cref="ScrollBarArrowButtonState"/> of the arrow button.</param>
        /// <param name="arrowUp">true for an up arrow, false otherwise.</param>
        /// <param name="orientation">The <see cref="ScrollBarOrientation"/>.</param>
        private void DrawArrowButton(Graphics g, Rectangle rect, ScrollBarArrowButtonState state, bool arrowUp, ScrollBarOrientation orientation)
        {
            if (g == null) throw new ArgumentNullException("g");

            if (rect.IsEmpty || g.IsVisibleClipEmpty || !g.VisibleClipBounds.IntersectsWith(rect))
                return;

            Color color;

            switch (state)
            {
                case ScrollBarArrowButtonState.UpHot:
                case ScrollBarArrowButtonState.DownHot:
                    color = arrowColorHot;
                    break;
                case ScrollBarArrowButtonState.UpPressed:
                case ScrollBarArrowButtonState.DownPressed:
                    color = arrowColorPressed;
                    break;
                default:
                    color = arrowColor;
                    break;
            }

            switch (orientation)
            {
                case ScrollBarOrientation.Vertical:
                    DrawArrowButtonVertical(g, rect, color, arrowUp);
                    break;
                default:
                    DrawArrowButtonHorizontal(g, rect, color, arrowUp);
                    break;
            }
        }

        /// <summary>
        /// Draws the background.
        /// </summary>
        /// <param name="g">The <see cref="Graphics"/> used to paint.</param>
        /// <param name="rect">The rectangle in which to paint.</param>
        private void DrawBackgroundVertical(Graphics g, Rectangle rect)
        {
            using (Brush brush = new SolidBrush(this.Enabled ? backColor : backColorDisabled))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                g.FillRectangle(brush, rect);
            }
        }

        /// <summary>
        /// Draws the background.
        /// </summary>
        /// <param name="g">The <see cref="Graphics"/> used to paint.</param>
        /// <param name="rect">The rectangle in which to paint.</param>
        private void DrawBackgroundHorizontal(Graphics g, Rectangle rect)
        {
            using (Brush brush = new SolidBrush(this.Enabled ? backColor : backColorDisabled))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                g.FillRectangle(brush, rect);
            }
        }

        /// <summary>
        /// Draws the vertical thumb.
        /// </summary>
        /// <param name="g">The <see cref="Graphics"/> used to paint.</param>
        /// <param name="rect">The rectangle in which to paint.</param>
        /// <param name="color">The color to draw the thumb with.</param>
        private static void DrawThumbVertical(Graphics g, Rectangle rect, Color color)
        {
            using (Brush brush = new SolidBrush(color))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                g.FillRectangle(brush, rect);
            }
        }

        /// <summary>
        /// Draws the horizontal thumb.
        /// </summary>
        /// <param name="g">The <see cref="Graphics"/> used to paint.</param>
        /// <param name="rect">The rectangle in which to paint.</param>
        /// <param name="color">The color to draw the thumb with.</param>
        private static void DrawThumbHorizontal(Graphics g, Rectangle rect, Color color)
        {
            using (Brush brush = new SolidBrush(color))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                g.FillRectangle(brush, rect);
            }
        }

        /// <summary>
        /// Draws arrow buttons for vertical scroll bar.
        /// </summary>
        /// <param name="g">The <see cref="Graphics"/> used to paint.</param>
        /// <param name="rect">The rectangle in which to paint.</param>
        /// <param name="color">The color to draw the arrow buttons with.</param>
        /// <param name="arrowUp">true for an up arrow, false otherwise.</param>
        private static void DrawArrowButtonVertical(Graphics g, Rectangle rect, Color color, bool arrowUp)
        {
            using (Brush brush = new SolidBrush(color))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                // When using anti-aliased drawing mode, a point has zero size and lies in the center of a pixel. To align with the pixel grid, use coordinates that are integers + 0.5f.
                PointF headPoint = new PointF(rect.Left + rect.Width / 2, (arrowUp ? rect.Top : rect.Bottom) - 0.5f);
                float baseY = (arrowUp ? rect.Bottom : rect.Top) - 0.5f;
                g.FillPolygon(brush, new PointF[]
                    {
                        new PointF(rect.Left - 0.5f, baseY),
                        new PointF(rect.Right - 0.5f, baseY),
                        headPoint
                    });

            }
        }

        /// <summary>
        /// Draws arrow buttons for horizontal scroll bar.
        /// </summary>
        /// <param name="g">The <see cref="Graphics"/> used to paint.</param>
        /// <param name="rect">The rectangle in which to paint.</param>
        /// <param name="color">The color to draw the arrow buttons with.</param>
        /// <param name="arrowLeft">true for a left arrow, false otherwise.</param>
        private static void DrawArrowButtonHorizontal(Graphics g, Rectangle rect, Color color, bool arrowLeft)
        {
            using (Brush brush = new SolidBrush(color))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                // When using anti-aliased drawing mode, a point has zero size and lies in the center of a pixel. To align with the pixel grid, use coordinates that are integers + 0.5f.
                PointF headPoint = new PointF((arrowLeft ? rect.Left : rect.Right) - 0.5f, rect.Top + rect.Height / 2);
                float baseX = (arrowLeft ? rect.Right : rect.Left) - 0.5f;
                g.FillPolygon(brush, new PointF[]
                    {
                        new PointF(baseX, rect.Top - 0.5f),
                        new PointF(baseX, rect.Bottom - 0.5f),
                        headPoint
                    });
            }
        }

        #endregion

        #region Fields

        /// <summary>
        /// Indicates many changes to the scrollbar are happening, so stop painting till finished.
        /// </summary>
        private bool inUpdate;

        /// <summary>
        /// Highlights the current line if: value > -1
        /// </summary>
        private int curPos = -1;

        /// <summary>
        /// The maximum value of curPos. If overScroll (EndAtLastLine) is disabled, it is greater than the maximum scrollbar value, otherwise they are equal.
        /// </summary>
        private int maxCurPos = 100;

        /// <summary>
        /// The scrollbar orientation - horizontal / vertical.
        /// </summary>
        private ScrollBarOrientation orientation = ScrollBarOrientation.Vertical;

        /// <summary>
        /// The scroll orientation in scroll events.
        /// </summary>
        private ScrollOrientation scrollOrientation = ScrollOrientation.VerticalScroll;

        /// <summary>
        /// The thumb rectangle.
        /// </summary>
        private Rectangle thumbRectangle;

        /// <summary>
        /// The top arrow rectangle.
        /// </summary>
        private Rectangle topArrowRectangle;

        /// <summary>
        /// The bottom arrow rectangle.
        /// </summary>
        private Rectangle bottomArrowRectangle;

        /// <summary>
        /// Indicates if top arrow was clicked.
        /// </summary>
        private bool topArrowClicked;

        /// <summary>
        /// Indicates if bottom arrow was clicked.
        /// </summary>
        private bool bottomArrowClicked;

        /// <summary>
        /// Indicates if channel rectangle above the thumb was clicked.
        /// </summary>
        private bool topBarClicked;

        /// <summary>
        /// Indicates if channel rectangle under the thumb was clicked.
        /// </summary>
        private bool bottomBarClicked;

        /// <summary>
        /// Indicates if the thumb was clicked.
        /// </summary>
        private bool thumbClicked;

        /// <summary>
        /// The state of the thumb.
        /// </summary>
        private ScrollBarState thumbState = ScrollBarState.Normal;

        /// <summary>
        /// The state of the top arrow.
        /// </summary>
        private ScrollBarArrowButtonState topButtonState = ScrollBarArrowButtonState.UpNormal;

        /// <summary>
        /// The state of the bottom arrow.
        /// </summary>
        private ScrollBarArrowButtonState bottomButtonState = ScrollBarArrowButtonState.DownNormal;

        /// <summary>
        /// The scrollbar value minimum.
        /// </summary>
        private int minimum;

        /// <summary>
        /// The scrollbar value maximum.
        /// </summary>
        private int maximum = 100;

        /// <summary>
        /// The view port size (page size) in relation to the maximum and minimum value.
        /// </summary>
        private int viewPortSize = 100;

        /// <summary>
        /// The small change value.
        /// </summary>
        private int smallChange = 1;

        /// <summary>
        /// The large change value.
        /// </summary>
        private int largeChange = 10;

        /// <summary>
        /// The value of the scrollbar.
        /// </summary>
        private int value;

        /// <summary>
        /// The thickness of the thumb.
        /// </summary>
        private const int THUMB_THICKNESS = 9;
        private int thumbThickness;

        /// <summary>
        /// The thickness of an arrow (base length).
        /// </summary>
        private const int ARROW_THICKNESS = THUMB_THICKNESS;
        private int arrowThickness;

        /// <summary>
        /// The length of an arrow (point-to-base distance).
        /// </summary>
        private const int ARROW_LENGTH = 5;
        private int arrowLength;

        /// <summary>
        /// The padding between an arrow's point and the nearest edge.
        /// </summary>
        private const int ARROW_PADDING = 6;
        private int arrowPadding;

        /// <summary>
        /// The gap between an arrow and the thumb.
        /// </summary>
        private const int ARROW_THUMB_GAP = ARROW_PADDING;
        private int arrowThumbGap;

        /// <summary>
        /// The length of an arrow, arrow padding and arrow-thumb gap
        /// </summary>
        private int arrowPaddedLength;

        /// <summary>
        /// The bottom limit for the thumb bottom.
        /// </summary>
        private int thumbBottomLimitBottom;

        /// <summary>
        /// The bottom limit for the thumb top.
        /// </summary>
        private int thumbBottomLimitTop;

        /// <summary>
        /// The top limit for the thumb top.
        /// </summary>
        private int thumbTopLimit;

        /// <summary>
        /// The current position of the thumb.
        /// </summary>
        private int thumbPosition;

        /// <summary>
        /// The track position.
        /// </summary>
        private int trackPosition;

        /// <summary>
        /// The progress timer for moving the thumb.
        /// </summary>
        private Timer progressTimer = new Timer();

        /// <summary>
        /// Context menu strip.
        /// </summary>
        private ContextMenuStrip contextMenu;

        /// <summary>
        /// Container for components.
        /// </summary>
        private IContainer components;

        /// <summary>
        /// Menu item.
        /// </summary>
        private ToolStripMenuItem tsmiScrollHere;

        /// <summary>
        /// Menu separator.
        /// </summary>
        private ToolStripSeparator toolStripSeparator1;

        /// <summary>
        /// Menu item.
        /// </summary>
        private ToolStripMenuItem tsmiTop;

        /// <summary>
        /// Menu item.
        /// </summary>
        private ToolStripMenuItem tsmiBottom;

        /// <summary>
        /// Menu separator.
        /// </summary>
        private ToolStripSeparator toolStripSeparator2;

        /// <summary>
        /// Menu item.
        /// </summary>
        private ToolStripMenuItem tsmiLargeUp;

        /// <summary>
        /// Menu item.
        /// </summary>
        private ToolStripMenuItem tsmiLargeDown;

        /// <summary>
        /// Menu separator.
        /// </summary>
        private ToolStripSeparator toolStripSeparator3;

        /// <summary>
        /// Menu item.
        /// </summary>
        private ToolStripMenuItem tsmiSmallUp;

        /// <summary>
        /// Menu item.
        /// </summary>
        private ToolStripMenuItem tsmiSmallDown;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ScrollBarEx"/> class.
        /// </summary>
        public ScrollBarEx()
        {
            // sets the control styles of the control
            SetStyle(ControlStyles.OptimizedDoubleBuffer 
                | ControlStyles.ResizeRedraw 
                | ControlStyles.Selectable 
                | ControlStyles.AllPaintingInWmPaint 
                | ControlStyles.UserPaint, true);
            // initializes the context menu
            this.InitializeComponent();
            this.Width = ScaleOddUp(17);
            this.Height = ScaleHelper.Scale(200);
            // sets the scrollbar up
            this.SetUpScrollBar();
            // timer for clicking and holding the mouse buttonь
            // over/below the thumb and on the arrow buttons
            this.progressTimer.Interval = PROGRESS_TIMER_TICK;
            this.progressTimer.Tick += this.ProgressTimerTick;
            this.ContextMenuStrip = this.contextMenu;
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when the scrollbar scrolled.
        /// </summary>
        [Category("Behavior")]
        [Description("Is raised, when the scrollbar was scrolled.")]
        public event ScrollEventHandler Scroll;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the current position.
        /// </summary>
        [Category("Behavior")]
        [Description("Gets or sets the current position.")]
        [DefaultValue(-1)]
        public int CurrentPosition
        {
            get
            {
                return this.curPos;
            }
            set
            {
                // no change, return
                if (this.curPos == value)
                {
                    return;
                }
                this.curPos = value;
                this.Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the maximum value of current position.
        /// </summary>
        [Category("Behavior")]
        [Description("Gets or sets the maximum value of current position.")]
        [DefaultValue(-1)]
        public int MaxCurrentPosition
        {
            get
            {
                return this.maxCurPos;
            }
            set
            {
                // no change, return
                if (this.maxCurPos == value)
                {
                    return;
                }
                this.maxCurPos = value;
                this.Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the orientation.
        /// </summary>
        [Category("Layout")]
        [Description("Gets or sets the orientation.")]
        [DefaultValue(ScrollBarOrientation.Vertical)]
        public ScrollBarOrientation Orientation
        {
            get
            {
                return this.orientation;
            }
            set
            {
                // no change - return
                if (value == this.orientation)
                {
                    return;
                }
                this.orientation = value;
                // change text of context menu entries
                this.ChangeContextMenuItems();
                // save scroll orientation for scroll event
                this.scrollOrientation = value == ScrollBarOrientation.Vertical ? ScrollOrientation.VerticalScroll : ScrollOrientation.HorizontalScroll;
                // only in DesignMode switch width and height
                if (this.DesignMode)
                {
                    this.Size = new Size(this.Height, this.Width);
                }
                // sets the scrollbar up
                this.SetUpScrollBar();
            }
        }

        /// <summary>
        /// Gets or sets the minimum value.
        /// </summary>
        [Category("Behavior")]
        [Description("Gets or sets the minimum value.")]
        [DefaultValue(0)]
        public int Minimum
        {
            get
            {
                return this.minimum;
            }
            set
            {
                // no change or value invalid - return
                if (this.minimum == value || value < 0 || value >= this.maximum)
                {
                    return;
                }
                this.minimum = value;
                // current value less than new minimum value - adjust
                if (this.value < value)
                {
                    this.value = value;
                }
                // is current large change value invalid - adjust
                if (this.largeChange > this.maximum - this.minimum)
                {
                    this.largeChange = this.maximum - this.minimum;
                }
                this.SetUpScrollBar();
                // current value less than new minimum value - adjust
                if (this.value < value)
                {
                    this.Value = value;
                }
                else
                {
                    // current value is valid - adjust thumb position (already done by SetUpScrollBar())
                    this.Refresh();
                }
            }
        }

        /// <summary>
        /// Gets or sets the maximum value.
        /// </summary>
        [Category("Behavior")]
        [Description("Gets or sets the maximum value.")]
        [DefaultValue(100)]
        public int Maximum
        {
            get
            {
                return this.maximum;
            }
            set
            {
                // no change or new max. value invalid - return
                if (value == this.maximum || value < 1 || value <= this.minimum)
                {
                    return;
                }
                this.maximum = value;
                // is large change value invalid - adjust
                if (this.largeChange > this.maximum - this.minimum)
                {
                    this.largeChange = this.maximum - this.minimum;
                }
                this.SetUpScrollBar();
                // is current value greater than new maximum value - adjust
                if (this.value > value)
                {
                    this.Value = this.maximum;
                }
                else
                {
                    // current value is valid - adjust thumb position (already done by SetUpScrollBar())
                    this.Refresh();
                }
            }
        }

        /// <summary>
        /// Gets or sets the viewPortSize value.
        /// </summary>
        [Category("Behavior")]
        [Description("Gets or sets the viewPortSize value.")]
        [DefaultValue(10)]
        public int ViewPortSize
        {
            get
            {
                return this.viewPortSize;
            }
            set
            {
                // no change or new value invalid - return
                if (value == this.viewPortSize || viewPortSize < 1)
                {
                    return;
                }
                this.viewPortSize = value;
                this.SetUpScrollBar();
                // adjust thumb position (already done by SetUpScrollBar())
                this.Refresh();
            }
        }

        /// <summary>
        /// Gets or sets the small change amount.
        /// </summary>
        [Category("Behavior")]
        [Description("Gets or sets the small change value.")]
        [DefaultValue(1)]
        public int SmallChange
        {
            get
            {
                return this.smallChange;
            }
            set
            {
                // no change or new small change value invalid - return
                if (value == this.smallChange || value < 1 || value >= this.largeChange)
                {
                    return;
                }
                this.smallChange = value;
                this.SetUpScrollBar();
            }
        }

        /// <summary>
        /// Gets or sets the large change amount.
        /// </summary>
        [Category("Behavior")]
        [Description("Gets or sets the large change value.")]
        [DefaultValue(10)]
        public int LargeChange
        {
            get
            {
                return this.largeChange;
            }
            set
            {
                // no change or new large change value is invalid - return
                if (value == this.largeChange || value < this.smallChange || value < 2)
                {
                    return;
                }
                // if value is greater than scroll area - adjust
                if (value > this.maximum - this.minimum)
                {
                    this.largeChange = this.maximum - this.minimum;
                }
                else
                {
                    // set new value
                    this.largeChange = value;
                }
                this.SetUpScrollBar();
            }
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        [Category("Behavior")]
        [Description("Gets or sets the current value.")]
        [DefaultValue(0)]
        public int Value
        {
            get
            {
                return this.value;
            }
            set
            {
                // no change or invalid value - return
                if (this.value == value || value < this.minimum || value > this.maximum)
                {
                    return;
                }
                this.value = value;
                // adjust thumb position
                this.ChangeThumbPosition(this.GetThumbPosition());
                // raise scroll event
                this.OnScroll(new ScrollEventArgs(ScrollEventType.ThumbPosition, -1, this.value, this.scrollOrientation));
                this.Refresh();
            }
        }

        /// <summary>
        /// Gets or sets the border color.
        /// </summary>
        [Category("Appearance")]
        [Description("Gets or sets the border color.")]
        [DefaultValue(typeof(SystemColors), "ActiveBorder")]
        public Color BorderColor
        {
            get
            {
                return this.borderColor;
            }
            set
            {
                this.borderColor = value;
                this.Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the border color in disabled state.
        /// </summary>
        [Category("Appearance")]
        [Description("Gets or sets the border color in disabled state.")]
        [DefaultValue(typeof(SystemColors), "InactiveBorder")]
        public Color DisabledBorderColor
        {
            get
            {
                return this.borderColorDisabled;
            }
            set
            {
                this.borderColorDisabled = value;
                this.Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the back color.
        /// </summary>
        [Category("Appearance")]
        [Description("Gets or sets the border color.")]
        [DefaultValue(typeof(SystemColors), "Control")]
        public override Color BackColor
        {
            get
            {
                return this.backColor;
            }
            set
            {
                this.backColor = value;
                this.Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the back color.
        /// </summary>
        [Category("Appearance")]
        [Description("Gets or sets the disabled back color.")]
        [DefaultValue(typeof(SystemColors), "ControlLight")]
        public Color DisabledBackColor
        {
            get
            {
                return this.backColorDisabled;
            }
            set
            {
                this.backColorDisabled = value;
                this.Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the fore color.
        /// </summary>
        [Category("Appearance")]
        [Description("Gets or sets the foreground color on idle.")]
        [DefaultValue(typeof(SystemColors), "ScrollBar")]
        public override Color ForeColor
        {
            get
            {
                return this.foreColor;
            }
            set
            {
                this.foreColor = value;
                this.Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the hot fore color.
        /// </summary>
        [Category("Appearance")]
        [Description("Gets or sets the foreground color on hover.")]
        [DefaultValue(typeof(SystemColors), "ControlDark")]
        public Color HotForeColor
        {
            get { return this.foreColorHot; }
            set
            {
                this.foreColorHot = value;
                this.Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the pressed fore color.
        /// </summary>
        [Category("Appearance")]
        [Description("Gets or sets the foreground color when active.")]
        [DefaultValue(typeof(SystemColors), "ControlDarkDark")]
        public Color ActiveForeColor
        {
            get
            {
                return this.foreColorPressed;
            }
            set
            {
                this.foreColorPressed = value;
                this.Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the arrow color.
        /// </summary>
        [Category("Appearance")]
        [Description("Gets or sets the arrow color on idle.")]
        [DefaultValue(typeof(SystemColors), "ControlDark")]
        public Color ArrowColor
        {
            get { return this.arrowColor; }
            set
            {
                this.arrowColor = value;
                this.Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the hot arrow color.
        /// </summary>
        [Category("Appearance")]
        [Description("Gets or sets the arrow color on hover.")]
        [DefaultValue(typeof(SystemColors), "Highlight")]
        public Color HotArrowColor
        {
            get { return this.arrowColorHot; }
            set
            {
                this.arrowColorHot = value;
                this.Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the pressed arrow color.
        /// </summary>
        [Category("Appearance")]
        [Description("Gets or sets the arrow color when active.")]
        [DefaultValue(typeof(SystemColors), "HotTrack")]
        public Color ActiveArrowColor
        {
            get { return this.arrowColorPressed; }
            set
            {
                this.arrowColorPressed = value;
                this.Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the current position indicator color.
        /// </summary>
        [Category("Appearance")]
        [Description("Gets or sets the current position indicator color.")]
        [DefaultValue(typeof(Color), "DarkBlue")]
        public Color CurrentPositionColor
        {
            get
            {
                return this.curPosColor;
            }
            set
            {
                this.curPosColor = value;
                this.Invalidate();
            }
        }

        /// <summary>
        /// Gets or sets the opacity of the context menu (from 0 - 1).
        /// </summary>
        [Category("Appearance")]
        [Description("Gets or sets the opacity of the context menu (from 0 - 1).")]
        [DefaultValue(typeof(double), "1")]
        public double Opacity
        {
            get
            {
                return this.contextMenu.Opacity;
            }
            set
            {
                // no change - return
                if (value == this.contextMenu.Opacity)
                {
                    return;
                }
                this.contextMenu.AllowTransparency = value != 1;
                this.contextMenu.Opacity = value;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prevents the drawing of the control until <see cref="EndUpdate"/> is called.
        /// </summary>
        public void BeginUpdate()
        {
            Win32.SendMessage(this.Handle, Win32.WM_SETREDRAW, 0, 0);
            this.inUpdate = true;
        }

        /// <summary>
        /// Ends the updating process and the control can draw itself again.
        /// </summary>
        public void EndUpdate()
        {
            Win32.SendMessage(this.Handle, Win32.WM_SETREDRAW, 1, 0);
            this.inUpdate = false;
            this.SetUpScrollBar();
            this.Refresh();
        }

        /// <summary>
        /// Raises the <see cref="Scroll"/> event.
        /// </summary>
        /// <param name="e">The <see cref="ScrollEventArgs"/> that contains the event data.</param>
        protected virtual void OnScroll(ScrollEventArgs e)
        {
            // if event handler is attached - raise scroll event
            Scroll?.Invoke(this, e);
        }

        /// <summary>
        /// Paints the background of the control.
        /// </summary>
        /// <param name="e">A <see cref="PaintEventArgs"/> that contains information about the control to paint.</param>
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // no painting here
        }

        /// <summary>
        /// Paints the control.
        /// </summary>
        /// <param name="e">A <see cref="PaintEventArgs"/> that contains information about the control to paint.</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            // draws the background
            DrawBackground(
               e.Graphics,
               ClientRectangle,
               this.orientation);
            // draw thumb and grip
            DrawThumb(
               e.Graphics,
               this.thumbRectangle,
               this.thumbState,
               this.orientation);

            // draw arrows
            DrawArrowButton(
               e.Graphics,
               this.topArrowRectangle,
               this.topButtonState,
               true,
               this.orientation);
            DrawArrowButton(
               e.Graphics,
               this.bottomArrowRectangle,
               this.bottomButtonState,
               false,
               this.orientation);

            // draw current line
            if (this.curPos > -1 && this.orientation == ScrollBarOrientation.Vertical)
            {
                using (SolidBrush brush = new SolidBrush(this.curPosColor))
                {
                    e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    e.Graphics.FillRectangle(brush, 0, GetCurPosition() - ScaleHelper.Scale(2f) / 2, this.Width, ScaleHelper.Scale(2f));
                }
            }

            // draw border
            using (Pen pen = new Pen((this.Enabled ? this.borderColor : this.borderColorDisabled)))
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
                e.Graphics.DrawRectangle(pen, 0, 0, this.Width - 1, this.Height - 1);
            }
        }

        /// <summary>
        /// Calculates the current position.
        /// </summary>
        /// <returns>The current position.</returns>
        private float GetCurPosition()
        {
            int bottomLimit = (this.maxCurPos > this.maximum) ? thumbBottomLimitBottom : this.thumbBottomLimitTop;
            int pixelRange = bottomLimit - this.thumbTopLimit; // == size - (overScroll ? thumbSize : 0) - arrows - paddings
            int realRange = this.maxCurPos - this.minimum;
            float perc = (realRange != 0) ? ((float)(this.curPos - this.minimum) / (float)realRange) : 0f;
            return Math.Max(this.thumbTopLimit, Math.Min(bottomLimit, perc * pixelRange + this.arrowPaddedLength));
        }

        /// <summary>
        /// Raises the MouseDown event.
        /// </summary>
        /// <param name="e">A <see cref="MouseEventArgs"/> that contains the event data.</param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            //this.Focus();
            if (e.Button == MouseButtons.Left)
            {
                // prevents showing the context menu if pressing the right mouse
                // button while holding the left
                this.ContextMenuStrip = null;
                Point mouseLocation = e.Location;
                if (this.thumbRectangle.Contains(mouseLocation))
                {
                    this.thumbClicked = true;
                    this.thumbPosition = this.orientation == ScrollBarOrientation.Vertical ? mouseLocation.Y - this.thumbRectangle.Y : mouseLocation.X - this.thumbRectangle.X;
                    this.thumbState = ScrollBarState.Pressed;
                    Invalidate(this.thumbRectangle);
                }
                else if (this.topArrowRectangle.Contains(mouseLocation))
                {
                    this.topArrowClicked = true;
                    this.topButtonState = ScrollBarArrowButtonState.UpPressed;
                    this.Invalidate(this.topArrowRectangle);
                    this.ProgressThumb(true);
                }
                else if (this.bottomArrowRectangle.Contains(mouseLocation))
                {
                    this.bottomArrowClicked = true;
                    this.bottomButtonState = ScrollBarArrowButtonState.DownPressed;
                    this.Invalidate(this.bottomArrowRectangle);
                    this.ProgressThumb(true);
                }
                else
                {
                    this.trackPosition = this.orientation == ScrollBarOrientation.Vertical ? mouseLocation.Y : mouseLocation.X;
                    if (this.trackPosition < (this.orientation == ScrollBarOrientation.Vertical ? this.thumbRectangle.Y : this.thumbRectangle.X))
                        this.topBarClicked = true;
                    else
                        this.bottomBarClicked = true;
                    this.ProgressThumb(true);
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                this.trackPosition = this.orientation == ScrollBarOrientation.Vertical ? e.Y : e.X;
            }
        }

        /// <summary>
        /// Raises the MouseUp event.
        /// </summary>
        /// <param name="e">A <see cref="MouseEventArgs"/> that contains the event data.</param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (e.Button == MouseButtons.Left)
            {
                this.ContextMenuStrip = this.contextMenu;
                if (this.thumbClicked)
                {
                    this.thumbClicked = false;
                    this.thumbState = ScrollBarState.Normal;
                    this.OnScroll(new ScrollEventArgs(ScrollEventType.EndScroll, -1, this.value, this.scrollOrientation));
                }
                else if (this.topArrowClicked)
                {
                    this.topArrowClicked = false;
                    this.topButtonState = ScrollBarArrowButtonState.UpNormal;
                    this.StopTimer();
                }
                else if (this.bottomArrowClicked)
                {
                    this.bottomArrowClicked = false;
                    this.bottomButtonState = ScrollBarArrowButtonState.DownNormal;
                    this.StopTimer();
                }
                else if (this.topBarClicked)
                {
                    this.topBarClicked = false;
                    this.StopTimer();
                }
                else if (this.bottomBarClicked)
                {
                    this.bottomBarClicked = false;
                    this.StopTimer();
                }
                Invalidate();
            }
        }

        /// <summary>
        /// Raises the MouseEnter event.
        /// </summary>
        /// <param name="e">A <see cref="EventArgs"/> that contains the event data.</param>
        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            this.bottomButtonState = ScrollBarArrowButtonState.DownActive;
            this.topButtonState = ScrollBarArrowButtonState.UpActive;
            this.thumbState = ScrollBarState.Active;
            Invalidate();
        }

        /// <summary>
        /// Raises the MouseLeave event.
        /// </summary>
        /// <param name="e">A <see cref="EventArgs"/> that contains the event data.</param>
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            this.ResetScrollStatus();
        }

        /// <summary>
        /// Raises the MouseMove event.
        /// </summary>
        /// <param name="e">A <see cref="MouseEventArgs"/> that contains the event data.</param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            // moving and holding the left mouse button
            if (e.Button == MouseButtons.Left)
            {
                // Update the thumb position, if the new location is within the bounds.
                if (this.thumbClicked)
                {
                    int oldThumbLocation = (this.orientation == ScrollBarOrientation.Vertical) ? this.thumbRectangle.Y : this.thumbRectangle.X;
                    int oldScrollValue = this.value;
                    this.topButtonState = ScrollBarArrowButtonState.UpActive;
                    this.bottomButtonState = ScrollBarArrowButtonState.DownActive;
                    int pos = (this.orientation == ScrollBarOrientation.Vertical ? e.Location.Y : e.Location.X) - thumbPosition;
                    // The thumb is all the way to the top
                    if (pos <= this.thumbTopLimit)
                    {
                        this.ChangeThumbPosition(this.thumbTopLimit);
                        this.value = this.minimum;
                    }
                    else if (pos >= this.thumbBottomLimitTop)
                    {
                        // The thumb is all the way to the bottom
                        this.ChangeThumbPosition(this.thumbBottomLimitTop);
                        this.value = this.maximum;
                    }
                    else
                    {
                        // The thumb is between the ends of the track.
                        this.ChangeThumbPosition(pos);
                        int pixelRange = this.thumbBottomLimitTop - this.thumbTopLimit; // == size - thumbSize - arrows - paddings
                        int position = ((this.orientation == ScrollBarOrientation.Vertical) ? this.thumbRectangle.Y : this.thumbRectangle.X) - this.arrowPaddedLength;
                        // percent of the new position
                        float perc = (pixelRange != 0) ? ((float)position / (float)pixelRange) : 0f;
                        // the new value is somewhere between max and min, starting
                        // at min position
                        this.value = Convert.ToInt32((perc * (this.maximum - this.minimum)) + this.minimum);
                    }

                    // raise scroll event if value has changed
                    if (oldScrollValue != this.value)
                    {
                        this.OnScroll(new ScrollEventArgs(ScrollEventType.ThumbTrack, oldScrollValue, this.value, this.scrollOrientation));
                        this.Refresh();
                    }
                    else
                    {
                        int newThumbLocation = (this.orientation == ScrollBarOrientation.Vertical) ? this.thumbRectangle.Y : this.thumbRectangle.X;
                        // repaint if thumb location has changed, but only at the top and bottom, to prevent thumb jumping around
                        if ((oldThumbLocation != newThumbLocation) && ((newThumbLocation == thumbTopLimit) || (newThumbLocation == thumbBottomLimitTop)))
                        {
                            this.Refresh();
                        }
                    }
                }
            }
            else if (!this.ClientRectangle.Contains(e.Location))
            {
                this.ResetScrollStatus();
            }
            else if (e.Button == MouseButtons.None) // only moving the mouse
            {
                if (this.topArrowRectangle.Contains(e.Location))
                {
                    this.topButtonState = ScrollBarArrowButtonState.UpHot;
                    this.Invalidate(this.topArrowRectangle);
                }
                else if (this.bottomArrowRectangle.Contains(e.Location))
                {
                    this.bottomButtonState = ScrollBarArrowButtonState.DownHot;
                    Invalidate(this.bottomArrowRectangle);
                }
                else if (this.thumbRectangle.Contains(e.Location))
                {
                    this.thumbState = ScrollBarState.Hot;
                    this.Invalidate(this.thumbRectangle);
                }
                else if (this.ClientRectangle.Contains(e.Location))
                {
                    this.topButtonState = ScrollBarArrowButtonState.UpActive;
                    this.bottomButtonState = ScrollBarArrowButtonState.DownActive;
                    this.thumbState = ScrollBarState.Active;
                    Invalidate();
                }
            }
        }

        /// <summary>
        /// Performs the work of setting the specified bounds of this control.
        /// </summary>
        /// <param name="x">The new x value of the control.</param>
        /// <param name="y">The new y value of the control.</param>
        /// <param name="width">The new width value of the control.</param>
        /// <param name="height">The new height value of the control.</param>
        /// <param name="specified">A bitwise combination of the <see cref="BoundsSpecified"/> values.</param>
        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            // only in design mode - constrain size
            if (this.DesignMode)
            {
                var pad = ScaleHelper.Scale(10);
                if (this.orientation == ScrollBarOrientation.Vertical)
                {
                    if (height < 2 * this.arrowPaddedLength)
                        height = 2 * this.arrowPaddedLength;
                    width = ScaleOddUp(17);
                }
                else
                {
                    if (width < 2 * this.arrowPaddedLength)
                        width = 2 * this.arrowPaddedLength;
                    height = ScaleOddUp(17);
                }
            }
            base.SetBoundsCore(x, y, width, height, specified);
            if (this.DesignMode)
            {
                this.SetUpScrollBar();
            }
        }

        /// <summary>
        /// Raises the <see cref="System.Windows.Forms.Control.SizeChanged"/> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            this.SetUpScrollBar();
        }

        /// <summary>
        /// Processes a dialog key.
        /// </summary>
        /// <param name="keyData">One of the <see cref="System.Windows.Forms.Keys"/> values that represents the key to process.</param>
        /// <returns>true, if the key was processed by the control, false otherwise.</returns>
        protected override bool ProcessDialogKey(Keys keyData)
        {
            // key handling is here - keys recognized by the control
            // Up&Down or Left&Right, PageUp, PageDown, Home, End
            Keys keyUp = Keys.Up;
            Keys keyDown = Keys.Down;
            if (this.orientation == ScrollBarOrientation.Horizontal)
            {
                keyUp = Keys.Left;
                keyDown = Keys.Right;
            }
            if ((keyData == keyUp) || (keyData == keyDown))
            {
                this.Value = this.GetValue(true, keyData == keyUp);
                return true;
            }
            if ((keyData == Keys.PageUp) || (keyData == Keys.PageDown))
            {
                this.Value = this.GetValue(false, keyData == Keys.PageUp);
                return true;
            }
            if (keyData == Keys.Home)
            {
                this.Value = this.minimum;
                return true;
            }
            if (keyData == Keys.End)
            {
                this.Value = this.maximum;
                return true;
            }
            return base.ProcessDialogKey(keyData);
        }

        /// <summary>
        /// Raises the <see cref="System.Windows.Forms.Control.EnabledChanged"/> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);

            if (this.Enabled)
            {
                this.thumbState = ScrollBarState.Normal;
                this.topButtonState = ScrollBarArrowButtonState.UpNormal;
                this.bottomButtonState = ScrollBarArrowButtonState.DownNormal;
            }
            else
            {
                this.thumbState = ScrollBarState.Disabled;
                this.topButtonState = ScrollBarArrowButtonState.UpDisabled;
                this.bottomButtonState = ScrollBarArrowButtonState.DownDisabled;
            }

            this.Refresh();
        }

        #endregion

        #region Misc Methods

        /// <summary>
        /// Sets up the scrollbar.
        /// </summary>
        private void SetUpScrollBar()
        {
            // if no drawing - return
            if (this.inUpdate)
            {
                return;
            }
            // save client rectangle
            Rectangle rect = ClientRectangle;
            // set up the width's, height's and rectangles for the different
            // elements
            this.thumbThickness = ScaleOddUp(THUMB_THICKNESS); // Should be odd for the thumb to be perfectly centered (since scrollbar width is odd)
            this.arrowThickness = ScaleOddUp(ARROW_THICKNESS); // Should be odd for nice and crisp arrow points.
            this.arrowLength = ScaleHelper.Scale(ARROW_LENGTH);
            this.arrowPadding = ScaleHelper.Scale(ARROW_PADDING);
            this.arrowThumbGap = ScaleHelper.Scale(ARROW_THUMB_GAP);
            this.arrowPaddedLength = this.arrowLength + this.arrowPadding + this.arrowThumbGap;
            if (this.orientation == ScrollBarOrientation.Vertical)
            {
                this.thumbRectangle = new Rectangle(
                   rect.X + (rect.Width - this.thumbThickness) / 2, // Assuming rect.Width and this.thumbThickness are both odd, so that (rect.Width - this.thumbThickness) is even.
                   rect.Y + this.arrowPaddedLength,
                   this.thumbThickness,
                   this.GetThumbSize()
                );
                this.topArrowRectangle = new Rectangle(
                   rect.X + (rect.Width - this.arrowThickness) / 2, // Assuming rect.Width and this.arrowThickness are both odd, so that (rect.Width - this.arrowThickness) is even.
                   rect.Y + this.arrowPadding,
                   this.arrowThickness,
                   this.arrowLength
                );
                this.bottomArrowRectangle = new Rectangle(
                   rect.X + (rect.Width - this.arrowThickness) / 2, // Assuming rect.Width and this.arrowThickness are both odd, so that (rect.Width - this.arrowThickness) is even.
                   rect.Bottom - this.arrowPadding - this.arrowLength,
                   this.arrowThickness,
                   this.arrowLength
                );
                // Set the default starting thumb position.
                //this.thumbPosition = this.thumbRectangle.Height / 2;
                // Set the bottom limit of the thumb's bottom border.
                this.thumbBottomLimitBottom = rect.Bottom - this.arrowPaddedLength;
                // Set the bottom limit of the thumb's top border.
                this.thumbBottomLimitTop = this.thumbBottomLimitBottom - this.thumbRectangle.Height;
                // Set the top limit of the thumb's top border.
                this.thumbTopLimit = rect.Y + this.arrowPaddedLength;
            }
            else
            {
                this.thumbRectangle = new Rectangle(
                   rect.X + this.arrowPaddedLength,
                   rect.Y + (rect.Height - this.thumbThickness) / 2, // Assuming rect.Height and this.thumbThickness are both odd, so that (rect.Height - this.thumbThickness) is even.
                   this.GetThumbSize(),
                   this.thumbThickness
                );
                this.topArrowRectangle = new Rectangle(
                   rect.X + this.arrowPadding,
                   rect.Y + (rect.Height - this.arrowThickness) / 2, // Assuming rect.Height and this.arrowThickness are both odd, so that (rect.Height - this.arrowThickness) is even.
                   this.arrowLength,
                   this.arrowThickness
                );
                this.bottomArrowRectangle = new Rectangle(
                   rect.Right - this.arrowPadding - this.arrowLength,
                   rect.Y + (rect.Height - this.arrowThickness) / 2, // Assuming rect.Height and this.arrowThickness are both odd, so that (rect.Height - this.arrowThickness) is even.
                   this.arrowLength,
                   this.arrowThickness
                );
                // Set the default starting thumb position.
                //this.thumbPosition = this.thumbRectangle.Width / 2;
                // Set the bottom limit of the thumb's bottom border.
                this.thumbBottomLimitBottom = rect.Right - this.arrowPaddedLength;
                // Set the bottom limit of the thumb's top border.
                this.thumbBottomLimitTop = this.thumbBottomLimitBottom - this.thumbRectangle.Width;
                // Set the top limit of the thumb's top border.
                this.thumbTopLimit = rect.X + this.arrowPaddedLength;
            }
            this.ChangeThumbPosition(this.GetThumbPosition());
            this.Refresh();
        }

        /// <summary>
        /// Handles the updating of the thumb.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">An object that contains the event data.</param>
        private void ProgressTimerTick(object sender, EventArgs e)
        {
            this.ProgressThumb(true);
        }

        /// <summary>
        /// Resets the scroll status of the scrollbar.
        /// </summary>
        private void ResetScrollStatus()
        {
            // get current mouse position
            Point pos = this.PointToClient(Cursor.Position);
            // set appearance of buttons in relation to where the mouse is -
            // outside or inside the control
            if (this.ClientRectangle.Contains(pos))
            {
                this.bottomButtonState = ScrollBarArrowButtonState.DownActive;
                this.topButtonState = ScrollBarArrowButtonState.UpActive;
            }
            else
            {
                this.bottomButtonState = ScrollBarArrowButtonState.DownNormal;
                this.topButtonState = ScrollBarArrowButtonState.UpNormal;
            }
            // set appearance of thumb
            this.thumbState = this.thumbRectangle.Contains(pos) ? ScrollBarState.Hot : ScrollBarState.Normal;
            this.bottomArrowClicked = this.bottomBarClicked = this.topArrowClicked = this.topBarClicked = false;
            this.StopTimer();
            this.Refresh();
        }

        /// <summary>
        /// Calculates the new value of the scrollbar.
        /// </summary>
        /// <param name="smallIncrement">true for a small change, false otherwise.</param>
        /// <param name="up">true for up movement, false otherwise.</param>
        /// <returns>The new scrollbar value.</returns>
        private int GetValue(bool smallIncrement, bool up)
        {
            // calculate the new value of the scrollbar
            // with checking if new value is in bounds (min/max)
            if (up)
                return Math.Max(this.minimum, this.value - (smallIncrement ? this.smallChange : this.largeChange));
            else
                return Math.Min(this.maximum, this.value + (smallIncrement ? this.smallChange : this.largeChange));
        }

        /// <summary>
        /// Calculates the new thumb position.
        /// </summary>
        /// <returns>The new thumb position.</returns>
        private int GetThumbPosition()
        {
            int pixelRange = this.thumbBottomLimitTop - this.thumbTopLimit; // == size - thumbSize - arrows - paddings
            int realRange = this.maximum - this.minimum;
            float perc = (realRange != 0) ? ((float)(this.value - this.minimum) / (float)realRange) : 0f;
            return Math.Max(this.thumbTopLimit, Math.Min(this.thumbBottomLimitTop, Convert.ToInt32((perc * pixelRange) + this.arrowPaddedLength)));
        }

        /// <summary>
        /// Calculates the length of the thumb.
        /// </summary>
        /// <returns>The length of the thumb.</returns>
        private int GetThumbSize()
        {
            int trackSize = (this.orientation == ScrollBarOrientation.Vertical ? this.Height : this.Width) - 2 * this.arrowPaddedLength;
            float newThumbSize = (float)this.viewPortSize * (float)trackSize / (float)(this.maximum - this.minimum + this.viewPortSize);
            return Convert.ToInt32(Math.Min((float)trackSize, Math.Max(newThumbSize, ScaleHelper.Scale(8))));
        }

        /// <summary>
        /// Enables the timer.
        /// </summary>
        private void EnableTimer()
        {
            // if timer is not already enabled - enable it
            if (!this.progressTimer.Enabled)
            {
                this.progressTimer.Interval = PROGRESS_TIMER_DELAY;
                this.progressTimer.Start();
            }
            else
            {
                // if already enabled, change tick time
                this.progressTimer.Interval = PROGRESS_TIMER_TICK;
            }
        }

        /// <summary>
        /// Stops the progress timer.
        /// </summary>
        private void StopTimer()
        {
            this.progressTimer.Stop();
        }

        /// <summary>
        /// Changes the position of the thumb.
        /// </summary>
        /// <param name="position">The new position.</param>
        private void ChangeThumbPosition(int position)
        {
            if (this.orientation == ScrollBarOrientation.Vertical)
                this.thumbRectangle.Y = position;
            else
                this.thumbRectangle.X = position;
        }

        /// <summary>
        /// Controls the movement of the thumb.
        /// </summary>
        /// <param name="enableTimer">true for enabling the timer, false otherwise.</param>
        private void ProgressThumb(bool enableTimer)
        {
            int scrollOldValue = this.value;
            ScrollEventType type = ScrollEventType.First;
            int thumbSize, thumbPos;
            thumbPos = (this.orientation == ScrollBarOrientation.Vertical) ? this.thumbRectangle.Y : this.thumbRectangle.X;
            thumbSize = (this.orientation == ScrollBarOrientation.Vertical) ? this.thumbRectangle.Height : this.thumbRectangle.Width;
            // arrow down or shaft down clicked
            if (this.bottomArrowClicked || (this.bottomBarClicked && (thumbPos + thumbSize) < this.trackPosition))
            {
                type = this.bottomArrowClicked ? ScrollEventType.SmallIncrement : ScrollEventType.LargeIncrement;
                this.value = this.GetValue(this.bottomArrowClicked, false);
                if (this.value == this.maximum)
                {
                    this.ChangeThumbPosition(this.thumbBottomLimitTop);

                    type = ScrollEventType.Last;
                }
                else this.ChangeThumbPosition(Math.Min(this.thumbBottomLimitTop, this.GetThumbPosition()));
            }
            else if (this.topArrowClicked || (this.topBarClicked && thumbPos > this.trackPosition))
            {
                type = this.topArrowClicked ? ScrollEventType.SmallDecrement : ScrollEventType.LargeDecrement;
                // arrow up or shaft up clicked
                this.value = this.GetValue(this.topArrowClicked, true);
                if (this.value == this.minimum)
                {
                    this.ChangeThumbPosition(this.thumbTopLimit);
                    type = ScrollEventType.First;
                }
                else this.ChangeThumbPosition(Math.Max(this.thumbTopLimit, this.GetThumbPosition()));
            }
            else if (!((this.topArrowClicked && thumbPos == this.thumbTopLimit) || (this.bottomArrowClicked && thumbPos == this.thumbBottomLimitTop)))
            {
                this.ResetScrollStatus();
                return;
            }
            if (scrollOldValue != this.value)
            {
                this.OnScroll(new ScrollEventArgs(type, scrollOldValue, this.value, this.scrollOrientation));
                this.Refresh();
                if (enableTimer)
                    this.EnableTimer();
            }
            else
            {
                if (this.topArrowClicked)
                    type = ScrollEventType.SmallDecrement;
                else if (this.bottomArrowClicked)
                    type = ScrollEventType.SmallIncrement;
                this.OnScroll(new ScrollEventArgs(type, this.value));
            }
        }

        /// <summary>
        /// Changes the displayed text of the context menu items dependent of the current <see cref="ScrollBarOrientation"/>.
        /// </summary>
        private void ChangeContextMenuItems()
        {
            if (this.orientation == ScrollBarOrientation.Vertical)
            {
                this.tsmiTop.Text = "Top";
                this.tsmiBottom.Text = "Bottom";
                this.tsmiLargeDown.Text = "Page Down";
                this.tsmiLargeUp.Text = "Page Up";
                this.tsmiSmallDown.Text = "Scroll Down";
                this.tsmiSmallUp.Text = "Scroll Up";
                this.tsmiScrollHere.Text = "Scroll Here";
            }
            else
            {
                this.tsmiTop.Text = "Left";
                this.tsmiBottom.Text = "Right";
                this.tsmiLargeDown.Text = "Page Left";
                this.tsmiLargeUp.Text = "Page Right";
                this.tsmiSmallDown.Text = "Scroll Right";
                this.tsmiSmallUp.Text = "Scroll Left";
                this.tsmiScrollHere.Text = "Scroll Here";
            }
        }

        #endregion

        #region Context Menu Methods

        /// <summary>
        /// Initializes the context menu.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiScrollHere = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiTop = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiBottom = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiLargeUp = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiLargeDown = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiSmallUp = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSmallDown = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenu
            // 
            this.contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiScrollHere,
            this.toolStripSeparator1,
            this.tsmiTop,
            this.tsmiBottom,
            this.toolStripSeparator2,
            this.tsmiLargeUp,
            this.tsmiLargeDown,
            this.toolStripSeparator3,
            this.tsmiSmallUp,
            this.tsmiSmallDown});
            this.contextMenu.Name = "contextMenu";
            this.contextMenu.Size = new System.Drawing.Size(151, 176);
            // 
            // tsmiScrollHere
            // 
            this.tsmiScrollHere.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsmiScrollHere.Name = "tsmiScrollHere";
            this.tsmiScrollHere.Size = new System.Drawing.Size(150, 22);
            this.tsmiScrollHere.Text = "Scroll Here";
            this.tsmiScrollHere.Click += ScrollHereClick;
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(147, 6);
            // 
            // tsmiTop
            // 
            this.tsmiTop.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsmiTop.Name = "tsmiTop";
            this.tsmiTop.Size = new System.Drawing.Size(150, 22);
            this.tsmiTop.Text = "Top";
            this.tsmiTop.Click += this.TopClick;
            // 
            // tsmiBottom
            // 
            this.tsmiBottom.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsmiBottom.Name = "tsmiBottom";
            this.tsmiBottom.Size = new System.Drawing.Size(150, 22);
            this.tsmiBottom.Text = "Bottom";
            this.tsmiBottom.Click += this.BottomClick;
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(147, 6);
            // 
            // tsmiLargeUp
            // 
            this.tsmiLargeUp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsmiLargeUp.Name = "tsmiLargeUp";
            this.tsmiLargeUp.Size = new System.Drawing.Size(150, 22);
            this.tsmiLargeUp.Text = "Page Up";
            this.tsmiLargeUp.Click += this.LargeUpClick;
            // 
            // tsmiLargeDown
            // 
            this.tsmiLargeDown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsmiLargeDown.Name = "tsmiLargeDown";
            this.tsmiLargeDown.Size = new System.Drawing.Size(150, 22);
            this.tsmiLargeDown.Text = "Page Down";
            this.tsmiLargeDown.Click += this.LargeDownClick;
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(147, 6);
            // 
            // tsmiSmallUp
            // 
            this.tsmiSmallUp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsmiSmallUp.Name = "tsmiSmallUp";
            this.tsmiSmallUp.Size = new System.Drawing.Size(150, 22);
            this.tsmiSmallUp.Text = "Scroll Up";
            this.tsmiSmallUp.Click += this.SmallUpClick;
            // 
            // tsmiSmallDown
            // 
            this.tsmiSmallDown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsmiSmallDown.Name = "tsmiSmallDown";
            this.tsmiSmallDown.Size = new System.Drawing.Size(150, 22);
            this.tsmiSmallDown.Text = "Scroll Down";
            this.tsmiSmallDown.Click += this.SmallDownClick;
            this.contextMenu.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        /// <summary>
        /// Context menu handler.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void ScrollHereClick(object sender, EventArgs e)
        {
            int thumbSize = (this.orientation == ScrollBarOrientation.Vertical) ? this.thumbRectangle.Height : this.thumbRectangle.Width;
            this.ChangeThumbPosition(Math.Max(this.thumbTopLimit, Math.Min(this.thumbBottomLimitTop, this.trackPosition - (thumbSize / 2))));
            int pixelRange = this.thumbBottomLimitTop - this.thumbTopLimit; // == size - thumbSize - arrows - paddings
            int position = ((this.orientation == ScrollBarOrientation.Vertical) ? this.thumbRectangle.Y : this.thumbRectangle.X) - this.arrowPaddedLength;
            // percent of the new position
            float perc = (pixelRange != 0) ? ((float)position / (float)pixelRange) : 0f;
            int oldValue = this.value;
            this.value = Convert.ToInt32((perc * (this.maximum - this.minimum)) + this.minimum);
            this.OnScroll(new ScrollEventArgs(ScrollEventType.ThumbPosition, oldValue, this.value, this.scrollOrientation));
            this.Refresh();
        }

        /// <summary>
        /// Context menu handler.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void TopClick(object sender, EventArgs e)
        {
            this.Value = this.minimum;
        }

        /// <summary>
        /// Context menu handler.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void BottomClick(object sender, EventArgs e)
        {
            this.Value = this.maximum;
        }

        /// <summary>
        /// Context menu handler.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void LargeUpClick(object sender, EventArgs e)
        {
            this.Value = this.GetValue(false, true);
        }

        /// <summary>
        /// Context menu handler.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void LargeDownClick(object sender, EventArgs e)
        {
            this.Value = this.GetValue(false, false);
        }

        /// <summary>
        /// Context menu handler.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void SmallUpClick(object sender, EventArgs e)
        {
            this.Value = this.GetValue(true, true);
        }

        /// <summary>
        /// Context menu handler.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        private void SmallDownClick(object sender, EventArgs e)
        {
            this.Value = this.GetValue(true, false);
        }

        #endregion

    }

    #region Scrollers

    public class ScrollerBase : IEventHandler, IDisposable
    {
        protected Control control;
        protected ScrollBarEx vScrollBar;
        protected ScrollBarEx hScrollBar;
        protected Control scrollerCorner;
        private bool disposed = false;

        /// <summary>
        /// Initialize ScrollerBase
        /// </summary>
        public ScrollerBase(Control control)
        {
            this.control = control;
        }

        /// <summary>
        /// Handle the incoming theme events
        /// </summary>
        public void HandleEvent(object sender, NotifyEvent e, HandlingPriority priority)
        {
            if (e.Type == EventType.ApplyTheme)
            {
                bool enabled = PluginBase.MainForm.GetThemeFlag("ScrollBar.UseGlobally", false);
                if (control.Parent == null) return;
                else if (enabled)
                {
                    if (!control.Parent.Controls.Contains(vScrollBar)) AddScrollBars();
                    UpdateScrollBarTheme();
                }
                else if (!enabled && control.Parent.Controls.Contains(vScrollBar))
                {
                    RemoveScrollBars();
                }
            }
        }

        /// <summary>
        /// Dispose the controls (public interface)
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose the controls
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposed) return;
            if (disposing)
            {
                control = null;
                vScrollBar.Dispose();
                hScrollBar.Dispose();
                scrollerCorner.Dispose();
            }
            disposed = true;
        }

        /// <summary>
        /// Init the custom scrollbars
        /// </summary>
        protected virtual void InitScrollBars()
        {
            vScrollBar = new ScrollBarEx();
            vScrollBar.Width = SystemInformation.VerticalScrollBarWidth; // Already scaled.
            // Should be odd for nice and crisp arrow points.
            if (vScrollBar.Width % 2 == 0) ++vScrollBar.Width;
            vScrollBar.Orientation = ScrollBarOrientation.Vertical;
            vScrollBar.ContextMenuStrip.Renderer = new DockPanelStripRenderer();
            hScrollBar = new ScrollBarEx();
            hScrollBar.Height = SystemInformation.HorizontalScrollBarHeight; // Already scaled.
            // Should be odd for nice and crisp arrow points.
            if (hScrollBar.Height % 2 == 0) ++hScrollBar.Width;
            hScrollBar.Orientation = ScrollBarOrientation.Horizontal;
            hScrollBar.ContextMenuStrip.Renderer = new DockPanelStripRenderer();
            scrollerCorner = new Control();
            scrollerCorner.Width = vScrollBar.Width;
            scrollerCorner.Height = hScrollBar.Height;
            if (PluginBase.MainForm.GetThemeFlag("ScrollBar.UseGlobally", false))
            {
                AddScrollBars();
                UpdateScrollBarTheme();
            }
            EventManager.AddEventHandler(this, EventType.ApplyTheme);
        }

        /// <summary>
        /// Add controls to container
        /// </summary>
        protected virtual void AddScrollBars()
        {
            control.Parent.Controls.Add(hScrollBar);
            control.Parent.Controls.Add(vScrollBar);
            control.Parent.Controls.Add(scrollerCorner);
            vScrollBar.Scroll += OnScroll;
            hScrollBar.Scroll += OnScroll;
            vScrollBar.VisibleChanged += OnResize;
            hScrollBar.VisibleChanged += OnResize;
            control.Parent.Resize += OnResize;
            control.Resize += OnResize;
            control.Paint += OnPaint;
        }

        /// <summary>
        /// Remove controls from container
        /// </summary>
        protected virtual void RemoveScrollBars()
        {
            control.Parent.Controls.Remove(hScrollBar);
            control.Parent.Controls.Remove(vScrollBar);
            control.Parent.Controls.Remove(scrollerCorner);
            vScrollBar.Scroll -= OnScroll;
            hScrollBar.Scroll -= OnScroll;
            vScrollBar.VisibleChanged -= OnResize;
            hScrollBar.VisibleChanged -= OnResize;
            control.Parent.Resize -= OnResize;
            control.Resize -= OnResize;
            control.Paint -= OnPaint;
        }

        /// <summary>
        /// Updates the scrollbar theme and applies old defaults
        /// </summary>
        protected virtual void UpdateScrollBarTheme()
        {
            PluginBase.MainForm.ThemeControls(vScrollBar);
            PluginBase.MainForm.ThemeControls(hScrollBar);
            // Apply settings so that old defaults work...
            vScrollBar.ArrowColor = PluginBase.MainForm.GetThemeColor("ScrollBar.ArrowColor", vScrollBar.ForeColor);
            vScrollBar.HotArrowColor = PluginBase.MainForm.GetThemeColor("ScrollBar.HotArrowColor", vScrollBar.ForeColor);
            vScrollBar.ActiveArrowColor = PluginBase.MainForm.GetThemeColor("ScrollBar.ActiveArrowColor", vScrollBar.ActiveForeColor);
            vScrollBar.HotForeColor = PluginBase.MainForm.GetThemeColor("ScrollBar.HotForeColor", vScrollBar.ForeColor);
            hScrollBar.ArrowColor = PluginBase.MainForm.GetThemeColor("ScrollBar.ArrowColor", hScrollBar.ForeColor);
            hScrollBar.HotArrowColor = PluginBase.MainForm.GetThemeColor("ScrollBar.HotArrowColor", hScrollBar.ForeColor);
            hScrollBar.ActiveArrowColor = PluginBase.MainForm.GetThemeColor("ScrollBar.ActiveArrowColor", hScrollBar.ActiveForeColor);
            hScrollBar.HotForeColor = PluginBase.MainForm.GetThemeColor("ScrollBar.HotForeColor", hScrollBar.ForeColor);
            scrollerCorner.BackColor = PluginBase.MainForm.GetThemeColor("ScrollBar.BackColor", vScrollBar.BackColor);
        }

        /// <summary>
        /// Updates the scrollbar scroll states
        /// </summary>
        protected virtual void UpdateScrollState()
        {
            Win32.SCROLLINFO vScroll = Win32.GetFullScrollInfo(control, false);
            Win32.SCROLLINFO hScroll = Win32.GetFullScrollInfo(control, true);
            if (vScroll != null && hScroll != null)
            {
                vScrollBar.Visible = vScroll.nMax > (vScroll.nPage - 1) && vScroll.nPage > 0;
                hScrollBar.Visible = hScroll.nMax > (hScroll.nPage - 1) && hScroll.nPage > 0;
                vScrollBar.Scroll -= OnScroll;
                vScrollBar.Minimum = vScroll.nMin;
                vScrollBar.Maximum = vScroll.nMax - (vScroll.nPage - 1);
                vScrollBar.ViewPortSize = vScrollBar.LargeChange = (vScroll.nPage - 1);
                vScrollBar.Value = vScroll.nPos;
                vScrollBar.Scroll += OnScroll;
                hScrollBar.Scroll -= OnScroll;
                hScrollBar.Minimum = hScroll.nMin;
                hScrollBar.Maximum = hScroll.nMax - (hScroll.nPage - 1);
                hScrollBar.ViewPortSize = hScrollBar.LargeChange = (hScroll.nPage - 1);
                hScrollBar.Value = hScroll.nPos;
                hScrollBar.Scroll += OnScroll;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void OnResize(object sender, EventArgs e)
        {
            int borderWidth = 0;
            if (control is PropertyGrid && borderWidth == 0) borderWidth = 1;
            int vScrollBarHeight = (control.Height - (borderWidth * 2)) - (hScrollBar.Visible ? hScrollBar.Height : 0);
            if (control is PropertyGrid)
            {
                foreach (Control ctrl in control.Controls)
                {
                    if (ctrl.Text == "PropertyGridView")
                    {
                        Type type = ctrl.GetType();
                        FieldInfo field = type.GetField("scrollBar", BindingFlags.Instance | BindingFlags.NonPublic);
                        var scrollBar = field.GetValue(ctrl) as ScrollBar;
                        vScrollBarHeight = scrollBar.Height;
                    }
                }
            }
            // Sets size, location and visibility
            vScrollBar.SetBounds(control.Location.X + control.Width - vScrollBar.Width - borderWidth, control.Location.Y + borderWidth, vScrollBar.Width, vScrollBarHeight);
            hScrollBar.SetBounds(control.Location.X + borderWidth, control.Location.Y + control.Height - hScrollBar.Height - borderWidth, (control.Width - (borderWidth * 2)) - (vScrollBar.Visible ? vScrollBar.Width : 0), hScrollBar.Height);
            scrollerCorner.Visible = vScrollBar.Visible && hScrollBar.Visible;
            if (scrollerCorner.Visible)
            {
                scrollerCorner.Location = new Point(vScrollBar.Location.X, hScrollBar.Location.Y);
                scrollerCorner.Refresh();
                scrollerCorner.BringToFront();
            }
            vScrollBar.BringToFront();
            hScrollBar.BringToFront();
            control.Invalidate();
        }

        /// <summary>
        /// Updates the scroll state on control paint
        /// </summary>
        protected virtual void OnPaint(object sender, PaintEventArgs e) => UpdateScrollState();

        /// <summary>
        /// Updates the control on scrollbar scroll
        /// </summary>
        protected virtual void OnScroll(object sender, ScrollEventArgs e) {}
    }

    public class TextBoxScroller : ScrollerBase
    {
        private bool disposed = false;
        private TextBox textBox;

        /// <summary>
        /// Initialize TextBoxScroller
        /// </summary>
        public TextBoxScroller(TextBox view) : base(view)
        {
            textBox = view;
            InitScrollBars();
        }

        /// <summary>
        /// Dispose the controls
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposed) return;
            if (disposing)
            {
                textBox = null;
            }
            disposed = true;
            base.Dispose(disposing);
        }

        /// <summary>
        /// Gets the current line index
        /// </summary>
        public int GetLineIndex(int index)
        {
            return Win32.SendMessage(textBox.Handle, Win32.EM_LINEINDEX, index, 0);
        }

        /// <summary>
        /// Gets the amount of lines, also with wrapping
        /// </summary>
        public int GetLineCount()
        {
            return Win32.SendMessage(textBox.Handle, Win32.EM_GETLINECOUNT, 0, 0);
        }

        /// <summary>
        /// Gets the first visible line on screen
        /// </summary>
        public int GetFirstVisibleLine()
        {
            return (int)Win32.SendMessage(textBox.Handle, Win32.EM_GETFIRSTVISIBLELINE, 0, 0);
        }

        /// <summary>
        /// Gets the amount of visible lines
        /// </summary>
        public int GetVisibleLines()
        {
            var rect = new Win32.RECT();
            Win32.SendMessage(textBox.Handle, Win32.EM_GETRECT, IntPtr.Zero, ref rect);
            var count = (rect.Bottom - rect.Top) / textBox.Font.Height;
            return count;
        }

        /// <summary>
        /// Updates the scrollbar scroll states
        /// </summary>
        protected override void UpdateScrollState()
        {
            int vScrollMax = GetLineCount();
            int vScrollPos = GetFirstVisibleLine();
            int vScrollPage = GetVisibleLines();
            if (this.textBox.ScrollBars != ScrollBars.Vertical)
            {
                // Force scrollbar so that content is displayed correctly...
                this.textBox.ScrollBars = ScrollBars.Vertical;
            }
            vScrollBar.Visible = vScrollMax > (vScrollPage - 1) && vScrollMax != vScrollPage;
            vScrollBar.Scroll -= OnScroll;
            vScrollBar.Minimum = 0;
            vScrollBar.Maximum = vScrollMax - (vScrollPage);
            vScrollBar.ViewPortSize = vScrollBar.LargeChange = (vScrollPage - 1);
            vScrollBar.Value = vScrollPos;
            vScrollBar.Scroll += OnScroll;
            hScrollBar.Visible = false;
        }

        /// <summary>
        /// Updates the textBox on scrollbar scroll
        /// </summary>
        protected override void OnScroll(object sender, ScrollEventArgs e)
        {
            if (e.OldValue == -1 || textBox.Lines.Length == 0) return;
            if (e.ScrollOrientation == ScrollOrientation.VerticalScroll)
            {
                int wParam = Win32.SB_THUMBPOSITION | e.NewValue << 16;
                Win32.SendMessage(textBox.Handle, Win32.WM_VSCROLL, (IntPtr)wParam, IntPtr.Zero);
            }
        }
    }

    public class RichTextBoxScroller : ScrollerBase
    {
        private bool disposed = false;
        private RichTextBox textBox;

        /// <summary>
        /// Initialize RichTextBoxScroller
        /// </summary>
        public RichTextBoxScroller(RichTextBox view) : base(view)
        {
            textBox = view;
            InitScrollBars();
        }

        /// <summary>
        /// Dispose the controls
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposed) return;
            if (disposing)
            {
                textBox = null;
            }
            disposed = true;
            base.Dispose(disposing);
        }

        /// <summary>
        /// Updates the textBox on scrollbar scroll
        /// </summary>
        protected override void OnScroll(object sender, ScrollEventArgs e)
        {
            if (e.OldValue == -1 || textBox.Lines.Length == 0) return;
            if (e.ScrollOrientation == ScrollOrientation.VerticalScroll)
            {
                int wParam = Win32.SB_THUMBPOSITION | e.NewValue << 16;
                Win32.SendMessage(textBox.Handle, Win32.WM_VSCROLL, (IntPtr)wParam, IntPtr.Zero);
            }
            else
            {
                int wParam = Win32.SB_THUMBPOSITION | e.NewValue << 16;
                Win32.SendMessage(textBox.Handle, Win32.WM_HSCROLL, (IntPtr)wParam, IntPtr.Zero);
            }
        }
    }

    public class PropertyGridScroller : ScrollerBase
    {
        private bool disposed = false;
        private PropertyGrid propertyGrid;

        /// <summary>
        /// Initialize PropertyGridScroller
        /// </summary>
        public PropertyGridScroller(PropertyGrid view) : base(view)
        {
            propertyGrid = view;
            InitScrollBars();
        }

        /// <summary>
        /// Dispose the controls
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposed) return;
            if (disposing)
            {
                propertyGrid = null;
            }
            disposed = true;
            base.Dispose(disposing);
        }

        /// <summary>
        /// Gets the amount of visible rows
        /// </summary>
        private int GetVisibleRows()
        {
            foreach (Control ctrl in propertyGrid.Controls)
            {
                if (ctrl.Text == "PropertyGridView")
                {
                    Type type = ctrl.GetType();
                    FieldInfo field = type.GetField("visibleRows", BindingFlags.Instance | BindingFlags.NonPublic);
                    return (int)field.GetValue(ctrl) - 1;
                }
            }
            return 0;
        }

        /// <summary>
        /// Updates the scroll position of the scrollbar
        /// </summary>
        private void SetScrollOffset(int value)
        {
            foreach (Control ctrl in propertyGrid.Controls)
            {
                if (ctrl.Text == "PropertyGridView")
                {
                    Type type = ctrl.GetType();
                    MethodInfo info = type.GetMethod("SetScrollOffset");
                    object[] parameters = { value };
                    info.Invoke(ctrl, parameters);
                }
            }
        }

        /// <summary>
        /// Gets the scrollbar reference
        /// </summary>
        private ScrollBar GetScrollBar()
        {
            foreach (Control ctrl in propertyGrid.Controls)
            {
                if (ctrl.Text == "PropertyGridView")
                {
                    Type type = ctrl.GetType();
                    FieldInfo field = type.GetField("scrollBar", BindingFlags.Instance | BindingFlags.NonPublic);
                    return field.GetValue(ctrl) as ScrollBar;
                }
            }
            return null;
        }

        /// <summary>
        /// Updates the scrollbar scroll states
        /// </summary>
        protected override void UpdateScrollState()
        {
            var scrollBar = GetScrollBar();
            if (scrollBar != null)
            {
                vScrollBar.Scroll -= OnScroll;
                vScrollBar.Visible = scrollBar.Visible;
                vScrollBar.Minimum = scrollBar.Minimum;
                vScrollBar.Maximum = scrollBar.Maximum - (GetVisibleRows() - 1);
                vScrollBar.SmallChange = scrollBar.SmallChange;
                vScrollBar.LargeChange = scrollBar.LargeChange;
                vScrollBar.ViewPortSize = scrollBar.LargeChange = (GetVisibleRows() - 1);
                vScrollBar.Value = scrollBar.Value;
                vScrollBar.Scroll += OnScroll;
            }
            else vScrollBar.Visible = false;
            hScrollBar.Visible = false;
        }

        /// <summary>
        /// Updates the propertyGrid on scrollbar scroll
        /// </summary>
        protected override void OnScroll(object sender, ScrollEventArgs e)
        {
            if (e.OldValue == -1 || propertyGrid.SelectedObjects.Length == 0) return;
            if (e.ScrollOrientation == ScrollOrientation.VerticalScroll)
            {
                SetScrollOffset(e.NewValue);
            }
        }
    }

    public class TreeViewScroller : ScrollerBase
    {
        private bool disposed = false;
        private TreeView treeView;

        /// <summary>
        /// Initialize TreeViewScroller
        /// </summary>
        public TreeViewScroller(TreeView view) : base(view)
        {
            treeView = view;
            InitScrollBars();
        }

        /// <summary>
        /// Dispose the controls
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposed) return;
            if (disposing)
            {
                treeView = null;
            }
            disposed = true;
            base.Dispose(disposing);
        }

        /// <summary>
        /// Updates the treeView on scrollbar scroll
        /// </summary>
        protected override void OnScroll(object sender, ScrollEventArgs e)
        {
            if (e.OldValue == -1 || treeView.Nodes.Count == 0) return;
            if (e.ScrollOrientation == ScrollOrientation.VerticalScroll)
            {
                if (!PlatformHelper.isRunningOnWine())
                {
                    treeView.BeginUpdate();
                    Win32.SetScrollPos(treeView.Handle, Win32.SB_VERT, e.NewValue, true);
                    treeView.EndUpdate();
                }
                else
                {
                    int wParam = Win32.SB_THUMBPOSITION | e.NewValue << 16;
                    Win32.SendMessage(treeView.Handle, Win32.WM_VSCROLL, (IntPtr)wParam, IntPtr.Zero);
                }

            }
            else
            {
                if (!PlatformHelper.isRunningOnWine())
                {
                    treeView.BeginUpdate();
                    Win32.SetScrollPos(treeView.Handle, Win32.SB_HORZ, e.NewValue, true);
                    treeView.EndUpdate();
                }
                else
                {
                    int wParam = Win32.SB_THUMBPOSITION | e.NewValue << 16;
                    Win32.SendMessage(treeView.Handle, Win32.WM_HSCROLL, (IntPtr)wParam, IntPtr.Zero);
                }
            }
        }
    }

    public class ListViewScroller : ScrollerBase
    {
        private bool disposed = false;
        protected ListView listView;

        /// <summary>
        /// Initialize ListViewScroller
        /// </summary>
        public ListViewScroller(ListView view) : base(view)
        {
            listView = view;
            InitScrollBars();
        }

        /// <summary>
        /// Dispose the controls
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposed) return;
            if (disposing)
            {
                listView = null;
            }
            disposed = true;
            base.Dispose(disposing);
        }

        /// <summary>
        /// Updates the listView on scrollbar scroll
        /// </summary>
        protected override void OnScroll(object sender, ScrollEventArgs e)
        {
            if (e.OldValue == -1 || listView.Items.Count == 0) return;
            int height = listView.GetItemRect(0).Height; // Item height in pixels
            if (e.ScrollOrientation == ScrollOrientation.VerticalScroll)
            {
                int vScroll;
                if (listView.ShowGroups && !PlatformHelper.isRunningOnWine())
                {
                    int prevPos = Win32.GetScrollPos(listView.Handle, Win32.SB_VERT);
                    vScroll = -(prevPos - e.NewValue);
                }
                else vScroll = -(e.OldValue - e.NewValue) * height;
                Win32.SendMessage(listView.Handle, (int)Win32.LVM_SCROLL, IntPtr.Zero, (IntPtr)vScroll);
            }
            else
            {
                int hScroll = -(e.OldValue - e.NewValue);
                Win32.SendMessage(listView.Handle, (int)Win32.LVM_SCROLL, (IntPtr)hScroll, IntPtr.Zero);
            }
        }
    }

    public class ListBoxScroller : ScrollerBase
    {
        private bool disposed = false;
        private ListBox listBox;

        /// <summary>
        /// Initialize ListBoxScroller
        /// </summary>
        public ListBoxScroller(ListBox view) : base(view)
        {
            listBox = view;
            InitScrollBars();
        }

        /// <summary>
        /// Dispose the controls
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposed) return;
            if (disposing)
            {
                listBox = null;
            }
            disposed = true;
            base.Dispose(disposing);
        }

        /// <summary>
        /// Updates the listBox on scrollbar scroll
        /// </summary>
        protected override void OnScroll(object sender, ScrollEventArgs e)
        {
            if (e.OldValue == -1 || listBox.Items.Count == 0) return;
            if (e.ScrollOrientation == ScrollOrientation.VerticalScroll)
            {
                Win32.PostMessage((IntPtr)listBox.Handle, Win32.WM_VSCROLL, 4 + 0x10000 * e.NewValue, 0);
            }
            else
            {
                Win32.PostMessage((IntPtr)listBox.Handle, Win32.WM_HSCROLL, 4 + 0x10000 * e.NewValue, 0);
            }
        }
    }

    public class DataGridViewScroller : ScrollerBase
    {
        private bool disposed = false;
        private DataGridView dataGridView;

        /// <summary>
        /// Initialize DataGridViewScroller
        /// </summary>
        public DataGridViewScroller(DataGridView view) : base(view)
        {
            dataGridView = view;
            InitScrollBars();
        }

        /// <summary>
        /// Dispose the controls
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposed) return;
            if (disposing)
            {
                dataGridView = null;
            }
            disposed = true;
            base.Dispose(disposing);
        }

        /// <summary>
        /// Updates the scrollbar scroll states
        /// </summary>
        protected override void UpdateScrollState()
        {
            int vScrollMax = dataGridView.RowCount;
            int vScrollPos = dataGridView.FirstDisplayedScrollingRowIndex;
            int vScrollPage = dataGridView.DisplayedRowCount(false);
            int hScrollMax = dataGridView.ColumnCount;
            int hScrollPos = dataGridView.FirstDisplayedScrollingColumnIndex;
            int hScrollPage = dataGridView.DisplayedColumnCount(false);
            vScrollBar.Visible = vScrollMax > (vScrollPage - 1) && vScrollMax != vScrollPage;
            hScrollBar.Visible = hScrollMax > (hScrollPage - 1) && hScrollMax != hScrollPage;
            vScrollBar.Scroll -= OnScroll;
            vScrollBar.Minimum = 0;
            vScrollBar.Maximum = vScrollMax - (vScrollPage);
            vScrollBar.ViewPortSize = vScrollBar.LargeChange = (vScrollPage - 1);
            vScrollBar.Value = vScrollPos;
            vScrollBar.Scroll += OnScroll;
            hScrollBar.Scroll -= OnScroll;
            hScrollBar.Minimum = 0;
            hScrollBar.Maximum = hScrollMax - (hScrollPage - 1);
            hScrollBar.ViewPortSize = hScrollBar.LargeChange = (hScrollPage - 1);
            hScrollBar.Value = hScrollPos;
            hScrollBar.Scroll += OnScroll;
        }

        /// <summary>
        /// Updates the dataGridView on scrollbar scroll
        /// </summary>
        protected override void OnScroll(object sender, ScrollEventArgs e)
        {
            if (e.OldValue == -1 || dataGridView.RowCount == 0) return;
            if (e.ScrollOrientation == ScrollOrientation.VerticalScroll) dataGridView.FirstDisplayedScrollingRowIndex = e.NewValue;
            else dataGridView.FirstDisplayedScrollingColumnIndex = e.NewValue;
        }

    }

    #endregion

    #region Designer

    /// <summary>
    /// The designer for the <see cref="ScrollBarEx"/> control.
    /// </summary>
    internal class ScrollBarDesigner : ControlDesigner
    {
        /// <summary>
        /// Gets the <see cref="SelectionRules"/> for the control.
        /// </summary>
        public override SelectionRules SelectionRules
        {
            get
            {
                // gets the property descriptor for the property "Orientation"
                PropertyDescriptor propDescriptor = TypeDescriptor.GetProperties(this.Component)["Orientation"];
                // if not null - we can read the current orientation of the scroll bar
                if (propDescriptor != null)
                {
                    // get the current orientation
                    ScrollBarOrientation orientation = (ScrollBarOrientation)propDescriptor.GetValue(this.Component);
                    // if vertical orientation
                    if (orientation == ScrollBarOrientation.Vertical)
                    {
                        return SelectionRules.Visible | SelectionRules.Moveable | SelectionRules.BottomSizeable | SelectionRules.TopSizeable;
                    }
                    return SelectionRules.Visible | SelectionRules.Moveable | SelectionRules.LeftSizeable | SelectionRules.RightSizeable;
                }
                return base.SelectionRules;
            }
        }

        /// <summary>
        /// Prefilters the properties so that unnecessary properties are hidden
        /// in the property browser of Visual Studio.
        /// </summary>
        /// <param name="properties">The property dictionary.</param>
        protected override void PreFilterProperties(System.Collections.IDictionary properties)
        {
            properties.Remove("Text");
            properties.Remove("BackgroundImage");
            properties.Remove("ImeMode");
            properties.Remove("Padding");
            properties.Remove("BackgroundImageLayout");
            properties.Remove("Font");
            properties.Remove("RightToLeft");
            base.PreFilterProperties(properties);
        }

    }

    #endregion

    #region Enums

    /// <summary>
    /// Enum for the scrollbar orientation.
    /// </summary>
    public enum ScrollBarOrientation
    {
        /// <summary>
        /// Indicates a horizontal scrollbar.
        /// </summary>
        Horizontal,

        /// <summary>
        /// Indicates a vertical scrollbar.
        /// </summary>
        Vertical
    }
    
    /// <summary>
    /// The scrollbar states.
    /// </summary>
    internal enum ScrollBarState
    {
        /// <summary>
        /// Indicates a normal scrollbar state.
        /// </summary>
        Normal,

        /// <summary>
        /// Indicates a hot scrollbar state.
        /// </summary>
        Hot,

        /// <summary>
        /// Indicates an active scrollbar state.
        /// </summary>
        Active,

        /// <summary>
        /// Indicates a pressed scrollbar state.
        /// </summary>
        Pressed,

        /// <summary>
        /// Indicates a disabled scrollbar state.
        /// </summary>
        Disabled
    }

    /// <summary>
    /// The scrollbar arrow button states.
    /// </summary>
    internal enum ScrollBarArrowButtonState
    {
        /// <summary>
        /// Indicates the up arrow is in normal state.
        /// </summary>
        UpNormal,

        /// <summary>
        /// Indicates the up arrow is in hot state.
        /// </summary>
        UpHot,

        /// <summary>
        /// Indicates the up arrow is in active state.
        /// </summary>
        UpActive,

        /// <summary>
        /// Indicates the up arrow is in pressed state.
        /// </summary>
        UpPressed,

        /// <summary>
        /// Indicates the up arrow is in disabled state.
        /// </summary>
        UpDisabled,

        /// <summary>
        /// Indicates the down arrow is in normal state.
        /// </summary>
        DownNormal,

        /// <summary>
        /// Indicates the down arrow is in hot state.
        /// </summary>
        DownHot,

        /// <summary>
        /// Indicates the down arrow is in active state.
        /// </summary>
        DownActive,

        /// <summary>
        /// Indicates the down arrow is in pressed state.
        /// </summary>
        DownPressed,

        /// <summary>
        /// Indicates the down arrow is in disabled state.
        /// </summary>
        DownDisabled
    }

    #endregion

}
