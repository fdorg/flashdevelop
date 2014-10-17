using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Runtime.InteropServices;
using PluginCore.Helpers;

namespace ProjectManager.Projects.AS3
{
    class FlexProjectReader : ProjectReader
    {
        AS3Project project;
        string mainApp;
        string outputPath;
        string fpVersion;
        PathCollection applications;

        public IDictionary<string, string> EnvironmentPaths { get; set; }

        public FlexProjectReader(string filename)
            : base(filename, new AS3Project(filename))
        {
            this.project = base.Project as AS3Project;
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
            if (mainApp.StartsWith("/")) mainApp = mainApp.Substring(1);
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
                project.MovieOptions.Platform = PluginCore.PlatformData.FLASHPLAYER_PLATFORM;
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
                        if (File.ReadAllText(mainFile).IndexOf("http://www.adobe.com/2006/mxml") > 0)
                        {
                            target = 3;
                            additional = "-compatibility-version=3.0.0\n" + additional;
                            if (project.MovieOptions.Platform == PluginCore.PlatformData.FLASHPLAYER_PLATFORM)
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
                ReadPaths("compilerSourcePathEntry", project.Classpaths);
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
                        string path = GetAttribute("path") ?? "";

                        if (path.StartsWith("$") && EnvironmentPaths != null)
                        {
                            string value;
                            string environmentPath = path.Substring(2, path.IndexOf('}') - 2);
                            if (EnvironmentPaths.TryGetValue(environmentPath, out value))
                                path = path.Replace("${" + environmentPath + "}", value);
                        }

                        if (path.Length > 0 && !path.StartsWith("$"))
                        {
                            asset = new LibraryAsset(project, path.Replace('/', '\\'));
                            if (exclude || GetAttribute("linkType").ToString() == "2")
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
            PathCollection targets = new PathCollection();
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
            string themeLocation = GetAttribute("themeLocation").ToString().Replace("${EXTERNAL_THEME_DIR}/",
                GetThemeFolderPath() + s);

            string themeName = themeLocation.Substring(themeLocation.LastIndexOf('\\') + 1).ToLower();
            string themeFile = ".packagedThemes" + s + themeName;
            string themeSwc = themeFile + ".swc";
            string themeCss = themeFile + ".css";

            if (!File.Exists(Path.Combine(project.Directory, (themeFile = themeSwc))) && 
                !File.Exists(Path.Combine(project.Directory, (themeFile = themeCss))))
            {
                if (!File.Exists(Path.Combine(themeLocation, (themeFile = themeName + ".swc"))) &&
                    !File.Exists(Path.Combine(themeLocation, (themeFile = themeName + ".css")))) return;

                foreach (var file in Directory.GetFiles(themeLocation, "*.*", SearchOption.TopDirectoryOnly))
                {
                    var relativePath = ".packagedThemes" + s + ProjectPaths.GetRelativePath(themeLocation, file);
                    var directory = Path.GetDirectoryName(relativePath);

                    if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

                    File.Copy(file, relativePath);
                }

                themeFile = ".packagedThemes" + s + themeFile;
            }
            
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
            if (path == null || path.Length == 0) return null;
            Boolean isPathNetworked = path.StartsWith("\\\\") || path.StartsWith("//");
            if (Path.IsPathRooted(path) || isPathNetworked) return path;
            String resolvedPath = Path.Combine(relativeTo, path);
            if (Directory.Exists(resolvedPath) || File.Exists(resolvedPath)) return resolvedPath;
            return null;
        }

        public static String GetThemeFolderPath()
        {
            char s = Path.DirectorySeparatorChar;
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Win32NT:
                case PlatformID.Win32S:
                case PlatformID.Win32Windows:
                case PlatformID.WinCE:
                    return string.Format("{0}{1}Application Data{1}Adobe{1}Flash Builder{1}Themes",
                                         Environment.GetEnvironmentVariable("USERPROFILE"), s);
                default:
                    return string.Format("{0}{1}Library{1}Application Support{1}Adobe{1}Flash Builder{1}Themes",
                                         Environment.GetEnvironmentVariable("HOME"), s);
            }
        }
    }
}

