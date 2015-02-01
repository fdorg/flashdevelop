using System.Windows.Forms;

namespace PluginCore.Controls
{
    /// <summary>
    /// A form that, unless forced directly by code, does not become the foreground window when shown or clicked
    /// </summary>
    public class InactiveForm : Form
    {

        private const int WS_EX_NOACTIVATE = 0x8000000;

        protected override bool ShowWithoutActivation
        {
            get { return true; }
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams p = base.CreateParams;
                
                p.ExStyle |= WS_EX_NOACTIVATE;

                return p;
            }
        }

    }
}
