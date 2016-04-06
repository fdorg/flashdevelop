using System;
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
        public static Image GetImage(Int32 img)
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
        public static Int32 GetMatchIndex(SearchMatch match, List<SearchMatch> matches)
        {
            for (Int32 i = 0; i < matches.Count; i++)
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
            Int32 start = sci.MBSafePosition(match.Index); // wchar to byte position
            Int32 end = start + sci.MBSafeTextLength(match.Value); // wchar to byte text length
            Int32 line = sci.LineFromPosition(start);
            sci.EnsureVisible(line);
            sci.SetSel(start, end);
        }

        /// <summary>
        /// Selects a search match in target
        /// </summary>
        public static void SelectMatchInTarget(ScintillaControl sci, SearchMatch match)
        {
            Int32 start = sci.MBSafePosition(match.Index); // wchar to byte position
            Int32 end = start + sci.MBSafeTextLength(match.Value); // wchar to byte text length
            Int32 line = sci.LineFromPosition(start);
            sci.EnsureVisible(line);
            sci.TargetStart = start;
            sci.TargetEnd = end;
        }

        /// <summary>
        /// Bookmarks a search match
        /// </summary>
        public static void BookmarkMatches(ScintillaControl sci, List<SearchMatch> matches)
        {
            for (Int32 i = 0; i < matches.Count; i++)
            {
                Int32 line = matches[i].Line - 1;
                sci.EnsureVisible(line);
                sci.MarkerAdd(line, 0);
            }
        }

        /// <summary>
        /// Filters the matches based on the start and end positions
        /// </summary>
        public static List<SearchMatch> FilterMatches(List<SearchMatch> matches, Int32 start, Int32 end)
        {
            List<SearchMatch> filtered = new List<SearchMatch>();
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
        public static SearchMatch GetNextDocumentMatch(ScintillaControl sci, List<SearchMatch> matches, Boolean forward, Boolean fixedPosition)
        {
            SearchMatch nearestMatch = matches[0];
            Int32 currentPosition = sci.MBSafeCharPosition(sci.CurrentPos);
            if (fixedPosition) currentPosition -= sci.MBSafeTextLength(sci.SelText);
            for (Int32 i = 0; i < matches.Count; i++)
            {
                if (forward)
                {
                    if (currentPosition > matches[matches.Count - 1].Index)
                    {
                        return matches[0];
                    }
                    if (matches[i].Index >= currentPosition)
                    {
                        return matches[i];
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
                    if (sci.SelText.Length == 0 && currentPosition == matches[i].Index + matches[i].Value.Length)
                    {
                        return matches[i];
                    }
                    if (matches[i].Index > nearestMatch.Index && matches[i].Index + matches[i].Value.Length < currentPosition)
                    {
                        nearestMatch = matches[i];
                    }
                }
            }
            return nearestMatch;
        }

    }

}
