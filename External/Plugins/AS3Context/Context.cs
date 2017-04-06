using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Timers;
using System.Windows.Forms;
using AS3Context.Compiler;
using ASCompletion.Completion;
using ASCompletion.Context;
using ASCompletion.Model;
using PluginCore;
using PluginCore.Controls;
using PluginCore.Helpers;
using PluginCore.Localization;
using PluginCore.Managers;
using ScintillaNet;
using ScintillaNet.Enums;
using SwfOp;
using Timer = System.Timers.Timer;

namespace AS3Context
{
    public class Context : AS2Context.Context
    {
        static readonly protected Regex re_genericType =
            new Regex("(?<gen>[^<]+)\\.<(?<type>.+)>$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        
        // C:\path\to\Main.as$raw$:31: col: 1:  Error #1084: Syntax error: expecting rightbrace before end of program.
        static readonly protected Regex re_syntaxError =
            new Regex("(?<filename>.*)\\$raw\\$:(?<line>[0-9]+): col: (?<col>[0-9]+):(?<desc>.*)", RegexOptions.Compiled);
        
        static readonly protected Regex re_customAPI =
            new Regex("[/\\\\](playerglobal|airglobal|builtin)\\.swc", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        #region initialization
        private AS3Settings as3settings;
        private bool hasAIRSupport;
        private bool hasMobileSupport;
        private MxmlFilterContext mxmlFilterContext; // extract inlined AS3 ranges & MXML tags
        private Timer timerCheck;
        private string fileWithSquiggles;
        protected bool mxmlEnabled;

        /// <summary>
        /// Do not call directly
        /// </summary>
        public Context()
        {
        }

        public Context(AS3Settings initSettings)
        {
            as3settings = initSettings;

            /* AS-LIKE OPTIONS */

            hasLevels = false;
            docType = "flash.display.MovieClip";

            /* DESCRIBE LANGUAGE FEATURES */

            mxmlEnabled = true;

            // language constructs
            features.hasPackages = true;
            features.hasNamespaces = true;
            features.hasImports = true;
            features.hasImportsWildcard = true;
            features.hasClasses = true;
            features.hasExtends = true;
            features.hasImplements = true;
            features.hasInterfaces = true;
            features.hasEnums = false;
            features.hasGenerics = true;
            features.hasEcmaTyping = true;
            features.hasVars = true;
            features.hasConsts = true;
            features.hasMethods = true;
            features.hasStatics = true;
            features.hasOverride = true;
            features.hasTryCatch = true;
            features.hasE4X = true;
            features.hasStaticInheritance = true;
            features.checkFileName = true;

            // allowed declarations access modifiers
            Visibility all = Visibility.Public | Visibility.Internal | Visibility.Protected | Visibility.Private;
            features.classModifiers = all;
            features.varModifiers = all;
            features.constModifiers = all;
            features.methodModifiers = all;

            // default declarations access modifiers
            features.classModifierDefault = Visibility.Internal;
            features.varModifierDefault = Visibility.Internal;
            features.methodModifierDefault = Visibility.Internal;

            // keywords
            features.dot = ".";
            features.voidKey = "void";
            features.objectKey = "Object";
            features.booleanKey = "Boolean";
            features.numberKey = "Number";
            features.stringKey = "String";
            features.arrayKey = "Array";
            features.dynamicKey = "*";
            features.importKey = "import";
            features.typesPreKeys = new string[] { "import", "new", "typeof", "is", "as", "extends", "implements" };
            features.codeKeywords = new string[] { 
                "var", "function", "const", "new", "delete", "typeof", "is", "as", "return", 
                "break", "continue", "if", "else", "for", "each", "in", "while", "do", "switch", "case", "default", "with",
                "null", "true", "false", "try", "catch", "finally", "throw", "use", "namespace"
            };
            features.accessKeywords = new string[] { 
                "native", "dynamic", "final", "public", "private", "protected", "internal", "static", "override"
            };
            features.declKeywords = new string[] { "var", "function", "const", "namespace", "get", "set" };
            features.typesKeywords = new string[] { "import", "class", "interface" };
            features.varKey = "var";
            features.constKey = "const";
            features.functionKey = "function";
            features.getKey = "get";
            features.setKey = "set";
            features.staticKey = "static";
            features.finalKey = "final";
            features.overrideKey = "override";
            features.publicKey = "public";
            features.internalKey = "internal";
            features.protectedKey = "protected";
            features.privateKey = "private";
            features.intrinsicKey = "extern";
            features.namespaceKey = "namespace";

            /* INITIALIZATION */

            settings = initSettings;
            //BuildClassPath(); // defered to first use

            // live syntax checking
            timerCheck = new Timer(500);
            timerCheck.SynchronizingObject = PluginBase.MainForm as Form;
            timerCheck.AutoReset = false;
            timerCheck.Elapsed += new ElapsedEventHandler(timerCheck_Elapsed);
            FlexShells.SyntaxError += new SyntaxErrorHandler(FlexShell_SyntaxError);
        }
        #endregion

        #region classpath management
        /// <summary>
        /// Classpathes & classes cache initialisation
        /// </summary>
        public override void BuildClassPath()
        {
            ReleaseClasspath();
            started = true;
            if (as3settings == null) throw new Exception("BuildClassPath() must be overridden");
            if (contextSetup == null)
            {
                contextSetup = new ContextSetupInfos();
                contextSetup.Lang = settings.LanguageId;
                contextSetup.Platform = "Flash Player";
                contextSetup.Version = as3settings.DefaultFlashVersion;
            }

            // external version definition
            platform = contextSetup.Platform;
            majorVersion = 10;
            minorVersion = 0;
            ParseVersion(contextSetup.Version, ref majorVersion, ref minorVersion);
            hasAIRSupport = platform == "AIR" || platform == "AIR Mobile";
            hasMobileSupport = platform == "AIR Mobile";

            string cpCheck = contextSetup.Classpath != null ?
                String.Join(";", contextSetup.Classpath).Replace('\\', '/') : "";

            // check if CP contains a custom playerglobal.swc
            bool hasCustomAPI = re_customAPI.IsMatch(cpCheck);

            //
            // Class pathes
            //
            classPath = new List<PathModel>();
            MxmlFilter.ClearCatalogs();
            MxmlFilter.AddProjectManifests();

            // SDK
            string compiler = PluginBase.CurrentProject != null
                ? PluginBase.CurrentProject.CurrentSDK
                : as3settings.GetDefaultSDK().Path;

            char S = Path.DirectorySeparatorChar;
            if (compiler == null) 
                compiler = Path.Combine(PathHelper.ToolDir, "flexlibs");
            string frameworks = compiler + S + "frameworks";
            string sdkLibs = frameworks + S + "libs";
            string sdkLocales = frameworks + S + "locale" + S + PluginBase.MainForm.Settings.LocaleVersion;
            string fallbackLibs = PathHelper.ResolvePath(PathHelper.ToolDir + S + "flexlibs" + S + "frameworks" + S + "libs");
            string fallbackLocale = PathHelper.ResolvePath(PathHelper.ToolDir + S + "flexlibs" + S + "frameworks" + S + "locale" + S + "en_US");
            List<string> addLibs = new List<string>();
            List<string> addLocales = new List<string>();

            if (!Directory.Exists(sdkLibs) && !sdkLibs.StartsWith('$')) // fallback
            {
                sdkLibs = PathHelper.ResolvePath(PathHelper.ToolDir + S + "flexlibs" + S + "frameworks" + S + "libs" + S + "player");
            }

            if (majorVersion > 0 && !String.IsNullOrEmpty(sdkLibs) && Directory.Exists(sdkLibs))
            {
                // core API SWC
                if (!hasCustomAPI)
                    if (hasAIRSupport)
                    {
                        addLibs.Add("air" + S + "airglobal.swc");
                        addLibs.Add("air" + S + "aircore.swc");
                        addLibs.Add("air" + S + "applicationupdater.swc");
                    }
                    else 
                    {
                        bool swcPresent = false;
                        string playerglobal = MatchPlayerGlobalExact(majorVersion, minorVersion, sdkLibs);
                        if (playerglobal != null) swcPresent = true;
                        else playerglobal = MatchPlayerGlobalExact(majorVersion, minorVersion, fallbackLibs);
                        if (playerglobal == null) playerglobal = MatchPlayerGlobalAny(ref majorVersion, ref minorVersion, fallbackLibs);
                        if (playerglobal == null) playerglobal = MatchPlayerGlobalAny(ref majorVersion, ref minorVersion, sdkLibs);
                        if (playerglobal != null)
                        {
                            // add missing SWC in new SDKs
                            if (!swcPresent && sdkLibs.IndexOfOrdinal(S + "flexlibs") < 0 && Directory.Exists(compiler))
                            {
                                string swcDir = sdkLibs + S + "player" + S;
                                if (!Directory.Exists(swcDir + "9") && !Directory.Exists(swcDir + "10"))
                                    swcDir += majorVersion + "." + minorVersion;
                                else
                                    swcDir += majorVersion;
                                try
                                {
                                    if (!File.Exists(swcDir + S + "playerglobal.swc"))
                                    {
                                        Directory.CreateDirectory(swcDir);
                                        File.Copy(playerglobal, swcDir + S + "playerglobal.swc");
                                        File.WriteAllText(swcDir + S + "FlashDevelopNotice.txt",
                                            "This 'playerglobal.swc' was copied here automatically by FlashDevelop from:\r\n" + playerglobal);
                                    }
                                    playerglobal = swcDir + S + "playerglobal.swc";
                                }
                                catch { }
                            }
                            addLibs.Add(playerglobal);
                        }
                    }
                addLocales.Add("playerglobal_rb.swc");

                // framework SWCs
                string as3Fmk = PathHelper.ResolvePath("Library" + S + "AS3" + S + "frameworks");

                // Flex core - ie. (Bitmap|Font|ByteArray|...)Asset / Flex(Sprite|MobieClip|Loader...)
                addLibs.Add("flex.swc");
                addLibs.Add("core.swc");
                
                // Flex framework
                if (cpCheck.IndexOf("Library/AS3/frameworks/Flex", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    bool isFlexJS = cpCheck.IndexOf("Library/AS3/frameworks/FlexJS", StringComparison.OrdinalIgnoreCase) >= 0;

                    if (!isFlexJS)
                    {
                        addLibs.Add("framework.swc");
                        addLibs.Add("mx/mx.swc");
                        addLibs.Add("rpc.swc");
                        addLibs.Add("datavisualization.swc");
                        addLibs.Add("flash-integration.swc");
                        addLocales.Add("framework_rb.swc");
                        addLocales.Add("mx_rb.swc");
                        addLocales.Add("rpc_rb.swc");
                        addLocales.Add("datavisualization_rb.swc");
                        addLocales.Add("flash-integration_rb.swc");
                    }

                    if (hasAIRSupport)
                    {
                        addLibs.Add("air" + S + "airframework.swc");
                        addLocales.Add("airframework_rb.swc");
                    }

                    if (isFlexJS)
                    {
                        string flexJsLibs = frameworks + S + "as" + S + "libs";
                        addLibs.Add(flexJsLibs + S + "FlexJSUI.swc");
                        //addLibs.Add(flexJsLibs + S + "FlexJSJX.swc");
                        MxmlFilter.AddManifest("http://ns.adobe.com/mxml/2009", as3Fmk + S + "FlexJS" + S + "manifest.xml");
                    }
                    else if (cpCheck.IndexOf("Library/AS3/frameworks/Flex4", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        addLibs.Add("spark.swc");
                        addLibs.Add("spark_dmv.swc");
                        addLibs.Add("sparkskins.swc");
                        //addLibs.Add("textLayout.swc");
                        addLibs.Add("osmf.swc");
                        addLocales.Add("spark_rb.swc");
                        //addLocales.Add("textLayout_rb.swc");
                        addLocales.Add("osmf_rb.swc");

                        if (hasAIRSupport)
                        {
                            addLibs.Add("air" + S + "airspark.swc");
                            addLocales.Add("airspark_rb.swc");

                            if (hasMobileSupport)
                            {
                                addLibs.Add("mobile" + S + "mobilecomponents.swc");
                                addLocales.Add("mobilecomponents_rb.swc");
                            }
                        }

                        MxmlFilter.AddManifest("http://ns.adobe.com/mxml/2009", as3Fmk + S + "Flex4" + S + "manifest.xml");
                    }
                    else 
                    {
                        MxmlFilter.AddManifest(MxmlFilter.OLD_MX, as3Fmk + S + "Flex3" + S + "manifest.xml");
                    }
                }
            }

            foreach (string file in addLocales)
            {
                string swcItem = sdkLocales + S + file;
                if (!File.Exists(swcItem)) swcItem = fallbackLocale + S + file;
                AddPath(swcItem);
            }
            foreach (string file in addLibs)
            {
                if (File.Exists(file)) AddPath(file);
                else AddPath(sdkLibs + S + file);
            }

            // intrinsics (deprecated, excepted for FP10 Vector.<T>)
            // add from the highest version number (FP11 > FP10 > FP9)
            string fp = as3settings.AS3ClassPath + S + "FP";
            for (int i = majorVersion; i >= 9; i--)
            {
                AddPath(PathHelper.ResolvePath(fp + i));
            }

            // add external paths
            List<PathModel> initCP = classPath;
            classPath = new List<PathModel>();
            if (contextSetup.Classpath != null)
            {
                foreach (string cpath in contextSetup.Classpath)
                    AddPath(cpath.Trim());
            }

            // add library
            AddPath(PathHelper.LibraryDir + S + "AS3" + S + "classes");
            // add user paths from settings
            if (settings.UserClasspath != null && settings.UserClasspath.Length > 0)
            {
                foreach (string cpath in settings.UserClasspath) AddPath(cpath.Trim());
            }

            // add initial paths
            foreach (PathModel mpath in initCP) AddPath(mpath);

            // parse top-level elements
            InitTopLevelElements();
            if (cFile != null) UpdateTopLevelElements();

            // add current temporary path
            if (temporaryPath != null)
            {
                string tempPath = temporaryPath;
                temporaryPath = null;
                SetTemporaryPath(tempPath);
            }
            FinalizeClasspath();
        }

        /// <summary>
        /// Find any playerglobal.swc
        /// </summary>
        private string MatchPlayerGlobalAny(ref int majorVersion, ref int minorVersion, string sdkLibs)
        {
            char S = Path.DirectorySeparatorChar;
            string libPlayer = sdkLibs + S + "player";
            for (int i = minorVersion; i >= 0; i--)
            {
                string version = majorVersion + "." + i;
                if (Directory.Exists(libPlayer + S + version))
                {
                    minorVersion = i;
                    return libPlayer + S + version + S + "playerglobal.swc";
                }
            }
            string playerglobal = null;
            if (Directory.Exists(libPlayer + S + majorVersion))
                playerglobal = "player" + S + majorVersion + S + "playerglobal.swc";

            if (playerglobal == null && majorVersion > 9)
            {
                int tempMajor = majorVersion - 1;
                int tempMinor = 9;
                playerglobal = MatchPlayerGlobalAny(ref tempMajor, ref tempMinor, sdkLibs);
                if (playerglobal != null)
                {
                    majorVersion = tempMajor;
                    minorVersion = tempMinor;
                    return playerglobal;
                }
            }

            return playerglobal;
        }

        /// <summary>
        /// Find version-matching playerglobal.swc
        /// </summary>
        private string MatchPlayerGlobalExact(int majorVersion, int minorVersion, string sdkLibs)
        {
            string playerglobal = null;
            char S = Path.DirectorySeparatorChar;
            string libPlayer = sdkLibs + S + "player";
            if (Directory.Exists(libPlayer + S + majorVersion + "." + minorVersion))
                playerglobal = libPlayer + S + majorVersion + "." + minorVersion + S + "playerglobal.swc";
            if (minorVersion == 0 && Directory.Exists(libPlayer + S + majorVersion))
                playerglobal = libPlayer + S + majorVersion + S + "playerglobal.swc";
            return playerglobal;
        }
        
        /// <summary>
        /// Build a list of file mask to explore the classpath
        /// </summary>
        public override string[] GetExplorerMask()
        {
            string[] mask = as3settings.AS3FileTypes;
            if (mask == null || mask.Length == 0 || (mask.Length == 1 && mask[0] == ""))
            {
                as3settings.AS3FileTypes = mask = new string[] { "*.as", "*.mxml" };
                return mask;
            }
            else
            {
                List<string> patterns = new List<string>();
                for (int i = 0; i < mask.Length; i++)
                {
                    string m = mask[i];
                    if (string.IsNullOrEmpty(m)) continue;
                    if (m[1] != '.' && m[0] != '.') m = '.' + m;
                    if (m[0] != '*') m = '*' + m;
                    patterns.Add(m);
                }
                return patterns.ToArray();
            }
        }

        /// <summary>
        /// Parse a packaged library file
        /// </summary>
        /// <param name="path">Models owner</param>
        public override void ExploreVirtualPath(PathModel path)
        {
            if (path.WasExplored)
            {
                if (MxmlFilter.HasCatalog(path.Path)) MxmlFilter.AddCatalog(path.Path);

                if (path.FilesCount != 0) // already parsed
                    return;
            }

            try
            {
                if (File.Exists(path.Path) && !path.WasExplored)
                {
                    bool isRefresh = path.FilesCount > 0;
                    //TraceManager.AddAsync("parse " + path.Path);
                    lock (path)
                    {
                        path.WasExplored = true;
                        ContentParser parser = new ContentParser(path.Path);
                        parser.Run();
                        AbcConverter.Convert(parser, path, this);
                    }
                    // reset FCSH
                    if (isRefresh)
                    {
                        EventManager.DispatchEvent(this,
                            new DataEvent(EventType.Command, "ProjectManager.RestartFlexShell", path.Path));
                    }
                }
            }
            catch (Exception ex)
            {
                string message = TextHelper.GetString("Info.ExceptionWhileParsing");
                TraceManager.AddAsync(message + " " + path.Path);
                TraceManager.AddAsync(ex.Message);
                TraceManager.AddAsync(ex.StackTrace);
            }
        }

        /// <summary>
        /// Delete current class's cached file
        /// </summary>
        public override void RemoveClassCompilerCache()
        {
            // not implemented - is there any?
        }

        /// <summary>
        /// Create a new file model without parsing file
        /// </summary>
        /// <param name="fileName">Full path</param>
        /// <returns>File model</returns>
        public override FileModel CreateFileModel(string fileName)
        {
            if (string.IsNullOrEmpty(fileName) || !File.Exists(fileName))
                return new FileModel(fileName);

            fileName = PathHelper.GetLongPathName(fileName);
            if (mxmlEnabled && fileName.EndsWith(".mxml", StringComparison.OrdinalIgnoreCase))
            {
                FileModel nFile = new FileModel(fileName);
                nFile.Context = this;
                nFile.HasFiltering = true;
                return nFile;
            }
            else return base.CreateFileModel(fileName);
        }

        private void GuessPackage(string fileName, FileModel nFile)
        {
            foreach(PathModel aPath in classPath)
                if (fileName.StartsWith(aPath.Path, StringComparison.OrdinalIgnoreCase))
                {
                    string local = fileName.Substring(aPath.Path.Length);
                    char sep = Path.DirectorySeparatorChar;
                    local = local.Substring(0, local.LastIndexOf(sep)).Replace(sep, '.');
                    nFile.Package = local.Length > 0 ? local.Substring(1) : "";
                    nFile.HasPackage = true;
                }
        }

        /// <summary>
        /// Build the file DOM
        /// </summary>
        /// <param name="filename">File path</param>
        protected override void GetCurrentFileModel(string fileName)
        {
            base.GetCurrentFileModel(fileName);
        }

        /// <summary>
        /// Refresh the file model
        /// </summary>
        /// <param name="updateUI">Update outline view</param>
        public override void UpdateCurrentFile(bool updateUI)
        {
            if (mxmlEnabled && cFile != null && cFile != FileModel.Ignore
                && cFile.FileName.EndsWith(".mxml", StringComparison.OrdinalIgnoreCase))
                cFile.HasFiltering = true;
            base.UpdateCurrentFile(updateUI);

            if (cFile.HasFiltering)
            {
                MxmlComplete.mxmlContext = mxmlFilterContext;
                MxmlComplete.context = this;
            }
        }

        /// <summary>
        /// Update the class/member context for the given line number.
        /// Be carefull to restore the context after calling it with a custom line number
        /// </summary>
        /// <param name="line"></param>
        public override void UpdateContext(int line)
        {
            base.UpdateContext(line);
        }

        /// <summary>
        /// Called if a FileModel needs filtering
        /// - define inline AS3 ranges
        /// </summary>
        /// <param name="src"></param>
        /// <returns></returns>
        public override string FilterSource(string fileName, string src)
        {
            mxmlFilterContext = new MxmlFilterContext();
            return MxmlFilter.FilterSource(Path.GetFileNameWithoutExtension(fileName), src, mxmlFilterContext);
        }

        /// <summary>
        /// Called if a FileModel needs filtering
        /// - modify parsed model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public override void FilterSource(FileModel model)
        {
            GuessPackage(model.FileName, model);
            if (mxmlFilterContext != null) MxmlFilter.FilterSource(model, mxmlFilterContext);
        }
        #endregion

        #region syntax checking

        internal void OnFileOperation(NotifyEvent e)
        {
            timerCheck.Stop();
            foreach (ITabbedDocument doc in PluginBase.MainForm.Documents)
                if (doc.FileName == fileWithSquiggles) ClearSquiggles(doc.SciControl);
        }

        public override void TrackTextChange(ScintillaControl sender, int position, int length, int linesAdded)
        {
            base.TrackTextChange(sender, position, length, linesAdded);
            if (as3settings != null && !as3settings.DisableLiveChecking && IsFileValid)
            {
                timerCheck.Stop();
                timerCheck.Start();
            }
        }

        private void timerCheck_Elapsed(object sender, ElapsedEventArgs e)
        {
            BackgroundSyntaxCheck();
        }

        /// <summary>
        /// Checking syntax of current file
        /// </summary>
        private void BackgroundSyntaxCheck()
        {
            if (!IsFileValid) return;

            ScintillaControl sci = CurSciControl;
            if (sci == null) return;
            ClearSquiggles(sci);

            string src = CurSciControl.Text;
            string sdk = PluginBase.CurrentProject != null && PluginBase.CurrentProject.Language == "as3"
                    ? PluginBase.CurrentProject.CurrentSDK
                    : as3settings.GetDefaultSDK().Path;
            FlexShells.Instance.CheckAS3(CurrentFile, sdk, src);
        }

        private void AddSquiggles(ScintillaControl sci, int line, int start, int end)
        {
            if (sci == null) return;
            fileWithSquiggles = CurrentFile;
            int position = sci.PositionFromLine(line) + start;
            sci.AddHighlight(2, (int)IndicatorStyle.Squiggle, 0x000000ff, position, end - start);
        }

        private void ClearSquiggles(ScintillaControl sci)
        {
            if (sci == null) return;
            try
            {
                sci.RemoveHighlights(2);
            }
            finally
            {
                fileWithSquiggles = null;
            }
        }

        private void FlexShell_SyntaxError(string error)
        {
            if (!IsFileValid) return;
            Match m = re_syntaxError.Match(error);
            if (!m.Success) return;

            ITabbedDocument document = PluginBase.MainForm.CurrentDocument;
            if (document == null || !document.IsEditable) return;

            ScintillaControl sci = document.SplitSci1;
            ScintillaControl sci2 = document.SplitSci2;

            if (m.Groups["filename"].Value != CurrentFile) return;
            try
            {
                int line = int.Parse(m.Groups["line"].Value) - 1;
                if (sci.LineCount < line) return;
                int start = MBSafeColumn(sci, line, int.Parse(m.Groups["col"].Value) - 1);
                if (line == sci.LineCount && start == 0 && line > 0) start = -1;
                AddSquiggles(sci, line, start, start + 1);
                AddSquiggles(sci2, line, start, start + 1);
            }
            catch { }
        }

        /// <summary>
        /// Convert multibyte column to byte length
        /// </summary>
        private int MBSafeColumn(ScintillaControl sci, int line, int length)
        {
            String text = sci.GetLine(line) ?? "";
            length = Math.Min(length, text.Length);
            return sci.MBSafeTextLength(text.Substring(0, length));
        }

        #endregion

        #region class resolution
        /// <summary>
        /// Evaluates the visibility of one given type from another.
        /// Caller is responsible of calling ResolveExtends() on 'inClass'
        /// </summary>
        /// <param name="inClass">Completion context</param>
        /// <param name="withClass">Completion target</param>
        /// <returns>Completion visibility</returns>
        public override Visibility TypesAffinity(ClassModel inClass, ClassModel withClass)
        {
            if (inClass == null || withClass == null) return Visibility.Public;
            // same file
            if (inClass.InFile == withClass.InFile)
                return Visibility.Public | Visibility.Internal | Visibility.Protected | Visibility.Private;
            
            // same package
            Visibility acc = Visibility.Public;
            if (inClass.InFile.Package == withClass.InFile.Package) acc |= Visibility.Internal;

            // inheritance affinity
            ClassModel tmp = inClass;
            while (!tmp.IsVoid())
            {
                if (tmp.Type == withClass.Type)
                {
                    acc |= Visibility.Protected;
                    break;
                }
                tmp = tmp.Extends;
            }
            return acc;
        }

        /// <summary>
        /// Return the full project classes list
        /// </summary>
        /// <returns></returns>
        public override MemberList GetAllProjectClasses()
        {
            // from cache
            if (!completionCache.IsDirty && completionCache.AllTypes != null)
                return completionCache.AllTypes;

            MemberList fullList = new MemberList();
            ClassModel aClass;
            MemberModel item;
            // public & internal classes
            string package = CurrentModel?.Package;
            foreach (PathModel aPath in classPath) if (aPath.IsValid && !aPath.Updating)
            {
                aPath.ForeachFile((aFile) =>
                {
                    if (!aFile.HasPackage)
                        return true; // skip

                    aClass = aFile.GetPublicClass();
                    if (!aClass.IsVoid() && aClass.IndexType == null)
                    {
                        if (aClass.Access == Visibility.Public
                            || (aClass.Access == Visibility.Internal && aFile.Package == package))
                        {
                            item = aClass.ToMemberModel();
                            item.Name = item.Type;
                            fullList.Add(item);
                        }
                    }
                    if (aFile.Package.Length > 0 && aFile.Members.Count > 0)
                    {
                        foreach (MemberModel member in aFile.Members)
                        {
                            item = member.Clone() as MemberModel;
                            item.Name = aFile.Package + "." + item.Name;
                            fullList.Add(item);
                        }
                    }
                    else if (aFile.Members.Count > 0)
                    {
                        foreach (MemberModel member in aFile.Members)
                        {
                            item = member.Clone() as MemberModel;
                            fullList.Add(item);
                        }
                    }
                    return true;
                });
            }
            // void
            fullList.Add(new MemberModel(features.voidKey, features.voidKey, FlagType.Class | FlagType.Intrinsic, 0));
            // private classes
            fullList.Add(GetPrivateClasses());

            // in cache
            fullList.Sort();
            completionCache.AllTypes = fullList;
            return fullList;
        }

        public override bool OnCompletionInsert(ScintillaControl sci, int position, string text, char trigger)
        {
            if (text == "Vector")
            {
                string insert = null;
                string line = sci.GetLine(sci.LineFromPosition(position));
                Match m = Regex.Match(line, @"\svar\s+(?<varname>.+)\s*:\s*Vector\.<(?<indextype>.+)(?=(>\s*=))");
                if (m.Success)
                {
                    insert = String.Format(".<{0}>", m.Groups["indextype"].Value);
                }
                else
                {
                    m = Regex.Match(line, @"\s*=\s*new");
                    if (m.Success)
                    {
                        ASResult result = ASComplete.GetExpressionType(sci, sci.PositionFromLine(sci.LineFromPosition(position)) + m.Index);
                        if (result != null && !result.IsNull() && result.Member != null && result.Member.Type != null)
                        {
                            m = Regex.Match(result.Member.Type, @"(?<=<).+(?=>)");
                            if (m.Success)
                            {
                                insert = String.Format(".<{0}>", m.Value);
                            }
                        }
                    }
                    if (insert == null)
                    {
                        if (trigger == '.' || trigger == '(') return true;
                        insert = ".<>";
                        sci.InsertText(position + text.Length, insert);
                        sci.CurrentPos = position + text.Length + 2;
                        sci.SetSel(sci.CurrentPos, sci.CurrentPos);
                        ASComplete.HandleAllClassesCompletion(sci, "", false, true);
                        return true;
                    }
                }
                if (trigger == '.')
                {
                    sci.InsertText(position + text.Length, insert.Substring(1));
                    sci.CurrentPos = position + text.Length;
                }
                else 
                {
                    sci.InsertText(position + text.Length, insert);
                    sci.CurrentPos = position + text.Length + insert.Length;
                }
                sci.SetSel(sci.CurrentPos, sci.CurrentPos);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Check if a type is already in the file's imports
        /// Throws an Exception if the type name is ambiguous 
        /// (ie. same name as an existing import located in another package)
        /// </summary>
        /// <param name="member">Element to search in imports</param>
        /// <param name="atLine">Position in the file</param>
        public override bool IsImported(MemberModel member, int atLine)
        {
            FileModel cFile = Context.CurrentModel;
            // same package is auto-imported
            string package = member.Type.Length > member.Name.Length 
                ? member.Type.Substring(0, member.Type.Length - member.Name.Length - 1)
                : "";
            if (package == cFile.Package) return true;
            return base.IsImported(member, atLine);
        }

        /// <summary>
        /// Retrieves a class model from its name
        /// </summary>
        /// <param name="cname">Class (short or full) name</param>
        /// <param name="inClass">Current file</param>
        /// <returns>A parsed class or an empty ClassModel if the class is not found</returns>
        public override ClassModel ResolveType(string cname, FileModel inFile)
        {
            // handle generic types
            if (cname != null && cname.IndexOf('<') >= 0)
            {
                if (cname.StartsWith('<'))
                {
                    //transform <T>[] to Vector.<T>
                    cname = Regex.Replace(cname, @">\[.*", ">");
                    cname = "Vector." + cname;
                }
                Match genType = re_genericType.Match(cname);
                if (genType.Success)
                    return ResolveGenericType(genType.Groups["gen"].Value, genType.Groups["type"].Value, inFile);
                else return ClassModel.VoidClass;
            }
            return base.ResolveType(cname, inFile);
        }

        /// <summary>
        /// Retrieve/build typed copies of generic types
        /// </summary>
        private ClassModel ResolveGenericType(string baseType, string indexType, FileModel inFile)
        {
            ClassModel originalClass = base.ResolveType(baseType, inFile);
            if (originalClass.IsVoid()) return originalClass;

            ClassModel indexClass = ResolveType(indexType, inFile);
            if (indexClass.IsVoid()) return originalClass;
            indexType = indexClass.QualifiedName;

            FileModel aFile = originalClass.InFile;
            // is the type already cloned?
            foreach (ClassModel otherClass in aFile.Classes)
                if (otherClass.IndexType == indexType) return otherClass;

            // clone the type
            ClassModel aClass = originalClass.Clone() as ClassModel;

            aClass.Name = baseType + ".<" + indexType + ">";
            aClass.IndexType = indexType;

            string typed = "<" + indexType + ">";
            foreach (MemberModel member in aClass.Members)
            {
                if (member.Name == baseType) member.Name = baseType.Replace("<T>", typed);
                if (member.Type != null && member.Type.IndexOf('T') >= 0)
                {
                    if (member.Type == "T") member.Type = indexType;
                    else member.Type = member.Type.Replace("<T>", typed);
                }
                if (member.Parameters != null)
                {
                    foreach (MemberModel param in member.Parameters)
                    {
                        if (param.Type != null && param.Type.IndexOf('T') >= 0)
                        {
                            if (param.Type == "T") param.Type = indexType;
                            else param.Type = param.Type.Replace("<T>", typed);
                        }
                    }
                }
            }

            aFile.Classes.Add(aClass);
            return aClass;
        }

        protected MemberList GetPrivateClasses()
        {
            MemberList list = new MemberList();
            // private classes
            foreach(ClassModel model in cFile.Classes)
                if (model.Access == Visibility.Private)
                {
                    MemberModel item = model.ToMemberModel();
                    item.Type = item.Name;
                    item.Access = Visibility.Private;
                    list.Add(item);
                }
            // 'Class' members
            if (cClass != null)
                foreach (MemberModel member in cClass.Members)
                    if (member.Type == "Class") list.Add(member);
            return list;
        }

        /// <summary>
        /// Prepare AS3 intrinsic known vars/methods/classes
        /// </summary>
        protected override void InitTopLevelElements()
        {
            string filename = "toplevel.as";
            topLevel = new FileModel(filename);

            if (topLevel.Members.Search("this", 0, 0) == null)
                topLevel.Members.Add(new MemberModel("this", "", FlagType.Variable | FlagType.Intrinsic, Visibility.Public));
            if (topLevel.Members.Search("super", 0, 0) == null)
                topLevel.Members.Add(new MemberModel("super", "", FlagType.Variable | FlagType.Intrinsic, Visibility.Public));
            if (topLevel.Members.Search(features.voidKey, 0, 0) == null)
                topLevel.Members.Add(new MemberModel(features.voidKey, "", FlagType.Intrinsic, Visibility.Public));
            topLevel.Members.Sort();
        }

        #endregion

        #region Command line compiler

        /// <summary>
        /// Retrieve the context's default compiler path
        /// </summary>
        public override string GetCompilerPath()
        {
            return as3settings.GetDefaultSDK().Path ?? "Tools\\flexsdk";
        }

        /// <summary>
        /// Check current file's syntax
        /// </summary>
        public override void CheckSyntax()
        {
            if (IsFileValid && cFile.InlinedIn == null)
            {
                PluginBase.MainForm.CallCommand("Save", null);

                string sdk = PluginBase.CurrentProject != null
                    ? PluginBase.CurrentProject.CurrentSDK
                    : PathHelper.ResolvePath(as3settings.GetDefaultSDK().Path);
                FlexShells.Instance.CheckAS3(cFile.FileName, sdk);
            }
        }

        /// <summary>
        /// Run MXMLC compiler in the current class's base folder with current classpath
        /// </summary>
        /// <param name="append">Additional comiler switches</param>
        public override void RunCMD(string append)
        {
            if (!IsCompilationTarget())
            {
                MessageBar.ShowWarning(TextHelper.GetString("Info.InvalidClass"));
                return;
            }

            string command = (append ?? "") + " -- " + CurrentFile;
            FlexShells.Instance.RunMxmlc(command, as3settings.GetDefaultSDK().Path);
        }

        private bool IsCompilationTarget()
        {
            return (!MainForm.CurrentDocument.IsUntitled && CurrentModel.Version >= 3);
        }

        /// <summary>
        /// Calls RunCMD with additional parameters taken from the classes @mxmlc doc tag
        /// </summary>
        public override bool BuildCMD(bool failSilently)
        {
            if (!IsCompilationTarget())
            {
                MessageBar.ShowWarning(TextHelper.GetString("Info.InvalidClass"));
                return false;
            }
            
            MainForm.CallCommand("SaveAllModified", null);

            string sdk = PluginBase.CurrentProject != null 
                    ? PluginBase.CurrentProject.CurrentSDK
                    : as3settings.GetDefaultSDK().Path;
            FlexShells.Instance.QuickBuild(CurrentModel, sdk, failSilently, as3settings.PlayAfterBuild);
            return true;
        }
        #endregion
    }
}
