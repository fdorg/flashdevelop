using System;
using System.Collections.Generic;
using System.IO;
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

        public override string Language { get { return "haxe"; } }
        public override string LanguageDisplayName { get { return "Haxe"; } }
        public override bool IsCompilable { get { return true; } }
        public override bool ReadOnly { get { return false; } }
        public override bool HasLibraries { get { return OutputType == OutputType.Application && IsFlashOutput; } }
        public override bool RequireLibrary { get { return IsFlashOutput; } }
        public override string DefaultSearchFilter { get { return "*.hx;*.hxp"; } }

        public override String LibrarySWFPath
        {
            get
            {
                string projectName = RemoveDiacritics(Name);
                return Path.Combine("obj", projectName + "Resources.swf");
            }
        }

        public string[] RawHXML
        {
            get { return rawHXML; }
            set { ParseHXML(value); }
        }

        public new HaxeOptions CompilerOptions { get { return (HaxeOptions)base.CompilerOptions; } }

        public string HaxeTarget
        {
            get
            {
                if (!MovieOptions.HasPlatformSupport) return null;
                return MovieOptions.PlatformSupport.HaxeTarget;
            }
        }

        public bool IsFlashOutput { get { return HaxeTarget == "swf"; } }

        public override string GetInsertFileText(string inFile, string path, string export, string nodeType)
        {
            bool isInjectionTarget = (UsesInjection && path == GetAbsolutePath(InputPath));
            if (export != null) return export;
            if (IsLibraryAsset(path) && !isInjectionTarget)
                return GetAsset(path).ID;

            String dirName = inFile;
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
                CompilerOptions.MainClass = "";
            }
        }

        private void ClearDocumentClass()
        {
            if (string.IsNullOrEmpty(CompilerOptions.MainClass)) 
                return;

            string docFile = CompilerOptions.MainClass.Replace('.', Path.DirectorySeparatorChar) + ".hx";
            CompilerOptions.MainClass = "";
            foreach (string cp in AbsoluteClasspaths)
                if (File.Exists(Path.Combine(cp, docFile)))
                {
                    SetCompileTarget(Path.Combine(cp, docFile), false);
                    break;
                }
        }

        public override bool Clean()
        {
            try
            {
                if (!string.IsNullOrEmpty(OutputPath) && File.Exists(GetAbsolutePath(OutputPath)))
                {
                    if (MovieOptions.HasPlatformSupport && MovieOptions.PlatformSupport.ExternalToolchain == null)
                        File.Delete(GetAbsolutePath(OutputPath));
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        string Quote(string s)
        {
            if (s.IndexOf(" ") >= 0)
                return "\"" + s + "\"";
            return s;
        }

        public string[] BuildHXML(string[] paths, string outfile, bool release )
        {
            List<String> pr = new List<String>();
            var isFlash = IsFlashOutput;

            if (rawHXML != null)
            {
                pr.AddRange(rawHXML);
            }
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
                foreach (string lib in CompilerOptions.Libraries)
                    if (lib.Length > 0)
                    {
                        if (lib.Trim().StartsWith("-lib")) pr.Add(lib);
                        else pr.Add("-lib " + lib);
                    }

                // class paths
                List<String> classPaths = new List<String>();
                foreach (string cp in paths)
                    classPaths.Add(cp);
                foreach (string cp in this.Classpaths)
                    classPaths.Add(cp);
                foreach (string cp in classPaths)
                {
                    String ccp = String.Join("/", cp.Split('\\'));
                    pr.Add("-cp " + Quote(ccp));
                }

                // compilation mode
                string mode = HaxeTarget;
                //throw new SystemException("Unknown mode");

                if (mode != null)
                {
                    outfile = String.Join("/", outfile.Split('\\'));
                    pr.Add("-" + mode + " " + Quote(outfile));
                }

                // flash options
                if (isFlash)
                {
                    string htmlColor = this.MovieOptions.Background.Substring(1);

                    if (htmlColor.Length > 0)
                        htmlColor = ":" + htmlColor;

                    pr.Add("-swf-header " + string.Format("{0}:{1}:{2}{3}", MovieOptions.Width, MovieOptions.Height, MovieOptions.Fps, htmlColor));

                    if (!UsesInjection && LibraryAssets.Count > 0)
                        pr.Add("-swf-lib " + Quote(LibrarySWFPath));

                    if (CompilerOptions.FlashStrict)
                        pr.Add("--flash-strict");

                    // haxe compiler uses Flash version directly
                    string version = MovieOptions.Version;
                    if (version != null) pr.Add("-swf-version " + version);
                }

                // defines
                foreach (string def in CompilerOptions.Directives)
                    pr.Add("-D " + Quote(def));

                // add project files marked as "always compile"
                foreach (string relTarget in CompileTargets)
                {
                    string absTarget = GetAbsolutePath(relTarget);
                    // guess the class name from the file name
                    foreach (string cp in classPaths)
                        if (absTarget.StartsWith(cp, StringComparison.OrdinalIgnoreCase))
                        {
                            string className = GetClassName(absTarget, cp);
                            if (CompilerOptions.MainClass != className)
                                pr.Add(className);
                        }
                }

                // add main class
                if (!string.IsNullOrEmpty(CompilerOptions.MainClass))
                    pr.Add("-main " + CompilerOptions.MainClass);
                
                // extra options
                foreach (string opt in CompilerOptions.Additional)
                {
                    String p = opt.Trim();
                    if (p == "" || p[0] == '#')
                        continue;
                    char[] space = { ' ' };
                    string[] parts = p.Split(space, 2);
                    if (parts.Length == 1)
                        pr.Add(p);
                    else
                        pr.Add(parts[0] + ' ' + Quote(parts[1]));
                }
            }

            // debug
            if (!release)
            {
                pr.Insert(0, "-debug");
                if (CurrentSDK == null || CurrentSDK.IndexOf("Motion-Twin") < 0) // Haxe 3+
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

        private string GetClassName(string absTarget, string cp)
        {
            string className = absTarget.Substring(cp.Length);
            className = className.Substring(0, className.LastIndexOf('.'));
            className = Regex.Replace(className, "[\\\\/]+", ".");
            if (className.StartsWith(".")) className = className.Substring(1);
            return className;
        }

        #region Load/Save

        public static HaxeProject Load(string path)
        {
            string ext = Path.GetExtension(path).ToLower();
            if (ext == ".hxml")
            {
                HaxeProject hxproj = new HaxeProject(path);
                hxproj.RawHXML = File.ReadAllLines(path);
                return hxproj;
            }

            HaxeProjectReader reader = new HaxeProjectReader(path);

            try
            {
                return reader.ReadProject();
            }
            catch (XmlException exception)
            {
                string format = string.Format("Error in XML Document line {0}, position {1}.",
                    exception.LineNumber, exception.LinePosition);
                throw new Exception(format, exception);
            }
            finally { reader.Close(); }
        }

        public override void Save()
        {
            SaveAs(ProjectPath);
        }

        public override void SaveAs(string fileName)
        {
            string ext = Path.GetExtension(fileName).ToLower();
            if (ext != ".hxproj") return;

            if (!AllowedSaving(fileName)) return;
            try
            {
                HaxeProjectWriter writer = new HaxeProjectWriter(this, fileName);
                writer.WriteProject();
                writer.Flush();
                writer.Close();
                if (saveHXML && OutputType != OutputType.CustomBuild) {
                    StreamWriter hxml = File.CreateText(Path.ChangeExtension(fileName, "hxml"));
                    foreach( string e in BuildHXML(new string[0],this.OutputPath,true) )
                        hxml.WriteLine(e);
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

        private void ParseHXML(string[] raw)
        {
            if (raw != null && (raw.Length == 0 || raw[0] == null))
                raw = null;
            rawHXML = raw;

            Regex reHxOp = new Regex("-([a-z0-9-]+)\\s*(.*)", RegexOptions.IgnoreCase);
            List<string> libs = new List<string>();
            List<string> defs = new List<string>();
            List<string> cps = new List<string>();
            List<string> add = new List<string>();
            string target = PlatformData.JAVASCRIPT_PLATFORM;
            string haxeTarget = "js";
            string output = "";
            if (raw != null)
            foreach(string line in raw)
            {
                if (line == null) break;
                Match m = reHxOp.Match(line.Trim());
                if (m.Success)
                {
                    string op = m.Groups[1].Value;
                    if (op == "-next")
                        break; // ignore the rest

                    string value = m.Groups[2].Value.Trim();
                    switch (op)
                    {
                        case "D": defs.Add(value); break;
                        case "cp": cps.Add(CleanPath(value)); break;
                        case "lib": libs.Add(value); break;
                        case "main": CompilerOptions.MainClass = value; break;
                        case "swf":
                        case "swf9": 
                            target = PlatformData.FLASHPLAYER_PLATFORM;
                            haxeTarget = "flash";
                            output = value; 
                            break;
                        case "swf-header":
                            var header = value.Split(':');
                            int.TryParse(header[0], out MovieOptions.Width);
                            int.TryParse(header[1], out MovieOptions.Height);
                            int.TryParse(header[2], out MovieOptions.Fps);
                            MovieOptions.Background = header[3];
                            break;
                        case "-connect": break; // ignore
                        case "-each": break; // ignore
                        default:
                            // detect platform (-cpp output, -js output, ...)
                            var targetPlatform = FindPlatform(op);
                            if (targetPlatform != null)
                            {
                                target = targetPlatform.Name;
                                haxeTarget = targetPlatform.HaxeTarget;
                                output = value;
                            }
                            else add.Add(line); 
                            break;
                    }
                }
            }
            CompilerOptions.Directives = defs.ToArray();
            CompilerOptions.Libraries = libs.ToArray();
            CompilerOptions.Additional = add.ToArray();
            if (cps.Count == 0) cps.Add(".");
            Classpaths.Clear();
            Classpaths.AddRange(cps);

            if (MovieOptions.HasPlatformSupport)
            {
                var platform = MovieOptions.PlatformSupport;
                MovieOptions.TargetBuildTypes = platform.Targets;

                if (platform.Name == "hxml" && string.IsNullOrEmpty(TargetBuild))
                    TargetBuild = haxeTarget ?? "";
            }
            else MovieOptions.TargetBuildTypes = null;

            if (MovieOptions.TargetBuildTypes == null)
            {
                OutputPath = output;
                OutputType = OutputType.Application;
                MovieOptions.Platform = target;
            }
        }

        private LanguagePlatform FindPlatform(string op)
        {
            var lang = PlatformData.SupportedLanguages["haxe"];
            foreach (var platform in lang.Platforms.Values)
            {
                if (platform.HaxeTarget == op) return platform;
            }
            return null;
        }

        private string CleanPath(string path)
        {
            path = path.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar).TrimEnd(Path.DirectorySeparatorChar);
            // handle if NME/OpenFL config file is not at the root of the project directory
            if (Path.IsPathRooted(path)) return path;
            
            var relDir = Path.GetDirectoryName(ProjectPath);
            var absPath = Path.GetFullPath(Path.Combine(relDir, path));
            return GetRelativePath(absPath);
        }
        #endregion
    }
}
