using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using PluginCore.BBCode;
using PluginCore.Managers;

namespace PluginCore.Controls
{
    /// <summary>
    /// RichTextBox-based tooltip
    /// </summary>
    public class RichToolTip : IEventHandler
    {
        public delegate void UpdateTipHandler(Control sender, Point mousePosition);

        // events
        public event UpdateTipHandler OnUpdateSimpleTip;
        public event CancelEventHandler OnShowing;
        public event EventHandler OnHidden;

        // controls
        protected InactiveForm host;
        protected Panel toolTip;
        protected SelectableRichTextBox toolTipRTB;
        protected string rawText;
        protected Dictionary<String, String> rtfCache;
        protected List<String> rtfCacheList;
        protected Point mousePos;

        protected ICompletionListHost owner;    // We could just use Control here, or pass a reference on each related call, as Control may be a problem with default implementation

        #region Public Properties

        public bool Focused
        {
            get { return toolTipRTB.Focused; }
        }

        public bool Visible
        {
            get { return host.Visible; }
        }

        public Size Size
        {
            get { return host.Size; }
            set { host.Size = value; }
        }

        public Point Location
        {
            get { return host.Location; }
            set { host.Location = value; }
        }

        public string RawText
        {
            get { return rawText; }
            set
            {
                SetText(value, true);
            }
        }

        public bool Selectable
        {
            get { return toolTipRTB.Selectable; }
            set
            {
                toolTipRTB.Selectable = value;
            }
        }

        public string Text
        {
            get { return toolTipRTB.Text; }
            set
            {
                SetText(value, true);
            }
        }

        #endregion

        #region Control creation

        public RichToolTip(ICompletionListHost owner)
        {
            EventManager.AddEventHandler(this, EventType.ApplyTheme);

            // host
            host = new InactiveForm();
            host.FormBorderStyle = FormBorderStyle.None;
            host.ShowInTaskbar = false;
            host.TopMost = true;
            host.StartPosition = FormStartPosition.Manual;
            host.KeyPreview = true;
            host.KeyDown += Host_KeyDown;

            this.owner = owner;

            // panel
            toolTip = new Panel();
            toolTip.Location = new Point(0, 0);
            toolTip.BackColor = SystemColors.Info;
            toolTip.ForeColor = SystemColors.InfoText;
            toolTip.BorderStyle = BorderStyle.FixedSingle;
            toolTip.Dock = DockStyle.Fill;
            host.Controls.Add(toolTip);
            // text
            toolTipRTB = new SelectableRichTextBox();
            toolTipRTB.Location = new Point(2, 1);
            toolTipRTB.BackColor = SystemColors.Info;
            toolTipRTB.ForeColor = SystemColors.InfoText;
            toolTipRTB.BorderStyle = BorderStyle.None;
            toolTipRTB.ScrollBars = RichTextBoxScrollBars.None;
            toolTipRTB.DetectUrls = false;
            toolTipRTB.ReadOnly = true;
            toolTipRTB.WordWrap = false;
            toolTipRTB.Visible = true;
            toolTipRTB.Text = "";
            toolTipRTB.LostFocus += Host_LostFocus;
            toolTip.Controls.Add(toolTipRTB);

            // rtf cache
            rtfCache = new Dictionary<String, String>();
            rtfCacheList = new List<String>();
        }
        
        public void HandleEvent(Object sender, NotifyEvent e, HandlingPriority priority)
        {
            if (e.Type == EventType.ApplyTheme)
            {
                Color fore = PluginBase.MainForm.GetThemeColor("RichToolTip.ForeColor");
                Color back = PluginBase.MainForm.GetThemeColor("RichToolTip.BackColor");
                toolTip.BackColor = back == Color.Empty ? SystemColors.Info : back;
                toolTip.ForeColor = fore == Color.Empty ? SystemColors.InfoText : fore;
                toolTipRTB.ForeColor = fore == Color.Empty ? SystemColors.InfoText : fore;
                toolTipRTB.BackColor = back == Color.Empty ? SystemColors.Info : back;
            }
        }

        #endregion

        #region Event Handlers

        protected virtual void Host_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Escape)
                Hide();
        }

        protected virtual void Host_LostFocus(object sender, EventArgs e)
        {
            if (!owner.Owner.ContainsFocus)
                Hide();
        }

        #endregion

        #region Tip Methods

        public bool AutoSize()
        {
            return AutoSize(0);
        }
        public bool AutoSize(int availableWidth)
        {
            return AutoSize(availableWidth, 1024);
        }
        public bool AutoSize(int availableWidth, int maxWidth)
        {
            bool tooSmall = false;
            bool wordWrap = false;
            Size txtSize = WinFormUtils.MeasureRichTextBox(toolTipRTB, false, toolTipRTB.Width, toolTipRTB.Height, false);

            // tooltip larger than the window: wrap
            var screenArea = Screen.FromControl(owner.Owner).WorkingArea;
            int limitLeft = screenArea.Left + 1;
            int limitRight = screenArea.Right - 1;
            int limitBottom = screenArea.Bottom - 26;
            //
            int maxW = availableWidth > 0 ? availableWidth : limitRight - limitLeft;
            if (maxW > maxWidth && maxWidth > 0)
                maxW = maxWidth;

            int w = txtSize.Width + 4;
            if (w > maxW)
            {
                wordWrap = true;
                w = maxW;
                if (w < 200)
                {
                    w = 200;
                    tooSmall = true;
                }

                txtSize = WinFormUtils.MeasureRichTextBox(toolTipRTB, false, w, 1000, true);
                w = txtSize.Width + 4;
            }

            int h = txtSize.Height + 2;
            int dh = 1;
            int dw = 2;
            if (h > (limitBottom - host.Top))
            {
                w += 15;
                h = limitBottom - host.Top;
                dh = 4;
                dw = 5;

                toolTipRTB.ScrollBars = RichTextBoxScrollBars.Vertical;
            }

            toolTipRTB.Size = new Size(w, h);
            host.Size = new Size(w + dw, h + dh);

            if (host.Left < limitLeft)
                host.Left = limitLeft;

            if (host.Left + host.Width > limitRight)
                host.Left = limitRight - host.Width;

            if (toolTipRTB.WordWrap != wordWrap)
                toolTipRTB.WordWrap = wordWrap;

            return !tooSmall;
        }

        public void ShowAtMouseLocation(string text)
        {
            if (text != Text)
            {
                host.Visible = false;
                Text = text;
            }
            ShowAtMouseLocation();
        }

        public void ShowAtMouseLocation()
        {
            //ITabbedDocument doc = PluginBase.MainForm.CurrentDocument;
            mousePos = Control.MousePosition;
            host.Left = mousePos.X;// +sci.Left;
            var screen = Screen.FromPoint(mousePos);
            if (host.Right > screen.WorkingArea.Right)
            {
                host.Left -= (host.Right - screen.WorkingArea.Right);
            }
            host.Top = mousePos.Y - host.Height - 10;// +sci.Top;
            if (host.Top < 5)
                host.Top = mousePos.Y + 10;
            Show();
        }

        public virtual void UpdateTip()
        {
            if (OnUpdateSimpleTip != null) OnUpdateSimpleTip(owner.Owner, mousePos);
        }

        public virtual void Hide()
        {
            if (host.Visible)
            {
                host.Visible = false;
                toolTipRTB.ResetText();
                if (OnHidden != null) OnHidden(this, EventArgs.Empty);
            }
        }

        public virtual void Show()
        {
            if (!host.Visible)
            {
                if (OnShowing != null)
                {
                    var cancelArgs = new CancelEventArgs();
                    OnShowing(this, cancelArgs);
                    if (cancelArgs.Cancel)
                    {
                        Hide();
                        return;
                    }
                }

                // Not really needed to set an owner, it has some advantages currently unused
                host.Owner = null;  // To avoid circular references that may happen because of Floating -> Docking panels
                host.Show(owner.Owner);
            }
        }

        public void SetText(String rawText, bool redraw)
        {
            this.rawText = rawText ?? "";
            if (redraw)
                Redraw();
        }

        public void Redraw()
        {
            Redraw(true);
        }
        public void Redraw(bool autoSize)
        {
            toolTipRTB.Rtf = getRtfFor(rawText);

            Color fore = PluginBase.MainForm.GetThemeColor("RichToolTip.ForeColor");
            Color back = PluginBase.MainForm.GetThemeColor("RichToolTip.BackColor");
            toolTip.BackColor = back == Color.Empty ? SystemColors.Info : back;
            toolTip.ForeColor = fore == Color.Empty ? SystemColors.InfoText : fore;
            toolTipRTB.ForeColor = fore == Color.Empty ? SystemColors.InfoText : fore;
            toolTipRTB.BackColor = back == Color.Empty ? SystemColors.Info : back;

            if (autoSize)
                AutoSize();
        }

        protected String getRtfFor(String bbcodeText)
        {
            String rtfText;

            if (rtfCache.TryGetValue(bbcodeText, out rtfText))
                return rtfText;

            if (rtfCacheList.Count >= 512)
            {
                String key = rtfCacheList[0];
                rtfCache.Remove(key);
                rtfCacheList.RemoveAt(0);
            }

            toolTipRTB.Text = "";
            toolTipRTB.ScrollBars = RichTextBoxScrollBars.None;
            toolTipRTB.WordWrap = false;

            rtfCacheList.Add(bbcodeText);
            rtfText = BBCodeUtils.bbCodeToRtf(bbcodeText, toolTipRTB);
            rtfCache[bbcodeText] = rtfText;
            return rtfText;
        }

        public bool IsMouseInside()
        {
            return host.Bounds.Contains(Control.MousePosition);
        }

        #endregion

        #region Selectable RichTextBox

        // If for some reason this is not compatible with CrossOver or we want some crossplatform alternative we could place a disabled Form with Opacity to 0.009 or something like that over the control's ClientRectangle
        // The downside is that on standard Windows configuration it will play an annoying "Bong" sound when clicking. Another option would be to hide the control and draw it on the form, the problem is the scrollbar,
        // but we could use the ones from Form or some Panel and draw the whole text instead of just the original visible area.
        protected class SelectableRichTextBox : RichTextBox
        {

            private bool _selectable = true;
            public bool Selectable
            {
                get { return _selectable; }
                set
                {
                    if (_selectable == value) return;
                    _selectable = value;
                    if (_lastCursor == null || _lastCursor == DefaultCursor)
                        base.Cursor = !_selectable ? Cursors.Default : DefaultCursor;
                }
            }

            private Cursor _lastCursor;
            public override Cursor Cursor
            {
                get
                {
                    return base.Cursor;
                }
                set
                {
                    _lastCursor = value;
                    base.Cursor = value;
                }
            }

            protected override void DefWndProc(ref Message m)
            {
                const int WM_MOUSEACTIVATE = 0x21;
                const int WM_CONTEXTMENU = 0x7b;
                const int WM_LBUTTONDOWN = 0x201;
                const int WM_LBUTTONDBLCLK = 0x203;
                const int MA_NOACTIVATE = 0x0003;

                if (!_selectable)
                {
                    switch (m.Msg)
                    {
                        case WM_MOUSEACTIVATE:
                            m.Result = (IntPtr)MA_NOACTIVATE;
                            return;
                        case WM_LBUTTONDOWN:
                        case WM_LBUTTONDBLCLK:
                        case WM_CONTEXTMENU:
                            m.Result = IntPtr.Zero;
                            return;
                    }
                }
                base.DefWndProc(ref m);
            }

        }

        #endregion

    }

}
