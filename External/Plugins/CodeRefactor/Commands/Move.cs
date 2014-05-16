﻿using System;
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
                msg = TextHelper.GetString("Info.RenamingDirectory");//TODO: LOCALIZE ME
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
            ProjectManager.Projects.PathCollection paths = project.AbsoluteClasspaths;
            if (paths.Contains(newPackage)) newPackage = "";
            if (!string.IsNullOrEmpty(newPackage))
            {
                paths = new ProjectManager.Projects.PathCollection(paths);
                paths.AddRange(ProjectManager.PluginMain.Settings.GlobalClasspaths);
                foreach (string path in paths)
                {
                    if (newPackage.StartsWith(path))
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
                string oldPackage;
                string newPackage;
                string newType;
                if (string.IsNullOrEmpty(oldFileModel.Package))
                {
                    oldPackage = "package";
                    newPackage = "package " + currentTarget.NewPackage;
                    newType = Path.GetFileNameWithoutExtension(currentTarget.OldFilePath);
                }
                else
                {
                    oldPackage = oldFileModel.Package;
                    newPackage = currentTarget.NewPackage;
                    newType = oldFileModel.Package + "." + Path.GetFileNameWithoutExtension(currentTarget.OldFilePath);
                }
                newType = newType.Trim('.');
                MessageBar.Locked = true;
                ScintillaControl sci = AssociatedDocumentHelper.LoadDocument(currentTarget.NewFilePath);
                FRSearch search = new FRSearch(oldPackage);
                search.WholeWord = true;
                search.SingleLine = true;
                search.NoCase = false;
                search.IsRegex = false;
                search.Filter = SearchFilter.None;
                List<SearchMatch> matches = search.Matches(sci.Text);
                RefactoringHelper.ReplaceMatches(matches, sci, newPackage, null);
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
                if (entry.Value.Count == 0) continue;
                UserInterfaceManager.ProgressDialog.UpdateStatusMessage(TextHelper.GetString("Info.Updating") + " \"" + entry.Key + "\"");
                ScintillaControl sci = AssociatedDocumentHelper.LoadDocument(entry.Key);
                if (isNotHaxe && entry.Key != currentTarget.NewFilePath && ASContext.Context.CurrentModel.Imports.Search(targetName, FlagType.Class & FlagType.Function & FlagType.Namespace, 0) == null)
                {
                    ASGenerator.InsertImport(new MemberModel(targetName, newType, FlagType.Import, 0), false);
                }
                if (packageIsNotEmpty) RefactoringHelper.ReplaceMatches(entry.Value, sci, newType, null);
                else
                {
                    foreach (SearchMatch sm in entry.Value)
                    {
                        if (sm.LineText.TrimStart().StartsWith("import"))
                        {
                            RefactoringHelper.SelectMatch(sci, sm);
                            sci.ReplaceSel(newType);
                        }
                    }
                }
                PluginBase.MainForm.CurrentDocument.Save();
                if (sci.IsModify) AssociatedDocumentHelper.MarkDocumentToKeep(entry.Key);
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