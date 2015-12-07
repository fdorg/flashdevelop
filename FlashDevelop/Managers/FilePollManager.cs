using System;
using System.Windows.Forms;
using FlashDevelop.Docking;
using PluginCore;
using PluginCore.Localization;

namespace FlashDevelop.Managers
{
    class FilePollManager
    {
        private static Timer FilePollTimer;
        private static Boolean YesToAll = false;

        /// <summary>
        /// Initialize the file change polling
        /// </summary>
        public static void InitializePolling()
        {
            CheckSettingValues();
            FilePollTimer = new Timer();
            FilePollTimer.Interval = Globals.Settings.FilePollInterval;
            FilePollTimer.Tick += new EventHandler(FilePollTimerTick);
            FilePollTimer.Start();
        }

        /// <summary>
        /// Checks the setting value validity
        /// </summary>
        private static void CheckSettingValues()
        {
            Int32 interval = Globals.Settings.FilePollInterval;
            if (interval == 0) Globals.Settings.FilePollInterval = 3000;
        }

        /// <summary>
        /// Checks if a file has been changed outside
        /// </summary>
        private static void CheckFileChange(ITabbedDocument document)
        {
            TabbedDocument casted = document as TabbedDocument;
            if (casted.IsEditable && casted.CheckFileChange())
            {
                if (Globals.Settings.AutoReloadModifiedFiles)
                {
                    casted.RefreshFileInfo();
                    casted.Reload(false);
                }
                else
                {
                    if (YesToAll)
                    {
                        casted.RefreshFileInfo();
                        casted.Reload(false);
                        return;
                    }
                    String dlgTitle = TextHelper.GetString("Title.InfoDialog");
                    String dlgMessage = TextHelper.GetString("Info.FileIsModifiedOutside");
                    String formatted = String.Format(dlgMessage, "\n", casted.FileName);
                    MessageBoxManager.Cancel = TextHelper.GetString("Label.YesToAll");
                    MessageBoxManager.Register(); // Use custom labels...
                    DialogResult result = MessageBox.Show(Globals.MainForm, formatted, " " + dlgTitle, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);
                    casted.RefreshFileInfo(); // User may have waited before responding, save info now
                    if (result == DialogResult.Yes) casted.Reload(false);
                    else if (result == DialogResult.Cancel)
                    {
                        casted.Reload(false);
                        YesToAll = true;
                    }
                    MessageBoxManager.Unregister();
                }
            }
        }

        /// <summary>
        /// After an interval check if the files have changed
        /// </summary>
        private static void FilePollTimerTick(Object sender, EventArgs e)
        {
            try
            {
                FilePollTimer.Enabled = false;
                ITabbedDocument[] documents = Globals.MainForm.Documents;
                ITabbedDocument current = Globals.MainForm.CurrentDocument;
                CheckFileChange(current); // Check the current first..
                foreach (ITabbedDocument document in documents)
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
