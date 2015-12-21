using System;
using System.Collections.Generic;
using ASCompletion.Completion;
using ASCompletion.Context;
using ASCompletion.Model;
using CodeRefactor.Commands;
using PluginCore;
using PluginCore.FRService;
using PluginCore.Managers;

namespace CodeRefactor.Provider
{
    class RenamingHelper
    {
        static readonly Queue<Rename> queue = new Queue<Rename>();
        static Rename currentCommand;

        internal static void AddToQueue(Rename rename)
        {
            queue.Enqueue(rename);

            if (currentCommand != null) return;
            currentCommand = rename;

            ASResult target = rename.Target;
            bool outputResults = rename.OutputResults;
            if (ASContext.Context.CurrentModel.haXe
                && target.Member != null
                && (target.Member.Flags & (FlagType.Getter | FlagType.Setter)) != 0)
            {
                string oldName = rename.OldName;
                string newName = rename.NewName;
                List<MemberModel> list = target.Member.Parameters;
                if (list[0].Name == "get") RenameMember(target.InClass, "get_" + oldName, "get_" + newName, outputResults);
                if (list[1].Name == "set") RenameMember(target.InClass, "set_" + oldName, "set_" + newName, outputResults);
            }

            if (outputResults) PluginBase.MainForm.CallCommand("PluginCommand", "ResultsPanel.ClearResults");
            ExecuteFirst();
        }

        static void RenameMember(ClassModel inClass, string name, string newName, bool outputResults)
        {
            List<MemberModel> members = inClass.Members.Items;
            for (int i = 0, length = members.Count; i < length; i++)
            {
                MemberModel member = members[i];
                if (member.Name == name)
                {
                    ASResult result = new ASResult();
                    ASComplete.FindMember(name, inClass, result, FlagType.Dynamic | FlagType.Function, 0);
                    if (result.Member == null) return;
                    new Rename(result, false, outputResults, newName);
                    break;
                }
            }
        }

        static void ExecuteFirst()
        {
            try
            {
                currentCommand = queue.Dequeue();
                currentCommand.OnRefactorComplete += OnRefactorComplete;
                currentCommand.Execute();
            }
            catch (Exception ex)
            {
                queue.Clear();
                currentCommand = null;
                ErrorManager.ShowError(ex);
            }
        }

        static void OnRefactorComplete(object sender, RefactorCompleteEventArgs<IDictionary<string, List<SearchMatch>>> e)
        {
            currentCommand.OnRefactorComplete -= OnRefactorComplete;
            if (queue.Count == 0)
            {
                if (currentCommand.OutputResults)
                {
                    PluginBase.MainForm.CallCommand("PluginCommand", "ResultsPanel.ShowResults");
                }
                currentCommand = null;
            }
            else ExecuteFirst();
        }
    }
}
