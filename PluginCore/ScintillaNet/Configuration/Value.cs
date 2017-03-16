// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using System;
using System.Xml.Serialization;

namespace ScintillaNet.Configuration
{
    [Serializable()]
    public class Value : ConfigItem
    {
        [XmlAttribute()]
        public string name;

        [XmlText()]
        public string val;
        
    }
    
}
