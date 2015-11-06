using System;
using System.Collections.Generic;
using System.IO;
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
        public string SendKeysName { get { return "${(}"+Name+"{)}"; } }
        public string FormattedName { get { return "$("+Name+")"; } }
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
            List<BuildEventInfo> infos = new List<BuildEventInfo>();

            infos.Add(new BuildEventInfo("FDBuild", FDBuild));
            infos.Add(new BuildEventInfo("ToolsDir", ToolsDir));
            infos.Add(new BuildEventInfo("TimeStamp", DateTime.Now.ToString("g")));
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

        public string FDBuildDir { get { return Path.GetDirectoryName(FDBuild); } }
        public string ToolsDir { get { return Path.GetDirectoryName(FDBuildDir); } }

        public string FDBuild
        {
            get
            {
                string url = Assembly.GetEntryAssembly().GetName().CodeBase;
                Uri uri = new Uri(url);
                
                // special behavior if we're running in flashdevelop.exe
                if (Path.GetFileName(uri.LocalPath).ToLower() == DistroConfig.DISTRIBUTION_NAME.ToLower() + ".exe")
                {
                    string startupDir = Path.GetDirectoryName(uri.LocalPath);
                    string local = Path.Combine(Path.GetDirectoryName(uri.LocalPath), ".local");
                    if (!File.Exists(local))
                    {
                        String appDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                        startupDir = Path.Combine(appDir, DistroConfig.DISTRIBUTION_NAME);
                    }
                    string toolsDir = Path.Combine(startupDir, "Tools");
                    string fdbuildDir = Path.Combine(toolsDir, "fdbuild");
                    return Path.Combine(fdbuildDir, "fdbuild.exe");
                }
                else return uri.LocalPath;
            }
        }
    }
}
