using System.Collections;
using System.Text.RegularExpressions;

namespace ASCompletion.Completion
{
    public class ArgumentsProcessor
    {
        public static string Process(string text, Hashtable variables) => new ArgumentsProcessor {variables = variables}.Run(text);

        static readonly Regex re_Argument =
            new Regex("\\$\\((?<name>[a-z]+)\\)", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

        Hashtable variables;

        string Run(string text) => re_Argument.Replace(text, Lookup);

        string Lookup(Match m)
        {
            var name = m.Groups["name"].Value;
            if (variables.ContainsKey(name)) return (string)variables[name];
            return m.Value;
        }
    }
}
