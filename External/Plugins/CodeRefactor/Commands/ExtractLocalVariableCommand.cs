using ASCompletion.Completion;
using PluginCore;
using ScintillaNet;

namespace CodeRefactor.Commands
{
    class ExtractLocalVariableCommand
    {
        private readonly string NewName;

        public ExtractLocalVariableCommand(string newName)
        {
            this.NewName = newName;
        }

        public void Execute()
        {
            ScintillaControl sci = PluginBase.MainForm.CurrentDocument.SciControl;
            sci.BeginUndoAction();
            try
            {
                ASGenerator.GenerateExtractVariable(sci, NewName);
            }
            finally
            {
                sci.EndUndoAction();
            }
        }
    }
}