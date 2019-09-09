using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using CodeRefactor.Provider;
using PluginCore.FRService;
using PluginCore.Helpers;
using PluginCore.Localization;
using PluginCore.Managers;

namespace CodeRefactor.Commands
{
    class RenameFile : RefactorCommand<IDictionary<string, List<SearchMatch>>>
    {
        private string oldPath;
        private string newPath;

        public RenameFile(string oldPath, string newPath) : this(oldPath, newPath, true)
        {
        }

        public RenameFile(string oldPath, string newPath, bool outputResults)
        {
            this.oldPath = oldPath;
            this.newPath = newPath;
            OutputResults = outputResults;
        }

        #region RefactorCommand Implementation

        protected override void ExecutionImplementation()
        {
            string oldFileName = Path.GetFileNameWithoutExtension(oldPath);
            string newFileName = Path.GetFileNameWithoutExtension(newPath);
            string msg = TextHelper.GetString("Info.RenamingFile");
            string title = string.Format(TextHelper.GetString("Title.RenameDialog"), oldFileName);
            if (MessageBox.Show(msg, title, MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                var target = RefactoringHelper.GetRefactorTargetFromFile(oldPath, AssociatedDocumentHelper);
                if (target != null)
                {
                    var command = CommandFactoryProvider.GetFactory(target).CreateRenameCommandAndExecute(target, true, newFileName);
                    command.RegisterDocumentHelper(AssociatedDocumentHelper);
                    return;
                }
            }

            string originalOld = oldPath;
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
                DocumentManager.MoveDocuments(originalOld, newPath);
                RefactoringHelper.RaiseMoveEvent(originalOld, newPath);
            }
        }

        public override bool IsValid() => true;

        #endregion

    }
}