using System;
using System.Runtime;
using System.Xml.Serialization;

namespace ScintillaNet.Configuration
{
    [SerializableAttribute()]
    public class include : ConfigItem
    {
        [XmlAttributeAttribute()]
        public string file;
        
    }
    
}
