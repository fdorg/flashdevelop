using System;
using System.IO;
using FDBuild.Building.AS3;
using ProjectManager.Projects;
using ProjectManager.Projects.AS2;
using ProjectManager.Projects.AS3;
using ProjectManager.Projects.Haxe;
using ProjectManager.Projects.Generic;
using ProjectManager.Building.AS2;
using ProjectManager.Building.AS3;
using ProjectManager.Building.Haxe;
using ProjectManager.Building.Generic;

namespace ProjectManager.Building
{
    public abstract class ProjectBuilder
    {
        readonly Project project;

        public string CompilerPath { get; set; }

        public ProjectBuilder(Project project, string compilerPath)
        {
            this.project = project;
            CompilerPath = compilerPath;
        }

        public static ProjectBuilder Create(Project project, string ipcName, string compilerPath)
        {
            if (project is AS2Project as2Project) return new AS2ProjectBuilder(as2Project, compilerPath);
            if (project is AS3Project as3Project)
            {
                if (Directory.Exists(Path.Combine(compilerPath, "js"))) return new FlexJSProjectBuilder(as3Project, compilerPath);
                return new AS3ProjectBuilder(as3Project, compilerPath, ipcName);
            }
            if (project is HaxeProject haxeProject) return new HaxeProjectBuilder(haxeProject, compilerPath);
            if (project is GenericProject genericProject) return new GenericProjectBuilder(genericProject, compilerPath);
            throw new Exception("FDBuild doesn't know how to build " + project.GetType().Name);
        }

        protected string FDBuildDirectory => ProjectPaths.ApplicationDirectory;

        public void Build(string[] extraClasspaths, bool debugMode, bool noPreBuild, bool noPostBuild)
        {
            Console.WriteLine("Building " + project.Name);
            var runner = new BuildEventRunner(project, CompilerPath);
            var attempedPostBuildEvent = false;
            try
            {
                if (!noPreBuild && project.PreBuildEvent.Length > 0)
                {
                    Console.WriteLine("Running Pre-Build Command Line...");
                    runner.Run(project.PreBuildEvent, debugMode);
                }

                if (project.OutputType == OutputType.Application) DoBuild(extraClasspaths, debugMode);
                attempedPostBuildEvent = true;

                if (!noPostBuild && project.PostBuildEvent.Length > 0)
                {
                    Console.WriteLine("Running Post-Build Command Line...");
                    runner.Run(project.PostBuildEvent, debugMode);
                }
            }
            finally
            {
                // honor the post-build request on a failed build if you want
                if (!attempedPostBuildEvent && project.AlwaysRunPostBuild &&
                    project.PostBuildEvent.Length > 0)
                {
                    Console.WriteLine("Running Post-Build Command Line...");
                    runner.Run(project.PostBuildEvent, debugMode);
                }
            }
            Console.WriteLine("Build succeeded");
        }

        public void BuildCommand(string[] extraClasspaths, bool noTrace) => DoBuild(extraClasspaths, noTrace);

        protected abstract void DoBuild(string[] extraClasspaths, bool noTrace);
    }
}