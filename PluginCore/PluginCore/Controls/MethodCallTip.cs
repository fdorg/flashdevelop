using System;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace PluginCore.Controls
{
    
    public class MethodCallTip: RichToolTip
    {
        public delegate void UpdateCallTipHandler(Control sender, int position);

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
        protected int currentLine;

        protected CompletionListControl completionList;

        public MethodCallTip(CompletionListControl owner): base(owner.Host)
        {
            completionList = owner;
            host.VisibleChanged += Host_VisibleChanged;
        }

        public bool CallTipActive
        {
            get { return isActive; }
        }

        public override void Hide()
        {
			if (isActive)
			{
				isActive = false;
			}
            currentText = null;
            currentHLStart = -1;
            currentHLEnd = -1;
            base.Hide();
        }

        public bool CheckPosition(int position)
        {
            return position == currentPos;
        }

		public void CallTipShow(int position, string text)
		{
			CallTipShow(position, text, true);
		}
        public void CallTipShow(int position, string text, bool redraw)
        {
            if (host.Visible && position == memberPos && text == currentText)
            {
                if (owner.GetLineFromCharIndex(owner.CurrentPos) != currentLine)
                    PositionControl();

                return;
            }

            host.Visible = false;
            currentText = text;
			SetText(text, true);

            memberPos = position;
            startPos = memberPos + toolTipRTB.Text.IndexOf('(');
            PositionControl();
            Show();
            // state
            isActive = true;
        }

        public void PositionControl()
        {
            currentPos = owner.CurrentPos;
            currentLine = owner.GetLineFromCharIndex(currentPos);
            // compute control location
            Point p = owner.GetPositionFromCharIndex(memberPos);
            p.Y = owner.GetPositionFromCharIndex(currentPos).Y;
            if (p.Y < 0 || p.Y > owner.Owner.Height || p.X < 0 || p.X > owner.Owner.Width)
            {
                Hide();
                return;
            }
            p = owner.Owner.PointToScreen(p);
            host.Left = p.X /*+ sci.Left*/;
            bool hasListUp = !completionList.Active || completionList.listUp;
            if (!hasListUp) host.Top = p.Y - host.Height /*+ sci.Top*/;
            else host.Top = p.Y + owner.GetLineHeight();
            // Keep on screen area
            var screen = Screen.FromControl(owner.Owner);
            if (host.Right > screen.WorkingArea.Right)
            {
                host.Left = screen.WorkingArea.Right - host.Width;
            }
            if (host.Left < 0)
            {
                host.Left = 0;
            }
        }

		public void CallTipSetHlt(int start, int end)
		{
			CallTipSetHlt(start, end, true);
		}
        public void CallTipSetHlt(int start, int end, bool forceRedraw)
        {
            if (currentHLStart == start && currentHLEnd == end)
            {
                if (owner.GetLineFromCharIndex(owner.CurrentPos) != currentLine)
                    PositionControl();
                return;
            }

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

        private void Host_VisibleChanged(object sender, EventArgs e)
        {
            if (host.Visible)
            {
                owner.KeyDown += Target_KeyDown;
                owner.KeyPosted += Target_KeyPosted;
                owner.KeyPress += Target_KeyPress;
                owner.PositionChanged += Target_PositionChanged;
                owner.LostFocus += Target_LostFocus;
                owner.MouseDown += Target_MouseDown;

                if (!completionList.Active)
                    Application.AddMessageFilter(completionList);
            }
            else
            {
                owner.KeyDown -= Target_KeyDown;
                owner.KeyPosted -= Target_KeyPosted;
                owner.KeyPress -= Target_KeyPress;
                owner.PositionChanged -= Target_PositionChanged;
                owner.LostFocus -= Target_LostFocus;
                owner.MouseDown -= Target_MouseDown;

                if (!completionList.Active)
                    Application.RemoveMessageFilter(completionList);
            }
        }

        private void Target_KeyDown(object sender, KeyEventArgs e)
        {
            if (!e.Handled)
                e.SuppressKeyPress = e.Handled = HandleKeys(e.KeyData);
        }

        private void Target_KeyPosted(object sender, KeyEventArgs e)
        {
            HandlePostedKeys(e.KeyData);
        }

        private void Target_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar))
                OnChar(e.KeyChar);
        }

        private void Target_PositionChanged(object sender, EventArgs e)
        {
            PositionControl();
        }

        private void Target_LostFocus(object sender, EventArgs e)
        {
            if (!Focused && !completionList.Tip.Focused)
                Hide();
        }

        private void Target_MouseDown(object sender, MouseEventArgs e)
        {
            if (owner.CurrentPos != currentPos)
                Hide();
        }

        #region Keys handling

        public void OnChar(int value)
        {
            currentPos++;
            UpdateTip();
        }

        public override void UpdateTip()
        {
            if (CallTipActive && OnUpdateCallTip != null) OnUpdateCallTip(owner.Owner, currentPos);
        }

        public bool HandleKeys(Keys key)
        {
            switch (key)
            {
                case Keys.PageDown:
                case Keys.PageUp:
                    if (!completionList.Active)
                        Hide();
                    break;

                case Keys.Up | Keys.Shift:
                case Keys.Down | Keys.Shift:
                case Keys.PageDown | Keys.Shift:
                case Keys.PageUp | Keys.Shift:
                    Hide();
                    break;

                case Keys.Escape:
                    Hide();
                    return true;
            }

            return false;
        }

        private void HandlePostedKeys(Keys key)
        {
            switch (key)
            {
                case Keys.Right:
                case Keys.Right | Keys.Shift:
                case Keys.Right | Keys.Control:
                    currentPos = owner.CurrentPos;
                    if (OnUpdateCallTip != null) OnUpdateCallTip(owner.Owner, currentPos);
                    break;

                case Keys.Left:
                case Keys.Left | Keys.Shift:
                case Keys.Left | Keys.Control:
                    currentPos = owner.CurrentPos;
                    if (currentPos < startPos) Hide();
                    if (OnUpdateCallTip != null) OnUpdateCallTip(owner.Owner, currentPos);
                    break;

                case Keys.Up:
                case Keys.Down:
                    currentPos = owner.CurrentPos;
                    if (!completionList.Active)
                        if (OnUpdateCallTip != null) OnUpdateCallTip(owner.Owner, currentPos);
                    break;

                case Keys.PageDown:
                case Keys.PageUp:
                    if (!completionList.Active)
                        Hide();
                    break;

                case Keys.Up | Keys.Shift:
                case Keys.Down | Keys.Shift:
                case Keys.PageDown | Keys.Shift:
                case Keys.PageUp | Keys.Shift:
                    Hide();
                    break;

                case Keys.Back:
                case Keys.Back | Keys.Control:
                case Keys.Delete:
                case Keys.Delete | Keys.Control:
                    currentPos = owner.CurrentPos;
                    if (currentPos < startPos) Hide();
                    if (OnUpdateCallTip != null) OnUpdateCallTip.BeginInvoke(owner.Owner, currentPos, null, null);
                    break;

                case Keys.Escape:
                    Hide();
                    break;
            }
        }

        #endregion

        #region Controls fading on Control key

        internal void FadeOut()
        {
            if (host.Opacity != 1) return;
            host.Opacity = 0;
        }

        internal void FadeIn()
        {
            if (host.Opacity == 1) return;
            host.Opacity = 1;
        }
        #endregion
    }
}
