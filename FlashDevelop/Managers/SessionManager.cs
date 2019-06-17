using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using PluginCore;
using PluginCore.Managers;
using PluginCore.Utilities;
using WeifenLuo.WinFormsUI.Docking;

namespace FlashDevelop.Managers
{
    class SessionManager
    {
        /// <summary>
        /// Saves the current session to a file
        /// </summary>
        public static void SaveSession(string file)
        {
            Session session = GetCurrentSession();
            SaveSession(file, session);
        }
        public static void SaveSession(string file, Session session)
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
        public static void RestoreSession(string file, SessionType type)
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
        public static void RestoreSession(string file, Session session)
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
                        for (int i = 0; i < session.Files.Count; i++)
                        {
                            string fileToOpen = session.Files[i];
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
                for (int i = 0; i < session.Nested.Count; i++)
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
            for (int i = 0; i < documents.Length; i++)
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
                int paneIndex = content.DockPanel.Panes.IndexOf(content.Pane);
                int nestIndex = content.DockPanel.Panes.IndexOf(content.Pane.NestedDockingStatus.PreviousPane);
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
        private int index = 0;
        private List<string> files = new List<string>();
        private List<NestedDock> nested = new List<NestedDock>();
        private SessionType type = SessionType.Startup;

        public Session() {}
        public Session(int index, List<string> files)
        {
            this.index = index;
            this.files = files;
        }
        public Session(int index, List<string> files, SessionType type)
        {
            this.index = index;
            this.files = files;
            this.type = type;
        }
        public int Index
        {
            get { return this.index; }
            set { this.index = value; }
        }
        public SessionType Type
        {
            get { return this.type; }
            set { this.type = value; }
        }
        public List<string> Files
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
        private int nest = -1;
        private int index = -1;
        private string file = "";
        private double prop = 0.5;
        private DockAlignment align = DockAlignment.Right;

        public NestedDock() { }
        public NestedDock(string file, int nest, int index, DockAlignment align, double prop)
        {
            this.file = file;
            this.nest = nest;
            this.index = index;
            this.align = align;
            this.prop = prop;
        }
        public string FileName
        {
            get { return this.file; }
            set { this.file = value; }
        }
        public int PaneIndex
        {
            get { return this.index; }
            set { this.index = value; }
        }
        public int NestIndex
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

