using System.Drawing;

namespace WeifenLuo.WinFormsUI.Docking
{
    public sealed class NestedDockingStatus
    {
        internal NestedDockingStatus(DockPane pane)
        {
            m_dockPane = pane;
        }

        private DockPane m_dockPane = null;
        public DockPane DockPane => m_dockPane;

        private NestedPaneCollection m_nestedPanes = null;
        public NestedPaneCollection NestedPanes => m_nestedPanes;

        private DockPane m_previousPane = null;
        public DockPane PreviousPane => m_previousPane;

        private DockAlignment m_alignment = DockAlignment.Left;
        public DockAlignment Alignment => m_alignment;

        private double m_proportion = 0.5;
        public double Proportion => m_proportion;

        private bool m_isDisplaying = false;
        public bool IsDisplaying => m_isDisplaying;

        private DockPane m_displayingPreviousPane = null;
        public DockPane DisplayingPreviousPane => m_displayingPreviousPane;

        private DockAlignment m_displayingAlignment = DockAlignment.Left;
        public DockAlignment DisplayingAlignment => m_displayingAlignment;

        private double m_displayingProportion = 0.5;
        public double DisplayingProportion => m_displayingProportion;

        private Rectangle m_logicalBounds = Rectangle.Empty; 
        public Rectangle LogicalBounds => m_logicalBounds;

        private Rectangle m_paneBounds = Rectangle.Empty;
        public Rectangle PaneBounds => m_paneBounds;

        private Rectangle m_splitterBounds = Rectangle.Empty;
        public Rectangle SplitterBounds => m_splitterBounds;

        internal void SetStatus(NestedPaneCollection nestedPanes, DockPane previousPane, DockAlignment alignment, double proportion)
        {
            m_nestedPanes = nestedPanes;
            m_previousPane = previousPane;
            m_alignment = alignment;
            m_proportion = proportion;
        }

        internal void SetDisplayingStatus(bool isDisplaying, DockPane displayingPreviousPane, DockAlignment displayingAlignment, double displayingProportion)
        {
            m_isDisplaying = isDisplaying;
            m_displayingPreviousPane = displayingPreviousPane;
            m_displayingAlignment = displayingAlignment;
            m_displayingProportion = displayingProportion;
        }

        internal void SetDisplayingBounds(Rectangle logicalBounds, Rectangle paneBounds, Rectangle splitterBounds)
        {
            m_logicalBounds = logicalBounds;
            m_paneBounds = paneBounds;
            m_splitterBounds = splitterBounds;
        }
    }
}
