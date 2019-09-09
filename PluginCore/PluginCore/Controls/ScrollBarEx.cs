// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
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
        public static IDisposable Attach(object obj) => Attach(obj, false);

        public static IDisposable Attach(object obj, bool childrenToo)
        {
            if (!Win32.ShouldUseWin32() && !PluginBase.MainForm.GetThemeFlag("ScrollBar.UseGlobally", false)) return null;
            if (obj is Control parent && childrenToo)
            {
                foreach (Control ctrl in parent.Controls) Attach(ctrl);
                return null;
            }
            if (obj is ListBox listBox) return new ListBoxScroller(listBox);
            if (obj is ListView listView) return new ListViewScroller(listView);
            if (obj is TreeView treeView) return new TreeViewScroller(treeView);
            if (obj is RichTextBox richTextBox) return new RichTextBoxScroller(richTextBox);
            if (obj is DataGridView dataGridView) return new DataGridViewScroller(dataGridView);
            if (obj is PropertyGrid propertyGrid) return new PropertyGridScroller(propertyGrid);
            if (obj is TextBox textBox && textBox.Multiline && textBox.WordWrap)
            {
                return new TextBoxScroller(textBox as TextBoxEx);
            }
            return null;
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
        const int PROGRESS_TIMER_DELAY = 300;

        /// <summary>
        /// Auto-repeat interval.
        /// </summary>
        const int PROGRESS_TIMER_TICK = 33;
        #endregion

        #region Drawing

        Color curPosColor = Color.DarkBlue;
        Color foreColor = SystemColors.ControlDarkDark;
        Color foreColorHot = SystemColors.Highlight;
        Color foreColorPressed = SystemColors.HotTrack;
        Color arrowColor = SystemColors.ControlDarkDark;
        Color arrowColorHot = SystemColors.Highlight;
        Color arrowColorPressed = SystemColors.HotTrack;
        Color backColor = SystemColors.ActiveBorder;
        Color backColorDisabled = SystemColors.ControlLight;
        Color borderColor = SystemColors.ActiveBorder;
        Color borderColorDisabled = SystemColors.Control;

        /// <summary>
        /// Draws the background.
        /// </summary>
        /// <param name="g">The <see cref="Graphics"/> used to paint.</param>
        /// <param name="rect">The rectangle in which to paint.</param>
        /// <param name="orientation">The <see cref="ScrollBarOrientation"/>.</param>
        void DrawBackground(Graphics g, Rectangle rect, ScrollBarOrientation orientation)
        {
            if (g is null) throw new ArgumentNullException(nameof(g));

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
        void DrawThumb(Graphics g, Rectangle rect, ScrollBarState state, ScrollBarOrientation orientation)
        {
            if (g is null) throw new ArgumentNullException(nameof(g));

            if (rect.IsEmpty || g.IsVisibleClipEmpty || !g.VisibleClipBounds.IntersectsWith(rect) || state == ScrollBarState.Disabled)
                return;
            var color = state switch
            {
                ScrollBarState.Hot => foreColorHot,
                ScrollBarState.Pressed => foreColorPressed,
                _ => foreColor,
            };
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
        void DrawArrowButton(Graphics g, Rectangle rect, ScrollBarArrowButtonState state, bool arrowUp, ScrollBarOrientation orientation)
        {
            if (g is null) throw new ArgumentNullException(nameof(g));

            if (rect.IsEmpty || g.IsVisibleClipEmpty || !g.VisibleClipBounds.IntersectsWith(rect))
                return;

            var color = state switch
            {
                ScrollBarArrowButtonState.UpHot => arrowColorHot,
                ScrollBarArrowButtonState.DownHot => arrowColorHot,
                ScrollBarArrowButtonState.UpPressed => arrowColorPressed,
                ScrollBarArrowButtonState.DownPressed => arrowColorPressed,
                _ => arrowColor,
            };
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
        void DrawBackgroundVertical(Graphics g, Rectangle rect)
        {
            using Brush brush = new SolidBrush(Enabled ? backColor : backColorDisabled);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
            g.FillRectangle(brush, rect);
        }

        /// <summary>
        /// Draws the background.
        /// </summary>
        /// <param name="g">The <see cref="Graphics"/> used to paint.</param>
        /// <param name="rect">The rectangle in which to paint.</param>
        void DrawBackgroundHorizontal(Graphics g, Rectangle rect)
        {
            using Brush brush = new SolidBrush(Enabled ? backColor : backColorDisabled);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
            g.FillRectangle(brush, rect);
        }

        /// <summary>
        /// Draws the vertical thumb.
        /// </summary>
        /// <param name="g">The <see cref="Graphics"/> used to paint.</param>
        /// <param name="rect">The rectangle in which to paint.</param>
        /// <param name="color">The color to draw the thumb with.</param>
        static void DrawThumbVertical(Graphics g, Rectangle rect, Color color)
        {
            using Brush brush = new SolidBrush(color);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
            g.FillRectangle(brush, rect);
        }

        /// <summary>
        /// Draws the horizontal thumb.
        /// </summary>
        /// <param name="g">The <see cref="Graphics"/> used to paint.</param>
        /// <param name="rect">The rectangle in which to paint.</param>
        /// <param name="color">The color to draw the thumb with.</param>
        static void DrawThumbHorizontal(Graphics g, Rectangle rect, Color color)
        {
            using Brush brush = new SolidBrush(color);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
            g.FillRectangle(brush, rect);
        }

        /// <summary>
        /// Draws arrow buttons for vertical scroll bar.
        /// </summary>
        /// <param name="g">The <see cref="Graphics"/> used to paint.</param>
        /// <param name="rect">The rectangle in which to paint.</param>
        /// <param name="color">The color to draw the arrow buttons with.</param>
        /// <param name="arrowUp">true for an up arrow, false otherwise.</param>
        static void DrawArrowButtonVertical(Graphics g, Rectangle rect, Color color, bool arrowUp)
        {
            using Brush brush = new SolidBrush(color);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            // When using anti-aliased drawing mode, a point has zero size and lies in the center of a pixel. To align with the pixel grid, use coordinates that are integers + 0.5f.
            PointF headPoint = new PointF(rect.Left + rect.Width / 2, (arrowUp ? rect.Top : rect.Bottom) - 0.5f);
            float baseY = (arrowUp ? rect.Bottom : rect.Top) - 0.5f;
            g.FillPolygon(brush, new[]
            {
                new PointF(rect.Left - 0.5f, baseY),
                new PointF(rect.Right - 0.5f, baseY),
                headPoint
            });
        }

        /// <summary>
        /// Draws arrow buttons for horizontal scroll bar.
        /// </summary>
        /// <param name="g">The <see cref="Graphics"/> used to paint.</param>
        /// <param name="rect">The rectangle in which to paint.</param>
        /// <param name="color">The color to draw the arrow buttons with.</param>
        /// <param name="arrowLeft">true for a left arrow, false otherwise.</param>
        static void DrawArrowButtonHorizontal(Graphics g, Rectangle rect, Color color, bool arrowLeft)
        {
            using Brush brush = new SolidBrush(color);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            // When using anti-aliased drawing mode, a point has zero size and lies in the center of a pixel. To align with the pixel grid, use coordinates that are integers + 0.5f.
            PointF headPoint = new PointF((arrowLeft ? rect.Left : rect.Right) - 0.5f, rect.Top + rect.Height / 2);
            float baseX = (arrowLeft ? rect.Right : rect.Left) - 0.5f;
            g.FillPolygon(brush, new[]
            {
                new PointF(baseX, rect.Top - 0.5f),
                new PointF(baseX, rect.Bottom - 0.5f),
                headPoint
            });
        }

        #endregion

        #region Fields

        /// <summary>
        /// Indicates many changes to the scrollbar are happening, so stop painting till finished.
        /// </summary>
        bool inUpdate;

        /// <summary>
        /// Highlights the current line if: value > -1
        /// </summary>
        int curPos = -1;

        /// <summary>
        /// The maximum value of curPos. If overScroll (EndAtLastLine) is disabled, it is greater than the maximum scrollbar value, otherwise they are equal.
        /// </summary>
        int maxCurPos = 100;

        /// <summary>
        /// The scrollbar orientation - horizontal / vertical.
        /// </summary>
        ScrollBarOrientation orientation = ScrollBarOrientation.Vertical;

        /// <summary>
        /// The scroll orientation in scroll events.
        /// </summary>
        ScrollOrientation scrollOrientation = ScrollOrientation.VerticalScroll;

        /// <summary>
        /// The thumb rectangle.
        /// </summary>
        Rectangle thumbRectangle;

        /// <summary>
        /// The top arrow rectangle.
        /// </summary>
        Rectangle topArrowRectangle;

        /// <summary>
        /// The bottom arrow rectangle.
        /// </summary>
        Rectangle bottomArrowRectangle;

        /// <summary>
        /// Indicates if top arrow was clicked.
        /// </summary>
        bool topArrowClicked;

        /// <summary>
        /// Indicates if bottom arrow was clicked.
        /// </summary>
        bool bottomArrowClicked;

        /// <summary>
        /// Indicates if channel rectangle above the thumb was clicked.
        /// </summary>
        bool topBarClicked;

        /// <summary>
        /// Indicates if channel rectangle under the thumb was clicked.
        /// </summary>
        bool bottomBarClicked;

        /// <summary>
        /// Indicates if the thumb was clicked.
        /// </summary>
        bool thumbClicked;

        /// <summary>
        /// The state of the thumb.
        /// </summary>
        ScrollBarState thumbState = ScrollBarState.Normal;

        /// <summary>
        /// The state of the top arrow.
        /// </summary>
        ScrollBarArrowButtonState topButtonState = ScrollBarArrowButtonState.UpNormal;

        /// <summary>
        /// The state of the bottom arrow.
        /// </summary>
        ScrollBarArrowButtonState bottomButtonState = ScrollBarArrowButtonState.DownNormal;

        /// <summary>
        /// The scrollbar value minimum.
        /// </summary>
        int minimum;

        /// <summary>
        /// The scrollbar value maximum.
        /// </summary>
        int maximum = 100;

        /// <summary>
        /// The view port size (page size) in relation to the maximum and minimum value.
        /// </summary>
        int viewPortSize = 100;

        /// <summary>
        /// The small change value.
        /// </summary>
        int smallChange = 1;

        /// <summary>
        /// The large change value.
        /// </summary>
        int largeChange = 10;

        /// <summary>
        /// The value of the scrollbar.
        /// </summary>
        int value;

        /// <summary>
        /// The thickness of the thumb.
        /// </summary>
        const int THUMB_THICKNESS = 9;

        int thumbThickness;

        /// <summary>
        /// The thickness of an arrow (base length).
        /// </summary>
        const int ARROW_THICKNESS = THUMB_THICKNESS;

        int arrowThickness;

        /// <summary>
        /// The length of an arrow (point-to-base distance).
        /// </summary>
        const int ARROW_LENGTH = 5;

        int arrowLength;

        /// <summary>
        /// The padding between an arrow's point and the nearest edge.
        /// </summary>
        const int ARROW_PADDING = 6;

        int arrowPadding;

        /// <summary>
        /// The gap between an arrow and the thumb.
        /// </summary>
        const int ARROW_THUMB_GAP = ARROW_PADDING;

        int arrowThumbGap;

        /// <summary>
        /// The length of an arrow, arrow padding and arrow-thumb gap
        /// </summary>
        int arrowPaddedLength;

        /// <summary>
        /// The bottom limit for the thumb bottom.
        /// </summary>
        int thumbBottomLimitBottom;

        /// <summary>
        /// The bottom limit for the thumb top.
        /// </summary>
        int thumbBottomLimitTop;

        /// <summary>
        /// The top limit for the thumb top.
        /// </summary>
        int thumbTopLimit;

        /// <summary>
        /// The current position of the thumb.
        /// </summary>
        int thumbPosition;

        /// <summary>
        /// The track position.
        /// </summary>
        int trackPosition;

        /// <summary>
        /// The progress timer for moving the thumb.
        /// </summary>
        readonly Timer progressTimer = new Timer();

        /// <summary>
        /// Context menu strip.
        /// </summary>
        ContextMenuStrip contextMenu;

        /// <summary>
        /// Container for components.
        /// </summary>
        IContainer components;

        /// <summary>
        /// Menu item.
        /// </summary>
        ToolStripMenuItem tsmiScrollHere;

        /// <summary>
        /// Menu separator.
        /// </summary>
        ToolStripSeparator toolStripSeparator1;

        /// <summary>
        /// Menu item.
        /// </summary>
        ToolStripMenuItem tsmiTop;

        /// <summary>
        /// Menu item.
        /// </summary>
        ToolStripMenuItem tsmiBottom;

        /// <summary>
        /// Menu separator.
        /// </summary>
        ToolStripSeparator toolStripSeparator2;

        /// <summary>
        /// Menu item.
        /// </summary>
        ToolStripMenuItem tsmiLargeUp;

        /// <summary>
        /// Menu item.
        /// </summary>
        ToolStripMenuItem tsmiLargeDown;

        /// <summary>
        /// Menu separator.
        /// </summary>
        ToolStripSeparator toolStripSeparator3;

        /// <summary>
        /// Menu item.
        /// </summary>
        ToolStripMenuItem tsmiSmallUp;

        /// <summary>
        /// Menu item.
        /// </summary>
        ToolStripMenuItem tsmiSmallDown;

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
            InitializeComponent();
            Width = ScaleOddUp(17);
            Height = ScaleHelper.Scale(200);
            // sets the scrollbar up
            SetUpScrollBar();
            // timer for clicking and holding the mouse buttonь
            // over/below the thumb and on the arrow buttons
            progressTimer.Interval = PROGRESS_TIMER_TICK;
            progressTimer.Tick += ProgressTimerTick;
            ContextMenuStrip = contextMenu;
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
            get => curPos;
            set
            {
                // no change, return
                if (curPos == value)
                {
                    return;
                }
                curPos = value;
                Invalidate();
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
            get => maxCurPos;
            set
            {
                // no change, return
                if (maxCurPos == value)
                {
                    return;
                }
                maxCurPos = value;
                Invalidate();
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
            get => orientation;
            set
            {
                // no change - return
                if (value == orientation)
                {
                    return;
                }
                orientation = value;
                // change text of context menu entries
                ChangeContextMenuItems();
                // save scroll orientation for scroll event
                scrollOrientation = value == ScrollBarOrientation.Vertical ? ScrollOrientation.VerticalScroll : ScrollOrientation.HorizontalScroll;
                // only in DesignMode switch width and height
                if (DesignMode)
                {
                    Size = new Size(Height, Width);
                }
                // sets the scrollbar up
                SetUpScrollBar();
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
            get => minimum;
            set
            {
                // no change or value invalid - return
                if (minimum == value || value < 0 || value >= maximum)
                {
                    return;
                }
                minimum = value;
                // current value less than new minimum value - adjust
                if (this.value < value)
                {
                    this.value = value;
                }
                // is current large change value invalid - adjust
                if (largeChange > maximum - minimum)
                {
                    largeChange = maximum - minimum;
                }
                SetUpScrollBar();
                // current value less than new minimum value - adjust
                if (this.value < value)
                {
                    Value = value;
                }
                else
                {
                    // current value is valid - adjust thumb position (already done by SetUpScrollBar())
                    Refresh();
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
            get => maximum;
            set
            {
                // no change or new max. value invalid - return
                if (value == maximum || value < 1 || value <= minimum)
                {
                    return;
                }
                maximum = value;
                // is large change value invalid - adjust
                if (largeChange > maximum - minimum)
                {
                    largeChange = maximum - minimum;
                }
                SetUpScrollBar();
                // is current value greater than new maximum value - adjust
                if (this.value > value)
                {
                    Value = maximum;
                }
                else
                {
                    // current value is valid - adjust thumb position (already done by SetUpScrollBar())
                    Refresh();
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
            get => viewPortSize;
            set
            {
                // no change or new value invalid - return
                if (value == viewPortSize || viewPortSize < 1)
                {
                    return;
                }
                viewPortSize = value;
                SetUpScrollBar();
                // adjust thumb position (already done by SetUpScrollBar())
                Refresh();
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
            get => smallChange;
            set
            {
                // no change or new small change value invalid - return
                if (value == smallChange || value < 1 || value >= largeChange)
                {
                    return;
                }
                smallChange = value;
                SetUpScrollBar();
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
            get => largeChange;
            set
            {
                // no change or new large change value is invalid - return
                if (value == largeChange || value < smallChange || value < 2)
                {
                    return;
                }
                // if value is greater than scroll area - adjust
                if (value > maximum - minimum)
                {
                    largeChange = maximum - minimum;
                }
                else
                {
                    // set new value
                    largeChange = value;
                }
                SetUpScrollBar();
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
            get => value;
            set
            {
                // no change or invalid value - return
                if (this.value == value || value < minimum || value > maximum)
                {
                    return;
                }
                this.value = value;
                // adjust thumb position
                ChangeThumbPosition(GetThumbPosition());
                // raise scroll event
                OnScroll(new ScrollEventArgs(ScrollEventType.ThumbPosition, -1, this.value, scrollOrientation));
                Refresh();
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
            get => borderColor;
            set
            {
                borderColor = value;
                Invalidate();
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
            get => borderColorDisabled;
            set
            {
                borderColorDisabled = value;
                Invalidate();
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
            get => backColor;
            set
            {
                backColor = value;
                Invalidate();
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
            get => backColorDisabled;
            set
            {
                backColorDisabled = value;
                Invalidate();
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
            get => foreColor;
            set
            {
                foreColor = value;
                Invalidate();
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
            get => foreColorHot;
            set
            {
                foreColorHot = value;
                Invalidate();
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
            get => foreColorPressed;
            set
            {
                foreColorPressed = value;
                Invalidate();
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
            get => arrowColor;
            set
            {
                arrowColor = value;
                Invalidate();
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
            get => arrowColorHot;
            set
            {
                arrowColorHot = value;
                Invalidate();
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
            get => arrowColorPressed;
            set
            {
                arrowColorPressed = value;
                Invalidate();
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
            get => curPosColor;
            set
            {
                curPosColor = value;
                Invalidate();
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
            get => contextMenu.Opacity;
            set
            {
                // no change - return
                if (value == contextMenu.Opacity)
                {
                    return;
                }
                contextMenu.AllowTransparency = value != 1;
                contextMenu.Opacity = value;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prevents the drawing of the control until <see cref="EndUpdate"/> is called.
        /// </summary>
        public void BeginUpdate()
        {
            Win32.SendMessage(Handle, Win32.WM_SETREDRAW, 0, 0);
            inUpdate = true;
        }

        /// <summary>
        /// Ends the updating process and the control can draw itself again.
        /// </summary>
        public void EndUpdate()
        {
            Win32.SendMessage(Handle, Win32.WM_SETREDRAW, 1, 0);
            inUpdate = false;
            SetUpScrollBar();
            Refresh();
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
               orientation);
            // draw thumb and grip
            DrawThumb(
               e.Graphics,
               thumbRectangle,
               thumbState,
               orientation);

            // draw arrows
            DrawArrowButton(
               e.Graphics,
               topArrowRectangle,
               topButtonState,
               true,
               orientation);
            DrawArrowButton(
               e.Graphics,
               bottomArrowRectangle,
               bottomButtonState,
               false,
               orientation);

            // draw current line
            if (curPos > -1 && orientation == ScrollBarOrientation.Vertical)
            {
                using SolidBrush brush = new SolidBrush(curPosColor);
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                e.Graphics.FillRectangle(brush, 0, GetCurPosition() - ScaleHelper.Scale(2f) / 2, Width, ScaleHelper.Scale(2f));
            }

            // draw border
            using Pen pen = new Pen((Enabled ? borderColor : borderColorDisabled));
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
            e.Graphics.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);
        }

        /// <summary>
        /// Calculates the current position.
        /// </summary>
        /// <returns>The current position.</returns>
        float GetCurPosition()
        {
            int bottomLimit = (maxCurPos > maximum) ? thumbBottomLimitBottom : thumbBottomLimitTop;
            int pixelRange = bottomLimit - thumbTopLimit; // == size - (overScroll ? thumbSize : 0) - arrows - paddings
            int realRange = maxCurPos - minimum;
            float perc = (realRange != 0) ? ((curPos - minimum) / (float)realRange) : 0f;
            return Math.Max(thumbTopLimit, Math.Min(bottomLimit, perc * pixelRange + arrowPaddedLength));
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
                ContextMenuStrip = null;
                Point mouseLocation = e.Location;
                if (thumbRectangle.Contains(mouseLocation))
                {
                    thumbClicked = true;
                    thumbPosition = orientation == ScrollBarOrientation.Vertical ? mouseLocation.Y - thumbRectangle.Y : mouseLocation.X - thumbRectangle.X;
                    thumbState = ScrollBarState.Pressed;
                    Invalidate(thumbRectangle);
                }
                else if (topArrowRectangle.Contains(mouseLocation))
                {
                    topArrowClicked = true;
                    topButtonState = ScrollBarArrowButtonState.UpPressed;
                    Invalidate(topArrowRectangle);
                    ProgressThumb(true);
                }
                else if (bottomArrowRectangle.Contains(mouseLocation))
                {
                    bottomArrowClicked = true;
                    bottomButtonState = ScrollBarArrowButtonState.DownPressed;
                    Invalidate(bottomArrowRectangle);
                    ProgressThumb(true);
                }
                else
                {
                    trackPosition = orientation == ScrollBarOrientation.Vertical ? mouseLocation.Y : mouseLocation.X;
                    if (trackPosition < (orientation == ScrollBarOrientation.Vertical ? thumbRectangle.Y : thumbRectangle.X))
                        topBarClicked = true;
                    else
                        bottomBarClicked = true;
                    ProgressThumb(true);
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                trackPosition = orientation == ScrollBarOrientation.Vertical ? e.Y : e.X;
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
                ContextMenuStrip = contextMenu;
                if (thumbClicked)
                {
                    thumbClicked = false;
                    thumbState = ScrollBarState.Normal;
                    OnScroll(new ScrollEventArgs(ScrollEventType.EndScroll, -1, value, scrollOrientation));
                }
                else if (topArrowClicked)
                {
                    topArrowClicked = false;
                    topButtonState = ScrollBarArrowButtonState.UpNormal;
                    StopTimer();
                }
                else if (bottomArrowClicked)
                {
                    bottomArrowClicked = false;
                    bottomButtonState = ScrollBarArrowButtonState.DownNormal;
                    StopTimer();
                }
                else if (topBarClicked)
                {
                    topBarClicked = false;
                    StopTimer();
                }
                else if (bottomBarClicked)
                {
                    bottomBarClicked = false;
                    StopTimer();
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
            bottomButtonState = ScrollBarArrowButtonState.DownActive;
            topButtonState = ScrollBarArrowButtonState.UpActive;
            thumbState = ScrollBarState.Active;
            Invalidate();
        }

        /// <summary>
        /// Raises the MouseLeave event.
        /// </summary>
        /// <param name="e">A <see cref="EventArgs"/> that contains the event data.</param>
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            ResetScrollStatus();
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
                if (thumbClicked)
                {
                    int oldThumbLocation = (orientation == ScrollBarOrientation.Vertical) ? thumbRectangle.Y : thumbRectangle.X;
                    int oldScrollValue = value;
                    topButtonState = ScrollBarArrowButtonState.UpActive;
                    bottomButtonState = ScrollBarArrowButtonState.DownActive;
                    int pos = (orientation == ScrollBarOrientation.Vertical ? e.Location.Y : e.Location.X) - thumbPosition;
                    // The thumb is all the way to the top
                    if (pos <= thumbTopLimit)
                    {
                        ChangeThumbPosition(thumbTopLimit);
                        value = minimum;
                    }
                    else if (pos >= thumbBottomLimitTop)
                    {
                        // The thumb is all the way to the bottom
                        ChangeThumbPosition(thumbBottomLimitTop);
                        value = maximum;
                    }
                    else
                    {
                        // The thumb is between the ends of the track.
                        ChangeThumbPosition(pos);
                        int pixelRange = thumbBottomLimitTop - thumbTopLimit; // == size - thumbSize - arrows - paddings
                        int position = ((orientation == ScrollBarOrientation.Vertical) ? thumbRectangle.Y : thumbRectangle.X) - arrowPaddedLength;
                        // percent of the new position
                        float perc = (pixelRange != 0) ? (position / (float)pixelRange) : 0f;
                        // the new value is somewhere between max and min, starting
                        // at min position
                        value = Convert.ToInt32((perc * (maximum - minimum)) + minimum);
                    }

                    // raise scroll event if value has changed
                    if (oldScrollValue != value)
                    {
                        OnScroll(new ScrollEventArgs(ScrollEventType.ThumbTrack, oldScrollValue, value, scrollOrientation));
                        Refresh();
                    }
                    else
                    {
                        int newThumbLocation = (orientation == ScrollBarOrientation.Vertical) ? thumbRectangle.Y : thumbRectangle.X;
                        // repaint if thumb location has changed, but only at the top and bottom, to prevent thumb jumping around
                        if ((oldThumbLocation != newThumbLocation) && ((newThumbLocation == thumbTopLimit) || (newThumbLocation == thumbBottomLimitTop)))
                        {
                            Refresh();
                        }
                    }
                }
            }
            else if (!ClientRectangle.Contains(e.Location))
            {
                ResetScrollStatus();
            }
            else if (e.Button == MouseButtons.None) // only moving the mouse
            {
                if (topArrowRectangle.Contains(e.Location))
                {
                    topButtonState = ScrollBarArrowButtonState.UpHot;
                    Invalidate(topArrowRectangle);
                }
                else if (bottomArrowRectangle.Contains(e.Location))
                {
                    bottomButtonState = ScrollBarArrowButtonState.DownHot;
                    Invalidate(bottomArrowRectangle);
                }
                else if (thumbRectangle.Contains(e.Location))
                {
                    thumbState = ScrollBarState.Hot;
                    Invalidate(thumbRectangle);
                }
                else if (ClientRectangle.Contains(e.Location))
                {
                    topButtonState = ScrollBarArrowButtonState.UpActive;
                    bottomButtonState = ScrollBarArrowButtonState.DownActive;
                    thumbState = ScrollBarState.Active;
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
            if (DesignMode)
            {
                var pad = ScaleHelper.Scale(10);
                if (orientation == ScrollBarOrientation.Vertical)
                {
                    if (height < 2 * arrowPaddedLength)
                        height = 2 * arrowPaddedLength;
                    width = ScaleOddUp(17);
                }
                else
                {
                    if (width < 2 * arrowPaddedLength)
                        width = 2 * arrowPaddedLength;
                    height = ScaleOddUp(17);
                }
            }
            base.SetBoundsCore(x, y, width, height, specified);
            if (DesignMode)
            {
                SetUpScrollBar();
            }
        }

        /// <summary>
        /// Raises the <see cref="System.Windows.Forms.Control.SizeChanged"/> event.
        /// </summary>
        /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            SetUpScrollBar();
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
            if (orientation == ScrollBarOrientation.Horizontal)
            {
                keyUp = Keys.Left;
                keyDown = Keys.Right;
            }
            if ((keyData == keyUp) || (keyData == keyDown))
            {
                Value = GetValue(true, keyData == keyUp);
                return true;
            }
            if ((keyData == Keys.PageUp) || (keyData == Keys.PageDown))
            {
                Value = GetValue(false, keyData == Keys.PageUp);
                return true;
            }
            if (keyData == Keys.Home)
            {
                Value = minimum;
                return true;
            }
            if (keyData == Keys.End)
            {
                Value = maximum;
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

            if (Enabled)
            {
                thumbState = ScrollBarState.Normal;
                topButtonState = ScrollBarArrowButtonState.UpNormal;
                bottomButtonState = ScrollBarArrowButtonState.DownNormal;
            }
            else
            {
                thumbState = ScrollBarState.Disabled;
                topButtonState = ScrollBarArrowButtonState.UpDisabled;
                bottomButtonState = ScrollBarArrowButtonState.DownDisabled;
            }

            Refresh();
        }

        #endregion

        #region Misc Methods

        /// <summary>
        /// Sets up the scrollbar.
        /// </summary>
        void SetUpScrollBar()
        {
            // if no drawing - return
            if (inUpdate)
            {
                return;
            }
            // save client rectangle
            Rectangle rect = ClientRectangle;
            // set up the width's, height's and rectangles for the different
            // elements
            thumbThickness = ScaleOddUp(THUMB_THICKNESS); // Should be odd for the thumb to be perfectly centered (since scrollbar width is odd)
            arrowThickness = ScaleOddUp(ARROW_THICKNESS); // Should be odd for nice and crisp arrow points.
            arrowLength = ScaleHelper.Scale(ARROW_LENGTH);
            arrowPadding = ScaleHelper.Scale(ARROW_PADDING);
            arrowThumbGap = ScaleHelper.Scale(ARROW_THUMB_GAP);
            arrowPaddedLength = arrowLength + arrowPadding + arrowThumbGap;
            if (orientation == ScrollBarOrientation.Vertical)
            {
                thumbRectangle = new Rectangle(
                   rect.X + (rect.Width - thumbThickness) / 2, // Assuming rect.Width and this.thumbThickness are both odd, so that (rect.Width - this.thumbThickness) is even.
                   rect.Y + arrowPaddedLength,
                   thumbThickness,
                   GetThumbSize()
                );
                topArrowRectangle = new Rectangle(
                   rect.X + (rect.Width - arrowThickness) / 2, // Assuming rect.Width and this.arrowThickness are both odd, so that (rect.Width - this.arrowThickness) is even.
                   rect.Y + arrowPadding,
                   arrowThickness,
                   arrowLength
                );
                bottomArrowRectangle = new Rectangle(
                   rect.X + (rect.Width - arrowThickness) / 2, // Assuming rect.Width and this.arrowThickness are both odd, so that (rect.Width - this.arrowThickness) is even.
                   rect.Bottom - arrowPadding - arrowLength,
                   arrowThickness,
                   arrowLength
                );
                // Set the default starting thumb position.
                //this.thumbPosition = this.thumbRectangle.Height / 2;
                // Set the bottom limit of the thumb's bottom border.
                thumbBottomLimitBottom = rect.Bottom - arrowPaddedLength;
                // Set the bottom limit of the thumb's top border.
                thumbBottomLimitTop = thumbBottomLimitBottom - thumbRectangle.Height;
                // Set the top limit of the thumb's top border.
                thumbTopLimit = rect.Y + arrowPaddedLength;
            }
            else
            {
                thumbRectangle = new Rectangle(
                   rect.X + arrowPaddedLength,
                   rect.Y + (rect.Height - thumbThickness) / 2, // Assuming rect.Height and this.thumbThickness are both odd, so that (rect.Height - this.thumbThickness) is even.
                   GetThumbSize(),
                   thumbThickness
                );
                topArrowRectangle = new Rectangle(
                   rect.X + arrowPadding,
                   rect.Y + (rect.Height - arrowThickness) / 2, // Assuming rect.Height and this.arrowThickness are both odd, so that (rect.Height - this.arrowThickness) is even.
                   arrowLength,
                   arrowThickness
                );
                bottomArrowRectangle = new Rectangle(
                   rect.Right - arrowPadding - arrowLength,
                   rect.Y + (rect.Height - arrowThickness) / 2, // Assuming rect.Height and this.arrowThickness are both odd, so that (rect.Height - this.arrowThickness) is even.
                   arrowLength,
                   arrowThickness
                );
                // Set the default starting thumb position.
                //this.thumbPosition = this.thumbRectangle.Width / 2;
                // Set the bottom limit of the thumb's bottom border.
                thumbBottomLimitBottom = rect.Right - arrowPaddedLength;
                // Set the bottom limit of the thumb's top border.
                thumbBottomLimitTop = thumbBottomLimitBottom - thumbRectangle.Width;
                // Set the top limit of the thumb's top border.
                thumbTopLimit = rect.X + arrowPaddedLength;
            }
            ChangeThumbPosition(GetThumbPosition());
            Refresh();
        }

        /// <summary>
        /// Handles the updating of the thumb.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">An object that contains the event data.</param>
        void ProgressTimerTick(object sender, EventArgs e)
        {
            ProgressThumb(true);
        }

        /// <summary>
        /// Resets the scroll status of the scrollbar.
        /// </summary>
        void ResetScrollStatus()
        {
            // get current mouse position
            Point pos = PointToClient(Cursor.Position);
            // set appearance of buttons in relation to where the mouse is -
            // outside or inside the control
            if (ClientRectangle.Contains(pos))
            {
                bottomButtonState = ScrollBarArrowButtonState.DownActive;
                topButtonState = ScrollBarArrowButtonState.UpActive;
            }
            else
            {
                bottomButtonState = ScrollBarArrowButtonState.DownNormal;
                topButtonState = ScrollBarArrowButtonState.UpNormal;
            }
            // set appearance of thumb
            thumbState = thumbRectangle.Contains(pos) ? ScrollBarState.Hot : ScrollBarState.Normal;
            bottomArrowClicked = bottomBarClicked = topArrowClicked = topBarClicked = false;
            StopTimer();
            Refresh();
        }

        /// <summary>
        /// Calculates the new value of the scrollbar.
        /// </summary>
        /// <param name="smallIncrement">true for a small change, false otherwise.</param>
        /// <param name="up">true for up movement, false otherwise.</param>
        /// <returns>The new scrollbar value.</returns>
        int GetValue(bool smallIncrement, bool up)
        {
            // calculate the new value of the scrollbar
            // with checking if new value is in bounds (min/max)
            if (up)
                return Math.Max(minimum, value - (smallIncrement ? smallChange : largeChange));
            return Math.Min(maximum, value + (smallIncrement ? smallChange : largeChange));
        }

        /// <summary>
        /// Calculates the new thumb position.
        /// </summary>
        /// <returns>The new thumb position.</returns>
        int GetThumbPosition()
        {
            int pixelRange = thumbBottomLimitTop - thumbTopLimit; // == size - thumbSize - arrows - paddings
            int realRange = maximum - minimum;
            float perc = (realRange != 0) ? ((value - minimum) / (float)realRange) : 0f;
            return Math.Max(thumbTopLimit, Math.Min(thumbBottomLimitTop, Convert.ToInt32((perc * pixelRange) + arrowPaddedLength)));
        }

        /// <summary>
        /// Calculates the length of the thumb.
        /// </summary>
        /// <returns>The length of the thumb.</returns>
        int GetThumbSize()
        {
            int trackSize = (orientation == ScrollBarOrientation.Vertical ? Height : Width) - 2 * arrowPaddedLength;
            float newThumbSize = viewPortSize * (float)trackSize / (maximum - minimum + viewPortSize);
            return Convert.ToInt32(Math.Min(trackSize, Math.Max(newThumbSize, ScaleHelper.Scale(8))));
        }

        /// <summary>
        /// Enables the timer.
        /// </summary>
        void EnableTimer()
        {
            // if timer is not already enabled - enable it
            if (!progressTimer.Enabled)
            {
                progressTimer.Interval = PROGRESS_TIMER_DELAY;
                progressTimer.Start();
            }
            else
            {
                // if already enabled, change tick time
                progressTimer.Interval = PROGRESS_TIMER_TICK;
            }
        }

        /// <summary>
        /// Stops the progress timer.
        /// </summary>
        void StopTimer() => progressTimer.Stop();

        /// <summary>
        /// Changes the position of the thumb.
        /// </summary>
        /// <param name="position">The new position.</param>
        void ChangeThumbPosition(int position)
        {
            if (orientation == ScrollBarOrientation.Vertical)
                thumbRectangle.Y = position;
            else
                thumbRectangle.X = position;
        }

        /// <summary>
        /// Controls the movement of the thumb.
        /// </summary>
        /// <param name="enableTimer">true for enabling the timer, false otherwise.</param>
        void ProgressThumb(bool enableTimer)
        {
            int scrollOldValue = value;
            var type = ScrollEventType.First;
            var thumbPos = (orientation == ScrollBarOrientation.Vertical) ? thumbRectangle.Y : thumbRectangle.X;
            var thumbSize = (orientation == ScrollBarOrientation.Vertical) ? thumbRectangle.Height : thumbRectangle.Width;
            // arrow down or shaft down clicked
            if (bottomArrowClicked || (bottomBarClicked && (thumbPos + thumbSize) < trackPosition))
            {
                type = bottomArrowClicked ? ScrollEventType.SmallIncrement : ScrollEventType.LargeIncrement;
                value = GetValue(bottomArrowClicked, false);
                if (value == maximum)
                {
                    ChangeThumbPosition(thumbBottomLimitTop);

                    type = ScrollEventType.Last;
                }
                else ChangeThumbPosition(Math.Min(thumbBottomLimitTop, GetThumbPosition()));
            }
            else if (topArrowClicked || (topBarClicked && thumbPos > trackPosition))
            {
                type = topArrowClicked ? ScrollEventType.SmallDecrement : ScrollEventType.LargeDecrement;
                // arrow up or shaft up clicked
                value = GetValue(topArrowClicked, true);
                if (value == minimum)
                {
                    ChangeThumbPosition(thumbTopLimit);
                    type = ScrollEventType.First;
                }
                else ChangeThumbPosition(Math.Max(thumbTopLimit, GetThumbPosition()));
            }
            else if (!((topArrowClicked && thumbPos == thumbTopLimit) || (bottomArrowClicked && thumbPos == thumbBottomLimitTop)))
            {
                ResetScrollStatus();
                return;
            }
            if (scrollOldValue != value)
            {
                OnScroll(new ScrollEventArgs(type, scrollOldValue, value, scrollOrientation));
                Refresh();
                if (enableTimer)
                    EnableTimer();
            }
            else
            {
                if (topArrowClicked)
                    type = ScrollEventType.SmallDecrement;
                else if (bottomArrowClicked)
                    type = ScrollEventType.SmallIncrement;
                OnScroll(new ScrollEventArgs(type, value));
            }
        }

        /// <summary>
        /// Changes the displayed text of the context menu items dependent of the current <see cref="ScrollBarOrientation"/>.
        /// </summary>
        void ChangeContextMenuItems()
        {
            if (orientation == ScrollBarOrientation.Vertical)
            {
                tsmiTop.Text = "Top";
                tsmiBottom.Text = "Bottom";
                tsmiLargeDown.Text = "Page Down";
                tsmiLargeUp.Text = "Page Up";
                tsmiSmallDown.Text = "Scroll Down";
                tsmiSmallUp.Text = "Scroll Up";
                tsmiScrollHere.Text = "Scroll Here";
            }
            else
            {
                tsmiTop.Text = "Left";
                tsmiBottom.Text = "Right";
                tsmiLargeDown.Text = "Page Left";
                tsmiLargeUp.Text = "Page Right";
                tsmiSmallDown.Text = "Scroll Right";
                tsmiSmallUp.Text = "Scroll Left";
                tsmiScrollHere.Text = "Scroll Here";
            }
        }

        #endregion

        #region Context Menu Methods

        /// <summary>
        /// Initializes the context menu.
        /// </summary>
        void InitializeComponent()
        {
            components = new Container();
            contextMenu = new ContextMenuStrip(components);
            tsmiScrollHere = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            tsmiTop = new ToolStripMenuItem();
            tsmiBottom = new ToolStripMenuItem();
            toolStripSeparator2 = new ToolStripSeparator();
            tsmiLargeUp = new ToolStripMenuItem();
            tsmiLargeDown = new ToolStripMenuItem();
            toolStripSeparator3 = new ToolStripSeparator();
            tsmiSmallUp = new ToolStripMenuItem();
            tsmiSmallDown = new ToolStripMenuItem();
            contextMenu.SuspendLayout();
            SuspendLayout();
            // 
            // contextMenu
            // 
            contextMenu.Items.AddRange(new ToolStripItem[] {
            tsmiScrollHere,
            toolStripSeparator1,
            tsmiTop,
            tsmiBottom,
            toolStripSeparator2,
            tsmiLargeUp,
            tsmiLargeDown,
            toolStripSeparator3,
            tsmiSmallUp,
            tsmiSmallDown});
            contextMenu.Name = "contextMenu";
            contextMenu.Size = new Size(151, 176);
            // 
            // tsmiScrollHere
            // 
            tsmiScrollHere.DisplayStyle = ToolStripItemDisplayStyle.Text;
            tsmiScrollHere.Name = "tsmiScrollHere";
            tsmiScrollHere.Size = new Size(150, 22);
            tsmiScrollHere.Text = "Scroll Here";
            tsmiScrollHere.Click += ScrollHereClick;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(147, 6);
            // 
            // tsmiTop
            // 
            tsmiTop.DisplayStyle = ToolStripItemDisplayStyle.Text;
            tsmiTop.Name = "tsmiTop";
            tsmiTop.Size = new Size(150, 22);
            tsmiTop.Text = "Top";
            tsmiTop.Click += TopClick;
            // 
            // tsmiBottom
            // 
            tsmiBottom.DisplayStyle = ToolStripItemDisplayStyle.Text;
            tsmiBottom.Name = "tsmiBottom";
            tsmiBottom.Size = new Size(150, 22);
            tsmiBottom.Text = "Bottom";
            tsmiBottom.Click += BottomClick;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(147, 6);
            // 
            // tsmiLargeUp
            // 
            tsmiLargeUp.DisplayStyle = ToolStripItemDisplayStyle.Text;
            tsmiLargeUp.Name = "tsmiLargeUp";
            tsmiLargeUp.Size = new Size(150, 22);
            tsmiLargeUp.Text = "Page Up";
            tsmiLargeUp.Click += LargeUpClick;
            // 
            // tsmiLargeDown
            // 
            tsmiLargeDown.DisplayStyle = ToolStripItemDisplayStyle.Text;
            tsmiLargeDown.Name = "tsmiLargeDown";
            tsmiLargeDown.Size = new Size(150, 22);
            tsmiLargeDown.Text = "Page Down";
            tsmiLargeDown.Click += LargeDownClick;
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new Size(147, 6);
            // 
            // tsmiSmallUp
            // 
            tsmiSmallUp.DisplayStyle = ToolStripItemDisplayStyle.Text;
            tsmiSmallUp.Name = "tsmiSmallUp";
            tsmiSmallUp.Size = new Size(150, 22);
            tsmiSmallUp.Text = "Scroll Up";
            tsmiSmallUp.Click += SmallUpClick;
            // 
            // tsmiSmallDown
            // 
            tsmiSmallDown.DisplayStyle = ToolStripItemDisplayStyle.Text;
            tsmiSmallDown.Name = "tsmiSmallDown";
            tsmiSmallDown.Size = new Size(150, 22);
            tsmiSmallDown.Text = "Scroll Down";
            tsmiSmallDown.Click += SmallDownClick;
            contextMenu.ResumeLayout(false);
            ResumeLayout(false);
        }

        /// <summary>
        /// Context menu handler.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        void ScrollHereClick(object sender, EventArgs e)
        {
            int thumbSize = (orientation == ScrollBarOrientation.Vertical) ? thumbRectangle.Height : thumbRectangle.Width;
            ChangeThumbPosition(Math.Max(thumbTopLimit, Math.Min(thumbBottomLimitTop, trackPosition - (thumbSize / 2))));
            int pixelRange = thumbBottomLimitTop - thumbTopLimit; // == size - thumbSize - arrows - paddings
            int position = ((orientation == ScrollBarOrientation.Vertical) ? thumbRectangle.Y : thumbRectangle.X) - arrowPaddedLength;
            // percent of the new position
            float perc = (pixelRange != 0) ? (position / (float)pixelRange) : 0f;
            int oldValue = value;
            value = Convert.ToInt32((perc * (maximum - minimum)) + minimum);
            OnScroll(new ScrollEventArgs(ScrollEventType.ThumbPosition, oldValue, value, scrollOrientation));
            Refresh();
        }

        /// <summary>
        /// Context menu handler.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        void TopClick(object sender, EventArgs e)
        {
            Value = minimum;
        }

        /// <summary>
        /// Context menu handler.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        void BottomClick(object sender, EventArgs e)
        {
            Value = maximum;
        }

        /// <summary>
        /// Context menu handler.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        void LargeUpClick(object sender, EventArgs e)
        {
            Value = GetValue(false, true);
        }

        /// <summary>
        /// Context menu handler.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        void LargeDownClick(object sender, EventArgs e)
        {
            Value = GetValue(false, false);
        }

        /// <summary>
        /// Context menu handler.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        void SmallUpClick(object sender, EventArgs e)
        {
            Value = GetValue(true, true);
        }

        /// <summary>
        /// Context menu handler.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event arguments.</param>
        void SmallDownClick(object sender, EventArgs e)
        {
            Value = GetValue(true, false);
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
        bool disposed;

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
                if (control.Parent is null) return;
                if (enabled)
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
        bool disposed;
        TextBox textBox;

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
            return Win32.SendMessage(textBox.Handle, Win32.EM_GETFIRSTVISIBLELINE, 0, 0);
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
            if (textBox.ScrollBars != ScrollBars.Vertical)
            {
                // Force scrollbar so that content is displayed correctly...
                textBox.ScrollBars = ScrollBars.Vertical;
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
        bool disposed;
        RichTextBox textBox;

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
        bool disposed;
        PropertyGrid propertyGrid;

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
        int GetVisibleRows()
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
        void SetScrollOffset(int value)
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
        ScrollBar GetScrollBar()
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
        bool disposed;
        TreeView treeView;

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
        bool disposed;
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
        bool disposed;
        ListBox listBox;

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
                Win32.PostMessage(listBox.Handle, Win32.WM_VSCROLL, 4 + 0x10000 * e.NewValue, 0);
            }
            else
            {
                Win32.PostMessage(listBox.Handle, Win32.WM_HSCROLL, 4 + 0x10000 * e.NewValue, 0);
            }
        }
    }

    public class DataGridViewScroller : ScrollerBase
    {
        bool disposed;
        DataGridView dataGridView;

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
    class ScrollBarDesigner : ControlDesigner
    {
        /// <summary>
        /// Gets the <see cref="SelectionRules"/> for the control.
        /// </summary>
        public override SelectionRules SelectionRules
        {
            get
            {
                // gets the property descriptor for the property "Orientation"
                PropertyDescriptor propDescriptor = TypeDescriptor.GetProperties(Component)["Orientation"];
                // if not null - we can read the current orientation of the scroll bar
                if (propDescriptor != null)
                {
                    // get the current orientation
                    ScrollBarOrientation orientation = (ScrollBarOrientation)propDescriptor.GetValue(Component);
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
    enum ScrollBarState
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
    enum ScrollBarArrowButtonState
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
