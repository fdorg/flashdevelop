using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using PluginCore.Helpers;
using PluginCore.Utilities;

namespace PluginCore.FRService
{
    public class FRConfiguration
    {
        /// <summary>
        /// Properties of the class 
        /// </summary> 
        protected string path;
        protected string mask;
        protected string source;
        protected bool recursive;
        protected List<string> files;
        protected OperationType type;
        protected FRSearch search;
        protected string replacement;
        protected bool cacheDocuments = false;
        protected bool updateSourceFile = true;
        protected IDictionary<string, ITabbedDocument> openDocuments = null;

        /// <summary>
        /// Enables the caching
        /// </summary>
        public bool CacheDocuments
        {
            get { return cacheDocuments; }
            set { cacheDocuments = value; }
        }

        /// <summary>
        /// Updates the source file only
        /// </summary>
        public bool UpdateSourceFileOnly
        {
            get { return updateSourceFile; }
            set { updateSourceFile = value; }
        }

        /// <summary>
        /// Warning: if this property is not null, the text will be replaced when running a background search
        /// </summary>
        public string Replacement
        {
            get { return replacement; }
            set { replacement = value; }
        }

        /// <summary>
        /// 
        /// </summary> 
        protected enum OperationType
        {
            FindInSource,
            FindInFile,
            FindInPath,
            FindInRange
        }

        /// <summary>
        /// Constructor of the class 
        /// </summary> 
        public FRConfiguration(List<string> files, FRSearch search)
        {
            this.type = OperationType.FindInRange;
            this.search = search;
            this.files = files;
        }
        public FRConfiguration(string fileName, string source, FRSearch search)
        {
            this.type = OperationType.FindInSource;
            this.path = fileName;
            this.search = search;
            this.source = source;
        }
        public FRConfiguration(string fileName, FRSearch search)
        {
            this.type = OperationType.FindInFile;
            this.path = fileName;
            this.search = search;
        }
        public FRConfiguration(string path, string fileMask, bool recursive, FRSearch search)
        {
            this.path = path;
            this.type = OperationType.FindInPath;
            this.recursive = recursive;
            this.mask = fileMask;
            this.search = search;
        }

        /// <summary>
        /// Gets the search
        /// </summary> 
        public FRSearch GetSearch()
        {
            return this.search;
        }

        /// <summary>
        /// Gets the source
        /// </summary>
        public string GetSource(string file)
        {
            switch (type)
            {
                case OperationType.FindInSource:
                    return this.source;

                default:
                    return ReadCurrentFileSource(file);
            }
        }

        /// <summary>
        /// Reads the source
        /// </summary>
        protected string ReadCurrentFileSource(string file)
        {
            if (cacheDocuments)
            {
                if (openDocuments == null) CacheOpenDocuments();
                if (openDocuments.ContainsKey(file)) return openDocuments[file].SciControl.Text;
            }
            return FileHelper.ReadFile(file);
        }

        /// <summary>
        /// Checks if the document is cached
        /// </summary>
        protected bool IsDocumentCached(string file)
        {
            return openDocuments.ContainsKey(file);
        }

        /// <summary>
        /// Caches the documents
        /// </summary>
        protected void CacheOpenDocuments()
        {
            this.openDocuments = new Dictionary<string, ITabbedDocument>();
            foreach (ITabbedDocument document in PluginBase.MainForm.Documents)
            {
                if (document.IsEditable)
                {
                    this.openDocuments[document.FileName] = document;
                }
            }
        }

        /// <summary>
        /// Updates the file source (ie. write the file)
        /// </summary>
        public void SetSource(string file, string src)
        {
            switch (type)
            {
                case OperationType.FindInSource:
                    this.source = src;
                    break;

                default:
                    EncodingFileInfo info = FileHelper.GetEncodingFileInfo(file);
                    if (this.updateSourceFile || !this.IsDocumentCached(file))
                    {
                        FileHelper.WriteFile(file, src, Encoding.GetEncoding(info.CodePage), info.ContainsBOM);
                    }
                    else 
                    {
                        // make this method thread safe
                        if ((PluginBase.MainForm as Form).InvokeRequired)
                        {
                            (PluginBase.MainForm as Form).BeginInvoke((MethodInvoker) delegate {
                                openDocuments[file].SciControl.Text = src;
                            });
                        }
                        else openDocuments[file].SciControl.Text = src;
                    }
                    break;
            }
        }

        /// <summary>
        /// Gets the files
        /// </summary>
        public List<string> GetFiles()
        {
            switch (type)
            {
                case OperationType.FindInRange:
                    return this.files;

                case OperationType.FindInSource:
                    if (this.files == null)
                    {
                        this.files = new List<string>();
                        this.files.Add(path);
                    }
                    return files;

                case OperationType.FindInFile:
                    if (this.files == null)
                    {
                        this.files = new List<string>();
                        this.files.Add(path);
                    }
                    return this.files;

                case OperationType.FindInPath:
                    if (this.files == null)
                    {
                        PathWalker walker = new PathWalker(this.path, this.mask, this.recursive);
                        this.files = walker.GetFiles();
                    }
                    return this.files;
            }
            return null;
        }

    }

}
