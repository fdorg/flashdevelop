using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace SetVersion
{
    class Program
    {
        static void Main(string[] args)
        {
            var output = args[0];
            var revOut = output + ".rev";
            if (!File.Exists(revOut))
            {
                Console.WriteLine("Template not found: " + revOut);
                return;
            }
            var info = "1501285 (HEAD, origin/master, origin/HEAD, master, feature/testbranch)"; // Console.ReadLine().Trim();
            var m = Regex.Match(info, "([a-z0-9]+) \\(([^\\)]+)\\)");
            if (!m.Success)
            {
                Console.WriteLine("Invalid data: " + info);
                return;
            }
            var names = Regex.Split(m.Groups[2].Value, ", ");
            var branch = names[names.Length - 1];
            var commit = m.Groups[1].Value;
            Console.WriteLine("Set revision: " + branch + " " + commit);
            var raw = File.ReadAllText(revOut);
            raw = raw.Replace("$BRANCH$", branch).Replace("$COMMIT$", commit);
            File.WriteAllText(output, raw);
        }
    }
}
