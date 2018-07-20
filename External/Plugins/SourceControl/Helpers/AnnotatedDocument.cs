﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        #region Fields

        private static List<AnnotatedDocument> documents;

        private IBlameCommand command;
        private string fileName;
        private ITabbedDocument document;
        private ScintillaControl sci;
        private AnnotationData[] annotations;
        private Dictionary<string, AnnotationData> commits;
        private ToolTip tooltip;
        private ContextMenuStrip contextMenu;
        private ToolStripMenuItem showOnFileHistoryMenuItem;
        private int MarginStart;
        private int MarginEnd;

        #endregion

        #region Constructors

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
            contextMenu = new ContextMenuStrip();

            InitializeContextMenu();
            AddEventHandlers();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates a new instance of <see cref="AnnotatedDocument"/>.
        /// </summary>
        /// <param name="command">The command object associated with this annoated document.</param>
        /// <param name="fileName">The path of the target file to annotate.</param>
        public static AnnotatedDocument CreateAnnotatedDocument(IBlameCommand command, string fileName)
        {
            string title = Path.Combine(Path.GetDirectoryName(fileName), "[Annotated] " + Path.GetFileName(fileName));
            var doc = PluginBase.MainForm.CreateEditableDocument(title, string.Empty, Encoding.UTF8.CodePage) as ITabbedDocument;
            return doc == null ? null : new AnnotatedDocument(command, fileName, doc);
        }

        /// <summary>
        /// Checks whether a document with the specified filename is already open,
        /// and if <code>true</code>, calls <see cref="IBlameCommand.Update"/> on the <see cref="IBlameCommand"/> object associated with the <see cref="AnnotatedDocument"/> object.
        /// </summary>
        /// <param name="fileName">The path of the file to check.</param>
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

        /// <summary>
        /// Applies the current theme to all open <see cref="AnnotatedDocument"/> objects.
        /// </summary>
        public static void ApplyTheme()
        {
            foreach (var doc in documents)
            {
                doc.UpdateTheme(true);
            }
        }

        /// <summary>
        /// Initializes this <see cref="AnnotatedDocument"/> object.
        /// </summary>
        public void Initialize()
        {
            sci.DwellStart -= Sci_DwellStart;
            sci.DwellEnd -= Sci_DwellEnd;
            ((Form) document).ContextMenuStrip = PluginBase.MainForm.EditorMenu;

            sci.IsReadOnly = false;
            sci.MarginTextClearAll();
            sci.SetText("Loading..."); // TODO: Localisation
            sci.SetSavePoint();
            sci.IsReadOnly = true;

            annotations = null;
            commits.Clear();
        }

        /// <summary>
        /// Displays the content of the file and the annotation data provided.
        /// </summary>
        /// <param name="annotationData">The annotation data to display on the left margin.</param>
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
                UpdateTheme(false);
                GetMarginBounds();

                sci.DwellStart += Sci_DwellStart;
                sci.DwellEnd += Sci_DwellEnd;
                ((Form) document).ContextMenuStrip = contextMenu;
            }
            finally
            {
                sci.SetSavePoint();
                sci.IsReadOnly = true;
            }
        }

        /// <summary>
        /// Shows the specified error message, or a generic message if <code>null</code> is passed.
        /// </summary>
        /// <param name="message">The error message to display instead of the content.</param>
        public void ShowError(string message = null)
        {
            sci.IsReadOnly = false;
            sci.SetText(message ?? "Error"); //TODO: Localisation
            sci.SetSavePoint();
            sci.IsReadOnly = true;
        }

        #endregion

        #region Initialization

        private void InitializeContextMenu()
        {
            showOnFileHistoryMenuItem = new ToolStripMenuItem("Show on File &History"); //TODO: Localisation
            showOnFileHistoryMenuItem.Click += ShowOnFileHistoryMenuItem_Click;

            //TODO: add more context menu items

            contextMenu.Items.AddRange(new[]
            {
                showOnFileHistoryMenuItem
            });
        }

        private void AddEventHandlers()
        {
            ((Form) document).FormClosed += Document_FormClosed;
            contextMenu.Opening += ContextMenu_Opening;
        }

        #endregion

        #region Private Functions

        private void OrganizeAnnotations()
        {
            Array.Sort(annotations, (x, y) => x.ResultLine.CompareTo(y.ResultLine));
            int i = 0;
            int offset = 0;
            int length = annotations.Length - 1;
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

        private void UpdateTheme(bool applyingTheme)
        {
            if (applyingTheme)
            {
                sci.BeginInvoke((MethodInvoker) delegate { UpdateTheme(false); });
                return;
            }
            var random = new Random();
            int fore = sci.StyleGetFore(0);
            int back = sci.StyleGetBack(0);
            int r = back >> 16 & 0xFF;
            int g = back >> 8 & 0xFF;
            int b = back & 0xFF;
            foreach (var annotation in commits.Values)
            {
                int newR = r + random.Next(0xFF) >> 1;
                int newG = g + random.Next(0xFF) >> 1;
                int newB = b + random.Next(0xFF) >> 1;
                sci.StyleSetFore(annotation.MarginStyle, fore);
                sci.StyleSetBack(annotation.MarginStyle, newR << 16 | newG << 8 | newB);
            }
        }

        private void GetMarginBounds()
        {
            MarginStart = sci.GetMarginWidthN(0) + sci.GetMarginWidthN(1) + sci.GetMarginWidthN(2) + sci.GetMarginWidthN(3);
            MarginEnd = MarginStart + sci.GetMarginWidthN(4);
        }

        private AnnotationData GetAnnotationData(int x, int y)
        {
            if (MarginStart <= x && x < MarginEnd)
            {
                int line = sci.LineFromPosition(sci.PositionFromPoint(x, y));
                // Binary search
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
            }
            return null;
        }

        #endregion

        #region Event Handlers

        private void Document_FormClosed(object sender, FormClosedEventArgs e)
        {
            Dispose();
        }

        private void ContextMenu_Opening(object sender, CancelEventArgs e)
        {
            var mousePosition = sci.PointToClient(Control.MousePosition);
            if (!disposed)
            {
                var annotationData = GetAnnotationData(mousePosition.X, mousePosition.Y);
                if (annotationData != null)
                {
                    contextMenu.Tag = annotationData;
                    return;
                }
            }
            e.Cancel = true;
            PluginBase.MainForm.EditorMenu.Show(sci, mousePosition);
        }

        private void Sci_DwellStart(ScintillaControl sender, int position, int x, int y)
        {
            if (!disposed)
            {
                var annotationData = GetAnnotationData(x, y);
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

        private void ShowOnFileHistoryMenuItem_Click(object sender, EventArgs e)
        {
            command.ShowOnFileHistory(((AnnotationData) contextMenu.Tag).Hash);
        }

        #endregion

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
                    }
                    if (contextMenu != null)
                    {
                        contextMenu.Opening -= ContextMenu_Opening;
                        showOnFileHistoryMenuItem.Click -= ShowOnFileHistoryMenuItem_Click;
                    }
                }

                command = null;
                fileName = null;
                document = null;
                sci = null;
                annotations = null;
                commits = null;
                tooltip = null;
                contextMenu = null;
                showOnFileHistoryMenuItem = null;

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

        /// <summary>
        /// Gets a simple one-line information of this <see cref="AnnotationData"/>.
        /// </summary>
        /// <param name="width">The minimum total width of characters.</param>
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

        /// <summary>
        /// Returns a detailed string representation of this <see cref="AnnotationData"/> for tooltips.
        /// </summary>
        public override string ToString()
        {
            //TODO: Localisation(?)
            return "Commit" + ": " + Hash + "\n" +
                "Author" + ": " + Author + " " + AuthorMail + "\n" +
                "Author Date" + ": " + AuthorTime + " " + AuthorTimeZone + "\n" +
                "Committer" + ": " + Committer + " " + CommitterMail + "\n" +
                "Committer Date" + ": " + CommitterTime + " " + CommitterTimeZone + "\n" +
                "Change" + ": " + (IsEdit ? "edit" : "add") + "\n" +
                (LineCount > 1 ?
                "Lines" + ": " + (ResultLine + 1) + "~" + (ResultLine + LineCount) :
                "Line" + ": " + (ResultLine + 1)) + "\n" +
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
