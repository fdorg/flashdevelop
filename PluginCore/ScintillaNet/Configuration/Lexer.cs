// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using System;
using System.Xml.Serialization;

namespace ScintillaNet.Configuration
{
    [Serializable()]
    public class Lexer : ConfigItem
    {
        [XmlAttribute("key")]
        public int key;
        
        [XmlAttribute("name")]
        public string name;

        [XmlAttribute("style-bits")]
        public int stylebits;
        
    }
    
}
