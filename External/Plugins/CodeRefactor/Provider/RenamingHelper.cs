using System;
using System.Collections.Generic;
using System.IO;
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
        internal const string ParamGetter = "get";
        internal const string ParamSetter = "set";
        internal const string PrefixGetter = "get_";
        internal const string PrefixSetter = "set_";
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
            var outputResults = rename.OutputResults;
            if (!target.IsPackage)
            {
                if (HasGetterSetter(target))
                {
                    var oldName = rename.OldName;
                    var newName = rename.NewName;
                    var list = target.Member.Parameters;
                    if (list != null)
                    {
                        var count = list.Count;
                        if (count > 0 && list[0].Name == ParamGetter) startState.Commands[1] = RenameMember(target, PrefixGetter + oldName, PrefixGetter + newName, outputResults);
                        if (count > 1 && list[1].Name == ParamSetter) startState.Commands[2] = RenameMember(target, PrefixSetter + oldName, PrefixSetter + newName, outputResults);
                    }
                }
                else if ((RefactoringHelper.GetRefactoringTarget(target).Flags & (FlagType.Constructor | FlagType.Class)) > 0)
                {
                    var ext = Path.GetExtension(startState.FileName);
                    startState.FileName = startState.FileName.Replace(rename.OldName + ext, rename.NewName + ext);
                }
            }

            if (outputResults) PluginBase.MainForm.CallCommand("PluginCommand", "ResultsPanel.ClearResults;" + PluginMain.TraceGroup);

            ExecuteFirst();
        }

        internal static bool HasGetterSetter(ASResult target)
        {
            return !target.IsPackage
                && ASContext.Context.CurrentModel.haXe
                && target.Member != null
                && (target.Member.Flags & (FlagType.Getter | FlagType.Setter)) != 0;
        }

        internal static ASResult FindGetterSetter(ASResult target, string name)
        {
            var inClass = target.InClass;
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
                        return result;
                    }
                }
            }
            return null;
        }

        private static Rename RenameMember(ASResult target, string name, string newName, bool outputResults)
        {
            var result = FindGetterSetter(target, name);
            if (result != null)
            {
                return new Rename(result, outputResults, newName);
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
