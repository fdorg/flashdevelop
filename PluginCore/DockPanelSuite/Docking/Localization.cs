// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.ComponentModel;

namespace WeifenLuo.WinFormsUI.Docking
{
    [AttributeUsage(AttributeTargets.All)]
    internal sealed class LocalizedDescriptionAttribute : DescriptionAttribute
    {
        private bool m_initialized = false;

        public LocalizedDescriptionAttribute(string key) : base(key)
        {
        }

        public override string Description
        {
            get
            {   
                if (!m_initialized)
                {
                    string key = base.Description;
                    DescriptionValue = ResourceHelper.GetString(key);
                    if (DescriptionValue is null)
                        DescriptionValue = string.Empty;

                    m_initialized = true;
                }

                return DescriptionValue;
            }
        }
    }

    [AttributeUsage(AttributeTargets.All)]
    internal sealed class LocalizedCategoryAttribute : CategoryAttribute
    {
        public LocalizedCategoryAttribute(string key) : base(key)
        {
        }

        protected override string GetLocalizedString(string key)
        {
            return ResourceHelper.GetString(key);
        }
    }
}
