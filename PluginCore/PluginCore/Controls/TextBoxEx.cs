using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace PluginCore.Controls
{
    public class TextBoxEx : TextBox
    {
        private const int WM_SYSKEYDOWN = 0x0104;

        public event KeyEventHandler KeyPosted; //Hacky event for MethodCallTip, although with some rather valid use cases
        public event ScrollEventHandler Scroll;

        protected override void DefWndProc(ref Message m)
        {
            base.DefWndProc(ref m);

            if (m.Msg == Win32.WM_KEYDOWN || m.Msg == WM_SYSKEYDOWN)  // If we're worried about performance/GC, we can store latest OnKeyDown e
                OnKeyPosted(new KeyEventArgs((Keys)((int)m.WParam) | ModifierKeys));
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case Win32.WM_HSCROLL:
                case Win32.WM_VSCROLL:
                case Win32.WM_MOUSEWHEEL:
                    WmScroll(ref m);
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }

        }

        /// <summary>
        ///     Raises the <see cref="Scroll"/> event.
        /// </summary>
        /// <param name="e">An <see cref="ScrollEventArgs"/> that contains the event data.</param>
        protected virtual void OnScroll(ScrollEventArgs e)
        {
            if (Scroll != null)
                Scroll(this, e);
        }

        protected virtual void OnKeyPosted(KeyEventArgs e)
        {
            if (KeyPosted != null)
                KeyPosted(this, e);
        }

        private void WmScroll(ref Message m)
        {
            ScrollOrientation so;
            ScrollEventType set = (ScrollEventType)((short)((int)(long)m.WParam & 0xffff));

            // We're not interested in the actual scroll change right now
            if (m.Msg == Win32.WM_HSCROLL)
            {
                so = ScrollOrientation.HorizontalScroll;
                base.WndProc(ref m);
            }
            else
            {
                so = ScrollOrientation.VerticalScroll;
                base.WndProc(ref m);
            }

            OnScroll(new ScrollEventArgs(set, 0, 0, so));
        }

    }

    public class TextBoxTarget : ICompletionListHost
    {

        #region ICompletionListTarget Members

        public event EventHandler LostFocus
        {
            add { _owner.LostFocus += value; }
            remove { _owner.LostFocus -= value; }
        }

        private EventHandler positionChanged;
        public event EventHandler PositionChanged
        {
            add
            {
                if (positionChanged == null || positionChanged.GetInvocationList().Length == 0)
                {
                    _owner.Scroll += Owner_Scroll;
                    BuildControlHierarchy(_owner);
                }
                positionChanged += value;
            }
            remove
            {
                positionChanged -= value;
                if (positionChanged == null || positionChanged.GetInvocationList().Length < 1)
                {
                    _owner.Scroll -= Owner_Scroll;
                    ClearControlHierarchy();
                }
            }
        }

        public event EventHandler SizeChanged
        {
            add { Owner.SizeChanged += value; }
            remove { Owner.SizeChanged -= value; }
        }

        public event KeyEventHandler KeyDown
        {
            add { _owner.KeyDown += value; }
            remove { _owner.KeyDown -= value; }
        }

        public event KeyEventHandler KeyPosted
        {
            add { _owner.KeyPosted += value; }
            remove { _owner.KeyPosted -= value; }
        }

        public event KeyPressEventHandler KeyPress
        {
            add { _owner.KeyPress += value; }
            remove { _owner.KeyPress -= value; }
        }

        public event MouseEventHandler MouseDown
        {
            add { _owner.MouseDown += value; }
            remove { _owner.MouseDown -= value; }
        }

        private TextBoxEx _owner;
        public Control Owner
        {
            get { return _owner; }
        }

        public string SelectedText
        {
            get { return _owner.SelectedText; }
            set { _owner.SelectedText = value; }
        }

        public int SelectionEnd
        {
            get { return _owner.SelectionStart + _owner.SelectionLength; }
            set { _owner.SelectionLength = value - _owner.SelectionStart; }
        }

        public int SelectionStart
        {
            get { return _owner.SelectionStart; }
            set { _owner.SelectionStart = value; }
        }

        public int CurrentPos
        {
            get { return _owner.SelectionStart; }
        }

        public bool IsEditable
        {
            get { return !_owner.ReadOnly; }
        }

        public TextBoxTarget(TextBoxEx owner)
        {
            _owner = owner;
        }

        public int GetLineFromCharIndex(int pos)
        {
            return _owner.GetLineFromCharIndex(pos);
        }

        public Point GetPositionFromCharIndex(int pos)
        {
            return _owner.GetPositionFromCharIndex(pos == _owner.TextLength ? pos - 1 : pos);
        }

        public int GetLineHeight()
        {
            using (Graphics g = _owner.CreateGraphics())
            {
                SizeF textSize = g.MeasureString("S", _owner.Font);
                return (int)Math.Ceiling(textSize.Height);
            }
        }

        public void SetSelection(int start, int end)
        {
            _owner.SelectionStart = start;
            _owner.SelectionLength = end - start;
        }

        public void BeginUndoAction()
        {
            // TODO
        }

        public void EndUndoAction()
        {
            // TODO
        }

        #endregion

        private List<Control> controlHierarchy = new List<Control>();

        private void BuildControlHierarchy(Control current)
        {
            while (current != null)
            {
                current.LocationChanged += Control_LocationChanged;
                current.ParentChanged += Control_ParentChanged;
                controlHierarchy.Add(current);
                current = current.Parent;
            }
        }

        private void ClearControlHierarchy()
        {
            foreach (var control in controlHierarchy)
            {
                control.LocationChanged -= Control_LocationChanged;
                control.ParentChanged -= Control_ParentChanged;
            }
            controlHierarchy.Clear();
        }

        private void Control_LocationChanged(object sender, EventArgs e)
        {
            if (positionChanged != null)
                positionChanged(sender, e);
        }

        private void Control_ParentChanged(object sender, EventArgs e)
        {
            ClearControlHierarchy();
            BuildControlHierarchy(_owner);
            if (positionChanged != null)
                positionChanged(sender, e);
        }

        private void Owner_Scroll(object sender, ScrollEventArgs e)
        {
            if (positionChanged != null)
                positionChanged(sender, e);
        }

    }

}
