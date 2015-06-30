using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace PluginCore.DockPanelSuite.Helpers
{
    public class DelayedDockContent : Control
    {
        private Func<Control> createView;

        public DelayedDockContent(Func<Control> createView)
        {
            this.createView = createView;
        }

        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);

            var dockContent = Parent as DockContent;
            if (dockContent != null)
            {
                dockContent.DockStateChanged += DockStateChanged;
            }
        }

        void DockStateChanged(object sender, EventArgs e)
        {
            var dockContent = Parent as DockContent;
            if (dockContent != null && dockContent.DockState != DockState.Hidden)
            {
                dockContent.DockStateChanged -= DockStateChanged;

                var view = createView();
                if (view != null)
                {
                    view.Dock = Dock;
                    Parent.Controls.Add(view);
                    Parent.Controls.Remove(this);
                }
            }
        }
    }
}
