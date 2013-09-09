using System;
using System.Text;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ASDocGen.Utilities
{
    public class DialogHelper
    {
        /// <summary>
        /// Shows an error dialog to the user.
        /// </summary>
        public static void ShowError(String message)
        {
            MessageBox.Show(message, " Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Shows a warning dialog to the user.
        /// </summary>
        public static void ShowWarning(String message)
        {
            MessageBox.Show(message, " Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
        }

        /// <summary>
        /// Shows an infromation dialog to the user.
        /// </summary>
        public static void ShowInformation(String message)
        {
            MessageBox.Show(message, " Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

    }

}
