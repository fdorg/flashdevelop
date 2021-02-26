using System;
using System.Windows.Forms;
using FlashDevelop.Docking;
using FlashDevelop.Settings;
using PluginCore;
using PluginCore.Localization;

namespace FlashDevelop.Managers
{
    internal class FilePollManager
    {
        static Timer FilePollTimer;
        static bool YesToAll;

        /// <summary>
        /// Initialize the file change polling
        /// </summary>
        public static void InitializePolling()
        {
            CheckSettingValues();
            FilePollTimer = new Timer {Interval = ((SettingObject) PluginBase.Settings).FilePollInterval};
            FilePollTimer.Tick += FilePollTimerTick;
            FilePollTimer.Start();
        }

        /// <summary>
        /// Checks the setting value validity
        /// </summary>
        static void CheckSettingValues()
        {
            var settings = (SettingObject)PluginBase.Settings;
            if (settings.FilePollInterval == 0) settings.FilePollInterval = 3000;
        }

        /// <summary>
        /// Checks if a file has been changed outside
        /// </summary>
        static void CheckFileChange(ITabbedDocument document)
        {
            if (document is TabbedDocument {SciControl: { } sci} doc && doc.CheckFileChange())
            {
                if (PluginBase.Settings.AutoReloadModifiedFiles)
                {
                    doc.RefreshFileInfo();
                    doc.Reload(false);
                }
                else
                {
                    if (YesToAll)
                    {
                        doc.RefreshFileInfo();
                        doc.Reload(false);
                        return;
                    }
                    string dlgTitle = TextHelper.GetString("Title.InfoDialog");
                    string dlgMessage = TextHelper.GetString("Info.FileIsModifiedOutside");
                    string formatted = string.Format(dlgMessage, "\n", sci.FileName);
                    MessageBoxManager.Cancel = TextHelper.GetString("Label.YesToAll");
                    MessageBoxManager.Register(); // Use custom labels...
                    var result = MessageBox.Show(PluginBase.MainForm, formatted, " " + dlgTitle, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);
                    doc.RefreshFileInfo(); // User may have waited before responding, save info now
                    if (result == DialogResult.Yes) doc.Reload(false);
                    else if (result == DialogResult.Cancel)
                    {
                        doc.Reload(false);
                        YesToAll = true;
                    }
                    MessageBoxManager.Unregister();
                }
            }
        }

        /// <summary>
        /// After an interval check if the files have changed
        /// </summary>
        static void FilePollTimerTick(object sender, EventArgs e)
        {
            try
            {
                FilePollTimer.Enabled = false;
                var current = PluginBase.MainForm.CurrentDocument;
                CheckFileChange(current); // Check the current first..
                foreach (var document in PluginBase.MainForm.Documents)
                {
                    if (document != current) CheckFileChange(document);
                }
                FilePollTimer.Enabled = true;
                YesToAll = false;
            }
            catch { /* No errors shown here.. */ }
        }
    }
}