using System;
using System.Collections.Generic;
using CodeRefactor.Commands;
using PluginCore;
using PluginCore.FRService;
using PluginCore.Managers;

namespace CodeRefactor.Provider
{
    class MovingHelper
    {
        private static readonly List<QueueItem> queue = new List<QueueItem>();
        private static readonly Dictionary<string, List<SearchMatch>> results = new Dictionary<string, List<SearchMatch>>();
        private static Move currentCommand;

        public static void AddToQueue(Dictionary<string, string> oldPathToNewPath)
        {
            AddToQueue(oldPathToNewPath, false);
        }

        public static void AddToQueue(Dictionary<string, string> oldPathToNewPath, bool outputResults)
        {
            AddToQueue(oldPathToNewPath, outputResults, false);
        }

        public static void AddToQueue(Dictionary<string, string> oldPathToNewPath, bool outputResults, bool renaming)
        {
            AddToQueue(oldPathToNewPath, outputResults, renaming, false);
        }

        public static void AddToQueue(Dictionary<string, string> oldPathToNewPath, bool outputResults, bool renaming, bool updatePackages)
        {
            queue.Add(new QueueItem(oldPathToNewPath, outputResults, renaming, updatePackages));
            if (currentCommand == null) ExecuteFirst();
        }

        private static void ExecuteFirst()
        {
            try
            {
                QueueItem item = queue[0];
                queue.Remove(item);
                currentCommand = new Move(item.OldPathToNewPath, item.OutputResults, item.Renaming, item.UpdatePackages);
                currentCommand.OnRefactorComplete += OnRefactorComplete;
                currentCommand.Execute();
            }
            catch(Exception ex)
            {
                queue.Clear();
                results.Clear();
                currentCommand = null;
                ErrorManager.ShowError(ex);
            }
        }

        private static void OnRefactorComplete(object sender, RefactorCompleteEventArgs<IDictionary<string, List<SearchMatch>>> e)
        {
            if (currentCommand.OutputResults)
            {
                foreach (KeyValuePair<string, List<SearchMatch>> entry in currentCommand.Results)
                {
                    string path = entry.Key;
                    if (!results.ContainsKey(path)) results[path] = new List<SearchMatch>();
                    results[path].AddRange(entry.Value);
                }
            }
            if (queue.Count > 0) ExecuteFirst();
            else
            {
                if (results.Count > 0) ReportResults();
                results.Clear();
                currentCommand = null;
            }
        }

        private static void ReportResults()
        {
            PluginBase.MainForm.CallCommand("PluginCommand", "ResultsPanel.ClearResults");
            foreach (KeyValuePair<string, List<SearchMatch>> entry in results)
            {
                Dictionary<int, int> lineOffsets = new Dictionary<int, int>();
                Dictionary<int, string> lineChanges = new Dictionary<int, string>();
                Dictionary<int, List<string>> reportableLines = new Dictionary<int, List<string>>();
                foreach (SearchMatch match in entry.Value)
                {
                    int column = match.Column;
                    int lineNumber = match.Line;
                    string changedLine = lineChanges.ContainsKey(lineNumber) ? lineChanges[lineNumber] : match.LineText;
                    int offset = lineOffsets.ContainsKey(lineNumber) ? lineOffsets[lineNumber] : 0;
                    column = column + offset;
                    lineChanges[lineNumber] = changedLine;
                    lineOffsets[lineNumber] = offset + (match.Value.Length - match.Length);
                    if (!reportableLines.ContainsKey(lineNumber)) reportableLines[lineNumber] = new List<string>();
                    reportableLines[lineNumber].Add(entry.Key + ":" + match.Line + ": chars " + column + "-" + (column + match.Value.Length) + " : {0}");
                }
                foreach (KeyValuePair<int, List<string>> lineSetsToReport in reportableLines)
                {
                    string renamedLine = lineChanges[lineSetsToReport.Key].Trim();
                    foreach (string lineToReport in lineSetsToReport.Value)
                    {
                        TraceManager.Add(string.Format(lineToReport, renamedLine), (int)TraceType.Info);
                    }
                }
            }
            PluginBase.MainForm.CallCommand("PluginCommand", "ResultsPanel.ShowResults");
        }
    }

    #region Helpers

    internal class QueueItem
    {
        public Dictionary<string, string> OldPathToNewPath;
        public bool OutputResults;
        public bool Renaming;
        public readonly bool UpdatePackages;

        public QueueItem(Dictionary<string, string> oldPathToNewPath, bool outputResults, bool renaming, bool updatePackages)
        {
            OldPathToNewPath = oldPathToNewPath;
            OutputResults = outputResults;
            Renaming = renaming;
            UpdatePackages = updatePackages;
        }
    }

    #endregion
}