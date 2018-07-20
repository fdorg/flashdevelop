using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using PluginCore.BBCode;
using PluginCore.Managers;
using ScintillaNet;
using PluginCore.Helpers;

namespace PluginCore.Controls
{
    /// <summary>
    /// RichTextBox-based tooltip
    /// </summary>
    public class RichToolTip : IEventHandler
    {
        public delegate void UpdateTipHandler(ScintillaControl sender, Point mousePosition);

        // events
        public event UpdateTipHandler OnUpdateSimpleTip;

        // controls
        protected Panel toolTip;
        protected RichTextBox toolTipRTB;
        protected string rawText;
        protected string lastRawText;
        protected string cachedRtf;
        protected Dictionary<String, String> rtfCache;
        protected List<String> rtfCacheList;
        protected Point mousePos;

        #region Public Properties
        
        public bool Visible 
        {
            get { return toolTip.Visible; }
        }

        public Size Size
        {
            get { return toolTip.Size; }
            set { toolTip.Size = value; }
        }

        public Point Location
        {
            get { return toolTip.Location;  }
            set { toolTip.Location = value; }
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
        
        public RichToolTip(IMainForm mainForm)
        {
            EventManager.AddEventHandler(this, EventType.ApplyTheme);
            // panel
            toolTip = new Panel();
            toolTip.Location = new Point(0,0);
            toolTip.BackColor = SystemColors.Info;
            toolTip.ForeColor = SystemColors.InfoText;
            toolTip.BorderStyle = BorderStyle.FixedSingle;
            toolTip.Visible = false;
            (mainForm as Form).Controls.Add(toolTip);
            // text
            toolTipRTB = new RichTextBox();
            toolTipRTB.Font = PluginBase.Settings.DefaultFont;
            toolTipRTB.BackColor = SystemColors.Info;
            toolTipRTB.ForeColor = SystemColors.InfoText;
            toolTipRTB.BorderStyle = BorderStyle.None;
            toolTipRTB.ScrollBars = RichTextBoxScrollBars.None;
            toolTipRTB.DetectUrls = false;
            toolTipRTB.ReadOnly = true;
            toolTipRTB.WordWrap = false;
            toolTipRTB.Visible = true;
            toolTipRTB.Text = "";
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

            int smallOffsetH = ScaleHelper.Scale(1);
            int smallOffsetW = ScaleHelper.Scale(2);
            int smallPadding = ScaleHelper.Scale(4);
            int mediumPadding = ScaleHelper.Scale(10);
            int minWidth = ScaleHelper.Scale(200);
            maxWidth = ScaleHelper.Scale(maxWidth);

            // tooltip larger than the window: wrap
            int limitLeft = ((Form)PluginBase.MainForm).ClientRectangle.Left + mediumPadding;
            int limitRight = ((Form)PluginBase.MainForm).ClientRectangle.Right - mediumPadding;
            int limitBottom = ((Form)PluginBase.MainForm).ClientRectangle.Bottom - ScaleHelper.Scale(26);
            
            int maxW = availableWidth > 0 ? availableWidth : limitRight - limitLeft;
            if (maxW > maxWidth && maxWidth > 0)
                maxW = maxWidth;

            int w = txtSize.Width + smallPadding;
            if (w > maxW)
            {
                wordWrap = true;
                w = maxW;
                if (w < minWidth)
                {
                    w = minWidth;
                    tooSmall = true;
                }

                txtSize = WinFormUtils.MeasureRichTextBox(toolTipRTB, false, w, maxWidth, true);
                w = txtSize.Width + smallPadding;
            }

            int h = txtSize.Height + smallOffsetH * 2;
            int dh = smallOffsetH;
            int dw = smallOffsetW;
            if (h > (limitBottom - toolTip.Top))
            {
                w += ScaleHelper.Scale(15);
                h = limitBottom - toolTip.Top;
                dh = smallPadding;
                dw = smallPadding + smallOffsetW / 2;

                toolTipRTB.ScrollBars = RichTextBoxScrollBars.Vertical;
            }

            toolTipRTB.Location = new Point(smallOffsetW, smallOffsetH);
            toolTipRTB.Size = new Size(w, h);
            toolTip.Size = new Size(w + dw, h + dh);

            if (toolTip.Left < limitLeft)
                toolTip.Left = limitLeft;

            if (toolTip.Left + toolTip.Width > limitRight)
                toolTip.Left = limitRight - toolTip.Width;

            if (toolTipRTB.WordWrap != wordWrap)
                toolTipRTB.WordWrap = wordWrap;

            return !tooSmall;
        }

        public void ShowAtMouseLocation(string text)
        {
            if (text != Text)
            {
                toolTip.Visible = false;
                Text = text;
            }
            ShowAtMouseLocation();
        }
        
        public void ShowAtMouseLocation()
        {
            mousePos = ((Form)PluginBase.MainForm).PointToClient(Control.MousePosition);
            toolTip.Left = mousePos.X;
            if (toolTip.Right > ((Form)PluginBase.MainForm).ClientRectangle.Right)
            {
                toolTip.Left -= (toolTip.Right - ((Form)PluginBase.MainForm).ClientRectangle.Right);
            }
            toolTip.Top = mousePos.Y - toolTip.Height - ScaleHelper.Scale(10);
            toolTip.Show();
            toolTip.BringToFront();
        }

        public void UpdateTip(ScintillaControl sci)
        {
            if (OnUpdateSimpleTip != null) OnUpdateSimpleTip(sci, mousePos);
        }
        
        public virtual void Hide()
        {
            if (toolTip.Visible)
            {
                toolTip.Visible = false;
                toolTipRTB.ResetText();
            }
        }

        public virtual void Show()
        {
            toolTip.Visible = true;
            toolTip.BringToFront();
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
            if (rtfCache.ContainsKey(bbcodeText))
                return rtfCache[bbcodeText];

            if (rtfCacheList.Count >= 512)
            {
                String key = rtfCacheList[0];
                rtfCache[key] = null;
                rtfCache.Remove(key);
                rtfCacheList[0] = null;
                rtfCacheList.RemoveAt(0);
            }

            toolTipRTB.Text = "";
            toolTipRTB.ScrollBars = RichTextBoxScrollBars.None;
            toolTipRTB.WordWrap = false;

            rtfCacheList.Add(bbcodeText);
            rtfCache[bbcodeText] = BBCodeUtils.bbCodeToRtf(bbcodeText, toolTipRTB);
            return rtfCache[bbcodeText];
        }

        #endregion

    }

}
