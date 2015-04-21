using System;
using System.Runtime;
using System.Xml.Serialization;

namespace ScintillaNet.Configuration
{
    [SerializableAttribute()]
    public class UseKeyword : ConfigItem
    {
        [XmlAttributeAttribute()]
        public int key;

        [XmlAttributeAttribute("name")]
        public int name;

        [XmlAttributeAttribute("class")]
        public string cls;
        
    }
    
}
