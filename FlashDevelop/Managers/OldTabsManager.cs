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
        public static List<string> OldTabs = new List<string>();
        
        /// <summary>
        /// Opens the last closed tabs if they are not open.
        /// </summary>
        public static void OpenOldTabDocument()
        {
            for (int i = 0; i < OldTabs.Count; i++)
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
        public static void SaveOldTabDocument(string filename)
        {
            if (!OldTabs.Contains(filename)) OldTabs.Insert(0, filename);
            while (OldTabs.Count > 10)
            {
                int last = OldTabs.Count - 1;
                OldTabs.RemoveAt(last);
            }
        }

    }

}
