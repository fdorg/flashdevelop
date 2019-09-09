using System;
using System.Collections.Generic;
using System.Text;
using SOReader.Sol.AMF.DataType;

namespace SharedObjectReader.Controls
{
    public interface IAMFDisplayPanel
    {
        void Populate(string name, IAMFBase element);
    }
}
