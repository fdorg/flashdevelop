using System.Windows.Forms;
using ProjectManager.Projects;

namespace ASClassWizard.Wizards
{
    interface IWizard
    {
        Project Project { set; }
        string Directory { set; }
        string StartupClassName { set; }
        string StartupPackage { set; }
        string GetPackage();
        string GetName();
        DialogResult ShowDialog();
    }
}
