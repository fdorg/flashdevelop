using System;
using System.Collections.Generic;
using PluginCore.Managers;

namespace FlashDevelop.Managers
{
    class OldTabsManager
    {
        /// <summary>
        /// List of closed tab documents.
        /// </summary>
        public static List<String> OldTabs = new List<String>();
        
        /// <summary>
        /// Opens the last closed tabs if they are not open.
        /// </summary>
        public static void OpenOldTabDocument()
        {
            for (Int32 i = 0; i < OldTabs.Count; i++)
            {
                if (DocumentManager.FindDocument(OldTabs[i]) == null)
                {
                    Globals.MainForm.OpenEditableDocument(OldTabs[i]);
                    break;
                }
            }
        }

        /// <summary>
        /// Saves the closed tab document if it's editable.
        /// </summary>
        public static void SaveOldTabDocument(String filename)
        {
            if (!OldTabs.Contains(filename)) OldTabs.Insert(0, filename);
            while (OldTabs.Count > 10)
            {
                Int32 last = OldTabs.Count - 1;
                if (last >= 0) OldTabs.RemoveAt(last);
            }
        }

    }

}
