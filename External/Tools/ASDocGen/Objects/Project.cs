using System;
using System.Xml;
using System.Text;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace ASDocGen.Objects
{
    [Serializable]
    [XmlRoot("docProject")]
    public class Project
    {
        public String pageTitle;
        public String classPaths;
        public String sourcesList;
        public String outputDirectory;
        public Int32 activeCompiler;
        public String extraOptions;

        public Project()
        {
            this.pageTitle = "";
            this.classPaths = "";
            this.sourcesList = "";
            this.outputDirectory = "";
            this.activeCompiler = 0;
            this.extraOptions = "";
        }

    }

}
