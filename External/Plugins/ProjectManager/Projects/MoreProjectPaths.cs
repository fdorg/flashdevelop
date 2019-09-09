// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
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
            List<string> allDirs = new List<string>();
            if (Directory.Exists(ProjectTemplatesDirectory))
            {
                allDirs.AddRange(Directory.GetDirectories(ProjectTemplatesDirectory));
            }
            if (!PluginBase.MainForm.StandaloneMode && Directory.Exists(PathHelper.UserProjectsDir))
            {
                allDirs.AddRange(Directory.GetDirectories(PathHelper.UserProjectsDir));
            }
            return allDirs;
        }

    }

}
