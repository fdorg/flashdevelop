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
	    private const String PluginName = "CodeRefactor";
	    private const String PluginGuid = "5c0d3740-a6f2-11de-8a39-0800200c9a66";
	    private const String PluginHelp = "www.flashdevelop.org/community/";
	    private String _pluginDesc = "Adds refactoring capabilities to FlashDevelop.";
	    private const String PluginAuth = "FlashDevelop Team";
	    private ToolStripMenuItem _editorReferencesItem;
        private ToolStripMenuItem _viewReferencesItem;
        private SurroundMenu _surroundContextMenu;
        private RefactorMenu _refactorContextMenu;
        private RefactorMenu _refactorMainMenu;
        private Settings _settingObject;
        private String _settingFilename;

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
			get { return PluginName; }
		}

        /// <summary>
        /// GUID of the plugin
        /// </summary>
        public String Guid
		{
			get { return PluginGuid; }
		}

        /// <summary>
        /// Author of the plugin
        /// </summary> 
        public String Author
		{
			get { return PluginAuth; }
		}

        /// <summary>
        /// Description of the plugin
        /// </summary> 
        public String Description
		{
			get { return _pluginDesc; }
		}

        /// <summary>
        /// Web address for help
        /// </summary> 
        public String Help
		{
			get { return PluginHelp; }
		}

        /// <summary>
        /// Object that contains the settings
        /// </summary>
        [Browsable(false)]
        public Object Settings
        {
            get { return _settingObject; }
        }
		
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
        }

		/// <summary>
		/// Disposes the plugin
		/// </summary>
		public void Dispose()
		{
            SaveSettings();
		}
		
		/// <summary>
		/// Handles the incoming events
		/// </summary>
		public void HandleEvent(Object sender, NotifyEvent e, HandlingPriority prority)
		{
            switch (e.Type)
            {
                case EventType.FileSwitch:
                    GenerateSurroundMenuItems();
                    break;

                case EventType.UIStarted:
                    // Expose plugin's refactor main menu & context menu...
                    EventManager.DispatchEvent(this, new DataEvent(EventType.Command, "CodeRefactor.Menu", _refactorMainMenu));
                    EventManager.DispatchEvent(this, new DataEvent(EventType.Command, "CodeRefactor.ContextMenu", _refactorContextMenu));
                    // Watch resolved context for menu item updating...
                    ASComplete.OnResolvedContextChanged += OnResolvedContextChanged;
                    UpdateMenuItems();
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
            if (!Directory.Exists(dataPath))
                Directory.CreateDirectory(dataPath);

            _settingFilename = Path.Combine(dataPath, "Settings.fdb");
            _pluginDesc = TextHelper.GetString("Info.Description");
        }

        /// <summary>
        /// Creates the required menu items
        /// </summary>
        private void CreateMenuItems()
        {
            MenuStrip mainMenu = PluginBase.MainForm.MenuStrip;
            ContextMenuStrip editorMenu = PluginBase.MainForm.EditorMenu;
            
            _refactorMainMenu = new RefactorMenu(true);
            _refactorMainMenu.RenameMenuItem.Click += RenameClicked;
            _refactorMainMenu.OrganizeMenuItem.Click += OrganizeImportsClicked;
            _refactorMainMenu.TruncateMenuItem.Click += TruncateImportsClicked;
            _refactorMainMenu.ExtractMethodMenuItem.Click += ExtractMethodClicked;
            _refactorMainMenu.DelegateMenuItem.Click += DelegateMethodsClicked;
            _refactorMainMenu.ExtractLocalVariableMenuItem.Click += ExtractLocalVariableClicked;
            _refactorMainMenu.CodeGeneratorMenuItem.Click += CodeGeneratorMenuItemClicked;
            
            _refactorContextMenu = new RefactorMenu(false);
            _refactorContextMenu.RenameMenuItem.Click += RenameClicked;
            _refactorContextMenu.OrganizeMenuItem.Click += OrganizeImportsClicked;
            _refactorContextMenu.TruncateMenuItem.Click += TruncateImportsClicked;
            _refactorContextMenu.DelegateMenuItem.Click += DelegateMethodsClicked;
            _refactorContextMenu.ExtractMethodMenuItem.Click += ExtractMethodClicked;
            _refactorContextMenu.ExtractLocalVariableMenuItem.Click += ExtractLocalVariableClicked;
            _refactorContextMenu.CodeGeneratorMenuItem.Click += CodeGeneratorMenuItemClicked;
           
            _surroundContextMenu = new SurroundMenu();
            editorMenu.Items.Insert(3, _refactorContextMenu);
            editorMenu.Items.Insert(4, _surroundContextMenu);
            mainMenu.Items.Insert(5, _refactorMainMenu);
           
            ToolStripMenuItem searchMenu = PluginBase.MainForm.FindMenuItem("SearchMenu") as ToolStripMenuItem;
            _viewReferencesItem = new ToolStripMenuItem(TextHelper.GetString("Label.FindAllReferences"), null, FindAllReferencesClicked);
            _editorReferencesItem = new ToolStripMenuItem(TextHelper.GetString("Label.FindAllReferences"), null, FindAllReferencesClicked);
            PluginBase.MainForm.RegisterShortcutItem("SearchMenu.ViewReferences", _viewReferencesItem);
           
            searchMenu.DropDownItems.Add(new ToolStripSeparator());
            searchMenu.DropDownItems.Add(_viewReferencesItem);
            editorMenu.Items.Insert(7, _editorReferencesItem);
        }

        /// <summary>
        /// Gets if the current documents language is haxe
        /// </summary>
        private static Boolean LanguageIsHaxe()
        {
            ITabbedDocument document = PluginBase.MainForm.CurrentDocument;
            if (document != null && document.IsEditable)
                return document.SciControl.ConfigurationLanguage == "haxe";
                
            return false;
        }

        /// <summary>
        /// Gets if the language is valid for refactoring
        /// </summary>
        private static Boolean GetLanguageIsValid()
        {
            ITabbedDocument document = PluginBase.MainForm.CurrentDocument;
            if (document != null && document.IsEditable)
            {
                String lang = document.SciControl.ConfigurationLanguage;
                return (lang == "as2" || lang == "as3" || lang == "haxe" || lang == "loom"); // TODO look for /Snippets/Generators
            }
                
            return false;
        }

        /// <summary>
        /// Cursor position changed and word at this position was resolved
        /// </summary>
        private void OnResolvedContextChanged(ResolvedContext resolved)
        {
            UpdateMenuItems();
        }

        /// <summary>
        /// Updates the state of the menu items
        /// </summary>
        private void UpdateMenuItems()
        {
            try
            {
                ResolvedContext resolved = ASComplete.CurrentResolvedContext;
                Boolean isValid = GetLanguageIsValid() && resolved != null && resolved.Position >= 0;
                _refactorMainMenu.DelegateMenuItem.Enabled = false;
                _refactorContextMenu.DelegateMenuItem.Enabled = false;
                ASResult result = isValid ? resolved.Result : null;
                if (result != null && !result.IsNull())
                {
                    _refactorContextMenu.RenameMenuItem.Enabled = true;
                    _refactorMainMenu.RenameMenuItem.Enabled = true;

                    _editorReferencesItem.Enabled = true;
                    _viewReferencesItem.Enabled = true;
                    if (result.Member != null && result.Type != null && result.InClass != null && result.InFile != null)
                    {
                        FlagType flags = result.Member.Flags;
                        if ((flags & FlagType.Variable) > 0 && (flags & FlagType.LocalVar) == 0 && (flags & FlagType.ParameterVar) == 0)
                        {
                            _refactorContextMenu.DelegateMenuItem.Enabled = true;
                            _refactorMainMenu.DelegateMenuItem.Enabled = true;
                        }
                    }
                }
                else
                {
                    _refactorMainMenu.RenameMenuItem.Enabled = false;
                    _refactorContextMenu.RenameMenuItem.Enabled = false;
                    _editorReferencesItem.Enabled = false;
                    _viewReferencesItem.Enabled = false;
                }

                IASContext context = ASContext.Context;
                if (context != null && context.CurrentModel != null)
                {
                    Boolean truncate = (GetLanguageIsValid() && context.CurrentModel.Imports.Count > 0);
                    Boolean organize = (GetLanguageIsValid() && context.CurrentModel.Imports.Count > 1);
                    _refactorContextMenu.OrganizeMenuItem.Enabled = organize;
                    _refactorContextMenu.TruncateMenuItem.Enabled = truncate && !LanguageIsHaxe();
                    _refactorMainMenu.OrganizeMenuItem.Enabled = organize;
                    _refactorMainMenu.TruncateMenuItem.Enabled = truncate && !LanguageIsHaxe();
                }

                _surroundContextMenu.Enabled = false;
                _refactorMainMenu.SurroundMenu.Enabled = false;
                _refactorContextMenu.ExtractMethodMenuItem.Enabled = false;
                _refactorContextMenu.ExtractLocalVariableMenuItem.Enabled = false;
                _refactorMainMenu.ExtractMethodMenuItem.Enabled = false;
                _refactorMainMenu.ExtractLocalVariableMenuItem.Enabled = false;
                
                ITabbedDocument document = PluginBase.MainForm.CurrentDocument;
                if (document != null && document.IsEditable && GetLanguageIsValid() && document.SciControl.SelTextSize > 1)
                {
                    Int32 selEnd = document.SciControl.SelectionEnd;
                    Int32 selStart = document.SciControl.SelectionStart;
                    if (!document.SciControl.PositionIsOnComment(selEnd) || !document.SciControl.PositionIsOnComment(selStart))
                    {
                        _surroundContextMenu.Enabled = true;
                        _refactorMainMenu.SurroundMenu.Enabled = true;
                        _refactorContextMenu.ExtractMethodMenuItem.Enabled = true;
                        _refactorMainMenu.ExtractMethodMenuItem.Enabled = true;
                        _refactorContextMenu.ExtractLocalVariableMenuItem.Enabled = true;
                        _refactorMainMenu.ExtractLocalVariableMenuItem.Enabled = true;
                    }
                }
                _refactorContextMenu.CodeGeneratorMenuItem.Enabled = isValid;
                _refactorMainMenu.CodeGeneratorMenuItem.Enabled = isValid;
            }
            catch {}
        }

        /// <summary>
        /// Generate surround main menu and context menu items
        /// </summary>
        private void GenerateSurroundMenuItems()
        {
            ITabbedDocument document = PluginBase.MainForm.CurrentDocument;
            if (document != null && document.IsEditable && GetLanguageIsValid())
            {
                _surroundContextMenu.GenerateSnippets(document.SciControl);
                foreach (ToolStripMenuItem item in _surroundContextMenu.DropDownItems)
                    item.Click += SurroundWithClicked;
                
                foreach (ToolStripMenuItem item in _refactorMainMenu.SurroundMenu.DropDownItems)
                    item.Click -= SurroundWithClicked;
                
                _refactorMainMenu.SurroundMenu.GenerateSnippets(document.SciControl);
                foreach (ToolStripMenuItem item in _refactorMainMenu.SurroundMenu.DropDownItems)
                    item.Click += SurroundWithClicked;
            }
            else
            {
                _surroundContextMenu.DropDownItems.Clear();
                _refactorMainMenu.SurroundMenu.DropDownItems.Clear();
                _refactorMainMenu.SurroundMenu.DropDownItems.Add("");
                _surroundContextMenu.DropDownItems.Add("");
            }
        }

        /// <summary>
        /// Invoked when the user selects the "Rename" command
        /// </summary>
        private static void RenameClicked(Object sender, EventArgs e)
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
        private static void SurroundWithClicked(Object sender, EventArgs e)
        {
            if (!(sender is ToolStripItem))
                return;
            
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
        public void FindAllReferencesClicked(Object sender, EventArgs e)
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
                OrganizeImports command = new OrganizeImports {SeparatePackages = _settingObject.SeparatePackages};
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
                OrganizeImports command = new OrganizeImports
                {
                    SeparatePackages = _settingObject.SeparatePackages,
                    TruncateImports = true
                };
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
        private static void DelegateMethodsClicked(Object sender, EventArgs e)
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
                            String name = m.Name;
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
        private static void ExtractMethodClicked(Object sender, EventArgs e)
        {
            try
            {
                String suggestion = "newMethod";
                String label = TextHelper.GetString("Label.NewName");
                String title = TextHelper.GetString("Title.ExtractMethodDialog");
                LineEntryDialog askName = new LineEntryDialog(title, label, suggestion);
                DialogResult choice = askName.ShowDialog();
                if (choice == DialogResult.OK && askName.Line.Trim().Length > 0 && askName.Line.Trim() != suggestion)
                    suggestion = askName.Line.Trim();

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
        private static void ExtractLocalVariableClicked(Object sender, EventArgs e)
        {
            try
            {
                String suggestion = "newVar";
                String label = TextHelper.GetString("Label.NewName");
                String title = TextHelper.GetString("Title.ExtractLocalVariableDialog");
                LineEntryDialog askName = new LineEntryDialog(title, label, suggestion);
                DialogResult choice = askName.ShowDialog();
                if (choice == DialogResult.OK && askName.Line.Trim().Length > 0 && askName.Line.Trim() != suggestion)
                    suggestion = askName.Line.Trim();

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
            _settingObject = new Settings();
            if (!File.Exists(_settingFilename))
                SaveSettings();
            else
            {
                Object obj = ObjectSerializer.Deserialize(_settingFilename, _settingObject);
                _settingObject = (Settings)obj;
            }
        }

        /// <summary>
        /// Saves the plugin settings
        /// </summary>
        public void SaveSettings()
        {
            ObjectSerializer.Serialize(_settingFilename, _settingObject);
        }

		#endregion

    }
	
}