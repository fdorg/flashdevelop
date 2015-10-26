using System;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Reflection;
using PluginCore.Helpers;
using PluginCore.Managers;
using PluginCore;

namespace System.Windows.Forms
{
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
        /// Makes the control spring automaticly
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
        /// Makes the control spring automaticly
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

    public class DataGridViewEx : DataGridView, IEventHandler
    {
        public DataGridViewEx()
        {
            this.CellPainting += this.OnDataGridViewCellPainting;
            EventManager.AddEventHandler(this, EventType.ApplyTheme);
        }

        protected override void Dispose(bool disposing)
        {
            EventManager.RemoveEventHandler(this);
            base.Dispose(disposing);
        }

        public void HandleEvent(object sender, NotifyEvent e, HandlingPriority priority)
        {
            if (e.Type == EventType.ApplyTheme)
            {
                RefreshColors();
            }
        }

        private void RefreshColors()
        {
            Color fore = PluginBase.MainForm.GetThemeColor("DataGridView.ForeColor");
            Color back = PluginBase.MainForm.GetThemeColor("DataGridView.BackColor");
            Color border = PluginBase.MainForm.GetThemeColor("DataGridView.LineColor");
            DefaultCellStyle.ForeColor = (fore != Color.Empty) ? fore : SystemColors.WindowText;
            DefaultCellStyle.BackColor = (back != Color.Empty) ? back : SystemColors.Window;
            GridColor = (border != Color.Empty) ? border :  SystemColors.ControlDark;
        }

        private void OnDataGridViewCellPainting(Object sender, DataGridViewCellPaintingEventArgs e)
        {
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

    }

    public class ListViewEx : ListView
    {
        private Timer expandDelay;
        public Boolean UseTheme = true;

        public ListViewEx()
        {
            this.OwnerDraw = true;
            this.DrawColumnHeader += this.OnDrawColumnHeader;
            this.DrawSubItem += this.OnDrawSubItem;
            this.DrawItem += this.OnDrawItem;
            this.expandDelay = new Timer();
            this.expandDelay.Interval = 50;
            this.expandDelay.Tick += this.ExpandDelayTick;
            this.expandDelay.Enabled = true;
            this.expandDelay.Start();
        }

        private void OnDrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            e.DrawDefault = true;
        }

        private void OnDrawItem(object sender, DrawListViewItemEventArgs e)
        {
            e.DrawDefault = true;
        }

        private void OnDrawColumnHeader(Object sender, DrawListViewColumnHeaderEventArgs e)
        {
            Color back = PluginBase.MainForm.GetThemeColor("ColumnHeader.BackColor");
            Color text = PluginBase.MainForm.GetThemeColor("ColumnHeader.TextColor");
            Color border = PluginBase.MainForm.GetThemeColor("ColumnHeader.BorderColor");
            if (UseTheme && back != Color.Empty && border != Color.Empty && text != Color.Empty)
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
        }

        protected override void WndProc(ref Message message)
        {
            switch (message.Msg)
            {
                case 0xf: // WM_PAINT
                    // Delay column expand...
                    this.expandDelay.Enabled = true;
                    break;
            }
            base.WndProc(ref message);
        }

    }

    public class TreeViewEx : TreeView
    {
        private static Int32 SIZE1 = ScaleHelper.Scale(1);
        private static Int32 SIZE2 = ScaleHelper.Scale(2);
        private static Int32 SIZE3 = ScaleHelper.Scale(3);

        public TreeViewEx() : base()
        {
            this.DrawMode = TreeViewDrawMode.OwnerDrawAll;
            this.DrawNode += OnDrawNode;
            this.DragOver += OnDragOver;
            this.ShowPlusMinus = true;
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000; // WS_CLIPCHILDREN
                return cp;
            }
        }

        private void OnDragOver(Object sender, DragEventArgs e)
        {
            Point point = this.PointToClient(new Point(e.X, e.Y));
            TreeViewHitTestInfo hit = this.HitTest(point);
            if (hit.Node != null) this.SelectedNode = hit.Node;
        }

        private void OnDrawNode(Object sender, DrawTreeNodeEventArgs e)
        {
            e.DrawDefault = false;
            Rectangle bounds = e.Node.Bounds;
            Color backHl = PluginBase.MainForm.GetThemeColor("TreeView.Highlight", SystemColors.Highlight);
            Color foreHl = PluginBase.MainForm.GetThemeColor("TreeView.HighlightText", SystemColors.HighlightText);
            SolidBrush brushFore = new SolidBrush(e.Node.TreeView.LineColor);
            SolidBrush brushBack = new SolidBrush(e.Node.TreeView.BackColor);
            if (bounds.IsEmpty || !e.Node.IsVisible) return;
            if ((e.State & TreeNodeStates.Focused) != 0)
            {
                e.Graphics.FillRectangle(new SolidBrush(backHl), Rectangle.Inflate(bounds, 1, 0));
            }
            else if ((e.State & TreeNodeStates.Selected) != 0)
            {
                e.Graphics.FillRectangle(new SolidBrush(Color.Gray), Rectangle.Inflate(bounds, 1, 0));
            }
            else e.Graphics.FillRectangle(brushBack, bounds);
            if (e.Node.Nodes.Count > 0)
            {
                Point[] arrow;
                Point middle = new Point(bounds.Left - 28, bounds.Top + bounds.Height / 2);
                if (e.Node.IsExpanded)
                {
                    arrow = new Point[]
                    {
                        new Point(middle.X - SIZE3, middle.Y - 1),
                        new Point(middle.X + SIZE3 + 1, middle.Y - 1),
                        new Point(middle.X, middle.Y + SIZE3)
                    };
                    e.Graphics.FillPolygon(brushFore, arrow);
                    arrow = new Point[]
                    {
                        new Point(middle.X - SIZE1, middle.Y - 1),
                        new Point(middle.X + SIZE1 + 1, middle.Y - 1),
                        new Point(middle.X, middle.Y + SIZE1)
                    };
                    e.Graphics.FillPolygon(brushBack, arrow);
                }
                else
                {
                    arrow = new Point[]
                    {
                        new Point(middle.X - SIZE2, middle.Y - 2 * SIZE2),
                        new Point(middle.X - SIZE2, middle.Y + 2 * SIZE2),
                        new Point(middle.X + SIZE2, middle.Y)
                    };
                    e.Graphics.FillPolygon(brushFore, arrow);
                    arrow = new Point[]
                    {
                        new Point(middle.X - SIZE1 - 1, middle.Y - 2 * SIZE1),
                        new Point(middle.X - SIZE1 - 1, middle.Y + 2 * SIZE1),
                        new Point(middle.X + SIZE1 - 1, middle.Y)
                    };
                    e.Graphics.FillPolygon(brushBack, arrow);
                }
            }
            if (e.Node.ImageIndex != -1)
            {
                Point nodePt = new Point(bounds.Location.X - 20, bounds.Location.Y + 1);
                Image nodeImg = e.Node.TreeView.ImageList.Images[e.Node.ImageIndex];
                e.Graphics.DrawImage(nodeImg, nodePt);
            }
            Rectangle textRect = bounds;
            Font nodeFont = e.Node.NodeFont;
            Color textColor = e.Node.ForeColor;
            textRect = Rectangle.Inflate(textRect, 2, 0);
            if (nodeFont == null) nodeFont = ((TreeView)sender).Font;
            if (nodeFont.Bold) textRect.X += 1;
            if ((e.State & TreeNodeStates.Focused) != 0) textColor = foreHl;
            if ((e.State & TreeNodeStates.Selected) != 0) textColor = foreHl;
            if ((e.State & TreeNodeStates.Hot) != 0) nodeFont = new Font(nodeFont, FontStyle.Underline);
            TextRenderer.DrawText(e.Graphics, e.Node.Text, nodeFont, textRect, textColor);
        }

    }

    public class ToolStripComboBoxEx : ToolStripControlHost
    {
        public ToolStripComboBoxEx() : base(new FlatCombo())
        {
            Font font = PluginBase.Settings.DefaultFont;
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
            set { this.FlatCombo.FlatStyle = value; }
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

    public class FlatCombo : ComboBox, IEventHandler
    {
        private Boolean useTheme = true;
        private Pen BorderPen = new Pen(SystemColors.ControlDark);
        private SolidBrush BackBrush = new SolidBrush(SystemColors.Window);
        private SolidBrush ArrowBrush = new SolidBrush(SystemColors.ControlText);
        private ComboBoxStyle prevStyle = ComboBoxStyle.DropDown;
        private Boolean updatingStyle = false;
        
        public FlatCombo()
        {
            this.UseTheme = true;
            EventManager.AddEventHandler(this, EventType.ApplyTheme);
        }

        protected override void Dispose(bool disposing)
        {
            EventManager.RemoveEventHandler(this);
            base.Dispose(disposing);
        }

        public void HandleEvent(Object sender, NotifyEvent e, HandlingPriority priority)
        {
            if (e.Type == EventType.ApplyTheme)
            {
                this.FlatStyle = PluginBase.Settings.ComboBoxFlatStyle;
                this.UseTheme = (this.FlatStyle == FlatStyle.Popup);
            }
        }

        public Boolean UseTheme
        {
            get { return this.useTheme; }
            set
            {
                this.useTheme = value;
                this.RefreshColors();
            }
        }

        private void RefreshColors()
        {
            Color fore = PluginBase.MainForm.GetThemeColor("ToolStripComboBoxControl.ForeColor");
            Color back = PluginBase.MainForm.GetThemeColor("ToolStripComboBoxControl.BackColor");
            Color border = PluginBase.MainForm.GetThemeColor("ToolStripComboBoxControl.BorderColor");
            if (fore == Color.Empty && back == Color.Empty && border == Color.Empty) this.useTheme = false;
            this.BorderPen.Color = useTheme && border != Color.Empty ? border : SystemColors.ControlDark;
            this.ArrowBrush.Color = useTheme && fore != Color.Empty ? fore : SystemColors.ControlText;
            this.BackBrush.Color = useTheme && back != Color.Empty ? back : SystemColors.Window;
            this.ForeColor = useTheme && fore != Color.Empty ? fore : SystemColors.ControlText;
            this.BackColor = useTheme && back != Color.Empty ? back : SystemColors.Window;
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (!this.useTheme) return;
            switch (m.Msg)
            {
                case 0x85: // WM_NCPAINT
                case 0x14: // WM_ERASEBKGND 
                case 0xf: // WM_PAINT
                    Int32 pad = ScaleHelper.Scale(2);
                    Int32 width = ScaleHelper.Scale(18);
                    Graphics g = this.CreateGraphics();
                    Rectangle backRect = new Rectangle(this.ClientRectangle.X, this.ClientRectangle.Y, this.ClientRectangle.Width - 1, this.ClientRectangle.Height - 1);
                    Rectangle dropRect = new Rectangle(this.ClientRectangle.Right - width, this.ClientRectangle.Y, width, this.ClientRectangle.Height);
                    if (this.Enabled) g.FillRectangle(BackBrush, dropRect);
                    g.DrawRectangle(BorderPen, backRect);
                    Point middle = new Point(dropRect.Left + (dropRect.Width / 2), dropRect.Top + (dropRect.Height / 2));
                    Point[] arrow = new Point[] 
                    {
                        new Point(middle.X - pad, middle.Y - 1),
                        new Point(middle.X + pad + 1, middle.Y - 1),
                        new Point(middle.X, middle.Y + pad)
                    };
                    if (this.Enabled) g.FillPolygon(ArrowBrush, arrow);
                    else g.FillPolygon(SystemBrushes.ControlDark, arrow);
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
            Color border = PluginBase.MainForm.GetThemeColor("ToolStripComboBoxControl.ActiveBorderColor");
            BorderPen.Color = border != Color.Empty ? border : SystemColors.Highlight;
            this.Invalidate();
        }

        protected override void OnMouseLeave(System.EventArgs e)
        {
            base.OnMouseLeave(e);
            if (this.Focused) return;
            Color border = PluginBase.MainForm.GetThemeColor("ToolStripComboBoxControl.BorderColor");
            BorderPen.Color = border != Color.Empty ? border : SystemColors.ControlDark;
            this.Invalidate();
        }

        protected override void OnLostFocus(System.EventArgs e)
        {
            base.OnLostFocus(e);
            Color border = PluginBase.MainForm.GetThemeColor("ToolStripComboBoxControl.BorderColor");
            BorderPen.Color = border != Color.Empty ? border : SystemColors.ControlDark;
            this.Invalidate();
        }

        protected override void OnGotFocus(System.EventArgs e)
        {
            base.OnGotFocus(e);
            Color border = PluginBase.MainForm.GetThemeColor("ToolStripComboBoxControl.ActiveBorderColor");
            BorderPen.Color = border != Color.Empty ? border : SystemColors.Highlight;
            this.Invalidate();
        }

        protected override void OnMouseHover(System.EventArgs e)
        {
            base.OnMouseHover(e);
            Color border = PluginBase.MainForm.GetThemeColor("ToolStripComboBoxControl.ActiveBorderColor");
            BorderPen.Color = border != Color.Empty ? border : SystemColors.Highlight;
            this.Invalidate();
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
