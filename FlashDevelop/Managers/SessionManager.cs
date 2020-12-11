// This is an open source non-commercial project. Dear PVS-Studio, please check it.
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
    internal class SessionManager
    {
        /// <summary>
        /// Saves the current session to a file
        /// </summary>
        public static void SaveSession(string file) => SaveSession(file, GetCurrentSession());

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
                var session = new Session();
                session = ObjectSerializer.Deserialize(file, session);
                session.Files ??= new List<string>();
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
                    var te = new DataEvent(EventType.RestoreSession, file, session);
                    EventManager.DispatchEvent(PluginBase.MainForm, te);
                    if (!te.Handled)
                    {
                        foreach (var fileToOpen in session.Files)
                        {
                            if (File.Exists(fileToOpen)) PluginBase.MainForm.OpenEditableDocument(fileToOpen);
                        }
                        RestoreDocks(session);
                        if (((MainForm) PluginBase.MainForm).DocumentsLength() == 0)
                        {
                            var ne = new NotifyEvent(EventType.FileEmpty);
                            EventManager.DispatchEvent(PluginBase.MainForm, ne);
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
        static void RestoreDocks(Session session)
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
                            prevPane = dockContent.DockPanel.Panes[nestedDock.NestIndex];
                            var ds = nestedDock.Alignment switch
                            {
                                DockAlignment.Top => DockStyle.Top,
                                DockAlignment.Left => DockStyle.Left,
                                DockAlignment.Bottom => DockStyle.Bottom,
                                _ => DockStyle.Right
                            };
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
            var documents = PluginBase.MainForm.Documents;
            for (int i = 0; i < documents.Length; i++)
            {
                var document = documents[i];
                if (document.SciControl is { } sci && !document.IsUntitled)
                {
                    if (document == PluginBase.MainForm.CurrentDocument)
                    {
                        session.Index = i;
                    }
                    session.Files.Add(sci.FileName);
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
                var content = (DockContent) document;
                var nestIndex = content.DockPanel.Panes.IndexOf(content.Pane.NestedDockingStatus.PreviousPane);
                if (nestIndex > -1)
                {
                    var paneIndex = content.DockPanel.Panes.IndexOf(content.Pane);
                    var align = content.Pane.NestedDockingStatus.Alignment;
                    var prop = content.Pane.NestedDockingStatus.Proportion;
                    var dock = new NestedDock(document.FileName, nestIndex, paneIndex, align, prop);
                    session.Nested.Add(dock);
                }
            }
            catch { /* No errors please... */ }
        }
    }

    [Serializable]
    public class Session : ISession
    {
        int index = 0;
        List<string> files = new List<string>();
        List<NestedDock> nested = new List<NestedDock>();
        SessionType type = SessionType.Startup;

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
        int nest = -1;
        int index = -1;
        string file = "";
        double prop = 0.5;
        DockAlignment align = DockAlignment.Right;

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