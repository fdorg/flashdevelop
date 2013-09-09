using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using ProjectManager.Projects.AS2;
using ProjectManager.Projects.AS3;
using ProjectManager.Projects.Haxe;
using ProjectManager.Helpers;

namespace ProjectManager.Projects
{
    class ProjectLoader
    {
        public static Project Load(string file)
        {
            string ext = Path.GetExtension(file).ToLower();

            if (FileInspector.IsProject(file))
            {
                Type projectType =
                    ProjectCreator.GetProjectType(ProjectCreator.KeyForProjectPath(file));
                if (projectType != null)
                {
                    object[] para = new object[1];
                    para[0] = file;
                    return (Project)projectType.GetMethod("Load").Invoke(null, para);
                }
                else
                {
                    throw new Exception("Invalid project type: " + Path.GetFileName(file));
                }
            }
            else
            {
                throw new Exception("Unknown project extension: " + Path.GetFileName(file));
            }
        }
    }
}
