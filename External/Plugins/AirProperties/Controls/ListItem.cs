using System;
using System.Text;
using System.Collections.Generic;

namespace AirProperties
{
    class ListItem
    {
        private string _name;
        private string _value;

        public ListItem(string name, string value)
        {
            this._name = name;
            this._value = value;
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string Value
        {
            get { return _value; }
            set { this._value = value; }
        }

        public override string ToString()
        {
            return Name;
        }

    }

}
