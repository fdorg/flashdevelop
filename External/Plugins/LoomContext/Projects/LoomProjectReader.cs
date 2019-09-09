using System.Collections.Generic;
using System.Xml;
using ProjectManager.Projects;

namespace LoomContext.Projects
{
    public class LoomProjectReader : ProjectReader
    {
        LoomProject project;

        public LoomProjectReader(string filename)
            : base(filename, new LoomProject(filename))
        {
            this.project = base.Project as LoomProject;
        }

        public new LoomProject ReadProject()
        {
            return base.ReadProject() as LoomProject;
        }

        protected override void PostProcess()
        {
            base.PostProcess();
            if (project.CompileTargets.Count == 0)
                project.CompileTargets.Add("src/main.ls");
            if (project.OutputType == OutputType.Unknown)
                project.OutputType = OutputType.Application;
        }

        // process Loom-specific stuff
        protected override void ProcessNode(string name)
        {
            if (NodeType == XmlNodeType.Element)
            switch (name)
            {
                case "build": ReadBuildOptions(); break;
                case "libraryPaths": ReadLibrayPath(); break;
                //case "intrinsics": ReadIntrinsicPaths(); break;
                default: base.ProcessNode(name); break;
            }
        }

        private void ReadLibrayPath()
        {
            project.CompilerOptions.LibraryPaths = ReadLibrary("libraryPaths", SwfAssetMode.Library);
        }

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
                    project.LoomLibraries.Add(asset);
                }
                Read();
            }
            ReadEndElement();
            string[] result = new string[elements.Count];
            elements.CopyTo(result);
            return result;
        }

        /*public void ReadLibraryAssets()
        {
            ReadStartElement("library");
            while (Name == "asset")
            {
                string path = OSPath(GetAttribute("path"));

                if (path == null)
                    throw new Exception("All library assets must have a 'path' attribute.");

                LibraryAsset asset = new LibraryAsset(project, path);
                project.LibraryAssets.Add(asset);

                asset.UpdatePath = OSPath(GetAttribute("update")); // could be null
                asset.FontGlyphs = GetAttribute("glyphs"); // could be null

                Read();
            }
            ReadEndElement();
        }*/

        public void ReadBuildOptions()
        {
            LoomOptions options = project.CompilerOptions;

            ReadStartElement("build");
            while (Name == "option")
            {
                MoveToFirstAttribute();
                switch (Name)
                {
                    //case "warnings": options.Warnings = BoolValue; break;
                    case "additional": options.Additional = Value.Split('\n'); break;
                }
                Read();
            }
            ReadEndElement();
        }
    }
}
