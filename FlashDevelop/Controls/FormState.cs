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
        Rectangle windowBounds;
        FormWindowState winState;
        FormBorderStyle borderStyle;

        /// <summary>
        /// Maximizes the form to fullscreen
        /// </summary>
        public void Maximize(Form form)
        {
            Save(form);
            form.WindowState = FormWindowState.Maximized;
            form.FormBorderStyle = FormBorderStyle.None;
            if (Win32.ShouldUseWin32()) Win32.SetWinFullScreen(form.Handle);
        }

        /// <summary>
        /// Saves the state before maximizing
        /// </summary>
        public void Save(Form form)
        {
            winState = form.WindowState;
            borderStyle = form.FormBorderStyle;
            windowBounds = form.Bounds;
        }

        /// <summary>
        /// Restores the form to old state
        /// </summary>
        public void Restore(Form form)
        {
            form.WindowState = winState;
            form.FormBorderStyle = borderStyle;
            form.Bounds = windowBounds;
        }
    }
}