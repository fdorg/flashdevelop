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
        public static bool OutputIsEnabled = true;

        /// <summary>
        /// Shows a visible info message to the user
        /// </summary>
        public static void ShowInfo(string info)
        {
            if (((Form) PluginBase.MainForm).InvokeRequired)
            {
                AddToLog($"Unsafe ShowInfo: {info}", null);
                return;
            }
            var title = TextHelper.GetString("FlashDevelop.Title.InfoDialog");
            MessageBox.Show(PluginBase.MainForm, info, $" {title}", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Shows a visible warning message to the user
        /// </summary>
        public static void ShowWarning(string info, Exception exception)
        {
            if (((Form) PluginBase.MainForm).InvokeRequired)
            {
                AddToLog($"Unsafe ShowWarning: {info}", exception);
                return;
            }
            var title = TextHelper.GetString("FlashDevelop.Title.WarningDialog");
            MessageBox.Show(PluginBase.MainForm, info, $" {title}", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            if (OutputIsEnabled && exception != null) AddToLog(info, exception);
        }

        /// <summary>
        /// Shows a simple error dialog to the user
        /// </summary>
        public static void ShowError(string info, Exception exception)
        {
            if (((Form) PluginBase.MainForm).InvokeRequired)
            {
                AddToLog($"Unsafe ShowError: {info}", exception);
                return;
            }
            var title = TextHelper.GetString("FlashDevelop.Title.ErrorDialog");
            MessageBox.Show(PluginBase.MainForm, info, $" {title}", MessageBoxButtons.OK, MessageBoxIcon.Error);
            if (OutputIsEnabled) AddToLog(info, exception);
        }

        /// <summary>
        /// Shows a visible error message to the user
        /// </summary>
        public static void ShowError(Exception exception)
        {
            if (((Form) PluginBase.MainForm).InvokeRequired)
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
        public static void AddToLog(string message, Exception exception)
        {
            var result = string.Empty;
            result += DateTime.Now + "\r\n\r\n";
            if (message != null) result += message + "\r\n\r\n";
            if (exception != null) result += exception + "\r\n\r\n";
            try
            {
                var fileName = Path.Combine(PathHelper.BaseDir, "Exceptions.log");
                using var sw = new StreamWriter(fileName, true);
                sw.Write(result);
            }
            catch {}
        }

    }

}
