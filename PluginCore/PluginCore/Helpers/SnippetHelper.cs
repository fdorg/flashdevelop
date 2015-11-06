using System;
using PluginCore.Utilities;
using ScintillaNet;

namespace PluginCore.Helpers
{
    public class SnippetHelper
    {
        public const String BOUNDARY = "$(Boundary)";
        public const String ENTRYPOINT = "$(EntryPoint)";
        public const String EXITPOINT = "$(ExitPoint)";

        /// <summary>
        /// Processes the snippet and template arguments
        /// </summary>
        public static Int32 PostProcessSnippets(ScintillaControl sci, Int32 currentPosition)
        {
            Int32 delta = 0;
            while (sci.SelectText(BOUNDARY, 0) != -1) { sci.ReplaceSel(""); delta -= BOUNDARY.Length; }
            String text = sci.Text; // Store text temporarily
            Int32 entryPosition = sci.MBSafePosition(text.IndexOf(ENTRYPOINT));
            Int32 exitPosition = sci.MBSafePosition(text.IndexOf(EXITPOINT));
            if (entryPosition != -1 && exitPosition != -1)
            {
                sci.SelectText(ENTRYPOINT, 0); sci.ReplaceSel(""); delta -= ENTRYPOINT.Length;
                sci.SelectText(EXITPOINT, 0); sci.ReplaceSel(""); delta -= EXITPOINT.Length;
                sci.SetSel(entryPosition, exitPosition - ENTRYPOINT.Length);
            }
            else if (entryPosition != -1 && exitPosition == -1)
            {
                sci.SelectText(ENTRYPOINT, 0); sci.ReplaceSel(""); delta -= ENTRYPOINT.Length;
                sci.SetSel(entryPosition, entryPosition);
            }
            else sci.SetSel(currentPosition, currentPosition);
            return delta;
        }

        /// <summary>
        /// Processes the text and returns correct action point
        /// </summary>
        public static ActionPoint ProcessActionPoint(String text)
        {
            text = text.Trim().Replace(BOUNDARY, "");
            Int32 entryPosition = text.IndexOf(ENTRYPOINT);
            Int32 exitPosition = text.IndexOf(EXITPOINT);
            if (entryPosition != -1 && exitPosition != -1)
            {
                String cleaned = text.Replace(ENTRYPOINT, "").Replace(EXITPOINT, "");
                return new ActionPoint(cleaned, entryPosition, exitPosition - ENTRYPOINT.Length);
            }
            else if (entryPosition != -1 && exitPosition == -1)
            {
                String cleaned = text.Replace(ENTRYPOINT, "");
                return new ActionPoint(cleaned, entryPosition, -1);
            }
            else return new ActionPoint(text, -1, -1);
        }

        /// <summary>
        /// Selects the text specified in the action point
        /// </summary>
        public static void ExecuteActionPoint(ActionPoint point, ScintillaControl sci)
        {
            if (point.EntryPosition != -1 && point.ExitPosition != -1)
            {
                Int32 start = sci.MBSafePosition(point.EntryPosition);
                Int32 end = sci.MBSafePosition(point.ExitPosition);
                sci.SetSel(start, end);
            }
            else if (point.EntryPosition != -1 && point.ExitPosition == -1)
            {
                Int32 start = sci.MBSafePosition(point.EntryPosition);
                sci.SetSel(start, start);
            }
        }

        /// <summary>
        /// Inserts the specified snippet to the document
        /// </summary>
        public static Int32 InsertSnippetText(ScintillaControl sci, Int32 currentPosition, String snippet)
        {
            sci.BeginUndoAction();
            try
            {
                Int32 newIndent; 
                String text = snippet;
                if (sci.SelTextSize > 0)
                    currentPosition -= sci.MBSafeTextLength(sci.SelText);
                Int32 line = sci.LineFromPosition(currentPosition);
                Int32 indent = sci.GetLineIndentation(line);
                sci.ReplaceSel("");
                
                Int32 lineMarker = LineEndDetector.DetectNewLineMarker(text, sci.EOLMode);
                String newline = LineEndDetector.GetNewLineMarker(lineMarker);
                if (newline != "\n") text = text.Replace(newline, "\n");
                newline = LineEndDetector.GetNewLineMarker((Int32)PluginBase.MainForm.Settings.EOLMode);
                text = PluginBase.MainForm.ProcessArgString(text).Replace(newline, "\n");
                newline = LineEndDetector.GetNewLineMarker(sci.EOLMode);
                String[] splitted = text.Trim().Split('\n');
                for (Int32 j = 0; j < splitted.Length; j++)
                {
                    if (j != splitted.Length - 1) sci.InsertText(sci.CurrentPos, splitted[j] + newline);
                    else sci.InsertText(sci.CurrentPos, splitted[j]);
                    sci.CurrentPos += sci.MBSafeTextLength(splitted[j]) + newline.Length;
                    if (j > 0)
                    {
                        line = sci.LineFromPosition(sci.CurrentPos - newline.Length);
                        newIndent = sci.GetLineIndentation(line) + indent;
                        sci.SetLineIndentation(line, newIndent);
                    }
                }
                Int32 length = sci.CurrentPos - currentPosition - newline.Length;
                Int32 delta = PostProcessSnippets(sci, currentPosition);
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
        public String Text;
        public Int32 EntryPosition;
        public Int32 ExitPosition;

        public ActionPoint(String text, Int32 entryPosition, Int32 exitPosition)
        {
            this.Text = text;
            this.EntryPosition = entryPosition;
            this.ExitPosition = exitPosition;
        }

    }

}
