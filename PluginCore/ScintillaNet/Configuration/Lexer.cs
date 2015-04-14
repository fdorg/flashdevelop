using System;
using System.Runtime;
using System.Xml.Serialization;

namespace ScintillaNet.Configuration
{
    [SerializableAttribute()]
    public class Lexer : ConfigItem
    {
        [XmlAttributeAttribute("key")]
        public int key;
        
        [XmlAttributeAttribute("name")]
        public string name;

        [XmlAttributeAttribute("style-bits")]
        public int stylebits;
        
    }
    
}
