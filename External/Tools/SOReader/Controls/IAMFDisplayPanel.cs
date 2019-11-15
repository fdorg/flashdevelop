// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
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
