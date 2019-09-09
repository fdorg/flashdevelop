using System.Collections.Generic;
using System.Windows.Forms;

namespace AirProperties
{
    public static class TableLayoutPanelExtensions
    {
        public static void RemoveRow(this TableLayoutPanel tableLayoutPanel, int rowNumber)
        {
            var controls = new List<Control>((IEnumerable<Control>) tableLayoutPanel.Controls);
            foreach (var control in controls)
            {
                var row = tableLayoutPanel.GetRow(control);
                if (row == rowNumber)
                {
                    tableLayoutPanel.Controls.Remove(control);
                }
            }           
            tableLayoutPanel.RowStyles.RemoveAt(rowNumber);
            foreach (Control control in tableLayoutPanel.Controls)
            {
                var row = tableLayoutPanel.GetRow(control);
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