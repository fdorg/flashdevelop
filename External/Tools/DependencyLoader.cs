using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace FDBuild
{
    /// <summary>
    /// Resolves ProjectManager.dll in the plugins directory.
    /// </summary>
    static class DependencyLoader
    {
        public static void Init()
        {
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
        }

        static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            AssemblyName an = new AssemblyName(args.Name);
            if (an.Name == "ProjectManager")
                return LoadProjectManagerDll();
            else
                return null;
        }

        public static Assembly LoadProjectManagerDll()
        {
            // Try looking for ProjectManager.dll in ..\..\Plugins
            string fdbuildDir = ApplicationDirectory;
            string baseDir = Path.GetDirectoryName(Path.GetDirectoryName(fdbuildDir));
            string pluginsDir = Path.Combine(baseDir, "Plugins");
            string dllPath = Path.Combine(pluginsDir, "ProjectManager.dll");

            return Assembly.LoadFile(dllPath);
        }

        public static string ApplicationDirectory
        {
            get
            {
                string url = Assembly.GetEntryAssembly().GetName().CodeBase;
                Uri uri = new Uri(url);
                return Path.GetDirectoryName(uri.LocalPath);
            }
        }
    }
}
