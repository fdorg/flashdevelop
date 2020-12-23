using System.Drawing;
using System.Windows.Forms;

namespace WeifenLuo.WinFormsUI.Docking
{
    internal static class DockHelper
    {
        public static bool IsDockStateAutoHide(DockState dockState)
        {
            return dockState == DockState.DockLeftAutoHide
                   || dockState == DockState.DockRightAutoHide
                   || dockState == DockState.DockTopAutoHide
                   || dockState == DockState.DockBottomAutoHide;
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
            return state == DockState.DockTop
                   || state == DockState.DockBottom
                   || state == DockState.DockLeft
                   || state == DockState.DockRight
                   || state == DockState.Document;
        }

        public static DockState ToggleAutoHideState(DockState state)
        {
            return state switch
            {
                DockState.DockLeft => DockState.DockLeftAutoHide,
                DockState.DockRight => DockState.DockRightAutoHide,
                DockState.DockTop => DockState.DockTopAutoHide,
                DockState.DockBottom => DockState.DockBottomAutoHide,
                DockState.DockLeftAutoHide => DockState.DockLeft,
                DockState.DockRightAutoHide => DockState.DockRight,
                DockState.DockTopAutoHide => DockState.DockTop,
                DockState.DockBottomAutoHide => DockState.DockBottom,
                _ => state
            };
        }

        public static DockPane PaneAtPoint(Point pt, DockPanel dockPanel)
        {
            if (!NativeMethods.ShouldUseWin32()) return null;
            for (Control control = Win32Helper.ControlAtPoint(pt); control != null; control = control.Parent)
            {
                switch (control)
                {
                    case IDockContent content when content.DockHandler.DockPanel == dockPanel:
                        return content.DockHandler.Pane;
                    case DockPane pane when pane.DockPanel == dockPanel:
                        return pane;
                }
            }
            return null;
        }

        public static FloatWindow FloatWindowAtPoint(Point pt, DockPanel dockPanel)
        {
            if (!NativeMethods.ShouldUseWin32()) return null;
            for (Control control = Win32Helper.ControlAtPoint(pt); control != null; control = control.Parent)
            {
                if (control is FloatWindow window && window.DockPanel == dockPanel)
                    return window;
            }
            return null;
        }
    }
}
