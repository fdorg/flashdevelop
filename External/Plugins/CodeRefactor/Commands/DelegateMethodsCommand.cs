using ASCompletion.Completion;
using ASCompletion.Model;
using PluginCore;
using ScintillaNet;
using System.Collections.Generic;

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