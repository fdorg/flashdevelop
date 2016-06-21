using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ASCompletion.Completion;
using CodeRefactor.Provider;
using PluginCore;
using PluginCore.Controls;
using PluginCore.FRService;
using ScintillaNet;
using ScintillaNet.Enums;
using Keys = System.Windows.Forms.Keys;

namespace CodeRefactor.Commands
{
    /// <summary>
    /// An asynchronously working command that enables users to rename variables in line with code.
    /// </summary>
    public class InlineRename : IDisposable, IMessageFilter
    {
        private const int MaxHistoryCount = 256;
        private const int Indicator = 0;
        private static InlineRename Current;

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

        private int start, end;
        private string oldName, newName/*, prevName*/;
        private bool includeComments;
        private bool includeStrings;
        private bool previewChanges;

        private ScintillaControl sci;
        private ITabbedDocument currentDoc;

        private ReferenceInfo currentRef;
        private ReferenceInfo[] refs;

        private DelayedExecution delayedExecution;
        private List<string> history;
        private int historyIndex;

        #region Events

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

        #endregion

        #region Static Methods

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
            if (InProgress)
            {
                Current.OnCancel();
                return true;
            }
            return false;
        }

        #endregion

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
            {
                throw new ArgumentNullException("previewTarget");
            }

            CancelCurrent();

            sci = control;
            start = position - original.Length;
            oldName = original;
            newName = original;
            //prevName = original;
            end = position;
            currentDoc = PluginBase.MainForm.CurrentDocument;
            delayedExecution = new DelayedExecution();
            history = new List<string>() { oldName };
            historyIndex = 0;
            this.includeComments = includeComments ?? false;
            this.includeStrings = includeStrings ?? false;
            this.previewChanges = previewChanges ?? false;

            sci.BeginUndoAction();
            InitializeHighlights();
            SetupLivePreview(includeComments.HasValue, includeStrings.HasValue, previewChanges.HasValue, previewTarget);
            AddMessageFilter();
            DisableControls();
            Highlight(start, end - start);
            sci.SetSel(start, end);
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

        /// <summary>
        /// Modify the highlight indicator alpha and select current word.
        /// </summary>
        private void InitializeHighlights()
        {
            sci.RemoveHighlights(Indicator);
            sci.SetIndicSetAlpha(Indicator, 100);
        }

        /// <summary>
        /// Set up required variables for live preview features.
        /// </summary>
        /// <param name="supportInsideComment">Whether searching inside comments are enabled.</param>
        /// <param name="supportInsideString">Whether searching inside strings are enabled.</param>
        /// <param name="supportPreviewChanges">Whether live preview is enabled.</param>
        /// <param name="target">Current target to rename.</param>
        private void SetupLivePreview(bool supportInsideComment, bool supportInsideString, bool supportPreviewChanges, ASResult target)
        {
            if (!supportPreviewChanges) return;

            var results = new FRRunner().SearchSync(GetConfig(oldName))[currentDoc.FileName];
            var tempRefs = new List<ReferenceInfo>();

            foreach (var match in results)
            {
                int index = match.Index;
                int length = match.Length;
                string value = match.Value;
                int style = sci.BaseStyleAt(index);
                bool insideComment = supportInsideComment && RefactoringHelper.IsCommentStyle(style);
                bool insideString = supportInsideString && RefactoringHelper.IsStringStyle(style);

                if (RefactoringHelper.DoesMatchPointToTarget(sci, match, target, null) || insideComment || insideString)
                {
                    var @ref = new ReferenceInfo() { Index = index, Length = length, Value = value };
                    tempRefs.Add(@ref);

                    if (currentRef == null && match.Index == start)
                    {
                        currentRef = @ref;
                    }
                    else if (previewChanges && (!insideComment || includeComments) && (!insideString || includeStrings))
                    {
                        Highlight(index, length);
                    }
                }
            }

            if (RenamingHelper.HasGetterSetter(target))
            {
                var list = target.Member.Parameters;
                if (list[0].Name == RenamingHelper.ParamGetter)
                {
                    AddGetterSetterPreview(tempRefs, target, RenamingHelper.PrefixGetter, oldName, supportInsideComment, supportInsideString);
                }
                if (list[1].Name == RenamingHelper.ParamSetter)
                {
                    AddGetterSetterPreview(tempRefs, target, RenamingHelper.PrefixSetter, oldName, supportInsideComment, supportInsideString);
                }
                tempRefs.Sort();
            }

            refs = tempRefs.ToArray();
        }

        private void AddGetterSetterPreview(List<ReferenceInfo> refInfos, ASResult target, string prefix, string name, bool supportInsideComment, bool supportInsideString)
        {
            target = RenamingHelper.FindGetterSetter(target, prefix + name);
            if (target == null) return;

            var results = new FRRunner().SearchSync(GetConfig(prefix + name))[currentDoc.FileName];
            int offset = prefix.Length;

            foreach (var match in results)
            {
                int index = match.Index + offset;
                int length = match.Length - offset;
                string value = match.Value.Substring(offset);
                int style = sci.BaseStyleAt(index);
                bool insideComment = supportInsideComment && RefactoringHelper.IsCommentStyle(style);
                bool insideString = supportInsideString && RefactoringHelper.IsStringStyle(style);

                if (RefactoringHelper.DoesMatchPointToTarget(sci, match, target, null) || insideComment || insideString)
                {
                    var @ref = new ReferenceInfo() { Index = index, Length = length, Value = value };
                    refInfos.Add(@ref);

                    if (previewChanges && (!insideComment || includeComments) && (!insideString || includeStrings))
                    {
                        Highlight(index, value.Length);
                    }
                }
            }
        }

        private FRConfiguration GetConfig(string name)
        {
            return new FRConfiguration(currentDoc.FileName, sci.Text, new FRSearch(name)
            {
                Filter = SearchFilter.None,
                IsRegex = false,
                IsEscaped = false,
                NoCase = false,
                WholeWord = true
            });
        }

        /// <summary>
        /// Add this <see cref="InlineRename"/> object as a message filter and handle internal
        /// messages and handle <see cref="ScintillaControl"/> events.
        /// </summary>
        private void AddMessageFilter()
        {
            Application.AddMessageFilter(this);
            sci.SelectionChanged += Sci_SelectionChanged;
            sci.TextInserted += Sci_TextInserted;
            sci.TextDeleted += Sci_TextDeleted;
            Current = this;
        }

        /// <summary>
        /// Disable main controls from <see cref="IMainForm"/>.
        /// </summary>
        private static void DisableControls()
        {
            PluginBase.MainForm.MenuStrip.Enabled = false;
            PluginBase.MainForm.ToolStrip.Enabled = false;
            PluginBase.MainForm.EditorMenu.Enabled = false;
            UITools.Manager.DisableEvents = true;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Apply current changes. 
        /// </summary>
        private void OnApply()
        {
            if (start == end || !IsValidFirstChar(sci.CharAt(start))) return;
            for (int i = start + 1; i < end; i++)
            {
                if (!IsValidChar(sci.CharAt(i))) return;
            }

            using (this)
            {
                Finish();
            }

            delayedExecution.Invoke(DelayedExecution_Apply);
        }

        /// <summary>
        /// Cancel any changes.
        /// </summary>
        private void OnCancel()
        {
            using (this)
            {
                Finish();
            }

            delayedExecution.Invoke(DelayedExecution_Cancel);
        }

        /// <summary>
        /// Update and record current changes.
        /// </summary>
        private void OnUpdate()
        {
            sci.DisableAllSciEvents = true;

            try
            {
                int pos = sci.CurrentPos;
                sci.SetSel(start, end);
                //prevName = newName;
                newName = sci.SelText;
                sci.SetSel(pos, pos);
                AddHistory(newName);
            }
            finally
            {
                sci.DisableAllSciEvents = false;
            }

            if (refs == null)
            {
                Highlight(start, end - start);
                if (Update != null) Update(this, /*prevName,*/ newName);
            }
            else
            {
                delayedExecution.Invoke(DelayedExecution_Update);
            }
        }

        /// <summary>
        /// Revert any changes and save the document to finish up.
        /// </summary>
        private void Finish()
        {
            // If the document was closed, don't do the finish-up task.
            if (Array.IndexOf(PluginBase.MainForm.Documents, currentDoc) == -1) return;

            sci.RemoveHighlights(Indicator);
            sci.SetIndicSetAlpha(Indicator, 40);

            sci.SetSel(start, end);
            newName = sci.SelText;
        }

        /// <summary>
        /// Remove this <see cref="InlineRename"/> from the system message filters, dispose any 
        /// resources used, and re-enable controls from <see cref="IMainForm"/>.
        /// </summary>
        void IDisposable.Dispose()
        {
            Application.RemoveMessageFilter(this);
            sci.SelectionChanged -= Sci_SelectionChanged;
            sci.TextInserted -= Sci_TextInserted;
            sci.TextDeleted -= Sci_TextDeleted;
            Current = null;

            currentDoc = null;
            currentRef = null;
            refs = null;

            history.Clear();
            history = null;

            PluginBase.MainForm.MenuStrip.Enabled = true;
            PluginBase.MainForm.ToolStrip.Enabled = true;
            PluginBase.MainForm.EditorMenu.Enabled = true;
            UITools.Manager.DisableEvents = false;
        }

        #endregion

        #region Updating References

        /// <summary>
        /// Update all references to the current target.
        /// </summary>
        /// <param name="replacement">Replacement string.</param>
        /// <param name="current">Whether the current reference is updated.</param>
        /// <param name="comments">Whether to update matches in comments.</param>
        /// <param name="strings">Whether to update matches in strings.</param>
        /// <param name="others">Whether to update other matches.</param>
        /// <param name="highlight">Whether to highlight the matches.</param>
        private void UpdateReferences(string replacement, bool current, bool comments, bool strings, bool others, bool highlight)
        {
            sci.DisableAllSciEvents = true;

            try
            {
                int pos = sci.CurrentPos;
                int newLength = replacement.Length;
                int delta = 0;

                pos -= start;

                for (int i = 0, l = refs.Length; i < l; i++)
                {
                    var @ref = refs[i];
                    int oldLength = @ref.Length;

                    @ref.Index += delta;
                    int s = @ref.Index;
                    int e = s + oldLength;

                    if (@ref == currentRef)
                    {
                        if (!current) continue;
                    }
                    else
                    {
                        bool replace;
                        int style = sci.BaseStyleAt(s);

                        if (RefactoringHelper.IsCommentStyle(style))
                        {
                            replace = comments;
                        }
                        else if (RefactoringHelper.IsStringStyle(style))
                        {
                            replace = strings;
                        }
                        else
                        {
                            replace = others;
                        }

                        if (!replace) continue;

                        sci.SetSel(s, e);
                        sci.ReplaceSel(replacement);
                    }

                    @ref.Length = newLength;
                    @ref.Value = replacement;
                    delta += newLength - oldLength;

                    if (highlight) Highlight(s, newLength);
                }

                start = currentRef.Index;
                end = start + currentRef.Length;

                pos += start;
                sci.SetSel(pos, pos);
            }
            finally
            {
                sci.DisableAllSciEvents = false;
            }
        }

        /// <summary>
        /// Update all references and notify.
        /// </summary>
        private void DelayedExecution_Update()
        {
            UpdateReferences(newName, true, previewChanges && includeComments, previewChanges && includeStrings, previewChanges, true);

            if (Update != null) Update(this, /*prevName,*/ newName);
        }

        /// <summary>
        /// Undo the whole action and raise apply event
        /// </summary>
        private void DelayedExecution_Apply()
        {
            delayedExecution.Dispose();
            sci.EndUndoAction();
            sci.Undo();

            if (Apply != null) Apply(this, oldName, newName);
        }

        /// <summary>
        /// Undo the whole action and raise cancel event
        /// </summary>
        private void DelayedExecution_Cancel()
        {
            delayedExecution.Dispose();
            sci.EndUndoAction();
            sci.Undo();

            if (Cancel != null) Cancel(this);
        }

        #endregion

        #region Shortcut Handlers

        /// <summary>
        /// Undo one step.
        /// </summary>
        private void PerformUndo()
        {
            if (historyIndex > 0) SafeReplace(history[--historyIndex]);
        }

        /// <summary>
        /// Redo one step.
        /// </summary>
        private void PerformRedo()
        {
            if (historyIndex + 1 < history.Count) SafeReplace(history[++historyIndex]);
        }

        /// <summary>
        /// Filter and paste any clipboard content.
        /// </summary>
        private void PerformPaste()
        {
            if (Clipboard.ContainsText() && CanWrite)
            {
                string value = Clipboard.GetText(); // fixed ???
                int length = value.Length;
                int count = 0;
                char[] validChars = new char[length]; // stackalloc ???

                for (int i = 0; i < length; i++)
                {
                    char c = value[i];
                    if (IsValidChar(c))
                    {
                        validChars[count++] = c;
                    }
                }

                value = new string(validChars, 0, count);
                if (!string.IsNullOrEmpty(value))
                {
                    sci.ReplaceSel(value);
                }
            }
        }

        /// <summary>
        /// Select the current target.
        /// </summary>
        private void PerformSelectAll()
        {
            sci.DisableAllSciEvents = true;
            sci.SetSel(start, end);
            sci.DisableAllSciEvents = false;
        }

        #endregion

        #region Scintilla Events

        /// <summary>
        /// Invoked when the selection of the <see cref="ScintillaControl"/> changes.
        /// </summary>
        /// <param name="sender">The <see cref="ScintillaControl"/> object.</param>
        private void Sci_SelectionChanged(ScintillaControl sender)
        {
            sci.DisableAllSciEvents = true;

            try
            {
                int s = sci.SelectionStart;
                int e = sci.SelectionEnd;
                int pos = sci.CurrentPos;

                if (pos < start || end < pos)
                {
                    if (!UpdateCurrentRef(pos))
                    {
                        pos = pos < start ? start : end;
                        sci.SetSel(pos, pos);
                        return;
                    }
                }

                if (s < start) sci.SetSel(start, e);
                else if (e > end) sci.SetSel(s, end);
            }
            finally
            {
                sci.DisableAllSciEvents = false;
            }
        }

        private bool UpdateCurrentRef(int pos)
        {
            for (int i = 0, length = refs.Length; i < length; i++)
            {
                var @ref = refs[i];
                int s = @ref.Index;
                int e = s + @ref.Length;
                if (s <= pos && pos <= e)
                {
                    currentRef = @ref;
                    start = s;
                    end = e;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Invoked when text is inserted on the <see cref="ScintillaControl"/> object.
        /// </summary>
        /// <param name="sender">The <see cref="ScintillaControl"/> object.</param>
        /// <param name="position">The position where text was inserted.</param>
        /// <param name="length">The length of the inserted text.</param>
        /// <param name="linesAdded">The number of lines inserted.</param>
        private void Sci_TextInserted(ScintillaControl sender, int position, int length, int linesAdded)
        {
            if (start <= position && position <= end)
            {
                end += length;
                OnUpdate();
            }
            else
            {
                OnCancel();
            }
        }

        /// <summary>
        /// Invoked when text is deleted from the <see cref="ScintillaControl"/> object.
        /// </summary>
        /// <param name="sender">The <see cref="ScintillaControl"/> object.</param>
        /// <param name="position">The position where text was deleted.</param>
        /// <param name="length">The length of the deleted text.</param>
        /// <param name="linesAdded">The number of lines inserted.</param>
        private void Sci_TextDeleted(ScintillaControl sender, int position, int length, int linesAdded)
        {
            position += length;

            if (start < position && position <= end)
            {
                end -= length;
                OnUpdate();
            }
            else
            {
                //OnCancel();
            }
        }

        #endregion

        #region Pre-Filter System Message

        /// <summary>
        /// Filters out a message before it is dispatched.
        /// </summary>
        /// <param name="m">The message to be dispatched. You cannot modify this message.</param>
        /// <returns>
        /// <code>true</code> to filter the message and stop it from being dispatched;
        /// <code>false</code> to allow the message to continue to the next filter or control.
        /// </returns>
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
                            if (!CanWrite) break;
                            return true;
                        case Keys.End:
                            if (!CanWrite) break;
                            sci.SetSel((modifier & Keys.Shift) == 0 ? end : sci.SelectionStart, end);
                            return true;
                        case Keys.Home:
                            if (!CanWrite) break;
                            sci.SetSel((modifier & Keys.Shift) == 0 ? start : sci.SelectionEnd, start);
                            return true;
                        case Keys.Tab:
                            return true;
                        default:
                            if (ToolStripManager.IsValidShortcut(key | modifier))
                            {
                                return HandleShortcuts(key | modifier);
                            }
                            break;
                    }
                    break;

                case 0x0102: //WM_CHAR
                case 0x0103: //WM_DEADCHAR
                    if (CanWrite && IsValidChar((int) m.WParam)) break;
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
            }

            return false;
        }

        /// <summary>
        /// Handles a shortcut.
        /// </summary>
        /// <param name="keys">The currently pressed keys.</param>
        /// <returns>
        /// <code>true</code> to allow the shortcut to be take the default action;
        /// <code>false</code> to filter the shortcut and take a customized action.
        /// </returns>
        private bool HandleShortcuts(Keys keys)
        {
            switch (PluginBase.MainForm.GetShortcutItemId(keys))
            {
                case "EditMenu.Paste":
                    PerformPaste();
                    break;
                case "EditMenu.Redo":
                    PerformRedo();
                    break;
                case "EditMenu.SelectAll":
                    PerformSelectAll();
                    break;
                case "EditMenu.Undo":
                    PerformUndo();
                    break;
                case "EditMenu.Copy":
                case "EditMenu.Cut":
                case "EditMenu.ToLowercase":
                case "EditMenu.ToUppercase":
                case "Scintilla.ResetZoom":
                case "Scintilla.ZoomIn":
                case "Scintilla.ZoomOut":
                    return false;
                default:
                    //string.Format("Shortcut \"{0}\" cannot be used during renaming", shortcut.Key)
                    break;
            }

            return true;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Gets a <see cref="bool"/> value specifying whether <see cref="Keys.Back"/> is currently available.
        /// </summary>
        private bool CanBackspace
        {
            get
            {
                int pos = sci.SelectionStart;
                if (start <= pos && pos <= end)
                {
                    pos = sci.SelectionEnd;
                    return start < pos && pos <= end;
                }
                return false;
            }
        }

        /// <summary>
        /// Gets a <see cref="bool"/> value specifying whether <see cref="Keys.Delete"/> is currently available.
        /// </summary>
        private bool CanDelete
        {
            get
            {
                int pos = sci.SelectionStart;
                if (start <= pos && pos <= end)
                {
                    pos = sci.SelectionEnd;
                    return start <= pos && pos < end;
                }
                return false;
            }
        }

        /// <summary>
        /// Gets a <see cref="bool"/> value specifying whether text can be inserted to the current position.
        /// </summary>
        private bool CanWrite
        {
            get
            {
                int pos = sci.SelectionStart;
                if (start <= pos && pos <= end)
                {
                    pos = sci.SelectionEnd;
                    return start <= pos && pos <= end;
                }
                return false;
            }
        }

        /// <summary>
        /// Gets a <see cref="bool"/> value specifying whether the cursor is at the start of the current target.
        /// </summary>
        private bool AtLeftmost
        {
            get { return sci.CurrentPos == start; }
        }

        /// <summary>
        /// Gets a <see cref="bool"/> value specifying whether the cursor is at the end of the current target.
        /// </summary>
        private bool AtRightmost
        {
            get { return sci.CurrentPos == end; }
        }

        /// <summary>
        /// Gets a <see cref="bool"/> value specifying whether a character is valid for an identifier.
        /// </summary>
        /// <param name="value">A character to test for validity.</param>
        /// <returns><code>true</code> if the character is valid; <code>false</code> otherwise.</returns>
        private static bool IsValidChar(int value)
        {
            return 0x61 <= value && value <= 0x7A
                || 0x40 <= value && value <= 0x5A
                || 0x30 <= value && value <= 0x39
                || value == 0x5F
                || value == 0x24;
        }

        /// <summary>
        /// Gets a <see cref="bool"/> value specifying whether a character is valid for the first character of an identifier.
        /// </summary>
        /// <param name="value">A character to test for validity.</param>
        /// <returns><code>true</code> if the character is valid; <code>false</code> otherwise.</returns>
        private static bool IsValidFirstChar(int value)
        {
            return 0x61 <= value && value <= 0x7A
                || 0x40 <= value && value <= 0x5A
                || value == 0x5F
                || value == 0x24;
        }

        /// <summary>
        /// Highlight the specified region.
        /// </summary>
        /// <param name="startIndex">The start index of the highlight.</param>
        /// <param name="length">The length of the highlight.</param>
        private void Highlight(int startIndex, int length)
        {
            int es = sci.EndStyled;
            int mask = (1 << sci.StyleBits) - 1;
            sci.SetIndicStyle(Indicator, (int) IndicatorStyle.Container);
            sci.SetIndicFore(Indicator, 0x00FF00);
            sci.CurrentIndicator = Indicator;
            sci.IndicatorFillRange(startIndex, length);
            sci.StartStyling(es, mask);
        }

        /// <summary>
        /// Record a new value to the history.
        /// </summary>
        /// <param name="value">A new value to be recorded.</param>
        private void AddHistory(string value)
        {
            // Delete redo history
            int excessCount = history.Count - ++historyIndex;
            if (excessCount > 0)
            {
                history.RemoveRange(historyIndex, excessCount);
            }

            history.Add(value);

            // Trim beginning of history
            excessCount = history.Count - MaxHistoryCount;
            if (excessCount > 0)
            {
                history.RemoveRange(0, excessCount);
                historyIndex -= excessCount;
            }
        }

        /// <summary>
        /// Replace all matches including declaration without dispatching <see cref="ScintillaControl"/> events.
        /// </summary>
        /// <param name="value">The string to replace with.</param>
        private void SafeReplace(string value)
        {
            sci.DisableAllSciEvents = true;

            try
            {
                int pos = sci.CurrentPos;
                sci.SetSel(start, end);
                sci.ReplaceSel(value);
                end = start + value.Length;
                //prevName = newName;
                newName = value;
                sci.SetSel(pos, pos);
            }
            finally
            {
                sci.DisableAllSciEvents = false;
            }

            if (refs == null)
            {
                Highlight(start, end - start);
                if (Update != null) Update(this, /*prevName,*/ newName);
            }
            else
            {
                DelayedExecution_Update();
            }
        }

        #endregion

        /// <summary>
        /// Simplified version of <see cref="SearchMatch"/>, only containing fields that are needed
        /// by <see cref="InlineRename"/>.
        /// </summary>
        private class ReferenceInfo : IComparable<ReferenceInfo>
        {
            /// <summary>
            /// The index of this reference.
            /// </summary>
            public int Index;

            /// <summary>
            /// The length of the value of this reference.
            /// </summary>
            public int Length;

            /// <summary>
            /// The value of this reference.
            /// </summary>
            public string Value;

            /// <summary>Compares the current instance with another object of the same type and
            /// returns an integer that indicates whether the current instance precedes, follows,
            /// or occurs in the same position in the sort order as the other object. </summary>
            /// <param name="other">An object to compare with this instance. </param>
            int IComparable<ReferenceInfo>.CompareTo(ReferenceInfo other)
            {
                return Index.CompareTo(other.Index);
            }
        }

        /// <summary>
        /// Used to invoke methods with a very short delay.
        /// This pattern is required since the <see cref="ScintillaControl"/> events
        /// (<code>TextInserted</code>, <code>TextDeleted</code>) do not allow the contents of the
        /// control to be modified while the event is being dispatched.
        /// </summary>
        private class DelayedExecution : IDisposable
        {
            private Action action;
            private Timer timer;

            /// <summary>
            /// Creates a new instance of <see cref="DelayedExecution"/>.
            /// </summary>
            public DelayedExecution()
            {
                timer = new Timer
                {
                    Enabled = false,
                    Interval = 1
                };
                timer.Tick += Timer_Tick;
            }

            /// <summary>
            /// Dispose this <see cref="DelayedExecution"/> instance.
            /// </summary>
            public void Dispose()
            {
                timer.Dispose();
                timer = null;
            }

            /// <summary>
            /// Invoke the specified delegate after a certain delay.
            /// </summary>
            /// <param name="callback">The delegate to invoke asynchronously.</param>
            public void Invoke(Action callback)
            {
                action = callback;
                timer.Start();
            }

            /// <summary>
            /// Occurs when the specified timer interval has elapsed and the timer is enabled.
            /// </summary>
            /// <param name="sender">The event sender object.</param>
            /// <param name="e">The event arguments.</param>
            private void Timer_Tick(object sender, EventArgs e)
            {
                timer.Stop();
                action.Invoke();
                action = null;
            }
        }
    }
}
