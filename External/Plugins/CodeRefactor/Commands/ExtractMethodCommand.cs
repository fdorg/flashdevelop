using ASCompletion.Completion;
using PluginCore;
using ScintillaNet;

namespace CodeRefactor.Commands
{
    class ExtractMethodCommand
    {
        private string NewName;

        public ExtractMethodCommand(string newName)
        {
            this.NewName = newName;
        }

        public void Execute()
        {
            ScintillaControl Sci = PluginBase.MainForm.CurrentDocument.SciControl;
            Sci.BeginUndoAction();
            try
            {
                ASGenerator.GenerateExtractMethod(Sci, NewName);
            }
            finally
            {
                Sci.EndUndoAction();
            }
        }

    }

}