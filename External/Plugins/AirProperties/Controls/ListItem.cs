using System;
using System.Text;
using System.Collections.Generic;

namespace AirProperties
{
    class ListItem
    {
        public ListItem(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; set; }

        public string Value { get; set; }

        public override string ToString()
        {
            return Name;
        }

    }

}
