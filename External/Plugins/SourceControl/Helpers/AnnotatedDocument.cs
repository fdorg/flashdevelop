using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using PluginCore;
using PluginCore.Helpers;
using ScintillaNet;
using ScintillaNet.Enums;

namespace SourceControl.Helpers
{
    internal class AnnotatedDocument : IDisposable
    {
        private static List<AnnotatedDocument> documents;

        private IBlameCommand command;
        private string fileName;
        private ITabbedDocument document;
        private ScintillaControl sci;
        private AnnotationData[] annotations;
        private Dictionary<string, AnnotationData> commits;
        private ToolTip tooltip;

        static AnnotatedDocument()
        {
            documents = new List<AnnotatedDocument>();
        }

        private AnnotatedDocument(IBlameCommand cmd, string file, ITabbedDocument doc)
        {
            documents.Add(this);
            command = cmd;
            fileName = file;
            document = doc;
            sci = document.SciControl;
            annotations = null;
            commits = new Dictionary<string, AnnotationData>();
            tooltip = new ToolTip();

            ((Form) document).FormClosed += Document_FormClosed;
            sci.DwellStart += Sci_DwellStart;
            sci.DwellEnd += Sci_DwellEnd;
            sci.MarginClick += Sci_MarginClick;
        }

        public static AnnotatedDocument CreateAnnotatedDocument(IBlameCommand command, string fileName)
        {
            string title = Path.Combine(Path.GetDirectoryName(fileName), "[Annotated] " + Path.GetFileName(fileName));
            var doc = PluginBase.MainForm.CreateEditableDocument(title, string.Empty, Encoding.UTF8.CodePage) as ITabbedDocument;
            return doc == null ? null : new AnnotatedDocument(command, fileName, doc);
        }

        public static bool CheckExisting(string fileName)
        {
            foreach (var doc in documents)
            {
                if (doc.fileName == fileName)
                {
                    doc.command.Update();
                    return true;
                }
            }
            return false;
        }

        public static void ApplyTheme()
        {
            foreach (var doc in documents)
            {
                doc.UpdateBackColor(true);
            }
        }

        public void Initialize()
        {
            sci.IsReadOnly = false;
            sci.MarginTextClearAll();
            sci.SetText("Loading..."); // TODO: Localisation
            sci.IsReadOnly = true;

            annotations = null;
            commits.Clear();
        }

        public void Annotate(AnnotationData[] annotationData)
        {
            sci.IsReadOnly = false;
            try
            {
                sci.SetText(FileHelper.ReadFile(fileName));

                annotations = annotationData;
                OrganizeAnnotations();
                string longestInfo = ParseCommits();
                ShowAnnotation(longestInfo);
                UpdateBackColor(false);
            }
            finally
            {
                sci.SetSavePoint();
                sci.IsReadOnly = true;
            }
        }

        private void OrganizeAnnotations()
        {
            Array.Sort(annotations, (x, y) => x.ResultLine.CompareTo(y.ResultLine));
            int i = 0, offset = 0, length = annotations.Length - 1;
            while (i < length)
            {
                var current = annotations[i++ - offset];
                var next = annotations[i];
                if (current.Hash == next.Hash)
                {
                    // Assume no lines are missing from 'git blame' output,
                    // meaning current.ResultLine + current.LineCount == next.ResultLine
                    current.LineCount += next.LineCount;
                    offset++;
                }
                else
                {
                    annotations[i - offset] = next;
                }
            }
            Array.Resize(ref annotations, annotations.Length - offset);
        }

        private string ParseCommits()
        {
            string longestInfo = string.Empty;
            foreach (var annotation in annotations)
            {
                if (commits.ContainsKey(annotation.Hash))
                {
                    var def = commits[annotation.Hash];
                    annotation.Author = def.Author;
                    annotation.AuthorMail = def.AuthorMail;
                    annotation.AuthorTime = def.AuthorTime;
                    annotation.AuthorTimeZone = def.AuthorTimeZone;
                    annotation.Committer = def.Committer;
                    annotation.CommitterMail = def.CommitterMail;
                    annotation.CommitterTime = def.CommitterTime;
                    annotation.CommitterTimeZone = def.CommitterTimeZone;
                    annotation.FileName = def.FileName;
                    annotation.Message = def.Message;
                    annotation.IsEdit = def.IsEdit;
                    annotation.MarginStyle = def.MarginStyle;
                }
                else
                {
                    commits.Add(annotation.Hash, annotation);
                    int style = 0x80 + commits.Count % 0x80;
                    sci.StyleSetFont(style, sci.StyleGetFont(0));
                    sci.StyleSetSize(style, sci.StyleGetSize(0));
                    sci.StyleSetFore(style, sci.StyleGetFore(0));
                    sci.StyleSetBack(style, sci.StyleGetBack(0));
                    sci.StyleSetBold(style, sci.StyleGetBold(0));
                    sci.StyleSetItalic(style, sci.StyleGetItalic(0));
                    annotation.MarginStyle = style;
                }

                string info = annotation.GetInfo();
                if (info.Length > longestInfo.Length)
                {
                    longestInfo = info;
                }
            }

            return longestInfo;
        }

        private void ShowAnnotation(string longestInfo)
        {
            int width = longestInfo.Length;
            sci.SetMarginTypeN(4, (int) MarginType.Text);
            sci.SetMarginWidthN(4, sci.TextWidth(0, longestInfo));
            sci.MarginSensitiveN(4, true);
            sci.SetMarginCursorN(4, (int) CursorShape.Arrow);

            foreach (var annotation in annotations)
            {
                int line = annotation.ResultLine;
                sci.SetMarginText(line, annotation.GetInfo(width));
                for (int i = 0; i < annotation.LineCount; i++)
                {
                    sci.SetMarginStyle(line + i, annotation.MarginStyle);
                }
            }
        }

        private void UpdateBackColor(bool applyingTheme)
        {
            if (applyingTheme)
            {
                sci.BeginInvoke((MethodInvoker) delegate { UpdateBackColor(false); });
                return;
            }
            var random = new Random();
            int color = sci.StyleGetBack(0);
            int r = color >> 16 & 0xFF;
            int g = color >> 8 & 0xFF;
            int b = color & 0xFF;
            foreach (var annotation in commits.Values)
            {
                int newR = r + random.Next(0xFF) >> 1;
                int newG = g + random.Next(0xFF) >> 1;
                int newB = b + random.Next(0xFF) >> 1;
                sci.StyleSetBack(annotation.MarginStyle, newR << 16 | newG << 8 | newB);
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
                    tooltip.Show(annotationData.ToString(), sci, x, y + 10);
                }
            }
        }

        private void Sci_DwellEnd(ScintillaControl sender, int position, int x, int y)
        {
            if (!disposed)
            {
                tooltip.Hide(sci);
            }
        }

        private void Sci_MarginClick(ScintillaControl sender, int modifiers, int position, int margin)
        {
            if (!disposed)
            {
                int line = sci.LineFromPosition(position);
                var annotationData = GetAnnotationData(line);
                if (annotationData != null && annotationData.ResultLine == line)
                {
                    command.ShowOnFileHistory(annotationData.Hash);
                }
            }
        }

        private AnnotationData GetAnnotationData(int line)
        {
            int low = 0;
            int high = annotations.Length - 1;
            while (low <= high)
            {
                int i = low + (high - low >> 1);
                var annotationData = annotations[i];
                if (line < annotationData.ResultLine)
                {
                    high = i - 1;
                }
                else if (line >= annotationData.ResultLine + annotationData.LineCount)
                {
                    low = i + 1;
                }
                else return annotationData;
            }
            return null;
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
                    documents.Remove(this);
                    if (command != null) command.Dispose();
                    if (tooltip != null) tooltip.Dispose();
                    if (document != null) ((Form) document).FormClosed -= Document_FormClosed;
                    if (sci != null)
                    {
                        sci.DwellStart -= Sci_DwellStart;
                        sci.DwellEnd -= Sci_DwellEnd;
                        sci.MarginClick -= Sci_MarginClick;
                    }
                }

                command = null;
                fileName = null;
                document = null;
                sci = null;
                annotations = null;
                commits = null;
                tooltip = null;

                disposed = true;
            }
        }

        #endregion
    }

    internal class AnnotationData : Commit
    {
        public bool IsEdit;
        public int SourceLine;
        public int ResultLine;
        public int LineCount;
        public int MarginStyle;

        public string GetInfo(int width = 0)
        {
            string commit = Hash.Substring(0, 8);
            string authorTime = AuthorTime.ToShortDateString();

            if (commit == "00000000")
            {
                return "Local"; // TODO: Localisation
            }
            width -= 10 + authorTime.Length;
            return Hash.Substring(0, 8) + " " + (width > 0 ? Author.PadRight(width) : Author) + " " + authorTime;
        }

        public override string ToString()
        {
            return "Commit" + ": " + Hash + "\n" +
                "Author" + ": " + Author + " " + AuthorMail + "\n" +
                "Author Date" + ": " + AuthorTime + " " + AuthorTimeZone + "\n" +
                "Committer" + ": " + Committer + " " + CommitterMail + "\n" +
                "Committer Date" + ": " + CommitterTime + " " + CommitterTimeZone + "\n" +
                "Change" + ": " + (IsEdit ? "edit" : "add") + "\n" +
                "Lines" + ": " + (ResultLine + 1) + "~" + (ResultLine + LineCount) + "\n" +
                "Path" + ": " + FileName + "\n" +
                "\n" + Message;
        }
    }

    internal interface IBlameCommand : IDisposable
    {
        void Update();
        void ShowOnFileHistory(string commit);
    }
}
