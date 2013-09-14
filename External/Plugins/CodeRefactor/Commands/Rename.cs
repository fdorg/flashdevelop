using System;
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;
using CodeRefactor.Provider;
using PluginCore.Controls;
using PluginCore.FRService;
using ASCompletion.Completion;
using ASCompletion.Context;
using ASCompletion.Model;
using PluginCore.Localization;
using PluginCore.Managers;
using ProjectManager.Helpers;
using ScintillaNet;
using PluginCore;

namespace CodeRefactor.Commands
{
    /// <summary>
    /// Refactors by renaming the given declaration and all its references.
    /// </summary>
    public class Rename : RefactorCommand<IDictionary<String, List<SearchMatch>>>
    {
        private readonly Boolean _outputResults;
        private readonly FindAllReferences _findAllReferencesCommand;

        public string NewName { get; private set; }

        /// <summary>
        /// A new Rename refactoring command.
        /// Outputs found results.
        /// Uses the current text location as the declaration target.
        /// </summary>
        public Rename() : this(true)
        {
        }

        /// <summary>
        /// A new Rename refactoring command.
        /// Uses the current text location as the declaration target.
        /// </summary>
        /// <param name="outputResults">If true, will send the found results to the trace log and results panel</param>
        public Rename(Boolean outputResults) : this(RefactoringHelper.GetDefaultRefactorTarget(), outputResults)
        {
        }

        /// <summary>
        /// A new Rename refactoring command.
        /// </summary>
        /// <param name="target">The target declaration to find references to.</param>
        /// <param name="outputResults">If true, will send the found results to the trace log and results panel</param>
        public Rename(ASResult target, Boolean outputResults) : this(target, outputResults, null)
        {

        }

        /// <summary>
        /// A new Rename refactoring command.
        /// </summary>
        /// <param name="target">The target declaration to find references to.</param>
        /// <param name="outputResults">If true, will send the found results to the trace log and results panel</param>
        /// <param name="newName">If provided, will not query the user for a new name.</param>
        public Rename(ASResult target, Boolean outputResults, String newName) : this(target, outputResults, newName, false)
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
                //TODO: the first call of rename for global function or global namespace
                TraceManager.Add("refactor target is null");
                return;
            }

            _outputResults = outputResults;

            Boolean isVoid = target.Type.IsVoid();
            Boolean isClass = !isVoid && target.IsStatic && target.Member == null;

            if (newName != null && newName.Trim() != String.Empty)
                NewName = newName;
            else if (isClass)
                NewName = GetNewName(target.Type.Name);
            else
                NewName = GetNewName(target.Member.Name);

            if (NewName == null) return;

            // create a FindAllReferences refactor to get all the changes we need to make
            // we'll also let it output the results, at least until we implement a way of outputting the renamed results later
            _findAllReferencesCommand = new FindAllReferences(target, false, ignoreDeclarationSource);
            // register a completion listener to the FindAllReferences so we can rename the entries
            _findAllReferencesCommand.OnRefactorComplete += OnFindAllReferencesCompleted;
        }

        #region RefactorCommand Implementation

        /// <summary>
        /// Entry point to execute renaming.
        /// </summary>
        protected override void ExecutionImplementation()
        {
            _findAllReferencesCommand.Execute();
        }

        /// <summary>
        /// Indicates if the current settings for the refactoring are valid.
        /// </summary>
        public override Boolean IsValid()
        {
            return NewName != null && NewName.Trim() != String.Empty;
        }

        #endregion

        #region Private Helper Methods

        /// <summary>
        /// Renames the given the set of matched references
        /// </summary>
        private void OnFindAllReferencesCompleted(Object sender, RefactorCompleteEventArgs<IDictionary<string, List<SearchMatch>>> eventArgs)
        {
            UserInterfaceManager.ProgressDialog.Show();
            UserInterfaceManager.ProgressDialog.SetTitle(TextHelper.GetString("Info.RenamingReferences"));
            MessageBar.Locked = true;
            foreach (KeyValuePair<String, List<SearchMatch>> entry in eventArgs.Results)
            {
                UserInterfaceManager.ProgressDialog.UpdateStatusMessage(TextHelper.GetString("Info.Updating") + " \"" + entry.Key + "\"");
                // re-open the document and replace all the text
                PluginBase.MainForm.OpenEditableDocument(entry.Key);
                ScintillaControl sci = ASContext.CurSciControl;
                // replace matches in the current file with the new name
                RefactoringHelper.ReplaceMatches(entry.Value, sci, NewName, sci.Text);
                if (sci.IsModify)
                    AssociatedDocumentHelper.MarkDocumentToKeep(sci.FileName);
            }
            
            Results = eventArgs.Results;
            if (_outputResults)
                ReportResults();
            
            UserInterfaceManager.ProgressDialog.Hide();
            MessageBar.Locked = false;
            FireOnRefactorComplete();

            RenameFile();
        }

        private void RenameFile()
        {
            ASResult target = _findAllReferencesCommand.CurrentTarget;
            Boolean isEnum = target.Type.IsEnum();
            Boolean isClass = false;
            Boolean isConstructor = false;
            
            if (!isEnum)
            {
                Boolean isVoid = target.Type.IsVoid();
                isClass = !isVoid && target.IsStatic && target.Member == null;
                isConstructor = !isVoid && !isClass && RefactoringHelper.CheckFlag(target.Member.Flags, FlagType.Constructor);
            }

            Boolean isGlobalFunction = false;
            Boolean isGlobalNamespace = false;

            if (!isEnum && !isClass && !isConstructor && (target.InClass == null || target.InClass.IsVoid()))
            {
                isGlobalFunction = RefactoringHelper.CheckFlag(target.Member.Flags, FlagType.Function);
                isGlobalNamespace = RefactoringHelper.CheckFlag(target.Member.Flags, FlagType.Namespace);
            }

            if (!isEnum && !isClass && !isConstructor && !isGlobalFunction && !isGlobalNamespace)
                return;

            FileModel inFile = null;
            String originName = null;

            if (isConstructor || isGlobalFunction || isGlobalNamespace)
            {
                inFile = target.Member.InFile;
                originName = target.Member.Name;
            }
            else if (isClass || isEnum)
            {
                inFile = target.Type.InFile;
                originName = target.Type.Name;
            }

            if (inFile == null)
                return;

            String oldFileName = inFile.FileName;
            String oldName = Path.GetFileNameWithoutExtension(oldFileName);

            if (oldName != null && !oldName.Equals(originName))
                return;

            String fullPath = Path.GetFullPath(inFile.FileName);
            fullPath = Path.GetDirectoryName(fullPath);

            if (fullPath == null)
                return;

            String newFileName = Path.Combine(fullPath, NewName + Path.GetExtension(oldFileName));

            if (oldFileName == null || oldFileName.Equals(newFileName))
                return;

            foreach (ITabbedDocument doc in PluginBase.MainForm.Documents)
                if (doc.FileName.Equals(oldFileName))
                {
                    doc.Save();
                    doc.Close();
                }

            File.Move(oldFileName, newFileName);
            PluginBase.MainForm.OpenEditableDocument(newFileName, false);

            //TODO: report
        }

        /// <summary>
        /// 
        /// </summary>
        private void ReportResults()
        {
            int newNameLength = NewName.Length;
            PluginBase.MainForm.CallCommand("PluginCommand", "ResultsPanel.ClearResults");
            // outputs the lines as they change
            // some funky stuff to make sure it highlights/reports the resultant changes rather than the old data
            // TODO: this works on the assumption that multiple changes on the same line will come from left-to-right; consider updating to work regardless of order
            foreach (KeyValuePair<String, List<SearchMatch>> entry in Results)
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
                    changedLine = changedLine.Substring(0, column) + NewName + changedLine.Substring(column + match.Length);
                    // stores the changes in case we have to modify the line again later
                    lineChanges[lineNumber] = changedLine;
                    lineOffsets[lineNumber] = offset + (newNameLength - match.Length);
                    // stores the line entry in our report set
                    if (!reportableLines.ContainsKey(lineNumber))
                        reportableLines[lineNumber] = new List<string>();
                    
                    // the data we store matches the TraceManager.Add's formatting.  We insert the {0} at the end so that we can insert the final line state later
                    reportableLines[lineNumber].Add(entry.Key + ":" + match.Line + ": characters " + column + "-" + (column + newNameLength) + " : {0}");
                }
                // report all the lines
                foreach (KeyValuePair<int, List<String>> lineSetsToReport in reportableLines)
                {
                    // the final state of the line after all renaming
                    String renamedLine = lineChanges[lineSetsToReport.Key].Trim();
                    foreach (String lineToReport in lineSetsToReport.Value)
                    {
                        // use the String.Format and replace the {0} from above with our final line state
                        TraceManager.Add(String.Format(lineToReport, renamedLine),  (Int32)TraceType.Info);
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
            String suggestion = originalName;
            LineEntryDialog askName = new LineEntryDialog(title, label, suggestion);
            DialogResult choice = askName.ShowDialog();
            if (choice == DialogResult.OK && askName.Line.Trim().Length > 0 && askName.Line.Trim() != originalName)
                return askName.Line.Trim();
            
            return null;
        }

        #endregion

    }

}
