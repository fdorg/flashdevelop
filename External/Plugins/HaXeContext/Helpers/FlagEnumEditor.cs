using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace HaXeContext.Helpers
{
    class FlagEnumEditor : UITypeEditor
    {
        CheckedList

        public FlagEnumEditor()
        {
            
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (context == null || context.PropertyDescriptor == null) return null;

            var service = (IWindowsFormsEditorService) provider.GetService(typeof(IWindowsFormsEditorService));
            
            service.DropDownControl();
            
            return base.EditValue(context, provider, value);
        }
    }

    class FlagCheckedListBox<T> : CheckedListBox
    {
        int internalValue = 0;

        public T Value
        {
            get
            {
                return Convert.ChangeType(internalValue, typeof(Enum));
            }
            set
            {
                
            }
        }
    }
}
