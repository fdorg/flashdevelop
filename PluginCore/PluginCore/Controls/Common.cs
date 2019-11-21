using PluginCore;
using PluginCore.Helpers;
using System.ComponentModel.Design;
using System.Drawing;
using System.Reflection;

namespace System.Windows.Forms
{
    public class BorderPanel : Panel
    {
        /// <summary>
        /// Border width of the panel
        /// </summary>
        public static int BorderWidth { get; set; } = 1;

        /// <summary>
        /// Should we use custom border panel?
        /// </summary>
        public static bool UseCustomBorder => PluginBase.MainForm.GetThemeFlag("ThemeManager.UseCustomBorder", false);

        /// <summary>
        /// Adds a wrapper panel to the control to create a border.
        /// </summary>
        public static BorderPanel Attach(Control ctrl)
        {
            if (!UseCustomBorder) return null;
            if (ctrl.Parent is TableLayoutPanel) return null;
            return Attach(ctrl, true);
        }
        public static BorderPanel Attach(Control ctrl, bool visible)
        {
            BorderPanel panel;
            if (!UseCustomBorder) return null;
            if (ctrl.Tag is BorderPanel) panel = ctrl.Tag as BorderPanel;
            else
            {
                panel = new BorderPanel();
                panel.Parent = ctrl.Parent;
                panel.Tag = ctrl;
                ctrl.Tag = panel;
            }
            var b = BorderWidth * 2;
            var s = new Size(ctrl.Width + b, ctrl.Height + b);
            var l = new Point(ctrl.Left - BorderWidth, ctrl.Top - BorderWidth);
            var v = (ctrl.Dock == DockStyle.None) && visible;
            if (!panel.Size.Equals(s)) panel.Size = s;
            if (!panel.Location.Equals(l)) panel.Location = l;
            if (!panel.Anchor.Equals(ctrl.Anchor)) panel.Anchor = ctrl.Anchor;
            if (panel.Visible != v) panel.Visible = v;
            return panel;
        }

        /// <summary>
        /// Gets the correct color for the bordered control
        /// </summary>
        Color GetBorderColor()
        {
            if (Tag is Control)
            {
                string name = ThemeHelper.GetFilteredTypeName(Tag.GetType());
                return PluginBase.MainForm.GetThemeColor(name + ".BorderColor", SystemColors.ControlDark);
            }

            return SystemColors.ControlDark;
        }

        /// <summary>
        /// Paint only background for the border
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.FillRectangle(new SolidBrush(GetBorderColor()), ClientRectangle);
        }

    }

    public class DataGridViewEx : DataGridView, IThemeHandler
    {
        bool themeBorder = false;
        public bool UseTheme { get; set; } = true;
        public Color BorderColor { get; set; } = SystemColors.ControlDark;

        public DataGridViewEx()
        {
            CellPainting += OnDataGridViewCellPainting;
        }

        public void AfterTheming()
        {
            GridColor = PluginBase.MainForm.GetThemeColor("DataGridView.LineColor", SystemColors.ControlDark);
            DefaultCellStyle.ForeColor = PluginBase.MainForm.GetThemeColor("DataGridView.ForeColor", SystemColors.WindowText);
            DefaultCellStyle.BackColor = PluginBase.MainForm.GetThemeColor("DataGridView.BackColor", SystemColors.Window);
        }

        void OnDataGridViewCellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (DesignMode) return;
            if (e.RowIndex == -1)
            {
                Color back = PluginBase.MainForm.GetThemeColor("ColumnHeader.BackColor");
                Color text = PluginBase.MainForm.GetThemeColor("ColumnHeader.TextColor");
                Color border = PluginBase.MainForm.GetThemeColor("ColumnHeader.BorderColor");
                if (back != Color.Empty && border != Color.Empty && text != Color.Empty)
                {
                    EnableHeadersVisualStyles = false;
                    ColumnHeadersDefaultCellStyle.ForeColor = text;
                    e.Graphics.FillRectangle(new SolidBrush(back), e.CellBounds);
                    e.Graphics.DrawLine(new Pen(border), e.CellBounds.X, e.CellBounds.Height - 1, e.CellBounds.X + e.CellBounds.Width, e.CellBounds.Height - 1);
                    e.Graphics.DrawLine(new Pen(border), e.CellBounds.X + e.CellBounds.Width - 1, 3, e.CellBounds.X + e.CellBounds.Width - 1, e.CellBounds.Height - 6);
                    e.PaintContent(e.ClipBounds);
                    e.Handled = true;
                }
                else EnableHeadersVisualStyles = true;
            }
        }

        protected override void WndProc(ref Message message)
        {
            base.WndProc(ref message);
            if (DesignMode) return;
            switch (message.Msg)
            {
                case Win32.WM_PAINT:
                    if (UseTheme && BorderStyle == BorderStyle.FixedSingle)
                    {
                        themeBorder = true;
                        BorderStyle = BorderStyle.None;
                    }
                    if (UseTheme && themeBorder)
                    {
                        BorderPanel.Attach(this);
                    }
                    break;
            }
        }

    }

    public class ListViewEx : ListView
    {
        readonly Timer expandDelay;
        bool themeBorder = false;
        public bool UseTheme { get; set; } = true;
        public Color GridLineColor { get; set; } = SystemColors.Control;
        public Color BorderColor { get; set; } = SystemColors.ControlDark;
        bool themeGridLines = false;

        public ListViewEx()
        {
            OwnerDraw = true;
            DoubleBuffered = true;
            DrawColumnHeader += OnDrawColumnHeader;
            DrawSubItem += OnDrawSubItem;
            DrawItem += OnDrawItem;
            expandDelay = new Timer();
            expandDelay.Interval = 50;
            expandDelay.Tick += ExpandDelayTick;
            expandDelay.Enabled = true;
            expandDelay.Start();
            GridLines = false;
        }

        void OnDrawItem(object sender, DrawListViewItemEventArgs e)
        {
            e.DrawDefault = true;
            if (themeGridLines && Items.Count > 0)
            {
                Pen pen = new Pen(GridLineColor);
                e.Graphics.DrawLine(pen, new Point(e.Bounds.Left, e.Bounds.Top - 1), new Point(e.Bounds.Right, e.Bounds.Top - 1));
            }
        }

        void OnDrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            e.DrawDefault = true;
            if (themeGridLines && Items.Count > 0)
            {
                Pen pen = new Pen(GridLineColor);
                e.Graphics.DrawLine(pen, new Point(e.Bounds.Left - 1, e.Bounds.Top), new Point(e.Bounds.Left - 1, e.Bounds.Bottom));
            }
        }

        protected virtual void OnDrawColumnHeader(object sender, DrawListViewColumnHeaderEventArgs e)
        {
            Color back = PluginBase.MainForm.GetThemeColor("ColumnHeader.BackColor");
            Color text = PluginBase.MainForm.GetThemeColor("ColumnHeader.TextColor");
            Color border = PluginBase.MainForm.GetThemeColor("ColumnHeader.BorderColor");
            if (UseTheme && back != Color.Empty && border != Color.Empty && text != Color.Empty)
            {
                e.Graphics.FillRectangle(new SolidBrush(back), e.Bounds.X, 0, e.Bounds.Width, e.Bounds.Height);
                e.Graphics.DrawLine(new Pen(border), e.Bounds.X, e.Bounds.Height - 1, e.Bounds.X + e.Bounds.Width, e.Bounds.Height - 1);
                e.Graphics.DrawLine(new Pen(border), e.Bounds.X + e.Bounds.Width - 1, 3, e.Bounds.X + e.Bounds.Width - 1, e.Bounds.Height - 6);
                int textHeight = TextRenderer.MeasureText("HeightTest", e.Font).Height + 1;
                Rectangle textRect = new Rectangle(e.Bounds.X + 3, e.Bounds.Y + (e.Bounds.Height / 2) - (textHeight / 2), e.Bounds.Width, e.Bounds.Height);
                TextRenderer.DrawText(e.Graphics, e.Header.Text, e.Font, textRect.Location, text);
            }
            else e.DrawDefault = true;
        }

        void ExpandDelayTick(object sender, EventArgs e)
        {
            expandDelay.Enabled = false;
            if (View == View.Details && Columns.Count > 0)
            {
                Columns[Columns.Count - 1].Width = -2;
            }
            if (UseTheme && GridLines) // Update gridlines...
            {
                GridLines = false;
                themeGridLines = true;
            }
        }

        // Removes/hides focus cues
        protected override void OnEnter(EventArgs e)
        {
            base.OnEnter(e);
            Message m = Message.Create(Handle, Win32.WM_CHANGEUISTATE, new IntPtr(0x10001), new IntPtr(0));
            WndProc(ref m);
        }
        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            base.OnSelectedIndexChanged(e);
            Message m = Message.Create(Handle, 0x127, new IntPtr(0x10001), new IntPtr(0));
            WndProc(ref m);
        }

        protected override void WndProc(ref Message message)
        {
            base.WndProc(ref message);
            if (DesignMode) return;
            switch (message.Msg)
            {
                case Win32.WM_PAINT:
                    expandDelay.Enabled = true;
                    OnPaint(new PaintEventArgs(Graphics.FromHwnd(Handle), Bounds));
                    if (UseTheme && BorderStyle == BorderStyle.FixedSingle)
                    {
                        themeBorder = true;
                        BorderStyle = BorderStyle.None;
                    }
                    if (UseTheme && themeBorder)
                    {
                        BorderPanel.Attach(this);
                    }
                    break;
            }
        }
    }

    public class TreeViewEx : TreeView
    {
        bool themeBorder = false;
        public bool UseTheme { get; set; } = true;
        public Color BorderColor { get; set; } = SystemColors.ControlDark;

        // Removes/hides focus cues
        protected override void OnEnter(EventArgs e) // Removes focus cues
        {
            base.OnEnter(e);
            Message m = Message.Create(Handle, Win32.WM_CHANGEUISTATE, new IntPtr(0x10001), new IntPtr(0));
            WndProc(ref m);
        }
        protected override void OnAfterSelect(TreeViewEventArgs e)
        {
            base.OnAfterSelect(e);
            Message m = Message.Create(Handle, Win32.WM_CHANGEUISTATE, new IntPtr(0x10001), new IntPtr(0));
            WndProc(ref m);
        }

        protected override void WndProc(ref Message message)
        {
            base.WndProc(ref message);
            switch (message.Msg)
            {
                case Win32.WM_PAINT:
                    OnPaint(new PaintEventArgs(Graphics.FromHwnd(Handle), Bounds));
                    if (UseTheme && BorderStyle == BorderStyle.FixedSingle)
                    {
                        themeBorder = true;
                        BorderStyle = BorderStyle.None;
                    }
                    if (UseTheme && themeBorder)
                    {
                        BorderPanel.Attach(this);
                    }
                    break;
            }
        }
    }

    public class ToolStripComboBoxEx : ToolStripControlHost
    {
        public ToolStripComboBoxEx() : base(new FlatCombo())
        {
            Font font = PluginBase.Settings.DefaultFont;
            FlatCombo.FlatStyle = FlatStyle.Popup;
            FlatCombo.Font = font;
        }

        protected override Size DefaultSize => new Size(100, 22);

        public ComboBoxStyle DropDownStyle
        {
            set => FlatCombo.DropDownStyle = value;
            get => FlatCombo.DropDownStyle;
        }

        public FlatStyle FlatStyle
        {
            set => FlatCombo.FlatStyle = FlatStyle.Popup;
            get => FlatCombo.FlatStyle;
        }

        public int SelectedIndex
        {
            set => FlatCombo.SelectedIndex = value;
            get => FlatCombo.SelectedIndex;
        }

        public object SelectedItem
        {
            set => FlatCombo.SelectedItem = value;
            get => FlatCombo.SelectedItem;
        }

        public ComboBox.ObjectCollection Items => FlatCombo.Items;

        public FlatCombo FlatCombo => Control as FlatCombo;
    }

    public class FlatCombo : ComboBox, IThemeHandler
    {
        public bool UseTheme { get; set; } = true;
        ComboBoxStyle prevStyle = ComboBoxStyle.DropDown;
        Color borderColor { get; set; } = SystemColors.ControlDark;
        bool updatingStyle = false;

        public void AfterTheming()
        {
            Color fore = PluginBase.MainForm.GetThemeColor("ToolStripComboBoxControl.ForeColor");
            Color back = PluginBase.MainForm.GetThemeColor("ToolStripComboBoxControl.BackColor");
            borderColor = PluginBase.MainForm.GetThemeColor("ToolStripComboBoxControl.BorderColor", SystemColors.ControlDark);
            ForeColor = UseTheme && fore != Color.Empty ? fore : SystemColors.ControlText;
            BackColor = UseTheme && back != Color.Empty ? back : SystemColors.Window;
        }

        // Removes/hides focus cues
        protected override void OnEnter(EventArgs e)
        {
            base.OnEnter(e);
            Message m = Message.Create(Handle, Win32.WM_CHANGEUISTATE, new IntPtr(0x10001), new IntPtr(0));
            WndProc(ref m);
        }
        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            base.OnSelectedIndexChanged(e);
            Message m = Message.Create(Handle, 0x127, new IntPtr(0x10001), new IntPtr(0));
            WndProc(ref m);
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (!UseTheme) return;
            switch (m.Msg)
            {
                case Win32.WM_PAINT:
                    int pad = ScaleHelper.Scale(2);
                    int width = ScaleHelper.Scale(18);
                    Graphics g = CreateGraphics();
                    var pen = new Pen(borderColor);
                    var back = new SolidBrush(PluginBase.MainForm.GetThemeColor("ToolStripComboBoxControl.BackColor", SystemColors.Window));
                    var arrow = new SolidBrush(PluginBase.MainForm.GetThemeColor("ToolStripComboBoxControl.ForeColor", SystemColors.ControlText));
                    Rectangle backRect = new Rectangle(ClientRectangle.X, ClientRectangle.Y, ClientRectangle.Width - 1, ClientRectangle.Height - 1);
                    Rectangle dropRect = new Rectangle(ClientRectangle.Right - width, ClientRectangle.Y, width, ClientRectangle.Height);
                    if (Enabled) g.FillRectangle(back, dropRect);
                    g.DrawRectangle(pen, backRect);
                    Point middle = new Point(dropRect.Left + (dropRect.Width / 2), dropRect.Top + (dropRect.Height / 2));
                    Point[] shape = new[]
                    {
                        new Point(middle.X - pad, middle.Y - 1),
                        new Point(middle.X + pad + 1, middle.Y - 1),
                        new Point(middle.X, middle.Y + pad)
                    };
                    if (Enabled) g.FillPolygon(arrow, shape);
                    else g.FillPolygon(SystemBrushes.ControlDark, shape);
                    break;
                default:
                    break;
            }
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);
            updatingStyle = true;
            if (Enabled) DropDownStyle = prevStyle;
            else
            {
                prevStyle = DropDownStyle;
                DropDownStyle = ComboBoxStyle.DropDownList;
            }
            updatingStyle = false;
        }

        protected override void OnDropDownStyleChanged(EventArgs e)
        {
            base.OnDropDownStyleChanged(e);
            if (!updatingStyle) prevStyle = DropDownStyle;
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            borderColor = PluginBase.MainForm.GetThemeColor("ToolStripComboBoxControl.ActiveBorderColor", SystemColors.Highlight);
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            if (Focused) return;
            borderColor = PluginBase.MainForm.GetThemeColor("ToolStripComboBoxControl.BorderColor", SystemColors.ControlDark);
            Invalidate();
        }

        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);
            borderColor = PluginBase.MainForm.GetThemeColor("ToolStripComboBoxControl.BorderColor", SystemColors.ControlDark);
            Invalidate();
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            borderColor = PluginBase.MainForm.GetThemeColor("ToolStripComboBoxControl.ActiveBorderColor", SystemColors.Highlight);
            Invalidate();
        }

        protected override void OnMouseHover(EventArgs e)
        {
            base.OnMouseHover(e);
            borderColor = PluginBase.MainForm.GetThemeColor("ToolStripComboBoxControl.ActiveBorderColor", SystemColors.Highlight);
            Invalidate();
        }
    }

    public class ListBoxEx : ListBox
    {
        bool themeBorder = false;
        public bool UseTheme { get; set; } = true;
        public Color BorderColor { get; set; } = SystemColors.ControlDark;

        protected override void WndProc(ref Message message)
        {
            base.WndProc(ref message);
            switch (message.Msg)
            {
                case Win32.WM_KEYUP:
                case Win32.WM_KEYDOWN:
                case Win32.WM_MOUSEWHEEL:
                    OnPaint(new PaintEventArgs(Graphics.FromHwnd(Handle), Bounds));
                    break;
                case Win32.WM_PAINT:
                    OnPaint(new PaintEventArgs(Graphics.FromHwnd(Handle), Bounds));
                    if (UseTheme && BorderStyle == BorderStyle.FixedSingle)
                    {
                        themeBorder = true;
                        BorderStyle = BorderStyle.None;
                    }
                    if (UseTheme && themeBorder)
                    {
                        BorderPanel.Attach(this);
                    }
                    break;
            }
        }
    }

    public class CheckedListBoxEx : CheckedListBox
    {
        bool themeBorder = false;
        public bool UseTheme { get; set; } = true;
        public Color BorderColor { get; set; } = SystemColors.ControlDark;

        protected override void WndProc(ref Message message)
        {
            base.WndProc(ref message);
            switch (message.Msg)
            {
                case Win32.WM_KEYUP:
                case Win32.WM_KEYDOWN:
                case Win32.WM_MOUSEWHEEL:
                    OnPaint(new PaintEventArgs(Graphics.FromHwnd(Handle), Bounds));
                    break;
                case Win32.WM_PAINT:
                    OnPaint(new PaintEventArgs(Graphics.FromHwnd(Handle), Bounds));
                    if (UseTheme && BorderStyle == BorderStyle.FixedSingle)
                    {
                        themeBorder = true;
                        BorderStyle = BorderStyle.None;
                    }
                    if (UseTheme && themeBorder)
                    {
                        BorderPanel.Attach(this);
                    }
                    break;
            }
        }
    }

    public class TextBoxEx : TextBox
    {
        bool themeBorder = false;
        public bool UseTheme { get; set; } = true;
        public Color BorderColor { get; set; } = SystemColors.ControlDark;

        protected override void WndProc(ref Message message)
        {
            base.WndProc(ref message);
            switch (message.Msg)
            {
                case Win32.WM_PAINT:
                    OnPaint(new PaintEventArgs(Graphics.FromHwnd(Handle), Bounds));
                    if (UseTheme && BorderStyle == BorderStyle.FixedSingle)
                    {
                        themeBorder = true;
                        if (Multiline) BorderStyle = BorderStyle.None;
                    }
                    if (UseTheme && themeBorder)
                    {
                        Graphics g = CreateGraphics();
                        Rectangle r = new Rectangle(0, 0, ClientSize.Width - 1, ClientSize.Height - 1);
                        if (Multiline)
                        {
                            g.DrawRectangle(new Pen(BackColor), r);
                            BorderPanel.Attach(this);
                        }
                        else g.DrawRectangle(new Pen(BorderColor), r);
                    }
                    break;
            }
        }
    }

    public class RichTextBoxEx : RichTextBox
    {
        public bool UseTheme { get; set; } = true;
        public Color BorderColor { get; set; } = SystemColors.ControlDark;
        bool themeBorder = false;

        public void Recreate()
        {
            RecreateHandle(); // TODO: Crashes FD sometimes on start...
        }

        protected override void WndProc(ref Message message)
        {
            base.WndProc(ref message);
            switch (message.Msg)
            {
                case Win32.WM_PAINT:
                    OnPaint(new PaintEventArgs(Graphics.FromHwnd(Handle), Bounds));
                    if (UseTheme && BorderStyle == BorderStyle.FixedSingle)
                    {
                        themeBorder = true;
                        BorderStyle = BorderStyle.None;
                    }
                    if (UseTheme && themeBorder)
                    {
                        BorderPanel.Attach(this);
                    }
                    break;
            }
        }
    }

    public class ProgressBarEx : ProgressBar
    {
        public bool UseTheme { get; set; } = true;
        public Color BorderColor { get; set; } = SystemColors.ControlDark;

        public ProgressBarEx()
        {
            SetStyle(ControlStyles.UserPaint, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (UseTheme)
            {
                Rectangle rec = new Rectangle(0, 0, Width, Height);
                double scaleFactor = ((Value - (double)Minimum) / (Maximum - (double)Minimum));
                rec.Width = (int)((rec.Width * scaleFactor) - 2); rec.Height -= 2;
                e.Graphics.FillRectangle(new SolidBrush(BackColor), new Rectangle(0, 0, Width - 1, Height - 1));
                e.Graphics.FillRectangle(new SolidBrush(ForeColor), 1, 1, rec.Width, rec.Height);
                e.Graphics.DrawRectangle(new Pen(BorderColor), new Rectangle(0, 0, Width - 1, Height - 1));
            }
            else base.OnPaint(e);
        }
    }

    public class GroupBoxEx : GroupBox
    {
        public bool UseTheme { get; set; } = true;
        public Color BorderColor { get; set; } = SystemColors.ControlDark;

        protected override void OnPaint(PaintEventArgs e)
        {
            if (UseTheme)
            {
                Size tSize = TextRenderer.MeasureText(Text, Font);
                Rectangle borderRect = ClientRectangle;
                borderRect.Y = (borderRect.Y + (tSize.Height / 2));
                borderRect.Height = (borderRect.Height - (tSize.Height / 2));
                ControlPaint.DrawBorder(e.Graphics, borderRect, BorderColor, ButtonBorderStyle.Solid);
                Rectangle textRect = ClientRectangle;
                textRect.X = (textRect.X + 6);
                textRect.Width = tSize.Width;
                textRect.Height = tSize.Height;
                e.Graphics.FillRectangle(new SolidBrush(BackColor), textRect);
                TextRenderer.DrawText(e.Graphics, Text, Font, textRect, ForeColor);
            }
            else base.OnPaint(e);
        }
    }

    public class PropertyGridEx : PropertyGrid
    {
        public bool UseTheme { get; set; } = true;

        public PropertyGridEx()
        {
            SelectedObjectsChanged += OnSelectedObjectsChanged;
        }

        public ScrollBar GetScrollBar()
        {
            foreach (Control ctrl in Controls)
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

        void OnValueChanged(object sender, EventArgs e)
        {
            OnPaint(new PaintEventArgs(Graphics.FromHwnd(Handle), Bounds));
        }

        void OnSelectedObjectsChanged(object sender, EventArgs e)
        {
            ScrollBar scrollBar = GetScrollBar();
            if (scrollBar != null) scrollBar.ValueChanged += OnValueChanged;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (DesignMode) return;
            if (UseTheme)
            {
                Color color = PluginBase.MainForm.GetThemeColor("PropertyGrid.BackColor", SystemColors.Control);
                e.Graphics.FillRectangle(new SolidBrush(color), ClientRectangle);
            }
        }
    }

    public class PictureBoxEx : PictureBox
    {
        public bool UseTheme { get; set; } = true;
        public Color BorderColor { get; set; } = SystemColors.ControlDark;
        bool themeBorder = false;

        protected override void WndProc(ref Message message)
        {
            base.WndProc(ref message);
            switch (message.Msg)
            {
                case Win32.WM_PAINT:
                    if (UseTheme && BorderStyle == BorderStyle.FixedSingle)
                    {
                        themeBorder = true;
                        BorderStyle = BorderStyle.None;
                    }
                    if (UseTheme && themeBorder)
                    {
                        BorderPanel.Attach(this);
                    }
                    break;
            }
        }
    }

    public class ButtonEx : Button
    {
        public bool UseTheme { get; set; } = true;
        public Color DisabledTextColor { get; set; } = SystemColors.ControlDark;
        public Color DisabledBackColor { get; set; } = SystemColors.Control;

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (UseTheme && !Enabled)
            {
                e.Graphics.FillRectangle(new SolidBrush(DisabledBackColor), Rectangle.Inflate(ClientRectangle, -2, -2));
                TextRenderer.DrawText(e.Graphics, Text, Font, ClientRectangle, DisabledTextColor);
                if (Image != null)
                {
                    var x = (Width / 2) - (Image.Width / 2);
                    var y = (Height / 2) - (Image.Height / 2);
                    ControlPaint.DrawImageDisabled(e.Graphics, Image, x, y, Color.Transparent);
                }
            }
        }
    }

    public class CheckBoxEx : CheckBox
    {
        public bool UseTheme { get; set; } = true;
        public Color DisabledTextColor { get; set; } = SystemColors.ControlDark;
        public Color BorderColor { get; set; } = SystemColors.ControlDark;

        protected override void OnPaint(PaintEventArgs e)
        {
            if (!DesignMode && UseTheme && PluginBase.MainForm.GetThemeColor("CheckBox.BackColor") != Color.Empty)
            {
                Size size = SystemInformation.MenuCheckSize;
                var offset = (ClientRectangle.Height - 1) - size.Height;
                var flags = TextFormatFlags.Left | TextFormatFlags.VerticalCenter;
                var checkRect = new Rectangle(ClientRectangle.X, ClientRectangle.Y + offset, size.Width - 2, size.Height - 2);
                var innerRect = new Rectangle(ClientRectangle.X + 3, ClientRectangle.Y + offset + 3, size.Width - 7, size.Height - 7);
                var textRect = new Rectangle(ClientRectangle.Location, ClientRectangle.Size);
                if (RightToLeft == RightToLeft.Yes)
                {
                    offset = ClientRectangle.Width - size.Width;
                    flags = TextFormatFlags.Right | TextFormatFlags.VerticalCenter;
                    textRect.Offset(-size.Width, 0);
                    checkRect.Offset(offset, 0);
                }
                else textRect.Offset(checkRect.Width + 3, 0);
                Color back = FlatAppearance.CheckedBackColor;
                if (ClientRectangle.Contains(PointToClient(MousePosition)))
                {
                    if (MouseButtons == MouseButtons.Left) back = FlatAppearance.MouseDownBackColor;
                    else back = FlatAppearance.MouseOverBackColor;
                }
                ButtonRenderer.DrawParentBackground(e.Graphics, ClientRectangle, this);
                e.Graphics.FillRectangle(new SolidBrush(BackColor), ClientRectangle);
                e.Graphics.FillRectangle(new SolidBrush(back), checkRect);
                e.Graphics.DrawRectangle(new Pen(BorderColor), checkRect);
                if (CheckState == CheckState.Indeterminate)
                {
                    e.Graphics.FillRectangle(new SolidBrush(BackColor), innerRect);
                }
                else if (CheckState == CheckState.Checked)
                {
                    Image image = PluginBase.MainForm.FindImageAndSetAdjust("485");
                    e.Graphics.DrawImage(image, checkRect, new Rectangle(Point.Empty, image.Size), GraphicsUnit.Pixel);
                }
                TextRenderer.DrawText(e.Graphics, Text, Font, textRect, ForeColor, flags);
            }
            else base.OnPaint(e);
        }
    }

    public class FormEx : Form
    {
        public virtual bool UseTheme { get; set; } = true;

        public FormEx()
        {
            ResizeRedraw = true;
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (!UseTheme)
            {
                base.OnPaintBackground(e);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (UseTheme)
            {
                Color color = PluginBase.MainForm.GetThemeColor("Form.BackColor", SystemColors.Control);
                e.Graphics.FillRectangle(new SolidBrush(color), ClientRectangle);
                if (WindowState != FormWindowState.Maximized && (FormBorderStyle == FormBorderStyle.Sizable || FormBorderStyle == FormBorderStyle.SizableToolWindow))
                {
                    SizeGripStyle = SizeGripStyle.Hide;
                    Color dark = PluginBase.MainForm.GetThemeColor("Form.3dDarkColor", SystemColors.ControlDark);
                    Color light = PluginBase.MainForm.GetThemeColor("Form.3dLightColor", SystemColors.ControlLight);
                    using SolidBrush darkBrush = new SolidBrush(dark), lightBrush = new SolidBrush(light);
                    int y = ClientRectangle.Bottom - 3 * 2 + 1;
                    for (int i = 3; i >= 1; i--)
                    {
                        int x = (ClientRectangle.Right - 3 * 2 + 1);
                        for (int j = 0; j < i; j++)
                        {
                            e.Graphics.FillRectangle(lightBrush, x + 1, y + 1, 2, 2);
                            e.Graphics.FillRectangle(darkBrush, x, y, 2, 2);
                            x -= 4;
                        }
                        y -= 4;
                    }
                }
            }
        }
    }

    public class TabControlEx : CustomTabControl, IThemeHandler
    {
        public bool UseTheme => PluginBase.MainForm.GetThemeFlag("TabControl.UseTheme", false);

        public TabControlEx()
        {
            if (UseTheme) DisplayStyle = TabStyle.Flat;
        }

        void MatchBackColor(Control parent)
        {
            foreach (Control ctrl in parent.Controls)
            {
                if (ctrl is Label || ctrl is CheckBox || ctrl is GroupBox)
                {
                    ctrl.BackColor = Color.Empty;
                }
                MatchBackColor(ctrl);
            }
        }

        public void AfterTheming()
        {
            MatchBackColor(this);
        }
    }

    public class StatusBarEx : StatusBar
    {
        public bool UseTheme { get; set; } = true;

        protected override void WndProc(ref Message message)
        {
            base.WndProc(ref message);
            if (DesignMode) return;
            switch (message.Msg)
            {
                case Win32.WM_PAINT:
                    OnPaint(new PaintEventArgs(Graphics.FromHwnd(Handle), Bounds));
                    if (UseTheme)
                    {
                        Graphics g = CreateGraphics();
                        Color back = PluginBase.MainForm.GetThemeColor("StatusBar.BackColor", SystemColors.Control);
                        Color fore = PluginBase.MainForm.GetThemeColor("StatusBar.ForeColor", SystemColors.ControlText);
                        g.FillRectangle(new SolidBrush(back), ClientRectangle);
                        g.DrawLine(SystemPens.ControlDark, new Point(ClientRectangle.X, ClientRectangle.Y + 1), new Point(ClientRectangle.Width, ClientRectangle.Y + 1));
                        if (SizingGrip)
                        {
                            Color dark = PluginBase.MainForm.GetThemeColor("StatusBar.3dDarkColor", SystemColors.ControlDark);
                            Color light = PluginBase.MainForm.GetThemeColor("StatusBar.3dLightColor", SystemColors.ControlLight);
                            using SolidBrush darkBrush = new SolidBrush(dark), lightBrush = new SolidBrush(light);
                            int y = ClientRectangle.Bottom - 3 * 2 + 1;
                            for (int i = 3; i >= 1; i--)
                            {
                                int x = (ClientRectangle.Right - 3 * 2 + 1);
                                for (int j = 0; j < i; j++)
                                {
                                    g.FillRectangle(lightBrush, x + 1, y + 1, 2, 2);
                                    g.FillRectangle(darkBrush, x, y, 2, 2);
                                    x -= 4;
                                }
                                y -= 4;
                            }
                        }
                        var tff = TextFormatFlags.VerticalCenter;
                        TextRenderer.DrawText(g, Text, Font, new Point(0, (Height / 2) + 2), fore, tff);
                    }
                    break;
            }
        }
    }

    public class ToolStripProgressBarEx : ToolStripProgressBar
    {
        static readonly Padding defaultMargin = new Padding(1, 2, 1, 1);
        static readonly Padding defaultStatusStripMargin = new Padding(1, 5, 1, 4);

        public ToolStripProgressBarEx()
        {
            OverrideControl();
            Font = PluginBase.Settings.DefaultFont;
            ProgressBar.ForeColor = PluginBase.MainForm.GetThemeColor("ToolStripProgressBar.ForeColor", SystemColors.Highlight);
            ProgressBar.Margin = DefaultMargin;
            ProgressBar.Size = DefaultSize;
        }

        void OverrideControl()
        {
            OnUnsubscribeControlEvents(Control);
            Type type = GetType();
            FieldInfo prop = type.BaseType.BaseType.GetField("control", BindingFlags.NonPublic | BindingFlags.Instance);
            prop.SetValue(this, new ProgressBarEx());
            OnSubscribeControlEvents(Control);
            Invalidate();
        }

        protected override Size DefaultSize => new Size(100, 12);

        protected override Padding DefaultMargin
        {
            get
            {
                if (Owner is StatusStrip)
                {
                    return defaultStatusStripMargin;
                }

                return defaultMargin;
            }
        }
    }

    public class ToolStripSpringComboBox : ToolStripComboBoxEx
    {
        public ToolStripSpringComboBox()
        {
            Control.PreviewKeyDown += OnPreviewKeyDown;
        }

        /// <summary>
        /// Fixes the Control+Alt (AltGr) key combination handling
        /// </summary>
        void OnPreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            Keys ctrlAlt = Keys.Control | Keys.Alt;
            if ((e.KeyData & ctrlAlt) == ctrlAlt) e.IsInputKey = true;
        }

        /// <summary>
        /// Makes the control spring automatically
        /// </summary>
        public override Size GetPreferredSize(Size constrainingSize)
        {
            // Use the default size if the text box is on the overflow menu
            // or is on a vertical ToolStrip.
            if (IsOnOverflow || Owner.Orientation == Orientation.Vertical)
            {
                return DefaultSize;
            }
            // Declare a variable to store the total available width as 
            // it is calculated, starting with the display width of the 
            // owning ToolStrip.
            int width = Owner.DisplayRectangle.Width;
            // Subtract the width of the overflow button if it is displayed. 
            if (Owner.OverflowButton.Visible)
            {
                width = width - Owner.OverflowButton.Width - Owner.OverflowButton.Margin.Horizontal;
            }
            // Declare a variable to maintain a count of ToolStripSpringComboBox 
            // items currently displayed in the owning ToolStrip. 
            int springBoxCount = 0;
            foreach (ToolStripItem item in Owner.Items)
            {
                // Ignore items on the overflow menu.
                if (item.IsOnOverflow) continue;
                if (item is ToolStripSpringComboBox)
                {
                    // For ToolStripSpringComboBox items, increment the count and 
                    // subtract the margin width from the total available width.
                    springBoxCount++;
                    width -= item.Margin.Horizontal;
                }
                else
                {
                    // For all other items, subtract the full width from the total
                    // available width.
                    width = width - item.Width - item.Margin.Horizontal;
                }
            }
            // If there are multiple ToolStripSpringComboBox items in the owning
            // ToolStrip, divide the total available width between them. 
            if (springBoxCount > 1) width /= springBoxCount;
            // If the available width is less than the default width, use the
            // default width, forcing one or more items onto the overflow menu.
            if (width < DefaultSize.Width) width = DefaultSize.Width;
            // Retrieve the preferred size from the base class, but change the
            // width to the calculated width. 
            Size size = base.GetPreferredSize(constrainingSize);
            size.Width = width;
            return size;
        }

    }

    public class ToolStripSpringTextBox : ToolStripTextBox
    {
        public ToolStripSpringTextBox()
        {
            Control.PreviewKeyDown += OnPreviewKeyDown;
        }

        /// <summary>
        /// Fixes the Control+Alt (AltGr) key combination handling
        /// </summary>
        void OnPreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            Keys ctrlAlt = Keys.Control | Keys.Alt;
            if ((e.KeyData & ctrlAlt) == ctrlAlt) e.IsInputKey = true;
        }

        /// <summary>
        /// Makes the control spring automatically
        /// </summary>
        public override Size GetPreferredSize(Size constrainingSize)
        {
            // Use the default size if the text box is on the overflow menu
            // or is on a vertical ToolStrip.
            if (IsOnOverflow || Owner.Orientation == Orientation.Vertical)
            {
                return DefaultSize;
            }
            // Declare a variable to store the total available width as 
            // it is calculated, starting with the display width of the 
            // owning ToolStrip.
            int width = Owner.DisplayRectangle.Width;
            // Subtract the width of the overflow button if it is displayed. 
            if (Owner.OverflowButton.Visible)
            {
                width = width - Owner.OverflowButton.Width - Owner.OverflowButton.Margin.Horizontal;
            }
            // Declare a variable to maintain a count of ToolStripSpringTextBox 
            // items currently displayed in the owning ToolStrip. 
            int springBoxCount = 0;
            foreach (ToolStripItem item in Owner.Items)
            {
                // Ignore items on the overflow menu.
                if (item.IsOnOverflow) continue;
                if (item is ToolStripSpringTextBox)
                {
                    // For ToolStripSpringTextBox items, increment the count and 
                    // subtract the margin width from the total available width.
                    springBoxCount++;
                    width -= item.Margin.Horizontal;
                }
                else
                {
                    // For all other items, subtract the full width from the total
                    // available width.
                    width = width - item.Width - item.Margin.Horizontal;
                }
            }
            // If there are multiple ToolStripSpringTextBox items in the owning
            // ToolStrip, divide the total available width between them. 
            if (springBoxCount > 1) width /= springBoxCount;
            // If the available width is less than the default width, use the
            // default width, forcing one or more items onto the overflow menu.
            if (width < DefaultSize.Width) width = DefaultSize.Width;
            // Retrieve the preferred size from the base class, but change the
            // width to the calculated width. 
            Size size = base.GetPreferredSize(constrainingSize);
            size.Width = width;
            return size;
        }

    }

    public class DescriptiveCollectionEditor : CollectionEditor
    {
        public DescriptiveCollectionEditor(Type type) : base(type) {}
        
        protected override CollectionForm CreateCollectionForm()
        {
            CollectionForm form = base.CreateCollectionForm();
            form.Shown += delegate
            {
                ShowDescription(form);
            };
            return form;
        }

        static void ShowDescription(Control control)
        {
            PropertyGrid grid = control as PropertyGrid;
            if (grid != null) grid.HelpVisible = true;
            foreach (Control child in control.Controls)
            {
                ShowDescription(child);
            }
        }

    }

}
