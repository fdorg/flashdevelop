// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Xml.Serialization;

namespace ScintillaNet.Configuration
{
    [Serializable]
    public class UseStyle : StyleClass
    {
        [XmlAttribute("class")]
        public string cls;

        [XmlAttribute]
        public int key;

        public override void init(ConfigurationUtility utility, ConfigFile theParent)
        {
            base.init(utility, theParent);
            if (!string.IsNullOrEmpty(cls)) inheritstyle = cls;
        }
    }
}