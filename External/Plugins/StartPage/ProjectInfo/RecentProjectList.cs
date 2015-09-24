using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace StartPage.ProjectInfo
{
    public class RecentProjectList : List<RecentProject>
    {
        private XmlSerializer xmlSerializer;

        public RecentProjectList()
        {
            this.xmlSerializer = new XmlSerializer(this.GetType());
        }
        public RecentProjectList(List<String> recentProjects)
        {
            this.Update(recentProjects);
        }

        /// <summary>
        /// Updates the recent project list from list of paths
        /// </summary>
        public void Update(List<String> recentProjects)
        {
            this.Clear();
            foreach (String recentProject in recentProjects)
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
        public String Path = "";
        public String Name = "";
        public String Type = "";
        public String Created = "Error getting file information.";
        public String Modified = "Error getting file information.";

        public RecentProject() {}
        public RecentProject(String path)
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
