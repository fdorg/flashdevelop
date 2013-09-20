using System.Windows.Forms;

namespace CodeRefactor.Controls
{
    public partial class RenameFileDialog : Form
    {
        public RenameFileDialog()
        {
            InitializeComponent();
            OKButton.DialogResult = DialogResult.OK;
        }
    }
}