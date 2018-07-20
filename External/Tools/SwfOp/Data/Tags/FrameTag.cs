// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
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
