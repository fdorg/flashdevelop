using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using WeifenLuo.WinFormsUI.Docking;
using PluginCore.Localization;
using FlashDevelop.Helpers;
using PluginCore.Utilities;
using PluginCore.Managers;
using PluginCore;

namespace FlashDevelop.Managers
{
    class SessionManager
    {
        /// <summary>
        /// Saves the current session to a file
        /// </summary>
        public static void SaveSession(String file)
        {
            Session session = GetCurrentSession();
            SaveSession(file, session);
        }
        public static void SaveSession(String file, Session session)
        {
            try
            {
                ObjectSerializer.Serialize(file, session);
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Loads and restores the saved session 
        /// </summary>
        public static void RestoreSession(String file, SessionType type)
        {
            try
            {
                Session session = new Session();
                session = (Session)ObjectSerializer.Deserialize(file, session);
                if (session.Files == null) session.Files = new List<string>();
                session.Type = type; // set the type here...
                RestoreSession(file, session);
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }
        public static void RestoreSession(String file, Session session)
        {
            try
            {
                Globals.MainForm.RestoringContents = true;
                Globals.MainForm.CloseAllDocuments(false);
                if (!Globals.MainForm.CloseAllCanceled)
                {
                    DataEvent te = new DataEvent(EventType.RestoreSession, file, session);
                    EventManager.DispatchEvent(Globals.MainForm, te);
                    if (!te.Handled)
                    {
                        for (Int32 i = 0; i < session.Files.Count; i++)
                        {
                            String fileToOpen = session.Files[i];
                            if (File.Exists(fileToOpen)) Globals.MainForm.OpenEditableDocument(fileToOpen);
                        }
                        RestoreDocks(session);
                        if (Globals.MainForm.Documents.Length == 0)
                        {
                            NotifyEvent ne = new NotifyEvent(EventType.FileEmpty);
                            EventManager.DispatchEvent(Globals.MainForm, ne);
                            if (!ne.Handled) Globals.MainForm.SmartNew(null, null);
                        }
                        DocumentManager.ActivateDocument(session.Index);
                    }
                }
                Globals.MainForm.RestoringContents = false;
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Restores the previous document docks
        /// </summary>
        private static void RestoreDocks(Session session)
        {
            try
            {
                DockPane prevPane;
                for (Int32 i = 0; i < session.Nested.Count; i++)
                {
                    NestedDock nestedDock = session.Nested[i];
                    DockContent dockContent = DocumentManager.FindDocument(nestedDock.FileName) as DockContent;
                    if (dockContent != null && nestedDock.NestIndex > -1)
                    {
                        if (dockContent.DockPanel.Panes.Count > nestedDock.PaneIndex)
                        {
                            prevPane = dockContent.DockPanel.Panes[nestedDock.PaneIndex];
                            dockContent.DockTo(prevPane, DockStyle.Fill, -1);
                        }
                        else if (dockContent.DockPanel.Panes.Count > nestedDock.NestIndex)
                        {
                            DockStyle ds = DockStyle.Right;
                            prevPane = dockContent.DockPanel.Panes[nestedDock.NestIndex];
                            if (nestedDock.Alignment == DockAlignment.Top) ds = DockStyle.Top;
                            else if (nestedDock.Alignment == DockAlignment.Left) ds = DockStyle.Left;
                            else if (nestedDock.Alignment == DockAlignment.Bottom) ds = DockStyle.Bottom;
                            dockContent.DockTo(prevPane, ds, -1, nestedDock.Proportion);
                        }
                    }
                }
            }
            catch { /* No errors please... */ }
        }

        /// <summary>
        /// Gets a session from the current documents
        /// </summary> 
        public static Session GetCurrentSession()
        {
            Session session = new Session();
            ITabbedDocument[] documents = Globals.MainForm.Documents;
            for (Int32 i = 0; i < documents.Length; i++)
            {
                ITabbedDocument document = documents[i];
                if (document.IsEditable && !document.IsUntitled)
                {
                    if (document == Globals.CurrentDocument)
                    {
                        session.Index = i;
                    }
                    session.Files.Add(document.FileName);
                    AddDocumentDock(document, session);
                }
                else session.Files.Add(document.Text);
            }
            return session;
        }

        /// <summary>
        /// Adds the document's dock state to the session
        /// </summary>
        public static void AddDocumentDock(ITabbedDocument document, Session session)
        {
            try
            {
                DockContent content = document as DockContent;
                double prop = content.Pane.NestedDockingStatus.Proportion;
                DockAlignment align = content.Pane.NestedDockingStatus.Alignment;
                Int32 paneIndex = content.DockPanel.Panes.IndexOf(content.Pane);
                Int32 nestIndex = content.DockPanel.Panes.IndexOf(content.Pane.NestedDockingStatus.PreviousPane);
                if (nestIndex > -1)
                {
                    NestedDock dock = new NestedDock(document.FileName, nestIndex, paneIndex, align, prop);
                    session.Nested.Add(dock);
                }
            }
            catch { /* No errors please... */ }
        }
    }

    [Serializable]
    public class Session : ISession
    {
        private Int32 index = 0;
        private List<String> files = new List<String>();
        private List<NestedDock> nested = new List<NestedDock>();
        private SessionType type = SessionType.Startup;

        public Session() {}
        public Session(Int32 index, List<String> files)
        {
            this.index = index;
            this.files = files;
        }
        public Session(Int32 index, List<String> files, SessionType type)
        {
            this.index = index;
            this.files = files;
            this.type = type;
        }
        public Int32 Index
        {
            get { return this.index; }
            set { this.index = value; }
        }
        public SessionType Type
        {
            get { return this.type; }
            set { this.type = value; }
        }
        public List<String> Files
        {
            get { return this.files; }
            set { this.files = value; }
        }
        public List<NestedDock> Nested
        {
            get { return this.nested; }
            set { this.nested = value; }
        }

    }

    [Serializable]
    public class NestedDock
    {
        private Int32 nest = -1;
        private Int32 index = -1;
        private String file = "";
        private double prop = 0.5;
        private DockAlignment align = DockAlignment.Right;

        public NestedDock() { }
        public NestedDock(String file, Int32 nest, Int32 index, DockAlignment align, double prop)
        {
            this.file = file;
            this.nest = nest;
            this.index = index;
            this.align = align;
            this.prop = prop;
        }
        public String FileName
        {
            get { return this.file; }
            set { this.file = value; }
        }
        public Int32 PaneIndex
        {
            get { return this.index; }
            set { this.index = value; }
        }
        public Int32 NestIndex
        {
            get { return this.nest; }
            set { this.nest = value; }
        }
        public DockAlignment Alignment
        {
            get { return this.align; }
            set { this.align = value; }
        }
        public double Proportion
        {
            get { return this.prop; }
            set { this.prop = value; }
        }

    }

}

