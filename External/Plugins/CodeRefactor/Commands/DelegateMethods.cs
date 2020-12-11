// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
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
            var sci = PluginBase.MainForm.CurrentDocument?.SciControl;
            if (sci is null) return;
            ASGenerator.GenerateDelegateMethods(sci, result.Member, selectedMembers, result.Type, result.InClass);
        }
    }
}