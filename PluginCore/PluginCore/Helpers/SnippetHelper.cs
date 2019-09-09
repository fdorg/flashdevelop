// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System.Collections.Generic;
using PluginCore.Utilities;
using ScintillaNet;

namespace PluginCore.Helpers
{
    public class SnippetHelper
    {
        public const string BOUNDARY = "$(Boundary)";
        public const string ENTRYPOINT = "$(EntryPoint)";
        public const string EXITPOINT = "$(ExitPoint)";

        /// <summary>
        /// Processes the snippet and template arguments
        /// </summary>
        public static int PostProcessSnippets(ScintillaControl sci, int currentPosition)
        {
            var delta = 0;
            while (sci.SelectText(BOUNDARY, 0) != -1)
            {
                sci.ReplaceSel("");
                delta -= BOUNDARY.Length;
            }
            if (!TryProcessEntryExitPoints(sci, 0, ref delta))
                sci.SetSel(currentPosition, currentPosition);
            return delta;
        }

        static bool TryProcessEntryExitPoints(ScintillaControl sci, int startPosition, ref int delta)
        {
            var startSelection = sci.SelectText(ENTRYPOINT, startPosition);
            if (startSelection == -1) return false;
            var positions = new List<int>();
            while (startSelection != -1)
            {
                positions.Add(sci.MBSafePosition(startSelection));
                sci.ReplaceSel(string.Empty);
                delta -= ENTRYPOINT.Length;
                var endSelection = startSelection;
                startSelection = sci.SelectText(EXITPOINT, startPosition);
                if (startSelection != -1)
                {
                    sci.ReplaceSel(string.Empty);
                    delta -= EXITPOINT.Length;
                    endSelection = startSelection;
                }
                positions.Add(sci.MBSafePosition(endSelection));
                startPosition = endSelection;
                startSelection = sci.SelectText(ENTRYPOINT, startPosition);
            }
            sci.SetSelection(positions[0], positions[1]);
            for (var i = 2; i < positions.Count; i += 2)
            {
                sci.AddSelection(positions[i], positions[i + 1]);
            }
            return true;
        }

        /// <summary>
        /// Processes the text and returns correct action point
        /// </summary>
        public static ActionPoint ProcessActionPoint(string text)
        {
            text = text.Trim().Replace(BOUNDARY, "");
            int entryPosition = text.IndexOfOrdinal(ENTRYPOINT);
            int exitPosition = text.IndexOfOrdinal(EXITPOINT);
            if (entryPosition != -1 && exitPosition != -1)
            {
                string cleaned = text.Replace(ENTRYPOINT, "").Replace(EXITPOINT, "");
                return new ActionPoint(cleaned, entryPosition, exitPosition - ENTRYPOINT.Length);
            }
            if (entryPosition != -1 && exitPosition == -1)
            {
                string cleaned = text.Replace(ENTRYPOINT, "");
                return new ActionPoint(cleaned, entryPosition, -1);
            }
            return new ActionPoint(text, -1, -1);
        }

        /// <summary>
        /// Selects the text specified in the action point
        /// </summary>
        public static void ExecuteActionPoint(ActionPoint point, ScintillaControl sci)
        {
            if (point.EntryPosition != -1 && point.ExitPosition != -1)
            {
                int start = sci.MBSafePosition(point.EntryPosition);
                int end = sci.MBSafePosition(point.ExitPosition);
                sci.SetSel(start, end);
            }
            else if (point.EntryPosition != -1 && point.ExitPosition == -1)
            {
                int start = sci.MBSafePosition(point.EntryPosition);
                sci.SetSel(start, start);
            }
        }

        /// <summary>
        /// Inserts the specified snippet to the document
        /// </summary>
        public static int InsertSnippetText(ScintillaControl sci, int currentPosition, string snippet)
        {
            sci.BeginUndoAction();
            try
            {
                string text = snippet;
                if (sci.SelTextSize > 0)
                    currentPosition -= sci.MBSafeTextLength(sci.SelText);
                int line = sci.LineFromPosition(currentPosition);
                int indent = sci.GetLineIndentation(line);
                sci.ReplaceSel("");
                
                int lineMarker = LineEndDetector.DetectNewLineMarker(text, sci.EOLMode);
                string newline = LineEndDetector.GetNewLineMarker(lineMarker);
                if (newline != "\n") text = text.Replace(newline, "\n");
                newline = LineEndDetector.GetNewLineMarker((int)PluginBase.MainForm.Settings.EOLMode);
                text = PluginBase.MainForm.ProcessArgString(text).Replace(newline, "\n");
                newline = LineEndDetector.GetNewLineMarker(sci.EOLMode);
                string[] splitted = text.Trim().Split('\n');
                for (int j = 0; j < splitted.Length; j++)
                {
                    if (j != splitted.Length - 1) sci.InsertText(sci.CurrentPos, splitted[j] + newline);
                    else sci.InsertText(sci.CurrentPos, splitted[j]);
                    sci.CurrentPos += sci.MBSafeTextLength(splitted[j]) + newline.Length;
                    if (j > 0)
                    {
                        line = sci.LineFromPosition(sci.CurrentPos - newline.Length);
                        var newIndent = sci.GetLineIndentation(line) + indent;
                        sci.SetLineIndentation(line, newIndent);
                    }
                }
                int length = sci.CurrentPos - currentPosition - newline.Length;
                int delta = PostProcessSnippets(sci, currentPosition);
                return length + delta;
            }
            finally
            {
                sci.EndUndoAction();
            }
        }
    }

    public class ActionPoint
    {
        public string Text;
        public int EntryPosition;
        public int ExitPosition;

        public ActionPoint(string text, int entryPosition, int exitPosition)
        {
            Text = text;
            EntryPosition = entryPosition;
            ExitPosition = exitPosition;
        }

    }

}
