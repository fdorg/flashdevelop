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
using System.Text;
using System.Windows.Forms;
using ProjectManager.Projects.Haxe;
using ProjectManager.Projects;
using AS3Context;
using HaXeContext.Completion;
using HaXeContext.Generators;
using HaXeContext.Model;
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
        private Func<string, InstalledSDK> getCustomSDK;
        private string haxeTarget;
        private bool resolvingDot;
        private bool resolvingFunction;
        HaxeCompletionCache hxCompletionCache;

        internal static readonly ClassModel stubFunctionClass = new ClassModel
        {
            Name = "Function",
            Type = "Function",
            Flags = FlagType.Class,
            Access = Visibility.Public,
            InFile = new FileModel {Package = "haxe", Module = "Constraints"},
        };

        public Context(HaXeSettings initSettings) : this(initSettings, path => null)
        {
        }
        
        public Context(HaXeSettings initSettings, Func<string, InstalledSDK> getCustomSDK)
        {
            hxsettings = initSettings;
            hxsettings.Init();
            this.getCustomSDK = getCustomSDK;

            /* AS-LIKE OPTIONS */

            hasLevels = false;
            docType = "Void"; // "flash.display.MovieClip";

            stubFunctionClass.InFile.Classes.Add(stubFunctionClass);

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
            features.Directives = new List<string> {"true"};

            // allowed declarations access modifiers
            const Visibility all = Visibility.Public | Visibility.Private;
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
            features.ExtendsKey = "extends";
            features.ImplementsKey = "implements";
            features.dot = ".";
            features.voidKey = "Void";
            features.objectKey = "Dynamic";
            features.booleanKey = "Bool";
            features.numberKey = "Float";
            features.IntegerKey = "Int";
            features.stringKey = "String";
            features.arrayKey = "Array<T>";
            features.dynamicKey = "Dynamic";
            features.importKey = "import";
            features.importKeyAlt = "using";
            features.varKey = "var";
            features.overrideKey = "override";
            features.functionKey = "function";
            features.staticKey = "static";
            features.publicKey = "public";
            features.privateKey = "private";
            features.intrinsicKey = "extern";
            features.inlineKey = "inline";
            features.ThisKey = "this";
            features.BaseKey = "super";
            features.hiddenPackagePrefix = '_';
            features.stringInterpolationQuotes = "'";
            features.ConstructorKey = "new";
            features.typesPreKeys = new[] {features.importKey, features.importKeyAlt, "new", features.ExtendsKey, features.ImplementsKey};
            features.codeKeywords = new[] {
                "var", "function", "new", "cast", "return", "break",
                "continue", "if", "else", "for", "in", "while", "do", "switch", "case", "default", "$type",
                "null", "untyped", "true", "false", "try", "catch", "throw", "trace", "macro"
            };
            features.declKeywords = new[] {features.varKey, features.functionKey};
            features.accessKeywords = new[] {features.intrinsicKey, features.inlineKey, "dynamic", "macro", features.overrideKey, features.publicKey, features.privateKey, features.staticKey};
            features.typesKeywords = new[] {features.importKey, features.importKeyAlt, "class", "interface", "typedef", "enum", "abstract" };
            features.ArithmeticOperators = new HashSet<char> {'+', '-', '*', '/', '%'};
            features.IncrementDecrementOperators = new[] {"++", "--"};
            features.BitwiseOperators = new[] {"~", "&", "|", "^", "<<", ">>", ">>>"};
            features.BooleanOperators = new[] {"<", ">", "&&", "||", "!=", "=="};
            /* INITIALIZATION */

            settings = initSettings;

            currentSDK = PathHelper.ResolvePath(hxsettings.GetDefaultSDK().Path) ?? "";
            initSettings.CompletionModeChanged += OnCompletionModeChange;
            initSettings.UseGenericsShortNotationChanged += UseGenericsShortNotationChange;
            //OnCompletionModeChange(); // defered to first use

            haxelibsCache = new Dictionary<string, List<string>>();
            CodeGenerator = new CodeGenerator();
            DocumentationGenerator = new DocumentationGenerator();
            CodeComplete = new CodeComplete();
            //BuildClassPath(); // defered to first use
        }
        #endregion

        #region classpath management

        private List<string> LookupLibrary(string lib)
        {
            try
            {
                return (GetCurrentSDK()?.IsHaxeShim ?? false) ? LookupLixLibrary(lib) : LookupHaxeLibLibrary(lib);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private List<string> LookupHaxeLibLibrary(string lib)
        {
            if (haxelibsCache.ContainsKey(lib))
                return haxelibsCache[lib];

            var haxePath = PathHelper.ResolvePath(GetCompilerPath());
            if (!Directory.Exists(haxePath) && !File.Exists(haxePath))
            {
                ErrorManager.ShowInfo(TextHelper.GetString("Info.InvalidHaXePath"));
                return null;
            }
            if (Directory.Exists(haxePath)) haxePath = Path.Combine(haxePath, "haxelib.exe");

            Process p = StartHiddenProcess(haxePath, "path " + lib);

            List<string> paths = new List<string>();
            do
            {
                string line = p.StandardOutput.ReadLine();
                if (string.IsNullOrEmpty(line)) continue;
                if (line.IndexOfOrdinal("not installed") > 0)
                {
                    TraceManager.Add(line, (int)TraceType.Error);
                }
                else if (!line.StartsWith('-'))
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

            p.WaitForExit();
            p.Close();

            if (paths.Count > 0)
            {
                haxelibsCache.Add(lib, paths);
                return paths;
            }
            return null;
        }

        private List<string> LookupLixLibrary(string lib)
        {
            var haxePath = PathHelper.ResolvePath(GetCompilerPath());
            if (Directory.Exists(haxePath))
            {
                string path = haxePath;
                haxePath = Path.Combine(path, "haxe.exe");
                if (!File.Exists(haxePath)) haxePath = Path.Combine(path, PlatformHelper.IsRunningOnWindows() ? "haxe.cmd" : "haxe");
            }
            if (!File.Exists(haxePath))
            {
                ErrorManager.ShowInfo(TextHelper.GetString("Info.InvalidHaXePath"));
                return null;
            }

            string projectDir = PluginBase.CurrentProject != null ? Path.GetDirectoryName(PluginBase.CurrentProject.ProjectPath) : "";
            Process p = StartHiddenProcess(haxePath, "--run resolve-args -lib " + lib, projectDir);

            List<string> paths = new List<string>();
            bool isPathExpected = false;
            do
            {
                string line = p.StandardOutput.ReadLine();
                if (string.IsNullOrEmpty(line)) continue;
                if (!line.StartsWith('-') && isPathExpected)
                {
                    try
                    {
                        if (Directory.Exists(line))
                            paths.Add(NormalizePath(line).TrimEnd(Path.DirectorySeparatorChar));
                    }
                    catch (Exception) { }
                }
                isPathExpected = line == "-cp";
            }
            while (!p.StandardOutput.EndOfStream);

            string error = p.StandardError.ReadToEnd();
            if (error != "") TraceManager.Add(error, (int)TraceType.Error);

            p.WaitForExit();
            p.Close();

            return paths.Count > 0 ? paths : null;
        }

        private Process StartHiddenProcess(string fileName, string arguments, string workingDirectory = "")
        {
            string hxPath = currentSDK;
            if (hxPath != null && Path.IsPathRooted(hxPath))
            {
                if (hxPath != currentEnv) SetHaxeEnvironment(hxPath);
                fileName = Path.Combine(hxPath, fileName);
                if (File.Exists(fileName + ".exe")) fileName += ".exe";
            }

            ProcessStartInfo pi = new ProcessStartInfo();
            pi.FileName = fileName;
            pi.Arguments = arguments;
            pi.WorkingDirectory = workingDirectory;
            pi.RedirectStandardOutput = true;
            pi.RedirectStandardError = true;
            pi.UseShellExecute = false;
            pi.CreateNoWindow = true;
            pi.WindowStyle = ProcessWindowStyle.Hidden;

            return Process.Start(pi);
        }

        /// <summary>
        /// User refreshes project tree
        /// </summary>
        public override void UserRefreshRequest()
        {
            haxelibsCache.Clear();
            var proj = PluginBase.CurrentProject as HaxeProject;
            if (proj != null) proj.UpdateVars(false);
        }

        /// <summary>
        /// Properly switch between different Haxe SDKs
        /// </summary>
        public void SetHaxeEnvironment(string sdkPath)
        {
            sdkPath = sdkPath.TrimEnd('/', '\\');
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
            path = path.TrimStart(';');
            path = sdkPath + ";" + path;
            if (neko != null) path = neko.TrimEnd('/', '\\') + ";" + path;
            Environment.SetEnvironmentVariable("PATH", path);
            currentEnv = sdkPath;

            LoadMetadata();

            if (GetCurrentSDKVersion() >= "3.3.0") features.SpecialPostfixOperators = new[] {'!'};
            else features.SpecialPostfixOperators = new char[0];

            UseGenericsShortNotationChange();
        }

        private void UseGenericsShortNotationChange()
        {
            // We may want to create 2 different feature flags for this, but atm it's enough this way
            features.HasGenericsShortNotation = GetCurrentSDKVersion() >= "3" && hxsettings.UseGenericsShortNotation;
        }

        public void LoadMetadata()
        {
            features.metadata = new Dictionary<string, string>();

            ProcessStartInfo processInfo = CreateHaxeProcessInfo("--help-metas");
            if (processInfo == null) return;
            string metaList;
            using (var process = new Process {StartInfo = processInfo, EnableRaisingEvents = true})
            {
                process.Start();

                metaList = process.StandardOutput.ReadToEnd();
            }

            Regex regex = new Regex("@:([a-zA-Z]*)(?: : )(.*?)(?=( @:[a-zA-Z]* :|$))");
            metaList = Regex.Replace(metaList, "\\s+", " ");

            MatchCollection matches = regex.Matches(metaList);

            foreach (Match m in matches)
            {
                features.metadata.Add(m.Groups[1].ToString(), m.Groups[2].ToString());
            }
        }

        /// <summary>
        /// Classpathes & classes cache initialization
        /// </summary>
        public override void BuildClassPath()
        {
            ReleaseClasspath();
            started = true;
            if (hxsettings == null) throw new Exception("BuildClassPath() must be overridden");
            if (contextSetup == null)
            {
                contextSetup = new ContextSetupInfos();
                contextSetup.Classpath = new[] { Environment.CurrentDirectory };
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
                    if (contextSetup.TargetBuild == null || contextSetup.TargetBuild.StartsWithOrdinal("flash"))
                        lang = "";
                    else if (contextSetup.TargetBuild.StartsWithOrdinal("html5"))
                        lang = "js";
                    else if (contextSetup.TargetBuild.Contains("neko"))
                        lang = "neko";
                }
            }
            else if (lang == "swf")
            {
                lang = "flash";
            }
            features.Directives.Add(lang);
            haxeTarget = lang;

            //
            // Class pathes
            //
            classPath = new List<PathModel>();
            // Haxe std
            var project = PluginBase.CurrentProject as HaxeProject;
            var hxPath = project != null ? project.CurrentSDK : PathHelper.ResolvePath(hxsettings.GetDefaultSDK().Path);
            if (hxPath != null)
            {
                if (currentSDK != hxPath)
                {
                    currentSDK = hxPath;
                    haxelibsCache = new Dictionary<string, List<string>>();
                    OnCompletionModeChange();
                }

                var installedSDK = GetCurrentSDK();
                var haxeCP = (installedSDK != null && installedSDK.IsHaxeShim) ? installedSDK.ClassPath : Path.Combine(hxPath, "std");
                if (Directory.Exists(haxeCP))
                {
                    if (project != null)
                    {
                        project.ExternalLibraries.Clear();
                        project.ExternalLibraries.Add(haxeCP);
                    }
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
                    if (haxeTarget == "flash")
                        lang = (majorVersion >= 6 && majorVersion < 9) ? FLASH_OLD : FLASH_NEW;

                    PathModel std = PathModel.GetModel(haxeCP, this);
                    if (!std.WasExplored && !Settings.LazyClasspathExploration)
                    {
                        string[] keep = { "sys", "haxe", "libs" };
                        var hide = new List<string>();
                        foreach (var dir in Directory.GetDirectories(haxeCP))
                            if (!keep.Contains(Path.GetFileName(dir)))
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

            // swf-libs
            if (project != null && haxeTarget == "flash" && majorVersion >= 9)
            {
                foreach(LibraryAsset asset in project.LibraryAssets)
                    if (asset.IsSwc)
                    {
                        string path = project.GetAbsolutePath(asset.Path);
                        if (File.Exists(path)) AddPath(path);
                    }
                foreach(string p in project.CompilerOptions.Additional)
                    if (p.IndexOfOrdinal("-swf-lib ") == 0) {
                        string path = project.GetAbsolutePath(p.Substring(9));
                        if (File.Exists(path)) AddPath(path);
                    }
            }

            // add haxe libraries
            if (project != null)
            {
                var libraries = project.CompilerOptions.Libraries.ToList();
                foreach (var it in project.CompilerOptions.Additional)
                {
                    var index = it.IndexOfOrdinal("-lib ");
                    if (index != -1) libraries.Add(it.Substring(index + "-lib ".Length).Trim());
                }
                foreach (string library in libraries)
                    if (!string.IsNullOrEmpty(library.Trim()))
                    {
                        List<string> libPaths = LookupLibrary(library);
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

        protected override bool ExplorePath(PathModel path)
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
                        if (src.Contains("<project name=\"nme\""))
                        {
                            ManualExploration(path, new[] { "js", "jeash", "neash", "native", "browser", "flash", "neko", "tools", "samples", "project" });
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
        /// Confirms that the FileModel should be added to the PathModel
        /// - typically classes whose context do not patch the classpath should be ignored
        /// </summary>
        /// <param name="aFile"></param>
        /// <param name="pathModel"></param>
        /// <returns></returns>
        public override bool IsModelValid(FileModel aFile, PathModel pathModel)
        {
            if (!pathModel.ValidatePackage) return true;
            var path = Path.GetDirectoryName(aFile.FileName);
            if (!path.StartsWith(pathModel.Path, StringComparison.OrdinalIgnoreCase)) return false;
            string package = path.Length <= pathModel.Path.Length ? "" : path.Substring(pathModel.Path.Length + 1).Replace('/', '.').Replace('\\', '.');
            return (aFile.Package == package);
        }

        /// <inheritdoc />
        public override FileModel GetCodeModel(FileModel result, string src, bool scriptMode)
        {
            result.haXe = true;
            base.GetCodeModel(result, src, scriptMode);
            if (result.Members != null)
            {
                for (var i = 0; i < result.Members.Count; i++)
                {
                    var member = result.Members[i];
                    if (!member.Flags.HasFlag(FlagType.Function) || !(member.Parameters?.Count > 0)) continue;
                    foreach (var parameter in member.Parameters)
                    {
                        if (parameter.Name[0] != '?') continue;
                        parameter.Name = parameter.Name.Substring(1);
                        var type = parameter.Type;
                        if (string.IsNullOrEmpty(type)) parameter.Type = "Null<Dynamic>";
                        else if (!type.StartsWithOrdinal("Null<")) parameter.Type = $"Null<{type}>";
                    }
                }
            }
            if (result.Classes != null)
            {
                for (int i = 0, length = result.Classes.Count; i < length; i++)
                {
                    var @class = result.Classes[i];
                    var flags = @class.Flags;
                    if ((flags & FlagType.Abstract) != 0)
                    {
                        var meta = @class.MetaDatas;
                        if (meta != null && meta.Any(it => it.Name == ":enum"))
                        {
                            /**
                             * transform
                             * @:enum abstract AType(T) {
                             *     var Value;
                             * }
                             * to
                             * @:enum abstract AType(T) {
                             *     public static var Value;
                             * }
                             */
                            for (int index = 0, count = @class.Members.Count; index < count; index++)
                            {
                                var member = @class.Members[index];
                                if (!member.Flags.HasFlag(FlagType.Variable)) continue;
                                member.Flags = FlagType.Enum | FlagType.Static | FlagType.Variable;
                                member.Access = Visibility.Public;
                                if (string.IsNullOrEmpty(member.Type)) member.Type = @class.Type;
                                member.InFile = @class.InFile;
                            }
                        }
                    }
                    else if (flags == FlagType.Class && @class.Members.Count > 0)
                    {
                        /**
                         * transform
                         * class Bar extends Foo {
                         *     override foo() {}
                         * }
                         * class Foo {
                         *     public function foo() {}
                         * }
                         * to
                         * class Bar extends Foo {
                         *     public override foo() {}
                         * }
                         * class Foo {
                         *     public function foo() {}
                         * }
                         */
                        @class.ResolveExtends();
                        var parent = @class.Extends;
                        while (!parent.IsVoid())
                        {
                            for (int index = 0, count = @class.Members.Count; index < count; index++)
                            {
                                var member = @class.Members[index];
                                if ((member.Flags & FlagType.Override) == 0
                                    || (member.Access & Visibility.Public) != 0
                                    || !parent.Members.Contains(member.Name, 0, Visibility.Public)) continue;
                                member.Access = Visibility.Public;
                                member.Flags |= FlagType.Access;
                                break;
                            }
                            parent = parent.Extends;
                        }
                    }
                }
            }
            return result;
        }

        protected override IFileParser GetCodeParser() => new FileParser(context.Features);

        /// <summary>
        /// Delete current class's cached file
        /// </summary>
        public override void RemoveClassCompilerCache()
        {
            // not implemented - is there any?
        }
        #endregion

        #region SDK
        private InstalledSDK GetCurrentSDK() => hxsettings.InstalledSDKs?.FirstOrDefault(sdk => sdk.Path == currentSDK) ?? getCustomSDK(currentSDK);

        public SemVer GetCurrentSDKVersion()
        {
            var sdk = GetCurrentSDK();
            return sdk != null ? new SemVer(sdk.Version) : SemVer.Zero;
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
            if (withClass != null && inClass.InFile == withClass.InFile && inClass.BaseType == withClass.BaseType)
                return Visibility.Public | Visibility.Private;
            // inheritance affinity
            var tmp = inClass;
            while (!tmp.IsVoid())
            {
                if (tmp == withClass)
                    return Visibility.Public | Visibility.Private;
                tmp = tmp.Extends;
            }
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
            string package = CurrentModel?.Package;
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

            if (cFile != null)
                foreach(ClassModel aClass in cFile.Classes)
                    fullList.Add(aClass.ToMemberModel());

            // in cache
            fullList.Sort();
            completionCache.AllTypes = fullList;
            return fullList;
        }

        public override bool OnCompletionInsert(ScintillaControl sci, int position, string text, char trigger)
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
        /// <param name="inFile">Current file</param>
        public override MemberList ResolveImports(FileModel inFile)
        {
            if (inFile == cFile && completionCache.Imports != null) 
                return completionCache.Imports;

            var imports = new MemberList();
            if (inFile == null) return imports;
            foreach (MemberModel item in inFile.Imports)
            {
                if (item.Name != "*") ResolveImport(item, imports);
                else
                {
                    var cname = item.Type.Substring(0, item.Type.Length - 2);
                    // classes matching wildcard
                    var matches = ResolvePackage(cname, false);
                    if (matches != null)
                    {
                        imports.Add(matches.Imports);
                        imports.Add(matches.Members);
                    }
                    else
                    {
                        var model = ResolveType(cname, null);
                        if (!model.IsVoid())
                        {
                            var access = TypesAffinity(model, Context.CurrentClass);
                            foreach (MemberModel member in model.Members)
                            {
                                if ((member.Flags & FlagType.Static) > 0 && (member.Access & access) != 0)
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
                if (cClass != null && !cClass.IsVoid())
                    ResolveImport(cClass, imports);
            }
            else
            {
                foreach (var aClass in inFile.Classes)
                    if (aClass.Access != Visibility.Private) ResolveImport(aClass, imports);
            }
            imports.Add(ResolveDefaults(inFile.Package));
            if (imports.Count > 0)
            {
                // haxe3: type resolution from bottom to top
                imports.Items.Reverse();
                if (inFile == cFile)
                {
                    completionCache.Imports = imports;
                    for (var i = 0; i < imports.Count; i++)
                    {
                        var import = imports[i].Type;
                        if (import == null) continue;
                        var p1 = import.LastIndexOf('.');
                        if (p1 == -1) continue;
                        var lpart = import.Substring(0, p1);
                        var p2 = lpart.LastIndexOf('.');
                        if (p2 != -1) lpart = import.Substring(p2 + 1);
                        if (char.IsLower(lpart[0])) continue;
                        var type = ResolveType(lpart, Context.CurrentModel);
                        if (type.IsVoid() || type.Members.Count <= 0) continue;
                        var rpart = import.Substring(p1 + 1);
                        var member = type.Members.Search(rpart, FlagType.Static, Visibility.Public);
                        if (member == null) continue;
                        member = (MemberModel) member.Clone();
                        member.InFile = type.InFile;
                        imports[i] = member;
                    }
                }
            }
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

            if (fileName.StartsWithOrdinal("flash" + dirSeparator))
            {
                if (haxeTarget != "flash" || majorVersion > 8) // flash9 remap
                    fileName = FLASH_NEW + fileName.Substring(5);
                else
                    fileName = FLASH_OLD + fileName.Substring(5);
            }

            bool matched = false;
            foreach (PathModel aPath in classPath)
                if (aPath.IsValid && !aPath.Updating)
                {
                    var path = aPath.Path + dirSeparator + fileName;

                    FileModel file;
                    // cached file
                    if (aPath.TryGetFile(path, out file))
                    {
                        if (file.Context != this)
                        {
                            // not associated with this context -> refresh
                            file.OutOfDate = true;
                            file.Context = this;
                        }

                        // add all public classes of Haxe modules
                        foreach (ClassModel c in file.Classes)
                            if (c.IndexType == null && c.Access == Visibility.Public)
                                imports.Add(c);
                        matched = true;
                    }
                }

            if (!matched) imports.Add(new MemberModel(item.Name, item.Type, FlagType.Class, Visibility.Public));
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
            if (member == ClassModel.VoidClass) return false;
            if (member.InFile?.Package == CurrentModel.Package) return true;
            var name = member.Name;
            var p = name.IndexOf('#');
            if (p > 0) name = name.Substring(0, p);
            var type = member.Type;
            var isShortType = name == type;
            var curFile = Context.CurrentModel;
            var imports = curFile.Imports.Items.Concat(ResolveDefaults(curFile.Package).Items).ToArray();
            foreach (var import in imports)
            {
                if ((isShortType && import.Name == name) || import.Type == type) return true;
                if (import.Name == "*" && import.Type.Replace("*", name) == type) return true;
                if (type.StartsWithOrdinal(import.Type + ".")) return true;
            }
            return false;
        }

        /// <summary>
        /// Retrieves a class model from its name
        /// </summary>
        /// <param name="cname">Class (short or full) name</param>
        /// <param name="inFile">Current file</param>
        /// <returns>A parsed class or an empty ClassModel if the class is not found</returns>
        public override ClassModel ResolveType(string cname, FileModel inFile)
        {
            // unknown type
            if (string.IsNullOrEmpty(cname) || cname == features.voidKey || classPath == null)
                return ClassModel.VoidClass;
            
            // handle generic types
            if (cname.Contains('<'))
            {
                var genType = re_genericType.Match(cname);
                if (genType.Success)
                    return ResolveGenericType(genType.Groups["gen"].Value, genType.Groups["type"].Value, inFile);
                return ClassModel.VoidClass;
            }
            
            // typed array
            if (cname.Contains('@')) return ResolveTypeIndex(cname, inFile);

            var package = "";
            var inPackage = (features.hasPackages && inFile != null) ? inFile.Package : "";

            var p = cname.LastIndexOf('.');
            if (p > 0)
            {
                package = cname.Substring(0, p);
                cname = cname.Substring(p + 1);
            }
            else 
            {
                // search in file
                if (inFile != null)
                    foreach (var aClass in inFile.Classes)
                        if (aClass.Name == cname)
                            return aClass;

                // search in imported classes
                var found = false;
                var imports = ResolveImports(inFile);
                foreach (MemberModel import in imports)
                {
                    if (import.Name != cname) continue;
                    if (import.Type != null && import.Type.Length > import.Name.Length)
                    {
                        var type = import.Type;
                        var temp = type.IndexOf('<');
                        if (temp > 0) type = type.Substring(0, temp);
                        var dotIndex = type.LastIndexOf('.');
                        if (dotIndex > 0) package = type.Substring(0, dotIndex);
                    }
                    found = true;
                    break;
                }
                if (!found && cname == "Function") return stubFunctionClass;
            }
            return GetModel(package, cname, inPackage);
        }

        static readonly Regex re_isExpr = new Regex(@"\((?<lv>.+)\s(?<op>is)\s+(?<rv>\w+)\)");

        public override ClassModel ResolveToken(string token, FileModel inFile)
        {
            var tokenLength = token != null ? token.Length : 0;
            if (tokenLength > 0)
            {
                if (token.StartsWithOrdinal("0x")) return ResolveType("Int", inFile);
                var first = token[0];
                var last = token[tokenLength - 1];
                if (first == '[' && last == ']')
                {
                    var arrCount = 0;
                    var dQuotes = 0;
                    var sQuotes = 0;
                    var arrayComprehensionEnd = tokenLength - 3;
                    for (var i = 1; i < tokenLength; i++)
                    {
                        var c = token[i];
                        if (c == '\"' && sQuotes == 0)
                        {
                            if (token[i - 1] == '\\') continue;
                            if (dQuotes == 0) dQuotes++;
                            else dQuotes--;
                        }
                        else if (c == '\'' && dQuotes == 0)
                        {
                            if (token[i - 1] == '\\') continue;
                            if (sQuotes == 0) sQuotes++;
                            else sQuotes--;
                        }
                        if(sQuotes > 0 || dQuotes > 0) continue;
                        if (c == '[') arrCount++;
                        else if (c == ']') arrCount--;
                        if (arrCount > 0) continue;
                        if (i <= arrayComprehensionEnd && c == '=' && token[i + 1] == '>')
                            // TODO: try parse K, V
                            return ResolveType("Map<K, V>", inFile);
                    }
                    return ResolveType(features.arrayKey, inFile);
                }
                if (first == '{' && last == '}')
                {
                    //TODO: parse anonymous type
                    return ResolveType(features.dynamicKey, inFile);
                }
                if (first == '(' && last == ')')
                {
                    if (re_isExpr.IsMatch(token)) return ResolveType(features.booleanKey, inFile);
                    if (GetCurrentSDKVersion() >= "3.1.0")
                    {
                        var groupCount = 0;
                        var length = tokenLength - 2;
                        var sb = new StringBuilder(length);
                        for (var i = length; i >= 1; i--)
                        {
                            var c = token[i];
                            if (c <= ' ') continue;
                            if (c == '}' || c == ')' || c == '>') groupCount++;
                            else if (c == '{' || c == '(' || c == '<') groupCount--;
                            else if (c == ':' && groupCount == 0) break;
                            sb.Insert(0, c);
                        }
                        return ResolveType(sb.ToString(), inFile);
                    }
                }
                else if (token.StartsWithOrdinal("cast("))
                {
                    var groupCount = 0;
                    var length = tokenLength - 2;
                    var sb = new StringBuilder(length);
                    for (var i = length; i >= 1; i--)
                    {
                        var c = token[i];
                        if (c <= ' ') continue;
                        if (c == '}' || c == ')' || c == '>') groupCount++;
                        else if (c == '{' || c == '(' || c == '<') groupCount--;
                        else if (c == ',' && groupCount == 0) break;
                        sb.Insert(0, c);
                    }
                    return ResolveType(sb.ToString(), inFile);
                }
                var index = token.IndexOfOrdinal(" ");
                if (index != -1)
                {
                    var word = token.Substring(0, index);
                    if (word == "new" && last == ')')
                    {
                        var dot = ' ';
                        var parCount = 0;
                        for (var i = 0; i < tokenLength; i++)
                        {
                            var c = token[i];
                            if (c == '(') parCount++;
                            else if (c == ')')
                            {
                                parCount--;
                                if (parCount == 0) dot = '.';
                            }
                            else if (dot != ' ' && c == dot) return ClassModel.VoidClass;
                        }
                        token = token.Substring(index + 1);
                        token = Regex.Replace(token, @"\(.*", string.Empty);
                        return ResolveType(token, inFile);
                    }
                }
            }
            return base.ResolveToken(token, inFile);
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

            if (aClass.QualifiedName == features.dynamicKey)
            {
                var result = MakeCustomObjectClass(aClass, indexType);
                result.Template = aClass.Template;
                result.Type = aClass.Type;
                return result;
            }

            FileModel aFile = aClass.InFile;
            // is the type already cloned?
            foreach (ClassModel otherClass in aFile.Classes)
                if (otherClass.IndexType == indexType && otherClass.BaseType == baseType)
                    return otherClass;

            // resolve T
            string Tname = "T";
            Match m = re_Template.Match(aClass.Type);
            if (m.Success)
            {
                Tname = m.Groups[1].Value;
            }
            Regex reReplaceType = new Regex("\\b" + Tname + "\\b");

            // clone the type
            aClass = (ClassModel) aClass.Clone();
            aClass.Name = baseType.Substring(baseType.LastIndexOf('.') + 1) + "<" + indexType + ">";
            aClass.IndexType = indexType;

            if (aClass.ExtendsType != null && aClass.ExtendsType.Contains(Tname))
                aClass.ExtendsType = reReplaceType.Replace(aClass.ExtendsType, indexType);

            // special Haxe Proxy support
            if (aClass.Type == "haxe.remoting.Proxy<T>" || aClass.Type == "haxe.remoting.Proxy.Proxy<T>")
            {
                aClass.ExtendsType = indexType;
            }

            foreach (MemberModel member in aClass.Members)
            {
                if (member.Type != null && member.Type.Contains(Tname))
                {
                    member.Type = reReplaceType.Replace(member.Type, indexType);
                }
                if (member.Parameters != null)
                {
                    foreach (MemberModel param in member.Parameters)
                    {
                        if (param.Type != null && param.Type.Contains(Tname))
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
            {
                var path = Path.Combine(aPath.Path, filename);
                if (File.Exists(path))
                {
                    filename = path;
                    topLevel = GetCachedFileModel(filename);
                    break;
                }
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

        /// <summary>
        /// https://haxe.org/blog/importhx-intro/
        /// Since Haxe 3.3.0
        /// </summary>
        /// <param name="package">Package path</param>
        /// <returns>Imported classes list (not null)</returns>
        private MemberList ResolveDefaults(string package)
        {
            var result = new MemberList();
            if (GetCurrentSDKVersion() < "3.3.0") return result;
            var packagePath = string.IsNullOrEmpty(package) ? string.Empty : package.Replace('.', dirSeparatorChar);
            while(true)
            {
                foreach (var it in classPath)
                {
                    if (!it.IsValid || it.Updating || it.FilesCount == 0) continue;
                    var path = Path.Combine(it.Path, packagePath, "import.hx");
                    FileModel model;
                    if (!it.TryGetFile(path, out model)) continue;
                    result.Add(model.Imports);
                    break;
                }
                if (string.IsNullOrEmpty(packagePath)) break;
                packagePath = Path.GetDirectoryName(packagePath);
            }
            return result;
        }

        public override string GetDefaultValue(string type)
        {
            if (string.IsNullOrEmpty(type) || type == features.voidKey) return null;
            switch (type)
            {
                case "Int":
                case "UInt": return "0";
                case "Float": return "Math.NaN";
                case "Bool": return "false";
                default: return "null";
            }
        }

        public override IEnumerable<string> DecomposeTypes(IEnumerable<string> types)
        {
            var characterClass = ScintillaControl.Configuration.GetLanguage("haxe").characterclass.Characters;
            var result = new HashSet<string>();
            foreach (var type in types)
            {
                if (string.IsNullOrEmpty(type)) continue;
                if (type.Contains('{') || type.Contains('<') || type.Contains("->"))
                {
                    var length = type.Length;
                    var braCount = 0;
                    var genCount = 0;
                    var inAnonType = false;
                    var hasColon = false;
                    var pos = 0;
                    for (var i = 0; i < length; i++)
                    {
                        var c = type[i];
                        if (c == '.' || characterClass.Contains(c)) continue;
                        if (c <= ' ') pos++;
                        else if (c == '(') pos = i + 1;
                        else if (c == '<')
                        {
                            genCount++;
                            result.Add(type.Substring(pos, i - pos));
                            pos = i + 1;
                        }
                        else if (c == '>')
                        {
                            genCount--;
                            if (i > pos) result.Add(type.Substring(pos, i - pos));
                            pos = i + 1;
                        }
                        else if (c == '{')
                        {
                            inAnonType = true;
                            braCount++;
                            hasColon = false;
                            pos = i + 1;
                        }
                        else if (c == '}')
                        {
                            if (i > pos) result.Add(type.Substring(pos, i - pos));
                            if (--braCount == 0) inAnonType = false;
                            pos = i + 1;
                        }
                        else if (inAnonType)
                        {
                            if (hasColon)
                            {
                                if (c == ',')
                                {
                                    result.Add(type.Substring(pos, i - pos));
                                    pos = i + 1;
                                    hasColon = false;
                                }
                            }
                            else if(c == ':')
                            {
                                hasColon = true;
                                pos = i + 1;
                            }
                        }
                        else if (genCount > 0)
                        {
                            if (c == ',')
                            {
                                if (i > pos) result.Add(type.Substring(pos, i - pos));
                                pos = i + 1;
                            }
                        }
                        if (c == '-')
                        {
                            if (i > pos) result.Add(type.Substring(pos, i - pos));
                            i++;
                            pos = i + 1;
                            hasColon = false;
                            if (braCount == 0 && genCount == 0 
                                && type.IndexOf('{', pos) == -1
                                && type.IndexOf('<', pos) == -1
                                && type.IndexOf(',', pos) == -1)
                            {
                                result.Add(type.Substring(pos));
                                break;
                            }
                        }
                    }
                }
                else result.Add(type);
            }
            return result;
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
                    completionModeHandler = new CompilerCompletionHandler(CreateHaxeProcessInfo(""));
                    break;
                case HaxeCompletionModeEnum.CompletionServer:
                    if (haxeSettings.CompletionServerPort < 1024)
                        completionModeHandler = new CompilerCompletionHandler(CreateHaxeProcessInfo(""));
                    else
                    {
                        completionModeHandler =
                            new CompletionServerCompletionHandler(
                                CreateHaxeProcessInfo("--wait " + haxeSettings.CompletionServerPort),
                                haxeSettings.CompletionServerPort);
                        ((CompletionServerCompletionHandler) completionModeHandler).FallbackNeeded += Context_FallbackNeeded;
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
            completionModeHandler = new CompilerCompletionHandler(CreateHaxeProcessInfo(""));
        }

        /**
         * Gets the needed information to create a haxe.exe process with the given arguments.
         */
        internal ProcessStartInfo CreateHaxeProcessInfo(string args)
        {
            // compiler path
            var hxPath = currentSDK ?? ""; 
            var process = Path.Combine(hxPath, "haxe.exe");
            if (!File.Exists(process) && (GetCurrentSDK()?.IsHaxeShim ?? false))
                process = Path.Combine(hxPath, PlatformHelper.IsRunningOnWindows() ? "haxe.cmd" : "haxe");
            if (!File.Exists(process))
                return null;

            // Prepare process information
            var procInfo = new ProcessStartInfo();
            procInfo.FileName = process;
            procInfo.Arguments = args;
            procInfo.UseShellExecute = false;
            procInfo.RedirectStandardOutput = true;
            procInfo.RedirectStandardError = true;
            procInfo.CreateNoWindow = true;
            procInfo.WindowStyle = ProcessWindowStyle.Hidden;
            return procInfo;
        }

        internal HaxeComplete GetHaxeComplete(ScintillaControl sci, ASExpr expression, bool autoHide, HaxeCompilerService compilerService)
        {
            var sdkVersion = GetCurrentSDKVersion();
            if (sdkVersion >= "4.0.0") return new HaxeComplete400(sci, expression, autoHide, completionModeHandler, compilerService, sdkVersion);
            if (sdkVersion >= "3.3.0") return new HaxeComplete330(sci, expression, autoHide, completionModeHandler, compilerService, sdkVersion);
            return new HaxeComplete(sci, expression, autoHide, completionModeHandler, compilerService, sdkVersion);
        }

        /// <summary>
        /// Let contexts handle code completion
        /// </summary>
        /// <param name="sci">Scintilla control</param>
        /// <param name="expression">Completion context</param>
        /// <param name="autoHide">Auto-started completion (is false when pressing Ctrl+Space)</param>
        /// <returns>Null (not handled) or member list</returns>
        public override MemberList ResolveDotContext(ScintillaControl sci, ASExpr expression, bool autoHide)
        {
            if (resolvingDot || hxsettings.CompletionMode == HaxeCompletionModeEnum.FlashDevelop
                || PluginBase.MainForm.CurrentDocument.IsUntitled)
                return null;
            if (autoHide && !hxsettings.DisableCompletionOnDemand) return null;
            var exprValue = expression.Value;
            // auto-started completion, can be ignored for performance (show default completion tooltip)
            if (!exprValue.Contains('.') || (autoHide && !exprValue.EndsWith('.')))
                if (hxsettings.DisableMixedCompletion && exprValue.Length > 0 && autoHide) return new MemberList();
                else return null;

            // empty expression
            if (exprValue != "")
            {
                // async processing
                var hc = GetHaxeComplete(sci, expression, autoHide, HaxeCompilerService.COMPLETION);
                hc.GetList(OnDotCompletionResult);
                resolvingDot = true;
            }
            return hxsettings.DisableMixedCompletion ? new MemberList() : null;
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

        public override void ResolveDotContext(ScintillaControl sci, ASResult expression, MemberList result)
        {
            if (expression.IsStatic && expression.Type is ClassModel type 
                && type.InFile is FileModel file && file.Classes.Count > 1
                && type == GetPublicClass(file))
            {
                // add sub-types
                foreach (var it in file.Classes)
                {
                    if (it != type) result.Add(it);
                }
                return;
            }
            var exprValue = expression.Context.Value;
            if (exprValue.Length >= 3)
            {
                var first = exprValue[0];
                if ((first == '\"' || first == '\'') && expression.Context.SubExpressions != null && expression.Context.SubExpressions.Count == 1)
                {
                    var s = exprValue.Replace(".#0~.", string.Empty);
                    if (s.Length == 3 || (s.Length == 4 && s[1] == '\\'))
                    {
                        result.Add(new MemberModel("code", "Int", FlagType.Getter, Visibility.Public) {Comments = "The character code of this character(inlined at compile-time)"});
                    }
                }
            }
        }

        ClassModel GetPublicClass(FileModel file)
        {
            if (file?.Classes != null)
            {
                var module = file.Module == "" ? Path.GetFileNameWithoutExtension(file.FileName) : file.Module;
                foreach (var model in file.Classes)
                    if ((model.Flags & (FlagType.Class | FlagType.Interface | FlagType.Enum)) != 0 && model.Name == module)
                    {
                        return model;
                    }
            }
            return ClassModel.VoidClass;
        }

        /// <summary>
        /// Return the top-level elements (this, super) for the current file
        /// </summary>
        /// <returns></returns>
        public override MemberList GetTopLevelElements()
        {
            GetVisibleExternalElements(); // update cache if needed
            if (topLevel == null) return hxCompletionCache.OtherElements;
            var items = new MemberList();
            if (topLevel.OutOfDate) InitTopLevelElements();
            items.Add(topLevel.Members);
            items.Add(hxCompletionCache.OtherElements);
            return items;
        }

        /// <inheritdoc />
        public override void ResolveTopLevelElement(string token, ASResult result)
        {
            var list = GetTopLevelElements();
            if (list != null && list.Count > 0)
            {
                var items = list.MultipleSearch(token, 0, 0);
                if (items != null)
                {
                    if (items.Count == 1)
                    {
                        var item = items[0];
                        result.InClass = ClassModel.VoidClass;
                        result.InFile = item.InFile;
                        result.Member = item;
                        result.Type = ResolveType(item.Type, item.InFile);
                        result.IsStatic = false;
                        result.IsPackage = false;
                    }
                    return;
                }
            }
            base.ResolveTopLevelElement(token, result);
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
                var elements = new MemberList();
                var other = new MemberList();

                // root types & packages
                var baseElements = ResolvePackage(null, false);
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
                    var package = cFile.Package;
                    do
                    {
                        var pLen = package.Length;
                        var packageElements = ResolvePackage(package, false);
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
                                var pkg = member.InFile.Package;
                                //if (qualify && pkg != "") member.Name = pkg + "." + member.Name;
                                member.Type = pkg != "" ? pkg + "." + member.Name : member.Name;
                                elements.Add(member);
                            }
                        }

                        var p = package.LastIndexOf('.'); // parent package
                        if (p < 0) break;
                        package = package.Substring(0, p);
                    } while (true);
                }
                // other types in same file
                if (cFile.Classes.Count > 1)
                {
                    var mainClass = cFile.GetPublicClass();
                    foreach (var aClass in cFile.Classes)
                    {
                        if (mainClass == aClass) continue;
                        elements.Add(aClass.ToMemberModel());
                        TryAddEnums(aClass, other);
                    }
                }
                // imports
                var imports = ResolveImports(CurrentModel);
                elements.Add(imports);
                foreach (MemberModel import in imports)
                {
                    TryAddEnums(import as ClassModel ?? ResolveType(import.Name, cFile), other);
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
        /// Adds members of `model` into `result` if `model` is enum or abstract with meta tag `@:enum`
        /// </summary>
        /// <param name="model"></param>
        /// <param name="result"></param>
        static void TryAddEnums(ClassModel model, MemberList result)
        {
            if (model == null || model.IsVoid()) return;
            if (model.IsEnum())
            {
                for (var i = 0; i < model.Members.Count; i++)
                {
                    var member = model.Members[i];
                    if (member.Type == null || model.InFile == null)
                    {
                        member = (MemberModel) member.Clone();
                        member.Type = model.Type;
                        member.InFile = model.InFile;
                    }
                    result.Add(member);
                }
            }
            else if (model.Flags.HasFlag(FlagType.Abstract))
            {
                var meta = model.MetaDatas;
                if (meta == null || meta.All(it => it.Name != ":enum")) return;
                foreach (MemberModel member in model.Members)
                {
                    if (member.Flags.HasFlag(FlagType.Variable)) result.Add(member);
                }
            }
        }

        /// <summary>
        /// Let contexts handle code completion
        /// </summary>
        /// <param name="sci">Scintilla control</param>
        /// <param name="expression">Completion context</param>
        /// <returns>Null (not handled) or function signature</returns>
        public override MemberModel ResolveFunctionContext(ScintillaControl sci, ASExpr expression, bool autoHide)
        {
            if (resolvingFunction || hxsettings.CompletionMode == HaxeCompletionModeEnum.FlashDevelop
                || PluginBase.MainForm.CurrentDocument.IsUntitled)
                return null;

            if (autoHide && !hxsettings.DisableCompletionOnDemand)
                return null;

            // Do not show error
            var val = expression.Value;
            if (val == "for" || 
                val == "while" ||
                val == "if" ||
                val == "switch" ||
                val == "function" ||
                val == "catch" ||
                val == "trace")
                return null;

            expression.Position++;
            var hc = GetHaxeComplete(sci, expression, autoHide, HaxeCompilerService.COMPLETION);
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
            if (hxsettings.CompletionMode == HaxeCompletionModeEnum.FlashDevelop || GetCurrentSDKVersion() < "3.2.0")
                return false;

            var hc = GetHaxeComplete(sci, expression, false, HaxeCompilerService.POSITION);
            hc.GetPosition(OnPositionResult);
            return true;
        }

        internal void OnPositionResult(HaxeComplete hc, HaxePositionResult result, HaxeCompleteStatus status)
        {
            if (hc.Sci.InvokeRequired)
            {
                hc.Sci.BeginInvoke((MethodInvoker)delegate
                {
                    HandlePositionResult(hc, result, status); 
                });
            }
            else HandlePositionResult(hc, result, status); 
        }

        private void HandlePositionResult(HaxeComplete hc, HaxePositionResult result, HaxeCompleteStatus status)
        {
            switch (status)
            {
                case HaxeCompleteStatus.ERROR:
                    TraceManager.Add(hc.Errors, -3);
                    break;

                case HaxeCompleteStatus.POSITION:
                    if (result == null) return;

                    ASComplete.SaveLastLookupPosition(hc.Sci);

                    PluginBase.MainForm.OpenEditableDocument(result.Path, false);
                    const string keywords = "(function|var|[,(])";

                    ASComplete.LocateMember(keywords, hc.CurrentWord, result.LineStart - 1);
                    break;
            }
        }

        #endregion

        #region command line compiler

        public static string TemporaryOutputFile;

        /// <summary>
        /// Retrieve the context's default compiler path
        /// </summary>
        public override string GetCompilerPath() => GetCurrentSDK()?.Path ?? hxsettings.GetDefaultSDK().Path;

        /// <summary>
        /// Check current file's syntax
        /// </summary>
        public override void CheckSyntax()
        {
            if (hxsettings.CompletionMode == HaxeCompletionModeEnum.FlashDevelop || PluginBase.MainForm.CurrentDocument.IsUntitled) return;

            EventManager.DispatchEvent(this, new NotifyEvent(EventType.ProcessStart));
            var hc = GetHaxeComplete(CurSciControl, new ASExpr(), false, HaxeCompilerService.COMPLETION);
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
                string currentPath = Directory.GetCurrentDirectory();
                string filePath = temporaryPath ?? Path.GetDirectoryName(cFile.FileName);
                filePath = NormalizePath(filePath);
                Directory.SetCurrentDirectory(filePath);
                
                // prepare command
                string command = haxePath;
                if (Path.GetExtension(command) == "") command = Path.Combine(command, "haxe.exe");

                command += ";";
                if (cFile.Package.Length > 0) command += cFile.Package+".";
                string cname = cFile.GetPublicClass().Name;
                if (cname.IndexOf('<') > 0) cname = cname.Substring(0, cname.IndexOf('<'));
                command += cname;

                if (haxeTarget == "flash" && (append == null || !append.Contains("-swf-version")))
                    command += " -swf-version " + majorVersion;
                // classpathes
                string hxPath = PathHelper.ResolvePath(hxsettings.GetDefaultSDK().Path);
                foreach (PathModel aPath in classPath)
                    if (aPath.Path != temporaryPath && !aPath.Path.StartsWith(hxPath, StringComparison.OrdinalIgnoreCase))
                        command += " -cp \"" + aPath.Path.TrimEnd('\\') + "\"";
                command = command.Replace(filePath, "");

                // run
                MainForm.CallCommand("RunProcessCaptured", command + " " + append);
                // restaure current directory
                if (Directory.GetCurrentDirectory() == filePath)
                    Directory.SetCurrentDirectory(currentPath);
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
            if (IsFileValid)
            {
                var cClass = cFile.GetPublicClass();
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
                    for (int i = 0; i < mPar.Count; i++)
                    {
                        var op = mPar[i].Groups["switch"].Value;
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

        #region haxelib

        internal void InstallLibrary(Dictionary<string, string> nameToVersion)
        {
            if (GetCurrentSDK()?.IsHaxeShim ?? false) InstallLixLibrary(nameToVersion);
            else InstallHaxeLibLibrary(nameToVersion);
        }

        internal void InstallHaxeLibLibrary(Dictionary<string, string> nameToVersion)
        {
            var haxePath = PathHelper.ResolvePath(GetCompilerPath());
            if (!Directory.Exists(haxePath) && !File.Exists(haxePath))
            {
                ErrorManager.ShowInfo(TextHelper.GetString("Info.InvalidHaXePath"));
                return;
            }
            if (Directory.Exists(haxePath)) haxePath = Path.Combine(haxePath, "haxelib.exe");

            var cwd = Directory.GetCurrentDirectory();
            nameToVersion.Select(it => $"{haxePath};install {it.Key} {it.Value} -cwd \"{cwd}\"")
                .ToList()
                .ForEach(it => MainForm.CallCommand("RunProcessCaptured", it));
        }

        internal void InstallLixLibrary(Dictionary<string, string> nameToVersion)
        {
            var lixPath = Path.Combine(PathHelper.ResolvePath(GetCompilerPath()), PlatformHelper.IsRunningOnWindows() ? "lix.cmd" : "lix");
            if (!File.Exists(lixPath))
            {
                ErrorManager.ShowInfo(TextHelper.GetString("Info.InvalidHaXePath"));
                return;
            }

            var projectDir = PluginBase.CurrentProject != null ? Path.GetDirectoryName(PluginBase.CurrentProject.ProjectPath) : "";
            foreach (var item in nameToVersion)
            {
                var p = StartHiddenProcess(lixPath, "install haxelib:" + item.Key, projectDir);
                var output = p.StandardOutput.ReadToEnd();
                var error = p.StandardError.ReadToEnd();
                if (output != "") TraceManager.Add(output, (int)TraceType.Info);
                else if (error != "") TraceManager.Add(error, (int)TraceType.Error);
                p.WaitForExit();
                p.Close();
            }
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
