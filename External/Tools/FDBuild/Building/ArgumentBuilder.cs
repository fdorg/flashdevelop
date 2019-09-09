// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System;
using System.Collections.Generic;

namespace ProjectManager.Building
{
    class ArgumentBuilder
    {
        readonly List<string> args;

        public ArgumentBuilder()
        {
            args = new List<string>();
        }

        public void Add(string[] arguments, bool releaseMode)
        {
            foreach (var argument in arguments)
                if (!string.IsNullOrEmpty(argument))
                {
                    var line = argument.Trim();
                    if (line.Length == 0) continue;
                    // conditional arguments
                    if (line.StartsWith("DEBUG:", StringComparison.Ordinal))
                    {
                        if (releaseMode) continue;
                        line = line.Substring("DEBUG:".Length).Trim();
                    }
                    if (line.StartsWith("RELEASE:", StringComparison.Ordinal))
                    {
                        if (!releaseMode) continue;
                        line = line.Substring("RELEASE:".Length).Trim();
                    }

                    args.Add(line);
                }
        }

        public void Add(string argument, params string[] values)
        {
            args.Add(argument);
            foreach (var value in values)
                if (!string.IsNullOrEmpty(value)) args.Add(value);
        }

        public override string ToString() => string.Join(" ", args);
    }
}