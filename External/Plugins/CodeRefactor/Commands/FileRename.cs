using ASCompletion.Completion;
using ASCompletion.Context;
using ASCompletion.Model;
using CodeRefactor.Commands;
using CodeRefactor.Provider;
using PluginCore;
using PluginCore.FRService;
using PluginCore.Managers;
using ScintillaNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CodeRefactor.Commands
{

    class FileRename : RefactorCommand<IDictionary<String, List<SearchMatch>>>
    {
        private readonly String _path;
        private readonly Boolean _updateReferences;
        private readonly Boolean _outputResults;

        public string NewName { get; private set; }

        public FileRename(String path, String newName) : this(path, newName, false)
        {
        }

        public FileRename(String path, String newName, Boolean updateReferences) : this(path, newName, updateReferences, true)
        {
        }

        public FileRename(String path, String newName, Boolean updateReferences, Boolean outputResults)
        {
            _path = path;
            NewName = newName;
            _updateReferences = updateReferences;
            _outputResults = outputResults;
        }

        #region RefactorCommand Implementation

        protected override void ExecutionImplementation()
        {
            String oldName = Path.GetFileNameWithoutExtension(_path);
            if (!IsValid() || NewName.Equals(oldName))
                return;

            if (_updateReferences)
            {
                PluginBase.MainForm.OpenEditableDocument(_path, false);
                ScintillaControl sci = PluginBase.MainForm.CurrentDocument.SciControl;
                if (!ASContext.Context.IsFileValid || sci == null)
                    return;

                Rename command = null;

                int position = 0;
                while (sci.SelectText(oldName, position) != -1)
                {
                    ASResult target = RefactoringHelper.GetDefaultRefactorTarget();
                    if (RefactoringHelper.CanRenameWithFile(target))
                    {
                        command = new Rename(target, true, NewName);
                        break;
                    }

                    position = sci.SelectionEnd;
                }

                if(command != null)
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

                String newFileName = Path.Combine(fullPath, NewName + Path.GetExtension(_path));

                if (_path == null || _path.Equals(newFileName))
                    return;

                Boolean reopen = false;
                foreach (ITabbedDocument doc in PluginBase.MainForm.Documents)
                    if (doc.FileName.Equals(_path))
                    {
                        doc.Save();
                        doc.Close();
                        reopen = true;
                    }

                File.Move(_path, newFileName);
                if (reopen)
                    PluginBase.MainForm.OpenEditableDocument(newFileName, false);
            }
        }

        public override Boolean IsValid()
        {
            return NewName != null && NewName.Trim() != String.Empty;
        }

        #endregion


    }
}
