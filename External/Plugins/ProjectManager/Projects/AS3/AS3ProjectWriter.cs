using System.Text;

namespace ProjectManager.Projects.AS3
{
    public class AS3ProjectWriter : ProjectWriter
    {
        AS3Project project;

        public AS3ProjectWriter(AS3Project project, string filename)
            : base(project, filename)
        {
            this.project = base.Project as AS3Project;
        }

        protected override void OnAfterWriteClasspaths()
        {
            WriteBuildOptions();
            WriteLibraries();
            WriteLibraryAssets();
        }

        public void WriteLibraryAssets()
        {
            WriteComment(" Assets to embed into the output SWF ");
            WriteStartElement("library");

            if (project.LibraryAssets.Count > 0)
            {
                foreach (LibraryAsset asset in project.LibraryAssets)
                {
                    WriteStartElement("asset");
                    WriteAttributeString("path", asset.Path);

                    if (asset.UpdatePath != null)
                        WriteAttributeString("update", asset.UpdatePath);

                    if (asset.FontGlyphs != null)
                        WriteAttributeString("glyphs", asset.FontGlyphs);

                    WriteEndElement();
                }
            }
            else WriteExample("asset", "path", "id", "update", "glyphs", "mode", "place", "sharepoint");

            WriteEndElement();
        }

        public void WriteLibraries()
        {
            MxmlcOptions options = project.CompilerOptions;

            WriteComment(" SWC Include Libraries ");
            WriteList("includeLibraries", options.IncludeLibraries);
            WriteComment(" SWC Libraries ");
            WriteList("libraryPaths", options.LibraryPaths);
            WriteComment(" External Libraries ");
            WriteList("externalLibraryPaths", options.ExternalLibraryPaths);
            WriteComment(" Runtime Shared Libraries ");
            WriteList("rslPaths", options.RSLPaths);
            WriteComment(" Intrinsic Libraries ");
            WriteList("intrinsics", options.IntrinsicPaths);
        }

        public void WriteBuildOptions()
        {
            WriteComment(" Build options ");
            WriteStartElement("build");

            MxmlcOptions options = project.CompilerOptions;

            WriteOption("accessible", options.Accessible);
            WriteOption("advancedTelemetry", options.AdvancedTelemetry);
            if (!string.IsNullOrEmpty(options.AdvancedTelemetryPassword))
                WriteOption("advancedTelemetryPassword", options.AdvancedTelemetryPassword);
            WriteOption("allowSourcePathOverlap", options.AllowSourcePathOverlap);
            WriteOption("benchmark", options.Benchmark);
            WriteOption("es", options.ES);
            WriteOption("inline", options.InlineFunctions);
            WriteOption("locale", options.Locale);
            WriteOption("loadConfig", options.LoadConfig);
            WriteOption("optimize", options.Optimize);
            WriteOption("omitTraces", options.OmitTraces);
            WriteOption("showActionScriptWarnings", options.ShowActionScriptWarnings);
            WriteOption("showBindingWarnings", options.ShowBindingWarnings);
            WriteOption("showInvalidCSS", options.ShowInvalidCSS);
            WriteOption("showDeprecationWarnings", options.ShowDeprecationWarnings);
            WriteOption("showUnusedTypeSelectorWarnings", options.ShowUnusedTypeSelectorWarnings);
            WriteOption("strict", options.Strict);
            WriteOption("useNetwork", options.UseNetwork);
            WriteOption("useResourceBundleMetadata", options.UseResourceBundleMetadata);
            WriteOption("warnings", options.Warnings);
            WriteOption("verboseStackTraces", options.VerboseStackTraces);
            WriteOption("linkReport", options.LinkReport);
            WriteOption("loadExterns", options.LoadExterns);
            WriteOption("staticLinkRSL", options.StaticLinkRSL);

            WriteOption("additional", string.Join("\n", options.Additional));
            WriteOption("compilerConstants", string.Join("\n", options.CompilerConstants));
            WriteOption("minorVersion", options.MinorVersion);

            WriteNamespaces(options);

            WriteEndElement();
        }

        void WriteList(string name, string[] items)
        {
            WriteStartElement(name);
            WritePaths(items, "element");
            WriteEndElement();
        }

        void WriteNamespaces(MxmlcOptions options)
        {
            if (options.Namespaces == null || options.Namespaces.Length == 0) return;
            var namespaces = new StringBuilder();
            foreach (var ns in options.Namespaces)
            {
                if (string.IsNullOrEmpty(ns.Uri) || string.IsNullOrEmpty(ns.Manifest)) continue;

                string relPath = project.GetRelativePath(ns.Manifest);

                namespaces.Append(ns.Uri).Append('\n').Append(relPath).Append('\n');
            }

            if (namespaces.Length == 0) return;
            
            namespaces.Remove(namespaces.Length - 1, 1);

            WriteOption("namespaces", namespaces.ToString());
        }
    }
}
