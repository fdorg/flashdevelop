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
using ProjectManager.Helpers;
using ProjectManager.Projects;

namespace CodeRefactor.Commands
{
    /// <summary>
    /// Refactors by renaming the given declaration and all its references.
    /// </summary>
    public class Rename : RefactorCommand<IDictionary<String, List<SearchMatch>>>
    {
        private String newName;
        private Boolean outputResults;
        private FindAllReferences findAllReferencesCommand;
        private Move renamePackage;

        private String oldFileName;
        private String newFileName;

        public String NewName
        {
            get { return this.newName; }
        }

        /// <summary>
        /// A new Rename refactoring command.
        /// Outputs found results.
        /// Uses the current text location as the declaration target.
        /// </summary>
        public Rename()
            : this(true)
        {
        }

        /// <summary>
        /// A new Rename refactoring command.
        /// Uses the current text location as the declaration target.
        /// </summary>
        /// <param name="outputResults">If true, will send the found results to the trace log and results panel</param>
        public Rename(Boolean outputResults)
            : this(RefactoringHelper.GetDefaultRefactorTarget(), outputResults)
        {
        }

        /// <summary>
        /// A new Rename refactoring command.
        /// </summary>
        /// <param name="target">The target declaration to find references to.</param>
        /// <param name="outputResults">If true, will send the found results to the trace log and results panel</param>
        public Rename(ASResult target, Boolean outputResults)
            : this(target, outputResults, null)
        {
        }

        /// <summary>
        /// A new Rename refactoring command.
        /// </summary>
        /// <param name="target">The target declaration to find references to.</param>
        /// <param name="outputResults">If true, will send the found results to the trace log and results panel</param>
        /// <param name="newName">If provided, will not query the user for a new name.</param>
        public Rename(ASResult target, Boolean outputResults, String newName)
            : this(target, outputResults, newName, false)
        {
        }

        /// <summary>
        /// A new Rename refactoring command.
        /// </summary>
        /// <param name="target">The target declaration to find references to.</param>
        /// <param name="outputResults">If true, will send the found results to the trace log and results panel</param>
        /// <param name="newName">If provided, will not query the user for a new name.</param>
        /// <param name="ignoreDeclarationSource">If true, will not rename the original declaration source.  Useful for Encapsulation refactoring.</param>
        public Rename(ASResult target, Boolean outputResults, String newName, Boolean ignoreDeclarationSource)
        {
            if (target == null)
            {
                TraceManager.Add("refactor target is null");
                return;
            }
            this.outputResults = outputResults;
            if (target.IsPackage)
            {
                string package = target.Path.Replace('.', Path.DirectorySeparatorChar);
                foreach (PathModel aPath in ASContext.Context.Classpath)
                {
                    if (aPath.IsValid && !aPath.Updating)
                    {
                        string path = Path.Combine(aPath.Path, package);
                        if (aPath.IsValid && Directory.Exists(path))
                        {
                            this.newName = string.IsNullOrEmpty(newName) ? GetNewName(Path.GetFileName(path)) : newName;
                            if (string.IsNullOrEmpty(this.newName)) return;
                            renamePackage = new Move(new Dictionary<string, string> { { path, this.newName } }, true, true);
                            return;
                        }
                    }
                }
                return;
            }

            this.newName = !string.IsNullOrEmpty(newName) ? newName : GetNewName(RefactoringHelper.GetRefactorTargetName(target));

            if (string.IsNullOrEmpty(this.newName)) return;

            // create a FindAllReferences refactor to get all the changes we need to make
            // we'll also let it output the results, at least until we implement a way of outputting the renamed results later
            this.findAllReferencesCommand = new FindAllReferences(target, false, ignoreDeclarationSource) { OnlySourceFiles = true };
            // register a completion listener to the FindAllReferences so we can rename the entries
            this.findAllReferencesCommand.OnRefactorComplete += OnFindAllReferencesCompleted;
        }

        #region RefactorCommand Implementation

        /// <summary>
        /// Entry point to execute renaming.
        /// </summary>
        protected override void ExecutionImplementation()
        {
            if (renamePackage != null)
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
        public override Boolean IsValid()
        {
            return renamePackage != null ? renamePackage.IsValid() : !string.IsNullOrEmpty(this.newName);
        }

        #endregion

        #region Private Helper Methods

        private bool ValidateTargets()
        {
            ASResult target = findAllReferencesCommand.CurrentTarget;
            Boolean isEnum = target.Type.IsEnum();
            Boolean isClass = false;

            if (!isEnum)
            {
                Boolean isVoid = target.Type.IsVoid();
                isClass = !isVoid && target.IsStatic && (target.Member == null || RefactoringHelper.CheckFlag(target.Member.Flags, FlagType.Constructor));
            }

            Boolean isGlobalFunction = false;
            Boolean isGlobalNamespace = false;

            if (!isEnum && !isClass && (target.InClass == null || target.InClass.IsVoid()))
            {
                isGlobalFunction = RefactoringHelper.CheckFlag(target.Member.Flags, FlagType.Function);
                isGlobalNamespace = RefactoringHelper.CheckFlag(target.Member.Flags, FlagType.Namespace);
            }

            // Types with not their own file
            if (!isEnum && !isClass && !isGlobalFunction && !isGlobalNamespace)
                return true;

            FileModel inFile;
            String originName;

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
            String oldName = Path.GetFileNameWithoutExtension(oldFileName);

            // Private classes and similars
            if (string.IsNullOrEmpty(oldName) || !oldName.Equals(originName))
                return true;

            String fullPath = Path.GetFullPath(inFile.FileName);
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
        private void OnFindAllReferencesCompleted(Object sender, RefactorCompleteEventArgs<IDictionary<string, List<SearchMatch>>> eventArgs)
        {
            UserInterfaceManager.ProgressDialog.Show();
            UserInterfaceManager.ProgressDialog.SetTitle(TextHelper.GetString("Info.UpdatingReferences"));
            MessageBar.Locked = true;
            foreach (KeyValuePair<String, List<SearchMatch>> entry in eventArgs.Results)
            {
                UserInterfaceManager.ProgressDialog.UpdateStatusMessage(TextHelper.GetString("Info.Updating") + " \"" + entry.Key + "\"");
                // re-open the document and replace all the text
                var doc = AssociatedDocumentHelper.LoadDocument(entry.Key);
                var sci = doc.SciControl;
                // replace matches in the current file with the new name
                RefactoringHelper.ReplaceMatches(entry.Value, sci, this.newName);
                //Uncomment if we want to keep modified files
                //if (sci.IsModify) AssociatedDocumentHelper.MarkDocumentToKeep(entry.Key);
                doc.Save();
            }
            if (newFileName != null) RenameFile(eventArgs.Results);
            this.Results = eventArgs.Results;
            AssociatedDocumentHelper.CloseTemporarilyOpenedDocuments();
            if (this.outputResults) this.ReportResults();
            UserInterfaceManager.ProgressDialog.Hide();
            MessageBar.Locked = false;
            this.FireOnRefactorComplete();
        }

        private void RenameFile(IDictionary<string, List<SearchMatch>> results)
        {
            // We close previous files to avoid unwanted "file modified" dialogs
            ITabbedDocument doc;
            Boolean reopen = false;
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
                var project = (Project)PluginBase.CurrentProject;
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
        private void ReportResults()
        {
            int newNameLength = this.NewName.Length;
            PluginBase.MainForm.CallCommand("PluginCommand", "ResultsPanel.ClearResults");
            // outputs the lines as they change
            // some funky stuff to make sure it highlights/reports the resultant changes rather than the old data
            // TODO: this works on the assumption that multiple changes on the same line will come from left-to-right; consider updating to work regardless of order
            foreach (KeyValuePair<String, List<SearchMatch>> entry in this.Results)
            {
                // as multiple changes are made to the same line, this stores the cumulative offset 
                Dictionary<int, int> lineOffsets = new Dictionary<int, int>();
                // as multiple changes are made to the same line, this stores the cumulative changes to the line
                Dictionary<int, String> lineChanges = new Dictionary<int, string>();
                // stores a listing of lines to report.  Can store multiple instances of each line as some will have different columns
                Dictionary<int, List<String>> reportableLines = new Dictionary<int, List<string>>();
                foreach (SearchMatch match in entry.Value)
                {
                    Int32 column = match.Column;
                    Int32 lineNumber = match.Line;
                    // if we've already modified the line, we use the data from the last change
                    string changedLine = (lineChanges.ContainsKey(lineNumber) ? lineChanges[lineNumber] : match.LineText);
                    int offset = (lineOffsets.ContainsKey(lineNumber) ? lineOffsets[lineNumber] : 0);
                    // offsets our column references to take into account previous changes to the line
                    column = column + offset;
                    // determines what the newly formed line will look like
                    changedLine = changedLine.Substring(0, column) + this.NewName + changedLine.Substring(column + match.Length);
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
                foreach (KeyValuePair<int, List<String>> lineSetsToReport in reportableLines)
                {
                    // the final state of the line after all renaming
                    String renamedLine = lineChanges[lineSetsToReport.Key].Trim();
                    foreach (String lineToReport in lineSetsToReport.Value)
                    {
                        // use the String.Format and replace the {0} from above with our final line state
                        TraceManager.Add(String.Format(lineToReport, renamedLine), (Int32)TraceType.Info);
                    }
                }
            }
            PluginBase.MainForm.CallCommand("PluginCommand", "ResultsPanel.ShowResults");
        }

        /// <summary>
        /// This retrieves the new name from the user
        /// </summary>
        private String GetNewName(String originalName)
        {
            String label = TextHelper.GetString("Label.NewName");
            String title = String.Format(TextHelper.GetString("Title.RenameDialog"), originalName);
            LineEntryDialog askName = new LineEntryDialog(title, label, originalName);
            if (askName.ShowDialog() == DialogResult.OK)
            {
                string newName = askName.Line.Trim();
                if(newName.Length > 0 && newName != originalName) return newName;
            }
            return null;
        }

        #endregion

    }
}