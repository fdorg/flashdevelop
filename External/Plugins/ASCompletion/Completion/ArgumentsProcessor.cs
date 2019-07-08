using System.Collections;
using System.Text.RegularExpressions;

namespace ASCompletion.Completion
{
    public class ArgumentsProcessor
    {
        public static string Process(string text, Hashtable variables)
        {
            return new ArgumentsProcessor {variables = variables}.Run(text);
        }

        /* PRIVATE */

        private static readonly Regex re_Argument =
            new Regex("\\$\\((?<name>[a-z]+)\\)", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

        private Hashtable variables;

        private string Run(string text) => re_Argument.Replace(text, Lookup);

        private string Lookup(Match m)
        {
            string name = m.Groups["name"].Value;
            if (variables.ContainsKey(name)) return (string)variables[name];
            return m.Value;
        }
    }
}
