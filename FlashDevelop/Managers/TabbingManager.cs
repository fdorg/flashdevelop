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
        public static int SequentialIndex;

        static TabbingManager()
        {
            TabTimer = new Timer();
            TabTimer.Interval = 100;
            TabTimer.Tick += OnTabTimer;
            TabHistory = new List<ITabbedDocument>();
            SequentialIndex = 0;
        }

        /// <summary>
        /// Checks to see if the Control key has been released
        /// </summary>
        private static void OnTabTimer(object sender, EventArgs e)
        {
            if ((Control.ModifierKeys & Keys.Control) == 0)
            {
                TabTimer.Enabled = false;
                TabHistory.Remove(PluginBase.MainForm.CurrentDocument);
                TabHistory.Insert(0, PluginBase.MainForm.CurrentDocument);
            }
        }

        /// <summary>
        /// Sets an index of the current document
        /// </summary>
        public static void UpdateSequentialIndex(ITabbedDocument document)
        {
            var documents = PluginBase.MainForm.Documents;
            int count = documents.Length;
            for (int i = 0; i < count; i++)
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
        public static void NavigateTabsSequentially(int direction)
        {
            var current = PluginBase.MainForm.CurrentDocument;
            var documents = PluginBase.MainForm.Documents;
            var count = documents.Length;
            if (count <= 1) return;
            for (int i = 0; i < count; i++)
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
        public static void NavigateTabHistory(int direction)
        {
            int currentIndex = 0;
            if (TabHistory.Count == 0) return;
            if (direction != 0)
            {
                currentIndex = TabHistory.IndexOf(PluginBase.MainForm.CurrentDocument);
                currentIndex = (currentIndex + direction) % TabHistory.Count;
                if (currentIndex == -1) currentIndex = TabHistory.Count - 1;
            }
            TabHistory[currentIndex].Activate();
        }
    }
}