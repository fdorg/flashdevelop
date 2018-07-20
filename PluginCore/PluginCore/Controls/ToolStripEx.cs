using System;
using System.Windows.Forms;
using System.Reflection;

namespace PluginCore.Controls
{
    public class ToolStripEx : ToolStrip
    {
        #region Win32

        private const UInt32 MA_ACTIVATE = 1;
        private const UInt32 MA_ACTIVATEANDEAT = 2;
        private const UInt32 WM_MOUSEACTIVATE = 0x21;

        #endregion

        private Boolean clickThrough = true;

        /// <summary>
        /// Listen for all items added
        /// </summary>
        public ToolStripEx()
        {
            this.ItemAdded += new ToolStripItemEventHandler(this.OnItemAdded);
            ((Form)PluginBase.MainForm).Deactivate += new EventHandler(this.OnFormDeactivate);
            ((Form)PluginBase.MainForm).Activated += new EventHandler(this.OnFormDeactivate);
        }

        /// <summary>
        /// When the main form loses or gains input focus, clear all selections and repaint
        /// </summary>
        private void OnFormDeactivate(object sender, EventArgs e)
        {
            try
            {
                MethodInfo method = typeof(ToolStrip).GetMethod("ClearAllSelections", BindingFlags.NonPublic | BindingFlags.Instance);
                method.Invoke(this, null);
                this.Invalidate();
            }
            catch { /* No errors... */ }
        }

        /// <summary>
        /// When button is added, listen for it's hover events
        /// </summary>
        private void OnItemAdded(Object sender, ToolStripItemEventArgs e)
        {
            if (e.Item is ToolStripButton)
            {
                e.Item.MouseEnter += new EventHandler(this.OnOverChange);
                e.Item.MouseLeave += new EventHandler(this.OnOverChange);
            }
        }

        /// <summary>
        /// Invalidate button if it is not selected to workaround the "textbox selected, no button hover" issue. :)
        /// </summary>
        private void OnOverChange(Object sender, EventArgs e)
        {
            ToolStripItem item = sender as ToolStripItem;
            if (!item.Selected) item.Invalidate();
        }

        /// <summary>
        /// Gets or sets whether the ToolStripEx honors item clicks when its containing form does not have input focus.
        /// </summary>
        public bool ClickThrough
        {
            get { return this.clickThrough; }
            set { this.clickThrough = value; }
        }

        /// <summary>
        /// If ClickThrough is enabled do not eat the activate event.
        /// </summary>
        protected override void WndProc(ref Message m)
        {
            try
            {
                base.WndProc(ref m);
                if (this.clickThrough && m.Msg == WM_MOUSEACTIVATE && m.Result == (IntPtr)MA_ACTIVATEANDEAT)
                {
                    m.Result = (IntPtr)MA_ACTIVATE;
                }
            }
            catch {}
        }

    }

}

