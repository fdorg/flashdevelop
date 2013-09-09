namespace Mono
{
    using System;

    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple=true)]
    public class AuthorAttribute : Attribute
    {
        // Methods
        public AuthorAttribute(string name)
        {
            this.Name = name;
            this.SubProject = null;
        }

        public AuthorAttribute(string name, string subProject)
        {
            this.Name = name;
            this.SubProject = subProject;
        }

        public override string ToString()
        {
            if (this.SubProject == null)
            {
                return this.Name;
            }
            return (this.Name + " (" + this.SubProject + ")");
        }


        // Fields
        public string Name;
        public string SubProject;
    }
}

