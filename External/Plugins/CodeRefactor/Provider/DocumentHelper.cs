using System.Collections.Generic;
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
        /// <summary>
        /// Constructor. Creates a new helper.  Stores the current state of 
        /// open files at this time.  Therefore, if there are temporary files 
        /// already open, this instance will not consider those files to be 
        /// temporary.  Consider this when managing multiple DocumentHelpers.
        /// </summary>
        public DocumentHelper()
        {
            InitiallyOpenedFiles = GetOpenDocuments();
        }

        public bool PreventClosing { get; } = false;

        /// <summary>
        /// Tracks files that have been opened.
        /// Key is the File Name.  Value is whether or not it's been used.
        /// If Value is false, it indicates the file is temporary and should be 
        /// closed.  If true, it indicates that even though the file was 
        /// opened, there is reportable/changed content in the file and it 
        /// should remain open.
        /// </summary>
        public IDictionary<string, bool> FilesOpenedAndUsed { get; } = new Dictionary<string, bool>();

        /// <summary>
        /// Keeps track of opened files.  Provides reference to the files' 
        /// associated DockContent so that they may be closed.
        /// </summary>
        public IDictionary<string, ITabbedDocument> FilesOpenedDocumentReferences { get; } = new Dictionary<string, ITabbedDocument>();

        /// <summary>
        /// Gets the collection of files opened when this DocumentHelper was created.
        /// </summary>
        public IDictionary<string, ITabbedDocument> InitiallyOpenedFiles { get; }

        /// <summary>
        /// Retrieves a list of the currently open documents.
        /// </summary>
        static IDictionary<string, ITabbedDocument> GetOpenDocuments()
        {
            var result = new Dictionary<string, ITabbedDocument>();
            foreach (var openDocument in PluginBase.MainForm.Documents)
            {
                if (openDocument.IsEditable)
                {
                    result[openDocument.FileName] = openDocument;
                }
            }
            return result;
        }

        /// <summary>
        /// Flags the given file as used (not temporary).
        /// This will prevent it from being purged later on.
        /// </summary>
        public void MarkDocumentToKeep(string fileName)
        {
            if (FilesOpenedAndUsed.ContainsKey(fileName))
            {
                FilesOpenedAndUsed[fileName] = true;
            }
            else
            {
                LoadDocument(fileName);
                if (FilesOpenedAndUsed.ContainsKey(fileName))
                {
                    FilesOpenedAndUsed[fileName] = true;
                }
            }
        }

        /// <summary>
        /// Loads the given document into FlashDevelop.  
        /// If the document was not already previously opened, this will flag 
        /// it as a temporary file.
        /// </summary>
        public ITabbedDocument LoadDocument(string fileName)
        {
            var result = (ITabbedDocument)PluginBase.MainForm.OpenEditableDocument(fileName);
            RegisterLoadedDocument(result);
            return result;
        }

        public void RegisterLoadedDocument(ITabbedDocument document)
        {
            //if it's null, it means it was already opened, or the caller sent us garbage
            if (document != null && !FilesOpenedAndUsed.ContainsKey(document.FileName))
            {
                //newly opened document.  Let's store it so we can close it later if it's not part of our result set.
                //false to indicate that it so far hasn't found any matching entries.
                FilesOpenedAndUsed.Add(document.FileName, false);
                FilesOpenedDocumentReferences.Add(document.FileName, document);
            }
        }

        /// <summary>
        /// Closes the given document and purges the stored indices for it.
        /// If the document is part of the initially opened files list, it 
        /// will not be closed or purged.
        /// </summary>
        public bool CloseDocument(string fileName)
        {
            if (FilesOpenedDocumentReferences.ContainsKey(fileName) && !InitiallyOpenedFiles.ContainsKey(fileName))
            {
                FilesOpenedDocumentReferences[fileName].Close();
                FilesOpenedAndUsed.Remove(fileName);
                FilesOpenedDocumentReferences.Remove(fileName);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Closes all temporary documents.
        /// </summary>
        public void CloseTemporarilyOpenedDocuments()
        {
            if (!PreventClosing)
            {
                // retrieve a list of documents to close
                var documentsToClose = new List<string>();
                foreach (var openedAndUsedFile in FilesOpenedAndUsed)
                {
                    // if the value is true, it means the document was flagged as permanent/changed, so we shouldn't close it
                    if (!openedAndUsedFile.Value)
                    {
                        documentsToClose.Add(openedAndUsedFile.Key);
                    }
                }
                // close each document
                foreach (var fileName in documentsToClose)
                {
                    CloseDocument(fileName);
                }
            }
        }
    }
}