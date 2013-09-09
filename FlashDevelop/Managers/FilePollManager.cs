using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Collections.Generic;
using PluginCore.Localization;
using FlashDevelop.Docking;
using PluginCore.Managers;
using PluginCore;

namespace FlashDevelop.Managers
{
    class FilePollManager
    {
        private static Timer FilePollTimer;

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
                if (Globals.Settings.AutoReloadModifiedFiles) casted.Reload(false);
                else
                {
                    String dlgTitle = TextHelper.GetString("Title.InfoDialog");
                    String dlgMessage = TextHelper.GetString("Info.FileIsModifiedOutside");
                    String formatted = String.Format(dlgMessage, "\n", casted.FileName);
                    if (MessageBox.Show(Globals.MainForm, formatted, " " + dlgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                    {
                        casted.Reload(false);
                    }
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
            }
            catch { /* No errors shown here.. */ }
        }

    }

}
