using System;
using System.IO;
using System.Windows.Forms;
using PluginCore;

namespace StartPage.Controls
{
    public class DragDropPanel : Panel
    {
        public DragDropPanel()
        {
            this.AutoSize = false;
            this.AllowDrop = true;
            this.DragOver += new DragEventHandler(this.PanelDragOver);
            this.DragDrop += new DragEventHandler(this.PanelDragDrop);
            this.Width = 300;
            this.Height = 90;
        }

        /// <summary>
        /// Do not paint the background.
        /// </summary>
        protected override void OnPaintBackground(PaintEventArgs e) { }

        /// <summary>
        /// Gets the creation parameters.
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x20;
                return cp;
            }
        }

        /// <summary>
        /// Handles the drag over event and enables correct dn'd effects.
        /// </summary>
        private void PanelDragOver(Object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Move | DragDropEffects.Copy;
            }
            else e.Effect = DragDropEffects.None;
        }

        /// <summary>
        /// Handles the actual file drop and opens them as editable documents.
        /// </summary>
        private void PanelDragDrop(Object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                String[] files = (String[])e.Data.GetData(DataFormats.FileDrop);
                foreach (String file in files)
                {
                    if (File.Exists(file))
                    {
                        PluginBase.MainForm.OpenEditableDocument(file);
                    }
                }
            }
        }

    }

}
