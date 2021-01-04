using System.IO;
using System.Windows.Forms;
using PluginCore;

namespace StartPage.Controls
{
    public class DragDropPanel : Panel
    {
        public DragDropPanel()
        {
            AutoSize = false;
            AllowDrop = true;
            DragOver += PanelDragOver;
            DragDrop += PanelDragDrop;
            Width = 300;
            Height = 90;
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
                var cp = base.CreateParams;
                cp.ExStyle |= 0x20;
                return cp;
            }
        }

        /// <summary>
        /// Handles the drag over event and enables correct dn'd effects.
        /// </summary>
        static void PanelDragOver(object sender, DragEventArgs e)
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
        static void PanelDragDrop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop)) return;
            var files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (var file in files)
            {
                if (File.Exists(file))
                {
                    PluginBase.MainForm.OpenEditableDocument(file);
                }
            }
        }
    }
}