// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
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
                if (session.Files is null) session.Files = new List<string>();
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
                        foreach (var fileToOpen in session.Files)
                        {
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
                foreach (var nestedDock in session.Nested)
                {
                    if (DocumentManager.FindDocument(nestedDock.FileName) is DockContent dockContent && nestedDock.NestIndex > -1)
                    {
                        DockPane prevPane;
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
            var session = new Session();
            var documents = Globals.MainForm.Documents;
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
                DockContent content = (DockContent) document;
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
            get => index;
            set => index = value;
        }
        public SessionType Type
        {
            get => type;
            set => type = value;
        }
        public List<string> Files
        {
            get => files;
            set => files = value;
        }
        public List<NestedDock> Nested
        {
            get => nested;
            set => nested = value;
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
            get => file;
            set => file = value;
        }
        public int PaneIndex
        {
            get => index;
            set => index = value;
        }
        public int NestIndex
        {
            get => nest;
            set => nest = value;
        }
        public DockAlignment Alignment
        {
            get => align;
            set => align = value;
        }
        public double Proportion
        {
            get => prop;
            set => prop = value;
        }

    }

}

