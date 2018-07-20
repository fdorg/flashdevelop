// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using System;
using System.IO;
using ProjectManager.ProjectFormat;
using ProjectManager.ProjectBuilding;

namespace FDBuild
{
    class CommandLineBuilder
    {
        public static int Run(string[] args)
        {
            FDBuildOptions options = new FDBuildOptions(args);

            // save current directory - ProjectBuilder might change it
            string directory = Environment.CurrentDirectory;

            // try and automagically figure out flashdevelop's library path
            // it should be at ..\..\Library
            try
            {
                string toolsDir = Path.GetDirectoryName(ProjectPaths.ApplicationDirectory);
                string firstRunDir = Path.GetDirectoryName(toolsDir);
                string libraryDir = Path.Combine(firstRunDir, "Library");
                string as2LibraryDir = Path.Combine(libraryDir, "AS2");
                if (Directory.Exists(as2LibraryDir))
                    options.ExtraClasspath = as2LibraryDir;
            }
            catch { }

            try
            {
                if (options.ProjectFile.Length > 0)
                    Build(Path.GetFullPath(options.ProjectFile),
                        options.ExtraClasspaths, options.NoTrace);
                else
                {
                    // build everything in this directory
                    string[] files = Directory.GetFiles(directory, "*.fdp");

                    if (files.Length == 0)
                    {
                        options.DoHelp();
                        return 0;
                    }

                    foreach (string file in files)
                        Build(file, options.ExtraClasspaths, options.NoTrace);
                }

                return 0;
            }
            catch (BuildException exception)
            {
                Console.Error.WriteLine(exception.Message);
                return 1;
            }
            catch (Exception exception)
            {
                Console.Error.WriteLine("Exception: " + exception.Message);
                return 1;
            }
            finally
            {
                Environment.CurrentDirectory = directory;

                if (options.PauseAtEnd)
                {
                    Console.WriteLine();
                    Console.WriteLine("Press enter to continue...");
                    Console.ReadLine();
                }
            }
        }

        public static void Build(string projectFile, string[] extraClasspaths, bool noTrace)
        {
            Project project = Project.Load(projectFile);
            ProjectBuilder builder = new ProjectBuilder(project);
            builder.Build(extraClasspaths, noTrace);
        }
    }
}
