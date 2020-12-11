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
            Name = name;
            Value = value;
        }

        // SendKeys requires brackets around certain characters which have meaning
        public string SendKeysName => "${(}" + Name + "{)}";
        public string FormattedName => "$(" + Name + ")";
    }

    public class BuildEventVars
    {
        readonly Project project;
        readonly List<BuildEventInfo> additional = new List<BuildEventInfo>();

        public BuildEventVars(Project project)
        {
            this.project = project;
        }

        public void AddVar(string name, string value) => additional.Add(new BuildEventInfo(name, value));

        public BuildEventInfo[] GetVars()
        {
            var infos = new List<BuildEventInfo>
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
                var appDir = ProjectPaths.ApplicationDirectory;
#if FDBUILD
                var toolsDir = Path.GetDirectoryName(appDir);
                appDir = Path.GetDirectoryName(toolsDir);
#endif
                var local = Path.Combine(appDir, ".local");
                if (File.Exists(local)) return appDir;
                var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                return Path.Combine(localAppData, DistroConfig.DISTRIBUTION_NAME);
            }
        }

        public string FDBuild
        {
            get
            {
                var localPath = ProjectPaths.GetAssemblyPath(Assembly.GetEntryAssembly());
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