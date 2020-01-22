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
        static readonly Queue<Rename> queue = new Queue<Rename>();
        static Rename currentCommand;
        static StartState startState;

        internal static void AddToQueue(Rename command)
        {
            queue.Enqueue(command);

            if (currentCommand != null) return;
            currentCommand = command;

            var sci = PluginBase.MainForm.CurrentDocument.SciControl;
            startState = new StartState
            {
                FileName = sci.FileName,
                CursorPosition = sci.CurrentPos,
                Commands = new[] { command, null, null }
            };

            var target = command.Target;
            var outputResults = command.OutputResults;
            if (target.IsPackage)
            {
                var separator = Path.DirectorySeparatorChar;
                startState.FileName = startState.FileName.Replace($"{separator}{command.OldName}{separator}", $"{separator}{command.NewName}{separator}");
            }
            else if (HasGetterSetter(target))
            {
                if (target.Member.Parameters is { } list && list.Count is { } count && count > 0)
                {
                    if (list[0].Name == ParamGetter) startState.Commands[1] = RenameMember(target, PrefixGetter + command.OldName, PrefixGetter + command.NewName, outputResults);
                    if (count > 1 && list[1].Name == ParamSetter) startState.Commands[2] = RenameMember(target, PrefixSetter + command.OldName, PrefixSetter + command.NewName, outputResults);
                }
            }
            else if ((RefactoringHelper.GetRefactoringTarget(target).Flags & (FlagType.Constructor | FlagType.Class)) > 0)
            {
                var ext = Path.GetExtension(startState.FileName);
                startState.FileName = startState.FileName.Replace(command.OldName + ext, command.NewName + ext);
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
            foreach (var member in inClass.Members)
            {
                if (member.Name != name) continue;
                var result = new ASResult();
                ASComplete.FindMember(name, inClass, result, FlagType.Dynamic | FlagType.Function, 0);
                if (result.Member != null) return result;
            }
            return null;
        }

        static Rename RenameMember(ASResult target, string name, string newName, bool outputResults)
        {
            return FindGetterSetter(target, name) is { } result
                ? new Rename(result, outputResults, newName)
                : null;
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
                if (startState != null) RestoreStartState();
                currentCommand = null;
                startState = null;
            }
            else ExecuteFirst();
        }

        static void RestoreStartState()
        {
            var pos = startState.CursorPosition;
            GetOffset(startState.Commands[0], ref pos);
            GetOffset(startState.Commands[1], ref pos);
            GetOffset(startState.Commands[2], ref pos);
            ((ITabbedDocument) PluginBase.MainForm.OpenEditableDocument(startState.FileName)).SciControl.SetSel(pos, pos);

            ASContext.Context.UpdateCurrentFile(true);
        }

        static void GetOffset(Rename command, ref int pos)
        {
            if (command is null) return;
            foreach (var entry in command.Results)
            {
                if (entry.Key != startState.FileName) continue;
                var offset = command.NewName.Length - command.TargetName.Length;
                foreach (var match in entry.Value)
                {
                    if (pos <= match.Index) break; // Assuming the results are sorted in ascending order of Index. Basically all rename (and many other refactoring) operations have this assumption.
                    pos += offset;
                }
                break;
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