using System;
using ProjectManager.Projects;

namespace ProjectManager.Building
{
    /// <summary>
    /// Processed pre and post-build steps, filling in project variables
    /// </summary>
    public class BuildEventRunner
    {
        Project project;
        BuildEventVars vars;

        public BuildEventRunner(Project project, string compilerPath)
        {
            this.project = project;
            project.CurrentSDK = compilerPath;
            this.vars = new BuildEventVars(project);
        }

        //parse line into command/argument pair
        private string[] tokenize(string line)
        {
            string[] result = new String[2];

            if (line.StartsWith("\"", StringComparison.Ordinal))
            {
                int endQuote = line.IndexOf('\"', 1);
                result[0] = (endQuote > -1) ? line.Substring(1, endQuote - 1) : line;
                result[1] = (endQuote > -1) ? line.Substring(endQuote + 1).TrimStart() : "";
            }
            else
            {
                int space = line.IndexOf(' ');
                result[0] = (space > -1) ? line.Substring(0, space) : line;
                result[1] = (space > -1) ? line.Substring(space + 1) : "";
            }

            return result;
        }

        public void Run(string buildEvents, bool noTrace)
        {
            string[] events = buildEvents.Split('\n');
            if (events.Length == 0)
                return;

            BuildEventInfo[] infos = vars.GetVars();
            foreach (string buildEvent in events)
            {
                Environment.CurrentDirectory = project.Directory;

                string line = buildEvent.Trim();

                if (line.Length <= 0)
                    continue; // nothing to do

                // conditional execution
                if (line.StartsWith("DEBUG:", StringComparison.Ordinal))
                {
                    if (noTrace) continue;
                    else line = line.Substring("DEBUG:".Length).Trim();
                }
                if (line.StartsWith("RELEASE:", StringComparison.Ordinal))
                {
                    if (!noTrace) continue;
                    else line = line.Substring("RELEASE:".Length).Trim();
                }
                // expand variables
                foreach (BuildEventInfo info in infos)
                    line = line.Replace(info.FormattedName, info.Value);
                // bin/debug output path
                line = project.FixDebugReleasePath(line);

                Console.WriteLine("cmd: " + line);
                string[] tokens = tokenize(line);
                string command = tokens[0];
                string args = tokens[1];

                //switch command and act on "recognized" commands
                switch (command)
                {
                    case "BuildProject":
                        FDBuild.Program.BuildProject(args);
                        break;

                    case "RunMxmlc":
                        string[] mxmlctokens = tokenize(args);
                        FDBuild.Program.BuildMXMLC(mxmlctokens[0], mxmlctokens[1]);
                        break;

                    case "RunCompc": 
                        string[] compctokens = tokenize(args);
                        FDBuild.Program.BuildCOMPC(compctokens[0], compctokens[1]);
                        break;

                    default:
                        if (!ProcessRunner.Run(command, args, false, false))
                            throw new BuildException("Build halted with errors.");
                        break;
                }
            }
        }
    }
}
