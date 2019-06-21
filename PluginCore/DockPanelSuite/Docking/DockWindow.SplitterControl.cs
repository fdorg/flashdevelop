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
