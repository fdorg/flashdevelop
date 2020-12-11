// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Xml.Serialization;

namespace ScintillaNet.Configuration
{
    [Serializable]
    public class UseKeyword : ConfigItem
    {
        [XmlAttribute]
        public int key;

        [XmlAttribute("name")]
        public int name;

        [XmlAttribute("class")]
        public string cls;
    }
}