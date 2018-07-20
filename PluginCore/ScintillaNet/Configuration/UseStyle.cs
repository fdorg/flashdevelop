// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using System;
using System.Xml.Serialization;

namespace ScintillaNet.Configuration
{
    [Serializable()]
    public class UseStyle : StyleClass
    {
        [XmlAttribute("class")]
        public string cls;

        [XmlAttribute()]
        public int key;

        public override void init(ConfigurationUtility utility, ConfigFile theParent)
        {
            base.init(utility, theParent);
            if (!string.IsNullOrEmpty(cls)) inheritstyle = cls;
        }
        
    }
    
}
