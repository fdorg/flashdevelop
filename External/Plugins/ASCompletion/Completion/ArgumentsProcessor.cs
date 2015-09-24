using System.Collections;
using System.Text.RegularExpressions;

namespace ASCompletion.Completion
{
    public class ArgumentsProcessor
    {
        static public string Process(string text, Hashtable variables)
        {
            ArgumentsProcessor proc = new ArgumentsProcessor();
            proc.variables = variables;
            return proc.Run(text);
        }

        /* PRIVATE */

        static private Regex re_Argument =
            new Regex("\\$\\((?<name>[a-z]+)\\)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private Hashtable variables;

        private string Run(string text)
        {
            return re_Argument.Replace(text, new MatchEvaluator(Lookup));
        }

        private string Lookup(Match m)
        {
            string name = m.Groups["name"].Value;
            if (variables.ContainsKey(name)) return (string)variables[name];
            else return m.Value;
        }
    }
}
