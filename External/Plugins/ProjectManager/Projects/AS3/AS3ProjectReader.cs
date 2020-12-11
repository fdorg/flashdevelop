// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;

namespace ProjectManager.Projects.AS3
{
    public class AS3ProjectReader : ProjectReader
    {
        readonly AS3Project project;

        public AS3ProjectReader(string filename) : base(filename, new AS3Project(filename)) => project = Project as AS3Project;

        public new AS3Project ReadProject() => base.ReadProject() as AS3Project;

        protected override void PostProcess()
        {
            base.PostProcess();
            if (version > 1) return;

            // import FD3 project
            if (project.MovieOptions.MajorVersion > 10)
            {
                // TODO slavara: actualize for last FP
                project.MovieOptions.MajorVersion = 10;
                project.MovieOptions.MinorVersion = 1;
            }

            bool isAIR = project.MovieOptions.Platform.Contains("AIR");
            if (project.CompilerOptions.Additional != null)
            {
                string add = string.Join("\n", project.CompilerOptions.Additional).Trim().Replace("\n\n", "\n");
                if (!isAIR && add.Contains("configname=air"))
                {
                    add = Regex.Replace(add, "(\\+)?configname=air", "");
                    project.CompilerOptions.Additional = add.Trim().Replace("\n\n", "\n").Split('\n');
                    project.MovieOptions.Platform = "AIR";
                    // TODO slavara: actualize for last AIR
                    project.MovieOptions.MajorVersion = 2;
                    project.MovieOptions.MinorVersion = 0;
                    project.Save();
                }
            }
        }

        // process AS3-specific stuff
        protected override void ProcessNode(string name)
        {
            if (NodeType == XmlNodeType.Element)
                switch (name)
                {
                    case "build": ReadBuildOptions(); break;
                    case "library": ReadLibraryAssets(); break;
                    case "includeLibraries": ReadIncludeLibraries(); break;
                    case "libraryPaths": ReadLibrayPath(); break;
                    case "externalLibraryPaths": ReadExternalLibraryPaths(); break;
                    case "rslPaths": ReadRSLPaths(); break;
                    case "intrinsics": ReadIntrinsicPaths(); break;
                    default: base.ProcessNode(name); break;
                }
        }

        private void ReadIntrinsicPaths() => project.CompilerOptions.IntrinsicPaths = ReadLibrary("intrinsics", SwfAssetMode.Ignore);

        private void ReadRSLPaths() => project.CompilerOptions.RSLPaths = ReadLibrary("rslPaths", SwfAssetMode.Ignore);

        private void ReadExternalLibraryPaths() => project.CompilerOptions.ExternalLibraryPaths = ReadLibrary("externalLibraryPaths", SwfAssetMode.ExternalLibrary);

        private void ReadLibrayPath() => project.CompilerOptions.LibraryPaths = ReadLibrary("libraryPaths", SwfAssetMode.Library);

        private void ReadIncludeLibraries() => project.CompilerOptions.IncludeLibraries = ReadLibrary("includeLibraries", SwfAssetMode.IncludedLibrary);

        private string[] ReadLibrary(string name, SwfAssetMode mode)
        {
            ReadStartElement(name);
            List<string> elements = new List<string>();
            while (Name == "element")
            {
                string path = OSPath(GetAttribute("path"));
                elements.Add(path);

                if (mode != SwfAssetMode.Ignore)
                {
                    LibraryAsset asset = new LibraryAsset(project, path);
                    asset.SwfMode = mode;
                    project.SwcLibraries.Add(asset);
                }
                Read();
            }
            ReadEndElement();
            return elements.ToArray();
        }

        public void ReadLibraryAssets()
        {
            ReadStartElement("library");
            while (Name == "asset")
            {
                string path = OSPath(GetAttribute("path"));

                if (path is null)
                    throw new Exception("All library assets must have a 'path' attribute.");

                LibraryAsset asset = new LibraryAsset(project, path);
                project.LibraryAssets.Add(asset);

                asset.UpdatePath = OSPath(GetAttribute("update")); // could be null
                asset.FontGlyphs = GetAttribute("glyphs"); // could be null

                Read();
            }
            ReadEndElement();
        }

        public void ReadBuildOptions()
        {
            MxmlcOptions options = project.CompilerOptions;

            ReadStartElement("build");
            while (Name == "option")
            {
                MoveToFirstAttribute();
                switch (Name)
                {
                    case "accessible": options.Accessible = BoolValue; break;
                    case "advancedTelemetry": options.AdvancedTelemetry = BoolValue; break;
                    case "advancedTelemetryPassword": options.AdvancedTelemetryPassword = Value; break;
                    case "allowSourcePathOverlap": options.AllowSourcePathOverlap = BoolValue; break;
                    case "benchmark": options.Benchmark = BoolValue; break;
                    case "es": options.ES = BoolValue; break;
                    case "inline": options.InlineFunctions = BoolValue; break;
                    case "locale": options.Locale = Value; break;
                    case "loadConfig": options.LoadConfig = Value; break;
                    case "optimize": options.Optimize = BoolValue; break;
                    case "omitTraces": options.OmitTraces = BoolValue; break;
                    case "showActionScriptWarnings": options.ShowActionScriptWarnings = BoolValue; break;
                    case "showBindingWarnings": options.ShowBindingWarnings = BoolValue; break;
                    case "showInvalidCSS": options.ShowInvalidCSS = BoolValue; break;
                    case "showDeprecationWarnings": options.ShowDeprecationWarnings = BoolValue; break;
                    case "showUnusedTypeSelectorWarnings": options.ShowUnusedTypeSelectorWarnings = BoolValue; break;
                    case "strict": options.Strict = BoolValue; break;
                    case "useNetwork": options.UseNetwork = BoolValue; break;
                    case "useResourceBundleMetadata": options.UseResourceBundleMetadata = BoolValue; break;
                    case "warnings": options.Warnings = BoolValue; break;
                    case "verboseStackTraces": options.VerboseStackTraces = BoolValue; break;
                    case "linkReport": options.LinkReport = Value; break;
                    case "loadExterns": options.LoadExterns = Value; break;
                    case "staticLinkRSL": options.StaticLinkRSL = BoolValue; break;
                    case "additional": options.Additional = Value.Split('\n'); break;
                    case "compilerConstants": options.CompilerConstants = Value.Split('\n'); break;
                    case "minorVersion": options.MinorVersion = Value; break;
                    case "namespaces": options.Namespaces = ReadNamespaces(); break;
                }
                Read();
            }
            ReadEndElement();
        }

        private MxmlNamespace[] ReadNamespaces()
        {
            var data = Value.Split('\n');
            int entriesNo = data.Length;
            if (entriesNo < 2) return null;
            var namespaces = new MxmlNamespace[entriesNo / 2];
            int j = 0;
            for (int i = 0; i < entriesNo; i += 2)
                namespaces[j++] = new MxmlNamespace {Uri = data[i], Manifest = data[i + 1]};

            return namespaces;
        }
    }
}