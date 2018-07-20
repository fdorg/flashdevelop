// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Text.RegularExpressions;
using System.Collections;
using System.IO;

namespace PHPFunctionsParser
{
    class Program
    {
        //ignore CVS-only functions
        const bool noCVS = true;

        static void Main(string[] args)
        {
            //include additional parameters (@optional and @multi)
            bool extended = false;

            WebClient wc = new WebClient();

            List<Dictionary<string, string>> funcs = new List<Dictionary<string, string>>();

            for (char i = 'A'; i <= 'Z'; i++)
            {
                string table = wc.DownloadString("http://phpfunction.info/browse.php?letter=" + i.ToString());

                MatchCollection mc = Regex.Matches(table, "<tr height=\"40\">(?<row>.*?)</tr>", RegexOptions.Multiline | RegexOptions.Compiled);
                foreach (Match m in mc)
                {
                    Dictionary<string, string> func = new Dictionary<string, string>();
                    MatchCollection mc1 = Regex.Matches(m.Groups["row"].Value, "<td[^>]+>(?<cell>.*?)</td>", RegexOptions.Multiline | RegexOptions.Compiled);
                    Match m1 = Regex.Match(mc1[0].Groups["cell"].Value, "<a.+?[^\\-]>(?<a>.*?)</a>", RegexOptions.Multiline | RegexOptions.Compiled);
                    func["name"] = m1.Groups["a"].Value.Trim();
                    //if (func["name"].Contains("->")) continue;
                    func["version"] = mc1[1].Groups["cell"].Value.Trim();
                    if (func["version"].Contains("only in CVS")) continue;
                    func["params"] = mc1[2].Groups["cell"].Value.Replace("N/A", "").Replace("void", "").Trim();
                    func["desc"] = mc1[3].Groups["cell"].Value.Trim();
                    if (func["desc"].StartsWith(func["name"]))
                    {
                        func["desc"] = func["desc"].Remove(0, func["name"].Length).Trim();
                    }
                    func["return"] = mc1[4].Groups["cell"].Value.Replace("void", "").Trim();
                    funcs.Add(func);
                }
            }
            string lastClass = "";
            foreach (Dictionary<string, string> func in funcs)
            {
                bool staticClassFunc = false;
                string curClass = "";
                if (func["name"].Contains("::"))
                {
                    //staticClassFunc = true;
                    string[] s = Regex.Split(func["name"], "::");
                    curClass = s[0];
                    func["name"] = s[1];
                    if (func["name"] == "__construct") func["name"] = curClass;
                }
                if (func["name"].Contains("->"))
                {
                    string[] s = Regex.Split(func["name"], "->");
                    curClass = s[0];
                    func["name"] = s[1];
                }
                if (lastClass.ToLower() != curClass.ToLower())
                {
                    if (lastClass != "") Write(lastClass, "}");
                    lastClass = curClass;
                    if (curClass != "") Write(lastClass, "class " + curClass + " {");
                }
                Write(lastClass, "/**");
                Write(lastClass, "* " + func["desc"]);
                Write(lastClass, "* @return " + func["return"]);
                Write(lastClass, "* @version " + func["version"]);

                string[] pars = func["params"].Split(',');
                string prevFunc = "";
                List<string> paramList = new List<string>();
                foreach (string par in pars)
                {
                    int optionalLevel = prevFunc.Split('[').Length - prevFunc.Split(']').Length;
                    string[] p = par.Replace(" [", "").Replace("[", "").Replace("]", "").Trim().Split(' ');
                    if (p.Length == 1) p = new string[] { "", p[0] };
                    if (p[1] != "")
                    {
                        p[1] = p[1].Replace("&#38;", "&$");
                        if (p[1] == "...")
                        {
                            int i = 1;
                            while (paramList.Contains("$params" + i)) { i++; }
                            p[1] = "params" + i;
                            if (extended) Write(lastClass, "* @multi $" + p[1]);
                        }
                        if (!p[1].StartsWith("&$")) p[1] = "$" + p[1];
                        paramList.Add(p[1]);
                        string opt = "";
                        if (optionalLevel > 0)
                        {
                            if (extended) Write(lastClass, "* @optional " + p[1] + " " + optionalLevel);
                            opt = "(optional) ";
                        }
                        Write(lastClass, "* @param " + p[1] + " " + opt + p[0]);
                    }
                    prevFunc += par;
                }
                Write(lastClass, "*/");
                string statFunc = (staticClassFunc) ? "static " : "";
                Write(lastClass, statFunc + "function " + func["name"] + "(" + String.Join(", ", paramList.ToArray()) + ");");
            }
            if (lastClass != "")
                Write(lastClass, "}");
            Close();
            Console.WriteLine("finished");
            Console.ReadLine();
        }

        private static Dictionary<string, StreamWriter> streams = new Dictionary<string, StreamWriter>();

        private static void Write(string className, string line)
        {
            if (className == "") className = "toplevel";
            className = className.ToLower();
            if (!streams.ContainsKey(className))
            {
                streams[className] = File.CreateText(className + ".php");
                streams[className].WriteLine("<?");
            }
            streams[className].WriteLine(line);
        }

        private static void Close()
        {
            foreach (StreamWriter stream in streams.Values)
            {
                stream.WriteLine("?>");
                stream.Close();
            }
        }
    }
}
