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
using ProjectManager.Actions;
using ProjectManager.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

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
		public void HandleEvent(Object sender, NotifyEvent e, HandlingPriority prority)
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
                                RenameFile(oldPath, newPath);
                                e.Handled = true;
                            }
                            break;

                        case ProjectFileActionsEvents.FileMove:
                            args = de.Data as string[];
                            oldPath = args[0];
                            newPath = args[1];
                            if (IsValidForMove(oldPath, newPath))
                            {
                                MovingHelper.AddToQueue(new Dictionary<string, string> { { oldPath, newPath } }, true);
                                e.Handled = true;
                            }
                            break;
                    }
                    break;
            }
		}

        /// <summary>
        /// Checks if the file is valid for rename file command
        /// </summary>
        private bool IsValidForRename(string oldPath, string newPath)
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
        private bool IsValidForMove(string oldPath, string newPath)
        {
            return PluginBase.CurrentProject != null
                && (File.Exists(oldPath) || Directory.Exists(oldPath))
                && IsValidFile(oldPath)
                && Regex.Match(Path.GetFileNameWithoutExtension(newPath), REG_IDENTIFIER, RegexOptions.Singleline).Success;
        }

        /// <summary>
        /// Checks if the file or directory is ok for refactoring
        /// </summary>
        private bool IsValidFile(string file)
        {
            IProject project = PluginBase.CurrentProject;
            if (project == null
                || !RefactoringHelper.IsProjectRelatedFile(project, file)
                || !Regex.Match(Path.GetFileNameWithoutExtension(file), REG_IDENTIFIER, RegexOptions.Singleline).Success)
                return false;
            if (Directory.Exists(file)) return true;
            return FileHelper.FileMatchesSearchFilter(file, project.DefaultSearchFilter);
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
        }

        /// <summary>
        /// Creates the required menu items
        /// </summary>
        private void CreateMenuItems()
        {
            this.refactorMainMenu = new RefactorMenu(true);
            this.refactorMainMenu.RenameMenuItem.Click += this.RenameClicked;
            this.refactorMainMenu.OrganizeMenuItem.Click += this.OrganizeImportsClicked;
            this.refactorMainMenu.TruncateMenuItem.Click += this.TruncateImportsClicked;
            this.refactorMainMenu.ExtractMethodMenuItem.Click += this.ExtractMethodClicked;
            this.refactorMainMenu.DelegateMenuItem.Click += this.DelegateMethodsClicked;
            this.refactorMainMenu.ExtractLocalVariableMenuItem.Click += this.ExtractLocalVariableClicked;
            this.refactorMainMenu.CodeGeneratorMenuItem.Click += this.CodeGeneratorMenuItemClicked;
            this.refactorContextMenu = new RefactorMenu(false);
            this.refactorContextMenu.RenameMenuItem.Click += this.RenameClicked;
            this.refactorContextMenu.OrganizeMenuItem.Click += this.OrganizeImportsClicked;
            this.refactorContextMenu.TruncateMenuItem.Click += this.TruncateImportsClicked;
            this.refactorContextMenu.DelegateMenuItem.Click += this.DelegateMethodsClicked;
            this.refactorContextMenu.ExtractMethodMenuItem.Click += this.ExtractMethodClicked;
            this.refactorContextMenu.ExtractLocalVariableMenuItem.Click += this.ExtractLocalVariableClicked;
            this.refactorContextMenu.CodeGeneratorMenuItem.Click += this.CodeGeneratorMenuItemClicked;
            ContextMenuStrip editorMenu = PluginBase.MainForm.EditorMenu;
            this.surroundContextMenu = new SurroundMenu();
            editorMenu.Items.Insert(3, this.refactorContextMenu);
            editorMenu.Items.Insert(4, this.surroundContextMenu);
            PluginBase.MainForm.MenuStrip.Items.Insert(5, this.refactorMainMenu);
            ToolStripMenuItem searchMenu = PluginBase.MainForm.FindMenuItem("SearchMenu") as ToolStripMenuItem;
            this.viewReferencesItem = new ToolStripMenuItem(TextHelper.GetString("Label.FindAllReferences"), null, this.FindAllReferencesClicked);
            this.editorReferencesItem = new ToolStripMenuItem(TextHelper.GetString("Label.FindAllReferences"), null, this.FindAllReferencesClicked);
            PluginBase.MainForm.RegisterShortcutItem("SearchMenu.ViewReferences", this.viewReferencesItem);
            PluginBase.MainForm.RegisterSecondaryItem("SearchMenu.ViewReferences", this.editorReferencesItem);
            searchMenu.DropDownItems.Add(new ToolStripSeparator());
            searchMenu.DropDownItems.Add(this.viewReferencesItem);
            editorMenu.Items.Insert(7, this.editorReferencesItem);
            RegisterMenuItems();
        }

        /// <summary>
        /// Registers the menu items with the shortcut manager
        /// </summary>
        private void RegisterMenuItems()
        {
            PluginBase.MainForm.RegisterShortcutItem("RefactorMenu.Rename", this.refactorMainMenu.RenameMenuItem);
            PluginBase.MainForm.RegisterShortcutItem("RefactorMenu.ExtractMethod", this.refactorMainMenu.ExtractMethodMenuItem);
            PluginBase.MainForm.RegisterShortcutItem("RefactorMenu.ExtractLocalVariable", this.refactorMainMenu.ExtractLocalVariableMenuItem);
            PluginBase.MainForm.RegisterShortcutItem("RefactorMenu.GenerateDelegateMethods", this.refactorMainMenu.DelegateMenuItem);
            PluginBase.MainForm.RegisterShortcutItem("RefactorMenu.OrganizeImports", this.refactorMainMenu.OrganizeMenuItem);
            PluginBase.MainForm.RegisterShortcutItem("RefactorMenu.TruncateImports", this.refactorMainMenu.TruncateMenuItem);
            PluginBase.MainForm.RegisterShortcutItem("RefactorMenu.CodeGenerator", this.refactorMainMenu.CodeGeneratorMenuItem);
            PluginBase.MainForm.RegisterSecondaryItem("RefactorMenu.Rename", this.refactorContextMenu.RenameMenuItem);
            PluginBase.MainForm.RegisterSecondaryItem("RefactorMenu.ExtractMethod", this.refactorContextMenu.ExtractMethodMenuItem);
            PluginBase.MainForm.RegisterSecondaryItem("RefactorMenu.ExtractLocalVariable", this.refactorContextMenu.ExtractLocalVariableMenuItem);
            PluginBase.MainForm.RegisterSecondaryItem("RefactorMenu.GenerateDelegateMethods", this.refactorContextMenu.DelegateMenuItem);
            PluginBase.MainForm.RegisterSecondaryItem("RefactorMenu.OrganizeImports", this.refactorContextMenu.OrganizeMenuItem);
            PluginBase.MainForm.RegisterSecondaryItem("RefactorMenu.TruncateImports", this.refactorContextMenu.TruncateMenuItem);
            PluginBase.MainForm.RegisterSecondaryItem("RefactorMenu.CodeGenerator", this.refactorContextMenu.CodeGeneratorMenuItem);
        }

        /// <summary>
        /// Gets if the current documents language is haxe
        /// </summary>
        private Boolean LanguageIsHaxe()
        {
            ITabbedDocument document = PluginBase.MainForm.CurrentDocument;
            if (document == null || !document.IsEditable) return false;
            return document.SciControl.ConfigurationLanguage == "haxe";
        }

        /// <summary>
        /// Gets if the language is valid for refactoring
        /// </summary>
        private Boolean GetLanguageIsValid()
        {
            ITabbedDocument document = PluginBase.MainForm.CurrentDocument;
            if (document == null || !document.IsEditable) return false;
            string lang = document.SciControl.ConfigurationLanguage;
            return lang == "as2" || lang == "as3" || lang == "haxe" || lang == "loom"; // TODO: look for /Snippets/Generators
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
                this.refactorMainMenu.DelegateMenuItem.Enabled = false;
                this.refactorContextMenu.DelegateMenuItem.Enabled = false;
                bool langIsValid = GetLanguageIsValid();
                ResolvedContext resolved = ASComplete.CurrentResolvedContext;
                bool isValid = langIsValid && resolved != null && resolved.Position >= 0;
                ASResult result = isValid ? resolved.Result : null;
                if (result != null && !result.IsNull())
                {
                    bool isRenameable = (result.Member != null && result.Member.InFile != null && File.Exists(result.Member.InFile.FileName))
                        || (result.Type != null && result.Type.InFile != null && File.Exists(result.Type.InFile.FileName))
                        || (result.InFile != null && File.Exists(result.InFile.FileName))
                        || result.IsPackage;
                    this.refactorContextMenu.RenameMenuItem.Enabled = isRenameable;
                    this.refactorMainMenu.RenameMenuItem.Enabled = isRenameable;
                    bool isNotPackage = !result.IsPackage;
                    this.editorReferencesItem.Enabled = isNotPackage;
                    this.viewReferencesItem.Enabled = isNotPackage;
                    if (result.Member != null && result.Type != null && result.InClass != null && result.InFile != null)
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
                    bool truncate = (langIsValid && context.CurrentModel.Imports.Count > 0) && !this.LanguageIsHaxe();
                    bool organize = (langIsValid && context.CurrentModel.Imports.Count > 1);
                    this.refactorContextMenu.OrganizeMenuItem.Enabled = organize;
                    this.refactorContextMenu.TruncateMenuItem.Enabled = truncate;
                    this.refactorMainMenu.OrganizeMenuItem.Enabled = organize;
                    this.refactorMainMenu.TruncateMenuItem.Enabled = truncate;
                }
                this.surroundContextMenu.Enabled = false;
                this.refactorMainMenu.SurroundMenu.Enabled = false;
                this.refactorContextMenu.ExtractMethodMenuItem.Enabled = false;
                this.refactorContextMenu.ExtractLocalVariableMenuItem.Enabled = false;
                this.refactorMainMenu.ExtractMethodMenuItem.Enabled = false;
                this.refactorMainMenu.ExtractLocalVariableMenuItem.Enabled = false;
                ITabbedDocument document = PluginBase.MainForm.CurrentDocument;
                if (document != null && document.IsEditable && langIsValid && document.SciControl.SelTextSize > 1)
                {
                    Int32 selEnd = document.SciControl.SelectionEnd;
                    Int32 selStart = document.SciControl.SelectionStart;
                    if (!document.SciControl.PositionIsOnComment(selEnd) || !document.SciControl.PositionIsOnComment(selStart))
                    {
                        this.surroundContextMenu.Enabled = true;
                        this.refactorMainMenu.SurroundMenu.Enabled = true;
                        this.refactorContextMenu.ExtractMethodMenuItem.Enabled = true;
                        this.refactorMainMenu.ExtractMethodMenuItem.Enabled = true;
                        this.refactorContextMenu.ExtractLocalVariableMenuItem.Enabled = true;
                        this.refactorMainMenu.ExtractLocalVariableMenuItem.Enabled = true;
                    }
                }
                this.refactorContextMenu.CodeGeneratorMenuItem.Enabled = isValid;
                this.refactorMainMenu.CodeGeneratorMenuItem.Enabled = isValid;
            }
            catch {}
        }

        /// <summary>
        /// Generate surround main menu and context menu items
        /// </summary>
        private void GenerateSurroundMenuItems()
        {
            ITabbedDocument document = PluginBase.MainForm.CurrentDocument;
            if (document != null && document.IsEditable && this.GetLanguageIsValid())
            {
                this.surroundContextMenu.GenerateSnippets(document.SciControl);
                foreach (ToolStripMenuItem item in this.surroundContextMenu.DropDownItems)
                {
                    item.Click += this.SurroundWithClicked;
                }
                foreach (ToolStripMenuItem item in this.refactorMainMenu.SurroundMenu.DropDownItems)
                {
                    item.Click -= this.SurroundWithClicked;
                }
                this.refactorMainMenu.SurroundMenu.GenerateSnippets(document.SciControl);
                foreach (ToolStripMenuItem item in this.refactorMainMenu.SurroundMenu.DropDownItems)
                {
                    item.Click += this.SurroundWithClicked;
                }
            }
            else
            {
                this.surroundContextMenu.DropDownItems.Clear();
                this.refactorMainMenu.SurroundMenu.DropDownItems.Clear();
                this.refactorMainMenu.SurroundMenu.DropDownItems.Add("");
                this.surroundContextMenu.DropDownItems.Add("");
            }
        }

        /// <summary>
        /// Invoked when the user selects the "Rename" command
        /// </summary>
        private void RenameClicked(Object sender, EventArgs e)
        {
            try
            {
                Rename command = new Rename(true);
                command.Execute();
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void RenameFile(string oldPath, string newPath)
        {
            try
            {
                RenameFile command = new RenameFile(oldPath, newPath);
                command.Execute();
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
        }

        /// <summary>
        /// Invoked when the user selects the "Surround with Try/catch block" command
        /// </summary>
        private void SurroundWithClicked(Object sender, EventArgs e)
        {
            try
            {
                SurroundWithCommand command = new SurroundWithCommand((sender as ToolStripItem).Text);
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
                FindAllReferences command = new FindAllReferences(true);
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
                OrganizeImports command = new OrganizeImports();
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
                OrganizeImports command = new OrganizeImports();
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
                DelegateMethodsDialog dd = new DelegateMethodsDialog();
                dd.FillData(members, result.Type);
                DialogResult choice = dd.ShowDialog();
                if (choice == DialogResult.OK && dd.checkedMembers.Count > 0)
                {
                    DelegateMethodsCommand command = new DelegateMethodsCommand(result, dd.checkedMembers);
                    command.Execute();
                }
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
                String suggestion = "newMethod";
                String label = TextHelper.GetString("Label.NewName");
                String title = TextHelper.GetString("Title.ExtractMethodDialog");
                LineEntryDialog askName = new LineEntryDialog(title, label, suggestion);
                DialogResult choice = askName.ShowDialog();
                if (choice == DialogResult.OK && askName.Line.Trim().Length > 0 && askName.Line.Trim() != suggestion)
                {
                    suggestion = askName.Line.Trim();
                }
                if (choice == DialogResult.OK)
                {
                    ExtractMethodCommand command = new ExtractMethodCommand(suggestion);
                    command.Execute();
                }
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
                String suggestion = "newVar";
                String label = TextHelper.GetString("Label.NewName");
                String title = TextHelper.GetString("Title.ExtractLocalVariableDialog");
                LineEntryDialog askName = new LineEntryDialog(title, label, suggestion);
                DialogResult choice = askName.ShowDialog();
                if (choice == DialogResult.OK && askName.Line.Trim().Length > 0 && askName.Line.Trim() != suggestion)
                {
                    suggestion = askName.Line.Trim();
                }
                if (choice == DialogResult.OK)
                {
                    ExtractLocalVariableCommand command = new ExtractLocalVariableCommand(suggestion);
                    command.Execute();
                }
            }
            catch (Exception ex)
            {
                ErrorManager.ShowError(ex);
            }
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

		#endregion

    }
}