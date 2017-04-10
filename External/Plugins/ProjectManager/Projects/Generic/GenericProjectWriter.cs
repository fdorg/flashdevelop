// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
namespace ProjectManager.Projects.Generic
{
    public class GenericProjectWriter : ProjectWriter
    {
        GenericProject project;

        public GenericProjectWriter(GenericProject project, string filename)
            : base(project, filename)
        {
            this.project = base.Project as GenericProject;
        }
    }
}
