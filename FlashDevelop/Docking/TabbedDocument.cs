using System;
using System.IO;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using WeifenLuo.WinFormsUI;
using WeifenLuo.WinFormsUI.Docking;
using PluginCore.Localization;
using FlashDevelop.Managers;
using FlashDevelop.Helpers;
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
        private Timer focusTimer;
        private Timer backupTimer;
        private String previousText;
        private List<Int32> bookmarks;
        private ScintillaControl editor;
        private ScintillaControl editor2;
        private ScintillaControl lastEditor;
        private SplitContainer splitContainer;
        private Boolean useCustomIcon;
        private Boolean isModified;
        private FileInfo fileInfo;

        public TabbedDocument()
        {
            this.focusTimer = new Timer();
            this.focusTimer.Interval = 100;
            this.bookmarks = new List<Int32>();
            this.focusTimer.Tick += new EventHandler(this.OnFocusTimer);
            this.ControlAdded += new ControlEventHandler(this.DocumentControlAdded);
            UITools.Manager.OnMarkerChanged += new UITools.LineEventHandler(this.OnMarkerChanged);
            this.DockPanel = Globals.MainForm.DockPanel;
            this.Font = Globals.Settings.DefaultFont;
            this.DockAreas = DockAreas.Document;
            this.BackColor = Color.White;
            this.useCustomIcon = false;
            this.StartBackupTiming();
        }

        /// <summary>
        /// Disables the automatic update of the icon
        /// </summary>
        public Boolean UseCustomIcon
        {
            get { return this.useCustomIcon; }
            set { this.useCustomIcon = value; }
        }

        /// <summary>
        /// Path of the document
        /// </summary>
        public String FileName
        {
            get
            {
                if (this.IsEditable) return this.SciControl.FileName;
                else return null;
            }
        }

        /// <summary>
        /// Do we contain a Browser control?
        /// </summary>
        public Boolean IsBrowsable
        {
            get
            {
                foreach (Control ctrl in this.Controls)
                {
                    if (ctrl is Browser) return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Do we contain a ScintillaControl?
        /// </summary>
        public Boolean IsEditable
        {
            get
            {
                if (this.SciControl == null) return false;
                else return true;
            }
        }

        /// <summary>
        /// Are we splitted in to two sci controls?
        /// </summary>
        public Boolean IsSplitted
        {
            get
            {
                if (!this.IsEditable || this.splitContainer.Panel2Collapsed) return false;
                else return true;
            }
            set
            {
                if (this.IsEditable)
                {
                    this.splitContainer.Panel2Collapsed = !value;
                    if (value) this.splitContainer.Panel2.Show();
                    else this.splitContainer.Panel2.Hide();
                }
            }
        }

        /// <summary>
        /// Does this document have any bookmarks?
        /// </summary>
        public Boolean HasBookmarks
        {
            get { return bookmarks.Count > 0; }
        }

        /// <summary>
        /// Does this document's pane have any other documents?
        /// </summary> 
        public Boolean IsAloneInPane
        {
            get
            {
                int count = 0;
                foreach (ITabbedDocument document in Globals.MainForm.Documents)
                {
                    if (document.DockHandler.PanelPane == DockHandler.PanelPane)
                        count++;
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
                foreach (Control ctrl in this.Controls)
                {
                    if (ctrl is ScintillaControl && !this.Disposing) return ctrl as ScintillaControl;
                    else if (ctrl is SplitContainer && ctrl.Name == "fdSplitView" && !this.Disposing)
                    {
                        SplitContainer casted = ctrl as SplitContainer;
                        ScintillaControl sci1 = casted.Panel1.Controls[0] as ScintillaControl;
                        ScintillaControl sci2 = casted.Panel2.Controls[0] as ScintillaControl;
                        if (sci2.IsFocus) return sci2;
                        else if (sci1.IsFocus) return sci1;
                        else if (this.lastEditor != null && this.lastEditor.Visible)
                        {
                            return this.lastEditor;
                        }
                        else return sci1;
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// First splitted ScintillaControl 
        /// </summary>
        public ScintillaControl SplitSci1
        {
            get
            {
                if (this.editor != null) return this.editor;
                else return null;
            }
        }

        /// <summary>
        /// Second splitted ScintillaControl
        /// </summary>
        public ScintillaControl SplitSci2
        {
            get
            {
                if (this.editor2 != null) return this.editor2;
                else return null;
            }
        }

        /// <summary>
        /// SplitContainer of the document
        /// </summary>
        public SplitContainer SplitContainer
        {
            get
            {
                if (this.splitContainer != null) return this.splitContainer;
                else return null;
            }
        }
            
        /// <summary>
        /// Gets if the file is untitled
        /// </summary>
        public Boolean IsUntitled
        {
            get
            {
                String untitledFileStart = TextHelper.GetString("Info.UntitledFileStart");
                if (this.IsEditable) return this.FileName.StartsWithOrdinal(untitledFileStart);
                else return false;
            }
        }

        /// <summary>
        /// Sets or gets if the file is modified
        /// </summary> 
        public Boolean IsModified
        {
            get { return this.isModified; }
            set 
            {
                if (!this.IsEditable) return;
                if (this.isModified != value)
                {
                    this.isModified = value;
                    ButtonManager.UpdateFlaggedButtons();
                    this.RefreshTexts();
                }
            }
        }

        /// <summary>
        /// Activates the scintilla control after a delay
        /// </summary>
        public new void Activate()
        {
            base.Activate();
            if (this.IsEditable)
            {
                this.focusTimer.Stop();
                this.focusTimer.Start();
            }
            ButtonManager.UpdateFlaggedButtons();
        }
        private void OnFocusTimer(Object sender, EventArgs e)
        {
            this.focusTimer.Stop();
            if (this.SciControl != null && this.DockPanel.ActiveContent != null && this.DockPanel.ActiveContent == this)
            {
                this.SciControl.Focus();
                this.InitBookmarks();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnMarkerChanged(ScintillaControl sci, Int32 line)
        {
            if (sci != this.editor && sci != this.editor2) return;
            if (line == -1) // all markers cleared
            {
                this.bookmarks.Clear();
                ButtonManager.UpdateFlaggedButtons();
                return;
            }
            Boolean hadBookmark = this.bookmarks.Contains(line);
            Boolean hasBookmark = MarkerManager.HasMarker(sci, 0, line);
            if (hadBookmark != hasBookmark) // any change?
            {
                if (!hadBookmark && hasBookmark) this.bookmarks.Add(line);
                else if (hadBookmark && !hasBookmark) this.bookmarks.Remove(line);
                ButtonManager.UpdateFlaggedButtons();
            }
        }

        /// <summary>
        /// Adds a new scintilla control to the document
        /// </summary>
        public void AddEditorControls(String file, String text, Int32 codepage)
        {
            this.editor = ScintillaManager.CreateControl(file, text, codepage);
            this.editor.Dock = DockStyle.Fill;
            this.editor2 = ScintillaManager.CreateControl(file, text, codepage);
            this.editor2.Dock = DockStyle.Fill;
            this.splitContainer = new SplitContainer();
            this.splitContainer.Name = "fdSplitView";
            this.splitContainer.SplitterWidth = ScaleHelper.Scale(this.splitContainer.SplitterWidth);
            this.splitContainer.Orientation = Orientation.Horizontal;
            this.splitContainer.BackColor = SystemColors.Control;
            this.splitContainer.Panel1.Controls.Add(this.editor);
            this.splitContainer.Panel2.Controls.Add(this.editor2);
            this.splitContainer.Dock = DockStyle.Fill;
            this.splitContainer.Panel2Collapsed = true;
            Int32 oldDoc = this.editor.DocPointer;
            this.editor2.DocPointer = oldDoc;
            this.editor.SavePointLeft += delegate
            {
                Globals.MainForm.OnDocumentModify(this);
            };
            this.editor.SavePointReached += delegate
            {
                this.editor.MarkerDeleteAll(2);
                this.IsModified = false;
            };
            this.editor.FocusChanged += new FocusHandler(this.EditorFocusChanged);
            this.editor2.FocusChanged += new FocusHandler(this.EditorFocusChanged);
            this.editor.UpdateSync += new UpdateSyncHandler(this.EditorUpdateSync);
            this.editor2.UpdateSync += new UpdateSyncHandler(this.EditorUpdateSync);
            this.Controls.Add(this.splitContainer);
        }

        /// <summary>
        /// Syncs both of the scintilla editors
        /// </summary>
        private void EditorUpdateSync(ScintillaControl sender)
        {
            if (!this.IsEditable) return;
            ScintillaControl e1 = editor;
            ScintillaControl e2 = editor2;
            if (sender == editor2)
            {
                 e1 = editor2;
                 e2 = editor;
            }
            e2.UpdateSync -= new UpdateSyncHandler(this.EditorUpdateSync);
            ScintillaManager.UpdateSyncProps(e1, e2);
            ScintillaManager.ApplySciSettings(e2);
            e2.UpdateSync += new UpdateSyncHandler(this.EditorUpdateSync);
            Globals.MainForm.RefreshUI();
        }

        /// <summary>
        /// When the user changes to sci, block events from inactive sci
        /// </summary>
        private void EditorFocusChanged(ScintillaControl sender)
        {
            if (sender.IsFocus)
            {
                this.lastEditor = sender;
                this.editor.DisableAllSciEvents = (sender == editor2);
                this.editor2.DisableAllSciEvents = (sender == editor);
            }
        }

        /// <summary>
        /// Checks if the the file is changed outside of fd
        /// </summary>
        public Boolean CheckFileChange()
        {
            if (this.fileInfo == null)
            {
                this.fileInfo = new FileInfo(this.FileName);
            }
            if (!Globals.MainForm.ClosingEntirely && File.Exists(this.FileName))
            {
                FileInfo fi = new FileInfo(this.FileName);
                if (this.fileInfo.LastWriteTime != fi.LastWriteTime) return true;
            }
            return false;
        }

        /// <summary>
        /// Updates the file info after user dismisses the change notification
        /// </summary>
        public void RefreshFileInfo()
        {
            if (!Globals.MainForm.ClosingEntirely && File.Exists(this.FileName))
            {
                this.fileInfo = new FileInfo(this.FileName);
            }
        }

        /// <summary>
        /// Saves an editable document
        /// </summary>
        public void Save(String file)
        {
            if (!this.IsEditable) return;
            if (!this.IsUntitled && FileHelper.FileIsReadOnly(this.FileName))
            {
                String dlgTitle = TextHelper.GetString("Title.ConfirmDialog");
                String message = TextHelper.GetString("Info.MakeReadOnlyWritable");
                if (MessageBox.Show(Globals.MainForm, message, dlgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    ScintillaManager.MakeFileWritable(this.SciControl);
                }
                else return;
            }
            String oldFile = this.SciControl.FileName;
            Boolean otherFile = (this.SciControl.FileName != file);
            if (otherFile)
            {
                String args = this.FileName + ";" + file;
                RecoveryManager.RemoveTemporaryFile(this.FileName);
                TextEvent renaming = new TextEvent(EventType.FileRenaming, args);
                EventManager.DispatchEvent(this, renaming);
                TextEvent close = new TextEvent(EventType.FileClose, this.FileName);
                EventManager.DispatchEvent(this, close);
            }
            TextEvent saving = new TextEvent(EventType.FileSaving, file);
            EventManager.DispatchEvent(this, saving);
            if (!saving.Handled)
            {
                this.UpdateDocumentIcon(file);
                this.SciControl.FileName = file;
                ScintillaManager.CleanUpCode(this.SciControl);
                DataEvent de = new DataEvent(EventType.FileEncode, file, this.SciControl.Text);
                EventManager.DispatchEvent(this, de); // Lets ask if a plugin wants to encode and save the data..
                if (!de.Handled) FileHelper.WriteFile(file, this.SciControl.Text, this.SciControl.Encoding, this.SciControl.SaveBOM);
                this.IsModified = false;
                this.SciControl.SetSavePoint();
                RecoveryManager.RemoveTemporaryFile(this.FileName);
                this.fileInfo = new FileInfo(this.FileName);
                if (otherFile)
                {
                    ScintillaManager.UpdateControlSyntax(this.SciControl);
                    Globals.MainForm.OnFileSave(this, oldFile);
                }
                else Globals.MainForm.OnFileSave(this, null);
            }
            this.RefreshTexts();
        }
        public void Save()
        {
            if (!this.IsEditable) return;
            this.Save(this.FileName);
        }

        /// <summary>
        /// Reloads an editable document
        /// </summary>
        public void Reload(Boolean showQuestion)
        {
            if (!this.IsEditable) return;
            if (showQuestion)
            {
                String dlgTitle = TextHelper.GetString("Title.ConfirmDialog");
                String message = TextHelper.GetString("Info.AreYouSureToReload");
                if (MessageBox.Show(Globals.MainForm, message, " " + dlgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;
            }
            Globals.MainForm.ReloadingDocument = true;
            Int32 position = this.SciControl.CurrentPos;
            TextEvent te = new TextEvent(EventType.FileReload, this.FileName);
            EventManager.DispatchEvent(Globals.MainForm, te);
            if (!te.Handled)
            {
                EncodingFileInfo info = FileHelper.GetEncodingFileInfo(this.FileName);
                if (info.CodePage == -1)
                {
                    Globals.MainForm.ReloadingDocument = false;
                    return; // If the files is locked, stop.
                }
                Encoding encoding = Encoding.GetEncoding(info.CodePage);
                this.SciControl.IsReadOnly = false;
                this.SciControl.Encoding = encoding;
                this.SciControl.Text = info.Contents;
                this.SciControl.IsReadOnly = FileHelper.FileIsReadOnly(this.FileName);
                this.SciControl.SetSel(position, position);
                this.SciControl.EmptyUndoBuffer();
                this.InitBookmarks();
            }
            Globals.MainForm.OnDocumentReload(this);
        }

        /// <summary>
        /// Reverts the document to the orginal state
        /// </summary>
        public void Revert(Boolean showQuestion)
        {
            if (!this.IsEditable) return;
            if (showQuestion)
            {
                String dlgTitle = TextHelper.GetString("Title.ConfirmDialog");
                String message = TextHelper.GetString("Info.AreYouSureToRevert");
                if (MessageBox.Show(Globals.MainForm, message, " " + dlgTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) return;
            }
            TextEvent te = new TextEvent(EventType.FileRevert, Globals.SciControl.FileName);
            EventManager.DispatchEvent(this, te);
            if (!te.Handled)
            {
                while (this.SciControl.CanUndo) this.SciControl.Undo();
                ButtonManager.UpdateFlaggedButtons();
            }
        }
        
        /// <summary>
        /// Starts the backup process timing
        /// </summary> 
        private void StartBackupTiming()
        {
            this.backupTimer = new Timer();
            this.backupTimer.Tick += new EventHandler(this.BackupTimerTick);
            this.backupTimer.Interval = Globals.Settings.BackupInterval;
            this.backupTimer.Start();
        }

        /// <summary>
        /// Saves a backup file after an interval
        /// </summary> 
        private void BackupTimerTick(Object sender, EventArgs e)
        {
            if (this.IsEditable && !this.IsUntitled && this.IsModified && this.previousText != this.SciControl.Text)
            {
                RecoveryManager.SaveTemporaryFile(this.FileName, this.SciControl.Text, this.SciControl.Encoding);
                this.previousText = this.SciControl.Text;
            }
        }

        /// <summary>
        /// Automatically updates the document icon
        /// </summary>
        private void UpdateDocumentIcon(String file)
        {
            if (this.useCustomIcon) return;
            if (Win32.ShouldUseWin32() && !this.IsBrowsable) this.Icon = IconExtractor.GetFileIcon(file, true);
            else
            {
                Image image = Globals.MainForm.FindImage("480", false);
                this.Icon = ImageKonverter.ImageToIcon(image);
                this.useCustomIcon = true;
            }
        }

        /// <summary>
        /// Refreshes the tab and tooltip texts
        /// </summary>
        public void RefreshTexts()
        {
            TabTextManager.UpdateTabTexts();
            this.UpdateToolTipText();
        }

        /// <summary>
        /// Checks the bookmarks when document is active.
        /// </summary>
        public void InitBookmarks()
        {
            this.bookmarks.Clear();
            for (Int32 i = 0; i < editor.LineCount; i++) 
            {
                if (MarkerManager.HasMarker(SciControl, 0, i)) this.bookmarks.Add(i);
            }
        }

        /// <summary>
        /// Updates the document's tooltip
        /// </summary>
        private void UpdateToolTipText()
        {
            if (!this.IsEditable) this.ToolTipText = "";
            else this.ToolTipText = this.FileName;
        }

        /// <summary>
        /// Updates the document icon when a control is added
        /// </summary>
        private void DocumentControlAdded(Object sender, ControlEventArgs e)
        {
            this.UpdateToolTipText();
            this.UpdateDocumentIcon(this.FileName);
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

