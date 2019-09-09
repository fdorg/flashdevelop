using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ASCompletion.Model;

namespace AS3IntrinsicsGenerator
{
    public class BaseModel
    {
        protected const string NL = "\r\n";
        protected const char SEMI = ';';

        public string Name;
        public string Comment;

        public virtual void Format(StringBuilder sb, string tabs) { }

        static public string Camelize(string name)
        {
            string[] parts = name.ToLower().Split('_');
            string result = "";
            foreach (string part in parts)
            {
                if (result.Length > 0)
                    result += Char.ToUpper(part[0]) + part.Substring(1);
                else result = part;
            }
            return result;
        }
    }

    public class BlockModel : BaseModel
    {
        public List<string> Imports;
        public List<EventModel> Events = new List<EventModel>();
        public List<PropertyModel> Properties = new List<PropertyModel>();
        public List<MethodModel> Methods = new List<MethodModel>();
        public List<BlockModel> Blocks = new List<BlockModel>();
    }

    public class MethodModel : BaseModel
    {
        static private Regex reComa = new Regex(",([a-z])", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        static private Regex reRest = new Regex("([a-z0-9]+):restParam", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        static private Regex reVector = new Regex("Vector\\$([a-z.]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        static private Regex reType = new Regex(":([a-z.$]+):", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public string Params;
        public string ReturnType;

        public void FixParams()
        {
            if (Params.Length > 0)
            {
                Params = Params.Replace("[", "");
                Params = Params.Replace("]", "");
                Params = reComa.Replace(Params, ", $1");
                Params = reType.Replace(Params, ":$1.");
                Params = reVector.Replace(Params, "Vector.<$1>");
                Match m = reRest.Match(Params);
                if (m.Success)
                    Params = Params.Substring(0, m.Index) + "..." + m.Groups[1].Value + Params.Substring(m.Index + m.Length);
                Params = Params.Replace("=", " = ");
            }
            if (ReturnType != null)
            {
                ReturnType = ReturnType.Replace(':', '.');
                if (ReturnType.StartsWith("Vector"))
                    ReturnType = reVector.Replace(ReturnType, "Vector.<$1>");
            }
        }
    }

    public class PropertyModel : BaseModel
    {
        public string ValueType;
        public string Kind = "var";
    }

    public class EventModel : BaseModel
    {
        public string EventType;
        public bool IsAIR;
        public bool IsFP10;
    }
}
