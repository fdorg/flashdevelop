// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.IO;
using System.Collections.Generic;
using PluginCore.Utilities;
using ScintillaNet;
using PluginCore;
using ScintillaNet.Configuration;

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
            ScintillaControl sci = GetScintillaControl(value);
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
			Boolean bCurrentLine = IsMarkerSet(sender, markerCurrentLine, line);
			Boolean bBpActive = IsMarkerSet(sender, markerBPEnabled, line);
			Boolean bBpDisabled = IsMarkerSet(sender, markerBPDisabled, line);
			if (bCurrentLine)
			{
				ScintillaHelper.RemoveHighlight(sender, line, indicatorDebugDisabledBreakpoint);
				ScintillaHelper.RemoveHighlight(sender, line, indicatorDebugEnabledBreakpoint);
				ScintillaHelper.AddHighlight(sender, line, indicatorDebugCurrentLine, 1);
			}
			else if (bBpActive)
			{
				ScintillaHelper.RemoveHighlight(sender, line, indicatorDebugCurrentLine);
				ScintillaHelper.RemoveHighlight(sender, line, indicatorDebugDisabledBreakpoint);
				ScintillaHelper.AddHighlight(sender, line, indicatorDebugEnabledBreakpoint, 1);
			}
			else if (bBpDisabled)
			{
				ScintillaHelper.RemoveHighlight(sender, line, indicatorDebugCurrentLine);
				ScintillaHelper.RemoveHighlight(sender, line, indicatorDebugEnabledBreakpoint);
				ScintillaHelper.AddHighlight(sender, line, indicatorDebugDisabledBreakpoint, 1);
			}
			else
			{
				ScintillaHelper.RemoveHighlight(sender, line, indicatorDebugCurrentLine);
				ScintillaHelper.RemoveHighlight(sender, line, indicatorDebugDisabledBreakpoint);
				ScintillaHelper.RemoveHighlight(sender, line, indicatorDebugEnabledBreakpoint);
			}
            PluginMain.breakPointManager.SetBreakPointInfo(sender.FileName, line, !(bBpActive || bBpDisabled), bBpActive);
        }

        /// <summary>
        /// 
        /// </summary>
		static public void SciControl_MarginClick(ScintillaControl sender, int modifiers, int position, int margin)
		{
            if (margin != 0) return;
            if (PluginMain.debugManager.FlashInterface.isDebuggerStarted && !PluginMain.debugManager.FlashInterface.isDebuggerSuspended) return;
			int line = sender.LineFromPosition(position);
			if (IsMarkerSet(sender, markerBPEnabled, line))
			{
                sender.MarkerDelete(line, markerBPEnabled);
			}
			else sender.MarkerAdd(line, markerBPEnabled);
		}

        /// <summary>
        /// 
        /// </summary>
        static public void RemoveSciEvent(String value)
        {
            ScintillaControl sci = GetScintillaControl(Path.GetFileName(value));
			sci.ModEventMask |= (Int32)ScintillaNet.Enums.ModificationFlags.ChangeMarker;
            sci.MarkerChanged -= new MarkerChangedHandler(SciControl_MarkerChanged);
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

        #endregion

        #region Breakpoint Management

		static internal void RunToCursor_Click(Object sender, EventArgs e)
        {
            ScintillaControl sci = PluginBase.MainForm.CurrentDocument.SciControl;
            int line = sci.LineFromPosition(sci.CurrentPos);
			PluginMain.breakPointManager.SetTemporaryBreakPoint(PluginBase.MainForm.CurrentDocument.FileName, line);
			PluginMain.debugManager.Continue_Click(sender, e);
		}

		static internal void ToggleBreakPoint_Click(Object sender, EventArgs e)
        {
            ScintillaControl sci = PluginBase.MainForm.CurrentDocument.SciControl;
            int line = sci.LineFromPosition(sci.CurrentPos);
			ToggleMarker(sci, markerBPEnabled, line);
        }

        static internal void DeleteAllBreakPoints_Click(Object sender, EventArgs e)
        {
            foreach (ITabbedDocument doc in PluginBase.MainForm.Documents)
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
            Int32 line = sci.LineFromPosition(sci.CurrentPos);
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
