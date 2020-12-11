using System.Collections.Generic;
using System.IO;
using PluginCore.Helpers;
using PluginCore;

namespace ProjectManager.Projects
{
    // This class is split off because FDBuild doesn't know what "PluginMain" is
    public partial class ProjectPaths
    {
        public static string ProjectTemplatesDirectory => PathHelper.ProjectsDir;

        public static string FileTemplatesDirectory => Path.Combine(PathHelper.TemplateDir, "ProjectFiles");

        /// <summary>
        /// 
        /// </summary>
        public static List<string> GetAllProjectDirs()
        {
            var result = new List<string>();
            if (Directory.Exists(ProjectTemplatesDirectory))
            {
                result.AddRange(Directory.GetDirectories(ProjectTemplatesDirectory));
            }
            if (!PluginBase.MainForm.StandaloneMode && Directory.Exists(PathHelper.UserProjectsDir))
            {
                result.AddRange(Directory.GetDirectories(PathHelper.UserProjectsDir));
            }
            return result;
        }
    }
}