using System;
using System.IO;
using System.Collections.Generic;
using PluginCore.Utilities;
using ScintillaNet;
using PluginCore;
using ScintillaNet.Configuration;
using PluginCore.Managers;
using PluginCore.Helpers;

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

        #region Scintilla Events

        /// <summary>
        /// 
        /// </summary>
        static public void AddSciEvent(String value)
        {
            ITabbedDocument document = DocumentManager.FindDocument(value);
            if (document != null && document.IsEditable)
            {
                InitMarkers(document.SplitSci1);
                InitMarkers(document.SplitSci2);
            }
        }

        static public void InitMarkers(ScintillaControl sci)
        {
            sci.ModEventMask |= (Int32)ScintillaNet.Enums.ModificationFlags.ChangeMarker;
            sci.MarkerChanged += new MarkerChangedHandler(SciControl_MarkerChanged);
            sci.MarginSensitiveN(0, true);
            int mask = sci.GetMarginMaskN(0);
            mask |= GetMarkerMask(markerBPEnabled);
            mask |= GetMarkerMask(markerBPDisabled);
            mask |= GetMarkerMask(markerBPNotAvailable);
            mask |= GetMarkerMask(markerCurrentLine);
            sci.SetMarginMaskN(0, mask);
            sci.MarkerDefinePixmap(markerBPEnabled, ScintillaNet.XPM.ConvertToXPM(Properties.Resource.Enabled, "#00FF00"));
            sci.MarkerDefinePixmap(markerBPDisabled, ScintillaNet.XPM.ConvertToXPM(Properties.Resource.Disabled, "#00FF00"));
            sci.MarkerDefinePixmap(markerCurrentLine, ScintillaNet.XPM.ConvertToXPM(Properties.Resource.CurLine, "#00FF00"));
            Language lang = PluginBase.MainForm.SciConfig.GetLanguage("as3"); // default
            sci.MarkerSetBack(markerBPEnabled, lang.editorstyle.ErrorLineBack); // enable
            sci.MarkerSetBack(markerBPDisabled, lang.editorstyle.DisabledLineBack); // disable
            sci.MarginClick += new MarginClickHandler(SciControl_MarginClick);
            sci.Modified += new ModifiedHandler(sci_Modified);
        }

        static public void sci_Modified(ScintillaControl sender, int position, int modificationType, string text, int length, int linesAdded, int line, int foldLevelNow, int foldLevelPrev)
        {
            if (linesAdded != 0)
            {
                int modline = sender.LineFromPosition(position);
                PluginMain.breakPointManager.UpdateBreakPoint(sender.FileName, modline, linesAdded);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        static public void SciControl_MarkerChanged(ScintillaControl sender, Int32 line)
        {
            if (line < 0) return;
            ITabbedDocument document = DocumentManager.FindDocument(sender);
            if (document == null || !document.IsEditable) return;
            ApplyHighlights(document.SplitSci1, line, true);
            ApplyHighlights(document.SplitSci2, line, false);
        }

        /// <summary>
        /// 
        /// </summary>
        public static void ApplyHighlights(ScintillaControl sender, Int32 line)
        {
            ApplyHighlights(sender, line, true);
        }

        /// <summary>
        /// 
        /// </summary>
        static public void ApplyHighlights(ScintillaControl sender, Int32 line, Boolean notify)
        {
            Boolean bCurrentLine = IsMarkerSet(sender, markerCurrentLine, line);
            Boolean bBpActive = IsMarkerSet(sender, markerBPEnabled, line);
            Boolean bBpDisabled = IsMarkerSet(sender, markerBPDisabled, line);
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

        /// <summary>
        /// 
        /// </summary>
        static public void SciControl_MarginClick(ScintillaControl sender, int modifiers, int position, int margin)
        {
            if (margin != 0) return;
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

        /// <summary>
        /// 
        /// </summary>
        static public void RemoveSciEvent(String value)
        {
            ITabbedDocument document = DocumentManager.FindDocument(Path.GetFileName(value));
            if (document != null && document.IsEditable)
            {
                document.SplitSci1.ModEventMask |= (Int32)ScintillaNet.Enums.ModificationFlags.ChangeMarker;
                document.SplitSci1.MarkerChanged -= new MarkerChangedHandler(SciControl_MarkerChanged);
                document.SplitSci2.ModEventMask |= (Int32)ScintillaNet.Enums.ModificationFlags.ChangeMarker;
                document.SplitSci2.MarkerChanged -= new MarkerChangedHandler(SciControl_MarkerChanged);
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// 
        /// </summary>
        static public void ToggleMarker(ScintillaControl sci, Int32 marker, Int32 line)
        {
            Int32 lineMask = sci.MarkerGet(line);
            if ((lineMask & GetMarkerMask(marker)) == 0) sci.MarkerAdd(line, marker);
            else sci.MarkerDelete(line, marker);
        }

        static public Boolean IsBreakPointEnabled(ScintillaControl sci, Int32 line)
        {
            return IsMarkerSet(sci, markerBPEnabled, line);
        }

        static public Boolean IsMarkerSet(ScintillaControl sci, Int32 marker, Int32 line)
        {
            return (sci.MarkerGet(line) & GetMarkerMask(marker)) != 0;
        }

        /// <summary>
        /// 
        /// </summary>
        static public Int32 GetMarkerMask(Int32 marker)
        {
            return 1 << marker;
        }

        #endregion

        #region Highlighting

        /// <summary>
        /// 
        /// </summary>
        static public void AddHighlight(ScintillaControl sci, Int32 line, Int32 indicator, Int32 value)
        {
            Int32 start = sci.PositionFromLine(line);
            Int32 length = sci.LineLength(line);
            if (start < 0 || length < 1)
            {
                return;
            }
            // Remember previous EndStyled marker and restore it when we are done.
            Int32 es = sci.EndStyled;
            // Mask for style bits used for restore.
            Int32 mask = (1 << sci.StyleBits) - 1;
            Language lang = PluginBase.MainForm.SciConfig.GetLanguage(sci.ConfigurationLanguage);
            if (indicator == indicatorDebugCurrentLine)
            {
                sci.SetIndicFore(indicator, lang.editorstyle.DebugLineBack);
            }
            else if (indicator == indicatorDebugEnabledBreakpoint)
            {
                sci.SetIndicFore(indicator, lang.editorstyle.ErrorLineBack);
            }
            else if (indicator == indicatorDebugDisabledBreakpoint)
            {
                sci.SetIndicFore(indicator, lang.editorstyle.DisabledLineBack);
            }
            sci.SetIndicStyle(indicator, 7);
            sci.CurrentIndicator = indicator;
            sci.IndicatorValue = value;
            sci.IndicatorFillRange(start, length);
            sci.StartStyling(es, mask);
        }

        /// <summary>
        /// 
        /// </summary>
        static public void RemoveHighlight(ScintillaControl sci, Int32 line, Int32 indicator)
        {
            if (sci == null) return;
            Int32 start = sci.PositionFromLine(line);
            Int32 length = sci.LineLength(line);
            if (start < 0 || length < 1)
            {
                return;
            }
            // Remember previous EndStyled marker and restore it when we are done.
            Int32 es = sci.EndStyled;
            // Mask for style bits used for restore.
            Int32 mask = (1 << sci.StyleBits) - 1;
            Language lang = PluginBase.MainForm.SciConfig.GetLanguage(sci.ConfigurationLanguage);
            if (indicator == indicatorDebugCurrentLine)
            {
                sci.SetIndicFore(indicator, lang.editorstyle.DebugLineBack);
            }
            else if (indicator == indicatorDebugEnabledBreakpoint)
            {
                sci.SetIndicFore(indicator, lang.editorstyle.ErrorLineBack);
            }
            else if (indicator == indicatorDebugDisabledBreakpoint)
            {
                sci.SetIndicFore(indicator, lang.editorstyle.DisabledLineBack);
            }
            sci.SetIndicStyle(indicator, 7);
            sci.CurrentIndicator = indicator;
            sci.IndicatorClearRange(start, length);
            sci.StartStyling(es, mask);
        }

        /// <summary>
        /// 
        /// </summary>
        static public void RemoveAllHighlights(ScintillaControl sci)
        {
            if (sci == null) return;
            Int32 es = sci.EndStyled;
            foreach (Int32 indicator in new Int32[] { indicatorDebugCurrentLine, indicatorDebugDisabledBreakpoint, indicatorDebugEnabledBreakpoint })
            {
                sci.CurrentIndicator = indicator;
                for (int position = 0; position < sci.Length; )
                {
                    Int32 start = sci.IndicatorStart(indicator, position);
                    Int32 end = sci.IndicatorEnd(indicator, start);
                    Int32 length = end - start;
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

        /// <summary>
        /// 
        /// </summary>
        static public ScintillaControl GetScintillaControl(string name)
        {
            ITabbedDocument[] documents = PluginBase.MainForm.Documents;
            foreach (ITabbedDocument docment in documents)
            {
                ScintillaControl sci = docment.SciControl;
                if (sci != null && name == sci.FileName) return sci;
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        static public Int32 GetScintillaControlIndex(ScintillaControl sci)
        {
            ITabbedDocument[] documents = PluginBase.MainForm.Documents;
            for (Int32 i = 0; i < documents.Length; i++)
            {
                if (documents[i].SciControl == sci) return i;
            }
            return -1;
        }

        static public ITabbedDocument GetDocument(string filefullpath)
        {
            ITabbedDocument[] documents = PluginBase.MainForm.Documents;
            foreach (ITabbedDocument document in documents)
            {
                ScintillaControl sci = document.SciControl;
                if (sci != null && filefullpath == sci.FileName) return document;
            }
            return null;
        }

        static public void ActivateDocument(string filefullpath)
        {
            ActivateDocument(filefullpath, -1, false);
        }

        static public void ActivateDocument(string filefullpath, int line, Boolean bSelectLine)
        {
            ScintillaControl sci = GetScintillaControl(filefullpath);
            if (sci == null)
            {
                PluginBase.MainForm.OpenEditableDocument(filefullpath);
                sci = GetScintillaControl(filefullpath);
            }
            Int32 i = GetScintillaControlIndex(sci);
            if (i != -1)
            {
                PluginBase.MainForm.Documents[i].Activate();
                if (line >= 0)
                {
                    sci.GotoLine(line);
                    if (bSelectLine)
                    {
                        Int32 start = sci.PositionFromLine(line);
                        Int32 end = start + sci.LineLength(line);
                        sci.SelectionStart = start;
                        sci.SelectionEnd = end;
                    }
                }
            }
        }

        #endregion

        #region Breakpoint Management

        static internal void RunToCursor_Click(Object sender, EventArgs e)
        {
            ScintillaControl sci = PluginBase.MainForm.CurrentDocument.SciControl;
            PluginMain.breakPointManager.SetTemporaryBreakPoint(PluginBase.MainForm.CurrentDocument.FileName, sci.CurrentLine);
            PluginMain.debugManager.Continue_Click(sender, e);
        }

        static internal void ToggleBreakPoint_Click(Object sender, EventArgs e)
        {
            ScintillaControl sci = PluginBase.MainForm.CurrentDocument.SciControl;
            ToggleMarker(sci, markerBPEnabled, sci.CurrentLine);
        }

        static internal void DeleteAllBreakPoints_Click(Object sender, EventArgs e)
        {
            foreach (ITabbedDocument doc in PluginBase.MainForm.Documents)
                if (doc.IsEditable)
                {
                    doc.SciControl.MarkerDeleteAll(markerBPEnabled);
                    doc.SciControl.MarkerDeleteAll(markerBPDisabled);
                    RemoveAllHighlights(doc.SciControl);
                }
            PanelsHelper.breakPointUI.Clear();
            PluginMain.breakPointManager.ClearAll();
        }

        static internal void ToggleBreakPointEnable_Click(Object sender, EventArgs e)
        {
            ScintillaControl sci = PluginBase.MainForm.CurrentDocument.SciControl;
            Int32 line = sci.CurrentLine;
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

        static internal void DisableAllBreakPoints_Click(Object sender, EventArgs e)
        {
            foreach (ITabbedDocument doc in PluginBase.MainForm.Documents)
            {
                List<int> list = PluginMain.breakPointManager.GetMarkers(doc.SciControl, markerBPEnabled);
                foreach (int line in list)
                {
                    doc.SciControl.MarkerDelete(line, markerBPEnabled);
                    doc.SciControl.MarkerAdd(line, markerBPDisabled);
                }
            }
        }

        static internal void EnableAllBreakPoints_Click(Object sender, EventArgs e)
        {
            foreach (ITabbedDocument doc in PluginBase.MainForm.Documents)
            {
                List<int> list = PluginMain.breakPointManager.GetMarkers(doc.SciControl, markerBPDisabled);
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
