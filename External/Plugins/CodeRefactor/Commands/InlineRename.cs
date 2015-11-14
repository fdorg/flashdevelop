using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ASCompletion.Completion;
using CodeRefactor.Controls;
using CodeRefactor.Provider;
using PluginCore;
using PluginCore.FRService;
using ScintillaNet;
using ScintillaNet.Enums;

namespace CodeRefactor.Commands
{
    public class InlineRename : IMessageFilter, IDisposable, IRenameHelper
    {
        const int indicator = 0;
        static InlineRename current;

        ScintillaControl sci;
        InlineRenameDialog dialog;
        Control.ControlCollection controls;
        string oldName, newName, prevName;
        int start, end;
        bool includeComments;
        bool includeStrings;
        bool previewChanges;

        ITabbedDocument currentDoc;
        List<ReferenceInfo> refs;
        ReferenceInfo declaration;
        DelayedExecution delayedExecution;

        public event Action<string, string> OnRename;
        public event Action<string, string> OnUpdate;
        public event Action OnCancel;

        public static bool InProgress
        {
            get { return current != null; }
        }

        public static bool CancelCurrent()
        {
            if (current == null) return false;
            current.Cancel();
            return true;
        }

        public InlineRename(ScintillaControl control, string original, int position, bool? includeComments, bool? includeStrings, bool? previewChanges, ASResult previewTarget)
        {
            if (InProgress)
                current.Cancel();

            sci = control;
            start = position - original.Length;
            oldName = original;
            newName = original;
            prevName = original;
            end = position;

            currentDoc = PluginBase.MainForm.CurrentDocument;

            InitializeHighlights();
            SetupDelayedExecution();
            CreateDialog(includeComments, includeStrings, previewChanges);
            SetupLivePreview(previewTarget);
            AddMessageFilter();
            Highlight(start, end);
        }

        public bool IncludeComments
        {
            get { return includeComments; }
        }

        public bool IncludeStrings
        {
            get { return includeStrings; }
        }

        public bool PreviewChanges
        {
            get { return previewChanges; }
        }

        #region Initialization

        void InitializeHighlights()
        {
            sci.SetSel(start, end);
            sci.RemoveHighlights(indicator);
            sci.SetIndicSetAlpha(indicator, 100);
        }

        void SetupDelayedExecution()
        {
            delayedExecution = new DelayedExecution();
        }

        void CreateDialog(bool? comments, bool? strings, bool? preview)
        {
            dialog = new InlineRenameDialog(oldName, comments, strings, preview);
            dialog.Left = sci.Width - dialog.Width;
            controls = currentDoc.SplitContainer.Parent.Controls;
            controls.Add(dialog);
            controls.SetChildIndex(dialog, 0);

            dialog.IncludeComments.CheckedChanged += IncludeComments_CheckedChanged;
            dialog.IncludeStrings.CheckedChanged += IncludeStrings_CheckedChanged;
            dialog.PreviewChanges.CheckedChanged += PreviewChanges_CheckedChanged;
            dialog.ApplyButton.Click += ApplyButton_Click;
            dialog.CancelButton.Click += CancelButton_Click;

            includeComments = dialog.IncludeComments.Checked;
            includeStrings = dialog.IncludeStrings.Checked;
            previewChanges = dialog.PreviewChanges.Checked;
        }

        void SetupLivePreview(ASResult target)
        {
            if (target == null) return;

            string file = currentDoc.FileName;
            var search = new FRSearch(oldName)
            {
                Filter = SearchFilter.None,
                IsRegex = false,
                IsEscaped = false,
                NoCase = false,
                WholeWord = true
            };
            var config = new FRConfiguration(file, sci.Text, search);
            var runner = new FRRunner();
            var results = runner.SearchSync(config)[file];
            refs = new List<ReferenceInfo>();

            foreach (var match in results)
            {
                int index = match.Index;
                string value = match.Value;
                int style = sci.BaseStyleAt(index);
                bool insideComment = RefactoringHelper.IsInsideComment(style);
                bool insideString = RefactoringHelper.IsInsideString(style);

                if (RefactoringHelper.DoesMatchPointToTarget(sci, match, target, null)
                    || insideComment || insideString)
                {
                    var @ref = new ReferenceInfo { Index = index, Value = value };
                    refs.Add(@ref);

                    if (declaration == null && match.Index == start)
                    {
                        declaration = @ref;
                    }
                    else if (previewChanges && (!insideComment || includeComments) && (!insideString || includeStrings))
                    {
                        Highlight(index, index + value.Length);
                    }
                }
            }
        }

        void AddMessageFilter()
        {
            Application.AddMessageFilter(this);
            sci.SelectionChanged += OnSelectionChanged;
            sci.TextInserted += OnTextInserted;
            sci.TextDeleted += OnTextDeleted;
            current = this;
        }

        #endregion

        #region States

        void Rename()
        {
            using (this)
            {
                Finish();
            }

            if (OnRename != null) OnRename(oldName, newName);
        }

        void Cancel()
        {
            using (this)
            {
                Finish();
            }

            if (OnCancel != null) OnCancel();
        }

        void Update()
        {
            int pos = sci.CurrentPos;
            sci.SetSel(start, end);
            prevName = newName;
            newName = sci.SelText;
            sci.SetSel(pos, pos);

            if (refs == null)
            {
                Highlight(start, end);
                if (OnUpdate != null) OnUpdate(prevName, newName);
            }
            else
            {
                delayedExecution.Start(DelayedUpdate);
            }
        }

        void Finish()
        {
            sci.RemoveHighlights(indicator);
            sci.SetIndicSetAlpha(indicator, 40);

            sci.SetSel(start, end);
            newName = sci.SelText;

            sci.DisableAllSciEvents = true;

            if (newName == oldName)
                sci.SetSel(end, end);
            else
                sci.ReplaceSel(oldName);

            if (refs != null)
                UpdateReferences(oldName, true, includeComments, includeStrings, true, false);

            sci.DisableAllSciEvents = false;
            currentDoc.Save();
        }

        void Highlight(int start, int end)
        {
            int es = sci.EndStyled;
            int mask = (1 << sci.StyleBits) - 1;
            sci.SetIndicStyle(indicator, (int) IndicatorStyle.Container);
            sci.SetIndicFore(indicator, 0x00FF00);
            sci.CurrentIndicator = indicator;
            sci.IndicatorFillRange(start, end - start);
            sci.StartStyling(es, mask);
        }

        void IDisposable.Dispose()
        {
            Application.RemoveMessageFilter(this);
            sci.SelectionChanged -= OnSelectionChanged;
            sci.TextInserted -= OnTextInserted;
            sci.TextDeleted -= OnTextDeleted;
            sci = null;
            current = null;
            currentDoc = null;

            controls.Remove(dialog);
            controls = null;

            dialog.Dispose();
            dialog = null;

            delayedExecution.Dispose();
            delayedExecution = null;

            if (refs != null)
            {
                refs.Clear();
                refs = null;
            }
        }

        #endregion

        #region Updating references

        void DelayedCancel()
        {
            sci.Undo();
            Cancel();
        }

        void DelayedUpdate()
        {
            UpdateReferences(newName, true, previewChanges && includeComments, previewChanges && includeStrings, previewChanges, true);

            if (OnUpdate != null)
                OnUpdate(prevName, newName);
        }

        void UpdateReferences(string replacement, bool decl, bool comments, bool strings, bool others, bool addHighlight)
        {
            sci.DisableAllSciEvents = true;
            sci.BeginUndoAction();

            try
            {
                int pos = sci.CurrentPos - start;
                int newLength = replacement.Length;
                int delta = 0;

                for (int i = 0, l = refs.Count; i < l; i++)
                {
                    var @ref = refs[i];
                    int oldIndex = @ref.Index;
                    int oldLength = @ref.Value.Length;

                    @ref.Index += delta;
                    int start = @ref.Index;
                    int end = start + oldLength;

                    if (@ref == declaration)
                    {
                        if (!decl) continue;
                    }
                    else
                    {
                        bool replace = false;
                        int style = sci.BaseStyleAt(start);

                        if (RefactoringHelper.IsInsideComment(style))
                            replace = comments;
                        else if (RefactoringHelper.IsInsideString(style))
                            replace = strings;
                        else
                            replace = others;

                        if (!replace) continue;

                        sci.SetSel(start, end);
                        sci.ReplaceSel(replacement);
                    }

                    @ref.Value = replacement;
                    delta += newLength - oldLength;

                    if (addHighlight)
                        Highlight(start, start + newLength);
                }

                start = declaration.Index;
                end = start + declaration.Value.Length;

                pos += start;
                sci.SetSel(pos, pos);
            }
            finally
            {
                sci.DisableAllSciEvents = false;
                sci.EndUndoAction();
            }
        }

        void IncludeComments_CheckedChanged(object sender, EventArgs e)
        {
            includeComments = dialog.IncludeComments.Checked;

            if (previewChanges)
            {
                if (includeComments)
                    UpdateReferences(newName, false, true, false, false, true);
                else
                    UpdateReferences(oldName, false, true, false, false, false);
            }

            sci.Focus();
        }

        void IncludeStrings_CheckedChanged(object sender, EventArgs e)
        {
            includeStrings = dialog.IncludeStrings.Checked;

            if (previewChanges)
            {
                if (includeStrings)
                    UpdateReferences(newName, false, false, true, false, true);
                else
                    UpdateReferences(oldName, false, false, true, false, false);
            }

            sci.Focus();
        }

        void PreviewChanges_CheckedChanged(object sender, EventArgs e)
        {
            previewChanges = dialog.PreviewChanges.Checked;

            UpdateReferences(previewChanges ? newName : oldName, false, includeComments, includeStrings, true, previewChanges);

            sci.Focus();
        }

        void CancelButton_Click(object sender, EventArgs e)
        {
            Cancel();
        }

        void ApplyButton_Click(object sender, EventArgs e)
        {
            Rename();
        }

        #endregion

        #region Scintilla events

        void OnSelectionChanged(ScintillaControl sender)
        {
            int s = sci.SelectionStart;
            int e = sci.SelectionEnd;

            if (sci.SelTextSize == sci.TextLength)
            {
                sci.SetSel(start, end);
                return;
            }

            if (sci.CurrentPos == e)
            {
                if (s < start) { if (e > start) sci.SetSel(s, start); }
                else if (s < end) { if (e > end) sci.SetSel(s, end); }
            }
            else
            {
                if (e > end) { if (s < end) sci.SetSel(e, end); }
                else if (e > start) { if (s < start) sci.SetSel(e, start); }
            }
        }

        void OnTextInserted(ScintillaControl sender, int position, int length, int linesAdded)
        {
            if (start <= position && position <= end)
            {
                end += length;
                Update();
            }
            else
            {
                if (position < start)
                {
                    start += length;
                    end += length;
                }

                delayedExecution.Start(DelayedCancel);
            }
        }

        void OnTextDeleted(ScintillaControl sender, int position, int length, int linesAdded)
        {
            position += length;

            if (start < position && position <= end)
            {
                end -= length;
                Update();
            }
            else
            {
                if (position <= start)
                {
                    start -= length;
                    end -= length;
                }

                delayedExecution.Start(DelayedCancel);
            }
        }

        #endregion

        #region Pre-Filter System Message

        bool IMessageFilter.PreFilterMessage(ref Message m)
        {
            if (PluginBase.MainForm.CurrentDocument != currentDoc)
            {
                Cancel();
                return false;
            }

            if (m.Msg == Win32.WM_KEYDOWN)
            {
                switch ((int) m.WParam)
                {
                    case 0x1B: //VK_ESCAPE
                        Cancel();
                        return true;
                    case 0x0D: //VK_RETURN
                        Rename();
                        return true;
                    case 0x08: //VK_BACK
                        if (!AtLeftmost) break;
                        if (sci.SelTextSize != 0) break;
                        return true;
                    case 0x25: //VK_LEFT
                        if (!AtLeftmost) break;
                        return true;
                    case 0x2E: //VK_DELETE
                        if (!AtRightmost) break;
                        if (sci.SelTextSize != 0) break;
                        return true;
                    case 0x27: //VK_RIGHT
                        if (!AtRightmost) break;
                        if (sci.SelTextSize != 0) sci.SetSel(end, end);
                        return true;
                    case 0x21: //VK_PRIOR
                    case 0x22: //VK_NEXY
                    case 0x26: //VK_UP
                    case 0x28: //VK_DOWN
                        if (!WithinRange) break;
                        return true;
                    case 0x23: //VK_END
                        if (!WithinRange) break;
                        sci.SetSel(end, end);
                        return true;
                    case 0x24: //VK_HOME
                        if (!WithinRange) break;
                        sci.SetSel(start, start);
                        return true;
                }
            }

            return false;
        }

        #endregion

        #region Utilities

        bool AtLeftmost
        {
            get { return sci.CurrentPos == start; }
        }

        bool AtRightmost
        {
            get { return sci.CurrentPos == end; }
        }

        bool WithinRange
        {
            get { return sci.CurrentPos >= start && sci.CurrentPos <= end; }
        }

        #endregion

        class ReferenceInfo
        {
            public int Index;
            public string Value;
        }

        class DelayedExecution : IDisposable
        {
            Action action;
            Timer timer;

            public DelayedExecution()
            {
                timer = new Timer();
                timer.Enabled = false;
                timer.Interval = 1;
                timer.Tick += Timer_Tick;
            }

            public void Dispose()
            {
                timer.Dispose();
                timer = null;
            }

            public void Start(Action callback)
            {
                action = callback;
                timer.Start();
            }

            void Timer_Tick(object sender, EventArgs e)
            {
                timer.Stop();
                action();
                action = null;
            }
        }
    }
}
