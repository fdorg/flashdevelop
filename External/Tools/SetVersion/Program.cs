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
            var git = ".git";
            var head = File.ReadAllText(Path.Combine(git, "HEAD")).Trim();
            var headRef = Regex.Match(head, "ref: refs/heads/(.*)");
            if (!headRef.Success)
            {
                Console.WriteLine("SetVersion: can not find HEAD ref");
                return;
            }
            var branch = headRef.Groups[1].Value;
            var refPath = Path.Combine(Path.Combine(git, "refs\\heads"), branch);
            if (!File.Exists(refPath))
            {
                Console.WriteLine("SetVersion: can not read ref commit hash");
                return;
            }
            var commit = File.ReadAllText(refPath).Trim();
            if (commit.Length == 40) commit = commit.Substring(0, 10);
            
            var output = args[0];
            var revOut = output + ".rev";
            if (!File.Exists(revOut))
            {
                Console.WriteLine("Template not found: " + revOut);
                return;
            }
            Console.WriteLine("Set revision: " + branch + " " + commit);
            var raw = File.ReadAllText(revOut);
            raw = raw.Replace("$BRANCH$", branch).Replace("$COMMIT$", commit);
            File.WriteAllText(output, raw);
        }
    }
}
