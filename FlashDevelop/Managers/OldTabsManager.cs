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
            foreach (var tab in OldTabs)
            {
                if (DocumentManager.FindDocument(tab) is null)
                {
                    Globals.MainForm.OpenEditableDocument(tab);
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
