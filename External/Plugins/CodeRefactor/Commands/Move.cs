using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
using ProjectManager.Projects;
using ScintillaNet;

/* Considerations and known problems: 
 *  A. When moving a model that makes use of other models in the old package there will be a problem
 *  as new needed imports aren't added, there are two solutions available:
 *     1. Scan for each model references with respect to the class being moved and add the import if needed.
 *     2. Add an * import if there are remaining files in the old package and forget about it.
 *  At any rate, it should be discussed first if this is really a task for this refactoring command.
 *  B. Adding needed imports currently fails if both public and private classes need of it.
 *  C. The resulting code isn't 100% right if there is another member with the same name imported. It thinks
 *  it's already imported, and would require to fully qualify all references.
 *  D. If a model already importing the type is in the new package, the import declaration could be deleted,
 *  but it's kept.
 *  E. If a model imports the whole new package, it doesn't need to import the class, but it currently does.
 *  
 * For some of these points using Context.IsImported would help a bit, although not entirely, and would require
 * several calls with different parameters to cover all possibilities.
 */
namespace CodeRefactor.Commands
{
    class Move : RefactorCommand<IDictionary<string, List<SearchMatch>>>
    {

        public Dictionary<string, string> OldPathToNewPath;
        public bool OutputResults;
        private bool renaming;
        private List<MoveTargetHelper> targets;
        private List<string> filesToReopen;
        private int currentTargetIndex;
        private ASResult currentTargetResult;

        private bool targetsOutsideClasspath;

        #region Constructors

        /// <summary>
        /// A new Move refactoring command.
        /// </summary>
        public Move(Dictionary<string, string>oldPathToNewPath) : this(oldPathToNewPath, true)
        {
        }

        public Move(Dictionary<string, string> oldPathToNewPath, bool outputResults) : this(oldPathToNewPath, outputResults, false)
        {
        }

        public Move(Dictionary<string, string> oldPathToNewPath, bool outputResults, bool renaming)
        {
            OldPathToNewPath = oldPathToNewPath;
            OutputResults = outputResults;
            this.renaming = renaming;
            Results = new Dictionary<string, List<SearchMatch>>();
        }

        #endregion

        #region RefactorCommand Implementation

        protected override void ExecutionImplementation()
        {
            string msg;
            string title = null;

            // To get the initial open documents, finding all references will interfere if we try later
            // We may already have an AssociatedDocumentHelper
            RegisterDocumentHelper(AssociatedDocumentHelper);

            CreateListOfMoveTargets();

            if (targetsOutsideClasspath)
            {
                msg = TextHelper.GetString("Info.MovingOutsideClasspath");
                title = TextHelper.GetString("FlashDevelop.Title.WarningDialog");
                if (MessageBox.Show(msg, title, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    MoveTargets();
                    ReopenInitialFiles();
                }
                FireOnRefactorComplete();
                return;
            }

            if (renaming)
            {
                msg = TextHelper.GetString("Info.RenamingDirectory");
                foreach (string path in OldPathToNewPath.Keys)
                {
                    title = string.Format(TextHelper.GetString("Title.RenameDialog"), Path.GetFileName(path));
                    break;
                }
            }
            else
            {
                msg = TextHelper.GetString("Info.MovingFile");
                title = TextHelper.GetString("Title.MoveDialog");
            }
            var dialogResult = MessageBox.Show(msg, title, MessageBoxButtons.YesNoCancel);
            if (dialogResult == DialogResult.Cancel)
            {
                FireOnRefactorComplete();
                return;
            }
            if (targets.Count > 0 && dialogResult == DialogResult.Yes)
            {
                // We must keep the original files for validating references
                CopyTargets();
                UpdateReferencesNextTarget();
            }
            else
            {
                MoveTargets();
                ReopenInitialFiles();
                FireOnRefactorComplete();
            }
        }

        public override bool IsValid()
        {
            return OldPathToNewPath != null;
        }

        #endregion

        #region Private Helper Methods

        private void CreateListOfMoveTargets()
        {
            targets = new List<MoveTargetHelper>();
            filesToReopen = new List<string>();
            IProject project = PluginBase.CurrentProject;
            if (project == null) return;
            string filterMask = project.DefaultSearchFilter;
            foreach (KeyValuePair<string, string> item in OldPathToNewPath)
            {
                string oldPath = item.Key;
                string newPath = item.Value;
                ITabbedDocument doc;
                if (File.Exists(oldPath))
                {
                    newPath = Path.Combine(newPath, Path.GetFileName(oldPath));

                    if (AssociatedDocumentHelper.InitiallyOpenedFiles.TryGetValue(oldPath, out doc))
                    {
                        doc.Save();
                        doc.Close();

                        filesToReopen.Add(newPath);

                        // We need to remove it from the collection so later it may be closed by CloseTemporarilyOpenedDocuments,
                        // otherwise there may be problems updating contents.
                        AssociatedDocumentHelper.InitiallyOpenedFiles.Remove(oldPath);
                    }
                    if (AssociatedDocumentHelper.InitiallyOpenedFiles.TryGetValue(newPath, out doc))
                    {
                        doc.Save();
                        doc.Close();
                        // We need to remove it from the collection so later it may be closed by CloseTemporarilyOpenedDocuments,
                        // otherwise there may be problems updating contents.
                        AssociatedDocumentHelper.InitiallyOpenedFiles.Remove(newPath);
                    }

                    if (FileHelper.FileMatchesSearchFilter(oldPath, filterMask))
                    {
                        var target = GetMoveTarget(oldPath, newPath, null);
                        if (target == null) continue;
                        targets.Add(target);
                    }
                }
                else if(Directory.Exists(oldPath))
                {
                    newPath = renaming ? Path.Combine(Path.GetDirectoryName(oldPath), newPath) : Path.Combine(newPath, Path.GetFileName(oldPath));

                    CloseDocuments(oldPath, newPath);

                    // Do not load every file and check for matches to save memory and computation time
                    foreach (string mask in filterMask.Split(';'))
                        foreach (string oldFilePath in Directory.GetFiles(oldPath, mask, SearchOption.AllDirectories))
                        {
                            var target = GetMoveTarget(oldFilePath, oldFilePath.Replace(oldPath, newPath), oldPath);
                            // If target == null there's no chance of the others being valid, actually, neither on the outer loop
                            if (target == null) break;
                            targets.Add(target);
                        }
                }
            }
        }

        private MoveTargetHelper GetMoveTarget(string oldFilePath, string newPath, string ownerPath)
        {
            MoveTargetHelper result = new MoveTargetHelper();
            result.OldFilePath = oldFilePath;
            result.OldFileModel = ASContext.Context.GetFileModel(oldFilePath);
            result.NewFilePath = newPath;
            result.OwnerPath = ownerPath;
            IProject project = PluginBase.CurrentProject;
            string newPackage = result.NewPackage = project.GetAbsolutePath(Path.GetDirectoryName(newPath));
            if (!string.IsNullOrEmpty(newPackage))
            {
                var basePaths = project.SourcePaths.Length == 0 ? new[] {Path.GetDirectoryName(project.ProjectPath)} : project.SourcePaths;
                var lookupPaths = basePaths.
                    Concat(ProjectManager.PluginMain.Settings.GetGlobalClasspaths(project.Language)).
                    Select(project.GetAbsolutePath).Distinct();

                foreach (string path in lookupPaths)
                {
                    if (path == newPackage)
                    {
                        newPackage = "";
                        break;
                    }
                    if (newPackage.StartsWith(path))
                    {
                        newPackage = newPackage.Substring((path + "\\").Length).Replace("\\", ".");
                        break;
                    }
                }
            }

            if (result.NewPackage == newPackage)
            {
                targetsOutsideClasspath = true;
                return null;
            }
            if (newPackage == result.OldFileModel.FullPackage && Path.GetFileName(result.OldFileModel.FileName) == Path.GetFileName(newPath))
            {
                // moving file to another classpath at the same level, no need to refactor
                return null;
            }
            result.NewPackage = newPackage;
            return result;
        }

        private void CopyTargets()
        {
            MessageBar.Locked = true;
            foreach (var target in targets)
            {
                string oldPath = target.OldFilePath;
                string newPath = target.NewFilePath;
                if (File.Exists(oldPath))
                {
                    if (oldPath.Equals(newPath, StringComparison.OrdinalIgnoreCase))
                    {
                        // name casing changed
                        // we cannot append to the extension, as it will break finding possibly needed references
                        // we don't use folders to avoid several possible problems and ease some logic
                        newPath = target.TmpFilePath = Path.Combine(Path.GetDirectoryName(oldPath),
                                                                    Path.GetFileNameWithoutExtension(oldPath) +
                                                                    "$renaming$" +
                                                                    Path.GetExtension(oldPath));
                    }
                    if (!Path.IsPathRooted(newPath)) newPath = Path.Combine(Path.GetDirectoryName(oldPath), newPath);
                    string newDirectory = Path.GetDirectoryName(newPath);
                    if (!Directory.Exists(newDirectory)) Directory.CreateDirectory(newDirectory);
                    RefactoringHelper.Copy(oldPath, newPath, true, true);
                }
            }
            MessageBar.Locked = false;
        }

        private void MoveTargets()
        {
            MessageBar.Locked = true;
            foreach (KeyValuePair<string, string> item in OldPathToNewPath)
            {
                string oldPath = item.Key;
                string newPath = item.Value;
                if (File.Exists(oldPath))
                {
                    newPath = Path.Combine(newPath, Path.GetFileName(oldPath));
                    // refactor failed or was refused
                    if (Path.GetFileName(oldPath).Equals(newPath, StringComparison.OrdinalIgnoreCase))
                    {
                        // name casing changed
                        string tmpPath = oldPath + "$renaming$";
                        RefactoringHelper.Move(oldPath, tmpPath);
                        oldPath = tmpPath;
                    }
                    if (!Path.IsPathRooted(newPath)) newPath = Path.Combine(Path.GetDirectoryName(oldPath), newPath);
                    RefactoringHelper.Move(oldPath, newPath, true);
                }
                else if (Directory.Exists(oldPath))
                {
                    if (Path.GetFileName(oldPath).Equals(newPath, StringComparison.OrdinalIgnoreCase))
                    {
                        // name casing changed
                        string tmpPath = oldPath + "$renaming$";
                        RefactoringHelper.Move(oldPath, tmpPath);
                        oldPath = tmpPath;
                    }
                    RefactoringHelper.Move(oldPath, newPath, renaming);
                }
            }
            MessageBar.Locked = false;
        }

        private void CloseDocuments(string oldDirectory, string newDirectory)
        {
            string oldPath = oldDirectory.Contains(Path.DirectorySeparatorChar.ToString())
                                 ? oldDirectory + Path.DirectorySeparatorChar
                                 : oldDirectory + Path.AltDirectorySeparatorChar;
            string newPath = newDirectory.Contains(Path.DirectorySeparatorChar.ToString())
                                 ? newDirectory + Path.DirectorySeparatorChar
                                 : newDirectory + Path.AltDirectorySeparatorChar;

            var fileMatches = new List<string>();
            foreach (var item in AssociatedDocumentHelper.InitiallyOpenedFiles)
            {
                if (item.Key.StartsWith(oldPath))
                {
                    item.Value.Save();
                    item.Value.Close();
                    filesToReopen.Add(item.Key.Replace(oldPath, newPath));
                    fileMatches.Add(item.Key);
                }
                else if (item.Key.StartsWith(newPath))
                {
                    item.Value.Save();
                    item.Value.Close();
                    fileMatches.Add(item.Key);
                }
            }

            // We need to remove files from the collection so later it may be closed by CloseTemporarilyOpenedDocuments,
            // otherwise there may be problems refreshing contents.
            foreach (var file in fileMatches) AssociatedDocumentHelper.InitiallyOpenedFiles.Remove(file);
        }

        private void UpdateReferencesNextTarget()
        {
            if (currentTargetIndex < targets.Count)
            {
                var currentTarget = targets[currentTargetIndex];
                FileModel oldFileModel = currentTarget.OldFileModel;
                FRSearch search;
                string oldType;
                if (string.IsNullOrEmpty(oldFileModel.Package))
                {
                    search = new FRSearch("package");
                    search.WholeWord = true;
                    oldType = Path.GetFileNameWithoutExtension(currentTarget.OldFilePath);
                }
                else
                {
                    search = new FRSearch("package\\s+(" + oldFileModel.Package + ")");
                    oldType = oldFileModel.Package + "." + Path.GetFileNameWithoutExtension(currentTarget.OldFilePath);
                }
                search.IsRegex = true;
                search.Filter = SearchFilter.None;
                oldType = oldType.Trim('.');
                MessageBar.Locked = true;
                string newFilePath = currentTarget.NewFilePath;
                var doc = AssociatedDocumentHelper.LoadDocument(currentTarget.TmpFilePath ?? newFilePath);
                ScintillaControl sci = doc.SciControl;
                search.SourceFile = sci.FileName;
                List<SearchMatch> matches = search.Matches(sci.Text);
                string packageReplacement = "package";
                if (currentTarget.NewPackage != "")
                    packageReplacement += " " + currentTarget.NewPackage;
                RefactoringHelper.ReplaceMatches(matches, sci, packageReplacement);
                int offset = "package ".Length;
                foreach (SearchMatch match in matches)
                {
                    match.Column += offset;
                    match.LineText = sci.GetLine(match.Line - 1);
                    match.Value = currentTarget.NewPackage;
                }
                if (matches.Count > 0)
                {
                    if (!Results.ContainsKey(newFilePath)) Results[newFilePath] = new List<SearchMatch>();
                    Results[newFilePath].AddRange(matches);
                }
                else if (sci.ConfigurationLanguage == "haxe")
                {
                    // haxe modules don't need to specify a package if it's empty
                    sci.InsertText(0, packageReplacement + ";\n\n");
                }
                //Do we want to open modified files?
                //if (sci.IsModify) AssociatedDocumentHelper.MarkDocumentToKeep(file);
                doc.Save();
                MessageBar.Locked = false;
                UserInterfaceManager.ProgressDialog.Show();
                UserInterfaceManager.ProgressDialog.SetTitle(TextHelper.GetString("Info.FindingReferences"));
                UserInterfaceManager.ProgressDialog.UpdateStatusMessage(TextHelper.GetString("Info.SearchingFiles"));
                currentTargetResult = RefactoringHelper.GetRefactorTargetFromFile(oldFileModel.FileName, AssociatedDocumentHelper);
                if (currentTargetResult != null)
                {
                    RefactoringHelper.FindTargetInFiles(currentTargetResult, UserInterfaceManager.ProgressDialog.UpdateProgress, FindFinished, true, true, true);
                }
                else
                {
                    currentTargetIndex++;
                    UserInterfaceManager.ProgressDialog.Hide();
                    UpdateReferencesNextTarget();
                }
            }
            else
            {
                MoveRefactoredFiles();
                ReopenInitialFiles();
                FireOnRefactorComplete();
            }
        }

        private void MoveRefactoredFiles()
        {
            MessageBar.Locked = true;
            AssociatedDocumentHelper.CloseTemporarilyOpenedDocuments();
            foreach (var target in targets)
            {
                File.Delete(target.OldFilePath);

                if (target.OwnerPath == null)
                    OldPathToNewPath.Remove(target.OldFilePath);

                // Casing changes, we cannot move directly here, there may be conflicts, better leave it to the next step
                if (target.TmpFilePath != null)
                    RefactoringHelper.Move(target.TmpFilePath, target.NewFilePath);
            }
            // Move non-source files and whole folders
            foreach (KeyValuePair<string, string> item in OldPathToNewPath)
            {
                string oldPath = item.Key;
                string newPath = item.Value;
                if (File.Exists(oldPath))
                {
                    newPath = Path.Combine(newPath, Path.GetFileName(oldPath));
                    if (!Path.IsPathRooted(newPath)) newPath = Path.Combine(Path.GetDirectoryName(oldPath), newPath);
                    RefactoringHelper.Move(oldPath, newPath, true);
                }
                else if (Directory.Exists(oldPath))
                {
                    newPath = renaming ? Path.Combine(Path.GetDirectoryName(oldPath), newPath) : Path.Combine(newPath, Path.GetFileName(oldPath));

                    // Look for document class changes
                    // Do not use RefactoringHelper to avoid possible dialogs that we don't want
                    Project project = (Project)PluginBase.CurrentProject;
                    string newDocumentClass = null;
                    string searchPattern = project.DefaultSearchFilter;
                    foreach (string pattern in searchPattern.Split(';'))
                    {
                        foreach (string file in Directory.GetFiles(oldPath, pattern, SearchOption.AllDirectories))
                        {
                            if (project.IsDocumentClass(file))
                            {
                                newDocumentClass = file.Replace(oldPath, newPath);
                                break;
                            }
                        }
                        if (newDocumentClass != null) break;
                    }

                    // Check if this is a name casing change
                    if (oldPath.Equals(newPath, StringComparison.OrdinalIgnoreCase))
                    {
                        string tmpPath = oldPath + "$renaming$";
                        FileHelper.ForceMoveDirectory(oldPath, tmpPath);
                        DocumentManager.MoveDocuments(oldPath, tmpPath);
                        oldPath = tmpPath;
                    }

                    // Move directory contents to final location
                    FileHelper.ForceMoveDirectory(oldPath, newPath);
                    DocumentManager.MoveDocuments(oldPath, newPath);

                    if (!string.IsNullOrEmpty(newDocumentClass))
                    {
                        project.SetDocumentClass(newDocumentClass, true);
                        project.Save();
                    }
                }
            }

            MessageBar.Locked = false;
        }

        private void ReopenInitialFiles()
        {
            foreach (string file in filesToReopen)
                PluginBase.MainForm.OpenEditableDocument(file, false);
        }

        #endregion

        #region Event Handlers

        private void FindFinished(FRResults results)
        {
            UserInterfaceManager.ProgressDialog.Show();
            UserInterfaceManager.ProgressDialog.SetTitle(TextHelper.GetString("Info.UpdatingReferences"));
            MessageBar.Locked = true;
            var currentTarget = targets[currentTargetIndex];
            string targetName = Path.GetFileNameWithoutExtension(currentTarget.OldFilePath);
            string oldType = (currentTarget.OldFileModel.Package + "." + targetName).Trim('.');
            string newType = (currentTarget.NewPackage + "." + targetName).Trim('.');

            foreach (KeyValuePair<string, List<SearchMatch>> entry in results)
            {
                List<SearchMatch> matches = entry.Value;
                if (matches.Count == 0 || entry.Key == currentTarget.OldFilePath || 
                    entry.Key == currentTarget.NewFilePath) continue;
                string file = entry.Key;
                UserInterfaceManager.ProgressDialog.UpdateStatusMessage(TextHelper.GetString("Info.Updating") + " \"" + file + "\"");
                ITabbedDocument doc;
                ScintillaControl sci;
                var actualMatches = new List<SearchMatch>();
                foreach (SearchMatch match in entry.Value)
                {
                    // we have to open/reopen the entry's file
                    // there are issues with evaluating the declaration targets with non-open, non-current files
                    // we have to do it each time as the process of checking the declaration source can change the currently open file!
                    sci = AssociatedDocumentHelper.LoadDocument(file).SciControl;
                    // if the search result does point to the member source, store it
                    if (RefactoringHelper.DoesMatchPointToTarget(sci, match, currentTargetResult, this.AssociatedDocumentHelper))
                        actualMatches.Add(match);
                }
                if (actualMatches.Count == 0) continue;
                int currLine = -1;
                doc = AssociatedDocumentHelper.LoadDocument(file);
                sci = doc.SciControl;
                string directory = Path.GetDirectoryName(file);
                // Let's check if we need to add the import. Check the considerations at the start of the file
                // directory != currentTarget.OwnerPath -> renamed owner directory, so both files in the same place
                bool needsImport = directory != Path.GetDirectoryName(currentTarget.NewFilePath) &&
                                   directory != currentTarget.OwnerPath &&
                                   ASContext.Context.CurrentModel.Imports.Search(targetName,
                                                                                 FlagType.Class & FlagType.Function &
                                                                                 FlagType.Namespace, 0) == null;

                // Replace matches
                int typeDiff = sci.MBSafeTextLength(oldType) - sci.MBSafeTextLength(targetName);
                int newTypeDiff = sci.MBSafeTextLength(newType) - sci.MBSafeTextLength(oldType);
                int accumulatedDiff = 0;
                int j = 0;
                for (int i = 0, count = actualMatches.Count; i < count; i++)
                {
                    var sm = actualMatches[j];
                    if (currLine == -1) currLine = sm.Line - 1;
                    if (sm.LineText.Contains(oldType))
                    {
                        sm.Index -= typeDiff - accumulatedDiff;
                        sm.Value = oldType;
                        RefactoringHelper.SelectMatch(sci, sm);
                        sm.Column -= typeDiff;
                        sci.ReplaceSel(newType);
                        sm.LineEnd = sci.SelectionEnd;
                        sm.LineText = sm.LineText.Replace(oldType, newType);
                        sm.Length = oldType.Length;
                        sm.Value = newType;
                        if (needsImport) sm.Line++;
                        accumulatedDiff += newTypeDiff;
                        j++;
                    }
                    else
                    {
                        actualMatches.RemoveAt(j);
                    }
                }
                if (needsImport)
                {
                    sci.GotoLine(currLine);
                    ASGenerator.InsertImport(new MemberModel(targetName, newType, FlagType.Import, 0), false);
                    int newLine = sci.LineFromPosition(sci.Text.IndexOf(newType));
                    var sm = new SearchMatch();
                    sm.Line = newLine + 1;
                    sm.LineText = sci.GetLine(newLine);
                    sm.Column = 0;
                    sm.Length = sci.MBSafeTextLength(sm.LineText);
                    sm.Value = sm.LineText;

                    actualMatches.Insert(0, sm);
                }
                if (actualMatches.Count == 0) continue;
                if (!Results.ContainsKey(file)) Results[file] = new List<SearchMatch>();
                Results[file].AddRange(actualMatches);
                //Do we want to open modified files?
                //if (sci.IsModify) AssociatedDocumentHelper.MarkDocumentToKeep(file);
                doc.Save();
            }

            currentTargetIndex++;

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
        public string TmpFilePath;
        public string OwnerPath;
    }

    #endregion
}