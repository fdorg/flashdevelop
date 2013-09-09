using System;
using System.Collections.Generic;
using System.Text;
using FDBuild.Building;
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
