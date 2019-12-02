// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Text;
using System.Xml;
using ProjectManager.Projects.AS3;
using System.IO;
using System.Linq;


namespace FDBuild.Building.AS3
{
    class FlexConfigWriter : XmlTextWriter
    {
        AS3Project project;
        bool flex4;
        bool asc2;

        public FlexConfigWriter(string libraryPath): base(libraryPath, new UTF8Encoding(false))
        {
            Formatting = Formatting.Indented;
        }

        public void WriteConfig(AS3Project project, double sdkVersion, string[] extraClasspaths, bool debugMode, bool asc2Mode)
        {
            this.project = project;
            project.UpdateVars(true);

            flex4 = sdkVersion >= 4;
            asc2 = asc2Mode;

            try { InternalWriteConfig(extraClasspaths, debugMode); }
            finally { Close(); }
        }

        private void InternalWriteConfig(string[] extraClasspaths, bool debugMode)
        {
            MxmlcOptions options = project.CompilerOptions;

            WriteStartDocument();
            WriteComment("This Adobe Flex compiler configuration file was generated by a tool.");
            WriteComment("Any modifications you make may be lost.");
            WriteStartElement("flex-config");
                AddTargetPlayer(options);
                AddBaseOptions(options);
                // TODO add metadatas
                WriteStartElement("compiler");
                    AddCompilerConstants(options, debugMode);
                    AddCompilerOptions(options, debugMode);
                    AddClassPaths(extraClasspaths);
                    AddLibraries();
                    AddNamespaces(options);
                WriteEndElement();
                AddRSLs();
                AddCompileTargets();
                AddMovieOptions();
            WriteEndElement();
        }

        private void AddCompilerConstants(MxmlcOptions options, bool debugMode)
        {
            WriteDefine("CONFIG::debug", debugMode ? "true" : "false");
            WriteDefine("CONFIG::release", debugMode ? "false" : "true");
            WriteDefine("CONFIG::timeStamp", "'" + DateTime.Now.ToString("d") + "'");
            var isMobile = project.MovieOptions.Platform == "AIR Mobile";
            var isDesktop = project.MovieOptions.Platform == "AIR";
            WriteDefine("CONFIG::air", isMobile || isDesktop ? "true" : "false");
            WriteDefine("CONFIG::mobile", isMobile ? "true" : "false");
            WriteDefine("CONFIG::desktop", isDesktop ? "true" : "false");

            if (options.CompilerConstants != null)
            {
                foreach (string define in options.CompilerConstants)
                {
                    int p = define.IndexOf(',');
                    if (p < 0) continue;
                    WriteDefine(define.Substring(0, p), define.Substring(p + 1));
                }
            }
        }

        private void WriteDefine(string name, string value)
        {
            WriteStartElement("define");
                WriteAttributeString("append", "true");
                WriteElementString("name", name);
                WriteElementString("value", value);
            WriteEndElement();
        }

        private void AddCompilerOptions(MxmlcOptions options, bool debugMode)
        {
            if (options.Locale.Length > 0)
            {
                WriteStartElement("locale");
                    WriteElementString("locale-element", options.Locale);
                WriteEndElement();
            }
            if (options.Accessible) WriteElementString("accessible", "true");
            if (options.AllowSourcePathOverlap) WriteElementString("allow-source-path-overlap", "true");
            if (options.ES)
            {
                WriteElementString("es", "true");
                WriteElementString("as3", "false");
            }
            if (!options.Strict) WriteElementString("strict", "false");
            if (!options.ShowActionScriptWarnings) WriteElementString("show-actionscript-warnings", "false");
            if (!options.ShowBindingWarnings) WriteElementString("show-binding-warnings", "false");
            if (!options.ShowInvalidCSS) WriteElementString("show-invalid-css-property-warnings", "false");
            if (!options.ShowDeprecationWarnings) WriteElementString("show-deprecation-warnings", "false");
            if (!options.ShowUnusedTypeSelectorWarnings) WriteElementString("show-unused-type-selector-warnings", "false");
            if (!options.UseResourceBundleMetadata) WriteElementString("use-resource-bundle-metadata", "false");

            if (!debugMode && options.Optimize) WriteElementString("optimize", "true");
            if (!debugMode && flex4) WriteElementString("omit-trace-statements", options.OmitTraces ? "true" : "false");
            if (debugMode) WriteElementString("verbose-stacktraces", "true");
            else WriteElementString("verbose-stacktraces", options.VerboseStackTraces ? "true" : "false");
        }

        private void AddBaseOptions(MxmlcOptions options)
        {
            if (!asc2)
            {
                if (!options.Benchmark) WriteElementString("benchmark", "false");
                else WriteElementString("benchmark", "true");
                WriteElementString("static-link-runtime-shared-libraries", options.StaticLinkRSL ? "true" : "false");
            }
            if (!options.UseNetwork) WriteElementString("use-network", "false");
            if (!options.Warnings) WriteElementString("warnings", "false");
        }

        private void AddTargetPlayer(MxmlcOptions options)
        {
            int majorVersion = project.MovieOptions.MajorVersion;
            int minorVersion = project.MovieOptions.MinorVersion;
            string version;
            if (options.MinorVersion.Length > 0)
                version = majorVersion + "." + options.MinorVersion;
            else
                version = majorVersion + "." + minorVersion;

            string platform = project.MovieOptions.Platform;
            if (platform != PluginCore.PlatformData.FLASHPLAYER_PLATFORM
                    && project.MovieOptions.HasPlatformSupport && project.MovieOptions.PlatformSupport.IsFlashPlatform)
                version = PluginCore.PlatformData.ResolveFlashPlayerVersion(project.Language, platform, version);

            WriteElementString("target-player", version);
        }

        private void AddLibraries()
        {
            MxmlcOptions options = project.CompilerOptions;
            string absPath;
            if (options.IncludeLibraries.Length > 0)
            {
                WriteStartElement("include-libraries");
                foreach (string path in options.IncludeLibraries)
                {
                    if (path.Trim().Length == 0) continue;
                    absPath = project.GetAbsolutePath(path);
                    if (File.Exists(absPath))
                        WriteElementPathString("library", absPath);
                    else if (Directory.Exists(absPath))
                    {
                        string[] libs = Directory.GetFiles(absPath, "*.swc");
                        foreach(string lib in libs)
                            WriteElementPathString("library", lib);
                    }
                }
                WriteEndElement();
            }
            if (options.ExternalLibraryPaths.Length > 0)
            {
                WriteStartElement("external-library-path");
                WriteAttributeString("append", "true");
                foreach (string path in options.ExternalLibraryPaths)
                {
                    if (path.Trim().Length == 0) continue;
                    absPath = project.GetAbsolutePath(path);
                    if (File.Exists(absPath) || Directory.Exists(absPath))
                        WriteElementPathString("path-element", absPath);
                }
                WriteEndElement();
            }
            if (options.LibraryPaths.Length > 0)
            {
                WriteStartElement("library-path");
                WriteAttributeString("append", "true");
                foreach (string path in options.LibraryPaths)
                {
                    if (path.Trim().Length == 0) continue;
                    absPath = project.GetAbsolutePath(path);
                    if (File.Exists(absPath) || Directory.Exists(absPath))
                        WriteElementPathString("path-element", absPath);
                }
                WriteEndElement();
            }
        }

        private void AddNamespaces(MxmlcOptions options)
        {
            if (options.Namespaces is null || options.Namespaces.Length == 0) return;

            WriteStartElement("namespaces");

            foreach (var ns in options.Namespaces)
            {
                string path = ns.Manifest.Trim();
                string uri = ns.Uri.Trim();
                if (path.Length == 0 || uri.Length == 0) continue;
                path = project.GetAbsolutePath(path);
                if (!File.Exists(path)) continue;

                WriteStartElement("namespace");

                WriteElementString("uri", uri);
                WriteElementString("manifest", path);

                WriteEndElement();
            }

            WriteEndElement();
        }

        private void AddRSLs()
        {
            MxmlcOptions options = project.CompilerOptions;
            if (options.RSLPaths.Length > 0)
            {
                foreach (string path in options.RSLPaths)
                {
                    string[] parts = path.Split(',');
                    if (parts.Length < 2) continue;
                    if (parts[0].Trim().Length == 0) continue;
                    string absPath = project.GetAbsolutePath(parts[0]);
                    if (File.Exists(absPath))
                    {
                        WriteStartElement("runtime-shared-library-path");
                            WriteElementString("path-element", absPath);
                            WriteElementString("rsl-url", parts[1]);
                            if (parts.Length > 2)
                                WriteElementString("policy-file-url", parts[2]);
                            if (parts.Length > 3)
                                WriteElementString("rsl-url", parts[3]);
                            if (parts.Length > 4)
                                WriteElementString("policy-file-url", parts[4]);
                        WriteEndElement();
                    }
                }
            }
        }

        private void AddMovieOptions()
        {
            WriteElementString("default-background-color", project.MovieOptions.Background);
            WriteElementString("default-frame-rate", project.MovieOptions.Fps.ToString());
            WriteStartElement("default-size");
                WriteElementString("width", project.MovieOptions.Width.ToString());
                WriteElementString("height", project.MovieOptions.Height.ToString());
            WriteEndElement();
        }

        public void AddClassPaths(string[] extraClasspaths)
        {
            WriteStartElement("source-path");
            WriteAttributeString("append", "true");

            // build classpaths
            var classPaths = project.AbsoluteClasspaths.ToList();
            foreach (string extraClassPath in extraClasspaths)
                if (Directory.Exists(extraClassPath))
                    classPaths.Add(extraClassPath);

            foreach (string classPath in classPaths)
                if (Directory.Exists(classPath))
                    WriteElementPathString("path-element", classPath);

            WriteEndElement();
        }

        private void WriteElementPathString(string name, string path)
        {
            if (Directory.Exists(path) || File.Exists(path))
                WriteElementString(name, path);
        }

        public void AddCompileTargets()
        {
            if (project.CompileTargets.Count == 0) return;
            WriteStartElement("file-specs");
            foreach (string relTarget in project.CompileTargets)
            {
                string target = project.GetAbsolutePath(relTarget);
                if (File.Exists(target))
                {
                    WriteElementString("path-element", target);
                    break;
                }
            }
            WriteEndElement();
        }
    }
}
