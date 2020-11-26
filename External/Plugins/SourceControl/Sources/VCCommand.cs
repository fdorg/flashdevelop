using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SourceControl.Sources
{
    public abstract class VCCommand
    {
        protected VCCommand nextCommand;

        public VCCommand ContinueWith(VCCommand command)
        {
            if (nextCommand is null)
                nextCommand = command;
            else
                nextCommand.ContinueWith(command);
            return this;
        }

        public abstract void Run();

    }
}
