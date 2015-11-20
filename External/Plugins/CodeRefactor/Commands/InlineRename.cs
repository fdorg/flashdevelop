using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using ASCompletion.Completion;
using ASCompletion.Context;
using CodeRefactor.Controls;
using CodeRefactor.Provider;
using PluginCore;
using PluginCore.FRService;
using ScintillaNet;
using ScintillaNet.Enums;
using Keys = System.Windows.Forms.Keys;

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
        KeyValuePair<string, Keys>[] shortcuts;

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

        #region Constructor

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

            InitializeFields();
            InitializeHighlights();
            CreateDialog(includeComments, includeStrings, previewChanges);
            SetupLivePreview(includeComments.HasValue, includeStrings.HasValue, previewChanges.HasValue, previewTarget);
            AddMessageFilter();
            DisableControls();
            Highlight(start, end);
        }

        #endregion

        #region Rename Options

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

        void InitializeFields()
        {
            delayedExecution = new DelayedExecution();
            shortcuts = PluginBase.MainForm.GetShortcutItems();
        }
        
        void InitializeHighlights()
        {
            sci.SetSel(start, end);
            sci.RemoveHighlights(Indicator);
            sci.SetIndicSetAlpha(Indicator, 100);
        }

        void CreateDialog(bool? comments, bool? strings, bool? preview)
        {
            currentDoc = PluginBase.MainForm.CurrentDocument;
            dialog = new InlineRenameDialog(oldName, comments, strings, preview);
            Sci_Resize(null, null);
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
            var config = new FRConfiguration(file, sci.Text, new FRSearch(oldName)
            {
                Filter = SearchFilter.None,
                IsRegex = false,
                IsEscaped = false,
                NoCase = false,
                WholeWord = true
            });
            var results = new FRRunner().SearchSync(config)[file];
            refs = new List<ReferenceInfo>();

            foreach (var match in results)
            {
                int index = match.Index;
                string value = match.Value;
                int style = sci.BaseStyleAt(index);
                bool insideComment = RefactoringHelper.IsCommentStyle(style);
                bool insideString = RefactoringHelper.IsStringStyle(style);

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
            sci.Resize += Sci_Resize;
            Current = this;
        }

        void DisableControls()
        {
            PluginBase.MainForm.MenuStrip.Enabled = false;
            PluginBase.MainForm.ToolStrip.Enabled = false;
            PluginBase.MainForm.EditorMenu.Enabled = false;
        }

        #endregion

        #region Methods

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

            if (previewChanges)
                UpdateReferences(oldName, true, includeComments, includeStrings, true, false);
            
            currentDoc.Save();
            ASContext.Context.UpdateCurrentFile(true);
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
            sci.Resize -= Sci_Resize;
            //sci = null;
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

            PluginBase.MainForm.MenuStrip.Enabled = true;
            PluginBase.MainForm.ToolStrip.Enabled = true;
            PluginBase.MainForm.EditorMenu.Enabled = true;
        }

        #endregion

        #region Updating References

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

                        if (RefactoringHelper.IsCommentStyle(style))
                            replace = comments;
                        else if (RefactoringHelper.IsStringStyle(style))
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

        #region Shortcut Handlers

        void PerformUndo()
        {
            
        }

        void PerformRedo()
        {
            
        }

        void PerformPaste()
        {
            if (Clipboard.ContainsText())
            {
                if (!CanWrite) return;
                string value = Regex.Replace(Clipboard.GetText(), @"\s", string.Empty);
                if (string.IsNullOrEmpty(value)) return;
                foreach (char i in value)
                {
                    if (!IsValidChar(i)) return;
                }
                sci.ReplaceSel(value);
            }
        }

        void PerformSelectAll()
        {
            sci.DisableAllSciEvents = true;
            sci.SetSel(start, end);
            sci.DisableAllSciEvents = false;
        }

        #endregion

        #region Scintilla Events

        void Sci_SelectionChanged(ScintillaControl sender)
        {
            sci.DisableAllSciEvents = true;

            try
            {
                int s = sci.SelectionStart;
                int e = sci.SelectionEnd;

                if (sci.CurrentPos == e)
                {
                    if (s < start) { /*if (e > start) sci.SetSel(s, start);*/ }
                    else if (s < end) { if (e > end) sci.SetSel(s, end); }
                }
                else
                {
                    if (e > end) { /*if (s < end) sci.SetSel(e, end);*/ }
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
                // throw exception?
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
                // throw exception?
            }
        }

        void Sci_Resize(object sender, EventArgs e)
        {
            dialog.Left = sci.Width - dialog.Width - 17;
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

            switch (m.Msg)
            {
                case 0x0100: //WM_KEYDOWN
                case 0x0104: //WM_SYSKEYDOWN
                    var key = (Keys) (int) m.WParam;
                    var modifier = Control.ModifierKeys;
                    switch (key)
                    {
                        case Keys.Escape:
                            OnCancel();
                            return true;
                        case Keys.Enter:
                            OnApply();
                            return true;
                        case Keys.Back:
                            if (CanBackspace) break;
                            return true;
                        case Keys.Left:
                            if (!AtLeftmost) break;
                            return true;
                        case Keys.Delete:
                            if (CanDelete) break;
                            return true;
                        case Keys.Right:
                            if (!AtRightmost) break;
                            if (sci.SelTextSize != 0) sci.SetSel(end, end);
                            return true;
                        case Keys.PageUp:
                        case Keys.PageDown:
                        case Keys.Up:
                        case Keys.Down:
                            if (OutsideRange) break;
                            return true;
                        case Keys.End:
                            if (OutsideRange) break;
                            sci.SetSel(end, end);
                            return true;
                        case Keys.Home:
                            if (OutsideRange) break;
                            sci.SetSel(start, start);
                            return true;
                        case Keys.Tab:
                            return true;
                        default:
                            if (Keys.F1 <= key && key <= Keys.F24 || (modifier & Keys.Control) != 0 || (modifier & Keys.Alt) != 0)
                                return !HandleShortcuts(key | modifier);
                            break;
                    }
                    break;

                case 0x0102: //WM_CHAR
                case 0x0103: //WM_DEADCHAR
                    int value = (int) m.WParam;
                    if (CanWrite && IsValidChar(value)) break;
                    return true;

                //case 0x0200: //WM_MOUSEMOVE
                //case 0x0201: //WM_LBUTTONDOWN
                //case 0x0202: //WM_LBUTTONUP
                //case 0x0203: //WM_LBUTTONDBCLICK
                //case 0x0204: //WM_RBUTTONDOWN
                //case 0x0205: //WM_RBUTTONUP
                //case 0x0206: //WM_RBUTTONDBCLICK
                //case 0x0207: //WM_MBUTTONDOWN
                //case 0x0208: //WM_MBUTTONUP
                //case 0x0209: //WM_MBUTTONDBCLICK
                //    if (sci.ClientRectangle.Contains(sci.PointToClient(Control.MousePosition))) break;
                //    return true;

                //case 0x007B: //WM_CONTEXTMENU
                //    return true;

                case 0x010D: //WM_IME_STARTCOMPOSITION
                case 0x010E: //WM_IME_ENDCOMPOSITION
                case 0x010F: //WM_IME_COMPOSITION
                    return true;
            }

            return false;
        }

        bool HandleShortcuts(Keys keys)
        {
            foreach (var shortcut in shortcuts)
            {
                if (shortcut.Value == keys)
                {
                    switch (shortcut.Key)
                    {
                        case "EditMenu.Copy":
                        case "EditMenu.Cut":
                            return true;
                        case "EditMenu.Paste":
                            PerformPaste();
                            break;
                        case "EditMenu.Redo":
                            PerformRedo();
                            break;
                        case "EditMenu.SelectAll":
                            PerformSelectAll();
                            break;
                        case "EditMenu.ToLowercase":
                        case "EditMenu.ToUppercase":
                            return true;
                        case "EditMenu.Undo":
                            PerformUndo();
                            break;
                        case "Scintilla.ResetZoom":
                        case "Scintilla.ZoomIn":
                        case "Scintilla.ZoomOut":
                            return true;
                        default:
                            //"Shortcut \"{0}\" cannot be used during renaming", shortcut.Key
                            break;
                    }

                    break;
                }
            }

            return false;
        }

        #endregion

        #region Utilities

        bool CanBackspace
        {
            get
            {
                int pos = sci.SelectionStart;
                if (pos < start || end < pos) return false;
                pos = sci.SelectionEnd;
                return start < pos && pos <= end;
            }
        }

        bool CanDelete
        {
            get
            {
                int pos = sci.SelectionStart;
                if (pos < start || end < pos) return false;
                pos = sci.SelectionEnd;
                return start <= pos && pos < end;
            }
        }

        bool CanWrite
        {
            get
            {
                int pos = sci.SelectionStart;
                if (pos < start || end < pos) return false;
                pos = sci.SelectionEnd;
                return start <= pos && pos <= end;
            }
        }

        bool AtLeftmost
        {
            get { return sci.CurrentPos == start; }
        }

        bool AtRightmost
        {
            get { return sci.CurrentPos == end; }
        }

        bool OutsideRange
        {
            get { return sci.CurrentPos < start || end < sci.CurrentPos; }
        }

        bool IsValidChar(int value)
        {
            return 0x60 < value && value < 0x7B
                || 0x3F < value && value < 0x5B
                || 0x29 < value && value < 0x3A
                || value == 0x5F
                || value == 0x24;
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
