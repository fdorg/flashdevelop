using System;
using System.Collections.Generic;
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
        Project project;
        public const string REG_IDENTIFIER_AS = "^[a-zA-Z_$][a-zA-Z0-9_$]*$";
        // $ is not a valid char in haxe class names
        public const string REG_IDENTIFIER_HAXE = "^[a-zA-Z_][a-zA-Z0-9_]*$";

        public AS3InterfaceWizard()
        {
            InitializeComponent();
            LocalizeText();
            CenterToParent();
            FormGuid = "E1D36E71-BD39-4C58-A436-F46D01EC0590";
            Font = PluginBase.Settings.DefaultFont;
            errorIcon.Image = PluginBase.MainForm.FindImage("197");
        }

        public void AfterTheming()
        {
            var color = PluginBase.MainForm.GetThemeColor("ListBox.BackColor", SystemColors.Window);
            var color1 = PluginBase.MainForm.GetThemeColor("Control.BackColor", SystemColors.Control);
            flowLayoutPanel1.BackColor = color1;
            flowLayoutPanel9.BackColor = color;
            titleLabel.BackColor = color;
        }

        void LocalizeText()
        {
            typeLabel.Text = TextHelper.GetString("Wizard.Label.Name");
            baseLabel.Text = TextHelper.GetString("Wizard.Label.ExtendsInterface");
            packageLabel.Text = TextHelper.GetString("Wizard.Label.Package");
            packageBrowse.Text = TextHelper.GetString("Wizard.Button.Browse");
            baseBrowse.Text = TextHelper.GetString("Wizard.Button.Browse");
            okButton.Text = TextHelper.GetString("Wizard.Button.Ok");
            cancelButton.Text = TextHelper.GetString("Wizard.Button.Cancel");
        }

        public string StartupPackage
        {
            set => packageBox.Text = value;
        }

        public string StartupClassName
        {
            set => classBox.Text = value;
        }

        public string Directory { get; set; }

        public Project Project
        {
            get => project;
            set 
            { 
                project = value;
                if (project.Language == "as2")
                {
                    var label = TextHelper.GetString("Wizard.Label.NewAs2Interface");
                    titleLabel.Text = label;
                    Text = label;
                }
                else if (project.Language == "haxe")
                {
                    var label = TextHelper.GetString("Wizard.Label.NewHaxeInterface");
                    titleLabel.Text = label;
                    Text = label;
                }
                else
                {
                    var label = TextHelper.GetString("Wizard.Label.NewAs3Interface");
                    titleLabel.Text = label;
                    Text = label;
                }
            }
        }

        void ValidateClass()
        {
            var errorMessage = "";
            var regex = (project.Language == "haxe") ? REG_IDENTIFIER_HAXE : REG_IDENTIFIER_AS; 
            if (GetName().Length == 0)
                errorMessage = TextHelper.GetString("Wizard.Error.EmptyInterfaceName");
            else if (!Regex.Match(GetName(), regex, RegexOptions.Singleline).Success)
                errorMessage = TextHelper.GetString("Wizard.Error.InvalidInterfaceName");
            else if (project.Language == "haxe" && char.IsLower(GetName()[0]))
                errorMessage = TextHelper.GetString("Wizard.Error.LowercaseInterfaceName");

            if (errorMessage.Length != 0)
            {
                okButton.Enabled = false;
                errorIcon.Visible = true;
            }
            else
            {
                okButton.Enabled = true;
                errorIcon.Visible = false;
            }
            errorLabel.Text = errorMessage;
        }

        #region EventHandlers

        /// <summary>
        /// Browse project packages
        /// </summary>
        void packageBrowse_Click(object sender, EventArgs e)
        {
            using PackageBrowser browser = new PackageBrowser();
            browser.Project = Project;

            foreach (string item in Project.AbsoluteClasspaths)
                browser.AddClassPath(item);

            if (browser.ShowDialog(this) == DialogResult.OK)
            {
                if (browser.Package != null)
                {
                    string classpath = Project.AbsoluteClasspaths.GetClosestParent(browser.Package);
                    string package = Path.GetDirectoryName(ProjectPaths.GetRelativePath(classpath, Path.Combine(browser.Package, "foo")));
                    if (package != null)
                    {
                        Directory = browser.Package;
                        package = package.Replace(Path.DirectorySeparatorChar, '.');
                        packageBox.Text = package;
                    }
                }
                else
                {
                    Directory = browser.Project.Directory;
                    packageBox.Text = "";
                }
            }
        }

        void AS3ClassWizard_Load(object sender, EventArgs e)
        {
            classBox.Select();
            ValidateClass();
        }

        void baseBrowse_Click(object sender, EventArgs e)
        {
            using var browser = new ClassBrowser();
            var context = ASContext.GetLanguageContext(PluginBase.CurrentProject.Language);
            try
            {
                browser.ClassList = context.GetAllProjectClasses();
            }
            catch { }
            browser.IncludeFlag = FlagType.Interface;
            if (browser.ShowDialog(this) == DialogResult.OK)
            {
                baseBox.Text = browser.SelectedClass;
            }
            okButton.Focus();
        }

        void packageBox_TextChanged(object sender, EventArgs e)
        {
            ValidateClass();
        }

        void classBox_TextChanged(object sender, EventArgs e)
        {
            ValidateClass();
        }

        void baseBox_TextChanged(object sender, EventArgs e)
        {
            ValidateClass();
        }

        #endregion

        #region user_options

        public string GetPackage() => packageBox.Text;

        public string GetName() => classBox.Text;

        public string GetExtends() => baseBox.Text;

        #endregion

        public List<string> GetInterfaces() => null;

        public bool IsPublic() => true;

        public bool IsDynamic() => false;

        public bool IsFinal() => false;

        public bool GetGenerateInheritedMethods() => false;

        public bool GetGenerateConstructor() => false;
    }
}
