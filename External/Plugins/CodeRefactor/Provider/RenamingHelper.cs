using CodeRefactor.Commands;
using PluginCore.FRService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using ASCompletion.Completion;
using ASCompletion.Context;
using ASCompletion.Model;
using PluginCore;
using PluginCore.Localization;
using PluginCore.Managers;
using ProjectManager.Helpers;

namespace CodeRefactor.Provider
{
    class RenamingHelper
    {
        static readonly List<Rename> queue = new List<Rename>();
        static Rename currentCommand;
        static StartState startState;

        public static void AddToQueue(ASResult target)
        {
            AddToQueue(target, true);
        }
        public static void AddToQueue(ASResult target, bool outputResults)
        {
            Rename cmd = null;
            if (target.IsPackage)
            {
                cmd = new Rename(target, outputResults);
                queue.Add(cmd);
            }
            else
            {
                string originalName = RefactoringHelper.GetRefactorTargetName(target);
                string label = TextHelper.GetString("Label.NewName");
                string title = string.Format(TextHelper.GetString("Title.RenameDialog"), originalName);
                LineEntryDialog askName = new LineEntryDialog(title, label, originalName);
                if (askName.ShowDialog() != DialogResult.OK) return;
                string newName = askName.Line.Trim();
                if (newName.Length == 0 || newName == originalName) return;
                cmd = new Rename(target, outputResults, newName);
                queue.Add(cmd);
                if (ASContext.Context.CurrentModel.haXe && target.Member != null &&
                    (target.Member.Flags & (FlagType.Getter | FlagType.Setter)) > 0)
                {
                    List<MemberModel> list = target.Member.Parameters;
                    if (list[0].Name == "get") RenameMember(target.InClass, "get_" + originalName, "get_" + newName, outputResults);
                    if (list[1].Name == "set") RenameMember(target.InClass, "set_" + originalName, "set_" + newName, outputResults);
                }
            }
            if (currentCommand != null) return;
            if (cmd != null)
            {
                var doc = PluginBase.MainForm.CurrentDocument;
                startState = new StartState
                {
                    FileName = doc.FileName,
                    CursorPosition = doc.SciControl.CurrentPos,
                    Cmd = cmd
                };
            }
            ExecuteFirst();
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
            else
            {
                if (startState != null) RestoreStartState();
                currentCommand = null;
                startState = null;
            }
        }

        static void RestoreStartState()
        {
            var fileName = startState.FileName;
            var cursorPosition = startState.CursorPosition;
            var cmd = startState.Cmd;
            var charsDiff = cmd.NewName.Length - cmd.TargetName.Length;
            foreach (var entry in cmd.Results)
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
        public Rename Cmd;
    }
}