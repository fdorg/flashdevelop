using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using PluginCore;

namespace ProjectManager.Projects
{
    public class ProjectReader : XmlTextReader
    {
        Project project;
        protected int version;

        public ProjectReader(string filename, Project project) : base(filename)
        {
            this.project = project;
            WhitespaceHandling = WhitespaceHandling.None;
        }

        protected Project Project { get { return project; } }

        public virtual Project ReadProject()
        {
            MoveToContent();
            ProcessRootNode();

            while (Read())
                ProcessNode(Name);

            Close();
            PostProcess();
            return project;
        }

        protected virtual void PostProcess()
        {
            if (version > 1) return;

            // import FD3 project
            if (project.OutputType == OutputType.Unknown)
                project.OutputType = project.MovieOptions.DefaultOutput(project.MovieOptions.Platform);

            else if (project.OutputType == OutputType.OtherIDE
                && (!String.IsNullOrEmpty(project.PreBuildEvent) || !String.IsNullOrEmpty(project.PostBuildEvent)))
                project.OutputType = OutputType.CustomBuild;
        }

        protected virtual void ProcessRootNode()
        {
            version = 1;
            int.TryParse(GetAttribute("version") ?? "1", out version);
        }

        protected virtual void ProcessNode(string name)
        {
            switch (name)
            {
                case "output": ReadOutputOptions(); break;
                case "classpaths": ReadClasspaths(); break;
                case "compileTargets": ReadCompileTargets(); break;
                case "hiddenPaths": ReadHiddenPaths(); break;
                case "preBuildCommand": ReadPreBuildCommand(); break;
                case "postBuildCommand": ReadPostBuildCommand(); break;
                case "options": ReadProjectOptions(); break;
                case "storage": ReadPluginStorage(); break;
            }
        }

        private void ReadPluginStorage()
        {
            if (IsEmptyElement)
            {
                Read();
                return;
            }

            ReadStartElement("storage");
            while (Name == "entry")
            {
                string key = GetAttribute("key");
                if (IsEmptyElement)
                {
                    Read();
                    continue;
                }

                Read();
                if (key != null) project.storage.Add(key, Value);
                Read();
                ReadEndElement();
            }
            ReadEndElement();
        }

        public void ReadOutputOptions()
        {
            ReadStartElement("output");
            while (Name == "movie")
            {
                MoveToFirstAttribute();
                switch (Name)
                {
                    case "disabled": 
                        project.OutputType = BoolValue ? OutputType.OtherIDE : OutputType.Application; 
                        break;
                    case "outputType":
                        if (Enum.IsDefined(typeof(OutputType), Value))
                            project.OutputType = (OutputType)Enum.Parse(typeof(OutputType), Value); 
                        break;
                    case "input": project.InputPath = OSPath(Value); break;
                    case "path": project.OutputPath = OSPath(Value); break;
                    case "fps": project.MovieOptions.Fps = IntValue; break;
                    case "width": project.MovieOptions.Width = IntValue; break;
                    case "height": project.MovieOptions.Height = IntValue; break;
                    case "version": project.MovieOptions.MajorVersion = IntValue; break;
                    case "minorVersion": project.MovieOptions.MinorVersion = IntValue; break;
                    case "platform": project.MovieOptions.Platform = Value; break;
                    case "background": project.MovieOptions.Background = Value; break;
                    case "preferredSDK": project.PreferredSDK = Value; break;
                }
                Read();
            }
            ReadEndElement();
        }

        public void ReadClasspaths()
        {
            ReadStartElement("classpaths");
            ReadPaths("class",project.Classpaths);
            ReadEndElement();
        }

        public void ReadCompileTargets()
        {
            ReadStartElement("compileTargets");
            ReadPaths("compile",project.CompileTargets);
            ReadEndElement();
        }

        public void ReadHiddenPaths()
        {
            ReadStartElement("hiddenPaths");
            ReadPaths("hidden",project.HiddenPaths);
            ReadEndElement();
        }

        public void ReadPreBuildCommand()
        {
            if (!IsEmptyElement)
            {
                ReadStartElement("preBuildCommand");
                project.PreBuildEvent = ReadString().Trim();
                ReadEndElement();
            }
        }

        public void ReadPostBuildCommand()
        {
            project.AlwaysRunPostBuild = Convert.ToBoolean(GetAttribute("alwaysRun"));

            if (!IsEmptyElement)
            {
                ReadStartElement("postBuildCommand");
                project.PostBuildEvent = ReadString().Trim();
                ReadEndElement();
            }
        }

        public void ReadProjectOptions()
        {
            ReadStartElement("options");
            while (Name == "option")
            {
                MoveToFirstAttribute();
                switch (Name)
                {
                    case "showHiddenPaths": project.ShowHiddenPaths = BoolValue; 
                        break;

                    case "testMovie":
                        // Be tolerant of unknown strings (older .fdp projects might have these)
                        List<string> acceptableValues  = new List<string>(Enum.GetNames(typeof(TestMovieBehavior)));
                        if (acceptableValues.Contains(Value)) project.TestMovieBehavior = (TestMovieBehavior)Enum.Parse(typeof(TestMovieBehavior), Value, true);
                        else project.TestMovieBehavior = TestMovieBehavior.NewTab;
                        break;

                    case "defaultBuildTargets":
                        if (!String.IsNullOrEmpty(Value.Trim()) && Value.IndexOf(",", StringComparison.Ordinal) > -1)
                        {
                            String[] cleaned = Value.Trim().Split(',').Select(x => x.Trim()).ToArray<String>();
                            project.MovieOptions.DefaultBuildTargets = cleaned;
                        }
                        break;

                    case "testMovieCommand": project.TestMovieCommand = Value;
                        break;
                    
                }
                Read();
            }
            ReadEndElement();
        }

        public bool BoolValue { get { return Convert.ToBoolean(Value); } }
        public int IntValue { get { return Convert.ToInt32(Value); } }

        public void ReadPaths(string pathNodeName, IAddPaths paths)
        {
            while (Name == pathNodeName)
            {
                paths.Add(OSPath(GetAttribute("path")));
                Read();
            }
        }

        protected string OSPath(string path)
        {
            if (path != null)
            {
                path = path.Replace('/', Path.DirectorySeparatorChar);
                path = path.Replace('\\', Path.DirectorySeparatorChar);
            }
            return path;
        }
    }
}
