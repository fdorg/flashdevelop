// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PluginCore;
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
            return img switch
            {
                1 => PluginBase.MainForm.FindImage("196", false),
                2 => PluginBase.MainForm.FindImage("197", false),
                _ => PluginBase.MainForm.FindImage("229", false),
            };
        }

        /// <summary>
        /// Adds the value to ComboBox items
        /// </summary>
        public static void UpdateComboBoxItems(ComboBox comboBox)
        {
            if (comboBox.Items.Contains(comboBox.Text)) return;
            comboBox.Items.Insert(0, comboBox.Text);
            comboBox.SelectedIndex = 0;
        }

        /// <summary>
        /// Gets an index of the search match
        /// </summary>
        public static int GetMatchIndex(SearchMatch match, List<SearchMatch> matches)
        {
            for (var i = 0; i < matches.Count; i++)
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
            sci.SetSel(start, start);
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
            => matches.Where(match => match.Index >= start && (match.Index + match.Length) <= end).ToList();

        /// <summary>
        /// Gets the next valid match but fixes position with selected text's length
        /// </summary>
        public static SearchMatch GetNextDocumentMatch(ScintillaControl sci, List<SearchMatch> matches, bool forward, bool fixedPosition)
        {
            var nearestMatch = matches[0];
            var currentPosition = sci.MBSafeCharPosition(sci.CurrentPos);
            if (fixedPosition) currentPosition -= sci.MBSafeTextLength(sci.SelText);
            foreach (var match in matches)
            {
                if (forward)
                {
                    if (currentPosition > matches[matches.Count - 1].Index) return matches[0];
                    if (match.Index >= currentPosition) return match;
                }
                else
                {
                    var sciSelTextSize = sci.SelTextSize;
                    if (sciSelTextSize > 0 && currentPosition <= matches[0].Index + matches[0].Value.Length)
                    {
                        return matches[matches.Count - 1];
                    }
                    if (currentPosition < matches[0].Index + matches[0].Value.Length)
                    {
                        return matches[matches.Count - 1];
                    }
                    if (sciSelTextSize == 0 && currentPosition == match.Index + match.Value.Length)
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