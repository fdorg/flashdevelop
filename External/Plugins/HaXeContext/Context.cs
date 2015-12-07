using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using PluginCore.Managers;
using ASCompletion.Context;
using ASCompletion.Model;
using PluginCore.Localization;
using PluginCore.Controls;
using PluginCore.Helpers;
using PluginCore;
using ASCompletion.Completion;
using System.Linq;
using System.Windows.Forms;
using ProjectManager.Projects.Haxe;
using ProjectManager.Projects;
using AS3Context;
using PluginCore.Utilities;
using ScintillaNet;

namespace HaXeContext
{
    public class Context : AS2Context.Context
    {
        #region initialization
        new static readonly protected Regex re_CMD_BuildCommand =
            new Regex("@haxe[\\s]+(?<params>.*)", RegexOptions.Compiled | RegexOptions.Multiline);

        static readonly protected Regex re_genericType =
            new Regex("(?<gen>[^<]+)<(?<type>.+)>", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        static readonly protected Regex re_Template =
            new Regex("<(?<name>[a-z]+)>", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        static public string FLASH_OLD = "flash";
        static public string FLASH_NEW = "flash9";
        static private string currentEnv;
        static private string currentSDK;
        
        private HaXeSettings hxsettings;
        private Dictionary<string, List<string>> haxelibsCache;
        private string HaxeTarget;
        private bool hasAIRSupport;
        private bool hasMobileSupport;
        private bool resolvingDot;
        private bool resolvingFunction;
        HaxeCompletionCache hxCompletionCache;
        ClassModel stubFunctionClass;

        public Context(HaXeSettings initSettings)
        {
            hxsettings = initSettings;
            hxsettings.Init();

            /* AS-LIKE OPTIONS */

            hasLevels = false;
            docType = "Void"; // "flash.display.MovieClip";

            stubFunctionClass = new ClassModel();
            stubFunctionClass.Name = stubFunctionClass.Type = "Function";
            stubFunctionClass.Flags = FlagType.Class;
            stubFunctionClass.Access = Visibility.Public;
            var funFile = new FileModel();
            funFile.Classes.Add(stubFunctionClass);
            stubFunctionClass.InFile = funFile;

            /* DESCRIBE LANGUAGE FEATURES */

            // language constructs
            features.hasPackages = true;
            features.hasFriendlyParentPackages = true;
            features.hasModules = true;
            features.hasImports = true;
            features.hasImportsWildcard = false;
            features.hasClasses = true;
            features.hasExtends = true;
            features.hasImplements = true;
            features.hasInterfaces = true;
            features.hasEnums = true;
            features.hasTypeDefs = true;
            features.hasGenerics = true;
            features.hasEcmaTyping = true;
            features.hasVars = true;
            features.hasConsts = false;
            features.hasMethods = true;
            features.hasStatics = true;
            features.hasTryCatch = true;
            features.hasInference = true;
            features.hasStringInterpolation = true;
            features.checkFileName = false;

            // haxe directives
            features.hasDirectives = true;
            features.Directives = new List<string>();
            features.Directives.Add("true");

            // allowed declarations access modifiers
            Visibility all = Visibility.Public | Visibility.Private;
            features.classModifiers = all;
            features.varModifiers = all;
            features.methodModifiers = all;

            // default declarations access modifiers
            features.classModifierDefault = Visibility.Public;
            features.enumModifierDefault = Visibility.Public;
            features.typedefModifierDefault = Visibility.Public;
            features.varModifierDefault = Visibility.Private;
            features.methodModifierDefault = Visibility.Private;

            // keywords
            features.dot = ".";
            features.voidKey = "Void";
            features.objectKey = "Dynamic";
            features.booleanKey = "Bool";
            features.numberKey = "Float";
            features.stringKey = "String";
            features.arrayKey = "Array<T>";
            features.importKey = "import";
            features.importKeyAlt = "using";
            features.typesPreKeys = new string[] { "import", "new", "extends", "implements", "using" };
            features.codeKeywords = new string[] { 
                "var", "function", "new", "cast", "return", "break", 
                "continue", "if", "else", "for", "while", "do", "switch", "case", "default", "$type",
                "null", "untyped", "true", "false", "try", "catch", "throw", "trace", "macro"
            };
            features.declKeywords = new string[] { "var", "function" };
            features.accessKeywords = new string[] { "extern", "inline", "dynamic", "macro", "override", "public", "private", "static" };
            features.typesKeywords = new string[] { "import", "using", "class", "interface", "typedef", "enum", "abstract" };
            features.varKey = "var";
            features.overrideKey = "override";
            features.functionKey = "function";
            features.staticKey = "static";
            features.publicKey = "public";
            features.privateKey = "private";
            features.intrinsicKey = "extern";
            features.inlineKey = "inline";
            features.hiddenPackagePrefix = '_';
            features.stringInterpolationQuotes = "'";

            /* INITIALIZATION */

            settings = initSettings;

            currentSDK = PathHelper.ResolvePath(hxsettings.GetDefaultSDK().Path) ?? "";
            initSettings.CompletionModeChanged += OnCompletionModeChange;
            //OnCompletionModeChange(); // defered to first use

            haxelibsCache = new Dictionary<string, List<string>>();
            //BuildClassPath(); // defered to first use
        }
        #endregion

        #region classpath management

        private List<string> LookupLibrary(string lib)
        {
            if (haxelibsCache.ContainsKey(lib))
                return haxelibsCache[lib];

            try
            {
                string haxelib = "haxelib";

                string hxPath = currentSDK;
                if (hxPath != null && Path.IsPathRooted(hxPath))
                {
                    if (hxPath != currentEnv) SetHaxeEnvironment(hxPath);
                    haxelib = Path.Combine(hxPath, haxelib);
                }
                
                ProcessStartInfo pi = new ProcessStartInfo();
                pi.FileName = haxelib;
                pi.Arguments = "path " + lib;
                pi.RedirectStandardOutput = true;
                pi.UseShellExecute = false;
                pi.CreateNoWindow = true;
                pi.WindowStyle = ProcessWindowStyle.Hidden;
                Process p = Process.Start(pi);
                p.WaitForExit();

                List<string> paths = new List<string>();
                string line = "";
                do { 
                    line = p.StandardOutput.ReadLine();
                    if (string.IsNullOrEmpty(line)) continue;
                    if (line.IndexOf("not installed") > 0)
                    {
                        TraceManager.Add(line, 3);
                    }
                    else if (!line.StartsWith("-"))
                    {
                        try
                        {
                            if (Directory.Exists(line))
                                paths.Add(NormalizePath(line).TrimEnd(Path.DirectorySeparatorChar));
                        }
                        catch (Exception) { }
                    }
                }
                while (!p.StandardOutput.EndOfStream);
                p.Close();

                if (paths.Count > 0)
                {
                    haxelibsCache.Add(lib, paths);
                    return paths;
                }
                else return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// User refreshes project tree
        /// </summary>
        public override void UserRefreshRequest()
        {
            haxelibsCache.Clear();
            HaxeProject proj = PluginBase.CurrentProject as HaxeProject;
            if (proj != null) proj.UpdateVars(false);
        }

        /// <summary>
        /// Properly switch between different Haxe SDKs
        /// </summary>
        public void SetHaxeEnvironment(string sdkPath)
        {
            sdkPath = sdkPath.TrimEnd(new char[] { '/', '\\' });
            if (currentEnv == sdkPath) return;
            Environment.SetEnvironmentVariable("HAXEPATH", sdkPath);

            var neko = Path.GetFullPath(Path.Combine(sdkPath, "..\\neko"));
            if (Directory.Exists(neko))
                Environment.SetEnvironmentVariable("NEKO_INSTPATH", neko);
            else neko = null;

            var path = ";" + Environment.GetEnvironmentVariable("PATH");
            if (currentEnv != null) path = path.Replace(currentEnv + ";", ";");
            if (neko != null) path = Regex.Replace(path, ";[^;]+neko[/\\\\]*;", ";", RegexOptions.IgnoreCase);
            path = Regex.Replace(path, ";[^;]+haxe[/\\\\]*;", ";", RegexOptions.IgnoreCase);
            path = path.TrimStart(new char[] { ';' });
            path = sdkPath + ";" + path;
            if (neko != null) path = neko.TrimEnd(new char[] { '/', '\\' }) + ";" + path;
            Environment.SetEnvironmentVariable("PATH", path);
            currentEnv = sdkPath;

            LoadMetadata();
        }

        public void LoadMetadata()
        {
            features.metadata = new Dictionary<string, string>();

            Process process = createHaxeProcess("--help-metas");
            if (process == null) return;
            process.Start();

            String metaList = process.StandardOutput.ReadToEnd();
            process.Close();

            Regex regex = new Regex("@:([a-zA-Z]*)(?: : )(.*?)(?=( @:[a-zA-Z]* :|$))");
            metaList = Regex.Replace(metaList, "\\s+", " ");

            MatchCollection matches = regex.Matches(metaList);

            foreach (Match m in matches)
            {
                features.metadata.Add(m.Groups[1].ToString(), m.Groups[2].ToString());
            }
        }

        /// <summary>
        /// Classpathes & classes cache initialisation
        /// </summary>
        public override void BuildClassPath()
        {
            ReleaseClasspath();
            started = true;
            if (hxsettings == null) throw new Exception("BuildClassPath() must be overridden");
            if (contextSetup == null)
            {
                contextSetup = new ContextSetupInfos();
                contextSetup.Classpath = new string[] { Environment.CurrentDirectory };
                contextSetup.Lang = settings.LanguageId;
                contextSetup.Platform = "Flash Player";
                contextSetup.Version = "10.0";
            }

            // external version definition
            platform = contextSetup.Platform;
            majorVersion = hxsettings.DefaultFlashVersion;
            minorVersion = 0;
            ParseVersion(contextSetup.Version, ref majorVersion, ref minorVersion);

            // NOTE: version > 10 for non-Flash platforms
            string lang = GetHaxeTarget(platform);
            hasAIRSupport = hasMobileSupport = false;
            features.Directives = new List<string>();

            if (lang == null)
            {
                if (contextSetup.Platform == "hxml")
                {
                    lang = contextSetup.TargetBuild ?? "";
                }
                else // assume game-related toolchain
                {
                    lang = "cpp";
                    if (contextSetup.TargetBuild == null || contextSetup.TargetBuild.StartsWith("flash"))
                        lang = "";
                    else if (contextSetup.TargetBuild.StartsWith("html5"))
                        lang = "js";
                    else if (contextSetup.TargetBuild.IndexOf("neko") >= 0)
                        lang = "neko";
                }
            }
            else if (lang == "swf")
            {
                lang = "flash";
                hasAIRSupport = platform.StartsWith("AIR");
                hasMobileSupport = platform == "AIR Mobile";
            }
            features.Directives.Add(lang);
            HaxeTarget = lang;

            //
            // Class pathes
            //
            classPath = new List<PathModel>();
            // haXe std
            string hxPath = PluginBase.CurrentProject is HaxeProject
                    ? PluginBase.CurrentProject.CurrentSDK
                    : PathHelper.ResolvePath(hxsettings.GetDefaultSDK().Path);
            if (hxPath != null)
            {
                if (currentSDK != hxPath)
                {
                    currentSDK = hxPath;
                    haxelibsCache = new Dictionary<string, List<string>>();
                    OnCompletionModeChange();
                }

                string haxeCP = Path.Combine(hxPath, "std");
                if (Directory.Exists(haxeCP))
                {
                    if (Directory.Exists(Path.Combine(haxeCP, "flash9")))
                    {
                        FLASH_NEW = "flash9";
                        FLASH_OLD = "flash";
                    }
                    else
                    {
                        FLASH_NEW = "flash";
                        FLASH_OLD = "flash8";
                    }
                    if (HaxeTarget == "flash")
                        lang = (majorVersion >= 6 && majorVersion < 9) ? FLASH_OLD : FLASH_NEW;

                    PathModel std = PathModel.GetModel(haxeCP, this);
                    if (!std.WasExplored && !Settings.LazyClasspathExploration)
                    {
                        string[] keep = new string[] { "sys", "haxe", "libs" };
                        List<String> hide = new List<string>();
                        foreach (string dir in Directory.GetDirectories(haxeCP))
                            if (Array.IndexOf<string>(keep, Path.GetFileName(dir)) < 0)
                                hide.Add(Path.GetFileName(dir));
                        ManualExploration(std, hide);
                    }
                    AddPath(std);

                    if (!string.IsNullOrEmpty(lang))
                    {
                        PathModel specific = PathModel.GetModel(Path.Combine(haxeCP, lang), this);
                        if (!specific.WasExplored && !Settings.LazyClasspathExploration)
                        {
                            ManualExploration(specific, null);
                        }
                        AddPath(specific);
                    }
                }

                // Haxe3/extralibs is a user global source location
                string extraCP = Path.Combine(hxPath, "extralibs");
                if (Directory.Exists(extraCP)) AddPath(extraCP);
            }
            HaxeProject proj = PluginBase.CurrentProject as HaxeProject;

            // swf-libs
            if (HaxeTarget == "flash" && majorVersion >= 9 && proj != null)
            {
                foreach(LibraryAsset asset in proj.LibraryAssets)
                    if (asset.IsSwc)
                    {
                        string path = proj.GetAbsolutePath(asset.Path);
                        if (File.Exists(path)) AddPath(path);
                    }
                foreach( string p in proj.CompilerOptions.Additional )
                    if( p.IndexOf("-swf-lib ") == 0 ) {
                        string path = proj.GetAbsolutePath(p.Substring(9));
                        if (File.Exists(path)) AddPath(path);
                    }
            }

            // add haxe libraries
            if (proj != null)
            {
                foreach (string param in proj.BuildHXML(new string[0], "", false))
                    if (!string.IsNullOrEmpty(param) && param.IndexOf("-lib ") == 0)
                    {
                        List<string> libPaths = LookupLibrary(param.Substring(5));
                        if (libPaths != null)
                        {
                            foreach (string path in libPaths)
                            {
                                PathModel libPath = AddPath(path);
                                if (libPath != null) AppendPath(contextSetup, libPath.Path);
                            }
                        }
                    }
            }

            // add external pathes
            List<PathModel> initCP = classPath;
            classPath = new List<PathModel>();
            if (contextSetup.Classpath != null)
            {
                foreach (string cpath in contextSetup.Classpath)
                    AddPath(cpath.Trim());
            }

            // add user pathes from settings
            if (settings.UserClasspath != null && settings.UserClasspath.Length > 0)
            {
                foreach (string cpath in settings.UserClasspath) AddPath(cpath.Trim());
            }
            // add initial pathes
            foreach (PathModel mpath in initCP) AddPath(mpath);

            // parse top-level elements
            InitTopLevelElements();
            if (cFile != null) UpdateTopLevelElements();

            // add current temporaty path
            if (temporaryPath != null)
            {
                string tempPath = temporaryPath;
                temporaryPath = null;
                SetTemporaryPath(tempPath);
            }
            FinalizeClasspath();

            resolvingDot = false;
            resolvingFunction = false;
            if (completionModeHandler == null) 
                OnCompletionModeChange();
        }

        private string GetHaxeTarget(string platformName)
        {
            if (!PlatformData.SupportedLanguages.ContainsKey("haxe")) return null;
            var haxeLang = PlatformData.SupportedLanguages["haxe"];
            if (haxeLang == null) return null;
            foreach (var platform in haxeLang.Platforms.Values)
                if (platform.Name == platformName) return platform.HaxeTarget;
            return null;
        }

        private void AppendPath(ContextSetupInfos contextSetup, string path)
        {
            foreach(string cp in contextSetup.Classpath) 
                if (path.Equals(cp, StringComparison.OrdinalIgnoreCase))
                    return;
            if (contextSetup.AdditionalPaths == null) contextSetup.AdditionalPaths = new List<string>();
            foreach (string cp in contextSetup.AdditionalPaths)
                if (path.Equals(cp, StringComparison.OrdinalIgnoreCase))
                    return;
            contextSetup.AdditionalPaths.Add(path);
        }

        override protected bool ExplorePath(PathModel path)
        {
            if (!path.WasExplored && !path.IsVirtual && !path.IsTemporaryPath)
            {
                // enable stricter validation for haxelibs which tend to include a lot of unrelated code (samples, templates)
                string haxelib = Path.Combine(path.Path, "haxelib.json");
                if (File.Exists(haxelib))
                {
                    path.ValidatePackage = true;
                }
                else 
                {
                    haxelib = Path.Combine(path.Path, "haxelib.xml");
                    if (File.Exists(haxelib))
                    {
                        path.ValidatePackage = true;
                        // let's hide confusing packages of NME library
                        string src = File.ReadAllText(haxelib);
                        if (src.IndexOf("<project name=\"nme\"") >= 0)
                        {
                            ManualExploration(path, new string[] { 
                                "js", "jeash", "neash", "native", "browser", "flash", "neko", "tools", "samples", "project" });
                        }
                    }
                }
            }
            return base.ExplorePath(path);
        }

        /// <summary>
        /// Parse a packaged library file
        /// </summary>
        /// <param name="path">Models owner</param>
        public override void ExploreVirtualPath(PathModel path)
        {
            try
            {
                if (File.Exists(path.Path))
                {
                    SwfOp.ContentParser parser = new SwfOp.ContentParser(path.Path);
                    parser.Run();
                    AbcConverter.Convert(parser, path, this);
                }
            }
            catch (Exception ex)
            {
                string message = TextHelper.GetString("Info.ExceptionWhileParsing");
                TraceManager.AddAsync(message + " " + path.Path);
                TraceManager.AddAsync(ex.Message);
            }
        }

        /// <summary>
        /// Create a new file model using the default file parser
        /// </summary>
        /// <param name="filename">Full path</param>
        /// <returns>File model</returns>
        public override FileModel GetFileModel(string fileName)
        {
            return base.GetFileModel(fileName);
        }

        /// <summary>
        /// Refresh the file model
        /// </summary>
        /// <param name="updateUI">Update outline view</param>
        public override void UpdateCurrentFile(bool updateUI)
        {
            base.UpdateCurrentFile(updateUI);
        }

        /// <summary>
        /// Confirms that the FileModel should be added to the PathModel
        /// - typically classes whose context do not patch the classpath should be ignored
        /// </summary>
        /// <param name="aFile"></param>
        /// <param name="pathModel"></param>
        /// <returns></returns>
        public override bool IsModelValid(FileModel aFile, PathModel pathModel)
        {
            if (!pathModel.ValidatePackage) return true;
            string path = Path.GetDirectoryName(aFile.FileName);
            if (path.StartsWith(pathModel.Path, StringComparison.OrdinalIgnoreCase))
            {
                string package = path.Length <= pathModel.Path.Length ? "" : path.Substring(pathModel.Path.Length + 1).Replace('/', '.').Replace('\\', '.');
                return (aFile.Package == package);
            }
            else return false;
        }

        /// <summary>
        /// Delete current class's cached file
        /// </summary>
        public override void RemoveClassCompilerCache()
        {
            // not implemented - is there any?
        }
        #endregion

        #region SDK
        private InstalledSDK GetCurrentSDK()
        {
            return hxsettings.InstalledSDKs.FirstOrDefault(sdk => sdk.Path == currentSDK);
        }

        private SemVer GetCurrentSDKVersion()
        {
            InstalledSDK currentSDK = GetCurrentSDK();
            if (currentSDK != null)
                return new SemVer(currentSDK.Version);
            return SemVer.Zero;
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
            // same file
            if (withClass != null && inClass.InFile == withClass.InFile)
                return Visibility.Public | Visibility.Private;
            // inheritance affinity
            ClassModel tmp = inClass;
            while (!tmp.IsVoid())
            {
                if (tmp == withClass)
                    return Visibility.Public | Visibility.Private;
                tmp = tmp.Extends;
            }
            // same package
            if (withClass != null && inClass.InFile.Package == withClass.InFile.Package)
                return Visibility.Public;
            // public only
            else
                return Visibility.Public;
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
            MemberModel item;
            // public & internal classes
            string package = CurrentModel.Package;
            foreach (PathModel aPath in classPath) if (aPath.IsValid && !aPath.Updating)
            {
                aPath.ForeachFile((aFile) =>
                {
                    string module = aFile.Module;
                    bool needModule = true;

                    if (aFile.Classes.Count > 0 && !aFile.Classes[0].IsVoid())
                        foreach (ClassModel aClass in aFile.Classes)
                        {
                            string tpackage = aClass.InFile.Package;
                            if (aClass.IndexType == null
                                && (aClass.Access == Visibility.Public
                                    || (aClass.Access == Visibility.Internal && tpackage == package)))
                            {
                                if (aClass.Name == module) needModule = false;
                                item = aClass.ToMemberModel();
                                //if (tpackage != package) 
                                if (item.Type != null) item.Name = item.Type;
                                fullList.Add(item);
                            }
                        }
                    // HX files correspond to a "module" which should appear in code completion
                    // (you don't import classes defined in modules but the module itself)
                    if (needModule)
                    {
                        string qmodule = aFile.FullPackage;
                        if (qmodule != null)
                        {
                            item = new MemberModel(qmodule, qmodule, FlagType.Class | FlagType.Module, Visibility.Public);
                            fullList.Add(item);
                        }
                    }
                    return true;
                });
            }
            // display imported classes and classes declared in imported modules
            MemberList imports = ResolveImports(cFile);
            FlagType mask = FlagType.Class | FlagType.Enum;
            foreach (MemberModel import in imports)
            {
                if ((import.Flags & mask) > 0)
                {
                    /*if (import is ClassModel)
                    {
                        MemberModel cmodel = (import as ClassModel).ToMemberModel();
                        cmodel.Name = cmodel.Type;
                        fullList.Add(cmodel);
                    }
                    else*/ fullList.Add(import);
                }
            }

            foreach(ClassModel aClass in cFile.Classes)
                fullList.Add(aClass.ToMemberModel());

            // in cache
            fullList.Sort();
            completionCache.AllTypes = fullList;
            return fullList;
        }

        public override bool OnCompletionInsert(ScintillaNet.ScintillaControl sci, int position, string text, char trigger)
        {
            // Commented out: the consensus is to not detect and add the type index type.

            /*if (text.Length > 0 && Char.IsUpper(text[0]))
            {
                string insert = null;
                string line = sci.GetLine(sci.LineFromPosition(position));
                Match m = Regex.Match(line, @"\svar\s+(?<varname>.+)\s*:\s*(?<type>[a-z0-9_.]+)\<(?<indextype>.+)(?=(>\s*=))", RegexOptions.IgnoreCase);
                if (m.Success)
                {
                    insert = String.Format("<{0}>", m.Groups["indextype"].Value);
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
                                insert = String.Format("<{0}>", m.Value);
                            }
                        }
                    }
                }
                if (insert == null) return false;
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
            }*/

            return false;
        }

        /// <summary>
        /// Return imported classes list (not null)
        /// </summary>
        /// <param name="package">Package to explore</param>
        /// <param name="inFile">Current file</param>
        public override MemberList ResolveImports(FileModel inFile)
        {
            if (inFile == cFile && completionCache.Imports != null) 
                return completionCache.Imports;

            MemberList imports = new MemberList();
            if (inFile == null) return imports;
            foreach (MemberModel item in inFile.Imports)
            {
                if (item.Name != "*") ResolveImport(item, imports);
                else
                {
                    string cname = item.Type.Substring(0, item.Type.Length - 2);
                    // classes matching wildcard
                    FileModel matches = ResolvePackage(cname, false);
                    if (matches != null)
                    {
                        foreach (MemberModel import in matches.Imports)
                            imports.Add(import);
                        foreach (MemberModel member in matches.Members)
                            imports.Add(member);
                    }
                    else
                    {
                        var model = ResolveType(cname, null);
                        if (!model.IsVoid())
                        {
                            foreach (MemberModel member in model.Members)
                            {
                                if ((member.Flags & FlagType.Static) > 0)
                                {
                                    member.InFile = model.InFile;
                                    imports.Add(member);
                                }
                            }
                        }
                    }
                }
            }

            if (inFile == cFile)
            {
                if (cClass != null && cClass != ClassModel.VoidClass)
                    ResolveImport(cClass, imports);
            }
            else
            {
                foreach (ClassModel aClass in inFile.Classes)
                    if (aClass.Access != Visibility.Private) ResolveImport(aClass, imports);
            }

            // haxe3: type resolution from bottom to top
            imports.Items.Reverse();
            if (inFile == cFile) completionCache.Imports = imports;
            return imports;
        }

        private void ResolveImport(MemberModel item, MemberList imports)
        {
            if (settings.LazyClasspathExploration)
            {
                imports.Add(item);
                return;
            }
            // HX files are "modules": when imported all the classes contained are available
            string fileName = item.Type.Replace(".", dirSeparator) + ".hx";

            if (fileName.StartsWith("flash" + dirSeparator))
            {
                if (HaxeTarget != "flash" || majorVersion > 8) // flash9 remap
                    fileName = FLASH_NEW + fileName.Substring(5);
                else
                    fileName = FLASH_OLD + fileName.Substring(5);
            }

            bool matched = false;
            foreach (PathModel aPath in classPath)
                if (aPath.IsValid && !aPath.Updating)
                {
                    string path;
                    path = aPath.Path + dirSeparator + fileName;

                    FileModel file = null;
                    // cached file
                    if (aPath.HasFile(path))
                    {
                        file = aPath.GetFile(path);
                        if (file.Context != this)
                        {
                            // not associated with this context -> refresh
                            file.OutOfDate = true;
                            file.Context = this;
                        }
                    }
                    if (file != null)
                    {
                        // add all public classes of Haxe modules
                        foreach (ClassModel c in file.Classes)
                            if (c.IndexType == null && c.Access == Visibility.Public) 
                                imports.Add(c);
                        matched = true;
                    }
                }

            if (!matched) // add anyway
                imports.Add(new MemberModel(item.Name, item.Type, FlagType.Class, Visibility.Public));
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
            int p = member.Name.IndexOf('#');
            if (p > 0)
            {
                member = member.Clone() as MemberModel;
                member.Name = member.Name.Substring(0, p);
            }

            FileModel cFile = ASContext.Context.CurrentModel;
            string fullName = member.Type;
            string name = member.Name;
            foreach (MemberModel import in cFile.Imports)
            {
                if (import.Name == name)
                {
                    //if (import.Type != fullName) throw new Exception(TextHelper.GetString("Info.AmbiguousType"));
                    return true;
                }
                else if (import.Name == "*" && import.Type.Replace("*", name) == fullName)
                    return true;
                else if (fullName.StartsWith(import.Type + "."))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Retrieves a class model from its name
        /// </summary>
        /// <param name="cname">Class (short or full) name</param>
        /// <param name="inClass">Current file</param>
        /// <returns>A parsed class or an empty ClassModel if the class is not found</returns>
        public override ClassModel ResolveType(string cname, FileModel inFile)
        {
            // unknown type
            if (string.IsNullOrEmpty(cname) || cname == features.voidKey || classPath == null)
                return ClassModel.VoidClass;

            if (cname == "Function") return stubFunctionClass;

            // handle generic types
            if (cname.IndexOf('<') > 0)
            {
                Match genType = re_genericType.Match(cname);
                if (genType.Success)
                    return ResolveGenericType(genType.Groups["gen"].Value, genType.Groups["type"].Value, inFile);
                else return ClassModel.VoidClass;
            }

            // typed array
            if (cname.IndexOf('@') > 0)
                return ResolveTypeIndex(cname, inFile);

            string package = "";
            string inPackage = (features.hasPackages && inFile != null) ? inFile.Package : "";

            int p = cname.LastIndexOf('.');
            if (p > 0)
            {
                package = cname.Substring(0, p);
                cname = cname.Substring(p + 1);
            }
            else 
            {
                // search in file
                if (inFile != null)
                    foreach (ClassModel aClass in inFile.Classes)
                        if (aClass.Name == cname)
                            return aClass;

                // search in imported classes
                MemberList imports = ResolveImports(inFile);
                foreach (MemberModel import in imports)
                {
                    if (import.Name == cname)
                    {
                        if (import.Type.Length > import.Name.Length)
                        {
                            var type = import.Type;
                            int temp = type.IndexOf('<');
                            if (temp > 0) type = type.Substring(0, temp);
                            int dotIndex = type.LastIndexOf('.');
                            if (dotIndex > 0) package = type.Substring(0, dotIndex);
                        }
                        break;
                    }
                }
            }

            return GetModel(package, cname, inPackage);
        }

        ClassModel ResolveTypeByPackage(string package, string cname, FileModel inFile, string inPackage)
        {
            // quick check in current file
            if (inFile != null && inFile.Classes.Count > 0)
            {
                foreach (ClassModel aClass in inFile.Classes)
                    if (aClass.Name == cname && (package == "" || package == inFile.Package))
                        return aClass;
            }

            // search in classpath
            return GetModel(package, cname, inPackage);
        }

        /// <summary>
        /// Retrieve/build typed copies of generic types
        /// </summary>
        private ClassModel ResolveGenericType(string baseType, string indexType, FileModel inFile)
        {
            ClassModel aClass = ResolveType(baseType, inFile);
            if (aClass.IsVoid()) return aClass;

            if (aClass.QualifiedName == "Dynamic")
            {
                ClassModel indexClass = ResolveType(indexType, inFile);
                //if (!indexClass.IsVoid()) return indexClass;
                return MakeCustomObjectClass(aClass, indexType);
            }

            FileModel aFile = aClass.InFile;
            // is the type already cloned?
            foreach (ClassModel otherClass in aFile.Classes)
                if (otherClass.IndexType == indexType && otherClass.BaseType == baseType)
                    return otherClass;

            // resolve T
            string Tdef = "<T>";
            string Tname = "T";
            Match m = re_Template.Match(aClass.Type);
            if (m.Success)
            {
                Tname = m.Groups[1].Value;
                Tdef = "<" + Tname + ">";
            }
            Regex reReplaceType = new Regex("\\b" + Tname + "\\b");

            // clone the type
            aClass = aClass.Clone() as ClassModel;
            aClass.Name = baseType.Substring(baseType.LastIndexOf('.') + 1) + "<" + indexType + ">";
            aClass.IndexType = indexType;

            if (aClass.ExtendsType != null && aClass.ExtendsType.IndexOf(Tname) >= 0)
                aClass.ExtendsType = reReplaceType.Replace(aClass.ExtendsType, indexType);

            // special Haxe Proxy support
            if (aClass.Type == "haxe.remoting.Proxy<T>" || aClass.Type == "haxe.remoting.Proxy.Proxy<T>")
            {
                aClass.ExtendsType = indexType;
            }

            foreach (MemberModel member in aClass.Members)
            {
                if (member.Type != null && member.Type.IndexOf(Tname) >= 0)
                {
                    member.Type = reReplaceType.Replace(member.Type, indexType);
                }
                if (member.Parameters != null)
                {
                    foreach (MemberModel param in member.Parameters)
                    {
                        if (param.Type != null && param.Type.IndexOf(Tname) >= 0)
                        {
                            param.Type = reReplaceType.Replace(param.Type, indexType);
                        }
                    }
                }
            }

            aFile.Classes.Add(aClass);
            return aClass;
        }

        /// <summary>
        /// Prepare haxe intrinsic known vars/methods/classes
        /// </summary>
        protected override void InitTopLevelElements()
        {
            string filename = "toplevel.hx";
            topLevel = new FileModel(filename);

            // search top-level declaration
            foreach (PathModel aPath in classPath)
                if (File.Exists(Path.Combine(aPath.Path, filename)))
                {
                    filename = Path.Combine(aPath.Path, filename);
                    topLevel = GetCachedFileModel(filename);
                    break;
                }

            if (File.Exists(filename))
            {
                // copy declarations as file-level
                ClassModel tlClass = topLevel.GetPublicClass();
                if (!tlClass.IsVoid())
                {
                    topLevel.Members = tlClass.Members;
                    tlClass.Members = null;
                    topLevel.Classes = new List<ClassModel>();
                }
            }
            // not found
            else
            {
                //ErrorHandler.ShowInfo("Top-level elements class not found. Please check your Program Settings.");
            }

            topLevel.Members.Add(new MemberModel("this", "", FlagType.Variable, Visibility.Public));
            topLevel.Members.Add(new MemberModel("super", "", FlagType.Variable, Visibility.Public));
            //topLevel.Members.Add(new MemberModel("Void", "", FlagType.Intrinsic, Visibility.Public));
            topLevel.Members.Sort();
            foreach (MemberModel member in topLevel.Members)
                member.Flags |= FlagType.Intrinsic;
        }

        /// <summary>
        /// Retrieves a package content
        /// </summary>
        /// <param name="name">Package path</param>
        /// <param name="lazyMode">Force file system exploration</param>
        /// <returns>Package folders and types</returns>
        public override FileModel ResolvePackage(string name, bool lazyMode)
        {
            if ((settings.LazyClasspathExploration || lazyMode) && majorVersion >= 9 && name == "flash") 
                name = "flash9";
            return base.ResolvePackage(name, lazyMode);
        }
        #endregion

        #region Custom code completion

        internal IHaxeCompletionHandler completionModeHandler;

        /// <summary>
        /// Checks completion mode changes to start/restart/stop the haXe completion server if needed.
        /// </summary>
        private void OnCompletionModeChange()
        {
            if (completionModeHandler != null)
            {
                completionModeHandler.Stop();
                completionModeHandler = null;
            }

            // fix environment for command line tools
            SetHaxeEnvironment(currentSDK);

            // configure completion provider
            var haxeSettings = (settings as HaXeSettings);
            features.externalCompletion = haxeSettings.CompletionMode != HaxeCompletionModeEnum.FlashDevelop;

            switch (haxeSettings.CompletionMode)
            {
                case HaxeCompletionModeEnum.Compiler:
                    completionModeHandler = new CompilerCompletionHandler(createHaxeProcess(""));
                    break;
                case HaxeCompletionModeEnum.CompletionServer:
                    if (haxeSettings.CompletionServerPort < 1024)
                        completionModeHandler = new CompilerCompletionHandler(createHaxeProcess(""));
                    else
                    {
                        completionModeHandler =
                            new CompletionServerCompletionHandler(
                                createHaxeProcess("--wait " + haxeSettings.CompletionServerPort),
                                haxeSettings.CompletionServerPort);
                        (completionModeHandler as CompletionServerCompletionHandler).FallbackNeeded += new FallbackNeededHandler(Context_FallbackNeeded);
                    }
                    break;
            }
        }

        void Context_FallbackNeeded(bool notSupported)
        {
            TraceManager.AddAsync("This SDK does not support server mode");
            if (completionModeHandler != null)
            {
                completionModeHandler.Stop();
                completionModeHandler = null;
            }
            completionModeHandler = new CompilerCompletionHandler(createHaxeProcess(""));
        }

        /**
         * Starts a haxe.exe process with the given arguments.
         */
        private Process createHaxeProcess(string args)
        {
            // compiler path
            var hxPath = currentSDK ?? ""; 
            var process = Path.Combine(hxPath, "haxe.exe");
            if (!File.Exists(process))
                return null;

            // Run haxe compiler
            Process proc = new Process();
            proc.StartInfo.FileName = process;
            proc.StartInfo.Arguments = args;
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.RedirectStandardError = true;
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            proc.EnableRaisingEvents = true;
            return proc;
        }

        /// <summary>
        /// Let contexts handle code completion
        /// </summary>
        /// <param name="sci">Scintilla control</param>
        /// <param name="expression">Completion context</param>
        /// <param name="autoHide">Auto-started completion (is false when pressing Ctrl+Space)</param>
        /// <returns>Null (not handled) or member list</returns>
        public override MemberList ResolveDotContext(ScintillaNet.ScintillaControl sci, ASExpr expression, bool autoHide)
        {
            if (resolvingDot || hxsettings.CompletionMode == HaxeCompletionModeEnum.FlashDevelop || PluginBase.MainForm.CurrentDocument.IsUntitled)
                return null;

            if (autoHide && !hxsettings.DisableCompletionOnDemand)
                return null;

            // auto-started completion, can be ignored for performance (show default completion tooltip)
            if (expression.Value.IndexOf(".") < 0 || (autoHide && !expression.Value.EndsWith(".")))
                if (hxsettings.DisableMixedCompletion && expression.Value.Length > 0 && autoHide) return new MemberList();
                else return null;

            // empty expression
            if (expression.Value != "")
            {
                // async processing
                var hc = new HaxeComplete(sci, expression, autoHide, completionModeHandler, HaxeCompilerService.COMPLETION);
                hc.GetList(OnDotCompletionResult);
                resolvingDot = true;
            }

            if (hxsettings.DisableMixedCompletion) return new MemberList();
            return null; 
        }

        internal void OnDotCompletionResult(HaxeComplete hc,  HaxeCompleteResult result, HaxeCompleteStatus status)
        {
            resolvingDot = false;

            switch (status)
            {
                case HaxeCompleteStatus.ERROR:
                    TraceManager.AddAsync(hc.Errors, -3);
                    break;

                case HaxeCompleteStatus.MEMBERS:
                    if (result.Members != null && result.Members.Count > 0)
                        ASComplete.DotContextResolved(hc.Sci, hc.Expr, result.Members, hc.AutoHide);
                    break;

                case HaxeCompleteStatus.TYPE:
                    // eg. Int
                    break;
            }
        }

        /// <summary>
        /// Return the top-level elements (this, super) for the current file
        /// </summary>
        /// <returns></returns>
        public override MemberList GetTopLevelElements()
        {
            GetVisibleExternalElements(); // update cache if needed

            if (topLevel != null)
            {
                MemberList items = new MemberList();
                if (topLevel.OutOfDate) InitTopLevelElements();
                items.Merge(topLevel.Members);
                items.Merge(hxCompletionCache.OtherElements);
                return items;
            }
            else return hxCompletionCache.OtherElements;
        }

        /// <summary>
        /// Return the visible elements (types, package-level declarations) visible from the current file
        /// </summary>
        /// <returns></returns>
        public override MemberList GetVisibleExternalElements()
        {
            if (!IsFileValid) return new MemberList();

            if (completionCache.IsDirty)
            {
                MemberList elements = new MemberList();
                MemberList other = new MemberList();

                // root types & packages
                FileModel baseElements = ResolvePackage(null, false);
                if (baseElements != null)
                {
                    elements.Add(baseElements.Imports);
                    foreach(MemberModel decl in baseElements.Members)
                        if ((decl.Flags & (FlagType.Class | FlagType.Enum | FlagType.TypeDef | FlagType.Abstract)) > 0)
                            elements.Add(decl);
                }
                elements.Add(new MemberModel(features.voidKey, features.voidKey, FlagType.Class | FlagType.Intrinsic, 0));

                // other classes in same package (or parent packages!)
                if (features.hasPackages && cFile.Package != "")
                {
                    string package = cFile.Package;
                    do
                    {
                        int pLen = package.Length;
                        FileModel packageElements = ResolvePackage(package, false);
                        if (packageElements != null)
                        {
                            foreach (MemberModel member in packageElements.Imports)
                            {
                                if (member.Flags != FlagType.Package && member.Type.LastIndexOf('.') == pLen)
                                {
                                    //if (qualify) member.Name = member.Type;
                                    elements.Add(member);
                                }
                            }
                            foreach (MemberModel member in packageElements.Members)
                            {
                                string pkg = member.InFile.Package;
                                //if (qualify && pkg != "") member.Name = pkg + "." + member.Name;
                                member.Type = pkg != "" ? pkg + "." + member.Name : member.Name;
                                elements.Add(member);
                            }
                        }

                        int p = package.LastIndexOf("."); // parent package
                        if (p < 0) break;
                        package = package.Substring(0, p);
                    }
                    while (true);
                }
                // other types in same file
                if (cFile.Classes.Count > 1)
                {
                    ClassModel mainClass = cFile.GetPublicClass();
                    foreach (ClassModel aClass in cFile.Classes)
                    {
                        if (mainClass == aClass) continue;
                        elements.Add(aClass.ToMemberModel());
                        if (aClass.IsEnum())
                            other.Add(aClass.Members);
                    }
                }

                // imports
                MemberList imports = ResolveImports(CurrentModel);
                elements.Add(imports);

                foreach (MemberModel import in imports)
                {
                    if (import is ClassModel)
                    {
                        ClassModel aClass = import as ClassModel;
                        if (aClass.IsEnum()) other.Add(aClass.Members);
                    }
                }

                // in cache
                elements.Sort();
                other.Sort();
                completionCache = hxCompletionCache = new HaxeCompletionCache(this, elements, other);

                // known classes colorization
                if (!CommonSettings.DisableKnownTypesColoring && !settings.LazyClasspathExploration && CurSciControl != null)
                {
                    try
                    {
                        CurSciControl.KeyWords(1, completionCache.Keywords); // additional-keywords index = 1
                        CurSciControl.Colourise(0, -1); // re-colorize the editor
                    }
                    catch (AccessViolationException) { } // catch memory errors
                }
            }

            return completionCache.Elements;
        }

        /// <summary>
        /// Let contexts handle code completion
        /// </summary>
        /// <param name="sci">Scintilla control</param>
        /// <param name="expression">Completion context</param>
        /// <returns>Null (not handled) or function signature</returns>
        public override MemberModel ResolveFunctionContext(ScintillaNet.ScintillaControl sci, ASExpr expression, bool autoHide)
        {
            if (resolvingFunction || hxsettings.CompletionMode == HaxeCompletionModeEnum.FlashDevelop || PluginBase.MainForm.CurrentDocument.IsUntitled)
                return null;

            if (autoHide && !hxsettings.DisableCompletionOnDemand)
                return null;

            // Do not show error
            string val = expression.Value;
            if (val == "for" || 
                val == "while" ||
                val == "if" ||
                val == "switch" ||
                val == "function" ||
                val == "catch" ||
                val == "trace")
                return null;

            expression.Position++;
            var hc = new HaxeComplete(sci, expression, autoHide, completionModeHandler, HaxeCompilerService.COMPLETION);
            hc.GetList(OnFunctionCompletionResult);

            resolvingFunction = true;
            return null; // running asynchronously
        }

        internal void OnFunctionCompletionResult(HaxeComplete hc, HaxeCompleteResult result, HaxeCompleteStatus status)
        {
            resolvingFunction = false;

            switch (status)
            {
                case HaxeCompleteStatus.ERROR:
                    TraceManager.AddAsync(hc.Errors, -3);
                    break;

                case HaxeCompleteStatus.TYPE:
                    hc.Expr.Position--;
                    ASComplete.FunctionContextResolved(hc.Sci, hc.Expr, result.Type, null, true);
                    break;
            }
        }

        public override bool HandleGotoDeclaration(ScintillaControl sci, ASExpr expression)
        {
            if (GetCurrentSDKVersion().IsOlderThan(new SemVer("3.2.0")))
                return false;

            var hc = new HaxeComplete(sci, expression, false, completionModeHandler, HaxeCompilerService.POSITION);
            hc.GetPosition(OnPositionCompletionResult);
            return true;
        }

        internal void OnPositionCompletionResult(HaxeComplete hc, HaxePositionCompleteResult result, HaxeCompleteStatus status)
        {
            if (hc.Sci.InvokeRequired)
            {
                hc.Sci.BeginInvoke((MethodInvoker)delegate
                {
                    HandlePositionCompletionResult(hc, result, status); 
                });
            }
            else HandlePositionCompletionResult(hc, result, status); 
        }

        private void HandlePositionCompletionResult(HaxeComplete hc, HaxePositionCompleteResult result, HaxeCompleteStatus status)
        {
            switch (status)
            {
                case HaxeCompleteStatus.ERROR:
                    TraceManager.Add(hc.Errors, -3);
                    break;

                case HaxeCompleteStatus.POSITION:
                    ASComplete.SaveLastLookupPosition(hc.Sci);

                    PluginBase.MainForm.OpenEditableDocument(result.Path, false);
                    const string keywords = "(function|var|[,(])";

                    ASComplete.LocateMember(keywords, hc.CurrentWord, result.LineStart - 1);
                    break;
            }
        }

        #endregion

        #region command line compiler

        static public string TemporaryOutputFile;

        /// <summary>
        /// Retrieve the context's default compiler path
        /// </summary>
        public override string GetCompilerPath()
        {
            return hxsettings.GetDefaultSDK().Path;
        }

        /// <summary>
        /// Check current file's syntax
        /// </summary>
        public override void CheckSyntax()
        {
            EventManager.DispatchEvent(this, new NotifyEvent(EventType.ProcessStart));
            var hc = new HaxeComplete(ASContext.CurSciControl, new ASExpr(), false, completionModeHandler, HaxeCompilerService.COMPLETION);
            hc.GetList(OnCheckSyntaxResult);
        }

        internal void OnCheckSyntaxResult(HaxeComplete hc, HaxeCompleteResult result, HaxeCompleteStatus status)
        {
            switch (status)
            {
                case HaxeCompleteStatus.ERROR:
                    TraceManager.Add(hc.Errors);
                    EventManager.DispatchEvent(this, new TextEvent(EventType.ProcessEnd, "Done(1)"));
                    break;

                default:
                    EventManager.DispatchEvent(this, new TextEvent(EventType.ProcessEnd, "Done(0)"));
                    break;
            }
        }

        /// <summary>
        /// Run haxe compiler in the current class's base folder with current classpath
        /// </summary>
        /// <param name="append">Additional comiler switches</param>
        public override void RunCMD(string append)
        {
            if (!IsFileValid || !File.Exists(CurrentFile))
                return;

            string haxePath = PathHelper.ResolvePath(hxsettings.GetDefaultSDK().Path);
            if (!Directory.Exists(haxePath) && !File.Exists(haxePath))
            {
                ErrorManager.ShowInfo(TextHelper.GetString("Info.InvalidHaXePath"));
                return;
            }

            SetStatusText(settings.CheckSyntaxRunning);

            try
            {
                // save modified files if needed
                if (outputFile != null) MainForm.CallCommand("SaveAllModified", null);
                else MainForm.CallCommand("SaveAllModified", ".hx");

                // change current directory
                string currentPath = System.IO.Directory.GetCurrentDirectory();
                string filePath = (temporaryPath == null) ? Path.GetDirectoryName(cFile.FileName) : temporaryPath;
                filePath = NormalizePath(filePath);
                System.IO.Directory.SetCurrentDirectory(filePath);
                
                // prepare command
                string command = haxePath;
                if (Path.GetExtension(command) == "") command = Path.Combine(command, "haxe.exe");

                command += ";";
                if (cFile.Package.Length > 0) command += cFile.Package+".";
                string cname = cFile.GetPublicClass().Name;
                if (cname.IndexOf('<') > 0) cname = cname.Substring(0, cname.IndexOf('<'));
                command += cname;

                if (HaxeTarget == "flash" && (append == null || append.IndexOf("-swf-version") < 0)) 
                    command += " -swf-version " + majorVersion;
                // classpathes
                string hxPath = PathHelper.ResolvePath(hxsettings.GetDefaultSDK().Path);
                foreach (PathModel aPath in classPath)
                    if (aPath.Path != temporaryPath
                        && !aPath.Path.StartsWith(hxPath, StringComparison.OrdinalIgnoreCase))
                        command += " -cp \"" + aPath.Path.TrimEnd('\\') + "\"";
                command = command.Replace(filePath, "");

                // run
                MainForm.CallCommand("RunProcessCaptured", command + " " + append);
                // restaure current directory
                if (System.IO.Directory.GetCurrentDirectory() == filePath)
                    System.IO.Directory.SetCurrentDirectory(currentPath);
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Calls RunCMD with additional parameters taken from the classes @haxe doc tag
        /// </summary>
        public override bool BuildCMD(bool failSilently)
        {
            if (!File.Exists(CurrentFile))
                return false;
            // check if @haxe is defined
            Match mCmd = null;
            ClassModel cClass = cFile.GetPublicClass();
            if (IsFileValid)
            {
                if (cFile.Comments != null)
                    mCmd = re_CMD_BuildCommand.Match(cFile.Comments);
                if ((mCmd == null || !mCmd.Success) && cClass.Comments != null)
                    mCmd = re_CMD_BuildCommand.Match(cClass.Comments);
            }

            if (mCmd == null || !mCmd.Success)
            {
                if (!failSilently)
                {
                    MessageBar.ShowWarning(TextHelper.GetString("Info.InvalidForQuickBuild"));
                }
                return false;
            }

            // build command
            string command = mCmd.Groups["params"].Value.Trim();
            try
            {
                command = Regex.Replace(command, "[\\r\\n]\\s*\\*", "", RegexOptions.Singleline);
                command = " " + MainForm.ProcessArgString(command) + " ";
                if (string.IsNullOrEmpty(command))
                {
                    if (!failSilently)
                        throw new Exception(TextHelper.GetString("Info.InvalidQuickBuildCommand"));
                    return false;
                }
                outputFile = null;
                outputFileDetails = "";
                trustFileWanted = false;

                // get some output information url
                MatchCollection mPar = re_SplitParams.Matches(command + "-eof");
                int mPlayIndex = -1;
                bool noPlay = false;
                if (mPar.Count > 0)
                {
                    string op;
                    for (int i = 0; i < mPar.Count; i++)
                    {
                        op = mPar[i].Groups["switch"].Value;
                        int start = mPar[i].Index + mPar[i].Length;
                        int end = (mPar.Count > i + 1) ? mPar[i + 1].Index : start;
                        if ((op == "-swf") && (outputFile == null) && (mPlayIndex < 0))
                        {
                            if (end > start)
                                outputFile = command.Substring(start, end - start).Trim();
                        }
                        else if (op == "-swf-header")
                        {
                            if (end > start)
                            {
                                string[] dims = command.Substring(start, end - start).Trim().Split(':');
                                if (dims.Length > 2) outputFileDetails = ";" + dims[0] + ";" + dims[1];
                            }
                        }
                        else if (op == "-play")
                        {
                            if (end > start)
                            {
                                mPlayIndex = i;
                                outputFile = command.Substring(start, end - start).Trim();
                            }
                        }
                        else if (op == "-trust")
                        {
                            trustFileWanted = true;
                        }
                        else if (op == "-noplay")
                        {
                            noPlay = true;
                        }
                    }
                }
                if (outputFile != null && outputFile.Length == 0) outputFile = null;

                // cleaning custom switches
                if (mPlayIndex >= 0)
                {
                    command = command.Substring(0, mPar[mPlayIndex].Index) + command.Substring(mPar[mPlayIndex + 1].Index);
                }
                if (trustFileWanted)
                {
                    command = command.Replace("-trust", "");
                }
                if (noPlay || !settings.PlayAfterBuild)
                {
                    command = command.Replace("-noplay", "");
                    outputFile = null;
                    runAfterBuild = false;
                }
                else runAfterBuild = (outputFile != null);
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
                return false;
            }

            // run
            RunCMD(command);
            return true;
        }
        #endregion

    }

    class HaxeCompletionCache: CompletionCache
    {
        public MemberList OtherElements;

        public HaxeCompletionCache(ASContext context, MemberList elements, MemberList otherElements)
            : base(context, elements)
        {
            OtherElements = otherElements;
        }
    }
}
