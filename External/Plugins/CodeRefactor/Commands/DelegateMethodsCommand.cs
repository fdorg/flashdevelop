using System;
using System.Collections.Generic;
using ASCompletion.Completion;
using ASCompletion.Context;
using ASCompletion.Model;
using PluginCore;
using ScintillaNet;

namespace CodeRefactor.Commands
{
    class DelegateMethodsCommand
    {
        private ScintillaControl Sci;
        private ASResult result;
        private Dictionary<MemberModel, ClassModel> selectedMembers;

        public DelegateMethodsCommand(ASResult result, Dictionary<MemberModel, ClassModel> selectedMembers)
        {
            this.result = result;
            this.selectedMembers = selectedMembers;
        }

        public void Execute()
        {
            Sci = PluginBase.MainForm.CurrentDocument.SciControl;

            IASContext context = ASContext.Context;
            Int32 pos = Sci.CurrentPos;

            ASGenerator.GenerateDelegateMethods(Sci, result.Member, selectedMembers, result.Type, result.InClass);
        }
    }
}
