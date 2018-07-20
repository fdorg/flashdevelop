// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
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
