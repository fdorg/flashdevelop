﻿// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using System;
using System.IO;
using PluginCore.Managers;

namespace SourceControl.Sources.Git
{
    class MoveCommand : BaseCommand
    {
        public MoveCommand(string fromPath, string toPath)
        {
            // directly move empty dirs
            if (Directory.Exists(fromPath) && Directory.GetFiles(fromPath).Length == 0)
            {
                toPath = Path.Combine(toPath, Path.GetFileName(fromPath));
                if (Directory.Exists(toPath)) return;
                try { Directory.Move(fromPath, toPath); }
                catch (Exception ex) { ErrorManager.ShowInfo(ex.Message); }
                return;
            }

            string args = String.Format("mv \"{0}\" \"{1}\"", Path.GetFileName(fromPath), toPath);

            Run(args, Path.GetDirectoryName(fromPath));
        }
    }
}
