namespace Mono
{
    using System;

    [AttributeUsage(AttributeTargets.Assembly)]
    public class UsageComplementAttribute : Attribute
    {
        // Methods
        public UsageComplementAttribute(string details)
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

