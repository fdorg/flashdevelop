using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ASCompletion.Settings;
using PluginCore.Controls;

namespace ASCompletion.Helpers
{
    class ModifierOrderEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            string[] modifierOrder = value as string[] ?? GeneralSettings.DEFAULT_DECLARATION_MODIFIER_ORDER;
            var dialog = new FixedValuesCollectionEditor<string>(GeneralSettings.ALL_DECLARATION_MODIFIERS, modifierOrder, GeneralSettings.DECLARATION_MODIFIER_OTHER);

            return dialog.ShowDialog(null) == DialogResult.OK ? dialog.Value : value;
        }
    }
}
