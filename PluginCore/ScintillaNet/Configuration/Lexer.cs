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
