// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace HaXeContext.Helpers
{
    class FlagEnumEditor : UITypeEditor
    {
        readonly FlagCheckedListBox<CompletionFeatures> list;

        public FlagEnumEditor()
        {
            list = new FlagCheckedListBox<CompletionFeatures>();
        }

        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) => UITypeEditorEditStyle.DropDown;

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (context?.PropertyDescriptor is null) return null;
            var service = (IWindowsFormsEditorService) provider.GetService(typeof(IWindowsFormsEditorService));
            list.Value = EnumHelper.ConvertToInt(value);
            service.DropDownControl(list);
            return (CompletionFeatures)list.Value;
        }
    }

    class CheckedListBoxItem<T>
    {
        public int IntValue;
        public T Value;

        public override string ToString() => Enum.GetName(typeof(T), Value);
    }

    class FlagCheckedListBox<T> : CheckedListBox
    {
        public FlagCheckedListBox()
        {
            CheckOnClick = true;
            foreach (T val in Enum.GetValues(typeof(T)))
            {
                var pos = EnumHelper.ConvertToInt(val);

                var item = new CheckedListBoxItem<T>
                {
                    Value = val,
                    IntValue = pos
                };
                Items.Add(item, false);
            }
        }

        public int Value
        {
            get
            {
                var value = 0;
                foreach (CheckedListBoxItem<T> item in CheckedItems)
                {
                    value |= item.IntValue;
                }
                return value;
            }
            set
            {
                for (var i = 0; i < Items.Count; ++i)
                {
                    var item = (CheckedListBoxItem<T>) Items[i];
                    SetItemChecked(i, (item.IntValue & value) == item.IntValue);
                }
            }
        }
    }

    class EnumHelper
    {
        public static int ConvertToInt<T>(T value) => (int)Convert.ChangeType(value, typeof(int));
    }
}
