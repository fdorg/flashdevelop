using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using ASCompletion.Completion;
using ASCompletion.Context;
using ASCompletion.Model;
using CodeRefactor.Provider;
using PluginCore;
using PluginCore.Controls;
using PluginCore.FRService;
using PluginCore.Helpers;
using PluginCore.Localization;
using PluginCore.Managers;
using ProjectManager.Helpers;
using ProjectManager.Projects;

namespace CodeRefactor.Commands
{
    using Command = RefactorCommand<IDictionary<string, List<SearchMatch>>>;

    /// <summary>
    /// Refactors by renaming the given declaration and all its references.
    /// </summary>
    public class Rename : Command
    {
        private readonly Command findAllReferencesCommand;
        private Command renamePackage;
        private bool isRenamePackage;
        private string renamePackagePath;

        private string oldFileName;
        private string newFileName;

        public string OldName { get; private set; }
        public string NewName { get; private set; }
        public ASResult Target { get; private set; }
        public string TargetName { get; private set; }

        /// <summary>
        /// A new Rename refactoring command.
        /// Doesn't use inline.
        /// Outputs found results.
        /// Uses the current text location as the declaration target.
        /// </summary>
        public Rename()
            : this(true) { }
        
        /// <summary>
        /// A new Rename refactoring command.
        /// Uses the current text location as the declaration target.
        /// </summary>
        /// <param name="outputResults">If true, will send the found results to the trace log and results panel</param>
        /// <param name="inline">Whether to use inline renaming.</param>
        public Rename(bool outputResults, bool inline = false)
           : this(RefactoringHelper.GetDefaultRefactorTarget(), outputResults, inline) { }

        /// <summary>
        /// A new Rename refactoring command.
        /// </summary>
        /// <param name="target">The target declaration to find references to.</param>
        /// <param name="outputResults">If true, will send the found results to the trace log and results panel</param>
        /// <param name="inline">Whether to use inline renaming.</param>
        public Rename(ASResult target, bool outputResults, bool inline = false)
            : this(target, outputResults, null, inline) { }

        /// <summary>
        /// A new Rename refactoring command.
        /// </summary>
        /// <param name="target">The target declaration to find references to.</param>
        /// <param name="outputResults">If true, will send the found results to the trace log and results panel</param>
        /// <param name="newName">If provided, will not query the user for a new name.</param>
        /// <param name="inline">Whether to use inline renaming.</param>
        public Rename(ASResult target, bool outputResults, string newName, bool inline = false)
            : this(target, outputResults, newName, false, inline) { }
        
        /// <summary>
        /// Initializes a new instance of <see cref="Rename"/> class.
        /// </summary>
        /// <param name="target">The target declaration to find references to.</param>
        /// <param name="outputResults">If true, will send the found results to the trace log and results panel</param>
        /// <param name="newName">If provided, will not query the user for a new name.</param>
        /// <param name="ignoreDeclarationSource">If true, will not rename the original declaration source.  Useful for Encapsulation refactoring.</param>
        /// <param name="inline">Whether to use inline renaming.</param>
        public Rename(ASResult target, bool outputResults, string newName, bool ignoreDeclarationSource, bool inline = false)
        {
            Results = new Dictionary<string, List<SearchMatch>>();
            if (target == null)
            {
                TraceManager.Add("Refactor target is null.");
                return;
            }
            Target = target;
            OutputResults = outputResults;
            if (target.IsPackage)
            {
                isRenamePackage = true;

                string package = target.Path.Replace('.', Path.DirectorySeparatorChar);
                foreach (var aPath in ASContext.Context.Classpath)
                {
                    if (aPath.IsValid && !aPath.Updating)
                    {
                        string path = Path.Combine(aPath.Path, package);
                        if (Directory.Exists(path))
                        {
                            TargetName = Path.GetFileName(path);
                            renamePackagePath = path;
                            StartRename(inline, TargetName, newName);
                            return;
                        }
                    }
                }
                return;
            }

            isRenamePackage = false;
            TargetName = RefactoringHelper.GetRefactorTargetName(target);

            // create a FindAllReferences refactor to get all the changes we need to make
            // we'll also let it output the results, at least until we implement a way of outputting the renamed results later
            findAllReferencesCommand = CommandFactoryProvider.GetFactory(target).CreateFindAllReferencesCommand(target, false, ignoreDeclarationSource, true);
            // register a completion listener to the FindAllReferences so we can rename the entries
            findAllReferencesCommand.OnRefactorComplete += OnFindAllReferencesCompleted;

            StartRename(inline, TargetName, newName);
        }

        #region RefactorCommand Implementation

        /// <summary>
        /// Entry point to execute renaming.
        /// </summary>
        protected override void ExecutionImplementation()
        {
            if (isRenamePackage)
            {
                renamePackage.RegisterDocumentHelper(AssociatedDocumentHelper);
                renamePackage.OnRefactorComplete += OnRenamePackageComplete;
                renamePackage.Execute();
            }
            else
            {
                // To get the initial open documents, finding all references will interfere if we try later
                // We may already have an AssociatedDocumentHelper
                RegisterDocumentHelper(AssociatedDocumentHelper);

                // Targets have to be validated before getting and modifying all references, otherwise we may end with some bad state
                if (ValidateTargets())
                {
                    findAllReferencesCommand.Execute();
                }
                else
                {
                    AssociatedDocumentHelper.CloseTemporarilyOpenedDocuments();
                    FireOnRefactorComplete();
                }
            }
        }

        /// <summary>
        /// Indicates if the current settings for the refactoring are valid.
        /// </summary>
        public override bool IsValid() => isRenamePackage ? renamePackage.IsValid() : !string.IsNullOrEmpty(NewName);

        #endregion

        #region Private Helper Methods

        private void OnRenamePackageComplete(object sender, RefactorCompleteEventArgs<IDictionary<string, List<SearchMatch>>> args)
        {
            Results = args.Results;
            FireOnRefactorComplete();
        }

        private bool ValidateTargets()
        {
            var target = findAllReferencesCommand.CurrentTarget;
            bool isEnum = target.Type.IsEnum();
            bool isClass = false;

            if (!isEnum)
            {
                bool isVoid = target.Type.IsVoid();
                isClass = !isVoid && target.IsStatic && (target.Member == null || RefactoringHelper.CheckFlag(target.Member.Flags, FlagType.Constructor));
            }

            bool isGlobalFunction = false;
            bool isGlobalNamespace = false;

            if (!isEnum && !isClass && (target.InClass == null || target.InClass.IsVoid()))
            {
                isGlobalFunction = RefactoringHelper.CheckFlag(target.Member.Flags, FlagType.Function);
                isGlobalNamespace = RefactoringHelper.CheckFlag(target.Member.Flags, FlagType.Namespace);
            }

            // Types with not their own file
            if (!isEnum && !isClass && !isGlobalFunction && !isGlobalNamespace)
                return true;

            var member = isEnum || isClass ? target.Type : target.Member;
            var inFile = member.InFile;

            oldFileName = inFile.FileName;
            string oldName = Path.GetFileNameWithoutExtension(oldFileName);

            // Private classes and similars
            if (string.IsNullOrEmpty(oldName) || !oldName.Equals(member.Name))
                return true;

            string fullPath = Path.GetFullPath(inFile.FileName);
            fullPath = Path.GetDirectoryName(fullPath);

            newFileName = Path.Combine(fullPath, NewName + Path.GetExtension(oldFileName));

            // No point in refactoring if the old and new name is the same
            if (string.IsNullOrEmpty(oldFileName) || oldFileName.Equals(newFileName)) return false;

            // Check if the new file name already exists
            return oldFileName.Equals(newFileName, StringComparison.OrdinalIgnoreCase) 
                || FileHelper.ConfirmOverwrite(newFileName);
        }

        /// <summary>
        /// Renames the given the set of matched references
        /// </summary>
        private void OnFindAllReferencesCompleted(object sender, RefactorCompleteEventArgs<IDictionary<string, List<SearchMatch>>> eventArgs)
        {
            UserInterfaceManager.ProgressDialog.Show();
            UserInterfaceManager.ProgressDialog.SetTitle(TextHelper.GetString("Info.UpdatingReferences"));
            MessageBar.Locked = true;
            var isParameterVar = (Target.Member?.Flags & FlagType.ParameterVar) > 0;
            var fileName = PluginBase.MainForm.CurrentDocument.FileName;
            foreach (var entry in eventArgs.Results)
            {
                UserInterfaceManager.ProgressDialog.UpdateStatusMessage(TextHelper.GetString("Info.Updating") + " \"" + entry.Key + "\"");
                // re-open the document and replace all the text
                var doc = AssociatedDocumentHelper.LoadDocument(entry.Key);
                var sci = doc.SciControl;
                var targetMatches = entry.Value;
                if (isParameterVar)
                {
                    var lineFrom = Target.Context.ContextFunction.LineFrom;
                    var lineTo = Target.Context.ContextFunction.LineTo;
                    var search = RefactoringHelper.GetFRSearch(NewName, false, false);
                    var config = new FRConfiguration(fileName, search) {CacheDocuments = true};
                    var matches = search.Matches(config.GetSource(fileName));
                    matches.RemoveAll(it => it.Line < lineFrom || it.Line > lineTo);
                    if (matches.Count != 0)
                    {
                        sci.BeginUndoAction();
                        try
                        {
                            for (var i = 0; i < matches.Count; i++)
                            {
                                var match = matches[i];
                                var expr = ASComplete.GetExpressionType(sci, sci.MBSafePosition(match.Index) + sci.MBSafeTextLength(match.Value));
                                if (expr.IsNull() || expr.Context.Value != NewName) continue;
                                string replacement;
                                var flags = expr.Member.Flags;
                                if ((flags & FlagType.Static) > 0) replacement = ASContext.Context.CurrentClass.Name + "." + NewName;
                                else if((flags & FlagType.LocalVar) == 0) replacement = "this." + NewName;
                                else continue;
                                RefactoringHelper.SelectMatch(sci, match);
                                sci.EnsureVisible(sci.LineFromPosition(sci.MBSafePosition(match.Index)));
                                sci.ReplaceSel(replacement);
                                for (var j = 0; j < targetMatches.Count; j++)
                                {
                                    var targetMatch = targetMatches[j];
                                    if (targetMatch.Line <= match.Line) continue;
                                    FRSearch.PadIndexes(targetMatches, j, match.Value, replacement);
                                    if (targetMatch.Line == match.Line + 1)
                                    {
                                        targetMatch.LineText = sci.GetLine(match.Line);
                                        targetMatch.Column += replacement.Length - match.Value.Length;
                                    }
                                    break;
                                }
                                FRSearch.PadIndexes(matches, i + 1, match.Value, replacement);
                            }
                        }
                        finally
                        {
                            sci.EndUndoAction();
                        }
                    }
                }
                // replace matches in the current file with the new name
                RefactoringHelper.ReplaceMatches(targetMatches, sci, NewName);
                //Uncomment if we want to keep modified files
                //if (sci.IsModify) AssociatedDocumentHelper.MarkDocumentToKeep(entry.Key);
                doc.Save();
            }
            if (newFileName != null) RenameFile(eventArgs.Results);
            Results = eventArgs.Results;
            AssociatedDocumentHelper.CloseTemporarilyOpenedDocuments();
            if (OutputResults) ReportResults();
            UserInterfaceManager.ProgressDialog.Hide();
            MessageBar.Locked = false;
            FireOnRefactorComplete();
        }

        private void RenameFile(IDictionary<string, List<SearchMatch>> results)
        {
            // We close previous files to avoid unwanted "file modified" dialogs
            ITabbedDocument doc;
            bool reopen = false;
            if (AssociatedDocumentHelper.InitiallyOpenedFiles.TryGetValue(oldFileName, out doc))
            {
                doc.Close();
                reopen = true;
            }
            if (AssociatedDocumentHelper.InitiallyOpenedFiles.TryGetValue(newFileName, out doc))
            {
                doc.Close();
                reopen = true;
            }

            // name casing changed
            if (oldFileName.Equals(newFileName, StringComparison.OrdinalIgnoreCase))
            {
                string tmpPath = oldFileName + "$renaming$";
                File.Move(oldFileName, tmpPath);
                RefactoringHelper.Move(tmpPath, newFileName, true, oldFileName);
            }
            else
            {
                var project = (Project) PluginBase.CurrentProject;
                FileHelper.ForceMove(oldFileName, newFileName);
                DocumentManager.MoveDocuments(oldFileName, newFileName);
                if (project.IsDocumentClass(oldFileName))
                {
                    project.SetDocumentClass(newFileName, true);
                    project.Save();
                }
            }

            if (results.ContainsKey(oldFileName))
            {
                results[newFileName] = results[oldFileName];
                results.Remove(oldFileName);
            }
            if (reopen)
                PluginBase.MainForm.OpenEditableDocument(newFileName);
        }

        /// <summary>
        /// Outputs the results to the TraceManager
        /// </summary>
        private void ReportResults()
        {
            int newNameLength = NewName.Length;
            // outputs the lines as they change
            // some funky stuff to make sure it highlights/reports the resultant changes rather than the old data
            // TODO: this works on the assumption that multiple changes on the same line will come from left-to-right; consider updating to work regardless of order
            foreach (var entry in Results)
            {
                // as multiple changes are made to the same line, this stores the cumulative offset 
                var lineOffsets = new Dictionary<int, int>();
                // as multiple changes are made to the same line, this stores the cumulative changes to the line
                var lineChanges = new Dictionary<int, string>();
                // stores a listing of lines to report.  Can store multiple instances of each line as some will have different columns
                var reportableLines = new Dictionary<int, List<string>>();
                foreach (var match in entry.Value)
                {
                    int column = match.Column;
                    int lineNumber = match.Line;
                    // if we've already modified the line, we use the data from the last change
                    string changedLine = (lineChanges.ContainsKey(lineNumber) ? lineChanges[lineNumber] : match.LineText);
                    int offset = (lineOffsets.ContainsKey(lineNumber) ? lineOffsets[lineNumber] : 0);
                    // offsets our column references to take into account previous changes to the line
                    column = column + offset;
                    // determines what the newly formed line will look like
                    changedLine = changedLine.Substring(0, column) + NewName + changedLine.Substring(column + match.Length);
                    // stores the changes in case we have to modify the line again later
                    lineChanges[lineNumber] = changedLine;
                    lineOffsets[lineNumber] = offset + (newNameLength - match.Length);
                    // stores the line entry in our report set
                    if (!reportableLines.ContainsKey(lineNumber))
                    {
                        reportableLines[lineNumber] = new List<string>();
                    }
                    // the data we store matches the TraceManager.Add's formatting.  We insert the {0} at the end so that we can insert the final line state later
                    reportableLines[lineNumber].Add(entry.Key + ":" + match.Line + ": chars " + column + "-" + (column + newNameLength) + " : {0}");
                }
                // report all the lines
                foreach (var lineSetsToReport in reportableLines)
                {
                    // the final state of the line after all renaming
                    string renamedLine = lineChanges[lineSetsToReport.Key].Trim();
                    foreach (string lineToReport in lineSetsToReport.Value)
                    {
                        // use the String.Format and replace the {0} from above with our final line state
                        TraceManager.Add(string.Format(lineToReport, renamedLine), (int) TraceType.Info, PluginMain.TraceGroup);
                    }
                }
            }
        }

        /// <summary>
        /// Begins the process of renaming.
        /// </summary>
        private void StartRename(bool useInline, string oldName, string newName)
        {
            if (!string.IsNullOrEmpty(newName))
            {
                OnApply(null, oldName, newName);
                return;
            }

            if (useInline)
            {
                var sci = PluginBase.MainForm.CurrentDocument.SciControl;
                int position = sci.WordEndPosition(sci.CurrentPos, true);

                var inlineRename = new InlineRename(sci, oldName, position, null, null, isRenamePackage ? new bool?() : new bool?(true), Target);
                inlineRename.Apply += OnApply;
                inlineRename.Cancel += OnCancel;
            }
            else
            {
                string title = " " + string.Format(TextHelper.GetString("Title.RenameDialog"), oldName);
                string label = TextHelper.GetString("Label.NewName");
                var dialog = new LineEntryDialog(title, label, oldName);

                switch (dialog.ShowDialog())
                {
                    case DialogResult.OK:
                        OnApply(null, oldName, dialog.Line.Trim());
                        break;
                    case DialogResult.Cancel:
                        OnCancel(null);
                        break;
                }
            }
        }

        /// <summary>
        /// Apply the new name.
        /// </summary>
        private void OnApply(InlineRename sender, string oldName, string newName)
        {
            UpdateDefaultFlags(sender);
            if (newName.Length == 0 || oldName == newName) return;

            if (isRenamePackage)
            {
                renamePackage = new Move(new Dictionary<string, string> { { renamePackagePath, newName } }, true, true);
            }

            OldName = oldName;
            NewName = newName;
            RenamingHelper.AddToQueue(this);
        }

        /// <summary>
        /// Cancel renaming and clean up.
        /// </summary>
        private void OnCancel(InlineRename sender)
        {
            UpdateDefaultFlags(sender);
            if (findAllReferencesCommand != null)
            {
                findAllReferencesCommand.OnRefactorComplete -= OnFindAllReferencesCompleted;
            }
        }

        /// <summary>
        /// Update the default flags and clean up.
        /// </summary>
        private void UpdateDefaultFlags(InlineRename inlineRename)
        {
            if (inlineRename != null)
            {
                inlineRename.Apply -= OnApply;
                inlineRename.Cancel -= OnCancel;
            }
        }

        #endregion

    }
}