using System;

namespace ProjectManager.Building
{
    public class BuildException : Exception
    {
        public BuildException(string message) : base(message) { }
    }
}
