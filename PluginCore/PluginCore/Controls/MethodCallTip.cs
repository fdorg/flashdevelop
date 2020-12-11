// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Drawing;
using System.Windows.Forms;
using PluginCore.Utilities;
using ScintillaNet;

namespace PluginCore.Controls
{
    
    public class MethodCallTip: RichToolTip
    {
        public delegate void UpdateCallTipHandler(ScintillaControl sender, int position);

        // events
        public event UpdateCallTipHandler OnUpdateCallTip;


        public static string HLTextStyleBeg = "[B]";
        public static string HLTextStyleEnd = "[/B]";
        public static string HLBgStyleBeg = "[BGCOLOR=#000:OVERLAY]";
        public static string HLBgStyleEnd = "[/BGCOLOR]";

        // state
        protected string currentText;
        protected int currentHLStart;
        protected int currentHLEnd;
        protected bool isActive;
        protected int memberPos;
        protected int startPos;
        protected int currentPos;
        protected int deltaPos;
        protected int currentLine;

        public MethodCallTip(IMainForm mainForm): base(mainForm)
        {
            var color = PluginBase.MainForm.GetThemeColor("MethodCallTip.SelectedBack");
            if (color != Color.Empty) HLBgStyleBeg = "[BGCOLOR=" + DataConverter.ColorToHex(color).Replace("0x", "#") + "]";
            var fore = PluginBase.MainForm.GetThemeColor("MethodCallTip.SelectedFore");
            if (fore != Color.Empty)
            {
                HLTextStyleBeg = "[B][COLOR=" + DataConverter.ColorToHex(fore).Replace("0x", "#") + "]";
                HLTextStyleEnd = "[/COLOR][/B]";
            }
        }

        public bool CallTipActive => isActive;

        public bool Focused => toolTipRTB.Focused;

        public override void Hide()
        {
            if (isActive)
            {
                isActive = false;
                UITools.Manager.UnlockControl(); // unlock keys
            }
            faded = false;
            currentText = null;
            currentHLStart = -1;
            currentHLEnd = -1;
            base.Hide();
        }

        public bool CheckPosition(int position) => position == currentPos;

        public void CallTipShow(ScintillaControl sci, int position, string text)
        {
            if (toolTip.Visible && position == memberPos && text == currentText) return;

            toolTip.Visible = false;
            currentText = text;
            SetText(text, true);

            memberPos = position;
            startPos = memberPos + toolTipRTB.Text.IndexOf('(');
            currentPos = sci.CurrentPos;
            deltaPos = startPos - currentPos + 1;
            currentLine = sci.CurrentLine;
            PositionControl(sci);
            // state
            isActive = true;
            faded = false;
            UITools.Manager.LockControl(sci);
        }

        public void PositionControl(ScintillaControl sci)
        {
            // compute control location
            Point p = new Point(sci.PointXFromPosition(memberPos), sci.PointYFromPosition(memberPos));
            p = ((Form)PluginBase.MainForm).PointToClient(sci.PointToScreen(p));
            toolTip.Left = p.X /*+ sci.Left*/;
            bool hasListUp = !CompletionList.Active || CompletionList.listUp;
            if (currentLine > sci.LineFromPosition(memberPos) || !hasListUp) toolTip.Top = p.Y - toolTip.Height /*+ sci.Top*/;
            else toolTip.Top = p.Y + UITools.Manager.LineHeight(sci) /*+ sci.Top*/;
            // Keep on control area
            if (toolTip.Right > ((Form)PluginBase.MainForm).ClientRectangle.Right)
            {
                toolTip.Left = ((Form)PluginBase.MainForm).ClientRectangle.Right - toolTip.Width;
            }
            toolTip.Show();
            toolTip.BringToFront();
        }

        public void CallTipSetHlt(int start, int end) => CallTipSetHlt(start, end, true);

        public void CallTipSetHlt(int start, int end, bool forceRedraw)
        {
            if (!forceRedraw && currentHLStart == start && currentHLEnd == end) return;

            currentHLStart = start;
            currentHLEnd = end;
            if (start != end)
            {
                string savedRawText = rawText;
                try
                {
                    rawText = rawText.Substring(0, start)
                            + HLBgStyleBeg + HLTextStyleBeg
                            + rawText.Substring(start, end - start)
                            + HLTextStyleEnd + HLBgStyleEnd
                            + rawText.Substring(end);

                    Redraw();
                }
                catch { }

                rawText = savedRawText;
            }
            else
            {
                Redraw();
            }
        }

        #region Keys handling

        [Obsolete("Please use MethodCallTip.OnChar(ScintillaControl sci)")]
        public void OnChar(ScintillaControl sci, int value) => OnChar(sci);

        public void OnChar(ScintillaControl sci)
        {
            currentPos++;
            UpdateTip(sci);
        }

        public new void UpdateTip(ScintillaControl sci) => OnUpdateCallTip?.Invoke(sci, currentPos);

        public bool HandleKeys(ScintillaControl sci, Keys key)
        {
            switch (key)
            {
                case Keys.Multiply:
                case Keys.Subtract:
                case Keys.Divide:
                case Keys.Decimal:
                case Keys.Add:
                    return false;

                case Keys.Up:
                    if (!CompletionList.Active) sci.LineUp();
                    return false;
                case Keys.Down:
                    if (!CompletionList.Active) sci.LineDown();
                    return false;
                case Keys.Up | Keys.Shift:
                    sci.LineUpExtend();
                    return false;
                case Keys.Down | Keys.Shift:
                    sci.LineDownExtend();
                    return false;
                case Keys.Left | Keys.Shift:
                    sci.CharLeftExtend();
                    return false;
                case Keys.Right | Keys.Shift:
                    sci.CharRightExtend();
                    return false;

                case Keys.Right:
                    if (!CompletionList.Active)
                    {
                        sci.CharRight();
                        currentPos = sci.CurrentPos;
                        if (sci.CurrentLine != currentLine) Hide();
                        else OnUpdateCallTip?.Invoke(sci, currentPos);
                    }
                    return true;

                case Keys.Left:
                    if (!CompletionList.Active)
                    {
                        sci.CharLeft();
                        currentPos = sci.CurrentPos;
                        if (currentPos < startPos) Hide();
                        else if (sci.CurrentLine != currentLine) Hide();
                        else OnUpdateCallTip?.Invoke(sci, currentPos);
                    }
                    return true;

                case Keys.Back:
                    currentPos = sci.CurrentPos - 1;
                    if (currentPos + deltaPos <= startPos) Hide();
                    else OnUpdateCallTip?.Invoke(sci, currentPos);
                    return false;

                case Keys.Tab:
                case Keys.Space:
                    return false;

                default:
                    if (!CompletionList.Active) Hide();
                    return false;
            }
        }

        #endregion

        #region Controls fading on Control key

        static bool faded;

        internal void FadeOut()
        {
            if (faded) return;
            faded = true;
            //base.Hide();
            toolTip.Visible = false;
        }

        internal void FadeIn()
        {
            if (!faded) return;
            faded = false;
            //base.Show();
            toolTip.Visible = true;
        }
        #endregion
    }
}