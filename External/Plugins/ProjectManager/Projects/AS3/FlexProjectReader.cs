using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using PluginCore;
using PluginCore.Helpers;

namespace ProjectManager.Projects.AS3
{
    class FlexProjectReader : ProjectReader
    {
        private static Regex reArgs = new Regex(@"\$\{(\w+)\}", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);
        
        private AS3Project project;
        private string mainApp;
        private string outputPath;
        private string fpVersion;
        private PathCollection applications;

        public IDictionary<string, string> EnvironmentPaths { get; set; }

        public FlexProjectReader(string filename)
            : base(filename, new AS3Project(filename))
        {
            this.project = base.Project as AS3Project;
            Directory.SetCurrentDirectory(project.Directory);
        }

        protected override void ProcessRootNode()
        {
            mainApp = GetAttribute("mainApplicationPath");
            fpVersion = GetAttribute("version");
            project.HiddenPaths.Add(".settings");
        }

        protected override void ProcessNode(string name)
        {
            if (NodeType == XmlNodeType.Element)
                switch (name)
                {
                    case "compiler": ReadCompilerOptions(); break;
                    case "applications": ReadApplications(); break;
                    case "modules": ReadModules(); break;
                    case "theme": ReadTheme(); break;
                    case "buildTargets": ReadBuildTargets(); break;
                }
        }

        private void ReadCompilerOptions()
        {
            outputPath = GetAttribute("outputFolderLocation") ?? (GetAttribute("outputFolderPath") ?? "");

            string src = GetAttribute("sourceFolderPath");
            if (src != null) project.Classpaths.Add(src);
            mainApp = (src ?? "") + "/" + mainApp;
            if (mainApp.StartsWith('/')) mainApp = mainApp.Substring(1);
            project.CompileTargets.Add(OSPath(mainApp.Replace('/', '\\')));

            project.TraceEnabled = GetAttribute("enableModuleDebug") == "true";
            project.CompilerOptions.Warnings = GetAttribute("warn") == "true";
            project.CompilerOptions.Strict = GetAttribute("strict") == "true";
            project.CompilerOptions.Accessible = GetAttribute("generateAccessible") == "true";

            string additional = GetAttribute("additionalCompilerArguments") ?? string.Empty;
            List<string> api = new List<string>();
            if (GetAttribute("useApolloConfig") == "true")
            {
                if (additional.Length > 0) additional += "\n";
                project.MovieOptions.Platform = "AIR";
                project.TestMovieBehavior = TestMovieBehavior.Custom;
            }
            else
            {
                project.MovieOptions.Platform = PlatformData.FLASHPLAYER_PLATFORM;
                project.TestMovieBehavior = TestMovieBehavior.Default;
            }
            project.MovieOptions.Version = fpVersion ?? project.MovieOptions.DefaultVersion(project.MovieOptions.Platform);

            if (Path.GetExtension(mainApp).ToLower() == ".mxml")
            {
                int target = 4;
                try
                {
                    string mainFile = ResolvePath(mainApp, project.Directory);
                    if (mainFile != null && File.Exists(mainFile))
                        if (File.ReadAllText(mainFile).IndexOfOrdinal("http://www.adobe.com/2006/mxml") > 0)
                        {
                            target = 3;
                            additional = "-compatibility-version=3.0.0\n" + additional;
                            if (project.MovieOptions.Platform == PlatformData.FLASHPLAYER_PLATFORM)
                                project.MovieOptions.Version = "9.0";
                        }
                }
                catch { }
                api.Add("Library\\AS3\\frameworks\\Flex" + target);
            }
            string[] projectAdditional = project.CompilerOptions.Additional ?? new string[] { };
            string[] fbAdditional = additional.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            if (fbAdditional.Length > 0)
            {
                // TODO: Analyze the additional arguments for better support
                int offset = projectAdditional.Length;
                Array.Resize(ref projectAdditional, projectAdditional.Length + fbAdditional.Length);
                Array.Copy(fbAdditional, 0, projectAdditional, offset, fbAdditional.Length);
            }
            project.CompilerOptions.Additional = projectAdditional;
            if (api.Count > 0) project.CompilerOptions.IntrinsicPaths = api.ToArray();

            while (Read() && Name != "compiler")
                ProcessCompilerOptionNode(Name);
        }

        private void ProcessCompilerOptionNode(string name)
        {
            if (NodeType == XmlNodeType.Element)
                switch (name)
                {
                    case "compilerSourcePath": ReadCompilerSourcePaths(); break;
                    case "libraryPath": ReadLibraryPaths(); break;
                }
        }

        private void ReadCompilerSourcePaths()
        {
            if (!IsEmptyElement)
            {
                ReadStartElement("compilerSourcePath");
                var paths = project.Classpaths;
                while (Name == "compilerSourcePathEntry")
                {
                    string path = OSPath(GetAttribute("path"));
                        
                    string pathTmp = "linked-src" + Path.DirectorySeparatorChar + 
                        path.Substring(path.LastIndexOf(Path.DirectorySeparatorChar) + 1);

                    if (!Directory.Exists(pathTmp) && !File.Exists(pathTmp))
                        pathTmp = reArgs.Replace(path, ReplaceVars);
                    
                    if (pathTmp.Length > 0 && !pathTmp.StartsWith("$"))
                        paths.Add(pathTmp);

                    Read();
                }
            }
        }

        private void ReadLibraryPaths()
        {
            if (!IsStartElement())
                return;
            ReadStartElement("libraryPath");
            LibraryAsset asset;
            bool exclude = false;
            while (Name != "libraryPath")
            {
                switch (Name)
                {
                    case "excludedEntries":
                        exclude = IsStartElement();
                        break;

                    case "libraryPathEntry":
                        string path = GetAttribute("path") ?? string.Empty;
                        if (path.Length == 0) break;
                        path = OSPath(path);

                        string pathTmp = "linked-swc" + Path.DirectorySeparatorChar + 
                            path.Substring(path.LastIndexOf(Path.DirectorySeparatorChar) + 1);

                        if (!Directory.Exists(pathTmp) && !File.Exists(pathTmp))
                            pathTmp = reArgs.Replace(path, ReplaceVars);
                            
                        if (pathTmp.Length > 0 && !pathTmp.StartsWith('$'))
                        {
                            asset = new LibraryAsset(project, pathTmp);
                            if (exclude || GetAttribute("linkType") == "2")
                                asset.SwfMode = SwfAssetMode.ExternalLibrary;
                            else
                                asset.SwfMode = SwfAssetMode.Library;
                            project.SwcLibraries.Add(asset);
                        }
                        break;
                }
                Read();
            }
            project.RebuildCompilerOptions();
        }

        public void ReadApplications()
        {
            ReadStartElement("applications");
            applications = new PathCollection();
            ReadPaths("application", applications);
            if (applications.Count > 0)
            {
                // Why doesn't it use mainApplicationPath?
                project.OutputPath = Path.Combine(outputPath,
                    Path.GetFileNameWithoutExtension(applications[0]) + ".swf");
            }
        }

        private void ReadModules()
        {
            ReadStartElement("modules");
            while (Name == "module")
            {
                string app = GetAttribute("application") ?? "";
                if (app == mainApp)
                {
                    project.OutputPath = Path.Combine(outputPath, GetAttribute("destPath") ?? "");
                }
                Read();
            }
        }

        private void ReadTheme()
        {
            char s = Path.DirectorySeparatorChar;
            string themeLocation = OSPath(GetAttribute("themeLocation"));
            bool isSdk = GetAttribute("themeIsSDK") == "true";
            string themeName = themeLocation.Substring(themeLocation.LastIndexOf(Path.DirectorySeparatorChar) + 1).ToLower();
            string[] tmpPaths;

            if (!isSdk)
            {
                tmpPaths = new[] {".packagedThemes", reArgs.Replace(themeLocation, ReplaceVars)};
            }
            else
            {
                tmpPaths = new [] {reArgs.Replace(themeLocation, ReplaceVars)};
            }

            string themeFile = string.Empty;
            bool themeFound = false;
            foreach (var tmpPath in tmpPaths)
            {
                themeFound = true;
                themeLocation = tmpPath;
                var metaFile = Path.Combine(tmpPath, "metadata.xml");
                if (File.Exists(metaFile))
                {
                    var doc = new XmlDocument();
                    doc.Load(metaFile);

                    themeFile = doc.FirstChild.SelectSingleNode("mainFile").InnerText;
                    if (File.Exists(Path.Combine(tmpPath, themeFile))) break;
                }

                if (File.Exists(Path.Combine(tmpPath, themeFile = themeName + ".swc")) ||
                    File.Exists(Path.Combine(tmpPath, themeFile = themeName + ".css")))
                    break;

                themeFound = false;
            }

            if (!themeFound) return;

            if (!isSdk)
            {
                foreach (var file in Directory.GetFiles(themeLocation, "*.*", SearchOption.AllDirectories))
                {
                    var relativePath = ".packagedThemes" + s + ProjectPaths.GetRelativePath(themeLocation, file);
                    var directory = Path.GetDirectoryName(relativePath);

                    if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

                    File.Copy(file, relativePath);
                }

                themeFile = ".packagedThemes" + s + themeFile;
            }
            else
                themeFile = Path.Combine(themeLocation, themeFile);
            
            string[] additional = project.CompilerOptions.Additional ?? new string[] { };
            Array.Resize(ref additional, additional.Length + 1);

            additional[additional.Length - 1] = "-theme" + (GetAttribute("themeIsDefault") == "false" ? "+=" : "=") + PathHelper.GetShortPathName(themeFile);

            project.CompilerOptions.Additional = additional;

            project.RebuildCompilerOptions();
        }

        private void ReadBuildTargets()
        {
            ReadStartElement("buildTargets");
            while (Name == "buildTarget")
            {
                if (GetAttribute("androidSettingsVersion") != null || GetAttribute("iosSettingsVersion") != null)
                {
                    project.MovieOptions.Platform = "AIR Mobile";
                    project.TestMovieBehavior = TestMovieBehavior.Custom;
                    break;
                }
                Read();
            }
        }

        public static String ResolvePath(String path, String relativeTo)
        {
            if (string.IsNullOrEmpty(path)) return null;
            Boolean isPathNetworked = path.StartsWithOrdinal("\\\\") || path.StartsWithOrdinal("//");
            if (Path.IsPathRooted(path) || isPathNetworked) return path;
            String resolvedPath = Path.Combine(relativeTo, path);
            if (Directory.Exists(resolvedPath) || File.Exists(resolvedPath)) return resolvedPath;
            return null;
        }

        private string ReplaceVars(Match match)
        {
            if (match.Groups.Count > 0)
            {
                string name = match.Groups[1].Value.ToUpperInvariant();
                string value;
                if (EnvironmentPaths != null && EnvironmentPaths.TryGetValue(name, out value))
                    return value;

                switch (name)
                {
                    case "EXTERNAL_THEME_DIR":
                        var path = GetThemeFolderPath();
                        if (path == string.Empty) return match.Value;
                        return path;
                    case "SDK_THEMES_DIR":
                        return PathHelper.ResolvePath(PluginBase.MainForm.ProcessArgString("$(FlexSDK)")) ?? "C:\\flex_sdk";
                    //case "PROJECT_FRAMEWORKS":    // We're leaving this one commented for the moment
                    //    return Path.Combine(PathHelper.ResolvePath(PluginBase.MainForm.ProcessArgString("$(FlexSDK)")) ?? "C:\\flex_sdk", "frameworks");
                }
            }
            return match.Value;
        }

        public static string GetThemeFolderPath()
        {
            char s = Path.DirectorySeparatorChar;
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Win32NT:
                case PlatformID.Win32S:
                case PlatformID.Win32Windows:
                case PlatformID.WinCE:
                    // The path should be the first one by default, but depending of Java versions may be resolved in other ways.
                    // It can be customized as well, in which case it cannot be easily got (we should check FB Java parameters, and then maybe
                    // Eclipse configuration files to get workspaces and their settings if further inspection is needed)
                    // For the second one in this list some Java versions are not even returning the right path... it's using this registry key:
                    // HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders\Desktop
                    // which use is discouraged to use, and other registry key should take precendence, but it's suggested to use the method here
                    string path;
                    if (!Directory.Exists(path = string.Format("{0}{1}Application Data{1}Adobe{1}Flash Builder{1}Themes",
                                      Environment.GetEnvironmentVariable("USERPROFILE"), s)) &&
                        !Directory.Exists(path = string.Format("{0}{1}Application Data{1}Adobe{1}Flash Builder{1}Themes",
                                      Directory.GetParent(Environment.GetFolderPath(Environment.SpecialFolder.Desktop)), s)) &&
                        !Directory.Exists(path = string.Format("{0}{1}Adobe{1}Flash Builder{1}Themes",
                                      Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), s)))

                        return string.Empty;

                    return path;
                    
                default:
                    return string.Format("{0}{1}Library{1}Application Support{1}Adobe{1}Flash Builder{1}Themes",
                                      Environment.GetEnvironmentVariable("HOME"), s);
            }
        }

    }
}
