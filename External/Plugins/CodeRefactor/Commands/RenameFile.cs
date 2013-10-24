using System;﻿
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using ASCompletion.Context;
using ASCompletion.Model;
using CodeRefactor.Controls;
using CodeRefactor.Provider;
using PluginCore.FRService;
using PluginCore.Managers;
using ScintillaNet;
using PluginCore;
using PluginCore.Localization;

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
            String oldName = Path.GetFileNameWithoutExtension(oldPath);
            String newName = Path.GetFileNameWithoutExtension(newPath);
            String msg = TextHelper.GetString("Info.RenamingFile");
            String title = String.Format(TextHelper.GetString("Title.RenameDialog"), oldName);
            if (MessageBox.Show(msg, title, MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Int32 line = 0;
                PluginBase.MainForm.OpenEditableDocument(oldPath, false);
                ScintillaControl sci = PluginBase.MainForm.CurrentDocument.SciControl;
                if (sci == null) return; // Should not happen...
                List<ClassModel> classes = ASContext.Context.CurrentModel.Classes;
                if (classes.Count > 0)
                {
                    foreach (ClassModel classModel in classes)
                    {
                        if (classModel.Name.Equals(oldName)) line = classModel.LineFrom;
                    }
                }
                else
                {
                    MemberList members = ASContext.Context.CurrentModel.Members;
                    foreach (MemberModel member in members)
                    {
                        if (member.Name.Equals(oldName)) line = member.LineFrom;
                    }
                }
                sci.SelectText(oldName, sci.PositionFromLine(line));
                Rename command = new Rename(RefactoringHelper.GetDefaultRefactorTarget(), true, newName);
                command.Execute();
            }
            else
            {
                oldName = Path.GetFileName(oldPath);
                if (oldName.Equals(newPath, StringComparison.OrdinalIgnoreCase))
                {
                    // name casing changed
                    string tmpPath = newPath + "$renaming$";
                    File.Move(oldPath, tmpPath);
                    oldPath = tmpPath;
                }
                File.Move(oldPath, newPath);
            }
        }

        public override Boolean IsValid()
        {
            return true;
        }

        #endregion

    }

}
