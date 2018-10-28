using System;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using ASCompletion.Context;
using ASCompletion.Model;
using PluginCore;
using PluginCore.Controls;
using PluginCore.Localization;
using ProjectManager.Projects;

namespace ASClassWizard.Wizards
{
    public partial class AS3InterfaceWizard : SmartForm, IThemeHandler, IWizard
    {
        private Project project;
        public const string REG_IDENTIFIER_AS = "^[a-zA-Z_$][a-zA-Z0-9_$]*$";
        // $ is not a valid char in haxe class names
        public const string REG_IDENTIFIER_HAXE = "^[a-zA-Z_][a-zA-Z0-9_]*$";

        public AS3InterfaceWizard()
        {
            InitializeComponent();
            LocalizeText();
            CenterToParent();
            this.FormGuid = "E1D36E71-BD39-4C58-A436-F46D01EC0590";
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
            this.baseLabel.Text = TextHelper.GetString("Wizard.Label.ExtendsInterface");
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
                if (project.Language == "as2")
                {
                    var label = TextHelper.GetString("Wizard.Label.NewAs2Interface");
                    this.titleLabel.Text = label;
                    this.Text = label;
                }
                if (project.Language == "haxe")
                {
                    var label = TextHelper.GetString("Wizard.Label.NewHaxeInterface");
                    this.titleLabel.Text = label;
                    this.Text = label;
                }
                else
                {
                    var label = TextHelper.GetString("Wizard.Label.NewAs3Interface");
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
                errorMessage = TextHelper.GetString("Wizard.Error.EmptyInterfaceName");
            else if (!Regex.Match(GetName(), regex, RegexOptions.Singleline).Success)
                errorMessage = TextHelper.GetString("Wizard.Error.InvalidInterfaceName");
            else if (project.Language == "haxe" && char.IsLower(GetName()[0]))
                errorMessage = TextHelper.GetString("Wizard.Error.LowercaseInterfaceName");

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
            using (var browser = new ClassBrowser())
            {
                var context = ASContext.GetLanguageContext(PluginBase.CurrentProject.Language);
                try
                {
                    browser.ClassList = context.GetAllProjectClasses();
                }
                catch { }
                //browser.ExcludeFlag = FlagType.Interface;
                browser.IncludeFlag = FlagType.Interface;
                if (browser.ShowDialog(this) == DialogResult.OK)
                {
                    this.baseBox.Text = browser.SelectedClass;
                }
                this.okButton.Focus();
            }
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
            ValidateClass();
        }

        #endregion

        #region user_options

        public string GetPackage() => packageBox.Text;

        public string GetName() => classBox.Text;

        public string GetExtends() => baseBox.Text;

        #endregion

    }
}
