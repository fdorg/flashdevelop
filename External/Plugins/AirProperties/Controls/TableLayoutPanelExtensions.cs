using System.Collections;
using System.Windows.Forms;

namespace AirProperties
{
    public static class TableLayoutPanelExtensions
    {
        public static void RemoveRow(this TableLayoutPanel tableLayoutPanel, int rowNumber)
        {
            ArrayList controls = new ArrayList(tableLayoutPanel.Controls);
            foreach (Control control in controls)
            {
                int row = tableLayoutPanel.GetRow(control);
                if (row == rowNumber)
                {
                    tableLayoutPanel.Controls.Remove(control);
                }
            }           
            tableLayoutPanel.RowStyles.RemoveAt(rowNumber);
            foreach (Control control in tableLayoutPanel.Controls)
            {
                int row = tableLayoutPanel.GetRow(control);
                if (row > rowNumber)
                {
                    tableLayoutPanel.SetRow(control, row - 1);
                }
            }
            tableLayoutPanel.PerformLayout();
            tableLayoutPanel.ResumeLayout(true);
            tableLayoutPanel.Refresh();
        }

    }

}

// You need this once (only), and it must be in this namespace
namespace System.Runtime.CompilerServices
{
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class ExtensionAttribute : Attribute {}
}
