using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using PluginCore;
using PluginCore.Localization;
using ProjectManager.Projects;
using ASCompletion.Context;
using ASCompletion.Model;
using System.Diagnostics;
using PluginCore.Controls;

namespace ASClassWizard.Wizards
{
    public partial class AS3ClassWizard : SmartForm, IThemeHandler, IWizard
    {
        private Project project;
        public const string REG_IDENTIFIER_AS = "^[a-zA-Z_$][a-zA-Z0-9_$]*$";
        // $ is not a valid char in haxe class names
        public const string REG_IDENTIFIER_HAXE = "^[a-zA-Z_][a-zA-Z0-9_]*$";

        public AS3ClassWizard()
        {
            InitializeComponent();
            LocalizeText();
            CenterToParent();
            this.FormGuid = "eb444130-58ea-47bd-9751-ad78a59c711f";
            this.Font = PluginBase.Settings.DefaultFont;
            this.errorIcon.Image = PluginBase.MainForm.FindImage("197");
        }

        public void AfterTheming()
        {
            Color color = PluginBase.MainForm.GetThemeColor("ListBox.BackColor", SystemColors.Window);
            Color color1 = PluginBase.MainForm.GetThemeColor("Control.BackColor", SystemColors.Control);
            this.flowLayoutPanel1.BackColor = color1;
            this.flowLayoutPanel9.BackColor = color;
            this.titleLabel.BackColor = color;
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

        public string Directory { get; set; }

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
                    var label = TextHelper.GetString("Wizard.Label.NewAs2Class");
                    this.titleLabel.Text = label;
                    this.Text = label;
                }
                if (project.Language == "haxe")
                {
                    this.internalRadio.Text = "private";
                    var label = TextHelper.GetString("Wizard.Label.NewHaxeClass");
                    this.titleLabel.Text = label;
                    this.Text = label;
                }
                else
                {
                    var label = TextHelper.GetString("Wizard.Label.NewAs3Class");
                    this.titleLabel.Text = label;
                    this.Text = label;
                }
            }
        }

        private void ValidateClass()
        {
            string errorMessage = "";
            string regex = (project.Language == "haxe") ? REG_IDENTIFIER_HAXE : REG_IDENTIFIER_AS; 
            if (GetName() == "")
                errorMessage = TextHelper.GetString("Wizard.Error.EmptyClassName");
            else if (!Regex.Match(GetName(), regex, RegexOptions.Singleline).Success)
                errorMessage = TextHelper.GetString("Wizard.Error.InvalidClassName");
            else if (project.Language == "haxe" && Char.IsLower(GetName()[0]))
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
        private void packageBrowse_Click(object sender, EventArgs e)
        {

            using (PackageBrowser browser = new PackageBrowser())
            {
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
                            Directory = browser.Package;
                            package = package.Replace(Path.DirectorySeparatorChar, '.');
                            this.packageBox.Text = package;
                        }
                    }
                    else
                    {
                        this.Directory = browser.Project.Directory;
                        this.packageBox.Text = "";
                    }
                }
            }
        }

        private void AS3ClassWizard_Load(object sender, EventArgs e)
        {
            this.classBox.Select();
            this.ValidateClass();
        }

        private void baseBrowse_Click(object sender, EventArgs e)
        {
            using (ClassBrowser browser = new ClassBrowser())
            {
                IASContext context = ASContext.GetLanguageContext(PluginBase.CurrentProject.Language);
                try
                {
                    browser.ClassList = context.GetAllProjectClasses();
                }
                catch { }
                browser.ExcludeFlag = FlagType.Interface;
                browser.IncludeFlag = FlagType.Class;
                if (browser.ShowDialog(this) == DialogResult.OK)
                {
                    this.baseBox.Text = browser.SelectedClass;
                }
                this.okButton.Focus();
            }
        }

        /// <summary>
        /// Added interface
        /// </summary>
        private void implementBrowse_Click(object sender, EventArgs e)
        {
            using (var browser = new ClassBrowser())
            {
                MemberList known = null;
                browser.IncludeFlag = FlagType.Interface;
                var context = ASContext.GetLanguageContext(PluginBase.CurrentProject.Language);
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
        }

        /// <summary>
        /// Remove interface
        /// </summary>
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

        #region user_options

        public string GetPackage() => this.packageBox.Text;

        public string GetName() => this.classBox.Text;

        public bool isDynamic() => this.dynamicCheck.Checked;

        public bool isFinal() => this.finalCheck.Checked;

        public bool isPublic() => this.publicRadio.Checked;

        public string GetExtends() => this.baseBox.Text;

        public List<string> getInterfaces()
        {
            List<string> _interfaces = new List<string>(this.implementList.Items.Count);
            foreach (string item in this.implementList.Items)
            {
                _interfaces.Add(item);
            }
            return _interfaces;
        }

        public bool hasInterfaces() => this.implementList.Items.Count > 0;

        public bool getGenerateConstructor() => this.constructorCheck.Checked;

        public bool getGenerateInheritedMethods() => this.superCheck.Checked;

        #endregion

    }
}
