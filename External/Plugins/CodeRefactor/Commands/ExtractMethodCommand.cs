using ASCompletion.Completion;
using PluginCore;
using ScintillaNet;

namespace CodeRefactor.Commands
{
    class ExtractMethodCommand
    {
        private readonly string _newName;

        public ExtractMethodCommand(string newName)
        {
            _newName = newName;
        }

        public void Execute()
        {
            ScintillaControl sci = PluginBase.MainForm.CurrentDocument.SciControl;
            sci.BeginUndoAction();
            try
            {
                ASGenerator.GenerateExtractMethod(sci, _newName);
            }
            finally
            {
                sci.EndUndoAction();
            }
        }
    }
}