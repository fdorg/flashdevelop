namespace Mono
{
    using System;

    [AttributeUsage(AttributeTargets.Assembly)]
    public class AboutAttribute : Attribute
    {
        // Methods
        public AboutAttribute(string details)
        {
            this.Details = details;
        }

        public override string ToString()
        {
            return this.Details;
        }


        // Fields
        public string Details;
    }
}

