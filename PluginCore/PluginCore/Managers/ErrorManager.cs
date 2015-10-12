using System;
using System.IO;
using System.Windows.Forms;
using PluginCore.Helpers;
using PluginCore.Localization;

namespace PluginCore.Managers
{
    public class ErrorManager
    {
        /// <summary>
        /// Enables/disables the log file output
        /// </summary>
        public static Boolean OutputIsEnabled = true;

        /// <summary>
        /// Shows a visible info message to the user
        /// </summary>
        public static void ShowInfo(String info)
        {
            if ((PluginBase.MainForm as Form).InvokeRequired)
            {
                AddToLog("Unsafe ShowInfo: " + info, null);
                return;
            }
            String title = TextHelper.GetString("FlashDevelop.Title.InfoDialog");
            MessageBox.Show(PluginBase.MainForm, info, " " + title, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Shows a visible warning message to the user
        /// </summary>
        public static void ShowWarning(String info, Exception exception)
        {
            if ((PluginBase.MainForm as Form).InvokeRequired)
            {
                AddToLog("Unsafe ShowWarning: " + info, exception);
                return;
            }
            String title = TextHelper.GetString("FlashDevelop.Title.WarningDialog");
            MessageBox.Show(PluginBase.MainForm, info, " " + title, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            if (OutputIsEnabled && exception != null) AddToLog(info, exception);
        }

        /// <summary>
        /// Shows a simple error dialog to the user
        /// </summary>
        public static void ShowError(String info, Exception exception)
        {
            if ((PluginBase.MainForm as Form).InvokeRequired)
            {
                AddToLog("Unsafe ShowError: " + info, exception);
                return;
            }
            String title = TextHelper.GetString("FlashDevelop.Title.ErrorDialog");
            MessageBox.Show(PluginBase.MainForm, info, " " + title, MessageBoxButtons.OK, MessageBoxIcon.Error);
            if (OutputIsEnabled) AddToLog(info, exception);
        }

        /// <summary>
        /// Shows a visible error message to the user
        /// </summary>
        public static void ShowError(Exception exception)
        {
            if ((PluginBase.MainForm as Form).InvokeRequired)
            {
                AddToLog("Unsafe ShowError", exception);
                return;
            }
            PluginBase.MainForm.ShowErrorDialog(new ErrorManager(), exception);
            if (OutputIsEnabled) AddToLog(null, exception);
        }

        /// <summary>
        /// Adds the message and exception to the error log silently
        /// </summary>
        public static void AddToLog(String message, Exception exception)
        {
            String result = String.Empty;
            result += DateTime.Now.ToString() + "\r\n\r\n";
            if (message != null) result += message.ToString() + "\r\n\r\n";
            if (exception != null) result += exception.ToString() + "\r\n\r\n";
            try
            {
                String fileName = Path.Combine(PathHelper.BaseDir, "Exceptions.log");
                using (StreamWriter sw = new StreamWriter(fileName, true))
                {
                    sw.Write(result);
                }
            }
            catch {}
        }

    }

}
