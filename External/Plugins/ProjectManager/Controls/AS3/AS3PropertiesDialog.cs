// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using ProjectManager.Projects.AS3;

namespace ProjectManager.Controls.AS3
{
    public partial class AS3PropertiesDialog : ProjectManager.Controls.PropertiesDialog
    {
        // For Designer
        public AS3PropertiesDialog() { InitializeComponent(); }

        AS3Project project => (AS3Project)BaseProject;
    }
}

