
#region Imports
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Text.RegularExpressions;

using PluginCore;
using PluginCore.Localization;
using ProjectManager.Projects;

using ASClassWizard.Wizards;
using ASClassWizard.Resources;

using ASCompletion.Context;
using ASCompletion.Model;

using AS3Context;
using AS2Context;
using System.Reflection;
using System.Diagnostics;
#endregion


namespace ASClassWizard.Wizards
{
    public partial class AS3ClassWizard : Form
    {
        private string directoryPath;
        private Project project;
        public const string REG_IDENTIFIER_AS = "^[a-zA-Z_$][a-zA-Z0-9_$]*$";
        // $ is not a valid char in haxe class names
        public const string REG_IDENTIFIER_HAXE = "^[a-zA-Z_][a-zA-Z0-9_]*$";

        public AS3ClassWizard()
        {
            InitializeComponent();
            LocalizeText();
            CenterToParent();
            this.Font = PluginBase.Settings.DefaultFont;
            this.errorIcon.Image = PluginMain.MainForm.FindImage("197");
        }

        private void LocalizeText()
        {
            this.classLabel.Text = TextHelper.GetString("Wizard.Label.Name");
            this.accessLabel.Text = TextHelper.GetString("Wizard.Label.Modifiers");
            this.baseLabel.Text = TextHelper.GetString("Wizard.Label.SuperClass");
            this.implementLabel.Text = TextHelper.GetString("Wizard.Label.Interfaces");
            this.generationLabel.Text = TextHelper.GetString("Wizard.Label.CodeGeneration");
            this.implementBrowse.Text = TextHelper.GetString("Wizard.Button.Add");
            this.implementRemove.Text = TextHelper.GetString("Wizard.Button.Remove");
            this.constructorCheck.Text = TextHelper.GetString("Wizard.Label.GenerateConstructor");
            this.superCheck.Text = TextHelper.GetString("Wizard.Label.GenerateInherited");
            this.packageLabel.Text = TextHelper.GetString("Wizard.Label.Package");
            this.packageBrowse.Text = TextHelper.GetString("Wizard.Button.Browse");
            this.baseBrowse.Text = TextHelper.GetString("Wizard.Button.Browse");
            this.okButton.Text = TextHelper.GetString("Wizard.Button.Ok");
            this.cancelButton.Text = TextHelper.GetString("Wizard.Button.Cancel");
        }

        public String StartupPackage
        {
            set { packageBox.Text = value; }
        }

		public String StartupClassName
		{
			set { classBox.Text = value; }
		}
		
        public string Directory
        {
            get { return this.directoryPath; }
            set { this.directoryPath = value; }
        }

        public Project Project
        {
            get { return project; }
            set 
            { 
                this.project = value;
                this.internalRadio.Text = "internal";
                if (project.Language == "as2")
                {
                    this.publicRadio.Enabled = false;
                    this.internalRadio.Enabled = false;
                    this.finalCheck.Enabled = false;
                    this.titleLabel.Text = TextHelper.GetString("Wizard.Label.NewAs2Class");
                    this.Text = TextHelper.GetString("Wizard.Label.NewAs2Class");
                }
                if (project.Language == "haxe")
                {
                    this.internalRadio.Text = "private";
                    this.titleLabel.Text = TextHelper.GetString("Wizard.Label.NewHaxeClass");
                    this.Text = TextHelper.GetString("Wizard.Label.NewHaxeClass");
                }
                else
                {
                    this.titleLabel.Text = TextHelper.GetString("Wizard.Label.NewAs3Class");
                    this.Text = TextHelper.GetString("Wizard.Label.NewAs3Class");
                }
            }
        }

        private void ValidateClass()
        {
            string errorMessage = "";
            string regex = (project.Language == "haxe") ? REG_IDENTIFIER_HAXE : REG_IDENTIFIER_AS; 
            if (getClassName() == "")
                errorMessage = TextHelper.GetString("Wizard.Error.EmptyClassName");
            else if (!Regex.Match(getClassName(), regex, RegexOptions.Singleline).Success)
                errorMessage = TextHelper.GetString("Wizard.Error.InvalidClassName");
            else if (project.Language == "haxe" && Char.IsLower(getClassName()[0]))
                errorMessage = TextHelper.GetString("Wizard.Error.LowercaseClassName");

            if (errorMessage != "")
            {
                okButton.Enabled = false;
                errorIcon.Visible = true;
            }
            else
            {
                okButton.Enabled = true;
                errorIcon.Visible = false;
            }
            this.errorLabel.Text = errorMessage;
        }

        #region EventHandlers

        /// <summary>
        /// Browse project packages
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void packageBrowse_Click(object sender, EventArgs e)
        {

            PackageBrowser browser = new PackageBrowser();
            browser.Project = this.Project;

            foreach (string item in Project.AbsoluteClasspaths)
                browser.AddClassPath(item);

            if (browser.ShowDialog(this) == DialogResult.OK)
            {
                if (browser.Package != null)
                {
                    string classpath = this.Project.AbsoluteClasspaths.GetClosestParent(browser.Package);
                    string package = Path.GetDirectoryName(ProjectPaths.GetRelativePath(classpath, Path.Combine(browser.Package, "foo")));
                    if (package != null)
                    {
                        directoryPath = browser.Package;
                        package = package.Replace(Path.DirectorySeparatorChar, '.');
                        this.packageBox.Text = package;
                    }
                }
                else
                {
                    this.directoryPath = browser.Project.Directory;
                    this.packageBox.Text = "";
                }
            }
        }

        private void AS3ClassWizard_Load(object sender, EventArgs e)
        {
            this.classBox.Select();
            this.ValidateClass();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void baseBrowse_Click(object sender, EventArgs e)
        {
            ClassBrowser browser = new ClassBrowser();
            IASContext context   = ASContext.GetLanguageContext(PluginBase.CurrentProject.Language);
            try
            {
                browser.ClassList = context.GetAllProjectClasses();
            }
            catch { }
            browser.ExcludeFlag  = FlagType.Interface;
            browser.IncludeFlag  = FlagType.Class;
            if (browser.ShowDialog(this) == DialogResult.OK)
            {
                this.baseBox.Text = browser.SelectedClass;
            }
            this.okButton.Focus();
        }

        /// <summary>
        /// Added interface
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void implementBrowse_Click(object sender, EventArgs e)
        {
            ClassBrowser browser = new ClassBrowser();
            MemberList known = null;
            browser.IncludeFlag = FlagType.Interface;
            IASContext context = ASContext.GetLanguageContext(PluginBase.CurrentProject.Language);
            try
            {
                known = context.GetAllProjectClasses();
                known.Merge(ASContext.Context.GetVisibleExternalElements());
            }
            catch (Exception error)
            {
                Debug.WriteLine(error.StackTrace);
            }
            browser.ClassList = known;
            if (browser.ShowDialog(this) == DialogResult.OK)
            {
                if (browser.SelectedClass != null)
                {
                    foreach (string item in this.implementList.Items)
                    {
                        if (item == browser.SelectedClass) return;
                    }
                    this.implementList.Items.Add(browser.SelectedClass);
                }
            }
            this.implementRemove.Enabled = this.implementList.Items.Count > 0;
            this.implementList.SelectedIndex = this.implementList.Items.Count - 1;
            this.superCheck.Enabled = this.implementList.Items.Count > 0;
            ValidateClass();
        }

        /// <summary>
        /// Remove interface
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void interfaceRemove_Click(object sender, EventArgs e)
        {
            if (this.implementList.SelectedItem != null)
            {
                this.implementList.Items.Remove(this.implementList.SelectedItem);
            }
            if (this.implementList.Items.Count > 0)
            {
                this.implementList.SelectedIndex = this.implementList.Items.Count - 1;
            }
            this.implementRemove.Enabled = this.implementList.Items.Count > 0;
            this.superCheck.Enabled = this.implementList.Items.Count > 0;
            ValidateClass();
        }

        private void packageBox_TextChanged(object sender, EventArgs e)
        {
            ValidateClass();
        }

        private void classBox_TextChanged(object sender, EventArgs e)
        {
            ValidateClass();
        }

        private void baseBox_TextChanged(object sender, EventArgs e)
        {
            this.constructorCheck.Enabled = this.baseBox.Text != "";
            ValidateClass();
        }

        #endregion

        public static Image GetResource( string resourceID )
        {
            resourceID = "ASClassWizard." + resourceID;
            Assembly assembly = Assembly.GetExecutingAssembly();
            Image image = new Bitmap(assembly.GetManifestResourceStream(resourceID));
            return image;
        }

        #region user_options

        public string getPackage()
        {
            return this.packageBox.Text;
        }

        public string getClassName()
        {
            return this.classBox.Text;
        }

        public bool isDynamic()
        {
            return this.dynamicCheck.Checked;
        }

        public bool isFinal()
        {
            return this.finalCheck.Checked;
        }

        public bool isPublic()
        {
            return this.publicRadio.Checked;
        }

        public string getSuperClass()
        {
            return this.baseBox.Text;
        }

        public List<string> getInterfaces()
        {
            List<string> _interfaces = new List<string>(this.implementList.Items.Count);
            foreach (string item in this.implementList.Items)
            {
                _interfaces.Add(item);
            }
            return _interfaces;
        }

        public bool hasInterfaces()
        {
            return this.implementList.Items.Count > 0;
        }

        public bool getGenerateConstructor()
        {
            return this.constructorCheck.Checked;
        }

        public bool getGenerateInheritedMethods()
        {
            return this.superCheck.Checked;
        }

        #endregion


    }
}
