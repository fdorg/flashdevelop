using System;
using System.Xml.Serialization;

namespace ScintillaNet.Configuration
{
    [Serializable()]
    public class UseKeyword : ConfigItem
    {
        [XmlAttribute()]
        public int key;

        [XmlAttribute("name")]
        public int name;

        [XmlAttribute("class")]
        public string cls;
        
    }
    
}
