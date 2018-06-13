using System;
using System.Collections.Generic;
using System.IO;
using ASCompletion.Completion;
using CodeRefactor.Provider;
using PluginCore;
using PluginCore.Controls;
using PluginCore.FRService;
using PluginCore.Localization;
using PluginCore.Managers;

namespace CodeRefactor.Commands
{
    /// <summary>
    /// Finds all references to a given declaration.
    /// </summary>
    public class FindAllReferences : RefactorCommand<IDictionary<String, List<SearchMatch>>>
    {
        internal const string TraceGroup = "CodeRefactor.FindAllReferences";

        protected bool IgnoreDeclarationSource { get; private set; }

        /// <summary>
        /// Gets or sets if searching is only performed on user defined classpaths
        /// </summary>
        public Boolean OnlySourceFiles { get; set; }

        public bool IncludeComments { get; set; }

        public bool IncludeStrings { get; set; }
        
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
        /// A new FindAllReferences refactoring command.
        /// </summary>
        /// <param name="target">The target declaration to find references to.</param>
        /// <param name="output">If true, will send the found results to the trace log and results panel</param>
        /// <param name="ignoreDeclarations"></param>
        public FindAllReferences(ASResult target, Boolean output, Boolean ignoreDeclarations)
        {
            CurrentTarget = target;
            OutputResults = output;
            IgnoreDeclarationSource = ignoreDeclarations;
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
            RefactoringHelper.FindTargetInFiles(CurrentTarget, RunnerProgress, FindFinished, true, OnlySourceFiles, true, IncludeComments, IncludeStrings);
        }

        /// <summary>
        /// Indicates if the current settings for the refactoring are valid.
        /// </summary>
        public override Boolean IsValid() => CurrentTarget != null;

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
        protected void FindFinished(FRResults results)
        {
            UserInterfaceManager.ProgressDialog.Reset();
            UserInterfaceManager.ProgressDialog.UpdateStatusMessage(TextHelper.GetString("Info.ResolvingReferences"));
            MessageBar.Locked = true;
            // First filter out any results that don't actually point to our source declaration
            this.Results = ResolveActualMatches(results, CurrentTarget);
            if (OutputResults) this.ReportResults();
            MessageBar.Locked = false;
            UserInterfaceManager.ProgressDialog.Hide();
            // Select first match
            if (this.Results.Count > 0)
            {
                foreach (var fileEntries in this.Results)
                {
                    if (fileEntries.Value.Count > 0 && File.Exists(fileEntries.Key))
                    {
                        SearchMatch entry = fileEntries.Value[0];
                        var doc = (ITabbedDocument)PluginBase.MainForm.OpenEditableDocument(fileEntries.Key, false);
                        RefactoringHelper.SelectMatch(doc.SciControl, entry);
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
            var actualMatches = new Dictionary<String, List<SearchMatch>>();
            var initialResultsList = RefactoringHelper.GetInitialResultsList(results);
            int matchesChecked = 0;
            int totalMatches = 0;
            foreach (KeyValuePair<String, List<SearchMatch>> entry in initialResultsList)
            {
                totalMatches += entry.Value.Count;
            }
            var foundDeclarationSource = false;
            var optionsEnabled = IncludeComments || IncludeStrings;
            foreach (var entry in initialResultsList)
            {
                var currentFileName = entry.Key;
                UserInterfaceManager.ProgressDialog.UpdateStatusMessage(TextHelper.GetString("Info.ResolvingReferencesIn") + " \"" + currentFileName + "\"");
                foreach (var match in entry.Value)
                {
                    // we have to open/reopen the entry's file
                    // there are issues with evaluating the declaration targets with non-open, non-current files
                    // we have to do it each time as the process of checking the declaration source can change the currently open file!
                    var sci = this.AssociatedDocumentHelper.LoadDocument(currentFileName).SciControl;
                    // if the search result does point to the member source, store it
                    var add = false;
                    if (RefactoringHelper.DoesMatchPointToTarget(sci, match, target, this.AssociatedDocumentHelper))
                    {
                        if (IgnoreDeclarationSource && !foundDeclarationSource && RefactoringHelper.IsMatchTheTarget(sci, match, target, AssociatedDocumentHelper))
                        {
                            //ignore the declaration source
                            foundDeclarationSource = true;
                        }
                        else
                        {
                            add = true;
                        }
                    }
                    else if (optionsEnabled)
                    {
                        add = RefactoringHelper.IsInsideCommentOrString(match, sci, IncludeComments, IncludeStrings);
                    }

                    if (add)
                    {
                        if (!actualMatches.ContainsKey(currentFileName))
                            actualMatches.Add(currentFileName, new List<SearchMatch>());

                        actualMatches[currentFileName].Add(match);
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
            var groupData = TraceManager.CreateGroupDataUnique(TraceGroup, CurrentTarget.Member == null ? CurrentTarget.Type.Name : CurrentTarget.Member.Name);
            PluginBase.MainForm.CallCommand("PluginCommand", "ResultsPanel.ClearResults;" + groupData);
            foreach (var entry in Results)
            {
                // Outputs the lines as they change
                foreach (var match in entry.Value)
                {
                    string message = $"{entry.Key}:{match.Line}: chars {match.Column}-{match.Column + match.Length} : {match.LineText.Trim()}";
                    TraceManager.Add(message, (int) TraceType.Info, groupData);
                }
            }
            PluginBase.MainForm.CallCommand("PluginCommand", "ResultsPanel.ShowResults;" + groupData);
        }

        #endregion

    }

}
