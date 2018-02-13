using System;
using System.ComponentModel;
using System.Drawing.Design;
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

            using (var dialog = new FixedValuesCollectionEditor<string>(GeneralSettings.ALL_DECLARATION_MODIFIERS, modifierOrder, GeneralSettings.DECLARATION_MODIFIER_REST))
                return dialog.ShowDialog(null) == DialogResult.OK ? dialog.Value : value;
        }
    }
}
