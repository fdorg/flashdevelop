// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using PluginCore.Controls;
using ScintillaNet;

namespace FlashDevelop.Managers
{
    class MarkerManager
    {
        /// <summary>
        /// Mask value for all availables markers
        /// </summary>
        public static int MARKERS = 1;

        /// <summary>
        /// Gets the mask of the marker
        /// </summary>
        public static int GetMarkerMask(int marker) => 1 << marker;

        /// <summary>
        /// Adds or removes a marker
        /// </summary>
        public static void ToggleMarker(ScintillaControl sci, int marker, int line)
        {
            if (!HasMarker(sci, marker, line)) sci.MarkerAdd(line, marker);
            else sci.MarkerDelete(line, marker);
            UITools.Manager.MarkerChanged(sci, line);
        }

        /// <summary>
        /// Whether a certain line has a certain marker.
        /// </summary>
        public static bool HasMarker(ScintillaControl sci, int marker, int line) => (sci.MarkerGet(line) & GetMarkerMask(marker)) > 0;

        /// <summary>
        /// Moves the cursor to the next marker
        /// </summary>
        public static void NextMarker(ScintillaControl sci, int marker, int line)
        {
            int lineMask = sci.MarkerGet(line);
            if ((lineMask & GetMarkerMask(marker)) != 0)
            {
                int next = sci.MarkerNext(line + 1, GetMarkerMask(marker));
                if (next != -1)
                {
                    sci.EnsureVisibleEnforcePolicy(next);
                    sci.GotoLineIndent(next);
                }
                else
                {
                    next = sci.MarkerNext(0, GetMarkerMask(marker));
                    if (next != -1)
                    {
                        sci.EnsureVisibleEnforcePolicy(next);
                        sci.GotoLineIndent(next);
                    }
                }
            }
            else
            {
                int next = sci.MarkerNext(line, GetMarkerMask(marker));
                if (next != -1)
                {
                    sci.EnsureVisibleEnforcePolicy(next);
                    sci.GotoLineIndent(next);
                }
                else
                {
                    next = sci.MarkerNext(0, GetMarkerMask(marker));
                    if (next != -1)
                    {
                        sci.EnsureVisibleEnforcePolicy(next);
                        sci.GotoLineIndent(next);
                    }
                }
            }
        }

        /// <summary>
        /// Moves the cursor to the previous marker
        /// </summary>
        public static void PreviousMarker(ScintillaControl sci, int marker, int line)
        {
            int lineMask = sci.MarkerGet(line);
            if ((lineMask & GetMarkerMask(marker)) != 0)
            {
                int prev = sci.MarkerPrevious(line - 1, GetMarkerMask(marker));
                if (prev != -1)
                {
                    sci.EnsureVisibleEnforcePolicy(prev);
                    sci.GotoLineIndent(prev);
                }
                else
                {
                    int count = sci.LineCount;
                    prev = sci.MarkerPrevious(count, GetMarkerMask(marker));
                    if (prev != -1)
                    {
                        sci.EnsureVisibleEnforcePolicy(prev);
                        sci.GotoLineIndent(prev);
                    }
                }
            }
            else
            {
                int prev = sci.MarkerPrevious(line, GetMarkerMask(marker));
                if (prev != -1)
                {
                    sci.EnsureVisibleEnforcePolicy(prev);
                    sci.GotoLineIndent(prev);
                }
                else
                {
                    int count = sci.LineCount;
                    prev = sci.MarkerPrevious(count, GetMarkerMask(marker));
                    if (prev != -1)
                    {
                        sci.EnsureVisibleEnforcePolicy(prev);
                        sci.GotoLineIndent(prev);
                    }
                }
            }
        }
    }
}