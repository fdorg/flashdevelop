// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System.Collections;
using System.IO;
using System.Text;
using System.Xml;

namespace ProjectManager.Projects
{
    public class ProjectWriter : XmlTextWriter
    {
        public ProjectWriter(Project project, string filename)
            : base(new FileStream(filename, File.Exists(filename) ? FileMode.Truncate : FileMode.CreateNew), Encoding.UTF8)
        {
            Project = project;
            Formatting = Formatting.Indented;
        }

        protected Project Project { get; }

        public void WriteProject()
        {
            // write while providing enough hooks for AS2ProjectWriter to exactly reproduce the
            // old .fdp format
            WriteStartDocument();
            WriteStartElement("project");
            WriteAttributeString("version", "2");
            OnAfterBeginProject();
            WriteOutputOptions();
            WriteClasspaths();
            OnAfterWriteClasspaths();
            WriteCompileTargets();
            OnAfterWriteCompileTargets();
            WriteHiddenPaths();
            WritePreBuildCommand();
            WritePostBuildCommand();
            WriteProjectOptions();
            WriteStorage();
            OnBeforeEndProject();
            WriteEndElement();
            WriteEndDocument();
        }

        protected virtual void OnAfterBeginProject() { }
        protected virtual void OnAfterWriteClasspaths() { }
        protected virtual void OnAfterWriteCompileTargets() { }
        protected virtual void OnBeforeEndProject() { }

        public void WriteOutputOptions()
        {
            WriteComment(" Output SWF options ");
            WriteStartElement("output");
            WriteOption("movie", "outputType", Project.OutputType);
            WriteOption("movie", "input", Project.InputPath);
            WriteOption("movie", "path", Project.OutputPath);
            WriteOption("movie", "fps", Project.MovieOptions.Fps);
            WriteOption("movie", "width", Project.MovieOptions.Width);
            WriteOption("movie", "height", Project.MovieOptions.Height);
            WriteOption("movie", "version", Project.MovieOptions.MajorVersion);
            WriteOption("movie", "minorVersion", Project.MovieOptions.MinorVersion);
            WriteOption("movie", "platform", Project.MovieOptions.Platform);
            WriteOption("movie", "background", Project.MovieOptions.Background);
            if (Project.PreferredSDK != null) WriteOption("movie", "preferredSDK", Project.PreferredSDK);
            WriteEndElement();
        }

        public void WriteClasspaths()
        {
            WriteComment(" Other classes to be compiled into your SWF ");
            WriteStartElement("classpaths");
            WritePaths(Project.Classpaths,"class");
            WriteEndElement();
        }

        public void WriteCompileTargets()
        {
            WriteComment(" Class files to compile (other referenced classes will automatically be included) ");
            WriteStartElement("compileTargets");
            WritePaths(Project.CompileTargets,"compile");
            WriteEndElement();          
        }

        public void WriteHiddenPaths()
        {
            WriteComment(" Paths to exclude from the Project Explorer tree ");
            WriteStartElement("hiddenPaths");
            WritePaths(Project.HiddenPaths,"hidden");
            WriteEndElement();          
        }

        public void WritePreBuildCommand()
        {
            WriteComment(" Executed before build ");
            WriteStartElement("preBuildCommand");
            if (Project.PreBuildEvent.Length > 0)
                WriteString(Project.PreBuildEvent);
            WriteEndElement();
        }

        public void WritePostBuildCommand()
        {
            WriteComment(" Executed after build ");
            WriteStartElement("postBuildCommand");
            WriteAttributeString("alwaysRun",Project.AlwaysRunPostBuild.ToString());
            if (Project.PostBuildEvent.Length > 0)
                WriteString(Project.PostBuildEvent);
            WriteEndElement();

        }

        public void WriteProjectOptions()
        {
            WriteComment(" Other project options ");
            WriteStartElement("options");
            WriteOption("showHiddenPaths",Project.ShowHiddenPaths);
            WriteOption("testMovie",Project.TestMovieBehavior);
            WriteOption("testMovieCommand", Project.TestMovieCommand ?? "");
            if (Project.MovieOptions.DefaultBuildTargets != null && Project.MovieOptions.DefaultBuildTargets.Length > 0)
            {
                WriteOption("defaultBuildTargets", string.Join(",", Project.MovieOptions.DefaultBuildTargets));
            }
            WriteEndElement();
        }

        private void WriteStorage()
        {
            WriteComment(" Plugin storage ");
            WriteStartElement("storage");
            foreach (string key in Project.storage.Keys)
            {
                string value = Project.storage[key];
                if (value is null) continue;
                WriteStartElement("entry");
                WriteAttributeString("key", key);
                WriteCData(value);
                WriteEndElement();
            }
            WriteEndElement();
        }

        public void WriteOption(string optionName, object optionValue) => WriteOption("option",optionName,optionValue);

        public void WriteOption(string nodeName, string optionName, object optionValue)
        {
            WriteStartElement(nodeName);
            WriteAttributeString(optionName,optionValue.ToString());
            WriteEndElement();
        }

        public void WritePaths(ICollection paths, string pathNodeName)
        {
            if (paths.Count > 0)
            {
                foreach (string path in paths)
                {
                    WriteStartElement(pathNodeName);
                    WriteAttributeString("path",path);
                    WriteEndElement();
                }
            }
            else WriteExample(pathNodeName,"path");
        }

        public void WriteExample(string nodeName, params string[] attributes)
        {
            StringBuilder example = new StringBuilder();
            example.Append(" example: <"+nodeName);
            foreach (string attribute in attributes)
                example.Append(" " + attribute + "=\"...\"");
            example.Append(" /> ");
            WriteComment(example.ToString());
        }
    }
}
