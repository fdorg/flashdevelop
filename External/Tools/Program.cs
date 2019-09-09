// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Reflection;

namespace FDBuild
{
    /// <summary>
    /// FDBuild.exe is a command-line tool for compiling .fdp project files using
    /// MTASC and Swfmill.
    /// </summary>
    public class Program
    {
        public static int Main(string[] args)
        {
            DependencyLoader.Init();

            // We can't actually have any code in this class that directly references anything
            // that depends on ProjectManager.dll, because the assembly resolver will try to
            // load it before executing any code we can control.  The idea is we only want
            // one copy of ProjectManager.dll period, and it's in a different directory, so
            // the DependencyLoader class should set up code that will find it.

            // So we have to use reflection to discover our main class which does the actual work.

            Type builderType = Type.GetType("FDBuild.CommandLineBuilder");
            MethodInfo buildMethod = builderType.GetMethod("Run");
            
            object builder = Activator.CreateInstance(builderType);
            return (int)buildMethod.Invoke(builder, new object[] { args });
        }
    }
}
