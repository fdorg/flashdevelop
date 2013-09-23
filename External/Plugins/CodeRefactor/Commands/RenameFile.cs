using ASCompletion.Completion;
using CodeRefactor.Controls;
using CodeRefactor.Provider;
using PluginCore;
using PluginCore.FRService;
using PluginCore.Managers;
using ScintillaNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace CodeRefactor.Commands
{
    class RenameFile : RefactorCommand<IDictionary<String, List<SearchMatch>>>
    {
        private readonly String _path;
        private readonly Boolean _outputResults;

        public RenameFile(String path) : this(path, true)
        {
        }

        public RenameFile(String path, Boolean outputResults)
        {
            _path = path;
            _outputResults = outputResults;
        }

        #region RefactorCommand Implementation

        protected override void ExecutionImplementation()
        {
            String oldName = Path.GetFileNameWithoutExtension(_path);

            RenameFileDialog dialog = UserInterfaceManager.RenameFileDialog;
            dialog.ShowDialogFor(_path);
            
            if (dialog.DialogResult == DialogResult.OK)
            {
                String newName = dialog.NewName.Text;

                if (dialog.UpdateReferences.Checked)
                {
                    PluginBase.MainForm.OpenEditableDocument(_path, false);
                    ScintillaControl sci = PluginBase.MainForm.CurrentDocument.SciControl;
                    
                    Rename command = null;
                    int position = 0;
                    while (sci.SelectText(oldName, position) != -1)
                    {
                        ASResult target = RefactoringHelper.GetDefaultRefactorTarget();
                        if (RefactoringHelper.GetRenameWithFile(target))
                        {
                            command = new Rename(target, true, newName);
                            break;
                        }

                        position = sci.SelectionEnd;
                    }

                    if (command == null)
                        return;

                    try
                    {
                        command.Execute();
                    }
                    catch (Exception ex)
                    {
                        ErrorManager.ShowError(ex);
                    }
                }
                else
                {
                    String fullPath = Path.GetFullPath(_path);
                    fullPath = Path.GetDirectoryName(fullPath);

                    String newFileName = Path.Combine(fullPath, newName + Path.GetExtension(_path));

                    RefactoringHelper.MoveFile(_path, newFileName);
                }
            }
        }

        public override Boolean IsValid()
        {
            return true;
        }

        #endregion

    }
}