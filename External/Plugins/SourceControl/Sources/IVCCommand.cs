using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SourceControl.Sources
{
    public interface IVCCommand
    {
        IVCCommand ContinueWith(IVCCommand command);

        void Run();
    }
}
