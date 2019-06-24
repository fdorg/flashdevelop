using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using PluginCore.FRService;
using ScintillaNet;

namespace FlashDevelop.Utilities
{
    class FRDialogGenerics
    {
        /// <summary>
        /// Gets the dialog icon
        /// </summary>
        public static Image GetImage(int img)
        {
            Image image;
            if (img == 1) image = Globals.MainForm.FindImage("196", false);
            else if (img == 2) image = Globals.MainForm.FindImage("197", false);
            else image = Globals.MainForm.FindImage("229", false);
            return image;
        }

        /// <summary>
        /// Adds the value to ComboBox items
        /// </summary>
        public static void UpdateComboBoxItems(ComboBox comboBox)
        {
            if (!comboBox.Items.Contains(comboBox.Text))
            {
                comboBox.Items.Insert(0, comboBox.Text);
                comboBox.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Gets an index of the search match
        /// </summary>
        public static int GetMatchIndex(SearchMatch match, List<SearchMatch> matches)
        {
            for (int i = 0; i < matches.Count; i++)
            {
                if (match == matches[i]) return i + 1;
            }
            return -1;
        }

        /// <summary>
        /// Selects a search match
        /// </summary>
        public static void SelectMatch(ScintillaControl sci, SearchMatch match)
        {
            int start = sci.MBSafePosition(match.Index); // wchar to byte position
            int end = start + sci.MBSafeTextLength(match.Value); // wchar to byte text length
            int line = sci.LineFromPosition(start);
            sci.EnsureVisibleEnforcePolicy(line);
            sci.SetSel(start, end);
        }

        /// <summary>
        /// Selects a search match in target
        /// </summary>
        public static void SelectMatchInTarget(ScintillaControl sci, SearchMatch match)
        {
            int start = sci.MBSafePosition(match.Index); // wchar to byte position
            int end = start + sci.MBSafeTextLength(match.Value); // wchar to byte text length
            int line = sci.LineFromPosition(start);
            sci.EnsureVisible(line);
            sci.TargetStart = start;
            sci.TargetEnd = end;
        }

        /// <summary>
        /// Bookmarks a search match
        /// </summary>
        public static void BookmarkMatches(ScintillaControl sci, List<SearchMatch> matches)
        {
            foreach (var match in matches)
            {
                int line = match.Line - 1;
                sci.EnsureVisible(line);
                sci.MarkerAdd(line, 0);
            }
        }

        /// <summary>
        /// Filters the matches based on the start and end positions
        /// </summary>
        public static List<SearchMatch> FilterMatches(List<SearchMatch> matches, int start, int end)
        {
            var filtered = new List<SearchMatch>();
            foreach (SearchMatch match in matches)
            {
                if (match.Index >= start && (match.Index + match.Length) <= end)
                {
                    filtered.Add(match);
                }
            }
            return filtered;
        }

        /// <summary>
        /// Gets the next valid match but fixes position with selected text's length
        /// </summary>
        public static SearchMatch GetNextDocumentMatch(ScintillaControl sci, List<SearchMatch> matches, bool forward, bool fixedPosition)
        {
            SearchMatch nearestMatch = matches[0];
            int currentPosition = sci.MBSafeCharPosition(sci.CurrentPos);
            if (fixedPosition) currentPosition -= sci.MBSafeTextLength(sci.SelText);
            foreach (var match in matches)
            {
                if (forward)
                {
                    if (currentPosition > matches[matches.Count - 1].Index)
                    {
                        return matches[0];
                    }
                    if (match.Index >= currentPosition)
                    {
                        return match;
                    }
                }
                else
                {
                    if (sci.SelText.Length > 0 && currentPosition <= matches[0].Index + matches[0].Value.Length)
                    {
                        return matches[matches.Count - 1];
                    }
                    if (currentPosition < matches[0].Index + matches[0].Value.Length)
                    {
                        return matches[matches.Count - 1];
                    }
                    if (sci.SelText.Length == 0 && currentPosition == match.Index + match.Value.Length)
                    {
                        return match;
                    }
                    if (match.Index > nearestMatch.Index && match.Index + match.Value.Length < currentPosition)
                    {
                        nearestMatch = match;
                    }
                }
            }
            return nearestMatch;
        }

    }

}
