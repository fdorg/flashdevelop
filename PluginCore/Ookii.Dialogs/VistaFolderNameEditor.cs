// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
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