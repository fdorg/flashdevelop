// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System.Windows.Forms;
using System.Drawing;

namespace WeifenLuo.WinFormsUI.Docking
{
    partial class DockPanel
    {
        /// <summary>
        /// DragHandlerBase is the base class for drag handlers. The derived class should:
        ///   1. Define its public method BeginDrag. From within this public BeginDrag method,
        ///      DragHandlerBase.BeginDrag should be called to initialize the mouse capture
        ///      and message filtering.
        ///   2. Override the OnDragging and OnEndDrag methods.
        /// </summary>
        abstract class DragHandlerBase : NativeWindow, IMessageFilter
        {
            protected DragHandlerBase()
            {
            }

            protected abstract Control DragControl
            {
                get;
            }

            Point m_startMousePosition = Point.Empty;
            protected Point StartMousePosition
            {
                get => m_startMousePosition;
                private set => m_startMousePosition = value;
            }

            protected bool BeginDrag()
            {
                // Avoid re-entrance;
                lock (this)
                {
                    if (DragControl is null)
                        return false;

                    StartMousePosition = Control.MousePosition;

                    if (NativeMethods.ShouldUseWin32())
                    {
                        if (!NativeMethods.DragDetect(DragControl.Handle, StartMousePosition)) return false;
                    }
                    DragControl.FindForm().Capture = true;
                    AssignHandle(DragControl.FindForm().Handle);
                    Application.AddMessageFilter(this);
                    return true;
                }
            }

            protected abstract void OnDragging();

            protected abstract void OnEndDrag(bool abort);

            void EndDrag(bool abort)
            {
                ReleaseHandle();
                Application.RemoveMessageFilter(this);
                DragControl.FindForm().Capture = false;

                OnEndDrag(abort);
            }

            bool IMessageFilter.PreFilterMessage(ref Message m)
            {
                if (m.Msg == (int)Win32.Msgs.WM_MOUSEMOVE)
                    OnDragging();
                else if (m.Msg == (int)Win32.Msgs.WM_LBUTTONUP)
                    EndDrag(false);
                else if (m.Msg == (int)Win32.Msgs.WM_CAPTURECHANGED)
                    EndDrag(true);
                else if (m.Msg == (int)Win32.Msgs.WM_KEYDOWN && (int)m.WParam == (int)Keys.Escape)
                    EndDrag(true);

                return OnPreFilterMessage(ref m);
            }

            protected virtual bool OnPreFilterMessage(ref Message m)
            {
                return false;
            }

            protected sealed override void WndProc(ref Message m)
            {
                if (m.Msg == (int)Win32.Msgs.WM_CANCELMODE || m.Msg == (int)Win32.Msgs.WM_CAPTURECHANGED)
                    EndDrag(true);

                base.WndProc(ref m);
            }
        }

        abstract class DragHandler : DragHandlerBase
        {
            readonly DockPanel m_dockPanel;

            protected DragHandler(DockPanel dockPanel)
            {
                m_dockPanel = dockPanel;
            }

            public DockPanel DockPanel => m_dockPanel;

            IDragSource m_dragSource;
            protected IDragSource DragSource
            {
                get => m_dragSource;
                set => m_dragSource = value;
            }

            protected sealed override Control DragControl => DragSource is null ? null : DragSource.DragControl;

            protected sealed override bool OnPreFilterMessage(ref Message m)
            {
                if ((m.Msg == (int)Win32.Msgs.WM_KEYDOWN || m.Msg == (int)Win32.Msgs.WM_KEYUP) &&
                    ((int)m.WParam == (int)Keys.ControlKey || (int)m.WParam == (int)Keys.ShiftKey))
                    OnDragging();

                return base.OnPreFilterMessage(ref m);
            }
        }
    }
}
