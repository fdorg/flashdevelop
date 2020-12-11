// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
namespace ProjectManager.Projects.Generic
{
    public class GenericProjectWriter : ProjectWriter
    {
        public GenericProjectWriter(Project project, string filename)
            : base(project, filename)
        {
        }
    }
}