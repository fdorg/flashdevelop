using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace ProjectManager.Projects
{
    public class ProjectReader : XmlTextReader
    {
        protected int version;

        public ProjectReader(string filename, Project project) : base(new FileStream(filename,FileMode.Open,FileAccess.Read))
        {
            Project = project;
            WhitespaceHandling = WhitespaceHandling.None;
        }

        protected Project Project { get; }

        public virtual Project ReadProject()
        {
            MoveToContent();
            ProcessRootNode();

            while (Read())
                ProcessNode(Name);

            Close();
            PostProcess();
            return Project;
        }

        protected virtual void PostProcess()
        {
            if (version > 1) return;

            // import FD3 project
            if (Project.OutputType == OutputType.Unknown)
                Project.OutputType = Project.MovieOptions.DefaultOutput(Project.MovieOptions.Platform);

            else if (Project.OutputType == OutputType.OtherIDE
                && (!string.IsNullOrEmpty(Project.PreBuildEvent) || !string.IsNullOrEmpty(Project.PostBuildEvent)))
                Project.OutputType = OutputType.CustomBuild;
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
                if (key != null) Project.storage.Add(key, Value);
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
                        Project.OutputType = BoolValue ? OutputType.OtherIDE : OutputType.Application; 
                        break;
                    case "outputType":
                        if (Enum.IsDefined(typeof(OutputType), Value))
                            Project.OutputType = (OutputType)Enum.Parse(typeof(OutputType), Value); 
                        break;
                    case "input": Project.InputPath = OSPath(Value); break;
                    case "path": Project.OutputPath = OSPath(Value); break;
                    case "fps": Project.MovieOptions.Fps = IntValue; break;
                    case "width": Project.MovieOptions.Width = IntValue; break;
                    case "height": Project.MovieOptions.Height = IntValue; break;
                    case "version": Project.MovieOptions.MajorVersion = IntValue; break;
                    case "minorVersion": Project.MovieOptions.MinorVersion = IntValue; break;
                    case "platform": Project.MovieOptions.Platform = Value; break;
                    case "background": Project.MovieOptions.Background = Value; break;
                    case "preferredSDK": Project.PreferredSDK = Value; break;
                }
                Read();
            }
            ReadEndElement();
        }

        public void ReadClasspaths()
        {
            ReadStartElement("classpaths");
            ReadPaths("class",Project.Classpaths);
            ReadEndElement();
        }

        public void ReadCompileTargets()
        {
            ReadStartElement("compileTargets");
            ReadPaths("compile",Project.CompileTargets);
            ReadEndElement();
        }

        public void ReadHiddenPaths()
        {
            ReadStartElement("hiddenPaths");
            ReadPaths("hidden",Project.HiddenPaths);
            ReadEndElement();
        }

        public void ReadPreBuildCommand()
        {
            if (!IsEmptyElement)
            {
                ReadStartElement("preBuildCommand");
                Project.PreBuildEvent = ReadString().Trim();
                ReadEndElement();
            }
        }

        public void ReadPostBuildCommand()
        {
            Project.AlwaysRunPostBuild = Convert.ToBoolean(GetAttribute("alwaysRun"));

            if (!IsEmptyElement)
            {
                ReadStartElement("postBuildCommand");
                Project.PostBuildEvent = ReadString().Trim();
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
                    case "showHiddenPaths": Project.ShowHiddenPaths = BoolValue; 
                        break;

                    case "testMovie":
                        // Be tolerant of unknown strings (older .fdp projects might have these)
                        List<string> acceptableValues  = new List<string>(Enum.GetNames(typeof(TestMovieBehavior)));
                        if (acceptableValues.Contains(Value)) Project.TestMovieBehavior = (TestMovieBehavior)Enum.Parse(typeof(TestMovieBehavior), Value, true);
                        else Project.TestMovieBehavior = TestMovieBehavior.NewTab;
                        break;

                    case "defaultBuildTargets":
                        if (!string.IsNullOrEmpty(Value.Trim()) && Value.Contains(","))
                        {
                            string[] cleaned = Value.Trim().Split(',').Select(x => x.Trim()).ToArray<string>();
                            Project.MovieOptions.DefaultBuildTargets = cleaned;
                        }
                        break;

                    case "testMovieCommand": Project.TestMovieCommand = Value;
                        break;
                    
                }
                Read();
            }
            ReadEndElement();
        }

        public bool BoolValue => Convert.ToBoolean(Value);

        public int IntValue => Convert.ToInt32(Value);

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