using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;

namespace ProjectManager.Projects.AS3
{
    [Serializable]
    public class MxmlNamespace
    {

        [Category("Location")]
        [Editor(typeof(FileNameEditor), typeof(UITypeEditor))]
        [DisplayName("Manifest File")]
        public string Manifest { get; set; }

        [Category("Properties")]
        public string Uri { get; set; }

        public override string ToString()
        {
            return String.IsNullOrEmpty(Uri) ? "New Namespace" : Uri;
        }
    }
}
