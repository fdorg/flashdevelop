using System.Drawing;

namespace WeifenLuo.WinFormsUI.Docking
{
    public sealed class NestedDockingStatus
    {
        internal NestedDockingStatus(DockPane pane)
        {
            m_dockPane = pane;
        }

        readonly DockPane m_dockPane = null;
        public DockPane DockPane => m_dockPane;

        NestedPaneCollection m_nestedPanes = null;
        public NestedPaneCollection NestedPanes => m_nestedPanes;

        DockPane m_previousPane = null;
        public DockPane PreviousPane => m_previousPane;

        DockAlignment m_alignment = DockAlignment.Left;
        public DockAlignment Alignment => m_alignment;

        double m_proportion = 0.5;
        public double Proportion => m_proportion;

        bool m_isDisplaying = false;
        public bool IsDisplaying => m_isDisplaying;

        DockPane m_displayingPreviousPane = null;
        public DockPane DisplayingPreviousPane => m_displayingPreviousPane;

        DockAlignment m_displayingAlignment = DockAlignment.Left;
        public DockAlignment DisplayingAlignment => m_displayingAlignment;

        double m_displayingProportion = 0.5;
        public double DisplayingProportion => m_displayingProportion;

        Rectangle m_logicalBounds = Rectangle.Empty; 
        public Rectangle LogicalBounds => m_logicalBounds;

        Rectangle m_paneBounds = Rectangle.Empty;
        public Rectangle PaneBounds => m_paneBounds;

        Rectangle m_splitterBounds = Rectangle.Empty;
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
