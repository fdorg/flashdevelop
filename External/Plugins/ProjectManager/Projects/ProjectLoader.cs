using System;
using System.IO;
using PluginCore.Helpers;
using ProjectManager.Helpers;

namespace ProjectManager.Projects
{
    internal class ProjectLoader
    {
        public static Project Load(string file)
        {
            var ext = Path.GetExtension(file).ToLower();
            if (!ProjectCreator.IsKnownProject(ext) && !FileInspector.IsProject(file, ext))
                throw new Exception("Unknown project extension: " + Path.GetFileName(file));
            var projectType = ProjectCreator.GetProjectType(ProjectCreator.KeyForProjectPath(file));
            if (projectType == null) throw new Exception("Invalid project type: " + Path.GetFileName(file));
            var para = new object[1];
            para[0] = file;
            return (Project)projectType.GetMethod("Load").Invoke(null, para);
        }
    }
}