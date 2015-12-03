using CodeRefactor.Commands;
using PluginCore.FRService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using ASCompletion.Completion;
using ASCompletion.Context;
using ASCompletion.Model;
using PluginCore.Localization;
using PluginCore.Managers;
using ProjectManager.Helpers;

namespace CodeRefactor.Provider
{
    class RenamingHelper
    {
        static readonly List<Rename> queue = new List<Rename>();
        static Rename currentCommand;

        public static void AddToQueue(ASResult target)
        {
            AddToQueue(target, true);
        }
        public static void AddToQueue(ASResult target, bool outputResults)
        {
            string originalName = RefactoringHelper.GetRefactorTargetName(target);
            string label = TextHelper.GetString("Label.NewName");
            string title = string.Format(TextHelper.GetString("Title.RenameDialog"), originalName);
            LineEntryDialog askName = new LineEntryDialog(title, label, originalName);
            if (askName.ShowDialog() == DialogResult.OK)
            {
                string newName = askName.Line.Trim();
                if (newName.Length == 0 || newName == originalName) return;
                queue.Add(new Rename(target, outputResults, newName));
                if (ASContext.Context.CurrentModel.haXe && target.Member != null &&
                    (target.Member.Flags & (FlagType.Getter | FlagType.Setter)) > 0)
                {
                    List<MemberModel> list = target.Member.Parameters;
                    if (list[0].Name == "get") RenameMember(target.InClass, "get_" + originalName, "get_" + newName, outputResults);
                    if (list[1].Name == "set") RenameMember(target.InClass, "set_" + originalName, "set_" + newName, outputResults);
                }
                if (currentCommand == null) ExecuteFirst();
            }
        }

        static void RenameMember(ClassModel inClass, string name, string newName, bool outputResults)
        {
            MemberModel m = inClass.Members.Items.FirstOrDefault(it => it.Name == name);
            if (m == null) return;
            ASResult result = new ASResult();
            ASComplete.FindMember(name, inClass, result, FlagType.Dynamic | FlagType.Function, 0);
            if (result.Member == null) return;
            queue.Add(new Rename(result, outputResults, newName));
        }

        static void ExecuteFirst()
        {
            try
            {
                currentCommand = queue[0];
                queue.Remove(currentCommand);
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
            if (queue.Count > 0) ExecuteFirst();
            else currentCommand = null;
        }
    }
}