using System.Collections.Generic;
using ASCompletion.Completion;
using ASCompletion.Model;
using PluginCore;

namespace CodeRefactor.Commands
{
    public class DelegateMethods
    {
        readonly ASResult result;
        readonly Dictionary<MemberModel, ClassModel> selectedMembers;

        public DelegateMethods(ASResult result, Dictionary<MemberModel, ClassModel> selectedMembers)
        {
            this.result = result;
            this.selectedMembers = selectedMembers;
        }

        public void Execute()
        {
            var sci = PluginBase.MainForm.CurrentDocument.SciControl;
            ASGenerator.GenerateDelegateMethods(sci, result.Member, selectedMembers, result.Type, result.InClass);
        }
    }
}