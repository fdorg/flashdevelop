using System;
using System.Collections.Generic;
using System.Text;

namespace SwfOp.Data.Tags
{
    class FrameTag : BaseTag
    {
        string _name;

        public FrameTag(string name)
        {
            _tagCode = (int)TagCodeEnum.FrameLabel;
            _name = name;
        }

        public string name 
        {
            get { return _name; }
            set {_name = value; }
        }
    }
}
