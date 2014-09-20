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
    class Move : RefactorCommand<IDictionary<string, List<SearchMatch>>>
    {
        public Dictionary<string, string> OldPathToNewPath;
        public bool OutputResults;
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

        public Move(Dictionary<string, string> oldPathToNewPath, bool outputResults) : this(oldPathToNewPath, outputResults, false)
        {
        }

        public Move(Dictionary<string, string> oldPathToNewPath, bool outputResults, bool renaming)
        {
            OldPathToNewPath = oldPathToNewPath;
            OutputResults = outputResults;
            this.renaming = renaming;
            Results = new Dictionary<string, List<SearchMatch>>();
            CreateListOfMoveTargets();
        }

        #endregion

        #region RefactorCommand Implementation

        protected override void ExecutionImplementation()
        {
            string msg;
            string title = "";
            if (renaming)
            {
                msg = TextHelper.GetString("Info.RenamingDirectory");
                foreach (KeyValuePair<string, string> item in OldPathToNewPath)
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

        public override bool IsValid()
        {
            return OldPathToNewPath != null;
        }

        #endregion

        #region Private Helper Methods

        private void CreateListOfMoveTargets()
        {
            targets = new List<MoveTargetHelper>();
            foreach (KeyValuePair<string, string> item in OldPathToNewPath)
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

        private bool IsValidFile(string file)
        {
            if (PluginBase.CurrentProject == null) return false;
            string ext = Path.GetExtension(file);
            return ext == ".as" || ext == ".hx" || ext == ".ls" && PluginBase.CurrentProject.DefaultSearchFilter.Contains(ext);
        }

        private MoveTargetHelper GetMoveTarget(string oldFilePath, string newPath)
        {
            MoveTargetHelper result = new MoveTargetHelper();
            result.OldFilePath = oldFilePath;
            result.OldFileModel = ASContext.Context.GetFileModel(oldFilePath);
            result.NewFilePath = newPath;
            IProject project = PluginBase.CurrentProject;
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

        private void MoveTargets()
        {
            Dictionary<string, ITabbedDocument> fileNameToOpenedDoc = new Dictionary<string, ITabbedDocument>();
            foreach (ITabbedDocument doc in PluginBase.MainForm.Documents)
            {
                fileNameToOpenedDoc.Add(doc.FileName, doc);
            }
            MessageBar.Locked = true;
            foreach (KeyValuePair<string, string> item in OldPathToNewPath)
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
                    search = new FRSearch("package");
                    search.WholeWord = true;
                    newType = Path.GetFileNameWithoutExtension(currentTarget.OldFilePath);
                }
                else
                {
                    search = new FRSearch("package\\s+(" + oldFileModel.Package + ")");
                    newType = oldFileModel.Package + "." + Path.GetFileNameWithoutExtension(currentTarget.OldFilePath);
                }
                search.IsRegex = true;
                search.Filter = SearchFilter.None;
                newType = newType.Trim('.');
                MessageBar.Locked = true;
                string newFilePath = currentTarget.NewFilePath;
                ScintillaControl sci = AssociatedDocumentHelper.LoadDocument(newFilePath);
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
            else FireOnRefactorComplete();
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
                if (packageIsNotEmpty) RefactoringHelper.ReplaceMatches(matches, sci, newType);
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