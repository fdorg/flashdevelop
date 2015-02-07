using System;
using System.Collections.Generic;
using ASCompletion.Context;
using ScintillaNet;
using PluginCore;

namespace CodeRefactor.Provider
{
    /// <summary>
    /// A managing class for the retrieval and closting of temporary documents.
    /// Given the current architecture of FlashDevelop, this assists 
    /// Refactoring commands in keeping track of and closing any documents 
    /// that needed to be opened simply to check its class structure.
    /// </summary>
    public class DocumentHelper
    {
        private Boolean preventClosing = false;
        private IDictionary<String, Boolean> filesOpenedAndUsed = new Dictionary<String, Boolean>();
        private IDictionary<String, ITabbedDocument> filesOpenedDocumentReferences = new Dictionary<String, ITabbedDocument>();
        private IDictionary<String, ITabbedDocument> initiallyOpenedFiles;
       
        /// <summary>
        /// Constructor. Creates a new helper.  Stores the current state of 
        /// open files at this time.  Therefore, if there are temporary files 
        /// already open, this instance will not consider those files to be 
        /// temporary.  Consider this when managing multiple DocumentHelpers.
        /// </summary>
        public DocumentHelper()
        {
            this.initiallyOpenedFiles = this.GetOpenDocuments();
        }

        /// <summary>
        /// 
        /// </summary>
        public Boolean PreventClosing
        {
            get
            {
                return this.preventClosing;
            }
            set
            {
                this.preventClosing = value;
            }
        }

        /// <summary>
        /// Tracks files that have been opened.
        /// Key is the File Name.  Value is whether or not it's been used.
        /// If Value is false, it indicates the file is temporary and should be 
        /// closed.  If true, it indicates that even though the file was 
        /// opened, there is reportable/changed content in the file and it 
        /// should remain open.
        /// </summary>
        public IDictionary<String, Boolean> FilesOpenedAndUsed
        {
            get
            {
                return this.filesOpenedAndUsed;
            }
            protected set
            {
                this.filesOpenedAndUsed = value;
            }
        }

        /// <summary>
        /// Keeps track of opened files.  Provides reference to the files' 
        /// associated DockContent so that they may be closed.
        /// </summary>
        public IDictionary<String, ITabbedDocument> FilesOpenedDocumentReferences
        {
            get
            {
                return this.filesOpenedDocumentReferences;
            }
            protected set
            {
                this.filesOpenedDocumentReferences = value;
            }
        }

        /// <summary>
        /// Gets the collection of files opened when this DocumentHelper was created.
        /// </summary>
        public IDictionary<String, ITabbedDocument> InitiallyOpenedFiles
        {
            get { return initiallyOpenedFiles; }
        }

        /// <summary>
        /// Retrieves a list of the currently open documents.
        /// </summary>
        protected IDictionary<String, ITabbedDocument> GetOpenDocuments()
        {
            IDictionary<String, ITabbedDocument> openedFiles = new Dictionary<String, ITabbedDocument>();
            foreach (ITabbedDocument openDocument in PluginBase.MainForm.Documents)
            {
                if (openDocument.IsEditable)
                {
                    openedFiles[openDocument.FileName] = openDocument;
                }
            }
            return openedFiles;
        }

        /// <summary>
        /// Flags the given file as used (not temporary).
        /// This will prevent it from being purged later on.
        /// </summary>
        public void MarkDocumentToKeep(String fileName)
        {
            if (this.filesOpenedAndUsed.ContainsKey(fileName))
            {
                this.filesOpenedAndUsed[fileName] = true;
            }
            else
            {
                this.LoadDocument(fileName);
                if (this.filesOpenedAndUsed.ContainsKey(fileName))
                {
                    this.filesOpenedAndUsed[fileName] = true;
                }
            }
        }

        /// <summary>
        /// Loads the given document into FlashDevelop.  
        /// If the document was not already previously opened, this will flag 
        /// it as a temporary file.
        /// </summary>
        public ScintillaControl LoadDocument(String fileName)
        {
            ITabbedDocument newDocument = (ITabbedDocument)PluginBase.MainForm.OpenEditableDocument(fileName);
            this.RegisterLoadedDocument(newDocument);
            return ASContext.CurSciControl;
        }

        /// <summary>
        /// 
        /// </summary>
        public void RegisterLoadedDocument(ITabbedDocument document)
        {
            //if it's null, it means it was already opened, or the caller sent us garbage
            if (document != null && !filesOpenedAndUsed.ContainsKey(document.FileName))
            {
                //newly opened document.  Let's store it so we can close it later if it's not part of our result set.
                //false to indicate that it so far hasn't found any matching entries.
                this.filesOpenedAndUsed.Add(document.FileName, false);
                this.filesOpenedDocumentReferences.Add(document.FileName, document);
            }
        }

        /// <summary>
        /// Closes the given document and purges the stored indices for it.
        /// If the document is part of the initially opened files list, it 
        /// will not be closed or purged.
        /// </summary>
        public Boolean CloseDocument(String fileName)
        {
            if (this.filesOpenedDocumentReferences.ContainsKey(fileName) && !this.initiallyOpenedFiles.ContainsKey(fileName))
            {
                this.filesOpenedDocumentReferences[fileName].Close();
                this.filesOpenedAndUsed.Remove(fileName);
                this.filesOpenedDocumentReferences.Remove(fileName);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Closes all temporary documents.
        /// </summary>
        public void CloseTemporarilyOpenedDocuments()
        {
            if (!this.PreventClosing)
            {
                // retrieve a list of documents to close
                List<String> documentsToClose = new List<String>();
                foreach (KeyValuePair<String, Boolean> openedAndUsedFile in filesOpenedAndUsed)
                {
                    // if the value is true, it means the document was flagged as permanent/changed, so we shouldn't close it
                    if (!openedAndUsedFile.Value)
                    {
                        documentsToClose.Add(openedAndUsedFile.Key);
                    }
                }
                // close each document
                foreach (String fileName in documentsToClose)
                {
                    this.CloseDocument(fileName);
                }
            }
        }

    }

}
