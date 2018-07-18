using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;
using ASCompletion.Commands;
using ASCompletion.Completion;
using ASCompletion.Generators;
using ASCompletion.Model;
using ASCompletion.Settings;
using PluginCore;
using PluginCore.Helpers;
using PluginCore.Managers;
using ScintillaNet;

namespace ASCompletion.Context
{
    public class ASContext: IASContext
    {
        #region internal
        // all contexts management
        protected static List<RegisteredContext> allContexts = new List<RegisteredContext>();
        protected static int currentLine;
        protected static IASContext context;
        protected static List<IASContext> validContexts;
        protected static ASContext defaultContext;
        protected static PluginMain plugin;
        // context state/models
        protected bool started;
        protected ContextSetupInfos contextSetup;
        protected List<PathModel> classPath;
        protected FileModel cFile;
        protected int cLine;
        protected MemberModel cMember;
        protected ClassModel cClass;
        protected bool inPrivateSection;
        protected FileModel topLevel;
        protected string lastClassWarning;
        protected internal CompletionCache completionCache;
        protected Timer cacheRefreshTimer;
        // path normalization
        protected static bool doPathNormalization;
        protected static string dirSeparator;
        protected static char dirSeparatorChar;
        protected static string dirAltSeparator;
        protected static char dirAltSeparatorChar;
        // settings
        protected IContextSettings settings;
        protected ContextFeatures features;
        protected string temporaryPath;
        protected string platform;
        protected int majorVersion;
        protected int minorVersion;
        // Checking / Quick Build
        protected bool runAfterBuild;
        protected string outputFile;
        protected string outputFileDetails;
        protected bool trustFileWanted;

        public ASContext()
        {
            features = new ContextFeatures();
            completionCache = new CompletionCache(this, null);
            cacheRefreshTimer = new Timer();
            cacheRefreshTimer.Interval = 1500; // delay initial refresh
            cacheRefreshTimer.Tick += cacheRefreshTimer_Tick;
        }
        #endregion

        #region static properties

        public static IMainForm MainForm => PluginBase.MainForm;

        public static ScintillaControl CurSciControl => PluginBase.MainForm.CurrentDocument?.SciControl;

        public static PluginUI Panel => plugin?.Panel;

        public static GeneralSettings CommonSettings => plugin.Settings as GeneralSettings;

        public static string DataPath => plugin.DataPath;

        //static private int setCount = 0;
        public static IASContext Context
        {
            get { return context; }
            set
            {
                if (value == null) context = defaultContext;
                else context = value;
                // update GUI
                if (Panel != null && context.CurrentModel != null)
                {
                    //TraceManager.Add("Refresh panel " + Path.GetFileName(context.CurrentModel.FileName));
                    Panel.UpdateView(context.CurrentModel);
                }
                //if (context.Settings != null) TraceManager.Add("Set context... " + (++setCount) + " " + context.Settings.LanguageId);
                // Update toolbar/menus state depending on the context state
                bool isValid = context.IsFileValid;
                foreach (ToolStripItem item in plugin.MenuItems)
                {
                    item.Enabled = isValid;
                }
            }
        }

        public static bool HasContext { get; protected internal set; }
        #endregion

        #region context properties
        
        public virtual IContextSettings Settings
        {
            get { return null; }
            set { }
        }

        public virtual ContextFeatures Features 
        {
            get { return features; }
            set { features = value; }
        }

        public virtual int CurrentLine
        {
            get { return cLine; }
            set
            {
                if (cFile != null)
                {
                    if (cLine != value)
                    {
                        cLine = value;
                        if (cFile.OutOfDate) UpdateCurrentFile(true);
                        else UpdateContext(cLine);
                    }
                    // require context
                    if (Context != this) Context = this;
                }
            }
        }

        public virtual MemberModel CurrentMember
        {
            get
            {
                if (cFile.OutOfDate) UpdateCurrentFile(true);
                return cMember;
            }
        }

        public virtual ClassModel CurrentClass
        {
            get
            {
                if (cFile.OutOfDate) UpdateCurrentFile(true);
                return cClass ?? ClassModel.VoidClass;
            }
            set { cClass = value; }
        }

        public virtual string CurrentFile
        {
            get
            {
                if (cFile == null) cFile = FileModel.Ignore;
                return cFile.FileName;
            }
            set
            {
                cacheRefreshTimer.Enabled = false;
                if (value == null)
                {
                    cFile = FileModel.Ignore;
                    cLine = -1;
                    return;
                }
                else
                {
                    // first use
                    if (!started) BuildClassPath();
                    // parse file
                    GetCurrentFileModel(value);
                }
                // require context
                Context = this;
            }
        }

        public virtual FileModel CurrentModel
        {
            get { return cFile; }
            set { cFile = value; }
        }

        /// <summary>
        /// Completion happens in an AS3 class private section
        /// </summary>
        public virtual bool InPrivateSection
        {
            get { return inPrivateSection; }
            set { inPrivateSection = value; }
        }

        /// <summary>
        /// Check if the current file is a valid file for the completion
        /// without parsing the file again.
        /// </summary>
        public virtual bool IsFileValid
        {
            get 
            {
                if (cFile == null || cFile == FileModel.Ignore || cFile.Version == 0 || Settings == null)
                    return false;
                if (cFile.InlinedRanges != null && CurSciControl != null)
                {
                    int position = CurSciControl.CurrentPos;
                    foreach (InlineRange range in cFile.InlinedRanges)
                    {
                        if (position > range.Start && position < range.End) return true;
                    }
                    return false;
                }
                else return true;
            }
        }

        public virtual bool CanBuild => false;

        /// <summary>
        /// Language built-in elements
        /// </summary>
        public FileModel TopLevel
        {
            get { return topLevel; }
            set { topLevel = value; }
        }

        /// <summary>
        /// Current active classpath
        /// </summary>
        public List<PathModel> Classpath
        {
            get { return classPath; }
            set { classPath = value; }
        }
        #endregion

        #region all contexts management
        /// <summary>
        /// Init completion engine context
        /// </summary>
        /// <param name="mainForm">Reference to MainForm</param>
        internal static void GlobalInit(PluginMain pluginMain)
        {
            dirSeparatorChar = Path.DirectorySeparatorChar;
            dirSeparator = dirSeparatorChar.ToString();
            dirAltSeparatorChar = Path.AltDirectorySeparatorChar;
            dirAltSeparator = dirAltSeparatorChar.ToString();
            doPathNormalization = (dirSeparator != dirAltSeparator);

            // language contexts
            plugin = pluginMain;
            validContexts = new List<IASContext>();
            context = null;
            try
            {
                context = defaultContext = new ASContext();
            }
            catch(Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Adds a language context
        /// </summary>
        /// <param name="contextReference">Language context</param>
        /// <param name="language">Language id (ie. Scintilla.ConfigurationLanguage)</param>
        public static void RegisterLanguage(IASContext contextReference, string language)
        {
            allContexts.Add(new RegisteredContext(contextReference, language, null));
        }

        /// <summary>
        /// Adds an inlined language context (ie. Javascript in Html)
        /// </summary>
        /// <param name="contextReference">Language context</param>
        /// <param name="language">File language id (ie. Scintilla.ConfigurationLanguage)</param>
        /// <param name="inlined">Inlined language id</param>
        public static void RegisterInlineLanguage(IASContext contextReference, string language, string inlined)
        {
            allContexts.Add(new RegisteredContext(contextReference, language, inlined));
        }

        /// <summary>
        /// Return the main context for a language
        /// </summary>
        /// <param name="lang">Language id (ie. Scintilla.ConfigurationLanguage)</param>
        public static IASContext GetLanguageContext(string lang)
        {
            if (lang == null) return null;
            lang = lang.ToLower();
            foreach (RegisteredContext reg in allContexts)
            {
                if (reg.Language == lang && reg.Inlined == null) return reg.Context;
            }
            return null;
        }

        /// <summary>
        /// Allows the Project Manager to define the languages projects' classpath
        /// </summary>
        /// <param name="lang">Language id (ie. Scintilla.ConfigurationLanguage)</param>
        /// <param name="classpath">Additional classpath</param>
        public static void SetLanguageClassPath(ContextSetupInfos setup)
        {
            foreach (RegisteredContext reg in allContexts)
            {
                if (reg.Language == setup.Lang) reg.Context.Setup(setup);
            }
        }

        /// <summary>
        /// Currently edited document
        /// </summary>
        internal static void SetCurrentFile(ITabbedDocument doc, bool shouldIgnore)
        {
            // reset previous contexts
            if (validContexts.Count > 0)
            {
                foreach (IASContext oldcontext in validContexts)
                    oldcontext.CurrentFile = null;
            }
            validContexts = new List<IASContext>();
            context = defaultContext;
            context.CurrentFile = null;

            // check document
            string filename = "";
            if (doc != null && doc.FileName != null)
            {
                filename = doc.FileName;
                if (doPathNormalization)
                    filename = filename.Replace(dirAltSeparator, dirSeparator);
            }
            else shouldIgnore = true;

            FileModel.Ignore.FileName = filename;
            // find the doc context(s)
            if (!shouldIgnore)
            {
                string lang = doc.SciControl.ConfigurationLanguage.ToLower();
                string ext = Path.GetExtension(filename);
                if (!string.IsNullOrEmpty(ext) && lang == "xml")
                    lang = ext.Substring(1).ToLower();
                foreach (RegisteredContext reg in allContexts)
                {
                    if (reg.Language == lang)
                    {
                        validContexts.Add(reg.Context);
                        reg.Context.CurrentFile = filename;
                    }
                }
                currentLine = -1;
                SetCurrentLine(doc.SciControl.CurrentLine);
            }
            // no context
            if (context == defaultContext) Panel.UpdateView(FileModel.Ignore);
            else Context.CheckModel(true);
        }

        internal static void SetCurrentLine(int line)
        {
            ScintillaControl sci = CurSciControl;
            if (validContexts.Count == 0 || sci == null)
            {
                HasContext = false;
                return;
            }
            if (line != currentLine)
            {
                // reevaluate active context
                HasContext = false;
                string needSyntax = null;
                currentLine = line;
                foreach (IASContext context in validContexts)
                {
                    context.CurrentLine = line;

                    // inline language coloring
                    if (context.CurrentModel != null && context.CurrentModel.InlinedRanges != null)
                    {
                        needSyntax = context.CurrentModel.InlinedIn;
                        int start = sci.MBSafeCharPosition(sci.PositionFromLine(line));
                        int end = start + sci.GetLine(line).Length;
                        foreach (InlineRange range in context.CurrentModel.InlinedRanges)
                        {
                            if (start > range.Start && end < range.End)
                            {
                                needSyntax = range.Syntax;
                                HasContext = true;
                                break;
                            }
                        }
                    }
                    else HasContext = true;
                }
                if (needSyntax != null)
                {
                    if (needSyntax != sci.ConfigurationLanguage)
                    {
                        sci.ConfigurationLanguage = needSyntax;
                        
                        if (!CommonSettings.DisableKnownTypesColoring && context is ASContext)
                        {
                            // known classes colorization
                            ASContext ctx = context as ASContext;
                            if (ctx.completionCache.Keywords.Length > 0)
                                sci.KeyWords(1, ctx.completionCache.Keywords); // additional-keywords index = 1
                        }
                        sci.Colourise(0, -1); // re-colorize the editor
                    }
                }
                Panel.Highlight(Context.CurrentClass, Context.CurrentMember);
            }
        }

        /// <summary>
        /// Current document's text changed
        /// </summary>
        public static void OnTextChanged(ScintillaControl sender, int position, int length, int linesAdded)
        {
            if (validContexts.Count > 0)
            {
                foreach (IASContext context in validContexts)
                    context.TrackTextChange(sender, position, length, linesAdded);
            }
        }

        /*private static void RepaintRanges(ScintillaNet.ScintillaControl sci)
        {
            if (context.CurrentModel == null || context.CurrentModel.InlinedRanges == null)
                return;

            int es = sci.EndStyled;
            int mask = (1 << sci.StyleBits) - 1;
            int pos = 0;
            foreach (InlineRange range in context.CurrentModel.InlinedRanges)
            {
                sci.StartStyling(pos, mask);
                sci.SetStyling(range.Start - pos, 1);
                pos = range.End;
            }
            sci.StartStyling(pos, mask);
            sci.SetStyling(sci.TextLength - pos, 1);
            sci.StartStyling(es, mask);
        }*/

        /// <summary>
        /// Clear and rebuild classpath models cache
        /// </summary>
        public static void RebuildClasspath()
        {
            Context = defaultContext;
            validContexts.Clear();
            foreach (RegisteredContext reg in allContexts)
                reg.Context.Reset();
            //PathExplorer.ClearAll();
            PathModel.ClearAll();

            Application.DoEvents();

            ITabbedDocument doc = PluginBase.MainForm.CurrentDocument;
            SetCurrentFile(doc, !doc.IsEditable); 
        }

        #endregion

        #region classpath management

        /// <summary>
        /// Mark pathes as "not in use"
        /// </summary>
        public virtual void ReleaseClasspath()
        {
            PathExplorer.BeginUpdate();

            if (started && classPath != null)
            {
                foreach (PathModel aPath in classPath) aPath.InUse = false;
            }
        }

        /// <summary>
        /// Mark pathes as "in use"
        /// </summary>
        public virtual void FinalizeClasspath()
        {
            if (classPath != null)
            {
                foreach (PathModel aPath in classPath) aPath.InUse = true;
            }
            PathModel.Compact();

            PathExplorer.EndUpdate();
        }

        /// <summary>
        /// Clear context state and classpaths
        /// </summary>
        public virtual void Reset()
        {
            cacheRefreshTimer.Enabled = false;
            if (classPath != null) classPath.Clear();
            cFile = FileModel.Ignore;
            cClass = ClassModel.VoidClass;
            cMember = null;
            cLine = 0;
            topLevel = null;
            started = false;
        }

        /// <summary>
        /// Add additional classpathes
        /// </summary>
        public virtual void Setup(ContextSetupInfos setup)
        {
            contextSetup = setup;
            BuildClassPath();
        }

        /// <summary>
        /// Add a path to the classpath
        /// </summary>
        /// <param name="path">Path to add</param>
        protected virtual PathModel AddPath(string path)
        {
            try
            {
                if (string.IsNullOrEmpty(path)) return null;
                if (Directory.Exists(path)) path = NormalizePath(path);
                else if (File.Exists(path)) path = NormalizeFilename(path);
                else return null;
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
                return null;
            }
            
            // avoid duplicated pathes
            string upath = path.ToUpper().TrimEnd('\\', '/');
            foreach(PathModel apath in classPath)
            {
                if (apath.Path.ToUpper() == upath)
                    return apath;
            }
            // add new path
            PathModel aPath = PathModel.GetModel(path, this);
            if (aPath != null)
            {
                classPath.Add(aPath);
                ExplorePath(aPath);
            }
            return aPath;
        }

        protected virtual PathModel AddPath(PathModel path)
        {
            // avoid duplicated pathes
            string upath = path.Path.ToUpper();
            foreach(PathModel apath in classPath)
            {
                if (!apath.IsTemporaryPath && apath.Path.ToUpper() == upath)
                        return apath;
            }
            // add new path
            classPath.Add(path);
            return path;
        }

        protected virtual void ManualExploration(PathModel path, IEnumerable<String> hideDirectories)
        {
            PathExplorer explorer = new PathExplorer(this, path);
            path.InUse = true;
            if (hideDirectories != null) explorer.HideDirectories(hideDirectories);
            explorer.OnExplorationDone += RefreshContextCache;
            explorer.OnExplorationProgress += ExplorationProgress;
            explorer.UseCache = !CommonSettings.DisableCache;
            explorer.Run();
        }

        protected virtual bool ExplorePath(PathModel path)
        {
            // exploration
            if (path.IsTemporaryPath)
            {
                path.IsTemporaryPath = false;
                path.WasExplored = false;
            }
            if (Settings != null && path.IsValid && !path.WasExplored
                && (!Settings.LazyClasspathExploration || path.IsVirtual))
            {
                //TraceManager.Add("EXPLORE: " + path.Path);
                PathExplorer explorer = new PathExplorer(this, path);
                explorer.OnExplorationDone += RefreshContextCache;
                explorer.OnExplorationProgress += ExplorationProgress;
                explorer.UseCache = !CommonSettings.DisableCache;
                explorer.Run();
                return true;
            }
            else if (path.WasExplored && path.IsVirtual)
            {
                // restore metadatas
                ExploreVirtualPath(path);
            }
            return false;
        }

        void ExplorationProgress(string state, int value, int max)
        {
            // SetStatus is thread safe
            plugin.Panel.SetStatus(state, value, max);
        }

        /// <summary>
        /// Called afer:
        /// - a PathExplorer has finished exploring
        /// - a PathModel has some internal change
        /// - an import was generated
        /// Warning: can be called many times.
        /// </summary>
        /// <param name="path">File or classname</param>
        public virtual void RefreshContextCache(string path)
        {
            if (plugin.Panel.InvokeRequired)
            {
                plugin.Panel.BeginInvoke((MethodInvoker)delegate { RefreshContextCache(path); });
                return;
            }
            completionCache.IsDirty = true;
            cacheRefreshTimer.Enabled = true;
        }

        void cacheRefreshTimer_Tick(object sender, EventArgs e)
        {
            cacheRefreshTimer.Enabled = false;
            cacheRefreshTimer.Interval = 200;
            if (completionCache.IsDirty && Context == this)
            {
                completionCache.IsDirty = false;
                UpdateCurrentFile(true);
                completionCache.IsDirty = true;
                GetVisibleExternalElements();
            }
        }

        /// <summary>
        /// Add the current class' base path to classpath
        /// </summary>
        /// <param name="path">Path to add</param>
        public virtual bool SetTemporaryPath(string path)
        {
            if (temporaryPath == path)
                return false;
            if (temporaryPath != null)
            {
                while (classPath.Count > 0 && classPath[0].IsTemporaryPath)
                {
                    classPath[0].InUse = false;
                    classPath.RemoveAt(0);
                }
                temporaryPath = null;
            }
            if (path != null && Directory.Exists(path))
            {
                // avoid duplicated pathes
                path = NormalizePath(path);
                foreach (PathModel apath in classPath)
                if (path.StartsWith(apath.Path, StringComparison.OrdinalIgnoreCase))
                {
                    temporaryPath = null;
                    return false;
                }
                // add path
                temporaryPath = path;
                PathModel tempModel = PathModel.GetModel(temporaryPath, this);
                if (!tempModel.WasExplored)
                {
                    tempModel.IsTemporaryPath = true;
                    tempModel.ReleaseWatcher();
                }
                tempModel.InUse = true;
                classPath.Insert(0, tempModel);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Classpathes & classes cache initialisation
        /// </summary>
        public virtual void BuildClassPath()
        {
            // To be implemented
            started = true;
        }

        /// <summary>
        /// Build a list of file mask to explore the classpath
        /// </summary>
        public virtual string[] GetExplorerMask()
        {
            if (Settings != null) return new string[] { "*" + Settings.DefaultExtension }; else return null;
        }

        /// <summary>
        /// User refreshes project tree
        /// </summary>
        public virtual void UserRefreshRequest()
        {
        }

        /// <summary>
        /// Refresh all contexts
        /// </summary>
        internal static void UserRefreshRequestAll()
        {
            foreach (RegisteredContext reg in allContexts)
                reg.Context.UserRefreshRequest();
        }

        #endregion

        #region model caching
        /// <summary>
        /// Track text modifications
        /// </summary>
        public virtual void TrackTextChange(ScintillaControl sender, int position, int length, int linesAdded)
        {
            if (cFile != FileModel.Ignore && !cFile.OutOfDate)
            {
                // test: modifications inside function body
                if (linesAdded == 0 && cMember != null && (cMember.Flags & FlagType.Function) > 0)
                {
                    // TODO  More precise text modifications tracking
                    int line = sender.LineFromPosition(position);
                    if (line > cMember.LineFrom && line <= cMember.LineTo)
                    {
                        // file AS3 in MXML ranges
                        if (cFile.InlinedRanges != null)
                        {
                            foreach (InlineRange range in cFile.InlinedRanges)
                            {
                                if (range.Start > position) range.Start += length;
                                if (range.End > position) range.End += length;
                            }
                        }
                        return;
                    }
                    /*string fbody = "";
                    for (int i = cMember.LineFrom; i <= cMember.LineTo; i++)
                    {
                        fbody += sci.GetLine(i);
                    }
                    position -= sci.PositionFromLine(cMember.LineFrom);
                    if (fbody.IndexOf('{') < position) return;*/
                }
                cFile.OutOfDate = true;
            }
        }

        /// <summary>
        /// Set current model out-of-date to force re-parse of the code when needed
        /// </summary>
        public virtual void SetOutOfDate()
        {
            if (cFile != FileModel.Ignore) cFile.OutOfDate = true;
        }
        /// <summary>
        /// Flag the model as up to date
        /// <returns>Model state before reseting the flag</returns>
        /// </summary>
        public virtual bool UnsetOutOfDate()
        {
            bool state = cFile.OutOfDate;
            if (cFile != FileModel.Ignore) cFile.OutOfDate = false;
            return state;
        }

        /// <summary>
        /// Retrieve a file model from the classpath cache
        /// </summary>
        /// <param name="fileName">Full path</param>
        /// <returns>File model</returns>
        public virtual FileModel GetCachedFileModel(string fileName)
        {
            if (string.IsNullOrEmpty(fileName) || !File.Exists(fileName)) 
                return new FileModel(fileName);

            FileModel nFile;
            fileName = PathHelper.GetLongPathName(fileName);
            if (classPath != null)
            {
                // check if in cache
                foreach (PathModel aPath in classPath)
                {
                    if (aPath.TryGetFile(fileName, out nFile))
                    {
                        nFile.Check();
                        return nFile;
                    }
                }
            }

            // parse and add to cache
            nFile = GetFileModel(fileName);
            if (classPath != null)
            {
                string upName = fileName.ToUpper();
                foreach (PathModel aPath in classPath)
                {
                    if (upName.StartsWith(aPath.Path, StringComparison.OrdinalIgnoreCase))
                    {
                        aPath.AddFile(nFile);
                        return nFile;
                    }
                }
            }

            // not owned
            return nFile;
        }

        /// <summary>
        /// Called if a FileModel needs filtering
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public virtual string FilterSource(string fileName, string src) => src;

        /// <summary>
        /// Called if a FileModel needs filtering
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual void FilterSource(FileModel model)
        {
            // to be implemented
        }

        /// <summary>
        /// Update model if needed and warn user if it has problems
        /// <param name="onFileOpen">Flag indicating it is the first model check</param>
        /// </summary>
        public virtual void CheckModel(bool onFileOpen)
        {
            if (cFile.OutOfDate)
            {
                cFile.Check();
                // update outline
                if (Context == this) Context = this;
            }
        }

        /// <summary>
        /// Parse a packaged library file
        /// </summary>
        /// <param name="path">Models owner</param>
        public virtual void ExploreVirtualPath(PathModel path)
        {
            // to be implemented
        }

        /// <summary>
        /// Create a new file model using the default file parser
        /// </summary>
        /// <param name="fileName">Full path</param>
        /// <returns>File model</returns>
        public virtual FileModel GetFileModel(string fileName)
        {
            var result = CreateFileModel(fileName);
            result.LastWriteTime = File.GetLastWriteTime(result.FileName);
            return GetCodeModel(result, FileHelper.ReadFile(result.FileName));
        }

        /// <summary>
        /// Create a new file model without parsing file
        /// </summary>
        /// <param name="fileName">Full path</param>
        /// <returns>File model</returns>
        public virtual FileModel CreateFileModel(string fileName)
        {
            if (string.IsNullOrEmpty(fileName) || !File.Exists(fileName))
                return new FileModel(fileName ?? "");
            var fileModel = new FileModel(PathHelper.GetLongPathName(fileName));
            fileModel.Context = this;
            return fileModel;
        }

        /// <summary>
        /// Search a fully qualified class in classpath
        /// </summary>
        /// <param name="package"></param>
        /// <param name="cname"></param>
        /// <param name="inPackage">Package reference for resolution</param>
        /// <returns></returns>
        public virtual ClassModel GetModel(string package, string cname, string inPackage)
        {
            // to be implemented
            return ClassModel.VoidClass;
        }

        /// <summary>
        /// Confirms that the FileModel should be added to the PathModel
        /// - typically classes whose context do not patch the classpath should be ignored
        /// </summary>
        /// <param name="aFile"></param>
        /// <param name="pathModel"></param>
        /// <returns></returns>
        public virtual bool IsModelValid(FileModel aFile, PathModel pathModel) => (aFile != null);

        /// <inheritdoc />
        public virtual FileModel GetCodeModel(string src) => GetCodeModel(src, false);

        /// <inheritdoc />
        public virtual FileModel GetCodeModel(string src, bool scriptMode) => GetCodeModel(CreateFileModel(string.Empty), src, scriptMode);

        /// <inheritdoc />
        public virtual FileModel GetCodeModel(FileModel result)
        {
            var fileName = result.FileName;
            if (!string.IsNullOrEmpty(fileName) && File.Exists(fileName)) GetCodeModel(result, FileHelper.ReadFile(fileName));
            return result;
        }

        /// <inheritdoc />
        public virtual FileModel GetCodeModel(FileModel result, string src) => GetCodeModel(result, src, false);

        /// <inheritdoc />
        public virtual FileModel GetCodeModel(FileModel result, string src, bool scriptMode)
        {
            var parser = GetCodeParser();
            parser.ScriptMode = scriptMode;
            if (src != null) parser.ParseSrc(result, src);
            return result;
        }

        /// <summary>
        /// Set local code parser features
        /// </summary>
        /// <returns></returns>
        protected virtual ASFileParser GetCodeParser() => new ASFileParser(context.Features);

        /// <summary>
        /// Build the file DOM
        /// </summary>
        /// <param name="fileName">File path</param>
        protected virtual void GetCurrentFileModel(string fileName)
        {
            cFile = GetCachedFileModel(fileName);
            cFile.FileName = fileName; // fix casing changes
            if (cFile.Context == null || cFile.Context != this)
            {
                cFile.Context = this;
                UpdateCurrentFile(false); // does update line & context
            }
            else if (CurSciControl != null)
            {
                cLine = CurSciControl.CurrentLine;
                UpdateContext(cLine);
            }
            // completion need refresh
            RefreshContextCache(null);
        }

        /// <summary>
        /// Refresh the file model
        /// </summary>
        /// <param name="updateUI">Update outline view</param>
        public virtual void UpdateCurrentFile(bool updateUI)
        {
            if (cFile == null || CurSciControl == null) return;
            GetCodeModel(cFile, CurSciControl.Text);
            cLine = CurSciControl.CurrentLine;
            UpdateContext(cLine);

            // update outline
            if (updateUI) Context = this;
        }

        /// <summary>
        /// Update the class/member context for the given line number.
        /// Be carefull to restore the context after calling it with a custom line number
        /// </summary>
        /// <param name="line"></param>
        public virtual void UpdateContext(int line)
        {
            if (cFile == FileModel.Ignore)
            {
                SetTemporaryPath(null);
                return;
            }

            if (SetTemporaryPath(NormalizePath(cFile.GetBasePath())))
            {
                var tPath = classPath[0];
                tPath.AddFile(cFile);
            }

            if (cFile.OutOfDate) UpdateCurrentFile(true);

            var ctx = GetDeclarationAtLine(line);
            if (ctx.InClass != cClass) 
            {
                cClass = ctx.InClass;
                // update "this" and "super" special vars
                UpdateTopLevelElements();
            }
            cMember = ctx.Member;

            // in package or after
            bool wasInPrivate = inPrivateSection;
            inPrivateSection = cFile.PrivateSectionIndex > 0 && line >= cFile.PrivateSectionIndex;
            if (wasInPrivate != inPrivateSection)
            {
                completionCache.IsDirty = true;
                completionCache.Classname = null;
            }

            // rebuild completion cache
            if (completionCache.IsDirty && IsFileValid &&
                (completionCache.Package != cFile.Package 
                || completionCache.Classname != cFile.GetPublicClass().Name))
                RefreshContextCache(null);
        }

        /// <summary>
        /// Resolve the class and member at the provided line index
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        public virtual ASResult GetDeclarationAtLine(int line)
        {
            var result = new ASResult();
            result.InClass = ClassModel.VoidClass;
            if (cFile == null) return result;
            // current class
            foreach (var aClass in cFile.Classes)
            {
                if (aClass.LineFrom <= line && aClass.LineTo >= line)
                {
                    result.InClass = aClass;

                    // current member
                    foreach (MemberModel member in result.InClass.Members)
                    {
                        if (member.LineFrom <= line && member.LineTo >= line)
                        {
                            result.Member = member;
                            return result;
                        }
                        if (member.LineFrom > line) return result;
                    }
                    return result;
                }
            }

            // current member
            foreach (MemberModel member in cFile.Members)
            {
                if (member.LineFrom <= line && member.LineTo >= line)
                {
                    result.Member = member;
                    return result;
                }
                if (member.LineFrom > line) return result;
            }
            return result;
        }

        #endregion

        #region language elements resolution

        /// <summary>
        /// Default types/member visibility
        /// </summary>
        public virtual Visibility DefaultVisibility => Visibility.Public;

        /// <summary>
        /// Default types inheritance
        /// </summary>
        /// <param name="package">File package</param>
        /// <param name="classname">Class name</param>
        /// <returns>Inherited type</returns>
        public virtual string DefaultInheritance(string package, string classname)
        {
            // no inheritance
            return null;
        }

        /// <summary>
        /// Prepare completion's intrinsic known vars/methods/classes
        /// </summary>
        protected virtual void InitTopLevelElements()
        {
            // to be implemented
        }

        /// <summary>
        /// Update completion intrinsic know vars
        /// </summary>
        protected virtual void UpdateTopLevelElements()
        {
            // to be implemented
        }

        /// <summary>
        /// Evaluates the visibility of one given type from another
        /// </summary>
        /// <param name="inClass"></param>
        /// <param name="withClass"></param>
        /// <returns>Completion visibility</returns>
        public virtual Visibility TypesAffinity(ClassModel inClass, ClassModel withClass)
        {
            // show all
            return 0;
        }

        /// <summary>
        /// Top-level elements lookup
        /// </summary>
        /// <param name="token">Element to search</param>
        /// <param name="result">Response structure</param>
        public virtual void ResolveTopLevelElement(string token, ASResult result)
        {
            // to be implemented
            // or let empty
        }

        /// <summary>
        /// Return imported classes list (not null)
        /// </summary>
        /// <param name="inFile">Current file</param>
        public virtual MemberList ResolveImports(FileModel inFile)
        {
            // to be implemented
            return new MemberList();
        }

        /// <summary>
        /// Check if a type is already in the file's imports
        /// </summary>
        /// <param name="member">Element to search in imports</param>
        /// <param name="atLine">Position in the file</param>
        public virtual bool IsImported(MemberModel member, int atLine)
        {
            // to be implemented
            return false;
        }

        /// <summary>
        /// Retrieves a class model from its name
        /// </summary>
        /// <param name="cname">Class (short or full) name</param>
        /// <param name="inFile">Current file</param>
        /// <returns>A parsed class or an empty ClassModel if the class is not found</returns>
        public virtual ClassModel ResolveType(string cname, FileModel inFile) => ClassModel.VoidClass;

        public virtual ClassModel ResolveToken(string token, FileModel inFile) => ClassModel.VoidClass;

        /// <summary>
        /// Retrieves a package content
        /// </summary>
        /// <param name="name">Package path</param>
        /// <param name="onlyUserDefined">Ignore language's intrinsic pathes</param>
        /// <returns>Package folders and types</returns>
        public virtual FileModel ResolvePackage(string name, bool onlyUserDefined)
        {
            // to be implemented
            return null;
        }

        /// <summary>
        /// Return the top-level elements (this, super) for the current file
        /// </summary>
        /// <returns></returns>
        public virtual MemberList GetTopLevelElements()
        {
            // to be implemented
            return new MemberList();
        }

        /// <summary>
        /// Return the visible elements (types, package-level declarations) visible from the current file
        /// </summary>
        /// <returns></returns>
        public virtual MemberList GetVisibleExternalElements()
        {
            // to be implemented
            completionCache = new CompletionCache(this, null);
            return new MemberList();
        }

        /// <summary>
        /// Return the full project classes list
        /// </summary>
        /// <returns></returns>
        public virtual MemberList GetAllProjectClasses()
        {
            return new MemberList();
        }

        /// <inheritdoc />
        public virtual string GetDefaultValue(string type) => null;

        public virtual IEnumerable<string> DecomposeTypes(IEnumerable<string> types) => types;
        #endregion

        #region operations on text insertion

        /// <summary>
        /// When an item from the completion list is inserted
        /// </summary>
        /// <param name="sci"></param>
        /// <param name="position"></param>
        /// <param name="text"></param>
        /// <returns>Indicator that the event is handled</returns>
        public virtual bool OnCompletionInsert(ScintillaControl sci, int position, string text, char trigger)
        {
            // override to do special handling operations (and return true)
            return false;
        }

        #endregion

        #region operations in the outline view

        /// <summary>
        /// When selecting a node in the outline view
        /// </summary>
        /// <param name="node"></param>
        public virtual void OnSelectOutlineNode(TreeNode node)
        {
            ClassModel aClass;
            // imports
            if (node.Tag as string == "import")
            {
                aClass = ResolveType(node.Text, CurrentModel);
                if (!aClass.IsVoid() && File.Exists(aClass.InFile.FileName))
                {
                    MainForm.OpenEditableDocument(aClass.InFile.FileName, false);
                    string name = (aClass.InFile.Version < 3) ? aClass.QualifiedName : aClass.Name;
                    ASComplete.LocateMember("(class|interface|abstract)", name, aClass.LineFrom);
                }
            }
            // classes
            else if (node.Tag as string == "class")
            {
                aClass = Context.CurrentModel.GetClassByName(node.Text);
                if (!aClass.IsVoid())
                {
                    string name = (aClass.InFile.Version < 3) ? aClass.QualifiedName : aClass.Name;
                    ASComplete.LocateMember("(class|interface|abstract)", name, aClass.LineFrom);
                }
            }
            else if (node.Tag is string)
            {
                string[] info = ((string) node.Tag).Split('@');
                int line;
                if (info.Length == 2 && int.TryParse(info[1], out line))
                {
                    ASComplete.LocateMember("(function|var|const|get|set|property|#region|namespace|,)", info[0], line);
                }
            }
        }

        #endregion

        #region Custom code completion
        /// <summary>
        /// Let contexts handle code completion
        /// </summary>
        /// <param name="sci">Scintilla control</param>
        /// <param name="expression">Completion context</param>
        /// <param name="autoHide">Auto-started completion (is false when pressing Ctrl+Space)</param>
        /// <returns>Null (not handled) or member list</returns>
        public virtual MemberList ResolveDotContext(ScintillaControl sci, ASExpr expression, bool autoHide) => null;

        /// <inheritdoc />
        public virtual void ResolveDotContext(ScintillaControl sci, ASExpr expression, MemberList result)
        {
        }

        /// <summary>
        /// Let contexts handle code completion
        /// </summary>
        /// <param name="sci">Scintilla control</param>
        /// <param name="expression">Completion context</param>
        /// <param name="autoHide">Auto-started completion (is false when pressing Ctrl+Space)</param>
        /// <returns>Null (not handled) or function signature</returns>
        public virtual MemberModel ResolveFunctionContext(ScintillaControl sci, ASExpr expression, bool autoHide) => null;

        public virtual bool HandleGotoDeclaration(ScintillaControl sci, ASExpr expression) => false;

        public IContextualGenerator CodeGenerator { get; protected set; } = new ASGenerator();

        public IContextualGenerator DocumentationGenerator { get; protected set; } = new DocumentationGenerator();

        public ASComplete CodeComplete { get; protected set; } = new ASComplete();
        #endregion

        #region plugin commands
        /// <summary>
        /// Delete current class's cached file of the compiler
        /// </summary>
        public virtual void RemoveClassCompilerCache()
        {
            // to be implemented
        }

        /// <summary>
        /// Browse to the first package folder in the classpath
        /// </summary>
        /// <param name="package">Package to show in the Files Panel</param>
        /// <returns>A folder was found and displayed</returns>
        public bool BrowseTo(string package)
        {
            package = package.Replace('.',dirSeparatorChar);
            foreach (PathModel aPath in classPath)
            {
                string path = Path.Combine(aPath.Path, package);
                if (Directory.Exists(path))
                {
                    DataEvent de = new DataEvent(EventType.Command, "FileExplorer.BrowseTo", path);
                    EventManager.DispatchEvent(this, de);
                    return de.Handled;
                }
            }
            return false;
        }

        /// <summary>
        /// Retrieve the context's default compiler path
        /// </summary>
        public virtual string GetCompilerPath()
        {
            // to be implemented
            return null;
        }

        /// <summary>
        /// Check current file's syntax
        /// </summary>
        public virtual void CheckSyntax()
        {
            // to be implemented
        }

        /// <summary>
        /// Run the command-line compiler in the current files's context
        /// </summary>
        /// <param name="append">Additional compiler switches</param>
        public virtual void RunCMD(string append)
        {
            // to be implemented
        }

        /// <summary>
        /// Calls compiler with default/automatic parameters (ie. quick build)
        /// </summary>
        public virtual bool BuildCMD(bool failSilently)
        {
            // to be implemented
            return false;
        }

        /// <summary>
        /// End of the CMD execution - if a SWF has been built, play it
        /// </summary>
        /// <param name="result">Execution result</param>
        public virtual void OnProcessEnd(string result)
        {
            if (Settings != null && GetStatusText() == Settings.CheckSyntaxRunning)
            {
                SetStatusText(Settings.CheckSyntaxDone);
            }

            if (outputFile == null) return;
            string swf = outputFile;
            outputFile = null;

            // on error, don't play
            if (!result.EndsWithOrdinal("(0)"))
                return;

            // remove quotes
            if (swf.StartsWith('\"')) swf = swf.Substring(1, swf.Length-2);

            // allow network access to the SWF
            if (trustFileWanted)
            {
                FileInfo info = new FileInfo(swf);
                string path = info.Directory.FullName;
                string trustFile = "FlashDevelop.cfg";
                CreateTrustFile.Run(trustFile, path);
            }

            // stop here if the user doesn't want to automatically play the SWF
            if (!runAfterBuild) return;

            // other plugin may handle the SWF playing
            DataEvent dePlay = new DataEvent(EventType.Command, "PlaySWF", swf + outputFileDetails);
            EventManager.DispatchEvent(this, dePlay);
            if (dePlay.Handled) return;

            try
            {
                // change current directory
                //string currentPath = System.IO.Directory.GetCurrentDirectory();
                //System.IO.Directory.SetCurrentDirectory(CurrentClass.BasePath);
                // run
                Process.Start(swf);
                // restaure current directory
                //if (System.IO.Directory.GetCurrentDirectory() == CurrentClass.BasePath)
                //System.IO.Directory.SetCurrentDirectory(currentPath);
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Generate an instrinsic class
        /// </summary>
        /// <param name="files">Semicolon-separated source & destination files</param>
        public void MakeIntrinsic(string files)
        {
            string src = null;
            string dest = null;
            if (!string.IsNullOrEmpty(files))
            {
                string[] list = files.Split(';');
                if (list.Length == 1) dest = list[0];
                else {
                    src = list[0];
                    dest = list[1];
                }
            }
            var aFile = src == null ? cFile : GetCodeModel(src);
            if (aFile.Version == 0) return;
            //
            string code = aFile.GenerateIntrinsic(false);

            // no destination, replace text
            if (dest == null)
            {
                MainForm.CallCommand("New", null);
                ScintillaControl sci = CurSciControl;
                if (sci != null)
                {
                    sci.CurrentPos = 0;
                    sci.Text = code;
                }
                return;
            }

            // write destination
            try
            {
                File.WriteAllText(dest, code, Encoding.UTF8);
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }
        #endregion

        #region common tool methods
        public static void SetStatusText(string text)
        {
            MainForm.StatusStrip.Items[0].Text = "  " + text;
        }
        protected static string GetStatusText()
        {
            if (MainForm.StatusStrip.Items[0].Text.Length > 2)
                return MainForm.StatusStrip.Items[0].Text.Substring(2);
            else
                return "";
        }

        public static string NormalizeFilename(string path)
        {
            if (string.IsNullOrEmpty(path)) return "";
            path = path.Trim();
            if (path.Length == 0) return path;
            if (doPathNormalization)
                path = path.Replace(dirAltSeparator, dirSeparator);
            path = path.Replace(dirSeparator + dirSeparator, dirSeparator);
            return PathHelper.GetLongPathName(path);
        }
        public static string NormalizePath(string path)
        {
            if (string.IsNullOrEmpty(path)) return "";
            path = path.Trim();
            if (path.Length == 0) return path;
            if (doPathNormalization)
                path = path.Replace(dirAltSeparator, dirSeparator);
            if (!path.EndsWithOrdinal(dirSeparator))
                path += dirSeparator;
            path = path.Replace(dirSeparator + dirSeparator, dirSeparator);
            return PathHelper.GetLongPathName(path);
        }
        public static string GetLastStringToken(string str, string sep)
        {
            if (str == null) return "";
            if (sep == null) return str;
            int p = str.LastIndexOfOrdinal(sep);
            return (p >= 0) ? str.Substring(p + 1) : str;
        }

        public static void ParseVersion(string version, ref int majorVersion, ref int minorVersion)
        {
            //if (version == "0.0") return;
            if (string.IsNullOrEmpty(version)) return;
            string[] parts = version.Split('.');
            int.TryParse(parts[0], out majorVersion);
            if (parts.Length > 1) int.TryParse(parts[1], out minorVersion);
        }

        #endregion
    }

    #region Registered Context class
    public class RegisteredContext
    {
        public IASContext Context;
        public string Language;
        public string Inlined;
        public bool SourceFilter;

        public RegisteredContext(IASContext context, string language, string inlinedLanguage)
        {
            Context = context;
            Language = language.ToLower();
            Inlined = inlinedLanguage?.ToLower();
            //TraceManager.Add("Register context: " + language + " (" + inlinedLanguage + ")");
        }
    }

    public class ContextSetupInfos
    {
        public string Lang;
        public string Platform;
        public string Version;
        public string TargetBuild;
        public string[] Classpath;
        public string[] HiddenPaths;
        public List<String> AdditionalPaths;
    }

    #endregion

    #region Completion cache
    public class CompletionCache
    {
        private bool isDirty;

        public string Package;
        public string Classname;
        public MemberList Elements;
        public MemberList AllTypes;
        public string Keywords;
        public MemberList Imports;
        public bool IsDirty
        {
            get { return isDirty; }
            set { isDirty = value; Imports = null; }
        }

        public CompletionCache(ASContext context, MemberList elements)
        {
            if (context.CurrentModel == null)
            {
                Package = "";
                Classname = "";
            }
            else
            {
                Package = context.CurrentModel.Package;
                Classname = context.CurrentModel.GetPublicClass().Name;
            }
            AllTypes = null;
            Elements = elements;
            Keywords = GetKeywords();
        }

        /// <summary>
        /// Build scintilla keywords from class names
        /// </summary>
        private string GetKeywords()
        {
            if (Elements == null) return "";
            List<string> keywords = new List<string>();
            foreach (MemberModel item in Elements)
            {
                if ((item.Flags & FlagType.Package) == 0)
                {
                    string name = item.Name;
                    if (name.IndexOf('<') > 0)
                    {
                        if (name.IndexOfOrdinal(".<") > 0) name = name.Substring(0, name.IndexOfOrdinal(".<"));
                        else name = name.Substring(0, name.IndexOf('<'));
                    }
                    if (name.IndexOf('.') > 0) name = name.Substring(name.LastIndexOf('.') + 1);
                    if (!keywords.Contains(name)) keywords.Add(name);
                }
            }
            return string.Join(" ", keywords.ToArray());
        }
    }
    #endregion
}
