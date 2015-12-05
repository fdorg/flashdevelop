using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using ASCompletion.Completion;
using ASCompletion.Context;
using ASCompletion.Model;
using CodeRefactor.Controls;
using CodeRefactor.Provider;
using PluginCore;
using PluginCore.Controls;
using PluginCore.FRService;
using PluginCore.Helpers;
using PluginCore.Localization;
using PluginCore.Managers;
using ProjectManager.Projects;

namespace CodeRefactor.Commands
{
    /// <summary>
    /// Refactors by renaming the given declaration and all its references.
    /// </summary>
    public class Rename : RefactorCommand<IDictionary<String, List<SearchMatch>>>
    {
        static bool includeComments, includeStrings, previewChanges = true;

        bool isRenamePackage;
        string renamePackagePath;
        FindAllReferences findAllReferencesCommand;
        Move renamePackage;
        IRenameHelper helper;

        string oldFileName;
        string newFileName;

        public string OldName { get; private set; }
        public string NewName { get; private set; }
        public bool OutputResults { get; private set; }
        public ASResult Target { get; private set; }

        /// <summary>
        /// A new Rename refactoring command.
        /// Outputs found results.
        /// Uses the current text location as the declaration target.
        /// </summary>
        public Rename()
            : this(false)
        {
        }

        /// <summary>
        /// A new Rename refactoring command.
        /// Outputs found results.
        /// </summary>
        /// <param name="inline">Whether to use inline renaming.</param>
        public Rename(bool inline)
            : this(inline, true)
        {
        }

        /// <summary>
        /// A new Rename refactoring command.
        /// Uses the current text location as the declaration target.
        /// </summary>
        /// <param name="inline">Whether to use inline renaming.</param>
        /// <param name="outputResults">If true, will send the found results to the trace log and results panel</param>
        public Rename(bool inline, bool outputResults)
            : this(RefactoringHelper.GetDefaultRefactorTarget(), inline, outputResults)
        {
        }

        /// <summary>
        /// A new Rename refactoring command.
        /// </summary>
        /// <param name="inline">Whether to use inline renaming.</param>
        /// <param name="target">The target declaration to find references to.</param>
        /// <param name="outputResults">If true, will send the found results to the trace log and results panel</param>
        public Rename(ASResult target, bool inline, bool outputResults)
            : this(target, inline, outputResults, null)
        {
        }

        /// <summary>
        /// A new Rename refactoring command.
        /// </summary>
        /// <param name="inline">Whether to use inline renaming.</param>
        /// <param name="target">The target declaration to find references to.</param>
        /// <param name="outputResults">If true, will send the found results to the trace log and results panel</param>
        /// <param name="newName">If provided, will not query the user for a new name.</param>
        public Rename(ASResult target, bool inline, bool outputResults, string newName)
            : this(target, inline, outputResults, newName, false)
        {
        }

        /// <summary>
        /// A new Rename refactoring command.
        /// </summary>
        /// <param name="inline">Whether to use inline renaming.</param>
        /// <param name="target">The target declaration to find references to.</param>
        /// <param name="outputResults">If true, will send the found results to the trace log and results panel</param>
        /// <param name="newName">If provided, will not query the user for a new name.</param>
        /// <param name="ignoreDeclarationSource">If true, will not rename the original declaration source.  Useful for Encapsulation refactoring.</param>
        public Rename(ASResult target, bool inline, bool outputResults, string newName, bool ignoreDeclarationSource)
        {
            if (target == null)
            {
                TraceManager.Add("refactor target is null");
                return;
            }
            Target = target;
            OutputResults = outputResults;
            if (target.IsPackage)
            {
                isRenamePackage = true;

                string package = target.Path.Replace('.', Path.DirectorySeparatorChar);
                foreach (PathModel aPath in ASContext.Context.Classpath)
                {
                    if (!aPath.IsValid || aPath.Updating) continue;
                    string path = Path.Combine(aPath.Path, package);
                    if (!aPath.IsValid || !Directory.Exists(path)) continue;
                    renamePackagePath = path;
                    StartRename(inline, Path.GetFileName(path), newName);
                    return;
                }
                return;
            }

            isRenamePackage = false;
            string oldName = RefactoringHelper.GetRefactorTargetName(target);

            // create a FindAllReferences refactor to get all the changes we need to make
            // we'll also let it output the results, at least until we implement a way of outputting the renamed results later
            findAllReferencesCommand = new FindAllReferences(target, false, ignoreDeclarationSource) { OnlySourceFiles = true };
            // register a completion listener to the FindAllReferences so we can rename the entries
            findAllReferencesCommand.OnRefactorComplete += OnFindAllReferencesCompleted;

            StartRename(inline, oldName, newName);
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
                    findAllReferencesCommand.IncludeComments = includeComments;
                    findAllReferencesCommand.IncludeStrings = includeStrings;
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
        public override bool IsValid()
        {
            return isRenamePackage ? renamePackage.IsValid() : !string.IsNullOrEmpty(NewName);
        }

        #endregion

        #region Private Helper Methods

        bool ValidateTargets()
        {
            ASResult target = findAllReferencesCommand.CurrentTarget;
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

            FileModel inFile;
            string originName;

            if (isEnum || isClass)
            {
                inFile = target.Type.InFile;
                originName = target.Type.Name;
            }
            else
            {
                inFile = target.Member.InFile;
                originName = target.Member.Name;
            }

            // Is this possible? should return false? I'm inclined to think so
            if (inFile == null) return true;

            oldFileName = inFile.FileName;
            string oldName = Path.GetFileNameWithoutExtension(oldFileName);

            // Private classes and similars
            if (string.IsNullOrEmpty(oldName) || !oldName.Equals(originName))
                return true;

            string fullPath = Path.GetFullPath(inFile.FileName);
            fullPath = Path.GetDirectoryName(fullPath);

            newFileName = Path.Combine(fullPath, NewName + Path.GetExtension(oldFileName));

            // No point in refactoring if the old and new name is the same
            if (string.IsNullOrEmpty(oldFileName) || oldFileName.Equals(newFileName)) return false;

            // Check if the new file name already exists
            return FileHelper.ConfirmOverwrite(newFileName);
        }

        /// <summary>
        /// Renames the given the set of matched references
        /// </summary>
        void OnFindAllReferencesCompleted(object sender, RefactorCompleteEventArgs<IDictionary<string, List<SearchMatch>>> eventArgs)
        {
            UserInterfaceManager.ProgressDialog.Show();
            UserInterfaceManager.ProgressDialog.SetTitle(TextHelper.GetString("Info.UpdatingReferences"));
            MessageBar.Locked = true;
            foreach (KeyValuePair<string, List<SearchMatch>> entry in eventArgs.Results)
            {
                UserInterfaceManager.ProgressDialog.UpdateStatusMessage(TextHelper.GetString("Info.Updating") + " \"" + entry.Key + "\"");
                // re-open the document and replace all the text
                var doc = AssociatedDocumentHelper.LoadDocument(entry.Key);
                var sci = doc.SciControl;
                // replace matches in the current file with the new name
                RefactoringHelper.ReplaceMatches(entry.Value, sci, NewName);
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

        void RenameFile(IDictionary<string, List<SearchMatch>> results)
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
                RefactoringHelper.Move(oldFileName, tmpPath);
                RefactoringHelper.Move(tmpPath, newFileName);
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
        /// 
        /// </summary>
        void ReportResults()
        {
            int newNameLength = NewName.Length;
            PluginBase.MainForm.CallCommand("PluginCommand", "ResultsPanel.ClearResults");
            // outputs the lines as they change
            // some funky stuff to make sure it highlights/reports the resultant changes rather than the old data
            // TODO: this works on the assumption that multiple changes on the same line will come from left-to-right; consider updating to work regardless of order
            foreach (KeyValuePair<string, List<SearchMatch>> entry in Results)
            {
                // as multiple changes are made to the same line, this stores the cumulative offset 
                Dictionary<int, int> lineOffsets = new Dictionary<int, int>();
                // as multiple changes are made to the same line, this stores the cumulative changes to the line
                Dictionary<int, string> lineChanges = new Dictionary<int, string>();
                // stores a listing of lines to report.  Can store multiple instances of each line as some will have different columns
                Dictionary<int, List<string>> reportableLines = new Dictionary<int, List<string>>();
                foreach (SearchMatch match in entry.Value)
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
                foreach (KeyValuePair<int, List<string>> lineSetsToReport in reportableLines)
                {
                    // the final state of the line after all renaming
                    string renamedLine = lineChanges[lineSetsToReport.Key].Trim();
                    foreach (string lineToReport in lineSetsToReport.Value)
                    {
                        // use the String.Format and replace the {0} from above with our final line state
                        TraceManager.Add(string.Format(lineToReport, renamedLine), (int) TraceType.Info);
                    }
                }
            }
            PluginBase.MainForm.CallCommand("PluginCommand", "ResultsPanel.ShowResults");
        }

        void StartRename(bool useInline, string oldName, string newName)
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

                var inlineRename = isRenamePackage ?
                    new InlineRename(sci, oldName, position, null, null, null, null)
                    : new InlineRename(sci, oldName, position, includeComments, includeStrings, previewChanges, findAllReferencesCommand.CurrentTarget);

                inlineRename.Apply += OnApply;
                inlineRename.Cancel += OnCancel;
                helper = inlineRename;
            }
            else
            {
                var dialog = new PopupRenameDialog(oldName, includeComments, includeStrings, isRenamePackage);
                helper = dialog;

                if (dialog.ShowDialog() == DialogResult.OK)
                    OnApply(null, oldName, dialog.Value.Trim());
            }
        }

        void OnApply(InlineRename sender, string oldName, string newName)
        {
            if (sender != null) RemoveInlineHandlers(sender);
            if (newName.Length == 0 || oldName == newName) return;

            if (isRenamePackage)
            {
                renamePackage = new Move(new Dictionary<string, string> { { renamePackagePath, newName } }, true, true);
            }
            else if (helper != null)
            {
                includeComments = helper.IncludeComments;
                includeStrings = helper.IncludeStrings;
                if (helper is InlineRename) previewChanges = ((InlineRename) helper).PreviewChanges;
                helper = null;
            }

            OldName = oldName;
            NewName = newName;
            RenamingHelper.AddToQueue(this);
        }

        void OnCancel(InlineRename sender)
        {
            RemoveInlineHandlers(sender);
            includeComments = sender.IncludeComments;
            includeStrings = sender.IncludeStrings;
            previewChanges = sender.PreviewChanges;
        }

        void RemoveInlineHandlers(InlineRename inlineRename)
        {
            inlineRename.Apply -= OnApply;
            inlineRename.Cancel -= OnCancel;
        }

        #endregion

    }
}