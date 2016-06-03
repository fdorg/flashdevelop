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
    internal static class RenamingHelper
    {
        private static readonly Queue<Rename> queue = new Queue<Rename>();
        private static Rename currentCommand;
        private static StartState startState;

        internal static void AddToQueue(Rename rename)
        {
            queue.Enqueue(rename);

            if (currentCommand != null) return;
            currentCommand = rename;

            var doc = PluginBase.MainForm.CurrentDocument;
            startState = new StartState
            {
                FileName = doc.FileName,
                CursorPosition = doc.SciControl.CurrentPos,
                Commands = new[] { rename, null, null }
            };

            var target = rename.Target;
            bool outputResults = rename.OutputResults;
            if (!target.IsPackage &&
                ASContext.Context.CurrentModel.haXe
                && target.Member != null
                && (target.Member.Flags & (FlagType.Getter | FlagType.Setter)) != 0)
            {
                string oldName = rename.OldName;
                string newName = rename.NewName;
                var list = target.Member.Parameters;
                if (list[0].Name == "get") startState.Commands[1] = RenameMember(target.InClass, "get_" + oldName, "get_" + newName, outputResults);
                if (list[1].Name == "set") startState.Commands[2] = RenameMember(target.InClass, "set_" + oldName, "set_" + newName, outputResults);
            }

            if (outputResults) PluginBase.MainForm.CallCommand("PluginCommand", "ResultsPanel.ClearResults");

            ExecuteFirst();
        }

        private static Rename RenameMember(ClassModel inClass, string name, string newName, bool outputResults)
        {
            var members = inClass.Members.Items;
            for (int i = 0, length = members.Count; i < length; i++)
            {
                var member = members[i];
                if (member.Name == name)
                {
                    var result = new ASResult();
                    ASComplete.FindMember(name, inClass, result, FlagType.Dynamic | FlagType.Function, 0);
                    if (result.Member != null)
                    {
                        return Rename.Create(result, false, outputResults, newName);
                    }
                }
            }
            return null;
        }

        private static void ExecuteFirst()
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

        private static void OnRefactorComplete(object sender, RefactorCompleteEventArgs<IDictionary<string, List<SearchMatch>>> e)
        {
            currentCommand.OnRefactorComplete -= OnRefactorComplete;
            if (queue.Count == 0)
            {
                //if (currentCommand.OutputResults)
                //{
                //    PluginBase.MainForm.CallCommand("PluginCommand", "ResultsPanel.ShowResults");
                //}
                if (startState != null) RestoreStartState();
                currentCommand = null;
                startState = null;
            }
            else ExecuteFirst();
        }

        private static void RestoreStartState()
        {
            int pos = startState.CursorPosition;
            GetOffset(startState.Commands[0], ref pos);
            GetOffset(startState.Commands[1], ref pos);
            GetOffset(startState.Commands[2], ref pos);
            ((ITabbedDocument) PluginBase.MainForm.OpenEditableDocument(startState.FileName)).SciControl.SetSel(pos, pos);

            ASContext.Context.UpdateCurrentFile(true);
        }

        private static void GetOffset(Rename command, ref int pos)
        {
            if (command != null)
            {
                foreach (var entry in command.Results)
                {
                    if (entry.Key == startState.FileName)
                    {
                        int offset = command.NewName.Length - command.TargetName.Length;
                        foreach (var match in entry.Value)
                        {
                            if (pos > match.Index)
                            {
                                pos += offset;
                            }
                            else break; // Assuming the results are sorted in ascending order of Index. Basically all rename (and many other refactoring) operations have this assumption.
                        }
                        break;
                    }
                }
            }
        }
    }

    internal class StartState
    {
        public string FileName;
        public int CursorPosition;
        public Rename[] Commands;
    }
}
