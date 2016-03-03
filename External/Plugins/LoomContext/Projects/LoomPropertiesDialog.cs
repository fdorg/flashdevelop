namespace LoomContext.Projects
{
    public partial class LoomPropertiesDialog : ProjectManager.Controls.PropertiesDialog
    {
        // For Designer
        public LoomPropertiesDialog() { InitializeComponent(); }

        LoomProject project { get { return (LoomProject)BaseProject; } }
    }
}

