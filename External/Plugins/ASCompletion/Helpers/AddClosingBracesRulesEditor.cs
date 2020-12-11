// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using ASCompletion.Completion;
using ASCompletion.Controls;

namespace ASCompletion.Helpers
{
    internal class AddClosingBracesRulesEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            using var dialog = new AddClosingBracesRulesEditorForm((Brace[]) value);
            return dialog.ShowDialog(null) == DialogResult.OK ? dialog.Value : value;
        }
    }
}
