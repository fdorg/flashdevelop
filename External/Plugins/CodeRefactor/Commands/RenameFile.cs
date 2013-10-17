﻿using ASCompletion.Context;
using ASCompletion.Model;
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
        private readonly string path;
        private readonly bool outputResults;

        public RenameFile(String path) : this(path, true)
        {
        }

        public RenameFile(String path, Boolean outputResults)
        {
            this.path = path;
            this.outputResults = outputResults;
        }

        #region RefactorCommand Implementation

        protected override void ExecutionImplementation()
        {
            string oldName = Path.GetFileNameWithoutExtension(path);

            RenameFileDialog dialog = UserInterfaceManager.RenameFileDialog;
            dialog.ShowDialogFor(path);

            if (dialog.DialogResult == DialogResult.OK)
            {
                string newName = dialog.NewName.Text;

                if (dialog.UpdateReferences.Checked)
                {
                    PluginBase.MainForm.OpenEditableDocument(path, false);
                    ScintillaControl sci = PluginBase.MainForm.CurrentDocument.SciControl;
                    if (sci == null)
                        return;

                    string line = null;

                    List<ClassModel> classes = ASContext.Context.CurrentModel.Classes;
                    if (classes.Count > 0)
                    {
                        foreach (ClassModel classModel in classes)
                            if (classModel.Name.Equals(oldName))
                                line = sci.GetLine(classModel.LineFrom);
                    }
                    else
                    {
                        MemberList members = ASContext.Context.CurrentModel.Members;
                        foreach (MemberModel member in members)
                            if (member.Name.Equals(oldName))
                                line = sci.GetLine(member.LineFrom);
                    }

                    if (string.IsNullOrEmpty(line))
                        return;

                    sci.SelectText(oldName, line.IndexOf(oldName));

                    Rename command = new Rename(RefactoringHelper.GetDefaultRefactorTarget(), true, newName);
                    command.Execute();
                }
                else
                {
                    string fullPath = Path.GetFullPath(path);
                    fullPath = Path.GetDirectoryName(fullPath);
                    
                    string newFileName = Path.Combine(fullPath, newName + Path.GetExtension(path));
                    bool reOpen = false;

                    foreach (ITabbedDocument doc in PluginBase.MainForm.Documents)
                        if (doc.FileName.Equals(path))
                        {
                            doc.Save();
                            doc.Close();

                            reOpen = true;
                            break;
                        }

                    File.Move(path, newFileName);
                    if(reOpen)
                        PluginBase.MainForm.OpenEditableDocument(newFileName, false);
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