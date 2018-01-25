using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using ASCompletion.Completion;
using ASCompletion.Context;
using ASCompletion.Model;
using CodeRefactor.Commands;
using CodeRefactor.Controls;
using CodeRefactor.Provider;
using PluginCore;
using PluginCore.Helpers;
using PluginCore.Localization;
using PluginCore.Managers;
using PluginCore.Utilities;
using ProjectManager;
using ProjectManager.Actions;
using ProjectManager.Controls.TreeView;
using ProjectManager.Helpers;
using CodeRefactor.Managers;

namespace CodeRefactor
{
    public class PluginMain : IPlugin
    {
        public const string REG_IDENTIFIER = "^[a-zA-Z_$][a-zA-Z0-9_$]*$";
        private String pluginName = "CodeRefactor";
        private String pluginGuid = "5c0d3740-a6f2-11de-8a39-0800200c9a66";
        private String pluginHelp = "www.flashdevelop.org/community/";
        private String pluginDesc = "Adds refactoring capabilities to FlashDevelop.";
        private String pluginAuth = "FlashDevelop Team";
        private ToolStripMenuItem editorReferencesItem;
        private ToolStripMenuItem viewReferencesItem;
        private SurroundMenu surroundContextMenu;
        private RefactorMenu refactorContextMenu;
        private RefactorMenu refactorMainMenu;
        private Settings settingObject;
        private String settingFilename;
        TreeView projectTreeView;

        public const string TraceGroup = "CodeRefactor";
        
        #region Required Properties

        /// <summary>
        /// Api level of the plugin
        /// </summary>
        public Int32 Api
        {
            get { return 1; }
        }

        /// <summary>
        /// Name of the plugin
        /// </summary> 
        public String Name
        {
            get { return this.pluginName; }
        }

        /// <summary>
        /// GUID of the plugin
        /// </summary>
        public String Guid
        {
            get { return this.pluginGuid; }
        }

        /// <summary>
        /// Author of the plugin
        /// </summary> 
        public String Author
        {
            get { return this.pluginAuth; }
        }

        /// <summary>
        /// Description of the plugin
        /// </summary> 
        public String Description
        {
            get { return this.pluginDesc; }
        }

        /// <summary>
        /// Web address for help
        /// </summary> 
        public String Help
        {
            get { return this.pluginHelp; }
        }

        /// <summary>
        /// Object that contains the settings
        /// </summary>
        [Browsable(false)]
        public Object Settings
        {
            get { return this.settingObject; }
        }

        #endregion

        #region Required Methods

        /// <summary>
        /// Initializes the plugin
        /// </summary>
        public void Initialize()
        {
            this.InitBasics();
            this.LoadSettings();
            this.CreateMenuItems();
            this.RegisterMenuItems();
            this.RegisterTraceGroups();
        }

        /// <summary>
        /// Disposes the plugin
        /// </summary>
        public void Dispose()
        {
            this.SaveSettings();
        }

        /// <summary>
        /// Handles the incoming events
        /// </summary>
        public void HandleEvent(Object sender, NotifyEvent e, HandlingPriority priority)
        {
            switch (e.Type)
            {
                case EventType.FileSwitch:
                    this.GenerateSurroundMenuItems();
                    UpdateMenuItems();
                    break;

                case EventType.UIStarted:
                    // Expose plugin's refactor main menu & context menu...
                    EventManager.DispatchEvent(this, new DataEvent(EventType.Command, "CodeRefactor.Menu", this.refactorMainMenu));
                    EventManager.DispatchEvent(this, new DataEvent(EventType.Command, "CodeRefactor.ContextMenu", this.refactorContextMenu));
                    // Watch resolved context for menu item updating...
                    ASComplete.OnResolvedContextChanged += OnResolvedContextChanged;
                    DirectoryNode.OnDirectoryNodeRefresh += OnDirectoryNodeRefresh;
                    this.UpdateMenuItems();
                    break;

                case EventType.Command:
                    DataEvent de = (DataEvent)e;
                    string[] args;
                    string oldPath;
                    string newPath;
                    switch (de.Action)
                    {
                        case ProjectFileActionsEvents.FileRename:
                            if (settingObject.DisableMoveRefactoring) break;
                            args = de.Data as string[];
                            oldPath = args[0];
                            newPath = args[1];
                            if (Directory.Exists(oldPath) && IsValidForMove(oldPath, newPath))
                            {
                                MovingHelper.AddToQueue(new Dictionary<string, string> { { oldPath, newPath } }, true, true);
                                e.Handled = true;
                            }
                            else if (IsValidForRename(oldPath, newPath))
                            {
                                MoveFile(oldPath, newPath);
                                e.Handled = true;
                            }
                            break;

                        case ProjectFileActionsEvents.FileMove:
                            if (settingObject.DisableMoveRefactoring) break;
                            args = de.Data as string[];
                            oldPath = args[0];
                            newPath = args[1];
                            if (IsValidForMove(oldPath, newPath))
                            {
                                MovingHelper.AddToQueue(new Dictionary<string, string> { { oldPath, newPath } }, true);
                                e.Handled = true;
                            }
                            break;

                        case "ASCompletion.ContextualGenerator.AddOptions":
                            OnAddRefactorOptions(de.Data as List<ICompletionListItem>);
                            break;

                        case ProjectManagerEvents.TreeSelectionChanged:
                            OnTreeSelectionChanged();
                            break;
                    }
                    break;
            }
        }

        /// <summary>
        /// Checks if the file is valid for rename file command
        /// </summary>
        private static bool IsValidForRename(string oldPath, string newPath)
        {
            string oldExt = Path.GetExtension(oldPath);
            string newExt = Path.GetExtension(newPath);
            return PluginBase.CurrentProject != null
                && File.Exists(oldPath)
                && oldExt == newExt
                && IsValidFile(oldPath)
                && Regex.Match(Path.GetFileNameWithoutExtension(newPath), REG_IDENTIFIER, RegexOptions.Singleline).Success;
        }

        /// <summary>
        /// Checks if the file or directory is valid for move command
        /// </summary>
        static bool IsValidForMove(string oldPath)
        {
            return PluginBase.CurrentProject != null
                   && (File.Exists(oldPath) || Directory.Exists(oldPath))
                   && IsValidFile(oldPath);
        }

        /// <summary>
        /// Checks if the file or directory is valid for move command
        /// </summary>
        static bool IsValidForMove(string oldPath, string newPath)
        {
            newPath = Path.GetFileNameWithoutExtension(newPath);
            return IsValidForMove(oldPath) && Regex.Match(newPath, REG_IDENTIFIER, RegexOptions.Singleline).Success;
        }

        /// <summary>
        /// Checks if the file or directory is ok for refactoring
        /// </summary>
        private static bool IsValidFile(string file)
        {
            IProject project = PluginBase.CurrentProject;
            return project != null 
                && RefactoringHelper.IsProjectRelatedFile(project, file)
                && Regex.Match(Path.GetFileNameWithoutExtension(file), REG_IDENTIFIER, RegexOptions.Singleline).Success
                && (Directory.Exists(file) || FileHelper.FileMatchesSearchFilter(file, project.DefaultSearchFilter));
        }

        #endregion

        #region Event Handling

        /// <summary>
        /// Initializes important variables
        /// </summary>
        public void InitBasics()
        {
            EventManager.AddEventHandler(this, EventType.UIStarted | EventType.FileSwitch | EventType.Command);
            String dataPath = Path.Combine(PathHelper.DataDir, "CodeRefactor");
            if (!Directory.Exists(dataPath)) Directory.CreateDirectory(dataPath);
            this.settingFilename = Path.Combine(dataPath, "Settings.fdb");
            this.pluginDesc = TextHelper.GetString("Info.Description");

            BatchProcessManager.AddBatchProcessor(new BatchProcessors.FormatCodeProcessor());
            BatchProcessManager.AddBatchProcessor(new BatchProcessors.OrganizeImportsProcessor());
            BatchProcessManager.AddBatchProcessor(new BatchProcessors.TruncateImportsProcessor());
            BatchProcessManager.AddBatchProcessor(new BatchProcessors.ConsistentEOLProcessor());
        }

        /// <summary>
        /// Creates the required menu items
        /// </summary>
        private void CreateMenuItems()
        {
            this.refactorMainMenu = new RefactorMenu(true);
            this.refactorMainMenu.RenameMenuItem.Click += this.RenameClicked;
            this.refactorMainMenu.MoveMenuItem.Click += MoveClicked;
            this.refactorMainMenu.OrganizeMenuItem.Click += this.OrganizeImportsClicked;
            this.refactorMainMenu.TruncateMenuItem.Click += this.TruncateImportsClicked;
            this.refactorMainMenu.ExtractMethodMenuItem.Click += this.ExtractMethodClicked;
            this.refactorMainMenu.DelegateMenuItem.Click += this.DelegateMethodsClicked;
            this.refactorMainMenu.ExtractLocalVariableMenuItem.Click += this.ExtractLocalVariableClicked;
            this.refactorMainMenu.CodeGeneratorMenuItem.Click += this.CodeGeneratorMenuItemClicked;
            this.refactorMainMenu.BatchMenuItem.Click += this.BatchMenuItemClicked;
            this.refactorContextMenu = new RefactorMenu(false);
            this.refactorContextMenu.RenameMenuItem.Click += this.RenameClicked;
            this.refactorContextMenu.MoveMenuItem.Click += MoveClicked;
            this.refactorContextMenu.OrganizeMenuItem.Click += this.OrganizeImportsClicked;
            this.refactorContextMenu.TruncateMenuItem.Click += this.TruncateImportsClicked;
            this.refactorContextMenu.DelegateMenuItem.Click += this.DelegateMethodsClicked;
            this.refactorContextMenu.ExtractMethodMenuItem.Click += this.ExtractMethodClicked;
            this.refactorContextMenu.ExtractLocalVariableMenuItem.Click += this.ExtractLocalVariableClicked;
            this.refactorContextMenu.CodeGeneratorMenuItem.Click += this.CodeGeneratorMenuItemClicked;
            this.refactorContextMenu.BatchMenuItem.Click += this.BatchMenuItemClicked;
            ContextMenuStrip editorMenu = PluginBase.MainForm.EditorMenu;
            this.surroundContextMenu = new SurroundMenu();
            editorMenu.Items.Insert(3, this.refactorContextMenu);
            editorMenu.Items.Insert(4, this.surroundContextMenu);
            PluginBase.MainForm.MenuStrip.Items.Insert(5, this.refactorMainMenu);
            ToolStripMenuItem searchMenu = PluginBase.MainForm.FindMenuItem("SearchMenu") as ToolStripMenuItem;
            this.viewReferencesItem = new ToolStripMenuItem(TextHelper.GetString("Label.FindAllReferences"), null, this.FindAllReferencesClicked);
            this.editorReferencesItem = new ToolStripMenuItem(TextHelper.GetString("Label.FindAllReferences"), null, this.FindAllReferencesClicked);
            PluginBase.MainForm.RegisterShortcutItem("RefactorMenu.SurroundWith", this.refactorMainMenu.SurroundMenu);
            PluginBase.MainForm.RegisterShortcutItem("SearchMenu.ViewReferences", this.viewReferencesItem);
            PluginBase.MainForm.RegisterSecondaryItem("RefactorMenu.SurroundWith", this.surroundContextMenu);
            PluginBase.MainForm.RegisterSecondaryItem("SearchMenu.ViewReferences", this.editorReferencesItem);
            searchMenu.DropDownItems.Add(new ToolStripSeparator());
            searchMenu.DropDownItems.Add(this.viewReferencesItem);
            editorMenu.Items.Insert(8, this.editorReferencesItem);
        }

        /// <summary>
        /// Registers the menu items with the shortcut manager
        /// </summary>
        private void RegisterMenuItems()
        {
            PluginBase.MainForm.RegisterShortcutItem("RefactorMenu.Rename", this.refactorMainMenu.RenameMenuItem);
            PluginBase.MainForm.RegisterShortcutItem("RefactorMenu.Move", this.refactorMainMenu.MoveMenuItem);
            PluginBase.MainForm.RegisterShortcutItem("RefactorMenu.ExtractMethod", this.refactorMainMenu.ExtractMethodMenuItem);
            PluginBase.MainForm.RegisterShortcutItem("RefactorMenu.ExtractLocalVariable", this.refactorMainMenu.ExtractLocalVariableMenuItem);
            PluginBase.MainForm.RegisterShortcutItem("RefactorMenu.GenerateDelegateMethods", this.refactorMainMenu.DelegateMenuItem);
            PluginBase.MainForm.RegisterShortcutItem("RefactorMenu.OrganizeImports", this.refactorMainMenu.OrganizeMenuItem);
            PluginBase.MainForm.RegisterShortcutItem("RefactorMenu.TruncateImports", this.refactorMainMenu.TruncateMenuItem);
            PluginBase.MainForm.RegisterShortcutItem("RefactorMenu.CodeGenerator", this.refactorMainMenu.CodeGeneratorMenuItem);
            PluginBase.MainForm.RegisterShortcutItem("RefactorMenu.BatchProcess", this.refactorMainMenu.BatchMenuItem);
            PluginBase.MainForm.RegisterSecondaryItem("RefactorMenu.Rename", this.refactorContextMenu.RenameMenuItem);
            PluginBase.MainForm.RegisterSecondaryItem("RefactorMenu.Move", this.refactorContextMenu.MoveMenuItem);
            PluginBase.MainForm.RegisterSecondaryItem("RefactorMenu.ExtractMethod", this.refactorContextMenu.ExtractMethodMenuItem);
            PluginBase.MainForm.RegisterSecondaryItem("RefactorMenu.ExtractLocalVariable", this.refactorContextMenu.ExtractLocalVariableMenuItem);
            PluginBase.MainForm.RegisterSecondaryItem("RefactorMenu.GenerateDelegateMethods", this.refactorContextMenu.DelegateMenuItem);
            PluginBase.MainForm.RegisterSecondaryItem("RefactorMenu.OrganizeImports", this.refactorContextMenu.OrganizeMenuItem);
            PluginBase.MainForm.RegisterSecondaryItem("RefactorMenu.TruncateImports", this.refactorContextMenu.TruncateMenuItem);
            PluginBase.MainForm.RegisterSecondaryItem("RefactorMenu.CodeGenerator", this.refactorContextMenu.CodeGeneratorMenuItem);
            PluginBase.MainForm.RegisterSecondaryItem("RefactorMenu.BatchProcess", this.refactorContextMenu.BatchMenuItem);
        }

        private void RegisterTraceGroups()
        {
            TraceManager.RegisterTraceGroup(TraceGroup, TextHelper.GetStringWithoutMnemonics("Label.Refactor"), false);
            TraceManager.RegisterTraceGroup(FindAllReferences.TraceGroup, TextHelper.GetString("Label.FindAllReferencesResult"), false, true);
        }

        /// <summary>
        /// Cursor position changed and word at this position was resolved
        /// </summary>
        private void OnResolvedContextChanged(ResolvedContext resolved)
        {
            this.UpdateMenuItems();
        }

        /// <summary>
        /// Updates the state of the menu items
        /// </summary>
        private void UpdateMenuItems()
        {
            try
            {
                var document = PluginBase.MainForm.CurrentDocument;
                var curFileName = document != null ? document.FileName : string.Empty;
                this.refactorMainMenu.DelegateMenuItem.Enabled = false;
                this.refactorContextMenu.DelegateMenuItem.Enabled = false;
                bool langIsValid = RefactoringHelper.GetLanguageIsValid();
                ResolvedContext resolved = ASComplete.CurrentResolvedContext;
                bool isValid = langIsValid && resolved != null && resolved.Position >= 0;
                ASResult result = isValid ? resolved.Result : null;
                if (result != null && !result.IsNull())
                {
                    bool isRenameable = (result.Member != null && RefactoringHelper.ModelFileExists(result.Member.InFile) && !RefactoringHelper.IsUnderSDKPath(result.Member.InFile))
                        || (result.Type != null && RefactoringHelper.ModelFileExists(result.Type.InFile) && !RefactoringHelper.IsUnderSDKPath(result.Type.InFile))
                        || (RefactoringHelper.ModelFileExists(result.InFile) && !RefactoringHelper.IsUnderSDKPath(result.InFile))
                        || result.IsPackage;
                    this.refactorContextMenu.RenameMenuItem.Enabled = isRenameable;
                    this.refactorMainMenu.RenameMenuItem.Enabled = isRenameable;
                    var enabled = !result.IsPackage && (File.Exists(curFileName) || curFileName.Contains("[model]"));
                    this.editorReferencesItem.Enabled = enabled;
                    this.viewReferencesItem.Enabled = enabled;
                    if (result.InFile != null && result.InClass != null && (result.InClass.Flags & FlagType.Interface) == 0 && result.Member != null && result.Type != null)
                    {
                        FlagType flags = result.Member.Flags;
                        if ((flags & FlagType.Variable) > 0 && (flags & FlagType.LocalVar) == 0 && (flags & FlagType.ParameterVar) == 0)
                        {
                            this.refactorContextMenu.DelegateMenuItem.Enabled = true;
                            this.refactorMainMenu.DelegateMenuItem.Enabled = true;
                        }
                    }
                }
                else
                {
                    this.refactorMainMenu.RenameMenuItem.Enabled = false;
                    this.refactorContextMenu.RenameMenuItem.Enabled = false;
                    this.editorReferencesItem.Enabled = false;
                    this.viewReferencesItem.Enabled = false;
                }
                IASContext context = ASContext.Context;
                if (context != null && context.CurrentModel != null)
                {
                    bool enabled = langIsValid && context.CurrentModel.Imports.Count > 0;
                    this.refactorContextMenu.OrganizeMenuItem.Enabled = enabled;
                    this.refactorContextMenu.TruncateMenuItem.Enabled = enabled;
                    this.refactorMainMenu.OrganizeMenuItem.Enabled = enabled;
                    this.refactorMainMenu.TruncateMenuItem.Enabled = enabled;
                }
                refactorMainMenu.MoveMenuItem.Enabled = false;
                refactorContextMenu.MoveMenuItem.Enabled = false;
                this.surroundContextMenu.Enabled = false;
                this.refactorMainMenu.SurroundMenu.Enabled = false;
                this.refactorMainMenu.ExtractMethodMenuItem.Enabled = false;
                this.refactorContextMenu.ExtractMethodMenuItem.Enabled = false;
                this.refactorMainMenu.ExtractLocalVariableMenuItem.Enabled = false;
                this.refactorContextMenu.ExtractLocalVariableMenuItem.Enabled = false;
                if (document != null && document.IsEditable && langIsValid)
                {
                    bool isValidFile = IsValidFile(curFileName);
                    refactorMainMenu.MoveMenuItem.Enabled = isValidFile;
                    refactorContextMenu.MoveMenuItem.Enabled = isValidFile;
                    var sci = document.SciControl;
                    if (sci.SelTextSize > 0)
                    {
                        if (!sci.PositionIsOnComment(sci.SelectionStart) || !sci.PositionIsOnComment(sci.SelectionEnd))
                        {
                            this.surroundContextMenu.Enabled = true;
                            this.refactorMainMenu.SurroundMenu.Enabled = true;
                            this.refactorMainMenu.ExtractMethodMenuItem.Enabled = true;
                            this.refactorContextMenu.ExtractMethodMenuItem.Enabled = true;
                        }
                        if (context != null)
                        {
                            var declAtSelStart = context.GetDeclarationAtLine(sci.LineFromPosition(sci.SelectionStart));
                            var declAtSelEnd = context.GetDeclarationAtLine(sci.LineFromPosition(sci.SelectionEnd));
                            if (declAtSelStart != null && declAtSelStart.Member != null && (declAtSelStart.Member.Flags & FlagType.Function) > 0
                                && declAtSelEnd != null && declAtSelStart.Member.Equals(declAtSelEnd.Member))
                            {
                                this.refactorMainMenu.ExtractLocalVariableMenuItem.Enabled = true;
                                this.refactorContextMenu.ExtractLocalVariableMenuItem.Enabled = true;
                            }
                        }
                    }
                }
                this.refactorMainMenu.CodeGeneratorMenuItem.Enabled = isValid;
                this.refactorContextMenu.CodeGeneratorMenuItem.Enabled = isValid;
            }
            catch {}
        }

        /// <summary>
        /// Generate surround main menu and context menu items
        /// </summary>
        private void GenerateSurroundMenuItems()
        {
            ITabbedDocument document = PluginBase.MainForm.CurrentDocument;
            if (document != null && document.IsEditable && RefactoringHelper.GetLanguageIsValid())
            {
                this.surroundContextMenu.GenerateSnippets(document.SciControl);
                this.refactorMainMenu.SurroundMenu.GenerateSnippets(document.SciControl);
            }
            else
            {
                this.surroundContextMenu.Clear();
                this.refactorMainMenu.SurroundMenu.Clear();
            }
        }

        /// <summary>
        /// Invoked when the user selects the "Rename" command
        /// </summary>
        private void RenameClicked(Object sender, EventArgs e)
        {
            if (InlineRename.InProgress) return;
            try
            {
                CommandFactoryProvider.GetFactoryForCurrentDocument().CreateRenameCommandAndExecute(true, settingObject.UseInlineRenaming);
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Invoked when the user selects the "Move" command
        /// </summary>
        static void MoveClicked(object sender, EventArgs e)
        {
            MoveFile(PluginBase.MainForm.CurrentDocument.FileName);
        }

        static void MoveFile(string fileName)
        {
            MoveDialog dialog = new MoveDialog(fileName);
            if (dialog.ShowDialog() != DialogResult.OK) return;
            Dictionary<string, string> oldPathToNewPath = new Dictionary<string, string>();
            foreach (string file in dialog.MovingFiles)
            {
                oldPathToNewPath[file] = dialog.SelectedDirectory;
            }
            MovingHelper.AddToQueue(oldPathToNewPath, true, false, dialog.FixPackages);
        }

        /// <summary>
        /// 
        /// </summary>
        private void MoveFile(string oldPath, string newPath)
        {
            try
            {
                var command = CommandFactoryProvider.GetFactoryForCurrentDocument().CreateRenameFileCommand(oldPath, newPath);
                command.Execute();
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Invoked when the user selects the "Find All References" command
        /// </summary>
        private void FindAllReferencesClicked(Object sender, EventArgs e)
        {
            try
            {
                var command = CommandFactoryProvider.GetFactoryForCurrentDocument().CreateFindAllReferencesCommand(true);
                command.Execute();
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Invoked when the user selects the "Organize Imports" command
        /// </summary>
        private void OrganizeImportsClicked(Object sender, EventArgs e)
        {
            try
            {
                var command = (OrganizeImports)CommandFactoryProvider.GetFactoryForCurrentDocument().CreateOrganizeImportsCommand();
                command.SeparatePackages = this.settingObject.SeparatePackages;
                command.Execute();
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Invoked when the user selects the "Truncate Imports" command
        /// </summary>
        private void TruncateImportsClicked(Object sender, EventArgs e)
        {
            try
            {
                var command = (OrganizeImports)CommandFactoryProvider.GetFactoryForCurrentDocument().CreateOrganizeImportsCommand();
                command.SeparatePackages = this.settingObject.SeparatePackages;
                command.TruncateImports = true;
                command.Execute();
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Invoked when the user selects the "Delegate Method" command
        /// </summary>
        private void DelegateMethodsClicked(Object sender, EventArgs e)
        {
            try
            {
                ASResult result = ASComplete.CurrentResolvedContext.Result;
                Dictionary<MemberModel, ClassModel> members = new Dictionary<MemberModel, ClassModel>();
                List<String> memberNames = new List<String>();
                ClassModel cm = result.Type;
                cm.ResolveExtends();
                while (cm != null && !cm.IsVoid() && cm.Type != "Object")
                {
                    cm.Members.Sort();
                    foreach (MemberModel m in cm.Members)
                    {
                        if (((m.Flags & FlagType.Function) > 0 || (m.Flags & FlagType.Getter) > 0 || (m.Flags & FlagType.Setter) > 0)
                            && (m.Access & Visibility.Public) > 0
                            && (m.Flags & FlagType.Constructor) == 0
                            && (m.Flags & FlagType.Static) == 0)
                        {
                            string name = m.Name;
                            if ((m.Flags & FlagType.Getter) > 0) name = "get " + name;
                            if ((m.Flags & FlagType.Setter) > 0) name = "set " + name;
                            if (!memberNames.Contains(name))
                            {
                                memberNames.Add(name);
                                members[m] = cm;
                            }
                        }
                    }
                    cm = cm.Extends;
                }
                var dialog = new DelegateMethodsDialog();
                dialog.FillData(members, result.Type);
                if (dialog.ShowDialog() != DialogResult.OK || dialog.checkedMembers.Count <= 0) return;
                var command = CommandFactoryProvider.GetFactoryForCurrentDocument().CreateDelegateMethodsCommand(result, dialog.checkedMembers);
                command.Execute();
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Invoked when the user selects the "Extract Method" command
        /// </summary>
        private void ExtractMethodClicked(Object sender, EventArgs e)
        {
            try
            {
                String newName = "newMethod";
                String label = TextHelper.GetString("Label.NewName");
                String title = TextHelper.GetString("Title.ExtractMethodDialog");
                LineEntryDialog askName = new LineEntryDialog(title, label, newName);
                var result = askName.ShowDialog();
                if (result != DialogResult.OK) return;
                if (askName.Line.Trim().Length > 0 && askName.Line.Trim() != newName)
                {
                    newName = askName.Line.Trim();
                }
                var command = CommandFactoryProvider.GetFactoryForCurrentDocument().CreateExtractMethodCommand(newName);
                command.Execute();
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Invoked when the user selects the "Extract Local Variable" command
        /// </summary>
        private void ExtractLocalVariableClicked(Object sender, EventArgs e)
        {
            try
            {
                var command = CommandFactoryProvider.GetFactoryForCurrentDocument().CreateExtractLocalVariableCommand();
                command.Execute();
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Invokes the batch processing dialog
        /// </summary>
        private void BatchMenuItemClicked(Object sender, EventArgs e)
        {
            BatchProcessDialog dialog = new BatchProcessDialog();
            dialog.ShowDialog();
        }

        /// <summary>
        /// Invokes the ASCompletion contextual generator
        /// </summary>
        private void CodeGeneratorMenuItemClicked(Object sender, EventArgs e)
        {
            DataEvent de = new DataEvent(EventType.Command, "ASCompletion.ContextualGenerator", null);
            EventManager.DispatchEvent(this, de);
        }

        /// <summary>
        /// Loads the plugin settings
        /// </summary>
        public void LoadSettings()
        {
            this.settingObject = new Settings();
            if (!File.Exists(this.settingFilename)) this.SaveSettings();
            else
            {
                Object obj = ObjectSerializer.Deserialize(this.settingFilename, this.settingObject);
                this.settingObject = (Settings)obj;
            }
        }

        /// <summary>
        /// Saves the plugin settings
        /// </summary>
        public void SaveSettings()
        {
            ObjectSerializer.Serialize(this.settingFilename, this.settingObject);
        }

        void OnAddRefactorOptions(List<ICompletionListItem> list)
        {
            if (list == null) return;

            RefactorItem.AddItemToList(refactorMainMenu.RenameMenuItem, list);
            RefactorItem.AddItemToList(refactorMainMenu.ExtractMethodMenuItem, list);
            RefactorItem.AddItemToList(refactorMainMenu.ExtractLocalVariableMenuItem, list);
            RefactorItem.AddItemToList(refactorMainMenu.DelegateMenuItem, list);
            RefactorItem.AddItemToList(refactorMainMenu.SurroundMenu, list);

            var features = ASContext.Context.Features;
            if (!features.hasImports) return;

            var sci = ASContext.CurSciControl;
            var line = sci.GetLine(sci.CurrentLine).TrimStart();

            if (line.StartsWithOrdinal(features.importKey)
                || !string.IsNullOrEmpty(features.importKeyAlt) && line.StartsWithOrdinal(features.importKeyAlt))
            {
                RefactorItem.AddItemToList(refactorMainMenu.OrganizeMenuItem, list);

                if (features.hasImportsWildcard)
                    RefactorItem.AddItemToList(refactorMainMenu.TruncateMenuItem, list);
            }
        }

        void OnDirectoryNodeRefresh(DirectoryNode node)
        {
            projectTreeView = node.TreeView;
        }

        void OnTreeSelectionChanged()
        {
            if (projectTreeView == null) return;
            string path = null;
            var node = projectTreeView.SelectedNode as GenericNode;
            if (node != null) path = node.BackingPath;
            if (string.IsNullOrEmpty(path) || !IsValidForMove(path)) return;
            var menu = (ProjectContextMenu) projectTreeView.ContextMenuStrip;
            var index = menu.Items.IndexOf(menu.Rename);
            if (index == -1) return;
            var item = new ToolStripMenuItem(TextHelper.GetString("Label.Move"));
            item.ShortcutKeys = PluginBase.MainForm.GetShortcutItemKeys("RefactorMenu.Move");
            item.Click += OnMoveItemClick;
            menu.Items.Insert(index + 1, item);
        }

        void OnMoveItemClick(object sender, EventArgs eventArgs)
        {
            string path = null;
            var node = projectTreeView.SelectedNode as GenericNode;
            if (node != null) path = node.BackingPath;
            if (string.IsNullOrEmpty(path) || !IsValidForMove(path)) return;
            MoveFile(path);
        }

        #endregion
    }
}