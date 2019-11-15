// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;

namespace ScintillaNet.Configuration
{
    [Serializable()]
    public class ConfigItem
    {
        [NonSerialized()]
        protected ConfigFile _parent;

        public virtual void init(ConfigurationUtility utility, ConfigFile theParent)
        {
            if (theParent is null) _parent = (ConfigFile)this;
            else _parent = theParent;
        }
        
    }

}
