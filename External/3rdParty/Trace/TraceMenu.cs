using System.Windows.Forms;

namespace Trace
{
    public class TraceMenu : ToolStripMenuItem
    {
        public ToolStripMenuItem TraceSimple;
        public ToolStripMenuItem TraceForIn;
        public ToolStripMenuItem TraceAlternateSimple;
        public ToolStripMenuItem TraceAlternateForIn;

        public TraceMenu()
        {
            TraceSimple = new ToolStripMenuItem("&Simple");
            TraceForIn = new ToolStripMenuItem("&For..in");
            TraceAlternateSimple = new ToolStripMenuItem("Alternate Simple");
            TraceAlternateForIn = new ToolStripMenuItem("Alternate For..in");

            Text = "Trace";
            DropDownItems.Add(TraceSimple);
            DropDownItems.Add(TraceForIn);
            DropDownItems.Add(new ToolStripSeparator());
            DropDownItems.Add(TraceAlternateSimple);
            DropDownItems.Add(TraceAlternateForIn);
        }
    }
}