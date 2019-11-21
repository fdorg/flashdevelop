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
        protected bool cacheDocuments;
        protected bool updateSourceFile = true;
        protected IDictionary<string, ITabbedDocument> openDocuments;

        /// <summary>
        /// Enables the caching
        /// </summary>
        public bool CacheDocuments
        {
            get => cacheDocuments;
            set => cacheDocuments = value;
        }

        /// <summary>
        /// Updates the source file only
        /// </summary>
        public bool UpdateSourceFileOnly
        {
            get => updateSourceFile;
            set => updateSourceFile = value;
        }

        /// <summary>
        /// Warning: if this property is not null, the text will be replaced when running a background search
        /// </summary>
        public string Replacement
        {
            get => replacement;
            set => replacement = value;
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
            type = OperationType.FindInRange;
            this.search = search;
            this.files = files;
        }
        public FRConfiguration(string fileName, string source, FRSearch search)
        {
            type = OperationType.FindInSource;
            path = fileName;
            this.search = search;
            this.source = source;
        }
        public FRConfiguration(string fileName, FRSearch search)
        {
            type = OperationType.FindInFile;
            path = fileName;
            this.search = search;
        }
        public FRConfiguration(string path, string fileMask, bool recursive, FRSearch search)
        {
            this.path = path;
            type = OperationType.FindInPath;
            this.recursive = recursive;
            mask = fileMask;
            this.search = search;
        }

        /// <summary>
        /// Gets the search
        /// </summary> 
        public FRSearch GetSearch() => search;

        /// <summary>
        /// Gets the source
        /// </summary>
        public string GetSource(string file)
        {
            return type switch
            {
                OperationType.FindInSource => source,
                _ => ReadCurrentFileSource(file),
            };
        }

        /// <summary>
        /// Reads the source
        /// </summary>
        protected string ReadCurrentFileSource(string file)
        {
            if (cacheDocuments)
            {
                if (openDocuments is null) CacheOpenDocuments();
                if (openDocuments.ContainsKey(file)) return openDocuments[file].SciControl.Text;
            }
            return FileHelper.ReadFile(file);
        }

        /// <summary>
        /// Checks if the document is cached
        /// </summary>
        protected bool IsDocumentCached(string file) => openDocuments.ContainsKey(file);

        /// <summary>
        /// Caches the documents
        /// </summary>
        protected void CacheOpenDocuments()
        {
            openDocuments = new Dictionary<string, ITabbedDocument>();
            foreach (var document in PluginBase.MainForm.Documents)
            {
                if (document.IsEditable)
                {
                    openDocuments[document.FileName] = document;
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
                    source = src;
                    break;

                default:
                    if (updateSourceFile || !IsDocumentCached(file))
                    {
                        var info = FileHelper.GetEncodingFileInfo(file);
                        FileHelper.WriteFile(file, src, Encoding.GetEncoding(info.CodePage), info.ContainsBOM);
                    }
                    else 
                    {
                        // make this method thread safe
                        if (((Form) PluginBase.MainForm).InvokeRequired)
                        {
                            ((Form) PluginBase.MainForm).BeginInvoke((MethodInvoker) (() =>
                                openDocuments[file].SciControl.Text = src));
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
            return type switch
            {
                OperationType.FindInRange => files,
                OperationType.FindInSource => (files ??= new List<string> {path}),
                OperationType.FindInFile => (files ??= new List<string> {path}),
                OperationType.FindInPath => (files ??= new PathWalker(path, mask, recursive).GetFiles()),
            };
        }
    }
}