using System;

namespace ProjectManager.Projects.AS2
{
    public class AS2ProjectWriter : ProjectWriter
    {
        AS2Project project;
        string filename;

        public AS2ProjectWriter(AS2Project project, string filename)
            : base(project, filename)
        {
            this.project = base.Project as AS2Project;
            this.filename = filename;
        }

        protected override void OnAfterWriteClasspaths()
        {
            WriteBuildOptions();
        }

        protected override void OnAfterWriteCompileTargets()
        {
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
                    
                    if (asset.IsImage)
                        WriteAttributeString("bitmap", asset.BitmapLinkage.ToString());

                    if (asset.ManualID != null)
                        WriteAttributeString("id", asset.ManualID);

                    if (asset.UpdatePath != null)
                        WriteAttributeString("update", asset.UpdatePath);

                    if (asset.FontGlyphs != null)
                        WriteAttributeString("glyphs", asset.FontGlyphs);

                    if (asset.IsSwf && asset.SwfMode != SwfAssetMode.Library)
                        WriteAttributeString("mode", asset.SwfMode.ToString());

                    if (asset.SwfMode == SwfAssetMode.Shared)
                    {
                        if (asset.Sharepoint != null)
                            WriteAttributeString("sharepoint", asset.Sharepoint);
                    }

                    WriteEndElement();
                }
            }
            else WriteExample("asset", "path", "id", "update", "glyphs", "mode", "place", "sharepoint");

            WriteEndElement();
        }

        public void WriteBuildOptions()
        {
            WriteComment(" Build options ");
            WriteStartElement("build");
            WriteOption("verbose", project.CompilerOptions.Verbose);
            WriteOption("strict", project.CompilerOptions.Strict);
            WriteOption("infer", project.CompilerOptions.Infer);

            foreach (string pack in project.CompilerOptions.IncludePackages)
                WriteOption("includePackage", pack);

            WriteOption("useMain", project.CompilerOptions.UseMain);
            WriteOption("useMX", project.CompilerOptions.UseMX);
            WriteOption("warnUnusedImports", project.CompilerOptions.WarnUnusedImports);

            // compatibility with .FDP
            if (filename.EndsWith(".fdp", StringComparison.OrdinalIgnoreCase)
                && project.CompilerOptions.TraceMode == TraceMode.FlashViewer)
                WriteOption("traceMode", "FlashOut");
            else
                WriteOption("traceMode", project.CompilerOptions.TraceMode);

            WriteOption("traceFunction", project.CompilerOptions.TraceFunction);
            WriteOption("libraryPrefix", project.CompilerOptions.LibraryPrefix);
            WriteOption("excludeFile", project.CompilerOptions.ExcludeFile);
            WriteOption("groupClasses", project.CompilerOptions.GroupClasses);
            WriteOption("frame", project.CompilerOptions.Frame);
            WriteOption("keep", project.CompilerOptions.Keep);
            WriteEndElement();
        }
    }
}
