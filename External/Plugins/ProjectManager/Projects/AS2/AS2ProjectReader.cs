using System;
using System.Collections.Generic;

namespace ProjectManager.Projects.AS2
{
    public class AS2ProjectReader : ProjectReader
    {
        AS2Project project;

        public AS2ProjectReader(string filename)
            : base(filename, new AS2Project(filename))
        {
            this.project = base.Project as AS2Project;
        }

        public new AS2Project ReadProject()
        {
            return base.ReadProject() as AS2Project;
        }

        // process AS2-specific stuff
        protected override void ProcessNode(string name)
        {
            switch (name)
            {
                case "build": ReadBuildOptions(); break;
                case "library": ReadLibraryAssets(); break;
                default: base.ProcessNode(name); break;
            }
        }

        public void ReadBuildOptions()
        {
            List<string> includePackages = new List<string>();

            ReadStartElement("build");
            while (Name == "option")
            {
                MoveToFirstAttribute();
                switch (Name)
                {
                    case "verbose": project.CompilerOptions.Verbose = BoolValue; break;
                    case "strict": project.CompilerOptions.Strict = BoolValue; break;
                    case "infer": project.CompilerOptions.Infer = BoolValue; break;
                    case "includePackage": includePackages.Add(OSPath(Value)); break;
                    case "useMain": project.CompilerOptions.UseMain = BoolValue; break;
                    case "useMX": project.CompilerOptions.UseMX = BoolValue; break;
                    case "warnUnusedImports": project.CompilerOptions.WarnUnusedImports = BoolValue; break;
                    case "traceMode":
                        // upgrade from .FDP
                        string traceMode = (Value == "FlashOut") ? "FlashViewer" : Value;
                        project.CompilerOptions.TraceMode
                            = (TraceMode)Enum.Parse(typeof(TraceMode), traceMode, true);
                        break;
                    case "traceFunction": project.CompilerOptions.TraceFunction = Value; break;
                    case "libraryPrefix": project.CompilerOptions.LibraryPrefix = Value; break;
                    case "excludeFile": project.CompilerOptions.ExcludeFile = Value; break;
                    case "groupClasses": project.CompilerOptions.GroupClasses = BoolValue; break;
                    case "frame": project.CompilerOptions.Frame = IntValue; break;
                    case "keep": project.CompilerOptions.Keep = BoolValue; break;
                }
                Read();
            }
            ReadEndElement();

            project.CompilerOptions.IncludePackages = includePackages.ToArray();
        }

        public void ReadLibraryAssets()
        {
            ReadStartElement("library");
            while (Name == "asset")
            {
                string path = OSPath(GetAttribute("path"));
                string mode = GetAttribute("mode");

                if (path == null)
                    throw new Exception("All library assets must have a 'path' attribute.");

                LibraryAsset asset = new LibraryAsset(project, path);
                project.LibraryAssets.Add(asset);

                asset.ManualID = GetAttribute("id"); // could be null
                asset.UpdatePath = OSPath(GetAttribute("update")); // could be null
                asset.FontGlyphs = GetAttribute("glyphs"); // could be null

                if (mode != null)
                    asset.SwfMode = (SwfAssetMode)Enum.Parse(typeof(SwfAssetMode), mode, true);

                if (asset.SwfMode == SwfAssetMode.Shared)
                    asset.Sharepoint = GetAttribute("sharepoint"); // could be null

                if (asset.IsImage && GetAttribute("bitmap") != null)
                    asset.BitmapLinkage = Boolean.Parse(GetAttribute("bitmap"));

                Read();
            }
            ReadEndElement();
        }
    }
}
