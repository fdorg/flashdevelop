using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;

namespace Ookii.Dialogs
{
    public class VistaFolderNameEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            using var dialog = new VistaFolderBrowserDialog();
            if (value != null) dialog.SelectedPath = $"{value}";
            if (dialog.ShowDialog(null) == DialogResult.OK)
                return dialog.SelectedPath;
            return value;
        }
    }
}