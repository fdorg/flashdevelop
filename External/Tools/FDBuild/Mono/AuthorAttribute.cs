namespace Mono
{
    using System;

    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple=true)]
    public class AuthorAttribute : Attribute
    {
        // Methods
        public AuthorAttribute(string name)
        {
            Name = name;
            SubProject = null;
        }

        public AuthorAttribute(string name, string subProject)
        {
            Name = name;
            SubProject = subProject;
        }

        public override string ToString()
        {
            if (SubProject is null)
            {
                return Name;
            }
            return (Name + " (" + SubProject + ")");
        }
        
        // Fields
        public string Name;
        public string SubProject;
    }
}