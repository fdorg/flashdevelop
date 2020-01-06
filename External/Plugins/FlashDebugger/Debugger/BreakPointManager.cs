using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using flash.tools.debugger;
using flash.tools.debugger.expression;
using PluginCore;
using PluginCore.Helpers;
using PluginCore.Managers;
using PluginCore.Utilities;
using ScintillaNet;
using Boolean = java.lang.Boolean;
using StringReader = java.io.StringReader;

namespace FlashDebugger
{
    public delegate void ConditionErrorEventHandler(object sender, BreakPointArgs e);
    public delegate void ChangeBreakPointEventHandler(object sender, BreakPointArgs e);
    public delegate void UpdateBreakPointEventHandler(object sender, UpdateBreakPointArgs e);

    public class BreakPointManager
    {
        IProject m_Project;
        string m_SaveFileFullPath;
        bool m_bAccessable = true;
        BreakPointInfo m_TemporaryBreakPointInfo;

        public event ChangeBreakPointEventHandler ChangeBreakPointEvent;
        public event UpdateBreakPointEventHandler UpdateBreakPointEvent;

        public List<BreakPointInfo> BreakPoints { get; private set; } = new List<BreakPointInfo>();

        public IProject Project
        {
            get => m_Project;
            set 
            {
                if (value != null)
                {
                    m_Project = value;
                    ClearAll();
                    m_SaveFileFullPath = GetBreakpointsFile(m_Project.ProjectPath);
                }
            }
        }

        static string GetBreakpointsFile(string path)
        {
            string cacheDir = Path.Combine(PathHelper.DataDir, "FlashDebugger", "Breakpoints");
            if (!Directory.Exists(cacheDir)) Directory.CreateDirectory(cacheDir);
            string hashFileName = HashCalculator.CalculateSHA1(path);
            return Path.Combine(cacheDir, hashFileName + ".xml");
        }

        public void InitBreakPoints()
        {
            foreach (var doc in PluginBase.MainForm.Documents)
            {
                var sci = doc.SciControl;
                if (Path.GetExtension(sci.FileName) != ".as" &&
                    Path.GetExtension(sci.FileName) != ".mxml") continue;
                var lines = GetMarkers(sci, ScintillaHelper.markerBPEnabled);
                var cbinfo = BreakPoints.Find(info => info.FileFullPath.Equals(sci.FileName, StringComparison.OrdinalIgnoreCase));
                var exp = string.Empty;
                if (cbinfo != null)
                {
                    exp = cbinfo.Exp;
                    BreakPoints.Remove(cbinfo);
                }
                foreach (int i in lines)
                {
                    BreakPoints.Add(new BreakPointInfo(sci.FileName, i, exp, false, true));
                }
            }
        }

        public void ClearAll() => BreakPoints.Clear();

        public void ResetAll()
        {
            for (int i = BreakPoints.Count - 1; i >= 0; i--)
            {
                if (BreakPoints[i].IsDeleted) BreakPoints.RemoveAt(i);
            }
        }

        public List<int> GetMarkers(ScintillaControl sci, int markerNum)
        {
            var line = 0;
            var result = new List<int>();
            while (true)
            {
                if ((sci.MarkerNext(line, GetMarkerMask(markerNum)) == -1) || (line > sci.LineCount)) break;
                line = sci.MarkerNext(line, GetMarkerMask(markerNum));
                result.Add(line);
                line++;
            }
            return result;
        }

        static int GetMarkerMask(int marker) => 1 << marker;

        public int GetBreakPointIndex(string fileName, int line)
            => BreakPoints.FindIndex(info => info.FileFullPath.Equals(fileName, StringComparison.OrdinalIgnoreCase) && info.Line == line);

        public bool ShouldBreak(SourceFile file, int line) => ShouldBreak(file, line, null);

        public bool ShouldBreak(SourceFile file, int line, Frame frame)
        {
            var localPath = PluginMain.debugManager.GetLocalPath(file);
            if (localPath is null) return false;
            if (m_TemporaryBreakPointInfo != null)
            {
                if (m_TemporaryBreakPointInfo.FileFullPath == localPath && m_TemporaryBreakPointInfo.Line == (line - 1))
                {
                    m_TemporaryBreakPointInfo.IsDeleted = true;
                    var bpList = new List<BreakPointInfo> {m_TemporaryBreakPointInfo};
                    PluginMain.debugManager.FlashInterface.UpdateBreakpoints(bpList);
                    m_TemporaryBreakPointInfo = null;
                    return true;
                }
            }
            int index = GetBreakPointIndex(localPath, line - 1);
            if (index < 0) return true;
            BreakPointInfo bpInfo = BreakPoints[index];
            if (bpInfo.ParsedExpression is null) return true;
            try
            {
                if (frame is null)
                {
                    // take currently active worker and frame
                    frame = PluginMain.debugManager.FlashInterface.GetFrames()[PluginMain.debugManager.CurrentFrame];
                }
                var ctx = new ExpressionContext(PluginMain.debugManager.FlashInterface.Session, frame);
                var val = bpInfo.ParsedExpression.evaluate(ctx);
                return val switch
                {
                    Boolean boolean => boolean.booleanValue(),
                    Value value => ECMA.toBoolean(value),
                    Variable variable => ECMA.toBoolean(variable.getValue()),
                    _ => throw new NotImplementedException(val.toString()),
                };
            }
            catch (/*Expression*/Exception e)
            {
                TraceManager.AddAsync("[Problem in breakpoint: "+e+"]", 4);
                ErrorManager.ShowError(e);
                return true;
            }
        }

        public void SetBreakPointsToEditor(ITabbedDocument[] documents)
        {
            m_bAccessable = false;
            foreach (ITabbedDocument document in documents)
            {
                ScintillaControl sci = document.SciControl;
                if (sci is null) continue;
                if (Path.GetExtension(sci.FileName) == ".as" || Path.GetExtension(sci.FileName) == ".mxml")
                {
                    foreach (BreakPointInfo info in BreakPoints)
                    {
                        if (info.FileFullPath.Equals(sci.FileName, StringComparison.OrdinalIgnoreCase) && !info.IsDeleted)
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
            foreach (ITabbedDocument document in PluginBase.MainForm.Documents)
            {
                ScintillaControl sci = document.SciControl;
                if (sci is null) continue;
                if (sci.FileName.Equals(filefullpath, StringComparison.OrdinalIgnoreCase))
                {
                    foreach (BreakPointInfo info in BreakPoints)
                    {
                        if (info.FileFullPath.Equals(sci.FileName, StringComparison.OrdinalIgnoreCase) && !info.IsDeleted)
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
            if (m_TemporaryBreakPointInfo is null) return;
            m_TemporaryBreakPointInfo.IsDeleted = true;
            var bpList = new List<BreakPointInfo>(new[] { m_TemporaryBreakPointInfo });
            PluginMain.debugManager.FlashInterface.UpdateBreakpoints(bpList);
            m_TemporaryBreakPointInfo = null;
        }

        public void SetTemporaryBreakPoint(string filefullpath, int line)
        {
            ClearTemporaryBreakPoint();
            m_TemporaryBreakPointInfo = new BreakPointInfo(filefullpath, line, string.Empty, false, true);
            var bpList = new List<BreakPointInfo>(new[] { m_TemporaryBreakPointInfo });
            PluginMain.debugManager.FlashInterface.UpdateBreakpoints(bpList);
        }

        internal void SetBreakPointInfo(string filefullpath, int line, bool bDeleted, bool bEnabled)
        {
            if (!m_bAccessable) return;
            var cbinfo = BreakPoints.Find(info => info.FileFullPath == filefullpath && info.Line == line);
            string exp = string.Empty;
            if (cbinfo != null)
            {
                bool chn = (cbinfo.IsDeleted != bDeleted || cbinfo.IsEnabled != bEnabled);
                cbinfo.IsDeleted = bDeleted;
                cbinfo.IsEnabled = bEnabled;
                exp = cbinfo.Exp;
                // TMP
                if (chn && PluginMain.debugManager.FlashInterface.isDebuggerStarted) PluginMain.debugManager.FlashInterface.UpdateBreakpoints(BreakPoints);
            }
            else if (!bDeleted)
            {
                BreakPoints.Add(new BreakPointInfo(filefullpath, line, exp, bDeleted, bEnabled));
                // TMP
                if (PluginMain.debugManager.FlashInterface.isDebuggerStarted) PluginMain.debugManager.FlashInterface.UpdateBreakpoints(BreakPoints);
            }

            ChangeBreakPointEvent?.Invoke(this, new BreakPointArgs(filefullpath, line, exp, bDeleted, bEnabled));
        }

        internal void SetBreakPointCondition(string filefullpath, int line, string exp)
        {
            int index = GetBreakPointIndex(filefullpath, line);
            if (index >= 0)
            {
                BreakPoints[index].Exp = exp;
            }
        }

        public void UpdateBreakPoint(string filefullpath, int line, int linesAdded)
        {
            foreach (BreakPointInfo info in BreakPoints)
            {
                if (info.FileFullPath == filefullpath && info.Line > line)
                {
                    int oldline = info.Line; 
                    info.Line += linesAdded;
                    UpdateBreakPointEvent?.Invoke(this, new UpdateBreakPointArgs(info.FileFullPath, oldline+1, info.Line+1));
                }
            }
        }

        public void Save() => Save(m_SaveFileFullPath);

        public void Save(string filePath)
        {
            if (m_Project is null) return;
            var bpSaveList = new List<BreakPointInfo>();
            foreach (BreakPointInfo info in BreakPoints)
            {
                if (!info.IsDeleted)
                {
                    BreakPointInfo infoCopy;

                    if (!Path.IsPathRooted(info.FileFullPath))
                        infoCopy = info;
                    else
                        infoCopy = new BreakPointInfo(m_Project.GetRelativePath(info.FileFullPath),
                            info.Line, info.Exp, info.IsDeleted, info.IsEnabled);
                        
                    bpSaveList.Add(infoCopy);
                }
            }
            Util.SerializeXML<List<BreakPointInfo>>.SaveFile(filePath, bpSaveList);
        }

        public void Load() => Load(m_SaveFileFullPath);

        public void Load(string filePath)
        {
            if (!File.Exists(filePath)) return;
            BreakPoints = Util.SerializeXML<List<BreakPointInfo>>.LoadFile(filePath);
            BreakPoints.RemoveAll(info => info.Line < 0);

            foreach (BreakPointInfo info in BreakPoints)
            {
                info.FileFullPath = m_Project.GetAbsolutePath(info.FileFullPath);
                ChangeBreakPointEvent?.Invoke(this, new BreakPointArgs(info.FileFullPath, info.Line, info.Exp, info.IsDeleted, info.IsEnabled));
            }
        }

        public void Import(string filePath)
        {
            if (!File.Exists(filePath)) return;
            var breakPointList = Util.SerializeXML<List<BreakPointInfo>>.LoadFile(filePath);

            foreach (BreakPointInfo info in breakPointList)
            {
                if (info.Line < 0) continue;
                info.FileFullPath = m_Project.GetAbsolutePath(info.FileFullPath);
                BreakPointInfo existing = null;
                if (BreakPoints != null && (existing = BreakPoints.Find(b => b.FileFullPath == info.FileFullPath && b.Line == info.Line)) != null && !existing.IsDeleted)
                    continue;

                if (existing != null)
                {
                    existing.IsDeleted = false;
                    existing.IsEnabled = info.IsEnabled;
                    existing.Exp = info.Exp;
                }
                else
                {
                    if (BreakPoints is null) BreakPoints = new List<BreakPointInfo>();
                    BreakPoints.Add(info);
                }

                ChangeBreakPointEvent?.Invoke(this, new BreakPointArgs(info.FileFullPath, info.Line, info.Exp, info.IsDeleted, info.IsEnabled));
            }
        }
    }

    #region Internal Models

    public class BreakPointArgs : EventArgs
    {
        public int Line;
        public string Exp;
        public string FileFullPath;
        public bool IsDelete;
        public bool Enable;

        public BreakPointArgs(string filefullpath, int line, string exp, bool isdelete, bool enable)
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
        string m_ConditionalExpression;
        ValueExp m_ParsedExpression;

        public string FileFullPath { get; set; }

        public int Line { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsEnabled { get; set; }

        public string Exp
        {
            get => m_ConditionalExpression;
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
                if (!string.IsNullOrEmpty(m_ConditionalExpression))
                {
                    try
                    {
                        // todo, we need to optimize in case of bad expession (to not clog the logs)
                        IASTBuilder builder = new ASTBuilder(false);
                        m_ParsedExpression = builder.parse(new StringReader(m_ConditionalExpression));
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
            FileFullPath = "";
            Line = 0;
            m_ConditionalExpression = "";
            IsDeleted = false;
            IsEnabled = false;
            m_ParsedExpression = null;
        }

        public BreakPointInfo(string fileFullPath, int line, string exp, bool bDeleted, bool bEnabled)
        {
            FileFullPath = fileFullPath;
            Line = line;
            IsDeleted = bDeleted;
            IsEnabled = bEnabled;
            m_ParsedExpression = null;
            Exp = exp;
        }

    }

    #endregion
}