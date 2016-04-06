using System;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.ComponentModel;
using PluginCore.Helpers;
using PluginCore.Managers;

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
        public static IDisposable Attach(ListView listView)
        {
            if (!Win32.ShouldUseWin32() && !PluginBase.MainForm.GetThemeFlag("ScrollBar.UseGlobally", false)) return null;
            else return new ListViewScroller(listView);
        }
        public static IDisposable Attach(TreeView treeView)
        {
            if (!Win32.ShouldUseWin32() && !PluginBase.MainForm.GetThemeFlag("ScrollBar.UseGlobally", false)) return null;
            else return new TreeViewScroller(treeView);
        }
        public static IDisposable Attach(RichTextBox richTextBox)
        {
            if (!Win32.ShouldUseWin32() && !PluginBase.MainForm.GetThemeFlag("ScrollBar.UseGlobally", false)) return null;
            else return new RichTextBoxScroller(richTextBox);
        }
        public static IDisposable Attach(DataGridView dataGridView)
        {
            if (!Win32.ShouldUseWin32() && !PluginBase.MainForm.GetThemeFlag("ScrollBar.UseGlobally", false)) return null;
            else return new DataGridViewScroller(dataGridView);
        }

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

            switch (orientation) {
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

            switch (orientation) {
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

            switch (orientation) {
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
            rect.X += ScaleHelper.Scale(2);
            rect.Width -= ScaleHelper.Scale(4);
            using (Brush brush = new SolidBrush(color))
            {
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
            rect.Y += ScaleHelper.Scale(2);
            rect.Height -= ScaleHelper.Scale(4);
            using (Brush brush = new SolidBrush(color))
            {
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
                Point[] arrow;
                Int32 pad;
                Point middle = new Point(rect.Left + rect.Width / 2, (rect.Top + rect.Height / 2));
                switch (arrowUp)
                {
                    case true:
                        pad = ScaleHelper.Scale(4);
                        middle.Y += ScaleHelper.Scale(2);
                        arrow = new Point[]
                        {
                            new Point(middle.X - pad , middle.Y + 1),
                            new Point(middle.X + pad  + 1, middle.Y + 1),
                            new Point(middle.X, middle.Y - pad)
                        };
                        break;
                    default:
                        pad = ScaleHelper.Scale(3);
                        middle.Y -= ScaleHelper.Scale(1);
                        arrow = new Point[]
                        {
                            new Point(middle.X - pad, middle.Y - 1),
                            new Point(middle.X + pad + 1, middle.Y - 1),
                            new Point(middle.X, middle.Y + pad)
                        };
                        break;
                }
                g.FillPolygon(brush, arrow);
            }

        }

        /// <summary>
        /// Draws arrow buttons for horizontal scroll bar.
        /// </summary>
        /// <param name="g">The <see cref="Graphics"/> used to paint.</param>
        /// <param name="rect">The rectangle in which to paint.</param>
        /// <param name="color">The color to draw the arrow buttons with.</param>
        /// <param name="arrowUp">true for an up arrow, false otherwise.</param>
        private static void DrawArrowButtonHorizontal(Graphics g, Rectangle rect, Color color, bool arrowUp)
        {
            using (Brush brush = new SolidBrush(color))
            {
                Point[] arrow;
                Int32 pad;
                Point middle = new Point(rect.Left + rect.Width / 2, rect.Top + rect.Height / 2);
                switch (arrowUp)
                {
                    case true:
                        pad = ScaleHelper.Scale(2);
                        arrow = new Point[]
                        {
                            new Point(middle.X + pad, middle.Y - 2 * pad),
                            new Point(middle.X + pad, middle.Y + 2 * pad),
                            new Point(middle.X - pad, middle.Y)
                        };
                        break;
                    default:
                        pad = ScaleHelper.Scale(2);
                        arrow = new Point[]
                        {
                            new Point(middle.X - pad, middle.Y - 2 * pad),
                            new Point(middle.X - pad, middle.Y + 2 * pad),
                            new Point(middle.X + pad, middle.Y)
                        };
                        break;
                }
                g.FillPolygon(brush, arrow);
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
        /// Is over scroll enabled? Follows large change value.
        /// </summary>
        private bool overScroll = false;

        /// <summary>
        /// The scrollbar orientation - horizontal / vertical.
        /// </summary>
        private ScrollBarOrientation orientation = ScrollBarOrientation.Vertical;

        /// <summary>
        /// The scroll orientation in scroll events.
        /// </summary>
        private ScrollOrientation scrollOrientation = ScrollOrientation.VerticalScroll;

        /// <summary>
        /// The clicked channel rectangle.
        /// </summary>
        private Rectangle clickedBarRectangle;

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
        /// The channel rectangle.
        /// </summary>
        private Rectangle channelRectangle;

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
        /// The width of the thumb.
        /// </summary>
        private int thumbWidth = ScaleHelper.Scale(13);

        /// <summary>
        /// The height of the thumb.
        /// </summary>
        private int thumbHeight;

        /// <summary>
        /// The width of an arrow.
        /// </summary>
        private int arrowWidth = ScaleHelper.Scale(13);

        /// <summary>
        /// The height of an arrow.
        /// </summary>
        private int arrowHeight = ScaleHelper.Scale(13);

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
            this.Width = ScaleHelper.Scale(17);
            this.Height = ScaleHelper.Scale(200);
            // sets the scrollbar up
            this.SetUpScrollBar();
            // timer for clicking and holding the mouse button
            // over/below the thumb and on the arrow buttons
            this.progressTimer.Interval = 20;
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
        [Description("Gets or sets the is over scroll is enabled.")]
        [DefaultValue(false)]
        public Boolean OverScroll
        {
            get
            {
                return this.overScroll;
            }
            set
            {
                // no change, return
                if (this.overScroll == value)
                {
                    return;
                }
                this.overScroll = value;
            }
        }

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
                    // current value is valid - adjust thumb position
                    this.ChangeThumbPosition(this.GetThumbPosition());
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
                    // current value is valid - adjust thumb position
                    this.ChangeThumbPosition(this.GetThumbPosition());
                    this.Refresh();
                }
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
            if (this.Scroll != null)
            {
                this.Scroll(this, e);
            }
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
            // sets the smoothing mode to none
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
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

            // check if top or bottom bar was clicked
            if (this.topBarClicked)
            {
                if (this.orientation == ScrollBarOrientation.Vertical)
                {
                    this.clickedBarRectangle.Y = this.thumbTopLimit;
                    this.clickedBarRectangle.Height = this.thumbRectangle.Y - this.thumbTopLimit;
                }
                else
                {
                    this.clickedBarRectangle.X = this.thumbTopLimit;
                    this.clickedBarRectangle.Width = this.thumbRectangle.X - this.thumbTopLimit;
                }
            }
            else if (this.bottomBarClicked)
            {
                if (this.orientation == ScrollBarOrientation.Vertical)
                {
                    this.clickedBarRectangle.Y = this.thumbRectangle.Bottom + 1;
                    this.clickedBarRectangle.Height = this.thumbBottomLimitBottom - this.clickedBarRectangle.Y + 1;
                }
                else
                {
                    this.clickedBarRectangle.X = this.thumbRectangle.Right + 1;
                    this.clickedBarRectangle.Width = this.thumbBottomLimitBottom - this.clickedBarRectangle.X + 1;
                }
            }
            // TODO: Horizontal?
            // draw current line
            if (this.curPos > -1 && this.orientation == ScrollBarOrientation.Vertical)
            {
                using (SolidBrush brush = new SolidBrush(this.curPosColor))
                {
                    Int32 position = Convert.ToInt32(GetCurPosition());
                    e.Graphics.FillRectangle(brush, 0, position, this.Width, ScaleHelper.Scale(2));
                }
            }
            // draw border
            using (Pen pen = new Pen((this.Enabled ? this.borderColor : this.borderColorDisabled)))
            {
                e.Graphics.DrawRectangle(pen, 0, 0, this.Width - 1, this.Height - 1);
            }
        }

        private int GetCurPosition()
        {
            int pixelRange, arrowSize;
            if (this.orientation == ScrollBarOrientation.Vertical)
            {
                pixelRange = this.Height - (2 * this.arrowHeight) - this.thumbHeight;
                arrowSize = this.arrowHeight;
            }
            else
            {
                pixelRange = this.Width - (2 * this.arrowWidth) - this.thumbWidth;
                arrowSize = this.arrowWidth;
            }
            int realRange = this.maximum - this.minimum;
            float perc = 0f;
            if (realRange != 0)
            {
                perc = ((float)this.curPos - (float)this.minimum) / (float)realRange;
            }
            return Math.Max(this.thumbTopLimit, Math.Min(this.thumbBottomLimitTop, Convert.ToInt32((perc * pixelRange) + arrowSize)));
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
                    {
                        this.topBarClicked = true;
                    }
                    else
                    {
                        this.bottomBarClicked = true;
                    }
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
                        int pixelRange, thumbPos, arrowSize;
                        // calculate the value - first some helper variables
                        // dependent on the current orientation
                        if (this.orientation == ScrollBarOrientation.Vertical)
                        {
                            pixelRange = this.Height - (2 * this.arrowHeight) - this.thumbHeight;
                            thumbPos = this.thumbRectangle.Y;
                            arrowSize = this.arrowHeight;
                        }
                        else
                        {
                            pixelRange = this.Width - (2 * this.arrowWidth) - this.thumbWidth;
                            thumbPos = this.thumbRectangle.X;
                            arrowSize = this.arrowWidth;
                        }
                        float perc = 0f;
                        if (pixelRange != 0)
                        {
                            // percent of the new position
                            perc = (float)(thumbPos - arrowSize) / (float)pixelRange;
                        }
                        // the new value is somewhere between max and min, starting
                        // at min position
                        this.value = Convert.ToInt32((perc * (this.maximum - this.minimum)) + this.minimum);
                    }
                    // raise scroll event if new value different
                    if (oldScrollValue != this.value)
                    {
                        this.OnScroll(new ScrollEventArgs(ScrollEventType.ThumbTrack, oldScrollValue, this.value, this.scrollOrientation));

                        this.Refresh();
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
                    if (height < (2 * this.arrowHeight) + pad)
                    {
                        height = (2 * this.arrowHeight) + pad;
                    }
                    width = ScaleHelper.Scale(17);
                }
                else
                {
                    if (width < (2 * this.arrowWidth) + pad)
                    {
                        width = (2 * this.arrowWidth) + pad;
                    }
                    height = ScaleHelper.Scale(17);
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
            if (keyData == keyUp)
            {
                this.Value -= this.smallChange;
                return true;
            }
            if (keyData == keyDown)
            {
                this.Value += this.smallChange;
                return true;
            }
            if (keyData == Keys.PageUp)
            {
                this.Value = this.GetValue(false, true);
                return true;
            }
            if (keyData == Keys.PageDown)
            {
                if (this.value + this.largeChange > this.maximum)
                {
                    this.Value = this.maximum;
                }
                else this.Value += this.largeChange;
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
            // save and mod client rectangle
            Rectangle rect = ClientRectangle;
            if (this.orientation == ScrollBarOrientation.Vertical)
            {
                rect.Height -= this.Margin.Bottom;
            }
            else rect.Width -= this.Margin.Right;
            // set up the width's, height's and rectangles for the different
            // elements
            if (this.orientation == ScrollBarOrientation.Vertical)
            {
                this.arrowHeight = ScaleHelper.Scale(13);
                this.arrowWidth = ScaleHelper.Scale(13);
                this.thumbWidth = ScaleHelper.Scale(13);
                this.thumbHeight = this.GetThumbSize();
                this.clickedBarRectangle = rect;
                this.clickedBarRectangle.Inflate(-1, -1);
                this.clickedBarRectangle.Y += this.arrowHeight;
                this.clickedBarRectangle.Height -= this.arrowHeight * 2;
                this.channelRectangle = this.clickedBarRectangle;
                this.thumbRectangle = new Rectangle(
                   rect.X + ScaleHelper.Scale(2),
                   rect.Y + this.arrowHeight + 2,
                   this.thumbWidth,
                   this.thumbHeight
                );
                this.topArrowRectangle = new Rectangle(
                   rect.X + ScaleHelper.Scale(2),
                   rect.Y + 1,
                   this.arrowWidth,
                   this.arrowHeight
                );
                this.bottomArrowRectangle = new Rectangle(
                   rect.X + ScaleHelper.Scale(2),
                   rect.Bottom - this.arrowHeight - 1,
                   this.arrowWidth,
                   this.arrowHeight
                );
                // Set the default starting thumb position.
                //this.thumbPosition = this.thumbRectangle.Height / 2;
                // Set the bottom limit of the thumb's bottom border.
                this.thumbBottomLimitBottom = rect.Bottom - this.arrowHeight - ScaleHelper.Scale(4);
                // Set the bottom limit of the thumb's top border.
                this.thumbBottomLimitTop = this.thumbBottomLimitBottom - this.thumbRectangle.Height;
                // Set the top limit of the thumb's top border.
                this.thumbTopLimit = rect.Y + this.arrowHeight + ScaleHelper.Scale(3);
            }
            else
            {
                this.arrowHeight = ScaleHelper.Scale(13);
                this.arrowWidth = ScaleHelper.Scale(13);
                this.thumbHeight = ScaleHelper.Scale(13);
                this.thumbWidth = this.GetThumbSize();
                this.clickedBarRectangle = rect;
                this.clickedBarRectangle.Inflate(-1, -1);
                this.clickedBarRectangle.X += this.arrowWidth;
                this.clickedBarRectangle.Width -= this.arrowWidth * 2;
                this.channelRectangle = this.clickedBarRectangle;
                this.thumbRectangle = new Rectangle(
                   rect.X + this.arrowWidth + 2,
                   rect.Y + ScaleHelper.Scale(2),
                   this.thumbWidth,
                   this.thumbHeight
                );
                this.topArrowRectangle = new Rectangle(
                   rect.X + 1,
                   rect.Y + ScaleHelper.Scale(2),
                   this.arrowWidth,
                   this.arrowHeight
                );
                this.bottomArrowRectangle = new Rectangle(
                   rect.Right - this.arrowWidth + 1,
                   rect.Y + ScaleHelper.Scale(2),
                   this.arrowWidth,
                   this.arrowHeight
                );
                // Set the default starting thumb position.
                //this.thumbPosition = this.thumbRectangle.Width / 2;
                // Set the bottom limit of the thumb's bottom border.
                this.thumbBottomLimitBottom = rect.Right - this.arrowWidth - ScaleHelper.Scale(3);
                // Set the bottom limit of the thumb's top border.
                this.thumbBottomLimitTop = this.thumbBottomLimitBottom - this.thumbRectangle.Width;
                // Set the top limit of the thumb's top border.
                this.thumbTopLimit = rect.X + this.arrowWidth + ScaleHelper.Scale(3);
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
            int newValue;
            // calculate the new value of the scrollbar
            // with checking if new value is in bounds (min/max)
            if (up)
            {
                newValue = this.value - (smallIncrement ? this.smallChange : this.largeChange);
                if (newValue < this.minimum)
                {
                    newValue = this.minimum;
                }
            }
            else
            {
                newValue = this.value + (smallIncrement ? this.smallChange : this.largeChange);
                if (newValue > this.maximum)
                {
                    newValue = this.maximum;
                }
            }
            return newValue;
        }

        /// <summary>
        /// Calculates the new thumb position.
        /// </summary>
        /// <returns>The new thumb position.</returns>
        private int GetThumbPosition()
        {
            int pixelRange, arrowSize;
            if (this.orientation == ScrollBarOrientation.Vertical)
            {
                pixelRange = this.Height - (2 * this.arrowHeight) - this.thumbHeight;
                arrowSize = this.arrowHeight;
            }
            else
            {
                pixelRange = this.Width - (2 * this.arrowWidth) - this.thumbWidth;
                arrowSize = this.arrowWidth;
            }
            int realRange = this.maximum - this.minimum;
            float perc = 0f;
            if (realRange != 0)
            {
                perc = ((float)this.value - (float)this.minimum) / (float)realRange;
            }
            return Math.Max(this.thumbTopLimit, Math.Min(this.thumbBottomLimitTop, Convert.ToInt32((perc * pixelRange) + arrowSize)));
        }

        /// <summary>
        /// Calculates the height of the thumb.
        /// </summary>
        /// <returns>The height of the thumb.</returns>
        private int GetThumbSize()
        {
            int trackSize = this.orientation == ScrollBarOrientation.Vertical ? this.Height - (2 * this.arrowHeight) : this.Width - (2 * this.arrowWidth);
            if (this.maximum == 0 || this.largeChange == 0)
            {
                return trackSize;
            }
            float newThumbSize = ((float)this.largeChange * (float)trackSize) / (float)(this.overScroll ? (float)(this.maximum + this.largeChange) : this.maximum);
            return Convert.ToInt32(Math.Min((float)trackSize, Math.Max(newThumbSize, 10f)));
        }

        /// <summary>
        /// Enables the timer.
        /// </summary>
        private void EnableTimer()
        {
            // if timer is not already enabled - enable it
            if (!this.progressTimer.Enabled)
            {
                this.progressTimer.Interval = 600;
                this.progressTimer.Start();
            }
            else
            {
                // if already enabled, change tick time
                this.progressTimer.Interval = 10;
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
            {
                this.thumbRectangle.Y = position;
            }
            else this.thumbRectangle.X = position;
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
            if (this.orientation == ScrollBarOrientation.Vertical)
            {
                thumbPos = this.thumbRectangle.Y;
                thumbSize = this.thumbRectangle.Height;
            }
            else
            {
                thumbPos = this.thumbRectangle.X;
                thumbSize = this.thumbRectangle.Width;
            }
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
                this.Invalidate(this.channelRectangle);
                if (enableTimer) this.EnableTimer();
            }
            else
            {
                if (this.topArrowClicked)
                {
                    type = ScrollEventType.SmallDecrement;
                }
                else if (this.bottomArrowClicked)
                {
                    type = ScrollEventType.SmallIncrement;
                }
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
            int thumbSize, thumbPos, arrowSize, size;
            if (this.orientation == ScrollBarOrientation.Vertical)
            {
                thumbSize = this.thumbHeight;
                arrowSize = this.arrowHeight;
                size = this.Height;
                this.ChangeThumbPosition(Math.Max(this.thumbTopLimit, Math.Min(this.thumbBottomLimitTop, this.trackPosition - (this.thumbRectangle.Height / 2))));
                thumbPos = this.thumbRectangle.Y;
            }
            else
            {
                thumbSize = this.thumbWidth;
                arrowSize = this.arrowWidth;
                size = this.Width;
                this.ChangeThumbPosition(Math.Max(this.thumbTopLimit, Math.Min(this.thumbBottomLimitTop, this.trackPosition - (this.thumbRectangle.Width / 2))));
                thumbPos = this.thumbRectangle.X;
            }
            int pixelRange = size - (2 * arrowSize) - thumbSize;
            float perc = 0f;
            if (pixelRange != 0)
            {
                perc = (float)(thumbPos - arrowSize) / (float)pixelRange;
            }
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

    public class ListViewScroller : IEventHandler, IDisposable
    {
        private ListView listView;
        private ScrollBarEx vScrollBar;
        private ScrollBarEx hScrollBar;

        /// <summary>
        /// Initialize ListViewScroller
        /// </summary>
        public ListViewScroller(ListView view)
        {
            listView = view;
            InitScrollBars();
        }

        /// <summary>
        /// Init the custom scrollbars
        /// </summary>
        public void InitScrollBars()
        {
            vScrollBar = new ScrollBarEx();
            vScrollBar.OverScroll = true;
            vScrollBar.Width = ScaleHelper.Scale(SystemInformation.VerticalScrollBarWidth);
            vScrollBar.Orientation = ScrollBarOrientation.Vertical;
            vScrollBar.ContextMenuStrip.Renderer = new DockPanelStripRenderer();
            vScrollBar.Margin = new Padding(0, 0, 0, ScaleHelper.Scale(SystemInformation.HorizontalScrollBarHeight));
            hScrollBar = new ScrollBarEx();
            hScrollBar.OverScroll = true;
            hScrollBar.Height = ScaleHelper.Scale(SystemInformation.HorizontalScrollBarHeight);
            hScrollBar.Orientation = ScrollBarOrientation.Horizontal;
            hScrollBar.ContextMenuStrip.Renderer = new DockPanelStripRenderer();
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
        public void AddScrollBars()
        {
            listView.Parent.Controls.Add(hScrollBar);
            listView.Parent.Controls.Add(vScrollBar);
            vScrollBar.Scroll += OnScrollBarScroll;
            hScrollBar.Scroll += OnScrollBarScroll;
            vScrollBar.VisibleChanged += delegate { OnListViewResize(null, null); };
            hScrollBar.VisibleChanged += delegate { OnListViewResize(null, null); };
            listView.Paint += delegate { UpdateScrollState(); };
            listView.Resize += OnListViewResize;
        }

        /// <summary>
        /// Remove controls from container
        /// </summary>
        private void RemoveScrollBars()
        {
            listView.Parent.Controls.Remove(hScrollBar);
            listView.Parent.Controls.Remove(vScrollBar);
            vScrollBar.Scroll -= OnScrollBarScroll;
            hScrollBar.Scroll -= OnScrollBarScroll;
            vScrollBar.VisibleChanged -= delegate { OnListViewResize(null, null); };
            hScrollBar.VisibleChanged -= delegate { OnListViewResize(null, null); };
            listView.Paint -= delegate { UpdateScrollState(); };
            listView.Resize -= OnListViewResize;
        }

        /// <summary>
        /// Handle the incoming theme events
        /// </summary>
        public void HandleEvent(Object sender, NotifyEvent e, HandlingPriority priority)
        {
            if (e.Type == EventType.ApplyTheme)
            {
                Boolean enabled = PluginBase.MainForm.GetThemeFlag("ScrollBar.UseGlobally", false);
                if (enabled && !listView.Parent.Controls.Contains(vScrollBar))
                {
                    AddScrollBars();
                    UpdateScrollBarTheme();
                }
                else if (!enabled && listView.Parent.Controls.Contains(vScrollBar))
                {
                    RemoveScrollBars();
                }
            }
        }

        /// <summary>
        /// Updates the scrollbar theme and applies old defaults
        /// </summary>
        private void UpdateScrollBarTheme()
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
        }

        /// <summary>
        /// Updates the scrollbar scroll states
        /// </summary>
        private void UpdateScrollState()
        {
            Win32.SCROLLINFO vScroll = Win32.GetFullScrollInfo(listView, false);
            Win32.SCROLLINFO hScroll = Win32.GetFullScrollInfo(listView, true);
            vScrollBar.Visible = vScroll.nMax > (vScroll.nPage - 1) && vScroll.nPage > 0;
            hScrollBar.Visible = hScroll.nMax > (hScroll.nPage - 1) && hScroll.nPage > 0;
            UpdateScrollBarMargins();
            vScrollBar.Scroll -= OnScrollBarScroll;
            vScrollBar.Minimum = vScroll.nMin;
            vScrollBar.Maximum = vScroll.nMax - (vScroll.nPage - 1);
            vScrollBar.LargeChange = vScroll.nPage - 1;
            vScrollBar.Value = vScroll.nPos;
            vScrollBar.Scroll += OnScrollBarScroll;
            hScrollBar.Scroll -= OnScrollBarScroll;
            hScrollBar.Minimum = hScroll.nMin;
            hScrollBar.Maximum = hScroll.nMax - (hScroll.nPage - 1);
            hScrollBar.LargeChange = (hScroll.nPage - 1);
            hScrollBar.Value = hScroll.nPos;
            hScrollBar.Scroll += OnScrollBarScroll;
        }

        /// <summary>
        /// Update the scrollbar margins
        /// </summary>
        private void UpdateScrollBarMargins()
        {
            if (!hScrollBar.Visible) vScrollBar.Margin = new Padding(0, 0, 0, 0);
            else vScrollBar.Margin = new Padding(0, 0, 0, ScaleHelper.Scale(SystemInformation.HorizontalScrollBarHeight));
        }

        /// <summary>
        /// Updates the scrollbars on listview resize
        /// </summary>
        private void OnListViewResize(Object sender, EventArgs e)
        {
            vScrollBar.BringToFront();
            vScrollBar.Location = new Point(listView.Location.X + listView.Width - vScrollBar.Width, listView.Location.Y);
            vScrollBar.Height = listView.Height;
            hScrollBar.BringToFront();
            hScrollBar.Location = new Point(listView.Location.X, listView.Location.Y + listView.Height - hScrollBar.Height);
            hScrollBar.Width = listView.Width - (vScrollBar.Visible ? vScrollBar.Width : 0);
            listView.Invalidate();
        }

        /// <summary>
        /// Updates the listView on scrollbar scroll
        /// </summary>
        private void OnScrollBarScroll(Object sender, ScrollEventArgs e)
        {
            if (e.OldValue == -1 || listView.Items.Count == 0) return;
            Int32 height = listView.GetItemRect(0).Height; // Item height in pixels
            if (e.ScrollOrientation == ScrollOrientation.VerticalScroll)
            {
                Int32 vScroll = -(e.OldValue - e.NewValue) * height;
                Win32.SendMessage(listView.Handle, (Int32)Win32.LVM_SCROLL, IntPtr.Zero, (IntPtr)vScroll);
            }
            else
            {
                Int32 hScroll = -(e.OldValue - e.NewValue);
                Win32.SendMessage(listView.Handle, (Int32)Win32.LVM_SCROLL, (IntPtr)hScroll, IntPtr.Zero);
            }
        }

        /// <summary>
        /// Dispose the controls
        /// </summary>
        void IDisposable.Dispose()
        {
            listView = null;
            vScrollBar.Dispose();
            hScrollBar.Dispose();
        }

    }

    public class TreeViewScroller : IEventHandler, IDisposable
    {
        private TreeView treeView;
        private ScrollBarEx vScrollBar;
        private ScrollBarEx hScrollBar;

        /// <summary>
        /// Initialize TreeViewScroller
        /// </summary>
        public TreeViewScroller(TreeView view)
        {
            treeView = view;
            InitScrollBars();
        }

        /// <summary>
        /// Init the custom scrollbars
        /// </summary>
        public void InitScrollBars()
        {
            vScrollBar = new ScrollBarEx();
            vScrollBar.OverScroll = true;
            vScrollBar.Width = ScaleHelper.Scale(SystemInformation.VerticalScrollBarWidth);
            vScrollBar.Orientation = ScrollBarOrientation.Vertical;
            vScrollBar.ContextMenuStrip.Renderer = new DockPanelStripRenderer();
            vScrollBar.Margin = new Padding(0, 0, 0, ScaleHelper.Scale(SystemInformation.HorizontalScrollBarHeight));
            hScrollBar = new ScrollBarEx();
            hScrollBar.OverScroll = true;
            hScrollBar.Height = ScaleHelper.Scale(SystemInformation.HorizontalScrollBarHeight);
            hScrollBar.Orientation = ScrollBarOrientation.Horizontal;
            hScrollBar.ContextMenuStrip.Renderer = new DockPanelStripRenderer();
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
        public void AddScrollBars()
        {
            treeView.Parent.Controls.Add(hScrollBar);
            treeView.Parent.Controls.Add(vScrollBar);
            vScrollBar.Scroll += OnScrollBarScroll;
            hScrollBar.Scroll += OnScrollBarScroll;
            vScrollBar.VisibleChanged += delegate { OnTreeViewResize(null, null); };
            hScrollBar.VisibleChanged += delegate { OnTreeViewResize(null, null); };
            treeView.Paint += delegate { UpdateScrollState(); };
            treeView.Resize += OnTreeViewResize;
        }

        /// <summary>
        /// Remove controls from container
        /// </summary>
        private void RemoveScrollBars()
        {
            treeView.Parent.Controls.Remove(hScrollBar);
            treeView.Parent.Controls.Remove(vScrollBar);
            vScrollBar.Scroll -= OnScrollBarScroll;
            hScrollBar.Scroll -= OnScrollBarScroll;
            vScrollBar.VisibleChanged -= delegate { OnTreeViewResize(null, null); };
            hScrollBar.VisibleChanged -= delegate { OnTreeViewResize(null, null); };
            treeView.Paint -= delegate { UpdateScrollState(); };
            treeView.Resize -= OnTreeViewResize;
        }

        /// <summary>
        /// Handle the incoming theme events
        /// </summary>
        public void HandleEvent(Object sender, NotifyEvent e, HandlingPriority priority)
        {
            if (e.Type == EventType.ApplyTheme)
            {
                Boolean enabled = PluginBase.MainForm.GetThemeFlag("ScrollBar.UseGlobally", false);
                if (enabled && !treeView.Parent.Controls.Contains(vScrollBar))
                {
                    AddScrollBars();
                    UpdateScrollBarTheme();
                }
                else if (!enabled && treeView.Parent.Controls.Contains(vScrollBar))
                {
                    RemoveScrollBars();
                }
            }
        }

        /// <summary>
        /// Updates the scrollbar theme and applies old defaults
        /// </summary>
        private void UpdateScrollBarTheme()
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
        }

        /// <summary>
        /// Updates the scrollbar scroll states
        /// </summary>
        private void UpdateScrollState()
        {
            Win32.SCROLLINFO vScroll = Win32.GetFullScrollInfo(treeView, false);
            Win32.SCROLLINFO hScroll = Win32.GetFullScrollInfo(treeView, true);
            vScrollBar.Visible = vScroll.nMax > (vScroll.nPage - 1) && vScroll.nPage > 0;
            hScrollBar.Visible = hScroll.nMax > (hScroll.nPage - 1) && hScroll.nPage > 0;
            UpdateScrollBarMargins();
            vScrollBar.Scroll -= OnScrollBarScroll;
            vScrollBar.Minimum = vScroll.nMin;
            vScrollBar.Maximum = vScroll.nMax - (vScroll.nPage - 1);
            vScrollBar.LargeChange = vScroll.nPage - 1;
            vScrollBar.Value = vScroll.nPos;
            vScrollBar.Scroll += OnScrollBarScroll;
            hScrollBar.Scroll -= OnScrollBarScroll;
            hScrollBar.Minimum = hScroll.nMin;
            hScrollBar.Maximum = hScroll.nMax - (hScroll.nPage - 1);
            hScrollBar.LargeChange = (hScroll.nPage - 1);
            hScrollBar.Value = hScroll.nPos;
            hScrollBar.Scroll += OnScrollBarScroll;
        }

        /// <summary>
        /// Update the scrollbar margins
        /// </summary>
        private void UpdateScrollBarMargins()
        {
            if (!hScrollBar.Visible) vScrollBar.Margin = new Padding(0, 0, 0, 0);
            else vScrollBar.Margin = new Padding(0, 0, 0, ScaleHelper.Scale(SystemInformation.HorizontalScrollBarHeight));
        }

        /// <summary>
        /// Updates the scrollbars on treeView resize
        /// </summary>
        private void OnTreeViewResize(Object sender, EventArgs e)
        {
            vScrollBar.BringToFront();
            vScrollBar.Location = new Point(treeView.Location.X + treeView.Width - vScrollBar.Width, treeView.Location.Y);
            vScrollBar.Height = treeView.Height;
            hScrollBar.BringToFront();
            hScrollBar.Location = new Point(treeView.Location.X, treeView.Location.Y + treeView.Height - hScrollBar.Height);
            hScrollBar.Width = treeView.Width - (vScrollBar.Visible ? vScrollBar.Width : 0);
            treeView.Invalidate();
        }

        /// <summary>
        /// Updates the treeView on scrollbar scroll
        /// </summary>
        private void OnScrollBarScroll(Object sender, ScrollEventArgs e)
        {
            if (e.OldValue == -1 || treeView.Nodes.Count == 0) return;
            if (e.ScrollOrientation == ScrollOrientation.VerticalScroll)
            {
                treeView.BeginUpdate();
                Win32.SetScrollPos(treeView.Handle, Win32.SB_VERT, e.NewValue, true);
                treeView.EndUpdate();
            }
            else
            {
                treeView.BeginUpdate();
                Win32.SetScrollPos(treeView.Handle, Win32.SB_HORZ, e.NewValue, true);
                treeView.EndUpdate();
            }
        }

        /// <summary>
        /// Dispose the controls
        /// </summary>
        void IDisposable.Dispose()
        {
            treeView = null;
            vScrollBar.Dispose();
            hScrollBar.Dispose();
        }

    }

    public class RichTextBoxScroller : IEventHandler, IDisposable
    {
        private RichTextBox richTextBox;
        private ScrollBarEx vScrollBar;
        private ScrollBarEx hScrollBar;

        /// <summary>
        /// Initialize TreeViewScroller
        /// </summary>
        public RichTextBoxScroller(RichTextBox view)
        {
            richTextBox = view;
            InitScrollBars();
        }

        /// <summary>
        /// Init the custom scrollbars
        /// </summary>
        public void InitScrollBars()
        {
            vScrollBar = new ScrollBarEx();
            vScrollBar.OverScroll = true;
            vScrollBar.Width = ScaleHelper.Scale(SystemInformation.VerticalScrollBarWidth);
            vScrollBar.Orientation = ScrollBarOrientation.Vertical;
            vScrollBar.ContextMenuStrip.Renderer = new DockPanelStripRenderer();
            vScrollBar.Margin = new Padding(0, 0, 0, ScaleHelper.Scale(SystemInformation.HorizontalScrollBarHeight));
            hScrollBar = new ScrollBarEx();
            hScrollBar.OverScroll = true;
            hScrollBar.Height = ScaleHelper.Scale(SystemInformation.HorizontalScrollBarHeight);
            hScrollBar.Orientation = ScrollBarOrientation.Horizontal;
            hScrollBar.ContextMenuStrip.Renderer = new DockPanelStripRenderer();
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
        public void AddScrollBars()
        {
            richTextBox.Parent.Controls.Add(hScrollBar);
            richTextBox.Parent.Controls.Add(vScrollBar);
            vScrollBar.Scroll += OnScrollBarScroll;
            hScrollBar.Scroll += OnScrollBarScroll;
            vScrollBar.VisibleChanged += delegate { OnRichTextBoxResize(null, null); };
            hScrollBar.VisibleChanged += delegate { OnRichTextBoxResize(null, null); };
            richTextBox.Paint += delegate { UpdateScrollState(); };
            richTextBox.Resize += OnRichTextBoxResize;
        }

        /// <summary>
        /// Remove controls from container
        /// </summary>
        private void RemoveScrollBars()
        {
            richTextBox.Parent.Controls.Remove(hScrollBar);
            richTextBox.Parent.Controls.Remove(vScrollBar);
            vScrollBar.Scroll -= OnScrollBarScroll;
            hScrollBar.Scroll -= OnScrollBarScroll;
            vScrollBar.VisibleChanged -= delegate { OnRichTextBoxResize(null, null); };
            hScrollBar.VisibleChanged -= delegate { OnRichTextBoxResize(null, null); };
            richTextBox.Paint -= delegate { UpdateScrollState(); };
            richTextBox.Resize -= OnRichTextBoxResize;
        }

        /// <summary>
        /// Handle the incoming theme events
        /// </summary>
        public void HandleEvent(Object sender, NotifyEvent e, HandlingPriority priority)
        {
            if (e.Type == EventType.ApplyTheme)
            {
                Boolean enabled = PluginBase.MainForm.GetThemeFlag("ScrollBar.UseGlobally", false);
                if (enabled && !richTextBox.Parent.Controls.Contains(vScrollBar))
                {
                    AddScrollBars();
                    UpdateScrollBarTheme();
                }
                else if (!enabled && richTextBox.Parent.Controls.Contains(vScrollBar))
                {
                    RemoveScrollBars();
                }
            }
        }

        /// <summary>
        /// Updates the scrollbar theme and applies old defaults
        /// </summary>
        private void UpdateScrollBarTheme()
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
        }

        /// <summary>
        /// Updates the scrollbar scroll states
        /// </summary>
        private void UpdateScrollState()
        {
            Win32.SCROLLINFO vScroll = Win32.GetFullScrollInfo(richTextBox, false);
            Win32.SCROLLINFO hScroll = Win32.GetFullScrollInfo(richTextBox, true);
            vScrollBar.Visible = vScroll.nMax > (vScroll.nPage - 1) && vScroll.nPage > 0;
            hScrollBar.Visible = hScroll.nMax > (hScroll.nPage - 1) && hScroll.nPage > 0;
            UpdateScrollBarMargins();
            vScrollBar.Scroll -= OnScrollBarScroll;
            vScrollBar.Minimum = vScroll.nMin;
            vScrollBar.Maximum = vScroll.nMax - (vScroll.nPage - 1);
            vScrollBar.LargeChange = vScroll.nPage - 1;
            vScrollBar.Value = vScroll.nPos;
            vScrollBar.Scroll += OnScrollBarScroll;
            hScrollBar.Scroll -= OnScrollBarScroll;
            hScrollBar.Minimum = hScroll.nMin;
            hScrollBar.Maximum = hScroll.nMax - (hScroll.nPage - 1);
            hScrollBar.LargeChange = (hScroll.nPage - 1);
            hScrollBar.Value = hScroll.nPos;
            hScrollBar.Scroll += OnScrollBarScroll;
        }

        /// <summary>
        /// Update the scrollbar margins
        /// </summary>
        private void UpdateScrollBarMargins()
        {
            if (!hScrollBar.Visible) vScrollBar.Margin = new Padding(0, 0, 0, 0);
            else vScrollBar.Margin = new Padding(0, 0, 0, ScaleHelper.Scale(SystemInformation.HorizontalScrollBarHeight));
        }

        /// <summary>
        /// Updates the scrollbars on richTextBox resize
        /// </summary>
        private void OnRichTextBoxResize(Object sender, EventArgs e)
        {
            vScrollBar.BringToFront();
            vScrollBar.Location = new Point(richTextBox.Location.X + richTextBox.Width - vScrollBar.Width, richTextBox.Location.Y);
            vScrollBar.Height = richTextBox.Height;
            hScrollBar.BringToFront();
            hScrollBar.Location = new Point(richTextBox.Location.X, richTextBox.Location.Y + richTextBox.Height - hScrollBar.Height);
            hScrollBar.Width = richTextBox.Width - (vScrollBar.Visible ? vScrollBar.Width : 0);
            richTextBox.Invalidate();
        }

        /// <summary>
        /// Updates the richTextBox on scrollbar scroll
        /// </summary>
        private void OnScrollBarScroll(Object sender, ScrollEventArgs e)
        {
            if (e.OldValue == -1 || richTextBox.Lines.Length == 0) return;
            if (e.ScrollOrientation == ScrollOrientation.VerticalScroll)
            {
                int wParam = Win32.SB_THUMBPOSITION | e.NewValue << 16;
                Win32.SendMessage(richTextBox.Handle, Win32.WM_VSCROLL, (IntPtr)wParam, IntPtr.Zero);
            }
            else
            {
                int wParam = Win32.SB_THUMBPOSITION | e.NewValue << 16;
                Win32.SendMessage(richTextBox.Handle, Win32.WM_HSCROLL, (IntPtr)wParam, IntPtr.Zero);
            }
        }

        /// <summary>
        /// Dispose the controls
        /// </summary>
        void IDisposable.Dispose()
        {
            richTextBox = null;
            vScrollBar.Dispose();
            hScrollBar.Dispose();
        }

    }

    public class DataGridViewScroller : IEventHandler, IDisposable
    {
        private DataGridView dataGridView;
        private ScrollBarEx vScrollBar;
        private ScrollBarEx hScrollBar;

        /// <summary>
        /// Initialize DataGridViewScroller
        /// </summary>
        public DataGridViewScroller(DataGridView view)
        {
            dataGridView = view;
            InitScrollBars();
        }

        /// <summary>
        /// Init the custom scrollbars
        /// </summary>
        public void InitScrollBars()
        {
            vScrollBar = new ScrollBarEx();
            vScrollBar.OverScroll = true;
            vScrollBar.Width = ScaleHelper.Scale(SystemInformation.VerticalScrollBarWidth);
            vScrollBar.Orientation = ScrollBarOrientation.Vertical;
            vScrollBar.ContextMenuStrip.Renderer = new DockPanelStripRenderer();
            vScrollBar.Margin = new Padding(0, 0, 0, ScaleHelper.Scale(SystemInformation.HorizontalScrollBarHeight));
            hScrollBar = new ScrollBarEx();
            hScrollBar.OverScroll = true;
            hScrollBar.Height = ScaleHelper.Scale(SystemInformation.HorizontalScrollBarHeight);
            hScrollBar.Orientation = ScrollBarOrientation.Horizontal;
            hScrollBar.ContextMenuStrip.Renderer = new DockPanelStripRenderer();
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
        public void AddScrollBars()
        {
            dataGridView.Parent.Controls.Add(hScrollBar);
            dataGridView.Parent.Controls.Add(vScrollBar);
            vScrollBar.Scroll += OnScrollBarScroll;
            hScrollBar.Scroll += OnScrollBarScroll;
            vScrollBar.VisibleChanged += delegate { OnDataGridViewResize(null, null); };
            hScrollBar.VisibleChanged += delegate { OnDataGridViewResize(null, null); };
            dataGridView.Paint += delegate { UpdateScrollState(); };
            dataGridView.Resize += OnDataGridViewResize;
        }

        /// <summary>
        /// Remove controls from container
        /// </summary>
        private void RemoveScrollBars()
        {
            dataGridView.Parent.Controls.Remove(hScrollBar);
            dataGridView.Parent.Controls.Remove(vScrollBar);
            vScrollBar.Scroll -= OnScrollBarScroll;
            hScrollBar.Scroll -= OnScrollBarScroll;
            vScrollBar.VisibleChanged -= delegate { OnDataGridViewResize(null, null); };
            hScrollBar.VisibleChanged -= delegate { OnDataGridViewResize(null, null); };
            dataGridView.Paint -= delegate { UpdateScrollState(); };
            dataGridView.Resize -= OnDataGridViewResize;
        }

        /// <summary>
        /// Handle the incoming theme events
        /// </summary>
        public void HandleEvent(Object sender, NotifyEvent e, HandlingPriority priority)
        {
            if (e.Type == EventType.ApplyTheme)
            {
                Boolean enabled = PluginBase.MainForm.GetThemeFlag("ScrollBar.UseGlobally", false);
                if (enabled && !dataGridView.Parent.Controls.Contains(vScrollBar))
                {
                    AddScrollBars();
                    UpdateScrollBarTheme();
                }
                else if (!enabled && dataGridView.Parent.Controls.Contains(vScrollBar))
                {
                    RemoveScrollBars();
                }
            }
        }

        /// <summary>
        /// Updates the scrollbar theme and applies old defaults
        /// </summary>
        private void UpdateScrollBarTheme()
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
        }

        /// <summary>
        /// Updates the scrollbar scroll states
        /// </summary>
        private void UpdateScrollState()
        {
            Int32 vScrollMax = dataGridView.RowCount;
            Int32 vScrollPos = dataGridView.FirstDisplayedScrollingRowIndex;
            Int32 vScrollPage = dataGridView.DisplayedRowCount(false);
            Int32 hScrollMax = dataGridView.ColumnCount;
            Int32 hScrollPos = dataGridView.FirstDisplayedScrollingColumnIndex;
            Int32 hScrollPage = dataGridView.DisplayedColumnCount(false);
            vScrollBar.Visible = vScrollMax > (vScrollPage - 1) && vScrollMax != vScrollPage;
            hScrollBar.Visible = hScrollMax > (hScrollPage - 1) && hScrollMax != hScrollPage;
            UpdateScrollBarMargins();
            vScrollBar.Scroll -= OnScrollBarScroll;
            vScrollBar.Minimum = 0;
            vScrollBar.Maximum = vScrollMax - (vScrollPage);
            vScrollBar.LargeChange = vScrollPage - 1;
            vScrollBar.Value = vScrollPos;
            vScrollBar.Scroll += OnScrollBarScroll;
            hScrollBar.Scroll -= OnScrollBarScroll;
            hScrollBar.Minimum = 0;
            hScrollBar.Maximum = hScrollMax - (hScrollPage - 1);
            hScrollBar.LargeChange = (hScrollPage - 1);
            hScrollBar.Value = hScrollPos;
            hScrollBar.Scroll += OnScrollBarScroll;
        }

        /// <summary>
        /// Update the scrollbar margins
        /// </summary>
        private void UpdateScrollBarMargins()
        {
            if (!hScrollBar.Visible) vScrollBar.Margin = new Padding(0, 0, 0, 0);
            else vScrollBar.Margin = new Padding(0, 0, 0, ScaleHelper.Scale(SystemInformation.HorizontalScrollBarHeight));
        }

        /// <summary>
        /// Updates the scrollbars on dataGridView resize
        /// </summary>
        private void OnDataGridViewResize(Object sender, EventArgs e)
        {
            vScrollBar.BringToFront();
            vScrollBar.Location = new Point(dataGridView.Location.X + dataGridView.Width - vScrollBar.Width, dataGridView.Location.Y);
            vScrollBar.Height = dataGridView.Height;
            hScrollBar.BringToFront();
            hScrollBar.Location = new Point(dataGridView.Location.X, dataGridView.Location.Y + dataGridView.Height - hScrollBar.Height);
            hScrollBar.Width = dataGridView.Width - (vScrollBar.Visible ? vScrollBar.Width : 0);
            dataGridView.Invalidate();
        }

        /// <summary>
        /// Updates the dataGridView on scrollbar scroll
        /// </summary>
        private void OnScrollBarScroll(Object sender, ScrollEventArgs e)
        {
            if (e.OldValue == -1 || dataGridView.RowCount == 0) return;
            if (e.ScrollOrientation == ScrollOrientation.VerticalScroll) dataGridView.FirstDisplayedScrollingRowIndex = e.NewValue;
            else dataGridView.FirstDisplayedScrollingColumnIndex = e.NewValue;
        }

        /// <summary>
        /// Dispose the controls
        /// </summary>
        void IDisposable.Dispose()
        {
            dataGridView = null;
            vScrollBar.Dispose();
            hScrollBar.Dispose();
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
