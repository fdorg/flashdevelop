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
            var commit = "";
            var branch = "";
            var git = ".git";
            var build = args[1];
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
                Console.WriteLine("SetVersion: Can not find HEAD ref, write from env vars.");
                commit = Environment.ExpandEnvironmentVariables("%APPVEYOR_REPO_COMMIT%");
                if (commit.Length == 40) commit = commit.Substring(0, 10);
                branch = Environment.ExpandEnvironmentVariables("%APPVEYOR_REPO_BRANCH%");
                build = Environment.ExpandEnvironmentVariables("%APPVEYOR_BUILD_NUMBER%");
                WriteFile(output, revOut, commit, branch, build);
                return;
            }
            branch = headRef.Groups[1].Value;
            var refPath = Path.Combine(Path.Combine(git, Path.Combine("refs", "heads")), branch);
            if (!File.Exists(refPath))
            {
                Console.WriteLine("SetVersion: Can not read ref commit hash.");
                return;
            }
            commit = File.ReadAllText(refPath).Trim();
            if (commit.Length == 40) commit = commit.Substring(0, 10);
            Console.WriteLine("Set revision: " + branch + "-" + commit + "-" + build);
            WriteFile(output, revOut, commit, branch, build);
        }

        /// <summary>
        /// 
        /// </summary>
        static void WriteFile(String output, String revOut, String commit, String branch, String build)
        {
            var raw = File.ReadAllText(revOut);
            if (build != "") build = "." + build;
            raw = raw.Replace("$BRANCH$", branch).Replace("$COMMIT$", commit).Replace("$BUILD$", build);
            File.WriteAllText(output, raw);
        }

    }
}
