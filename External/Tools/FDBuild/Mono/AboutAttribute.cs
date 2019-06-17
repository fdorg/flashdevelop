namespace Mono
{
    using System;

    [AttributeUsage(AttributeTargets.Assembly)]
    public class AboutAttribute : Attribute
    {
        // Methods
        public AboutAttribute(string details)
        {
            Details = details;
        }

        public override string ToString()
        {
            return Details;
        }


        // Fields
        public string Details;
    }
}

