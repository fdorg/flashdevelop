using System;
using System.Collections.Generic;
using System.Text;
using PluginCore;
using System.IO;
using PluginCore.Managers;

namespace ProjectManager.Projects
{
    public class Solution: ISolution
    {
        public string Name { get; private set; }
        public string SolutionPath { get; private set; }
        public IProject[] Projects { get; private set; }
        public IProject RunProject;
        public IProject MainProject;

        public bool TraceEnabled
        {
            get
            {
                return (MainProject as Project).TraceEnabled;
            }
            set
            {
                foreach (Project project in Projects)
                    project.TraceEnabled = value;
            }
        }

        public string TargetBuild 
        {
            get
            {
                return (MainProject as Project).TargetBuild;
            }
            set
            {
                foreach (Project project in Projects)
                    project.TargetBuild = value;
            }
        }

        public bool ShowHiddenPaths 
        {
            get
            {
                return (MainProject as Project).ShowHiddenPaths;
            }
            set
            {
                foreach (Project project in Projects)
                    project.ShowHiddenPaths = value;
            }
        }

        public Solution(Project project)
        {
            Init(project);
        }

        public Solution(string path)
        {
            Init(ProjectLoader.Load(path));
        }

        void Init(Project project)
        {
            Name = Path.GetFileNameWithoutExtension(project.ProjectPath);
            SolutionPath = project.ProjectPath;

            RunProject = MainProject = project;
            LoadProjects();
        }

        void LoadProjects()
        {
            Project main = MainProject as Project;
            string solutionData = null;
            main.Storage.TryGetValue("Solution", out solutionData);
            if (solutionData == null)
            {
                Projects = new IProject[] { MainProject };
                return;
            }

            var projects = new List<IProject>();
            var solutionPaths = solutionData.Split('|');
            foreach (string relPath in solutionPaths)
            {
                try
                {
                    var fullPath = ValidatePath(main, relPath);
                    if (fullPath == null) continue;
                    if (fullPath == main.ProjectPath) projects.Add(MainProject);
                    else projects.Add(ProjectLoader.Load(fullPath));
                }
                catch (Exception ex) 
                {
                    TraceManager.AddAsync(ex.Message);
                }
            }
            Projects = projects.ToArray();
        }

        string ValidatePath(Project main, string relPath)
        {
            var fullPath = main.GetAbsolutePath(relPath);

            if (String.Equals(fullPath, main.ProjectPath, StringComparison.OrdinalIgnoreCase))
                return main.ProjectPath;

            if (!File.Exists(fullPath))
                throw new Exception(String.Format("Project '{0}' in solution '{1}' does not exist.", fullPath, main.ProjectPath));

            return fullPath;
        }

        public void UpdateVars()
        {
            foreach (Project project in Projects)
                project.UpdateVars(true);
        }

        public bool GetDefaultTraceEnabled()
        {
            if (Projects.Length == 0) return false;
            Project project = MainProject as Project;
            return project.EnableInteractiveDebugger
                && project.OutputType != OutputType.OtherIDE && project.OutputPath != "";
        }

        internal void Add(Project project)
        {
            project.TraceEnabled = TraceEnabled;
            project.TargetBuild = TargetBuild;
            project.UpdateVars(true);

            List<IProject> temp = new List<IProject>(Projects);
            temp.Add(project);
            Projects = temp.ToArray();
        }

        public void Save()
        {
            if (MainProject == null) return;

            Project main = MainProject as Project;

            var paths = new List<String>();
            foreach (Project project in Projects)
            {
                var path = main.GetRelativePath(project.ProjectPath);
                paths.Add(path);
            }
            var data = String.Join("|", paths.ToArray());

            main.Storage.Add("Solution", data);
            main.Save();
        }
    }
}
