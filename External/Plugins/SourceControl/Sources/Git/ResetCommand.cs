﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++, C#, and Java: http://www.viva64.com
using System.Collections.Generic;
using System.IO;

namespace SourceControl.Sources.Git
{
    internal class ResetCommand : BaseCommand
    {
        readonly string args;
        readonly string dir;

        public ResetCommand(IReadOnlyList<string> paths)
        {
            args = "reset ";
            foreach (var path in paths)
                args += " \"" + Path.GetFileName(path) + "\"";

            dir = Path.GetDirectoryName(paths[0]);
        }

        public override void Run() => Run(args, dir);
    }
}
