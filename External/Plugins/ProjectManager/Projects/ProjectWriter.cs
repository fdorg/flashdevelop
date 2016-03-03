using System;
using System.Collections;
using System.Text;
using System.Xml;

namespace ProjectManager.Projects
{
    public class ProjectWriter : XmlTextWriter
    {
        Project project;

        public ProjectWriter(Project project, string filename) : base(filename,Encoding.UTF8)
        {
            this.project = project;
            this.Formatting = Formatting.Indented;
        }

        protected Project Project { get { return project; } }

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
            WriteOption("movie", "outputType", project.OutputType);
            WriteOption("movie", "input", project.InputPath);
            WriteOption("movie", "path", project.OutputPath);
            WriteOption("movie", "fps", project.MovieOptions.Fps);
            WriteOption("movie", "width", project.MovieOptions.Width);
            WriteOption("movie", "height", project.MovieOptions.Height);
            WriteOption("movie", "version", project.MovieOptions.MajorVersion);
            WriteOption("movie", "minorVersion", project.MovieOptions.MinorVersion);
            WriteOption("movie", "platform", project.MovieOptions.Platform);
            WriteOption("movie", "background", project.MovieOptions.Background);
            if (project.PreferredSDK != null) WriteOption("movie", "preferredSDK", project.PreferredSDK);
            WriteEndElement();
        }

        public void WriteClasspaths()
        {
            WriteComment(" Other classes to be compiled into your SWF ");
            WriteStartElement("classpaths");
            WritePaths(project.Classpaths,"class");
            WriteEndElement();
        }

        public void WriteCompileTargets()
        {
            WriteComment(" Class files to compile (other referenced classes will automatically be included) ");
            WriteStartElement("compileTargets");
            WritePaths(project.CompileTargets,"compile");
            WriteEndElement();          
        }

        public void WriteHiddenPaths()
        {
            WriteComment(" Paths to exclude from the Project Explorer tree ");
            WriteStartElement("hiddenPaths");
            WritePaths(project.HiddenPaths,"hidden");
            WriteEndElement();          
        }

        public void WritePreBuildCommand()
        {
            WriteComment(" Executed before build ");
            WriteStartElement("preBuildCommand");
            if (project.PreBuildEvent.Length > 0)
                WriteString(project.PreBuildEvent);
            WriteEndElement();
        }

        public void WritePostBuildCommand()
        {
            WriteComment(" Executed after build ");
            WriteStartElement("postBuildCommand");
            WriteAttributeString("alwaysRun",project.AlwaysRunPostBuild.ToString());
            if (project.PostBuildEvent.Length > 0)
                WriteString(project.PostBuildEvent);
            WriteEndElement();

        }

        public void WriteProjectOptions()
        {
            WriteComment(" Other project options ");
            WriteStartElement("options");
            WriteOption("showHiddenPaths",project.ShowHiddenPaths);
            WriteOption("testMovie",project.TestMovieBehavior);
            WriteOption("testMovieCommand", project.TestMovieCommand ?? "");
            if (project.MovieOptions.DefaultBuildTargets != null && project.MovieOptions.DefaultBuildTargets.Length > 0)
            {
                WriteOption("defaultBuildTargets", String.Join(",", project.MovieOptions.DefaultBuildTargets));
            }
            WriteEndElement();
        }

        private void WriteStorage()
        {
            WriteComment(" Plugin storage ");
            WriteStartElement("storage");
            foreach (string key in project.storage.Keys)
            {
                string value = project.storage[key];
                if (value == null) continue;
                WriteStartElement("entry");
                WriteAttributeString("key", key);
                WriteCData(value);
                WriteEndElement();
            }
            WriteEndElement();
        }

        public void WriteOption(string optionName, object optionValue)
        {
            WriteOption("option",optionName,optionValue);
        }

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
