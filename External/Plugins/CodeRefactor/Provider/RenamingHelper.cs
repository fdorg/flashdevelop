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
    static class RenamingHelper
    {
        static readonly Queue<Rename> queue = new Queue<Rename>();
        static Rename currentCommand;
        static StartState startState;

        internal static void AddToQueue(Rename rename)
        {
            queue.Enqueue(rename);

            if (currentCommand != null) return;
            currentCommand = rename;

            ASResult target = rename.Target;
            bool outputResults = rename.OutputResults;
            if (!target.IsPackage &&
                ASContext.Context.CurrentModel.haXe
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

            var doc = PluginBase.MainForm.CurrentDocument;
            startState = new StartState
            {
                FileName = doc.FileName,
                CursorPosition = doc.SciControl.CurrentPos,
                Command = rename
            };

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
                startState = null;
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
                if (startState != null) RestoreStartState();
                currentCommand = null;
                startState = null;
            }
        }

        static void RestoreStartState()
        {
            var fileName = startState.FileName;
            var cursorPosition = startState.CursorPosition;
            var command = startState.Command;
            var charsDiff = command.NewName.Length - command.TargetName.Length;
            foreach (var entry in command.Results)
            {
                if (entry.Key != fileName) continue;
                SearchMatch match = null;
                foreach (var tmpMatch in entry.Value)
                {
                    var start = tmpMatch.Index - charsDiff;
                    if (cursorPosition >= start)
                    {
                        charsDiff += charsDiff;
                        match = tmpMatch;
                    }
                    else break;
                }
                var doc = (ITabbedDocument) PluginBase.MainForm.OpenEditableDocument(fileName);
                var sci = doc.SciControl;
                var pos = sci.PositionFromLine(match.Line - 1) + match.Column;
                sci.SetSel(pos, pos);
                break;
            }
        }
    }

    class StartState
    {
        public string FileName;
        public int CursorPosition;
        public Rename Command;
    }
}
