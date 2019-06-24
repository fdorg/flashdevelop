using System;
using System.IO;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using WeifenLuo.WinFormsUI.Docking;
using PluginCore.Localization;
using FlashDevelop.Managers;
using FlashDevelop.Controls;
using PluginCore.Utilities;
using PluginCore.Managers;
using PluginCore.Helpers;
using PluginCore.Controls;
using ScintillaNet;
using PluginCore;

namespace FlashDevelop.Docking
{
    public class TabbedDocument : DockContent, ITabbedDocument
    {
        private readonly Timer focusTimer;
        private Timer backupTimer;
        private string previousText;
        private readonly List<int> bookmarks;
        private ScintillaControl lastEditor;
        private bool isModified;
        private FileInfo fileInfo;

        public TabbedDocument()
        {
            focusTimer = new Timer();
            focusTimer.Interval = 100;
            bookmarks = new List<int>();
            focusTimer.Tick += OnFocusTimer;
            ControlAdded += DocumentControlAdded;
            UITools.Manager.OnMarkerChanged += OnMarkerChanged;
            DockPanel = Globals.MainForm.DockPanel;
            Font = Globals.Settings.DefaultFont;
            DockAreas = DockAreas.Document;
            BackColor = Color.White;
            UseCustomIcon = false;
            StartBackupTiming();
        }

        /// <summary>
        /// Disables the automatic update of the icon
        /// </summary>
        public bool UseCustomIcon { get; set; }

        /// <summary>
        /// Path of the document
        /// </summary>
        public string FileName => IsEditable ? SciControl.FileName : null;

        /// <summary>
        /// Do we contain a Browser control?
        /// </summary>
        public bool IsBrowsable
        {
            get
            {
                foreach (Control ctrl in Controls)
                {
                    if (ctrl is Browser) return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Do we contain a ScintillaControl?
        /// </summary>
        public bool IsEditable => SciControl != null;

        /// <summary>
        /// Are we splitted in to two sci controls?
        /// </summary>
        public bool IsSplitted
        {
            get => IsEditable && !SplitContainer.Panel2Collapsed;
            set
            {
                if (IsEditable)
                {
                    SplitContainer.Panel2Collapsed = !value;
                    if (value) SplitContainer.Panel2.Show();
                    else SplitContainer.Panel2.Hide();
                }
            }
        }

        /// <summary>
        /// Does this document have any bookmarks?
        /// </summary>
        public bool HasBookmarks => bookmarks.Count > 0;

        /// <summary>
        /// Does this document's pane have any other documents?
        /// </summary> 
        public bool IsAloneInPane
        {
            get
            {
                int count = 0;
                foreach (ITabbedDocument document in Globals.MainForm.Documents)
                {
                    if (document.DockHandler.PanelPane == DockHandler.PanelPane) count++;
                }
                return count <= 1;
            }
        }

        /// <summary>
        /// Current ScintillaControl of the document
        /// </summary>
        public ScintillaControl SciControl
        {
            get
            {
                foreach (Control ctrl in Controls)
                {
                    if (ctrl is ScintillaControl control && !Disposing && !IsDisposed) return control;
                    if (ctrl is SplitContainer casted && casted.Name == "fdSplitView" && !Disposing && !IsDisposed)
                    {
                        ScintillaControl sci1 = casted.Panel1.Controls[0] as ScintillaControl;
                        ScintillaControl sci2 = casted.Panel2.Controls[0] as ScintillaControl;
                        if (sci2.IsFocus) return sci2;
                        if (sci1.IsFocus) return sci1;
                        if (lastEditor != null && lastEditor.Visible)
                        {
                            return lastEditor;
                        }
                        return sci1;
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// First splitted ScintillaControl 
        /// </summary>
        public ScintillaControl SplitSci1 { get; set; }

        /// <summary>
        /// Second splitted ScintillaControl
        /// </summary>
        public ScintillaControl SplitSci2 { get; set; }

        /// <summary>
        /// SplitContainer of the document
        /// </summary>
        public SplitContainer SplitContainer { get; set; }

        /// <summary>
        /// Gets if the file is untitled
        /// </summary>
        public bool IsUntitled
        {
            get
            {
                string untitledFileStart = TextHelper.GetString("Info.UntitledFileStart");
                return IsEditable && FileName.StartsWithOrdinal(untitledFileStart);
            }
        }

        /// <summary>
        /// Sets or gets if the file is modified
        /// </summary> 
        public bool IsModified
        {
            get => isModified;
            set 
            {
                if (!IsEditable) return;
                if (isModified != value)
                {
                    isModified = value;
                    ButtonManager.UpdateFlaggedButtons();
                    RefreshTexts();
                }
            }
        }

        /// <summary>
        /// Activates the scintilla control after a delay
        /// </summary>
        public new void Activate()
        {
            base.Activate();
            if (IsEditable)
            {
                focusTimer.Stop();
                focusTimer.Start();
            }
            ButtonManager.UpdateFlaggedButtons();
        }
        private void OnFocusTimer(object sender, EventArgs e)
        {
            focusTimer.Stop();
            if (SciControl != null && DockPanel.ActiveContent != null && DockPanel.ActiveContent == this)
            {
                SciControl.Focus();
                InitBookmarks();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnMarkerChanged(ScintillaControl sci, int line)
        {
            if (sci != SplitSci1 && sci != SplitSci2) return;
            if (line == -1) // all markers cleared
            {
                bookmarks.Clear();
                ButtonManager.UpdateFlaggedButtons();
                return;
            }
            bool hadBookmark = bookmarks.Contains(line);
            bool hasBookmark = MarkerManager.HasMarker(sci, 0, line);
            if (hadBookmark != hasBookmark) // any change?
            {
                if (!hadBookmark && hasBookmark) bookmarks.Add(line);
                else if (hadBookmark && !hasBookmark) bookmarks.Remove(line);
                ButtonManager.UpdateFlaggedButtons();
            }
        }

        /// <summary>
        /// Adds a new scintilla control to the document
        /// </summary>
        public void AddEditorControls(string file, string text, int codepage)
        {
            SplitSci1 = ScintillaManager.CreateControl(file, text, codepage);
            SplitSci1.Dock = DockStyle.Fill;
            SplitSci2 = ScintillaManager.CreateControl(file, text, codepage);
            SplitSci2.Dock = DockStyle.Fill;
            SplitContainer = new SplitContainer();
            SplitContainer.Name = "fdSplitView";
            SplitContainer.SplitterWidth = ScaleHelper.Scale(SplitContainer.SplitterWidth);
            SplitContainer.Orientation = Orientation.Horizontal;
            SplitContainer.BackColor = SystemColors.Control;
            SplitContainer.Panel1.Controls.Add(SplitSci1);
            SplitContainer.Panel2.Controls.Add(SplitSci2);
            SplitContainer.Dock = DockStyle.Fill;
            SplitContainer.Panel2Collapsed = true;
            int oldDoc = SplitSci1.DocPointer;
            SplitSci2.DocPointer = oldDoc;
            SplitSci1.SavePointLeft += delegate
            {
                Globals.MainForm.OnDocumentModify(this);
            };
            SplitSci1.SavePointReached += delegate
            {
                SplitSci1.MarkerDeleteAll(2);
                IsModified = false;
            };
            SplitSci1.FocusChanged += EditorFocusChanged;
            SplitSci2.FocusChanged += EditorFocusChanged;
            SplitSci1.UpdateSync += EditorUpdateSync;
            SplitSci2.UpdateSync += EditorUpdateSync;
            Controls.Add(SplitContainer);
        }

        /// <summary>
        /// Syncs both of the scintilla editors
        /// </summary>
        private void EditorUpdateSync(ScintillaControl sender)
        {
            if (!IsEditable) return;
            ScintillaControl e1 = SplitSci1;
            ScintillaControl e2 = SplitSci2;
            if (sender == SplitSci2)
            {
                 e1 = SplitSci2;
                 e2 = SplitSci1;
            }
            e2.UpdateSync -= EditorUpdateSync;
            ScintillaManager.UpdateSyncProps(e1, e2);
            ScintillaManager.ApplySciSettings(e2);
            e2.UpdateSync += EditorUpdateSync;
            Globals.MainForm.RefreshUI();
        }

        /// <summary>
        /// When the user changes to sci, block events from inactive sci
        /// </summary>
        private void EditorFocusChanged(ScintillaControl sender)
        {
            if (sender.IsFocus)
            {
                lastEditor = sender;
                SplitSci1.DisableAllSciEvents = (sender == SplitSci2);
                SplitSci2.DisableAllSciEvents = (sender == SplitSci1);
            }
        }

        /// <summary>
        /// Checks if the the file is changed outside of fd
        /// </summary>
        public bool CheckFileChange()
        {
            if (fileInfo == null)
            {
                fileInfo = new FileInfo(FileName);
            }
            if (!Globals.MainForm.ClosingEntirely && File.Exists(FileName))
            {
                FileInfo fi = new FileInfo(FileName);
                if (fileInfo.IsReadOnly != fi.IsReadOnly) return true;
                if (fileInfo.LastWriteTime != fi.LastWriteTime) return true;
            }
            return false;
        }

        /// <summary>
        /// Updates the file info after user dismisses the change notification
        /// </summary>
        public void RefreshFileInfo()
        {
            if (!Globals.MainForm.ClosingEntirely && File.Exists(FileName))
            {
                fileInfo = new FileInfo(FileName);
            }
        }

        /// <summary>
        /// Saves an editable document
        /// </summary>
        /// <param name="file"></param>
        /// <param name="reason">is passed on when raising the FileSave event</param>
        public void Save(string file, string reason)
        {
            if (!IsEditable) return;
            if (!IsUntitled && FileHelper.FileIsReadOnly(FileName))
            {
                string dlgTitle = TextHelper.GetString("Title.ConfirmDialog");
                string message = TextHelper.GetString("Info.MakeReadOnlyWritable");
                if (MessageBox.Show(Globals.MainForm, message, dlgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    ScintillaManager.MakeFileWritable(SciControl);
                }
                else return;
            }
            string oldFile = SciControl.FileName;
            bool otherFile = (SciControl.FileName != file);
            if (otherFile)
            {
                string args = FileName + ";" + file;
                RecoveryManager.RemoveTemporaryFile(FileName);
                TextEvent renaming = new TextEvent(EventType.FileRenaming, args);
                EventManager.DispatchEvent(this, renaming);
                TextEvent close = new TextEvent(EventType.FileClose, FileName);
                EventManager.DispatchEvent(this, close);
            }
            TextEvent saving = new TextEvent(EventType.FileSaving, file);
            EventManager.DispatchEvent(this, saving);
            if (!saving.Handled)
            {
                if (otherFile)
                {
                    UpdateDocumentIcon(file);
                    SciControl.FileName = file;
                }
                ScintillaManager.CleanUpCode(SciControl);
                DataEvent de = new DataEvent(EventType.FileEncode, file, SciControl.Text);
                EventManager.DispatchEvent(this, de); // Lets ask if a plugin wants to encode and save the data..
                if (!de.Handled) FileHelper.WriteFile(file, SciControl.Text, SciControl.Encoding, SciControl.SaveBOM);
                IsModified = false;
                SciControl.SetSavePoint();
                RecoveryManager.RemoveTemporaryFile(FileName);
                fileInfo = new FileInfo(FileName);
                if (otherFile)
                {
                    ScintillaManager.UpdateControlSyntax(SciControl);
                    Globals.MainForm.OnFileSave(this, oldFile, reason);
                }
                else Globals.MainForm.OnFileSave(this, null, reason);
            }
            RefreshTexts();
        }
        /// <summary>
        /// Saves an editable document
        /// </summary>
        public void Save(string file) => Save(file, null);

        public void Save()
        {
            if (!IsEditable) return;
            Save(FileName);
        }

        /// <summary>
        /// Reloads an editable document
        /// </summary>
        public void Reload(bool showQuestion)
        {
            if (!IsEditable) return;
            if (showQuestion)
            {
                string dlgTitle = TextHelper.GetString("Title.ConfirmDialog");
                string message = TextHelper.GetString("Info.AreYouSureToReload");
                if (MessageBox.Show(Globals.MainForm, message, " " + dlgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;
            }
            Globals.MainForm.ReloadingDocument = true;
            int position = SciControl.CurrentPos;
            TextEvent te = new TextEvent(EventType.FileReload, FileName);
            EventManager.DispatchEvent(Globals.MainForm, te);
            if (!te.Handled)
            {
                EncodingFileInfo info = FileHelper.GetEncodingFileInfo(FileName);
                if (info.CodePage == -1)
                {
                    Globals.MainForm.ReloadingDocument = false;
                    return; // If the files is locked, stop.
                }
                Encoding encoding = Encoding.GetEncoding(info.CodePage);
                SciControl.IsReadOnly = false;
                SciControl.Encoding = encoding;
                SciControl.Text = info.Contents;
                SciControl.IsReadOnly = FileHelper.FileIsReadOnly(FileName);
                SciControl.SetSel(position, position);
                SciControl.EmptyUndoBuffer();
                int lineCount = SciControl.LineCount;
                foreach (var lineNum in bookmarks)
                {
                    if (lineNum < 0) continue;
                    if (lineNum >= lineCount)
                    {
                        if (!MarkerManager.HasMarker(SciControl, 0, lineCount - 1))
                        {
                            MarkerManager.ToggleMarker(SciControl, 0, lineCount - 1);
                        }
                    }
                    else MarkerManager.ToggleMarker(SciControl, 0, lineNum);
                }
                InitBookmarks();
                fileInfo = new FileInfo(FileName);
            }
            Globals.MainForm.OnDocumentReload(this);
        }

        /// <summary>
        /// Reverts the document to the orginal state
        /// </summary>
        public void Revert(bool showQuestion)
        {
            if (!IsEditable) return;
            if (showQuestion)
            {
                string dlgTitle = TextHelper.GetString("Title.ConfirmDialog");
                string message = TextHelper.GetString("Info.AreYouSureToRevert");
                if (MessageBox.Show(Globals.MainForm, message, " " + dlgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;
            }
            TextEvent te = new TextEvent(EventType.FileRevert, Globals.SciControl.FileName);
            EventManager.DispatchEvent(this, te);
            if (!te.Handled)
            {
                while (SciControl.CanUndo) SciControl.Undo();
                ButtonManager.UpdateFlaggedButtons();
            }
        }
        
        /// <summary>
        /// Starts the backup process timing
        /// </summary> 
        private void StartBackupTiming()
        {
            backupTimer = new Timer();
            backupTimer.Tick += BackupTimerTick;
            backupTimer.Interval = Globals.Settings.BackupInterval;
            backupTimer.Start();
        }

        /// <summary>
        /// Saves a backup file after an interval
        /// </summary> 
        private void BackupTimerTick(object sender, EventArgs e)
        {
            if (IsEditable && !IsUntitled && IsModified && previousText != SciControl.Text)
            {
                RecoveryManager.SaveTemporaryFile(FileName, SciControl.Text, SciControl.Encoding);
                previousText = SciControl.Text;
            }
        }

        /// <summary>
        /// Automatically updates the document icon
        /// </summary>
        private void UpdateDocumentIcon(string file)
        {
            if (UseCustomIcon) return;
            if (Win32.ShouldUseWin32() && !IsBrowsable) Icon = IconExtractor.GetFileIcon(file, true);
            else
            {
                Image image = Globals.MainForm.FindImage("480", false);
                Icon = ImageKonverter.ImageToIcon(image);
                UseCustomIcon = true;
            }
        }

        /// <summary>
        /// Refreshes the tab and tooltip texts
        /// </summary>
        public void RefreshTexts()
        {
            TabTextManager.UpdateTabTexts();
            UpdateToolTipText();
        }

        /// <summary>
        /// Checks the bookmarks when document is active.
        /// </summary>
        public void InitBookmarks()
        {
            bookmarks.Clear();
            for (int i = 0; i < SplitSci1.LineCount; i++) 
            {
                if (MarkerManager.HasMarker(SciControl, 0, i)) bookmarks.Add(i);
            }
        }

        /// <summary>
        /// Updates the document's tooltip
        /// </summary>
        private void UpdateToolTipText()
        {
            ToolTipText = !IsEditable ? "" : FileName;
        }

        /// <summary>
        /// Updates the document icon when a control is added
        /// </summary>
        private void DocumentControlAdded(object sender, ControlEventArgs e)
        {
            UpdateToolTipText();
            UpdateDocumentIcon(FileName);
        }

        /// <summary>
        /// Close the document and update buttons
        /// </summary>
        public new void Close()
        {
            base.Close();
            ButtonManager.UpdateFlaggedButtons();
        }

    }

}

