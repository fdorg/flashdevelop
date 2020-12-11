// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System.Collections.Generic;
using System.Windows.Forms;
using ProjectManager.Projects;

namespace ASClassWizard.Wizards
{
    public interface IWizard
    {
        Project Project { set; }
        string Directory { set; }
        string StartupClassName { set; }
        string StartupPackage { set; }
        string GetPackage();
        string GetName();
        string GetExtends();
        List<string> GetInterfaces();
        bool IsPublic();
        bool IsDynamic();
        bool IsFinal();
        bool GetGenerateInheritedMethods();
        bool GetGenerateConstructor();
        DialogResult ShowDialog();
    }
}