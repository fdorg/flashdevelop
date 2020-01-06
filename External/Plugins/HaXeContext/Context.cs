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
using PluginCore.Collections;
using PluginCore.Utilities;
using ScintillaNet;

namespace HaXeContext
{
    public class Context : AS2Context.Context
    {
        #region initialization
        protected new static readonly Regex re_CMD_BuildCommand =
            new Regex("@haxe[\\s]+(?<params>.*)", RegexOptions.Compiled | RegexOptions.Multiline);

        protected static readonly Regex re_genericType =
            new Regex("(?<gen>[^<]+)<(?<type>.+)>", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        protected static readonly Regex re_Template =
            new Regex("<(?<name>[a-z]+)>", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static string FLASH_OLD = "flash";
        public static string FLASH_NEW = "flash9";
        static string currentEnv;
        static string currentSDK;

        readonly HaXeSettings haxeSettings;
        readonly Func<string, InstalledSDK> getCustomSDK;
        Dictionary<string, List<string>> haxelibsCache;
        string haxeTarget;
        bool resolvingDot;
        bool resolvingFunction;
        HaxeCompletionCache hxCompletionCache;

        internal static readonly ClassModel StubFunctionClass = new ClassModel
        {
            Name = "Function",
            Type = "Function",
            Flags = FlagType.Class,
            Access = Visibility.Public,
            InFile = new FileModel {Package = "haxe", Module = "Constraints"},
        };

        internal static readonly MemberModel StubSafeCastFunction = new MemberModel("cast", "TResult", FlagType.Dynamic | FlagType.Function, 0)
        {
            Template = "<TResult>",
            Parameters = new List<MemberModel>
            {
                new MemberModel("expression", "Dynamic", FlagType.Variable | FlagType.ParameterVar, 0),
                new MemberModel("type", "Class<TResult>", FlagType.Variable | FlagType.ParameterVar, 0),
            },
            Comments = "\r\t * Attempts to convert an <b>expression</b> to a given <b>type</b>. If the conversion is not possible a run-time error is generated." +
                       "\r\t * @param expression The expression for convert to a given type." +
                       "\r\t * @param type The type of the result." +
                       "\r\t * @return a value of type."
        };

        internal static readonly MemberModel StubUnsafeCastFunction = new MemberModel("cast expr", null, FlagType.Declaration, 0)
        {
            Comments = "\r\t * Unsafe casts are useful to subvert the type system. The compiler types <b>expr</b> as usual and then wraps it in a monomorph. This allows the expression to be assigned to anything." +
                       "\r\t * Unsafe casts do not introduce any dynamic types, as the following example shows:" +
                       "\r\t * " +
                       "\r\t * var i = 1;" +
                       "\r\t * $type(i); // Int" +
                       "\r\t * var s = cast i;" +
                       "\r\t * $type(s); // Unknown<0>" +
                       "\r\t * Std.parseInt(s);" +
                       "\r\t * $type(s); // String" +
                       "\r\t * " +
                       "\r\t * Variable <b>i</b> is typed as <b>Int</b> and then assigned to variable <b>s</b> using the unsafe cast <b>cast i</b>. This causes s to be of an unknown type, a monomorph. Following the usual rules of unification, it can then be bound to any type, such as <b>String</b> in this example." +
                       "\r\t * These casts are called <i>unsafe</i> because the runtime behavior for invalid casts is not defined. While most dynamic targets are likely to work, it might lead to undefined errors on static targets." +
                       "\r\t * Unsafe casts have little to no runtime overhead."
        };

        internal static readonly MemberModel StubStringCodeProperty = new MemberModel("code", "Int", FlagType.Getter, Visibility.Public)
        {
            Comments = "The character code of this character(inlined at compile-time)"
        };

        public Context(HaXeSettings initSettings) : this(initSettings, path => null)
        {
        }
        
        public Context(HaXeSettings initSettings, Func<string, InstalledSDK> getCustomSDK)
        {
            haxeSettings = initSettings;
            haxeSettings.Init();
            this.getCustomSDK = getCustomSDK;

            /* AS-LIKE OPTIONS */

            hasLevels = false;
            docType = "Void"; // "flash.display.MovieClip";

            StubFunctionClass.InFile.Classes.Add(StubFunctionClass);

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
            features.HasMultilineString = true;
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
            features.typesPreKeys = new[] {features.importKey, features.importKeyAlt, features.ConstructorKey, features.ExtendsKey, features.ImplementsKey};
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
            features.BooleanOperators = new[] {"<", ">", "&&", "||", "!=", "==", "!"};
            features.TernaryOperators = new[] {"?", ":"};
            /* INITIALIZATION */

            settings = initSettings;

            currentSDK = PathHelper.ResolvePath(haxeSettings.GetDefaultSDK().Path) ?? "";
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

        List<string> LookupLibrary(string lib)
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

        List<string> LookupHaxeLibLibrary(string lib)
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

            var paths = new List<string>();
            do
            {
                var line = p.StandardOutput.ReadLine();
                if (string.IsNullOrEmpty(line)) continue;
                if (line.Contains("not installed"))
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
                    catch
                    {
                        // ignored
                    }
                }
            }
            while (!p.StandardOutput.EndOfStream);

            p.WaitForExit();
            p.Close();

            if (paths.Count == 0) return null;
            haxelibsCache.Add(lib, paths);
            return paths;
        }

        List<string> LookupLixLibrary(string lib)
        {
            var haxePath = PathHelper.ResolvePath(GetCompilerPath());
            if (Directory.Exists(haxePath))
            {
                var path = haxePath;
                haxePath = Path.Combine(path, "haxe.exe");
                if (!File.Exists(haxePath)) haxePath = Path.Combine(path, PlatformHelper.IsRunningOnWindows() ? "haxe.cmd" : "haxe");
            }
            if (!File.Exists(haxePath))
            {
                ErrorManager.ShowInfo(TextHelper.GetString("Info.InvalidHaXePath"));
                return null;
            }

            var projectDir = PluginBase.CurrentProject != null ? Path.GetDirectoryName(PluginBase.CurrentProject.ProjectPath) : "";
            var p = StartHiddenProcess(haxePath, "--run resolve-args -lib " + lib, projectDir);

            var paths = new List<string>();
            var isPathExpected = false;
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
                    catch { }
                }
                isPathExpected = line == "-cp";
            }
            while (!p.StandardOutput.EndOfStream);

            var error = p.StandardError.ReadToEnd();
            if (error.Length != 0) TraceManager.Add(error, (int)TraceType.Error);

            p.WaitForExit();
            p.Close();

            return paths.Count > 0 ? paths : null;
        }

        Process StartHiddenProcess(string fileName, string arguments, string workingDirectory = "")
        {
            string hxPath = currentSDK;
            if (hxPath != null && Path.IsPathRooted(hxPath))
            {
                if (hxPath != currentEnv) SetHaxeEnvironment(hxPath);
                fileName = Path.Combine(hxPath, fileName);
                if (File.Exists(fileName + ".exe")) fileName += ".exe";
            }

            var pi = new ProcessStartInfo();
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
            if (PluginBase.CurrentProject is HaxeProject project) project.UpdateVars(false);
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

            features.SpecialPostfixOperators = GetCurrentSDKVersion() >= "3.3.0"
                ? new[] {'!'}
                : EmptyArray<char>.Instance;

            UseGenericsShortNotationChange();
        }

        void UseGenericsShortNotationChange()
        {
            // We may want to create 2 different feature flags for this, but atm it's enough this way
            features.HasGenericsShortNotation = GetCurrentSDKVersion() >= "3" && haxeSettings.UseGenericsShortNotation;
        }

        public void LoadMetadata()
        {
            features.metadata = new Dictionary<string, string>();

            var processInfo = CreateHaxeProcessInfo("--help-metas");
            if (processInfo is null) return;
            using var process = new Process {StartInfo = processInfo, EnableRaisingEvents = true};
            process.Start();

            var metaList = process.StandardOutput.ReadToEnd();

            var regex = new Regex("@:([a-zA-Z]*)(?: : )(.*?)(?=( @:[a-zA-Z]* :|$))");
            metaList = Regex.Replace(metaList, "\\s+", " ");

            var matches = regex.Matches(metaList);

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
            if (contextSetup is null)
            {
                contextSetup = new ContextSetupInfos();
                contextSetup.Classpath = new[] { Environment.CurrentDirectory };
                contextSetup.Lang = settings.LanguageId;
                contextSetup.Platform = "Flash Player";
                contextSetup.Version = "10.0";
            }

            // external version definition
            platform = contextSetup.Platform;
            majorVersion = haxeSettings.DefaultFlashVersion;
            minorVersion = 0;
            ParseVersion(contextSetup.Version, ref majorVersion, ref minorVersion);

            // NOTE: version > 10 for non-Flash platforms
            var lang = GetHaxeTarget(platform);
            if (lang is null)
            {
                if (contextSetup.Platform == "hxml" || contextSetup.Platform.EndsWith("multitarget", StringComparison.OrdinalIgnoreCase))
                    lang = contextSetup.TargetBuild ?? "";
                else // assume game-related toolchain
                {
                    lang = "cpp";
                    if (contextSetup.TargetBuild is null || contextSetup.TargetBuild.StartsWithOrdinal("flash")) lang = "";
                    else if (contextSetup.TargetBuild.StartsWithOrdinal("html5")) lang = "js";
                    else if (contextSetup.TargetBuild.Contains("neko")) lang = "neko";
                }
            }
            else if (lang == "swf") lang = "flash";
            features.Directives = new List<string> {lang};
            haxeTarget = lang;

            //
            // Class pathes
            //
            classPath = new List<PathModel>();
            // Haxe std
            var project = PluginBase.CurrentProject as HaxeProject;
            var hxPath = project != null ? project.CurrentSDK : PathHelper.ResolvePath(haxeSettings.GetDefaultSDK().Path);
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
                        var hide = new List<string>();
                        foreach (var dir in Directory.GetDirectories(haxeCP))
                            if (Path.GetFileName(dir) is { } dirName
                                && dirName != "sys"
                                && dirName != "haxe"
                                && dirName != "libs")
                            {
                                hide.Add(dirName);
                            }
                        ManualExploration(std, hide);
                    }
                    AddPath(std);

                    if (!string.IsNullOrEmpty(lang))
                    {
                        var specific = PathModel.GetModel(Path.Combine(haxeCP, lang), this);
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
            if (project != null && haxeTarget == "flash")
            {
                if (majorVersion < 9)
                {
                    // for example: HXML MultipTarget
                    var value = project.CompilerOptions.Additional.FirstOrDefault(it => it.StartsWithOrdinal("-swf-version "));
                    if (value != null) ParseVersion(value.Substring("-swf-version ".Length), ref majorVersion, ref minorVersion);
                }
                if (majorVersion >= 9)
                {
                    foreach(LibraryAsset asset in project.LibraryAssets)
                        if (asset.IsSwc)
                        {
                            var path = project.GetAbsolutePath(asset.Path);
                            if (File.Exists(path)) AddPath(path);
                        }
                    foreach(var line in project.CompilerOptions.Additional)
                        if (line.StartsWithOrdinal("-swf-lib ")) {
                            var path = project.GetAbsolutePath(line.Substring(9/*"-swf-lib ".Length*/));
                            if (File.Exists(path)) AddPath(path);
                        }
                        else if (line.StartsWithOrdinal("-swf-lib-extern "))
                        {
                            var path = project.GetAbsolutePath(line.Substring(16/*"-swf-lib-extern ".Length*/));
                            if (File.Exists(path)) AddPath(path);
                        }
                }
            }

            // add haxe libraries
            if (project != null)
            {
                var libraries = project.CompilerOptions.Libraries.ToList();
                foreach (var it in project.CompilerOptions.Additional)
                {
                    if (it.Contains("-lib ", out var index)) libraries.Add(it.Substring(index + "-lib ".Length).Trim());
                }
                foreach (var library in libraries)
                    if (!string.IsNullOrEmpty(library.Trim()))
                    {
                        var libPaths = LookupLibrary(library);
                        if (libPaths is null) continue;
                        foreach (var path in libPaths)
                        {
                            var libPath = AddPath(path);
                            if (libPath != null) AppendPath(contextSetup, libPath.Path);
                        }
                    }
            }

            // add external pathes
            var initCP = classPath;
            classPath = new List<PathModel>();
            if (contextSetup.Classpath != null)
            {
                foreach (string cpath in contextSetup.Classpath)
                    AddPath(cpath.Trim());
            }

            // add user pathes from settings
            if (!settings.UserClasspath.IsNullOrEmpty())
            {
                foreach (string cpath in settings.UserClasspath) AddPath(cpath.Trim());
            }
            // add initial pathes
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

            resolvingDot = false;
            resolvingFunction = false;
            if (completionModeHandler is null) 
                OnCompletionModeChange();
        }

        public string GetHaxeTarget(string platformName)
        {
            if (!PlatformData.SupportedLanguages.ContainsKey("haxe")) return null;
            var haxeLang = PlatformData.SupportedLanguages["haxe"];
            if (haxeLang is null) return null;
            foreach (var platform in haxeLang.Platforms.Values)
                if (platform.Name == platformName) return platform.HaxeTarget;
            return null;
        }

        static void AppendPath(ContextSetupInfos contextSetup, string path)
        {
            foreach(string cp in contextSetup.Classpath) 
                if (path.Equals(cp, StringComparison.OrdinalIgnoreCase))
                    return;
            if (contextSetup.AdditionalPaths is null) contextSetup.AdditionalPaths = new List<string>();
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
                var haxelib = Path.Combine(path.Path, "haxelib.json");
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
                        var src = File.ReadAllText(haxelib);
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
                    var parser = new SwfOp.ContentParser(path.Path);
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
            if (path is null || !path.StartsWith(pathModel.Path, StringComparison.OrdinalIgnoreCase)) return false;
            var package = path.Length <= pathModel.Path.Length ? "" : path.Substring(pathModel.Path.Length + 1).Replace('/', '.').Replace('\\', '.');
            return (aFile.Package == package);
        }

        /// <inheritdoc />
        public override FileModel GetCodeModel(FileModel result, string src, bool scriptMode)
        {
            result.haXe = true;
            base.GetCodeModel(result, src, scriptMode);
            // members
            {
                foreach (var member in result.Members)
                {
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
            // classes
            {
                var isHaxe4 = GetCurrentSDKVersion() >= "4.0.0";
                for (var i = 0; i < result.Classes.Count; i++)
                {
                    var @class = result.Classes[i];
                    var flags = @class.Flags;
                    if ((flags & FlagType.Abstract) != 0)
                    {
                        var meta = @class.MetaDatas;
                        if (meta is null || meta.All(it => it.Name != ":enum")) continue;
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
                            member.Flags = FlagType.Enum | FlagType.Static | (FlagType) HaxeFlagType.Inline | FlagType.Variable;
                            member.Access = Visibility.Public;
                            if (string.IsNullOrEmpty(member.Type)) member.Type = @class.Type;
                            member.InFile = @class.InFile;
                            if (isHaxe4 && member.Value is null)
                            {
                                @class.ResolveExtends();
                                var extends = @class;
                                while (!extends.IsVoid())
                                {
                                    switch (extends.Name)
                                    {
                                        case "String":
                                            /**
                                             * for example:
                                             * transform
                                             * enum abstract AString(String) {
                                             *     var A;
                                             * }
                                             * to
                                             * enum abstract AString(String) {
                                             *     var A = "A";
                                             * }
                                             */
                                            member.Value = $"\"{member.Name}\"";
                                            extends = ClassModel.VoidClass;
                                            break;
                                        case "Int":
                                            /**
                                             * for example:
                                             * transform
                                             * enum abstract AString(Int) {
                                             *     var A;
                                             *     var B;
                                             *     var C = 5;
                                             *     var D;
                                             * }
                                             * to
                                             * enum abstract AString(Int) {
                                             *     var A = 0;
                                             *     var B = 1;
                                             *     var C = 5;
                                             *     var D = 6;
                                             * }
                                             */
                                            var prevIndex = index;
                                            while (--prevIndex >= 0)
                                            {
                                                var prevMember = @class.Members[prevIndex];
                                                if (!prevMember.Flags.HasFlag(FlagType.Variable)) continue;
                                                if (int.TryParse(prevMember.Value, out var value))
                                                    member.Value = (value + 1).ToString();
                                                break;
                                            }
                                            if (member.Value is null) member.Value = "0";
                                            extends = ClassModel.VoidClass;
                                            break;
                                    }
                                    extends = extends.Extends;
                                }
                            }
                        }
                    }
                    else if ((flags & FlagType.Enum) != 0) @class.ExtendsType = "EnumValue";
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

        InstalledSDK GetCurrentSDK() => (context.Settings ?? settings).InstalledSDKs?.FirstOrDefault(sdk => sdk.Path == currentSDK) ?? getCustomSDK(currentSDK);

        public SemVer GetCurrentSDKVersion() => GetCurrentSDK() is { } sdk ? new SemVer(sdk.Version) : SemVer.Zero;

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
                if (tmp == withClass) return Visibility.Public | Visibility.Private;
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

            var result = new MemberList();
            MemberModel item;
            // public & internal classes
            var package = CurrentModel?.Package;
            foreach (var aPath in classPath)
                if (aPath.IsValid && !aPath.Updating)
                {
                    aPath.ForeachFile(aFile =>
                    {
                        var module = aFile.Module;
                        var needModule = true;
                        if (aFile.Classes.Count > 0 && !aFile.Classes[0].IsVoid())
                            foreach (var aClass in aFile.Classes)
                            {
                                if (aClass.IndexType is null
                                    && (aClass.Access == Visibility.Public
                                        || (aClass.Access == Visibility.Internal && aClass.InFile.Package == package)))
                                {
                                    if (aClass.Name == module) needModule = false;
                                    item = aClass.ToMemberModel();
                                    //if (tpackage != package) 
                                    if (item.Type != null) item.Name = item.Type;
                                    result.Add(item);
                                }
                            }
                        // HX files correspond to a "module" which should appear in code completion
                        // (you don't import classes defined in modules but the module itself)
                        if (needModule && aFile.FullPackage is { } qmodule)
                        {
                            item = new MemberModel(qmodule, qmodule, FlagType.Class | FlagType.Module, Visibility.Public);
                            result.Add(item);
                        }
                        return true;
                    });
                }
            // display imported classes and classes declared in imported modules
            var imports = ResolveImports(cFile);
            const FlagType mask = FlagType.Class | FlagType.Enum;
            foreach (var import in imports)
            {
                if ((import.Flags & mask) > 0)
                {
                    result.Add(import);
                }
            }

            if (cFile != null)
                foreach(var aClass in cFile.Classes)
                    result.Add(aClass.ToMemberModel());

            // in cache
            result.Sort();
            completionCache.AllTypes = result;
            return result;
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
                if (insert is null) return false;
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
            if (inFile is null) return imports;
            foreach (var item in inFile.Imports)
            {
                if (item.Name != "*") ResolveImport(item, imports);
                else
                {
                    var package = item.Type.Substring(0, item.Type.Length - 2);
                    ResolveImports(package, imports);
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
                        if (import is null) continue;
                        var p1 = import.LastIndexOf('.');
                        if (p1 == -1) continue;
                        var lpart = import.Substring(0, p1);
                        var p2 = lpart.LastIndexOf('.');
                        if (p2 != -1) lpart = import.Substring(p2 + 1);
                        if (char.IsLower(lpart[0])) continue;
                        var type = ResolveType(lpart, Context.CurrentModel);
                        if (type.IsVoid() || type.Members.Count == 0) continue;
                        var rpart = import.Substring(p1 + 1);
                        var member = type.Members.Search(rpart, FlagType.Static, Visibility.Public);
                        if (member is null) continue;
                        member = (MemberModel) member.Clone();
                        member.InFile = type.InFile;
                        imports[i] = member;
                    }
                }
            }
            return imports;
        }

        void ResolveImports(string package, MemberList result)
        {
            var matches = ResolvePackage(package, false);
            if (matches != null)
            {
                result.Add(matches.Imports);
                result.Add(matches.Members);
            }
            else
            {
                var model = ResolveType(package, null);
                if (model.IsVoid()) return;
                var access = TypesAffinity(model, Context.CurrentClass);
                foreach (MemberModel member in model.Members)
                {
                    if ((member.Flags & FlagType.Static) > 0 && (member.Access & access) != 0)
                    {
                        member.InFile = model.InFile;
                        result.Add(member);
                    }
                }
            }
        }

        void ResolveImport(MemberModel item, MemberList imports)
        {
            if (settings.LazyClasspathExploration)
            {
                imports.Add(item);
                return;
            }
            // HX files are "modules": when imported all the classes contained are available
            var fileName = item.Type.Replace(".", dirSeparator) + ".hx";
            if (fileName.StartsWithOrdinal("flash" + dirSeparator))
            {
                if (haxeTarget != "flash" || majorVersion > 8) // flash9 remap
                    fileName = FLASH_NEW + fileName.Substring(5);
                else
                    fileName = FLASH_OLD + fileName.Substring(5);
            }
            var isUsing = item.Flags.HasFlag(FlagType.Using);
            var matched = false;
            foreach (var aPath in classPath)
                if (aPath.IsValid && !aPath.Updating)
                {
                    var path = aPath.Path + dirSeparator + fileName;

                    // cached file
                    if (!aPath.TryGetFile(path, out var file)) continue;
                    if (file.Context != this)
                    {
                        // not associated with this context -> refresh
                        file.OutOfDate = true;
                        file.Context = this;
                    }

                    // add all public classes of Haxe modules
                    foreach (var @class in file.Classes)
                    {
                        var c = @class;
                        if (c.IndexType is null && c.Access == Visibility.Public)
                        {
                            if (isUsing)
                            {
                                c = (ClassModel) c.Clone();
                                c.Flags |= FlagType.Using;
                            }
                            imports.Add(c);
                        }
                    }
                    matched = true;
                }

            if (!matched) imports.Add(item);
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
            if (member.InFile?.FullPackage == CurrentModel.FullPackage) return true;
            var name = member.Name;
            if (name.Contains('#', out var p)) name = name.Substring(0, p);
            var type = member.Type;
            var isShortType = name == type;
            var curFile = Context.CurrentModel;
            var imports = curFile.Imports.Concat(ResolveDefaults(curFile.Package)).ToArray();
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
            if (string.IsNullOrEmpty(cname) || cname == features.voidKey || classPath is null)
                return ClassModel.VoidClass;
            // for example: {x:Int}
            if (cname.StartsWith('{'))
            {
                var genCount = 0;
                var isEmpty = true;
                // transform {x:Int} to class AnonymousType { public var x:Int; }
                var sb = new StringBuilder("class AnonymousStructure {public var ");
                for (var i = 1; i < cname.Length - 1; i++)
                {
                    var c = cname[i];
                    sb.Append(c);
                    if (c == '<')
                    {
                        genCount++;
                        isEmpty = false;
                    }
                    else if (c == '>') genCount--;
                    else if (c == ',' && genCount == 0)
                    {
                        sb.Append(" public var");
                        isEmpty = false;
                    }
                    else if (char.IsLetterOrDigit(c)) isEmpty = false;
                }
                if (isEmpty) return ResolveType(features.dynamicKey, null);
                sb.Append('}');
                var model = GetCodeModel(sb.ToString());
                var result = model.Classes.First();
                result.Type = cname;
                result.Flags = FlagType.Struct;
                return result;
            }
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
                var found = false;
                if (inFile != null)
                {
                    // search in file
                    foreach (var aClass in inFile.Classes)
                        if (aClass.Name == cname)
                            return aClass;

                    // search in imported classes
                    var imports = ResolveImports(inFile);
                    foreach (var import in imports)
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
                }
                if (!found && cname == "Function") return StubFunctionClass;
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
                    return ResolveType(features.dynamicKey, null);
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
                if (token.Contains(' ', out var index))
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
                foreach (var aClass in inFile.Classes)
                    if (aClass.Name == cname && (package == "" || package == inFile.Package))
                        return aClass;
            }

            // search in classpath
            return GetModel(package, cname, inPackage);
        }

        /// <summary>
        /// Retrieve/build typed copies of generic types
        /// </summary>
        ClassModel ResolveGenericType(string baseType, string indexType, FileModel inFile)
        {
            var aClass = ResolveType(baseType, inFile);
            if (aClass.IsVoid()) return aClass;

            if (aClass.QualifiedName == features.dynamicKey)
            {
                var result = MakeCustomObjectClass(aClass, indexType);
                result.Template = aClass.Template;
                result.Type = aClass.Type;
                return result;
            }

            var aFile = aClass.InFile;
            // is the type already cloned?
            foreach (var otherClass in aFile.Classes)
                if (otherClass.IndexType == indexType && otherClass.BaseType == baseType)
                    return otherClass;

            // resolve T
            string Tname = "T";
            var m = re_Template.Match(aClass.Type);
            if (m.Success)
            {
                Tname = m.Groups[1].Value;
            }
            var reReplaceType = new Regex("\\b" + Tname + "\\b");

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

            foreach (var member in aClass.Members)
            {
                if (member.Type != null && member.Type.Contains(Tname))
                {
                    member.Type = reReplaceType.Replace(member.Type, indexType);
                }
                if (member.Parameters != null)
                {
                    foreach (var param in member.Parameters)
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
            var filename = "toplevel.hx";
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
                var tlClass = topLevel.GetPublicClass();
                if (!tlClass.IsVoid())
                {
                    topLevel.Members = tlClass.Members;
                    tlClass.Members = null;
                    topLevel.Classes = new List<ClassModel>();
                }
            }
            // not found

            topLevel.Members.Add(new MemberModel(features.ThisKey, "", FlagType.Variable, Visibility.Public));
            topLevel.Members.Add(new MemberModel(features.BaseKey, "", FlagType.Variable, Visibility.Public));
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
        MemberList ResolveDefaults(string package)
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
                    if (!it.TryGetFile(path, out var model)) continue;
                    foreach (var import in model.Imports)
                    {
                        // for example: using package.Type;
                        if ((import.Flags & FlagType.Using) != 0)
                        {
                            /**
                             * The static extension keyword using implies the effect of import.
                             * https://haxe.org/manual/type-system-import.html
                             */
                            it.ForeachFile(file =>
                            {
                                if (file.FullPackage != import.Type) return true;
                                foreach (var @class in file.Classes)
                                {
                                    var type = (ClassModel) @class.Clone();
                                    type.Flags |= FlagType.Using;
                                    result.Merge(type);
                                }
                                return false;
                            });
                        }
                        // for example: import package.Type;
                        else if (import.Name != "*") result.Add(import);
                        // for example: import package.*;
                        else ResolveImports(import.Type.Substring(0, import.Type.Length - 2), result);
                    }
                    break;
                }
                if (string.IsNullOrEmpty(packagePath)) break;
                packagePath = Path.GetDirectoryName(packagePath);
            }
            return result;
        }

        /// <summary>
        /// https://haxe.org/manual/lf-static-extension.html
        /// </summary>
        /// <param name="type">Type for which need to find extension methods</param>
        /// <param name="inFile">Current file</param>
        /// <param name="result">A new ClassModel with extension methods or the original ClassModel if no extension are found</param>
        /// <returns>True if extension are found</returns>
        internal static bool TryResolveStaticExtensions(ClassModel type, FileModel inFile, out ClassModel result)
        {
            result = type;
            var imports = Context.ResolveImports(inFile);
            if ((type.Flags & FlagType.Enum) != 0 && (type.Flags & FlagType.Abstract) == 0
                && Context.ResolveType("haxe.EnumTools.EnumValueTools", null) is { } @using && !@using.IsVoid())
            {
                @using = (ClassModel)@using.Clone();
                @using.Flags |= FlagType.Using;
                imports.Add(@using);
            }
            if (imports.Count == 0) return false;
            var extensions = new MemberList();
            for (var i = imports.Count - 1; i >= 0; i--)
            {
                {
                    if (imports[i] is { } import && !(import is ClassModel) && (import.Flags & FlagType.Using) != 0)
                    {
                        imports[i] = Context.ResolveType(import.Type, Context.CurrentModel);
                        imports[i].Flags |= FlagType.Using;
                    }
                }
                {
                    if (imports[i] is ClassModel import && (import.Flags & FlagType.Using) != 0 && import.Members.Count > 0)
                    {
                        var access = Context.TypesAffinity(type, import);
                        var extends = type;
                        extends.ResolveExtends();
                        while (!extends.IsVoid())
                        {
                            foreach (var member in import.Members)
                            {
                                if ((member.Access & access) == 0
                                    || (member.Flags & FlagType.Static) == 0 || (member.Flags & FlagType.Function) == 0
                                    || member.Parameters.IsNullOrEmpty()
                                    || extensions.Contains(member.Name, 0, 0)
                                    || !CanBeExtended(extends, member, access)) continue;
                                // transform `extensionMethod(target:Type, ...params)` to `extensionMethod(...params)`
                                var extension = (MemberModel)member.Clone();
                                extension.Parameters.RemoveAt(0);
                                extension.Flags = FlagType.Dynamic | FlagType.Function | FlagType.Using;
                                extension.InFile = import.InFile;
                                extensions.Add(extension);
                            }
                            extends = extends.Extends;
                        }
                    }
                }
            }
            if (extensions.Count == 0) return false;
            result = (ClassModel) type.Clone();
            result.Members.Merge(extensions);
            return true;
            // Utils
            bool CanBeExtended(ClassModel target, MemberModel extension, Visibility access)
            {
                var firstParamType = extension.Parameters[0].Type;
                if (string.IsNullOrEmpty(firstParamType)) return false;
                if (!string.IsNullOrEmpty(extension.Template) && Context.ResolveType(firstParamType, null).IsVoid())
                {
                    // transform methodName<T:ConcreteType>(a:T, b) to methodName<T:ConcreteType>(a:ConcreteType, b)
                    var template = extension.Template.Substring(1, extension.Template.Length - 2);
                    var parts = Context is Context ctx && ctx.GetCurrentSDKVersion() >= "4.0.0"
                        ? template.Split(new[] {'&'}, StringSplitOptions.RemoveEmptyEntries)
                        : template.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var part in parts)
                    {
                        if (!part.Contains(':')) continue;
                        var templateToType = part.Split(new[] {':'}, StringSplitOptions.RemoveEmptyEntries);
                        if (templateToType[0] != firstParamType) continue;
                        if (templateToType.Length == 2) firstParamType = templateToType[1];
                        break;
                    }
                }
                if ((target.Flags & FlagType.Enum) != 0 && (target.Flags & FlagType.Abstract) == 0 && firstParamType == "EnumValue")
                {
                }
                else if (firstParamType != "Dynamic" && !firstParamType.StartsWithOrdinal("Dynamic<"))
                {
                    var targetType = type.Type;
                    var index = targetType.IndexOf('<');
                    if (index != -1) targetType = targetType.Remove(index);
                    index = firstParamType.IndexOf('<');
                    if (index != -1) firstParamType = firstParamType.Remove(index);
                    if (firstParamType != targetType)
                    {
                        /**
                         * for example:
                         * typedef Typedef = {
                         *     x:Int,
                         *     y:Int,
                         * }
                         *
                         * class Point {
                         *     var x:Int = 0;
                         *     var x:Int = 0;
                         * }
                         *
                         * ...
                         * using Example;
                         * class Example {
                         *     static function extensionMethod(to:Typedef, ...args) {}
                         *
                         *     function f(p:Point) {
                         *         p.extensionMethod();
                         *     }
                         * }
                         */
                        var paramType = Context.ResolveType(firstParamType, null);
                        if (!paramType.Flags.HasFlag(FlagType.TypeDef)) return false;
                        foreach (var typedefMember in paramType.Members)
                        {
                            if (!type.Members.Contains(typedefMember.Name, typedefMember.Flags, 0)) return false;
                        }
                    }
                }
                while (!target.IsVoid())
                {
                    var model = target.Members.Search(extension.Name, 0, access);
                    if (model != null && (model.Flags & FlagType.Static) == 0) return false;
                    target = target.Extends;
                }
                return true;
            }
        }

        public override string GetDefaultValue(string type)
        {
            if (string.IsNullOrEmpty(type) || type == features.voidKey) return null;
            return type switch
            {
                "Int" => "0",
                "UInt" => "0",
                "Float" => "Math.NaN",
                "Bool" => "false",
                _ => "null",
            };
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
                        else if (c == ')')
                        {
                            if (i > pos) result.Add(type.Substring(pos, i - pos));
                            pos = i + 1;
                        }
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
                                && !type.Contains('{', pos)
                                && !type.Contains('<', pos)
                                && !type.Contains(',', pos)
                                && !type.Contains('-', pos))
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
        void OnCompletionModeChange()
        {
            if (completionModeHandler != null)
            {
                completionModeHandler.Stop();
                completionModeHandler = null;
            }

            // fix environment for command line tools
            SetHaxeEnvironment(currentSDK);

            // configure completion provider
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

        /// <summary>
        /// Gets the needed information to create a haxe.exe process with the given arguments.
        /// </summary>
        internal ProcessStartInfo CreateHaxeProcessInfo(string args)
        {
            // compiler path
            var hxPath = currentSDK ?? ""; 
            var process = Path.Combine(hxPath, "haxe.exe");
            if (!File.Exists(process) && (GetCurrentSDK()?.IsHaxeShim ?? false))
                process = Path.Combine(hxPath, PlatformHelper.IsRunningOnWindows() ? "haxe.cmd" : "haxe");
            if (!File.Exists(process)) return null;

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
            if (resolvingDot || haxeSettings.CompletionMode == HaxeCompletionModeEnum.FlashDevelop
                || PluginBase.MainForm.CurrentDocument.IsUntitled)
                return null;
            if (autoHide && !haxeSettings.DisableCompletionOnDemand) return null;
            var exprValue = expression.Value;
            // auto-started completion, can be ignored for performance (show default completion tooltip)
            if (!exprValue.Contains('.') || (autoHide && !exprValue.EndsWith('.')))
                if (haxeSettings.DisableMixedCompletion && exprValue.Length > 0 && autoHide) return new MemberList();
                else return null;

            // empty expression
            if (exprValue.Length != 0)
            {
                // async processing
                var hc = GetHaxeComplete(sci, expression, autoHide, HaxeCompilerService.COMPLETION);
                hc.GetList(OnDotCompletionResult);
                resolvingDot = true;
            }
            return haxeSettings.DisableMixedCompletion ? new MemberList() : null;
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
                    if (!result.Members.IsNullOrEmpty())
                        ASComplete.DotContextResolved(hc.Sci, hc.Expr, result.Members, hc.AutoHide);
                    break;

                case HaxeCompleteStatus.TYPE:
                    // eg. Int
                    break;
            }
        }

        public override void ResolveDotContext(ScintillaControl sci, ASResult expression, MemberList result)
        {
            if (expression.IsStatic && expression.Type is { } type)
            {
                // Attempt to add callback `new` into `result`
                if (type.Flags == FlagType.Class)
                {
                    var member = type.Members.Search(type.Name, FlagType.Constructor, 0);
                    if (member is null)
                    {
                        type.ResolveExtends();
                        while (!(type = type.Extends).IsVoid())
                        {
                            member = type.Members.Search(type.Name, FlagType.Constructor, 0);
                            if (member != null) break;
                        }
                    }
                    if (member != null && (member.Access & TypesAffinity(Context.CurrentClass, type)) != 0)
                    {
                        member = (MemberModel) member.Clone();
                        member.Name = "new";
                        result.Add(member);
                    }
                }
                if (type.InFile is { } file && file.Classes.Count > 1 && type == GetPublicClass(file))
                {
                    // add sub-types
                    foreach (var it in file.Classes)
                    {
                        if (it != type) result.Add(it);
                    }
                    return;
                }
            }
            var exprValue = expression.Context.Value;
            // for example: '1'.<complete> or '\n'.<complete>
            if (exprValue.Length >= 3)
            {
                var first = exprValue[0];
                if ((first == '"' || first == '\'') && expression.Context.SubExpressions != null && expression.Context.SubExpressions.Count == 1)
                {
                    var s = exprValue.Replace(".#0~.", string.Empty);
                    if (s.Length == 3 || (s.Length == 4 && s[1] == '\\'))
                        result.Add(StubStringCodeProperty);
                }
            }
        }

        static ClassModel GetPublicClass(FileModel file)
        {
            if (file?.Classes != null)
            {
                var module = file.Module.IsNullOrEmpty() ? Path.GetFileNameWithoutExtension(file.FileName) : file.Module;
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
            if (topLevel is null) return hxCompletionCache.OtherElements;
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
            if (!list.IsNullOrEmpty())
            {
                var items = list.MultipleSearch(token, 0, 0);
                if (items.Count == 1)
                {
                    var item = items[0];
                    result.RelClass = result.InClass = Context.CurrentClass ?? ClassModel.VoidClass;
                    result.InFile = item.InFile;
                    result.Member = item;
                    result.Type = ResolveType(item.Type, item.InFile);
                    result.IsStatic = false;
                    result.IsPackage = false;
                }
                return;
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
                    foreach(var decl in baseElements.Members)
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
                            foreach (var member in packageElements.Imports)
                            {
                                if (member.Flags != FlagType.Package && member.Type.LastIndexOf('.') == pLen)
                                {
                                    //if (qualify) member.Name = member.Type;
                                    elements.Add(member);
                                }
                            }
                            foreach (var member in packageElements.Members)
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
                foreach (var import in imports)
                {
                    TryAddEnums(import as ClassModel ?? ResolveType(import.Name, cFile), other);
                }
                // in cache
                elements.Sort();
                other.Sort();
                completionCache = hxCompletionCache = new HaxeCompletionCache(this, elements, other);

                // known classes colorization
                if (!CommonSettings.DisableKnownTypesColoring && !settings.LazyClasspathExploration && PluginBase.MainForm.CurrentDocument?.SciControl is { } sci)
                {
                    try
                    {
                        sci.KeyWords(1, completionCache.Keywords); // additional-keywords index = 1
                        sci.Colourise(0, -1); // re-colorize the editor
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
            if (model is null || model.IsVoid()) return;
            if (model.Flags.HasFlag(FlagType.Enum))
            {
                for (var i = 0; i < model.Members.Count; i++)
                {
                    var member = model.Members[i];
                    if (member.Type is null || model.InFile is null)
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
                if (meta is null || meta.All(it => it.Name != ":enum")) return;
                foreach (MemberModel member in model.Members)
                {
                    if (member.Flags.HasFlag(FlagType.Variable)) result.Add(member);
                }
            }
        }

        /// <inheritdoc />
        public override MemberModel ResolveFunctionContext(ScintillaControl sci, ASExpr expression, bool autoHide)
        {
            if (resolvingFunction || haxeSettings.CompletionMode == HaxeCompletionModeEnum.FlashDevelop
                || PluginBase.MainForm.CurrentDocument.IsUntitled)
                return null;

            if (autoHide && !haxeSettings.DisableCompletionOnDemand)
                return null;

            // Do not show error
            var value = expression.Value;
            if (value == "for"
                || value == "while"
                || value == "if"
                || value == "switch"
                || value == "function"
                || value == "catch"
                || value == "trace")
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
            var position = ASComplete.ExpressionEndPosition(sci, expression.Position);
            if (position != expression.Position) expression = ASComplete.GetExpressionType(sci, position, false, true).Context;
            if (classPath != null && expression.Value is { } fullPackage)
            {
                foreach (var pathModel in classPath)
                {
                    if (!pathModel.IsValid || pathModel.Updating) continue;
                    var isFound = false;
                    pathModel.ForeachFile(model =>
                    {
                        if (model.FullPackage != fullPackage) return true;
                        PluginBase.MainForm.OpenEditableDocument(model.FileName);
                        isFound = true;
                        return false;
                    });
                    if (isFound) return true;
                }
            }
            if (haxeSettings.CompletionMode == HaxeCompletionModeEnum.FlashDevelop || GetCurrentSDKVersion() < "3.2.0")
                return false;
            var hc = GetHaxeComplete(sci, expression, false, HaxeCompilerService.POSITION);
            hc.GetPosition(OnPositionResult);
            return true;
        }

        internal void OnPositionResult(HaxeComplete hc, HaxePositionResult result, HaxeCompleteStatus status)
        {
            if (hc.Sci.InvokeRequired) hc.Sci.BeginInvoke((MethodInvoker) (() => HandlePositionResult(hc, result, status)));
            else HandlePositionResult(hc, result, status); 
        }

        static void HandlePositionResult(HaxeComplete hc, HaxePositionResult result, HaxeCompleteStatus status)
        {
            switch (status)
            {
                case HaxeCompleteStatus.ERROR:
                    TraceManager.Add(hc.Errors, -3);
                    break;

                case HaxeCompleteStatus.POSITION:
                    if (result is null || string.IsNullOrEmpty(result.Path)) return;

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

        /// <inheritdoc />
        public override string GetCompilerPath() => GetCurrentSDK()?.Path ?? haxeSettings.GetDefaultSDK().Path;

        /// <summary>
        /// Check current file's syntax
        /// </summary>
        public override void CheckSyntax()
        {
            if (haxeSettings.CompletionMode == HaxeCompletionModeEnum.FlashDevelop || PluginBase.MainForm.CurrentDocument.IsUntitled) return;

            EventManager.DispatchEvent(this, new NotifyEvent(EventType.ProcessStart));
            var hc = GetHaxeComplete(PluginBase.MainForm.CurrentDocument.SciControl, new ASExpr(), false, HaxeCompilerService.COMPLETION);
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
            if (!IsFileValid || !File.Exists(CurrentFile)) return;
            var haxePath = PathHelper.ResolvePath(haxeSettings.GetDefaultSDK().Path);
            if (!Directory.Exists(haxePath) && !File.Exists(haxePath))
            {
                ErrorManager.ShowInfo(TextHelper.GetString("Info.InvalidHaXePath"));
                return;
            }

            SetStatusText(settings.CheckSyntaxRunning);

            try
            {
                // save modified files if needed
                if (outputFile is null) PluginBase.MainForm.CallCommand("SaveAllModified", ".hx");
                else PluginBase.MainForm.CallCommand("SaveAllModified", null);

                // change current directory
                var currentPath = Directory.GetCurrentDirectory();
                var filePath = temporaryPath ?? Path.GetDirectoryName(cFile.FileName);
                filePath = NormalizePath(filePath);
                Directory.SetCurrentDirectory(filePath);
                
                // prepare command
                var command = haxePath;
                if (Path.GetExtension(command) == "") command = Path.Combine(command, "haxe.exe");

                command += ";";
                if (cFile.Package.Length > 0) command += cFile.Package + ".";
                var cname = cFile.GetPublicClass().Name;
                if (cname.Contains('<', out var p) && p > 0) cname = cname.Substring(0, p);
                command += cname;

                if (haxeTarget == "flash" && (append is null || !append.Contains("-swf-version")))
                    command += " -swf-version " + majorVersion;
                // classpathes
                string hxPath = PathHelper.ResolvePath(haxeSettings.GetDefaultSDK().Path);
                foreach (PathModel aPath in classPath)
                    if (aPath.Path != temporaryPath && !aPath.Path.StartsWith(hxPath, StringComparison.OrdinalIgnoreCase))
                        command += " -cp \"" + aPath.Path.TrimEnd('\\') + "\"";
                command = command.Replace(filePath, "");

                // run
                PluginBase.MainForm.CallCommand("RunProcessCaptured", command + " " + append);
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
            if (!File.Exists(CurrentFile)) return false;
            // check if @haxe is defined
            Match mCmd = null;
            if (IsFileValid)
            {
                if (cFile.Comments != null) mCmd = re_CMD_BuildCommand.Match(cFile.Comments);
                if ((mCmd is null || !mCmd.Success) && cFile.GetPublicClass() is { } cClass && cClass.Comments != null)
                    mCmd = re_CMD_BuildCommand.Match(cClass.Comments);
            }

            if (mCmd is null || !mCmd.Success)
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
                command = " " + PluginBase.MainForm.ProcessArgString(command) + " ";
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
                        if ((op == "-swf") && (outputFile is null) && (mPlayIndex < 0))
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
                .ForEach(it => PluginBase.MainForm.CallCommand("RunProcessCaptured", it));
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
                if (output.Length != 0) TraceManager.Add(output, (int)TraceType.Info);
                else if (error.Length != 0) TraceManager.Add(error, (int)TraceType.Error);
                p.WaitForExit();
                p.Close();
            }
        }

        #endregion

        #region Custom behavior of Scintilla

        /// <inheritdoc cref="ASContext.OnBraceMatch"/>
        public override void OnBraceMatch(ScintillaControl sci)
        {
            if (!sci.IsBraceMatching || sci.SelText.Length != 0) return;
            var position = sci.CurrentPos - 1;
            var c = (char)sci.CharAt(position);
            if (c != '<' && c != '>')
            {
                position = sci.CurrentPos;
                c = (char)sci.CharAt(position);
            }
            if (c == '<' || c == '>')
            {
                if (!sci.PositionIsOnComment(position))
                {
                    var bracePosStart = position;
                    var bracePosEnd = BraceMatch(sci, position);
                    if (bracePosEnd != -1) sci.BraceHighlight(bracePosStart, bracePosEnd);
                    if (sci.UseHighlightGuides)
                    {
                        var line = sci.LineFromPosition(position);
                        sci.HighlightGuide = sci.GetLineIndentation(line);
                    }
                }
                else
                {
                    sci.BraceHighlight(-1, -1);
                    sci.HighlightGuide = 0;
                }
            }
        }

        /// <summary>
        /// Find the position of a matching '<' and '>' or INVALID_POSITION if no match.
        /// </summary>
        protected internal int BraceMatch(ScintillaControl sci, int position)
        {
            if (sci.PositionIsOnComment(position) || sci.PositionIsInString(position)) return -1;
            var language = ScintillaControl.Configuration.GetLanguage(sci.ConfigurationLanguage);
            if (language is null) return -1;
            var characters = language.characterclass.Characters;
            var sub = 0;
            var genCount = 0;
            var parCount = 0;
            switch (sci.CharAt(position))
            {
                case '<':
                    var length = sci.TextLength;
                    if (position == length) return -1;
                    switch (sci.CharAt(position + 1))
                    {
                        case '=': // a $(EntryPoint)<= b
                        case '<': // a $(EntryPoint)<< b
                            return -1;
                    }
                    // a <$(EntryPoint)< b
                    if (sci.CharAt(position - 1) == '<') return -1;
                    while (position < length)
                    {
                        position++;
                        if (sci.PositionIsOnComment(position)) continue;
                        var c = sci.CharAt(position);
                        if (c == ';') return -1;
                        if (c <= ' ') continue;
                        if (c == '<') sub++;
                        else if (c == '>')
                        {
                            // TParameter-$(EntryPoint)>TReturn
                            if (sci.CharAt(position - 1) == '-') continue;
                            sub--;
                            if (sub < 0) return position;
                        }
                        // Map<Int$(EntryPoint), String>
                        else if (c == ',') continue;
                        // Array<Int->$(EntryPoint)?String>
                        else if (c == '?') continue;
                        else if (c == '-'
                            && position < length
                            // $(EntryPoint)->
                            && sci.CharAt(position + 1) == '>')
                            position++;
                        else if (// a < b $(EntryPoint)||
                                 c == '|'      
                                 // a < b $(EntryPoint)&&
                                 || c == '&'
                                 // a <$(EntryPoint)= b
                                 || c == '='
                                 // a < b$(EntryPoint);
                                 || c == ';')
                            return -1;
                        // Array<Int->$(EntryPoint){v:Type}>
                        else if (parCount == 0 && c == '{') genCount++;
                        else if (parCount == 0 && c == '}' && genCount > 0) genCount--;
                        // Array<$(EntryPoint)(Int->{v:Type})>
                        else if (genCount == 0 && c == '(') parCount++;
                        else if (genCount == 0 && c == ')' && parCount > 0) parCount--;
                        else if (genCount == 0 && parCount == 0 && !characters.Contains((char)c)) return -1;
                    }
                    break;
                case '>':
                    // -$(EntryPoint)>
                    if (sci.CharAt(position - 1) == '-') return -1;
                    // $(EntryPoint)>=
                    if (sci.CharAt(position + 1) == '=') return -1; 
                    while (position > 0)
                    {
                        position--;
                        if (sci.PositionIsOnComment(position)) continue;
                        var c = sci.CharAt(position);
                        if (c == ';') return -1;
                        if (c <= ' ') continue;
                        if (c == '>')
                        {
                            if (// TParameter->$(EntryPoint)TReturn
                                sci.CharAt(position - 1) == '-')
                            {
                                position--;
                                continue;
                            }
                            sub++;
                        }
                        else if (c == '<')
                        {
                            sub--;
                            if (sub < 0) return position;
                        }
                        // Map<Int,$(EntryPoint) String>
                        else if (c == ',') continue;
                        // Array<Int->$(EntryPoint)?String>
                        else if (c == '?') continue;
                        // $(EntryPoint)->
                        else if (c == '-' && sci.CharAt(position + 1) == '>') {}
                        // Array<Int->{v:Type}$(EntryPoint)>
                        else if (parCount == 0 && c == '}') genCount++;
                        else if (parCount == 0 && c == '{' && genCount > 0) genCount--;
                        // Array<(Int->{v:Type})$(EntryPoint)>
                        else if (genCount == 0 && c == ')') parCount++;
                        else if (genCount == 0 && c == '(' && parCount > 0) parCount--;
                        else if (genCount == 0 && parCount == 0 && !characters.Contains((char)c)) return -1;
                    }
                    break;
            }
            return -1;
        }

        #endregion
    }

    class HaxeCompletionCache: CompletionCache
    {
        public readonly MemberList OtherElements;

        public HaxeCompletionCache(IASContext context, MemberList elements, MemberList otherElements)
            : base(context, elements)
        {
            OtherElements = otherElements;
        }
    }
}