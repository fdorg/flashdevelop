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
        public FileMappingRequest(string[] files) => Files = files;

        public string[] Files { get; set; }

        public FileMapping Mapping { get; set; } = new FileMapping();
    }
}