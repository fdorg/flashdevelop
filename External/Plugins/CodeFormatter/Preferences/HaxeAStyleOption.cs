using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeFormatter.Preferences
{
    [Serializable]
    public class HaxeAStyleOption
    {
        private string name;
        private object value;

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }

        public object Value
        {
            get
            {
                return value;
            }
            set
            {
                this.value = value;
            }
        }

        public HaxeAStyleOption(string opt, object value)
        {
            this.name = opt;
            this.value = value;
        }

        public HaxeAStyleOption(string opt) : this(opt, null)
        {
        }

        override public string ToString()
        {
            if (this.Value != null)
            {
                return this.Name + "=" + this.Value.ToString();
            }

            return this.Name;
        }
    }

    [Serializable]
    public class HaxeAStyleOptions : List<HaxeAStyleOption>
    {
        public HaxeAStyleOptions(IEnumerable<HaxeAStyleOption> e) : base(e)
        {
        }

        public HaxeAStyleOptions() : base()
        {
        }

        public HaxeAStyleOption Find(string name)
        {
            return this.Find(o => name == o.Name);
        }

        public bool Exists(string name)
        {
            return this.Exists(o => name == o.Name);
        }

        public string[] ToStringArray()
        {
            return this.Select(o => o.ToString()).ToArray();
        }
    }
}
