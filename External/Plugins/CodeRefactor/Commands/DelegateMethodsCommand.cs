using System.Collections.Generic;
using ASCompletion.Completion;
using ASCompletion.Model;
using PluginCore;
using ScintillaNet;

namespace CodeRefactor.Commands
{
    class DelegateMethodsCommand
    {
        private ScintillaControl _sci;
        private readonly ASResult _result;
        private readonly Dictionary<MemberModel, ClassModel> _selectedMembers;

        public DelegateMethodsCommand(ASResult result, Dictionary<MemberModel, ClassModel> selectedMembers)
        {
            _result = result;
            _selectedMembers = selectedMembers;
        }

        public void Execute()
        {
            _sci = PluginBase.MainForm.CurrentDocument.SciControl;
            ASGenerator.GenerateDelegateMethods(_sci, _result.Member, _selectedMembers, _result.Type, _result.InClass);
        }
    }
}