// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using ASCompletion.Completion;
using PluginCore;
using ScintillaNet;

namespace CodeRefactor.Commands
{
    public class ExtractMethodCommand
    {
        private readonly string NewName;

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