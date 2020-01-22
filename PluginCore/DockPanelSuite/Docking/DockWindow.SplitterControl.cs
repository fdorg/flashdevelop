// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
namespace WeifenLuo.WinFormsUI.Docking
{
    public partial class DockWindow
    {
        private class SplitterControl : SplitterBase
        {
            protected override int SplitterSize => Measures.SplitterSize;

            protected override void StartDrag()
            {
                DockWindow window = Parent as DockWindow;

                window?.DockPanel.BeginDrag(window, window.RectangleToScreen(Bounds));
            }
        }
    }
}
