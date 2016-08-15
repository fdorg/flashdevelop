using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using PluginCore;
using ScintillaNet;
using ScintillaNet.Enums;

namespace SourceControl.Sources.Git
{
    internal class BlameCommand : BaseCommand, IDisposable
    {
        private static List<BlameCommand> blameCommands;

        private string fileName;
        private ITabbedDocument document;
        private ScintillaControl sci;
        private LinkedList<string> outputLines;
        private List<AnnotationData> annotations;
        private Dictionary<string, AnnotationData> commits;
        private ToolTip tooltip;
        private bool loading;

        static BlameCommand()
        {
            blameCommands = new List<BlameCommand>();
            PluginMain.ApplyTheme += ApplyTheme;
        }

        public BlameCommand(string file)
        {
            if (string.IsNullOrEmpty(file))
            {
                Dispose();
            }
            else
            {
                fileName = file;
                annotations = new List<AnnotationData>();
                commits = new Dictionary<string, AnnotationData>();
                CheckExisting();
                OpenAnnotatedDocument();
                Update();
            }
        }

        private static void ApplyTheme(object sender, EventArgs e)
        {
            foreach (var cmd in blameCommands)
            {
                cmd.UpdateBackColor(true);
            }
        }
        
        private void CheckExisting()
        {
            for (int i = 0, length = blameCommands.Count; i < length; i++)
            {
                var cmd = blameCommands[i];
                if (cmd.fileName == fileName)
                {
                    cmd.Update();
                    Dispose();
                    return;
                }
            }
            blameCommands.Add(this);
        }

        private void OpenAnnotatedDocument()
        {
            if (!disposed)
            {
                string documentName = Path.Combine(Path.GetDirectoryName(fileName), "[Annotated] " + Path.GetFileName(fileName));
                document = PluginBase.MainForm.CreateEditableDocument(documentName, "", Encoding.UTF8.CodePage) as ITabbedDocument;
                if (document == null)
                {
                    Dispose();
                }
                else
                {
                    ((Form) document).FormClosed += Document_FormClosed;
                    sci = document.SciControl;
                } 
            }
        }

        private void Update()
        {
            if (!disposed && !loading)
            {
                loading = true;
                sci.IsReadOnly = false;
                sci.SetText("Loading..."); // TODO: Localisation
                sci.IsReadOnly = true;
                outputLines = new LinkedList<string>();
                Run("blame --porcelain \"" + fileName + "\"", Path.GetDirectoryName(fileName));
            }
        }

        protected override void Runner_Output(object sender, string line)
        {
            if (!disposed)
            {
                outputLines.AddLast(line);
            }
        }

        protected override void Runner_ProcessEnded(object sender, int exitCode)
        {
            if (!disposed)
            {
                sci.IsReadOnly = false;
                try
                {
                    if (outputLines.Count > 0 && exitCode == 0)
                    {
                        annotations.Clear();
                        commits.Clear();
                        int i;
                        string longestInfo = "";
                        var sb = new StringBuilder();
                        AnnotationData lastAnnotation = null;
                        for (i = 0; 0 < outputLines.Count; i++)
                        {
                            var annotation = ParseAnnotation();
                            if (lastAnnotation != null)
                            {
                                lastAnnotation.LineEnd = i - 1;
                                annotation.Line = sci.NewLineMarker + annotation.Line;
                            }

                            sb.Append(annotation.Line);

                            if (lastAnnotation == null || annotation.Commit != lastAnnotation.Commit)
                            {
                                if (commits.ContainsKey(annotation.Commit))
                                {
                                    var def = commits[annotation.Commit];
                                    annotation.Author = def.Author;
                                    annotation.AuthorMail = def.AuthorMail;
                                    annotation.AuthorTime = def.AuthorTime;
                                    annotation.AuthorTimeZone = def.AuthorTimeZone;
                                    annotation.Committer = def.Committer;
                                    annotation.CommitterMail = def.CommitterMail;
                                    annotation.CommitterTime = def.CommitterTime;
                                    annotation.CommitterTimeZone = def.CommitterTimeZone;
                                    annotation.FileName = def.FileName;
                                    annotation.IsEdit = def.IsEdit;
                                    annotation.Summary = def.Summary;
                                    annotation.Color = def.Color;
                                }
                                else
                                {
                                    commits.Add(annotation.Commit, annotation);
                                    int style = 128 + commits.Count % 128;
                                    sci.StyleSetFont(style, sci.StyleGetFont(0));
                                    sci.StyleSetSize(style, sci.StyleGetSize(0));
                                    sci.StyleSetFore(style, sci.StyleGetFore(0));
                                    sci.StyleSetBack(style, sci.StyleGetBack(0));
                                    sci.StyleSetBold(style, sci.StyleGetBold(0));
                                    sci.StyleSetItalic(style, sci.StyleGetItalic(0));
                                    annotation.Color = style;
                                }

                                string info = annotation.GetInfo();
                                if (info.Length > longestInfo.Length)
                                {
                                    longestInfo = info;
                                }
                                annotation.LineStart = i;
                                lastAnnotation = annotation;
                                annotations.Add(annotation);
                            }
                        }

                        lastAnnotation.LineEnd = i - 1;

                        sci.SetMarginTypeN(4, (int) MarginType.Text);
                        sci.SetMarginWidthN(4, sci.TextWidth(0, longestInfo));
                        sci.MarginSensitiveN(4, true);
                        sci.SetMarginCursorN(4, 2);

                        sci.SetText(sb.ToString());

                        for (i = 0; i < annotations.Count; i++)
                        {
                            var annotation = annotations[i];
                            sci.SetMarginText(annotation.LineStart, annotation.GetInfo(longestInfo.Length));
                            for (int j = annotation.LineStart; j <= annotation.LineEnd; j++)
                            {
                                sci.SetMarginStyle(j, annotation.Color);
                            }
                        }

                        UpdateBackColor(false);

                        outputLines.Clear();
                        outputLines = null;
                        annotations.TrimExcess();
                        tooltip = new ToolTip();
                        sci.DwellStart += Sci_DwellStart;
                        sci.DwellEnd += Sci_DwellEnd;
                        sci.MarginClick += Sci_MarginClick;
                    }
                }
                finally
                {
                    sci.SetSavePoint();
                    sci.IsReadOnly = true;
                    loading = false;
                }
            }
            base.Runner_ProcessEnded(sender, exitCode);
        }
        
        private void UpdateBackColor(bool applyingTheme)
        {
            if (applyingTheme)
            {
                sci.BeginInvoke((MethodInvoker) delegate { UpdateBackColor(false); });
                return;
            }
            var random = new Random();
            int count = 0;
            int color = sci.StyleGetBack(0);
            int r = color >> 16 & 0xFF;
            int g = color >> 8 & 0xFF;
            int b = color & 0xFF;
            foreach (var annotation in commits.Values)
            {
                int style = 128 + ++count % 128;
                int newR = r + random.Next(0xFF) >> 1;
                int newG = g + random.Next(0xFF) >> 1;
                int newB = b + random.Next(0xFF) >> 1;
                sci.StyleSetBack(style, newR << 16 | newG << 8 | newB);
            }
        }

        private void Document_FormClosed(object sender, FormClosedEventArgs e)
        {
            Dispose();
        }

        private void Sci_DwellStart(ScintillaControl sender, int position, int x, int y)
        {
            if (!disposed && position == -1)
            {
                int line = sci.LineFromPosition(sci.PositionFromPoint(x, y));
                var annotationData = GetAnnotationData(line);
                if (annotationData != null)
                {
                    tooltip.Show(annotationData.ToString(), sci, x, y);
                }
            }
        }

        private void Sci_DwellEnd(ScintillaControl sender, int position, int x, int y)
        {
            if (!disposed) tooltip.Hide(sci);
        }
        
        private void Sci_MarginClick(ScintillaControl sender, int modifiers, int position, int margin)
        {
            if (!disposed)
            {
                Console.WriteLine(Control.MouseButtons);
                int line = sci.LineFromPosition(position);
                var annotationData = GetAnnotationData(line);
                if (annotationData != null && annotationData.LineStart == line)
                {
                    TortoiseProc.ExecuteCustom("log", string.Format("/path:\"{0}\" /rev:{1}", annotationData.FileName, annotationData.Commit));
                }
            }
        }

        private AnnotationData ParseAnnotation()
        {
            var data = new AnnotationData();
            data.Commit = GetNextOutputLine().Substring(0, 40);

            string line;
            while (!(line = GetNextOutputLine()).StartsWith('\t'))
            {
                int separator = line.IndexOf(' ');
                string key = line.Substring(0, separator);
                string value = line.Substring(separator + 1);

                switch (key)
                {
                    case "author":
                        data.Author = value;
                        break;
                    case "author-mail":
                        data.AuthorMail = value;
                        break;
                    case "author-time":
                        data.AuthorTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(int.Parse(value));
                        break;
                    case "author-tz":
                        data.AuthorTimeZone = value;
                        break;
                    case "committer":
                        data.Committer = value;
                        break;
                    case "committer-mail":
                        data.CommitterMail = value;
                        break;
                    case "committer-time":
                        data.CommitterTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(int.Parse(value));
                        break;
                    case "committer-tz":
                        data.CommitterTimeZone = value;
                        break;
                    case "previous":
                        data.IsEdit = true;
                        break;
                    case "summary":
                        data.Summary = value;
                        break;
                    case "filename":
                        data.FileName = value;
                        break;
                }
            }

            data.Line = line.Substring(1);
            return data;
        }

        private AnnotationData GetAnnotationData(int line)
        {
            int low = 0;
            int high = annotations.Count - 1;
            while (low <= high)
            {
                int i = low + (high - low >> 1);
                var annotationData = annotations[i];
                if (line < annotationData.LineStart)
                {
                    high = i - 1;
                }
                else if (line > annotationData.LineEnd)
                {
                    low = i + 1;
                }
                else return annotationData;
            }
            return null;
        }

        private string GetNextOutputLine()
        {
            string value = outputLines.First.Value;
            outputLines.RemoveFirst();
            return value;
        }

        #region IDisposable

        private bool disposed = false;

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    blameCommands.Remove(this);
                    if (document != null) ((Form) document).FormClosed -= Document_FormClosed;
                    if (sci != null)
                    {
                        sci.DwellStart -= Sci_DwellStart;
                        sci.DwellEnd -= Sci_DwellEnd;
                        sci.MarginClick -= Sci_MarginClick;
                    }
                    if (outputLines != null) outputLines.Clear();
                    if (annotations != null) annotations.Clear();
                    if (commits != null) commits.Clear();
                    if (tooltip != null) tooltip.Dispose();
                }
                
                fileName = null;
                document = null;
                sci = null;
                outputLines = null;
                annotations = null;
                commits = null;
                tooltip = null;
                loading = false;

                disposed = true;
            }
        }

        #endregion

        class AnnotationData
        {
            public string Commit;
            public string Author;
            public string AuthorMail;
            public DateTime AuthorTime;
            public string AuthorTimeZone;
            public string Committer;
            public string CommitterMail;
            public DateTime CommitterTime;
            public string CommitterTimeZone;
            public string FileName;
            public string Summary;
            public string Line;
            public bool IsEdit;
            public int LineStart;
            public int LineEnd;
            public int Color;

            public string GetInfo(int width = 0)
            {
                string commit = Commit.Substring(0, 8);
                string authorTime = AuthorTime.ToShortDateString();

                if (commit == "00000000")
                {
                    return "Local";
                }
                width -= 10 + authorTime.Length;
                return Commit.Substring(0, 8) + " " + (width > 0 ? Author.PadRight(width) : Author) + " " + authorTime;
            }

            public override string ToString()
            {
                return "Commit" + ": " + Commit + "\n" +
                    "Author" + ": " + Author + " " + AuthorMail + "\n" +
                    "Author Date" + ": " + AuthorTime + " " + AuthorTimeZone + "\n" +
                    "Committer" + ": " + Committer + " " + CommitterMail + "\n" +
                    "Committer Date" + ": " + CommitterTime + " " + CommitterTimeZone + "\n" +
                    "Change" + ": " + (IsEdit ? "edit" : "add") + "\n" +
                    "Lines" + ": " + (LineStart + 1) + "~" + (LineEnd + 1) + "\n" +
                    "Path" + ": " + FileName + "\n" +
                    "\n" + Summary;
            }
        }
    }
}
