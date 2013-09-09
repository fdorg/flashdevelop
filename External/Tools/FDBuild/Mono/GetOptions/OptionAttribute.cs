namespace Mono.GetOptions
{
    using System;

    [AttributeUsage(AttributeTargets.Field | (AttributeTargets.Property | AttributeTargets.Method))]
    public class OptionAttribute : Attribute
    {
        // Methods
        public OptionAttribute(string shortDescription)
        {
            this.SetValues(shortDescription, ' ', string.Empty, string.Empty, 1);
        }

        public OptionAttribute(int maxOccurs, string shortDescription)
        {
            this.SetValues(shortDescription, ' ', string.Empty, string.Empty, maxOccurs);
        }

        public OptionAttribute(string shortDescription, char shortForm)
        {
            this.SetValues(shortDescription, shortForm, string.Empty, string.Empty, 1);
        }

        public OptionAttribute(string shortDescription, string longForm)
        {
            this.SetValues(shortDescription, ' ', longForm, string.Empty, 1);
        }

        public OptionAttribute(int maxOccurs, string shortDescription, char shortForm)
        {
            this.SetValues(shortDescription, shortForm, string.Empty, string.Empty, maxOccurs);
        }

        public OptionAttribute(int maxOccurs, string shortDescription, string longForm)
        {
            this.SetValues(shortDescription, ' ', longForm, string.Empty, maxOccurs);
        }

        public OptionAttribute(string shortDescription, char shortForm, string longForm)
        {
            this.SetValues(shortDescription, shortForm, longForm, string.Empty, 1);
        }

        public OptionAttribute(string shortDescription, string longForm, string alternateForm)
        {
            this.SetValues(shortDescription, ' ', longForm, alternateForm, 1);
        }

        public OptionAttribute(int maxOccurs, string shortDescription, char shortForm, string longForm)
        {
            this.SetValues(shortDescription, shortForm, longForm, string.Empty, maxOccurs);
        }

        public OptionAttribute(int maxOccurs, string shortDescription, string longForm, string alternateForm)
        {
            this.SetValues(shortDescription, ' ', longForm, alternateForm, maxOccurs);
        }

        public OptionAttribute(string shortDescription, char shortForm, string longForm, string alternateForm)
        {
            this.SetValues(shortDescription, shortForm, longForm, alternateForm, 1);
        }

        public OptionAttribute(int maxOccurs, string shortDescription, char shortForm, string longForm, string alternateForm)
        {
            this.SetValues(shortDescription, shortForm, longForm, alternateForm, maxOccurs);
        }

        private void SetValues(string shortDescription, char shortForm, string longForm, string alternateForm, int maxOccurs)
        {
            this.ShortDescription = shortDescription;
            this.ShortForm = shortForm;
            this.LongForm = longForm;
            this.MaxOccurs = maxOccurs;
            this.AlternateForm = alternateForm;
        }


        // Fields
        public string AlternateForm;
        public string LongForm;
        public int MaxOccurs;
        public bool SecondLevelHelp;
        public string ShortDescription;
        public char ShortForm;
        public bool VBCStyleBoolean;
    }
}

