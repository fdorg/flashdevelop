// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using System.Collections.Generic;
using ASCompletion.Completion;
using ASCompletion.Model;
using PluginCore;
using ScintillaNet;

namespace CodeRefactor.Commands
{
    public class DelegateMethodsCommand
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