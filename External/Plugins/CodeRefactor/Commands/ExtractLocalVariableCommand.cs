using PluginCore;
using ScintillaNet;
using ASCompletion.Completion;

namespace CodeRefactor.Commands
{
    class ExtractLocalVariableCommand
    {
        private readonly string _newName;

        public ExtractLocalVariableCommand(string newName)
        {
            _newName = newName;
        }

        public void Execute()
        {
            ScintillaControl sci = PluginBase.MainForm.CurrentDocument.SciControl;
            sci.BeginUndoAction();
            try
            {
                ASGenerator.GenerateExtractVariable(sci, _newName);
            }
            finally
            {
                sci.EndUndoAction();
            }
        }
    }
}