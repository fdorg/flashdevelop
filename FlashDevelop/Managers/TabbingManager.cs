using System;
using System.Collections.Generic;
using System.Windows.Forms;
using PluginCore;

namespace FlashDevelop.Managers
{
    class TabbingManager
    {
        public static Timer TabTimer;
        public static List<ITabbedDocument> TabHistory;
        public static Int32 SequentialIndex;

        static TabbingManager()
        {
            TabTimer = new Timer();
            TabTimer.Interval = 100;
            TabTimer.Tick += new EventHandler(OnTabTimer);
            TabHistory = new List<ITabbedDocument>();
            SequentialIndex = 0;
        }

        /// <summary>
        /// Checks to see if the Control key has been released
        /// </summary>
        private static void OnTabTimer(Object sender, EventArgs e)
        {
            if ((Control.ModifierKeys & Keys.Control) == 0)
            {
                TabTimer.Enabled = false;
                TabHistory.Remove(Globals.MainForm.CurrentDocument);
                TabHistory.Insert(0, Globals.MainForm.CurrentDocument);
            }
        }

        /// <summary>
        /// Sets an index of the current document
        /// </summary>
        public static void UpdateSequentialIndex(ITabbedDocument document)
        {
            ITabbedDocument[] documents = Globals.MainForm.Documents;
            Int32 count = documents.Length;
            for (Int32 i = 0; i < count; i++)
            {
                if (document == documents[i])
                {
                    SequentialIndex = i;
                    return;
                }
            }
        }

        /// <summary>
        /// Activates next document in tabs
        /// </summary>
        public static void NavigateTabsSequentially(Int32 direction)
        {
            ITabbedDocument current = Globals.CurrentDocument;
            ITabbedDocument[] documents = Globals.MainForm.Documents;
            Int32 count = documents.Length; if (count <= 1) return;
            for (Int32 i = 0; i < count; i++)
            {
                if (documents[i] == current)
                {
                    if (direction > 0)
                    {
                        if (i < count - 1) documents[i + 1].Activate();
                        else documents[0].Activate();
                    }
                    else if (direction < 0)
                    {
                        if (i > 0) documents[i - 1].Activate();
                        else documents[count - 1].Activate();
                    }
                }
            }
        }

        /// <summary>
        /// Visual Studio style keyboard tab navigation: similar to Alt-Tab
        /// </summary>
        public static void NavigateTabHistory(Int32 direction)
        {
            Int32 currentIndex = 0;
            if (TabHistory.Count < 1) return;
            if (direction != 0)
            {
                currentIndex = TabHistory.IndexOf(Globals.MainForm.CurrentDocument);
                currentIndex = (currentIndex + direction) % TabHistory.Count;
                if (currentIndex == -1) currentIndex = TabHistory.Count - 1;
            }
            TabHistory[currentIndex].Activate();
        }

    }

}
