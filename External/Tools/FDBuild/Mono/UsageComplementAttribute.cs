namespace Mono
{
    using System;

    [AttributeUsage(AttributeTargets.Assembly)]
    public class UsageComplementAttribute : Attribute
    {
        // Methods
        public UsageComplementAttribute(string details)
        {
            Details = details;
        }

        public override string ToString() => Details;


        // Fields
        public string Details;
    }
}