// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.IO;
using FlashDebugger.Properties;
using PluginCore;
using PluginCore.Helpers;
using PluginCore.Managers;
using ScintillaNet;
using ScintillaNet.Configuration;
using ScintillaNet.Enums;

namespace FlashDebugger
{
    public class ScintillaHelper
    {
        public const int markerBPEnabled = 3;
        public const int markerBPDisabled = 4;
        public const int markerBPNotAvailable = 5;
        public const int markerCurrentLine = 6;
        public const int indicatorDebugEnabledBreakpoint = 28;
        public const int indicatorDebugDisabledBreakpoint = 29;
        public const int indicatorDebugCurrentLine = 30;
        const int BreakpointMargin = 0; //same as BookmarksMargin (see FlashDevelop.Managers.ScintillaManager)

        #region Scintilla Events

        public static void AddSciEvent(string fileName)
        {
            var document = DocumentManager.FindDocument(fileName);
            if (document is null || !document.IsEditable) return;
            InitMarkers(document.SplitSci1);
            InitMarkers(document.SplitSci2);
        }

        public static void InitMarkers(ScintillaControl sci)
        {
            sci.ModEventMask |= (int)ModificationFlags.ChangeMarker;
            sci.MarkerChanged += SciControl_MarkerChanged;
            sci.MarginSensitiveN(BreakpointMargin, true);
            int mask = sci.GetMarginMaskN(BreakpointMargin);
            mask |= GetMarkerMask(markerBPEnabled);
            mask |= GetMarkerMask(markerBPDisabled);
            mask |= GetMarkerMask(markerBPNotAvailable);
            mask |= GetMarkerMask(markerCurrentLine);
            sci.SetMarginMaskN(BreakpointMargin, mask);
            var enabledImage = ScaleHelper.Scale(Resource.Enabled);
            var disabledImage = ScaleHelper.Scale(Resource.Disabled);
            var curlineImage = ScaleHelper.Scale(Resource.CurLine);
            sci.MarkerDefineRGBAImage(markerBPEnabled, enabledImage);
            sci.MarkerDefineRGBAImage(markerBPDisabled, disabledImage);
            sci.MarkerDefineRGBAImage(markerCurrentLine, curlineImage);
            Language lang = PluginBase.MainForm.SciConfig.GetLanguage("as3"); // default
            sci.MarkerSetBack(markerBPEnabled, lang.editorstyle.ErrorLineBack); // enable
            sci.MarkerSetBack(markerBPDisabled, lang.editorstyle.DisabledLineBack); // disable
            sci.MarginClick += SciControl_MarginClick;
            sci.Modified += Sci_Modified;
        }

        public static void Sci_Modified(ScintillaControl sender, int position, int modificationType, string text, int length, int linesAdded, int line, int foldLevelNow, int foldLevelPrev)
        {
            if (linesAdded != 0)
            {
                int modline = sender.LineFromPosition(position);
                PluginMain.breakPointManager.UpdateBreakPoint(sender.FileName, modline, linesAdded);
            }
        }

        public static void SciControl_MarkerChanged(ScintillaControl sender, int line)
        {
            if (line < 0) return;
            var document = DocumentManager.FindDocument(sender);
            if (document is null || !document.IsEditable) return;
            ApplyHighlights(document.SplitSci1, line, true);
            ApplyHighlights(document.SplitSci2, line, false);
        }

        public static void ApplyHighlights(ScintillaControl sender, int line) => ApplyHighlights(sender, line, true);

        public static void ApplyHighlights(ScintillaControl sender, int line, bool notify)
        {
            bool bCurrentLine = IsMarkerSet(sender, markerCurrentLine, line);
            bool bBpActive = IsMarkerSet(sender, markerBPEnabled, line);
            bool bBpDisabled = IsMarkerSet(sender, markerBPDisabled, line);
            if (bCurrentLine)
            {
                RemoveHighlight(sender, line, indicatorDebugDisabledBreakpoint);
                RemoveHighlight(sender, line, indicatorDebugEnabledBreakpoint);
                AddHighlight(sender, line, indicatorDebugCurrentLine, 1);
            }
            else if (bBpActive)
            {
                RemoveHighlight(sender, line, indicatorDebugCurrentLine);
                RemoveHighlight(sender, line, indicatorDebugDisabledBreakpoint);
                AddHighlight(sender, line, indicatorDebugEnabledBreakpoint, 1);
            }
            else if (bBpDisabled)
            {
                RemoveHighlight(sender, line, indicatorDebugCurrentLine);
                RemoveHighlight(sender, line, indicatorDebugEnabledBreakpoint);
                AddHighlight(sender, line, indicatorDebugDisabledBreakpoint, 1);
            }
            else
            {
                RemoveHighlight(sender, line, indicatorDebugCurrentLine);
                RemoveHighlight(sender, line, indicatorDebugDisabledBreakpoint);
                RemoveHighlight(sender, line, indicatorDebugEnabledBreakpoint);
            }
            if (notify)
                PluginMain.breakPointManager.SetBreakPointInfo(sender.FileName, line, !(bBpActive || bBpDisabled), bBpActive);
        }

        public static void SciControl_MarginClick(ScintillaControl sender, int modifiers, int position, int margin)
        {
            if (margin != BreakpointMargin) return;
            //if (PluginMain.debugManager.FlashInterface.isDebuggerStarted && !PluginMain.debugManager.FlashInterface.isDebuggerSuspended) return;
            int line = sender.LineFromPosition(position);
            if (IsMarkerSet(sender, markerBPEnabled, line))
            {
                sender.MarkerDelete(line, markerBPEnabled);
            }
            else
            {
                if (IsMarkerSet(sender, markerBPDisabled, line)) sender.MarkerDelete(line, markerBPDisabled);
                sender.MarkerAdd(line, markerBPEnabled);
            }
        }

        public static void RemoveSciEvent(string value)
        {
            var document = DocumentManager.FindDocument(Path.GetFileName(value));
            if (document is null || !document.IsEditable) return;
            document.SplitSci1.ModEventMask |= (int)ModificationFlags.ChangeMarker;
            document.SplitSci1.MarkerChanged -= SciControl_MarkerChanged;
            document.SplitSci2.ModEventMask |= (int)ModificationFlags.ChangeMarker;
            document.SplitSci2.MarkerChanged -= SciControl_MarkerChanged;
        }

        #endregion

        #region Helper Methods

        public static void ToggleMarker(ScintillaControl sci, int marker, int line)
        {
            int lineMask = sci.MarkerGet(line);
            if ((lineMask & GetMarkerMask(marker)) == 0) sci.MarkerAdd(line, marker);
            else sci.MarkerDelete(line, marker);
        }

        public static bool IsBreakPointEnabled(ScintillaControl sci, int line) => IsMarkerSet(sci, markerBPEnabled, line);

        public static bool IsMarkerSet(ScintillaControl sci, int marker, int line) => (sci.MarkerGet(line) & GetMarkerMask(marker)) != 0;

        public static int GetMarkerMask(int marker) => 1 << marker;

        #endregion

        #region Highlighting

        public static void AddHighlight(ScintillaControl sci, int line, int indicator, int value)
        {
            if (sci is null) return;
            int start = sci.PositionFromLine(line);
            int length = sci.LineLength(line);
            if (start < 0 || length < 1) return;
            int es = sci.EndStyled;
            int mask = (1 << sci.StyleBits) - 1;
            Language lang = PluginBase.MainForm.SciConfig.GetLanguage(sci.ConfigurationLanguage);
            if (indicator == indicatorDebugCurrentLine)
            {
                sci.SetIndicFore(indicator, lang.editorstyle.DebugLineBack);
                sci.SetIndicSetAlpha(indicator, 40); // Improve contrast
            }
            else if (indicator == indicatorDebugEnabledBreakpoint)
            {
                sci.SetIndicFore(indicator, lang.editorstyle.ErrorLineBack);
                sci.SetIndicSetAlpha(indicator, 40); // Improve contrast
            }
            else if (indicator == indicatorDebugDisabledBreakpoint)
            {
                sci.SetIndicFore(indicator, lang.editorstyle.DisabledLineBack);
                sci.SetIndicSetAlpha(indicator, 40); // Improve contrast
            }
            sci.SetIndicStyle(indicator, 7);
            sci.CurrentIndicator = indicator;
            sci.IndicatorValue = value;
            sci.IndicatorFillRange(start, length);
            sci.StartStyling(es, mask);
        }

        public static void RemoveHighlight(ScintillaControl sci, int line, int indicator)
        {
            if (sci is null) return;
            int start = sci.PositionFromLine(line);
            int length = sci.LineLength(line);
            if (start < 0 || length < 1) return;
            int es = sci.EndStyled;
            int mask = (1 << sci.StyleBits) - 1;
            Language lang = PluginBase.MainForm.SciConfig.GetLanguage(sci.ConfigurationLanguage);
            if (indicator == indicatorDebugCurrentLine)
            {
                sci.SetIndicFore(indicator, lang.editorstyle.DebugLineBack);
                sci.SetIndicSetAlpha(indicator, 40); // Improve contrast
            }
            else if (indicator == indicatorDebugEnabledBreakpoint)
            {
                sci.SetIndicFore(indicator, lang.editorstyle.ErrorLineBack);
                sci.SetIndicSetAlpha(indicator, 40); // Improve contrast
            }
            else if (indicator == indicatorDebugDisabledBreakpoint)
            {
                sci.SetIndicFore(indicator, lang.editorstyle.DisabledLineBack);
                sci.SetIndicSetAlpha(indicator, 40); // Improve contrast
            }
            sci.SetIndicStyle(indicator, 7);
            sci.CurrentIndicator = indicator;
            sci.IndicatorClearRange(start, length);
            sci.StartStyling(es, mask);
        }

        public static void RemoveAllHighlights(ScintillaControl sci)
        {
            if (sci is null) return;
            int es = sci.EndStyled;
            int[] indics = { indicatorDebugCurrentLine, indicatorDebugDisabledBreakpoint, indicatorDebugEnabledBreakpoint };
            foreach (int indicator in indics)
            {
                sci.CurrentIndicator = indicator;
                for (int position = 0; position < sci.Length;)
                {
                    int start = sci.IndicatorStart(indicator, position);
                    int end = sci.IndicatorEnd(indicator, start);
                    int length = end - start;
                    if (length > 0)
                    {
                        sci.IndicatorClearRange(start, length);
                        position = start + length + 1;
                    }
                    else break;
                }
            }
            sci.StartStyling(es, (1 << sci.StyleBits) - 1);
        }

        #endregion

        #region Document Management

        public static ScintillaControl GetScintillaControl(string name)
        {
            foreach (var document in PluginBase.MainForm.Documents)
            {
                var sci = document.SciControl;
                if (sci != null && sci.FileName == name) return sci;
            }
            return null;
        }

        public static int GetScintillaControlIndex(ScintillaControl sci)
        {
            var documents = PluginBase.MainForm.Documents;
            for (int i = 0; i < documents.Length; i++)
            {
                if (documents[i].SciControl == sci) return i;
            }
            return -1;
        }

        public static ITabbedDocument GetDocument(string filefullpath)
        {
            var documents = PluginBase.MainForm.Documents;
            foreach (var document in documents)
            {
                var sci = document.SciControl;
                if (sci != null && sci.FileName == filefullpath) return document;
            }
            return null;
        }

        public static void ActivateDocument(string filefullpath) => ActivateDocument(filefullpath, -1, false);

        public static ScintillaControl ActivateDocument(string filefullpath, int line, bool bSelectLine)
        {
            var doc = PluginBase.MainForm.OpenEditableDocument(filefullpath, false) as ITabbedDocument;
            var sci = doc?.SciControl;
            if (sci is null || sci.FileName != filefullpath) return null;
            if (line >= 0)
            {
                sci.EnsureVisible(line);
                int start = sci.PositionFromLine(line);
                if (bSelectLine)
                {
                    int end = start + sci.LineLength(line);
                    sci.SetSel(start, end);
                }
                else sci.SetSel(start, start);
            }
            return sci;
        }

        #endregion

        #region Breakpoint Management

        internal static void RunToCursor_Click(object sender, EventArgs e)
        {
            var sci = PluginBase.MainForm.CurrentDocument.SciControl;
            PluginMain.breakPointManager.SetTemporaryBreakPoint(sci.FileName, sci.CurrentLine);
            PluginMain.debugManager.Continue_Click(sender, e);
        }

        internal static void ToggleBreakPoint_Click(object sender, EventArgs e)
        {
            var sci = PluginBase.MainForm.CurrentDocument.SciControl;
            ToggleMarker(sci, markerBPEnabled, sci.CurrentLine);
        }

        internal static void DeleteAllBreakPoints_Click(object sender, EventArgs e)
        {
            foreach (var doc in PluginBase.MainForm.Documents)
                if (doc.SciControl is { } sci)
                {
                    sci.MarkerDeleteAll(markerBPEnabled);
                    sci.MarkerDeleteAll(markerBPDisabled);
                    RemoveAllHighlights(sci);
                }
            PanelsHelper.breakPointUI.Clear();
            PluginMain.breakPointManager.ClearAll();
        }

        internal static void ToggleBreakPointEnable_Click(object sender, EventArgs e)
        {
            var sci = PluginBase.MainForm.CurrentDocument.SciControl;
            var line = sci.CurrentLine;
            if (IsMarkerSet(sci, markerBPEnabled, line))
            {
                sci.MarkerDelete(line, markerBPEnabled);
                sci.MarkerAdd(line, markerBPDisabled);
            }
            else if (IsMarkerSet(sci, markerBPDisabled, line))
            {
                sci.MarkerDelete(line, markerBPDisabled);
                sci.MarkerAdd(line, markerBPEnabled);
            }
        }

        internal static void DisableAllBreakPoints_Click(object sender, EventArgs e)
        {
            foreach (var doc in PluginBase.MainForm.Documents)
            {
                var list = PluginMain.breakPointManager.GetMarkers(doc.SciControl, markerBPEnabled);
                foreach (int line in list)
                {
                    doc.SciControl.MarkerDelete(line, markerBPEnabled);
                    doc.SciControl.MarkerAdd(line, markerBPDisabled);
                }
            }
        }

        internal static void EnableAllBreakPoints_Click(object sender, EventArgs e)
        {
            foreach (var doc in PluginBase.MainForm.Documents)
            {
                var list = PluginMain.breakPointManager.GetMarkers(doc.SciControl, markerBPDisabled);
                foreach (int line in list)
                {
                    doc.SciControl.MarkerDelete(line, markerBPDisabled);
                    doc.SciControl.MarkerAdd(line, markerBPEnabled);
                }
            }
        }
        
        #endregion
    }
}