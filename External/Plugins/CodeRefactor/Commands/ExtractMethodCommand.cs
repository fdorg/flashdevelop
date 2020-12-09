using ASCompletion.Completion;
using PluginCore;

namespace CodeRefactor.Commands
{
    public class ExtractMethodCommand
    {
        readonly string NewName;

        public ExtractMethodCommand(string newName) => NewName = newName;

        public void Execute()
        {
            var sci = PluginBase.MainForm.CurrentDocument?.SciControl;
            if (sci is null) return;
            sci.BeginUndoAction();
            try
            {
                ASGenerator.GenerateExtractMethod(sci, NewName);
            }
            finally
            {
                sci.EndUndoAction();
            }
        }
    }
}