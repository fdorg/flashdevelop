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
            _parent = theParent switch
            {
                null => (ConfigFile) this,
                _ => theParent
            };
        }
    }
}