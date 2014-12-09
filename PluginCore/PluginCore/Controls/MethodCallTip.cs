using System;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
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
        }

        public bool CallTipActive
        {
            get { return isActive; }
        }

        public bool Focused
        {
            get { return toolTipRTB.Focused; }
        }

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

        public bool CheckPosition(int position)
        {
            return position == currentPos;
        }

		public void CallTipShow(ScintillaControl sci, int position, string text)
		{
			CallTipShow(sci, position, text, true);
		}
        public void CallTipShow(ScintillaControl sci, int position, string text, bool redraw)
        {
            if (toolTip.Visible && position == memberPos && text == currentText)
                return;

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
            p = ((Form)PluginBase.MainForm).PointToClient(((Control)sci).PointToScreen(p));
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

		public void CallTipSetHlt(int start, int end)
		{
			CallTipSetHlt(start, end, true);
		}
        public void CallTipSetHlt(int start, int end, bool forceRedraw)
        {
			if (currentHLStart == start && currentHLEnd == end)
				return;

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

        public void OnChar(ScintillaControl sci, int value)
        {
            currentPos++;
            UpdateTip(sci);
        }

        public new void UpdateTip(ScintillaControl sci)
        {
            if (OnUpdateCallTip != null) OnUpdateCallTip(sci, currentPos);
        }

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
                        else if (OnUpdateCallTip != null) OnUpdateCallTip(sci, currentPos);
                    }
                    return true;

                case Keys.Left:
                    if (!CompletionList.Active)
                    {
                        sci.CharLeft();
                        currentPos = sci.CurrentPos;
                        if (currentPos < startPos) Hide();
                        else
                        {
                            if (sci.CurrentLine != currentLine) Hide();
                            else if (OnUpdateCallTip != null) OnUpdateCallTip(sci, currentPos);
                        }
                    }
                    return true;

                case Keys.Back:
                    sci.DeleteBack();
                    currentPos = sci.CurrentPos;
					if (currentPos + deltaPos < startPos) Hide();
                    else if (OnUpdateCallTip != null) OnUpdateCallTip(sci, currentPos);
                    return true;

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
        private static bool faded;

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
