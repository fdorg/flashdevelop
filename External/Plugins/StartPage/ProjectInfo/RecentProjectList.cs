using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace StartPage.ProjectInfo
{
    public class RecentProjectList : List<RecentProject>
    {
        private readonly XmlSerializer xmlSerializer;

        public RecentProjectList()
        {
            this.xmlSerializer = XmlSerializer.FromTypes(new[]{this.GetType()})[0];
        }
        public RecentProjectList(List<string> recentProjects)
        {
            this.Update(recentProjects);
        }

        /// <summary>
        /// Updates the recent project list from list of paths
        /// </summary>
        public void Update(List<string> recentProjects)
        {
            this.Clear();
            foreach (string recentProject in recentProjects)
            {
                this.Add(new RecentProject(recentProject));
            }
        }

        /// <summary>
        /// Returns recent project list as xml string
        /// </summary>
        public string ToXml()
        {
            StringWriter sw = new StringWriter();
            this.xmlSerializer.Serialize(sw, this);
            return sw.ToString();
        }

    }

    public class RecentProject
    {
        public string Path = "";
        public string Name = "";
        public string Type = "";
        public string Created = "Error getting file information.";
        public string Modified = "Error getting file information.";

        public RecentProject(string path)
        {
            FileInfo fileInfo = new FileInfo(path);
            this.Path = path; // Store the path...
            this.Name = fileInfo.Name.Substring(0, fileInfo.Name.Length - fileInfo.Extension.Length);
            if (this.Name.Length == 0) // FlexBuilder project are "path/to/.actionscriptProperties"
            {
                this.Name = System.IO.Path.GetFileNameWithoutExtension(System.IO.Path.GetDirectoryName(path));
            }
            this.Type = fileInfo.Extension;
            if (fileInfo.Exists)
            {
                this.Created = fileInfo.CreationTime.ToString();
                this.Modified = fileInfo.LastWriteTime.ToString();
            }
        }

    }

}
