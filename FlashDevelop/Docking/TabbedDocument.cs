using System;
using System.IO;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using WeifenLuo.WinFormsUI.Docking;
using PluginCore.Localization;
using FlashDevelop.Managers;
using FlashDevelop.Controls;
using FlashDevelop.Settings;
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
        readonly Timer focusTimer;
        Timer backupTimer;
        string previousText;
        readonly List<int> bookmarks;
        ScintillaControl lastEditor;
        bool isModified;
        FileInfo fileInfo;

        public TabbedDocument()
        {
            focusTimer = new Timer {Interval = 100};
            bookmarks = new List<int>();
            focusTimer.Tick += OnFocusTimer;
            ControlAdded += DocumentControlAdded;
            UITools.Manager.OnMarkerChanged += OnMarkerChanged;
            DockPanel = PluginBase.MainForm.DockPanel;
            Font = PluginBase.Settings.DefaultFont;
            DockAreas = DockAreas.Document;
            BackColor = Color.White;
            UseCustomIcon = false;
            StartBackupTiming();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                focusTimer?.Dispose();
                backupTimer?.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Disables the automatic update of the icon
        /// </summary>
        public bool UseCustomIcon { get; set; }

        /// <summary>
        /// Path of the document
        /// </summary>
        public string FileName => SciControl?.FileName;

        /// <summary>
        /// Do we contain a ScintillaControl?
        /// </summary>
        public bool IsEditable => SciControl != null;

        /// <summary>
        /// Do we contain a Browser control?
        /// </summary>
        public bool IsBrowsable => Controls.OfType<Browser>().Any();

        /// <summary>
        /// Are we splitted in to two sci controls?
        /// </summary>
        public bool IsSplitted
        {
            get => IsEditable && !SplitContainer.Panel2Collapsed;
            set
            {
                if (!IsEditable) return;
                SplitContainer.Panel2Collapsed = !value;
                if (value) SplitContainer.Panel2.Show();
                else SplitContainer.Panel2.Hide();
            }
        }

        /// <summary>
        /// Does this document have any bookmarks?
        /// </summary>
        public bool HasBookmarks => bookmarks.Count > 0;

        /// <summary>
        /// Does this document's pane have any other documents?
        /// </summary> 
        public bool IsAloneInPane => PluginBase.MainForm.Documents.Count(it => it.DockHandler.PanelPane == DockHandler.PanelPane) <= 1;

        /// <summary>
        /// Current ScintillaControl of the document
        /// </summary>
        public ScintillaControl? SciControl
        {
            get
            {
                foreach (Control ctrl in Controls)
                {
                    switch (ctrl)
                    {
                        case ScintillaControl control when !Disposing && !IsDisposed: return control;
                        case SplitContainer container when container.Name == "fdSplitView" && !Disposing && !IsDisposed:
                        {
                            var sci2 = (ScintillaControl) container.Panel2.Controls[0];
                            if (sci2.IsFocus) return sci2;
                            var sci1 = (ScintillaControl) container.Panel1.Controls[0];
                            if (sci1.IsFocus) return sci1;
                            return lastEditor != null && lastEditor.Visible 
                                ? lastEditor
                                : sci1;
                        }
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
        public bool IsUntitled => SciControl is { } sci
                                  && TextHelper.GetString("Info.UntitledFileStart") is { } untitledFileStart
                                  && sci.FileName.StartsWithOrdinal(untitledFileStart);

        /// <summary>
        /// Sets or gets if the file is modified
        /// </summary> 
        public bool IsModified
        {
            get => isModified;
            set 
            {
                if (!IsEditable) return;
                if (value == isModified) return;
                isModified = value;
                ButtonManager.UpdateFlaggedButtons();
                RefreshTexts();
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

        void OnFocusTimer(object sender, EventArgs e)
        {
            focusTimer.Stop();
            if (DockPanel.ActiveContent == this && SciControl is { } sci)
            {
                sci.Focus();
                InitBookmarks();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        void OnMarkerChanged(ScintillaControl sci, int line)
        {
            if (sci != SplitSci1 && sci != SplitSci2) return;
            if (line == -1) // all markers cleared
            {
                bookmarks.Clear();
                ButtonManager.UpdateFlaggedButtons();
                return;
            }
            var hadBookmark = bookmarks.Contains(line);
            if (hadBookmark != MarkerManager.HasMarker(sci, 0, line)) // any change?
            {
                if (!hadBookmark) bookmarks.Add(line);
                else bookmarks.Remove(line);
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
            SplitContainer = new SplitContainer {Name = "fdSplitView"};
            SplitContainer.SplitterWidth = ScaleHelper.Scale(SplitContainer.SplitterWidth);
            SplitContainer.Orientation = Orientation.Horizontal;
            SplitContainer.BackColor = SystemColors.Control;
            SplitContainer.Panel1.Controls.Add(SplitSci1);
            SplitContainer.Panel2.Controls.Add(SplitSci2);
            SplitContainer.Dock = DockStyle.Fill;
            SplitContainer.Panel2Collapsed = true;
            SplitSci2.DocPointer = SplitSci1.DocPointer;
            SplitSci1.SavePointLeft += sender => Globals.MainForm.OnDocumentModify(this);
            SplitSci1.SavePointReached += sender =>
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
        void EditorUpdateSync(ScintillaControl sender)
        {
            if (!IsEditable) return;
            var sci1 = SplitSci1;
            var sci2 = SplitSci2;
            if (sender == sci2)
            {
                 sci1 = sci2;
                 sci2 = SplitSci1;
            }
            sci2.UpdateSync -= EditorUpdateSync;
            ScintillaManager.UpdateSyncProps(sci1, sci2);
            ScintillaManager.ApplySciSettings(sci2);
            sci2.UpdateSync += EditorUpdateSync;
            Globals.MainForm.RefreshUI();
        }

        /// <summary>
        /// When the user changes to sci, block events from inactive sci
        /// </summary>
        void EditorFocusChanged(ScintillaControl sender)
        {
            if (!sender.IsFocus) return;
            lastEditor = sender;
            SplitSci1.DisableAllSciEvents = (sender == SplitSci2);
            SplitSci2.DisableAllSciEvents = (sender == SplitSci1);
        }

        /// <summary>
        /// Checks if the the file is changed outside of fd
        /// </summary>
        public bool CheckFileChange()
        {
            var fileName = FileName;
            fileInfo ??= new FileInfo(fileName);
            if (PluginBase.MainForm.ClosingEntirely || !File.Exists(fileName)) return false;
            var fi = new FileInfo(fileName);
            return fileInfo.IsReadOnly != fi.IsReadOnly || fileInfo.LastWriteTime != fi.LastWriteTime;
        }

        /// <summary>
        /// Updates the file info after user dismisses the change notification
        /// </summary>
        public void RefreshFileInfo()
        {
            if (!PluginBase.MainForm.ClosingEntirely
                && FileName is { } fileName
                && File.Exists(fileName))
            {
                fileInfo = new FileInfo(fileName);
            }
        }

        /// <summary>
        /// Saves an editable document
        /// </summary>
        /// <param name="file"></param>
        /// <param name="reason">is passed on when raising the FileSave event</param>
        public void Save(string file, string reason)
        {
            var sci = SciControl;
            if (sci is null) return;
            if (!IsUntitled && FileHelper.FileIsReadOnly(sci.FileName))
            {
                var dlgTitle = TextHelper.GetString("Title.ConfirmDialog");
                var message = TextHelper.GetString("Info.MakeReadOnlyWritable");
                if (MessageBox.Show(PluginBase.MainForm, message, dlgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    ScintillaManager.MakeFileWritable(sci);
                }
                else return;
            }
            var oldFile = sci.FileName;
            var otherFile = oldFile != file;
            if (otherFile)
            {
                var args = oldFile + ";" + file;
                RecoveryManager.RemoveTemporaryFile(oldFile);
                var renaming = new TextEvent(EventType.FileRenaming, args);
                EventManager.DispatchEvent(this, renaming);
                var close = new TextEvent(EventType.FileClose, FileName);
                EventManager.DispatchEvent(this, close);
            }
            var saving = new TextEvent(EventType.FileSaving, file);
            EventManager.DispatchEvent(this, saving);
            if (!saving.Handled)
            {
                sci = SciControl;
                if (otherFile)
                {
                    UpdateDocumentIcon(file);
                    sci.FileName = file;
                }
                ScintillaManager.CleanUpCode(sci);
                var de = new DataEvent(EventType.FileEncode, file, sci.Text);
                EventManager.DispatchEvent(this, de); // Lets ask if a plugin wants to encode and save the data..
                if (!de.Handled) FileHelper.WriteFile(file, sci.Text, sci.Encoding, sci.SaveBOM);
                IsModified = false;
                sci.SetSavePoint();
                RecoveryManager.RemoveTemporaryFile(sci.FileName);
                fileInfo = new FileInfo(sci.FileName);
                if (otherFile)
                {
                    ScintillaManager.UpdateControlSyntax(sci);
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
            if (SciControl?.FileName is {} fileName) Save(fileName);
        }

        /// <summary>
        /// Reloads an editable document
        /// </summary>
        public void Reload(bool showQuestion)
        {
            if (!IsEditable) return;
            if (showQuestion)
            {
                var dlgTitle = TextHelper.GetString("Title.ConfirmDialog");
                var message = TextHelper.GetString("Info.AreYouSureToReload");
                if (MessageBox.Show(PluginBase.MainForm, message, " " + dlgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;
            }
            Globals.MainForm.ReloadingDocument = true;
            var position = SciControl.CurrentPos;
            var te = new TextEvent(EventType.FileReload, FileName);
            EventManager.DispatchEvent(PluginBase.MainForm, te);
            if (!te.Handled)
            {
                var info = FileHelper.GetEncodingFileInfo(FileName);
                if (info.CodePage == -1)
                {
                    Globals.MainForm.ReloadingDocument = false;
                    return; // If the files is locked, stop.
                }
                var encoding = Encoding.GetEncoding(info.CodePage);
                var sci = SciControl;
                sci.IsReadOnly = false;
                sci.Encoding = encoding;
                sci.Text = info.Contents;
                sci.IsReadOnly = FileHelper.FileIsReadOnly(sci.FileName);
                sci.SetSel(position, position);
                sci.EmptyUndoBuffer();
                int lineCount = sci.LineCount;
                foreach (var lineNum in bookmarks)
                {
                    if (lineNum < 0) continue;
                    if (lineNum >= lineCount)
                    {
                        if (!MarkerManager.HasMarker(sci, 0, lineCount - 1))
                            MarkerManager.ToggleMarker(sci, 0, lineCount - 1);
                    }
                    else MarkerManager.ToggleMarker(sci, 0, lineNum);
                }
                InitBookmarks();
                fileInfo = new FileInfo(sci.FileName);
            }
            Globals.MainForm.OnDocumentReload(this);
        }

        /// <summary>
        /// Reverts the document to the original state
        /// </summary>
        public void Revert(bool showQuestion)
        {
            var sci = SciControl;
            if (sci is null) return;
            if (showQuestion)
            {
                var dlgTitle = TextHelper.GetString("Title.ConfirmDialog");
                var message = TextHelper.GetString("Info.AreYouSureToRevert");
                if (MessageBox.Show(PluginBase.MainForm, message, " " + dlgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;
            }
            var te = new TextEvent(EventType.FileRevert, sci.FileName);
            EventManager.DispatchEvent(this, te);
            if (te.Handled) return;
            while (sci.CanUndo) sci.Undo();
            ButtonManager.UpdateFlaggedButtons();
        }
        
        /// <summary>
        /// Starts the backup process timing
        /// </summary> 
        void StartBackupTiming()
        {
            backupTimer = new Timer();
            backupTimer.Tick += BackupTimerTick;
            backupTimer.Interval = ((SettingObject)PluginBase.Settings).BackupInterval;
            backupTimer.Start();
        }

        /// <summary>
        /// Saves a backup file after an interval
        /// </summary> 
        void BackupTimerTick(object sender, EventArgs e)
        {
            if (SciControl is { } sci
                && !IsUntitled
                && IsModified
                && sci.Text is { } text
                && text != previousText)
            {
                RecoveryManager.SaveTemporaryFile(sci.FileName, text, sci.Encoding);
                previousText = text;
            }
        }

        /// <summary>
        /// Automatically updates the document icon
        /// </summary>
        void UpdateDocumentIcon(string file)
        {
            if (UseCustomIcon) return;
            if (Win32.ShouldUseWin32() && !IsBrowsable) Icon = IconExtractor.GetFileIcon(file, true);
            else
            {
                var image = PluginBase.MainForm.FindImage("480", false);
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
            var sci = SciControl;
            var lineCount = sci.LineCount;
            for (var i = 0; i < lineCount; i++) 
            {
                if (MarkerManager.HasMarker(sci, 0, i))
                    bookmarks.Add(i);
            }
        }

        /// <summary>
        /// Updates the document's tooltip
        /// </summary>
        void UpdateToolTipText() => UpdateToolTipText(FileName ?? string.Empty);

        /// <summary>
        /// Updates the document's tooltip
        /// </summary>
        void UpdateToolTipText(string text) => ToolTipText = text;

        /// <summary>
        /// Updates the document icon when a control is added
        /// </summary>
        void DocumentControlAdded(object sender, ControlEventArgs e)
        {
            var fileName = FileName;
            UpdateToolTipText(fileName);
            UpdateDocumentIcon(fileName);
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