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
            using (VistaFolderBrowserDialog browser = new VistaFolderBrowserDialog())
            {
                if (value != null)
                {
                    browser.SelectedPath = $"{value}";
                }

                if (browser.ShowDialog(null) == DialogResult.OK)
                    return browser.SelectedPath;
            }

            return value;
        }
    }
}