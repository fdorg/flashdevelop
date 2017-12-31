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
        public static Int32 BorderWidth { get; set; } = 1;

        /// <summary>
        /// Should we use custom border panel?
        /// </summary>
        public static Boolean UseCustomBorder
        {
            get { return PluginBase.MainForm.GetThemeFlag("ThemeManager.UseCustomBorder", false); }
        }

        /// <summary>
        /// Adds a wrapper panel to the control to create a border.
        /// </summary>
        public static BorderPanel Attach(Control ctrl)
        {
            if (!UseCustomBorder) return null;
            if (ctrl.Parent is TableLayoutPanel) return null;
            return Attach(ctrl, true);
        }
        public static BorderPanel Attach(Control ctrl, Boolean visible)
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
            if (!panel.Location.Equals(s)) panel.Location = l;
            if (!ctrl.Anchor.Equals(ctrl.Anchor)) panel.Anchor = ctrl.Anchor;
            if (panel.Visible != v) panel.Visible = v;
            return panel;
        }

        /// <summary>
        /// Gets the correct color for the bordered control
        /// </summary>
        private Color GetBorderColor()
        {
            if (this.Tag is Control)
            {
                String name = ThemeHelper.GetFilteredTypeName(this.Tag.GetType());
                return PluginBase.MainForm.GetThemeColor(name + ".BorderColor", SystemColors.ControlDark);
            }
            else return SystemColors.ControlDark;
        }

        /// <summary>
        /// Paint only background for the border
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.FillRectangle(new SolidBrush(this.GetBorderColor()), this.ClientRectangle);
        }

    }

    public class DataGridViewEx : DataGridView, IThemeHandler
    {
        private Boolean themeBorder = false;
        public Boolean UseTheme { get; set; } = true;
        public Color BorderColor { get; set; } = SystemColors.ControlDark;

        public DataGridViewEx()
        {
            this.CellPainting += this.OnDataGridViewCellPainting;
        }

        public void AfterTheming()
        {
            GridColor = PluginBase.MainForm.GetThemeColor("DataGridView.LineColor", SystemColors.ControlDark);
            DefaultCellStyle.ForeColor = PluginBase.MainForm.GetThemeColor("DataGridView.ForeColor", SystemColors.WindowText);
            DefaultCellStyle.BackColor = PluginBase.MainForm.GetThemeColor("DataGridView.BackColor", SystemColors.Window);
        }

        private void OnDataGridViewCellPainting(Object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (this.DesignMode) return;
            if (e.RowIndex == -1)
            {
                Color back = PluginBase.MainForm.GetThemeColor("ColumnHeader.BackColor");
                Color text = PluginBase.MainForm.GetThemeColor("ColumnHeader.TextColor");
                Color border = PluginBase.MainForm.GetThemeColor("ColumnHeader.BorderColor");
                if (back != Color.Empty && border != Color.Empty && text != Color.Empty)
                {
                    this.EnableHeadersVisualStyles = false;
                    this.ColumnHeadersDefaultCellStyle.ForeColor = text;
                    e.Graphics.FillRectangle(new SolidBrush(back), e.CellBounds);
                    e.Graphics.DrawLine(new Pen(border), e.CellBounds.X, e.CellBounds.Height - 1, e.CellBounds.X + e.CellBounds.Width, e.CellBounds.Height - 1);
                    e.Graphics.DrawLine(new Pen(border), e.CellBounds.X + e.CellBounds.Width - 1, 3, e.CellBounds.X + e.CellBounds.Width - 1, e.CellBounds.Height - 6);
                    e.PaintContent(e.ClipBounds);
                    e.Handled = true;
                }
                else this.EnableHeadersVisualStyles = true;
            }
        }

        protected override void WndProc(ref Message message)
        {
            base.WndProc(ref message);
            if (this.DesignMode) return;
            switch (message.Msg)
            {
                case Win32.WM_PAINT:
                    if (this.UseTheme && this.BorderStyle == BorderStyle.FixedSingle)
                    {
                        this.themeBorder = true;
                        this.BorderStyle = BorderStyle.None;
                    }
                    if (this.UseTheme && this.themeBorder)
                    {
                        BorderPanel.Attach(this);
                    }
                    break;
            }
        }

    }

    public class ListViewEx : ListView
    {
        private Timer expandDelay;
        private Boolean themeBorder = false;
        public Boolean UseTheme { get; set; } = true;
        public Color GridLineColor { get; set; } = SystemColors.Control;
        public Color BorderColor { get; set; } = SystemColors.ControlDark;
        private Boolean themeGridLines = false;

        public ListViewEx()
        {
            this.OwnerDraw = true;
            this.DoubleBuffered = true;
            this.DrawColumnHeader += this.OnDrawColumnHeader;
            this.DrawSubItem += this.OnDrawSubItem;
            this.DrawItem += this.OnDrawItem;
            this.expandDelay = new Timer();
            this.expandDelay.Interval = 50;
            this.expandDelay.Tick += this.ExpandDelayTick;
            this.expandDelay.Enabled = true;
            this.expandDelay.Start();
            base.GridLines = false;
        }

        private void OnDrawItem(object sender, DrawListViewItemEventArgs e)
        {
            e.DrawDefault = true;
            if (this.themeGridLines && this.Items.Count > 0)
            {
                Pen pen = new Pen(this.GridLineColor);
                e.Graphics.DrawLine(pen, new Point(e.Bounds.Left, e.Bounds.Top - 1), new Point(e.Bounds.Right, e.Bounds.Top - 1));
            }
        }

        private void OnDrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            e.DrawDefault = true;
            if (this.themeGridLines && this.Items.Count > 0)
            {
                Pen pen = new Pen(this.GridLineColor);
                e.Graphics.DrawLine(pen, new Point(e.Bounds.Left - 1, e.Bounds.Top), new Point(e.Bounds.Left - 1, e.Bounds.Bottom));
            }
        }

        protected virtual void OnDrawColumnHeader(Object sender, DrawListViewColumnHeaderEventArgs e)
        {
            Color back = PluginBase.MainForm.GetThemeColor("ColumnHeader.BackColor");
            Color text = PluginBase.MainForm.GetThemeColor("ColumnHeader.TextColor");
            Color border = PluginBase.MainForm.GetThemeColor("ColumnHeader.BorderColor");
            if (this.UseTheme && back != Color.Empty && border != Color.Empty && text != Color.Empty)
            {
                e.Graphics.FillRectangle(new SolidBrush(back), e.Bounds.X, 0, e.Bounds.Width, e.Bounds.Height);
                e.Graphics.DrawLine(new Pen(border), e.Bounds.X, e.Bounds.Height - 1, e.Bounds.X + e.Bounds.Width, e.Bounds.Height - 1);
                e.Graphics.DrawLine(new Pen(border), e.Bounds.X + e.Bounds.Width - 1, 3, e.Bounds.X + e.Bounds.Width - 1, e.Bounds.Height - 6);
                Int32 textHeight = TextRenderer.MeasureText("HeightTest", e.Font).Height + 1;
                Rectangle textRect = new Rectangle(e.Bounds.X + 3, e.Bounds.Y + (e.Bounds.Height / 2) - (textHeight / 2), e.Bounds.Width, e.Bounds.Height);
                TextRenderer.DrawText(e.Graphics, e.Header.Text, e.Font, textRect.Location, text);
            }
            else e.DrawDefault = true;
        }

        private void ExpandDelayTick(object sender, EventArgs e)
        {
            this.expandDelay.Enabled = false;
            if (this.View == View.Details && this.Columns.Count > 0)
            {
                this.Columns[this.Columns.Count - 1].Width = -2;
            }
            if (this.UseTheme && this.GridLines) // Update gridlines...
            {
                base.GridLines = false;
                this.themeGridLines = true;
            }
        }

        // Removes/hides focus cues
        protected override void OnEnter(EventArgs e)
        {
            base.OnEnter(e);
            Message m = Message.Create(this.Handle, Win32.WM_CHANGEUISTATE, new IntPtr(0x10001), new IntPtr(0));
            this.WndProc(ref m);
        }
        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            base.OnSelectedIndexChanged(e);
            Message m = Message.Create(this.Handle, 0x127, new IntPtr(0x10001), new IntPtr(0));
            this.WndProc(ref m);
        }

        protected override void WndProc(ref Message message)
        {
            base.WndProc(ref message);
            if (this.DesignMode) return;
            switch (message.Msg)
            {
                case Win32.WM_PAINT:
                    this.expandDelay.Enabled = true;
                    this.OnPaint(new PaintEventArgs(Graphics.FromHwnd(this.Handle), this.Bounds));
                    if (this.UseTheme && this.BorderStyle == BorderStyle.FixedSingle)
                    {
                        this.themeBorder = true;
                        this.BorderStyle = BorderStyle.None;
                    }
                    if (this.UseTheme && this.themeBorder)
                    {
                        BorderPanel.Attach(this);
                    }
                    break;
            }
        }
    }

    public class TreeViewEx : TreeView
    {
        private Boolean themeBorder = false;
        public Boolean UseTheme { get; set; } = true;
        public Color BorderColor { get; set; } = SystemColors.ControlDark;

        // Removes/hides focus cues
        protected override void OnEnter(EventArgs e) // Removes focus cues
        {
            base.OnEnter(e);
            Message m = Message.Create(this.Handle, Win32.WM_CHANGEUISTATE, new IntPtr(0x10001), new IntPtr(0));
            this.WndProc(ref m);
        }
        protected override void OnAfterSelect(TreeViewEventArgs e)
        {
            base.OnAfterSelect(e);
            Message m = Message.Create(this.Handle, Win32.WM_CHANGEUISTATE, new IntPtr(0x10001), new IntPtr(0));
            this.WndProc(ref m);
        }

        protected override void WndProc(ref Message message)
        {
            base.WndProc(ref message);
            switch (message.Msg)
            {
                case Win32.WM_PAINT:
                    this.OnPaint(new PaintEventArgs(Graphics.FromHwnd(this.Handle), this.Bounds));
                    if (this.UseTheme && this.BorderStyle == BorderStyle.FixedSingle)
                    {
                        this.themeBorder = true;
                        this.BorderStyle = BorderStyle.None;
                    }
                    if (this.UseTheme && this.themeBorder)
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
            this.FlatCombo.FlatStyle = FlatStyle.Popup;
            this.FlatCombo.Font = font;
        }

        protected override Size DefaultSize
        {
            get { return new Size(100, 22); }
        }

        public ComboBoxStyle DropDownStyle
        {
            set { this.FlatCombo.DropDownStyle = value; }
            get { return this.FlatCombo.DropDownStyle; }
        }

        public FlatStyle FlatStyle
        {
            set { this.FlatCombo.FlatStyle = FlatStyle.Popup; }
            get { return this.FlatCombo.FlatStyle; }
        }

        public Int32 SelectedIndex
        {
            set { this.FlatCombo.SelectedIndex = value; }
            get { return this.FlatCombo.SelectedIndex; }
        }

        public Object SelectedItem
        {
            set { this.FlatCombo.SelectedItem = value; }
            get { return this.FlatCombo.SelectedItem; }
        }

        public FlatCombo.ObjectCollection Items
        {
            get { return this.FlatCombo.Items; }
        }

        public FlatCombo FlatCombo
        {
            get { return this.Control as FlatCombo; }
        }
    }

    public class FlatCombo : ComboBox, IThemeHandler
    {
        public Boolean UseTheme { get; set; } = true;
        private ComboBoxStyle prevStyle = ComboBoxStyle.DropDown;
        private Color borderColor { get; set; } = SystemColors.ControlDark;
        private Boolean updatingStyle = false;

        public void AfterTheming()
        {
            Color fore = PluginBase.MainForm.GetThemeColor("ToolStripComboBoxControl.ForeColor");
            Color back = PluginBase.MainForm.GetThemeColor("ToolStripComboBoxControl.BackColor");
            this.borderColor = PluginBase.MainForm.GetThemeColor("ToolStripComboBoxControl.BorderColor", SystemColors.ControlDark);
            this.ForeColor = UseTheme && fore != Color.Empty ? fore : SystemColors.ControlText;
            this.BackColor = UseTheme && back != Color.Empty ? back : SystemColors.Window;
        }

        // Removes/hides focus cues
        protected override void OnEnter(EventArgs e)
        {
            base.OnEnter(e);
            Message m = Message.Create(this.Handle, Win32.WM_CHANGEUISTATE, new IntPtr(0x10001), new IntPtr(0));
            this.WndProc(ref m);
        }
        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            base.OnSelectedIndexChanged(e);
            Message m = Message.Create(this.Handle, 0x127, new IntPtr(0x10001), new IntPtr(0));
            this.WndProc(ref m);
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (!this.UseTheme) return;
            switch (m.Msg)
            {
                case Win32.WM_PAINT:
                    Int32 pad = ScaleHelper.Scale(2);
                    Int32 width = ScaleHelper.Scale(18);
                    Graphics g = this.CreateGraphics();
                    var pen = new Pen(this.borderColor);
                    var back = new SolidBrush(PluginBase.MainForm.GetThemeColor("ToolStripComboBoxControl.BackColor", SystemColors.Window));
                    var arrow = new SolidBrush(PluginBase.MainForm.GetThemeColor("ToolStripComboBoxControl.ForeColor", SystemColors.ControlText));
                    Rectangle backRect = new Rectangle(this.ClientRectangle.X, this.ClientRectangle.Y, this.ClientRectangle.Width - 1, this.ClientRectangle.Height - 1);
                    Rectangle dropRect = new Rectangle(this.ClientRectangle.Right - width, this.ClientRectangle.Y, width, this.ClientRectangle.Height);
                    if (this.Enabled) g.FillRectangle(back, dropRect);
                    g.DrawRectangle(pen, backRect);
                    Point middle = new Point(dropRect.Left + (dropRect.Width / 2), dropRect.Top + (dropRect.Height / 2));
                    Point[] shape = new Point[]
                    {
                        new Point(middle.X - pad, middle.Y - 1),
                        new Point(middle.X + pad + 1, middle.Y - 1),
                        new Point(middle.X, middle.Y + pad)
                    };
                    if (this.Enabled) g.FillPolygon(arrow, shape);
                    else g.FillPolygon(SystemBrushes.ControlDark, shape);
                    break;
                default:
                    break;
            }
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);
            this.updatingStyle = true;
            if (this.Enabled) this.DropDownStyle = this.prevStyle;
            else
            {
                this.prevStyle = this.DropDownStyle;
                this.DropDownStyle = ComboBoxStyle.DropDownList;
            }
            this.updatingStyle = false;
        }

        protected override void OnDropDownStyleChanged(EventArgs e)
        {
            base.OnDropDownStyleChanged(e);
            if (!this.updatingStyle) this.prevStyle = this.DropDownStyle;
        }

        protected override void OnMouseEnter(System.EventArgs e)
        {
            base.OnMouseEnter(e);
            this.borderColor = PluginBase.MainForm.GetThemeColor("ToolStripComboBoxControl.ActiveBorderColor", SystemColors.Highlight);
            this.Invalidate();
        }

        protected override void OnMouseLeave(System.EventArgs e)
        {
            base.OnMouseLeave(e);
            if (this.Focused) return;
            this.borderColor = PluginBase.MainForm.GetThemeColor("ToolStripComboBoxControl.BorderColor", SystemColors.ControlDark);
            this.Invalidate();
        }

        protected override void OnLostFocus(System.EventArgs e)
        {
            base.OnLostFocus(e);
            this.borderColor = PluginBase.MainForm.GetThemeColor("ToolStripComboBoxControl.BorderColor", SystemColors.ControlDark);
            this.Invalidate();
        }

        protected override void OnGotFocus(System.EventArgs e)
        {
            base.OnGotFocus(e);
            this.borderColor = PluginBase.MainForm.GetThemeColor("ToolStripComboBoxControl.ActiveBorderColor", SystemColors.Highlight);
            this.Invalidate();
        }

        protected override void OnMouseHover(System.EventArgs e)
        {
            base.OnMouseHover(e);
            this.borderColor = PluginBase.MainForm.GetThemeColor("ToolStripComboBoxControl.ActiveBorderColor", SystemColors.Highlight);
            this.Invalidate();
        }
    }

    public class ListBoxEx : ListBox
    {
        private Boolean themeBorder = false;
        public Boolean UseTheme { get; set; } = true;
        public Color BorderColor { get; set; } = SystemColors.ControlDark;

        protected override void WndProc(ref Message message)
        {
            base.WndProc(ref message);
            switch (message.Msg)
            {
                case Win32.WM_KEYUP:
                case Win32.WM_KEYDOWN:
                case Win32.WM_MOUSEWHEEL:
                    this.OnPaint(new PaintEventArgs(Graphics.FromHwnd(this.Handle), this.Bounds));
                    break;
                case Win32.WM_PAINT:
                    this.OnPaint(new PaintEventArgs(Graphics.FromHwnd(this.Handle), this.Bounds));
                    if (this.UseTheme && this.BorderStyle == BorderStyle.FixedSingle)
                    {
                        this.themeBorder = true;
                        this.BorderStyle = BorderStyle.None;
                    }
                    if (this.UseTheme && this.themeBorder)
                    {
                        BorderPanel.Attach(this);
                    }
                    break;
            }
        }
    }

    public class CheckedListBoxEx : CheckedListBox
    {
        private Boolean themeBorder = false;
        public Boolean UseTheme { get; set; } = true;
        public Color BorderColor { get; set; } = SystemColors.ControlDark;

        protected override void WndProc(ref Message message)
        {
            base.WndProc(ref message);
            switch (message.Msg)
            {
                case Win32.WM_KEYUP:
                case Win32.WM_KEYDOWN:
                case Win32.WM_MOUSEWHEEL:
                    this.OnPaint(new PaintEventArgs(Graphics.FromHwnd(this.Handle), this.Bounds));
                    break;
                case Win32.WM_PAINT:
                    this.OnPaint(new PaintEventArgs(Graphics.FromHwnd(this.Handle), this.Bounds));
                    if (this.UseTheme && this.BorderStyle == BorderStyle.FixedSingle)
                    {
                        this.themeBorder = true;
                        this.BorderStyle = BorderStyle.None;
                    }
                    if (this.UseTheme && this.themeBorder)
                    {
                        BorderPanel.Attach(this);
                    }
                    break;
            }
        }
    }

    public class TextBoxEx : TextBox
    {
        private Boolean themeBorder = false;
        public Boolean UseTheme { get; set; } = true;
        public Color BorderColor { get; set; } = SystemColors.ControlDark;

        protected override void WndProc(ref Message message)
        {
            base.WndProc(ref message);
            switch (message.Msg)
            {
                case Win32.WM_PAINT:
                    this.OnPaint(new PaintEventArgs(Graphics.FromHwnd(this.Handle), this.Bounds));
                    if (this.UseTheme && this.BorderStyle == BorderStyle.FixedSingle)
                    {
                        this.themeBorder = true;
                        if (this.Multiline) this.BorderStyle = BorderStyle.None;
                    }
                    if (this.UseTheme && this.themeBorder)
                    {
                        Graphics g = this.CreateGraphics();
                        Rectangle r = new Rectangle(0, 0, this.ClientSize.Width - 1, this.ClientSize.Height - 1);
                        if (this.Multiline)
                        {
                            g.DrawRectangle(new Pen(this.BackColor), r);
                            BorderPanel.Attach(this);
                        }
                        else g.DrawRectangle(new Pen(this.BorderColor), r);
                    }
                    break;
            }
        }
    }

    public class RichTextBoxEx : RichTextBox
    {
        public Boolean UseTheme { get; set; } = true;
        public Color BorderColor { get; set; } = SystemColors.ControlDark;
        private Boolean themeBorder = false;

        public void Recreate()
        {
            this.RecreateHandle(); // TODO: Crashes FD sometimes on start...
        }

        protected override void WndProc(ref Message message)
        {
            base.WndProc(ref message);
            switch (message.Msg)
            {
                case Win32.WM_PAINT:
                    this.OnPaint(new PaintEventArgs(Graphics.FromHwnd(this.Handle), this.Bounds));
                    if (this.UseTheme && this.BorderStyle == BorderStyle.FixedSingle)
                    {
                        this.themeBorder = true;
                        this.BorderStyle = BorderStyle.None;
                    }
                    if (this.UseTheme && this.themeBorder)
                    {
                        BorderPanel.Attach(this);
                    }
                    break;
            }
        }
    }

    public class ProgressBarEx : ProgressBar
    {
        public Boolean UseTheme { get; set; } = true;
        public Color BorderColor { get; set; } = SystemColors.ControlDark;

        public ProgressBarEx()
        {
            this.SetStyle(ControlStyles.UserPaint, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (this.UseTheme)
            {
                Rectangle rec = new Rectangle(0, 0, this.Width, this.Height);
                double scaleFactor = (((double)Value - (double)Minimum) / ((double)Maximum - (double)Minimum));
                rec.Width = (int)((rec.Width * scaleFactor) - 2); rec.Height -= 2;
                e.Graphics.FillRectangle(new SolidBrush(this.BackColor), new Rectangle(0, 0, this.Width - 1, this.Height - 1));
                e.Graphics.FillRectangle(new SolidBrush(this.ForeColor), 1, 1, rec.Width, rec.Height);
                e.Graphics.DrawRectangle(new Pen(this.BorderColor), new Rectangle(0, 0, this.Width - 1, this.Height - 1));
            }
            else base.OnPaint(e);
        }
    }

    public class GroupBoxEx : GroupBox
    {
        public Boolean UseTheme { get; set; } = true;
        public Color BorderColor { get; set; } = SystemColors.ControlDark;

        protected override void OnPaint(PaintEventArgs e)
        {
            if (this.UseTheme)
            {
                Size tSize = TextRenderer.MeasureText(this.Text, this.Font);
                Rectangle borderRect = this.ClientRectangle;
                borderRect.Y = (borderRect.Y + (tSize.Height / 2));
                borderRect.Height = (borderRect.Height - (tSize.Height / 2));
                ControlPaint.DrawBorder(e.Graphics, borderRect, this.BorderColor, ButtonBorderStyle.Solid);
                Rectangle textRect = this.ClientRectangle;
                textRect.X = (textRect.X + 6);
                textRect.Width = tSize.Width;
                textRect.Height = tSize.Height;
                e.Graphics.FillRectangle(new SolidBrush(this.BackColor), textRect);
                TextRenderer.DrawText(e.Graphics, this.Text, this.Font, textRect, this.ForeColor);
            }
            else base.OnPaint(e);
        }
    }

    public class PropertyGridEx : PropertyGrid
    {
        public Boolean UseTheme { get; set; } = true;

        public PropertyGridEx()
        {
            this.SelectedObjectsChanged += this.OnSelectedObjectsChanged;
        }

        public ScrollBar GetScrollBar()
        {
            foreach (Control ctrl in this.Controls)
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

        private void OnValueChanged(Object sender, EventArgs e)
        {
            this.OnPaint(new PaintEventArgs(Graphics.FromHwnd(this.Handle), this.Bounds));
        }

        private void OnSelectedObjectsChanged(Object sender, EventArgs e)
        {
            ScrollBar scrollBar = GetScrollBar();
            if (scrollBar != null) scrollBar.ValueChanged += this.OnValueChanged;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (this.DesignMode) return;
            if (this.UseTheme)
            {
                Color color = PluginBase.MainForm.GetThemeColor("PropertyGrid.BackColor", SystemColors.Control);
                e.Graphics.FillRectangle(new SolidBrush(color), this.ClientRectangle);
            }
        }
    }

    public class PictureBoxEx : PictureBox
    {
        public Boolean UseTheme { get; set; } = true;
        public Color BorderColor { get; set; } = SystemColors.ControlDark;
        private Boolean themeBorder = false;

        protected override void WndProc(ref Message message)
        {
            base.WndProc(ref message);
            switch (message.Msg)
            {
                case Win32.WM_PAINT:
                    if (this.UseTheme && this.BorderStyle == BorderStyle.FixedSingle)
                    {
                        this.themeBorder = true;
                        this.BorderStyle = BorderStyle.None;
                    }
                    if (this.UseTheme && this.themeBorder)
                    {
                        BorderPanel.Attach(this);
                    }
                    break;
            }
        }
    }

    public class ButtonEx : Button
    {
        public Boolean UseTheme { get; set; } = true;
        public Color DisabledTextColor { get; set; } = SystemColors.ControlDark;
        public Color DisabledBackColor { get; set; } = SystemColors.Control;

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (this.UseTheme && !this.Enabled)
            {
                e.Graphics.FillRectangle(new SolidBrush(this.DisabledBackColor), Rectangle.Inflate(this.ClientRectangle, -2, -2));
                TextRenderer.DrawText(e.Graphics, this.Text, this.Font, this.ClientRectangle, this.DisabledTextColor);
                if (this.Image != null)
                {
                    var x = (this.Width / 2) - (this.Image.Width / 2);
                    var y = (this.Height / 2) - (this.Image.Height / 2);
                    ControlPaint.DrawImageDisabled(e.Graphics, this.Image, x, y, Color.Transparent);
                }
            }
        }
    }

    public class CheckBoxEx : CheckBox
    {
        public Boolean UseTheme { get; set; } = true;
        public Color DisabledTextColor { get; set; } = SystemColors.ControlDark;
        public Color BorderColor { get; set; } = SystemColors.ControlDark;

        protected override void OnPaint(PaintEventArgs e)
        {
            if (!this.DesignMode && this.UseTheme && PluginBase.MainForm.GetThemeColor("CheckBox.BackColor") != Color.Empty)
            {
                Size size = SystemInformation.MenuCheckSize;
                var offset = (this.ClientRectangle.Height - 1) - size.Height;
                var flags = TextFormatFlags.Left | TextFormatFlags.VerticalCenter;
                var checkRect = new Rectangle(this.ClientRectangle.X, this.ClientRectangle.Y + offset, size.Width - 2, size.Height - 2);
                var innerRect = new Rectangle(this.ClientRectangle.X + 3, this.ClientRectangle.Y + offset + 3, size.Width - 7, size.Height - 7);
                var textRect = new Rectangle(this.ClientRectangle.Location, this.ClientRectangle.Size);
                if (this.RightToLeft == RightToLeft.Yes)
                {
                    offset = this.ClientRectangle.Width - size.Width;
                    flags = TextFormatFlags.Right | TextFormatFlags.VerticalCenter;
                    textRect.Offset(-size.Width, 0);
                    checkRect.Offset(offset, 0);
                }
                else textRect.Offset(checkRect.Width + 3, 0);
                Color back = this.FlatAppearance.CheckedBackColor;
                if (ClientRectangle.Contains(PointToClient(MousePosition)))
                {
                    if (MouseButtons == MouseButtons.Left) back = this.FlatAppearance.MouseDownBackColor;
                    else back = this.FlatAppearance.MouseOverBackColor;
                }
                ButtonRenderer.DrawParentBackground(e.Graphics, this.ClientRectangle, this);
                e.Graphics.FillRectangle(new SolidBrush(this.BackColor), this.ClientRectangle);
                e.Graphics.FillRectangle(new SolidBrush(back), checkRect);
                e.Graphics.DrawRectangle(new Pen(this.BorderColor), checkRect);
                if (this.CheckState == CheckState.Indeterminate)
                {
                    e.Graphics.FillRectangle(new SolidBrush(this.BackColor), innerRect);
                }
                else if (this.CheckState == CheckState.Checked)
                {
                    Image image = PluginBase.MainForm.FindImageAndSetAdjust("485");
                    e.Graphics.DrawImage(image, checkRect, new Rectangle(Point.Empty, image.Size), GraphicsUnit.Pixel);
                }
                TextRenderer.DrawText(e.Graphics, this.Text, this.Font, textRect, this.ForeColor, flags);
            }
            else base.OnPaint(e);
        }
    }

    public class FormEx : Form
    {
        public virtual Boolean UseTheme { get; set; } = true;

        public FormEx()
        {
            this.ResizeRedraw = true;
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (!this.UseTheme)
            {
                base.OnPaintBackground(e);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (this.UseTheme)
            {
                Color color = PluginBase.MainForm.GetThemeColor("Form.BackColor", SystemColors.Control);
                e.Graphics.FillRectangle(new SolidBrush(color), this.ClientRectangle);
                if (this.WindowState != FormWindowState.Maximized && (this.FormBorderStyle == FormBorderStyle.Sizable || this.FormBorderStyle == FormBorderStyle.SizableToolWindow))
                {
                    this.SizeGripStyle = SizeGripStyle.Hide;
                    Color dark = PluginBase.MainForm.GetThemeColor("Form.3dDarkColor", SystemColors.ControlDark);
                    Color light = PluginBase.MainForm.GetThemeColor("Form.3dLightColor", SystemColors.ControlLight);
                    using (SolidBrush darkBrush = new SolidBrush(dark), lightBrush = new SolidBrush(light))
                    {
                        Int32 y = this.ClientRectangle.Bottom - 3 * 2 + 1;
                        for (Int32 i = 3; i >= 1; i--)
                        {
                            Int32 x = (this.ClientRectangle.Right - 3 * 2 + 1);
                            for (Int32 j = 0; j < i; j++)
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
    }

    public class TabControlEx : CustomTabControl, IThemeHandler
    {
        public Boolean UseTheme
        {
            get { return PluginBase.MainForm.GetThemeFlag("TabControl.UseTheme", false); }
        }

        public TabControlEx()
        {
            if (this.UseTheme) this.DisplayStyle = TabStyle.Flat;
        }

        private void MatchBackColor(Control parent)
        {
            foreach (Control ctrl in parent.Controls)
            {
                if (ctrl is Label || ctrl is CheckBox || ctrl is GroupBox)
                {
                    ctrl.BackColor = Color.Empty;
                }
                this.MatchBackColor(ctrl);
            }
        }

        public void AfterTheming()
        {
            this.MatchBackColor(this);
        }
    }

    public class StatusBarEx : StatusBar
    {
        public Boolean UseTheme { get; set; } = true;

        protected override void WndProc(ref Message message)
        {
            base.WndProc(ref message);
            if (this.DesignMode) return;
            switch (message.Msg)
            {
                case Win32.WM_PAINT:
                    this.OnPaint(new PaintEventArgs(Graphics.FromHwnd(this.Handle), this.Bounds));
                    if (this.UseTheme)
                    {
                        Graphics g = this.CreateGraphics();
                        Color back = PluginBase.MainForm.GetThemeColor("StatusBar.BackColor", SystemColors.Control);
                        Color fore = PluginBase.MainForm.GetThemeColor("StatusBar.ForeColor", SystemColors.ControlText);
                        g.FillRectangle(new SolidBrush(back), this.ClientRectangle);
                        g.DrawLine(SystemPens.ControlDark, new Point(this.ClientRectangle.X, this.ClientRectangle.Y + 1), new Point(this.ClientRectangle.Width, this.ClientRectangle.Y + 1));
                        if (this.SizingGrip)
                        {
                            Color dark = PluginBase.MainForm.GetThemeColor("StatusBar.3dDarkColor", SystemColors.ControlDark);
                            Color light = PluginBase.MainForm.GetThemeColor("StatusBar.3dLightColor", SystemColors.ControlLight);
                            using (SolidBrush darkBrush = new SolidBrush(dark), lightBrush = new SolidBrush(light))
                            {
                                Int32 y = this.ClientRectangle.Bottom - 3 * 2 + 1;
                                for (Int32 i = 3; i >= 1; i--)
                                {
                                    Int32 x = (this.ClientRectangle.Right - 3 * 2 + 1);
                                    for (Int32 j = 0; j < i; j++)
                                    {
                                        g.FillRectangle(lightBrush, x + 1, y + 1, 2, 2);
                                        g.FillRectangle(darkBrush, x, y, 2, 2);
                                        x -= 4;
                                    }
                                    y -= 4;
                                }
                            }
                        }
                        var tff = TextFormatFlags.VerticalCenter;
                        TextRenderer.DrawText(g, this.Text, this.Font, new Point(0, (this.Height / 2) + 2), fore, tff);
                    }
                    break;
            }
        }
    }

    public class ToolStripProgressBarEx : ToolStripProgressBar
    {
        private static readonly Padding defaultMargin = new Padding(1, 2, 1, 1);
        private static readonly Padding defaultStatusStripMargin = new Padding(1, 5, 1, 4);

        public ToolStripProgressBarEx() : base()
        {
            this.OverrideControl();
            this.Font = PluginBase.Settings.DefaultFont;
            this.ProgressBar.ForeColor = PluginBase.MainForm.GetThemeColor("ToolStripProgressBar.ForeColor", SystemColors.Highlight);
            this.ProgressBar.Margin = DefaultMargin;
            this.ProgressBar.Size = DefaultSize;
        }

        private void OverrideControl()
        {
            this.OnUnsubscribeControlEvents(this.Control);
            Type type = this.GetType();
            FieldInfo prop = type.BaseType.BaseType.GetField("control", BindingFlags.NonPublic | BindingFlags.Instance);
            prop.SetValue(this, new ProgressBarEx());
            this.OnSubscribeControlEvents(this.Control);
            this.Invalidate();
        }

        protected override Size DefaultSize
        {
            get { return new Size(100, 12); }
        }

        protected override Padding DefaultMargin
        {
            get
            {
                if (this.Owner != null && this.Owner is StatusStrip)
                {
                    return defaultStatusStripMargin;
                }
                else return defaultMargin;
            }
        }
    }

    public class ToolStripSpringComboBox : ToolStripComboBoxEx
    {
        public ToolStripSpringComboBox()
        {
            this.Control.PreviewKeyDown += new PreviewKeyDownEventHandler(this.OnPreviewKeyDown);
        }

        /// <summary>
        /// Fixes the Control+Alt (AltGr) key combination handling
        /// </summary>
        private void OnPreviewKeyDown(Object sender, PreviewKeyDownEventArgs e)
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
            Int32 width = Owner.DisplayRectangle.Width;
            // Subtract the width of the overflow button if it is displayed. 
            if (Owner.OverflowButton.Visible)
            {
                width = width - Owner.OverflowButton.Width - Owner.OverflowButton.Margin.Horizontal;
            }
            // Declare a variable to maintain a count of ToolStripSpringComboBox 
            // items currently displayed in the owning ToolStrip. 
            Int32 springBoxCount = 0;
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
            this.Control.PreviewKeyDown += new PreviewKeyDownEventHandler(this.OnPreviewKeyDown);
        }

        /// <summary>
        /// Fixes the Control+Alt (AltGr) key combination handling
        /// </summary>
        private void OnPreviewKeyDown(Object sender, PreviewKeyDownEventArgs e)
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
            Int32 width = Owner.DisplayRectangle.Width;
            // Subtract the width of the overflow button if it is displayed. 
            if (Owner.OverflowButton.Visible)
            {
                width = width - Owner.OverflowButton.Width - Owner.OverflowButton.Margin.Horizontal;
            }
            // Declare a variable to maintain a count of ToolStripSpringTextBox 
            // items currently displayed in the owning ToolStrip. 
            Int32 springBoxCount = 0;
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

        private static void ShowDescription(Control control)
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
