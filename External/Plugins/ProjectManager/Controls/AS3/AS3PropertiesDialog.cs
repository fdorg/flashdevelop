using ProjectManager.Projects.AS3;

namespace ProjectManager.Controls.AS3
{
    public partial class AS3PropertiesDialog : PropertiesDialog
    {
        // For Designer
        public AS3PropertiesDialog() => InitializeComponent();

        AS3Project project => (AS3Project)BaseProject;
    }
}