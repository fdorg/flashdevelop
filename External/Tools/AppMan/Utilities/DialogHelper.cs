using System.IO;
using System.Windows.Forms;

namespace AppMan.Utilities
{
    public class DialogHelper
    {
        /// <summary>
        /// Shows an error dialog to the user.
        /// </summary>
        public static void ShowError(string message)
        {
            MessageBox.Show(message, " Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            try
            {
                var logFile = Path.Combine(PathHelper.LOG_DIR, "Exceptions.log");
                File.AppendAllText(logFile, message + "\n\r\n\r");
            }
            catch { /* NO ERRORS */ }
        }

        /// <summary>
        /// Shows a warning dialog to the user.
        /// </summary>
        public static void ShowWarning(string message) => MessageBox.Show(message, " Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

        /// <summary>
        /// Shows an information dialog to the user.
        /// </summary>
        public static void ShowInformation(string message) => MessageBox.Show(message, " Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
}