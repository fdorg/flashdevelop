using System.Collections;
using System.Windows.Forms;
using AirProperties.Controls;

namespace AirProperties.Forms
{
    static class WizardHelper
    {

        public static void SetControlValue(string value, TextBox field)
        {
            if (value == null)
                field.Text = string.Empty;
            else
                field.Text = value;
        }

        public static void SetControlValue(string value, CheckBox field)
        {
            if (string.IsNullOrEmpty(value))
                field.CheckState = CheckState.Indeterminate;
            else
            {
                value = value.ToLower();
                field.Checked = value == "true" || value == "yes";
            }
        }

        public static void SetControlValue(bool? value, CheckBox field)
        {
            if (value == null)
                field.CheckState = CheckState.Indeterminate;
            else
                field.Checked = value.Value;
        }

        public static void SetControlValue(string value, ComboBox field, int defaultIndex)
        {
            bool foundListItem = false;
            if (value != null)
            {
                foreach (ListItem listItem in field.Items)
                {
                    if (listItem.Value == value.Trim())
                    {
                        field.SelectedItem = listItem;
                        foundListItem = true;
                        break;
                    }
                }
                if (!foundListItem) field.SelectedIndex = defaultIndex;
            }
            else field.SelectedIndex = defaultIndex;
        }

        public static void SetControlValue(IEnumerable value, CheckedComboBox field)
        {
            if (value != null)
            {
                for (int i = 0, count = field.Items.Count; i < count; i++)
                {
                    bool valuePresent = false;
                    var listItem = ((ListItem) field.Items[i]);
                    foreach (object item in value)
                    {
                        string val = item as string;
                        if (item == null) continue;

                        if (val.Trim() == listItem.Value)
                        {
                            valuePresent = true;
                            break;
                        }
                    }

                    field.SetItemChecked(i, valuePresent);
                }
            }
            else
                for (int i = 0, count = field.Items.Count; i < count; i++)
                    field.SetItemChecked(i, false);
        }

    }
}
