using System;
using System.ComponentModel;

namespace PluginCore.Localization
{
    [AttributeUsage(AttributeTargets.All)]
    public class LocalizedCategoryAttribute : CategoryAttribute
    {
        public LocalizedCategoryAttribute(string key) : base(key) { }

        /// <summary>
        /// Gets the localized string
        /// </summary>
        protected override string GetLocalizedString(string key) => TextHelper.GetString(key);
    }

    [AttributeUsage(AttributeTargets.All)]
    public class LocalizedDescriptionAttribute : DescriptionAttribute
    {
        bool initialized;

        public LocalizedDescriptionAttribute(string key) : base(key) { }

        /// <summary>
        /// Gets the description of the string
        /// </summary>
        public override string Description
        {
            get
            {
                if (!initialized)
                {
                    DescriptionValue = TextHelper.GetString(base.Description);
                    initialized = true;
                }
                return DescriptionValue;
            }
        }

    }

    [AttributeUsage(AttributeTargets.All)]
    public class StringValueAttribute : Attribute
    {
        public StringValueAttribute(string value)
        {
            Value = value;
        }

        /// <summary>
        /// Gets the string value of the class
        /// </summary>
        public string Value { get; }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class RequiresRestartAttribute : Attribute
    {
        public override bool Match(object obj) => obj is RequiresRestartAttribute;
    }
}