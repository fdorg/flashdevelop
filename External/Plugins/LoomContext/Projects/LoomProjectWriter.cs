// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using ProjectManager.Projects;

namespace LoomContext.Projects
{
    public class LoomProjectWriter : ProjectWriter
    {
        LoomProject project;

        public LoomProjectWriter(LoomProject project, string filename)
            : base(project, filename)
        {
            this.project = base.Project as LoomProject;
        }

        protected override void OnAfterWriteClasspaths()
        {
            WriteBuildOptions();
            WriteLibraries();
        }

        public void WriteLibraries()
        {
            LoomOptions options = project.CompilerOptions;

            WriteComment(" Loom Libraries ");
            WriteList("libraryPaths", options.LibraryPaths);
            //WriteComment(" Intrinsic Libraries ");
            //WriteList("intrinsics", options.IntrinsicPaths);
        }

        public void WriteBuildOptions()
        {
            WriteComment(" Build options ");
            WriteStartElement("build");

            LoomOptions options = project.CompilerOptions;

            //WriteOption("warnings", options.Warnings);
            WriteOption("additional", string.Join("\n", options.Additional));

            WriteEndElement();
        }

        void WriteList(string name, string[] items)
        {
            WriteStartElement(name);
            WritePaths(items, "element");
            WriteEndElement();
        }
    }
}
