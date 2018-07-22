using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using PluginCore;

namespace ProjectManager.Projects
{
    public class BuildEventInfo
    {
        public string Name;
        public string Value;

        public BuildEventInfo(string name, string value)
        {
            this.Name = name;
            this.Value = value;
        }

        // SendKeys requires brackets around certain characters which have meaning
        public string SendKeysName => "${(}" + Name + "{)}";
        public string FormattedName => "$(" + Name + ")";
    }

    public class BuildEventVars
    {
        Project project;
        List<BuildEventInfo> additional = new List<BuildEventInfo>();

        public BuildEventVars(Project project)
        {
            this.project = project;
        }

        public void AddVar(string name, string value)
        {
            additional.Add(new BuildEventInfo(name, value));
        }

        public BuildEventInfo[] GetVars()
        {
            List<BuildEventInfo> infos = new List<BuildEventInfo>
            {
                new BuildEventInfo("BaseDir", BaseDir),
                new BuildEventInfo("FDBuild", FDBuild),
                new BuildEventInfo("ToolsDir", ToolsDir),
                new BuildEventInfo("TimeStamp", DateTime.Now.ToString("g"))
            };
            if (project != null)
            {
                infos.Add(new BuildEventInfo("OutputFile", project.OutputPath));
                infos.Add(new BuildEventInfo("OutputDir", Path.GetDirectoryName(project.GetAbsolutePath(project.OutputPath))));
                infos.Add(new BuildEventInfo("OutputName", Path.GetFileName(project.OutputPath)));
                infos.Add(new BuildEventInfo("ProjectName", project.Name));
                infos.Add(new BuildEventInfo("ProjectDir", project.Directory));
                infos.Add(new BuildEventInfo("ProjectPath", project.ProjectPath));
                infos.Add(new BuildEventInfo("BuildConfig", project.TraceEnabled ? "debug" : "release"));
                infos.Add(new BuildEventInfo("TargetPlatform", project.MovieOptions.Platform));
                infos.Add(new BuildEventInfo("TargetVersion", project.MovieOptions.Version));
                infos.Add(new BuildEventInfo("TargetBuild", project.TargetBuild ?? ""));
                infos.Add(new BuildEventInfo("CompilerPath", project.CurrentSDK));
                if (project.Language == "as3") infos.Add(new BuildEventInfo("FlexSDK", project.CurrentSDK));
            }
            infos.AddRange(additional);
            return infos.ToArray();
        }

        public string FDBuildDir => Path.GetDirectoryName(FDBuild);
        public string ToolsDir => Path.GetDirectoryName(FDBuildDir);

        public string BaseDir
        {
            get
            {
                string localPath = new Uri(Assembly.GetEntryAssembly().GetName().CodeBase).LocalPath;
                string appDir = Path.GetDirectoryName(localPath);
#if FDBUILD
                string toolsDir = Path.GetDirectoryName(appDir);
                appDir = Path.GetDirectoryName(toolsDir);
#endif

                string local = Path.Combine(appDir, ".local");
                if (File.Exists(local)) return appDir;

                string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                return Path.Combine(localAppData, DistroConfig.DISTRIBUTION_NAME);
            }
        }

        public string FDBuild
        {
            get
            {
                string localPath = new Uri(Assembly.GetEntryAssembly().GetName().CodeBase).LocalPath;

#if !FDBUILD
                string appDir = Path.GetDirectoryName(localPath);
                return Path.Combine(appDir, "Tools", "fdbuild", "fdbuild.exe");
#else
                return localPath;
#endif
            }
        }

    }

}
