// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
namespace ProjectManager.Projects.Generic
{
    public class GenericProjectReader : ProjectReader
    {
        GenericProject project;

        public GenericProjectReader(string filename)
            : base(filename, new GenericProject(filename))
        {
            this.project = base.Project as GenericProject;
        }

        public new GenericProject ReadProject()
        {
            return base.ReadProject() as GenericProject;
        }
    }
}
