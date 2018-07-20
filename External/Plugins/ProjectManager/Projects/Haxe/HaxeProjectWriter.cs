namespace ProjectManager.Projects.Haxe
{
    public class HaxeProjectWriter : ProjectWriter
    {
        HaxeProject project;

        public HaxeProjectWriter(HaxeProject project, string filename)
            : base(project, filename)
        {
            this.project = base.Project as HaxeProject;
        }

        protected override void OnAfterWriteClasspaths()
        {
            WriteBuildOptions();
            WriteLibraries();
        }

        public void WriteLibraries()
        {
            WriteComment(" haxelib libraries ");
            WriteStartElement("haxelib");
            string[] list = project.CompilerOptions.Libraries;
            if (list.Length > 0)
            {
                foreach (string name in list)
                {
                    WriteStartElement("library");
                    WriteAttributeString("name", name);
                    WriteEndElement();
                }
            }
            else WriteExample("library", "name");
            WriteEndElement();
        }

        protected override void OnAfterWriteCompileTargets()
        {
            if (project.IsFlashOutput) WriteLibraryAssets();
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

            HaxeOptions options = project.CompilerOptions;

            WriteOption("directives", string.Join("\n", options.Directives));
            WriteOption("flashStrict", options.FlashStrict);
            WriteOption("noInlineOnDebug", options.NoInlineOnDebug);
            WriteOption("mainClass", options.MainClass);
            WriteOption("enabledebug", options.EnableDebug);
            WriteOption("additional", string.Join("\n", options.Additional));
            
            WriteEndElement();
        }
    }
}
