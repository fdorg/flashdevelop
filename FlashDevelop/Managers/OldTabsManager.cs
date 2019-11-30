using System;
using System.Collections.Generic;
using PluginCore;
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
            foreach (var fileName in OldTabs)
            {
                if (DocumentManager.FindDocument(fileName) is null)
                {
                    PluginBase.MainForm.OpenEditableDocument(fileName);
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
            if (OldTabs.Count - 10 is var count && count > 0) OldTabs.RemoveRange(10, count);
        }
    }
}