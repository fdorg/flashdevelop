// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
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
