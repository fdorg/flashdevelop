using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using ASCompletion.Completion;
using ASCompletion.Context;
using ASCompletion.Model;
using CodeRefactor.Commands;
using CodeRefactor.Controls;
using CodeRefactor.CustomControls;
using CodeRefactor.Provider;
using ProjectManager.Helpers;
using PluginCore.Helpers;
using PluginCore.Localization;
using PluginCore.Managers;
using PluginCore.Utilities;
using PluginCore;

namespace CodeRefactor
{
	public class PluginMain : IPlugin
	{
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
                    break;

                case EventType.UIStarted:
                    // Expose plugin's refactor main menu & context menu...
                    EventManager.DispatchEvent(this, new DataEvent(EventType.Command, "CodeRefactor.Menu", this.refactorMainMenu));
                    EventManager.DispatchEvent(this, new DataEvent(EventType.Command, "CodeRefactor.ContextMenu", this.refactorContextMenu));
                    // Watch resolved context for menu item updating...
                    ASComplete.OnResolvedContextChanged += new ResolvedContextChangeHandler(this.OnResolvedContextChanged);
                    this.UpdateMenuItems();
                    break;
            }
		}

        #endregion
   
        #region Event Handling
        
        /// <summary>
        /// Initializes important variables
        /// </summary>
        public void InitBasics()
        {
            EventManager.AddEventHandler(this, EventType.UIStarted | EventType.FileSwitch);
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
            MenuStrip mainMenu = PluginBase.MainForm.MenuStrip;
            ContextMenuStrip editorMenu = PluginBase.MainForm.EditorMenu;
            this.refactorMainMenu = new RefactorMenu(true);
            this.refactorMainMenu.RenameMenuItem.Click += new EventHandler(this.RenameClicked);
            this.refactorMainMenu.OrganizeMenuItem.Click += new EventHandler(this.OrganizeImportsClicked);
            this.refactorMainMenu.TruncateMenuItem.Click += new EventHandler(this.TruncateImportsClicked);
            this.refactorMainMenu.ExtractMethodMenuItem.Click += new EventHandler(this.ExtractMethodClicked);
            this.refactorMainMenu.DelegateMenuItem.Click += new EventHandler(this.DelegateMethodsClicked);
            this.refactorMainMenu.ExtractLocalVariableMenuItem.Click += new EventHandler(this.ExtractLocalVariableClicked);
            this.refactorMainMenu.CodeGeneratorMenuItem.Click += new EventHandler(this.CodeGeneratorMenuItemClicked);
            this.refactorContextMenu = new RefactorMenu(false);
            this.refactorContextMenu.RenameMenuItem.Click += new EventHandler(this.RenameClicked);
            this.refactorContextMenu.OrganizeMenuItem.Click += new EventHandler(this.OrganizeImportsClicked);
            this.refactorContextMenu.TruncateMenuItem.Click += new EventHandler(this.TruncateImportsClicked);
            this.refactorContextMenu.DelegateMenuItem.Click += new EventHandler(this.DelegateMethodsClicked);
            this.refactorContextMenu.ExtractMethodMenuItem.Click += new EventHandler(this.ExtractMethodClicked);
            this.refactorContextMenu.ExtractLocalVariableMenuItem.Click += new EventHandler(this.ExtractLocalVariableClicked);
            this.refactorContextMenu.CodeGeneratorMenuItem.Click += new EventHandler(this.CodeGeneratorMenuItemClicked);
            this.surroundContextMenu = new SurroundMenu();
            editorMenu.Items.Insert(3, this.refactorContextMenu);
            editorMenu.Items.Insert(4, this.surroundContextMenu);
            mainMenu.Items.Insert(5, this.refactorMainMenu);
            ToolStripMenuItem searchMenu = PluginBase.MainForm.FindMenuItem("SearchMenu") as ToolStripMenuItem;
            this.viewReferencesItem = new ToolStripMenuItem(TextHelper.GetString("Label.FindAllReferences"), null, new EventHandler(this.FindAllReferencesClicked));
            this.editorReferencesItem = new ToolStripMenuItem(TextHelper.GetString("Label.FindAllReferences"), null, new EventHandler(this.FindAllReferencesClicked));
            PluginBase.MainForm.RegisterShortcutItem("SearchMenu.ViewReferences", this.viewReferencesItem);
            searchMenu.DropDownItems.Add(new ToolStripSeparator());
            searchMenu.DropDownItems.Add(this.viewReferencesItem);
            editorMenu.Items.Insert(7, this.editorReferencesItem);
        }

        /// <summary>
        /// Gets if the current documents language is haxe
        /// </summary>
        private Boolean LanguageIsHaxe()
        {
            ITabbedDocument document = PluginBase.MainForm.CurrentDocument;
            if (document != null && document.IsEditable)
            {
                return document.SciControl.ConfigurationLanguage == "haxe";
            }
            else return false;
        }

        /// <summary>
        /// Gets if the language is valid for refactoring
        /// </summary>
        private Boolean GetLanguageIsValid()
        {
            ITabbedDocument document = PluginBase.MainForm.CurrentDocument;
            if (document != null && document.IsEditable)
            {
                String lang = document.SciControl.ConfigurationLanguage;
                return (lang == "as2" || lang == "as3" || lang == "haxe" || lang == "loom"); // TODO look for /Snippets/Generators
            }
            else return false;
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
                ResolvedContext resolved = ASComplete.CurrentResolvedContext;
                Boolean isValid = this.GetLanguageIsValid() && resolved != null && resolved.Position >= 0;
                this.refactorMainMenu.DelegateMenuItem.Enabled = false;
                this.refactorContextMenu.DelegateMenuItem.Enabled = false;
                ASResult result = isValid ? resolved.Result : null;
                if (result != null && !result.IsNull())
                {
                    Boolean isVoid = result.Type.IsVoid();
                    Boolean isClass = !isVoid && result.IsStatic && result.Member == null;
                    Boolean isConstructor = !isVoid && !isClass && RefactoringHelper.CheckFlag(result.Member.Flags, FlagType.Constructor);
                    this.refactorContextMenu.RenameMenuItem.Enabled = !(isClass || isConstructor);
                    this.refactorMainMenu.RenameMenuItem.Enabled = !(isClass || isConstructor);
                    this.editorReferencesItem.Enabled = this.viewReferencesItem.Enabled = true;
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
                    Boolean truncate = (this.GetLanguageIsValid() && context.CurrentModel.Imports.Count > 0);
                    Boolean organize = (this.GetLanguageIsValid() && context.CurrentModel.Imports.Count > 1);
                    this.refactorContextMenu.OrganizeMenuItem.Enabled = organize;
                    this.refactorContextMenu.TruncateMenuItem.Enabled = truncate && !this.LanguageIsHaxe();
                    this.refactorMainMenu.OrganizeMenuItem.Enabled = organize;
                    this.refactorMainMenu.TruncateMenuItem.Enabled = truncate && !this.LanguageIsHaxe();
                }
                this.surroundContextMenu.Enabled = false;
                this.refactorMainMenu.SurroundMenu.Enabled = false;
                this.refactorContextMenu.ExtractMethodMenuItem.Enabled = false;
                this.refactorContextMenu.ExtractLocalVariableMenuItem.Enabled = false;
                this.refactorMainMenu.ExtractMethodMenuItem.Enabled = false;
                this.refactorMainMenu.ExtractLocalVariableMenuItem.Enabled = false;
                ITabbedDocument document = PluginBase.MainForm.CurrentDocument;
                if (document != null && document.IsEditable && this.GetLanguageIsValid() && document.SciControl.SelTextSize > 1)
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
                String name;
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
                            name = m.Name;
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
