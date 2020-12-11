// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace StartPage.ProjectInfo
{
    public class RecentProjectList : List<RecentProject>
    {
        readonly XmlSerializer xmlSerializer;

        public RecentProjectList()
        {
            xmlSerializer = XmlSerializer.FromTypes(new[]{GetType()})[0];
        }
        public RecentProjectList(List<string> recentProjects)
        {
            Update(recentProjects);
        }

        /// <summary>
        /// Updates the recent project list from list of paths
        /// </summary>
        public void Update(List<string> recentProjects)
        {
            Clear();
            foreach (string recentProject in recentProjects)
            {
                Add(new RecentProject(recentProject));
            }
        }

        /// <summary>
        /// Returns recent project list as xml string
        /// </summary>
        public string ToXml()
        {
            var writer = new StringWriter();
            xmlSerializer.Serialize(writer, this);
            return writer.ToString();
        }
    }

    public class RecentProject
    {
        public string Path = "";
        public string Name = "";
        public string Type = "";
        public string Created = "Error getting file information.";
        public string Modified = "Error getting file information.";

        public RecentProject() { }
        public RecentProject(string path)
        {
            var fileInfo = new FileInfo(path);
            Path = path; // Store the path...
            Name = fileInfo.Name.Substring(0, fileInfo.Name.Length - fileInfo.Extension.Length);
            if (Name.Length == 0) // FlexBuilder project are "path/to/.actionscriptProperties"
            {
                Name = System.IO.Path.GetFileNameWithoutExtension(System.IO.Path.GetDirectoryName(path));
            }
            Type = fileInfo.Extension;
            if (fileInfo.Exists)
            {
                Created = fileInfo.CreationTime.ToString();
                Modified = fileInfo.LastWriteTime.ToString();
            }
        }
    }
}