namespace ProjectManager.Projects.Generic
{
    public class GenericProjectReader : ProjectReader
    {
        public GenericProjectReader(string filename)
            : base(filename, new GenericProject(filename))
        {
        }

        public new GenericProject ReadProject() => base.ReadProject() as GenericProject;
    }
}