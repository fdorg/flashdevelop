using System;
using ASCompletion.Context;
using ASCompletion.Model;
using PluginCore;
using PluginCore.Helpers;
using ScintillaNet;
using ASCompletion.Completion;

namespace CodeRefactor.Commands
{
    class ExtractLocalVariableCommand
    {
        private string NewName;

        public ExtractLocalVariableCommand(string newName)
        {
            this.NewName = newName;
        }

        public void Execute()
        {
            ScintillaControl Sci = PluginBase.MainForm.CurrentDocument.SciControl;
            Sci.BeginUndoAction();
            try
            {
                ASGenerator.GenerateExtractVariable(Sci, NewName);
            }
            finally
            {
                Sci.EndUndoAction();
            }
        }
    }
}
