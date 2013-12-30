﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml.Serialization;
using flash.tools.debugger;
using flash.tools.debugger.expression;
using PluginCore.Helpers;
using PluginCore.Utilities;
using PluginCore.Managers;
using ScintillaNet;
using PluginCore;

namespace FlashDebugger
{
    public delegate void ConditionErrorEventHandler(Object sender, BreakPointArgs e);
    public delegate void ChangeBreakPointEventHandler(Object sender, BreakPointArgs e);
    public delegate void UpdateBreakPointEventHandler(Object sender, UpdateBreakPointArgs e);

    public class BreakPointManager
    {
        private IProject m_Project;
        private string m_SaveFileFullPath;
        private Boolean m_bAccessable = true;
        private BreakPointInfo m_TemporaryBreakPointInfo = null;
        private List<BreakPointInfo> m_BreakPointList = new List<BreakPointInfo>();
   
        public event ChangeBreakPointEventHandler ChangeBreakPointEvent = null;
        public event UpdateBreakPointEventHandler UpdateBreakPointEvent = null;

		public List<BreakPointInfo> BreakPoints
		{
			get { return m_BreakPointList; }
		}

		public IProject Project
        {
            get { return m_Project; }
            set 
            {
                if (value != null)
                {
					m_Project = value;
                    this.ClearAll();
					m_SaveFileFullPath = GetBreakpointsFile(m_Project.ProjectPath);
                }
            }
        }

        private string GetBreakpointsFile(string path)
        {
            String dataDir = Path.Combine(PathHelper.DataDir, "FlashDebugger");
            String cacheDir = Path.Combine(dataDir, "Breakpoints");
            if (!Directory.Exists(cacheDir)) Directory.CreateDirectory(cacheDir);
            String hashFileName = HashCalculator.CalculateSHA1(path);
            return Path.Combine(cacheDir, hashFileName + ".xml");
        }

        public void InitBreakPoints()
        {
            foreach (PluginCore.ITabbedDocument doc in PluginBase.MainForm.Documents)
            {
                if (Path.GetExtension(doc.SciControl.FileName) == ".as" || Path.GetExtension(doc.SciControl.FileName) == ".mxml")
                {
					List<int> lines = GetMarkers(doc.SciControl, ScintillaHelper.markerBPEnabled);
					BreakPointInfo cbinfo = m_BreakPointList.Find(delegate(BreakPointInfo info)
                    {
                        return info.FileFullPath == doc.FileName;
                    });
                    string exp = string.Empty;
                    if (cbinfo != null)
                    {
                        exp = cbinfo.Exp;
						m_BreakPointList.Remove(cbinfo);
                    }
                    foreach (int i in lines)
                    {
						m_BreakPointList.Add(new BreakPointInfo(doc.SciControl.FileName, i, exp, false, true));
                    }
                }
            }
        }

        public void ClearAll()
        {
			m_BreakPointList.Clear();
        }

		public void ResetAll()
		{
			List<BreakPointInfo> deleteList = new List<BreakPointInfo>();
			foreach (BreakPointInfo info in m_BreakPointList)
			{
				if (info.IsDeleted)
				{
					deleteList.Add(info);
				}
			}
			foreach (BreakPointInfo item in deleteList)
			{
				m_BreakPointList.Remove(item);
			}
		}

        public List<Int32> GetMarkers(ScintillaControl sci, int markerNum)
        {
            Int32 line = 0;
            List<Int32> markerLines = new List<Int32>();
            while (true)
            {
                int i = sci.MarkerNext(line, GetMarkerMask(markerNum));
                if ((sci.MarkerNext(line, GetMarkerMask(markerNum)) == -1) || (line > sci.LineCount)) break;
                line = sci.MarkerNext(line, GetMarkerMask(markerNum));
                markerLines.Add(line);
                line++;
            }
            return markerLines;
        }

        private static Int32 GetMarkerMask(Int32 marker)
        {
            return 1 << marker;
        }

		public int GetBreakPointIndex(String fileName, int line)
		{
			int index = m_BreakPointList.FindIndex(delegate(BreakPointInfo info)
			{
				return info.FileFullPath == fileName && info.Line == line;
			});
			return index;
		}

		public Boolean ShouldBreak(SourceFile file, int line)
		{
			String localPath = PluginMain.debugManager.GetLocalPath(file);
			if (localPath == null)
			{
				return false;
			}
			if (m_TemporaryBreakPointInfo != null)
			{
				if (m_TemporaryBreakPointInfo.FileFullPath == localPath && m_TemporaryBreakPointInfo.Line == (line - 1))
				{
					m_TemporaryBreakPointInfo.IsDeleted = true;
					List<BreakPointInfo> bpList = new List<BreakPointInfo>();
					bpList.Add(m_TemporaryBreakPointInfo);
					PluginMain.debugManager.FlashInterface.UpdateBreakpoints(bpList);
					m_TemporaryBreakPointInfo = null;
					return true;
				}
			}
			int index = GetBreakPointIndex(localPath, line - 1);
			if (index >= 0)
			{
				BreakPointInfo bpInfo = m_BreakPointList[index];
				if (bpInfo.ParsedExpression != null)
				{
                    try
                    {
                        var ctx = new ExpressionContext(PluginMain.debugManager.FlashInterface.Session, PluginMain.debugManager.FlashInterface.GetFrames()[PluginMain.debugManager.CurrentFrame]);
                        var val = bpInfo.ParsedExpression.evaluate(ctx);
                        if (val is java.lang.Boolean)
                        {
                            return ((java.lang.Boolean)val).booleanValue();
                        }
                        if (val is Value)
                        {
                            return ECMA.toBoolean(((Value)val));
                        }
                        if (val is Variable)
                        {
                            return ECMA.toBoolean(((Variable)val).getValue());
                        }
                        throw new NotImplementedException(val.toString());
                    }
					catch (/*Expression*/Exception e)
					{
                        TraceManager.AddAsync("[Problem in breakpoint: "+e.ToString()+"]", 4);
                        ErrorManager.ShowError(e);
						return true;
					}
				}
				else return true;
			}

			return true;
		}

		public void SetBreakPointsToEditor(PluginCore.ITabbedDocument[] documents)
        {
			m_bAccessable = false;
            for (int i = 0; i < documents.Length; i++)
            {
                ScintillaControl sci = documents[i].SciControl;
				if (sci == null) continue;
                if (Path.GetExtension(sci.FileName) == ".as" || Path.GetExtension(sci.FileName) == ".mxml")
                {
					foreach (BreakPointInfo info in m_BreakPointList)
                    {
						if (info.FileFullPath == sci.FileName && !info.IsDeleted)
                        {
							if (info.Line < 0 || info.Line + 1 > sci.LineCount) continue;
							sci.MarkerAdd(info.Line, info.IsEnabled ? ScintillaHelper.markerBPEnabled : ScintillaHelper.markerBPDisabled);
                        }
                    }
                }
            }
            m_bAccessable = true;
        }

        public void SetBreakPointsToEditor(string filefullpath)
        {
            m_bAccessable = false;
            PluginCore.ITabbedDocument[] documents = PluginBase.MainForm.Documents;
			for (int i = 0; i < documents.Length; i++)
            {
                ScintillaControl sci = documents[i].SciControl;
				if (sci == null) continue;
                if (sci.FileName == filefullpath)
                {
                    foreach (BreakPointInfo info in m_BreakPointList)
                    {
                        if (info.FileFullPath == sci.FileName && !info.IsDeleted)
                        {
							if (info.Line < 0 || info.Line + 1 > sci.LineCount) continue;
							sci.MarkerAdd(info.Line, info.IsEnabled ? ScintillaHelper.markerBPEnabled : ScintillaHelper.markerBPDisabled);
                        }
                    }
                    break;
                }
            }
            m_bAccessable = true;
        }

		public void ClearTemporaryBreakPoint()
		{
			if (m_TemporaryBreakPointInfo != null)
			{
				m_TemporaryBreakPointInfo.IsDeleted = true;
				List<BreakPointInfo> bpList = new List<BreakPointInfo>(new BreakPointInfo[] { m_TemporaryBreakPointInfo });
				PluginMain.debugManager.FlashInterface.UpdateBreakpoints(bpList);
				m_TemporaryBreakPointInfo = null;
			}
		}

		public void SetTemporaryBreakPoint(string filefullpath, int line)
		{
			ClearTemporaryBreakPoint();
			m_TemporaryBreakPointInfo = new BreakPointInfo(filefullpath, line, string.Empty, false, true);
			List<BreakPointInfo> bpList = new List<BreakPointInfo>(new BreakPointInfo[] { m_TemporaryBreakPointInfo });
			PluginMain.debugManager.FlashInterface.UpdateBreakpoints(bpList);
		}

		internal void SetBreakPointInfo(string filefullpath, int line, Boolean bDeleted, Boolean bEnabled)
        {
            if (!m_bAccessable) return;
            BreakPointInfo cbinfo = m_BreakPointList.Find(delegate(BreakPointInfo info)
            {
                return info.FileFullPath == filefullpath && info.Line == line;
            });
			string exp = string.Empty;
			if (cbinfo != null)
            {
                bool chn = (cbinfo.IsDeleted != bDeleted || cbinfo.IsEnabled != bEnabled);
                cbinfo.IsDeleted = bDeleted;
				cbinfo.IsEnabled = bEnabled;
				exp = cbinfo.Exp;
                // TMP
                if (chn && PluginMain.debugManager.FlashInterface.isDebuggerStarted) PluginMain.debugManager.FlashInterface.UpdateBreakpoints(this.BreakPoints);
            }
			else if (!bDeleted)
            {
				m_BreakPointList.Add(new BreakPointInfo(filefullpath, line, exp, bDeleted, bEnabled));
                // TMP
                if (PluginMain.debugManager.FlashInterface.isDebuggerStarted) PluginMain.debugManager.FlashInterface.UpdateBreakpoints(this.BreakPoints);
            }
            if (ChangeBreakPointEvent != null)
            {
				ChangeBreakPointEvent(this, new BreakPointArgs(filefullpath, line, exp, bDeleted, bEnabled));
            }
        }

		internal void SetBreakPointCondition(string filefullpath, int line, string exp)
		{
			int index = GetBreakPointIndex(filefullpath, line);
			if (index >= 0)
			{
				m_BreakPointList[index].Exp = exp;
			}
		}

		public void UpdateBreakPoint(string filefullpath, int line, int linesAdded)
        {
            foreach (BreakPointInfo info in m_BreakPointList)
            {
                if (info.FileFullPath == filefullpath && info.Line > line)
                {
                    int oldline = info.Line; 
                    info.Line += linesAdded;
                    if (ChangeBreakPointEvent != null)
                    {
                        UpdateBreakPointEvent(this, new UpdateBreakPointArgs(info.FileFullPath, oldline+1, info.Line+1));
                    }
                }
            }
        }

        public void Save()
        {
			if (m_Project != null)
            {
				List<BreakPointInfo> bpSaveList = new List<BreakPointInfo>();
				Uri u1 = new Uri(m_Project.ProjectPath);
                foreach (BreakPointInfo info in m_BreakPointList)
                {
					if (!info.IsDeleted)
					{
						Uri u2 = new Uri(u1, info.FileFullPath);
						info.FileFullPath = u1.MakeRelativeUri(u2).ToString();
						bpSaveList.Add(info);
					}
                }
                Util.SerializeXML<List<BreakPointInfo>>.SaveFile(m_SaveFileFullPath, bpSaveList);
            }
        }

        public void Load()
        {
            if (File.Exists(m_SaveFileFullPath))
            {
                m_BreakPointList = Util.SerializeXML<List<BreakPointInfo>>.LoadFile(m_SaveFileFullPath);

				Uri u1 = new Uri(m_Project.ProjectPath);
                foreach (BreakPointInfo info in m_BreakPointList)
                {
                    Uri u2 = new Uri(u1, info.FileFullPath);
                    info.FileFullPath = u2.LocalPath;
					if (ChangeBreakPointEvent != null)
                    {
                        ChangeBreakPointEvent(this, new BreakPointArgs(info.FileFullPath, info.Line, info.Exp, info.IsDeleted, info.IsEnabled));
                    }
                }
            }
        }
    }

    #region Internal Models

    public class BreakPointArgs : EventArgs
    {
        public int Line;
        public string Exp;
        public string FileFullPath;
        public Boolean IsDelete;
        public Boolean Enable;

        public BreakPointArgs(string filefullpath, int line, string exp, Boolean isdelete, Boolean enable)
        {
            FileFullPath = filefullpath;
            Line = line;
            Exp = exp;
            IsDelete = isdelete;
            Enable = enable;
        }
    }

    public class UpdateBreakPointArgs : EventArgs
    {
        public int OldLine;
        public int NewLine;
        public string FileFullPath;

        public UpdateBreakPointArgs(string filefullpath, int oldline, int newline)
        {
            FileFullPath = filefullpath;
            OldLine = oldline;
            NewLine = newline;
        }
    }

    public class BreakPointInfo
    {
        private int m_Line;
        private Boolean m_bDeleted;
        private Boolean m_bEnabled;
        private string m_FileFullPath;
		private string m_ConditionalExpression;
		private ValueExp m_ParsedExpression;

        public string FileFullPath
        {
			get { return m_FileFullPath; }
			set { m_FileFullPath = value; }
        }

		public int Line
        {
			get { return m_Line; }
			set { m_Line = value; }
        }

        public Boolean IsDeleted
        {
			get { return m_bDeleted; }
			set { m_bDeleted = value; }
        }

		public Boolean IsEnabled
        {
			get { return m_bEnabled; }
			set { m_bEnabled = value; }
        }

		public string Exp
        {
			get { return m_ConditionalExpression; }
			set
			{
                m_ConditionalExpression = value;
                m_ParsedExpression = null;
			}
        }

		[XmlIgnore]
		public ValueExp ParsedExpression
        {
			get
            {
                if (m_ParsedExpression != null) return m_ParsedExpression;
                if (m_ConditionalExpression != null && m_ConditionalExpression.Length > 0)
                {
                    try
                    {
                        // todo, we need to optimize in case of bad expession (to not clog the logs)
                        IASTBuilder builder = new ASTBuilder(false);
                        m_ParsedExpression = builder.parse(new java.io.StringReader(m_ConditionalExpression));
                    }
                    catch (Exception e)
                    {
                        ErrorManager.ShowError(e);
                    }
                }
                return m_ParsedExpression; 
            }
        }

        public BreakPointInfo()
        {
			m_FileFullPath = "";
			m_Line = 0;
			m_ConditionalExpression = "";
			m_bDeleted = false;
			m_bEnabled = false;
			m_ParsedExpression = null;
		}

        public BreakPointInfo(string fileFullPath, int line, string exp, Boolean bDeleted, Boolean bEnabled)
        {
			m_FileFullPath = fileFullPath;
			m_Line = line;
			m_bDeleted = bDeleted;
			m_bEnabled = bEnabled;
			m_ParsedExpression = null;
			Exp = exp;
		}

    }

    #endregion

}
