using System;
using System.Collections.Generic;
using ASCompletion.Completion;
using PluginCore.FRService;
using CodeRefactor.Provider;
using PluginCore.Localization;
using PluginCore.Managers;
using ScintillaNet;
using PluginCore;

namespace CodeRefactor.Commands
{
    /// <summary>
    /// Finds all references to a given declaration.
    /// </summary>
    public class FindAllReferences : RefactorCommand<IDictionary<String, List<SearchMatch>>>
    {
        private ASResult currentTarget;
        private Boolean outputResults;
        private Boolean ignoreDeclarationSource;

        /// <summary>
        /// Gets or sets if searching is only performed on user defined classpaths
        /// </summary>
        public Boolean OnlySourceFiles { get; set; }

        /// <summary>
        /// The current declaration target that references are being found to.
        /// </summary>
        public ASResult CurrentTarget
        {
            get { return this.currentTarget; }
        }

        /// <summary>
        /// A new FindAllReferences refactoring command. Outputs found results.
        /// Uses the current text location as the declaration target.
        /// </summary>
        public FindAllReferences() : this(true)
        {
        }

        /// <summary>
        /// A new FindAllReferences refactoring command.
        /// Uses the current text location as the declaration target.
        /// </summary>
        /// <param name="output">If true, will send the found results to the trace log and results panel</param>
        public FindAllReferences(Boolean output) : this(RefactoringHelper.GetDefaultRefactorTarget(), output)
        {
            this.outputResults = output;
        }

        /// <summary>
        /// A new FindAllReferences refactoring command.
        /// </summary>
        /// <param name="target">The target declaration to find references to.</param>
        /// <param name="output">If true, will send the found results to the trace log and results panel</param>
        public FindAllReferences(ASResult target, Boolean output) : this(target, output, false)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="target">The target declaration to find references to.</param>
        /// <param name="output">If true, will send the found results to the trace log and results panel</param>
        public FindAllReferences(ASResult target, Boolean output, Boolean ignoreDeclarations)
        {
            this.currentTarget = target;
            this.outputResults = output;
            this.ignoreDeclarationSource = ignoreDeclarations;
        }

        #region RefactorCommand Implementation

        /// <summary>
        /// Entry point to execute finding.
        /// </summary>
        protected override void ExecutionImplementation()
        {
            UserInterfaceManager.ProgressDialog.Show();
            UserInterfaceManager.ProgressDialog.SetTitle(TextHelper.GetString("Info.FindingReferences"));
            UserInterfaceManager.ProgressDialog.UpdateStatusMessage(TextHelper.GetString("Info.SearchingFiles"));
            RefactoringHelper.FindTargetInFiles(currentTarget, new FRProgressReportHandler(this.RunnerProgress), new FRFinishedHandler(this.FindFinished), true, OnlySourceFiles);
        }

        /// <summary>
        /// Indicates if the current settings for the refactoring are valid.
        /// </summary>
        public override Boolean IsValid()
        {
            return this.currentTarget != null;
        }

        #endregion

        #region Private Helper Methods

        /// <summary>
        /// Invoked as the FRSearch gathers results.
        /// </summary>
        private void RunnerProgress(Int32 percentDone)
        {
            // perhaps we should show some progress to the user, especially if there are a lot of files to check...
            UserInterfaceManager.ProgressDialog.UpdateProgress(percentDone);
        }

        /// <summary>
        /// Invoked when the FRSearch completes its search
        /// </summary>
        private void FindFinished(FRResults results)
        {

            UserInterfaceManager.ProgressDialog.Reset();
            UserInterfaceManager.ProgressDialog.UpdateStatusMessage(TextHelper.GetString("Info.ResolvingReferences"));
            // First filter out any results that don't actually point to our source declaration
            this.Results = ResolveActualMatches(results, currentTarget);
            if (this.outputResults) this.ReportResults();
            UserInterfaceManager.ProgressDialog.Hide();
            // Select first match
            if (this.Results.Count > 0)
            {
                foreach (var fileEntries in this.Results)
                {

                    if (fileEntries.Value.Count > 0 && System.IO.File.Exists(fileEntries.Key))
                    {
                        SearchMatch entry = fileEntries.Value[0];
                        PluginBase.MainForm.OpenEditableDocument(fileEntries.Key, false);
                        RefactoringHelper.SelectMatch(PluginBase.MainForm.CurrentDocument.SciControl, entry);
                        break;
                    }
                }
            }
            this.FireOnRefactorComplete();
        }

        /// <summary>
        /// Filters the initial result set by determining which entries actually resolve back to our declaration target.
        /// </summary>
        private IDictionary<String, List<SearchMatch>> ResolveActualMatches(FRResults results, ASResult target)
        {
            // this will hold actual references back to the source member (some result hits could point to different members with the same name)
            IDictionary<String, List<SearchMatch>> actualMatches = new Dictionary<String, List<SearchMatch>>();
            IDictionary<String, List<SearchMatch>> initialResultsList = RefactoringHelper.GetInitialResultsList(results);
            int matchesChecked = 0; int totalMatches = 0;
            foreach (KeyValuePair<String, List<SearchMatch>> entry in initialResultsList)
            {
                totalMatches += entry.Value.Count;
            }
            Boolean foundDeclarationSource = false;
            foreach (KeyValuePair<String, List<SearchMatch>> entry in initialResultsList)
            {
                String currentFileName = entry.Key;
                UserInterfaceManager.ProgressDialog.UpdateStatusMessage(TextHelper.GetString("Info.ResolvingReferencesIn") + " \"" + currentFileName + "\"");
                foreach (SearchMatch match in entry.Value)
                {
                    // we have to open/reopen the entry's file
                    // there are issues with evaluating the declaration targets with non-open, non-current files
                    // we have to do it each time as the process of checking the declaration source can change the currently open file!
                    ScintillaControl sci = this.AssociatedDocumentHelper.LoadDocument(currentFileName);
                    // if the search result does point to the member source, store it
                    if (RefactoringHelper.DoesMatchPointToTarget(sci, match, target, this.AssociatedDocumentHelper))
                    {
                        if (ignoreDeclarationSource && !foundDeclarationSource && RefactoringHelper.IsMatchTheTarget(sci, match, target))
                        {
                            //ignore the declaration source
                            foundDeclarationSource = true;
                        }
                        else
                        {
                            if (!actualMatches.ContainsKey(currentFileName))
                            {
                                actualMatches.Add(currentFileName, new List<SearchMatch>());
                            }
                            actualMatches[currentFileName].Add(match);
                        }
                    }
                    matchesChecked++;
                    UserInterfaceManager.ProgressDialog.UpdateProgress((100 * matchesChecked) / totalMatches);
                }
            }
            this.AssociatedDocumentHelper.CloseTemporarilyOpenedDocuments();
            return actualMatches;
        }

        /// <summary>
        /// Outputs the results to the TraceManager
        /// </summary>
        private void ReportResults()
        {
            PluginBase.MainForm.CallCommand("PluginCommand", "ResultsPanel.ClearResults");
            foreach (KeyValuePair<String, List<SearchMatch>> entry in this.Results)
            {
                // Outputs the lines as they change
                foreach (SearchMatch match in entry.Value)
                {
                    TraceManager.Add(entry.Key + ":" + match.Line + ": chars " + match.Column + "-" + (match.Column + match.Length) + " : " + match.LineText.Trim(), (Int32)TraceType.Info);
                }
            }
            PluginBase.MainForm.CallCommand("PluginCommand", "ResultsPanel.ShowResults");
        }

        #endregion

    }

}
