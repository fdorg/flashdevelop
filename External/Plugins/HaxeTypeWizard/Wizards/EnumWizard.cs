using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using ASClassWizard.Wizards;
using PluginCore;
using PluginCore.Controls;
using PluginCore.Localization;
using ProjectManager.Projects;

namespace HaxeTypeWizard.Wizards
{
    public partial class EnumWizard : SmartForm, IThemeHandler, IWizard
    {
        public const string REG_IDENTIFIER = "^[a-zA-Z_][a-zA-Z0-9_]*$";

        public EnumWizard()
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
            flowLayoutPanel1.BackColor = PluginBase.MainForm.GetThemeColor("Control.BackColor", SystemColors.Control);
            flowLayoutPanel9.BackColor = color;
            titleLabel.BackColor = color;
        }

        void LocalizeText()
        {
            typeLabel.Text = TextHelper.GetString("ASClassWizard.Wizard.Label.Name");
            packageLabel.Text = TextHelper.GetString("ASClassWizard.Wizard.Label.Package");
            packageBrowse.Text = TextHelper.GetString("ASClassWizard.Wizard.Button.Browse");
            okButton.Text = TextHelper.GetString("ASClassWizard.Wizard.Button.Ok");
            cancelButton.Text = TextHelper.GetString("ASClassWizard.Wizard.Button.Cancel");
            var label = TextHelper.GetString("Wizard.Label.NewEnum");
            titleLabel.Text = label;
            Text = label;
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

        public Project Project { get; set; }

        void ValidateType()
        {
            var errorMessage = string.Empty;
            var name = GetName().Trim();
            if (name.Length == 0)
                errorMessage = TextHelper.GetString("Wizard.Error.EmptyEnumName");
            else if (!Regex.Match(name, REG_IDENTIFIER, RegexOptions.Singleline).Success)
                errorMessage = TextHelper.GetString("Wizard.Error.InvalidEnumName");
            else if (char.IsLower(name[0]))
                errorMessage = TextHelper.GetString("Wizard.Error.LowercaseEnumName");

            var isError = errorMessage.Length > 0;
            okButton.Enabled = !isError;
            errorIcon.Visible = isError;
            errorLabel.Text = errorMessage;
        }

        #region EventHandlers

        void PackageBrowse_Click(object sender, EventArgs e)
        {
            using var browser = new PackageBrowser {Project = Project};
            Project.AbsoluteClasspaths.ForEach(browser.AddClassPath);
            if (browser.ShowDialog(this) != DialogResult.OK) return;
            if (browser.Package != null)
            {
                var classpath = Project.AbsoluteClasspaths.GetClosestParent(browser.Package);
                var package = Path.GetDirectoryName(ProjectPaths.GetRelativePath(classpath, Path.Combine(browser.Package, "foo")));
                if (package is null) return;
                Directory = browser.Package;
                packageBox.Text = package.Replace(Path.DirectorySeparatorChar, '.');
            }
            else
            {
                Directory = browser.Project.Directory;
                packageBox.Text = string.Empty;
            }
        }

        void ClassWizard_Load(object sender, EventArgs e)
        {
            classBox.Select();
            ValidateType();
        }

        void PackageBox_TextChanged(object sender, EventArgs e) => ValidateType();

        void ClassBox_TextChanged(object sender, EventArgs e) => ValidateType();

        #endregion

        #region user_options

        public string GetPackage() => packageBox.Text;
        public string GetName() => classBox.Text;
        public string GetExtends() => string.Empty;
        public List<string> GetInterfaces() => null;
        public bool IsPublic() => true;
        public bool IsDynamic() => false;
        public bool IsFinal() => false;
        public bool GetGenerateInheritedMethods() => false;
        public bool GetGenerateConstructor() => false;

        #endregion
    }
}