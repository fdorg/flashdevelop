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
        /// <summary>
        /// 
        /// </summary>
        static void Main(string[] args)
        {
            var git = ".git";
            var output = args[0];
            var revOut = output + ".rev";
            if (!File.Exists(revOut))
            {
                Console.WriteLine("Template not found: " + revOut);
                return;
            }
            var head = File.ReadAllText(Path.Combine(git, "HEAD")).Trim();
            var headRef = Regex.Match(head, "ref: refs/heads/(.*)");
            if (!headRef.Success)
            {
                Console.WriteLine("SetVersion: can not find HEAD ref, write null.");
                WriteFile(output, revOut, Environment.ExpandEnvironmentVariables("%CommitId%"), Environment.ExpandEnvironmentVariables("%BranchId%"));
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
            Console.WriteLine("Set revision: " + branch + " " + commit);
            WriteFile(output, revOut, commit, branch);
        }

        /// <summary>
        /// 
        /// </summary>
        static void WriteFile(String output, String revOut, String commit, String branch)
        {
            var raw = File.ReadAllText(revOut);
            raw = raw.Replace("$BRANCH$", branch).Replace("$COMMIT$", commit);
            File.WriteAllText(output, raw);
        }

    }
}
