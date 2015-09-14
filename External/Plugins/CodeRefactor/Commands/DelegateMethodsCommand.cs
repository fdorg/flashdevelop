using System.Collections.Generic;
using ASCompletion.Completion;
using ASCompletion.Model;
using PluginCore;
using ScintillaNet;

namespace CodeRefactor.Commands
{
    class DelegateMethodsCommand
    {
        private ASResult result;
        private Dictionary<MemberModel, ClassModel> selectedMembers;

        public DelegateMethodsCommand(ASResult result, Dictionary<MemberModel, ClassModel> selectedMembers)
        {
            this.result = result;
            this.selectedMembers = selectedMembers;
        }

        public void Execute()
        {
            ScintillaControl Sci = PluginBase.MainForm.CurrentDocument.SciControl;
            ASGenerator.GenerateDelegateMethods(Sci, result.Member, selectedMembers, result.Type, result.InClass);
        }
    }
}