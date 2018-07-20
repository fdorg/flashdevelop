// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
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
