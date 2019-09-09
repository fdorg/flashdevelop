// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
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