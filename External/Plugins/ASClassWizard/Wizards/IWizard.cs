// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
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
