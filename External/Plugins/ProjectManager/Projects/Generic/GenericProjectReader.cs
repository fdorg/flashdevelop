namespace ProjectManager.Projects.Generic
{
    public class GenericProjectReader : ProjectReader
    {
        GenericProject project;

        public GenericProjectReader(string filename)
            : base(filename, new GenericProject(filename))
        {
            project = Project as GenericProject;
        }

        public new GenericProject ReadProject()
        {
            return base.ReadProject() as GenericProject;
        }
    }
}
