using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ASCompletion.Completion;
using ASCompletion.Controls;
using CodeFormatter.Dialogs;
using CodeFormatter.Preferences;

namespace CodeFormatter.Utilities
{
    class HaxeAStyleEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            var dialog = new HaxeAStyleDialog((HaxeAStyleOptions) value);
            return dialog.ShowDialog(PluginCore.PluginBase.MainForm) == DialogResult.OK ? dialog.GetOptions() : value;
        }
    }
}
