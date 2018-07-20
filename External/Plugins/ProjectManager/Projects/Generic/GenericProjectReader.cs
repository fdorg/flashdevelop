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
