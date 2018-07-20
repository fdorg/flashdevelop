// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using System;
using System.Collections;

namespace ProjectManager.Building
{
    class ArgumentBuilder
    {
        ArrayList args;

        public ArgumentBuilder()
        {
            args = new ArrayList();
        }

        public void Add(string[] arguments, bool releaseMode)
        {
            foreach (string argument in arguments)
                if (argument != null && argument.Length > 0)
                {
                    string line = argument.Trim();
                    if (line.Length == 0) 
                        continue;
                    // conditional arguments
                    if (line.StartsWith("DEBUG:", StringComparison.Ordinal))
                    {
                        if (releaseMode) continue;
                        else line = line.Substring("DEBUG:".Length).Trim();
                    }
                    if (line.StartsWith("RELEASE:", StringComparison.Ordinal))
                    {
                        if (!releaseMode) continue;
                        else line = line.Substring("RELEASE:".Length).Trim();
                    }

                    args.Add(line);
                }
        }

        public void Add(string argument, params string[] values)
        {
            args.Add(argument);
            foreach (string value in values)
                if (value != null && value.Length > 0) args.Add(value);
        }

        public override string ToString()
        {
            string[] argArray = args.ToArray(typeof(string)) as string[];
            string line = string.Join(" ", argArray);
            return line;
        }
    }
}
