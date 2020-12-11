// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ProjectManager.Projects.AS3;

namespace LoomContext.Projects
{
    public partial class LoomPropertiesDialog : ProjectManager.Controls.PropertiesDialog
    {
        // For Designer
        public LoomPropertiesDialog() { InitializeComponent(); }

        LoomProject project { get { return (LoomProject)BaseProject; } }
    }
}

