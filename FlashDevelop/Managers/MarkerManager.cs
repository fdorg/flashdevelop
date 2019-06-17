using System;
using PluginCore.Controls;
using ScintillaNet;

namespace FlashDevelop.Managers
{
    class MarkerManager
    {
        /// <summary>
        /// Mask value for all availables markers
        /// </summary>
        public static int MARKERS = 1 << 0;

        /// <summary>
        /// Gets the mask of the marker
        /// </summary>
        public static int GetMarkerMask(int marker)
        {
            return 1 << marker;
        }

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
        public static bool HasMarker(ScintillaControl sci, int marker, int line)
        {
            int lineMask = sci.MarkerGet(line);
            return (lineMask & GetMarkerMask(marker)) > 0;
        }

        /// <summary>
        /// Moves the cursor to the next marker
        /// </summary>
        public static void NextMarker(ScintillaControl sci, int marker, int line)
        {
            int next = 0;
            int lineMask = sci.MarkerGet(line);
            if ((lineMask & GetMarkerMask(marker)) != 0)
            {
                next = sci.MarkerNext(line + 1, GetMarkerMask(marker));
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
                next = sci.MarkerNext(line, GetMarkerMask(marker));
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
            int prev = 0; int count = 0;
            int lineMask = sci.MarkerGet(line);
            if ((lineMask & GetMarkerMask(marker)) != 0)
            {
                prev = sci.MarkerPrevious(line - 1, GetMarkerMask(marker));
                if (prev != -1)
                {
                    sci.EnsureVisibleEnforcePolicy(prev);
                    sci.GotoLineIndent(prev);
                }
                else
                {
                    count = sci.LineCount;
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
                prev = sci.MarkerPrevious(line, GetMarkerMask(marker));
                if (prev != -1)
                {
                    sci.EnsureVisibleEnforcePolicy(prev);
                    sci.GotoLineIndent(prev);
                }
                else
                {
                    count = sci.LineCount;
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
