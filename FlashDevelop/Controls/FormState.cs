using System.Drawing;
using System.Windows.Forms;
using PluginCore;

// From: http://www.vesic.org/english/blog/winforms/full-screen-maximize/

namespace FlashDevelop.Controls
{
    /// <summary>
    /// Class used to preserve and restore state of the form
    /// </summary>
    public class FormState
    {
        private Rectangle windowBounds;
        private FormWindowState winState;
        private FormBorderStyle borderStyle;

        /// <summary>
        /// Maximizes the form to fullscreen
        /// </summary>
        public void Maximize(Form form)
        {
            this.Save(form);
            form.WindowState = FormWindowState.Maximized;
            form.FormBorderStyle = FormBorderStyle.None;
            if (Win32.ShouldUseWin32()) Win32.SetWinFullScreen(form.Handle);
        }

        /// <summary>
        /// Saves the state before maximizing
        /// </summary>
        public void Save(Form form)
        {
            this.winState = form.WindowState;
            this.borderStyle = form.FormBorderStyle;
            this.windowBounds = form.Bounds;
        }

        /// <summary>
        /// Restores the form to old state
        /// </summary>
        public void Restore(Form form)
        {
            form.WindowState = this.winState;
            form.FormBorderStyle = this.borderStyle;
            form.Bounds = this.windowBounds;
        }

    }

}
