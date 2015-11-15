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
    /// <summary>
    /// An asynchronously working command that enables users to rename variables in line with code.
    /// </summary>
    public class InlineRename : IDisposable, IMessageFilter, IRenameHelper
    {
        const int Indicator = 0;
        static InlineRename Current;

        #region Event delegates

        /// <summary>
        /// Event delegate for <see cref="Apply"/>.
        /// </summary>
        /// <param name="sender">The <see cref="InlineRename"/> object.</param>
        /// <param name="oldName">The original name of the renaming target.</param>
        /// <param name="newName">The new name to be replaced with.</param>
        public delegate void InlineRenameApplyHandler(InlineRename sender, string oldName, string newName);

        /// <summary>
        /// Event delegate for <see cref="Update"/>.
        /// </summary>
        /// <param name="sender">The <see cref="InlineRename"/> object.</param>
        /// <param name="newName">The value that is currently entered as the new name.</param>
        public delegate void InlineRenameUpdateHandler(InlineRename sender, /*string prevName,*/ string newName);

        /// <summary>
        /// Event delegate for <see cref="Cancel"/>.
        /// </summary>
        /// <param name="sender">The <see cref="InlineRename"/> object.</param>
        public delegate void InlineRenameCancelHandler(InlineRename sender);

        #endregion

        int start, end;
        string oldName, newName/*, prevName*/;
        bool includeComments;
        bool includeStrings;
        bool previewChanges;
        ScintillaControl sci;
        InlineRenameDialog dialog;
        ITabbedDocument currentDoc;
        Control.ControlCollection controls;
        DelayedExecution delayedExecution;
        ReferenceInfo declaration;
        List<ReferenceInfo> refs;

        /// <summary>
        /// Occurs when the user clicks the Apply button or presses Enter.
        /// </summary>
        public event InlineRenameApplyHandler Apply;

        /// <summary>
        /// Occurs when the target name is changed by the user.
        /// </summary>
        public event InlineRenameUpdateHandler Update;

        /// <summary>
        /// Occurs when the user clicks the Cancel button, presses Escape or edits the content of
        /// the <see cref="ScintillaControl"/> outside the boundary of target name.
        /// </summary>
        public event InlineRenameCancelHandler Cancel;

        /// <summary>
        /// Gets a value specifying whether there is an <see cref="InlineRename"/> object currently
        /// working in progress.
        /// </summary>
        public static bool InProgress
        {
            get { return Current != null; }
        }

        /// <summary>
        /// Cancels any existing <see cref="InlineRename"/> in progress, and returns a
        /// <see cref="bool"/> value specifying whether an existing progress was canceled.
        /// </summary>
        /// <returns> Returns true if there was an <see cref="InlineRename"/> in progress. False otherwise.</returns>
        public static bool CancelCurrent()
        {
            if (Current == null) return false;
            Current.OnCancel();
            return true;
        }

        /// <summary>
        /// Creates a new instance of <see cref="InlineRename"/>. Any existing instance in progress
        /// is automatically canceled.
        /// </summary>
        /// <param name="control">The <see cref="ScintillaControl"/> object.</param>
        /// <param name="original">The original name of the target.</param>
        /// <param name="position">Word end position of the target.</param>
        /// <param name="includeComments">Whether to initially include comments in search. Pass <code>null</code> to disable this option.</param>
        /// <param name="includeStrings">Whether to initially include strings in search. Pass <code>null</code> to disable this option.</param>
        /// <param name="previewChanges">Whether to initially preview changes during renaming. Pass <code>null</code> to disable this option.</param>
        /// <param name="previewTarget">An <see cref="ASResult"/> object specifying the target. This parameter must not be <code>null</code> if <code>previewChanges</code> is not <code>null</code>.</param>
        public InlineRename(ScintillaControl control, string original, int position, bool? includeComments, bool? includeStrings, bool? previewChanges, ASResult previewTarget)
        {
            if (previewChanges.HasValue && previewTarget == null)
                throw new ArgumentNullException("previewTarget");

            if (InProgress)
                Current.OnCancel();

            sci = control;
            start = position - original.Length;
            oldName = original;
            newName = original;
            //prevName = original;
            end = position;

            InitializeHighlights();
            SetupDelayedExecution();
            CreateDialog(includeComments, includeStrings, previewChanges);
            SetupLivePreview(includeComments.HasValue, includeStrings.HasValue, previewChanges.HasValue, previewTarget);
            AddMessageFilter();
            Highlight(start, end);
        }

        #region Retrieve Rename Options

        /// <summary>
        /// Gets a value specifying whether current renaming process includes comments.
        /// </summary>
        public bool IncludeComments
        {
            get { return includeComments; }
        }

        /// <summary>
        /// Gets a value specifying whether current renaming process includes strings.
        /// </summary>
        public bool IncludeStrings
        {
            get { return includeStrings; }
        }

        /// <summary>
        /// Gets a value specifying whether current renaming previews changes.
        /// </summary>
        public bool PreviewChanges
        {
            get { return previewChanges; }
        }

        #endregion

        #region Initialization

        void InitializeHighlights()
        {
            sci.SetSel(start, end);
            sci.RemoveHighlights(Indicator);
            sci.SetIndicSetAlpha(Indicator, 100);
        }

        void SetupDelayedExecution()
        {
            delayedExecution = new DelayedExecution();
        }

        void CreateDialog(bool? comments, bool? strings, bool? preview)
        {
            currentDoc = PluginBase.MainForm.CurrentDocument;
            dialog = new InlineRenameDialog(oldName, comments, strings, preview);
            dialog.Left = sci.Width - dialog.Width - 17;
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

        void SetupLivePreview(bool supportInsideComment, bool supportInsideString, bool supportPreviewChanges, ASResult target)
        {
            if (!supportPreviewChanges) return;

            string file = currentDoc.FileName;
            var search = new FRSearch(oldName)
            {
                Filter = SearchFilter.None,
                IsRegex = false,
                IsEscaped = false,
                NoCase = false,
                WholeWord = true
            };
            var config = new FRConfiguration(file, sci.GetText(sci.Length), search);
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
                    || insideComment && supportInsideComment
                    || insideString && supportInsideString)
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
            sci.SelectionChanged += Sci_SelectionChanged;
            sci.TextInserted += Sci_TextInserted;
            sci.TextDeleted += Sci_TextDeleted;
            Current = this;
        }

        #endregion

        #region States

        void OnApply()
        {
            using (this)
            {
                Finish();
            }

            if (Apply != null) Apply(this, oldName, newName);
        }

        void OnCancel()
        {
            using (this)
            {
                Finish();
            }

            if (Cancel != null) Cancel(this);
        }

        void OnUpdate()
        {
            sci.DisableAllSciEvents = true;

            try
            {
                int pos = sci.CurrentPos;
                sci.SetSel(start, end);
                //prevName = newName;
                newName = sci.SelText;
                sci.SetSel(pos, pos);
            }
            finally
            {
                sci.DisableAllSciEvents = false;
            }

            if (refs == null)
            {
                Highlight(start, end);
                if (Update != null) Update(this, /*prevName,*/ newName);
            }
            else
            {
                delayedExecution.Start(DelayedExecution_Update);
            }
        }

        void Finish()
        {
            sci.RemoveHighlights(Indicator);
            sci.SetIndicSetAlpha(Indicator, 40);

            sci.DisableAllSciEvents = true;

            try
            {
                sci.SetSel(start, end);
                newName = sci.SelText;

                if (newName == oldName)
                    sci.SetSel(end, end);
                else
                    sci.ReplaceSel(oldName);
            }
            finally
            {
                sci.DisableAllSciEvents = false;
            }

            if (refs != null)
                UpdateReferences(oldName, true, includeComments, includeStrings, true, false);

            currentDoc.Save();
        }

        void Highlight(int start, int end)
        {
            int es = sci.EndStyled;
            int mask = (1 << sci.StyleBits) - 1;
            sci.SetIndicStyle(Indicator, (int) IndicatorStyle.Container);
            sci.SetIndicFore(Indicator, 0x00FF00);
            sci.CurrentIndicator = Indicator;
            sci.IndicatorFillRange(start, end - start);
            sci.StartStyling(es, mask);
        }

        void IDisposable.Dispose()
        {
            Application.RemoveMessageFilter(this);
            sci.SelectionChanged -= Sci_SelectionChanged;
            sci.TextInserted -= Sci_TextInserted;
            sci.TextDeleted -= Sci_TextDeleted;
            sci = null;
            Current = null;
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

        void UpdateReferences(string replacement, bool decl, bool comments, bool strings, bool others, bool addHighlight)
        {
            sci.BeginUndoAction();
            sci.DisableAllSciEvents = true;

            try
            {
                int pos = sci.CurrentPos;
                int newLength = replacement.Length;
                int delta = 0;

                if (pos < start) pos = 0;
                else if (pos > end) pos = end - start;
                else pos -= start;

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
                        bool replace;
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
                sci.EndUndoAction();
                sci.DisableAllSciEvents = false;
            }
        }

        void DelayedExecution_Cancel()
        {
            sci.Undo();
            OnCancel();
        }

        void DelayedExecution_Update()
        {
            UpdateReferences(newName, true, previewChanges && includeComments, previewChanges && includeStrings, previewChanges, true);

            if (Update != null)
                Update(this, /*prevName,*/ newName);
        }

        void IncludeComments_CheckedChanged(object sender, EventArgs e)
        {
            includeComments = dialog.IncludeComments.Checked;

            if (previewChanges)
            {
                UpdateReferences(includeComments ? newName : oldName, false, true, false, false, includeComments);
            }

            sci.Focus();
        }

        void IncludeStrings_CheckedChanged(object sender, EventArgs e)
        {
            includeStrings = dialog.IncludeStrings.Checked;

            if (previewChanges)
            {
                UpdateReferences(includeStrings ? newName : oldName, false, false, true, false, includeStrings);
            }

            sci.Focus();
        }

        void PreviewChanges_CheckedChanged(object sender, EventArgs e)
        {
            previewChanges = dialog.PreviewChanges.Checked;
            UpdateReferences(previewChanges ? newName : oldName, false, includeComments, includeStrings, true, previewChanges);

            sci.Focus();
        }

        void ApplyButton_Click(object sender, EventArgs e)
        {
            OnApply();
        }

        void CancelButton_Click(object sender, EventArgs e)
        {
            OnCancel();
        }

        #endregion

        #region Scintilla events

        void Sci_SelectionChanged(ScintillaControl sender)
        {
            sci.DisableAllSciEvents = true;

            try
            {
                int s = sci.SelectionStart;
                int e = sci.SelectionEnd;

                //When selecting all, select the target text instead
                if (sci.SelTextSize == sci.Length)
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
            finally
            {
                sci.DisableAllSciEvents = false;
            }
        }

        void Sci_TextInserted(ScintillaControl sender, int position, int length, int linesAdded)
        {
            if (start <= position && position <= end)
            {
                end += length;
                OnUpdate();
            }
            else
            {
                delayedExecution.Start(DelayedExecution_Cancel);
            }
        }

        void Sci_TextDeleted(ScintillaControl sender, int position, int length, int linesAdded)
        {
            position += length;

            if (start < position && position <= end)
            {
                end -= length;
                OnUpdate();
            }
            else
            {
                delayedExecution.Start(DelayedExecution_Cancel);
            }
        }

        #endregion

        #region Pre-Filter System Message

        bool IMessageFilter.PreFilterMessage(ref Message m)
        {
            if (PluginBase.MainForm.CurrentDocument != currentDoc)
            {
                OnCancel();
                return false;
            }

            if (m.Msg == Win32.WM_KEYDOWN)
            {
                switch ((int) m.WParam)
                {
                    case 0x1B: //VK_ESCAPE
                        OnCancel();
                        return true;
                    case 0x0D: //VK_RETURN
                        OnApply();
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

        /// <summary>
        /// Simplified version of <see cref="SearchMatch"/>, only containing fields that are needed
        /// by <see cref="InlineRename"/>.
        /// </summary>
        class ReferenceInfo
        {
            public int Index;
            public string Value;
        }

        /// <summary>
        /// Used to invoke methods with a very short delay.
        /// This pattern is required since the <see cref="ScintillaControl"/> events
        /// (<code>TextInserted</code>, <code>TextDeleted</code>) do not allow the contents of the
        /// control to be modified while the event is being dispatched.
        /// </summary>
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
                action.Invoke();
                action = null;
            }
        }
    }
}
