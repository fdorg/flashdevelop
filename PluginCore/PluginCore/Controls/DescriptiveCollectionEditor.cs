﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;
using PluginCore.Helpers;

namespace PluginCore.Controls
{
    /// <summary>
    /// A <see cref="CollectionEditor"/> of type <see cref="T"/> with a description panel.
    /// </summary>
    /// <typeparam name="T">Type of items in the collection.</typeparam>
    public class DescriptiveCollectionEditor<T> : CollectionEditor
    {
        public DescriptiveCollectionEditor(Type type) : base(type) { }

        protected override CollectionForm CreateCollectionForm()
        {
            var form = base.CreateCollectionForm();
            form.Size = ScaleHelper.Scale(new Size(600, 400));
            ShowDescription(form);
            return form;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            var list = new List<T>((T[]) value);
            list = (List<T>) base.EditValue(context, provider, list);
            return list.ToArray();
        }

        protected override Type CreateCollectionItemType()
        {
            return typeof(T);
        }

        static void ShowDescription(Control control)
        {
            var grid = control as PropertyGrid;
            if (grid != null) grid.HelpVisible = true;
            foreach (Control child in control.Controls) ShowDescription(child);
        }
    }
}
