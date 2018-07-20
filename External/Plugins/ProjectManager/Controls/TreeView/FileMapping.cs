using System.Collections.Generic;

namespace ProjectManager.Controls.TreeView
{
    public class FileMapping : Dictionary<string, string>
    {
        public void Map(string file, string parent)
        {
            if (!ContainsKey(file))
                Add(file, parent);
        }
    }

    public class FileMappingRequest
    {
        string[] files;
        FileMapping mapping;

        public FileMappingRequest(string[] files)
        {
            this.files = files;
            this.mapping = new FileMapping();
        }

        public string[] Files
        {
            get { return files; }
            set { files = value; }
        }

        public FileMapping Mapping
        {
            get { return mapping; }
            set { mapping = value; }
        }
    }
}
