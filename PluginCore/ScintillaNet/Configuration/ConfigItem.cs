using System;

namespace ScintillaNet.Configuration
{
    [Serializable]
    public class ConfigItem
    {
        [NonSerialized]
        protected ConfigFile _parent;

        public virtual void init(ConfigurationUtility utility, ConfigFile theParent)
        {
            if (theParent is null) _parent = (ConfigFile)this;
            else _parent = theParent;
        }
    }
}