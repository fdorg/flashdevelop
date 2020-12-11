// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using PluginCore;

namespace FlashDevelop.Managers
{
    internal class TabbingManager
    {
        public static Timer TabTimer = new Timer {Interval = 100};
        public static List<ITabbedDocument> TabHistory = new List<ITabbedDocument>();
        public static int SequentialIndex;

        static TabbingManager() => TabTimer.Tick += OnTabTimer;

        /// <summary>
        /// Checks to see if the Control key has been released
        /// </summary>
        static void OnTabTimer(object sender, EventArgs e)
        {
            if ((Control.ModifierKeys & Keys.Control) != 0) return;
            TabTimer.Enabled = false;
            var document = PluginBase.MainForm.CurrentDocument;
            if (document is null) return;
            TabHistory.Remove(document);
            TabHistory.Insert(0, document);
        }

        /// <summary>
        /// Sets an index of the current document
        /// </summary>
        public static void UpdateSequentialIndex(ITabbedDocument document)
        {
            var documents = PluginBase.MainForm.Documents;
            var count = documents.Length;
            for (var i = 0; i < count; i++)
            {
                if (documents[i] == document)
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
            if (current is null) return;
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