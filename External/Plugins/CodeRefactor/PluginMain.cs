using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
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
        const string REG_IDENTIFIER = "^[a-zA-Z_$][a-zA-Z0-9_$]*$";
        private ToolStripMenuItem editorReferencesItem;
        private ToolStripMenuItem viewReferencesItem;
        private SurroundMenu surroundContextMenu;
        private RefactorMenu refactorContextMenu;
        private RefactorMenu refactorMainMenu;
        private Settings settingObject;
        private string settingFilename;
        TreeView projectTreeView;

        public const string TraceGroup = nameof(CodeRefactor);
        
        #region Required Properties

        /// <summary>
        /// Api level of the plugin
        /// </summary>
        public int Api => 1;

        /// <summary>
        /// Name of the plugin
        /// </summary> 
        public string Name { get; } = nameof(CodeRefactor);

        /// <summary>
        /// GUID of the plugin
        /// </summary>
        public string Guid { get; } = "5c0d3740-a6f2-11de-8a39-0800200c9a66";

        /// <summary>
        /// Author of the plugin
        /// </summary> 
        public string Author { get; } = "FlashDevelop Team";

        /// <summary>
        /// Description of the plugin
        /// </summary> 
        public string Description { get; set; } = "Adds refactoring capabilities to FlashDevelop.";

        /// <summary>
        /// Web address for help
        /// </summary> 
        public string Help { get; } = "www.flashdevelop.org/community/";

        /// <summary>
        /// Object that contains the settings
        /// </summary>
        [Browsable(false)]
        public object Settings => settingObject;

        #endregion

        #region Required Methods

        /// <summary>
        /// Initializes the plugin
        /// </summary>
        public void Initialize()
        {
            InitBasics();
            LoadSettings();
            CreateMenuItems();
            RegisterMenuItems();
            RegisterTraceGroups();
        }

        /// <summary>
        /// Disposes the plugin
        /// </summary>
        public void Dispose() => SaveSettings();

        /// <summary>
        /// Handles the incoming events
        /// </summary>
        public void HandleEvent(object sender, NotifyEvent e, HandlingPriority priority)
        {
            switch (e.Type)
            {
                case EventType.FileSwitch:
                    GenerateSurroundMenuItems();
                    UpdateMenuItems();
                    break;

                case EventType.UIStarted:
                    // Expose plugin's refactor main menu & context menu...
                    EventManager.DispatchEvent(this, new DataEvent(EventType.Command, "CodeRefactor.Menu", refactorMainMenu));
                    EventManager.DispatchEvent(this, new DataEvent(EventType.Command, "CodeRefactor.ContextMenu", refactorContextMenu));
                    // Watch resolved context for menu item updating...
                    ASComplete.OnResolvedContextChanged += OnResolvedContextChanged;
                    DirectoryNode.OnDirectoryNodeRefresh += OnDirectoryNodeRefresh;
                    UpdateMenuItems();
                    break;

                case EventType.Command:
                    var de = (DataEvent)e;
                    string[] args;
                    string oldPath;
                    string newPath;
                    switch (de.Action)
                    {
                        case ProjectFileActionsEvents.FileRename:
                            if (settingObject.DisableMoveRefactoring) break;
                            args = (string[]) de.Data;
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
                            args = (string[]) de.Data;
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
        static bool IsValidForRename(string oldPath, string newPath)
        {
            return PluginBase.CurrentProject != null
                && Path.GetExtension(oldPath) is string oldExt && File.Exists(oldPath)
                && Path.GetExtension(newPath) is string newExt && oldExt == newExt
                && IsValidFile(oldPath)
                && Path.GetFileNameWithoutExtension(newPath) is string newPathWithoutExtension
                && Regex.Match(newPathWithoutExtension, REG_IDENTIFIER, RegexOptions.Singleline).Success;
        }

        /// <summary>
        /// Checks if the file or directory is valid for move command
        /// </summary>
        static bool IsValidForMove(string oldPath)
        {
            return PluginBase.CurrentProject != null
                && !RefactoringHelper.IsUnderSDKPath(oldPath)
                && (File.Exists(oldPath) || Directory.Exists(oldPath))
                && IsValidFile(oldPath);
        }

        /// <summary>
        /// Checks if the file or directory is valid for move command
        /// </summary>
        static bool IsValidForMove(string oldPath, string newPath)
        {
            return IsValidForMove(oldPath)
                && Path.GetFileNameWithoutExtension(newPath) is string newPathWithoutExtension
                && Regex.Match(newPathWithoutExtension, REG_IDENTIFIER, RegexOptions.Singleline).Success;
        }

        /// <summary>
        /// Checks if the file or directory is ok for refactoring
        /// </summary>
        static bool IsValidFile(string file)
        {
            return PluginBase.CurrentProject is IProject project
                && RefactoringHelper.IsProjectRelatedFile(project, file)
                && Path.GetFileNameWithoutExtension(file) is string fileNameWithoutExtension
                && Regex.Match(fileNameWithoutExtension, REG_IDENTIFIER, RegexOptions.Singleline).Success
                && ((Directory.Exists(file) && !IsEmpty(file, project.DefaultSearchFilter)) || FileHelper.FileMatchesSearchFilter(file, project.DefaultSearchFilter));
            // Utils
            bool IsEmpty(string directoryPath, string searchPattern)
            {
                return searchPattern.Split(';').All(pattern => !Directory.EnumerateFiles(directoryPath, pattern, SearchOption.AllDirectories).Any());
            }
        }

        #endregion

        #region Event Handling

        /// <summary>
        /// Initializes important variables
        /// </summary>
        private void InitBasics()
        {
            EventManager.AddEventHandler(this, EventType.UIStarted | EventType.FileSwitch | EventType.Command);
            var dataPath = Path.Combine(PathHelper.DataDir, nameof(CodeRefactor));
            if (!Directory.Exists(dataPath)) Directory.CreateDirectory(dataPath);
            settingFilename = Path.Combine(dataPath, "Settings.fdb");
            Description = TextHelper.GetString("Info.Description");

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
            refactorMainMenu = new RefactorMenu(true);
            refactorMainMenu.RenameMenuItem.Click += RenameClicked;
            refactorMainMenu.MoveMenuItem.Click += MoveClicked;
            refactorMainMenu.OrganizeMenuItem.Click += OrganizeImportsClicked;
            refactorMainMenu.TruncateMenuItem.Click += TruncateImportsClicked;
            refactorMainMenu.ExtractMethodMenuItem.Click += ExtractMethodClicked;
            refactorMainMenu.DelegateMenuItem.Click += DelegateMethodsClicked;
            refactorMainMenu.ExtractLocalVariableMenuItem.Click += ExtractLocalVariableClicked;
            refactorMainMenu.CodeGeneratorMenuItem.Click += CodeGeneratorMenuItemClicked;
            refactorMainMenu.BatchMenuItem.Click += BatchMenuItemClicked;
            refactorContextMenu = new RefactorMenu(false);
            refactorContextMenu.RenameMenuItem.Click += RenameClicked;
            refactorContextMenu.MoveMenuItem.Click += MoveClicked;
            refactorContextMenu.OrganizeMenuItem.Click += OrganizeImportsClicked;
            refactorContextMenu.TruncateMenuItem.Click += TruncateImportsClicked;
            refactorContextMenu.DelegateMenuItem.Click += DelegateMethodsClicked;
            refactorContextMenu.ExtractMethodMenuItem.Click += ExtractMethodClicked;
            refactorContextMenu.ExtractLocalVariableMenuItem.Click += ExtractLocalVariableClicked;
            refactorContextMenu.CodeGeneratorMenuItem.Click += CodeGeneratorMenuItemClicked;
            refactorContextMenu.BatchMenuItem.Click += BatchMenuItemClicked;
            ContextMenuStrip editorMenu = PluginBase.MainForm.EditorMenu;
            surroundContextMenu = new SurroundMenu();
            editorMenu.Items.Insert(3, refactorContextMenu);
            editorMenu.Items.Insert(4, surroundContextMenu);
            PluginBase.MainForm.MenuStrip.Items.Insert(5, refactorMainMenu);
            ToolStripMenuItem searchMenu = PluginBase.MainForm.FindMenuItem("SearchMenu") as ToolStripMenuItem;
            viewReferencesItem = new ToolStripMenuItem(TextHelper.GetString("Label.FindAllReferences"), null, FindAllReferencesClicked);
            editorReferencesItem = new ToolStripMenuItem(TextHelper.GetString("Label.FindAllReferences"), null, FindAllReferencesClicked);
            PluginBase.MainForm.RegisterShortcutItem("RefactorMenu.SurroundWith", refactorMainMenu.SurroundMenu);
            PluginBase.MainForm.RegisterShortcutItem("SearchMenu.ViewReferences", viewReferencesItem);
            PluginBase.MainForm.RegisterSecondaryItem("RefactorMenu.SurroundWith", surroundContextMenu);
            PluginBase.MainForm.RegisterSecondaryItem("SearchMenu.ViewReferences", editorReferencesItem);
            searchMenu.DropDownItems.Add(new ToolStripSeparator());
            searchMenu.DropDownItems.Add(viewReferencesItem);
            editorMenu.Items.Insert(8, editorReferencesItem);
        }

        /// <summary>
        /// Registers the menu items with the shortcut manager
        /// </summary>
        private void RegisterMenuItems()
        {
            PluginBase.MainForm.RegisterShortcutItem("RefactorMenu.Rename", refactorMainMenu.RenameMenuItem);
            PluginBase.MainForm.RegisterShortcutItem("RefactorMenu.Move", refactorMainMenu.MoveMenuItem);
            PluginBase.MainForm.RegisterShortcutItem("RefactorMenu.ExtractMethod", refactorMainMenu.ExtractMethodMenuItem);
            PluginBase.MainForm.RegisterShortcutItem("RefactorMenu.ExtractLocalVariable", refactorMainMenu.ExtractLocalVariableMenuItem);
            PluginBase.MainForm.RegisterShortcutItem("RefactorMenu.GenerateDelegateMethods", refactorMainMenu.DelegateMenuItem);
            PluginBase.MainForm.RegisterShortcutItem("RefactorMenu.OrganizeImports", refactorMainMenu.OrganizeMenuItem);
            PluginBase.MainForm.RegisterShortcutItem("RefactorMenu.TruncateImports", refactorMainMenu.TruncateMenuItem);
            PluginBase.MainForm.RegisterShortcutItem("RefactorMenu.CodeGenerator", refactorMainMenu.CodeGeneratorMenuItem);
            PluginBase.MainForm.RegisterShortcutItem("RefactorMenu.BatchProcess", refactorMainMenu.BatchMenuItem);
            PluginBase.MainForm.RegisterSecondaryItem("RefactorMenu.Rename", refactorContextMenu.RenameMenuItem);
            PluginBase.MainForm.RegisterSecondaryItem("RefactorMenu.Move", refactorContextMenu.MoveMenuItem);
            PluginBase.MainForm.RegisterSecondaryItem("RefactorMenu.ExtractMethod", refactorContextMenu.ExtractMethodMenuItem);
            PluginBase.MainForm.RegisterSecondaryItem("RefactorMenu.ExtractLocalVariable", refactorContextMenu.ExtractLocalVariableMenuItem);
            PluginBase.MainForm.RegisterSecondaryItem("RefactorMenu.GenerateDelegateMethods", refactorContextMenu.DelegateMenuItem);
            PluginBase.MainForm.RegisterSecondaryItem("RefactorMenu.OrganizeImports", refactorContextMenu.OrganizeMenuItem);
            PluginBase.MainForm.RegisterSecondaryItem("RefactorMenu.TruncateImports", refactorContextMenu.TruncateMenuItem);
            PluginBase.MainForm.RegisterSecondaryItem("RefactorMenu.CodeGenerator", refactorContextMenu.CodeGeneratorMenuItem);
            PluginBase.MainForm.RegisterSecondaryItem("RefactorMenu.BatchProcess", refactorContextMenu.BatchMenuItem);
        }

        private void RegisterTraceGroups()
        {
            TraceManager.RegisterTraceGroup(TraceGroup, TextHelper.GetStringWithoutMnemonics("Label.Refactor"), false);
            TraceManager.RegisterTraceGroup(FindAllReferences.TraceGroup, TextHelper.GetString("Label.FindAllReferencesResult"), false, true);
        }

        /// <summary>
        /// Cursor position changed and word at this position was resolved
        /// </summary>
        private void OnResolvedContextChanged(ResolvedContext resolved) => UpdateMenuItems(resolved);

        /// <summary>
        /// Updates the state of the menu items
        /// </summary>
        private void UpdateMenuItems() => UpdateMenuItems(ASComplete.CurrentResolvedContext);

        private void UpdateMenuItems(ResolvedContext resolved)
        {
            try
            {
                var document = PluginBase.MainForm.CurrentDocument;
                var curFileName = document != null ? document.FileName : string.Empty;
                var langIsValid = RefactoringHelper.GetLanguageIsValid();
                var isValid = langIsValid && resolved != null && resolved.Position >= 0;
                var result = isValid ? resolved.Result : null;
                if (result != null && !result.IsNull())
                {
                    // Rename
                    var validator = CommandFactoryProvider.GetFactory(result)?.GetValidator(typeof(Rename))
                                    ?? CommandFactoryProvider.DefaultFactory.GetValidator(typeof(Rename));
                    var enabled = validator(result);
                    refactorContextMenu.RenameMenuItem.Enabled = enabled;
                    refactorMainMenu.RenameMenuItem.Enabled = enabled;
                    // Find All References
                    enabled = !result.IsPackage && (File.Exists(curFileName) || curFileName.Contains("[model]"));
                    editorReferencesItem.Enabled = enabled;
                    viewReferencesItem.Enabled = enabled;
                    // Generate Delegate Methods
                    validator = CommandFactoryProvider.GetFactoryForCurrentDocument().GetValidator(typeof(DelegateMethods))
                             ?? CommandFactoryProvider.DefaultFactory.GetValidator(typeof(DelegateMethods));
                    enabled = validator(result);
                    refactorContextMenu.DelegateMenuItem.Enabled = enabled;
                    refactorMainMenu.DelegateMenuItem.Enabled = enabled;
                }
                else
                {
                    refactorMainMenu.RenameMenuItem.Enabled = false;
                    refactorContextMenu.RenameMenuItem.Enabled = false;
                    editorReferencesItem.Enabled = false;
                    viewReferencesItem.Enabled = false;
                    refactorMainMenu.DelegateMenuItem.Enabled = false;
                    refactorContextMenu.DelegateMenuItem.Enabled = false;
                }
                var context = ASContext.Context;
                if (context?.CurrentModel != null)
                {
                    var enabled = false;
                    if (RefactoringHelper.GetLanguageIsValid())
                    {
                        var validator = CommandFactoryProvider.GetFactoryForCurrentDocument().GetValidator(typeof(OrganizeImports))
                                        ?? CommandFactoryProvider.DefaultFactory.GetValidator(typeof(OrganizeImports));
                        enabled = validator(new ASResult {InFile = context.CurrentModel});
                    }
                    refactorContextMenu.OrganizeMenuItem.Enabled = enabled;
                    refactorContextMenu.TruncateMenuItem.Enabled = enabled;
                    refactorMainMenu.OrganizeMenuItem.Enabled = enabled;
                    refactorMainMenu.TruncateMenuItem.Enabled = enabled;
                }
                refactorMainMenu.MoveMenuItem.Enabled = false;
                refactorContextMenu.MoveMenuItem.Enabled = false;
                surroundContextMenu.Enabled = false;
                refactorMainMenu.SurroundMenu.Enabled = false;
                refactorMainMenu.ExtractMethodMenuItem.Enabled = false;
                refactorContextMenu.ExtractMethodMenuItem.Enabled = false;
                refactorMainMenu.ExtractLocalVariableMenuItem.Enabled = false;
                refactorContextMenu.ExtractLocalVariableMenuItem.Enabled = false;
                if (document != null && document.IsEditable && langIsValid)
                {
                    var isValidFile = IsValidForMove(curFileName);
                    refactorMainMenu.MoveMenuItem.Enabled = isValidFile;
                    refactorContextMenu.MoveMenuItem.Enabled = isValidFile;
                    var sci = document.SciControl;
                    if (sci.SelTextSize > 0)
                    {
                        if (!sci.PositionIsOnComment(sci.SelectionStart) || !sci.PositionIsOnComment(sci.SelectionEnd))
                        {
                            surroundContextMenu.Enabled = true;
                            refactorMainMenu.SurroundMenu.Enabled = true;
                            refactorMainMenu.ExtractMethodMenuItem.Enabled = true;
                            refactorContextMenu.ExtractMethodMenuItem.Enabled = true;
                        }
                        if (context != null)
                        {
                            var declAtSelStart = context.GetDeclarationAtLine(sci.LineFromPosition(sci.SelectionStart));
                            var declAtSelEnd = context.GetDeclarationAtLine(sci.LineFromPosition(sci.SelectionEnd));
                            if (declAtSelStart?.Member != null && (declAtSelStart.Member.Flags & FlagType.Function) > 0
                                && declAtSelEnd != null && declAtSelStart.Member.Equals(declAtSelEnd.Member))
                            {
                                refactorMainMenu.ExtractLocalVariableMenuItem.Enabled = true;
                                refactorContextMenu.ExtractLocalVariableMenuItem.Enabled = true;
                            }
                        }
                    }
                }
                refactorMainMenu.CodeGeneratorMenuItem.Enabled = isValid;
                refactorContextMenu.CodeGeneratorMenuItem.Enabled = isValid;
            }
            catch { }
        }

        /// <summary>
        /// Generate surround main menu and context menu items
        /// </summary>
        private void GenerateSurroundMenuItems()
        {
            var document = PluginBase.MainForm.CurrentDocument;
            if (document != null && document.IsEditable && RefactoringHelper.GetLanguageIsValid())
            {
                surroundContextMenu.GenerateSnippets(document.SciControl);
                refactorMainMenu.SurroundMenu.GenerateSnippets(document.SciControl);
            }
            else
            {
                surroundContextMenu.Clear();
                refactorMainMenu.SurroundMenu.Clear();
            }
        }

        /// <summary>
        /// Invoked when the user selects the "Rename" command
        /// </summary>
        private void RenameClicked(object sender, EventArgs e)
        {
            if (InlineRename.InProgress) return;
            try
            {
                var factory = CommandFactoryProvider.GetFactoryForCurrentDocument() ?? CommandFactoryProvider.DefaultFactory;
                factory.CreateRenameCommandAndExecute(true, settingObject.UseInlineRenaming);
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Invoked when the user selects the "Move" command
        /// </summary>
        static void MoveClicked(object sender, EventArgs e) => MoveFile(PluginBase.MainForm.CurrentDocument.FileName);

        static void MoveFile(string fileName)
        {
            var dialog = new MoveDialog(fileName);
            if (dialog.ShowDialog() != DialogResult.OK) return;
            var oldPathToNewPath = new Dictionary<string, string>();
            foreach (var file in dialog.MovingFiles)
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
                var factory = CommandFactoryProvider.GetFactoryForCurrentDocument() ?? CommandFactoryProvider.DefaultFactory;
                var command = factory.CreateRenameFileCommand(oldPath, newPath);
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
        private void FindAllReferencesClicked(object sender, EventArgs e)
        {
            ASComplete.OnResolvedContextChanged -= OnResolvedContextChanged;
            EventManager.RemoveEventHandler(this, EventType.FileSwitch, HandlingPriority.Normal);
            try
            {
                var command = CommandFactoryProvider.GetFactoryForCurrentDocument().CreateFindAllReferencesCommand(true);
                command.Execute();
                command.OnRefactorComplete += (o, args) =>
                {
                    ASComplete.OnResolvedContextChanged += OnResolvedContextChanged;
                    EventManager.AddEventHandler(this, EventType.FileSwitch);
                };
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
                ASComplete.OnResolvedContextChanged += OnResolvedContextChanged;
                EventManager.AddEventHandler(this, EventType.FileSwitch);
            }
        }

        /// <summary>
        /// Invoked when the user selects the "Organize Imports" command
        /// </summary>
        private void OrganizeImportsClicked(object sender, EventArgs e)
        {
            try
            {
                var command = (OrganizeImports)CommandFactoryProvider.GetFactoryForCurrentDocument().CreateOrganizeImportsCommand();
                command.SeparatePackages = settingObject.SeparatePackages;
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
        private void TruncateImportsClicked(object sender, EventArgs e)
        {
            try
            {
                var command = (OrganizeImports)CommandFactoryProvider.GetFactoryForCurrentDocument().CreateOrganizeImportsCommand();
                command.SeparatePackages = settingObject.SeparatePackages;
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
        private void DelegateMethodsClicked(object sender, EventArgs e)
        {
            try
            {
                var result = ASComplete.CurrentResolvedContext.Result;
                var members = new Dictionary<MemberModel, ClassModel>();
                var memberNames = new List<string>();
                var cm = result.Type;
                cm.ResolveExtends();
                while (!cm.IsVoid() && cm.Type != "Object")
                {
                    cm.Members.Sort();
                    foreach (MemberModel m in cm.Members)
                    {
                        if (((m.Flags & FlagType.Function) > 0 || (m.Flags & FlagType.Getter) > 0 || (m.Flags & FlagType.Setter) > 0)
                            && (m.Access & Visibility.Public) > 0
                            && (m.Flags & FlagType.Constructor) == 0
                            && (m.Flags & FlagType.Static) == 0)
                        {
                            var name = m.Name;
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
        private void ExtractMethodClicked(object sender, EventArgs e)
        {
            try
            {
                var newName = "newMethod";
                var label = TextHelper.GetString("Label.NewName");
                var title = TextHelper.GetString("Title.ExtractMethodDialog");
                var askName = new LineEntryDialog(title, label, newName);
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
        private void ExtractLocalVariableClicked(object sender, EventArgs e)
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
        private void BatchMenuItemClicked(object sender, EventArgs e)
        {
            var dialog = new BatchProcessDialog();
            dialog.ShowDialog();
        }

        /// <summary>
        /// Invokes the ASCompletion contextual generator
        /// </summary>
        private void CodeGeneratorMenuItemClicked(object sender, EventArgs e)
        {
            var de = new DataEvent(EventType.Command, "ASCompletion.ContextualGenerator", null);
            EventManager.DispatchEvent(this, de);
        }

        /// <summary>
        /// Loads the plugin settings
        /// </summary>
        void LoadSettings()
        {
            settingObject = new Settings();
            if (!File.Exists(settingFilename)) SaveSettings();
            else settingObject = (Settings) ObjectSerializer.Deserialize(settingFilename, settingObject);
        }

        /// <summary>
        /// Saves the plugin settings
        /// </summary>
        void SaveSettings() => ObjectSerializer.Serialize(settingFilename, settingObject);

        void OnAddRefactorOptions(List<ICompletionListItem> list)
        {
            if (list is null) return;

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

        void OnDirectoryNodeRefresh(DirectoryNode node) => projectTreeView = node.TreeView;

        void OnTreeSelectionChanged()
        {
            if (projectTreeView is null) return;
            string path = null;
            if (projectTreeView.SelectedNode is GenericNode node) path = node.BackingPath;
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
            if (projectTreeView.SelectedNode is GenericNode node) path = node.BackingPath;
            if (string.IsNullOrEmpty(path) || !IsValidForMove(path)) return;
            MoveFile(path);
        }

        #endregion
    }
}