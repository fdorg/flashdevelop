using System;
using System.Collections.Generic;
using System.Linq;
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
        static readonly List<Rename> queue = new List<Rename>();
        static Rename currentCommand;
        
        public static void AddToQueue(Rename rename)
        {
            queue.Add(rename);
            if (queue.Count == 1)
            {
                var target = rename.Target;
                bool outputResults = rename.OutputResults;
                if (ASContext.Context.CurrentModel.haXe && target.Member != null
                    && (target.Member.Flags & (FlagType.Getter | FlagType.Setter)) != 0)
                {
                    string oldName = rename.OldName;
                    string newName = rename.NewName;
                    List<MemberModel> list = target.Member.Parameters;
                    if (list[0].Name == "get") RenameMember(target.InClass, "get_" + oldName, "get_" + newName, outputResults);
                    if (list[1].Name == "set") RenameMember(target.InClass, "set_" + oldName, "set_" + newName, outputResults);
                }
                if (currentCommand == null)
                {
                    if (outputResults) PluginBase.MainForm.CallCommand("PluginCommand", "ResultsPanel.ClearResults");
                    ExecuteFirst();
                }
            }
        }
        
        static void RenameMember(ClassModel inClass, string name, string newName, bool outputResults)
        {
            MemberModel m = inClass.Members.Items.FirstOrDefault(it => it.Name == name);
            if (m == null) return;
            ASResult result = new ASResult();
            ASComplete.FindMember(name, inClass, result, FlagType.Dynamic | FlagType.Function, 0);
            if (result.Member == null) return;
            new Rename(result, false, outputResults, newName);
        }

        static void ExecuteFirst()
        {
            try
            {
                currentCommand = queue[0];
                queue.RemoveAt(0);
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
                    PluginBase.MainForm.CallCommand("PluginCommand", "ResultsPanel.ShowResults");

                currentCommand = null;
            }
            else ExecuteFirst();
        }
    }
}