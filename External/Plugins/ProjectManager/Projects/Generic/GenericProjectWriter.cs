namespace ProjectManager.Projects.Generic
{
    public class GenericProjectWriter : ProjectWriter
    {
        GenericProject project;

        public GenericProjectWriter(GenericProject project, string filename)
            : base(project, filename)
        {
            this.project = Project as GenericProject;
        }
    }
}
