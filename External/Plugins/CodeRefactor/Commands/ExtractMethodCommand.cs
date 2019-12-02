// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using ASCompletion.Completion;
using PluginCore;

namespace CodeRefactor.Commands
{
    public class ExtractMethodCommand
    {
        private readonly string NewName;

        public ExtractMethodCommand(string newName)
        {
            NewName = newName;
        }

        public void Execute()
        {
            var sci = PluginBase.MainForm.CurrentDocument.SciControl;
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