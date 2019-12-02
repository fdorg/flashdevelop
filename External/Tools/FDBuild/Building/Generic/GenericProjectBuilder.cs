// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using ProjectManager.Projects.Generic;

namespace ProjectManager.Building.Generic
{
    public class GenericProjectBuilder : ProjectBuilder
    {
        public GenericProjectBuilder(GenericProject project, string compilerPath)
            : base(project, compilerPath)
        {
            // nothing
        }

        protected override void DoBuild(string[] extraClasspaths, bool noTrace)
        {
            // nothing
        }
    }
}
