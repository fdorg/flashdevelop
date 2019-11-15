// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Xml;
using System.Text;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace ASDocGen.Objects
{
    [Serializable]
    [XmlRoot("appSettings")]
    public class Settings
    {
        public String asdocLocation;
        public String as2apiLocation;
        public Boolean copyCustomFiles;
        
        public Settings()
        {
            this.asdocLocation = "";
            this.as2apiLocation = "$(AppDir)\\Tools";
            this.copyCustomFiles = true;
        }

    }

}
