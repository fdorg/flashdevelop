// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
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
