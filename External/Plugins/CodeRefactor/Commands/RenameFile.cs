using ASCompletion.Context;
using ASCompletion.Model;
using CodeRefactor.Provider;
using PluginCore.FRService;
using PluginCore.Helpers;
using PluginCore.Localization;
using ScintillaNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace CodeRefactor.Commands
{
    class RenameFile : RefactorCommand<IDictionary<String, List<SearchMatch>>>
    {
        private string oldPath;
        private string newPath;
        private readonly bool outputResults;

        public RenameFile(String oldPath, String newPath) : this(oldPath, newPath, true)
        {
            this.oldPath = oldPath;
            this.newPath = newPath;
        }

        public RenameFile(String oldPath, String newPath, Boolean outputResults)
        {
            this.oldPath = oldPath;
            this.newPath = newPath;
            this.outputResults = outputResults;
        }

        #region RefactorCommand Implementation

        protected override void ExecutionImplementation()
        {
            String oldFileName = Path.GetFileNameWithoutExtension(oldPath);
            String newFileName = Path.GetFileNameWithoutExtension(newPath);
            String msg = TextHelper.GetString("Info.RenamingFile");
            String title = String.Format(TextHelper.GetString("Title.RenameDialog"), oldFileName);
            if (MessageBox.Show(msg, title, MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Int32 line = 0;
                ScintillaControl sci = AssociatedDocumentHelper.LoadDocument(oldPath);
                if (sci == null) return; // Should not happen...
                List<ClassModel> classes = ASContext.Context.CurrentModel.Classes;
                if (classes.Count > 0)
                {
                    foreach (ClassModel classModel in classes)
                    {
                        if (classModel.Name.Equals(oldFileName))
                        {
                            line = classModel.LineFrom;
                            break;
                        }
                    }
                }
                else
                {
                    foreach (MemberModel member in ASContext.Context.CurrentModel.Members)
                    {
                        if (member.Name.Equals(oldFileName))
                        {
                            line = member.LineFrom;
                            break;
                        }
                    }
                }
                if (line > 0)
                {
                    sci.SelectText(oldFileName, sci.PositionFromLine(line));
                    Rename command = new Rename(RefactoringHelper.GetDefaultRefactorTarget(), true, newFileName);
                    command.RegisterDocumentHelper(AssociatedDocumentHelper);
                    command.Execute();
                    return;
                }
            }
            // refactor failed or was refused
            if (Path.GetFileName(oldPath).Equals(newPath, StringComparison.OrdinalIgnoreCase))
            {
                // name casing changed
                string tmpPath = oldPath + "$renaming$";
                File.Move(oldPath, tmpPath);
                oldPath = tmpPath;
            }
            if (!Path.IsPathRooted(newPath)) newPath = Path.Combine(Path.GetDirectoryName(oldPath), newPath);

            if (FileHelper.ConfirmOverwrite(newPath))
            {
                FileHelper.ForceMove(oldPath, newPath);
                PluginCore.Managers.DocumentManager.MoveDocuments(oldPath, newPath);
            }
        }

        public override Boolean IsValid()
        {
            return true;
        }

        #endregion

    }
}