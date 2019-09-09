// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
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

        public override string ToString() => string.IsNullOrEmpty(Uri) ? "New Namespace" : Uri;
    }
}