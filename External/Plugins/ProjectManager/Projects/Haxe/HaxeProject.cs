using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using PluginCore;
using PluginCore.Helpers;

namespace ProjectManager.Projects.Haxe
{
    public class HaxeProject : Project
    {
        // hack : we cannot reference settings HaxeProject is also used by FDBuild
        public static bool saveHXML = false;

        protected string[] rawHXML;

        public HaxeProject(string path) : base(path, new HaxeOptions())
        {
            movieOptions = new HaxeMovieOptions();
        }

        public override string Language => "haxe";
        public override string LanguageDisplayName => "Haxe";
        public override bool IsCompilable => true;
        public override bool ReadOnly => IsFolderProject();
        public override bool HasLibraries => OutputType == OutputType.Application && IsFlashOutput;
        public override bool RequireLibrary => IsFlashOutput;
        public override string DefaultSearchFilter => "*.hx;*.hxp";

        public override string LibrarySWFPath
        {
            get
            {
                var projectName = RemoveDiacritics(Name);
                return Path.Combine("obj", projectName + "Resources.swf");
            }
        }

        public string[] RawHXML
        {
            get => rawHXML;
            set => ParseHxml(value);
        }

        public new HaxeOptions CompilerOptions => (HaxeOptions)base.CompilerOptions;

        public string HaxeTarget => MovieOptions.HasPlatformSupport ? MovieOptions.PlatformSupport.HaxeTarget : null;

        public bool IsFlashOutput => HaxeTarget == "swf";

        public override string GetInsertFileText(string inFile, string path, string export, string nodeType)
        {
            if (export != null) return export;
            var isInjectionTarget = (UsesInjection && path == GetAbsolutePath(InputPath));
            if (IsLibraryAsset(path) && !isInjectionTarget)
                return GetAsset(path).ID;

            var dirName = inFile;
            if (FileInspector.IsHaxeFile(inFile, Path.GetExtension(inFile).ToLower()))
                dirName = ProjectPath;

            return '"' + ProjectPaths.GetRelativePath(Path.GetDirectoryName(dirName), path).Replace('\\', '/') + '"'; 
        }

        public override CompileTargetType AllowCompileTarget(string path, bool isDirectory)
        {
            if (isDirectory || !FileInspector.IsHaxeFile(path, Path.GetExtension(path))) return CompileTargetType.None;

            foreach (string cp in AbsoluteClasspaths)
                if (path.StartsWith(cp, StringComparison.OrdinalIgnoreCase))
                    return CompileTargetType.AlwaysCompile | CompileTargetType.DocumentClass;
            return CompileTargetType.None;
        }

        public override bool IsDocumentClass(string path) 
        {
            foreach (string cp in AbsoluteClasspaths)
                if (path.StartsWith(cp, StringComparison.OrdinalIgnoreCase))
                {
                    string cname = GetClassName(path, cp);
                    if (CompilerOptions.MainClass == cname) return true;
                }
            return false;
        }

        public override void SetDocumentClass(string path, bool isMain)
        {
            if (isMain)
            {
                ClearDocumentClass();
                if (!IsCompileTarget(path)) SetCompileTarget(path, true);
                foreach (string cp in AbsoluteClasspaths)
                    if (path.StartsWith(cp, StringComparison.OrdinalIgnoreCase))
                    {
                        CompilerOptions.MainClass = GetClassName(path, cp);
                        break;
                    }
            }
            else 
            {
                SetCompileTarget(path, false);
                CompilerOptions.MainClass = string.Empty;
            }
        }

        void ClearDocumentClass()
        {
            if (string.IsNullOrEmpty(CompilerOptions.MainClass)) return;
            var docFile = CompilerOptions.MainClass.Replace('.', Path.DirectorySeparatorChar) + ".hx";
            CompilerOptions.MainClass = string.Empty;
            foreach (var cp in AbsoluteClasspaths)
            {
                var path = Path.Combine(cp, docFile);
                if (File.Exists(path))
                {
                    SetCompileTarget(path, false);
                    break;
                }
            }
        }

        public override bool Clean()
        {
            try
            {
                if (!string.IsNullOrEmpty(OutputPath) && File.Exists(GetAbsolutePath(OutputPath)))
                {
                    if (MovieOptions.HasPlatformSupport && MovieOptions.PlatformSupport.ExternalToolchain is null)
                        File.Delete(GetAbsolutePath(OutputPath));
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        static string Quote(string s)
        {
            if (s.IndexOf(' ') >= 0)
                return "\"" + s + "\"";
            return s;
        }

        public string[] BuildHXML(string[] paths, string outfile, bool release)
        {
            var pr = new List<string>();
            var isFlash = IsFlashOutput;

            if (rawHXML != null) pr.AddRange(rawHXML);
            else
            {
                // SWC libraries
                if (isFlash)
                    foreach (LibraryAsset asset in LibraryAssets)
                    {
                        if (asset.IsSwc)
                            pr.Add("-swf-lib " + asset.Path);
                    }

                // libraries
                foreach (var lib in CompilerOptions.Libraries)
                    if (lib.Length > 0)
                    {
                        if (lib.Trim().StartsWith("-lib", StringComparison.Ordinal)) pr.Add(lib);
                        else pr.Add("-lib " + lib);
                    }

                // class paths
                var classPaths = paths.ToList();
                classPaths.AddRange(Classpaths);
                foreach (var cp in classPaths)
                {
                    var ccp = string.Join("/", cp.Split('\\'));
                    pr.Add("-cp " + Quote(ccp));
                }

                // compilation mode
                var mode = HaxeTarget;
                //throw new SystemException("Unknown mode");

                if (mode != null)
                {
                    outfile = string.Join("/", outfile.Split('\\'));
                    pr.Add("-" + mode + " " + Quote(outfile));
                }

                // flash options
                if (isFlash)
                {
                    var htmlColor = MovieOptions.Background.Substring(1);
                    if (htmlColor.Length > 0)
                        htmlColor = ":" + htmlColor;

                    pr.Add($"-swf-header {MovieOptions.Width}:{MovieOptions.Height}:{MovieOptions.Fps}{htmlColor}");

                    if (!UsesInjection && LibraryAssets.Count > 0)
                        pr.Add("-swf-lib " + Quote(LibrarySWFPath));

                    if (CompilerOptions.FlashStrict)
                        pr.Add("--flash-strict");

                    // haxe compiler uses Flash version directly
                    var version = MovieOptions.Version;
                    if (version != null) pr.Add("-swf-version " + version);
                }

                // defines
                foreach (var def in CompilerOptions.Directives)
                    pr.Add("-D " + Quote(def));

                // add project files marked as "always compile"
                foreach (var relTarget in CompileTargets)
                {
                    var absTarget = GetAbsolutePath(relTarget);
                    // guess the class name from the file name
                    foreach (var cp in classPaths)
                        if (absTarget.StartsWith(cp, StringComparison.OrdinalIgnoreCase))
                        {
                            var className = GetClassName(absTarget, cp);
                            if (CompilerOptions.MainClass != className)
                                pr.Add(className);
                        }
                }

                // add main class
                if (!string.IsNullOrEmpty(CompilerOptions.MainClass))
                    pr.Add("-main " + CompilerOptions.MainClass);

                // extra options
                char[] space = { ' ' };
                foreach (var opt in CompilerOptions.Additional)
                {
                    var p = opt.Trim();
                    if (p.Length == 0 || p[0] == '#') continue;
                    var parts = p.Split(space, 2);
                    if (parts.Length == 1) pr.Add(p);
                    else pr.Add(parts[0] + ' ' + Quote(parts[1]));
                }
            }

            // debug
            if (!release)
            {
                pr.Insert(0, "-debug");
                if (CurrentSDK is null || !CurrentSDK.Contains("Motion-Twin")) // Haxe 3+
                    pr.Insert(1, "--each");
                if (isFlash && EnableInteractiveDebugger && CompilerOptions.EnableDebug)
                {
                    pr.Insert(1, "-D fdb");
                    if (CompilerOptions.NoInlineOnDebug)
                        pr.Insert(2, "--no-inline");
                }
            }
            return pr.ToArray();
        }

        static string GetClassName(string absTarget, string cp)
        {
            var className = absTarget.Substring(cp.Length);
            className = className.Substring(0, className.LastIndexOf('.'));
            className = Regex.Replace(className, "[\\\\/]+", ".");
            if (className.StartsWith(".")) className = className.Substring(1);
            return className;
        }

        #region Load/Save

        public static HaxeProject Load(string path)
        {
            var ext = Path.GetExtension(path).ToLower();
            if (ext == ".hxml")
            {
                return new HaxeProject(path) {RawHXML = File.ReadAllLines(path)};
            }

            var reader = new HaxeProjectReader(path);
            try
            {
                return reader.ReadProject();
            }
            catch (XmlException e)
            {
                var format = $"Error in XML Document line {e.LineNumber}, position {e.LinePosition}.";
                throw new Exception(format, e);
            }
            finally { reader.Close(); }
        }

        public override void Save() => SaveAs(ProjectPath);

        public override void SaveAs(string fileName)
        {
            var ext = Path.GetExtension(fileName).ToLower();
            if (ext != ".hxproj") return;

            if (!AllowedSaving(fileName)) return;
            try
            {
                var writer = new HaxeProjectWriter(this, fileName);
                writer.WriteProject();
                writer.Flush();
                writer.Close();
                if (saveHXML && OutputType != OutputType.CustomBuild)
                {
                    var hxml = File.CreateText(Path.ChangeExtension(fileName, "hxml"));
                    foreach(var line in BuildHXML(new string[0], OutputPath,true))
                        hxml.WriteLine(line);
                    hxml.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "IO Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region HXML parsing

        void ParseHxml(string[] lines)
        {
            if (lines != null && (lines.Length == 0 || lines[0] is null)) lines = null;
            rawHXML = lines;
            var entries = new HxmlEntries();
            if (lines != null) ParseHxmlEntries(lines, entries);

            CompilerOptions.Directives = entries.defs.ToArray();
            CompilerOptions.Libraries = entries.libs.ToArray();
            CompilerOptions.Additional = entries.add.ToArray();
            Classpaths.Clear();
            if (entries.cps.Count == 0) Classpaths.Add(".");
            else Classpaths.AddRange(entries.cps);
            if (MovieOptions.HasPlatformSupport)
            {
                var platform = MovieOptions.PlatformSupport;
                MovieOptions.TargetBuildTypes = platform.Targets;

                if (platform.Name == "hxml" && string.IsNullOrEmpty(TargetBuild))
                    TargetBuild = entries.haxeTarget ?? string.Empty;
            }
            else MovieOptions.TargetBuildTypes = null;
            if (MovieOptions.TargetBuildTypes is null)
            {
                OutputPath = entries.output;
                OutputType = OutputType.Application;
                MovieOptions.Platform = entries.target;
            }
        }

        void ParseHxmlEntries(string[] lines, HxmlEntries entries)
        {
            var reHxOp = new Regex("^-([a-z0-9-]+)\\s*(.*)", RegexOptions.IgnoreCase);
            for (var i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                if (line is null) break;
                var trimmedLine = line.Trim();
                var m = reHxOp.Match(trimmedLine);
                if (m.Success)
                {
                    var op = m.Groups[1].Value;
                    if (op == "-next") break; // ignore the rest
                    var value = m.Groups[2].Value.Trim();
                    switch (op)
                    {
                        // Haxe 3
                        case "D":
                        // Haxe 4
                        case "-define":
                            entries.defs.Add(value);
                            break;
                        // Haxe 3
                        case "cp":
                        // Haxe 4
                        case "p":
                        case "-class-path":
                            entries.cps.Add(CleanPath(value, entries.cwd));
                            break;
                        // Haxe 3
                        case "lib":
                        // Haxe 4
                        case "L":
                        case "-library":
                            entries.libs.Add(value);
                            break;
                        // Haxe 3
                        case "main":
                        // Haxe 4
                        case "m":
                        case "-main":
                            CompilerOptions.MainClass = value;
                            break;
                        // Haxe 3
                        case "swf":
                        case "swf9":
                        // Haxe 4
                        case "-swf":
                            entries.target = PlatformData.FLASHPLAYER_PLATFORM;
                            entries.haxeTarget = "flash";
                            entries.output = value;
                            break;
                        // Haxe 3
                        case "swf-header":
                        // Haxe 4
                        case "-swf-header":
                            var header = value.Split(':');
                            int.TryParse(header[0], out MovieOptions.Width);
                            int.TryParse(header[1], out MovieOptions.Height);
                            int.TryParse(header[2], out MovieOptions.Fps);
                            MovieOptions.Background = header[3];
                            break;
                        case "-connect": break; // ignore
                        case "-each": break; // ignore
                        case "-cwd":
                            entries.cwd = CleanPath(value, entries.cwd);
                            break;
                        // Haxe 3
                        case "as3":
                        // Haxe 4
                        case "-as3":
                            entries.target = PlatformData.CUSTOM_PLATFORM;
                            entries.haxeTarget = "flash";
                            entries.output = "as3";
                            break;
                        default:
                            // detect platform (-cpp output, -js output, ...)
                            var targetPlatform = FindPlatform(op);
                            if (targetPlatform != null)
                            {
                                entries.target = targetPlatform.Name;
                                entries.haxeTarget = targetPlatform.HaxeTarget;
                                entries.output = value;
                            }
                            else entries.add.Add(line);
                            break;
                    }
                }
                else if (!trimmedLine.StartsWith("#") && trimmedLine.EndsWith(".hxml", StringComparison.OrdinalIgnoreCase))
                {
                    var subhxml = GetAbsolutePath(CleanPath(trimmedLine, entries.cwd));
                    if (entries.Dependencies.Contains(subhxml))
                    {
                        // Cyclic dependency
                        // TODO slavara: print error
                    }
                    else if (File.Exists(subhxml))
                    {
                        entries.Dependencies.Add(subhxml);
                        ParseHxmlEntries(File.ReadAllLines(subhxml), entries);
                    }
                }
            }
        }

        static LanguagePlatform FindPlatform(string target) => PlatformData.SupportedLanguages["haxe"].Platforms.Values.FirstOrDefault(it => it.HaxeTarget == target);

        string CleanPath(string path, string cwd)
        {
            path = path.Replace('"', ' ');
            path = path.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar).TrimEnd(Path.DirectorySeparatorChar);
            // handle if NME/OpenFL config file is not at the root of the project directory
            if (Path.IsPathRooted(path)) return path;
            
            var relDir = Path.GetDirectoryName(ProjectPath);
            var absPath = Path.GetFullPath(Path.Combine(relDir, cwd, path));
            return GetRelativePath(absPath);
        }
        #endregion

        class HxmlEntries
        {
            public readonly HashSet<string> Dependencies = new HashSet<string>();
            public readonly List<string> defs = new List<string>();
            public readonly List<string> cps = new List<string>();
            public readonly List<string> libs = new List<string>();
            public readonly List<string> add = new List<string>();
            public string target = PlatformData.JAVASCRIPT_PLATFORM;
            public string haxeTarget = "js";
            public string output = string.Empty;
            public string cwd = ".";
        }
    }
}