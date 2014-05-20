using System;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using ASCompletion.Completion;
using ASCompletion.Context;
using ASCompletion.Model;
using CodeRefactor.Provider;
using PluginCore.Controls;
using PluginCore.FRService;
using PluginCore.Localization;
using ScintillaNet;
using PluginCore;

namespace CodeRefactor.Commands
{
    class Move : RefactorCommand<IDictionary<String, List<SearchMatch>>>
    {
        private Dictionary<string, string> oldPathToNewPath;
        private bool outputResults;
        private bool renaming;
        private List<MoveTargetHelper> targets;
        private MoveTargetHelper currentTarget;

        #region Constructors

        /// <summary>
        /// A new Move refactoring command.
        /// </summary>
        public Move(Dictionary<string, string>oldPathToNewPath) : this(oldPathToNewPath, true)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public Move(Dictionary<string, string> oldPathToNewPath, bool outputResults) : this(oldPathToNewPath, outputResults, false)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public Move(Dictionary<string, string> oldPathToNewPath, bool outputResults, bool renaming)
        {
            this.oldPathToNewPath = oldPathToNewPath;
            this.outputResults = outputResults;
            this.renaming = renaming;
            Results = new Dictionary<string, List<SearchMatch>>();
            CreateListOfMoveTargets();
        }

        #endregion

        #region RefactorCommand Implementation

        /// <summary>
        /// 
        /// </summary>
        protected override void ExecutionImplementation()
        {
            string msg;
            string title = "";
            if (renaming)
            {
                msg = TextHelper.GetString("Info.RenamingDirectory");
                foreach (KeyValuePair<string, string> item in oldPathToNewPath)
                {
                    title = string.Format(TextHelper.GetString("Title.RenameDialog"), Path.GetFileName(item.Key));
                    break;
                }
            }
            else
            {
                msg = TextHelper.GetString("Info.MovingFile");
                title = TextHelper.GetString("Title.MoveDialog");
            }
            if (MessageBox.Show(msg, title, MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                MoveTargets();
                UpdateReferencesNextTarget();
            }
            else
            {
                MoveTargets();
                FireOnRefactorComplete();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override bool IsValid()
        {
            return oldPathToNewPath != null;
        }

        #endregion

        #region Private Helper Methods

        /// <summary>
        /// 
        /// </summary>
        private void CreateListOfMoveTargets()
        {
            targets = new List<MoveTargetHelper>();
            foreach (KeyValuePair<string, string> item in oldPathToNewPath)
            {
                string oldPath = item.Key;
                string newPath = item.Value;
                if (File.Exists(oldPath))
                {
                    if (IsValidFile(oldPath)) targets.Add(GetMoveTarget(oldPath, Path.Combine(item.Value, Path.GetFileName(oldPath))));
                }
                else if(Directory.Exists(oldPath))
                {
                    newPath = renaming ? Path.Combine(Path.GetDirectoryName(oldPath), newPath) : Path.Combine(newPath, Path.GetFileName(oldPath));
                    foreach (string oldFilePath in Directory.GetFiles(oldPath, "*.*", SearchOption.AllDirectories))
                    {
                        if (IsValidFile(oldFilePath)) targets.Add(GetMoveTarget(oldFilePath, oldFilePath.Replace(oldPath, newPath)));
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private bool IsValidFile(string file)
        {
            if (PluginBase.CurrentProject == null) return false;
            string ext = Path.GetExtension(file);
            return ext == ".as" || ext == ".hx" || ext == ".ls" && PluginBase.CurrentProject.DefaultSearchFilter.Contains(ext);
        }

        /// <summary>
        /// 
        /// </summary>
        private MoveTargetHelper GetMoveTarget(string oldFilePath, string newPath)
        {
            MoveTargetHelper result = new MoveTargetHelper();
            result.OldFilePath = oldFilePath;
            result.OldFileModel = ASContext.Context.GetFileModel(oldFilePath);
            result.NewFilePath = newPath;
            ProjectManager.Projects.Project project = (ProjectManager.Projects.Project)PluginBase.CurrentProject;
            string newPackage = project.GetAbsolutePath(Path.GetDirectoryName(newPath));
            if (!string.IsNullOrEmpty(newPackage))
            {
                foreach (PathModel pathModel in ASContext.Context.Classpath)
                {
                    string path = project.GetAbsolutePath(pathModel.Path);
                    if (path == newPackage)
                    {
                        newPackage = "";
                        break;
                    }
                    else if (newPackage.StartsWith(path))
                    {
                        newPackage = newPackage.Substring((path + "\\").Length).Replace("\\", ".");
                        break;
                    }
                }
            }
            result.NewPackage = newPackage;
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        private void MoveTargets()
        {
            Dictionary<string, ITabbedDocument> fileNameToOpenedDoc = new Dictionary<string, ITabbedDocument>();
            foreach (ITabbedDocument doc in PluginBase.MainForm.Documents)
            {
                fileNameToOpenedDoc.Add(doc.FileName, doc);
            }
            MessageBar.Locked = true;
            foreach (KeyValuePair<string, string> item in oldPathToNewPath)
            {
                string oldPath = item.Key;
                string newPath = item.Value;
                if (Path.HasExtension(oldPath))
                {
                    if (fileNameToOpenedDoc.ContainsKey(oldPath))
                    {
                        fileNameToOpenedDoc[oldPath].Save();
                        fileNameToOpenedDoc[oldPath].Close();
                    }
                    newPath = Path.Combine(item.Value, Path.GetFileName(oldPath));
                    // refactor failed or was refused
                    if (Path.GetFileName(oldPath).Equals(newPath, StringComparison.OrdinalIgnoreCase))
                    {
                        // name casing changed
                        string tmpPath = oldPath + "$renaming$";
                        File.Move(oldPath, tmpPath);
                        oldPath = tmpPath;
                    }
                    if (!Path.IsPathRooted(newPath)) newPath = Path.Combine(Path.GetDirectoryName(oldPath), newPath);
                    RefactoringHelper.Move(oldPath, newPath);
                }
                else
                {
                    foreach (string file in Directory.GetFiles(oldPath, "*.*", SearchOption.AllDirectories))
                    {
                        if (fileNameToOpenedDoc.ContainsKey(file))
                        {
                            fileNameToOpenedDoc[file].Save();
                            fileNameToOpenedDoc[file].Close();
                        }
                    }
                    RefactoringHelper.Move(oldPath, newPath, renaming);
                }
            }
            MessageBar.Locked = false;
        }

        /// <summary>
        /// 
        /// </summary>
        private void UpdateReferencesNextTarget()
        {
            if (targets.Count > 0)
            {
                currentTarget = targets[0];
                targets.Remove(currentTarget);
                FileModel oldFileModel = currentTarget.OldFileModel;
                FRSearch search;
                string newType;
                if (string.IsNullOrEmpty(oldFileModel.Package))
                {
                    search = new FRSearch("(package)+\\s*");
                    newType = Path.GetFileNameWithoutExtension(currentTarget.OldFilePath);
                }
                else
                {
                    search = new FRSearch("(package)+\\s+(" + oldFileModel.Package + ")\\s*");
                    newType = oldFileModel.Package + "." + Path.GetFileNameWithoutExtension(currentTarget.OldFilePath);
                }
                search.IsRegex = true;
                search.Filter = SearchFilter.None;
                newType = newType.Trim('.');
                MessageBar.Locked = true;
                string newFilePath = currentTarget.NewFilePath;
                ScintillaControl sci = AssociatedDocumentHelper.LoadDocument(newFilePath);
                List<SearchMatch> matches = search.Matches(sci.Text);
                RefactoringHelper.ReplaceMatches(matches, sci, "package " + currentTarget.NewPackage + " ", null);
                int offset = "package ".Length;
                foreach (SearchMatch match in matches)
                {
                    match.Column += offset;
                    match.LineText = sci.GetLine(match.Line - 1);
                    match.Value = currentTarget.NewPackage;
                }
                if (!Results.ContainsKey(newFilePath)) Results[newFilePath] = new List<SearchMatch>();
                Results[newFilePath].AddRange(matches.ToArray());
                PluginBase.MainForm.CurrentDocument.Save();
                if (sci.IsModify) AssociatedDocumentHelper.MarkDocumentToKeep(currentTarget.OldFilePath);
                MessageBar.Locked = false;
                UserInterfaceManager.ProgressDialog.Show();
                UserInterfaceManager.ProgressDialog.SetTitle(TextHelper.GetString("Info.FindingReferences"));
                UserInterfaceManager.ProgressDialog.UpdateStatusMessage(TextHelper.GetString("Info.SearchingFiles"));
                ASResult target = new ASResult() { Member = new MemberModel(newType, newType, FlagType.Import, 0) };
                RefactoringHelper.FindTargetInFiles(target, UserInterfaceManager.ProgressDialog.UpdateProgress, FindFinished, true);
            }
            else
            {
                if (outputResults) ReportResults();
                FireOnRefactorComplete();
            }
        }

        private void ReportResults()
        {
            PluginBase.MainForm.CallCommand("PluginCommand", "ResultsPanel.ClearResults");
            foreach (KeyValuePair<string, List<SearchMatch>> entry in Results)
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
                    reportableLines[lineNumber].Add(entry.Key + ":" + match.Line + ": characters " + column + "-" + (column + match.Value.Length) + " : {0}");
                }
                foreach (KeyValuePair<int, List<string>> lineSetsToReport in reportableLines)
                {
                    string renamedLine = lineChanges[lineSetsToReport.Key].Trim();
                    foreach (string lineToReport in lineSetsToReport.Value)
                    {
                        PluginCore.Managers.TraceManager.Add(string.Format(lineToReport, renamedLine), (int)TraceType.Info);
                    }
                }
            }
            PluginBase.MainForm.CallCommand("PluginCommand", "ResultsPanel.ShowResults");
        }

        #endregion

        #region Event Handlers

        private void FindFinished(FRResults results)
        {
            UserInterfaceManager.ProgressDialog.Show();
            UserInterfaceManager.ProgressDialog.SetTitle(TextHelper.GetString("Info.UpdatingReferences"));
            MessageBar.Locked = true;
            bool isNotHaxe = !PluginBase.CurrentProject.Language.StartsWith("haxe");
            bool packageIsNotEmpty = !string.IsNullOrEmpty(currentTarget.OldFileModel.Package);
            string targetName = Path.GetFileNameWithoutExtension(currentTarget.OldFilePath);
            string oldType = (currentTarget.OldFileModel.Package + "." + targetName).Trim('.');
            string newType = (currentTarget.NewPackage + "." + targetName).Trim('.');
            foreach (KeyValuePair<string, List<SearchMatch>> entry in results)
            {
                List<SearchMatch> matches = entry.Value;
                if (matches.Count == 0) continue;
                string path = entry.Key;
                UserInterfaceManager.ProgressDialog.UpdateStatusMessage(TextHelper.GetString("Info.Updating") + " \"" + path + "\"");
                ScintillaControl sci = AssociatedDocumentHelper.LoadDocument(path);
                if (isNotHaxe && path != currentTarget.NewFilePath && ASContext.Context.CurrentModel.Imports.Search(targetName, FlagType.Class & FlagType.Function & FlagType.Namespace, 0) == null)
                {
                    ASGenerator.InsertImport(new MemberModel(targetName, newType, FlagType.Import, 0), false);
                }
                if (packageIsNotEmpty) RefactoringHelper.ReplaceMatches(matches, sci, newType, null);
                else
                {
                    foreach (SearchMatch sm in matches)
                    {
                        if (sm.LineText.TrimStart().StartsWith("import"))
                        {
                            RefactoringHelper.SelectMatch(sci, sm);
                            sci.ReplaceSel(newType);
                        }
                    }
                }
                foreach (SearchMatch match in matches)
                {
                    match.LineText = sci.GetLine(match.Line - 1);
                    match.Value = newType;
                }
                if (!Results.ContainsKey(path)) Results[path] = new List<SearchMatch>();
                Results[path].AddRange(matches.ToArray());
                PluginBase.MainForm.CurrentDocument.Save();
                if (sci.IsModify) AssociatedDocumentHelper.MarkDocumentToKeep(path);
            }
            UserInterfaceManager.ProgressDialog.Hide();
            MessageBar.Locked = false;
            UpdateReferencesNextTarget();
        }

        #endregion
    }

    #region Helpers

    internal class MoveTargetHelper
    {
        public string OldFilePath;
        public FileModel OldFileModel;
        public string NewFilePath;
        public string NewPackage;

        public MoveTargetHelper()
        {
        }
    }

    #endregion
}