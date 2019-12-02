// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System.Drawing;
using System.Windows.Forms;

namespace WeifenLuo.WinFormsUI.Docking
{
    internal static class DockHelper
    {
        public static bool IsDockStateAutoHide(DockState dockState)
        {
            if (dockState == DockState.DockLeftAutoHide ||
                dockState == DockState.DockRightAutoHide ||
                dockState == DockState.DockTopAutoHide ||
                dockState == DockState.DockBottomAutoHide)
                return true;
            return false;
        }

        public static bool IsDockStateValid(DockState dockState, DockAreas dockableAreas)
        {
            if (((dockableAreas & DockAreas.Float) == 0) &&
                (dockState == DockState.Float))
                return false;
            if (((dockableAreas & DockAreas.Document) == 0) &&
                (dockState == DockState.Document))
                return false;
            if (((dockableAreas & DockAreas.DockLeft) == 0) &&
                (dockState == DockState.DockLeft || dockState == DockState.DockLeftAutoHide))
                return false;
            if (((dockableAreas & DockAreas.DockRight) == 0) &&
                (dockState == DockState.DockRight || dockState == DockState.DockRightAutoHide))
                return false;
            if (((dockableAreas & DockAreas.DockTop) == 0) &&
                (dockState == DockState.DockTop || dockState == DockState.DockTopAutoHide))
                return false;
            if (((dockableAreas & DockAreas.DockBottom) == 0) &&
                (dockState == DockState.DockBottom || dockState == DockState.DockBottomAutoHide))
                return false;
            return true;
        }

        public static bool IsDockWindowState(DockState state)
        {
            if (state == DockState.DockTop || state == DockState.DockBottom || state == DockState.DockLeft ||
                state == DockState.DockRight || state == DockState.Document)
                return true;
            return false;
        }

        public static DockState ToggleAutoHideState(DockState state)
        {
            if (state == DockState.DockLeft)
                return DockState.DockLeftAutoHide;
            if (state == DockState.DockRight)
                return DockState.DockRightAutoHide;
            if (state == DockState.DockTop)
                return DockState.DockTopAutoHide;
            if (state == DockState.DockBottom)
                return DockState.DockBottomAutoHide;
            if (state == DockState.DockLeftAutoHide)
                return DockState.DockLeft;
            if (state == DockState.DockRightAutoHide)
                return DockState.DockRight;
            if (state == DockState.DockTopAutoHide)
                return DockState.DockTop;
            if (state == DockState.DockBottomAutoHide)
                return DockState.DockBottom;
            return state;
        }

        public static DockPane PaneAtPoint(Point pt, DockPanel dockPanel)
        {
            if (!NativeMethods.ShouldUseWin32()) return null;
            for (Control control = Win32Helper.ControlAtPoint(pt); control != null; control = control.Parent)
            {
                IDockContent content = control as IDockContent;
                if (content != null && content.DockHandler.DockPanel == dockPanel)
                    return content.DockHandler.Pane;

                DockPane pane = control as DockPane;
                if (pane != null && pane.DockPanel == dockPanel)
                    return pane;
            }

            return null;
        }

        public static FloatWindow FloatWindowAtPoint(Point pt, DockPanel dockPanel)
        {
            if (!NativeMethods.ShouldUseWin32()) return null;
            for (Control control = Win32Helper.ControlAtPoint(pt); control != null; control = control.Parent)
            {
                FloatWindow floatWindow = control as FloatWindow;
                if (floatWindow != null && floatWindow.DockPanel == dockPanel)
                    return floatWindow;
            }

            return null;
        }
    }
}
