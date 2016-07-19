using System.Windows.Forms;

namespace PluginCore.Controls
{
    /// <summary>
    /// A form that, unless forced directly by code, does not become the foreground window when shown or clicked
    /// </summary>
    public class InactiveForm : Form
    {

        private const int WS_EX_TOPMOST = 0x8;
        private const int WS_EX_NOACTIVATE = 0x8000000;

        protected override bool ShowWithoutActivation
        {
            get { return true; }
        }

        private bool topMost;
        /// <summary>
        /// Determines whether the form should display as top most. Unlike Form.TopMost, this does not give focus
        /// </summary>
        public new bool TopMost
        {
            get { return topMost; }
            set
            {
                if (topMost == value) return;
                topMost = value;
                if (IsHandleCreated) RecreateHandle();
            }
        }

        private bool noActivate;
        /// <summary>
        /// Gets or sets if the form can become the foreground window. In the case of a top level window this only works for
        /// other applications, not within the same one.
        /// </summary>
        public bool NoActivate
        {
            get { return noActivate; }
            set
            {
                if (noActivate == value) return;
                noActivate = value;
                if (IsHandleCreated) RecreateHandle();
            }
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams p = base.CreateParams;
                
                if (noActivate)
                    p.ExStyle |= WS_EX_NOACTIVATE;

                if (topMost)
                    p.ExStyle |= WS_EX_TOPMOST;

                return p;
            }
        }

    }
}
