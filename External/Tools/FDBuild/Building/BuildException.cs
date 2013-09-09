using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectManager.Building
{
    public class BuildException : Exception
    {
        public BuildException(string message) : base(message) { }
    }
}
